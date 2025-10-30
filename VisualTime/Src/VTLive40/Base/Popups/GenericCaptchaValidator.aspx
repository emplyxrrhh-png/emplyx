<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Base_GenericCaptchaValidator" CodeBehind="GenericCaptchaValidator.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">

        function CloseClick(s, e) {

            var clientPopupName = document.getElementById("hdnPopupName").value;
            var clientPopup = null;
            if (clientPopupName == "") {
                clientPopup = window.parent.CaptchaObjectPopup_Client;
            } else {
                clientPopup = eval('window.parent.' + clientPopupName);
            }

            clientPopup.SetContentUrl('');
            clientPopup.Hide();
        }

        function AcceptClick(s, e) {
            var inputCaptcha = document.getElementById('panelUploadContent_c1').textContent;
            inputCaptcha = inputCaptcha + document.getElementById('panelUploadContent_c2').textContent;
            inputCaptcha = inputCaptcha + document.getElementById('panelUploadContent_c3').textContent;
            inputCaptcha = inputCaptcha + document.getElementById('panelUploadContent_c4').textContent;

            var userCaptcha = document.getElementById('panelUploadContent_txtCaptcha').value;

            if (inputCaptcha != userCaptcha) {
                var callbackFuncName = document.getElementById("hdnCallbackName").value;

                if (callbackFuncName == "") {
                    callbackFunc = window.parent.captchaCallback;
                } else {
                    callbackFunc = eval('window.parent.' + callbackFuncName);
                }

                callbackFunc("ERROR");
            } else {
                AuditCallbackClient.PerformCallback("AUDIT");
            }
        }

        function AuditCallback_CallbackComplete(s, e) {
            var captchaAction = document.getElementById("hdnCaptchaAction").value
            var callbackFuncName = document.getElementById("hdnCallbackName").value;
            var clientPopupName = document.getElementById("hdnPopupName").value;

            var clientPopup = null;
            var callbackFunc = null;

            if (clientPopupName == "") {
                clientPopup = window.parent.CaptchaObjectPopup_Client;
            } else {
                clientPopup = eval('window.parent.' + clientPopupName);
            }

            if (callbackFuncName == "") {
                callbackFunc = window.parent.captchaCallback;
            } else {
                callbackFunc = eval('window.parent.' + callbackFuncName);
            }

            clientPopup.SetContentUrl('');
            clientPopup.Hide();

            callbackFunc(captchaAction);
        }
    </script>
</head>
<body>
    <input type="hidden" runat="server" id="hdnCaptchaAction" />
    <input type="hidden" runat="server" id="hdnPopupName" />
    <input type="hidden" runat="server" id="hdnCallbackName" />

    <form id="form1" runat="server">

        <dx:ASPxCallback ID="AuditCallback" runat="server" ClientInstanceName="AuditCallbackClient" ClientSideEvents-CallbackComplete="AuditCallback_CallbackComplete"></dx:ASPxCallback>
        <dx:ASPxPanel ID="panelUploadContent" runat="server" Width="0px" Height="0px">
            <PanelCollection>
                <dx:PanelContent ID="PopupUploadPanelContent" runat="server">
                    <div class="bodyPopupExtended" style="table-layout: fixed; height: 235px; left: 4px; width: 470px; position: relative; top: 10px;">
                        <table width="100%">
                            <tr>
                                <td style="padding-top: 5px; padding-bottom: 10px;" height="20px" valign="top">
                                    <div class="panHeader2">
                                        <span style="">
                                            <asp:Label ID="lblValidationCode" runat="server" Text="Validación"></asp:Label>
                                        </span>
                                    </div>
                                </td>
                            </tr>
                            <tr valign="top">
                                <td>
                                    <table>
                                        <tr>
                                            <td valign="top" style="padding-left: 15px; padding-right: 15px;">
                                                <img alt="" id="Img2" width="48" height="48" src="~/Base/Images/logovtl.ico" runat="server" />
                                            </td>
                                            <td align="left" valign="middle">
                                                <asp:Label ID="lblCaptchaDesc1" runat="server" CssClass="editTextFormat" Text="Introduzca el código de seguridad mostrado abajo y pulse aceptar para continuar"></asp:Label>
                                                <br />
                                                <br />
                                                <div class="divCaptchaDesc2" cssclass="editTextFormat">
                                                    <span id="spanCaptchaDesc2" runat="server">
                                                        <%--                                                        <asp:Label ID="Label1" runat="server" Text="Validación"></asp:Label>--%>
                                                    </span>
                                                </div>
                                                <br />
                                                <%--                                                <asp:Label ID="lblCaptchaDesc2" runat="server" CssClass="editTextFormat" Text=""></asp:Label>--%>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr valign="top">
                                <td>
                                    <table style="padding-left: 95px;">
                                        <tr>
                                            <td>
                                                <table>
                                                    <tr>
                                                        <td>
                                                            <table style="width: 100%" cellspacing="0" cellpadding="0" border="0">
                                                                <tr>
                                                                    <td>
                                                                        <asp:Label ID="c1" runat="server" Text="" BackColor="#C9C9C9" BorderColor="#484848" BorderStyle="Solid" BorderWidth="1" Font-Bold="True" Font-Size="X-Large" Height="30" Width="30" Style="text-align: center;" />
                                                                    </td>
                                                                    <td>
                                                                        <asp:Label ID="c2" runat="server" Text="" BackColor="#C9C9C9" BorderColor="#484848" BorderStyle="Solid" BorderWidth="1" Font-Bold="True" Font-Size="X-Large" Height="30" Width="30" Style="text-align: center;" />
                                                                    </td>
                                                                    <td>
                                                                        <asp:Label ID="c3" runat="server" Text="" BackColor="#C9C9C9" BorderColor="#484848" BorderStyle="Solid" BorderWidth="1" Font-Bold="True" Font-Size="X-Large" Height="30" Width="30" Style="text-align: center;" />
                                                                    </td>
                                                                    <td>
                                                                        <asp:Label ID="c4" runat="server" Text="" BackColor="#C9C9C9" BorderColor="#484848" BorderStyle="Solid" BorderWidth="1" Font-Bold="True" Font-Size="X-Large" Height="30" Width="30" Style="text-align: center;" />
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                        <td align="left" valign="middle" style="padding-left: 40px;">
                                                            <input type="text" id="txtCaptcha" value="" runat="server" maxlength="4" class="RoundCorner captchaStyle defaultContrastColor" convertcontrol="textField" ccallowblank="false" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <br />
                        <table style="margin-left: auto;">
                            <tr>
                                <td>
                                    <dx:ASPxButton ID="btnAccept" runat="server" AutoPostBack="False" CausesValidation="False" Text="${Button.Accept}" ToolTip="${Button.Accept}"
                                        HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                        <ClientSideEvents Click="AcceptClick" />
                                    </dx:ASPxButton>
                                </td>
                                <td>
                                    <dx:ASPxButton ID="btnCancel" runat="server" AutoPostBack="False" CausesValidation="False" Text="${Button.Cancel}" ToolTip="${Button.Cancel}"
                                        HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                        <ClientSideEvents Click="CloseClick" />
                                    </dx:ASPxButton>
                                </td>
                            </tr>
                        </table>
                    </div>
                </dx:PanelContent>
            </PanelCollection>
        </dx:ASPxPanel>
    </form>
</body>
</html>