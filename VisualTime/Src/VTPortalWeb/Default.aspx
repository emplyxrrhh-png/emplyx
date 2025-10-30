<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Default.aspx.vb" Inherits="VTPortalWeb._Default" %>

<!DOCTYPE html>

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Visualtime Portal</title>
    <meta name="robots" content="noindex" />
    <meta name="googlebot" content="noindex" />
    <meta name="robots" content="nofollow" />
    <meta name="robots" content="nosnippet" />
    <meta name="robots" content="noarchive" />

    <link rel="stylesheet" href="<%=Me.ResolveUrl("css/anywhere_pc.css") %>" type="text/css" />
    <link rel="shortcut icon" href="~/2/images/logovtl.ico" />
</head>
<body class="bg-first">
    <form id="form1" runat="server">
        <div style="height: 100%; width: 100%;">
            <div class="div-first">
                <asp:LinkButton ID="btOpen" class="logo-first" runat="server"></asp:LinkButton>
            </div>
        </div>
    </form>
</body>
</html>