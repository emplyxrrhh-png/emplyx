<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.EmployeePermissions"
    Culture="auto" UICulture="auto" CodeBehind="EmployeePermissions.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Permisos ${Employees}</title>
</head>
<body class="bodyPopup" style="background-attachment: fixed;">

    <form id="frmEmployeePremissions" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <script language="javascript" type="text/javascript">

            function PageBase_Load() {

                var idPassport = document.getElementById('hdnID').value;
                var idFeature = document.getElementById('hdnIDFeature').value;
                LoadEmployeeGroups(idPassport, idFeature);
                LoadEmployees(idPassport, idFeature);

            }
        </script>

        <input type="hidden" id="hdnID" runat="server" />
        <input type="hidden" id="hdnIDFeature" runat="server" />

        <table style="width: 100%; height: 100%" border="0">
            <tr>
                <td colspan="2" style="padding-top: 5px; padding-bottom: 0px;" valign="top" height="20px">
                    <div class="panHeader2">
                        <span style="">
                            <asp:Label runat="server" ID="lblTitle" Text="Gestión de ${Employees}"></asp:Label></span>
                    </div>
                </td>
            </tr>
            <tr style="height: 48px">
                <td style="padding: 4px; padding-bottom: 10px;">
                    <img src="Images/Permissions_48.png" />
                </td>
                <td align="left" valign="middle" style="padding: 4px; padding-bottom: 10px;">
                    <asp:Label ID="lblDescription" runat="server" CssClass="editTextFormat" Text=""></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="2" align="center" valign="top" style="">
                    <table style="width: 100%;">
                        <tr>
                            <td style="width: 50%;">
                                <div class="panHeader2">
                                    <div style="float: left; text-align: left; vertical-align: middle;">
                                        <asp:Label ID="lblEmployeeGroupsTitle" Text="Grupos ${Employees}" runat="server" CssClass="panHeaderLabel" />
                                    </div>
                                </div>
                            </td>
                            <td style="width: 50%;">
                                <div class="panHeader2">
                                    <div style="float: left; text-align: left; vertical-align: middle;">
                                        <asp:Label ID="lblEmployeesTitle" Text="${Employees}" runat="server" CssClass="panHeaderLabel" />
                                    </div>
                                    <div style="float: right; text-align: right; vertical-align: middle; padding-right: 10px;">
                                        <a href="javascript: void(0)" id="btAddEmployee" class="btnAddMode" runat="server" onclick="ShowEmployeeSelector(this);">
                                            <asp:Label ID="lblAddEmployee" Text="Añadir" runat="server" /></a>
                                    </div>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td align="center" valign="top" style="width: 50%;">
                                <div id="divEmployeeGroups" runat="server" style="width: 100%; height: 390px; overflow: auto;">
                                </div>
                            </td>
                            <td align="center" valign="top" style="widows: 50%;">
                                <div id="divEmployees" runat="server" style="width: 100%; height: 390px; overflow: auto;">
                                </div>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td colspan="2" align="right">
                    <asp:Button ID="btClose" Text="${Button.Close}" runat="server" OnClientClick="Close(); return false;" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                </td>
            </tr>
        </table>

        <roWebControls:roPopupFrameV2 ID="RoPopupFrame1" runat="server" ShowTitleBar="true" BehaviorID="RoPopupFrame1Behavior" CssClassPopupExtenderBackground="modalBackgroundTransparent" Width="330px">
            <FrameContentTemplate>
                <table cellpadding="0" cellspacing="0">
                    <tr>
                        <td>
                            <asp:Label ID="lblGroupSelection" Text="Selección ${Employee}" runat="server" />
                        </td>
                        <td align="right">
                            <asp:ImageButton ID="btSelectorOk" runat="server" ImageUrl="~/Base/Images/ButtonOK_16.png" Style="cursor: pointer;" OnClientClick='addEmployeeSelected(); return false;' />
                            <asp:ImageButton ID="btSelectorCancel" runat="server" ImageUrl="~/Base/Images/ButtonCancel_16.png" Style="cursor: pointer;" OnClientClick='HideEmployeeSelector(); return false;' />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" valign="top">
                            <iframe id="ifEmployeeSelector" runat="server" src="" width="290px" height="350px"
                                scrolling="auto" frameborder="0" marginheight="0" marginwidth="0" />
                        </td>
                    </tr>
                </table>
            </FrameContentTemplate>
        </roWebControls:roPopupFrameV2>
    </form>
</body>
</html>