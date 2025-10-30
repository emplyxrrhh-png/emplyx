<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.Base_WebUserControls_AllowedApplications" CodeBehind="AllowedApplications.ascx.vb" %>

<input type="hidden" id="hdnType" value="Employee" runat="server" />
<input type="hidden" id="hdnID" value="-1" runat="server" />

<table style="height: 100%; width: 95%; margin-left: auto; margin-right: auto;">
    <tr style="height: 38px">
        <td colspan="2" align="left" valign="middle">
            <asp:Label ID="lblDescription" runat="server" ForeColor="#2D4155" Text="Desde esta página puede indicar a qué aplicaciones tiene acceso el empleado."></asp:Label>
        </td>
    </tr>
    <tr>
        <td style="vertical-align: top;">
            <roUserControls:roGroupBox ID="GroupBox2" runat="server">
                <Content>
                    <table>
                        <tr>
                            <td style="vertical-align: top; padding-top: 20px;">
                                <img alt="" id="Img1" src="~/Base/Images/IdentifyMethods/IdentifyMethods_Username_36.png" runat="server" />
                            </td>
                            <td>
                                <table style="width: 100%; padding-left: 10px;">
                                    <tr>
                                        <td colspan="2" style="height: 40px;">
                                            <asp:Label ID="lblApp" runat="server" Text="Aplicaciones Web:" Font-Bold="True"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="height: 25px; padding-left: 20px;">
                                            <input type="checkbox" runat="server" id="chkVisualTimeDesktop" onchange="hasAllowedApplicationsChanges();" />
                                        </td>
                                        <td>

                                            <asp:Label ID="lblApp1" runat="server" Text="VisualTime Desktop"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="height: 25px; padding-left: 20px;">
                                            <input type="checkbox" runat="server" id="chkVisualTimePortal" onchange="hasAllowedApplicationsChanges();" />
                                        </td>
                                        <td>
                                            <asp:Label ID="lblApp2" runat="server" Text="VisualTime Portal"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="height: 25px; padding-left: 20px;">
                                            <input type="checkbox" runat="server" id="chkVisualTimeVisites" onchange="hasAllowedApplicationsChanges();" />
                                        </td>
                                        <td>
                                            <asp:Label ID="lblApp4" runat="server" Text="VisualTime Visits"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </Content>
            </roUserControls:roGroupBox>
        </td>
        <td style="vertical-align: top;">
            <roUserControls:roGroupBox ID="RoGroupBox1" runat="server">
                <Content>
                    <table>
                        <tr>
                            <td style="vertical-align: top; padding-top: 20px;">
                                <img alt="" id="Img3" src="~/Base/Images/IdentifyMethods/IdentifyMethods_Username_36.png" runat="server" />
                            </td>
                            <td>
                                <table style="width: 100%; padding-left: 10px;">
                                    <tr>
                                        <td colspan="2" style="height: 40px;">
                                            <asp:Label ID="Label1" runat="server" Text="Aplicaciones Dispositivos Móviles:" Font-Bold="True"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="height: 25px; padding-left: 20px;">
                                            <input type="checkbox" runat="server" id="chkVisualTimePortalApp" onchange="hasAllowedApplicationsChanges();" />
                                        </td>
                                        <td>
                                            <asp:Label ID="lblMov1" runat="server" Text="VisualTime Portal App"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </Content>
            </roUserControls:roGroupBox>
        </td>
    </tr>

    <tr style="height: 100px;">
        <td colspan="2">
            <roUserControls:roGroupBox ID="RoGroupBox3" runat="server">
                <Content>
                    <table>
                        <tr>
                            <td style="vertical-align: top; padding-top: 20px;">
                                <img alt="" id="Img4" src="~/Base/Images/IdentifyMethods/IdentifyMethods_Username_36.png" runat="server" />
                            </td>
                            <td>
                                <table style="width: 100%; padding-left: 10px;">
                                    <tr>
                                        <td colspan="2" style="height: 40px;">
                                            <asp:Label ID="noContractTitle" runat="server" Text="Acceso a VTPortal sin contrato en vigor" Font-Bold="True"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="height: 25px; padding-left: 20px;">
                                            <input type="checkbox" runat="server" id="chkNoContract" onchange="hasAllowedApplicationsChanges();" />
                                        </td>
                                        <td>
                                            <asp:Label ID="noContractDesc" runat="server" Text="El usuario puede acceder a VTPortal en modo consulta en caso de tener contrato a futuro"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </Content>
            </roUserControls:roGroupBox>
        </td>
    </tr>
    <tr style="height: 100px;">
        <td colspan="2">
            <roUserControls:roGroupBox ID="RoGroupBox2" runat="server">
                <Content>
                    <table>
                        <tr>
                            <td style="vertical-align: top; padding-top: 20px;">
                                <img alt="" id="Img2" src="~/Base/Images/IdentifyMethods/IdentifyMethods_Properties.png" runat="server" />
                            </td>
                            <td>
                                <table style="width: 100%; padding-left: 10px;">
                                    <tr>
                                        <td colspan="2" style="height: 40px;">
                                            <asp:Label ID="lblAppProperties" runat="server" Text="Propiedades:" Font-Bold="True"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="height: 25px; padding-left: 20px;">
                                            <input type="checkbox" runat="server" id="ckPhotoEnabled" onchange="hasAllowedApplicationsChanges();" />
                                        </td>
                                        <td>
                                            <asp:Label ID="lblPhotoEnabled" runat="server" Text="Requerir fichaje con foto"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="height: 25px; padding-left: 20px;">
                                            <input type="checkbox" runat="server" id="ckLocationEnabled" onchange="hasAllowedApplicationsChanges();" />
                                        </td>
                                        <td>
                                            <asp:Label ID="lblLocationEnabled" runat="server" Text="Requerir fichaje con geolocalización"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </Content>
            </roUserControls:roGroupBox>
        </td>
    </tr>
</table>