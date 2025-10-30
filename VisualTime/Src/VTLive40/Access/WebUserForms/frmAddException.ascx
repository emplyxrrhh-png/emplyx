<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.frmAddException" CodeBehind="frmAddException.ascx.vb" %>

<!-- Div flotant AddException -->
<input type="hidden" id="hdnAddExceptionIDZone" />
<input type="hidden" id="hdnAddExceptionIDRow" />
<div id="<%= Me.ClientID %>_frm" style="position: fixed; *position: absolute; z-index: 10999; display: none; top: 50%; left: 50%; *width: 350px;">
    <div id="<%= Me.ClientID %>_BgS" style="position: absolute; top: 0; left: 0; display: none; z-index: 10998;"></div>
    <div class="bodyPopupExtended" style="">
        <div style="width: 98%; height: 100%; background-color: White;" class="bodyPopup">
            <table style="width: 100%; padding-top: 5px;" border="0">
                <tr>
                    <td colspan="2">
                        <div class="panHeader2">
                            <span style="">
                                <asp:Label runat="server" ID="lblAddException" Text="Excepciones"></asp:Label></span>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="padding: 2px;">
                        <table border="0" style="width: 100%;">
                            <tr>
                                <td></td>
                                <td style="text-align: left;">
                                    <asp:Label ID="lblTitleFormAddException" runat="server" CssClass="spanEmp-class" Text="Introduzca la fecha en la que no se tendrán en cuenta los periodos de puertas abiertas."></asp:Label></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="padding: 2px; padding-bottom: 10px; text-align: center;">
                        <dx:ASPxDateEdit ID="dpDateException" runat="server" Width="150" AllowNull="false" ClientInstanceName="dpDateExceptionClient">
                        </dx:ASPxDateEdit>
                    </td>
                </tr>
            </table>
            <div>
                <table style="float: right; margin-top: -20px;">
                    <tr>
                        <td>
                            <dx:ASPxButton ID="btnAccept" ClientInstanceName="btnAcceptClient" runat="server" AutoPostBack="False" CausesValidation="False" Text="${Button.Accept}" ToolTip="${Button.Accept}"
                                HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                <ClientSideEvents Click="frmAddException_Save" />
                            </dx:ASPxButton>
                        </td>
                        <td>
                            <dx:ASPxButton ID="btnCancel" ClientInstanceName="btnCancelClient" runat="server" AutoPostBack="False" CausesValidation="False" Text="${Button.Cancel}" ToolTip="${Button.Cancel}"
                                HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                <ClientSideEvents Click="frmAddException_Close" />
                            </dx:ASPxButton>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
</div>
<!-- End Div flotant AddException -->