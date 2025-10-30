<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.WebUserForms_frmDailyRule" CodeBehind="frmDailyRule.ascx.vb" %>

<%@ Register Src="~/Shifts/WebUserForms/frmDailyActionRule.ascx" TagName="frmDailyActionRule" TagPrefix="roForms" %>
<%@ Register Src="~/Shifts/WebUserForms/frmDailyConditionRule.ascx" TagName="frmDailyConditionRule" TagPrefix="roForms" %>

<div id="<%= Me.ClientID %>_frm" style="position: fixed; *position: absolute; z-index: 9010; display: none; top: 50%; left: 50%; *width: 900px; max-height: 95vh; overflow-y: auto;">
    <div id="<%= Me.ClientID %>_BgS" style="position: absolute; top: 0; left: 0; display: none; z-index: 10009;"></div>

    <div class="bodyPopupExtended" style="width: 1180px;">
        <div style="">
            <div style="width: 100%; height: 100%; background-color: White;" class="bodyPopup">
                <table style="width: 100%; padding-top: 5px;" border="0">
                    <tr>
                        <td>
                            <!-- Este div es el header -->
                            <div class="panBottomMargin">
                                <div class="panHeader2 panBottomMargin">
                                    <span class="panelTitleSpan">
                                        <asp:Label runat="server" ID="lblGeneralTitle" Text="General"></asp:Label>
                                    </span>
                                </div>
                                <!-- La descripción es opcional -->
                                <div class="panelHeaderContent" style="height: 17px;">
                                    <div class="panelDescriptionImage" style="height: inherit;">
                                        <img alt="" src="<%=Me.Page.ResolveUrl("~/LabAgree/Images/LabAgree.png")%>" style="position: absolute" />
                                    </div>
                                    <div class="panelDescriptionText">
                                        <asp:Label ID="lblGeneralDescription" runat="server" Text="Definición de un regla de totales diarios de justificación"></asp:Label>
                                    </div>
                                </div>
                            </div>

                            <div class="panBottomMargin">

                                <div style="width:100%;display:flex">
                                    <div class="divRow" >
                                        <div class="divRowDescription">
                                            <asp:Label ID="lblNameDescription" runat="server" Text="Nombre identificativo de la regla diaria"></asp:Label>
                                        </div>
                                        <asp:Label ID="lblName" runat="server" Text="Nombre:" CssClass="labelForm" style="padding-top:8px;"></asp:Label>
                                        <div class="componentForm">
                                            <dx:ASPxTextBox ID="txtName" runat="server" ClientInstanceName="txtName_Client" Width="100%" NullText="_____" MaxLength="100">
                                                <ClientSideEvents Validation="LengthValidation" TextChanged="" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                <ValidationSettings SetFocusOnError="True" ValidationGroup="rulesInfo">
                                                    <RequiredField IsRequired="True" ErrorText="(*)" />
                                                </ValidationSettings>
                                            </dx:ASPxTextBox>
                                        </div>
                                    </div>

                                    <div class="divRow">
                                        <div class="divRowDescription" style="padding-left: 75px;">
                                            <asp:Label ID="lblDescriptionDesc" runat="server" Text="Descripción de la regla diaria"></asp:Label>
                                        </div>
                                        <asp:Label ID="lblDescription" runat="server" Text="Descripción:" CssClass="labelForm" Style="width: 70px; padding-top: 8px;"></asp:Label>
                                        <div class="componentForm" style="width: calc(100% - 90px);">
                                            <dx:ASPxMemo ID="txtDescription" ClientInstanceName="txtDescription_Client" runat="server" Rows="4" Width="100%" Height="40" c>
                                                <ClientSideEvents TextChanged="function(s,e){ }" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                            </dx:ASPxMemo>
                                        </div>
                                    </div>
                                </div>


                                

                                <div class="divRow">
                                    <div class="divRowDescription">
                                        <asp:Label ID="lblValidateWhenDesc" runat="server" Text="La regla solo tendrá efecto si cumple la siguiente condición"></asp:Label>
                                    </div>
                                    <asp:Label ID="lblValidateWhenTitle" runat="server" Text="Aplicar" CssClass="labelForm" style="padding-top:8px;"></asp:Label>
                                    <div class="componentForm">
                                        <dx:ASPxComboBox ID="cmbApplyWhen" runat="server" Width="250px">
                                            <ClientSideEvents SelectedIndexChanged="function(s,e){ }" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                        </dx:ASPxComboBox>
                                    </div>
                                </div>
                                <div class="divRow">
                                    <div class="divRowDescription">
                                        <asp:Label ID="lblOldShiftBehaviourDesc" runat="server" Text="La regla solo se validará en el caso de que el anterior horario  planificado sea uno de los indicados"></asp:Label>
                                    </div>
                                    <asp:Label ID="lblOldShiftBehaviour" runat="server" Text="Validar si el anterior horario planificado era" CssClass="labelForm"></asp:Label>
                                    <div class="componentForm">
                                        <div style="float:left" >
                                            <dx:ASPxRadioButton ID="rbOldShiftAny" ClientInstanceName="rbOldShiftAnyClient" runat="server" Checked="true" GroupName="rbRepeatGroup" Text="Cualquiera" > 
                                                <ClientSideEvents CheckedChanged="validateOldShiftRadio" />
                                            </dx:ASPxRadioButton> 
                                        </div>
                                        <div style="float:left" >
                                            <dx:ASPxRadioButton ID="rbOldShiftInList"  ClientInstanceName="rbOldShiftInListClient"  runat="server" Checked="true" GroupName="rbRepeatGroup" Text="De la lista" >
                                                <ClientSideEvents CheckedChanged="validateOldShiftRadio" />
                                            </dx:ASPxRadioButton>
                                        </div>
                                        <div style="float:left;padding-left:20px;padding-top:5px" >
                                            <dx:ASPxTokenBox ID="tbOldShiftInList" ClientInstanceName="tbOldShiftInListClient" runat="server" Width="600px">
                                                <ClientSideEvents TextChanged="" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                            </dx:ASPxTokenBox>

                                            <dx:ASPxHiddenField ID="hdnSourceShiftList" runat="server" ClientInstanceName="hdnSourceShiftListClient"></dx:ASPxHiddenField>
                                        </div>
                                    </div>
                                </div>
                                <div class="divRow">
                                    <div class="divRowDescription">
                                        <asp:Label ID="lblScheduleValidationRuleDesc" runat="server" Text="Cumplimiento de la reglas de planificación de descanso entre jornadas"></asp:Label>
                                    </div>
                                    <asp:Label ID="lblScheduleValidationRuleTitle" runat="server" Text="Validar" CssClass="labelForm" style="padding-top:8px;"></asp:Label>
                                    <div class="componentForm" style="display: flex;">
                                        <dx:ASPxComboBox ID="cmbApplyScheduleValidationRule" ClientInstanceName="cmbApplyScheduleValidationRuleClient" runat="server" Width="150px" style="height: fit-content;">
                                            <ClientSideEvents SelectedIndexChanged="function(s,e){ validateTBScheduleValidationRule() }" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                        </dx:ASPxComboBox>
                                        <div style="float:left;padding-left:20px;" >
                                            <dx:ASPxTokenBox ID="tbScheduleValidationRule" ClientInstanceName="tbScheduleValidationRuleClient" runat="server" Width="600px">
                                                <ClientSideEvents TextChanged="" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                            </dx:ASPxTokenBox>
                                            <p id="lblScheduleValidationRuleNotFound" style="color:#2d4155; display:none;"><%= Me.Language.Translate("DailyRule.ScheduleRulesNotFound", Me.DefaultScope) %></p>

                                            <dx:ASPxHiddenField ID="hdnScheduleValidationRuleCount" runat="server" ClientInstanceName="hdnScheduleValidationRuleCountClient"></dx:ASPxHiddenField>

                                        </div>
                                    </div>
                                    
                                </div>
                            </div>

                            <div class="panBottomMargin">

                                <div class="panBottomMargin" style="display: flex;">
                                    <div style="width: 100%;">
                                        <div class="jsGrid">
                                            <asp:Label ID="lblConditionsTitle" runat="server" CssClass="jsGridTitle" Text="Condiciones a validar"></asp:Label>
                                            <div id="panTbConditions" runat="server" class="jsgridButton">
                                                <div id="btnAddCondition" class="btnFlat" style="float: right; padding: 0 !important">
                                                    <a href="javascript: void(0)" runat="server" onclick="AddDailyCondition();">
                                                        <asp:Label ID="lblAddCondition" runat="server" Text="Añadir"></asp:Label>
                                                    </a>
                                                </div>

                                                <div id="btnRemoveCondition" class="btnFlat" style="float: right; padding: 0 !important">
                                                    <a href="javascript: void(0)" onclick="RemoveDailyCondition();">
                                                        <asp:Label ID="lblDeleteCondition" runat="server" Text="Eliminar actual"></asp:Label>
                                                    </a>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div id="collapsableCondition">

                                    <div class="jsGridContent">
                                        <div id="conditionsAccordion">
                                            <div class="conditionTab">
                                                <h3 class="conditionHeader"><span class="headerText"><%= Me.Language.Translate("DailyRule.Condition1", Me.DefaultScope) %></span></h3>
                                                <div class="conditionContent">
                                                    <div id="contentCondition1">
                                                        <roForms:frmDailyConditionRule ID="frmDailyConditionRule1" Instance="0" runat="server" />
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="conditionTab" style="display: none">
                                                <h3 class="conditionHeader"><span class="headerText"><%= Me.Language.Translate("DailyRule.Condition2", Me.DefaultScope) %></span></h3>
                                                <div class="conditionContent">
                                                    <div id="contentCondition2">
                                                        <roForms:frmDailyConditionRule ID="frmDailyConditionRule2" Instance="1" runat="server" />
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="conditionTab" style="display: none">
                                                <h3 class="conditionHeader"><span class="headerText"><%= Me.Language.Translate("DailyRule.Condition3", Me.DefaultScope) %></span></h3>
                                                <div class="conditionContent">
                                                    <div id="contentCondition3">
                                                        <roForms:frmDailyConditionRule ID="frmDailyConditionRule3" Instance="2" runat="server" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="panBottomMargin">
                                <div class="jsGrid">
                                    <asp:Label ID="lblActionsTitle" runat="server" CssClass="jsGridTitle" Text="Acciones a realizar"></asp:Label>
                                    <div id="pantbAction" runat="server" class="jsgridButton">
                                        <div id="btnAddAction" class="btnFlat" style="float: right; padding: 0 !important">
                                            <a href="javascript: void(0)" runat="server" onclick="AddDailyAction();">
                                                <asp:Label ID="lblAddAction" runat="server" Text="Añadir"></asp:Label>
                                            </a>
                                        </div>

                                        <div id="btnRemoveAction" class="btnFlat" style="float: right; padding: 0 !important">
                                            <a href="javascript: void(0)" onclick="RemoveDailyAction();">
                                                <asp:Label ID="lblDeleteAction" runat="server" Text="Eliminar actual"></asp:Label>
                                            </a>
                                        </div>
                                    </div>
                                </div>

                                <div class="jsGridContent" style="min-height: inherit">
                                    <div id="actionsAccordion">

                                        <div id="action1" class="actionTab">
                                            <h3 class="actionHeader"><span class="headerText"><%= Me.Language.Translate("DailyRule.Action1", Me.DefaultScope) %></span></h3>
                                            <div class="actionContent">
                                                <div id="contentAction1">
                                                    <roForms:frmDailyActionRule ID="frmDailyActionRule1" Instance="0" runat="server" />
                                                </div>
                                            </div>
                                        </div>

                                        <div id="action2" class="actionTab" style="display: none">
                                            <h3 class="actionHeader"><span class="headerText"><%= Me.Language.Translate("DailyRule.Action2", Me.DefaultScope) %></span> </h3>
                                            <div class="actionContent">
                                                <div id="contentAction2">
                                                    <roForms:frmDailyActionRule ID="frmDailyActionRule2" Instance="1" runat="server" />
                                                </div>
                                            </div>
                                        </div>

                                        <div id="action3" class="actionTab" style="display: none">
                                            <h3 class="actionHeader"><span class="headerText"><%= Me.Language.Translate("DailyRule.Action3", Me.DefaultScope) %></span></h3>
                                            <div class="actionContent">
                                                <div id="contentAction3">
                                                    <roForms:frmDailyActionRule ID="frmDailyActionRule3" Instance="2" runat="server" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </td>
                    </tr>
                </table>
                <table border="0" style="width: 100%;">
                    <tr>
                        <td>&nbsp;</td>
                        <td style="width: 110px;" align="right">
                            <dx:ASPxButton ID="btnOk" runat="server" AutoPostBack="False" CausesValidation="False" Text="Aceptar" ToolTip="Aceptar" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                <ClientSideEvents Click="function(s,e){ frmDailyRule_Save(); }" />
                            </dx:ASPxButton>
                        </td>
                        <td style="width: 110px;" align="left">
                            <dx:ASPxButton ID="btnCancel" runat="server" AutoPostBack="False" CausesValidation="False" Text="Cancelar" ToolTip="Cancelar" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                <ClientSideEvents Click="function(s,e){ frmDailyRule_Close(); }" />
                            </dx:ASPxButton>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
</div>
<!-- End Div flotant AddZone -->