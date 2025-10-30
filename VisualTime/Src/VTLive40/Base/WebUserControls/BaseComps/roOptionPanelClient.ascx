<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.WebUserControls_roOptionPanelClient" CodeBehind="roOptionPanelClient.ascx.vb" %>
<div id="<%= Me.ClientID %>_panOptionPanel" venabled="<%= Me.Enabled %>" vmode="<%= Me.TypeOPanel %>" value="<%= me.Value %>" vclientscript="<%= Me.ClientScript  %>">

    <div id="<%= Me.ClientID %>_tblContainerClient" class="optionPanelRoboticsV2" vclass="optionPanelRoboticsV2" width="100%" height="20px" border="0" cellpadding="0" cellspacing="0">
        <div>
            <table border="0" style="width: 100%;">
                <tr>
                    <td>
                        <table border="0" cellpadding="0" cellspacing="0" style="width: 100%;">
                            <tr>
                                <td valign="top" style="width: 25px;" align="center">
                                    <input type="radio" id="rButton" runat="server" />
                                    <input type="checkbox" id="chkButton" runat="server" />
                                    <img src="" id="imgButton" runat="server" />
                                </td>
                                <td valign="top" align="left">
                                    <a href="javascript: void(0);" class="OptionPanelCheckBoxStyle" style="width: 100%;" id="aTitle" runat="server">
                                        <asp:PlaceHolder ID="externalTitle" runat="server" />
                                    </a>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 25px;"></td>
                                <td>
                                    <a href="javascript: void(0);" class="OptionPanelDescStyle" style="width: 100%; padding-left: 0px;" id="aDescription" runat="server">
                                        <asp:PlaceHolder ID="externalDescription" runat="server" />
                                    </a>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div id="<%= Me.ClientID %>_panContainer" style="display: <%= Me.ContentVisible %>;">
                            <asp:PlaceHolder ID="externalContent" runat="server" />
                        </div>
                    </td>
                </tr>
            </table>
        </div>
    </div>
</div>
<script language="javascript" type="text/javascript">
<% If Me.Enabled = True Then  %>
    enableChildElements('<%= Me.ClientID %>_panOptionPanel');
<% Else %>
    disableChildElements('<%= Me.ClientID %>_panOptionPanel');
<% End If %>
</script>