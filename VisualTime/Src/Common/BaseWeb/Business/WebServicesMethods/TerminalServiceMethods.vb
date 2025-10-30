Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Terminal
Imports Robotics.Security

Namespace API

    Public NotInheritable Class TerminalServiceMethods

        Public Shared Function GetTerminalsLiveStatus(ByVal oPage As System.Web.UI.Page, ByVal strWhere As String, ByVal liteCharge As Boolean) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TerminalState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.TerminalBaseMethods.GetTerminalsListStatus(strWhere, liteCharge, oState)

                oSession.States.TerminalState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TerminalState.Result <> TerminalBaseResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.TerminalState)
                Else
                    If response.Value.Tables.Count > 0 Then
                        oRet = response.Value.Tables(0)
                    End If
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-720")
            End Try

            Return oRet

        End Function

        Public Shared Function GetTerminals(ByVal oPage As System.Web.UI.Page, Optional ByVal strWhere As String = "", Optional ByVal _passportTicket As roPassportTicket = Nothing, Optional ByVal excludeState As Boolean = False) As roTerminalList

            Dim oRet As roTerminalList = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TerminalState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of roTerminalList) = VTLiveApi.TerminalBaseMethods.GetTerminals(strWhere, oState)
                oRet = response.Value

                oSession.States.TerminalState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TerminalState.Result <> TerminalBaseResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.TerminalState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-721")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Devuelve una lista de los Tipos de Terminales (sysroReaderTemplates)
        ''' </summary>
        ''' <param name="oPage">Pagina donde devolver errores</param>
        ''' <param name="strDirection">'Local' o 'Remote'. En blanco para recuperar todos</param>
        ''' <returns>Un DataTable con los campos 'type' y 'Direction'</returns>
        ''' <remarks></remarks>
        Public Shared Function GetTerminalTypes(ByVal oPage As System.Web.UI.Page, Optional ByVal strDirection As String = "") As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TerminalState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.TerminalBaseMethods.GetTerminalTypes(strDirection, oState)

                oSession.States.TerminalState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TerminalState.Result = TerminalBaseResultEnum.NoError Then
                    If response.Value.Tables.Count > 0 Then
                        oRet = response.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.TerminalState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-723")
            End Try

            Return oRet

        End Function

        Public Shared Function GetTerminalsDataSet(ByVal oPage As System.Web.UI.Page, Optional ByVal strWhere As String = "") As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TerminalState

            WebServiceHelper.SetState(oState)
            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.TerminalBaseMethods.GetTerminalsDataSet(strWhere, oState)

                oSession.States.TerminalState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TerminalState.Result = TerminalBaseResultEnum.NoError Then
                    If response.Value.Tables.Count > 0 Then
                        oRet = response.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.TerminalState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-724")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Recuperem la plantilla de les posibles combinacions dels Readers
        ''' </summary>
        ''' <param name="oPage">Pagina a retornar errors</param>
        ''' <param name="strType">Tipus de Terminal</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetTerminalsReadersTemplate(ByVal oPage As System.Web.UI.Page, ByVal strType As String) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TerminalState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.TerminalBaseMethods.GetTerminalReadersTemplates(strType, oState)

                oSession.States.TerminalState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TerminalState.Result = TerminalBaseResultEnum.NoError Then
                    If response.Value.Tables.Count > 0 Then
                        oRet = response.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.TerminalState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-725")
            End Try

            Return oRet

        End Function

        Public Shared Function GetTerminal(ByVal oPage As System.Web.UI.Page, ByVal intID As Integer, ByVal bAudit As Boolean) As roTerminal

            Dim oRet As roTerminal = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TerminalState

            WebServiceHelper.SetState(oState)

            Try
                Dim response As roGenericVtResponse(Of roTerminal) = VTLiveApi.TerminalBaseMethods.GetTerminal(intID, oState, bAudit)
                oRet = response.Value

                oSession.States.TerminalState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TerminalState.Result <> TerminalBaseResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.TerminalState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-726")
            End Try

            Return oRet

        End Function

        Public Shared Function GetTerminalName(ByVal oPage As System.Web.UI.Page, ByVal intID As Integer) As String

            Dim strRet As String = String.Empty

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TerminalState

            WebServiceHelper.SetState(oState)

            Try
                Dim response As roGenericVtResponse(Of String) = VTLiveApi.TerminalBaseMethods.GetTerminalName(intID, oState)
                strRet = response.Value

                oSession.States.TerminalState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TerminalState.Result <> TerminalBaseResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.TerminalState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-727")
            End Try

            Return strRet

        End Function

        Public Shared Function SaveNFCReader(ByVal oPage As System.Web.UI.Page, ByVal terminalID As Integer, ByVal readerid As Integer, ByVal idzone As Integer, ByVal nfc As String, ByVal idmode As Integer, ByVal description As String, ByVal bAudit As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TerminalState

            WebServiceHelper.SetState(oState)

            Try
                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.TerminalBaseMethods.SaveNFCReader(terminalID, readerid, idzone, nfc, idmode, description, oState, bAudit)

                bolRet = response.Value

                oSession.States.TerminalState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TerminalState.Result <> TerminalBaseResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.TerminalState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-728:SaveNFCTag")
            End Try

            Return bolRet

        End Function

        Public Shared Function SaveTerminal(ByVal oPage As System.Web.UI.Page, ByRef oTerminal As roTerminal, ByVal bAudit As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TerminalState

            WebServiceHelper.SetState(oState)

            Try
                Dim response As roGenericVtResponse(Of (roTerminal, Boolean)) = VTLiveApi.TerminalBaseMethods.SaveTerminal(oTerminal, oState, bAudit)
                oTerminal = response.Value.Item1
                bolRet = response.Value.Item2

                oSession.States.TerminalState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TerminalState.Result <> TerminalBaseResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.TerminalState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-728")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Comprobación Núm. de Serie del Terminal
        ''' </summary>
        ''' <param name="oPage">Me.Page de la pagina para devolver el mensaje de error</param>
        ''' <param name="intID">id del terminal a registrar</param>
        ''' <param name="SNTerminal">Codigo de registro del Terminal</param>
        ''' <returns>True si el núm. de serie es correcto</returns>
        ''' <remarks></remarks>
        Public Shared Function RegisterMxCTerminal(ByVal oPage As System.Web.UI.Page, ByVal intID As Integer, ByVal SNTerminal As String) As Boolean
            Dim oRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TerminalState

            WebServiceHelper.SetState(oState)

            Try
                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.TerminalBaseMethods.RegisterMxcTerminal(intID, SNTerminal, oState, True)
                oRet = response.Value

                oSession.States.TerminalState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TerminalState.Result <> TerminalBaseResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.TerminalState)
                    oRet = False
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-729")
            End Try

            Return oRet
        End Function

        ''' <summary>
        ''' Comprobación Núm. de Serie del Terminal
        ''' </summary>
        ''' <param name="oPage">Me.Page de la pagina para devolver el mensaje de error</param>
        ''' <param name="Serial">Núm. de Serie Visualtime</param>
        ''' <param name="SNTerminal">Núm. de Serie del Terminal</param>
        ''' <returns>True si el núm. de serie es correcto</returns>
        ''' <remarks></remarks>
        Public Shared Function CheckTerminalSerialNum(ByVal oPage As System.Web.UI.Page, ByVal Serial As String, ByVal SNTerminal As String) As Boolean
            Dim oRet As Boolean

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TerminalState

            WebServiceHelper.SetState(oState)

            Try
                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.TerminalBaseMethods.CheckTerminalSerialNum(Serial, SNTerminal, oState)
                oRet = response.Value

                oSession.States.TerminalState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TerminalState.Result <> TerminalBaseResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.TerminalState)
                    oRet = False
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-729")
                oRet = False
            End Try

            Return oRet
        End Function

        ''' <summary>
        ''' Recupera el siguiente número de Id. de Terminal
        ''' </summary>
        ''' <param name="oPage">Me.Page de la pagina para devolver el mensaje de error</param>
        ''' <returns>Devuelve el núm. siguiente de terminal (inferior a 90), en caso de error -1</returns>
        ''' <remarks></remarks>
        Public Shared Function RetrieveTerminalNextID(ByVal oPage As System.Web.UI.Page) As Integer
            Dim oRet As Integer = -1

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TerminalState

            WebServiceHelper.SetState(oState)

            Try
                Dim response As roGenericVtResponse(Of Integer) = VTLiveApi.TerminalBaseMethods.RetrieveTerminalNextID(oState)
                oRet = response.Value

                oSession.States.TerminalState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TerminalState.Result <> TerminalBaseResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.TerminalState)
                    oRet = -1
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-730")
                oRet = -1
            End Try

            Return oRet
        End Function

        ''' <summary>
        ''' Recupera el siguiente número de Id. de Terminal
        ''' </summary>
        ''' <param name="oPage">Me.Page de la pagina para devolver el mensaje de error</param>
        ''' <returns>Devuelve el núm. siguiente de terminal (inferior a 90), en caso de error -1</returns>
        ''' <remarks></remarks>
        Public Shared Function RetrieveTerminalReaderNextID(ByVal oPage As System.Web.UI.Page, ByVal IDTerminal As Integer) As Integer
            Dim oRet As Integer = -1

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TerminalState

            WebServiceHelper.SetState(oState)

            Try
                Dim response As roGenericVtResponse(Of Integer) = VTLiveApi.TerminalBaseMethods.RetrieveTerminalReaderNextID(IDTerminal, oState)
                oRet = response.Value

                oSession.States.TerminalState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TerminalState.Result <> TerminalBaseResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.TerminalState)
                    oRet = -1
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-731")
                oRet = -1
            End Try

            Return oRet
        End Function

        ''' <summary>
        ''' Añade un TerminalReader a un Terminal
        ''' </summary>
        ''' <param name="oTerminal">roTerminal al que añadir el roTerminalReader</param>
        ''' <param name="oTerminalReader">TerminalReader a crear / actualizar</param>
        ''' <returns>Devuelve TRUE si consigue añadirlo / actualizarlo</returns>
        ''' <remarks></remarks>
        Public Shared Function AddTerminalReader(ByVal oPage As System.Web.UI.Page, ByRef oTerminal As roTerminal, ByRef oTerminalReader As roTerminal.roTerminalReader) As Boolean
            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TerminalState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of (roTerminal, roTerminal.roTerminalReader, Boolean)) = VTLiveApi.TerminalBaseMethods.AddTerminalReader(oTerminal, oTerminalReader, oState)
                oTerminal = response.Value.Item1
                oTerminalReader = response.Value.Item2
                bolRet = response.Value.Item3

                oSession.States.TerminalState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TerminalState.Result <> TerminalBaseResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.TerminalState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-732")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Elimina un TerminalReader de un Terminal
        ''' </summary>
        ''' <param name="oTerminal">roTerminal al que eliminar el roTerminalReader</param>
        ''' <param name="oTerminalReader">TerminalReader a eliminar</param>
        ''' <returns>Devuelve TRUE si consigue eliminarlo</returns>
        ''' <remarks></remarks>
        Public Shared Function RemoveTerminalReader(ByVal oPage As System.Web.UI.Page, ByRef oTerminal As roTerminal, ByRef oTerminalReader As roTerminal.roTerminalReader) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TerminalState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of (roTerminal, roTerminal.roTerminalReader, Boolean)) = VTLiveApi.TerminalBaseMethods.RemoveTerminalReader(oTerminal, oTerminalReader, oState)

                oTerminal = response.Value.Item1
                oTerminalReader = response.Value.Item2
                bolRet = response.Value.Item3

                oSession.States.TerminalState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TerminalState.Result <> TerminalBaseResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.TerminalState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-733")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Elimina un terminal
        ''' </summary>
        ''' <param name="oPage">Página que solicita la acción</param>
        ''' <param name="oTerminal">Terminal a borrar</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function DeleteTerminal(ByVal oPage As System.Web.UI.Page, ByVal oTerminal As roTerminal, ByVal bAudit As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TerminalState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.TerminalBaseMethods.DeleteTerminal(oTerminal, oState, bAudit)
                bolRet = response.Value

                oSession.States.TerminalState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TerminalState.Result <> TerminalBaseResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.TerminalState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-734")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Elimina un terminal
        ''' </summary>
        ''' <param name="oPage">Página que solicita la acción</param>
        ''' <param name="intIDTerminal">Código del terminal a borrar</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function DeleteTerminal(ByVal oPage As System.Web.UI.Page, ByVal intIDTerminal As Integer, ByVal bAudit As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TerminalState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.TerminalBaseMethods.DeleteTerminalById(intIDTerminal, oState, bAudit)
                bolRet = response.Value

                oSession.States.TerminalState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TerminalState.Result <> TerminalBaseResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.TerminalState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-735")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Graba una configuración de sirena
        ''' </summary>
        ''' <param name="oPage">Página que solicita la acción</param>
        ''' <param name="oTerminalSiren">Configuración de sirena a guardar</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function SaveTerminalSiren(ByVal oPage As System.Web.UI.Page, ByVal oTerminalSiren As roTerminalSiren, ByVal bAudit As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TerminalState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.TerminalBaseMethods.SaveTerminalSiren(oTerminalSiren, oState, bAudit)
                bolRet = response.Value

                oSession.States.TerminalState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TerminalState.Result <> TerminalBaseResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.TerminalState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-736")
            End Try
            Return bolRet

        End Function

        Public Shared Function SaveMx9Parameter(ByVal oPage As System.Web.UI.Page, idTerminal As Integer, strParameterName As String, strParameterValue As String) As Boolean

            Dim strRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TerminalState

            WebServiceHelper.SetState(oState)

            Try
                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.TerminalBaseMethods.SaveMx9Parameter(idTerminal, strParameterName, strParameterValue, oState)
                strRet = response.Value

                oSession.States.TerminalState = oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TerminalState.Result <> TerminalBaseResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.TerminalState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-736")
            End Try
            Return strRet

        End Function

        Public Shared Function ImportVTC(ByVal oPage As System.Web.UI.Page, strFileContents As String) As Boolean

            Dim strRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TerminalState

            WebServiceHelper.SetState(oState)

            Try
                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.TerminalBaseMethods.ImportVTC(strFileContents, oState)
                strRet = response.Value

                oSession.States.TerminalState = oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TerminalState.Result <> TerminalBaseResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.TerminalState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-736")
            End Try
            Return strRet

        End Function

        Public Shared Function RegisterTerminalOnMT(ByVal oPage As System.Web.UI.Page, strCompanyName As String, strSerialNumber As String, strTerminalType As String) As String
            Dim strRet As String = String.Empty

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TerminalState

            WebServiceHelper.SetState(oState)

            Try
                Dim response As roGenericVtResponse(Of String) = VTLiveApi.TerminalBaseMethods.RegisterTerminalOnMT(strCompanyName, strSerialNumber, strTerminalType, oState)
                strRet = response.Value

                oSession.States.TerminalState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TerminalState.Result <> TerminalBaseResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.TerminalState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-736")
            End Try
            Return strRet
        End Function

        ''' <summary>
        ''' Elimina una configuración de sirena
        ''' </summary>
        ''' <param name="oPage">Página que solicita la acción</param>
        ''' <param name="oTerminalSiren">Configuración de sirena a eliminar</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function DeleteTerminalSiren(ByVal oPage As System.Web.UI.Page, ByVal oTerminalSiren As roTerminalSiren, ByVal bAudit As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TerminalState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.TerminalBaseMethods.DeleteTerminalSiren(oTerminalSiren, oState, bAudit)
                bolRet = response.Value

                oSession.States.TerminalState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TerminalState.Result <> TerminalBaseResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.TerminalState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-737")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Elimina una configuración de sirena
        ''' </summary>
        ''' <param name="oPage">Página que solicita la acción</param>
        ''' <param name="intIDTerminal">Código del terminal al que pertenece la sirena</param>
        ''' <param name="intID">Código de la sirena a borrar</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function DeleteTerminalSiren(ByVal oPage As System.Web.UI.Page, ByVal intIDTerminal As Integer, ByVal intID As Integer, ByVal bAudit As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TerminalState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.TerminalBaseMethods.DeleteTerminalSirenById(intIDTerminal, intID, oState, bAudit)
                bolRet = response.Value

                oSession.States.TerminalState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TerminalState.Result <> TerminalBaseResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.TerminalState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-738")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Verifica que la configuración del terminal sea correcta.
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="oTerminal"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function ValidateTerminal(ByVal oPage As System.Web.UI.Page, ByVal oTerminal As roTerminal) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TerminalState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.TerminalBaseMethods.ValidateTerminal(oTerminal, oState)
                bolRet = response.Value

                oSession.States.TerminalState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TerminalState.Result <> TerminalBaseResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.TerminalState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-739")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Verifica que la configuración del lector sea correcta.
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="oReader"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function ValidateReader(ByVal oPage As System.Web.UI.Page, ByVal oReader As roTerminal.roTerminalReader) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TerminalState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.TerminalBaseMethods.ValidateReader(oReader, oState)
                bolRet = response.Value

                oSession.States.TerminalState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TerminalState.Result <> TerminalBaseResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.TerminalState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-740")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Devuelve la fecha y hora actual de un terminal, según la fecha actual del servidor y la diferencia horaria configurada para el terminal.
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="intIDTerminal">Código de terminal.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetCurrentDateTime(ByVal oPage As System.Web.UI.Page, ByVal intIDTerminal As Integer) As DateTime

            Dim xRet As DateTime

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TerminalState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DateTime) = VTLiveApi.TerminalBaseMethods.GetCurrentDateTime(intIDTerminal, oState)
                xRet = response.Value

                oSession.States.TerminalState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TerminalState.Result <> TerminalBaseResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.TerminalState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-741")
            End Try

            Return xRet

        End Function

        ''' <summary>
        ''' Devuelve una lista con la definición de las zonas horarias que se pueden configurar a un terminal.
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetTimeZones(ByVal oPage As System.Web.UI.Page, ByVal _TerminalType As String) As Generic.List(Of roTerminalTimeZone)

            Dim oRet As New Generic.List(Of roTerminalTimeZone)

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TerminalState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of List(Of roTerminalTimeZone)) = VTLiveApi.TerminalBaseMethods.GetTimeZones(_TerminalType, oState)

                oSession.States.TerminalState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TerminalState.Result = TerminalBaseResultEnum.NoError Then
                    oRet = response.Value
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.TerminalState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-742")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Obtiene la lista de terminales de un tipo por los que puede fichar un empleado. <br></br>
        ''' Tiene en cuenta la configuración de limitación de empleados.<br></br>
        ''' No tiene en cuenta la configuración de accesos.
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="_IDEmployee"></param>
        ''' <param name="_TerminalType"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetEmployeeTerminals(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, ByVal _TerminalType As String, Optional ByVal _passportTicket As roPassportTicket = Nothing, Optional ByVal excludeState As Boolean = False) As Generic.List(Of roTerminal)

            Dim oRet As New Generic.List(Of roTerminal)

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TerminalState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of List(Of roTerminal)) = VTLiveApi.TerminalBaseMethods.GetEmployeeTerminals(_IDEmployee, _TerminalType, oState)

                oSession.States.TerminalState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TerminalState.Result = TerminalBaseResultEnum.NoError Then
                    oRet = response.Value
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.TerminalState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-743")
            End Try

            Return oRet

        End Function

        Public Shared Function GetSupremaConfiguration(ByVal oPage As System.Web.UI.Page, Optional ByVal _passportTicket As roPassportTicket = Nothing) As SupremaConfigurationParameters

            Dim oRet As New SupremaConfigurationParameters

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TerminalState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of SupremaConfigurationParameters) = VTLiveApi.TerminalBaseMethods.GetSupremaConfiguration(oState)

                oSession.States.TerminalState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TerminalState.Result = TerminalBaseResultEnum.NoError Then
                    oRet = response.Value
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.TerminalState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-743")
            End Try

            Return oRet

        End Function

        Public Shared Function SaveSupremaConfiguration(ByVal oPage As System.Web.UI.Page, supremaConfiguration As SupremaConfigurationParameters) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TerminalState

            WebServiceHelper.SetState(oState)

            Try
                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.TerminalBaseMethods.SaveSupremaConfiguration(supremaConfiguration, oState)

                bolRet = response.Value

                oSession.States.TerminalState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TerminalState.Result <> TerminalBaseResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.TerminalState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-728:SaveNFCTag")
            End Try

            Return bolRet

        End Function

        Public Shared Function LastErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.TerminalState.ErrorText
            End If
            Return strRet
        End Function

    End Class

End Namespace