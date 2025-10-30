<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.LoginRedirect" CodeBehind="LoginRedirect.aspx.vb" %>

<%@ OutputCache Duration="1" NoStore="true" VaryByParam="none" %>
<!-- LOGINREDIRECT (NO QUITAR ESTA LINEA!) -->
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Redirect Login</title>

    <script type="text/javascript" language="javascript">
        var returnUrl = null;
        // Check wether LoginRedirect is displayed within a frame.
        if (window == window.top) {
            // Get ReturnUrl from query string.
            var args = getArgs(); // From Scripts/QueryStrings.js
            returnUrl = args["ReturnUrl"];
        } else {
            // When redirect occurs within a frame, ReturnUrl should be
            // the top window's location.
            returnUrl = window.top.location.toString();
        }

        // Ensure RedirectUrl is not one of the login pages.
        var returnUrlFile = returnUrl;
        if (returnUrl != null && returnUrl != "") {
            var Pos = returnUrl.lastIndexOf("/");
            if (Pos > -1)
                returnUrlFile = returnUrl.slice(Pos + 1).toLowerCase();
            if (returnUrlFile == "loginwin.aspx" || returnUrlFile == "loginselect.aspx" || returnUrlFile == "loginredirect.aspx")
                returnUrl = "loginweb.aspx";
        }
        else
            returnUrl = "loginweb.aspx";

        // Store ReturnUrl into a cookie so it can be retrieved after login.
        createCookie("ReturnUrl", returnUrl, null);

        // Redirect to login page.
        window.top.location = "LoginWin.aspx?TryOnly=1";
    </script>
</head>
<body>
</body>
</html>