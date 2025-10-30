<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="frmBusinessCenterSelector.ascx.vb" Inherits="VTLive40.frmBusinessCenterSelector" %>

<div id="<%= Me.ClientID %>_frm" style="position: fixed; z-index: 19010; display: none; top: 50%; left: 50%; width: 740px;">
    <div id="<%= Me.ClientID %>_BgS" style="position: absolute; top: 0; left: 0; display: none; z-index: 9009;"></div>
    <script type="text/javascript">
</script>
    <div class="bodyPopupExtended businessCenterSelector">
        <dx:ASPxCallbackPanel ID="ASPxBusinessCentersSelectorCallbackPanelContenido" runat="server" Width="100%" Height="580px" ClientInstanceName="ASPxBusinessCentersSelectorCallbackPanelContenidoClient">
            <SettingsLoadingPanel Enabled="false" />
            <ClientSideEvents EndCallback="ASPxBusinessCentersSelectorCallbackPanelContenidoClient_EndCallBack" />
            <PanelCollection>
                <dx:PanelContent ID="PanelContent2" runat="server">
                    <div id="divContentFrmPanels" style="padding-right: 20px">
                        <div id="div3" class="contentPanel" runat="server" name="menuPanel">
                            <!-- Este div es el header -->
                            <div class="panBottomMargin">
                                <div class="panHeader2 panBottomMargin">
                                    <span class="panelTitleSpan">
                                        <asp:Label runat="server" ID="lblBCSelectorDesc" Text="Selector de Centros de Coste con filtros avanzados"></asp:Label>
                                    </span>
                                </div>
                                <input type="hidden" id="hdnMustRefresh_PageBase" value="0" runat="server" />
                                <!-- Filtros-->
                                <div style="width: 100%; padding-top: 10px">
                                    <div class="divRow" style="margin-left: 0px;">
                                        <div class="businessCenterSelectorFilter">
                                            <asp:Label ID="lblCenterName" runat="server" Text="Nombre:" CssClass="labelForm" Style="width: 55px"></asp:Label>
                                            <div class="componentForm">
                                                <dx:ASPxTextBox ID="txtCenterName" runat="server" ClientInstanceName="txtCenterNameClient" Width="313px">
                                                    <ValidationSettings Display="None" />
                                                </dx:ASPxTextBox>
                                            </div>
                                        </div>
                                    </div>
                                    <br />
                                    <div class="divRow" style="margin-left: 0px;">
                                        <div class="businessCenterSelectorFilter">
                                            <asp:Label ID="lblField" runat="server" Text="Campo:" CssClass="labelForm" Style="width: 55px"></asp:Label>
                                            <div class="componentForm">
                                                <dx:ASPxComboBox runat="server" ID="cmbBCFieldsValues1" Width="125px" ClientInstanceName="cmbBCFieldsValues1Client">
                                                    <ValidationSettings Display="None" />
                                                </dx:ASPxComboBox>
                                            </div>
                                        </div>
                                        <div class="businessCenterSelectorFilter">
                                            <asp:Label ID="lblCriteria" runat="server" Text="Criterio:" CssClass="labelForm" Style="width: auto"></asp:Label>
                                            <dx:ASPxComboBox runat="server" ID="cmbBCCriteria1" Width="125px" ClientInstanceName="cmbBCCriteria1Client">
                                                <ValidationSettings Display="None" />
                                            </dx:ASPxComboBox>
                                        </div>
                                        <div class="businessCenterSelectorFilter">
                                            <asp:Label ID="lblValue" runat="server" Text="Valor:" CssClass="labelForm" Style="width: auto" />
                                            <dx:ASPxTextBox runat="server" ID="txtValue1" Width="125px" ClientInstanceName="txtValue1Client">
                                                <ValidationSettings Display="None" />
                                            </dx:ASPxTextBox>
                                        </div>
                                        <div class="businessCenterSelectorFilterSearch">
                                            <dx:ASPxButton ID="btnSearch" runat="server" AutoPostBack="False" CausesValidation="False"
                                                ClientInstanceName="btnSearchClient" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack btnNoPadding"
                                                Text="Filtrar" Width="100">
                                                <Image Url="~/Base/Images/txtFilter/icofilter_19.png"></Image>
                                                <HoverStyle CssClass="btnFlat-hover btnFlatBlack-hover"></HoverStyle>
                                                <ClientSideEvents Click="function(s,e) { FilterCentersTree() }" />
                                            </dx:ASPxButton>
                                        </div>
                                    </div>
                                    <br />
                                </div>
                                <!-- Arbol y grid de centros de coste -->
                                <div style="width: 100%; height: 400px;" id="divSelectorBc">
                                    <div style="height: 100%; float: left; width: 46%;" id="divActivityTree">
                                        <div style="width: 100%; height: 100%; overflow-y: auto; overflow-x: auto;" id="loadPanelTree">
                                            <dx:ASPxTreeList ID="tltBusinessCenters" runat="server" AutoGenerateColumns="False"
                                                Width="300px" ClientInstanceName="tltBusinessCentersClient">
                                                <Styles>
                                                    <Header Border-BorderStyle="None" Border-BorderWidth="0" BackColor="Orange"></Header>
                                                    <Cell CssClass="activityCell" />
                                                    <SelectedNode CssClass="activityCellSelected" />
                                                </Styles>
                                                <Columns>
                                                    <dx:TreeListDataColumn FieldName="Name" Caption="Nombre" VisibleIndex="0" />
                                                </Columns>
                                                <SettingsBehavior ExpandCollapseAction="NodeDblClick" />
                                                <SettingsSelection Enabled="True" />
                                                <Settings ShowColumnHeaders="false" />
                                            </dx:ASPxTreeList>
                                        </div>
                                        <div>
                                            <dx:ASPxCheckBox ID="chkCenters" runat="server" ClientInstanceName="chkCentersClient" AutoPostBack="false" Text="Seleccionar todos los centros de coste">
                                                <ClientSideEvents CheckedChanged="function(s,e){selectAllCenters();}" />
                                            </dx:ASPxCheckBox>
                                        </div>
                                    </div>
                                    <div style="width: calc(10% - 2px); float: left;">
                                        <div style="text-align: center; padding: 20px 0 20px 0;">
                                            <dx:ASPxButton ID="btnAdd" runat="server" AutoPostBack="False" CausesValidation="False"
                                                ClientInstanceName="btnAddClient" ToolTip="" Width="16" CssClass="btnNoPadding">
                                                <Image Url="~/Tasks/Images/tree_select.png"></Image>
                                                <ClientSideEvents Click="function(s,e) { SendSelectedCenters(); }" />
                                            </dx:ASPxButton>
                                        </div>
                                        <div style="text-align: center; padding: 20px 0 20px 0;">
                                            <dx:ASPxButton ID="btnDelete" runat="server" AutoPostBack="False" CausesValidation="False"
                                                ClientInstanceName="btnAddClient" ToolTip="" Width="16" CssClass="btnNoPadding">
                                                <Image Url="~/Tasks/Images/trash.png"></Image>
                                                <ClientSideEvents Click="function(s,e) { DeleteCentersGrid(); }" />
                                            </dx:ASPxButton>
                                        </div>
                                    </div>
                                    <div style="float: left;" id="gridDiv">
                                        <div class="jsGridContent" id="loadPanelGrid">
                                            <dx:ASPxGridView ID="GridCenters" runat="server" AutoGenerateColumns="False" ClientInstanceName="GridCentersClient"
                                                KeyboardSupport="True" Width="300px" OnRowDeleting="GridCenters_RowDeleting">
                                                <Settings ShowTitlePanel="False" VerticalScrollBarMode="Auto" VerticalScrollableHeight="400" />
                                                <SettingsCommandButton>
                                                    <DeleteButton Image-Url="~/Base/Images/Grid/remove.png" Image-ToolTip="">
                                                        <Image Url="~/Base/Images/Grid/remove.png"></Image>
                                                    </DeleteButton>
                                                </SettingsCommandButton>
                                                <Styles>
                                                    <Header CssClass="jsBussinessCenterGridHeaderCell" ForeColor="White" />
                                                    <Cell Wrap="True" />
                                                </Styles>
                                                <ClientSideEvents EndCallback="GridCenters_EndCallback" />
                                                <SettingsPager>
                                                    <PageSizeItemSettings ShowAllItem="true"></PageSizeItemSettings>
                                                </SettingsPager>
                                            </dx:ASPxGridView>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <br />
                        <div style="width: 100%;">
                            <table border="0" style="width: 100%;">
                                <tr>
                                    <td align="right" style="padding-top: 3px; padding-right: 5px;">
                                        <table>
                                            <tr>
                                                <td>
                                                    <dx:ASPxButton ID="btnOk" runat="server" AutoPostBack="False" CausesValidation="False" Text="Aceptar" ToolTip="Aceptar" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                                        <ClientSideEvents Click="function(s,e){ SaveBusinessCenters(); }" />
                                                        <HoverStyle CssClass="btnFlat-hover btnFlatBlack-hover"></HoverStyle>
                                                    </dx:ASPxButton>
                                                </td>
                                                <td style="width: 110px;" align="left">
                                                    <dx:ASPxButton ID="btnCancel" runat="server" AutoPostBack="False" CausesValidation="False" Text="Cancelar" ToolTip="Cancelar" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                                        <ClientSideEvents Click="function(s,e){ CancelBusinessCenters(); }" />
                                                        <HoverStyle CssClass="btnFlat-hover btnFlatBlack-hover"></HoverStyle>
                                                    </dx:ASPxButton>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>
                </dx:PanelContent>
            </PanelCollection>
        </dx:ASPxCallbackPanel>
        <dx:ASPxLoadingPanel ID="lpTree" runat="server" ClientInstanceName="lpTreeClient" Modal="True">
            <LoadingDivStyle Opacity="0"></LoadingDivStyle>
        </dx:ASPxLoadingPanel>
        <dx:ASPxLoadingPanel ID="lgGrid" runat="server" ClientInstanceName="lgGridClient" Modal="True">
            <LoadingDivStyle Opacity="0"></LoadingDivStyle>
        </dx:ASPxLoadingPanel>
        <dx:ASPxLoadingPanel ID="lpSelector" runat="server" ClientInstanceName="lpSelectorClient" Modal="True">
            <LoadingDivStyle Opacity="0"></LoadingDivStyle>
        </dx:ASPxLoadingPanel>
    </div>
</div>