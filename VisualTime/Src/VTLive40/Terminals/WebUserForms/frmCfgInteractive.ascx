<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.frmCfgInteractive" CodeBehind="frmCfgInteractive.ascx.vb" %>

<!-- Div flotant cfgInteractive -->
<input type="hidden" id="<%= Me.ClientID %>_hdnCfgInteractiveIDReader" value="<%= Me.IDReader %>" />
<div id="<%= Me.ClientID %>_frm" style="position: fixed; *position: absolute; z-index: 15010; display: none; top: 50%; left: 50%; *width: 550px;">
    <div id="<%= Me.ClientID %>_BgS" style="position: absolute; top: 0; left: 0; display: none; z-index: 15009;"></div>
    <div class="bodyPopupExtended" style="">
        <div style="">
            <div style="width: 100%; height: 100%; background-color: White;" class="bodyPopup">
                <table style="width: 100%; padding-top: 5px;" border="0">
                    <tr>
                        <td colspan="2">
                            <div class="panHeader2">
                                <span style="">
                                    <asp:Label runat="server" ID="lblAddCfgInteractive" Text="Configuración"></asp:Label></span>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" style="padding: 2px; padding-top: 10px;">
                            <roUserControls:roTabContainerClient ID="tabCtl" runat="server">
                                <TabTitle1>
                                    <asp:Label ID="lblRepeatedPunches" runat="server" Text="Fichajes repetidos"></asp:Label>
                                </TabTitle1>
                                <TabContainer1>
                                    <table border="0" style="width: 100%;">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblRepeatedPunchesDesc" runat="server" CssClass="spanEmp-class" Text="Desde este formulario configuraremos los tiempos de detección entre una entrada i una salida, así como los fichajes repetidos."></asp:Label></td>
                                        </tr>
                                        <tr>
                                            <td style="padding: 5px; padding-left: 50px;">
                                                <table>
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lblMinimumESTime" runat="server" CssClass="spanEmp-Class" Text="El tiempo mínimo permitido entre una entrada y una salida es de:"></asp:Label></td>
                                                        <td>
                                                            <input type="text" id="<%= Me.ClientID %>_txtMinimESTime" cctime="true" convertcontrol="TextField" style="width: 50px; text-align: right;" class="textClass x-form-text x-form-field" maxlength="5" ccallowblank="false" /></td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lblMinimumSETime" runat="server" CssClass="spanEmp-Class" Text="El tiempo mínimo permitido entre una salida y una entrada es de:"></asp:Label></td>
                                                        <td>
                                                            <input type="text" id="<%= Me.ClientID %>_txtMinimSETime" cctime="true" convertcontrol="TextField" style="width: 50px; text-align: right;" class="textClass x-form-text x-form-field" maxlength="5" ccallowblank="false" /></td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </TabContainer1>
                                <TabTitle2>
                                    <asp:Label ID="lblEIP" runat="server" Text="Portal del empleado"></asp:Label>
                                </TabTitle2>
                                <TabContainer2>
                                    <table border="0" style="width: 100%;">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblEIPDesc" runat="server" CssClass="spanEmp-class" Text="Desde este formulario configuraremos los permisos para el Portal del empleado."></asp:Label></td>
                                        </tr>
                                        <tr>
                                            <td style="padding: 5px; padding-left: 50px;">
                                                <table>
                                                    <tr>
                                                        <td style="height: 20px;">
                                                            <input id="chkPermitQrys" type="checkbox" runat="server" /></td>
                                                        <td>
                                                            <a href="javascript: void(0);" onclick="CheckLinkClick('<%= chkPermitQrys.ClientID %>');">
                                                                <asp:Label ID="lblPermitQuerys" runat="server" Text="Permitir consultas"></asp:Label>
                                                            </a>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="height: 20px;">
                                                            <input id="chkPermitSol" type="checkbox" runat="server" /></td>
                                                        <td>
                                                            <a href="javascript: void(0);" onclick="CheckLinkClick('<%= chkPermitSol.ClientID %>');">
                                                                <asp:Label ID="lblPermitSol" runat="server" Text="Permitir solicitudes"></asp:Label>
                                                            </a>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </TabContainer2>
                            </roUserControls:roTabContainerClient>
                        </td>
                    </tr>
                </table>
                <table border="0" style="width: 100%;">
                    <tr>
                        <td>&nbsp;</td>
                        <td style="width: 100px;" align="right">
                            <asp:Button ID="btnOk" Text="Aceptar" runat="server" OnClientClick="" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                        </td>
                        <td style="width: 110px;" align="left">
                            <asp:Button ID="btnCancel" Text="Cancelar" runat="server" OnClientClick="" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
</div>
<!-- End Div flotant CfgInteractive -->