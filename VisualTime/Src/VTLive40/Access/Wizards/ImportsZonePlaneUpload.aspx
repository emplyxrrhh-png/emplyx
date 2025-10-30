<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.ImportsZonePlaneUpload" CodeBehind="ImportsZonePlaneUpload.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>ImportsZonePlaneUpload.aspx</title>
</head>
<body>
    <form id="form1" runat="server">

        <script language="javascript" type="text/javascript">
            function SendFiles() {
                try {
                    var IDPlane = parent.retIDPlane();
                    document.getElementById('hdnIDPlane').value = IDPlane;
                    //if (IDPlane != "" && IDPlane != "-1") {
                    //espera     __doPostBack('btnSend', '');
                    //}
                    return true;
                }
                catch (e) {
                    return false;
                }
            }
        </script>

        <table width="100%" style="margin: 10px;">
            <tr>
                <td style="padding: 2px;">
                    <asp:Label ID="lblFileOrig" runat="server" Text="Imagen"></asp:Label></td>
                <td style="padding: 2px;">
                    <asp:FileUpload ID="fileOrig" runat="server" BackColor="#F0F0F0" BorderColor="#CDCDCD" BorderStyle="Dotted" BorderWidth="1" />
                </td>
                <td>
                    <asp:Button ID="btnSend" runat="server" Text="Enviar Imagen" BorderStyle="Dotted" BorderWidth="1" BorderColor="#CDCDCD" BackColor="#F0F0F0" OnClientClick="return SendFiles();" />
                </td>
            </tr>
            <tr>
                <td colspan="3" align="left">
                    <asp:Label ID="lblMsg" runat="server" CssClass="errorText" Visible="false"></asp:Label></td>
            </tr>
        </table>
        <input type="hidden" runat="server" id="hdnIDPlane" />
    </form>
</body>
</html>