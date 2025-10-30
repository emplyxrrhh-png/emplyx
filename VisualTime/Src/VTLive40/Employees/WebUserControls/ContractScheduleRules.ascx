<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.WebUserControls_ContractScheduleRules" CodeBehind="ContractScheduleRules.ascx.vb" %>

<%@ Register Src="~/LabAgree/WebUserForms/frmEditScheduleRules.ascx" TagName="frmEditScheduleRules" TagPrefix="roForms" %>

<div id="<%= Me.ClientID %>_frm" style="position: fixed; z-index: 9010; display: none; top: 50%; left: 50%; width: 1400px;">
    <div id="<%= Me.ClientID %>_BgS" style="position: absolute; top: 0; left: 0; display: none; z-index: 9009;"></div>
    <div class="bodyPopupExtended">
        <dx:ASPxCallbackPanel ID="ContractScheduleRulesCallback" runat="server" Width="100%" Height="100%" ClientInstanceName="ContractScheduleRulesCallbackClient">
            <SettingsLoadingPanel Enabled="false" />
            <ClientSideEvents EndCallback="ContractScheduleRulesCallback_EndCallBack" />
            <PanelCollection>
                <dx:PanelContent ID="PanelContent1" runat="server">
                    <div id="divContentPanels" class="contractScheduleRules" style="padding-right: 20px">
                        <div id="div00" class="contentPanel" runat="server" name="menuPanel">

                            <div id="main" style="display: flex;">
                                <div id="divSchedulingRules" style="flex:0.5;" runat="server">
                                    <div class="panBottomMargin">
                                        <div class="panHeader2 panBottomMargin">
                                            <span class="panelTitleSpan">
                                                <asp:Label runat="server" ID="lblGeneralTitle" Text="Parámetros configurables"></asp:Label>
                                            </span>
                                        </div>
                                        <!-- La descripción es opcional -->
                                        <div style="text-align: left; padding-left: 15px; padding-top: 15px;">
                                            <asp:Label ID="lblParameterDesc" runat="server" Text="Los parametros activos sobreescribirán el valor de convenio equivalente con el indicado. Para que vuelva a aplicar el valor de convenio, debe descarcar el interruptor."></asp:Label>
                                        </div>
                                    </div>

                                    <div class="panBottomMargin">
                                        <div class="divRow">
                                            <div class="divRowDescription">
                                                <asp:Label ID="lblYearHoursDesc" runat="server" Text="Horas anuales que marca el convenio"></asp:Label>
                                            </div>
                                            <asp:Label ID="lblYearHoursTitle" runat="server" Text="Horas anuales:" CssClass="labelForm"></asp:Label>
                                            <div class="componentForm">
                                                <div style="float: left; padding-right: 10px">
                                                    <div id="switchYearHours"></div>
                                                </div>
                                                <div style="margin-top: -3px;">
                                                    <div id="divtxtYearHours" style="float: left">
                                                        <dx:ASPxTextBox runat="server" ID="txtYearHours" MaxLength="12" Width="75" ClientInstanceName="txtYearHoursClient">
                                                            <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                            <ValidationSettings ErrorDisplayMode="None">
                                                            </ValidationSettings>
                                                            <MaskSettings Mask="<0..999999>" />
                                                        </dx:ASPxTextBox>
                                                    </div>
                                                    <div id="divlblFork" style="float: left; padding-right: 10px; line-height: 20px; color: #2D4155;">
                                                        <asp:Label ID="lblFork" runat="server" Text="Horquilla:"></asp:Label>
                                                    </div>
                                                    <div id="divtxtFork" style="float: left; padding-right: 10px">
                                                        <dx:ASPxTextBox runat="server" ID="txtFork" MaxLength="12" Width="75" ClientInstanceName="txtForkClient">
                                                            <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                            <ValidationSettings ErrorDisplayMode="None">
                                                            </ValidationSettings>
                                                            <MaskSettings Mask="<0..999999>" />
                                                        </dx:ASPxTextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="divRow">
                                            <div class="divRowDescription">
                                                <asp:Label ID="lblHolidayYearDaysDesc" runat="server" Text="Días de vacaciones anuales marcados por el convenio"></asp:Label>
                                            </div>
                                            <asp:Label ID="lblHolidayYearDays" runat="server" Text="Días anuales:" CssClass="labelForm"></asp:Label>
                                            <div class="componentForm">
                                                <div style="float: left; padding-right: 10px">
                                                    <div id="switchYearHolidays"></div>
                                                </div>
                                                <div style="float: left; padding-right: 10px">
                                                    <dx:ASPxTextBox runat="server" ID="txtYearHolidays" MaxLength="12" Width="75" ClientInstanceName="txtYearHolidaysClient">
                                                        <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                        <MaskSettings Mask="<0..366>" />
                                                    </dx:ASPxTextBox>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="divRow">
                                            <div class="divRowDescription">
                                                <asp:Label ID="lblWorkingDaysDesc" runat="server" Text="Indica los días de la semana laborales"></asp:Label>
                                            </div>
                                            <asp:Label ID="lblWorkingDaysTitle" runat="server" Text="Días laborables:" CssClass="labelForm"></asp:Label>
                                            <div class="componentForm">
                                                <div style="float: left; padding-right: 10px">
                                                    <div id="switchWorkingDays"></div>
                                                </div>
                                                <div style="float: left">
                                                    <dx:ASPxTokenBox ID="tbWorkingDays" ClientInstanceName="tbWorkingDaysClient" runat="server" Width="100%">
                                                        <ClientSideEvents TextChanged="function(s,e){ hasChanges(true); updatePatternColumns();}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                    </dx:ASPxTokenBox>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="divRow">
                                            <div class="divRowDescription">
                                                <asp:Label ID="lblCanWorkOnFeastDesc" runat="server" Text="Indica si se permite que se trabaje en días festivos"></asp:Label>
                                            </div>
                                            <asp:Label ID="lblCanWorkOnFeastTitle" runat="server" Text="Días festivos:" CssClass="labelForm"></asp:Label>
                                            <div class="componentForm">
                                                <div style="float: left; padding-right: 10px">
                                                    <div id="switchCanWorkOnFeastDays"></div>
                                                </div>
                                                <div style="float: left">
                                                    <dx:ASPxCheckBox type="checkbox" runat="server" ID="chkCanWorkOnFeastDays" Text="" ClientInstanceName="chkCanWorkOnFeastDaysClient">
                                                        <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                    </dx:ASPxCheckBox>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="divRow">
                                            <div class="divRowDescription">
                                                <asp:Label ID="lblCanWorkOnNonWorkingDaysDesc" runat="server" Text="Indica si se permite que se trabaje en días no laborales"></asp:Label>
                                            </div>
                                            <asp:Label ID="lblCanWorkOnNonWorkingDaysTitle" runat="server" Text="Días no laborales:" CssClass="labelForm"></asp:Label>
                                            <div class="componentForm">
                                                <div style="float: left; padding-right: 10px">
                                                    <div id="switchCanWorkOnNonWorkingDays"></div>
                                                </div>
                                                <div style="float: left">
                                                    <dx:ASPxCheckBox type="checkbox" runat="server" ID="chkCanWorkOnNonWorkingDays" Text="" ClientInstanceName="chkCanWorkOnNonWorkingDaysClient">
                                                        <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                    </dx:ASPxCheckBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div style="flex: 1;width: 100%;min-width: 60%;margin-left: 10px;">
                                    <div id="divTelecommutingGeneral" runat="server">
                                        <div class="panHeader2">
                                            <span style="">
                                                <asp:Label runat="server" ID="lblTelecommuting" Text="Teletrabajo"></asp:Label>
                                            </span>
                                        </div>
                                        <div class="divRow">
                                            <roUserControls:roOptionPanelClient ID="optOverwriteTelecommuting" CConClick="enableTCMain()" runat="server" TypeOPanel="CheckboxOption" width="100%" height="Auto" Checked="False" Enabled="True" style="width: 100%;">
                                                <Title>
                                                    <asp:Label ID="optOverwriteTelecommutingTitle" runat="server" Text="Sobreescribir las condiciones de teletrabajo."></asp:Label>
                                                </Title>
                                                <Description>
                                                    <asp:Label ID="optOverwriteTelecommutingDesc" runat="server" Text="Si sobreescribes las condiciones de teletrabajo no aplicará ninguna de las definidas en el convenio."></asp:Label>
                                                </Description>
                                                <Content>
                                                    <div class="divRow">
                                                        <div style="float: left; padding-top: 15px; padding-left: 15px;">
                                                            <asp:Label ID="lblCanTelecommuteTitle" runat="server" Text="¿Se permite teletrabajar?" CssClass="labelForm bigLabAgreeText"></asp:Label>
                                                        </div>

                                                        <div style="clear: both; padding-bottom: 10px;">
                                                            <div style="float: left; padding-right: 20px">
                                                                <dx:ASPxRadioButton GroupName="CanTelecommute" ID="ckTelecommuteYes" runat="server" ClientInstanceName="ckTelecommuteYes_client" Text="Sí">
                                                                    <ClientSideEvents CheckedChanged="function(s,e){ enableTC();}" />
                                                                </dx:ASPxRadioButton>
                                                            </div>
                                                            <div style="float: left; padding-right: 20px;">
                                                                <dx:ASPxRadioButton GroupName="CanTelecommute" ID="ckTelecommuteNo" runat="server" ClientInstanceName="ckTelecommuteNo_client" Text="No">
                                                                    <ClientSideEvents CheckedChanged="function(s,e){ enableTC();}" />
                                                                </dx:ASPxRadioButton>
                                                            </div>

                                                            <div style="float: left; padding-bottom: 10px;">
                                                                <div style="float: left; padding-top: 10px;">
                                                                    <asp:Label ID="txtCanTelecommuteFromDesc" runat="server" Text="Desde" CssClass="labelForm lblminWidth"></asp:Label>
                                                                </div>

                                                                <div id="divtxtCanTelecommuteFrom" style="float: left; padding-top: 5px;">
                                                                    <dx:ASPxDateEdit runat="server" PopupVerticalAlign="WindowCenter" EditFormat="Date" ID="txtCanTelecommuteFrom" Width="150px" AllowNull="false" ClientInstanceName="txtCanTelecommuteFromClient">
                                                                    </dx:ASPxDateEdit>
                                                                </div>
                                                                <div style="float: left; padding-top: 10px;">
                                                                    <asp:Label ID="txtCanTelecommuteToDesc" runat="server" Text="hasta" CssClass="labelForm lblminWidth"></asp:Label>
                                                                </div>
                                                                <div id="divtxtCanTelecommuteTo" style="float: left; padding-top: 5px; padding-left: 20px;">
                                                                    <dx:ASPxDateEdit runat="server" PopupVerticalAlign="WindowCenter" EditFormat="Date" ID="txtCanTelecommuteTo" Width="150px" AllowNull="false" ClientInstanceName="txtCanTelecommuteToClient">
                                                                    </dx:ASPxDateEdit>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <br />
                                                    <div class="divRow">
                                                        <asp:Label ID="lblTelecommutingDaysTitle" runat="server" Text="¿Cuántos días podrá teletrabajar?" CssClass="labelForm bigLabAgreeText"></asp:Label>
                                                        <div class="componentForm" style="margin-top: -3px; margin-bottom: 5px;">

                                                            <div class="divRow">

                                                                <div style="float: left; clear: both">
                                                                    <%--   <dx:ASPxCheckBox ID="chkTCOptionalDays" ClientInstanceName="chkTCOptionalDaysClient" runat="server" Text="Podrán escoger">
                                                            <ClientSideEvents CheckedChanged="function(s,e){  hasChanges(true,false); }" />
                                                        </dx:ASPxCheckBox>--%>
                                                                    <dx:ASPxLabel ID="txtTCOptionalDays" CssClass="roLabel" runat="server" Text="Podrán escoger" />
                                                                </div>
                                                                <div style="float: left; padding-top: 3px;">
                                                                    <dx:ASPxTextBox runat="server" ID="txtTelecommutingMaxOptional" MaxLength="3" Width="35" ClientInstanceName="txtTelecommutingMaxOptional_Client">
                                                                        <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                        <MaskSettings Mask="<0..100>" />
                                                                        <ValidationSettings Display="None" />
                                                                    </dx:ASPxTextBox>
                                                                </div>
                                                                <div style="float: left; padding-left: 10px">
                                                                    <dx:ASPxComboBox ID="cmbDaysOrPercent" runat="server" ClientInstanceName="cmbDaysOrPercentClient" Width="150px">
                                                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ hasChanges(true)}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                    </dx:ASPxComboBox>
                                                                </div>
                                                                <div style="float: left; padding-left: 10px">
                                                                    <dx:ASPxLabel ID="lblOf" CssClass="roLabel" runat="server" Text="de" />
                                                                </div>

                                                                <div style="float: left; padding-left: 10px">
                                                                    <dx:ASPxComboBox ID="cmbWeekOrMonth" runat="server" ClientInstanceName="cmbWeekOrMonthClient" Width="100px">
                                                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ hasChanges(true)}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                    </dx:ASPxComboBox>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <br />
                                                    <div class="divRow">
                                                        <div class="" style="padding-bottom: 10px;">
                                                            <asp:Label ID="txtTelecommutingDistributionDesc" runat="server" Text="Los usuarios distribuirán semanalmente su jornada de teletrabajo de la siguiente forma:"></asp:Label>
                                                        </div>
                                                    </div>
                                                    <div class="divRow">

                                                        <div id="divTelecommutingPatternGrid" runat="server" class="jsGridContentSmall dextremeGrid">
                                                        </div>
                                                    </div>
                                                    <br />
                                                </Content>
                                            </roUserControls:roOptionPanelClient>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <!-- Este div es el header -->

                            <div class="divRow">
                                <roUserControls:roOptionPanelClient ID="optOverwriteScheduleRules" runat="server" TypeOPanel="CheckboxOption" width="100%" height="Auto" Checked="False" Enabled="True" style="width: 100%;">
                                    <Title>
                                        <asp:Label ID="optOverwriteScheduleRulesTitle" runat="server" Text="Sobreescribir las reglas de planificación."></asp:Label>
                                    </Title>
                                    <Description>
                                        <asp:Label ID="optOverwriteScheduleRulesDesc" runat="server" Text="Si sobreescribe las reglas de planificación no aplicará ninguna de las definidas en el convenio."></asp:Label>
                                    </Description>
                                    <Content>
                                        <div class="jsGrid">
                                            <asp:Label ID="lblLabAgreeScheduleRule" runat="server" CssClass="jsGridTitle" Text="Reglas de planificación"></asp:Label>
                                            <div class="jsgridButton">
                                                <dx:ASPxButton ID="btnAddNewLabAgreeScheduleRules" runat="server" AutoPostBack="False" CausesValidation="False" Text="Añadir" ToolTip="Añadir" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat">
                                                    <Image Url="~/Base/Images/Grid/add.png"></Image>
                                                    <ClientSideEvents Click="AddNewContractScheduleRule" />
                                                </dx:ASPxButton>
                                            </div>
                                        </div>
                                        <dx:ASPxGridView ID="GridContractScheduleRules" runat="server" AutoGenerateColumns="False" ClientInstanceName="GridContractScheduleRulesClient" KeyboardSupport="True" Width="100%">
                                            <SettingsBehavior AllowFocusedRow="True" AllowSelectSingleRowOnly="True" AllowSort="False" />
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
                                            <SettingsPager Mode="ShowAllRecords" ShowEmptyDataRows="false" />
                                            <Border BorderColor="#CDCDCD" />
                                            <SettingsLoadingPanel Text="" />
                                            <ClientSideEvents BeginCallback="GridContractScheduleRules_BeginCallback" CustomButtonClick="GridContractScheduleRules_CustomButtonClick"
                                                EndCallback="GridContractScheduleRules_EndCallback" RowDblClick="GridContractScheduleRules_OnRowDblClick" FocusedRowChanged="GridContractScheduleRules_FocusedRowChanged" />
                                            <Settings ShowTitlePanel="false" VerticalScrollBarMode="Auto" UseFixedTableLayout="True" VerticalScrollableHeight="150" />
                                        </dx:ASPxGridView>
                                    </Content>
                                </roUserControls:roOptionPanelClient>
                            </div>
                            <roForms:frmEditScheduleRules ID="frmEditScheduleRules" Mode="Contract" runat="server" />
                        </div>
                        <div style="width: 100%;">
                            <table border="0" style="width: 100%;">
                                <tr>
                                    <td>&nbsp;</td>
                                    <td style="width: 110px;" align="right">
                                        <dx:ASPxButton ID="btnOk" runat="server" AutoPostBack="False" CausesValidation="False" Text="Aceptar" ToolTip="Aceptar" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                            <ClientSideEvents Click="function(s,e){ frmShowContractScheduleRules_Save(); }" />
                                        </dx:ASPxButton>
                                    </td>
                                    <td style="width: 110px;" align="left">
                                        <dx:ASPxButton ID="btnCancel" runat="server" AutoPostBack="False" CausesValidation="False" Text="Cancelar" ToolTip="Cancelar" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                            <ClientSideEvents Click="function(s,e){ frmShowContractScheduleRules_Close(); }" />
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