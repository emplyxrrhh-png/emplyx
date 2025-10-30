<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.EmployeeAssignments" EnableEventValidation="false" Culture="auto" UICulture="auto" EnableSessionState="True" CodeBehind="EmployeeEditAssignments.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Asignación de puestos a empleados</title>
</head>
<body class="bodyPopup" style="background-attachment: fixed;">

    <form id="frmAssignments" runat="server">

        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <script language="javascript" type="text/javascript">

            function PageBase_Load() {

            }

            function KeyPressFunction(e) {
                tecla = (document.all) ? e.keyCode : e.which;
                if (tecla == 13) {
                    RunAccept();
                    //var button = $get('LoginObject_Login_btAccept_btButton');
                    //ButtonClick(button);
                    return false;
                }
            }

            function RunAccept() {
                //TODO:..
            }

            function GridAssignments_BeginCallback(e, c) {

            }

            function GridAssignments_EndCallback(s, e) {
                if (s.IsEditing()) {

                }
            }

            function GridAssignments_OnRowDblClick(s, e) {
                if (s.IsEditing()) {
                    s.UpdateEdit();
                }
                s.StartEditRow(e.visibleIndex);
            }

            function GridAssignments_FocusedRowChanged(s, e) {
                if (s.IsEditing()) {
                    s.UpdateEdit();
                }
            }

            //Agregar nueva fila en el grid de incidencias
            function AddNewAssignment(s, e) {
                var grid = ASPxClientGridView.Cast("GridAssignmentsClient");
                grid.AddNewRow();
            }

            //Eliminar una incidencia en el datatable del servidor
            function DeleteAssignment(IdRow) {
                grid = ASPxClientGridView.Cast("GridAssignmentsClient");
                grid.DeleteRow(IdRow);
            }
        </script>

        <div>

            <table style="width: 100%; height: 100%" border="0">
                <tr>
                    <td colspan="2" style="padding-top: 5px; padding-bottom: 0px;" valign="top" height="20px">
                        <div class="panHeader2">
                            <span style="">
                                <asp:Label ID="lblTitle" runat="server" Text="Dotación {1} para el {2}"></asp:Label></span>
                        </div>
                    </td>
                </tr>
                <tr style="height: 48px">
                    <td style="padding: 4px; padding-bottom: 10px;">
                        <img src="../Assignments/Images/Assignments48.png" alt="" />
                    </td>
                    <td align="left" valign="middle" style="padding: 4px; padding-bottom: 10px;">
                        <asp:Label ID="lblDescription" runat="server" CssClass="editTextFormat" Text="Asigne los puestos el ${Employee} y su idoneidad."></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="center" style="vertical-align: top">
                        <div class="jsGrid">
                            <asp:Label ID="lblAssignmentsCaption" runat="server" CssClass="jsGridTitle" Text="Puestos"></asp:Label>
                            <div class="jsgridButton">
                                <dx:ASPxButton ID="btnAddNewAssignment" runat="server" AutoPostBack="False" CausesValidation="False" Text="Añadir" ToolTip="Añadir nuevo puesto" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat">
                                    <Image Url="~/Base/Images/Grid/add.png"></Image>
                                    <ClientSideEvents Click="AddNewAssignment" />
                                </dx:ASPxButton>
                            </div>
                        </div>
                        <div class="jsGridContent">
                            <dx:ASPxGridView ID="GridAssignments" runat="server" AutoGenerateColumns="False" ClientInstanceName="GridAssignmentsClient" KeyboardSupport="True" Width="100%" ClientSideEvents-BeginCallback="GridAssignments_BeginCallback">
                                <Settings ShowTitlePanel="False" VerticalScrollBarMode="Auto" UseFixedTableLayout="True" VerticalScrollableHeight="150" />
                                <ClientSideEvents EndCallback="GridAssignments_EndCallback" RowDblClick="GridAssignments_OnRowDblClick" FocusedRowChanged="GridAssignments_FocusedRowChanged" />
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
                                            <asp:Button ID="btAccept" Text="${Button.Accept}" runat="server" OnClientClick="" CssClass="btnFlat btnFlatBlack btnFlatAsp" /></td>
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