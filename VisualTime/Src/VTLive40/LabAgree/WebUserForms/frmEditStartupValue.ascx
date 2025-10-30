<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.WebUserForms_frmEditStartupValue" CodeBehind="frmEditStartupValue.ascx.vb" %>

<input type='hidden' id='hdnDif0' runat="server" value='' />
<input type='hidden' id='hdnDif1' runat="server" value='' />
<input type='hidden' id='hdnDif2' runat="server" value='' />

<div id="<%= Me.ClientID %>_frm" style="position: fixed; z-index: 9010; display: none; top: 50%; left: 50%; width: 1280px;">
    <div id="<%= Me.ClientID %>_BgS" style="position: absolute; top: 0; left: 0; display: none; z-index: 9009;"></div>
    <div class="bodyPopupExtended">

        <dx:ASPxCallbackPanel ID="ASPxStartupValueCallbackPanelContenido" runat="server" Width="100%" Height="100%" ClientInstanceName="ASPxStartupValueCallbackPanelContenidoClient">
            <SettingsLoadingPanel Enabled="false" />
            <ClientSideEvents EndCallback="ASPxFormCallbackPanelContenidoClient_EndCallBack" />
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
                                <!-- La descripción es opcional -->
                                <div class="panelHeaderContent">
                                    <div class="panelDescriptionImage">
                                        <img alt="" src="<%=Me.Page.ResolveUrl("~/LabAgree/Images/LabAgree.png")%>" />
                                    </div>
                                    <div class="panelDescriptionText">
                                        <asp:Label ID="lblGeneralDescription" runat="server" Text="Definición de un valor inicial para convenios"></asp:Label>
                                    </div>
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
                                        <dx:ASPxTextBox ID="txtName" runat="server" ClientInstanceName="txtName_Client" Width="100%" NullText="_____" MaxLength="50">
                                            <ClientSideEvents Validation="LengthValidation" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                            <ValidationSettings SetFocusOnError="True">
                                                <RequiredField IsRequired="True" ErrorText="(*)" />
                                            </ValidationSettings>
                                        </dx:ASPxTextBox>
                                    </div>
                                </div>
                            </div>

                            <!-- Este div es el header -->
                            <div class="panBottomMargin">
                                <div class="panHeader2 panBottomMargin">
                                    <span class="panelTitleSpan">
                                        <asp:Label runat="server" ID="lblStartupValuesDefinitionTitle" Text="Definición"></asp:Label>
                                    </span>
                                </div>
                                <!-- La descripción es opcional -->
                                <div class="panelHeaderContent">
                                    <div class="panelDescriptionImage">
                                        <img alt="" src="<%=Me.Page.ResolveUrl("~/LabAgree/Images/StartupValues.png")%>" />
                                    </div>
                                    <div class="panelDescriptionText">
                                        <asp:Label ID="lblStartupValues" runat="server" Text="Aquí se definen los valores iniciales para los convenios."></asp:Label>
                                    </div>
                                </div>
                            </div>

                            <!-- Este div es un formulario -->
                            <div class="panBottomMargin">
                                <div class="divRow">
                                    <div class="divRowDescription">
                                        <asp:Label ID="lblcmbConceptDescription" runat="server" Text="Saldo utlizado para definir el valor inicial"></asp:Label>
                                    </div>
                                    <asp:Label ID="lblcmbConcept" runat="server" Text="${Concept}" CssClass="labelForm"></asp:Label>
                                    <div class="componentForm">
                                        <dx:ASPxComboBox runat="server" ID="cmbIDConcept" Width="250px" NullText="_____" ClientInstanceName="cmbIDConceptClient">
                                            <ClientSideEvents Validation="SelectedItemRequiered" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" SelectedIndexChanged="cmbConceptChanged" />
                                            <ValidationSettings SetFocusOnError="True">
                                                <RequiredField IsRequired="True" ErrorText="(*)" />
                                            </ValidationSettings>
                                        </dx:ASPxComboBox>
                                    </div>
                                </div>
                            </div>
                            <div class="panBottomMargin">

                                <div class="divRow">

                                    <input type="hidden" id="hdnIDType" runat="server" />
                                    <input type="hidden" id="hdnDefaultQuery" runat="server" />

                                    <div style="width: 100%; margin-left: auto; margin-right: auto; padding-top: 8px;">
                                        <table border="0" cellpadding="0" cellspacing="0">
                                            <tr>
                                                <td>
                                                    <div class="RoundCornerFrame">
                                                        <table>
                                                            <tr>
                                                                <td>
                                                                    <a id="TABBUTTON_LASV00" href="javascript: void(0);" runat="server" class="bTab-active" onclick="javascript: changeLabAgreeStartupTabs(0);" style="margin: -1px;">
                                                                        <%=Me.Language.Translate("tabLabAgreeStartupValueTitle", Me.DefaultScope)%></a>
                                                                </td>
                                                                <td>
                                                                    <a id="TABBUTTON_LASA01" href="javascript: void(0);" runat="server" class="bTab" onclick="javascript: changeLabAgreeStartupTabs(1);">
                                                                        <%=Me.Language.Translate("tabLabAgreeStartupAlertsTitle", Me.DefaultScope)%></a>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>

                                    <div id="Content_LASV00" runat="server" style="width: calc(100% - 7px); margin-top: -3px; margin-left: 1px;">

                                        <div style="float: left; max-width: 60%">
                                            <roUserControls:roOptionPanelClient ID="optInitializaWith" runat="server" TypeOPanel="CheckboxOption" Width="100%" Height="Auto" Checked="False" Enabled="True" style="width: 100%;" CConClick="checkInitializeStatus();">
                                                <Title>
                                                    <asp:Label ID="lblInitializaWith" runat="server" Text="Iniciar con el valor"></asp:Label>
                                                </Title>
                                                <Description></Description>
                                                <Content>
                                                    <div class="divRow">
                                                        <div class="componentFormWithoutSize">
                                                            <div class="panBottomMargin">
                                                                <div style="clear: both">
                                                                    <div style="float: left">
                                                                        <dx:ASPxRadioButton GroupName="gStartValue" Text="Valor del campo de la ficha del empleado" runat="server" ID="rbStartValueUF" ClientInstanceName="rbStartValueUFClient" />
                                                                    </div>
                                                                    <div style="float: left; padding-left: 7px;">
                                                                        <dx:ASPxComboBox runat="server" ID="cmbStartValue" Width="200px" ClientInstanceName="cmbStartValueClient">
                                                                            <ClientSideEvents SelectedIndexChanged="" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                        </dx:ASPxComboBox>
                                                                    </div>
                                                                </div>

                                                                <div style="clear: both">
                                                                    <div style="float: left; padding-top: 5px">
                                                                        <dx:ASPxRadioButton GroupName="gStartValue" Text="Valor fijo" runat="server" ID="rbStartValueFix" ClientInstanceName="rbStartValueFixClient" />
                                                                    </div>
                                                                    <div style="float: left; padding-top: 8px; padding-left: 3px;">
                                                                        <dx:ASPxTextBox runat="server" ID="txtStartValue" Width="200px" ClientInstanceName="txtStartValueClient">
                                                                            <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                            <MaskSettings Mask="<0..9999>.<0..9999>" ErrorText="" />
                                                                            <ValidationSettings ErrorDisplayMode="None" />
                                                                        </dx:ASPxTextBox>
                                                                    </div>
                                                                </div>

                                                                <div style="clear: both">
                                                                    <div style="float: left; padding-top: 5px">
                                                                        <dx:ASPxRadioButton GroupName="gStartValue" ID="rbCalculatedStartupValue" runat="server" Checked="false" Text="Valor calculado" ClientInstanceName="rbCalculatedStartupValueClient" />
                                                                    </div>
                                                                    <div style="clear: both; padding-left: 25px; padding-top: 5px">
                                                                        <div style="clear: both">
                                                                            <div style="float: left; padding-left: 3px;">
                                                                                <dx:ASPxCheckBox Text="En base a días trabajados en el año" runat="server" ID="chkYear" ClientInstanceName="ckYearClient">
                                                                                    <ClientSideEvents CheckedChanged="checkCtlFieldCriteriaVisible" />
                                                                                </dx:ASPxCheckBox>
                                                                            </div>
                                                                            <div style="float: left; padding-left: 3px;">
                                                                                <asp:Label ID="lblOpenStartupValueCalculatedFormula" runat="server" Style="cursor: pointer" Text="[...]" CssClass="editTextFormat" onclick="showLabAgreeStartupValue_Calculated()" />
                                                                            </div>
                                                                        </div>
                                                                        <div style="clear: both">
                                                                            <div style="float: left; padding-left: 3px;">
                                                                                <dx:ASPxCheckBox Text="En base a antigüedad" runat="server" ID="chkAnt" ClientInstanceName="ckAntClient">
                                                                                    <ClientSideEvents CheckedChanged="checkAnt" />
                                                                                </dx:ASPxCheckBox>
                                                                            </div>
                                                                            <div style="float: left; padding-left: 3px;">
                                                                                <asp:Label ID="lblOpenStartupAnt" runat="server" Style="cursor: pointer" Text="[...]" CssClass="editTextFormat" onclick="showStartupValue_Ant()" />
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>

                                                                <div style="clear: both">
                                                                    <div style="float: left; padding-top: 5px; width: 100%">
                                                                        <div>
                                                                            <dx:ASPxCheckBox Text="Excepción al inciar un nuevo contrato" runat="server" Font-Bold="true" ID="ckStartContractException" ClientInstanceName="ckStartContractExceptionClient">
                                                                                <ClientSideEvents CheckedChanged="checkCtlFieldCriteriaVisible" />
                                                                            </dx:ASPxCheckBox>
                                                                        </div>
                                                                        <div style="clear: both; padding-top: 5px">
                                                                            <roUserControls:roUserCtlFieldCriteria2 FieldTypesFilter="0,1" Prefix="ctl00_contentMainBody_ASPxCallbackPanelContenido_frmEditStartupValue_ASPxStartupValueCallbackPanelContenido_optInitializaWith_contractExceptionCriteria" ID="contractExceptionCriteria" runat="server" />
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </Content>
                                            </roUserControls:roOptionPanelClient>
                                        </div>
                                        <div style="float: right; width: 50%">
                                            <div style="padding-top: 30px;">
                                                <roUserControls:roOptionPanelClient ID="optAccrualExpiration" runat="server" TypeOPanel="CheckboxOption" Width="100%" Height="Auto" Checked="False" Enabled="True" style="width: 100%;" CConClick="">
                                                    <Title>
                                                        <asp:Label ID="lblAccrualExpirationTitle" runat="server" Text="Configurar la caducidad del saldo"></asp:Label>
                                                    </Title>
                                                    <Description></Description>
                                                    <Content>
                                                        <div id="divAccrualExpirationContent">
                                                            <div class="divRow">
                                                                <div class="labelFloat">
                                                                    <asp:Label ID="lblAccrualExpiration1" runat="server" Text="Los valores ingresados en este saldo tienen una caducidad de "></asp:Label>
                                                                </div>
                                                                <div class="componentFloat" style="clear: both;">
                                                                    <dx:ASPxTextBox ID="txtExpirationPeriodValue" runat="server" Width="75px" ClientInstanceName="txtExpirationPeriodValueClient">
                                                                        <ClientSideEvents TextChanged="function(s,e){}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                        <ValidationSettings ErrorDisplayMode="None" />
                                                                        <MaskSettings Mask="<0..9999>" />
                                                                    </dx:ASPxTextBox>
                                                                </div>
                                                                <div class="componentFloat" style="margin-top: -1px;">
                                                                    <dx:ASPxComboBox runat="server" ID="cmbExpirationPeriodType" Width="175px" ClientInstanceName="cmbExpirationPeriodTypeClient">
                                                                        <ClientSideEvents SelectedIndexChanged="function(s,e){}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                        <ValidationSettings ErrorDisplayMode="None" />
                                                                    </dx:ASPxComboBox>
                                                                </div>

                                                                <div class="labelFloat" style="clear: both;">
                                                                    <asp:Label ID="lblAccrualExpiration2" runat="server" Text=", al caducar se restan del saldo y se justifican como"></asp:Label>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </Content>
                                                </roUserControls:roOptionPanelClient>
                                            </div>
                                            <div>
                                                <roUserControls:roOptionPanelClient ID="optAccrualEnjoyment" runat="server" TypeOPanel="CheckboxOption" Width="100%" Height="Auto" Checked="False" Enabled="True" style="width: 100%;" CConClick="">
                                                    <Title>
                                                        <asp:Label ID="lblAccrualEnjoymentTitle" runat="server" Text="Configurar el periodo de disfrute"></asp:Label>
                                                    </Title>
                                                    <Description></Description>
                                                    <Content>
                                                        <div id="divAccrualExpirationContent">
                                                            <div class="divRow">
                                                                <div class="labelFloat">
                                                                    <asp:Label ID="lblAccrualEnjoyment1" runat="server" Text="Los valores ingresados en este saldo se pueden disfrutar a partir de "></asp:Label>
                                                                </div>
                                                                <div class="componentFloat" style="clear: both;">
                                                                    <dx:ASPxTextBox ID="txtEnjoymentPeriodValue" runat="server" Width="75px" ClientInstanceName="txtEnjoymentPeriodValueClient">
                                                                        <ClientSideEvents TextChanged="function(s,e){}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                        <ValidationSettings ErrorDisplayMode="None" />
                                                                        <MaskSettings Mask="<0..9999>" />
                                                                    </dx:ASPxTextBox>
                                                                </div>
                                                                <div class="componentFloat" style="margin-top: -1px;">
                                                                    <dx:ASPxComboBox runat="server" ID="cmbEnjoymentPeriodType" Width="175px" ClientInstanceName="cmbEnjoymentPeriodTypeClient">
                                                                        <ClientSideEvents SelectedIndexChanged="function(s,e){}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                        <ValidationSettings ErrorDisplayMode="None" />
                                                                    </dx:ASPxComboBox>
                                                                </div>

                                                                <div class="labelFloat" style="clear: both;">
                                                                    <asp:Label ID="lblAccrualEnjoyment2" runat="server" Text=" posteriores a la fecha de adquisición"></asp:Label>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </Content>
                                                </roUserControls:roOptionPanelClient>
                                            </div>
                                        </div>

                                        <dx:ASPxPopupControl ID="calculatedStartupPopup" runat="server" AllowDragging="False" CloseAction="None" Modal="True"
                                            PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter" ModalBackgroundStyle-Opacity="30" Width="550" Height="750"
                                            ShowHeader="False" ScrollBars="Auto" ShowPageScrollbarWhenModal="True" ClientInstanceName="calculatedStartupPopupClient" PopupAnimationType="None" BackColor="Transparent" ContentStyle-Paddings-Padding="0px" Border-BorderColor="Transparent" ShowShadow="false">
                                            <SettingsLoadingPanel Enabled="false" />
                                            <ContentCollection>
                                                <dx:PopupControlContentControl runat="server">
                                                    <div id="labAgreeStartupCalculatedValue" runat="server" class="bodyPopupExtended">
                                                        <table style="width: 100%" cellspacing="0" class="bodyPopup">
                                                            <tr style="height: 20px;">
                                                                <td>
                                                                    <div class="panHeader2">
                                                                        <span style="">
                                                                            <asp:Label runat="server" ID="lblCalculatedValue" Text="Indique la composición del valor"></asp:Label>
                                                                        </span>
                                                                    </div>
                                                                </td>
                                                            </tr>

                                                            <tr>
                                                                <td>
                                                                    <roUserControls:roGroupBox ID="roBaseHoursGroup" runat="server">
                                                                        <Content>
                                                                            <div class="divRow" style="margin-left: 5px;">
                                                                                <div class="componentFormWithoutSize">
                                                                                    <div class="panBottomMargin">
                                                                                        <div style="clear: both">
                                                                                            <table border="0" cellpadding="0" cellspacing="0" style="width: 100%; padding-bottom: 5px;">
                                                                                                <tbody>
                                                                                                    <tr>
                                                                                                        <td>
                                                                                                            <dx:ASPxLabel runat="server" ID="lblBaseHoursTitle" Text="Valor base total" CssClass="OptionPanelCheckBoxStyle" />
                                                                                                        </td>
                                                                                                    </tr>
                                                                                                    <tr>
                                                                                                        <td>
                                                                                                            <dx:ASPxLabel runat="server" ID="lblBaseHoursDescription" CssClass="OptionPanelDescStyle" Text="Valor base total" Style="width: 100%; padding-left: 0px;" />
                                                                                                        </td>
                                                                                                    </tr>
                                                                                                </tbody>
                                                                                            </table>
                                                                                        </div>
                                                                                    </div>
                                                                                </div>
                                                                            </div>
                                                                            <div class="divRow">
                                                                                <div class="componentFormWithoutSize">
                                                                                    <div class="panBottomMargin">
                                                                                        <div style="clear: both">
                                                                                            <div style="float: left">
                                                                                                <dx:ASPxRadioButton GroupName="gBaseHours" Text="Valor del campo de la ficha del empleado" runat="server" ID="rbBaseHoursUF" ClientInstanceName="rbBaseHoursUFClient" />
                                                                                            </div>
                                                                                            <div style="float: left; padding-left: 7px;">
                                                                                                <dx:ASPxComboBox runat="server" ID="cmbBaseHoursUF" Width="200px" ClientInstanceName="cmbStartValueClient" DropDownRows="3">
                                                                                                    <ClientSideEvents SelectedIndexChanged="" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                                                </dx:ASPxComboBox>
                                                                                            </div>
                                                                                        </div>
                                                                                    </div>
                                                                                </div>
                                                                            </div>

                                                                            <div class="divRow">
                                                                                <div class="componentFormWithoutSize">
                                                                                    <div class="panBottomMargin">
                                                                                        <div style="clear: both">
                                                                                            <div style="float: left; padding-top: 5px">
                                                                                                <dx:ASPxRadioButton GroupName="gBaseHours" Text="Valor fijo" runat="server" ID="rbBaseHoursFix" ClientInstanceName="rbBaseHoursFixClient" />
                                                                                            </div>
                                                                                            <div style="float: left; padding-top: 8px; padding-left: 3px;">
                                                                                                <dx:ASPxTextBox runat="server" ID="txtBaseHoursFix" Width="200px" ClientInstanceName="txtStartValueClient">
                                                                                                    <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                                                    <MaskSettings Mask="<0..9999>.<0..9999>" ErrorText="" />
                                                                                                    <ValidationSettings ErrorDisplayMode="None" />
                                                                                                </dx:ASPxTextBox>
                                                                                            </div>
                                                                                        </div>
                                                                                    </div>
                                                                                </div>
                                                                            </div>
                                                                        </Content>
                                                                    </roUserControls:roGroupBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <roUserControls:roGroupBox ID="roTotalHoursGroup" runat="server">
                                                                        <Content>
                                                                            <div class="divRow" style="margin-left: 5px;">
                                                                                <div class="componentFormWithoutSize">
                                                                                    <div class="panBottomMargin">
                                                                                        <div style="clear: both">
                                                                                            <table border="0" cellpadding="0" cellspacing="0" style="width: 100%; padding-bottom: 5px;">
                                                                                                <tbody>
                                                                                                    <tr>
                                                                                                        <td>
                                                                                                            <dx:ASPxLabel runat="server" ID="lblTotalHoursTitle" Text="Valor teórico total" CssClass="OptionPanelCheckBoxStyle" />
                                                                                                        </td>
                                                                                                    </tr>
                                                                                                    <tr>
                                                                                                        <td>
                                                                                                            <dx:ASPxLabel runat="server" ID="lblTotalHoursDescription" Text="Valor teórico total" CssClass="OptionPanelDescStyle" Style="width: 100%; padding-left: 0px;" />
                                                                                                        </td>
                                                                                                    </tr>
                                                                                                </tbody>
                                                                                            </table>
                                                                                        </div>
                                                                                    </div>
                                                                                </div>
                                                                            </div>

                                                                            <div class="divRow">
                                                                                <div class="componentFormWithoutSize">
                                                                                    <div class="panBottomMargin">
                                                                                        <div style="clear: both">
                                                                                            <div style="float: left">
                                                                                                <dx:ASPxRadioButton GroupName="gTotalHours" Text="Valor del campo de la ficha del empleado" runat="server" ID="rbTotalHoursUF" ClientInstanceName="rbTotalHoursUFClient" />
                                                                                            </div>
                                                                                            <div style="float: left; padding-left: 7px;">
                                                                                                <dx:ASPxComboBox runat="server" ID="cmdTotalHoursUF" Width="200px" ClientInstanceName="cmdTotalHoursUFClient" DropDownRows="3">
                                                                                                    <ClientSideEvents SelectedIndexChanged="" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                                                </dx:ASPxComboBox>
                                                                                            </div>
                                                                                        </div>
                                                                                    </div>
                                                                                </div>
                                                                            </div>

                                                                            <div class="divRow">
                                                                                <div class="componentFormWithoutSize">
                                                                                    <div class="panBottomMargin">
                                                                                        <div style="clear: both">
                                                                                            <div style="float: left; padding-top: 5px">
                                                                                                <dx:ASPxRadioButton GroupName="gTotalHours" Text="Valor fijo" runat="server" ID="rbTotalHoursFix" ClientInstanceName="rbTotalHoursFixClient" />
                                                                                            </div>
                                                                                            <div style="float: left; padding-top: 8px; padding-left: 3px;">
                                                                                                <dx:ASPxTextBox runat="server" ID="txtTotalHoursFix" Width="200px" ClientInstanceName="txtStartValueClient">
                                                                                                    <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                                                    <MaskSettings Mask="<0..9999>.<0..9999>" ErrorText="" />
                                                                                                    <ValidationSettings ErrorDisplayMode="None" />
                                                                                                </dx:ASPxTextBox>
                                                                                            </div>
                                                                                        </div>
                                                                                    </div>
                                                                                </div>
                                                                            </div>
                                                                        </Content>
                                                                    </roUserControls:roGroupBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <roUserControls:roGroupBox ID="roAutomaticAccrual" runat="server">
                                                                        <Content>
                                                                            <div class="divRow" style="margin-left: 5px;">
                                                                                <div class="componentFormWithoutSize">
                                                                                    <div class="panBottomMargin">
                                                                                        <div style="clear: both">
                                                                                            <table border="0" cellpadding="0" cellspacing="0" style="width: 100%; padding-bottom: 5px;">
                                                                                                <tbody>
                                                                                                    <tr>
                                                                                                        <td>
                                                                                                            <dx:ASPxLabel runat="server" ID="lblAutomaticAccrualTitle" Text="Coeficiente de parcialidad" CssClass="OptionPanelCheckBoxStyle" />
                                                                                                        </td>
                                                                                                    </tr>
                                                                                                    <tr>
                                                                                                        <td>
                                                                                                            <dx:ASPxLabel runat="server" ID="lblAutomaticAccrualDesc" Text="Coeficiente de parcialidad" CssClass="OptionPanelDescStyle" Style="width: 100%; padding-left: 0px;" />
                                                                                                        </td>
                                                                                                    </tr>
                                                                                                </tbody>
                                                                                            </table>
                                                                                        </div>
                                                                                    </div>
                                                                                </div>
                                                                            </div>
                                                                            <div class="divRow">
                                                                                <div class="componentFormWithoutSize">
                                                                                    <div class="panBottomMargin">
                                                                                        <div style="clear: both">
                                                                                            <div style="float: left">
                                                                                                <dx:ASPxRadioButton GroupName="gAutomaticAccrual" Text="Valor del campo de la ficha del empleado" runat="server" ID="rbAutomaticAccrualUF" ClientInstanceName="rbAutomaticAccrualUFClient" />
                                                                                            </div>
                                                                                            <div style="float: left; padding-left: 7px;">
                                                                                                <dx:ASPxComboBox runat="server" ID="cmbAutomaticAccrualUF" Width="200px" ClientInstanceName="cmbAutomaticAccrualUFClient" DropDownRows="3">
                                                                                                    <ClientSideEvents SelectedIndexChanged="" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                                                </dx:ASPxComboBox>
                                                                                            </div>
                                                                                        </div>
                                                                                    </div>
                                                                                </div>
                                                                            </div>

                                                                            <div class="divRow">
                                                                                <div class="componentFormWithoutSize">
                                                                                    <div class="panBottomMargin">
                                                                                        <div style="clear: both">
                                                                                            <div style="float: left; padding-top: 5px">
                                                                                                <dx:ASPxRadioButton GroupName="gAutomaticAccrual" Text="Valor fijo" runat="server" ID="rbAutomaticAccrualFix" ClientInstanceName="rbAutomaticAccrualFixClient" />
                                                                                            </div>
                                                                                            <div style="float: left; padding-top: 8px; padding-left: 3px;">
                                                                                                <dx:ASPxTextBox runat="server" ID="txtAutomaticAccrualFix" Width="200px" ClientInstanceName="txtAutomaticAccrualFixClient">
                                                                                                    <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                                                    <MaskSettings Mask="<0..9999>.<0..9999>" ErrorText="" />
                                                                                                    <ValidationSettings ErrorDisplayMode="None" />
                                                                                                </dx:ASPxTextBox>
                                                                                            </div>
                                                                                        </div>
                                                                                    </div>
                                                                                </div>
                                                                            </div>
                                                                        </Content>
                                                                    </roUserControls:roGroupBox>
                                                                </td>
                                                            </tr>

                                                            <tr>
                                                                <td>
                                                                    <roUserControls:roGroupBox ID="roEndCustomPeriod" runat="server">
                                                                        <Content>
                                                                            <div class="divRow" style="margin-left: 5px;">
                                                                                <div class="componentFormWithoutSize">
                                                                                    <div class="panBottomMargin">
                                                                                        <div style="clear: both">
                                                                                            <table border="0" cellpadding="0" cellspacing="0" style="width: 100%; padding-bottom: 5px;">
                                                                                                <tbody>
                                                                                                    <tr>
                                                                                                        <td>
                                                                                                            <dx:ASPxLabel runat="server" ID="lblEndCustomPeriodTitle" Text="Fin del periodo" CssClass="OptionPanelCheckBoxStyle" />
                                                                                                        </td>
                                                                                                    </tr>
                                                                                                    <tr>
                                                                                                        <td>
                                                                                                            <dx:ASPxLabel runat="server" ID="lblEndCustomPeriodDesc" Text="Indicar fecha de fin de periodo diferente a la del saldo" CssClass="OptionPanelDescStyle" Style="width: 100%; padding-left: 0px;" />
                                                                                                        </td>
                                                                                                    </tr>
                                                                                                </tbody>
                                                                                            </table>
                                                                                        </div>
                                                                                    </div>
                                                                                </div>
                                                                            </div>
                                                                            <div class="divRow">
                                                                                <div class="componentFormWithoutSize">
                                                                                    <div class="panBottomMargin">
                                                                                        <div style="clear: both">
                                                                                            <div style="float: left">
                                                                                                <dx:ASPxCheckBox Text="Aplicar el valor del campo de la ficha: " runat="server" ID="ckEndCustomPeriod" ClientInstanceName="ckEndCustomPeriodClient">
                                                                                                    <%-- <ClientSideEvents CheckedChanged="checkEndCustomPeriod" />--%>
                                                                                                </dx:ASPxCheckBox>
                                                                                            </div>
                                                                                            <div style="float: left; padding-left: 7px;">
                                                                                                <dx:ASPxComboBox runat="server" ID="cmbEndCustomPeriod" Width="200px" ClientInstanceName="cmbEndCustomPeriodClient" DropDownRows="3">
                                                                                                    <ClientSideEvents SelectedIndexChanged="" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                                                </dx:ASPxComboBox>
                                                                                            </div>
                                                                                        </div>
                                                                                        <br />
                                                                                        <br />
                                                                                        <dx:ASPxLabel CssClass="textClassWithoutBorder" runat="server" ID="lblEndCustomPeriod" Text="con fecha de fin de periodo en el caso que tenga valor. Si no, utilizar por defecto la del propio saldo." Style="width: 100%; padding-left: 0px;" />
                                                                                    </div>
                                                                                </div>
                                                                            </div>
                                                                        </Content>
                                                                    </roUserControls:roGroupBox>
                                                                </td>
                                                            </tr>

                                                            <tr>
                                                                <td>
                                                                    <div class="divRow" style="min-height: 90px;">
                                                                        <div class="componentFormWithoutSize">
                                                                            <div class="panBottomMargin">
                                                                                <div style="clear: both">
                                                                                    <div style="float: left">
                                                                                        <dx:ASPxLabel runat="server" ID="lblRoundValue" Text="Redondear el valor" />
                                                                                    </div>
                                                                                    <div style="float: left; padding-left: 7px;">
                                                                                        <dx:ASPxComboBox runat="server" ID="cmbRoundType" Width="200px" ClientInstanceName="cmbRoundTypeClient" DropDownRows="3">
                                                                                            <ClientSideEvents SelectedIndexChanged="" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                                        </dx:ASPxComboBox>
                                                                                    </div>
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                        <br />
                                                        <table style="margin-left: auto;">
                                                            <tr>
                                                                <td>
                                                                    <dx:ASPxButton ID="btnAcceptCalculatedInitialValue" runat="server" AutoPostBack="False" CausesValidation="False" Text="${Button.Accept}" ToolTip="${Button.Accept}"
                                                                        HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                                                        <ClientSideEvents Click="AcceptCalculatedInitialValueClick" />
                                                                    </dx:ASPxButton>
                                                                </td>
                                                                <td>
                                                                    <dx:ASPxButton ID="btnCancelCalculatedInitialValue" runat="server" AutoPostBack="False" CausesValidation="False" Text="${Button.Cancel}" ToolTip="${Button.Cancel}"
                                                                        HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                                                        <ClientSideEvents Click="CloseCalculatedInitialValueClick" />
                                                                    </dx:ASPxButton>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </div>
                                                </dx:PopupControlContentControl>
                                            </ContentCollection>
                                        </dx:ASPxPopupControl>

                                        <dx:ASPxPopupControl ID="antStartupPopup" runat="server" AllowDragging="False" CloseAction="None" Modal="True"
                                            PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter" ModalBackgroundStyle-Opacity="30" Width="550" Height="800"
                                            ShowHeader="False" ScrollBars="Auto" ShowPageScrollbarWhenModal="True" ClientInstanceName="antStartupPopupClient" PopupAnimationType="None" BackColor="Transparent" ContentStyle-Paddings-Padding="0px" Border-BorderColor="Transparent" ShowShadow="false">
                                            <SettingsLoadingPanel Enabled="false" />
                                            <ContentCollection>
                                                <dx:PopupControlContentControl runat="server">
                                                    <div id="Div1" runat="server" class="bodyPopupExtended">
                                                        <table style="width: 100%" cellspacing="0" class="bodyPopup">
                                                            <tr style="height: 20px;">
                                                                <td>
                                                                    <div class="panHeader2">
                                                                        <span style="">
                                                                            <asp:Label runat="server" ID="lblField" Text="Campo"></asp:Label>
                                                                        </span>
                                                                    </div>
                                                                </td>
                                                            </tr>

                                                            <tr>
                                                                <td>
                                                                    <roUserControls:roGroupBox ID="RoGroupBox1" runat="server">
                                                                        <Content>
                                                                            <div class="divRow" style="margin-left: 5px;">
                                                                                <div class="componentFormWithoutSize">
                                                                                    <div class="panBottomMargin">
                                                                                        <div style="clear: both">
                                                                                            <table border="0" cellpadding="0" cellspacing="0" style="width: 100%; padding-bottom: 5px;">
                                                                                                <tbody>
                                                                                                    <tr>
                                                                                                        <td>
                                                                                                            <dx:ASPxLabel runat="server" ID="lblDesc" Text="Campo de la ficha en función del que se calculará el valor a aplicar:" CssClass="OptionPanelCheckBoxStyle" />
                                                                                                        </td>
                                                                                                    </tr>
                                                                                                </tbody>
                                                                                            </table>
                                                                                        </div>
                                                                                    </div>
                                                                                </div>
                                                                            </div>
                                                                            <div class="divRow">
                                                                                <div class="componentFormWithoutSize">
                                                                                    <div class="panBottomMargin">
                                                                                        <div style="clear: both">
                                                                                            <div style="float: left;">
                                                                                                <dx:ASPxComboBox runat="server" ID="cmbAntUserField" Width="300px" ClientInstanceName="cmbAntUserFieldClient" DropDownRows="3">
                                                                                                    <ClientSideEvents SelectedIndexChanged="" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                                                </dx:ASPxComboBox>
                                                                                            </div>
                                                                                        </div>
                                                                                    </div>
                                                                                </div>
                                                                            </div>
                                                                        </Content>
                                                                    </roUserControls:roGroupBox>
                                                                </td>
                                                            </tr>
                                                            <tr style="height: 20px;">
                                                                <td>
                                                                    <div class="panHeader2">
                                                                        <span style="">
                                                                            <asp:Label runat="server" ID="lblEscalado" Text="Escalado"></asp:Label>
                                                                        </span>
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <roUserControls:roGroupBox ID="RoGroupBox2" runat="server">
                                                                        <Content>
                                                                            <div id="divValuesGrid" runat="server" class="jsGridContent dextremeGrid">
                                                                                <!-- Carrega del Grid Usuari General -->
                                                                            </div>

                                                                            <%-- Jubilacion--%>
                                                                            <div>
                                                                                <%-- <div class="componentFormWithoutSize">--%>
                                                                                <div class="panBottomMargin">
                                                                                    <div style="clear: both">
                                                                                        <div style="float: left">
                                                                                            <dx:ASPxCheckBox Text="Aplicar coeficiente indicado en el campo de la ficha: " runat="server" Font-Bold="true" ID="ckCoe" ClientInstanceName="ckCoeClient">
                                                                                                <ClientSideEvents CheckedChanged="checkCtlFieldCriteriaVisible" />
                                                                                            </dx:ASPxCheckBox>
                                                                                        </div>
                                                                                        <div style="float: left; padding-left: 7px;">
                                                                                            <dx:ASPxComboBox runat="server" ID="cmbCoeField" ClientInstanceName="cmbCoeFieldClient" DropDownRows="3">
                                                                                                <ValidationSettings ErrorDisplayMode="None"></ValidationSettings>
                                                                                                <ClientSideEvents SelectedIndexChanged="" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                                            </dx:ASPxComboBox>
                                                                                        </div>
                                                                                    </div>
                                                                                </div>
                                                                                <%--  </div>--%>
                                                                            </div>
                                                                            <br />
                                                                            <br />
                                                                        </Content>
                                                                    </roUserControls:roGroupBox>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                        <br />
                                                        <table style="margin-left: auto;">
                                                            <tr>
                                                                <td>
                                                                    <dx:ASPxButton ID="ASPxButton1" runat="server" AutoPostBack="False" CausesValidation="False" Text="${Button.Accept}" ToolTip="${Button.Accept}"
                                                                        HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                                                        <ClientSideEvents Click="AcceptAntInitialValueClick" />
                                                                    </dx:ASPxButton>
                                                                </td>
                                                                <td>
                                                                    <dx:ASPxButton ID="ASPxButton2" runat="server" AutoPostBack="False" CausesValidation="False" Text="${Button.Cancel}" ToolTip="${Button.Cancel}"
                                                                        HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                                                        <ClientSideEvents Click="CloseAntInitialValueClick" />
                                                                    </dx:ASPxButton>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </div>
                                                </dx:PopupControlContentControl>
                                            </ContentCollection>
                                        </dx:ASPxPopupControl>

                                        <div style="display: none">
                                            <!-- Popup edición modo inicialización compuesto -->
                                        </div>
                                    </div>

                                    <div id="Content_LASA01" runat="server" style="display: none; width: calc(100% - 7px); margin-top: -3px; margin-left: 1px;">
                                        <roUserControls:roOptionPanelClient ID="optAlertWith" runat="server" TypeOPanel="CheckboxOption" Width="100%" Height="Auto" Checked="False" Enabled="True" style="width: 100%;" CConClick="checkAlertWithStatus();">
                                            <Title>
                                                <asp:Label ID="lblAlertWith" runat="server" Text="Avisar si supera el valor"></asp:Label>
                                            </Title>
                                            <Description></Description>
                                            <Content>
                                                <div class="divRow">
                                                    <div class="componentFormWithoutSize">
                                                        <div class="panBottomMargin">
                                                            <div>
                                                                <div style="float: left">
                                                                    <dx:ASPxRadioButton GroupName="gAlertValue" Text="Valor del campo de la ficha del empleado" runat="server" ID="rbAlertValueUF" ClientInstanceName="rbAlertValueUFClient" />
                                                                </div>
                                                                <div style="float: left; padding-left: 7px;">
                                                                    <dx:ASPxComboBox runat="server" ID="cmbMaximumValue" Width="200px" ClientInstanceName="cmbMaximumValueClient">
                                                                        <ClientSideEvents SelectedIndexChanged="function(s,e){}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                    </dx:ASPxComboBox>
                                                                </div>
                                                            </div>

                                                            <div style="clear: both">
                                                                <div style="float: left; padding-top: 5px">
                                                                    <dx:ASPxRadioButton GroupName="gAlertValue" Text="Valor fijo" runat="server" ID="rbAlertValueFix" ClientInstanceName="rbAlertValueFixClient" />
                                                                </div>
                                                                <div style="float: left; padding-top: 5px; padding-left: 7px;">
                                                                    <dx:ASPxTextBox runat="server" ID="txtMaximumValue" Width="200px" ClientInstanceName="txtMaximumValueClient">
                                                                        <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                        <MaskSettings Mask="<0..9999>.<0..9999>" ErrorText="" />
                                                                        <ValidationSettings ErrorDisplayMode="None" />
                                                                    </dx:ASPxTextBox>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </Content>
                                        </roUserControls:roOptionPanelClient>

                                        <roUserControls:roOptionPanelClient ID="optAlertMin" runat="server" TypeOPanel="CheckboxOption" Width="100%" Height="Auto" Checked="False" Enabled="True" style="width: 100%;" CConClick="checkAlertMinStatus();">
                                            <Title>
                                                <asp:Label ID="lblMin" runat="server" Text="Avisar si el saldo alcanza el valor"></asp:Label>
                                            </Title>
                                            <Description></Description>
                                            <Content>
                                                <div class="divRow">
                                                    <div class="componentFormWithoutSize">
                                                        <div class="panBottomMargin">
                                                            <div>
                                                                <div style="float: left">
                                                                    <dx:ASPxRadioButton GroupName="gMinValue" Text="Valor del campo de la ficha del empleado" runat="server" ID="rbAlertMInValueUF" ClientInstanceName="rbAlertMInValueUFClient" />
                                                                </div>
                                                                <div style="float: left; padding-left: 7px;">
                                                                    <dx:ASPxComboBox runat="server" ID="cmbMinimumValue" Width="200px" ClientInstanceName="cmbMinimumValueClient">
                                                                        <ClientSideEvents SelectedIndexChanged="function(s,e){}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                    </dx:ASPxComboBox>
                                                                </div>
                                                            </div>

                                                            <div style="clear: both">
                                                                <div style="float: left; padding-top: 5px">
                                                                    <dx:ASPxRadioButton GroupName="gMinValue" Text="Valor fijo" runat="server" ID="rbAlertMInValueFix" ClientInstanceName="rbAlertMInValueFixClient" />
                                                                </div>
                                                                <div style="float: left; padding-top: 5px; padding-left: 7px;">
                                                                    <dx:ASPxTextBox runat="server" ID="txtMinimumValue" Width="200px" ClientInstanceName="txtMinimumValueClient">
                                                                        <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                        <MaskSettings Mask="<0..9999>.<0..9999>" ErrorText="" />
                                                                        <ValidationSettings ErrorDisplayMode="None" />
                                                                    </dx:ASPxTextBox>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </Content>
                                        </roUserControls:roOptionPanelClient>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div style="width: 100%;">
                            <table border="0" style="width: 100%;">
                                <tr>
                                    <td>&nbsp;</td>
                                    <td style="width: 110px;" align="right">
                                        <dx:ASPxButton ID="btnOk" runat="server" AutoPostBack="False" CausesValidation="False" Text="Aceptar" ToolTip="Aceptar" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                            <ClientSideEvents Click="function(s,e){ frmEditStartupValue_Save(); }" />
                                        </dx:ASPxButton>
                                    </td>
                                    <td style="width: 110px;" align="left">
                                        <dx:ASPxButton ID="btnCancel" runat="server" AutoPostBack="False" CausesValidation="False" Text="Cancelar" ToolTip="Cancelar" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                            <ClientSideEvents Click="function(s,e){ frmEditStartupValue_Close(); }" />
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
