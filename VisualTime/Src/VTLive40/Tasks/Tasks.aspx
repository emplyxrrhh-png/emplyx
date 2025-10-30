<%@ Page Language="VB" MasterPageFile="~/Base/mastEmp.master" AutoEventWireup="false" Inherits="VTLive40.Tasks_Tasks" Title="${tasks}" CodeBehind="Tasks.aspx.vb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentMainBody" runat="Server">

    <script language="javascript" type="text/javascript">

        function PageBase_Load() {
            ConvertControls();
            //roCB_addComboHandler('cmbGroup_DropDown');

            ASPxCallbackpanelDetailsTaskClient.SetVisible(false);
            panel00.SetVisible(true);
            panel01.SetVisible(false);
            panel02.SetVisible(false);
            panel03.SetVisible(false);
            panel04.SetVisible(false);
            panel05.SetVisible(false);
            panel06.SetVisible(false);
            //$("#deleteTaskCell").hide();

            window.parent.setUPReportsAndWizards({ HasReports: true, ReportsAction: "ShowReports('<%= Me.Language.TranslateJavaScript("Reports.Title", Me.DefaultScope) %>', '', 'Tasks',<%= Me.ReportDefaultVersion %>,'<%= Me.DefaultRootURL %>')", HasAssistants: false });
        }

        //Filtra grid de tareas con tareas pendientes, finalizadas o canceladas.
        function FilterTasksByState(s, e, i) {
            var hdnFilterStatus = document.getElementById("ctl00_contentMainBody_hdnFilterStatus");
            if (hdnFilterStatus != null) {
                if (!s.checked)
                    hdnFilterStatus.value = hdnFilterStatus.value.substr(0, i) + "1" + hdnFilterStatus.value.substr(i + 1);
                else
                    hdnFilterStatus.value = hdnFilterStatus.value.substr(0, i) + "0" + hdnFilterStatus.value.substr(i + 1);
            }
            GridTareas.Refresh();
        }

        function GetTaskDetails(s, e) {
            s.GetRowValues(e.visibleIndex, 'ID;Worked', CargaNodoTarea);
        }

        function UpdateActionsOnFocusing(s, e) {
            s.GetRowValues(e.visibleIndex, 'ID;Worked;Name', updateGridTaskActions);
        }

        function FocuseOnFirstRow(s, e) {
            $(".dxgvDataRow")[1].click();
            $(".dxgvDataRow")[0].click();
        }

        function TaskDetailShowSelector() {
            try {
                var Title = '';
                var iFrm = document.getElementById('<%= GroupSelectorFrame.ClientID %>');
                iFrm.style.width = "475px";
                iFrm.style.height = "290px";

                iFrm.style.top = "5px";
                iFrm.style.left = "5px";

                var strBase = '<%= Me.Page.ResolveUrl("~/Base/") %>' + "WebUserControls/roWizardSelectorContainerMultiSelectV3.aspx?" +
                    "PrefixTree=treeEmpDetailTask&FeatureAlias=Tasks&PrefixCookie=objContainerTreeV3_treeEmpDetailTaskGrid";
                strBase += '&FilterFixed=Employees.Type="J"';
                iFrm.src = strBase;

                ShowGroupSelector();

            }
            catch (e) {
                showError("TaskDetailShowSelector", e);
            }
        }

        function ShowTasksSelector(TypeActivationTask) {
            try {
                hasChanges(true);
                var Title = '';
                $("#hdnControl").val(TypeActivationTask);
                var hBase = '<%= Me.Page.ResolveURL("~/Base/") %>' + "WebUserControls/roFilterListSelector.aspx";
                ShowExternalForm2(hBase, 300, 270, Title, '', false, false, false);
            }
            catch (e) {
                showError("ShowTasksSelector", e);
            }
        }
    </script>

    <asp:HiddenField ID="hdnFilterStatus" runat="server" Value="1001" />
    <asp:HiddenField ID="nameTareas" runat="server" Value="" />
    <input type="hidden" runat="server" id="hdnModeEdit" value="" />

    <input type="hidden" runat="server" id="IDLoadTask" value="0" />

    <input type="hidden" id="hdnCaptionGrid" value="<%= Me.Language.Translate("CaptionGrid", Me.DefaultScope) %>" />
    <input type="hidden" id="hdnSeleccionar" value="<%= Me.Language.Translate("TaskDetail.Seleccionar",Me.DefaultScope) %>,<%= Me.Language.Translate("TaskDetail.Seleccionados",Me.DefaultScope) %>" />
    <input type="hidden" id="hdnCaptionGridStatsCol1" value="<%= Me.Language.Translate("CaptionGridStats.Employee",Me.DefaultScope) %>,<%= Me.Language.Translate("CaptionGridStats.Date",Me.DefaultScope) %>,<%= Me.Language.Translate("CaptionGridStats.Group",Me.DefaultScope) %>,<%= Me.Language.Translate("CaptionGridStats.Diversions",Me.DefaultScope) %>" />
    <input type="hidden" id="hdnCaptionGridStatsCol2" value="<%= Me.Language.Translate("CaptionGridStats.Total",Me.DefaultScope) %>" />
    <input type="hidden" id="hdnControl" value="0" />
    <input type="hidden" id="hdnCaptionTaskFieldsName" value="<%= Me.Language.Translate("CaptionGridTaskFields.Name",Me.DefaultScope) %>" />
    <input type="hidden" id="hdnCaptionTaskFieldsValue" value="<%= Me.Language.Translate("CaptionGridTaskFields.Value",Me.DefaultScope) %>" />
    <input type="hidden" id="hdnCaptionTaskFieldsAction" value="<%= Me.Language.Translate("CaptionGridTaskFields.Action",Me.DefaultScope) %>" />
    <input type="hidden" id="hdnCaptionTaskAlertsName" value="<%= Me.Language.Translate("CaptionGridTaskAlerts.Name",Me.DefaultScope) %>" />
    <input type="hidden" id="hdnCaptionTaskAlertsDate" value="<%= Me.Language.Translate("CaptionGridTaskAlerts.Date",Me.DefaultScope) %>" />
    <input type="hidden" id="hdnCaptionTaskAlertsComment" value="<%= Me.Language.Translate("CaptionGridTaskAlerts.Comment",Me.DefaultScope) %>" />
    <input type="hidden" id="hdnCaptionTaskAlertsReaded" value="<%= Me.Language.Translate("CaptionGridTaskAlerts.Readed",Me.DefaultScope) %>" />
    <input type="hidden" id="hdnCaptionTaskAssignmentName" value="<%= Me.Language.Translate("CaptionGridTaskAssignments.Readed",Me.DefaultScope) %>" />

    <input type="hidden" id="msgHasChanges" value="" runat="server" clientidmode="Static" />
    <input type="hidden" id="msgHasErrors" value="" runat="server" clientidmode="Static" />

    <div id="divMainBody">
        <!-- TAB SUPERIOR -->
        <div id="divTabInfo" class="divDataCells">
            <div style="min-height: 10px"></div>
            <div id="divAccessGroup" class="blackRibbonTitle">
                <div class="blackRibbonIcon">
                    <img src="Images/Task.png" height="50px" alt="" />
                </div>
                <div class="blackRibbonDescription">
                    <span id="readOnlyNameAssignments" class="NameText"><%=Me.Language.Translate("CaptionGrid", Me.DefaultScope)%></span>
                </div>
                <div id="tbButtons" runat="server" class="blackRibbonButtons" style="padding-top: 25px">
                </div>
            </div>
            <div style="min-height: 10px"></div>
        </div>
        <!-- FIN TAB SUPERIOR -->
        <!-- DETALLE -->
        <div id="divTabData" class="divDataCells">
            <div id="divContenido" class="divAllContent">
                <div id="divContent" style="height: initial;" class="maxHeight">
                    <div id="divGridTasks" runat="server" style="text-align: left; vertical-align: top; padding: 0px; height: 95%; display: block;">
                        <div style="width: 100%; height: 100%; padding: 0px;">
                            <div class="RoundCornerFrame roundCorner">
                                <dx:ASPxGridView ID="GridTareas" runat="server" AutoGenerateColumns="False" Width="100%" Cursor="pointer" DataSourceID="LinqServerModeDataSource1" ClientInstanceName="GridTareas">
                                    <ClientSideEvents Init="modifyHeaders" EndCallback="modifyHeaders" CustomButtonClick="GridTareasClientCustomButton_Click" />
                                    <Templates>
                                        <TitlePanel>
                                            <div class="RoundCornerFrame" style="float: left; height: 34px; width: 167px; border: 1px solid #595959;">
                                                <table>
                                                    <tr>
                                                        <td>
                                                            <dx:ASPxButton ID="btnShowPendiente" runat="server" AutoPostBack="False" CausesValidation="False"
                                                                Image-Url="Images/Pendiente.png" GroupName="Filter1" Width="39px" Height="30px" OnInit="btnShowPendiente_Init">
                                                                <FocusRectBorder BorderStyle="Groove" BorderWidth="1px" BorderColor="ActiveBorder" />
                                                                <Image Height="19px" Width="19px" />
                                                                <Paddings PaddingTop="3px" />
                                                                <ClientSideEvents Click="function(s, e) { FilterTasksByState(s,e,0); }" />
                                                                <Image Url="Images/Pendiente.png" ToolTip="Activa">
                                                                </Image>
                                                            </dx:ASPxButton>
                                                        </td>
                                                        <td>
                                                            <dx:ASPxButton ID="btnShowEnded" runat="server" AutoPostBack="False" CausesValidation="False"
                                                                Image-Url="Images/Finalizada.png" GroupName="Filter2" Width="39px" Height="30px" OnInit="btnShowEnded_Init">
                                                                <Image Height="19px" Width="19px" />
                                                                <Paddings PaddingTop="3px" />
                                                                <FocusRectBorder BorderStyle="Groove" BorderWidth="1px" BorderColor="ActiveBorder" />
                                                                <ClientSideEvents Click="function(s, e) { FilterTasksByState(s,e,1); }" />
                                                                <Image Url="Images/Finalizada.png" ToolTip="Finalizada">
                                                                </Image>
                                                            </dx:ASPxButton>
                                                        </td>
                                                        <td>
                                                            <dx:ASPxButton ID="btnShowCancelled" runat="server" AutoPostBack="False" CausesValidation="False"
                                                                Image-Url="Images/Cancelada.png" GroupName="Filter3" Width="39px" Height="30px" OnInit="btnShowCancelled_Init">
                                                                <Image Height="19px" Width="19px" />
                                                                <Paddings PaddingTop="3px" />
                                                                <FocusRectBorder BorderStyle="Groove" BorderWidth="1px" BorderColor="ActiveBorder" />
                                                                <ClientSideEvents Click="function(s, e) { FilterTasksByState(s,e,2); }" />
                                                                <Image Url="Images/Cancelada.png" ToolTip="Cancelada">
                                                                </Image>
                                                            </dx:ASPxButton>
                                                        </td>
                                                        <td>
                                                            <dx:ASPxButton ID="btnShowConfirmation" runat="server" AutoPostBack="False" CausesValidation="False"
                                                                Image-Url="Images/PendienteConfirmacion.png" GroupName="Filter4" Width="39px" Height="30px" OnInit="btnShowConfirmation_Init">
                                                                <Image Height="19px" Width="19px" />
                                                                <Paddings PaddingTop="3px" />
                                                                <FocusRectBorder BorderStyle="Groove" BorderWidth="1px" BorderColor="ActiveBorder" />
                                                                <ClientSideEvents Click="function(s, e) { FilterTasksByState(s,e,3); }" />
                                                                <Image Url="Images/PendienteConfirmacion.png" ToolTip="Pendiente confirmación">
                                                                </Image>
                                                            </dx:ASPxButton>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>
                                            <div class="RoundCornerFrame" style="float: right; padding: 2px; border: 1px solid #595959;">
                                                <dx:ASPxButton ID="ASPxbtnExportGrid" runat="server" AutoPostBack="true" CausesValidation="False"
                                                    Image-Url="Images/ExportToExcel.png" OnClick="ASPxbtnExportGrid_Click" Width="39px" Height="30px">
                                                    <Image Height="19px" Width="19px" />
                                                    <Paddings PaddingTop="3px" />
                                                    <FocusRectBorder BorderStyle="Groove" BorderWidth="1px" BorderColor="ActiveBorder" />
                                                </dx:ASPxButton>
                                            </div>
                                        </TitlePanel>
                                    </Templates>

                                    <SettingsLoadingPanel Text="Cargando&amp;hellip;"></SettingsLoadingPanel>

                                    <SettingsFilterControl ViewMode="Visual" ShowAllDataSourceColumns="True" MaxHierarchyDepth="1" />
                                    <Settings ShowFilterBar="Visible" />
                                    <Settings ShowTitlePanel="True" />
                                </dx:ASPxGridView>
                                <dx:LinqServerModeDataSource ID="LinqServerModeDataSource1" runat="server" />
                            </div>
                        </div>
                    </div>

                    <div id="taskDetails" style="display: none">
                        <div id="divMenuTask" runat="server" style="width: 90%; margin-top: 10px; margin-left: auto; margin-right: auto; padding: 4px;">
                            <table border="0" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td>
                                        <div id="tdBackToGrid" style="padding: 2px; margin-right: 5px;"
                                            class="RoundCornerFrame">
                                            <dx:ASPxButton ID="ASPxButton2" runat="server" AutoPostBack="False" CausesValidation="False"
                                                Image-Url="Images/BackToGridSmall.png">
                                                <ClientSideEvents Click="function(s, e) { BackToGrid(); }" />
                                                <FocusRectBorder BorderStyle="Groove" BorderWidth="1px" BorderColor="ActiveBorder" />
                                            </dx:ASPxButton>
                                        </div>
                                    </td>
                                    <td>
                                        <div class="RoundCornerFrame">
                                            <table>
                                                <tr>
                                                    <td>
                                                        <a id="TABBUTTON_00" href="javascript: void(0);" class="bTab-active" onclick="javascript: changeTabs(0);">
                                                            <%=Me.Language.Translate("tabEstado", Me.DefaultScope)%></a>
                                                    </td>
                                                    <td>
                                                        <a id="TABBUTTON_01" href="javascript: void(0);" class="bTab" onclick="javascript: changeTabs(1);">
                                                            <%=Me.Language.Translate("tabGeneral", Me.DefaultScope)%></a>
                                                    </td>
                                                    <td>
                                                        <a id="TABBUTTON_02" href="javascript: void(0);" class="bTab" onclick="javascript: changeTabs(2);">
                                                            <%=Me.Language.Translate("tabTeorico", Me.DefaultScope)%></a>
                                                    </td>
                                                    <td>
                                                        <a id="TABBUTTON_03" href="javascript: void(0);" class="bTab" onclick="javascript: changeTabs(3);">
                                                            <%=Me.Language.Translate("tabAutorizados", Me.DefaultScope)%></a>
                                                    </td>
                                                    <td>
                                                        <a id="TABBUTTON_04" href="javascript: void(0);" class="bTab" onclick="javascript: changeTabs(4);">
                                                            <%=Me.Language.Translate("tabFicha", Me.DefaultScope)%></a>
                                                    </td>
                                                    <td>
                                                        <a id="TABBUTTON_05" href="javascript: void(0);" class="bTab" onclick="javascript: changeTabs(5);">
                                                            <%=Me.Language.Translate("tabAlertas", Me.DefaultScope)%></a>
                                                    </td>
                                                    <td>
                                                        <a id="TABBUTTON_06" runat="server" href="javascript: void(0);" class="bTab" onclick="javascript: changeTabs(6);">
                                                            <%=Me.Language.Translate("tabPuestos", Me.DefaultScope)%></a>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </div>

                        <dx:ASPxCallbackPanel ID="ASPxCallbackpanelDetailsTask" runat="server" ClientInstanceName="ASPxCallbackpanelDetailsTaskClient" CssClass="defaultContrastColor"
                            Style="overflow: auto; min-height: 570px; width: 90%; height: 85%; min-width: 1100px; margin-top: 5px; vertical-align: top; margin-left: auto; margin-right: auto;">
                            <SettingsLoadingPanel Enabled="false" />
                            <ClientSideEvents CallbackError="function(s,e){ alert('error'); }" EndCallback="ASPxCallbackpanelDetailsTaskClient_EndCallBack" />
                            <PanelCollection>
                                <dx:PanelContent ID="PanelContent1" runat="server">
                                    <div id="divMsgTop" class="divMsg" style="display: none; width: calc(100% - 2px) !important">
                                        <table style="width: 100%;">
                                            <tr>
                                                <td align="center" style="width: 20px; height: 16px;">
                                                    <img id="Img1" src="~/Base/Images/MessageFrame/Alert16.png" runat="server" /></td>
                                                <td align="left" style="padding-left: 10px; color: white;"><span id="msgTop"></span></td>
                                                <td align="right" style="color: White; padding-right: 10px;">
                                                    <a href="javascript: void(0);" class="aMsg" onclick="saveChanges();"><span id="lblSaveChanges" runat="server" /></a>
                                                    &nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;
                                                    <a href="javascript: void(0);" onclick="undoChanges();" class="aMsg"><span id="lblUndoChanges" runat="server" /></a>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>

                                    <!-- Tab Panel 0 Estado -->
                                    <dx:ASPxPanel ID="panel00" runat="server" ClientInstanceName="panel00" Style="height: 85%; width: 98%;"
                                        Paddings-PaddingTop="5px" Paddings-PaddingLeft="15px">
                                        <PanelCollection>
                                            <dx:PanelContent ID="PanelContent4" runat="server">
                                                <div class="labelInfoBig">
                                                    <asp:Label ID="lblInfoPpalEstado" runat="server" Text="Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."></asp:Label>
                                                </div>
                                                <div class="panHeader2">
                                                    <span style="">
                                                        <asp:Label runat="server" ID="lblEstadoTitle" Text="Estado"></asp:Label></span>
                                                </div>
                                                <table style="padding-left: 40px; width: 100%;">
                                                    <tr>
                                                        <td style="width: 550px;">
                                                            <table>
                                                                <tr>
                                                                    <td>
                                                                        <asp:Label ID="lblLineProgreso" runat="server" Text="Progreso:" class="spanEmp-Class"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td width="600px;">
                                                                        <div id="divProgreso" style="height: 40px; width: 500px; border: 1px solid #2D4155; text-align: center;"
                                                                            class="RoundCornerFrame">
                                                                            <div id="divProgresoIn" class="RoundCornerFrame contenedor" style="float: left; width: 0px; height: 39px; text-align: center; border-right-style: solid; border-right-width: thin; border-right-color: #595959; border-bottom-style: solid; border-bottom-width: thin; border-bottom-color: #595959; border-top-style: solid; border-top-width: thin; border-top-color: #595959; font-weight: bold; font-size: 12px;">
                                                                            </div>
                                                                            <div id="divProgresoIn2" style="display: none; float: left; height: 39px; text-align: left; padding-top: 12px; margin-left: 4px; font-weight: bold; font-size: 12px;">
                                                                            </div>
                                                                        </div>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                        <td>
                                                            <table style="width: 100%;">
                                                                <tr>
                                                                    <td id="trTaskInfo">
                                                                        <img id="icoTaskInfo" runat="server" alt="" src="Images/Info.png" style="float: left; margin-right: 5px; margin-top: 3px;" />
                                                                        <asp:Label ID="lblTaskInfo" runat="server" Text="" class="spanEmp-Class"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td id="trTaskInfo2">
                                                                        <img id="icoTaskInfo2" runat="server" alt="" src="Images/Info.png" style="float: left; margin-right: 5px; margin-top: 3px;" />
                                                                        <asp:Label ID="lblTaskInfo2" runat="server" Text="" class="spanEmp-Class"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <img id="icoTaskInfo3" runat="server" alt="" src="Images/Info.png" style="float: left; margin-right: 5px; margin-top: 3px;" />
                                                                        <asp:Label ID="lblTaskInfo3" runat="server" Text="" class="spanEmp-Class"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <table>
                                                                            <tr>
                                                                                <td style="width: 280px;">
                                                                                    <img id="Img2" runat="server" alt="" src="Images/Info.png" style="float: left; margin-right: 5px; margin-top: 3px;" />
                                                                                    <asp:Label ID="lblTaskInfoState" runat="server" Text="La tarea se encuentra actualmente activa"
                                                                                        class="spanEmp-Class"></asp:Label>
                                                                                </td>
                                                                                <td>
                                                                                    <table>
                                                                                        <tr>
                                                                                            <td style="width: 110px;">
                                                                                                <dx:ASPxButton ID="ASPxbtnSetActive" runat="server" AutoPostBack="False" CausesValidation="False"
                                                                                                    ClientInstanceName="ASPxbtnSetActiveCli" Image-Url="Images/Pendiente.png" GroupName="Status"
                                                                                                    Text="Activa" title="Pendiente" Width="100">
                                                                                                    <FocusRectBorder BorderStyle="Groove" BorderWidth="1px" BorderColor="ActiveBorder" />
                                                                                                </dx:ASPxButton>
                                                                                            </td>
                                                                                            <td style="width: 110px;">
                                                                                                <dx:ASPxButton ID="ASPxbtnSetEnded" runat="server" AutoPostBack="False" CausesValidation="False"
                                                                                                    ClientInstanceName="ASPxbtnSetEndedCli" Image-Url="Images/Finalizada.png" GroupName="Status"
                                                                                                    Text="Finalizada" title="Finalizada" Width="100">
                                                                                                    <FocusRectBorder BorderStyle="Groove" BorderWidth="1px" BorderColor="ActiveBorder" />
                                                                                                </dx:ASPxButton>
                                                                                            </td>
                                                                                            <td style="width: 110px;">
                                                                                                <dx:ASPxButton ID="ASPxbtnSetCanceled" runat="server" AutoPostBack="False" CausesValidation="False"
                                                                                                    ClientInstanceName="ASPxbtnSetCanceledCli" Image-Url="Images/Cancelada.png" GroupName="Status"
                                                                                                    Text="Cancelada" title="Cancelada" Width="100">
                                                                                                    <FocusRectBorder BorderStyle="Groove" BorderWidth="1px" BorderColor="ActiveBorder" />
                                                                                                </dx:ASPxButton>
                                                                                            </td>
                                                                                            <td style="width: 110px;">
                                                                                                <dx:ASPxButton ID="ASPxbtnSetPendienteVal" runat="server" AutoPostBack="False" CausesValidation="False"
                                                                                                    ClientInstanceName="ASPxbtnSetWaitingConf" Image-Url="Images/PendienteConfirmacion.png" GroupName="Status"
                                                                                                    Text="Pendiente" title="Pendiente" Width="100">
                                                                                                    <FocusRectBorder BorderStyle="Groove" BorderWidth="1px" BorderColor="ActiveBorder" />
                                                                                                </dx:ASPxButton>
                                                                                            </td>
                                                                                        </tr>
                                                                                    </table>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <table>
                                                                            <tr>
                                                                                <td style="width: 350px;">
                                                                                    <img id="icoTaskClosedInfo" runat="server" alt="" src="Images/Info.png" style="float: left; margin-right: 5px; margin-top: 3px;" />
                                                                                    <asp:Label ID="lblTaskClosedInfo" runat="server" Text="" class="spanEmp-Class"></asp:Label>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <div class="panHeader2">
                                                    <span style="">
                                                        <asp:Label runat="server" ID="Label1" Text="Tiempos"></asp:Label></span>
                                                </div>

                                                <table cellpadding="2" cellspacing="2" border="0" style="margin-top: 15px;">
                                                    <tr>
                                                        <td align="right" width="150px" style="padding-right: 10px;">
                                                            <asp:Label ID="lblTimeChangedRequirements" runat="server" Text="Cambio requisitos:"
                                                                class="spanEmp-Class"></asp:Label>
                                                        </td>
                                                        <td align="left" width="55px">
                                                            <dx:ASPxTextBox runat="server" ID="txtTimeChangedRequirements" MaxLength="7" Width="70px" ClientInstanceName="txtTimeChangedRequirementsClient">
                                                                <MaskSettings Mask="<-999..999>:<00..59>" />
                                                                <ClientSideEvents TextChanged="function(s,e){ refreshTotalDesviations();hasChanges(true);}" />
                                                                <ValidationSettings ErrorDisplayMode="None" />
                                                            </dx:ASPxTextBox>
                                                        </td>
                                                        <td align="right" width="50px" style="padding-right: 10px;">
                                                            <asp:Label ID="lblEmployeeTime" runat="server" Text="Personal:" class="spanEmp-Class"></asp:Label>
                                                        </td>
                                                        <td align="left" width="75px">
                                                            <dx:ASPxTextBox runat="server" ID="txtEmployeeTime" MaxLength="7" Width="70px" ClientInstanceName="txtEmployeeTimeClient">
                                                                <MaskSettings Mask="<-999..999>:<00..59>" />
                                                                <ClientSideEvents TextChanged="function(s,e){ refreshTotalDesviations();hasChanges(true);}" />
                                                                <ValidationSettings ErrorDisplayMode="None" />
                                                            </dx:ASPxTextBox>
                                                        </td>
                                                        <td align="right" width="50px" style="padding-right: 10px;">
                                                            <asp:Label ID="lblMaterialTime" runat="server" Text="Material:" class="spanEmp-Class"></asp:Label>
                                                        </td>
                                                        <td align="left" style="width: 75px">
                                                            <dx:ASPxTextBox runat="server" ID="txtMaterialTime" MaxLength="7" Width="70px" ClientInstanceName="txtMaterialTimeClient">
                                                                <MaskSettings Mask="<-999..999>:<00..59>" />
                                                                <ClientSideEvents TextChanged="function(s,e){ refreshTotalDesviations();hasChanges(true);}" />
                                                                <ValidationSettings ErrorDisplayMode="None" />
                                                            </dx:ASPxTextBox>
                                                        </td>
                                                        <td align="right" style="padding-right: 10px;">
                                                            <asp:Label ID="lblForecastErrorTime" runat="server" Text="Error previsión:" class="spanEmp-Class"></asp:Label>
                                                        </td>
                                                        <td align="left">
                                                            <dx:ASPxTextBox runat="server" ID="txtForecastErrorTime" MaxLength="7" Width="70px" ClientInstanceName="txtForecastErrorTimeClient">
                                                                <MaskSettings Mask="<-999..999>:<00..59>" />
                                                                <ClientSideEvents TextChanged="function(s,e){ refreshTotalDesviations();hasChanges(true);}" />
                                                                <ValidationSettings ErrorDisplayMode="None" />
                                                            </dx:ASPxTextBox>
                                                        </td>
                                                        <td align="right" style="padding-right: 10px;">
                                                            <asp:Label ID="lblTeamTime" runat="server" Text="Equipo:" class="spanEmp-Class"></asp:Label>
                                                        </td>
                                                        <td align="left">
                                                            <dx:ASPxTextBox runat="server" ID="txtTeamTime" MaxLength="7" Width="70px" ClientInstanceName="txtTeamTimeClient">
                                                                <MaskSettings Mask="<-999..999>:<00..59>" />
                                                                <ClientSideEvents TextChanged="function(s,e){ refreshTotalDesviations();hasChanges(true);}" />
                                                                <ValidationSettings ErrorDisplayMode="None" />
                                                            </dx:ASPxTextBox>
                                                        </td>
                                                        <td align="right" style="padding-right: 10px;">
                                                            <asp:Label ID="lblOtherTime" runat="server" Text="Otros:" class="spanEmp-Class"></asp:Label>
                                                        </td>
                                                        <td align="left">
                                                            <dx:ASPxTextBox runat="server" ID="txtOtherTime" MaxLength="7" Width="70px" ClientInstanceName="txtOtherTimeClient">
                                                                <MaskSettings Mask="<-999..999>:<00..59>" />
                                                                <ClientSideEvents TextChanged="function(s,e){ refreshTotalDesviations();hasChanges(true);}" />
                                                                <ValidationSettings ErrorDisplayMode="None" />
                                                            </dx:ASPxTextBox>
                                                        </td>
                                                        <td align="right" style="padding-right: 10px;">
                                                            <asp:Label ID="lblNonProductiveTimeIncidence" runat="server" Text="Incidencias no previstas:"
                                                                class="spanEmp-Class"></asp:Label>
                                                        </td>
                                                        <td align="left">
                                                            <dx:ASPxTextBox runat="server" ID="txtNonProductiveTimeIncidence" MaxLength="7" Width="70px" ClientInstanceName="txtNonProductiveTimeIncidenceClient">
                                                                <MaskSettings Mask="<-999..999>:<00..59>" />
                                                                <ClientSideEvents TextChanged="function(s,e){ refreshTotalDesviations();hasChanges(true);}" />
                                                                <ValidationSettings ErrorDisplayMode="None" />
                                                            </dx:ASPxTextBox>
                                                        </td>
                                                        <td align="right" style="font-weight: bold; padding-right: 10px;">
                                                            <asp:Label ID="lblTotalDesvios" runat="server" Text="Total:" class="spanEmp-Class"></asp:Label>
                                                        </td>
                                                        <td align="left">
                                                            <dx:ASPxTextBox runat="server" ID="txtTotalDesviationHours" MaxLength="8" Width="80px" ReadOnly="true" ClientInstanceName="txtTotalDesviationHoursClient">
                                                                <MaskSettings Mask="<-9999..99999>:<00..59>" />
                                                                <ValidationSettings ErrorDisplayMode="None" />
                                                            </dx:ASPxTextBox>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <br />
                                                <table cellpadding="2" cellspacing="2" border="0" style="width: 95%;">
                                                    <tr>
                                                        <td align="left" width="80px" valign="top" style="padding-left: 10px;">
                                                            <asp:Label ID="lblTimeRemarks" runat="server" Text="Comentarios:" class="spanEmp-Class"></asp:Label>
                                                        </td>
                                                        <td align="left" valign="top">
                                                            <%--<textarea id="txtTimeRemarks" runat="server" rows="5" style="width: 98%; height: 30px;"
                                                            class="textClass x-form-text x-form-field" convertcontrol="TextArea" ccallowblank="true"></textarea>--%>
                                                            <dx:ASPxMemo ID="txtTimeRemarks" runat="server" Rows="2" Width="98%" Height="30">
                                                                <ClientSideEvents TextChanged="function(s,e){ hasChanges(true)}" />
                                                            </dx:ASPxMemo>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <br />
                                                <div class="panHeader2">
                                                    <span style="">
                                                        <asp:Label runat="server" ID="lblEstadisticasTitle" Text="Estadísticas"></asp:Label>
                                                    </span>
                                                </div>
                                                <table border="0" cellpadding="2" cellspacing="2" style="padding-left: 40px;">
                                                    <tr>
                                                        <td style="padding-right: 10px;">
                                                            <asp:Label ID="lblView" runat="server" Text="Vista:" class="spanEmp-Class"></asp:Label>
                                                        </td>
                                                        <td width="180px">
                                                            <dx:ASPxComboBox ID="cmbView" runat="server" Width="200px" Font-Size="11px" CssClass="editTextFormat"
                                                                Font-Names="Arial;Verdana;Sans-Serif" IncrementalFilteringMode="Contains" ClientInstanceName="cmbViewClient">
                                                                <ClientSideEvents SelectedIndexChanged="function(s,e){ UpdateStatisticsView(s.GetSelectedIndex()); }" />
                                                            </dx:ASPxComboBox>
                                                        </td>
                                                        <td id="tdlabelGroupBy" style="padding-right: 10px;">
                                                            <asp:Label ID="lblGroupBy" runat="server" Text="Agrupado por:" class="spanEmp-Class"></asp:Label>
                                                        </td>
                                                        <td id="tdcmbGroupBy" width="150px">
                                                            <dx:ASPxComboBox ID="cmbGroupBy" runat="server" Width="125px" Font-Size="11px" CssClass="editTextFormat"
                                                                Font-Names="Arial;Verdana;Sans-Serif" IncrementalFilteringMode="Contains" ClientInstanceName="cmbGroupByClient">
                                                                <ClientSideEvents SelectedIndexChanged="function(s,e){ UpdateStatisticsGroupBy(s.GetSelectedIndex()); }" />
                                                            </dx:ASPxComboBox>
                                                        </td>
                                                        <td width="300px">
                                                            <table id="tbGroupbyDates" style="display: none;">
                                                                <tr>
                                                                    <td style="width: 100px;">
                                                                        <dx:ASPxDateEdit runat="server" ID="txtGroupByDateInf" Width="100" ClientInstanceName="txtGroupByDateInfClient">
                                                                            <CalendarProperties ShowClearButton="false" />
                                                                            <ClientSideEvents DateChanged="function(s,e){ UpdateStatisticsGroupBy(1)}" />
                                                                        </dx:ASPxDateEdit>
                                                                    </td>
                                                                    <td style="width: 40px;">
                                                                        <asp:Label ID="lblTo" runat="server" Text="hasta" class="spanEmp-Class"></asp:Label>
                                                                    </td>
                                                                    <td style="width: 100px;">
                                                                        <dx:ASPxDateEdit runat="server" ID="txtGroupByDateSup" Width="100" ClientInstanceName="txtGroupByDateSupClient">
                                                                            <CalendarProperties ShowClearButton="false" />
                                                                            <ClientSideEvents DateChanged="function(s,e){ UpdateStatisticsGroupBy(1)}" />
                                                                        </dx:ASPxDateEdit>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <table border="0" cellpadding="2" cellspacing="2" style="padding-left: 40px;">
                                                    <tr>
                                                        <td valign="top">
                                                            <div id="grdEstadisticas" class="statsGrid">
                                                            </div>
                                                        </td>
                                                        <td valign="top">
                                                            <div id="divGraf" style="overflow: auto; height: 230px; margin-left: 1px; width: 550px; margin-top: 9px;">
                                                                <dx:ASPxCallbackPanel ID="ASPxCallbackPanelGraf" runat="server" Width="1px" ClientInstanceName="pnlTaskGraf">
                                                                    <ClientSideEvents EndCallback="pnlTaskGraf_EndCallBack" />
                                                                    <PanelCollection>
                                                                        <dx:PanelContent ID="PanelContent5" runat="server">
                                                                            <dxchartsweb:WebChartControl ID="Grafico" runat="server" Height="230px" Width="500px">
                                                                            </dxchartsweb:WebChartControl>
                                                                        </dx:PanelContent>
                                                                    </PanelCollection>
                                                                </dx:ASPxCallbackPanel>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </dx:PanelContent>
                                        </PanelCollection>
                                    </dx:ASPxPanel>

                                    <!-- Tab Panel 1 General -->
                                    <dx:ASPxPanel ID="panel01" runat="server" ClientInstanceName="panel01" Style="height: 85%; width: 98%;"
                                        Paddings-PaddingTop="15px" Paddings-PaddingLeft="15px">
                                        <PanelCollection>
                                            <dx:PanelContent ID="PanelContent7" runat="server">
                                                <div class="panHeader2">
                                                    <span style="">
                                                        <asp:Label runat="server" ID="lblGeneralTitle" Text="General"></asp:Label></span>
                                                </div>
                                                <br />
                                                <table width="100%" border="0" cellpadding="2" cellspacing="2">
                                                    <tr>
                                                        <td width="150px" align="right" style="padding-right: 10px;">
                                                            <asp:Label ID="lblName" runat="server" Text="Nombre:" class="spanEmp-Class"></asp:Label>
                                                        </td>
                                                        <td align="left">
                                                            <dx:ASPxMemo ID="txtName" runat="server" Rows="2" Width="500" Height="25" MaxLength="70" ClientInstanceName="txtNameClient">
                                                                <ClientSideEvents TextChanged="function(s,e){ hasChanges(true); updateProjectName();}" />
                                                            </dx:ASPxMemo>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td width="150px" align="right" style="padding-right: 10px;">
                                                            <asp:Label ID="lblProject" runat="server" Text="Proyecto:" class="spanEmp-Class"></asp:Label>
                                                        </td>
                                                        <td align="left">
                                                            <dx:ASPxMemo ID="txtProject" runat="server" Rows="2" Width="500" Height="25" MaxLength="70" ClientInstanceName="txtProjectClient">
                                                                <ClientSideEvents TextChanged="function(s,e){ hasChanges(true); updateProjectName();}" />
                                                            </dx:ASPxMemo>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td width="150px" align="right" style="padding-right: 10px;">
                                                            <asp:Label ID="lblCenter" runat="server" Text="Centro de coste:" class="spanEmp-Class"></asp:Label>
                                                        </td>
                                                        <td>
                                                            <dx:ASPxComboBox ID="cmbGroup" runat="server" Width="270px" Font-Size="11px" CssClass="editTextFormat"
                                                                Font-Names="Arial;Verdana;Sans-Serif" IncrementalFilteringMode="Contains" ClientInstanceName="cmbGroupClient">
                                                                <ClientSideEvents TextChanged="function(s,e){ hasChanges(true)}" />
                                                            </dx:ASPxComboBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td width="150px" align="right" style="padding-right: 10px;">
                                                            <asp:Label ID="lblShortName" runat="server" Text="Nombre abreviado:" class="spanEmp-Class"></asp:Label>
                                                        </td>
                                                        <td align="left">
                                                            <dx:ASPxMemo ID="txtShortName" runat="server" Rows="2" Width="45" Height="25" MaxLength="4">
                                                                <ClientSideEvents TextChanged="function(s,e){ hasChanges(true)}" />
                                                            </dx:ASPxMemo>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td width="150px" align="right" style="padding-right: 10px;">
                                                            <asp:Label ID="lblColorDesc" runat="server" Text="Color identificativo:" class="spanEmp-Class"></asp:Label>
                                                        </td>
                                                        <td align="left">
                                                            <dx:ASPxColorEdit ID="dxColorPicker" runat="server" ClientInstanceName="dxColorPickerClient" EnableCustomColors="true" Width="14px">
                                                                <ClientSideEvents ColorChanged="function(s,e){s.GetInputElement().style.display = 'none'; hasChanges(true)}" />
                                                            </dx:ASPxColorEdit>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td width="150px" align="right" valign="top" style="padding-right: 10px;">
                                                            <asp:Label ID="lblDescription" runat="server" Text="Descripción:" class="spanEmp-Class"></asp:Label>
                                                        </td>
                                                        <td align="left" valign="top" style="padding-right: 30px;">
                                                            <dx:ASPxMemo ID="txtDescription" runat="server" Rows="2" Width="500" Height="120">
                                                                <ClientSideEvents TextChanged="function(s,e){ hasChanges(true)}" />
                                                            </dx:ASPxMemo>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td width="150px" align="right" style="padding-right: 10px;">
                                                            <asp:Label ID="lblTag" runat="server" Text="Tags:" class="spanEmp-Class"></asp:Label>
                                                        </td>
                                                        <td align="left">
                                                            <dx:ASPxMemo ID="txtTag" runat="server" Rows="2" Width="500" Height="25" MaxLength="70">
                                                                <ClientSideEvents TextChanged="function(s,e){ hasChanges(true)}" />
                                                            </dx:ASPxMemo>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td width="150px" align="right" style="padding-right: 10px;">
                                                            <asp:Label ID="lblBarCode" runat="server" Text="Código de Barras:" class="spanEmp-Class"></asp:Label>
                                                        </td>
                                                        <td align="left">
                                                            <dx:ASPxMemo ID="txtBarCode" runat="server" Rows="2" Width="500" Height="25" MaxLength="50">
                                                                <ClientSideEvents TextChanged="function(s,e){ hasChanges(true)}" />
                                                            </dx:ASPxMemo>
                                                        </td>
                                                    </tr>

                                                    <tr style="height: 80px; padding-top: 5px;">
                                                        <td width="150px" align="right" style="padding-right: 10px;">
                                                            <asp:Label ID="lblPriority" runat="server" Text="Prioridad:" class="spanEmp-Class"></asp:Label>
                                                        </td>
                                                        <td align="left" style="padding-left: 10px;">
                                                            <input type="text" runat="server" id="txtPriority" class="textClass x-form-text x-form-field"
                                                                maxlength="3" style="display: none; width: 25px;" convertcontrol="TextField"
                                                                ccallowblank="true" />
                                                            <dx:ASPxTrackBar ID="TrackBarPriority" runat="server" ScalePosition="LeftOrTop" MinValue="0"
                                                                MaxValue="20" Step="1" Width="500px" LargeTickInterval="1" SmallTickFrequency="1"
                                                                ClientInstanceName="TrackBarPriority" DragHandleToolTip="" IncrementButtonToolTip=""
                                                                DecrementButtonToolTip="">
                                                                <ClientSideEvents ValueChanged="function(s, e) { SetTrackbarValue(s,e); hasChanges(true); }" />
                                                            </dx:ASPxTrackBar>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </dx:PanelContent>
                                        </PanelCollection>
                                    </dx:ASPxPanel>

                                    <!-- Tab2 Panel 2Teoricos-->
                                    <dx:ASPxPanel ID="panel02" runat="server" ClientInstanceName="panel02" Style="height: 85%; width: 98%;"
                                        Paddings-PaddingTop="15px" Paddings-PaddingLeft="15px">
                                        <PanelCollection>
                                            <dx:PanelContent ID="PanelContent9" runat="server">
                                                <div class="labelInfoBig">
                                                    <asp:Label ID="lblInfoPpalDatosTeo" runat="server" Text="Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."></asp:Label>
                                                </div>
                                                <table>
                                                    <tr>
                                                        <td valign="top" width="600px">
                                                            <div class="panHeader2">
                                                                <span style="">
                                                                    <asp:Label runat="server" ID="lblPrevisionTitle" Text="Previsión"></asp:Label></span>
                                                            </div>
                                                            <div style="height: 300px; width: 99%; margin-top: 1px; padding-top: 10px; border: 1px solid #CDCDCD">
                                                                <div class="labelInfoSmall">
                                                                    <asp:Label ID="lblInfoDatosTeo1" runat="server" Text="Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam."></asp:Label>
                                                                </div>
                                                                <table cellpadding="0" cellspacing="0" border="0" style="width: 100%; height: 80px; padding-right: 10px;">
                                                                    <tr style="height: 40px;">
                                                                        <td align="right" width="450px" style="padding-right: 10px;">
                                                                            <asp:Label ID="lblFechaIniPrev" runat="server" Text="Fecha y hora de inicio prevista:"
                                                                                class="spanEmp-Class"></asp:Label>
                                                                        </td>
                                                                        <td align="left" width="100px">
                                                                            <dx:ASPxDateEdit runat="server" ID="txtExpectedStartDate" Width="100px" ClientInstanceName="txtExpectedStartDateClient" AllowNull="true">
                                                                                <CalendarProperties ShowClearButton="false" />
                                                                                <ClientSideEvents DateChanged="function(s,e){ hasChanges(true)}" />
                                                                                <ValidationSettings ErrorDisplayMode="None" />
                                                                            </dx:ASPxDateEdit>
                                                                        </td>
                                                                        <td align="left" width="100px">
                                                                            <dx:ASPxTextBox runat="server" ID="txtExpectedStartTime" MaxLength="5" Width="50px" ClientInstanceName="txtExpectedStartTimeClient">
                                                                                <MaskSettings Mask="<00..23>:<00..59>" />
                                                                                <ClientSideEvents TextChanged="function(s,e){ hasChanges(true)}" />
                                                                                <ValidationSettings ErrorDisplayMode="None" />
                                                                            </dx:ASPxTextBox>
                                                                        </td>
                                                                    </tr>
                                                                    <tr style="height: 40px;">
                                                                        <td align="right" width="450px" style="padding-right: 10px;">
                                                                            <asp:Label ID="lblFechaFinPrev" runat="server" Text="Fecha y hora de finalización prevista:"
                                                                                class="spanEmp-Class"></asp:Label>
                                                                        </td>
                                                                        <td align="left" width="100px">
                                                                            <dx:ASPxDateEdit runat="server" ID="txtExpectedEndDate" Width="100" ClientInstanceName="txtExpectedEndDateClient" AllowNull="true">
                                                                                <CalendarProperties ShowClearButton="false" />
                                                                                <ClientSideEvents DateChanged="function(s,e){ hasChanges(true)}" />
                                                                            </dx:ASPxDateEdit>
                                                                        </td>
                                                                        <td align="left" width="450px">
                                                                            <dx:ASPxTextBox runat="server" ID="txtExpectedEndTime" MaxLength="5" Width="50" ClientInstanceName="txtExpectedEndTimeClient">
                                                                                <MaskSettings Mask="<00..23>:<00..59>" />
                                                                                <ClientSideEvents TextChanged="function(s,e){ hasChanges(true)}" />
                                                                            </dx:ASPxTextBox>
                                                                        </td>
                                                                    </tr>
                                                                    <tr style="height: 40px;">
                                                                        <td align="right" width="100px" style="padding-right: 10px;">
                                                                            <asp:Label ID="lblInitialTime" runat="server" Text="Horas Iniciales:" class="spanEmp-Class"></asp:Label>
                                                                        </td>
                                                                        <td align="left" width="60px">
                                                                            <dx:ASPxTextBox runat="server" ID="txtInitialTime" MaxLength="7" Width="70" ClientInstanceName="txtInitialTimeClient">
                                                                                <MaskSettings Mask="<0..9999>:<00..59>" />
                                                                                <ClientSideEvents TextChanged="function(s,e){ hasChanges(true)}" />
                                                                                <ValidationSettings ErrorDisplayMode="None" />
                                                                            </dx:ASPxTextBox>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </div>
                                                        </td>
                                                        <td valign="top" width="600px">
                                                            <div class="panHeader2">
                                                                <span style="">
                                                                    <asp:Label runat="server" ID="lblCierre" Text="Cierre"></asp:Label></span>
                                                            </div>
                                                            <div style="height: 300px; width: 99%; margin-top: 1px; padding-top: 10px; border: 1px solid #CDCDCD">
                                                                <div class="labelInfoSmall">
                                                                    <asp:Label ID="lblInfoDatosTeo4" runat="server" Text="Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam."></asp:Label>
                                                                </div>
                                                                <table cellpadding="0" cellspacing="0" border="0" style="width: 100%; height: 150px; padding-right: 10px;">
                                                                    <tr>
                                                                        <td style="padding-left: 10px; height: 120px;" valign="top">
                                                                            <roUserControls:roOptionPanelClient ID="optClosingAllways" runat="server" TypeOPanel="RadioOption"
                                                                                Width="100%" Height="Auto" Checked="False" Enabled="True" Border="True" Value="0" CConClick="hasChanges(true)">
                                                                                <Title>
                                                                                    <asp:Label ID="lblClosingAllwaysTitle" runat="server" Text="Siempre"></asp:Label>
                                                                                </Title>
                                                                                <Description>
                                                                                    <asp:Label ID="lblClosingAllwaysDesc" runat="server" Text="La tarea siempre está disponible para su cierre."></asp:Label>
                                                                                </Description>
                                                                                <Content>
                                                                                </Content>
                                                                            </roUserControls:roOptionPanelClient>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td style="padding-left: 10px; height: 120px;" valign="top">
                                                                            <roUserControls:roOptionPanelClient ID="optClosingByDate" runat="server" TypeOPanel="RadioOption"
                                                                                Width="100%" Height="Auto" Checked="False" Enabled="True" Border="True" Value="1" CConClick="hasChanges(true)">
                                                                                <Title>
                                                                                    <asp:Label ID="lblClosingByDateTitle" runat="server" Text="Fecha y Hora"></asp:Label>
                                                                                </Title>
                                                                                <Description>
                                                                                </Description>
                                                                                <Content>
                                                                                    <table style="padding-left: 10px;">
                                                                                        <tr>
                                                                                            <td style="padding-right: 3px;" align="left">
                                                                                                <asp:Label ID="lblClosingByDateDesc" ForeColor="steelblue" runat="server" Text="Se cierra mediante una fecha y hora fija."></asp:Label>
                                                                                            </td>
                                                                                            <td align="left" width="100px">
                                                                                                <dx:ASPxDateEdit runat="server" ID="txtClosingDate" Width="100px" ClientInstanceName="txtClosingDateClient" AllowNull="true">
                                                                                                    <CalendarProperties ShowClearButton="false" />
                                                                                                    <ClientSideEvents DateChanged="function(s,e){ hasChanges(true)}" />
                                                                                                    <ValidationSettings ErrorDisplayMode="None" />
                                                                                                </dx:ASPxDateEdit>
                                                                                            </td>
                                                                                            <td align="left" width="70px">
                                                                                                <dx:ASPxTextBox runat="server" ID="txtClosingTime" MaxLength="5" Width="50px" ClientInstanceName="txtClosingTimeClient">
                                                                                                    <MaskSettings Mask="<00..23>:<00..59>" />
                                                                                                    <ClientSideEvents TextChanged="function(s,e){ hasChanges(true)}" />
                                                                                                    <ValidationSettings ErrorDisplayMode="None" />
                                                                                                </dx:ASPxTextBox>
                                                                                            </td>
                                                                                        </tr>
                                                                                    </table>
                                                                                </Content>
                                                                            </roUserControls:roOptionPanelClient>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <br />
                                                <table>
                                                    <tr>
                                                        <td valign="top" width="1200px">
                                                            <div class="panHeader2">
                                                                <span style="">
                                                                    <asp:Label runat="server" ID="lblActivacion" Text="Activación"></asp:Label></span>
                                                            </div>
                                                            <div style="height: 300px; width: 99%; margin-top: 1px; padding-top: 10px; border: 1px solid #CDCDCD">
                                                                <input type="hidden" id="hdnActivationTask" runat="server" value="0" />
                                                                <div class="labelInfoSmall">
                                                                    <asp:Label ID="lblInfoDatosTeo3" runat="server" Text="Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam."></asp:Label>
                                                                </div>
                                                                <table cellpadding="0" cellspacing="0" border="0" style="width: 100%; height: 150px; padding-right: 10px;">
                                                                    <tr>
                                                                        <td style="padding-left: 10px; height: 120px; width: 50%" valign="top">
                                                                            <roUserControls:roOptionPanelClient ID="optActivAllways" runat="server" TypeOPanel="RadioOption"
                                                                                Width="100%" Height="Auto" Checked="False" Enabled="True" Border="True" Value="0" CConClick="hasChanges(true)">
                                                                                <Title>
                                                                                    <asp:Label ID="lbloptActivAllwaysTitle" runat="server" Text="Siempre"></asp:Label>
                                                                                </Title>
                                                                                <Description>
                                                                                    <asp:Label ID="lbloptActivAllwaysDesc" runat="server" Text="La tarea siempre está disponible para su activación."></asp:Label>
                                                                                </Description>
                                                                                <Content>
                                                                                    <br />
                                                                                </Content>
                                                                            </roUserControls:roOptionPanelClient>
                                                                        </td>
                                                                        <td style="padding-left: 10px; height: 120px; width: 50%" valign="top">
                                                                            <roUserControls:roOptionPanelClient ID="optActivByDate" runat="server" TypeOPanel="RadioOption"
                                                                                Width="100%" Height="Auto" Checked="False" Enabled="True" Border="True" Value="1" CConClick="hasChanges(true)">
                                                                                <Title>
                                                                                    <asp:Label ID="lblActivByDateTitle" runat="server" Text="Fecha y Hora"></asp:Label>
                                                                                </Title>
                                                                                <Description>
                                                                                </Description>
                                                                                <Content>
                                                                                    <table style="padding-left: 10px;">
                                                                                        <tr>
                                                                                            <td style="padding-right: 3px; min-width: 150px" align="left">
                                                                                                <asp:Label ID="lblActivByDateDesc" ForeColor="steelblue" runat="server" Text="Se activa mediante una fecha y hora fija."></asp:Label>
                                                                                            </td>
                                                                                            <td align="left" style="width: 105px">
                                                                                                <dx:ASPxDateEdit runat="server" ID="txtActivationDate" Width="105px" ClientInstanceName="txtActivationDateClient" AllowNull="true">
                                                                                                    <CalendarProperties ShowClearButton="false" />
                                                                                                    <ClientSideEvents DateChanged="function(s,e){ hasChanges(true)}" />
                                                                                                </dx:ASPxDateEdit>
                                                                                            </td>
                                                                                            <td align="left">
                                                                                                <dx:ASPxTextBox runat="server" ID="txtActivationTime" Width="70px" MaxLength="5" ClientInstanceName="txtActivationTimeClient">
                                                                                                    <MaskSettings Mask="<00..23>:<00..59>" />
                                                                                                    <ClientSideEvents TextChanged="function(s,e){ hasChanges(true)}" />
                                                                                                    <ValidationSettings ErrorDisplayMode="None" />
                                                                                                </dx:ASPxTextBox>
                                                                                            </td>
                                                                                        </tr>
                                                                                    </table>
                                                                                </Content>
                                                                            </roUserControls:roOptionPanelClient>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td style="padding-left: 10px; height: 120px; width: 50%" valign="top">
                                                                            <roUserControls:roOptionPanelClient ID="optActivByEndTask" runat="server" TypeOPanel="RadioOption"
                                                                                Width="100%" Height="Auto" Checked="False" Enabled="True" Border="True" Value="2" CConClick="hasChanges(true)">
                                                                                <Title>
                                                                                    <asp:Label ID="lblActivByEndTaskTitle" runat="server" Text="Al finalizar una tarea"></asp:Label>
                                                                                </Title>
                                                                                <Description>
                                                                                    <asp:Label ID="lblActivByEndTaskDesc" runat="server" Text="Esta tarea se activará cuando finalice la tarea seleccionada:"></asp:Label>
                                                                                </Description>
                                                                                <Content>
                                                                                    <table style="padding-left: 22px;">
                                                                                        <tr>
                                                                                            <td>
                                                                                                <dx:ASPxTextBox ID="txtEndTask" ReadOnly="true" runat="server" Width="240px" CssClass="editTextFormat" ClientInstanceName="txtEndTaskClient"></dx:ASPxTextBox>
                                                                                            </td>
                                                                                            <td>
                                                                                                <a onclick="ShowTasksSelector(1);" title="Selector de Tareas" href="javascript: void(0);">
                                                                                                    <img alt="Selector de Tareas" src="Images/Task16.png" /></a>
                                                                                            </td>
                                                                                        </tr>
                                                                                    </table>
                                                                                </Content>
                                                                            </roUserControls:roOptionPanelClient>
                                                                        </td>
                                                                        <td style="padding-left: 10px; height: 120px; width: 50%" valign="top">
                                                                            <roUserControls:roOptionPanelClient ID="optActivByIniTask" runat="server" TypeOPanel="RadioOption"
                                                                                Width="100%" Height="Auto" Checked="False" Enabled="True" Border="True" Value="3" CConClick="hasChanges(true)">
                                                                                <Title>
                                                                                    <asp:Label ID="lblActivByIniTaskTitle" runat="server" Text="Al iniciar una tarea"></asp:Label>
                                                                                </Title>
                                                                                <Description>
                                                                                    <asp:Label ID="lblActivByIniTaskDesc" runat="server" Text="Esta tarea se activará cuando se inicie la tarea seleccionada:"></asp:Label>
                                                                                </Description>
                                                                                <Content>
                                                                                    <table style="padding-left: 22px;">
                                                                                        <tr>
                                                                                            <td>
                                                                                                <dx:ASPxTextBox ID="txtIniTask" ReadOnly="true" runat="server" Width="240px" CssClass="editTextFormat" ClientInstanceName="txtIniTaskClient"></dx:ASPxTextBox>
                                                                                            </td>
                                                                                            <td>
                                                                                                <a onclick="ShowTasksSelector(2);" title="Selector de Tareas" href="javascript: void(0);">
                                                                                                    <img alt="Selector de Tareas" src="Images/Task16.png" /></a>
                                                                                            </td>
                                                                                        </tr>
                                                                                    </table>
                                                                                </Content>
                                                                            </roUserControls:roOptionPanelClient>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </dx:PanelContent>
                                        </PanelCollection>
                                    </dx:ASPxPanel>

                                    <!-- Tab3 Panel Autorizados-->
                                    <dx:ASPxPanel ID="panel03" runat="server" ClientInstanceName="panel03" Style="height: 85%; width: 98%;"
                                        Paddings-PaddingTop="15px" Paddings-PaddingLeft="15px">
                                        <PanelCollection>
                                            <dx:PanelContent ID="PanelContent10" runat="server">
                                                <div class="labelInfoBig">
                                                    <asp:Label ID="lblInfoPpalColab" runat="server" Text="Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."></asp:Label>
                                                </div>
                                                <table>
                                                    <tr>
                                                        <td valign="top" width="500px">
                                                            <div class="panHeader2">
                                                                <span style="">
                                                                    <asp:Label runat="server" ID="lblColaboracion" Text="Colaboración"></asp:Label></span>
                                                            </div>
                                                            <div style="height: 400px; width: 99%; margin-top: 1px; padding-top: 10px; border: 1px solid #CDCDCD">
                                                                <div class="labelInfoSmall">
                                                                    <asp:Label ID="lblInfoColab1" runat="server" Text="Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam."></asp:Label>
                                                                </div>
                                                                <table cellpadding="0" cellspacing="0" border="0" style="width: 100%; height: 150px; padding-right: 10px;">
                                                                    <tr>
                                                                        <td style="padding-left: 10px; height: 280px;" valign="top">
                                                                            <roUserControls:roOptionPanelClient ID="optColabOnlyOneEmp" runat="server" TypeOPanel="RadioOption"
                                                                                Width="100%" Height="Auto" Checked="False" Enabled="True" Border="True" Value="1">
                                                                                <Title>
                                                                                    <asp:Label ID="lblColabOnlyOneEmpTitle" runat="server" Text="Un empleado"></asp:Label>
                                                                                </Title>
                                                                                <Description>
                                                                                    <asp:Label ID="lblColabOnlyOneEmpDesc" runat="server" Text="Sólo puede trabajar un empleado al mismo tiempo."></asp:Label>
                                                                                </Description>
                                                                                <Content>
                                                                                    <div class="panHeader2" style="margin-left: 30px; margin-top: 10px; text-align: left; width: 80%;">
                                                                                        <span style="">
                                                                                            <asp:Label runat="server" ID="lblLimitaciones" Text="Limitaciones"></asp:Label></span>
                                                                                    </div>
                                                                                    <div style="width: 80%; margin-left: 30px; margin-top: 1px; padding-top: 10px; border: 1px solid #CDCDCD">
                                                                                        <table cellpadding="0" cellspacing="0" border="0" style="width: 100%; height: 150px; padding-right: 10px;">
                                                                                            <tr>
                                                                                                <td style="padding-left: 10px; height: 80px;" valign="top">
                                                                                                    <roUserControls:roOptionPanelClient ID="optTypeCollabAny" runat="server" TypeOPanel="RadioOption"
                                                                                                        Width="100%" Height="Auto" Checked="False" Enabled="True" Border="True" Value="0" CConClick="hasChanges(true)">
                                                                                                        <Title>
                                                                                                            <asp:Label ID="lblTypeCollabAnyTitle" runat="server" Text="Cualquiera"></asp:Label>
                                                                                                        </Title>
                                                                                                        <Description>
                                                                                                            <asp:Label ID="lblTypeCollabAnyDesc" runat="server" Text="Puede trabajar cualquier empleado autorizado."></asp:Label>
                                                                                                        </Description>
                                                                                                        <Content>
                                                                                                        </Content>
                                                                                                    </roUserControls:roOptionPanelClient>
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td style="padding-left: 10px; height: 80px;" valign="top">
                                                                                                    <roUserControls:roOptionPanelClient ID="optTypeCollabFirst" runat="server" TypeOPanel="RadioOption"
                                                                                                        Width="100%" Height="Auto" Checked="False" Enabled="True" Border="True" Value="1" CConClick="hasChanges(true)">
                                                                                                        <Title>
                                                                                                            <asp:Label ID="lblTypeCollabFirstTitle" runat="server" Text="Primero"></asp:Label>
                                                                                                        </Title>
                                                                                                        <Description>
                                                                                                            <asp:Label ID="lblTypeCollabFirstDesc" runat="server" Text="Limita los autorizados al primero que fiche."></asp:Label>
                                                                                                        </Description>
                                                                                                        <Content>
                                                                                                        </Content>
                                                                                                    </roUserControls:roOptionPanelClient>
                                                                                                </td>
                                                                                            </tr>
                                                                                        </table>
                                                                                    </div>
                                                                                </Content>
                                                                            </roUserControls:roOptionPanelClient>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td style="padding-left: 10px; height: 80px;" valign="top">
                                                                            <roUserControls:roOptionPanelClient ID="optColabAllEmp" runat="server" TypeOPanel="RadioOption"
                                                                                Width="100%" Height="Auto" Checked="False" Enabled="True" Border="True" Value="0" CConClick="hasChanges(true)">
                                                                                <Title>
                                                                                    <asp:Label ID="lblColabAllEmpTitle" runat="server" Text="Todos los empleados"></asp:Label>
                                                                                </Title>
                                                                                <Description>
                                                                                    <asp:Label ID="lblColabAllEmpDesc" runat="server" Text="Pueden trabajar todos los empleados simultáneamente."></asp:Label>
                                                                                </Description>
                                                                                <Content>
                                                                                </Content>
                                                                            </roUserControls:roOptionPanelClient>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </div>
                                                        </td>
                                                        <td valign="top" width="500px">
                                                            <div class="panHeader2">
                                                                <span style="">
                                                                    <asp:Label runat="server" ID="lblEmpAutorizadosTitle" Text="Empleados autorizados"></asp:Label></span>
                                                            </div>
                                                            <div style="height: 230px; width: 99%; margin-top: 1px; padding-top: 10px; border: 1px solid #CDCDCD">
                                                                <div class="labelInfoSmall">
                                                                    <asp:Label ID="lblInfoColab2" runat="server" Text="Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam."></asp:Label>
                                                                </div>
                                                                <table cellpadding="0" cellspacing="0" border="0" style="width: 100%; height: 150px; padding-right: 10px;">
                                                                    <tr>
                                                                        <td style="padding-left: 10px; height: 80px;" valign="top">
                                                                            <roUserControls:roOptionPanelClient ID="optAutEmpAll" runat="server" TypeOPanel="RadioOption"
                                                                                Width="100%" Height="Auto" Checked="False" Enabled="True" Border="True" Value="0" CConClick="hasChanges(true)">
                                                                                <Title>
                                                                                    <asp:Label ID="lblAutEmpAllTitle" runat="server" Text="Empleados"></asp:Label>
                                                                                </Title>
                                                                                <Description>
                                                                                    <asp:Label ID="lblAutEmpAllDesc" runat="server" Text="Cualquier empleado tiene acceso a la tarea."></asp:Label>
                                                                                </Description>
                                                                                <Content>
                                                                                </Content>
                                                                            </roUserControls:roOptionPanelClient>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td style="padding-left: 10px; height: 80px;" valign="top">
                                                                            <roUserControls:roOptionPanelClient ID="optAutEmpSelect" runat="server" TypeOPanel="RadioOption"
                                                                                Width="100%" Height="Auto" Checked="False" Enabled="True" Border="True" Value="1" CConClick="hasChanges(true)">
                                                                                <Title>
                                                                                    <asp:Label ID="lblAutEmpSelectTitle" runat="server" Text="Empleados seleccionados"></asp:Label>
                                                                                </Title>
                                                                                <Description>
                                                                                    <asp:Label ID="lblAutEmpSelectDesc" runat="server" Text="Sólo los empleados ó grupos seleccionados tienen acceso a la tarea."></asp:Label>
                                                                                </Description>
                                                                                <Content>
                                                                                    <a href="javascript: void(0)" id="aEmpSelect" class="btnMode" style="width: 200px; margin-left: 22px; white-space: nowrap;">
                                                                                        <asp:Label ID="lblEmpSelect" runat="server" Text="Seleccionar..."></asp:Label>
                                                                                    </a>
                                                                                </Content>
                                                                            </roUserControls:roOptionPanelClient>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td valign="top" width="500px"></td>
                                                    </tr>
                                                </table>
                                            </dx:PanelContent>
                                        </PanelCollection>
                                    </dx:ASPxPanel>

                                    <dx:ASPxPanel ID="panel04" runat="server" ClientInstanceName="panel04" Style="height: 85%; width: 98%;"
                                        Paddings-PaddingTop="15px" Paddings-PaddingLeft="15px">
                                        <PanelCollection>
                                            <dx:PanelContent ID="PanelContent12" runat="server">
                                                <div class="labelInfoBig">
                                                    <asp:Label ID="lblInfoPpalFicha" runat="server" Text="Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."></asp:Label>
                                                </div>
                                                <div class="panHeader2">
                                                    <span style="">
                                                        <asp:Label runat="server" ID="lblFicha" Text="Ficha"></asp:Label></span>
                                                </div>
                                                <table width="100%">
                                                    <tr>
                                                        <td>
                                                            <table width="100%">
                                                                <tr>
                                                                    <td valign="top">
                                                                        <div class="labelInfoBig">
                                                                            <asp:Label ID="lblUserFieldsTaskTemplate" runat="server" Text="Campos de la ficha pertenecientes a esta tarea. Seleccione el botón Añadir para acceder al asistente y añadir nuevos campos."></asp:Label>
                                                                        </div>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td colspan="2" align="right">
                                                                        <!-- Barra Herramientas AccessPeriods -->
                                                                        <div id="panTbUserFieldsTaskTemplate" runat="server">
                                                                            <table style="margin-bottom: 0pt; margin-top: 0pt; margin-right: 0pt; width: 100%;"
                                                                                border="0" cellpadding="0" cellspacing="0">
                                                                                <tbody>
                                                                                    <tr>
                                                                                        <td colspan="2" style="padding: 2px 5px 2px 2px;" align="right">
                                                                                            <div class="btnFlat">
                                                                                                <a href="javascript: void(0)" id="btnAddUserFieldsTaskTemplate" runat="server" onclick="editGridTaskFieldsList(-1);">
                                                                                                    <span class="btnIconAdd"></span>
                                                                                                    <span id="lblAddUserFieldsTaskTemplate"><%=Me.Language.Translate("addNew", DefaultScope)%></span>
                                                                                                </a>
                                                                                            </div>
                                                                                        </td>
                                                                                    </tr>
                                                                                </tbody>
                                                                            </table>
                                                                        </div>
                                                                        <!-- Fin Barra Herramientas Inactivity -->
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td valign="top" align="center">
                                                            <div id="grdUserFieldsTask" style="width: 90%;">
                                                            </div>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </dx:PanelContent>
                                        </PanelCollection>
                                    </dx:ASPxPanel>

                                    <dx:ASPxPanel ID="panel05" runat="server" ClientInstanceName="panel05" Style="height: 85%; width: 98%;"
                                        Paddings-PaddingTop="15px" Paddings-PaddingLeft="15px">
                                        <PanelCollection>
                                            <dx:PanelContent ID="PanelContent13" runat="server">
                                                <div class="labelInfoBig">
                                                    <asp:Label ID="lblInfoPpalAlertas" runat="server" Text="Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."></asp:Label>
                                                </div>
                                                <div class="panHeader2">
                                                    <span style="">
                                                        <asp:Label runat="server" ID="lblAlerta" Text="Alertas"></asp:Label></span>
                                                </div>
                                                <table width="100%">
                                                    <tr>
                                                        <td>
                                                            <table width="100%">
                                                                <tr>
                                                                    <td valign="top">
                                                                        <div class="labelInfoBig">
                                                                            <asp:Label ID="lblAlerts" runat="server" Text="Alertas pertenecientes a esta tarea. Puede revisar el histórico de alertas e indicar si ya han sido leídas."></asp:Label>
                                                                        </div>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td valign="top" align="center">
                                                            <div id="grdAlertsTask" style="width: 90%;">
                                                            </div>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </dx:PanelContent>
                                        </PanelCollection>
                                    </dx:ASPxPanel>

                                    <dx:ASPxPanel ID="panel06" runat="server" ClientInstanceName="panel06" Style="height: 85%; width: 98%;"
                                        Paddings-PaddingTop="15px" Paddings-PaddingLeft="15px">
                                        <PanelCollection>
                                            <dx:PanelContent ID="PanelContent14" runat="server">
                                                <div class="panHeader2">
                                                    <span style="">
                                                        <asp:Label runat="server" ID="lblPuesto" Text="Puestos"></asp:Label></span>
                                                </div>
                                                <table width="100%">
                                                    <tr>
                                                        <td>
                                                            <table width="100%">
                                                                <tr>
                                                                    <td valign="top">
                                                                        <div class="labelInfoBig">
                                                                            <asp:Label ID="lblTaskAssignment" runat="server" Text="Puestos asignados a la tarea. Seleccione el botón Añadir para acceder al asistente y añadir nuevos puestos."></asp:Label>
                                                                        </div>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td colspan="2" align="right">
                                                                        <!-- Barra Herramientas AccessPeriods -->
                                                                        <div id="panTbTaskAssigment" runat="server">
                                                                            <table style="margin-bottom: 0pt; margin-top: 0pt; margin-right: 0pt; width: 100%;"
                                                                                border="0" cellpadding="0" cellspacing="0">
                                                                                <tbody>
                                                                                    <tr>
                                                                                        <td colspan="2" style="padding: 2px 5px 2px 2px;" align="right">
                                                                                            <div id="tblAddTask" class="btnFlat">
                                                                                                <a href="javascript: void(0)" id="btnAddTaskAssigment" runat="server" onclick="editGridTaskAssigmentsList();">
                                                                                                    <span id="lblAddTaskAssigment"><%=Me.Language.Translate("addEdit", DefaultScope)%></span>
                                                                                                </a>
                                                                                            </div>
                                                                                        </td>
                                                                                    </tr>
                                                                                </tbody>
                                                                            </table>
                                                                        </div>
                                                                        <!-- Fin Barra Herramientas Inactivity -->
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td valign="top" align="center">
                                                            <div id="grdAssignmentsTask" style="width: 60%;">
                                                            </div>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </dx:PanelContent>
                                        </PanelCollection>
                                    </dx:ASPxPanel>
                                </dx:PanelContent>
                            </PanelCollection>
                        </dx:ASPxCallbackPanel>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <dx:ASPxCallbackPanel ID="ASPxCallbackPanel2" runat="server" Width="1px" ClientInstanceName="pnlTaskDetails">
        <SettingsLoadingPanel Enabled="false" />
        <ClientSideEvents EndCallback="pnlTaskDetails_EndCallBack" />
        <PanelCollection>
            <dx:PanelContent ID="PanelContent2" runat="server">
                <asp:HiddenField ID="hdnFieldsASP" runat="server" Value="" />
                <asp:HiddenField ID="hdnFieldsStatistics" runat="server" Value="" />
            </dx:PanelContent>
        </PanelCollection>
    </dx:ASPxCallbackPanel>
    <dx:ASPxCallbackPanel ID="ASPxCallbackPanel3" runat="server" Width="1px" ClientInstanceName="pnlTaskStatistics">
        <SettingsLoadingPanel Enabled="false" />
        <ClientSideEvents EndCallback="pnlTaskStatistics_EndCallBack" />
        <PanelCollection>
            <dx:PanelContent ID="PanelContent3" runat="server">
                <asp:HiddenField ID="hdnFieldsStatisticsSmall" runat="server" Value="" />
            </dx:PanelContent>
        </PanelCollection>
    </dx:ASPxCallbackPanel>
    <dx:ASPxCallbackPanel ID="ASPxCallbackPanel4" runat="server" Width="1px" ClientInstanceName="pnlTaskFieldsTask">
        <SettingsLoadingPanel Enabled="false" />
        <ClientSideEvents EndCallback="pnlTaskFieldsTask_EndCallBack" />
        <PanelCollection>
            <dx:PanelContent ID="PanelContent6" runat="server">
                <asp:HiddenField ID="hdnTaskFieldsTask" runat="server" Value="" />
            </dx:PanelContent>
        </PanelCollection>
    </dx:ASPxCallbackPanel>
    <dx:ASPxCallbackPanel ID="ASPxCallbackPanel5" runat="server" Width="1px" ClientInstanceName="pnlTaskAlertsTask">
        <SettingsLoadingPanel Enabled="false" />
        <ClientSideEvents EndCallback="pnlTaskAlertsTask_EndCallBack" />
        <PanelCollection>
            <dx:PanelContent ID="PanelContent8" runat="server">
                <asp:HiddenField ID="hdnTaskAlertsTask" runat="server" Value="" />
            </dx:PanelContent>
        </PanelCollection>
    </dx:ASPxCallbackPanel>
    <dx:ASPxCallbackPanel ID="ASPxCallbackPanel6" runat="server" Width="1px" ClientInstanceName="pnlTaskAssignmentsTask">
        <SettingsLoadingPanel Enabled="false" />
        <ClientSideEvents EndCallback="pnlTaskAssignmentsTask_EndCallBack" />
        <PanelCollection>
            <dx:PanelContent ID="PanelContent11" runat="server">
                <asp:HiddenField ID="hdnTaskAssignmentsTask" runat="server" Value="" />
            </dx:PanelContent>
        </PanelCollection>
    </dx:ASPxCallbackPanel>

    <!-- POPUP EMPLEADOS EN TAREA -->
    <dx:ASPxPopupControl ID="PopupTaskEmployees" runat="server" AllowDragging="False" CloseAction="None" Modal="True" ContentUrl="~/Tasks/TaskEmployeeStatus.aspx"
        PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter" Width="480px" Height="520px"
        ShowHeader="False" ScrollBars="Auto" ShowPageScrollbarWhenModal="True" ClientInstanceName="PopupPopupTaskEmployees_Client" PopupAnimationType="none" BackColor="Transparent" ContentStyle-Paddings-Padding="0px" Border-BorderColor="Transparent" ShowShadow="false">
    </dx:ASPxPopupControl>

    <roWebControls:roPopupFrameV2 ID="RoPopupFrame1" runat="server" ShowTitleBar="true"
        BehaviorID="RoPopupFrame1Behavior" CssClassPopupExtenderBackground="modalBackgroundTransparent">
        <FrameContentTemplate>
            <table cellpadding="0" cellspacing="0">
                <tr>
                    <td>
                        <asp:Label ID="lblGroupSelection" Text="Selector de Empleados" runat="server" />
                    </td>
                    <td align="right"></td>
                </tr>
                <tr>
                    <td colspan="2" valign="top">
                        <asp:HiddenField ID="hdnIDGroupSelected" runat="server" Value="" />
                        <asp:HiddenField ID="hdnIDGroupSelectedName" runat="server" Value="" />
                        <iframe id="GroupSelectorFrame" runat="server" style="background-color: Transparent;"
                            height="200" width="200" scrolling="no" frameborder="0" marginheight="0" marginwidth="0"
                            src="" />
                    </td>
                </tr>
                <tr style="height: 35px;">
                    <td align="right">
                        <table>
                            <tr>
                                <td>
                                    <asp:ImageButton ID="btSelectorOk" runat="server" ImageUrl="~/Base/Images/ButtonOK_16.png"
                                        Style="cursor: pointer;" OnClientClick='HideGroupSelector(); return false;' />
                                    <!-- <asp:ImageButton ID="btSelectorCancel" runat="server" ImageUrl="~/Base/Images/ButtonCancel_16.png" style="cursor: pointer;" OnClientClick='HideGroupSelector(false); return false;' /> -->
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </FrameContentTemplate>
    </roWebControls:roPopupFrameV2>
    <dx:ASPxGridViewExporter ID="ASPxGridViewExporter1" runat="server">
    </dx:ASPxGridViewExporter>

    <Local:ExternalForm ID="externalform1" runat="server" />
</asp:Content>