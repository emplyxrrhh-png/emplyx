<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.VTLive40_roDailyRule" CodeBehind="roDailyRule.aspx.vb" %>
<%@ Register Src="~/Shifts/WebUserForms/frmDailyRule.ascx" TagName="frmDailyRule" TagPrefix="roForms" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>roWizardSelectorContainerMultiSelectV3</title>
    <style type="text/css">
        .ui-accordion .ui-accordion-content {
            height: 135px !important;
        }
    </style>
</head>


<body>
    

    <form id="form1" runat="server" style="height: 100%;">
        <div style="overflow: auto;" class="defaultBackgroundColor">
            <input type="hidden" id="origen" value="<%= Me.Language.Translate("Compositions.gridHeaderOrigen", DefaultScope) %>" />
            <input type="hidden" id="destino" value="<%= Me.Language.Translate("Compositions.gridHeaderDestino", DefaultScope) %>" /><input type="hidden" id="dateFormatValue" runat="server" value="" />
            <input type="hidden" id="OperationPlus" value="<%=  Me.Language.Translate("Compositions.OperationsPlus", DefaultScope) %>" />
            <input type="hidden" id="OperationMinus" value="<%=  Me.Language.Translate("Compositions.OperationMinus", DefaultScope) %>" />
            <input type="hidden" id="header1" value="<%= Me.Language.Translate("Compositions.gridHeaderCause", DefaultScope) %>" />
            <input type="hidden" id="header2" value="<%= Me.Language.Translate("Compositions.gridHeaderOperation", DefaultScope) %>" />
            <input type="hidden" id="header3" value="<%= Me.Language.Translate("Compositions.gridHeaderTimeZone", DefaultScope) %>" />
            <input type="hidden" runat="server" id="hdnModeEdit" value="false" />

            <roForms:frmDailyRule ID="frmDailyRule1" runat="server" />
        </div>
    </form>
</body>
</html>