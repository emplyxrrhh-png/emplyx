<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.WebUserForms_frmCompositions" CodeBehind="frmCompositions.ascx.vb" %>

<!-- Div flotant compositions -->
<div id="<%= Me.ClientID %>_frm" style="position: fixed; *position: absolute; z-index: 999; display: none; top: 50%; left: 50%; *width: 600px;">
    <div id="<%= Me.ClientID %>_BgS" style="position: absolute; top: 0; left: 0; display: none; z-index: 998;"></div>
    <div class="bodyPopupExtended" style="">
        <!-- Controls Popup Aqui -->
        <div id="divComposition" runat="server" style="">
            <div style="width: 100%; height: 100%; background-color: White;" class="bodyPopup">
                <!-- Tags idioma per passar a js -->
                <input type="hidden" id="header1" value="<%= Me.Language.Translate("gridHeaderCause",DefaultScope) %>" />
                <input type="hidden" id="header2" value="<%= Me.Language.Translate("gridHeaderOperation",DefaultScope) %>" />

                <input type="hidden" id="msgErrorNoConditions" value="<%= Me.Language.Translate("msgErrorNoConditions",DefaultScope) %>" />
                <table style="width: 100%; padding-top: 5px; padding-bottom: 10px;" border="0">
                    <tr>
                        <td colspan="2">
                            <div class="panHeader2">
                                <span style="">
                                    <asp:Label runat="server" ID="lblCompTit" Text="Composición"></asp:Label></span>
                                <dx:ASPxHiddenField ID="conceptCompositionData" runat="server" ClientInstanceName="conceptCompositionDataClient" />
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" style="padding: 10px;">
                            <table border="0" style="width: 100%; text-align: left;">
                                <tr>
                                    <td colspan="2">
                                        <div style="clear: both; padding-bottom: 10px;">
                                            <div class="panHeader2">
                                                <span style="">
                                                    <asp:Label runat="server" ID="lblValue" Text="Valor"></asp:Label></span>
                                            </div>
                                        </div>
                                        <div style="clear: both; padding-bottom: 10px;">
                                            <div style="float: left; width: 26%">
                                                <dx:ASPxRadioButton GroupName="ValueGroup" ID="ckCause" runat="server" ClientInstanceName="ckCause_client" Text="Justificación">
                                                    <ClientSideEvents CheckedChanged="ckCause_client_CheckedChanged" />
                                                </dx:ASPxRadioButton>
                                            </div>
                                            <div style="float: left; width: calc(74% - 4px)">
                                                <dx:ASPxComboBox ID="cmbCause" runat="server" ClientInstanceName="cmbCauseClient" Width="250px">
                                                    <ValidationSettings ErrorDisplayMode="None" />
                                                </dx:ASPxComboBox>
                                            </div>

                                            <div style="float: left; width: 26%; clear: both">
                                                <dx:ASPxRadioButton GroupName="ValueGroup" ID="ckValue" runat="server" ClientInstanceName="ckValue_client" Text="Valor justificado como">
                                                    <ClientSideEvents CheckedChanged="ckValue_client_CheckedChanged" />
                                                </dx:ASPxRadioButton>
                                            </div>
                                            <div style="float: left; width: calc(74% - 4px)">
                                                <dx:ASPxComboBox ID="cmbValueDailyCause" runat="server" ClientInstanceName="cmbValueDailyCauseClient" Width="250px">
                                                    <ValidationSettings ErrorDisplayMode="None" />
                                                </dx:ASPxComboBox>

                                                <dx:ASPxComboBox ID="cmbValueCustomCause" runat="server" ClientInstanceName="cmbValueCustomCauseClient" Width="250px">
                                                    <ValidationSettings ErrorDisplayMode="None" />
                                                </dx:ASPxComboBox>
                                            </div>

                                            <div style="float: left; width: 26%; clear: both">
                                                <dx:ASPxRadioButton GroupName="ValueGroup" ID="ckDayContainsCause" runat="server" ClientInstanceName="ckDayContainsCause_client" Text="Día que contenga la justificación">
                                                    <ClientSideEvents CheckedChanged="ckDayContainsCause_client_CheckedChanged" />
                                                </dx:ASPxRadioButton>
                                            </div>
                                            <div style="float: left; width: calc(74% - 4px)">
                                                <dx:ASPxComboBox ID="cmbDayContainsCause" runat="server" ClientInstanceName="cmbDayContainsCauseClient" Width="250px">
                                                    <ValidationSettings ErrorDisplayMode="None" />
                                                </dx:ASPxComboBox>
                                            </div>

                                            <div style="float: left; width: 26%; clear: both">
                                                <dx:ASPxRadioButton GroupName="ValueGroup" ID="ckShift" runat="server" ClientInstanceName="ckShift_client" Text="Día planificado con el horario">
                                                    <ClientSideEvents CheckedChanged="ckShift_client_CheckedChanged" />
                                                </dx:ASPxRadioButton>
                                            </div>
                                            <div style="float: left; width: calc(31% - 4px)">
                                                <dx:ASPxComboBox ID="cmbShift" runat="server" ClientInstanceName="cmbShiftClient" Width="250px">
                                                    <ValidationSettings ErrorDisplayMode="None" />
                                                    <ClientSideEvents SelectedIndexChanged="cmbShiftClient_SelectedIndexChanged" />
                                                </dx:ASPxComboBox>
                                            </div>
                                            <div style="float: left; width: calc(35% - 4px)">
                                                <div style="float: left; width: 20%; padding-left: 5px;">
                                                    <dx:ASPxLabel ID="lblDaysType" runat="server" Text="en un día" />
                                                </div>
                                                <div style="float: right; width: calc(79% - 4px)">
                                                    <dx:ASPxComboBox ID="cmbDaysType" runat="server" ClientInstanceName="cmbDaysTypeClient" Width="100px">
                                                        <ValidationSettings ErrorDisplayMode="None" />
                                                    </dx:ASPxComboBox>
                                                </div>
                                            </div>

                                            <div style="float: left; width: 26%; clear: both">
                                                <dx:ASPxRadioButton GroupName="ValueGroup" ID="ckIncidence" runat="server" ClientInstanceName="ckIncidence_client" Text="Día con previsión de ausencia por">
                                                    <ClientSideEvents CheckedChanged="ckIncidence_client_CheckedChanged" />
                                                </dx:ASPxRadioButton>
                                            </div>
                                            <div style="float: left; width: calc(31% - 4px)">
                                                <dx:ASPxComboBox ID="cmbDayCause" runat="server" ClientInstanceName="cmbDayCauseClient" Width="250px">
                                                    <ValidationSettings ErrorDisplayMode="None" />
                                                </dx:ASPxComboBox>
                                            </div>

                                            <div style="float: left; width: calc(35% - 4px)">
                                                <div style="float: left; width: 20%; padding-left: 5px;">
                                                    <dx:ASPxLabel ID="lblDayCauseType" runat="server" Text="en un día" />
                                                </div>
                                                <div style="float: right; width: calc(79% - 4px)">
                                                    <dx:ASPxComboBox ID="cmbDaysTypeCause" runat="server" ClientInstanceName="cmbDaysTypeCauseClient" Width="100px">
                                                        <ValidationSettings ErrorDisplayMode="None" />
                                                    </dx:ASPxComboBox>
                                                </div>
                                            </div>
                                        </div>

                                        <div style="clear: both; padding-bottom: 10px; padding-top: 10px">
                                            <div class="panHeader2">
                                                <span style="">
                                                    <asp:Label runat="server" ID="lbltxtFactor" Text="Factor"></asp:Label></span>
                                            </div>
                                        </div>
                                        <div style="clear: both; padding-bottom: 10px;">
                                            <div style="float: left; width: 20%">
                                                <dx:ASPxRadioButton GroupName="FactorGroup" ID="ckFixedValue" runat="server" ClientInstanceName="ckFixedValue_client" Text="Factor">
                                                    <ClientSideEvents CheckedChanged="ckFixedValue_client_CheckedChanged" />
                                                </dx:ASPxRadioButton>
                                            </div>
                                            <div style="float: right; width: calc(80% - 4px)">
                                                <dx:ASPxTextBox runat="server" ID="txtFactorComposition" MaxLength="12" Width="75" ClientInstanceName="txtFactorCompositionClient">
                                                    <MaskSettings Mask="<-9999..9999>.<000000..999999>" />
                                                </dx:ASPxTextBox>
                                            </div>
                                            <div style="float: left; width: 20%; clear: both">
                                                <dx:ASPxRadioButton GroupName="FactorGroup" ID="ckFactorUserField" runat="server" ClientInstanceName="ckFactorUserField_client" Text="Según campo de la ficha">
                                                    <ClientSideEvents CheckedChanged="ckFactorUserField_client_CheckedChanged" />
                                                </dx:ASPxRadioButton>
                                            </div>
                                            <div style="float: right; width: calc(80% - 4px)">
                                                <dx:ASPxComboBox ID="cmbFactorUserField" runat="server" ClientInstanceName="cmbFactorUserFieldClient" Width="250px" />
                                            </div>
                                        </div>
                                        <div style="clear: both; padding-bottom: 10px; padding-top: 10px">
                                            <div class="panHeader2">
                                                <span style="">
                                                    <asp:Label runat="server" ID="lblCriteria" Text="Criterios"></asp:Label></span>
                                            </div>
                                        </div>
                                        <div style="clear: both">
                                            <roUserControls:roOptionPanelClient ID="optChkCondition" runat="server" TypeOPanel="CheckboxOption" width="100%" height="Auto" Checked="False" Enabled="True" style="width: 100%;" CConClick="frmCompositionChanges();">
                                                <Title>
                                                    <asp:Label ID="lblChkCondition" runat="server" Text="Se acumulará según el siguiente criterio"></asp:Label>
                                                </Title>
                                                <Description>
                                                    <asp:Label ID="lblChkCondDesc" runat="server" Text=""></asp:Label>
                                                </Description>
                                                <Content>
                                                    <div style="padding-top: 10px;" cellpadding="0" cellspacing="0">
                                                        <table border="0">
                                                            <tr>
                                                                <td>
                                                                    <table cellpadding="0" cellspacing="0">
                                                                        <tr>
                                                                            <td>
                                                                                <div id="divListActions" style="background-color: #E8EEF7; width: 100%; height: 25px; text-align: right; vertical-align: middle;"
                                                                                    runat="server">
                                                                                    <table border="0" style="width: 100%;">
                                                                                        <tr>
                                                                                            <td style="padding-left: 5px;">
                                                                                                <asp:Label ID="lblCauses" runat="server" Text="Justificaciones"></asp:Label>
                                                                                            </td>
                                                                                            <td align="right">
                                                                                                <div style="padding-top: 5px; padding-right: 10px;">
                                                                                                    <img id="imgAddListValue" src="" visible="true" runat="server"
                                                                                                        title='<%# Me.Language.Translate("addListValue",Me.DefaultScope) %>' style="cursor: pointer;"
                                                                                                        onclick="if(document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmCompositions1_optChkCondition_chkButton').checked == true){ ShowWindow('divNewCauseConditions',true); }" />
                                                                                                    <img id="imgRemoveListValue" src="" visible="true"
                                                                                                        runat="server" title='<%# Me.Language.Translate("delListValue",Me.DefaultScope) %>'
                                                                                                        style="cursor: pointer;" onclick="RemoveListValue();" />
                                                                                                </div>
                                                                                                <div style="position: absolute; z-index: 15010;">
                                                                                                    <!-- ModalPopup para Nueva Causa -->
                                                                                                    <div id="divNewCauseConditions" style="position: absolute; width: 400px; z-index: 15050; display: none;">
                                                                                                        <div class="bodyPopupExtended" style="">
                                                                                                            <table width="100%" cellspacing="0" border="0">
                                                                                                                <tr id="panNewCauseDragHandle" style="height: 20px;">
                                                                                                                    <td align="center"></td>
                                                                                                                    <td style="padding-left: 10px;">
                                                                                                                        <asp:Label ID="lblNewCauseTitle" Text="Nueva justificación" Font-Bold="true" ForeColor="#485A6B"
                                                                                                                            runat="server" />
                                                                                                                    </td>
                                                                                                                    <td align="right">
                                                                                                                        <img id="ibtNewCauseOK" runat="server" src="" onclick="AddCauseOK(); return false;"
                                                                                                                            style="cursor: pointer;" />
                                                                                                                        <img id="ibtNewCauseCancel" onclick="ShowWindow('divNewCauseConditions',false);"
                                                                                                                            style="cursor: pointer;" src="" runat="server" />
                                                                                                                    </td>
                                                                                                                </tr>
                                                                                                                <tr>
                                                                                                                    <td colspan="3" style="padding-left: 10px; padding-top: 5px; vertical-align: middle">
                                                                                                                        <table style="width: 100%;" border="0">
                                                                                                                            <tr>
                                                                                                                                <td colspan="2" style="padding-bottom: 2px;">
                                                                                                                                    <asp:Label ID="lblCauseTitle2" CssClass="spanEmp-Class" runat="server" Text="Seleccione la justificación" />
                                                                                                                                </td>
                                                                                                                            </tr>
                                                                                                                            <tr>
                                                                                                                                <td colspan="2" style="padding-bottom: 5px; text-align: center; padding-left: 10px;">
                                                                                                                                    <dx:ASPxComboBox ID="cmbCauseAdd" runat="server" ClientInstanceName="cmbCauseAddClient" Width="300px" />
                                                                                                                                </td>
                                                                                                                            </tr>
                                                                                                                            <tr>
                                                                                                                                <td valign="bottom" style="height: 19px;">
                                                                                                                                    <input id="opPlus" type="radio" name="optPlusMinus" value="0" checked="true" />&nbsp;<a
                                                                                                                                        href="javascript: void(0);" onclick="document.getElementById('opPlus').checked=true;"><asp:Label
                                                                                                                                            ID="lblopPlus" runat="server" Text="Se sumará la justificación"></asp:Label></a>
                                                                                                                                </td>
                                                                                                                                <td valign="bottom" style="height: 19px;">
                                                                                                                                    <input id="opMinus" type="radio" name="optPlusMinus" value="1" />&nbsp;<a href="javascript: void(0);"
                                                                                                                                        onclick="document.getElementById('opMinus').checked=true;"><asp:Label ID="lblopMinus"
                                                                                                                                            runat="server" Text="Se restará la justificación"></asp:Label></a>
                                                                                                                                </td>
                                                                                                                            </tr>
                                                                                                                        </table>
                                                                                                                    </td>
                                                                                                                </tr>
                                                                                                            </table>
                                                                                                        </div>
                                                                                                    </div>
                                                                                                    <!-- Fin ModalPopup -->
                                                                                                </div>
                                                                                            </td>
                                                                                        </tr>
                                                                                    </table>
                                                                                </div>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td>
                                                                                <input type="hidden" id="selectedIdx" value="" />
                                                                                <div id="divConditionsCauses" runat="server" style="width: 280px; height: 100px; display: block; border: solid 1px silver; overflow: auto;">
                                                                                </div>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                                <td valign="top">
                                                                    <table cellpadding="0" cellspacing="0">
                                                                        <tr>
                                                                            <td>
                                                                                <div style="background-color: #E8EEF7; width: 100%; height: 25px; text-align: right; vertical-align: middle;">
                                                                                    <table border="0" style="width: 100%;">
                                                                                        <tr>
                                                                                            <td style="padding-left: 5px; padding-top: 5px;">
                                                                                                <asp:Label ID="lblCompares" runat="server" Text="Comparación"></asp:Label>
                                                                                            </td>
                                                                                        </tr>
                                                                                    </table>
                                                                                </div>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td>
                                                                                <dx:ASPxComboBox ID="cmbCompare" runat="server" ClientInstanceName="cmbCompareClient" Width="150px" />
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                                <td valign="top">
                                                                    <table cellpadding="0" cellspacing="0">
                                                                        <tr>
                                                                            <td>
                                                                                <div style="background-color: #E8EEF7; width: 100%; height: 25px; text-align: right; vertical-align: middle;">
                                                                                    <table border="0" style="width: 100%;">
                                                                                        <tr>
                                                                                            <td style="padding-left: 5px; padding-top: 5px;">
                                                                                                <asp:Label ID="lblTypeValues" runat="server" Text="Tipo"></asp:Label>
                                                                                            </td>
                                                                                        </tr>
                                                                                    </table>
                                                                                </div>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td>
                                                                                <dx:ASPxComboBox ID="cmbTypeValue" runat="server" ClientInstanceName="cmbTypeValueClient" Width="150px">
                                                                                    <ClientSideEvents SelectedIndexChanged="function(s,e){ showTypeValue(s.GetSelectedIndex());}" />
                                                                                </dx:ASPxComboBox>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                                <td valign="top">
                                                                    <table cellpadding="0" cellspacing="0">
                                                                        <tr>
                                                                            <td>
                                                                                <div style="background-color: #E8EEF7; width: 100%; height: 25px; text-align: right; vertical-align: middle;">
                                                                                    <table border="0" style="width: 100%;">
                                                                                        <tr>
                                                                                            <td style="padding-left: 5px; padding-top: 5px;">
                                                                                                <asp:Label ID="lblValues" runat="server" Text="Valor"></asp:Label>
                                                                                            </td>
                                                                                        </tr>
                                                                                    </table>
                                                                                </div>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td align="right">
                                                                                <dx:ASPxComboBox ID="cmbCauseType" runat="server" ClientInstanceName="cmbCauseTypeClient" Width="150px">
                                                                                </dx:ASPxComboBox>
                                                                                <input type="text" id="txtValueType" class="textClass x-form-text x-form-field" style="width: 185px; text-align: right;"
                                                                                    value="" runat="server" convertcontrol="TextField"
                                                                                    cctime="true" />
                                                                                <dx:ASPxComboBox ID="cmbCauseUField" runat="server" ClientInstanceName="cmbCauseUFieldClient" Width="150px" />
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </div>
                                                </Content>
                                            </roUserControls:roOptionPanelClient>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <input type="hidden" id="hdnMustRefresh_PageBase" value="0" runat="server" />
                <table border="0" style="width: 100%;">
                    <tr>
                        <td colspan="3" align="right">
                            <table>
                                <tr>
                                    <td style="width: 110px;" align="right">
                                        <dx:ASPxButton ID="btnOk" ClientInstanceName="btnAcceptClient" runat="server" AutoPostBack="False" CausesValidation="False" Text="Aceptar" ToolTip="Aceptar"
                                            HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                            <ClientSideEvents Click="function(s, e) { saveComposition(); }" />
                                        </dx:ASPxButton>
                                    </td>
                                    <td style="width: 110px;" align="left">
                                        <dx:ASPxButton ID="btnCancel" ClientInstanceName="btnCancelClient" runat="server" AutoPostBack="False" CausesValidation="False" Text="Cancelar" ToolTip="Cancelar"
                                            HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                            <ClientSideEvents Click="function(s, e) { cancelComposition(); }" />
                                        </dx:ASPxButton>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
    <!-- End Div flotant compositions -->