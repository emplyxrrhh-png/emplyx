<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.EmployeeEditMessages" EnableEventValidation="false" EnableViewState="True" CodeBehind="EmployeeEditMessages.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Edición autorizaciones</title>
</head>
<body class="bodyPopup" style="background-attachment: fixed;">

    <form id="frmEmployeeEditMobility" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <div>

            <script language="javascript" type="text/javascript">

                function PageBase_Load() {
                    ConvertControls('');
                }

                function GridMessages_BeginCallback(e, c) {

                }

                function GridMessages_EndCallback(s, e) {
                    if (s.IsEditing()) { }
                    else {
                    }
                }

                function GridMessages_OnRowDblClick(s, e) {

                }

                function GridMessages_FocusedRowChanged(s, e) {

                }

                function DeleteAccessGroup(IdRow) {
                    grid = ASPxClientGridView.Cast("GridMessagesClient");
                    grid.DeleteRow(IdRow);
                }
            </script>

            <table style="width: 100%;" border="0">
                <tr>
                    <td colspan="2" style="padding-top: 5px; padding-bottom: 0px;" height="20px" valign="top">
                        <div class="panHeader2">
                            <span style="">
                                <asp:Label ID="lblTitle" runat="server" Text="Mensajes a empleado"></asp:Label></span>
                        </div>
                    </td>
                </tr>
                <tr style="height: 48px">
                    <td style="padding: 4px; padding-bottom: 10px;">
                        <img src="~/Employees/Images/email_48.png" alt="" runat="server" />
                    </td>
                    <td align="left" valign="middle" style="padding: 4px; padding-bottom: 10px;">
                        <asp:Label ID="lblDescription" runat="server" CssClass="editTextFormat" Text="Puede gestionar las mensajes que visualizará el empleado mediante el portal o los terminales"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="center">
                        <div class="jsGrid">
                            <asp:Label ID="lblMessagesCaption" runat="server" CssClass="jsGridTitle" Text="Mensajes"></asp:Label>
                        </div>
                        <div class="jsGridContent">
                            <dx:ASPxGridView ID="GridMessages" runat="server" AutoGenerateColumns="False" ClientInstanceName="GridMessagesClient" KeyboardSupport="True" Width="100%" ClientSideEvents-BeginCallback="GridMessages_BeginCallback">
                                <Settings ShowTitlePanel="False" VerticalScrollBarMode="Auto" UseFixedTableLayout="True" VerticalScrollableHeight="200" />
                                <ClientSideEvents EndCallback="GridMessages_EndCallback" RowDblClick="GridMessages_OnRowDblClick" FocusedRowChanged="GridMessages_FocusedRowChanged" />
                                <SettingsCommandButton>
                                    <DeleteButton Image-Url="~/Base/Images/Grid/remove.png" Image-ToolTip="" />
                                    <UpdateButton Image-Url="~/Base/Images/Grid/save.png" Image-ToolTip="" />
                                    <CancelButton Image-Url="~/Base/Images/Grid/cancel.png" Image-ToolTip="" />
                                    <EditButton Image-Url="~/Base/Images/Grid/edit.png" Image-ToolTip="" />
                                </SettingsCommandButton>
                                <Styles>
                                    <CommandColumn Spacing="5px" />
                                    <Header CssClass="jsGridHeaderCell" />
                                    <Cell Wrap="False" />
                                </Styles>
                                <SettingsPager Mode="ShowAllRecords" ShowEmptyDataRows="false">
                                </SettingsPager>
                            </dx:ASPxGridView>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td align="right" colspan="2" style="height: 20px; padding-right: 5px;">
                        <asp:UpdatePanel ID="updActions" runat="server">
                            <ContentTemplate>
                                <table>
                                    <tr>
                                        <td>
                                            <asp:Button ID="btCancel" Text="${Button.Close}" runat="server" OnClientClick="Close(); return false;" CssClass="btnFlat btnFlatBlack btnFlatAsp" /></td>
                                    </tr>
                                </table>
                                <asp:HiddenField ID="hdnCanClose_PageBase" runat="server" />
                                <asp:HiddenField ID="hdnMustRefresh_PageBase" Value="0" runat="server" />
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