<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Employees_DeleteEmployeeCaptcha" CodeBehind="DeleteEmployeeCaptcha.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">

        function CloseClick(s, e) {
            Close();
            return false;
        }

        function AcceptClick(s, e) {
            var inputCaptcha = document.getElementById('c1').textContent;
            inputCaptcha = inputCaptcha + document.getElementById('c2').textContent;
            inputCaptcha = inputCaptcha + document.getElementById('c3').textContent;
            inputCaptcha = inputCaptcha + document.getElementById('c4').textContent;

            var userCaptcha = document.getElementById('txtCaptcha').value;

            if (inputCaptcha != userCaptcha) {
                var url = "Employees/srvMsgBoxEmployees.aspx?action=MessageEx";
                url = url + "&TitleKey=CommonError.Title";
                url = url + "&DescriptionText=" + encodeURIComponent(document.getElementById('hdnErrorValue').value);
                url = url + "&Option1TextKey=CommonError.Option1Text";
                url = url + "&Option1DescriptionKey=CommonError.Option1Description";
                url = url + "&Option1OnClickScript=HideMsgBoxForm();return false;";
                url = url + "&IconUrl=~/Base/Images/MessageFrame/stock_dialog-error.png";

                window.parent.parent.ShowMsgBoxForm(url, 400, 300, '');
                //alert(document.getElementById('panelUploadContent_hdnErrorValue').value);
            } else {
                var idEmployee = window.parent.frames["ifPrincipal"].actualEmployee;
                window.parent.frames["ifPrincipal"].removeEmployee(idEmployee);
                Close();
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />
        <div>
        </div>

        <asp:HiddenField ID="hdnCanClose_PageBase" runat="server" />
        <asp:HiddenField ID="hdnMustRefresh_PageBase" Value="0" runat="server" />
        <asp:HiddenField ID="hdnParams_PageBase" runat="server" />

        <table width="100%">
            <tr>
                <td style="padding-top: 5px; padding-bottom: 10px;" height="20px" valign="top">
                    <div class="panHeader2">
                        <span style="">
                            <asp:Label ID="lblTitle" runat="server" Text="Eliminar empleado"></asp:Label></span>
                    </div>
                </td>
            </tr>
            <tr valign="top" style="height: 90px;">
                <td>
                    <table>
                        <tr>
                            <td valign="top" style="padding-left: 15px; padding-right: 15px;">
                                <img alt="" id="Img1" src="Images/Employee-Old-32x32.gif" runat="server" />
                            </td>
                            <td align="left" valign="middle">
                                <asp:Label ID="lblDescription1" runat="server" CssClass="editTextFormat" Text="Se dispone a eliminar todos los datos del empleado"></asp:Label>
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
    </form>
</body>
</html>