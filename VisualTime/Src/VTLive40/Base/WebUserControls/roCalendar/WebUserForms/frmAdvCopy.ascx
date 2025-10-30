<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.WebUserForms_frmAdvCopy" CodeBehind="frmAdvCopy.ascx.vb" %>

<script type="text/javascript">

    function <%= Me.ClientID %>_showRepeatPopup() {
        $("#<%= Me.ClientID %>_repeat_frm").dialog({
            autoOpen: false,
            height: 'auto',
            width: '550px',
            modal: true,
            resizable: false,
            draggable: false,
            buttons: [{
                text: "Accept",
                "class": 'btnFlat btnFlatBlack',
                click: function () {
                    var tmpObj = eval("<%= Me.ClientID %>_rbAdvancedRepeatEnabledClient");
                    tmpObj.SetChecked(true);
                    $("#<%= Me.ClientID %>_repeat_frm").dialog("close");
                },

            }, {
                    text: "Cancelar",
                    "class": 'btnFlat btnFlatBlack',
                    click: function () {
                        $("#<%= Me.ClientID %>_repeat_frm").dialog("close");
                    },
                }],
            close: function () {

            }
        }).dialog('open');
    }
    function <%= Me.ClientID %>_showBloquedPopup() {
        $("#<%= Me.ClientID %>_bloqued_frm").dialog({
            autoOpen: false,
            height: 'auto',
            width: '550px',
            modal: true,
            resizable: false,
            draggable: false,
            buttons: [{
                text: "Accept",
                "class": 'btnFlat btnFlatBlack',
                click: function () {
                    var tmpObj = eval("<%= Me.ClientID %>_rbAdvancedBloquedEnabledClient");
                    tmpObj.SetChecked(true);
                    $("#<%= Me.ClientID %>_bloqued_frm").dialog("close");
                },

            }, {
                    text: "Cancelar",
                    "class": 'btnFlat btnFlatBlack',
                    click: function () {
                        $("#<%= Me.ClientID %>_bloqued_frm").dialog("close");
                    },
                }],
            close: function () {

            }
        }).dialog('open');
    }
    function <%= Me.ClientID %>_showHolidaysPopup() {
        $("#<%= Me.ClientID %>_holidays_frm").dialog({
            autoOpen: false,
            height: 'auto',
            width: '550px',
            modal: true,
            resizable: false,
            draggable: false,
            buttons: [{
                text: "Accept",
                "class": 'btnFlat btnFlatBlack',
                click: function () {
                    var tmpObj = eval("<%= Me.ClientID %>_rbAdvancedHolidaysEnabledClient");
                    tmpObj.SetChecked(true);
                    $("#<%= Me.ClientID %>_holidays_frm").dialog("close");
                },

            }, {
                    text: "Cancelar",
                    "class": 'btnFlat btnFlatBlack',
                    click: function () {
                        $("#<%= Me.ClientID %>_holidays_frm").dialog("close");
                    },
                }],
            close: function () {

            }
        }).dialog('open');
    }
</script>

<div id="<%= Me.ClientID %>_frm" class="ui-dialog-content">
    <form id="<%= Me.ClientID %>_attr">
        <table style="width: 100%" cellspacing="0" class="bodyPopup">
            <tr style="height: 20px;">
                <td>
                    <div class="panHeader2">
                        <span style="">
                            <asp:Label ID="lblTitle" runat="server" Text="Pegado Especial" />
                        </span>
                    </div>
                </td>
            </tr>
            <tr>
                <td style="padding: 15px; padding-bottom: 0px;">
                    <asp:Label ID="lblEspecialPasteInfo" runat="server" Text="Este formulario le permite asignar los horarios que tenga copiados en el portapapeles a un empleado entre las fechas que especifique." CssClass="editTextFormat" />
                </td>
            </tr>
            <tr>
                <td style="padding: 15px">
                    <div style="width: 100%">
                        <div class="panBottomMargin">
                            <div class="panHeader2 panBottomMargin">
                                <span class="panelTitleSpan">
                                    <asp:Label runat="server" ID="lblActionResume" Text="Resumen de acciones"></asp:Label>
                                </span>
                            </div>
                        </div>
                        <div class="panBottomMargin">
                            <div class="divRow dialogDivRow">
                                <div style="float: left">
                                    <asp:Label ID="lblCopy" runat="server" Text="Copiar" CssClass="editTextFormat" />
                                </div>
                                <div style="float: left; padding-left: 5px; color: red">
                                    <span id="<%= Me.ClientID %>_shiftCount">6</span>
                                </div>
                                <div style="float: left; padding-left: 5px">
                                    <asp:Label ID="lblShiftsDesc" runat="server" Text="horarios empezando el" CssClass="editTextFormat" />
                                </div>
                                <div style="float: left; padding-left: 5px; color: red">
                                    <span id="<%= Me.ClientID %>_shiftStartDate">05/05/2016</span>
                                </div>
                            </div>
                            <div class="divRow" style="margin-left: 50px; width: calc(100% - 50px);">
                                <roUserControls:roGroupBox ID="RoGroupBox3" runat="server">
                                    <Content>
                                        <div class="divRow dialogDivRow">
                                            <div style="float: left">
                                                <dx:ASPxRadioButton ID="rbRepeatTimes" runat="server" Checked="true" GroupName="rbRepeatGroup" Text="" />
                                            </div>
                                            <div style="float: left; padding-left: 5px; padding-top: 4px">
                                                <asp:Label ID="lblRepeat" runat="server" Text="Repetir" CssClass="editTextFormat" />
                                            </div>
                                            <div style="float: left; padding-left: 5px;">
                                                <dx:ASPxSpinEdit ID="txtRepeatTimes" runat="server" Number="1" MinValue="1" MaxValue="52" Width="75px">
                                                    <SpinButtons ShowIncrementButtons="True" ShowLargeIncrementButtons="False" />
                                                </dx:ASPxSpinEdit>
                                            </div>
                                            <div style="float: left; padding-left: 5px; padding-top: 4px">
                                                <asp:Label ID="lblTimes" runat="server" Text=" veces" CssClass="editTextFormat" />
                                            </div>
                                        </div>

                                        <div class="divRow dialogDivRow">
                                            <div style="float: left">
                                                <dx:ASPxRadioButton ID="rbRepeatUntil" runat="server" Checked="false" GroupName="rbRepeatGroup" Text="" />
                                            </div>
                                            <div style="float: left; padding-left: 5px; padding-top: 4px">
                                                <asp:Label ID="lblRepeatTo" runat="server" Text="Repetir hasta el" CssClass="editTextFormat" />
                                            </div>
                                            <div style="float: left; padding-left: 5px;">
                                                <dx:ASPxDateEdit ID="txtToDate" AllowNull="false" runat="server" Width="150px">
                                                    <ValidationSettings ErrorDisplayMode="None" />
                                                </dx:ASPxDateEdit>
                                            </div>
                                            <div style="float: left; padding-left: 5px; padding-top: 4px">
                                                <asp:Label ID="lblAddIn" runat="server" Text=" inclusive" CssClass="editTextFormat" />
                                            </div>
                                        </div>

                                        <div class="divRow dialogDivRow" id="divBlockDestinationDays" runat="server">
                                            <div style="float: left">
                                                <dx:ASPxCheckBox ID="ckBloqDestDays" runat="server" Checked="false" Text="" />
                                            </div>
                                            <div style="float: left; padding-left: 5px; padding-top: 4px">
                                                <asp:Label ID="lblBloqDestDays" runat="server" Text="Bloquear días una vez finalizada la planificación" CssClass="editTextFormat" />
                                            </div>
                                        </div>
                                    </Content>
                                </roUserControls:roGroupBox>
                            </div>
                        </div>
                    </div>

                    <div style="width: 100%" id="divTelecommuteOptions" runat="server">
                        <div class="panBottomMargin">
                            <div class="divRow dialogDivRow">
                                <div style="float: left">
                                    <asp:Label ID="lblCopyOptions" runat="server" Text="Opciones de copia" CssClass="editTextFormat" />
                                </div>
                            </div>
                            <div class="divRow" style="margin-left: 50px; width: calc(100% - 50px);">
                                <roUserControls:roGroupBox ID="RoGroupBox7" runat="server">
                                    <Content>
                                        <div class="divRow dialogDivRow">
                                            <div style="float: left">
                                                <dx:ASPxRadioButton ID="rbTelecommuteKeep" runat="server" Checked="true" GroupName="rbTelecommute" Text="">
                                                    <ClientSideEvents CheckedChanged="ckTelecommuteVisibilityChanged" />
                                                </dx:ASPxRadioButton>
                                            </div>
                                            <div style="float: left; padding-left: 5px; padding-top: 4px">
                                                <asp:Label ID="lblCopyOnlySchedule" runat="server" Text="Copiar sólo planificación horaria" CssClass="editTextFormat" />
                                            </div>
                                        </div>

                                        <div class="divRow dialogDivRow" style="display: none">
                                            <div style="float: left">
                                                <dx:ASPxRadioButton ID="rbTelecommuteCopy" runat="server" Checked="true" GroupName="rbTelecommute" Text="">
                                                    <ClientSideEvents CheckedChanged="ckTelecommuteVisibilityChanged" />
                                                </dx:ASPxRadioButton>
                                            </div>
                                            <div style="float: left; padding-left: 5px; padding-top: 4px">
                                                <asp:Label ID="lblCopyScheduleAndTelecommute" runat="server" Text="Copiar planificación horaria y de teletrabajo" CssClass="editTextFormat" />
                                            </div>
                                        </div>
                                        <div class="divRow dialogDivRow">
                                            <div style="float: left">
                                                <dx:ASPxRadioButton ID="rbTelecommuteDefault" runat="server" Checked="true" GroupName="rbTelecommute" Text="">
                                                    <ClientSideEvents CheckedChanged="ckTelecommuteVisibilityChanged" />
                                                </dx:ASPxRadioButton>
                                            </div>
                                            <div style="float: left; padding-left: 5px; padding-top: 4px">
                                                <asp:Label ID="lblCopyOnlyTelecommute" runat="server" Text="Copiar sólo planificación de teletrabajo" CssClass="editTextFormat" />
                                            </div>
                                        </div>
                                    </Content>
                                </roUserControls:roGroupBox>
                            </div>
                        </div>
                    </div>

                    <div style="width: 100%" id="divRepeatOptions" runat="server">
                        <div class="panBottomMargin">
                            <div class="divRow dialogDivRow">
                                <div style="float: left">
                                    <asp:Label ID="lblAdvancedRules" runat="server" Text="Opciones de repetición" CssClass="editTextFormat" />
                                </div>
                            </div>
                            <div class="divRow" style="margin-left: 50px; width: calc(100% - 50px);">
                                <roUserControls:roGroupBox ID="RoGroupBox4" runat="server">
                                    <Content>
                                        <div class="divRow dialogDivRow">
                                            <div style="float: left">
                                                <dx:ASPxRadioButton ID="rbAdvancedRepeatDisabled" runat="server" Checked="true" GroupName="rbAdvancedRepeat" Text="" />
                                            </div>
                                            <div style="float: left; padding-left: 5px; padding-top: 4px">
                                                <asp:Label ID="lblAdvancedRepeatDisabled" runat="server" Text="Cada repetición empezará justo al terminar la anterior." CssClass="editTextFormat" />
                                            </div>
                                        </div>

                                        <div class="divRow dialogDivRow">
                                            <div style="float: left">
                                                <dx:ASPxRadioButton ID="rbAdvancedRepeatEnabled" runat="server" Checked="true" GroupName="rbAdvancedRepeat" Text="" />
                                            </div>
                                            <div style="float: left; padding-left: 5px; padding-top: 4px">
                                                <asp:Label ID="lblAdvancedRepeatEnabled" runat="server" Text="Usar reglas de repetición personalizadas" CssClass="editTextFormat" />
                                            </div>
                                            <div style="float: left; padding-left: 5px; padding-top: 4px">
                                                <asp:Label ID="Label1" runat="server" Style="cursor: pointer" Text="[...]" CssClass="editTextFormat" />
                                                <%--<dx:ASPxButton ID="btnOpenPopupRepeat" runat="server" AutoPostBack="False" CausesValidation="False" Text="..." ToolTip="" Width="40px" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat">
                                                    <ClientSideEvents Click="function(s,e){ showRepeatPopup(); }" />
                                                </dx:ASPxButton>--%>
                                            </div>
                                        </div>
                                    </Content>
                                </roUserControls:roGroupBox>
                            </div>
                        </div>
                    </div>

                    <div style="width: 100%" id="divBloquedDaysOptions" runat="server">
                        <div class="panBottomMargin">
                            <div class="divRow dialogDivRow">
                                <div style="float: left">
                                    <asp:Label ID="lblAdvancedBloqued" runat="server" Text="Opciones de días bloqueados" CssClass="editTextFormat" />
                                </div>
                            </div>
                            <div class="divRow" style="margin-left: 50px; width: calc(100% - 50px);">
                                <roUserControls:roGroupBox ID="RoGroupBox5" runat="server">
                                    <Content>
                                        <div class="divRow dialogDivRow">
                                            <div style="float: left">
                                                <dx:ASPxRadioButton ID="rbAdvancedBloquedDisabled" runat="server" Checked="true" GroupName="rbAdvancedBloqued" Text="" />
                                            </div>
                                            <div style="float: left; padding-left: 5px; padding-top: 4px">
                                                <asp:Label ID="lblAdvancedBloquedDisabled" runat="server" Text="Ignorar días bloqueados y proseguir con el siguiente horario a copiar" CssClass="editTextFormat" />
                                            </div>
                                        </div>

                                        <div class="divRow dialogDivRow">
                                            <div style="float: left">
                                                <dx:ASPxRadioButton ID="rbAdvancedBloquedEnabled" runat="server" Checked="true" GroupName="rbAdvancedBloqued" Text="" />
                                            </div>
                                            <div style="float: left; padding-left: 5px; padding-top: 4px">
                                                <asp:Label ID="lblAdvancedBloquedEnabled" runat="server" Text="Realizar un tratamiento personalizado" CssClass="editTextFormat" />
                                            </div>
                                            <div style="float: left; padding-left: 5px; padding-top: 4px">
                                                <asp:Label ID="Label4" runat="server" Style="cursor: pointer" Text="[...]" CssClass="editTextFormat" />
                                            </div>
                                        </div>
                                    </Content>
                                </roUserControls:roGroupBox>
                            </div>
                        </div>
                    </div>

                    <div style="width: 100%" id="divHolidaysOptions" runat="server">
                        <div class="panBottomMargin">
                            <div class="divRow dialogDivRow">
                                <div style="float: left">
                                    <asp:Label ID="lblAdvancedHolidays" runat="server" Text="Opciones de días de vacaciones" CssClass="editTextFormat" />
                                </div>
                            </div>
                            <div class="divRow" style="margin-left: 50px; width: calc(100% - 50px);">
                                <roUserControls:roGroupBox ID="RoGroupBox6" runat="server">
                                    <Content>
                                        <div class="divRow dialogDivRow">
                                            <div style="float: left">
                                                <dx:ASPxRadioButton ID="rbAdvancedHolidaysDisabled" runat="server" Checked="true" GroupName="rbAdvancedHolidays" Text="" />
                                            </div>
                                            <div style="float: left; padding-left: 5px; padding-top: 4px">
                                                <asp:Label ID="lblAdvancedHolidaysDisabled" runat="server" Text="Ignorar días de vacaciones y proseguir con el siguiente horario a copiar" CssClass="editTextFormat" />
                                            </div>
                                        </div>

                                        <div class="divRow dialogDivRow">
                                            <div style="float: left">
                                                <dx:ASPxRadioButton ID="rbAdvancedHolidaysEnabled" runat="server" Checked="true" GroupName="rbAdvancedHolidays" Text="" />
                                            </div>
                                            <div style="float: left; padding-left: 5px; padding-top: 4px">
                                                <asp:Label ID="lblAdvancedHolidaysEnabled" runat="server" Text="Realizar un tratamiento personalizado" CssClass="editTextFormat" />
                                            </div>
                                            <div style="float: left; padding-left: 5px; padding-top: 4px">
                                                <asp:Label ID="Label8" runat="server" Style="cursor: pointer" Text="[...]" CssClass="editTextFormat" />
                                            </div>
                                        </div>
                                    </Content>
                                </roUserControls:roGroupBox>
                            </div>
                        </div>
                    </div>

                    <div style="width: 100%" id="divBudgetWarning" runat="server">
                        <div class="panBottomMargin">
                            <div class="divRow dialogDivRow">
                                <div style="float: left">
                                    <asp:Label ID="lblBudgetWarning" runat="server" Text="Si el empleado no tiene contrato o está asignado a otro nodo se ignorará y se proseguirá con el siguiente empleado" CssClass="editTextFormat" />
                                </div>
                            </div>
                        </div>
                    </div>
                </td>
            </tr>
        </table>
    </form>
</div>

<div style="display: none">

    <div id="<%= Me.ClientID %>_repeat_frm" class="ui-dialog-content">
        <form id="<%= Me.ClientID %>_repeat_attr">
            <table style="width: 100%" cellspacing="0" class="bodyPopup">
                <tr style="height: 20px;">
                    <td>
                        <div class="panHeader2">
                            <span style="">
                                <asp:Label runat="server" ID="lblAdvRepeat" Text="Repeticiones personalizadas"></asp:Label>
                            </span>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td style="padding: 15px">
                        <div style="width: 100%">
                            <div class="panBottomMargin">
                                <div class="divRow dialogDivRow">
                                    <asp:Label ID="lblRepeatStart" runat="server" Text="Cada repetición empieza:" CssClass="editTextFormat" />
                                </div>
                                <div class="divRow" style="margin-left: 50px; width: calc(100% - 50px);">
                                    <roUserControls:roGroupBox ID="GroupBox1" runat="server">
                                        <Content>
                                            <div class="divRow dialogDivRow">
                                                <div style="float: left">
                                                    <dx:ASPxRadioButton ID="rbStartsInmediately" runat="server" Checked="true" GroupName="rbRepeatStart" Text="" />
                                                </div>
                                                <div style="float: left; padding-left: 5px; padding-top: 4px;">
                                                    <asp:Label ID="lblStartsInmediately" runat="server" Text="Cada repetición empezará justo al terminar la anterior." CssClass="editTextFormat" />
                                                </div>
                                            </div>
                                            <div class="divRow dialogDivRow">
                                                <div style="float: left">
                                                    <dx:ASPxRadioButton ID="rbStartsNextDay" runat="server" Checked="false" GroupName="rbRepeatStart" Text="" />
                                                </div>
                                                <div style="float: left; padding-left: 5px; padding-top: 4px">
                                                    <asp:Label ID="Label2" runat="server" Text="Día de la semana" CssClass="editTextFormat" />
                                                </div>
                                                <div style="float: left; padding-left: 5px;">
                                                    <dx:ASPxComboBox ID="cmbRepeatStartsDay" runat="server" Width="150px" DropDownRows="3">
                                                    </dx:ASPxComboBox>
                                                </div>
                                            </div>
                                            <div class="divRow dialogDivRow">
                                                <div style="float: left">
                                                    <dx:ASPxRadioButton ID="rbStartsNextMonth" runat="server" Checked="false" GroupName="rbRepeatStart" Text="" />
                                                </div>
                                                <div style="float: left; padding-left: 5px; padding-top: 4px">
                                                    <asp:Label ID="Label3" runat="server" Text="Día del mes" CssClass="editTextFormat" />
                                                </div>
                                                <div style="float: left; padding-left: 5px;">
                                                    <dx:ASPxSpinEdit ID="txtRepeatStartsMonth" runat="server" Number="1" MinValue="1" MaxValue="31" Width="75px">
                                                        <SpinButtons ShowIncrementButtons="True" ShowLargeIncrementButtons="False" />
                                                    </dx:ASPxSpinEdit>
                                                </div>
                                            </div>
                                        </Content>
                                    </roUserControls:roGroupBox>
                                </div>
                            </div>
                            <div class="panBottomMargin">
                                <div class="divRow dialogDivRow">
                                    <asp:Label ID="lblRepeatOptions" runat="server" Text="Otras opciones:" CssClass="editTextFormat" />
                                </div>
                                <div class="divRow" style="margin-left: 50px; width: calc(100% - 50px);">
                                    <roUserControls:roGroupBox ID="GroupBox2" runat="server">
                                        <Content>

                                            <div class="divRow dialogDivRow" style="margin-left: 0px; margin-bottom: 8px;">
                                                <div style="float: left">
                                                    <dx:ASPxCheckBox ID="ckSkipOptions" runat="server" Checked="false" Text="" />
                                                </div>
                                                <div style="float: left; padding-left: 5px; padding-top: 4px">
                                                    <asp:Label ID="lblSkipOptionsEach" runat="server" Text="Cada" CssClass="editTextFormat" />
                                                </div>
                                                <div style="float: left; padding-left: 5px">
                                                    <dx:ASPxSpinEdit ID="txtSkipOptions" runat="server" Number="1" MinValue="1" MaxValue="31" Width="75px">
                                                        <SpinButtons ShowIncrementButtons="True" ShowLargeIncrementButtons="False" />
                                                    </dx:ASPxSpinEdit>
                                                </div>
                                                <div style="float: left; padding-left: 5px; padding-top: 4px">
                                                    <asp:Label ID="lblSkipOptionsDesc" runat="server" Text="repeticiones saltar:" CssClass="editTextFormat" />
                                                </div>
                                            </div>
                                            <div class="divRow dialogDivRow">
                                                <div style="float: left">
                                                    <dx:ASPxRadioButton ID="rbRepeatSkipWeek" runat="server" Checked="false" GroupName="rbRepeatSkip" Text="" />
                                                </div>
                                                <div style="float: left; padding-left: 5px; padding-top: 4px">
                                                    <asp:Label ID="LblSkipToWeek" runat="server" Text="Hasta el siguiente día de la semana" CssClass="editTextFormat" />
                                                </div>
                                                <div style="float: left; padding-left: 5px;">
                                                    <dx:ASPxComboBox ID="cmbSkipsWeekDay" runat="server" Width="150px" DropDownRows="3">
                                                    </dx:ASPxComboBox>
                                                </div>
                                            </div>
                                            <div class="divRow dialogDivRow">
                                                <div style="float: left">
                                                    <dx:ASPxRadioButton ID="rbRepeatSkipMonth" runat="server" Checked="false" GroupName="rbRepeatSkip" Text="" />
                                                </div>
                                                <div style="float: left; padding-left: 5px; padding-top: 4px">
                                                    <asp:Label ID="LblSkipToMonth" runat="server" Text="hasta el siguiente día del mes" CssClass="editTextFormat" />
                                                </div>
                                                <div style="float: left; padding-left: 5px;">
                                                    <dx:ASPxSpinEdit ID="txtSkipMonthDayValue" runat="server" Number="1" MinValue="1" MaxValue="31" Width="75px">
                                                        <SpinButtons ShowIncrementButtons="True" ShowLargeIncrementButtons="False" />
                                                    </dx:ASPxSpinEdit>
                                                </div>
                                            </div>
                                            <div class="divRow dialogDivRow">
                                                <div style="float: left">
                                                    <dx:ASPxRadioButton ID="rbRepeatSkipDays" runat="server" Checked="false" GroupName="rbRepeatSkip" Text="" />
                                                </div>
                                                <div style="float: left; padding-left: 5px;">
                                                    <dx:ASPxSpinEdit ID="txtRepeatSkipDays" runat="server" Number="1" MinValue="1" MaxValue="7" Width="75px">
                                                        <SpinButtons ShowIncrementButtons="True" ShowLargeIncrementButtons="False" />
                                                    </dx:ASPxSpinEdit>
                                                </div>
                                                <div style="float: left; padding-left: 5px; padding-top: 4px">
                                                    <asp:Label ID="lblRepeatSkipDays" runat="server" Text="días" CssClass="editTextFormat" />
                                                </div>
                                            </div>
                                        </Content>
                                    </roUserControls:roGroupBox>
                                </div>
                            </div>
                        </div>
                    </td>
                </tr>
            </table>
        </form>
    </div>

    <div id="<%= Me.ClientID %>_holidays_frm" class="ui-dialog-content">
        <form id="<%= Me.ClientID %>_holidays_attr">
            <table style="width: 100%" cellspacing="0" class="bodyPopup">
                <tr style="height: 20px;">
                    <td>
                        <div class="panHeader2">
                            <span style="">
                                <asp:Label runat="server" ID="lblAdvHolidays" Text="Cuando se encuentra un día de vacaciones..."></asp:Label>
                            </span>
                        </div>
                    </td>
                </tr>

                <tr>
                    <td style="padding: 15px">
                        <div class="panBottomMargin">
                            <div class="divRow">
                                <roUserControls:roGroupBox ID="RoGroupBox2" runat="server">
                                    <Content>
                                        <div>
                                            <div class="divRow dialogDivRow">
                                                <div style="float: left">
                                                    <dx:ASPxRadioButton ID="rbHolidaySkip" runat="server" Checked="true" GroupName="rbHolidayDays" Text="" />
                                                </div>
                                                <div style="float: left; padding-left: 5px; padding-top: 4px;">
                                                    <asp:Label ID="lblHolidaySkip" runat="server" Text="Saltar el día y el horario que íbamos a copiar y proseguir" CssClass="editTextFormat" />
                                                </div>
                                            </div>
                                            <div class="divRow dialogDivRow">
                                                <div style="float: left; padding-left: 25px; padding-top: 4px;">
                                                    <asp:Label ID="lblHolidaySkipDesc" runat="server" Style="color: dodgerblue!important" Text="Ej.: Con repeticiones 'Mañana','Tarde','Noche', un lunes copiamos 'Mañana' y el martes tiene vacaciones. El programa proseguirá copiando el miércoles el horario siguiente 'Noche'." CssClass="editTextFormat" />
                                                </div>
                                            </div>
                                        </div>

                                        <div id="divHolidaysIgnore" runat="server">
                                            <div class="divRow dialogDivRow">
                                                <div style="float: left">
                                                    <dx:ASPxRadioButton ID="rbHolidayIgnore" runat="server" Checked="false" GroupName="rbHolidayDays" Text="" />
                                                </div>
                                                <div style="float: left; padding-left: 5px; padding-top: 4px;">
                                                    <asp:Label ID="lblHolidayIgnore" runat="server" Text="Saltar el día pero mantener el horario que íbamos a copiar y proseguir" CssClass="editTextFormat" />
                                                </div>
                                            </div>
                                            <div class="divRow dialogDivRow">
                                                <div style="float: left; padding-left: 25px; padding-top: 4px;">
                                                    <asp:Label ID="lblHolidayIgnoreDesc" runat="server" Style="color: dodgerblue!important" Text="Ej.: Con repeticiones 'Mañana','Tarde','Noche', un lunes copiamos 'Mañana' y el martes tiene vacaciones. El programa proseguirá copiando el miércoles el horario 'Tarde'." CssClass="editTextFormat" />
                                                </div>
                                            </div>
                                        </div>

                                        <div>
                                            <div class="divRow dialogDivRow">
                                                <div style="float: left">
                                                    <dx:ASPxRadioButton ID="rbHolidayOverwrite" runat="server" Checked="false" GroupName="rbHolidayDays" Text="" />
                                                </div>
                                                <div style="float: left; padding-left: 5px; padding-top: 4px;">
                                                    <asp:Label ID="lblHolidayOverwrite" runat="server" Text="Sobreescribir aunque haya vacaciones" CssClass="editTextFormat" />
                                                </div>
                                            </div>
                                        </div>
                                    </Content>
                                </roUserControls:roGroupBox>
                            </div>
                        </div>
                    </td>
                </tr>
            </table>
        </form>
    </div>

    <div id="<%= Me.ClientID %>_bloqued_frm" class="ui-dialog-content">
        <form id="<%= Me.ClientID %>_bloqued_attr">
            <table style="width: 100%" cellspacing="0" class="bodyPopup">
                <tr style="height: 20px;">
                    <td>
                        <div class="panHeader2">
                            <span style="">
                                <asp:Label runat="server" ID="lblAdvBloqued" Text="Cuando se encuentra un día bloqueado..."></asp:Label>
                            </span>
                        </div>
                    </td>
                </tr>

                <tr>
                    <td style="padding: 15px">
                        <div style="width: 100%">
                            <div class="panBottomMargin">
                                <div class="divRow">
                                    <roUserControls:roGroupBox ID="RoGroupBox1" runat="server">
                                        <Content>
                                            <div class="divRow dialogDivRow">
                                                <div style="float: left">
                                                    <dx:ASPxRadioButton ID="rbBloquedSkip" runat="server" Checked="true" GroupName="rbBloquedDays" Text="" />
                                                </div>
                                                <div style="float: left; padding-left: 5px; padding-top: 4px;">
                                                    <asp:Label ID="lblBloquedSkip" runat="server" Text="Saltar el día que y el horario que íbamos a copiar y proseguir" CssClass="editTextFormat" />
                                                </div>
                                            </div>
                                            <div class="divRow dialogDivRow">
                                                <div style="float: left; padding-left: 25px; padding-top: 4px;">
                                                    <asp:Label ID="lblBloquedSkipDesc" runat="server" Style="color: dodgerblue!important" Text="Ej.: Con repeticiones 'Mañana','Tarde','Noche', un lunes copiamos 'Mañana' y el martes está bloqueado para copiar 'Tarde'. El programa proseguirá copiando el miércoles el horario siguiente 'Noche'." CssClass="editTextFormat" />
                                                </div>
                                            </div>
                                            <div class="divRow dialogDivRow">
                                                <div style="float: left">
                                                    <dx:ASPxRadioButton ID="rbBloquedIgnore" runat="server" Checked="false" GroupName="rbBloquedDays" Text="" />
                                                </div>
                                                <div style="float: left; padding-left: 5px; padding-top: 4px;">
                                                    <asp:Label ID="lblBloquedIgnore" runat="server" Text="Saltar el día pero mantener el horario que íbamos a copiar y proseguir" CssClass="editTextFormat" />
                                                </div>
                                            </div>
                                            <div class="divRow dialogDivRow">
                                                <div style="float: left; padding-left: 25px; padding-top: 4px;">
                                                    <asp:Label ID="lblBloquedIgnoreDesc" runat="server" Style="color: dodgerblue!important" Text="Ej.: Con repeticiones 'Mañana','Tarde','Noche', un lunes copiamos 'Mañana' y el martes está bloqueado para copiar 'Tarde'. El programa proseguirá copiando el miércoles el horario 'Tarde'." CssClass="editTextFormat" />
                                                </div>
                                            </div>

                                            <div class="divRow dialogDivRow">
                                                <div style="float: left">
                                                    <dx:ASPxRadioButton ID="rbBloquedOverWrite" runat="server" Checked="false" GroupName="rbBloquedDays" Text="" />
                                                </div>
                                                <div style="float: left; padding-left: 5px; padding-top: 4px;">
                                                    <asp:Label ID="lblBloquedOverWrite" runat="server" Text="Sobreescribir aunque este bloqueado" CssClass="editTextFormat" />
                                                </div>
                                            </div>
                                        </Content>
                                    </roUserControls:roGroupBox>
                                </div>
                            </div>
                        </div>
                    </td>
                </tr>
            </table>
        </form>
    </div>
</div>