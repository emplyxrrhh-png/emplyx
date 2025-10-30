<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.BlankPage" CodeBehind="BlankPage.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="robots" content="noindex" />
    <meta name="googlebot" content="noindex" />
    <meta name="robots" content="nofollow" />
    <meta name="robots" content="nosnippet" />
    <meta name="robots" content="noarchive" />

    <title>Blank Page</title>
</head>
<body class="bodyPopup" style="width: 100%; height: 100%; border: 0;">
    <script type="text/javascript" language="javascript">

        window.onload = pageLoad;

        function pageLoad(sender, args) {
            showLoader();
        }

        function showLoader() {
            LoadingBlankPanelClient.Show();
        }
    </script>
    <form id="form1" runat="server" style="width: 100%; height: 98%; border: 0;">
        <div id="divMain" style="width: 100%; height: 98%; border: 0;">
        </div>
    </form>

    <dx:ASPxLoadingPanel ID="LoadingBlankPanel" runat="server" ClientInstanceName="LoadingBlankPanelClient" ImageSpacing="10" Modal="True" CssClass="LoadingDiv" Font-Size="1em">
        <Image Url="Images/Loaders/loader_v5.gif" Width="48" />
        <LoadingDivStyle Opacity="30" BackColor="Gray" />
    </dx:ASPxLoadingPanel>
</body>
</html>