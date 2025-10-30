<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.WebUserControls_QueryEmployeeScheduleRules" CodeBehind="QueryEmployeeScheduleRules.ascx.vb" %>

<%@ Register Src="~/LabAgree/WebUserForms/frmEditScheduleRules.ascx" TagName="frmEditScheduleRules" TagPrefix="roForms" %>

<div id="<%= Me.ClientID %>_frm" style="position: fixed; z-index: 9010; display: none; top: 50%; left: 50%; width: 750px;">
    <div id="<%= Me.ClientID %>_BgS" style="position: absolute; top: 0; left: 0; display: none; z-index: 9009;"></div>
    <div class="bodyPopupExtended">
        <div id="divContentPanels" style="padding-right: 20px">
            <div id="div00" class="contentPanel" runat="server" name="menuPanel">
                <!-- Este div es el header -->
                <div class="panBottomMargin">
                    <div class="panHeader2 panBottomMargin">
                        <span class="panelTitleSpan">
                            <asp:Label runat="server" ID="lblGeneralTitle" Text="Parámetros configurables"></asp:Label>
                        </span>
                    </div>
                    <!-- La descripción es opcional -->
                    <div style="text-align: left; padding-left: 15px;">
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
                            <div style="float: left">
                                <dx:ASPxTextBox runat="server" ID="txtYearHours" MaxLength="12" Width="75" ClientInstanceName="txtYearHoursClient" ReadOnly="true">
                                    <MaskSettings Mask="<0..999999>" />
                                </dx:ASPxTextBox>
                            </div>
                            <div id="divlblFork" style="float: left; padding-right: 10px; line-height: 20px; color: #2D4155;">
                                <asp:Label ID="lblFork" runat="server" Text="Horquilla:"></asp:Label>
                            </div>
                            <div id="divtxtFork" style="float: left; padding-right: 10px">
                                <dx:ASPxTextBox runat="server" ID="txtFork" MaxLength="12" Width="75" ClientInstanceName="txtForkClient">
                                    <ClientSideEvents TextChanged="function(s,e){ hasChanges(true)}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                    <ValidationSettings ErrorDisplayMode="None">
                                    </ValidationSettings>
                                    <MaskSettings Mask="<0..999999>" />
                                </dx:ASPxTextBox>
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
                                <dx:ASPxTextBox runat="server" ID="txtYearHolidays" MaxLength="12" Width="75" ClientInstanceName="txtYearHolidaysClient" ReadOnly="true">
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
                            <div style="float: left">
                                <dx:ASPxTokenBox ID="tbWorkingDays" runat="server" Width="100%" ClientInstanceName="tbWorkingDaysClient" ReadOnly="true">
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
                            <div style="float: left">
                                <dx:ASPxCheckBox type="checkbox" runat="server" ID="chkCanWorkOnFeastDays" Text="" ClientInstanceName="chkCanWorkOnFeastDaysClient" ReadOnly="true">
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
                            <div style="float: left">
                                <dx:ASPxCheckBox type="checkbox" runat="server" ID="chkCanWorkOnNonWorkingDays" Text="" ClientInstanceName="chkCanWorkOnNonWorkingDaysClient" ReadOnly="true">
                                </dx:ASPxCheckBox>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="divRow">
                    <div class="jsGrid">
                        <asp:Label ID="lblLabAgreeScheduleRule" runat="server" CssClass="jsGridTitle" Text="Reglas de planificación"></asp:Label>
                    </div>
                    <dx:ASPxGridView ID="QueryEmployeeRules" runat="server" AutoGenerateColumns="False" ClientInstanceName="QueryEmployeeRulesClient" KeyboardSupport="True" Width="100%">
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
                        <ClientSideEvents BeginCallback="QueryEmployeeRules_BeginCallback" CustomButtonClick="QueryEmployeeRules_CustomButtonClick"
                            EndCallback="QueryEmployeeRules_EndCallback" RowDblClick="QueryEmployeeRules_OnRowDblClick" FocusedRowChanged="QueryEmployeeRules_FocusedRowChanged" />
                        <Settings ShowTitlePanel="false" VerticalScrollBarMode="Auto" UseFixedTableLayout="True" VerticalScrollableHeight="150" />
                    </dx:ASPxGridView>
                </div>
                <roForms:frmEditScheduleRules ID="frmEditScheduleRules" Mode="Query" runat="server" />
            </div>
            <div style="width: 100%;">
                <table border="0" style="width: 100%;">
                    <tr>
                        <td>&nbsp;</td>
                        <td style="width: 110px;" align="right"></td>
                        <td style="width: 110px; padding-top: 25px;" align="left">
                            <dx:ASPxButton ID="btnCancel" runat="server" AutoPostBack="False" CausesValidation="False" Text="Cancelar" ToolTip="Cancelar" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                <ClientSideEvents Click="function(s,e){ closeActualIdContractScheduleRules(); }" />
                            </dx:ASPxButton>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
</div>