<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Requests_RequestApprovals" CodeBehind="RequestApprovals.aspx.vb" %>

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Histórico aprobaciones solicitud</title>
</head>
<body class="bodyPopup" style="background-attachment: fixed;">

    <form id="frmRequestApprovals" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <div>

            <script lang="javascript" type="text/javascript">
                function PageBase_Load() {

                    ConvertControls();

                    getApprovalsGrid(getUrlParameter('IDRequest'));

                }
            </script>

            <table style="width: 100%; height: 100%" border="0">
                <tr>
                    <td colspan="2" style="padding-top: 5px; padding-bottom: 0px;" valign="top" height="20px">
                        <div class="panHeader2">
                            <span style="">
                                <asp:Label ID="lblTitle" runat="server" Text="Histórico aprobaciones solicitud"></asp:Label></span>
                        </div>
                    </td>
                </tr>
                <tr style="height: 48px">
                    <td style="padding: 4px; padding-bottom: 10px;">
                        <img id="Img1" src="" runat="server" />
                    </td>
                    <td align="left" valign="middle" style="padding: 4px; padding-bottom: 10px;">
                        <asp:Label ID="lblDescription" runat="server" CssClass="editTextFormat" Text="Puede consultar el histórico de aprobaciones de la solicitud."></asp:Label>
                    </td>
                </tr>
                <tr style="vertical-align: top;">
                    <td colspan="2">
                        <div id="divRequestApprovals" style="height: 270px; overflow: auto; width: 685px;" runat="server">
                        </div>
                    </td>
                </tr>
                <tr>

                    <td align="right" valign="bottom" colspan="2" style="height: 20px; padding-right: 5px;">
                        <asp:UpdatePanel ID="updActions" runat="server">
                            <ContentTemplate>
                                <table>
                                    <tr>
                                        <td>
                                            <asp:Button ID="btClose" Text="${Button.Close}" runat="server" OnClientClick="Close(); return false;" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </div>

        <Local:MessageFrame ID="MessageFrame1" runat="server" />
    </form>
</body>
</html>