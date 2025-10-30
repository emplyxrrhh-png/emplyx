<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.UserFieldHistory" EnableEventValidation="false" Culture="auto" UICulture="auto" CodeBehind="UserFieldHistory.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Histórico valores campo de la ficha</title>
</head>
<body class="bodyPopup" style="background-attachment: fixed;">

    <form id="frmUserFieldHistory" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <script language="javascript" type="text/javascript">

            function GridHistoric_BeginCallback(e, c) {
            }

            function GridHistoric_EndCallback(s, e) {
                if (s.IsEditing()) {

                }
            }

            function GridHistoric_OnRowDblClick(s, e) {
                if (s.IsEditing()) {
                    s.UpdateEdit();
                }
                s.StartEditRow(e.visibleIndex);
            }

            function ShowLoading() {

                window.parent.frames['ifPrincipal'].showLoadingGrid(true);
            }

            function endRequestHandler() {
                window.parent.frames["ifPrincipal"].showLoadingGrid(false);
                //hidePopupLoader();
            }

            function GridHistoric_FocusedRowChanged(s, e) {
                if (s.IsEditing()) {
                    s.UpdateEdit();
                }
            }

            //Agregar nueva fila en el grid de incidencias
            function AddNewHistoric(s, e) {
                var grid = ASPxClientGridView.Cast("GridHistoricClient");
                grid.AddNewRow();
            }

            //Eliminar una incidencia en el datatable del servidor
            function DeleteIncidence(IdRow) {
                grid = ASPxClientGridView.Cast("GridHistoricClient");
                grid.DeleteRow(IdRow);
            }
        </script>

        <div>

            <table style="width: 100%; height: 100%" border="0">
                <tr>
                    <td style="padding-top: 5px; padding-bottom: 0px;" valign="top" height="20px">
                        <div class="panHeader2">
                            <span style="">
                                <asp:Label ID="LblTitle" runat="server" Text="Histórico valores"></asp:Label></span>
                        </div>
                    </td>
                </tr>

                <tr style="height: 215px; vertical-align: top">
                    <td>
                        <div class="jsGrid">
                            <asp:Label ID="lblHistoricCaption" runat="server" CssClass="jsGridTitle" Text="Historico"></asp:Label>
                            <div class="jsgridButton">
                                <dx:ASPxButton ID="btnAddNewHistoric" runat="server" AutoPostBack="False" CausesValidation="False" Text="Añadir" ToolTip="Añadir nueva entrada" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat">
                                    <Image Url="~/Base/Images/Grid/add.png"></Image>
                                    <ClientSideEvents Click="AddNewHistoric" />
                                </dx:ASPxButton>
                            </div>
                        </div>
                        <div class="jsGridContent">
                            <dx:ASPxGridView ID="GridHistoric" runat="server" AutoGenerateColumns="False" ClientInstanceName="GridHistoricClient" KeyboardSupport="True" Width="100%" ClientSideEvents-BeginCallback="GridHistoric_BeginCallback">
                                <Settings ShowTitlePanel="False" VerticalScrollBarMode="Auto" UseFixedTableLayout="True" VerticalScrollableHeight="150" />
                                <ClientSideEvents EndCallback="GridHistoric_EndCallback" RowDblClick="GridHistoric_OnRowDblClick" FocusedRowChanged="GridHistoric_FocusedRowChanged" />
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

                    <td align="right" style="height: 20px; padding-right: 5px;">
                        <asp:UpdatePanel ID="updActions" runat="server">
                            <ContentTemplate>
                                <table>
                                    <tr>
                                        <td>
                                            <asp:Button ID="btAccept" Text="${Button.Accept}" runat="server" OnClientClick="ShowLoading();" CssClass="btnFlat btnFlatBlack btnFlatAsp" /></td>
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