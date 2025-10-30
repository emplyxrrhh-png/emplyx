<%@ Page Language="VB" MasterPageFile="~/Base/mastEmp.master" AutoEventWireup="false" Inherits="VTLive40.Assignments_Assignments" Title="Puestos" CodeBehind="Assignments.aspx.vb" %>

<asp:Content ID="cMainBody" ContentPlaceHolderID="contentMainBody" runat="server">

    <script language="javascript" type="text/javascript">

        function PageBase_Load() {
            resizeFrames();
            resizeTreeAssignments();
        }
    </script>

    <input type="hidden" runat="server" id="noRegs" value="" />

    <input type="hidden" id="msgHasChanges" value="" runat="server" clientidmode="Static" />
    <input type="hidden" id="msgHasErrors" value="" runat="server" clientidmode="Static" />

    <div id="divMainBody">
        <!-- TAB SUPERIOR -->
        <div id="divTabInfo" class="divDataCells">
            <div style="min-height: 10px"></div>
            <div id="divAssignments" class="blackRibbonTitle">
            </div>
            <div style="min-height: 10px"></div>
        </div>
        <!-- FIN TAB SUPERIOR -->

        <!-- ARBOL Y DETALLE -->
        <div id="divTabData" class="divDataCells">

            <div id="divTree" class="treeSize">
                <div id="ctlTreeDiv" style="height: 500px;">
                    <rws:roTreesSelector ID="roTreesAssignments" runat="server" ShowEmployeeFilters="false" PrefixTree="roTreesAssignments"
                        Tree1Visible="true" Tree1MultiSel="false" Tree1ShowOnlyGroups="false" Tree1Function="cargaNodo" Tree1ImagePath="images/AssignmentsSelector" Tree1SelectorPage="../../Assignments/AssignmentsSelectorData.aspx"
                        ShowTreeCaption="true"></rws:roTreesSelector>
                </div>
            </div>

            <div id="divButtons" class="divMiddleButtons">
                <div id="divBarButtons" class="centerTop">&nbsp</div>
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
                                        <div class="panHeader2">
                                            <span style="">
                                                <asp:Label runat="server" ID="lblAssignmentsTitleGeneral" Text="General"></asp:Label></span>
                                        </div>

                                        <table border="0" style="padding-top: 5px;">
                                            <tr>
                                                <td align="left" valign="top" style="width: 50px; padding-left: 25px; padding-right: 5px;">
                                                    <img src="Images/Assignments80.png" alt="" /></td>
                                                <td valign="top" style="width: 100%; padding: 5px; padding-right: 30px;">
                                                    <asp:Label ID="lblAssignmentDesc" runat="server" Text="Datos especificos para puestos." CssClass="spanEmp-Class"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-top: 5px;" colspan="2" align="center">
                                                    <table border="0" cellpadding="0" cellspacing="0" style="height: 90%; width: 90%;">
                                                        <tr>
                                                            <td style="padding-left: 10px;" valign="top">
                                                                <table border="0" width="100%" style="height: 100%;">
                                                                    <tr>
                                                                        <td valign="top" style="height: 20px;">
                                                                            <table width="100%">
                                                                                <tr>
                                                                                    <td style="width: 150px; padding-right: 5px;" align="right" valign="top">
                                                                                        <asp:Label ID="lblName" runat="server" Text="Nombre:" class="spanEmp-Class"></asp:Label></td>
                                                                                    <td>
                                                                                        <dx:ASPxTextBox ID="txtName" runat="server" ClientInstanceName="txtName_Client" NullText="_____">
                                                                                            <ClientSideEvents Validation="LengthValidation" TextChanged="function(s,e){checkAssignmentEmptyName(s.GetValue());}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                                            <ValidationSettings SetFocusOnError="True">
                                                                                                <RequiredField IsRequired="True" ErrorText="(*)" />
                                                                                            </ValidationSettings>
                                                                                        </dx:ASPxTextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td style="width: 150px; padding-right: 5px;" align="right" valign="top">
                                                                                        <asp:Label ID="lblDescAssignment" runat="server" Text="Descripción:" class="spanEmp-Class"></asp:Label></td>
                                                                                    <td>
                                                                                        <dx:ASPxMemo ID="txtDescription" runat="server" Rows="5" Width="100%">
                                                                                            <ClientSideEvents TextChanged="function() {hasChanges(true,false);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                                        </dx:ASPxMemo>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right" style="width: 150px; padding-right: 5px;">
                                                                                        <asp:Label ID="lblShortName" runat="server" Text="Nombre abreviado:" class="spanEmp-Class"></asp:Label></td>
                                                                                    <td style="text-align: left;">
                                                                                        <dx:ASPxTextBox ID="txtShortName" Width="50" runat="server" MaxLength="2" NullText="__">
                                                                                            <ClientSideEvents Validation="LengthValidation" TextChanged="function(s,e){hasChanges(true,false);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                                            <ValidationSettings SetFocusOnError="True">
                                                                                                <RequiredField IsRequired="True" ErrorText="(*)" />
                                                                                            </ValidationSettings>
                                                                                        </dx:ASPxTextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right" style="width: 150px; padding-right: 5px;">
                                                                                        <asp:Label ID="lblColorDesc" runat="server" Text="Color identificativo:" class="spanEmp-Class"></asp:Label></td>
                                                                                    <td>
                                                                                        <dx:ASPxColorEdit ID="dxColorPicker" runat="server" ClientInstanceName="dxColorPickerClient" EnableCustomColors="true" Width="14px">
                                                                                            <ClientSideEvents ColorChanged="function(s,e) {s.GetInputElement().style.display = 'none';hasChanges(true,false);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                                        </dx:ASPxColorEdit>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right" style="width: 150px; padding-right: 5px;">
                                                                                        <asp:Label ID="lblExport" runat="server" Text="Equivalencia:" class="spanEmp-Class"></asp:Label></td>
                                                                                    <td style="text-align: left;">
                                                                                        <dx:ASPxTextBox ID="txtExport" runat="server" MaxLength="5" NullText="__">
                                                                                            <ClientSideEvents Validation="LengthValidation" TextChanged="function(s,e){hasChanges(true,false);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                                            <ValidationSettings SetFocusOnError="True">
                                                                                                <RequiredField IsRequired="True" ErrorText="(*)" />
                                                                                            </ValidationSettings>
                                                                                        </dx:ASPxTextBox>
                                                                                    </td>
                                                                                </tr>

                                                                                <tr>
                                                                                    <td></td>
                                                                                    <td style="text-align: left; padding-top: 10px; padding-bottom: 5px; padding-right: 30px">
                                                                                        <asp:Label ID="lblDescCost" CssClass="OptionPanelDescStyle" runat="server" Text="El coste medio es el valor que se aplica a un puesto en función de un campo de la ficha de la empresa. Dicho valor se utilizará en los informes de planificación para realizar comparativas."></asp:Label></td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="right" style="width: 150px; padding-right: 5px;">
                                                                                        <asp:Label ID="lblCost" runat="server" Text="Coste medio:" class="spanEmp-Class"></asp:Label></td>
                                                                                    <td>
                                                                                        <dx:ASPxComboBox ID="cmbCostField" runat="server" Width="200px" Font-Size="11px" CssClass="editTextFormat"
                                                                                            Font-Names="Arial;Verdana;Sans-Serif" IncrementalFilteringMode="Contains">
                                                                                            <ClientSideEvents SelectedIndexChanged="function() {hasChanges(true,false);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                                        </dx:ASPxComboBox>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                    </table>
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

            <!-- POPUP NEW OBJECT -->
            <dx:ASPxPopupControl ID="NewObjectPopup" runat="server" AllowDragging="False" CloseAction="None" Modal="True" ContentUrl="~/Base/Popups/CreateObjectPopup.aspx"
                PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter" Width="470px" Height="300px"
                ShowHeader="False" ScrollBars="Auto" ShowPageScrollbarWhenModal="True" ClientInstanceName="NewObjectPopup_Client" PopupAnimationType="None" BackColor="Transparent" ContentStyle-Paddings-Padding="0px" Border-BorderColor="Transparent" ShowShadow="false">
                <SettingsLoadingPanel Enabled="false" />
            </dx:ASPxPopupControl>
        </div>
    </div>

    <script language="javascript" type="text/javascript">

        function resizeTreeAssignments() {
            try {
                var ctlPrefix = "<%= roTreesAssignments.ClientID %>";
                eval(ctlPrefix + "_resizeTrees();");
            }
            catch (e) {
                showError("resizeTreeAssignments", e);
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
            resizeTreeAssignments();
        }

        if (document.getElementById('<%= noRegs.ClientID %>').value != '') {
            cargaAssignment('-1');
        }
    </script>
</asp:Content>