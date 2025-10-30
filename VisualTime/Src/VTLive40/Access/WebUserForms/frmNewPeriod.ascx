<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.frmNewPeriod" CodeBehind="frmNewPeriod.ascx.vb" %>

<!-- Div flotant NewPeriod -->
<input type="hidden" id="hdnAddPeriodIDZone" />
<input type="hidden" id="hdnAddPeriodIDRow" />
<div id="<%= Me.ClientID %>_frm" style="position: fixed; *position: absolute; z-index: 10999; display: none; top: 50%; left: 50%; *width: 600px;">
    <div id="<%= Me.ClientID %>_BgS" style="position: absolute; top: 0; left: 0; display: none; z-index: 10998;"></div>

    <div class="bodyPopupExtended" style="">
        <div style="width: 98%; height: 100%; background-color: White;" class="bodyPopup">
            <table style="width: 100%; padding-top: 5px;" border="0">
                <tr>
                    <td>
                        <div class="panHeader2">
                            <span style="">
                                <asp:Label runat="server" ID="lblAccessPeriod" Text="Nuevo período de acceso"></asp:Label></span>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td style="padding: 2px;">
                        <table border="0" style="width: 100%;">
                            <tr>
                                <td style="text-align: left;">
                                    <asp:Label ID="lblTitleFormNewPeriod" runat="server" CssClass="spanEmp-class" Text="Introducir un período de acceso para el grupo seleccionado."></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="padding-top: 5px; padding-bottom: 10px; text-align: center">
                                    <table border="0" style="width: 100%;">
                                        <tr>
                                            <td>
                                                <table>
                                                    <tr>
                                                        <td style="text-align: left; padding-bottom: 8px;">
                                                            <asp:Label ID="lblPeriodType" runat="server" Text="Tipo de período"></asp:Label></td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <roUserControls:roOptionPanelClient ID="opTypePeriodNormal" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Enabled="True" Value="0">
                                                                <Title>
                                                                    <asp:Label ID="lblTypePeriodNormal" runat="server" Text="Habitual"></asp:Label>
                                                                </Title>
                                                                <Description>
                                                                    <asp:Label ID="lblTypePeriodNormalDesc" runat="server" Text="Los períodos normales definen las horas en las que se puede acceder en fechas laborables y fines de semana."></asp:Label>
                                                                </Description>
                                                                <Content>
                                                                    <table>
                                                                        <tr>
                                                                            <td style="padding-right: 4px; padding-left: 25px;">
                                                                                <asp:Label ID="lblNormal1" runat="server" Text="Los "></asp:Label>
                                                                            </td>
                                                                            <td style="padding-right: 4px;">
                                                                                <dx:ASPxComboBox ID="cmbWeekDay" runat="server" Width="200px" Font-Size="11px" ForeColor="#2D4155"
                                                                                    Font-Names="Arial;Verdana;Sans-Serif" IncrementalFilteringMode="Contains" ClientInstanceName="cmbWeekDayClient">
                                                                                </dx:ASPxComboBox>
                                                                            </td>
                                                                            <td style="padding-right: 4px;">
                                                                                <asp:Label ID="lblNormal2" runat="server" Text=" conceder acceso entre las "></asp:Label>
                                                                            </td>
                                                                            <td style="padding-right: 4px; width: 60px;">
                                                                                <dx:ASPxTimeEdit ID="txtNormalHourBegin" EditFormatString="HH:mm" EditFormat="Custom" runat="server" Width="85" ClientInstanceName="txtNormalHourBeginClient"></dx:ASPxTimeEdit>
                                                                            </td>
                                                                            <td style="padding-right: 4px;">
                                                                                <asp:Label ID="lblNormal3" runat="server" Text=" y las "></asp:Label>
                                                                            </td>
                                                                            <td style="padding-right: 4px; width: 60px;">
                                                                                <dx:ASPxTimeEdit ID="txtNormalHourEnd" EditFormatString="HH:mm" EditFormat="Custom" runat="server" Width="85" ClientInstanceName="txtNormalHourEndClient"></dx:ASPxTimeEdit>
                                                                            </td>
                                                                            <td style="padding-right: 4px;">
                                                                                <asp:Label ID="lblNormal4" runat="server" Text=" inclusive."></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </Content>
                                                            </roUserControls:roOptionPanelClient>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <roUserControls:roOptionPanelClient ID="opTypePeriodEspecific" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Enabled="True" Value="1">
                                                                <Title>
                                                                    <asp:Label ID="lblTypePeriodEspecific" runat="server" Text="Fechas específicas"></asp:Label>
                                                                </Title>
                                                                <Description>
                                                                    <asp:Label ID="lblTypePeriodEspecificDesc" runat="server" Text="Los períodos de fechas específicas tienen prioridad sobre los habituales y se usan para festividades y otros eventos."></asp:Label>
                                                                </Description>
                                                                <Content>
                                                                    <table>
                                                                        <tr>
                                                                            <td colspan="5">
                                                                                <table>
                                                                                    <tr>
                                                                                        <td style="padding-left: 22px;">
                                                                                            <input type="radio" id="optDay" name="optNewEvent" /></td>
                                                                                        <td style="padding-left: 4px;">
                                                                                            <a href="javascript:void(0);" onclick="CheckRadioClick('optDay')">
                                                                                                <asp:Label ID="lblSpecific1" runat="server" Text="El día "></asp:Label></a>
                                                                                        </td>
                                                                                        <td style="padding-left: 4px;">
                                                                                            <input type="text" runat="server" id="txtDay" class="textClass" maxlength="2" style="width: 30px; text-align: right;" convertcontrol="NumberField" ccallowblank="false" ccdecimalprecision="0" ccallowdecimals="false" />
                                                                                        </td>
                                                                                        <td style="padding-left: 4px;">
                                                                                            <asp:Label ID="lblSpecific2" runat="server" Text=" de "></asp:Label>
                                                                                        </td>
                                                                                        <td style="padding-left: 4px;">
                                                                                            <dx:ASPxComboBox ID="cmbMonths" runat="server" Width="200px" Font-Size="11px" ForeColor="#2D4155"
                                                                                                Font-Names="Arial;Verdana;Sans-Serif" IncrementalFilteringMode="Contains" ClientInstanceName="cmbMonthsClient">
                                                                                            </dx:ASPxComboBox>
                                                                                        </td>
                                                                                        <td>&nbsp;</td>
                                                                                    </tr>
                                                                                </table>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td colspan="5">
                                                                                <table>
                                                                                    <tr>
                                                                                        <td style="padding-left: 22px;">
                                                                                            <input type="radio" id="optEvent" name="optNewEvent" /></td>
                                                                                        <td style="padding-left: 4px;">
                                                                                            <a href="javascript:void(0);" onclick="CheckRadioClick('optEvent')">
                                                                                                <asp:Label ID="lblEvent" runat="server" Text="El día que se produzca un evento al que se asigne el periodo "></asp:Label></a>
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td>&nbsp;</td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td colspan="5">
                                                                                <table>
                                                                                    <tr>
                                                                                        <td style="padding-left: 22px;">
                                                                                            <input type="radio" id="optGrantAccess" name="optNewPeriod" /></td>
                                                                                        <td style="padding-left: 4px;">
                                                                                            <a href="javascript:void(0);" onclick="CheckRadioClick('optGrantAccess')">
                                                                                                <asp:Label ID="lblGrant1" runat="server" Text="Conceder acceso entre las "></asp:Label></a>
                                                                                        </td>
                                                                                        <td style="padding-left: 4px; width: 60px;">
                                                                                            <dx:ASPxTimeEdit ID="txtSpecificHourBegin" EditFormatString="HH:mm" EditFormat="Custom" runat="server" Width="85" ClientInstanceName="txtSpecificHourBeginClient"></dx:ASPxTimeEdit>
                                                                                        </td>
                                                                                        <td style="padding-left: 4px;">
                                                                                            <asp:Label ID="lblGrant2" runat="server" Text=" y las "></asp:Label></td>
                                                                                        <td style="padding-left: 4px; width: 60px;">
                                                                                            <dx:ASPxTimeEdit ID="txtSpecificHourEnd" EditFormatString="HH:mm" EditFormat="Custom" runat="server" Width="85" ClientInstanceName="txtSpecificHourEndClient"></dx:ASPxTimeEdit>
                                                                                        </td>
                                                                                        <td style="padding-left: 4px;">
                                                                                            <asp:Label ID="lblGrant3" runat="server" Text=" inclusive."></asp:Label></td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td style="padding-left: 22px;">
                                                                                            <input type="radio" id="optDeniedAccess" name="optNewPeriod" /></td>
                                                                                        <td colspan="5" style="padding-left: 4px; text-align: left;">
                                                                                            <a href="javascript: void(0);" onclick="CheckRadioClick('optDeniedAccess');">
                                                                                                <asp:Label ID="lblDenied" runat="server" Text="No conceder acceso."></asp:Label></a>
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
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <div>
                <table style="float: right; margin-top: -20px;">
                    <tr>
                        <td>
                            <dx:ASPxButton ID="btnAccept" ClientInstanceName="btnAcceptClient" runat="server" AutoPostBack="False" CausesValidation="False" Text="${Button.Accept}" ToolTip="${Button.Accept}"
                                HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                <ClientSideEvents Click="save" />
                            </dx:ASPxButton>
                        </td>
                        <td>
                            <dx:ASPxButton ID="btnCancel" ClientInstanceName="btnCancelClient" runat="server" AutoPostBack="False" CausesValidation="False" Text="${Button.Cancel}" ToolTip="${Button.Cancel}"
                                HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                <ClientSideEvents Click="close" />
                            </dx:ASPxButton>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
</div>
<!-- End Div flotant AddPeriod -->