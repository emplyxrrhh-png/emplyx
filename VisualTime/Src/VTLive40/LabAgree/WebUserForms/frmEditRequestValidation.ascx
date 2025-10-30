<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.WebUserForms_frmEditRequestValidation" CodeBehind="frmEditRequestValidation.ascx.vb" %>

<div id="<%= Me.ClientID %>_frm" style="position: fixed; z-index: 9010; display: none; top: 50%; left: 50%; width: 900px;">
    <div id="<%= Me.ClientID %>_BgS" style="position: absolute; top: 0; left: 0; display: none; z-index: 9009;"></div>
    <div class="bodyPopupExtended">

        <dx:ASPxCallback ID="ASPxRequestValidationCallbackContenido" runat="server" ClientInstanceName="ASPxRequestValidationCallbackContenidoClient" ClientSideEvents-CallbackComplete="ASPxRequestValidationCallbackContenidoClient_CallbackComplete"></dx:ASPxCallback>

        <dx:ASPxCallbackPanel ID="ASPxRequestValidationCallbackPanelContenido" runat="server" Width="100%" Height="100%" ClientInstanceName="ASPxRequestValidationCallbackPanelContenidoClient">
            <SettingsLoadingPanel Enabled="false" />
            <ClientSideEvents EndCallback="ASPxRequestValidationCallbackPanelContenido_EndCallBack" />
            <PanelCollection>
                <dx:PanelContent ID="PanelContent1" runat="server">
                    <div id="divContentPanels" style="padding-right: 20px">
                        <div id="div00" class="contentPanel" runat="server" name="menuPanel">
                            <!-- Este div es el header -->
                            <div class="panBottomMargin">
                                <div class="panHeader2 panBottomMargin">
                                    <span class="panelTitleSpan">
                                        <asp:Label runat="server" ID="lblGeneralTitle" Text="General"></asp:Label>
                                    </span>
                                </div>
                            </div>

                            <!-- Este div es un formulario -->
                            <div class="panBottomMargin">
                                <div class="divRow">
                                    <div class="divRowDescription">
                                        <asp:Label ID="lblNameDescription" runat="server" Text="Nombre identificativo del valor inicial"></asp:Label>
                                    </div>
                                    <asp:Label ID="lblName" runat="server" Text="Nombre:" CssClass="labelForm"></asp:Label>
                                    <div class="componentForm">
                                        <div style="float: left; width: 75%">
                                            <dx:ASPxTextBox ID="txtName" runat="server" ClientInstanceName="txtName_Client" Width="100%" NullText="_____" MaxLength="50">
                                                <ClientSideEvents Validation="LengthValidation" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                <ValidationSettings SetFocusOnError="True">
                                                    <RequiredField IsRequired="True" ErrorText="(*)" />
                                                </ValidationSettings>
                                            </dx:ASPxTextBox>
                                        </div>
                                        <div id="ActiveEnabled" style="float: right">
                                            <dx:ASPxCheckBox runat="server" ID="ckRuleActive" Width="75px" Text="Activa" Checked="true">
                                                <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                            </dx:ASPxCheckBox>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <!-- Este div es un formulario -->
                            <div class="panBottomMargin">
                                <div class="divRow">
                                    <div class="divRowDescription">
                                        <asp:Label ID="lblRequestTypeDesc" runat="server" Text="Tipo de solicitud para la que desea crear una regla de validación"></asp:Label>
                                    </div>
                                    <asp:Label ID="lblRequestType" runat="server" Text="Tipo de solicitud" CssClass="labelForm"></asp:Label>
                                    <div class="componentForm">
                                        <dx:ASPxComboBox runat="server" ID="cmbAvailableRequests" Width="350px" NullText="_____" ClientInstanceName="cmbAvailableRequests_Client">
                                            <ClientSideEvents SelectedIndexChanged="availableRequestChanged" Validation="SelectedItemRequiered" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                            <ValidationSettings SetFocusOnError="True">
                                                <RequiredField IsRequired="True" ErrorText="(*)" />
                                            </ValidationSettings>
                                        </dx:ASPxComboBox>
                                    </div>
                                </div>
                            </div>

                            <!-- Este div es un formulario -->
                            <div class="panBottomMargin">
                                <div class="divRow">
                                    <div class="divRowDescription">
                                        <asp:Label ID="lblRuleTypeDesc" runat="server" Text="Tipo de regla que desea configurar"></asp:Label>
                                    </div>
                                    <asp:Label ID="lblRuleType" runat="server" Text="Tipo de regla" CssClass="labelForm"></asp:Label>
                                    <div class="componentForm">
                                        <dx:ASPxComboBox runat="server" ID="cmbRuleType" ClientInstanceName="cmbRuleType_Client" Width="350px" NullText="_____">
                                            <ClientSideEvents SelectedIndexChanged="loadRuleSpecificConfigurationDiv" Validation="SelectedItemRequiered" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                            <ValidationSettings SetFocusOnError="True">
                                                <RequiredField IsRequired="True" ErrorText="(*)" />
                                            </ValidationSettings>
                                        </dx:ASPxComboBox>
                                    </div>
                                </div>
                            </div>

                            <!-- Este div es un formulario -->
                            <div class="panBottomMargin">
                                <div class="divRow">
                                    <div class="divRowDescription">
                                        <asp:Label ID="lblDescriptionDesc" runat="server" Text="Descripción de la regla de validación"></asp:Label>
                                    </div>
                                    <asp:Label ID="lblDescription" runat="server" Text="Regla de validación:" CssClass="labelForm"></asp:Label>
                                    <div class="componentForm">
                                        <dx:ASPxMemo ID="txtDescription" runat="server" Rows="4" Width="100%" Height="40">
                                            <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                        </dx:ASPxMemo>
                                    </div>
                                </div>
                            </div>

                            <!-- Este div es el header -->
                            <div class="panBottomMargin">
                                <div class="panHeader2 panBottomMargin">
                                    <span class="panelTitleSpan">
                                        <asp:Label runat="server" ID="lblRuleConfiguration" Text="Configuración"></asp:Label>
                                    </span>
                                </div>
                            </div>

                            <div class="panBottomMargin">

                                <div class="divRow">
                                    <div style="width: 100%; margin-left: auto; margin-right: auto; padding-top: 8px;">
                                        <table border="0" cellpadding="0" cellspacing="0">
                                            <tr>
                                                <td>
                                                    <div class="RoundCornerFrame">
                                                        <table>
                                                            <tr>
                                                                <td>
                                                                    <a id="TABBUTTON_LARV01" href="javascript: void(0);" runat="server" class="bTab-active" onclick="javascript: changeLabAgreeRequestValidationTabs(0);">
                                                                        <%=Me.Language.Translate("lblConditionTitle", Me.DefaultScope)%></a>
                                                                </td>
                                                                <td>
                                                                    <a id="TABBUTTON_LARV00" href="javascript: void(0);" runat="server" class="bTab" onclick="javascript: changeLabAgreeRequestValidationTabs(1);" style="margin: -1px;">
                                                                        <%=Me.Language.Translate("lblWhenTitle", Me.DefaultScope)%></a>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>

                                    <div id="Content_LARV00" runat="server" style="width: calc(100% - 7px); border: 1px solid #CDCDCD; margin-top: -3px; margin-left: 1px; display: none">

                                        <div class="panBottomMargin">
                                            <div class="divRow">
                                                <div class="divRowDescription">
                                                    <asp:Label ID="lblWhenDescription" runat="server" Text="Indique cuándo se deben realizar las validaciones sobre la solicitud"></asp:Label>
                                                </div>
                                                <asp:Label ID="lblWhen" runat="server" Text="Cuándo:" CssClass="labelForm"></asp:Label>
                                                <div class="componentForm">
                                                    <div id="divRuleWhen">
                                                        <dx:ASPxRadioButton ID="ckCheckOnEmployeeRequest" runat="server" Text="Al realizar la solicitud por parte del empleado" GroupName="ckApplyWhen" Checked="true">
                                                            <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                        </dx:ASPxRadioButton>
                                                        <dx:ASPxRadioButton ID="ckCheckOnSupervisorApprove" runat="server" Text="Al aprobar la solicitud por parte del supervisor" GroupName="ckApplyWhen" Checked="false">
                                                            <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                        </dx:ASPxRadioButton>
                                                    </div>
                                                    <div id="divRuleWhen_4" style="display: none">
                                                        <div class="divRow">
                                                            <asp:Label ID="lblWhenAutomatic" runat="server" Text="Cuando se cree una solicitud del tipo indicado y el motivo esté incluido en la configuración de la regla" CssClass=""></asp:Label>
                                                        </div>
                                                    </div>
                                                    <div id="divRuleWhen_9" style="display: none">
                                                        <div class="divRow">
                                                            <asp:Label ID="lblWhenRejected" runat="server" Text="Cuando se cree una solicitud del tipo indicado y pasen los días indicados, se rechazará de forma automática." CssClass=""></asp:Label>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="panBottomMargin">
                                            <div id="divActionGlobal" class="divRow">
                                                <div class="divRowDescription">
                                                    <asp:Label ID="lblActionDescription" runat="server" Text="Indique que acción se debe tomar cuando las condiciones se cumplan"></asp:Label>
                                                </div>
                                                <asp:Label ID="lblAction" runat="server" Text="Acción:" CssClass="labelForm"></asp:Label>
                                                <div class="componentForm">
                                                    <div id="divRuleAction">
                                                        <dx:ASPxRadioButton ID="ckActionForbid" runat="server" Text="No permitir realizar la acción" GroupName="ckActionWhen" Checked="true">
                                                            <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                        </dx:ASPxRadioButton>
                                                        <dx:ASPxRadioButton ID="ckActionAsk" runat="server" Text="Avisar y preguntar que desea hacer" GroupName="ckActionWhen" Checked="false">
                                                            <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                        </dx:ASPxRadioButton>
                                                    </div>
                                                    <div id="divRuleAction_4" style="display: none">
                                                        <div class="divRow">
                                                            <asp:Label ID="lblActionAutomatic" runat="server" Text="Se aprobará o denegará la solicitud en función del resto de reglas configuradas para las solicitudes del tipo indicado" CssClass=""></asp:Label>
                                                        </div>

                                                        <div class="divRow">
                                                            <div class="jsGrid">
                                                                <asp:Label ID="lblAutomaticRequestsTitle" runat="server" CssClass="jsGridTitle" Text="Reglas"></asp:Label>
                                                            </div>
                                                            <div id="grdAutomaticRequests" class="jsGridContent" runat="server">
                                                                <!-- Aqui va el grid de Puestos-->
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div id="Content_LARV01" runat="server" style="width: calc(100% - 7px); border: 1px solid #CDCDCD; margin-top: -3px; margin-left: 1px;">
                                        <div class="panBottomMargin">
                                            <div id="ruleType_1" style="display: none">
                                                <div class="panBottomMargin">
                                                    <div class="divRow">
                                                        <div class="divRowDescription">
                                                            <asp:Label ID="lblPositiveAccrualDesc" runat="server" Text="Si el valor del saldo es negativo (Contabilizando los días/horas solicitados)"></asp:Label>
                                                        </div>
                                                        <asp:Label ID="lblPositiveAccrual" runat="server" Text="Al solicitar" CssClass="labelForm"></asp:Label>
                                                        <div class="componentForm">
                                                            <dx:ASPxTokenBox ID="tbPositiveAccrualReason" runat="server" Width="100%" AllowCustomTokens="false">
                                                                <ClientSideEvents TextChanged="" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                            </dx:ASPxTokenBox>
                                                            <%--<dx:ASPxComboBox runat="server" ID="cmbPositiveAccrualReason" Width="250px" NullText="_____">
                                                                <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                            </dx:ASPxComboBox>--%>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div id="ruleType_2">
                                                <div class="panBottomMargin">
                                                    <div class="divRow">
                                                        <div class="divRowDescription">
                                                            <asp:Label ID="lblMaxDaysRequestFromDesc" runat="server" Text="Indique la justificación sobre la cual aplicará la condición"></asp:Label>
                                                        </div>
                                                        <asp:Label ID="lblMaxDaysRequestFrom" runat="server" Text="Al solicitar" CssClass="labelForm"></asp:Label>
                                                        <div class="componentForm">
                                                            <dx:ASPxTokenBox ID="tbMaxDayCause" runat="server" Width="100%" AllowCustomTokens="false">
                                                                <ClientSideEvents TextChanged="" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                            </dx:ASPxTokenBox>
                                                            <%--<dx:ASPxComboBox runat="server" ID="cmbMaxDayCause" Width="250px" NullText="_____">
                                                                <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                            </dx:ASPxComboBox>--%>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="panBottomMargin">
                                                    <div class="divRow">
                                                        <div class="divRowDescription">
                                                            <asp:Label ID="lblMaxDaysDesc" runat="server" Text="Indique el número máximo de días que puede solicitar dentro del periodo"></asp:Label>
                                                        </div>
                                                        <asp:Label ID="lblMaxDaysTitle" runat="server" Text="Días" CssClass="labelForm"></asp:Label>
                                                        <div class="componentForm">
                                                            <div style="float: left">
                                                                <dx:ASPxTextBox ID="txtMaxDaysValue" runat="server" Text="0" Width="30" MaxLength="3">
                                                                    <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                    <ValidationSettings Display="None" />
                                                                    <MaskSettings Mask="<0..999>" IncludeLiterals="None" />
                                                                </dx:ASPxTextBox>
                                                            </div>
                                                            <div style="float: left; padding-left: 5px; margin-top: -1px">
                                                                <dx:ASPxComboBox runat="server" ID="cmbDayType" Width="100" NullText="_____">
                                                                    <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                    <ValidationSettings Display="None" />
                                                                </dx:ASPxComboBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div id="ruleType_3" style="display: none">
                                                <div class="panBottomMargin">
                                                    <div class="divRow">
                                                        <div class="divRowDescription">
                                                            <asp:Label ID="lblAppliPeriodReasonDesc" runat="server" Text="Indique la justificación sobre la cual aplicará la condición"></asp:Label>
                                                        </div>
                                                        <asp:Label ID="lblAppliPeriodReason" runat="server" Text="Al solicitar" CssClass="labelForm"></asp:Label>
                                                        <div class="componentForm">
                                                            <dx:ASPxTokenBox ID="tbAppliPeriodReason" runat="server" Width="100%" AllowCustomTokens="false">
                                                                <ClientSideEvents TextChanged="" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                            </dx:ASPxTokenBox>
                                                            <%--<dx:ASPxComboBox runat="server" ID="cmbAppliPeriodReason" Width="250px" NullText="_____">
                                                                <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                            </dx:ASPxComboBox>--%>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div style="padding-bottom: 10px" class="panBottomMargin">
                                                    <div class="divRow">
                                                        <div class="divRowDescription">
                                                            <asp:Label ID="lblApplyPeriodDesc" runat="server" Text="Indique el periodo de disfrute"></asp:Label>
                                                        </div>
                                                        <asp:Label ID="lblEnjoyPeriod" runat="server" Text="Periodo de disfrute" CssClass="labelForm"></asp:Label>
                                                        <div class="componentForm">
                                                            <div>
                                                                <div style="float: left; padding-left: 20px;">
                                                                    <div>
                                                                        <div style="float: left; width: 40px; line-height: 30px;">
                                                                            <asp:Label ID="lblInitialPeriod" runat="server" Text="Del" CssClass=""></asp:Label>
                                                                        </div>
                                                                        <div style="float: left">
                                                                            <dx:ASPxDateEdit runat="server" ID="dtAllowPeriodInitial" PopupVerticalAlign="WindowCenter" EditFormat="Custom" EditFormatString="dd/MM" Width="80px" AllowNull="false" NullText="_____">
                                                                                <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                            </dx:ASPxDateEdit>
                                                                        </div>
                                                                        <div style="float: left; padding-left: 5px;">
                                                                            <dx:ASPxComboBox runat="server" ID="cmbAllowPeriodYearInitial" Width="175px" NullText="_____">
                                                                                <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                            </dx:ASPxComboBox>
                                                                        </div>
                                                                    </div>
                                                                    <div style="clear: both">
                                                                        <div style="float: left; width: 40px; line-height: 30px;">
                                                                            <asp:Label ID="lblEndPeriod" runat="server" Text="Hasta"></asp:Label>
                                                                        </div>
                                                                        <div style="float: left">
                                                                            <dx:ASPxDateEdit runat="server" ID="dtAllowPeriodEnd" PopupVerticalAlign="WindowCenter" EditFormat="Custom" EditFormatString="dd/MM" Width="80px" AllowNull="false" NullText="_____">
                                                                                <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                            </dx:ASPxDateEdit>
                                                                        </div>
                                                                        <div style="float: left; padding-left: 5px;">
                                                                            <dx:ASPxComboBox runat="server" ID="cmbAllowPeriodYearEnd" Width="175px" NullText="_____">
                                                                                <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                            </dx:ASPxComboBox>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div id="ruleType_4" style="display: none">
                                            <div class="panBottomMargin">
                                                <div class="divRow">
                                                    <div class="divRowDescription">
                                                        <asp:Label ID="lblAutomaticValidationDesc" runat="server" Text="Indica los distintos motivos para los que la solicitud entra en el proceso automático de aprovación"></asp:Label>
                                                    </div>
                                                    <asp:Label ID="lblAutomaticValidation" runat="server" Text="Al solicitar" CssClass="labelForm"></asp:Label>
                                                    <div class="componentForm">
                                                        <dx:ASPxTokenBox ID="tbAutomaticValidationReason" runat="server" Width="100%" AllowCustomTokens="false">
                                                            <ClientSideEvents TextChanged="" ValueChanged="checkActivatedrules" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                        </dx:ASPxTokenBox>
                                                        <%--<dx:ASPxComboBox runat="server" ID="cmbPositiveAccrualReason" Width="250px" NullText="_____">
                                                                <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                            </dx:ASPxComboBox>--%>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="panBottomMargin">
                                                <div class="divRow">
                                                    <div class="divRowDescription">
                                                        <asp:Label ID="lblAutomaticValidationDaysDesc" runat="server" Text="Indique el número de días antes de la fecha efectiva en la que se debe aprobar automáticamente la solicitud"></asp:Label>
                                                    </div>
                                                    <asp:Label ID="lblAutomaticValidationDays" runat="server" Text="Días" CssClass="labelForm"></asp:Label>
                                                    <div class="componentForm">
                                                        <div style="float: left">
                                                            <dx:ASPxTextBox ID="txtAutomaticValidationDays" runat="server" Text="0" Width="30" MaxLength="3">
                                                                <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                <ValidationSettings Display="None" />
                                                                <MaskSettings Mask="<0..356>" IncludeLiterals="None" />
                                                            </dx:ASPxTextBox>
                                                        </div>
                                                        <div style="float: left; padding-left: 5px; margin-top: -1px">
                                                            <dx:ASPxComboBox runat="server" ID="cmbAutomaticDayType" Width="100" NullText="_____">
                                                                <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                <ValidationSettings Display="None" />
                                                            </dx:ASPxComboBox>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div id="ruleType_5" style="display: none">
                                            <div class="panBottomMargin">
                                                <div class="divRow">
                                                    <div class="divRowDescription">
                                                        <asp:Label ID="lblLimitDateRequestedDesc" runat="server" Text="Indique la justificación sobre la cual aplicará la condición"></asp:Label>
                                                    </div>
                                                    <asp:Label ID="lblLimitDateRequested" runat="server" Text="Al solicitar" CssClass="labelForm"></asp:Label>
                                                    <div class="componentForm">
                                                        <dx:ASPxTokenBox ID="tbLimitDateRequested" runat="server" Width="100%" AllowCustomTokens="false">
                                                            <ClientSideEvents TextChanged="" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                        </dx:ASPxTokenBox>
                                                        <%--<dx:ASPxComboBox runat="server" ID="cmbMaxDayCause" Width="250px" NullText="_____">
                                                                <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                            </dx:ASPxComboBox>--%>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="panBottomMargin">
                                                <div class="divRow">
                                                    <div class="divRowDescription">
                                                        <asp:Label ID="lblLimitDateRequestedDaysDesc" runat="server" Text="Indique el periodo máximo en días en el que el empleado podrá realizar la solicitud"></asp:Label>
                                                    </div>
                                                    <asp:Label ID="lblLimitDateRequestedDays" runat="server" Text="Días" CssClass="labelForm"></asp:Label>
                                                    <div class="componentForm">
                                                        <div style="float: left">
                                                            <dx:ASPxTextBox ID="txtLimitDateRequestedValue" runat="server" Text="0" Width="30" MaxLength="3">
                                                                <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                <ValidationSettings Display="None" />
                                                                <MaskSettings Mask="<0..999>" IncludeLiterals="None" />
                                                            </dx:ASPxTextBox>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="panBottomMargin">
                                                <div class="divRow">
                                                    <div class="divRowDescription">
                                                        <asp:Label ID="lblWhoAppliesRuleDesc" runat="server" Text="A que empleados con el convenio debe aplicar la regla"></asp:Label>
                                                    </div>
                                                    <asp:Label ID="lblWhoAppliesRule" runat="server" Text="A quien aplica:" CssClass="labelForm"></asp:Label>
                                                    <div class="componentForm">
                                                        <div style="float: left; clear: both; width: 650px;">
                                                            <roUserControls:roOptionPanelClient ID="opVisibilityAll" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Checked="False" Enabled="True">
                                                                <Title>
                                                                    <asp:Label ID="lblVisibilityAllTitle" runat="server" Text="Todos"></asp:Label>
                                                                </Title>
                                                                <Description>
                                                                    <asp:Label ID="lblVisibilityAllDesc" runat="server" Text="Todos los ${Employees} podrán solicitar este ${Shift}."></asp:Label>
                                                                </Description>
                                                                <Content>
                                                                </Content>
                                                            </roUserControls:roOptionPanelClient>
                                                        </div>
                                                        <div style="float: left; clear: both; width: 650px;">
                                                            <roUserControls:roOptionPanelClient ID="opVisibilityCriteria" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Checked="False" Enabled="True" CConClick="checkCriteriaVisibility();">
                                                                <Title>
                                                                    <asp:Label ID="lblVisibilityCriteriaTitle" runat="server" Text="Según criterio"></asp:Label>
                                                                </Title>
                                                                <Description>
                                                                    <asp:Label ID="lblVisibilityCriteriaDesc" runat="server" Text="Los ${Employees} que cumplan los siguientes criterios podrán solicitar este ${Shift}."></asp:Label>
                                                                </Description>
                                                                <Content>
                                                                    <table border="0" width="100%" style="padding: 20px; padding-top: 5px;" align="center">
                                                                        <tr>
                                                                            <td id="criteriaCell" align="left" style="display: none; padding-left: 12px;">
                                                                                <roUserControls:roUserCtlFieldCriteria2 Prefix="ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditRequestValidation_ASPxRequestValidationCallbackPanelContenido_opVisibilityCriteria_visibilityCriteria" ID="visibilityCriteria" runat="server" />
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </Content>
                                                            </roUserControls:roOptionPanelClient>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div id="ruleType_6" style="display: none">
                                            <div class="panBottomMargin">
                                                <div class="divRow">
                                                    <div class="divRowDescription">
                                                        <asp:Label ID="lblMinDaysRequestFromDesc" runat="server" Text="Indique la justificación sobre la cual aplicará la condición"></asp:Label>
                                                    </div>
                                                    <asp:Label ID="lblMinDaysRequestFrom" runat="server" Text="Al solicitar" CssClass="labelForm"></asp:Label>
                                                    <div class="componentForm">
                                                        <dx:ASPxTokenBox ID="tbMinDayCause" runat="server" Width="100%" AllowCustomTokens="false">
                                                            <ClientSideEvents TextChanged="" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                        </dx:ASPxTokenBox>
                                                        <%--<dx:ASPxComboBox runat="server" ID="cmbMaxDayCause" Width="250px" NullText="_____">
                                                                <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                            </dx:ASPxComboBox>--%>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="panBottomMargin">
                                                <div class="divRow">
                                                    <div class="divRowDescription">
                                                        <asp:Label ID="lblMinDaysDesc" runat="server" Text="Indique el número máximo de días de antelación en que se permite realizar la solicitud"></asp:Label>
                                                    </div>
                                                    <asp:Label ID="lblMinDays" runat="server" Text="Días" CssClass="labelForm"></asp:Label>
                                                    <div class="componentForm">
                                                        <div style="float: left">
                                                            <dx:ASPxTextBox ID="txtMinDaysValue" runat="server" Text="0" Width="30" MaxLength="3">
                                                                <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                <ValidationSettings Display="None" />
                                                                <MaskSettings Mask="<0..999>" IncludeLiterals="None" />
                                                            </dx:ASPxTextBox>
                                                        </div>
                                                        <div style="float: left; padding-left: 5px; margin-top: -1px">
                                                            <dx:ASPxComboBox runat="server" ID="cmbMinDaysType" Width="100" NullText="_____">
                                                                <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                <ValidationSettings Display="None" />
                                                            </dx:ASPxComboBox>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div id="ruleType_7" style="display: none">
                                            <div class="panBottomMargin">
                                                <div class="divRow">
                                                    <div class="divRowDescription">
                                                        <asp:Label ID="lblMaxNotScheduledDaysDesc" runat="server" Text="Se denegará la solicitud si existen días sin planificar al indicar los siguientes motivos:"></asp:Label>
                                                    </div>
                                                    <asp:Label ID="lblMaxNotScheduledDays" runat="server" Text="Al solicitar" CssClass="labelForm"></asp:Label>
                                                    <div class="componentForm">
                                                        <dx:ASPxTokenBox ID="tbMaxNotScheduledDays" runat="server" Width="100%" AllowCustomTokens="false">
                                                            <ClientSideEvents TextChanged="" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                        </dx:ASPxTokenBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div id="ruleType_8" style="display: none">
                                            <div class="panBottomMargin">
                                                <div class="divRow">
                                                    <div class="divRowDescription">
                                                        <asp:Label ID="lblMinCoverageRequieredDesc" runat="server" Text="Se denegará la solicitud si no existe cobertura suficiente al indicar los siguientes motivos"></asp:Label>
                                                    </div>
                                                    <asp:Label ID="lblMinCoverageRequiered" runat="server" Text="Al solicitar" CssClass="labelForm"></asp:Label>
                                                    <div class="componentForm">
                                                        <dx:ASPxTokenBox ID="tbMinCoverageRequiered" runat="server" Width="100%" AllowCustomTokens="false">
                                                            <ClientSideEvents TextChanged="" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                        </dx:ASPxTokenBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div id="ruleType_9" style="display: none">
                                            <div class="panBottomMargin">
                                                <div class="divRow">
                                                    <div class="divRowDescription">
                                                        <asp:Label ID="lblAutomaticRejectionDesc" runat="server" Text="Indica los distintos motivos para los que la solicitud se rechaza automáticamente"></asp:Label>
                                                    </div>
                                                    <asp:Label ID="lblAutomaticRejection" runat="server" Text="Al solicitar" CssClass="labelForm"></asp:Label>
                                                    <div class="componentForm">
                                                        <dx:ASPxTokenBox ID="tbAutomaticRejectionReason" runat="server" Width="100%" AllowCustomTokens="false">
                                                            <ClientSideEvents TextChanged="" ValueChanged="checkActivatedrules" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                        </dx:ASPxTokenBox>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="panBottomMargin">
                                                <div class="divRow">
                                                    <div class="divRowDescription">
                                                        <asp:Label ID="lblAutomaticRejectionDaysDesc" runat="server" Text="Indique el número de días tras los que debe rechazarse de forma automática la solicitud"></asp:Label>
                                                    </div>
                                                    <asp:Label ID="lblAutomaticRejectionDays" runat="server" Text="Días" CssClass="labelForm"></asp:Label>
                                                    <div class="componentForm">
                                                        <div style="float: left">
                                                            <dx:ASPxTextBox ID="txtAutomaticRejectionDays" runat="server" Text="0" Width="30" MaxLength="3">
                                                                <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                <ValidationSettings Display="None" />
                                                                <MaskSettings Mask="<0..365>" IncludeLiterals="None" />
                                                            </dx:ASPxTextBox>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div id="ruleType_10" style="display: none">
                                            <div class="panBottomMargin">
                                                <div class="divRow">
                                                    <div class="divRowDescription">
                                                        <asp:Label ID="lblPlanificationRule" runat="server" Text="Se deberán cumplir las siguientes reglas de planificación:"></asp:Label>
                                                    </div>
                                                    <asp:Label ID="lblPlanificationRuleDesc" runat="server" Text="Indica las distintas reglas de planificación para los que la solicitud se validará" CssClass="labelForm"></asp:Label>
                                                    <div class="componentForm">
                                                        <dx:ASPxTokenBox ID="tbPlanificationRules" runat="server" Width="100%" AllowCustomTokens="false">
                                                            <ClientSideEvents TextChanged="" ValueChanged="onChangePlanificationRules" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                        </dx:ASPxTokenBox>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="panBottomMargin">
                                                <div class="divRow">
                                                    <div class="divRowDescription">
                                                        <asp:Label ID="lblPlanificationRuleShift" runat="server" Text="A que horarios se debe aplicar:"></asp:Label>
                                                    </div>
                                                    <asp:Label ID="lblPlanificationRuleShiftDesc" runat="server" Text="Al solicitar" CssClass="labelForm"></asp:Label>
                                                    <div class="componentForm">
                                                        <dx:ASPxTokenBox ID="tbPlanificationRulesShifts" runat="server" Width="100%" AllowCustomTokens="false">
                                                            <ClientSideEvents TextChanged="" ValueChanged="onChangeShiftPlanificationRules" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                        </dx:ASPxTokenBox>
                                                    </div>
                                                </div>
                                            </div>
                                       </div>

                                        <div id="ruleType_11" style="display: none;">
                                            <div class="panBottomMargin">
                                                <div class="divRow">
                                                    <div class="divRowDescription">
                                                        <asp:Label ID="lblMinConsecutiveDaysRequestFromDesc" runat="server" Text="Indica los distintos motivos para los que la solicitud se validará"></asp:Label>
                                                    </div>
                                                    <asp:Label ID="lblMinConsecutiveDaysRequestFrom" runat="server" Text="Al solicitar" CssClass="labelForm"></asp:Label>
                                                    <div class="componentForm">
                                                        <dx:ASPxTokenBox ID="tbMinConsecutiveDayCause" runat="server" Width="100%" AllowCustomTokens="false">
                                                            <ClientSideEvents TextChanged="" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                        </dx:ASPxTokenBox>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="panBottomMargin">
                                                <div class="divRow">
                                                    <div class="divRowDescription">
                                                        <asp:Label ID="lblMinConsecutiveDaysDesc" runat="server" Text="Indique el número mínimo de días consecutivos que puede solicitar dentro del periodo"></asp:Label>
                                                    </div>
                                                    <asp:Label ID="lblMinConsecutiveDaysTitle" runat="server" Text="Días" CssClass="labelForm"></asp:Label>
                                                    <div class="componentForm">
                                                        <div style="float: left">
                                                            <dx:ASPxTextBox ID="txtMinConsecutiveDaysValue" runat="server" Text="0" Width="30" MaxLength="3">
                                                                <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                <ValidationSettings Display="None" />
                                                                <MaskSettings Mask="<0..999>" IncludeLiterals="None" />
                                                            </dx:ASPxTextBox>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div style="width: 100%; padding-top: 10px">
                            <table border="0" style="width: 100%;">
                                <tr>
                                    <td>&nbsp;</td>
                                    <td style="width: 110px;" align="right">
                                        <dx:ASPxButton ID="btnOk" runat="server" AutoPostBack="False" CausesValidation="False" Text="Aceptar" ToolTip="Aceptar" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                            <ClientSideEvents Click="function(s,e){ frmEditRequestValidation_Save(); }" />
                                        </dx:ASPxButton>
                                    </td>
                                    <td style="width: 110px;" align="left">
                                        <dx:ASPxButton ID="btnCancel" runat="server" AutoPostBack="False" CausesValidation="False" Text="Cancelar" ToolTip="Cancelar" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                            <ClientSideEvents Click="function(s,e){ frmEditRequestValidation_Close(); }" />
                                        </dx:ASPxButton>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>
                </dx:PanelContent>
            </PanelCollection>
        </dx:ASPxCallbackPanel>
    </div>
</div>