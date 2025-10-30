<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.WebUserForms_frmEditLabAgreeRule" CodeBehind="frmEditLabAgreeRule.ascx.vb" %>

<input type="hidden" runat="server" id="hdnModeEdit" value="" />
<input type="hidden" runat="server" id="noRegs" value="" />
<input type="hidden" id="dateFormatValue" runat="server" value="" />

<!-- dif enum values -->
<input type='hidden' id='hdnDifUntilValue' value='<%= Robotics.Base.DTOs.LabAgreeRuleDefinitionDif.UntilValue %>' />
<input type='hidden' id='hdnDifValue' value='<%= Robotics.Base.DTOs.LabAgreeRuleDefinitionDif.Value %>' />
<input type='hidden' id='hdnDifAll' value='<%= Robotics.Base.DTOs.LabAgreeRuleDefinitionDif.All %>' />
<input type='hidden' id='hdnDifDiff' value='<%= Robotics.Base.DTOs.LabAgreeRuleDefinitionDif.Diff %>' />
<input type='hidden' id='hdnDifUserFieldUntilValue' value='<%= Robotics.Base.DTOs.LabAgreeRuleDefinitionDif.UserFieldUntilValue %>' />
<input type='hidden' id='hdnDifUserFieldValue' value='<%= Robotics.Base.DTOs.LabAgreeRuleDefinitionDif.UserFieldValue %>' />
<!-- dif language tags -->
<input type='hidden' id='hdnDifUntilValueText' value='<%= Me.Language.KeyWord("Dif.UntilValue") %>' />
<input type='hidden' id='hdnDifValueText' value='<%= Me.Language.KeyWord("Dif.Value") %>' />
<input type='hidden' id='hdnDifAllText' value='<%= Me.Language.KeyWord("Dif.All") %>' />
<input type='hidden' id='hdnDifDiffText' value='<%= Me.Language.KeyWord("Dif.Diff") %>' />
<input type='hidden' id='hdnDifUserFieldUntilValueText' value='<%= Me.Language.KeyWord("Dif.UserFieldUntilValue") %>' />
<input type='hidden' id='hdnDifUserFieldValueText' value='<%= Me.Language.KeyWord("Dif.UserFieldValue") %>' />

<div id="<%= Me.ClientID %>_frm" style="position: fixed; z-index: 9010; display: none; top: 50%; left: 50%; width: 1050px;">
    <div id="<%= Me.ClientID %>_BgS" style="position: absolute; top: 0; left: 0; display: none; z-index: 9009;"></div>
    <div class="bodyPopupExtended">
        <dx:ASPxCallbackPanel ID="ASPxRuleCallbackPanelContenido" runat="server" Width="100%" Height="100%" ClientInstanceName="ASPxRuleCallbackPanelContenidoClient">
            <SettingsLoadingPanel Enabled="false" />
            <ClientSideEvents EndCallback="ASPxRuleCallbackPanelContenidoClient_EndCallBack" />
            <PanelCollection>
                <dx:PanelContent ID="PanelContent1" runat="server">
                    <div id="divContentPanels" style="padding-right: 20px">
                        <!-- Panell General -->
                        <div id="div00" class="contentPanel" runat="server" name="menuPanel">
                            <!-- Este div es el header -->
                            <div class="panBottomMargin">
                                <div class="panHeader2 panBottomMargin">
                                    <span class="panelTitleSpan">
                                        <asp:Label runat="server" ID="lblGeneralTitle" Text="General"></asp:Label>
                                    </span>
                                </div>
                                <!-- La descripción es opcional -->
                                <div class="panelHeaderContent">
                                    <div class="panelDescriptionImage">
                                        <img alt="" src="<%=Me.Page.ResolveUrl("~/LabAgree/Images/LabAgree.png")%>" />
                                    </div>
                                    <div class="panelDescriptionText">
                                        <asp:Label ID="lblGeneralDescription" runat="server" Text="Definición de una regla de convenio"></asp:Label>
                                    </div>
                                </div>
                            </div>

                            <!-- Este div es un formulario -->
                            <div class="panBottomMargin">
                                <div class="divRow">
                                    <div class="divRowDescription">
                                        <asp:Label ID="lblNameDescription" runat="server" Text="Nombre de la regla de convenio"></asp:Label>
                                    </div>
                                    <asp:Label ID="lblName" runat="server" Text="Nombre:" CssClass="labelForm"></asp:Label>
                                    <div class="componentForm">
                                        <dx:ASPxTextBox ID="txtName" runat="server" ClientInstanceName="txtName_Client" Width="100%" NullText="_____" MaxLength="50">
                                            <ClientSideEvents Validation="LengthValidation" TextChanged="" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                            <ValidationSettings SetFocusOnError="True">
                                                <RequiredField IsRequired="True" ErrorText="(*)" />
                                            </ValidationSettings>
                                        </dx:ASPxTextBox>
                                    </div>
                                </div>

                                <div class="divRow">
                                    <div class="divRowDescription">
                                        <asp:Label ID="lblDescriptionDesc" runat="server" Text="Descripción de la regla de convenio"></asp:Label>
                                    </div>
                                    <asp:Label ID="lblDescription" runat="server" Text="Descripción:" CssClass="labelForm"></asp:Label>
                                    <div class="componentForm">
                                        <dx:ASPxMemo ID="txtDescription" runat="server" Rows="4" Width="100%" Height="40">
                                            <ClientSideEvents TextChanged="function(s,e){ }" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                        </dx:ASPxMemo>
                                    </div>
                                </div>

                                <div class="divRow">
                                    <div class="divRowDescription">
                                        <asp:Label ID="lblInitialDateDesc" runat="server" Text="Fecha inicial de validez de la regla"></asp:Label>
                                    </div>
                                    <asp:Label ID="lblInitialDate" runat="server" Text="Fecha inicial:" CssClass="labelForm"></asp:Label>
                                    <div class="componentForm">
                                        <dx:ASPxDateEdit ID="txtInitialDate" PopupVerticalAlign="TopSides" PopupHorizontalAlign="OutsideRight" runat="server" AllowNull="false">
                                            <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                        </dx:ASPxDateEdit>
                                    </div>
                                </div>

                                <div class="divRow">
                                    <div class="divRowDescription">
                                        <asp:Label ID="lblEndDateDesc" runat="server" Text="Fecha final de validez de la regla"></asp:Label>
                                    </div>
                                    <asp:Label ID="lblEndDate" runat="server" Text="Fecha final:" CssClass="labelForm"></asp:Label>
                                    <div class="componentForm">
                                        <dx:ASPxDateEdit ID="txtEndDate" PopupVerticalAlign="TopSides" PopupHorizontalAlign="OutsideRight" runat="server" AllowNull="true">
                                            <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                        </dx:ASPxDateEdit>
                                    </div>
                                </div>
                            </div>

                            <!-- Este div es el header -->
                            <div class="panBottomMargin">
                                <div class="panHeader2 panBottomMargin">
                                    <span class="panelTitleSpan">
                                        <asp:Label runat="server" ID="lblLabAgreeRulesDefinitionTitle" Text="Definición"></asp:Label>
                                    </span>
                                </div>
                                <!-- La descripción es opcional -->
                                <div class="panelHeaderContent">
                                    <div class="panelDescriptionImage">
                                        <img alt="" src="<%=Me.Page.ResolveUrl("~/LabAgree/Images/LabAgreeRules.png")%>" />
                                    </div>
                                    <div class="panelDescriptionText">
                                        <asp:Label ID="lblLabAgreeRules" runat="server" Text="Las reglas de convenios permiten comparar, dentro de un período completo, un convenio con otro o con un valor fijo, con el fin de mover o copiar a una justificación todo él, la diferencia o parte"></asp:Label>
                                    </div>
                                </div>
                            </div>

                            <!-- Este div es un formulario -->
                            <div class="panBottomMargin">
                                <div class="divRow">
                                    <roUserControls:roGroupBox ID="RoGroupBox1" runat="server">
                                        <Content>
                                            <input type="hidden" id="hdnIDType" runat="server" />
                                            <div class="divRow">
                                                <asp:Label ID="lblLARuleIf" runat="server" Text="Si hay" CssClass="labelForm midWidth"></asp:Label>
                                                <div class="componentFormWithoutSize">
                                                    <dx:ASPxComboBox runat="server" ID="cmbMainAccrual" Width="200px" ClientInstanceName="cmbMainAccrual_Client" NullText="_____">
                                                        <ClientSideEvents Validation="SelectedItemRequiered" SelectedIndexChanged="function(s,e){changeValueType(s.GetSelectedItem().value.split('_')[1]);setScheduleVisibility(s.GetSelectedItem().value.split('_')[2]);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                        <ValidationSettings SetFocusOnError="True" ErrorDisplayMode="None">
                                                            <RequiredField IsRequired="True" ErrorText="(*)" />
                                                        </ValidationSettings>
                                                    </dx:ASPxComboBox>
                                                </div>
                                                <div class="componentFormWithoutSize">
                                                    <dx:ASPxComboBox runat="server" ID="cmbComparation" Width="200px" NullText="_____">
                                                        <ClientSideEvents Validation="SelectedItemRequiered" SelectedIndexChanged="function(s,e){;}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                        <ValidationSettings SetFocusOnError="True" ErrorDisplayMode="None">
                                                            <RequiredField IsRequired="True" ErrorText="(*)" />
                                                        </ValidationSettings>
                                                    </dx:ASPxComboBox>
                                                </div>
                                                <div class="componentFormWithoutSize">
                                                    <dx:ASPxComboBox runat="server" ID="cmbValueTypes" Width="200px" ClientInstanceName="cmbValueTypes_Client" NullText="_____">
                                                        <ClientSideEvents Validation="SelectedItemRequiered" SelectedIndexChanged="function(s,e){ShowTheValue(s.GetSelectedIndex());;}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                        <ValidationSettings SetFocusOnError="True" ErrorDisplayMode="None">
                                                            <RequiredField IsRequired="True" ErrorText="(*)" />
                                                        </ValidationSettings>
                                                    </dx:ASPxComboBox>
                                                </div>
                                                <div class="componentFormWithoutSize" style="padding-top: 3px;">
                                                    <div id="divValueTime" class="componentFormWithoutSize" style="margin-top: -5px; display: none;">
                                                        <div id="divValueTime1" style="display: none;">
                                                            <table>
                                                                <tr>
                                                                    <td>
                                                                        <dx:ASPxTextBox ID="txtValueTime" runat="server" MaxLength="8">
                                                                            <ClientSideEvents TextChanged="function(s,e){ ;}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                            <MaskSettings Mask="<0..9999>.<000..999>" IncludeLiterals="DecimalSymbol" />
                                                                            <ValidationSettings SetFocusOnError="True" ErrorDisplayMode="None"></ValidationSettings>
                                                                        </dx:ASPxTextBox>
                                                                    </td>
                                                                    <td style="padding-left: 2px;">
                                                                        <asp:Label ID="lblValueTime" runat="server" Text="horas"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </div>
                                                        <div id="divValueOnce1" style="display: none;">
                                                            <table>
                                                                <tr>
                                                                    <td>
                                                                        <dx:ASPxTextBox ID="txtValueOnce" runat="server" MaxLength="3">
                                                                            <ClientSideEvents TextChanged="function(s,e){ ;}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                            <MaskSettings Mask="<0..999>" />
                                                                            <ValidationSettings SetFocusOnError="True" ErrorDisplayMode="None"></ValidationSettings>
                                                                        </dx:ASPxTextBox>
                                                                    </td>
                                                                    <td style="padding-left: 2px;">
                                                                        <asp:Label ID="lblValueOnce" runat="server" Text="veces"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </div>
                                                    </div>
                                                    <div id="divValueUserFields" class="componentFormWithoutSize" style="display: none;">
                                                        <div id="divValueUserFieldsH" style="display: none;">
                                                            <dx:ASPxComboBox runat="server" ID="cmbValueUserFieldsH" Width="200px">
                                                                <ClientSideEvents SelectedIndexChanged="function(s,e){;}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                <ValidationSettings ErrorDisplayMode="None" />
                                                            </dx:ASPxComboBox>
                                                        </div>
                                                        <div id="divValueUserFieldsO" style="display: none;">
                                                            <dx:ASPxComboBox runat="server" ID="cmbValueUserFieldsO" Width="200px">
                                                                <ClientSideEvents SelectedIndexChanged="function(s,e){;}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                <ValidationSettings ErrorDisplayMode="None" />
                                                            </dx:ASPxComboBox>
                                                        </div>
                                                    </div>
                                                    <div id="divValueConcepts" class="componentFormWithoutSize" style="display: none;">
                                                        <dx:ASPxComboBox runat="server" ID="cmbValueConcepts" Width="200px">
                                                            <ClientSideEvents SelectedIndexChanged="function(s,e){;}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                            <ValidationSettings ErrorDisplayMode="None" />
                                                        </dx:ASPxComboBox>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="divRow">
                                                <asp:Label ID="lblLARuleThen" runat="server" Text="entonces" CssClass="labelForm midWidth"></asp:Label>
                                                <div class="componentFormWithoutSize">
                                                    <dx:ASPxComboBox runat="server" ID="cmbAction" Width="200px" NullText="_____">
                                                        <ClientSideEvents Validation="SelectedItemRequiered" SelectedIndexChanged="function(s,e){;}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                        <ValidationSettings SetFocusOnError="True" ErrorDisplayMode="None">
                                                            <RequiredField IsRequired="True" />
                                                        </ValidationSettings>
                                                    </dx:ASPxComboBox>
                                                </div>
                                                <div class="componentFormWithoutSize">
                                                    <dx:ASPxComboBox runat="server" ID="cmbDif" Width="200px" ClientInstanceName="cmbDif_Client" NullText="_____">
                                                        <ClientSideEvents Validation="SelectedItemRequiered" SelectedIndexChanged="function(s,e){changeComboDif(s.GetSelectedItem().value);;}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                        <ValidationSettings SetFocusOnError="True" ErrorDisplayMode="None">
                                                            <RequiredField IsRequired="True" />
                                                        </ValidationSettings>
                                                    </dx:ASPxComboBox>
                                                </div>
                                                <div class="componentFormWithoutSize" style="padding-top: 3px;">
                                                    <div id="divUntilTime" style="margin-top: -5px; display: none;">
                                                        <div id="divUntilTime1" style="display: none;">
                                                            <table>
                                                                <tr>
                                                                    <td>
                                                                        <dx:ASPxTextBox ID="txtUntilTime" runat="server" MaxLength="7">
                                                                            <ClientSideEvents TextChanged="function(s,e){ ;}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                            <MaskSettings Mask="<0..999>.<000..999>" />
                                                                            <ValidationSettings ErrorDisplayMode="None"></ValidationSettings>
                                                                        </dx:ASPxTextBox>
                                                                    </td>
                                                                    <td style="padding-left: 2px;">
                                                                        <asp:Label ID="lblUntilTime" runat="server" Text="horas"></asp:Label></td>
                                                                </tr>
                                                            </table>
                                                        </div>
                                                        <div id="divUntilOnce1" style="display: none;">
                                                            <table>
                                                                <tr>
                                                                    <td>
                                                                        <dx:ASPxTextBox ID="txtUntilOnce" runat="server" MaxLength="3">
                                                                            <ClientSideEvents TextChanged="function(s,e){ ;}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                            <MaskSettings Mask="<0..999>" />
                                                                            <ValidationSettings ErrorDisplayMode="None"></ValidationSettings>
                                                                        </dx:ASPxTextBox>
                                                                    </td>
                                                                    <td style="padding-left: 2px;">
                                                                        <asp:Label ID="lblUntilOnce" runat="server" Text="veces"></asp:Label></td>
                                                                </tr>
                                                            </table>
                                                        </div>
                                                    </div>
                                                    <div id="divUntilUserFields" style="display: none;">
                                                        <div id="divUntilUserFieldsH" style="display: none;">
                                                            <dx:ASPxComboBox runat="server" ID="cmbUntilUserFieldsH" Width="200px">
                                                                <ClientSideEvents SelectedIndexChanged="function(s,e){;}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                            </dx:ASPxComboBox>
                                                        </div>
                                                        <div id="divUntilUserFieldsO" style="display: none;">
                                                            <dx:ASPxComboBox runat="server" ID="cmbUntilUserFieldsO" Width="200px">
                                                                <ClientSideEvents SelectedIndexChanged="function(s,e){;}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                            </dx:ASPxComboBox>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="divRow">
                                                <asp:Label ID="lblLARuleInto" runat="server" Text="en" CssClass="labelForm midWidth"></asp:Label>
                                                <div class="componentFormWithoutSize">
                                                    <dx:ASPxComboBox runat="server" ID="cmbDestiCause" Width="200px" NullText="_____">
                                                        <ClientSideEvents Validation="SelectedItemRequiered" SelectedIndexChanged="function(s,e){;}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                        <ValidationSettings SetFocusOnError="True" ErrorDisplayMode="None">
                                                            <RequiredField IsRequired="True" />
                                                        </ValidationSettings>
                                                    </dx:ASPxComboBox>
                                                </div>
                                            </div>
                                        </Content>
                                    </roUserControls:roGroupBox>
                                </div>

                                <div class="divRow">
                                    <roUserControls:roGroupBox ID="gBox1" runat="server">
                                        <Content>
                                            <roUserControls:roOptSchedule runat="server" ID="optSchedule1"></roUserControls:roOptSchedule>
                                        </Content>
                                    </roUserControls:roGroupBox>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div style="width: 100%;">
                        <table border="0" style="width: 100%;">
                            <tr>
                                <td>&nbsp;</td>
                                <td style="width: 110px;" align="right">
                                    <dx:ASPxButton ID="btnOk" runat="server" AutoPostBack="False" CausesValidation="False" Text="Aceptar" ToolTip="Aceptar" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                        <ClientSideEvents Click="function(s,e){ frmEditLabAgreeRule_Save(); }" />
                                    </dx:ASPxButton>
                                </td>
                                <td style="width: 110px;" align="left">
                                    <dx:ASPxButton ID="btnCancel" runat="server" AutoPostBack="False" CausesValidation="False" Text="Cancelar" ToolTip="Cancelar" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                        <ClientSideEvents Click="function(s,e){ frmEditLabAgreeRule_Close(); }" />
                                    </dx:ASPxButton>
                                </td>
                            </tr>
                        </table>
                    </div>
                </dx:PanelContent>
            </PanelCollection>
        </dx:ASPxCallbackPanel>
    </div>
</div>