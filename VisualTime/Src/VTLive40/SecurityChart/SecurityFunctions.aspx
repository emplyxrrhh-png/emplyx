<%@ Page Language="VB" MasterPageFile="~/Base/mastEmp.master" AutoEventWireup="false" Inherits="VTLive40.SecurityFunctions" Title="Página sin título" CodeBehind="SecurityFunctions.aspx.vb" %>

<%@ Register Src="~/Base/WebUserControls/frmBusinessCenterSelector.ascx" TagPrefix="roForms" TagName="frmBusinessCenterSelector" %>

<asp:Content ID="cMainBody" ContentPlaceHolderID="contentMainBody" runat="Server">

    <script language="javascript" type="text/javascript">
        var passportLoaded = false;

        function PageBase_Load() {
            resizeFrames();
            resizePassportTrees();
            checkPermission();
            //ConvertControls('divContent');
        }
    </script>

    <div id="divModalBgDisabled" class="modalBackground" style="position: absolute; left: 0px; top: 0px; z-index: 990; width: 1680px; height: 900px; display: none;"></div>
    <input type="hidden" runat="server" id="hdnValueGridName" />

    <dx:ASPxCallback ID="PermissionCallback" runat="server" ClientInstanceName="PermissionCallbackClient" ClientSideEvents-CallbackComplete="PermissionCallback_CallbackComplete"></dx:ASPxCallback>

    <input type="hidden" id="msgHasChanges" value="" runat="server" clientidmode="Static" />
    <input type="hidden" id="msgHasErrors" value="" runat="server" clientidmode="Static" />
    <input type="hidden" id="hdnExistingExternalIds" runat="server" value="" />                                                                                                                        

    <div id="divMainBody">
        <!-- TAB SUPERIOR -->
        <div id="divTabInfo" class="divDataCells">
            <div style="min-height: 10px"></div>
            <div id="divSecurityFunction" class="blackRibbonTitle">
            </div>
            <div style="min-height: 10px"></div>
        </div>
        <!-- FIN TAB SUPERIOR -->

        <!-- ARBOL Y DETALLE -->
        <div id="divTabData" class="divDataCells">

            <div id="divTree" class="treeSize">
                <div id="ctlTreeDiv" style="height: 500px;">
                    <rws:roTreesSelector ID="roTreesSecurityFunctions" runat="server" ShowEmployeeFilters="false" PrefixTree="roTreesSecurityFunctions"
                        Tree1Visible="true" Tree1MultiSel="false" Tree1ShowOnlyGroups="false" Tree1Function="cargaNodo" Tree1ImagePath="images/SecurityFunctionsSelector"
                        Tree1SelectorPage="../../SecurityChart/SecurityFunctionsSelectorData.aspx" ShowTreeCaption="true"></rws:roTreesSelector>
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
                                    <!-- PANEL GENERAL -->
                                    <div id="divGeneral" class="contentPanel" style="display: none;" runat="server">
                                        <!-- Este div es el header -->
                                        <div class="panBottomMargin">
                                            <div class="panHeader2 panBottomMargin">
                                                <span class="panelTitleSpan">
                                                    <asp:Label runat="server" ID="lblSecurityFunctionName" Text="General"></asp:Label>
                                                </span>
                                            </div>
                                            <!-- La descripción es opcional -->
                                            <div class="panelHeaderContent">
                                                <div class="panelDescriptionImage">
                                                    <img alt="" src="<%=Me.Page.ResolveUrl("~/SecurityChart/Images/SecurityFunctions.png")%>" />
                                                </div>
                                                <div class="panelDescriptionText">
                                                    <asp:Label ID="lblDinningRoomGeneralDesc" runat="server" Text="Datos especificos de las funciones para asignar a un supervisor."></asp:Label>
                                                </div>
                                            </div>
                                        </div>

                                        <!-- Este div es un formulario -->
                                        <div class="panBottomMargin">
                                            <div class="divRow">
                                                <div class="divRowDescription">
                                                    <asp:Label ID="lblNameDescription" runat="server" Text="Nombre identificativo de la función"></asp:Label>
                                                </div>
                                                <asp:Label ID="Label1" runat="server" Text="Nombre:" CssClass="labelForm"></asp:Label>
                                                <div class="componentForm">
                                                    <dx:ASPxTextBox ID="txtName" runat="server" ClientInstanceName="txtName_Client" NullText="_____">
                                                        <ClientSideEvents Validation="LengthValidation" TextChanged="function(s,e){checkDinningRoomEmptyName(s.GetValue());onRoleNameChange();}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                        <ValidationSettings SetFocusOnError="True">
                                                            <RequiredField IsRequired="True" ErrorText="(*)" />
                                                        </ValidationSettings>
                                                    </dx:ASPxTextBox>
                                                </div>
                                            </div>

                                            <div class="divRow">
                                                <div class="divRowDescription">
                                                    <asp:Label ID="lblDescSecurityDescription" runat="server" Text="Descripción de función de seguridad"></asp:Label>
                                                </div>
                                                <asp:Label ID="lblDescCamera" runat="server" Text="Descripción:" CssClass="labelForm"></asp:Label>
                                                <div class="componentForm">
                                                    <dx:ASPxMemo ID="txtDescription" runat="server" Rows="4" Width="100%" Height="40">
                                                        <ClientSideEvents TextChanged="function(s,e){ hasChanges(true)}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                    </dx:ASPxMemo>
                                                </div>
                                            </div>
                                                <div class="divRow">
                                                    <div class="divRowDescription">
                                                        <asp:Label ID="lblExportName" runat="server" Text="Equivalencia"></asp:Label>
                                                    </div>
                                                    <asp:Label ID="lblExport" runat="server" Text="Equivalencia:" CssClass="labelForm"></asp:Label>
                                                    <div class="componentForm">
                                                        <dx:ASPxTextBox ID="txtExport" runat="server" MaxLength="15" ClientInstanceName="txtExport_Client">
                                                            <ClientSideEvents TextChanged="function(s,e){hasChanges(true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                            <MaskSettings IncludeLiterals="None" />
                                                        </dx:ASPxTextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                                                                                <ClientSideEvents Validation="LengthValidation" TextChanged="function(s,e){checkShiftEmptyName(s.GetValue());}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />

                                        <!-- Este div es un formulario -->
                                        <div id="divBusinessRoles" runat="server" class="panBottomMargin">
                                            <table width="100%">
                                                <tr>
                                                    <td>
                                                        <div class="panHeader2">
                                                            <span style="">
                                                                <asp:Label runat="server" ID="lblDefTitle2" Text="Grupos de Negocio"></asp:Label></span>
                                                        </div>
                                                        <br />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td valign="top" align="left">
                                                        <div class="RoundCornerFrame roundCorner">
                                                            <table border="0" style="width: 100%;">
                                                                <tr>
                                                                    <td>
                                                                        <asp:Label ID="lblSelectBusinessGroup" CssClass="editTextFormat" runat="server" Text="Seleccione Grupos de Negocio"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <table style="width: 100%;">
                                                                            <tr>
                                                                                <td style="padding: 5px;">
                                                                                    <roUserControls:roOptionPanelClient ID="optBGListAll" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Enabled="True" Value="0" CConClick="hasChanges(true);">
                                                                                        <Title>
                                                                                            <asp:Label ID="lblVisibilityAllTitle" runat="server" Text="Todos"></asp:Label>
                                                                                        </Title>
                                                                                        <Description>
                                                                                            <asp:Label ID="lblVisibilityAllDesc" runat="server" Text="Este grupo de usuarios tendrá permiso sobre todos los grupos de negocio."></asp:Label>
                                                                                        </Description>
                                                                                        <Content>
                                                                                        </Content>
                                                                                    </roUserControls:roOptionPanelClient>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td style="padding: 5px;">
                                                                                    <roUserControls:roOptionPanelClient ID="optBGListValue" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Enabled="True" Value="1" CConClick="hasChanges(true);">
                                                                                        <Title>
                                                                                            <asp:Label ID="lblVisibilityCriteriaTitle" runat="server" Text="Los indicados a continuación"></asp:Label>
                                                                                        </Title>
                                                                                        <Description>
                                                                                            <asp:Label ID="lblVisibilityCriteriaDesc" runat="server" Text="Este grupo de usuarios tendrá permiso sólo sobre los grupos de negocio especificados."></asp:Label>
                                                                                        </Description>
                                                                                        <Content>
                                                                                            <div class="divRow">
                                                                                                <div id="lstBusinessGroups">
                                                                                                </div>
                                                                                            </div>
                                                                                        </Content>
                                                                                    </roUserControls:roOptionPanelClient>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </div>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                        <div class="panBottomMargin">
                                            <table id="tbBusinessCenter" runat="server" width="100%">
                                                <tr>
                                                    <td>
                                                        <div class="panHeader2">
                                                            <span style="">
                                                                <asp:Label runat="server" ID="lblDefTitle3" Text="Centros de Coste"></asp:Label></span>
                                                        </div>
                                                        <br />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td valign="top" align="left" style="padding-top: 5px;">
                                                        <div class="RoundCornerFrame roundCorner">
                                                            <asp:Label ID="lblBusinessCenterDesc" CssClass="editTextFormat" Style="padding-left: 13px;" runat="server" Text="Este grupo de usuarios tendrá permiso sobre los Centros de Coste especificados."></asp:Label>

                                                            <table style="padding-left: 13px;">
                                                                <tr>
                                                                    <td style="padding-right: 10px; width: 100%">
                                                                        <input type="text" id="txtBusinessCenter" runat="server" style="width: 100%;" convertcontrol="TextField" class="textClass" readonly="readonly" />
                                                                        <br />
                                                                    </td>
                                                                    <td valign="middle" style="padding-left: 3px; padding-top: 3px;">
                                                                        <div id="btAddBusinessCenter" runat="server" class="securityAdd">
                                                                            <img id="img3" alt="" src="~/Base/Images/Grid/add.png" visible="true" runat="server" title='<%# Me.Language.Translate("addBusinessGroup",Me.DefaultScope) %>' style="cursor: pointer;" onclick="ShowBusinessCenter(); " />
                                                                        </div>
                                                                        <roForms:frmBusinessCenterSelector runat="server" ID="frmBusinessCenterSelector" />
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </div>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </div>

                                    <!-- PANEL PERMISOS -->
                                    <div id="divPermissions" class="contentPanel" style="display: none;" runat="server">
                                        <div class="panHeader2">
                                            <span style="">
                                                <asp:Label runat="server" ID="lblPermissionsTitle" Text="Permisos"></asp:Label></span>
                                        </div>
                                        <br />
                                        <table border="0" width="99%" style="height: 50px; table-layout: fixed;">
                                            <tr>
                                                <td width="48px" height="48px" style="padding-left: 20px;">
                                                    <img src="Images/Permissions_48.png" alt="" style="border: 0;" />
                                                </td>
                                                <td valign="top" align="left">
                                                    <span id="spanPermissions" runat="server" class="spanEmp-Class" style="cursor: default;">
                                                        <asp:Label ID="lblPermissions" runat="server" Text="En esta pantalla se pueden modificar los permisos para las distintas funcionalidades ..."></asp:Label>
                                                    </span>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2" style="padding: 10px 10px 10px 10px;">
                                                    <div id="divPermissionsTable" runat="server"></div>
                                                </td>
                                            </tr>
                                        </table>
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

    <script language="javascript" type="text/javascript">
        function resizePassportTrees() {
            try {
                var ctlPrefix = "<%= roTreesSecurityFunctions.ClientID%>";
                eval(ctlPrefix + "_resizeTrees();");

            }
            catch (e) {
                showError("resizePassportTrees", e);
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
            resizePassportTrees();
        }
    </script>
</asp:Content>