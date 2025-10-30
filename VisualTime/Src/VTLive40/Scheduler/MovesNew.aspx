<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Scheduler_MovesNew" CodeBehind="MovesNew.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<%@ Register Src="Controls/LocalizationMapControl.ascx" TagName="LocalizationMapControl" TagPrefix="uc1" %>

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Edición de ${Punches}</title>

    <script type="text/javascript">

        var isPostBackValue = false;
        var hasMovesNewChanges = 0;
        var _timeout;

        function PageBase_Load() {
            if (isPostBackValue == false) {
                isPostBackValue = true;
                initialMovesNewLoad();

                var hdnshowAlert = document.getElementById("hdnMustShowUserAlert");
                if (hdnshowAlert && hdnshowAlert.value == "1") {
                    try {
                        var url = "Scheduler/srvMsgBoxScheduler.aspx?action=Message";
                        url = url + "&TitleKey=INFO.userChanged";
                        url = url + "&DescriptionKey=Info.UserInfoDesc";
                        url = url + "&Option1TextKey=INFO.OK";
                        url = url + "&Option1DescriptionKey=";
                        url = url + "&Option1OnClickScript=HideMsgBoxForm();return false;";
                        url = url + "&IconUrl=~/Base/Images/MessageFrame/dialog-information.png";
                        window.parent.parent.ShowMsgBoxForm(url, 400, 300, '');
                    } catch (e) { showError("showErrorPopup", e); }
                    hdnshowAlert.value = "0";
                }

                var hdnshowAlert = document.getElementById("hdnExceptionOccurred");
                if (hdnshowAlert && hdnshowAlert.value == "1") {
                    try {
                        RefreshPageComplete(true);
                        hdnshowAlert.value = "0";
                    } catch (e) { showError("showErrorPopup", e); }
                }

            } else {
                GetDailyScheduleStatus(false);
            }

            //Activar pestaña TOTAL o DIARIO del grid de acumulados
            var SchedulerMovesTabAcumSelected = TabAcumSelected("GET");
            if (SchedulerMovesTabAcumSelected == "ANUAL") {
                var btn = ASPxClientButton.Cast(btnShowTotalAcumClient);
                btn.SetChecked(true);
            }
            else {
                var btn = ASPxClientButton.Cast(btnShowDailyAcumClient);
                btn.SetChecked(true);
            }
        }

        function TabAcumSelected(mode, value) {
            if (mode == "SAVE") {
                eraseCookie("SchedulerMovesTabAcumSelected");
                if (value == "ANUAL")
                    createCookie("SchedulerMovesTabAcumSelected", "ANUAL");
                else
                    createCookie("SchedulerMovesTabAcumSelected", "DAILY");
            }
            else {
                if (mode == "GET") {
                    var SchedulerMovesTabAcumSelected = readCookie("SchedulerMovesTabAcumSelected", "ANUAL");
                    if (SchedulerMovesTabAcumSelected == "ANUAL")
                        return "ANUAL";
                    else
                        return "DAILY";
                }
            }
        }

        //MOSTRAR DATOS ANUALES EN EL GRID ACUMULADOS
        function ShowTotalAcum(s, e) {
            TabAcumSelected("SAVE", "ANUAL");
            var miCallback = ASPxClientCallback.Cast("CallbackSessionClient");
            var jasonificado = jasonifica("SHOWTOTALACUM");
            miCallback.PerformCallback(jasonificado);
        }

        //MOSTRAR DATOS DIARIOS EN EL GRID ACUMULADOS
        function ShowDailyAcum(s, e) {
            TabAcumSelected("SAVE", "DAILY");
            var miCallback = ASPxClientCallback.Cast("CallbackSessionClient");
            var jasonificado = jasonifica("SHOWDAILYACUM");
            miCallback.PerformCallback(jasonificado);
        }

        function ShowMyProgress() {
            var divBackgroundCheckStatus = document.getElementById("divBackgroundCheckStatus");
            if (divBackgroundCheckStatus != null) {
                divBackgroundCheckStatus.style.display = "";
                divBackgroundCheckStatus.style.width = '968px';
                divBackgroundCheckStatus.style.height = '218px';
                divBackgroundCheckStatus.style.left = '0px';
            }
        }

        function HideMyProgress() {
            var divBackgroundCheckStatus = document.getElementById("divBackgroundCheckStatus");
            if (divBackgroundCheckStatus != null) {
                divBackgroundCheckStatus.style.display = "none";
            }
        }

        function HideMyProgressAccruals() {
            var divBackgroundCheckStatus = document.getElementById("divBackgroundCheckStatus");
            if (divBackgroundCheckStatus != null) {
                divBackgroundCheckStatus.style.display = "";
                divBackgroundCheckStatus.style.width = '427px';
                divBackgroundCheckStatus.style.left = '974px';
                divBackgroundCheckStatus.style.height = '218px';
            }
        }
    </script>
</head>

<body class="bodyPopup" style="background-attachment: fixed;">

    <%If DesignMode Then%>
    <script src="~/DevExpressScript/ASPxScriptIntelliSense.js" type="text/javascript"></script>
    <%End If%>

    <form id="frmMovesNew" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <div id="divPage" style="width: 100%; height: 100%">

            <!-- ASPxCallback1 General -->
            <dx:ASPxCallback ID="CallbackSession" runat="server" ClientInstanceName="CallbackSessionClient" ClientSideEvents-CallbackComplete="CallbackSession_CallbackComplete">
            </dx:ASPxCallback>

            <dx:ASPxCallback ID="StatusCallback" runat="server" ClientInstanceName="StatusCallbackClient" ClientSideEvents-CallbackComplete="StatusCallback_CallbackComplete">
            </dx:ASPxCallback>

            <dx:ASPxCallbackPanel ID="ASPxMovesNewPanel" runat="server" Width="100%" Height="100%" ClientInstanceName="ASPxMovesNewPanelClient">
                <SettingsLoadingPanel Enabled="false" />
                <ClientSideEvents EndCallback="ASPxMovesNewPanel_EndCallBack" />
                <PanelCollection>
                    <dx:PanelContent ID="PanelContent6" runat="server">
                        <asp:HiddenField ID="hdnIDEmployeePage" Value="0" runat="server" />
                        <asp:HiddenField ID="hdnDatePage" Value="" runat="server" />
                        <asp:HiddenField ID="hdnViewPage" Value="0" runat="server" />

                        <!-- Contingut general -->
                        <div id="divContenido" runat="server" style="width: 100%; height: 94%;">
                            <!-- FILA COMANDOS -->
                            <table class="panHeaderNew" style="height: 40px; width: 100%;">
                                <tr>
                                    <td style="width: 40px; padding-left: 24px;">
                                        <asp:Label ID="lblView" runat="server" Text="Vista:"></asp:Label>
                                    </td>

                                    <td style="width: 230px;">
                                        <span>
                                            <dx:ASPxComboBox ID="cmbView" runat="server" Width="220px" ClientInstanceName="cmbViewClient" Visible="true" CssClass="editTextFormat" Font-Size="11px">
                                                <ClientSideEvents SelectedIndexChanged="cmbView_SelectedIndexChanged" KeyDown="function(s, e) { DoProcessEnterKey(e.htmlEvent); }" />
                                            </dx:ASPxComboBox>
                                        </span>
                                        <span>
                                            <asp:Label ID="lblSubtitle" runat="server" Text="x" Visible="false" />
                                        </span>
                                    </td>
                                    <td style="width: 56px;">
                                        <asp:Label ID="lblEmployeeName" Text="Empleado:" runat="server" Style="" />&nbsp;
                                    </td>
                                    <td style="width: 180px;">
                                        <asp:TextBox ID="txtEmployeeName" Text="" ReadOnly="true" runat="server" CssClass="textClass x-form-text x-form-field" Style="width: 170px;" onkeydown="return DoProcessEnterKeyEx(event);" />
                                    </td>
                                    <td style="width: 10px;">
                                        <asp:ImageButton ID="ibtPreviousEmployee" ImageUrl="Images/left.png" runat="server" CausesValidation="False" OnClientClick="navigateToPreviousEmployee(); return false;" />
                                    </td>
                                    <td style="width: 65px;">
                                        <asp:ImageButton ID="ibtNextEmployee" ImageUrl="Images/right.png" runat="server" Width="16px" CausesValidation="False" OnClientClick="navigateToNextEmployee(); return false;" />
                                    </td>
                                    <td style="width: 110px; padding-right: 5px; text-align: right;">
                                        <asp:Label ID="lblDate" Text="Fecha ${Punch}:" Visible="true" runat="server" />
                                    </td>
                                    <td style="width: 100px; padding-right: 5px;">
                                        <span>
                                            <dx:ASPxDateEdit ID="txtDatePage" runat="server" AllowNull="False" Width="120px" CssClass="editTextFormat" Font-Size="11px" ClientInstanceName="txtDatePageClient" AllowMouseWheel="False" AutoPostBack="False"  MinDate="1900-01-01" MaxDate="2079-06-06">
                                                <CalendarProperties ClearButtonText="Vaciar" TodayButtonText="Hoy">
                                                    <FastNavProperties OkButtonText="Aceptar" CancelButtonText="Cancelar"></FastNavProperties>
                                                </CalendarProperties>
                                                <ClientSideEvents DateChanged="navigateToSelectedDate" />
                                            </dx:ASPxDateEdit>
                                        </span>
                                        <span>
                                            <asp:TextBox ID="txtDate2" Text="" ReadOnly="true" Style="width: 65px; text-align: center;" runat="server" CssClass="textClass x-form-text x-form-field" Visible="false" />
                                        </span>
                                    </td>
                                    <td style="width: 10px;">
                                        <asp:ImageButton ID="ibtPreviousDate" ImageUrl="Images/left.png" runat="server" ToolTip="Día anterior" OnClientClick="navigateToPreviousDate(); return false;" />
                                    </td>
                                    <td style="width: 65px;">
                                        <asp:ImageButton ID="ibtNextDate" ImageUrl="Images/right.png" runat="server" ToolTip="Día siguiente" OnClientClick="navigateToNextDate(); return false;" />
                                    </td>
                                    <td>
                                        <asp:Panel runat="server" ID="pnlSelector" ScrollBars="None" Height="20px">
                                            <table id="movesmaster-selector-buttons" style="vertical-align: middle;">
                                                <tr>
                                                    <td valign="top">
                                                        <asp:ImageButton ID="ibtSelector" ImageUrl="~/Base/Images/txtFilter/icoSearch.png" ToolTip="Búsqueda avanzada" runat="server" OnClientClick="CanOpenSelector(null);return false;" />&nbsp;
                                                    </td>
                                                    <td valign="top">
                                                        <asp:ImageButton ID="ibtPreviousSelector" ImageUrl="Images/left.png" runat="server" CausesValidation="False" OnClientClick="navigateToPreviousSelector(); return false;" />
                                                    </td>
                                                    <td valign="top">
                                                        <asp:ImageButton ID="ibtNextSelector" ImageUrl="Images/right.png" runat="server" Width="16px" CausesValidation="False" OnClientClick="navigateToNextSelector(); return false;" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </asp:Panel>
                                    </td>
                                </tr>
                            </table>

                            <div style="width: 100%">
                                <div class="MovesNewRow">
                                    <!-- GRID MOVES -->
                                    <div class="MovesNewLeftColumn">
                                        <div class="jsGrid">
                                            <asp:Label ID="lblPunchesCaption" runat="server" CssClass="jsGridTitle" Text="Fichajes"></asp:Label>
                                            <div class="jsgridButton">
                                                <dx:ASPxButton ID="btnAddNewMove" runat="server" AutoPostBack="False" CausesValidation="False" Text="Añadir" ToolTip="Añadir nuevo fichaje" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat">
                                                    <Image Url="~/Base/Images/Grid/add.png"></Image>
                                                    <ClientSideEvents Click="AddNewMove" />
                                                </dx:ASPxButton>
                                            </div>
                                        </div>
                                        <div class="jsGridContent">
                                            <dx:ASPxGridView ID="GridMoves" runat="server" Cursor="pointer" AutoGenerateColumns="False" ClientInstanceName="GridMovesClient" KeyboardSupport="True" Width="100%" ClientSideEvents-BeginCallback="GridMovesClient_beginCallback">
                                                <Settings ShowTitlePanel="False" VerticalScrollBarMode="Auto" UseFixedTableLayout="True" VerticalScrollableHeight="150" />
                                                <SettingsPager EnableAdaptivity="true"></SettingsPager>
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
                                                <SettingsBehavior AllowFocusedRow="True" AllowSelectSingleRowOnly="True" AllowSort="False" />
                                                <SettingsPager Mode="ShowAllRecords" ShowEmptyDataRows="false"></SettingsPager>
                                                <ClientSideEvents EndCallback="GridMovesClient_EndCallback" CustomButtonClick="GridMoves_CustomButtonClick" />
                                            </dx:ASPxGridView>
                                        </div>
                                    </div>
                                    <!-- HORARIOS Y AUSENCIAS PREVISTAS -->
                                    <div class="MovesNewRightColumn">
                                        <!-- HORARIOS -->
                                        <div>
                                            <div class="panHeaderNew" style="height: 37px;">
                                                <div style="float: left; text-align: left; width: 100%;">
                                                    <asp:Label ID="lblShiftCaption" Text="Horario" runat="server" CssClass="panHeaderLabelNew" />
                                                </div>
                                            </div>
                                            <div style="width: 425px; padding-top: 3px;">
                                                <dx:ASPxCallbackPanel ID="CallbackPanelShift" runat="server" Width="100%" ClientInstanceName="CallbackPanelShiftClient" ClientSideEvents-EndCallback="CallbackPanelShiftClient_EndCallback">
                                                    <PanelCollection>
                                                        <dx:PanelContent ID="PanelContent2" runat="server">
                                                            <div id="divShiftShape" runat="server" class="RoundCornerFrame" style="border: thin solid #CDCDCD; width: 100%; height: 60px; border-radius: 5px;">
                                                                <span style="float: left; padding-top: 10px; padding-left: 10px; padding-right: 5px;">
                                                                    <asp:TextBox ID="txtShiftUsed" Text="" ReadOnly="true" runat="server" CssClass="textClass x-form-text x-form-field"
                                                                        Width="375px" Style="padding-left: 3px;" onkeydown="return DoProcessEnterKeyEx(event);" />
                                                                </span>
                                                                <span style="float: left; padding-top: 13px;">
                                                                    <asp:ImageButton ID="ibtShiftSelector" ImageUrl="~/Scheduler/Images/down.png" OnClientClick="CanShowShiftSelector(); return false;" runat="server" />
                                                                </span>
                                                                <br />
                                                                <span runat="server" id="spRemoveHolidays" style="float: left; padding: 5px 10px;">
                                                                    <asp:ImageButton ID="ibtRemoveHolidays" ImageUrl="~/Scheduler/Images/Collapse.png" OnClientClick="RemoveHolidays(); return false;" runat="server" />
                                                                    <asp:Label ID="lbRemoveHolidays" CssClass="textClass" Text="Quitar vacaciones" runat="server" Style="vertical-align: top; border: 0" />
                                                                </span>
                                                                <asp:HiddenField ID="hdnIDShiftChange" runat="server" />
                                                                <asp:HiddenField ID="hdnStartShiftChange" Value="" runat="server" />
                                                                <asp:HiddenField ID="hdnIDAssignmentChange" Value="" runat="server" />
                                                            </div>
                                                            <asp:Label ID="Label1" Text="Selección de ${Shifts}" runat="server" Style="display: none; visibility: hidden" />
                                                            <asp:HiddenField ID="hdnIDShiftPage" runat="server" />
                                                        </dx:PanelContent>
                                                    </PanelCollection>
                                                    <ClientSideEvents EndCallback="CallbackPanelShiftClient_EndCallback"></ClientSideEvents>
                                                </dx:ASPxCallbackPanel>
                                            </div>
                                            <!-- AUSENCIAS PREVISTAS -->
                                            <div id="divpanHeader" runat="server" style="padding-top: 5px; height: 100px;">
                                                <dx:ASPxCallbackPanel ID="CallbackPanelAbsence" runat="server" Width="100%" ClientInstanceName="CallbackPanelAbsenceClient">
                                                    <PanelCollection>
                                                        <dx:PanelContent ID="PanelContent1" runat="server">
                                                            <div class="panHeaderNew" style="height: 37px;">
                                                                <div style="float: left; text-align: left; width: 64%;">
                                                                    <dx:ASPxLabel ID="lblAbsenceDetailsInfo" Text="" runat="server" CssClass="panHeaderLabelNew" Cursor="pointer">
                                                                        <ClientSideEvents Click="ShowCurrentForecasts" />
                                                                    </dx:ASPxLabel>
                                                                    <dx:ASPxHiddenField ID="hdnAbsencesInfo" runat="server" ClientInstanceName="hdnAbsencesInfoClient" />
                                                                </div>
                                                                <div style="float: right; margin-top: 7px; margin-right: 4px;">
                                                                    <table>
                                                                        <tr>
                                                                            <td>
                                                                                <dx:ASPxButton ID="btnAddNewAbsence" runat="server" AutoPostBack="False" CausesValidation="False" Text="Añadir" ToolTip="Añadir nueva Ausencia" Width="50px" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlatWithoutWith" CssPostfix="onlyPadding">
                                                                                    <Image Url="~/Base/Images/Grid/add.png"></Image>
                                                                                    <ClientSideEvents Click="CanAddNewAbsence" />
                                                                                </dx:ASPxButton>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </div>
                                                            </div>
                                                            <div id="addAbsencePopover" style="display: none">
                                                                <div id="absencesAvailableList"></div>
                                                            </div>

                                                            <div id="listAbsencesPopover" style="display: none">
                                                                <div class="jsGrid" style="width: calc(100%) !important;">
                                                                    <asp:Label ID="lblForecastsTitle" runat="server" CssClass="jsGridTitle" Text="Previsiones existentes"></asp:Label>
                                                                </div>
                                                                <div id="divForecastsGrid" class="jsGridContent dextremeGrid">
                                                                    <!-- Carrega del Grid Usuari General -->
                                                                </div>
                                                            </div>
                                                        </dx:PanelContent>
                                                    </PanelCollection>
                                                </dx:ASPxCallbackPanel>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="MovesNewRow">
                                    <div class="MovesNewLeftColumn">
                                        <div id="divBackgroundCheckStatus" class="cssBackgroundCheckStatus" style="position: absolute; width: 736px; display: none; height: 215px;">
                                            <div style="display: block; padding-top: 90px; text-align: center;">
                                                <asp:Image ID="imgLoading" ImageUrl="~/Base/Images/Progress/yui/activity.gif" runat="server" />
                                                <br />
                                                <asp:Label ID="lblEditIncidences" Text="Actualizando datos. Por favor espere ..." Font-Bold="true" ForeColor="white" runat="server" />
                                            </div>
                                        </div>

                                        <!-- GRID INCIDENCES -->
                                        <div id="tbHtmlIncidences" runat="server" style="width: 100%;">
                                            <div class="jsGrid">
                                                <asp:Label ID="lblIncidencesCaption" runat="server" CssClass="jsGridTitle" Text="Incidencias y justificaciones"></asp:Label>
                                                <div class="jsgridButton">
                                                    <dx:ASPxButton ID="btnAddNewIncidence" runat="server" AutoPostBack="False" CausesValidation="False" Text="Añadir" ToolTip="Añadir nueva justificación" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat">
                                                        <Image Url="~/Base/Images/Grid/add.png"></Image>
                                                        <ClientSideEvents Click="AddNewIncidence" />
                                                    </dx:ASPxButton>
                                                </div>
                                            </div>
                                            <div class="jsGridContent">
                                                <dx:ASPxGridView ID="GridIncidences" runat="server" AutoGenerateColumns="False" ClientInstanceName="GridIncidencesClient"
                                                    KeyboardSupport="True" Width="100%" ClientSideEvents-BeginCallback="gridIncidences_BeginCallback">
                                                    <Settings ShowTitlePanel="False" VerticalScrollBarMode="Auto" UseFixedTableLayout="True" VerticalScrollableHeight="150" />
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
                                                    <SettingsBehavior AllowFocusedRow="True" AllowSelectSingleRowOnly="True" AllowSort="False" />
                                                    <SettingsPager Mode="ShowAllRecords" ShowEmptyDataRows="false"></SettingsPager>
                                                    <ClientSideEvents EndCallback="GridIncidencesClient_EndCallback" FocusedRowChanged="AcceptGridChangesLocally" />
                                                </dx:ASPxGridView>
                                            </div>
                                        </div>

                                        <!-- LOCALIZATION -->
                                        <div id="tbHtmlLocalization" runat="server" style="width: 100%; display: none;">
                                            <div class="jsGrid">
                                                <asp:Label ID="lblLocalization" runat="server" CssClass="jsGridTitle" Text="Localización"></asp:Label>
                                            </div>
                                            <div class="jsGridContent">
                                                <dx:ASPxCallbackPanel ID="CallbackPanelLocalization" runat="server" Width="100%" Height="100%" ClientInstanceName="CallbackPanelLocalizationClient">
                                                    <PanelCollection>
                                                        <dx:PanelContent ID="PanelContent4" runat="server">
                                                            <uc1:LocalizationMapControl ID="LocalizationMapControl1" runat="server" />
                                                        </dx:PanelContent>
                                                    </PanelCollection>
                                                </dx:ASPxCallbackPanel>
                                            </div>
                                        </div>
                                    </div>

                                    <!-- GRID ACUM -->
                                    <div class="MovesNewRightColumn" style="padding-top: 3px;">
                                        <div class="jsGrid" style="padding-top: 4px;">
                                            <asp:Label ID="lblAcumCaption" runat="server" CssClass="jsGridTitle" Text="Saldos"></asp:Label>
                                            <div class="jsgridButton">
                                                <dx:ASPxButton ID="btnShowDailyAcum" Checked="false" Width="50px" runat="server" AutoPostBack="False" CausesValidation="False" GroupName="AcumGroup"
                                                    Text="Diario" ToolTip="Diario" ClientInstanceName="btnShowDailyAcumClient" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlatWithoutWith" CssPostfix="onlyPadding">
                                                    <ClientSideEvents Click="ShowDailyAcum" />
                                                    <CheckedStyle ForeColor="White" BackColor="#636363" />
                                                </dx:ASPxButton>
                                            </div>
                                            <div class="jsgridButton">
                                                <dx:ASPxButton ID="btnShowTotalAcum" Checked="true" Width="50px" runat="server" AutoPostBack="False" CausesValidation="False" GroupName="AcumGroup"
                                                    Text="Total" ToolTip="Total" ClientInstanceName="btnShowTotalAcumClient" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlatWithoutWith" CssPostfix="onlyPadding">
                                                    <ClientSideEvents Click="ShowTotalAcum" />
                                                    <CheckedStyle ForeColor="White" BackColor="#636363" />
                                                </dx:ASPxButton>
                                            </div>
                                        </div>
                                        <div class="jsGridContent">
                                            <dx:ASPxGridView ID="GridAcum" runat="server" AutoGenerateColumns="False" ClientInstanceName="GridAcumClient" KeyboardSupport="True" Width="100%">
                                                <Settings ShowTitlePanel="False" VerticalScrollBarMode="Auto" UseFixedTableLayout="True" VerticalScrollableHeight="150" />
                                                <SettingsPager Mode="ShowAllRecords" ShowEmptyDataRows="false"></SettingsPager>
                                                <SettingsBehavior AllowFocusedRow="True" AllowSelectSingleRowOnly="True" AllowSort="False" />
                                                <Styles>
                                                    <CommandColumn Spacing="5px" />
                                                    <Header CssClass="jsGridHeaderCell" />
                                                    <Cell Wrap="False" />
                                                </Styles>
                                            </dx:ASPxGridView>
                                        </div>
                                    </div>
                                </div>

                                <div id="trHtmlRemarks" runat="server" class="MovesNewRow onlyPadding" style="width: 99.8%">
                                    <div class="panHeaderNew" style="height: 25px; width: 100%; background-position: 6px 6px;">
                                        <div style="float: left; text-align: left; padding-right: 10px;">
                                            <asp:Label ID="lblObs" Text="Observaciones" runat="server" CssClass="panHeaderLabelNew" Style="padding-top: 6px;" />
                                        </div>
                                    </div>
                                    <div style="padding-top: 5px; padding-top: 5px;">
                                        <dx:ASPxCallbackPanel ID="CallbackPanelRemarks" runat="server" Width="100%" ClientInstanceName="CallbackPanelRemarksClient" ClientSideEvents-EndCallback="CallbackPanelRemarksClient_EndCallback">
                                            <PanelCollection>
                                                <dx:PanelContent ID="PanelContent3" runat="server">
                                                    <asp:TextBox ID="txtRemarks" TextMode="MultiLine" Height="50px" Width="100%" runat="server" onkeyup="EnableApplyButton();" onchange="EnableApplyButton();return false;" CssClass="textClass x-form-text x-form-field" Style="padding: 0px;" />
                                                </dx:PanelContent>
                                            </PanelCollection>
                                        </dx:ASPxCallbackPanel>
                                    </div>
                                </div>
                            </div>

                            <!-- SELECTOR EMPLEADOS-FECHAS -->
                            <div id="divSelector" runat="server" style="position: absolute; left: 455px; top: 25px; z-index: 9000; width: 500px; height: 400px; display: none; background-color: Transparent;">
                                <div class="RoundCornerFrame roundCorner bodyPopup">
                                    <table id="movesmaster-selector-grid" cellpadding="2" cellspacing="2" style="text-align: left;">
                                        <tr>
                                            <td>
                                                <asp:Panel ID="panSelectorBody" runat="server" Width="100%" Height="100%">
                                                    <div>
                                                        <div style="float: left">
                                                            <asp:Label ID="lblSelectorCaption" Text="Seleccionar empleado y fecha:" runat="server" Height="30px"></asp:Label>
                                                        </div>
                                                        <div style="float: right">
                                                            <img alt="" id="ControlBox_Close" onclick='OpenSelector(null);' style="cursor: pointer" src="~/Base/Images/btnClose.png" runat="server" />
                                                        </div>
                                                    </div>

                                                    <div style="clear: both">
                                                        <dx:ASPxGridView ID="GridSelector" runat="server" Cursor="pointer" ClientInstanceName="GridSelectorClient" AutoGenerateColumns="False" KeyboardSupport="True" Width="100%">
                                                            <ClientSideEvents RowClick="GridSelector_FocusedRowChanged" />
                                                            <SettingsPager Mode="ShowAllRecords" ShowEmptyDataRows="True"></SettingsPager>
                                                            <Settings VerticalScrollBarMode="Auto" UseFixedTableLayout="True" VerticalScrollableHeight="150" />
                                                            <SettingsBehavior AllowFocusedRow="True" AllowSelectSingleRowOnly="True" AllowSort="False" ProcessFocusedRowChangedOnServer="false" />
                                                            <Styles>
                                                                <Cell Wrap="False"></Cell>
                                                                <TitlePanel CssClass="TitlePanelClass"></TitlePanel>
                                                            </Styles>
                                                        </dx:ASPxGridView>
                                                    </div>

                                                    <%--<asp:HiddenField ID="hdnSelectorBeginDate" runat="server" />
                                                <asp:HiddenField ID="hdnSelectorEndDate" runat="server" />--%>
                                                </asp:Panel>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </div>

                            <!-- VARIABLES HIDDEN -->
                            <div>

                                <asp:HiddenField ID="hdnGridChanges" runat="server" Value="" />

                                <asp:HiddenField ID="hdnClientStatus" Value="-1" runat="server" />

                                <asp:HiddenField ID="hdnGridSelectorRowSel" Value="-1" runat="server" />
                            </div>

                            <!-- VARIABLES HIDDEN 2 -->
                            <div>
                                <asp:HiddenField ID="hdnDetailsShiftDate" runat="server" />
                                <asp:HiddenField ID="hdnDetailsMoveDate" runat="server" />
                                <asp:HiddenField ID="hdnDetailsMoveTime" runat="server" />
                                <asp:HiddenField ID="hdnDetailsType" runat="server" />
                                <asp:HiddenField ID="hdnDetailsActualType" runat="server" />
                                <asp:HiddenField ID="hdnDetailsIdCause" runat="server" />
                                <asp:HiddenField ID="hdnDetailsIdCenter" runat="server" />
                                <asp:HiddenField ID="hdnDetailsIdAction" runat="server" />
                                <asp:HiddenField ID="hdnDetailsIsNotReliable" runat="server" />
                                <asp:HiddenField ID="hdnDetailsIdTerminal" runat="server" />
                                <asp:HiddenField ID="hdnDetailsIdPassport" runat="server" />
                                <asp:HiddenField ID="hdnDetailsNumRow" runat="server" />
                                <asp:HiddenField ID="hdnDetailsCity" runat="server" />
                                <asp:HiddenField ID="hdnDetailsIdZone" runat="server" />
                                <asp:HiddenField ID="hdnDetailsPosition" runat="server" />
                                <asp:HiddenField ID="hdnDetailsIDTask" runat="server" />
                                <asp:HiddenField ID="hdnDetailsTask" runat="server" />
                                <asp:HiddenField ID="hdnDaysToAdd" runat="server" />
                                <asp:HiddenField ID="hdnDetailsInvalidtype" runat="server" />
                                <asp:HiddenField ID="hdnDetailsTypeDetails" runat="server" />
                                <asp:HiddenField ID="hdnDetailsTimeZone" runat="server" />
                                <asp:HiddenField ID="hdnDetailsAddress" runat="server" />
                                <asp:HiddenField ID="HiddenField1" runat="server" />
                                <asp:HiddenField ID="hdnSwitchReliable" runat="server" />
                                <asp:HiddenField ID="hdnSwitchTelecommuting" runat="server" />
                                <asp:HiddenField ID="hdnMaskAlert" runat="server" />
                                <asp:HiddenField ID="hdnTemperatureAlert" runat="server" />
                                <asp:HiddenField ID="hdnVerificationType" runat="server" />
                                <asp:HiddenField ID="hdnIDRequest" runat="server" />
                                <asp:HiddenField ID="hdnIsRequestApproved" runat="server" />
                            </div>
                        </div>

                        <!-- Contingut ERROR -->
                        <div id="divError" runat="server" style="width: 100%; height: 94%;" visible="false">
                            <div style="padding-top: 200px;">
                                <div style="border: thin solid #444444; width: 400px; margin-left: auto; margin-right: auto; text-align: center; padding-bottom: 20px; padding-top: 20px;" class="defaultBackgroundColor">
                                    <asp:Label runat="server" ID="lblError" Text="" CssClass="editTextFormat" Font-Size="Large" Font-Bold="true"></asp:Label>
                                </div>
                            </div>
                        </div>

                        <!-- BOTONS -->
                        <div style="width: 100%; height: 60px;">
                            <table align="right" cellspacing="3">
                                <tr>
                                    <td>
                                        <dx:ASPxButton ID="btnAply" runat="server" AutoPostBack="False" CausesValidation="False" Text="Aplicar" ToolTip="Aplicar cambios" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat" ClientInstanceName="btnAplyClient">
                                            <Image Url="~/Base/Images/Grid/button_ok.png"></Image>
                                            <ClientSideEvents Click="function(s,e){SaveAll();}" />
                                        </dx:ASPxButton>
                                    </td>
                                    <td>
                                        <dx:ASPxButton ID="btnUndo" runat="server" AutoPostBack="False" CausesValidation="False" Text="Deshacer" ToolTip="Deshacer cambios" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat">
                                            <Image Url="~/Base/Images/Grid/button_undo.png"></Image>
                                            <ClientSideEvents Click="function(s,e){CanUndoAll();}" />
                                        </dx:ASPxButton>
                                    </td>
                                    <td>
                                        <dx:ASPxButton ID="bntClose" runat="server" AutoPostBack="False" CausesValidation="False" Text="Cerrar" ToolTip="Cerrar" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat" ClientInstanceName="bntCloseClient">
                                            <Image Url="~/Base/Images/Grid/button_cancel.png"></Image>
                                            <ClientSideEvents Click="function(s,e){CanCloseMe();}" />
                                        </dx:ASPxButton>
                                    </td>
                                </tr>
                            </table>
                            <asp:HiddenField ID="hdnMustShowUserAlert" Value="0" runat="server" />
                            <asp:HiddenField ID="hdnExceptionOccurred" Value="0" runat="server" />
                            <asp:HiddenField ID="hdnCanClose_PageBase" runat="server" />
                            <asp:HiddenField ID="hdnMustRefresh_PageBase" Value="0" runat="server" />
                            <asp:HiddenField ID="hdnParams_PageBase" Value="0" runat="server" />
                        </div>
                    </dx:PanelContent>
                </PanelCollection>
            </dx:ASPxCallbackPanel>

            <Local:MsgBoxForm ID="MsgBoxForm1" runat="server" />

            <Local:ExternalForm ID="externalform1" runat="server" />

            <%--<Local:MessageForm ID="MessageFrame1" runat="server" />--%>

            <!-- POPUP DETALLE DEL FICHAJE -->
            <dx:ASPxPopupControl ID="PopDetails" runat="server" AllowDragging="False" Modal="true" CloseAction="None" ClientInstanceName="PopDetailsClient"
                PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter" Width="700px" Height="620px"
                ShowHeader="False" ScrollBars="Auto" ShowPageScrollbarWhenModal="True" PopupAnimationType="None" BackColor="Transparent" ContentStyle-Paddings-Padding="0px" Border-BorderColor="Transparent" ShowShadow="false">
                <ClientSideEvents Shown="function(s, e) {setTimeout(function(){txtDetailsTimeClient.Focus();},200);}" />
                <ContentStyle>
                    <Paddings Padding="0px" />
                </ContentStyle>
                <Border BorderStyle="None" />
                <ContentCollection>
                    <dx:PopupControlContentControl ID="PopupControlContentControl1" runat="server">

                        <dx:ASPxLoadingPanel ID="LoadingPanelDetails" runat="server" ContainerElementID="CallbackPanelDetails" ClientInstanceName="LoadingPanelDetailsClient"
                            HorizontalOffset="300" VerticalOffset="200" Text="x" Border-BorderColor="#6C6C6C" Border-BorderStyle="Solid" Border-BorderWidth="1px">
                        </dx:ASPxLoadingPanel>

                        <dx:ASPxCallbackPanel ID="CallbackPanelDetails" ClientInstanceName="CallbackPanelDetailsClient" runat="server">
                            <SettingsLoadingPanel Enabled="false" />
                            <ClientSideEvents BeginCallback="CallbackPanelDetailsClient_BeginCallback" EndCallback="CallbackPanelDetailsClient_EndCallback" />
                            <PanelCollection>
                                <dx:PanelContent ID="PanelContent5" runat="server">
                                    <div id="tbMoveDetail">
                                        <div class="bodyPopupExtended" style="">
                                            <div class="panHeader2">
                                                <asp:Label ID="lblDetailsTitle" runat="server" Text="Detalles" CssClass="panHeaderLabel" />
                                            </div>
                                            <div>
                                                <div style="width: 80%; float: left">
                                                    <!-- fila del dia del horario -->
                                                    <div style="width: 100%">
                                                        <div style="float: left; padding: 5px 5px 0px 5px;">
                                                            <asp:Label ID="lblDetailsTypeCaption" runat="server" Text="Fichaje de"></asp:Label>
                                                        </div>
                                                        <div style="float: left; padding: 0px;">
                                                            <dx:ASPxComboBox ID="cmbDetailsType" runat="server" Width="120px" Font-Size="11px" CssClass="editTextFormat"
                                                                Font-Names="Arial;Verdana;Sans-Serif" ClientSideEvents-ValueChanged="cmbDetailsTypeClient_ValueChanged"
                                                                ClientInstanceName="cmbDetailsTypeClient" IncrementalFilteringMode="Contains">
                                                                <ClientSideEvents ValueChanged="cmbDetailsTypeClient_ValueChanged" KeyDown="function(s, e) { DoProcessEnterKeyDetails(e.htmlEvent); }"></ClientSideEvents>
                                                                <ValidationSettings ErrorDisplayMode="None" />
                                                            </dx:ASPxComboBox>
                                                        </div>
                                                        <div style="float: left; padding: 5px 5px 0px 5px;">
                                                            <asp:Label ID="lblDetailsTimeCaption" runat="server" Text="a las"></asp:Label>
                                                        </div>
                                                        <div style="float: left; padding: 0px;">
                                                            <dx:ASPxTextBox ID="txtDetailsTime" runat="server" Width="55px" MaskSettings-Mask="HH:mm" CssClass="editTextFormat textClass x-form-text x-form-field"
                                                                Font-Size="11px" Font-Names="Arial;Verdana;Sans-Serif" ClientInstanceName="txtDetailsTimeClient">
                                                                <MaskSettings Mask="HH:mm"></MaskSettings>
                                                                <ValidationSettings Display="None"></ValidationSettings>
                                                                <ClientSideEvents KeyDown="function(s, e) { DoProcessEnterKeyDetails(e.htmlEvent); }" />
                                                            </dx:ASPxTextBox>
                                                        </div>
                                                        <div style="float: left; padding: 0px 5px 0px 5px;">
                                                            <dx:ASPxComboBox ID="cmbDetailsDateSelection" runat="server" Width="150px" Font-Size="11px" CssClass="editTextFormat"
                                                                Font-Names="Arial;Verdana;Sans-Serif" ClientInstanceName="cmbDetailsDateSelectionClient" IncrementalFilteringMode="Contains">
                                                                <ClientSideEvents KeyDown="function(s, e) { DoProcessEnterKeyDetails(e.htmlEvent); }" />
                                                                <ValidationSettings ErrorDisplayMode="None" />
                                                            </dx:ASPxComboBox>
                                                        </div>
                                                    </div>

                                                    <!-- fila del dia del horario -->
                                                    <div style="width: 100%; clear: both; padding-top: 10px">
                                                        <div style="float: left; padding: 5px 5px 0px 5px;">
                                                            <asp:Label ID="lblTask" runat="server" Text="En la tarea"></asp:Label>
                                                            <div id="divCenterCaption" runat="server">
                                                                <asp:Label ID="lblDetailsCenterCaption" runat="server" Text="En el centro"></asp:Label>
                                                            </div>
                                                            <div id="divCauseCaption" runat="server">
                                                                <asp:Label ID="lblDetailsCauseCaption" runat="server" Text="Con la justificación:"></asp:Label>&nbsp;
                                                            </div>
                                                        </div>
                                                        <div style="float: left; padding: 0px;">
                                                            <asp:TextBox ID="txtTask" runat="server" ReadOnly="True" CssClass="textClass x-form-text x-form-field" Width="250px" />
                                                            <div id="divCauseCmb" runat="server">
                                                                <dx:ASPxComboBox ID="cmbDetailsCause" runat="server" Width="300px" Font-Size="11px" CssClass="editTextFormat"
                                                                    ClientInstanceName="cmbDetailsCauseClient" Font-Names="Arial;Verdana;Sans-Serif" IncrementalFilteringMode="Contains">
                                                                    <ClientSideEvents KeyDown="function(s, e) { DoProcessEnterKeyDetails(e.htmlEvent); }" />
                                                                    <ValidationSettings ErrorDisplayMode="None" />
                                                                </dx:ASPxComboBox>
                                                            </div>
                                                            <div id="divCenterCmb" runat="server">
                                                                <dx:ASPxComboBox ID="cmbDetailsCenter" runat="server" Width="350px" Font-Size="11px" ForeColor="#2D4155"
                                                                    ClientInstanceName="cmbDetailsCenterClient" Font-Names="Arial;Verdana;Sans-Serif" IncrementalFilteringMode="Contains">
                                                                    <ClientSideEvents KeyDown="function(s, e) { DoProcessEnterKeyDetails(e.htmlEvent); }" />
                                                                    <ValidationSettings ErrorDisplayMode="None" />
                                                                </dx:ASPxComboBox>
                                                            </div>
                                                        </div>
                                                        <div style="float: left; padding: 2px 5px 0px 0px;">
                                                            <asp:Image ID="ImgTask" runat="server" Style="cursor: pointer" BorderColor="#666666" BorderStyle="Solid" BorderWidth="1px"
                                                                ImageUrl="~/Scheduler/Images/Task.png" onclick="ShowTasks();" />
                                                        </div>
                                                    </div>

                                                    <!-- fila del productiV -->
                                                    <div style="width: 100%; clear: both; padding-top: 10px" id="divTaskFields" runat="server">
                                                        <div style="float: left; padding: 5px 5px 0px 5px;">
                                                            <asp:Label ID="lblFieldTask1" runat="server" Text="Campo1:"></asp:Label>
                                                        </div>
                                                        <div style="float: left; padding: 0px 6px 0px 6px;">
                                                            <dx:ASPxTextBox ID="txtFieldTask1" runat="server" Width="58px" CssClass="editTextFormat textClass x-form-text x-form-field"
                                                                Font-Size="11px" Font-Names="Arial;Verdana;Sans-Serif" ClientInstanceName="txtFieldTask1Client" HorizontalAlign="Right">
                                                                <ClientSideEvents KeyDown="function(s, e) { DoProcessEnterKeyDetails(e.htmlEvent); }" />
                                                                <ValidationSettings ErrorDisplayMode="None" />
                                                            </dx:ASPxTextBox>
                                                        </div>
                                                        <div style="float: left; padding: 2px 5px 0px 0px; min-width: 18px;">
                                                            <asp:Image ID="ImgField1" runat="server" Style="cursor: pointer" BorderColor="#666666" BorderStyle="Solid" BorderWidth="1px"
                                                                ImageUrl="~/Scheduler/Images/ver_fichajes.png" onclick="ShowField(1);" />
                                                        </div>
                                                        <div style="float: left; padding: 5px 5px 0px 5px;">
                                                            <asp:Label ID="lblFieldTask2" runat="server" Text="Campo2:"></asp:Label>
                                                        </div>
                                                        <div style="float: left; padding: 0px 6px 0px 6px;">
                                                            <dx:ASPxTextBox ID="txtFieldTask2" runat="server" Width="58px" CssClass="editTextFormat textClass x-form-text x-form-field"
                                                                Font-Size="11px" Font-Names="Arial;Verdana;Sans-Serif" ClientInstanceName="txtFieldTask2Client" HorizontalAlign="Right">
                                                                <ClientSideEvents KeyDown="function(s, e) { DoProcessEnterKeyDetails(e.htmlEvent); }" />
                                                                <ValidationSettings ErrorDisplayMode="None" />
                                                            </dx:ASPxTextBox>
                                                        </div>
                                                        <div style="float: left; padding: 2px 5px 0px 0px; min-width: 18px;">
                                                            <asp:Image ID="ImgField2" runat="server" Style="cursor: pointer" BorderColor="#666666" BorderStyle="Solid" BorderWidth="1px"
                                                                ImageUrl="~/Scheduler/Images/ver_fichajes.png" onclick="ShowField(2);" />
                                                        </div>

                                                        <div style="float: left; padding: 5px 5px 0px 5px;">
                                                            <asp:Label ID="lblFieldTask3" runat="server" Text="Campo3:"></asp:Label>
                                                        </div>
                                                        <div style="float: left; padding: 0px 6px 0px 6px;">
                                                            <dx:ASPxTextBox ID="txtFieldTask3" runat="server" Width="58px" CssClass="editTextFormat textClass x-form-text x-form-field"
                                                                Font-Size="11px" Font-Names="Arial;Verdana;Sans-Serif" ClientInstanceName="txtFieldTask3Client" HorizontalAlign="Right">
                                                                <ClientSideEvents KeyDown="function(s, e) { DoProcessEnterKeyDetails(e.htmlEvent); }" />
                                                                <ValidationSettings ErrorDisplayMode="None" />
                                                            </dx:ASPxTextBox>
                                                        </div>
                                                        <div style="float: left; padding: 2px 5px 0px 0px; min-width: 18px;">
                                                            <asp:Image ID="ImgField3" runat="server" Style="cursor: pointer" BorderColor="#666666" BorderStyle="Solid" BorderWidth="1px"
                                                                ImageUrl="~/Scheduler/Images/ver_fichajes.png" onclick="ShowField(3);" />
                                                        </div>
                                                    </div>

                                                    <!-- campos de la ficha de tareas -->
                                                    <div style="width: 100%; clear: both; padding-top: 10px" id="divTaskFields2" runat="server">
                                                        <div style="float: left; padding: 5px 5px 0px 5px;">
                                                            <asp:Label ID="lblFieldTask4" Style="width: 30px;" runat="server" Text="Campo4:"></asp:Label>
                                                        </div>
                                                        <div style="float: left; padding: 0px 4px 0px 3px;">
                                                            <dx:ASPxTextBox ID="txtFieldTask4" runat="server" Width="58px" CssClass="editTextFormat textClass x-form-text x-form-field"
                                                                Font-Size="11px" Font-Names="Arial;Verdana;Sans-Serif" ClientInstanceName="txtFieldTask4Client" HorizontalAlign="Right">
                                                                <MaskSettings Mask="<0..999999>.<00..99>" IncludeLiterals="DecimalSymbol" PromptChar=""></MaskSettings>
                                                                <ClientSideEvents KeyDown="function(s, e) { DoProcessEnterKeyDetails(e.htmlEvent); }" />
                                                                <ValidationSettings ErrorDisplayMode="None" />
                                                            </dx:ASPxTextBox>
                                                        </div>
                                                        <div style="float: left; padding: 5px 5px 0px 0px; min-width: 18px;">
                                                            <asp:Image ID="ImgField4" runat="server" Style="cursor: pointer" BorderColor="#666666" BorderStyle="Solid" BorderWidth="1px"
                                                                ImageUrl="~/Scheduler/Images/ver_fichajes.png" onclick="ShowField(4);" />
                                                        </div>
                                                        <div style="float: left; padding: 5px 5px 0px 5px;">
                                                            <asp:Label ID="lblFieldTask5" runat="server" Text="Campo5:"></asp:Label>
                                                        </div>
                                                        <div style="float: left; padding: 0px 4px 0px 2px;">
                                                            <dx:ASPxTextBox ID="txtFieldTask5" runat="server" Width="58px" CssClass="editTextFormat textClass x-form-text x-form-field"
                                                                Font-Size="11px" Font-Names="Arial;Verdana;Sans-Serif" ClientInstanceName="txtFieldTask5Client" HorizontalAlign="Right">
                                                                <MaskSettings Mask="<0..999999>.<00..99>" IncludeLiterals="DecimalSymbol" PromptChar=""></MaskSettings>
                                                                <ClientSideEvents KeyDown="function(s, e) { DoProcessEnterKeyDetails(e.htmlEvent); }" />
                                                                <ValidationSettings ErrorDisplayMode="None" />
                                                            </dx:ASPxTextBox>
                                                        </div>
                                                        <div style="float: left; padding: 5px 5px 0px 0px; min-width: 18px;">
                                                            <asp:Image ID="ImgField5" runat="server" Style="cursor: pointer" BorderColor="#666666" BorderStyle="Solid" BorderWidth="1px"
                                                                ImageUrl="~/Scheduler/Images/ver_fichajes.png" onclick="ShowField(5);" />
                                                        </div>

                                                        <div style="float: left; padding: 5px 5px 0px 5px;">
                                                            <asp:Label ID="lblFieldTask6" runat="server" Text="Campo6:"></asp:Label>
                                                        </div>
                                                        <div style="float: left; padding: 0px 4px 0px 3px;">
                                                            <dx:ASPxTextBox ID="txtFieldTask6" runat="server" Width="58px" CssClass="editTextFormat textClass x-form-text x-form-field"
                                                                Font-Size="11px" Font-Names="Arial;Verdana;Sans-Serif" ClientInstanceName="txtFieldTask6Client" HorizontalAlign="Right">
                                                                <MaskSettings Mask="<0..999999>.<00..99>" IncludeLiterals="DecimalSymbol" PromptChar=""></MaskSettings>
                                                                <ClientSideEvents KeyDown="function(s, e) { DoProcessEnterKeyDetails(e.htmlEvent); }" />
                                                                <ValidationSettings ErrorDisplayMode="None" />
                                                            </dx:ASPxTextBox>
                                                        </div>
                                                        <div style="float: left; padding: 5px 5px 0px 0px; min-width: 18px;">
                                                            <asp:Image ID="ImgField6" runat="server" Style="cursor: pointer" BorderColor="#666666" BorderStyle="Solid" BorderWidth="1px"
                                                                ImageUrl="~/Scheduler/Images/ver_fichajes.png" onclick="ShowField(6);" />
                                                        </div>
                                                    </div>

                                                    <!-- fila de fiabilidad -->
                                                    <div style="width: 100%; clear: both; padding-top: 10px">

                                                        <div style="float: left; padding: 5px 5px 0px 5px;">
                                                            <asp:Label ID="lblDetailsReliability" runat="server" Text="Fiabilidad: "></asp:Label>
                                                        </div>

                                                        <div style="float: left; padding: 2px;" id="switchReliability"></div>
                                                        <div style="float: left; padding: 5px 5px 0px 5px;">
    <asp:Label ID="lblDetailsNotReliable" runat="server" Text="Motivo de no fiabilidad: "></asp:Label>
</div>
                                                    </div>

                                                    <!-- fila de teletrabajo -->
                                                    <div id="divTelecommutingBlock" runat="server">
                                                        <div style="width: 100%; clear: both; padding-top: 10px">

                                                            <div style="float: left; padding: 5px 5px 0px 5px;">
                                                                <asp:Label ID="lblTelecommuting" runat="server" Text="Teletrabajo: "></asp:Label>
                                                            </div>
                                                            <div id="divTelecommuting" runat="server">
                                                                <div style="float: left; padding: 1px 5px 0px 47px;" id="switchTelecommuting"></div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <!-- fila de fiabilidad -->
                                                    <div style="width: 100%; clear: both; padding-top: 10px">
                                                        <div style="width: 110px; float: left; padding: 5px 5px 0px 5px;">
                                                            <asp:Label ID="lblMaskUse" runat="server" Text="Uso de mascarilla: "></asp:Label>
                                                        </div>

                                                        <div style="float: left; padding: 2px;" id="cbMaskControl">
                                                            <dx:ASPxComboBox ID="cmbMaskControl" runat="server" Width="100px" Font-Size="11px" ForeColor="#2D4155" ClientEnabled="false"
                                                                ClientInstanceName="cmbMaskControlClient" Font-Names="Arial;Verdana;Sans-Serif" IncrementalFilteringMode="Contains">
                                                                <ClientSideEvents KeyDown="function(s, e) { DoProcessEnterKeyDetails(e.htmlEvent); }" />
                                                                <ValidationSettings ErrorDisplayMode="None" />
                                                            </dx:ASPxComboBox>
                                                        </div>
                                                        <div style="float: left; padding: 5px 5px 0px 5px;">
                                                            <asp:Label ID="lblTemperatureControl" runat="server" Text="Control de temperatura: "></asp:Label>
                                                        </div>

                                                        <div style="float: left; padding: 2px;" id="cbTemperatureControl">
                                                            <dx:ASPxComboBox ID="cmbTemperatureControl" runat="server" Width="125px" Font-Size="11px" ForeColor="#2D4155" ClientEnabled="false"
                                                                ClientInstanceName="cmbTemperatureControlClient" Font-Names="Arial;Verdana;Sans-Serif" IncrementalFilteringMode="Contains">
                                                                <ClientSideEvents KeyDown="function(s, e) { DoProcessEnterKeyDetails(e.htmlEvent); }" />
                                                                <ValidationSettings ErrorDisplayMode="None" />
                                                            </dx:ASPxComboBox>
                                                        </div>
                                                        <div style="width: 110px; float: left; clear: both; padding: 5px 5px 0px 5px;">
                                                            <asp:Label ID="lblVerifyType" runat="server" Text="Origen: "></asp:Label>
                                                        </div>

                                                        <div style="float: left; padding: 2px;" id="cbVerifyType">
                                                            <dx:ASPxComboBox ID="cmbVerifyType" runat="server" Width="100px" Font-Size="11px" ForeColor="#2D4155" ClientEnabled="false"
                                                                ClientInstanceName="cmbVerifyTypeClient" Font-Names="Arial;Verdana;Sans-Serif" IncrementalFilteringMode="Contains">
                                                                <ClientSideEvents KeyDown="function(s, e) { DoProcessEnterKeyDetails(e.htmlEvent); }" />
                                                                <ValidationSettings ErrorDisplayMode="None" />
                                                            </dx:ASPxComboBox>
                                                        </div>
                                                    </div>

                                                    <!-- fila de comedor -->
                                                    <div style="width: 100%; clear: both; padding-top: 10px; display: none" id="trDinningRoomValidate" runat="server">
                                                        <div style="float: left; padding: 5px 5px 0px 5px;">
                                                            <dx:ASPxCheckBox ID="chkValidateDinningRoom" runat="server" Checked="false" ClientInstanceName="chkValidateDinningRoomClient" />
                                                        </div>
                                                        <div style="float: left; padding: 0px;">
                                                            <asp:Label ID="lblDinningRoomValidateDescription" runat="server" Text=""></asp:Label>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div id="divCaptureRights" runat="server" style="width: 20%; float: right;">
                                                    <div id="dvEmployeePunch" style="width: 150px; display: none;">
                                                        <img id="imgEmployeePunch" style="height: 120px; width: 150px;" alt="" src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mNgYAAAAAMAASsJTYQAAAAASUVORK5CYII=" />
                                                    </div>
                                                    <div id="dvSummaryLoadPunch" style="width: 150px; height: 120px;">
                                                        <img src="../Base/images/icoRefresh.png" id="imgEmployeeCapture" runat="server" clientidmode="Static" alt=""
                                                            style="height: 20px; width: 20px; display: block; padding-top: 40px; margin: auto; cursor: pointer;" onclick="auditAndShowPunchImage()" />
                                                    </div>
                                                </div>
                                            </div>

                                            <div style="clear: both;"></div>

                                            <div class="panHeader2">
                                                <asp:Label ID="lblLocationTitle" runat="server" Text="Localización" CssClass="panHeaderLabel" />
                                            </div>

                                            <div>
                                                <div style="width: 100%; clear: both; padding-top: 10px;" id="trLocation" runat="server">
                                                    <div style="width: 110px; float: left; padding: 5px 5px 0px 5px;">
                                                        <asp:Label ID="lblLocationCity" runat="server" Text="Población "></asp:Label>
                                                    </div>
                                                    <div style="float: left; padding: 0px;">
                                                        <asp:TextBox ID="txtCity" runat="server" ReadOnly="True" CssClass="textClass x-form-text x-form-field" Height="16px" Width="200px"></asp:TextBox>
                                                    </div>
                                                    <div style="float: left; padding: 1px;">
                                                        <asp:Image ID="imgCity" runat="server" Style="cursor: pointer" BorderColor="#666666" BorderStyle="Solid" BorderWidth="1px" ImageUrl="~/Base/Images/tbt/map16.png" onclick="ShowMap();" />
                                                    </div>

                                                    <div style="float: left; padding: 5px 5px 0px 5px;">
                                                        <asp:Label ID="lblTimeZone" runat="server" Text="Zona horaria"></asp:Label>
                                                    </div>
                                                    <div style="float: left; padding: 0px;">
                                                        <asp:TextBox ID="txtTimeZone" runat="server" ReadOnly="True" CssClass="textClass x-form-text x-form-field" Height="16px" Width="200px"></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div style="width: 100%; clear: both; padding-top: 10px;" id="trLocation2" runat="server">
                                                    <div style="width: 110px; float: left; padding: 5px 5px 0px 5px;">
                                                        <asp:Label ID="lblFullAddress" runat="server" Text="Dirección "></asp:Label>
                                                    </div>
                                                    <div style="float: left">
                                                        <asp:TextBox ID="txtFullAddress" runat="server" ReadOnly="True" CssClass="textClass x-form-text x-form-field" Height="16px" Width="506px"></asp:TextBox>
                                                    </div>
                                                </div>

                                                <div style="width: 100%; clear: both; padding-top: 10px;">
                                                    <div style="width: 110px; float: left; padding: 5px 5px 0px 5px;">
                                                        <asp:Label ID="lblLocationTerminal" runat="server" Text="En el terminal"></asp:Label>
                                                    </div>
                                                    <div style="float: left; padding: 0px;">
                                                        <dx:ASPxComboBox ID="cmbTerminal" runat="server" Width="200px" Font-Size="11px" CssClass="editTextFormat"
                                                            ClientInstanceName="cmbTerminalClient" Font-Names="Arial;Verdana;Sans-Serif" IncrementalFilteringMode="Contains">
                                                            <ClientSideEvents KeyDown="function(s, e) { DoProcessEnterKeyDetails(e.htmlEvent); }" />
                                                            <ValidationSettings ErrorDisplayMode="None" />
                                                        </dx:ASPxComboBox>
                                                    </div>
                                                    <div style="float: left; padding: 5px 5px 0px 5px;">
                                                        <div id="tdlblLocationZone" runat="server">
                                                            <asp:Label ID="lblLocationZone" runat="server" Text="De la zona de acceso"></asp:Label>&nbsp;
                                                        </div>
                                                        <div id="tdlblDiningRoomTurn" runat="server">
                                                            <asp:Label ID="lblDiningRoomTurn" runat="server" Text="Turno Comedor"></asp:Label>&nbsp;
                                                        </div>
                                                    </div>
                                                    <div style="float: left; padding: 0px;">
                                                        <div id="tdcmbZone" runat="server">
                                                            <dx:ASPxComboBox ID="cmbZone" runat="server" Width="187px" Font-Size="11px" CssClass="editTextFormat" IncrementalFilteringMode="Contains"
                                                                ClientInstanceName="cmbZoneClient" Font-Names="Arial;Verdana;Sans-Serif">
                                                                <ClientSideEvents KeyDown="function(s, e) { DoProcessEnterKeyDetails(e.htmlEvent); }" />
                                                                <ValidationSettings ErrorDisplayMode="None" />
                                                            </dx:ASPxComboBox>
                                                        </div>
                                                        <div id="tdcmbDiningRoomTurn" runat="server" style="display: block;">
                                                            <dx:ASPxComboBox ID="cmbDiningRoomTurn" runat="server" Width="187px" Font-Size="11px" CssClass="editTextFormat" IncrementalFilteringMode="Contains"
                                                                ClientInstanceName="cmbDiningRoomTurnClient" Font-Names="Arial;Verdana;Sans-Serif">
                                                                <ClientSideEvents KeyDown="function(s, e) { DoProcessEnterKeyDetails(e.htmlEvent); }" />
                                                                <ValidationSettings ErrorDisplayMode="None" />
                                                            </dx:ASPxComboBox>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div style="clear: both;"></div>

                                            <div class="panHeader2">
                                                <asp:Label ID="lblActionsTitle" runat="server" Text="Acciones" CssClass="panHeaderLabel" />
                                            </div>

                                            <div>
                                                <table>
                                                    <tr style="height: 40px; vertical-align: middle;">
                                                        <td>
                                                            <asp:Label ID="lblAction" runat="server" Text="El usuario {0} ha {1} manualmente este fichaje."></asp:Label>
                                                        </td>
                                                        <td id="tdlblErrorDetail" style="display: none;">
                                                            <asp:Label ID="lblErrorDetail" runat="server" Text="" ForeColor="#CC3300" Font-Bold="True"></asp:Label>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>
                                            <!-- BOTONES -->
                                            <table style="margin-left: auto;">
                                                <tr>
                                                    <td>
                                                        <dx:ASPxButton ID="btnAcceptPop" runat="server" AutoPostBack="False" CausesValidation="False" Text="Guardar cambios" ToolTip="${Button.Accept}" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                                            <ClientSideEvents Click="btnAcceptClick" />
                                                        </dx:ASPxButton>
                                                    </td>
                                                    <td>
                                                        <dx:ASPxButton ID="btnCancelPop" runat="server" AutoPostBack="False" CausesValidation="False" Text="${Button.Cancel}" ToolTip="${Button.Cancel}" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                                            <ClientSideEvents Click="CloseClick" />
                                                        </dx:ASPxButton>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                        <!-- VARIABLES HIDDEN -->
                                        <div>
                                            <asp:HiddenField ID="hdnDetailsTypePop" runat="server" />
                                            <asp:HiddenField ID="hdnDetailsKeyRowPop" runat="server" />
                                            <asp:HiddenField ID="hdnDetailsCityPop" runat="server" />
                                            <asp:HiddenField ID="hdnCanEdit" runat="server" />
                                            <asp:HiddenField ID="hdnDetailsIdCurrentActionPop" runat="server" Value="0" />
                                            <asp:HiddenField ID="hdnDetailsHasChangesPop" runat="server" Value="0" />
                                            <asp:HiddenField ID="hdnCanClosePop" runat="server" />
                                            <asp:HiddenField ID="hdnDetailsIdMovePop" runat="server" />
                                            <asp:HiddenField ID="hdnDetailsCanEditPop" runat="server" />
                                            <asp:HiddenField ID="hdnDetailsReliabilityPop" runat="server" />
                                            <asp:HiddenField ID="hdnDetailsTelecommutingPop" runat="server" />
                                            <asp:HiddenField ID="hdnDetailsCanTelecommutePop" runat="server" />
                                            <asp:HiddenField ID="hdnDetailsIdTerminalPop" runat="server" />
                                            <asp:HiddenField ID="hdnDetailsNumRowPop" runat="server" />
                                            <asp:HiddenField ID="hdnDetailsIdPassportPop" runat="server" />
                                            <asp:HiddenField ID="hdnDetailsIdPassportModifiedPop" runat="server" />
                                            <asp:HiddenField ID="hdnDetailsIdActionPop" runat="server" Value="0" />
                                            <asp:HiddenField ID="hdnDetailsShiftDatePop" runat="server" />
                                            <asp:HiddenField ID="hdnDetailsMoveDatePop" runat="server" />
                                            <asp:HiddenField ID="hdnDetailsMoveTimePop" runat="server" />
                                            <asp:HiddenField ID="hdnDetailsPositionPop" runat="server" />
                                            <asp:HiddenField ID="hdnDetailsIDTaskPop" runat="server" />
                                            <asp:HiddenField ID="hdnDetailsField1Pop" runat="server" />
                                            <asp:HiddenField ID="hdnDetailsField2Pop" runat="server" />
                                            <asp:HiddenField ID="hdnDetailsField3Pop" runat="server" />
                                            <asp:HiddenField ID="hdnDetailsField4Pop" runat="server" />
                                            <asp:HiddenField ID="hdnDetailsField5Pop" runat="server" />
                                            <asp:HiddenField ID="hdnDetailsField6Pop" runat="server" />

                                            <asp:HiddenField ID="hdnDetailsInvalidTypePop" runat="server" />
                                            <asp:HiddenField ID="hdnDetailsTypeDetailsPop" runat="server" />
                                        </div>
                                    </div>
                                </dx:PanelContent>
                            </PanelCollection>
                        </dx:ASPxCallbackPanel>
                    </dx:PopupControlContentControl>
                </ContentCollection>
            </dx:ASPxPopupControl>
        </div>
    </form>
</body>
</html>