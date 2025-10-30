<%@ Page Language="VB" MasterPageFile="~/Base/mastEmp.master" AutoEventWireup="false" Inherits="VTLive40.AccessPeriods" Title="${AccessPeriods}" CodeBehind="AccessPeriods.aspx.vb" %>

<%@ Register Src="~/Access/WebUserForms/frmNewPeriod.ascx" TagName="frmNewPeriod" TagPrefix="roForms" %>

<asp:Content ID="cMainBody" ContentPlaceHolderID="contentMainBody" runat="Server">

    <script language="javascript" type="text/javascript">

        function PageBase_Load() {
            resizeFrames();
            resizeTreeAccessPeriods();

            venableOPC('frmNewPeriod1_opTypePeriodNormal');
            venableOPC('frmNewPeriod1_opTypePeriodEspecific');
            linkOPCItems('frmNewPeriod1_opTypePeriodNormal,frmNewPeriod1_opTypePeriodEspecific');
        }
    </script>

    <input type="hidden" runat="server" id="hdnModeEdit" value="" />
    <input type="hidden" runat="server" id="noRegs" value="" />
    <input type="hidden" id="hdnStrDescription" value="<%= Me.Language.Translate("gridHeader.Description", Me.DefaultScope) %>" />
    <div id="divModalBgDisabled" class="modalBackground" style="position: absolute; left: 0px; top: 0px; z-index: 990; width: 100%; height: 100%; display: none;"></div>

    <input type="hidden" id="msgHasChanges" value="" runat="server" clientidmode="Static" />
    <input type="hidden" id="msgHasErrors" value="" runat="server" clientidmode="Static" />

    <div id="divMainBody">
        <!-- TAB SUPERIOR -->
        <div id="divTabInfo" class="divDataCells" style="">
            <div style="min-height: 10px"></div>
            <div id="divAccessPeriod" class="blackRibbonTitle">
            </div>
            <div style="min-height: 10px"></div>
        </div>
        <!-- FIN TAB SUPERIOR -->

        <!-- ARBOL Y DETALLE -->
        <div id="divTabData" class="divDataCells">

            <div id="divTree" class="treeSize">
                <div id="ctlTreeDiv" style="height: 500px;">
                    <rws:roTreesSelector ID="roTreesAccessPeriods" runat="server" ShowEmployeeFilters="false" PrefixTree="roTreesAccessPeriods"
                        Tree1Visible="true" Tree1MultiSel="false" Tree1ShowOnlyGroups="false" Tree1Function="cargaNodo" Tree1ImagePath="images/AccessPeriodsSelector" Tree1SelectorPage="../../Access/AccessPeriodsSelectorData.aspx"
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
                                    <!-- Pantalla Grupos -->
                                    <div id="div00" class="contentPanel" name="menuPanel" runat="server">
                                        <!-- Este div es el header -->
                                        <div class="panBottomMargin">
                                            <div class="panHeader2 panBottomMargin">
                                                <span class="panelTitleSpan">
                                                    <asp:Label runat="server" ID="lblAccessPeriodsGeneral" Text="General"></asp:Label>
                                                </span>
                                            </div>
                                            <!-- La descripción es opcional -->
                                            <div class="panelHeaderContent">
                                                <div class="panelDescriptionImage">
                                                    <img alt="" src="<%=Me.Page.ResolveUrl("~/Access/Images/ZonePeriods.png")%>" />
                                                </div>
                                                <div class="panelDescriptionText">
                                                    <asp:Label ID="lblAccessPeriodsGeneralDescription" runat="server" Text="Datos generales un periodo de acceso."></asp:Label>
                                                </div>
                                            </div>
                                        </div>

                                        <!-- Este div es un formulario -->
                                        <div class="panBottomMargin">
                                            <div class="divRow">
                                                <div class="divRowDescription">
                                                    <asp:Label ID="lblNameDescription" runat="server" Text="Nombre del periodo de acceso"></asp:Label>
                                                </div>
                                                <asp:Label ID="lblName" runat="server" Text="Nombre:" CssClass="labelForm"></asp:Label>
                                                <div class="componentForm">
                                                    <dx:ASPxTextBox ID="txtName" runat="server" ClientInstanceName="txtName_Client" NullText="_____">
                                                        <ClientSideEvents Validation="LengthValidation" TextChanged="function(s,e){checkAccessPeriodEmptyName(s.GetValue());}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                        <ValidationSettings SetFocusOnError="True">
                                                            <RequiredField IsRequired="True" ErrorText="(*)" />
                                                        </ValidationSettings>
                                                    </dx:ASPxTextBox>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="panBottomMargin">
                                            <div class="jsGrid">
                                                <asp:Label ID="lbl" runat="server" CssClass="jsGridTitle" Text="Zonas de acceso"></asp:Label>
                                                <div class="jsgridButton">
                                                    <div class="btnFlat">
                                                        <a href="javascript: void(0)" id="btnAddAccessPeriods" runat="server" onclick="AddNewGridPeriod();">
                                                            <span class="btnIconAdd"></span>
                                                            <asp:Label ID="lblAddAccessGroups" runat="server" Text="Añadir"></asp:Label>
                                                        </a>
                                                    </div>
                                                </div>
                                            </div>

                                            <div id="grdPeriods" class="jsGridContent" runat="server">
                                            </div>
                                            <roForms:frmNewPeriod ID="frmNewPeriod1" runat="server" />
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

        function resizeTreeAccessPeriods() {
            try {
                var ctlPrefix = "<%= roTreesAccessPeriods.ClientID %>";
                eval(ctlPrefix + "_resizeTrees();");
            }
            catch (e) {
                showError("resizeTreeAccessPeriods", e);
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
            resizeTreeAccessPeriods();
        }

        if (document.getElementById('<%= noRegs.ClientID %>').value != '') {
            cargaAccessPeriod(-1);
        }
    </script>
</asp:Content>