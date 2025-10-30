Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Cause

Namespace API

    Public NotInheritable Class CausesServiceMethods

        Public Shared Function GetCauses(ByVal oPage As System.Web.UI.Page, Optional ByVal strWhere As String = "", Optional ByVal bolFilterBusinessGroups As Boolean = False) As DataTable
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.CauseState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.CausesMethods.GetCauses(strWhere, bolFilterBusinessGroups, oState)

                oSession.States.AccessGroupState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oState.Result = CauseResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                Dim oLanguage As New roLanguageWeb
                oTmpState.Result = 1
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-084")
            End Try

            Return oRet

        End Function

        Public Shared Function GetCausesByEmployeeInputPermissions(ByVal oPage As System.Web.UI.Page, ByVal intIDEmployee As Integer, ByVal bolFilterBusinessGroups As Boolean) As DataTable
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.CauseState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.CausesMethods.GetCausesByEmployeeInputPermissions(intIDEmployee, bolFilterBusinessGroups, oState)

                oSession.States.AccessGroupState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oState.Result = CauseResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                Dim oLanguage As New roLanguageWeb
                oTmpState.Result = 1
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-085")
            End Try

            Return oRet

        End Function

        Public Shared Function GetCausesByEmployeeVisibilityPermissions(ByVal oPage As System.Web.UI.Page, ByVal intIDEmployee As Integer) As DataTable '//////////Probablemente la función está en desuso.\\\\\\\\\\
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.CauseState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.CausesMethods.GetCausesByEmployeeVisibilityPermissions(intIDEmployee, oState)

                oSession.States.AccessGroupState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oState.Result = CauseResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                Dim oLanguage As New roLanguageWeb
                oTmpState.Result = 1
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-086")
            End Try

            Return oRet

        End Function

        Public Shared Function GetCausesShortList(ByVal oPage As System.Web.UI.Page, Optional ByVal bolFilterBusinessGroups As Boolean = False) As DataTable
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.CauseState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.CausesMethods.GetCausesShortList(bolFilterBusinessGroups, oState)

                oSession.States.AccessGroupState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oState.Result = CauseResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                Dim oLanguage As New roLanguageWeb
                oTmpState.Result = 1
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-087")
            End Try

            Return oRet

        End Function

        Public Shared Function GetCausesShortListByRequestType(ByVal oPage As System.Web.UI.Page, ByVal eRequestType As eCauseRequest, Optional ByVal bolFilterBusinessGroups As Boolean = False) As DataTable
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.CauseState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.CausesMethods.CausesShortListByRequestType(eRequestType, bolFilterBusinessGroups, oState)

                oSession.States.AccessGroupState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oState.Result = CauseResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                Dim oLanguage As New roLanguageWeb
                oTmpState.Result = 1
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-087")
            End Try

            Return oRet

        End Function

        Public Shared Function GetBusinessGroupFromCauseGroups(ByVal oPage As System.Web.UI.Page) As DataTable
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.CauseState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.CausesMethods.GetBusinessGroupFromCauseGroups(oState)

                oSession.States.AccessGroupState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oState.Result = CauseResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                Dim oLanguage As New roLanguageWeb
                oTmpState.Result = 1
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-088")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Recupera una justificación por ID
        ''' </summary>
        ''' <param name="oPage">Página donde mostrar los errores</param>
        ''' <param name="intIDCause">ID de justificación a recuperar</param>
        ''' <returns>Devuelve un roCause</returns>
        ''' <remarks></remarks>
        Public Shared Function GetCauseByID(ByVal oPage As System.Web.UI.Page, ByVal intIDCause As Integer, ByVal bAudit As Boolean) As roCause
            Dim oRet As roCause = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.CauseState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of roCause) = VTLiveApi.CausesMethods.GetCauseByID(intIDCause, oState, bAudit)

                oSession.States.CauseState = wsRet.Status
                roWsUserManagement.SessionObject = oSession
                oRet = wsRet.Value

                If oState.Result <> CauseResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-089")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Guardar justificación
        ''' </summary>
        ''' <param name="oPage">Página web donde mostrar si hay errores</param>
        ''' <param name="oCause">Justificación a guardar</param>
        ''' <returns>Devuelve TRUE si se puede grabar correctamente</returns>
        ''' <remarks></remarks>
        Public Shared Function SaveCause(ByVal oPage As System.Web.UI.Page, ByRef oCause As roCause, ByVal bAudit As Boolean) As Boolean
            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.CauseState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of roCause) = VTLiveApi.CausesMethods.SaveCause(oCause, oState, bAudit)

                oSession.States.CauseState = wsRet.Status
                roWsUserManagement.SessionObject = oSession
                If oState.Result <> CauseResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oState)
                End If

                If wsRet.Value IsNot Nothing Then
                    bolRet = True
                    oCause = wsRet.Value
                Else
                    bolRet = False
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-090")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Eliminar justificación
        ''' </summary>
        ''' <param name="oPage">Página web donde mostrar si hay errores</param>
        ''' <param name="IDCause">ID de Justificación a eliminar</param>
        ''' <returns>Devuelve TRUE si se ha podido eliminar correctamente</returns>
        ''' <remarks></remarks>
        Public Shared Function DeleteCause(ByVal oPage As System.Web.UI.Page, ByVal IDCause As Integer, ByVal bAudit As Boolean) As Boolean
            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.CauseState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.CausesMethods.DeleteCause(IDCause, oState, bAudit)

                oSession.States.CauseState = wsRet.Status
                roWsUserManagement.SessionObject = oSession
                bolRet = wsRet.Value

                If oState.Result <> CauseResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-091")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Comprobar si se esta utilizando la justificacion.
        ''' </summary>
        ''' <param name="oPage">Página web donde mostrar si hay errores</param>
        ''' <param name="IDCause">ID de Justificación a comprobar</param>
        ''' <returns>Devuelve TRUE si se encuentra en uso</returns>
        ''' <remarks></remarks>
        Public Shared Function CauseIsUsed(ByVal oPage As System.Web.UI.Page, ByVal IDCause As Integer) As Boolean '//////////Probablemente la función está en desuso.\\\\\\\\\\
            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.CauseState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.CausesMethods.CauseIsUsed(IDCause, oState)

                oSession.States.CauseState = wsRet.Status
                roWsUserManagement.SessionObject = oSession
                bolRet = wsRet.Value

                If oState.Result <> CauseResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-092")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Comprueba si los datos de la justificación són validos
        ''' </summary>
        ''' <param name="oPage">Página web donde mostrar si hay errores</param>
        ''' <param name="oCause">Justificación a validar</param>
        ''' <returns>Devuelve TRUE si los datos són correctos</returns>
        ''' <remarks></remarks>
        Public Shared Function ValidateCause(ByVal oPage As System.Web.UI.Page, ByVal oCause As roCause) As Boolean
            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.CauseState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.CausesMethods.ValidateCause(oCause, oState)

                oSession.States.CauseState = wsRet.Status
                roWsUserManagement.SessionObject = oSession
                bolRet = wsRet.Value

                If oState.Result <> CauseResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-093")
            End Try

            Return bolRet

        End Function

        Public Shared Function LastErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.CauseState.ErrorText
            End If
            Return strRet
        End Function

    End Class

End Namespace