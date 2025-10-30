<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.frmAddSiren" CodeBehind="frmAddSiren.ascx.vb" %>

<!-- Div flotant AddSiren -->
<input type="hidden" id="hdnAddSirenID" />
<input type="hidden" id="hdnAddSirenIDRow" />
<div id="<%= Me.ClientID %>_frm" style="position: fixed; *position: absolute; z-index: 9010; display: none; top: 50%; left: 50%; *width: 450px;">
    <div id="<%= Me.ClientID %>_BgS" style="position: absolute; top: 0; left: 0; display: none; z-index: 15009;"></div>
    <div class="bodyPopupExtended" style="">
        <!-- Controls Popup Aqui -->
        <div style="">
            <div style="width: 100%; height: 100%; background-color: White;" class="bodyPopup">
                <table style="width: 100%; padding-top: 5px;" border="0">
                    <tr>
                        <td colspan="2">
                            <div class="panHeader2">
                                <span style="">
                                    <asp:Label runat="server" ID="lblAddSiren" Text="Sirenas"></asp:Label></span>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" style="padding: 2px;">
                            <table border="0" style="width: 100%;">
                                <tr>
                                    <td></td>
                                    <td>
                                        <asp:Label ID="lblTitleFormAddSiren" runat="server" CssClass="spanEmp-class" Text="Introducir un nuevo toque de sirena para que el terminal active el relé a la hora correspondiente"></asp:Label></td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" style="padding: 2px 2px 5px 32px; padding-bottom: 10px;" align="left">
                            <table>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblNewSirenDay" runat="server" Text="El nuevo toque sonará el "></asp:Label></td>
                                    <td style="padding-left: 7px;" colspan="2">
                                        <dx:ASPxComboBox ID="cmbSirenWeekDay" runat="server" Width="150px" ClientInstanceName="cmbSirenWeekDayClient">
                                            <ClientSideEvents SelectedIndexChanged="function(s,e){ hasChanges(true); }" />
                                        </dx:ASPxComboBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblNewSirenTime" runat="server" Text=" a las "></asp:Label></td>
                                    <td style="padding-left: 7px;">
                                        <dx:ASPxTimeEdit ID="txtSirenTime" runat="server" EditFormatString="HH:mm" EditFormat="Custom" ClientInstanceName="txtSirenTimeClient">
                                            <ClientSideEvents DateChanged="function(s,e){ hasChanges(true); }" />
                                        </dx:ASPxTimeEdit>
                                    </td>
                                    <td>
                                        <asp:Label ID="lblNewSirenHours" runat="server" Text=" horas,"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblNewSirenDuration" runat="server" Text=" durante "></asp:Label></td>
                                    <td style="padding-left: 7px;">
                                        <dx:ASPxTextBox ID="txtSirenDuration" runat="server" MaxLength="3" ClientInstanceName="txtSirenDurationClient">
                                            <MaskSettings Mask="<0..999>" />
                                            <ClientSideEvents TextChanged="function(s,e){hasChanges(true); }" />
                                        </dx:ASPxTextBox>
                                    </td>
                                    <td>
                                        <asp:Label ID="lblNewSirenSeconds" runat="server" Text=" segundos."></asp:Label>
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
                            <dx:ASPxButton ID="btnOk" ClientInstanceName="btnAcceptClient" runat="server" AutoPostBack="False" CausesValidation="False" Text="Aceptar" ToolTip="Aceptar"
                                HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                <ClientSideEvents Click="function(s, e) { frmAddSiren_Save(); }" />
                            </dx:ASPxButton>
                        </td>
                        <td style="width: 110px;" align="left">
                            <dx:ASPxButton ID="btnCancel" ClientInstanceName="btnCancelClient" runat="server" AutoPostBack="False" CausesValidation="False" Text="Cancelar" ToolTip="Cancelar"
                                HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                <ClientSideEvents Click="function(s, e) { frmAddSiren_Close(); }" />
                            </dx:ASPxButton>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
</div>
<!-- End Div flotant AddException -->