<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.frmComments" CodeBehind="frmComments.ascx.vb" %>

<!-- Div flotant Comments -->
<input type="hidden" id="hdnCommentsRequestID" />
<input type="hidden" id="hdnCommentsRequestIDTableRow" />
<input type="hidden" id="hdnCommentsApproveRefuse" />
<div id="<%= Me.ClientID %>_frm" style="position: fixed; *position: absolute; z-index: 9010; display: none; top: 50%; left: 50%; *width: 450px;">
    <div id="<%= Me.ClientID %>_BgS" style="position: absolute; top: 0; left: 0; display: none; z-index: 15009;"></div>
    <div class="bodyPopupExtended" style="">
        <div style="width: 98%; height: 100%; background-color: White;" class="bodyPopup">
            <table style="width: 100%; padding-top: 5px;" border="0">
                <tr>
                    <td colspan="2">
                        <div class="panHeader2">
                            <span style="">
                                <asp:Label runat="server" ID="lblComments" Text="Comentarios"></asp:Label></span>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="padding: 2px;">
                        <table border="0" style="width: 100%;">
                            <tr>
                                <td></td>
                                <td>
                                    <asp:Label ID="lblTitleFormComments" runat="server" CssClass="spanEmp-class" Text="Introduzca los comentarios:"></asp:Label></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="padding: 2px; padding-bottom: 10px;" align="center">
                        <textarea id="txtfrmComments" rows="5" style="width: 375px; height: 100px;" class="textClass" convertcontrol="TextArea" ccallowblank="true"></textarea>
                    </td>
                </tr>
            </table>
            <table border="0" style="width: 100%;">
                <tr>
                    <td>&nbsp;</td>
                    <td style="width: 110px;" align="right">
                        <div class="btnFlat btnFlatBlack">
                            <a href="javascript: void(0)" id="aAcept" runat="server" onclick="if (document.getElementById('hdnCommentsApproveRefuse').value == '0') { approveRequest(document.getElementById('hdnCommentsRequestID').value, document.getElementById('hdnCommentsRequestIDTableRow').value ,document.getElementById('txtfrmComments').value); } else { refuseRequest(document.getElementById('hdnCommentsRequestID').value, document.getElementById('hdnCommentsRequestIDTableRow').value, document.getElementById('txtfrmComments').value); } closeComments(); return false;">
                                <asp:Label ID="btnOk" runat="server" Text="Aceptar"></asp:Label>
                            </a>
                        </div>
                    </td>
                    <td style="width: 110px;" align="left">
                        <div class="btnFlat btnFlatBlack">
                            <a href="javascript: void(0)" id="aCancel" runat="server" onclick="closeComments(); return false;">
                                <asp:Label ID="btnCancel" runat="server" Text="Cancelar"></asp:Label>
                            </a>
                        </div>
                    </td>
                </tr>
            </table>
        </div>
    </div>
</div>
<!-- End Div flotant Comments -->