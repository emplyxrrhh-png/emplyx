Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTUserFields.UserFields

Namespace API

    Public NotInheritable Class UserFieldServiceMethods

        Public Shared Function GetUserFields(ByVal oPage As System.Web.UI.Page, ByVal _Type As Types, ByVal strWhere As String, ByVal bolCheckInProcess As Boolean, Optional ByVal bolIsEmergencyReport As Boolean = False) As DataTable ', Optional ByVal strWhere As String = "") As DataTable
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserFieldState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.UserFieldMethods.GetUserFieldsDataSet(_Type, strWhere, bolCheckInProcess, oState, bolIsEmergencyReport)

                oSession.States.UserFieldState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserFieldState.Result = UserFieldResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.UserFieldState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-781")
            End Try

            Return oRet

        End Function

        Public Shared Function GetTaskFields(ByVal oPage As System.Web.UI.Page, ByVal _Type As Types, Optional ByVal strWhere As String = "") As DataTable
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserFieldState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.UserFieldMethods.GetTaskFieldsDataSet(_Type, strWhere, oState)

                oSession.States.UserFieldState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserFieldState.Result = UserFieldResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.UserFieldState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-782")
            End Try

            Return oRet

        End Function

        Public Shared Function GetBusinessCenterFields(ByVal oPage As System.Web.UI.Page, ByVal _Type As Types, Optional ByVal strWhere As String = "") As DataTable
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserFieldState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.UserFieldMethods.GetBusinessCenterFieldsDataSet(_Type, strWhere, oState)

                oSession.States.UserFieldState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserFieldState.Result = UserFieldResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.UserFieldState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-783")
            End Try

            Return oRet

        End Function

        Public Shared Function SaveUserFields(ByVal oPage As System.Web.UI.Page, ByVal tbUserFields As DataTable, ByVal bAudit As Boolean) As Boolean
            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserFieldState

            WebServiceHelper.SetState(oState)

            Try
                Dim dsUserFields As DataSet
                If tbUserFields.DataSet IsNot Nothing Then
                    dsUserFields = tbUserFields.DataSet
                Else
                    dsUserFields = New DataSet
                    dsUserFields.Tables.Add(tbUserFields)
                End If

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.UserFieldMethods.SaveUserFields(dsUserFields, oState, bAudit)

                oSession.States.UserFieldState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserFieldState.Result <> UserFieldResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.UserFieldState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-784")
            End Try

            Return bolRet

        End Function

        Public Shared Function SaveTaskFields(ByVal oPage As System.Web.UI.Page, ByVal tbTaskFields As DataTable, ByVal bAudit As Boolean) As Boolean
            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserFieldState

            WebServiceHelper.SetState(oState)

            Try
                Dim dsUserFields As DataSet
                If tbTaskFields.DataSet IsNot Nothing Then
                    dsUserFields = tbTaskFields.DataSet
                Else
                    dsUserFields = New DataSet
                    dsUserFields.Tables.Add(tbTaskFields)
                End If

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.UserFieldMethods.SaveTaskFields(dsUserFields, oState, bAudit)

                oSession.States.UserFieldState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserFieldState.Result <> UserFieldResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.UserFieldState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-785")
            End Try

            Return bolRet

        End Function

        Public Shared Function SaveBusinessCenterFields(ByVal oPage As System.Web.UI.Page, ByVal tbBusinessCenterFields As DataTable, ByVal bAudit As Boolean) As Boolean
            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserFieldState

            WebServiceHelper.SetState(oState)

            Try
                Dim dsUserFields As DataSet
                If tbBusinessCenterFields.DataSet IsNot Nothing Then
                    dsUserFields = tbBusinessCenterFields.DataSet
                Else
                    dsUserFields = New DataSet
                    dsUserFields.Tables.Add(tbBusinessCenterFields)
                End If

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.UserFieldMethods.SaveBusinessCenterFields(dsUserFields, oState, bAudit)

                oSession.States.UserFieldState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserFieldState.Result <> UserFieldResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.UserFieldState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-786")
            End Try

            Return bolRet

        End Function

        Public Shared Function GetTaskField(ByVal oPage As System.Web.UI.Page, ByVal _ID As Integer, ByVal bAudit As Boolean) As roTaskFieldDefinition
            Dim oRet As roTaskFieldDefinition = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserFieldState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of roTaskFieldDefinition) = VTLiveApi.UserFieldMethods.GetTaskField(_ID, oState, bAudit)

                oSession.States.UserFieldState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserFieldState.Result <> UserFieldResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.UserFieldState)
                End If

                oRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-787")
            End Try

            Return oRet

        End Function

        Public Shared Function GetBusinessCenterField(ByVal oPage As System.Web.UI.Page, ByVal _ID As Integer, ByVal bAudit As Boolean) As roBusinessCenterFieldDefinition
            Dim oRet As roBusinessCenterFieldDefinition = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserFieldState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of roBusinessCenterFieldDefinition) = VTLiveApi.UserFieldMethods.GetBusinessCenterField(_ID, oState, bAudit)

                oSession.States.UserFieldState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserFieldState.Result <> UserFieldResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.UserFieldState)
                End If

                oRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-788")
            End Try

            Return oRet

        End Function

        Public Shared Function GetUserField(ByVal oPage As System.Web.UI.Page, ByVal _FieldName As String, ByVal _Type As Types, ByVal _bCheckInProgress As Boolean, ByVal bAudit As Boolean) As roUserField
            Dim oRet As roUserField = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserFieldState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of roUserField) = VTLiveApi.UserFieldMethods.GetUserField(_FieldName, _Type, _bCheckInProgress, oState, bAudit)

                oSession.States.UserFieldState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserFieldState.Result <> UserFieldResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.UserFieldState)
                End If

                oRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-789")
            End Try

            Return oRet

        End Function

        Public Shared Function SaveTaskField(ByVal oPage As System.Web.UI.Page, ByVal _TaskField As roTaskFieldDefinition, ByVal bAudit As Boolean) As Boolean
            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserFieldState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.UserFieldMethods.SaveTaskField(_TaskField, oState, bAudit)

                oSession.States.UserFieldState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserFieldState.Result <> UserFieldResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.UserFieldState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-790")
            End Try

            Return bolRet

        End Function

        Public Shared Function SaveBusinessCenterField(ByVal oPage As System.Web.UI.Page, ByVal _BusinessCenterField As roBusinessCenterFieldDefinition, ByVal bAudit As Boolean) As Boolean
            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserFieldState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.UserFieldMethods.SaveBusinessCenterField(_BusinessCenterField, oState, bAudit)

                oSession.States.UserFieldState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserFieldState.Result <> UserFieldResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.UserFieldState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-791")
            End Try

            Return bolRet

        End Function

        Public Shared Function SaveUserField(ByVal oPage As System.Web.UI.Page, ByVal _UserField As roUserField, ByVal bAudit As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserFieldState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.UserFieldMethods.SaveUserField(_UserField, oState, bAudit)

                oSession.States.UserFieldState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserFieldState.Result <> UserFieldResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.UserFieldState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-792")
            End Try

            Return bolRet

        End Function

        Public Shared Function GetUserFieldValues(ByVal oPage As System.Web.UI.Page, ByVal strUserField As String, ByVal _Type As Types, ByVal strParent As String, ByVal strSeparator As String, ByVal strFieldWhere As String, Optional ByVal xDate As Date = Nothing) As DataTable
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserFieldState

            WebServiceHelper.SetState(oState)

            Try

                If xDate = Nothing Then xDate = Now.Date

                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.UserFieldMethods.GetUserFieldValues(strUserField, _Type, xDate, strParent, strSeparator, strFieldWhere, oState)

                oSession.States.UserFieldState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserFieldState.Result = UserFieldResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.UserFieldState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-793")
            End Try

            Return oRet

        End Function

        Public Shared Function GetEmployeesFromUserFieldWithType(ByVal oPage As System.Web.UI.Page, ByVal strUserField As String, ByVal strUserFieldValue As String, ByVal strFieldWhere As String, ByVal Feature As String, Optional ByVal strFeatureType As String = "U", Optional ByVal xDate As Date = Nothing) As DataTable
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserFieldState

            WebServiceHelper.SetState(oState)

            Try

                If xDate = Nothing Then xDate = Now.Date

                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.UserFieldMethods.GetEmployeesFromUserFieldWithType(strUserField, strUserFieldValue, xDate, strFieldWhere, Feature, strFeatureType, oState)

                oSession.States.UserFieldState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserFieldState.Result = UserFieldResultEnum.NoError Then
                    If wsRet.Value IsNot Nothing AndAlso wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.UserFieldState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-794")
            End Try

            Return oRet

        End Function

        Public Shared Function GetCategories(ByVal oPage As System.Web.UI.Page, ByVal bolOnlyUsed As Boolean) As String()
            Dim bolRet(-1) As String

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserFieldState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of List(Of String)) = VTLiveApi.UserFieldMethods.GetCategories(bolOnlyUsed, oState)

                oSession.States.UserFieldState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserFieldState.Result <> UserFieldResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.UserFieldState)
                End If

                bolRet = wsRet.Value.ToArray
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-795")
            End Try

            Return bolRet

        End Function

        Public Shared Function GetUserFieldList(ByVal oPage As PageBase, ByVal _Type As Types, ByVal bolOnlyUsed As Boolean, ByVal bolCheckInProcess As Boolean, ByVal bolAudit As Boolean) As Generic.List(Of roUserField)
            Dim bolRet As New Generic.List(Of roUserField)

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserFieldState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of List(Of roUserField)) = VTLiveApi.UserFieldMethods.GetUserFieldsList(_Type, bolOnlyUsed, bolCheckInProcess, oState, bolAudit)

                oSession.States.UserFieldState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserFieldState.Result <> UserFieldResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.UserFieldState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-796")
            End Try

            Return bolRet

        End Function

        Public Shared Function CanUserFieldApplyUniqueConstraint(ByVal oPage As PageBase, userFieldName As String, userFieldId As Integer, ByVal bolAudit As Boolean) As Boolean
            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserFieldState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.UserFieldMethods.CanUserFieldApplyUniqueConstraint(userFieldName, userFieldId, oState, bolAudit)

                oSession.States.UserFieldState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserFieldState.Result <> UserFieldResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.UserFieldState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-796")
            End Try

            Return bolRet

        End Function

        Public Shared Function SetUniqueConstraintToUserField(ByVal oPage As PageBase, userFieldName As String, userFieldId As Integer, ByVal bolAudit As Boolean) As Boolean
            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserFieldState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.UserFieldMethods.SetUniqueConstraintToUserField(userFieldName, userFieldId, oState, bolAudit)

                oSession.States.UserFieldState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserFieldState.Result <> UserFieldResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.UserFieldState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-796")
            End Try

            Return bolRet

        End Function

        Public Shared Function GetUserFieldConditionFilter(ByVal oPage As System.Web.UI.Page, ByRef oCondition As roUserFieldCondition) As String
            Dim bolRet As String = ""

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserFieldState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of String) = VTLiveApi.UserFieldMethods.GetUserFieldConditionFilter(oCondition, oState)

                oSession.States.UserFieldState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserFieldState.Result <> UserFieldResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.UserFieldState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-797")
            End Try

            Return bolRet

        End Function

        Public Shared Function GetUserFieldConditionFilterGlobal(ByVal oPage As System.Web.UI.Page, ByVal FilterList As Generic.List(Of roUserFieldCondition),
                                                                 ByVal FilterListCondition As Generic.List(Of String)) As String
            Dim bolRet As String = ""

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserFieldState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of String) = VTLiveApi.UserFieldMethods.GetUserFieldConditionFilterGlobal(FilterList, FilterListCondition, oState)

                oSession.States.UserFieldState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserFieldState.Result <> UserFieldResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.UserFieldState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-798")
            End Try

            Return bolRet

        End Function

        'Public Shared Function GetUserFieldConditionsXml(ByVal oPage As System.Web.UI.Page, ByVal oConditions As Generic.List(Of roUserFieldCondition)) As String
        '    Dim bolRet As String = ""

        '    Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
        '    Dim oState As roWsState = oSession.States.UserFieldState

        '    WebServiceHelper.SetState(oState)

        '    Try

        '        Dim wsRet As roGenericVtResponse(Of String) = VTLiveApi.UserFieldMethods.GetUserFieldConditionsXml(oConditions, oState)

        '        oSession.States.UserFieldState = wsRet.Status
        '        roWsUserManagement.SessionObject = oSession

        '        If oSession.States.UserFieldState.Result <> UserFieldResultEnum.NoError Then
        '            ' Mostrar el error
        '            HelperWeb.ShowError(oPage, oSession.States.UserFieldState)
        '        End If

        '        bolRet = wsRet.Value

        '    Catch ex As Exception
        '        Dim oTmpState As New Robotics.Base.DTOs.roWsState
        '        oTmpState.Result = 1
        '        Dim oLanguage As New roLanguageWeb
        '        oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
        '        HelperWeb.ShowError(oPage, oTmpState, "9-BW01-799")
        '    End Try

        '    Return bolRet

        'End Function
        Public Shared Function GetUserFieldConditionsXml(ByVal oPage As System.Web.UI.Page, ByVal oConditions As List(Of roUserFieldCondition)) As String
            Dim bolRet As String = ""

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserFieldState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of String) = VTLiveApi.UserFieldMethods.GetUserFieldConditionsXml(oConditions, oState)

                oSession.States.UserFieldState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserFieldState.Result <> UserFieldResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.UserFieldState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-800")
            End Try

            Return bolRet

        End Function

        Public Shared Function LoadUserFieldConditionsFromXml(ByVal oPage As System.Web.UI.Page, ByVal strXml As String) As roUserFieldCondition()
            Dim bolRet As roUserFieldCondition() = {}

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserFieldState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of List(Of roUserFieldCondition)) = VTLiveApi.UserFieldMethods.LoadUserFieldConditionsFromXml(strXml, oState)

                oSession.States.UserFieldState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserFieldState.Result <> UserFieldResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.UserFieldState)
                End If

                bolRet = wsRet.Value.ToArray
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-801")
            End Try

            Return bolRet

        End Function

        Public Shared Function GetTaskTemplateFieldsDataSet(ByVal oPage As PageBase, ByVal _IDTaskTemplate As Integer) As DataTable
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserTaskFieldState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.UserFieldMethods.GetTaskTemplateFieldsDataSet(_IDTaskTemplate, oState)

                oSession.States.UserTaskFieldState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserFieldState.Result = TaskFieldResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.UserFieldState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-802")
            End Try

            Return oRet
        End Function

        Public Shared Function GetAvailableTaskTemplateFieldsDataSet(ByVal oPage As PageBase, ByVal _IDTaskTemplate As Integer) As DataTable
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserTaskFieldState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.UserFieldMethods.GetAvailableTaskTemplateFieldsDataSet(_IDTaskTemplate, oState)

                oSession.States.UserTaskFieldState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserFieldState.Result = TaskFieldResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.UserFieldState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-803")
            End Try

            Return oRet

        End Function

        Public Shared Function GetProjectTemplateFieldsDataSet(ByVal oPage As PageBase, ByVal _IDTaskTemplate As Integer) As DataTable
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserTaskFieldState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.UserFieldMethods.GetProjectTemplateFieldsDataSet(_IDTaskTemplate, oState)

                oSession.States.UserTaskFieldState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserFieldState.Result = TaskFieldResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.UserFieldState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-804")
            End Try

            Return oRet
        End Function

        Public Shared Function GetAvailableProjectTemplateFieldsDataSet(ByVal oPage As PageBase, ByVal _IDTaskTemplate As Integer) As DataTable
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserTaskFieldState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.UserFieldMethods.GetAvailableProjectTemplateFieldsDataSet(_IDTaskTemplate, oState)

                oSession.States.UserTaskFieldState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserFieldState.Result = TaskFieldResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.UserFieldState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-805")
            End Try

            Return oRet

        End Function

        Public Shared Function SaveProjectTemplateFields(ByVal oPage As PageBase, ByVal IDProjectTemplate As Integer, ByVal _UserFields As Generic.List(Of roTaskFieldProjectTemplate)) As Boolean
            Dim bolRet As String = ""

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserTaskFieldState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.UserFieldMethods.SaveProjectTemplateFields(IDProjectTemplate, _UserFields, oState)

                oSession.States.UserTaskFieldState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserFieldState.Result <> TaskFieldResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.UserFieldState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-806")
            End Try

            Return bolRet

        End Function

        Public Shared Function SaveTaskTemplateFields(ByVal oPage As PageBase, ByVal IDTaskTemplate As Integer, ByVal _UserFields As Generic.List(Of roTaskFieldTaskTemplate)) As Boolean
            Dim bolRet As String = ""

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserTaskFieldState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.UserFieldMethods.SaveTaskTemplateFields(IDTaskTemplate, _UserFields, oState)

                oSession.States.UserTaskFieldState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserFieldState.Result <> TaskFieldResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.UserFieldState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-807")
            End Try

            Return bolRet

        End Function

        Public Shared Function LastErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.UserFieldState.ErrorText
            End If
            Return strRet
        End Function

    End Class

End Namespace