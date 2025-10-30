<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.ChangeCommsStateWizard" Culture="auto" UICulture="auto" CodeBehind="ChangeCommsStateWizard.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Asistente para el cambio de estato de comunicaciones con ${Terminals}</title>
</head>
<body class="bodyPopup" style="background-attachment: fixed;">

    <form id="frmChangeCommsStateWizard" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <div>

            <script language="javascript" type="text/javascript">
                function PageBase_Load() {
                }
                function endRequestHandler() {
                    hidePopupLoader();
                }

                function showPopupLoader() {
                    if (typeof (window.parent.frames["ifPrincipal"]) != "undefined") {
                        window.parent.frames["ifPrincipal"].showLoadingGrid(true);
                    } else {
                        window.parent.parent.frames["ifPrincipal"].showLoadingGrid(true);
                    }
                }

                function hidePopupLoader() {
                    if (typeof (window.parent.frames["ifPrincipal"]) != "undefined") {
                        window.parent.frames["ifPrincipal"].showLoadingGrid(false);
                    } else {
                        window.parent.parent.frames["ifPrincipal"].showLoadingGrid(false);
                    }
                }
            </script>

            <asp:UpdatePanel ID="upChangeCommsStateWizard" runat="server" RenderMode="Inline">
                <ContentTemplate>

                    <div class="popupWizardContent">
                        <%-- WELCOME --%>
                        <div id="divStep0" runat="server" style="display: block">
                            <table id="tbStep0" style="" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td style="height: 360px">
                                        <asp:Image ID="imgWelcome" runat="server" Style="border-radius: 5px;" ImageUrl="~/Base/Images/Wizards/wzconnect.gif" />
                                    </td>
                                    <td style="padding-left: 20px; padding-right: 20px; padding-top: 50px" valign="top">

                                        <asp:Label ID="lblWelcome1" runat="server" Text="Bienvenido al asistente para cambiar el estado de las comunicaciones."
                                            Font-Bold="True" Font-Size="Large"></asp:Label>
                                        <br />
                                        <br />
                                        <br />
                                        <asp:Label ID="lblWelcome2" runat="server" Text="Este asistente le permitirá habilitar o deshabilitar las comunicaciones con los ${Terminals}." Font-Bold="true"></asp:Label>
                                        <br />
                                        <br />
                                        <asp:Label ID="lblWelcome3" runat="server" Text="Para continuar, haga clic en siguiente."></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="height: 10px" colspan="2" class="ChangeCommsStateWizards_ButtonsPanel"></td>
                                </tr>
                            </table>
                        </div>

                        <asp:Label ID="hdnStepTitle" Text="Asistente para el cambio de estado de comunicaciones. " runat="server" Style="display: none; visibility: hidden" />

                        <div id="divStep1" runat="server" style="display: none;">
                            <table id="tbStep1" runat="server" style="" cellspacing="0" cellpadding="0" border="0">
                                <tr>
                                    <td class="ChangeCommsStateWizards_StepTitle" valign="top" style="">
                                        <asp:Label ID="lblStep1Title" runat="server" Text="Paso 1 de 2." Font-Bold="True" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="ChangeCommsStateWizards_StepError popupWizardError" style="">
                                        <asp:Label ID="lblStep1Error" runat="server" CssClass="errorText" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="ChangeCommsStateWizards_StepContent" valign="top" style="padding-top: 10px;">
                                        <table border="0" style="width: 100%;">
                                            <caption>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lblCommsConnectionActualStatus" runat="server"
                                                            Text="Las comunicaciones con los terminale están actualmente activadas." />
                                                    </td>
                                                </tr>
                                                <br />
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lblChangeConnectionStateDesc" runat="server"
                                                            Text="Para deshabilitar las comunicaciones con los terminales ..." />
                                                    </td>
                                                </tr>
                                            </caption>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </div>

                        <div id="divStep2" runat="server" style="display: none">
                            <table id="btStep2" style="" cellspacing="0" cellpadding="0" border="0">
                                <tr>
                                    <td class="ChangeCommsStateWizards_StepTitle" valign="top" style="">
                                        <asp:Label ID="lblStep2Title" runat="server" Text="Paso 2 de 2." Font-Bold="True" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="ChangeCommsStateWizards_StepError popupWizardError" style="">
                                        <asp:Label ID="lblStep2Error" runat="server" CssClass="errorText" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="ChangeCommsStateWizards_StepContent" valign="top" style="padding-top: 10px;">
                                        <table border="0" style="width: 100%;">
                                            <tr>
                                                <td colspan="2" style="padding-left: 10px; padding-bottom: 10px;">
                                                    <asp:Label ID="lblTerminalInfoTitle" runat="server" Text="Información del Terminal" Font-Bold="true"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 15px; width: 50%;">
                                                    <asp:Label ID="lblUsername" runat="server" Text="Usuario:"></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:Label ID="lblCurrentUser" runat="server" Text="Nombre"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr id="trPassword">
                                                <td style="padding-left: 15px; width: 50%;">
                                                    <asp:Label ID="lblPassword" runat="server" Text="Contraseña:"></asp:Label>
                                                </td>
                                                <td>
                                                    <input type="password" id="txtPassword" runat="server" convertcontrol="TextField" ccallowblank="false" ccinputtype="password" class="textClass" onkeypress="return onEnterPress(event,'');" autocomplete="off" />
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
                            <tr class="ChangeCommsStateWizards_ButtonsPanel" style="height: 44px">
                                <td>&nbsp
                                </td>
                                <td>
                                    <asp:Button ID="btPrev" Text="${Button.Previous}" runat="server" OnClientClick="showPopupLoader();" Visible="false" TabIndex="1" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                </td>
                                <td>
                                    <asp:Button ID="btNext" Text="${Button.Next}" runat="server" OnClientClick="showPopupLoader();" TabIndex="2" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                    <asp:Button ID="btEnd" Text="${Button.End}" runat="server" Visible="false" TabIndex="4" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
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