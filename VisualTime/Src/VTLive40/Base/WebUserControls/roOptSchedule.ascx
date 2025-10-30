<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.WebUserControls_roOptSchedule" CodeBehind="roOptSchedule.ascx.vb" %>

<table border="0" width="100%" style="padding: 20px; padding-top: 5px;" align="center">
    <tr>
        <td style="width: 150px;">
            <table>
                <tr>
                    <td>
                        <input type="radio" name="nameOptSchedule" id="optDiary" runat="server" />&nbsp;<a href="javascript:void(0);" onclick="CheckRadioClick('<%= Me.ClientID %>_optDiary');"><asp:Label ID="lblDiary" runat="server" Text="Diariamente"></asp:Label></a></td>
                </tr>
                <tr>
                    <td>
                        <input type="radio" name="nameOptSchedule" id="optWeekly" runat="server" />&nbsp;<a href="javascript:void(0);" onclick="CheckRadioClick('<%= Me.ClientID %>_optWeekly');"><asp:Label ID="lblWeekly" runat="server" Text="Semanalmente"></asp:Label></a></td>
                </tr>
                <tr>
                    <td>
                        <input type="radio" name="nameOptSchedule" id="optMonthly" runat="server" />&nbsp;<a href="javascript:void(0);" onclick="CheckRadioClick('<%= Me.ClientID %>_optMonthly');"><asp:Label ID="lblMonthly" runat="server" Text="Mensualmente"></asp:Label></a></td>
                </tr>
                <tr>
                    <td>
                        <input type="radio" name="nameOptSchedule" id="optAnnual" runat="server" />&nbsp;<a href="javascript:void(0);" onclick="CheckRadioClick('<%= Me.ClientID %>_optAnnual');"><asp:Label ID="lblAnnual" runat="server" Text="Anualmente"></asp:Label></a></td>
                </tr>
            </table>
        </td>
        <!-- Contenidor divs seleccio -->
        <td valign="top">
            <div id="divDaily" runat="server" style="width: 100%; height: auto;">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblDailyTo" runat="server" Text="Cada"></asp:Label>&nbsp;</td>
                        <td>
                            <dx:ASPxTextBox ID="txtDaily" MaxLength="3" runat="server">
                                <MaskSettings Mask="<0..999>" />
                                <ClientSideEvents TextChanged="function(s,e){ hasChanges(true); }" />
                                <ValidationSettings ErrorDisplayMode="none" />
                            </dx:ASPxTextBox>
                        </td>
                        <td>&nbsp;<asp:Label ID="lblDailyDays" runat="server" Text="días"></asp:Label></td>
                    </tr>
                </table>
            </div>
            <div id="divWeekly" runat="server" style="display: none; width: 100%; height: auto;">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblWeeklyTo" runat="server" Text="El"></asp:Label>&nbsp;</td>
                        <td style="padding-left: 4px; padding-right: 4px;">
                            <dx:ASPxComboBox ID="cmbWeeklyTo" runat="server">
                                <ClientSideEvents SelectedIndexChanged="function(s,e){ hasChanges(true); }" />
                                <ValidationSettings ErrorDisplayMode="none" />
                            </dx:ASPxComboBox>
                        </td>
                        <td style="padding-left: 4px; padding-right: 4px;">&nbsp;<asp:Label ID="lblWeeklyWeek" runat="server" Text="de cada semana"></asp:Label></td>
                    </tr>
                </table>
            </div>
            <div id="divMonthly" runat="server" style="display: none; width: 100%; height: auto;" name="<%= Me.ClientID %>_nameOptScheduleTab">
                <table>
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <input type="radio" id="opMonth1" runat="server" name="opMonthName" /></td>
                                    <td><a href="javascript:void(0);" onclick="CheckRadioClick('<%= Me.ClientID %>_opMonth1');">
                                        <asp:Label ID="lblM1TheDay" runat="server" Text="El día "></asp:Label></a></td>
                                    <td>
                                        <div id="<%= Me.ClientID %>_divOpM1">
                                            <table>
                                                <tr>
                                                    <td style="padding-left: 4px; padding-right: 4px;">
                                                        <dx:ASPxComboBox ID="cmbM1O1" runat="server">
                                                            <ClientSideEvents SelectedIndexChanged="function(s,e){ hasChanges(true); }" />
                                                            <ValidationSettings ErrorDisplayMode="none" />
                                                        </dx:ASPxComboBox>
                                                    </td>
                                                    <td style="padding-left: 4px; padding-right: 4px;">
                                                        <asp:Label ID="lblM1Of" runat="server" Text=" de cada "></asp:Label></td>
                                                    <td style="padding-left: 4px; padding-right: 4px;">
                                                        <dx:ASPxComboBox ID="cmbM1O2" runat="server">
                                                            <ClientSideEvents SelectedIndexChanged="function(s,e){ hasChanges(true); }" />
                                                            <ValidationSettings ErrorDisplayMode="none" />
                                                        </dx:ASPxComboBox>
                                                    </td>
                                                    <td style="padding-left: 4px; padding-right: 4px;">
                                                        <asp:Label ID="lblM1Months" runat="server" Text=" meses."></asp:Label></td>
                                                </tr>
                                            </table>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <input type="radio" id="opMonth2" runat="server" name="opMonthName" /></td>
                                    <td><a href="javascript:void(0);" onclick="CheckRadioClick('<%= Me.ClientID %>_opMonth2');">
                                        <asp:Label ID="lblM2The" runat="server" Text="El "></asp:Label></a></td>
                                    <td>
                                        <div id="<%= Me.ClientID %>_divOpM2">
                                            <table>
                                                <tr>
                                                    <td style="padding-left: 4px; padding-right: 4px;">
                                                        <dx:ASPxComboBox ID="cmbM2O1" runat="server">
                                                            <ClientSideEvents SelectedIndexChanged="function(s,e){ hasChanges(true); }" />
                                                            <ValidationSettings ErrorDisplayMode="none" />
                                                        </dx:ASPxComboBox>
                                                    </td>
                                                    <td style="padding-left: 4px; padding-right: 4px;">
                                                        <dx:ASPxComboBox ID="cmbM2O2" runat="server">
                                                            <ClientSideEvents SelectedIndexChanged="function(s,e){ hasChanges(true); }" />
                                                            <ValidationSettings ErrorDisplayMode="none" />
                                                        </dx:ASPxComboBox>
                                                    </td>
                                                    <td style="padding-left: 4px; padding-right: 4px;">
                                                        <asp:Label ID="lblM2Of" runat="server" Text=" de cada "></asp:Label></td>
                                                    <td style="padding-left: 4px; padding-right: 4px;">
                                                        <dx:ASPxComboBox ID="cmbM2O3" runat="server">
                                                            <ClientSideEvents SelectedIndexChanged="function(s,e){ hasChanges(true); }" />
                                                            <ValidationSettings ErrorDisplayMode="none" />
                                                        </dx:ASPxComboBox>
                                                    </td>
                                                    <td style="padding-left: 4px; padding-right: 4px;">
                                                        <asp:Label ID="lblM2Months" runat="server" Text=" meses."></asp:Label></td>
                                                </tr>
                                            </table>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </div>
            <div id="divAnnual" runat="server" style="display: none; width: 100%; height: auto;">
                <table>
                    <tr>
                        <%--<td>
                            <input type="radio" name="nameOptFixDay" id="anualFixDay" runat="server" /></td>
                        <td style="padding-left: 4px; padding-right: 4px;">
                            <a href="javascript:void(0);" onclick="CheckRadioClick('<%= Me.ClientID %>_anualFixDay');">
                                <asp:Label ID="lblAnnualDay" runat="server" Text="El día "></asp:Label></a></td>--%>
                        <td style="padding-left: 4px; padding-right: 4px;">
                            <asp:Label ID="lblAnnualDay" runat="server" Text="El día "></asp:Label></td>
                        <td>

                            <div id="<%= Me.ClientID %>_divAnualM1">
                                <table>
                                    <tr>
                                        <td style="padding-left: 4px; padding-right: 4px;">
                                            <dx:ASPxComboBox ID="cmbA1O1" runat="server">
                                                <ClientSideEvents SelectedIndexChanged="function(s,e){ hasChanges(true); }" />
                                                <ValidationSettings ErrorDisplayMode="none" />
                                            </dx:ASPxComboBox>
                                        </td>
                                        <td style="padding-left: 4px; padding-right: 4px;">
                                            <asp:Label ID="lblAnnualSeparator" runat="server" Text="/"></asp:Label></td>
                                        <td style="padding-left: 4px; padding-right: 4px;">
                                            <dx:ASPxComboBox ID="cmbA1O2" runat="server">
                                                <ClientSideEvents SelectedIndexChanged="function(s,e){ hasChanges(true); }" />
                                                <ValidationSettings ErrorDisplayMode="none" />
                                            </dx:ASPxComboBox>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>
                    <%--<tr >
                        <td>
                            <input type="radio" name="nameOptFixDay" id="anualLastDay" runat="server" />
                        </td>
                        <td style="padding-left: 4px; padding-right: 4px;" colspan="2">
                            <a href="javascript:void(0);" onclick="CheckRadioClick('<%= Me.ClientID %>_anualLastDay');">
                                <asp:Label ID="lblAnualLastDay" runat="server" Text="El último día del periodo"></asp:Label></a>
                        </td>
                    </tr>--%>
                </table>
            </div>
        </td>
    </tr>
</table>