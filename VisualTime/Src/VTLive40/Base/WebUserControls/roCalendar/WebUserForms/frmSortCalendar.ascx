<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.WebUserForms_frmSortCalendar" CodeBehind="frmSortCalendar.ascx.vb" %>

<div id="<%= Me.ClientID %>_frm" class="ui-dialog-content">
    <form id="<%= Me.ClientID %>_attr">
        <table width="100%" cellspacing="0" class="bodyPopup">
            <tr style="height: 20px;">
                <td colspan="3">
                    <div class="panHeader2">
                        <span style="">
                            <asp:Label ID="lblSortTitle" runat="server" Text="Ordenar calendario" />
                        </span>
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan="3" style="padding: 15px; padding-bottom: 0px;">
                    <asp:Label ID="lblErrorDescription" runat="server" Text="Seleccione el orden de los campos por los que se realizará la ordenación." CssClass="editTextFormat" />
                </td>
            </tr>
            <tr>
                <td colspan="3" style="padding: 15px; padding-bottom: 0px;">
                    <ul id="<%= Me.ClientID %>_sortCalendar" class="sortCalendar">
                        <li class="ui-state-default" data-orderelement="assignment"><span class="ui-icon ui-icon-arrowthick-2-n-s"></span><%= Me.Language.Translate("Order.Assignment", Me.DefaultScope) %></li>
                        <li class="ui-state-default" data-orderelement="group"><span class="ui-icon ui-icon-arrowthick-2-n-s"></span><%= Me.Language.Translate("Order.Group", Me.DefaultScope) %></li>
                        <li class="ui-state-default" data-orderelement="employee"><span class="ui-icon ui-icon-arrowthick-2-n-s"></span><%= Me.Language.Translate("Order.Employee", Me.DefaultScope) %></li>
                        <li class="ui-state-default" data-orderelement="shift"><span class="ui-icon ui-icon-arrowthick-2-n-s"></span><%= Me.Language.Translate("Order.Shift", Me.DefaultScope) %></li>
                        <li class="ui-state-default" data-orderelement="budget"><span class="ui-icon ui-icon-arrowthick-2-n-s"></span><%= Me.Language.Translate("Order.Budget", Me.DefaultScope) %></li>
                    </ul>
                </td>
            </tr>
        </table>
    </form>
</div>