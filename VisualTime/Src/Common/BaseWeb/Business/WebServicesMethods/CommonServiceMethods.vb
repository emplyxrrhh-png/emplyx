Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Common.AdvancedParameter
Imports Robotics.Base.VTEmployees.Employee
Imports Robotics.Security
Imports Robotics.VTBase

Namespace API

    Public NotInheritable Class CommonServiceMethods

        Public Shared Function GetRuntimeId() As String

            Dim strRet As String = ""

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()

            Try

                strRet = VTLiveApi.CommonMethods.GetRuntimeId().Value
            Catch ex As Exception

            End Try

            Return strRet

        End Function

        Public Shared Function GetVisualTimeEdition() As String

            Dim strRet As String = ""

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()

            Try

                strRet = VTLiveApi.CommonMethods.GetVisualTimeEdition().Value
            Catch ex As Exception

            End Try

            Return strRet

        End Function

        Public Shared Function DefaultLanguage() As String

            Dim strRet As String = ""

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.CommonState

            Try

                strRet = VTLiveApi.CommonMethods.DefaultLanguage().Value
            Catch ex As Exception

            End Try

            Return strRet

        End Function

        Public Shared Function GetSystemEmailUserFieldName() As String
            Dim sFieldName As String = String.Empty
            Try

                sFieldName = WLHelper.GetSystemEmailUserFieldName()
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
            End Try

            Return sFieldName

        End Function


        Public Shared Function GetAdvancedParameterLite(ByVal parameterName As String) As String
            Dim oRet As New roAdvancedParameter

            Try

                If roTypes.Any2String(System.Web.HttpContext.Current.Session("roMultiCompanyId")) = String.Empty Then
                    Return String.Empty
                Else
                    ' VTLiveApi.CommonMethods.Timeout = System.Threading.Timeout.Infinite
                    Dim wsRet As roGenericVtResponse(Of roAdvancedParameter) = VTLiveApi.CommonMethods.GetAdvancedParameter(parameterName, New roWsState(), False)
                    oRet = wsRet.Value
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
            End Try

            Return oRet.Value

        End Function

        Public Shared Function GetCompanyAdvancedParameterLite(ByVal companyName As String, ByVal parameterName As String) As String
            Dim oRet As New roAdvancedParameter

            Try
                ' VTLiveApi.CommonMethods.Timeout = System.Threading.Timeout.Infinite
                Dim wsRet As roGenericVtResponse(Of roAdvancedParameter) = VTLiveApi.CommonMethods.GetCompanyAdvancedParameter(companyName, parameterName, New roWsState() With {.IDPassport = -2}, False)
                oRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
            End Try

            Return roTypes.Any2String(oRet.Value)

        End Function

        Public Shared Function GetAdvancedParameter(ByVal oPage As System.Web.UI.Page, ByVal parameterName As String, Optional ByVal bolAudit As Boolean = False) As roAdvancedParameter
            Dim oRet As New roAdvancedParameter
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.CommonState

            WebServiceHelper.SetState(oState)

            Try
                ' VTLiveApi.CommonMethods.Timeout = System.Threading.Timeout.Infinite
                Dim wsRet As roGenericVtResponse(Of roAdvancedParameter) = VTLiveApi.CommonMethods.GetAdvancedParameter(parameterName, oState, bolAudit)

                oSession.States.CommonState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                oRet = wsRet.Value

                If oSession.States.CommonState.Result <> AdvancedParameterResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.CommonState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-096")
            End Try

            Return oRet

        End Function

        Public Shared Function GetAdvancedParameterList(ByVal oPage As System.Web.UI.Page, Optional ByVal bolAudit As Boolean = False) As List(Of roAdvancedParameter)
            Dim oRet As List(Of roAdvancedParameter) = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.CommonState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of List(Of roAdvancedParameter)) = VTLiveApi.CommonMethods.GetAdvancedParameterList(oState, bolAudit)

                oRet = wsRet.Value

                oSession.States.CommonState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.CommonState.Result <> AdvancedParameterResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.CommonState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-097")
            End Try

            Return oRet

        End Function

        Public Shared Function LastErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.CommonState.ErrorText
            End If
            Return strRet

        End Function

        Public Shared Function SaveAdvancedParameter(ByVal oPage As System.Web.UI.Page, ByRef oAdvancedParameter As roAdvancedParameter, ByVal bAudit As Boolean) As Boolean
            Dim oRet As Boolean

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.CommonState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.CommonMethods.SaveAdvancedParameter(oAdvancedParameter, oState, bAudit)

                oSession.States.CommonState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                oRet = wsRet.Value

                If oSession.States.CommonState.Result <> AdvancedParameterResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.CommonState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-098")
            End Try

            Return oRet

        End Function

        Public Shared Function GetEmployeeListFromFilter(ByVal oPage As System.Web.UI.Page, ByVal sWhere As String) As String
            Dim oRet As String = String.Empty

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.CommonState

            WebServiceHelper.SetState(oState)

            Try
                Dim bState = New roEmployeeState(-1)
                roWsStateManager.CopyTo(oState, bState)

                oRet = VTBusiness.Common.roBusinessSupport.GetEmployeeListFromFilter(sWhere, bState)

                Dim newGState As New roWsState
                roWsStateManager.CopyTo(bState, oState)

                oSession.States.CommonState = oState

                If oSession.States.CommonState.Result <> AdvancedParameterResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.CommonState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-098")
            End Try

            Return oRet
        End Function

    End Class

End Namespace