<%@ Page Language="VB" AutoEventWireup="false" EnableEventValidation="false" Inherits="VTLive40.ZoomStatus" Culture="auto" UICulture="auto" CodeBehind="ZoomStatus.aspx.vb" %>

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Zoom Status</title>
</head>

<body>

    <script language="javascript" type="text/javascript">        

        var isInternetExplorer = navigator.appName.indexOf("Microsoft") != -1;

        function StatusMap_DoFSCommand(command, args) {
            try {
                var oResult = "";
                var StatusMapObj = isInternetExplorer ? document.all.StatusMap : document.StatusMap;
                switch (command.toUpperCase()) {
                    case "REPOSITION":
                        try {
                            StatusMapObj.jsReposition(args);
                        } catch (e) { }
                        break;
                    case "RESET":
                        try {
                            StatusMapObj.jsReset();
                        } catch (e) { }
                        break;
                }
                return oResult;
            } catch (e) { showError("StatusMap_DoFSCommand", e); }
        }

        //Redireccionem a la zona seleccionada
        function ShowZone(IdZone) {
            var oParms = document.getElementById('hdnParams_PageBase');
            document.getElementById('hdnMustRefresh_PageBase').value = "1";
            oParms.value = IdZone;
            Close();
            return false;
        }
    </script>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <input type="hidden" id="hdnParams" value="<%= Request("strParams") %>" />
        <input type="hidden" id="hdnImageID" value="<%= Request("ImageID") %>" />

        <table width="100%" height="100%" class="bodyPopup defaultContrastColor">
            <tr>
                <td>
                    <table>
                        <tr>
                            <td>
                                <img src="Images/AccessZones.png" /></td>
                            <td>
                                <asp:Label ID="lblZoom" runat="server" Text="Desde esta area puede visualizar el estado de las zonas de acceso"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td width="100%" height="100%">
                    <div id="divStatusMap" style="width: 100%; height: 100%;"></div>
                </td>
            </tr>
            <tr>
                <td width="100%" height="100%" align="right">
                    <table>
                        <tr>
                            <td>
                                <asp:Button ID="btClose" Text="${Button.Close}" runat="server" OnClientClick="Close(); return false;" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
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