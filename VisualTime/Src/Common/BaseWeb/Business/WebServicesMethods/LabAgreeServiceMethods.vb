Imports Robotics.Base.DTOs
Imports Robotics.Base.VTEmployees.LabAgree

Namespace API

    Public NotInheritable Class LabAgreeServiceMethods

#Region "LabAgree"

        Public Shared Function GetLabAgrees(ByVal oPage As System.Web.UI.Page) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.LabAgreeState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.LabAgreesMethods.GetLabAgrees(oState)

                oSession.States.LabAgreeState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.LabAgreeState.Result = LabAgreeResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.LabAgreeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-363")
            End Try

            Return oRet

        End Function

        Public Shared Function GetLabAgreeByID(ByVal oPage As System.Web.UI.Page, ByVal intIDLabAgree As Integer, ByVal bAudit As Boolean) As roLabAgree

            Dim oRet As roLabAgree = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.LabAgreeState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of roLabAgree) = VTLiveApi.LabAgreesMethods.GetLabAgreeByID(intIDLabAgree, oState, bAudit)

                oSession.States.LabAgreeState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.LabAgreeState.Result <> LabAgreeResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.LabAgreeState)
                Else
                    oRet = wsRet.Value
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-364")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Guarda el convenio
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="oLabAgree"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function SaveLabAgree(ByVal oPage As System.Web.UI.Page, ByRef oLabAgree As roLabAgree, ByVal bAudit As Boolean) As Boolean
            Dim oRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.LabAgreeState

            WebServiceHelper.SetState(oState)

            Try
                ' VTLiveApi.LabAgreesMethods.Timeout = System.Threading.Timeout.Infinite
                Dim wsRet As roGenericVtResponse(Of Tuple(Of Boolean, roLabAgree)) = VTLiveApi.LabAgreesMethods.SaveLabAgree(oLabAgree, oState, bAudit)

                oRet = wsRet.Value.Item1
                oLabAgree = wsRet.Value.Item2

                oSession.States.LabAgreeState = wsRet.Status
                roWsUserManagement.SessionObject = oSession
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-365")
            End Try

            Return oRet
        End Function

        ''' <summary>
        ''' Elimina el convenio
        ''' </summary>
        ''' <param name="oPage">Página web donde mostrar si hay errores</param>
        ''' <param name="ID">ID del convenio a eliminar</param>
        ''' <returns>Devuelve TRUE si se ha podido eliminar correctamente</returns>
        ''' <remarks></remarks>
        Public Shared Function DeleteLabAgree(ByVal oPage As System.Web.UI.Page, ByVal ID As Integer, ByVal bAudit As Boolean) As Boolean

            Dim oRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.LabAgreeState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.LabAgreesMethods.DeleteLabAgree(ID, oState, bAudit)

                oSession.States.LabAgreeState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.LabAgreeState.Result <> LabAgreeResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.LabAgreeState)
                Else
                    oRet = wsRet.Value
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-366")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Comprueba si los datos del convenio són validos
        ''' </summary>
        ''' <param name="oPage">Página web donde mostrar si hay errores</param>
        ''' <param name="oLabAgree">convenio a validar</param>
        ''' <returns>Devuelve TRUE si los datos són correctos</returns>
        ''' <remarks></remarks>
        Public Shared Function ValidateLabAgree(ByVal oPage As System.Web.UI.Page, ByVal oLabAgree As roLabAgree) As Boolean

            Dim oRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.LabAgreeState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.LabAgreesMethods.ValidateLabAgree(oLabAgree, oState)

                oSession.States.LabAgreeState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.LabAgreeState.Result <> LabAgreeResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.LabAgreeState)
                Else
                    oRet = wsRet.Value
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-367")
            End Try

            Return oRet

        End Function

        Public Shared Function ValidateLabAgreeDailyCausesOnDate(ByVal oPage As System.Web.UI.Page, ByVal IDEmployee As Integer, ByVal valDate As DateTime, ByVal valIdCause() As Integer, ByVal valValue() As Double) As Boolean
            Dim oRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.LabAgreeState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.LabAgreesMethods.ValidateLabAgreeDailyCausesOnDate(IDEmployee, valDate, valIdCause, valValue, oState)

                oSession.States.LabAgreeState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.LabAgreeState.Result <> LabAgreeResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.LabAgreeState)
                Else
                    oRet = wsRet.Value
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-368")
            End Try

            Return oRet
        End Function

#End Region

#Region "LabAgreeRule"

        Public Shared Function GetLabAgreeRuleByID(ByVal oPage As System.Web.UI.Page, ByVal intIDLabAgreeRule As Integer, ByVal bAudit As Boolean) As roLabAgreeRule

            Dim oRet As roLabAgreeRule = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.LabAgreeState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of roLabAgreeRule) = VTLiveApi.LabAgreesMethods.GetLabAgreeRuleByID(intIDLabAgreeRule, oState, bAudit)

                oSession.States.LabAgreeState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.LabAgreeState.Result <> LabAgreeResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.LabAgreeState)
                Else
                    oRet = wsRet.Value
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-369")
            End Try

            Return oRet

        End Function

#End Region

#Region "StartupValue"

        Public Shared Function GetStartupValueByID(ByVal oPage As System.Web.UI.Page, ByVal intIDConcept As Integer, ByVal bAudit As Boolean) As roStartupValue

            Dim oRet As roStartupValue = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.LabAgreeState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of roStartupValue) = VTLiveApi.LabAgreesMethods.GetStartupValueByID(intIDConcept, oState, bAudit)

                oSession.States.LabAgreeState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.LabAgreeState.Result <> LabAgreeResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.LabAgreeState)
                Else
                    oRet = wsRet.Value
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-370")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Retorna un Datatable amb valors inicials que contenen el camp de la fitxa
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="strFieldName">Camp de la fitxa</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetStartupValuesUseUserField(ByVal oPage As System.Web.UI.Page, ByVal strFieldName As String) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.LabAgreeState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.LabAgreesMethods.GetStartupValuesUseUserField(strFieldName, oState)

                oSession.States.LabAgreeState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.LabAgreeState.Result = LabAgreeResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.LabAgreeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-371")
            End Try

            Return oRet

        End Function

#End Region

        ''' <summary>
        ''' Retorna un Datatable con los tipos de reglas de solicitudes en funcion del tipo de solicitud
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetAvailableRequestType(ByVal oPage As System.Web.UI.Page) As DataTable
            Dim oRet As DataTable = Nothing
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.LabAgreeState
            WebServiceHelper.SetState(oState)
            Try
                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.LabAgreesMethods.GetAvailableRequestType(oState)

                oSession.States.LabAgreeState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.LabAgreeState.Result = LabAgreeResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.LabAgreeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-372")
            End Try
            Return oRet
        End Function

        ''' <summary>
        ''' Retorna un Datatable con los tipos de reglas de solicitudes en funcion del tipo de solicitud
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="RequestType">Camp de la fitxa</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetRuleTypesByRequestType(ByVal oPage As System.Web.UI.Page, ByVal RequestType As Integer) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.LabAgreeState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.LabAgreesMethods.GetRuleTypesByRequestType(RequestType, oState)

                oSession.States.LabAgreeState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.LabAgreeState.Result = LabAgreeResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.LabAgreeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-373")
            End Try
            Return oRet

        End Function

        Public Shared Function LastErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.LabAgreeState.ErrorText
            End If
            Return strRet
        End Function

        Public Shared Function LastError() As roWsState
            Dim oState As New roWsState
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                oState = roWsUserManagement.SessionObject().States.LabAgreeState
            End If
            Return oState
        End Function

    End Class

End Namespace