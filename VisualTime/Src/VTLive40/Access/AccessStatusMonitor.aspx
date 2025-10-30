<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.AccessStatusMonitor" Title="" CodeBehind="AccessStatusMonitor.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script language="javascript" type="text/javascript">

        //$(document).ready(function() {
        //    //ConvertControls();
        //    IniMonitor();
        //})
    </script>
</head>
<body>
    <form id="form1" runat="server">

        <!-- <input type="hidden" runat="server" id="hdnSeconds" value="5" /> -->
        <!-- <input type="hidden" runat="server" id="hdnHours" value="1" /> -->
        <input type="hidden" runat="server" id="hdnListZones" value="" />

        <div style="width: 100%; height: 100%; vertical-align: top;" id="divMainBody">
            <table cellpadding="0" cellspacing="0" border="0" style="width: 100%; height: 100%;">
                <tr>
                    <td style="width: 100%; height: 100%;" valign="top" align="left">
                        <!-- Contenido -->
                        <div id="divContenido" style="text-align: left; vertical-align: top; padding: 0px; height: 95%;">
                            <div style="width: 100%; height: 100%; padding: 0px;">
                                <div class="RoundCornerFrame roundCorner">
                                    <table cellpadding="0" cellspacing="0" width="100%" height="100%" border="0">
                                        <tr>
                                            <td valign="top" style="padding-top: 2px;">
                                                <div class="monitorList">
                                                    <div style="float: left;">
                                                        <asp:CheckBoxList ID="chkListzones" runat="server" RepeatColumns="5" CausesValidation="False" AutoPostBack="False" Height="100%" CellSpacing="5"></asp:CheckBoxList>
                                                    </div>
                                                    <div style="float: right; text-align: right; padding-top: 3px; padding-right: 3px; padding-bottom: 3px;">
                                                        <asp:Button ID="btnListzones" runat="server" Text="Aplicar" OnClientClick="StopMonitor();" />
                                                    </div>
                                                    <div style="clear: both;"></div>
                                                </div>
                                                <div id="divContent" style="height: 90%; width: 1100px; margin-left: auto; margin-right: auto;"></div>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>