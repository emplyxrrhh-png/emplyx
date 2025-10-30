Imports Robotics.Base.DTOs
Imports Robotics.Base.VTRequests.Requests

Namespace API

    Public NotInheritable Class RequestServiceMethods

#Region "Request"

        Public Shared Function GetRequestsByEmployee(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, Optional ByVal _Filter As String = "",
                                                     Optional ByVal _Order As String = "") As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.RequestState

            WebServiceHelper.SetState(oState)

            Try

                'oSession.AccessApi.WebServices.RequestSvc.Timeout = System.Threading.Timeout.Infinite

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.RequestMethods.GetRequestsByEmployee(_IDEmployee, _Filter, _Order, oState)

                oSession.States.RequestState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.RequestState.Result = RequestResultEnum.NoError Then
                    If response.Value.Tables.Count > 0 Then
                        oRet = response.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.RequestState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-516")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Devuelve un Datatable con todos las solicitudes a las que tiene acceso un supervisor o validador
        ''' </summary>
        ''' <param name="_IDPassport">ID de passaporte del usuario supervisor</param>
        ''' <param name="_Filter">Filtro SQL para el Where (ejemplo: 'RequestType = 1 And Reque...')</param>
        ''' <param name="_Order">Ordenación SQL (ejemplo: 'RequestType ASC' o 'RequestDate ASC')</param>
        ''' <param name="_Audit">Auditar consulta masiva</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetRequestsSupervisor(ByVal oPage As System.Web.UI.Page, ByVal _IDPassport As Integer, Optional ByVal _Filter As String = "",
                                                     Optional ByVal _Order As String = "", Optional ByVal _Audit As Boolean = True,
                                                     Optional ByVal NumRequestToLoad As Integer = 0, Optional ByVal LevelsBelow As String = "", Optional ByVal IdCause As Integer = 0, Optional ByVal IdSupervisor As Integer = 0, Optional ByVal bIncludeAutomaticRequests As Boolean = False) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.RequestState

            WebServiceHelper.SetState(oState)

            Try

                'oSession.AccessApi.WebServices.RequestService.Timeout = System.Threading.Timeout.Infinite

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.RequestMethods.GetRequestsSupervisor(_IDPassport, _Filter, _Order, NumRequestToLoad, LevelsBelow, IdCause, IdSupervisor, bIncludeAutomaticRequests, _Audit, oState)

                oSession.States.RequestState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.RequestState.Result = RequestResultEnum.NoError Then
                    If response.Value IsNot Nothing AndAlso response.Value.Tables.Count > 0 Then
                        oRet = response.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.RequestState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-517")
            End Try

            Return oRet

        End Function

        Public Shared Function GetRequestsDashboardResume(ByVal oPage As System.Web.UI.Page, ByVal _IDPassport As Integer, Optional ByVal _Filter As String = "") As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.RequestState

            WebServiceHelper.SetState(oState)

            Try

                'VTLiveApi.RequestMethods.Timeout = System.Threading.Timeout.Infinite

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.RequestMethods.GetRequestsDashboardResume(_IDPassport, _Filter, oState)

                oSession.States.RequestState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.RequestState.Result = RequestResultEnum.NoError Then
                    If response.Value IsNot Nothing AndAlso response.Value.Tables.Count > 0 Then
                        oRet = response.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.RequestState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-518")
            End Try

            Return oRet

        End Function

        Public Shared Function GetRequestPendingSupervisors(ByVal oPage As System.Web.UI.Page, ByVal intIDRequest As Integer, ByVal bAudit As Boolean) As String
            Dim oRet As String = String.Empty

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.RequestState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of String) = VTLiveApi.RequestMethods.GetRequestPendingSupervisors(intIDRequest, oState, bAudit)
                oRet = response.Value

                oSession.States.RequestState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.RequestState.Result <> RequestResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.RequestState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-520")
            End Try

            Return oRet
        End Function

        Public Shared Function GetRequestByID(ByVal oPage As System.Web.UI.Page, ByVal intIDRequest As Integer, ByVal bAudit As Boolean) As roRequest

            Dim oRet As roRequest = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.RequestState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of roRequest) = VTLiveApi.RequestMethods.GetRequestByID(intIDRequest, oState, bAudit)
                oRet = response.Value

                oSession.States.RequestState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.RequestState.Result <> RequestResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.RequestState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-521")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Guarda la solicitud
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="oRequest"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function SaveRequest(ByVal oPage As System.Web.UI.Page, ByRef oRequest As roRequest, ByVal bAudit As Boolean) As Boolean
            Dim oRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.RequestState

            WebServiceHelper.SetState(oState)

            Try
                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.RequestMethods.SaveRequest(oRequest, oState, bAudit)
                oRet = response.Value

                oSession.States.RequestState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.RequestState.Result <> RequestResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.RequestState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-522")
            End Try

            Return oRet
        End Function

        ''' <summary>
        ''' Elimina la solicitud (si esta pendiente)
        ''' </summary>
        ''' <param name="oPage">Página web donde mostrar si hay errores</param>
        ''' <param name="ID">ID de la solicitud a eliminar</param>
        ''' <returns>Devuelve TRUE si se ha podido eliminar correctamente</returns>
        ''' <remarks></remarks>
        Public Shared Function DeleteRequest(ByVal oPage As System.Web.UI.Page, ByVal ID As Integer, ByVal bAudit As Boolean) As Boolean

            Dim oRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.RequestState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.RequestMethods.DeleteRequest(ID, oState, bAudit)
                oRet = response.Value

                oSession.States.RequestState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.RequestState.Result <> RequestResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.RequestState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-523")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Comprueba si los datos són validos
        ''' </summary>
        ''' <param name="oPage">Página web donde mostrar si hay errores</param>
        ''' <param name="oRequest">solicitud a validar</param>
        ''' <returns>Devuelve TRUE si los datos són correctos</returns>
        ''' <remarks></remarks>
        Public Shared Function ValidateRequest(ByVal oPage As System.Web.UI.Page, ByVal oRequest As roRequest) As Boolean

            Dim oRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.RequestState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.RequestMethods.ValidateRequest(oRequest, oState)
                oRet = response.Value

                oSession.States.RequestState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.RequestState.Result <> RequestResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.RequestState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-524")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Devuelve la definición de seguridad para un tipo de solicitud.
        ''' </summary>
        ''' <param name="oPage">Página desde la que se hace la petición.</param>
        ''' <param name="eRequestType">Tipo de solicitud</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetRequestTypeSecurity(ByVal oPage As System.Web.UI.Page, ByVal eRequestType As eRequestType) As roRequestTypeSecurity

            Dim oRet As roRequestTypeSecurity = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.RequestState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of roRequestTypeSecurity) = VTLiveApi.RequestMethods.GetRequestTypeSecurity(eRequestType, oState)
                oRet = response.Value

                oSession.States.RequestState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.RequestState.Result <> RequestResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.RequestState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-525")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Devuelve la lista de definiciones de seguridad para todos los tipos de solicitud.
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetRequestTypeSecurityListAll(ByVal oPage As System.Web.UI.Page) As Generic.List(Of roRequestTypeSecurity)

            Dim oRet As Generic.List(Of roRequestTypeSecurity) = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.RequestState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of List(Of roRequestTypeSecurity)) = VTLiveApi.RequestMethods.GetRequestTypeSecurityListAll(oState)

                oSession.States.RequestState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.RequestState.Result = RequestResultEnum.NoError Then
                    oRet = response.Value
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.RequestState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-526")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Devuelve la lista de definiciones de seguridad para los tipo de solicitud indicados.
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="RequestTypes"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetRequestTypeSecurityList(ByVal oPage As System.Web.UI.Page, ByVal RequestTypes() As eRequestType) As Generic.List(Of roRequestTypeSecurity)

            Dim oRet As Generic.List(Of roRequestTypeSecurity) = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.RequestState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of List(Of roRequestTypeSecurity)) = VTLiveApi.RequestMethods.GetRequestTypeSecurityList(RequestTypes, oState)

                oSession.States.RequestState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.RequestState.Result = RequestResultEnum.NoError Then
                    oRet = response.Value
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.RequestState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-527")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Obtiene la lista de tipos de solicitudes con sus identificadores y descripciones
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetRequestTypes(ByVal oPage As System.Web.UI.Page) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.RequestState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.RequestMethods.GetRequestTypes(oState)

                oSession.States.RequestState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.RequestState.Result = RequestResultEnum.NoError Then
                    If response.Value.Tables.Count > 0 Then
                        oRet = response.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.RequestState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-528")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Obtiene la lista de estados de solicitudes con sus identificadores y descripciones
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetRequestStates(ByVal oPage As System.Web.UI.Page) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.RequestState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.RequestMethods.GetRequestStates(oState)

                oSession.States.RequestState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.RequestState.Result = RequestResultEnum.NoError Then
                    If response.Value.Tables.Count > 0 Then
                        oRet = response.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.RequestState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-529")
            End Try

            Return oRet

        End Function

        Public Shared Function ApproveRefuse(ByVal oPage As System.Web.UI.Page, ByVal _IDRequest As Integer, ByVal _IDPassport As Integer, ByVal _ApproveRefuse As Boolean, ByVal _Comments As String, Optional ByVal _CheckLockedDays As Boolean = False, Optional ByVal _ForceApprove As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.RequestState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.RequestMethods.ApproveRefuse(_IDRequest, _IDPassport, _ApproveRefuse, _Comments, _CheckLockedDays, _ForceApprove, oState)
                bolRet = response.Value

                oSession.States.RequestState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.RequestState.Result <> RequestResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.RequestState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-530")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Obtiene la descripción del contenido de la solicitud
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="_IDRequest">Número de la solicitud</param>
        ''' <param name="bolDetail">Para obtener la información detallada o no</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetRequestInfo(ByVal oPage As System.Web.UI.Page, ByVal _IDRequest As Integer, ByVal bolDetail As Boolean) As String

            Dim strRet As String = ""

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.RequestState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of String) = VTLiveApi.RequestMethods.GetRequestInfo(_IDRequest, bolDetail, oState)
                strRet = response.Value

                oSession.States.RequestState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.RequestState.Result <> RequestResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.RequestState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-531")
            End Try

            Return strRet

        End Function

        Public Shared Function GetFilterRequests(ByVal oPage As System.Web.UI.Page, ByVal IdPassport As Integer) As String

            Dim strRet As String = ""

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.RequestState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of String) = VTLiveApi.RequestMethods.GetFilterRequests(IdPassport, oState)
                strRet = response.Value

                oSession.States.RequestState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.RequestState.Result <> RequestResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.RequestState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-532")
            End Try

            Return strRet

        End Function

        Public Shared Function SetFilterRequests(ByVal oPage As System.Web.UI.Page, ByVal IdPassport As Integer, ByVal Filter As String) As Boolean
            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.RequestState

            WebServiceHelper.SetState(oState)

            Try
                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.RequestMethods.SetFilterRequests(IdPassport, Filter, oState)
                bolRet = response.Value

                oSession.States.RequestState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.RequestState.Result <> RequestResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.RequestState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-533")
            End Try

            Return bolRet
        End Function

#End Region

        Public Shared Function LastErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.RequestState.ErrorText
            End If
            Return strRet
        End Function

    End Class

End Namespace