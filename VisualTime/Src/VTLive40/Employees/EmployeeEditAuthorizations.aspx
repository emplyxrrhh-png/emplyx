<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.EmployeeEditAuthorizations" EnableEventValidation="false" EnableViewState="True" CodeBehind="EmployeeEditAuthorizations.aspx.vb" %>

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

                function GridAccessGroups_BeginCallback(e, c) {

                }

                function GridAccessGroups_EndCallback(s, e) {
                    if (s.IsEditing()) { }
                    else {
                    }
                }

                function GridAccessGroups_OnRowDblClick(s, e) {

                }

                function GridAccessGroups_FocusedRowChanged(s, e) {

                }

                function AddNewAccessGroup(s, e) {
                    if (parseInt($("#hdnAccessMode").val(), 10) == 1 || (parseInt($("#hdnAccessMode").val(), 10) != 1 && GridAccessGroupsClient.GetVisibleRowsOnPage() < 1)) {
                        var grid = ASPxClientGridView.Cast("GridAccessGroupsClient");
                        grid.AddNewRow();
                    } else {
                        window.parent.frames['ifPrincipal'].showErrorPopup("Error.ValidationTitle", "error", "Error.OnlyOneAuthorization", "Error.OK", "Error.OKDesc", "");
                    }
                }

                function DeleteAccessGroup(IdRow) {
                    grid = ASPxClientGridView.Cast("GridAccessGroupsClient");
                    grid.DeleteRow(IdRow);
                }
            </script>

            <table style="width: 100%;" border="0">
                <tr>
                    <td colspan="2" style="padding-top: 5px; padding-bottom: 0px;" height="20px" valign="top">
                        <div class="panHeader2">
                            <span style="">
                                <asp:Label ID="lblTitle" runat="server" Text="Autorizaciones"></asp:Label></span>
                        </div>
                    </td>
                </tr>
                <tr style="height: 48px">
                    <td style="padding: 4px; padding-bottom: 10px;">
                        <img src="~/Access/Images/AccessGroup.png" runat="server" />
                    </td>
                    <td align="left" valign="middle" style="padding: 4px; padding-bottom: 10px;">
                        <asp:Label ID="lblDescription" runat="server" CssClass="editTextFormat" Text="Puede gestionar las autorizaciones sobre los grupos de acceso donde el empleado tiene permiso para acceder"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="center">

                        <div class="jsGrid">
                            <input type="hidden" value="" runat="server" id="hdnAccessMode" />
                            <input type="hidden" value="" runat="server" id="hdnAccessRowsCount" />
                            <asp:Label ID="lblContractsCaption" runat="server" CssClass="jsGridTitle" Text="Autorizaciones"></asp:Label>
                            <div class="jsgridButton">
                                <dx:ASPxButton ID="btnAddNewAccessGroup" runat="server" AutoPostBack="False" CausesValidation="False" Text="Añadir" ToolTip="Añadir nueva autorización" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat">
                                    <Image Url="~/Base/Images/Grid/add.png"></Image>
                                    <ClientSideEvents Click="AddNewAccessGroup" />
                                </dx:ASPxButton>
                            </div>
                        </div>
                        <div class="jsGridContent">
                            <dx:ASPxGridView ID="GridAccessGroups" runat="server" AutoGenerateColumns="False" ClientInstanceName="GridAccessGroupsClient" KeyboardSupport="True" Width="100%" ClientSideEvents-BeginCallback="GridAccessGroups_BeginCallback">
                                <Settings ShowTitlePanel="False" VerticalScrollBarMode="Auto" UseFixedTableLayout="True" VerticalScrollableHeight="200" />
                                <ClientSideEvents EndCallback="GridAccessGroups_EndCallback" RowDblClick="GridAccessGroups_OnRowDblClick" FocusedRowChanged="GridAccessGroups_FocusedRowChanged" />
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
                                            <asp:Button ID="btOK" Text="${Button.Accept}" runat="server" OnClientClick="" CssClass="btnFlat btnFlatBlack btnFlatAsp" /></td>
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

            <Local:MessageFrame ID="MessageFrame1" runat="server" />
        </div>
    </form>
</body>
</html>