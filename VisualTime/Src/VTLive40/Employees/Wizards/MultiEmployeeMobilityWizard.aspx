<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Wizards_MultiEmployeeMobilityWizard" EnableEventValidation="false" Culture="auto" UICulture="auto" EnableViewState="True" CodeBehind="MultiEmployeeMobilityWizard.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<%@ Register Src="~/Employees/WebUserControls/EmployeeType.ascx" TagName="EmployeeType" TagPrefix="Local" %>

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Asistente para el alta de múltiples ${Employees}</title>
</head>
<body class="bodyPopup" style="background-attachment: fixed;">

    <form id="frmMultiEmployeeMobilityWizard" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <script language="javascript" type="text/javascript">

            var bolLoaded = false;

            async function PageBase_Load() {
                if (!bolLoaded) {
                    await getroTreeState('objContainerTreeV3_treeMultiEmployeeMobilityEmployeesWizard').then(roState => roState.reset());
                    await getroTreeState('objContainerTreeV3_treeMultiEmployeeMobilityEmployeesWizardGrid').then(roState => roState.reset());
                    await getroTreeState('objContainerTreeV3_treeMultiEmployeeMobilityGroupWizard').then(roState => roState.reset());
                }
                DateDisableState();
                var oActiveFrameIndex = document.getElementById('<%= hdnActiveFrame.ClientID %>').value;
                ConvertControls('divStep' + oActiveFrameIndex);
                checkOPCPanelClients();

                if (!bolLoaded) bolLoaded = true;
            }

            function checkOPCPanelClients() {
                linkOPCItems('<%= optNow.ClientID %>,<%= optFuture.ClientID %>');
                venableOPC('<%= optNow.ClientID %>');
                venableOPC('<%= optFuture.ClientID %>');
            }

            function DateDisableState() {
                txtMoveDateClient.SetEnabled(false);
            }

            function DateEnableState(status) {
                if (document.getElementById('optFuture_rButton').checked == true) {
                    txtMoveDateClient.SetEnabled(true);
                }
            }

            //Llença aquest script al recarregar els updatepanels per poder controlar per js els opclient
            function endRequestHandler() {
                checkOPCPanelClients();
                hidePopupLoader();
            }

            function showPopupLoader() {
                window.parent.frames["ifPrincipal"].showLoadingGrid(true);
            }

            function hidePopupLoader() {
                window.parent.frames["ifPrincipal"].showLoadingGrid(false);
            }

            function CheckFrame() {
                var oActiveFrameIndex = document.getElementById('<%= hdnActiveFrame.ClientID %>').value;

                if (CheckConvertControls('divStep' + oActiveFrameIndex) == false) {
                    return false;
                }
                else {
                    return true;
                }
            }

            function devKeyPress(s, e) {
                return onEnterPress(e, 'if (CheckFrame() == true) {__doPostBack(\'btNext\',\'\');}');
            }

            function GetSelectedTreeV3(oParm1, oParm2, oParm3) {
                var hdnEmployeesSelected = document.getElementById('<%= me.hdnEmployeesSelected.ClientID %>');
                hdnEmployeesSelected.value = oParm1;

                var hdnFilter = document.getElementById('<%= me.hdnFilter.ClientID %>');
                hdnFilter.value = oParm2;

                var hdnFilterUser = document.getElementById('<%= me.hdnFilterUser.ClientID %>');
                hdnFilterUser.value = oParm3;
            }

            function GroupSelected(Nodo) {
                var hdnSelected = document.getElementById('<%= Me.hdnIDGroupSelected.ClientID %>');
                hdnSelected.value = Nodo.id;
            }
        </script>

        <asp:Label ID="hdnStepTitle" Text="Asistente para realizar múltiples movilidades de ${Employees}. " runat="server" Style="display: none; visibility: hidden" />
        <asp:Label ID="hdnStepTitle2" Text="Paso {0} de {1}." runat="server" Style="display: none; visibility: hidden" />

        <div class="popupWizardContent">
            <asp:UpdatePanel ID="updStep0" runat="server" RenderMode="Inline">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btPrev" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="btNext" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="btEnd" EventName="Click" />
                </Triggers>
                <ContentTemplate>
                    <div id="divStep0" runat="server" style="display: block;">
                        <table id="tbStep0" style="" cellspacing="0" cellpadding="0">
                            <tr>
                                <td style="height: 360px">
                                    <asp:Image ID="imgNewMultiEmployeeWizard" runat="server" Style="border-radius: 5px;" ImageUrl="~/Base/Images/Wizards/wzmens.gif" />
                                </td>
                                <td style="padding-left: 20px; padding-right: 20px; padding-top: 50px" valign="top">
                                    <asp:Label ID="lblWelcome1" runat="server" Text="Bienvenido al asistente para movilidades de ${Employees}."
                                        Font-Bold="True" Font-Size="Large"></asp:Label>
                                    <br />
                                    <br />
                                    <br />
                                    <asp:Label ID="lblWelcome2" runat="server" Text="Este asistente le ayudará a realizar la movilidad de múltiples ${Employees}."
                                        Font-Bold="true"></asp:Label>
                                    <br />
                                    <br />
                                    <asp:Label ID="lblWelcome3" runat="server" Text="Para continuar, haga clic en siguiente."></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td style="height: 10px" colspan="2" class="NewEmployeeWizards_ButtonsPanel"></td>
                            </tr>
                        </table>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>

            <asp:UpdatePanel ID="updStep1" runat="server" RenderMode="Inline">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btPrev" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="btNext" EventName="Click" />
                </Triggers>
                <ContentTemplate>
                    <div id="divStep1" runat="server" style="display: none; width: 500px;">
                        <table style="width: 790px; height: 410px;" cellspacing="0" cellpadding="0">
                            <tr>
                                <td class="NewEmployeeWizards_StepTitle">
                                    <table style="width: 100%">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblStep1Title" runat="server" Text="" Font-Bold="True" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="padding-left: 20px; padding-right: 50px;">
                                                <asp:Label ID="lblStep1Info" runat="server" Text="" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td class="NewEmployeeWizards_StepContent">

                                    <table style="width: 100%; height: 100%;" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblStep1Info2" runat="server" Text="Ahora seleccione los ${Employees} sobre los que desea realizar la mobilidad." />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="padding-left: 10px; padding-top: 5px" valign="top">

                                                <div style="height: 100%;">
                                                    <input type="hidden" id="hdnEmployeesSelected" runat="server" value="" />
                                                    <input type="hidden" id="hdnFilter" runat="server" value="" />
                                                    <input type="hidden" id="hdnFilterUser" runat="server" value="" />
                                                    <iframe id="ifEmployeesSelector" runat="server" style="background-color: Transparent" height="290" width="100%"
                                                        scrolling="no" frameborder="0" marginheight="0" marginwidth="0" src="" />
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td class="NewEmployeeWizards_StepError">
                                    <asp:Label ID="lblStep1Error" runat="server" CssClass="errorText" />
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
                                <td class="NewEmployeeWizards_StepTitle">
                                    <table style="width: 100%">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblStep2Title" runat="server" Text="" Font-Bold="True" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="padding-left: 20px">
                                                <asp:Label ID="lblSetp2Info" runat="server" Text="" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td class="NewEmployeeWizards_StepContent">
                                    <table style="width: 100%; height: 100%;" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblSetp2Info2" runat="server" Text="Ahora seleccione el grupo al qual desea mover los ${Employees}" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="padding-left: 10px; padding-top: 5px" valign="top">

                                                <div style="height: 100%;">
                                                    <asp:HiddenField ID="hdnIDGroupSelected" runat="server" Value="" />
                                                    <iframe id="ifGroupSelector" runat="server" style="background-color: Transparent; overflow: auto" height="290px" width="100%"
                                                        scrolling="no" frameborder="0" marginheight="0" marginwidth="0" src="" />
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td class="NewEmployeeWizards_StepError">
                                    <asp:Label ID="lblStep2Error" runat="server" CssClass="errorText" />
                                </td>
                            </tr>
                        </table>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>

            <asp:UpdatePanel ID="updStep3" runat="server" RenderMode="Inline">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btPrev" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="btNext" EventName="Click" />
                </Triggers>
                <ContentTemplate>
                    <div id="divStep3" runat="server" style="display: none;">
                        <table style="" cellspacing="0" cellpadding="0">
                            <tr>
                                <td class="NewEmployeeWizards_StepTitle">
                                    <table style="width: 100%">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblStep3Title" runat="server" Text="" Font-Bold="True" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="padding-left: 20px">
                                                <asp:Label ID="lblStep3Info" runat="server" Text="" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>

                            <tr>
                                <td class="NewEmployeeWizards_StepContent">
                                    <table style="width: 100%;" cellspacing="0" cellpadding="0" border="0">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblStep3Info2" runat="server" Text="" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td>
                                                <roUserControls:roOptionPanelClient ID="optNow" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Enabled="True" CConClick="DateDisableState(false)">
                                                    <Title>
                                                        <asp:Label ID="lblNumberContractLastTitle" runat="server" Text="Mover los ${Employees} ahora"></asp:Label>
                                                    </Title>
                                                    <Description>
                                                        <asp:Label ID="lblNowDescription" Text="Moverá los ${Employees} seleccionados inmediatamente." ForeColor="DarkBlue" runat="server"></asp:Label>
                                                    </Description>
                                                    <Content>
                                                    </Content>
                                                </roUserControls:roOptionPanelClient>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <roUserControls:roOptionPanelClient ID="optFuture" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Enabled="True" CConClick="DateEnableState(true)">
                                                    <Title>
                                                        <asp:Label ID="lblFutureTitle" runat="server" Text="Mover los ${Employees} seleccionados a partir de una fecha específica"></asp:Label>
                                                    </Title>
                                                    <Description>
                                                        <asp:Label ID="lblFutureDescription" Text="El movimiento quedará guardado y se hará efectivo en la fecha " ForeColor="DarkBlue" runat="server"></asp:Label>
                                                    </Description>
                                                    <Content>
                                                        <table cellpadding="0" cellspacing="5" style="padding-left: 10px; padding-top: 5px;" width="100%">
                                                            <tr>
                                                                <td align="left" style="padding-left: 10px;">
                                                                    <dx:ASPxDateEdit ID="txtMoveDate" runat="server" AllowNull="false" Width="150" ClientEnabled="true" ClientInstanceName="txtMoveDateClient">
                                                                        <ClientSideEvents DateChanged="function(s,e){}" />
                                                                    </dx:ASPxDateEdit>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </Content>
                                                </roUserControls:roOptionPanelClient>
                                                <roUserControls:roOptPanelClientGroup ID="optGroup" runat="server" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>

                            <tr>
                                <td class="NewEmployeeWizards_StepError popupWizardError">
                                    <asp:Label ID="lblStep3Error" runat="server" CssClass="errorText" />
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
                        <tr class="NewEmployeeWizards_ButtonsPanel" style="height: 44px">
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
    </form>
</body>
</html>