<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Base_WebUserControls_roWizardSelectorContainer" CodeBehind="roWizardSelectorContainer.aspx.vb" %>

<%@ Register Src="~/Base/WebUserControls/roChildSelector.ascx" TagName="roChildSelector" TagPrefix="rws" %>
<%@ Register Src="~/Base/WebUserControls/roTrees/roTrees.ascx" TagName="roTrees" TagPrefix="rws" %>

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>roWizardSelectorContainer</title>
</head>
<body style="height: 100%;" onload="resizeTree();">
    <form id="form1" runat="server" style="height: 100%;">
        <div style="width: 100%;">
            <script language="javascript" type="text/javascript">

                function resizeTree() {

                    // Redimensiona el árbol
                    var ctlPrefix = "<%= roTressWClientID() %>";
                    eval(ctlPrefix + "_resizeTrees();");

                }

                window.onresize = function () {
                    resizeTree();
                }
            </script>

            <rws:roChildSelector ID="roChildSelectorW" FilterFloat="false" runat="server" TreesBehaviorID="roTreesW" PrefixTree="roTreesW">
                <TreeContainer>
                </TreeContainer>
            </rws:roChildSelector>
        </div>
    </form>
</body>
</html>