<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Ooops.aspx.vb" Inherits="VTLive40.Ooops" %>

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
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <link href="styles/roLiveStyles.min.css" rel="stylesheet" />
    <style>
        .PopUpFrame_Center {
            background-color: transparent !important;
        }
    </style>
</head>
<body>
    <form id="frmLogin" method="post" runat="server">
        <div class="loginFull">
            <div class="rbBackgroundImg" id="rbBackground" runat="server" style="background-image: url(/Base/Images/LoginBackground/img-form_C.png)"></div>
            <div class="rbLoginColumn">
                <div class="rbLogoInformation LogoLogin">
                    <div id="vtLogoVersionDiv" class="tbd_bar_textVersion" translate="no">
                        <asp:Image ID="titleVT" runat="server" ImageUrl="~/Base/Images/Logo_VT5.png" Width="75%" Height="75%" />
                    </div>
                </div>
                <div class="rbLoginData">
                    <table id="table" align="center" width="300" class="PopUpFrame_Center" border="0" cellpadding="2" cellspacing="1">
                        <tr id="errorRow" runat="server">
                            <td colspan="2" align="center" style="padding-left: 25px; padding-top: 5px; padding-bottom: 5px;">
                                <asp:Label ID="lblResult" runat="server" Font-Bold="true" CssClass="errorText" Text="Se ha producido un error, inténtalo más tarde"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </div>
                <div class="rbTextClass">
                    <div style="height: 50px">
                        <a href="https://www.cegid.com/ib/es/productos/cegid-visualtime/">© 2025 Visualtime</a>&nbsp;|&nbsp; <a href="https://www.cegid.com/ib/es/asistencia-al-cliente/cegid-visualtime/">Soporte técnico</a>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>