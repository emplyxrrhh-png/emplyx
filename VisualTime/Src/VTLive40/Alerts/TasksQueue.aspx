<%@ Page Language="VB" MasterPageFile="~/Base/mastEmp.master" AutoEventWireup="false" Inherits="VTLive40.TasksQueue" Title="Información de licencia" EnableEventValidation="True" EnableViewState="True" EnableSessionState="True" CodeBehind="TasksQueue.aspx.vb" %>

<asp:Content ID="cMainBody" ContentPlaceHolderID="contentMainBody" runat="Server">

    <script language="javascript" type="text/javascript">
        function PageBase_Load() {
            ConvertControls();
            $get('panTasksQueue').style.display = 'block';

            // Reestablezco el tab activo
            if ($get('<%= ConfigurationOptions_TabVisibleName.ClientID %>').value != '') {
                SelectTab($get('<%= ConfigurationOptions_TabVisibleName.ClientID %>').value);
            }
            else {
                SelectTab('panTasksQueue');
            }
            checkOPCPanelClients();
            setTimeout(function () { initializeScreen(); }, 60000);

            window.parent.setUPReportsAndWizards({ HasReports: false, HasAssistants: false });
        }

        function initializeScreen() {
            setInterval(tasksqueue.loadData, 60000);
        }

        function SelectTab(SelectedTab) {
            // Hacer invisibles los panels
            $get('panTasksQueue').style.display = 'none';

            // Desmarcar los botones de la barra
            $get('<%= TABBUTTON_TasksQueueInfo.ClientID%>').className = 'bTab';

            var TabID;
            if (SelectedTab == 'panTasksQueue') {
                TabID = 'panTasksQueue';
                $get('<%= TABBUTTON_TasksQueueInfo.ClientID%>').className = 'bTab-active';
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

    <div id="divMainBody">
        <input type="hidden" value="/#/./Scheduler/AnalyticsScheduler" runat="server" id="ScheduleAnalyticURI" />
        <input type="hidden" value="/#/./Tasks/Analytics" runat="server" id="TaskAnalyticURI" />
        <input type="hidden" value="/#/./Scheduler/AnalyticsCostControl" runat="server" id="CostCenterAnalyticURI" />
        <input type="hidden" value="/#/./Access/Analytics" runat="server" id="AccessURI" />
        <input type="hidden" value="/#/./Genius" runat="server" id="GeniusURI" />
        <input type="hidden" value="" runat="server" id="IsSaas" />
        <!-- TAB SUPERIOR -->
        <div id="divTabInfo" class="divDataCells">
            <div style="min-height: 10px"></div>
            <div id="divStartRibbon" class="blackRibbonTitle" style="">
                <div class="blackRibbonIcon">
                    <asp:Image ID="imgConfigurationOptions" ImageUrl="Images/TasksQueue90.png" runat="server" />
                </div>
                <div class="blackRibbonDescription">
                    <asp:Label ID="lblHeader" runat="server" Text="Informes solicitados" CssClass="NameText"></asp:Label>
                    <br />
                    <asp:Label ID="lblInfo" runat="server" Text="Desde esta pantalla podrá consultar la lista de informes en curso, pendientes y finalizados así como el orden en el que se ejecutarán en el sistema."></asp:Label>
                </div>
                <div class="blackRibbonButtons" style="">
                    <table border="0" cellpadding="0" cellspacing="0" width="100%" style="padding: 2px 5px 5px 5px;">
                        <tr>
                            <td style="width: 100px;" valign="middle"></td>
                            <td valign="top" style="padding-top: 10px; padding-bottom: 20px;"></td>
                            <td id="rowTabButtons1" runat="server" align="right" valign="top" style="width: 140px; padding-top: 0px; padding-right: 1px;">
                                <a id="TABBUTTON_TasksQueueInfo" href="javascript: void(0);" class="bTab" onclick="SelectTab('panTasksQueue');" runat="server">
                                    <asp:Label ID="lblDatabaseOptionsTabButton" Text="Informes solicitados" runat="server" /></a>
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
                                            <div id="panTasksQueue" style="width: 100%; display: none;">
                                                <div id="tbLicenceInfo" runat="server" width="100%">
                                                    <div class="panBottomMargin">
                                                        <div class="panHeader2 panBottomMargin">
                                                            <span class="panelTitleSpan">
                                                                <asp:Label runat="server" ID="lblCompleted" Text="Informes completados"></asp:Label>
                                                            </span>
                                                        </div>
                                                    </div>

                                                    <!-- Este div es un formulario -->
                                                    <div class="panBottomMargin">
                                                        <div class="divRow">
                                                            <div class="jsGrid">
                                                                <asp:Label ID="Label3" runat="server" CssClass="jsGridTitle" Text="Informes completados"></asp:Label>
                                                            </div>
                                                            <div class="jsGridContent">
                                                                <dx:ASPxGridView ID="gridCompleted" runat="server" AutoGenerateColumns="False" ClientInstanceName="gridCompleted" KeyboardSupport="True" Width="100%">
                                                                    <Settings ShowTitlePanel="False" VerticalScrollBarMode="Auto" UseFixedTableLayout="True" VerticalScrollableHeight="125" />
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
                                                                    <SettingsPager Mode="ShowPager" PageSize="10" ShowEmptyDataRows="false">
                                                                    </SettingsPager>
                                                                    <ClientSideEvents CustomButtonClick="grid_CustomButtonClick" />
                                                                </dx:ASPxGridView>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="panBottomMargin">
                                                        <div class="panHeader2 panBottomMargin">
                                                            <span class="panelTitleSpan">
                                                                <asp:Label runat="server" ID="lblRunningTitle" Text="Informes en curso"></asp:Label>
                                                            </span>
                                                        </div>
                                                    </div>

                                                    <!-- Este div es un formulario -->
                                                    <div class="panBottomMargin">
                                                        <div class="divRow">
                                                            <div class="jsGrid">
                                                                <asp:Label ID="lblSolutionsCaption" runat="server" CssClass="jsGridTitle" Text="Informes en curso"></asp:Label>
                                                            </div>
                                                            <div class="jsGridContent">
                                                                <dx:ASPxGridView ID="gridRunning" runat="server" AutoGenerateColumns="False" ClientInstanceName="GridRunning" KeyboardSupport="True" Width="100%">
                                                                    <Settings ShowTitlePanel="False" VerticalScrollBarMode="Auto" UseFixedTableLayout="True" VerticalScrollableHeight="125" />
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
                                                                    <SettingsPager Mode="ShowPager" PageSize="10" ShowEmptyDataRows="false">
                                                                    </SettingsPager>
                                                                </dx:ASPxGridView>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="panBottomMargin">
                                                        <div class="panHeader2 panBottomMargin">
                                                            <span class="panelTitleSpan">
                                                                <asp:Label runat="server" ID="lblWaitingTitle" Text="Informes en espera"></asp:Label>
                                                            </span>
                                                        </div>
                                                    </div>

                                                    <!-- Este div es un formulario -->
                                                    <div class="panBottomMargin">
                                                        <div class="divRow">
                                                            <div class="jsGrid">
                                                                <asp:Label ID="Label2" runat="server" CssClass="jsGridTitle" Text="Informes en espera"></asp:Label>
                                                            </div>
                                                            <div class="jsGridContent">
                                                                <dx:ASPxGridView ID="gridWaiting" runat="server" AutoGenerateColumns="False" ClientInstanceName="gridWaiting" KeyboardSupport="True" Width="100%">
                                                                    <Settings ShowTitlePanel="False" VerticalScrollBarMode="Auto" UseFixedTableLayout="True" VerticalScrollableHeight="125" />
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
                                                                    <SettingsPager Mode="ShowPager" PageSize="10" ShowEmptyDataRows="false">
                                                                    </SettingsPager>
                                                                </dx:ASPxGridView>
                                                            </div>
                                                        </div>
                                                    </div>
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

    <Local:MessageFrame ID="MessageFrame1" runat="server" />
</asp:Content>