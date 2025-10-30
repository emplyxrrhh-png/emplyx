<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.frmNewAccPermission" CodeBehind="frmNewAccPermission.ascx.vb" %>

<!-- Div flotant NewAccPermission -->
<input type="hidden" id="hdnAddNewAccPermissionID" />
<input type="hidden" id="hdnAddAccPermissionIDRow" />
<div id="<%= Me.ClientID %>_frm" style="position: fixed; *position: absolute; z-index: 10999; display: none; top: 50%; left: 50%; *width: 400px;">
    <div id="<%= Me.ClientID %>_BgS" style="position: absolute; top: 0; left: 0; display: none; z-index: 10998;"></div>
    <div class="bodyPopupExtended" style="">
        <!-- Controls Popup Aqui -->
        <div style="width: 98%; height: 100%; background-color: White;" class="bodyPopup">
            <table style="width: 100%; padding-top: 5px;" border="0">
                <tr>
                    <td colspan="2">
                        <div class="panHeader2">
                            <span style="">
                                <asp:Label runat="server" ID="lblAccessPermission" Text="Nuevos permisos de acceso"></asp:Label></span>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="padding: 2px;">
                        <table border="0" style="width: 100%;">
                            <tr>
                                <td></td>
                                <td style="text-align: left;">
                                    <asp:Label ID="lblTitleFormNewAccPermission" runat="server" CssClass="spanEmp-class" Text="Introducir una relación entre una zona y un período de acceso para el grupo seleccionado."></asp:Label></td>
                            </tr>
                            <tr>
                                <td colspan="3" style="padding-top: 5px; padding-bottom: 10px;" align="center">
                                    <table style="width: 100%" border="0">
                                        <tr>
                                            <td style="padding-left: 20px; text-align: left;">
                                                <asp:Label ID="lblZoneAccPermission" CssClass="spanEmp-class" runat="server" Text="Zona a la que se dará acceso"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="padding-left: 25px; text-align: center;">
                                                <dx:ASPxComboBox ID="cmbZone" runat="server" Width="200px" ClientInstanceName="cmbZoneClient">
                                                    <ClientSideEvents SelectedIndexChanged="function(s,e){}" />
                                                </dx:ASPxComboBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="padding-left: 20px; text-align: left;">
                                                <asp:Label ID="lblPeriodAccPermission" CssClass="spanEmp-class" runat="server" Text="Período durante el que se podrá acceder."></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="padding-left: 25px; text-align: center;">
                                                <dx:ASPxComboBox ID="cmbPeriod" runat="server" Width="200px" ClientInstanceName="cmbPeriodClient">
                                                    <ClientSideEvents SelectedIndexChanged="function(s,e){}" />
                                                </dx:ASPxComboBox>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td></td>
                                <td style="text-align: left;">
                                    <asp:Label ID="Label11" runat="server" CssClass="spanEmp-class" Text="Los períodos sólo aplican a terminales de control de accesos."></asp:Label></td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <div style="clear: both; height: 20px;">
                <table style="float: right;">
                    <tr>
                        <td>
                            <dx:ASPxButton ID="btnAccept" ClientInstanceName="btnAcceptClient" runat="server" AutoPostBack="False" CausesValidation="False" Text="${Button.Accept}" ToolTip="${Button.Accept}"
                                HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                <ClientSideEvents Click="save" />
                            </dx:ASPxButton>
                        </td>
                        <td>
                            <dx:ASPxButton ID="btnCancel" ClientInstanceName="btnCancelClient" runat="server" AutoPostBack="False" CausesValidation="False" Text="${Button.Cancel}" ToolTip="${Button.Cancel}"
                                HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                <ClientSideEvents Click="close" />
                            </dx:ASPxButton>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
</div>
<!-- End Div flotant AddNewAccPermission -->