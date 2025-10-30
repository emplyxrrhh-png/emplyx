<%@ Page Language="VB" MasterPageFile="~/Base/mastEmp.master" AutoEventWireup="false" Inherits="VTLive40.EmergencyReport" Title="Opciones de configuración" EnableEventValidation="True" EnableViewState="True" EnableSessionState="True" CodeBehind="EmergencyReport.aspx.vb" %>

<asp:Content ID="cMainBody" ContentPlaceHolderID="contentMainBody" runat="Server">

    <script language="javascript" type="text/javascript">

        function PageBase_Load() {

            ConvertControls();

            $get('panEmergencyOptions').style.display = '';

            // Reestablezco el tab activo
            if ($get('<%= ConfigurationOptions_TabVisibleName.ClientID %>').value != '') {
                SelectTab($get('<%= ConfigurationOptions_TabVisibleName.ClientID %>').value);
            }
            else {
                SelectTab('panEmergencyOptions');
            }

            checkOPCPanelClients();

            //Enllaç dels OptionPanelClients
            checkOnChangeOPCheck('<%= opCheckEmergencyActive.ClientID %>', '0', 'true');

            if ($get('<%= hdnChanged.ClientID %>').value == '0' || $get('<%= hdnChanged.ClientID %>').value == '') {
                hasChanges(false);
            } else {
                hasChanges(true);
            }

            top.focus();
        }

        function SelectTab(SelectedTab) {

            // Hacer invisibles los panels
            $get('panEmergencyOptions').style.display = 'none';

            $get('<%= TABBUTTON_EmergencyOptions.ClientID %>').className = 'bTab';

            var TabID;
            if (SelectedTab == 'panEmergencyOptions') {
                TabID = 'panEmergencyOptions';
                $get('<%= TABBUTTON_EmergencyOptions.ClientID %>').className = 'bTab-active';
            }

            $get(TabID).style.display = 'block';
            $get('<%= ConfigurationOptions_TabVisibleName.ClientID %>').value = TabID;

        }

        function RefreshScreen(DataType) {
            ButtonClick($get('<%= btRefresh.ClientID %>'));
        }

        function checkOPCPanelClients() {
            venableOPC('<%= opCheckEmergencyActive.ClientID%>');
        }

        //Llença aquest script al recarregar els updatepanels per poder controlar per js els opclient
        function endRequestHandler() {
            checkOPCPanelClients();
        }

        function saveChanges() {
            try {
                if (CheckSave() == true) {
                    showLoadingGrid(true);
                    __doPostBack('<%= btSave.ClientID %>');
                }

            } catch (e) { showError("saveChanges", e); }
        }

        function undoChanges() {
            try {
                __doPostBack('<%= btCancel.ClientID %>');
                //hasChanges(false);
            } catch (e) { showError("undoChanges", e); }
        }
    </script>

    <input type="hidden" runat="server" id="hdnModeEdit" value="" />
    <input type="hidden" id="msgHasChanges" value="" runat="server" clientidmode="Static" />
    <input type="hidden" id="msgHasErrors" value="" runat="server" clientidmode="Static" />

    <div id="divMainBody">
        <!-- TAB SUPERIOR -->
        <div id="divTabInfo" class="divDataCells">
            <div style="min-height: 10px"></div>
            <div id="divStartRibbon" class="blackRibbonTitle" style="">
                <div class="blackRibbonIcon">
                    <asp:Image ID="imgConfigurationOptions" ImageUrl="Images/EmergencyPrint.png" runat="server" />
                </div>
                <div class="blackRibbonDescription">
                    <asp:Label ID="lblHeader" runat="server" Text="Informe de emergencia" CssClass="NameText"></asp:Label>
                    <br />
                    <asp:Label ID="lblInfo" runat="server" Text="Esta es la pantalla donde podrá habilitar la página específica para el lanzamiento del informe de emergéncia y configurar su clave de seguridad."></asp:Label>
                </div>
                <div class="blackRibbonButtons" style="">
                    <table border="0" cellpadding="0" cellspacing="0" width="100%" style="padding: 2px 5px 5px 5px;">
                        <tr>
                            <td style="width: 100px;" valign="middle"></td>
                            <td valign="top" style="padding-top: 10px; padding-bottom: 20px;"></td>
                            <td id="rowTabButtons1" runat="server" align="right" valign="top" style="width: 140px; padding-top: 0px; padding-right: 1px;">
                                <a id="TABBUTTON_EmergencyOptions" href="javascript: void(0);" class="bTab" onclick="SelectTab('panEmergencyOptions');" runat="server">
                                    <asp:Label ID="lblEmergencyOptionsTabButton" Text="Emergencia" runat="server" /></a>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
            <div style="min-height: 10px"></div>
        </div>
        <!-- FIN TAB SUPERIOR -->
        <!-- DETALLE -->
        <div id="divTabData" class="divDataCells">
            <div id="divContenido" class="divAllContent">
                <div id="divContent" style="height: initial;" class="maxHeight">
                    <asp:UpdatePanel ID="upBody" runat="server">
                        <ContentTemplate>
                            <div style="margin: 5px;">
                                <table cellpadding="0" cellspacing="0" style="width: 100%; height: 100%; padding-left: 10px; padding-right: 10px;">
                                    <tr>
                                        <td valign="top" style="padding-top: 2px;">
                                            <!-- Mensajes -->
                                            <div id="divMsgTop" class="divMsg" style="display: none;">
                                                <table style="width: 100%;">
                                                    <tr>
                                                        <td align="center" style="width: 20px; height: 16px;">
                                                            <img id="Img1" src="~/Base/Images/MessageFrame/Alert16.png" runat="server" /></td>
                                                        <td align="left" style="padding-left: 10px; color: white;"><span id="msgTop"></span></td>
                                                        <td align="right" style="color: White; padding-right: 10px;">
                                                            <a href="javascript: void(0);" class="aMsg" onclick="saveChanges();"><span id="lblSaveChanges" runat="server" /></a>
                                                            &nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;
                                                            <a href="javascript: void(0);" onclick="undoChanges();" class="aMsg"><span id="lblUndoChanges" runat="server" /></a>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>
                                        </td>
                                    </tr>

                                    <tr>
                                        <td valign="top">

                                            <div id="panEmergencyOptions" style="width: 100%; display: none;">
                                                <table id="tbEmergencyOptions" runat="server" cellpadding="0" cellspacing="0" width="100%">
                                                    <tr>
                                                        <td>
                                                            <div class="panHeader2">
                                                                <span style="">
                                                                    <asp:Label ID="Label55" runat="server" Text="Emergencia" Font-Bold="true" /></span>
                                                            </div>
                                                            <br />
                                                        </td>
                                                    </tr>
                                                    <tr style="padding-top: 5px">
                                                        <td style="padding-left: 20px">
                                                            <asp:Label ID="lblEmergencyOptionsInfo" Text="En este formulario configuraremos parámetros del informe de Emergencia." runat="server"></asp:Label>
                                                            <br />
                                                            <br />
                                                        </td>
                                                    </tr>
                                                    <tr id="Tr33" style="padding-top: 20px; padding-bottom: 20px; padding-left: 60px; vertical-align: top;">
                                                        <td style="padding-left: 20px">
                                                            <ajaxToolkit:TabContainer ID="tcEmergencyOptions" CssClass="" runat="server">
                                                                <ajaxToolkit:TabPanel ID="tpEmergencyOptions_FreezeDate" runat="server" ScrollBars="Auto">
                                                                    <HeaderTemplate>
                                                                        <asp:Label ID="lblParametersTitle" Text="Informe de Emergencia" runat="server" />
                                                                    </HeaderTemplate>
                                                                    <ContentTemplate>
                                                                        <table cellpadding="0" cellspacing="0" style="padding-left: 10px; padding-top: 20px">
                                                                            <tr>
                                                                                <td>
                                                                                    <asp:Label ID="lblEmergencyReportInfo" Text="Seleccione el modo en el que cualquier usuario podrá lanzar el informe de emergencia." ForeColor="blue" runat="server" />
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td style="padding-top: 5px; padding-left: 30px;">
                                                                                    <roUserControls:roOptionPanelClient ID="opCheckEmergencyActive" runat="server" TypeOPanel="CheckboxOption" Width="100%" Height="Auto" Checked="False" Enabled="True">
                                                                                        <Title>
                                                                                            <asp:Label ID="opCheckEmergencyActiveTitle" runat="server" Text="Activar pantalla de Informes de Emergencia"></asp:Label>
                                                                                        </Title>
                                                                                        <Description></Description>
                                                                                        <Content>
                                                                                            <table>
                                                                                                <tr>
                                                                                                    <td valign="middle" align="right" style="padding-left: 25px; padding-right: 10px;">
                                                                                                        <asp:Label ID="lblClave" runat="server" Text="Clave"></asp:Label>
                                                                                                    </td>
                                                                                                    <td>
                                                                                                        <%--<input type="password" runat="server" id="txtEmergencyReportKey" class="textClass x-form-text x-form-field" maxlength="15" style="width: 80px;" convertcontrol="TextField" cconchange="hasChanges(true);" />--%>
                                                                                                        <%--<dx:ASPxTextBox id="txtEmergencyReportKey" runat="server" Password="true" MaxLength="15" Width="80px">
                                                                                                            <ClientSideEvents TextChanged="function(s,e){hasChanges(true)}" />
                                                                                                        </dx:ASPxTextBox>--%>
                                                                                                        <asp:TextBox ID="txtEmergencyReportKey" runat="server" TextMode="Password" Width="150px" onChange="hasChanges(true);" />
                                                                                                    </td>
                                                                                                </tr>
                                                                                            </table>
                                                                                        </Content>
                                                                                    </roUserControls:roOptionPanelClient>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td style="padding-top: 10px;">
                                                                                    <asp:Label ID="lblURL" Text="La URL de la página es:" ForeColor="blue" runat="server" />
                                                                                    <a id="aURLDescript" runat="server" href="#" />
                                                                                    <%--<asp:Label ID="lblURLDescript" Text="\VTLive\Emergency\Emergency.aspx" ForeColor="blue" runat="server" />--%>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </ContentTemplate>
                                                                </ajaxToolkit:TabPanel>
                                                                <ajaxToolkit:TabPanel ID="tpEmergencyOptions_Database" runat="server" Visible="false">
                                                                    <HeaderTemplate>
                                                                        <asp:Label ID="lblEmergencyTitle" Text="Emergencia" runat="server" />
                                                                    </HeaderTemplate>
                                                                    <ContentTemplate>
                                                                    </ContentTemplate>
                                                                </ajaxToolkit:TabPanel>
                                                            </ajaxToolkit:TabContainer>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" valign="bottom" class="DetailFrame_Background" style="padding-right: 20px; height: 100%; vertical-align: bottom;">
                                            <table>
                                                <tr align="right">
                                                    <td>
                                                        <asp:Button ID="btSave" Text="${Button.ApplyChanges}" runat="server" OnClientClick="return CheckSave();" Visible="false" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                                    </td>
                                                    <td>
                                                        <asp:Button ID="btCancel" Text="${Button.UndoChanges}" runat="server" Visible="false" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <!-- Mensajes -->
                                            <div id="divMsgBottom" class="divMsg" style="display: none;">
                                                <table style="width: 100%;">
                                                    <tr>
                                                        <td align="center" style="width: 20px; height: 16px;">
                                                            <img id="Img2" src="~/Base/Images/MessageFrame/Alert16.png" runat="server" /></td>
                                                        <td align="left" style="padding-left: 10px; color: white;"><span id="msgBottom"></span></td>
                                                        <td align="right" style="color: White; padding-right: 10px;">
                                                            <a href="javascript: void(0);" class="aMsg" onclick="saveChanges();"><span id="lblSaveChangesBottom" runat="server" /></a>
                                                            &nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;
                                                            <a href="javascript: void(0);" onclick="undoChanges();" class="aMsg"><span id="lblUndoChangesBottom" runat="server" /></a>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>
                                        </td>
                                    </tr>
                                </table>

                                <asp:Button ID="btRefresh" runat="server" Style="display: none;" />

                                <asp:HiddenField ID="hdnChanged" runat="server" />

                                <asp:HiddenField ID="hdnIsPostBack_PageBase" runat="server" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>
    <asp:HiddenField ID="ConfigurationOptions_TabVisibleName" Value="" runat="server" />

    <Local:MessageFrame ID="MessageFrame1" runat="server" />
</asp:Content>