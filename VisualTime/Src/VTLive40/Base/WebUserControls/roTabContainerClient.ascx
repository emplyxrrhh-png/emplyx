<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.base_WebUserControls_roTabContainerClient" CodeBehind="roTabContainerClient.ascx.vb" %>
<table border="0" cellpadding="0" cellspacing="0" class="tabControl" style="width: 100%;" name="tabContainerClient">
    <tr>
        <td align="left" style="padding-left: 0; padding-bottom: 0;">
            <table border="0" cellpadding="0" cellspacing="0">
                <tr>
                    <td style="padding: 0; margin: 0; white-space: nowrap; height: 24px;" id="tdTitle1" runat="server">
                        <a id="tab01" runat="server" href="javascript: void(0);" class="tabHeader" onclick="">
                            <asp:PlaceHolder ID="tabTitle01" runat="server"></asp:PlaceHolder>
                        </a>
                    </td>
                    <td style="padding: 0; margin: 0; white-space: nowrap; height: 24px;" id="tdTitle2" runat="server">
                        <a id="tab02" runat="server" href="javascript: void(0);" class="tabHeader" onclick="">
                            <asp:PlaceHolder ID="tabTitle02" runat="server"></asp:PlaceHolder>
                        </a>
                    </td>
                    <td style="padding: 0; margin: 0; white-space: nowrap; height: 24px;" id="tdTitle3" runat="server">
                        <a id="tab03" runat="server" href="javascript: void(0);" class="tabHeader" onclick="">
                            <asp:PlaceHolder ID="tabTitle03" runat="server"></asp:PlaceHolder>
                        </a>
                    </td>
                    <td style="padding: 0; margin: 0; white-space: nowrap; height: 24px;" id="tdTitle4" runat="server">
                        <a id="tab04" runat="server" href="javascript: void(0);" class="tabHeader" onclick="">
                            <asp:PlaceHolder ID="tabTitle04" runat="server"></asp:PlaceHolder>
                        </a>
                    </td>
                    <td style="padding: 0; margin: 0; white-space: nowrap; height: 24px;" id="tdTitle5" runat="server">
                        <a id="tab05" runat="server" href="javascript: void(0);" class="tabHeader" onclick="">
                            <asp:PlaceHolder ID="tabTitle05" runat="server"></asp:PlaceHolder>
                        </a>
                    </td>
                    <td style="padding: 0; margin: 0; white-space: nowrap; height: 24px;" id="tdTitle6" runat="server">
                        <a id="tab06" runat="server" href="javascript: void(0);" class="tabHeader" onclick="">
                            <asp:PlaceHolder ID="tabTitle06" runat="server"></asp:PlaceHolder>
                        </a>
                    </td>
                    <td style="padding: 0; margin: 0; white-space: nowrap; height: 24px;" id="tdTitle7" runat="server">
                        <a id="tab07" runat="server" href="javascript: void(0);" class="tabHeader" onclick="">
                            <asp:PlaceHolder ID="tabTitle07" runat="server"></asp:PlaceHolder>
                        </a>
                    </td>
                    <td style="padding: 0; margin: 0; white-space: nowrap; height: 24px;" id="tdTitle8" runat="server">
                        <a id="tab08" runat="server" href="javascript: void(0);" class="tabHeader" onclick="">
                            <asp:PlaceHolder ID="tabTitle08" runat="server"></asp:PlaceHolder>
                        </a>
                    </td>
                    <td style="padding: 0; margin: 0; white-space: nowrap; height: 24px;" id="tdTitle9" runat="server">
                        <a id="tab09" runat="server" href="javascript: void(0);" class="tabHeader" onclick="">
                            <asp:PlaceHolder ID="tabTitle09" runat="server"></asp:PlaceHolder>
                        </a>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>
            <div id="tbC01" runat="server" style="display: none;">
                <asp:PlaceHolder ID="tabContainer01" runat="server"></asp:PlaceHolder>
            </div>
            <div id="tbC02" runat="server" style="display: none;">
                <asp:PlaceHolder ID="tabContainer02" runat="server"></asp:PlaceHolder>
            </div>
            <div id="tbC03" runat="server" style="display: none;">
                <asp:PlaceHolder ID="tabContainer03" runat="server"></asp:PlaceHolder>
            </div>
            <div id="tbC04" runat="server" style="display: none;">
                <asp:PlaceHolder ID="tabContainer04" runat="server"></asp:PlaceHolder>
            </div>
            <div id="tbC05" runat="server" style="display: none;">
                <asp:PlaceHolder ID="tabContainer05" runat="server"></asp:PlaceHolder>
            </div>
            <div id="tbC06" runat="server" style="display: none;">
                <asp:PlaceHolder ID="tabContainer06" runat="server"></asp:PlaceHolder>
            </div>
            <div id="tbC07" runat="server" style="display: none;">
                <asp:PlaceHolder ID="tabContainer07" runat="server"></asp:PlaceHolder>
            </div>
            <div id="tbC08" runat="server" style="display: none;">
                <asp:PlaceHolder ID="tabContainer08" runat="server"></asp:PlaceHolder>
            </div>
            <div id="tbC09" runat="server" style="display: none;">
                <asp:PlaceHolder ID="tabContainer09" runat="server"></asp:PlaceHolder>
            </div>
        </td>
    </tr>
</table>
<br />
<input type="hidden" id="hdnActiveTab" runat="server" />
<script language="javascript" type="text/javascript">
    eval("var <%= Me.ClientID %>_arrTdTabs = new Array('<%= tdTitle1.ClientID %>','<%= tdTitle2.ClientID %>','<%= tdTitle3.ClientID %>','<%= tdTitle4.ClientID %>','<%= tdTitle5.ClientID %>','<%= tdTitle6.ClientID %>','<%= tdTitle7.ClientID %>','<%= tdTitle8.ClientID %>','<%= tdTitle9.ClientID %>');");
    eval("var <%= Me.ClientID %>_arrTabs = new Array('<%= tab01.ClientID %>','<%= tab02.ClientID %>','<%= tab03.ClientID %>','<%= tab04.ClientID %>','<%= tab05.ClientID %>','<%= tab06.ClientID %>','<%= tab07.ClientID %>','<%= tab08.ClientID %>','<%= tab09.ClientID %>');");
    eval("var <%= Me.ClientID %>_arrTabContainers = new Array('<%= tbC01.ClientID %>','<%= tbC02.ClientID %>','<%= tbC03.ClientID %>','<%= tbC04.ClientID %>','<%= tbC05.ClientID %>','<%= tbC06.ClientID %>','<%= tbC07.ClientID %>','<%= tbC08.ClientID %>','<%= tbC09.ClientID %>');");
</script>