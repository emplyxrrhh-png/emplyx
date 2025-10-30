<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Wizards_NewCompanyWizard" EnableViewState="True" CodeBehind="NewCompanyWizard.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Asistente para la creación de empresas</title>
    <script type="text/javascript" language="javascript">
        function devKeyPress(s, e) {
            return onEnterPress(e, 'if (CheckFrame() == true) {__doPostBack(\'btNext\',\'\');}');
        }
    </script>
</head>
<body class="bodyPopup" style="background-attachment: fixed;">

    <form id="frmNewCompanyWizard" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />
        <div>

            <script language="javascript" type="text/javascript">

                function PageBase_Load() {

                    var oActiveFrameIndex = document.getElementById('<%= hdnActiveFrame.ClientID %>').value;
                    ConvertControls('divStep' + oActiveFrameIndex);
                }

                function CheckFrame() {
                    var bolRet = true;
                    var oActiveFrameIndex = document.getElementById('<%= hdnActiveFrame.ClientID %>').value;

                    if (CheckConvertControls('divStep' + oActiveFrameIndex) == false) {
                        bolRet = false;
                    }
                    else {
                        bolRet = true;
                    }

                    if (!bolRet) hidePopupLoader();
                    return bolRet;
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

            <div class="popupWizardContent">

                <asp:UpdatePanel ID="updStep0" runat="server" RenderMode="Inline">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btPrev" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btNext" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btEnd" EventName="Click" />
                    </Triggers>
                    <ContentTemplate>

                        <%-- WELCOME --%>
                        <div id="divStep0" runat="server" style="display: block;">
                            <table id="tbStep0" style="" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td style="height: 360px">
                                        <asp:Image ID="imgNewCompanyWizard" runat="server" Style="border-radius: 5px;" ImageUrl="~/Base/Images/Wizards/wzCompany.gif" />
                                    </td>
                                    <td style="padding-left: 20px; padding-right: 20px; padding-top: 50px" valign="top">

                                        <asp:Label ID="lblWelcome1" runat="server" Text="Bienvenido al asistente generar empresas."
                                            Font-Bold="True" Font-Size="Large"></asp:Label>
                                        <br />
                                        <br />
                                        <br />
                                        <asp:Label ID="lblWelcome2" runat="server" Text="El asistente le ayudará a dar de alta una empresa." Font-Bold="true"></asp:Label>
                                        <br />
                                        <br />
                                        <asp:Label ID="lblWelcome3" runat="server" Text="Para continuar, haga clic en siguiente."></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="height: 10px" colspan="2" class="NewCompanyWizards_ButtonsPanel"></td>
                                </tr>
                            </table>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>

                <asp:Label ID="hdnStepTitle" Text="Asistente para el alta de una empresa. " runat="server" Style="display: none; visibility: hidden" />
                <asp:Label ID="hdnStepTitle2" Text="Paso {0} de {1}." runat="server" Style="display: none; visibility: hidden" />

                <asp:UpdatePanel ID="updStep1" runat="server" RenderMode="Inline">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btPrev" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btNext" EventName="Click" />
                    </Triggers>
                    <ContentTemplate>
                        <div id="divStep1" runat="server" style="display: none;">
                            <table style="" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td class="NewCompanyWizards_StepTitle">
                                        <table style="width: 100%">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblStep1Title" runat="server" Text="" Font-Bold="True" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 20px">
                                                    <asp:Label ID="lblSetp1Info" runat="server" Text="En primer lugar, debe introducir el nombre de la empresa." />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="NewCompanyWizards_StepError popupWizardError">
                                        <asp:Label ID="lblStep1Error" runat="server" CssClass="errorText" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="NewCompanyWizards_StepContent">

                                        <table>
                                            <tr>
                                                <td colspan="2">
                                                    <asp:Label ID="lblStep1Info2" runat="server" Text="Escriba el nombre de la nueva empresa y pulse el botón 'Siguiente'." />
                                                    <br />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 5px; padding-top: 20px">
                                                    <asp:Label ID="lblCompanyName" runat="server" Text="Nombre de la empresa:" />
                                                </td>
                                                <td style="padding-left: 10px; padding-top: 20px">
                                                    <dx:ASPxTextBox ID="txtCompanyName" Text="" runat="server" ClientInstanceName="txtCompanyName" MaxLength="64">
                                                        <ClientSideEvents KeyPress="devKeyPress" />
                                                    </dx:ASPxTextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>

                <asp:UpdatePanel ID="updStep2" runat="server" RenderMode="Inline">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btPrev" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btNext" EventName="Click" />
                    </Triggers>
                    <ContentTemplate>

                        <div id="divStep2" runat="server" style="display: none;">
                            <table style="" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td class="NewCompanyWizards_StepTitle">
                                        <table style="width: 100%">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblStep2Title" runat="server" Text="" Font-Bold="True" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 20px; padding-right: 50px;">
                                                    <asp:Label ID="lblStep2Info" runat="server" Text="" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="NewCompanyWizards_StepError popupWizardError">
                                        <asp:Label ID="lblStep2Error" runat="server" CssClass="errorText" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="NewCompanyWizards_StepContent">

                                        <table style="width: 100%;" cellspacing="0" cellpadding="0">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblStep2Info2" runat="server" Text="El asistente ya dispone de suficientes datos para poder crear la nueva empresa. Pulse finalizar para iniciar el proceso." />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>

            <div class="popupWizardButtons">

                <asp:UpdatePanel ID="updButtons" runat="server" RenderMode="Inline">
                    <ContentTemplate>

                        <table align="right" cellpadding="0" cellspacing="0">
                            <tr class="NewCompanyWizards_ButtonsPanel" style="height: 44px">
                                <td>&nbsp
                                </td>
                                <td>
                                    <asp:Button ID="btPrev" Text="${Button.Previous}" runat="server" OnClientClick="showPopupLoader();" Visible="false" TabIndex="1" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                </td>
                                <td>
                                    <asp:Button ID="btNext" Text="${Button.Next}" runat="server" OnClientClick="showPopupLoader();return CheckFrame();" TabIndex="2" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                    <asp:Button ID="btEnd" Text="${Button.End}" runat="server" OnClientClick="showPopupLoader();" Visible="false" TabIndex="4" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                </td>
                                <td>
                                    <asp:Button ID="btClose" Text="${Button.Cancel}" runat="server" OnClientClick="Close(); return false;" TabIndex="5" CssClass="btnFlat btnFlatBlack btnFlatAsp" />

                                    <asp:HiddenField ID="hdnCanClose_PageBase" runat="server" />
                                    <asp:HiddenField ID="hdnMustRefresh_PageBase" Value="0" runat="server" />
                                </td>
                            </tr>
                        </table>

                        <input type="hidden" id="hdnActiveFrame" value="0" runat="server" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>

            <Local:MessageFrame ID="MessageForm" runat="server" />
        </div>
    </form>
</body>
</html>