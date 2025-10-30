<%@ Page Language="VB" MasterPageFile="~/Base/mastEmp.master" AutoEventWireup="false" Inherits="VTLive40.Shiftsv2" Title="${Shifts}" EnableEventValidation="false" CodeBehind="Shiftsv2.aspx.vb" %>

<%@ Register Src="~/Shifts/WebUserForms/frmEditShiftFlexible.ascx" TagName="frmEditShiftFlexible" TagPrefix="roForms" %>
<%@ Register Src="~/Shifts/WebUserForms/frmEditShiftMandatory.ascx" TagName="frmEditShiftMandatory" TagPrefix="roForms" %>
<%@ Register Src="~/Shifts/WebUserForms/frmEditShiftBreak.ascx" TagName="frmEditShiftBreak" TagPrefix="roForms" %>
<%@ Register Src="~/Shifts/WebUserForms/frmAddZone.ascx" TagName="frmAddZone" TagPrefix="roForms" %>
<%@ Register Src="~/Shifts/WebUserForms/frmDailyRule.ascx" TagName="frmDailyRule" TagPrefix="roForms" %>

<asp:Content ID="cMainBody" ContentPlaceHolderID="contentMainBody" runat="Server">

    <script language="javascript" type="text/javascript">

        function PageBase_Load() {

            resizeFrames();
            resizeShiftTrees();

            var oTreePath = readCookie("ctl00_contentMainBody_roTreesShifts_Selector_SelectedPath1");
            if (oTreePath == null) {
                var ctlPrefix = "ctl00_contentMainBody_roTreesShifts_roTrees";
                eval(ctlPrefix + ".TreeSelectNode('1',null,null,true);");
            }
            
        }        
               


$( document ).ready(function() {
$("#ShiftReset").dxButton({"icon":"undo","onClick":undoChanges,"type":"default"});
$("#ShiftDelete").dxButton({"icon":"trash","onClick":ShowRemoveShift,"type":"danger"});
$("#ShiftSave").dxButton({"icon":"todo","onClick":saveChanges,"type":"success"});
});
        
    </script>
    <input type="hidden" runat="server" id="hdnModeEdit" value="" />    
    <input type="hidden" runat="server" id="noRegs" value="" />
    <div id="divModalBgDisabled" class="modalBackground" style="position: absolute; left: 0px; top: 0px; z-index: 990; width: 1680px; height: 900px; display: none;"></div>
    <input type="hidden" id="dateFormatValue" runat="server" value="" />
    <input type="hidden" id="OperationPlus" value="<%=  Me.Language.Translate("Compositions.OperationsPlus", DefaultScope) %>" />
    <input type="hidden" id="OperationMinus" value="<%=  Me.Language.Translate("Compositions.OperationMinus", DefaultScope) %>" />
    <input type="hidden" id="header1" value="<%= Me.Language.Translate("Compositions.gridHeaderCause", DefaultScope) %>" />
    <input type="hidden" id="header2" value="<%= Me.Language.Translate("Compositions.gridHeaderOperation", DefaultScope) %>" />
    <input type="hidden" id="header3" value="<%= Me.Language.Translate("Compositions.gridHeaderTimeZone", DefaultScope) %>" />
    <input type="hidden" id="origen" value="<%= Me.Language.Translate("Compositions.gridHeaderOrigen", DefaultScope) %>" />
    <input type="hidden" id="destino" value="<%= Me.Language.Translate("Compositions.gridHeaderDestino", DefaultScope) %>" />    

    <input type="hidden" id="msgHasChanges" value="" runat="server" clientidmode="Static" />
    <input type="hidden" id="msgHasErrors" value="" runat="server" clientidmode="Static" />
    <input type="hidden" id="msgErrorNoCorrectRule" runat="server" clientidmode="Static" /> 
    <dx:ASPxHiddenField ID="hdnShiftFlags" runat="server" ClientInstanceName="hdnShiftFlagsClient"></dx:ASPxHiddenField>
    <div id="divMainBody">
        <!-- TAB SUPERIOR -->
        <div id="divTabInfo" class="divDataCells">
            <div style="min-height: 10px"></div>
            <div id="divShifts" class="blackRibbonTitle">
            </div>
            <div style="min-height: 10px"></div>
        </div>
        <!-- FIN TAB SUPERIOR -->

        <!-- ARBOL Y DETALLE -->
        <div id="divTabData" class="divDataCells">

            <div id="divTree" class="treeSize">
                <div id="ctlTreeDiv" style="height: 500px;">
                    <rws:roTreesSelector ID="roTreesShifts" runat="server" ShowEmployeeFilters="false" PrefixTree="roTreesShifts"
                        Tree1Visible="true" Tree1MultiSel="false" Tree1ShowOnlyGroups="false" Tree1Function="cargaNodo" Tree1ImagePath="images/ShiftSelector" Tree1SelectorPage="../../Shifts/ShiftSelectorData.aspx"
                        ShowTreeCaption="true"></rws:roTreesSelector>
                </div>
            </div>

            <div id="divButtons" class="divMiddleButtons">
                <div id="divBarButtons" class="maxHeight">&nbsp</div>
            </div>
            <div id="actionButtons" style="display:none;">
                <div id="ShiftDelete"></div>
<div id="ShiftReset" /></div>
<div id="ShiftSave"></div>
</div>
            <div id="divContenido" class="divRightContent">
                                <div id="divContent" class="maxHeight">                    
                    <dx:ASPxCallbackPanel ID="ASPxCallbackPanelContenido" runat="server" Width="100%" Height="100%" ClientInstanceName="ASPxCallbackPanelContenidoClient">
                        <SettingsLoadingPanel Enabled="false" />
                        <ClientSideEvents EndCallback="ASPxCallbackPanelContenidoClient_EndCallBack" />
                        <PanelCollection>
                            <dx:PanelContent ID="PanelContent1" runat="server">                                
                                    
                                <div id="divMsgTop" class="divMsg2 divMessageTop" style="display: none">                                    
                                    <div class="messageText">
                                        <span id="msgTop"></span>
                                    </div>
                                                                        
                                </div>

                                <dx:ASPxHiddenField ID="recalcConfig" runat="server" ClientInstanceName="recalcConfigClient"></dx:ASPxHiddenField>
                                <input type="hidden" id="isHRScheduling" value="0" runat="server" />

                                <!-- Div flotant canvis a rules -->
                                <div id="divMsgChg" style="position: absolute; z-index: 995; display: none; top: 50%; left: 50%; margin-left: -238px; margin-top: -182px;">
                                    <div id="divBgMsgS" style="position: absolute; top: 0; left: 0; display: none; z-index: 991;"></div>
                                    <div class="bodyPopupExtended" style="">
                                        <!-- Controls Popup Aqui -->
                                        <table width="450px" height="300px" border="0">
                                            <tr>
                                                <td height="32px" width="50px" align="center">
                                                    <img id="Img3" src="~/Base/Images/StartMenuIcos/Shifts.png" runat="server" /></td>
                                                <td height="32px" valign="middle" style="padding-left: 5px;">
                                                    <asp:Label ID="lblTitleRecalculateShift" runat="server" Text="Recalcular el Shifto" Font-Size="Large" Font-Bold="true"></asp:Label></td>
                                            </tr>
                                            <tr>
                                                <td colspan="2" valign="top">
                                                    <asp:Label ID="lblRecalcDesc" CssClass="spanEmp-Class" runat="server" Text="Ha realizado cambios en la composición del Shifto actual. El Shifto se debe recalcular."></asp:Label>
                                                    <asp:Label ID="lblRecalcDesc2" CssClass="spanEmp-Class" runat="server" Text="Elija el modo de recálculo del Shifto."></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                    <table>
                                                        <tr>
                                                            <td>
                                                                <roUserControls:roOptionPanelClient ID="optRecalcByDate" runat="server" TypeOPanel="RadioOption" width="100%" height="Auto" Checked="False" Enabled="True" Border="True" CConClick="hasChanges(true);">
                                                                    <Title>
                                                                        <asp:Label ID="lblRecalcByDateTitle" runat="server" Text="Recalcular a partir de una fecha especificada."></asp:Label>
                                                                    </Title>
                                                                    <Description>
                                                                        <asp:Label ID="lblRecalcByDateDesc" runat="server" Text="Se hará una copia del horario y se recalculará el nuevo horario desde la fecha especificada."></asp:Label>
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
                                                                <roUserControls:roOptionPanelClient ID="optRecalcAllDays" runat="server" TypeOPanel="RadioOption" width="100%" height="Auto" Checked="False" Enabled="True" Border="True" CConClick="hasChanges(true);">
                                                                    <Title>
                                                                        <asp:Label ID="lblRecalcAllDaysTitle" runat="server" Text="Recalcular todos los días"></asp:Label>
                                                                    </Title>
                                                                    <Description>
                                                                        <asp:Label ID="lblRecalcAllDaysDesc" runat="server" Text="Se recalculará el Shifto para todos los días donde el Shifto sea válido."></asp:Label>
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
                                                                <dx:ASPxButton ID="ASPxButton1" runat="server" AutoPostBack="False" CausesValidation="False" Text="Aceptar" ToolTip="Aceptar" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                                                    <ClientSideEvents Click="function(s,e){ recalcRules(); }" />
                                                                </dx:ASPxButton>
                                                            </td>
                                                            <td style="width: 110px;" align="left">
                                                                <dx:ASPxButton ID="ASPxButton2" runat="server" AutoPostBack="False" CausesValidation="False" Text="Cancelar" ToolTip="Cancelar" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                                                    <ClientSideEvents Click="function(s,e){ closeWndRecalShift(); }" />
                                                                </dx:ASPxButton>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                </div>
                                <!-- End Div flotant rules -->
                                <input type="hidden" id="hdnRecalcChanges" value="0" runat="server" />
                                <!-- Div flotant Rules -->
                                <input type="hidden" id="hdnRuleChanges" value="0" runat="server" />
                                <div id="divRule" style="position: fixed; z-index: 9010; display: none; top: 50%; left: 50%;">

                                    <!-- <div id="divBgS" style="position: absolute; top: 0; left: 0; display: none; z-index: 15009;"></div>  -->
                                    <div id="divBgS" style="position: absolute; top: 0; left: 0; display: none; z-index: 9009;"></div>
                                    <div class="bodyPopupExtended" style="">
                                        <!-- Controls Popup Aqui -->
                                        <div id="divRules" runat="server" style="">
                                            <div style="width: 100%; height: 100%;" class="bodyPopup defaultContrastColor">
                                                <!-- Tags idioma per passar a js -->
                                                <input type="hidden" id="hdnIDShift" />
                                                <input type="hidden" id="hdnIDRule" />
                                                <input type="hidden" id="hdnNewRule" />
                                                <table style="width: 100%; padding-top: 5px;" border="0">
                                                    <tr>
                                                        <td colspan="2">
                                                            <div class="panHeader2">
                                                                <span style="">
                                                                    <asp:Label runat="server" ID="lblRuleTit" Text="Reglas de justificación"></asp:Label></span>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="2" style="padding: 2px;">
                                                            <table border="0" style="width: 100%;">
                                                                <tr>
                                                                    <td>
                                                                        <img alt="" src="Images/Rules.PNG" style="display: none;" /></td>
                                                                    <td style="text-align: left; width: 460px">
                                                                        <asp:Label ID="lblTitleFormRule" runat="server" CssClass="spanEmp-class" Text="Este formulario le permite crear o modificar las reglas horarias." Width="465px"></asp:Label></td>
                                                                </tr>
                                                                <tr>
                                                                    <td colspan="2" style="padding-top: 5px;">
                                                                        <table border="0" cellpadding="2" cellspacing="2">
                                                                            <tr>
                                                                                <td colspan="4" style="text-align: left; padding-bottom: 5px;">
                                                                                    <asp:Label ID="lblRuleTitleBold" runat="server" Text="Regla" Font-Bold="true"></asp:Label></td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td>
                                                                                    <asp:Label ID="lblCaptionIf1" runat="server" Text="Si queda"></asp:Label></td>
                                                                                <td>
                                                                                    <dx:ASPxComboBox runat="server" ID="cmbRuleConcept1" Width="175px" ClientInstanceName="cmbRuleConcept1Client">
                                                                                        <ClientSideEvents SelectedIndexChanged="function(s,e){}" />
                                                                                    </dx:ASPxComboBox>
                                                                                    <dx:ASPxComboBox runat="server" ID="cmbRuleConcept2" Width="175px" ClientInstanceName="cmbRuleConcept2Client">
                                                                                        <ClientSideEvents SelectedIndexChanged="function(s,e){}" />
                                                                                    </dx:ASPxComboBox>
                                                                                </td>
                                                                                <td style="padding-left: 2px; padding-right: 2px;">
                                                                                    <asp:Label ID="lblCaptionIf2" runat="server" Text="en"></asp:Label></td>
                                                                                <td>
                                                                                    <dx:ASPxComboBox runat="server" ID="cmbRuleZone1" Width="175px" ClientInstanceName="cmbRuleZone1Client">
                                                                                        <ClientSideEvents SelectedIndexChanged="function(s,e){}" />
                                                                                    </dx:ASPxComboBox>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td></td>
                                                                                <td>
                                                                                    <dx:ASPxComboBox runat="server" ID="cmbRuleCriteria1" Width="175px" ClientInstanceName="cmbRuleCriteria1Client">
                                                                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ showCriteria1(s.GetSelectedIndex()); }" />
                                                                                    </dx:ASPxComboBox>
                                                                                </td>
                                                                                <td></td>
                                                                                <td>
                                                                                    <div id="divConditionValueType" style="display: none;">
                                                                                        <dx:ASPxComboBox runat="server" ID="cmbRuleConditionValueType" Width="175px" ClientInstanceName="cmbRuleConditionValueTypeClient">
                                                                                            <ClientSideEvents SelectedIndexChanged="function(s,e){ showConditionValueType(s.GetSelectedIndex()); }" />
                                                                                        </dx:ASPxComboBox>
                                                                                    </div>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td></td>
                                                                                <td colspan="3">
                                                                                    <div id="divConditionValueDirect" style="display: none;">
                                                                                        <table border="0" cellpadding="0" cellspacing="0">
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <div style="width: 75px; display: none;" id="divSelCriteria11">
                                                                                                        <dx:ASPxTimeEdit ID="txtCriteria11" runat="server" Width="85px" EditFormatString="HH:mm" EditFormat="Custom" ClientInstanceName="txtCriteria11Client">
                                                                                                            <ClientSideEvents DateChanged="function(s,e){}" />
                                                                                                        </dx:ASPxTimeEdit>
                                                                                                    </div>
                                                                                                </td>
                                                                                                <td>
                                                                                                    <div style="display: none; padding-left: 4px; padding-right: 4px;" id="divSelCriteria10">
                                                                                                        <asp:Label ID="lblCriteriaAnd" runat="server" Text=" y "></asp:Label>
                                                                                                    </div>
                                                                                                </td>
                                                                                                <td>
                                                                                                    <div style="width: 75px; display: none;" id="divSelCriteria12">
                                                                                                        <dx:ASPxTimeEdit ID="txtCriteria12" runat="server" Width="85px" EditFormatString="HH:mm" EditFormat="Custom" ClientInstanceName="txtCriteria12Client">
                                                                                                            <ClientSideEvents DateChanged="function(s,e){}" />
                                                                                                        </dx:ASPxTimeEdit>
                                                                                                    </div>
                                                                                                </td>
                                                                                                <td></td>
                                                                                            </tr>
                                                                                        </table>
                                                                                    </div>
                                                                                    <div id="divConditionValueUserField" style="display: none;">
                                                                                        <table border="0" cellpadding="0" cellspacing="0">
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <div id="divFromValueUserField" style="display: none;">
                                                                                                        <dx:ASPxComboBox runat="server" ID="cmbRuleFromValueUserFields" Width="175px" ClientInstanceName="cmbRuleFromValueUserFieldsClient">
                                                                                                            <ClientSideEvents SelectedIndexChanged="function(s,e){}" />
                                                                                                        </dx:ASPxComboBox>
                                                                                                    </div>
                                                                                                </td>
                                                                                                <td>
                                                                                                    <div id="divToValueUserField" style="display: none;">
                                                                                                        <dx:ASPxComboBox runat="server" ID="cmbRuleToValueUserFields" Width="175px" ClientInstanceName="cmbRuleToValueUserFieldsClient">
                                                                                                            <ClientSideEvents SelectedIndexChanged="function(s,e){}" />
                                                                                                        </dx:ASPxComboBox>
                                                                                                    </div>
                                                                                                </td>
                                                                                                <td>
                                                                                                    <div id="divBetweenValueUserField" style="display: none;">
                                                                                                        <dx:ASPxComboBox runat="server" ID="cmbRuleBetweenValueUserFields" Width="175px" ClientInstanceName="cmbRuleBetweenValueUserFieldsClient">
                                                                                                            <ClientSideEvents SelectedIndexChanged="function(s,e){}" />
                                                                                                        </dx:ASPxComboBox>
                                                                                                    </div>
                                                                                                </td>
                                                                                            </tr>
                                                                                        </table>
                                                                                    </div>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td colspan="4" style="text-align: left; padding-top: 5px; padding-bottom: 5px;">
                                                                                    <asp:Label ID="lblConsiderCaption" runat="server" Text="Entonces considera como"></asp:Label></td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td></td>
                                                                                <td>
                                                                                    <dx:ASPxComboBox runat="server" ID="cmbRuleCauses2" Width="175px" ClientInstanceName="cmbRuleCauses2Client">
                                                                                        <ClientSideEvents SelectedIndexChanged="function(s,e){}" />
                                                                                    </dx:ASPxComboBox>
                                                                                </td>
                                                                                <td colspan="2">
                                                                                    <dx:ASPxComboBox runat="server" ID="cmbRuleCriteria2" Width="175px" ClientInstanceName="cmbRuleCriteria2Client">
                                                                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ showCriteria2(s.GetSelectedIndex() == 1)}" />
                                                                                    </dx:ASPxComboBox>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td></td>
                                                                                <td colspan="3">
                                                                                    <div id="divSelCriteria2" style="display: none;">
                                                                                        <table border="0" cellpadding="0" cellspacing="0">
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <dx:ASPxComboBox runat="server" ID="cmbRuleActionValueType" Width="175px" ClientInstanceName="cmbRuleActionValueTypeClient">
                                                                                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ showActionValueType(s.GetSelectedIndex()); }" />
                                                                                                    </dx:ASPxComboBox>
                                                                                                </td>
                                                                                                <td>
                                                                                                    <div id="divMaxValueDirect" style="display: none; width: 75px; padding-left: 3px;">
                                                                                                        <dx:ASPxTimeEdit ID="txtCriteria2To" runat="server" Width="85px" EditFormatString="HH:mm" EditFormat="Custom" ClientInstanceName="txtCriteria2ToClient">
                                                                                                            <ClientSideEvents DateChanged="function(s,e){}" />
                                                                                                        </dx:ASPxTimeEdit>
                                                                                                    </div>
                                                                                                    <div id="divMaxValueUserField" style="display: none; padding-left: 3px;">
                                                                                                        <dx:ASPxComboBox runat="server" ID="cmbRuleMaxValueUserFields" Width="175px" ClientInstanceName="cmbRuleMaxValueUserFieldsClient">
                                                                                                            <ClientSideEvents SelectedIndexChanged="function(s,e){}" />
                                                                                                        </dx:ASPxComboBox>
                                                                                                    </div>
                                                                                                </td>
                                                                                            </tr>
                                                                                        </table>
                                                                                    </div>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>

                                                <input type="hidden" id="hdnMustRefresh_PageBase" value="0" runat="server" />
                                                <table border="0" style="width: 100%;">
                                                    <tr>
                                                        <td colspan="3" align="right">
                                                            <table>
                                                                <tr>
                                                                    <td style="width: 110px;" align="right">
                                                                        <dx:ASPxButton ID="btnOk" runat="server" AutoPostBack="False" CausesValidation="False" Text="Aceptar" ToolTip="Aceptar" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                                                            <ClientSideEvents Click="function(s,e){ saveRule(); }" />
                                                                        </dx:ASPxButton>
                                                                    </td>
                                                                    <td style="width: 110px;" align="left">
                                                                        <dx:ASPxButton ID="btnCancel" runat="server" AutoPostBack="False" CausesValidation="False" Text="Cancelar" ToolTip="Cancelar" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                                                            <ClientSideEvents Click="function(s,e){ cancelRule(); }" />
                                                                        </dx:ASPxButton>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <!-- End Div flotant Rules -->

                                <div id="divContentPanels" class="divContentPanelsWithOutMessage">

                                    <div id="ShiftsContent" runat="server" class="contentPanel" style="display: none">
                                        <!-- Panell General -->
                                                                                <!-- Panell General -->
                                        <div id="div00" class="contentPanel" style="display: none" runat="server" name="menuPanel">
                                            <div class="panHeader2">
                                                <span style="">
                                                    <asp:Label runat="server" ID="lblGeneralTitle" Text="General"></asp:Label></span>
                                            </div>
                                            <br />
                                            <!-- La descripción es opcional -->
                                            <div class="panelHeaderContent">
                                                <div class="panelDescriptionImage">
                                                    <img alt="" src="<%=Me.Page.ResolveUrl("../Shifts/Images/Shifts80.png")%>" height="48px" />
                                                </div>
                                                <div class="panelDescriptionText">
                                                    <asp:Label ID="lblShiftsGeneralDescription" runat="server" Text="Datos generales del horario."></asp:Label>
                                                </div>
                                            </div>

                                            <!-- Este div es un formulario -->
                                            <div class="panBottomMargin">
                                                <div class="divRow">
                                                    <div class="divRowDescription">
                                                        <asp:Label ID="lblNameDescription" runat="server" Text="Nombre identificativo del horario"></asp:Label>
                                                    </div>
                                                    <asp:Label ID="lblShiftName" runat="server" Text="Nombre:" CssClass="labelForm"></asp:Label>
                                                    <div class="componentForm">
                                                        <dx:ASPxTextBox ID="txtShiftName" runat="server" MaxLength="50" Width="200px" ClientInstanceName="txtShiftName_Client" NullText="_____">
                                                            <ClientSideEvents Validation="LengthValidation" TextChanged="function(s,e){checkShiftEmptyName(s.GetValue());OnShiftNameChange();}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                            <ValidationSettings SetFocusOnError="True" ValidationGroup="shiftMainInfo">
                                                                <RequiredField IsRequired="True" ErrorText="(*)" />
                                                            </ValidationSettings>
                                                        </dx:ASPxTextBox>
                                                    </div>
                                                </div>

                                                <div class="divRow">
                                                    <div class="divRowDescription">
                                                        <asp:Label ID="lblShiftShortnameDescription" runat="server" Text="Nombre abreviado utilizado como referencia al horario"></asp:Label>
                                                    </div>
                                                    <asp:Label ID="lblShiftShortname" runat="server" Text="Nombre abreviado:" CssClass="labelForm"></asp:Label>
                                                    <div class="componentForm">
                                                        <dx:ASPxTextBox ID="txtShortName" runat="server" MaxLength="3" Width="50" NullText="_____" ClientInstanceName="txtShortName_Client">
                                                            <ClientSideEvents Validation="LengthValidation" TextChanged="function(s,e){ hasChanges(true,false)}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                            <ValidationSettings SetFocusOnError="True" ValidationGroup="shiftMainInfo">
                                                                <RequiredField IsRequired="True" ErrorText="(*)" />
                                                            </ValidationSettings>
                                                        </dx:ASPxTextBox>
                                                    </div>
                                                </div>

                                                <div class="divRow">
                                                    <div class="divRowDescription">
                                                        <asp:Label ID="lblColorHorarioDesc" runat="server" Text="Para el color de fondo del horario"></asp:Label>
                                                    </div>
                                                    <asp:Label ID="lblColor" runat="server" Text="Color identificativo:" class="labelForm"></asp:Label>
                                                    <div class="componentForm">
                                                        <dx:ASPxColorEdit ID="colorShift" runat="server" EnableCustomColors="true" Width="14px">
                                                            <ClientSideEvents ColorChanged="function(s,e){s.GetInputElement().style.display = 'none';hasChanges(true,false);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                        </dx:ASPxColorEdit>
                                                    </div>
                                                </div>

                                                <div class="divRow">
                                                    <div class="divRowDescription">
                                                        <asp:Label ID="lblGroupDesc" runat="server" Text="Grupo al que el horario pertenece"></asp:Label>
                                                    </div>
                                                    <asp:Label ID="lblGroup" runat="server" Text="Grupo:" class="labelForm"></asp:Label>
                                                    <div class="componentForm">
                                                        <dx:ASPxComboBox runat="server" ID="cmbGroup" Width="250px">
                                                            <ClientSideEvents SelectedIndexChanged="function(s,e){hasChanges(true,false);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                        </dx:ASPxComboBox>
                                                    </div>
                                                </div>

                                                <div class="divRow">
                                                    <div class="divRowDescription">
                                                        <asp:Label ID="lblDescription2Desc" runat="server" Text="Descripción"></asp:Label>
                                                    </div>
                                                    <asp:Label ID="lblDescription2" runat="server" Text="Descripción:" class="labelForm"></asp:Label>
                                                    <div class="componentForm">
                                                        <dx:ASPxMemo ID="txtDescription" runat="server" Rows="5" Width="475px">
                                                            <ClientSideEvents TextChanged="function(s,e){hasChanges(true,false);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                        </dx:ASPxMemo>
                                                    </div>
                                                </div>                                                
<div class="divRow">
    <div class="divRowDescription">
        <asp:Label ID="lblTotHorPrevDesc"  runat="server" Text="Total de horas previsto para este horario"></asp:Label>
    </div>
    <asp:Label ID="lblTotHorPrev" runat="server" Text="´Horas teóricas:" class="labelForm"></asp:Label>
    <div class="componentForm">
        <dx:ASPxTimeEdit ID="txtTotHorPrev" runat="server" Width="85px" EditFormatString="HH:mm" EditFormat="Custom" ClientInstanceName="txtTotHorPrevClient" AllowMouseWheel="true">
                <ClientSideEvents DateChanged="function(s,e){hasChanges(true,true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
            </dx:ASPxTimeEdit>
    </div>
</div>
                                                <div class="panHeader2">
    <span style="">
        <asp:Label runat="server" ID="lblTypeTitle" Text="Tipo"></asp:Label></span>
</div>
<br />
<div style="width: auto; padding: 15px;">
    <table border="0" style="width: 100%;">
        <tbody>
            <tr>
                <td width="48" height="48">
                    <a href="javascript:void(0)" id="imgType" onclick="">
                        <img src="Images/Rules.png" style="border: 0pt none;" />
                    </a>
                </td>
                <td align="left" valign="top">
                    <span id="span6" class="spanEmp-Class">
                        <asp:Label ID="lblTypeDesc" runat="server" Text="Especifique el tipo de ${Shift}.$CRLFEl tipo de ${Shift} establece el comportamiento del mismo y sus posibilidades de parametrización."></asp:Label>
                    </span>
                </td>
            </tr>
            <tr >
                <td colspan="2" align="left">
                    <table style="width: 100%; padding-top: 10px;" border="0" width="100%">
                        <tr style="display:none" class="availableShiftType">
                            <td style="padding-left: 25px; padding-top: 15px; padding-right: 25px; width: 100%;">
                                <roUserControls:roOptionPanelClient ID="optVacations" runat="server" TypeOPanel="RadioOption" width="100%" height="Auto" Checked="False" Enabled="True" Border="True" Value="1" CConClick="ShiftTypeChanged();hasChanges(true,true);">
                                    <Title>
                                        <asp:Label ID="lblVacations" runat="server" Text="Vacaciones o permiso"></asp:Label>
                                    </Title>
                                    <Description>
                                        <asp:Label ID="lblVacationsDesc" runat="server" Text="El ${Shift} se considera como festivo en el que no se realizarán ${Punches}. Quedan desactivadas todas las opciones de parametrización salvo nombre, color y nombre abreviado. El ${Shift} se podrá utilizar en el Portal del ${Employee} para realizar solicitudes de vacaciones o permisos."></asp:Label>
                                    </Description>
                                    <Content>

                                        <div class="divRow">
                                            <div class="labelFloat">
                                                <asp:Label ID="lblCauseHoliday" runat="server" Text="Al asignar el horario, se generará la justificación "></asp:Label>
                                            </div>
                                            <div class="componentFloat">
                                                <dx:ASPxComboBox runat="server" ID="cmbCauseHoliday" Width="175px" ClientInstanceName="cmbCauseHolidayClient">
                                                    <ClientSideEvents SelectedIndexChanged="function(s,e){changeHolidayTypeVisibility(); hasChanges(true,true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                    <ValidationSettings ErrorDisplayMode="None" />
                                                </dx:ASPxComboBox>
                                            </div>
                                            <div class="labelFloat">
                                                <asp:Label ID="lblCauseHoliday2" runat="server" Text="con el valor de"></asp:Label>
                                            </div>

                                            <div class="componentFloat">
                                                <dx:ASPxComboBox runat="server" ID="cmbHolidayShiftTypeHours" ClientInstanceName="cmbHolidayShiftTypeHoursClient" Width="175px">
                                                    <ClientSideEvents SelectedIndexChanged="function(s,e){changeHolidayTypeVisibility(); hasChanges(true,true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                    <ValidationSettings ErrorDisplayMode="None" />
                                                </dx:ASPxComboBox>
                                            </div>

                                            <div id="divHolidaySelectHours" style="display: none" class="componentFloat">
                                                <dx:ASPxTimeEdit ID="dtHolidaySelectHours" runat="server" Width="85px" EditFormatString="HH:mm" EditFormat="Custom" ClientInstanceName="dtHolidaySelectHoursClient">
                                                    <ClientSideEvents DateChanged="function(s,e){hasChanges(true,true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                </dx:ASPxTimeEdit>
                                            </div>

                                            <div id="divHolidaySelectTime" style="display: none" class="componentFloat">
                                                <dx:ASPxTextBox ID="txtHolidaySelectHours" runat="server" Width="75px" ClientInstanceName="txtHolidaySelectHoursClient">
                                                    <ClientSideEvents TextChanged="function(s,e){hasChanges(true,true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                    <ValidationSettings ErrorDisplayMode="None" />
                                                    <MaskSettings Mask="<-999..999>.<000..999>" />
                                                </dx:ASPxTextBox>
                                            </div>
                                        </div>

                                        <div class="divRow">
                                            <div class="labelFloat">
                                                <asp:Label ID="lblrequestHoliday" runat="server" Text="Al aprobar una solicitud de vacaciones o permisos, se aplicará a los días "></asp:Label>
                                            </div>
                                            <div class="componentFloat">
                                                <dx:ASPxComboBox runat="server" ID="cmbRequestDays" Width="175px">
                                                    <ClientSideEvents SelectedIndexChanged="function(s,e){hasChanges(true,false);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                    <ValidationSettings ErrorDisplayMode="None" />
                                                </dx:ASPxComboBox>
                                            </div>
                                            <div class="labelFloat">
                                                <asp:Label ID="lblrequestHoliday2" runat="server" Text="del periodo solicitado."></asp:Label>
                                            </div>
                                        </div>

                                        <div class="divRow">
                                            <div class="labelFloat">
                                                <asp:Label ID="lblConceptBalance" runat="server" Text="Al aprobar una solicitud de vacaciones o permisos, el saldo utilizado como saldo actual será el siguiente: "></asp:Label>
                                            </div>
                                            <div class="componentFloat">
                                                <dx:ASPxComboBox runat="server" ID="cmbConceptBalance" AutoResizeWithContainer="True" Width="350" ClientInstanceName="cmbConceptBalance">
                                                    <ClientSideEvents SelectedIndexChanged="function(s,e){hasChanges(true,false);cmbConceptBalanceChanged();}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                    <ValidationSettings ErrorDisplayMode="None" />
                                                </dx:ASPxComboBox>
                                            </div>
                                        </div>

                                        <div class="divRow">
                                            <div class="labelFloat">
                                                <asp:Label ID="lblDailyFactor" runat="server" Text="Los días solicitados/planificados se contabilizarán con el siguiente factor: "></asp:Label>
                                            </div>
                                            <div class="componentFloat">
                                                <dx:ASPxSpinEdit ID="txtDailyFactor" runat="server" Width="70" DecimalPlaces="2" MaxValue="99" ClientInstanceName="txtDailyFactorClient">
                                                    <ClientSideEvents ValueChanged="function(s,e){ hasChanges(true,false)}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                </dx:ASPxSpinEdit>
                                            </div>
                                        </div>

                                        <div class="divRow">
                                            <div class="labelFloat">
                                                <asp:Label ID="lblFutureHolidays" runat="server" Text="Al realizar una solicitud para el año siguiente, utilizar hasta final de año el siguiente saldo: "></asp:Label>
                                            </div>
                                            <div class="componentFloat">
                                                <dx:ASPxComboBox runat="server" ID="cmbFutureBalance" ClientInstanceName="cmbFutureBalance" AutoResizeWithContainer="True" Width="350">
                                                    <ClientSideEvents SelectedIndexChanged="function(s,e){hasChanges(true,false);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                    <ValidationSettings ErrorDisplayMode="None" />
                                                </dx:ASPxComboBox>
                                            </div>
                                        </div>
                                    </Content>
                                </roUserControls:roOptionPanelClient>
                            </td>
                        </tr>
                        <tr style="display:none" class="availableShiftType">
                            <td style="padding-left: 25px; padding-right: 25px; width: 100%;">
                                <roUserControls:roOptionPanelClient ID="optNormal" runat="server" TypeOPanel="RadioOption" width="100%" height="Auto" Checked="False" Enabled="True" Border="True" Value="0" CConClick="ShiftTypeChanged();hasChanges(true,true);">
                                    <Title>
                                        <asp:Label ID="lblNormal" runat="server" Text="Normal"></asp:Label>
                                    </Title>
                                    <Description>
                                        <asp:Label ID="lblNormalDesc" runat="server" Text="El ${Shift} se considera como ${Shift} laboral. Se podrá emplear en el Portal del ${Employee} para realizar las solicitudes de cambio de ${Shift}."></asp:Label>
                                    </Description>
                                    <Content>
                                    </Content>
                                </roUserControls:roOptionPanelClient>
                            </td>
                        </tr>
                        <tr style="display:none" class="availableShiftType">
                            <td style="padding-left: 25px; padding-right: 25px; width: 100%;">
                                <roUserControls:roOptionPanelClient ID="optPerHours" runat="server" TypeOPanel="RadioOption" width="100%" height="Auto" Checked="False" Enabled="True" Border="True" Value="0" CConClick="ShiftTypeChanged();hasChanges(true,true);">
                                    <Title>
                                        <asp:Label ID="lblPerHours" runat="server" Text="Por Horas"></asp:Label>
                                    </Title>
                                    <Description>
                                        <asp:Label ID="lblPerHoursDesc" runat="server" Text="El ${Shift} se considera como ${Shift} laboral. Se podrá emplear en el Portal del ${Employee} para realizar las solicitudes de cambio de ${Shift}. <br /> Adicionalmente, se podrán definir horas complementarias y el inicio y duración de sus franjas."></asp:Label>
                                    </Description>
                                    <Content>
                                        <table style="display: none">
                                            <tr>
                                                <td>
                                                    <dx:ASPxCheckBox ID="chkAllowComplementary" runat="server" ClientInstanceName="chkAllowComplementaryClient" Checked="false">
                                                        <ClientSideEvents CheckedChanged="function(s, e) { hasChanges(true,false); changeRules(s,e) }" />
                                                    </dx:ASPxCheckBox>
                                                </td>
                                                <td>
                                                    <asp:Label ID="lblAllowComplementary" runat="server" Text="El horario permite realizar horas complementarias"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </Content>
                                </roUserControls:roOptionPanelClient>
                            </td>
                        </tr>
                        <tr style="display:none" class="availableShiftType">
                            <td style="padding-left: 25px; padding-right: 25px; width: 100%;">
                                <div id="divNormalFloating" runat="server">
                                    <roUserControls:roOptionPanelClient ID="optNormalFloating" runat="server" TypeOPanel="RadioOption" width="100%" height="Auto" Checked="False" Enabled="True" Border="True" Value="2" CConClick="ShiftTypeChanged();hasChanges(true,true);">
                                        <Title>
                                            <asp:Label ID="lblNormalFloating" runat="server" Text="Normal Flotante"></asp:Label>
                                        </Title>
                                        <Description>
                                            <asp:Label ID="lblNormalFloatingDesc" runat="server" Text="Tiene las características de un ${Shift} normal y, al planificarlo, permite definir la hora de inicio en que se aplicará para ese día y empleado. En el momento de realizar una solicitud en el Portal del ${Employee} también se deberá indicar la hora de inicio."></asp:Label>
                                        </Description>
                                        <Content>
                                            <table style="padding-top: 10px; padding-left: 10px; width: 460px;">
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lblStartFloating" runat="server" Text="Hora de inicio del ${Shift}: "></asp:Label>&nbsp;&nbsp;
                                                    </td>
                                                    <td style="width: 75px; text-align: right;">
                                                        <dx:ASPxTimeEdit ID="txtStartFloating" runat="server" Width="85px" EditFormatString="HH:mm" EditFormat="Custom" ClientInstanceName="txtStartFloatingClient">
                                                            <ClientSideEvents DateChanged="function(s,e){hasChanges(true,true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                        </dx:ASPxTimeEdit>
                                                    </td>
                                                    <td style="width: 212px;" align="right">
                                                        <dx:ASPxComboBox runat="server" ID="cmbStartFloating" Width="175px">
                                                            <ClientSideEvents SelectedIndexChanged="function(s,e){hasChanges(true,true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                        </dx:ASPxComboBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </Content>
                                    </roUserControls:roOptionPanelClient>
                                </div>
                            </td>
                        </tr>
                        <tr style="display:none" class="unavailableShiftType">
                            <td style="padding-left: 25px; padding-right: 25px; width: 100%;">
                                <roUserControls:roOptionPanelClient ID="optUnique" runat="server" TypeOPanel="RadioOption" width="100%" height="Auto" Checked="False" Enabled="True" Border="True" Value="0" CConClick="ShiftTypeChanged();hasChanges(true,true);">
                                    <Title>
                                        <asp:Label ID="lblUniqueTitle" runat="server" Text="Único"></asp:Label>
                                    </Title>
                                    <Description>
                                        <asp:Label ID="lblUniqueDesc" runat="server" Text="Tiene las carácteristicas de un horario flotante. Al planificarlo, permite definir las horas teóricas que aplican al día. El color y el nombre del horario en el calendario se generan automáticamente en el momento de planificar en funcion de sus propiedades."></asp:Label>
                                    </Description>
                                    <Content>
                                    </Content>
                                </roUserControls:roOptionPanelClient>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </tbody>
    </table>
</div>
<div class="panHeader2">
    <span style="">
        <asp:Label runat="server" ID="lblDefinitionTitle" Text="Franjas"></asp:Label></span>
</div>
<br />
<div style="width: auto; padding: 20px; padding-top: 20px;">
    <table border="0" style="width: 100%;" height="50">
        <tbody>
            <tr>
                <td width="48" height="48">
                    <a href="javascript:void(0)" id="a5" onclick="">
                        <img src="Images/Rules.png" style="border: 0pt none;" />
                    </a>
                </td>
                <td align="left" valign="top">
                    <span id="span1" class="spanEmp-Class">
                        <asp:Label ID="lblDefinitionDesc" runat="server" Text="En esta pantalla se definen detalladamente los tiempos del ${Shift} seleccionado."></asp:Label>
                    </span>
                </td>
            </tr>
            <tr>
                <td colspan="2" align="center">
                    <!-- Boto Editar campos Ausencias previstas-->
                    <table style="margin-bottom: 0pt; margin-top: 0pt; margin-right: 0pt; width: 100%;" border="0" cellpadding="0" cellspacing="0">
                        <tbody>
                            <tr>
                                <td align="left">
                                    <div class="jsGrid" style="width: 974px">
                                        <asp:Label ID="lblLayersGridTitle" runat="server" CssClass="jsGridTitle" Text="Franjas horarias"></asp:Label>
                                        <div id="toolbarDefinition" runat="server" style="float: right">
                                            <div class="jsgridButton">
                                                <div class="btnFlat">
                                                    <a href="javascript: void(0)" id="lnkDefinitionAdd" runat="server" onmouseover="document.getElementById('divFloatMenu').style.display='';" onmouseout="document.getElementById('divFloatMenu').style.display='none';">
                                                        <span class="btnIconAdd"></span>
                                                        <asp:Label ID="lblDefinitionAdd" runat="server" Text="Añadir franja"></asp:Label>
                                                    </a>
                                                    <div id="divFloatMenu" class="floatMenuCompactShift defaultBackgroundColor" style="display: none;" onmouseover="document.getElementById('divFloatMenu').style.display='';" onmouseout="document.getElementById('divFloatMenu').style.display='none';">
                                                        <table border="0" style="margin-left: 10px; margin-right: 10px;">
                                                            <tr>
                                                                <td nowrap="nowrap"><a href="javascript: void(0)" id="lnkAddShiftFlex" class="btnMode" onclick="frmEditShiftFlexible_ShowNew();">
                                                                    <asp:Label ID="lblAddShiftFlex" runat="server" Text="Horas flexibles"></asp:Label></a></td>
                                                            </tr>
                                                            <tr>
                                                                <td nowrap="nowrap"><a href="javascript: void(0)" id="lnkAddShiftRigid" class="btnMode" onclick="frmEditShiftMandatory_ShowNew();">
                                                                    <asp:Label ID="lblAddShiftRigid" runat="server" Text="Horas rígidas"></asp:Label></a></td>
                                                            </tr>
                                                            <tr>
                                                                <td nowrap="nowrap"><a href="javascript: void(0)" id="lnkAddShiftBreak" class="btnMode" onclick="frmEditShiftBreak_ShowNew();">
                                                                    <asp:Label ID="lblAddShiftBreak" runat="server" Text="Descanso"></asp:Label></a></td>
                                                            </tr>
                                                        </table>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="jsgridButton">
                                                <div class="btnFlat">
                                                    <a href="javascript: void(0)" id="lnkDefinitionDel" runat="server" onclick="ShowDelDefinition();">
                                                        <span class="btnIconAdd"></span>
                                                        <asp:Label ID="lblDefinitionDel" runat="server" Text="Eliminar franja"></asp:Label>
                                                    </a>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <roUserControls:roTimeLine ID="tmDefinitions" runat="server">
                                        <Title>
                                            <asp:Label ID="lblTMDefinifions" runat="server" Text="Definición del ${Shift}"></asp:Label>
                                        </Title>
                                    </roUserControls:roTimeLine>
                                    <!-- popup EditShiftFlexible -->
                                    <roForms:frmEditShiftFlexible ID="frmEditShiftFlexible" runat="server" />
                                    <!-- popup EditShiftMandatory -->
                                    <roForms:frmEditShiftMandatory ID="frmEditShiftMandatory" runat="server" />
                                    <!-- popup EditShiftBreak -->
                                    <roForms:frmEditShiftBreak ID="frmEditShiftBreak" runat="server" />
                                </td>
                            </tr>                                                        
                        </tbody>
                    </table>
                </td>
            </tr>
        </tbody>
    </table>
</div>
                                                <div class="panHeader2">
    <span style="">
        <asp:Label ID="lblHeaderVisibility" Text="Visibilidad" runat="server" /></span>
</div>
<br />
<table cellpadding="0" cellspacing="0" style="width: 90%; padding-left: 20px;">
    <tr style="height: 48px">
        <td align="center" width="60px">
            <asp:Image ID="imgVisibility" ImageUrl="Images/Rules.png" runat="server" />
        </td>
        <td align="left" width="100%" style="padding-left: 10px;">
            <asp:Label ID="lblVisibilityDescription" runat="server" Text="Desde este apartado, podrá seleccionar qué ${Employees} podrán acceder al ${Shift} desde el Portal del ${Employee}."></asp:Label>
        </td>
    </tr>
    <tr>
        <td colspan="2" valign="top" style="padding: 0px;">
            <table border="0" style="width: 100%; padding-top: 5px; padding-left: 5px;">
                <tr style="height: 40px;">
                    <td>
                        <asp:Label ID="lblWhoCanSol" runat="server" Text="Quién puede solicitar"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table style="width: 100%;">
                            <tr>
                                <td style="padding-left: 5px;">
                                    <roUserControls:roOptionPanelClient ID="opVisibilityAll" runat="server" TypeOPanel="RadioOption" width="100%" height="Auto" Checked="False" Enabled="True" CConClick="hasChanges(true,false);">
                                        <Title>
                                            <asp:Label ID="lblVisibilityAllTitle" runat="server" Text="Todos"></asp:Label>
                                        </Title>
                                        <Description>
                                            <asp:Label ID="lblVisibilityAllDesc" runat="server" Text="Todos los ${Employees} podrán solicitar este ${Shift}."></asp:Label>
                                        </Description>
                                        <Content>
                                        </Content>
                                    </roUserControls:roOptionPanelClient>
                                </td>
                            </tr>
                            <tr>
                                <td style="padding-left: 5px; padding-top: 5px">
                                    <roUserControls:roOptionPanelClient ID="opVisibilityNobody" runat="server" TypeOPanel="RadioOption" width="100%" height="Auto" Checked="False" Enabled="True" CConClick="hasChanges(true,false);">
                                        <Title>
                                            <asp:Label ID="lblVisibilityNobodyTitle" runat="server" Text="Nadie"></asp:Label>
                                        </Title>
                                        <Description>
                                            <asp:Label ID="lblVisibilityNobodyDesc" runat="server" Text="Ningún ${Employee} podrá solicitar este ${Shift}."></asp:Label>
                                        </Description>
                                        <Content>
                                        </Content>
                                    </roUserControls:roOptionPanelClient>
                                </td>
                            </tr>
                            <tr>
                                <td style="padding-left: 5px; padding-top: 5px">
                                    <roUserControls:roOptionPanelClient ID="opVisibilityCriteria" runat="server" TypeOPanel="RadioOption" width="100%" height="Auto" Checked="False" Enabled="True" CConClick="hasChanges(true,false);checkCriteriaVisibility();">
                                        <Title>
                                            <asp:Label ID="lblVisibilityCriteriaTitle" runat="server" Text="Según criterio"></asp:Label>
                                        </Title>
                                        <Description>
                                            <asp:Label ID="lblVisibilityCriteriaDesc" runat="server" Text="Los ${Employees} que cumplan los siguientes criterios podrán solicitar este ${Shift}."></asp:Label>
                                        </Description>
                                        <Content>
                                            <table id="visibilityCriteriaTable"  border="0" width="100%" style="display:none; padding: 20px; padding-top: 5px;" align="center">
                                                <tr>
                                                    <td id="criteriaCell" align="left" style="padding-left: 12px;">
                                                        <roUserControls:roUserCtlFieldCriteria2 Prefix="ctl00_contentMainBody_ASPxCallbackPanelContenido_opVisibilityCriteria_visibilityCriteria" ID="visibilityCriteria" runat="server" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </Content>
                                    </roUserControls:roOptionPanelClient>
                                </td>
                            </tr>
                                                        <tr>
    <td style="padding-left: 5px; padding-top: 5px">
        <roUserControls:roOptionPanelClient ID="optVisibilityCollectives" runat="server" TypeOPanel="RadioOption" width="100%" height="Auto" Checked="False" Enabled="True" CConClick="hasChanges(true,false);checkCollectivesVisibility();">
            <Title>
                <asp:Label ID="lblCollectivesCriteriaTitle" runat="server" Text="Según colectivo"></asp:Label>
            </Title>
            <Description>
                <asp:Label ID="lblCollectivesCriteriaDesc" runat="server" Text="Los usuarios que pertenezcan a los colectivos seleccionados podrán solicitar este horario."></asp:Label>
            </Description>
            <Content>
                <table border="0" width="100%" style="padding: 20px; padding-top: 5px;" align="center">
                    <tr>
                        <td id="collectivesCell" align="left" style="display: none; padding-left: 12px;">
                            <dx:ASPxTokenBox ID="tbCollectives" ClientInstanceName="tbCollectivesClient" runat="server" Width="600px">
    <ClientSideEvents 
        TextChanged="function(s, e) { hasChanges(true,false); }" 
        ValueChanged="function(s, e) { hasChanges(true,false); }" 
        TokensChanged="function(s, e) { hasChanges(true,false); }" 
        GotFocus="HightlightOnGotFocus" 
        LostFocus="FadeOnLostFocus" />
</dx:ASPxTokenBox>

                        </td>
                    </tr>
                </table>
            </Content>
        </roUserControls:roOptionPanelClient>
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
                                                      

                                        <!-- Panell Tipo -->
                                        <dx:ASPxHiddenField ID="hdnNewShift" runat="server" ClientInstanceName="hdnNewShiftClient"></dx:ASPxHiddenField>                                        
                                        <input type="hidden" id="hdnSelectedGroup" runat="server" value="" />                                                                                                                        
                                        <input type="hidden" id="hdnExistingShortNames" runat="server" value="" />                                                                                                                        

                                        <!-- Panell Reglas Justificacion -->
                                        <div id="div02" class="contentPanel" style="display: none" runat="server" name="menuPanel">
                                            <div class="panHeader2">
                                                <span style="">
                                                    <asp:Label runat="server" ID="lblRulesTitle" Text="Reglas de justificación"></asp:Label></span>
                                            </div>
                                            <br />
                                            <div style="width: auto; padding: 20px; padding-top: 20px;">
                                                <table border="0" style="width: 100%;">
                                                    <tr>
                                                        <td width="48" height="48">
                                                            <img alt="" src="Images/Rules.png" style="border: 0pt none;" />
                                                        </td>
                                                        <td align="left" valign="top">
                                                            <span id="span3" class="spanEmp-Class">
                                                                <asp:Label ID="lblRulesDesc" runat="server" Text="Defina aquí las reglas de justificación del ${Shift}."></asp:Label></span>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="2" align="center">
                                                            <!-- Tags idioma -->
                                                            <input type="hidden" id="headerRule1" value="<%= Me.Language.Translate("gridHeaderRuleName",DefaultScope) %>" />
                                                            <input type="hidden" id="tagEditTitle" value="<%= Me.Language.Translate("tagEditTitle",DefaultScope) %>" />
                                                            <input type="hidden" id="tagRemoveTitle" value="<%= Me.Language.Translate("tagRemoveTitle",DefaultScope) %>" />

                                                            <!-- Grid JS -->
                                                            <div class="jsGrid">
                                                                <asp:Label ID="lblRulesGridTitle" runat="server" CssClass="jsGridTitle" Text="Reglas de justificación"></asp:Label>
                                                                <div id="panTbRules" runat="server" class="jsgridButton">
                                                                    <div class="btnFlat">
                                                                        <a href="javascript: void(0)" id="btnAddRules" runat="server" onclick="AddNewRules();">
                                                                            <span class="btnIconAdd"></span>
                                                                            <asp:Label ID="lblAddAus" runat="server" Text="Añadir puesto"></asp:Label>
                                                                        </a>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div id="gridRules" class="jsGridContent" runat="server">
                                                                <!-- Aqui va el grid de Puestos-->
                                                            </div>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>

                                            <br />
                                            <div class="panHeader2">
                                                <span style="">
                                                    <asp:Label runat="server" ID="lblDailyRules" Text="Reglas diarias"></asp:Label></span>
                                            </div>
                                            <br />
                                            <div style="width: auto; padding: 20px; padding-top: 20px;">
                                                <table border="0" style="width: 100%;">
                                                    <tr>
                                                        <td width="48" height="48">
                                                            <img alt="" src="Images/Rules.png" style="border: 0pt none;" />
                                                        </td>
                                                        <td align="left" valign="top">
                                                            <span class="spanEmp-Class">
                                                                <asp:Label ID="lblDailyRulesDescription" runat="server" Text="Defina aquí las reglas de justificación diarias del ${Shift}."></asp:Label></span>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="2" align="center">
                                                            <!-- Grid JS -->
                                                            <div class="jsGrid">
                                                                <asp:Label ID="lblDailyRuleTitle" runat="server" CssClass="jsGridTitle" Text="Reglas diarias"></asp:Label>
                                                                <div id="panTbDailyRules" runat="server" class="jsgridButton">
                                                                    <div class="btnFlat">
                                                                        <a href="javascript: void(0)" id="btnAddDailyRule" runat="server" onclick="AddDailyRule();">
                                                                            <span class="btnIconAdd"></span>
                                                                            <asp:Label ID="lblAddDailyRule" runat="server" Text="Añadir regla"></asp:Label>
                                                                        </a>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <input type="hidden" id="hdnDailyRuleChanges" value="0" runat="server" />
                                                            <div id="gridDailyRules" class="jsGridContent" runat="server">
                                                                <!-- Aqui va el grid de Puestos-->
                                                            </div>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>

                                            <roForms:frmDailyRule ID="frmDailyRule1" runat="server" />
                                        </div>      
                                                                            <!-- Panell avanzado -->
                                        <div id="div01" class="contentPanel" style="display: none" runat="server" name="menuPanel">
                                            <div class="panHeader2">
    <span style="">
        <asp:Label runat="server" ID="lblProperties" Text="Propiedades"></asp:Label></span>
</div>
                                            </br>
                                            <div class="divRow">
    <div class="divRowDescription">
        <asp:Label ID="lblCObsoleteDesc" runat="server" Text="Indicación de que el horario ya no es vigente"></asp:Label>
    </div>
    <a href="javascript: void(0);" onclick="CheckLinkClick('ctl00_contentMainBody_ASPxCallbackPanelContenido_chkObsolete');">
        <asp:Label ID="lblCObsolete" runat="server" Text="El ${Shift} es obsoleto: " class="labelForm"></asp:Label></a>

    <div class="componentForm">
        <input type="checkbox" runat="server" id="chkObsoleteLastValue" style="display: none" />
        <input type="checkbox" runat="server" id="chkObsolete" onchange="hasChanges(true,false);" />
    </div>
</div>
<br />
<div class="divRow">
    <div class="divRowDescription">
        <asp:Label ID="lblExportDesc" runat="server" Text="Código usado para la exportación de horarios (debe ser único)"></asp:Label>
    </div>
    <asp:Label ID="lblExport" runat="server" Text="Exportación:" class="labelForm"></asp:Label>
    <div class="componentForm">
        <dx:ASPxTextBox ID="txtExport" runat="server" MaxLength="5" Width="50" NullText="_______" ClientInstanceName="txtExportClient">
            <ClientSideEvents Validation="LengthValidation" TextChanged="function(s,e){ hasChanges(true,false)}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
            <ValidationSettings SetFocusOnError="True" ValidationGroup="shiftMainInfo">
                <RequiredField IsRequired="True" ErrorText="(*)" />
            </ValidationSettings>
        </dx:ASPxTextBox>
    </div>
</div>
                                            <br />
                                            
                                            <!-- Panell DailyRecord -->
<!-- Panell DailyRecord -->
<div id="divDR" runat="server" name="menuPanel">
    <input type="hidden" id="hdnCurrentPair" runat="server" value="1" />
    <div class="panHeader2">
        <span style="">
            <asp:Label runat="server" ID="lblDailyRecordTitle" Text="Declaración de la jornada"></asp:Label></span>
    </div>
    <br />
    <div class="panelHeaderContent">
        <div class="panelDescriptionImage">
            <img alt="" src="<%=Me.Page.ResolveUrl("../Shifts/Images/Shifts80.png")%>" height="48px" />
        </div>
        <div class="panelDescriptionText">
            <asp:Label ID="LabelDailyRecordDesc" runat="server" Text="En esta sección se pueden añadir las franjas de la declaración de la jornada para que aparezca a modo de sugerencia en el portal del empleado."></asp:Label>
        </div>
    </div>
    <div style="width: auto; padding: 25px; padding-top: 0; padding-left: 35px;" class="divRow">
        <div class="dailyRecord_container">
            <div class="dailyRecord_pair" style="margin-right: 38px;">
                <asp:Label ID="LabelDailyRecordEntrada" runat="server" Text="Entrada"></asp:Label>
                <asp:Label ID="LabelDailyRecordSalida" runat="server" Text="Salida"></asp:Label>
            </div>
            <div class="dailyRecord_pair" id="drPair1" runat="server">
                <dx:ASPxTimeEdit ID="txtDREntrada1" runat="server" Width="85px" EditFormatString="HH:mm" EditFormat="Custom" ClientInstanceName="txtTotHorPrevClient" AllowMouseWheel="true">
                    <ClientSideEvents DateChanged="function(s,e){hasChanges(true,false);updatePattern();}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                </dx:ASPxTimeEdit>
                <dx:ASPxTimeEdit ID="txtDRSalida1" runat="server" Width="85px" EditFormatString="HH:mm" EditFormat="Custom" ClientInstanceName="txtTotHorPrevClient" AllowMouseWheel="true">
                    <ClientSideEvents DateChanged="function(s,e){hasChanges(true,false);updatePattern();}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                </dx:ASPxTimeEdit>
                <a href="#" class="dx-link dx-link-delete dx-icon-trash dx-link-icon" id="btnRemovePair1" runat="server" onclick="removePair()" title="Eliminar" aria-label="Eliminar"></a>
            </div>
            <div class="dailyRecord_pair" id="drPair2" style="display: none" runat="server">
                <dx:ASPxTimeEdit ID="txtDREntrada2" runat="server" Width="85px" EditFormatString="HH:mm" EditFormat="Custom" ClientInstanceName="txtTotHorPrevClient" AllowMouseWheel="true">
                    <ClientSideEvents DateChanged="function(s,e){hasChanges(true,false);updatePattern();}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                </dx:ASPxTimeEdit>
                <dx:ASPxTimeEdit ID="txtDRSalida2" runat="server" Width="85px" EditFormatString="HH:mm" EditFormat="Custom" ClientInstanceName="txtTotHorPrevClient" AllowMouseWheel="true">
                    <ClientSideEvents DateChanged="function(s,e){hasChanges(true,false);updatePattern();}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                </dx:ASPxTimeEdit>
                <a href="#" class="dx-link dx-link-delete dx-icon-trash dx-link-icon" id="btnRemovePair2" runat="server" onclick="removePair()" title="Eliminar" aria-label="Eliminar"></a>
            </div>
            <div class="dailyRecord_pair" id="drPair3" style="display: none" runat="server">
                <dx:ASPxTimeEdit ID="txtDREntrada3" runat="server" Width="85px" EditFormatString="HH:mm" EditFormat="Custom" ClientInstanceName="txtTotHorPrevClient" AllowMouseWheel="true">
                    <ClientSideEvents DateChanged="function(s,e){hasChanges(true,false);updatePattern();}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                </dx:ASPxTimeEdit>
                <dx:ASPxTimeEdit ID="txtDRSalida3" runat="server" Width="85px" EditFormatString="HH:mm" EditFormat="Custom" ClientInstanceName="txtTotHorPrevClient" AllowMouseWheel="true">
                    <ClientSideEvents DateChanged="function(s,e){hasChanges(true,false);updatePattern();}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                </dx:ASPxTimeEdit>
                <a href="#" class="dx-link dx-link-delete dx-icon-trash dx-link-icon" id="btnRemovePair3" runat="server" onclick="removePair()" title="Eliminar" aria-label="Eliminar"></a>
            </div>
            <div class="dailyRecord_pair" id="drPair4" style="display: none" runat="server">
                <dx:ASPxTimeEdit ID="txtDREntrada4" runat="server" Width="85px" EditFormatString="HH:mm" EditFormat="Custom" ClientInstanceName="txtTotHorPrevClient" AllowMouseWheel="true">
                    <ClientSideEvents DateChanged="function(s,e){hasChanges(true,false);updatePattern();}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                </dx:ASPxTimeEdit>
                <dx:ASPxTimeEdit ID="txtDRSalida4" runat="server" Width="85px" EditFormatString="HH:mm" EditFormat="Custom" ClientInstanceName="txtTotHorPrevClient" AllowMouseWheel="true">
                    <ClientSideEvents DateChanged="function(s,e){hasChanges(true,false);updatePattern();}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                </dx:ASPxTimeEdit>
                <a href="#" class="dx-link dx-link-delete dx-icon-trash dx-link-icon" id="btnRemovePair4" runat="server" onclick="removePair()" title="Eliminar" aria-label="Eliminar"></a>
            </div>
            <div class="dailyRecord_pair" id="drPair5" style="display: none" runat="server">
                <dx:ASPxTimeEdit ID="txtDREntrada5" runat="server" Width="85px" EditFormatString="HH:mm" EditFormat="Custom" ClientInstanceName="txtTotHorPrevClient" AllowMouseWheel="true">
                    <ClientSideEvents DateChanged="function(s,e){hasChanges(true,false);updatePattern();}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                </dx:ASPxTimeEdit>
                <dx:ASPxTimeEdit ID="txtDRSalida5" runat="server" Width="85px" EditFormatString="HH:mm" EditFormat="Custom" ClientInstanceName="txtTotHorPrevClient" AllowMouseWheel="true">
                    <ClientSideEvents DateChanged="function(s,e){hasChanges(true,false);updatePattern();}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                </dx:ASPxTimeEdit>
                <a href="#" class="dx-link dx-link-delete dx-icon-trash dx-link-icon" id="btnRemovePair5" runat="server" onclick="removePair()" title="Eliminar" aria-label="Eliminar"></a>
            </div>
            <div class="btnFlat" id="btnAddPair" runat="server">
                <a href="#" id="A3" runat="server" onclick="addNewPair()">
                    <span class="btnIconAdd"></span>
                    <asp:Label ID="lblAddPair" runat="server" Text="Añadir franja"></asp:Label>
                </a>
            </div>
            <div class="dayChanged" id="drDailyChanged" runat="server">
                <asp:Label ID="lblDRDayChanged" runat="server" Text="El patrón incluye un cambio de día"></asp:Label>
            </div>
        </div>
    </div>
</div>
                                            </br>
                                            <!-- Panell Zonas horarias -->
<div>
    <div class="panHeader2">
        <span style="">
            <asp:Label runat="server" ID="lblZonesTitle" Text="Segmentos horarios"></asp:Label></span>
    </div>
    <br />
    <div style="width: auto; padding: 20px; padding-top: 20px;">
        <table border="0" style="width: 100%;" height="50">
            <tbody>
                <tr>
                    <td width="48" height="48">
                        <a href="javascript:void(0)" id="a6" onclick="">
                            <img src="Images/Rules.png" style="border: 0pt none;">
                        </a>
                    </td>
                    <td align="left" valign="top">
                        <span class="spanEmp-Class">
                            <asp:Label ID="lblZonesDesc" runat="server" Text="Desde aqui puede definir las distintos segmentos horarios"></asp:Label>
                        </span>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="center">
                        <!-- Boto Editar campos Ausencias previstas-->
                        <table style="margin-bottom: 0pt; margin-top: 0pt; margin-right: 0pt; width: 100%;" border="0" cellpadding="0" cellspacing="0">
                            <tbody>
                                <tr>
                                    <td align="left">
                                        <div class="jsGrid" style="width: 974px">
                                            <asp:Label ID="lblZonesGridTitle" runat="server" CssClass="jsGridTitle" Text="Segmentos horarios"></asp:Label>
                                            <div id="toolbarZones" runat="server" style="float: right">
                                                <div class="jsgridButton">
                                                    <div class="btnFlat">
                                                        <a href="javascript: void(0)" id="lnkZonesAdd" runat="server" onclick="frmAddZone_ShowNew();">
                                                            <span class="btnIconAdd"></span>
                                                            <asp:Label ID="lblZonesAdd" runat="server" Text="Añadir..."></asp:Label>
                                                        </a>
                                                    </div>
                                                </div>
                                                <div class="jsgridButton">
                                                    <div class="btnFlat">
                                                        <a href="javascript: void(0)" id="lnkZonesDel" runat="server" onclick="ShowDelZones();">
                                                            <span class="btnIconAdd"></span>
                                                            <asp:Label ID="lblZoneDel" runat="server" Text="Quitar"></asp:Label>
                                                        </a>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <roUserControls:roTimeLine ID="tmZones" runat="server">
                                            <Title>
                                                <asp:Label ID="lblTMZones" runat="server" Text="Segmentos horarios"></asp:Label>
                                            </Title>
                                        </roUserControls:roTimeLine>
                                        <!-- popup TabContainerHoras -->
                                        <roForms:frmAddZone ID="frmAddZone1" runat="server" />
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
</div>
                                            </br>
                                            <div>
    <div class="panHeader2">
        <span style="">
            <asp:Label runat="server" ID="lblScheduleCompliance" Text="Cumplimiento horario"></asp:Label></span>
    </div>
                                                <div><table style="margin-bottom: 0pt; margin-top: 0pt; margin-right: 0pt; width: 100%;" border="0" cellpadding="0" cellspacing="0">
    <tbody>
                                                    <tr>
    <td align="left">
        <div style="width: 650px; margin-top: 15px;">            
                    <table id="scheduleComplianceTable" style="margin: 5px;" border="0">                                                                                                
                        <tr style="height:45px;">
                            <td style="height: 12px;">
                                <table border="0" style="height: 12px;">
                                    <tr class="complianceRow">
                                        <td style="width:24px;">
                                            <input type="checkbox" id="chkTotTimeNoLleg" runat="server" onclick="hasChanges(true, true);" />&nbsp;&nbsp;</td>
                                        <td>
                                            <asp:Label ID="lblTotTimeNoLleg1" runat="server" Text="Si el tiempo no llega a"></asp:Label>&nbsp;</td>
                                        <td style="width: 75px; height: 22px;padding-right:4px;">
                                            <dx:ASPxTimeEdit ID="txtTotTimeNoLleg1" NullText="00:00" runat="server" Width="85px" EditFormatString="HH:mm" EditFormat="Custom" ClientInstanceName="txtTotTimeNoLleg1Client">
                                                <ClientSideEvents DateChanged="function(s,e){hasChanges(true,true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                            </dx:ASPxTimeEdit>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblTotTimeNoLleg2" runat="server" Text="horas, entonces se generará una incidencia"></asp:Label></td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr style="height:45px;">
                            <td>
                                <table border="0" cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td>
                                            <table>
                                                <tr class="complianceRow">
                                                    <td style="width:24px;">
                                                        <input type="checkbox" id="chkTotTimeNoSup" runat="server" onclick="hasChanges(true, true);" />&nbsp;&nbsp;</td>
                                                    <td>
                                                        <asp:Label ID="lblTotTimeNoSup1" runat="server" Text="Si el tiempo supera las"></asp:Label>&nbsp;</td>
                                                    <td style="width: 75px; height: 22px;padding-right:4px;">
                                                        <dx:ASPxTimeEdit ID="txtTotTimeNoSup" NullText="00:00" runat="server" Width="85px" EditFormatString="HH:mm" EditFormat="Custom" ClientInstanceName="txtTotTimeNoSupClient">
                                                            <ClientSideEvents DateChanged="function(s,e){hasChanges(true,true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                        </dx:ASPxTimeEdit>
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="lblTotTimeNoSup2" runat="server" Text="horas, entonces"></asp:Label>&nbsp;
                                                    </td>
                                                    <td>
    <dx:ASPxComboBox runat="server" ID="cmbTotTimeNoSup" Width="220px">
        <ClientSideEvents SelectedIndexChanged="function(s,e){hasChanges(true,true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
    </dx:ASPxComboBox>
</td>
                                                </tr>
                                            </table>
                                        </td>                                        
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr style="height:45px;">
    <td>
        <table border="0" cellpadding="0" cellspacing="0">
            <tr class="complianceRow">
                <td style="width:24px;">
    <input type="checkbox" id="chkGSumRetInf" runat="server" onclick="hasChanges(true, true);" /></td>
                <td><asp:Label ID="lblGSUmRetInf" runat="server" Text="Para sumas de retrasos inferiores a"></asp:Label>&nbsp;</td>
<td style="width: 90px;">
    <dx:ASPxTimeEdit EditFormatString="HH:mm" EditFormat="Custom" ID="txtGSumRetHour" NullText="00:00" runat="server" Width="85px" ClientInstanceName="txtGSumRetHourClient">
        <ClientSideEvents DateChanged="function(s,e){hasChanges(true,true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
    </dx:ASPxTimeEdit>
</td>
<td style="width: 220px; text-align: right;">
    <dx:ASPxComboBox runat="server" ID="cmbGSumRetInf" Width="220px">
        <ClientSideEvents SelectedIndexChanged="function(s,e){hasChanges(true,true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
    </dx:ASPxComboBox>
</td>
            </tr>
        </table>
    </td>
</tr>
                                                <tr style="height:45px;">
    <td>
        <table border="0" cellpadding="0" cellspacing="0">
            <tr class="complianceRow">
                <td style="width:24px;">
    <input type="checkbox" id="chkGSumRetInt" runat="server" onclick="hasChanges(true, true);" />&nbsp;</td>
                <td><asp:Label ID="lblGSUmRetInt" runat="server" Text="Para sumas de interrupciones inferiores a"></asp:Label>&nbsp;</td>
<td style="width: 90px;">
    <dx:ASPxTimeEdit ID="txtGSumretInt" NullText="00:00" runat="server" Width="85px" EditFormatString="HH:mm" EditFormat="Custom" ClientInstanceName="txtGSumretIntClient">
        <ClientSideEvents DateChanged="function(s,e){hasChanges(true,true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
    </dx:ASPxTimeEdit>
</td>
<td style="width: 220px; text-align: right;">
    <dx:ASPxComboBox runat="server" ID="cmbGSumRetInt" Width="220px">
        <ClientSideEvents SelectedIndexChanged="function(s,e){hasChanges(true,true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
    </dx:ASPxComboBox>
</td>
            </tr>
        </table>
    </td>
</tr>
                                                                        <tr style="height:45px;">
    <td>
        <table border="0" cellpadding="0" cellspacing="0">
            <tr class="complianceRow">
                <td style="width:24px;">
    <input type="checkbox" id="chkGSumAntInf" runat="server" onclick="hasChanges(true, true);" />&nbsp;</td>
                <td><asp:Label ID="lblGSumAntInf" runat="server" Text="Para sumas de salidas anticipadas inferiores a"></asp:Label>&nbsp;</td>
<td style="width: 90px;">
    <dx:ASPxTimeEdit ID="txtGSumAntInf" NullText="00:00" runat="server" Width="85px" EditFormatString="HH:mm" EditFormat="Custom" ClientInstanceName="txtGSumAntInfClient">
        <ClientSideEvents DateChanged="function(s,e){hasChanges(true,true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
    </dx:ASPxTimeEdit>
</td>
<td style="width: 220px; text-align: right;">
    <dx:ASPxComboBox runat="server" ID="cmbGSumAntInf" Width="220px">
        <ClientSideEvents Init="adjustColumnWidths" SelectedIndexChanged="function(s,e){hasChanges(true,true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
    </dx:ASPxComboBox>
</td>
            </tr>
        </table>
    </td>
</tr>
                        </tbody>
                    </table>                                    
        </div>
    </td>
</tr>
                                                     </table></div>
                                                </div>
    <br />
                                            <div class="panHeader2">
                                                <span style="">
                                                    <asp:Label runat="server" ID="lblDetectTitle" Text="Detección"></asp:Label></span>
                                            </div>
                                            <br />
                                            <div style="width: auto; padding: 15px;">
                                                <table border="0" style="width: 100%;" height="50">
                                                    <tbody>
                                                        <tr>
                                                            <td width="48" height="48">
                                                                <a href="javascript:void(0)" id="a2" onclick="">
                                                                    <img src="Images/Rules.png" style="border: 0pt none;">
                                                                </a>
                                                            </td>
                                                            <td align="left" valign="top">
                                                                <span id="span4" class="spanEmp-Class">
                                                                    <asp:Label ID="lblDetecDesc" runat="server" Text="Por defecto, VisualTime decide automaticamente qué entradas y salidas pertenecen a cada fecha según la definición del ${Shift}. Sin embargo, en algunos casos es posible que prefiera indicar manualmente los limites de la fecha si la detección automática no se ajusta a las convenciones de su empresa."></asp:Label>
                                                                </span>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="2" align="center">
                                                                <table style="width: 100%; padding-top: 10px;" border="0" width="100%">
                                                                    <tr>
                                                                        <td style="padding-left: 25px; padding-top: 15px; padding-right: 25px; width: 100%;">
                                                                            <roUserControls:roOptionPanelClient ID="optAutoDetect" runat="server" TypeOPanel="RadioOption" width="100%" height="Auto" Checked="False" Enabled="True" Border="True" Value="0" CConClick="hasChanges(true,true);">
                                                                                <Title>
                                                                                    <asp:Label ID="lblAutoDetect" runat="server" Text="Detectar automáticamente"></asp:Label>
                                                                                </Title>
                                                                                <Description>
                                                                                    <asp:Label ID="lblAutoDetectDesc" runat="server" Text="VisualTime detectará automáticamente qué entradas y salidas son de este ${Shift} y cuáles son de las fechas anteriores o posteriores."></asp:Label>
                                                                                </Description>
                                                                                <Content>
                                                                                </Content>
                                                                            </roUserControls:roOptionPanelClient>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td style="padding-left: 25px; padding-right: 25px; width: 100%;">
                                                                            <roUserControls:roOptionPanelClient ID="optManualDetect" runat="server" TypeOPanel="RadioOption" width="100%" height="Auto" Checked="False" Enabled="True" Border="True" Value="1" CConClick="hasChanges(true,true);">
                                                                                <Title>
                                                                                    <asp:Label ID="lblManualDetect" runat="server" Text="Indicar manualmente los límites para este ${Shift}"></asp:Label>
                                                                                </Title>
                                                                                <Description>
                                                                                    <asp:Label ID="lblManualDetectDesc" runat="server" Text="Indique manualmente los límites dentro de los cuales las entradas y salidas se considerarán de este ${Shift}. Las anteriores se considerarán de la fecha anterior, y las posteriores del día siguiente."></asp:Label>
                                                                                </Description>
                                                                                <Content>
                                                                                    <table>
                                                                                        <tr>
                                                                                            <td valign="middle" align="right" style="padding-top: 10px; padding-left: 25px;">
                                                                                                <asp:Label ID="lblESUntil" runat="server" Text="Usar entradas y salidas entre las"></asp:Label></td>
                                                                                            <td>
                                                                                                <table>
                                                                                                    <tr>
                                                                                                        <td style="width: 75px;">
                                                                                                            <dx:ASPxTimeEdit ID="txtStartES" runat="server" Width="85px" EditFormatString="HH:mm" EditFormat="Custom" ClientInstanceName="txtStartESClient">
                                                                                                                <ClientSideEvents DateChanged="function(s,e){hasChanges(true,true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                                                            </dx:ASPxTimeEdit>
                                                                                                        </td>
                                                                                                        <td align="right">
                                                                                                            <dx:ASPxComboBox runat="server" ID="cmbStartES" Width="250px">
                                                                                                                <ClientSideEvents SelectedIndexChanged="function(s,e){hasChanges(true,true)}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                                                            </dx:ASPxComboBox>
                                                                                                        </td>
                                                                                                    </tr>
                                                                                                </table>
                                                                                            </td>
                                                                                        </tr>
                                                                                        <tr>
                                                                                            <td valign="middle" align="right" style="padding-left: 25px;">
                                                                                                <asp:Label ID="lblEndUntil" runat="server" Text="y las"></asp:Label></td>
                                                                                            <td>
                                                                                                <table border="0">
                                                                                                    <tr>
                                                                                                        <td style="width: 75px;">
                                                                                                            <dx:ASPxTimeEdit ID="txtEndES" runat="server" Width="85px" EditFormatString="HH:mm" EditFormat="Custom" ClientInstanceName="txtEndESClient">
                                                                                                                <ClientSideEvents DateChanged="function(s,e){hasChanges(true,true)}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                                                            </dx:ASPxTimeEdit>
                                                                                                        </td>
                                                                                                        <td>
                                                                                                            <dx:ASPxComboBox runat="server" ID="cmbEndES" Width="250px">
                                                                                                                <ClientSideEvents SelectedIndexChanged="function(s,e){hasChanges(true,true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                                                            </dx:ASPxComboBox>
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
                                                            </td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </div>
                                            </br>
                                            <div ID="paramsObsolete" runat="server">
                                            <div class="panHeader2">
    <span style="">
        <asp:Label runat="server" ID="lblParamsTitle" Text="Parametros"></asp:Label></span>
</div>
<br />
<div style="width: auto; padding: 15px; padding-bottom: 25px;">
    <table border="0" style="width: 100%;" height="50">
        <tbody>
            <tr>
                <td width="48" height="48" valign="middle"><a href="javascript:void(0)" id="a4" onclick="">
                    <img src="Images/Rules.png" style="border: 0pt none;"></a></td>
                <td align="left" valign="top">
                    <span id="span5" class="spanEmp-Class">
                        <asp:Label ID="lblParamsDesc" runat="server" Text="Para cada uno de los ${Shifts} puede definirse una serie de parametros avanzados con el fin de utilizarlos en determinados procesos de cálculo dentro de VisualTime."></asp:Label>
                    </span>
                </td>
            </tr>
            <tr>
                <td colspan="2" style="padding: 10px; padding-left: 20px;">
                    <asp:Label ID="lblCaptionParams" runat="server" Text="Parametros"></asp:Label><br />
                    <dx:ASPxMemo ID="txtParams" runat="server" Rows="5" Width="100%">
                        <ClientSideEvents TextChanged="function(s,e){hasChanges(true,false);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                    </dx:ASPxMemo>
                </td>
            </tr>
        </tbody>
    </table>
</div>
                                                </div>
                                        </div>                                       
                                    </div>

                                    
                                    <div id="ShiftGroupContent" runat="server" class="contentPanel" style="display: none">
                                        <!-- PANELL GRUPS GENERAL -->
                                        <div id="div20" class="contentPanel" style="display: none" runat="server" name="menuPanel">
                                            <div class="panHeader2">
                                                <span style="">
                                                    <asp:Label runat="server" ID="lblShiftGroupGeneralTitle" Text="General"></asp:Label>
                                                </span>
                                            </div>
                                            <br />
                                            <table style="width: 99%;">
                                                <tr>
                                                    <td width="70px" align="right" style="padding-right: 5px;">
                                                        <asp:Label ID="lblShiftGroupName" runat="server" Text="Nombre:" class="spanEmp-Class"></asp:Label></td>
                                                    <td>
                                                        <dx:ASPxTextBox ID="txtShiftGroupName" runat="server" MaxLength="50" Width="200px" ClientInstanceName="txtShiftGroupName_Client" NullText="_____">
                                                            <ClientSideEvents Validation="LengthValidation" TextChanged="function(s,e){checkShiftGroupEmptyName(s.GetValue());}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                            <ValidationSettings SetFocusOnError="True">
                                                                <RequiredField IsRequired="True" ErrorText="(*)" />
                                                            </ValidationSettings>
                                                        </dx:ASPxTextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                            <br />
                                            <div class="panHeader2">
                                                <span style="">
                                                    <asp:Label runat="server" ID="lblTitleEmpActShiftGroup" Text="${Shifts} que actualmente estan en el grupo"></asp:Label>
                                                </span>
                                            </div>
                                            <br />
                                            <table style="margin: 10px; width: 99%;">
                                                <tr>
                                                    <td align="center" valign="top">
                                                        <table width="100%" border="0" style="text-align: center;">
                                                            <tr>
                                                                <td width="100%" align="center" valign="top">
                                                                    <table width="100%" border="0" cellpadding="0" cellspacing="0">
                                                                        <tr>
                                                                            <td height="20px" valign="top">
                                                                                <div style="width: 100%; height: auto;">
                                                                                    <div id="divHeaderShift" runat="server" style="width: 100%;">
                                                                                    </div>
                                                                                </div>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td valign="top">
                                                                                <div style="width: 100%; height: 280px; overflow: auto;">
                                                                                    <div id="divGridShift" runat="server" style="width: 100%;">
                                                                                    </div>
                                                                                </div>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>
                                            <div id="divBusinessGroup" runat="server">
                                                <div class="panHeader2">
                                                    <span style="">
                                                        <asp:Label runat="server" ID="lblBusinessGroupDesc" Text="Grupo de Negocio"></asp:Label>
                                                    </span>
                                                </div>
                                                <br />
                                                <table style="margin: 10px; width: 99%;">
                                                    <tr>
                                                        <td colspan="2">
                                                            <asp:Label runat="server" ID="lblBusinessGroupDescPpal" Text="Los grupos de negocio se utilizan para limitar los horarios rápidamente al realizar planificación en la pantalla de calendario."></asp:Label>
                                                            <br />
                                                            <asp:Label runat="server" ID="lblBusinessGroupDescPpal2" Text="Para cada grupo de usuarios podrá indicar qué grupo ó grupos de negocio puede gestionar."></asp:Label>
                                                            <br />
                                                            <br />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding-left: 1px; padding-top: 10px; width: 100px;">
                                                            <asp:Label ID="lblBusinessGroup" runat="server" class="spanEmp-Class" Text="Grupo de Negocio:"></asp:Label>
                                                        </td>
                                                        <td style="padding-left: 1px; padding-top: 10px">
                                                            <dx:ASPxTextBox ID="txtBusinessGroup" runat="server">
                                                                <ClientSideEvents TextChanged="function(s,e){ hasChanges(true,false);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                            </dx:ASPxTextBox>
                                                        </td>
                                                    </tr>
                                                </table>
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
        function resizeShiftTrees() {
            try {
                var ctlPrefix = "ctl00_contentMainBody_roTreesShifts";
                eval(ctlPrefix + "_resizeTrees();");
            }
            catch (e) {
                showError("resizeShiftTrees", e);
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
            resizeShiftTrees();
        }

        if (document.getElementById('<%= noRegs.ClientID %>').value != '') {
            cargaShift('-1');
        }
    </script>
</asp:Content>