<%@ Page Language="VB" MasterPageFile="~/Base/mastEmp.master" AutoEventWireup="false" Inherits="VTLive40.License" Title="Información de licencia" EnableEventValidation="True" EnableViewState="True" EnableSessionState="True" CodeBehind="License.aspx.vb" %>

<asp:Content ID="cMainBody" ContentPlaceHolderID="contentMainBody" runat="Server">

    <script language="javascript" type="text/javascript">
        function PageBase_Load() {
            ConvertControls();
            $get('panLicense').style.display = 'block';
            $get('panConcurrency').style.display = 'block';
            // Reestablezco el tab activo
            if ($get('<%= ConfigurationOptions_TabVisibleName.ClientID %>').value != '') {
                SelectTab($get('<%= ConfigurationOptions_TabVisibleName.ClientID %>').value);
            }
            else {
                SelectTab('panLicense');
            }

            checkOPCPanelClients();
        }

        function SelectTab(SelectedTab) {
            // Hacer invisibles los panels
            $get('panLicense').style.display = 'none';
            $get('panConcurrency').style.display = 'none';

            // Desmarcar los botones de la barra
            $get('<%= TABBUTTON_LicenceInfo.ClientID%>').className = 'bTab';
            $get('<%= TABBUTTON_ConcurrencyInfo.ClientID%>').className = 'bTab';

            var TabID;
            if (SelectedTab == 'panLicense') {
                TabID = 'panLicense';
                $get('<%= TABBUTTON_LicenceInfo.ClientID%>').className = 'bTab-active';
            }

            if (SelectedTab == 'panConcurrency') {
                TabID = 'panConcurrency';
                $get('<%= TABBUTTON_ConcurrencyInfo.ClientID%>').className = 'bTab-active';
                CallbackSessionClient.PerformCallback("LOADGRAPHDATA");
            }

            $get(TabID).style.display = 'block';
            $get('<%= ConfigurationOptions_TabVisibleName.ClientID %>').value = TabID;
        }

        function checkOPCPanelClients() {
        }

        //Llença aquest script al recarregar els updatepanels per poder controlar per js els opclient
        function endRequestHandler() {

        }
    </script>

    <dx:ASPxCallback ID="CallbackSession" runat="server" ClientInstanceName="CallbackSessionClient" ClientSideEvents-CallbackComplete="CallbackSession_CallbackComplete"></dx:ASPxCallback>

    <div id="divMainBody">
        <!-- TAB SUPERIOR -->
        <div id="divTabInfo" class="divDataCells">
            <div style="min-height: 10px"></div>
            <div id="divStartRibbon" class="blackRibbonTitle" style="">
                <div class="blackRibbonIcon">
                    <asp:Image ID="imgConfigurationOptions" ImageUrl="Images/LicenseInfo90.png" runat="server" />
                </div>
                <div class="blackRibbonDescription">
                    <asp:Label ID="lblHeader" runat="server" Text="Licencia" CssClass="NameText"></asp:Label>
                    <br />
                    <asp:Label ID="lblInfo" runat="server" Text="Desde esta pantalla podrá consultar la información de la licencia contratada y los módulos activos."></asp:Label>
                </div>
                <div class="blackRibbonButtons" style="">
                    <table border="0" cellpadding="0" cellspacing="0" width="100%" style="padding: 2px 5px 5px 5px;">
                        <tr>
                            <td style="width: 100px;" valign="middle"></td>
                            <td valign="top" style="padding-top: 10px; padding-bottom: 20px;"></td>
                            <td id="rowTabButtons1" runat="server" align="right" valign="top" style="width: 140px; padding-top: 0px; padding-right: 1px;">
                                <a id="TABBUTTON_LicenceInfo" href="javascript: void(0);" class="bTab" onclick="SelectTab('panLicense');" runat="server">
                                    <asp:Label ID="lblDatabaseOptionsTabButton" Text="Licencia" runat="server" /></a>
                                <a id="TABBUTTON_ConcurrencyInfo" href="javascript: void(0);" class="bTab" onclick="SelectTab('panConcurrency');" runat="server">
                                    <asp:Label ID="lblDatabaseConcurrencyTabButton" Text="Concurrencia" runat="server" /></a>
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
                                            </div>
                                        </td>
                                    </tr>

                                    <tr>
                                        <td valign="top">
                                            <div id="panLicense" style="width: 100%; display: none;">
                                                <div id="tbLicenceInfo" runat="server" width="100%">
                                                    <div class="panBottomMargin">
                                                        <div class="panHeader2 panBottomMargin">
                                                            <span class="panelTitleSpan">
                                                                <asp:Label runat="server" ID="lblSolutionTitle" Text="Soluciones"></asp:Label>
                                                            </span>
                                                        </div>
                                                    </div>

                                                    <!-- Este div es un formulario -->
                                                    <div class="panBottomMargin">
                                                        <div class="divRow">
                                                            <div class="jsGrid">
                                                                <asp:Label ID="lblSolutionsCaption" runat="server" CssClass="jsGridTitle" Text="Soluciones"></asp:Label>
                                                            </div>
                                                            <div class="jsGridContent">
                                                                <dx:ASPxGridView ID="GridSolutions" runat="server" AutoGenerateColumns="False" ClientInstanceName="GridSolutions" KeyboardSupport="True" Width="100%">
                                                                    <Settings ShowTitlePanel="False" VerticalScrollBarMode="Auto" UseFixedTableLayout="True" VerticalScrollableHeight="200" />
                                                                    <SettingsCommandButton>
                                                                        <DeleteButton Image-Url="~/Base/Images/Grid/remove.png" Image-ToolTip="" />
                                                                        <UpdateButton Image-Url="~/Base/Images/Grid/save.png" Image-ToolTip="" />
                                                                        <CancelButton Image-Url="~/Base/Images/Grid/cancel.png" Image-ToolTip="" />
                                                                        <EditButton Image-Url="~/Base/Images/Grid/edit.png" Image-ToolTip="" />
                                                                    </SettingsCommandButton>
                                                                    <Styles>
                                                                        <CommandColumn Spacing="5px" />
                                                                        <Header CssClass="jsGridHeaderCell" />
                                                                        <Cell Wrap="False" />
                                                                    </Styles>
                                                                    <SettingsPager Mode="ShowAllRecords" ShowEmptyDataRows="false">
                                                                    </SettingsPager>
                                                                </dx:ASPxGridView>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="panBottomMargin">
                                                        <div class="panHeader2 panBottomMargin">
                                                            <span class="panelTitleSpan">
                                                                <asp:Label runat="server" ID="lblModules" Text="Módulos"></asp:Label>
                                                            </span>
                                                        </div>
                                                    </div>

                                                    <div class="panBottomMargin">
                                                        <div class="divRow">
                                                            <div class="jsGrid">
                                                                <asp:Label ID="lblModulesCaption" runat="server" CssClass="jsGridTitle" Text="Módulos"></asp:Label>
                                                            </div>
                                                            <div class="jsGridContent">
                                                                <dx:ASPxGridView ID="GridModules" runat="server" AutoGenerateColumns="False" ClientInstanceName="GridModules" KeyboardSupport="True" Width="100%">
                                                                    <Settings ShowTitlePanel="False" VerticalScrollBarMode="Auto" UseFixedTableLayout="True" VerticalScrollableHeight="350" />
                                                                    <SettingsCommandButton>
                                                                        <DeleteButton Image-Url="~/Base/Images/Grid/remove.png" Image-ToolTip="" />
                                                                        <UpdateButton Image-Url="~/Base/Images/Grid/save.png" Image-ToolTip="" />
                                                                        <CancelButton Image-Url="~/Base/Images/Grid/cancel.png" Image-ToolTip="" />
                                                                        <EditButton Image-Url="~/Base/Images/Grid/edit.png" Image-ToolTip="" />
                                                                    </SettingsCommandButton>
                                                                    <Styles>
                                                                        <CommandColumn Spacing="5px" />
                                                                        <Header CssClass="jsGridHeaderCell" />
                                                                        <Cell Wrap="False" />
                                                                    </Styles>
                                                                    <SettingsPager Mode="ShowAllRecords" ShowEmptyDataRows="false">
                                                                    </SettingsPager>
                                                                </dx:ASPxGridView>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div id="panConcurrency" style="width: 100%; display: none;">
                                                <div id="tbConcurrencyInfo" runat="server" width="100%">
                                                    <div class="panBottomMargin">
                                                        <div class="panHeader2 panBottomMargin">
                                                            <span class="panelTitleSpan">
                                                                <asp:Label runat="server" ID="lblConcurrencyTitle" Text="Licencias Usadas"></asp:Label>
                                                            </span>
                                                        </div>
                                                    </div>
                                                    <div id="dxChart" style="display: inline-block;"></div>
                                                </div>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <!-- Mensajes -->
                                            <div id="divMsgBottom" class="divMsg" style="display: none;">
                                            </div>
                                        </td>
                                    </tr>
                                </table>

                                <asp:Button ID="btRefresh" runat="server" Style="display: none;" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>
    <asp:HiddenField ID="ConfigurationOptions_TabVisibleName" Value="" runat="server" />
    <asp:HiddenField ID="maxConcurrentUsers" Value="" runat="server" ClientIDMode="Static" />

    <Local:MessageFrame ID="MessageFrame1" runat="server" />
</asp:Content>