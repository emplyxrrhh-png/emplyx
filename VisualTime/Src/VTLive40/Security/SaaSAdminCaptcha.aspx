<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Security_SaaSAdminCaptcha" CodeBehind="SaaSAdminCaptcha.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
        function CloseClick() {
            window.parent.PopupCaptcha_Client.SetContentUrl('');
            window.parent.PopupCaptcha_Client.Hide();
        }
        function AcceptClick() {
            var inputCaptcha = document.getElementById('panelUploadContent_c1').textContent;
            inputCaptcha = inputCaptcha + document.getElementById('panelUploadContent_c2').textContent;
            inputCaptcha = inputCaptcha + document.getElementById('panelUploadContent_c3').textContent;
            inputCaptcha = inputCaptcha + document.getElementById('panelUploadContent_c4').textContent;

            var userCaptcha = document.getElementById('panelUploadContent_txtCaptcha').value;

            if (inputCaptcha == userCaptcha) {
                window.parent.PerformAction();
                window.parent.PopupCaptcha_Client.SetContentUrl('');
                window.parent.PopupCaptcha_Client.Hide();
            } else {
                alert(document.getElementById('panelUploadContent_hdnErrorValue').value);
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <dx:ASPxPanel ID="panelUploadContent" runat="server" Width="0px" Height="0px">
            <PanelCollection>
                <dx:PanelContent ID="PopupUploadPanelContent" runat="server">
                    <div class="bodyPopupExtended" style="table-layout: fixed; height: 250px; width: 445px;">
                        <table width="100%">
                            <tr>
                                <td style="padding-top: 5px; padding-bottom: 10px;" height="20px" valign="top">
                                    <div class="panHeader2">
                                        <span style="">
                                            <asp:Label ID="lblTitle" runat="server" Text="Eliminar seguimiento de ausencia"></asp:Label></span>
                                    </div>
                                </td>
                            </tr>
                            <tr valign="top" style="height: 90px;">
                                <td>
                                    <table>
                                        <tr>
                                            <td valign="top" style="padding-left: 15px; padding-right: 15px;">
                                                <img alt="" id="Img1" src="~/Security/Images/VTSaas_32.png" runat="server" />
                                            </td>
                                            <td align="left" valign="middle">
                                                <asp:Label ID="lblDescription1" runat="server" CssClass="editTextFormat" Text="Se dispone a eliminar el seguimiento"></asp:Label>
                                                <br />
                                                <asp:Label ID="lblNameEmployee" runat="server" Font-Bold="True" Font-Size="12px" Height="20" Style="white-space: nowrap;" />
                                                <br />
                                                <asp:Label ID="lblDescription2" runat="server" CssClass="editTextFormat" Text="Si realmente lo desea hacer, introduzca los números mostrados a continuación y pulse el botón Aceptar."></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr style="height: 45px;" valign="top">
                                <td>
                                    <table style="padding-left: 95px;">
                                        <tr>
                                            <td>
                                                <table style="width: 100%" cellspacing="0" cellpadding="0" border="0">
                                                    <tr>
                                                        <td>
                                                            <input type="hidden" runat="server" id="hdnErrorValue" />
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

                        <table style="margin-left: auto; margin-right: auto">
                            <tr>
                                <td>
                                    <asp:Label ID="lblError" runat="server" Text="" Font-Bold="True" Font-Size="13px" Height="30" Style="white-space: nowrap; color: Red" />
                                </td>
                            </tr>
                        </table>
                        <table style="margin-left: auto;">
                            <tr>
                                <td>
                                    <dx:ASPxButton ID="btnAccept" runat="server" AutoPostBack="false" CausesValidation="False" Image-Url="~/Base/Images/Grid/button_ok.png"
                                        Text="${Button.Accept}" Width="121px" Height="24px"
                                        CssClass="editTextFormat" Border-BorderColor="Transparent" BackColor="Transparent" FocusRectBorder-BorderStyle="None">
                                        <Image Url="~/Base/Images/Grid/button_ok.png"></Image>
                                        <ClientSideEvents Click="AcceptClick" />
                                        <HoverStyle BackgroundImage-ImageUrl="~/Base/Images/Buttons/DevButtonHover120.png" />
                                        <BackgroundImage ImageUrl="~/Base/Images/Buttons/DevButton120.png" VerticalPosition="center" Repeat="NoRepeat" />
                                        <PressedStyle BackgroundImage-ImageUrl="~/Base/Images/Buttons/DevButtonClick120.png" ForeColor="White" />
                                        <FocusRectBorder BorderStyle="Groove" BorderWidth="1px" BorderColor="ActiveBorder" />
                                    </dx:ASPxButton>
                                </td>
                                <td>
                                    <dx:ASPxButton ID="btnCancel" runat="server" AutoPostBack="False" CausesValidation="False" Image-Url="~/Base/Images/Grid/button_cancel.png" Text="${Button.Cancel}" Width="121px" Height="24px"
                                        CssClass="editTextFormat" Border-BorderColor="Transparent" BackColor="Transparent" FocusRectBorder-BorderStyle="None">
                                        <Image Url="~/Base/Images/Grid/button_cancel.png"></Image>
                                        <ClientSideEvents Click="CloseClick" />
                                        <HoverStyle BackgroundImage-ImageUrl="~/Base/Images/Buttons/DevButtonHover120.png" />
                                        <BackgroundImage ImageUrl="~/Base/Images/Buttons/DevButton120.png" VerticalPosition="center" Repeat="NoRepeat" />
                                        <PressedStyle BackgroundImage-ImageUrl="~/Base/Images/Buttons/DevButtonClick120.png" ForeColor="White" />
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