<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Wizards_LaunchTaskTemplateWizard" EnableViewState="True" CodeBehind="LaunchTaskTemplateWizard.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Asistente para el lanzamiento de ${TasksTemplate}</title>
</head>
<body class="bodyPopup" style="background-attachment: fixed;">

    <form id="frmLaunchTaskTemplateWizard" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <div>

            <asp:UpdatePanel ID="upLaunchTaskTemplateWizard" runat="server" RenderMode="Inline">
                <ContentTemplate>
                    <script type="text/javascript">
                        var allSelected = false;

                        function selectComboBusinessCentersVisibility_ClientClick() {
                            if (allSelected == true) {
                                allSelected = false;
                                deselectAllBusinessCenters_ClientClick();
                            }
                            else {
                                allSelected = true;
                                selectAllBusinessCenters_ClientClick();
                            }
                        }

                        function selectAllBusinessCenters_ClientClick() {
                            var childContainer = document.getElementById("RoGroupBox3_treeTaskTemplates")
                            var childChkBoxes = childContainer.getElementsByTagName("input");
                            var childChkBoxCount = childChkBoxes.length;
                            for (var i = 0; i < childChkBoxCount; i++) {
                                childChkBoxes[i].checked = 'checked';
                            }
                        }

                        function deselectAllBusinessCenters_ClientClick() {
                            var childContainer = document.getElementById("RoGroupBox3_treeTaskTemplates")
                            var childChkBoxes = childContainer.getElementsByTagName("input");
                            var childChkBoxCount = childChkBoxes.length;
                            for (var i = 0; i < childChkBoxCount; i++) {
                                childChkBoxes[i].checked = '';
                            }
                        }

                        function PageBase_Load() {
                        }

                        function endRequestHandler() {
                            hidePopupLoader();
                        }

                        function showPopupLoader() {
                            if (typeof (window.parent.frames["ifPrincipal"]) != "undefined") {
                                window.parent.frames["ifPrincipal"].showLoadingGrid(true);
                            } else {
                                window.parent.parent.frames["ifPrincipal"].showLoadingGrid(true);
                            }
                        }

                        function hidePopupLoader() {
                            if (typeof (window.parent.frames["ifPrincipal"]) != "undefined") {
                                window.parent.frames["ifPrincipal"].showLoadingGrid(false);
                            } else {
                                window.parent.parent.frames["ifPrincipal"].showLoadingGrid(false);
                            }
                        }
                    </script>

                    <div class="popupWizardContent">
                        <%-- WELCOME --%>
                        <div id="divStep0" runat="server" style="display: block">
                            <table id="tbStep0" style="width: 100%; height: 100%;" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td style="height: 360px">
                                        <asp:Image ID="imgWelcome" runat="server" Style="border-radius: 5px;" ImageUrl="~/Base/Images/Wizards/wzTasks.gif" />
                                    </td>
                                    <td style="padding-left: 20px; padding-right: 20px; padding-top: 50px" valign="top">

                                        <asp:Label ID="lblWelcome1" runat="server" Text="Bienvenido al asistente para el lanzamiento de ${TasksTemplate}."
                                            Font-Bold="True" Font-Size="Large"></asp:Label>
                                        <br />
                                        <br />
                                        <br />
                                        <asp:Label ID="lblWelcome2" runat="server" Text="Este asistente le ayudará a lanzar una ${TasksTemplate}." Font-Bold="true"></asp:Label>
                                        <br />
                                        <br />
                                        <asp:Label ID="lblWelcome3" runat="server" Text="Para continuar, haga clic en siguiente."></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="height: 10px" colspan="2" class="LaunchTaskTemplateWizard_ButtonsPanel"></td>
                                </tr>
                            </table>
                        </div>

                        <asp:Label ID="hdnStepTitle" Text="Asistente para el lanzamiento de ${TasksTemplate}. " runat="server" Style="display: none; visibility: hidden" />

                        <div id="divStep1" runat="server" style="display: none;">
                            <table id="tbStep1" runat="server" style="width: 100%;" cellspacing="0" cellpadding="0" border="0">
                                <tr>
                                    <td class="LaunchTaskTemplateWizard_StepTitle" valign="top">
                                        <asp:Label ID="lblStep1Title" runat="server" Text="Paso 1 de 5." Font-Bold="True" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="LaunchTaskTemplateWizard_StepError popupWizardError" style="">
                                        <asp:Label ID="lblStep1Error" runat="server" CssClass="errorText" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="LaunchTaskTemplateWizard_StepContent" valign="top" style="padding-top: 10px;">
                                        <div style="max-height: 370px; overflow: auto">
                                            <roUserControls:roGroupBox ID="GroupBox3" runat="server">
                                                <Content>
                                                    <table border="0" style="width: 100%; width: 60%; padding-left: 10px;">
                                                        <tr>
                                                            <td style="height: 40px;">
                                                                <asp:Label ID="lblProjectTemplateName" runat="server" Text="Plantilla a utilizar:"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <roWebControls:roComboBox ID="cmbProjectTemplates" runat="server" ItemsRunAtServer="false" ParentWidth="200px" EnableViewState="true"
                                                                    HiddenText="cmbProjectTemplates_Text" ChildsVisible="6" AutoResizeChildsWidth="True" HiddenValue="cmbProjectTemplates_Value" />
                                                                <asp:HiddenField ID="cmbProjectTemplates_Text" runat="server" />
                                                                <asp:HiddenField ID="cmbProjectTemplates_Value" runat="server" />
                                                            </td>
                                                        </tr>
                                                    </table>

                                                    <table border="0" style="width: 100%; width: 60%; padding-left: 10px;">
                                                        <tr>
                                                            <td style="height: 40px;">
                                                                <asp:Label ID="lblProjectName" runat="server" Text="Nombre proyecto:"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <input type="text" runat="server" id="txtProjectName" class="textClass x-form-text x-form-field" style="width: 200px;" convertcontrol="TextField" ccallowblank="false" />
                                                            </td>
                                                        </tr>
                                                    </table>

                                                    <table border="0" style="width: 100%; width: 60%; padding-left: 10px;">
                                                        <tr>
                                                            <td style="height: 40px;">
                                                                <asp:Label ID="lblBusinessCenter" runat="server" Text="Centro de coste:"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <roWebControls:roComboBox ID="cmbBusinessCenter" runat="server" ItemsRunAtServer="false" ParentWidth="200px" EnableViewState="true"
                                                                    HiddenText="cmbBusinessCenter_Text" ChildsVisible="6" AutoResizeChildsWidth="True" HiddenValue="cmbBusinessCenter_Value" />
                                                                <asp:HiddenField ID="cmbBusinessCenter_Text" runat="server" />
                                                                <asp:HiddenField ID="cmbBusinessCenter_Value" runat="server" />
                                                            </td>
                                                        </tr>
                                                    </table>

                                                    <table border="0" style="width: 100%; width: 60%; padding-left: 10px;">
                                                        <tr>
                                                            <td style="height: 20px;">
                                                                <asp:Label ID="lblAutomaticBarCodes" runat="server" Text="Códigos de Barra:"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td style="height: 20px;">
                                                                <asp:CheckBox ID="chkAutomaticBarCodes" AutoPostBack="false" Text="Crear códigos de barra automáticamente. " Checked="false" runat="server" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </Content>
                                            </roUserControls:roGroupBox>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </div>

                        <div id="DivStep2" runat="server" style="display: none">
                            <table id="tbStep2" runat="server" style="width: 100%" cellspacing="0" cellpadding="0" border="0">
                                <tr>
                                    <td class="LaunchTaskTemplateWizard_StepTitle" valign="top">
                                        <asp:Label ID="lblStep2Title" runat="server" Text="Paso 2 de 5." Font-Bold="True" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="LaunchTaskTemplateWizard_StepError popupWizardError" style="">
                                        <asp:Label ID="lblStep2Error" runat="server" CssClass="errorText" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="LaunchTaskTemplateWizard_StepContent" valign="top" style="padding-top: 20px;">
                                        <div style="max-height: 370px; overflow: auto">
                                            <dx:ASPxGridView ID="GridProjectFields" runat="server" Cursor="pointer" AutoGenerateColumns="False" ClientInstanceName="GridProjectFields" KeyboardSupport="True" Width="100%" ClientSideEvents-BeginCallback="GridProjectFields_beginCallback" Settings-ShowTitlePanel="True">
                                                <Templates>
                                                    <TitlePanel>
                                                        <div style="float: left; padding-top: 7px; padding-left: 5px;">
                                                            <dx:ASPxLabel ID="lblProjectFieldsCaption" Text="Atributos de proyecto" runat="server" OnInit="lblProjectFieldsCaption_Init" />
                                                        </div>
                                                    </TitlePanel>
                                                </Templates>
                                                <SettingsBehavior AllowFocusedRow="True" AllowSelectSingleRowOnly="True" AllowSort="False" />
                                                <Styles>
                                                    <Cell Wrap="False"></Cell>
                                                    <TitlePanel CssClass="TitlePanelClass"></TitlePanel>
                                                </Styles>
                                                <SettingsPager Mode="ShowAllRecords" ShowEmptyDataRows="false"></SettingsPager>
                                                <Border BorderColor="#CDCDCD" />
                                                <SettingsLoadingPanel Text=""></SettingsLoadingPanel>
                                                <ClientSideEvents EndCallback="GridProjectFields_EndCallback" RowDblClick="GridProjectFields_OnRowDblClick" FocusedRowChanged="GridProjectFields_FocusedRowChanged" />
                                                <Settings VerticalScrollBarMode="Auto" UseFixedTableLayout="True" VerticalScrollableHeight="290" />
                                            </dx:ASPxGridView>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </div>

                        <div id="divStep3" runat="server" style="display: none">
                            <table id="tbStep3" runat="server" style="width: 100%" cellspacing="0" cellpadding="0" border="0">
                                <tr>
                                    <td class="LaunchTaskTemplateWizard_StepTitle" valign="top">
                                        <asp:Label ID="lblStep3Title" runat="server" Text="Paso 3 de 5." Font-Bold="True" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="LaunchTaskTemplateWizard_StepError popupWizardError" style="">
                                        <asp:Label ID="lblStep3Error" runat="server" CssClass="errorText" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="LaunchTaskTemplateWizard_StepContent" valign="top" style="padding-top: 20px;">
                                        <div style="max-height: 350px; overflow: auto">
                                            <roUserControls:roGroupBox ID="RoGroupBox3" runat="server">
                                                <Content>

                                                    <table border="0" style="width: 100%; padding-left: 10px;">
                                                        <tr>
                                                            <td style="height: 40px;">
                                                                <asp:Label ID="lblTreeTaskTemplates" runat="server" Text="Tareas:"></asp:Label>
                                                            </td>
                                                            <td style="height: 40px;"></td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:TreeView ID="treeTaskTemplates" ShowCheckBoxes="All" ShowExpandCollapse="false" runat="server"></asp:TreeView>
                                                                <asp:HiddenField ID="treeTaskTemplates_Value" runat="server" />
                                                            </td>
                                                            <td style="vertical-align: top; height: 40px;">
                                                                <table>
                                                                    <tr>
                                                                        <td colspan="3">
                                                                            <asp:CheckBox ID="chkTaskTemplates" onclick="selectComboBusinessCentersVisibility_ClientClick();" AutoPostBack="false" Text="Marcar todas las ${Tasks} como seleccionadas. " Checked="false" runat="server" />
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td colspan="3">&nbsp;&nbsp;
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td colspan="3">
                                                                            <asp:Label ID="lblTreeTaskTemplatesFilters" runat="server" Text="Filtros:"></asp:Label>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>
                                                                            <input type="text" runat="server" id="txtMaskFilter" class="textClass x-form-text x-form-field" maxlength="50" style="width: 200px;" convertcontrol="TextField" />
                                                                        </td>
                                                                        <td>
                                                                            <div id="markButton" class="buttonApproveRequest" title="Marcar" onclick="applySelectedMask(); return false;" style="cursor: pointer; height: 20px;"><%= Me.Language.Translate("mark",Me.DefaultScope) %> </div>
                                                                        </td>
                                                                        <td>
                                                                            <div id="demarkButton" class="buttonRefuseRequest" title="Desmarcar" onclick="removeSelectedMask(); return false;" style="cursor: pointer; height: 20px;"><%= Me.Language.Translate("unmark",Me.DefaultScope) %> </div>
                                                                        </td>
                                                                        <td>
                                                                            <div id="filterButton" class="buttonFilterRequest" title="Filtrar" onclick="filterSelectedMask(); return false;" style="cursor: pointer; height: 20px;"><%= Me.Language.Translate("filter", Me.DefaultScope)%> </div>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </Content>
                                            </roUserControls:roGroupBox>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </div>

                        <div id="divStep4" runat="server" style="display: none;">
                            <table id="tbStep4" runat="server" style="width: 100%" cellspacing="0" cellpadding="0" border="0">
                                <tr>
                                    <td class="LaunchTaskTemplateWizard_StepTitle" valign="top">
                                        <asp:Label ID="lblStep4Title" runat="server" Text="Paso 4 de 5." Font-Bold="True" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="LaunchTaskTemplateWizard_StepError popupWizardError" style="">
                                        <asp:Label ID="lblStep4Error" runat="server" CssClass="errorText" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="LaunchTaskTemplateWizard_StepContent" valign="top" style="padding-top: 10px;">
                                        <div style="max-height: 370px; overflow: auto">
                                            <dx:ASPxGridView ID="GridTasks" runat="server" Cursor="pointer" AutoGenerateColumns="False" ClientInstanceName="GridTasksClient" KeyboardSupport="True" Width="100%" ClientSideEvents-BeginCallback="GridTasks_beginCallback" Settings-ShowTitlePanel="True">
                                                <Templates>
                                                    <TitlePanel>
                                                        <div style="float: left; padding-top: 7px; padding-left: 5px;">
                                                            <dx:ASPxLabel ID="lblTasksCaption" Text="Tareas" runat="server" OnInit="lblTasksCaption_Init" />
                                                        </div>
                                                    </TitlePanel>
                                                </Templates>
                                                <SettingsBehavior AllowFocusedRow="True" AllowSelectSingleRowOnly="True" AllowSort="False" />
                                                <Styles>
                                                    <Cell Wrap="False"></Cell>
                                                    <TitlePanel CssClass="TitlePanelClass"></TitlePanel>
                                                </Styles>
                                                <SettingsPager Mode="ShowAllRecords" ShowEmptyDataRows="false"></SettingsPager>
                                                <Border BorderColor="#CDCDCD" />
                                                <SettingsLoadingPanel Text=""></SettingsLoadingPanel>
                                                <ClientSideEvents EndCallback="GridTasks_EndCallback" RowDblClick="GridTasks_OnRowDblClick" FocusedRowChanged="GridTasks_FocusedRowChanged" />
                                                <Settings VerticalScrollBarMode="Auto" UseFixedTableLayout="True" VerticalScrollableHeight="290" />
                                            </dx:ASPxGridView>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </div>

                        <div id="divStep5" runat="server" style="display: none;">
                            <table id="tbStep5" runat="server" style="width: 100%" cellspacing="0" cellpadding="0" border="0">
                                <tr>
                                    <td class="LaunchTaskTemplateWizard_StepTitle" valign="top">
                                        <asp:Label ID="lblStep5Title" runat="server" Text="Paso 5 de 5." Font-Bold="True" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="LaunchTaskTemplateWizard_StepError popupWizardError" style="">
                                        <asp:Label ID="lblStep5Error" runat="server" CssClass="errorText" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="LaunchTaskTemplateWizard_StepContent" valign="top" style="padding-top: 10px;">
                                        <div style="max-height: 370px; overflow: auto">
                                            <dx:ASPxGridView ID="GridTaskFieldsTask" runat="server" Cursor="pointer" AutoGenerateColumns="False" ClientInstanceName="GridTaskFieldsTask" KeyboardSupport="True" Width="100%" ClientSideEvents-BeginCallback="GridTaskFieldsTask_beginCallback" Settings-ShowTitlePanel="True">
                                                <Templates>
                                                    <TitlePanel>
                                                        <div style="float: left; padding-top: 7px; padding-left: 5px;">
                                                            <dx:ASPxLabel ID="lblTaskFieldsCaption" Text="Atributos de tarea" runat="server" OnInit="lblTaskFieldsCaption_Init" />
                                                        </div>
                                                    </TitlePanel>
                                                </Templates>
                                                <SettingsBehavior AllowFocusedRow="True" AllowSelectSingleRowOnly="True" AllowSort="False" />
                                                <Styles>
                                                    <Cell Wrap="False"></Cell>
                                                    <TitlePanel CssClass="TitlePanelClass"></TitlePanel>
                                                </Styles>
                                                <SettingsPager Mode="ShowAllRecords" ShowEmptyDataRows="false"></SettingsPager>
                                                <Border BorderColor="#CDCDCD" />
                                                <SettingsLoadingPanel Text=""></SettingsLoadingPanel>
                                                <ClientSideEvents EndCallback="GridTaskFieldsTask_EndCallback" RowDblClick="GridTaskFieldsTask_OnRowDblClick" FocusedRowChanged="GridTaskFieldsTask_FocusedRowChanged" />
                                                <Settings VerticalScrollBarMode="Auto" UseFixedTableLayout="True" VerticalScrollableHeight="290" />
                                            </dx:ASPxGridView>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>

                    <div class="popupWizardButtons">
                        <table align="right" cellpadding="0" cellspacing="0">
                            <tr class="LaunchTaskTemplateWizard_ButtonsPanel" style="height: 44px">
                                <td>&nbsp
                                </td>
                                <td>
                                    <asp:Button ID="btPrev" Text="${Button.Previous}" runat="server" OnClientClick="showPopupLoader();" Visible="false" TabIndex="1" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                </td>
                                <td>
                                    <asp:Button ID="btNext" Text="${Button.Next}" runat="server" OnClientClick="showPopupLoader();" TabIndex="2" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                    <asp:Button ID="btEnd" Text="${Button.End}" runat="server" OnClientClick="" Visible="false" TabIndex="4" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                </td>
                                <td>
                                    <asp:Button ID="btClose" Text="${Button.Cancel}" runat="server" OnClientClick="Close(); return false;" TabIndex="5" CssClass="btnFlat btnFlatBlack btnFlatAsp" />

                                    <asp:HiddenField ID="hdnCanClose_PageBase" runat="server" />
                                    <asp:HiddenField ID="hdnMustRefresh_PageBase" Value="0" runat="server" />
                                </td>
                            </tr>
                        </table>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>

            <Local:MessageFrame ID="MessageFrame1" runat="server" />
        </div>
    </form>
</body>
</html>