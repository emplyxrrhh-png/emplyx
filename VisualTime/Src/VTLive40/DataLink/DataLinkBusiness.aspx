<%@ Page Language="VB" MasterPageFile="~/Base/mastEmp.master" AutoEventWireup="true" Inherits="VTLive40.DataLinkBusiness" Title="${Datalink}" EnableEventValidation="false" CodeBehind="DataLinkBusiness.aspx.vb" %>

<%@ Register TagPrefix="dx" Namespace="DevExpress.Web" Assembly="DevExpress.Web.v23.1" %>
<%@ Register TagPrefix="dx" Namespace="DevExpress.Web.ASPxSpreadsheet" Assembly="DevExpress.Web.ASPxSpreadsheet.v23.1" %>
<asp:Content ID="cMainBody" ContentPlaceHolderID="contentMainBody" runat="Server">

    <script language="javascript" type="text/javascript">

        function PageBase_Load() {
            resizeFrames();
            resizeTreeDatalink();
            //PPR desactivado temporalmente NO ELIMINAR--> loadInitialPageValues();
        }

        function GetSelectedTreeV3(oParm1, oParm2, oParm3) {
            if (oParm1 == "") {
                document.getElementById('ctl00_contentMainBody_ExportScheduleEdit_ASPxPanel4_CallbackPopupOperations_hdnEmployees').value = "";
                document.getElementById('ctl00_contentMainBody_ExportScheduleEdit_ASPxPanel4_CallbackPopupOperations_hdnFilter').value = "";
                document.getElementById('ctl00_contentMainBody_ExportScheduleEdit_ASPxPanel4_CallbackPopupOperations_hdnFilterUser').value = "";
            }
            else {
                document.getElementById('ctl00_contentMainBody_ExportScheduleEdit_ASPxPanel4_CallbackPopupOperations_hdnEmployees').value = oParm1;
                document.getElementById('ctl00_contentMainBody_ExportScheduleEdit_ASPxPanel4_CallbackPopupOperations_hdnFilter').value = oParm2;
                document.getElementById('ctl00_contentMainBody_ExportScheduleEdit_ASPxPanel4_CallbackPopupOperations_hdnFilterUser').value = oParm3;
            }
        }

        function PopupSelectorEmployeesClient_PopUp(s, e) {
            try {
                s.SetHeaderText("");
                var iFrm = document.getElementById('<%= GroupSelectorFrame.ClientID %>');

                var strBase = '<%= Me.Page.ResolveURL("~/Base/") %>' + "WebUserControls/roWizardSelectorContainerMultiSelectV3.aspx?" +
                    "PrefixTree=treeEmpDatalinkBusinessExport&FeatureAlias=Calendar&PrefixCookie=objContainerTreeV3_treeEmpDatalinkBusinessExportGrid&" +
                    "AfterSelectFuncion=parent.GetSelectedTreeV3";
                iFrm.src = strBase;
            }
            catch (e) {
                showError("PopupSelectorEmployeesClient_PopUp", e);
            }
        }
    </script>

    <input type="hidden" runat="server" id="noRegs" value="" />
    <input type="hidden" id="msgHasChanges" value="" runat="server" clientidmode="Static" />
    <input type="hidden" id="msgHasErrors" value="" runat="server" clientidmode="Static" />

    <div id="divMainBody">
        <!-- TAB SUPERIOR -->
        <div id="divTabInfo" class="divDataCells">
            <div style="min-height: 10px"></div>
            <div id="divDatalinkBusiness" class="blackRibbonTitle">
            </div>
            <div style="min-height: 10px"></div>
        </div>
        <!-- FIN TAB SUPERIOR -->

        <!-- ARBOL Y DETALLE -->
        <div id="divTabData" class="divDataCells">

            <div id="divTree" class="treeSize">
                <div id="ctlTreeDiv" style="height: 500px;">
                    <rws:roTreesSelector ID="roTreesDatalink" runat="server" ShowEmployeeFilters="false" PrefixTree="roTreesDatalink" Tree1Visible="true" Tree1MultiSel="false"
                        Tree1ShowOnlyGroups="false" Tree1Function="cargaNodo" Tree1ImagePath="images/DatalinkSelector" Tree1SelectorPage="../../Datalink/DatalinkSelectorData.aspx"
                        ShowTreeCaption="true" ShowRefreshTree="True"></rws:roTreesSelector>
                </div>
            </div>

            <div id="divContenido" class="divRightContent divRightContentWithoutBar">
                <div id="divContent" class="maxHeight">
                    <%--<dx:ASPxCallback ID="CallbackPopupOperations" runat="server" ClientInstanceName="CallbackPopupOperationsClient" ClientSideEvents-CallbackComplete="CallbackPopupOperations_CallbackComplete"></dx:ASPxCallback>--%>
                    <dx:ASPxCallback ID="CallbackExcel" runat="server" ClientInstanceName="CallbackExcelClient" ClientSideEvents-CallbackComplete="CallbackExcel_CallbackComplete"></dx:ASPxCallback>
                    <dx:ASPxCallbackPanel ID="ASPxCallbackPanelContenido" runat="server" Width="100%" Height="100%" ClientInstanceName="ASPxCallbackPanelContenidoClient">
                        <SettingsLoadingPanel Enabled="false" />
                        <ClientSideEvents BeginCallback="ASPxCallbackPanelContenidoClient_BeginCallBack" EndCallback="ASPxCallbackPanelContenidoClient_EndCallBack" />
                        <PanelCollection>
                            <dx:PanelContent ID="PanelContent1" runat="server">
                                <input type="hidden" runat="server" id="hdnSampleFileName" value="" />

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
                                    <!-- Panell Importación -->
                                    <div id="div00" class="contentPanel" runat="server" name="menuPanel">
                                        <div class="panHeader2">
                                            <span style="">
                                                <asp:Label runat="server" ID="lblDatalinkBusinessTitle" Text="Descripción"></asp:Label></span>
                                        </div>
                                        <br />
                                        <div class="panBottomMargin">
                                            <div class="divRow">
                                                <div class="componentForm">
                                                    <asp:Label ID="lblDatalinkBusinessInnerDescription" runat="server" Text="" CssClass="textClassWithoutBorder"></asp:Label>
                                                    <br />
                                                    <asp:Label ID="lblDatalinkBusinessDescription" runat="server" CssClass="textClassWithoutBorder" Text="Selecciona entre las distintas plantilla disponibles para obtener información que más se ajuste a tus necesidades."></asp:Label>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="panBottomMargin">
                                            <div class="panHeader2">
                                                <span style="">
                                                    <asp:Label runat="server" ID="lblManualImport" Text="Datos para la importación manual"></asp:Label></span>
                                            </div>
                                            <br />
                                            <div class="panBottomMargin">
                                                <div class="divRow" style="display: none">
                                                    <div class="divRowDescription">
                                                        <asp:Label ID="lblUploadFileNameDesc" runat="server" Text="Indique el archivo a cargar para importar"></asp:Label>
                                                    </div>
                                                    <asp:Label ID="lblUploadFileName" runat="server" Text="Fichero:" CssClass="labelForm"></asp:Label>
                                                    <div class="componentForm">
                                                        <dx:ASPxUploadControl ID="fImportUpload" runat="server" ClientInstanceName="fImportUploadClient" Width="100%"
                                                            NullText="Select multiple files..." UploadMode="Auto" ShowUploadButton="false" ShowProgressPanel="True" Theme="Robo">
                                                            <AdvancedModeSettings EnableMultiSelect="false" EnableFileList="false" EnableDragAndDrop="True" />
                                                        </dx:ASPxUploadControl>
                                                    </div>
                                                </div>

                                                <div class="divRow">
                                                    <table width="100%" border="0" style="margin-top: 30px; display: none" id="btnImportDiv" runat="server">
                                                        <tr>
                                                            <td align="center" style="padding-bottom: 20px">
                                                                <dx:ASPxLabel ID="lblFileSampleDownload" runat="server" Text="No dispone de la plantilla básica para realizar la importación? Descarguela hicendo click aquí." Cursor="pointer" CssClass="textClassWithoutBorder">
                                                                    <ClientSideEvents Click="function(s, e) { downloadSampleTemplate(); return false; }" />
                                                                </dx:ASPxLabel>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="center">
                                                                <dx:ASPxButton ID="btnImport" runat="server" AutoPostBack="False" CausesValidation="False" Text="Lanzar importación..." ToolTip="Lanzar importación..."
                                                                    HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat">
                                                                    <ClientSideEvents Click="function(s, e) { ShowImportWizard(); return false; }" />
                                                                </dx:ASPxButton>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="panBottomMargin" id="rowAutomaticImport" style="display: none" runat="server">
                                            <div class="panHeader2">
                                                <span style="">
                                                    <asp:Label runat="server" ID="lblAutomaticImport" Text="Configurar importación automática"></asp:Label></span>
                                            </div>
                                            <div class="panBottomMargin" style="margin-top: 30px;">

                                                <div class="divRow">
                                                    <div style="padding-left: 250px" class="componentFormMaxWidth">
                                                        <div style="float: left">
                                                            <dx:ASPxCheckBox ID="ckImportEnable" ClientInstanceName="ckImportEnableClient" runat="server" Text="Habilitar importación automática" CssClass="textClassWithoutBorder">
                                                                <ValidationSettings ErrorDisplayMode="None" />
                                                                <ClientSideEvents CheckedChanged="function(s,e){ enableAutomaticImport(s.GetChecked());}" />
                                                            </dx:ASPxCheckBox>
                                                        </div>
                                                        <div style="float: left; padding-left: 20px;">
                                                            <dx:ASPxCheckBox ID="ckImportActive" runat="server" Text="Habilitada pero no activa" ReadOnly="true" Checked="false" CssClass="textClassWithoutBorder">
                                                                <ValidationSettings ErrorDisplayMode="None" />
                                                                <CheckedImage Url="Images/green_light.png" ToolTip="Checked" Width="16" Height="16" SpriteProperties-CssClass="imgContain" />
                                                                <UncheckedImage Url="Images/red_light.png" ToolTip="Unchecked" Width="16" Height="16" SpriteProperties-CssClass="imgContain" />
                                                                <ClientSideEvents CheckedChanged="function(s,e){ hasChanges(true);}" />
                                                            </dx:ASPxCheckBox>
                                                        </div>
                                                        <div style="float: left; padding-left: 20px;">
                                                            <dx:ASPxButton ID="btnLastImportDetails" runat="server" AutoPostBack="False" CausesValidation="False" Text="Detalles última importación" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat">
                                                                <ClientSideEvents Click="function(s, e) { PopupImportLogsViewClient.Show(); return false; }" />
                                                            </dx:ASPxButton>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div id="divFormatImport" class="divRow" runat="server">
                                                    <asp:Label ID="lblFormatImport" runat="server" Text="Tipo de formato:" CssClass="labelForm maxWidth"></asp:Label>
                                                    <div class="componentFormMaxWidth">
                                                        <dx:ASPxComboBox runat="server" ID="cmbFormatImport" Width="250px" ClientInstanceName="cmbFormatImportClient">
                                                            <ClientSideEvents SelectedIndexChanged="function(s,e){hasChanges(true); cmbImportIsASCII();}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                        </dx:ASPxComboBox>
                                                    </div>
                                                </div>
                                                <div id="divImportSeparator" class="divRow divImportSeparator" runat="server">
                                                    <asp:Label ID="lblImportSeparator" runat="server" Text="Separador:" CssClass="labelForm maxWidth"></asp:Label>
                                                    <div class="componentFormMaxWidth">
                                                        <dx:ASPxTextBox ID="txtImportSeparator" runat="server" Width="30" ClientInstanceName="txtImportSeparatorClient">
                                                            <ClientSideEvents TextChanged="function(s,e){ hasChanges(true)}" />
                                                        </dx:ASPxTextBox>
                                                    </div>
                                                </div>
                                                <div class="divRow divImportType" id="divImportType" runat="server">
                                                    <asp:Label ID="lblImportType" runat="server" Text="Plantilla:" CssClass="labelForm maxWidth"></asp:Label>
                                                    <div class="componentFormMaxWidth">
                                                        <dx:ASPxComboBox runat="server" ID="cmbImportType" Width="250px" ClientInstanceName="cmbImportTypeClient">
                                                            <ClientSideEvents SelectedIndexChanged="function(s,e){hasChanges(true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                        </dx:ASPxComboBox>
                                                    </div>
                                                </div>
                                                <div class="divRow divFileTemplate" id="divFileTemplate" runat="server" clientidmode="Static" style="display: none">
                                                    <asp:Label ID="lblTemplate" runat="server" Text="Obtener fichero de formato de:" class="labelForm maxWidth"></asp:Label>
                                                    <div class="componentFormMaxWidth">
                                                        <dx:ASPxTextBox ID="txtfileTmp" runat="server" Width="300">
                                                            <ClientSideEvents TextChanged="function(s,e){ hasChanges(true)}" />
                                                        </dx:ASPxTextBox>
                                                    </div>
                                                </div>
                                                <div class="divRow">
                                                    <asp:Label ID="lblImportFileName" runat="server" Text="Obtener fichero de datos de:" CssClass="labelForm maxWidth"></asp:Label>
                                                    <div class="componentFormMaxWidth">
                                                        <dx:ASPxTextBox ID="txtfileOrig" runat="server" Width="300" ClientInstanceName="txtfileOrigClient">
                                                            <ClientSideEvents TextChanged="function(s,e){ hasChanges(true)}" />
                                                        </dx:ASPxTextBox>

                                                        <div id="divFileInfo">
                                                            <asp:Label ID="lblfileTmpInfo" runat="server" Text="Indique el nombre del archivo que contiene los datos a importar en la ruta configurada" Style="padding: 0px;" class="descriptionClassWithoutBorder"></asp:Label>
                                                            <br />
                                                            <asp:Label ID="lblfileTmpInfo2" runat="server" Text="Por ejemplo: fichero.xls, prefijo*.xls, prefijo[yyyyMMddHHmm].xls" Style="padding: 0px;" class="descriptionClassWithoutBorder"></asp:Label>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="divRow">
                                                    <div style="padding-left: 250px; display: none" class="componentFormMaxWidth">
                                                        <dx:ASPxCheckBox ID="ckFileBackup" runat="server" Text="Realizar una cópia de seguridad del fichero origen" CssClass="textClassWithoutBorder ">
                                                            <ValidationSettings ErrorDisplayMode="None" />
                                                            <ClientSideEvents CheckedChanged="function(s,e){ hasChanges(true);}" />
                                                        </dx:ASPxCheckBox>
                                                    </div>
                                                </div>
                                                <div class="divRow">
                                                    <table width="100%" border="0">
                                                        <tr>
                                                            <td align="center">
                                                                <dx:ASPxButton ID="btnActivateAutomaticImport" runat="server" AutoPostBack="False" CausesValidation="False" Text="Guardar y validar exportación automática" ToolTip=""
                                                                    HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat">
                                                                    <ClientSideEvents Click="function(s, e) { ShowCaptchaActivate(); return false; }" />
                                                                </dx:ASPxButton>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <!-- Panel2 Exportación -->
                                    <div id="div01" class="contentPanel" runat="server" name="menuPanel" style="display: none">
                                        <div class="panHeader2">
                                            <span style="">
                                                <asp:Label runat="server" ID="lblExportDescription" Text="Descripción"></asp:Label></span>
                                        </div>
                                        <br />
                                        <div class="panBottomMargin">
                                            <div class="divRow">
                                                <div class="componentForm">
                                                    <asp:Label ID="lblDatalinkBusinessExportDescription" runat="server" Text="" CssClass="textClassWithoutBorder"></asp:Label>
                                                    <br />
                                                    <asp:Label ID="lblDatalinkBusinessExportSharedDescription" runat="server" CssClass="textClassWithoutBorder" Text="Selecciona entre las distintas plantilla disponibles para obtener información que más se ajuste a tus necesidades."></asp:Label>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="panBottomMargin">
                                            <div class="panHeader2">
                                                <span style="">
                                                    <asp:Label runat="server" ID="lblExportSection" Text="Datos para la exportación manual"></asp:Label></span>
                                            </div>
                                            <br />
                                            <div class="panBottomMargin">
                                                <div class="divRow">
                                                    <table width="100%" style="margin-top: 30px; display: none" id="btnExportDiv" runat="server">
                                                        <tr>
                                                            <td align="center" style="padding: 20px;">
                                                                <dx:ASPxButton ID="btnExport" runat="server" AutoPostBack="False" CausesValidation="False" Text="Lanzar exportación..." ToolTip="Lanzar exportación..."
                                                                    HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat">
                                                                    <ClientSideEvents Click="function(s, e) { ShowExportWizard(false); return false; }" />
                                                                </dx:ASPxButton>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="panBottomMargin" id="rowAutomaticExport" style="display: none" runat="server">
                                            <div class="panHeader2">
                                                <span style="">
                                                    <asp:Label runat="server" ID="lblAutomaticExport" Text="Configurar exportación automática"></asp:Label></span>
                                            </div>
                                            <div class="panBottomMargin" style="margin-top: 30px;">
                                                <div class="divRow" margin="20px">
                                                    <div style="width: 99%">
                                                        <div class="jsGrid">
                                                            <asp:Label ID="lblAutomaticExportSchedule" runat="server" CssClass="jsGridTitle" Text="Planificaciones actuales"></asp:Label>
                                                            <div class="jsgridButton">
                                                                <dx:ASPxButton ID="btnAddNewSchedule" runat="server" AutoPostBack="False" CausesValidation="False" Text="Añadir" ToolTip="Añadir" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat">
                                                                    <Image Url="~/Base/Images/Grid/add.png"></Image>
                                                                    <ClientSideEvents Click="AddNewExport" />
                                                                </dx:ASPxButton>
                                                            </div>
                                                        </div>
                                                        <dx:ASPxGridView ID="GridExports" runat="server" AutoGenerateColumns="False" ClientInstanceName="GridExportsClient" KeyboardSupport="True" Width="100%" ClientSideEvents-BeginCallback="GridExports_BeginCallback" Theme="Robo">
                                                            <Settings ShowTitlePanel="False" VerticalScrollBarMode="Auto" UseFixedTableLayout="True" VerticalScrollableHeight="200" />
                                                            <SettingsBehavior AllowFocusedRow="false" />
                                                            <ClientSideEvents CustomButtonClick="GridExports_CustomButtonClick" EndCallback="GridExports_EndCallback" RowDblClick="GridExports_OnRowDblClick" FocusedRowChanged="GridExports_FocusedRowChanged" />
                                                            <SettingsCommandButton>
                                                                <DeleteButton Image-Url="~/Base/Images/Grid/remove.png" Image-ToolTip="" />
                                                                <UpdateButton Image-Url="~/Base/Images/Grid/save.png" Image-ToolTip="" />
                                                                <CancelButton Image-Url="~/Base/Images/Grid/cancel.png" Image-ToolTip="" />
                                                                <EditButton Image-Url="~/Base/Images/Grid/edit.png" Image-ToolTip="" />
                                                            </SettingsCommandButton>
                                                            <Styles>
                                                                <Cell Wrap="False" />
                                                            </Styles>
                                                            <SettingsPager Mode="ShowAllRecords" ShowEmptyDataRows="false">
                                                            </SettingsPager>
                                                        </dx:ASPxGridView>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <!-- Panel3 Plantillas -->   
                                    <div id="div02" class="contentPanel" runat="server" name="menuPanel" style="display: none">
                                        <div class="panHeader2">
                                            <span style="">
                                                <asp:Label runat="server" ID="lblExportGuideTemplateTitle" Text="Descripción"></asp:Label></span>
                                        </div>
                                        <br />
                                        <div class="panBottomMargin">
                                            <div class="divRow">
                                                <div class="componentForm">
                                                    <asp:Label ID="lblExportGuideTemplateInnerDescription" runat="server" Text="" CssClass="textClassWithoutBorder"></asp:Label>
                                                    <br />
                                                    <asp:Label ID="lblExportGuideTemplateDescription" runat="server" CssClass="textClassWithoutBorder" Text="Configure las diferentes plantillas de exportación."></asp:Label>
                                                </div>
                                            </div>
                                        </div>
										<!-- Este div es un formulario -->
										<div class="panBottomMargin">
											<div class="divRow" style="padding-top: 10px;">
												<div class="componentForm" style="width: 1280px">
													<div style="float: left">
														<asp:Label ID="lblAdvTemplateEdit" runat="server" Text="Tipo de plantilla:"></asp:Label>
													</div>
													<div style="float: left; padding-left: 10px;">
														<dx:ASPxComboBox runat="server" ID="cmbAdvTemplateType" Width="250px" NullText="_____" ClientInstanceName="cmbAdvTemplateTypeClient">
															<ClientSideEvents SelectedIndexChanged="function(s, e) { loadAdvTemplateNames(); }" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
														</dx:ASPxComboBox>
													</div>
													<div style="float: left; padding-left: 20px;">
														<asp:Label ID="lblTemplateName" runat="server" Text="Nombre de la plantilla:"></asp:Label>
													</div>
													<div style="float: left; padding-left: 10px;">
														<dx:ASPxComboBox runat="server" ID="cmbAdvTemplateName" Width="250px" NullText="_____" ClientInstanceName="cmbAdvTemplateNameClient">
															<ClientSideEvents SelectedIndexChanged="function(s, e) { loadAdvTemplateContent(); }" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
														</dx:ASPxComboBox>
													</div>
													<div style="float: left; margin: -4px; padding-left: 14px;">
														<dx:ASPxButton ID="btSaveChanges" runat="server" AutoPostBack="False" CausesValidation="False" Text="Guardar cambios" ToolTip="Guardar cambios"
															HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat">
															<ClientSideEvents Click="function(s, e) { saveCurrentTemplateFile(); }" />
														</dx:ASPxButton>
													</div>
													<div style="float: left; margin: -4px; padding-left: 10px;">
														<dx:ASPxButton ID="btNewDocument" runat="server" AutoPostBack="False" CausesValidation="False" Text="Guardar como..." ToolTip="Guardar como..."
															HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat">
															<ClientSideEvents Click="function(s, e) { duplicateCurrentTemplate(); }" />
														</dx:ASPxButton>
													</div>
												</div>
											</div>
										</div>

										<!-- Este div es un formulario -->
										<div class="panBottomMargin">
											<dx:ASPxSpreadsheet ID="advTemplate" runat="server" Width="100%" Height="570px" ActiveTabIndex="0" ClientInstanceName="advTemplateClient">
												<ClientSideEvents DocumentChanged="onDocumentChanged" EndSynchronization="onEndSynchronization" />
											</dx:ASPxSpreadsheet>
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

                                <!-- POPUP DE Log de importación -->
                                <dx:ASPxPopupControl ID="PopupImportLogsView" runat="server" AllowDragging="True" CloseAction="None" Modal="True" ClientInstanceName="PopupImportLogsViewClient"
                                    PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter" Height="390px" Width="540px"
                                    ShowHeader="False" ScrollBars="Auto" ShowPageScrollbarWhenModal="True" PopupAnimationType="None" BackColor="Transparent" ContentStyle-Paddings-Padding="0px" Border-BorderColor="Transparent" ShowShadow="false">
                                    <ContentCollection>
                                        <dx:PopupControlContentControl ID="PopupSaveViewControlContent" runat="server">
                                            <dx:ASPxPanel ID="ASPxPanel2" runat="server" Width="0px" Height="0px">
                                                <PanelCollection>
                                                    <dx:PanelContent ID="PopupSaveViewPanelContent" runat="server">
                                                        <div class="bodyPopupExtended" style="table-layout: fixed; height: 350px; width: 500px;">
                                                            <div class="panHeader2">
                                                                <span style="">
                                                                    <asp:Label ID="lblLogViewTitle" runat="server" Text="Log de la última ejecución" /></span>
                                                            </div>
                                                            <div>
                                                                <dx:ASPxMemo ID="txtLogImport" runat="server" CssClass="editTextFormat" Rows="15" Width="100%" ReadOnly="True">
                                                                </dx:ASPxMemo>
                                                            </div>

                                                            <!-- BOTONES -->
                                                            <table style="margin-left: auto;">
                                                                <tr>
                                                                    <td>
                                                                        <dx:ASPxButton ID="PopupLogsViewClose" runat="server" AutoPostBack="false" CausesValidation="False" Text="${Button.Close}" ToolTip="${Button.Close}"
                                                                            HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                                                            <ClientSideEvents Click="function(s, e) { PopupImportLogsViewClient.Hide(); }" />
                                                                        </dx:ASPxButton>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </div>
                                                    </dx:PanelContent>
                                                </PanelCollection>
                                            </dx:ASPxPanel>
                                        </dx:PopupControlContentControl>
                                    </ContentCollection>
                                </dx:ASPxPopupControl>

                            </dx:PanelContent>
                        </PanelCollection>
                    </dx:ASPxCallbackPanel>
                </div>
            </div>
        </div>
    </div>

    <!-- POPUP DE Log de importación -->
    <dx:ASPxPopupControl ID="PopupExportLogsView" runat="server" AllowDragging="True" CloseAction="None" Modal="True" ClientInstanceName="PopupExportLogsViewClient"
        PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter" Height="390px" Width="540px"
        ShowHeader="False" ScrollBars="Auto" ShowPageScrollbarWhenModal="True" PopupAnimationType="None" BackColor="Transparent" ContentStyle-Paddings-Padding="0px" Border-BorderColor="Transparent" ShowShadow="false">
        <ContentCollection>
            <dx:PopupControlContentControl ID="PopupControlContentControl1" runat="server">
                <dx:ASPxPanel ID="ASPxPanel1" runat="server" Width="0px" Height="0px">
                    <PanelCollection>
                        <dx:PanelContent ID="PanelContent2" runat="server">
                            <div class="bodyPopupExtended" style="table-layout: fixed; height: 350px; width: 500px;">
                                <div class="panHeader2">
                                    <span style="">
                                        <asp:Label ID="lblLogExportViewTitle" runat="server" Text="Log de la última ejecución" /></span>
                                </div>
                                <div>
                                    <dx:ASPxMemo ID="txtLogExport" ClientInstanceName="txtLogExportClient" runat="server" CssClass="editTextFormat" Rows="15" Width="100%" ReadOnly="True">
                                    </dx:ASPxMemo>
                                </div>

                                <!-- BOTONES -->
                                <table style="margin-left: auto;">
                                    <tr>
                                        <td>
                                            <dx:ASPxButton ID="PopupExportLogsViewClose" runat="server" AutoPostBack="false" CausesValidation="False" Text="${Button.Close}" ToolTip="${Button.Close}"
                                                HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                                <ClientSideEvents Click="function(s, e) { PopupExportLogsViewClient.Hide(); }" />
                                            </dx:ASPxButton>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </dx:PanelContent>
                    </PanelCollection>
                </dx:ASPxPanel>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>

    <!-- POPUP DE Edición creación de exportaciones -->
    <dx:ASPxPopupControl ID="ExportScheduleEdit" runat="server" AllowDragging="True" CloseAction="None" Modal="True" ClientInstanceName="ExportScheduleEditClient"
        PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter" Height="850px" Width="1064px"
        ShowHeader="False" ScrollBars="Auto" ShowPageScrollbarWhenModal="True" PopupAnimationType="None" BackColor="Transparent" ContentStyle-Paddings-Padding="0px" Border-BorderColor="Transparent" ShowShadow="false">
        <ContentCollection>
            <dx:PopupControlContentControl ID="PopupControlContentControl3" runat="server">
                <dx:ASPxPanel ID="ASPxPanel4" runat="server" Width="0px" Height="0px">
                    <PanelCollection>
                        <dx:PanelContent ID="PanelContent4" runat="server">
                            <div class="bodyPopupExtended" style="table-layout: fixed; height: 745px; width: 1024px;">

                                <dx:ASPxCallbackPanel ID="CallbackPopupOperations" runat="server" Width="100%" Height="100%" ClientInstanceName="CallbackPopupOperationsClient">
                                    <SettingsLoadingPanel Enabled="false" />
                                    <ClientSideEvents EndCallback="CallbackPopupOperations_EndCallBack" />
                                    <PanelCollection>
                                        <dx:PanelContent ID="PanelContent5" runat="server">
                                            <dx:ASPxHiddenField ID="hdnScheduleId" runat="server" ClientInstanceName="hdnScheduleIdClient" />
                                            <asp:HiddenField ID="hdnEmployeesSelected" runat="server" Value="0" />
                                            <asp:HiddenField ID="hdnEmployees" runat="server" Value="" />
                                            <asp:HiddenField ID="hdnFilter" runat="server" Value="" />
                                            <asp:HiddenField ID="hdnFilterUser" runat="server" Value="" />
                                            <table>
                                                <tr>
                                                    <td>
                                                        <div class="panHeader2">
                                                            <span style="">
                                                                <asp:Label runat="server" ID="lblExportParameters" Text="Indique los valores de la planificación"></asp:Label></span>
                                                        </div>
                                                        <div>
                                                            <div id="divScheduleName" class="divRow" runat="server">
                                                                <asp:Label ID="lblScheduleName" runat="server" Text="Nombre de la planificación:" CssClass="labelForm maxWidth"></asp:Label>
                                                                <div class="componentFormMaxWidth">
                                                                    <div style="float: left">
                                                                        <dx:ASPxTextBox ID="txtScheduleName" runat="server" Width="250px" ClientInstanceName="txtScheduleNameClient">
                                                                            <ClientSideEvents TextChanged="function(s,e){ }" />
                                                                        </dx:ASPxTextBox>
                                                                    </div>
                                                                    <div style="float: left">
                                                                        <asp:Label ID="lblActive" runat="server" Text="Activa:" CssClass="labelForm" Style="width: 60px"></asp:Label>
                                                                        <div class="componentFormMaxWidth">
                                                                            <dx:ASPxCheckBox ID="ckScheduleActive" runat="server" Width="30" ClientInstanceName="ckScheduleActiveClient">
                                                                                <ClientSideEvents ValueChanged="function(s,e){ }" />
                                                                            </dx:ASPxCheckBox>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="divRow">
                                                                <asp:Label ID="lblExportType" runat="server" Text="Plantilla:" CssClass="labelForm maxWidth"></asp:Label>
                                                                <div class="componentFormMaxWidth">
                                                                    <div style="float: left">
                                                                        <dx:ASPxComboBox runat="server" ID="cmbExportType" Width="250px" ClientInstanceName="cmbExportTypeClient">
                                                                            <ClientSideEvents SelectedIndexChanged="function(s,e){ profileChanged(s,e); }" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                        </dx:ASPxComboBox>
                                                                    </div>
                                                                    <div style="float: left; padding-left: 20px; max-width: 425px; margin-top: -9px;">
                                                                        <div id="divApplyLockDate" runat="server" style="clear: both">
                                                                            <dx:ASPxCheckBox ID="ckApplyLockDate" runat="server" MaxLength="" Text="Aplicar fecha de cierre por empleado al finalizar la explortación" ClientInstanceName="ckApplyLockDateClient">
                                                                                <ClientSideEvents CheckedChanged="function(s,e){ }" />
                                                                            </dx:ASPxCheckBox>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div id="divFormatExport" class="divRow" runat="server">
                                                                <asp:Label ID="lblExportFormat" runat="server" Text="Tipo de formato:" CssClass="labelForm maxWidth"></asp:Label>
                                                                <div class="componentFormMaxWidth">
                                                                    <div style="float: left">
                                                                        <dx:ASPxComboBox runat="server" ID="cmbFormatExport" Width="250px" ClientInstanceName="cmbFormatExportClient">
                                                                            <ClientSideEvents SelectedIndexChanged="function(s,e){cmbIsASCII();}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                        </dx:ASPxComboBox>
                                                                    </div>
                                                                    <div id="divSeparator" class="divSeparator" style="float: left; padding-left: 20px" runat="server">
                                                                        <asp:Label ID="lblExportSeparator" runat="server" Text="Separador:" CssClass="labelForm" Style="width: 60px"></asp:Label>
                                                                        <div class="componentFormMaxWidth">
                                                                            <dx:ASPxTextBox ID="txtExportSeparator" runat="server" Width="30" ClientInstanceName="txtSeparatorClient">
                                                                                <ClientSideEvents TextChanged="function(s,e){ }" />
                                                                            </dx:ASPxTextBox>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>

                                                            <div class="divRow">
                                                                <asp:Label ID="lblExportFileName" runat="server" Text="Nombre del archivo de datos" CssClass="labelForm maxWidth"></asp:Label>
                                                                <div class="componentFormMaxWidth">
                                                                    <dx:ASPxTextBox ID="txtExportfileOrig" runat="server" Width="300" ClientInstanceName="txtExportfileOrigClient">
                                                                        <ClientSideEvents TextChanged="function(s,e){ }" />
                                                                    </dx:ASPxTextBox>

                                                                    <div id="divFileExportInfo">
                                                                        <asp:Label ID="lblFileDescription" runat="server" Text="Indique el nombre destino del archivo que desea generar en la ruta configurada. El fichero contendrá una máscara de tiempo." Style="padding: 0px;" class="descriptionClassWithoutBorder"></asp:Label>
                                                                        <br />
                                                                        <asp:Label ID="lblFileDescription2" runat="server" Text="Por ejemplo: fichero.xls será fichero[yyyyMMddHHmmss].xls" Style="padding: 0px;" class="descriptionClassWithoutBorder"></asp:Label>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="divRow">
                                                                <asp:Label ID="lblEmployeeSelector" runat="server" Text="Seleccione los empleados deseados" CssClass="labelForm maxWidth"></asp:Label>
                                                                <div class="componentFormMaxWidth">
                                                                    <div>
                                                                        <div style="float: left">
                                                                            <dx:ASPxTextBox ID="txtSelectedEmployees" runat="server" Width="300" ClientReadOnly="true" ClientInstanceName="txtSelectedEmployeesClient">
                                                                                <ClientSideEvents TextChanged="function(s,e){ }" />
                                                                            </dx:ASPxTextBox>
                                                                        </div>
                                                                        <div style="float: left; padding-left: 20px;">
                                                                            <dx:ASPxButton ID="btnOpenPopupSelectorEmployees" runat="server" AutoPostBack="False" CausesValidation="False" Text="Seleccionar" ToolTip="Empleados..." HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat" ClientInstanceName="btnOpenPopupSelectorEmployeesClient">
                                                                                <Image Url="~/Scheduler/Images/EmployeeSelector16.png"></Image>
                                                                                <ClientSideEvents Click="btnOpenPopupSelectorEmployeesClient_Click" />
                                                                            </dx:ASPxButton>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="divRow">
                                                                <asp:Label ID="lblExportPeriod" runat="server" Text="Indique el periodo a exportar" CssClass="labelForm maxWidth"></asp:Label>
                                                                <div class="componentFormMaxWidth">
                                                                    <roUserControls:roOptSchedulePeriod ID="optSchedulePeriod" runat="server" />
                                                                </div>
                                                            </div>
                                                            <div class="divRow">
                                                                <asp:Label ID="lblScheduleDefinition" runat="server" Text="Indique cada cuanto quiere ejecutar la exportación" CssClass="labelForm maxWidth"></asp:Label>
                                                                <div class="componentFormMaxWidth">
                                                                    <div style="width: 500px; border: 1px solid #acacac">
                                                                        <roUserControls:roOptSchedule2 runat="server" ID="optSchedule1"></roUserControls:roOptSchedule2>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <%--<div class="divRow">
                                                                <table width="100%" border="0">
                                                                    <tr>
                                                                        <td align="center">
                                                                            <dx:ASPxButton ID="btnActivateAutomaticExport" runat="server" AutoPostBack="False" CausesValidation="False" Text="Guardar y validar importación automática" ToolTip=""
                                                                                HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat">
                                                                                <ClientSideEvents Click="function(s, e) { ShowCaptchaActivateExport(); return false; }" />
                                                                            </dx:ASPxButton>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </div>--%>
                                                        </div>
                                                    </td>
                                                </tr>
                                            </table>
                                            <!-- BOTONES -->
                                            <table style="margin-left: auto; padding-right: 48px">
                                                <tr>
                                                    <td>
                                                        <dx:ASPxButton ID="btnAccept" ClientInstanceName="btnAcceptClient" runat="server" AutoPostBack="False" CausesValidation="False" Text="Guardar" ToolTip="Guardar"
                                                            HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                                            <ClientSideEvents Click="AcceptExportScheduleClick" />
                                                        </dx:ASPxButton>
                                                    </td>
                                                    <td>
                                                        <dx:ASPxButton ID="btnCancel" ClientInstanceName="btnCancelClient" runat="server" AutoPostBack="False" CausesValidation="False" Text="Cancelar" ToolTip="Cancelar"
                                                            HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                                            <ClientSideEvents Click="CancelExportScheduleClick" />
                                                        </dx:ASPxButton>
                                                    </td>
                                                </tr>
                                            </table>
                                        </dx:PanelContent>
                                    </PanelCollection>
                                </dx:ASPxCallbackPanel>
                            </div>
                        </dx:PanelContent>
                    </PanelCollection>
                </dx:ASPxPanel>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>

    <!-- POPUP DEL SELECTOR DE EMPLEADOS -->
    <dx:ASPxPopupControl ID="PopupSelectorEmployees" runat="server" AllowDragging="True" CloseAction="None" Modal="True" ClientInstanceName="PopupSelectorEmployeesClient" ClientSideEvents-PopUp="PopupSelectorEmployeesClient_PopUp"
        PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter" Height="500px" Width="800px"
        ShowHeader="False" ScrollBars="Auto" ShowPageScrollbarWhenModal="True" PopupAnimationType="None" BackColor="Transparent" ContentStyle-Paddings-Padding="0px" Border-BorderColor="Transparent" ShowShadow="false">
        <ContentCollection>
            <dx:PopupControlContentControl ID="PopupControlContentControl2" runat="server">
                <dx:ASPxPanel ID="ASPxPanel3" runat="server" Width="0px" Height="0px">
                    <PanelCollection>
                        <dx:PanelContent ID="PanelContent3" runat="server">
                            <div class="bodyPopupExtended" style="table-layout: fixed; height: 460px; width: 775px;">
                                <table id="tbPopupFrame" cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td valign="top">
                                            <iframe id="GroupSelectorFrame" runat="server" style="background-color: Transparent;" height="420" width="775" scrolling="no"
                                                frameborder="0" marginheight="0" marginwidth="0" src="" />
                                        </td>
                                    </tr>
                                    <tr style="height: 35px;">
                                        <td align="right">
                                            <table>
                                                <tr>
                                                    <td>
                                                        <dx:ASPxButton ID="btnPopupSelectorEmployeesAccept" runat="server" AutoPostBack="False" CausesValidation="False" Text="${Button.Accept}" ToolTip="${Button.Accept}" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                                            <ClientSideEvents Click="btnPopupSelectorEmployeesAcceptClient_Click" />
                                                        </dx:ASPxButton>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </dx:PanelContent>
                    </PanelCollection>
                </dx:ASPxPanel>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>

    <dx:ASPxPopupControl ID="PopupNewTemplateName" runat="server" AllowDragging="True" CloseAction="None" Modal="True" ClientInstanceName="PopupNewTemplateNameClient"
    PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter" Height="225px" Width="420px"
    ShowHeader="False" ScrollBars="Auto" ShowPageScrollbarWhenModal="True" PopupAnimationType="None" BackColor="Transparent" ContentStyle-Paddings-Padding="0px" Border-BorderColor="Transparent" ShowShadow="false">
    <ContentCollection>
        <dx:PopupControlContentControl ID="PopupControlContentControl4" runat="server">
            <dx:ASPxPanel ID="ASPxPanel5" runat="server" Width="0px" Height="0px">
                <PanelCollection>
                    <dx:PanelContent ID="PanelContent6" runat="server">
                        <div class="bodyPopupExtended" style="table-layout: fixed; height: 185px; width: 375px;">
                            <table width="100%">
                                <tr>

                                    <td style="padding-bottom: 10px;" height="20px" valign="top">
                                        <div class="panHeader2">
                                            <span style="">
                                                <asp:Label ID="lblTitle" runat="server" Text="Guardar como..."></asp:Label>
                                            </span>
                                        </div>
                                    </td>
                                </tr>
                                <tr valign="top">
                                    <td>
                                        <table>
                                            <tr>
                                                <td valign="top" style="padding-left: 15px; padding-right: 15px;">
                                                    <img style="width: 32px" alt="" id="Img6" src="~/Base/Images/logovtl.ico" runat="server" />
                                                </td>
                                                <td align="left" valign="middle">
                                                    <asp:Label ID="lblDescription1" runat="server" CssClass="editTextFormat" Text="Introduzca el nombre de la plantilla"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr valign="top">
                                    <td>
                                        <table style="padding-left: 95px;">
                                            <tr>
                                                <td>
                                                    <table style="width: 100%" cellspacing="0" cellpadding="0" border="0">
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="lblName" runat="server" CssClass="editTextFormat" Text="Nombre"></asp:Label>
                                                            </td>
                                                            <td style="padding-left: 5px">
                                                                <dx:ASPxTextBox ID="newObjectName" runat="server" ClientInstanceName="newObjectName_Client" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <table style="margin-left: auto;">
                                <tr>
                                    <td>
                                        <dx:ASPxButton ID="ASPxButton1" ClientInstanceName="btnAcceptClient" runat="server" AutoPostBack="False" CausesValidation="False" Text="Guardar" ToolTip="Guardar"
                                            HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                            <ClientSideEvents Click="AcceptTemplateNameClick" />
                                        </dx:ASPxButton>
                                    </td>
                                    <td>
                                        <dx:ASPxButton ID="ASPxButton2" ClientInstanceName="btnCancelClient" runat="server" AutoPostBack="False" CausesValidation="False" Text="Cancelar" ToolTip="Cancelar"
                                            HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                            <ClientSideEvents Click="CancelTemplateNameClick" />
                                        </dx:ASPxButton>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </dx:PanelContent>
                </PanelCollection>
            </dx:ASPxPanel>
        </dx:PopupControlContentControl>
    </ContentCollection>
</dx:ASPxPopupControl>

    <!-- POPUP NEW OBJECT -->
    <dx:ASPxPopupControl ID="CaptchaObjectPopup" runat="server" AllowDragging="False" CloseAction="None" Modal="True" ContentUrl="~/Base/Popups/GenericCaptchaValidator.aspx"
        PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter" ModalBackgroundStyle-Opacity="0" Width="500" Height="320"
        ShowHeader="False" ScrollBars="Auto" ShowPageScrollbarWhenModal="True" ClientInstanceName="CaptchaObjectPopup_Client" PopupAnimationType="None" BackColor="Transparent" ContentStyle-Paddings-Padding="0px" Border-BorderColor="Transparent" ShowShadow="false">
        <SettingsLoadingPanel Enabled="false" />
    </dx:ASPxPopupControl>



    <script language="javascript" type="text/javascript">

        function resizeTreeDatalink() {
            try {
                var ctlPrefix = "<%= roTreesDatalink.ClientID %>";
                eval(ctlPrefix + "_resizeTrees();");
            }
            catch (e) {
                showError("resizeTreeDatalink", e);
            }
        }

        function resizeFrames() {
            var divMainBodyHeight = $("#divMainBody").outerHeight(true);
            var divHeight = 0;
            if (divMainBodyHeight < 525) {
                divHeight = 525 - $("#divTabInfo").outerHeight(true);
            } else {
                divHeight = divMainBodyHeight - $("#divTabInfo").outerHeight();
            }

            $("#divTabData").height(divHeight - 10);

            var divTreeHeight = $("#divTree").height();
            $("#ctlTreeDiv").height(divTreeHeight);
        }

        window.onresize = function () {
            resizeFrames();
            resizeTreeDatalink();
        }
    </script>
</asp:Content>