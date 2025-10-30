Imports Robotics.Base.DTOs
Imports Robotics.Base.VTEmployees.Contract

Namespace API

    Public NotInheritable Class ContractsServiceMethods

        Public Shared Function GetActiveContract(ByVal oPage As System.Web.UI.Page, ByVal IDEmployee As Integer, ByVal bAudit As Boolean) As roContract

            Dim oContract As roContract = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ContractState

            WebServiceHelper.SetState(oState)

            Try

                oState.IDPassport = WLHelperWeb.CurrentPassport.ID
                Dim wsRet As roGenericVtResponse(Of roContract) = VTLiveApi.ContractsMethods.GetActiveContract(IDEmployee, oState, bAudit)

                oContract = wsRet.Value

                oSession.States.ContractState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ContractState.Result <> ContractsResultEnum.NoError And
                    oSession.States.ContractState.Result <> ContractsResultEnum.ContractNotFound Then
                    HelperWeb.ShowError(oPage, oState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-127")
            End Try

            Return oContract
        End Function

        Public Shared Function GetContractsByIDEmployee(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, ByVal bAudit As Boolean) As Data.DataTable

            Dim oRet As Data.DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ContractState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.ContractsMethods.GetContractsByIDEmployee(_IDEmployee, oState, bAudit)

                oSession.States.ContractState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ContractState.Result = ContractsResultEnum.NoError Then
                    If wsRet.Value IsNot Nothing AndAlso wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    HelperWeb.ShowError(oPage, oSession.States.ContractState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-128")
            End Try

            Return oRet

        End Function

        Public Shared Function GetDatesOfEmployeePeriodByContractInDate(ByVal oPage As System.Web.UI.Page, type As SummaryType, ByVal IDEmployee As Integer, ByVal xDate As DateTime) As List(Of Date)
            Dim oRet As List(Of Date) = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ContractState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of List(Of Date)) = VTLiveApi.ContractsMethods.GetDatesOfEmployeePeriodByContractInDate(type, IDEmployee, xDate, oState)

                oSession.States.ContractState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ContractState.Result = ContractsResultEnum.NoError Then
                    If wsRet.Value IsNot Nothing AndAlso wsRet.Value.Count > 0 Then
                        oRet = wsRet.Value
                    End If
                Else
                    HelperWeb.ShowError(oPage, oSession.States.ContractState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-128")
            End Try

            Return oRet
        End Function

        Public Shared Function GetDatesOfAnnualWorkPeriodsInDate(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, ByVal xDate As DateTime) As List(Of Date)

            Dim oRet As List(Of Date) = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ContractState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of List(Of Date)) = VTLiveApi.ContractsMethods.GetDatesOfAnnualWorkPeriodsInDate(_IDEmployee, xDate, oState)

                oSession.States.ContractState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ContractState.Result = ContractsResultEnum.NoError Then
                    If wsRet.Value IsNot Nothing AndAlso wsRet.Value.Count > 0 Then
                        oRet = wsRet.Value
                    End If
                Else
                    HelperWeb.ShowError(oPage, oSession.States.ContractState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-128")
            End Try

            Return oRet

        End Function

        Public Shared Function GetContract(ByVal oPage As System.Web.UI.Page, ByVal _IDContract As String, ByVal bAudit As Boolean) As roContract

            Dim oRet As roContract = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ContractState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of roContract) = VTLiveApi.ContractsMethods.GetContract(_IDContract, oState, bAudit)

                oSession.States.ContractState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ContractState.Result <> ContractsResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.ContractState)
                Else
                    oRet = wsRet.Value
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-129")
            End Try

            Return oRet

        End Function

        Public Shared Function DeleteContract(ByVal oPage As System.Web.UI.Page, ByVal _IDContract As String, ByVal bAudit As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ContractState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.ContractsMethods.DeleteContract(_IDContract, oState, bAudit)

                oSession.States.ContractState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ContractState.Result <> ContractsResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.ContractState)
                Else
                    bolRet = wsRet.Value
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-130")
            End Try

            Return bolRet

        End Function

        Public Shared Function SaveContract(ByVal oPage As PageBase, ByVal oContract As roContract, ByVal bAudit As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ContractState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.ContractsMethods.SaveContract(oContract, oState, bAudit)

                oSession.States.ContractState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ContractState.Result <> ContractsResultEnum.NoError Then
                    Select Case oState.Result
                        Case ContractsResultEnum.InvalidDateInterval
                            HelperWeb.ShowMessage(oPage, "", oPage.Language.Translate("InvalidDateInterval.Message", oPage.DefaultScope))
                        Case Else
                            HelperWeb.ShowError(oPage, oSession.States.ContractState)
                    End Select
                Else
                    bolRet = wsRet.Value
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-131")
            End Try

            Return bolRet

        End Function

        Public Shared Function ExistsCardID(ByVal oPage As System.Web.UI.Page, ByVal strCardID As String) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ContractState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.ContractsMethods.ExistsCardID(strCardID, Now, oState)

                oSession.States.ContractState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ContractState.Result <> ContractsResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.ContractState)
                Else
                    bolRet = wsRet.Value
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-132")
            End Try

            Return bolRet

        End Function

        Public Shared Function ExistsContractID(ByVal oPage As System.Web.UI.Page, ByVal strContractID As String) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ContractState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.ContractsMethods.ExistsContractID(strContractID, oState)

                oSession.States.ContractState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ContractState.Result <> ContractsResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.ContractState)
                Else
                    bolRet = wsRet.Value
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-133")
            End Try

            Return bolRet

        End Function

        Public Shared Function ValidateCardID(ByVal oPage As System.Web.UI.Page, ByVal strCardID As String, ByVal _IDEmployee As Integer, ByVal xBeginContract As Date, ByVal xEndContract As Date) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ContractState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.ContractsMethods.ValidateCardID(strCardID, _IDEmployee, xBeginContract, xEndContract, oState)

                oSession.States.ContractState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ContractState.Result <> ContractsResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.ContractState)
                Else
                    bolRet = wsRet.Value
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-134")
            End Try

            Return bolRet

        End Function

        Public Shared Function GetMaxIDContract(ByVal oPage As System.Web.UI.Page) As ULong

            Dim lngRet As Long = 0

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ContractState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of ULong) = VTLiveApi.ContractsMethods.GetMaxIDContract(oState)

                oSession.States.ContractState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ContractState.Result <> ContractsResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.ContractState)
                Else
                    lngRet = wsRet.Value
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-135")
            End Try

            Return lngRet
        End Function

        Public Shared Function LastErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.ContractState.ErrorText
            End If
            Return strRet
        End Function

        Public Shared Function LastError() As roWsState
            Dim oState As New roWsState
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                oState = roWsUserManagement.SessionObject().States.ContractState
            End If
            Return oState
        End Function

    End Class

End Namespace