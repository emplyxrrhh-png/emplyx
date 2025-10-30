Imports System.Data.Common
Imports System.DirectoryServices
Imports System.Net
Imports System.Web
Imports System.Xml.Serialization
Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.roConstants

Namespace Base


    ''' <summary>
    ''' Serves as the main entry point to access WebLogin objects.
    ''' </summary>
    Public NotInheritable Class AuthHelper
        ''' <summary>
        ''' Authenticates a user by validating it's credential and password for a specified login method.
        ''' </summary>
        ''' <param name="method">The authentication method to use.</param>
        ''' <param name="credential">Credentials: username, windows account, biometrical data, etc.</param>
        ''' <param name="password">Hashed password to validate credential.</param>
        ''' <returns>A passport if authentication succeeds, otherwise, nothing.</returns>
        Public Shared Function Authenticate(ByVal method As AuthenticationMethod, ByVal credential As String, ByVal password As String) As roPassportTicket
            Return Authenticate(method, credential, password, False, "")
        End Function


        Public Shared Function Authenticate(ByVal authenticatedPassport As roPassportTicket, ByVal method As AuthenticationMethod, ByVal credential As String, ByRef password As String, ByVal hashPassword As Boolean, ByRef oState As roSecurityState, ByVal bisAccessingFromApp As Boolean,
                                     ByVal strVersionAPP As String, ByVal strVersionServer As String, ByVal strMail As String, ByVal isSSOLogin As Boolean, ByVal strAuthToken As String, ByRef strSecurityToken As String, ByVal bolAudit As Boolean) As roPassportTicket

            Dim oPassport As roPassportTicket = Nothing
            Dim strNamePassport As String = ""

            Try


                ' Miramos si debo autenticar contra dominio
                Dim defaultDomain As String = String.Empty
                If method = AuthenticationMethod.Password OrElse method = AuthenticationMethod.IntegratedSecurity Then
                    If credential.StartsWith(".\") Then
                        ' Si se informó ".\", entro sin dominio aunque haya por defecto
                        credential = credential.Replace(".\", "")
                    ElseIf Not credential.Contains("\") Then
                        ' Si no se informó dominio, y hay uno por defecto, aplico el por defecto
                        defaultDomain = DataLayer.roCacheManager.GetInstance().GetAdvParametersCache(Azure.RoAzureSupport.GetCompanyName(), "VTLive.AD.DefaultDomain")
                        If defaultDomain.Length > 0 Then
                            credential = defaultDomain & "\" & credential
                        End If
                    End If
                End If

                Dim adAuthentaication As Boolean = False
                Dim bRet As Boolean = False
                Dim appliesMFARestrictions As Boolean = True

                If (method = AuthenticationMethod.Password OrElse method = AuthenticationMethod.IntegratedSecurity) AndAlso credential.Contains("\") Then
                    appliesMFARestrictions = False
                    Dim strDomain As String = credential.Substring(0, credential.IndexOf("\")).Trim()
                    Dim strUser As String = credential.Substring(credential.IndexOf("\")).Trim()
                    If strUser.Length > 0 Then strUser = strUser.Substring(1)

                    If Not isSSOLogin Then
                        Dim oAuthenticateAD As New AuthenticateAD()
                        bRet = oAuthenticateAD.AuthenticateByActiveDirectory(oState, strDomain, strUser, password)
                    Else
                        bRet = True
                    End If

                    If bRet Then
                        'Si la autenticacion real se ha heho bajo AD, en VTLive se hace con el pass igual al nombre del usuario porque el pass de la BBDD es irrelevante
                        'solo se usa a efectos de configuracion de Web services
                        password = credential.ToLower()
                        hashPassword = True
                        adAuthentaication = True
                    End If
                Else
                    bRet = True
                End If

                If bRet Then
                    If authenticatedPassport IsNot Nothing Then
                        oPassport = authenticatedPassport
                    Else
                        oPassport = AuthHelper.Authenticate(method, credential, password, hashPassword, "")
                    End If

                    If oPassport IsNot Nothing Then
                        strNamePassport = oPassport.Name
                        '***************************************************************
                        ApplyPassportActiveRestrictions(oPassport, oState, method, credential, bisAccessingFromApp, strVersionAPP, strVersionServer, strMail, bolAudit, adAuthentaication, appliesMFARestrictions)


                        If oPassport IsNot Nothing Then
                            oState.AuthenticateAttempts = 0
                            ' Verificamos licencia coneciones concurrentes
                            If SessionHelper.SessionCheck(oState.SessionID, oPassport, True, method, oState) Then
                                oState.IDPassport = oPassport.ID
                                ' Verificar si és un passport activo
                                If bolAudit Then
                                    ' Auditamos conexión
                                    oState.Audit(Audit.Action.aConnect, Audit.ObjectType.tConnection, oPassport.Name, Nothing, -1)

                                    ' Guardamos historial de conexion del usuario
                                    SessionHelper.UpdateConnectionHistory(oState, oPassport.Name, Audit.Action.aConnect)
                                End If
                            Else
                                oPassport = Nothing
                            End If
                        End If
                    Else
                        oState.Result = SecurityResultEnum.PassportAuthenticationIncorrect
                        If Not adAuthentaication Then
                            ' Comprobamos si existe el usuario con la cuenta ya bloqueda
                            ' para indicar el mensaje de cuenta bloqueada aunque haya puesto de forma erronea la contraseña
                            Dim oPassporttmp As roPassport = Nothing
                            oPassporttmp = roPassportManager.GetPassportByCredential(credential, method, "")
                            If oPassporttmp IsNot Nothing Then
                                ' Miramos si la cuenta ya esta bloqueada temporal o indefinidamente
                                If oPassporttmp.IsBloquedAccessApp Then
                                    oState.Result = SecurityResultEnum.BloquedAccessApp
                                    oPassport = Nothing
                                ElseIf oPassporttmp.IsTemporayBloqued Then
                                    oState.Result = SecurityResultEnum.TemporayBloqued
                                    oPassport = Nothing
                                End If
                            End If
                        End If
                    End If
                Else
                    oState.Result = SecurityResultEnum.PassportAuthenticationIncorrect
                End If

                If oState.Result <> SecurityResultEnum.NoError AndAlso bolAudit Then
                    If strNamePassport = String.Empty Then
                        strNamePassport = credential
                    End If
                    Dim tbParameters As System.Data.DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{ErrorText}", oState.Result.ToString, "", 1)
                    oState.Audit(Audit.Action.aConnectFail, Audit.ObjectType.tConnection, strNamePassport, tbParameters, -1)
                End If

                If oState.Result = SecurityResultEnum.NoError OrElse oState.Result = SecurityResultEnum.IsExpired OrElse
                oState.Result = SecurityResultEnum.NeedTemporaryKeyRequest OrElse oState.Result = SecurityResultEnum.NeedTemporaryKeyRequestExpired OrElse
                oState.Result = SecurityResultEnum.NeedTemporaryKeyRequestRobotics OrElse oState.Result = SecurityResultEnum.NeedTemporaryKeyRequestExpiredRobotics OrElse oState.Result = SecurityResultEnum.NeedMailRequest Then
                    strSecurityToken = CryptographyHelper.EncryptWithSHA256(strAuthToken & "@" & oPassport.ID & "@" & DateTime.Now.ToString("yyyyMMddHHmmss"))
                    Dim bIsSupervisor As Boolean = False
                    If oState.AppType = roAppType.VTLive OrElse (oState.AppType = roAppType.VTPortal AndAlso oPassport.IsSupervisor) Then bIsSupervisor = True

                    AuthHelper.SetSecurityTokens(oPassport.ID, bIsSupervisor, strAuthToken, strSecurityToken, method, oState)
                End If
            Catch ex As System.Data.Common.DbException
                oState.UpdateStateInfo(ex, "wscSecurity::Authenticate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "wscSecurity::Authenticate")
            End Try

            Return oPassport
        End Function

        ''' <summary>
        ''' Authenticates a user by validating it's credential and password for a specified login method.
        ''' </summary>
        ''' <param name="method">The authentication method to use.</param>
        ''' <param name="credential">Credentials: username, windows account, biometrical data, etc.</param>
        ''' <param name="password">Hashed password to validate credential.</param>
        ''' <param name="hashPassword">Wether to hash password.</param>
        ''' <param name="version">Version of authentication method (default is '').</param>
        ''' <returns>A passport if authentication succeeds, otherwise, nothing.</returns>
        Public Shared Function Authenticate(ByVal method As AuthenticationMethod, ByVal credential As String, ByVal password As String, ByVal hashPassword As Boolean, ByVal version As String) As roPassportTicket
            Dim Result As roPassportTicket = Nothing

            Dim Pass As String = password

            If hashPassword Then
                Pass = CryptographyHelper.EncryptWithMD5(password)

                ' En el caso que no venga encriptada
                ' debemos revisar si el password del usuario esta ya transformado a MD5
                ' en caso contrario , convertirlo ahora
                If method = AuthenticationMethod.Password Then
                    'Dim SQLStr As String = "@SELECT# isnull(IDPassport, 0) as IDPassport, isnull(Password, '') as Password FROM sysroPassports_AuthenticationMethods WHERE Credential ='" & credential & "' AND Method=1"
                    Dim SQLStr As String = "@SELECT# isnull(IDPassport, 0) as IDPassport, isnull(Password, '') as Password FROM sysroPassports_AuthenticationMethods WHERE Credential =@credential AND Method=1"

                    Dim oParams As New List(Of CommandParameter)
                    oParams.Add(New CommandParameter("@credential", DbType.String, credential))

                    Dim tbs As DataTable = DataLayer.AccessHelper.CreateDataTable(SQLStr, oParams, "Methods")
                    If tbs IsNot Nothing AndAlso tbs.Rows.Count = 1 Then
                        Dim bUpdate As Boolean = False
                        Dim strPwd As String = tbs.Rows(0).Item("Password").ToString
                        If credential.Contains("\") AndAlso strPwd <> Pass Then
                            strPwd = Pass
                            bUpdate = True
                        ElseIf strPwd.Length <> 32 Then
                            bUpdate = True
                            strPwd = CryptographyHelper.EncryptWithMD5(CryptographyHelper.Decrypt(strPwd))
                        End If

                        If bUpdate Then
                            Dim intIDPassport As Integer = roTypes.Any2Integer(tbs.Rows(0).Item("IDPassport"))
                            DataLayer.AccessHelper.ExecuteSql("@UPDATE# sysroPassports_AuthenticationMethods SET Password='" & strPwd & "' WHERE IDPassport= " & intIDPassport.ToString & " AND Method=1 AND Credential ='" & credential & "'")
                        End If

                    End If
                End If
            End If

            ' Check credentials.
            Dim IDPassport As Nullable(Of Integer) = AuthHelper.Authenticate(method, credential, Pass, version)
            If IDPassport.HasValue Then
                Result = roPassportManager.GetPassportTicket(IDPassport.Value, LoadType.Passport)
            End If
            Return Result
        End Function

        ''' <summary>
        ''' Autentifica un usuario validando la información biométrica
        ''' </summary>
        ''' <param name="version">Versión del fw de la información biométrica.</param>
        ''' <param name="BiometricData">Información biomñetrica a comparar.</param>
        ''' <param name="BiometricID">Si se informa -1, se comparará con todos los ids biometricos, sinó con el especificado.</param>
        ''' <returns>El passport si la validación es correcta, o nothing si es erronea.</returns>
        ''' <remarks></remarks>
        Public Shared Function Authenticate(ByVal version As String, ByVal BiometricData As Byte(), Optional ByVal BiometricID As Integer = -1) As roPassportTicket

            Dim Result As roPassportTicket = Nothing

            ' Check biometric credentials
            Dim IDPassport As Nullable(Of Integer) = AuthHelper.AuthenticateBiometric(version, BiometricData, BiometricID)
            If IDPassport.HasValue Then
                Result = roPassportManager.GetPassportTicket(IDPassport.Value)
                If Not (Result.IDEmployee.HasValue) Then
                    Result = Nothing
                End If
            End If
            Return Result

        End Function

        Public Shared Function AuthenticateTimeGate(ByVal oPassport As roPassportTicket, ByVal strAuthToken As String, ByRef strSecurityToken As String, ByVal bolAudit As Boolean, ByVal authMethod As AuthenticationMethod, ByRef oState As roSecurityState) As roPassportTicket

            Try
                Dim passportName As String = oPassport.Name
                ' Verificamos licencia coneciones concurrentes
                If SessionHelper.SessionCheck(oState.SessionID, oPassport, True, authMethod, oState) Then
                    oState.IDPassport = oPassport.ID
                    ' Verificar si és un passport activo
                    If bolAudit Then
                        ' Auditamos conexión
                        oState.Audit(Audit.Action.aConnect, Audit.ObjectType.tConnection, oPassport.Name, Nothing, -1)
                    End If
                Else
                    oPassport = Nothing
                End If

                If oState.Result <> SecurityResultEnum.NoError AndAlso bolAudit Then

                    Dim tbParameters As System.Data.DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{ErrorText}", oState.Result.ToString, "", 1)
                    oState.Audit(Audit.Action.aConnectFail, Audit.ObjectType.tConnection, passportName, tbParameters, -1)
                End If

                If oPassport IsNot Nothing AndAlso oState.Result = SecurityResultEnum.NoError Then
                    strSecurityToken = CryptographyHelper.EncryptWithSHA256(strAuthToken & "@" & oPassport.ID & "@" & DateTime.Now.ToString("yyyyMMddHHmmss"))
                    Dim bIsSupervisor As Boolean = False
                    If oState.AppType = roAppType.VTLive OrElse (oState.AppType = roAppType.VTPortal AndAlso oPassport.IsSupervisor) Then bIsSupervisor = True

                    AuthHelper.SetSecurityTokens(oPassport.ID, bIsSupervisor, strAuthToken, strSecurityToken, authMethod, oState)
                End If
            Catch ex As System.Data.Common.DbException
                oState.UpdateStateInfo(ex, "wscSecurity::Authenticate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "wscSecurity::Authenticate")
            End Try

            Return oPassport
        End Function


        Public Shared Sub ApplyPassportActiveRestrictions(ByRef oPassport As roPassportTicket, ByRef oState As roSecurityState, method As AuthenticationMethod, credential As String, bisAccessingFromApp As Boolean, strVersionAPP As String,
                                                            strVersionServer As String, strMail As String, bolAudit As Boolean, adAuthentaication As Boolean, appliesMFARestrictions As Boolean)

            Dim bolKeyRequestExpired As Boolean = False
            Dim bAccessAsEmployee As Boolean = (oState.AppType = roAppType.VTPortal)
            Dim sessionTimeout = roTypes.Any2Integer(roConstants.GetGlobalEnvironmentParameter(GlobalAsaxParameter.ServerTimeout))
            Dim accessRestrictionEnabled As Boolean = (Not roTypes.Any2Boolean(roCacheManager.GetInstance().GetAdvParametersCache(RoAzureSupport.GetCompanyName(), "VTLive.IPRestriction.OnlyPunches")))

            If Not adAuthentaication AndAlso oPassport.IsPrivateUser Then
                '  Usuarios de Robotics permitidos
                If Not AuthHelper.IsRoboticsUserAllowed() Then
                    oState.Result = SecurityResultEnum.RoboticsUserAllowed
                    oPassport = Nothing
                Else
                    ' Si la cuenta es de Robotics y entra en el VTDesktop
                    If oState.AppType = roAppType.VTLive Then
                        If strMail.Length = 0 Then
                            AuthHelper.ResetValidationCodeRobotics(oPassport.ID)
                            oState.Result = SecurityResultEnum.NeedMailRequest
                        Else
                            'Miramos que el formato de la cuenta de Robotics sea correcto
                            If Not AuthHelper.ValidateRoboticsAccount(strMail) Then
                                oState.Result = SecurityResultEnum.InvalidRoboticsAccountFormat
                                oPassport = Nothing
                            Else
                                If AuthHelper.NeedRoboticsPassportKey(oPassport.ID, bolKeyRequestExpired, strMail) Then
                                    If Not bolKeyRequestExpired Then
                                        oState.Result = SecurityResultEnum.NeedTemporaryKeyRequestRobotics
                                    Else
                                        oState.Result = SecurityResultEnum.NeedTemporaryKeyRequestExpiredRobotics
                                    End If
                                Else
                                    oState.Result = SecurityResultEnum.PassportAuthenticationIncorrect
                                    oPassport = Nothing
                                End If

                            End If
                        End If
                    Else
                        oState.Result = SecurityResultEnum.InvalidApp
                        oPassport = Nothing
                    End If
                End If
            Else
                If oPassport.IsBloquedAccessApp Then
                    oState.Result = SecurityResultEnum.BloquedAccessApp
                    oPassport = Nothing
                Else
                    ' 1.Bloqueo temporal. Comprobar si se han excedido el nº de errores y han pasado menos de 10 minutos desde el ultimo acceso erroneo
                    If oPassport.IsTemporayBloqued AndAlso Not adAuthentaication Then
                        oState.Result = SecurityResultEnum.TemporayBloqued
                        oPassport = Nothing
                    Else
                        ' 2. Contraseña correcta.Resetear numero de intentos de acceso erroneos
                        AuthHelper.SetResetInvalidAccessAttemps(method, credential, "", oPassport.ID)

                        ' 3. Validar aplicaciones permitidas
                        If Not roPassportManager.IsValidApp(oPassport, oState, bisAccessingFromApp) Then
                            oState.Result = SecurityResultEnum.InvalidApp
                            oPassport = Nothing
                        Else
                            ' 4. Validar bloqueos de acceso generales y del grupo
                            If AuthHelper.IsGeneralBlockAccess(oPassport.ID, oPassport.IsSupervisor, oPassport.IDGroupFeature, oState) Then
                                oState.Result = SecurityResultEnum.GeneralBlockAccess
                                oPassport = Nothing
                            Else
                                Dim bCheckSupervisorIsActive As Boolean = True

                                'Si accedo al portal del empleado  en seguridad > = V2 y soy empleado no compruebo el estado del supervisor
                                If bAccessAsEmployee AndAlso oPassport.IDEmployee.HasValue AndAlso oPassport.IDEmployee > 0 Then
                                    bCheckSupervisorIsActive = False
                                End If

                                'Si accedo a live en seguridad >=V2 compruebo si soy supervisor
                                If Not bAccessAsEmployee Then
                                    bCheckSupervisorIsActive = oPassport.IsSupervisor
                                End If

                                '5. Validamos si el passport esta activo
                                If bCheckSupervisorIsActive AndAlso Not oPassport.IsActivePassport Then
                                    oState.Result = SecurityResultEnum.PassportInactive
                                    oPassport = Nothing
                                Else
                                    If (accessRestrictionEnabled) Then
                                        ' 7. Validacion IP del cliente si no es APP
                                        Dim strClientLocation As String = oState.ClientAddress.Split("#")(0)
                                        If Not bisAccessingFromApp AndAlso Not AuthHelper.IsValidClientLocation(oPassport.ID, strClientLocation) Then
                                            oState.Result = SecurityResultEnum.InvalidClientLocation
                                            oPassport = Nothing
                                        Else
                                            ' 9Validamos si la APP tiene la misma version que el servidor de VT
                                            If Not AuthHelper.IsValidVersionAPP(oPassport.ID, bisAccessingFromApp, strVersionAPP, strVersionServer) Then
                                                oState.Result = SecurityResultEnum.InvalidVersionAPP
                                                oPassport = Nothing
                                            Else
                                                ' 9. Guardamos IP y usuario y Validacion si se necesita pedir solicitud de clave temporal
                                                If appliesMFARestrictions AndAlso AuthHelper.LoginKeyRequiered(oPassport.ID, strClientLocation, bolKeyRequestExpired) Then
                                                    If Not bolKeyRequestExpired Then
                                                        oState.Result = SecurityResultEnum.NeedTemporaryKeyRequest
                                                    Else
                                                        oState.Result = SecurityResultEnum.NeedTemporaryKeyRequestExpired
                                                    End If
                                                    'oPassport = Nothing
                                                Else
                                                    ' 10. Contraseña caducada
                                                    If oPassport.IsExpiredPwd AndAlso method = AuthenticationMethod.Password Then
                                                        oState.Result = SecurityResultEnum.IsExpired
                                                    Else

                                                        If oPassport.IDEmployee > 0 AndAlso oState.AppType = roAppType.VTPortal Then

                                                            Dim strSQLField = "@select# value from EmployeeUserFieldValues where fieldname='Acceso Portal' and IDEmployee= '" & oPassport.IDEmployee & "'"
                                                            Dim field = roTypes.Any2String(DataLayer.AccessHelper.ExecuteScalar(strSQLField))

                                                            If field.ToString.ToLower = "siempre" Then
                                                                'do nothing. Employee can enter
                                                            ElseIf field.ToString.ToLower = "nunca" Then
                                                                oState.Result = SecurityResultEnum.PassportDoesNotExists
                                                                oPassport = Nothing
                                                            Else
                                                                Dim strSQL As String = "@SELECT# HasFutureContract, sp.LoginWithoutContract, case when svw.CurrentEmployee is null then 0 else svw.CurrentEmployee end as CurrentEmployee FROM " &
                                                                     "(@SELECT# CASE WHEN max(enddate) > getdate() THEN 1 ELSE 0 END AS HasFutureContract FROM  " &
                                                                     "employeecontracts ec WHERE ec.IDEmployee=" & oPassport.IDEmployee & "  ) a INNER JOIN sysropassports sp ON sp.IDEmployee = " & oPassport.IDEmployee & "  " &
                                                                     " left join sysrovwCurrentEmployeeGroups svw on svw.IDEmployee = " & oPassport.IDEmployee & " "

                                                                Dim resultTable As DataTable = DataLayer.AccessHelper.CreateDataTable(strSQL)

                                                                If resultTable IsNot Nothing AndAlso resultTable.Rows.Count > 0 Then

                                                                    For Each oRow As DataRow In resultTable.Rows
                                                                        If roTypes.Any2Integer(oRow("CurrentEmployee")) = 0 Then
                                                                            If roTypes.Any2Integer(oRow("HasFutureContract")) = 1 AndAlso roTypes.Any2Integer(oRow("LoginWithoutContract")) = 1 Then
                                                                                'do nothing. Employee can enter
                                                                            Else
                                                                                oState.Result = SecurityResultEnum.PassportDoesNotExists
                                                                                oPassport = Nothing
                                                                            End If
                                                                        End If
                                                                    Next
                                                                End If
                                                            End If
                                                        End If

                                                    End If
                                                End If
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
            End If

        End Sub

        Public Shared Function SetPassportKeyValidated(idPassport As Integer, validated As Boolean, token As String, isUpdatingPwd As Boolean) As Boolean

            '
            ' Validamos si la IP con la que se esta accediendo esta permitida
            '
            Dim bolRet As Boolean = True

            Try
                'Si la contraseña no cumple con los requisitos, guardamos en todas las aplicaciones que debe ir al login
                If Not validated Then
                    Dim sSQL As String = $"@UPDATE# sysroPassports_Data set KeyValidated = 0 where IDPassport = {idPassport}"
                    AccessHelper.ExecuteSql(sSQL)
                Else

                    'Si la contraseña cumple con los requisitos, solo guardamos en todas la aplicación en cuestión que ha sido validada
                    Dim sSQL As String = $"@UPDATE# sysroPassports_Data set KeyValidated = 1 where IDPassport = {idPassport} AND SecurityToken = '{token}'"
                    AccessHelper.ExecuteSql(sSQL)

                    'Si se esta cambiando la contraseña, marcaremos el resto de aplicaciones como no válidas para forzar a introducir las nuevas credenciales
                    If isUpdatingPwd Then
                        sSQL = $"@UPDATE# sysroPassports_Data set KeyValidated = 0 where IDPassport = {idPassport} AND SecurityToken <> '{token}'"
                        AccessHelper.ExecuteSql(sSQL)
                    End If
                End If

            Catch ex As DbException
                bolRet = False
            Catch ex As Exception
                bolRet = False
            End Try

            Return bolRet
        End Function

        Public Shared Function GetPassportKeyValidated(idPassport As Integer, token As String) As Boolean

            '
            ' Validamos si la IP con la que se esta accediendo esta permitida
            '
            Dim bolRet As Boolean = True

            Try

                Dim sSQL As String = $"@SELECT# KeyValidated FROM sysroPassports_Data where IDPassport = {idPassport} AND SecurityToken = '{token}'"
                bolRet = roTypes.Any2Integer(AccessHelper.ExecuteScalar(sSQL))
            Catch ex As DbException
                bolRet = False
            Catch ex As Exception
                bolRet = False
            End Try

            Return bolRet
        End Function

        ''' <summary>
        ''' IsValidClientLocation
        ''' </summary>
        Public Shared Function IsValidClientLocation(ByVal idpassport As Integer, ByVal strClientLocation As String) As Boolean

            '
            ' Validamos si la IP con la que se esta accediendo esta permitida
            '
            Dim bolRet As Boolean = True

            Try

                If strClientLocation.Length = 0 Then
                    Return bolRet
                End If

                If strClientLocation = "127.0.0.1" OrElse strClientLocation = "::1" Then
                    Return bolRet
                End If

                ' Quitamos la información de puerto de la IP origen
                strClientLocation = strClientLocation.Split(":")(0)

                ' Parametros generales
                Dim table As New DataTable()

                Dim strSQL As String = "@SELECT# isnull(OnlyAllowedAdress, '') as OnlyAllowedAdress FROM sysroSecurityParameters " &
                                       "WHERE IDPassport = @IDPassport "
                Dim cmd As DbCommand = AccessHelper.CreateCommand(strSQL)
                AccessHelper.AddParameter(cmd, "@IDPassport", DbType.Int32).Value = 0

                Dim Adapter As DbDataAdapter = AccessHelper.CreateDataAdapter(cmd)
                Adapter.Fill(table)
                Dim Rows As DataRow() = table.Select

                EvaluateClientLocation(Rows, strClientLocation, bolRet)

                If Not bolRet Then
                    table = New DataTable()

                    ' Verificamos si alguno de sus padres tiene restricciones de IP
                    strSQL = "@SELECT# isnull(OnlyAllowedAdress, '') as OnlyAllowedAdress FROM sysroSecurityParameters " &
                             "WHERE IDPassport IN (@SELECT# ID FROM dbo.GetPassportParents(@IDPassport)) AND len(convert(nvarchar(1000),isnull(OnlyAllowedAdress, ''))) > 0  "
                    cmd = AccessHelper.CreateCommand(strSQL)
                    AccessHelper.AddParameter(cmd, "@IDPassport", DbType.Int32).Value = idpassport
                    cmd.Parameters("@IDPassport").Value = idpassport
                    Adapter = AccessHelper.CreateDataAdapter(cmd)
                    Adapter.Fill(table)
                    Rows = table.Select

                    If Rows IsNot Nothing AndAlso Rows.Length > 0 Then
                        ' Sólo reevaluamos si algno de los padres tiene restricciones de IP
                        EvaluateClientLocation(Rows, strClientLocation, bolRet)
                    End If

                End If
            Catch ex As DbException
                bolRet = True
            Catch ex As Exception
                bolRet = True
            Finally

            End Try

            Return bolRet
        End Function


        ''' <summary>
        ''' IsGeneralBlockAccess
        ''' </summary>
        Public Shared Function IsGeneralBlockAccess(ByVal idpassport As Integer, ByVal isSupervisor As Boolean, ByVal groupFeature As Integer, ByVal oState As roBaseState) As Boolean
            Dim bolRet As Boolean = False

            Try
                ' Parametros generales
                Dim table As New DataTable()

                Dim strSQL As String = "@SELECT# isnull(BlockAccessVTPortal, 0) as BlockAccessVTPortal, isnull(BlockAccessVTDesktop, 0) as BlockAccessVTDesktop, isnull(BlockAccessVTVisits, 0) as BlockAccessVTVisits FROM sysroSecurityParameters " &
                                       "WHERE IDPassport = @IDPassport "
                Dim cmd As DbCommand = AccessHelper.CreateCommand(strSQL)
                AccessHelper.AddParameter(cmd, "@IDPassport", DbType.Int32).Value = 0

                Dim Adapter As DbDataAdapter = AccessHelper.CreateDataAdapter(cmd)
                Adapter.Fill(table)
                Dim Rows As DataRow() = table.Select

                If Rows IsNot Nothing AndAlso Rows.Length > 0 Then
                    For Each oRow As DataRow In Rows
                        Select Case oState.AppType
                            Case roAppType.VTLive
                                bolRet = oRow("BlockAccessVTDesktop")
                            Case roAppType.VTPortal
                                bolRet = oRow("BlockAccessVTPortal")
                            Case roAppType.VTVisits
                                bolRet = oRow("BlockAccessVTVisits")
                            Case Else
                                bolRet = False
                        End Select
                    Next
                End If

                ' En caso que el acceso sea a VT Desktop y el usuario este dentro del grupo administrador siempre tiene acceso
                If oState.AppType = roAppType.VTLive AndAlso bolRet AndAlso groupFeature = 0 Then
                    bolRet = False
                End If
            Catch ex As Exception
                bolRet = False
            End Try

            Return bolRet
        End Function

        Public Shared Function GetAuthMethodUsed(ByVal authToken As String, ByVal securityToken As String, ByRef oState As roSecurityState) As AuthenticationMethod
            Dim omethod As AuthenticationMethod = AuthenticationMethod.Password
            Try

                Dim strSQL As String = "@SELECT# Method FROM sysroPassports_Data WITH (NOLOCK) WHERE AuthToken = '" & authToken & "'" & " and securityToken = '" & securityToken & "'"

                Dim iMethod As Integer = roTypes.Any2Integer(AccessHelper.ExecuteScalar(strSQL))
                If iMethod > 0 Then omethod = iMethod

            Catch ex As Exception
                oState.UpdateStateInfo(ex, "WLHelper::GetAuthMethodUsed")
            End Try

            Return omethod
        End Function

        Public Shared Function ValidateSecurityTokens(ByVal authToken As String, ByVal securityToken As String, ByRef timeZone As String, ByVal bExcludeUpdateSesstion As Boolean, ByVal oMethod As AuthenticationMethod, ByRef oState As roSecurityState) As roPassportTicket
            Dim oRet As roPassportTicket = Nothing
            Try
                Dim iPassportID As Integer = -1
                ' Verificamos que el servidor esté en marxa

                Dim sessionContext As String = String.Empty
                oRet = roPassportManager.GetPassportIDBySecurityTokens(iPassportID, authToken, securityToken, sessionContext, timeZone)
                If oRet IsNot Nothing Then
                    If sessionContext <> String.Empty Then
                        Dim context As String() = sessionContext.Split("%")
                        If context.Length = 2 Then
                            oState.SessionID = context(0)
                            oState.ClientAddress = context(1)
                            If Not bExcludeUpdateSesstion AndAlso Not SessionHelper.SessionCheck(oState.SessionID, oRet.ID, False, oMethod, oState) Then
                                oRet = Nothing
                            End If
                        Else
                            oState.Result = SecurityResultEnum.SessionExpired
                        End If
                    Else
                        oState.Result = SecurityResultEnum.SessionExpired
                    End If
                Else
                    oState.Result = SecurityResultEnum.SecurityTokenNotValid
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "WLHelper::ValidateSecurityTokens")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "WLHelper::ValidateSecurityTokens")
            End Try

            Return oRet
        End Function

        Public Shared Function SetSecurityTokens(ByVal idPassport As Integer, ByVal bIsSupervisor As Boolean, ByVal authToken As String, ByVal securityToken As String, ByVal method As AuthenticationMethod, ByRef oState As roSecurityState) As Boolean
            Dim bRet As Boolean = False

            Dim sessionContext As String = String.Empty
            If authToken <> String.Empty AndAlso securityToken <> String.Empty Then sessionContext = oState.SessionID & "%" & oState.ClientAddress

            Dim strSQL As String = "IF EXISTS(@SELECT# 1 from sysroPassports_Data where IDPassport = " & idPassport & " AND AppCode='" & oState.GetApplicationSourceName() & "')" &
                                    " BEGIN" &
                                    " @UPDATE# sysroPassports_Data SET AuthToken = '" & authToken & "', SecurityToken = '" & securityToken & "', SessionContext='" & sessionContext & "', IsSupervisor=" & If(bIsSupervisor, 1, 0) & ", Method= " & CInt(method) & " WHERE IDPassport = " & idPassport & " AND AppCode='" & oState.GetApplicationSourceName() & "'" &
                                    " END"

            bRet = AccessHelper.ExecuteSql(strSQL)

            Return bRet
        End Function


        Public Shared Function CheckAliasPassport(ByVal strAliasToken As String, ByRef oState As roSecurityState) As roPassportTicket
            Dim oRet As roPassportTicket = Nothing
            Try
                Dim iEmployeePassportID As Integer = GetkAliasPassportId(strAliasToken, oState)

                If iEmployeePassportID <> -1 Then
                    oRet = roPassportManager.GetPassportTicket(iEmployeePassportID, LoadType.Employee)
                Else
                    oRet = Nothing
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "WLHelper::ValidateSecurityTokens")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "WLHelper::ValidateSecurityTokens")
            End Try

            Return oRet
        End Function

        Public Shared Function GetkAliasPassportId(ByVal strAliasToken As String, ByRef oState As roSecurityState) As Integer
            Dim iEmployeePassportID As Integer = -1
            Try
                If strAliasToken.Length > 0 Then
                    Try
                        Dim tmpIdStr As String = System.Text.Encoding.Default.GetString(Convert.FromBase64String(strAliasToken.Replace("*", "="))).Replace("roImpersonateUser##", "")
                        iEmployeePassportID = roTypes.Any2Integer(tmpIdStr.Replace("##", ""))
                    Catch ex As Exception
                        iEmployeePassportID = -1
                    End Try
                End If

            Catch ex As DbException
                oState.UpdateStateInfo(ex, "WLHelper::GetkAliasPassportId")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "WLHelper::GetkAliasPassportId")
            End Try

            Return iEmployeePassportID
        End Function


        Private Shared Function Authenticate(ByVal method As Integer, ByVal credential As String, ByVal password As String, Optional ByVal version As String = "") As Nullable(Of Integer)
            Try

                Dim Command As DbCommand = AccessHelper.CreateCommand("WebLogin_Authenticate")
                Command.CommandType = CommandType.StoredProcedure
                AccessHelper.AddParameter(Command, "@method", DbType.Int32).Value = method
                AccessHelper.AddParameter(Command, "@credential", DbType.String, 255).Value = credential
                AccessHelper.AddParameter(Command, "@password", DbType.String, 1000).Value = password
                AccessHelper.AddParameter(Command, "@version", DbType.String, 50).Value = version
                AccessHelper.AddParameter(Command, "@biometricID", DbType.Int32).Value = 0
                Dim Result As Object = Command.ExecuteScalar()
                If Result Is Nothing OrElse Result Is DBNull.Value Then
                    Dim oPassporttmp As roPassport = Nothing
                    oPassporttmp = roPassportManager.GetPassportByCredential(credential, method, version)
                    ' Si la cuenta es de Robotics no guardamos los intentos erroneos
                    If Not (oPassporttmp IsNot Nothing AndAlso oPassporttmp.IsPrivateUser) Then
                        AuthHelper.SetInvalidAccessAttemps(method, credential, version)
                    End If

                    Return Nothing
                Else
                    Return CInt(Result)
                End If
            Finally

            End Try
        End Function

        Private Shared Function SetInvalidAccessAttemps(ByVal method As Integer, ByVal credential As String, ByVal version As String) As Boolean
            '
            ' Si es un intento de autentificación de Login (method 1 y con una credential existente, guardamos el intento de acceso erroneo)
            '

            Try

                Dim Command As DbCommand = AccessHelper.CreateCommand("WebLogin_Authenticate")
                Command.CommandType = CommandType.StoredProcedure

                If method = 1 AndAlso credential.Length > 0 Then
                    'Si existe la credencial, obtenemos el passport y actualizamos el contador de errores
                    If roPassportManager.CredentialExists(credential, method, version, Nothing) Then
                        Dim table As New DataTable()
                        Command = AccessHelper.CreateCommand("WebLogin_Passports_AuthenticationMethod_Select")
                        Command.CommandType = CommandType.StoredProcedure
                        AccessHelper.AddParameter(Command, "@method", DbType.Int32).Value = AuthenticationMethod.Password
                        AccessHelper.AddParameter(Command, "@version", DbType.String, 50).Value = version
                        Dim Adapter As DbDataAdapter = AccessHelper.CreateDataAdapter(Command)
                        Adapter.Fill(table)
                        Dim Rows As DataRow() = table.Select("Credential = '" & credential.Replace("'", "''") & "'")

                        For Each oRow As DataRow In Rows
                            If Not IsDBNull(oRow("IDPassport")) And oRow("BloquedAccessApp") = False Then
                                ' Aumentamos contador de accesos erroneos
                                ' Actualizamos fecha de intento erroneo
                                Dim InvalidAccessAttemps As Integer = 0
                                If IsDBNull(oRow("InvalidAccessAttemps")) Then
                                    InvalidAccessAttemps = 1
                                Else
                                    InvalidAccessAttemps = oRow("InvalidAccessAttemps") + 1
                                End If

                                ' Comprobamos si tenemos que bloquear la cuenta porque se ha excedido el maximo de intentos de acceso
                                Dim AccessAttempsPermanentBlock As Integer = 10
                                Dim BloquedAccessApp As Boolean = False
                                Dim LastDateInvalidAccessAttempted As DateTime = Now

                                Dim strSQL As String = "@SELECT# AccessAttempsPermanentBlock FROM sysroSecurityParameters " &
                                                       "WHERE IDPassport = 0 "
                                Dim cmd As DbCommand = AccessHelper.CreateCommand(strSQL)

                                Dim ret As Object = cmd.ExecuteScalar()
                                If Not IsDBNull(ret) Then
                                    AccessAttempsPermanentBlock = Val(ret)
                                End If

                                ' Si llegamos a los intentos de bloqueo temporal
                                ' enviamos mail al usuario/empleado
                                Dim AccessAttempsTemporaryBlock As Integer = 0
                                strSQL = "@SELECT# AccessAttempsTemporaryBlock FROM sysroSecurityParameters " &
                                                       "WHERE IDPassport = 0 "
                                cmd = AccessHelper.CreateCommand(strSQL)

                                ret = cmd.ExecuteScalar()
                                If Not IsDBNull(ret) Then
                                    AccessAttempsTemporaryBlock = Val(ret)
                                End If

                                If AccessAttempsTemporaryBlock = InvalidAccessAttemps Then
                                    ' Insertamos registro en la tabla de sysroNotificationTasks
                                    ' Tipo Notification  = 36 Envio de mail cuando cuenta bloqueada temporalmente
                                    ' ID Notification  = 1900

                                    ' Obtenemos el IDEmployee del passport relacionado en caso que lo tenga
                                    Dim intIDEmployee As Integer = 0
                                    strSQL = "@SELECT# isnull(IDEmployee, 0) FROM sysroPassports  " &
                                                           "WHERE ID = " & oRow("IDPassport")
                                    cmd = AccessHelper.CreateCommand(strSQL)
                                    ret = cmd.ExecuteScalar()
                                    If Not IsDBNull(ret) Then
                                        intIDEmployee = Val(ret)
                                    End If

                                    strSQL = "@INSERT# INTO sysroNotificationTasks (IDNotification, Key1Numeric, Key2Numeric ) VALUES " &
                                        "(1900, " & intIDEmployee & ", " & oRow("IDPassport") & ")"
                                    cmd = AccessHelper.CreateCommand(strSQL)
                                    cmd.ExecuteNonQuery()

                                End If

                                ' Si se han producido igual o mas accesos incorrectos que el parametro de configuracion bloqueamos la cuenta
                                ' Solo en el caso que no sea del grupo administrador
                                Dim bolIsAdminGroup As Boolean = False

                                If InvalidAccessAttemps >= AccessAttempsPermanentBlock Then

                                    ' Verificamos si alguno de sus padres es el grupo administrador
                                    strSQL = "@SELECT# ID FROM dbo.GetPassportParents(@IDPassport) WHERE ID = 3 "
                                    cmd = AccessHelper.CreateCommand(strSQL)
                                    AccessHelper.AddParameter(cmd, "@IDPassport", DbType.Int32).Value = oRow("IDPassport")
                                    Dim GroupAdmin As Object = cmd.ExecuteScalar()
                                    If Not IsDBNull(GroupAdmin) Then
                                        If GroupAdmin > 0 Then
                                            bolIsAdminGroup = True
                                        End If
                                    End If

                                    If Not bolIsAdminGroup Then
                                        BloquedAccessApp = True
                                    End If
                                End If

                                If AccessAttempsPermanentBlock = InvalidAccessAttemps And BloquedAccessApp Then
                                    ' Insertamos registro en la tabla de sysroNotificationTasks
                                    ' Tipo Notification  = 37 Envio de mail cuando cuenta bloqueada indefinidamente
                                    ' ID Notification  = 1901

                                    ' Obtenemos el IDEmployee del passport relacionado en caso que lo tenga
                                    Dim intIDEmployee As Integer = 0
                                    strSQL = "@SELECT# isnull(IDEmployee, 0) FROM sysroPassports  " &
                                                           "WHERE ID = " & oRow("IDPassport")
                                    cmd = AccessHelper.CreateCommand(strSQL)
                                    ret = cmd.ExecuteScalar()
                                    If Not IsDBNull(ret) Then
                                        intIDEmployee = Val(ret)
                                    End If

                                    strSQL = "@INSERT# INTO sysroNotificationTasks (IDNotification, Key1Numeric, Key2Numeric ) VALUES " &
                                        "(1901, " & intIDEmployee & ", " & oRow("IDPassport") & ")"
                                    cmd = AccessHelper.CreateCommand(strSQL)
                                    cmd.ExecuteNonQuery()

                                End If

                                If BloquedAccessApp Then
                                    InvalidAccessAttemps = 0
                                End If

                                Command = AccessHelper.CreateCommand("WebLogin_Passports_AuthenticationMethods_Update")
                                Command.CommandType = CommandType.StoredProcedure
                                AccessHelper.AddParameter(Command, "@idPassport", DbType.Int32, 0, "IDPassport").Value = oRow("IDPassport")
                                AccessHelper.AddParameter(Command, "@method", DbType.Int32, 0, "Method").Value = oRow("Method")
                                AccessHelper.AddParameter(Command, "@version", DbType.String, 50, "Version").Value = oRow("Version")
                                AccessHelper.AddParameter(Command, "@credential", DbType.String, 255, "Credential").Value = oRow("Credential")
                                AccessHelper.AddParameter(Command, "@password", DbType.String, 1000, "Password").Value = oRow("Password")
                                AccessHelper.AddParameter(Command, "@startDate", DbType.DateTime, 0, "StartDate").Value = oRow("StartDate")
                                AccessHelper.AddParameter(Command, "@expirationDate", DbType.DateTime, 0, "ExpirationDate").Value = oRow("ExpirationDate")
                                AccessHelper.AddParameter(Command, "@biometricID", DbType.Int32, 0, "BiometricID").Value = oRow("BiometricID")
                                AccessHelper.AddParameter(Command, "@timestamp", DbType.DateTime, 0, "TimeStamp").Value = oRow("TimeStamp")
                                AccessHelper.AddParameter(Command, "@enabled", DbType.Boolean, 1, "Enabled").Value = oRow("Enabled")
                                AccessHelper.AddParameter(Command, "@LastUpdatePassword", DbType.DateTime, 1, "LastUpdatePassword").Value = oRow("LastUpdatePassword")
                                AccessHelper.AddParameter(Command, "@BloquedAccessApp", DbType.Boolean, 1, "BloquedAccessApp").Value = BloquedAccessApp
                                AccessHelper.AddParameter(Command, "@InvalidAccessAttemps", DbType.Int32, 0, "InvalidAccessAttemps").Value = InvalidAccessAttemps
                                AccessHelper.AddParameter(Command, "@LastDateInvalidAccessAttempted", DbType.DateTime, 1, "LastDateInvalidAccessAttempted").Value = IIf(BloquedAccessApp, Global.System.Convert.DBNull, LastDateInvalidAccessAttempted)
                                Command.ExecuteNonQuery()
                                Exit For

                            End If
                        Next

                    End If
                End If
            Finally

            End Try

            Return True

        End Function

        ''' <summary>
        ''' IsRoboticsUserAllowed
        ''' </summary>
        Private Shared Function IsRoboticsUserAllowed() As Boolean
            Dim bolRet As Boolean = False

            Try
                ' Parametros generales
                Dim table As New DataTable()
                Dim strSQL As String = "@SELECT# EnabledAccessSupportRobotics FROM sysroSecurityParameters " &
                                       "WHERE IDPassport = 0 "
                Dim cmd As DbCommand = AccessHelper.CreateCommand(strSQL)
                AccessHelper.AddParameter(cmd, "@IDPassport", DbType.Int32).Value = 0

                Dim Adapter As DbDataAdapter = AccessHelper.CreateDataAdapter(cmd)
                Adapter.Fill(table)
                Dim Rows As DataRow() = table.Select

                If Rows IsNot Nothing AndAlso Rows.Length > 0 Then
                    For Each oRow As DataRow In Rows
                        bolRet = oRow("EnabledAccessSupportRobotics")
                    Next
                End If
            Catch ex As DbException
                bolRet = False
            Catch ex As Exception
                bolRet = False
            End Try

            Return bolRet
        End Function

        Public Shared Function ResetValidationCodeRobotics(ByVal idPassport As Integer) As Boolean
            '
            ' Reseteamos la marca para pedir el codigo de validacion
            '
            Dim bolRet As Boolean

            Try

                Dim strSQL As String = "@UPDATE# sysroPassports SET NeedValidationCode=0,ValidationCode='',TimeStampValidationCode=NULL  WHERE ID=" & idPassport
                Dim cmd As DbCommand = AccessHelper.CreateCommand(strSQL)
                cmd.ExecuteNonQuery()

                bolRet = True
            Catch ex As DbException
                bolRet = False
            Catch ex As Exception
                bolRet = False
            End Try

            Return bolRet

        End Function

        Private Shared Function ValidateRoboticsAccount(ByVal strMail As String) As Boolean
            Return True
        End Function

        ''' <summary>
        ''' NeedRoboticsPassportKey
        ''' </summary>
        Private Shared Function NeedRoboticsPassportKey(ByVal idpassport As Integer, ByRef KeyRequestExpired As Boolean, ByVal strMail As String) As Boolean
            Dim bolRet As Boolean

            Try
                Dim table As New DataTable()
                Dim TimeStampValidationCode As DateTime = Now
                Dim Rows As DataRow() = Nothing
                Dim Adapter As DbDataAdapter = Nothing
                Dim bolNeedValidationCode As Boolean = False

                Dim tb As New DataTable("sysroPassport")
                Dim strSQL As String = "@SELECT# isnull(NeedValidationCode, 0) as NeedValidationCode , TimeStampValidationCode FROM sysroPassports " &
                                       "WHERE ID=" & idpassport
                Dim cmd As DbCommand = AccessHelper.CreateCommand(strSQL)
                table = New DataTable()
                Adapter = AccessHelper.CreateDataAdapter(cmd)
                Adapter.Fill(table)
                Rows = table.Select
                If Rows IsNot Nothing AndAlso Rows.Length > 0 Then
                    For Each oRow As DataRow In Rows
                        bolNeedValidationCode = oRow("NeedValidationCode")
                        If Not IsDBNull(oRow("TimeStampValidationCode")) Then
                            TimeStampValidationCode = oRow("TimeStampValidationCode")
                        End If
                    Next
                End If

                KeyRequestExpired = False

                If bolNeedValidationCode Then
                    ' Si no se ha excedido el tiempo de 10 minutos pedimos el codigo de validacion
                    If DateDiff(DateInterval.Minute, TimeStampValidationCode, Now) <= 10 Then
                        bolRet = True
                        Return bolRet
                    End If

                    KeyRequestExpired = True
                End If

                ' marcamos el passport conforme se tiene que pedir código de validación
                ' nos guardamos el codigo de validacion y la fecha en la que hemos enviado el codigo
                Dim strValidationCode As String = ""

                Dim r As New Random()
                strValidationCode = CStr(r.Next(0, 9))
                strValidationCode += CStr(r.Next(0, 9))
                strValidationCode += CStr(r.Next(0, 9))
                strValidationCode += CStr(r.Next(0, 9))

                strValidationCode = CryptographyHelper.Encrypt(strValidationCode)

                strSQL = "@UPDATE# sysroPassports SET NeedValidationCode=1,ValidationCode='" & strValidationCode & "',TimeStampValidationCode=getdate()  WHERE ID=" & idpassport
                cmd = AccessHelper.CreateCommand(strSQL)
                cmd.ExecuteNonQuery()

                ' enviamos mail al usuario con el codigo de validacion
                ' creamos una notificacion de envio

                ' Tipo Notification  = 38 Envio de mail con codigo de validacion
                ' ID Notification  = 1902
                strSQL = "@INSERT# INTO sysroNotificationTasks (IDNotification, Key1Numeric, Key2Numeric, Parameters, Executed) output INSERTED.ID VALUES " &
                    "(1902, 0, " & idpassport & ",'" & strMail.Replace("'", "''") & "',1)"
                Dim iNew As Integer = roTypes.Any2Integer(DataLayer.AccessHelper.ExecuteScalar(strSQL))

                SendValidationCode(iNew)

                bolRet = True
            Catch ex As DbException
                bolRet = False
            Catch ex As Exception
                bolRet = False
            End Try

            Return bolRet
        End Function

        ''' <summary>
        ''' Reset invalid access on passport
        ''' </summary>
        Private Shared Function SetResetInvalidAccessAttemps(ByVal method As AuthenticationMethod, ByVal credential As String, ByVal version As String, ByVal idpassport As Integer) As Boolean

            Try

                Dim Command As DbCommand = AccessHelper.CreateCommand("WebLogin_Authenticate")
                Command.CommandType = CommandType.StoredProcedure

                ' Si es un intento de autentificación de Login (method 1 y con una credential existente
                ' reseteamos el contador de errores
                If method = 1 AndAlso credential.Length > 0 Then
                    Dim table As New DataTable()
                    Command = AccessHelper.CreateCommand("WebLogin_Passports_AuthenticationMethods_Select")
                    Command.CommandType = CommandType.StoredProcedure
                    AccessHelper.AddParameter(Command, "@method", DbType.Int32).Value = AuthenticationMethod.Password
                    AccessHelper.AddParameter(Command, "@version", DbType.String, 50).Value = version
                    AccessHelper.AddParameter(Command, "@idPassport", DbType.Int32, 0).Value = idpassport
                    Dim Adapter As DbDataAdapter = AccessHelper.CreateDataAdapter(Command)
                    Adapter.Fill(table)
                    Dim Rows As DataRow() = table.Select

                    For Each oRow As DataRow In Rows
                        If Not IsDBNull(oRow("IDPassport")) Then
                            ' Reseteamos los intentos erroneos y la ultima fecha de intento erroneo
                            Command = AccessHelper.CreateCommand("WebLogin_Passports_AuthenticationMethods_Update")
                            Command.CommandType = CommandType.StoredProcedure
                            AccessHelper.AddParameter(Command, "@idPassport", DbType.Int32, 0, "IDPassport").Value = oRow("IDPassport")
                            AccessHelper.AddParameter(Command, "@method", DbType.Int32, 0, "Method").Value = oRow("Method")
                            AccessHelper.AddParameter(Command, "@version", DbType.String, 50, "Version").Value = oRow("Version")
                            AccessHelper.AddParameter(Command, "@credential", DbType.String, 255, "Credential").Value = oRow("Credential")
                            AccessHelper.AddParameter(Command, "@password", DbType.String, 1000, "Password").Value = oRow("Password")
                            AccessHelper.AddParameter(Command, "@startDate", DbType.DateTime, 0, "StartDate").Value = oRow("StartDate")
                            AccessHelper.AddParameter(Command, "@expirationDate", DbType.DateTime, 0, "ExpirationDate").Value = oRow("ExpirationDate")
                            AccessHelper.AddParameter(Command, "@biometricID", DbType.Int32, 0, "BiometricID").Value = oRow("BiometricID")
                            AccessHelper.AddParameter(Command, "@timestamp", DbType.DateTime, 0, "TimeStamp").Value = oRow("TimeStamp")
                            AccessHelper.AddParameter(Command, "@enabled", DbType.Boolean, 1, "Enabled").Value = oRow("Enabled")
                            AccessHelper.AddParameter(Command, "@LastUpdatePassword", DbType.DateTime, 1, "LastUpdatePassword").Value = oRow("LastUpdatePassword")
                            AccessHelper.AddParameter(Command, "@BloquedAccessApp", DbType.Boolean, 1, "BloquedAccessApp").Value = oRow("BloquedAccessApp")
                            AccessHelper.AddParameter(Command, "@InvalidAccessAttemps", DbType.Int32, 0, "InvalidAccessAttemps").Value = 0
                            AccessHelper.AddParameter(Command, "@LastDateInvalidAccessAttempted", DbType.DateTime, 1, "LastDateInvalidAccessAttempted").Value = Global.System.Convert.DBNull
                            Command.ExecuteNonQuery()
                            Exit For
                        End If
                    Next
                End If
            Catch ex As Exception
                Return False
            End Try

            Return True
        End Function


        Private Shared Sub EvaluateClientLocation(ByVal Rows() As DataRow, ByVal strClientLocation As String, ByRef bolIsValidLocation As Boolean)
            bolIsValidLocation = True
            If Rows IsNot Nothing AndAlso Rows.Length > 0 Then
                For Each oRow As DataRow In Rows
                    Dim strOnlyAllowedAdress As String = oRow("OnlyAllowedAdress")
                    Dim strListofAdress As String = ""
                    If strOnlyAllowedAdress.Length > 0 Then
                        For i As Integer = 0 To strOnlyAllowedAdress.Split("#").Length - 1
                            If Not strOnlyAllowedAdress.Split("#")(i).Contains(":") Then
                                strListofAdress = strListofAdress & ",@" & strOnlyAllowedAdress.Split("#")(i) & "@"
                            Else
                                Dim strRange As String = strOnlyAllowedAdress.Split("#")(i)
                                Dim strFrom As String = strRange.Split(":")(0)
                                Dim strTo As String = strRange.Split(":")(1)
                                Dim strTmp As String = ""

                                For z As Integer = 0 To strFrom.Split(".").Length - 2
                                    If strTmp.Length = 0 Then
                                        strTmp = strFrom.Split(".")(z)
                                    Else
                                        strTmp = strTmp & "." & strFrom.Split(".")(z)
                                    End If
                                Next
                                For x As Integer = strFrom.Split(".")(strFrom.Split(".").Length - 1) To strTo
                                    strListofAdress = strListofAdress & ",@" & strTmp & "." & x & "@"
                                Next
                            End If
                        Next

                        ' Si la ip no esta dentro de la lista de IP validas no permitimos el acceso
                        If Not strListofAdress.Contains("@" & strClientLocation & "@") Then
                            bolIsValidLocation = False
                        End If
                    End If
                Next
            End If

        End Sub

        ''' <summary>
        ''' IsValidVersionAPP
        ''' </summary>
        Private Shared Function IsValidVersionAPP(ByVal idpassport As Integer, ByVal bisAccessingFromApp As Boolean, ByVal strVersion As String, ByVal strVersionServer As String) As Boolean
            '
            ' Validamos si la APP tiene la misma version que el servior de VT
            '
            Dim bolRet As Boolean = True

            Try

                If Not bisAccessingFromApp Then
                    Return bolRet
                    Exit Function
                End If

                ' Parametros generales
                Dim table As New DataTable()

                Dim tb As New DataTable("sysroSecurityParameters")
                Dim strSQL As String = "@SELECT# isnull(OnlySameVersionApp, 0) as OnlySameVersionApp FROM sysroSecurityParameters " &
                                       "WHERE IDPassport = @IDPassport "
                Dim cmd As DbCommand = AccessHelper.CreateCommand(strSQL)
                AccessHelper.AddParameter(cmd, "@IDPassport", DbType.Int32).Value = 0

                Dim Adapter As DbDataAdapter = AccessHelper.CreateDataAdapter(cmd)
                Dim bolOnlySameVersionApp As Boolean = False

                Adapter.Fill(table)
                Dim Rows() As DataRow = table.Select
                If Rows IsNot Nothing AndAlso Rows.Length > 0 Then
                    For Each oRow As DataRow In Rows
                        bolOnlySameVersionApp = oRow("OnlySameVersionApp")
                    Next
                End If

                If Not bolOnlySameVersionApp Then
                    table = New DataTable()
                    ' Verificamos si alguno de sus padres tiene marcado validar la version de APP
                    strSQL = "@SELECT# isnull(OnlySameVersionApp, 0) as OnlySameVersionApp FROM sysroSecurityParameters " &
                             "WHERE IDPassport IN (@SELECT# ID FROM dbo.GetPassportParents(@IDPassport)) AND isnull(OnlySameVersionApp, 0) = 1  "
                    cmd = AccessHelper.CreateCommand(strSQL)
                    AccessHelper.AddParameter(cmd, "@IDPassport", DbType.Int32).Value = idpassport
                    cmd.Parameters("@IDPassport").Value = idpassport
                    Adapter = AccessHelper.CreateDataAdapter(cmd)
                    Adapter.Fill(table)
                    Rows = table.Select
                    If Rows IsNot Nothing AndAlso Rows.Length > 0 Then
                        For Each oRow As DataRow In Rows
                            bolOnlySameVersionApp = oRow("OnlySameVersionApp")
                        Next
                    End If
                End If

                ' Validamos la version de la APP que sea igual que la de VT
                If bolOnlySameVersionApp AndAlso Not strVersionServer.Trim.Contains(strVersion.Trim) Then
                    bolRet = False
                End If
            Catch ex As DbException
                bolRet = True
            Catch ex As Exception
                bolRet = True
            End Try

            Return bolRet
        End Function

        ''' <summary>
        ''' TemporaryKeyRequest
        ''' </summary>
        Public Shared Function LoginKeyRequiered(ByVal idpassport As Integer, ByVal strClientLocation As String, ByRef keyRequestExpired As Boolean) As Boolean
            '
            ' Validamos si se necesita pedir validacion de clave temporal y guardamos IP y usuario autorizado
            '

            Try
                If strClientLocation <> "127.0.0.1" AndAlso strClientLocation <> "::1" Then
                    strClientLocation = strClientLocation.Split(":")(0)
                End If

                Dim secState As New roSecurityOptionsState(roConstants.GetSystemUserId())
                Dim secOptions As roSecurityOptions = roSecurityOptions.GetInheritedSecurityOptions(idpassport, secState)

                If secOptions Is Nothing Then Return False

                ' Si no se necesita solicitar clave salimos y actualizamos el passport
                If Not secOptions.RequestValidationCode Then
                    AccessHelper.ExecuteSql("@UPDATE# sysroPassports SET NeedValidationCode = 0, ValidationCode = NULL, TimeStampValidationCode= NULL WHERE ID =  " & idpassport)
                    AccessHelper.ExecuteSql("@DELETE# FROM sysroPassports_AuthorizedAdress WHERE IDPassport= " & idpassport)

                    Return False
                End If

                ' Miramos si ya esta marcado pedir la clave temporal y si han pasado los 10 minutos
                Dim dtPassport As DataTable = AccessHelper.CreateDataTable("@SELECT# isnull(NeedValidationCode, 0) as NeedValidationCode , TimeStampValidationCode, ValidationCode FROM sysroPassports WHERE ID=" & idpassport)
                If dtPassport Is Nothing OrElse dtPassport.Rows.Count = 0 Then Return False

                keyRequestExpired = False
                If roTypes.Any2Boolean(dtPassport.Rows(0)("NeedValidationCode")) Then

                    Dim TimeStampValidationCode As DateTime = Now
                    If Not IsDBNull(dtPassport.Rows(0)("TimeStampValidationCode")) Then TimeStampValidationCode = dtPassport.Rows(0)("TimeStampValidationCode")

                    ' Si no se ha excedido el tiempo de 15 minutos pedimos el codigo de validacion
                    If DateDiff(DateInterval.Minute, TimeStampValidationCode, Now) <= 15 Then
                        If roTypes.Any2String(dtPassport.Rows(0)("ValidationCode")) = String.Empty Then SendValidationKeyToUser(idpassport)

                        Return True
                    End If

                    ' En caso contrario hay que volver a generar un nuevo codigo de validacion y volver a enviar
                    keyRequestExpired = True
                End If

                'Borramos las ip's caducadas
                AccessHelper.ExecuteSql($"@DELETE# FROM sysroPassports_AuthorizedAdress WHERE IDPassport={idpassport} AND DateDiff(Day, TimeStamp, GETDATE()) > {secOptions.SaveAuthorizedPointDays}")

                ' Miramos si existe la IP y el Usuario con un registro válido
                Dim exists As Object = AccessHelper.ExecuteScalar($"@SELECT# 1 as Total FROM sysroPassports_AuthorizedAdress WHERE Adress = '{strClientLocation}' AND IDPassport={idpassport} AND DATEDIFF(DAY, TimeStamp, GETDATE()) <= {secOptions.SaveAuthorizedPointDays}")

                ' Si ya existe el registro marcamos el passport como que no esta pendiente de ningún código de validación y salimos 
                If Not IsDBNull(exists) AndAlso roTypes.Any2Integer(exists) > 0 Then
                    AccessHelper.ExecuteSql("@UPDATE# sysroPassports SET NeedValidationCode = 0, ValidationCode = NULL, TimeStampValidationCode= NULL WHERE ID =  " & idpassport)
                    Return False
                End If

                'Si llegamos aquí, no existe la IP en la lista de autorizadas y hay que volver a solicitar la clave
                Dim authorizedIps As Integer = roTypes.Any2Integer(AccessHelper.ExecuteScalar($"@SELECT# count(*) as Total FROM sysroPassports_AuthorizedAdress WHERE IDPassport={idpassport}"))

                'Si se ha excedido el número de IPs autorizadas, se elimina la IP más antigua
                If authorizedIps >= secOptions.DifferentAccessPointExceeded Then
                    AccessHelper.ExecuteSql($"@DELETE# FROM sysroPassports_AuthorizedAdress WHERE IDPassport={idpassport} AND ID IN (@SELECT# TOP {(authorizedIps - secOptions.DifferentAccessPointExceeded + 1).ToString()} ID FROM sysroPassports_AuthorizedAdress WHERE IDPassport={idpassport} ORDER BY TimeStamp ASC)")
                End If

                'Si quedan IPs disponibles o no hay ninguna, se autoriza la IP y envíamos el código de validación
                AccessHelper.ExecuteSql($"MERGE INTO sysroPassports_AuthorizedAdress As Target
                                            USING (VALUES ({idpassport},'{strClientLocation}')) AS Source (IDPassport, Adress)
                                            ON Target.IDPassport = Source.IDPassport AND Target.Adress = Source.Adress
                                            WHEN MATCHED THEN
                                                @UPDATE# SET TimeStamp = GETDATE()
                                            WHEN NOT MATCHED THEN
                                                @INSERT# (IDPassport, Adress, TimeStamp)
                                                VALUES (Source.IDPassport, Source.Adress, GETDATE());")

                SendValidationKeyToUser(idpassport)
                Return True

            Catch ex As Exception
                roLog.GetInstance.logMessage(roLog.EventType.roError, "AuthHelper::LoginKeyRequiered::Error ", ex)
            End Try

            Return False
        End Function

        Private Shared Sub SendValidationKeyToUser(idpassport As Integer)
            ' marcamos el passport conforme se tiene que pedir código de validación
            ' nos guardamos el codigo de validacion y la fecha en la que hemos enviado el codigo
            Dim strValidationCode As String = ""

            Dim r As New Random()
            strValidationCode = CStr(r.Next(0, 9))
            strValidationCode += CStr(r.Next(0, 9))
            strValidationCode += CStr(r.Next(0, 9))
            strValidationCode += CStr(r.Next(0, 9))

            strValidationCode = CryptographyHelper.Encrypt(strValidationCode)

            AccessHelper.ExecuteSql("@UPDATE# sysroPassports SET NeedValidationCode=1,ValidationCode='" & strValidationCode & "',TimeStampValidationCode=getdate()  WHERE ID=" & idpassport)

            ' enviamos mail al usuario con el codigo de validacion
            ' creamos una notificacion de envio
            ' Obtenemos el IDEmployee del passport relacionado en caso que lo tenga
            Dim intIDEmployee As Integer = roTypes.Any2Integer(AccessHelper.ExecuteScalar("@SELECT# isnull(IDEmployee, 0) FROM sysroPassports WHERE ID = " & idpassport))
            Dim iNew As Integer = roTypes.Any2Integer(DataLayer.AccessHelper.ExecuteScalar($"@INSERT# INTO sysroNotificationTasks (IDNotification, Key1Numeric, Key2Numeric, Executed) output INSERTED.ID VALUES (1902, {intIDEmployee}, {idpassport},1)"))
            SendValidationCode(iNew)
        End Sub

        Private Shared Function AuthenticateBiometric(ByVal version As String, ByVal biometricData() As Byte, Optional ByVal biometricID As Integer = -1) As Nullable(Of Integer)

            Try

                Dim oRet As Nullable(Of Integer) = Nothing

                Dim table As New DataTable()
                Dim Command As DbCommand = AccessHelper.CreateCommand("WebLogin_Passports_AuthenticationMethod_Select")
                Command.CommandType = CommandType.StoredProcedure
                AccessHelper.AddParameter(Command, "@method", DbType.Int32).Value = AuthenticationMethod.Biometry
                AccessHelper.AddParameter(Command, "@version", DbType.String, 50).Value = version
                Dim Adapter As DbDataAdapter = AccessHelper.CreateDataAdapter(Command)
                Adapter.Fill(table)

                Dim Rows As DataRow() = table.Select(IIf(biometricID <> -1, "BiometricID = " & biometricID, ""))

                Dim bBioData As Byte()
                For Each oRow As DataRow In Rows

                    If Not IsDBNull(oRow("BiometricData")) Then

                        bBioData = oRow("BiometricData")

                        If bBioData.Length = biometricData.Length Then

                            Dim bolEqual As Boolean = True
                            For n As Integer = 0 To bBioData.Length - 1
                                If bBioData(n) <> biometricData(n) Then
                                    bolEqual = False
                                    Exit For
                                End If
                            Next
                            If bolEqual Then
                                oRet = oRow("IDPassport")
                                Exit For
                            End If

                        End If

                    End If

                Next

                Return oRet
            Finally

            End Try
        End Function

        ''' <summary>
        ''' Returns if reset validation code
        ''' </summary>
        ''' <param name="idPassport">The ID of the passport to return ticket for.</param>
        Public Shared Function ResetValidationCode(ByVal idPassport As Integer, ByVal strClientLocation As String) As Boolean
            '
            ' Reseteamos la marca para pedir el codigo de validacion
            '
            Dim bolRet As Boolean = False

            Try
                If strClientLocation <> "127.0.0.1" AndAlso strClientLocation <> "::1" Then
                    strClientLocation = strClientLocation.Split(":")(0)
                End If

                ''''''''''''''''''''''
                Dim secState As New roSecurityOptionsState(roConstants.GetSystemUserId())
                Dim secOptions As roSecurityOptions = roSecurityOptions.GetInheritedSecurityOptions(idPassport, secState)

                AccessHelper.ExecuteSql("@UPDATE# sysroPassports SET NeedValidationCode = 0, ValidationCode = NULL, TimeStampValidationCode= NULL WHERE ID =  " & idPassport)

                ' Eliminamos IP antiguas
                AccessHelper.ExecuteSql($"@DELETE# FROM sysroPassports_AuthorizedAdress WHERE IDPassport={idPassport} AND DateDiff(Day, TimeStamp, GETDATE()) > {secOptions.SaveAuthorizedPointDays}")

                bolRet = True
            Catch ex As Exception
                bolRet = False
                roLog.GetInstance.logMessage(roLog.EventType.roError, "AuthHelper::ResetValidationCode::Error ", ex)
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Returns if validation code is correcto
        ''' </summary>
        ''' <param name="idPassport">The ID of the passport to return ticket for.</param>
        ''' <param name="strCode">Validation code.</param>
        Public Shared Function IsValidCode(ByVal idPassport As Integer, ByVal strCode As String) As Boolean
            '
            ' validamos el codigo de validacion
            '
            Dim bolRet As Boolean = False
            Dim _strCode As String = strCode

            Try

                _strCode = CryptographyHelper.Encrypt(_strCode)

                Dim tb As New DataTable("sysroPassports")
                Dim strSQL As String = "@SELECT# isnull(ValidationCode, '') as ValidationCode FROM sysroPassports " &
                                       "WHERE ID=" & idPassport & " AND ValidationCode='" & _strCode & "'"
                Dim cmd As DbCommand = AccessHelper.CreateCommand(strSQL)
                Dim table As New DataTable()
                Dim Adapter As DbDataAdapter = AccessHelper.CreateDataAdapter(cmd)
                Adapter.Fill(table)
                Dim Rows() As DataRow = Nothing
                Rows = table.Select
                If Rows IsNot Nothing AndAlso Rows.Length > 0 Then
                    bolRet = True
                End If
            Catch ex As DbException
                bolRet = False
            Catch ex As Exception
                bolRet = False
            Finally

            End Try

            Return bolRet

        End Function

        Public Shared Function AccessAttempsTemporaryBlock(ByVal idPassport As Integer) As Integer

            Dim tb As New DataTable("sysroSecurityParameters")
            Dim strSQL As String = "@SELECT# AccessAttempsTemporaryBlock FROM sysroSecurityParameters " &
                                   "WHERE IDPassport = @IDPassport "
            Dim cmd As DbCommand = AccessHelper.CreateCommand(strSQL)
            AccessHelper.AddParameter(cmd, "@IDPassport", DbType.Int32)

            cmd.Parameters("@IDPassport").Value = idPassport

            Dim ret As Object = cmd.ExecuteScalar()
            If Not IsDBNull(ret) Then
                Return Val(ret)
            Else
                Return 0
            End If

        End Function

        Public Shared Function DaysPasswordExpired(ByVal idPassport As Integer) As Integer

            Dim tb As New DataTable("sysroSecurityParameters")
            Dim strSQL As String = "@SELECT# DaysPasswordExpired FROM sysroSecurityParameters " &
                                   "WHERE IDPassport = @IDPassport "
            Dim cmd As DbCommand = AccessHelper.CreateCommand(strSQL)
            AccessHelper.AddParameter(cmd, "@IDPassport", DbType.Int32)

            cmd.Parameters("@IDPassport").Value = idPassport

            Dim ret As Object = cmd.ExecuteScalar()
            If Not IsDBNull(ret) Then
                Return Val(ret)
            Else
                Return 0
            End If

        End Function

#Region "Send validation code"

        Private Shared Sub SendValidationCode(ByVal idNotificationTask As Integer)

            Try

                Dim strSQL As String = String.Empty

                Dim smsSQL As String = "@SELECT# Value from sysroLiveAdvancedParameters where ParameterName = 'VTLive.SMS.Enabled'"
                Dim SMSEnabled As Boolean = roTypes.Any2Boolean(AccessHelper.ExecuteScalar(smsSQL))

                strSQL = "@SELECT# sysroNotificationTasks.*, Notifications.Destination, Notifications.Condition, Notifications.IDType, Notifications.CreatorID, " &
                         "isnull(AllowMail, 0) AS AllowMail, Notifications.Name AS NotificationName " &
                         "FROM sysroNotificationTasks with (nolock), Notifications with (nolock) " &
                         "WHERE sysroNotificationTasks.IDNotification = Notifications.ID " &
                         "AND sysroNotificationTasks.ID = " & idNotificationTask & " " &
                         "ORDER BY sysroNotificationTasks.ID ASC"

                Dim strRet As String = String.Empty

                Dim oSendMail As Mail.SendMail = Nothing
                Dim oSmtpServer As Net.Mail.SmtpClient = Nothing

                Dim tb As DataTable = AccessHelper.CreateDataTable(strSQL)
                If tb IsNot Nothing AndAlso tb.Rows.Count = 1 Then
                    Dim oRow As DataRow = tb.Rows(0)
                    Dim isRoboticsUser As Boolean = False

                    ' Enviar correo al supervisor directo
                    Dim strCredential As String = roTypes.Any2String(AccessHelper.ExecuteScalar("@SELECT# isnull(Credential, '') as credential  FROM sysroPassports_AuthenticationMethods WHERE IDPassport = " & roTypes.Any2String(oRow("Key2Numeric")) & " AND Method=1 "))
                    Dim strCode As String = roTypes.Any2String(AccessHelper.ExecuteScalar("@SELECT# ValidationCode FROM sysroPassports WHERE ID = " & roTypes.Any2String(oRow("Key2Numeric"))))
                    Dim strSMSCode As String = Robotics.VTBase.CryptographyHelper.Decrypt(strCode)
                    Dim userLoginName As String = roTypes.Any2String(AccessHelper.ExecuteScalar("@SELECT# Name FROM sysropassports WHERE ID = " & roTypes.Any2String(oRow("Key2Numeric"))))
                    Dim strSMSDestination As String = roTypes.Any2String(oRow("Parameters"))

                    Dim PassportDescription = roTypes.Any2String(AccessHelper.ExecuteScalar("@SELECT# Description FROM sysropassports WHERE ID = " & roTypes.Any2String(oRow("Key2Numeric"))))

                    If PassportDescription.Contains("@@ROBOTICS@@") Then
                        isRoboticsUser = True
                    End If

                    Dim defaultLanguage As String
                    Dim databaseIdentifier As String

                    defaultLanguage = "ESP"
                    If roTypes.Any2Boolean(VTBase.roConstants.GetConfigurationParameter("VTLive.MultitenantService")) Then
                        databaseIdentifier = roTypes.Any2String(Threading.Thread.GetDomain.GetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString & "_company"))
                    Else
                        databaseIdentifier = roTypes.Any2String(VTBase.roConstants.GetGlobalEnvironmentParameter(GlobalAsaxParameter.CompanyId))
                    End If

                    If SMSEnabled AndAlso isRoboticsUser Then

                        Dim oNotificationItem As New roNotificationItem With {
                                    .Type = NotificationItemType.sms,
                                    .Content = userLoginName & ";" & "" & ";" & strSMSDestination & ";" & databaseIdentifier & ";" & strSMSCode & ";",
                                    .Body = String.Empty,
                                    .Destination = String.Empty,
                                    .Subject = String.Empty
                                }
                        Dim bSend As Boolean = Azure.RoAzureSupport.SendTaskToQueue(roTypes.Any2Integer(oRow("ID")), Azure.RoAzureSupport.GetCompanyName(), roLiveTaskTypes.SendEmail, VTBase.roJSONHelper.SerializeNewtonSoft(oNotificationItem))

                        If bSend Then
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "CNotificationServer::ExecuteSendNotifications::" & oRow("ID") & "::SentMessageToQueue")
                            strRet = "OK"
                        Else
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "CNotificationServer::ExecuteSendNotifications::" & oRow("ID") & "::ErrorSendingMessageToQueue")
                            strRet = "KO"
                        End If
                    Else
                        Dim idEmployee As Long = roTypes.Any2Integer(oRow("Key1Numeric"))

                        ' Intentamos obtener el mail del usuario,
                        ' si no tiene obtenemos la del empleado
                        Dim strMail As String = roTypes.Any2String(AccessHelper.ExecuteScalar("@SELECT# isnull(Email, '') FROM sysroPassports WHERE ID = " & roTypes.Any2String(oRow("Key2Numeric"))))

                        Dim employeeConf As Tuple(Of String, String, Boolean, Long) = GetEmployeeEmailConfiguration(-1, roTypes.Any2String(oRow("Key2Numeric")), "", False, defaultLanguage)
                        If employeeConf Is Nothing Then
                            If Len(roTypes.Any2String(oRow("Parameters"))) > 0 Then strMail = roTypes.Any2String(oRow("Parameters"))
                            employeeConf = New Tuple(Of String, String, Boolean, Long)(roTypes.Any2String(oRow("Parameters")), defaultLanguage, False, idEmployee)
                        End If

                        Dim Params As New ArrayList

                        Params.Add(strCredential)
                        Params.Add(Robotics.VTBase.CryptographyHelper.Decrypt(strCode))

                        Dim strSubject As String = Message("Notification.AdviceValidationCode.Subject", Nothing, , employeeConf.Item2)
                        Dim strBody As String = Message("Notification.AdviceValidationCode.Body", Params, , employeeConf.Item2)

                        strBody = strBody & " </br></br> " & Message("Notification.Message.EndBody.SecurityAdvice", Params, , employeeConf.Item2)

                        'Añadimos la coletilla legal
                        If strBody.Length > 0 Then
                            strBody = strBody & " </br></br> " & Message("Notification.Message.EndBody", , , employeeConf.Item2)
                        End If

                        Dim oNotificationItem As New roNotificationItem With {
                                    .Type = NotificationItemType.email,
                                                        .Content = String.Empty,
                                                        .Body = strBody,
                                                        .Destination = employeeConf.Item1,
                                                        .Subject = strSubject
                                }
                        Dim bSend As Boolean = Azure.RoAzureSupport.SendTaskToQueue(roTypes.Any2Integer(oRow("ID")), Azure.RoAzureSupport.GetCompanyName(), roLiveTaskTypes.SendEmail, VTBase.roJSONHelper.SerializeNewtonSoft(oNotificationItem))

                        If bSend Then
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "CNotificationServer::ExecuteSendNotifications::" & oRow("ID") & "::SentMessageToQueue")
                            strRet = "OK"
                        Else
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "CNotificationServer::ExecuteSendNotifications::" & oRow("ID") & "::ErrorSendingMessageToQueue")
                            strRet = "KO"
                        End If

                    End If

                End If
            Catch Ex As Exception
                'Stop
                roLog.GetInstance().logMessage(roLog.EventType.roError, "CSecurityLayer::SendValidationCode :", Ex)
            End Try
        End Sub

        Private Shared Function Message(ByVal strKey As String, Optional ByVal oParamList As ArrayList = Nothing, Optional ByVal strFileReference As String = "ProcessNotificationServer", Optional ByVal strLanguageKey As String = "ESP", Optional bInvalidateCache As Boolean = False) As String
            Dim strMessage As String = String.Empty

            If strMessage = String.Empty Then
                Dim oLanguage = New roLanguage

                oLanguage.SetLanguageReference(strFileReference, strLanguageKey)
                oLanguage.ClearUserTokens()
                If oParamList IsNot Nothing Then
                    For i As Integer = 0 To oParamList.Count - 1
                        oLanguage.AddUserToken(oParamList(i))
                    Next
                End If

                strMessage = oLanguage.Translate(strKey, "")
            End If

            Return strMessage
        End Function

        Private Shared Function GetEmployeeEmailConfiguration(idEmployee As Long, idPassport As Long, strFieldName As String, bEmployeeNotification As Boolean, Optional ByVal defaultLanguage As String = "ESP") As Tuple(Of String, String, Boolean, Long)
            Dim strDestinationFieldName As String = String.Empty
            Dim oRet As Tuple(Of String, String, Boolean, Long) = Nothing
            Dim employeeLanguage As String = defaultLanguage
            Dim bUseFixedFieldName As Boolean = True

            If strFieldName = String.Empty Then
                bUseFixedFieldName = False
                Dim strSQL As String = "@SELECT# FieldName from sysroUserFields where Alias = 'sysroEmail'"
                strFieldName = roTypes.Any2String(AccessHelper.ExecuteScalar(strSQL))
            End If

            If strFieldName.Length > 0 Then

                Dim tbs As DataTable = Nothing

                Dim strSQL As String = "@SELECT# ISNULL(ID, 0) AS id, isnull(Email, '') AS Email, isnull(IDEmployee, 0) as IDEmployee FROM sysroPassports "

                If idEmployee > 0 Then
                    strSQL = strSQL & "where IDEmployee =" + idEmployee.ToString
                Else
                    strSQL = strSQL & "where ID =" + idPassport.ToString
                End If

                Dim tbPassports As DataTable = AccessHelper.CreateDataTable(strSQL)
                If tbPassports.Rows.Count > 0 Then
                    ' Intetamos obtener el mail del empleado
                    Dim intIDEmployee As Double = roTypes.Any2Double(tbPassports.Rows(0)("IDEmployee"))
                    Dim intPassport As Double = roTypes.Any2Double(tbPassports.Rows(0)("ID"))

                    If intIDEmployee > 0 Then
                        If strFieldName.Length > 0 Then
                            Dim strAux As String = "@DECLARE# @Date smalldatetime " &
                                     "SET @Date = " & roTypes.Any2Time(Now.Date).SQLSmallDateTime & " " &
                                     "@SELECT# * FROM GetEmployeeUserFieldValue(" & roTypes.Any2String(intIDEmployee) & ",'" & strFieldName & "', @Date)"
                            tbs = AccessHelper.CreateDataTable(strAux)
                            If tbs IsNot Nothing AndAlso tbs.Rows.Count > 0 Then
                                strDestinationFieldName = roTypes.Any2String(tbs.Rows(0).Item("Value").ToString).Trim
                            End If
                        End If
                    End If

                    If strDestinationFieldName.Length = 0 AndAlso bUseFixedFieldName = False Then
                        strDestinationFieldName = roTypes.Any2String(tbPassports.Rows(0)("Email")).Trim
                    End If

                    ' No personalizamos pie del email
                    strSQL = "@SELECT# LanguageKey from sysroLanguages where ID = (@SELECT# idLanguage from sysroPassports where ID = " & intPassport & ")"
                    tbs = AccessHelper.CreateDataTable(strSQL)
                    If tbs IsNot Nothing AndAlso tbs.Rows.Count > 0 Then
                        employeeLanguage = roTypes.Any2String(tbs.Rows(0).Item("LanguageKey").ToString)
                        If employeeLanguage = String.Empty Then employeeLanguage = defaultLanguage
                    End If

                End If

                For Each email As String In strDestinationFieldName.Split(";")
                    If email <> String.Empty Then
                        oRet = New Tuple(Of String, String, Boolean, Long)(email, employeeLanguage, bEmployeeNotification, idEmployee)
                    End If
                Next
            End If

            Return oRet
        End Function

#End Region


    End Class

End Namespace
