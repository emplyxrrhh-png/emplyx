Imports System.Configuration
Imports System.Globalization
Imports System.Threading
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports Robotics.Base.DTOs

Public Class MasterPageBase
    Inherits System.Web.UI.MasterPage

#Region "Declarations"

    Private oLanguage As roLanguageWeb

    Private bolIsPopup As Boolean = False

    Private bolShowLoading As Boolean = True
    Private oLoadingPanel As HtmlControls.HtmlGenericControl = Nothing

    Private strConvertControlsDiv As String = ""

    Public Shared MasterVersion As String = NoCachePageBase.MasterVersion
    Public NoCacheBase As New NoCachePageBase

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
                strLanguageFile = ConfigurationManager.AppSettings("LanguageFile")
            End If
            Return strLanguageFile
        End Get
    End Property

    Public ReadOnly Property DefaultScope() As String
        Get
            Dim strScope As String
            If Me.TemplateControl IsNot Nothing Then
                strScope = System.IO.Path.GetFileNameWithoutExtension(Me.TemplateControl.AppRelativeVirtualPath)
            ElseIf Me.Page.Form IsNot Nothing Then
                strScope = Me.Page.Form.ID
            ElseIf Me.Page.Header IsNot Nothing Then
                strScope = Me.Page.Header.ID
            Else
                strScope = Me.UniqueID
            End If
            Return strScope
        End Get
    End Property

    Public ReadOnly Property IsPopup() As Boolean
        Get
            Return Me.bolIsPopup
        End Get
    End Property

    Public Property CanClose() As Boolean
        Get
            Dim bolRet As Boolean = False
            Dim oHdnCanClose As System.Web.UI.WebControls.HiddenField = HelperWeb.GetControl(Me.Controls, "hdnCanClose_PageBase")
            If oHdnCanClose IsNot Nothing Then
                bolRet = (oHdnCanClose.Value = "1")
            End If
            Return bolRet
        End Get
        Set(ByVal value As Boolean)
            Dim oHdnCanClose As System.Web.UI.WebControls.HiddenField = HelperWeb.GetControl(Me.Controls, "hdnCanClose_PageBase")
            If oHdnCanClose IsNot Nothing Then
                oHdnCanClose.Value = IIf(value, "1", "0")
            End If
        End Set
    End Property

    Public Property MustRefresh() As String
        Get
            Dim strRet As Integer = "0"

            If TypeOf HelperWeb.GetControl(Me.Controls, "hdnMustRefresh_PageBase") Is System.Web.UI.WebControls.HiddenField Then
                Dim oHdnMustRefresh As System.Web.UI.WebControls.HiddenField = HelperWeb.GetControl(Me.Controls, "hdnMustRefresh_PageBase")
                If oHdnMustRefresh IsNot Nothing Then
                    strRet = oHdnMustRefresh.Value
                End If

            ElseIf TypeOf HelperWeb.GetControl(Me.Controls, "hdnMustRefresh_PageBase") Is System.Web.UI.HtmlControls.HtmlInputHidden Then
                Dim oHdnMustRefresh As System.Web.UI.HtmlControls.HtmlInputHidden = HelperWeb.GetControl(Me.Controls, "hdnMustRefresh_PageBase")

                If oHdnMustRefresh IsNot Nothing Then
                    strRet = oHdnMustRefresh.Value
                End If
            End If

            Return strRet
        End Get
        Set(ByVal value As String)
            If TypeOf HelperWeb.GetControl(Me.Controls, "hdnMustRefresh_PageBase") Is System.Web.UI.WebControls.HiddenField Then
                Dim oHdnMustRefresh As System.Web.UI.WebControls.HiddenField = HelperWeb.GetControl(Me.Controls, "hdnMustRefresh_PageBase")
                If oHdnMustRefresh IsNot Nothing Then
                    oHdnMustRefresh.Value = value
                End If

            ElseIf TypeOf HelperWeb.GetControl(Me.Controls, "hdnMustRefresh_PageBase") Is System.Web.UI.HtmlControls.HtmlInputHidden Then
                Dim oHdnMustRefresh As System.Web.UI.HtmlControls.HtmlInputHidden = HelperWeb.GetControl(Me.Controls, "hdnMustRefresh_PageBase")

                If oHdnMustRefresh IsNot Nothing Then
                    oHdnMustRefresh.Value = value
                End If
            End If

        End Set
    End Property

    Public ReadOnly Property ContextMenuPanel() As Panel
        Get
            Dim oRet As Panel = Nothing
            Dim oContextBar As Control = HelperWeb.GetControl(Me.Controls, "ContextBar1")
            If oContextBar IsNot Nothing Then
                oRet = HelperWeb.GetControl(oContextBar.Controls, "panContextMenu")
            End If
            Return oRet
        End Get
    End Property
    Public ReadOnly Property ContextMenuPanelID() As String
        Get
            Dim strRet As String = ""
            If Me.ContextMenuPanel IsNot Nothing Then strRet = Me.ContextMenuPanel.ClientID
            Return strRet
        End Get
    End Property

    Public Property ShowLoading() As Boolean
        Get
            Return Me.bolShowLoading
        End Get
        Set(ByVal value As Boolean)
            Me.bolShowLoading = value
            ' Borrar el divLoading si value=false
            ' ...
        End Set
    End Property

    Public ReadOnly Property LoadingPanel() As HtmlControls.HtmlGenericControl
        Get
            If Me.bolShowLoading Then
                If Me.oLoadingPanel Is Nothing Then
                    Try
                        Me.oLoadingPanel = New HtmlControls.HtmlGenericControl("div")

                        If Me.Master IsNot Nothing Then
                            Dim oContentContextBar As ContentPlaceHolder = HelperWeb.GetControl(Me.Master.Controls, "contentContextBar")
                            If oContentContextBar IsNot Nothing Then
                                oContentContextBar.Controls.Add(Me.oLoadingPanel)
                            Else
                                Me.Controls.Add(Me.oLoadingPanel)
                            End If
                        Else
                            Me.Controls.Add(Me.oLoadingPanel)
                        End If

                        'Carrega del Format data
                        Dim oHdnDateFormat As New HtmlControls.HtmlInputHidden()
                        oHdnDateFormat.ID = "hdnPageBaseDateFormat"
                        oHdnDateFormat.Value = HelperWeb.GetShortDateFormat
                        Me.Controls.Add(oHdnDateFormat)

                        'Carrega del Format date (numeric)
                        'Guardem el format de la data en un camp ocult per poder traduir les dates correctament
                        'dia=0, mes=1, any=2
                        Dim strDate As String = HelperWeb.GetShortDateFormat.ToLower
                        strDate = Replace(strDate, "ddd", "d")
                        strDate = Replace(strDate, "dd", "d")
                        strDate = Replace(strDate, "mm", "m")
                        strDate = Replace(strDate, "yyyy", "y")
                        strDate = Replace(strDate, "yy", "y")
                        strDate = Replace(strDate, "/", "")
                        strDate = Replace(strDate, "-", "")
                        strDate = Replace(strDate, "d", "0")
                        strDate = Replace(strDate, "m", "1")
                        strDate = Replace(strDate, "y", "2")

                        Dim oHdnDateFormatType As New HtmlControls.HtmlInputHidden()
                        oHdnDateFormatType.ID = "hdnPageBaseDateFormatType"
                        oHdnDateFormatType.Value = strDate
                        Me.Controls.Add(oHdnDateFormatType)
                    Catch
                        Me.bolShowLoading = False
                    End Try
                End If
            End If
            Return Me.oLoadingPanel
        End Get
    End Property

    Public Property ConvertControlsDivID() As String
        Get
            Return Me.strConvertControlsDiv
        End Get
        Set(ByVal value As String)
            Me.strConvertControlsDiv = value
        End Set
    End Property

    ''Public ReadOnly Property ScriptManager() As Object
    ''    Get
    ''        If Me.Master IsNot Nothing Then
    ''            Return HelperWeb.GetControl(Me.Master.Controls, "ScriptManager1")
    ''        Else
    ''            Return HelperWeb.GetControl(Me.Controls, "ScriptManager1")
    ''        End If
    ''    End Get
    ''End Property

#End Region

#Region "Events"

    Private Sub Page_Error(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Error
        Session.Add("VTException", Server.GetLastError())
        Robotics.VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "VTLive::MasterPageBase::" & Server.GetLastError().Message)

        Me.Page.ErrorPage = Me.ResolveUrl("~/Base/Ooops.aspx")
    End Sub

    Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        NoCacheBase.InsertJavascriptIncludes(Me.Page)

        Robotics.Web.Base.HelperWeb.CreateCookie("serverURL", "/" & Configuration.RootUrl & "/", True)
    End Sub

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        NoCacheBase.InsertCssIncludes(Me.Page)

        Me.applyCulture()

        'If WLHelperWeb.CurrentPassport IsNot Nothing Then

        Me.oLanguage = New roLanguageWeb
        WLHelperWeb.SetLanguage(Me.oLanguage, Me.LanguageFile)

        Dim IsPopup As String = Request.Params("IsPopup")
        If IsPopup IsNot Nothing AndAlso IsPopup.Length > 0 AndAlso IsPopup.ToLower = "true" Then
            Me.bolIsPopup = True
        End If

        ' Añade la funcionalidad para detectar 'IsPostBack' en el cliente javascript
        Me.SetIsPostBackScript()

        Me.SetLoadingScript()

        ' Añade funcionalidad para recargar información de la página padre si es necesario
        Me.SetRefreshParentIfNeededScript()

        ' Añade funcionalidad cerrar página si es necesario
        Me.SetCloseScript()

        Me.SetOnKeyDownScript()

        If Not Me.IsPostBack Then
            Me.oLanguage.Translate(DirectCast(Me.Page, PageBase))
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

    Private Sub SetIsPostBackScript()

        Dim oHdnIsPostBack As HiddenField = HelperWeb.GetControl(Me.Controls, "hdnIsPostBack_PageBase")
        If oHdnIsPostBack IsNot Nothing Then

            oHdnIsPostBack.Value = IIf(Me.IsPostBack, "TRUE", "FALSE")

            If (Not Me.Page.ClientScript.IsClientScriptBlockRegistered("IsPostBackScript")) Then

                Dim strScript As String =
                    "<script language=JavaScript> " &
                    "function IsPostBack() { " &
                        "var hdn = $get('" & oHdnIsPostBack.ClientID & "'); " &
                        "return (hdn.value == 'TRUE'); " &
                    "} " &
                    "</script>"

                Me.Page.ClientScript.RegisterClientScriptBlock(GetType(String), "IsPostBackScript", strScript)

            End If

        End If

    End Sub

    Private Sub SetRefreshParentIfNeededScript()

        Dim strCode As String = ""

        Dim oHdnMustRefresh As Object = HelperWeb.GetControl(Me.Controls, "hdnMustRefresh_PageBase")
        Dim oHdnParams As Object = HelperWeb.GetControl(Me.Controls, "hdnParams_PageBase")

        If oHdnMustRefresh IsNot Nothing Then
            If oHdnParams IsNot Nothing Then
                ' Recargar información página padre si es necessario
                strCode = "var MustRefresh = $get('" & oHdnMustRefresh.ClientID & "').value; " &
                          "var _Params = '';" &
                          "if(document.getElementById('" & oHdnParams.ClientID & "') != null){ _Params = $get('" & oHdnParams.ClientID & "').value; } " &
                          "if (MustRefresh != '0') { " &
                              "try { " &
                                  "parent.RefreshScreen(MustRefresh, _Params); " &
                              "} catch(e) {} " &
                          "}"
            Else
                ' Recargar información página padre si es necessario
                strCode = "var MustRefresh = $get('" & oHdnMustRefresh.ClientID & "').value; " &
                          "var _Params = '';" &
                          "if (MustRefresh != '0') { " &
                              "try { " &
                                  "parent.RefreshScreen(MustRefresh, _Params); " &
                              "} catch(e) {} " &
                          "}"
            End If
        End If

        If (Not Me.Page.ClientScript.IsClientScriptBlockRegistered("RefreshParentIfNeededScript")) Then

            Dim strScript As String =
                "<script language=JavaScript> " &
                "function RefreshParentIfNeeded() { " &
                    strCode & " " &
                "} " &
                "</script>"

            Me.Page.ClientScript.RegisterClientScriptBlock(GetType(String), "RefreshParentIfNeededScript", strScript)

        End If

    End Sub

    Private Sub SetCloseScript()

        Dim strCode As String = ""

        Dim oHdnCanClose As System.Web.UI.WebControls.HiddenField = HelperWeb.GetControl(Me.Controls, "hdnCanClose_PageBase")
        If oHdnCanClose IsNot Nothing Then
            ' Cerrar el formulario si está pendiente de cerrar.
            strCode = "var _CanClose = $get('" & oHdnCanClose.ClientID & "'); " &
                      "if (_CanClose.value == '1') Close(); "
        End If

        If (Not Me.Page.ClientScript.IsClientScriptBlockRegistered("CloseScript")) Then
            Dim strScript As String =
                "<script language=JavaScript> " &
                "function Close() { " &
                    "var bolClose = true; " &
                    "try { " &
                        "bolClose = CheckClose_PageBase(); " &
                    "} catch (e) { bolClose = true; } " &
                    "if (bolClose == true) { " &
                        "try { " &
                            "parent.HideExternalForm(); " &
                        "} catch (e){} " &
                    "} " &
                "} " &
                "function CloseIfNeeded() { " &
                    strCode & " " &
                "} " &
                "</script>"

            Me.Page.ClientScript.RegisterClientScriptBlock(GetType(String), "CloseScript", strScript)

        End If

    End Sub

    Private Sub SetOnKeyDownScript()

        If (Not Me.Page.ClientScript.IsClientScriptBlockRegistered("OnKeyDownScript")) Then

            Dim strScript As String =
                "<script language=JavaScript> " &
                "function _KeyDown(e){ " &
                    "var bolRet = true; " &
                    "try { " &
                        "bolRet = OnKeyDown_PageBase(e); " &
                    "} catch(ex) { bolRet = true;} " &
                    "if(bolRet && e && e.keyCode == Sys.UI.Key.esc) { " &
                        "Close(); " &
                    "} " &
                "} " &
                "</script>"

            Me.Page.ClientScript.RegisterClientScriptBlock(GetType(String), "OnKeyDownScript", strScript)

        End If

    End Sub

    Private Sub SetLoadingScript()

        Dim oDivLoading As HtmlControls.HtmlGenericControl = Nothing
        If Me.Master Is Nothing Then
            ' Si la página no tiene master, cogemos el divloading para mostrar el icono de carga, sinó se muestra el loading de la pàgina principal
            oDivLoading = Me.LoadingPanel()
        End If

        If (Not Me.Page.ClientScript.IsClientScriptBlockRegistered("LoadingScript")) Then

            Dim strScript As String

            strScript =
                    "<script language=JavaScript> " & vbCrLf &
                    "function AddRequestEvents() { " & vbCrLf &
                        "with(Sys.WebForms.PageRequestManager.getInstance()) { " & vbCrLf &
                            "add_beginRequest(ShowLoading); " & vbCrLf &
                            "add_endRequest(HideLoading); " & vbCrLf &
                        "} " &
                    "}"

            strScript &=
                "function showLoader(bolLoading) { " & vbCrLf &
                    "if (bolLoading) { " & vbCrLf &
                        "if(typeof(LoadingPanelClient) != 'undefined') { LoadingPanelClient.Show(); }" & vbCrLf &
                    "} else { " & vbCrLf &
                       "if(typeof(LoadingPanelClient) != 'undefined') { LoadingPanelClient.Hide(); }" & vbCrLf &
                    "} " & vbCrLf &
                "} " & vbCrLf &
                "function ShowLoading() { " & vbCrLf

            If Me.ShowLoading Then
                If oDivLoading IsNot Nothing Then
                    ''strScript &= "$get('" & oDivLoading.ClientID & "').style.display = ''; " & vbCrLf
                    ' Ya no utilizamos el pandel de loading creado dinàmicamente, sinó que utilizamos la función showLoader con el loading de la Ext
                    strScript &= "showLoader(true); " & vbCrLf
                Else
                    strScript &= "parent.showLoader(true); " & vbCrLf
                End If
            End If
            strScript &= "} " & vbCrLf &
                "function HideLoading() { " & vbCrLf
            If Me.ShowLoading Then
                If oDivLoading IsNot Nothing Then
                    ''strScript &= "$get('" & oDivLoading.ClientID & "').style.display = 'none'; " & vbCrLf
                    ' Ya no utilizamos el pandel de loading creado dinàmicamente, sinó que utilizamos la función showLoader con el loading de la Ext
                    strScript &= "showLoader(false); " & vbCrLf
                Else
                    strScript &= "parent.showLoader(false); " & vbCrLf
                End If
            End If
            strScript &= "if(typeof endRequestHandler == ""function""){ endRequestHandler(); }" & vbCrLf
            strScript &= "} " & vbCrLf &
                "</script>"
            Me.Page.ClientScript.RegisterClientScriptBlock(GetType(String), "LoadingScript", strScript)

        End If

    End Sub

    Protected Sub SetLanguage()

        'If WLHelperWeb.CurrentPassport IsNot Nothing Then
        Me.oLanguage = New roLanguageWeb
        WLHelperWeb.SetLanguage(Me.oLanguage, Me.LanguageFile)
        'End If

    End Sub

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

End Class