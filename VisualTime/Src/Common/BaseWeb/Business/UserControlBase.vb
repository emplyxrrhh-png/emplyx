Imports System.Configuration
Imports System.Globalization
Imports System.Threading
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports Robotics.Base.DTOs

Public Class UserControlBase
    Inherits System.Web.UI.UserControl

#Region "Declarations"

    Private oLanguage As roLanguageWeb

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
            Dim strLanguageFile As String
            If Me.TemplateControl.AppRelativeTemplateSourceDirectory.StartsWith("~/Base/") Then
                strLanguageFile = ConfigurationManager.AppSettings("LanguageBaseFile")
            Else
                Try
                    strLanguageFile = "Live" & Me.TemplateControl.AppRelativeTemplateSourceDirectory.Split("/")(1)
                Catch ex As Exception
                    strLanguageFile = ConfigurationManager.AppSettings("LanguageFile")
                End Try

            End If
            Return strLanguageFile
        End Get
    End Property

    Public ReadOnly Property DefaultScope() As String
        Get
            Dim strScope As String
            If Me.TemplateControl IsNot Nothing Then
                strScope = System.IO.Path.GetFileNameWithoutExtension(Me.TemplateControl.AppRelativeVirtualPath)
            Else
                strScope = Me.ID
            End If
            Return strScope
        End Get
    End Property

#End Region

#Region "Events"

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Me.applyCulture()

        'If WLHelperWeb.CurrentPassport IsNot Nothing Then

        Me.oLanguage = New roLanguageWeb
        WLHelperWeb.SetLanguage(Me.oLanguage, Me.LanguageFile)

        If Not Me.IsPostBack Then
            Me.oLanguage.Translate(Me)
        End If

        'End If

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

    Protected Function GetFeaturePermission(ByVal strFeatureAlias As String, Optional ByVal strFeatureType As String = "U") As Permission
        Return API.SecurityServiceMethods.GetPermissionOverFeature(Me.Page, strFeatureAlias, strFeatureType)
    End Function

    Protected Function GetFeaturePermissionByEmployee(ByVal strFeatureAlias As String, ByVal intIDEmployee As Integer, Optional ByVal strFeatureType As String = "U") As Permission
        If strFeatureAlias.Split(".").Length > 1 Then
            ' No es una funcionalidad raiz, miramos los permisos del empleado sobre la funcionalidad raiz (ej: Employees.Type -> Employees)
            Dim oEmployeePermission As Permission = API.SecurityServiceMethods.GetPermissionOverEmployeeAppAlias(Me.Page, intIDEmployee, strFeatureAlias.Split(".")(0), strFeatureType)
            Dim oFeaturePermission As Permission = API.SecurityServiceMethods.GetPermissionOverFeature(Me.Page, strFeatureAlias, strFeatureType)
            If oEmployeePermission > oFeaturePermission Then
                Return oFeaturePermission
            Else
                Return oEmployeePermission
            End If
        Else
            Return API.SecurityServiceMethods.GetPermissionOverEmployeeAppAlias(Me.Page, intIDEmployee, strFeatureAlias, strFeatureType)
        End If
    End Function

    Protected Function GetFeaturePermissionByGroup(ByVal strFeatureAlias As String, ByVal intIDGroup As Integer, Optional ByVal strFeatureType As String = "U") As Permission
        If strFeatureAlias.Split(".").Length > 1 Then
            ' No es una funcionalidad raiz, miramos los permisos del empleado sobre la funcionalidad raiz (ej: Employees.Type -> Employees)
            Dim oGroupPermission As Permission = API.SecurityServiceMethods.GetPermissionOverGroupAppAlias(Me.Page, intIDGroup, strFeatureAlias.Split(".")(0), strFeatureType)
            Dim oFeaturePermission As Permission = API.SecurityServiceMethods.GetPermissionOverFeature(Me.Page, strFeatureAlias, strFeatureType)
            If oGroupPermission > oFeaturePermission Then
                Return oFeaturePermission
            Else
                Return oGroupPermission
            End If
        Else
            Return API.SecurityServiceMethods.GetPermissionOverGroupAppAlias(Me.Page, intIDGroup, strFeatureAlias, strFeatureType)
        End If
    End Function

    Protected Function HasFeaturePermission(ByVal strFeatureAlias As String, ByVal oPermission As Permission, Optional ByVal strFeatureType As String = "U") As Boolean
        Return API.SecurityServiceMethods.HasPermissionOverFeature(Me.Page, strFeatureAlias, strFeatureType, oPermission)
    End Function

    Protected Function HasFeaturePermissionByEmployee(ByVal strFeatureAlias As String, ByVal oPermission As Permission, ByVal intIDEmployee As Integer, Optional ByVal strFeatureType As String = "U") As Boolean
        If strFeatureAlias.Split(".").Length > 1 Then
            ' No es una funcionalidad raiz, miramos los permisos del empleado sobre la funcionalidad raiz (ej: Employees.Type -> Employees)
            Dim bolEmployeeHasPermission As Boolean = API.SecurityServiceMethods.HasPermissionOverEmployee(Me.Page, intIDEmployee, strFeatureAlias.Split(".")(0), strFeatureType, oPermission)
            Dim bolFeatureHasPermission As Boolean = API.SecurityServiceMethods.HasPermissionOverFeature(Me.Page, strFeatureAlias, strFeatureType, oPermission)
            Return (bolEmployeeHasPermission And bolFeatureHasPermission)
        Else
            Return API.SecurityServiceMethods.HasPermissionOverEmployee(Me.Page, intIDEmployee, strFeatureAlias, strFeatureType, oPermission)
        End If
    End Function

    Protected Function HasFeaturePermissionByGroup(ByVal strFeatureAlias As String, ByVal oPermission As Permission, ByVal intIDGroup As Integer, Optional ByVal strFeatureType As String = "U") As Boolean
        If strFeatureAlias.Split(".").Length > 1 Then
            ' No es una funcionalidad raiz, miramos los permisos del empleado sobre la funcionalidad raiz (ej: Employees.Type -> Employees)
            Dim bolGroupHasPermission As Boolean = API.SecurityServiceMethods.HasPermissionOverGroupAppAlias(Me.Page, intIDGroup, strFeatureAlias.Split(".")(0), strFeatureType, oPermission)
            Dim bolFeatureHasPermission As Boolean = API.SecurityServiceMethods.HasPermissionOverFeature(Me.Page, strFeatureAlias, strFeatureType, oPermission)
            Return (bolGroupHasPermission And bolFeatureHasPermission)
        Else
            Return API.SecurityServiceMethods.HasPermissionOverGroupAppAlias(Me.Page, intIDGroup, strFeatureAlias, strFeatureType, oPermission)
        End If
    End Function

    Private DisableControlTypes() As String = {"System.Web.UI.WebControls.TextBox", "System.Web.UI.WebControls.CheckBox", "System.Web.UI.WebControls.RadioButton",
                                               "System.Web.UI.WebControls.ListBox",
                                               "System.Web.UI.HtmlControls.HtmlInputText",
                                               "System.Web.UI.HtmlControls.HtmlInputPassword", "System.Web.UI.HtmlControls.HtmlInputCheckBox",
                                               "System.Web.UI.HtmlControls.HtmlInputRadioButton",
                                               "Robotics.WebControls.roComboBox", "ASP.base_webusercontrols_rooptionpanelclient_ascx"}

    Protected Sub DisableControls(Optional ByVal _Controls As ControlCollection = Nothing)
        If _Controls Is Nothing Then _Controls = Me.Controls
        Me.DisableControlsRecursive(_Controls)
    End Sub

    Private Sub DisableControlsRecursive(ByVal Controls As ControlCollection)

        Dim oProperty As System.Reflection.PropertyInfo
        Dim oContainer As Object = Nothing

        For Each oControl As Control In Controls
            If DisableControlTypes.Contains(oControl.GetType.ToString) Then
                oProperty = HelperWeb.GetProperty(oControl, New String() {"Enabled"}, oContainer)
                If oProperty IsNot Nothing Then
                    oProperty.SetValue(oContainer, False, Nothing)
                Else
                    Try
                        CType(oControl, Object).Attributes("disabled") = "disabled"
                    Catch
                    End Try
                End If
            End If
            If Not TypeOf oControl Is GridView Then
                If oControl.Controls.Count > 0 Then
                    DisableControlsRecursive(oControl.Controls)
                End If
            End If
        Next

    End Sub

#End Region

    Protected Sub SetLanguage()

        'If WLHelperWeb.CurrentPassport IsNot Nothing Then
        Me.oLanguage = New roLanguageWeb
        WLHelperWeb.SetLanguage(Me.oLanguage, Me.LanguageFile)
        'End If

    End Sub

End Class