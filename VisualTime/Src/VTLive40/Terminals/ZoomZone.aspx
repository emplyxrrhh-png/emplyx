<%@ Page Language="VB" AutoEventWireup="false" EnableEventValidation="false" Inherits="VTLive40.ZoomZone" Culture="auto" UICulture="auto" CodeBehind="ZoomZone.aspx.vb" %>

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Zoom Zone</title>
</head>

<body onload="loadReaderMapFlashX()">

    <script language="javascript" type="text/javascript">
        function loadReaderMapFlashX() {
            try {
                var ImageID = document.getElementById('hdnImageID').value;
                var strParams = document.getElementById('hdnParams').value;
                var FixPoint = document.getElementById('hdnFixPoint').value;

                if (document.getElementById('divReaderMap') != null) {
                    ret = '<object classid="clsid:d27cdb6e-ae6d-11cf-96b8-444553540000" codebase="http://fpdownload.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=9,0,0,0" id="ReaderMap" width="100%" height="100%" align="middle">';
                    ret += '<param name="allowScriptAccess" value="always" />';
                    ret += '<param name="movie" value="fla/ReadersMap.swf" />';
                    ret += '<param name="quality" value="high" />';
                    ret += '<param name="bgcolor" value="#cccccc" />';
                    ret += '<param name="wmode" value="transparent" />';
                    ret += '<embed src="fla/ReadersMap.swf?bgImageLocation=' + ImageID;
                    ret += '" wmode="transparent" quality="high" bgcolor="#cccccc" width="100%" height="100%" swLiveConnect=true id="ReaderMap" name="ReaderMap" align="middle" allowScriptAccess="always" type="application/x-shockwave-flash" pluginspage="http://www.macromedia.com/go/getflashplayer" />';
                    ret += '</object>';
                    document.getElementById('divReaderMap').innerHTML = ret;
                }

                if (FixPoint == "true") {
                    document.getElementById('<%= lblFixReader.ClientID %>').style.display = "";
                document.getElementById('<%= lblZoomReaders.ClientID %>').style.display = "none";
                setTimeout("ReaderMapX_DoFSCommand('REPOSITIONFIX', '" + strParams + "');", 800);
            } else {
                document.getElementById('<%= lblFixReader.ClientID %>').style.display = "none";
                document.getElementById('<%= lblZoomReaders.ClientID %>').style.display = "";
                    setTimeout("ReaderMapX_DoFSCommand('REPOSITION', '" + strParams + "');", 800);
                }

            } catch (e) { showError("loadReaderMapFlashX", e); }
        }

        var isInternetExplorer = navigator.appName.indexOf("Microsoft") != -1;

        function ReaderMapX_DoFSCommand(command, args) {
            try {
                var oResult = "";
                var ReaderMapObj = isInternetExplorer ? document.all.ReaderMap : document.ReaderMap;
                switch (command.toUpperCase()) {
                    case "REPOSITION":
                        try {
                            ReaderMapObj.jsReposition(args);
                        } catch (e) { }
                        break;
                    case "REPOSITIONFIX":
                        try {
                            ReaderMapObj.jsRepositionAndFix(args);
                        } catch (e) { }
                        break;
                    case "RESET":
                        try {
                            ReaderMapObj.jsReset();
                        } catch (e) { }
                        break;
                    case "GETPARMS":
                        try {
                            oResult = ReaderMapObj.jsRetParms(args);
                        } catch (e) { }
                        break;
                }
                return oResult;
            } catch (e) { showError("ReaderMap_DoFSCommand", e); }
        }

        function SavePosition() {
            if (document.getElementById('hdnFixPoint').value == "true") {
                var oParms = document.getElementById('hdnParams_PageBase');
                document.getElementById('hdnMustRefresh_PageBase').value = "1";
                oParms.value = ReaderMapX_DoFSCommand('GETPARMS', 'false');
            }
            Close();
            return false;
        }
    </script>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <input type="hidden" id="hdnParams" value="<%= Request("strParams") %>" />
        <input type="hidden" id="hdnImageID" value="<%= Request("ImageID") %>" />
        <input type="hidden" id="hdnIDReader" value="<%= Request("IDReader") %>" />
        <input type="hidden" id="hdnFixPoint" value="<%= Request("FixPoint") %>" />

        <table width="100%" height="100%" class="bodyPopup defaultContrastColor">
            <tr>
                <td>
                    <table>
                        <tr>
                            <td>
                                <img src="Images/TerminalReaders.png" /></td>
                            <td>
                                <asp:Label ID="lblFixReader" runat="server" Text="Marque en un punto del mapa para posicionar el lector.<br />"></asp:Label>
                                <asp:Label ID="lblZoomReaders" runat="server" Text="Desde esta area puede visualizar los Lectores de acceso del Terminal seleccionado."></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td width="100%" height="100%">
                    <div id="divReaderMap" style="width: 100%; height: 100%;"></div>
                </td>
            </tr>
            <tr>
                <td width="100%" height="100%" align="right">
                    <table>
                        <tr>
                            <td>
                                <asp:Button ID="btOk" Text="${Button.Accept}" runat="server" OnClientClick="return SavePosition();" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                            </td>
                            <td>
                                <asp:Button ID="btCancel" Text="${Button.Cancel}" runat="server" OnClientClick="Close(); return false;" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <asp:HiddenField ID="hdnCanClose_PageBase" runat="server" />
        <asp:HiddenField ID="hdnMustRefresh_PageBase" Value="0" runat="server" />
        <asp:HiddenField ID="hdnParams_PageBase" runat="server" />
    </form>
</body>
</html>