<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.frmDayDetails" CodeBehind="frmDayDetails.ascx.vb" %>

<!-- Div flotant Comments -->
<input type="hidden" id="hdnDayDetailsRequestID" />

<div id="<%= Me.ClientID %>_frm" style="position: fixed; *position: absolute; z-index: 9010; display: none; top: 50%; left: 50%; *width: 450px;">
    <div id="<%= Me.ClientID %>_BgS" style="position: absolute; top: 0; left: 0; display: none; z-index: 15009;"></div>
    <div class="bodyPopupExtended" style="">
        <div style="width: 98%; height: 100%; background-color: White;" class="bodyPopup">
            <table style="width: 100%; padding-top: 5px;" border="0">
                <tr>
                    <td colspan="2">
                        <div class="panHeader2">
                            <span style="">
                                <asp:Label runat="server" ID="lblDayDetailsTitle" Text="Resumen de días solicitados"></asp:Label></span>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="padding: 2px; padding-bottom: 10px;" align="center">
                        <div id="grdDayDetailsResume" style="height: 100px; overflow: auto;" runat="server">
                            <!-- Aqui va el grid de Incidencias Previstas -->
                        </div>
                    </td>
                </tr>
            </table>
            <table border="0" style="width: 100%;">
                <tr>
                    <td>&nbsp;</td>
                    <td style="width: 110px;" align="right">&nbsp;
                    </td>
                    <td style="width: 110px;" align="left">
                        <div class="btnFlat btnFlatBlack">
                            <a href="javascript: void(0)" id="aCancel" runat="server" onclick="closeDayDetails(); return false;">
                                <asp:Label ID="btnCloseDetails" runat="server" Text="Cerrar"></asp:Label>
                            </a>
                        </div>
                    </td>
                </tr>
            </table>
        </div>
    </div>
</div>
<!-- End Div flotant Comments -->