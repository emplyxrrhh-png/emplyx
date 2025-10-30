<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Recover" CodeBehind="Recover.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>VisualTime Live</title>
    <link rel="shortcut icon" href="~/Base/Images/logovtl.ico" />
</head>
<body onload="loadpage()">

    <script type="text/javascript" language="javascript">
        function loadpage() {
            $(".boxOptions1").dxBox({
                direction: "row",
                width: "100%",
                height: 75
            });

            $("#boxOptions2").dxBox({
                direction: "row",
                width: "100%",
                height: 75,
                align: "center",
                crossAlign: "center"
            });
        }
    </script>

    <form id="frmRecover" method="post" runat="server">
        <div class="MainToolbarPad">
            <div id="tbToolbar" class="Toolbar">
                <div class="tbd_logo" style="cursor: pointer;">
                </div>
                <div id="tdLogoBar" class="tbd_bar">
                    <div id="vtLogoTextDiv" class="tbd_bar_text" style="cursor: pointer">
                        <div id="vtLogoVersionDiv" class="tbd_bar_textVersion">
                            <span class="notranslate">VisualTime R5</span>
                        </div>
                        <div id="vtLogoExpressDiv" class="tbd_bar_textExpress">
                            <div style="float: left">
                                <span class="notranslate">Live</span>
                            </div>
                        </div>
                    </div>
                </div>
                <div id="menu_icons">
                </div>
            </div>
        </div>

        <div class="boxOptions1">
            <div style="background-color: grey" data-options="dxItem: {ratio: 1}">
                <div id="boxOptions2">
                    <div style="background-color: yellowgreen" data-options="dxItem: {ratio: 1, baseSize: '300'}">
                        hola a todos
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>