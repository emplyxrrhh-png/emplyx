<%@ Page Language="VB" MasterPageFile="~/Base/mastEmp.master" AutoEventWireup="false" Inherits="VTLive40.ManagePunches" Title="" CodeBehind="ManagePunches.aspx.vb" %>

<asp:Content ID="cMainBody" ContentPlaceHolderID="contentMainBody" runat="Server">
    <script type="text/javascript">
        function PageBase_Load() {
            //if (typeof actualTab != 'undefined') {
            cargaTabSuperior(actualTab);
            //} else
            //cargaTabSuperior(0);
        }
    </script>

    <div id="divMainBody">
        <!-- TAB SUPERIOR -->
        <div id="divTabInfo" class="divDataCells">
            <div style="min-height: 10px"></div>
            <div id="divManagePunches" class="blackRibbonTitle">
            </div>
            <div style="min-height: 10px"></div>
        </div>
        <!-- FIN TAB SUPERIOR -->
        <%--DETALLE--%>

        <div id="divContenido" class="divAllContent">
            <div class="panBottomMargin">
                <div class="panHeader2 panBottomMargin">
                    <span class="panelTitleSpan">
                        <asp:Label runat="server" ID="lblErasePunches" Text="Borrado de fichajes"></asp:Label>
                    </span>
                </div>
            </div>
            <div class="panBottomMargin">
                <div class="divRow">
                    <asp:Label ID="lblErasePunchesDescription" CssClass="editTextFormat" Text="Los fichajes a borrar se determinarán en base al fichero VTX seleccionado:" runat="server"></asp:Label>
                </div>
                <div class="divRow">
                    <input type="file" id="txtFileToErase" style="width: 275px" accept=".vtx" />
                </div>
            </div>
            <br />
            <div class="panBottomMargin">
                <dx:ASPxCallbackPanel ID="ASPxCallbackPanelContenido" runat="server" Width="100%" Height="100%" ClientInstanceName="ASPxCallbackPanelContenidoClient">
                    <SettingsLoadingPanel Enabled="false" />
                    <ClientSideEvents EndCallback="ASPxCallbackPanelContenidoClient_EndCallBack" />
                    <PanelCollection>
                        <dx:PanelContent>
                            <div>
                                <div class="jsGridContent">
                                    <dx:ASPxGridView ID="GridPunchesToImport" runat="server" AutoGenerateColumns="False" ClientInstanceName="GridPunchesToImportClient" KeyboardSupport="True" Width="100%">
                                        <Settings ShowTitlePanel="False" VerticalScrollBarMode="Auto" UseFixedTableLayout="True" VerticalScrollableHeight="150" />
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
                                        <Columns>
                                            <dx:GridViewCommandColumn ShowSelectCheckbox="True" ShowClearFilterButton="true" VisibleIndex="0" SelectAllCheckboxMode="Page" />
                                        </Columns>
                                        <SettingsPager Mode="ShowAllRecords" ShowEmptyDataRows="false">
                                        </SettingsPager>
                                    </dx:ASPxGridView>
                                </div>
                                <br />
                                <div style="float: right; margin-right: 42px;">
                                    <dx:ASPxLabel ID="lblCountToImport" ClientInstanceName="lblCountToImport_Client" runat="server" Text="Se borrarán 0 fichajes e importarán 0 fichajes"></dx:ASPxLabel>
                                    <dx:ASPxButton ID="btnImport" ClientInstanceName="btnImport_Client" runat="server" Text="Ejecutar" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat">
                                        <ClientSideEvents Click="showCaptchaImport" />
                                    </dx:ASPxButton>
                                </div>
                            </div>
                        </dx:PanelContent>
                    </PanelCollection>
                </dx:ASPxCallbackPanel>
            </div>

            <!-- POPUP NEW OBJECT -->
            <dx:ASPxPopupControl ID="CaptchaObjectPopup" runat="server" AllowDragging="False" CloseAction="None" Modal="True" ContentUrl="~/Base/Popups/GenericCaptchaValidator.aspx"
                PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter" ModalBackgroundStyle-Opacity="0" Width="500" Height="320"
                ShowHeader="False" ScrollBars="Auto" ShowPageScrollbarWhenModal="True" ClientInstanceName="CaptchaObjectPopup_Client" PopupAnimationType="None" BackColor="Transparent" ContentStyle-Paddings-Padding="0px" Border-BorderColor="Transparent" ShowShadow="false">
                <SettingsLoadingPanel Enabled="false" />
            </dx:ASPxPopupControl>

            <dx:ASPxLoadingPanel ID="LoadingPanelSDK" runat="server" ClientInstanceName="LoadingPanelSDKClient" ImageSpacing="10" Modal="True" CssClass="LoadingDiv" Font-Size="1em">
                <Image Url="../Base/Images/Loaders/loader_v5.gif" Width="48" />
                <LoadingDivStyle Opacity="30" BackColor="Gray" />
            </dx:ASPxLoadingPanel>
        </div>
    </div>
</asp:Content>