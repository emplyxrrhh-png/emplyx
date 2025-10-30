<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.EmployeeType" CodeBehind="EmployeeType.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<%@ Register Src="~/Employees/WebUserControls/EmployeeType.ascx" TagName="EmployeeType" TagPrefix="Local" %>

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Tipo ${Employee}</title>
</head>
<body class="bodyPopup">
    <form id="frmEmployeeType" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />
        <div style="padding-top: 10px;">

            <asp:UpdatePanel ID="updEmployeeType" runat="server">
                <ContentTemplate>
                    <div class="popupWizardContent">
                        <asp:HiddenField ID="hdnJobEnabled" runat="server" />
                        <Local:EmployeeType ID="cnEmployeeType" runat="server" ShowDisabledTypes="true" />
                    </div>
                    <div class="popupWizardButtons" align="right">
                        <table>
                            <tr>
                                <td>
                                    <asp:Button ID="btnAccept" Text="${Button.Accept}" runat="server" OnClientClick="" CssClass="btnFlat btnFlatBlack btnFlatAsp" /></td>
                                <td>
                                    <asp:Button ID="btnCancel" Text="${Button.Cancel}" runat="server" OnClientClick="Close(); return false;" CssClass="btnFlat btnFlatBlack btnFlatAsp" /></td>
                            </tr>
                        </table>
                        <asp:HiddenField ID="hdnCanClose_PageBase" runat="server" />
                        <asp:HiddenField ID="hdnMustRefresh_PageBase" Value="0" runat="server" />
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>

        <Local:MessageFrame ID="MessageFrame1" runat="server" />
    </form>
</body>
</html>