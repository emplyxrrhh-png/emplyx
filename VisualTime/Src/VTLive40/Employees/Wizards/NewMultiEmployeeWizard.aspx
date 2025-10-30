<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Wizards_NewMultiEmployeeWizard" EnableEventValidation="false" Culture="auto" UICulture="auto" EnableViewState="True" CodeBehind="NewMultiEmployeeWizard.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<%@ Register Src="~/Employees/WebUserControls/EmployeeType.ascx" TagName="EmployeeType" TagPrefix="Local" %>

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Asistente para el alta de múltiples ${Employees}</title>
</head>
<body class="bodyPopup" style="background-attachment: fixed;">

    <form id="frmNewMultiEmployeeWizard" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <asp:HiddenField ID="hdnIdGroup" runat="server" Value="0" />
        <asp:HiddenField ID="hdnDisableBiometric" runat="server" Value="false" />

        <script language="javascript" type="text/javascript">

            var bolLoaded = false;

            function PageBase_Load() {

                var oActiveFrameIndex = document.getElementById('<%= hdnActiveFrame.ClientID %>').value;
                ConvertControls('divStep' + oActiveFrameIndex);

                //Enllaç dels OptionPanelClients
                linkOPCItems('<%= optNumberContractLast.ClientID %>,<%= optNumberContractNew.ClientID %>');
                linkOPCItems('<%= optCardsFromLast.ClientID %>,<%= optCardsContract.ClientID %>,<%= optCardsFromNumber.ClientID %>');

                checkOPCPanelClients();

                var hdnDisableBiometricData = document.getElementById('<%= Me.hdnDisableBiometric.ClientID %>');
                if (hdnDisableBiometricData.value == 'true') {                                
                    var oChk = document.getElementById("cnIdentifyMethods_chkBiometric_chkButton");
                    oChk.disabled = true;
                    var oLnk = document.getElementById("cnIdentifyMethods_chkBiometric_aTitle");
                    oLnk.removeAttribute("href");   

                }                    
                if (bolLoaded == false) bolLoaded = true;
            }
            
            function checkOPCPanelClients() {
                venableOPC('<%= optNumberContractLast.ClientID %>');
                venableOPC('<%= optNumberContractNew.ClientID %>');
                venableOPC('<%= optCardsFromLast.ClientID %>');
                venableOPC('<%= optCardsContract.ClientID %>');
                venableOPC('<%= optCardsFromNumber.ClientID %>');
                //roCB_disable('cnIdentifyMethods_cmbFunctionality', true);
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

            function OnchkParentGroupNull(chk) {
                var ifPassportSelector = document.getElementById('<%= ifPassportSelector.ClientID %>');
                if (chk.checked) {
                    ifPassportSelector.style.display = 'none';
                }
                else {
                    ifPassportSelector.style.display = '';
                }
            }

            function PassportEmployeeSelected(Nodo) {
                var hdnSelected = document.getElementById('<%= Me.hdnPassportSelected.ClientID %>');
                hdnSelected.value = Nodo.id;
            }

            function devKeyPress(s, e) {
                return onEnterPress(e, 'if (CheckFrame() == true) {__doPostBack(\'btNext\',\'\');}');
            }

            function gridEmployeesWizard_BeginCallback(e, c) {
            }

            function gridEmployeesWizard_EndCallback(s, e) {
                if (s.IsEditing()) { }
            }

            function gridEmployeesWizard_FocusedRowChanged(s, e) {
                if (s.IsEditing()) s.UpdateEdit();
            }
        </script>

        <asp:Label ID="hdnStepTitle" Text="Asistente para el alta de múltiples ${Employees}. " runat="server" Style="display: none; visibility: hidden" />
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
                                    <asp:Label ID="lblWelcome1" runat="server" Text="Bienvenido al asistente para generar múltiples ${Employees}."
                                        Font-Bold="True" Font-Size="Large"></asp:Label>
                                    <br />
                                    <br />
                                    <br />
                                    <asp:Label ID="lblWelcome2" runat="server" Text="Este asistente le ayudará a dar de alta múltiples ${Employees}."
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
                    <div id="divStep1" runat="server" style="display: none;">
                        <table style="" cellspacing="0" cellpadding="0">
                            <tr>
                                <td class="NewEmployeeWizards_StepTitle">
                                    <table style="width: 100%">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblStep1Title" runat="server" Text="" Font-Bold="True" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="padding-left: 20px">
                                                <asp:Label ID="lblSetp2Info" runat="server" Text="Introduzca el número de ${Employees} que desea crear." />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td class="NewEmployeeWizards_StepError popupWizardError">
                                    <asp:Label ID="lblStep1Error" runat="server" CssClass="errorText" />
                                </td>
                            </tr>
                            <tr>
                                <td class="NewEmployeeWizards_StepContent">
                                    <table style="width: 100%" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td colspan="2" style="padding-left: 10px; padding-top: 20px; font-weight: bold;">
                                                <asp:Label ID="lblStep1Info2a" runat="server" Text="Los empleados que cree dependerán del ${Group}" />
                                                <asp:Label ID="lblStep1Info2b" runat="server" Text="" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2" style="padding-left: 10px; padding-top: 20px;">
                                                <asp:Label ID="lblStep1Info2" runat="server" Text="Escriba el número de ${Employees} que quiere crear." />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right" style="padding-top: 20px;">
                                                <asp:Label ID="lblNumberEmployees" Text="Número de ${Employees} a dar de alta:" runat="server"></asp:Label>
                                            </td>
                                            <td align="left" style="padding-left: 10px; padding-top: 20px; width: 50%;">
                                                <dx:ASPxTextBox ID="txtNumberEmployees" Text="1" runat="server">
                                                    <MaskSettings Mask="<0..100>" IncludeLiterals="None" />
                                                    <ClientSideEvents KeyPress="devKeyPress" />
                                                </dx:ASPxTextBox>
                                                <%--<input type="text" id="txtNumberEmployees" value="1" runat="server" class="textClass"
                                                    convertcontrol="NumberField" ccallowblank="false" ccmaxvalue="100" ccallowdecimals="false"
                                                    style="width: 55px;" onkeypress="return onEnterPress(event,'if (CheckFrame() == true) {__doPostBack(\'btNext\',\'\');}');"
                                                    maxlength="3" />--%>
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
                                <td class="NewEmployeeWizards_StepTitle">
                                    <table style="width: 100%">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblStep2Title" runat="server" Text="" Font-Bold="True" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="padding-left: 20px">
                                                <asp:Label ID="lblSetp3Info" runat="server" Text="Introduzca la fecha de inicio de contrato de los ${Employees} que va a crear." />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td class="NewEmployeeWizards_StepContent">
                                    <table style="width: 100%;" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td colspan="2" valign="top">
                                                <asp:Label ID="lblStep2Info2" runat="server" Text="Escriba la fecha de inicio de contrato de los ${Employees} que está dando de alta. Si necesitara especificar fechas diferentes para cada ${Employee}, podrá editar la fecha de inicio de contrato para cada ${Employee} después de crearlos." />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="NewEmployeeWizards_StepError popupWizardError">
                                                <asp:Label ID="lblStep2Error" runat="server" CssClass="errorText" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="padding-top: 5px; padding-top: 20px; width: 300px;">
                                                <asp:Label ID="lblBeginContract" Text="Fecha de inicio del contrato de los ${Employees}:"
                                                    runat="server"></asp:Label>
                                            </td>
                                            <td align="left" style="padding-left: 10px; padding-top: 20px;">
                                                <dx:ASPxDateEdit ID="txtBeginContract" runat="server" Width="150" AllowNull="false">
                                                    <ClientSideEvents KeyPress="()=>{setTimeout(function(){devKeyPress();},200);}" />
                                                </dx:ASPxDateEdit>
                                                <%--<input type="text" id="txtBeginContract" runat="server" class="textClass" convertcontrol="DatePicker"
                                                    ccallowblank="false" style="width: 75px;" onkeypress="return onEnterPress(event,'if (CheckFrame() == true) {__doPostBack(\'btNext\',\'\');}');" />--%>
                                            </td>
                                        </tr>
                                    </table>
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
                                                <asp:Label ID="lblStep3Info" runat="server" Text="Seleccione el modo que quiere utilizar para crear los números de contrato." />
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
                            <tr>
                                <td class="NewEmployeeWizards_StepContent">
                                    <table style="width: 100%;" cellspacing="0" cellpadding="0" border="0">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblStep3Info2" runat="server" Text="Seleccione el modo de crear los nuevos números de contrato. Si decide crear nuevos números de contrato, debe especificar a partir de qué número quiere comenzar a crearlos." />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="padding-left: 10px; padding-top: 10px">
                                                <roUserControls:roOptionPanelClient ID="optNumberContractLast" runat="server" TypeOPanel="RadioOption"
                                                    Width="100%" Height="Auto" Enabled="True">
                                                    <Title>
                                                        <asp:Label ID="lblNumberContractLastTitle" runat="server" Text="Crear los números de contrato a partir del último número existente"></asp:Label>
                                                    </Title>
                                                    <Description>
                                                        <asp:Label ID="lblNumberContractLastDescription" runat="server" Text="Los números se crearán a continuación del último número de contrato existente."></asp:Label>
                                                    </Description>
                                                    <Content>
                                                    </Content>
                                                </roUserControls:roOptionPanelClient>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="padding-left: 10px; padding-top: 5px">
                                                <roUserControls:roOptionPanelClient ID="optNumberContractNew" runat="server" TypeOPanel="RadioOption"
                                                    Width="100%" Height="Auto">
                                                    <Title>
                                                        <asp:Label ID="lblNumberContractNewTitle" runat="server" Text="Crear nuevos números de contrato"></asp:Label>
                                                    </Title>
                                                    <Description>
                                                        <asp:Label ID="lblNumberContractNewDescription" runat="server" Text="Los nuevos números de contrato se crearán según los límites que especifique."></asp:Label>
                                                    </Description>
                                                    <Content>
                                                        <table cellpadding="0" cellspacing="5" style="padding-left: 10px; padding-top: 5px;"
                                                            width="100%">
                                                            <tr>
                                                                <td align="right">
                                                                    <asp:Label ID="lblNumberContract" runat="server" Text="Comenzar a crear los números de contrato desde el número:"></asp:Label>
                                                                </td>
                                                                <td align="left" style="padding-left: 10px;">
                                                                    <dx:ASPxTextBox ID="txtNumberContract" runat="server" MaxLength="10">
                                                                        <MaskSettings Mask="<0..9999999999>" IncludeLiterals="None" />
                                                                        <ClientSideEvents KeyPress="devKeyPress" />
                                                                    </dx:ASPxTextBox>
                                                                    <%--<input type="text" id="txtNumberContract" runat="server" class="textClass" convertcontrol="NumberField"
                                                                        ccallowblank="false" ccmaxlength="10" ccallowdecimals="false" style="width: 80px;"
                                                                        onkeypress="return onEnterPress(event,'if (CheckFrame() == true) {__doPostBack(\'btNext\',\'\');}');" />--%>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </Content>
                                                </roUserControls:roOptionPanelClient>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="padding-top: 5px; padding-left: 10px;">
                                                <roUserControls:roGroupBox ID="gBox1" runat="server">
                                                    <Content>
                                                        <table style="width: 100%;">
                                                            <tr>
                                                                <td style="padding-left: 22px;">
                                                                    <asp:Label ID="lblLabAgree" runat="server" Font-Bold="true" Text="Convenio al que pertenecerá el/los empleado(s)"></asp:Label>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="center" style="padding-top: 10px;">
                                                                    <dx:ASPxComboBox ID="cmbLabAgree" runat="server" Width="200" />
                                                                    <%--<roWebControls:roComboBox ID="cmbLabAgree" runat="server" EnableViewState="True"
                                                                        ParentWidth="200px" HiddenText="cmbLabAgree_Text" HiddenValue="cmbLabAgree_Value"
                                                                        ChildsVisible="5" />
                                                                    <asp:HiddenField ID="cmbLabAgree_Text" runat="server" />
                                                                    <asp:HiddenField ID="cmbLabAgree_Value" runat="server" />--%>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </Content>
                                                </roUserControls:roGroupBox>
                                            </td>
                                        </tr>
                                    </table>
                                    <roUserControls:roOptPanelClientGroup ID="optNumberContractGroup" runat="server" />
                                </td>
                            </tr>
                        </table>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>

            <asp:UpdatePanel ID="updStep4" runat="server" RenderMode="Inline">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btPrev" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="btNext" EventName="Click" />
                </Triggers>
                <ContentTemplate>
                    <div id="divStep4" runat="server" style="display: none;">
                        <table style="" cellspacing="0" cellpadding="0">
                            <tr>
                                <td class="NewEmployeeWizards_StepTitle">
                                    <table style="width: 100%">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblStep4Title" runat="server" Text="" Font-Bold="True" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="padding-left: 20px">
                                                <asp:Label ID="lblSetp4Info" runat="server" Text="Defina los métodos de identificación a utilizar para identificarse." />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td class="NewEmployeeWizards_StepError popupWizardError">
                                    <asp:Label ID="lblStep4Error" runat="server" CssClass="errorText" />
                                </td>
                            </tr>
                            <tr>
                                <td class="NewEmployeeWizards_StepContent">
                                    <table style="width: 100%; height: 100%" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblStep4Info2" runat="server" Text="Defina los métodos de identificación." />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <div style="height: 240px; width: 100%; overflow: hidden;">
                                                    <roUserControls:IdentifyMethods ID="cnIdentifyMethods" ModoWizardNew="ModeNewMulti" Type="Employee"
                                                        runat="server" />
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>

            <asp:UpdatePanel ID="updStep5" runat="server" RenderMode="Inline">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btPrev" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="btNext" EventName="Click" />
                </Triggers>
                <ContentTemplate>
                    <div id="divStep5" runat="server" style="display: none;">
                        <table style="" cellspacing="0" cellpadding="0">
                            <tr>
                                <td class="NewEmployeeWizards_StepTitle">
                                    <table style="width: 100%">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblStep5Title" runat="server" Text="" Font-Bold="True" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="padding-left: 20px">
                                                <asp:Label ID="lblSetp5Info" runat="server" Text="Elija el modo de generar los números de ${Card}." />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td class="NewEmployeeWizards_StepError popupWizardError">
                                    <asp:Label ID="lblStep5Error" runat="server" CssClass="errorText" />
                                </td>
                            </tr>
                            <tr>
                                <td class="NewEmployeeWizards_StepContent">
                                    <table style="width: 100%;" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblStep5Info2" runat="server" Text="Seleccione el modo de crear los nuevos números de ${Card}. Si decide crear nuevos números de ${Card} debe especificar a partir de que número quiere empezar a crear los números de ${Card}." />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="padding-left: 10px; padding-top: 5px">
                                                <roUserControls:roOptionPanelClient ID="optCardsFromLast" runat="server" TypeOPanel="RadioOption"
                                                    Width="100%" Height="Auto" Enabled="True">
                                                    <Title>
                                                        <asp:Label ID="lblCardsFromLastTitle" runat="server" Text="Crear los números de ${Card} a partir del último número existente"></asp:Label>
                                                    </Title>
                                                    <Description>
                                                        <asp:Label ID="lblCardsFromLastDescription" runat="server" Text="Los nuevos números de ${Card} se crearán a continuación del último número existente."></asp:Label>
                                                    </Description>
                                                    <Content>
                                                    </Content>
                                                </roUserControls:roOptionPanelClient>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="padding-left: 10px; padding-top: 5px">
                                                <roUserControls:roOptionPanelClient ID="optCardsContract" runat="server" TypeOPanel="RadioOption"
                                                    Width="100%" Height="Auto" Enabled="True">
                                                    <Title>
                                                        <asp:Label ID="lblCardsContractTitle" runat="server" Text="Utilizar los números de contrato"></asp:Label>
                                                    </Title>
                                                    <Description>
                                                        <asp:Label ID="lblCardsContractDescription" runat="server" Text="Se utilizarán los números de contrato para crear los números de ${Card}."></asp:Label>
                                                    </Description>
                                                    <Content>
                                                    </Content>
                                                </roUserControls:roOptionPanelClient>
                                            </td>
                                        </tr>
                                        <td style="padding-left: 10px; padding-top: 5px">
                                            <roUserControls:roOptionPanelClient ID="optCardsFromNumber" runat="server" TypeOPanel="RadioOption"
                                                Width="100%" Height="Auto" Enabled="True">
                                                <Title>
                                                    <asp:Label ID="lblCardsFromNumberTitle" runat="server" Text="Crear nuevos números de ${Card}"></asp:Label>
                                                </Title>
                                                <Description>
                                                    <asp:Label ID="lblCardsFromNumberDescription" runat="server" Text="Los nuevos números de ${Card} se crearán según los límites que especifique."></asp:Label>
                                                </Description>
                                                <Content>
                                                    <table cellpadding="0" cellspacing="5" style="padding-left: 10px; padding-top: 5px;"
                                                        width="100%">
                                                        <tr>
                                                            <td align="right">
                                                                <asp:Label ID="lblCardsFromNumber" runat="server" Text="Comenzar a crear los números de ${Card} desde el número:"></asp:Label>
                                                            </td>
                                                            <td align="left" style="padding-left: 10px;">
                                                                <dx:ASPxTextBox ID="txtCardsFromNumber" runat="server" MaxLength="18">
                                                                    <MaskSettings Mask="<0..999999999999999999>" IncludeLiterals="None" />
                                                                    <ClientSideEvents KeyPress="devKeyPress" />
                                                                </dx:ASPxTextBox>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </Content>
                                            </roUserControls:roOptionPanelClient>
                                        </td>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>

            <asp:UpdatePanel ID="updStep6" runat="server" RenderMode="Inline">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btPrev" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="btNext" EventName="Click" />
                </Triggers>
                <ContentTemplate>
                    <div id="divStep6" runat="server" style="display: none;">
                        <table style="" cellspacing="0" cellpadding="0">
                            <tr>
                                <td class="NewEmployeeWizards_StepTitle">
                                    <table style="width: 100%">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblStep6Title" runat="server" Text="" Font-Bold="True" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="padding-left: 20px">
                                                <asp:Label ID="lblStep6Info" runat="server" Text="Si lo desea, puede introducir los nombres de los ${Employees} que se van a crear." />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td class="NewEmployeeWizards_StepError popupWizardError">
                                    <asp:Label ID="lblStep6Error" runat="server" CssClass="errorText" />
                                </td>
                            </tr>
                            <tr>
                                <td class="NewEmployeeWizards_StepContent">
                                    <table style="width: 100%;" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblStep6Info2" runat="server" Text="Indique los nombres de los nuevos ${Employees}." />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>

                                                <div class="jsGridContent">
                                                    <dx:ASPxGridView ID="gridEmployeesWizard" runat="server" AutoGenerateColumns="False" ClientInstanceName="GridEmployeesWizardClient" KeyboardSupport="True" Width="100%">
                                                        <Settings ShowTitlePanel="False" VerticalScrollBarMode="Auto" UseFixedTableLayout="True" VerticalScrollableHeight="150" />
                                                        <SettingsCommandButton>
                                                            <UpdateButton Image-Url="~/Base/Images/Grid/save.png" Image-ToolTip="" />
                                                            <CancelButton Image-Url="~/Base/Images/Grid/cancel.png" Image-ToolTip="" />
                                                            <EditButton Image-Url="~/Base/Images/Grid/edit.png" Image-ToolTip="" />
                                                        </SettingsCommandButton>
                                                        <Styles>
                                                            <AlternatingRow Enabled="True" BackColor="#d7e5ea"></AlternatingRow>
                                                            <CommandColumn Spacing="10px" />
                                                            <Header CssClass="jsGridHeaderCell" />
                                                            <Cell Wrap="False" />
                                                        </Styles>
                                                        <SettingsPager Mode="ShowAllRecords" ShowEmptyDataRows="false">
                                                        </SettingsPager>
                                                        <SettingsEditing Mode="Inline" />
                                                    </dx:ASPxGridView>
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>

            <asp:UpdatePanel ID="updStep7" runat="server" RenderMode="Inline">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btPrev" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="btNext" EventName="Click" />
                </Triggers>
                <ContentTemplate>
                    <div id="divStep7" runat="server" style="display: none;">
                        <table style="" cellspacing="0" cellpadding="0">
                            <tr>
                                <td class="NewEmployeeWizards_StepTitle">
                                    <table style="width: 100%">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblStep7Title" runat="server" Text="" Font-Bold="True" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="padding-left: 20px">
                                                <asp:Label ID="lblSetp7Info" runat="server" Text="Seleccione el tipo de los ${Employees}." />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td class="NewEmployeeWizards_StepContent">
                                    <table style="width: 100%;" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblStep7Info2" runat="server" Text="Seleccione el tipo de los ${Employees} que va a crear." />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="NewEmployeeWizards_StepError popupWizardError">
                                                <asp:Label ID="lblStep7Error" runat="server" CssClass="errorText" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <div style="height: 240px; width: 100%; overflow: auto;">
                                                    <Local:EmployeeType ID="cnEmployeeType" runat="server" ShowDisabledTypes="false" />
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>

            <asp:UpdatePanel ID="updStep8" runat="server" RenderMode="Inline">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btPrev" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="btNext" EventName="Click" />
                </Triggers>
                <ContentTemplate>
                    <div id="divStep8" runat="server" style="display: none;">
                        <table style="" cellspacing="0" cellpadding="0">
                            <tr>
                                <td class="NewEmployeeWizards_StepTitle">
                                    <table style="width: 100%">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblStep8Title" runat="server" Text="" Font-Bold="True" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="padding-left: 20px">
                                                <asp:Label ID="lblSetp8Info" runat="server" Text="Seleccione el grupo de usuarios al que pertenecerán los nuevos empleados." />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td class="NewEmployeeWizards_StepError popupWizardError">
                                    <asp:Label ID="lblStep8Error" runat="server" CssClass="errorText" />
                                </td>
                            </tr>
                            <tr>
                                <td valign="top">
                                    <table width="100%" style="margin-top: 10px;">
                                        <tr>
                                            <td style="padding-left: 10px;">
                                                <asp:CheckBox ID="chkParentGroupNull" Text="Los nuevos empleados no pertenecerán a ningún grupo de Usuarios."
                                                    Checked="true" runat="server" onclick="OnchkParentGroupNull(this);" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:HiddenField ID="hdnPassportSelected" runat="server" Value="" />
                                                <iframe id="ifPassportSelector" runat="server" style="background-color: Transparent; display: none;"
                                                    height="260" width="100%" scrolling="auto" frameborder="0" marginheight="0" marginwidth="0" src="" />
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