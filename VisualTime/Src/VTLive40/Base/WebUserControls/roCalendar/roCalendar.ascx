<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.roCalendar" CodeBehind="roCalendar.ascx.vb" %>

<%@ Register Src="~/Base/WebUserControls/roCalendar/WebUserForms/frmComplementary.ascx" TagName="frmComplementary" TagPrefix="roForms" %>
<%@ Register Src="~/Base/WebUserControls/roCalendar/WebUserForms/frmCopyOptions.ascx" TagName="frmCopyOptions" TagPrefix="roForms" %>
<%@ Register Src="~/Base/WebUserControls/roCalendar/WebUserForms/frmErrorDetail.ascx" TagName="frmErrorDetail" TagPrefix="roForms" %>
<%@ Register Src="~/Base/WebUserControls/roCalendar/WebUserForms/frmImportExcel.ascx" TagName="frmImportExcel" TagPrefix="roForms" %>
<%@ Register Src="~/Base/WebUserControls/roCalendar/WebUserForms/frmAdvCopy.ascx" TagName="frmAdvCopy" TagPrefix="roForms" %>
<%@ Register Src="~/Base/WebUserControls/roCalendar/WebUserForms/frmImportDetail.ascx" TagName="frmImportDetail" TagPrefix="roForms" %>
<%@ Register Src="~/Base/WebUserControls/roCalendar/WebUserForms/frmFilterCalendar.ascx" TagName="frmFilterCalendar" TagPrefix="roForms" %>
<%@ Register Src="~/Base/WebUserControls/roCalendar/WebUserForms/frmAssignments.ascx" TagName="frmAssignments" TagPrefix="roForms" %>
<%@ Register Src="~/Base/WebUserControls/roCalendar/WebUserForms/frmSortCalendar.ascx" TagName="frmSortCalendar" TagPrefix="roForms" %>
<%@ Register Src="~/Base/WebUserControls/roCalendar/WebUserForms/frmShiftSelector.ascx" TagName="frmShiftSelector" TagPrefix="roForms" %>
<%@ Register Src="~/Base/WebUserControls/roCalendar/WebUserForms/frmProductiveUnitSelector.ascx" TagName="frmProductiveUnitSelector" TagPrefix="roForms" %>
<%@ Register Src="~/Base/WebUserControls/roCalendar/WebUserForms/frmBudgetAddEmployees.ascx" TagName="frmBudgetAddEmployees" TagPrefix="roForms" %>
<%@ Register Src="~/Base/WebUserControls/roCalendar/WebUserForms/frmEditDailyView.ascx" TagName="frmEditDailyView" TagPrefix="roForms" %>
<%@ Register Src="~/Base/WebUserControls/roCalendar/WebUserForms/frmStarterShift.ascx" TagName="frmStarter" TagPrefix="roForms" %>

<input runat="server" type="hidden" id="hdnCalendarWorkMode" value="roCalendar" />
<input runat="server" type="hidden" id="hdnClientInstanceName" value="objCalendar" />

<dx:ASPxCallback ID="PerformActionCallback" runat="server"></dx:ASPxCallback>

<dx:ASPxCallback ID="ASPxRoCalendarCallback" runat="server" width="100%" height="100%">
</dx:ASPxCallback>

<dx:ASPxHiddenField ID="hdnCalendarConfig" runat="server" ClientInstanceName="hdnCalendarConfigClient"></dx:ASPxHiddenField>
<input type="hidden" id="hdnHolidayShiftPeriodicity" value="hola" runat="server" />

<div id="roCalendarRender" runat="server" class="roCalendarRender" style="height: 100%; width: 100%">
</div>

<div style="display: none">
    <!-- popup copia avanzada -->
    <roForms:frmAdvCopy ID="dlgAdvCopy" runat="server" />
    <!-- popup horario starter -->
    <roForms:frmStarter ID="dlgStarter" runat="server" />
    <!-- popup horas complementarias -->
    <roForms:frmComplementary ID="dlgComplementary" runat="server" />
    <!-- popup opciones copia -->
    <roForms:frmCopyOptions ID="dlgCopy" runat="server" />
    <!-- popup detalles error -->
    <roForms:frmErrorDetail ID="dlgError" runat="server" />
    <!-- popup detalles error -->
    <roForms:frmImportDetail ID="dlgImportError" runat="server" />
    <!-- popup opciones importacion -->
    <roForms:frmImportExcel ID="dlgImport" runat="server" />
    <!-- popup filtraje puestos -->
    <roForms:frmFilterCalendar ID="dlgFilterCalendar" runat="server" />
    <!-- popup edicion puestos -->
    <roForms:frmAssignments ID="dlgAssignments" runat="server" />
    <!-- popup ordenación calendario -->
    <roForms:frmSortCalendar ID="dlgSortCalendar" runat="server" />
    <!-- popup selección horarios -->
    <roForms:frmShiftSelector ID="dlgShiftSelector" runat="server" />
    <!-- popup selección de unidades productivas -->
    <roForms:frmProductiveUnitSelector ID="dlgProductiveUnitSelector" runat="server" />
    <!-- popup adición de empleados a presupuestos -->
    <roForms:frmBudgetAddEmployees ID="dlgBudgetAddEmployees" runat="server" />

    <!-- popup adición de empleados a presupuestos -->
    <roForms:frmEditDailyView ID="dlgEditDailyView" runat="server" />
</div>