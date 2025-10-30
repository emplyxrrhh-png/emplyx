<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.WebUserControls_EmployeeType" CodeBehind="EmployeeType.ascx.vb" %>

<roUserControls:roOptionPanelContainer ID="chkAttendance" runat="server" TypeOPanel="CheckboxOption" Width="100%" Height="70px" Text="Control de Presencia">
    <Content>
        <table cellpadding="0" cellspacing="0">
            <tr>
                <td style="padding-right: 3px;">
                    <asp:ImageButton ID="ibtAttendance" ImageUrl="~/Employees/Images/Employee_32x32.GIF" runat="server" />
                </td>
                <td>
                    <asp:Label ID="lblAttendanceDescription" Text="Gestión de tiempos de horas de exceso, absentismos, horas laborales, vacaciones, control de convenios,..." runat="server"></asp:Label>
                </td>
            </tr>
        </table>
    </Content>
</roUserControls:roOptionPanelContainer>

<roUserControls:roOptionPanelContainer ID="chkJob" runat="server" TypeOPanel="CheckboxOption" Width="100%" Height="70px" Text="Control de Producción">
    <Content>
        <table cellpadding="0" cellspacing="0">
            <tr>
                <td style="padding-right: 3px;">
                    <asp:ImageButton ID="ibtJob" ImageUrl="~/Employees/Images/Produccion.GIF" runat="server" />
                </td>
                <td>
                    <asp:Label ID="lblJobDescription" Text="Gestión de los trabajos diarios realizados individualmente o en equipo, supervisión estado máquinas, tiempos improductivos, incidencias,..." runat="server"></asp:Label>
                </td>
            </tr>
        </table>
    </Content>
</roUserControls:roOptionPanelContainer>

<roUserControls:roOptionPanelContainer ID="chkAccess" runat="server" TypeOPanel="CheckboxOption" Width="100%" Height="70px" Text="Control de Accesos">
    <Content>
        <table cellpadding="0" cellspacing="0">
            <tr>
                <td style="padding-right: 3px;">
                    <asp:ImageButton ID="ibtAccess" ImageUrl="~/Employees/Images/Accessgroup.png" runat="server" />
                </td>
                <td>
                    <asp:Label ID="lblAccessDescription" Text="Restricción de horas en las que el empleado puede acceder a determinadas zonas de la empresa." runat="server"></asp:Label>
                </td>
            </tr>
        </table>
    </Content>
</roUserControls:roOptionPanelContainer>

<div style="display: none;">
    <roUserControls:roOptionPanelContainer ID="chkExterns" runat="server" TypeOPanel="CheckboxOption" Width="100%" Height="70px" Text="Control de Externos">
        <Content>
            <table cellpadding="0" cellspacing="0">
                <tr>
                    <td style="padding-right: 3px;">
                        <asp:ImageButton ID="ibtExterns" ImageUrl="~/Employees/Images/Employee_32x32.gif" runat="server" />
                    </td>
                    <td>
                        <asp:Label ID="lblExternsDescription" Text="Gestión de las empresas externas que prestan servicio en la empresa." runat="server"></asp:Label>
                    </td>
                </tr>
            </table>
        </Content>
    </roUserControls:roOptionPanelContainer>
</div>
<div style="display: none;">
    <roUserControls:roOptionPanelContainer ID="chkPrevention" runat="server" TypeOPanel="CheckboxOption" Width="100%" Height="70px" Text="Prevención de riesgos laborales">
        <Content>
            <table cellpadding="0" cellspacing="0">
                <tr>
                    <td style="padding-right: 3px;">
                        <asp:ImageButton ID="ibtPrevention" ImageUrl="~/Employees/Images/Prevention32.png" runat="server" />
                    </td>
                    <td>
                        <asp:Label ID="lblPreventionDescription" Text="Gestión de la documentación necesaria para poder acceder a las instalaciones de la empresa." runat="server"></asp:Label>
                    </td>
                </tr>
            </table>
        </Content>
    </roUserControls:roOptionPanelContainer>
</div>