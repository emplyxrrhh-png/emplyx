<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.TaskAssignments" EnableEventValidation="false" Culture="auto" UICulture="auto" EnableSessionState="True" CodeBehind="TaskEditAssignments.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Asignación de puestos a tareas</title>
</head>
<body class="bodyPopup" style="background-attachment: fixed;">

    <form id="frmAssignments" runat="server">

        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <script language="javascript" type="text/javascript">

            function PageBase_Load() {

                // Establecer selección actual en la grid 'grdEntries'
                var oGrd = $get('grdAssignments');
                var currow = $get('<%= hdnGrdAssignmentsSelectedRowIndex.ClientID %>').value;
                var curcol = $get('<%= hdnGrdAssignmentsSelectedColIndex.ClientID %>').value;
                if (currow != -1 && curcol != -1) {
                    roGridViewControl_Load(oGrd, currow, curcol, 'row-select');
                }

                ConvertControls('<%= divAssignments.ClientID %>');

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
                        <asp:Label ID="lblDescription" runat="server" CssClass="editTextFormat" Text="Asigne los puestos a la ${Task}."></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="right">
                        <div id="tbAddAssignment" class="btnFlat" runat="server">
                            <a href="javascript: void(0)" id="btAddAssignment" runat="server">
                                <span class="btnIconAdd"></span>
                                <asp:Label ID="lblAddAssignment" runat="server" Text="Añadir"></asp:Label>
                            </a>
                        </div>
                    </td>
                </tr>
                <tr style="height: 200px; vertical-align: top">
                    <td colspan="2">

                        <asp:UpdatePanel ID="updAssignments" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="btAddAssignment" EventName="ServerClick" />
                            </Triggers>
                            <ContentTemplate>

                                <div id="divAssignments" runat="server">

                                    <asp:ObjectDataSource ID="BlankData" runat="server" SelectMethod="_Select" TypeName="BlankDataObject" />
                                    <roWebControls:roGridViewControl ID="grdAssignments" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None"
                                        Width="100%" ScrollWidth="97%" Height="280px" Scrolling="Auto" DataSourceID="BlankData" AutoGenerateColumns="false"
                                        CssClass="yui-datatable-small-theme" DataKeyNames="IDTask, IDAssignment"
                                        CellHoverCssClass="row-over" CellSelectCssClass="row-select"
                                        RowSelectedControlID='<%# hdnGrdAssignmentsSelectedRowIndex.ClientID%>'
                                        ColSelectedControlID='<%# hdnGrdAssignmentsSelectedColIndex.ClientID%>'>
                                        <RowStyle CssClass="data-row" />
                                        <AlternatingRowStyle CssClass="alt-data-row" />
                                        <Columns>
                                            <asp:ButtonField Text="" CommandName="EditClick" Visible="False" />
                                            <asp:ButtonField Text="" CommandName="RemoveClick" Visible="False" />
                                            <asp:TemplateField HeaderText="Puesto">
                                                <ItemTemplate>
                                                    <asp:Label ID="Assignment_Label" runat="server"></asp:Label>
                                                    <asp:Label ID="IDAssignment_Label" runat="server" Text='<%# Eval("IDAssignment") %>' Visible="false"></asp:Label>
                                                    <asp:DropDownList ID="IDAssignment_DropDownList" Visible="false" OnSelectedIndexChanged="IDAssignment_DropDownList_SelectedIndexChanged" runat="server" CssClass="yui-datatable-moves-theme" Width="225" />
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Left" />
                                                <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:TemplateField>
                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <div class="fix-data-row-div">
                                                        <div class="fix-data-row-div-left">
                                                            <img id="imgEdit" src="~/Base/Images/Grid/edit.png" visible="true" runat="server" title="Editar" />
                                                            <img id="imgEditAccept" src="~/Base/Images/Grid/save.png" visible="false" runat="server" title="Aplicar" />
                                                        </div>
                                                        <div class="fix-data-row-div-right">
                                                            <img id="imgRemove" src="~/Base/Images/Grid/remove.png" visible="true" runat="server" title="Eliminar" />
                                                            <img id="imgEditCancel" src="~/Base/Images/Grid/cancel.png" visible="false" runat="server" title="Cancelar" />
                                                        </div>
                                                    </div>
                                                </ItemTemplate>
                                                <ItemStyle Width="40px" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="fix-data-row" />
                                                <HeaderStyle CssClass="fix-header" />
                                            </asp:TemplateField>
                                        </Columns>
                                    </roWebControls:roGridViewControl>
                                    <asp:HiddenField ID="hdnGrdAssignmentsSelectedRowIndex" runat="server" Value="-1" />
                                    <asp:HiddenField ID="hdnGrdAssignmentsSelectedColIndex" runat="server" Value="-1" />
                                    <asp:HiddenField ID="hdnGrdAssignmentsSelectedAssignment" runat="server" Value="-1" />
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>

                    <td align="right" colspan="2" style="height: 20px; padding-right: 5px;">
                        <asp:UpdatePanel ID="updActions" runat="server">
                            <ContentTemplate>
                                <table>
                                    <tr>
                                        <td>
                                            <asp:Button ID="btAccept" Text="${Button.Accept}" runat="server" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
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