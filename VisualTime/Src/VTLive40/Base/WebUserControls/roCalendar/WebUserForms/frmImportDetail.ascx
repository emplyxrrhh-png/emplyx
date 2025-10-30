<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.WebUserForms_frmImportDetail" CodeBehind="frmImportDetail.ascx.vb" %>

<div id="<%= Me.ClientID %>_frm" class="ui-dialog-content">
    <form id="<%= Me.ClientID %>_attr">
        <table width="100%" cellspacing="0" class="bodyPopup">
            <tr style="height: 20px;">
                <td colspan="3">
                    <div class="panHeader2">
                        <span style="">
                            <asp:Label ID="lblErrorTitle" runat="server" Text="Resultado de la importación" />
                        </span>
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan="3" style="padding: 15px; padding-bottom: 0px;">
                    <asp:Label ID="lblErrorDescription" runat="server" Text="Solo se han podido importar los días que comprendidos entre el periodo de fechas y empleados seleccionados." CssClass="editTextFormat" />
                </td>
            </tr>
            <tr>
                <td colspan="3" style="padding: 15px; padding-bottom: 0px;">
                    <dx:ASPxMemo ID="txtErrorMemo" SkinID="None" runat="server" Native="True" Text="" Height="185px" Width="290px" ReadOnly="true">
                    </dx:ASPxMemo>
                </td>
            </tr>
        </table>
    </form>
</div>