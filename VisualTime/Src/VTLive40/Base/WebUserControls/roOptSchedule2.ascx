<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.WebUserControls_roOptSchedule2" CodeBehind="roOptSchedule2.ascx.vb" %>

<table border="0" width="100%" style="padding: 20px; padding-top: 5px;" align="center">
    <tr>
        <td style="width: 150px;" valign="top">
            <table>
                <tr>
                    <td>
                        <input type="radio" name="nameOptSchedule2" id="optHours" runat="server" />&nbsp;<a href="javascript:void(0);" onclick="CheckRadioClick('<%= Me.ClientID %>_optHours');hasChanges(true);"><asp:Label ID="Label1" runat="server" Text="Intervalo horario"></asp:Label></a></td>
                </tr>
                <tr>
                    <td>
                        <input type="radio" name="nameOptSchedule2" id="optDiary" runat="server" />&nbsp;<a href="javascript:void(0);" onclick="CheckRadioClick('<%= Me.ClientID %>_optDiary'); hasChanges(true);"><asp:Label ID="lblDiary" runat="server" Text="Diariamente"></asp:Label></a></td>
                </tr>
                <tr>
                    <td>
                        <input type="radio" name="nameOptSchedule2" id="optWeekly" runat="server" />&nbsp;<a href="javascript:void(0);" onclick="CheckRadioClick('<%= Me.ClientID %>_optWeekly');hasChanges(true);"><asp:Label ID="lblWeekly" runat="server" Text="Semanalmente"></asp:Label></a></td>
                </tr>
                <tr>
                    <td>
                        <input type="radio" name="nameOptSchedule2" id="optMonthly" runat="server" />&nbsp;<a href="javascript:void(0);" onclick="CheckRadioClick('<%= Me.ClientID %>_optMonthly');hasChanges(true);"><asp:Label ID="lblMonthly" runat="server" Text="Mensualmente"></asp:Label></a></td>
                </tr>
                <tr>
                    <td>
                        <input type="radio" name="nameOptSchedule2" id="optOneTime" runat="server" />&nbsp;<a href="javascript:void(0);" onclick="CheckRadioClick('<%= Me.ClientID %>_optOneTime');hasChanges(true);"><asp:Label ID="lblOneTime" runat="server" Text="Sólo una vez"></asp:Label></a></td>
                </tr>
            </table>
        </td>
        <!-- Contenidor divs seleccio -->
        <td valign="top">
            <div id="divHours" runat="server" style="display: ; width: 100%; height: auto;" name="<%= Me.ClientID %>_nameOptSchedule2Tab">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblEach" runat="server" Text="Cada"></asp:Label>&nbsp;</td>
                        <td>

                            <dx:ASPxTimeEdit ID="txtHoursSchedule" EditFormatString="HH:mm" EditFormat="Custom" runat="server" Width="85" ClientInstanceName="txtHoursScheduleClient">
                                <ClientSideEvents DateChanged="function(s,e) {hasChanges(true,false);}" />
                            </dx:ASPxTimeEdit>
                        </td>
                        <td>&nbsp;<asp:Label ID="lblEachHours" runat="server" Text="horas"></asp:Label></td>
                    </tr>
                </table>
            </div>
            <div id="divDaily" runat="server" style="display: ; width: 100%; height: auto;" name="<%= Me.ClientID %>_nameOptSchedule2Tab">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblDailyTo" runat="server" Text="Cada"></asp:Label>&nbsp;</td>
                        <td>
                            <dx:ASPxComboBox ID="cmbDays" runat="server" Width="75px" Font-Size="11px" ForeColor="#2D4155"
                                Font-Names="Arial;Verdana;Sans-Serif" IncrementalFilteringMode="Contains" ClientInstanceName="cmbDaysClient">
                                <ClientSideEvents SelectedIndexChanged="function(s,e) {hasChanges(true,false);}" />
                            </dx:ASPxComboBox>
                        </td>
                        <td>&nbsp;<asp:Label ID="lblDailyDays" runat="server" Text="días"></asp:Label></td>
                    </tr>
                </table>
            </div>
            <div id="divWeekly" runat="server" style="display: none; width: 100%; height: auto;" name="<%= Me.ClientID %>_nameOptSchedule2Tab">
                <table>
                    <tr>
                        <td style="width: 20px; white-space: nowrap;">
                            <asp:Label ID="lblWeeklyTo" runat="server" Text="Cada "></asp:Label>&nbsp;</td>
                        <td style="padding-left: 4px; padding-right: 4px; width: 60px;">
                            <dx:ASPxComboBox ID="cmbWeeklyTo" runat="server" Width="75px" Font-Size="11px" ForeColor="#2D4155"
                                Font-Names="Arial;Verdana;Sans-Serif" IncrementalFilteringMode="Contains" ClientInstanceName="cmbWeeklyToClient">
                                <ClientSideEvents SelectedIndexChanged="function(s,e) {hasChanges(true,false);}" />
                            </dx:ASPxComboBox>
                        </td>
                        <td style="padding-left: 4px; padding-right: 4px;">&nbsp;<asp:Label ID="lblWeeklyWeek" runat="server" Text="semanas, el"></asp:Label></td>
                    </tr>
                    <tr>
                        <td colspan="3" align="left" style="padding-left: 25px; padding-top: 5px;">
                            <table style="width: 250px;" cellpadding="1" cellspacing="1">
                                <tr>
                                    <td style="width: 15px;">
                                        <input type="checkbox" runat="server" onclick="hasChanges(true);" id="chkWeekDay1" /></td>
                                    <td><a href="javascript: void(0);" onclick="CheckLinkClick('<%= chkWeekDay1.ClientID %>');">
                                        <asp:Label ID="lblWeekDay1" runat="server" Text="Lunes"></asp:Label></a></td>
                                    <td style="width: 15px;">
                                        <input type="checkbox" runat="server" onclick="hasChanges(true);" id="chkWeekDay2" /></td>
                                    <td><a href="javascript: void(0);" onclick="CheckLinkClick('<%= chkWeekDay2.ClientID %>');">
                                        <asp:Label ID="lblWeekDay2" runat="server" Text="Martes"></asp:Label></a></td>
                                </tr>
                                <tr>
                                    <td style="width: 15px;">
                                        <input type="checkbox" runat="server" onclick="hasChanges(true);" id="chkWeekDay3" /></td>
                                    <td><a href="javascript: void(0);" onclick="CheckLinkClick('<%= chkWeekDay3.ClientID %>');">
                                        <asp:Label ID="lblWeekDay3" runat="server" Text="Miercoles"></asp:Label></a></td>
                                    <td style="width: 15px;">
                                        <input type="checkbox" runat="server" onclick="hasChanges(true);" id="chkWeekDay4" /></td>
                                    <td><a href="javascript: void(0);" onclick="CheckLinkClick('<%= chkWeekDay4.ClientID %>');">
                                        <asp:Label ID="lblWeekDay4" runat="server" Text="Jueves"></asp:Label></a></td>
                                </tr>
                                <tr>
                                    <td style="width: 15px;">
                                        <input type="checkbox" runat="server" onclick="hasChanges(true);" id="chkWeekDay5" /></td>
                                    <td><a href="javascript: void(0);" onclick="CheckLinkClick('<%= chkWeekDay5.ClientID %>');">
                                        <asp:Label ID="lblWeekDay5" runat="server" Text="Viernes"></asp:Label></a></td>
                                    <td style="width: 15px;">
                                        <input type="checkbox" runat="server" onclick="hasChanges(true);" id="chkWeekDay6" /></td>
                                    <td><a href="javascript: void(0);" onclick="CheckLinkClick('<%= chkWeekDay6.ClientID %>');">
                                        <asp:Label ID="lblWeekDay6" runat="server" Text="Sabado"></asp:Label></a></td>
                                </tr>
                                <tr>
                                    <td style="width: 15px;">
                                        <input type="checkbox" runat="server" onclick="hasChanges(true);" id="chkWeekDay7" /></td>
                                    <td><a href="javascript: void(0);" onclick="CheckLinkClick('<%= chkWeekDay7.ClientID %>');">
                                        <asp:Label ID="lblWeekDay7" runat="server" Text="Domingo"></asp:Label></a></td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </div>
            <div id="divMonthly" runat="server" style="display: none; width: 100%; height: auto;" name="<%= Me.ClientID %>_nameOptSchedule2Tab">
                <table>
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <input type="radio" runat="server" id="opMonth1" name="opMonthName" /></td>
                                    <td><a href="javascript:void(0);" onclick="CheckRadioClick('<%= Me.ClientID %>_opMonth1');">
                                        <asp:Label ID="lblM1TheDay" runat="server" Text="El día "></asp:Label></a></td>
                                    <td>
                                        <div id="<%= Me.ClientID %>_divOpM1">
                                            <table>
                                                <tr>
                                                    <td style="padding-left: 4px; padding-right: 4px;">
                                                        <dx:ASPxComboBox ID="cmbM1O1" runat="server" Width="75px" Font-Size="11px" ForeColor="#2D4155"
                                                            Font-Names="Arial;Verdana;Sans-Serif" IncrementalFilteringMode="Contains" ClientInstanceName="cmbM1O1Client">
                                                            <ClientSideEvents SelectedIndexChanged="function(s,e) {hasChanges(true,false);}" />
                                                        </dx:ASPxComboBox>
                                                    </td>
                                                    <td style="padding-left: 4px; padding-right: 4px;">
                                                        <asp:Label ID="lblM1Of" runat="server" Text=" de cada mes"></asp:Label></td>
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
                                        <input type="radio" runat="server" id="opMonth2" name="opMonthName" /></td>
                                    <td><a href="javascript:void(0);" onclick="CheckRadioClick('<%= Me.ClientID %>_opMonth2');">
                                        <asp:Label ID="lblM2The" runat="server" Text="El "></asp:Label></a></td>
                                    <td>
                                        <div id="<%= Me.ClientID %>_divOpM2">
                                            <table>
                                                <tr>
                                                    <td style="padding-left: 4px; padding-right: 4px;">
                                                        <dx:ASPxComboBox ID="cmbM2O1" runat="server" Width="110px" Font-Size="11px" ForeColor="#2D4155"
                                                            Font-Names="Arial;Verdana;Sans-Serif" IncrementalFilteringMode="Contains" ClientInstanceName="cmbM2O1Client">
                                                            <ClientSideEvents SelectedIndexChanged="function(s,e) {hasChanges(true,false);}" />
                                                        </dx:ASPxComboBox>
                                                    </td>
                                                    <td style="padding-left: 4px; padding-right: 4px;">
                                                        <dx:ASPxComboBox ID="cmbM2O2" runat="server" Width="110px" Font-Size="11px" ForeColor="#2D4155"
                                                            Font-Names="Arial;Verdana;Sans-Serif" IncrementalFilteringMode="Contains" ClientInstanceName="cmbM2O2Client">
                                                            <ClientSideEvents SelectedIndexChanged="function(s,e) {hasChanges(true,false);}" />
                                                        </dx:ASPxComboBox>
                                                    </td>
                                                    <td style="padding-left: 4px; padding-right: 4px;">
                                                        <asp:Label ID="lblM2Of" runat="server" Text=" de cada mes"></asp:Label></td>
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
            <div id="divOneTime" runat="server" style="display: none; width: 100%; height: auto;" name="<%= Me.ClientID %>_nameOptSchedule2Tab">
                <table>
                    <tr>
                        <td style="padding-left: 4px; padding-right: 4px;">
                            <asp:Label ID="lblOneTimeDay" runat="server" Text="El día "></asp:Label></td>
                        <td style="padding-left: 4px; padding-right: 4px;">
                            <%--<input type="text" id="txtDateSchedule" style="width:75px;" class="textClass" ConvertControl="DatePicker" CCallowblank="true" runat="server" />--%>
                            <dx:ASPxDateEdit runat="server" ID="txtDateSchedule" PopupVerticalAlign="WindowCenter" Width="150" ClientInstanceName="txtDateScheduleClient">
                                <CalendarProperties ShowClearButton="false" />
                                <ClientSideEvents DateChanged="function(s,e) {hasChanges(true,false);}" />
                            </dx:ASPxDateEdit>
                        </td>
                    </tr>
                </table>
            </div>
        </td>
    </tr>
    <tr>
        <td></td>
        <td>
            <div id="divCommonHours" runat="server" style="display: ; width: 100%; height: auto;" name="<%= Me.ClientID %>_nameOptSchedule2Tab">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblHoursTo" runat="server" Text=" a las "></asp:Label></td>
                        <td>
                            <%--<input type="text" runat="server" id="txtHours" class="textClass x-form-text x-form-field" style="width: 40px; display:;" value="" ConvertControl="TimeField" ccallowblank="false" />--%>
                            <dx:ASPxTimeEdit ID="txtHours" EditFormatString="HH:mm" EditFormat="Custom" runat="server" Width="85" ClientInstanceName="txtHoursClient">
                                <ClientSideEvents DateChanged="function(s,e) {hasChanges(true,false);}" />
                            </dx:ASPxTimeEdit>
                        </td>
                    </tr>
                </table>
            </div>
        </td>
    </tr>
</table>