<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.frmNewUserException" CodeBehind="frmNewUserException.ascx.vb" %>

<!-- Div flotant NewRequestCategory -->
<div id="<%= Me.ClientID %>_frm" style="position: fixed; *position: absolute; z-index: 10999; display: none; top: 50%; left: 50%; *width: 400px;">
    <div id="<%= Me.ClientID %>_BgS" style="position: absolute; top: 0; left: 0; display: none; z-index: 10998;"></div>
    <div class="bodyPopupExtended" style="">
        <!-- Controls Popup Aqui -->
        <div style="height: 100%; background-color: White; margin-bottom: 20px;" class="bodyPopup">
            <table style="width: 100%;" border="0">
                <tr>
                    <td colspan="2">
                        <div class="panHeader2">
                            <span style="">
                                <asp:Label runat="server" ID="lblNewUserException" Text="Añadir excepción a usurio"></asp:Label></span>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="padding: 2px;">
                        <table border="0" style="width: 100%;">
                            <tr>
                                <td></td>
                                <td style="text-align: left;">
                                    <asp:Label ID="lblTitleNewUserException" runat="server" CssClass="spanEmp-class" Text="Seleccione el usuario al que desea excluir"></asp:Label></td>
                            </tr>
                            <tr>
                                <td colspan="3" style="padding-top: 5px; padding-bottom: 10px;" align="center">
                                    <table style="width: 100%" border="0">
                                        <tr>
                                            <td style="padding-left: 20px; text-align: left;">
                                                <asp:Label ID="lblcmbAvailablePassport" CssClass="spanEmp-class" runat="server" Text="Usuario"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="padding-left: 25px; text-align: center;">
                                                <dx:ASPxComboBox ID="cmbPassportAvailable" runat="server" Width="350px" ClientInstanceName="cmbPassportAvailableClient">
                                                    <ClientSideEvents SelectedIndexChanged="function(s,e){}" />
                                                </dx:ASPxComboBox>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <div style="clear: both; height: 20px;">
                <table style="float: right;">
                    <tr>
                        <td>
                            <dx:ASPxButton ID="btnAccept" ClientInstanceName="btnAcceptEXClient" runat="server" AutoPostBack="False" CausesValidation="False" Text="${Button.Accept}" ToolTip="${Button.Accept}"
                                HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                <ClientSideEvents Click="saveUserException" />
                            </dx:ASPxButton>
                        </td>
                        <td>
                            <dx:ASPxButton ID="btnCancel" ClientInstanceName="btnCancelEXClient" runat="server" AutoPostBack="False" CausesValidation="False" Text="${Button.Cancel}" ToolTip="${Button.Cancel}"
                                HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                <ClientSideEvents Click="closeUserException" />
                            </dx:ASPxButton>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
</div>