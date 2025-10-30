<%@ Page Language="VB" MasterPageFile="~/Base/mastEmp.master" AutoEventWireup="false" Inherits="VTLive40.TaskTemplates" Title="${TaskTemplates}" EnableEventValidation="false" CodeBehind="TaskTemplates.aspx.vb" %>

<asp:Content ID="cMainBody" ContentPlaceHolderID="contentMainBody" runat="Server">

    <script language="javascript" type="text/javascript">

        function PageBase_Load() {
            resizeFrames();
            resizeTaskTemplatesTrees();
        }
    </script>

    <input type="hidden" runat="server" id="hdnModeEdit" value="" />
    <input type="hidden" runat="server" id="noRegs" value="" />
    <div id="divModalBgDisabled" class="modalBackground" style="position: absolute; left: 0px; top: 0px; z-index: 990; width: 1680px; height: 900px; display: none;"></div>
    <input type="hidden" id="hdnSeleccionar" value="<%= Me.Language.Translate("TaskDetail.Seleccionar", Me.DefaultScope) %>,<%= Me.Language.Translate("TaskDetail.Seleccionados", Me.DefaultScope) %>" />
    <input type="hidden" id="hdnControl" value="0" />
    <input type="hidden" id="hdnProjectSelected" value="0" />
    <input type="hidden" id="hdnHdrZoneName" value="<%= Me.Language.Translate("GridFilter.hdnHdrZoneName",Defaultscope) %>" />

    <input type="hidden" id="msgHasChanges" value="" runat="server" clientidmode="Static" />
    <input type="hidden" id="msgHasErrors" value="" runat="server" clientidmode="Static" />

    <div id="divMainBody">
        <!-- TAB SUPERIOR -->
        <div id="divTabInfo" class="divDataCells">
            <div style="min-height: 10px"></div>
            <div id="divShifts" class="blackRibbonTitle">
            </div>
            <div style="min-height: 10px"></div>
        </div>
        <!-- FIN TAB SUPERIOR -->

        <!-- ARBOL Y DETALLE -->
        <div id="divTabData" class="divDataCells">

            <div id="divTree" class="treeSize">
                <div id="ctlTreeDiv" style="height: 500px;">
                    <rws:roTreesSelector ID="roTreesTaskTemplates" runat="server" ShowEmployeeFilters="false" PrefixTree="roTreesTaskTemplates"
                        Tree1Visible="true" Tree1MultiSel="false" Tree1ShowOnlyGroups="false" Tree1Function="cargaNodo" Tree1ImagePath="images/TaskTemplateSelector" Tree1SelectorPage="../../TaskTemplates/TaskTemplateSelectorData.aspx"
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

                                <dx:ASPxHiddenField ID="ProjectTaskConfig" runat="server" ClientInstanceName="ProjectTaskConfigClient"></dx:ASPxHiddenField>

                                <div id="divContentPanels" class="divContentPanelsWithOutMessage">
                                    <div id="trProjects" runat="server" class="contentPanel" style="display: none">
                                        <!-- PANELL GRUPS GENERAL -->
                                        <div id="div00" class="contentPanel" style="display: none" runat="server" name="menuPanel">
                                            <div class="panHeader2">
                                                <span style="">
                                                    <asp:Label runat="server" ID="lblProjectGeneralTitle" Text="General"></asp:Label>
                                                </span>
                                            </div>
                                            <br />
                                            <table style="width: 99%;">
                                                <tr>
                                                    <td width="70px" align="right" style="padding-right: 5px;">
                                                        <asp:Label ID="lblProjectName" runat="server" Text="Nombre:" class="spanEmp-Class"></asp:Label></td>
                                                    <td>
                                                        <dx:ASPxTextBox ID="txtProjectName" runat="server" MaxLength="50" Width="200px" ClientInstanceName="txtProjectName_Client" NullText="_____">
                                                            <ClientSideEvents Validation="LengthValidation" TextChanged="function(s,e){checkProjectEmptyName(s.GetValue());}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                            <ValidationSettings SetFocusOnError="True">
                                                                <RequiredField IsRequired="True" ErrorText="(*)" />
                                                            </ValidationSettings>
                                                        </dx:ASPxTextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                            <br />
                                            <div class="panHeader2">
                                                <span style="">
                                                    <asp:Label runat="server" ID="lblTitleEmpActShiftGroup" Text="${TasksTemplate} que actualmente estan en el proyecto"></asp:Label>
                                                </span>
                                            </div>
                                            <br />
                                            <table style="margin: 10px; width: 99%;">
                                                <tr>
                                                    <td align="center" valign="top">
                                                        <table width="100%" border="0" style="text-align: center;">
                                                            <tr>
                                                                <td width="100%" align="center" valign="top">
                                                                    <table width="100%" border="0" cellpadding="0" cellspacing="0">
                                                                        <tr>
                                                                            <td height="20px" valign="top">
                                                                                <div style="width: 100%; height: auto;">
                                                                                    <div id="divHeaderProject" runat="server" style="width: 100%;">
                                                                                    </div>
                                                                                </div>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td valign="top">
                                                                                <div style="width: 100%; height: 280px; overflow: auto;">
                                                                                    <div id="divGridProject" runat="server" style="width: 100%;">
                                                                                    </div>
                                                                                </div>
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
                                                    <asp:Label runat="server" ID="lblBusinessGroupDesc" Text="Grupo de Negocio"></asp:Label>
                                                </span>
                                            </div>
                                            <br />
                                            <table style="margin: 10px;">
                                                <tr>
                                                    <td style="padding-left: 1px; padding-top: 10px; width: 100px;">
                                                        <asp:Label ID="lblCenter" runat="server" Text="Centro de coste:" class="spanEmp-Class"></asp:Label>
                                                    </td>
                                                    <td style="padding-left: 1px; padding-top: 10px; width: 100px;">
                                                        <dx:ASPxComboBox ID="cmbGroup" runat="server">
                                                            <ClientSideEvents SelectedIndexChanged="function(s,e){ hasChanges(true); }" />
                                                        </dx:ASPxComboBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>

                                        <!-- Campos de la ficha -->
                                        <div id="div01" class="contentPanel" style="display: none" runat="server" name="menuPanel">
                                            <div class="panHeader2">
                                                <span style="">
                                                    <asp:Label runat="server" ID="lblFieldTaskTitle" Text="Campos de la ficha"></asp:Label></span>
                                            </div>
                                            <br />
                                            <table border="0" width="100%">
                                                <tr>
                                                    <td width="90%" valign="top" align="center">
                                                        <!-- GRID -->
                                                        <table width="100%">
                                                            <tr>
                                                                <td>
                                                                    <table width="100%">
                                                                        <tr>
                                                                            <td style="padding-left: 10px; width: 48px; height: 60px;">
                                                                                <img src="Images/FieldAssign.gif" alt="" /></td>
                                                                            <td valign="top">
                                                                                <asp:Label ID="lblUserFieldsProject" runat="server" Text="Campos de la ficha pertenecientes a este proyecto. Seleccione el botón Añadir para acceder al asistente y añadir nuevos campos."></asp:Label></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td colspan="2" align="right">
                                                                                <!-- Barra Herramientas AccessPeriods -->
                                                                                <div id="panTbUserFieldsProject" runat="server">
                                                                                    <table style="margin-bottom: 0pt; margin-top: 0pt; margin-right: 0pt; width: 100%;" border="0" cellpadding="0" cellspacing="0">
                                                                                        <tbody>
                                                                                            <tr>
                                                                                                <td colspan="2" style="padding: 2px 5px 2px 2px;" align="right">
                                                                                                    <div class="btnFlat">
                                                                                                        <a href="javascript: void(0)" id="btnAddUserFieldsProject" runat="server" onclick="ShowTemplateUserFieldsWizard(true);">
                                                                                                            <span class="btnIconAdd"></span>
                                                                                                            <span id="lblAddUserFieldsProject"><%= Me.Language.Translate("addNew",DefaultScope) %></span>
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
                                                                    <div id="grdUserFieldsProject" runat="server" style="width: 90%;"></div>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </div>

                                    <div id="trTaskTemplate" runat="server" class="contentPanel" style="display: none">
                                        <!-- Panell General -->
                                        <div id="div20" class="contentPanel" style="display: none" runat="server" name="menuPanel">
                                            <div class="panHeader2">
                                                <span style="">
                                                    <asp:Label runat="server" ID="lblGeneralTitle" Text="General"></asp:Label></span>
                                            </div>
                                            <br />
                                            <!-- Tab Panel 1 General -->
                                            <table width="100%" border="0" cellpadding="2" cellspacing="2">
                                                <tr>
                                                    <td width="150px" align="right" style="padding-right: 10px;">
                                                        <asp:Label ID="Label2" runat="server" Text="Nombre abreviado:" class="spanEmp-Class"></asp:Label></td>
                                                    <td align="left">
                                                        <dx:ASPxTextBox ID="txtTaskTemplateName" runat="server" MaxLength="50" Width="200px" ClientInstanceName="txtTaskTemplateName_Client" NullText="_____">
                                                            <ClientSideEvents Validation="LengthValidation" TextChanged="function(s,e){checkTaskTemplateEmptyName(s.GetValue());}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                            <ValidationSettings SetFocusOnError="True">
                                                                <RequiredField IsRequired="True" ErrorText="(*)" />
                                                            </ValidationSettings>
                                                        </dx:ASPxTextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td width="150px" align="right" style="padding-right: 10px;">
                                                        <asp:Label ID="lblShortName" runat="server" Text="Nombre abreviado:" class="spanEmp-Class"></asp:Label></td>
                                                    <td align="left">
                                                        <dx:ASPxTextBox ID="txtShortName" runat="server" MaxLength="4" Width="50px" NullText="_____" ClientInstanceName="txtShortName_Client">
                                                            <ClientSideEvents Validation="LengthValidation" TextChanged="function(s,e){ hasChanges(true); }" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                            <ValidationSettings SetFocusOnError="True">
                                                                <RequiredField IsRequired="True" ErrorText="(*)" />
                                                            </ValidationSettings>
                                                        </dx:ASPxTextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td width="150px" align="right" style="padding-right: 10px;">
                                                        <asp:Label ID="lblColorDesc" runat="server" Text="Color identificativo:" class="spanEmp-Class"></asp:Label></td>
                                                    <td align="left">
                                                        <dx:ASPxColorEdit ID="colorTaskTemplate" EnableCustomColors="true" runat="server" Width="14px">
                                                            <ClientSideEvents ColorChanged="function(s,e){s.GetInputElement().style.display = 'none'; hasChanges(true); }" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                        </dx:ASPxColorEdit>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td width="150px" align="right" valign="top" style="padding-right: 10px;">
                                                        <asp:Label ID="lblDescription" runat="server" Text="Descripción:" class="spanEmp-Class"></asp:Label></td>
                                                    <td align="left" valign="top" style="padding-right: 30px;">
                                                        <dx:ASPxMemo ID="txtDescription" runat="server" Rows="5" Width="500px">
                                                            <ClientSideEvents TextChanged="function(s,e){ hasChanges(true); }" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                        </dx:ASPxMemo>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td width="150px" align="right" style="padding-right: 10px;">
                                                        <asp:Label ID="lblTag" runat="server" Text="Tags:" class="spanEmp-Class"></asp:Label></td>
                                                    <td align="left">
                                                        <input type="text" runat="server" id="txtTag" class="textClass x-form-text x-form-field" maxlength="70" style="width: 500px;" convertcontrol="TextField" ccallowblank="true" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td width="150px" align="right" style="padding-right: 10px;">
                                                        <asp:Label ID="lblPriority" runat="server" Text="Prioridad:" class="spanEmp-Class"></asp:Label></td>
                                                    <td align="left">
                                                        <dx:ASPxTrackBar ID="TrackBarPriority" runat="server" ScalePosition="LeftOrTop" MinValue="0"
                                                            MaxValue="20" Step="1" Width="500px" LargeTickInterval="1" SmallTickFrequency="1" Visible="true"
                                                            ClientInstanceName="TrackBarPriority" DragHandleToolTip="" IncrementButtonToolTip=""
                                                            DecrementButtonToolTip="">
                                                            <ClientSideEvents ValueChanged="function(s, e) { SetTrackbarValue(s,e); hasChanges(true); }" />
                                                        </dx:ASPxTrackBar>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>

                                        <!-- Datos teóricos -->
                                        <div id="div21" class="contentPanel" style="display: none" runat="server" name="menuPanel">
                                            <div class="panHeader2">
                                                <span style="">
                                                    <asp:Label runat="server" ID="lblTypeTitle" Text="Datos teóricos"></asp:Label></span>
                                            </div>
                                            <br />

                                            <!-- Tab2 Panel 2Teoricos-->
                                            <table>
                                                <tr>
                                                    <td valign="top" width="1200px">
                                                        <div class="panHeader2">
                                                            <span style="">
                                                                <asp:Label runat="server" ID="lblPrevisionTitle" Text="Previsión"></asp:Label></span>
                                                        </div>
                                                        <div style="width: 100%; margin-top: 1px; padding-top: 10px; border: 1px solid #CDCDCD">

                                                            <div class="labelInfoSmall">
                                                                <asp:Label ID="lblInfoDatosTeo1" runat="server" Text=""></asp:Label>
                                                            </div>

                                                            <table cellpadding="0" cellspacing="0" border="0" style="height: 40px; padding-right: 10px;">
                                                                <tr style="height: 40px;">
                                                                    <td align="right" width="100px" style="padding-right: 10px;">
                                                                        <asp:Label ID="lblInitialTime" runat="server" Text="Horas Iniciales:" class="spanEmp-Class"></asp:Label>
                                                                    </td>
                                                                    <td align="left" width="60px">
                                                                        <dx:ASPxTextBox ID="txtInitialTime" runat="server" Width="50px" MaxLength="7">
                                                                            <MaskSettings Mask="<0..9999>:<00..59>" />
                                                                            <ClientSideEvents TextChanged="function(s,e){ hasChanges(true); }" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                        </dx:ASPxTextBox>
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
                                                    <td valign="top" width="800px">
                                                        <div class="panHeader2">
                                                            <span style="">
                                                                <asp:Label runat="server" ID="lblActivacion" Text="Activación"></asp:Label></span>
                                                        </div>
                                                        <div style="height: 200px; width: 99%; margin-top: 1px; padding-top: 10px; border: 1px solid #CDCDCD">

                                                            <input type="hidden" id="hdnActivationTask" runat="server" value="0" />

                                                            <div class="labelInfoSmall">
                                                                <asp:Label ID="lblInfoDatosTeo3" runat="server" Text=""></asp:Label>
                                                            </div>

                                                            <table cellpadding="0" cellspacing="0" border="0" style="width: 100%; height: 150px; padding-right: 10px;">
                                                                <tr>
                                                                    <td style="padding-left: 10px; height: 80px;" valign="top">
                                                                        <roUserControls:roOptionPanelClient ID="optActivAllways" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Checked="False" Enabled="True" Border="True" Value="0" CConClick="hasChanges(true);">
                                                                            <Title>
                                                                                <asp:Label ID="lbloptActivAllwaysTitle" runat="server" Text="Siempre"></asp:Label>
                                                                            </Title>
                                                                            <Description>
                                                                                <asp:Label ID="lbloptActivAllwaysDesc" runat="server" Text="La tarea siempre está disponible para su activación."></asp:Label>
                                                                            </Description>
                                                                            <Content>
                                                                            </Content>
                                                                        </roUserControls:roOptionPanelClient>
                                                                    </td>
                                                                    <td style="padding-left: 10px; height: 80px;" valign="top">
                                                                        <roUserControls:roOptionPanelClient ID="optActivByDate" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Checked="False" Enabled="True" Border="True" Value="1" CConClick="hasChanges(true);">
                                                                            <Title>
                                                                                <asp:Label ID="lblActivByDateTitle" runat="server" Text="Fecha y Hora"></asp:Label>
                                                                            </Title>
                                                                            <Description>
                                                                                <asp:Label ID="lblActivByDateDesc" ForeColor="steelblue" runat="server" Text="Se activa mediante una fecha y hora fija."></asp:Label>
                                                                            </Description>
                                                                            <Content>
                                                                            </Content>
                                                                        </roUserControls:roOptionPanelClient>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td style="padding-left: 10px; height: 80px;" valign="top">
                                                                        <roUserControls:roOptionPanelClient ID="optActivByEndTask" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Checked="False" Enabled="True" Border="True" Value="2" CConClick="hasChanges(true);">
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
                                                                                            <input type="text" runat="server" id="txtEndTask" readonly="readonly" class="textClass x-form-text x-form-field" style="width: 250px;" convertcontrol="TextField" />
                                                                                        </td>
                                                                                        <td>
                                                                                            <a onclick="ShowTasksSelector(1);" title="Selector de Tareas" href="javascript: void(0);">
                                                                                                <img alt="Selector de Tareas" src="Images/TaskTemplates16.png" /></a>
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                            </Content>
                                                                        </roUserControls:roOptionPanelClient>
                                                                    </td>
                                                                    <td style="padding-left: 10px; height: 80px;" valign="top">
                                                                        <roUserControls:roOptionPanelClient ID="optActivByIniTask" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Checked="False" Enabled="True" Border="True" Value="3" CConClick="hasChanges(true);">
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
                                                                                            <input type="text" runat="server" id="txtIniTask" readonly="readonly" class="textClass x-form-text x-form-field" style="width: 250px;" convertcontrol="TextField" />
                                                                                        </td>
                                                                                        <td>
                                                                                            <a onclick="ShowTasksSelector(2);" title="Selector de Tareas" href="javascript: void(0);">
                                                                                                <img alt="Selector de Tareas" src="Images/TaskTemplates16.png" /></a>
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
                                                    <td valign="top" width="400px">
                                                        <div class="panHeader2">
                                                            <span style="">
                                                                <asp:Label runat="server" ID="lblCierre" Text="Cierre"></asp:Label></span>
                                                        </div>
                                                        <div style="height: 200px; width: 99%; margin-top: 1px; padding-top: 10px; border: 1px solid #CDCDCD">

                                                            <div class="labelInfoSmall">
                                                                <asp:Label ID="lblInfoDatosTeo4" runat="server" Text=""></asp:Label>
                                                            </div>

                                                            <table cellpadding="0" cellspacing="0" border="0" style="width: 100%; height: 150px; padding-right: 10px;">
                                                                <tr>
                                                                    <td style="padding-left: 10px; height: 80px;" valign="top">
                                                                        <roUserControls:roOptionPanelClient ID="optClosingAllways" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Checked="False" Enabled="True" Border="True" Value="0" CConClick="hasChanges(true);">
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
                                                                    <td style="padding-left: 10px; height: 80px;" valign="top">
                                                                        <roUserControls:roOptionPanelClient ID="optClosingByDate" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Checked="False" Enabled="True" Border="True" Value="1" CConClick="hasChanges(true);">
                                                                            <Title>
                                                                                <asp:Label ID="lblClosingByDateTitle" runat="server" Text="Fecha y Hora"></asp:Label>
                                                                            </Title>
                                                                            <Description>
                                                                                <asp:Label ID="lblClosingByDateDesc" ForeColor="steelblue" runat="server" Text="Se cierra mediante una fecha y hora fija."></asp:Label>
                                                                            </Description>
                                                                            <Content>
                                                                            </Content>
                                                                        </roUserControls:roOptionPanelClient>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </div>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>

                                        <!-- Autorizados -->
                                        <div id="div22" class="contentPanel" style="display: none" runat="server" name="menuPanel">
                                            <div class="panHeader2">
                                                <span style="">
                                                    <asp:Label runat="server" ID="lblDefinitionTitle" Text="Autorizados"></asp:Label></span>
                                            </div>
                                            <br />
                                            <!-- Tab3 Panel Autorizados-->
                                            <table>
                                                <tr>
                                                    <td valign="top" width="500px">
                                                        <div class="panHeader2">
                                                            <span style="">
                                                                <asp:Label runat="server" ID="lblColaboracion" Text="Colaboración"></asp:Label></span>
                                                        </div>
                                                        <div style="height: 400px; width: 99%; margin-top: 1px; padding-top: 10px; border: 1px solid #CDCDCD">

                                                            <div class="labelInfoSmall">
                                                                <asp:Label ID="lblInfoColab1" runat="server" Text=""></asp:Label>
                                                            </div>

                                                            <table cellpadding="0" cellspacing="0" border="0" style="width: 100%; height: 150px; padding-right: 10px;">
                                                                <tr>
                                                                    <td style="padding-left: 10px; height: 280px;" valign="top">
                                                                        <roUserControls:roOptionPanelClient ID="optColabOnlyOneEmp" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Checked="False" Enabled="True" Border="True" Value="1" CConClick="hasChanges(true);">
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
                                                                                                <roUserControls:roOptionPanelClient ID="optTypeCollabAny" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Checked="False" Enabled="True" Border="True" Value="0" CConClick="hasChanges(true);">
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
                                                                                                <roUserControls:roOptionPanelClient ID="optTypeCollabFirst" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Checked="False" Enabled="True" Border="True" Value="1" CConClick="hasChanges(true);">
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
                                                                        <roUserControls:roOptionPanelClient ID="optColabAllEmp" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Checked="False" Enabled="True" Border="True" Value="0" CConClick="hasChanges(true);">
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
                                                                <asp:Label ID="lblInfoColab2" runat="server" Text=""></asp:Label>
                                                            </div>

                                                            <table cellpadding="0" cellspacing="0" border="0" style="width: 100%; height: 150px; padding-right: 10px;">
                                                                <tr>
                                                                    <td style="padding-left: 10px; height: 80px;" valign="top">
                                                                        <roUserControls:roOptionPanelClient ID="optAutEmpAll" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Checked="False" Enabled="True" Border="True" Value="0" CConClick="hasChanges(true);">
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
                                                                        <roUserControls:roOptionPanelClient ID="optAutEmpSelect" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Checked="False" Enabled="True" Border="True" Value="1" CConClick="hasChanges(true);">
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
                                        </div>

                                        <!-- Campos de la ficha -->
                                        <div id="div23" class="contentPanel" style="display: none" runat="server" name="menuPanel">
                                            <div class="panHeader2">
                                                <span style="">
                                                    <asp:Label runat="server" ID="Label1" Text="Campos de la ficha"></asp:Label></span>
                                            </div>
                                            <br />
                                            <table border="0" width="100%" height="100%">
                                                <tr>
                                                    <td width="90%" height="100%" valign="top" align="center">
                                                        <!-- GRID -->
                                                        <table width="100%">
                                                            <tr>
                                                                <td>
                                                                    <table width="100%">
                                                                        <tr>
                                                                            <td style="padding-left: 10px; width: 48px; height: 60px;">
                                                                                <img src="Images/FieldAssign.gif" alt="" /></td>
                                                                            <td valign="top">
                                                                                <asp:Label ID="lblUserFieldsTaskTemplate" runat="server" Text="Campos de la ficha pertenecientes a esta plantilla. Seleccione el botón Añadir para acceder al asistente y añadir nuevos campos."></asp:Label></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td colspan="2" align="right">
                                                                                <!-- Barra Herramientas AccessPeriods -->
                                                                                <div id="panTbUserFieldsTaskTemplate" runat="server">
                                                                                    <table style="margin-bottom: 0pt; margin-top: 0pt; margin-right: 0pt; width: 100%;" border="0" cellpadding="0" cellspacing="0">
                                                                                        <tbody>
                                                                                            <tr>
                                                                                                <td colspan="2" style="padding: 2px 5px 2px 2px;" align="right">
                                                                                                    <div class="btnFlat">
                                                                                                        <a href="javascript: void(0)" id="btnAddUserFieldsTaskTemplate" runat="server" onclick="ShowTemplateUserFieldsWizard(false);">
                                                                                                            <span class="btnIconAdd"></span>
                                                                                                            <span id="lblAddUserFieldsTaskTemplate"><%= Me.Language.Translate("addNew",DefaultScope) %></span>
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
                                                                    <div id="grdUserFieldsTaskTemplate" style="width: 90%;" runat="server"></div>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>
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

        <!-- POPUP NEW OBJECT -->
        <dx:ASPxPopupControl ID="NewObjectPopup" runat="server" AllowDragging="False" CloseAction="None" Modal="True" ContentUrl="~/Base/Popups/CreateObjectPopup.aspx"
            PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter" Width="470px" Height="300px"
            ShowHeader="False" ScrollBars="Auto" ShowPageScrollbarWhenModal="True" ClientInstanceName="NewObjectPopup_Client" PopupAnimationType="None" BackColor="Transparent" ContentStyle-Paddings-Padding="0px" Border-BorderColor="Transparent" ShowShadow="false">
            <SettingsLoadingPanel Enabled="false" />
        </dx:ASPxPopupControl>
    </div>
    <roWebControls:roPopupFrameV2 ID="RoPopupFrame1" runat="server" ShowTitleBar="true" BehaviorID="RoPopupFrame1Behavior" CssClassPopupExtenderBackground="modalBackgroundTransparent">
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
                        <iframe id="GroupSelectorFrame" runat="server" style="background-color: Transparent;" height="200" width="200" scrolling="no" frameborder="0"
                            marginheight="0" marginwidth="0" src="" />
                    </td>
                </tr>
                <tr style="height: 35px;">
                    <td align="right">
                        <table>
                            <tr>
                                <td>
                                    <asp:ImageButton ID="btSelectorOk" runat="server" ImageUrl="~/Base/Images/ButtonOK_16.png" Style="cursor: pointer;" OnClientClick='HideGroupSelector(); return false;' />
                                    <!-- <asp:ImageButton ID="btSelectorCancel" runat="server" ImageUrl="~/Base/Images/ButtonCancel_16.png" style="cursor: pointer;" OnClientClick='HideGroupSelector(false); return false;' /> -->
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </FrameContentTemplate>
    </roWebControls:roPopupFrameV2>

    <script language="javascript" type="text/javascript">
        function resizeTaskTemplatesTrees() {
            try {
                var ctlPrefix = "ctl00_contentMainBody_roTreesTaskTemplates";
                eval(ctlPrefix + "_resizeTrees();");
            }
            catch (e) {
                showError("resizeTaskTemplatesTrees", e);
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
            resizeTaskTemplatesTrees();
        }

        if (document.getElementById('<%= noRegs.ClientID %>').value != '') {
            cargaProject(-1);
        }
    </script>
</asp:Content>