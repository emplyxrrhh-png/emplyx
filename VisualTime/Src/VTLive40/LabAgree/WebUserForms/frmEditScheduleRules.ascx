<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.WebUserForms_frmEditScheduleRules" CodeBehind="frmEditScheduleRules.ascx.vb" %>

<div id="<%= Me.ClientID %>_frm" style="position: fixed; z-index: 9010; display: none; top: 50%; left: 50%; width: 650px;">
    <div id="<%= Me.ClientID %>_BgS" style="position: absolute; top: 0; left: 0; display: none; z-index: 9009;"></div>
    <div class="bodyPopupExtended">

        <dx:ASPxCallbackPanel ID="ASPxScheduleRulesCallbackPanelContenido" runat="server" Width="100%" Height="100%" ClientInstanceName="ASPxScheduleRulesCallbackPanelContenidoClient">
            <SettingsLoadingPanel Enabled="false" />
            <ClientSideEvents EndCallback="ASPxScheduleRulesCallbackPanelContenido_EndCallBack" />
            <PanelCollection>
                <dx:PanelContent ID="PanelContent1" runat="server">
                    <div id="divContentPanels" style="padding-right: 20px">
                        <div id="div00" class="contentPanel" runat="server" name="menuPanel">
                            <input type="hidden" runat="server" id="hdnModeEdit" />
                            <!-- Este div es el header -->
                            <div class="panBottomMargin">
                                <div class="panHeader2 panBottomMargin">
                                    <span class="panelTitleSpan">
                                        <asp:Label runat="server" ID="lblGeneralTitle" Text="General"></asp:Label>
                                    </span>
                                </div>
                            </div>

                            <!-- Este div es un formulario -->
                            <div class="panBottomMargin">
                                <div class="divRow">
                                    <div class="divRowDescription">
                                        <asp:Label ID="lblNameDescription" runat="server" Text="Nombre identificativo de la regla de planificación"></asp:Label>
                                    </div>
                                    <asp:Label ID="lblName" runat="server" Text="Nombre:" CssClass="labelForm"></asp:Label>
                                    <div class="componentForm">
                                        <div style="float: left; width: 75%">
                                            <dx:ASPxTextBox ID="txtName" runat="server" ClientInstanceName="txtName_Client" Width="100%" NullText="_____" MaxLength="50">
                                                <ClientSideEvents Validation="LengthValidation" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" Init="function(s, e) {dxcollection.push(s);}" />
                                                <ValidationSettings SetFocusOnError="True">
                                                    <RequiredField IsRequired="True" ErrorText="(*)" />
                                                </ValidationSettings>
                                            </dx:ASPxTextBox>
                                        </div>
                                        <div style="float: right">
                                            <dx:ASPxCheckBox runat="server" ID="ckRuleActive" Width="75px" Text="Activa" Checked="true">
                                                <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" Init="function(s, e) {dxcollection.push(s);}" />
                                            </dx:ASPxCheckBox>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <!-- Este div es un formulario -->
                            <div class="panBottomMargin">
                                <div class="divRow">
                                    <div class="divRowDescription">
                                        <asp:Label ID="lblRuleTypeDesc" runat="server" Text="Tipo de regla que desea configurar"></asp:Label>
                                    </div>
                                    <asp:Label ID="lblRuleType" runat="server" Text="Tipo de regla" CssClass="labelForm"></asp:Label>
                                    <div class="componentForm">
                                        <dx:ASPxComboBox runat="server" ID="cmbScheduleRuleType" ClientInstanceName="cmbScheduleRuleType_Client" Width="250px" NullText="_____">
                                            <ClientSideEvents SelectedIndexChanged="loadSpecificScheduleRuleConfigurationDiv" Validation="SelectedItemRequiered" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" Init="function(s, e) {dxcollection.push(s);}" />
                                            <ValidationSettings SetFocusOnError="True">
                                                <RequiredField IsRequired="True" ErrorText="(*)" />
                                            </ValidationSettings>
                                        </dx:ASPxComboBox>
                                    </div>
                                </div>
                            </div>

                            <!-- Este div es un formulario -->
                            <div class="panBottomMargin">
                                <div class="divRow">
                                    <div class="divRowDescription">
                                        <asp:Label ID="lblDescriptionDesc" runat="server" Text="Descripción de la regla de validación"></asp:Label>
                                    </div>
                                    <asp:Label ID="lblDescription" runat="server" Text="Regla de validación:" CssClass="labelForm"></asp:Label>
                                    <div class="componentForm">
                                        <dx:ASPxMemo ID="txtDescription" runat="server" Rows="4" Width="100%" Height="40">
                                            <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" Init="function(s, e) {dxcollection.push(s);}" />
                                        </dx:ASPxMemo>
                                    </div>
                                </div>
                            </div>

                            <!-- Este div es el header -->
                            <div class="panBottomMargin">
                                <div class="panHeader2 panBottomMargin">
                                    <span class="panelTitleSpan">
                                        <asp:Label runat="server" ID="lblConfigurationTitle" Text="Configuración de la regla"></asp:Label>
                                    </span>
                                </div>
                            </div>

                            <div class="panBottomMargin" style="min-height: 200px">
                                <div id="scheduleRuleType_0" style="display: none">
                                    <!-- OneShiftOneDay -->
                                    <div class="divRow">
                                        <div class="divRowDescription">
                                            <asp:Label ID="lblOneShiftOneDayDesc" runat="server" Text="Horarios a los que afecta la regla"></asp:Label>
                                        </div>
                                        <asp:Label ID="lblOneShiftOneDayTitle" runat="server" Text="Horarios" CssClass="labelForm"></asp:Label>
                                        <div class="componentForm">
                                            <dx:ASPxTokenBox ID="tbOneShiftOneDayAvailableShifts" runat="server" Width="100%">
                                                <ClientSideEvents TextChanged="" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" Init="function(s, e) {dxcollection.push(s);}" />
                                            </dx:ASPxTokenBox>
                                        </div>
                                    </div>
                                    <div class="divRow">
                                        <div class="divRowDescription">
                                            <asp:Label ID="lblOneShiftOneDayParamDesc" runat="server" Text="Solo se podrá planificar los horarios especificados si cumplen los siguientes requisitos"></asp:Label>
                                        </div>
                                        <asp:Label ID="lblOneShiftOneDayParamTitle" runat="server" Text="Planificado en" CssClass="labelForm"></asp:Label>
                                        <div class="componentForm">
                                            <div>
                                                <div style="float: left">
                                                    <dx:ASPxComboBox runat="server" ID="cmbOneShiftOneDayWhen" Width="125px" NullText="_____" ClientInstanceName="cmbOneShiftOneDayWhenChangedClient">
                                                        <ClientSideEvents SelectedIndexChanged="cmbOneShiftOneDayWhenChanged" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" Init="function(s, e) {dxcollection.push(s);}" />
                                                    </dx:ASPxComboBox>
                                                </div>
                                                <div style="float: left; padding-left: 10px">
                                                    <div id="cmbOneShiftOneDayWhenDay" style="display: none">
                                                        <dx:ASPxComboBox runat="server" ID="cmbOneShiftOneDayWhenDayOfWeek" Width="150px" NullText="_____">
                                                            <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" Init="function(s, e) {dxcollection.push(s);}" />
                                                        </dx:ASPxComboBox>
                                                    </div>
                                                    <div id="cmbOneShiftOneDayWhenWeek" style="display: none">
                                                        <dx:ASPxDateEdit ID="txtOneShiftOneDayWhenWeek" PopupVerticalAlign="WindowCenter" runat="server" Width="150" DisplayFormatString="dd/MM" EditFormat="Custom" EditFormatString="dd/MM">
                                                            <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" Init="function(s, e) {dxcollection.push(s);}" />
                                                            <CalendarProperties ShowClearButton="false" />
                                                        </dx:ASPxDateEdit>
                                                    </div>
                                                    <div id="cmbOneShiftOneDayWhenYear" style="display: none">
                                                        <dx:ASPxDateEdit ID="txtOneShiftOneDayWhenYear" PopupVerticalAlign="WindowCenter" runat="server" Width="150">
                                                            <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" Init="function(s, e) {dxcollection.push(s);}" />
                                                            <CalendarProperties ShowClearButton="false" />
                                                        </dx:ASPxDateEdit>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="divRow">
                                        <asp:Label ID="lblOneShiftOneDayValidateTitle" runat="server" Text="y el" CssClass="labelForm" Style="padding-top: 5px;"></asp:Label>
                                        <div class="componentForm">
                                            <table>
                                                <tr>
                                                    <td>
                                                        <div>
                                                            <div style="float: left; padding-right: 5px">
                                                                <dx:ASPxCheckBox ID="rbOneShiftOneDayValidateShift" runat="server" Text="El día anterior">
                                                                    <ClientSideEvents Init="function(s, e) {dxcollection.push(s);}" />
                                                                </dx:ASPxCheckBox>
                                                            </div>
                                                            <div style="float: left; padding-left: 10px">
                                                                <dx:ASPxComboBox runat="server" ID="cmbOneShiftOneDayValidate" Width="75px" NullText="_____">
                                                                    <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" Init="function(s, e) {dxcollection.push(s);}" />
                                                                </dx:ASPxComboBox>
                                                            </div>
                                                            <div style="float: left; padding-left: 10px">
                                                                <dx:ASPxTokenBox ID="tbOneShiftOneDayValidateComparison" runat="server" Width="200px">
                                                                    <ClientSideEvents TextChanged="" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" Init="function(s, e) {dxcollection.push(s);}" />
                                                                </dx:ASPxTokenBox>
                                                            </div>
                                                        </div>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <div>
                                                            <div style="float: left">
                                                                <dx:ASPxCheckBox ID="rbPostOneShiftOneDayValidateShift" runat="server" Text="El día posterior">
                                                                    <ClientSideEvents Init="function(s, e) {dxcollection.push(s);}" />
                                                                </dx:ASPxCheckBox>
                                                            </div>
                                                            <div style="float: left; padding-left: 10px">
                                                                <dx:ASPxComboBox runat="server" ID="cmbPostOneShiftOneDayValidate" Width="75px" NullText="_____">
                                                                    <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" Init="function(s, e) {dxcollection.push(s);}" />
                                                                </dx:ASPxComboBox>
                                                            </div>
                                                            <div style="float: left; padding-left: 10px">
                                                                <dx:ASPxTokenBox ID="tbPostOneShiftOneDayValidateComparison" runat="server" Width="200px">
                                                                    <ClientSideEvents TextChanged="" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" Init="function(s, e) {dxcollection.push(s);}" />
                                                                </dx:ASPxTokenBox>
                                                            </div>
                                                        </div>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <div style="clear: both">
                                                            <div style="float: left">
                                                                <dx:ASPxCheckBox ID="rbOneShiftOneDayValidateHours" runat="server" Text="Mínimo descanso desde la última jornada sea superior a">
                                                                    <ClientSideEvents Init="function(s, e) {dxcollection.push(s);}" />
                                                                </dx:ASPxCheckBox>
                                                            </div>
                                                            <div style="float: left; padding-left: 10px">
                                                                <dx:ASPxTimeEdit ID="tbOneShiftOneDayValidateHours" EditFormatString="HH:mm" EditFormat="Custom" runat="server" Width="85px" AllowNull="false">
                                                                    <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" Init="function(s, e) {dxcollection.push(s);}" />
                                                                </dx:ASPxTimeEdit>
                                                            </div>
                                                        </div>
                                                    </td>
                                                </tr>
                                            </table>

                                            <br />
                                        </div>
                                    </div>
                                </div>
                                <div id="scheduleRuleType_1" style="display: none">
                                    <!-- RestBetweenShift -->
                                    <div class="divRow">
                                        <div class="divRowDescription">
                                            <asp:Label ID="lblRestBetweenShifsDesc" runat="server" Text="Número de horas de descanso que deben existir entre jornadas"></asp:Label>
                                        </div>
                                        <asp:Label ID="lblRestBetweenShifsTitle" runat="server" Text="Número de horas" CssClass="labelForm"></asp:Label>
                                        <div class="componentForm">
                                            <dx:ASPxTimeEdit ID="lblRestBetweenShifsTime" EditFormatString="HH:mm" EditFormat="Custom" runat="server" Width="85px" AllowNull="false">
                                                <ClientSideEvents DateChanged="function(s,e){}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" Init="function(s, e) {dxcollection.push(s);}" />
                                            </dx:ASPxTimeEdit>
                                        </div>
                                    </div>
                                </div>
                                <div id="scheduleRuleType_2" style="display: none">
                                    <div class="divRow">
                                        <div class="divRowDescription">
                                            <asp:Label ID="lblMinMaxFreeLabourDaysInPeriodPeriodDesc" runat="server" Text="Período en el que se calculará la regla"></asp:Label>
                                        </div>
                                        <asp:Label ID="lblMinMaxFreeLabourDaysInPeriodPeriodTitle" runat="server" Text="Período:" CssClass="labelForm"></asp:Label>
                                        <div class="componentForm">
                                            <dx:ASPxComboBox runat="server" ID="cmbMinMaxFreeLabourDaysInPeriodPeriod" Width="125px" NullText="_____">
                                                <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" Init="function(s, e) {dxcollection.push(s);}" />
                                            </dx:ASPxComboBox>
                                        </div>
                                    </div>
                                    <div class="divRow">
                                        <div class="divRowDescription">
                                            <asp:Label ID="lblMinMaxFreeLabourDaysInPeriodTypeDesc" runat="server" Text="Tipo de día"></asp:Label>
                                        </div>
                                        <asp:Label ID="lblMinMaxFreeLabourDaysInPeriodTypeTitle" runat="server" Text="Tipo:" CssClass="labelForm"></asp:Label>
                                        <div class="componentForm">
                                            <dx:ASPxComboBox runat="server" ID="cmbMinMaxFreeLabourDaysInPeriodType" Width="125px" NullText="_____">
                                                <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" Init="function(s, e) {dxcollection.push(s);}" />
                                            </dx:ASPxComboBox>
                                        </div>
                                    </div>
                                    <div class="divRow">
                                        <div class="divRowDescription">
                                            <asp:Label ID="lblMinFreeLabourDaysInPeriodRepeatDesc" runat="server" Text="Número mínimo de días"></asp:Label>
                                        </div>
                                        <asp:Label ID="lblMinFreeLabourDaysInPeriodRepeatTitle" runat="server" Text="Mínimo:" CssClass="labelForm"></asp:Label>
                                        <div class="componentForm">
                                            <div style="float: left">
                                                <dx:ASPxCheckBox ID="ckMinFreeLabourDaysInPeriodRepeat" runat="server" Text="">
                                                    <ClientSideEvents Init="function(s, e) {dxcollection.push(s);}" />
                                                </dx:ASPxCheckBox>
                                            </div>
                                            <div style="float: left; padding-left: 10px">
                                                <dx:ASPxTextBox runat="server" ID="txtMinFreeLabourDaysInPeriodRepeat" MaxLength="12" Width="75" ValidationSettings-Display="None">
                                                    <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" Init="function(s, e) {dxcollection.push(s);}" />
                                                    <MaskSettings Mask="<0..366>" ErrorText="(*)" />
                                                </dx:ASPxTextBox>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="divRow">
                                        <div class="divRowDescription">
                                            <asp:Label ID="lblMaxFreeLabourDaysInPeriodRepeatDesc" runat="server" Text="Número máximo de días"></asp:Label>
                                        </div>
                                        <asp:Label ID="lblMaxFreeLabourDaysInPeriodRepeatTitle" runat="server" Text="Máximo:" CssClass="labelForm"></asp:Label>
                                        <div class="componentForm">
                                            <div style="float: left">
                                                <dx:ASPxCheckBox ID="ckMaxFreeLabourDaysInPeriodRepeat" runat="server" Text="">
                                                    <ClientSideEvents Init="function(s, e) {dxcollection.push(s);}" />
                                                </dx:ASPxCheckBox>
                                            </div>
                                            <div style="float: left; padding-left: 10px">
                                                <dx:ASPxTextBox runat="server" ID="txtMaxFreeLabourDaysInPeriodRepeat" MaxLength="12" Width="75" ValidationSettings-Display="None">
                                                    <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" Init="function(s, e) {dxcollection.push(s);}" />
                                                    <MaskSettings Mask="<0..366>" ErrorText="(*)" />
                                                </dx:ASPxTextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div id="scheduleRuleType_3" style="display: none">
                                    <div class="divRow">
                                        <div class="divRowDescription" style="padding-top: 5px !important">
                                            <asp:Label ID="lblMinMaxShiftsInPeriodPeriodDesc" runat="server" Text="Período en el que se calculará la regla"></asp:Label>
                                        </div>
                                        <asp:Label ID="lblEvery" runat="server" Text="Cada:" CssClass="labelForm"></asp:Label>
                                        <div class="componentForm">

                                            <div style="float: left; padding-right: 10px;" id="divPeriod">
                                                <dx:ASPxTextBox runat="server" ID="txtPeriodicty" MaxLength="6" Width="40" ClientInstanceName="txtPeriodictyClient">
                                                    <ClientSideEvents TextChanged="function(s,e){ hasChanges(true)}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                    <ValidationSettings ErrorDisplayMode="None">
                                                    </ValidationSettings>
                                                    <MaskSettings Mask="<1..99999>" />
                                                </dx:ASPxTextBox>
                                            </div>
                                            <div style="float: left; padding-right: 10px; padding-top: 3px">
                                                <dx:ASPxComboBox runat="server" ID="cmbMinMaxShiftsInPeriodPeriod" Width="100px" NullText="_____">
                                                    <ClientSideEvents GotFocus="HightlightOnGotFocus" SelectedIndexChanged="function(s,e){ periodChanged(s.GetSelectedItem().value)}" LostFocus="FadeOnLostFocus" Init="function(s, e) {dxcollection.push(s);}" />
                                                </dx:ASPxComboBox>
                                            </div>
                                            <div style="float: left; padding-right: 5px; padding-top: 3px" id="divAlways">
                                                <dx:ASPxCheckBox ID="ckAlways" runat="server" Width="105px" Text="entre periodos" ClientInstanceName="clAlwaysClient">
                                                    <ClientSideEvents Init="function(s, e) {dxcollection.push(s);}" CheckedChanged="enablePeriod" />
                                                </dx:ASPxCheckBox>
                                            </div>

                                            <div style="float: left; padding-right: 10px; padding-top: 5px;" id="divEntre">
                                                <asp:Label ID="txtFrom" runat="server" Text="entre"></asp:Label>
                                            </div>
                                            <div style="float: left; padding-right: 10px; padding-top: 3px" id="divFrom">
                                                <dx:ASPxDateEdit ID="txtDateFrom" runat="server" PopupVerticalAlign="WindowCenter" DisplayFormatString="dd/MM" EditFormat="Custom" EditFormatString="dd/MM" AllowNull="False" Width="70px" CssClass="editTextFormat" Font-Size="11px" ClientInstanceName="txtDateFromClient">
                                                </dx:ASPxDateEdit>
                                            </div>
                                            <div style="float: left; padding-right: 10px; padding-top: 5px;" id="divHasta">
                                                <asp:Label ID="txtTo" runat="server" Text="y"></asp:Label>
                                            </div>
                                            <div style="float: left; padding-top: 3px" id="divTo">
                                                <dx:ASPxDateEdit ID="txtDateTo" runat="server" PopupVerticalAlign="WindowCenter" DisplayFormatString="dd/MM" EditFormat="Custom" EditFormatString="dd/MM" AllowNull="False" Width="70px" CssClass="editTextFormat" Font-Size="11px" ClientInstanceName="txtDateToClient">
                                                </dx:ASPxDateEdit>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="divRow">
                                        <div class="divRowDescription">
                                            <asp:Label ID="lblMinMaxShiftsInPeriodDesc" runat="server" Text="Indique los horarios a los que afecta la regla"></asp:Label>
                                        </div>
                                        <asp:Label ID="lblMinMaxShiftsInPeriodTitle" runat="server" Text="Horarios:" CssClass="labelForm"></asp:Label>
                                        <div class="componentForm">
                                            <dx:ASPxTokenBox ID="tbMinMaxShiftsInPeriod" runat="server" Width="100%">
                                                <ClientSideEvents TextChanged="" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" Init="function(s, e) {dxcollection.push(s);}" />
                                            </dx:ASPxTokenBox>
                                        </div>
                                    </div>
                                    <div class="divRow">
                                        <div class="divRowDescription">
                                            <asp:Label ID="lblMinShiftsInPeriodRepeatDesc" runat="server" Text="Número mínimo de días planificados con el horario"></asp:Label>
                                        </div>
                                        <asp:Label ID="lblMinShiftsInPeriodRepeatTitle" runat="server" Text="Mínimo:" CssClass="labelForm"></asp:Label>
                                        <div class="componentForm">
                                            <div style="float: left">
                                                <dx:ASPxCheckBox ID="ckMinShiftsInPeriodRepeat" runat="server" Text="">
                                                    <ClientSideEvents Init="function(s, e) {dxcollection.push(s);}" />
                                                </dx:ASPxCheckBox>
                                            </div>
                                            <div style="float: left; padding-left: 10px">
                                                <dx:ASPxTextBox runat="server" ID="txtMinShiftsInPeriodRepeat" MaxLength="12" Width="75" ValidationSettings-Display="None">
                                                    <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" Init="function(s, e) {dxcollection.push(s);}" />
                                                    <MaskSettings Mask="<0..366>" ErrorText="(*)" />
                                                </dx:ASPxTextBox>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="divRow">
                                        <div class="divRowDescription">
                                            <asp:Label ID="lblMaxShiftsInPeriodRepeatDesc" runat="server" Text="Número máximo de días planificados con el horario"></asp:Label>
                                        </div>
                                        <asp:Label ID="lblMaxShiftsInPeriodRepeatTitle" runat="server" Text="Máximo:" CssClass="labelForm"></asp:Label>
                                        <div class="componentForm">
                                            <div style="float: left">
                                                <dx:ASPxCheckBox ID="ckMaxShiftsInPeriodRepeat" runat="server" Text="">
                                                    <ClientSideEvents Init="function(s, e) {dxcollection.push(s);}" />
                                                </dx:ASPxCheckBox>
                                            </div>
                                            <div style="float: left; padding-left: 10px">
                                                <dx:ASPxTextBox runat="server" ID="txtMaxShiftsInPeriodRepeat" MaxLength="12" Width="75" ValidationSettings-Display="None">
                                                    <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" Init="function(s, e) {dxcollection.push(s);}" />
                                                    <MaskSettings Mask="<0..366>" ErrorText="(*)" />
                                                </dx:ASPxTextBox>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="divRow">
                                        <div class="divRowDescription">
                                            <asp:Label ID="lblLogicOrDesc" runat="server" Text=" Indique a qué horarios aplica la regla"></asp:Label>
                                        </div>
                                        <asp:Label ID="lblLogicOrTitle" runat="server" Text="Aplica a:" CssClass="labelForm"></asp:Label>
                                        <div class="componentForm">
                                            <div style="float: left">
                                                <dx:ASPxComboBox runat="server" ID="cmbLogicOr" Width="150px">
                                                    <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" Init="function(s, e) {dxcollection.push(s);}" />
                                                </dx:ASPxComboBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div id="scheduleRuleType_4" style="display: none">
                                    <div class="divRow">
                                        <div class="divRowDescription">
                                            <asp:Label ID="lblMinWeekendsInPeriodDesc" runat="server" Text="Número mínimo de jornadas de descanso en el período especificado"></asp:Label>
                                        </div>
                                        <asp:Label ID="lblMinWeekendsInPeriodTitle" runat="server" Text="Número:" CssClass="labelForm"></asp:Label>
                                        <div class="componentForm">
                                            <div style="float: left">
                                                <dx:ASPxTextBox runat="server" ID="txtMinWeekendsInPeriod" MaxLength="12" Width="75" ValidationSettings-Display="None">
                                                    <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" Init="function(s, e) {dxcollection.push(s);}" />
                                                    <MaskSettings Mask="<0..366>" ErrorText="(*)" />
                                                </dx:ASPxTextBox>
                                            </div>
                                            <div style="float: left; padding-left: 10px">
                                                <dx:ASPxComboBox runat="server" ID="cmbMinWeekendsInPeriod" Width="125px" NullText="_____">
                                                    <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" Init="function(s, e) {dxcollection.push(s);}" />
                                                </dx:ASPxComboBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div id="scheduleRuleType_5" style="display: none"></div>
                                <div id="scheduleRuleType_6" style="display: none">
                                    <div class="divRow">
                                        <div class="divRowDescription">
                                            <asp:Label ID="lblTwoShiftSequenceDesc" runat="server" Text="No se podrán planificar los siguientes horarios"></asp:Label>
                                        </div>
                                        <asp:Label ID="TwoShiftSequenceTitle" runat="server" Text="Horarios" CssClass="labelForm"></asp:Label>
                                        <div class="componentForm">
                                            <dx:ASPxTokenBox ID="tbTwoShiftSequenceCurrentDay" runat="server" Width="100%">
                                                <ClientSideEvents TextChanged="" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" Init="function(s, e) {dxcollection.push(s);}" />
                                            </dx:ASPxTokenBox>
                                        </div>
                                    </div>
                                    <div class="divRow">
                                        <div class="divRowDescription">
                                            <asp:Label ID="lblTwoShiftSequenceBeforeDesc" runat="server" Text="Si el día anterior hay planificado uno de los siguientes"></asp:Label>
                                        </div>
                                        <asp:Label ID="lblTwoShiftSequenceBeforeTitle" runat="server" Text="Horarios" CssClass="labelForm"></asp:Label>
                                        <div class="componentForm">
                                            <dx:ASPxTokenBox ID="tbTwoShiftSequenceBeforeDay" runat="server" Width="100%">
                                                <ClientSideEvents TextChanged="" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" Init="function(s, e) {dxcollection.push(s);}" />
                                            </dx:ASPxTokenBox>
                                        </div>
                                    </div>
                                </div>
                                <div id="scheduleRuleType_7" style="display: none"></div>
                                <div id="scheduleRuleType_8" style="display: none"></div>
                                <div id="scheduleRuleType_9" style="display: none"></div>
                                <div id="scheduleRuleType_10" style="display: none"></div>
                                <div id="scheduleRuleType_11" style="display: none">
                                    <div class="divRow">
                                        <div class="divRowDescription">
                                            <asp:Label ID="lblMaxNotScheduledDesc" runat="server" Text="Número máximo de jornadas sin planificar anuales"></asp:Label>
                                        </div>
                                        <asp:Label ID="lblMaxNotScheduledTitle" runat="server" Text="Número:" CssClass="labelForm"></asp:Label>
                                        <div class="componentForm">
                                            <div style="float: left">
                                                <dx:ASPxTextBox runat="server" ID="txtMaxNotScheduled" MaxLength="12" Width="75" ValidationSettings-Display="None">
                                                    <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" Init="function(s, e) {dxcollection.push(s);}" />
                                                    <MaskSettings Mask="<0..366>" ErrorText="(*)" />
                                                </dx:ASPxTextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div id="scheduleRuleType_12" style="display: none">
                                    <div class="divRow">
                                        <div class="divRowDescription">
                                            <asp:Label ID="lblSequenceDesc" runat="server" Text="Máximo número consecutivo de jornadas del tipo indicado"></asp:Label>
                                        </div>
                                        <asp:Label ID="lblSequenceTitle" runat="server" Text="Número:" CssClass="labelForm"></asp:Label>
                                        <div class="componentForm">
                                            <div style="float: left">
                                                <dx:ASPxTextBox runat="server" ID="txtSequence" MaxLength="12" Width="75" ValidationSettings-Display="None">
                                                    <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" Init="function(s, e) {dxcollection.push(s);}" />
                                                    <MaskSettings Mask="<0..366>" ErrorText="(*)" />
                                                </dx:ASPxTextBox>
                                            </div>
                                            <div style="float: left; padding-left: 10px">
                                                <dx:ASPxComboBox runat="server" ID="cmbSequence" Width="125px" NullText="_____">
                                                    <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" Init="function(s, e) {dxcollection.push(s);}" />
                                                </dx:ASPxComboBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div id="scheduleRuleType_13" style="display: none">
                                    <div class="divRow">
                                        <div class="divRowDescription">
                                            <asp:Label ID="lblMinMaxExpectedHoursInPeriodPeriodDesc" runat="server" Text="Período en el que se calculará la regla"></asp:Label>
                                        </div>
                                        <asp:Label ID="lblMinMaxExpectedHoursInPeriodPeriodTitle" runat="server" Text="Período:" CssClass="labelForm"></asp:Label>
                                        <div class="componentForm">
                                            <dx:ASPxComboBox runat="server" ID="cmbMinMaxExpectedHoursInPeriodPeriod" Width="125px" NullText="_____">
                                                <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" Init="function(s, e) {dxcollection.push(s);}" />
                                            </dx:ASPxComboBox>
                                        </div>
                                    </div>
                                    <div class="divRow">
                                        <div class="divRowDescription">
                                            <asp:Label ID="lblMinExpectedHoursInPeriodRepeatDesc" runat="server" Text="Número mínimo de horas planificados con el horario"></asp:Label>
                                        </div>
                                        <asp:Label ID="lblMinExpectedHoursInPeriodRepeatTitle" runat="server" Text="Mínimo:" CssClass="labelForm"></asp:Label>
                                        <div class="componentForm">
                                            <div style="float: left">
                                                <dx:ASPxCheckBox ID="ckMinExpectedHoursInPeriodRepeat" runat="server" Text="">
                                                    <ClientSideEvents Init="function(s, e) {dxcollection.push(s);}" />
                                                </dx:ASPxCheckBox>
                                            </div>
                                            <div style="float: left; padding-left: 10px">
                                                <dx:ASPxTextBox runat="server" ID="txtMinExpectedHoursInPeriodRepeat" MaxLength="12" Width="75" ValidationSettings-Display="None">
                                                    <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" Init="function(s, e) {dxcollection.push(s);}" />
                                                    <MaskSettings Mask="<0..9999>" ErrorText="(*)" />
                                                </dx:ASPxTextBox>
                                            </div>
                                        </div>
                                    </div>

                                    <%--   <div class="divRow">

                                        <asp:Label ID="lblMaxExpectedHoursInPeriodRepeatTitle" runat="server" Text="Máximo:" CssClass="labelForm"></asp:Label>
                                        <div class="componentForm">
                                            <div style="float:left">
                                                <dx:ASPxCheckBox ID="ckMaxExpectedHoursInPeriodRepeat" runat="server" Text="">
                                                    <ClientSideEvents Init="function(s, e) {dxcollection.push(s);}" />
                                                </dx:ASPxCheckBox>
                                            </div>
                                            <div style="float:left;padding-left:10px">
                                            </div>
                                        </div>
                                    </div>--%>

                                    <div class="divRow">
                                        <div class="divRowDescription">
                                            <asp:Label ID="lblMaxExpectedHoursInPeriodRepeatDesc" runat="server" Text="Número máximo de horas planificados con el horario"></asp:Label>
                                        </div>
                                        <asp:Label ID="lblYearHoursTitle" runat="server" Text="Máximo:" CssClass="labelForm"></asp:Label>
                                        <div class="componentForm" style="margin-top: -3px; margin-bottom: 5px;">

                                            <div style="clear: both;">
                                                <div style="float: left; padding-right: 20px">
                                                    <dx:ASPxRadioButton GroupName="HoursYears" ID="ckMaxExpectedHoursInPeriodRepeat" runat="server" ClientInstanceName="ckFixedValuePeriod_client" Text="Fijas">
                                                        <ClientSideEvents CheckedChanged="ckFixedValuePeriod_client_CheckedChanged" />
                                                    </dx:ASPxRadioButton>
                                                </div>
                                                <div id="divtxtYearHours" style="float: left; padding-top: 3px;">
                                                    <dx:ASPxTextBox runat="server" ID="txtMaxExpectedHoursInPeriodRepeat" MaxLength="12" Width="75" ValidationSettings-Display="None" ClientInstanceName="txtMaxExpectedHoursInPeriodRepeat_client">
                                                        <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" Init="function(s, e) {dxcollection.push(s);}" />
                                                        <MaskSettings Mask="<0..9999>" ErrorText="(*)" />
                                                    </dx:ASPxTextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="divRow">
                                        <div class="labelForm">&nbsp</div>

                                        <div class="componentForm" style="margin-top: -3px; margin-bottom: 5px;">

                                            <div style="clear: both; padding-bottom: 10px;">
                                                <div style="float: left; padding-right: 20px;">
                                                    <dx:ASPxRadioButton GroupName="HoursYears" ID="ckUserField" runat="server" ClientInstanceName="ckUserField_client" Text="Según campo de la ficha">
                                                        <ClientSideEvents CheckedChanged="ckUserFieldPeriod_client_CheckedChanged" />
                                                    </dx:ASPxRadioButton>
                                                </div>
                                                <div style="float: left; padding-left: 3px; padding-top: 3px;" id="divcmbUserField">
                                                    <dx:ASPxComboBox ID="cmbUserField" runat="server" ClientInstanceName="cmbUserFieldPeriodClient" Width="215px">
                                                        <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                    </dx:ASPxComboBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div id="scheduleRuleType_14" style="display: none">
                                    <div class="divRow">
                                        <div class="divRowDescription">
                                            <asp:Label ID="lblMinMaxShiftsSequenceDesc" runat="server" Text="Indique los horarios a los que afecta la regla"></asp:Label>
                                        </div>
                                        <asp:Label ID="lblMinMaxShiftsSequenceTitle" runat="server" Text="Horarios:" CssClass="labelForm"></asp:Label>
                                        <div class="componentForm">

                                            <div style="float: left; padding-right: 10px;" id="divTypeSequence">
                                                <dx:ASPxComboBox runat="server" ID="cmbTypeSequence" Width="100px" ClientInstanceName="cmbTypeSequenceClient">
                                                    <ClientSideEvents GotFocus="HightlightOnGotFocus" SelectedIndexChanged="function(s,e){ typeChangedSequence(s.GetSelectedItem().value)}" LostFocus="FadeOnLostFocus" Init="function(s, e) {dxcollection.push(s);}" />
                                                </dx:ASPxComboBox>
                                            </div>
                                            <div style="float: left; padding-right: 10px; width: 175px;" id="divcmbShiftSequence">
                                                <dx:ASPxComboBox runat="server" ID="cmbShiftSequence" Width="175px" ClientInstanceName="cmbShiftSequenceClient">
                                                    <ClientSideEvents GotFocus="HightlightOnGotFocus" SelectedIndexChanged="function(s,e){ shiftChangedSequence(s.GetSelectedItem().value)}" LostFocus="FadeOnLostFocus" Init="function(s, e) {dxcollection.push(s);}" />
                                                </dx:ASPxComboBox>
                                            </div>
                                            <div style="float: left; padding-right: 10px; width: 175px; display: none" id="divcmbTypeSequence">
                                                <%-- <dx:ASPxComboBox runat="server" ID="cmbShiftTypeSequence" Width="175px" NullText="_____">
                                                 <ClientSideEvents GotFocus="HightlightOnGotFocus" SelectedIndexChanged="function(s,e){ shiftChangedSequence(s.GetSelectedItem().value)}" LostFocus="FadeOnLostFocus" Init="function(s, e) {dxcollection.push(s);}" />
                                             </dx:ASPxComboBox>--%>

                                                <dx:ASPxTextBox ID="txtTypeSequence" runat="server" Width="175px" MaxLength="50" ClientInstanceName="txtTypeSequenceClient">
                                                    <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" Init="function(s, e) {dxcollection.push(s);}" />
                                                </dx:ASPxTextBox>
                                            </div>
                                            <div style="float: left; padding-right: 10px; padding-top: 3px;" id="divButtonSequence">
                                                <div>
                                                    <a href="javascript: void(0)" id="btnAddNewAccessGroup" runat="server" onclick="addShift(); " data-toggle="tooltip" title="Añadir horario">
                                                        <span class="btnIconAdd"></span>
                                                    </a>
                                                </div>
                                                <%--      <dx:ASPxButton ID="buttonSequence" runat="server" AutoPostBack="False" CausesValidation="False"  Text="Añadir" ToolTip="Añadir">
                                                 <ClientSideEvents Click="function(s,e){ addShift(); }" />
                                             </dx:ASPxButton>--%>
                                            </div>

                                            <div style="float: left; padding-right: 10px; padding-top: 3px;" id="divDeleteSequence">

                                                <div>
                                                    <a href="javascript: void(0)" id="A1" runat="server" onclick="deleteTokens();" data-toggle="tooltip" title="Eliminar secuencia">
                                                        <span class="btnIconCancel"></span>
                                                    </a>
                                                </div>

                                                <%--                                             <dx:ASPxButton ID="buttonDelete" runat="server" AutoPostBack="False" CausesValidation="False"  Text="Borrar" ToolTip="Borrar">
                                                 <ClientSideEvents Click="function(s,e){ deleteTokens(); }" />
                                             </dx:ASPxButton>--%>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="divRow">
                                        <div class="divRowDescription">
                                            <asp:Label ID="emptytitle" runat="server" Text=" "></asp:Label>
                                        </div>
                                        <span id="emptylabel" class="labelForm">&nbsp;</span>
                                        <div class="componentForm">
                                            <div style="float: left; padding-right: 10px; max-height: 100px; overflow-y: auto" id="divTokenSequence">
                                                <dx:ASPxTokenBox ID="tbMinMaxShiftsSequence" runat="server" Width="100%" Height="50px" ClientInstanceName="tbMinMaxShiftsSequenceClient">
                                                    <ClientSideEvents TextChanged="" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" Init="function(s, e) {dxcollection.push(s);}" />
                                                    <TokenRemoveButtonStyle CssClass="HideRemoveButton"></TokenRemoveButtonStyle>
                                                </dx:ASPxTokenBox>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="divRow">
                                        <div class="divRowDescription" style="padding-top: 5px !important">
                                            <asp:Label ID="txtSequencePeriod" runat="server" Text="Período en el que se calculará la regla"></asp:Label>
                                        </div>
                                        <asp:Label ID="lblEverySequence" runat="server" Text="Cada:" CssClass="labelForm"></asp:Label>
                                        <div class="componentForm">

                                            <div style="float: left; padding-right: 10px;" id="divPeriodSequence">
                                                <dx:ASPxTextBox runat="server" ID="txtPeriodictySequence" MaxLength="6" Width="40" ClientInstanceName="txtPeriodictySequenceClient">
                                                    <ClientSideEvents TextChanged="function(s,e){ hasChanges(true)}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                    <ValidationSettings ErrorDisplayMode="None">
                                                    </ValidationSettings>
                                                    <MaskSettings Mask="<1..99999>" />
                                                </dx:ASPxTextBox>
                                            </div>
                                            <div style="float: left; padding-right: 10px; padding-top: 3px">
                                                <dx:ASPxComboBox runat="server" ID="cmbMinMaxShiftsSequence" Width="100px" NullText="_____">
                                                    <ClientSideEvents GotFocus="HightlightOnGotFocus" SelectedIndexChanged="function(s,e){ periodChangedSequence(s.GetSelectedItem().value)}" LostFocus="FadeOnLostFocus" Init="function(s, e) {dxcollection.push(s);}" />
                                                </dx:ASPxComboBox>
                                            </div>
                                            <div style="float: left; padding-right: 5px; padding-top: 3px" id="divAlwaysSequence">
                                                <dx:ASPxCheckBox ID="ckAlwaysSequence" runat="server" Width="105px" Text="entre periodos" ClientInstanceName="ckAlwaysSequenceClient">
                                                    <ClientSideEvents Init="function(s, e) {dxcollection.push(s);}" CheckedChanged="enablePeriodSequence" />
                                                </dx:ASPxCheckBox>
                                            </div>

                                            <div style="float: left; padding-right: 10px; padding-top: 5px;" id="divEntreSequence">
                                                <asp:Label ID="txtSequenceFrom" runat="server" Text="entre"></asp:Label>
                                            </div>
                                            <div style="float: left; padding-right: 10px; padding-top: 3px" id="divFromSequence">
                                                <dx:ASPxDateEdit ID="txtDateSequenceFrom" runat="server" PopupVerticalAlign="WindowCenter" DisplayFormatString="dd/MM" EditFormat="Custom" EditFormatString="dd/MM" AllowNull="False" Width="70px" CssClass="editTextFormat" Font-Size="11px" ClientInstanceName="txtDateSequenceFromClient">
                                                </dx:ASPxDateEdit>
                                            </div>
                                            <div style="float: left; padding-right: 10px; padding-top: 5px;" id="divHastaSequence">
                                                <asp:Label ID="txtSequenceTo" runat="server" Text="y"></asp:Label>
                                            </div>
                                            <div style="float: left; padding-top: 3px" id="divToSequence">
                                                <dx:ASPxDateEdit ID="txtDateSequenceTo" runat="server" DisplayFormatString="dd/MM" EditFormat="Custom" EditFormatString="dd/MM" AllowNull="False" Width="70px" CssClass="editTextFormat" Font-Size="11px" ClientInstanceName="txtDateSequenceToClient">
                                                </dx:ASPxDateEdit>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="divRow">
                                        <div class="divRowDescription">
                                            <asp:Label ID="lblMinShiftsSequenceRepeatDesc" runat="server" Text="Número mínimo de días planificados con el horario"></asp:Label>
                                        </div>
                                        <asp:Label ID="lblMinShiftsSequenceRepeatTitle" runat="server" Text="Mínimo:" CssClass="labelForm"></asp:Label>
                                        <div class="componentForm">
                                            <div style="float: left">
                                                <dx:ASPxCheckBox ID="ckMinShiftsSequenceRepeat" runat="server" Text="">
                                                    <ClientSideEvents Init="function(s, e) {dxcollection.push(s);}" />
                                                </dx:ASPxCheckBox>
                                            </div>
                                            <div style="float: left; padding-left: 10px">
                                                <dx:ASPxTextBox runat="server" ID="txtMinShiftsSequenceRepeat" MaxLength="12" Width="75" ValidationSettings-Display="None">
                                                    <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" Init="function(s, e) {dxcollection.push(s);}" />
                                                    <MaskSettings Mask="<0..366>" ErrorText="(*)" />
                                                </dx:ASPxTextBox>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="divRow">
                                        <div class="divRowDescription">
                                            <asp:Label ID="lblMaxShiftsSequenceRepeatDesc" runat="server" Text="Número máximo de días planificados con el horario"></asp:Label>
                                        </div>
                                        <asp:Label ID="lblMaxShiftsSequenceRepeatTitle" runat="server" Text="Máximo:" CssClass="labelForm"></asp:Label>
                                        <div class="componentForm">
                                            <div style="float: left">
                                                <dx:ASPxCheckBox ID="ckMaxShiftsSequenceRepeat" runat="server" Text="">
                                                    <ClientSideEvents Init="function(s, e) {dxcollection.push(s);}" />
                                                </dx:ASPxCheckBox>
                                            </div>
                                            <div style="float: left; padding-left: 10px">
                                                <dx:ASPxTextBox runat="server" ID="txtMaxShiftsSequenceRepeat" MaxLength="12" Width="75" ValidationSettings-Display="None">
                                                    <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" Init="function(s, e) {dxcollection.push(s);}" />
                                                    <MaskSettings Mask="<0..366>" ErrorText="(*)" />
                                                </dx:ASPxTextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div id="scheduleRuleType_15" style="display: none"></div>
                            </div>
                        </div>
                        <div style="width: 100%;">
                            <table border="0" style="width: 100%;">
                                <tr>
                                    <td>&nbsp;</td>
                                    <td id="tdSaveReaOnlyScheduleRule" style="width: 110px;" align="right">
                                        <dx:ASPxButton ID="btnOk" runat="server" AutoPostBack="False" CausesValidation="False" Text="Aceptar" ToolTip="Aceptar" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                            <ClientSideEvents Click="function(s,e){ frmEditScheduleRules_Save(); }" />
                                        </dx:ASPxButton>
                                    </td>
                                    <td style="width: 110px;" align="left">
                                        <dx:ASPxButton ID="btnCancel" runat="server" AutoPostBack="False" CausesValidation="False" Text="Cancelar" ToolTip="Cancelar" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                            <ClientSideEvents Click="function(s,e){ frmEditScheduleRules_Close(); }" />
                                        </dx:ASPxButton>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>
                </dx:PanelContent>
            </PanelCollection>
        </dx:ASPxCallbackPanel>
    </div>
</div>
