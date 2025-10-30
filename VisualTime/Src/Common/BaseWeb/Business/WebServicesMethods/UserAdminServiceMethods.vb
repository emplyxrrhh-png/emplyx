Imports System.Web.UI
Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.UsersAdmin
Imports Robotics.UsersAdmin.Business

Namespace API

    Public NotInheritable Class UserAdminServiceMethods

        Public Shared Function GetUserAdmins(ByVal oPage As Page) As wscUserAdminList

            Dim oRet As wscUserAdminList = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.WscState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of wscUserAdminList) = VTLiveApi.UserAdminMethods.GetUserAdmins(oState)
                oRet = response.Value

                oSession.States.WscState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.WscState.Result <> WscResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.WscState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-744")
            End Try

            Return oRet

        End Function

        Public Shared Function GetUserAdmin(ByVal oPage As Page, ByVal intIDPassport As Integer, Optional ByVal passportTicket As roPassportTicket = Nothing, Optional ByVal excludeState As Boolean = False) As wscUserAdmin

            Dim oRet As wscUserAdmin = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.WscState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of wscUserAdmin) = VTLiveApi.UserAdminMethods.GetUserAdmin(intIDPassport, oState)
                oRet = response.Value

                oSession.States.WscState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.WscState.Result <> WscResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.WscState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-745")
            End Try

            Return oRet

        End Function

        Public Shared Function GetConnectionString(ByVal oPage As Page, ByVal intIDPassport As Integer) As String

            Dim oRet As String = ""

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserAdminSecurityState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of String) = VTLiveApi.UserAdminMethods.GetConnectionString(intIDPassport, oState)
                oRet = response.Value

                oSession.States.UserAdminSecurityState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.WscState.Result <> DTOs.SecurityResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.WscState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-746")
            End Try

            Return oRet

        End Function

        Public Shared Function UpdatePassportNameAndLanguage(ByVal oPage As Page, ByVal idPassport As Integer, ByVal sNewName As String, ByVal idLang As Integer) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.WscState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.UserAdminMethods.UpdatePassportNameAndLanguage(idPassport, sNewName, idLang, oState)
                bolRet = response.Value

                oSession.States.WscState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.WscState.Result <> WscResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.WscState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-747")
            End Try

            Return bolRet

        End Function

        Public Shared Function UpdateUserAdmin(ByVal oPage As Page, ByVal oUser As wscUserAdmin) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.WscState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.UserAdminMethods.UpdateUserAdmin(oUser, oState)
                bolRet = response.Value

                oSession.States.WscState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.WscState.Result <> WscResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.WscState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-747")
            End Try

            Return bolRet

        End Function

        Public Shared Function GetPassport(ByVal oPage As Page, ByVal intIDPassport As Integer, ByVal loadType As DTOs.LoadType, Optional ByVal DescryptPwds As Boolean = False, Optional ByVal excludeState As Boolean = False, Optional ByVal passportTicket As roPassportTicket = Nothing) As roPassport

            Dim oRet As roPassport = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserAdminSecurityState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of roPassport) = VTLiveApi.UserAdminMethods.GetPassport(intIDPassport, loadType, DescryptPwds, oState)
                oRet = response.Value

                oSession.States.UserAdminSecurityState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserAdminSecurityState.Result <> DTOs.SecurityResultEnum.NoError AndAlso
                    oSession.States.UserAdminSecurityState.Result <> DTOs.SecurityResultEnum.PassportDoesNotExists Then
                    HelperWeb.ShowError(oPage, oSession.States.UserAdminSecurityState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-748")
            End Try

            Return oRet

        End Function

        Public Shared Function IsRoboticsUserOrConsultant(ByVal oPage As Page, ByVal intIDPassport As Integer) As Boolean

            Dim oRet As Boolean = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserAdminSecurityState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.UserAdminMethods.IsRoboticsUserOrConsultant(intIDPassport, oState)
                oRet = response.Value

                oSession.States.UserAdminSecurityState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserAdminSecurityState.Result <> DTOs.SecurityResultEnum.NoError AndAlso
                    oSession.States.UserAdminSecurityState.Result <> DTOs.SecurityResultEnum.PassportDoesNotExists Then
                    HelperWeb.ShowError(oPage, oSession.States.UserAdminSecurityState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-748")
            End Try

            Return oRet

        End Function

        Public Shared Function IsConsultant(ByVal oPage As Page, ByVal intIDPassport As Integer) As Boolean

            Dim oRet As Boolean = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserAdminSecurityState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.UserAdminMethods.IsConsultant(intIDPassport, oState)
                oRet = response.Value

                oSession.States.UserAdminSecurityState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserAdminSecurityState.Result <> DTOs.SecurityResultEnum.NoError AndAlso
                    oSession.States.UserAdminSecurityState.Result <> DTOs.SecurityResultEnum.PassportDoesNotExists Then
                    HelperWeb.ShowError(oPage, oSession.States.UserAdminSecurityState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-748")
            End Try

            Return oRet

        End Function

        Public Shared Function IsSupervisorPassport(ByVal oPage As Page, ByVal intIDPassport As Integer, Optional ByVal excludeState As Boolean = False) As Boolean

            Dim oRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserAdminSecurityState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.UserAdminMethods.IsSupervisorPassport(intIDPassport, oState)
                oRet = response.Value

                oSession.States.UserAdminSecurityState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserAdminSecurityState.Result <> DTOs.SecurityResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.UserAdminSecurityState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-749")
            End Try

            Return oRet

        End Function

        Public Shared Function SavePassport(ByVal oPage As Page, ByRef oPassport As roPassport, Optional ByVal EncryptPwds As Boolean = False, Optional ByVal audit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserAdminSecurityState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of (roPassport, Boolean)) = VTLiveApi.UserAdminMethods.SavePassport(oPassport, EncryptPwds, oState, audit)

                oPassport = response.Value.Item1
                bolRet = response.Value.Item2

                oSession.States.UserAdminSecurityState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserAdminSecurityState.Result <> DTOs.SecurityResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.UserAdminSecurityState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-750")
            End Try

            Return bolRet

        End Function

        Public Shared Function DeletePassport(ByVal oPage As Page, ByRef oPassport As roPassport, Optional ByVal updateDriversTasks As Boolean = True) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserAdminSecurityState

            WebServiceHelper.SetStateSmall(oState)

            Try

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.UserAdminMethods.DeletePassport(oPassport, updateDriversTasks, oState)
                bolRet = response.Value

                oSession.States.UserAdminSecurityState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserAdminSecurityState.Result <> DTOs.SecurityResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.UserAdminSecurityState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-751")
            End Try

            Return bolRet

        End Function

        Public Shared Function DeletePassportByID(ByVal oPage As Page, ByRef IDPassport As Integer, Optional ByVal updateDriversTasks As Boolean = True) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserAdminSecurityState

            WebServiceHelper.SetStateSmall(oState)

            Try

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.UserAdminMethods.DeletePassportByID(IDPassport, updateDriversTasks, oState)
                bolRet = response.Value

                oSession.States.UserAdminSecurityState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserAdminSecurityState.Result <> DTOs.SecurityResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.UserAdminSecurityState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-752")
            End Try

            Return bolRet

        End Function

        Public Shared Function DeleteEmployeeFromPassport(ByVal oPage As Page, ByRef oPassport As roPassport, Optional ByVal updateDriversTasks As Boolean = True) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserAdminSecurityState

            WebServiceHelper.SetStateSmall(oState)

            Try

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.UserAdminMethods.DeleteEmployeeFromPassport(oPassport, updateDriversTasks, oState)
                bolRet = response.Value

                oSession.States.UserAdminSecurityState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserAdminSecurityState.Result <> DTOs.SecurityResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.UserAdminSecurityState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-753")
            End Try

            Return bolRet

        End Function

        Public Shared Function CredentialExists(ByVal oPage As Page, ByVal credential As String, ByVal method As AuthenticationMethod, ByVal version As String, ByVal idpassport As Nullable(Of Integer)) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserAdminSecurityState

            WebServiceHelper.SetStateSmall(oState)

            Try

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.UserAdminMethods.CredentialExists(credential, method, version, idpassport, oState)
                bolRet = response.Value

                oSession.States.UserAdminSecurityState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserAdminSecurityState.Result <> DTOs.SecurityResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.UserAdminSecurityState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-756")
            End Try

            Return bolRet

        End Function

        Public Shared Function PasswordExists(ByVal oPage As Page, ByVal password As String, ByVal method As AuthenticationMethod, ByVal version As String, ByVal idpassport As Nullable(Of Integer), ByVal hashPassword As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserAdminSecurityState

            WebServiceHelper.SetStateSmall(oState)

            Try

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.UserAdminMethods.PasswordExists(password, method, version, idpassport, hashPassword, oState)
                bolRet = response.Value

                oSession.States.UserAdminSecurityState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserAdminSecurityState.Result <> DTOs.SecurityResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.UserAdminSecurityState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-757")
            End Try
            Return bolRet

        End Function

        Public Shared Function MaxCredentialvalue(ByVal oPage As Page, ByVal method As AuthenticationMethod, ByVal version As String) As Long

            Dim lngRet As Long = 0

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserAdminSecurityState

            WebServiceHelper.SetStateSmall(oState)

            Try

                Dim response As roGenericVtResponse(Of Long) = VTLiveApi.UserAdminMethods.MaxCredentialValue(method, version, oState)
                lngRet = response.Value

                oSession.States.UserAdminSecurityState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserAdminSecurityState.Result <> DTOs.SecurityResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.UserAdminSecurityState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-758")
            End Try

            Return lngRet

        End Function

        Public Shared Function GetPassportsLite(ByVal oPage As Page, ByVal loadType As DTOs.LoadType) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserAdminSecurityState

            WebServiceHelper.SetStateSmall(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.UserAdminMethods.GetLitePassports(loadType, oState)

                oSession.States.UserAdminSecurityState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserAdminSecurityState.Result = DTOs.SecurityResultEnum.NoError Then
                    If response.Value.Tables.Count > 0 Then
                        oRet = response.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.UserAdminSecurityState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-760")
            End Try

            Return oRet

        End Function

        Public Shared Function GetSupervisorPassports(ByVal oPage As Page) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserAdminSecurityState

            WebServiceHelper.SetStateSmall(oState)

            Try
                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.UserAdminMethods.GetSupervisorPassports(oState)

                oSession.States.UserAdminSecurityState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserAdminSecurityState.Result = DTOs.SecurityResultEnum.NoError Then
                    If response.Value.Tables.Count > 0 Then
                        oRet = response.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.UserAdminSecurityState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-760")
            End Try

            Return oRet

        End Function

        Public Shared Function GetAuditPassports(ByVal oPage As Page) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserAdminSecurityState

            WebServiceHelper.SetStateSmall(oState)

            Try
                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.UserAdminMethods.GetAuditPassports(oState)

                oSession.States.UserAdminSecurityState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserAdminSecurityState.Result = DTOs.SecurityResultEnum.NoError Then
                    If response.Value.Tables.Count > 0 Then
                        oRet = response.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.UserAdminSecurityState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-761")
            End Try

            Return oRet

        End Function

        Public Shared Function GetPassportsByParent(ByVal oPage As Page, ByVal idParentPassport As Nullable(Of Integer), ByVal groupType As String) As roPassport()

            Dim oRet As roPassport() = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserAdminSecurityState

            WebServiceHelper.SetStateSmall(oState)

            Try

                Dim response As roGenericVtResponse(Of List(Of roPassport)) = VTLiveApi.UserAdminMethods.GetPassportsByParent(idParentPassport, groupType, oState)
                oRet = response.Value.ToArray

                oSession.States.UserAdminSecurityState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserAdminSecurityState.Result <> DTOs.SecurityResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.UserAdminSecurityState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-762")
            End Try

            Return oRet

        End Function

        Public Shared Function GetPassportsByParentLite(ByVal oPage As Page, ByVal idParentPassport As Nullable(Of Integer), ByVal groupType As String) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserAdminSecurityState

            WebServiceHelper.SetStateSmall(oState)

            Try
                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.UserAdminMethods.GetPassportsByParentLite(idParentPassport, groupType, oState)

                oSession.States.UserAdminSecurityState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserAdminSecurityState.Result = DTOs.SecurityResultEnum.NoError Then
                    If response.Value.Tables.Count > 0 Then
                        oRet = response.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.UserAdminSecurityState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-763")
            End Try

            Return oRet

        End Function

#Region "Features"

        Public Shared Function GetFeaturesList(ByVal oPage As Page, ByVal type As String) As Feature()

            Dim oRet As Feature() = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserAdminSecurityState

            WebServiceHelper.SetState(oState)

            Try
                Dim response As roGenericVtResponse(Of List(Of Feature)) = VTLiveApi.UserAdminMethods.GetFeaturesList(type, oState)
                oRet = response.Value.ToArray

                oSession.States.UserAdminSecurityState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserAdminSecurityState.Result <> DTOs.SecurityResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.UserAdminSecurityState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-764")
            End Try

            Return oRet

        End Function

        Public Shared Function GetFeaturesFromPassport(ByVal oPage As Page, ByVal idPassport As Integer, ByVal idFeature As Nullable(Of Integer), ByVal type As String) As Feature()

            Dim oRet As Feature() = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserAdminSecurityState

            WebServiceHelper.SetStateSmall(oState)

            Try
                Dim response As roGenericVtResponse(Of List(Of Feature)) = VTLiveApi.UserAdminMethods.GetFeaturesFromPassport(idPassport, idFeature, type, oState)
                oRet = response.Value.ToArray

                oSession.States.UserAdminSecurityState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserAdminSecurityState.Result <> DTOs.SecurityResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.UserAdminSecurityState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-765")
            End Try

            Return oRet

        End Function

        Public Shared Function GetFeaturesFromPassportAll(ByVal oPage As Page, ByVal idPassport As Integer, ByVal type As String) As Feature()

            Dim oRet As Feature() = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserAdminSecurityState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of List(Of Feature)) = VTLiveApi.UserAdminMethods.GetFeaturesFromPassportAll(idPassport, type, oState)
                oRet = response.Value.ToArray

                oSession.States.UserAdminSecurityState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserAdminSecurityState.Result <> DTOs.SecurityResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.UserAdminSecurityState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-766")
            End Try

            Return oRet

        End Function

        Public Shared Function GetFeaturePermissions(ByVal oPage As Page, ByVal idPassport As Integer, ByVal idFeature As Integer, ByVal FeatureType As String) As Permission()

            Dim oRet As Permission() = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserAdminSecurityState

            WebServiceHelper.SetStateSmall(oState)

            Try

                Dim response As roGenericVtResponse(Of List(Of Permission)) = VTLiveApi.UserAdminMethods.GetFeaturePermissions(idPassport, idFeature, FeatureType, oState)
                oRet = response.Value.ToArray

                oSession.States.UserAdminSecurityState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserAdminSecurityState.Result <> DTOs.SecurityResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.UserAdminSecurityState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-767")
            End Try

            Return oRet

        End Function

        Public Shared Function SetFeaturePermission(ByVal oPage As Page, ByVal idPassport As Integer, ByVal idFeature As Integer, ByVal FeatureType As String, ByVal _permission As Permission, ByRef FeaturesChanged As List(Of Feature)) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserAdminSecurityState

            WebServiceHelper.SetStateSmall(oState)

            Try

                Dim response As roGenericVtResponse(Of (Boolean, List(Of Feature))) = VTLiveApi.UserAdminMethods.SetFeaturePermission(idPassport, idFeature, FeatureType, _permission, FeaturesChanged, oState)
                FeaturesChanged = response.Value.Item2
                bolRet = response.Value.Item1

                oSession.States.UserAdminSecurityState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserAdminSecurityState.Result <> DTOs.SecurityResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.UserAdminSecurityState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-768")
            End Try

            Return bolRet

        End Function

        Public Shared Function SetDefaultFeaturePermission(ByVal oPage As Page, ByVal idPassport As Integer, ByVal idFeature As Integer, ByVal FeatureType As String, ByRef FeaturesChanged As List(Of Feature)) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserAdminSecurityState

            WebServiceHelper.SetStateSmall(oState)

            Try

                Dim response As roGenericVtResponse(Of (Boolean, List(Of Feature))) = VTLiveApi.UserAdminMethods.SetDefaultFeaturePermission(idPassport, idFeature, FeatureType, FeaturesChanged, oState)
                FeaturesChanged = response.Value.Item2
                bolRet = response.Value.Item1

                oSession.States.UserAdminSecurityState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserAdminSecurityState.Result <> DTOs.SecurityResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.UserAdminSecurityState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-769")
            End Try

            Return bolRet

        End Function

#End Region

        Public Shared Function GetLanguages(ByVal oPage As Page) As roPassportLanguage()

            Dim oRet As roPassportLanguage() = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserAdminSecurityState

            WebServiceHelper.SetStateSmall(oState)

            Try

                Dim response As roGenericVtResponse(Of roPassportLanguage()) = VTLiveApi.UserAdminMethods.GetLanguages(oState)
                oRet = response.Value

                oSession.States.UserAdminSecurityState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserAdminSecurityState.Result <> DTOs.SecurityResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.UserAdminSecurityState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-775")
            End Try
            Return oRet

        End Function

        Public Shared Function GetLanguageByKey(ByVal oPage As Page, key As String) As roPassportLanguage

            Dim oRet As roPassportLanguage = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserAdminSecurityState

            WebServiceHelper.SetStateSmall(oState)

            Try

                Dim response As roGenericVtResponse(Of roPassportLanguage) = VTLiveApi.UserAdminMethods.GetLanguageByKey(key, oState)
                oRet = response.Value

                oSession.States.UserAdminSecurityState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserAdminSecurityState.Result <> DTOs.SecurityResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.UserAdminSecurityState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-775")
            End Try
            Return oRet

        End Function

        Public Shared Function CreateUserOfPassport(ByVal oPage As Page, ByRef oPassport As roPassport) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserAdminSecurityState

            WebServiceHelper.SetStateSmall(oState)

            Try

                Dim response As roGenericVtResponse(Of (Boolean, roPassport)) = VTLiveApi.UserAdminMethods.CreateUserOfPassport(oPassport, oState)
                bolRet = response.Value.Item1
                oPassport = response.Value.Item2

                oSession.States.UserAdminSecurityState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserAdminSecurityState.Result <> DTOs.SecurityResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.UserAdminSecurityState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-776")
            End Try
            Return bolRet

        End Function

        Public Shared Function CreateUserOfPassports(ByVal oPage As Page, ByRef EmployeeList As Generic.List(Of Integer), ByVal IdGroup As Integer, Optional ByVal encryptPassword As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserAdminSecurityState

            WebServiceHelper.SetStateSmall(oState)

            Try

                Dim strErrorInfo As String = ""
                Dim oPassport As roPassport = Nothing

                For Each id As Integer In EmployeeList
                    oPassport = API.UserAdminServiceMethods.GetPassport(oPage, id, LoadType.Employee, False)
                    If oPassport IsNot Nothing Then
                        If oPassport.GroupType = "E" AndAlso Not oPassport.IDUser.HasValue Then
                            'crear user
                            If Not API.UserAdminServiceMethods.CreateUserOfPassport(oPage, oPassport) Then
                                strErrorInfo = API.UserAdminServiceMethods.SecurityLastErrorText
                            End If
                        End If

                        If strErrorInfo = "" Then
                            oPassport.IDParentPassport = IdGroup

                            If Not API.UserAdminServiceMethods.SavePassport(oPage, oPassport, encryptPassword) Then
                                strErrorInfo = API.UserAdminServiceMethods.SecurityLastErrorText
                            End If
                        End If
                    End If

                    roWsUserManagement.SessionObject = oSession

                    If strErrorInfo <> "" Then Exit For

                Next

                If strErrorInfo = "" Then bolRet = True
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-777")
            End Try
            Return bolRet

        End Function




        Public Shared Function BusinessGroupListInUse(ByVal oPage As Page, ByVal strBusinessGroup As String) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserAdminSecurityState

            WebServiceHelper.SetStateSmall(oState)

            Try

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.UserAdminMethods.BusinessGroupListInUse(strBusinessGroup, oState)
                bolRet = response.Value

                oSession.States.UserAdminSecurityState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserAdminSecurityState.Result <> DTOs.SecurityResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.UserAdminSecurityState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-780")
            End Try

            Return bolRet

        End Function

        Public Shared Function SaveParameterInContext(ByVal oPage As Page, ByVal strParameterName As String, ByVal strParameterValue As String) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserAdminSecurityState

            WebServiceHelper.SetStateSmall(oState)

            Try

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.UserAdminMethods.SaveParameterInContext(strParameterName, strParameterValue, oState)
                bolRet = response.Value

                oSession.States.UserAdminSecurityState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserAdminSecurityState.Result <> DTOs.SecurityResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.UserAdminSecurityState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-780")
            End Try

            Return bolRet

        End Function

        Public Shared Function LastErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.WscState.ErrorText
            End If
            Return strRet
        End Function

        Public Shared Function SecurityLastErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.UserAdminSecurityState.ErrorText
            End If
            Return strRet
        End Function

    End Class

End Namespace