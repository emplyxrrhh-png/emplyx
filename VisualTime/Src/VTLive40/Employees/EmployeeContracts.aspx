<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Forms_EmployeeContracts" EnableEventValidation="false" CodeBehind="EmployeeContracts.aspx.vb" %>

<%@ Register Src="~/Employees/WebUserControls/ContractScheduleRules.ascx" TagName="frmContractScheduleRules" TagPrefix="roForms" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Contratos</title>
</head>
<body class="bodyPopup" style="background-attachment: fixed;">

    <form id="frmEmployeeContracts" runat="server">

        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />
        <script language="javascript" type="text/javascript">

            var deleteRowId = -1;

            function PageBase_Load() {
                ConvertControls('');
            }

            function GridContracts_BeginCallback(e, c) {

            }

            function GridContracts_EndCallback(s, e) {
                if (s.IsEditing()) { }
                else {
                    if (s.cpActionRO == "DELETE") {
                        GridContractsClient.PerformCallback("RELOAD");
                    }
                }
            }

            function GridContracts_OnRowDblClick(s, e) {
                if (s.IsEditing()) {
                    s.UpdateEdit();
                }
                s.StartEditRow(e.visibleIndex);
            }

            function GridContracts_FocusedRowChanged(s, e) {
                if (s.IsEditing()) {
                    s.UpdateEdit();
                }
            }

            //Agregar nueva fila en el grid de incidencias
            function AddNewContract(s, e) {
                var grid = ASPxClientGridView.Cast("GridContractsClient");
                grid.AddNewRow();
            }

            //Eliminar una incidencia en el datatable del servidor
            function DeleteAssignment(IdRow) {
                grid = ASPxClientGridView.Cast("GridContractsClient");
                grid.DeleteRow(IdRow);
            }

            function GridContracts_CustomButtonClick(s, e) {
                if (e.buttonID == "DeleteContractRow") {
                    if (e.visibleIndex > -1) {
                        deleteRowId = e.visibleIndex;
                        GridContractsClient.GetRowValues(e.visibleIndex, 'IDContract', RemoveContractRow);
                    }
                } else if (e.buttonID == "UpdateContractRow") {
                    UpdateContractRow();
                } else if (e.buttonID == "EditScheduleRowsRow") {
                    GridContractsClient.GetRowValues(e.visibleIndex, 'IDContract;IDLabAgree', frmShowContractScheduleRules_Show);
                }
            }

            function RemoveContractRow(values) {
                window.parent.showLoader(true);
                if (deleteRowId > -1) {
                    var contentUrl = "../Employees/DeleteEmployeeContractCaptcha.aspx";
                    PopupCaptcha_Client.SetContentUrl(contentUrl);
                    PopupCaptcha_Client.Show();
                }
            }

            function UpdateContractRow() {
                if (GridContractsClient.IsNewRowEditing() == false) {
                    window.parent.showLoader(true);
                    if (GridContractsClient.IsEditing()) {
                        var contentUrl = "../Employees/UpdateEmployeeContractCaptcha.aspx";
                        PopupCaptcha_Client.SetContentUrl(contentUrl);
                        PopupCaptcha_Client.Show();
                    }
                } else {
                    GridContractsClient.UpdateEdit();
                }
            }

            function RemoveContractRowExec() {
                GridContractsClient.DeleteRow(deleteRowId);
            }

            function UpdateContractRowExec() {
                GridContractsClient.UpdateEdit();
            }
        </script>

        <div class="popupWizardContent">
            <table style="width: 100%; height: 100%" border="0">
                <tr>
                    <td colspan="2" style="padding-top: 5px; padding-bottom: 0px;" valign="top" height="20px">
                        <div class="panHeader2">
                            <span style="">
                                <asp:Label ID="LblTitle" runat="server" Text="Contratos"></asp:Label></span>
                        </div>
                    </td>
                </tr>
                <tr style="height: 48px">
                    <td style="padding: 4px; padding-bottom: 10px;">
                        <img id="Img1" src="~/Base/Images/Employees/Contratos.png" runat="server" />
                    </td>
                    <td align="left" valign="middle" style="padding: 4px; padding-bottom: 10px;">
                        <asp:Label ID="lblDescription" runat="server" CssClass="editTextFormat" Text="Puede consultar y modificar los períodos de contrato del empleado seleccionado. Si un período de contrato está activo aparece indicado con un triángulo. Los períodos de contrato pasados o futuros están señalizados con un cuadrado."></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="vertical-align: top" align="center">
                        <div class="jsGrid">
                            <asp:Label ID="lblContractsCaption" runat="server" CssClass="jsGridTitle" Text="Contratos"></asp:Label>
                            <div class="jsgridButton">
                                <dx:ASPxButton ID="btnAddNewContract" runat="server" AutoPostBack="False" CausesValidation="False" Text="Añadir" ToolTip="Añadir nuevo contrato" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat">
                                    <Image Url="~/Base/Images/Grid/add.png"></Image>
                                    <ClientSideEvents Click="AddNewContract" />
                                </dx:ASPxButton>
                            </div>
                        </div>
                        <div class="jsGridContent">
                            <dx:ASPxGridView ID="GridContracts" runat="server" AutoGenerateColumns="False" ClientInstanceName="GridContractsClient" KeyboardSupport="True" Width="100%" ClientSideEvents-BeginCallback="GridContracts_BeginCallback">
                                <Settings ShowTitlePanel="False" VerticalScrollBarMode="Auto" UseFixedTableLayout="True" VerticalScrollableHeight="400" />
                                <ClientSideEvents CustomButtonClick="GridContracts_CustomButtonClick" EndCallback="GridContracts_EndCallback" RowDblClick="GridContracts_OnRowDblClick" FocusedRowChanged="GridContracts_FocusedRowChanged" />
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
                            <roForms:frmContractScheduleRules ID="frmContractScheduleRules" runat="server" />
                        </div>
                    </td>
                </tr>
            </table>
        </div>
        <div class="popupWizardButtons" align="right">
            <table>
                <tr>
                    <td align="right" valign="bottom" colspan="2" style="height: 20px; padding-right: 5px;">
                        <asp:UpdatePanel ID="updActions" runat="server">
                            <ContentTemplate>
                                <table>
                                    <tr>
                                        <td>
                                            <asp:Button ID="btClose" Text="${Button.Close}" runat="server" OnClientClick="Close(); return false;" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </div>

        <!-- POPUP CAPTCHA -->
        <dx:ASPxPopupControl ID="PopupCaptcha" runat="server" AllowDragging="False" CloseAction="None" Modal="True" ContentUrl="~/Employees/DeleteEmployeeContractCaptcha.aspx"
            PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter" Width="470px" Height="320px"
            ShowHeader="False" ScrollBars="Auto" ShowPageScrollbarWhenModal="True" ClientInstanceName="PopupCaptcha_Client" PopupAnimationType="None" BackColor="Transparent" ContentStyle-Paddings-Padding="0px" Border-BorderColor="Transparent" ShowShadow="false">
        </dx:ASPxPopupControl>

        <asp:HiddenField ID="hdnCanClose_PageBase" runat="server" />

        <Local:MessageFrame ID="MessageFrame1" runat="server" />
    </form>
</body>
</html>