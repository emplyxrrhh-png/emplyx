Imports System.Net
Imports System.Security.Principal
Imports System.Web
Imports System.Web.Mail
Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Common.AdvancedParameter
Imports Robotics.Base.VTBusiness.Terminal
Imports Robotics.Base.VTChannels
Imports Robotics.Base.VTCommuniques
Imports Robotics.Base.VTRequests
Imports Robotics.DataLayer
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Namespace VTPortal

    Public Class SecurityHelper

        Public Shared Function GetFeaturePermission(ByVal idPassport As Integer, ByVal strFeatureAlias As String, Optional ByVal strFeatureType As String = "U") As Permission
            Try
                Return WLHelper.GetPermissionOverFeature(idPassport, strFeatureAlias, strFeatureType)
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::SecurityHelper::GetFeaturePermission")

                Return Permission.None
            End Try

        End Function

        Public Shared Function HasReadModeEnabled(ByVal idEmployee As Integer) As Boolean
            Try
                Return roPassportManager.HasReadModeEnabled(idEmployee)
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::SecurityHelper::HasReadModeEnabled")

                Return False
            End Try

        End Function

        Public Shared Function GetFeaturePermissionByEmployee(ByVal idPassport As Integer, ByVal strFeatureAlias As String, ByVal intIDEmployee As Integer, Optional ByVal strFeatureType As String = "U") As Permission
            Try
                If strFeatureAlias.Split(".").Length > 1 Then
                    ' No es una funcionalidad raiz, miramos los permisos del empleado sobre la funcionalidad raiz (ej: Employees.Type -> Employees)
                    Dim oEmployeePermission As Permission = WLHelper.GetPermissionOverEmployeeOnDate(idPassport, intIDEmployee, strFeatureAlias.Split(".")(0), DateTime.Now, strFeatureType)
                    Dim oFeaturePermission As Permission = WLHelper.GetPermissionOverFeature(idPassport, strFeatureAlias, strFeatureType)
                    If oEmployeePermission > oFeaturePermission Then
                        Return oFeaturePermission
                    Else
                        Return oEmployeePermission
                    End If
                Else
                    Return WLHelper.GetPermissionOverEmployeeOnDate(idPassport, intIDEmployee, strFeatureAlias, DateTime.Now, strFeatureType)
                End If
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::SecurityHelper::GetFeaturePermissionByEmployee")

                Return Permission.None
            End Try
        End Function

        Public Shared Function GetFeaturePermissionByGroup(ByVal idPassport As Integer, ByVal strFeatureAlias As String, ByVal intIDGroup As Integer, Optional ByVal strFeatureType As String = "U") As Permission
            Try
                If strFeatureAlias.Split(".").Length > 1 Then
                    ' No es una funcionalidad raiz, miramos los permisos del empleado sobre la funcionalidad raiz (ej: Employees.Type -> Employees)
                    Dim oGroupPermission As Permission = WLHelper.GetPermissionOverGroup(idPassport, intIDGroup, strFeatureAlias.Split(".")(0), strFeatureType)
                    Dim oFeaturePermission As Permission = WLHelper.GetPermissionOverFeature(idPassport, strFeatureAlias, strFeatureType)
                    If oGroupPermission > oFeaturePermission Then
                        Return oFeaturePermission
                    Else
                        Return oGroupPermission
                    End If
                Else
                    Return WLHelper.GetPermissionOverGroup(idPassport, intIDGroup, strFeatureAlias, strFeatureType)
                End If
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::SecurityHelper::GetFeaturePermissionByGroup")

                Return Permission.None
            End Try
        End Function

        Public Shared Function HasFeaturePermission(ByVal idPassport As Integer, ByVal strFeatureAlias As String, ByVal oPermission As Permission, Optional ByVal strFeatureType As String = "U") As Boolean
            Try
                Return WLHelper.HasPermissionOverFeature(idPassport, strFeatureAlias, strFeatureType, oPermission)
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::SecurityHelper::HasFeaturePermission")

                Return False
            End Try
        End Function

        Public Shared Function HasFeaturePermissionByEmployee(ByVal idPassport As Integer, ByVal strFeatureAlias As String, ByVal oPermission As Permission, ByVal intIDEmployee As Integer, Optional ByVal strFeatureType As String = "U") As Boolean
            Try
                If strFeatureAlias.Split(".").Length > 1 Then
                    ' No es una funcionalidad raiz, miramos los permisos del empleado sobre la funcionalidad raiz (ej: Employees.Type -> Employees)
                    Dim bolEmployeeHasPermission As Boolean = WLHelper.HasPermissionOverEmployee(idPassport, intIDEmployee, strFeatureAlias.Split(".")(0), oPermission, strFeatureType)
                    Dim bolFeatureHasPermission As Boolean = WLHelper.HasPermissionOverFeature(idPassport, strFeatureAlias, strFeatureType, oPermission)
                    Return (bolEmployeeHasPermission AndAlso bolFeatureHasPermission)
                Else
                    Return WLHelper.HasPermissionOverEmployee(idPassport, intIDEmployee, strFeatureAlias, oPermission, strFeatureType)
                End If
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::SecurityHelper::HasFeaturePermissionByEmployee")
                Return False
            End Try

        End Function

        Public Shared Function HasFeaturePermissionByGroup(ByVal idPassport As Integer, ByVal strFeatureAlias As String, ByVal oPermission As Permission, ByVal intIDGroup As Integer, Optional ByVal strFeatureType As String = "U") As Boolean
            Try
                If strFeatureAlias.Split(".").Length > 1 Then
                    ' No es una funcionalidad raiz, miramos los permisos del empleado sobre la funcionalidad raiz (ej: Employees.Type -> Employees)
                    Dim bolGroupHasPermission As Boolean = WLHelper.HasPermissionOverGroup(idPassport, intIDGroup, strFeatureAlias.Split(".")(0), strFeatureType, oPermission)
                    Dim bolFeatureHasPermission As Boolean = WLHelper.HasPermissionOverFeature(idPassport, strFeatureAlias, strFeatureType, oPermission)
                    Return (bolGroupHasPermission And bolFeatureHasPermission)
                Else
                    Return WLHelper.HasPermissionOverGroup(idPassport, intIDGroup, strFeatureAlias, strFeatureType, oPermission)
                End If
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::SecurityHelper::HasFeaturePermissionByGroup")
                Return False
            End Try

        End Function

        Public Shared Function GetEmployeePermissions(ByVal oPassport As roPassportTicket, ByVal terminal As roTerminal, ByVal ostate As Requests.roRequestState) As PermissionList
            Dim lrret As New PermissionList

            Try
                Dim RequestTypes() As eRequestType = RequestsHelper.GetPortalEnabled()

                Dim oRanquingParam As Boolean = roTypes.Any2Boolean(New AdvancedParameter.roAdvancedParameter("VTPortal.RequestsHasRanquings", New AdvancedParameter.roAdvancedParameterState(ostate.IDPassport)).Value)
                Dim oContract As VTEmployees.Contract.roContract = Nothing

                If oPassport.IDEmployee.HasValue Then
                    oContract = VTEmployees.Contract.roContract.GetActiveContract(oPassport.IDEmployee, New VTEmployees.Contract.roContractState(ostate.IDPassport), False)
                End If

                Dim oPermList As New Generic.List(Of ReqPermission)
                For Each oRequestTypeSecurity As eRequestType In RequestTypes
                    Dim oPermission As New ReqPermission
                    oPermission.RequestType = oRequestTypeSecurity

                    If oRequestTypeSecurity = eRequestType.Telecommute Then
                        lrret.CanCreateRequests = True
                        oPermission.Permission = True
                    Else
                        If VTPortal.SecurityHelper.GetFeaturePermission(oPassport.ID, New Requests.roRequestTypeSecurity(oRequestTypeSecurity, ostate).EmployeeFeatureName, "E") >= Permission.Write Then
                            lrret.CanCreateRequests = True
                            oPermission.Permission = True
                        Else
                            oPermission.Permission = False
                        End If
                    End If

                    oPermission.AutomaticReasons = {}
                    oPermission.HasRankings = False

                    If oPermission.Permission Then
                        If oContract IsNot Nothing AndAlso oContract.LabAgree IsNot Nothing Then
                            'Buscamos las reglas que sean del tipo de la solicitud y de tipo 4(automaticas)
                            Dim oRequestRules = oContract.LabAgree.LabAgreeRequestRules.FindAll(Function(x) x.IDRequestType = oRequestTypeSecurity And x.IDRuleType = eRequestRuleType.MinCoverageRequiered)
                            If oRequestRules IsNot Nothing AndAlso oRequestRules.Count > 0 Then
                                For Each oRule In oRequestRules
                                    Dim tmpList As Generic.List(Of Integer) = oPermission.AutomaticReasons.ToList()
                                    tmpList.AddRange(oRule.Definition.IDReasons)
                                    oPermission.AutomaticReasons = tmpList.ToArray()
                                Next
                                'Indicamos si se deben mostrar ranquings según parametros.
                                oPermission.HasRankings = oRanquingParam
                            End If
                        End If
                    End If

                    oPermList.Add(oPermission)
                Next

                Dim oServerLicense As New roServerLicense()

                Dim oPass As roPassport = roPassportManager.GetPassport(oPassport.ID)

                lrret.DocumentsEnabled = oServerLicense.FeatureIsInstalled("Feature\Documents")
                lrret.LeavesEnabled = oServerLicense.FeatureIsInstalled("Feature\Absences")
                Dim oGeolocalizationParameter As New Common.AdvancedParameter.roAdvancedParameter("VTPortal.GeolocalizationConfiguration", New Common.AdvancedParameter.roAdvancedParameterState)

                If Not IsDBNull(oGeolocalizationParameter.Value) AndAlso oGeolocalizationParameter.Value <> String.Empty Then
                    Dim oPortalGeolocalizationConfiguration As roPortalGeolocalizationConfiguration = New roPortalGeolocalizationConfiguration()
                    oPortalGeolocalizationConfiguration = roJSONHelper.DeserializeNewtonSoft(oGeolocalizationParameter.Value, GetType(roPortalGeolocalizationConfiguration))
                    Select Case (oPortalGeolocalizationConfiguration.Geolocalization)
                        Case 1
                            lrret.MustUseGPS = True
                        Case 2
                            lrret.MustUseGPS = False
                        Case 3
                            lrret.MustUseGPS = oPass.LocationRequiered
                    End Select
                Else
                    lrret.MustUseGPS = oPass.LocationRequiered
                End If

                Dim oZoneRequiredParameter As New Common.AdvancedParameter.roAdvancedParameter("VTPortal.PunchOptions", New Common.AdvancedParameter.roAdvancedParameterState)

                If Not IsDBNull(oZoneRequiredParameter.Value) AndAlso oZoneRequiredParameter.Value <> String.Empty Then

                    Dim oPunchOptionsConfiguration As roPortalPunchOptions = New roPortalPunchOptions()
                    oPunchOptionsConfiguration = roJSONHelper.DeserializeNewtonSoft(oZoneRequiredParameter.Value, GetType(roPortalPunchOptions))
                    If oPunchOptionsConfiguration.ZoneRequired Then
                        lrret.MustSelectZone = True
                    Else
                        lrret.MustSelectZone = False
                    End If
                Else
                    lrret.MustSelectZone = False
                End If

                ' Si el terminal es de tipo "TIME GATE" y tiene un lector, y este lector tiene una zona asociada, no se debe seleccionar zona
                If terminal IsNot Nothing AndAlso
                   terminal.Type.ToUpper = "TIME GATE" AndAlso
                   terminal.ReaderByID(1) IsNot Nothing AndAlso
                   terminal.ReaderByID(1).IDZone.HasValue AndAlso
                   terminal.ReaderByID(1).IDZone > 0 Then
                    lrret.MustUseGPS = False
                    lrret.MustSelectZone = False
                End If

                ' Si el terminal es NFC, no se debe solicitar Zona
                If lrret.MustSelectZone AndAlso terminal IsNot Nothing AndAlso terminal.Type.ToUpper = "NFC" Then
                    lrret.MustSelectZone = False
                End If

                lrret.MustUsePhoto = oPass.PhotoRequiered

                lrret.Punch.SchedulePunch = VTPortal.SecurityHelper.GetFeaturePermission(oPassport.ID, "Punches.Punches", "E") >= Permission.Write
                lrret.Punch.ProductiVPunch = VTPortal.SecurityHelper.GetFeaturePermission(oPassport.ID, "TaskPunches.Punches", "E") >= Permission.Write
                lrret.Punch.CostCenterPunch = VTPortal.SecurityHelper.GetFeaturePermission(oPassport.ID, "Punches.Punches", "E") >= Permission.Write

                lrret.Punch.ScheduleQuery = VTPortal.SecurityHelper.GetFeaturePermission(oPassport.ID, "Punches.Query", "E") >= Permission.Read
                lrret.Punch.ProductiVQuery = VTPortal.SecurityHelper.GetFeaturePermission(oPassport.ID, "TaskPunches.Query", "E") >= Permission.Read

                lrret.Punch.Query = lrret.Punch.ScheduleQuery OrElse lrret.Punch.ProductiVQuery

                lrret.Schedule.ProductivAccruals = VTPortal.SecurityHelper.GetFeaturePermission(oPassport.ID, "Tasktotals.Query", "E") >= Permission.Read
                lrret.Schedule.ScheduleAccruals = VTPortal.SecurityHelper.GetFeaturePermission(oPassport.ID, "Totals.Query", "E") >= Permission.Read
                lrret.Schedule.QuerySchedule = VTPortal.SecurityHelper.GetFeaturePermission(oPassport.ID, "Planification.Query", "E") >= Permission.Read
                lrret.DailyRecord.QueryDailyRecord = VTPortal.SecurityHelper.GetFeaturePermission(oPassport.ID, "Punches.DailyRecord", "E") >= Permission.Read
                lrret.UserFieldQuery = VTPortal.SecurityHelper.GetFeaturePermission(oPassport.ID, "UserFields.Query", "E") >= Permission.Read
                lrret.Forecast = VTPortal.SecurityHelper.GetFeaturePermission(oPassport.ID, "Planification.Forecast", "E") >= Permission.Write
                lrret.Terminals = VTPortal.SecurityHelper.GetFeaturePermission(oPassport.ID, "Terminals.Definition", "U") >= Permission.Admin

                lrret.Requests = oPermList.ToArray
                lrret.Status = ErrorCodes.OK
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::SecurityHelper::GetEmployeePermissions")
            End Try

            Return lrret
        End Function

        Public Shared Function RecoverEmployeePassword(ByVal strUserName As String, ByVal strEmail As String, ByVal ostate As roSecurityState) As StdResponse
            Dim lrret As New StdResponse

            Try
                lrret.Status = ErrorCodes.OK
                lrret.Result = WLHelper.RecoverEmployeePassword(strUserName, strEmail, roAppType.VTPortal)

                If Not lrret.Result Then
                    lrret.Status = ErrorCodes.USER_OR_EMAIL_NOTFOUND
                End If
            Catch ex As Exception
                lrret.Result = False
                lrret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::SecurityHelper::RecoverEmployeePassword")
            End Try

            Return lrret
        End Function

        Public Shared Function GetLastLogin(ByVal idPassport As Integer, ByVal ostate As roSecurityState) As DateTime
            Dim lrret As New DateTime
            Try
                lrret = WLHelper.GetLastLogin(idPassport)
            Catch ex As Exception
            End Try
            Return lrret
        End Function

        Public Shared Function UpdateLastLogin(ByVal idPassport As Integer, ByVal ostate As roSecurityState) As StdResponse
            Dim lrret As New StdResponse

            Try
                lrret.Status = ErrorCodes.OK
                lrret.Result = WLHelper.UpdateLastLogin(idPassport)

                If Not lrret.Result Then
                    lrret.Status = ErrorCodes.GENERAL_ERROR
                End If
            Catch ex As Exception
                lrret.Result = False
                lrret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::SecurityHelper::UpdateLastLogin")
            End Try

            Return lrret
        End Function

        Public Shared Function DeleteFirebaseToken(ByVal idPassport As Integer, ByVal uuid As String, ByVal ostate As roSecurityState) As roTokenResponse
            Dim lrret As New roTokenResponse

            Try
                lrret.Status = ErrorCodes.OK
                lrret.Result = WLHelper.DeleteFirebaseToken(idPassport, uuid)

                If Not lrret.Result Then
                    lrret.Status = ErrorCodes.GENERAL_ERROR
                End If
            Catch ex As Exception
                lrret.Result = False
                lrret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::SecurityHelper::RegisterFirebaseToken")
            End Try

            Return lrret
        End Function

        Public Shared Function RegisterFirebaseToken(ByVal token As String, ByVal uuid As String, ByVal idPassport As Integer, ByVal ostate As roSecurityState) As roTokenResponse
            Dim lrret As New roTokenResponse

            Try
                lrret.Status = ErrorCodes.OK
                lrret.Result = WLHelper.RegisterFirebaseToken(token, uuid, idPassport)

                If Not lrret.Result Then
                    lrret.Status = ErrorCodes.GENERAL_ERROR
                End If
            Catch ex As Exception
                lrret.Result = False
                lrret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::SecurityHelper::RegisterFirebaseToken")
            End Try

            Return lrret
        End Function

        Public Shared Function ChangePassword(ByVal idPassport As Integer, ByVal oldPassword As String, ByVal newPassword As String, token As String, ByVal ostate As roSecurityState) As StdResponse
            Dim lrret As New StdResponse

            Try
                lrret.Status = ErrorCodes.OK

                Dim resultCode As PasswordLevelError = roSecurityOptions.IsValidPwd(newPassword, Nothing, idPassport, ostate, True, oldPassword, True)

                Select Case (resultCode)
                    Case PasswordLevelError.Low_Error
                        lrret.Status = ErrorCodes.LOGIN_RESULT_LOW_STRENGHT_ERROR
                    Case PasswordLevelError.Medium_Error
                        lrret.Status = ErrorCodes.LOGIN_RESULT_MEDIUM_STRENGHT_ERROR
                    Case PasswordLevelError.High_Error
                        lrret.Status = ErrorCodes.LOGIN_RESULT_HIGH_STRENGHT_ERROR
                    Case PasswordLevelError.No_Error
                        Dim oManager As New roPassportManager
                        Dim oPassport As roPassport = oManager.LoadPassport(idPassport, LoadType.Passport)

                        oPassport.AuthenticationMethods.PasswordRow.Password = CryptographyHelper.EncryptWithMD5(newPassword)
                        oPassport.AuthenticationMethods.PasswordRow.LastUpdatePassword = DateTime.Now
                        oPassport.AuthenticationMethods.PasswordRow.RowState = RowState.UpdateRow


                        AuthHelper.SetPassportKeyValidated(idPassport, True, token, True)


                        oManager.Save(oPassport)
                        lrret.Result = True
                End Select
            Catch ex As Exception
                lrret.Result = False
                lrret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::SecurityHelper::ChangePassword")
            End Try

            Return lrret
        End Function

        Public Shared Function ResetPasswordToNew(ByVal userName As String, ByVal requestKey As String, ByVal newPassword As String, ByVal appType As roAppType, token As String, ByVal ostate As roSecurityState) As StdResponse
            Dim lrret As New StdResponse

            Try
                lrret.Status = ErrorCodes.OK

                Dim oPassport As roPassport = roPassportManager.GetPassportByCredential(userName, AuthenticationMethod.Password, "")

                If oPassport IsNot Nothing Then

                    Dim resultCode As PasswordLevelError = roSecurityOptions.IsValidPwd(newPassword, Nothing, oPassport.ID, ostate)

                    Select Case (resultCode)
                        Case PasswordLevelError.Low_Error
                            lrret.Status = ErrorCodes.LOGIN_RESULT_LOW_STRENGHT_ERROR
                        Case PasswordLevelError.Medium_Error
                            lrret.Status = ErrorCodes.LOGIN_RESULT_MEDIUM_STRENGHT_ERROR
                        Case PasswordLevelError.High_Error
                            lrret.Status = ErrorCodes.LOGIN_RESULT_HIGH_STRENGHT_ERROR
                        Case PasswordLevelError.No_Error
                            If WLHelper.ValidateRecoverPasswordRequestKey(requestKey, oPassport.ID, appType) Then
                                oPassport.AuthenticationMethods.PasswordRow.Password = CryptographyHelper.EncryptWithMD5(newPassword)
                                oPassport.AuthenticationMethods.PasswordRow.LastUpdatePassword = DateTime.Now
                                oPassport.AuthenticationMethods.PasswordRow.RowState = RowState.UpdateRow

                                Dim oManager As New roPassportManager
                                oManager.Save(oPassport)

                                AuthHelper.SetPassportKeyValidated(oPassport.ID, True, token, True)

                                lrret.Result = True
                            Else
                                lrret.Status = ErrorCodes.RECOVERKEY_NOTFOUND
                            End If
                    End Select
                Else
                    lrret.Status = ErrorCodes.RECOVERKEY_NOTFOUND
                End If
            Catch ex As Exception
                lrret.Result = False
                lrret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::SecurityHelper::ChangePassword")
            End Try

            Return lrret
        End Function

        Public Shared Function AuthenticatedSessionInfo(oSupervisor As roPassportTicket, oIdentity As roPassportTicket, bIsSupervisor As Boolean, oAuthMetod As AuthenticationMethod, bIsModeapp As Boolean, oState As roSecurityState, oToken As String) As LoggedInUserInfo
            Dim lrret As New LoggedInUserInfo

            Try
                lrret.Status = ErrorCodes.OK

                If oSupervisor IsNot Nothing Then
                    lrret = VTPortal.SecurityHelper.GetLoggedInUserInfo(oSupervisor, HttpContext.Current.Request, bIsSupervisor, oAuthMetod, oToken, bIsModeapp, oState)
                Else
                    lrret = VTPortal.SecurityHelper.GetLoggedInUserInfo(oIdentity, HttpContext.Current.Request, bIsSupervisor, oAuthMetod, oToken, bIsModeapp, oState)
                End If

                lrret.IsSaas = True

                Dim oServerLicense As New roServerLicense
                If oServerLicense.FeatureIsInstalled("Feature\CegidHub") Then
                    lrret.IsCegidHub = True
                End If

                Dim oCommunicateManager As New roCommuniqueManager
                If oServerLicense.FeatureIsInstalled("Feature\AdvancedCommuniques") OrElse oCommunicateManager.GetAllCommuniques(lrret.EmployeeId).Count > 0 Then
                    lrret.IsCommuniquesEnabled = True 'Activamos comunicados si tiene módulo activo o si tiene comunicados en BD (pbi: 1190266)
                End If

                Dim oChannelsManager As New roChannelManager
                If (oServerLicense.FeatureIsInstalled("Feature\Channels") OrElse oServerLicense.FeatureIsInstalled("Feature\Complaints")) OrElse oChannelsManager.GetAllChannels(lrret.EmployeeId).Count > 0 Then
                    lrret.IsChannelsEnabled = True
                End If

                lrret.SSOServerEnabled = False
                lrret.SSOUserLoggedIn = False
                lrret.SSOUserName = ""

                Dim cName As String = RoAzureSupport.GetCompanyName()

                Dim oVal As String = roCacheManager.GetInstance.GetAdvParametersCache(cName, "VTPortal.HeaderConfiguration")
                If oVal <> String.Empty Then
                    lrret.HeaderMD5 = CryptographyHelper.EncryptWithMD5(oVal)
                Else
                    lrret.HeaderMD5 = ""
                End If

                lrret.ShowPermissionsIcon = roTypes.Any2Boolean(roCacheManager.GetInstance.GetAdvParametersCache(cName, "VTPortal.ShowPermissionsIcon"))
                lrret.DefaultVersion = roTypes.Any2String(roCacheManager.GetInstance.GetAdvParametersCache(cName, "VTPortal.DefaultVersion"))
            Catch ex As Exception
                lrret.Status = ErrorCodes.GENERAL_ERROR
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::SecurityHelper::AuthenticatedSessionInfo")
            End Try

            Return lrret
        End Function

        Public Shared Function GetLoggedInUserInfo(ByVal oLoggedInPassport As roPassportTicket, ByVal oRequest As Web.HttpRequest, ByVal bIsSupervisor As Boolean, ByVal oAuthMetod As AuthenticationMethod, ByVal token As String, bIsModeapp As Boolean, oState As roSecurityState) As LoggedInUserInfo
            Dim lrret As New LoggedInUserInfo

            Try

                Dim cName As String = RoAzureSupport.GetCompanyName()

                If Not String.IsNullOrEmpty(cName) Then
                    lrret.ApiVersion = roTypes.Any2Integer(roCacheManager.GetInstance.GetAdvParametersCache(cName, AdvancedParameterType.VTPortalApiVersion.ToString()))


                    Dim oPassportCredential As String
                    If oAuthMetod = AuthenticationMethod.Password Then
                        oPassportCredential = oLoggedInPassport.AuthCredential
                    Else
                        oPassportCredential = oLoggedInPassport.CegidIdCredential
                    End If

                    Dim oAdfs As String = roCacheManager.GetInstance.GetAdvParametersCache(cName, "ADFSEnabled")
                    Dim ssoLogin As Boolean = (oAdfs = "1" AndAlso oPassportCredential.IndexOf("\") <> -1)

                    If ssoLogin OrElse token <> String.Empty Then
                        lrret.AnywhereLicense = True
                        lrret.RequiereFineLocation = (lrret.AnywhereLicense AndAlso roTypes.Any2Boolean(roCacheManager.GetInstance.GetAdvParametersCache(cName, "VTPortal.RequiereFineLocation")))
                        lrret.SupervisorPortalEnabled = bIsSupervisor
                        lrret.Consent = New roPassportConsent() With {.IsValid = True, .IDPassport = oLoggedInPassport.ID, .Type = ConsentTypeEnum._Portal}
                        lrret.LicenseAccepted = lrret.Consent.IsValid
                        lrret.ShowForbiddenSections = roTypes.Any2Boolean(roCacheManager.GetInstance.GetAdvParametersCache(cName, "VTPortal.ShowForbiddenSections"))
                        lrret.ShowLogoutHome = roTypes.Any2Boolean(roCacheManager.GetInstance.GetAdvParametersCache(cName, "VTPortalShowLogoutHome"))

                        Dim oLicSupport As New roServerLicense()
                        Dim dailyRecordInstalled As Boolean = oLicSupport.FeatureIsInstalled("Feature\DailyRecord")

                        If dailyRecordInstalled Then
                            lrret.DailyRecordEnabled = Security.WLHelper.GetPermissionOverFeature(oLoggedInPassport.ID, "Punches.DailyRecord", "E")
                        Else
                            lrret.DailyRecordEnabled = False
                        End If

                        If lrret.DailyRecordEnabled Then
                            lrret.DailyRecordPatternEnabled = roTypes.Any2Boolean(roCacheManager.GetInstance.GetAdvParametersCache(cName, "VTPortal.DailyRecordPattern"))
                        End If

                        Dim oPortalConfig As roPortalGeneralConfiguration = roJSONHelper.DeserializeNewtonSoft(roCacheManager.GetInstance.GetAdvParametersCache(cName, "VTPortal.GeneralConfiguration"), GetType(roPortalGeneralConfiguration))
                        If oPortalConfig IsNot Nothing Then
                            lrret.IsImpersonateEnabled = roTypes.Any2Boolean(oPortalConfig.Impersonate)
                        Else
                            lrret.IsImpersonateEnabled = True 'Por defecto la dejamos a True, como funcionaba anteriormente
                        End If

                        lrret.IsLatamMex = roTypes.Any2Boolean(roCacheManager.GetInstance.GetAdvParametersCache(cName, "Latam.Mex"))
                        lrret.IsAD = roTypes.Any2String(roCacheManager.GetInstance.GetAdvParametersCache(cName, "VTLive.AD.DefaultDomain"))
                        lrret.ServerTimezone = Afk.ZoneInfo.TzTimeZone.CurrentTzTimeZone.Name

                        Dim idEmployee As Integer = -1
                        If oLoggedInPassport.IDEmployee.HasValue Then idEmployee = oLoggedInPassport.IDEmployee

                        lrret.EmployeeId = idEmployee
                        lrret.Language = oLoggedInPassport.Language.Key
                        If token <> String.Empty Then
                            lrret.Token = token
                        Else
                            lrret.Token = If(oRequest.Headers("roToken") IsNot Nothing AndAlso oRequest.Headers("roToken").ToString() <> "null", oRequest.Headers("roToken"), "")
                        End If

                        lrret.SecurityLevel = roTypes.Any2Integer(New roSecurityOptions(oLoggedInPassport.ID, New roSecurityOptionsState()).PasswordSecurityLevel)
                        lrret.UserId = oLoggedInPassport.ID
                        lrret.UserName = oPassportCredential
                        If Not ssoLogin Then

                            Dim appliesMFARestrictions As Boolean = True
                            If (oAuthMetod = AuthenticationMethod.Password OrElse oAuthMetod = AuthenticationMethod.IntegratedSecurity) AndAlso lrret.UserName.Contains("\") Then appliesMFARestrictions = False

                            AuthHelper.ApplyPassportActiveRestrictions(oLoggedInPassport, oState, oAuthMetod, lrret.UserName, bIsModeapp, "", "", "", False, False, appliesMFARestrictions)

                            If oState.Result = SecurityResultEnum.NoError Then
                                Dim isValid As Boolean = AuthHelper.GetPassportKeyValidated(oLoggedInPassport.ID, token)
                                If Not isValid Then lrret.Status = ErrorCodes.LOGIN_PASSWORD_EXPIRED
                            Else
                                lrret.Status = ErrorCodes.BAD_CREDENTIALS
                                Select Case oState.Result
                                    Case SecurityResultEnum.BloquedAccessApp
                                        lrret.Status = ErrorCodes.LOGIN_BLOCKED_ACCESS_APP
                                    Case SecurityResultEnum.TemporayBloqued
                                        lrret.Status = ErrorCodes.LOGIN_TEMPORANY_BLOQUED
                                    Case SecurityResultEnum.GeneralBlockAccess
                                        lrret.Status = ErrorCodes.LOGIN_GENERAL_BLOCK_ACCESS
                                    Case SecurityResultEnum.InvalidClientLocation
                                        lrret.Status = ErrorCodes.LOGIN_INVALID_CLIENT_LOCATION
                                    Case SecurityResultEnum.InvalidVersionAPP
                                        lrret.Status = ErrorCodes.LOGIN_INVALID_VERSION_APP
                                    Case SecurityResultEnum.InvalidApp
                                        lrret.Status = ErrorCodes.LOGIN_INVALID_APP
                                    Case SecurityResultEnum.ServerStopped
                                        lrret.Status = ErrorCodes.SERVER_NOT_RUNNING
                                    Case SecurityResultEnum.PassportDoesNotExists
                                        lrret.Status = ErrorCodes.LOGIN_GENERAL_BLOCK_ACCESS
                                    Case SecurityResultEnum.IsExpired
                                        lrret.Status = ErrorCodes.LOGIN_PASSWORD_EXPIRED
                                    Case SecurityResultEnum.NeedTemporaryKeyRequest
                                        lrret.Status = ErrorCodes.LOGIN_NEED_TEMPORANY_KEY
                                    Case SecurityResultEnum.NeedTemporaryKeyRequestExpired
                                        lrret.Status = ErrorCodes.LOGIN_TEMPORANY_KEY_EXPIRED
                                End Select
                            End If

                        End If
                    Else
                        lrret.Status = ErrorCodes.BAD_CREDENTIALS
                    End If
                End If
            Catch ex As Exception

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::SecurityHelper::GetLoggedInUserInfo")
            End Try

            Return lrret
        End Function

        Public Shared Function Login(ByVal usr As String, ByVal pwd As String, ByVal language As String, ByVal serverVersion As String, ByVal appVersion As String, ByVal validationCode As String, ByVal timeZone As String, ByVal strAuthToken As String, ByVal accessFromApp As Boolean, ByVal appSource As roAppSource, ByRef ostate As roSecurityState, ByVal ssoUserName As String, ByVal manualAction As Boolean) As LoginResult
            Dim lrret As New LoginResult

            Try
                Dim ssoLogin As Boolean = False
                Dim Pass As roPassportTicket = Nothing
                Dim strSecurityToken As String = ""

                lrret.ApiVersion = roTypes.Any2Integer(roCacheManager.GetInstance().GetAdvParametersCache(RoAzureSupport.GetCompanyName(), "VTPortalApiVersion"))
                Dim intSessionTimeout As Integer = roTypes.Any2Integer(roConstants.GetGlobalEnvironmentParameter(GlobalAsaxParameter.ServerTimeout))
                If intSessionTimeout <= 0 Then intSessionTimeout = 30

                Dim checkPass As String = pwd
                Pass = roPassportManager.ValidateCredentials(AuthenticationMethod.Password, usr, checkPass, True, "", ssoLogin, ostate)

                If Pass IsNot Nothing Then
                    roBaseState.SetSessionSmall(ostate, Pass.ID, appSource, "")
                    checkPass = pwd
                    Pass = AuthHelper.Authenticate(Pass, AuthenticationMethod.Password, usr, checkPass, True, ostate, accessFromApp, appVersion, serverVersion, "", ssoLogin, strAuthToken, strSecurityToken, True)

                    If Pass IsNot Nothing Then
                        Dim bolExpired As Boolean = ostate.Result = SecurityResultEnum.IsExpired
                        Dim needTemporanyKey As Boolean = ostate.Result = SecurityResultEnum.NeedTemporaryKeyRequest
                        Dim TemporanyKeyExpired As Boolean = ostate.Result = SecurityResultEnum.NeedTemporaryKeyRequestExpired
                        Dim clientLocation As String = ostate.ClientAddress.Split("#")(0)

                        roPassportManager.SetTimeZoneData(roAppType.VTPortal, Pass.ID, timeZone, New roSecurityState)
                        lrret.Status = ErrorCodes.OK
                        Dim continueLoading As Boolean = True
                        If needTemporanyKey OrElse TemporanyKeyExpired Then
                            continueLoading = False

                            If TemporanyKeyExpired Then
                                lrret.Status = ErrorCodes.LOGIN_TEMPORANY_KEY_EXPIRED
                            Else
                                If validationCode <> "" Then
                                    If AuthHelper.IsValidCode(Pass.ID, validationCode) AndAlso AuthHelper.ResetValidationCode(Pass.ID, clientLocation) Then
                                        ostate = New roSecurityState()
                                        roBaseState.SetSessionSmall(ostate, -1, appSource, "")
                                        checkPass = pwd
                                        Pass = AuthHelper.Authenticate(Pass, AuthenticationMethod.Password, usr, checkPass, True, ostate, accessFromApp, appVersion, serverVersion, "", ssoLogin, strAuthToken, strSecurityToken, False)
                                        continueLoading = True
                                    Else
                                        continueLoading = False
                                        lrret.Status = ErrorCodes.LOGIN_INVALID_VALIDATION_CODE
                                    End If
                                Else
                                    lrret.Status = ErrorCodes.LOGIN_NEED_TEMPORANY_KEY
                                End If

                            End If
                        End If

                        Dim oTerminalState As New Terminal.roTerminalState()
                        roBusinessState.CopyTo(ostate, oTerminalState)
                        Dim idEmployee As Integer = -1
                        If Pass.IDEmployee.HasValue Then idEmployee = Pass.IDEmployee


                        Dim oTerminals As Generic.List(Of Terminal.roTerminal) = Terminal.roTerminal.GetEmployeeTerminals(idEmployee, "LIVEPORTAL", oTerminalState)
                        If (oTerminals IsNot Nothing AndAlso oTerminals.Count > 0) OrElse (appSource = roAppSource.Visits) Then
                            lrret = CommitLogin(lrret, Pass, strSecurityToken, language, accessFromApp, appSource, ostate, manualAction)

                            If continueLoading AndAlso Not Pass.IsSSO Then
                                ' Si la cuenta esta caducada , forzamos a cambiar la contraseña
                                If bolExpired Then
                                    lrret.Status = ErrorCodes.LOGIN_PASSWORD_EXPIRED
                                Else
                                    Dim securityState As New roSecurityState()
                                    Dim resultCode As PasswordLevelError = roSecurityOptions.IsValidPwd(pwd, Pass, Pass.ID, securityState)
                                    AuthHelper.SetPassportKeyValidated(Pass.ID, resultCode = PasswordLevelError.No_Error, strSecurityToken, False)

                                    If resultCode <> PasswordLevelError.No_Error Then
                                        Select Case (resultCode)
                                            Case PasswordLevelError.Low_Error
                                                lrret.Status = ErrorCodes.LOGIN_RESULT_LOW_STRENGHT_ERROR
                                            Case PasswordLevelError.Medium_Error
                                                lrret.Status = ErrorCodes.LOGIN_RESULT_MEDIUM_STRENGHT_ERROR
                                            Case PasswordLevelError.High_Error
                                                lrret.Status = ErrorCodes.LOGIN_RESULT_HIGH_STRENGHT_ERROR
                                        End Select
                                    End If
                                End If
                            End If
                        Else
                            lrret.Status = ErrorCodes.NO_LIVE_PORTAL
                        End If
                    Else
                        lrret.Status = ErrorCodes.BAD_CREDENTIALS
                        Select Case ostate.Result
                            Case SecurityResultEnum.BloquedAccessApp
                                lrret.Status = ErrorCodes.LOGIN_BLOCKED_ACCESS_APP
                            Case SecurityResultEnum.TemporayBloqued
                                lrret.Status = ErrorCodes.LOGIN_TEMPORANY_BLOQUED
                            Case SecurityResultEnum.GeneralBlockAccess
                                lrret.Status = ErrorCodes.LOGIN_GENERAL_BLOCK_ACCESS
                            Case SecurityResultEnum.InvalidClientLocation
                                lrret.Status = ErrorCodes.LOGIN_INVALID_CLIENT_LOCATION
                            Case SecurityResultEnum.InvalidVersionAPP
                                lrret.Status = ErrorCodes.LOGIN_INVALID_VERSION_APP
                            Case SecurityResultEnum.InvalidApp
                                lrret.Status = ErrorCodes.LOGIN_INVALID_APP
                            Case SecurityResultEnum.ServerStopped
                                lrret.Status = ErrorCodes.SERVER_NOT_RUNNING
                            Case SecurityResultEnum.PassportDoesNotExists
                                lrret.Status = ErrorCodes.LOGIN_GENERAL_BLOCK_ACCESS

                        End Select
                    End If
                Else
                    lrret.Status = ErrorCodes.BAD_CREDENTIALS
                    Select Case ostate.Result
                        Case SecurityResultEnum.BloquedAccessApp
                            lrret.Status = ErrorCodes.LOGIN_BLOCKED_ACCESS_APP
                        Case SecurityResultEnum.TemporayBloqued
                            lrret.Status = ErrorCodes.LOGIN_TEMPORANY_BLOQUED
                        Case SecurityResultEnum.GeneralBlockAccess
                            lrret.Status = ErrorCodes.LOGIN_GENERAL_BLOCK_ACCESS
                        Case SecurityResultEnum.InvalidClientLocation
                            lrret.Status = ErrorCodes.LOGIN_INVALID_CLIENT_LOCATION
                        Case SecurityResultEnum.InvalidVersionAPP
                            lrret.Status = ErrorCodes.LOGIN_INVALID_VERSION_APP
                        Case SecurityResultEnum.InvalidApp
                            lrret.Status = ErrorCodes.LOGIN_INVALID_APP
                        Case SecurityResultEnum.ServerStopped
                            lrret.Status = ErrorCodes.SERVER_NOT_RUNNING
                        Case SecurityResultEnum.PassportAuthenticationIncorrect
                            lrret.Status = ErrorCodes.BAD_CREDENTIALS

                        Case SecurityResultEnum.PassportInactive
                            lrret.Status = ErrorCodes.LOGIN_GENERAL_BLOCK_ACCESS
                    End Select

                    Dim tbParameters As System.Data.DataTable = ostate.CreateAuditParameters()
                    ostate.AddAuditParameter(tbParameters, "{ErrorText}", ostate.Result.ToString, "", 1)
                    ostate.Audit(Audit.Action.aConnectFail, Audit.ObjectType.tConnection, usr, tbParameters, -1)
                End If
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::SecurityHelper::Login")
            End Try

            Return lrret
        End Function


        Public Shared Function validateCode(ByVal Pass As roPassportTicket, ByVal validationCode As String, ByRef ostate As roSecurityState) As StdResponse
            Dim lrret As New StdResponse

            Try
                lrret.Result = False
                If validationCode <> "" Then
                    Dim clientLocation As String = ostate.ClientAddress.Split("#")(0)
                    If AuthHelper.IsValidCode(Pass.ID, validationCode) AndAlso AuthHelper.ResetValidationCode(Pass.ID, clientLocation) Then
                        lrret.Status = ErrorCodes.OK
                        lrret.Result = True
                    Else
                        lrret.Status = ErrorCodes.LOGIN_INVALID_VALIDATION_CODE
                    End If
                Else
                    lrret.Status = ErrorCodes.LOGIN_NEED_TEMPORANY_KEY
                End If
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::SecurityHelper::Login")
            End Try

            Return lrret
        End Function


        Public Shared Function doTimeGateLogin(ByVal idEmployee As Integer, ByVal authMethod As AuthenticationMethod, ByVal strAuthToken As String, ByVal serverVersion As String,
                                               ByVal accessFromApp As Boolean, ByVal appSource As roAppSource, ByRef ostate As roSecurityState) As LoginResult
            Dim lrret As New LoginResult

            Try
                Dim Pass As roPassportTicket = Nothing
                Dim strSecurityToken As String = ""

                lrret.ApiVersion = roTypes.Any2Integer(roCacheManager.GetInstance().GetAdvParametersCache(RoAzureSupport.GetCompanyName(), "VTPortalApiVersion"))
                Dim passportManager As New roPassportManager()
                Pass = passportManager.LoadPassportTicket(idEmployee, LoadType.Employee)

                If Pass Is Nothing Then
                    lrret.Status = ErrorCodes.BAD_CREDENTIALS
                    Return lrret
                End If

                'Confirmamos que tiene permisos para entrar en el portal
                If (Not accessFromApp AndAlso Pass.EnabledVTPortal) OrElse (accessFromApp AndAlso Pass.EnabledVTPortalApp) Then
                    roBaseState.SetSessionSmall(ostate, Pass.ID, appSource, "")
                    Pass = AuthHelper.AuthenticateTimeGate(Pass, strAuthToken, strSecurityToken, False, authMethod, ostate)

                    lrret = CommitLogin(lrret, Pass, strSecurityToken, Pass.Language.Key, accessFromApp, appSource, ostate, False)
                Else
                    lrret.Status = ErrorCodes.GENERAL_ERROR_NoPermissions
                    Return lrret
                End If
            Catch ex As Exception
                lrret.Status = ErrorCodes.GENERAL_ERROR
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::SecurityHelper::Login")
            End Try

            Return lrret
        End Function


        Private Shared Function CommitLogin(ByRef lrret As LoginResult, ByRef Pass As roPassportTicket, ByRef strSecurityToken As String, ByVal language As String, ByVal accessFromApp As Boolean,
                                            ByVal appSource As roAppSource, ByRef ostate As roSecurityState, ByVal manualAction As Boolean) As LoginResult
            Dim oServerLicense As roServerLicense = Nothing
            Dim bolAuthenticated As Boolean = False

            Dim cName As String = RoAzureSupport.GetCompanyName()

            Try
                lrret.DefaultVersion = roTypes.Any2String(roCacheManager.GetInstance.GetAdvParametersCache(cName, "VTPortal.DefaultVersion"))

                If Pass.IsActivePassport Then
                    lrret.SupervisorPortalEnabled = Pass.IsSupervisor
                Else
                    lrret.SupervisorPortalEnabled = False
                End If

                Dim oPortalConfig As roPortalGeneralConfiguration = roJSONHelper.DeserializeNewtonSoft(roCacheManager.GetInstance.GetAdvParametersCache(cName, "VTPortal.GeneralConfiguration"), GetType(roPortalGeneralConfiguration))
                If oPortalConfig IsNot Nothing Then
                    lrret.IsImpersonateEnabled = roTypes.Any2Boolean(oPortalConfig.Impersonate)
                Else
                    lrret.IsImpersonateEnabled = True 'Por defecto la dejamos a True, como funcionaba anteriormente
                End If

                lrret.IsLatamMex = roTypes.Any2Boolean(roCacheManager.GetInstance.GetAdvParametersCache(cName, "Latam.Mex"))

                oServerLicense = New roServerLicense()

                Dim strRequieredFeature As String = String.Empty
                Select Case appSource
                    Case roAppSource.TimeGate, roAppSource.VTPortal, roAppSource.VTPortalApp, roAppSource.VTPortalWeb
                        strRequieredFeature = "Feature\LivePortal"
                        lrret.Consent = New roPassportConsent() With {.IsValid = True, .IDPassport = Pass.ID, .Type = ConsentTypeEnum._Portal}
                    Case roAppSource.Visits
                        strRequieredFeature = "Feature\Visits"
                        lrret.Consent = New roPassportConsent() With {.IsValid = True, .IDPassport = Pass.ID, .Type = ConsentTypeEnum._Visits}
                    Case Else
                        lrret.Consent = New roPassportConsent() With {.IsValid = True, .IDPassport = Pass.ID, .Type = ConsentTypeEnum._Desktop}
                End Select

                lrret.LicenseAccepted = lrret.Consent.IsValid

                If oServerLicense.FeatureIsInstalled(strRequieredFeature) OrElse strRequieredFeature = String.Empty Then
                    lrret.AnywhereLicense = True
                    lrret.RequiereFineLocation = (lrret.AnywhereLicense AndAlso roTypes.Any2Boolean(roCacheManager.GetInstance.GetAdvParametersCache(cName, "VTPortal.RequiereFineLocation")))
                    lrret.ShowForbiddenSections = roTypes.Any2Boolean(roCacheManager.GetInstance.GetAdvParametersCache(cName, "VTPortal.ShowForbiddenSections"))
                    lrret.ShowLogoutHome = roTypes.Any2Boolean(roCacheManager.GetInstance.GetAdvParametersCache(cName, "VTPortalShowLogoutHome"))
                    lrret.IsAD = roTypes.Any2String(roCacheManager.GetInstance.GetAdvParametersCache(cName, "VTLive.AD.DefaultDomain"))
                    lrret.ShowPermissionsIcon = roTypes.Any2Boolean(roCacheManager.GetInstance.GetAdvParametersCache(cName, "VTPortal.ShowPermissionsIcon"))
                    lrret.ServerTimezone = Afk.ZoneInfo.TzTimeZone.CurrentTzTimeZone.Name
                    lrret.SecurityLevel = roTypes.Any2Integer(New roSecurityOptions(Pass.ID, New roSecurityOptionsState()).PasswordSecurityLevel)
                    lrret.ShowLegalText = Robotics.VTBase.roTypes.Any2Boolean(roCacheManager.GetInstance.GetAdvParametersCache(cName, "VisualTime.Security.ShowLegalText"))

                    Dim oLicSupport As New roServerLicense()
                    Dim dailyRecordInstalled As Boolean = oLicSupport.FeatureIsInstalled("Feature\DailyRecord")

                    If dailyRecordInstalled Then
                        lrret.DailyRecordEnabled = Security.WLHelper.GetPermissionOverFeature(Pass.ID, "Punches.DailyRecord", "E")
                        lrret.DailyRecordPatternEnabled = roTypes.Any2Boolean(roCacheManager.GetInstance.GetAdvParametersCache(cName, "VTPortal.DailyRecordPattern"))
                    Else
                        lrret.DailyRecordEnabled = False
                        lrret.DailyRecordPatternEnabled = False
                    End If

                    lrret.Token = strSecurityToken
                    lrret.UserId = Pass.ID

                    If Pass.IDEmployee.HasValue Then
                        lrret.EmployeeId = Pass.IDEmployee
                    Else
                        lrret.EmployeeId = -1
                    End If


                    bolAuthenticated = True
                Else
                    lrret.Status = ErrorCodes.NOT_LICENSED
                End If


                If bolAuthenticated Then
                    lrret.DefaultLanguage = Pass.Language.Key

                    If language IsNot Nothing AndAlso language <> String.Empty Then
                        Dim oLanguages As roPassportLanguage() = New roLanguageManager().LoadLanguages()

                        For Each oLanguage As roPassportLanguage In oLanguages
                            If oLanguage.Key = language AndAlso oLanguage.ID <> Pass.Language.ID Then
                                Pass.Language = oLanguage
                                lrret.DefaultLanguage = oLanguage.Key

                                Dim oManager As New roPassportManager
                                oManager.UpdatePassportNameAndLanguage(Pass.ID, Pass.Name, oLanguage.ID)
                            End If
                        Next
                    End If
                End If

            Catch ex As Exception

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::SecurityHelper::CommitLogin")
            End Try

            If lrret.Status = ErrorCodes.OK AndAlso manualAction Then VTPortal.SecurityHelper.UpdateLastLogin(lrret.UserId, ostate)

            If oServerLicense Is Nothing Then oServerLicense = New roServerLicense

            If oServerLicense IsNot Nothing AndAlso oServerLicense.FeatureIsInstalled("Feature\CegidHub") Then
                lrret.IsCegidHub = True
            End If
            lrret.IsSaas = True

            If lrret.Status = ErrorCodes.OK OrElse
                lrret.Status = ErrorCodes.LOGIN_PASSWORD_EXPIRED OrElse
                lrret.Status = ErrorCodes.LOGIN_RESULT_LOW_STRENGHT_ERROR OrElse
                lrret.Status = ErrorCodes.LOGIN_RESULT_MEDIUM_STRENGHT_ERROR OrElse
                lrret.Status = ErrorCodes.LOGIN_RESULT_HIGH_STRENGHT_ERROR Then

                Dim oCommunicateManager As New roCommuniqueManager

                If (oServerLicense IsNot Nothing AndAlso oServerLicense.FeatureIsInstalled("Feature\AdvancedCommuniques")) OrElse oCommunicateManager.GetAllCommuniques(lrret.EmployeeId).Count > 0 Then
                    lrret.IsCommuniquesEnabled = True 'Activamos comunicados si tiene módulo activo o si tiene comunicados en BD (pbi: 1190266)
                End If

                Dim oChannelManager As New roChannelManager
                If (oServerLicense.FeatureIsInstalled("Feature\Channels") OrElse oServerLicense.FeatureIsInstalled("Feature\Complaints")) OrElse oChannelManager.GetAllChannels(lrret.EmployeeId).Count > 0 Then
                    lrret.IsChannelsEnabled = True 'Activamos canales si tiene módulo activo o si tiene canales en BD
                End If

                Dim oValue As String = roCacheManager.GetInstance.GetAdvParametersCache(cName, "VTPortal.HeaderConfiguration")
                If oValue <> String.Empty Then
                    lrret.HeaderMD5 = CryptographyHelper.EncryptWithMD5(oValue)
                Else
                    lrret.HeaderMD5 = ""
                End If

                If lrret.UserId > 0 Then
                    lrret.LastLogin = VTPortal.SecurityHelper.GetLastLogin(lrret.UserId, ostate)
                Else
                    lrret.LastLogin = roTypes.CreateDateTime(2079, 1, 1)
                End If
            Else
                lrret.LastLogin = roTypes.CreateDateTime(2079, 1, 1)
                lrret.HeaderMD5 = ""
                lrret.IsChannelsEnabled = False
                lrret.IsCommuniquesEnabled = False
            End If

            Return lrret
        End Function

        Public Shared Function UpdateServerLanguage(ByVal idPassport As Integer, ByVal sNewLang As String, ByVal ostate As roSecurityState) As StdResponse
            Dim lrret As New StdResponse
            Dim bupdate As Boolean = False
            Try
                Dim Pass As roPassportTicket = roPassportManager.GetPassportTicket(idPassport, LoadType.Passport)

                If sNewLang IsNot Nothing AndAlso sNewLang <> String.Empty Then
                    Dim oLanguages As roPassportLanguage() = New roLanguageManager().LoadLanguages()

                    For Each oLanguage As roPassportLanguage In oLanguages
                        If oLanguage.Key = sNewLang AndAlso oLanguage.ID <> Pass.Language.ID Then
                            Pass.Language = oLanguage
                            bupdate = New roPassportManager().UpdatePassportNameAndLanguage(Pass.ID, Pass.Name, oLanguage.ID)
                        End If
                    Next
                End If

                lrret.Status = If(bupdate, ErrorCodes.OK, ErrorCodes.GENERAL_ERROR)
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::SecurityHelper::UpdateServerLanguage")
                lrret.Status = ErrorCodes.GENERAL_ERROR
            End Try

            Return lrret
        End Function


        Public Shared Function LogoutUserSession(idUser As Integer, appSource As roAppSource, uuid As String) As StdResponse
            Dim lrret As New StdResponse()

            Dim oState As New roSecurityState()
            roBaseState.SetSessionSmall(oState, idUser, appSource, "")
            lrret = VTPortal.SecurityHelper.Logout(idUser, oState)
            If lrret.Result AndAlso uuid IsNot Nothing Then
                VTPortal.SecurityHelper.DeleteFirebaseToken(idUser, uuid, oState)
            End If

            Return lrret
        End Function

        Public Shared Function LogoutTimegateSession(timegateSerialNumber As String, appSource As roAppSource) As StdResponse
            Dim lrret As New StdResponse()

            Dim sSQL As String = $"@SELECT# IDPassport from sysroPassports_Data WHERE AuthToken='{timegateSerialNumber}'"

            Dim tbPassports As DataTable = AccessHelper.CreateDataTable(sSQL)

            If tbPassports IsNot Nothing AndAlso tbPassports.Rows.Count > 0 Then
                For Each oRow In tbPassports.Rows
                    Dim idUser As Integer = oRow("IDPassport")

                    Dim oState As New roSecurityState()
                    roBaseState.SetSessionSmall(oState, idUser, appSource, "")
                    lrret = VTPortal.SecurityHelper.Logout(idUser, oState)
                    If lrret.Result AndAlso timegateSerialNumber IsNot Nothing Then
                        VTPortal.SecurityHelper.DeleteFirebaseToken(idUser, timegateSerialNumber, oState)
                    End If
                Next
            End If

            Return lrret
        End Function

        Public Shared Function Logout(ByVal idPassport As Integer, ByVal ostate As roSecurityState) As StdResponse
            Dim lrret As New StdResponse
            Try
                'WLHelper.SetSecurityTokens(idPassport, False, "", "", ostate)
                SessionHelper.SessionRemove(ostate.SessionID, ostate)
                lrret.Result = True
                lrret.Status = ErrorCodes.OK
            Catch ex As Exception
                lrret.Result = False
                lrret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::SecurityHelper::Logout")
            End Try

            Return lrret
        End Function

        Public Shared Function AcceptLicense(ByVal oPassportTicket As roPassportTicket, ByVal bAcceptValue As Boolean, ByVal ostate As roSecurityState) As StdResponse
            Dim lrret As New StdResponse

            Try
                lrret.Result = roPassportManager.SetLicenseAgreementValidation(oPassportTicket.ID, bAcceptValue, New roSecurityState(oPassportTicket.ID, Nothing))

                If lrret.Result Then
                    lrret.Status = ErrorCodes.OK
                Else
                    lrret.Status = ErrorCodes.GENERAL_ERROR
                End If
            Catch ex As Exception
                lrret.Result = False
                lrret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::SecurityHelper::AcceptLicense")
            End Try

            Return lrret
        End Function

    End Class

End Namespace