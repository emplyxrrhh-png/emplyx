<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.WebUserForms_frmNewTypeZone" CodeBehind="frmNewTypeZone.ascx.vb" %>

<!-- Div flotant NewTypeZone -->
<input type="hidden" id="hdnNewTypeZoneLayer" />

<div id="<%= Me.ClientID %>_frm" style="position: fixed; *position: absolute; z-index: 9010; display: none; top: 50%; left: 50%; *width: 900px;">
    <div id="<%= Me.ClientID %>_BgS" style="position: absolute; top: 0; left: 0; display: none; z-index: 9009;"></div>
    <div class="bodyPopupExtended" style="">
        <!-- Controls Popup Aqui -->
        <div style="">
            <div style="width: 100%; height: 100%; background-color: White;" class="bodyPopup">
                <table style="width: 100%; padding-top: 5px;" border="0">
                    <tr>
                        <td colspan="2">
                            <div class="panHeader2">
                                <span style="">
                                    <asp:Label runat="server" ID="lblAddNewTypeZone" Text="Nuevo tipo franja horaria"></asp:Label></span>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" style="padding: 2px;">
                            <table border="0" style="width: 100%;">
                                <tr>
                                    <td></td>
                                    <td>
                                        <asp:Label ID="lblTitleFormAddNewTypeZone" runat="server" CssClass="spanEmp-class" Text="Use este diálogo para crear una nuevo tipo de franja horaria"></asp:Label></td>
                                </tr>
                                <tr>
                                    <td colspan="3" style="padding-top: 5px; padding-bottom: 10px;" align="center">
                                        <table width="100%" border="0">
                                            <tr>
                                                <td style="padding-left: 5px;" valign="top">
                                                    <asp:Label ID="lblNewZoneName" runat="server" Text="Nombre:" CssClass="spanEmp-class"></asp:Label>
                                                </td>
                                                <td style="width: 315px; text-align: right;">
                                                    <dx:ASPxTextBox ID="txtNewZoneName" runat="server" MaxLength="50" Width="310" ClientInstanceName="txtNewZoneNameClient">
                                                        <ClientSideEvents TextChanged="function(s,e){}" />
                                                    </dx:ASPxTextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 5px;" valign="top">
                                                    <asp:Label ID="lblNewZoneDesc" runat="server" Text="Descripción:" CssClass="spanEmp-class"></asp:Label>
                                                </td>
                                                <td style="width: 315px; text-align: right;">
                                                    <dx:ASPxMemo ID="txtNewZoneDesc" runat="server" Rows="5" Width="310px" ClientInstanceName="txtNewZoneDescClient">
                                                        <ClientSideEvents TextChanged="function(s,e){}" />
                                                    </dx:ASPxMemo>
                                                </td>
                                            </tr>
                                        </table>
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
                                <ClientSideEvents Click="function(s,e){ frmNewTypeZone_Save(); }" />
                            </dx:ASPxButton>
                        </td>
                        <td style="width: 110px;" align="left">
                            <dx:ASPxButton ID="btnCancel" runat="server" AutoPostBack="False" CausesValidation="False" Text="Cancelar" ToolTip="Cancelar" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                <ClientSideEvents Click="function(s,e){ frmNewTypeZone_Close(); }" />
                            </dx:ASPxButton>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
</div>
<!-- End Div flotant NewTypeZone -->