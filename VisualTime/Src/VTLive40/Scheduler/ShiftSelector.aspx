<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.ShiftSelector" CodeBehind="ShiftSelector.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Selector ${Shifts}</title>
</head>
<body class="bodyPopup">
    <form id="frmShiftsSelector" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <div>

            <script language="javascript" type="text/javascript">

                var _generalUpdateProgressDiv;
                var _generalBackgroundDiv;

                function PageBase_Load() {

                    ConvertControls();

                    // Cerrar el formulario si está pendiente de cerrar.
                    CloseIfNeeded();

                    ShowStartShiftIfNeeded();

                }

                function PageBase_Unload() {
                    var IDShiftSelected = $get('<%= Me.hdnIDShiftSelected.ClientID %>').value;
                var StartShiftSelected = $get('<%= Me.hdnStartShift.ClientID %>').value;
                var IDAssignmentSelected = $get('<%= Me.hdnIDAssignmentSelected.ClientID %>').value;
                    if (IDShiftSelected != '0') {
                        try {
                            parent.ShiftChange(IDShiftSelected, StartShiftSelected, IDAssignmentSelected);
                        } catch (e) { }
                    }
                }

                function CloseIfNeeded() {
                    // Cerrar el formulario si está pendiente de cerrar.
                    var _CanClose = $get('<%= Me.hdnCanClose.ClientID %>');
                    if (_CanClose.value == '1') Close();
                }

                function ShowStartShiftIfNeeded() {
                    var _StartShift = $get('<%= Me.hdnStartShift.ClientID %>');
                    if (_StartShift.value != '') {
                        SetStartFloating(_StartShift.value);
                        $find('mpxStartShiftBehavior').show();
                        txtStartFloatingClient.Focus();
                    }
                    else {
                        ShowAssignmentIfNeeded();
                    }

                }

                function ShowAssignmentIfNeeded() {
                    var _ShowAssignmentSelector = $get('<%= Me.hdnShowAssignmentSelector.ClientID %>');
                    if (_ShowAssignmentSelector.value == '1') {
                        $find('mpxAssignmentBehavior').show();
                        return true;
                    }
                    else {
                        return false;
                    }

                }

                function Close() {
                    try {
                        parent.HideExternalForm();
                    } catch (e) { }
                }

                function SetStartFloating(StartFloating) {

                    var cmbID = '<%= cmbStartFloating.ClientID %>';
                var cnTextID = '<%= cmbStartFloating_Text.ClientID %>';
                var cnValueID = '<%= cmbStartFloating_Value.ClientID %>';

                    if (StartFloating != null && StartFloating != '') {

                        switch (StartFloating.substr(6, 2)) {
                            case '29':
                                txtStartFloatingClient.SetDate(new Date(1899, 12, 29, StartFloating.substr(8, 2), StartFloating.substr(10, 2), 0));
                                roCB_setValue('0', cmbID + '_ComboBoxLabel', cnTextID, cnValueID);
                                break;
                            case '30':
                                txtStartFloatingClient.SetDate(new Date(1899, 12, 30, StartFloating.substr(8, 2), StartFloating.substr(10, 2), 0));
                                roCB_setValue('1', cmbID + '_ComboBoxLabel', cnTextID, cnValueID);
                                break;
                            case '31':
                                txtStartFloatingClient.SetDate(new Date(1899, 12, 31, StartFloating.substr(8, 2), StartFloating.substr(10, 2), 0));
                                roCB_setValue('2', cmbID + '_ComboBoxLabel', cnTextID, cnValueID);
                                break;
                        }
                    }
                    else {
                        txtStartFloatingClient.SetDate(new Date(1899, 12, 30, 0, 0, 0));
                        roCB_setValue('1', cmbID + '_ComboBoxLabel', cnTextID, cnValueID);
                    }

                }

                function StartShiftOK() {

                    if (CheckConvertControls('<%= divStartShift.ClientID %>') == true) {

                    if (document.getElementById('<%= cmbStartFloating_Value.ClientID %>').value != '') {

                        var StartShiftSelected = '';

                        switch (document.getElementById('<%= cmbStartFloating_Value.ClientID %>').value) {
                            case '0':
                                StartShiftSelected = '18991229';
                                break;
                            case '1':
                                StartShiftSelected = '18991230';
                                break;
                            case '2':
                                StartShiftSelected = '18991231';
                                break;
                        }

                        var strStartFloating = txtStartFloatingClient.GetDate();
                        StartShiftSelected = StartShiftSelected + strStartFloating.format2Time().replace(':', '');

                        var _StartShift = $get('<%= Me.hdnStartShift.ClientID %>');
                            _StartShift.value = StartShiftSelected;

                            hidePopup('mpxStartShiftBehavior');

                            if (ShowAssignmentIfNeeded() == false) {
                                Close();
                            }

                        }
                    }

                }

                function StartShiftCancel() {

                    hidePopup('mpxStartShiftBehavior');
                    $get('<%= Me.hdnStartShift.ClientID %>').value = '';
                $get('<%= Me.hdnIDShiftSelected.ClientID %>').value = '';
                $get('<%= Me.hdnIDAssignmentSelected.ClientID %>').value = '';

                    Close();

                }

                function AssignmentOK() {

                    if (CheckConvertControls('<%= divAssignment.ClientID %>') == true) {

                    if (cmbAssignmentSelectorNewClient.GetText() != '') {

                        var _IDAssignmentSelected = $get('<%= Me.hdnIDAssignmentSelected.ClientID %>');
                            _IDAssignmentSelected.value = cmbAssignmentSelectorNewClient.GetValue();

                            hidePopup('mpxAssignmentBehavior');

                            Close();

                        }
                    }

                }

                function AssignmentCancel() {

                    hidePopup('mpxAssignmentBehavior');
                    $get('<%= Me.hdnStartShift.ClientID %>').value = '';
                $get('<%= Me.hdnIDShiftSelected.ClientID %>').value = '';
                $get('<%= Me.hdnIDAssignmentSelected.ClientID %>').value = '';

                    Close();

                }

                function MinMaxStartFloatingSelector(Minimize) {

                    var div = document.getElementById('<%= Me.divStartShift.ClientID %>');
                    var tdStartFloatingCmb = document.getElementById('tdStartFloatingCmb');
                    var aStartFloating = document.getElementById('aStartFloating');

                    var bolMinimize;

                    if (Minimize == null) {
                        if (aStartFloating.className == 'icoTbMaximize') {
                            bolMinimize = false;
                        }
                        else {
                            bolMinimize = true;
                        }
                    }
                    else {
                        bolMinimize = Minimize;
                    }

                    if (bolMinimize == false) {
                        div.style.width = '460px';
                        tdStartFloatingCmb.style.display = '';
                        aStartFloating.className = 'icoTbMinimize';
                    }
                    else {
                        div.style.width = '240px';
                        tdStartFloatingCmb.style.display = 'none';
                        aStartFloating.className = 'icoTbMaximize';
                    }

                }

                function KeyPressFunction(e, FuncExec, noFunct) {
                    tecla = (document.all) ? e.keyCode : e.which;
                    if (tecla == 13) {
                        if (noFunct) {
                            return false;
                        }
                        else {
                            if (FuncExec != null && FuncExec != '') {
                                eval(FuncExec);
                            }
                            else {
                                //RunAccept();
                            }
                            return false;
                        }
                    }
                }
            </script>

            <asp:UpdatePanel ID="updShiftSelector" runat="server">
                <ContentTemplate>

                    <table cellpadding="0" cellspacing="0">
                        <tr>
                            <td colspan="2">
                                <div class="panHeader2">
                                    <asp:Label ID="lblShiftSelectorTitle" Text="Asignar" CssClass="panHeaderLabel" runat="server" />
                                </div>
                            </td>
                        </tr>
                        <tr style="padding-top: 5px">
                            <td colspan="2">
                                <asp:Label ID="lblShiftSelectorInfo" CssClass="SectionDescription" Text="Seleccione el ${Shift} que desea asignar al ${Employee} en este día en concreto." runat="server"></asp:Label>
                                <asp:Label ID="lblComplementaryInfo" CssClass="SectionDescription" ForeColor="Red" Text="Los horarios con horas complementarias deben planificarse desde la pantalla de calendario." runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" style="padding-top: 10px">
                                <asp:Label ID="lblShiftSelectorInfo2" Text="${Shifts} disponibles:" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" style="padding-top: 5px">
                                <asp:Panel ID="panShifts" Height="260" Width="100%" ScrollBars="Vertical" runat="server">
                                    <asp:TreeView ID="treeShifts" ShowCheckBoxes="None" SelectedNodeStyle-ImageUrl="" SelectedNodeStyle-Font-Bold="true" runat="server" Style="background-color: Transparent;"></asp:TreeView>
                                </asp:Panel>
                            </td>
                        </tr>
                        <tr>
                            <td align="left">
                                <asp:Label ID="lblError" Text="" CssClass="errorText" runat="server" Visible="false"></asp:Label>
                            </td>
                            <td align="right">
                                <table>
                                    <tr>
                                        <td>
                                            <asp:Button ID="btAccept" Text="${Button.Accept}" runat="server" OnClientClick="" CssClass="btnFlat btnFlatBlack btnFlatAsp" /></td>
                                        <td>
                                            <asp:Button ID="btCancel" Text="${Button.Cancel}" runat="server" OnClientClick="Close(); return false;" CssClass="btnFlat btnFlatBlack btnFlatAsp" /></td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>

                    <asp:HiddenField ID="hdnCanClose" runat="server" />
                    <asp:HiddenField ID="hdnIDShiftSelected" Value="0" runat="server" />
                    <asp:HiddenField ID="hdnStartShift" Value="" runat="server" />
                    <asp:HiddenField ID="hdnShowAssignmentSelector" Value="0" runat="server" />
                    <asp:HiddenField ID="hdnIDAssignmentSelected" Value="" runat="server" />

                    <ajaxToolkit:ModalPopupExtender ID="mpxStartShift" runat="server" BehaviorID="mpxStartShiftBehavior"
                        TargetControlID="hiddenTargetControlForStartShiftPopup"
                        PopupControlID="divStartShift"
                        DropShadow="False"
                        PopupDragHandleControlID="panStartShiftDragHandle"
                        EnableViewState="true" BackgroundCssClass="mpxStyleBack">
                    </ajaxToolkit:ModalPopupExtender>
                    <asp:Button runat="server" ID="hiddenTargetControlForStartShiftPopup" Style="display: none; z-index: 995;" />
                    <div id="divStartShift" runat="server" style="height: 150px; width: 240px;">
                        <roWebControls:roPopupFrameV2 ID="ropfStartShift" runat="server" BorderStyle="None" Height="150px" Width="100%">
                            <FrameContentTemplate>
                                <table width="100%" cellspacing="0" class="bodyPopup">
                                    <tr id="panStartShiftDragHandle" style="cursor: move; height: 20px;">
                                        <td colspan="3">
                                            <div class="panHeader2">
                                                <span style="">
                                                    <asp:Label ID="lblStartShiftTitle" runat="server" Text="Hora de inicio del ${Shift} flotante" /></span>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3" style="padding: 15px; padding-bottom: 0px;">
                                            <asp:Label ID="lblStartShiftInfo" runat="server" Text="Este formulario le permite indicar la hora de inicio del ${Shift} flotante." CssClass="editTextFormat" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3" style="padding: 15px;" align="center">
                                            <table>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lblStartShiftShift" runat="server" Text="Hora de inicio :" /></td>
                                                    <td style="width: 85px; text-align: right;">
                                                        <dx:ASPxTimeEdit ID="txtStartFloating" Width="85" EditFormatString="HH:mm" EditFormat="Custom" runat="server" ClientInstanceName="txtStartFloatingClient">
                                                            <ClientSideEvents KeyPress="function(s,e){ return KeyPressFunction(event, 'StartShiftOK();'); }" />
                                                        </dx:ASPxTimeEdit>
                                                    </td>
                                                    <td id="tdStartFloatingCmb" style="width: 212px; display: none;" align="right">
                                                        <roWebControls:roComboBox ID="cmbStartFloating" runat="server" EnableViewState="true" AutoResizeChildsWidth="True" ParentWidth="175px" ChildsVisible="4" ItemsRunAtServer="false" HiddenText="cmbStartFloating_Text" HiddenValue="cmbStartFloating_Value"></roWebControls:roComboBox>
                                                        <input type="hidden" id="cmbStartFloating_Text" runat="server" />
                                                        <input type="hidden" id="cmbStartFloating_Value" runat="server" />
                                                    </td>
                                                    <td>
                                                        <a id="aStartFloating" onclick="MinMaxStartFloatingSelector();" class="icoTbMaximize" href="javascript: void(0);"></a>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3" align="center">
                                            <table>
                                                <tr>
                                                    <td>
                                                        <asp:Button ID="btStartShiftOK" Text="${Button.Accept}" runat="server" OnClientClick="StartShiftOK(); return false;" CssClass="btnFlat btnFlatBlack btnFlatAsp" /></td>
                                                    <td>
                                                        <asp:Button ID="btStartShiftCancel" Text="${Button.Cancel}" runat="server" OnClientClick="StartShiftCancel(); return false;" CssClass="btnFlat btnFlatBlack btnFlatAsp" /></td>
                                                </tr>
                                            </table>
                                            <input type="hidden" id="hdnStartShiftResponseFunction" value="" />
                                        </td>
                                    </tr>
                                </table>
                            </FrameContentTemplate>
                        </roWebControls:roPopupFrameV2>
                    </div>

                    <ajaxToolkit:ModalPopupExtender ID="mpxAssignment" runat="server" BehaviorID="mpxAssignmentBehavior"
                        TargetControlID="hiddenTargetControlForAssignmentPopup"
                        PopupControlID="divAssignment"
                        DropShadow="False"
                        PopupDragHandleControlID="panAssignmentDragHandle"
                        EnableViewState="true" BackgroundCssClass="mpxStyleBack">
                    </ajaxToolkit:ModalPopupExtender>
                    <asp:Button runat="server" ID="hiddenTargetControlForAssignmentPopup" Style="display: none; z-index: 995;" />
                    <div id="divAssignment" runat="server" style="display: ; height: 150px; width: 340px;">
                        <roWebControls:roPopupFrameV2 ID="ropfAssignment" runat="server" BorderStyle="None" Height="150px" Width="100%">
                            <FrameContentTemplate>
                                <table width="100%" cellspacing="0" class="bodyPopup">
                                    <tr id="panAssignmentDragHandle" style="cursor: move; height: 20px;">
                                        <td colspan="3">
                                            <div class="panHeader2">
                                                <span style="">
                                                    <asp:Label ID="lblAssignmentTitle" runat="server" Text="Puesto que cubrirá el ${Employee} con el ${Shift}" /></span>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3" style="padding: 15px; padding-bottom: 0px;">
                                            <asp:Label ID="lblAssignmentInfo" runat="server" Text="Este formulario le permite indicar el puesto que cubrirá el ${Employee} con el ${Shift} seleccionado." CssClass="editTextFormat" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3" style="padding: 15px;" align="center">
                                            <table>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lblAssignmentShift" runat="server" Text="Puesto :" /></td>
                                                    <td id="tdStartFloatingCmb" style="width: 212px; display: ;" align="left">
                                                        <dx:ASPxComboBox runat="server" ID="cmbAssignmentSelectorNew" Width="212px" ClientInstanceName="cmbAssignmentSelectorNewClient" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3" align="center">
                                            <table>
                                                <tr>
                                                    <td>
                                                        <asp:Button ID="btAssignmentOK" Text="${Button.Accept}" runat="server" OnClientClick="AssignmentOK(); return false;" CssClass="btnFlat btnFlatBlack btnFlatAsp" /></td>
                                                    <td>
                                                        <asp:Button ID="btAssignmentCancel" Text="${Button.Cancel}" runat="server" OnClientClick="AssignmentCancel(); return false;" CssClass="btnFlat btnFlatBlack btnFlatAsp" /></td>
                                                </tr>
                                            </table>
                                            <input type="hidden" id="hdnAssignmentResponseFunction" value="" />
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