<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.TaskAlert" CodeBehind="TaskAlert.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Campo personalizado</title>
</head>
<body class="bodyPopup" style="background-attachment: fixed;">

    <form id="frmTaskAlert" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <div>

            <script language="javascript" type="text/javascript">

                function PageBase_Load() {
                    ConvertControls();
                }

                function endRequestHandler() {
                    ConvertControls();
                }

                function Refresh() {
                    var DataChanged = document.getElementById('<%= hdnMustRefresh_PageBase.ClientID %>').value;
                if (DataChanged == '1') parent.RefreshTaskAlertsScreen('1', document.getElementById('<%= hdnParams_PageBase.ClientID %>').value);
                }
            </script>

            <table style="width: 100%; height: 100%;" border="0">
                <tr>
                    <td colspan="2" style="padding-top: 5px; padding-bottom: 0px;" valign="top" height="20px">
                        <div class="panHeader2">
                            <span style="">
                                <asp:Label ID="LblTitle" runat="server" Text="Alerta"></asp:Label></span>
                        </div>
                    </td>
                </tr>
                <tr style="height: 48px">
                    <td style="padding: 4px; padding-bottom: 10px;">
                        <img src="Images/UserFields 32.gif" />
                    </td>
                    <td align="left" valign="middle" style="padding: 4px; padding-bottom: 10px;">
                        <asp:Label ID="lblDescription" runat="server" CssClass="editTextFormat" Text="Puede indicar si la alerta ha sido leída o no y ver su contenido."></asp:Label>
                    </td>
                </tr>
                <tr style="vertical-align: top; height: 300px;">
                    <td colspan="2">

                        <asp:UpdatePanel ID="updTaskAlert" runat="server">
                            <ContentTemplate>
                                <roUserControls:roTabContainerClient ID="tabCtl01" runat="server">
                                    <TabTitle1>
                                        <asp:Label ID="lblGeneralTitle" runat="server" Text="Alerta"></asp:Label>
                                    </TabTitle1>
                                    <TabContainer1>

                                        <table cellpadding="1" cellspacing="1" width="100%" style="padding-top: 0px; height: 100%;" border="0">
                                            <tr>
                                                <td width="150px" align="right" valign="top" style="padding-right: 10px;">
                                                    <asp:Label ID="lblEmployee" runat="server" Text="Empleado:" class="spanEmp-Class"></asp:Label>
                                                </td>

                                                <td style="height: 10px;" colspan="2">
                                                    <input type="text" runat="server" id="txtEmployeeName" class="textClass x-form-text x-form-field" maxlength="50" style="width: 200px;" convertcontrol="TextField" ccallowblank="false" />
                                                </td>
                                            </tr>

                                            <tr>
                                                <td width="150px" align="right" valign="top" style="padding-right: 10px;">
                                                    <asp:Label ID="lblDate" runat="server" Text="Fecha:" class="spanEmp-Class"></asp:Label>
                                                </td>

                                                <td style="height: 10px;" colspan="2">
                                                    <input type="text" runat="server" id="txtDate" class="textClass x-form-text x-form-field" maxlength="50" style="width: 120px;" convertcontrol="TextField" ccallowblank="false" />
                                                </td>
                                            </tr>
                                            <tr style="padding-top: 5px">
                                                <td width="150px" align="right" valign="top" style="padding-right: 10px;">
                                                    <asp:Label ID="lblComment" runat="server" Text="Comentario:" class="spanEmp-Class"></asp:Label>
                                                </td>
                                                <td align="left" valign="top" style="padding-right: 30px;">
                                                    <textarea id="txtComment" runat="server" rows="5" style="width: 290px; height: 120px;"
                                                        class="textClass x-form-text x-form-field" convertcontrol="TextArea" ccallowblank="true"></textarea>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td width="150px" align="right" valign="top" style="padding-right: 10px; padding-top: 5px">
                                                    <input type="checkbox" id="chkReaded" runat="server" />&nbsp;<asp:Label ID="lblReaded" runat="server" Text="Leída"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </TabContainer1>
                                </roUserControls:roTabContainerClient>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <td align="right" colspan="2" style="height: auto;">
                        <asp:UpdatePanel ID="updActions" runat="server">
                            <ContentTemplate>
                                <table>
                                    <tr align="right">
                                        <td>
                                            <asp:Button ID="btAccept" Text="${Button.Accept}" runat="server" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                        </td>
                                        <td>
                                            <asp:Button ID="btCancel" Text="${Button.Cancel}" runat="server" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                        </td>
                                    </tr>
                                </table>
                                <asp:HiddenField ID="hdnIDEmployee" runat="server" />
                                <asp:HiddenField ID="hdnIDAlert" runat="server" />
                                <asp:HiddenField ID="hdnCanClose_PageBase" runat="server" />
                                <asp:HiddenField ID="hdnMustRefresh_PageBase" Value="0" runat="server" />
                                <asp:HiddenField ID="hdnParams_PageBase" runat="server" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="center">
                        <asp:UpdatePanel ID="updError" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <asp:Label ID="lblError" Visible="false" CssClass="errorText" runat="server" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>

            <Local:MessageFrame ID="MessageFrame1" runat="server" />
        </div>
    </form>
</body>
</html>