Imports DevExpress.Web
Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Portal.Business
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions.roServerLicense
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API
Imports ServiceApi

Partial Class WebUserControls_roMainMenu
    Inherits UserControlBase

    <Runtime.Serialization.DataContract()>
    Private Class MainMenuCallbackRequest

        <Runtime.Serialization.DataMember(Name:="action")>
        Public Action As String

        <Runtime.Serialization.DataMember(Name:="path")>
        Public path As String

        <Runtime.Serialization.DataMember(Name:="url")>
        Public url As String

        <Runtime.Serialization.DataMember(Name:="mvcPath")>
        Public mvcPath As String

    End Class

#Region "Declarations"

    Private oMenuList As New wscMenuElementList
    Private oSelectedToolbar1Item As wscMenuElement

    Public Event OptionSelected()
    Public showAIChatBot As Boolean = False
    Public UIDPulse As String = String.Empty

    Private strMainImagesPath As String = "../Images/MainMenu/"

    Private strSelectedToolbar1ItemUrl As String
    Private oPermission As Permission

#End Region

#Region "Properties"

    'Public ReadOnly Property ChangePwdExtender() As AjaxControlToolkit.ModalPopupExtender
    '    Get
    '        Return Me.mpxChangePwd
    '    End Get
    'End Property

    Public ReadOnly Property hdnLicenseIssueClientID() As String
        Get
            Return Me.hdnLicenseIssue.ClientID
        End Get
    End Property

    Public ReadOnly Property hdnLOPDClientID() As String
        Get
            Return Me.hdnLOPD.ClientID
        End Get
    End Property

    Public ReadOnly Property hdnShowENSPopupID() As String
        Get
            Return Me.hdnShowENSPopup.ClientID
        End Get
    End Property

    Public ReadOnly Property hdnPASSWORDEXPIREDClientID() As String
        Get
            Return Me.hdnPASSWORDEXPIRED.ClientID
        End Get
    End Property

    'Public ReadOnly Property Menu_Selected() As String
    '    Get
    '        Return Me.lblMenu_Selected.Text
    '    End Get
    'End Property

    Public ReadOnly Property MenuPath_Selected() As String
        Get
            Return Me.lblMenuPath_Selected.Text
        End Get
    End Property

#End Region

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.licenseExpireSoon.Style("display") = "none"

        'Si esta logat, carrega la botonera de menús...
        If WLHelperWeb.CurrentPassport IsNot Nothing Then

            oPermission = GetFeaturePermission("Administration.Alerts")

            Dim oPassport As roPassportTicket = WLHelperWeb.CurrentPassport

            If oPassport.IsPrivateUser Then
                Me.btChangePwd2.Visible = False
            End If

            If WLHelperWeb.LastAlertsLoadDate.Year = 1970 Then
                lastUpdateText.Text = "Actualizando"
            Else
                lastUpdateText.Text = WLHelperWeb.LastAlertsLoadDate.ToString("HH:mm:ss")
            End If

            Dim oNewTable As New HtmlTable
            Dim oTableRow As HtmlTableRow
            Dim oCell As System.Web.UI.HtmlControls.HtmlTableCell
            Dim oMenu As ASPxMenu

            'Recupera dels webservices els menús PRINCIPAL (el que s'amaga de l'icona)
            oMenuList = PortalServiceMethods.GetMainMenu(Me.Page, "Portal", oPassport.ID, FeatureTypes.ManageFeature, WLHelperWeb.ServerLicense, oPassport.Language.Key)

            oNewTable.CellPadding = 0
            oNewTable.CellSpacing = 0
            oNewTable.Style("padding-bottom") = "7px"
            oNewTable.Style("padding-top") = "7px"
            oNewTable.Border = 0
            oNewTable.Width = "100%"

            Me.VersionHistoryText.Value = Me.Language.Translate("VersionHistory", Me.DefaultScope)

            Dim oScreenMenuElement As MenuItem
            For Each oMenuElement In oMenuList.List
                ' Solo mostramos el elemento si tiene opciones disponibles
                If oMenuElement.Path <> "Portal\Company" Then

                    If oMenuElement.Childs IsNot Nothing AndAlso oMenuElement.Childs.List.Count = 0 Then
                        oTableRow = New HtmlTableRow
                        oNewTable.Rows.Add(oTableRow) ' Creo la fila en la tabla que insertare

                        oCell = New System.Web.UI.HtmlControls.HtmlTableCell ' Creo la celda que contendrá el botón

                        oMenu = New ASPxMenu
                        oMenu.BackColor = System.Drawing.ColorTranslator.FromHtml("#e5ecff")
                        oMenu.Border.BorderWidth = 0
                        oMenu.Width = System.Web.UI.WebControls.Unit.Pixel(240)
                        oMenu.Paddings.PaddingRight = 0
                        oMenu.Orientation = Orientation.Vertical
                        oMenu.ItemStyle.Paddings.Padding = 0
                        oMenu.ShowPopOutImages = DevExpress.Utils.DefaultBoolean.False
                        oMenu.Style("overflow") = "hidden"

                        oMenu.SubMenuItemStyle.Width = System.Web.UI.WebControls.Unit.Pixel(200)
                        oMenu.SubMenuStyle.Wrap = DevExpress.Utils.DefaultBoolean.True
                        oMenu.SubMenuItemStyle.Paddings.Padding = 0

                        oMenu.SubMenuItemStyle.BackColor = Drawing.Color.Transparent

                        Dim menuText As String = oMenuElement.Name

                        If menuText.Length > 33 Then
                            menuText = menuText.Substring(0, 30) & "..."
                        End If

                        oScreenMenuElement = New MenuItem(menuText, oMenuElement.Path.Replace("\", ""), ResolveUrl("~/Base/Images/MainMenu/" & oMenuElement.ImageUrl))

                        oScreenMenuElement.ItemStyle.Paddings.Padding = 0
                        oScreenMenuElement.SubMenuStyle.GutterWidth = 36
                        oScreenMenuElement.ItemStyle.HoverStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("#2460fe")
                        oScreenMenuElement.ItemStyle.HoverStyle.BorderColor = System.Drawing.ColorTranslator.FromHtml("#2460fe")
                        oScreenMenuElement.ItemStyle.HoverStyle.ForeColor = System.Drawing.ColorTranslator.FromHtml("#ffffff")

                        oScreenMenuElement.NavigateUrl = "javascript:top.reenviaFrame('" & String.Format("/{0}/" & oMenuElement.URL, Configuration.RootUrl) & "','','" & oMenuElement.Name.Replace("'", "\'") & "','" & oMenuElement.Path.Replace("\", "") & "');"
                        oScreenMenuElement.Image.Width = 32
                        oScreenMenuElement.Image.Height = 32

                        oMenu.Items.Add(oScreenMenuElement)
                        oCell.Controls.Add(oMenu)

                        oCell.Visible = True

                        oTableRow.Cells.Add(oCell)
                    Else
                        If oMenuElement.Childs IsNot Nothing AndAlso oMenuElement.Childs.List.Count > 0 Then

                            oTableRow = New HtmlTableRow
                            oNewTable.Rows.Add(oTableRow) ' Creo la fila en la tabla que insertare

                            oCell = New System.Web.UI.HtmlControls.HtmlTableCell ' Creo la celda que contendrá el botón

                            oMenu = New ASPxMenu
                            oMenu.BackColor = System.Drawing.ColorTranslator.FromHtml("#e5ecff")
                            oMenu.Border.BorderWidth = 0
                            oMenu.Width = System.Web.UI.WebControls.Unit.Pixel(240)
                            oMenu.Paddings.PaddingRight = 0
                            oMenu.Orientation = Orientation.Vertical
                            oMenu.ItemStyle.Paddings.Padding = 0
                            oMenu.ShowPopOutImages = DevExpress.Utils.DefaultBoolean.False
                            oMenu.Style("overflow") = "hidden"

                            oMenu.SubMenuItemStyle.Width = System.Web.UI.WebControls.Unit.Pixel(200)
                            oMenu.SubMenuStyle.Wrap = DevExpress.Utils.DefaultBoolean.True
                            oMenu.SubMenuItemStyle.Paddings.Padding = 0
                            oMenu.SubMenuStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("#ffffff")

                            'oMenu.Style("background-image") = "url('" & ResolveUrl("~/Base/Images/Transparencia.gif") & "') !important"

                            Dim menuText As String = oMenuElement.Name

                            If menuText.Length > 33 Then
                                menuText = menuText.Substring(0, 30) & "..."
                            End If

                            oScreenMenuElement = New MenuItem(menuText, oMenuElement.Path.Replace("\", ""), ResolveUrl("~/Base/Images/MainMenu/" & oMenuElement.ImageUrl))

                            oScreenMenuElement.ItemStyle.HoverStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("#2460fe")
                            oScreenMenuElement.ItemStyle.HoverStyle.BorderColor = System.Drawing.ColorTranslator.FromHtml("#2460fe")
                            oScreenMenuElement.ItemStyle.HoverStyle.ForeColor = System.Drawing.ColorTranslator.FromHtml("#ffffff")

                            oScreenMenuElement.ItemStyle.Paddings.Padding = 0
                            oScreenMenuElement.SubMenuStyle.GutterWidth = 0

                            For Each oChildElement As wscMenuElement In oMenuElement.Childs.List
                                menuText = oChildElement.Name
                                If menuText.Length > 33 Then
                                    menuText = menuText.Substring(0, 30) & "..."
                                End If

                                Dim childMenu As New DevExpress.Web.MenuItem(menuText, oChildElement.Path.Replace("\", ""), ResolveUrl("~/Base/Images/StartMenuIcos/" & oChildElement.ImageUrl))
                                childMenu.NavigateUrl = "javascript:top.reenviaFrame('" & String.Format("/{0}/" & oChildElement.URL, Configuration.RootUrl) & "','','" & oChildElement.Name.Replace("'", "\'") & "','" & oChildElement.Path.Replace("\", "") & "');"
                                childMenu.Image.Width = 32
                                childMenu.Image.Height = 32
                                childMenu.ItemStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("#f1f5ff")
                                childMenu.ItemStyle.HoverStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("#2460fe")
                                childMenu.ItemStyle.HoverStyle.BorderColor = System.Drawing.ColorTranslator.FromHtml("#2460fe")
                                childMenu.ItemStyle.HoverStyle.ForeColor = System.Drawing.ColorTranslator.FromHtml("#ffffff")

                                oScreenMenuElement.Items.Add(childMenu)
                            Next

                            oMenu.Items.Add(oScreenMenuElement)
                            oCell.Controls.Add(oMenu)

                            oCell.Visible = True

                            oTableRow.Cells.Add(oCell)

                        End If
                    End If
                End If
            Next

            Me.panToolbar1.Controls.Add(oNewTable)

            ' Actualizar nombre empleado
            If WLHelperWeb.CurrentPassport Is Nothing Then
                lblUserName2.Text = ""
            Else
                lblUserName2.Text = WLHelperWeb.CurrentPassport.Name
            End If

            Me.LoadLanguageCombo()
            If Not Me.IsPostBack Then
                Me.cmbLanguage.SelectedValue = WLHelperWeb.CurrentPassport.Language.Key
            End If

            Dim currentVersion As String = ""
            Dim currentVersionDate As String = ""
            Dim versionHistory As String() = Nothing
            If API.LicenseServiceMethods.VersionInfo(currentVersion, currentVersionDate, versionHistory) Then
                If currentVersion.Split(" ").Length > 1 Then
                    Me.lblVersionInfo1.Text = currentVersionDate & " " & currentVersion.Split(" ")(0)
                    Me.lblVersionInfo2.Text = currentVersion.Split(" ")(1).Trim
                    If Me.lblVersionInfo2.Text.StartsWith("(") AndAlso Me.lblVersionInfo2.Text.Length > 1 Then Me.lblVersionInfo2.Text = Me.lblVersionInfo2.Text.Substring(1)
                    If Me.lblVersionInfo2.Text.EndsWith(")") AndAlso Me.lblVersionInfo2.Text.Length > 1 Then Me.lblVersionInfo2.Text = Me.lblVersionInfo2.Text.Substring(0, Me.lblVersionInfo2.Text.Length - 1)
                Else
                    Me.lblVersionInfo1.Text = currentVersion
                    Me.lblVersionInfo2.Text = "" 'currentVersionDate
                End If

                Me.HiddenVersionHistory.Value = String.Join("*", versionHistory)

                If HelperSession.GetFeatureIsInstalledFromApplication("Version\LiveExpress") Then
                    Me.lblVersionType.Text = "Visualtime Live eXpress"
                Else
                    If HelperSession.GetFeatureIsInstalledFromApplication("Feature\ONE") Then
                        Me.lblVersionType.Text = "cegid Visualtime One"
                    Else
                        Me.lblVersionType.Text = "cegid Visualtime"
                    End If
                End If

            End If
        End If

        If (oPermission > Permission.None) Then
            btnAlerts.Visible = True
            Me.btnAlerts.Attributes("onclick") = "reenviaFrame('" & String.Format("/{0}/Alerts/Alerts.aspx", Configuration.RootUrl) & "', '','Alertas','PortalAlerts');"
        Else
            btnAlerts.Visible = False
        End If

        If Not Me.IsPostBack Then

            If Not roTypes.Any2Boolean(HelperSession.AdvancedParametersCache("CheckLicenseLimits")) Then
                Me.hdnLicenseIssue.Value = "1"
            Else
                Me.hdnLicenseIssue.Value = "0"
            End If

            Dim oConfigValue As roAzureConfig = New roConfigRepository().GetConfigParameter(roConfigParameter.showaichatbot)

            If oConfigValue IsNot Nothing AndAlso roTypes.Any2Boolean(Robotics.VTBase.roJSONHelper.DeserializeNewtonSoft(oConfigValue.value, GetType(Boolean))) Then
                showAIChatBot = True
            End If

            Dim oConfigUIDPulseValue As roAzureConfig = New roConfigRepository().GetConfigParameter(roConfigParameter.uidpulse)
            If oConfigUIDPulseValue IsNot Nothing AndAlso oConfigUIDPulseValue.value.Length > 0 Then
                UIDPulse = oConfigUIDPulseValue.value
            End If

            Dim showClientNameToolBar As Boolean = Robotics.VTBase.roTypes.Any2Boolean(HelperSession.AdvancedParametersCache("VTLive.ShowClientName"))
            'Si queremos mostrar el nombre del cliente, ocultamos el div original sin el nombre del cliente y mostramos el que lo contiene.
            If Not showClientNameToolBar Then
                vtLogoVersionDiv.Visible = True
                vtLogoVersionClientNameDiv.Visible = False
            Else
                Dim companyGUID As String = Robotics.DataLayer.roCacheManager.GetInstance().GetCompanyGUID(RoAzureSupport.GetCompanyName())
                Dim oCompanyInfo As roCompanyInfo = Robotics.DataLayer.roCacheManager.GetInstance().GetCompanyInfo(companyGUID)
                If oCompanyInfo IsNot Nothing AndAlso Not String.IsNullOrEmpty(oCompanyInfo.name) Then
                    ClientName.InnerText = oCompanyInfo.name
                    vtLogoVersionClientNameDiv.Visible = True
                    vtLogoVersionDiv.Visible = False
                Else
                    'Si no obtenemos el nombre del cliente, mostramos el menu original
                    vtLogoVersionDiv.Visible = True
                    vtLogoVersionClientNameDiv.Visible = False
                End If
            End If

            Me.hdnLOPD.Value = IIf(Session("LOPD") IsNot Nothing AndAlso Session("LOPD") = True, "1", "0")
            Me.hdnPASSWORDEXPIRED.Value = IIf(Session("PASSWORDEXPIRED") IsNot Nothing AndAlso Session("PASSWORDEXPIRED") = True, "1", "0")
            Me.hdnShowENSPopup.Value = IIf(Session("ShowLegalText") IsNot Nothing AndAlso Session("ShowLegalText").Equals(True) AndAlso Session("ShowLegalText.VTLive") IsNot Nothing AndAlso Session("ShowLegalText.VTLive").Equals(True), "1", "0")
            'Else
            '    Me.hdnLicenseIssue.Value = "0"
            '    Me.hdnLOPD.Value = "0"
            '    Me.hdnPASSWORDEXPIRED.Value = "0"
        End If

        Dim bMustShow As Boolean = False

        If Me.hdnLOPD.Value = "1" Then 'AndAlso Session("LOPD") IsNot Nothing AndAlso Session("LOPD") = True Then
            bMustShow = True
        End If

        If Me.hdnPASSWORDEXPIRED.Value = "1" Then ' AndAlso Session("PASSWORDEXPIRED") IsNot Nothing AndAlso Session("PASSWORDEXPIRED") = True Then
            bMustShow = True
        End If

        If bMustShow Then
            ScriptManager.RegisterStartupScript(Me.Page, Me.GetType(), "showChangePwd", "setTimeout(function(){ShowChangePwd(false);},1500);", True)
            'Me.ChangePwdExtender.Show()
        End If

        Me.licenseExpireSoon.Style("display") = "none"
        Dim iDays As Integer = roTypes.Any2Integer(HelperSession.AdvancedParametersCache("VTLive.LicenseExpireAlert"))
        If WLHelperWeb.ServerLicense IsNot Nothing AndAlso WLHelperWeb.ServerLicense.ServerLicensceStatus <> roLicenseStatus.roLicense_Active Then
            If WLHelperWeb.ServerLicense.ServerLicensceStatus = roLicenseStatus.roLicense_Inactive AndAlso DateTime.Now.Date.AddDays(iDays) > WLHelperWeb.ServerLicense.ExpirationDate Then
                Me.licenseExpireSoon.Style("display") = ""

                Dim oParams As New Generic.List(Of String)
                oParams.Add((WLHelperWeb.ServerLicense.ExpirationDate.Subtract(DateTime.Now.Date)).Days)
                Me.lblExpireSoonDescription.Text = Me.Language.Translate("lblExpireSoonDescriptionComplete", Me.DefaultScope, oParams)
            End If
        End If

        'comprobar si el usuario puede cambiar la contraseña
        Me.trChangePwd.Visible = False

        If WLHelperWeb.CurrentPassportCredential <> String.Empty AndAlso Not WLHelperWeb.CurrentPassportCredential.Contains("\") Then
            Me.trChangePwd.Visible = True
            'Dim oPassportTicket As SecurityService.PassportTicket = WLHelperWeb.CurrentPassport
            'Dim oUser As UserAdminService.wscUserAdmin = API.UserAdminServiceMethods.GetUserAdmin(Me.Page, oPassportTicket.ID)
            'If Not oUser.Login.Contains("\") Then 'Se valida en AD y por lo tanto no puede cambiar la contraseña
            '    Me.trChangePwd.Visible = True
            'End If
        End If

        If WLHelperWeb.CurrentPassportCredential.StartsWith(".\") Then
            Me.trChangePwd.Visible = True
        End If

        If Not Me.HasFeaturePermission("Administration.ReportScheduler.EmergencyReport", Permission.Read) Then
            tdEmergencyPrint.Visible = False
        Else
            lblprintEmer.Text = Me.Language.Translate("EmergencyReports", Me.DefaultScope)
            End If

    End Sub

    Protected Sub ASPxCallbackMainMenu_Callback(ByVal sender As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase) Handles ASPxCallbackMainMenu.Callback

        If WLHelperWeb.CurrentPassport Is Nothing Then
            ASPxCallbackMainMenu.JSProperties.Add("cpLoggedInRO", False)
            Return
        End If

        Dim strParameter As String = roTypes.Any2String(e.Parameter)

        strParameter = Server.UrlDecode(strParameter)

        Dim oParameters As New MainMenuCallbackRequest()
        oParameters = roJSONHelper.Deserialize(strParameter, oParameters.GetType())

        If oParameters.Action = "REFRESHMENU" Then
            Dim path As String = oParameters.path
            Dim url As String = oParameters.url
            Dim mvcPath As String = oParameters.mvcPath
            Dim bIsMVC As Boolean = False

            Dim elementGroup As String
            If url.IndexOf("?") > 0 Then
                elementGroup = SelectToolbar1ItemPath(path, url.Substring(0, url.IndexOf("?")), bIsMVC)
            Else
                elementGroup = SelectToolbar1ItemPath(path, url, bIsMVC)
            End If

            Dim elementText As String = ShowToolbar2(False, bIsMVC)

            If oSelectedToolbar1Item IsNot Nothing AndAlso oSelectedToolbar1Item.Childs.List.Count = 0 AndAlso elementText = String.Empty AndAlso elementGroup <> String.Empty Then
                elementText = elementGroup
                elementGroup = String.Empty
            End If

            If elementGroup = "" AndAlso elementText = "" AndAlso oParameters.url <> String.Empty Then
                If url.IndexOf("?") > 0 Then
                    elementGroup = SelectToolbar1ItemByUrl(path, url.Replace("/" & Configuration.RootUrl & "/", "").Split("?")(0), bIsMVC)
                Else
                    elementGroup = SelectToolbar1ItemByUrl(path, url.Replace("/" & Configuration.RootUrl & "/", ""), bIsMVC)
                End If
                elementText = ShowToolbar2(False, bIsMVC)

                If oSelectedToolbar1Item IsNot Nothing AndAlso oSelectedToolbar1Item.Childs.List.Count = 0 AndAlso elementText = String.Empty AndAlso elementGroup <> String.Empty Then
                    elementText = elementGroup
                    elementGroup = String.Empty
                End If
            End If

            If Not bIsMVC Then
                If (path <> "PortalHome" AndAlso url.Trim <> "#Start") Then
                    If url.IndexOf("?") > 0 Then
                        url = url.Replace("?", ".aspx?")
                    Else
                        url = url & ".aspx"
                    End If
                Else
                    url = "Start"
                End If
            End If

            ASPxCallbackMainMenu.JSProperties.Add("cpUrlRO", url)
            ASPxCallbackMainMenu.JSProperties.Add("cpPathRO", path)
            ASPxCallbackMainMenu.JSProperties.Add("cpPathMVCRO", mvcPath)
            ASPxCallbackMainMenu.JSProperties.Add("cpTextRO", elementText)
            If elementText = String.Empty AndAlso Not url.Contains("Start") Then
                ASPxCallbackMainMenu.JSProperties.Add("cpNotFoundRO", True)
            Else
                ASPxCallbackMainMenu.JSProperties.Add("cpNotFoundRO", False)
            End If

            If WLHelperWeb.CurrentPassport IsNot Nothing Then
                ASPxCallbackMainMenu.JSProperties.Add("cpLoggedInRO", True)
            Else
                ASPxCallbackMainMenu.JSProperties.Add("cpLoggedInRO", False)
            End If
        ElseIf oParameters.Action = "LOGOUT" Then
            WLHelperWeb.SignOut(Me.Page, WLHelperWeb.CurrentPassport)
            ASPxCallbackMainMenu.JSProperties.Add("cpLoggedInRO", False)
            ASPxCallbackMainMenu.JSProperties.Add("cpNotFoundRO", True)
        End If

        If (Me.hdnShowENSPopup.Value = "0") Then
            Session("ShowLegalText.VTLive") = False
        End If
    End Sub

    Protected Sub Toolbar1_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim bIsMvc As Boolean = False
        SelectToolbar1Item(sender.id)

        ShowToolbar2(True, bIsMvc)

    End Sub

    Protected Sub ibtChangeLanguageOK_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btChangeLanguageOKHdn.Click '' ibtChangeLanguageOK.Click
        If API.SecurityServiceMethods.SetLanguage(Me.Page, WLHelperWeb.CurrentPassport.ID, Me.cmbLanguage_Value.Value) Then

            Dim jsLocale As String = "es"
            Dim oLanguages As roPassportLanguage() = API.SecurityServiceMethods.GetLanguages(Me.Page)
            For Each oLang As roPassportLanguage In oLanguages
                If oLang.Key = Me.cmbLanguage_Value.Value Then
                    jsLocale = oLang.Culture.Split("-")(0)
                    Exit For
                End If
            Next

            'Recargamos el currentpassportticket para forzar la actualización del idioma
            HttpContext.Current.Session("WLPassportTicket") = API.SecurityServiceMethods.GetPassportTicket(Me.Page, WLHelperWeb.CurrentPassportID)

            ' Actualizamos la cookie con el último nombre de usuario utilizado
            HelperWeb.EraseCookie("VTLive_Language")
            HelperWeb.CreateCookie("VTLive_Language", jsLocale, False)

            WLHelperWeb.RedirectDefault()
        End If
    End Sub

#End Region

#Region "Methods"

    Public Sub SelectToolbar1Item(ByVal ItemName As String)
        Dim oMenuElement As wscMenuElement

        ' Busco el elemento de la lista que han pulsado
        oSelectedToolbar1Item = Nothing
        For Each oMenuElement In oMenuList.List
            If ItemName = oMenuElement.Name Then
                oSelectedToolbar1Item = oMenuElement
                Try
                    strSelectedToolbar1ItemUrl = "/./" & oMenuElement.Childs.List(0).URL.Replace(".aspx", "")
                Catch ex As Exception
                    strSelectedToolbar1ItemUrl = String.Empty
                End Try
                Me.lblMenu_Selected.Text = oSelectedToolbar1Item.Name
                Me.lblMenuPath_Selected.Text = oSelectedToolbar1Item.Path
                Exit For
            End If
        Next

    End Sub

    Public Function SelectToolbar1ItemPath(ByVal ItemPath As String, ByVal ItemPathUrl As String, ByRef bIsMVC As Boolean) As String
        Dim oMenuElement As wscMenuElement
        Dim elementText As String = ""
        ' Busco el elemento de la lista que han pulsado
        oSelectedToolbar1Item = Nothing

        Dim aspxPath As String = ItemPathUrl & ".aspx"

        If Not oMenuList Is Nothing AndAlso Not oMenuList.List Is Nothing Then
            For Each oMenuElement In oMenuList.List
                If ItemPath = oMenuElement.Path.Replace("\", "") Then
                    oSelectedToolbar1Item = oMenuElement
                    elementText = oSelectedToolbar1Item.Name
                    Me.lblMenu_Selected.Text = oSelectedToolbar1Item.Name
                    Me.lblMenuPath_Selected.Text = oSelectedToolbar1Item.Path

                    If oSelectedToolbar1Item.URL.IndexOf(".aspx") > 0 Then
                        bIsMVC = False
                    Else
                        bIsMVC = True
                    End If
                    strSelectedToolbar1ItemUrl = ItemPathUrl

                    Exit For
                End If
            Next
        End If
        Return elementText
    End Function

    Public Function SelectToolbar1ItemByUrl(ByRef ItemPath As String, ByVal ItemPathUrl As String, ByRef bIsMVC As Boolean) As String
        Dim oMenuElement As wscMenuElement
        Dim elementText As String = ""
        ' Busco el elemento de la lista que han pulsado
        oSelectedToolbar1Item = Nothing
        If Not oMenuList Is Nothing AndAlso Not oMenuList.List Is Nothing Then
            For Each oMenuElement In oMenuList.List

                If oMenuElement.URL.Replace("\", "/").Replace(".aspx", "") = ItemPathUrl Then
                    Dim aspxPath As String = ItemPathUrl & ".aspx"
                    Dim mvcPath As String = ItemPathUrl

                    If aspxPath = oMenuElement.URL Or mvcPath = oMenuElement.URL Then

                        ItemPath = oMenuElement.Path.Replace("\", "")

                        If aspxPath = oMenuElement.URL Then
                            bIsMVC = False
                            strSelectedToolbar1ItemUrl = aspxPath
                        Else
                            bIsMVC = True
                            strSelectedToolbar1ItemUrl = mvcPath
                        End If

                        oSelectedToolbar1Item = oMenuElement
                        elementText = oSelectedToolbar1Item.Name
                        Me.lblMenu_Selected.Text = oSelectedToolbar1Item.Name
                        Me.lblMenuPath_Selected.Text = oSelectedToolbar1Item.Path
                        Exit For
                    End If
                ElseIf oMenuElement.Childs IsNot Nothing AndAlso oMenuElement.Childs.List.Count > 0 Then
                    For Each oChildElement As wscMenuElement In oMenuElement.Childs.List

                        Dim aspxPath As String = ItemPathUrl & ".aspx"
                        Dim mvcPath As String = ItemPathUrl

                        If aspxPath = oChildElement.URL Or mvcPath = oChildElement.URL Then

                            ItemPath = oMenuElement.Path.Replace("\", "")

                            If aspxPath = oChildElement.URL Then
                                bIsMVC = False
                                strSelectedToolbar1ItemUrl = aspxPath
                            Else
                                bIsMVC = True
                                strSelectedToolbar1ItemUrl = mvcPath
                            End If

                            oSelectedToolbar1Item = oMenuElement
                            elementText = oSelectedToolbar1Item.Name
                            Me.lblMenu_Selected.Text = oSelectedToolbar1Item.Name
                            Me.lblMenuPath_Selected.Text = oSelectedToolbar1Item.Path
                            Exit For
                        End If
                    Next

                End If

                If elementText <> String.Empty Then
                    Exit For
                End If

            Next
        End If
        Return elementText
    End Function

    ''' <summary>
    ''' Carrega els botons de la botonera dinamicament
    ''' </summary>
    ''' <param name="bolLoadFirstOption"></param>
    ''' <remarks></remarks>
    Public Function ShowToolbar2(ByVal bolLoadFirstOption As Boolean, ByRef bIsMVC As Boolean) As String
        Dim selectedItemText As String = ""
        If oSelectedToolbar1Item IsNot Nothing Then

            Dim oMenuElement As wscMenuElement

            ' Borrar la table del menú si ya existe
            Dim oMenuTable As HtmlTable = Me.tdMenuToolbar.FindControl("tbMenu")
            If oMenuTable IsNot Nothing Then Me.tdMenuToolbar.Controls.Remove(oMenuTable)

            Dim oTable As New HtmlTable
            Dim oTableRow As New HtmlTableRow
            Dim oTableCell As HtmlTableCell

            Dim oMenuButton As HtmlAnchor

            oTable.ID = "tbMenu"
            oTable.CellPadding = 0
            oTable.CellSpacing = 0
            oTable.Border = 0

            Select Case oSelectedToolbar1Item.Childs.List.Count
                Case 1
                    oTable.Attributes("class") = "tbMenu"
                Case 2
                    oTable.Attributes("class") = "tbMenu2"
                Case 3
                    oTable.Attributes("class") = "tbMenu3"
                Case 4
                    oTable.Attributes("class") = "tbMenu4"
                Case 5
                    oTable.Attributes("class") = "tbMenu5"
                Case 6
                    oTable.Attributes("class") = "tbMenu6"
                Case 7
                    oTable.Attributes("class") = "tbMenu7"
                Case 8
                    oTable.Attributes("class") = "tbMenu8"
                Case Else
                    oTable.Attributes("class") = "tbMenu8"
            End Select

            oTable.Rows.Add(oTableRow)

            Me.mainMenuBar.Controls.Clear()

            For n As Integer = 0 To oSelectedToolbar1Item.Childs.List.Count - 1

                oMenuElement = oSelectedToolbar1Item.Childs.List(n)

                If oMenuElement.URL = "" Then Continue For

                oTableCell = New HtmlTableCell
                oTableCell.Attributes("class") = "tbd_button"

                oMenuButton = New HtmlAnchor

                With oMenuButton
                    If oMenuElement.URL <> "" Then
                        .ID = oSelectedToolbar1Item.Name & "_MainMenu_Button" & n.ToString
                        .Name = oSelectedToolbar1Item.Name & "_MainMenu_Button" & n.ToString
                        .InnerText = oMenuElement.Name
                        .Title = oMenuElement.Name

                        Try
                            .Attributes("class") = "tb_button"

                            If String.IsNullOrEmpty(strSelectedToolbar1ItemUrl) = False Then
                                If strSelectedToolbar1ItemUrl.Contains(".aspx") Then
                                    If strSelectedToolbar1ItemUrl = oMenuElement.URL Then
                                        .Attributes("class") = "tb_button_active"
                                        selectedItemText = oMenuElement.Name
                                        oTableCell.Attributes("class") = "tbd_button tb_button_active_back"

                                        If oMenuElement.URL.Contains(".aspx") Then
                                            bIsMVC = False
                                        Else
                                            bIsMVC = True
                                        End If
                                    End If
                                Else
                                    If strSelectedToolbar1ItemUrl.Replace("/" & Configuration.RootUrl & "/", "") & ".aspx" = oMenuElement.URL Or
                                        strSelectedToolbar1ItemUrl.Replace("/" & Configuration.RootUrl & "/", "") = oMenuElement.URL Then
                                        .Attributes("class") = "tb_button_active"
                                        selectedItemText = oMenuElement.Name
                                        oTableCell.Attributes("class") = "tbd_button tb_button_active_back"

                                        If oMenuElement.URL.Contains(".aspx") Then
                                            bIsMVC = False
                                        Else
                                            bIsMVC = True
                                        End If
                                    End If
                                End If
                            End If
                        Catch ex As Exception
                            .Attributes("class") = "tb_button"
                        End Try

                        If Request.ApplicationPath.EndsWith("/") Then
                            .HRef = Request.ApplicationPath + "Main.aspx#/" + Configuration.RootUrl + "/" + oMenuElement.URL.Split(".")(0)
                        Else
                            .HRef = Request.ApplicationPath + "/Main.aspx#/" + Configuration.RootUrl + "/" + oMenuElement.URL.Split(".")(0)
                        End If

                        Dim strPath As String = oMenuElement.Path.Substring(0, oMenuElement.Path.LastIndexOf("\"))
                        .Attributes("onclick") = "reenviaFrame('" & String.Format("/{0}/" & oMenuElement.URL, Configuration.RootUrl) & "', '" & "MainMenu_" & oSelectedToolbar1Item.Name.Replace("'", "\'") & "_MainMenu_Button" & n.ToString & "','" & oMenuElement.Name.Replace("'", "\'") & "','" & strPath & "'); return false;"
                        .Attributes("linkPath") = strPath.Replace("\", "")

                        Dim span = New HtmlGenericControl("span")
                        span.Controls.Add(oMenuButton)
                        oTableCell.Controls.Add(span)
                    End If
                End With

                oTableRow.Cells.Add(oTableCell)
            Next

            Me.mainMenuBar.Controls.Add(oTable)

        End If

        Return selectedItemText
    End Function

    Private Sub ShowChilds(ByVal MenuElement As wscMenuElement, ByVal MenuItem As WebControls.MenuItem)
        Dim oMenuElement As wscMenuElement
        Dim oMenuItem As WebControls.MenuItem

        For Each oMenuElement In MenuElement.Childs.List
            oMenuItem = New WebControls.MenuItem
            oMenuItem.Text = oMenuElement.Name
            oMenuItem.ToolTip = oMenuElement.Name
            oMenuItem.Value = oMenuElement.URL
            oMenuItem.ImageUrl = Me.strMainImagesPath & oMenuElement.ImageUrl
            oMenuItem.NavigateUrl = String.Format("/{0}/" & oMenuElement.URL, Configuration.RootUrl)

            MenuItem.ChildItems.Add(oMenuItem)
            If oMenuElement.Childs.List.Count > 0 Then
                ShowChilds(oMenuElement, oMenuItem)
            End If
        Next

    End Sub

    Private Function GetControl(ByVal ParentObject As Object, ByVal ControlName As String) As Object
        Dim oControl As Object

        For Each oControl In ParentObject.controls
            If oControl.ID = ControlName Then
                Return oControl
            End If
        Next

        Return Nothing
    End Function

    Public Sub UpdateContext(Optional ByVal _IDPassport As Integer = -1)

        Dim _Context As WebCContext = WLHelperWeb.Context(Me.Request, _IDPassport)
        Dim bIsMVC As Boolean = False
        If _Context IsNot Nothing Then
            With _Context
                If .MenuGroup <> "" Then
                    SelectToolbar1Item(.MenuGroup)
                    ShowToolbar2(False, bIsMVC)
                End If
            End With
        End If

    End Sub

    Protected Sub btSignOut_ServerClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles btSignOut2.ServerClick
        WLHelperWeb.SignOut(Me.Page, WLHelperWeb.CurrentPassport)
        WLHelperWeb.RedirectNotAuthenticated()
    End Sub

    Private Sub LoadLanguageCombo()
        Dim oLanguages As roPassportLanguage() = API.SecurityServiceMethods.GetLanguages(Me.Page)

        With Me.cmbLanguage
            .ClearItems()
            Dim oParams As Generic.List(Of String)
            For Each oLanguage As roPassportLanguage In oLanguages
                oParams = New Generic.List(Of String)
                oParams.Add(IIf(oLanguage.Installed, "", Me.Language.Translate("Language.NotInstalled", Me.DefaultScope)))
                .AddItem(Language.Translate("Language." & oLanguage.Key, Me.DefaultScope, oParams), oLanguage.Key, oLanguage.Installed, "")
            Next
        End With

    End Sub

    Public Function LiveVersion() As String

        Dim strRet As String = "Undefined"

        If WLHelperWeb.CurrentPassport(True) IsNot Nothing Then

            Dim oLicSupport As New Robotics.VTBase.Extensions.roLicenseSupport()
            Dim oLicInfo As Robotics.VTBase.Extensions.roVTLicense = oLicSupport.GetVTLicenseInfo()
            If oLicInfo.Edition = Robotics.VTBase.Extensions.roServerLicense.roVisualTimeEdition.Starter Then
                strRet = "Starter"
            Else
                If HelperSession.GetFeatureIsInstalledFromApplication("Version\LiveExpress", WLHelperWeb.CurrentPassport(True), True) Then
                    strRet = "LiveExpress"
                Else
                    If HelperSession.GetFeatureIsInstalledFromApplication("Feature\ONE", WLHelperWeb.CurrentPassport(True), True) Then
                        strRet = "One"
                    End If

                    strRet = "Live"
                End If
            End If
        Else
            strRet = "Live"
        End If

        Return strRet

    End Function

#End Region

End Class