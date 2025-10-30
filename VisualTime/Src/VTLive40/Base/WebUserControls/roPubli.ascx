<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.WebUserControls_roPubli" CodeBehind="roPubli.ascx.vb" %>
<table style="width: 770px;" border="0" cellpadding="1" cellspacing="1">
    <tbody>
        <tr>
            <td colspan="2" align="center" style="padding-top: 10px; padding-bottom: 5px;">
                <table style="width: 430px;">
                    <tr>
                        <td style="padding-left: 10px;">
                            <asp:Label ID="PubliLine01" runat="server" Text="Actualice ahora a" Font-Size="14px"></asp:Label></td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:Label ID="PubliLine02" runat="server" Text="VisualTime Live Pro" Font-Size="24px" Font-Bold="True"></asp:Label></td>
                    </tr>
                    <tr>
                        <td align="right">
                            <asp:Label ID="PubliLine03" runat="server" Text="y obtendrá entre otros los siguientes beneficios:" Font-Size="14px"></asp:Label>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td style="width: 300px; height: 260px;" align="center" valign="middle">
                <img id="Img1" style="width: 280px;" alt="Image" src="~/Base/Images/VTBox.png" runat="server" /></td>
            <td style="width: 437px; padding-top: 30px; padding-bottom: 30px;" valign="top">
                <div style="display: block; padding: 5px; background-color: #EFEFEF; border: solid 1px silver; padding-right: 15px; width: 435px;">
                    <ul style="font-family: Tahoma;">
                        <li style="text-align: left;">
                            <asp:Label ID="PubliLine1" runat="server" Text="Posibilidad de definir Horarios complejos, con reglas automáticas de justificación de incidencias."></asp:Label></li>
                        <li style="text-align: left;">
                            <asp:Label ID="PubliLine2" runat="server" Text="Soporte para un número ilimitado de terminales"></asp:Label></li>
                        <li style="text-align: left;">
                            <asp:Label ID="PubliLine3" runat="server" Text="Previsión de incidencias futuras, como por ejemplo una visita al médico, que será justificada automáticamente cuando se produzca"></asp:Label></li>
                        <li style="text-align: left;">
                            <asp:Label ID="PubliLine4" runat="server" Text="Creaci&oacute;n de nuevos usuarios que puedan trabajar con VisualTime Live, así como la posibilidad de definir permisos específicos para estos usuarios"></asp:Label></li>
                        <li style="text-align: left;">
                            <asp:Label ID="PubliLine5" runat="server" Text="Definición de justificaciones y acumulados sin límites"></asp:Label></li>
                        <li style="text-align: left;">
                            <asp:Label ID="PubliLine6" runat="server" Text="Acceso a todas las extensiones de VisualTime Live para Control Horario"></asp:Label></li>
                        <li style="text-align: left;">
                            <asp:Label ID="PubliLine7" runat="server" Text="Posibilidad de integrar nuestro Control de Accesos, VisualTime Live Accesos"></asp:Label></li>
                    </ul>
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="2" align="center"><a href="http://www.robotics.es/" target="_blank">
                <asp:Label ID="PubliMoreInfo" runat="server" Text="Mas información" Font-Size="16px" Font-Bold="True"></asp:Label></a></td>
        </tr>
    </tbody>
</table>