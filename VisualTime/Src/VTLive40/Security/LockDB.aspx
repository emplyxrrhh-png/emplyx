<%@ Page Language="VB" MasterPageFile="~/Base/mastEmp.master" AutoEventWireup="false" Inherits="VTLive40.LockDB" Title="Opciones de configuración" EnableEventValidation="True" EnableViewState="True" EnableSessionState="True" CodeBehind="LockDB.aspx.vb" %>

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
                txtAlertFreezeDatePeriodClient.SetEnabled(true);
            } else {
                txtAlertFreezeDatePeriodClient.SetEnabled(false);
            }
        }

        function SelectTab(SelectedTab) {
            // Hacer invisibles los panels
            $get('panDatabaseOptions').style.display = 'none';

            // Desmarcar los botones de la barra
            $get('<%= TABBUTTON_DatabaseOptions.ClientID %>').className = 'bTab';

            var TabID;
            if (SelectedTab == 'panDatabaseOptions') {
                TabID = 'panDatabaseOptions';
                $get('<%= TABBUTTON_DatabaseOptions.ClientID %>').className = 'bTab-active';
            }

            $get(TabID).style.display = 'block';
            $get('<%= ConfigurationOptions_TabVisibleName.ClientID %>').value = TabID;
        }

        function RefreshScreen(DataType) {
            ButtonClick($get('<%= btRefresh.ClientID %>'));
        }

        function checkOPCPanelClients() {
            checkActiveControls(chkAlertFreezeDateClient);
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
                    <asp:Image ID="imgConfigurationOptions" ImageUrl="Images/LockDB.png" runat="server" />
                </div>
                <div class="blackRibbonDescription">
                    <asp:Label ID="lblHeader" runat="server" Text="Fecha de cierre" CssClass="NameText"></asp:Label>
                    <br />
                    <asp:Label ID="lblInfo" runat="server" Text="Desde esta pantalla podrá configurar la fecha de cierre de VisualTime."></asp:Label>
                </div>
                <div class="blackRibbonButtons" style="">
                    <table border="0" cellpadding="0" cellspacing="0" width="100%" style="padding: 2px 5px 5px 5px;">
                        <tr>
                            <td style="width: 100px;" valign="middle"></td>
                            <td valign="top" style="padding-top: 10px; padding-bottom: 20px;"></td>
                            <td id="rowTabButtons1" runat="server" align="right" valign="top" style="width: 140px; padding-top: 0px; padding-right: 1px;">
                                <a id="TABBUTTON_DatabaseOptions" href="javascript: void(0);" class="bTab" onclick="SelectTab('panDatabaseOptions');" runat="server">
                                    <asp:Label ID="lblDatabaseOptionsTabButton" Text="Base de datos" runat="server" /></a>
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
                                                            <a href="javascript: void(0);" class="aMsg" onclick="showCaptcha();"><span id="lblSaveChanges" runat="server" /></a>
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
                                                                    <asp:Label ID="Label2" runat="server" Text="Base de datos" Font-Bold="true" /></span>
                                                            </div>
                                                            <br />
                                                        </td>
                                                    </tr>
                                                    <tr style="padding-top: 5px">
                                                        <td style="padding-left: 20px">
                                                            <asp:Label ID="lblDatabaseOptionsInfo" Text="En este formulario, configuraremos la base de datos. Asignaremos el periodo que utilizará VisualTime para hacer las copias de seguridad, así como la fecha de congelación." runat="server"></asp:Label>
                                                            <br />
                                                            <br />
                                                        </td>
                                                    </tr>
                                                    <tr id="RowInfo2" style="padding-top: 20px; padding-bottom: 20px; padding-left: 60px; vertical-align: top;">
                                                        <td style="padding-left: 20px">

                                                            <ajaxToolkit:TabContainer ID="tcDatabaseOptions" CssClass="" runat="server">
                                                                <ajaxToolkit:TabPanel ID="tpDatabaseOptions_FreezeDate" runat="server" ScrollBars="Auto">
                                                                    <HeaderTemplate>
                                                                        <asp:Label ID="lblFreezeDateTitle" Text="Fecha de congelación" runat="server" />
                                                                    </HeaderTemplate>
                                                                    <ContentTemplate>
                                                                        <table cellpadding="0" cellspacing="0" style="padding-left: 10px; padding-top: 20px">
                                                                            <tr>
                                                                                <td>
                                                                                    <asp:Label ID="lblFreezeDateInfo" Text="La fecha de congelación nos indica hasta qué fecha, ésta incluida, NO se podrán tocar los datos. Ésto le permite dejar bloqueados los datos hasta la fecha indicada, para prevenir posibles errores al modificar datos de fechas pasadas, por ejemplo." ForeColor="blue" runat="server" />
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td style="padding-top: 15px;">
                                                                                    <table>
                                                                                        <tr>
                                                                                            <td>
                                                                                                <asp:Label ID="lblFreezeDate" Text="Fecha hasta la que están congelados los datos:" runat="server" />
                                                                                            </td>
                                                                                            <td style="padding-left: 10px;">
                                                                                                <dx:ASPxHiddenField runat="server" ID="initialFreezeDate" ClientInstanceName="initialFreezeDateClient" />
                                                                                                <dx:ASPxDateEdit runat="server" ID="txtFreezeDate" Width="120px" ClientInstanceName="txtFreezeDateClient">
                                                                                                    <CalendarProperties ShowClearButton="false" />
                                                                                                    <ClientSideEvents DateChanged="function(s,e){ hasChanges(true)}" />
                                                                                                </dx:ASPxDateEdit>
                                                                                            </td>
                                                                                        </tr>
                                                                                    </table>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td style="padding-top: 15px;">
                                                                                    <table>
                                                                                        <tr>
                                                                                            <td>
                                                                                                <dx:ASPxCheckBox ID="chkAlertFreezeDate" Text="Avisar siempre que la fecha de cierre sea anterior a:" runat="server" ClientInstanceName="chkAlertFreezeDateClient">
                                                                                                    <ClientSideEvents CheckedChanged="function(s,e){ hasChanges(true); checkActiveControls(s); }" />
                                                                                                </dx:ASPxCheckBox>
                                                                                            </td>
                                                                                            <td style="padding-left: 10px;">
                                                                                                <dx:ASPxTextBox runat="server" ID="txtAlertFreezeDatePeriod" Width="100px" ClientInstanceName="txtAlertFreezeDatePeriodClient">
                                                                                                    <ClientSideEvents ValueChanged="function(s,e){ hasChanges(true)}" />
                                                                                                </dx:ASPxTextBox>
                                                                                            </td>
                                                                                            <td>
                                                                                                <asp:Label ID="lbldaysText" Text="días" runat="server" />
                                                                                            </td>
                                                                                        </tr>
                                                                                    </table>
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
                                                            <a href="javascript: void(0);" class="aMsg" onclick="showCaptcha();"><span id="lblSaveChangesBottom" runat="server" /></a>
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
        PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter" ModalBackgroundStyle-Opacity="0" Width="500" Height="420"
        ShowHeader="False" ScrollBars="Auto" ShowPageScrollbarWhenModal="True" ClientInstanceName="CaptchaObjectPopup_Client" PopupAnimationType="None" BackColor="Transparent" ContentStyle-Paddings-Padding="0px" Border-BorderColor="Transparent" ShowShadow="false">
        <SettingsLoadingPanel Enabled="false" />
    </dx:ASPxPopupControl>

    <Local:MessageFrame ID="MessageFrame1" runat="server" />
</asp:Content>