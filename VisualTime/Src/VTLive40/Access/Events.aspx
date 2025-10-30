<%@ Page Language="VB" MasterPageFile="~/Base/mastEmp.master" AutoEventWireup="false" Inherits="VTLive40.Events" Title="Eventos" CodeBehind="Events.aspx.vb" %>

<%@ Register Src="~/Access/WebUserForms/frmAddAuthorization.ascx" TagName="frmAddAuthorization" TagPrefix="roForms" %>

<asp:Content ID="cMainBody" ContentPlaceHolderID="contentMainBody" runat="Server">

    <script language="javascript" type="text/javascript">

        function PageBase_Load() {
            resizeFrames();
            resizeTreeEvents();
        }

        //Recarrega corresponent al clickar el arbre
        function TabClick(numTab) {
            try {
                showLoadingGrid(true);
                resizeTreeEvents();
                if (numTab == '1') { //Events
                    var ctlPrefix = "ctl00_contentMainBody_roTreesEvents_roTrees";
                    eval(ctlPrefix + ".clickTree('1');");
                }
            } catch (e) { showError("TabClick", e); }
        }

        function GridAccessGroups_BeginCallback(e, c) {

        }

        function GridAccessGroups_EndCallback(s, e) {
            if (s.IsEditing()) { }
            else {
                hasChanges(true);
            }
        }

        function GridAccessGroups_OnRowDblClick(s, e) {

        }

        function GridAccessGroups_FocusedRowChanged(s, e) {

        }

        function AddNewAccessGroup(s, e) {
            var grid = ASPxClientGridView.Cast("GridAccessGroupsClient");
            grid.AddNewRow();
        }

        function DeleteAccessGroup(IdRow) {
            grid = ASPxClientGridView.Cast("GridAccessGroupsClient");
            grid.DeleteRow(IdRow);
        }
    </script>

    <input type="hidden" runat="server" id="hdnModeEdit" value="" />
    <input type="hidden" runat="server" id="noRegs" value="" />
    <input type="hidden" id="hdnLngAuthType" value="<%= Me.Language.Translate("gridHeader.AuthType",Me.DefaultScope) %>" />
    <input type="hidden" id="hdnLngAuthorization" value="<%= Me.Language.Translate("gridHeader.Authorization",Me.DefaultScope) %>" />
    <input type="hidden" id="hdnLngDateTextSelected" runat="server" value="" />
    <input type="hidden" id="hdnLngDatesTextSelected" runat="server" value="" />
    <input type="hidden" id="hdnDates" runat="server" value="" />
    <div id="divModalBgDisabled" class="modalBackground" style="position: absolute; left: 0px; top: 0px; z-index: 990; width: 100%; height: 100%; display: none;"></div>

    <input type="hidden" id="msgHasChanges" value="" runat="server" clientidmode="Static" />
    <input type="hidden" id="msgHasErrors" value="" runat="server" clientidmode="Static" />

    <div id="divMainBody">
        <!-- TAB SUPERIOR -->
        <div id="divTabInfo" class="divDataCells">
            <div style="min-height: 10px"></div>
            <div id="divEventsScheduler" class="blackRibbonTitle">
            </div>
            <div style="min-height: 10px"></div>
        </div>
        <!-- FIN TAB SUPERIOR -->

        <!-- ARBOL Y DETALLE -->
        <div id="divTabData" class="divDataCells">

            <div id="divTree" class="treeSize">
                <div id="ctlTreeDiv" style="height: 500px;">
                    <rws:roTreesSelector ID="roTreesEvents" runat="server" ShowEmployeeFilters="false" PrefixTree="roTreesEvents"
                        Tree1Visible="true" Tree1MultiSel="false" Tree1ShowOnlyGroups="false" Tree1Function="cargaNodo" Tree1ImagePath="images/EventSchedulerSelector" Tree1SelectorPage="../../Access/EventSchedulerSelectorData.aspx"
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
                                                    <asp:Label runat="server" ID="lblGeneralTitle" Text="General"></asp:Label>
                                                </span>
                                            </div>
                                            <!-- La descripción es opcional -->
                                            <div class="panelHeaderContent">
                                                <div class="panelDescriptionImage">
                                                    <img alt="" src="<%=Me.Page.ResolveUrl("~/Base/Images/StartMenuIcos/Events.png")%>" />
                                                </div>
                                                <div class="panelDescriptionText">
                                                    <asp:Label ID="lblEventDesc" runat="server" Text="Definición de los ${Events}"></asp:Label>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="divRow">
                                            <div class="divRowDescription">
                                                <asp:Label ID="lblNameDescription" runat="server" Text="Nombre del ${Event}"></asp:Label>
                                            </div>
                                            <asp:Label ID="lblName" runat="server" Text="Nombre:" CssClass="labelForm"></asp:Label>
                                            <div class="componentForm">
                                                <dx:ASPxTextBox ID="txtName" runat="server" ClientInstanceName="txtName_Client" NullText="_____" Width="300">
                                                    <ClientSideEvents Validation="LengthValidation" TextChanged="function(s,e){hasChanges(true);checkEventSchedulerEmptyName(s.GetValue());}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                    <ValidationSettings SetFocusOnError="True">
                                                        <RequiredField IsRequired="True" ErrorText="(*)" />
                                                    </ValidationSettings>
                                                </dx:ASPxTextBox>
                                            </div>
                                        </div>

                                        <!-- Este div es un formulario -->
                                        <div class="panBottomMargin">
                                            <div class="divRow">
                                                <div class="divRowDescription">
                                                    <asp:Label ID="lblDateDescription" runat="server" Text="Fecha del ${Event}"></asp:Label>
                                                </div>
                                                <asp:Label ID="lblDate" runat="server" Text="Fecha:" CssClass="labelForm"></asp:Label>
                                                <div class="componentForm">
                                                    <dx:ASPxDateEdit runat="server" ID="txtDate" Width="125" ClientInstanceName="txtDateClient" AllowNull="false">
                                                        <CalendarProperties ShowClearButton="false" />
                                                        <ClientSideEvents DateChanged="function(s,e) {hasChanges(true);}" />
                                                        <ValidationSettings SetFocusOnError="True">
                                                            <RequiredField IsRequired="True" ErrorText="(*)" />
                                                        </ValidationSettings>
                                                    </dx:ASPxDateEdit>
                                                </div>
                                            </div>

                                            <div class="divRow">
                                                <div class="divRowDescription">
                                                    <asp:Label ID="lblEndDateDescription" runat="server" Text="Fecha fin del ${Event}"></asp:Label>
                                                </div>
                                                <asp:Label ID="lblEndDate" runat="server" Text="Fecha fin:" CssClass="labelForm"></asp:Label>
                                                <div class="componentForm">
                                                    <dx:ASPxDateEdit runat="server" ID="txtEndDate" Width="125" ClientInstanceName="txtEndDateClient" AllowNull="false">
                                                        <CalendarProperties ShowClearButton="false" />
                                                        <ClientSideEvents DateChanged="function(s,e) {hasChanges(true);}" />
                                                        <ValidationSettings SetFocusOnError="True">
                                                            <RequiredField IsRequired="True" ErrorText="(*)" />
                                                        </ValidationSettings>
                                                    </dx:ASPxDateEdit>
                                                </div>
                                            </div>

                                            <div class="divRow">
                                                <div class="divRowDescription">
                                                    <asp:Label ID="lblMainDateDescription" runat="server" Text="Fecha principal del ${Event}"></asp:Label>
                                                </div>
                                                <asp:Label ID="lblMainDate" runat="server" Text="Fecha principal:" CssClass="labelForm"></asp:Label>
                                                <div class="componentForm">
                                                    <dx:ASPxDateEdit runat="server" ID="txtMainDate" Width="125" ClientInstanceName="txtMainDateClient" AllowNull="false">
                                                        <CalendarProperties ShowClearButton="false" />
                                                        <ClientSideEvents DateChanged="function(s,e) {hasChanges(true);}" />
                                                        <ValidationSettings SetFocusOnError="True">
                                                            <RequiredField IsRequired="True" ErrorText="(*)" />
                                                        </ValidationSettings>
                                                    </dx:ASPxDateEdit>
                                                </div>
                                            </div>

                                            <div class="divRow">
                                                <div class="divRowDescription">
                                                    <asp:Label ID="lblShortNameDescription" runat="server" Text="Nombre corto del ${Event}"></asp:Label>
                                                </div>
                                                <asp:Label ID="lblShortName" runat="server" Text="Nombre corto:" CssClass="labelForm"></asp:Label>
                                                <div class="componentForm">
                                                    <dx:ASPxTextBox ID="txtShortName" runat="server" ClientInstanceName="txtShortName_Client" NullText="______" MaxLength="6" Width="60">
                                                        <ClientSideEvents Validation="LengthValidation" TextChanged="function(s,e){hasChanges(true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                        <ValidationSettings SetFocusOnError="True">
                                                            <RequiredField IsRequired="True" ErrorText="(*)" />
                                                        </ValidationSettings>
                                                    </dx:ASPxTextBox>
                                                </div>
                                            </div>

                                            <div class="divRow">
                                                <div class="divRowDescription">
                                                    <asp:Label ID="lblDescDescription" runat="server" Text="Descripción del ${Event}"></asp:Label>
                                                </div>
                                                <asp:Label ID="lblDescription" runat="server" Text="Descripción:" CssClass="labelForm"></asp:Label>
                                                <div class="componentForm">
                                                    <dx:ASPxMemo ID="txtDescription" runat="server" Rows="5" Width="100%" Height="40">
                                                        <ClientSideEvents TextChanged="function(s,e){ hasChanges(true)}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                    </dx:ASPxMemo>
                                                </div>
                                            </div>
                                        </div>

                                        <!-- Este div es el header -->
                                        <div class="panBottomMargin">
                                            <div class="panHeader2 panBottomMargin">
                                                <span class="panelTitleSpan">
                                                    <asp:Label runat="server" ID="lblAutorizations" Text="${Authorizations}"></asp:Label>
                                                </span>
                                            </div>
                                            <!-- La descripción es opcional -->
                                            <div class="panelHeaderContent">
                                                <div class="panelDescriptionImage">
                                                    <img alt="" src="<%=Me.Page.ResolveUrl("~/Access/Images/AccessGroup.png")%>" />
                                                </div>
                                                <div class="panelDescriptionText">
                                                    <asp:Label ID="Label2" runat="server" Text="${Authorizations} asignadas al ${Event}"></asp:Label>
                                                </div>
                                            </div>
                                        </div>

                                        <div id="generalReportType">
                                            <!-- Grid JS -->
                                            <div class="jsGrid">
                                                <asp:Label ID="lblContractsCaption" runat="server" CssClass="jsGridTitle" Text="Autorizaciones"></asp:Label>
                                                <div id="panTbAuthorization" runat="server" class="jsgridButton">
                                                    <div class="btnFlat">
                                                        <a href="javascript: void(0)" id="btnAddNewAccessGroup" runat="server" onclick="AddNewAuthorization();">
                                                            <span class="btnIconAdd"></span>
                                                            <asp:Label ID="lblAddAuthorization" runat="server" Text="Añadir"></asp:Label>
                                                        </a>
                                                    </div>
                                                </div>
                                            </div>
                                            <div id="grdAuthorizations" class="jsGridContent" style="width: 100%;">
                                                <%-- <dx:ASPxGridView ID="GridAccessGroups" runat="server" AutoGenerateColumns="False" ClientInstanceName="GridAccessGroupsClient" KeyboardSupport="True" Width="100%" ClientSideEvents-BeginCallback="GridAccessGroups_BeginCallback">
                                                    <Settings ShowTitlePanel="False" VerticalScrollBarMode="Auto" UseFixedTableLayout="True" VerticalScrollableHeight="200" />
                                                    <ClientSideEvents EndCallback="GridAccessGroups_EndCallback" RowDblClick="GridAccessGroups_OnRowDblClick" FocusedRowChanged="GridAccessGroups_FocusedRowChanged" />
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
                                                </dx:ASPxGridView>--%>
                                            </div>
                                            <!-- Fin Barra Herramientas Destinations -->
                                            <roForms:frmAddAuthorization ID="frmAddAuthorization1" runat="server" />
                                        </div>

                                        <%-- <div style="width: calc(100% - 40px)">
                                            <div class="jsGrid">
                                                <asp:Label ID="lblContractsCaption" runat="server" CssClass="jsGridTitle" Text="Autorizaciones"></asp:Label>
                                                <div class="jsgridButton">
                                                    <dx:ASPxButton ID="btnAddNewAccessGroup" runat="server" AutoPostBack="False" CausesValidation="False" Text="Añadir" ToolTip="Añadir nueva autorización" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat">
                                                        <Image Url="~/Base/Images/Grid/add.png"></Image>
                                                        <ClientSideEvents Click="AddNewAccessGroup" />
                                                    </dx:ASPxButton>
                                                </div>
                                            </div>
                                            <div class="jsGridContent">
                                                <dx:ASPxGridView ID="GridAccessGroups" runat="server" AutoGenerateColumns="False" ClientInstanceName="GridAccessGroupsClient" KeyboardSupport="True" Width="100%" ClientSideEvents-BeginCallback="GridAccessGroups_BeginCallback">
                                                    <Settings ShowTitlePanel="False" VerticalScrollBarMode="Auto" UseFixedTableLayout="True" VerticalScrollableHeight="200" />
                                                    <ClientSideEvents EndCallback="GridAccessGroups_EndCallback" RowDblClick="GridAccessGroups_OnRowDblClick" FocusedRowChanged="GridAccessGroups_FocusedRowChanged" />
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
                                        </div>--%>
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

        <dx:ASPxPopupControl ID="EventPeriodPopup" runat="server" AllowDragging="False" CloseAction="None" Modal="True" ContentUrl="~/Base/Popups/CreateObjectPopup.aspx"
            PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter" Width="470px" Height="300px"
            ShowHeader="False" ScrollBars="Auto" ShowPageScrollbarWhenModal="True" ClientInstanceName="EventPeriodPopup_Client" PopupAnimationType="None" BackColor="Transparent" ContentStyle-Paddings-Padding="0px" Border-BorderColor="Transparent" ShowShadow="false">
            <SettingsLoadingPanel Enabled="false" />
        </dx:ASPxPopupControl>

        <dx:ASPxPopupControl ID="CopyEventPopup" runat="server" AllowDragging="False" CloseAction="None" Modal="True" ContentUrl=""
            PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter" Width="400" Height="150px"
            ShowHeader="False" ScrollBars="Auto" ShowPageScrollbarWhenModal="True" ClientInstanceName="CopyEventPopup_Client" PopupAnimationType="None" BackColor="Transparent" ContentStyle-Paddings-Padding="0px" Border-BorderColor="Transparent" ShowShadow="false">
            <SettingsLoadingPanel Enabled="false" />

            <ContentCollection>
                <dx:PopupControlContentControl ID="PopupControlContentControl2" runat="server">
                    <dx:ASPxPanel ID="ASPxPanel3" runat="server" Width="0px" Height="0px">
                        <PanelCollection>
                            <dx:PanelContent ID="PanelContent3" runat="server">
                                <div class="bodyPopupExtended" style="table-layout: fixed; height: 120px; width: 315px; background-color: white;">
                                    <div class="panHeader2">
                                        <span style="">
                                            <asp:Label ID="lblNewDateDesc" runat="server" Text="Fecha del nuevo evento"></asp:Label></span>
                                    </div>

                                    <table id="tbPopupFrame" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td valign="top">
                                                <div>
                                                    <asp:Label ID="lblNewDate" runat="server" Text="Fecha:" CssClass="labelForm"></asp:Label>
                                                    <div class="componentForm">
                                                        <dx:ASPxDateEdit runat="server" ID="txtNewDate" ClientInstanceName="txtNewDateClient" AllowNull="false">
                                                            <CalendarProperties ShowClearButton="false" />
                                                            <ClientSideEvents DateChanged="function(s,e) {hasChanges(true);}" />
                                                        </dx:ASPxDateEdit>
                                                    </div>
                                                </div>
                                            </td>
                                        </tr>
                                        <br />
                                        <tr style="height: 35px;">

                                            <td align="center">
                                                <table>
                                                    <tr>
                                                        <td>
                                                            <br />
                                                            <dx:ASPxButton ID="btnPopupSelectorDateAccept" runat="server" AutoPostBack="False" CausesValidation="False" Text="${Button.Accept}" ToolTip="${Button.Accept}" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                                                <ClientSideEvents Click="btnPopupSelectorDateAcceptClient_Click" />
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
    </div>

    <script language="javascript" type="text/javascript">
        function resizeTreeEvents() {
            try {
                var ctlPrefix = "<%= roTreesEvents.ClientID %>";
                eval(ctlPrefix + "_resizeTrees();");
            }
            catch (e) {
                showError("resizeTreeEvents", e);
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
            resizeTreeEvents();
        }

        if (document.getElementById('<%= noRegs.ClientID %>').value != '') {
            cargaEventScheduler('-1');
        }
    </script>
</asp:Content>