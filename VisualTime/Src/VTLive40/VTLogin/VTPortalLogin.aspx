<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="VTPortalLogin.aspx.vb" Inherits="VTLive40.VTPortalLogin" %>

<!DOCTYPE html>

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>VisualTime Live</title>
    <meta name="robots" content="noindex" />
    <meta name="googlebot" content="noindex" />
    <meta name="robots" content="nofollow" />
    <meta name="robots" content="nosnippet" />
    <meta name="robots" content="noarchive" />

    <link rel="shortcut icon" href="Images/logovtl.ico" />
    <link rel="stylesheet" href="css/anywhere_pc.css" type="text/css" />
    <link rel="stylesheet" href="css/roLiveStyles.min.css" type="text/css" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
</head>
<body>
    <script type="text/javascript">

        function PageBase_Load() {
        }

        function getLoggedInUserInfo() {
            if (document.getElementById('hdnAdfsUserName').value != '' && document.getElementById('hdnAdfsToken').value != '') {
                var returnData = JSON.stringify({ "close": true, "username": document.getElementById('hdnAdfsUserName').value, "token": document.getElementById('hdnAdfsToken').value });
                document.getElementById('hdnAdfsUserName').value = '';
                document.getElementById('hdnAdfsToken').value = '';

                return returnData;
            } else {
                return JSON.stringify({ "close": false, "username": '', "token": '' });
            }
        }
    </script>

    <form id="frmPortalLogin" method="post" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />
        <div style="height: 100%; width: 100%;" class="bg-first">
            <div class="rbPortalBackgroundImg"></div>
            <div class="div-first">
                <div id="btOpen" class="logo-first" runat="server"></div>
            </div>
        </div>
    </form>
    <input type="hidden" runat="server" id="hdnAdfsUserName" />
    <input type="hidden" runat="server" id="hdnAdfsToken" />
</body>
</html>