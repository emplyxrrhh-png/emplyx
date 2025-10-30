<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Base_ErrorPage" CodeBehind="ErrorPage.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="robots" content="noindex" />
    <meta name="googlebot" content="noindex" />
    <meta name="robots" content="nofollow" />
    <meta name="robots" content="nosnippet" />
    <meta name="robots" content="noarchive" />

    <title>Página de error</title>
</head>
<body>

    <div id="divErrorPage" style="width: 100%; height: 99%;">

        <form id="frmErrorPage" runat="server">
            <div>

                <table cellpadding="0" cellspacing="0" width="100%" style="height: 96%">
                    <tr>
                        <td>
                            <asp:Label ID="lblTitle" Text="Error" Font-Bold="true" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblMessage" Text="" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblDescription" Text="" runat="server"></asp:Label>
                        </td>
                    </tr>
                </table>
            </div>
        </form>
    </div>
</body>
</html>