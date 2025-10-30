<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Employees_EmployeePermissions" CodeBehind="EmployeePermissions.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<%@ Register Src="~/Security/WebUserControls/PassportPermissions.ascx" TagName="PassportPermissions" TagPrefix="Local" %>

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="frmEmployeePermissions" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />
        <div>

            <script type="text/javascript">

                function PageBase_Load() {
                }
            </script>

            <div class="popupWizardContent">
                <div class="panHeader2">
                    <span style="">
                        <asp:Label runat="server" ID="lblEmployeePermissionsTitle" Text="Permisos ${Employee}"></asp:Label></span>
                </div>
                <br />
                <table style="width: 98%;" border="0">
                    <tr>
                        <td width="9%" height="48px" style="padding-left: 20px;">
                            <img src="Images/EmployeePermissions_48.png" style="border: 0;" />
                        </td>
                        <td width="86%" align="left" width="100px">
                            <span id="span1" runat="server" class="spanEmp-Class" style="cursor: default;">
                                <asp:Label ID="lblEmployeePermissions" runat="server" Text="En esta pantalla se pueden modificar los permisos para las distintas funcionalidades del ${Employee}"></asp:Label>
                            </span>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" style="">
                            <div style="overflow-y: scroll;">
                                <Local:PassportPermissions ID="cnPassportPermissions" TableHeight="385px" runat="server" />
                            </div>
                        </td>
                    </tr>
                </table>
            </div>
            <div class="popupWizardButtons" align="right">
                <table>
                    <tr>
                        <td align="right" colspan="2" style="height: 20px; padding-right: 5px;">
                            <asp:UpdatePanel ID="updActions" runat="server">
                                <ContentTemplate>
                                    <table>
                                        <tr>
                                            <td>
                                                <asp:Button ID="btCancel" Text="${Button.Cancel}" runat="server" OnClientClick="Close(); return false;" CssClass="btnFlat btnFlatBlack btnFlatAsp" /></td>
                                        </tr>
                                    </table>
                                    <asp:HiddenField ID="hdnCanClose_PageBase" runat="server" />
                                    <asp:HiddenField ID="hdnMustRefresh_PageBase" Value="0" runat="server" />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                </table>
            </div>
            <Local:MsgBoxForm ID="MsgBoxForm1" runat="server" />
        </div>
    </form>
</body>
</html>