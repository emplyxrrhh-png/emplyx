<%@ Control Language="vb" AutoEventWireup="false" Inherits="VTLive40.WebUserForms_frmFilterBusinessCenters" CodeBehind="frmFilterBusinessCenters.ascx.vb" %>

<div id="<%= Me.ClientID %>_frm" style="position: fixed; z-index: 9010; display: none; top: 50%; left: 50%; width: 650px;">
    <div id="<%= Me.ClientID %>_BgS" style="position: absolute; top: 0; left: 0; display: none; z-index: 9009;"></div>
    <div class="bodyPopupExtended">
        <input type="hidden" id="hdnEditEnabled" runat="server" value="1" />
        <dx:ASPxCallbackPanel ID="ASPxBusinessCentersCallbackPanelContenido" runat="server" Width="100%" Height="100%" ClientInstanceName="ASPxBusinessCentersCallbackPanelContenidoClient">
            <SettingsLoadingPanel Enabled="false" />
            <ClientSideEvents EndCallback="ASPxBusinessCentersCallbackPanelContenidoClient_EndCallBack" />
            <PanelCollection>
                <dx:PanelContent ID="PanelContent2" runat="server">
                    <div id="divContentFrmPanels" style="padding-right: 20px">
                        <div id="div3" class="contentPanel" runat="server" name="menuPanel">

                            <!-- Este div es el header -->
                            <div class="panBottomMargin">
                                <div class="panHeader2 panBottomMargin">
                                    <span class="panelTitleSpan">
                                        <asp:Label runat="server" ID="Label1" Text="General"></asp:Label>
                                    </span>
                                </div>
                                <!-- La descripción es opcional -->
                                <div class="panelHeaderContent">
                                    <div class="panelDescriptionImage">
                                        <img alt="" src="<%=Me.Page.ResolveUrl("~/Tasks/Images/ExportToExcel.png")%>" />
                                    </div>
                                    <div class="panelDescriptionText">
                                        <asp:Label ID="Label2" runat="server" Text="Aplicar filtro para busqueda de Centros de Coste"></asp:Label>
                                    </div>
                                </div>
                            </div>
                            <roUserControls:roGroupBox ID="GroupBox2" runat="server">
                                <Content>
                                    <div class="panBottomMargin">
                                        <!-- Row Estado -->
                                        <div class="divRow">
                                            <div class="splitDivLeft">
                                                <div class="panBottomMargin">
                                                    <div class="divRow">
                                                        <div class="">
                                                            <asp:Label ID="lblEstado" runat="server" Text="Estado"></asp:Label>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="splitDivRight">
                                                <div class="panBottomMargin">
                                                    <div class="divRow">
                                                        <div class="">
                                                            <dx:ASPxComboBox runat="server" ID="cmbState" Width="200px" ClientInstanceName="cmbStateClient">
                                                                <ClientSideEvents SelectedIndexChanged="function(s,e){}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                            </dx:ASPxComboBox>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <!-- Row Nombre CC -->
                                        <div class="divRow">
                                            <div class="splitDivLeft">
                                                <div class="panBottomMargin">
                                                    <div class="divRow">
                                                        <div class="">
                                                            <asp:Label ID="lblBusinessCenterName" runat="server" Text="Nombre:"></asp:Label>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="splitDivRight">
                                                <div class="panBottomMargin">
                                                    <div class="divRow">
                                                        <div class="">
                                                            <dx:ASPxTextBox runat="server" ID="txtBusinessCenterName" Width="200px" ClientInstanceName="txtBusinessCenterNameClient"></dx:ASPxTextBox>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <!-- Headers Campos -->
                                        <div id="divBCFields1" runat="server">
                                            <div class="panBottomMargin">
                                                <div style="float: left; width: calc(33%)">
                                                    <div class="divRow">
                                                        <asp:Label runat="server" ID="lblHeaderField" Text="Campo" />
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="panBottomMargin">
                                                <div style="float: left; width: calc(33%)">
                                                    <div class="divRow">
                                                        <asp:Label runat="server" ID="lblHeaderCriteria" Text="Criterio" />
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="panBottomMargin">
                                                <div style="float: left; width: calc(33%)">
                                                    <div class="divRow">
                                                        <asp:Label runat="server" ID="lblHeaderValue" Text="Valor" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <br />
                                        <roUserControls:roGroupBox ID="RoGroupBox1" runat="server">
                                            <Content>
                                                <!-- Row Campos de la Ficha 1 -->
                                                <div>
                                                    <div class="panBottomMargin">
                                                        <div style="float: left; width: calc(33%)">
                                                            <div class="">
                                                                <dx:ASPxComboBox runat="server" ID="cmbBCFieldsValues1" Width="125px" ClientInstanceName="cmbBCFieldsValues1Client">
                                                                    <ClientSideEvents SelectedIndexChanged="function(s,e){}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                </dx:ASPxComboBox>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="panBottomMargin">
                                                        <div style="float: left; width: calc(33%)">
                                                            <div class="">
                                                                <dx:ASPxComboBox runat="server" ID="cmbBCCriteria1" Width="125px" ClientInstanceName="cmbBCCriteria1Client">
                                                                    <ClientSideEvents SelectedIndexChanged="function(s,e){}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                </dx:ASPxComboBox>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="panBottomMargin">
                                                        <div style="float: left; width: calc(33%)">
                                                            <div class="">
                                                                <dx:ASPxTextBox runat="server" ID="txtValue1" Width="125px" ClientInstanceName="txtValue1Client"></dx:ASPxTextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="divRow">
                                                    <dx:ASPxRadioButtonList ID="rblAndOr1"
                                                        runat="server"
                                                        ClientInstanceName="rblAndOr1Client"
                                                        RepeatColumns="2"
                                                        RepeatLayout="Table"
                                                        RepeatDirection="Horizontal"
                                                        Paddings="0px">
                                                        <CaptionSettings Position="Top" />
                                                    </dx:ASPxRadioButtonList>
                                                </div>
                                                <!-- Row Campos de la Ficha 2 -->
                                                <div>
                                                    <div class="panBottomMargin">
                                                        <div style="float: left; width: calc(33%)">
                                                            <div class="">
                                                                <dx:ASPxComboBox runat="server" ID="cmbBCFieldsValues2" Width="125px" ClientInstanceName="cmbBCFieldsValues2Client">
                                                                    <ClientSideEvents SelectedIndexChanged="function(s,e){}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                </dx:ASPxComboBox>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="panBottomMargin">
                                                        <div style="float: left; width: calc(33%)">
                                                            <div class="">
                                                                <dx:ASPxComboBox runat="server" ID="cmbBCCriteria2" Width="125px" ClientInstanceName="cmbBCCriteria2Client">
                                                                    <ClientSideEvents SelectedIndexChanged="function(s,e){}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                </dx:ASPxComboBox>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="panBottomMargin">
                                                        <div style="float: left; width: calc(33%)">
                                                            <div class="">
                                                                <dx:ASPxTextBox runat="server" ID="txtValue2" Width="125px" ClientInstanceName="txtValue2Client"></dx:ASPxTextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="divRow">
                                                    <dx:ASPxRadioButtonList ID="rblAndOr2"
                                                        runat="server"
                                                        ClientInstanceName="rblAndOr2Client"
                                                        RepeatColumns="2"
                                                        RepeatLayout="Table"
                                                        RepeatDirection="Horizontal">
                                                        <CaptionSettings Position="Top" />
                                                    </dx:ASPxRadioButtonList>
                                                </div>
                                                <!-- Row Campos de la Ficha 3 -->
                                                <div>
                                                    <div class="panBottomMargin">
                                                        <div style="float: left; width: calc(33%)">
                                                            <div class="">
                                                                <dx:ASPxComboBox runat="server" ID="cmbBCFieldsValues3" Width="125px" ClientInstanceName="cmbBCFieldsValues3Client">
                                                                    <ClientSideEvents SelectedIndexChanged="function(s,e){}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                </dx:ASPxComboBox>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="panBottomMargin">
                                                        <div style="float: left; width: calc(33%)">
                                                            <div class="">
                                                                <dx:ASPxComboBox runat="server" ID="cmbBCCriteria3" Width="125px" ClientInstanceName="cmbBCCriteria3Client">
                                                                    <ClientSideEvents SelectedIndexChanged="function(s,e){}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                </dx:ASPxComboBox>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="panBottomMargin">
                                                        <div style="float: left; width: calc(33%)">
                                                            <div class="">
                                                                <dx:ASPxTextBox runat="server" ID="txtValue3" Width="125px" ClientInstanceName="txtValue3Client"></dx:ASPxTextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="divRow">
                                                    <dx:ASPxRadioButtonList ID="rblAndOr3"
                                                        runat="server"
                                                        ClientInstanceName="rblAndOr3Client"
                                                        RepeatColumns="2"
                                                        RepeatLayout="Table"
                                                        RepeatDirection="Horizontal">
                                                        <CaptionSettings Position="Top" />
                                                    </dx:ASPxRadioButtonList>
                                                </div>
                                                <!-- Row Campos de la Ficha 4 -->
                                                <div>
                                                    <div class="panBottomMargin">
                                                        <div style="float: left; width: calc(33%)">
                                                            <div class="">
                                                                <dx:ASPxComboBox runat="server" ID="cmbBCFieldsValues4" Width="125px" ClientInstanceName="cmbBCFieldsValues4Client">
                                                                    <ClientSideEvents SelectedIndexChanged="function(s,e){}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                </dx:ASPxComboBox>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="panBottomMargin">
                                                        <div style="float: left; width: calc(33%)">
                                                            <div class="">
                                                                <dx:ASPxComboBox runat="server" ID="cmbBCCriteria4" Width="125px" ClientInstanceName="cmbBCCriteria4Client">
                                                                    <ClientSideEvents SelectedIndexChanged="function(s,e){}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                </dx:ASPxComboBox>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="panBottomMargin">
                                                        <div style="float: left; width: calc(33%)">
                                                            <div class="">
                                                                <dx:ASPxTextBox runat="server" ID="txtValue4" Width="125px" ClientInstanceName="txtValue4Client"></dx:ASPxTextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="divRow">
                                                    <dx:ASPxRadioButtonList ID="rblAndOr4"
                                                        runat="server"
                                                        ClientInstanceName="rblAndOr4Client"
                                                        RepeatColumns="2"
                                                        RepeatLayout="Table"
                                                        RepeatDirection="Horizontal">
                                                        <CaptionSettings Position="Top" />
                                                    </dx:ASPxRadioButtonList>
                                                </div>
                                                <!-- Row Campos de la Ficha 5 -->
                                                <div>
                                                    <div class="panBottomMargin">
                                                        <div style="float: left; width: calc(33%)">
                                                            <div class="">
                                                                <dx:ASPxComboBox runat="server" ID="cmbBCFieldsValues5" Width="125px" ClientInstanceName="cmbBCFieldsValues5Client">
                                                                    <ClientSideEvents SelectedIndexChanged="function(s,e){}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                </dx:ASPxComboBox>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="panBottomMargin">
                                                        <div style="float: left; width: calc(33%)">
                                                            <div class="">
                                                                <dx:ASPxComboBox runat="server" ID="cmbBCCriteria5" Width="125px" ClientInstanceName="cmbBCCriteria5Client">
                                                                    <ClientSideEvents SelectedIndexChanged="function(s,e){}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                </dx:ASPxComboBox>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="panBottomMargin">
                                                        <div style="float: left; width: calc(33%)">
                                                            <div class="">
                                                                <dx:ASPxTextBox runat="server" ID="txtValue5" Width="125px" ClientInstanceName="txtValue5Client"></dx:ASPxTextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <br />
                                                <br />
                                            </Content>
                                        </roUserControls:roGroupBox>
                                    </div>
                                </Content>
                            </roUserControls:roGroupBox>
                        </div>

                        <div style="width: 100%;">
                            <table border="0" style="width: 100%;">
                                <tr>
                                    <td>
                                        <dx:ASPxButton ID="btnErase" runat="server" AutoPostBack="False" CausesValidation="False" Text="Borrar" ToolTip="Borrar" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                            <ClientSideEvents Click="function(s,e){ frmFilterBusinessCenters_CleanFilter(); }" />
                                            <HoverStyle CssClass="btnFlat-hover btnFlatBlack-hover"></HoverStyle>
                                        </dx:ASPxButton>
                                    </td>
                                    <td style="width: 110px;" align="right">
                                        <dx:ASPxButton ID="btnOk" runat="server" AutoPostBack="False" CausesValidation="False" Text="Aceptar" ToolTip="Aceptar" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                            <ClientSideEvents Click="function(s,e){ frmFilterBusinessCenters_LoadFilter(); }" />
                                            <HoverStyle CssClass="btnFlat-hover btnFlatBlack-hover"></HoverStyle>
                                        </dx:ASPxButton>
                                    </td>
                                    <td style="width: 110px;" align="left">
                                        <dx:ASPxButton ID="btnCancel" runat="server" AutoPostBack="False" CausesValidation="False" Text="Cancelar" ToolTip="Cancelar" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                            <ClientSideEvents Click="function(s,e){ frmFilterBusinessCenters_Cancel(); }" />
                                            <HoverStyle CssClass="btnFlat-hover btnFlatBlack-hover"></HoverStyle>
                                        </dx:ASPxButton>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>
                </dx:PanelContent>
            </PanelCollection>
        </dx:ASPxCallbackPanel>
    </div>
</div>