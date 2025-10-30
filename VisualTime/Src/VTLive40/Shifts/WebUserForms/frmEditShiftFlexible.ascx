<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.WebUserForms_frmEditShiftFlexible" CodeBehind="frmEditShiftFlexible.ascx.vb" %>

<input type="hidden" id="hdnFlexibleLayerID" />
<input type="hidden" id="hdnFlexibleParentID" />

<!-- Div flotant EditShiftFlexible -->
<input type="hidden" id="<%= Me.ClientID %>_hdnRuleChanges" value="0" />

<div id="<%= Me.ClientID %>_frm" style="position: fixed; *position: absolute; z-index: 9010; display: none; top: 50%; left: 50%; *width: 900px;">
    <div id="<%= Me.ClientID %>_BgS" style="position: absolute; top: 0; left: 0; display: none; z-index: 9009;"></div>
    <div class="bodyPopupExtended" style="">
        <div style="">
            <div style="width: 100%; height: 100%; background-color: White;" class="bodyPopup">
                <table style="width: 100%; padding-top: 5px;" border="0">
                    <tr>
                        <td colspan="2">
                            <div class="panHeader2">
                                <span style="">
                                    <asp:Label runat="server" ID="lblEditShiftFlexible" Text="Editar horario flexible"></asp:Label></span>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" style="padding: 2px;">
                            <table border="0" style="width: 450px;">
                                <tr>
                                    <td style="padding-top: 5px;">
                                        <roUserControls:roTabContainerClient ID="tbCont1" runat="server">
                                            <TabTitle1>
                                                <asp:Label ID="lblTitInterval" runat="server" Text="Intervalo"></asp:Label>
                                            </TabTitle1>
                                            <TabContainer1>
                                                <table style="margin: 5px; height: 130px;">
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lblEmpPresDesc" runat="server" Text="El empleado puede estar presente:" CssClass="spanEmp-class"></asp:Label></td>
                                                    </tr>
                                                    <tr>
                                                        <td align="center">
                                                            <table>
                                                                <tr>
                                                                    <td style="width: 100px;">
                                                                        <asp:Label ID="lblPresFromTime" runat="server" Text="Desde las " CssClass="spanEmp-class"></asp:Label></td>
                                                                    <td style="width: 75px;">
                                                                        <dx:ASPxTimeEdit ID="txtPresFromTime" runat="server" Width="85px" EditFormatString="HH:mm" EditFormat="Custom" ClientInstanceName="txtPresFromTimeFlexClient">
                                                                            <ClientSideEvents DateChanged="function(s,e){}" />
                                                                        </dx:ASPxTimeEdit>
                                                                    </td>
                                                                    <td>
                                                                        <dx:ASPxComboBox runat="server" ID="cmbPresFromTime" Width="175px" ClientInstanceName="cmbPresFromTimeFlexClient">
                                                                            <ClientSideEvents SelectedIndexChanged="function(s,e){}" />
                                                                        </dx:ASPxComboBox>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td style="width: 100px;">
                                                                        <asp:Label ID="lblPresToTime" runat="server" Text="y hasta las " CssClass="spanEmp-class"></asp:Label></td>
                                                                    <td style="width: 75px;">
                                                                        <dx:ASPxTimeEdit ID="txtPresToTime" runat="server" Width="85px" EditFormatString="HH:mm" EditFormat="Custom" ClientInstanceName="txtPresToTimeFlexClient">
                                                                            <ClientSideEvents DateChanged="function(s,e){}" />
                                                                        </dx:ASPxTimeEdit>
                                                                    </td>
                                                                    <td>
                                                                        <dx:ASPxComboBox runat="server" ID="cmbPresToTime" Width="175px" ClientInstanceName="cmbPresToTimeFlexClient">
                                                                            <ClientSideEvents SelectedIndexChanged="function(s,e){}" />
                                                                        </dx:ASPxComboBox>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </TabContainer1>
                                            <TabTitle2>
                                                <asp:Label ID="lblTitFilters" runat="server" Text="Filtros"></asp:Label>
                                            </TabTitle2>
                                            <TabContainer2>
                                                <table style="margin: 5px; height: 130px;">
                                                    <tr>
                                                        <td>
                                                            <table>
                                                                <tr>
                                                                    <td>
                                                                        <asp:Label ID="lblDurTime" runat="server" Text="Durante este tiempo, debe estar presente entre" CssClass="spanEmp-class"></asp:Label></td>
                                                                    <td style="width: 75px;">
                                                                        <dx:ASPxTimeEdit ID="txtDurTime1" runat="server" Width="85px" EditFormatString="HH:mm" EditFormat="Custom" ClientInstanceName="txtDurTime1FlexClient">
                                                                            <ClientSideEvents DateChanged="function(s,e){}" />
                                                                        </dx:ASPxTimeEdit>
                                                                    </td>
                                                                    <td>&nbsp;<asp:Label ID="lblDurTimeAnd" runat="server" Text="y" CssClass="spanEmp-class"></asp:Label>&nbsp;</td>
                                                                    <td style="width: 75px;">
                                                                        <dx:ASPxTimeEdit ID="txtDurTime2" runat="server" Width="85px" EditFormatString="HH:mm" EditFormat="Custom" ClientInstanceName="txtDurTime2FlexClient">
                                                                            <ClientSideEvents DateChanged="function(s,e){}" />
                                                                        </dx:ASPxTimeEdit>
                                                                    </td>
                                                                    <td>&nbsp;<asp:Label ID="lblDurTimeHours" runat="server" Text="horas." CssClass="spanEmp-class"></asp:Label></td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right">
                                                            <asp:Label ID="lblDMinim" runat="server" Text="Si no llega al mínimo, se generará una incidencia." CssClass="spanEmp-class"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="left">
                                                            <table>
                                                                <tr>
                                                                    <td>
                                                                        <asp:Label ID="lblDMinThen" runat="server" Text="Si supera el máximo, entonces " CssClass="spanEmp-class"></asp:Label></td>
                                                                    <td>
                                                                        <dx:ASPxComboBox runat="server" ID="cmbMaxThen" Width="175px" ClientInstanceName="cmbMaxThenFlexClient">
                                                                            <ClientSideEvents SelectedIndexChanged="function(s,e){}" />
                                                                        </dx:ASPxComboBox>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </TabContainer2>
                                        </roUserControls:roTabContainerClient>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <table border="0" style="width: 100%;">
                    <tr>
                        <td>&nbsp;</td>
                        <td style="width: 110px;" align="right">
                            <dx:ASPxButton ID="btnOk" runat="server" AutoPostBack="False" CausesValidation="False" Text="Aceptar" ToolTip="Aceptar" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                <ClientSideEvents Click="function(s,e){ frmEditShiftFlexible_Save(); }" />
                            </dx:ASPxButton>
                        </td>
                        <td style="width: 110px;" align="left">
                            <dx:ASPxButton ID="btnCancel" runat="server" AutoPostBack="False" CausesValidation="False" Text="Cancelar" ToolTip="Cancelar" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                <ClientSideEvents Click="function(s,e){ frmEditShiftFlexible_Close(); }" />
                            </dx:ASPxButton>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
</div>
<!-- End Div flotant Addshift -->