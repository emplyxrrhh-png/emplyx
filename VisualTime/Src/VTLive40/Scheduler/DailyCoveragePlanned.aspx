<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Scheduler_DailyCoveragePlanned" EnableEventValidation="false" Culture="auto" UICulture="auto" EnableSessionState="True" CodeBehind="DailyCoveragePlanned.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Planificación dotación</title>
</head>
<body class="bodyPopup" style="background-attachment: fixed;">

    <form id="frmDailyCoveragePlanned" runat="server">

        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <script language="javascript" type="text/javascript">

            function PageBase_Load() {

                // Establecer selección actual en la grid 'grdEntries'
                var oGrd = $get('grdCoverages');
                var currow = $get('<%= hdnGrdCoveragesSelectedRowIndex.ClientID %>').value;
                var curcol = $get('<%= hdnGrdCoveragesSelectedColIndex.ClientID %>').value;
                if (currow != -1 && curcol != -1) {
                    roGridViewControl_Load(oGrd, currow, curcol, 'row-select');
                }

                ConvertControls('<%= divCoverages.ClientID %>');

                var curIsFloating = $get('<%= hdnIsFloatingMonday.ClientID %>').value;

                if (curIsFloating == "1") {
                    document.getElementById('tdStartShiftMonday').style.display = '';
                }
                else {
                    document.getElementById('tdStartShiftMonday').style.display = 'none';
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

            function HideStartShift(tdID, hdnIsFloatingID) {

                document.getElementById(tdID).style.display = 'none';
                document.getElementById(hdnIsFloatingID).value = '0';

            }

            function ShowStartShift(tdID, txtID, cmbID, cnTextID, cnValueID, StartShiftDef, hdnIsFloatingID) {

                document.getElementById(tdID).style.display = '';

                if (StartShiftDef != null && StartShiftDef != '') {
                    switch (StartShiftDef.substr(6, 2)) {
                        case '29':
                            txtStartFloatingMondayClient.SetDate(new Date(1899, 12, 29, StartShiftDef.substr(8, 2), StartShiftDef.substr(10, 2), 0));
                            roCB_setValue('0', cmbID + '_ComboBoxLabel', cnTextID, cnValueID);
                            break;
                        case '30':
                            txtStartFloatingMondayClient.SetDate(new Date(1899, 12, 30, StartShiftDef.substr(8, 2), StartShiftDef.substr(10, 2), 0));
                            roCB_setValue('1', cmbID + '_ComboBoxLabel', cnTextID, cnValueID);
                            break;
                        case '31':
                            txtStartFloatingMondayClient.SetDate(new Date(1899, 12, 31, StartShiftDef.substr(8, 2), StartShiftDef.substr(10, 2), 0));
                            roCB_setValue('2', cmbID + '_ComboBoxLabel', cnTextID, cnValueID);
                            break;
                    }
                } else {
                    txtStartFloatingMondayClient.SetDate(new Date(1899, 12, 30, 0, 0, 0));
                    roCB_setValue('1', cmbID + '_ComboBoxLabel', cnTextID, cnValueID);
                }

                document.getElementById(hdnIsFloatingID).value = '1';
            }

            function RunAccept() {
                //TODO:..
            }

            function ShowDailyCoverageAddPlan(_IDAssignment) {

                ShowDailyCoverageEmployees();

            }

            function ShowDailyCoverageEmployees() {

                $find('mpxDailyCoverageEmployeesBehavior').show();

            }

            function HideDailyCoverageEmployees() {
                hidePopup('mpxDailyCoverageEmployeesBehavior');
            }

            function Refresh() {
                var DataChanged = document.getElementById('<%= hdnDataChanged.ClientID %>').value;
                if (DataChanged == '1') parent.RefreshScreen('DailyCoveragePlanned', '');
            }

            function RefreshGrid() {
                ButtonClick($get('<%= btRefresh.ClientID %>'));
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
                    <td style="padding: 4px; padding-bottom: 10px; width: 50px;">
                        <img src="Images/DailyCoveragePlanned_48.png" />
                    </td>
                    <td align="left" valign="middle" style="padding: 4px; padding-bottom: 10px;">
                        <asp:Label ID="lblDescription" runat="server" CssClass="editTextFormat" Text="Asigne a cada ${Assignment} los ${Employees} que cubrirán cada dotación."></asp:Label>
                    </td>
                </tr>
            </table>
            <table style="width: 100%; height: 100%" border="0">
                <tr style="height: 100px; vertical-align: top">
                    <td>
                        <asp:UpdatePanel ID="updCoverages" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <Triggers>
                            </Triggers>
                            <ContentTemplate>

                                <table style="width: 100%; height: 100%;" cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td align="right" valign="top">
                                            <asp:Button runat="server" ID="btRefresh" Style="display: none; z-index: 995;" />
                                            <a onclick="RefreshGrid();" title="Actualizar" class="icoRefresh icoClass" href="javascript: void(0)"></a>
                                        </td>
                                        <td align="left">
                                            <div>
                                                <asp:Label ID="Label1" runat="server" CssClass="editTextFormat" Text="${Employees} asignados:"></asp:Label>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="vertical-align: top">

                                            <div id="divCoverages" style="" runat="server">

                                                <asp:ObjectDataSource ID="BlankData" runat="server" SelectMethod="_Select" TypeName="BlankDataObject" />
                                                <roWebControls:roGridViewControl ID="grdCoverages" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None"
                                                    ScrollWidth="97%" Height="315px" Scrolling="Auto" DataSourceID="BlankData" AutoGenerateColumns="false"
                                                    CssClass="yui-datatable-small-theme" DataKeyNames="IDGroup, Date, IDAssignment"
                                                    CellHoverCssClass="row-over" CellSelectCssClass="row-select"
                                                    RowSelectedControlID='<%# hdnGrdCoveragesSelectedRowIndex.ClientID%>'
                                                    ColSelectedControlID='<%# hdnGrdCoveragesSelectedColIndex.ClientID%>'>
                                                    <RowStyle CssClass="data-row" />
                                                    <AlternatingRowStyle CssClass="alt-data-row" />
                                                    <Columns>
                                                        <asp:ButtonField Text="" CommandName="SelectClick" Visible="False" />
                                                        <asp:ButtonField Text="" CommandName="ActionClick" Visible="False" />
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
                                                        <asp:BoundField DataField="AssignmentName" HeaderText="${Assignment}"
                                                            ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="250" ItemStyle-CssClass="yui-datatable-moves-theme" />
                                                        <asp:BoundField DataField="ExpectedCoverage" HeaderText="Teórica"
                                                            ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="50" ItemStyle-CssClass="yui-datatable-moves-theme" />
                                                        <asp:BoundField DataField="PlannedCoverage" HeaderText="Planificada"
                                                            ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="50" ItemStyle-CssClass="yui-datatable-moves-theme" />
                                                        <asp:TemplateField>
                                                            <ItemTemplate>
                                                                <div class="fix-data-row-div">
                                                                    <div class="fix-data-row-div-left">
                                                                        <img id="imgEdit" src="~/Base/Images/Grid/edit.png" visible="false" runat="server" title="Editar" />
                                                                        <img id="imgEditAccept" src="~/Base/Images/Grid/save.png" visible="false" runat="server" title="Aplicar" />
                                                                    </div>
                                                                    <div class="fix-data-row-div-right" style="display: none;">
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
                                                <asp:HiddenField ID="hdnGrdCoveragesSelectedRowIndex" runat="server" Value="-1" />
                                                <asp:HiddenField ID="hdnGrdCoveragesSelectedColIndex" runat="server" Value="-1" />

                                                <asp:HiddenField ID="hdnIDAssignmentSel" runat="server" Value="-1" />
                                            </div>
                                        </td>
                                        <td style="vertical-align: top">
                                            <asp:UpdatePanel ID="updCoveragesDetail" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                                <Triggers>
                                                </Triggers>
                                                <ContentTemplate>

                                                    <div id="divCoveragesDetail" style="width: 100%;" runat="server">

                                                        <roWebControls:roGridViewControl ID="grdCoverageDetail" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None"
                                                            Height="310px" ScrollWidth="97%" Scrolling="Auto" DataSourceID="BlankData" AutoGenerateColumns="false"
                                                            CssClass="yui-datatable-small-theme" DataKeyNames="IDEmployee"
                                                            CellHoverCssClass="row-over" CellSelectCssClass="row-select"
                                                            RowSelectedControlID='<%# hdnGrdCoverageDetailSelectedRowIndex.ClientID%>'
                                                            ColSelectedControlID='<%# hdnGrdCoverageDetailSelectedColIndex.ClientID%>'>
                                                            <RowStyle CssClass="data-row" />
                                                            <AlternatingRowStyle CssClass="alt-data-row" />
                                                            <Columns>
                                                                <asp:BoundField DataField="EmployeeName" HeaderText="${Employee}"
                                                                    ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="150" ItemStyle-CssClass="yui-datatable-moves-theme" />
                                                                <asp:BoundField DataField="ShiftName" HeaderText="${Shift}"
                                                                    ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="100" ItemStyle-CssClass="yui-datatable-moves-theme" />
                                                                <asp:BoundField DataField="GroupName" HeaderText="${Group}"
                                                                    ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="100" ItemStyle-CssClass="yui-datatable-moves-theme" />
                                                                <asp:BoundField DataField="Points" HeaderText="Puntos"
                                                                    ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="60" ItemStyle-CssClass="yui-datatable-moves-theme" />
                                                                <asp:BoundField DataField="Cost" HeaderText="Coste"
                                                                    ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="50" ItemStyle-CssClass="yui-datatable-moves-theme" />
                                                                <asp:BoundField DataField="Suitability" HeaderText="Idoneidad"
                                                                    ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="50" ItemStyle-CssClass="yui-datatable-moves-theme" />
                                                                <asp:BoundField DataField="Coverage" HeaderText="Cobertura"
                                                                    ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="50" ItemStyle-CssClass="yui-datatable-moves-theme" />
                                                                <asp:BoundField DataField="ConceptValue" HeaderText="Saldo"
                                                                    ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="50" ItemStyle-CssClass="yui-datatable-moves-theme" />
                                                            </Columns>
                                                        </roWebControls:roGridViewControl>
                                                        <asp:HiddenField ID="hdnGrdCoverageDetailSelectedRowIndex" runat="server" Value="-1" />
                                                        <asp:HiddenField ID="hdnGrdCoverageDetailSelectedColIndex" runat="server" Value="-1" />
                                                    </div>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </td>
                                    </tr>
                                </table>
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
                                            <asp:Button ID="btClose" Text="${Button.Close}" runat="server" OnClientClick="Refresh(); Close(); return false;" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
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

            <asp:UpdatePanel ID="updEmployees" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                <Triggers>
                </Triggers>
                <ContentTemplate>

                    <ajaxToolkit:ModalPopupExtender ID="mpxDailyCoverageEmployees" runat="server" BehaviorID="mpxDailyCoverageEmployeesBehavior"
                        TargetControlID="hiddenTargetControlForDailyCoverageEmployeesPopup"
                        PopupControlID="divDailyCoverageEmployees"
                        DropShadow="False"
                        EnableViewState="true" BackgroundCssClass="mpxStyleBack">
                    </ajaxToolkit:ModalPopupExtender>
                    <asp:Button runat="server" ID="hiddenTargetControlForDailyCoverageEmployeesPopup" Style="display: none; z-index: 995;" />
                    <div id="divDailyCoverageEmployees" style="height: 480px; width: 950px;">

                        <roWebControls:roPopupFrameV2 ID="ContentFrame" runat="server" Height="480px" Width="950px">
                            <FrameContentTemplate>

                                <table style="width: 100%; height: 100%" border="0" class="bodyPopup">
                                    <tr>
                                        <td colspan="2" style="padding-top: 5px; padding-bottom: 0px;" valign="top" height="20px">
                                            <div class="panHeader2">
                                                <span style="">
                                                    <asp:Label ID="lblDailyCoverageEmployeesTitle" runat="server" Text="Agregar ${Employees} a la dotación de {1}"></asp:Label></span>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" align="left" style="padding-top: 10px;">
                                            <asp:Label ID="lblShiftAssignmed" runat="server" CssClass="editTextFormat" Text="Indicar ${Shift} para los ${Employees} seleccionados:"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <roWebControls:roComboBox ID="cmbShiftsMonday" runat="server" ItemsRunAtServer="False" ParentWidth="250px" EnableViewState="true" AutoResizeChildsWidth="False" HiddenText="cmbShiftsMonday_Text" HiddenValue="cmbShiftsMonday_Value" />
                                            <asp:HiddenField ID="cmbShiftsMonday_Text" runat="server" />
                                            <asp:HiddenField ID="cmbShiftsMonday_Value" runat="server" />
                                            <input type="hidden" id="hdnIsFloatingMonday" value="0" runat="server" />
                                        </td>
                                        <td id="tdStartShiftMonday" style="display: none;">
                                            <table border="0" cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td style="width: 75px; text-align: right;">
                                                        <dx:ASPxTimeEdit ID="txtStartFloatingMonday" EditFormatString="HH:mm" EditFormat="Custom" runat="server" ClientInstanceName="txtStartFloatingMondayClient" Width="85"></dx:ASPxTimeEdit>
                                                    </td>
                                                    <td style="width: 175px;" align="right">
                                                        <roWebControls:roComboBox ID="cmbStartFloatingMonday" runat="server" EnableViewState="true" AutoResizeChildsWidth="True" ParentWidth="175px" ChildsVisible="4" ItemsRunAtServer="false" HiddenText="cmbStartFloatingMonday_Text" HiddenValue="cmbStartFloatingMonday_Value"></roWebControls:roComboBox>
                                                        <input type="hidden" id="cmbStartFloatingMonday_Text" runat="server" />
                                                        <input type="hidden" id="cmbStartFloatingMonday_Value" runat="server" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" align="left" style="padding-top: 10px;">
                                            <asp:Label ID="lblGridTitle" runat="server" CssClass="editTextFormat" Text="${Employees} disponibles:"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr style="height: 200px; vertical-align: top">
                                        <td colspan="2">

                                            <div id="divEmployees" runat="server">

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
                                                                        <img id="imgSelect" src="~/Base/Images/Grid/Select-check.png" visible="true" runat="server" title="Seleccionar" />
                                                                        <img id="imgSelected" src="~/Base/Images/Grid/Selected-check.png" visible="false" runat="server" title="Seleccionado" />
                                                                    </div>
                                                                </div>
                                                            </ItemTemplate>
                                                            <ItemStyle Width="20px" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="fix-data-row" />
                                                            <HeaderStyle CssClass="fix-header" />
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="EmployeeName" HeaderText="${Employee}"
                                                            ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left" ItemStyle-CssClass="yui-datatable-moves-theme" />
                                                        <asp:TemplateField HeaderText="${Shift} actual">
                                                            <ItemTemplate>
                                                                <asp:Label ID="Shift_Label" runat="server"></asp:Label>
                                                                <asp:Label ID="IDShift_Label" runat="server" Text='<%# Eval("IDShift1") %>' Visible="false"></asp:Label>
                                                                <asp:DropDownList ID="IDShift_DropDownList" Visible="false" runat="server" CssClass="yui-datatable-moves-theme" Width="110" />
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="Left" />
                                                            <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="AssignmentName" HeaderText="${Assignment} actual"
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
                                        </td>
                                    </tr>
                                    <tr>

                                        <td align="right" colspan="2" style="height: 20px; padding-right: 5px;">
                                            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                                <ContentTemplate>
                                                    <table>
                                                        <tr>
                                                            <td>
                                                                <asp:Button ID="btAccept" Text="${Button.Accept}" runat="server" OnClientClick="" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                                            </td>
                                                            <td>
                                                                <asp:Button ID="btCancel" Text="${Button.Cancel}" runat="server" OnClientClick="HideDailyCoverageEmployees(); return false;" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                    <input type="hidden" id="hdnLockedMsg" value="" runat="server" />
                                                    <input type="hidden" id="hdnLockedEmployee" value="" runat="server" />
                                                    <input type="hidden" id="hdnLockedDay" value="" runat="server" />
                                                    <input type="hidden" id="hdnDataChanged" value="0" runat="server" />
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </td>
                                    </tr>
                                </table>
                            </FrameContentTemplate>
                        </roWebControls:roPopupFrameV2>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </form>
</body>
</html>