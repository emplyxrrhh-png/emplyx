<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.OptionsBusinessCenterField" CodeBehind="BusinessCenterField.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Campo personalizado</title>
</head>
<body class="bodyPopup" style="background-attachment: fixed;">

    <form id="frmBusinessCenterField" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <div>

            <script language="javascript" type="text/javascript">

                function PageBase_Load() {

                    ConvertControls();

                }

                function endRequestHandler() {
                    ConvertControls();
                }

                function VisibleAccessValidationControl(visible) {

                }

                function ShowAddCategory(button) {

                }
                function AddCategoryOK() {
                }
                function RemoveCategory() {

                }

                function CheckSave() {
                    if (CheckConvertControls('') == false) {
                        var message = "TitleKey=SaveBusinessCenterField.Check.Invalid.Text&" +
                            "DescriptionKey=SaveBusinessCenterField.Check.Invalid.Description&" +
                            "Option1TextKey=SaveBusinessCenterField.Check.Invalid.Option1Text&" +
                            "Option1DescriptionKey=SaveBusinessCenterField.Check.Invalid.Option1Description&" +
                            "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                                  'IconUrl=<%= Robotics.Web.Base.HelperWeb.MsgBoxIconsUrl(Robotics.Web.Base.HelperWeb.MsgBoxIcons.ErrorIcon) %>'
                        var url = "Options/srvMsgBoxOptions.aspx?action=MessageEx&Parameters=" + encodeURIComponent(message);
                        parent.ShowMsgBoxForm(url, 500, 300, '');
                        return false;
                    }
                    else {
                        return true;
                    }
                }

                function RequestCriteriaOK() {
                }

                function ShowRequestCriteria() {

                }
            </script>

            <table style="width: 100%; height: 100%;" border="0">
                <tr>
                    <td colspan="2" style="padding-top: 5px; padding-bottom: 0px;" valign="top" height="20px">
                        <div class="panHeader2">
                            <span style="">
                                <asp:Label ID="LblTitle" runat="server" Text="Campo personalizado"></asp:Label></span>
                        </div>
                    </td>
                </tr>
                <tr style="height: 48px">
                    <td style="padding: 4px; padding-bottom: 10px;">
                        <img src="Images/UserFields 32.gif" />
                    </td>
                    <td align="left" valign="middle" style="padding: 4px; padding-bottom: 10px;">
                        <asp:Label ID="lblDescription" runat="server" CssClass="editTextFormat" Text="Puede definir un campo personalizado para la ficha del ${BusinessCenter}."></asp:Label>
                    </td>
                </tr>
                <tr style="vertical-align: top; height: 300px;">
                    <td colspan="2">

                        <asp:UpdatePanel ID="updBusinessCenterField" runat="server">
                            <ContentTemplate>
                                <roUserControls:roTabContainerClient ID="tabCtl01" runat="server">
                                    <TabTitle1>
                                        <asp:Label ID="lblGeneralTitle" runat="server" Text="General"></asp:Label>
                                    </TabTitle1>
                                    <TabContainer1>

                                        <table cellpadding="1" cellspacing="1" width="100%" style="padding-top: 0px; height: 100%;" border="0">
                                            <tr style="padding-top: 10px;">
                                                <td align="right" style="height: 10px;">
                                                    <asp:Label ID="lblName" Text="Nombre" runat="server" />
                                                </td>
                                                <td style="padding-left: 5px" style="height: 10px;" colspan="1">
                                                    <input type="text" id="txtName" value="" style="width: 204px;" runat="server" convertcontrol="TextField" ccallowblank="false" ccmaxlength="50" class="textClass" />
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
                                            <asp:Button ID="btAccept" Text="${Button.Accept}" runat="server" OnClientClick="return CheckSave();" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                        </td>
                                        <td>
                                            <asp:Button ID="btCancel" Text="${Button.Cancel}" runat="server" OnClientClick="Close(); return false;" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                        </td>
                                    </tr>
                                </table>
                                <asp:HiddenField ID="hdnCanClose_PageBase" runat="server" />
                                <asp:HiddenField ID="hdnMustRefresh_PageBase" Value="0" runat="server" />
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