<%@ Page Language="VB" MasterPageFile="~/Base/mastEmp.master" AutoEventWireup="false" Inherits="VTLive40.AdvancedSecurity" Title="Opciones de configuración" EnableEventValidation="True" EnableViewState="True" EnableSessionState="True" CodeBehind="AdvancedSecurity.aspx.vb" %>

<asp:Content ID="cMainBody" ContentPlaceHolderID="contentMainBody" runat="Server">

    <script language="javascript" type="text/javascript">
        function PageBase_Load() {
            ConvertControls();
            $get('panDatabaseOptions').style.display = 'block';

            // Reestablezco el tab activo
            if ($get('<%= ConfigurationOptions_TabVisibleName.ClientID %>').value != '') {
                SelectTab($get('<%= ConfigurationOptions_TabVisibleName.ClientID %>').value);
            }
            else {
                SelectTab('panDatabaseOptions');
            }

            checkOPCPanelClients();

            if ($get('<%= hdnChanged.ClientID %>').value == '0' || $get('<%= hdnChanged.ClientID %>').value == '') {
                hasChanges(false);
            } else {
                hasChanges(true);
            }

            top.focus();
        }

        function checkActiveControls(s) {
            if (s.GetValue() == true) {
                txtBlockUserPeriodClient.SetEnabled(true);
            } else {
                txtBlockUserPeriodClient.SetEnabled(false);
            }
        }

        function RefreshScreen(DataType) {
            ButtonClick($get('<%= btRefresh.ClientID %>'));
        }

        function checkOPCPanelClients() {
            checkActiveControls(chkBlockUserClient);
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
                    <asp:Image ID="imgConfigurationOptions" ImageUrl="Images/SecurityFunctions.png" runat="server" Width="90px" Height="90px" />
                </div>
                <div class="blackRibbonDescription">
                    <asp:Label ID="lblHeader" runat="server" Text="Seguridad avanzada" CssClass="NameText"></asp:Label>
                    <br />
                    <asp:Label ID="lblInfo" runat="server" Text="Desde esta pantalla podrá configurar la seguridad avanzada de VisualTime."></asp:Label>
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
                                            <div id="panDatabaseOptions" style="width: 100%; display: none;">
                                                <table id="tbDatabaseOptions" runat="server" cellpadding="0" cellspacing="0" width="100%">
                                                    <tr>
                                                        <td>
                                                            <div class="panHeader2">
                                                                <span style="">
                                                                    <asp:Label ID="Label2" runat="server" Text="Bloqueo de usuarios inactivos" Font-Bold="true" /></span>
                                                            </div>
                                                            <br />
                                                        </td>
                                                    </tr>
                                                    <tr style="padding-top: 5px">
                                                        <td style="padding-left: 20px">
                                                            <asp:Label ID="lblDatabaseOptionsInfo" Text="En este formulario configuraremos el tiempo de inactividad con el que se bloqueará un usuario desde su último acceso a Visualtime" runat="server"></asp:Label>
                                                            <br />
                                                            <br />
                                                        </td>
                                                    </tr>
                                                    <tr id="RowInfo2" style="padding-top: 20px; padding-bottom: 20px; padding-left: 60px; vertical-align: top;">
                                                        <td style="padding-left: 20px">

                                                            <ajaxToolkit:TabContainer ID="tcDatabaseOptions" CssClass="" runat="server">
                                                                <ajaxToolkit:TabPanel ID="tpDatabaseOptions_BlockUser" runat="server" ScrollBars="Auto">
                                                                    <HeaderTemplate>
                                                                        <asp:Label ID="lblBlockUserTitle" Text="Tiempo de inactividad" runat="server" />
                                                                    </HeaderTemplate>
                                                                    <ContentTemplate>
                                                                        <table cellpadding="0" cellspacing="0" style="padding-left: 10px; padding-top: 10px; padding-bottom: 10px;">
                                                                            <tr>
                                                                                <td>
                                                                                    <asp:Label ID="lblBlockUserInfo" Text="Si un usuario pasa un tiempo especificado inactivo, el usuario quedará bloqueado y no podrá acceder a Visualtime hasta que un supervisor lo desbloquee desde la pantalla de Empleado > Autorizaciones > Medios de identificación." runat="server" />
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td style="padding-top: 15px;">
                                                                                    <table>
                                                                                        <tr>
                                                                                            <td>
                                                                                                <dx:ASPxCheckBox ID="chkBlockUser" Text="Bloquear a un usuario cuando lleve " runat="server" ClientInstanceName="chkBlockUserClient">
                                                                                                    <ClientSideEvents CheckedChanged="function(s,e){ hasChanges(true); checkActiveControls(s); }" />
                                                                                                </dx:ASPxCheckBox>
                                                                                            </td>
                                                                                            <td style="padding-left: 5px; padding-right: 5px;">
                                                                                                <dx:ASPxSpinEdit ID="txtBlockUserPeriod" runat="server" Width="70" MinValue="3" MaxValue="24" NumberType="Integer" ClientInstanceName="txtBlockUserPeriodClient">
                                                                                                    <ClientSideEvents ValueChanged="function(s,e){ hasChanges(true,false); validatePeriod();}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                                                </dx:ASPxSpinEdit>
                                                                                            </td>
                                                                                            <td>
                                                                                                <asp:Label ID="lbldaysText" Text="meses sin acceder a Visualtime." runat="server" />
                                                                                            </td>
                                                                                        </tr>
                                                                                    </table>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td style="padding-top: 5px;">
                                                                                    <asp:Label ID="Label1" Text="Esta acción no bloqueará a usuarios consultores." ForeColor="blue" runat="server" />
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </ContentTemplate>
                                                                </ajaxToolkit:TabPanel>
                                                                <ajaxToolkit:TabPanel ID="tpDatabaseOptions_Database" runat="server" Visible="false">
                                                                    <HeaderTemplate>
                                                                        <asp:Label ID="lblDatabaseTitle" Text="Base de datos" runat="server" />
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
                                        <td valign="top">
                                            <div id="panDatabaseOptions2" style="width: 100%; margin: 30px 0;">
                                                <table id="tbDatabaseOptions2" runat="server" cellpadding="0" cellspacing="0" width="100%">
                                                    <tr>
                                                        <td>
                                                            <div class="panHeader2">
                                                                <span style="">
                                                                    <asp:Label ID="Label3" runat="server" Text="Mostrar aviso legal en cada inicio de sesión" Font-Bold="true" /></span>
                                                            </div>
                                                            <br />
                                                        </td>
                                                    </tr>
                                                    <tr style="padding-top: 5px">
                                                        <td style="padding-left: 20px">
                                                            <asp:Label ID="Label4" Text="Con esta opción configuraremos la opción de si queremos mostrar un aviso legal al inicio de cada acceso a Visualtime" runat="server"></asp:Label>
                                                            <br />
                                                            <br />
                                                        </td>
                                                    </tr>
                                                    <tr id="RowInfo3" style="padding-top: 20px; padding-bottom: 20px; padding-left: 60px; vertical-align: top;">
                                                        <td style="padding-left: 20px">

                                                            <ajaxToolkit:TabContainer ID="tcDatabaseOptions2" CssClass="" runat="server">
                                                                <ajaxToolkit:TabPanel ID="tpDatabaseOptions2_ShowLegalText" runat="server" ScrollBars="Auto">
                                                                    <HeaderTemplate>
                                                                        <asp:Label ID="Label5" Text="Aviso a los usuarios del sistema" runat="server" />
                                                                    </HeaderTemplate>
                                                                    <ContentTemplate>
                                                                        <table cellpadding="0" cellspacing="0" style="padding-left: 10px; padding-top: 10px; padding-bottom: 10px;">
                                                                            <tr>
                                                                                <td>
                                                                                    <table>
                                                                                        <tr>
                                                                                            <td>
                                                                                                <dx:ASPxCheckBox ID="chkShowText" Text="Mostrar mensaje legal cuando un usuario inicia sesión en Visualtime." runat="server" ClientInstanceName="chkShowTextClient">
                                                                                                    <ClientSideEvents CheckedChanged="function(s,e){ hasChanges(true); checkActiveControls(s); }" />
                                                                                                </dx:ASPxCheckBox>
                                                                                            </td>
                                                                                        </tr>
                                                                                    </table>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td style="padding-top: 5px;">
                                                                                    <asp:Label ID="Label8" Text="Esta acción aplicará en los accesos a VTLive, VTPortal y VTVisits." ForeColor="blue" runat="server" />
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </ContentTemplate>
                                                                </ajaxToolkit:TabPanel>
                                                                <ajaxToolkit:TabPanel ID="TabPanel2" runat="server" Visible="false">
                                                                    <HeaderTemplate>
                                                                        <asp:Label ID="Label9" Text="Base de datos" runat="server" />
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
                                                        <asp:Button ID="btSave" Text="${Button.ApplyChanges}" runat="server" OnClientClick="saveChanges();" Visible="false" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                                    </td>
                                                    <td>
                                                        <asp:Button ID="btCancel" Text="${Button.UndoChanges}" runat="server" OnClientClick="undoChanges();" Visible="false" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
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

    <!-- POPUP NEW OBJECT -->
    <dx:ASPxPopupControl ID="CaptchaObjectPopup" runat="server" AllowDragging="False" CloseAction="None" Modal="True" ContentUrl="~/Base/Popups/GenericCaptchaValidator.aspx"
        PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter" ModalBackgroundStyle-Opacity="0" Width="500" Height="320"
        ShowHeader="False" ScrollBars="Auto" ShowPageScrollbarWhenModal="True" ClientInstanceName="CaptchaObjectPopup_Client" PopupAnimationType="None" BackColor="Transparent" ContentStyle-Paddings-Padding="0px" Border-BorderColor="Transparent" ShowShadow="false">
        <SettingsLoadingPanel Enabled="false" />
    </dx:ASPxPopupControl>

    <Local:MessageFrame ID="MessageFrame1" runat="server" />
</asp:Content>