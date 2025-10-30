<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Scheduler_AddCoverage" EnableEventValidation="false" Culture="auto" UICulture="auto" EnableSessionState="True" CodeBehind="AddCoverage.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Configuración cobertura</title>
</head>
<body class="bodyPopup" style="background-attachment: fixed;">

    <form id="frmAddCoverage" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <script language="javascript" type="text/javascript">

            function PageBase_Load() {

                // Establecer selección actual en la grid 'grdEmployees'
                var oGrd = $get('grdEmployees');
                var currow = $get('<%= hdnGrdEmployeesSelectedRowIndex.ClientID %>').value;
                var curcol = $get('<%= hdnGrdEmployeesSelectedColIndex.ClientID %>').value;
                if (currow != -1 && curcol != -1) {
                    roGridViewControl_Load(oGrd, currow, curcol, 'row-select');
                }

                ConvertControls('<%= divEmployees.ClientID %>');

                var hdnIncorrectAssignment = document.getElementById('<%= hdnIncorrectAssignment.ClientID %>');
                if (hdnIncorrectAssignment != null) {
                    if (hdnIncorrectAssignment.value == '1') {
                        ButtonClick($get('<%= btIncorrectAssignment.ClientID %>'));
                    }
                }

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
                                <asp:Label ID="lblTitle" runat="server" Text="Cubrir a {1} el día {2} cómo {3} y ${Shift} {4}"></asp:Label></span>
                        </div>
                    </td>
                </tr>
                <tr style="height: 48px;">
                    <td style="padding: 4px; padding-bottom: 10px; width: 50px;">
                        <img src="Images/DailyCoverageCoverage_48.png" />
                    </td>
                    <td align="left" valign="middle" style="padding: 4px; padding-bottom: 10px;">
                        <asp:Label ID="lblDescription" runat="server" CssClass="editTextFormat" Text="Indique el ${Employee} que realizará la cobertura."></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="left">
                        <asp:Label ID="lblGridTitle" runat="server" CssClass="editTextFormat" Text="Seleccione el ${Employee} que realizará la cobertura:"></asp:Label>
                    </td>
                </tr>
                <tr style="height: 200px; vertical-align: top">
                    <td colspan="2">

                        <asp:UpdatePanel ID="updEmployees" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <Triggers>
                            </Triggers>
                            <ContentTemplate>

                                <div id="divEmployees" runat="server">

                                    <asp:ObjectDataSource ID="BlankData" runat="server" SelectMethod="_Select" TypeName="BlankDataObject" />
                                    <roWebControls:roGridViewControl ID="grdEmployees" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None"
                                        Width="100%" ScrollWidth="97%" Height="300px" Scrolling="Auto" DataSourceID="BlankData" AutoGenerateColumns="false"
                                        CssClass="yui-datatable-small-theme" DataKeyNames="IDEmployee"
                                        CellHoverCssClass="row-over" CellSelectCssClass="row-select"
                                        RowSelectedControlID='<%# hdnGrdEmployeesSelectedRowIndex.ClientID%>'
                                        ColSelectedControlID='<%# hdnGrdEmployeesSelectedColIndex.ClientID%>'>
                                        <RowStyle CssClass="data-row" />
                                        <AlternatingRowStyle CssClass="alt-data-row" />
                                        <Columns>
                                            <asp:ButtonField Text="" CommandName="SelectClick" Visible="False" />
                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <div class="fix-data-row-div" style="width: 20px;">
                                                        <div class="fix-data-row-div-left">
                                                            <img id="imgSelect" src="~/Base/Images/Grid/Select.png" visible="true" runat="server" title="Seleccionar" />
                                                            <img id="imgSelected" src="~/Base/Images/Grid/Selected.png" visible="false" runat="server" title="Seleccionado" />
                                                        </div>
                                                    </div>
                                                </ItemTemplate>
                                                <ItemStyle Width="20px" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="fix-data-row" />
                                                <HeaderStyle CssClass="fix-header" />
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="EmployeeName" HeaderText="${Employee}"
                                                ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left" ItemStyle-CssClass="yui-datatable-moves-theme" />
                                            <asp:TemplateField HeaderText="${Shift}">
                                                <ItemTemplate>
                                                    <asp:Label ID="Shift_Label" runat="server"></asp:Label>
                                                    <asp:Label ID="IDShift_Label" runat="server" Text='<%# Eval("IDShift1") %>' Visible="false"></asp:Label>
                                                    <asp:DropDownList ID="IDShift_DropDownList" Visible="false" runat="server" CssClass="yui-datatable-moves-theme" Width="125" />
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Left" />
                                                <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="AssignmentName" HeaderText="${Assignment}"
                                                ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left" ItemStyle-CssClass="yui-datatable-moves-theme" />
                                            <asp:BoundField DataField="GroupName" HeaderText="${Group}"
                                                ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left" ItemStyle-CssClass="yui-datatable-moves-theme" />
                                            <asp:BoundField DataField="Points" HeaderText="Puntos"
                                                ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left" ItemStyle-CssClass="yui-datatable-moves-theme" />
                                            <asp:BoundField DataField="Cost" HeaderText="Coste"
                                                ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left" ItemStyle-CssClass="yui-datatable-moves-theme" />
                                            <asp:BoundField DataField="Suitability" HeaderText="Idoneidad"
                                                ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ItemStyle-CssClass="yui-datatable-moves-theme" />
                                            <asp:BoundField DataField="Coverage" HeaderText="Cobertura"
                                                ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ItemStyle-CssClass="yui-datatable-moves-theme" />
                                            <asp:BoundField DataField="ConceptValue" HeaderText="Saldo"
                                                ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ItemStyle-CssClass="yui-datatable-moves-theme" />
                                        </Columns>
                                    </roWebControls:roGridViewControl>
                                    <asp:HiddenField ID="hdnGrdEmployeesSelectedRowIndex" runat="server" Value="-1" />
                                    <asp:HiddenField ID="hdnGrdEmployeesSelectedColIndex" runat="server" Value="-1" />
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
                                            <asp:Button ID="btAccept" Text="${Button.Accept}" runat="server" OnClientClick="" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                        </td>
                                        <td>
                                            <asp:Button ID="btCancel" Text="${Button.Cancel}" runat="server" OnClientClick="Close(); return false;" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                        </td>
                                    </tr>
                                </table>
                                <asp:HiddenField ID="hdnCanClose_PageBase" runat="server" />
                                <asp:HiddenField ID="hdnMustRefresh_PageBase" Value="0" runat="server" />
                                <input type="hidden" id="hdnLockedMsg" value="" runat="server" />
                                <input type="hidden" id="hdnLockedEmployee" value="" runat="server" />
                                <input type="hidden" id="hdnLockedDay" value="" runat="server" />

                                <input type="hidden" id="hdnIncorrectAssignment" value="" runat="server" />
                                <asp:Button ID="btIncorrectAssignment" Style="display: none;" runat="server" />
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