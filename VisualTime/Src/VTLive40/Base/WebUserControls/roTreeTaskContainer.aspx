<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Base_WebUserControls_roTreeTaskContainer" CodeBehind="roTreeTaskContainer.aspx.vb" %>

<%@ Register Src="~/Base/WebUserControls/roTreeTask.ascx" TagName="roTreeTask" TagPrefix="rws" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>roTreeTaskContainer</title>
</head>
<body>
    <form id="form1" runat="server">
        <div style="border: thin solid silver; height: 97%; width: 98%; padding: 3px;" class="defaultBackgroundColor">
            <rws:roTreeTask ID="oTreeTask" runat="server" />
        </div>
    </form>
</body>
</html>