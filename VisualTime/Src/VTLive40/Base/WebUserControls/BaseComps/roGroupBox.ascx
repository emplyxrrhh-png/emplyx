<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.base_WebUserControls_roGroupBox" CodeBehind="roGroupBox.ascx.vb" %>
<div id="<%= Me.ClientID %>_tblContainerClient" class="optionPanelRoboticsV2">
    <%--<tr>
    <td class="<%= tblCSSPrefix %>-tl" height="7"></td>
    <td class="<%= tblCSSPrefix %>-tm" height="7"></td>
    <td class="<%= tblCSSPrefix %>-tr" height="7"></td>
</tr>
<tr>
    <td class="<%= tblCSSPrefix %>-ml"></td>
    <td class="<%= tblCSSPrefix %>-mm">--%>
    <asp:PlaceHolder ID="externalContent" runat="server" />
    <%--</td>
    <td class="<%= tblCSSPrefix %>-mr"></td>
</tr>
<tr>
    <td class="<%= tblCSSPrefix %>-bl"></td>
    <td class="<%= tblCSSPrefix %>-bm"></td>
    <td class="<%= tblCSSPrefix %>-br"></td>
</tr>
</table>--%>
</div>