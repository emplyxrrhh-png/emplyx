<%@ Page Language="VB" MasterPageFile="~/Base/mastEmp.master" AutoEventWireup="false" Inherits="VTLive40.Cameras" Title="${Cameras}" CodeBehind="Cameras.aspx.vb" %>

<asp:Content ID="cMainBody" ContentPlaceHolderID="contentMainBody" runat="Server">

    <script language="javascript" type="text/javascript">

        function PageBase_Load() {
            resizeFrames();
            resizeTreeCameras();

            if (document.getElementById('<%= noRegs.ClientID %>').value != '') {
                cargaCamera('-1');
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
            <div id="divCamera" class="blackRibbonTitle">
            </div>
            <div style="min-height: 10px"></div>
        </div>
        <!-- FIN TAB SUPERIOR -->

        <!-- ARBOL Y DETALLE -->
        <div id="divTabData" class="divDataCells">

            <div id="divTree" class="treeSize">
                <div id="ctlTreeDiv" style="height: 500px;">
                    <rws:roTreesSelector ID="roTreesCameras" runat="server" ShowEmployeeFilters="false" PrefixTree="roTreesCameras"
                        Tree1Visible="true" Tree1MultiSel="false" Tree1ShowOnlyGroups="false" Tree1Function="cargaNodo" Tree1ImagePath="images/CameraSelector" Tree1SelectorPage="../../Cameras/CameraSelectorData.aspx"
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
                                                    <asp:Label runat="server" ID="lblCamerasGeneral" Text="General"></asp:Label>
                                                </span>
                                            </div>
                                            <!-- La descripción es opcional -->
                                            <div class="panelHeaderContent">
                                                <div class="panelDescriptionImage">
                                                    <img alt="" src="<%=Me.Page.ResolveUrl("~/Cameras/Images/Cameras.png")%>" />
                                                </div>
                                                <div class="panelDescriptionText">
                                                    <asp:Label ID="lblCamerasDesc" runat="server" Text="Datos especificos de la cámara."></asp:Label>
                                                </div>
                                            </div>
                                        </div>

                                        <!-- Este div es un formulario -->
                                        <div class="panBottomMargin">
                                            <div class="divRow">
                                                <div class="divRowDescription">
                                                    <asp:Label ID="lblNameDescription" runat="server" Text="Nombre identificativo de la cámara"></asp:Label>
                                                </div>
                                                <asp:Label ID="lblName" runat="server" Text="Nombre:" CssClass="labelForm"></asp:Label>
                                                <div class="componentForm">
                                                    <dx:ASPxTextBox ID="txtName" runat="server" ClientInstanceName="txtName_Client" NullText="_____">
                                                        <ClientSideEvents Validation="LengthValidation" TextChanged="function(s,e){checkCameraEmptyName(s.GetValue());}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                        <ValidationSettings SetFocusOnError="True">
                                                            <RequiredField IsRequired="True" ErrorText="(*)" />
                                                        </ValidationSettings>
                                                    </dx:ASPxTextBox>
                                                </div>
                                            </div>

                                            <div class="divRow">
                                                <div class="divRowDescription">
                                                    <asp:Label ID="lblDescCameraDescription" runat="server" Text="Descripción de la cámara"></asp:Label>
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
                                                    <asp:Label ID="lblUrlDescription" runat="server" Text="Url donde se puede consultar el contenido de la cámara"></asp:Label>
                                                </div>
                                                <asp:Label ID="lblURLCamera" runat="server" Text="URL:" CssClass="labelForm"></asp:Label>
                                                <div class="componentForm" style="width: 400px">
                                                    <dx:ASPxTextBox ID="txtURL" runat="server" Width="400px" ClientInstanceName="txtURL_Client" NullText="_____">
                                                        <ClientSideEvents Validation="LengthValidation" TextChanged="function(s,e){hasChanges(true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                        <ValidationSettings ErrorDisplayMode="None">
                                                            <RequiredField IsRequired="True" ErrorText="" />
                                                        </ValidationSettings>
                                                    </dx:ASPxTextBox>
                                                </div>
                                                <div class="componentForm" style="width: 20px">
                                                    <a href="javascript: void(0)" onclick="viewCam();" class="btnViewCam"></a>
                                                </div>
                                            </div>

                                            <div class="divRow">
                                                <div class="divRowDescription">
                                                    <asp:Label ID="lblModelDescription" runat="server" Text="Modelo de la cámara"></asp:Label>
                                                </div>
                                                <asp:Label ID="lblModel" runat="server" Text="Porcentaje de:" CssClass="labelForm"></asp:Label>
                                                <div class="componentForm">
                                                    <dx:ASPxComboBox runat="server" ID="cmbModel" Width="250px">
                                                        <ClientSideEvents SelectedIndexChanged="function(s,e){hasChanges(true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                        <ValidationSettings>
                                                            <RequiredField IsRequired="False" />
                                                        </ValidationSettings>
                                                    </dx:ASPxComboBox>
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

        function resizeTreeCameras() {
            try {
                var ctlPrefix = "<%= roTreesCameras.ClientID %>";
                eval(ctlPrefix + "_resizeTrees();");
            }
            catch (e) {
                showError("resizeTreeCameras", e);
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
        //        var oQueryStringState = new roQueryStringState("Cameras");

        //        if (oQueryStringState.ActiveTab != "")
        //            actualTab = oQueryStringState.ActiveTab;
        //
        //        if (oQueryStringState.HasReg == "0")
        //            cargaCamera('-1');
        //
        //        oQueryStringState.clear();
        //    }

        window.onresize = function () {
            resizeFrames();
            resizeTreeCameras();
        }
    </script>
</asp:Content>