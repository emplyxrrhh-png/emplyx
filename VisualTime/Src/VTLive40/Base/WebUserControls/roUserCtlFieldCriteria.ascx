<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.WebUserControls_roUserCtlFieldCriteria" CodeBehind="roUserCtlFieldCriteria.ascx.vb" %>

<table border="0" width="100%" style="padding: 20px; padding-top: 5px;" align="center">
    <tr>
        <td align="left" style="padding-left: 20px;">
            <table>
                <tr>
                    <td colspan="3">
                        <table>
                            <tr>
                                <td>
                                    <asp:Label ID="lblVisibilityDescCriteria" runat="server" Text="Solo los empleados que el campo de la ficha "></asp:Label></td>
                                <td style="padding-left: 10px;">
                                    <roWebControls:roComboBox ID="cmbVisibilityCriteria1" runat="server" EnableViewState="true" AutoResizeChildsWidth="True" ParentWidth="170px" ChildsVisible="7" ItemsRunAtServer="false" HiddenText="cmbVisibilityCriteria1_Text" HiddenValue="cmbVisibilityCriteria1_Value"></roWebControls:roComboBox>
                                    <input type="hidden" id="cmbVisibilityCriteria1_Text" runat="server" />
                                    <input type="hidden" id="cmbVisibilityCriteria1_Value" runat="server" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td>
                        <roWebControls:roComboBox ID="cmbVisibilityCriteria2" runat="server" EnableViewState="true" AutoResizeChildsWidth="True" ParentWidth="170px" ChildsVisible="7" ItemsRunAtServer="false" HiddenText="cmbVisibilityCriteria2_Text" HiddenValue="cmbVisibilityCriteria2_Value"></roWebControls:roComboBox>
                        <input type="hidden" id="cmbVisibilityCriteria2_Text" runat="server" />
                        <input type="hidden" id="cmbVisibilityCriteria2_Value" runat="server" />
                    </td>
                    <td>
                        <roWebControls:roComboBox ID="cmbVisibilityCriteria3" runat="server" EnableViewState="true" AutoResizeChildsWidth="True" ParentWidth="170px" ChildsVisible="7" ItemsRunAtServer="false" HiddenText="cmbVisibilityCriteria3_Text" HiddenValue="cmbVisibilityCriteria3_Value"></roWebControls:roComboBox>
                        <input type="hidden" id="cmbVisibilityCriteria3_Text" runat="server" />
                        <input type="hidden" id="cmbVisibilityCriteria3_Value" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td colspan="2">
                        <!-- Texte -->
                        <asp:Panel ID="panVValueTextBox" runat="server" Style="display: none;">
                            <input type="text" runat="server" id="txtVisibilityValue" class="textClass" maxlength="50" style="width: 420px;" convertcontrol="TextField" ccallowblank="true" cconchange="hasChanges(true);" />
                        </asp:Panel>
                        <!-- Numero -->
                        <asp:Panel ID="panVValueNumericBox" runat="server" Style="display: none;">
                            <input type="text" runat="server" id="numVisibilityValue" class="textClass" maxlength="5" style="width: 40px; text-align: right;" convertcontrol="NumberField" ccallowblank="true" ccdecimalprecision="0" ccallowdecimals="false" cconchange="hasChanges(true);" />
                        </asp:Panel>
                        <!-- Decimal -->
                        <asp:Panel ID="panVValueDecimalBox" runat="server" Style="display: none;">
                            <input type="text" runat="server" id="decVisibilityValue" class="textClass" maxlength="5" style="width: 40px; text-align: right;" convertcontrol="NumberField" ccallowblank="true" ccdecimalprecision="2" ccallowdecimals="true" cconchange="hasChanges(true);" />
                        </asp:Panel>
                        <!-- Date -->
                        <asp:Panel ID="panVValueMaskTextBox" runat="server" Style="display: none;">
                            <input type="text" id="mskVisibilityValue" runat="server" style="width: 75px;" class="textClass" convertcontrol="DatePicker" cconchange="hasChanges(true);" ccallowblank="true" />
                        </asp:Panel>
                        <!-- Time -->
                        <asp:Panel ID="panVValueMaskTextBoxTime" runat="server" Style="display: none;">
                            <input type="text" id="mskVisibilityValueTime" runat="server" cctime="false" convertcontrol="TextField" ccregex="/^([0-9]?[0-9]?[0-9]?[0-9]):([0-5][0-9])$/" ccmaxlength="7" style="width: 50px; text-align: right;" class="textClass x-form-text x-form-field" cconchange="hasChanges(true);" />
                        </asp:Panel>
                        <!-- Combo Llista -->
                        <asp:Panel ID="panVValueComboBox" runat="server" Style="display: none;">
                            <roWebControls:roComboBox ID="cmbVisibilityValue" runat="server" EnableViewState="true" AutoResizeChildsWidth="True" ParentWidth="170px" ChildsVisible="7" ItemsRunAtServer="false" HiddenText="cmbVisibilityValue_Text" HiddenValue="cmbVisibilityValue_Value"></roWebControls:roComboBox>
                            <input type="hidden" id="cmbVisibilityValue_Text" runat="server" />
                            <input type="hidden" id="cmbVisibilityValue_Value" runat="server" />
                        </asp:Panel>
                        <!-- Periodes Date -->
                        <asp:Panel ID="panVValuePeriods" runat="server" Style="display: none;">
                            <table>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblPeriodFrom" runat="server" Text="De "></asp:Label></td>
                                    <td style="width: 100px;">
                                        <input type="text" id="dtBegin" runat="server" style="width: 75px;" class="textClass x-form-text x-form-field" convertcontrol="DatePicker" cconchange="hasChanges(true);" ccallowblank="true" /></td>
                                    <td>
                                        <asp:Label ID="lblPeriodOf" runat="server" Text=" a "></asp:Label></td>
                                    <td style="width: 100px;">
                                        <input type="text" id="dtEnd" runat="server" style="width: 75px;" class="textClass x-form-text x-form-field" convertcontrol="DatePicker" cconchange="hasChanges(true);" ccallowblank="true" /></td>
                                </tr>
                            </table>
                        </asp:Panel>
                        <!-- Periodes Time -->
                        <asp:Panel ID="panVValueTimePeriods" runat="server" Style="display: none;">
                            <table>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblPeriodTimeFrom" runat="server" Text="De "></asp:Label></td>
                                    <td style="width: 65px; padding-left: 2px;">
                                        <input type="text" id="tBegin" runat="server" cctime="true" convertcontrol="TextField" style="width: 50px; text-align: right;" class="textClass x-form-text x-form-field" maxlength="5" cconchange="hasChanges(true);" /></td>
                                    <td>
                                        <asp:Label ID="lblPeriodTimeOf" runat="server" Text=" a "></asp:Label></td>
                                    <td style="width: 65px; padding-left: 2px;">
                                        <input type="text" id="tEnd" runat="server" cctime="true" convertcontrol="TextField" style="width: 50px; text-align: right;" class="textClass x-form-text x-form-field" maxlength="5" cconchange="hasChanges(true);" /></td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>