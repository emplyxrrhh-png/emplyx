<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.RemarksConfig" EnableEventValidation="false" Culture="auto" UICulture="auto" EnableSessionState="True" CodeBehind="RemarksConfig.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Configuración resaltes calendario</title>
</head>
<body class="bodyPopup" style="background-attachment: fixed;">

    <form id="frmRemarksConfig" runat="server">

        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <script language="javascript" type="text/javascript">

            function PageBase_Load() {
            }

            function GridRemarks_EndCallback(s, e) {
                if (s.IsEditing()) { }
                else {
                    if (s.cpActionRO != "REFRESH" && s.cpActionRO != "RELOAD") GridRemarksClient.PerformCallback("REFRESH");
                }
            }

            function GridRemarks_OnRowDblClick(s, e) {
                if (s.IsEditing()) {
                    s.UpdateEdit();
                }
                s.StartEditRow(e.visibleIndex);
            }

            function GridRemarks_FocusedRowChanged(s, e) {
                if (s.IsEditing()) {
                    s.UpdateEdit();
                }
            }

            function AddRemark(s, e) {
                var grid = ASPxClientGridView.Cast("GridRemarksClient");
                grid.AddNewRow();
            }

            function GridRemarks_CustomButtonClick(s, e) {
                var grid = ASPxClientGridView.Cast("GridRemarksClient");
                if (e.buttonID == "MoveDownButton") {
                    grid.PerformCallback("MoveDownButton")
                } else if (e.buttonID == "MoveUPButton") {
                    grid.PerformCallback("MoveUPButton");
                }
            }
        </script>

        <div>

            <table style="width: 100%; height: 100%" border="0">
                <tr>
                    <td colspan="2" style="padding-top: 5px; padding-bottom: 0px;" valign="top" height="20px">
                        <div class="panHeader2">
                            <span style="">
                                <asp:Label ID="LblTitle" runat="server" Text="Resaltes"></asp:Label></span>
                        </div>
                    </td>
                </tr>
                <tr style="height: 48px">
                    <td style="padding: 4px; padding-bottom: 10px;">
                        <img src="Images/SchedulerRemarks.png" />
                    </td>
                    <td align="left" valign="middle" style="padding: 4px; padding-bottom: 10px;">
                        <asp:Label ID="lblDescription" runat="server" CssClass="editTextFormat" Text="Cree las reglas para resaltar casillas del calendario dependiendo de su contenido."></asp:Label>
                    </td>
                </tr>
                <tr style="height: 200px; vertical-align: top">
                    <td colspan="2">
                        <div class="jsGrid">
                            <asp:Label ID="lblAddRemarkCaption" runat="server" CssClass="jsGridTitle" Text="Resaltes"></asp:Label>
                            <div class="jsgridButton">
                                <dx:ASPxButton ID="btnAddRemark" runat="server" AutoPostBack="False" CausesValidation="False" Text="Añadir" ToolTip="Añadir" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat">
                                    <Image Url="~/Base/Images/Grid/add.png"></Image>
                                    <ClientSideEvents Click="AddRemark" />
                                </dx:ASPxButton>
                            </div>
                        </div>
                        <div class="jsGridContent">
                            <dx:ASPxGridView ID="GridRemarks" runat="server" AutoGenerateColumns="False" ClientInstanceName="GridRemarksClient" KeyboardSupport="True" Width="100%">
                                <Settings ShowTitlePanel="False" VerticalScrollBarMode="Auto" UseFixedTableLayout="True" VerticalScrollableHeight="250" />
                                <ClientSideEvents EndCallback="GridRemarks_EndCallback" RowDblClick="GridRemarks_OnRowDblClick" FocusedRowChanged="GridRemarks_FocusedRowChanged" CustomButtonClick="GridRemarks_CustomButtonClick" />
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
                                            <asp:Button ID="btAccept" Text="${Button.Accept}" runat="server" OnClientClick="" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
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
            </table>

            <Local:MessageFrame ID="MessageFrame1" runat="server" />
        </div>
    </form>
</body>
</html>