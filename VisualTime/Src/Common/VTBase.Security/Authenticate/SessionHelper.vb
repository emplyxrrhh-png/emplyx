Imports System.Data.Common
Imports System.Security.AccessControl
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
    Public NotInheritable Class SessionHelper

        Public Shared Function GetCurrentPassportSessionID(ByVal idPassport As Integer, appsource As roAppSource) As String
            Dim sSesssionID As String = String.Empty

            Try
                Dim strSQL As String = $"@SELECT# ID FROM sysroPassports_Sessions WHERE IdPassport={idPassport} and ApplicationName='{appsource.ToString.ToLower}' and InvalidationCode = 0"

                sSesssionID = roTypes.Any2String(Robotics.DataLayer.AccessHelper.ExecuteScalar(strSQL)).Split("*")(0)

            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "SessionHelper::Could not validate sessionID")
            End Try

            Return sSesssionID

        End Function

        Public Shared Function SessionCheck(ByVal SessionID As String, ByVal oPassport As roPassportTicket, ByVal bolIsNewSession As Boolean, ByVal oMethod As AuthenticationMethod, ByVal oState As roSecurityState) As Boolean
            Return SessionHelper.SessionCheck(SessionID, oPassport.ID, bolIsNewSession, oMethod, oState)
        End Function

        Public Shared Function SessionCheck(ByVal SessionID As String, ByVal PassportID As Integer, ByVal bolIsNewSession As Boolean, ByVal oMethod As AuthenticationMethod, ByRef oState As roSecurityState) As Boolean

            Dim bolRet As Boolean = False
            Try

                Dim intMaxConcurrentSessions As Integer = roTypes.Any2Integer(roConstants.GetGlobalEnvironmentParameter(GlobalAsaxParameter.MaxConcurrentSessions))
                Dim intSessionTimeout As Integer = roTypes.Any2Integer(roConstants.GetGlobalEnvironmentParameter(GlobalAsaxParameter.ServerTimeout))

                Dim strClientLocation As String = oState.ClientAddress.Split("#")(0)

                Dim sessionInvalidationCode As Integer = 0
                If SessionHelper.IsSessionAlive(oState, PassportID) Then
                    'tengo una sesión activa con mi id y en código 0, solo tengo que actualizar el updatedate y lastaction
                    If SessionHelper.UpdateSessionLastAccess(oState, PassportID, DateTime.Now, oMethod) Then
                        bolRet = True
                    Else
                        oState.Result = SecurityResultEnum.UpdateSessionError
                    End If
                Else

                    If SessionHelper.HasAvailableSessions(oState, intSessionTimeout, intMaxConcurrentSessions, PassportID) Then
                        If SessionHelper.UpdateSession(oState, PassportID, Now, strClientLocation, sessionInvalidationCode, bolIsNewSession, oMethod) Then
                            Select Case sessionInvalidationCode
                                Case 1
                                    oState.Result = SecurityResultEnum.SessionInvalidatedByPermissions
                                Case 2
                                    oState.Result = SecurityResultEnum.AlreadyLoggedinInOtherLocation
                                Case 3
                                    oState.Result = SecurityResultEnum.SessionExpired
                                Case 4
                                    oState.Result = SecurityResultEnum.SessionInvalidatedOtherUserWithSameSession
                                Case -1
                                    oState.Result = SecurityResultEnum.UpdateSessionError
                            End Select
                            bolRet = True
                        Else
                            oState.Result = SecurityResultEnum.UpdateSessionError
                        End If
                    Else
                        oState.Result = SecurityResultEnum.MaxCurrentSessionsExceeded
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "WLHelper::SessionCheck")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "WLHelper::SessionCheck")
            End Try

            Return bolRet

        End Function

        Public Shared Function GetPassportTicketBySessionID(ByVal iPassportID As Integer, ByVal oMethod As AuthenticationMethod, ByVal oState As roSecurityState) As roPassportTicket
            Dim oRet As roPassportTicket = Nothing

            Try
                ' Obtenemos el máximo de sessiones concurrentes configurado en la licencia
                Dim intMaxSessions As Integer = roTypes.Any2Integer(roConstants.GetConfigurationParameter(GlobalAsaxParameter.MaxConcurrentSessions.ToString()))

                ' Obtenemos el timeout configurado para la caducidad de las sesiones
                Dim intSessionTimeout = roTypes.Any2Integer(roConstants.GetConfigurationParameter(GlobalAsaxParameter.ServerTimeout.ToString()))

                Dim strClientLocation As String = oState.ClientAddress.Split("#")(0)

                Dim sessionInvalidationCode As Integer = 0
                If SessionHelper.IsSessionAlive(oState, iPassportID) Then
                    'tengo una sesión activa con mi id y en código 0, solo tengo que actualizar el updatedate y lastaction
                    If SessionHelper.UpdateSessionLastAccess(oState, iPassportID, DateTime.Now, oMethod) Then
                        oRet = roPassportManager.GetPassportTicket(iPassportID, LoadType.Passport)
                    Else
                        oState.Result = SecurityResultEnum.UpdateSessionError
                    End If
                Else

                    If SessionHelper.HasAvailableSessions(oState, intSessionTimeout, intMaxSessions, iPassportID) Then
                        If SessionHelper.UpdateSession(oState, iPassportID, Now, strClientLocation, sessionInvalidationCode, False, oMethod) Then
                            Select Case sessionInvalidationCode
                                Case 0
                                    oRet = roPassportManager.GetPassportTicket(iPassportID, LoadType.Passport)
                                Case 1
                                    oRet = roPassportManager.GetPassportTicket(iPassportID, LoadType.Passport)
                                    oState.Result = SecurityResultEnum.SessionInvalidatedByPermissions
                                Case 2
                                    oState.Result = SecurityResultEnum.AlreadyLoggedinInOtherLocation
                                Case 3
                                    oState.Result = SecurityResultEnum.SessionExpired
                                Case 4
                                    oState.Result = SecurityResultEnum.SessionInvalidatedOtherUserWithSameSession
                                Case -1
                                    oState.Result = SecurityResultEnum.UpdateSessionError
                            End Select
                        Else
                            oState.Result = SecurityResultEnum.UpdateSessionError
                        End If
                    Else
                        oState.Result = SecurityResultEnum.MaxCurrentSessionsExceeded
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "WLHelper::SessionCheck")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "WLHelper::SessionCheck")
            End Try

            Return oRet

        End Function



        Public Shared Function UpdateConcurrenceStatus(ByRef oState As roSecurityState) As Boolean

            Dim bolRet As Boolean = False
            Try
                Dim intSessionTimeout As Integer = roTypes.Any2Integer(DataLayer.roCacheManager.GetInstance().GetParametersCache(RoAzureSupport.GetCompanyName(), Parameters.SessionTimeout))
                If intSessionTimeout <= 0 Then
                    intSessionTimeout = 30
                End If

                bolRet = SessionHelper.UpdateSessions(intSessionTimeout)
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "WLHelper::SessionCheck")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "WLHelper::SessionCheck")
            End Try

            Return bolRet

        End Function



        Public Shared Function UpdateLastAccessTime(ByVal SessionID As String, ByVal PassportID As Integer, ByRef oState As roSecurityState) As Boolean
            Dim bolRet As Boolean = False

            Try
                Dim strClientLocation As String = oState.ClientAddress.Split("#")(0)

                If SessionHelper.UpdateAccessTime(oState, PassportID, Now, strClientLocation) Then
                    bolRet = True
                Else
                    oState.Result = SecurityResultEnum.UpdateSessionError
                End If
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "WLHelper::SessionCheck")
            End Try

            Return bolRet
        End Function

        Public Shared Function SessionRemove(ByVal SessionID As String, ByVal oState As roSecurityState) As Boolean

            Dim bolRet As Boolean = False

            Try
                bolRet = SessionHelper.DeleteSession(SessionID)
                If Not bolRet Then
                    oState.Result = SecurityResultEnum.DeleteSessionError
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "WLHelper::SessionRemove")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "WLHelper::SessionRemove")
            End Try

            Return bolRet
        End Function


        Public Shared Function SetSessionAsSupervisor(ByVal idPassport As Integer, ByVal authToken As String, ByVal securityToken As String, oMethod As AuthenticationMethod, ByRef oState As roSecurityState) As Boolean
            Dim oRet As Boolean = False

            Try
                Dim strSQL As String = "@SELECT# IsSupervisor FROM sysropassports WHERE Id=" & idPassport

                Dim bIsSupervisor As Boolean = roTypes.Any2Boolean(Robotics.DataLayer.AccessHelper.ExecuteScalar(strSQL))

                If bIsSupervisor Then
                    Dim intMaxConcurrentSessions As Integer = roTypes.Any2Integer(roConstants.GetGlobalEnvironmentParameter(GlobalAsaxParameter.MaxConcurrentSessions))
                    Dim intSessionTimeout = roTypes.Any2Integer(roConstants.GetGlobalEnvironmentParameter(GlobalAsaxParameter.ServerTimeout))

                    If SessionHelper.HasAvailableSessions(oState, intSessionTimeout, intMaxConcurrentSessions, idPassport) Then
                        oRet = True
                    Else
                        oRet = False
                        oState.Result = SecurityResultEnum.MaxCurrentSessionsExceeded
                    End If
                Else
                    oRet = True
                End If

                If oRet Then
                    AuthHelper.SetSecurityTokens(idPassport, bIsSupervisor, authToken, securityToken, oMethod, oState)
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "WLHelper::SetSessionAsSupervisor")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "WLHelper::SetSessionAsSupervisor")
            End Try

            Return oRet
        End Function


        Private Shared Function UpdateAccessTime(ByVal oState As roBaseState, ByVal PassportID As Integer, ByVal xDate As DateTime, ByVal ClientLocation As String) As Boolean
            Dim bolRet As Boolean = False

            Dim strSQL As String

            Dim tb As New DataTable("sysroPassports_Sessions")
            strSQL = "@SELECT# * FROM sysroPassports_Sessions " &
                     "WHERE ID = @SessionID AND ApplicationName like @applicationName  AND ClientLocation = @ClientLocation AND IDPassport = @IDPassport AND InvalidationCode=0"

            Dim cmd As DbCommand = AccessHelper.CreateCommand(strSQL)
            AccessHelper.AddParameter(cmd, "@SessionID", DbType.String)
            AccessHelper.AddParameter(cmd, "@applicationName", DbType.String)
            AccessHelper.AddParameter(cmd, "@ClientLocation", DbType.String)
            AccessHelper.AddParameter(cmd, "@IDPassport", DbType.Int32)

            cmd.Parameters("@SessionID").Value = oState.SessionID
            cmd.Parameters("@applicationName").Value = oState.GetApplicationSourceName().ToString()
            cmd.Parameters("@ClientLocation").Value = ClientLocation
            cmd.Parameters("@IDPassport").Value = PassportID

            Dim ad As DbDataAdapter = AccessHelper.CreateDataAdapter(cmd, True)
            ad.Fill(tb)

            If tb.Rows.Count = 1 Then
                Dim oRow As DataRow = tb.Rows(0)

                oRow("UpdateDate") = xDate

                ad.Update(tb)

                bolRet = True
            Else
                bolRet = False
            End If

            Return bolRet

        End Function

        ''' <summary>
        ''' Elimina la sesión indicada.
        ''' </summary>
        ''' <param name="SessionID">Identificador único de la sesión a eliminar.</param>
        ''' <param name="trans">Opcional. Transacción activa.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function DeleteSession(ByVal SessionID As String) As Boolean

            Dim bolRet As Boolean = False

            Dim strSQL As String = "@SELECT# IDPassport FROM sysroPassports_Sessions " &
                                       "WHERE ID = '" & SessionID & "' AND InvalidationCode = 0"
            Dim iPassport As Integer = VTBase.roTypes.Any2Integer(AccessHelper.ExecuteScalar(strSQL))

            strSQL = "@SELECT# DataId FROM sysroPassports_Sessions " &
                                   "WHERE ID = '" & SessionID & "' AND InvalidationCode = 0"

            Dim iData As String = VTBase.roTypes.Any2String(AccessHelper.ExecuteScalar(strSQL))

            strSQL = "@DELETE# FROM sysroPassports_Sessions " &
                                   "WHERE ID = @SessionID And InvalidationCode = 0"
            Dim cmd As DbCommand = AccessHelper.CreateCommand(strSQL)
            AccessHelper.AddParameter(cmd, "@SessionID", DbType.String)
            cmd.Parameters("@SessionID").Value = SessionID
            cmd.ExecuteNonQuery()

            If iPassport > 0 AndAlso iData <> String.Empty Then
                bolRet = SessionHelper.RemoveSecurityTokens(iData)
            End If

            Return bolRet

        End Function

        Private Shared Function RemoveSecurityTokens(ByVal idData As String) As Boolean
            Dim bRet As Boolean = False

            Dim strSQL As String = "@DELETE# sysroPassports_Data where Id='" & idData & "'"
            bRet = AccessHelper.ExecuteSql(strSQL)
            Return bRet
        End Function

        Private Shared Function IsSessionAlive(ByVal oState As roBaseState, ByVal idPassport As Integer) As Boolean
            Dim bSessionIsAlive As Boolean = False
            Try
                Dim strSQL As String = " @SELECT# count(IDPassport) FROM sysroPassports_Sessions Where " &
                                        " IDPassport = " & idPassport & " AND ApplicationName = '" & oState.GetApplicationSourceName() & "' AND ID = '" & oState.SessionID & "' AND InvalidationCode = 0"

                bSessionIsAlive = roTypes.Any2Integer(AccessHelper.ExecuteScalar(strSQL)) > 0

            Catch ex As Exception
                VTBase.roLog.GetInstance().logMessage(roLog.EventType.roError, "ConnectionAccess::IsSessionAlive::Unhandled error::" & ex.Message, ex)
            End Try

            Return bSessionIsAlive
        End Function


        Public Shared Function UpdateSessionLastAccess(ByVal oState As roBaseState, ByVal PassportID As Integer, ByVal xDate As DateTime, ByVal oMethod As AuthenticationMethod) As Boolean

            Dim bolRet As Boolean = True
            Dim strSQL As String
            Dim bUpdate As Boolean = True

            'Revisamos si es una petición que deba actualizar el estado de sesión para poder mantener el estado correctamente
            Try
                If System.Web.HttpContext.Current IsNot Nothing Then
                    Dim curRequest As System.Web.HttpRequest = System.Web.HttpContext.Current.Request

                    Dim sPath As String = curRequest.Path.ToLower()
                    If sPath.Contains("getdocumentationfaultalerts") OrElse sPath.Contains("getdesktopalerts") OrElse sPath.Contains("getusertasks") OrElse
                            sPath.Contains("getschedulestatus") OrElse sPath.Contains("securitybasesvc") OrElse sPath.Contains("usertaskscheck") Then
                        bUpdate = False
                    End If
                End If

                If bUpdate Then
                    strSQL = "@UPDATE# sysroPassports_Sessions SET UpdateDate = " & roTypes.Any2Time(xDate).SQLDateTime & " WHERE ID = '" & oState.SessionID & "' AND ApplicationName = '" & oState.GetApplicationSourceName() & "' AND IDPassport = " & PassportID.ToString & " AND InvalidationCode = 0"
                    AccessHelper.ExecuteSql(strSQL)
                End If

                strSQL = "@UPDATE# sysroPassports_AuthenticationMethods SET LastAppActionDate = " & roTypes.Any2Time(xDate.Date).SQLDateTime & ", BlockedAccessByInactivity = 0 WHERE Method = " & CInt(oMethod) & " AND IDPassport = " & PassportID.ToString
                AccessHelper.ExecuteSql(strSQL)
            Catch ex As Exception
                bolRet = True
                VTBase.roLog.GetInstance().logMessage(roLog.EventType.roError, "ConnectionAccess::UpdateSessionLastAccess::Unhandled error::" & ex.Message, ex)
            End Try
            Return bolRet

        End Function


        ''' <summary>
        ''' Nos indica si hay sesiones disponibles para poder hacer login en visualtime
        ''' </summary>
        ''' <param name="SessionID">Opcional. Código de session a no tener en cuenta en la selección.</param>
        ''' <param name="SessionTimeout">Tiempo en el que invalidamos una sessión.</param>
        ''' <param name="intMaxConcurrentSessions">Número máximo de licencias concurrentes que se adminten en visualtime.</param>
        ''' <param name="applicationName">Nombre de la aplicación para tener en cuenta como computa en el global de licencias.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function HasAvailableSessions(ByVal oState As roBaseState, ByVal SessionTimeout As Integer, ByVal intMaxConcurrentSessions As Integer, ByVal idPassport As Integer) As Boolean

            Try

                Dim isSupervisor As Boolean = roTypes.Any2Boolean(AccessHelper.ExecuteScalar($"@SELECT# IsSupervisor From sysroPassports where Id = {idPassport}"))
                Dim activeSessions As Double = GetActiveSessions(oState.SessionID, idPassport, SessionTimeout)
                Dim bCheckTotalSessions As Boolean = False

                If oState.AppType = roAppType.VTLive Then
                    activeSessions = activeSessions + roConstants.APPLICATION_Desktop_License
                    bCheckTotalSessions = True
                End If
                If oState.AppType = roAppType.VTPortal AndAlso isSupervisor Then
                    activeSessions = activeSessions + roConstants.APPLICATION_Supervisor_License
                    bCheckTotalSessions = True
                End If

                If (bCheckTotalSessions) Then
                    If activeSessions <= intMaxConcurrentSessions Then
                        Return True
                    Else

                        Dim strSQL As String = " @INSERT# INTO sysroConcurrentLicenses (DateTime, Count, VTLive, VTSupervisor, VTPortal) " &
                                            " @Select# GetDate(), count(sp.ID), " &
                                                    " sum(CASE When sp.ApplicationName='VTLive' Then " & roConstants.APPLICATION_Desktop_License.ToString & " Else 0 End ) as VTLive, " &
                                                    " sum(CASE When sp.ApplicationName='VTSupervisor' Then " & roConstants.APPLICATION_Supervisor_License.ToString.Replace(",", ".") & " Else 0 End ) as VTSupervisor, " &
                                                    " sum(CASE When sp.ApplicationName='VTPortal' Then " & roConstants.APPLICATION_Supervisor_License.ToString.Replace(",", ".") & " Else 0 End ) as VTPortal " &
                                                    " From sysroPassports_Sessions sp WITH(NOLOCK) inner Join sysroPassports_Data spd WITH(NOLOCK) on sp.IDPassport = spd.IDPassport And spd.Id = sp.DataId " &
                                                    " Where sp.InvalidationCode = 0 And spd.IsSupervisor = 1 "

                        AccessHelper.ExecuteSqlWithoutTimeOut(strSQL)

                        Return False
                    End If
                Else
                    Return True
                End If
            Catch ex As Exception
                'do nothing
            End Try

            Return True
        End Function


        ''' <summary>
        ''' Obtiene el número de sesiones activas. Opcionalmente se puede indicar un id de sesión que se excluirá de la selección.
        ''' </summary>
        ''' <param name="SessionID">Opcional. Código de session a no tener en cuenta en la selección.</param>
        ''' <param name="trans">Opcional. Transacción activa.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function GetActiveSessions(ByVal SessionID As String, ByVal intIDPassport As Integer, ByVal SessionTimeout As Integer) As Double

            Dim intRet As Double = 0

            Dim bolIsRobUser As Boolean = False
            Dim isRobUserSQL As String = "@SELECT# TOP 1 * FROM sysroPassports WITH(NOLOCK) WHERE ID=" & intIDPassport

            Dim tbPassport As DataTable = AccessHelper.CreateDataTable(isRobUserSQL)

            If tbPassport IsNot Nothing AndAlso tbPassport.Rows.Count = 1 Then
                Dim strDescriptionUser As String = Robotics.VTBase.roTypes.Any2String(tbPassport.Rows(0)("Description"))
                If strDescriptionUser.StartsWith("@@ROBOTICS@@") Then
                    bolIsRobUser = True
                End If
            End If

            If Not bolIsRobUser Then
                Dim liveActiveConnections As Double = 0
                Dim SupervisorActiveConnections As Double = 0

                Dim tb As DataTable = AccessHelper.CreateDataTable("@SELECT# * FROM sysroPassports_Sessions WITH(NOLOCK) WHERE ID = ''")

                Dim strSQL As String = "@SELECT# COUNT(*) FROM sysroPassports_Sessions srps WITH(NOLOCK) inner join sysroPassports srp WITH(NOLOCK)  on srp.ID = srps.IDPassport " &
                    "inner join sysroPassports_Data srpd WITH(NOLOCK) on srpd.Id = srps.DataId " &
                    "WHERE srp.Description not like '@@ROBOTICS@@%' " &
                    "AND DATEDIFF(minute, srps.UpdateDate, GETDATE()) < @SessionTimeout AND srps.IDPassport <> @idPassport AND srps.InvalidationCode = 0 AND srpd.IsSupervisor = 1"

                If tb.Columns.Contains("ApplicationName") Then strSQL &= " AND ISNULL(ApplicationName, '') = 'VTLive'"
                If SessionID <> "" Then strSQL &= " AND srps.ID <> @SessionID"

                Dim cmd As DbCommand = AccessHelper.CreateCommand(strSQL)

                AccessHelper.AddParameter(cmd, "@SessionTimeout", DbType.Int32)
                cmd.Parameters("@SessionTimeout").Value = SessionTimeout
                AccessHelper.AddParameter(cmd, "@idPassport", DbType.Int32)
                cmd.Parameters("@idPassport").Value = intIDPassport

                If SessionID <> "" Then
                    AccessHelper.AddParameter(cmd, "@SessionID", DbType.String)
                    cmd.Parameters("@SessionID").Value = SessionID
                End If

                Try
                    Dim ret As Object = cmd.ExecuteScalar()
                    If Not IsDBNull(ret) Then
                        liveActiveConnections = Val(ret) * roConstants.APPLICATION_Desktop_License
                    End If
                Catch ex As Exception
                    liveActiveConnections = 0
                End Try

                strSQL = "@SELECT# COUNT(*) FROM sysroPassports_Sessions srps WITH(NOLOCK) inner join sysroPassports srp WITH(NOLOCK)  on srp.ID = srps.IDPassport " &
                    "inner join sysroPassports_Data srpd WITH(NOLOCK) on srpd.Id = srps.DataId " &
                    "WHERE srp.Description not like '@@ROBOTICS@@%' " &
                    "AND DATEDIFF(minute, srps.UpdateDate, GETDATE()) < @SessionTimeout AND srps.IDPassport <> @idPassport AND srps.InvalidationCode = 0 AND srpd.IsSupervisor = 1"
                If tb.Columns.Contains("ApplicationName") Then strSQL &= " AND (ISNULL(ApplicationName, '') = 'VTSupervisor' OR ISNULL(ApplicationName, '') = 'VTPortal') "
                If SessionID <> "" Then strSQL &= " AND srps.ID <> @SessionID"

                cmd = AccessHelper.CreateCommand(strSQL)

                AccessHelper.AddParameter(cmd, "@SessionTimeout", DbType.Int32)
                cmd.Parameters("@SessionTimeout").Value = SessionTimeout
                AccessHelper.AddParameter(cmd, "@idPassport", DbType.Int32)
                cmd.Parameters("@idPassport").Value = intIDPassport

                If SessionID <> "" Then
                    AccessHelper.AddParameter(cmd, "@SessionID", DbType.String)
                    cmd.Parameters("@SessionID").Value = SessionID
                End If

                Try
                    Dim ret As Object = cmd.ExecuteScalar()
                    If Not IsDBNull(ret) Then
                        SupervisorActiveConnections = Val(ret) * roConstants.APPLICATION_Supervisor_License
                    End If
                Catch ex As Exception
                    SupervisorActiveConnections = 0
                End Try

                intRet = liveActiveConnections + SupervisorActiveConnections
            Else
                Return 0
            End If

            Return intRet

        End Function


        ''' <summary>
        ''' Actualiza la información de la sessión a la tabla de sessiones.
        ''' </summary>
        ''' <param name="SessionID">Identificador único de sesión.</param>
        ''' <param name="PassportID">Código de pasaporte.</param>
        ''' <param name="xDate">Fecha de actualización.</param>
        ''' <param name="ClientLocation">Localización del cliente.</param>
        ''' <param name="SessionTimeout"></param>
        ''' <param name="trans"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function UpdateSessions(ByVal SessionTimeout As Integer) As Boolean

            Dim bolRet As Boolean = True
            Dim strSQL As String

            Try
                Dim sDate As String = roTypes.Any2Time(DateTime.Now).SQLSmallDateTime

                'Borramos todas las sesiones que no tengan un IDSession válido
                strSQL = "@DELETE# FROM sysroPassports_Sessions WHERE ID like '-1*%'"
                AccessHelper.ExecuteSql(strSQL)

                'Borramos todas las sesiones que su timeout y el tiempo de grácia haya pasado
                strSQL = "@DELETE# FROM sysroPassports_Sessions " &
                         " WHERE ApplicationName <> 'VTPortal' AND (DATEDIFF(minute, UpdateDate, " & sDate & ") >=  " & (SessionTimeout + 5) & " OR InvalidationCode <> 0)"
                AccessHelper.ExecuteSqlWithoutTimeOut(strSQL)

                ' Borramos todas las sesiones que no tengan un registro en la sysropassports_data válido
                strSQL = "@DELETE# FROM sysroPassports_Sessions " &
                         " WHERE DataID not in (@SELECT# ID FROM sysroPassports_Data WITH(NOLOCK)) "
                AccessHelper.ExecuteSqlWithoutTimeOut(strSQL)

                ' Borramos todas las sesiones marcadas como invalidas con un tiempo de actualización superior al timeout mas un margen
                strSQL = "@DELETE# FROM sysroPassports_Data WHERE RecoverKey is null AND Id NOT IN " &
                         " (@SELECT# DataId FROM sysroPassports_Sessions WITH(NOLOCK))"
                AccessHelper.ExecuteSqlWithoutTimeOut(strSQL)

                ' Marcamos las conexiones inactivas de la tabla de todas las aplicaciones como sesión caducada
                strSQL = "@UPDATE# sysroPassports_Sessions SET InvalidationCode = 3 " &
                             "WHERE DATEDIFF(minute, UpdateDate, GETDATE()) >=  " & SessionTimeout & " AND InvalidationCode = 0 "
                AccessHelper.ExecuteSqlWithoutTimeOut(strSQL)

                strSQL = "@delete# from sysroConcurrentLicenses where datetime <= Dateadd(Month, Datediff(Month, 0, DATEADD(m, -6, current_timestamp)), 0)"
                AccessHelper.ExecuteSqlWithoutTimeOut(strSQL)

                strSQL = " @INSERT# INTO sysroConcurrentLicenses (DateTime, Count, VTLive, VTSupervisor, VTPortal) " &
                                            " @Select# GetDate(), count(sp.ID), " &
                                                    " sum(CASE When sp.ApplicationName='VTLive' Then " & roConstants.APPLICATION_Desktop_License.ToString & " Else 0 End ) as VTLive, " &
                                                    " sum(CASE When sp.ApplicationName='VTSupervisor' Then " & roConstants.APPLICATION_Supervisor_License.ToString.Replace(",", ".") & " Else 0 End ) as VTSupervisor, " &
                                                    " sum(CASE When sp.ApplicationName='VTPortal' Then " & roConstants.APPLICATION_Supervisor_License.ToString.Replace(",", ".") & " Else 0 End ) as VTPortal " &
                                                    " From sysroPassports_Sessions sp WITH(NOLOCK) inner Join sysroPassports_Data spd WITH(NOLOCK) on sp.IDPassport = spd.IDPassport And spd.Id = sp.DataId " &
                                                    " Where sp.InvalidationCode = 0 And spd.IsSupervisor = 1 "

                AccessHelper.ExecuteSqlWithoutTimeOut(strSQL)

                strSQL = "@delete# from sysroPassports_DeviceTokens where registrationdate < Dateadd(DAY, -30, convert(date,GETDATE()))"
                AccessHelper.ExecuteSqlWithoutTimeOut(strSQL)
            Catch ex As Exception
                bolRet = False
                VTBase.roLog.GetInstance().logMessage(roLog.EventType.roError, "ConnectionAccess::UpdateSessions::Unhandled error::" & ex.Message, ex)
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Actualiza la información de la sessión a la tabla de sessiones.
        ''' </summary>
        ''' <param name="SessionID">Identificador único de sesión.</param>
        ''' <param name="PassportID">Código de pasaporte.</param>
        ''' <param name="xDate">Fecha de actualización.</param>
        ''' <param name="ClientLocation">Localización del cliente.</param>
        ''' <param name="trans"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function UpdateSession(ByVal oState As roBaseState, ByVal PassportID As Integer, ByVal xDate As DateTime, ByVal ClientLocation As String, ByRef sessionInvalidationCode As Integer, ByVal bolIsNewSession As Boolean, ByVal oMethod As AuthenticationMethod) As Boolean

            Dim bolRet As Boolean = False
            Dim strSQL As String
            Dim bUpdate As Boolean = True
            Dim bUpdateLastActionDate As Boolean = False
            Dim bSessionExists As Boolean = False

            'Revisamos si es una petición que deba actualizar el estado de sesión para poder mantener el estado correctamente
            Try
                If System.Web.HttpContext.Current IsNot Nothing Then
                    Dim curRequest As System.Web.HttpRequest = System.Web.HttpContext.Current.Request

                    Dim sPath As String = curRequest.Path.ToLower()
                    If sPath.Contains("getdocumentationfaultalerts") OrElse sPath.Contains("getdesktopalerts") OrElse sPath.Contains("getusertasks") OrElse
                            sPath.Contains("getschedulestatus") OrElse sPath.Contains("securitybasesvc") OrElse sPath.Contains("usertaskscheck") Then
                        bUpdate = False
                    End If
                End If
            Catch ex As Exception
            End Try

            Dim bNeedToCreateSession As Boolean = False
            'Tratamos diferente el caso del portal ya que pueden existir sesiones con invalidationCode = 3 (sesión caducada) y que estas se deban recuperar como válidas
            If oState.AppType = roAppType.VTPortal Then
                bSessionExists = roTypes.Any2Integer(AccessHelper.ExecuteScalar(" @SELECT# count(*) FROM sysroPassports_Sessions Where " &
                                        " InvalidationCode IN(0,3) AND IDPassport = " & PassportID & " AND ApplicationName = '" & oState.GetApplicationSourceName() & "' AND ID = '" & oState.SessionID & "'")) > 0

                If bSessionExists Then
                    'Borramos todos los registros de sesión para el pasaporte y la aplicación que haya en base de datos
                    strSQL = "@DELETE# FROM sysroPassports_Data WHERE Id IN " &
                                        "(@SELECT# DataId FROM sysroPassports_Sessions WHERE ID = '" & oState.SessionID & "' AND ApplicationName = '" & oState.GetApplicationSourceName() & "' AND IDPassport = " & PassportID.ToString & ")"
                    AccessHelper.ExecuteScalar(strSQL)

                    strSQL = "@DELETE# FROM sysroPassports_Sessions WHERE ID = '" & oState.SessionID & "' AND ApplicationName = '" & oState.GetApplicationSourceName() & "' AND IDPassport = " & PassportID.ToString
                    AccessHelper.ExecuteScalar(strSQL)

                    strSQL = "@DELETE# FROM sysroPassports_Data WHERE IdPassport = " & PassportID.ToString & " AND AppCode = 'VTPORTAL' and RecoverKey is not NULL"
                    AccessHelper.ExecuteScalar(strSQL)

                End If

                bNeedToCreateSession = True
            Else
                bSessionExists = roTypes.Any2Integer(AccessHelper.ExecuteScalar(" @SELECT# count(*) FROM sysroPassports_Sessions Where " &
                                        " IDPassport = " & PassportID & " AND ApplicationName = '" & oState.GetApplicationSourceName() & "' AND ID = '" & oState.SessionID & "'")) > 0

                sessionInvalidationCode = roTypes.Any2Integer(AccessHelper.ExecuteScalar(" @SELECT# InvalidationCode FROM sysroPassports_Sessions Where " &
                                            " IDPassport = " & PassportID & " AND ApplicationName = '" & oState.GetApplicationSourceName() & "' AND ID = '" & oState.SessionID & "'"))

                'Si no tengo una sesión activa o tengo una sesión invalidada, reviso si tengo otras sesiones para invalidarlas
                If bSessionExists AndAlso sessionInvalidationCode <= 0 Then
                    bUpdateLastActionDate = True
                    If bUpdate Then
                        sessionInvalidationCode = 0
                        strSQL = "@UPDATE# sysroPassports_Sessions SET UpdateDate = " & roTypes.Any2Time(xDate).SQLDateTime & " WHERE ID = '" & oState.SessionID & "' AND ApplicationName = '" & oState.GetApplicationSourceName() & "' AND IDPassport = " & PassportID.ToString & " AND InvalidationCode = 0"
                        AccessHelper.ExecuteSql(strSQL)
                    End If
                Else
                    If bolIsNewSession Then bNeedToCreateSession = True
                End If
            End If

            If bNeedToCreateSession Then
                bUpdateLastActionDate = True

                ' Marcamos las conexiones de otros usuarios con mi mismo id de sesión como inválidas en la tabla
                ' Inicio de sesión con otro usuario en la misma sesión
                strSQL = "@UPDATE# sysroPassports_Sessions SET InvalidationCode = 4 " &
                         "WHERE ID = '" & oState.SessionID & "' AND ApplicationName = '" & oState.GetApplicationSourceName() & "' AND IDPassport <> " & PassportID.ToString & " AND InvalidationCode = 0"

                AccessHelper.ExecuteSql(strSQL)

                ' Marcamos las conexiones de mi usuario y otro id de sesión como inválida en la tabla
                ' Inicio de sesión desde otra localización
                strSQL = "@UPDATE# sysroPassports_Sessions SET InvalidationCode = 2 " &
                         "WHERE IDPassport = " & PassportID.ToString & " AND ID <> '" & oState.SessionID & "' AND ApplicationName = '" & oState.GetApplicationSourceName() & "' AND InvalidationCode = 0"
                AccessHelper.ExecuteSql(strSQL)

                sessionInvalidationCode = 0
                Dim sPassportDataId As String = Guid.NewGuid.ToString
                'Si me estan creando una sesión, inserto el nuevo registro
                strSQL = "@INSERT# INTO sysroPassports_Sessions(ID,IDPassport,StartDate,UpdateDate,ClientLocation,ApplicationName,InvalidationCode,DataId) VALUES " &
                                 "('" & oState.SessionID & "'," & PassportID & "," & Robotics.VTBase.roTypes.Any2Time(xDate).SQLDateTime & "," & Robotics.VTBase.roTypes.Any2Time(xDate).SQLDateTime & ",'" & ClientLocation & "','" & oState.GetApplicationSourceName() & "',0,'" & sPassportDataId & "')"
                Dim resultadoInsert = AccessHelper.ExecuteSql(strSQL)
                ' Actualizamos datos de concurrencia

                If resultadoInsert Then
                    strSQL = "IF EXISTS(@SELECT# 1 from sysroPassports_Data where IDPassport = " & PassportID & " AND AppCode='" & oState.GetApplicationSourceName() & "')" &
                                    " BEGIN" &
                                    " @UPDATE# sysroPassports_Data SET AuthToken = '', SecurityToken = '', SessionContext='', IsSupervisor=0, Id='" & sPassportDataId & "' WHERE IDPassport = " & PassportID & " AND AppCode='" & oState.GetApplicationSourceName() & "'" &
                                    " END" &
                                    " ELSE" &
                                    " BEGIN " &
                                    " @INSERT# INTO sysroPassports_Data (IDPassport, AuthToken, SecurityToken, SessionContext, IsSupervisor, AppCode, Id, KeyValidated) VALUES (" & PassportID & ",'','','',0,'" & oState.GetApplicationSourceName() & "','" & sPassportDataId & "',1)" &
                                    " END"

                    AccessHelper.ExecuteSql(strSQL)
                End If
            Else
                bSessionExists = roTypes.Any2Integer(AccessHelper.ExecuteScalar(" @SELECT# count(*) FROM sysroPassports_Sessions WITH(NOLOCK) Where " &
                                            " IDPassport = " & PassportID & " AND ApplicationName = '" & oState.GetApplicationSourceName() & "' AND ID = '" & oState.SessionID & "'")) > 0

                If bSessionExists Then
                    sessionInvalidationCode = roTypes.Any2Integer(AccessHelper.ExecuteScalar(" @SELECT# InvalidationCode FROM sysroPassports_Sessions WITH(NOLOCK) Where " &
                                        " IDPassport = " & PassportID & " AND ApplicationName = '" & oState.GetApplicationSourceName() & "' AND ID = '" & oState.SessionID & "'"))
                Else
                    sessionInvalidationCode = -1
                End If
            End If

            If sessionInvalidationCode > 0 AndAlso sessionInvalidationCode <> 3 Then
                If sessionInvalidationCode > 1 Then
                    strSQL = "@DELETE# FROM sysroPassports_Data WHERE Id = " &
                                        "(@SELECT# top 1 DataId FROM sysroPassports_Sessions WITH(NOLOCK) WHERE ID = '" & oState.SessionID & "' AND ApplicationName = '" & oState.GetApplicationSourceName() & "' AND IDPassport = " & PassportID.ToString & " AND InvalidationCode=" & sessionInvalidationCode & ")"
                    AccessHelper.ExecuteScalar(strSQL)

                    ' Si no es un cambio de permisos borramos la sesión
                    strSQL = "@DELETE# FROM sysroPassports_Sessions WHERE ID = '" & oState.SessionID & "' AND ApplicationName = '" & oState.GetApplicationSourceName() & "' AND IDPassport = " & PassportID.ToString & " AND InvalidationCode=" & sessionInvalidationCode
                    AccessHelper.ExecuteScalar(strSQL)
                Else
                    ' Si es un cambio de permisos la marcamos como válida y mantenemos el estado a cambio de permisos para que borre la cache si es necesario
                    strSQL = "@UPDATE# sysroPassports_Sessions SET InvalidationCode = 0 WHERE ID = '" & oState.SessionID & "' AND ApplicationName = '" & oState.GetApplicationSourceName() & "' AND IDPassport = " & PassportID.ToString & " AND InvalidationCode=" & sessionInvalidationCode
                    AccessHelper.ExecuteScalar(strSQL)
                End If

            End If

            If bUpdateLastActionDate Then
                strSQL = "@UPDATE# sysroPassports_AuthenticationMethods SET LastAppActionDate = " & roTypes.Any2Time(xDate.Date).SQLDateTime & ", BlockedAccessByInactivity = 0 WHERE Method = " & CInt(oMethod) & " AND IDPassport = " & PassportID.ToString
                AccessHelper.ExecuteSql(strSQL)
            End If

            bolRet = True

            Return bolRet

        End Function

        Public Shared Function GetCurrentLoggedUsers(ByRef oState As roSecurityState) As UserInfo()

            Dim users As New Generic.List(Of UserInfo)
            Dim tb As DataTable

            Try

                Dim strSQL As String = "@SELECT# Name, Description, s.ClientLocation, s.ApplicationName, s.IDPassport from sysroPassports p inner join sysroPassports_Sessions s on p.ID= s.IDPassport where InvalidationCode = 0"

                tb = AccessHelper.CreateDataTable(strSQL)

                If tb IsNot Nothing Then
                    For Each user As DataRow In tb.Rows

                        users.Add(New UserInfo With {
                                     .Name = user("Name"),
                                     .Description = user("Description"),
                                     .ClientLocation = user("ClientLocation"),
                                     .ApplicationName = user("ApplicationName"),
                                     .UserId = user("IDPassport")})

                    Next
                End If
            Catch e As Exception
                tb = Nothing
                users = New Generic.List(Of UserInfo)
                oState.Result = SecurityResultEnum.Exception
            Finally

            End Try

            Return users.ToArray
        End Function


        Public Shared Function GetConcurrencyInfo(ByRef oState As roSecurityState) As ConcurrencyInfo()

            Dim concurrencyInfo As New Generic.List(Of ConcurrencyInfo)
            Dim tb As DataTable

            Try

                Dim strSQL As String = "  @SELECT# * from sysroConcurrentLicenses where DateTime >= DATEADD(MONTH, -2, GETDATE())  "

                tb = AccessHelper.CreateDataTable(strSQL)

                If tb IsNot Nothing Then
                    For Each info As DataRow In tb.Rows

                        concurrencyInfo.Add(New ConcurrencyInfo With {
                                     .Datetime = info("DateTime"),
                                     .Count = info("Count"),
                                     .RealCount = 0,
                                     .VTLive = If(info("VTLive") IsNot DBNull.Value, info("VTLive"), 0),
                                     .VTPortal = If(info("VTPortal") IsNot DBNull.Value, info("VTPortal"), 0),
                                     .VTSupervisor = If(info("VTSupervisor") IsNot DBNull.Value, info("VTSupervisor"), 0),
                                     .Max = If(info("Status") IsNot DBNull.Value, 1, 0)})

                    Next

                    For Each concurrency As ConcurrencyInfo In concurrencyInfo

                        concurrency.RealCount = concurrency.VTLive + concurrency.VTSupervisor
                        'concurrency.RealCount = concurrency.VTLive + concurrency.VTSupervisor + concurrency.VTPortal
                    Next

                End If
            Catch e As Exception
                tb = Nothing
                concurrencyInfo = New Generic.List(Of ConcurrencyInfo)
                oState.Result = SecurityResultEnum.Exception
            Finally

            End Try

            Return concurrencyInfo.ToArray
        End Function

        Public Shared Function UpdateConnectionHistory(ByVal oState As roBaseState, ByVal UserName As String, ByVal EventType As Audit.Action) As Boolean

            Dim bolRet As Boolean = True
            Dim strSQL As String

            'Auditamos los login/logout a VTLive y Portal
            Try

                strSQL = " @INSERT# INTO sysroConnectionHistory (UserName, Application, EventDateTime, EventType) VALUES ('" &
                                            UserName.Replace("'", "''") & "'," & oState.AppType & ", getdate()," & EventType & ")"

                bolRet = AccessHelper.ExecuteSql(strSQL)

            Catch ex As Exception
                bolRet = False
                VTBase.roLog.GetInstance().logMessage(roLog.EventType.roError, "ConnectionAccess::UpdateConnectionHistory::Unhandled error::" & ex.Message, ex)
            End Try
            Return bolRet

        End Function

    End Class

End Namespace
