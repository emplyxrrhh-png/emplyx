<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.srvAccessStatusMonitor" CodeBehind="srvAccessStatusMonitor.aspx.vb" %>

<div id="divZoneInfo" style="width: 100%; height: 100%; padding: 0px;" name="menuPanel">
    <table style="margin: 10px; width: 99%;">
        <tr>
            <td width="45%" align="left" valign="top">
                <div class="panHeader2">
                    <span style="">
                        <asp:label runat="server" id="lblTitleEmpInZone" text="Empleados que hay actualmente"></asp:label>
                    </span>
                </div>
                <table border="0" style="text-align: left;">
                    <tr>
                        <td width="100%" align="center" valign="top">
                            <table width="100%" border="0" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td height="20px" valign="top">
                                        <div id="divHeaderEmpInZone" runat="server" style="width: 100%; height: auto;">
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td valign="top">
                                        <div id="divGridEmpInZone" runat="server" style="width: 100%; height: 720px; overflow: auto;">
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
            <td width="50%" align="left" valign="top">
                <div class="panHeader2">
                    <span style="">
                        <asp:label id="lblTitleIncorrectAccess" runat="server" text="Accesos incorrectos en esta zona"></asp:label>
                    </span>
                </div>
                <table border="0" style="text-align: left;">
                    <tr>
                        <td width="100%" align="center" valign="top">
                            <table width="100%" border="0" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td height="20px" valign="top">
                                        <div id="divHeaderIncorrectAccess" runat="server" style="width: 100%; height: auto;">
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td valign="top">
                                        <div id="divGridIncorrectAccess" runat="server" style="width: 100%; height: 720px; overflow: auto;">
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</div>