<%@ Page Language="VB" MasterPageFile="~/Base/mastEmp.master" AutoEventWireup="false" Inherits="VTLive40.AccessStatus" Title="${AccessStatus}" CodeBehind="AccessStatus.aspx.vb" %>

<asp:Content ID="cMainBody" ContentPlaceHolderID="contentMainBody" runat="Server">

    <script language="javascript" type="text/javascript">

        function PageBase_Load() {
            resizeFrames();
            resizeTreeAccessStatus();
        }
    </script>

    <input type="hidden" runat="server" id="noRegs" value="" />
    <input type="hidden" runat="server" id="hdnModeEdit" value="" />

    <input type="hidden" id="msgHasChanges" value="" runat="server" clientidmode="Static" />
    <input type="hidden" id="msgHasErrors" value="" runat="server" clientidmode="Static" />

    <div id="divMainBody">
        <!-- TAB SUPERIOR -->
        <div id="divTabInfo" class="divDataCells">
            <div style="min-height: 10px"></div>
            <div id="divAccessStatus" class="blackRibbonTitle">
            </div>
            <div style="min-height: 10px"></div>
        </div>
        <!-- FIN TAB SUPERIOR -->

        <!-- ARBOL Y DETALLE -->
        <div id="divTabData" class="divDataCells">

            <div id="divTree" class="treeSize">
                <div id="ctlTreeDiv" style="height: 500px;">
                    <rws:roTreesSelector ID="roTreesAccessStatus" runat="server" ShowEmployeeFilters="false" PrefixTree="roTreesAccessStatus"
                        Tree1Visible="true" Tree1MultiSel="false" Tree1ShowOnlyGroups="false" Tree1Function="cargaNodo" Tree1ImagePath="images/AccessStatusSelector" Tree1SelectorPage="../../Access/AccessStatusSelectorData.aspx"
                        ShowTreeCaption="true"></rws:roTreesSelector>
                </div>
            </div>

            <div id="divButtons" class="divMiddleButtons">
                <div id="divBarButtons" class="maxHeight">&nbsp</div>
            </div>

            <dx:ASPxCallback ID="CallbackHelper" runat="server" ClientInstanceName="CallbackHelperClient">
                <ClientSideEvents CallbackComplete="CallbackHelperClient_CallbackComplete" />
            </dx:ASPxCallback>

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
                                    <!-- Pantalla Grupos -->
                                    <dx:ASPxHiddenField ID="hdnManagePlane" runat="server" ClientInstanceName="hdnManagePlaneClient"></dx:ASPxHiddenField>
                                    <div id="divEmpresa" class="contentPanel" name="menuPanel" runat="server">
                                        <!-- Este div es el header -->
                                        <div class="panBottomMargin">
                                            <div class="panHeader2 panBottomMargin">
                                                <span class="panelTitleSpan">
                                                    <asp:Label runat="server" ID="lblAccessStatusTitleGeneral" Text="Estado"></asp:Label>
                                                </span>
                                            </div>
                                            <!-- La descripción es opcional -->
                                            <div class="panelHeaderContent">
                                                <div class="panelDescriptionImage">
                                                    <img alt="" src="<%=Me.Page.ResolveUrl("~/Base/Images/StartMenuIcos/AccessStatus.png")%>" />
                                                </div>
                                                <div class="panelDescriptionText">
                                                    <asp:Label ID="lblAccessStatusTitleGeneralDescription" runat="server" Text="Puede selecciona una zona dentro de su empresa para ver el mapa asociado o seleccionar una zona para ver el estado de entradas y salidas de la misma"></asp:Label>
                                                </div>
                                            </div>
                                        </div>

                                        <!-- Este div es un formulario -->
                                        <div class="panBottomMargin">
                                            <div class="divRow">
                                                <table border="0" cellpadding="0" cellspacing="0">
                                                    <tr>
                                                        <td style="padding-left: 5px;">
                                                            <asp:Label ID="lblZonePlane" runat="server" Text="Plano de zona:"></asp:Label></td>
                                                        <td style="padding-left: 10px;">
                                                            <dx:ASPxComboBox ID="cmbStatusPlaneMain" runat="server" Width="250px" Font-Size="11px" CssClass="editTextFormat"
                                                                Font-Names="Arial;Verdana;Sans-Serif" IncrementalFilteringMode="Contains" ClientInstanceName="cmbStatusPlaneMainClient">
                                                                <ClientSideEvents SelectedIndexChanged="function(s,e) { reloadflBg(s.GetSelectedItem().value); }" />
                                                            </dx:ASPxComboBox>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>
                                            <div class="divRow">
                                                <!-- flash aqui -->
                                                <div class="flashLeft">
                                                    <div id="divStatusMap" class="flashMaps"></div>
                                                </div>
                                                <div class="flashRight">
                                                    <a href="javascript: void(0);" onclick="ShowZoomStatus();">
                                                        <img alt="" src="Images/LocationMap/btnzoom.png" border="0" />
                                                    </a>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div id="divStatus" class="contentPanel" name="menuPanel" runat="server">
                                        <div id="divZoneInfo" style="width: 100%; height: 100%; padding: 0px;" runat="server" name="menuPanel">
                                            <!-- Grid ACTUAL -->
                                            <!-- Este div es el header -->
                                            <div class="panBottomMargin">
                                                <div class="panHeader2 panBottomMargin">
                                                    <span class="panelTitleSpan">
                                                        <asp:Label runat="server" ID="lblTitleEmpInZone" Text="Empleados que hay actualmente"></asp:Label>
                                                    </span>
                                                </div>
                                                <!-- La descripción es opcional -->
                                                <div class="panelHeaderContent">
                                                    <div class="panelDescriptionImage">
                                                        <img alt="" src="<%=Me.Page.ResolveUrl("~/Base/Images/StartMenuIcos/AccessStatus.png")%>" />
                                                    </div>
                                                    <div class="panelDescriptionText">
                                                        <asp:Label ID="lblTitleEmpInZoneDescription" runat="server" Text="Empleados que actualmente se encuentran dentro de la zona"></asp:Label>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="panBottomMargin">
                                                <div class="jsGrid">
                                                    <asp:Label ID="lblTitleEmpInZoneGrid" runat="server" CssClass="jsGridTitle" Text="Empleados que hay actualmente"></asp:Label>
                                                </div>

                                                <div id="divGridEmpInZoneHeader" class="jsGridContent statusGridHeader" runat="server">
                                                    <!-- Aqui va el header grid de Ausencias Previstas -->
                                                </div>
                                                <div id="divGridEmpInZone" class="jsGridContent statusGridContent" runat="server">
                                                    <!-- Aqui va el grid de Ausencias Previstas -->
                                                </div>
                                            </div>

                                            <!-- Grid TRANSIT -->
                                            <!-- Este div es el header -->
                                            <div class="panBottomMargin">
                                                <div class="panHeader2 panBottomMargin">
                                                    <span class="panelTitleSpan">
                                                        <asp:Label runat="server" ID="lblTitleLeaveLastHour" Text="Empleados recientes"></asp:Label>
                                                    </span>
                                                </div>
                                                <!-- La descripción es opcional -->
                                                <div class="panelHeaderContent">
                                                    <div class="panelDescriptionImage">
                                                        <img alt="" src="<%=Me.Page.ResolveUrl("~/Base/Images/StartMenuIcos/AccessStatus.png")%>" />
                                                    </div>
                                                    <div class="panelDescriptionText">
                                                        <asp:Label ID="lblTitleLeaveLastHourDescription" runat="server" Text="Empleados que se fueron en la última hora de la zona de acceso."></asp:Label>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="panBottomMargin">
                                                <div class="jsGrid">
                                                    <asp:Label ID="lblTitleLeaveLastHourGrid" runat="server" CssClass="jsGridTitle" Text="Empleados recientes"></asp:Label>
                                                </div>

                                                <div id="divGridLeaveLastHourHeader" class="jsGridContent statusGridHeader" runat="server">
                                                    <!-- Aqui va el header grid de Ausencias Previstas -->
                                                </div>
                                                <div id="divGridLeaveLastHour" class="jsGridContent statusGridContent" runat="server">
                                                    <!-- Aqui va el grid de Ausencias Previstas -->
                                                </div>
                                            </div>

                                            <!-- Grid PASSAT -->
                                            <!-- Este div es el header -->
                                            <div class="panBottomMargin">
                                                <div class="panHeader2 panBottomMargin">
                                                    <span class="panelTitleSpan">
                                                        <asp:Label runat="server" ID="lblTitleIncorrectAccess" Text="Accesos incorrectos"></asp:Label>
                                                    </span>
                                                </div>
                                                <!-- La descripción es opcional -->
                                                <div class="panelHeaderContent">
                                                    <div class="panelDescriptionImage">
                                                        <img alt="" src="<%=Me.Page.ResolveUrl("~/Base/Images/StartMenuIcos/AccessStatus.png")%>" />
                                                    </div>
                                                    <div class="panelDescriptionText">
                                                        <asp:Label ID="lblTitleIncorrectAccessDescription" runat="server" Text="Accesos incorrectos en esta zona."></asp:Label>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="panBottomMargin">
                                                <div class="jsGrid">
                                                    <asp:Label ID="lblTitleIncorrectAccessGrid" runat="server" CssClass="jsGridTitle" Text="Accesos incorrectos"></asp:Label>
                                                </div>

                                                <div id="divGridIncorrectAccessHeader" class="jsGridContent statusGridHeader" runat="server">
                                                    <!-- Aqui va el header grid de Ausencias Previstas -->
                                                </div>
                                                <div id="divGridIncorrectAccess" class="jsGridContent statusGridContent" runat="server">
                                                    <!-- Aqui va el grid de Ausencias Previstas -->
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
        </div>
    </div>

    <script language="javascript" type="text/javascript">

        function resizeTreeAccessStatus() {
            try {
                var ctlPrefix = "<%= roTreesAccessStatus.ClientID %>";
                eval(ctlPrefix + "_resizeTrees();");
            }
            catch (e) {
                showError("resizeTreeAccessStatus", e);
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
            resizeTreeAccessStatus();
        }
    </script>
</asp:Content>