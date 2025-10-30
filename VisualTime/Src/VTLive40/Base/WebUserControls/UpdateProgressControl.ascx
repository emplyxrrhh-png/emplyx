<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.WebUserControls_UdateProgressControl" CodeBehind="UpdateProgressControl.ascx.vb" %>

<asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
    <ProgressTemplate>
        <div id="DivProgressBackground" runat="server" style="position: absolute; left: 0px; top: 0px; height: 100%; width: 100%; background-color: Black; filter: alpha(Opacity= 20); opacity: 0.2; -moz-opacity: 0.2; z-index: 999">
        </div>
        <div id="DivProgress" runat="server" style="position: absolute; left: 50%; top: 50%; margin-left: -150px; margin-top: -100px; z-index: 1000; vertical-align: middle">
            <roWebControls:roPopupFrameV2 ID="RoPopupFrame1" runat="server" Width="300px" Height="100px">
                <FrameContentTemplate>
                    <table style="height: 100px" width="100%">
                        <tr>
                            <td align="right" style="width: 30%">
                                <asp:Image ID="imgLoading" ImageUrl="~/Base/Images/Progress/Loading.gif" runat="server" />
                            </td>
                            <td align="left" style="padding-left: 10px">
                                <asp:Label ID="lblLoading" runat="server" Text="Cargando..." class="UpdateProgressMessage"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </FrameContentTemplate>
            </roWebControls:roPopupFrameV2>
        </div>
    </ProgressTemplate>
</asp:UpdateProgress>