<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.frmBudgetAddEmployees" CodeBehind="frmBudgetAddEmployees.ascx.vb" %>

<div id="<%= Me.ClientID %>_frm" class="ui-dialog-content">
    <form id="<%= Me.ClientID %>_attr" style="max-height: 500px">
        <table width="100%" cellspacing="0" class="bodyPopup">
            <tr style="height: 20px;">
                <td colspan="3">
                    <div class="panHeader2">
                        <span style="">
                            <asp:Label ID="lblBudgetAddEmployeeTitle" runat="server" Text="Agregar empleados a posición" />
                        </span>
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    <div style="padding-left: 24px;" class="descriptionAssignDiv">
                        <div id="lblDayInformation" runat="server">
                        </div>
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    <div class="statusAssignDiv">
                        <div class="alertsContainter">
                            <div class="AddEmployeeAlert redAlert">
                                <dx:ASPxLabel CssClass="EmployeeAlertText" ID="lblWithoutAssignment" runat="server" />
                            </div>
                            <div id="lblWithoutAssignmentContainer" runat="server" style="float: left; width: 120px">
                                <dx:ASPxLabel CssClass="EmployeeDescText" ID="lblWithoutAssignmentDesc" Text="Sin puesto" runat="server" />
                            </div>
                            <div id="lblWithoutAssignmentTooltip" runat="server">
                                <div id="lstEmployeesWithousAssignment" style="width: 200px; height: 300px;" runat="server"></div>
                            </div>

                            <div class="AddEmployeeAlert skinAlert">
                                <dx:ASPxLabel CssClass="EmployeeAlertText" ID="lblInRest" runat="server" />
                            </div>
                            <div id="lblInRestContainer" runat="server" style="float: left; width: 120px">
                                <dx:ASPxLabel CssClass="EmployeeDescText" ID="lblInRestDesc" Text="En descanso" runat="server" />
                            </div>
                            <div id="lblInRestTooltip" runat="server">
                                <div id="lstEmployeesInRest" style="width: 200px; height: 300px;" runat="server"></div>
                            </div>

                            <div class="AddEmployeeAlert greenAlert">
                                <dx:ASPxLabel CssClass="EmployeeAlertText" ID="lblInHolidays" runat="server" />
                            </div>
                            <div id="lblInHolidaysContainer" runat="server" style="float: left; width: 120px">
                                <dx:ASPxLabel CssClass="EmployeeDescText" ID="lblInHolidaysDesc" Text="De vacaciones" runat="server" />
                            </div>
                            <div id="lblInHolidaysTooltip" runat="server">
                                <div id="lstEmployeesInHolidays" style="width: 200px; height: 300px;" runat="server"></div>
                            </div>

                            <div class="AddEmployeeAlert yellowAlert">
                                <dx:ASPxLabel CssClass="EmployeeAlertText" ID="lblOnAbsence" runat="server" />
                            </div>
                            <div id="lblOnAbsenceContainer" runat="server" style="float: left; width: 120px">
                                <dx:ASPxLabel CssClass="EmployeeDescText" ID="lblOnAbsenceDesc" Text="En ausencia" runat="server" />
                            </div>
                            <div id="lblOnAbsenceTooltip" runat="server">
                                <div id="lstEmployeesOnAbsence" style="width: 200px; height: 300px;" runat="server"></div>
                            </div>
                        </div>
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan="3">
                    <div class="jsGrid">
                        <asp:Label ID="lblAvailableEmployeesTitle" runat="server" CssClass="jsGridTitle" Text="Empleados"></asp:Label>
                    </div>
                    <div id="divAvailableEmployeesGrid" runat="server" style="min-height: 250px;" class="jsGridContent dextremeGrid">
                        <!-- Carrega del Grid Usuari General -->
                    </div>
                </td>
            </tr>
        </table>
    </form>
</div>