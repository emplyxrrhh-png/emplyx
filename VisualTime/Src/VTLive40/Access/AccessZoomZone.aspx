<%@ Page Language="VB" AutoEventWireup="false" EnableEventValidation="false" Inherits="VTLive40.AccessZoomZone" Culture="auto" UICulture="auto" CodeBehind="AccessZoomZone.aspx.vb" %>

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Zoom Zone</title>
</head>

<body onload="loadLocation()">

    <script language="javascript" type="text/javascript">

        function loadLocation() {
            if (document.getElementById('hdnGlobal').value == "0") {
                loadLocationMapFlash();
            } else {
                loadLocationGlobalMapFlash();
            }
        }

        function loadLocationMapFlash() {
            var ImageID = document.getElementById('hdnImageID').value;
            var strParams = document.getElementById('hdnParams').value;
            var FixPoint = document.getElementById('hdnFixPoints').value;

            var oParams = new Array();
            oParams = strParams.split(",");
            var oCount = Math.round(oParams.length / 6);

            var strParm = "";

            if (oParams.length > 6) {
                for (var xy = 0; xy < oCount; xy++) {
                    var oIdx = xy * 6;
                    strParm += oParams[oIdx] + ',' + oParams[oIdx + 1] + ',' + oParams[oIdx + 2] + ',' + oParams[oIdx + 3] + ',' + FixPoint + ',' + oParams[oIdx + 5] + ',';
                } //end for
                strParm = strParm.substring(0, strParm.length - 1);
            } else {
                strParm = oParams[0] + ',' + oParams[1] + ',' + oParams[2] + ',' + oParams[3] + ',' + FixPoint + ',' + oParams[5];
            } //end if

            if (document.getElementById('divLocationMap') != null) {
                ret = '<object classid="clsid:d27cdb6e-ae6d-11cf-96b8-444553540000" codebase="http://fpdownload.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=9,0,0,0" id="LocationMap" width="100%" height="100%" align="middle">';
                ret += '<param name="allowScriptAccess" value="always" />';
                ret += '<param name="movie" value="fla/LocationMap.swf" />';
                ret += '<param name="quality" value="high" />';
                ret += '<param name="bgcolor" value="#cccccc" />';
                ret += '<param name="wmode" value="transparent" />';
                ret += '<embed src="fla/LocationMap.swf?bgImageLocation=' + ImageID;
                if (strParams != "") {
                    ret += '&strParamZones=' + strParm;
                }
                ret += '" wmode="transparent" quality="high" bgcolor="#cccccc" width="100%" height="100%" swLiveConnect=true id="LocationMap" name="LocationMap" align="middle" allowScriptAccess="always" type="application/x-shockwave-flash" pluginspage="http://www.macromedia.com/go/getflashplayer" />';
                ret += '</object>';
                document.getElementById('divLocationMap').innerHTML = ret;
            }
            if (FixPoint == "true") {
                document.getElementById('<%= lblFixPoints.ClientID %>').style.display = "";
                document.getElementById('<%= lblZoom.ClientID %>').style.display = "none";
            } else {
                document.getElementById('<%= lblFixPoints.ClientID %>').style.display = "none";
                document.getElementById('<%= lblZoom.ClientID %>').style.display = "";
            }
        }

        function loadLocationGlobalMapFlash() {
            var ImageID = document.getElementById('hdnImageID').value;
            var strParams = document.getElementById('hdnParams').value;
            var FixPoint = document.getElementById('hdnFixPoints').value;

            var oParams = new Array();
            oParams = strParams.split(",");
            var oCount = Math.round(oParams.length / 6);

            if (document.getElementById('divLocationMap') != null) {
                ret = '<object classid="clsid:d27cdb6e-ae6d-11cf-96b8-444553540000" codebase="http://fpdownload.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=9,0,0,0" id="LocationMap" width="100%" height="100%" align="middle">';
                ret += '<param name="allowScriptAccess" value="always" />';
                ret += '<param name="movie" value="fla/LocGlobalMap.swf" />';
                ret += '<param name="quality" value="high" />';
                ret += '<param name="bgcolor" value="#cccccc" />';
                ret += '<param name="wmode" value="transparent" />';
                ret += '<embed src="fla/LocGlobalMap.swf?bgImageLocation=' + ImageID;
                if (strParams != "") {
                    ret += '&strParamZones=' + strParams;
                }
                ret += '" wmode="transparent" quality="high" bgcolor="#cccccc" width="100%" height="100%" swLiveConnect=true id="LocationMap" name="LocationMap" align="middle" allowScriptAccess="always" type="application/x-shockwave-flash" pluginspage="http://www.macromedia.com/go/getflashplayer" />';
                ret += '</object>';
                document.getElementById('divLocationMap').innerHTML = ret;
            }
            if (FixPoint == "true") {
                document.getElementById('<%= lblFixPoints.ClientID %>').style.display = "";
            document.getElementById('<%= lblZoom.ClientID %>').style.display = "none";
        } else {
            document.getElementById('<%= lblFixPoints.ClientID %>').style.display = "none";
            document.getElementById('<%= lblZoom.ClientID %>').style.display = "";
            }
        }

        var isInternetExplorer = navigator.appName.indexOf("Microsoft") != -1;

        function LocationMap_DoFSCommand(command, args) {
            try {
                var oResult = "";
                var LocationMapObj = isInternetExplorer ? document.all.LocationMap : document.LocationMap;
                switch (command.toUpperCase()) {
                    case "REPOSITION":
                        try {
                            LocationMapObj.jsReposition(args);
                        } catch (e) { }
                        break;
                    case "RESET":
                        try {
                            LocationMapObj.jsReset();
                        } catch (e) { }
                        break;
                    case "GETPARMS":
                        try {
                            oResult = LocationMapObj.jsRetParms(args);
                        } catch (e) { }
                        break;
                }
                return oResult;
            } catch (e) { showError("LocationMap_DoFSCommand", e); }
        }

        function SavePosition() {
            var oParms = document.getElementById('hdnParams_PageBase');
            document.getElementById('hdnMustRefresh_PageBase').value = "1";
            oParms.value = LocationMap_DoFSCommand('GETPARMS', 'false');
            Close();
            return false;
        }

        function ShowZone(IDZone) {
            var oParms = document.getElementById('hdnParams_PageBase');
            document.getElementById('hdnMustRefresh_PageBase').value = "1";
            oParms.value = IDZone;
            Close();
            return false;
        }
    </script>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <input type="hidden" id="hdnParams" value="<%= Request("strParams") %>" />
        <input type="hidden" id="hdnImageID" value="<%= Request("ImageID") %>" />
        <input type="hidden" id="hdnFixPoints" value="<%= Request("FixPoints") %>" />
        <input type="hidden" id="hdnGlobal" value="<%= Request("Global") %>" />

        <table width="100%" height="100%" class="bodyPopup defaultContrastColor">
            <tr>
                <td>
                    <table>
                        <tr>
                            <td>
                                <img src="Images/AccessZones.png" /></td>
                            <td>
                                <asp:Label ID="lblFixPoints" runat="server" Text="Marque dos puntos en el mapa para generar una zona.<br /> Una vez creada, puede arrastrarla seleccionando uno de los dos selectores."></asp:Label>
                                <asp:Label ID="lblZoom" runat="server" Text="Desde esta area puede visualizar las Zonas de acceso"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td width="100%" height="100%">
                    <div id="divLocationMap" style="width: 100%; height: 100%;"></div>
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