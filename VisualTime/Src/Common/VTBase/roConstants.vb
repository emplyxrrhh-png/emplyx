Imports System.Configuration
Imports System.Threading
Imports System.Web.Compilation
Imports Microsoft.VisualBasic.ApplicationServices
Imports Robotics.Base.DTOs

Public Class roConstants
    Public Const APPLICATION_Desktop_License = 1
    Public Const APPLICATION_Supervisor_License = 0.25

    Public Const DXAnalityc = 1
    Public Const GeniusAnalytic = 2

    Public Const roNullDate As String = "1/1/2079"

    Public Shared Function GetGeniusCodeFromEnum(ByVal eType As GeniusTypeEnum) As String
        Select Case eType
            Case GeniusTypeEnum._NOTDEFINDED : Return "0"
            Case GeniusTypeEnum._COSTCENTERS : Return "B"
            Case GeniusTypeEnum._PRODUCTIV : Return "T"
            Case GeniusTypeEnum._ACCESS : Return "A"
            Case GeniusTypeEnum._SCHEDULER : Return "C"
            Case GeniusTypeEnum._ACCRUALS : Return "S"
            Case GeniusTypeEnum._EQUALITYPLAN : Return "E"
            Case GeniusTypeEnum._REQUESTS : Return "R"
            Case GeniusTypeEnum._PUNCHES : Return "P"
            Case GeniusTypeEnum._USERS : Return "U"
            Case Else : Return "0"
        End Select
    End Function

    Public Shared Function GetGeniusEnumFromCode(ByVal code As String) As GeniusTypeEnum
        Select Case code
            Case "0" : Return GeniusTypeEnum._NOTDEFINDED
            Case "B" : Return GeniusTypeEnum._COSTCENTERS
            Case "T" : Return GeniusTypeEnum._PRODUCTIV
            Case "A" : Return GeniusTypeEnum._ACCESS
            Case "S" : Return GeniusTypeEnum._ACCRUALS
            Case "U" : Return GeniusTypeEnum._USERS
            Case "R" : Return GeniusTypeEnum._REQUESTS
            Case "P" : Return GeniusTypeEnum._PUNCHES
            Case "C" : Return GeniusTypeEnum._SCHEDULER
            Case "E" : Return GeniusTypeEnum._EQUALITYPLAN
            Case Else : Return GeniusTypeEnum._NOTDEFINDED
        End Select
    End Function


    Public Shared Function GetAppTypeFromSession(ByVal oState As roBaseState) As roAppType
        Dim strApplicationName As String = ""
        If oState.ClientAddress.Split("#").Length > 1 Then strApplicationName = oState.ClientAddress.Split("#")(1)

        Dim result As roAppType = roAppType.Unknown
        If [Enum].TryParse(strApplicationName, True, result) Then
            Return result
        Else
            Return roAppType.Unknown
        End If

    End Function

    Public Shared Function GetCurrentAppType() As roAppType
        Dim strApplicationName As String = ""
        If IsMultitenantServiceEnabled() Then
            strApplicationName = roTypes.Any2String(Threading.Thread.GetDomain().GetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_AppName"))
        Else
            strApplicationName = roTypes.Any2String(GetGlobalEnvironmentParameter(GlobalAsaxParameter.AppName))
        End If

        Dim result As roAppType = roAppType.Unknown
        If [Enum].TryParse(strApplicationName, True, result) Then
            Return result
        Else
            Return roAppType.Unknown
        End If

    End Function

    Public Shared Function GetDefaultSourceForType(appType As roAppType) As roAppSource
        Dim defaultSource As roAppSource = roAppSource.unknown


        Select Case appType
            Case roAppType.VTLive
                defaultSource = roAppSource.VTLive
            Case roAppType.VTLiveApi
                defaultSource = roAppSource.VTLiveApi
            Case roAppType.VTPortal
                defaultSource = roAppSource.VTPortal
            Case roAppType.VTVisits
                defaultSource = roAppSource.Visits
            Case roAppType.TerminalsPushServer
                defaultSource = roAppSource.TerminalsPushServer
            Case Else
                defaultSource = roAppSource.unknown
        End Select

        Return defaultSource

    End Function


    Public Shared Function GetDefaultLogLevel() As roLog.EventType
        Dim oLogLevel As roLog.EventType = roLog.EventType.roDebug

        Try
            If roConstants.IsMultitenantServiceEnabled() Then
                If Threading.Thread.GetDomain().GetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_DefaultLogLevel") IsNot Nothing Then
                    oLogLevel = roTypes.Any2Integer(Threading.Thread.GetDomain().GetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_DefaultLogLevel"))
                Else
                    oLogLevel = roLog.EventType.roDebug
                End If
            Else
                Dim webAppParam As Object = GetGlobalEnvironmentParameter(GlobalAsaxParameter.LogLevel)
                If webAppParam IsNot Nothing Then oLogLevel = roTypes.Any2Integer(webAppParam)
            End If
        Catch ex As Exception
            oLogLevel = roLog.EventType.roDebug
        End Try

        Return oLogLevel
    End Function


    Public Shared Function GetDefaultTraceLevel() As roTrace.TraceType
        Dim oTraceLevel As roTrace.TraceType = roTrace.TraceType.roDebug

        Try
            If roConstants.IsMultitenantServiceEnabled() Then
                If Threading.Thread.GetDomain().GetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_DefaultTraceLevel") IsNot Nothing Then
                    oTraceLevel = roTypes.Any2Integer(Threading.Thread.GetDomain().GetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_DefaultTraceLevel"))
                Else
                    oTraceLevel = roTrace.TraceType.roDebug
                End If
            Else
                Dim webAppParam As Object = GetGlobalEnvironmentParameter(GlobalAsaxParameter.TraceLevel)
                If webAppParam IsNot Nothing Then oTraceLevel = roTypes.Any2Integer(webAppParam)
            End If
        Catch ex As Exception
            oTraceLevel = roTrace.TraceType.roDebug
        End Try

        Return oTraceLevel
    End Function


    Public Shared Sub SetDefaultCompanyTraceAndLogLevel(companyLogLevel As String, companyTraceLevel As String)
        Try
            If roConstants.IsMultitenantServiceEnabled() Then
                If Not String.IsNullOrEmpty(companyLogLevel) Then
                    Dim iDebugUntil As DateTime = roTypes.Any2DateTime(companyLogLevel)
                    If iDebugUntil.Date >= DateTime.Now.Date Then Threading.Thread.GetDomain().SetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_DefaultLogLevel", CInt(roLog.EventType.roDebug))
                End If

                If Not String.IsNullOrEmpty(companyTraceLevel) Then
                    Dim iDebugUntil As DateTime = roTypes.Any2DateTime(companyTraceLevel)
                    If iDebugUntil.Date >= DateTime.Now.Date Then Threading.Thread.GetDomain().SetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_DefaultTraceLevel", CInt(roTrace.TraceType.roDebug))
                End If
            Else
                '2029/01/01'
                If Not String.IsNullOrEmpty(companyLogLevel) Then
                    Dim iDebugUntil As DateTime = roTypes.Any2DateTime(companyLogLevel)
                    If iDebugUntil.Date >= DateTime.Now.Date Then SetGlobalEnvironmentParameter(GlobalAsaxParameter.LogLevel, CInt(roLog.EventType.roDebug))
                End If

                If Not String.IsNullOrEmpty(companyTraceLevel) Then
                    Dim iDebugUntil As DateTime = roTypes.Any2DateTime(companyTraceLevel)
                    If iDebugUntil.Date >= DateTime.Now.Date Then SetGlobalEnvironmentParameter(GlobalAsaxParameter.TraceLevel, CInt(roTrace.TraceType.roDebug))
                End If
            End If
        Catch ex As Exception
            'do nothing
        End Try

    End Sub

    ''' <summary>
    ''' Función para recuperar parámetros de configiración desde clientes web
    ''' </summary>
    ''' <param name="eParamater"></param>
    ''' <returns></returns>
    Public Shared Function GetGlobalEnvironmentParameter(ByVal eParamater As GlobalAsaxParameter) As Object
        Dim oRet As Object = Nothing

        Try

            If (System.Web.HttpContext.Current IsNot Nothing) Then
                Dim oWebApp As Web.HttpApplication = System.Web.HttpContext.Current.ApplicationInstance
                Dim propertyInfo As System.Reflection.PropertyInfo = oWebApp.GetType().GetProperty(eParamater.ToString)
                If propertyInfo IsNot Nothing Then
                    oRet = propertyInfo.GetValue(oWebApp)
                End If
            End If
        Catch ex As Exception
            oRet = Nothing
        End Try

        Return oRet
    End Function

    Public Shared Function SetGlobalEnvironmentParameter(ByVal eParamater As GlobalAsaxParameter, value As Object) As Object
        Dim oRet As Object = Nothing

        Try

            If (System.Web.HttpContext.Current IsNot Nothing) Then
                Dim oWebApp As Web.HttpApplication = System.Web.HttpContext.Current.ApplicationInstance
                Dim propertyInfo As System.Reflection.PropertyInfo = oWebApp.GetType().GetProperty(eParamater.ToString)
                If propertyInfo IsNot Nothing AndAlso propertyInfo.CanWrite Then
                    propertyInfo.SetValue(oWebApp, value)
                End If
            End If
        Catch ex As Exception
            oRet = Nothing
        End Try

        Return oRet
    End Function



    Public Shared Function GetSystemUserId() As Integer
        Dim idPassport As Integer

        Dim tmpObject = VTBase.roConstants.GetGlobalEnvironmentParameter(GlobalAsaxParameter.SystemPassportID)

        If tmpObject IsNot Nothing AndAlso roTypes.Any2Integer(tmpObject) > 0 Then
            idPassport = VTBase.roTypes.Any2Integer(tmpObject)
        Else

            tmpObject = Thread.GetDomain().GetData(Thread.CurrentThread.ManagedThreadId.ToString() & "_" & GlobalAsaxParameter.SystemPassportID.ToString())
            If tmpObject IsNot Nothing Then
                idPassport = roTypes.Any2Integer(tmpObject)
            Else
                idPassport = -1
            End If
        End If

        Return idPassport
    End Function

    Public Shared Function PushServerClassicLogs() As Boolean
        Dim bPushServerClassicLogs As Boolean = True

        Try
            bPushServerClassicLogs = VTBase.roTypes.Any2Boolean(VTBase.roConstants.GetConfigurationParameter("VTLive.PushServer.ClassicLogs"))
        Catch ex As Exception
            bPushServerClassicLogs = True
        End Try

        Return bPushServerClassicLogs
    End Function

    Public Shared Function GetConfigurationParameter(ByVal confKey As String) As String
        Dim value As String = Nothing

        Try
            If ConfigurationManager.AppSettings.Get(confKey) IsNot Nothing AndAlso ConfigurationManager.AppSettings.Get(confKey) <> String.Empty Then
                value = roTypes.Any2String(ConfigurationManager.AppSettings.Get(confKey))
            ElseIf Environment.GetEnvironmentVariable(confKey) IsNot Nothing AndAlso Environment.GetEnvironmentVariable(confKey) <> String.Empty Then
                value = roTypes.Any2String(Environment.GetEnvironmentVariable(confKey))
            Else
                value = String.Empty
            End If
        Catch ex As Exception
            roLog.GetInstance().logSystemMessage(roLog.EventType.roError, $"roConstants::GetConfigurationParameter", ex)

            value = String.Empty
        End Try

        Select Case confKey.ToLower
            Case "debugmode"
                If value = String.Empty Then value = "false"
        End Select

        Return value
    End Function


    Public Shared Function GetLoggedInPassportTicket() As roPassportTicket
        If Not IsMultitenantServiceEnabled() Then
            Dim tmpObject = VTBase.roConstants.GetGlobalEnvironmentParameter(GlobalAsaxParameter.LoggedInPassportTicket)

            If tmpObject Is Nothing Then
                Return Nothing
            Else
                Return CType(tmpObject, roPassportTicket)
            End If
        Else
            Return Nothing
        End If
    End Function

    Public Shared Function IsMultitenantServiceEnabled() As Boolean
        Dim bIsDistributedEnvironment As Boolean

        Try
            bIsDistributedEnvironment = VTBase.roTypes.Any2Boolean(VTBase.roConstants.GetConfigurationParameter("VTLive.MultitenantService"))
        Catch ex As Exception
            bIsDistributedEnvironment = False
        End Try

        Return bIsDistributedEnvironment
    End Function

    Public Shared Function VTLiveDefaultURL() As String
        Dim sURL As String

        Try
            sURL = VTBase.roTypes.Any2String(VTBase.roConstants.GetConfigurationParameter("URL.VTLive"))
            If Not String.IsNullOrEmpty(sURL) AndAlso Not sURL.EndsWith("/") Then
                sURL &= "/"
            End If
        Catch ex As Exception
            sURL = String.Empty
        End Try

        If sURL = String.Empty Then sURL = "https://vtlive.visualtime.net/"

        Return sURL
    End Function

    Public Shared Function VTPortalDefaultURL() As String
        Dim sURL As String

        Try
            sURL = VTBase.roTypes.Any2String(VTBase.roConstants.GetConfigurationParameter("URL.VTPortal"))
            If Not String.IsNullOrEmpty(sURL) AndAlso Not sURL.EndsWith("/") Then
                sURL &= "/"
            End If
        Catch ex As Exception
            sURL = String.Empty
        End Try

        If sURL = String.Empty Then sURL = "https://vtportal.visualtime.net/"

        Return sURL
    End Function

    Public Shared Function GetManagedThreadGUID() As String

        Dim actualGUID As String = roTypes.Any2String(Threading.Thread.GetDomain().GetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() + "_GUID"))

        If actualGUID = String.Empty Then
            actualGUID = Guid.NewGuid().ToString
            Threading.Thread.GetDomain().SetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() + "_GUID", actualGUID)
        End If

        Return actualGUID

    End Function

    Public Shared Sub InitializeFunctionCallEnvironment(functionTaskName As String)
        If roConstants.IsMultitenantServiceEnabled() Then
            System.Threading.Thread.GetDomain().SetData(System.Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_RequestGUID", Guid.NewGuid().ToString())
            roTrace.GetInstance().AddTraceInfo(Guid.NewGuid().ToString(), functionTaskName, "")

            Dim initMessage As String = $"Time trigger"
            If functionTaskName = "SendPushNotification" Then
                initMessage = "Push"
            ElseIf functionTaskName = "RunMail" Then
                initMessage = "Mail"
            End If

            roTrace.GetInstance().TraceMessage(roTrace.TraceType.roDebug, roTrace.TraceResult.Init, "Start")
        End If


    End Sub

    Public Shared Function MTApplicationName() As String
        Dim sAppName = "VisualTime Live Business"
        If VTBase.roConstants.IsMultitenantServiceEnabled Then
            sAppName = roTypes.Any2String(Threading.Thread.GetDomain().GetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_PoolName"))
            If sAppName = String.Empty Then
                sAppName = "AZURE function"
            End If
        Else
            If BuildManager.GetGlobalAsaxType() IsNot Nothing Then
                sAppName = BuildManager.GetGlobalAsaxType().BaseType.Assembly.GetName().Name
            End If

            If sAppName = String.Empty Then
                sAppName = "AZURE WebApp"
            End If
        End If

        Return sAppName
    End Function


    Public Shared Function BackupThreadData() As roThreadData

        Return New roThreadData With {
            .AppName = Threading.Thread.GetDomain().GetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_AppName"),
            .DefaultLogLevel = Threading.Thread.GetDomain().GetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_DefaultLogLevel"),
            .DefaultTraceLevel = Threading.Thread.GetDomain().GetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_DefaultTraceLevel"),
            .PoolName = Threading.Thread.GetDomain().GetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_PoolName"),
            .DBConnectionString = Threading.Thread.GetDomain().GetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_DBConnectionString"),
            .ReadDBConnectionString = Threading.Thread.GetDomain().GetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_ReadDBConnectionString"),
            .Company = Threading.Thread.GetDomain().GetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_company"),
            .License = Threading.Thread.GetDomain().GetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_license"),
            .RequestGUID = Threading.Thread.GetDomain().GetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_RequestGUID"),
            .SystemPassportID = Threading.Thread.GetDomain().GetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_" & GlobalAsaxParameter.SystemPassportID.ToString())
            }
    End Function

    Public Shared Sub RestoreThreadData(ByVal threadData As roThreadData)

        Threading.Thread.GetDomain().SetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_AppName", threadData.AppName)
        Threading.Thread.GetDomain().SetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_DefaultLogLevel", threadData.DefaultLogLevel)
        Threading.Thread.GetDomain().SetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_DefaultTraceLevel", threadData.DefaultTraceLevel)
        Threading.Thread.GetDomain().SetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_PoolName", threadData.PoolName)
        Threading.Thread.GetDomain().SetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_DBConnectionString", threadData.DBConnectionString)
        Threading.Thread.GetDomain().SetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_ReadDBConnectionString", threadData.ReadDBConnectionString)
        Threading.Thread.GetDomain().SetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_company", threadData.Company)
        Threading.Thread.GetDomain().SetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_license", threadData.License)
        Threading.Thread.GetDomain().SetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_RequestGUID", threadData.RequestGUID)
        Threading.Thread.GetDomain().SetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_" & GlobalAsaxParameter.SystemPassportID.ToString(), threadData.SystemPassportID)

    End Sub

End Class