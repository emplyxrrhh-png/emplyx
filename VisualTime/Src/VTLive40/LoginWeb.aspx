<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.LoginWeb" CodeBehind="LoginWeb.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="robots" content="noindex" />
    <meta name="googlebot" content="noindex" />
    <meta name="robots" content="nofollow" />
    <meta name="robots" content="nosnippet" />
    <meta name="robots" content="noarchive" />

    <title>VisualTime Live</title>
    <link rel="shortcut icon" href="~/Base/Images/logovtl.ico" />
</head>
<body onload="ShowMessageOutdatedVisibleIfNecessary();CheckRedirectAfterLogin()">

    <script type="text/javascript" language="javascript">
        function openRecoverPasswordPopup() {
            RecoverPasswordPopUp_Client.Show();
        }
    </script>

    <form id="frmLogin" method="post" runat="server" autocomplete="off">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />
        <div class="loginFull">
            <div class="rbBackgroundImg" id="rbBackground" runat="server"></div>
            <div class="rbLoginColumn">
                <div class="rbLogoInformation LogoLogin">
                    <div id="vtLogoVersionDiv" class="tbd_bar_textVersion" translate="no">
                        <asp:Image ID="titleVT" runat="server" ImageUrl="~/Base/Images/Logo_VT5.png" Width="75%" Height="75%" />
                        <asp:Image ID="titleVTOne" runat="server" ImageUrl="~/Base/Images/Logo_VTOne02.png" Width="50%" Height="50%" />
                        <p align="right" style="padding-right: 27%;">
                            <asp:Label ID="versionVTOne" Text="R4" runat="server" />
                        </p>
                    </div>
                </div>
                <div class="rbLoginData">
                    <asp:UpdatePanel ID="LoginUpdatePanel" runat="server">
                        <contenttemplate>
                            <asp:HiddenField ID="HiddenFieldRedirectUrl" runat="server" />
                            <asp:HiddenField ID="hdnOSVersion" runat="server" />
                            <input type="hidden" value="<%= Me.Language.Translate("Error.AlreadyLoggedinInOtherLocation", Me.DefaultScope)%>" id="AlreadyLoggedinInOtherLocation" />
                            <input type="hidden" value="<%= Me.Language.Translate("Error.SessionInvalidatedByPermissions", Me.DefaultScope)%>" id="SessionInvalidatedByPermissions" />
                            <input type="hidden" value="<%= Me.Language.Translate("Error.SessionExpired", Me.DefaultScope)%>" id="SessionExpired" />
                            <input type="hidden" value="<%= Me.Language.Translate("Error.UpdateSessionError", Me.DefaultScope)%>" id="UpdateSessionError" />

                            <table id="table" align="center" width="320" style="background-color: #f3f6ff !important" class="PopUpFrame_Center" border="0" cellpadding="2" cellspacing="1">
                                <tr id="trVersionNotSuportedBrowser" style="display: none;" runat="server">
                                    <td align="center" style="height: 60px;">
                                        <table border="0" cellpadding="0" cellspacing="0">
                                            <tr>
                                                <td align="center" colspan="2">
                                                    <asp:Label ID="lblInvalidBrowser" runat="server" ForeColor="Blue" Text="Invalid browser"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr id="trVersionNotRecognizedBrowser" style="display: none;" runat="server">
                                    <td align="center" style="height: 60px;">
                                        <table border="0" cellpadding="0" cellspacing="0">
                                            <tr>
                                                <td align="center" colspan="2">
                                                    <asp:Label ID="lblUnrecognizedBrowser" runat="server" ForeColor="Blue" Text="Invalid browser"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr id="trValidBrowser" style="" runat="server">
                                    <td>
                                        <table border="0" cellpadding="0" cellspacing="0">
                                            <tr>
                                                <td colspan="2" align="center" style="padding-top: 20px; padding-bottom: 10px;">
                                                    <asp:Label ID="InvalidPassword" runat="server" CssClass="errorText" Text="Nombre o contrase&ntilde;a incorrectos" Visible="False"></asp:Label>
                                                    <asp:Label ID="lblTitle" runat="server" Text="Introduce tu usuario y contrase&ntilde;a" /><br />
                                                    <asp:Label ID="lblResult" runat="server" Font-Bold="true" CssClass="errorText" Visible="False"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr runat="server" id="trMultitennantServer" style="display: none;">
                                                <td class="labeltext" style="padding-left: 10px;">
                                                    <asp:Label ID="lblServer" Text="Servidor:" runat="server" />
                                                </td>
                                                <td class="labeltext" style="padding: 1px; padding-left: 10px;">
                                                    <asp:TextBox ID="ServerText" Style="border-radius: 5px; height: 25px; padding-left: 8px;" runat="server" TabIndex="1" CssClass="textClass x-form-text x-form-field loginInput" OnKeypress="return KeyPressFunction(event);" Width="195px" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="labeltext" style="padding-left: 10px; padding-top: 5px;">
                                                    <asp:Label ID="lblName" Text="Usuario:" runat="server" />
                                                </td>
                                                <td class="labeltext" style="padding: 1px; padding-left: 10px; padding-top: 5px;">
                                                    <asp:TextBox ID="UserName" Style="border-radius: 5px; height: 25px; padding-left: 8px;" runat="server" TabIndex="1" CssClass="textClass x-form-text x-form-field loginInput" onChange="return false;" OnKeypress="return KeyPressFunction(event);" Width="195px" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="labeltext" style="padding-left: 10px; padding-top: 5px;">
                                                    <asp:Label ID="lblPassword" Text="Contrase&ntilde;a:" runat="server" />
                                                </td>
                                                <td class="labeltext" style="padding: 1px; padding-left: 10px; padding-top: 5px;">
                                                    <asp:TextBox ID="Password" Style="border-radius: 5px; height: 25px; padding-left: 8px;" runat="server" TextMode="Password" TabIndex="2" CssClass="textClass x-form-text x-form-field loginInput" OnKeypress="return KeyPressFunction(event);" Width="195px" AutoCompleteType="Disabled" />
                                                </td>
                                            </tr>
                                            <tr runat="server" id="trRoboticsMail" style="display: none;">
                                                <td class="labeltext" style="padding-left: 10px; padding-top: 5px;">
                                                    <asp:Label ID="lblRoboticsMail" Text="Dirección de correo:" runat="server" />
                                                </td>
                                                <td class="labeltext" style="padding: 1px; padding-left: 10px; padding-top: 5px;">
                                                    <asp:TextBox ID="txtRoboticsMail" Style="border-radius: 5px; height: 25px; padding-left: 8px;" runat="server" TabIndex="4" CssClass="textClass x-form-text x-form-field loginInput" OnKeypress="return KeyPressFunction(event);" Width="195px" />
                                                </td>
                                            </tr>
                                            <tr runat="server" id="trValidationCode" style="display: none;">
                                                <td class="labeltext" style="padding-left: 10px; padding-top: 5px;">
                                                    <asp:Label ID="lblValidationCode" Text="Código de validación:" runat="server" />
                                                </td>
                                                <td class="labeltext" style="padding: 1px; padding-left: 10px; padding-top: 5px;">
                                                    <asp:TextBox ID="txtValidationCode" Style="border-radius: 5px; height: 25px; padding-left: 8px;" runat="server" TextMode="Password" TabIndex="5" CssClass="textClass x-form-text x-form-field loginInput" OnKeypress="return KeyPressFunction(event);" Width="195px" AutoCompleteType="Disabled" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="labeltext" style="padding-left: 10px; padding-top: 30px;" valign="middle">
                                                    <asp:Label ID="lblLanguage" Text="Idioma:" runat="server" />
                                                </td>
                                                <td class="labeltext" style="padding: 1px; padding-left: 10px; padding-top: 20px;">
                                                    <rowebcontrols:rocombobox id="cmbLanguage" style="height: 25px; padding-left: 5px;" runat="server" parentwidth="162px" autoresizechildswidth="true" hiddentext="cmbLanguage_Text" hiddenvalue="cmbLanguage_Value" childsvisible="4"></rowebcontrols:rocombobox>
                                                    <input type="hidden" id="cmbLanguage_Text" runat="server" />
                                                    <input type="hidden" id="cmbLanguage_Value" runat="server" />
                                                    <asp:Button ID="btChangeLanguage" runat="server" Style="display: none;" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2" align="center" style="height: 26px;">
                                                    <asp:UpdateProgress ID="LoginUpdateProgress" runat="server" AssociatedUpdatePanelID="LoginUpdatePanel">
                                                        <progresstemplate>
                                                            <asp:Image ID="Image2" runat="server" ImageUrl="Base/Images/loading.gif"></asp:Image>
                                                        </progresstemplate>
                                                    </asp:UpdateProgress>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2" align="center" style="padding-left: 10px; padding-top: 5px; padding-bottom: 5px;">
                                                    <div id="acceptButton" runat="server" style="float: left">
                                                        <asp:Button ID="btAccept" Text="Aceptar" runat="server" TabIndex="3" Style="border: 0px !important" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                                    </div>
                                                    <div id="ssoAcceptButton" runat="server" style="float: right">
                                                        <asp:Button ID="btnRedirectSSO" Text="Iniciar sesión con SSO" runat="server" TabIndex="3" Style="background-color: #7b91ff !important; border: 0px !important" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2" align="center" style="padding-top: 10px; padding-bottom: 10px; cursor: pointer;">
                                                    <asp:Label ID="recoverPassword" runat="server" Text="¿Has olvidado tu contraseña?" Visible="true" onclick="openRecoverPasswordPopup()"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </contenttemplate>
                    </asp:UpdatePanel>
                </div>
                <div class="rbTextClass">
                    <div style="height: 50px">
                        <a href="https://www.cegid.com/ib/es/productos/cegid-visualtime/">©2025 Visualtime</a>
                    </div>
                </div>
            </div>
        </div>
        <!-- POPUP NEW OBJECT -->
        <dx:aspxpopupcontrol id="RecoverPasswordPopUp" text="" runat="server" allowdragging="False" closeaction="OuterMouseClick" modal="True"
            popupverticalalign="WindowCenter" popuphorizontalalign="WindowCenter" minwidth="470px" width="470px" minheight="310px" height="310px" cssclass="bodyPopupExtended"
            showheader="False" scrollbars="Auto" showpagescrollbarwhenmodal="True" clientinstancename="RecoverPasswordPopUp_Client" popupanimationtype="None" contentstyle-paddings-padding="0px" showshadow="false">

            <contentcollection>
                <dx:popupcontrolcontentcontrol id="PopupConceptsControlContent" runat="server">
                    <div style="width: 90%">
                        <div style="width: 100%; margin: 0px !important" class="panHeader2 panBottomMargin">
                            <span class="panelTitleSpan">
                                <asp:Label runat="server" ID="lblTypeEmployee" Text="Recuperar contraseña"></asp:Label>
                            </span>
                        </div>

                        <div class="panelHeaderContent">
                            <div style="padding-left: 10px; padding-top: 5px; color: #2D4155;">
                                <asp:Label ID="lblRecoverPasswordDesc" runat="server" Text="Indica tu nombre de usuario y correo electrónico. Al pulsar sobre el botón Siguiente se te enviará un mensaje con la clave de recuperación que deberás introducir para poder restablecer la contraseña."></asp:Label>
                            </div>
                        </div>

                        <div class="divRow" style="margin-top: 25px" runat="server" id="divRecoverUser">
                            <div style="margin-top: 6px; float: left; margin-right: 10px; margin-left: 15px; color: #2D4155;">
                                <asp:Label ID="lblUserName" runat="server" Text="Usuario:"></asp:Label>
                            </div>
                            <div class="componentForm">
                                <dx:aspxtextbox id="txtUser" runat="server" width="280px" clientinstancename="txtUser_Client" nulltext="">
                                </dx:aspxtextbox>
                            </div>
                        </div>

                        <div class="divRow" runat="server" style="margin-top: 15px" id="divRecoverMail">
                            <div style="margin-top: 6px; float: left; margin-right: 10px; margin-left: 15px; color: #2D4155;">
                                <asp:Label ID="lblEmail" runat="server" Text="Correo:"></asp:Label>
                            </div>
                            <div class="componentForm">
                                <dx:aspxtextbox id="txtEmail" runat="server" width="280px" clientinstancename="lblEmail_Client" nulltext="">
                                </dx:aspxtextbox>
                            </div>
                        </div>

                        <div class="divRow" style="margin-top: 25px" runat="server" id="divRecoverCode" visible="false">
                            <div style="margin-top: 6px; float: left; margin-right: 10px; margin-left: 15px; color: #2D4155;">
                                <asp:Label ID="lblCodeName" runat="server" Text="Código:"></asp:Label>
                            </div>
                            <div class="componentForm">
                                <dx:aspxtextbox id="txtCode" runat="server" width="280px" clientinstancename="txtCode_Client" nulltext="">
                                </dx:aspxtextbox>
                            </div>
                        </div>

                        <div class="divRow" runat="server" style="margin-top: 15px" id="divRecoverNewPassword" visible="false">
                            <div style="margin-top: 6px; float: left; margin-right: 10px; margin-left: 15px; color: #2D4155;">
                                <asp:Label ID="lblNewPassword" runat="server" Text="Nueva contraseña:"></asp:Label>
                            </div>
                            <div class="componentForm">
                                <dx:aspxtextbox id="txtNewPassword" runat="server" width="221px" clientinstancename="lblNewPassword_Client" nulltext="" password="true">
                                </dx:aspxtextbox>
                            </div>
                        </div>
                        <br />
                        <div id="divbtnRecoverPasswordNext" runat="server" style="display: table; margin: 0 auto;">
                            <asp:Button ID="btnRecoverPasswordNext" Text="Siguiente" runat="server" Style="background-color: #FF5C35 !important; border: 0px !important" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                        </div>

                        <div id="divbtnRecoverPasswordEnd" runat="server" visible="false" style="margin: 0 auto; display: table;">

                            <div id="divbtnRecoverPasswordBack" runat="server" style="display: inline-block; margin: 0 auto;">
                                <asp:Button ID="btnRecoverPasswordBack" Text="Volver" runat="server" Style="background-color: #FF5C35 !important; border: 0px !important" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                            </div>

                            <div id="divbtnRecoverPasswordFinish" runat="server" style="display: inline-block; margin: 0 auto;">
                                <asp:Button ID="btnRecoverPasswordFinish" Text="Finalizar" runat="server" Style="background-color: #FF5C35 !important; border: 0px !important" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                            </div>
                        </div>

                        <div id="errorDiv" style="display: table; margin: 0 auto; text-align: center; margin-top: -5px;" runat="server" visible="false">
                            <br />
                            <div style="color: #ff0000;">
                                <asp:Label ID="errorText" runat="server" Text=""></asp:Label>
                            </div>
                        </div>
                    </div>
                </dx:popupcontrolcontentcontrol>
            </contentcollection>

            <clientsideevents />
            <settingsloadingpanel enabled="false" />
        </dx:aspxpopupcontrol>

        <local:msgboxform id="MsgBoxForm1" runat="server" />
        <local:messageframe id="MessageFrame1" runat="server" />
    </form>
</body>
</html>