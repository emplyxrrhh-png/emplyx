<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="VTLiveAuth.aspx.vb" Inherits="VTLive40.VTLiveAuth" %>

<!DOCTYPE html>

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>VisualTime Live</title>
    <meta name="robots" content="noindex" />
    <meta name="googlebot" content="noindex" />
    <meta name="robots" content="nofollow" />
    <meta name="robots" content="nosnippet" />
    <meta name="robots" content="noarchive" />

    <link rel="shortcut icon" href="~/Base/Images/logovtl.ico" />
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

    <input type="hidden" runat="server" id="hdnAdfsUserName" />
    <input type="hidden" runat="server" id="hdnAdfsToken" />
    <form id="frmLogin" method="post" runat="server">
        <div class="loginFull">
            <div class="rbBackgroundImg" id="rbBackground" runat="server"></div>
            <div class="rbLoginColumn">
                <div class="rbLogoInformation LogoLogin">
                    <div id="vtLogoVersionDiv" class="tbd_bar_textVersion" translate="no">
                        <asp:Image ID="titleVT" runat="server" ImageUrl="~/Base/Images/Logo_VT5.png" Width="75%" Height="75%" />
                    </div>
                </div>
                <div class="rbLoginData">
                    <table id="table" align="center" width="300" style="background-color: #f3f6ff !important" class="PopUpFrame_Center" border="0" cellpadding="2" cellspacing="1">
                        <tr id="trUserNameDesc" runat="server" class="LogoLogin">
                            <td colspan="2" align="center" style="padding-left: 25px; padding-top: 5px; padding-bottom: 25px; color: black" class="tbd_bar_textVersion">
                                <asp:Label ID="lblUserName" runat="server" Text="Bienvenido "></asp:Label>
                            </td>
                        </tr>
                        <tr id="errorRow" runat="server">
                            <td colspan="2" align="center" style="padding-left: 25px; padding-top: 5px; padding-bottom: 5px;">
                                <div>
                                    <asp:Label ID="lblResult" runat="server" Font-Bold="true" CssClass="errorText"></asp:Label>
                                </div>
                                <div style="padding-top: 25px; width: 100%">
                                    <div id="acceptButton" runat="server" style="margin: auto">
                                        <asp:Button ID="btReturnToVT" Text="Volver a VisualTime" runat="server" TabIndex="3" Style="background-color: #FF5C35 !important; border: 0px !important" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                    </div>
                                </div>
                            </td>
                        </tr>
                    </table>
                </div>
                <div class="rbTextClass">
                    <div style="height: 50px">
                        <a href="https://www.cegid.com/ib/es/productos/cegid-visualtime/">©2025 Visualtime</a>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>