<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.EmployeeCenters" EnableEventValidation="false" EnableViewState="True" CodeBehind="EmployeeCenters.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Edición centros de coste autorizados</title>
</head>
<body class="bodyPopup" style="background-attachment: fixed;">

    <form id="frmEmployeeEditCost" runat="server">

        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />
        <div>

            <script language="javascript" type="text/javascript">

                function PageBase_Load() {
                    ConvertControls('');
                }

                function GridCenters_BeginCallback(e, c) {

                }

                function GridCenters_EndCallback(s, e) {
                    if (s.IsEditing()) { }
                    else {
                    }
                }

                function GridCenters_OnRowDblClick(s, e) {

                }

                function GridCenters_FocusedRowChanged(s, e) {

                }

                function AddNewCenter(s, e) {
                    var grid = ASPxClientGridView.Cast("GridCentersClient");
                    grid.AddNewRow();
                }

                function DeleteCenter(IdRow) {
                    grid = ASPxClientGridView.Cast("GridCentersClient");
                    grid.DeleteRow(IdRow);
                }
            </script>

            <table style="width: 100%;" border="0">
                <tr>
                    <td colspan="2" style="padding-top: 5px; padding-bottom: 0px;" height="20px" valign="top">
                        <div class="panHeader2">
                            <span style="">
                                <asp:Label ID="lblTitle" runat="server" Text="Centros de coste autorizados para realizar cesión"></asp:Label></span>
                        </div>
                    </td>
                </tr>
                <tr style="height: 48px">
                    <td style="padding: 4px; padding-bottom: 10px;">
                        <img src="~/Tasks/Images/BusinessCenters48.png" runat="server" />
                    </td>
                    <td align="left" valign="middle" style="padding: 4px; padding-bottom: 10px;">
                        <asp:Label ID="lblDescription" runat="server" CssClass="editTextFormat" Text="Puede gestionar los centros de coste donde el empleado está autorizado para poder realizar una cesión"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="center">
                        <div class="jsGrid">
                            <asp:Label ID="lblContractsCaption" runat="server" CssClass="jsGridTitle" Text="Centros de coste"></asp:Label>
                            <div class="jsgridButton">
                                <dx:ASPxButton ID="btnAddNewCenter" runat="server" AutoPostBack="False" CausesValidation="False" Text="Añadir" ToolTip="Añadir nuevo centro" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat">
                                    <Image Url="~/Base/Images/Grid/add.png"></Image>
                                    <ClientSideEvents Click="AddNewCenter" />
                                </dx:ASPxButton>
                            </div>
                        </div>
                        <div class="jsGridContent">
                            <dx:ASPxGridView ID="GridCenters" runat="server" AutoGenerateColumns="False" ClientInstanceName="GridCentersClient" KeyboardSupport="True" Width="100%" ClientSideEvents-BeginCallback="GridCenters_BeginCallback">
                                <Settings ShowTitlePanel="False" VerticalScrollBarMode="Auto" UseFixedTableLayout="True" VerticalScrollableHeight="200" />
                                <ClientSideEvents EndCallback="GridCenters_EndCallback" RowDblClick="GridCenters_OnRowDblClick" FocusedRowChanged="GridCenters_FocusedRowChanged" />
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