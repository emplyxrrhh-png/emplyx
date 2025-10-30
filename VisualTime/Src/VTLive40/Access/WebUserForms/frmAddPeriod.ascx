<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.frmAddPeriod" CodeBehind="frmAddPeriod.ascx.vb" %>

<!-- Div flotant AddPeriod -->
<input type="hidden" id="hdnAddPeriodIDZone" />
<input type="hidden" id="hdnAddPeriodIDRow" />
<div id="<%= Me.ClientID %>_frm" style="position: fixed; *position: absolute; z-index: 10999; display: none; top: 50%; left: 50%; *width: 400px;">
    <div id="<%= Me.ClientID %>_BgS" style="position: absolute; top: 0; left: 0; display: none; z-index: 10998;"></div>
    <div class="bodyPopupExtended" style="">
        <div style="width: 98%; height: 100%; background-color: White;" class="bodyPopup">
            <table style="width: 100%; padding-top: 5px;" border="0">
                <tr>
                    <td colspan="2">
                        <div class="panHeader2"><span style="">
                            <asp:Label runat="server" ID="lblAddPeriod" Text="Período de puertas abiertas"></asp:Label></span></div>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="padding: 2px;">
                        <table border="0" style="width: 100%;">
                            <tr>
                                <td></td>
                                <td>
                                    <asp:Label ID="lblTitleFormAddPeriod" runat="server" CssClass="spanEmp-class" Text="Introducir los datos para definir el período de puertas abiertas de la zona."></asp:Label></td>
                            </tr>
                            <tr>
                                <td colspan="3" style="padding-top: 5px; padding-bottom: 10px; text-align: center">
                                    <table border="0" style="width: 100%;">
                                        <tr>
                                            <td style="text-align: center">
                                                <table border="0" cellpadding="1" cellspacing="1">
                                                    <tr>
                                                        <td style="width: 75px;">
                                                            <asp:Label ID="lblAddPeriodInactivity" runat="server" Text="Inactividad el "></asp:Label></td>
                                                        <td colspan="3">
                                                            <dx:ASPxComboBox ID="cmbPeriodWeekDay" runat="server" Width="200px" Font-Size="11px" ForeColor="#2D4155"
                                                                Font-Names="Arial;Verdana;Sans-Serif" IncrementalFilteringMode="Contains" ClientInstanceName="cmbPeriodWeekDayClient">
                                                            </dx:ASPxComboBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="vertical-align: top; padding-top: 3px;">
                                                            <asp:Label ID="lblAddPeriodFrom" runat="server" Text=" de "></asp:Label>
                                                        </td>
                                                        <td style="width: 65px;">
                                                            <dx:ASPxTimeEdit ID="txtPeriodFromTime" EditFormatString="HH:mm" EditFormat="Custom" runat="server" Width="85" ClientInstanceName="txtPeriodFromTimeClient"></dx:ASPxTimeEdit>
                                                        </td>
                                                        <td style="vertical-align: top; padding-top: 3px;">
                                                            <asp:Label ID="lblAddPeriodTo" runat="server" Text=" a "></asp:Label>
                                                        </td>
                                                        <td style="width: 65px;">
                                                            <dx:ASPxTimeEdit ID="txtPeriodToTime" EditFormatString="HH:mm" EditFormat="Custom" runat="server" Width="85" ClientInstanceName="txtPeriodToTimeClient"></dx:ASPxTimeEdit>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <div>
                <table style="float: right; margin-top: -20px;">
                    <tr>
                        <td>
                            <dx:ASPxButton ID="btnAccept" ClientInstanceName="btnAcceptClient" runat="server" AutoPostBack="False" CausesValidation="False" Text="${Button.Accept}" ToolTip="${Button.Accept}"
                                HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                <ClientSideEvents Click="frmAddPeriod_Save" />
                            </dx:ASPxButton>
                        </td>
                        <td>
                            <dx:ASPxButton ID="btnCancel" ClientInstanceName="btnCancelClient" runat="server" AutoPostBack="False" CausesValidation="False" Text="${Button.Cancel}" ToolTip="${Button.Cancel}"
                                HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                <ClientSideEvents Click="frmAddPeriod_Close" />
                            </dx:ASPxButton>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
</div>
<!-- End Div flotant AddPeriod -->