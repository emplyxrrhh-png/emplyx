<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Security_PassportsBusinessCenters" CodeBehind="PassportsBusinessCenters.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Grupos de Negocio asignados</title>
</head>
<body class="bodyPopup" style="background-attachment: fixed;">

    <form id="frmBusinessCenters" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <div class="DetailFrame_TopMid">

            <script type="text/javascript">

                var allSelected = false;
                function PageBase_Load() {
                    ConvertControls();
                }

                function selectComboBusinessCentersVisibility_ClientClick() {
                    if (allSelected == true) {
                        allSelected = false;
                        deselectAllBusinessCenters_ClientClick();
                    }
                    else {
                        allSelected = true;
                        selectAllBusinessCenters_ClientClick();
                    }
                }

                function selectAllBusinessCenters_ClientClick() {
                    var childContainer = document.getElementById("treeBusinessCenters");
                    var childChkBoxes = childContainer.getElementsByTagName("input");
                    var childChkBoxCount = childChkBoxes.length;
                    for (var i = 0; i < childChkBoxCount; i++) {
                        childChkBoxes[i].checked = 'checked';
                    }
                }

                function deselectAllBusinessCenters_ClientClick() {
                    var childContainer = document.getElementById("treeBusinessCenters");
                    var childChkBoxes = childContainer.getElementsByTagName("input");
                    var childChkBoxCount = childChkBoxes.length;
                    for (var i = 0; i < childChkBoxCount; i++) {
                        childChkBoxes[i].checked = '';
                    }
                }
            </script>

            <div style="width: 100%; padding-top: 10px; padding-bottom: 10px;">
                <div class="panHeader2">
                    <span style="">
                        <asp:Label ID="lblTitle" runat="server" Text="Grupos de Negocio"></asp:Label></span>
                </div>
            </div>
            <div style="width: 100%; padding-top: 10px; padding-bottom: 10px;">
                <asp:Label ID="lblInfo" Text="Grupos de Negocio existentes" CssClass="editTextFormat" runat="server" />
            </div>
            <div style="width: 95%; height: 275px; margin-left: auto; margin-right: auto; overflow: auto">
                <table cellpadding="0" cellspacing="0" width="100%" style="height: 100%;">
                    <tr>
                        <td valign="top">
                            <asp:UpdatePanel ID="updBusinessCenters" runat="server">
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="btSave" EventName="Click" />
                                </Triggers>
                                <ContentTemplate>
                                    <asp:HiddenField ID="hdnCanClose_PageBase" runat="server" />
                                    <asp:HiddenField ID="hdnMustRefresh_PageBase" Value="0" runat="server" />
                                    <table>
                                        <tr>
                                            <td>
                                                <img id="readImage" alt="Seleccionado" src="Images/Features/Access_InheritedRead.gif" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:TreeView ID="treeBusinessCenters" ShowCheckBoxes="All" ShowExpandCollapse="false" runat="server"></asp:TreeView>
                                            </td>
                                        </tr>
                                    </table>
                                    <br />
                                    <asp:CheckBox ID="chkCostCenters" onclick="selectComboBusinessCentersVisibility_ClientClick();" AutoPostBack="false" Text="Seleccionar todos los centros de coste. " runat="server" />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                </table>

                <Local:MessageFrame ID="MessageFrame1" runat="server" />
            </div>
            <div>
                <table width="100%" border="0">
                    <tr>
                        <td align="right" style="padding-top: 3px; padding-right: 5px;">
                            <table>
                                <tr>
                                    <td>
                                        <asp:Button ID="btSave" Text="${Button.Accept}" runat="server" OnClientClick="" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                    </td>
                                    <td>
                                        <asp:Button ID="btCancel" Text="${Button.Cancel}" runat="server" OnClientClick="Close(); return false;" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </form>
</body>
</html>