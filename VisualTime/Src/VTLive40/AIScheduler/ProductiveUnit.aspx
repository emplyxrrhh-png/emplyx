<%@ Page Language="VB" MasterPageFile="~/Base/mastEmp.master" AutoEventWireup="true" Inherits="VTLive40.ProductiveUnit" Title="${ProductiveUnit}" EnableEventValidation="false" CodeBehind="ProductiveUnit.aspx.vb" %>

<%@ Register Src="~/Base/WebUserControls/roCalendar/roCalendar.ascx" TagName="roCalendar" TagPrefix="roForms" %>

<asp:Content ID="cMainBody" ContentPlaceHolderID="contentMainBody" runat="Server">

    <script language="javascript" type="text/javascript">

        function PageBase_Load() {
            resizeFrames();
            resizeTreeProductiveUnit();
            //PPR desactivado temporalmente NO ELIMINAR--> loadInitialPageValues();
        }
    </script>

    <input type="hidden" runat="server" id="noRegs" value="" />
    <input type="hidden" id="msgHasChanges" value="" runat="server" clientidmode="Static" />
    <input type="hidden" id="msgHasErrors" value="" runat="server" clientidmode="Static" />

    <div id="divMainBody">
        <!-- TAB SUPERIOR -->
        <div id="divTabInfo" class="divDataCells">
            <div style="min-height: 10px"></div>
            <div id="divProductiveUnit" class="blackRibbonTitle">
            </div>
            <div style="min-height: 10px"></div>
        </div>
        <!-- FIN TAB SUPERIOR -->

        <!-- ARBOL Y DETALLE -->
        <div id="divTabData" class="divDataCells">

            <div id="divTree" class="treeSize">
                <div id="ctlTreeDiv" style="height: 500px;">
                    <rws:roTreesSelector ID="roTreesProductiveUnit" runat="server" ShowEmployeeFilters="false" PrefixTree="roTreesProductiveUnit"
                        Tree1Visible="true" Tree1MultiSel="false" Tree1ShowOnlyGroups="false" Tree1Function="cargaNodo" Tree1ImagePath="images/ProductiveUnitSelector" Tree1SelectorPage="../../AIScheduler/ProductiveUnitSelectorData.aspx"
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
                                    <div id="div00" class="contentPanel" runat="server" name="menuPanel">
                                        <!-- Este div es el header -->
                                        <div class="panBottomMargin">
                                            <div class="panHeader2 panBottomMargin">
                                                <span class="panelTitleSpan">
                                                    <asp:Label runat="server" ID="lblProductiveUnitGeneral" Text="General"></asp:Label>
                                                </span>
                                            </div>
                                            <!-- La descripción es opcional -->
                                            <div class="panelHeaderContent">
                                                <div class="panelDescriptionImage">
                                                    <img alt="" src="<%=Me.Page.ResolveUrl("~/AIScheduler/Images/ProductiveUnit.png")%>" />
                                                </div>
                                                <div class="panelDescriptionText">
                                                    <asp:Label ID="lblProductiveUnitGeneralDesc" runat="server" Text="Datos especificos de la ${ProductiveUnit}."></asp:Label>
                                                </div>
                                            </div>
                                        </div>

                                        <!-- Este div es un formulario -->
                                        <div class="panBottomMargin">
                                            <div class="divRow">
                                                <div class="divRowDescription">
                                                    <asp:Label ID="lblNameDescription" runat="server" Text="Nombre identificativo de la ${ProductiveUnit}"></asp:Label>
                                                </div>
                                                <asp:Label ID="lblName" runat="server" Text="Nombre:" CssClass="labelForm"></asp:Label>
                                                <div class="componentForm">
                                                    <dx:ASPxTextBox ID="txtName" runat="server" ClientInstanceName="txtName_Client" NullText="_____">
                                                        <ClientSideEvents Validation="LengthValidation" TextChanged="function(s,e){checkProductiveUnitEmptyName(s.GetValue());}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                        <ValidationSettings SetFocusOnError="True">
                                                            <RequiredField IsRequired="True" ErrorText="(*)" />
                                                        </ValidationSettings>
                                                    </dx:ASPxTextBox>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="divRow">
                                            <div class="divRowDescription">
                                                <asp:Label ID="lblProductiveUnitShortnameDescription" runat="server" Text="Nombre abreviado utilizado como referencia de la ${ProductiveUnit}"></asp:Label>
                                            </div>
                                            <asp:Label ID="lblProductiveUnitShortNameDesc" runat="server" Text="Nombre abreviado:" CssClass="labelForm"></asp:Label>
                                            <div class="componentForm">
                                                <dx:ASPxTextBox ID="txtShortName" runat="server" MaxLength="3" Width="50" NullText="_____" ClientInstanceName="txtShortName_Client">
                                                    <ClientSideEvents Validation="LengthValidation" TextChanged="function(s,e){ hasChanges(true)}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                    <ValidationSettings SetFocusOnError="True">
                                                        <RequiredField IsRequired="True" ErrorText="(*)" />
                                                    </ValidationSettings>
                                                </dx:ASPxTextBox>
                                            </div>
                                        </div>

                                        <div class="divRow">
                                            <div class="divRowDescription">
                                                <asp:Label ID="lblProductiveUnitColorDesc" runat="server" Text="Color identificativo de la ${ProductiveUnit} utilizado en los ${Budgets}"></asp:Label>
                                            </div>
                                            <asp:Label ID="lblProductiveUnitColorName" runat="server" Text="Color identificativo:" class="labelForm"></asp:Label>
                                            <div class="componentForm">
                                                <dx:ASPxColorEdit ID="dxcolor" runat="server" EnableCustomColors="true" Width="14px">
                                                    <ClientSideEvents ColorChanged="function(s,e){s.GetInputElement().style.display='none';hasChanges(true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                </dx:ASPxColorEdit>
                                            </div>
                                        </div>

                                        <div id="costCenterRow" class="divRow" runat="server" style="display: none">
                                            <div class="divRowDescription">
                                                <asp:Label ID="lclCostCenterDesc" runat="server" Text="Centro de coste al que se asignan las incidencias de la ${ProductiveUnit}"></asp:Label>
                                            </div>
                                            <asp:Label ID="lclCostCenterTitle" runat="server" Text="Centro de coste:" class="labelForm"></asp:Label>
                                            <div class="componentForm">
                                                <dx:ASPxComboBox runat="server" ID="cmbCostCenter" Width="250px">
                                                    <ClientSideEvents SelectedIndexChanged="function(s,e){hasChanges(true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                </dx:ASPxComboBox>
                                            </div>
                                        </div>

                                        <div class="divRow">
                                            <div class="divRowDescription">
                                                <asp:Label ID="lblProductiveUnitDescription2Desc" runat="server" Text="Descripción"></asp:Label>
                                            </div>
                                            <asp:Label ID="lblProductiveUnitDescription2" runat="server" Text="Descripción:" class="labelForm"></asp:Label>
                                            <div class="componentForm">
                                                <dx:ASPxMemo ID="txtDescription" runat="server" Rows="5" Width="475px">
                                                    <ClientSideEvents TextChanged="function(s,e){hasChanges(true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                </dx:ASPxMemo>
                                            </div>
                                        </div>

                                        <div class="panBottomMargin">
                                            <div class="panHeader2 panBottomMargin">
                                                <span class="panelTitleSpan">
                                                    <asp:Label runat="server" ID="lblUnitModes" Text="Modos"></asp:Label>
                                                </span>
                                            </div>
                                            <div class="panelHeaderContent">
                                                <div class="panelDescriptionImage">
                                                    <img alt="" src="<%=Me.Page.ResolveUrl("~/AIScheduler/Images/PUnitMode.png")%>" />
                                                </div>
                                                <div class="panelDescriptionText">
                                                    <asp:Label ID="lblUnitModesDesc" runat="server" Text="En esta pantalla se definen todos los modos en los que puede trabajar sobre la ${ProductiveUnit}. Estos modos serán los utilizados en el momento de planificar diariamente las ${ProductiveUnits} de los ${Budgets} "></asp:Label>
                                                </div>
                                            </div>

                                            <div class="jsGrid">
                                                <asp:Label ID="lblUnitModesTitle" runat="server" CssClass="jsGridTitle" Text="Modos definidos para la ${ProductiveUnit}"></asp:Label>
                                                <div class="jsgridButton">
                                                    <dx:ASPxButton ID="btnAddNewMode" runat="server" AutoPostBack="False" CausesValidation="False" Text="Añadir" ToolTip="Añadir nuevo modo" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat">
                                                        <Image Url="~/Base/Images/Grid/add.png"></Image>
                                                        <ClientSideEvents Click="AddNewProductiveUnit" />
                                                    </dx:ASPxButton>
                                                </div>
                                            </div>
                                            <div id="divModesGrid" runat="server" class="jsGridContent dextremeGrid">
                                                <!-- Carrega del Grid Usuari General -->
                                            </div>
                                        </div>
                                    </div>

                                    <!-- Panell General -->
                                    <div id="div01" class="contentPanel" runat="server" name="menuPanel">
                                        <div class="panBottomMargin">
                                            <div class="panHeader2 panBottomMargin">
                                                <span class="panelTitleSpan">
                                                    <asp:Label runat="server" ID="lblPUSummary" Text="Resumen / Gráficos"></asp:Label>
                                                </span>
                                            </div>
                                        </div>
                                        <div style="width: 50%">
                                            <div class="divRow">
                                                <div class="divRowDescription" style="width: 100%;">
                                                    <asp:Label ID="Label2lblEmployeeSummaryDesc" runat="server" Text="Puede consultar los distintos saldos, justificaciones, datos de productiv y centros de coste del empleado en el periodo seleccionado."></asp:Label>
                                                </div>
                                                <asp:Label ID="lblEmployeSummaryPeriod" runat="server" Text="Periodo:" CssClass="labelForm"></asp:Label>
                                                <div class="componentForm">
                                                    <dx:ASPxComboBox runat="server" ID="cmbSummaryPeriod" Width="200px" ClientInstanceName="cmbSummaryPeriodClient">
                                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ if(cmbSummaryPeriodClient.GetSelectedItem() != null){ LoadSummary(cmbSummaryPeriodClient.GetSelectedItem().value);}}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                    </dx:ASPxComboBox>
                                                </div>
                                            </div>
                                        </div>

                                        <div id="noDataRow" runat="server" class="divRow" style="display: none">
                                            <div class="panBottomMargin">
                                                <br />
                                                <br />
                                                <div class="panHeader3 panBottomMargin">
                                                    <span class="panelTitleSpan">
                                                        <asp:Label ID="lblSummaryNoData" Text="No hay datos en el periodo seleccionado o no dispone de permisos para consultarlos." runat="server" />
                                                    </span>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="divRow">
                                            <div id="divPUSummary" runat="server" class="divSummary">
                                                <div class="panBottomMargin">
                                                    <div class="panHeader2 panBottomMargin">
                                                        <span class="panelTitleSpan">
                                                            <asp:Label runat="server" ID="lblPUModeSummary" Text="Modos"></asp:Label>
                                                        </span>
                                                    </div>
                                                </div>
                                                <div id="divPUDraw" runat="server" class="accrualSumary" style="width: 96%">
                                                    <!-- Dibuja los saldos -->
                                                </div>

                                                <div id="divPUCanvas" style="height: 400px; margin-left: 45px;" runat="server">
                                                </div>
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

            <!-- POPUP NEW OBJECT -->
            <dx:ASPxPopupControl ID="NewObjectPopup" runat="server" AllowDragging="False" CloseAction="None" Modal="True" ContentUrl="~/Base/Popups/CreateObjectPopup.aspx"
                PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter" Width="470px" Height="300px"
                ShowHeader="False" ScrollBars="Auto" ShowPageScrollbarWhenModal="True" ClientInstanceName="NewObjectPopup_Client" PopupAnimationType="None" BackColor="Transparent" ContentStyle-Paddings-Padding="0px" Border-BorderColor="Transparent" ShowShadow="false">
                <SettingsLoadingPanel Enabled="false" />
            </dx:ASPxPopupControl>

            <!-- POPUP roUnitMode -->
            <div style="display: none">
                <div id="dialogUnitNode" class="ui-dialog-content">
                    <!-- Este div es el header -->
                    <div class="panBottomMargin">
                        <div class="panHeader2 panBottomMargin">
                            <span class="panelTitleSpan">
                                <asp:Label runat="server" ID="lblUnitModeGeneral" Text="General"></asp:Label>
                            </span>
                        </div>
                        <!-- La descripción es opcional -->
                        <div class="panelHeaderContent">
                            <div class="panelDescriptionImage">
                                <img alt="" src="<%=Me.Page.ResolveUrl("~/AIScheduler/Images/PUnitMode.png")%>" />
                            </div>
                            <div class="panelDescriptionText">
                                <asp:Label ID="lblUnitModeGeneralDesc" runat="server" Text="En esta sección se define el modo de la ${ProductiveUnit}, indicando los datos generales, el equipo necesario y el coste diario por no cubrir las necesidades del modo."></asp:Label>
                            </div>
                        </div>
                    </div>

                    <!-- Este div es un formulario -->
                    <div class="panBottomMargin">
                        <div class="divRow">
                            <div class="divRowDescription">
                                <asp:Label ID="lblUnitModeNameDesc" runat="server" Text="Nombre identificativo del modo"></asp:Label>
                            </div>
                            <asp:Label ID="lblUnitModeNameTitle" runat="server" Text="Nombre:" CssClass="labelForm"></asp:Label>
                            <div class="componentForm">
                                <dx:ASPxTextBox ID="txtUnitModeName" runat="server" ClientInstanceName="txtUnitModeNameClient" NullText="_____">
                                    <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                </dx:ASPxTextBox>
                            </div>
                        </div>
                    </div>

                    <div class="divRow">
                        <div class="divRowDescription">
                            <asp:Label ID="lblUnitModeShortNameDesc" runat="server" Text="Nombre abreviado utilizado como referencia del modo"></asp:Label>
                        </div>
                        <asp:Label ID="lblUnitModeShortNameTitle" runat="server" Text="Nombre abreviado:" CssClass="labelForm"></asp:Label>
                        <div class="componentForm">
                            <dx:ASPxTextBox ID="txtUnitModeShortName" runat="server" MaxLength="3" Width="50" NullText="_____" ClientInstanceName="txtUnitModeShortNameClient">
                                <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                            </dx:ASPxTextBox>
                        </div>
                    </div>

                    <div class="divRow">
                        <div class="divRowDescription">
                            <asp:Label ID="lblUnitModeShortCostDesc" runat="server" Text="Coste del modo si no se cubren sus necesidades"></asp:Label>
                        </div>
                        <asp:Label ID="lblUnitModeShortCostTitle" runat="server" Text="Coste:" CssClass="labelForm"></asp:Label>
                        <div class="componentForm">
                            <dx:ASPxTextBox ID="txtUnitModeShortCost" runat="server" Width="100" NullText="_____" ClientInstanceName="txtUnitModeShortCostClient">
                                <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                <MaskSettings Mask="<0..99999>.<00..99>" IncludeLiterals="DecimalSymbol" />
                            </dx:ASPxTextBox>
                        </div>
                    </div>

                    <div class="divRow">
                        <div class="divRowDescription">
                            <asp:Label ID="lblUnitModeColorDesc" runat="server" Text="Color identificativo de la ${ProductiveUnit} utilizado en los ${Budgets}"></asp:Label>
                        </div>
                        <asp:Label ID="lblUnitModeColorTitle" runat="server" Text="Color identificativo:" class="labelForm"></asp:Label>
                        <div class="componentForm">
                            <dx:ASPxColorEdit ID="txtUnitModeColor" runat="server" EnableCustomColors="true" Width="14px" ClientInstanceName="txtUnitModeColorClient">
                                <ClientSideEvents ColorChanged="function(s,e){s.GetInputElement().style.display='none';}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                            </dx:ASPxColorEdit>
                        </div>
                    </div>

                    <div class="divRow">
                        <div class="divRowDescription">
                            <asp:Label ID="lblUnitModeDescDesc" runat="server" Text="Descripción"></asp:Label>
                        </div>
                        <asp:Label ID="lblUnitModeDescTitle" runat="server" Text="Descripción:" class="labelForm"></asp:Label>
                        <div class="componentForm">
                            <dx:ASPxMemo ID="txtUnitModeDesc" runat="server" Rows="5" Width="475px" ClientInstanceName="txtUnitModeDescClient">
                                <ClientSideEvents TextChanged="" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                            </dx:ASPxMemo>
                        </div>
                    </div>

                    <div class="panBottomMargin">
                        <div class="panHeader2 panBottomMargin">
                            <span class="panelTitleSpan">
                                <asp:Label runat="server" ID="lblTeam" Text="Equipo"></asp:Label>
                            </span>
                        </div>
                    </div>

                    <div class="panBottomMargin">
                        <div class="jsGrid">
                            <asp:Label ID="lblAddPosition" runat="server" CssClass="jsGridTitle" Text="Posiciones"></asp:Label>
                            <div class="jsgridButton">
                                <dx:ASPxButton ID="btAddPosition" runat="server" AutoPostBack="False" CausesValidation="False" Text="Añadir" ToolTip="Añadir nueva posición" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat">
                                    <Image Url="~/Base/Images/Grid/add.png"></Image>
                                    <ClientSideEvents Click="AddNewPUnitPosition" />
                                </dx:ASPxButton>
                            </div>
                        </div>
                        <div class="jsGridContent" style="overflow: hidden; height: 300px">
                            <roForms:roCalendar ID="oCalendar" WorkMode="roProductiveUnit" runat="server" Feature="Calendar" ClientInstanceName="roPUnitCalendar" Height="100%"
                                roCalendar_EndCallback="CallbackCalendar_CallbackComplete" performAction_EndCallback="PerformActionCallback_CallbackComplete"
                                complementary_EndCallback="complementaryDefinitionCallback_CallbackComplete" assignments_EndCallback="assignmentDefinitionCallback_CallbackComplete" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script language="javascript" type="text/javascript">

        function resizeTreeProductiveUnit() {
            try {
                var ctlPrefix = "<%= roTreesProductiveUnit.ClientID %>";
                eval(ctlPrefix + "_resizeTrees();");
            }
            catch (e) {
                showError("resizeTreeProductiveUnit", e);
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
            resizeTreeProductiveUnit();
        }

        if (document.getElementById('<%= noRegs.ClientID %>').value != '') {
            cargaProductiveUnit('-1');
        }
    </script>
</asp:Content>