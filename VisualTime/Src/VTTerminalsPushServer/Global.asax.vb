Imports System.Runtime.Caching
Imports System.Security.Cryptography.X509Certificates
Imports System.Web.Http
Imports Robotics
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Terminal
Imports Robotics.VTBase
Imports Robotics.Web.Base

Public Class WebApiApplication
    Inherits roBaseGlobalAsax


    Private Shared _sessionsCache As MemoryCache = MemoryCache.Default
    Private Shared _htSession As New Hashtable

    Private Shared _lastConnectionStatusChecked As DateTime = DateTime.MinValue
    Private Shared _updateSessionExculdeMethods = {"GetDocumentationFaultAlerts", "RecoverPassword", "ResetPasswordToNew"}

#Region "Properties"

    Public Shared ReadOnly Property ApplicationSource As roAppSource
        Get
            Dim roAuth As String = If(HttpContext.Current.Request.Headers("roAuth") IsNot Nothing, HttpContext.Current.Request.Headers("roAuth"), "")

            Dim oApiSession As roApiSession = ObtainSession(roAuth)
            If oApiSession IsNot Nothing Then
                Return oApiSession.ApplicationSource
            End If

            Return Nothing
        End Get
    End Property

    Public Shared ReadOnly Property TerminalSecurityToken As String
        Get
            Dim roAuth As String = If(HttpContext.Current.Request.Headers("roAuth") IsNot Nothing, HttpContext.Current.Request.Headers("roAuth"), "")

            Dim oApiSession As roApiSession = ObtainSession(roAuth)
            If oApiSession IsNot Nothing Then
                Return oApiSession.TerminalSecurityToken
            End If

            Return Nothing
        End Get
    End Property

    Public Shared ReadOnly Property TerminalIdentity As Terminal.roTerminal
        Get

            If HttpContext.Current.Request IsNot Nothing AndAlso HttpContext.Current.Request.Path.ToLower().Contains("iclock") Then
                Dim roAuth As String = If(HttpContext.Current.Request.Params("SN") IsNot Nothing, HttpContext.Current.Request.Params("SN"), "")

                Dim oApiSession As roTerminalApiSession = ObtainRxSession(roAuth)
                If oApiSession IsNot Nothing Then
                    Return oApiSession.Terminal
                End If
            Else
                Dim roAuth As String = If(HttpContext.Current.Request.Headers("roAuth") IsNot Nothing, HttpContext.Current.Request.Headers("roAuth"), "")

                Try
                    Dim oApiSession As roApiSession = ObtainSession(roAuth)
                    If oApiSession IsNot Nothing Then
                        If oApiSession.IdTerminal = -1 Then
                            Return Nothing
                        Else
                            Return CType(oApiSession.Terminal, Terminal.roTerminal)
                        End If
                    End If
                Catch ex As Exception
                    roLog.GetInstance().logMessage(roLog.EventType.roError, "GlobalAsax::TerminalIdentity::Could not obtain TerminalIdentity", ex)
                    Return Nothing
                End Try
            End If

            Return Nothing
        End Get
    End Property

    Public Shared ReadOnly Property TerminalLogicIdentity As Object
        Get
            Dim roAuth As String = If(HttpContext.Current.Request.Params("SN") IsNot Nothing, HttpContext.Current.Request.Params("SN"), "")

            Dim oApiSession As roTerminalApiSession = ObtainRxSession(roAuth)
            If oApiSession IsNot Nothing Then
                Return oApiSession.TerminalLogic
            End If

            Return Nothing
        End Get
    End Property

    Public Shared Function IsLoggedIn(oApiSession As roTerminalApiSession) As Boolean
        Try
            ' Tengo identificado el cliente
            If roTypes.Any2String(oApiSession.CompanyId).Length = 0 Then Return False
            If (roTypes.Any2String(roConstants.GetGlobalEnvironmentParameter(GlobalAsaxParameter.CompanyId)).Length = 0) Then Return False

            ' Tengo definido el terminal y sus lectores
            If oApiSession.Terminal Is Nothing OrElse oApiSession.Terminal.Readers Is Nothing OrElse oApiSession.Terminal.Readers.Count = 0 OrElse oApiSession.Terminal.ReaderByIndex(0).Mode.Length = 0 OrElse Not oApiSession.Terminal.Enabled Then
                Return False
            Else
                Return True
            End If
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Shared ReadOnly Property LoggedIn As Boolean
        Get
            If HttpContext.Current.Request IsNot Nothing AndAlso HttpContext.Current.Request.Path.ToLower().Contains("iclock") Then
                Dim roAuth As String = If(HttpContext.Current.Request.Params("SN") IsNot Nothing, HttpContext.Current.Request.Params("SN"), "")

                Dim oApiSession As roTerminalApiSession = ObtainRxSession(roAuth)
                If oApiSession IsNot Nothing Then
                    Return IsLoggedIn(oApiSession)
                Else
                    Return False
                End If

            Else
                Dim roAuth As String = If(HttpContext.Current.Request.Headers("roAuth") IsNot Nothing, HttpContext.Current.Request.Headers("roAuth"), "")

                Dim oApiSession As roApiSession = ObtainSession(roAuth)
                If oApiSession IsNot Nothing Then
                    Return oApiSession.LoggedIn AndAlso WebApiApplication.SecurityResult <> SecurityResultEnum.MaxCurrentSessionsExceeded
                End If
            End If

            Return False

        End Get
    End Property

    Public Shared ReadOnly Property SecurityResult As SecurityResultEnum
        Get
            Dim roAuth As String = If(HttpContext.Current.Request.Headers("roAuth") IsNot Nothing, HttpContext.Current.Request.Headers("roAuth"), "")

            Dim oApiSession As roApiSession = ObtainSession(roAuth)
            If oApiSession IsNot Nothing Then
                Return oApiSession.SecurityResult
            End If

            Return SecurityResultEnum.NoError
        End Get
    End Property

    Public Shared ReadOnly Property TerminalResult As TerminalBaseResultEnum
        Get
            Dim roAuth As String = If(HttpContext.Current.Request.Headers("roAuth") IsNot Nothing, HttpContext.Current.Request.Headers("roAuth"), "")

            Dim oApiSession As roApiSession = ObtainSession(roAuth)
            If oApiSession IsNot Nothing Then
                Return oApiSession.TerminalResult
            End If

            Return TerminalBaseResultEnum.NoError
        End Get
    End Property

#End Region

#Region "Base"
    Protected Overrides Function GetLoggedInPassportFromSession() As roPassportTicket
        Return Nothing
    End Function

    Protected Overrides Function GetLoggedInPassportIdFromSession() As Integer
        Return -1
    End Function
#End Region

    Protected Sub Application_Start()
        Robotics.DataLayer.AccessHelper.InitializeSharedInstanceData(roAppType.TerminalsPushServer, roLiveQueueTypes.terminals)

        AreaRegistration.RegisterAllAreas()
        GlobalConfiguration.Configure(AddressOf WebApiConfig.Register)
        FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters)
        RouteConfig.RegisterRoutes(RouteTable.Routes)
    End Sub

    Protected Sub Application_AcquireRequestState(sender As Object, e As EventArgs)
        If Request IsNot Nothing AndAlso (Request.Path.ToLower().Contains("iclock") OrElse Request.Path.ToLower() = "/") Then BeginRxRequest(sender, e)

        If Request IsNot Nothing AndAlso Request.Path.ToLower().Contains("terminalsvcx.svc") Then BeginRequestMX9(sender, e)
    End Sub

    Sub Application_PreSendRequestHeaders()
        Response.Headers.Remove("Server")
        Response.Headers.Remove("X-AspNet-Version")
        Response.Headers.Remove("X-AspNetMvc-Version")
    End Sub

    Sub Application_EndRequest(ByVal sender As Object, ByVal e As EventArgs)
        Me.OnApplicationEndRequest()
    End Sub

    Protected Sub BeginRxRequest(sender As Object, e As EventArgs)

        Dim SN As String = HttpContext.Current.Request.Params("SN")
        Dim strIP As String = HttpContext.Current.Request.Params("REMOTE_ADDR")
        Dim strPort As String = HttpContext.Current.Request.Params("SERVER_PORT")
        Dim oApiSession As roTerminalApiSession = Nothing

        Try
            If SN IsNot Nothing AndAlso SN <> String.Empty Then
                ' Recupero datos del terminal del caché
                oApiSession = ObtainRxSession(SN)

                If oApiSession Is Nothing Then
                    oApiSession = New roTerminalApiSession With {
                        .SN = SN,
                        .LastRequest = DateTime.Now
                    }
                End If

                ' Si el terminal no está identificado con el cliente al que pertenece, lo hago ahora
                If (oApiSession.CompanyId Is Nothing OrElse oApiSession.CompanyId.Trim = "") Then
                    Dim oTerminalConfiguration As roTerminalRegister = DataLayer.roCacheManager.GetInstance().GetTerminal(SN)

                    If oTerminalConfiguration IsNot Nothing AndAlso oTerminalConfiguration.model.Length > 0 AndAlso oTerminalConfiguration.companyname.Length > 0 AndAlso oTerminalConfiguration.enabled Then
                        oApiSession.CompanyId = oTerminalConfiguration.companyname
                        oApiSession.Model = [Enum].Parse(GetType(roTerminalApiSession.roTerminalModel), oTerminalConfiguration.model.ToString)
                    Else
                        roLog.GetInstance().logMessage(roLog.EventType.roWarning, $"TerminalPushServer::Terminal Not Found On CosmosDB::SN:: {SN}. Register to a Company, or register to a Unknown Company and disable it to avoid Cosmos requests")
                        HttpContext.Current.Session("roClientCompanyId") = String.Empty
                        ' No se encontró la empresa. Salgo
                        HttpContext.Current.Items("SkipRequest") = True
                        Return
                    End If
                End If

                HttpContext.Current.Session("roClientCompanyId") = oApiSession.CompanyId

                Me.OnApplicationReloadSharedData()

                roTrace.GetInstance().AddTerminalInfo(oApiSession.SN, oApiSession.Model.ToString, oApiSession.CompanyId)

                ' Borro lógicas de terminales desconectados ...
                PurgeRxSessionData(SN, oApiSession.CompanyId)

                ' Si no tengo la lógica en la caché, la incorporo
                If oApiSession.TerminalLogic Is Nothing Then
                    ' Miramos si el estado de registro del terminal
                    Dim oTermState As New roTerminalState(-1)
                    Dim regsitrationState As TerminalRegitrationState = roTerminal.GetRegistratrionState(SN, oTermState)
                    ' Si el terminal existe pero no está configurado, no inicializo lógica dado que no tiene sentido (luego no se van a guardar en la sesión (IsLoggedIn = False)) 
                    If regsitrationState <> TerminalRegitrationState.ExistsButNotConfigured Then
                        Dim oTerminalLogic As Object
                        Select Case oApiSession.Model
                            Case roTerminalApiSession.roTerminalModel.mxS
                                oTerminalLogic = New Comms.DrivermxS.BusinesProtocol.TerminalLogicMxS(oApiSession.Terminal)
                            Case Else
                                oTerminalLogic = New Comms.DriverZKPush2.BusinesProtocol.TerminalLogicZKPush2(oApiSession.Terminal)
                        End Select

                        If oTerminalLogic.Initialize(SN, strIP, strPort, oApiSession.Model.ToString.ToUpper) Then
                            oApiSession.TerminalLogic = oTerminalLogic
                            If oApiSession.Terminal Is Nothing Then
                                oApiSession.Terminal = oTerminalLogic.mTerminal.DBTerminal
                            End If
                        End If
                    End If
                Else
                    ' Refreco la lógica
                    oApiSession.TerminalLogic.CheckConnectionStatusOnIIS()
                    ' Refresco el terminal
                    oApiSession.Terminal = oApiSession.TerminalLogic.mTerminal.DBTerminal
                End If

                If IsLoggedIn(oApiSession) Then
                    oApiSession.LastRequest = DateTime.Now
                    SaveRxSessionData(SN, oApiSession)
                Else
                    RemoveRxSessionData(SN)
                    HttpContext.Current.Items("SkipRequest") = True
                End If
            Else
                HttpContext.Current.Items("SkipRequest") = True
            End If
        Catch ex As DataLayer.ConnectionStringException
            roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalPushServer::Error::EmptyConnectionString::SN::" & SN, ex)

            HttpContext.Current.Items("SkipRequest") = True
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalPushServer::Error::UnknownException::" & ex.Message)

            HttpContext.Current.Items("SkipRequest") = True
        End Try

    End Sub

    Sub BeginRequestMX9(ByVal sender As Object, ByVal e As EventArgs)
        Robotics.Web.Base.CorsSupport.HandlePreflightRequest(HttpContext.Current)

        If HttpContext.Current.Request.Url.PathAndQuery.Contains("KeepAlive") Then
            Return
        End If

        Try
            Dim roAuth As String = If(HttpContext.Current.Request.Headers("roAuth") IsNot Nothing, HttpContext.Current.Request.Headers("roAuth"), "")
            Dim roApp As String = If(HttpContext.Current.Request.Headers("roApp") IsNot Nothing, HttpContext.Current.Request.Headers("roApp"), "")
            If roAuth = String.Empty Then Return

            Me.OnApplicationReloadSharedData()

            Dim oApiSession As roApiSession = ObtainSession(roAuth)
            Select Case roApp.ToUpper
                Case roAppSource.mx9.ToString.ToUpper
                    oApiSession.ApplicationSource = roAppSource.mx9
                    oApiSession = Robotics.Web.Base.ApiSession.PrepareTerminalSession(roAuth, oApiSession, _updateSessionExculdeMethods) 'Me.LogEnabled
                Case Else
                    oApiSession.ApplicationSource = roAppSource.VTLive
                    oApiSession = Robotics.Web.Base.ApiSession.GetNewApiSession("")
            End Select

            SaveSessionData(oApiSession)
        Catch ex As Exception
            roLog.GetInstance.logMessage(roLog.EventType.roError, "roliveApi::GlobalASAX::Application_BeginRequest::" & HttpContext.Current.Request.Url.AbsolutePath & "::" & ex.Message, ex)
        End Try

    End Sub

#Region "Session Management"

    Private Shared Sub SaveSessionData(oApiSession As roApiSession)
        Robotics.DataLayer.roCacheManager.GetInstance.UpdateVTLiveApiSession(oApiSession)
    End Sub

    Private Shared Function ObtainSession(roAuth As String) As roApiSession
        Dim oApiSession As roApiSession = Robotics.DataLayer.roCacheManager.GetInstance.GetVTLiveApiSession(roAuth)
        If oApiSession Is Nothing Then oApiSession = Robotics.Web.Base.ApiSession.GetNewApiSession(roAuth)

        Return oApiSession
    End Function


    Private Shared Function ObtainRxSession(terminalSN As String) As roTerminalApiSession
        Dim sSessionMode As String = VTBase.roConstants.GetConfigurationParameter("SessionMode")

        If sSessionMode <> String.Empty AndAlso roTypes.Any2Integer(sSessionMode) = 1 Then
            If _htSession.Contains(terminalSN) Then
                Return CType(_htSession(terminalSN), roTerminalApiSession)
            End If
            Return Nothing
        Else
            If _sessionsCache.Contains(terminalSN) Then
                Return CType(_sessionsCache(terminalSN), roTerminalApiSession)
            End If
            Return Nothing
        End If

    End Function

    Private Shared Sub SaveRxSessionData(terminalSN As String, oApiSession As roTerminalApiSession)
        Dim sSessionMode As String = VTBase.roConstants.GetConfigurationParameter("SessionMode")

        If sSessionMode <> String.Empty AndAlso roTypes.Any2Integer(sSessionMode) = 1 Then
            If _htSession.Contains(terminalSN) Then
                _htSession(terminalSN) = oApiSession
            Else
                _htSession.Add(terminalSN, oApiSession)
            End If
        Else
            _sessionsCache.Set(terminalSN, oApiSession, DateTimeOffset.Now.AddMinutes(60))
        End If

    End Sub

    Private Shared Sub RemoveRxSessionData(terminalSN As String)
        Dim sSessionMode As String = VTBase.roConstants.GetConfigurationParameter("SessionMode")

        If sSessionMode <> String.Empty AndAlso roTypes.Any2Integer(sSessionMode) = 1 Then
            SyncLock _htSession
                If _htSession.Contains(terminalSN) Then _htSession.Remove(terminalSN)
            End SyncLock
        End If

    End Sub

    Private Shared Sub PurgeRxSessionData(terminalSN As String, oldCompanyId As String)
        ' Only proceed if using hashtable session mode
        Dim sSessionMode As String = VTBase.roConstants.GetConfigurationParameter("SessionMode")
        If sSessionMode = String.Empty OrElse roTypes.Any2Integer(sSessionMode) <> 1 Then Return

        ' Only check connections every 15 minutes
        If _lastConnectionStatusChecked <> Date.MinValue AndAlso Now.Subtract(_lastConnectionStatusChecked).TotalMinutes <= 15 Then Return

        _lastConnectionStatusChecked = Date.Now

        Try
            ' Find expired sessions (inactive for more than 5 minutes)
            Dim keysToRemove As New List(Of String)
            For Each entry As DictionaryEntry In _htSession
                Dim key As String = CStr(entry.Key)
                If key <> terminalSN Then
                    Dim session As roTerminalApiSession = DirectCast(entry.Value, roTerminalApiSession)
                    If Now.Subtract(session.LastRequest).TotalMinutes > 5 Then
                        keysToRemove.Add(key)
                    End If
                End If
            Next

            ' Remove expired sessions
            If keysToRemove.Count > 0 Then
                SyncLock _htSession
                    For Each key As String In keysToRemove
                        _htSession.Remove(key)
                    Next
                End SyncLock
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError,
                "TerminalPushServer::Error::PurgeSessions::" & ex.Message)
            HttpContext.Current.Session("roClientCompanyId") = oldCompanyId
        End Try
    End Sub
#End Region
End Class