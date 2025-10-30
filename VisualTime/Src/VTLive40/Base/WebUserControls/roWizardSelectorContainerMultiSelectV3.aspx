<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Base_WebUserControls_roWizardSelectorContainerMultiSelectV3" CodeBehind="roWizardSelectorContainerMultiSelectV3.aspx.vb" %>

<%@ Register Src="~/Base/WebUserControls/roTreeV3.ascx" TagName="roTreeV3" TagPrefix="rws" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>roWizardSelectorContainerMultiSelectV3</title>
</head>
<body>
    <form id="form1" runat="server" style="height: 100%;">
        <div style="height: 97%; width: 98%; padding: 3px;" class="defaultBackgroundColor">
            <rws:roTreeV3 ID="objContainerTreeV3" runat="server" />
        </div>
    </form>
</body>
</html>