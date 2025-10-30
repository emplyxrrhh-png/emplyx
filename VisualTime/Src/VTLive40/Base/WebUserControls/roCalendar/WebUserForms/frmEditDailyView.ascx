<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.WebUserForms_frmEditDailyView" CodeBehind="frmEditDailyView.ascx.vb" %>

<div id="<%= Me.ClientID %>_frm" class="ui-dialog-content">
    <form id="<%= Me.ClientID %>_attr">
        <table width="100%" cellspacing="0" class="bodyPopup">
            <tr style="height: 20px;">
                <td colspan="3">
                    <div class="panHeader2">
                        <span style="">
                            <asp:Label ID="lblErrorTitle" runat="server" Text="Configuración de vista diaria" />
                        </span>
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan="3" style="padding: 15px; padding-bottom: 0px;">
                    <dx:ASPxRadioButtonList runat="server" ID="rbViewModes"></dx:ASPxRadioButtonList>
                </td>
            </tr>
        </table>
    </form>
</div>