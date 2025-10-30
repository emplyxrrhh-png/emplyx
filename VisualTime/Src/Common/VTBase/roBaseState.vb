Imports System.Runtime.Serialization
Imports System.Web
Imports Robotics.Base.DTOs

<Serializable()>
<DataContract>
Public MustInherit Class roBaseState

#Region "Declarations - Constructor"

    'Private oLog As roLog

    Private intIDPassport As Integer
    'Protected intResult As ResultEnum
    Private intErrorNumber As Integer
    Private strErrorText As String
    Private strReturnCode As String = String.Empty
    Private strErrorDetail As String
    Private strResultDetail As String

    <IgnoreDataMember>
    Private oContext As System.Web.HttpContext
    Private strClientAddress As String
    Private strSessionID As String
    Private _appType As roAppType
    Private _appSource As roAppSource
    Private _logLevel As roLog.EventType = roLog.EventType.roDebug

    'Private bolMustUpdateAccessTime As Boolean
    'Protected Shared dateLastAccessUpdate As Date
    'Private lockObject As New Object

    Public Sub New()

    End Sub

    Public Sub New(ByVal _IDPassport As Integer, ByVal strClassNameLog As String, Optional ByVal _Context As System.Web.HttpContext = Nothing, Optional ByVal _ClientAddress As String = "", Optional ByVal _SessionID As String = "")

        Me.intIDPassport = _IDPassport
        Me.intErrorNumber = -1
        Me.strErrorText = ""
        Me.strErrorDetail = ""

        Me.oContext = _Context
        Me.strClientAddress = _ClientAddress
        Me.strSessionID = _SessionID

    End Sub

#End Region

#Region "Properties"

    Public Property IDPassport() As Integer
        Get
            Return Me.intIDPassport
        End Get
        Set(ByVal value As Integer)
            Me.intIDPassport = value
        End Set
    End Property

    Public Property ErrorNumber() As Integer
        Get
            Return Me.intErrorNumber
        End Get
        Set(ByVal value As Integer)
            Me.intErrorNumber = value
        End Set
    End Property

    Public Property ReturnCode() As String
        Get
            Return Me.strReturnCode
        End Get
        Set(ByVal value As String)
            Me.strReturnCode = value
        End Set
    End Property

    Public Property ErrorText() As String
        Get
            Return Me.strErrorText
        End Get
        Set(ByVal value As String)
            Me.strErrorText = value
        End Set
    End Property

    Public Property ErrorDetail() As String
        Get
            Return Me.strErrorDetail
        End Get
        Set(ByVal value As String)
            Me.strErrorDetail = value
        End Set
    End Property

    Public Property ResultDetail() As String
        Get
            Return Me.strResultDetail
        End Get
        Set(ByVal value As String)
            Me.strResultDetail = value
        End Set
    End Property

    <Xml.Serialization.XmlIgnore()>
    Public Property Context() As System.Web.HttpContext
        Get
            Return Me.oContext
        End Get
        Set(ByVal value As System.Web.HttpContext)
            Me.oContext = value
        End Set
    End Property

    Public Property ClientAddress() As String
        Get
            Return Me.strClientAddress
        End Get
        Set(ByVal value As String)
            Me.strClientAddress = value
        End Set
    End Property

    Public ReadOnly Property ClientIP() As String
        Get
            Dim strRet As String = Me.ClientAddress.Split("#")(0)
            If strRet = "::1" Then strRet = "127.0.0.1"
            Return strRet.Split(":")(0)
        End Get
    End Property

    Public Property SessionID() As String
        Get
            Return Me.strSessionID
        End Get
        Set(ByVal value As String)
            Me.strSessionID = value
        End Set
    End Property

    Public Property AppType() As roAppType
        Get
            Return Me._appType
        End Get
        Set(ByVal value As roAppType)
            Me._appType = value
        End Set
    End Property

    Public Property AppSource() As roAppSource
        Get
            Return Me._appSource
        End Get
        Set(ByVal value As roAppSource)
            Me._appSource = value
        End Set
    End Property

    Public WriteOnly Property LogLevel As roLog.EventType
        Set(ByVal value As roLog.EventType)
            _logLevel = value
        End Set
    End Property
#End Region

#Region "Methods"

    Public Interface IResultHandler
        Sub SetExceptionResult()
        Sub ClearResult()
    End Interface

    Public Overridable Sub UpdateStateInfo(Optional ByVal _Context As System.Web.HttpContext = Nothing)
        Me.ErrorNumber = -1
        Me.ErrorText = ""
        Me.ErrorDetail = ""
        If _Context IsNot Nothing Then Me.oContext = _Context
    End Sub

    Public Overridable Sub UpdateStateInfo(ByVal Ex As Exception, Optional ByVal strUbication As String = "")
        ' Si implementa la interfaz, llamamos al método específico
        If TypeOf Me Is IResultHandler Then
            DirectCast(Me, IResultHandler).SetExceptionResult()
        End If

        Me.ErrorNumber = 0
        Me.ErrorText = "Unknown application error"
        Me.ErrorDetail = "Unknown application error"
        roLog.GetInstance().logMessage(_logLevel, strUbication, Ex)
        _logLevel = roLog.EventType.roDebug
    End Sub

    Public Overridable Sub UpdateStateInfo(ByVal Ex As System.Data.Common.DbException, Optional ByVal strUbication As String = "")
        ' Si implementa la interfaz, llamamos al método específico
        If TypeOf Me Is IResultHandler Then
            DirectCast(Me, IResultHandler).SetExceptionResult()
        End If

        Me.ErrorNumber = Ex.ErrorCode
        Me.ErrorText = "Unknown application error"
        Me.ErrorDetail = "Unknown application error"
        roLog.GetInstance().logMessage(_logLevel, strUbication, Ex)
        _logLevel = roLog.EventType.roDebug
    End Sub

    Public Function GetApplicationSourceName() As String
        Return roBaseState.GetApplicationSourceName(Me.AppSource, Me.AppType)
    End Function

    Public Function GetSessionId(ByVal aspnetSessionId As String) As String

        Return roBaseState.GetSessionId(aspnetSessionId, Me.intIDPassport, Me.AppSource, Me.AppType)
    End Function

#End Region


#Region "Shared mehtods"
    Public Shared Function GetSessionId(ByVal aspnetSessionId As String, ByVal idPassport As Integer, ByVal appSource As roAppSource, ByVal appType As roAppType) As String

        Return IIf(String.IsNullOrEmpty(aspnetSessionId), idPassport, aspnetSessionId) & "*" & roBaseState.GetApplicationSourceName(appSource, appType)
    End Function

    Public Shared Function GetApplicationSourceName(ByVal appSource As roAppSource, ByVal appType As roAppType) As String
        Select Case appType
            Case roAppType.TerminalsPushServer
                Return roAppType.TerminalsPushServer.ToString()
            Case roAppType.VTPortal
                Select Case appSource
                    Case roAppSource.TimeGate
                        Return $"{roAppType.VTPortal.ToString()}.{roAppSource.TimeGate.ToString()}"
                    Case Else
                        Return roAppType.VTPortal.ToString()
                End Select
            Case roAppType.VTLiveApi
                Return roAppType.TerminalsPushServer.ToString()
            Case roAppType.VTVisits
                Return roAppType.VTVisits.ToString()
            Case roAppType.VTLive
                Return roAppType.VTLive.ToString()
            Case Else
                Return String.Empty
        End Select

    End Function



    Public Shared Sub SetSessionSmall(ByRef oState As Object, ByVal _IDPassport As Integer, ByVal appSource As roAppSource, ByVal aspnetSessionId As String)

        Dim appType As roAppType = roAppType.Unknown
        Dim strAppName As String = String.Empty

        Select Case appSource
            Case roAppSource.TerminalsPushServer
                appType = roAppType.TerminalsPushServer
                strAppName = roAppType.TerminalsPushServer.ToString()
            Case roAppSource.TimeGate, roAppSource.VTPortal, roAppSource.VTPortalApp, roAppSource.VTPortalWeb
                appType = roAppType.VTPortal
                strAppName = roAppType.VTPortal.ToString()
            Case roAppSource.VTLiveApi, roAppSource.mx9
                appType = roAppType.VTLiveApi
                strAppName = roAppType.VTLiveApi.ToString()
            Case roAppSource.Visits
                appType = roAppType.VTVisits
                strAppName = roAppType.VTVisits.ToString()
            Case roAppSource.VTLive
                appType = roAppType.VTLive
                strAppName = roAppType.VTLive.ToString()
        End Select

        Dim bAnonymousRequest As Boolean = False
        Try
            If HttpContext.Current IsNot Nothing AndAlso HttpContext.Current.Session IsNot Nothing AndAlso
                HttpContext.Current.Session("DexAnonymous") IsNot Nothing AndAlso roTypes.Any2Boolean(HttpContext.Current.Session("DexAnonymous")) Then

                bAnonymousRequest = True
            End If
        Catch ex As Exception
            bAnonymousRequest = False
        End Try



        Dim strIP As String = String.Empty
        ' Establecer el objecto de estado para las llamadas a los ws
        With oState

            If bAnonymousRequest Then
                .IDPassport = -1
                .ClientAddress = String.Empty
            Else

                .AppType = appType
                .AppSource = appSource
                .IDPassport = _IDPassport
                Try
                    strIP = "unknownIP"
                    If System.Web.HttpContext.Current IsNot Nothing AndAlso System.Web.HttpContext.Current.Request IsNot Nothing Then
                        strIP = GetClientAddress(System.Web.HttpContext.Current.Request)
                    End If
                Catch
                    strIP = "unknownIP"
                End Try
                .ClientAddress = strIP & "#" & roBaseState.GetApplicationSourceName(appSource, appType)
                .SessionID = roBaseState.GetSessionId(aspnetSessionId, _IDPassport, appSource, appType)
            End If

        End With

    End Sub

    Public Shared Function GetClientAddress(ByVal oRequest As System.Web.HttpRequest) As String
        Dim strRet As String = ""
        If oRequest IsNot Nothing Then
            ' See if the address has been forwarded through a proxy server:
            strRet = oRequest.ServerVariables("HTTP_X_FORWARDED_FOR")
            ' If not, then get the real remote address:
            If strRet = String.Empty Then
                strRet = oRequest.ServerVariables("REMOTE_ADDR")
            End If
        End If
        Return strRet
    End Function
#End Region
End Class