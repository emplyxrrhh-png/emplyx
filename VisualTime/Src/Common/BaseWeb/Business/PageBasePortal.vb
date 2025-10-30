Imports System.Globalization
Imports System.Threading
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports Robotics.Base.DTOs

Public Class PageBasePortal
    Inherits NoCachePageBase

#Region "Declarations"

    Private bolIsPopup As Boolean = False

    Private bolShowLoading As Boolean = True
    Private oLoadingPanel As HtmlControls.HtmlGenericControl = Nothing

    Private strConvertControlsDiv As String = ""

#End Region

#Region "Events"

    Private Sub Page_Error(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Error

        Session.Add("VTException", Server.GetLastError())

    End Sub

    Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

    End Sub

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Me.IsPostBack Then
            roWsUserManagement.SessionObject.AccessApi.InitWebServices()
        End If

        Me.applyCulture()

        'If WLHelperWeb.CurrentPassport IsNot Nothing Then

        Me.oLanguage = New roLanguageWeb
        WLHelperWeb.SetLanguage(Me.oLanguage, Me.LanguageFile)

        If Not Me.IsPostBack Then
            Me.oLanguage.Translate(Me)
        End If

        'End If

        Dim IsPopup As String = Request.Params("IsPopup")
        If IsPopup IsNot Nothing AndAlso IsPopup.Length > 0 AndAlso IsPopup.ToLower = "true" Then
            Me.bolIsPopup = True
        End If

    End Sub

#End Region

#Region "Culture Methods"

    Public Sub applyCulture()
        Me.InitializeCultures()
    End Sub

    Protected Sub InitializeCultures()

        Dim strCurrentCulture As String = WLHelperWeb.CurrentCulture

        If (Not String.IsNullOrEmpty(strCurrentCulture)) Then
            Thread.CurrentThread.CurrentCulture = New CultureInfo(strCurrentCulture)
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture

            Me.SetControlsCulture(Me.Controls, strCurrentCulture)
        End If

    End Sub

    Protected Sub SetControlsCulture(ByVal oControls As ControlCollection, ByVal strCulture As String)

        Dim oContainer As Object = Nothing

        For Each oControl As Control In oControls

            Dim oProperty As System.Reflection.PropertyInfo =
                    HelperWeb.GetProperty(oControl, New String() {"CultureName"}, oContainer)
            If oProperty IsNot Nothing Then
                oProperty.SetValue(oContainer, strCulture, Nothing)
            End If

            If Not TypeOf oControl Is GridView Then
                If oControl.Controls.Count > 0 Then
                    SetControlsCulture(oControl.Controls, strCulture)
                End If
            End If

        Next

    End Sub

#End Region

#Region "Permissions methods"

    Public Function GetFeaturePermission(ByVal strFeatureAlias As String, Optional ByVal strFeatureType As String = "U") As Permission
        Return API.SecurityServiceMethods.GetPermissionOverFeature(Me, strFeatureAlias, strFeatureType)
    End Function

    Public Function GetFeaturePermissionByEmployee(ByVal strFeatureAlias As String, ByVal intIDEmployee As Integer, Optional ByVal strFeatureType As String = "U") As Permission
        If strFeatureAlias.Split(".").Length > 1 Then
            ' No es una funcionalidad raiz, miramos los permisos del empleado sobre la funcionalidad raiz (ej: Employees.Type -> Employees)
            Dim oEmployeePermission As Permission = API.SecurityServiceMethods.GetPermissionOverEmployeeAppAlias(Me, intIDEmployee, strFeatureAlias.Split(".")(0), strFeatureType)
            Dim oFeaturePermission As Permission = API.SecurityServiceMethods.GetPermissionOverFeature(Me, strFeatureAlias, strFeatureType)
            If oEmployeePermission > oFeaturePermission Then
                Return oFeaturePermission
            Else
                Return oEmployeePermission
            End If
        Else
            Return API.SecurityServiceMethods.GetPermissionOverEmployeeAppAlias(Me, intIDEmployee, strFeatureAlias, strFeatureType)
        End If
    End Function

    Public Function GetFeaturePermissionByGroup(ByVal strFeatureAlias As String, ByVal intIDGroup As Integer, Optional ByVal strFeatureType As String = "U") As Permission
        If strFeatureAlias.Split(".").Length > 1 Then
            ' No es una funcionalidad raiz, miramos los permisos del empleado sobre la funcionalidad raiz (ej: Employees.Type -> Employees)
            Dim oGroupPermission As Permission = API.SecurityServiceMethods.GetPermissionOverGroupAppAlias(Me, intIDGroup, strFeatureAlias.Split(".")(0), strFeatureType)
            Dim oFeaturePermission As Permission = API.SecurityServiceMethods.GetPermissionOverFeature(Me, strFeatureAlias, strFeatureType)
            If oGroupPermission > oFeaturePermission Then
                Return oFeaturePermission
            Else
                Return oGroupPermission
            End If
        Else
            Return API.SecurityServiceMethods.GetPermissionOverGroupAppAlias(Me, intIDGroup, strFeatureAlias, strFeatureType)
        End If
    End Function

    Public Function HasFeaturePermission(ByVal strFeatureAlias As String, ByVal oPermission As Permission, Optional ByVal strFeatureType As String = "U") As Boolean
        Return API.SecurityServiceMethods.HasPermissionOverFeature(Me, strFeatureAlias, strFeatureType, oPermission)
    End Function

    Public Function HasFeaturePermissionByEmployee(ByVal strFeatureAlias As String, ByVal oPermission As Permission, ByVal intIDEmployee As Integer, Optional ByVal strFeatureType As String = "U") As Boolean
        If strFeatureAlias.Split(".").Length > 1 Then
            ' No es una funcionalidad raiz, miramos los permisos del empleado sobre la funcionalidad raiz (ej: Employees.Type -> Employees)
            Dim bolEmployeeHasPermission As Boolean = API.SecurityServiceMethods.HasPermissionOverEmployee(Me, intIDEmployee, strFeatureAlias.Split(".")(0), strFeatureType, oPermission)
            Dim bolFeatureHasPermission As Boolean = API.SecurityServiceMethods.HasPermissionOverFeature(Me, strFeatureAlias, strFeatureType, oPermission)
            Return (bolEmployeeHasPermission And bolFeatureHasPermission)
        Else
            Return API.SecurityServiceMethods.HasPermissionOverEmployee(Me, intIDEmployee, strFeatureAlias, strFeatureType, oPermission)
        End If
    End Function

    Public Function HasFeaturePermissionByGroup(ByVal strFeatureAlias As String, ByVal oPermission As Permission, ByVal intIDGroup As Integer, Optional ByVal strFeatureType As String = "U") As Boolean
        If strFeatureAlias.Split(".").Length > 1 Then
            ' No es una funcionalidad raiz, miramos los permisos del empleado sobre la funcionalidad raiz (ej: Employees.Type -> Employees)
            Dim bolGroupHasPermission As Boolean = API.SecurityServiceMethods.HasPermissionOverGroupAppAlias(Me, intIDGroup, strFeatureAlias.Split(".")(0), strFeatureType, oPermission)
            Dim bolFeatureHasPermission As Boolean = API.SecurityServiceMethods.HasPermissionOverFeature(Me, strFeatureAlias, strFeatureType, oPermission)
            Return (bolGroupHasPermission And bolFeatureHasPermission)
        Else
            Return API.SecurityServiceMethods.HasPermissionOverGroupAppAlias(Me, intIDGroup, strFeatureAlias, strFeatureType, oPermission)
        End If
    End Function

#End Region

End Class