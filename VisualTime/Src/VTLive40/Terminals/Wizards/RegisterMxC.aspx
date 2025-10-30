<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Forms_RegisterMxC" Culture="auto" UICulture="auto" CodeBehind="RegisterMxC.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Asistente para el alta de ${Terminals}</title>
</head>
<body class="bodyPopup" style="background-attachment: fixed;">

    <form id="frmNewTerminalWizard" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <div>

            <script language="javascript" type="text/javascript">
                function PageBase_Load() {
                }

                function EndClick() {
                    showPopupLoader();
                }

                //Llença aquest script al recarregar els updatepanels per poder controlar per js els opclient
                function endRequestHandler() {
                    window.parent.frames["ifPrincipal"].showLoadingGrid(false);
                    hidePopupLoader();
                }

                function showPopupLoader() {
                    window.parent.frames["ifPrincipal"].showLoadingGrid(true);
                }

                function hidePopupLoader() {
                    window.parent.frames["ifPrincipal"].showLoadingGrid(false);
                }
            </script>

            <asp:UpdatePanel ID="upNewTerminalWizard" runat="server" RenderMode="Inline">
                <ContentTemplate>

                    <div class="popupWizardContent">
                        <%-- WELCOME --%>
                        <div id="divStep0" runat="server" style="display: block">
                            <table id="tbStep0" style="width: 100%; height: 100%;" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td style="height: 360px">
                                        <asp:Image ID="imgWelcome" runat="server" Style="border-radius: 5px;" ImageUrl="~/Base/Images/Wizards/wzconnect.gif" />
                                    </td>
                                    <td style="padding-left: 20px; padding-right: 20px; padding-top: 50px" valign="top">

                                        <asp:Label ID="lblWelcome1" runat="server" Text="Bienvenido al asistente para registro de ${Terminals}."
                                            Font-Bold="True" Font-Size="Large"></asp:Label>
                                        <br />
                                        <br />
                                        <br />
                                        <asp:Label ID="lblWelcome2" runat="server" Text="Este asistente le ayudará a registrar un ${Terminal}." Font-Bold="true"></asp:Label>
                                        <br />
                                        <br />
                                        <asp:Label ID="lblWelcome3" runat="server" Text="Para continuar, haga clic en siguiente."></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="height: 10px" colspan="2" class="NewTerminalWizards_ButtonsPanel"></td>
                                </tr>
                            </table>
                        </div>

                        <asp:Label ID="hdnStepTitle" Text="Asistente para el registro de ${Terminals}. " runat="server" Style="display: none; visibility: hidden" />

                        <div id="divStep1" runat="server" style="display: none;">
                            <table id="tbStep1" runat="server" style="" cellspacing="0" cellpadding="0" border="0">
                                <tr>
                                    <td class="NewTerminalWizards_StepTitle" valign="top" style="">
                                        <asp:Label ID="lblStep1Title" runat="server" Text="Paso 1 de 2." Font-Bold="True" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="NewTerminalWizards_StepError popupWizardError" style="">
                                        <asp:Label ID="lblStep1Error" runat="server" CssClass="errorText" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="NewTerminalWizards_StepContent" valign="top" style="padding-top: 10px;">
                                        <table border="0" style="width: 100%;">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblTerminalRegisterDesc" runat="server" Text="Inserte la licencia facilitada para poder activar el Terminal y continuar con el registro."></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-top: 10px;" align="center">
                                                    <asp:TextBox ID="txtRegister" runat="server" Text="" Width="350px" CssClass="textClass" onkeypress="return onEnterPress(event,'__doPostBack(\'btNext\',\'\')');"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>

                    <div class="popupWizardButtons">
                        <table align="right" cellpadding="0" cellspacing="0">
                            <tr class="NewTerminalWizards_ButtonsPanel" style="height: 44px">
                                <td>&nbsp
                                </td>
                                <td>
                                    <asp:Button ID="btPrev" Text="${Button.Previous}" runat="server" OnClientClick="showPopupLoader();" Visible="false" TabIndex="1" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                </td>
                                <td>
                                    <asp:Button ID="btNext" Text="${Button.Next}" runat="server" OnClientClick="showPopupLoader();" TabIndex="2" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                    <asp:Button ID="btEnd" Text="${Button.End}" runat="server" OnClientClick="EndClick();" Visible="false" TabIndex="4" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                </td>
                                <td>
                                    <asp:Button ID="btClose" Text="${Button.Cancel}" runat="server" OnClientClick="Close(); return false;" TabIndex="5" CssClass="btnFlat btnFlatBlack btnFlatAsp" />

                                    <asp:HiddenField ID="hdnCanClose_PageBase" runat="server" />
                                    <asp:HiddenField ID="hdnMustRefresh_PageBase" Value="0" runat="server" />
                                </td>
                            </tr>
                        </table>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>

            <Local:MessageFrame ID="MessageFrame1" runat="server" />
        </div>
    </form>
</body>
</html>