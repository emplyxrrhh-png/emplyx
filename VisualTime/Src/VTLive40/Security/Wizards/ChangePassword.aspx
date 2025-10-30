<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Security_ChangePassword" CodeBehind="ChangePassword.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
</script>
</head>
<body class="bodyPopup" style="background-attachment: fixed;">

    <div>

        <script language="javascript" type="text/javascript">

            function PageBase_Load() {

                hidePopupLoader();
                var mustClose = document.getElementById('hdnMustClose').value;

                if (mustClose == "1") Close();

                var txtOldPassword = document.getElementById('MainMenu_ropfChangePwd_txtOldPwd');
                if (txtOldPassword != null) {
                    txtOldPassword.focus();
                }

            }

            function OnKeyDown_PageBase(e) {
                if (e && e.keyCode == Sys.UI.Key.esc) {
                    return false;
                }
            }

            //Llença aquest script al recarregar els updatepanels per poder controlar per js els opclient
            function endRequestHandler() {
                checkOPCPanelClients();
                hidePopupLoader();
            }

            function showPopupLoader() {
                if (typeof (window.parent) != "undefined") {
                    window.parent.showLoadingGrid(true);
                }
            }

            function hidePopupLoader() {
                if (typeof (window.parent) != "undefined") {
                    window.parent.showLoadingGrid(false);
                }
            }

            function KeyPressFunction(e, noFunct) {
                tecla = (document.all) ? e.keyCode : e.which;
                if (tecla == 13) {
                    RunAcceptChangePwd();
                    return false;
                }
            }

            function RunAcceptChangePwd() {
                hidePopupLoader();
                ButtonClick($get('<%= btChangePwdOK.ClientID %>'));
            }
        </script>

        <div>

            <form id="form1" runat="server">
                <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

                <asp:HiddenField ID="hdnMustClose" Value="0" runat="server" />

                <table width="100%" cellspacing="0" border="0" class="bodyPopup">

                    <tr id="panChangePwdDragHandle" style="cursor: move; height: 30px;" class="panHeaderDashboardSmallGlobal">
                        <td align="center" class="chpassForm"></td>
                        <td>
                            <asp:Label ID="lblChangePwdTitle" Text="Cambiar contraseña" Font-Size="12px" Font-Bold="true" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3" style="padding-left: 10px; padding-top: 5px; vertical-align: middle">
                            <br />
                            <div style="min-height: 150px; max-height: 150px;">
                                <table cellpadding="1" cellspacing="1">
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblOldPwd" runat="server" Text="Contraseña anterior :" />
                                        </td>
                                        <td style="padding-left: 10px">
                                            <asp:TextBox ID="txtOldPwd" Width="175" TextMode="Password" autocomplete="off" AutoCompleteType="Disabled" runat="server" class="textClass x-form-text x-form-field " OnKeypress="return KeyPressFunction(event, true);"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblNewPwd" runat="server" Text="Nueva Contraseña :" />
                                        </td>
                                        <td style="padding-left: 10px">
                                            <asp:TextBox ID="txtNewPwd" Width="175" TextMode="Password" autocomplete="off" AutoCompleteType="Disabled" runat="server" class="textClass x-form-text x-form-field " OnKeypress="return KeyPressFunction(event, true);"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblNewPwdConfirm" runat="server" Text="Confirma nueva Contraseña :" />
                                        </td>
                                        <td style="padding-left: 10px">
                                            <asp:TextBox ID="txtNewPwdConfirm" Width="175" autocomplete="off" TextMode="Password" AutoCompleteType="Disabled" runat="server" class="textClass x-form-text x-form-field " OnKeypress="return KeyPressFunction(event,true);"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" align="center">
                                            <br />
                                            <asp:Label ID="lblChangePwdMessage" Text="" Style="display: none; visibility: hidden" CssClass="errorText" runat="server"></asp:Label>
                                            <br />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3" align="center" style="padding-bottom: 5px;">
                            <table id="tbChangePwdButtons" border="0" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td>
                                        <br />
                                        <asp:Button ID="btChangePwdOK" Text="${Button.Accept}" runat="server" TabIndex="3" OnClientClick="showPopupLoader();" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                    </td>
                                    <td>
                                        <br />
                                        <div id="changePWDcontainer">
                                            <asp:Button ID="btChangePwdCancel" Text="${Button.Cancel}" runat="server" TabIndex="4" OnClientClick="Close(); return false;" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                        </div>
                                    </td>
                                </tr>
                            </table>
                            <img id="imgLoading" src="Base/Images/loading.gif" alt="Loading..." style="display: none;" />
                        </td>
                    </tr>
                </table>
            </form>
        </div>
    </div>
</body>
</html>