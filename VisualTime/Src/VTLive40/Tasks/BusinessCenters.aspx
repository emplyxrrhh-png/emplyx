<%@ Page Language="VB" MasterPageFile="~/Base/mastEmp.master" AutoEventWireup="false" Inherits="VTLive40.Tasks_BusinessCenters" Title="${BusinessCenters}" CodeBehind="BusinessCenters.aspx.vb" %>

<%@ Register Src="~/Tasks/WebUserForms/frmFilterBusinessCenters.ascx" TagName="frmFilterBusinessCenters" TagPrefix="roForms" %>

<asp:Content ID="cMainBody" ContentPlaceHolderID="contentMainBody" runat="Server">

    <script language="javascript" type="text/javascript">

        function PageBase_Load() {
            resizeFrames();
            resizeTreeBusinessCenters();

            if (document.getElementById('<%= noRegs.ClientID %>').value != '') {
                setTimeout(function () { newBusinessCenter(); }, 200);
            }
        }

        //Agregar nueva fila en el grid de incidencias
        function AddNewZone(s, e) {
            var grid = ASPxClientGridView.Cast("GridZonesClient");
            hasChanges(true);
            grid.AddNewRow();
        }

        function GridZones_BeginCallback(e, c) {

        }

        function GridZones_EndCallback(s, e) {
            if (s.IsEditing()) {
                hasChanges(true);
            } else {
                if (s.cpAction == "ROWINSERTING" || s.cpAction == "ROWUPDATING" || s.cpAction == "ROWDELETE") {
                    hasChanges(true);
                }
            }
        }

        function GridZones_OnRowDblClick(s, e) {

        }

        function GridZones_FocusedRowChanged(s, e) {

        }

        function DeleteZones(IdRow) {
            grid = ASPxClientGridView.Cast("GridZonesClient");
            grid.DeleteRow(IdRow);
        }
    </script>

    <div id="divModalBgDisabled" class="modalBackground" style="position: absolute; left: 0px; top: 0px; z-index: 990; width: 1680px; height: 900px; display: none;"></div>
    <input type="hidden" runat="server" id="hdnModeEdit" value="" />
    <input type="hidden" runat="server" id="noRegs" value="" />

    <input type="hidden" id="msgHasChanges" value="" runat="server" clientidmode="Static" />
    <input type="hidden" id="msgHasErrors" value="" runat="server" clientidmode="Static" />

    <div id="divMainBody">
        <!-- TAB SUPERIOR -->
        <div id="divTabInfo" class="divDataCells">
            <div style="min-height: 10px"></div>
            <div id="divBusinessCenters" class="blackRibbonTitle">
            </div>
            <div style="min-height: 10px"></div>
        </div>
        <!-- FIN TAB SUPERIOR -->

        <!-- ARBOL Y DETALLE -->
        <div id="divTabData" class="divDataCells">

            <div id="divTree" class="treeSize">
                <div id="ctlTreeDiv" style="height: 500px;">
                    <rws:roTreesSelector ID="roTreesBusinessCenters" runat="server" ShowEmployeeFilters="false" PrefixTree="roTreesBusinessCenters"
                        Tree1Visible="true" Tree1MultiSel="false" Tree1ShowOnlyGroups="false" Tree1Function="cargaNodo" Tree1ImagePath="images/BusinessCentersSelector" Tree1SelectorPage="../../Tasks/BusinessCentersSelectorData.aspx"
                        ShowTreeCaption="true" FiltersVisible="111"></rws:roTreesSelector>
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
                                    <div id="div00" class="contentPanel" style="display: none;" runat="server">
                                        <!-- Este div es el header -->
                                        <div class="panBottomMargin">
                                            <div class="panHeader2 panBottomMargin">
                                                <span class="panelTitleSpan">
                                                    <asp:Label runat="server" ID="lblBusinessCentersTitleGeneral" Text="General"></asp:Label>
                                                </span>
                                            </div>
                                            <!-- La descripción es opcional -->
                                            <div class="panelHeaderContent">
                                                <div class="panelDescriptionImage">
                                                    <img alt="" src="<%=Me.Page.ResolveUrl("~/Tasks/Images/BusinessCenters48.png")%>" />
                                                </div>
                                                <div class="panelDescriptionText">
                                                    <asp:Label ID="lblBusinessCentersTitleGeneralDesc" runat="server" Text="Datos especificos los centros de negocio."></asp:Label>
                                                </div>
                                            </div>
                                        </div>

                                        <!-- Este div es un formulario -->
                                        <div class="panBottomMargin">
                                            <div class="divRow">
                                                <div class="divRowDescription">
                                                    <asp:Label ID="lblNameDescription" runat="server" Text="Nombre del centro de negocio"></asp:Label>
                                                </div>
                                                <asp:Label ID="lblName" runat="server" Text="Nombre:" CssClass="labelForm"></asp:Label>
                                                <div class="componentForm">
                                                    <dx:ASPxTextBox ID="txtName" runat="server" MaxLength="150" Width="300px" ClientInstanceName="txtName_Client" NullText="_____">
                                                        <ClientSideEvents Validation="LengthValidation" TextChanged="function(s,e){checkBusinessCenterEmptyName(s.GetValue());}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                        <ValidationSettings SetFocusOnError="True">
                                                            <RequiredField IsRequired="True" ErrorText="(*)" />
                                                        </ValidationSettings>
                                                    </dx:ASPxTextBox>
                                                </div>
                                            </div>

                                            <div class="divRow">
                                                <div class="divRowDescription">
                                                    <asp:Label ID="lblDescLabAgreeDescription" runat="server" Text="Descripción del centro de negocio"></asp:Label>
                                                </div>
                                                <asp:Label ID="lblDescLabAgree" runat="server" Text="Descripción:" CssClass="labelForm"></asp:Label>
                                                <div class="componentForm">
                                                    <dx:ASPxMemo ID="txtDescription" runat="server" Rows="4" Width="100%" Height="40">
                                                        <ClientSideEvents TextChanged="function(s,e){ hasChanges(true)}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                    </dx:ASPxMemo>
                                                </div>
                                            </div>
                                            <roForms:frmFilterBusinessCenters runat="server" ID="frmFilterBusinessCenters" />
                                        </div>
                                        <!-- Campos ficha -->
                                        <div class="panBottomMargin">
                                            <div class="panHeader2 panBottomMargin">
                                                <span class="panelTitleSpan">
                                                    <asp:Label runat="server" ID="lblFieldsBusinessCenter" Text="Campos ficha del centro de coste"></asp:Label>
                                                </span>
                                            </div>
                                            <div class="jsGrid">
                                                <asp:Label ID="lblFieldsBusinessCenterDescr" runat="server" CssClass="jsGridTitle" Text="Campos ficha del centro de coste"></asp:Label>
                                                <div id="btn1FieldsBusinessCenter" runat="server" class="jsgridButton">
                                                    <div class="btnFlat">
                                                        <a href="javascript: void(0)" id="editFieldsGrid" runat="server" onclick="">
                                                            <span class="btnIconEdit"></span>
                                                            <asp:Label ID="lblFieldsBusinessCenterEdit" runat="server" Text="Editar"></asp:Label>
                                                        </a>
                                                    </div>
                                                </div>
                                                <div id="btn3FieldsBusinessCenter" runat="server" class="jsgridButton" visible="false">
                                                    <div class="btnFlat">
                                                        <a href="javascript: void(0)" id="cancelFieldsGrid" runat="server" onclick="">
                                                            <span class="btnIconCancel"></span>
                                                            <asp:Label ID="lblFieldsBusinessCenterCancel" runat="server" Text="Cancelar"></asp:Label>
                                                        </a>
                                                    </div>
                                                </div>
                                                <div id="btn2FieldsBusinessCenter" runat="server" class="jsgridButton" visible="false">
                                                    <div class="btnFlat">
                                                        <a href="javascript: void(0)" id="saveFieldsGrid" runat="server" onclick="">
                                                            <span class="btnIconSave"></span>
                                                            <asp:Label ID="lblFieldsBusinessCenterSave" runat="server" Text="Guardar"></asp:Label>
                                                        </a>
                                                    </div>
                                                </div>
                                            </div>
                                            <div id="divBusinessCenterFields" runat="server" class="jsGridContent">
                                                <!-- Campos de la ficha del centro de costeactual -->
                                            </div>
                                        </div>
                                        <!-- Panel Estado -->
                                        <div class="panBottomMargin">
                                            <!-- Este div es el header -->
                                            <div class="panHeader2 panBottomMargin">
                                                <span class="panelTitleSpan">
                                                    <asp:Label runat="server" ID="lblBusinessCentersTitleStatus" Text="Estado"></asp:Label>
                                                </span>
                                            </div>
                                            <!-- La descripción es opcional -->
                                            <div class="panelHeaderContent">
                                                <div class="panelDescriptionText">
                                                    <asp:Label ID="lblBusinessCentersTitleStatusDesc" runat="server" Text="Indique el estado actual del ${BusinessCenter}."></asp:Label>
                                                </div>
                                            </div>
                                        </div>
                                        <!-- Este div es un formulario -->
                                        <div class="panBottomMargin">
                                            <table style="width: 100%;">
                                                <tr>
                                                    <td style="padding-left: 5px;">
                                                        <roUserControls:roOptionPanelClient ID="opActive" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Checked="False" Enabled="True" CConClick="hasChanges(true);">
                                                            <Title>
                                                                <asp:Label ID="lbloptActive" runat="server" Text="Activo"></asp:Label>
                                                            </Title>
                                                            <Description>
                                                                <asp:Label ID="lbloptActiveDesc" runat="server" Text="El ${BusinessCenter} está habilitado para que tanto los ${Employees} como los supervisores lo pueden utilizar."></asp:Label>
                                                            </Description>
                                                            <Content>
                                                            </Content>
                                                        </roUserControls:roOptionPanelClient>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="padding-left: 5px; padding-top: 5px">
                                                        <roUserControls:roOptionPanelClient ID="opNoActive" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Checked="False" Enabled="True" CConClick="hasChanges(true);">
                                                            <Title>
                                                                <asp:Label ID="lbloptNoActive" runat="server" Text="Inactivo"></asp:Label>
                                                            </Title>
                                                            <Description>
                                                                <asp:Label ID="lbloptNoActiveDesc" runat="server" Text="El ${BusinessCenter} está deshabilitado, nadie podrá fichar en él, ni utilizarlo en la gestión diaria."></asp:Label>
                                                            </Description>
                                                            <Content>
                                                            </Content>
                                                        </roUserControls:roOptionPanelClient>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="padding-left: 5px; padding-top: 5px"></td>
                                                </tr>
                                            </table>
                                        </div>
                                    </div>

                                    <!-- Zonas -->
                                    <div id="div01" class="contentPanel" style="display: none;" runat="server">
                                        <!-- Este div es el header -->
                                        <div class="panBottomMargin">
                                            <div class="panHeader2 panBottomMargin">
                                                <span class="panelTitleSpan">
                                                    <asp:Label runat="server" ID="lblZonesTitleStatus" Text="Zonas autorizadas"></asp:Label>
                                                </span>
                                            </div>
                                            <!-- La descripción es opcional -->
                                            <div class="panelHeaderContent">
                                                <div class="panelDescriptionText">
                                                    <asp:Label ID="lblZonesTitleStatusDesc" runat="server" Text="Indique las zona en las que se puede fichar el ${BusinessCenter}."></asp:Label>
                                                </div>
                                            </div>
                                        </div>

                                        <!-- Este div es un formulario -->
                                        <div class="panBottomMargin">
                                            <div class="divRow">
                                                <table style="width: 100%;">
                                                    <tr>
                                                        <td style="padding-left: 5px;">
                                                            <roUserControls:roOptionPanelClient ID="opAllZones" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Checked="False" Enabled="True" CConClick="hasChanges(true);">
                                                                <Title>
                                                                    <asp:Label ID="lblopAllZones" runat="server" Text="Se puede fichar en todas las zonas"></asp:Label>
                                                                </Title>
                                                                <Description>
                                                                    <asp:Label ID="lblopAllZonesDesc" runat="server" Text="El ${BusinessCenter} está habilitado para que los ${Employees} puedan fichar en él en cualquier zona."></asp:Label>
                                                                </Description>
                                                                <Content>
                                                                </Content>
                                                            </roUserControls:roOptionPanelClient>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding-left: 5px; padding-top: 5px">
                                                            <roUserControls:roOptionPanelClient ID="opSelectedZones" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Checked="False" Enabled="True" CConClick="hasChanges(true);">
                                                                <Title>
                                                                    <asp:Label ID="lblopSelectedZones" runat="server" Text="Se puede fichar en las zonas seleccionadas"></asp:Label>
                                                                </Title>
                                                                <Description>
                                                                    <asp:Label ID="lblopSelectedZonesDesc" runat="server" Text="El ${BusinessCenter} está habilitado para que los ${Employees} puedan fichar únicamente en las zonas seleccionadas."></asp:Label>
                                                                </Description>
                                                                <Content>
                                                                    <div class="jsGrid">
                                                                        <asp:Label ID="lblZonesCaption" runat="server" CssClass="jsGridTitle" Text="Zonas autorizadas"></asp:Label>
                                                                        <div class="jsgridButton">
                                                                            <dx:ASPxButton ID="btnAddNewZone" runat="server" AutoPostBack="False" CausesValidation="False" Text="Añadir" ToolTip="Añadir" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat">
                                                                                <Image Url="~/Base/Images/Grid/add.png"></Image>
                                                                                <ClientSideEvents Click="AddNewZone" />
                                                                            </dx:ASPxButton>
                                                                        </div>
                                                                    </div>
                                                                    <div class="jsGridContent">
                                                                        <dx:ASPxGridView ID="GridZones" runat="server" AutoGenerateColumns="False" ClientInstanceName="GridZonesClient" KeyboardSupport="True" Width="100%" ClientSideEvents-BeginCallback="GridZones_BeginCallback">
                                                                            <Settings ShowTitlePanel="False" VerticalScrollBarMode="Auto" UseFixedTableLayout="True" VerticalScrollableHeight="200" />
                                                                            <ClientSideEvents EndCallback="GridZones_EndCallback" RowDblClick="GridZones_OnRowDblClick" FocusedRowChanged="GridZones_FocusedRowChanged" />
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
                                                                            <SettingsPager Mode="ShowAllRecords" ShowEmptyDataRows="false">
                                                                            </SettingsPager>
                                                                        </dx:ASPxGridView>
                                                                    </div>
                                                                </Content>
                                                            </roUserControls:roOptionPanelClient>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding-left: 5px; padding-top: 5px"></td>
                                                    </tr>
                                                </table>
                                            </div>
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

    <script language="javascript" type="text/javascript">

        function resizeTreeBusinessCenters() {
            try {
                var ctlPrefix = "<%= roTreesBusinessCenters.ClientID %>";
                eval(ctlPrefix + "_resizeTrees();");
            }
            catch (e) {
                showError("resizeTreeBusinessCenters", e);
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
            resizeTreeBusinessCenters();
        }
    </script>
</asp:Content>