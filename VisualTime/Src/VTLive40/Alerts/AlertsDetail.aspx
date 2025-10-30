<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Alerts_Default" CodeBehind="AlertsDetail.aspx.vb" %>

<!DOCTYPE html>

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>

    <script language="javascript" type="text/javascript">
        function PageBase_Load() {
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div style="width: 820px">
            <div style="display: none">
                <input type="hidden" value="/#/./Employees/Employees" runat="server" id="EmployeeURI" />
                <input type="hidden" value="/#/./Employees/Groups" runat="server" id="CompanyURI" />
                <input type="hidden" value="/#/./Scheduler/Scheduler?IDEmployee=" runat="server" id="CalendarURI" />
                <input type="hidden" value="/#/./Tasks/Tasks?IDTask=" runat="server" id="TasksURI" />
                <input type="hidden" value="/#/./Absences/AbsencesStatus" runat="server" id="AbsencesURI" />
                <input type="hidden" value="/#/./AIScheduler/Budget" runat="server" id="AISchedulerURI" />
                <input type="hidden" value="/#/./Security/Passports" runat="server" id="SupervisorURI" />

                <input type="hidden" value="" runat="server" id="hdnNotifType" />
                <input type="hidden" value="" runat="server" id="hdnDocAlertType" />
                <input type="hidden" value="" runat="server" id="hdnIdRelatedObject" />
                <input type="hidden" value="" runat="server" id="hdnDocType" />
                <input type="hidden" value="" runat="server" id="hdnDocumentAlertType" />
            </div>
            <div class="jsGrid" style="width: 789px">
                <asp:Label ID="Label17" runat="server" CssClass="jsGridTitle" Text="Detalles"></asp:Label>
                <div class="jsgridButton">&nbsp;</div>
            </div>

            <div class="jsGridContent" style="width: 795px">
                <dx:ASPxGridView ID="GridDetails" runat="server" AutoGenerateColumns="False" ClientInstanceName="GridDetailsClient" KeyboardSupport="True" Width="795px">
                    <ClientSideEvents CustomButtonClick="GridDetailsClient_Click" />
                    <Settings ShowTitlePanel="False" VerticalScrollBarMode="Auto" />
                    <Styles>
                        <CommandColumn Spacing="5px" />
                        <Header CssClass="jsGridHeaderCell" />
                        <Cell Wrap="False" />
                    </Styles>
                </dx:ASPxGridView>
            </div>
            <div style="width: 795px">
                <div style="display: block; float: right; padding: 5px 8px;">
                    <dx:ASPxButton ID="btnAlertsDetailClose" runat="server" AutoPostBack="False" CausesValidation="False" Text="Cerrar" ToolTip="Cerrar" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat btnFlatBlack">
                        <ClientSideEvents Click="function(s, e) { parent.AlertDetailsPopup_Client.Hide(); }" />
                    </dx:ASPxButton>
                </div>
            </div>
        </div>
    </form>
</body>
</html>