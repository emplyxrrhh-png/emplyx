<%@ Page Language="VB" MasterPageFile="~/Base/mastEmp.master" AutoEventWireup="false" Inherits="VTLive40.Concepts" Title="${Concepts}" EnableEventValidation="false" CodeBehind="Concepts.aspx.vb" %>

<%@ Register Src="~/Concepts/WebUserForms/frmCompositions.ascx" TagName="frmCompositions" TagPrefix="roForms" %>

<asp:Content ID="cMainBody" ContentPlaceHolderID="contentMainBody" runat="Server">

    <script language="javascript" type="text/javascript">

        function PageBase_Load() {
            resizeFrames();
            resizeTreeConcepts();

            venableOPC('<%= optRecalcByDate.ClientID %>');
            venableOPC('<%= optRecalcAllDays.ClientID %>');
            linkOPCItems('<%= optRecalcByDate.ClientID %>,<%= optRecalcAllDays.ClientID %>');
            resizeTreeConcepts();
        }

        /* funcio per bloquejar sols l'area menu */
        function disableScreen(bol) {
            var divBg = document.getElementById('divModalBgDisabled');
            if (divBg != null) {
                if (bol == true) {
                    document.body.style.overflow = "hidden";
                    divBg.style.height = 2000;  //document.body.offsetHeight;
                    divBg.style.width = 3000;  //document.body.offsetWidth;

                    divBg.style.display = '';
                }
                else {
                    document.body.style.overflow = "";
                    divBg.style.display = 'none';
                }
            }
        }

        //Recarrega corresponent al clickar el arbre
        function TabClick(numTab) {
            try {
                showLoadingGrid(true);
                resizeTreeConcepts();
                if (numTab == '1') { //Concepts
                    var ctlPrefix = "ctl00_contentMainBody_roTreesConcepts_roTrees";
                    eval(ctlPrefix + ".LoadTreeViews(true, true, true);");
                } else { //ConceptGroups
                    var ctlPrefixGroups = "ctl00_contentMainBody_roTreesConceptGroups_roTrees";
                    eval(ctlPrefixGroups + ".LoadTreeViews(true, true, true);");
                }
            } catch (e) { showError("TabClick", e); }
        }
    </script>

    <input type="hidden" runat="server" id="hdnModeEdit" value="" />
    <input type="hidden" runat="server" id="noRegs" value="" />
    <div id="divModalBgDisabled" class="modalBackground" style="position: absolute; left: 0px; top: 0px; z-index: 990; width: 1680px; height: 900px; display: none;"></div>
    <input type="hidden" id="OperationPlus" value="<%=  Me.Language.Translate("Compositions.OperationsPlus", DefaultScope) %>" />
    <input type="hidden" id="OperationMinus" value="<%=  Me.Language.Translate("Compositions.OperationMinus",DefaultScope) %>" />
    <input type="hidden" id="dateFormatValue" runat="server" value="" />
    <input type="hidden" id="msgHasChanges" value="" runat="server" clientidmode="Static" />
    <input type="hidden" id="msgHasErrors" value="" runat="server" clientidmode="Static" />
    <input type="hidden" id="tagEditTitle" value="<%= Me.Language.Translate("tagEditTitle",DefaultScope) %>" />
    <input type="hidden" id="tagRemoveTitle" value="<%= Me.Language.Translate("tagRemoveTitle",DefaultScope) %>" />

    <div id="divMainBody">
        <!-- TAB SUPERIOR -->
        <div id="divTabInfo" class="divDataCells">
            <div style="min-height: 10px"></div>
            <div id="divConcepts" class="blackRibbonTitle">
            </div>
            <div style="min-height: 10px"></div>
        </div>
        <!-- FIN TAB SUPERIOR -->

        <!-- ARBOL Y DETALLE -->
        <div id="divTabData" class="divDataCells">

            <div id="divTree" class="treeSize">
                <div id="ctlTreeDiv" style="height: 500px;">
                    <div id="dvTreeConcepts" style="width: 100%">
                        <rws:roTreesSelector ID="roTreesConcepts" runat="server" ShowEmployeeFilters="false" PrefixTree="roTreesConcepts"
                            Tree1Visible="true" Tree1MultiSel="false" Tree1ShowOnlyGroups="false" Tree1Function="cargaConceptNodo" Tree1ImagePath="images/ConceptSelector" Tree1SelectorPage="../../Concepts/ConceptSelectorData.aspx"
                            ShowTreeCaption="true"
                            TreeIsDouble="true" TreeDoubleClickFuncion="ShowTreesConceptGroups();"></rws:roTreesSelector>
                    </div>
                    <div id="dvTreeConceptGroups" style="display: none; width: 100%">
                        <rws:roTreesSelector ID="roTreesConceptGroups" runat="server" ShowEmployeeFilters="false" PrefixTree="roTreesConceptGroups"
                            Tree1Visible="true" Tree1MultiSel="false" Tree1ShowOnlyGroups="false" Tree1Function="cargaConceptGroupNodo" Tree1ImagePath="images/ConceptGroupsSelector" Tree1SelectorPage="../../Concepts/ConceptGroupsSelectorData.aspx"
                            ShowTreeCaption="true"
                            TreeIsDouble="true" TreeDoubleClickFuncion="ShowTreesConcepts();"></rws:roTreesSelector>
                    </div>
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

                                <dx:ASPxHiddenField ID="recalcConfig" runat="server" ClientInstanceName="recalcConfigClient"></dx:ASPxHiddenField>

                                <!-- Div flotant canvis a composició -->
                                <div id="divMsgChg" style="position: absolute; z-index: 995; display: none; top: 50%; left: 50%; margin-left: -238px; margin-top: -182px;">
                                    <div id="divBgMsgS" style="position: absolute; top: 0; left: 0; display: none; z-index: 991;"></div>
                                    <div class="bodyPopupExtended" style="">
                                        <table width="450px" height="300px" border="0">
                                            <tr>
                                                <td height="32px" width="50px" align="center">
                                                    <img id="Img3" src="~/Base/Images/StartMenuIcos/Concepts.png" runat="server" /></td>
                                                <td height="32px" valign="middle" style="padding-left: 5px;">
                                                    <asp:Label ID="lblTitleRecalculateConcept" runat="server" Text="Recalcular el concepto" Font-Size="Large" Font-Bold="true"></asp:Label></td>
                                            </tr>
                                            <tr>
                                                <td colspan="2" valign="top">
                                                    <asp:Label ID="lblRecalcDesc" CssClass="spanEmp-Class" runat="server" Text="Ha realizado cambios en la composición del concepto actual. El concepto se debe recalcular."></asp:Label>
                                                    <asp:Label ID="lblRecalcDesc2" CssClass="spanEmp-Class" runat="server" Text="Elija el modo de recálculo del concepto."></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                    <table>
                                                        <tr>
                                                            <td>
                                                                <roUserControls:roOptionPanelClient ID="optRecalcByDate" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Checked="False" Enabled="True" Border="True">
                                                                    <Title>
                                                                        <asp:Label ID="lblRecalcByDateTitle" runat="server" Text="Recalcular a partir de una fecha especificada."></asp:Label>
                                                                    </Title>
                                                                    <Description>
                                                                        <asp:Label ID="lblRecalcByDateDesc" runat="server" Text="Se hará una copia del concepto y se recalculará el nuevo concepto desde la fecha especificada."></asp:Label>
                                                                    </Description>
                                                                    <Content>
                                                                        <table width="100%">
                                                                            <tr>
                                                                                <td align="right" width="70%" style="padding-right: 10px;">
                                                                                    <asp:Label ID="lblRecDateDesc" runat="server" CssClass="spanEmp-Class" Text="Recalcular a partir del "></asp:Label></td>
                                                                                <td>
                                                                                    <dx:ASPxDateEdit ID="dtRecDate" Width="105" runat="server" AllowNull="false" ClientInstanceName="dtRecDateClient">
                                                                                        <ClientSideEvents DateChanged="function(s,e){hasChanges(true);}" />
                                                                                    </dx:ASPxDateEdit>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </Content>
                                                                </roUserControls:roOptionPanelClient>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <roUserControls:roOptionPanelClient ID="optRecalcAllDays" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Checked="False" Enabled="True" Border="True">
                                                                    <Title>
                                                                        <asp:Label ID="lblRecalcAllDaysTitle" runat="server" Text="Recalcular todos los días"></asp:Label>
                                                                    </Title>
                                                                    <Description>
                                                                        <asp:Label ID="lblRecalcAllDaysDesc" runat="server" Text="Se recalculará el concepto para todos los días donde el concepto sea válido."></asp:Label>
                                                                    </Description>
                                                                    <Content>
                                                                    </Content>
                                                                </roUserControls:roOptionPanelClient>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2" align="right">
                                                    <table border="0" style="width: 100%;">
                                                        <tr>
                                                            <td>&nbsp;</td>
                                                            <td style="width: 110px;" align="right">
                                                                <dx:ASPxButton ID="btnSaveOk" runat="server" AutoPostBack="False" CausesValidation="False" Text="Aceptar" ToolTip="Aceptar" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                                                    <Image Url="~/Base/Images/Grid/button_ok.png"></Image>
                                                                    <ClientSideEvents Click="function(s,e){ recalcComposition(); }" />
                                                                </dx:ASPxButton>
                                                            </td>
                                                            <td style="width: 110px;" align="left">
                                                                <dx:ASPxButton ID="btnSaveCancel" runat="server" AutoPostBack="False" CausesValidation="False" Text="Cancelar" ToolTip="Cancelar" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                                                    <Image Url="~/Base/Images/Grid/button_cancel.png"></Image>
                                                                    <ClientSideEvents Click="function(s,e){ closeWndRecalConcept(); }" />
                                                                </dx:ASPxButton>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                </div>
                                <!-- End Div flotant canvis a composició -->

                                <!-- Div flotant ConceptGroups -->
                                <input type="hidden" id="hdnConceptGroupsChanges" value="0" />
                                <div id="divConceptGroupsItems" style="position: fixed; *position: absolute; z-index: 15010; display: none; top: 50%; left: 50%; *width: 900px;">
                                    <div id="divBgS" style="position: absolute; top: 0; left: 0; display: none; z-index: 15009;"></div>
                                    <div class="bodyPopupExtended" style="">
                                        <!-- Controls Popup Aqui -->
                                        <div id="div1" runat="server" style="">
                                            <div style="width: 100%; height: 100%;" class="bodyPopup defaultContrastColor">
                                                <!-- Tags idioma per passar a js -->
                                                <input type="hidden" id="header1" runat="server" clientidmode="static" />
                                                <input type="hidden" id="msgErrorNoConditions" runat="server" clientidmode="static" />
                                                <table style="width: 100%; padding-top: 5px;" border="0">
                                                    <tr>
                                                        <td colspan="2">
                                                            <div class="panHeader2">
                                                                <span style="">
                                                                    <asp:Label runat="server" ID="lblCGTit" Text="Acumulado"></asp:Label></span>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="2" style="padding: 10px;"></td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="2" style="padding: 10px;">
                                                            <table border="0" style="width: 100%;">
                                                                <tr>
                                                                    <td style="padding-left: 10px; width: 15%;">
                                                                        <asp:Label ID="lblcmbConcept" runat="server" Text="Acumulado"></asp:Label></td>
                                                                    <td>
                                                                        <dx:ASPxComboBox ID="cmbConcepts" runat="server" ClientInstanceName="cmbConceptsClient">
                                                                        </dx:ASPxComboBox>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td colspan="2" style="padding-top: 10px; padding-right: 20px; text-align: right;"></td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <input type="hidden" id="hdnMustRefresh_PageBase" value="0" runat="server" />
                                                <table border="0" style="width: 100%;">
                                                    <tr>
                                                        <td>&nbsp;</td>

                                                        <td style="width: 110px;" align="right">
                                                            <dx:ASPxButton ID="btnOk" runat="server" AutoPostBack="False" CausesValidation="False" Text="Aceptar" ToolTip="Aceptar" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                                                <ClientSideEvents Click="function(s,e){ saveConceptGroupsItem(); }" />
                                                            </dx:ASPxButton>
                                                        </td>
                                                        <td style="width: 110px;" align="left">
                                                            <dx:ASPxButton ID="btnCancel" runat="server" AutoPostBack="False" CausesValidation="False" Text="Cancelar" ToolTip="Cancelar" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                                                <ClientSideEvents Click="function(s,e){ cancelConceptGroupsItem(); }" />
                                                            </dx:ASPxButton>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <!-- End Div flotant ConceptGroups -->

                                <div id="divContentPanels" class="divContentPanelsWithOutMessage">

                                    <div id="ConceptsContent" runat="server" class="contentPanel" style="display: none">
                                        <!-- Panell General -->
                                        <div id="div00" class="contentPanel" style="display: none" runat="server" name="menuPanel">
                                            <div class="panHeader2">
                                                <span style="">
                                                    <asp:Label runat="server" ID="lblGeneralTitle" Text="General"></asp:Label></span>
                                            </div>
                                            <br />
                                            <table width="100%" border="0" style="padding-top: 5px">
                                                <tr>
                                                    <td width="150px" align="right" valign="top" style="padding-right: 5px;">
                                                        <asp:Label ID="lblName" runat="server" Text="Nombre:" class="spanEmp-Class"></asp:Label></td>
                                                    <td valign="top" style="padding-right: 30px;">
                                                        <dx:ASPxTextBox ID="txtConceptName" runat="server" MaxLength="50" Width="200px" ClientInstanceName="txtConceptName_Client" NullText="_____">
                                                            <ClientSideEvents Validation="LengthValidation" TextChanged="function(s,e){checkConceptEmptyName(s.GetValue());}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                            <ValidationSettings SetFocusOnError="True">
                                                                <RequiredField IsRequired="True" ErrorText="(*)" />
                                                            </ValidationSettings>
                                                        </dx:ASPxTextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td width="150px" align="right" valign="top" style="padding-right: 5px;">
                                                        <asp:Label ID="lblDescription" runat="server" Text="Descripción:" class="spanEmp-Class"></asp:Label></td>
                                                    <td valign="top" style="padding-right: 30px;">
                                                        <dx:ASPxMemo ID="txtDescription" runat="server" Rows="5" Width="475px" ClientInstanceName="txtDescription_Client">
                                                            <ClientSideEvents TextChanged="function(s,e){hasChanges(true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                        </dx:ASPxMemo>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td></td>
                                                    <td style="padding-top: 10px; padding-bottom: 5px; padding-right: 30px">
                                                        <asp:Label ID="lblDescShortName" CssClass="SectionDescription" runat="server" Text="El nombre abreviado es el que se usa en el calendario y en algunos listados donde el normal sería demasiado largo."></asp:Label></td>
                                                </tr>
                                                <tr>
                                                    <td width="150px" align="right" style="padding-right: 5px;">
                                                        <asp:Label ID="lblShortName" runat="server" Text="Nombre abreviado:" class="spanEmp-Class"></asp:Label></td>
                                                    <td>
                                                        <dx:ASPxTextBox ID="txtShortName" runat="server" MaxLength="3" Width="50" NullText="___" ClientInstanceName="txtShortName_Client">
                                                            <ClientSideEvents Validation="LengthValidation" TextChanged="function(s,e){ hasChanges(true)}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                            <ValidationSettings SetFocusOnError="True">
                                                                <RequiredField IsRequired="True" ErrorText="(*)" />
                                                            </ValidationSettings>
                                                        </dx:ASPxTextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td></td>
                                                    <td style="padding-top: 10px; padding-bottom: 5px; padding-right: 30px">
                                                        <asp:Label ID="lblDescColor" CssClass="SectionDescription" runat="server" Text="El color se usa en algunos listados para identificar fácilmente el saldo sin necesidad de leer."></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td width="150px" align="right" style="padding-right: 5px;">
                                                        <asp:Label ID="lblColorDesc" runat="server" Text="Color identificativo:" class="spanEmp-Class"></asp:Label></td>
                                                    <td>
                                                        <dx:ASPxColorEdit ID="ColorConcept" runat="server" EnableCustomColors="true" Width="14px">
                                                            <ClientSideEvents ColorChanged="function(s,e){s.GetInputElement().style.display = 'none';hasChanges(true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                        </dx:ASPxColorEdit>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td></td>
                                                    <td style="padding-top: 10px; padding-bottom: 5px; padding-right: 30px">
                                                        <asp:Label ID="lblDescFactor" CssClass="SectionDescription" runat="server" Text="El factor de exportación es el número con el que se identifica este acumulado en los procesos de exportación -p.e. en la exportación a nómina-."></asp:Label></td>
                                                </tr>
                                                <tr>
                                                    <td width="150px" align="right" style="padding-right: 5px;">
                                                        <asp:Label ID="lblFactor" class="spanEmp-Class" runat="server" Text="Factor de exportación:"></asp:Label></td>
                                                    <td>
                                                        <dx:ASPxTextBox ID="txtFactor" runat="server">
                                                            <ClientSideEvents TextChanged="function(s,e){hasChanges(true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                            <MaskSettings IncludeLiterals="None" />
                                                        </dx:ASPxTextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                            <br />
                                            <div class="panHeader2">
                                                <span style="">
                                                    <asp:Label runat="server" ID="lblPeriodicityTitle" Text="Periodicidad"></asp:Label></span>
                                            </div>
                                            <input type="hidden" id="hdnConceptPeriodType" value="0" runat="server" />
                                            <table width="100%" border="0" style="padding-top: 5px">
                                                <tr>
                                                    <td style="padding-top: 10px; padding-left: 30px; padding-right: 30px">
                                                        <asp:Label ID="lblQueryDesc" runat="server" CssClass="SectionDescription" Text="En consultas realizadas desde un terminal ó Live Portal, se mostrará el valor de este ${Concept} para:"></asp:Label></td>
                                                </tr>
                                                <tr>
                                                    <td style="padding-left: 60px; padding-top: 5px; padding-bottom: 4px;">
                                                        <table>
                                                            <tr>
                                                                <td align="right">
                                                                    <asp:Label ID="lblShowValue" CssClass="editTextFormat" runat="server" Text="Mostrar el valor del:"></asp:Label>&nbsp;</td>
                                                                <td>
                                                                    <dx:ASPxComboBox runat="server" ID="cmbShowValue" Width="200px" ClientInstanceName="cmbShowValueClient">
                                                                        <ClientSideEvents SelectedIndexChanged="function(s,e){accrualTypeChanged(s.GetSelectedItem().value);hasChanges(true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                    </dx:ASPxComboBox>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="padding-top: 5px; padding-left: 30px; padding-right: 30px">
                                                        <asp:Label ID="lblQueryAux" runat="server" CssClass="SectionDescription" Text="En el caso de terminales sólo si técnicamente lo permiten."></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td id="divAccrualExpiration" style="display: none; padding-top: 5px; padding-left: 30px; padding-right: 30px">
                                                        <roUserControls:roOptionPanelClient ID="optAccrualExpiration" runat="server" TypeOPanel="CheckboxOption" Width="100%" Height="Auto" Checked="False" Enabled="True" style="width: 100%;" CConClick="accrualExpirationHasChanges();hasChanges(true);">
                                                            <Title>
                                                                <asp:Label ID="lblAccrualExpirationTitle" runat="server" Text="Configurar la caducidad del saldo"></asp:Label>
                                                            </Title>
                                                            <Description></Description>
                                                            <Content>
                                                                <div id="divAccrualExpirationContent" style="display: none">
                                                                    <div class="divRow">
                                                                        <div class="labelFloat">
                                                                            <asp:Label ID="lblAccrualExpiration1" runat="server" Text="Los valores ingresados en este saldo tienen una caducidad de "></asp:Label>
                                                                        </div>
                                                                        <div class="componentFloat" style="margin-top: -7px;">
                                                                            <dx:ASPxTextBox ID="txtExpirationPeriodValue" runat="server" Width="75px" ClientInstanceName="txtExpirationPeriodValueClient">
                                                                                <ClientSideEvents TextChanged="function(s,e){accrualExpirationHasChanges();hasChanges(true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                                <ValidationSettings ErrorDisplayMode="None" />
                                                                                <MaskSettings Mask="<0..999>" />
                                                                            </dx:ASPxTextBox>
                                                                        </div>
                                                                        <div class="componentFloat">
                                                                            <dx:ASPxComboBox runat="server" ID="cmbExpirationPeriodType" Width="175px" ClientInstanceName="cmbExpirationPeriodTypeClient">
                                                                                <ClientSideEvents SelectedIndexChanged="function(s,e){accrualExpirationHasChanges();hasChanges(true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                                <ValidationSettings ErrorDisplayMode="None" />
                                                                            </dx:ASPxComboBox>
                                                                        </div>

                                                                        <div class="labelFloat">
                                                                            <asp:Label ID="lblAccrualExpiration2" runat="server" Text=", al caducar se restan del saldo y se justifican como"></asp:Label>
                                                                        </div>
                                                                        <div class="componentFloat">
                                                                            <dx:ASPxComboBox runat="server" ID="cmbExpirationPeriodCause" ClientInstanceName="cmbExpirationPeriodCauseClient" Width="175px">
                                                                                <ClientSideEvents SelectedIndexChanged="function(s,e){accrualExpirationHasChanges();hasChanges(true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                                <ValidationSettings ErrorDisplayMode="None" />
                                                                            </dx:ASPxComboBox>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </Content>
                                                        </roUserControls:roOptionPanelClient>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>

                                        <!-- Panell Contenido -->
                                        <div id="div2" class="contentPanel" style="display: none" runat="server" name="menuPanel">
                                            <div class="panHeader2">
                                                <span style="">
                                                    <asp:Label runat="server" ID="lblTypeTitle" Text="Tipo"></asp:Label></span>
                                            </div>
                                            <br />
                                            <table width="100%" border="0" style="padding-top: 5px">
                                                <tr>
                                                    <td style="padding-top: 20px; padding-left: 30px; padding-right: 20px">
                                                        <asp:Label ID="lblTypeDesc" CssClass="SectionDescription" runat="server" Text="Aquí, debe seleccionar el tipo de acumulado que va a realizar. El acumulado puede sumar tiempos o el número de veces que ocurren una o más justificaciones."></asp:Label></td>
                                                </tr>
                                                <tr>
                                                    <td style="padding: 30px; padding-bottom: 10px;">
                                                        <input type="hidden" id="oldConceptType" runat="server" />
                                                        <input type="hidden" id="RecalcParamType" value="0" runat="server" />
                                                        <roUserControls:roOptionPanelClient ID="opConceptTime" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Checked="False" Enabled="True" CConClick="conceptTypeHasChanges();">
                                                            <Title>
                                                                <asp:Label ID="lblConceptTime" runat="server" Text="El acumulado acumula el tiempo."></asp:Label>
                                                            </Title>
                                                            <Description>
                                                                <asp:Label ID="lblConceptTimeDesc" runat="server" Text="El acumulado suma la cantidad de tiempo de cada justificación que compone el acumulado."></asp:Label>
                                                            </Description>
                                                            <Content>
                                                            </Content>
                                                        </roUserControls:roOptionPanelClient>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="padding: 30px; padding-top: 0; padding-bottom: 10px;">
                                                        <roUserControls:roOptionPanelClient ID="opConceptTimes" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Checked="False" Enabled="True" CConClick="if(conceptTypeHasChanges()){hasChanges(true);}">
                                                            <Title>
                                                                <asp:Label ID="lblConceptTimes" runat="server" Text="El acumulado acumula el número de veces."></asp:Label>
                                                            </Title>
                                                            <Description>
                                                                <asp:Label ID="lblConceptTimesDesc" runat="server" Text="El acumulado suma el número de veces que ocurren las justificaciones que componen el acumulado."></asp:Label>
                                                            </Description>
                                                            <Content>
                                                            </Content>
                                                        </roUserControls:roOptionPanelClient>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="padding: 30px; padding-top: 0; padding-bottom: 10px;">
                                                        <roUserControls:roOptionPanelClient ID="opConceptCustom" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Checked="False" Enabled="True" CConClick="if(conceptTypeHasChanges()){hasChanges(true);}">
                                                            <Title>
                                                                <asp:Label ID="lblConceptCustom" runat="server" Text="El saldo es personalizado."></asp:Label>
                                                            </Title>
                                                            <Description>
                                                                <asp:Label ID="lblConceptCustomDesc" runat="server" Text="El saldo contabiliza cualquier otro cosa que no sean ni horas ni días, por ejemplo: Pluses, tickets, etc..."></asp:Label>
                                                            </Description>
                                                            <Content>
                                                            </Content>
                                                        </roUserControls:roOptionPanelClient>
                                                    </td>
                                                </tr>
                                            </table>
                                            <div class="panHeader2">
                                                <span style="">
                                                    <asp:Label runat="server" ID="lblRequest" Text="Solicitudes"></asp:Label></span>
                                            </div>
                                            <br />
                                            <table width="100%" border="0" style="padding-top: 5px">
                                                <tr>
                                                    <td style="padding-top: 20px; padding-left: 30px; padding-right: 20px">
                                                        <asp:Label ID="lblRequestDesc" CssClass="SectionDescription" runat="server" Text="Indique si en el momento de configurar un horario o una justificación de vacaciones se mostrará este saldo en la lista de disponibles para realizar su control."></asp:Label></td>
                                                </tr>
                                                <tr>
                                                    <td style="padding: 30px; padding-top: 0;">
                                                        <roUserControls:roOptionPanelClient ID="chkRequestHolidays" runat="server" TypeOPanel="CheckboxOption" Width="100%" Height="Auto" Checked="False" Enabled="True" CConClick="hasChanges(true);">
                                                            <Title>
                                                                <asp:Label ID="lblRequestHolidays" runat="server" Text="Este saldo se utiliza para la gestión de solicitudes de vacaciones/permisos."></asp:Label>
                                                            </Title>
                                                            <Description>
                                                            </Description>
                                                            <Content>
                                                            </Content>
                                                        </roUserControls:roOptionPanelClient>
                                                    </td>
                                                </tr>
                                            </table>
                                            <div id="divDailyRecord" style="display: none" runat="server">
                                                <div class="panHeader2">
                                                    <span style="">
                                                        <asp:Label runat="server" ID="lblDailyRecordTitle" Text="Declaración de jornada"></asp:Label></span>
                                                </div>
                                                <br />
                                                <table width="100%" border="0" style="padding-top: 5px; width: 100%;">
                                                    <tr>
                                                        <td style="padding-top: 20px; padding-left: 30px; padding-right: 20px">
                                                            <asp:Label ID="lblDailyRecordDesc" CssClass="SectionDescription" runat="server" Text="Indique si este saldo se utilizará para determinar si una declaración de jornada está ajustada o no y el margen de error permitido."></asp:Label></td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding-left: 60px; padding-top: 20px; padding-bottom: 10px; padding-right: 20px;">
                                                            <roUserControls:roOptionPanelClient ID="chkDailyRecord" class="" runat="server" TypeOPanel="CheckboxOption" Width="100%" Height="Auto" Checked="False" Enabled="True" CConClick="hasChanges(true);">
                                                                <Title>
                                                                    <asp:Label Style="float: left; margin-right: 10px" ID="lblDailyRecord" runat="server" Text="Este saldo se utiliza para calcular si una declaración de jornada está ajustada al margen de error."></asp:Label>
                                                                </Title>
                                                                <Description></Description>
                                                                <Content>
                                                                    <table>
                                                                        <tr>
                                                                            <td style="padding-left: 25px; width: 100%; display: flex; align-items: center;">
                                                                                <asp:Label ID="lblMarginMinutesInitial" runat="server" Style="float: left;" CssClass="editTextFormat" Text="Margen de error permitido de "></asp:Label>
                                                                                <dx:ASPxSpinEdit ID="txtDailyRecordMargin" Style="float: left; margin-right: 10px; margin-left: 10px;" runat="server" Number="1" MinValue="0" Width="60px">
                                                                                    <ClientSideEvents ValueChanged="function(s,e){ hasChanges(true); }" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                                    <SpinButtons ShowIncrementButtons="False" ShowLargeIncrementButtons="False" />
                                                                                </dx:ASPxSpinEdit>
                                                                                <asp:Label ID="lblMarginMinutes" runat="server" Text="minutos"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td style="padding-top: 15px; padding-left: 25px;">
                                                                                <input type="checkbox" onclick="hasChanges(true);" runat="server" id="chkAutoApproveDR" />&nbsp;<a href="javascript: void(0);" onclick="document.getElementById('<%= chkAutoApproveDR.ClientID %>').checked=(!document.getElementById('<%= chkAutoApproveDR.ClientID %>').checked);hasChanges(true);"><asp:Label ID="lblAutoApproveDR" runat="server" Text="Se aprobarán automáticamente todas las solicitudes cuyo saldo esté ajustado"></asp:Label></a></td>
                                                                        </tr>
                                                                    </table>
                                                                </Content>
                                                            </roUserControls:roOptionPanelClient>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>
                                        </div>

                                        <!-- Panell Composicion -->
                                        <div id="div3" class="contentPanel" style="display: none" runat="server" name="menuPanel">
                                            <div class="panHeader2">
                                                <span style="">
                                                    <asp:Label runat="server" ID="lblCompTitle" Text="Composición"></asp:Label></span>
                                            </div>
                                            <br />
                                            <div style="width: auto; height: 85%; padding: 40px; padding-top: 20px;">
                                                <table border="0" style="width: 100%;" height="50">
                                                    <tbody>
                                                        <tr>
                                                            <td width="48" height="48">
                                                                <a href="javascript:void(0)" id="a1" onclick="">
                                                                    <img id="Img5" src="~/Base/Images/StartMenuIcos/Concepts.png" style="border: 0pt none;" runat="server">
                                                                </a>
                                                            </td>
                                                            <td align="left" valign="top">
                                                                <span id="span1" class="spanEmp-Class">
                                                                    <asp:Label ID="lblCompositionDesc" runat="server" Text="Defina qué justificaciones componen el acumulado. Para cada justificación que sume o reste, puede indicar el factor a multiplicar. El valor total que aparecerá en el acumulado será el de la suma de todas las justificaciones que lo componen multiplicadas por su factor."></asp:Label>
                                                                </span>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="2" align="center">
                                                                <!-- Grid JS -->
                                                                <div class="jsGrid">
                                                                    <asp:Label ID="lblConceptCompositionTitle" runat="server" CssClass="jsGridTitle" Text="Composicón del saldo"></asp:Label>
                                                                    <div id="addCompositionTable" runat="server" class="jsgridButton">
                                                                        <div class="btnFlat">
                                                                            <a href="javascript: void(0)" id="btnAddComposition" runat="server" onclick="AddNewComposition();">
                                                                                <span class="btnIconAdd"></span>
                                                                                <asp:Label ID="lblAddAus" runat="server" Text="Añadir Composición"></asp:Label>
                                                                            </a>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                                <div id="gridCompositions" class="jsGridContent" runat="server">
                                                                    <!-- Aqui va el grid de Composiciones -->
                                                                </div>

                                                                <!-- form Compositions -->
                                                                <input type="hidden" id="hdnCompositionChanges" value="0" runat="server" />
                                                                <roForms:frmCompositions ID="frmCompositions1" runat="server" />
                                                            </td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </div>

                                        <!-- Panell Redondeo -->
                                        <div id="div4" class="contentPanel" style="display: none" runat="server" name="menuPanel">
                                            <div class="panHeader2">
                                                <span style="">
                                                    <asp:Label runat="server" ID="lblRoundTitle" Text="Redondeo"></asp:Label></span>
                                            </div>
                                            <br />
                                            <table width="100%" border="0" style="padding-top: 5px; width: 100%;">
                                                <tr>
                                                    <td style="width: 100%; padding-top: 20px; padding-left: 30px; padding-right: 30px;">
                                                        <asp:Label ID="lblRoundDesc" CssClass="SectionDescription" runat="server" Text="La opción de redondeo se utiliza para cuadrar la duración del acumulado. Para indicar que desea utilizar el redondeo en este acumulado, seleccione la casilla que hay bajo estas líneas e indique el valor de redondeo. (Nota: Si el saldo tiene caducidad no aplica el valor de redondeo)"></asp:Label></td>
                                                </tr>
                                                <tr>
                                                    <td style="padding: 30px; padding-bottom: 10px; width: 100%;">
                                                        <roUserControls:roOptionPanelClient ID="optChkRound" runat="server" TypeOPanel="CheckboxOption" Width="100%" Height="Auto" Checked="False" Enabled="True" style="width: 100%;" CConClick="hasChanges(true);">
                                                            <Title>
                                                                <asp:Label ID="lblChkRound" runat="server" Text="Redondear el valor"></asp:Label>
                                                            </Title>
                                                            <Description></Description>
                                                            <Content>
                                                                <table style="width: 100%; padding-top: 10px;" border="0" width="100%">
                                                                    <tr>
                                                                        <td style="padding-left: 25px; width: 100%;">
                                                                            <asp:Label ID="lblRoundVal" runat="server" CssClass="editTextFormat" Text="Redondear el valor a "></asp:Label>
                                                                            <dx:ASPxTextBox runat="server" ID="txtRoundVal" Text="0">
                                                                                <MaskSettings IncludeLiterals="DecimalSymbol" Mask="<0..999999>.<0..9>" />
                                                                                <ClientSideEvents TextChanged="function(s,e){hasChanges(true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                            </dx:ASPxTextBox>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td style="padding-left: 75px; padding-top: 15px; padding-right: 75px; width: 100%;">
                                                                            <roUserControls:roOptionPanelClient ID="optRoundUP" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Checked="False" Enabled="True" Border="True" CConClick="hasChanges(true);">
                                                                                <Title>
                                                                                    <asp:Label ID="lblRoundUP" runat="server" Text="Redondear por arriba."></asp:Label>
                                                                                </Title>
                                                                                <Description>
                                                                                    <asp:Label ID="lblRoundUPDesc" runat="server" Text="Si la incidencia es de 1:24 se redondeará a 1:25"></asp:Label>
                                                                                </Description>
                                                                                <Content>
                                                                                </Content>
                                                                            </roUserControls:roOptionPanelClient>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td style="padding-left: 75px; padding-right: 75px; width: 100%;">
                                                                            <roUserControls:roOptionPanelClient ID="optRoundDown" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Checked="False" Enabled="True" Border="True" CConClick="hasChanges(true);">
                                                                                <Title>
                                                                                    <asp:Label ID="lblRoundDown" runat="server" Text="Redondear por abajo."></asp:Label>
                                                                                </Title>
                                                                                <Description>
                                                                                    <asp:Label ID="lblRoundDownDesc" runat="server" Text="Si la incidencia es de 1:24, se redondeará a 1:20."></asp:Label>
                                                                                </Description>
                                                                                <Content>
                                                                                </Content>
                                                                            </roUserControls:roOptionPanelClient>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td style="padding-left: 75px; padding-right: 75px; width: 100%;">
                                                                            <roUserControls:roOptionPanelClient ID="optRoundAprox" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Checked="False" Enabled="True" Border="True" CConClick="hasChanges(true);">
                                                                                <Title>
                                                                                    <asp:Label ID="lblRoundAprox" runat="server" Text="Redondear por aproximación."></asp:Label>
                                                                                </Title>
                                                                                <Description>
                                                                                    <asp:Label ID="lblRoundAproxDesc" runat="server" Text="Si la incidencia es de 1:24, se redondeará a 1:25."></asp:Label>
                                                                                </Description>
                                                                                <Content>
                                                                                </Content>
                                                                            </roUserControls:roOptionPanelClient>
                                                                            <roUserControls:roOptPanelClientGroup ID="roOptValGroup" runat="server" />
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </Content>
                                                        </roUserControls:roOptionPanelClient>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>

                                        <!-- Panell Consultas -->
                                        <div id="div5" class="contentPanel" style="display: none" runat="server" name="menuPanel">
                                            <div class="panHeader2">
                                                <span style="">
                                                    <asp:Label runat="server" ID="lblViewTitle" Text="Visualización"></asp:Label></span>
                                            </div>
                                            <table width="100%" border="0" style="padding-top: 5px">
                                                <tr>
                                                    <td style="padding-top: 5px; padding-left: 30px; padding-right: 30px">
                                                        <asp:Label ID="lblViewDesc" runat="server" CssClass="SectionDescription" Text="Formato de visualización del saldo:"></asp:Label></td>
                                                </tr>
                                                <tr>
                                                    <td style="padding-left: 60px; padding-top: 5px;">
                                                        <table style="width: 100%;">
                                                            <tr>
                                                                <td>
                                                                    <roUserControls:roOptionPanelClient ID="optViewDays" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Enabled="True" Border="True" CConClick="hasChanges(true);">
                                                                        <Title>
                                                                            <asp:Label ID="lblViewDays" runat="server" Text="Visualizar por días"></asp:Label>
                                                                        </Title>
                                                                        <Description>
                                                                            <asp:Label ID="lblViewDaysDesc" runat="server" Text="El valor del saldo se visualizará en formato numérico #0.00."></asp:Label>
                                                                        </Description>
                                                                        <Content>
                                                                        </Content>
                                                                    </roUserControls:roOptionPanelClient>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <roUserControls:roOptionPanelClient ID="optViewHours" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Enabled="True" Border="True" CConClick="hasChanges(true);">
                                                                        <Title>
                                                                            <asp:Label ID="lblViewHours" runat="server" Text="Visualizar por horas"></asp:Label>
                                                                        </Title>
                                                                        <Description>
                                                                            <asp:Label ID="lblViewHoursDesc" runat="server" Text="El valor del saldo se visualizará en formato tiempo HH:MM."></asp:Label>
                                                                        </Description>
                                                                        <Content>
                                                                        </Content>
                                                                    </roUserControls:roOptionPanelClient>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>

                                            <div class="panHeader2">
                                                <span style="">
                                                    <asp:Label runat="server" ID="lblQueryTitle" Text="Consultas"></asp:Label></span>
                                            </div>
                                            <table width="100%" border="0" style="padding-top: 5px">
                                                <tr style="display: none;">
                                                    <td style="padding-top: 10px; padding-left: 30px; padding-right: 30px">
                                                        <asp:Label ID="lblQueryDesc2" runat="server" CssClass="SectionDescription" Text="Si desea que el acumulado pueda ser visualizado por los empleados con acceso a consultas en los Terminales, marque la casilla siguiente:"></asp:Label></td>
                                                </tr>
                                                <tr style="display: none;">
                                                    <td style="padding-left: 60px; padding-top: 10px;">
                                                        <input type="checkbox" id="chkShowInTerminals" runat="server" onclick="hasChanges(true);" />&nbsp;<a href="javascript: void(0);" onclick="CheckLinkClick('chkShowInTerminals');"><asp:Label ID="lblchkShowInTerminals" runat="server" Font-Bold="true" Text="Este acumulado aparecerá en las consultas realizadas en los Terminales"></asp:Label></a>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="padding-top: 5px; padding-left: 30px; padding-right: 30px">
                                                        <asp:Label ID="lblEmployeesPermissionDesc" runat="server" CssClass="SectionDescription" Text="Quién puede consultar el ${Concept} desde un terminal ó desde Live Portal:"></asp:Label></td>
                                                </tr>
                                                <tr>
                                                    <td style="padding-left: 60px; padding-top: 5px;">
                                                        <table style="width: 100%;">
                                                            <tr>
                                                                <td>
                                                                    <roUserControls:roOptionPanelClient ID="optEmployeesPermissionAll" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Enabled="True" Border="True" CConClick="hasChanges(true);">
                                                                        <Title>
                                                                            <asp:Label ID="lblEmployeesPermissionAll" runat="server" Text="Todos"></asp:Label>
                                                                        </Title>
                                                                        <Description>
                                                                            <asp:Label ID="lblEmployeesPermissionAllDesc" runat="server" Text="Todos los ${Employees} podrán consultar el ${Concept}."></asp:Label>
                                                                        </Description>
                                                                        <Content>
                                                                        </Content>
                                                                    </roUserControls:roOptionPanelClient>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <roUserControls:roOptionPanelClient ID="optEmployeesPermissionNobody" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Enabled="True" Border="True" CConClick="hasChanges(true);">
                                                                        <Title>
                                                                            <asp:Label ID="lblEmployeesPermissionNobody" runat="server" Text="Nadie"></asp:Label>
                                                                        </Title>
                                                                        <Description>
                                                                            <asp:Label ID="lblEmployeesPermissionNobodyDesc" runat="server" Text="Ningún empleado podrá consultar el ${Concept}."></asp:Label>
                                                                        </Description>
                                                                        <Content>
                                                                        </Content>
                                                                    </roUserControls:roOptionPanelClient>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <roUserControls:roOptionPanelClient ID="optEmployeesPermissionCriteria" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Enabled="true" Border="True" CConClick="hasChanges(true);checkCriteriaVisibility();">
                                                                        <Title>
                                                                            <asp:Label ID="lblEmployeesPermissionCriteria" runat="server" Text="Según criterio"></asp:Label>
                                                                        </Title>
                                                                        <Description>
                                                                            <asp:Label ID="lblEmployeesPermissionCriteriaDesc" runat="server" Text="Los ${Employees} que cumplan los siguientes criterios podrán consultar el ${Concept}."></asp:Label>
                                                                        </Description>
                                                                        <Content>
                                                                            <table border="0" width="100%" style="padding: 20px; padding-top: 5px;" align="center">
                                                                                <tr>
                                                                                    <td id="criteriaCell" align="left" style="display: none; padding-left: 12px;">
                                                                                        <roUserControls:roUserCtlFieldCriteria2 Prefix="ctl00_contentMainBody_ASPxCallbackPanelContenido_optEmployeesPermissionCriteria_employeeCriteria" ID="employeeCriteria" runat="server" />
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </Content>
                                                                    </roUserControls:roOptionPanelClient>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                        <roUserControls:roOptPanelClientGroup ID="optEmployeesPermissionGroup" runat="server" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="padding-top: 5px; padding-left: 30px; padding-right: 30px">
                                                        <asp:Label ID="lblQueryDesc3" runat="server" CssClass="SectionDescription" Visible="false" Text="Puede configurar el orden en el que se mostrarán los acumulados en el Terminal pulsando en el botón de la barra de herramientas."></asp:Label></td>
                                                </tr>
                                            </table>
                                        </div>

                                        <!-- Panell Pagos -->
                                        <div id="div6" class="contentPanel" style="display: none" runat="server" name="menuPanel">
                                            <div class="panHeader2">
                                                <span style="">
                                                    <asp:Label runat="server" ID="lblPayTitle" Text="Pagos"></asp:Label></span>
                                            </div>
                                            <br />
                                            <table width="100%" border="0" style="padding-top: 5px">
                                                <tr>
                                                    <td style="padding-top: 20px; padding-left: 30px; padding-right: 30px">
                                                        <asp:Label ID="lblPayDesc" runat="server" CssClass="SectionDescription" Text="Si desea que el acumulado pueda ser visualizado en el listado de pagos marque la casilla que hay a continuación e indique el precio por hora."></asp:Label></td>
                                                </tr>
                                                <tr>
                                                    <td style="padding-left: 20px; padding-top: 20px;">
                                                        <roUserControls:roOptionPanelClient ID="optPayPerHours" runat="server" TypeOPanel="CheckboxOption" Width="100%" Height="Auto" Checked="False" Enabled="True" style="width: 100%;" CConClick="hasChanges(true);">
                                                            <Title>
                                                                <asp:Label ID="lblPayPerHours" runat="server" Text="Este acumulado se utilizará en los informes de pagos por horas"></asp:Label>
                                                            </Title>
                                                            <Description></Description>
                                                            <Content>
                                                                <table style="width: 100%; padding-top: 10px;" border="0" width="100%">
                                                                    <tr>
                                                                        <td style="padding-left: 25px; width: 100%;">
                                                                            <asp:Label ID="lblPricePerHour" runat="server" CssClass="editTextFormat" Text="El precio por hora será "></asp:Label>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td style="padding-left: 75px; padding-top: 15px; padding-right: 75px; width: 100%;">
                                                                            <roUserControls:roOptionPanelClient ID="optFixedByAll" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Checked="False" Enabled="True" Border="True" CConClick="hasChanges(true);">
                                                                                <Title>
                                                                                    <asp:Label ID="lblFixedByAll" runat="server" Text="Fijado para todos los empleados"></asp:Label>
                                                                                </Title>
                                                                                <Description>
                                                                                </Description>
                                                                                <Content>
                                                                                    <table>
                                                                                        <tr>
                                                                                            <td style="padding: 10px; padding-left: 30px; padding-right: 20px;">
                                                                                                <asp:Label ID="lblFixedPrice" runat="server" Text="Precio: "></asp:Label>
                                                                                                <dx:ASPxTextBox runat="server" ID="txtFixedPrice">
                                                                                                    <MaskSettings IncludeLiterals="DecimalSymbol" Mask="<-999999999..999999999>.<00..99>" />
                                                                                                    <ClientSideEvents TextChanged="function(s,e){hasChanges(true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                                                </dx:ASPxTextBox>
                                                                                            </td>
                                                                                        </tr>
                                                                                    </table>
                                                                                </Content>
                                                                            </roUserControls:roOptionPanelClient>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td style="padding-left: 75px; padding-right: 75px; width: 100%;">
                                                                            <roUserControls:roOptionPanelClient ID="optFixedByEmp" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Checked="False" Enabled="True" Border="True" CConClick="hasChanges(true);">
                                                                                <Title>
                                                                                    <asp:Label ID="lblFixedByEmp" runat="server" Text="Por empleado, y se utilizará el campo de la ficha seleccionado"></asp:Label>
                                                                                </Title>
                                                                                <Description>
                                                                                </Description>
                                                                                <Content>
                                                                                    <table>
                                                                                        <tr>
                                                                                            <td style="padding: 10px; padding-left: 30px;">
                                                                                                <asp:Label ID="lblFixedField" runat="server" Text="Campo a utilizar: "></asp:Label>&nbsp;</td>
                                                                                            <td style="padding: 10px; padding-right: 30px;">
                                                                                                <dx:ASPxComboBox ID="cmbFixedField" Width="200px" runat="server">
                                                                                                    <ClientSideEvents SelectedIndexChanged="function(s,e){ hasChanges(true)}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                                                </dx:ASPxComboBox>
                                                                                            </td>
                                                                                        </tr>
                                                                                    </table>
                                                                                </Content>
                                                                            </roUserControls:roOptionPanelClient>
                                                                        </td>
                                                                    </tr>
                                                                    <%-- 06/03/2017: Deshabilitamos el parametro "Redondear el valor a x minutos, siguiendo el documento "mejoras mayores". Aparentemente no se usaba en ningún sitio.
                                                                    Al cabo de un tiempo lo eliminaremos por completo--%>
                                                                    <tr style="visibility: hidden">
                                                                        <td style="padding-top: 10px; padding-left: 95px; padding-right: 75px; width: 100%;">
                                                                            <table>
                                                                                <tr>
                                                                                    <td>
                                                                                        <input type="checkbox" runat="server" id="chkPayRound" class="textClass" onclick="hasChanges(true);" />&nbsp;
                                                                                    </td>
                                                                                    <td>
                                                                                        <a href="javascript: void(0);" onclick="document.getElementById('<%= chkPayRound.ClientID %>').checked=(!document.getElementById('<%= chkPayRound.ClientID %>').checked);hasChanges(true);">
                                                                                            <asp:Label ID="lblPayRound" runat="server" Text="Redondear el valor a"></asp:Label>
                                                                                        </a>&nbsp;
                                                                                    </td>
                                                                                    <td style="max-width: 180px">
                                                                                        <dx:ASPxTextBox ID="txtPayRound" runat="server">
                                                                                            <MaskSettings Mask="<-999999..999999>.<00..99>" IncludeLiterals="DecimalSymbol" />
                                                                                            <ClientSideEvents TextChanged="function(s,e){hasChanges(true)}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                                        </dx:ASPxTextBox>
                                                                                        &nbsp;
                                                                                    </td>
                                                                                    <td>
                                                                                        <asp:Label ID="lblPayRoundMinutes" runat="server" Text="minutos."></asp:Label>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </Content>
                                                        </roUserControls:roOptionPanelClient>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>

                                        <!-- Panell Absentismo -->
                                        <div id="div7" class="contentPanel" style="display: none" runat="server" name="menuPanel">
                                            <div class="panHeader2">
                                                <span style="">
                                                    <asp:Label runat="server" ID="lblAbsTitle" Text="Absentismo"></asp:Label></span>
                                            </div>
                                            <br />
                                            <table width="100%" border="0" style="padding-top: 5px">
                                                <tr>
                                                    <td style="padding-top: 20px; padding-left: 30px; padding-right: 30px">
                                                        <asp:Label ID="lblAbsDesc" runat="server" CssClass="SectionDescription" Text="Si desea que el acumulado pueda ser visualizado en el listado de absentismos marque la casilla que hay a continuación e indique si desea que sea retribuido."></asp:Label></td>
                                                </tr>
                                                <tr>
                                                    <td style="padding-left: 60px; padding-top: 20px; padding-bottom: 10px; padding-right: 20px;">
                                                        <roUserControls:roOptionPanelClient ID="optCheckAbsReport" runat="server" TypeOPanel="CheckboxOption" Width="100%" Height="Auto" Checked="False" Enabled="True" CConClick="hasChanges(true);">
                                                            <Title>
                                                                <asp:Label ID="lblCheckAbsReport" runat="server" Text="Este acumulado se utilizará en informes de absentismo."></asp:Label>
                                                            </Title>
                                                            <Description>
                                                            </Description>
                                                            <Content>
                                                                <table>
                                                                    <tr>
                                                                        <td style="padding-top: 15px; padding-left: 25px;">
                                                                            <input type="checkbox" onclick="hasChanges(true);" runat="server" id="chkAbsRet" />&nbsp;<a href="javascript: void(0);" onclick="document.getElementById('<%= chkAbsRet.ClientID %>').checked=(!document.getElementById('<%= chkAbsRet.ClientID %>').checked);hasChanges(true);"><asp:Label ID="lblAbsRet" runat="server" Text="El absentismo reflejado por este acumulado será retribuido."></asp:Label></a></td>
                                                                    </tr>
                                                                </table>
                                                            </Content>
                                                        </roUserControls:roOptionPanelClient>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="padding-left: 60px; padding-top: 5px; padding-bottom: 10px; padding-right: 20px;">
                                                        <roUserControls:roOptionPanelClient ID="chkAbsWorkHours" runat="server" TypeOPanel="CheckboxOption" Width="100%" Height="Auto" Checked="False" Enabled="True" CConClick="hasChanges(true);">
                                                            <Title>
                                                                <asp:Label ID="lblAbsWorkHours" runat="server" Text="Este acumulado es el acumulado de horas trabajadas."></asp:Label>
                                                            </Title>
                                                            <Description>
                                                            </Description>
                                                            <Content>
                                                            </Content>
                                                        </roUserControls:roOptionPanelClient>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>

                                        <!-- Panell Periodo de Validez -->
                                        <div id="div8" class="contentPanel" style="display: none" runat="server" name="menuPanel">
                                            <div class="panHeader2">
                                                <span style="">
                                                    <asp:Label runat="server" ID="lblPeriodTitle" Text="Periodo de Validez"></asp:Label></span>
                                            </div>
                                            <br />
                                            <table width="100%" border="0" style="padding-top: 5px">
                                                <tr>
                                                    <td style="padding-top: 20px; padding-left: 30px; padding-right: 30px;">
                                                        <img alt="" id="Img4" src="~/Base/Images/MessageFrame/Alert16.png" runat="server" />
                                                        <asp:Label ID="lblPeriodNote" runat="server" CssClass="alertColor" Text="Atención: VisualTime actualiza de forma automática los datos del período de validez. No debería modificar estos datos si no se trata de modificaciones avanzadas."></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="padding-top: 20px; padding-left: 30px; padding-right: 30px">
                                                        <asp:Label ID="lblPeriodDesc" runat="server" CssClass="SectionDescription" Text="El período de validez indica entre que fechas es válido este acumulado. Se utiliza de forma automática cuando, en algún momento, cambia el convenio y se realizan cambios en la definición de este acumulado. VisualTime guarda entonces las dos definiciones de este acumulado, la nueva y la anterior, indicando entre qué fechas debe usar cada una."></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="padding-left: 60px; padding-top: 20px; padding-bottom: 10px; padding-right: 20px;">
                                                        <table border="0">
                                                            <tr>
                                                                <td valign="top">
                                                                    <asp:Label ID="lblValidPeriod" runat="server" Text="Este acumulado es válido desde" class="spanEmp-Class"></asp:Label>
                                                                </td>
                                                                <td valign="top" style="width: 90px;">
                                                                    <dx:ASPxDateEdit ID="dpPeriodStart" runat="server" Width="105" AllowNull="false">
                                                                        <ClientSideEvents DateChanged="function(s,e){PeriodHasChanges();hasChanges(true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                    </dx:ASPxDateEdit>
                                                                </td>
                                                                <td valign="top">
                                                                    <asp:Label ID="lblPeriodTo" runat="server" Text="hasta" class="spanEmp-Class"></asp:Label>
                                                                </td>
                                                                <td valign="top" style="width: 90px;">
                                                                    <dx:ASPxDateEdit ID="dpPeriodEnd" runat="server" Width="105" AllowNull="true">
                                                                        <ClientSideEvents DateChanged="function(s,e){PeriodHasChanges();hasChanges(true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                    </dx:ASPxDateEdit>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>

                                        <!-- Panell Devengos -->
                                        <div id="div9" class="contentPanel" style="display: none" runat="server" name="menuPanel">
                                            <div class="panHeader2">
                                                <span style="">
                                                    <asp:Label runat="server" ID="lblAutomaticAccrualsTitle" Text="Devengos"></asp:Label></span>
                                            </div>
                                            <br />
                                            <table width="100%" border="0" style="padding-top: 5px">
                                                <tr>
                                                    <td style="padding-top: 20px; padding-left: 30px; padding-right: 30px">
                                                        <asp:Label ID="lblAutomaticAccrualsDesc" runat="server" CssClass="editTextFormat" Text="Si desea que el saldo pueda generar devengos automáticos debe indicarlo a continuación configurando los parámetros especificados."></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="padding-left: 60px; padding-top: 20px; padding-bottom: 10px; padding-right: 20px;">
                                                        <table style="width: 100%; padding-top: 10px;" border="0" width="100%">
                                                            <tr>
                                                                <td style="padding-left: 25px; padding-top: 15px; padding-right: 25px; width: 100%;">
                                                                    <roUserControls:roOptionPanelClient ID="optNoAutomaticAccruals" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Checked="False" Enabled="True" Border="True" Value="0" CConClick="NoAutomaticAccrualHasChanges(true, false);">
                                                                        <Title>
                                                                            <asp:Label ID="lblNoAutomaticAccruals" runat="server" Text="No realizar devengos automáticos"></asp:Label>
                                                                        </Title>
                                                                        <Description>
                                                                            <asp:Label ID="lblNoAutomaticAccrualsDesc" runat="server" Text="No se realizará ningún tipo de devengo automático relacionado con el saldo."></asp:Label>
                                                                        </Description>
                                                                        <Content>
                                                                        </Content>
                                                                    </roUserControls:roOptionPanelClient>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td style="padding-left: 25px; padding-right: 25px; width: 100%;">
                                                                    <roUserControls:roOptionPanelClient ID="optHoursAutomaticAccruals" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Checked="False" Enabled="True" Border="True" Value="2" CConClick="AutomaticAccrualHoursHasChanges(true, false, false);">
                                                                        <Title>
                                                                            <asp:Label ID="lbloptHoursAutomaticAccruals" runat="server" Text="Devengo por horas"></asp:Label>
                                                                        </Title>
                                                                        <Description>
                                                                            <asp:Label ID="lbloptHoursAutomaticAccrualsDesc" runat="server" Text="El saldo actual va a generar un devengo automático en función de las horas generadas. Aplicable a saldos por horas o días."></asp:Label>
                                                                        </Description>
                                                                        <Content>
                                                                            <div class="divRow" style="padding-top: 5px">
                                                                                <div id="divHoursAutomaticAccrual_lblHours" runat="server" style="display: none" class="labelFloat">
                                                                                    <asp:Label ID="lblHoursAutomaticAccrualTime" runat="server" Text="Generar horas de devengo a razón de: "></asp:Label>
                                                                                </div>

                                                                                <div id="divHoursAutomaticAccrual_lblDays" runat="server" style="display: none" class="labelFloat">
                                                                                    <asp:Label ID="lblHoursAutomaticAccrualCount" runat="server" Text="Generar días de devengo a razón de:"></asp:Label>
                                                                                </div>
                                                                                <div class="componentFloat">
                                                                                    <dx:ASPxComboBox runat="server" ID="cmbHoursAutomaticAccrual" Width="200px" ClientInstanceName="cmbHoursAutomaticAccrual_Client">
                                                                                        <ClientSideEvents SelectedIndexChanged="function(s,e){AutomaticAccrualHoursHasChanges(true, true, true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                                        <ValidationSettings ErrorDisplayMode="None" />
                                                                                    </dx:ASPxComboBox>
                                                                                </div>

                                                                                <div id="divHoursAutomaticAccrual_Userfield" style="display: none" class="componentFloat">
                                                                                    <dx:ASPxComboBox runat="server" ID="cmbHoursAutomaticAccrual_Userfield" Width="175px">
                                                                                        <ClientSideEvents SelectedIndexChanged="function(s,e){AutomaticAccrualHoursHasChanges(true, true, true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                                        <ValidationSettings ErrorDisplayMode="None" />
                                                                                    </dx:ASPxComboBox>
                                                                                </div>

                                                                                <div id="divHoursAutomaticAccrual_Fixed" style="margin-top: -6px" class="componentFloat">
                                                                                    <dx:ASPxTextBox ID="txtHoursAutomaticAccrual_Fixed" runat="server">
                                                                                        <ClientSideEvents TextChanged="function(s,e){ AutomaticAccrualHoursHasChanges(true, true, true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                                        <ValidationSettings ErrorDisplayMode="None" />
                                                                                        <MaskSettings Mask="<-9999..9999>.<000000..999999>" ErrorText="" />
                                                                                    </dx:ASPxTextBox>
                                                                                </div>
                                                                                <div class="labelFloat">
                                                                                    <asp:Label ID="lblHoursAutomaticDesc" runat="server" Text="por cada hora de las siguientes justificaciones"></asp:Label>
                                                                                </div>
                                                                            </div>
                                                                            <div class="divRow">
                                                                                <div id="lstCausesHoursAutomaticAccrual">
                                                                                </div>
                                                                            </div>
                                                                        </Content>
                                                                    </roUserControls:roOptionPanelClient>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td style="padding-left: 25px; padding-right: 25px; width: 100%;">
                                                                    <roUserControls:roOptionPanelClient ID="optDaysAutomaticAccruals" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Checked="False" Enabled="True" Border="True" Value="1" CConClick="AutomaticAccrualDayHasChanges(true, false, false);">
                                                                        <Title>
                                                                            <asp:Label ID="lbloptDaysAutomaticAccruals" runat="server" Text="Devengo por días"></asp:Label>
                                                                        </Title>
                                                                        <Description>
                                                                            <asp:Label ID="lbloptDaysAutomaticAccrualsDesc" runat="server" Text="El saldo actual va a generar un devengo automático en función de los días generados. Solo aplica a saldos por días."></asp:Label>
                                                                        </Description>
                                                                        <Content>
                                                                            <div class="divRow" style="padding-top: 5px">
                                                                                <div class="labelFloat">
                                                                                    <asp:Label ID="lblDaysAutomaticAccrualDesc" runat="server" Text="Generar días de devengo a razón de "></asp:Label>
                                                                                </div>
                                                                                <div class="componentFloat">
                                                                                    <dx:ASPxComboBox runat="server" ID="cmbDaysAutomaticAccrual" Width="200px" ClientInstanceName="cmbDaysAutomaticAccrual_Client">
                                                                                        <ClientSideEvents SelectedIndexChanged="function(s,e){AutomaticAccrualDayHasChanges(true, true, true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                                        <ValidationSettings ErrorDisplayMode="None" />
                                                                                    </dx:ASPxComboBox>
                                                                                </div>

                                                                                <div id="divDaysAutomaticAccrual_Userfield" style="display: none" class="componentFloat">
                                                                                    <dx:ASPxComboBox runat="server" ID="cmbDaysAutomaticAccrual_Userfield" Width="175px">
                                                                                        <ClientSideEvents SelectedIndexChanged="function(s,e){AutomaticAccrualDayHasChanges(true, true, true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                                        <ValidationSettings ErrorDisplayMode="None" />
                                                                                    </dx:ASPxComboBox>
                                                                                </div>

                                                                                <div id="divDaysAutomaticAccrual_Fixed" style="margin-top: -6px" class="componentFloat">
                                                                                    <dx:ASPxTextBox ID="txtDaysAutomaticAccrual_Fixed" runat="server">
                                                                                        <ClientSideEvents TextChanged="function(s,e){ AutomaticAccrualDayHasChanges(true, true, true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                                        <ValidationSettings ErrorDisplayMode="None" />
                                                                                        <MaskSettings Mask="<-9999..9999>.<000000..999999>" ErrorText="" />
                                                                                    </dx:ASPxTextBox>
                                                                                </div>

                                                                                <div class="labelFloat">
                                                                                    <asp:Label ID="lblDaysAutomaticAccrualDesc2" runat="server" Text="por cada día de contrato"></asp:Label>
                                                                                </div>
                                                                            </div>

                                                                            <div class="divRow">
                                                                                <dx:ASPxRadioButton ID="ckDaysAutomaticAccrualAllDays" ClientInstanceName="ckDaysAutomaticAccrualAllDays_client" runat="server" Text="Todos los días devengan excepto si contienen las siguientes justificaciones u horarios" GroupName="ckDaysAutomaticAccrual">
                                                                                    <ClientSideEvents CheckedChanged="function(s,e){AutomaticAccrualDayHasChanges(true, true, true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                                </dx:ASPxRadioButton>
                                                                                <dx:ASPxRadioButton ID="ckDaysAutomaticAccrualOnlyDays" ClientInstanceName="ckDaysAutomaticAccrualOnlyDays_client" runat="server" Text="Solo devengan los días que contengan las siguientes justificaciones u horarios" GroupName="ckDaysAutomaticAccrual">
                                                                                    <ClientSideEvents CheckedChanged="function(s,e){AutomaticAccrualDayHasChanges(true, true, true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                                </dx:ASPxRadioButton>
                                                                            </div>

                                                                            <div class="divRow">
                                                                                <asp:Label ID="lblCausesDaysAutomaticAccrual" runat="server" Text="Justificaciones"></asp:Label><br />
                                                                                <div id="lstCausesDaysAutomaticAccrual">
                                                                                </div>
                                                                            </div>

                                                                            <div class="divRow">
                                                                                <asp:Label ID="lblShiftsDaysAutomaticAccrual" runat="server" Text="Horarios"></asp:Label><br />
                                                                                <div id="lstShiftsDaysAutomaticAccrual">
                                                                                </div>
                                                                            </div>
                                                                        </Content>
                                                                    </roUserControls:roOptionPanelClient>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td style="padding-left: 25px; padding-right: 25px; width: 100%;">
                                                                    <div class="divRow">
                                                                        <div class="labelFloat">
                                                                            <asp:Label ID="lblAutomaticAccrualCause" runat="server" Text="Justificación asociada para generar el valor del devengo"></asp:Label>
                                                                        </div>
                                                                        <div class="componentFloat">
                                                                            <dx:ASPxComboBox runat="server" ID="cmbAutomaticAccrualCause" Width="175px" ClientInstanceName="cmbAutomaticAccrualCause_Client">
                                                                                <ClientSideEvents SelectedIndexChanged="function(s,e){NoAutomaticAccrualHasChanges(true, true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                                <ValidationSettings ErrorDisplayMode="None" />
                                                                            </dx:ASPxComboBox>
                                                                        </div>
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </div>

                                    <div id="ConceptGroupContent" runat="server" class="contentPanel" style="display: none">
                                        <div id="div20" class="contentPanel" style="display: none" runat="server" name="menuPanel">
                                            <!-- Este div es el header -->
                                            <div class="panBottomMargin">
                                                <div class="panHeader2 panBottomMargin">
                                                    <span class="panelTitleSpan">
                                                        <asp:Label runat="server" ID="lblConceptGroupGeneralTitle" Text="General"></asp:Label>
                                                    </span>
                                                </div>
                                                <!-- La descripción es opcional -->
                                                <div class="panelHeaderContent">
                                                    <div class="panelDescriptionImage">
                                                        <img alt="" src="<%=Me.Page.ResolveUrl("~/Base/Images/StartMenuIcos/ConceptGroups.png")%>" />
                                                    </div>
                                                    <div class="panelDescriptionText">
                                                        <asp:Label ID="lblConceptGroupsDesc" runat="server" Text="Datos específicos de un grupo de saldos"></asp:Label>
                                                    </div>
                                                </div>
                                            </div>

                                            <!-- Este div es un formulario -->
                                            <div class="panBottomMargin">
                                                <div class="divRow">
                                                    <div class="divRowDescription">
                                                        <asp:Label ID="lblConceptGroupNameDescription" runat="server" Text="Nombre identificativo del grupo de saldos"></asp:Label>
                                                    </div>
                                                    <asp:Label ID="lblConceptGroupName" runat="server" Text="Nombre:" CssClass="labelForm"></asp:Label>
                                                    <div class="componentForm">
                                                        <dx:ASPxTextBox ID="txtConceptGroupName" runat="server" ClientInstanceName="txtConceptGroupName_Client" NullText="_____">
                                                            <ClientSideEvents Validation="LengthValidation" TextChanged="function(s,e){checkConceptGroupEmptyName(s.GetValue());}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                            <ValidationSettings SetFocusOnError="True">
                                                                <RequiredField IsRequired="True" ErrorText="(*)" />
                                                            </ValidationSettings>
                                                        </dx:ASPxTextBox>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="panBottomMargin">
                                                <div class="panHeader2 panBottomMargin">
                                                    <span class="panelTitleSpan">
                                                        <asp:Label runat="server" ID="lblBusinessGroupDesc" Text="Grupo de Negocio"></asp:Label>
                                                    </span>
                                                </div>
                                            </div>
                                            <div class="panBottomMargin">
                                                <div class="divRow">
                                                    <div class="divRowDescription">
                                                        <asp:Label ID="lblBusinessGroupDescPpal" runat="server" Text="Los grupos de negocio se utilizan para limitar los grupos de saldos que cada usuario en la pantalla de edición de fichajes, informes, analitíca, etc.."></asp:Label>
                                                    </div>
                                                    <asp:Label ID="lblBusinessGroup" runat="server" Text="Grupo de Negocio:" CssClass="labelForm"></asp:Label>
                                                    <div class="componentForm">
                                                        <dx:ASPxTextBox ID="txtBusinessGroup" runat="server">
                                                            <ClientSideEvents TextChanged="function(s,e){ hasChanges(true,false);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                        </dx:ASPxTextBox>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="panBottomMargin">
                                                <div class="panHeader2">
                                                    <span style="">
                                                        <asp:Label runat="server" ID="lblConceptTitle" Text="Grupo de Acumulados"></asp:Label></span>
                                                </div>
                                            </div>
                                            <br />
                                            <div class="panBottomMargin">
                                                <div class="divRow">
                                                    <!-- Grid JS -->
                                                    <div class="jsGrid">
                                                        <asp:Label ID="lblConceptGroupGridTitle" runat="server" CssClass="jsGridTitle" Text="Composicón de grupo de saldos"></asp:Label>
                                                        <div id="addConceptTable" runat="server" class="jsgridButton">
                                                            <div class="btnFlat">
                                                                <a href="javascript: void(0)" id="btnAddConceptGroup" runat="server" onclick="AddNewConceptGroup();">
                                                                    <span class="btnIconAdd"></span>
                                                                    <asp:Label ID="lblAddConceptGroup" runat="server" Text="Añadir Acumulado"></asp:Label>
                                                                </a>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div id="gridConceptGroups" class="jsGridContent" runat="server">
                                                        <!-- Aqui va el grid de Composiciones -->
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
        </div>
        <!-- POPUP NEW OBJECT -->
        <dx:ASPxPopupControl ID="NewObjectPopup" runat="server" AllowDragging="False" CloseAction="None" Modal="True" ContentUrl="~/Base/Popups/CreateObjectPopup.aspx"
            PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter" Width="470px" Height="300px"
            ShowHeader="False" ScrollBars="Auto" ShowPageScrollbarWhenModal="True" ClientInstanceName="NewObjectPopup_Client" PopupAnimationType="None" BackColor="Transparent" ContentStyle-Paddings-Padding="0px" Border-BorderColor="Transparent" ShowShadow="false">
            <SettingsLoadingPanel Enabled="false" />
        </dx:ASPxPopupControl>
    </div>

    <script language="javascript" type="text/javascript">

        function resizeTreeConcepts() {
            try {
                var ctlPrefix = "<%= roTreesConcepts.ClientID %>";
                eval(ctlPrefix + "_resizeTrees();");

                var ctlPrefixGroups = "<%= roTreesConceptGroups.ClientID %>";
                eval(ctlPrefixGroups + "_resizeTrees();");
            }
            catch (e) {
                showError("resizeTreeConcepts", e);
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
            resizeTreeConcepts();
        }

        if (document.getElementById('<%= noRegs.ClientID %>').value != '') {
            newConcept();
        }
    </script>
</asp:Content>