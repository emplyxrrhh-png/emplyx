<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Wizards_NewGroupWizard" CodeBehind="NewGroupWizard.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml" height="100%">
<head runat="server">
    <title>Asistente para la creación de ${Groups}</title>
</head>
<body class="bodyPopup" style="background-attachment: fixed;">
    <form id="frmNewGroupWizard" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />
        <div>

            <script language="javascript" type="text/javascript">

                function PageBase_Load() {
                    ConvertControls();
                }

                function devKeyPress(s, e) {
                    return onEnterPress(e, 'if (CheckFrame() == true) {__doPostBack(\'btNext\',\'\');}');
                }
                function endRequestHandler() {
                    hidePopupLoader();
                }

                function showPopupLoader() {
                    window.parent.frames["ifPrincipal"].showLoadingGrid(true);
                }

                function hidePopupLoader() {
                    window.parent.frames["ifPrincipal"].showLoadingGrid(false);
                }
            </script>

            <asp:HiddenField ID="hdnIdGroup" runat="server" Value="0" />

            <asp:UpdatePanel ID="upNewGroupWizard" runat="server" RenderMode="Inline">
                <ContentTemplate>

                    <div class="popupWizardContent">
                        <%-- WELCOME --%>
                        <div id="divStep0" runat="server" style="display: block;">
                            <table id="tbStep0" style="" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td style="height: 360px">
                                        <asp:Image ID="imgNewGroupWizard" runat="server" Style="border-radius: 5px;" ImageUrl="~/Base/Images/Wizards/wzmens.gif" />
                                    </td>
                                    <td style="padding-left: 20px; padding-right: 20px; padding-top: 50px" valign="top">

                                        <asp:Label ID="lblNewGroupWelcome1" runat="server" Text="Bienvenido al asistente para el alta de ${Groups}."
                                            Font-Bold="True" Font-Size="Large"></asp:Label>
                                        <br />
                                        <br />
                                        <br />
                                        <asp:Label ID="lblNewGroupWelcome2" runat="server" Text="Este asistente le ayudará a dar de alta un nuevo ${Group}." Font-Bold="true"></asp:Label>
                                        <br />
                                        <br />
                                        <asp:Label ID="lblNewGroupWelcome3" runat="server" Text="Para continuar, haga clic en siguiente."></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="height: 10px" colspan="2" class="NewGroupWizards_ButtonsPanel"></td>
                                </tr>
                            </table>
                        </div>

                        <asp:Label ID="hdnStepTitle" Text="Asistente para la creación de ${Groups}. " runat="server" Style="display: none; visibility: hidden" />

                        <div id="divStep1" runat="server" style="display: none">
                            <table id="tbStep1" runat="server" style="" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td class="NewGroupWizards_StepTitle">
                                        <table style="width: 100%">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblStep1Title" runat="server" Text="Paso 1 de 2." Font-Bold="True" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 20px">
                                                    <asp:Label ID="lblSetp1Info" runat="server" Text="En primer lugar debe escoger el nombre del nuevo ${Group}." />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="NewGroupWizards_StepError popupWizardError">
                                        <asp:Label ID="lblStep1Error" runat="server" CssClass="errorText" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="NewGroupWizards_StepContent">

                                        <table>
                                            <tr>
                                                <td style="padding-left: 10px; padding-top: 20px; font-weight: bold;">
                                                    <asp:Label ID="lblStep1Info2a" runat="server" Text="Dependerá del ${Group}" />
                                                    <asp:Label ID="lblStep1Info2b" runat="server" Text="" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 10px; padding-top: 20px">
                                                    <asp:Label ID="lblStep1Info2" runat="server" Text="Escriba el nombre del nuevo ${Group} y pulse el botón 'Siguiente'." />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 10px; padding-top: 20px">
                                                    <asp:Label ID="lblGroupName" runat="server" Text="Nombre del grupo:" />
                                                    <br />
                                                    <dx:ASPxTextBox ID="txtGroupName" Text="" runat="server" ClientInstanceName="txtGroupNameClient" MaxLength="64">
                                                        <ClientSideEvents KeyPress="devKeyPress" />
                                                    </dx:ASPxTextBox>
                                                </td>
                                            </tr>

                                            <!-- Descripcion del grupo -->
                                            <tr>
                                                <td style="padding-left: 10px; padding-top: 20px">
                                                    <asp:Label ID="lblDescription" runat="server" Text="Descripción del grupo:" />
                                                    <br />
                                                    <dx:ASPxMemo ID="txtDescription" runat="server" Rows="2" Width="450" />
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
                            <tr class="NewGroupWizards_ButtonsPanel" style="height: 44px">
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
        </div>
    </form>
</body>
</html>