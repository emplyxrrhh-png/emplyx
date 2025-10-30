Imports Robotics.Base.DTOs
Imports Robotics.Security
Imports Robotics.VTBase.Extensions

Namespace API

    Public NotInheritable Class ConnectorServiceMethods

        Public Shared Function LaunchBroadcaster(ByVal oPage As System.Web.UI.Page) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ConnectorState

            WebServiceHelper.SetState(oState)

            Try

                Dim strParamsXML As String = "<?xml version=""1.0""?><roCollection version=""2.0""><Item key=""ON_CHANGE_MANUALLY"" type=""11"">True</Item></roCollection>"

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.ConnectorMethods.InitTaskWithParams(TasksType.BROADCASTER, strParamsXML, oState)

                oSession.States.ConnectorState = wsRet.Status
                roWsUserManagement.SessionObject = oSession
                bolRet = wsRet.Value

                If oSession.States.ConnectorState.Result <> ConnectorResultEnum.NoError Then
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ConnectorState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-115")
            End Try

            Return bolRet

        End Function

        Public Shared Function LaunchBroadcasterForTerminal(ByVal oPage As System.Web.UI.Page, ByVal IDTerminal As Integer) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ConnectorState

            WebServiceHelper.SetState(oState)

            Try

                Dim strParamsXML As String = "<?xml version=""1.0""?><roCollection version=""2.0""><Item key=""Command"" type=""8"">RESET_TERMINAL</Item>" &
                                               "<Item key=""IDTerminal"" type=""2"">" + IDTerminal.ToString + "</Item></roCollection>"

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.ConnectorMethods.InitTaskWithParams(TasksType.BROADCASTER, strParamsXML, oState)

                oSession.States.ConnectorState = wsRet.Status
                roWsUserManagement.SessionObject = oSession
                bolRet = wsRet.Value

                If oSession.States.ConnectorState.Result <> ConnectorResultEnum.NoError Then
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ConnectorState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-116")
            End Try

            Return bolRet

        End Function

        Public Shared Function LaunchBroadcasterForTerminalTask(ByVal oPage As System.Web.UI.Page, ByVal IDTerminal As Integer, ByVal TerminalTask As String) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ConnectorState

            WebServiceHelper.SetState(oState)

            Try

                Dim strParamsXML As String = "<?xml version=""1.0""?><roCollection version=""2.0"">"
                strParamsXML += "<Item key=""Command"" type=""8"">ON_ADD_TASK</Item>"
                strParamsXML += "<Item key=""IDTerminal"" type=""2"">" + IDTerminal.ToString + "</Item>"
                strParamsXML += "<Item key=""TerminalsTask"" type=""8"">" + TerminalTask + "</Item>"
                strParamsXML += "</roCollection>"

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.ConnectorMethods.InitTaskWithParams(TasksType.BROADCASTER, strParamsXML, oState)

                oSession.States.ConnectorState = wsRet.Status
                roWsUserManagement.SessionObject = oSession
                bolRet = wsRet.Value

                If oSession.States.ConnectorState.Result <> ConnectorResultEnum.NoError Then
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ConnectorState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-117")
            End Try

            Return bolRet

        End Function

        Public Shared Function GetFirstDate(ByVal oPage As System.Web.UI.Page) As Date

            Dim oRet As Date = New Date(1900, 1, 1)

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ConnectorState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Object) = VTLiveApi.ConnectorMethods.GetParameter(Parameters.FirstDate, oState)

                If IsDate(wsRet.Value) Then
                    oRet = CDate(wsRet.Value)
                End If

                oSession.States.ConnectorState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ConnectorState.Result <> ConnectorResultEnum.NoError Then
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ConnectorState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-118")
            End Try

            Return oRet

        End Function

        Public Shared Function GetParameters(ByVal oPage As System.Web.UI.Page) As roParameters

            Dim oRet As roParameters = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ConnectorState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of roParameters) = VTLiveApi.ConnectorMethods.GetParameters(oState)

                oSession.States.ConnectorState = wsRet.Status
                roWsUserManagement.SessionObject = oSession
                oRet = wsRet.Value

                If oSession.States.ConnectorState.Result <> ConnectorResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ConnectorState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-119")
            End Try

            Return oRet

        End Function

        '        Public Shared Function GetParametersByKey(ByVal oPage As System.Web.UI.Page, ByVal strKey As String) As ConnectorService.roParameters

        '            Dim oRet As ConnectorService.roParameters = Nothing

        '            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
        '            Dim oState As wscConnectorState = oSession.States.ConnectorState

        '            WebServiceHelper.SetState(oState)

        '            Try

        '                oRet = VTLiveApi.ConnectorMethods.GetParametersByKey(oState, strKey)

        '                oSession.States.ConnectorState = oState
        '                roWsUserManagement.SessionObject = oSession

        '                If oState.Result <> ConnectorService.ResultEnum.NoError Then
        '                    ' Mostrar el error
        '                    HelperWeb.ShowError(oPage, oState)
        '                End If

        '            Catch ex As Exception
        '                Dim oTmpState As New Robotics.Base.DTOs.roWsState
        '                oTmpState.Result = 1
        '                Dim oLanguage As New roLanguageWeb
        'oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
        '                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-120")
        '            End Try

        '            Return oRet

        '        End Function

        Public Shared Function GetParameter(ByVal oPage As System.Web.UI.Page, ByVal Parameter As Parameters) As Object

            Dim oRet As Object = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ConnectorState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Object) = VTLiveApi.ConnectorMethods.GetParameter(Parameter, oState)

                oSession.States.ConnectorState = wsRet.Status
                roWsUserManagement.SessionObject = oSession
                oRet = wsRet.Value

                If oSession.States.ConnectorState.Result <> ConnectorResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ConnectorState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-121")
            End Try

            Return oRet

        End Function

        Public Shared Function ServerIsRunning(ByVal oPage As System.Web.UI.Page, ByVal oPassportTicket As roPassportTicket) As Boolean
            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ConnectorState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Object) = VTLiveApi.ConnectorMethods.GetSetting(eKeys.Running, oState)

                bolRet = CBool(wsRet.Value)

                oSession.States.ConnectorState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ConnectorState.Result <> ConnectorResultEnum.NoError Then
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ConnectorState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-122")
            End Try

            Return bolRet

        End Function

        Public Shared Function GetSetting(ByVal oPage As System.Web.UI.Page, ByVal oKey As eKeys) As Object

            Dim oRet As Object = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ConnectorState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Object) = VTLiveApi.ConnectorMethods.GetSetting(oKey, oState)

                oSession.States.ConnectorState = wsRet.Status
                roWsUserManagement.SessionObject = oSession
                oRet = wsRet.Value

                If oSession.States.ConnectorState.Result <> ConnectorResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ConnectorState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-123")
            End Try

            Return oRet

        End Function

        '        Public Shared Function SetSetting(ByVal oPage As System.Web.UI.Page, ByVal oKey As ConnectorService.eKeys, ByVal strValue As String) As Boolean

        '            Dim bolRet As Boolean = False

        '            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
        '            Dim oState As wscConnectorState = oSession.States.ConnectorState

        '            WebServiceHelper.SetState(oState)

        '            Try

        '                bolRet = VTLiveApi.ConnectorMethods.SetSetting(oKey, strValue, oState)

        '                oSession.States.ConnectorState = oState
        '                roWsUserManagement.SessionObject = oSession

        '                If oState.Result <> ConnectorService.ResultEnum.NoError Then
        '                    ' Mostrar el error
        '                    HelperWeb.ShowError(oPage, oState)
        '                End If
        '            Catch ex As Exception
        '                Dim oTmpState As New Robotics.Base.DTOs.roWsState
        '                oTmpState.Result = 1
        '                Dim oLanguage As New roLanguageWeb
        'oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
        '                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-124")
        '            End Try

        '            Return bolRet

        '        End Function

        Public Shared Function SaveParameters(ByVal oPage As System.Web.UI.Page, ByVal oParameters As roParameters,
                                              ByVal bAudit As Boolean) As Boolean
            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ConnectorState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.ConnectorMethods.SaveParameters(oParameters, oState, bAudit)

                oSession.States.ConnectorState = wsRet.Status
                roWsUserManagement.SessionObject = oSession
                bolRet = wsRet.Value

                If oSession.States.ConnectorState.Result <> ConnectorResultEnum.NoError Then
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ConnectorState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-125")
            End Try

            Return bolRet

        End Function

        '        Public Shared Function SaveParametersByKey(ByVal oPage As System.Web.UI.Page, ByVal oParameters As ConnectorService.roParameters, ByVal strKey As String, ByVal bAudit As Boolean) As Boolean

        '            Dim bolRet As Boolean = False

        '            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
        '            Dim oState As wscConnectorState = oSession.States.ConnectorState

        '            WebServiceHelper.SetState(oState)

        '            Try

        '                bolRet = VTLiveApi.ConnectorMethods.SaveParametersByKey(oParameters, strKey, oState, bAudit)

        '                oSession.States.ConnectorState = oState
        '                roWsUserManagement.SessionObject = oSession

        '                If oState.Result <> ConnectorService.ResultEnum.NoError Then
        '                    ' Mostrar el error
        '                    HelperWeb.ShowError(oPage, oState)
        '                End If

        '            Catch ex As Exception
        '                Dim oTmpState As New Robotics.Base.DTOs.roWsState
        '                oTmpState.Result = 1
        '                Dim oLanguage As New roLanguageWeb
        'oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
        '                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-126")
        '            End Try

        '            Return bolRet

        '        End Function

        Public Shared Function LastErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.ConnectorState.ErrorText
            End If
            Return strRet
        End Function

    End Class

End Namespace