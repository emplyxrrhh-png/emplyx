<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.WebUserControls_frmTerminalReaderV2" CodeBehind="frmTerminalReaderV2.ascx.vb" %>

<%@ Register Src="~/Terminals/WebUserForms/frmCfgInteractive.ascx" TagName="frmCfgInteractive" TagPrefix="roForms" %>

<input type="hidden" id="HDNHP" runat="server" />
<input type="hidden" id="HDNVAL" runat="server" />
<!-- Controls Flash Readers -->
<input type="hidden" id="FlPositionIn" runat="server" />
<input type="hidden" id="FlReadersIn" runat="server" />
<input type="hidden" id="FlZoneImgIn" runat="server" />
<input type="hidden" id="FlActualReaderIn" runat="server" />
<input type="hidden" id="FlPositionOut" runat="server" />
<input type="hidden" id="FlReadersOut" runat="server" />
<input type="hidden" id="FlZoneImgOut" runat="server" />
<input type="hidden" id="FlActualReaderOut" runat="server" />

<table border="0" width="100%" cellpadding="0" cellspacing="0" style="padding: 5px;">
    <tr>
        <td style="width: 110px; padding-left: 5px; padding-top: 15px;" align="right" valign="top">
            <!-- Pan Vert -->
            <table border="0" cellpadding="0" cellspacing="0">
                <tr>
                    <td>
                        <a href="javascript: void(0);" name="VertBtn_<%= Me.ClientID %>" class="tabVertActive" onclick="changeVertTabs(0, '<%= Me.ClientID %>');">
                            <asp:Label ID="lblPunch" runat="server" Text="Fichaje"></asp:Label></a>
                    </td>
                </tr>
                <tr id="validationTab" runat="server">
                    <td>
                        <a href="javascript: void(0);" name="VertBtn_<%= Me.ClientID %>" class="tabVertInactive" onclick="changeVertTabs(1, '<%= Me.ClientID %>');">
                            <asp:Label ID="lblValidation" runat="server" Text="Validación"></asp:Label></a>
                    </td>
                </tr>
                <tr>
                    <td>
                        <a href="javascript: void(0);" name="VertBtn_<%= Me.ClientID %>" class="tabVertInactive" onclick="changeVertTabs(2, '<%= Me.ClientID %>');">
                            <asp:Label ID="lblPosition" runat="server" Text="Posición"></asp:Label></a>
                    </td>
                </tr>
                <tr id ="simpleDispTab" runat="server">
                    <td>
                        <a href="javascript: void(0);" name="VertBtn_<%= Me.ClientID %>" class="tabVertInactive" onclick="changeVertTabs(3, '<%= Me.ClientID %>');">
                            <asp:Label ID="lblSimpleDev" runat="server" Text="Disp. simple"></asp:Label></a>
                    </td>
                </tr>
                 <tr id="ticketTab" runat="server">
                    <td>
                        <a href="javascript: void(0);" name="VertBtn_<%= Me.ClientID %>" class="tabVertInactive" onclick="changeVertTabs(4, '<%= Me.ClientID %>');">
                            <asp:Label ID="lblTicket" runat="server" Text="Ticket"></asp:Label></a>
                    </td>
                </tr>
            </table>
        </td>
        <td style="width: auto; min-height: 350px; height: 100%; border: solid 1px silver;" valign="top">
            <!-- Fichaje -->
            <div style="width: auto; height: auto; padding: 10px;" id="Div0_<%= Me.ClientID %>" name="VertDiv_<%= Me.ClientID %>">
                <table width="95%">
                    <tr style="display: none">
                        <td style="padding-left: 10px;">
                            <input type="checkbox" runat="server" id="chkUseDispKey" />&nbsp;
                            <a href="javascript: void(0);" onclick="CheckLinkChange('<%= chkUseDispKey.ClientID %>');">
                                <asp:Label ID="lblUseDispKey" runat="server" Text="Permitir interactuar con el teclado / pantalla" Font-Bold="true"></asp:Label></a>
                        </td>
                    </tr>

                    <tr>
                        <td>
                            <div class="panHeader2">
                                <span style="">
                                    <asp:Label ID="lblWhatPunchesTitle" runat="server" Text="¿ Qué se ficha ?" Font-Bold="true" />
                                </span>
                            </div>
                            <br />
                        </td>
                    </tr>
                    <tr style="padding-top: 5px">
                        <td style="padding-left: 20px">
                            <asp:Label ID="lblWhatPunchesDescription" Text="En esta sección puede configurar el comportamiento del terminal." runat="server"></asp:Label>
                            <br />
                        </td>
                    </tr>

                    <tr>
                        <td style="padding-left: 20px;">
                            <roUserControls:roGroupBox ID="gBoxWhatPunches" runat="server">
                                <Content>
                                    <table>
                                        <tr>
                                            <td style="padding-left: 5px; padding-right: 5px; padding-top: 5px; text-align: left; min-width: 75px">
                                                <asp:Label ID="lblComportamiento" runat="server" CssClass="spanEmp-Class" Text="Comportamiento"></asp:Label>
                                            </td>
                                            <td style="padding-left: 15px; padding-top: 5px; width: 400px;">
                                                <input type="hidden" id="hdnTerminalModeSelected" value="" runat="server" />
                                                <input type="hidden" id="hdnLabelDirectionIn" value="" runat="server" />
                                                <input type="hidden" id="hdnLabelDirectionOut" value="" runat="server" />
                                                <input type="hidden" id="hdnLabelDirectionUndefined" value="" runat="server" />

                                                <dx:ASPxComboBox ID="cmbTerminalModes" runat="server" ClientInstanceName="cmbTerminalModesClient" Width="600px">
                                                </dx:ASPxComboBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="padding-left: 5px; padding-right: 5px; padding-top: 5px; text-align: left; min-width: 75px">
                                                <asp:Label ID="lblCostCenters" runat="server" CssClass="spanEmp-Class" Text="Centro de coste"></asp:Label>
                                            </td>
                                            <td style="padding-left: 15px; padding-top: 5px; width: 400px;">
                                                <dx:ASPxComboBox ID="cmbCostCenters" runat="server" Width="600px">
                                                    <ClientSideEvents SelectedIndexChanged="function(s,e){hasChanges(true);}" />
                                                </dx:ASPxComboBox>
                                            </td>
                                        </tr>
                                    </table>
                                </Content>
                            </roUserControls:roGroupBox>
                        </td>
                    </tr>

                    <tr id="whoPunches1" runat="server">
                        <td>
                            <div class="panHeader2">
                                <span style="">
                                    <asp:Label ID="lblWhoPunchesTitle" runat="server" Text="¿ Quién puede fichar ?" Font-Bold="true" />
                                </span>
                            </div>
                            <br />
                        </td>
                    </tr>
                    <tr style="padding-top: 5px"  id="whoPunches2" runat="server">
                        <td style="padding-left: 20px">
                            <asp:Label ID="lblWhoPunchesDescription" Text="En esta sección puede consultar los empleados que pueden fichar en el terminal." runat="server"></asp:Label>
                            <br />
                        </td>
                    </tr>
                    <tr  id="whoPunches3" runat="server">
                        <td style="padding-left: 20px;">
                            <div id="divCanPunchNoRestriction" runat="server" style="display: none">
                                <div class="panBottomMargin">
                                    <div class="divRow" style="margin-left: 0px;">
                                        <div class="divRowDescription" style="padding-left: 0px !important;">
                                            <asp:Label ID="lblAllEmployeesCanPunch" runat="server" Text="No dispone de ninguna restricción activa. Todos los empleados pueden fichar en el terminal."></asp:Label>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div id="divCanPunchRestrictedAuthorizations" runat="server" style="display: none">
                                <div class="panBottomMargin">
                                    <div class="divRow" style="margin-left: 0px;">
                                        <div class="divRowDescription" style="padding-left: 90px;">
                                            <asp:Label ID="lblCanPunchRestrictedAuthorizationsDesc" runat="server" Text="Solo los empleados que dispongan de las siguientes autorizaciones pueden fichar en el terminal"></asp:Label>
                                        </div>
                                        <asp:Label ID="lblCanPunchRestrictedAuthorizationsTitle" runat="server" Text="Autorizaciones:" CssClass="labelForm" Width="85px"></asp:Label>
                                        <div class="componentForm">
                                            <dx:ASPxTokenBox ID="tobAuthorizations" runat="server" Width="100%" ReadOnly="true">
                                            </dx:ASPxTokenBox>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div id="divNobodyCanPunch" runat="server" style="display: none">
                                <div class="panBottomMargin">
                                    <div class="divRow" style="margin-left: 0px;">
                                        <div class="divRowDescription" style="padding-left: 0px !important">
                                            <asp:Label ID="lblRestrictionActive" runat="server" Text="Ninguno de los empleados pueden fichar en el terminal."></asp:Label>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div id="divModeLegacy" runat="server" style="display: none">
                                <roUserControls:roGroupBox ID="gBoxWhoPunches" runat="server">
                                    <Content>
                                        <table style="width: 100%;">
                                            <tr>
                                                <td>
                                                    <div class="panHeader2">
                                                        <span style="">
                                                            <asp:Label ID="lblEmployeesLimit" runat="server" Text="Limitar a empleados" Font-Bold="true" />
                                                        </span>
                                                    </div>
                                                    <br />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="text-align: left; vertical-align: top">
                                                    <input type="hidden" id="hdnEmployees" value="" runat="server" />
                                                    <input type="hidden" id="hdnFilter" value="" runat="server" />
                                                    <input type="hidden" id="hdnFilterUser" value="" runat="server" />
                                                    <table border="0">
                                                        <tr>
                                                            <td style="padding-left: 5px; padding-right: 5px;">
                                                                <asp:Label ID="lblFEmployees" runat="server" CssClass="spanEmp-Class" Text="Empleado(s)"></asp:Label>
                                                            </td>
                                                            <td style="width: 200px; border: solid 1px #CCCCCC; background-color: #EEEEEF; display: block;">
                                                                <a href="javascript: void(0)" id="aFEmployees" runat="server" class="btnDDownMode"></a>
                                                                <div id="divFloatMenuE" runat="server" style="width: 200px; z-index: 999; border: solid 1px #CCCCCC; background-color: #EEEEEF; position: absolute; display: block; display: none;">
                                                                    <table border="0" style="width: 170px; margin-left: 10px; margin-right: 10px;">
                                                                        <tr>
                                                                            <td nowrap="nowrap"><a href="javascript: void(0)" id="<%= Me.ClientID %>_aEmpAll" class="btnMode" onclick="ShowSelector('<%= Me.ClientID %>',1);DDown_Out('<%= Me.ClientID %>');" style="width: 100%;">
                                                                                <asp:Label ID="lblAllEmp" runat="server" Text="Todos los empleados"></asp:Label></a></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td nowrap="nowrap"><a href="javascript: void(0)" id="<%= Me.ClientID %>_aEmpSelect" class="btnMode" onclick="ShowSelector('<%= Me.ClientID %>',2);DDown_Out('<%= Me.ClientID %>');" style="width: 100%;">
                                                                                <asp:Label ID="lblEmpSelect" runat="server" Text="Seleccionar.."></asp:Label></a></td>
                                                                        </tr>
                                                                    </table>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </Content>
                                </roUserControls:roGroupBox>
                            </div>
                        </td>
                    </tr>
                </table>
            </div>
            <!-- validacion -->
            <div style="width: auto; height: auto; padding: 10px; display: none;" id="Div1_<%= Me.ClientID %>" name="VertDiv_<%= Me.ClientID %>">
                <table width="95%">
                    <tr>
                        <td style="padding-left: 10px;">
                            <asp:Label ID="lblHowValidate" runat="server" Text="¿ Cómo se valida ?" Font-Bold="true"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <roUserControls:roOptionPanelClient ID="optServerLocal" runat="server" TypeOPanel="RadioOption" width="100%" height="Auto" Checked="False" Enabled="True" Border="True" CConClick="hasChanges(true)">
                                <Title>
                                    <asp:Label ID="lblServerLocal" runat="server" Text="Servidor - Local"></asp:Label>
                                </Title>
                                <Description>
                                    <asp:Label ID="lblServerLocalDesc" runat="server" Text="Server - Local lorem ipsum dolor est."></asp:Label>
                                </Description>
                                <Content>
                                </Content>
                            </roUserControls:roOptionPanelClient>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <roUserControls:roOptionPanelClient ID="optLocalServer" runat="server" TypeOPanel="RadioOption" width="100%" height="Auto" Checked="False" Enabled="True" Border="True" CConClick="hasChanges(true)">
                                <Title>
                                    <asp:Label ID="lblLocalServer" runat="server" Text="Local - Servidor"></asp:Label>
                                </Title>
                                <Description>
                                    <asp:Label ID="lblLocalServerDesc" runat="server" Text="Local - Server lorem ipsum dolor est."></asp:Label>
                                </Description>
                                <Content>
                                </Content>
                            </roUserControls:roOptionPanelClient>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <roUserControls:roOptionPanelClient ID="optLocal" runat="server" TypeOPanel="RadioOption" width="100%" height="Auto" Checked="False" Enabled="True" Border="True" CConClick="hasChanges(true)">
                                <Title>
                                    <asp:Label ID="lblLocal" runat="server" Text="Local"></asp:Label>
                                </Title>
                                <Description>
                                    <asp:Label ID="lblLocalDesc" runat="server" Text="Local lorem ipsum dolor est."></asp:Label>
                                </Description>
                                <Content>
                                </Content>
                            </roUserControls:roOptionPanelClient>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <roUserControls:roOptionPanelClient ID="optServer" runat="server" TypeOPanel="RadioOption" width="100%" height="Auto" Checked="False" Enabled="True" Border="True" CConClick="hasChanges(true)">
                                <Title>
                                    <asp:Label ID="lblServer" runat="server" Text="Servidor"></asp:Label>
                                </Title>
                                <Description>
                                    <asp:Label ID="lblServerDesc" runat="server" Text="Server lorem ipsum dolor est."></asp:Label>
                                </Description>
                                <Content>
                                </Content>
                            </roUserControls:roOptionPanelClient>
                        </td>
                    </tr>
                </table>
            </div>
            <!-- Posicion -->
            <div style="width: auto; height: auto; padding: 10px; display: none;" id="Div2_<%= Me.ClientID %>" name="VertDiv_<%= Me.ClientID %>">
                <roUserControls:roTabContainerClient ID="tabZones" runat="server" onEventTabClick="ctlTabZonePositions">
                    <TabTitle1>
                        <asp:Label ID="lblTabReaderIn" runat="server" Text="Entrada" Font-Bold="true"></asp:Label>
                    </TabTitle1>
                    <TabContainer1>
                        <table width="95%">
                            <tr>
                                <td>
                                    <table border="0">
                                        <tr>
                                            <td style="min-width: 50px; padding-left: 10px; padding-right: 5px;" align="right">
                                                <asp:Label ID="lblPosZoneIn" runat="server" Text="Zona entrada:" Font-Bold="true"></asp:Label>
                                            </td>
                                            <td>
                                                <input type="hidden" id="hdnCmbPosZoneInSelection" value="" runat="server" />
                                                <dx:ASPxComboBox ID="cmbPosZoneIn" runat="server" Width="200px">
                                                    <ClientSideEvents SelectedIndexChanged="function(s,e){ posZoneInIndexChanged(s,e); hasChanges(true);}" />
                                                </dx:ASPxComboBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="min-width: 50px; padding-left: 10px; padding-right: 5px;" align="right">
                                                <asp:Label ID="lblCameraIn" runat="server" Text="Cámara Entrada:" Font-Bold="true"></asp:Label>
                                            </td>
                                            <td>
                                                <dx:ASPxComboBox ID="cmbCameraIn" runat="server" Width="200px">
                                                    <ClientSideEvents SelectedIndexChanged="function(s,e){hasChanges(true);}" />
                                                </dx:ASPxComboBox>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </TabContainer1>
                    <TabTitle2>
                        <asp:Label ID="lblTabReaderOut" runat="server" Text="Salida" Font-Bold="true"></asp:Label>
                    </TabTitle2>
                    <TabContainer2>
                        <table width="95%">
                            <tr>
                                <td>
                                    <table border="0">
                                        <tr>
                                            <td style="min-width: 50px; padding-left: 10px; padding-right: 5px;" align="right">
                                                <asp:Label ID="lblPosZoneOut" runat="server" Text="Zona salida:" Font-Bold="true"></asp:Label>
                                            </td>
                                            <td>
                                                <input type="hidden" id="hdnCmbPosZoneOutSelection" value="" runat="server" />
                                                <dx:ASPxComboBox ID="cmbPosZoneOut" runat="server" Width="200px">
                                                    <ClientSideEvents SelectedIndexChanged="function(s,e){ posZoneOutIndexChanged(s,e); hasChanges(true);}" />
                                                </dx:ASPxComboBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="min-width: 50px; padding-left: 10px; padding-right: 5px;" align="right">
                                                <asp:Label ID="lblCameraOut" runat="server" Text="Cámara Entrada:" Font-Bold="true"></asp:Label>
                                            </td>
                                            <td>
                                                <dx:ASPxComboBox ID="cmbCameraOut" runat="server" Width="200px">
                                                    <ClientSideEvents SelectedIndexChanged="function(s,e){hasChanges(true);}" />
                                                </dx:ASPxComboBox>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </TabContainer2>
                </roUserControls:roTabContainerClient>
            </div>
            <!-- Rele -->
            <div style="width: auto; height: auto; padding: 10px; display: none;" id="Div3_<%= Me.ClientID %>" name="VertDiv_<%= Me.ClientID %>">
                <table width="95%">
                    <tr>
                        <td style="padding-left: 10px; padding-bottom: 5px;">
                            <asp:Label ID="lblSimpleDevTitle" runat="server" Text="Control de dispositivos simples" Font-Bold="true"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <roUserControls:roOptionPanelClient ID="optChkOutPut" runat="server" TypeOPanel="CheckboxOption" width="100%" height="Auto" Checked="false" Enabled="True" Border="True" CConClick="hasChanges(true);">
                                <Title>
                                    <asp:Label ID="lblChkOutPut" runat="server" Text="Siempre que se realice un fichaje correcto"></asp:Label>
                                </Title>
                                <Description>
                                    <asp:Label ID="lblChkOutPutDesc" runat="server" Text="Server lorem ipsum dolor est."></asp:Label>
                                </Description>
                                <Content>
                                    <table>
                                        <tr>
                                            <td style="padding-left: 50px;">
                                                <asp:Label ID="lblOutPutRelay" runat="server" Text="Activar relé"></asp:Label></td>
                                            <td style="padding-left: 5px;">
                                                <dx:ASPxComboBox ID="cmbOutputRelay" runat="server" Width="200px">
                                                    <ClientSideEvents SelectedIndexChanged="function(s,e){hasChanges(true);}" />
                                                </dx:ASPxComboBox>
                                            </td>
                                            <td style="padding-left: 5px;">
                                                <asp:Label ID="lblOutputDuration" runat="server" Text=" durante "></asp:Label></td>
                                            <td style="padding-left: 5px;">
                                                <input type="text" convertcontrol="NumberField" style="width: 50px; text-align: right;" class="textClass x-form-text x-form-field" runat="server" id="txtOutPutDuration" maxlength="3" ccallowblank="true" ccdecimalprecision="0" ccallowdecimals="false" onchange="hasChanges(true);" />
                                            </td>
                                            <td style="padding-left: 5px;">
                                                <asp:Label ID="lblOutputDurationSeconds" runat="server" Text=" segundos."></asp:Label></td>
                                        </tr>
                                    </table>
                                </Content>
                            </roUserControls:roOptionPanelClient>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <roUserControls:roOptionPanelClient ID="optChkInvalidOutPut" runat="server" TypeOPanel="CheckboxOption" width="100%" height="Auto" Checked="false" Enabled="True" Border="True" CConClick="hasChanges(true);">
                                <Title>
                                    <asp:Label ID="lblChkInvalidOutPut" runat="server" Text="Siempre que se realice un fichaje incorrecto"></asp:Label>
                                </Title>
                                <Description>
                                    <asp:Label ID="lblChkInvalidOutPutDesc" runat="server" Text="Server lorem ipsum dolor est."></asp:Label>
                                </Description>
                                <Content>
                                    <table>
                                        <tr>
                                            <td style="padding-left: 50px;">
                                                <asp:Label ID="lblInvalidOutputRelay" runat="server" Text="Activar relé"></asp:Label></td>
                                            <td style="padding-left: 5px;">
                                                <dx:ASPxComboBox ID="cmbInvalidOutputRelay" runat="server" Width="200px">
                                                    <ClientSideEvents SelectedIndexChanged="function(s,e){hasChanges(true);}" />
                                                </dx:ASPxComboBox>
                                            </td>
                                            <td style="padding-left: 5px;">
                                                <asp:Label ID="lblInvalidOutputDuration" runat="server" Text=" durante "></asp:Label></td>
                                            <td style="padding-left: 5px;">
                                                <input type="text" convertcontrol="NumberField" style="width: 50px; text-align: right;" class="textClass x-form-text x-form-field" runat="server" id="txtInvalidOutPutDuration" maxlength="3" align="right" ccallowblank="true" ccdecimalprecision="0" ccallowdecimals="false" onchange="hasChanges(true);" />
                                            </td>
                                            <td style="padding-left: 5px;">
                                                <asp:Label ID="lblInvalidOutputDurationSeconds" runat="server" Text=" segundos."></asp:Label></td>
                                        </tr>
                                    </table>
                                </Content>
                            </roUserControls:roOptionPanelClient>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <roUserControls:roOptionPanelClient ID="optChkCustomButtons" runat="server" TypeOPanel="CheckboxOption" width="100%" height="Auto" Checked="false" Enabled="True" Border="True" CConClick="hasChanges(true);">
                                <Title>
                                    <asp:Label ID="lblChkCustomButtons" runat="server" Text="Mostrar tecla"></asp:Label>
                                </Title>
                                <Description>
                                    <asp:Label ID="lblChkCustomButtonsDesc" runat="server" Text="Server lorem ipsum dolor est."></asp:Label>
                                </Description>
                                <Content>
                                    <table>
                                        <tr>
                                            <td style="padding-left: 50px;">
                                                <asp:Label ID="lblCustomRelay" runat="server" Text="Activar relé"></asp:Label></td>
                                            <td colspan="2">
                                                <table>
                                                    <tr>
                                                        <td style="padding-left: 8px;">
                                                            <dx:ASPxComboBox ID="cmbOutPutCustom" runat="server" Width="200px">
                                                                <ClientSideEvents SelectedIndexChanged="function(s,e){hasChanges(true);}" />
                                                            </dx:ASPxComboBox>
                                                        </td>
                                                        <td style="padding-left: 5px;">
                                                            <asp:Label ID="lblCustomDuration" runat="server" Text=" durante "></asp:Label></td>
                                                        <td style="padding-left: 5px;">
                                                            <input type="text" convertcontrol="NumberField" style="width: 50px; text-align: right;" class="textClass x-form-text x-form-field" runat="server" id="txtOutputCustomDuration" maxlength="3" align="right" ccallowblank="true" ccdecimalprecision="0" ccallowdecimals="false" onchange="hasChanges(true);" />
                                                        </td>
                                                        <td style="padding-left: 5px;">
                                                            <asp:Label ID="lblCustomSeconds" runat="server" Text=" segundos."></asp:Label></td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="padding-left: 50px;">
                                                <asp:Label ID="lbltxtButton" runat="server" Text="Texto botón"></asp:Label></td>
                                            <td style="padding-left: 10px;">
                                                <input type="text" id="txtTextButton" runat="server" style="width: 250px;" class="textClass x-form-text x-form-field" convertcontrol="TextField" ccallowblank="false" onchange="hasChanges(true);">
                                            </td>
                                        </tr>
                                    </table>

                                    <table>
                                        <tr>
                                            <td style="padding-left: 50px;">
                                                <input type="checkbox" id="chkOnlyEntrance" runat="server" onchange="hasChanges(true);" />&nbsp;
                                            <a href="javascript: void(0);" onclick="CheckLinkClick('<%= Me.chkOnlyEntrance.ClientID %>');">
                                                <asp:Label ID="lblOnlyEntrance" runat="server" Font-Bold="true" Text="Solo entrada"></asp:Label></a>
                                            </td>
                                        </tr>
                                    </table>
                                </Content>
                            </roUserControls:roOptionPanelClient>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <roUserControls:roOptionPanelClient ID="optChkPrintTicket" runat="server" TypeOPanel="CheckboxOption" width="100%" height="Auto" Checked="false" Enabled="True" Border="True" CConClick="hasChanges(true);">
                                <Title>
                                    <asp:Label ID="Label1" runat="server" Text="Siempre que se acceda al comedor"></asp:Label>
                                </Title>
                                <Description>
                                    <asp:Label ID="Label2" runat="server" Text="Se imprimirá un ticket por la impresora seleccionada. La impresora solo puede tener letras y números, sin espacios ni símbolos."></asp:Label>
                                </Description>
                                <Content>
                                    <table id="tablePrinters" runat="server">
                                        <tr>
                                            <td style="padding-left: 50px;">
                                                <asp:Label ID="Label3" runat="server" Text="Impresora de Ticket"></asp:Label></td>
                                            <td style="padding-left: 5px;">
                                                <dx:ASPxTextBox ID="cmbPrinters" runat="server" MaxLength="30" Width="200px" ClientInstanceName="cmbPrinters_Client">
                                                    <ClientSideEvents TextChanged="function(s,e){hasChanges(true);}" />
                                                    <ValidationSettings>
                                                        <RegularExpression ErrorText="(*)" ValidationExpression="^[a-zA-Z0-9]+$" />
                                                    </ValidationSettings>
                                                 </dx:ASPxTextBox>
                                            </td>
                                        </tr>
                                    </table>
                                </Content>
                            </roUserControls:roOptionPanelClient>
                        </td>
                    </tr>
                </table>
            </div>
            <div style="width: auto; height: auto; padding: 10px; display: none;" id="Div4_<%= Me.ClientID %>" name="VertDiv_<%= Me.ClientID %>">
                <table width="95%">
                    <tr>
                        <td style="padding-left: 10px; padding-bottom: 5px;">
                            <asp:Label ID="lblTicketTitle" runat="server" Text="Personalización del ticket comedor" Font-Bold="true"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <dx:aspxhtmleditor id="dxPrinterNameEditor" clientinstancename="dxPrinterNameEditorClient" runat="server" width="100%" height="360px">
                                <clientsideevents htmlchanged="function(s,e){ hasChanges(true); }" />
                                    <toolbars>
                                        <dx:htmleditortoolbar name="StandardToolbar1">
                                            <items>
                                                <dx:toolbarundobutton begingroup="True">
                                                </dx:toolbarundobutton>
                                                <dx:toolbarredobutton>
                                                </dx:toolbarredobutton>
                                                <dx:toolbarfontsizeedit begingroup="True">
                                                    <Items>
                                                        <dx:ToolbarListEditItem Text="1 (8pt)" Value="1" />
                                                        <dx:ToolbarListEditItem Text="2 (10pt)" Value="2" />
                                                        <dx:ToolbarListEditItem Text="3 (12pt)" Value="3" />
                                                        <dx:ToolbarListEditItem Text="4 (14pt)" Value="4" />
                                                        <dx:ToolbarListEditItem Text="5 (18pt)" Value="5" />
                                                        <dx:ToolbarListEditItem Text="6 (24pt)" Value="6" />
                                                        <dx:ToolbarListEditItem Text="7 (36pt)" Value="7" />
                                                    </Items>
                                                </dx:toolbarfontsizeedit>
                                                <dx:toolbarboldbutton begingroup="True">
                                                </dx:toolbarboldbutton>
                                                <dx:toolbaritalicbutton>
                                                </dx:toolbaritalicbutton>
                                                <dx:toolbarunderlinebutton>
                                                </dx:toolbarunderlinebutton>
                                                <dx:toolbarstrikethroughbutton>
                                                </dx:toolbarstrikethroughbutton>
                                                <dx:toolbarjustifyleftbutton begingroup="True">
                                                </dx:toolbarjustifyleftbutton>
                                                <dx:toolbarjustifycenterbutton>
                                                </dx:toolbarjustifycenterbutton>
                                                <dx:toolbarjustifyrightbutton>
                                                </dx:toolbarjustifyrightbutton>
                                                <dx:toolbarbackcolorbutton begingroup="True">
                                                </dx:toolbarbackcolorbutton>
                                                <dx:toolbarfontcolorbutton>
                                                </dx:toolbarfontcolorbutton>
                                            <dx:toolbarinsertplaceholderdialogbutton begingroup="True">
                                            </dx:toolbarinsertplaceholderdialogbutton>
                                        </items>
                                    </dx:htmleditortoolbar>
                                </toolbars>
                            </dx:aspxhtmleditor>
                        </td>
                    </tr>
                </table>
            </div>
        </td>
    </tr>
</table>