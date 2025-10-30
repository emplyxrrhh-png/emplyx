<%@ Page Language="VB" MasterPageFile="~/Base/mastEmp.master" AutoEventWireup="false" Inherits="VTLive40.AccessZones" Title="${AccessZones}" CodeBehind="AccessZones.aspx.vb" %>

<%@ Register Src="~/Access/WebUserForms/frmAddException.ascx" TagName="frmAddException" TagPrefix="roForms" %>
<%@ Register Src="~/Access/WebUserForms/frmAddPeriod.ascx" TagName="frmAddPeriod" TagPrefix="roForms" %>

<asp:Content ID="cMainBody" ContentPlaceHolderID="contentMainBody" runat="Server">

    <script language="javascript" type="text/javascript">

        function PageBase_Load() {
            resizeFrames();
            resizeTreeAccessZones();
        }
    </script>

    <input type="hidden" runat="server" id="hdnModeEdit" value="" />
    <input type="hidden" runat="server" id="noRegs" value="" />
    <input type="hidden" id="hdnLngDateException" value='<%= Me.Language.TranslateJavaScript("gridHeader.DateException", Me.DefaultScope) %>' />
    <input type="hidden" id="hdnLngWeekdayName" value='<%= Me.Language.TranslateJavaScript("gridHeader.WeekDayName",Me.DefaultScope) %>' />
    <input type="hidden" id="hdnLngDateBegin" value='<%= Me.Language.TranslateJavaScript("gridHeader.DateBegin",Me.DefaultScope) %>' />
    <input type="hidden" id="hdnLngDateEnd" value='<%= Me.Language.TranslateJavaScript("gridHeader.DateEnd",Me.DefaultScope) %>' />
    <div id="divModalBgDisabled" class="modalBackground" style="position: absolute; left: 0px; top: 0px; z-index: 990; width: 100%; height: 100%; display: none;"></div>

    <dx:ASPxHiddenField ID="dateConfig" runat="server" ClientInstanceName="dateConfig" SyncWithServer="false"></dx:ASPxHiddenField>

    <input type="hidden" id="msgHasChanges" value="" runat="server" clientidmode="Static" />
    <input type="hidden" id="msgHasErrors" value="" runat="server" clientidmode="Static" />

    <div id="divMainBody">
        <!-- TAB SUPERIOR -->
        <div id="divTabInfo" class="divDataCells">
            <div style="min-height: 10px"></div>
            <div id="divAccessZone" class="blackRibbonTitle">
            </div>
            <div style="min-height: 10px"></div>
        </div>

        <!-- FIN TAB SUPERIOR -->

        <!-- ARBOL Y DETALLE -->
        <div id="divTabData" class="divDataCells">

            <div id="divTree" class="treeSize">
                <div id="ctlTreeDiv" style="height: 500px;">
                    <rws:roTreesSelector ID="roTreesAccessZones" runat="server" ShowEmployeeFilters="false" PrefixTree="roTreesAccessZones"
                        Tree1Visible="true" Tree1MultiSel="false" Tree1ShowOnlyGroups="false" Tree1Function="cargaNodo" Tree1ImagePath="images/AccessZonesSelector" Tree1SelectorPage="../../Access/AccessZonesSelectorData.aspx"
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

                                <dx:ASPxHiddenField ID="hdnManagePlane" runat="server" ClientInstanceName="hdnManagePlaneClient"></dx:ASPxHiddenField>
                                <div id="divContentPanels" class="divContentPanelsWithOutMessage">
                                    <div id="divEmpresa" class="contentPanel" name="menuPanel" runat="server">
                                        <div class="panHeader2">
                                            <span style="">
                                                <asp:Label runat="server" ID="lblAccessZonesMainTitleGeneral" Text="General"></asp:Label></span>
                                        </div>
                                        <br />
                                        <table border="0" style="width: 100%; height: 90%; padding-top: 5px">
                                            <tr>
                                                <td width="70px" align="right" valign="top" style="padding-right: 5px;">
                                                    <img src="Images/AccessZones.png" /></td>
                                                <td valign="top" style="padding: 5px; padding-right: 30px;">
                                                    <asp:Label ID="lblAccessZonesEmp" runat="server" Text="Datos especificos para la zona de acceso. En esta pantalla se puede visualizar y cambiar los datos de la zona de acceso seleccionada: nombre, situación en el mapa..." CssClass="spanEmp-class"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2" style="padding-left: 75px;">
                                                    <table border="0" cellpadding="0" cellspacing="0">
                                                        <tr>
                                                            <td style="padding-left: 5px;">
                                                                <asp:Label ID="lblZonePlane" runat="server" Text="Plano de zona:"></asp:Label></td>
                                                            <td style="padding-left: 10px;">
                                                                <dx:ASPxComboBox ID="cmbZonePlaneMain" runat="server" Width="250px" ClientInstanceName="cmbZonePlaneMainClient">
                                                                    <ClientSideEvents SelectedIndexChanged="function(s,e) { reloadflBg(s.GetSelectedItem().value); }" />
                                                                </dx:ASPxComboBox>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-top: 5px; width: 100%; height: 99%;" colspan="2" align="center" valign="top">
                                                    <table border="0" cellpadding="0" cellspacing="0" width="90%" height="90%">
                                                        <tr>
                                                            <td>
                                                                <!-- flash aqui -->
                                                                <div id="divLocationMap1" style="height: 400px;" class="flashMaps"></div>
                                                            </td>
                                                            <td valign="bottom" style="padding-bottom: 20px; border-left: solid 1px black;">
                                                                <table border="0" cellpadding="0" cellspacing="0" width="30px" height="40px">
                                                                    <tr>
                                                                        <td><a href="javascript: void(0);" onclick="ShowZoomZone('false');">
                                                                            <img alt="" src="Images/LocationMap/btnzoom.png" border="0" /></a></td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="height: 30px;">&nbsp;</td>
                                            </tr>
                                        </table>
                                    </div>

                                    <div id="divZone" class="contentPanel" name="menuPanel" runat="server">
                                        <!-- Panell General -->
                                        <div id="div00" class="contentPanel" runat="server" name="menuPanel">
                                            <div class="panBottomMargin">
                                                <div class="panHeader2 panBottomMargin">
                                                    <span class="panelTitleSpan">
                                                        <asp:Label runat="server" ID="lblAccessZonesTitleGeneral" Text="General"></asp:Label>
                                                    </span>
                                                </div>
                                                <div class="panelHeaderContent">
                                                    <div class="panelDescriptionImage">
                                                        <img alt="" src="<%=Me.Page.ResolveUrl("~/Base/Images/StartMenuIcos/AccessZones.png")%>" />
                                                    </div>
                                                    <div class="panelDescriptionText">
                                                        <asp:Label ID="lblAccessZonesTitleGeneralDescription" runat="server" Text="Definición de las zonas de acceso" CssClass="spanEmp-class"></asp:Label>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="splitDivLeft">
                                                <div class="flashLeft">
                                                    <div id="divLocationMap" style="height: 300px;" class="flashMaps"></div>
                                                </div>
                                                <div class="flashRight">
                                                    <a href="javascript: void(0);" onclick="ShowZoomZone('false');">
                                                        <img alt="" src="Images/LocationMap/btnzoom2.png" border="0" /></a>
                                                    <a href="javascript: void(0);" onclick="ShowZoomZone('true');">
                                                        <img alt="" src="Images/LocationMap/btnfixlocation.png" border="0" /></a>
                                                    <a href="javascript: void(0);" onclick="ShowChangeZoneImage();" style="display: none;">
                                                        <img alt="" src="Images/LocationMap/btnimgupload.png" border="0" /></a>
                                                </div>
                                            </div>

                                            <div class="splitDivRight">
                                                <div class="panBottomMargin">
                                                    <div class="divRow">
                                                        <div class="divRowDescription midWidthDescription">
                                                            <asp:Label ID="lblNameDescription" runat="server" Text="Nombre de la zona de acceso"></asp:Label>
                                                        </div>
                                                        <asp:Label ID="lblName" runat="server" Text="Nombre:" CssClass="labelForm midWidth"></asp:Label>
                                                        <div class="componentForm">
                                                            <dx:ASPxTextBox ID="txtName" runat="server" ClientInstanceName="txtName_Client" NullText="_____">
                                                                <ClientSideEvents Validation="LengthValidation" TextChanged="function(s,e){checkAccessZoneEmptyName(s.GetValue());}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                <ValidationSettings SetFocusOnError="True">
                                                                    <RequiredField IsRequired="True" ErrorText="(*)" />
                                                                </ValidationSettings>
                                                            </dx:ASPxTextBox>
                                                        </div>
                                                    </div>

                                                    <div class="divRow">
                                                        <div class="divRowDescription midWidthDescription">
                                                            <asp:Label ID="lblColorField" runat="server" Text="Color identificativo de la zona"></asp:Label>
                                                        </div>
                                                        <asp:Label ID="lblColor" runat="server" Text="Color:" CssClass="labelForm midWidth"></asp:Label>
                                                        <div class="componentForm">
                                                            <dx:ASPxColorEdit ID="dxColorPicker" runat="server" ClientInstanceName="dxColorPickerClient" EnableCustomColors="true" Width="14px">
                                                                <ClientSideEvents ColorChanged="function(s,e){s.GetInputElement().style.display = 'none';hasChanges(true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                            </dx:ASPxColorEdit>
                                                        </div>
                                                    </div>
                                                    <div class="divRow">
                                                        <div class="divRowDescription midWidthDescription">
                                                            <asp:Label ID="lblZonaPlaneDescDescription" runat="server" Text="Plano de la zona"></asp:Label>
                                                        </div>
                                                        <asp:Label ID="lblZonaPlaneDesc" runat="server" Text="Plano:" CssClass="labelForm midWidth"></asp:Label>
                                                        <div class="componentForm" style="width: 200px">
                                                            <dx:ASPxComboBox ID="cmbZonePlane" runat="server" Width="200px" ClientInstanceName="cmbZonePlaneClient">
                                                                <ClientSideEvents SelectedIndexChanged="function(s,e) { reloadflBg(s.GetSelectedItem().value); hasChanges(true); }" />
                                                                <ValidationSettings ErrorDisplayMode="None">
                                                                </ValidationSettings>
                                                            </dx:ASPxComboBox>
                                                        </div>
                                                        <div class="componentForm" style="width: 20px">
                                                            <a href="javascript: void(0);" onclick="ShowZonePlanes();" class="btnZonePlanes"></a>
                                                        </div>
                                                    </div>
                                                    <div class="divRow">
                                                        <div class="divRowDescription midWidthDescription">
                                                            <asp:Label ID="lblDescCameraDesc" runat="server" Text="Cámara de la zona"></asp:Label>
                                                        </div>
                                                        <asp:Label ID="lblDescCamera" runat="server" Text="Camara:" CssClass="labelForm midWidth"></asp:Label>
                                                        <div class="componentForm">
                                                            <dx:ASPxComboBox ID="cmbCamera" runat="server" Width="200px">
                                                                <ClientSideEvents SelectedIndexChanged="function(s,e) { hasChanges(true); }" />
                                                                <ValidationSettings ErrorDisplayMode="None">
                                                                </ValidationSettings>
                                                            </dx:ASPxComboBox>
                                                        </div>
                                                    </div>

                                                    <div class="divRow">
                                                        <div class="divRowDescription midWidthDescription">
                                                            <asp:Label ID="lblDefaultTimeZoneDescription" runat="server" Text="Zona horaria por defecto donde esta la zona"></asp:Label>
                                                        </div>
                                                        <asp:Label ID="lblDefaultTimeZone" runat="server" Text="Zona horaria:" CssClass="labelForm midWidth"></asp:Label>
                                                        <div class="componentForm" style="width: 200px">
                                                            <dx:ASPxComboBox ID="cmbDefaultTimeZone" runat="server" Width="200px" ClientInstanceName="cmbDefaultTimeZoneClient">
                                                                <ClientSideEvents SelectedIndexChanged="function(s,e) { reloadflBg(s.GetSelectedItem().value); hasChanges(true); }" />
                                                                <ValidationSettings ErrorDisplayMode="None">
                                                                </ValidationSettings>
                                                            </dx:ASPxComboBox>
                                                        </div>
                                                    </div>

                                                    <div class="divRow">
                                                        <div class="divRowDescription midWidthDescription">
                                                            <asp:Label ID="lblDescZoneDesc" runat="server" Text="Descripción de la zona"></asp:Label>
                                                        </div>
                                                        <asp:Label ID="lblDescZone" runat="server" Text="Descripción:" CssClass="labelForm midWidth"></asp:Label>
                                                        <div class="componentForm">
                                                            <dx:ASPxMemo ID="txtDescription" runat="server" Rows="2" Width="100%" Height="40">
                                                                <ClientSideEvents TextChanged="function(s,e){ hasChanges(true)}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                            </dx:ASPxMemo>
                                                        </div>
                                                    </div>
                                                    <div class="divRow">
                                                        <roUserControls:roTabContainerClient ID="tabCtl01" runat="server">
                                                            <TabTitle1>
                                                                <asp:Label ID="lblPresence" runat="server" Text="Para presencia"></asp:Label>
                                                            </TabTitle1>
                                                            <TabContainer1>
                                                                <dx:ASPxRadioButtonList ID="optList" runat="server" ValueType="System.String" Border-BorderStyle="None">
                                                                    <ClientSideEvents SelectedIndexChanged="function() {hasChanges(true);}" />
                                                                </dx:ASPxRadioButtonList>
                                                            </TabContainer1>
                                                        </roUserControls:roTabContainerClient>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <!-- Pantalla Puertas Abiertas -->
                                        <div id="div01" class="contentPanel" runat="server" name="menuPanel">
                                            <div class="panBottomMargin">
                                                <div class="panHeader2 panBottomMargin">
                                                    <span class="panelTitleSpan">
                                                        <asp:Label runat="server" ID="lblAccessZonesMainTitlePeriods" Text="Período de puertas abiertas"></asp:Label>
                                                    </span>
                                                </div>
                                                <div class="panelHeaderContent">
                                                    <div class="panelDescriptionImage">
                                                        <img alt="" src="<%=Me.Page.ResolveUrl("~/Base/Images/StartMenuIcos/AccessStatus.png")%>" />
                                                    </div>
                                                    <div class="panelDescriptionText">
                                                        <asp:Label ID="lblAccessZonesMainTitlePeriodsDescription" runat="server" Text="Aquí puede definir todos los periodos de zonas abiertas con sus respectivas excepciones" CssClass="spanEmp-class"></asp:Label>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="splitDivLeft">
                                                <div class="panBottomMargin">
                                                    <div class="panHeader2 panBottomMargin">
                                                        <span class="panelTitleSpan">
                                                            <asp:Label runat="server" ID="lblZonesInactivityTitle" Text="Periodos de acceso libre"></asp:Label>
                                                        </span>
                                                    </div>
                                                    <div class="panelHeaderContent">
                                                        <div class="panelDescriptionImage">
                                                            <img alt="" src="<%=Me.Page.ResolveUrl("~/Access/Images/ZonesInactivity.png")%>" />
                                                        </div>
                                                        <div class="panelDescriptionText">
                                                            <asp:Label ID="lblZonesInactivity" runat="server" Text="Introduzca los períodos en los que el acceso a esta zona será libre." CssClass="spanEmp-class"></asp:Label>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="panBottomMargin">
                                                    <!-- Grid JS -->
                                                    <div class="jsGrid">
                                                        <asp:Label ID="lblInactivityGridDesc" runat="server" CssClass="jsGridTitle" Text="Periodos de puertas abiertas"></asp:Label>
                                                        <div class="jsgridButton">
                                                            <div id="AddZoneInactivityBtn" runat="server" class="btnFlat">
                                                                <a href="javascript: void(0)" id="btnAddZonesInactivity" runat="server" onclick="AddNewZonesInactivity();">
                                                                    <span class="btnIconAdd"></span>
                                                                    <asp:Label ID="lblAddInactivity" runat="server" Text="Añadir"></asp:Label>
                                                                </a>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <roForms:frmAddPeriod ID="frmAddPeriod1" runat="server" />

                                                    <div id="grdPeriods" class="jsGridContent" runat="server">
                                                        <!-- Aqui va el grid de Ausencias Previstas -->
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="splitDivRight">
                                                <div class="panBottomMargin">
                                                    <div class="panHeader2 panBottomMargin">
                                                        <span class="panelTitleSpan">
                                                            <asp:Label runat="server" ID="lblZonesExceptionTitle" Text="Excepciones"></asp:Label>
                                                        </span>
                                                    </div>
                                                    <div class="panelHeaderContent">
                                                        <div class="panelDescriptionImage">
                                                            <img alt="" src="<%=Me.Page.ResolveUrl("~/Access/Images/ZonesException.png")%>" />
                                                        </div>
                                                        <div class="panelDescriptionText">
                                                            <asp:Label ID="lblZonesException" runat="server" Text="Introduzca los días en los que no se aplicarán los periodos de puertas abiertas definidos. Por ejemplo, introduzca los días festivos de su empresa." CssClass="spanEmp-class"></asp:Label>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="panBottomMargin">
                                                    <!-- Grid JS -->
                                                    <div class="jsGrid">
                                                        <asp:Label ID="lblZonesExceptionGridTitle" runat="server" CssClass="jsGridTitle" Text="Excepciones"></asp:Label>
                                                        <div id="AddZoneExceptionBtn" runat="server" class="jsgridButton">
                                                            <div class="btnFlat">
                                                                <a href="javascript: void(0)" id="btnAddZonesException" runat="server" onclick="AddNewZonesExceptions()">
                                                                    <span class="btnIconAdd"></span>
                                                                    <asp:Label ID="lblAddException" runat="server" Text="Añadir"></asp:Label>
                                                                </a>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <roForms:frmAddException ID="frmAddException1" runat="server" />

                                                    <div id="grdExceptions" class="jsGridContent" runat="server">
                                                        <!-- Aqui va el grid de Ausencias Previstas -->
                                                    </div>
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
        </div>
    </div>

    <script language="javascript" type="text/javascript">

        function resizeTreeAccessZones() {
            try {
                var ctlPrefix = "<%= roTreesAccessZones.ClientID %>";
                eval(ctlPrefix + "_resizeTrees();");
            }
            catch (e) {
                showError("resizeTreeAccessZones", e);
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

            $("#divTabData").height(divHeight - 30);

            var divTreeHeight = $("#divTree").height();
            $("#ctlTreeDiv").height(divTreeHeight);
        }

        window.onresize = function () {
            resizeFrames();
            resizeTreeAccessZones();
        }

        if (document.getElementById('<%= noRegs.ClientID %>').value != '') {
            cargaAccessZone(-1);
        }
    </script>
</asp:Content>