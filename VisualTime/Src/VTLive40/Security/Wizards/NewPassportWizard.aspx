<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Wizards_NewPassportWizard" Culture="auto" UICulture="auto" CodeBehind="NewPassportWizard.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Asistente para el alta de pasaportes</title>
</head>
<body class="bodyPopup" style="background-attachment: fixed;">

    <form id="frmNewPassportWizard" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <div>

            <script language="javascript" type="text/javascript">

                function PageBase_Load() {

                    var oActiveFrameIndex = document.getElementById('<%= hdnActiveFrame.ClientID %>').value;
                    ConvertControls('divStep' + oActiveFrameIndex);

                    checkOPCPanelClients();

                    //Enllaç dels OptionPanelClients
                    linkOPCItems('<%= optUserPassport.ClientID %>,<%= optEmployeePassport.ClientID %>');

                }

                function checkOPCPanelClients() {
                    venableOPC('<%= optUserPassport.ClientID %>');
                    venableOPC('<%= optEmployeePassport.ClientID %>');
                }

                //Llença aquest script al recarregar els updatepanels per poder controlar per js els opclient
                function endRequestHandler() {
                    checkOPCPanelClients();
                    IdentifyMethodsLoad();
                    hidePopupLoader();
                }

                function showPopupLoader() {
                    if (typeof (window.parent.frames["ifPrincipal"]) != "undefined") {
                        if (typeof (window.parent.frames["ifPrincipal"].showLoadingGrid) == "function") {
                            window.parent.frames["ifPrincipal"].showLoadingGrid(true);
                        } else {
                            window.parent.frames["ifPrincipal"][0].showLoadingGrid(true);
                        }
                    } else {
                        window.parent.parent.frames["ifPrincipal"].showLoadingGrid(true);
                    }
                }

                function hidePopupLoader() {
                    if (typeof (window.parent.frames["ifPrincipal"]) != "undefined") {
                        if (typeof (window.parent.frames["ifPrincipal"].showLoadingGrid) == "function") {
                            window.parent.frames["ifPrincipal"].showLoadingGrid(false);
                        } else {
                            window.parent.frames["ifPrincipal"][0].showLoadingGrid(false);
                        }
                    } else {
                        window.parent.parent.frames["ifPrincipal"].showLoadingGrid(false);
                    }
                }

                function CloseNewPopup() {
                    var MustRefresh = $get('hdnMustRefresh_PageBase').value;
                    var _Params = '';
                    if (MustRefresh != '0') {
                        try {
                            parent.frames['ifPrincipal'].frames[0].RefreshScreen(MustRefresh);
                            $("#hdnMustRefresh_PageBase").val('0');
                        } catch (e) { }
                    }
                    Close();
                }

                function EmployeeSelected(Nodo) {
                    var hdnSelected = document.getElementById('<%= Me.hdnEmployeeSelected.ClientID %>');
                    hdnSelected.value = Nodo.id;
                }

                function CheckFrame() {
                    var bolRet = true;
                    var oActiveFrameIndex = document.getElementById('<%= hdnActiveFrame.ClientID %>').value;

                    if (CheckConvertControls('divStep' + oActiveFrameIndex) == false) {
                        bolRet = false;
                    } else {
                        bolRet = true;
                    }
                    if (!bolRet) hidePopupLoader();
                    return bolRet;
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
                                        <asp:Image ID="imgNewPassportWizard" runat="server" Style="border-radius: 5px;" ImageUrl="~/Base/Images/Wizards/wzPassport.gif" />
                                    </td>
                                    <td style="padding-left: 20px; padding-right: 20px; padding-top: 50px" valign="top">

                                        <asp:Label ID="lblWelcome1" runat="server" Text="Bienvenido al asistente generar un nuevo pasaporte."
                                            Font-Bold="True" Font-Size="Large"></asp:Label>
                                        <br />
                                        <br />
                                        <br />
                                        <asp:Label ID="lblWelcome2" runat="server" Text="El asistente le guiará en la creación de un nuevo pasaporte." Font-Bold="true"></asp:Label>
                                        <br />
                                        <br />
                                        <asp:Label ID="lblWelcome3" runat="server" Text="Para continuar, haga clic en siguiente."></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="height: 10px" colspan="2" class="PassportWizards_ButtonsPanel"></td>
                                </tr>
                            </table>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>

                <asp:Label ID="hdnStepTitle" Text="Asistente para crear un nuevo pasaporte. " runat="server" Style="display: none; visibility: hidden" />
                <asp:Label ID="hdnStepTitle2" Text="Paso {0} de {1}." runat="server" Style="display: none; visibility: hidden" />

                <asp:UpdatePanel ID="updStep1" runat="server" RenderMode="Inline">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btPrev" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btNext" EventName="Click" />
                    </Triggers>
                    <ContentTemplate>

                        <div id="divStep1" runat="server" style="display: none; width: 500px;">
                            <table style="width: 100%;" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td class="PassportWizards_StepTitle">
                                        <table style="width: 100%">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblStep1Title" runat="server" Text="" Font-Bold="True" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 20px">
                                                    <asp:Label ID="lblSetp1Info" runat="server" Text="Primero debe seleccionar el tipo del pasaporte a crear." />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="PassportWizards_StepError  popupWizardError">
                                        <asp:Label ID="lblStep1Error" runat="server" CssClass="errorText" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="PassportWizards_StepContent" style="padding-top: 5px;">

                                        <table style="width: 100%; height: 100%" cellspacing="0" cellpadding="0">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblStep1Info2" runat="server" Text="Seleccione el tipo de pasaporte a crear." />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 10px; padding-top: 3px">
                                                    <roUserControls:roOptionPanelClient ID="optUserPassport" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Enabled="True">
                                                        <Title>
                                                            <asp:Label ID="lblUserPassportTitle" runat="server" Text="Supervisor"></asp:Label>
                                                        </Title>
                                                        <Description>
                                                            <table cellpadding="0" cellspacing="5" style="padding-left: 10px; padding-top: 5px;" width="100%">
                                                                <tr>
                                                                    <td valign="top">
                                                                        <asp:Image ID="Image2" ImageUrl="~/Security/Images/Passport_32.png" Width="32" Height="32" runat="server" />
                                                                    </td>
                                                                    <td align="left" valign="middle">
                                                                        <asp:Label ID="lblUserPassportDescription" runat="server" Text="..."></asp:Label>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </Description>
                                                        <Content>
                                                        </Content>
                                                    </roUserControls:roOptionPanelClient>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 10px; padding-top: 3px">
                                                    <roUserControls:roOptionPanelClient ID="optEmployeePassport" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Enabled="True">
                                                        <Title>
                                                            <asp:Label ID="lblEmployeePassportTitle" runat="server" Text="Supervisor a partir de un usuario"></asp:Label>
                                                        </Title>
                                                        <Description>
                                                            <table cellpadding="0" cellspacing="5" style="padding-left: 10px; padding-top: 5px;" width="100%">
                                                                <tr>
                                                                    <td valign="top">
                                                                        <asp:Image ID="Image3" ImageUrl="~/Security/Images/PassportEmployee_32.png" Width="32" Height="32" runat="server" />
                                                                    </td>
                                                                    <td align="left" valign="middle">
                                                                        <asp:Label ID="lblEmployeePassportDescription" runat="server" Text="..."></asp:Label>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </Description>
                                                        <Content>
                                                        </Content>
                                                    </roUserControls:roOptionPanelClient>
                                                </td>
                                            </tr>
                                        </table>

                                        <roUserControls:roOptPanelClientGroup ID="optGroup" runat="server" />
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

                        <div id="divStep2" runat="server" style="display: none">
                            <table id="btStep2" style="width: 100%;" cellspacing="0" cellpadding="0" border="0">
                                <tr>
                                    <td class="PassportWizards_StepTitle">
                                        <table style="width: 100%">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblStep2Title" runat="server" Text="Paso {0} de {1}." Font-Bold="True" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 20px">
                                                    <asp:Label ID="lblStep2Info" runat="server" Text="Introduzca los datos generales del pasaporte que quiere dar de alta." />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="PassportWizards_StepError popupWizardError">
                                        <asp:Label ID="lblStep2Error" runat="server" CssClass="errorText" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="PassportWizards_StepContent" valign="top">
                                        <table width="100%">
                                            <tr>
                                                <td colspan="2">
                                                    <asp:Label ID="lblStep2Info2" runat="server" Text="Introduzca el nombre y la descripción del pasaporte. " />
                                                    <br />
                                                    <br />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" style="padding-left: 20px; width: 75px;">
                                                    <asp:Label ID="lblPassportName" runat="server" Text="Nombre" />
                                                </td>
                                                <td align="left" style="padding-left: 10px;">
                                                    <asp:TextBox ID="txtPassportName" CssClass="textClass" runat="server" ConvertControl="TextField" CCallowBlank="false" CCmaxLength="50"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" valign="top" style="padding-left: 20px;">
                                                    <asp:Label ID="lblPassportDescription" runat="server" Text="Descripción" />
                                                </td>
                                                <td align="left" style="padding-left: 10px;">
                                                    <textarea id="txtPassportDescription" runat="server" style="height: 150px; width: 100%;" class="textClass" convertcontrol="AreaField"></textarea>
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

                        <div id="divStep3" runat="server" style="display: none">
                            <table id="tbStep3" runat="server" style="width: 100%;" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td class="PassportWizards_StepTitle">
                                        <table style="width: 100%">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblStep3Title" runat="server" Text="Paso {0} de {1}." Font-Bold="True" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 20px">
                                                    <asp:Label ID="lblStep3Info" runat="server" Text="Seleccione el periodo de validez del passaporte que quiere generar." />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="PassportWizards_StepError  popupWizardError">
                                        <asp:Label ID="lblStep3Error" runat="server" CssClass="errorText" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="PassportWizards_StepContent" valign="middle">

                                        <table>
                                            <tr>
                                                <td align="right" style="padding-left: 20px; width: 100px;">
                                                    <asp:Label ID="lblStartDate" runat="server" Text="Válido desde el día "></asp:Label>
                                                </td>
                                                <td align="left" style="padding-left: 10px;">
                                                    <dx:ASPxDateEdit ID="txtStartDate" PopupVerticalAlign="WindowCenter" runat="server" Width="115px" AllowNull="true" />
                                                    <%--<input type="text" id="txtStartDate" runat="server" style="width: 75px;" class="textClass" convertcontrol="DatePicker" />--%>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="right" style="padding-left: 20px; width: 100px;">
                                                    <asp:Label ID="lblExpirationDate" runat="server" Text=" hasta el día "></asp:Label>
                                                </td>
                                                <td align="left" style="padding-left: 10px;">
                                                    <dx:ASPxDateEdit ID="txtExpirationDate" PopupVerticalAlign="WindowCenter" runat="server" Width="115px" AllowNull="true" />
                                                    <%--<input type="text" id="txtExpirationDate" runat="server" style="width: 75px;" class="textClass" convertcontrol="DatePicker" />--%>
                                                </td>
                                            </tr>
                                        </table>
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

                        <div id="divStep4" runat="server" style="display: none">
                            <table runat="server" style="width: 100%;" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td class="PassportWizards_StepTitle">
                                        <table style="width: 100%">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblStep4Title" runat="server" Text="Paso {0} de {1}." Font-Bold="True" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 20px">
                                                    <asp:Label ID="lblStep4Info" runat="server" Text="Seleccione la ubicación del nuevo pasaporte." />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="PassportWizards_StepError popupWizardError">
                                        <asp:Label ID="lblStep4Error" runat="server" CssClass="errorText" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="PassportWizards_StepContent" style="padding-top: 5px;"> </td>
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

                        <div id="divStep5" runat="server" style="display: none">
                            <table runat="server" style="width: 100%;" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td class="PassportWizards_StepTitle">
                                        <table style="width: 100%">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblStep5Title" runat="server" Text="Paso {0} de {1}." Font-Bold="True" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 20px">
                                                    <asp:Label ID="lblStep5Info" runat="server" Text="Seleccione los permisos que se le aplicarán al nuevo grupo." />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="PassportWizards_StepError popupWizardError">
                                        <asp:Label ID="lblStep5Error" runat="server" CssClass="errorText" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="PassportWizards_StepContent">

                                        <asp:Label ID="lblStep5Info2" runat="server" Text="" />
                                        <br />
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

                        <div id="divStep6" runat="server" style="display: none">
                            <table runat="server" style="width: 100%;" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td class="PassportWizards_StepTitle">
                                        <table style="width: 100%">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblStep6Title" runat="server" Text="Paso {0} de {1}." Font-Bold="True" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 20px">
                                                    <asp:Label ID="lblStep6Info" runat="server" Text="Seleccione los métodos de identificación del nuevo pasaporte que está generando." />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="PassportWizards_StepError popupWizardError">
                                        <asp:Label ID="lblStep6Error" runat="server" CssClass="errorText" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="PassportWizards_StepContent" style="padding-top: 5px;">

                                        <asp:Label ID="lblStep6Info2" runat="server" Text="" />
                                        <br />
                                        <div class="RoundCorner" style="border: thin solid silver; height: 280px; width: 370px; overflow: auto; margin-left: auto; margin-right: auto;">
                                            <roUserControls:IdentifyMethods ID="cnIdentifyMethods" ModoWizardNew="ModeNewSingle" Type="Passport" runat="server" />
                                        </div>
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

                        <div id="divStep7" runat="server" style="display: none">
                            <table runat="server" style="width: 100%;" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td class="PassportWizards_StepTitle">
                                        <table style="width: 100%">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblStep7Title" runat="server" Text="Paso {0} de {1}." Font-Bold="True" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 20px">
                                                    <asp:Label ID="lblStep7Info" runat="server" Text="Seleccione el ${Employee} al que se le asignará el nuevo pasaporte." />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="PassportWizards_StepError  popupWizardError">
                                        <asp:Label ID="lblStep7Error" runat="server" CssClass="errorText" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="PassportWizards_StepContent">
                                        <%--
									    <asp:Label ID="lblStep7Info2" runat="server" Text="" />
									    <br />
                                        --%>
                                        <table width="100%">
                                            <tr>
                                                <td>
                                                    <asp:HiddenField ID="hdnEmployeeSelected" runat="server" Value="" />
                                                    <iframe id="ifEmployeeSelector" runat="server" style="background-color: Transparent" height="290" width="100%"
                                                        scrolling="auto" frameborder="0" marginheight="0" marginwidth="0" src="" />
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

                        <div id="divStep8" runat="server" style="display: none">
                            <table runat="server" style="width: 100%;" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td class="PassportWizards_StepTitle">
                                        <table style="width: 100%">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblStep8Title" runat="server" Text="Paso {0} de {1}." Font-Bold="True" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 20px">
                                                    <asp:Label ID="lblStep8Info" runat="server" Text="Medio de identificación en aplicaciones" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="PassportWizards_StepError popupWizardError">
                                        <asp:Label ID="lblStep8Error" runat="server" CssClass="errorText" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="PassportWizards_StepContent" style="padding-top: 5px;">

                                        <asp:Label ID="lblStep8Info2" runat="server" Text="" />
                                        <br />
                                        <div class="RoundCorner" style="height: 280px; width: 370px; overflow: auto; margin-left: auto; margin-right: auto;">
                                            <table width="100%">
                                                <tr>
                                                    <td colspan="2">
                                                        <asp:Label ID="lblCredential" runat="server" Text="Indica el correo electrónico y el nombre de usuario como medio de identificación en aplicaciones" />
                                                        <br />
                                                        <br />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="right" style="padding-left: 20px; width: 75px;">
                                                        <asp:Label ID="lblMail" runat="server" Text="Mail" />
                                                    </td>
                                                    <td align="left" style="padding-left: 10px;">
                                                        <asp:TextBox ID="txtMail" CssClass="textClass" runat="server" ConvertControl="TextField" CCallowBlank="false" CCmaxLength="50"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="right" style="padding-left: 20px; width: 75px;">
                                                        <asp:Label ID="lblCredential2" runat="server" Text="Usuario" />
                                                    </td>
                                                    <td align="left" style="padding-left: 10px;">
                                                        <asp:TextBox ID="txtCredential" CssClass="textClass" runat="server" ConvertControl="TextField" CCallowBlank="false" CCmaxLength="50"></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
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
                            <tr class="PassportWizards_ButtonsPanel" style="height: 44px">
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
                                    <asp:Button ID="btClose" Text="${Button.Cancel}" runat="server" OnClientClick="CloseNewPopup(); return false;" TabIndex="5" CssClass="btnFlat btnFlatBlack btnFlatAsp" />

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