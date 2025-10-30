<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Forms_EmergencyReports" CodeBehind="EmergencyReports.aspx.vb" %>

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Informes de emergencia</title>
</head>
<body class="bodyPopup" style="background-attachment: fixed;">

    <form id="form1" runat="server">

        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <script lang="javascript" type="text/javascript">

            function PageBase_Load() {
            }
        </script>

        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="chkAll" EventName="CheckedChanged" />
            </Triggers>
            <ContentTemplate>
                <div style="width: 450px; height: 400px; padding-top: 0px;" class="DetailFrame_TopMid">
                    <table cellpadding="1" cellspacing="1" border="0">
                        <tr>
                            <td>
                                <div class="panHeader2">
                                    <asp:Label ID="lblTitle" Text="Informes de emergencia" Font-Bold="true" runat="server" CssClass="panHeaderLabel" />
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td style="padding: 5px;">
                                <asp:Label ID="lblInfo" runat="server" CssClass="editTextFormat"
                                    Text="Seleccione los informes planificados a lanzar"></asp:Label>
                            </td>
                        </tr>

                        <tr style="padding-top: 10px">
                            <td>
                                <asp:Panel ID="panelCheckReports" runat="server" Style="display: block; padding: 2px; padding-left: 4px; border: solid 1px silver; width: 440px; height: 180px; overflow: auto;">
                                    <asp:CheckBoxList ID="chkList" runat="server">
                                    </asp:CheckBoxList>
                                </asp:Panel>
                            </td>
                        </tr>
                        <tr>
                            <td style="padding-top: 5px; padding-left: 10px;">
                                <asp:CheckBox ID="chkAll" runat="server" Text="Seleccionar todos los informes" AutoPostBack="True" />
                            </td>
                        </tr>
                        <tr style="padding-top: 10px">
                            <td style="padding-top: 20px">
                                <div class="panHeader2">
                                    <asp:Label ID="lblStatusLastReport" Text="Estado último informe lanzado" Font-Bold="true" runat="server" CssClass="panHeaderLabel" />
                                </div>
                            </td>
                        </tr>
                        <tr style="padding-top: 10px">
                            <td>
                                <asp:Panel ID="panelStatusLastEmergencyReportExecuted" runat="server" Style="display: block; padding: 2px; padding-left: 4px; border: solid 1px silver; width: 440px; height: 60px; overflow: auto;">
                                    <asp:Label ID="lblLastEmergencyReportExecuted" runat="server" Text=""></asp:Label>
                                </asp:Panel>
                            </td>
                        </tr>
                        <tr>
                            <td style="padding: 5px;">
                                <asp:Label ID="msgSelectOne" runat="server" Visible="false"
                                    Text="Por favor, seleccione al menos un informe a ejecutar."></asp:Label>
                            </td>
                        </tr>
                        <tr style="height: 40px">
                            <td align="right" style="padding-right: 5px; padding-top: 20px;">
                                <table>
                                    <tr>
                                        <td>
                                            <asp:Button ID="btOK" Text="${Button.Accept}" runat="server" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                        </td>
                                        <td>
                                            <asp:Button ID="btCancel" Text="${Button.Cancel}" runat="server" OnClientClick="Close(); return false;" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                        </td>
                                    </tr>
                                </table>
                                <asp:HiddenField ID="hdnCanClose_PageBase" runat="server" />
                                <asp:HiddenField ID="hdnMustRefresh_PageBase" Value="0" runat="server" />
                            </td>
                        </tr>
                    </table>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>

        <Local:MessageFrame ID="MessageFrame1" runat="server" />
    </form>
</body>
</html>