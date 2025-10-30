Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.AccessGroup

Namespace API

    Public NotInheritable Class AccessGroupServiceMethods

#Region "AccessGroups"

        Public Shared Function GetAccessGroups(ByVal oPage As System.Web.UI.Page) As DataTable

            Dim oRet As DataTable = Nothing

            Try
                Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
                Dim oState As roWsState = oSession.States.AccessGroupState

                WebServiceHelper.SetState(oState)

                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.AccessGroupMethods.GetAccessGroupsDataSet(oState)

                oSession.States.AccessGroupState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.AccessGroupState.Result = AccessGroupResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, roWsUserManagement.SessionObject().States.AccessGroupState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                Dim oLanguage As New roLanguageWeb
                oTmpState.Result = 1
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-004")
            End Try

            Return oRet

        End Function

        Public Shared Function GetAccessGroupsByPassport(ByVal oPage As System.Web.UI.Page, ByVal idPassport As Integer) As Integer()

            Dim oRet As Integer() = Nothing

            Try
                Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
                Dim oState As roWsState = oSession.States.AccessGroupState

                WebServiceHelper.SetState(oState)

                Dim wsRet As roGenericVtResponse(Of Integer()) = VTLiveApi.AccessGroupMethods.GetAccessGroupsByPassport(oState, idPassport)

                oRet = wsRet.Value
                oSession.States.AccessGroupState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If roWsUserManagement.SessionObject().States.AccessGroupState.Result <> AccessGroupResultEnum.NoError Then ''''''''''''''''''
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, roWsUserManagement.SessionObject().States.AccessGroupState) '''''''''''''''''''''''''''''
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-005")

                oRet = {}
            End Try
            Return oRet
        End Function

        Public Shared Function GetAccessGroupsByPassportDataTable(ByVal oPage As System.Web.UI.Page, ByVal idPassport As Integer, ByVal validateParent As Boolean) As DataTable

            Dim oRet As DataTable = Nothing

            Try
                Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
                Dim oState As roWsState = oSession.States.AccessGroupState

                WebServiceHelper.SetState(oState)

                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.AccessGroupMethods.GetAccessGroupsByPassportDataSet(oState, idPassport, validateParent)

                oSession.States.AccessGroupState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.AccessGroupState.Result = AccessGroupResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, roWsUserManagement.SessionObject().States.AccessGroupState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-006")
            End Try

            Return oRet

        End Function

        Public Shared Function EmptyAccessGroupEmployees(ByVal oPage As System.Web.UI.Page, ByVal IDAccessSource As Integer, Optional ByVal bAudit As Boolean = True) As Boolean
            Dim oRet As Boolean

            Try
                Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
                Dim oState As roWsState = oSession.States.AccessGroupState

                WebServiceHelper.SetState(oState)

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.AccessGroupMethods.EmptyAccessGroupEmployees(IDAccessSource, oState, bAudit)

                oRet = wsRet.Value
                oSession.States.AccessGroupState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If roWsUserManagement.SessionObject().States.AccessGroupState.Result <> AccessGroupResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, roWsUserManagement.SessionObject().States.AccessGroupState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-007")
            End Try

            Return oRet

        End Function

        Public Shared Function CopyAccess(ByVal oPage As System.Web.UI.Page, ByVal IDAccessSource As Integer, Optional ByVal NewName As String = "", Optional ByVal bAudit As Boolean = True) As roAccessGroup

            Dim oRet As roAccessGroup = Nothing
            Try
                Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
                Dim oState As roWsState = oSession.States.AccessGroupState

                WebServiceHelper.SetState(oState)

                Dim wsRet As roGenericVtResponse(Of roAccessGroup) = VTLiveApi.AccessGroupMethods.CopyAccess(IDAccessSource, NewName, oState, bAudit)

                oRet = wsRet.Value
                oSession.States.AccessGroupState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If roWsUserManagement.SessionObject().States.AccessGroupState.Result <> AccessGroupResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, roWsUserManagement.SessionObject().States.AccessGroupState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-008")
            End Try

            Return oRet

        End Function

        Public Shared Function GetAccessGroupByID(ByVal oPage As System.Web.UI.Page, ByVal intID As Integer, Optional ByVal bolAudit As Boolean = False, Optional idPassport As Integer = 0) As roAccessGroup

            Dim oRet As roAccessGroup = Nothing
            Try
                Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
                Dim oState As roWsState = oSession.States.AccessGroupState

                WebServiceHelper.SetState(oState)

                Dim wsRet As roGenericVtResponse(Of roAccessGroup) = VTLiveApi.AccessGroupMethods.GetAccessGroupByID(intID, oState, bolAudit, idPassport)

                oRet = wsRet.Value
                oSession.States.AccessGroupState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If roWsUserManagement.SessionObject().States.AccessGroupState.Result <> AccessGroupResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, roWsUserManagement.SessionObject().States.AccessGroupState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-009")
            End Try

            Return oRet

        End Function

        Public Shared Function SaveAccessGroup(ByVal oPage As System.Web.UI.Page, ByRef oAccessGroup As roAccessGroup, Optional ByVal bolAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try
                Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
                Dim oState As roWsState = oSession.States.AccessGroupState

                WebServiceHelper.SetState(oState)

                Dim wsRet As roGenericVtResponse(Of roAccessGroup) = VTLiveApi.AccessGroupMethods.SaveAccessGroup(oAccessGroup, oState, bolAudit)

                oSession.States.AccessGroupState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If roWsUserManagement.SessionObject().States.AccessGroupState.Result <> AccessPeriodResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, roWsUserManagement.SessionObject().States.AccessGroupState)
                Else
                    bolRet = True
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-010")
            End Try

            Return bolRet

        End Function

        Public Shared Function SaveAccessGroupByPassport(ByVal oPage As System.Web.UI.Page, ByVal _IDPassport As Integer, ByVal _intAccessGroups() As Integer, ByVal _Audit As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Try
                Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
                Dim oState As roWsState = oSession.States.AccessGroupState

                WebServiceHelper.SetState(oState)

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.AccessGroupMethods.SaveAccessGroupByPassport(_IDPassport, _intAccessGroups, oState, _Audit)

                bolRet = wsRet.Value
                oSession.States.AccessGroupState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If roWsUserManagement.SessionObject().States.AccessGroupState.Result <> AccessGroupResultEnum.NoError Then
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, roWsUserManagement.SessionObject().States.AccessGroupState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-011")
            End Try

            Return bolRet

        End Function

        Public Shared Function DeleteAccessGroup(ByVal oPage As System.Web.UI.Page, ByVal intID As Integer, Optional ByVal bolAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False
            Try
                Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
                Dim oState As roWsState = oSession.States.AccessGroupState

                WebServiceHelper.SetState(oState)

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.AccessGroupMethods.DeleteAccessGroupByID(intID, oState, bolAudit)

                bolRet = wsRet.Value
                oSession.States.AccessGroupState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If roWsUserManagement.SessionObject().States.AccessGroupState.Result <> AccessGroupResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, roWsUserManagement.SessionObject().States.AccessGroupState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-012")
            End Try

            Return bolRet

        End Function

        Public Shared Function GetEmployeeAuthorizations(ByVal oPage As System.Web.UI.Page, ByVal intIDEmployee As Integer, ByVal intIDGroup As Integer, Optional ByVal bolAudit As Boolean = False) As DataTable

            Dim oRet As DataTable = Nothing

            Try
                Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
                Dim oState As roWsState = oSession.States.AccessGroupState

                WebServiceHelper.SetState(oState)

                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.AccessGroupMethods.GetEmployeeAuthorizations(intIDEmployee, intIDGroup, oState)

                oSession.States.AccessGroupState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.AccessGroupState.Result = AccessGroupResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, roWsUserManagement.SessionObject().States.AccessGroupState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-013")
            End Try

            Return oRet

        End Function

        Public Shared Function GetAuthorizationsByZone(ByVal oPage As System.Web.UI.Page, ByVal intIdZone As Integer, Optional ByVal bolAudit As Boolean = False) As roAccessGroup()

            Dim oRet As roAccessGroup() = {}

            Try
                Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
                Dim oState As roWsState = oSession.States.AccessGroupState

                WebServiceHelper.SetState(oState)

                Dim wsRet As roGenericVtResponse(Of List(Of roAccessGroup)) = VTLiveApi.AccessGroupMethods.GetAuthorizationsByZone(intIdZone, oState, bolAudit)

                oSession.States.AccessGroupState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If roWsUserManagement.SessionObject().States.AccessGroupState.Result <> AccessGroupResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, roWsUserManagement.SessionObject().States.AccessGroupState)
                Else
                    oRet = wsRet.Value.ToArray
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-014")
            End Try

            Return oRet

        End Function

        Public Shared Function UpgradeAccessMode(ByVal oPage As System.Web.UI.Page) As Boolean
            Dim oRet As Boolean
            Try
                Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
                Dim oState As roWsState = oSession.States.AccessGroupState

                WebServiceHelper.SetState(oState)

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.AccessGroupMethods.UpgradeAccessMode(oState, True)

                oRet = wsRet.Value
                oSession.States.AccessGroupState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If roWsUserManagement.SessionObject().States.AccessGroupState.Result <> AccessGroupResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, roWsUserManagement.SessionObject().States.AccessGroupState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-007")
            End Try

            Return oRet
        End Function

#End Region

        Public Shared Function LastErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.AccessGroupState.ErrorText
            End If
            Return strRet
        End Function

    End Class

End Namespace