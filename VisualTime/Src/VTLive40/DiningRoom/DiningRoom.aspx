<%@ Page Language="VB" MasterPageFile="~/Base/mastEmp.master" AutoEventWireup="true" Inherits="VTLive40.DiningRoom" Title="${DiningRoom}" EnableEventValidation="false" CodeBehind="DiningRoom.aspx.vb" %>

<asp:Content ID="cMainBody" ContentPlaceHolderID="contentMainBody" runat="Server">

    <script language="javascript" type="text/javascript">

        function PageBase_Load() {
            resizeFrames();
            resizeTreeDiningRoom();
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
            <div id="divDiningRoom" class="blackRibbonTitle">
            </div>
            <div style="min-height: 10px"></div>
        </div>
        <!-- FIN TAB SUPERIOR -->

        <!-- ARBOL Y DETALLE -->
        <div id="divTabData" class="divDataCells">

            <div id="divTree" class="treeSize">
                <div id="ctlTreeDiv" style="height: 500px;">
                    <rws:roTreesSelector ID="roTreesDiningRoom" runat="server" ShowEmployeeFilters="false" PrefixTree="roTreesDiningRoom"
                        Tree1Visible="true" Tree1MultiSel="false" Tree1ShowOnlyGroups="false" Tree1Function="cargaNodo" Tree1ImagePath="images/DiningRoomSelector" Tree1SelectorPage="../../DiningRoom/DiningRoomSelectorData.aspx"
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
                                                    <asp:Label runat="server" ID="lblDinningRoomGeneral" Text="General"></asp:Label>
                                                </span>
                                            </div>
                                            <!-- La descripción es opcional -->
                                            <div class="panelHeaderContent">
                                                <div class="panelDescriptionImage">
                                                    <img alt="" src="<%=Me.Page.ResolveUrl("~/DiningRoom/Images/DiningRoom.png")%>" />
                                                </div>
                                                <div class="panelDescriptionText">
                                                    <asp:Label ID="lblDinningRoomGeneralDesc" runat="server" Text="Datos especificos del comedor."></asp:Label>
                                                </div>
                                            </div>
                                        </div>

                                        <!-- Este div es un formulario -->
                                        <div class="panBottomMargin">
                                            <div class="divRow">
                                                <div class="divRowDescription">
                                                    <asp:Label ID="lblNameDescription" runat="server" Text="Nombre identificativo del comedor"></asp:Label>
                                                </div>
                                                <asp:Label ID="lblName" runat="server" Text="Nombre:" CssClass="labelForm"></asp:Label>
                                                <div class="componentForm">
                                                    <dx:ASPxTextBox ID="txtName" runat="server" ClientInstanceName="txtName_Client" NullText="_____">
                                                        <ClientSideEvents Validation="LengthValidation" TextChanged="function(s,e){checkDinningRoomEmptyName(s.GetValue());}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                        <ValidationSettings SetFocusOnError="True">
                                                            <RequiredField IsRequired="True" ErrorText="(*)" />
                                                        </ValidationSettings>
                                                    </dx:ASPxTextBox>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="panBottomMargin">
    <div class="divRow">
        <div class="divRowDescription">
            <asp:Label ID="lblExportName" runat="server" Text="La equivalencia de exportación es el valor con el que se identifica este turno en los procesos de exportación"></asp:Label>
        </div>
        <asp:Label ID="lblExport" runat="server" Text="Equivalencia:" CssClass="labelForm"></asp:Label>
        <div class="componentForm">
            <dx:ASPxTextBox ID="txtExport" runat="server" MaxLength="15">
    <ClientSideEvents TextChanged="function(s,e){hasChanges(true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
    <MaskSettings IncludeLiterals="None" />
</dx:ASPxTextBox>
        </div>
    </div>
</div>

                                        <!-- Este div es el header -->
                                        <div class="panBottomMargin">
                                            <div class="panHeader2 panBottomMargin">
                                                <span class="panelTitleSpan">
                                                    <asp:Label runat="server" ID="lblHeaderWho" Text="Quién"></asp:Label>
                                                </span>
                                            </div>
                                            <!-- La descripción es opcional -->
                                            <div class="panelHeaderContent">
                                                <div class="panelDescriptionImage">
                                                    <img alt="" src="<%=Me.Page.ResolveUrl("~/DiningRoom/Images/who.png")%>" />
                                                </div>
                                                <div class="panelDescriptionText">
                                                    <asp:Label ID="lblWhoDescription" runat="server" Text="Desde este apartado podrá seleccionar qué empleados podrán utilizar el turno"></asp:Label>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="panBottomMargin">
                                            <div class="divRow">
                                                <div class="splitDivLeft">
                                                    <roUserControls:roOptionPanelClient ID="optAll" runat="server" TypeOPanel="RadioOption" width="100%" height="Auto" Checked="True" Enabled="True" Border="True" Value="0" CConClick="hasChanges(true);">
                                                        <Title>
                                                            <asp:Label ID="lblVisibilityAllTitle" runat="server" Text="Todos"></asp:Label>
                                                        </Title>
                                                        <Description>
                                                            <asp:Label ID="lblVisibilityAllDesc" runat="server" Text="Todos los empleados tendrán acceso al comedor sin ninguna restricción."></asp:Label>
                                                        </Description>
                                                        <Content>
                                                            <div style="min-height: 71px">
                                                            </div>
                                                        </Content>
                                                    </roUserControls:roOptionPanelClient>
                                                </div>
                                                <div class="splitDivRight">
                                                    <roUserControls:roOptionPanelClient ID="optSelection" runat="server" TypeOPanel="RadioOption" width="100%" height="Auto" Checked="False" Enabled="True" Border="True" Value="1" CConClick="hasChanges(true);">
                                                        <Title>
                                                            <asp:Label ID="lblVisibilityCriteriaTitle" runat="server" Text="Según criterio"></asp:Label>
                                                        </Title>
                                                        <Description>
                                                            <asp:Label ID="lblVisibilityCriteriaDesc" runat="server" Text="Sólo los empleados que cumplan el siguiente criterio podrán acceder al comedor."></asp:Label>
                                                        </Description>
                                                        <Content>
                                                            <table border="0" width="100%" style="padding: 20px; padding-top: 5px;" align="center">
                                                                <tr>
                                                                    <td align="left" style="padding-left: 20px;">
                                                                        <table>
                                                                            <tr>
                                                                                <td>
                                                                                    <asp:Label ID="lblVisibilityDescCriteria" runat="server" Text="Aquellos empleados cuyo campo de la ficha "></asp:Label>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td>
                                                                                    <dx:ASPxComboBox ID="cmbUserFields" runat="server" Width="200px" Font-Size="11px" CssClass="editTextFormat"
                                                                                        Font-Names="Arial;Verdana;Sans-Serif" IncrementalFilteringMode="Contains">
                                                                                        <ClientSideEvents SelectedIndexChanged="function() {hasChanges(true,false);}" />
                                                                                    </dx:ASPxComboBox>
                                                                                </td>
                                                                                <td>
                                                                                    <asp:Label ID="lblVisibilityDescValor" runat="server" Text="sea igual al valor"></asp:Label>
                                                                                </td>
                                                                                <td>
                                                                                    <input type="text" id="txtUserFields" runat="server" style="width: 100px;" convertcontrol="TextField" class="textClass x-form-text x-form-field" cconchange="hasChanges(true);" />
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </Content>
                                                    </roUserControls:roOptionPanelClient>
                                                </div>
                                            </div>
                                        </div>

                                        <!-- Este div es el header -->
                                        <div class="panBottomMargin">
                                            <div class="panHeader2 panBottomMargin">
                                                <span class="panelTitleSpan">
                                                    <asp:Label runat="server" ID="lblHeaderWhen" Text="Cuando"></asp:Label>
                                                </span>
                                            </div>
                                            <!-- La descripción es opcional -->
                                            <div class="panelHeaderContent">
                                                <div class="panelDescriptionImage">
                                                    <img alt="" src="<%=Me.Page.ResolveUrl("~/DiningRoom/Images/when.png")%>" />
                                                </div>
                                                <div class="panelDescriptionText">
                                                    <asp:Label ID="lblVisibilityDescription" runat="server" Text="Desde este apartado podrá seleccionar cuando podrán acceder al comedor los empleados"></asp:Label>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="panBottomMargin">
                                            <div class="divRow">

                                                <div class="splitDivLeft">
                                                    <roUserControls:roGroupBox ID="GroupBox2" runat="server">
                                                        <Content>

                                                            <table border="0" style="width: 100%">
                                                                <tr>
                                                                    <td colspan="2" style="height: 40px;">
                                                                        <asp:Label ID="lblTime" runat="server" Text="Indique los límites de horas entre las que se permitirá el acceso:"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td valign="middle" align="left" style="width: 175px; padding-left: 30px;">
                                                                        <asp:Label ID="lblBeginTime" runat="server" Text="Permitir entrada desde las"></asp:Label>
                                                                    </td>
                                                                    <td>
                                                                        <dx:ASPxTimeEdit ID="txtBeginTime" EditFormatString="HH:mm" EditFormat="Custom" runat="server" Width="85">
                                                                            <ClientSideEvents DateChanged="function() {hasChanges(true,false);}" />
                                                                        </dx:ASPxTimeEdit>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td valign="middle" align="left" style="width: 175px; padding-left: 30px;">
                                                                        <asp:Label ID="lblEndTime" runat="server" Text="hasta las"></asp:Label>
                                                                    </td>
                                                                    <td>
                                                                        <dx:ASPxTimeEdit ID="txtEndTime" EditFormatString="HH:mm" EditFormat="Custom" runat="server" Width="85">
                                                                            <ClientSideEvents DateChanged="function() {hasChanges(true,false);}" />
                                                                        </dx:ASPxTimeEdit>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </Content>
                                                    </roUserControls:roGroupBox>
                                                </div>

                                                <div class="splitDivRight">
                                                    <roUserControls:roGroupBox ID="GroupBox3" runat="server">
                                                        <Content>
                                                            <div style="min-height: 90px">
                                                                <table border="0" style="width: 100%;">
                                                                    <tr>
                                                                        <td colspan="7" style="height: 40px;">
                                                                            <asp:Label ID="lblWeekDays" runat="server" Text="En los días de la semana siguientes:"></asp:Label>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td style="width: 75px; padding-left: 30px;">
                                                                            <input type="checkbox" runat="server" id="chkWeekDay1" onchange="hasChanges(true,false);" /><asp:Label ID="lblWeekDay1" runat="server" Text="Lunes"></asp:Label></td>
                                                                        <td style="width: 75px; padding-left: 10px;">
                                                                            <input type="checkbox" runat="server" id="chkWeekDay2" onchange="hasChanges(true,false);" /><asp:Label ID="lblWeekDay2" runat="server" Text="Martes"></asp:Label></td>
                                                                        <td style="width: 75px; padding-left: 10px;">
                                                                            <input type="checkbox" runat="server" id="chkWeekDay3" onchange="hasChanges(true,false);" /><asp:Label ID="lblWeekDay3" runat="server" Text="Miércoles"></asp:Label></td>
                                                                        <td style="width: 75px; padding-left: 10px;">
                                                                            <input type="checkbox" runat="server" id="chkWeekDay4" onchange="hasChanges(true,false);" /><asp:Label ID="lblWeekDay4" runat="server" Text="Jueves"></asp:Label></td>
                                                                        <td style="width: 75px; padding-left: 10px;">
                                                                            <input type="checkbox" runat="server" id="chkWeekDay5" onchange="hasChanges(true,false);" /><asp:Label ID="lblWeekDay5" runat="server" Text="Viernes"></asp:Label></td>
                                                                        <td style="width: 75px; padding-left: 10px;">
                                                                            <input type="checkbox" runat="server" id="chkWeekDay6" onchange="hasChanges(true,false);" /><asp:Label ID="lblWeekDay6" runat="server" Text="Sábado"></asp:Label></td>
                                                                        <td style="width: 75px; padding-left: 10px;">
                                                                            <input type="checkbox" runat="server" id="chkWeekDay7" onchange="hasChanges(true,false);" /><asp:Label ID="lblWeekDay7" runat="server" Text="Domingo"></asp:Label></td>
                                                                    </tr>
                                                                </table>
                                                            </div>
                                                        </Content>
                                                    </roUserControls:roGroupBox>
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

        function resizeTreeDiningRoom() {
            try {
                var ctlPrefix = "<%= roTreesDiningRoom.ClientID %>";
                eval(ctlPrefix + "_resizeTrees();");
            }
            catch (e) {
                showError("resizeTreeDiningRoom", e);
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

        //PPR desactivado temporalmente NO ELIMINAR-->
        //    function loadInitialPageValues() {
        //        var oQueryStringState = new roQueryStringState("DiningRoom");

        //        if (oQueryStringState.ActiveTab != "")
        //            actualTab = oQueryStringState.ActiveTab;
        //
        //        if (oQueryStringState.HasReg == "0")
        //            cargaDiningRoom('-1');
        //
        //        oQueryStringState.clear();
        //    }

        window.onresize = function () {
            resizeFrames();
            resizeTreeDiningRoom();
        }

        if (document.getElementById('<%= noRegs.ClientID %>').value != '') {
            cargaDiningRoom('-1');
        }
    </script>
</asp:Content>