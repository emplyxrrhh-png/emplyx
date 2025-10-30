<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.Requests_WebUserControls_frmRequestResume" CodeBehind="frmRequestResume.ascx.vb" %>

<table cellpadding="0" cellspacing="0" width="100%">
    <tr>
        <td valign="top" style="height: 50px; border-bottom: solid 1px silver;">
            <table cellpadding="0" cellspacing="0" border="0" width="100%">
                <tr>
                    <td rowspan="4" style="width: 40px; vertical-align: top; padding-top: 5px;">
                        <img id="imgEmployee" style="cursor: pointer; border-radius: 50%;" height="48" runat="server" />
                    </td>
                    <td colspan="2" align="left" style="padding-top: 3px; padding-left: 5px;">
                        <asp:Label ID="lblEmployeeName" Style="font-size: 13px;" runat="server"></asp:Label>
                    </td>
                    <td align="right">
                        <table cellpadding="0" cellspacing="0">
                            <tr>
                                <td style="height: 20px;">
                                    <div id="divApproveRequest" runat="server" style="height: 20px;">
                                    </div>
                                </td>
                                <td style="height: 20px;">
                                    <div id="divRefuseRequest" runat="server" style="height: 20px;">
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="left" style="width: 20px; padding-left: 15px; padding-top: 5px;">
                        <img id="imgRequestType" runat="server" />
                    </td>
                    <td align="left" style="padding-top: 0px; padding-left: 5px;">
                        <asp:Label ID="lblRequestType" Style="font-size: 11px; font-weight: normal;" runat="server"></asp:Label>
                    </td>
                    <td align="right">
                        <table cellpadding="0" cellspacing="0">
                            <tr>
                                <td>
                                    <img alt="" src="<%= Me.Page.ResolveURL("~/Requests/Images/RequestDate16.png") %>" />
                                </td>
                                <td style="padding-left: 5px;">
                                    <asp:Label ID="lblFromDays" Style="font-size: 11px;" runat="server"></asp:Label>
                                </td>
                                <td style="padding-left: 10px;">
                                    <img alt="" src="<%= Me.Page.ResolveURL("~/Requests/Images/LastApproval16.png") %>" />
                                </td>
                                <td style="padding-left: 5px; padding-right: 5px;">
                                    <asp:Label ID="lblLastApprovalDays" Style="font-size: 11px;" runat="server"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="left" style="padding-top: 0px; padding-left: 15px;">
                        <asp:Label ID="lblRequestInfo" Style="font-size: 11px;" runat="server"></asp:Label>
                    </td>
                    <td align="right" style="padding-right: 5px;">
                        <asp:Label ID="lblLastAction" Style="font-size: 11px;" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="left" style="padding-top: 3px; padding-left: 15px;">
                        <asp:Label ID="lblRequestDate" Style="font-size: 11px;" runat="server"></asp:Label>
                    </td>
                    <td align="right" style="padding-right: 5px;">
                        <!-- <asp:Label ID="lblNextLevelPassports" style="font-size: 11px; " runat="server" ></asp:Label> -->
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>