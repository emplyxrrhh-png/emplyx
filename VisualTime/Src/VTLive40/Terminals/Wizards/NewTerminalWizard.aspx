<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.NewTerminalWizard" Culture="auto" UICulture="auto" CodeBehind="NewTerminalWizard.aspx.vb" %>

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

                    UpdateControls($('#<%= cmbType_Direction.ClientID %>').val(), $('#<%= cmbType_Value.ClientID %>').val());
                }

                /* addCssClass: Afegeix una nova Clase a un objecte */
                /*************************************************************************************************************/
                function addCssClass(obj, clsTxt) {
                    obj.className = obj.className + ' ' + clsTxt;
                }

                /* removeCssClass: Elimina una Clase a un objecte */
                /*************************************************************************************************************/
                function removeCssClass(obj, clsTxt) {
                    var parmCss = new Array();
                    parmCss = obj.className.split(" ");

                    obj.className = ''; //Reset dels CSS
                    //Carreguem tots els anteriors atributs
                    for (nCss = 0; nCss < parmCss.length; nCss++) {
                        if (parmCss[nCss] != clsTxt) {
                            obj.className = obj.className + ' ' + parmCss[nCss];
                        }
                    }
                }

                /* onmouseour Row (tr) */
                function rowOver(rowID) {
                    var table = document.getElementById(rowID);
                    var cells = table.getElementsByTagName("td");
                    for (var i = 0; i < cells.length; i++) {
                        addCssClass(cells[i], "gridRowOver");
                    }
                }

                /* onmouseout Row (tr) */
                function rowOut(rowID) {
                    var table = document.getElementById(rowID);
                    var cells = table.getElementsByTagName("td");
                    for (var i = 0; i < cells.length; i++) {
                        removeCssClass(cells[i], "gridRowOver");
                    }
                }

                /* selecció Row (tr) */
                function rowClick(rowID, ID, dTable) {
                    //alert('ID=' + ID);
                    document.getElementById('<%= TermToReplaceID.ClientID %>').value = ID;
                    var tParent = document.getElementById(dTable);
                    var tCells = tParent.getElementsByTagName("td");
                    for (var i = 0; i < tCells.length; i++) {
                        removeCssClass(tCells[i], "gridRowOver");
                        removeCssClass(tCells[i], "gridRowSelected");
                    }

                    var table = document.getElementById(rowID);
                    var cells = table.getElementsByTagName("td");
                    for (var i = 0; i < cells.length; i++) {
                        removeCssClass(cells[i], "gridRowOver");
                        addCssClass(cells[i], "gridRowSelected");
                    }
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

                function UpdateControls(TerminalType, TerminalName) {

                    if (TerminalType != '') {

                        var _TerminalSN = $("#trTerminalSN");
                        var _TerminalIP = $('#trTerminalIP');
                        var _TerminalRegister = $('#trTerminalRegister');
                        var _TerminalRegister2 = $('#trTerminalRegister2');

                        switch (TerminalType) {
                            case 'remote':
                                if (TerminalName.toUpperCase() == 'RXF' || TerminalName.toUpperCase() == 'VIRTUAL' || TerminalName.toUpperCase() == 'RXC' || TerminalName.toUpperCase() == 'RXCE') {
                                    _TerminalSN.css('display', 'none');
                                    _TerminalIP.css('display', '');
                                    _TerminalRegister.css('display', 'none');
                                    _TerminalRegister2.css('display', 'none');
                                }
                                else if (TerminalName.toUpperCase() == 'NFC' || TerminalName.toUpperCase() == 'MASTERASP') {
                                    _TerminalSN.css('display', 'none');
                                    _TerminalIP.css('display', 'none');
                                    _TerminalRegister.css('display', 'none');
                                    _TerminalRegister2.css('display', 'none');
                                }
                               else if (TerminalName.toUpperCase() == 'SUPREMA') {
                                        _TerminalSN.css('display', '');
                                        _TerminalIP.css('display', 'none');
                                        _TerminalRegister.css('display', 'none');
                                        _TerminalRegister2.css('display', 'none');
                                    }
                                else {
                                    _TerminalSN.css('display', '');
                                    _TerminalIP.css('display', '');
                                    _TerminalRegister.css('display', '');
                                    _TerminalRegister2.css('display', '');
                                }

                                break;

                            case 'web':
                                _TerminalSN.css('display', 'none');
                                _TerminalIP.css('display', 'none');
                                _TerminalRegister.css('display', 'none');
                                _TerminalRegister2.css('display', 'none');
                                break;

                            case '':
                                _TerminalSN.css('display', 'none');
                                _TerminalRegister.css('display', 'none');
                                _TerminalRegister2.css('display', 'none');
                                break;
                        }

                    }

                }
            </script>

            <asp:UpdatePanel ID="upNewTerminalWizard" runat="server" RenderMode="Inline">
                <ContentTemplate>

                    <div class="popupWizardContent">
                        <%-- WELCOME --%>
                        <div id="divStep0" runat="server" style="display: block">
                            <table id="tbStep0" style="width: 100%;" cellpadding="0" cellspacing="0">
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
                                        <asp:Label ID="lblStep1Title" runat="server" Text="Paso 1 de 4." Font-Bold="True" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="NewTerminalWizards_StepError popupWizardError" style="">
                                        <asp:Label ID="lblStep1Error" runat="server" CssClass="errorText" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="NewTerminalWizards_StepContent" valign="top" style="padding-top: 20px;">
                                        <table border="0" style="width: 100%;">
                                            <tr>
                                                <td colspan="2" style="padding-left: 10px; padding-bottom: 10px;">
                                                    <asp:Label ID="lblTerminalInfoTitle" runat="server" Text="Información del Terminal" Font-Bold="true"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 15px; width: 50%;">
                                                    <asp:Label ID="lblTerminalModel" runat="server" Text="Módelo del Terminal:"></asp:Label>
                                                </td>
                                                <td>
                                                    <roWebControls:roComboBox ID="cmbType" runat="server" EnableViewState="true" AutoResizeChildsWidth="True" ParentWidth="200px" ChildsVisible="8" ItemsRunAtServer="false" HiddenText="cmbType_Text" HiddenValue="cmbType_Value"></roWebControls:roComboBox>
                                                    <input type="hidden" id="cmbType_Text" runat="server" />
                                                    <input type="hidden" id="cmbType_Value" runat="server" />
                                                    <input type="hidden" id="cmbType_Direction" runat="server" />
                                                </td>
                                            </tr>
                                            <tr id="trTerminalSN">
                                                <td style="padding-left: 15px; width: 50%;">
                                                    <asp:Label ID="lblTerminalSN" runat="server" Text="Núm. de Serie:"></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="TerminalSN" runat="server" Text="" Width="150px" CssClass="textClass" onkeypress="return onEnterPress(event,'');"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr id="trTerminalIP">
                                                <td style="padding-left: 15px; width: 50%;">
                                                    <asp:Label ID="lblTerminalIP" runat="server" Text="Dirección del Terminal:"></asp:Label>
                                                </td>
                                                <!-- aqui controles de la ip
                                            -->
                                                <td>
                                                    <asp:TextBox ID="TerminalAddress" runat="server" Text="" Width="235px" CssClass="textClass" onkeypress="return onEnterPress(event,'');"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr id="trTerminalRegister">
                                                <td colspan="2" style="padding-top: 30px; padding-left: 10px; padding-bottom: 10px;">
                                                    <asp:Label ID="lblTerminalInfoRegister" runat="server" Text="Núm. de Licencia del Terminal" Font-Bold="true"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr id="trTerminalRegister2">
                                                <td colspan="2" align="center">
                                                    <table border="0" style="width: 90%; height: 100%;">
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
                                    </td>
                                </tr>
                            </table>
                        </div>

                        <div id="DivStep2" runat="server" style="display: none">
                            <table id="btStep2" style="" cellspacing="0" cellpadding="0" border="0">
                                <tr>
                                    <td class="NewTerminalWizards_StepTitle" valign="top" style="">
                                        <asp:Label ID="lblStep2Title" runat="server" Text="Paso 2 de 4." Font-Bold="True" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="NewTerminalWizards_StepError popupWizardError" style="">
                                        <asp:Label ID="lblStep2Error" runat="server" CssClass="errorText" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="NewTerminalWizards_StepContent" valign="top" style="padding-top: 20px;">
                                        <table border="0" style="width: 100%;">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblTerminalTypeDesc" runat="server" Text="Seleccione la acción que corresponda realizar." />
                                                    <br />
                                                    <table>
                                                        <tr>
                                                            <td style="padding-top: 10px">
                                                                <roUserControls:roOptionPanelContainer ID="optNewTerminal" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto"
                                                                    Text="Es un nuevo ${Terminal}"
                                                                    Description="El ${Terminal} se crea nuevamente, no se sustituye otro terminal."
                                                                    Checked="true">
                                                                    <Content></Content>
                                                                </roUserControls:roOptionPanelContainer>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td style="padding-top: 10px">
                                                                <roUserControls:roOptionPanelContainer ID="optReplaceTerminal" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto"
                                                                    Text="Se reemplaza un ${Terminal} existente"
                                                                    Description="Se sustituye un ${Terminal} existente por el nuevo Terminal. Se mantiene la configuración del anterior."
                                                                    Enabled="true">
                                                                    <Content></Content>
                                                                </roUserControls:roOptionPanelContainer>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                    <roUserControls:roOptionPanelGroup ID="optTerminalGroup" runat="server" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </div>

                        <div id="divStep3" runat="server" style="display: none">
                            <table id="tbStep3" runat="server" style="" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td class="NewTerminalWizards_StepTitle" valign="top">
                                        <asp:Label ID="lblStep3Title" runat="server" Text="Paso 3 de 4." Font-Bold="True" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="NewTerminalWizards_StepError popupWizardError" style="">
                                        <asp:Label ID="lblStep3Error" runat="server" CssClass="errorText" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="NewTerminalWizards_StepContent" valign="top" style="padding-top: 20px;">
                                        <table border="0" style="width: 100%;">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblReplaceTermDesc" runat="server" Text="Seleccione el terminal que desea reemplazar." />
                                                    <br />
                                                    <table>
                                                        <tr>
                                                            <td style="padding-top: 10px; padding-left: 20px;" align="center">
                                                                <!-- Grid Terminals -->
                                                                <div style="width: 420px; height: 280px; display: block; text-align: left; border: solid 1px #D2DCE4;" runat="server" id="grdTerminales">
                                                                </div>
                                                                <asp:HiddenField ID="TermToReplaceID" runat="server" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </div>

                        <div id="divStep4" runat="server" style="display: none;">
                            <table id="tbStep4" runat="server" style="" cellspacing="0" cellpadding="0" border="0">
                                <tr>
                                    <td class="NewTerminalWizards_StepTitle" valign="top" style="">
                                        <asp:Label ID="lblStep4Title" runat="server" Text="Paso 4 de 4." Font-Bold="True" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="NewTerminalWizards_StepError popupWizardError" style="">
                                        <asp:Label ID="lblStep4Error" runat="server" CssClass="errorText" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="NewTerminalWizards_StepContent" valign="top" style="padding-top: 20px;">
                                        <table border="0" style="width: 100%;">
                                            <tr>
                                                <td colspan="2" align="center">
                                                    <table border="0" style="width: 90%; height: 100%;">
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="lblInsertName" runat="server" Text="Inserte el nombre descriptivo que desea para el terminal."></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td style="padding-top: 10px;" align="center">
                                                                <asp:TextBox ID="txtNameTerminal" runat="server" Text="" Width="350px" CssClass="textClass" onkeypress="return onEnterPress(event,'__doPostBack(\'btEnd\',\'\')');"></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                    </table>
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