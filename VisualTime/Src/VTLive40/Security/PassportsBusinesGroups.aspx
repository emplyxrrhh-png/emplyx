<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Security_PassportsBusinesGroups" CodeBehind="PassportsBusinesGroups.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Grupos de Negocio asignados</title>
</head>
<body class="bodyPopup" style="background-attachment: fixed;">

    <form id="frmBusinessGroup" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <div class="DetailFrame_TopMid">

            <script type="text/javascript">

                function PageBase_Load() {
                    ConvertControls();

                }

            //function SaveBusinessGroups() {
            //    return true;
            //}
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
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td valign="top">
                            <asp:UpdatePanel ID="updBusinessGroups" runat="server">
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="btSave" EventName="Click" />
                                </Triggers>
                                <ContentTemplate>
                                    <asp:HiddenField ID="hdnCanClose_PageBase" runat="server" />
                                    <asp:HiddenField ID="hdnMustRefresh_PageBase" Value="0" runat="server" />
                                    <asp:TreeView ID="treeConcepts" ShowCheckBoxes="All" ShowExpandCollapse="false" runat="server"></asp:TreeView>
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