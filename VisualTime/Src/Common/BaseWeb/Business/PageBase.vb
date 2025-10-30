Imports System.Globalization
Imports System.Threading
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports Robotics.Base.DTOs

Public Class PageBase
    Inherits NoCachePageBase

#Region "Declarations"

    Private bolIsPopup As Boolean = False

    Private bolShowLoading As Boolean = True
    Private oLoadingPanel As HtmlControls.HtmlGenericControl = Nothing

    Private strConvertControlsDiv As String = ""

    Public Enum QuerystringDefaultParam
        UNDEFINED
        ID
        IDGROUP
        TAB
        SHOW
        DATERANGE 'Calendario FechaDesde y FechaHasta
    End Enum

#End Region

#Region "Properties"

    Public ReadOnly Property GetQuerystringDefaultParamString(ByVal strValue As String) As QuerystringDefaultParam
        Get
            Try
                Dim myEnum As QuerystringDefaultParam
                myEnum = CType(System.Enum.Parse(GetType(QuerystringDefaultParam), strValue.ToUpper), QuerystringDefaultParam)
                Return myEnum
            Catch
                Return QuerystringDefaultParam.UNDEFINED
            End Try
        End Get
    End Property

    Public ReadOnly Property GetParamsFromQuerystring(ByVal strQuerystring) As Generic.Dictionary(Of QuerystringDefaultParam, String)
        Get
            Dim dic As New Generic.Dictionary(Of QuerystringDefaultParam, String)()
            Try
                Dim arrURLParams As String() = strQuerystring.Split("&")
                Dim enumParam As QuerystringDefaultParam
                For i As Byte = 0 To arrURLParams.Length - 1
                    Dim arrParam As Array = arrURLParams(i).Split("=")
                    If arrParam.Length = 2 Then
                        If arrParam(0) <> String.Empty And arrParam(1) <> String.Empty Then
                            enumParam = GetQuerystringDefaultParamString(arrParam(0))
                            If enumParam <> QuerystringDefaultParam.UNDEFINED Then
                                Try
                                    Select Case enumParam

                                        Case QuerystringDefaultParam.ID
                                            Dim intID As Integer = 0
                                            If Integer.TryParse(arrParam(1), intID) Then
                                                dic.Add(enumParam, arrParam(1))
                                            End If

                                        Case QuerystringDefaultParam.IDGROUP
                                            Dim intIDGroup As Integer = 0
                                            If Integer.TryParse(arrParam(1), intIDGroup) Then
                                                dic.Add(enumParam, arrParam(1))
                                            End If

                                        Case QuerystringDefaultParam.TAB
                                            Dim intTab As Integer = 0
                                            If Integer.TryParse(arrParam(1), intTab) Then
                                                dic.Add(enumParam, arrParam(1))
                                            End If

                                        Case QuerystringDefaultParam.SHOW
                                            dic.Add(enumParam, arrParam(1))

                                        Case QuerystringDefaultParam.DATERANGE
                                            Dim strDates As String = arrParam(1)
                                            Dim tmpDate As DateTime
                                            Dim strDate As String = String.Empty
                                            'Aqui hay dos fechas. La longitud minima de una fecha valida es de 10 caracteres

                                            Select Case strDates.Length
                                                Case 16
                                                    Try
                                                        tmpDate = New DateTime(strDates.Substring(0, 4), strDates.Substring(4, 2), strDates.Substring(6, 2))
                                                        strDate = tmpDate.Year() & "#" & tmpDate.Month() & "#" & tmpDate.Day()

                                                        tmpDate = New DateTime(strDates.Substring(8, 4), strDates.Substring(12, 2), strDates.Substring(14, 2))
                                                        strDate &= "-" & tmpDate.Year() & "#" & tmpDate.Month() & "#" & tmpDate.Day()
                                                    Catch
                                                    End Try

                                                Case Is >= 20
                                                    Try
                                                        If DateTime.TryParse(strDates.Substring(0, 10), tmpDate) Then
                                                            strDate = tmpDate.Year() & "#" & tmpDate.Month() & "#" & tmpDate.Day()

                                                            If DateTime.TryParse(strDates.Substring(strDates.Length() - 10, 10), tmpDate) Then
                                                                strDate &= "-" & tmpDate.Year() & "#" & tmpDate.Month() & "#" & tmpDate.Day()
                                                            Else
                                                                strDate = String.Empty
                                                            End If
                                                        End If
                                                    Catch
                                                    End Try
                                            End Select

                                            If strDate <> String.Empty Then
                                                dic.Add(enumParam, strDate)
                                            End If

                                    End Select
                                Catch
                                End Try
                            End If
                        End If
                    End If
                Next
            Catch
            End Try
            Return dic
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
                    Me.oLoadingPanel = New HtmlControls.HtmlGenericControl("div")
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

#End Region

#Region "Events"

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()

        If Me.oLanguage Is Nothing Then
            Me.oLanguage = New roLanguageWeb
            WLHelperWeb.SetLanguage(Me.oLanguage, Me.LanguageFile)

            Me.applyCulture()
        End If
    End Sub

    Private Sub Page_Error(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Error
        Session.Add("VTException", Server.GetLastError())

        Robotics.VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "VTLive::PageBase::" & Server.GetLastError().Message)

        Me.ErrorPage = Me.ResolveUrl("~/Base/Ooops.aspx")
    End Sub

    Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        InsertJavascriptIncludes()
        ThemeControls(Me.Controls)
        InitializeCulture()
    End Sub

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        InsertCssIncludes()

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
            Me.Language.Translate(Me)

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

            If Not Me.IsCallback Then
                Me.SetControlsCulture(Me.Controls, strCurrentCulture)
            End If

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

            'Dim oHdnIsPostBack As System.Web.UI.HtmlControls.HtmlInputHidden
            'If Not Me.IsPostBack Then
            '    oHdnIsPostBack = New System.Web.UI.HtmlControls.HtmlInputHidden
            '    oHdnIsPostBack.ID = "hdnIsPostBack_PageBase"
            '    Me.Form.Controls.Add(oHdnIsPostBack)
            'Else
            '    oHdnIsPostBack = HelperWeb.GetControl(Me.Controls, "hdnIsPostBack_PageBase")
            'End If

            oHdnIsPostBack.Value = IIf(Me.IsPostBack, "TRUE", "FALSE")

            If (Not ClientScript.IsClientScriptBlockRegistered("IsPostBackScript")) Then

                Dim strScript As String =
                    "<script language=JavaScript> " &
                    "function IsPostBack() { " &
                        "var hdn = $get('" & oHdnIsPostBack.ClientID & "'); " &
                        "return (hdn.value == 'TRUE'); " &
                    "} " &
                    "</script>"

                ClientScript.RegisterClientScriptBlock(GetType(String), "IsPostBackScript", strScript)

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

        If (Not ClientScript.IsClientScriptBlockRegistered(GetType(String), "RefreshParentIfNeededScript")) Then

            Dim strScript As String =
                "<script language=JavaScript> " &
                "function RefreshParentIfNeeded() { " &
                    strCode & " " &
                "} " &
                "</script>"

            ClientScript.RegisterClientScriptBlock(GetType(String), "RefreshParentIfNeededScript", strScript)

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

        If (Not ClientScript.IsClientScriptBlockRegistered(GetType(String), "CloseScript")) Then

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

            ClientScript.RegisterClientScriptBlock(GetType(String), "CloseScript", strScript)

        End If

    End Sub

    Private Sub SetOnKeyDownScript()

        If (Not ClientScript.IsClientScriptBlockRegistered(GetType(String), "OnKeyDownScript")) Then

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

            ClientScript.RegisterClientScriptBlock(GetType(String), "OnKeyDownScript", strScript)

        End If

    End Sub

    Private Sub SetLoadingScript()

        Dim oDivLoading As HtmlControls.HtmlGenericControl = Nothing
        If Me.Master Is Nothing Then
            ' Si la página no tiene master, cogemos el divloading para mostrar el icono de carga, sinó se muestra el loading de la pàgina principal
            oDivLoading = Me.LoadingPanel()
        End If

        If (Not ClientScript.IsClientScriptBlockRegistered(GetType(String), "LoadingScript")) Then

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
                    strScript &= "showLoader(true); " & vbCrLf
                Else
                    strScript &= "parent.showLoader(true); " & vbCrLf
                End If
            End If
            strScript &= "} " & vbCrLf &
                "function HideLoading() { " & vbCrLf
            If Me.ShowLoading Then
                If oDivLoading IsNot Nothing Then
                    strScript &= "showLoader(false); " & vbCrLf
                Else
                    strScript &= "parent.showLoader(false); " & vbCrLf
                End If
            End If
            strScript &= "if(typeof endRequestHandler == ""function""){ endRequestHandler(); }" & vbCrLf
            strScript &= "} " & vbCrLf &
                "</script>"
            ClientScript.RegisterClientScriptBlock(GetType(String), "LoadingScript", strScript)

        End If

    End Sub

    Protected Function DownloadFile(ByVal Path As String) As Boolean
        Try
            Dim arrBytes As Byte() = Nothing

            If IO.File.Exists(Path) Then
                arrBytes = IO.File.ReadAllBytes(Path)
                Me.Controls.Clear()
                Response.Clear()
                Response.ClearHeaders()
                Response.ClearContent()
                If Path.Length > 0 AndAlso arrBytes IsNot Nothing AndAlso arrBytes.Length > 0 Then

                    Dim lstFileParameterNames As New List(Of String)
                    Dim lstFileParameterValues As New List(Of String)
                    lstFileParameterNames.Add("{Path}")
                    lstFileParameterValues.Add(Path)
                    lstFileParameterNames.Add("{FileName}")
                    lstFileParameterValues.Add(IO.Path.GetFileName(Path))
                    API.AuditServiceMethods.Audit(Robotics.VTBase.Audit.Action.aExecuted, Robotics.VTBase.Audit.ObjectType.tDownloadFile, IO.Path.GetFileName(Path), lstFileParameterNames, lstFileParameterValues, Me.Page)

                    Response.AddHeader("Content-Disposition", "attachment; filename=""" & IO.Path.GetFileName(Path).Replace(" ", "_") & """")
                    Response.AddHeader("Content-Type", "application/force-download")
                    Response.AddHeader("Content-Transfer-Encoding", "binary")
                    Response.AddHeader("Content-Length", arrBytes.Length)
                    Response.ContentType = "application/octet-stream"
                    Response.OutputStream.Write(arrBytes, 0, arrBytes.Length)
                    Response.Flush()
                    Response.Close()
                    Return True
                End If
            End If
        Catch ex As Exception

        End Try
        Return False
    End Function

#Region "Permissions methods"

    Protected Function GetFeaturePermission(ByVal strFeatureAlias As String, Optional ByVal strFeatureType As String = "U") As Permission
        Return API.SecurityServiceMethods.GetPermissionOverFeature(Me, strFeatureAlias, strFeatureType)
    End Function

    Protected Function GetFeaturePermissionByEmployee(ByVal strFeatureAlias As String, ByVal intIDEmployee As Integer, Optional ByVal strFeatureType As String = "U") As Permission
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

    Protected Function GetFeaturePermissionByEmployeeOnDate(ByVal strFeatureAlias As String, ByVal intIDEmployee As Integer, ByVal dDate As DateTime, Optional ByVal strFeatureType As String = "U") As Permission
        If strFeatureAlias.Split(".").Length > 1 Then
            ' No es una funcionalidad raiz, miramos los permisos del empleado sobre la funcionalidad raiz (ej: Employees.Type -> Employees)
            Dim oEmployeePermission As Permission = API.SecurityServiceMethods.GetPermissionOverEmployeeOnDateAppAlias(Me, intIDEmployee, strFeatureAlias.Split(".")(0), dDate, strFeatureType)
            Dim oFeaturePermission As Permission = API.SecurityServiceMethods.GetPermissionOverFeature(Me, strFeatureAlias, strFeatureType)
            If oEmployeePermission > oFeaturePermission Then
                Return oFeaturePermission
            Else
                Return oEmployeePermission
            End If
        Else
            Return API.SecurityServiceMethods.GetPermissionOverEmployeeOnDateAppAlias(Me, intIDEmployee, strFeatureAlias, dDate, strFeatureType)
        End If
    End Function

    Protected Function GetFeaturePermissionByGroup(ByVal strFeatureAlias As String, ByVal intIDGroup As Integer, Optional ByVal strFeatureType As String = "U") As Permission
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

    Protected Function HasFeaturePermission(ByVal strFeatureAlias As String, ByVal oPermission As Permission, Optional ByVal strFeatureType As String = "U") As Boolean
        Return API.SecurityServiceMethods.HasPermissionOverFeature(Me, strFeatureAlias, strFeatureType, oPermission)
    End Function

    Protected Function HasFeaturePermissionByEmployee(ByVal strFeatureAlias As String, ByVal oPermission As Permission, ByVal intIDEmployee As Integer, Optional ByVal strFeatureType As String = "U") As Boolean
        If strFeatureAlias.Split(".").Length > 1 Then
            ' No es una funcionalidad raiz, miramos los permisos del empleado sobre la funcionalidad raiz (ej: Employees.Type -> Employees)
            Dim bolEmployeeHasPermission As Boolean = API.SecurityServiceMethods.HasPermissionOverEmployee(Me, intIDEmployee, strFeatureAlias.Split(".")(0), strFeatureType, oPermission)
            Dim bolFeatureHasPermission As Boolean = API.SecurityServiceMethods.HasPermissionOverFeature(Me, strFeatureAlias, strFeatureType, oPermission)
            Return (bolEmployeeHasPermission And bolFeatureHasPermission)
        Else
            Return API.SecurityServiceMethods.HasPermissionOverEmployee(Me, intIDEmployee, strFeatureAlias, strFeatureType, oPermission)
        End If
    End Function

    Protected Function HasFeaturePermissionByEmployeeOnDate(ByVal strFeatureAlias As String, ByVal oPermission As Permission, ByVal intIDEmployee As Integer, ByVal dDate As DateTime, Optional ByVal strFeatureType As String = "U") As Boolean
        If strFeatureAlias.Split(".").Length > 1 Then
            ' No es una funcionalidad raiz, miramos los permisos del empleado sobre la funcionalidad raiz (ej: Employees.Type -> Employees)
            Dim bolEmployeeHasPermission As Boolean = API.SecurityServiceMethods.HasPermissionOverEmployeeOnDate(Me, intIDEmployee, strFeatureAlias.Split(".")(0), strFeatureType, oPermission, dDate)
            Dim bolFeatureHasPermission As Boolean = API.SecurityServiceMethods.HasPermissionOverFeature(Me, strFeatureAlias, strFeatureType, oPermission)
            Return (bolEmployeeHasPermission And bolFeatureHasPermission)
        Else
            Return API.SecurityServiceMethods.HasPermissionOverEmployeeOnDate(Me, intIDEmployee, strFeatureAlias, strFeatureType, oPermission, dDate)
        End If
    End Function

    Protected Function HasFeaturePermissionByGroup(ByVal strFeatureAlias As String, ByVal oPermission As Permission, ByVal intIDGroup As Integer, Optional ByVal strFeatureType As String = "U") As Boolean
        If strFeatureAlias.Split(".").Length > 1 Then
            ' No es una funcionalidad raiz, miramos los permisos del empleado sobre la funcionalidad raiz (ej: Employees.Type -> Employees)
            Dim bolGroupHasPermission As Boolean = API.SecurityServiceMethods.HasPermissionOverGroupAppAlias(Me, intIDGroup, strFeatureAlias.Split(".")(0), strFeatureType, oPermission)
            Dim bolFeatureHasPermission As Boolean = API.SecurityServiceMethods.HasPermissionOverFeature(Me, strFeatureAlias, strFeatureType, oPermission)
            Return (bolGroupHasPermission And bolFeatureHasPermission)
        Else
            Return API.SecurityServiceMethods.HasPermissionOverGroupAppAlias(Me, intIDGroup, strFeatureAlias, strFeatureType, oPermission)
        End If
    End Function

    Protected Function HasFeaturePermissionByEmployeeOnDateEx(ByVal strFeatureAlias As String, ByVal oPermission As Permission, ByRef strEmployees As Generic.List(Of String), Optional ByVal strFeatureType As String = "U") As Generic.List(Of String)
        Return API.SecurityServiceMethods.HasPermissionOverEmployeeOnDateEx(Me, strEmployees, strFeatureAlias, strFeatureType, oPermission)
    End Function

    Protected Function HasFeaturePermissionByGroupEx(ByVal strFeatureAlias As String, ByVal oPermission As Permission, ByRef strGroups As Generic.List(Of String), Optional ByVal strFeatureType As String = "U") As Generic.List(Of String)
        Return API.SecurityServiceMethods.HasPermissionOverGroupAppAliasEx(Me, strGroups, strFeatureAlias, strFeatureType, oPermission)
    End Function

    Private DisableControlTypes() As String = {"System.Web.UI.WebControls.TextBox", "System.Web.UI.WebControls.CheckBox", "System.Web.UI.WebControls.RadioButton",
                                               "System.Web.UI.WebControls.ListBox",
                                               "System.Web.UI.HtmlControls.HtmlInputText",
                                               "System.Web.UI.HtmlControls.HtmlInputPassword", "System.Web.UI.HtmlControls.HtmlInputCheckBox",
                                               "System.Web.UI.HtmlControls.HtmlInputRadioButton",
                                               "Robotics.WebControls.roComboBox", "ASP.base_webusercontrols_rooptionpanelclient_ascx", "ASP.base_webusercontrols_basecomps_rooptionpanelclient_ascx",
                                               "DevExpress.Web.ASPxMemo", "DevExpress.Web.ASPxColorEdit", "DevExpress.Web.ASPxComboBox",
                                               "DevExpress.Web.ASPxDateEdit", "DevExpress.Web.ASPxTextBox", "DevExpress.Web.ASPxTimeEdit",
                                               "DevExpress.Web.ASPxRadioButton", "DevExpress.Web.ASPxCheckBox", "DevExpress.Web.ASPxRadioButtonList", "DevExpress.Web.ASPxCheckBoxList"}

    Protected Sub ReadOnlyControls(Optional ByVal _Controls As ControlCollection = Nothing)
        If _Controls Is Nothing Then _Controls = Me.Controls
        Me.ReadOnlyControlsRecursive(_Controls)
    End Sub

    Private Sub ReadOnlyControlsRecursive(ByVal Controls As ControlCollection)

        Dim oProperty As System.Reflection.PropertyInfo
        Dim oContainer As Object = Nothing

        For Each oControl As Control In Controls
            If DisableControlTypes.Contains(oControl.GetType.ToString) Then
                oProperty = HelperWeb.GetProperty(oControl, New String() {"ReadOnly"}, oContainer)
                If oProperty IsNot Nothing Then
                    oProperty.SetValue(oContainer, True, Nothing)
                Else
                    Try
                        CType(oControl, Object).Attributes("readonly") = "readonly"
                    Catch
                    End Try
                End If
            End If
            If Not TypeOf oControl Is GridView Then
                If oControl.Controls.Count > 0 Then
                    ReadOnlyControlsRecursive(oControl.Controls)
                End If
            End If
        Next

    End Sub

    Protected Sub DisableControls(Optional ByVal _Controls As ControlCollection = Nothing)
        If _Controls Is Nothing Then _Controls = Me.Controls
        Me.DisableControlsRecursive(_Controls)
    End Sub

    Private Sub DisableControlsRecursive(ByVal Controls As ControlCollection)

        Dim oProperty As System.Reflection.PropertyInfo
        Dim oContainer As Object = Nothing

        For Each oControl As Control In Controls
            If DisableControlTypes.Contains(oControl.GetType.ToString) Then

                If (oControl.GetType.ToString.ToUpper.Contains("DEVEXPRESS")) Then
                    oProperty = HelperWeb.GetProperty(oControl, New String() {"ReadOnly"}, oContainer)
                    If oProperty IsNot Nothing Then
                        oProperty.SetValue(oContainer, True, Nothing)
                    Else
                        Try
                            CType(oControl, Object).Attributes("ReadOnly") = "readonly"
                        Catch
                        End Try
                    End If
                Else
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
            Else
                Dim i As Integer = 0

            End If
            If Not TypeOf oControl Is GridView Then
                If oControl.Controls.Count > 0 Then
                    DisableControlsRecursive(oControl.Controls)
                End If
            End If
        Next

    End Sub

    Private Sub ThemeControls(ByVal oControlCollection As ControlCollection)
        If Me.oSelectedTheme <> "" Then
            For Each oControl As Control In oControlCollection
                If oThemedControls.Contains(oControl.GetType.ToString) Then
                    CType(oControl, DevExpress.Web.ASPxWebControl).Theme = Me.oSelectedTheme
                End If

                If oControl.Controls IsNot Nothing AndAlso oControl.Controls.Count > 0 Then ThemeControls(oControl.Controls)
            Next
        End If
    End Sub

#End Region

End Class