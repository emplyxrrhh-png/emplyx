Imports System.Drawing
Imports System.Threading
Imports System.Web
Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer
Imports Robotics.Security
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.roConstants

Public MustInherit Class roBaseGlobalAsax
    Inherits System.Web.HttpApplication

#Region "Properties with VTBase access"

    Private Shared _AppName As String
    Private Shared _LogFileName As String = String.Empty
    Private Shared xLastRefreshDateTime As New Hashtable ' DateTime = DateTime.MinValue
    Private Shared iTimeout As New Hashtable 'Integer = 30
    Private Shared iMaxConcurrentSessions As New Hashtable 'Integer = 1
    Private Shared iLogLevel As roLog.EventType = roLog.EventType.roDebug
    Private Shared iTraceLevel As roTrace.TraceType = roTrace.TraceType.roInfo
    Private Shared iSystemPassportID As New Hashtable 'Integer = 0

    Public ReadOnly Property LastServerCheckDateTime As DateTime
        Get
            Return DateTime.Now
        End Get
    End Property

    Public ReadOnly Property LastRefreshDateTime As DateTime
        Get
            If roBaseGlobalAsax.xLastRefreshDateTime(CompanyId) Is Nothing Then
                roBaseGlobalAsax.xLastRefreshDateTime(CompanyId) = DateTime.MinValue
            End If

            Return roTypes.Any2DateTime(roBaseGlobalAsax.xLastRefreshDateTime(CompanyId))
        End Get
    End Property

    Public ReadOnly Property ServerTimeout As Integer
        Get
            Return roTypes.Any2Integer(roBaseGlobalAsax.iTimeout(CompanyId))
        End Get
    End Property

    Public ReadOnly Property MaxConcurrentSessions As Integer
        Get
            Return roTypes.Any2Integer(roBaseGlobalAsax.iMaxConcurrentSessions(CompanyId))
        End Get
    End Property

    Public ReadOnly Property LogEnabled As Boolean
        Get
            Return False
        End Get
    End Property

    Public Property LogFileName As String
        Get
            Return _LogFileName
        End Get
        Set(value As String)
            _LogFileName = value
        End Set
    End Property

    Public ReadOnly Property SystemPassportID As Integer
        Get
            Return roTypes.Any2Integer(roBaseGlobalAsax.iSystemPassportID(CompanyId))
        End Get
    End Property

    Public ReadOnly Property CurrentIdPassport As Integer
        Get
            Return GetLoggedInPassportIdFromSession()
        End Get
    End Property

    Public ReadOnly Property CompanyId As String
        Get

            Dim computedCompanyName As String = String.Empty
            If _AppName = roAppType.VTLive.ToString() Then
                If HttpContext.Current.Session IsNot Nothing Then
                    computedCompanyName = If(HttpContext.Current.Session("roMultiCompanyId") IsNot Nothing, HttpContext.Current.Session("roMultiCompanyId"), "")
                Else
                    computedCompanyName = ""
                End If
            ElseIf (_AppName = roAppType.TerminalsPushServer.ToString()) AndAlso HttpContext.Current.Session IsNot Nothing Then
                computedCompanyName = If(HttpContext.Current.Session("roClientCompanyId") IsNot Nothing, HttpContext.Current.Session("roClientCompanyId"), "")
            ElseIf (_AppName = roAppType.VTPortal.ToString()) Then
                computedCompanyName = If(HttpContext.Current.Request.Headers("roCompanyID") IsNot Nothing, HttpContext.Current.Request.Headers("roCompanyID"), "")
            ElseIf (_AppName = roAppType.VTVisits.ToString()) Then
                computedCompanyName = If(HttpContext.Current.Request.Headers("roCompanyID") IsNot Nothing, HttpContext.Current.Request.Headers("roCompanyID"), "")
            ElseIf (_AppName = roAppType.VTLiveApi.ToString()) Then
                If Request IsNot Nothing AndAlso (Request.Path.ToLower.Contains("/api") OrElse Request.Path.ToLower.Contains("/datalink")) AndAlso HttpContext.Current.Session IsNot Nothing Then
                    computedCompanyName = If(HttpContext.Current.Session("roClientCompanyId") IsNot Nothing, HttpContext.Current.Session("roClientCompanyId"), "")
                Else
                    computedCompanyName = If(HttpContext.Current.Request.Headers("roCompanyID") IsNot Nothing, HttpContext.Current.Request.Headers("roCompanyID"), "")
                End If
            Else
                computedCompanyName = ""
            End If

            Return computedCompanyName.ToLower()
        End Get
    End Property

    Public ReadOnly Property ClientCompanyId As String
        Get
            If HttpContext.Current.Session IsNot Nothing Then
                Return If(HttpContext.Current.Session("roClientCompanyId") IsNot Nothing, HttpContext.Current.Session("roClientCompanyId"), "")
            Else
                Return String.Empty
            End If
        End Get
    End Property

    Public ReadOnly Property RequestGUID As String
        Get
            Try
                If HttpContext.Current.Request IsNot Nothing AndAlso HttpContext.Current.Request.Headers("RequestId") IsNot Nothing Then
                    Return HttpContext.Current.Request.Headers("RequestId")
                Else
                    Return String.Empty
                End If
            Catch ex As Exception
                Return String.Empty
            End Try
        End Get
    End Property

    Public Property LogLevel As roLog.EventType
        Get
            Dim oTmpLogLevel As roLog.EventType = iLogLevel

            Return oTmpLogLevel
        End Get
        Set(value As roLog.EventType)
            iLogLevel = value
        End Set
    End Property

    Public Property TraceLevel As roTrace.TraceType
        Get
            Dim oTmpLogLevel As roTrace.TraceType = iTraceLevel

            Return oTmpLogLevel
        End Get
        Set(value As roTrace.TraceType)
            iTraceLevel = value
        End Set
    End Property

    Public Property AppName As String
        Get
            Return _AppName
        End Get
        Set(value As String)
            _AppName = value
        End Set
    End Property

    Public ReadOnly Property LoggedInPassportTicket As roPassportTicket
        Get
            Return GetLoggedInPassportFromSession()
        End Get
    End Property

#End Region

#Region "Abstract Methods to be implemented in derived classes"
    Protected MustOverride Function GetLoggedInPassportFromSession() As roPassportTicket
    Protected MustOverride Function GetLoggedInPassportIdFromSession() As Integer

#End Region

#Region "Global asax events"
    Sub OnApplicationBeginRequest()
        roBaseGlobalAsax.ReloadSharedData()

    End Sub

    Sub OnApplicationReloadSharedData()
        roBaseGlobalAsax.ReloadSharedData()
    End Sub

    Sub OnApplicationEndRequest()
        Robotics.DataLayer.AccessHelper.CleanUpCurrentConnection()
        Dim absolutePath As String = HttpContext.Current.Request.Url.AbsolutePath
        If absolutePath.Contains(".png") OrElse absolutePath.Contains(".gif") OrElse
            absolutePath.Contains(".jpeg") OrElse absolutePath.Contains(".jpg") OrElse
            absolutePath.Contains(".js") OrElse absolutePath.Contains(".css") OrElse
            absolutePath.Contains(".eot") OrElse absolutePath.Contains(".svg") OrElse
            absolutePath.Contains(".woff") OrElse absolutePath.Contains(".otf") OrElse
            absolutePath.Contains(".ttf") Then
            Return
        End If

        Dim hasData As Boolean = False

        Try
            If roTypes.Any2String(HttpContext.Current.Request.Headers("RequestId")) <> String.Empty Then
                hasData = True
            End If
        Catch ex As Exception
            hasData = False
        End Try

        If hasData Then roTrace.GetInstance().TraceMessage(roTrace.TraceType.roInfo, roTrace.TraceResult.Ok, $"Request")

        roTelemetryInfo.GetInstance().ClearO11yInfo()
    End Sub

#End Region

#Region "Global asax shared methods"
    Public Shared Sub ResetSystemCache()
        Dim strCompanyId As String = VTBase.roConstants.GetGlobalEnvironmentParameter(GlobalAsaxParameter.CompanyId)

        xLastRefreshDateTime(strCompanyId) = Nothing
        iTimeout(strCompanyId) = Nothing
        iMaxConcurrentSessions(strCompanyId) = Nothing
        iSystemPassportID(strCompanyId) = Nothing

        roBaseGlobalAsax.ReloadSharedData()
    End Sub

    Public Shared Sub ReloadSharedData()

        Try
            If HttpContext.Current.Request IsNot Nothing AndAlso HttpContext.Current.Request.Headers("RequestId") Is Nothing Then HttpContext.Current.Request.Headers.Add("RequestId", Guid.NewGuid.ToString())
        Catch ex As Exception
            VTBase.roLog.GetInstance().logSystemMessage(roLog.EventType.roError, "BaseGlobalAsax::Error::Coul not set RequestID", ex)
        End Try

        Dim absolutePath As String = HttpContext.Current.Request.Url.AbsolutePath
        If absolutePath.Contains(".png") OrElse absolutePath.Contains(".gif") OrElse
            absolutePath.Contains(".jpeg") OrElse absolutePath.Contains(".jpg") OrElse
            absolutePath.Contains(".js") OrElse absolutePath.Contains(".css") OrElse
            absolutePath.Contains(".eot") OrElse absolutePath.Contains(".svg") OrElse
            absolutePath.Contains(".woff") OrElse absolutePath.Contains(".otf") OrElse
            absolutePath.Contains(".ttf") Then
            Return
        End If

        roTelemetryInfo.GetInstance().ClearO11yInfo()
        If roConstants.GetGlobalEnvironmentParameter(GlobalAsaxParameter.RequestGUID) <> String.Empty AndAlso RoAzureSupport.GetCompanyName() <> "" Then
            roCacheManager.GetInstance().NeedToRefreshCompanyCache(RoAzureSupport.GetCompanyName())

            Dim dbInitialized As Boolean = True
            Try
                Dim strSQL As String = "@SELECT# ID FROM sysroPassports WHERE Description = '@@ROBOTICS@@System'"
                roBaseGlobalAsax.iSystemPassportID(RoAzureSupport.GetCompanyName()) = Robotics.DataLayer.AccessHelper.ExecuteScalar(strSQL)
            Catch ex As Exception
                dbInitialized = False
                roBaseGlobalAsax.iTimeout(RoAzureSupport.GetCompanyName()) = 30
            End Try

            If Not dbInitialized Then Return

            LoadRequestRelatedData()
        End If

    End Sub

    Private Shared Sub LoadRequestRelatedData()
        Dim companyLogLevel As String = DataLayer.roCacheManager.GetInstance().GetAdvParametersCache(RoAzureSupport.GetCompanyName(), $"Application.LogLevel").Trim
        Dim companyTraceLevel As String = DataLayer.roCacheManager.GetInstance().GetAdvParametersCache(RoAzureSupport.GetCompanyName(), $"Application.TraceLevel").Trim
        roConstants.SetDefaultCompanyTraceAndLogLevel(companyLogLevel, companyTraceLevel)

        roTrace.GetInstance().AddTraceInfo(HttpContext.Current.Request.Headers("RequestId"), HttpContext.Current.Request.Url.AbsolutePath, RoAzureSupport.GetCompanyName())
        roTrace.GetInstance().TraceMessage(roTrace.TraceType.roDebug, roTrace.TraceResult.Init, "Request")

        roBaseGlobalAsax.iTimeout(RoAzureSupport.GetCompanyName()) = roTypes.Any2Integer(roCacheManager.GetInstance().GetParametersCache(RoAzureSupport.GetCompanyName(), DTOs.Parameters.SessionTimeout))

        Dim oLicense As New roServerLicense
        Dim objRet As Object = oLicense.FeatureData("VisualTime Server", "MaxConcurrentSessions")
        If Not IsNothing(objRet) Then
            If Not Integer.TryParse(objRet.ToString, roBaseGlobalAsax.iMaxConcurrentSessions(RoAzureSupport.GetCompanyName())) Then
                roBaseGlobalAsax.iMaxConcurrentSessions(RoAzureSupport.GetCompanyName()) = 0
            End If
        Else
            roBaseGlobalAsax.iMaxConcurrentSessions(RoAzureSupport.GetCompanyName()) = 0
        End If
    End Sub
#End Region
End Class