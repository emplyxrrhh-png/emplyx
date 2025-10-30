<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Scheduler_AssignHolidays" EnableEventValidation="false" Culture="auto" UICulture="auto" EnableSessionState="True" CodeBehind="AssignHolidays.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Configuración vacaciones</title>
</head>
<body class="bodyPopup" style="background-attachment: fixed;">

    <form id="frmAddCoverage" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <script language="javascript" type="text/javascript">

            function PageBase_Load() {
                ConvertControls('<%= divShifts.ClientID %>');
            }

            function KeyPressFunction(e) {
                tecla = (document.all) ? e.keyCode : e.which;
                if (tecla == 13) {
                    RunAccept();
                    return false;
                }
            }

            function RunAccept() {
                //TODO:..
            }
        </script>

        <div>

            <table style="width: 100%; height: 100%" border="0">
                <tr>
                    <td colspan="2" style="padding-top: 5px; padding-bottom: 0px;" valign="top" height="20px">
                        <div class="panHeader2">
                            <span style="">
                                <asp:Label ID="lblTitle" runat="server" Text="Seleccione un horario de vacaciones"></asp:Label>
                            </span>
                        </div>
                    </td>
                </tr>
                <tr style="height: 48px;">
                    <td style="padding: 4px; padding-bottom: 10px; width: 50px;">
                        <img src="Images/holidays_48.png" />
                    </td>
                    <td align="left" valign="middle" style="padding: 4px; padding-bottom: 10px;">
                        <asp:Label ID="lblDescription" runat="server" CssClass="editTextFormat" Text="Seleccione el ${Shift} deseado."></asp:Label>
                    </td>
                </tr>
                <tr style="height: 200px; vertical-align: top">
                    <td colspan="2">

                        <asp:UpdatePanel ID="updShifts" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <Triggers>
                            </Triggers>
                            <ContentTemplate>
                                <div id="divShifts" runat="server">
                                    <asp:Panel ID="panShifts" Height="260" Width="100%" ScrollBars="Vertical" runat="server">
                                        <asp:TreeView ID="treeShifts" ShowCheckBoxes="None" SelectedNodeStyle-ImageUrl="" SelectedNodeStyle-Font-Bold="true" runat="server" Style="background-color: Transparent;"></asp:TreeView>
                                    </asp:Panel>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>

                    <td align="right" colspan="2" style="height: 20px; padding-right: 5px;">
                        <asp:UpdatePanel ID="updActions" runat="server">
                            <ContentTemplate>
                                <table>
                                    <tr>
                                        <td>
                                            <asp:Button ID="btAccept" Text="${Button.Accept}" runat="server" OnClientClick="" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                        </td>
                                        <td>
                                            <asp:Button ID="btCancel" Text="${Button.Cancel}" runat="server" OnClientClick="Close(); return false;" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                        </td>
                                    </tr>
                                </table>
                                <asp:HiddenField ID="hdnCanClose_PageBase" runat="server" />
                                <asp:HiddenField ID="hdnMustRefresh_PageBase" Value="0" runat="server" />
                                <input type="hidden" id="hdnLockedMsg" value="" runat="server" />
                                <input type="hidden" id="hdnLockedEmployee" value="" runat="server" />
                                <input type="hidden" id="hdnLockedDay" value="" runat="server" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>

            <Local:MessageFrame ID="MessageFrame1" runat="server" />
        </div>
    </form>
</body>
</html>