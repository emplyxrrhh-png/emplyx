<%@ Page Language="VB" MasterPageFile="~/Base/mastEmp.master" AutoEventWireup="false" Inherits="VTLive40.AccessGroups" Title="${AccessGroups}" CodeBehind="AccessGroups.aspx.vb" %>

<%@ Register Src="~/Access/WebUserForms/frmNewAccPermission.ascx" TagName="frmNewAccPermission" TagPrefix="roForms" %>

<asp:Content ID="cMainBody" ContentPlaceHolderID="contentMainBody" runat="Server">

    <script language="javascript" type="text/javascript">

        function PageBase_Load() {
            resizeFrames();
            resizeTreeAccessGroups();
        }
    </script>

    <input type="hidden" runat="server" id="noRegs" value="" />
    <input type="hidden" runat="server" id="hdnModeEdit" value="" />
    <input type="hidden" runat="server" id="hdnModeEditEmployees" value="" />

    <input type="hidden" id="hdnStrZoneName" value="<%= Me.Language.Translate("gridHeader.ZoneName",Me.DefaultScope) %>" />
    <input type="hidden" id="hdnStrPeriodName" value="<%= Me.Language.Translate("gridHeader.ZonePeriod",Me.DefaultScope) %>" />
    <input type="hidden" id="hdnStrEmpName" value="<%= Me.Language.Translate("gridHeaderEmp.EmployeeName",Me.DefaultScope) %>" />

    <input type="hidden" id="hdnEmployeeUrl" value="<%= Me.ResolveUrl("~/Base/Images/EmployeeSelector/Empleado-16x16.gif") %>" />
    <input type="hidden" id="hdnGroupUrl" value="<%= Me.ResolveUrl("~/Base/Images/EmployeeSelector/Grupos-16x16.Gif") %>" />
    <div id="divModalBgDisabled" class="modalBackground" style="position: absolute; left: 0px; top: 0px; z-index: 990; width: 100%; height: 100%; display: none;"></div>

    <input type="hidden" id="msgHasChanges" value="" runat="server" clientidmode="Static" />
    <input type="hidden" id="msgHasErrors" value="" runat="server" clientidmode="Static" />

    <div id="divMainBody">
        <!-- TAB SUPERIOR -->

        <div id="divTabInfo" class="divDataCells" style="">
            <div style="min-height: 10px"></div>
            <div id="divAccessGroup" class="blackRibbonTitle">
            </div>
            <div style="min-height: 10px"></div>
        </div>
        <!-- FIN TAB SUPERIOR -->

        <!-- ARBOL Y DETALLE -->
        <div id="divTabData" class="divDataCells">

            <div id="divTree" class="treeSize">
                <div id="ctlTreeDiv" style="height: 500px;">
                    <rws:roTreesSelector ID="roTreesAccessGroups" runat="server" ShowEmployeeFilters="false" PrefixTree="roTreesAccessGroups"
                        Tree1Visible="true" Tree1MultiSel="false" Tree1ShowOnlyGroups="false" Tree1Function="cargaNodo" Tree1ImagePath="images/AccessGroupsSelector" Tree1SelectorPage="../../Access/AccessGroupsSelectorData.aspx"
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
                                                    <asp:Label runat="server" ID="lblAccessZonesMainTitleGroups" Text="Grupos de acceso"></asp:Label>
                                                </span>
                                            </div>
                                            <!-- La descripción es opcional -->
                                            <div class="panelHeaderContent">
                                                <div class="panelDescriptionImage">
                                                    <img alt="" src="<%=Me.Page.ResolveUrl("~/Access/Images/AccessGroup.png")%>" />
                                                </div>
                                                <div class="panelDescriptionText">
                                                    <asp:Label ID="lblAccessGroups" runat="server" Text="Datos generales sobre grupo de acceso. En esta pantalla puede ver y cambiar los datos generales de este grupo de acceso. Los grupos de acceso nos permiten agrupar las restricciones de tiempo y zonas a los empleados pertenecientes."></asp:Label>
                                                </div>
                                            </div>
                                        </div>
                                        <!-- Este div es un formulario -->
                                        <div class="panBottomMargin">
                                            <div class="divRow">
                                                <div class="divRowDescription">
                                                    <asp:Label ID="lblNameDescription" runat="server" Text="Nombre grupo de acceso"></asp:Label>
                                                </div>
                                                <asp:Label ID="lblName" runat="server" Text="Nombre:" CssClass="labelForm"></asp:Label>
                                                <div class="componentForm">
                                                    <dx:ASPxTextBox ID="txtName" runat="server" ClientInstanceName="txtName_Client" NullText="_____">
                                                        <ClientSideEvents Validation="LengthValidation" TextChanged="function(s,e){checkAccessGroupEmptyName(s.GetValue());}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                        <ValidationSettings SetFocusOnError="True">
                                                            <RequiredField IsRequired="True" ErrorText="(*)" />
                                                        </ValidationSettings>
                                                    </dx:ASPxTextBox>
                                                </div>
                                            </div>
                                            <div class="divRow">
                                                <div class="divRowDescription">
                                                    <asp:Label ID="lblShortNameDesc" runat="server" Text="Nombre corto de la autorización utilizado para las importaciones"></asp:Label>
                                                </div>
                                                <asp:Label ID="lblShortName" runat="server" Text="Nombre corto:" CssClass="labelForm"></asp:Label>
                                                <div class="componentForm">
                                                    <dx:ASPxTextBox ID="txtShortName" runat="server" MaxLength="3" Width="50" NullText="_____" ClientInstanceName="txtShortName_Client">
                                                        <ClientSideEvents Validation="LengthValidation" TextChanged="function(s,e){ hasChanges(true,false)}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
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
                                                        <a href="javascript: void(0)" id="btnAddAccessGroups" runat="server" onclick="AddNewGridGroup();">
                                                            <span class="btnIconAdd"></span>
                                                            <asp:Label ID="lblAddAccessGroups" runat="server" Text="Añadir"></asp:Label>
                                                        </a>
                                                    </div>
                                                </div>
                                            </div>

                                            <div id="grdGroups" class="jsGridContent" runat="server">
                                                <!-- Aqui va el grid de Ausencias Previstas -->
                                            </div>

                                            <roForms:frmNewAccPermission ID="frmNewAccPermission1" runat="server" />
                                        </div>
                                    </div>

                                    <!-- Pantalla Empleados -->
                                    <div id="div01" class="contentPanel" name="menuPanel" runat="server">
                                        <!-- Este div es el header -->
                                        <div class="panBottomMargin">
                                            <div class="panHeader2 panBottomMargin">
                                                <span class="panelTitleSpan">
                                                    <asp:Label runat="server" ID="lblAccessGroupsEmployeesTitle" Text="Empleados asignados al grupo de accesos"></asp:Label>
                                                </span>
                                            </div>
                                            <!-- La descripción es opcional -->
                                            <div class="panelHeaderContent">
                                                <div class="panelDescriptionImage">
                                                    <img alt="" src="<%=Me.Page.ResolveUrl("~/Access/Images/AccessGroup.png")%>" />
                                                </div>
                                                <div class="panelDescriptionText">
                                                    <asp:Label ID="lblAccessGroupsEmployees" runat="server" Text="Empleados pertenecientes a este grupo de acceso. Seleccione el botón Añadir para acceder al asistente cambio de grupo de acceso y asignar empleados a este grupo de acceso."></asp:Label>
                                                </div>
                                            </div>
                                        </div>

                                        <!-- Este div es un formulario -->
                                        <div class="panBottomMargin">
                                            <div class="jsGrid">
                                                <asp:Label ID="lblEmployeesInGroup" runat="server" CssClass="jsGridTitle" Text="Empleados en grupo"></asp:Label>
                                                <div class="jsgridButton">
                                                    <div class="btnFlat">
                                                        <a href="javascript: void(0)" id="btnAddAccessGroupsEmployees" runat="server" onclick="ShowNewAccessGroupsWizard();">
                                                            <span class="btnIconAdd"></span>
                                                            <asp:Label ID="lblAddAccessGroupsEmployee" runat="server" Text="Añadir"></asp:Label>
                                                        </a>
                                                    </div>
                                                </div>
                                                <div class="jsgridButton">
                                                    <div class="btnFlat">
                                                        <a href="javascript: void(0)" id="btnRemoveAll" runat="server" onclick="EmptyAccessGroup();">
                                                            <span class=""></span>
                                                            <asp:Label ID="lblRemoveAll" runat="server" Text="Quitar todos los empleados"></asp:Label>
                                                        </a>
                                                    </div>
                                                </div>
                                            </div>

                                            <div id="grdEmployees" class="jsGridContent" runat="server">
                                                <!-- Aqui va el grid de Ausencias Previstas -->
                                            </div>
                                        </div>
                                    </div>

                                    <!-- Pantalla Documentos requeridos -->
                                    <div id="div02" class="contentPanel" name="menuPanel" runat="server">
                                        <!-- Este div es el header -->
                                        <div class="panBottomMargin">
                                            <div class="panHeader2 panBottomMargin">
                                                <span class="panelTitleSpan">
                                                    <asp:Label runat="server" ID="lblRequieredDocuments" Text="Documentos requeridos"></asp:Label>
                                                </span>
                                            </div>
                                            <!-- La descripción es opcional -->
                                            <div class="panelHeaderContent">
                                                <div class="panelDescriptionImage">
                                                    <img alt="" src="<%=Me.Page.ResolveUrl("~/Documents/Images/DocumentTemplate.png")%>" />
                                                </div>
                                                <div class="panelDescriptionText">
                                                    <asp:Label ID="lblRequieredDocumentsDesc" runat="server" Text="Documentos requeridos para conceder el acceso mediante la autorización. Seleccione el botón Añadir para seleccionar los documentos requeridos o elimine los ya existentes."></asp:Label>
                                                </div>
                                            </div>
                                        </div>

                                        <!-- Este div es un formulario -->
                                        <div class="panBottomMargin">
                                            <div class="jsGrid">
                                                <asp:Label ID="lblDocuments" runat="server" CssClass="jsGridTitle" Text="Documentos"></asp:Label>
                                                <div class="jsgridButton">
                                                    <dx:ASPxButton ID="btnAddDocument" runat="server" AutoPostBack="False" CausesValidation="False" Text="Añadir" ToolTip="Añadir" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat">
                                                        <Image Url="~/Base/Images/Grid/add.png"></Image>
                                                        <ClientSideEvents Click="AddDocumentsToAuthorization" />
                                                    </dx:ASPxButton>
                                                </div>
                                            </div>

                                            <div class="jsGridContent" runat="server">
                                                <dx:ASPxGridView ID="gridDocumentsAuthorized" runat="server" AutoGenerateColumns="False" ClientInstanceName="gridDocumentsAuthorizedClient" KeyboardSupport="True" Width="100%">
                                                    <Settings ShowTitlePanel="False" VerticalScrollBarMode="Auto" UseFixedTableLayout="True" VerticalScrollableHeight="125" />
                                                    <ClientSideEvents EndCallback="gridDocumentsAuthorized_EndCallback" />
                                                    <SettingsCommandButton>
                                                        <DeleteButton Image-Url="~/Base/Images/Grid/remove.png" Image-ToolTip="" />
                                                        <UpdateButton Image-Url="~/Base/Images/Grid/save.png" Image-ToolTip="" />
                                                        <CancelButton Image-Url="~/Base/Images/Grid/cancel.png" Image-ToolTip="" />
                                                        <EditButton Image-Url="~/Base/Images/Grid/edit.png" Image-ToolTip="" />
                                                    </SettingsCommandButton>
                                                    <Styles>
                                                        <AlternatingRow Enabled="True" BackColor="#d7e5ea"></AlternatingRow>
                                                        <CommandColumn Spacing="5px" />
                                                        <Header CssClass="jsGridHeaderCell" />
                                                        <Cell Wrap="False" />
                                                    </Styles>
                                                    <SettingsPager Mode="ShowAllRecords" ShowEmptyDataRows="false">
                                                    </SettingsPager>
                                                </dx:ASPxGridView>
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

            <!-- POPUP NEW OBJECT -->
            <dx:ASPxPopupControl ID="CaptchaObjectPopup" runat="server" AllowDragging="False" CloseAction="None" Modal="True" ContentUrl="~/Base/Popups/GenericCaptchaValidator.aspx"
                PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter" ModalBackgroundStyle-Opacity="0" Width="500" Height="320"
                ShowHeader="False" ScrollBars="Auto" ShowPageScrollbarWhenModal="True" ClientInstanceName="CaptchaObjectPopup_Client" PopupAnimationType="None" BackColor="Transparent" ContentStyle-Paddings-Padding="0px" Border-BorderColor="Transparent" ShowShadow="false">
                <SettingsLoadingPanel Enabled="false" />
            </dx:ASPxPopupControl>
        </div>
    </div>

    <script language="javascript" type="text/javascript">

        function resizeTreeAccessGroups() {
            try {
                var ctlPrefix = "<%= roTreesAccessGroups.ClientID %>";
                eval(ctlPrefix + "_resizeTrees();");
            }
            catch (e) {
                showError("resizeTreeAccessGroups", e);
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
            resizeTreeAccessGroups();
        }

        if (document.getElementById('<%= noRegs.ClientID %>').value != '') {
            cargaAccessGroup(-1);
        }
    </script>
</asp:Content>