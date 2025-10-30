<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.WebUserForms_frmEditCauseLimit" CodeBehind="frmEditCauseLimit.ascx.vb" %>

<input type='hidden' id='hdnDif0' runat="server" value='' />
<input type='hidden' id='hdnDif1' runat="server" value='' />
<input type='hidden' id='hdnDif2' runat="server" value='' />

<div id="<%= Me.ClientID %>_frm" style="position: fixed; z-index: 9010; display: none; top: 50%; left: 50%; width: 660px; max-height: 91vh; overflow-y: auto;">
    <div id="<%= Me.ClientID %>_BgS" style="position: absolute; top: 0; left: 0; display: none; z-index: 9009;"></div>
    <div class="bodyPopupExtended">

        <dx:ASPxCallbackPanel ID="ASPxCauseLimitCallbackPanelContenido" runat="server" Width="100%" Height="100%" ClientInstanceName="ASPxCauseLimitCallbackPanelContenidoClient">
            <SettingsLoadingPanel Enabled="false" />
            <ClientSideEvents EndCallback="ASPxCauseLimitCallbackPanelContenido_EndCallBack" />
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
                                        <asp:Label ID="lblNameDescription" runat="server" Text="Nombre identificativo del valor máximo"></asp:Label>
                                    </div>
                                    <asp:Label ID="lblName" runat="server" Text="Nombre:" CssClass="labelForm"></asp:Label>
                                    <div class="componentForm">
                                        <dx:ASPxTextBox ID="txtName" runat="server" ClientInstanceName="txtName_Client" Width="100%" NullText="_____" MaxLength="50">
                                            <ClientSideEvents Validation="LengthValidation" TextChanged="" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                            <ValidationSettings SetFocusOnError="True">
                                                <RequiredField IsRequired="True" ErrorText="(*)" />
                                            </ValidationSettings>
                                        </dx:ASPxTextBox>
                                    </div>
                                </div>
                            </div>

                            <div class="divRow">
                                <div class="divRowDescription">
                                    <asp:Label ID="lblInitialDateDesc" runat="server" Text="Fecha inicial de validez del valor máximo"></asp:Label>
                                </div>
                                <asp:Label ID="lblInitialDate" runat="server" Text="Fecha inicial:" CssClass="labelForm"></asp:Label>
                                <div class="componentForm">
                                    <div style="float: left">
                                        <dx:ASPxDateEdit ID="txtInitialDate" PopupVerticalAlign="TopSides" PopupHorizontalAlign="OutsideRight" runat="server" AllowNull="false">
                                            <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                        </dx:ASPxDateEdit>
                                    </div>
                                    <div style="float: left; padding: 0px 5px 5px 5px; color: #2D4155;">
                                        <asp:Label ID="lblEndDate" runat="server" Text="a" CssClass=""></asp:Label>
                                    </div>
                                    <div style="float: left">
                                        <dx:ASPxDateEdit ID="txtEndDate" PopupVerticalAlign="TopSides" PopupHorizontalAlign="OutsideLeft" runat="server" AllowNull="true">
                                            <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                        </dx:ASPxDateEdit>
                                    </div>
                                </div>
                            </div>
                            <br />
                            <!-- Este div es el header -->
                            <div class="panBottomMargin">
                                <div class="panHeader2 panBottomMargin">
                                    <span class="panelTitleSpan">
                                        <asp:Label runat="server" ID="lblCauseLimitDefinitionTitle" Text="Definición"></asp:Label>
                                    </span>
                                </div>
                                <!-- La descripción es opcional -->
                                <div class="panelHeaderContent">
                                    <div class="panelDescriptionImage">
                                        <img alt="" src="<%=Me.Page.ResolveUrl("~/LabAgree/Images/StartupValues.png")%>" />
                                    </div>
                                    <div class="panelDescriptionText">
                                        <asp:Label ID="lblCauseLimitDef" runat="server" Text="Aquí se definen los valores máximos para los convenios."></asp:Label>
                                    </div>
                                </div>
                            </div>

                            <!-- Este div es un formulario -->
                            <div class="panBottomMargin">
                                <div class="divRow">
                                    <div class="divRowDescription">
                                        <asp:Label ID="lblcmbCausesDescription" runat="server" Text="Justificación sobre la que definir un valor máximo"></asp:Label>
                                    </div>
                                    <asp:Label ID="lblcmbCauses" runat="server" Text="${Cause}" CssClass="labelForm"></asp:Label>
                                    <div class="componentForm">
                                        <dx:ASPxComboBox runat="server" ID="cmbIDCause" Width="250px" NullText="_____">
                                            <ClientSideEvents Validation="SelectedItemRequiered" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                            <ValidationSettings ErrorDisplayMode="None" SetFocusOnError="True">
                                                <RequiredField IsRequired="True" ErrorText="(*)" />
                                            </ValidationSettings>
                                        </dx:ASPxComboBox>
                                    </div>
                                </div>
                            </div>
                            <div class="panBottomMargin" style="margin-bottom: 0px">
                                <div class="divRow">

                                    <input type="hidden" id="hdnIDType" runat="server" />

                                    <roUserControls:roOptionPanelClient ID="optAnnual" runat="server" TypeOPanel="CheckboxOption" width="100%" height="Auto" Checked="False" Enabled="True" style="width: 100%; padding-top: 0px; padding-bottom: 0px" CConClick="checkAnnualStatus();">
                                        <Title>
                                            <asp:Label ID="lblAlertWith" runat="server" Text="Máximo anual"></asp:Label>
                                        </Title>
                                        <Description></Description>
                                        <Content>
                                            <div class="divRow">
                                                <div class="componentFormWithoutSize">
                                                    <div class="panBottomMargin">
                                                        <div>
                                                            <div style="float: left">
                                                                <dx:ASPxRadioButton GroupName="gAlertValue" Text="Valor del campo de la ficha del empleado" runat="server" ID="rbAnnualUF" ClientInstanceName="rbAnnualUFClient" />
                                                            </div>
                                                            <div style="float: left; padding-left: 7px;">
                                                                <dx:ASPxComboBox runat="server" ID="cmbAnnualValue" Width="200px" ClientInstanceName="cmbAnnualValueClient">
                                                                    <ClientSideEvents SelectedIndexChanged="function(s,e){}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                </dx:ASPxComboBox>
                                                            </div>
                                                        </div>

                                                        <div style="clear: both">
                                                            <div style="float: left; padding-top: 5px">
                                                                <dx:ASPxRadioButton GroupName="gAlertValue" Text="Valor fijo" runat="server" ID="rbAnnualFix" ClientInstanceName="rbAnnualFixClient" />
                                                            </div>
                                                            <div style="float: left; padding-top: 5px; padding-left: 7px;">
                                                                <dx:ASPxTextBox runat="server" ID="txtAnnualValue" Width="200px" ClientInstanceName="txtAnnualValueClient">
                                                                    <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                    <MaskSettings Mask="<0..9999>.<0..9999>" ErrorText="" />
                                                                </dx:ASPxTextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </Content>
                                    </roUserControls:roOptionPanelClient>

                                    <roUserControls:roOptionPanelClient ID="optMonthly" runat="server" TypeOPanel="CheckboxOption" width="100%" height="Auto" Checked="False" Enabled="True" style="width: 100%; padding-top: 0px; padding-bottom: 0px" CConClick="checkMonthlyStatus();">
                                        <Title>
                                            <asp:Label ID="lblMin" runat="server" Text="Máximo mensual"></asp:Label>
                                        </Title>
                                        <Description></Description>
                                        <Content>
                                            <div class="divRow">
                                                <div class="componentFormWithoutSize">
                                                    <div class="panBottomMargin">
                                                        <div>
                                                            <div style="float: left">
                                                                <dx:ASPxRadioButton GroupName="gMinValue" Text="Valor del campo de la ficha del empleado" runat="server" ID="rbMonthlyUF" ClientInstanceName="rbMonthlyUFClient" />
                                                            </div>
                                                            <div style="float: left; padding-left: 7px;">
                                                                <dx:ASPxComboBox runat="server" ID="cmbMonthlyValue" Width="200px" ClientInstanceName="cmbMonthlyValueClient">
                                                                    <ClientSideEvents SelectedIndexChanged="function(s,e){}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                </dx:ASPxComboBox>
                                                            </div>
                                                        </div>

                                                        <div style="clear: both">
                                                            <div style="float: left; padding-top: 5px">
                                                                <dx:ASPxRadioButton GroupName="gMinValue" Text="Valor fijo" runat="server" ID="rbMonthlyFix" ClientInstanceName="rbMonthlyFixClient" />
                                                            </div>
                                                            <div style="float: left; padding-top: 5px; padding-left: 7px;">
                                                                <dx:ASPxTextBox runat="server" ID="txtMonthlyValue" Width="200px" ClientInstanceName="txtMonthlyValueClient">
                                                                    <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                    <MaskSettings Mask="<0..9999>.<0..9999>" ErrorText="" />
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
                            <div class="panBottomMargin">
                                <div class="divRow">
                                    <asp:Label ID="lblExcessTitle" runat="server" Text="En el caso de exceder el valor justificará como" CssClass="labelForm extraWidth"></asp:Label>
                                    <div class="componentFormWithoutSize">
                                        <dx:ASPxComboBox runat="server" ID="cmbCauseExcess" Width="250px" NullText="_____">
                                            <ClientSideEvents Validation="SelectedItemRequiered" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                            <ValidationSettings ErrorDisplayMode="None" SetFocusOnError="True">
                                                <RequiredField IsRequired="True" ErrorText="(*)" />
                                            </ValidationSettings>
                                        </dx:ASPxComboBox>
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
                                            <ClientSideEvents Click="function(s,e){ frmEditCauseLimit_Save(); }" />
                                        </dx:ASPxButton>
                                    </td>
                                    <td style="width: 110px;" align="left">
                                        <dx:ASPxButton ID="btnCancel" runat="server" AutoPostBack="False" CausesValidation="False" Text="Cancelar" ToolTip="Cancelar" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                            <ClientSideEvents Click="function(s,e){ frmEditCauseLimit_Close(); }" />
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