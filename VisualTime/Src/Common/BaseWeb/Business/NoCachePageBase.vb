Imports System.Configuration
Imports System.IO
Imports System.Security.Cryptography
Imports System.Text
Imports System.Web.UI
Imports DevExpress.Web

Public Class NoCachePageBase
    Inherits System.Web.UI.Page

    Protected oLanguage As roLanguageWeb
    Protected strOverridableScope As String = String.Empty
    Protected strOverrrideLanguageFile As String = String.Empty

    Public oSelectedTheme As String = "Material1" '"Robo" '/ "Office365"

    Public oThemedControls As String() = {
        "DevExpress.Web.ASPxTextBox",
        "DevExpress.Web.ASPxMemo",
        "DevExpress.Web.ASPxDateEdit",
        "DevExpress.Web.ASPxTimeEdit",
        "DevExpress.Web.ASPxComboBox",
        "DevExpress.Web.ASPxRadioButton",
        "DevExpress.Web.ASPxRadioButtonList",
        "DevExpress.Web.ASPxCheckBox",
        "DevExpress.Web.ASPxCheckBoxList",
        "DevExpress.Web.ASPxLabel",
        "DevExpress.Web.ASPxTokenBox",
         "DevExpress.Web.ASPxSpinEdit",
        "DevExpress.Web.ASPxSpreadsheet.ASPxSpreadsheet",
        "DevExpress.Web.ASPxPivotGrid.ASPxPivotGrid",
        "DevExpress.Web.ASPxHtmlEditor.ASPxHtmlEditor"
        } 'ArrayList

    Private Const oMapsKey As String = "AIzaSyDEu7Qo0HTVOFAw3xDla35s_wMhvY7qiGw"

    Public Shared MasterVersion As String = "?v=" & Robotics.VTBase.roTypes.Any2String(Reflection.Assembly.GetExecutingAssembly().GetCustomAttributes(GetType(Reflection.AssemblyDescriptionAttribute), False).FirstOrDefault().Description) 'Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString

    Public ReadOnly Property MapsKey() As String
        Get
            Return NoCachePageBase.oMapsKey
        End Get
    End Property

    Public ReadOnly Property Language() As roLanguageWeb
        Get
            If Me.oLanguage Is Nothing Then Me.SetLanguage()
            Return Me.oLanguage
        End Get
    End Property

    Public Property OverrrideLanguageFile As String
        Get
            Return strOverrrideLanguageFile
        End Get
        Set(value As String)
            strOverrrideLanguageFile = value
        End Set
    End Property

    Public ReadOnly Property LanguageFile() As String
        Get
            Dim strLanguageFile As String
            If strOverrrideLanguageFile = String.Empty Then
                If Me.TemplateControl.AppRelativeTemplateSourceDirectory.StartsWith("~/Base/") Then
                    strLanguageFile = ConfigurationManager.AppSettings("LanguageBaseFile")
                Else
                    strLanguageFile = ConfigurationManager.AppSettings("LanguageFile")
                End If
            Else
                strLanguageFile = strOverrrideLanguageFile
            End If

            Return strLanguageFile
        End Get
    End Property

    Public Property OverrrideDefaultScope As String
        Get
            Return strOverridableScope
        End Get
        Set(value As String)
            strOverridableScope = value
        End Set
    End Property

    Public ReadOnly Property DefaultScope() As String
        Get
            If strOverridableScope = String.Empty Then
                Dim strScope As String
                If Me.TemplateControl IsNot Nothing Then
                    strScope = System.IO.Path.GetFileNameWithoutExtension(Me.TemplateControl.AppRelativeVirtualPath)
                ElseIf Me.Form IsNot Nothing Then
                    strScope = Me.Form.ID
                ElseIf Me.Header IsNot Nothing Then
                    strScope = Me.Header.ID
                Else
                    strScope = Me.UniqueID
                End If
                Return strScope
            Else
                Return strOverridableScope
            End If

        End Get
    End Property

    Public CssIncludes As String() = {
        "~/Base/Styles/dx.common.css",
        "~/Base/Styles/dx.robotics.main.css",
        "~/Base/Styles/dx.robotics.css",
        "~/Base/Styles/FontAwesone/css/font-awesome.min.css",
        "~/Base/Styles/" & WLHelperWeb.ThemeUsed,
        "~/Base/ext-3.4.0/resources/css/ext-all.css",
        "~/Base/jquerylayout/layout-default-latest.css",
        "~/Base/jquery/ui-lightness/jquery-ui.css",
        "~/Base/Styles/fontawesome-stars.css"
    }

    Private Sub NoCachePageBase_Init(sender As Object, e As EventArgs) Handles Me.Init
        Dim oCallbackCollection As New Generic.List(Of ASPxCallback)
        HelperWeb.GetControlList(Of ASPxCallback)(Me.Controls, oCallbackCollection)

        For Each oControl As ASPxCallback In oCallbackCollection
            Dim bMapScript As Boolean = True

            If oControl.ClientSideEvents.EndCallback = String.Empty Then
                oControl.ClientSideEvents.EndCallback = "function(s,e) { Robotics.Client.JSErrors.showDxErrorPopup(s,e); }"
            ElseIf oControl.ClientSideEvents.CallbackComplete = String.Empty Then
                oControl.ClientSideEvents.CallbackComplete = "function(s,e) { Robotics.Client.JSErrors.showDxErrorPopup(s,e); }"
                bMapScript = False
            End If

            If bMapScript Then
                If oControl.JSProperties.ContainsKey("cpJsErrorScript") Then
                    oControl.JSProperties("cpJsErrorScript") = String.Empty
                Else
                    oControl.JSProperties.Add("cpJsErrorScript", String.Empty)
                End If
            End If
        Next

        Robotics.Web.Base.HelperWeb.CreateCookie("serverURL", "/" & Configuration.RootUrl & "/", True)

    End Sub

#Region "Javascripts functions"

    Public Sub InsertJavascriptIncludes(Optional ByVal caller As System.Web.UI.Page = Nothing)
        Dim oPage As System.Web.UI.Page
        Dim script As ClientScriptManager

        If caller Is Nothing Then
            oPage = Me.Page
            script = oPage.ClientScript
        Else
            oPage = caller
            script = oPage.ClientScript
        End If

        If Not script.IsClientScriptIncludeRegistered(GetType(String), "BrowserDetect") Then script.RegisterClientScriptInclude(GetType(String), "BrowserDetect", oPage.ResolveUrl("~/Base/Scripts/BrowserDetect.js") & NoCachePageBase.MasterVersion)
        If Not script.IsClientScriptIncludeRegistered(GetType(String), "Ajax") Then script.RegisterClientScriptInclude(GetType(String), "Ajax", oPage.ResolveUrl("~/Base/Scripts/Ajax.js") & NoCachePageBase.MasterVersion)
        If Not script.IsClientScriptIncludeRegistered(GetType(String), "ContextMenu") Then script.RegisterClientScriptInclude(GetType(String), "ContextMenu", oPage.ResolveUrl("~/Base/Scripts/ContextMenu.js") & NoCachePageBase.MasterVersion)
        If Not script.IsClientScriptIncludeRegistered(GetType(String), "ConvertControls") Then script.RegisterClientScriptInclude(GetType(String), "ConvertControls", oPage.ResolveUrl("~/Base/Scripts/extConvertControls.js") & NoCachePageBase.MasterVersion)
        If Not script.IsClientScriptIncludeRegistered(GetType(String), "Cookies") Then script.RegisterClientScriptInclude(GetType(String), "Cookies", oPage.ResolveUrl("~/Base/Scripts/Cookies.js") & NoCachePageBase.MasterVersion)
        If Not script.IsClientScriptIncludeRegistered(GetType(String), "OpenWindow") Then script.RegisterClientScriptInclude(GetType(String), "OpenWindow", oPage.ResolveUrl("~/Base/Scripts/OpenWindow.js") & NoCachePageBase.MasterVersion)
        If Not script.IsClientScriptIncludeRegistered(GetType(String), "aesC") Then script.RegisterClientScriptInclude(GetType(String), "aesC", oPage.ResolveUrl("~/Base/Scripts/aes.js") & NoCachePageBase.MasterVersion)
        If Not script.IsClientScriptIncludeRegistered(GetType(String), "rgbColor") Then script.RegisterClientScriptInclude(GetType(String), "rgbColor", oPage.ResolveUrl("~/Base/Scripts/rgbcolor.js") & NoCachePageBase.MasterVersion)

        ' --------------- JQuery Librerias ------------------------------
        If Not script.IsClientScriptIncludeRegistered(GetType(String), "jquery") Then script.RegisterClientScriptInclude(GetType(String), "jquery", oPage.ResolveUrl("~/Base/jquery/jquery-3.7.1.min.js") & NoCachePageBase.MasterVersion)
        If Not script.IsClientScriptIncludeRegistered(GetType(String), "jquery-ui") Then script.RegisterClientScriptInclude(GetType(String), "jquery-ui", oPage.ResolveUrl("~/Base/jquery/jquery-ui.min.js") & NoCachePageBase.MasterVersion)
        If Not script.IsClientScriptIncludeRegistered(GetType(String), "jquery.contextmenu.r2") Then script.RegisterClientScriptInclude(GetType(String), "jquery.contextmenu.r2", oPage.ResolveUrl("~/Base/jquery/jquery.contextmenu.r2.js") & NoCachePageBase.MasterVersion)
        If Not script.IsClientScriptIncludeRegistered(GetType(String), "jquery-layout") Then script.RegisterClientScriptInclude(GetType(String), "jquery-layout", oPage.ResolveUrl("~/Base/jquerylayout/jquery.layout-latest.min.js") & NoCachePageBase.MasterVersion)

        If Not script.IsClientScriptIncludeRegistered(GetType(String), "quill") Then script.RegisterClientScriptInclude(GetType(String), "quill", oPage.ResolveUrl("~/Base/Scripts/Live/Communique/dx-quill.min.js") & NoCachePageBase.MasterVersion)

        ' ---------------JQuery Calendar V1 ------------------------
        If Not script.IsClientScriptIncludeRegistered(GetType(String), "jquery-fixedTableHeader") Then script.RegisterClientScriptInclude(GetType(String), "jquery-fixedTableHeader", oPage.ResolveUrl("~/Base/jquery/jquery.fixedTableHeader-0.03.js") & NoCachePageBase.MasterVersion)

        ' --------------  JSzip Library -------------------------------
        If Not script.IsClientScriptIncludeRegistered(GetType(String), "jszip") Then script.RegisterClientScriptInclude(GetType(String), "jszip", oPage.ResolveUrl("~/Scripts/jszip.min.js") & NoCachePageBase.MasterVersion)

        ' --------------  DevXtreme Library -------------------------------
        If Not script.IsClientScriptIncludeRegistered(GetType(String), "dxCldr") Then script.RegisterClientScriptInclude(GetType(String), "dxCldr", oPage.ResolveUrl("~/Scripts/cldr.min.js") & NoCachePageBase.MasterVersion)
        If Not script.IsClientScriptIncludeRegistered(GetType(String), "dxCldrEvent") Then script.RegisterClientScriptInclude(GetType(String), "dxCldrEvent", oPage.ResolveUrl("~/Scripts/cldr/event.min.js") & NoCachePageBase.MasterVersion)
        If Not script.IsClientScriptIncludeRegistered(GetType(String), "dxCldrSupplemental") Then script.RegisterClientScriptInclude(GetType(String), "dxCldrSupplemental", oPage.ResolveUrl("~/Scripts/cldr/supplemental.min.js") & NoCachePageBase.MasterVersion)
        If Not script.IsClientScriptIncludeRegistered(GetType(String), "dxCldrUnresolved") Then script.RegisterClientScriptInclude(GetType(String), "dxCldrUnresolved", oPage.ResolveUrl("~/Scripts/cldr/unresolved.min.js") & NoCachePageBase.MasterVersion)
        If Not script.IsClientScriptIncludeRegistered(GetType(String), "dxGlobalize") Then script.RegisterClientScriptInclude(GetType(String), "dxGlobalize", oPage.ResolveUrl("~/Scripts/globalize.min.js") & NoCachePageBase.MasterVersion)

        If Not script.IsClientScriptIncludeRegistered(GetType(String), "dxGlobalizeMessage") Then script.RegisterClientScriptInclude(GetType(String), "dxGlobalizeMessage", oPage.ResolveUrl("~/Scripts/globalize/message.min.js") & NoCachePageBase.MasterVersion)
        If Not script.IsClientScriptIncludeRegistered(GetType(String), "dxGlobalizeNumber") Then script.RegisterClientScriptInclude(GetType(String), "dxGlobalizeNumber", oPage.ResolveUrl("~/Scripts/globalize/number.min.js") & NoCachePageBase.MasterVersion)

        If Not script.IsClientScriptIncludeRegistered(GetType(String), "dxGlobalizeDate") Then script.RegisterClientScriptInclude(GetType(String), "dxGlobalizeDate", oPage.ResolveUrl("~/Scripts/globalize/date.min.js") & NoCachePageBase.MasterVersion)
        If Not script.IsClientScriptIncludeRegistered(GetType(String), "dxGlobalizeCurrency") Then script.RegisterClientScriptInclude(GetType(String), "dxGlobalizeCurrency", oPage.ResolveUrl("~/Scripts/globalize/currency.min.js") & NoCachePageBase.MasterVersion)

        If Not script.IsClientScriptIncludeRegistered(GetType(String), "devXtreme") Then script.RegisterClientScriptInclude(GetType(String), "devXtreme", oPage.ResolveUrl("~/Scripts/dx.all.js") & NoCachePageBase.MasterVersion)
        If Not script.IsClientScriptIncludeRegistered(GetType(String), "sugarjs") Then script.RegisterClientScriptInclude(GetType(String), "sugarjs", oPage.ResolveUrl("~/Base/globalize/sugar.min.js") & NoCachePageBase.MasterVersion)
        If Not script.IsClientScriptIncludeRegistered(GetType(String), "moment") Then script.RegisterClientScriptInclude(GetType(String), "moment", oPage.ResolveUrl("~/Base/Scripts/moment.min.js") & NoCachePageBase.MasterVersion)
        If Not script.IsClientScriptIncludeRegistered(GetType(String), "momenttz") Then script.RegisterClientScriptInclude(GetType(String), "momenttz", oPage.ResolveUrl("~/Base/Scripts/moment-tz.min.js") & NoCachePageBase.MasterVersion)
        If Not script.IsClientScriptIncludeRegistered(GetType(String), "roDate") Then script.RegisterClientScriptInclude(GetType(String), "roDate", oPage.ResolveUrl("~/Base/Scripts/Live/roDateManager.min.js") & NoCachePageBase.MasterVersion)
        If Not script.IsClientScriptIncludeRegistered(GetType(String), "dxInitialize") Then script.RegisterClientScriptInclude(GetType(String), "dxInitialize", oPage.ResolveUrl("~/Base/globalize/dx.Initialize.js") & NoCachePageBase.MasterVersion)
        If Not script.IsClientScriptIncludeRegistered(GetType(String), "dxCulture") Then script.RegisterClientScriptInclude(GetType(String), "dxCulture", oPage.ResolveUrl("~/Base/globalize/dx.InitializeCulture.js") & NoCachePageBase.MasterVersion)
        If Not script.IsClientScriptIncludeRegistered(GetType(String), "dxErrors") Then script.RegisterClientScriptInclude(GetType(String), "dxErrors", oPage.ResolveUrl("~/Base/globalize/dx.errors.js") & NoCachePageBase.MasterVersion)

        If Not script.IsClientScriptIncludeRegistered(GetType(String), "jquery.barrating") Then script.RegisterClientScriptInclude(GetType(String), "jquery.barrating", oPage.ResolveUrl("~/Base/jquery/jquery.barrating.min.js") & NoCachePageBase.MasterVersion)

        ' --------------  ExtJS Librerias -------------------------------
        If Not script.IsClientScriptIncludeRegistered(GetType(String), "ext-jquery-adapter") Then script.RegisterClientScriptInclude(GetType(String), "ext-jquery-adapter", oPage.ResolveUrl("~/Base/ext-3.4.0/ext-jquery-adapter.js") & NoCachePageBase.MasterVersion)
        If Not script.IsClientScriptIncludeRegistered(GetType(String), "ext-all") Then script.RegisterClientScriptInclude(GetType(String), "ext-all", oPage.ResolveUrl("~/Base/ext-3.4.0/ext-all.js") & NoCachePageBase.MasterVersion)

        ' --------------  Comun Librerias -------------------------------
        If Not script.IsClientScriptIncludeRegistered(GetType(String), "Generic") Then script.RegisterClientScriptInclude(GetType(String), "Generic", oPage.ResolveUrl("~/Base/Scripts/Generic.js") & NoCachePageBase.MasterVersion)
        If Not script.IsClientScriptIncludeRegistered(GetType(String), "GenericData") Then script.RegisterClientScriptInclude(GetType(String), "GenericData", oPage.ResolveUrl("~/Base/Scripts/GenericData.js") & NoCachePageBase.MasterVersion)

        If Not script.IsClientScriptIncludeRegistered(GetType(String), "roClassExtender") Then script.RegisterClientScriptInclude(GetType(String), "roClassExtender", oPage.ResolveUrl("~/Base/Scripts/roClassExtender.js") & NoCachePageBase.MasterVersion)
        If Not script.IsClientScriptIncludeRegistered(GetType(String), "roDevExpressStandards") Then script.RegisterClientScriptInclude(GetType(String), "roDevExpressStandards", oPage.ResolveUrl("~/Base/Scripts/roDevExpressStandards.js") & NoCachePageBase.MasterVersion)

        'ppr iDIOMA
        If Not script.IsClientScriptIncludeRegistered(GetType(String), "StartupLanguageExt") Then
            Dim ScriptLanguage As String = oPage.ResolveUrl("~/Base/ext-3.4.0/locale/ext-lang-" & WLHelperWeb.CurrentExtLanguage & ".js")
            script.RegisterClientScriptInclude(GetType(String), "StartupLanguageExt", ScriptLanguage & NoCachePageBase.MasterVersion)
        End If

        If (Not script.IsStartupScriptRegistered(GetType(String), "Startup")) Then
            Dim scriptString As String

            Dim scriptOnContinue As String = "function(){" & vbCrLf &
                            "InitConvertControls('" & oPage.ResolveUrl("~/Base/ext-3.4.0/locale/ext-lang-" & WLHelperWeb.CurrentExtLanguage & ".js") & NoCachePageBase.MasterVersion & "', '" & WLHelperWeb.CurrentExtDatePicketFormat() & "', " & WLHelperWeb.CurrentExtDatePicketStartDay() & "); " & vbCrLf &
                           "try{ PageBase_Load(); } catch(e) {} " & vbCrLf &
                           "try{ AddRequestEvents(); } catch(e) {} " & vbCrLf &
                           "try{ OverwriteDXTimeBehaviour(); } catch(e) {} " & vbCrLf &
                "}"

            scriptString = "<script type=""text/javascript"" language=""JavaScript""> " & vbCrLf &
                   "var hBaseRef = '" & oPage.ResolveUrl("~/Base/") & "';" & vbCrLf &
                   "var vtApplicationPath = '" & oPage.ResolveUrl("~") & "';" & vbCrLf &
                   "function pageLoad(sender, args){ " & vbCrLf &
                       "ContextMeunInit(); " & vbCrLf &
                       "try { " & vbCrLf &
                           "loadJSLanguages('" + oPage.ResolveUrl("~/Base/globalize/cldr/locales/likelySubtags.json").Replace("/globalize/cldr/locales/likelySubtags.json", "") + "','" + Robotics.VTBase.roTypes.Any2String(Reflection.Assembly.GetExecutingAssembly().GetCustomAttributes(GetType(Reflection.AssemblyDescriptionAttribute), False).FirstOrDefault().Description) + "', " & scriptOnContinue & " );" & vbCrLf &
                       "} catch(e) { /*showError('pageLoad', e);*/ } " & vbCrLf &
                       "try{ CloseIfNeeded(); } catch(e) {}" & vbCrLf &
                       "try{ $addHandler(document, ""keydown"", _KeyDown); } catch(e) {}" & vbCrLf &
                    "} " & vbCrLf &
                    "function pageUnload(sender, args){ " & vbCrLf &
                        "try {" & vbCrLf &
                            "PageBase_Unload(); " & vbCrLf &
                        "} catch(e) {} " & vbCrLf &
                        "try{ RefreshParentIfNeeded();  } catch(e) {}" & vbCrLf &
                    "} " & vbCrLf &
                    "</script>"

            script.RegisterStartupScript(GetType(String), "Startup", scriptString)
        End If

    End Sub

    Public Sub InsertExtraJavascript(ByVal scriptName As String, ByVal scriptPath As String, Optional ByVal caller As System.Web.UI.Page = Nothing, Optional ByVal includeOnHeader As Boolean = False, Optional ByVal includeVersion As Boolean = True)
        Dim oPage As System.Web.UI.Page
        Dim script As ClientScriptManager

        If caller Is Nothing Then
            oPage = Me.Page
            script = oPage.ClientScript
        Else
            oPage = caller
            script = oPage.ClientScript
        End If

        'If Not script.IsClientScriptIncludeRegistered(GetType(String), scriptName) Then script.RegisterClientScriptInclude(GetType(String), scriptName, oPage.ResolveUrl(scriptPath) & NoCachePageBase.MasterVersion)

        If Me.Page.Header IsNot Nothing AndAlso includeOnHeader = True Then
            Dim href As String = String.Empty

            If Not includeVersion Then
                href = oPage.ResolveUrl(scriptPath)
            Else
                href = oPage.ResolveUrl(scriptPath) & NoCachePageBase.MasterVersion
            End If

            Dim found = False
            For Each ctrl As Control In Me.Page.Header.Controls
                If TypeOf ctrl Is HtmlControls.HtmlGenericControl Then
                    If CType(ctrl, HtmlControls.HtmlGenericControl).Attributes("src") = href Then
                        found = True
                        Exit For
                    End If
                End If
            Next

            If Not found Then
                Dim js As New HtmlControls.HtmlGenericControl("script")
                js.Attributes.Add("type", "text/javascript")
                js.Attributes.Add("src", href)
                oPage.Header.Controls.Add(js)
            End If
        Else
            If Not includeVersion Then
                If Not script.IsClientScriptIncludeRegistered(GetType(String), scriptName) Then script.RegisterClientScriptInclude(GetType(String), scriptName, oPage.ResolveUrl(scriptPath))
            Else
                If Not script.IsClientScriptIncludeRegistered(GetType(String), scriptName) Then script.RegisterClientScriptInclude(GetType(String), scriptName, oPage.ResolveUrl(scriptPath) & NoCachePageBase.MasterVersion)
            End If

        End If

    End Sub

#End Region

#Region "Css function"

    Public Sub InsertCssIncludes(Optional ByVal caller As System.Web.UI.Page = Nothing)
        Dim oPage As System.Web.UI.Page

        If caller Is Nothing Then
            oPage = Me.Page
        Else
            oPage = caller
        End If

        Try
            If Not Me.Page.Header Is Nothing Then 'PPR: si la pagina no tiene header no se puede acceder (No provocamos la excepcion voluntariamente)

                For Each cCss As String In Me.CssIncludes

                    Dim href As String = oPage.Request.Url.GetLeftPart(UriPartial.Authority) & oPage.ResolveUrl(cCss) & NoCachePageBase.MasterVersion
                    Dim found = False
                    For Each ctrl As Control In oPage.Header.Controls
                        If TypeOf ctrl Is HtmlControls.HtmlLink Then
                            If CType(ctrl, HtmlControls.HtmlLink).Href = href Then
                                found = True
                                Exit For
                            End If
                        End If
                    Next

                    If Not found Then
                        Dim cHeader As HtmlControls.HtmlLink = New HtmlControls.HtmlLink()
                        cHeader.Href = href
                        cHeader.Attributes.Add("rel", "Stylesheet")
                        cHeader.Attributes.Add("type", "text/css")
                        oPage.Header.Controls.Add(cHeader)
                    End If
                Next

                Dim h2 As HtmlControls.HtmlLink = New HtmlControls.HtmlLink()
                h2.Href = Me.Page.ResolveUrl("~/Base/ext-3.4.0/resources/css/ext-all.css") & NoCachePageBase.MasterVersion
                h2.Attributes.Add("rel", "Stylesheet")
                h2.Attributes.Add("type", "text/css")
                oPage.Header.Controls.Add(h2)
            End If
        Catch
        End Try
    End Sub

    Public Sub InsertExtraCssIncludes(ByVal cssFile As String, Optional ByVal caller As System.Web.UI.Page = Nothing)
        Dim oPage As System.Web.UI.Page

        If caller Is Nothing Then
            oPage = Me.Page
        Else
            oPage = caller
        End If

        Try
            If Not oPage.Header Is Nothing Then 'PPR: si la pagina no tiene header no se puede acceder (No provocamos la excepcion voluntariamente)
                Dim href As String = oPage.Request.Url.GetLeftPart(UriPartial.Authority) & oPage.ResolveUrl(cssFile) & NoCachePageBase.MasterVersion
                Dim found = False
                For Each ctrl As Control In oPage.Header.Controls
                    If TypeOf ctrl Is HtmlControls.HtmlLink Then
                        If CType(ctrl, HtmlControls.HtmlLink).Href = href Then
                            found = True
                            Exit For
                        End If
                    End If
                Next

                If Not found Then
                    Dim cHeader As HtmlControls.HtmlLink = New HtmlControls.HtmlLink()
                    cHeader.Href = href
                    cHeader.Attributes.Add("rel", "Stylesheet")
                    cHeader.Attributes.Add("type", "text/css")
                    oPage.Header.Controls.Add(cHeader)
                End If
            End If
        Catch
        End Try
    End Sub

#End Region

#Region "Cryptography Javascript"

    Public Shared Function Encrypt(ByVal strText As String, Optional ByVal padding As PaddingMode = PaddingMode.PKCS7, Optional ByVal mode As CipherMode = CipherMode.CBC) As String
        Dim key() As Byte = {&H15, &H2A, &H32, &H43, &HB4, &H15, &H76, &H17, &HC8, &H1F, &H2A, &H6B, &H1C, &H2D, &H3E, &H4F}
        Dim IV() As Byte = {&H10, &H1A, &H12, &H64, &H14, &H15, &H16, &H17, &H13, &H39, &H1A, &H1C, &H1C, &H1D, &H9E, &H1F}
        Try
            If strText <> "" Then
                Dim InputByteArray() As Byte = System.Text.Encoding.UTF8.GetBytes(strText)
                Dim aes As New AesCryptoServiceProvider
                aes.Padding = padding
                aes.Mode = mode
                Dim ms As New MemoryStream
                Dim cs As New CryptoStream(ms, aes.CreateEncryptor(key, IV), CryptoStreamMode.Write)
                cs.Write(InputByteArray, 0, InputByteArray.Length)
                cs.FlushFinalBlock()
                Return Convert.ToBase64String(ms.ToArray())
            Else
                Return ""
            End If
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    Public Shared Function Decrypt(ByVal strText As String, Optional ByVal padding As PaddingMode = PaddingMode.PKCS7, Optional ByVal mode As CipherMode = CipherMode.CBC) As String
        Dim key() As Byte = {&H15, &H2A, &H32, &H43, &HB4, &H15, &H76, &H17, &HC8, &H1F, &H2A, &H6B, &H1C, &H2D, &H3E, &H4F}
        Dim IV() As Byte = {&H10, &H1A, &H12, &H64, &H14, &H15, &H16, &H17, &H13, &H39, &H1A, &H1C, &H1C, &H1D, &H9E, &H1F}
        Dim inputByteArray(strText.Length) As Byte
        Try
            If strText <> "" Then
                Dim aes As New AesCryptoServiceProvider
                aes.Padding = padding
                aes.Mode = mode
                inputByteArray = Convert.FromBase64String(strText)
                Dim ms As New MemoryStream
                Dim cs As New CryptoStream(ms, aes.CreateDecryptor(key, IV), CryptoStreamMode.Write)
                cs.Write(inputByteArray, 0, inputByteArray.Length)
                cs.FlushFinalBlock()
                Dim encoding As System.Text.Encoding = System.Text.Encoding.UTF8
                Return encoding.GetString(ms.ToArray())
            Else
                Return ""
            End If
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

#End Region

#Region "Language files"

    Protected Sub SetLanguage()

        'If WLHelperWeb.CurrentPassport IsNot Nothing Then
        Me.oLanguage = New roLanguageWeb
        WLHelperWeb.SetLanguage(Me.oLanguage, Me.LanguageFile)
        'End If

    End Sub

#End Region

End Class