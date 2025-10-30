Imports System.Configuration
Imports System.Globalization
Imports System.Threading
Imports System.Web
Imports System.Web.SessionState
Imports Robotics.Base.DTOs
Imports Robotics.VTBase

Public MustInherit Class handlerBase : Implements IHttpHandler, IRequiresSessionState

#Region "Declarations"

    Private oLanguage As roLanguageWeb
    Protected Request As HttpRequest
    Protected Response As HttpResponse
    Protected Session As HttpSessionState

    Protected FeaturesNotRequiereUpdateSession() As String = {}

    Public scope As String = ""

#End Region

#Region "Properties"

    Public ReadOnly Property Language() As roLanguageWeb
        Get
            If Me.oLanguage Is Nothing Then Me.SetLanguage()
            Return Me.oLanguage
        End Get
    End Property

    Public ReadOnly Property LanguageFile() As String
        Get
            Dim strLanguageFile As String = ConfigurationManager.AppSettings("LanguageFile")
            Return strLanguageFile
        End Get
    End Property

    Public ReadOnly Property DefaultScope() As String
        Get
            If scope <> String.Empty Then
                Return scope
            Else
                Return Me.GetType().Name.Replace("srv", "")
            End If
        End Get
    End Property

#End Region

#Region "Events"

    Public ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

    Public Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        Me.applyCulture()
        Me.Request = context.Request
        Me.Response = context.Response
        Me.Session = context.Session

        Me.SetActionsNotRequieredUpdateSession()

        If Not FeaturesNotRequiereUpdateSession.Contains(roTypes.Any2String(Request("Action"))) Then
            If WLHelperWeb.CurrentPassport Is Nothing Then Exit Sub
        End If

        ProcessRoboticsRequest(context)
    End Sub

#End Region

#Region "Culture Methods"

    Public Overridable Sub SetActionsNotRequieredUpdateSession()
        FeaturesNotRequiereUpdateSession = {}
    End Sub

    Public MustOverride Sub ProcessRoboticsRequest(ByVal context As HttpContext)

    Public Sub applyCulture()
        Me.InitializeCultures()
    End Sub

    Protected Sub InitializeCultures()
        Dim strCurrentCulture As String = WLHelperWeb.CurrentCulture
        If (Not String.IsNullOrEmpty(strCurrentCulture)) Then
            Thread.CurrentThread.CurrentCulture = New CultureInfo(strCurrentCulture)
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture
        End If
    End Sub

#End Region

    Protected Sub SetLanguage()

        'If WLHelperWeb.CurrentPassport IsNot Nothing Then
        Me.oLanguage = New roLanguageWeb
        WLHelperWeb.SetLanguage(Me.oLanguage, Me.LanguageFile)
        'End If

    End Sub

#Region "Security methods"

    Protected Function GetFeaturePermission(ByVal strFeatureAlias As String, Optional ByVal strFeatureType As String = "U") As Permission
        Return API.SecurityServiceMethods.GetPermissionOverFeature(Nothing, strFeatureAlias, strFeatureType)
    End Function

    Protected Function HasFeaturePermission(ByVal strFeatureAlias As String, ByVal oPermission As Permission, Optional ByVal strFeatureType As String = "U") As Boolean
        Return API.SecurityServiceMethods.HasPermissionOverFeature(Nothing, strFeatureAlias, strFeatureType, oPermission)
    End Function

    Protected Function GetFeaturePermissionByEmployee(ByVal strFeatureAlias As String, ByVal intIDEmployee As Integer, Optional ByVal strFeatureType As String = "U") As Permission
        If strFeatureAlias.Split(".").Length > 1 Then
            ' No es una funcionalidad raiz, miramos los permisos del empleado sobre la funcionalidad raiz (ej: Employees.Type -> Employees)
            Dim oEmployeePermission As Permission = API.SecurityServiceMethods.GetPermissionOverEmployeeAppAlias(Nothing, intIDEmployee, strFeatureAlias.Split(".")(0), strFeatureType)
            Dim oFeaturePermission As Permission = API.SecurityServiceMethods.GetPermissionOverFeature(Nothing, strFeatureAlias, strFeatureType)
            If oEmployeePermission > oFeaturePermission Then
                Return oFeaturePermission
            Else
                Return oEmployeePermission
            End If
        Else
            Return API.SecurityServiceMethods.GetPermissionOverEmployeeAppAlias(Nothing, intIDEmployee, strFeatureAlias, strFeatureType)
        End If
    End Function

    Protected Function HasFeaturePermissionByEmployee(ByVal strFeatureAlias As String, ByVal oPermission As Permission, ByVal intIDEmployee As Integer, Optional ByVal strFeatureType As String = "U") As Boolean
        If strFeatureAlias.Split(".").Length > 1 Then
            ' No es una funcionalidad raiz, miramos los permisos del empleado sobre la funcionalidad raiz (ej: Employees.Type -> Employees)
            Dim bolEmployeeHasPermission As Boolean = API.SecurityServiceMethods.HasPermissionOverEmployee(Nothing, intIDEmployee, strFeatureAlias.Split(".")(0), strFeatureType, oPermission)
            Dim bolFeatureHasPermission As Boolean = API.SecurityServiceMethods.HasPermissionOverFeature(Nothing, strFeatureAlias, strFeatureType, oPermission)
            Return (bolEmployeeHasPermission And bolFeatureHasPermission)
        Else
            Return API.SecurityServiceMethods.HasPermissionOverEmployee(Nothing, intIDEmployee, strFeatureAlias, strFeatureType, oPermission)
        End If
    End Function

    Protected Function HasFeaturePermissionByEmployeeOnDate(ByVal strFeatureAlias As String, ByVal oPermission As Permission, ByVal intIDEmployee As Integer, ByVal dDate As DateTime, Optional ByVal strFeatureType As String = "U") As Boolean
        If strFeatureAlias.Split(".").Length > 1 Then
            ' No es una funcionalidad raiz, miramos los permisos del empleado sobre la funcionalidad raiz (ej: Employees.Type -> Employees)
            Dim bolEmployeeHasPermission As Boolean = API.SecurityServiceMethods.HasPermissionOverEmployeeOnDate(Nothing, intIDEmployee, strFeatureAlias.Split(".")(0), strFeatureType, oPermission, dDate)
            Dim bolFeatureHasPermission As Boolean = API.SecurityServiceMethods.HasPermissionOverFeature(Nothing, strFeatureAlias, strFeatureType, oPermission)
            Return (bolEmployeeHasPermission And bolFeatureHasPermission)
        Else
            Return API.SecurityServiceMethods.HasPermissionOverEmployeeOnDate(Nothing, intIDEmployee, strFeatureAlias, strFeatureType, oPermission, dDate)
        End If
    End Function

    Protected Function GetFeaturePermissionByGroup(ByVal strFeatureAlias As String, ByVal intIDGroup As Integer, Optional ByVal strFeatureType As String = "U") As Permission
        If strFeatureAlias.Split(".").Length > 1 Then
            ' No es una funcionalidad raiz, miramos los permisos del empleado sobre la funcionalidad raiz (ej: Employees.Type -> Employees)
            Dim oGroupPermission As Permission = API.SecurityServiceMethods.GetPermissionOverGroupAppAlias(Nothing, intIDGroup, strFeatureAlias.Split(".")(0), strFeatureType)
            Dim oFeaturePermission As Permission = API.SecurityServiceMethods.GetPermissionOverFeature(Nothing, strFeatureAlias, strFeatureType)
            If oGroupPermission > oFeaturePermission Then
                Return oFeaturePermission
            Else
                Return oGroupPermission
            End If
        Else
            Return API.SecurityServiceMethods.GetPermissionOverGroupAppAlias(Nothing, intIDGroup, strFeatureAlias, strFeatureType)
        End If
    End Function

#End Region

End Class