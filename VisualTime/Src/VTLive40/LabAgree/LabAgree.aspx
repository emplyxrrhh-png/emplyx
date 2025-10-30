<%@ Page Language="VB" MasterPageFile="~/Base/mastEmp.master" AutoEventWireup="false" Inherits="VTLive40.LabAgree" Title="${LabAgree}" CodeBehind="LabAgree.aspx.vb" %>

<%@ Register Src="~/LabAgree/WebUserForms/frmEditStartupValue.ascx" TagName="frmEditStartupValue" TagPrefix="roForms" %>
<%@ Register Src="~/LabAgree/WebUserForms/frmEditLabAgreeRule.ascx" TagName="frmEditLabAgreeRule" TagPrefix="roForms" %>
<%@ Register Src="~/LabAgree/WebUserForms/frmEditCauseLimit.ascx" TagName="frmEditCauseLimit" TagPrefix="roForms" %>
<%@ Register Src="~/LabAgree/WebUserForms/frmEditRequestValidation.ascx" TagName="frmEditRequestValidation" TagPrefix="roForms" %>
<%@ Register Src="~/LabAgree/WebUserForms/frmEditScheduleRules.ascx" TagName="frmEditScheduleRules" TagPrefix="roForms" %>

<asp:Content ID="cMainBody" ContentPlaceHolderID="contentMainBody" runat="Server">

    <script language="javascript" type="text/javascript">

        function PageBase_Load() {
            resizeFrames();
            resizeTreeLabAgrees();
        }
    </script>

    <div id="divModalBgDisabled" class="modalBackground" style="position: absolute; left: 0px; top: 0px; z-index: 990; width: 1680px; height: 900px; display: none;"></div>
    <input type="hidden" id="dateFormatValue" runat="server" value="" />
    <input type="hidden" runat="server" id="noRegs" value="" />
    <input type="hidden" runat="server" id="hdnModeEdit" value="" />

    <input type="hidden" id="msgHasChanges" value="" runat="server" clientidmode="Static" />
    <input type="hidden" id="msgHasErrors" value="" runat="server" clientidmode="Static" />
    <div id="divMainBody">
        <!-- TAB SUPERIOR -->
        <div id="divTabInfo" class="divDataCells">
            <div style="min-height: 10px"></div>
            <div id="divLabAgrees" class="blackRibbonTitle">
            </div>
            <div style="min-height: 10px"></div>
        </div>
        <!-- FIN TAB SUPERIOR -->

        <!-- ARBOL Y DETALLE -->
        <div id="divTabData" class="divDataCells">

            <div id="divTree" class="treeSize">
                <div id="ctlTreeDiv" style="height: 500px;">
                    <rws:roTreesSelector ID="roTreesLabAgrees" runat="server" ShowEmployeeFilters="false" PrefixTree="roTreesLabAgrees"
                        Tree1Visible="true" Tree1MultiSel="false" Tree1ShowOnlyGroups="false" Tree1Function="cargaNodo" Tree1ImagePath="images/LabAgreeSelector" Tree1SelectorPage="../../LabAgree/LabAgreeSelectorData.aspx"
                        ShowTreeCaption="true"></rws:roTreesSelector>
                </div>
            </div>

            <div id="divButtons" class="divMiddleButtons">
                <div id="divBarButtons" class="maxHeight">&nbsp</div>
            </div>

            <div id="divContenido" class="divRightContent">
                <div id="divContent" class="maxHeight">
                    <dx:ASPxCallbackPanel ID="ASPxCallbackPanelContenido" runat="server" Width="100%" Height="100%" ClientInstanceName="ASPxCallbackPanelContenidoClient">
                        <SettingsLoadingPanel Enabled="false" />
                        <ClientSideEvents EndCallback="ASPxCallbackPanelContenidoClient_EndCallBack" />
                        <PanelCollection>
                            <dx:PanelContent ID="PanelContent1" runat="server">
                                <div id="divMsgTop" class="divMsg2 divMessageTop" style="display: none">
                                    <div class="divImageMsg">
                                        <img alt="" id="Img1" src="~/Base/Images/MessageFrame/Alert16.png" runat="server" />
                                    </div>
                                    <div class="messageText">
                                        <span id="msgTop"></span>
                                    </div>
                                    <div align="right" class="messageActions">
                                        <a href="javascript: void(0);" class="aMsg" onclick="saveChanges();"><span id="lblSaveChanges" runat="server" /></a>
                                        &nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;
                                        <a href="javascript: void(0);" onclick="undoChanges();" class="aMsg"><span id="lblUndoChanges" runat="server" /></a>
                                    </div>
                                </div>
                                <div id="divContentPanels" class="divContentPanelsWithOutMessage">
                                    <!-- Panell General -->
                                    <div id="div00" class="contentPanel" runat="server" name="menuPanel" style="height: auto;">
                                        <!-- Este div es el header -->
                                        <div class="panBottomMargin">
                                            <div class="panHeader2 panBottomMargin">
                                                <span class="panelTitleSpan">
                                                    <asp:Label runat="server" ID="lblLabAgreesTitleGeneral" Text="General"></asp:Label>
                                                </span>
                                            </div>
                                            <!-- La descripción es opcional -->
                                            <div class="panelHeaderContent">
                                                <div class="panelDescriptionImage">
                                                    <img alt="" src="<%=Me.Page.ResolveUrl("~/LabAgree/Images/LabAgree.png")%>" />
                                                </div>
                                                <div class="panelDescriptionText">
                                                    <asp:Label ID="lblLabAgreesDesc" runat="server" Text="Datos especificos para convenios."></asp:Label>
                                                </div>
                                            </div>
                                        </div>

                                        <!-- Este div es un formulario -->
                                        <div class="panBottomMargin">
                                            <div class="divRow">
                                                <div class="divRowDescription">
                                                    <asp:Label ID="lblNameDescription" runat="server" Text="Nombre del convenio"></asp:Label>
                                                </div>
                                                <asp:Label ID="lblName" runat="server" Text="Nombre:" CssClass="labelForm"></asp:Label>
                                                <div class="componentForm">
                                                    <dx:ASPxTextBox ID="txtName" runat="server" ClientInstanceName="txtLabAgreeName_Client" NullText="_____">
                                                        <ClientSideEvents Validation="LengthValidation" TextChanged="function(s,e){checkLabAgreeEmptyName(s.GetValue());}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                        <ValidationSettings SetFocusOnError="True" ValidationGroup="nameDesc">
                                                            <RequiredField IsRequired="True" ErrorText="(*)" />
                                                        </ValidationSettings>
                                                    </dx:ASPxTextBox>
                                                </div>
                                            </div>

                                            <div class="divRow">
                                                <div class="divRowDescription">
                                                    <asp:Label ID="lblDescLabAgreeDescription" runat="server" Text="Descripción del convenio"></asp:Label>
                                                </div>
                                                <asp:Label ID="lblDescLabAgree" runat="server" Text="Descripción:" CssClass="labelForm"></asp:Label>
                                                <div class="componentForm">
                                                    <dx:ASPxMemo ID="txtDescription" runat="server" Rows="4" Width="100%" Height="40" ValidationSettings-ValidationGroup="nameDesc">
                                                        <ClientSideEvents TextChanged="function(s,e){ hasChanges(true)}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                    </dx:ASPxMemo>
                                                </div>
                                            </div>

                                            <div class="divRow">
                                                <div class="divRowDescription">
                                                    <asp:Label ID="lblYearHoursDesc" runat="server" Text="Horas anuales que marca el convenio"></asp:Label>
                                                </div>
                                                <asp:Label ID="lblYearHoursTitle" runat="server" Text="Horas anuales:" CssClass="labelForm"></asp:Label>
                                                <div class="componentForm" style="margin-top: -3px; margin-bottom: 5px;">

                                                    <div style="clear: both; padding-bottom: 10px;">
                                                        <div style="float: left; padding-right: 20px">
                                                            <dx:ASPxRadioButton GroupName="HoursYears" ID="ckFixedValue" runat="server" ClientInstanceName="ckFixedValue_client" Text="Fijas">
                                                                <ClientSideEvents CheckedChanged="ckFixedValue_client_CheckedChanged" />
                                                            </dx:ASPxRadioButton>
                                                        </div>
                                                        <div id="divtxtYearHours" style="float: left; padding-top: 3px;">
                                                            <dx:ASPxTextBox runat="server" ID="txtYearHours" MaxLength="12" Width="75" ClientInstanceName="txtYearHoursClient">
                                                                <ClientSideEvents TextChanged="function(s,e){ hasChanges(true)}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                <ValidationSettings ErrorDisplayMode="None">
                                                                </ValidationSettings>
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
                                                                <ClientSideEvents CheckedChanged="ckUserField_client_CheckedChanged" />
                                                            </dx:ASPxRadioButton>
                                                        </div>
                                                        <div style="float: left; padding-left: 3px; padding-top: 3px;" id="divcmbUserField">
                                                            <dx:ASPxComboBox ID="cmbUserField" runat="server" ClientInstanceName="cmbUserFieldClient" Width="215px">
                                                                <ClientSideEvents SelectedIndexChanged="function(s,e){ hasChanges(true)}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                            </dx:ASPxComboBox>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="divRow">

                                                <asp:Label ID="divlblFork" runat="server" Text="Horquilla:" CssClass="labelForm"></asp:Label>
                                                <div class="componentForm" style="margin-top: -3px; margin-bottom: 5px;">

                                                    <div style="clear: both; padding-bottom: 10px;">

                                                        <div id="divtxtFork" style="float: left;">
                                                            <dx:ASPxTextBox runat="server" ID="txtFork" MaxLength="12" Width="75" ClientInstanceName="txtForkClient">
                                                                <ClientSideEvents TextChanged="function(s,e){ hasChanges(true)}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                <ValidationSettings ErrorDisplayMode="None">
                                                                </ValidationSettings>
                                                                <MaskSettings Mask="<0..999999>" />
                                                            </dx:ASPxTextBox>
                                                        </div>
                                                    </div>
                                                </div>
                                                <br />
                                            </div>

                                            <div class="divRow">
                                                <div class="divRowDescription">
                                                    <asp:Label ID="lblHolidayYearDaysDesc" runat="server" Text="Días de vacaciones anuales marcados por el convenio"></asp:Label>
                                                </div>
                                                <asp:Label ID="lblHolidayYearDays" runat="server" Text="Días anuales:" CssClass="labelForm"></asp:Label>
                                                <div class="componentForm">
                                                    <dx:ASPxTextBox runat="server" ID="txtYearHolidays" MaxLength="12" Width="75">
                                                        <ClientSideEvents TextChanged="function(s,e){ hasChanges(true)}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                        <MaskSettings Mask="<0..366>" />
                                                    </dx:ASPxTextBox>
                                                </div>
                                            </div>

                                            <div class="divRow">
                                                <div class="divRowDescription">
                                                    <asp:Label ID="lblWorkingDaysDesc" runat="server" Text="Indica los días de la semana laborales"></asp:Label>
                                                </div>
                                                <asp:Label ID="lblWorkingDaysTitle" runat="server" Text="Días laborables:" CssClass="labelForm"></asp:Label>
                                                <div class="componentForm">
                                                    <dx:ASPxTokenBox ID="tbWorkingDays" ClientInstanceName="tbWorkingDaysClient" runat="server" Width="100%">
                                                        <ClientSideEvents TextChanged="function(s,e){ hasChanges(true); updatePatternColumns();}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                    </dx:ASPxTokenBox>
                                                </div>
                                            </div>

                                            <div class="divRow">
                                                <div class="divRowDescription">
                                                    <asp:Label ID="lblCanWorkOnFeastDesc" runat="server" Text="Indica si se permite que se trabaje en días festivos"></asp:Label>
                                                </div>
                                                <asp:Label ID="lblCanWorkOnFeastTitle" runat="server" Text="Días festivos:" CssClass="labelForm"></asp:Label>
                                                <div class="componentForm">
                                                    <input type="checkbox" runat="server" id="chkCanWorkOnFeastDays" onchange="hasChanges(true);" />
                                                </div>
                                            </div>

                                            <div class="divRow">
                                                <div class="divRowDescription">
                                                    <asp:Label ID="lblCanWorkOnNonWorkingDaysDesc" runat="server" Text="Indica si se permite que se trabaje en días no laborales"></asp:Label>
                                                </div>
                                                <asp:Label ID="lblCanWorkOnNonWorkingDaysTitle" runat="server" Text="Días no laborales:" CssClass="labelForm"></asp:Label>
                                                <div class="componentForm">
                                                    <input type="checkbox" runat="server" id="chkCanWorkOnNonWorkingDays" onchange="hasChanges(true);" />
                                                </div>
                                            </div>

                                            <br />

                                            <div id="divTelecommutingGeneral" runat="server">
                                                <div class="panHeader2">
                                                    <span style="">
                                                        <asp:Label runat="server" ID="lblTelecommuting" Text="Teletrabajo"></asp:Label></span>
                                                </div>
                                                <br />

                                                <div class="divRow">
                                                    <div class="divRowDescription">
                                                        <asp:Label ID="lblCanTelecommuteDesc" runat="server" Text="Indica si el usuario puede hacer teletrabajo"></asp:Label>
                                                    </div>
                                                    <asp:Label ID="lblCanTelecommuteTitle" runat="server" Text="¿Se permite teletrabajar?" CssClass="labelForm bigLabAgreeText"></asp:Label>
                                                    <div class="componentForm" style="margin-top: -3px; margin-bottom: 5px;">

                                                        <div style="clear: both; padding-bottom: 10px;">
                                                            <div style="float: left; padding-right: 20px">
                                                                <dx:ASPxRadioButton GroupName="CanTelecommute" ID="ckTelecommuteYes" runat="server" ClientInstanceName="ckTelecommuteYes_client" Text="Sí">
                                                                    <ClientSideEvents CheckedChanged="function(s,e){ enableTC(); hasChanges(true);}" />
                                                                </dx:ASPxRadioButton>
                                                            </div>
                                                            <div style="float: left; padding-right: 20px;">
                                                                <dx:ASPxRadioButton GroupName="CanTelecommute" ID="ckTelecommuteNo" runat="server" ClientInstanceName="ckTelecommuteNo_client" Text="No">
                                                                    <ClientSideEvents CheckedChanged="function(s,e){ hasChanges(true);}" />
                                                                </dx:ASPxRadioButton>
                                                            </div>
                                                            <div style="float: left; padding-top: 10px;">
                                                                <asp:Label ID="txtCanTelecommuteFromDesc" runat="server" Text="Desde" CssClass="labelForm lblminWidth"></asp:Label>
                                                            </div>

                                                            <div id="divtxtCanTelecommuteFrom" style="float: left; padding-top: 5px;">
                                                                <dx:ASPxDateEdit runat="server" PopupVerticalAlign="BottomSides" PopupHorizontalAlign="OutsideRight" EditFormat="Date" ID="txtCanTelecommuteFrom" Width="150px" AllowNull="false" ClientInstanceName="txtCanTelecommuteFromClient">
                                                                    <ClientSideEvents ValueChanged="function(s,e){hasChanges(true);}" />
                                                                </dx:ASPxDateEdit>
                                                            </div>
                                                            <div style="float: left; padding-top: 10px;">
                                                                <asp:Label ID="txtCanTelecommuteToDesc" runat="server" Text="hasta" CssClass="labelForm lblminWidth"></asp:Label>
                                                            </div>
                                                            <div id="divtxtCanTelecommuteTo" style="float: left; padding-top: 5px; padding-left: 20px;">
                                                                <dx:ASPxDateEdit runat="server" PopupVerticalAlign="BottomSides" PopupHorizontalAlign="OutsideLeft" EditFormat="Date" ID="txtCanTelecommuteTo" Width="150px" AllowNull="false" ClientInstanceName="txtCanTelecommuteToClient">
                                                                    <ClientSideEvents ValueChanged="function(s,e){hasChanges(true);}" />
                                                                </dx:ASPxDateEdit>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <br />
                                                </div>
                                                <br />
                                                <div class="divRow">
                                                    <asp:Label ID="lblTelecommutingDaysTitle" runat="server" Text="¿Cuántos días podrá teletrabajar?" CssClass="labelForm bigLabAgreeText"></asp:Label>
                                                    <div class="componentForm" style="margin-top: -3px; margin-bottom: 5px;">
                                                        <div style="float: left; clear: both; margin-right: 5px;">
                                                            <dx:ASPxLabel ID="txtTCOptionalDays" CssClass="roLabel" runat="server" Text="Podrán escoger" />
                                                        </div>
                                                        <div style="float: left; padding-top: 3px;">
                                                            <dx:ASPxTextBox runat="server" ID="txtTelecommutingMaxOptional" MaxLength="3" Width="50" ClientInstanceName="txtTelecommutingMaxOptional_Client">
                                                                <ClientSideEvents TextChanged="function(s,e){ hasChanges(true)}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
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
                                                            <dx:ASPxComboBox ID="cmbWeekOrMonth" runat="server" ClientInstanceName="cmbWeekOrMonthClient" Width="150px">
                                                                <ClientSideEvents SelectedIndexChanged="function(s,e){ hasChanges(true)}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                            </dx:ASPxComboBox>
                                                        </div>
                                                    </div>
                                                </div>
                                                <br />
                                                <div class="divRow labAgreepadding">
                                                    <div class="" style="padding-bottom: 10px;">
                                                        <asp:Label ID="txtTelecommutingDistributionDesc" runat="server" Text="Los usuarios distribuirán semanalmente su jornada de teletrabajo de la siguiente forma:"></asp:Label>
                                                    </div>
                                                </div>
                                                <div class="divRow labAgreepadding">

                                                    <div id="divTelecommutingPatternGrid" runat="server" class="jsGridContentSmall dextremeGrid">
                                                    </div>
                                                </div>
                                            </div>

                                            <br />
                                            <div id="divHorasExtraGeneral" runat="server">
                                                <div class="panHeader2">
                                                    <span>
                                                        <asp:Label runat="server" ID="lblExtraHours" Text="Horas Extra"></asp:Label></span>
                                                </div>
                                                <br />
                                                <div style="display: flex;">
                                                    <div id="extraHoursConfig" style="display: inline-block; padding-right: 5em;">
                                                        <div class="divRow">
                                                            <div class="divRowDescription">
                                                                <asp:Label ID="lblConfExtraHours" runat="server" Text="Configuración del tipo de horas extra"></asp:Label>
                                                            </div>
                                                            <asp:Label ID="lblEnableExtraHours" runat="server" Text="¿Se permite realizar horas extra?" CssClass="labelForm"></asp:Label>
                                                            <div class="componentForm" style="margin-top: -3px; margin-bottom: 5px;">

                                                                <div style="clear: both; padding-bottom: 10px;">
                                                                    <div style="float: left; padding-right: 20px">
                                                                        <dx:ASPxRadioButton GroupName="Extras" ID="ckExtraDisabled" runat="server" ClientInstanceName="ckExtraDisabled_client" Text="Desactivado">
                                                                            <ClientSideEvents CheckedChanged="function(s,e){ hasChanges(true); }" />
                                                                        </dx:ASPxRadioButton>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>

                                                        <div class="divRow">
                                                            <div class="labelForm">&nbsp</div>

                                                            <div class="componentForm" style="margin-top: -3px; margin-bottom: 5px;">

                                                                <div style="clear: both; padding-bottom: 10px;">
                                                                    <div style="float: left; padding-right: 20px;">
                                                                        <dx:ASPxRadioButton GroupName="Extras" ID="ckExtra3x3" runat="server" ClientInstanceName="ckExtra3x3_client" Text="Modelo 3x3">
                                                                            <ClientSideEvents CheckedChanged="function(s,e){ hasChanges(true); }" />
                                                                        </dx:ASPxRadioButton>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>

                                                        <div class="divRow">
                                                            <div class="labelForm">&nbsp</div>

                                                            <div class="componentForm" style="margin-top: -3px; margin-bottom: 5px;">

                                                                <div style="clear: both; padding-bottom: 10px;">
                                                                    <div style="float: left; padding-right: 20px;">
                                                                        <dx:ASPxRadioButton GroupName="Extras" ID="ckExtra9acc" runat="server" ClientInstanceName="ckExtra9acc_client" Text="Modelo 9 Acumulable">
                                                                            <ClientSideEvents CheckedChanged="function(s,e){ hasChanges(true); }" />
                                                                        </dx:ASPxRadioButton>
                                                                    </div>

                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div id="extraHoursCauses" style="display: flex; flex-direction: column; flex: 1; gap: 10px;">
                                                        <div style="display: none;">
                                                            <dx:ASPxTokenBox ID="tbCauses" ClientInstanceName="tbCausesClient" runat="server" Width="100%">
                                                            </dx:ASPxTokenBox>
                                                        </div>
                                                        <div>
                                                            <div class="divRowDescription" style="padding-left: 4px;">
                                                                <asp:Label ID="lblSelectorCausesExtraHours" runat="server" Text="Justificaciones a computar como horas extra" Style="font-size: 11px;"></asp:Label>
                                                            </div>
                                                            <div class="componentForm">
                                                                <dx:ASPxTokenBox ID="tbExtras" ClientInstanceName="tbExtrasClient" runat="server" EnableClientSideAPI="True" Width="100%">
                                                                    <ClientSideEvents EndCallback="OnInitCauses" TextChanged="function(s,e){ hasChanges(true); updateExtraCauses(s,e); }" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                </dx:ASPxTokenBox>
                                                            </div>
                                                        </div>
                                                        <div style="display: flex; gap: 10px;">
                                                            <div>
                                                                <div class="divRowDescription" style="padding-left: 4px;">
                                                                    <asp:Label ID="lblSelectorDoublesExtraHours" runat="server" Text="Justificación para dobles" Style="font-size: 11px;"></asp:Label>
                                                                </div>
                                                                <div style="padding-left: 3px; padding-top: 3px;" id="divcmbCausesDbl">
                                                                    <dx:ASPxComboBox ID="cmbCausesDbl" runat="server" ClientInstanceName="cmbCausesDblClient" Width="215px">
                                                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ hasChanges(true); updateExtraCauses(s,e); }" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                    </dx:ASPxComboBox>
                                                                </div>
                                                            </div>
                                                            <div>
                                                                <div class="divRowDescription" style="padding-left: 4px;">
                                                                    <asp:Label ID="lblSelectorTriplesExtraHours" runat="server" Text="Justificación para triples" Style="font-size: 11px;"></asp:Label>
                                                                </div>
                                                                <div style="padding-left: 3px; padding-top: 3px;" id="divcmbCausesTpl">
                                                                    <dx:ASPxComboBox ID="cmbCausesTpl" runat="server" ClientInstanceName="cmbCausesTplClient" Width="215px">
                                                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ hasChanges(true); updateExtraCauses(s,e); }" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                    </dx:ASPxComboBox>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <br />
                                            </div>
                                        </div>
                                    </div>

                                    <div id="div01" class="contentPanel" runat="server" name="menuPanel" style="display: none;">
                                        <!-- Este div es el header -->
                                        <div class="panBottomMargin">
                                            <div class="panHeader2 panBottomMargin">
                                                <span class="panelTitleSpan">
                                                    <asp:Label runat="server" ID="lblLabAgreesMainTitleRules" Text="Reglas de convenio"></asp:Label>
                                                </span>
                                            </div>
                                            <!-- La descripción es opcional -->
                                            <div class="panelHeaderContent">
                                                <div class="panelDescriptionImage">
                                                    <img alt="" src="<%=Me.Page.ResolveUrl("~/LabAgree/Images/LabAgreeRules.png")%>" />
                                                </div>
                                                <div class="panelDescriptionText">
                                                    <asp:Label ID="lblLabAgreesMainTitleRulesDescription" runat="server" Text="Datos especificos sobre valores iniciales y reglas de convenios."></asp:Label>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="divRow">
                                            <div class="splitDivLeft">
                                                <div class="jsGrid">
                                                    <asp:Label ID="lblStartupValueCaption" runat="server" CssClass="jsGridTitle" Text="Valores iniciales"></asp:Label>
                                                    <div class="jsgridButton">
                                                        <dx:ASPxButton ID="btnAddNewStartupValue" runat="server" AutoPostBack="False" CausesValidation="False" Text="Añadir" ToolTip="Añadir" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat">
                                                            <Image Url="~/Base/Images/Grid/add.png"></Image>
                                                            <ClientSideEvents Click="AddNewStartupValue" />
                                                        </dx:ASPxButton>
                                                    </div>
                                                </div>
                                                <dx:ASPxGridView ID="GridStartupValues" runat="server" AutoGenerateColumns="False" ClientInstanceName="GridStartupValuesClient" KeyboardSupport="True" Width="100%">
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
                                                    <ClientSideEvents BeginCallback="GridStartupValues_BeginCallback" CustomButtonClick="GridStartupValues_CustomButtonClick" EndCallback="GridStartupValues_EndCallback" RowDblClick="GridStartupValues_OnRowDblClick" FocusedRowChanged="GridStartupValues_FocusedRowChanged" />
                                                    <Settings ShowTitlePanel="false" VerticalScrollBarMode="Auto" UseFixedTableLayout="True" VerticalScrollableHeight="150" />
                                                </dx:ASPxGridView>

                                                <!-- popup EditShiftBreak -->
                                                <roForms:frmEditStartupValue ID="frmEditStartupValue" runat="server" />
                                            </div>
                                            <div class="splitDivRight">
                                                <div class="jsGrid">
                                                    <asp:Label ID="lblLabAgreeRuleCaption" runat="server" CssClass="jsGridTitle" Text="Reglas de conveio"></asp:Label>
                                                    <div class="jsgridButton">
                                                        <dx:ASPxButton ID="btnAddNeLabAgreeRule" runat="server" AutoPostBack="False" CausesValidation="False" Text="Añadir" ToolTip="Añadir" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat">
                                                            <Image Url="~/Base/Images/Grid/add.png"></Image>
                                                            <ClientSideEvents Click="AddNewLabAgreeRule" />
                                                        </dx:ASPxButton>
                                                    </div>
                                                </div>
                                                <dx:ASPxGridView ID="GridLabAgreeRules" runat="server" AutoGenerateColumns="False" ClientInstanceName="GridLabAgreeRulesClient" KeyboardSupport="True" Width="100%">
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
                                                    <ClientSideEvents BeginCallback="GridLabAgreeRules_BeginCallback" CustomButtonClick="GridLabAgreeRules_CustomButtonClick"
                                                        EndCallback="GridLabAgreeRules_EndCallback" RowDblClick="GridLabAgreeRules_OnRowDblClick" FocusedRowChanged="GridLabAgreeRules_FocusedRowChanged" />
                                                    <Settings ShowTitlePanel="false" VerticalScrollBarMode="Auto" UseFixedTableLayout="True" VerticalScrollableHeight="150" />
                                                </dx:ASPxGridView>
                                                <roForms:frmEditLabAgreeRule ID="frmEditLabAgreeRule" runat="server" />
                                            </div>
                                        </div>
                                        <div class="divRow">
                                            <div class="splitDivLeft">
                                                <div class="jsGrid">
                                                    <asp:Label ID="lblCauseLimitValuesCaption" runat="server" CssClass="jsGridTitle" Text="Valores máximos"></asp:Label>
                                                    <div class="jsgridButton">
                                                        <dx:ASPxButton ID="btnAddNewLabAgreeCauseLimit" runat="server" AutoPostBack="False" CausesValidation="False" Text="Añadir" ToolTip="Añadir" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat">
                                                            <Image Url="~/Base/Images/Grid/add.png"></Image>
                                                            <ClientSideEvents Click="AddNewLabAgreeCauseLimit" />
                                                        </dx:ASPxButton>
                                                    </div>
                                                </div>
                                                <dx:ASPxGridView ID="GridLabAgreeCauseLimit" runat="server" AutoGenerateColumns="False" ClientInstanceName="GridLabAgreeCauseLimitClient" KeyboardSupport="True" Width="100%">
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
                                                    <ClientSideEvents BeginCallback="GridLabAgreeCauseLimit_BeginCallback" CustomButtonClick="GridLabAgreeCauseLimit_CustomButtonClick"
                                                        EndCallback="GridLabAgreeCauseLimit_EndCallback" RowDblClick="GridLabAgreeCauseLimit_OnRowDblClick" FocusedRowChanged="GridLabAgreeCauseLimit_FocusedRowChanged" />
                                                    <Settings ShowTitlePanel="false" VerticalScrollBarMode="Auto" UseFixedTableLayout="True" VerticalScrollableHeight="150" />
                                                </dx:ASPxGridView>
                                                <roForms:frmEditCauseLimit ID="frmEditCauseLimit" runat="server" />
                                            </div>
                                            <div class="splitDivRight">
                                                <div class="jsGrid">
                                                    <asp:Label ID="lblRequestValidationsCaption" runat="server" CssClass="jsGridTitle" Text="Validaciones de solicitudes"></asp:Label>
                                                    <div class="jsgridButton">
                                                        <dx:ASPxButton ID="btnAddNewLabAgreeRequestValidation" runat="server" AutoPostBack="False" CausesValidation="False" Text="Añadir" ToolTip="Añadir" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat">
                                                            <Image Url="~/Base/Images/Grid/add.png"></Image>
                                                            <ClientSideEvents Click="AddNewLabAgreeRequestValidation" />
                                                        </dx:ASPxButton>
                                                    </div>
                                                </div>
                                                <dx:ASPxGridView ID="GridLabAgreeRequestValidation" runat="server" AutoGenerateColumns="False" ClientInstanceName="GridLabAgreeRequestValidationClient" KeyboardSupport="True" Width="100%">
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
                                                    <ClientSideEvents BeginCallback="GridLabAgreeRequestValidation_BeginCallback" CustomButtonClick="GridLabAgreeRequestValidation_CustomButtonClick"
                                                        EndCallback="GridLabAgreeRequestValidation_EndCallback" RowDblClick="GridLabAgreeRequestValidation_OnRowDblClick"
                                                        FocusedRowChanged="GridLabAgreeRequestValidation_FocusedRowChanged" ColumnSorting="function(s,e){ setTimeout(function(){GridLabAgreeRequestValidationClient.PerformCallback('REFRESH');},200); }" />
                                                    <Settings ShowTitlePanel="false" VerticalScrollBarMode="Auto" UseFixedTableLayout="True" VerticalScrollableHeight="150" />
                                                </dx:ASPxGridView>

                                                <roForms:frmEditRequestValidation ID="frmEditRequestValidation" runat="server" />
                                            </div>
                                        </div>
                                    </div>

                                    <div id="div02" class="contentPanel" runat="server" name="menuPanel" style="display: none">
                                        <!-- Este div es el header -->
                                        <div class="panBottomMargin">
                                            <div class="panHeader2 panBottomMargin">
                                                <span class="panelTitleSpan">
                                                    <asp:Label runat="server" ID="LblScheduleRules" Text="Reglas de planificación"></asp:Label>
                                                </span>
                                            </div>
                                            <!-- La descripción es opcional -->
                                            <div class="panelHeaderContent">
                                                <div class="panelDescriptionImage">
                                                    <img alt="" src="<%=Me.Page.ResolveUrl("~/LabAgree/Images/LabAgreeScheduleRules.png")%>" />
                                                </div>
                                                <div class="panelDescriptionText">
                                                    <asp:Label ID="LblScheduleRulesDesc" runat="server" Text="Datos especificos sobre las reglas de planificación que aplican al convenio."></asp:Label>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="divRow" style="padding-left: 15px !important; margin-left: 0px !important">
                                            <div class="jsGrid">
                                                <asp:Label ID="lblLabAgreeScheduleRule" runat="server" CssClass="jsGridTitle" Text="Reglas de planificación"></asp:Label>
                                                <div class="jsgridButton">
                                                    <dx:ASPxButton ID="btnAddNewLabAgreeScheduleRules" runat="server" AutoPostBack="False" CausesValidation="False" Text="Añadir" ToolTip="Añadir" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat">
                                                        <Image Url="~/Base/Images/Grid/add.png"></Image>
                                                        <ClientSideEvents Click="AddNewLabAgreeScheduleRule" />
                                                    </dx:ASPxButton>
                                                </div>
                                            </div>
                                            <dx:ASPxGridView ID="GridLabAgreeScheduleRules" runat="server" AutoGenerateColumns="False" ClientInstanceName="GridLabAgreeScheduleRulesClient" KeyboardSupport="True" Width="100%">
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
                                                <ClientSideEvents BeginCallback="GridLabAgreeScheduleRules_BeginCallback" CustomButtonClick="GridLabAgreeScheduleRules_CustomButtonClick"
                                                    EndCallback="GridLabAgreeScheduleRules_EndCallback" RowDblClick="GridLabAgreeScheduleRules_OnRowDblClick" FocusedRowChanged="GridLabAgreeScheduleRules_FocusedRowChanged" />
                                                <Settings ShowTitlePanel="false" VerticalScrollBarMode="Auto" UseFixedTableLayout="True" VerticalScrollableHeight="600" />
                                            </dx:ASPxGridView>
                                            <roForms:frmEditScheduleRules ID="frmEditScheduleRules" Mode="LabAgree" runat="server" />
                                        </div>
                                    </div>
                                </div>
                <div id="divMsgBottom" class="divMsg2 divMessageBottom" style="display: none">
                    <div class="divImageMsg">
                        <img alt="" id="Img2" src="~/Base/Images/MessageFrame/Alert16.png" runat="server" />
                    </div>
                    <div class="messageText">
                        <span id="msgBottom"></span>
                    </div>
                    <div align="right" class="messageActions">
                        <a href="javascript: void(0);" class="aMsg" onclick="saveChanges();"><span id="lblSaveChangesBottom" runat="server" /></a>
                        &nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;
                                        <a href="javascript: void(0);" onclick="undoChanges();" class="aMsg"><span id="lblUndoChangesBottom" runat="server" /></a>
                    </div>
                </div>
                </dx:PanelContent>
                        </PanelCollection>
                    </dx:ASPxCallbackPanel>
            </div>
        </div>
    </div>
    </div>

    <!-- POPUP NEW OBJECT -->
    <dx:ASPxPopupControl ID="NewObjectPopup" runat="server" AllowDragging="False" CloseAction="None" Modal="True" ContentUrl="~/Base/Popups/CreateObjectPopup.aspx"
        PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter" Width="470px" Height="300px"
        ShowHeader="False" ScrollBars="Auto" ShowPageScrollbarWhenModal="True" ClientInstanceName="NewObjectPopup_Client" PopupAnimationType="None" BackColor="Transparent" ContentStyle-Paddings-Padding="0px" Border-BorderColor="Transparent" ShowShadow="false">
        <SettingsLoadingPanel Enabled="false" />
    </dx:ASPxPopupControl>

    <!-- POPUP NEW OBJECT -->
    <dx:ASPxPopupControl ID="CaptchaObjectPopup" runat="server" AllowDragging="False" CloseAction="None" Modal="True" ContentUrl="~/Base/Popups/GenericCaptchaValidator.aspx"
        PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter" ModalBackgroundStyle-Opacity="0" Width="500" Height="320"
        ShowHeader="False" ScrollBars="Auto" ShowPageScrollbarWhenModal="True" ClientInstanceName="CaptchaObjectPopup_Client" PopupAnimationType="None" BackColor="Transparent" ContentStyle-Paddings-Padding="0px" Border-BorderColor="Transparent" ShowShadow="false">
        <SettingsLoadingPanel Enabled="false" />
    </dx:ASPxPopupControl>

    <script language="javascript" type="text/javascript">

        function resizeTreeLabAgrees() {
            try {
                var ctlPrefix = "<%= roTreesLabAgrees.ClientID %>";
                eval(ctlPrefix + "_resizeTrees();");
            }
            catch (e) {
                showError("resizeTreeLabAgrees", e);
            }
        }

        function resizeFrames() {
            var divMainBodyHeight = $("#divMainBody").outerHeight(true);
            var divHeight = 0;
            if (divMainBodyHeight < 525) {
                divHeight = 525 - $("#divTabInfo").outerHeight(true);
            }
            else {
                divHeight = divMainBodyHeight - $("#divTabInfo").outerHeight();
            }

            $("#divTabData").height(divHeight - 10);

            var divTreeHeight = $("#divTree").height();
            $("#ctlTreeDiv").height(divTreeHeight);
        }

        window.onresize = function () {
            resizeFrames();
            resizeTreeLabAgrees();
        }

        if (document.getElementById('<%= noRegs.ClientID %>').value != '') {
            cargaLabAgrees(-1);
        }
    </script>
</asp:Content>
