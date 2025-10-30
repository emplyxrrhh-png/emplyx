<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.frmAddAuthorization" CodeBehind="frmAddAuthorization.ascx.vb" %>

<!-- Div flotant AddAuthorization -->
<input type="hidden" id="hdnAddAuthorizationID" />
<input type="hidden" id="hdnAddAuthorizationType" />
<input type="hidden" id="hdnAddAuthorizationIDRow" />
<input type="hidden" id="hdnbeginDate" />
<input type="hidden" id="hdnendDate" />
<input type="hidden" id="AuthDates" />

<div id="<%= Me.ClientID %>_frm" style="position: fixed; *position: absolute; z-index: 999; display: none; top: 50%; left: 50%; *width: 600px;">
    <div id="<%= Me.ClientID %>_BgS" style="position: absolute; top: 0; left: 0; display: none; z-index: 998;"></div>
    <div class="bodyPopupExtended" style="">
        <div style="">
            <div style="width: 100%; height: 100%; background-color: White;" class="bodyPopup">
                <table style="width: 100%; padding-top: 5px;" border="0">
                    <tr>
                        <td colspan="2">
                            <div class="panHeader2">
                                <span style="">
                                    <asp:Label runat="server" ID="lblAddAuthorization" Text="Añadir Autorización"></asp:Label></span>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" style="padding: 2px;">
                            <table border="0" style="width: 100%;">
                                <tr>
                                    <td></td>
                                    <td>
                                        <asp:Label ID="lblTitleFormAddAuthorization" runat="server" CssClass="spanEmp-class" Text="Este formulario le permite añadir autorizaciones y especificar las fechas de validez de las mismas dentro del periodo establecido"></asp:Label></td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" style="padding: 2px; padding-bottom: 10px;">
                            <br />
                            <div class="clear: both">
                                <table border="0" style="padding-left: 5px;">
                                    <tr>
                                        <td>
                                            <div style="float: left; font-weight: bold; width: 110px;">
                                                <asp:Label ID="lblAuthorizations" runat="server" Text="Autorizaciones:"></asp:Label>
                                            </div>
                                            <div style="float: left; padding-left: 7px;">
                                                <dx:ASPxComboBox ID="tbAuthorizations" runat="server" Width="100%" ClientInstanceName="tbAuthorizationsClient">
                                                    <ClientSideEvents TextChanged="" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" Init="" />
                                                </dx:ASPxComboBox>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <br />
                            <div class="clear: both">
                                <table border="0" style="padding-left: 5px;">
                                    <tr>
                                        <td>
                                            <div style="float: left; font-weight: bold; width: 110px;">
                                                <asp:Label ID="lblopTypeMultipleDates" runat="server" Text="Fechas de validez:"></asp:Label>
                                            </div>
                                            <div style="float: left; padding-left: 7px; width: 635px;">
                                                <dx:ASPxTokenBox ID="tbAvailableDates" runat="server" Width="100%" ClientInstanceName="tbAvailableDatesClient">
                                                    <ClientSideEvents TextChanged="" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" Init="" />
                                                </dx:ASPxTokenBox>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>
                </table>
                <roUserControls:roOptPanelClientGroup ID="optAuthorizationGroup" runat="server" />
                <table border="0" style="width: 100%;">
                    <tr>
                        <td>&nbsp;</td>
                        <td style="width: 110px;" align="right">
                            <dx:ASPxButton ID="btnOk" ClientInstanceName="btnAcceptClient" runat="server" AutoPostBack="False" CausesValidation="False" Text="${Button.Accept}" ToolTip="${Button.Accept}"
                                HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                <ClientSideEvents Click="function(s,e) {frmAddAuthorization_Save();}" />
                            </dx:ASPxButton>
                        </td>
                        <td style="width: 110px;" align="left">
                            <dx:ASPxButton ID="btnCancel" ClientInstanceName="btnCancelClient" runat="server" AutoPostBack="False" CausesValidation="False" Text="${Button.Cancel}" ToolTip="${Button.Cancel}"
                                HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                <ClientSideEvents Click="function(s,e) {frmAddAuthorization_Close();}" />
                            </dx:ASPxButton>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
</div>
<!-- End Div flotant AddAuthorization -->