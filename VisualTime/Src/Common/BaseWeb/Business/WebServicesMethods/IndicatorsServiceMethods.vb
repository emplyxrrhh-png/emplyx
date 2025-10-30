Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Indicator

Namespace API

    Public NotInheritable Class IndicatorsServiceMethods

        ''' <summary>
        ''' Obtiene la lista de indicadores definidos.
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="_Order"></param>
        ''' <param name="_Audit"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetIndicators(ByVal _IndicatorType As IndicatorsType, ByVal oPage As System.Web.UI.Page, ByVal _Order As String, ByVal _Audit As Boolean) As Generic.List(Of roIndicator)

            'Dim oRet As Generic.List(Of roIndicator) = Nothing

            'Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            'Dim oState As roIndicatorState = oSession.States.IndicatorState

            'WebServiceHelper.SetState(oState, oPage)

            'Try

            '    Dim _Indicators() As roIndicator = VTLiveApi.IndicatorMethods.GetIndicators(_IndicatorType, _Order, _Audit, oState)

            '    oSession.States.IndicatorState = oState
            '    roWsUserManagement.SessionObject = oSession

            '    If _Indicators IsNot Nothing Then
            '        oRet = _Indicators.ToList
            '    End If
            '    If oSession.States.IndicatorState.Result <> ResultEnum.NoError Then
            '        ' Mostrar el error
            '        oSession.States.IndicatorState.Result
            '    End If

            'Catch ex As Exception
            '    Dim oTmpState As New Robotics.Base.DTOs.roWsState
            '    oTmpState.Result = 1
            '    Dim oLanguage As New roLanguageWeb
            '    oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
            '    HelperWeb.ShowError(oPage, oTmpState, "9-BW01-358")
            'End Try

            'Return oRet

            Dim oRet As Generic.List(Of roIndicator) = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.IndicatorState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of List(Of roIndicator)) = VTLiveApi.IndicatorMethods.GetIndicators(_IndicatorType, _Order, _Audit, oState)

                oSession.States.IndicatorState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.IndicatorState.Result = IndicatorResultEnum.NoError Then
                    oRet = wsRet.Value
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.IndicatorState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-358")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Obtiene la lista de indicadores definidos.
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="_Order"></param>
        ''' <param name="_Audit"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetIndicatorsDataTable(ByVal _IndicatorType As IndicatorsType, ByVal oPage As System.Web.UI.Page, ByVal _Order As String, ByVal _Audit As Boolean) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.IndicatorState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.IndicatorMethods.GetIndicatorsDataTable(_IndicatorType, _Order, _Audit, oState)

                oSession.States.IndicatorState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.IndicatorState.Result = IndicatorResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.IndicatorState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-359")
            End Try

            Return oRet

        End Function

        Public Shared Function GetIndicator(ByVal oPage As System.Web.UI.Page, ByVal _ID As Integer, ByVal _Audit As Boolean) As roIndicator

            'Dim oRet As roIndicator = Nothing

            'Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            'Dim oState As roIndicatorState = oSession.States.IndicatorState

            'WebServiceHelper.SetState(oState, oPage)

            'Try

            '    oRet = VTLiveApi.IndicatorMethods.GetIndicator(_ID, _Audit, oState)

            '    oSession.States.IndicatorState = oState
            '    roWsUserManagement.SessionObject = oSession

            '    If oSession.States.IndicatorState.Result <> ResultEnum.NoError Then
            '        ' Mostrar el error
            '        oSession.States.IndicatorState.Result
            '    End If

            'Catch ex As Exception
            '    Dim oTmpState As New Robotics.Base.DTOs.roWsState
            '    oTmpState.Result = 1
            '    Dim oLanguage As New roLanguageWeb
            '    oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
            '    HelperWeb.ShowError(oPage, oTmpState, "9-BW01-360")
            'End Try

            'Return oRet

            Dim oRet As roIndicator = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.IndicatorState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of roIndicator) = VTLiveApi.IndicatorMethods.GetIndicator(_ID, _Audit, oState)

                oSession.States.IndicatorState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.IndicatorState.Result <> IndicatorResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.IndicatorState)
                End If

                oRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-360")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Guarda la configuración de un indicador de rendimiento
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="_Indicator"></param>
        ''' <param name="_Audit"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function SaveIndicator(ByVal _IndicatorType As IndicatorsType, ByVal oPage As System.Web.UI.Page, ByRef _Indicator As roIndicator, ByVal _Audit As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.IndicatorState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of roIndicator) = VTLiveApi.IndicatorMethods.SaveIndicator(_IndicatorType, _Indicator, oState, _Audit)

                oSession.States.IndicatorState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.IndicatorState.Result <> IndicatorResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.IndicatorState)
                End If

                If wsRet.Value IsNot Nothing Then
                    bolRet = True
                    _Indicator = wsRet.Value
                Else
                    bolRet = False
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-361")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Elimina un indicador de rendimiento
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="_IDIndicator"></param>
        ''' <param name="_Audit"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function DeleteIndicator(ByVal oPage As System.Web.UI.Page, ByVal _IDIndicator As Integer, ByVal _Audit As Boolean) As Boolean
            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.IndicatorState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.IndicatorMethods.DeleteIndicator(_IDIndicator, oState, _Audit)

                oSession.States.IndicatorState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.IndicatorState.Result <> IndicatorResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.IndicatorState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-362")
            End Try

            Return bolRet

        End Function

        Public Shared Function LastErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.IndicatorState.ErrorText
            End If
            Return strRet
        End Function

    End Class

End Namespace