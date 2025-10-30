<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Base_AccessDenied" Culture="auto" UICulture="auto" CodeBehind="AccessDenied.aspx.vb" %>

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="robots" content="noindex" />
    <meta name="googlebot" content="noindex" />
    <meta name="robots" content="nofollow" />
    <meta name="robots" content="nosnippet" />
    <meta name="robots" content="noarchive" />

    <title>Acceso denegado</title>

    <script language="javascript" type="text/javascript">
        function hidePreloader() {
            try {
                parent.showLoader(false);
            } catch (e) { }
        }
    </script>
</head>
<body class="bodyPopup" style="background-attachment: fixed;" onload="hidePreloader();">
    <form id="frmAccessDenied" runat="server">
        <div style="width: 100%; height: 100%; vertical-align: top;">

            <table width="100%">
                <tr>
                    <td align="left" valign="top">

                        <asp:Label ID="lblTitle" Text="Acceso denegado" runat="server" CssClass="errorText" Font-Size="Large"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td align="right" valign="bottom" style="height: 100%;">
                        <asp:Button ID="btClose" Text="${Button.Close}" runat="server" CssClass="btnFlat btnFlatBlack btnFlatAsp" TabIndex="5" OnClientClick="Close(); return false;" />
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>