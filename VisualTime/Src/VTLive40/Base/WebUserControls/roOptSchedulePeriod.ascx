<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.WebUserControls_roOptSchedulePeriod" CodeBehind="roOptSchedulePeriod.ascx.vb" %>

<script type="text/javascript" language="javascript">

    function setSchedulePriodStatus(bStatus) {
        rblPeriodClient.SetEnabled(bStatus);

        if (!bStatus) {
            bdPClie.SetEnabled(bStatus); edPClie.SetEnabled(bStatus); //etPClie.SetEnabled(true); btPClie.SetEnabled(true);
        } else {
            if (rblPeriodClient.GetSelectedItem() != null && rblPeriodClient.GetSelectedItem().value == 0) {
                bdPClie.SetEnabled(true); edPClie.SetEnabled(false); //etPClie.SetEnabled(true); btPClie.SetEnabled(true);
            } else {
                bdPClie.SetEnabled(false); edPClie.SetEnabled(false); //etPClie.SetEnabled(false); btPClie.SetEnabled(false);
            }
        }

    }

    function optSchedulePeriodHasChanges(bHasChanges) {
        try { hasChanges(bHasChanges); } catch (e) { }
    }

    function refreshDatesValues() {
        roOptSchedulePeriodPanelClient.PerformCallback("REFRESH");
    }

    function roOptSchedulePeriodPanelClient_EndCallback(s, e) {
        bdPClie.SetValue(s.cp_beginDate); edPClie.SetValue(s.cp_endDate); //etPClie.SetValue(s.cp_endDate); btPClie.SetValue(s.cp_beginDate);

        if (rblPeriodClient.GetSelectedItem().value == 0) {
            bdPClie.SetEnabled(true); edPClie.SetEnabled(true); //etPClie.SetEnabled(true); btPClie.SetEnabled(true);
        } else {
            bdPClie.SetEnabled(false); edPClie.SetEnabled(false); //etPClie.SetEnabled(false); btPClie.SetEnabled(false);
        }
    }
</script>

<div>
    <dx:ASPxCallbackPanel ID="roOptSchedulePeriodPanel" ClientInstanceName="roOptSchedulePeriodPanelClient" runat="server">
        <ClientSideEvents EndCallback="roOptSchedulePeriodPanelClient_EndCallback" />
        <SettingsLoadingPanel Enabled="false" />
        <PanelCollection>
            <dx:PanelContent ID="PanelContent1" runat="server">

                <div style="float: left">
                    <dx:ASPxRadioButtonList ID="rblPeriod" runat="server" RepeatColumns="2" RepeatLayout="Table" ClientInstanceName="rblPeriodClient">
                        <CaptionSettings Position="Top" />
                        <ClientSideEvents ValueChanged="function(s,e){ refreshDatesValues(); optSchedulePeriodHasChanges(true); if( s.GetSelectedItem().value == 0){ bdPClie.SetEnabled(true);  edPClie.SetEnabled(true);  } else { bdPClie.SetEnabled(false); edPClie.SetEnabled(false); } }" />
                    </dx:ASPxRadioButtonList>
                </div>
                <div style="float: left; padding-left: 5px; vertical-align: middle; padding-top: 20px">
                    <div>
                        <div style="float: left; min-width: 50px; margin-top: 10px">
                            <asp:Label ID="lblBeginDate" runat="server" Text="Desde:" Font-Bold="true"></asp:Label>
                        </div>
                        <div style="float: left">
                            <dx:ASPxDateEdit ID="txtBeginDate" PopupVerticalAlign="WindowCenter" runat="server" AllowNull="false" PopupHorizontalAlign="RightSides" Width="150" ClientInstanceName="bdPClie" EditFormatString="dd/MM/yyyy HH:mm">
                                <TimeSectionProperties Visible="true">
                                    <TimeEditProperties EditFormatString="HH:mm" />
                                </TimeSectionProperties>
                                <ClientSideEvents ValueChanged="function(s,e){optSchedulePeriodHasChanges(true);}" />
                                <ValidationSettings ErrorDisplayMode="None" />
                            </dx:ASPxDateEdit>
                        </div>
                    </div>
                    <div style="clear: both">
                        <div style="float: left; min-width: 50px;margin-top: 10px">
                            <asp:Label ID="lblEndPeriod" runat="server" Text="Hasta:" Font-Bold="true"></asp:Label>
                        </div>
                        <div style="float: left">
                            <dx:ASPxDateEdit ID="txtEndDate" PopupVerticalAlign="WindowCenter" runat="server" AllowNull="false" Width="150" ClientInstanceName="edPClie" EditFormatString="dd/MM/yyyy HH:mm" PopupHorizontalAlign="RightSides">
                                <TimeSectionProperties Visible="true">
                                    <TimeEditProperties EditFormatString="HH:mm" />
                                </TimeSectionProperties>
                                <ClientSideEvents ValueChanged="function(s,e){optSchedulePeriodHasChanges(true);}" />
                                <ValidationSettings ErrorDisplayMode="None" />
                            </dx:ASPxDateEdit>
                        </div>
                    </div>
                    <div style="clear:both; padding-top:100px">
                        <div style="float:left; padding-top:5px">
                            <dx:ASPxComboBox ID="cmbMonthsShiftedDay" runat="server" Width="55px" Font-Size="11px" ForeColor="#2D4155"
                                Font-Names="Arial;Verdana;Sans-Serif" IncrementalFilteringMode="Contains" ClientInstanceName="cmbMonthsShiftedDayClient">
                                <ClientSideEvents SelectedIndexChanged="function(s,e){ refreshDatesValues(); optSchedulePeriodHasChanges(true); if( s.GetSelectedItem().value == 0){ bdPClie.SetEnabled(true);  edPClie.SetEnabled(true);  } else { bdPClie.SetEnabled(false); edPClie.SetEnabled(false); } }" />
                            </dx:ASPxComboBox>
                        </div>
                        <div style="float:left; margin-top: 15px; margin-left: 5px; margin-right: 5px; font: inherit; font-family: 'Robotics'; font-size: 11px;">
                            <asp:Label ID="lblDaysShiftedMonth" runat="server" Text="de" ></asp:Label>
                        </div>
                        <div style="float:left; padding-top:5px">
                            <dx:ASPxComboBox ID="cmbMonthsShifted" runat="server" Width="55px" Font-Size="11px" ForeColor="#2D4155"
                                Font-Names="Arial;Verdana;Sans-Serif" IncrementalFilteringMode="Contains" ClientInstanceName="cmbMonthsShiftedClient">
                                <ClientSideEvents SelectedIndexChanged="function(s,e){ refreshDatesValues(); optSchedulePeriodHasChanges(true); if( s.GetSelectedItem().value == 0){ bdPClie.SetEnabled(true);  edPClie.SetEnabled(true);  } else { bdPClie.SetEnabled(false); edPClie.SetEnabled(false); } }" />
                            </dx:ASPxComboBox>
                        </div>
                        <div style="float:left; margin-top: 15px; margin-left: 5px; font: inherit; font-family: 'Robotics'; font-size: 11px;">
                            <asp:Label ID="lblmonthsShifted" runat="server" Text="meses atrás" ></asp:Label>
                        </div>
                    </div>
            </dx:PanelContent>
        </PanelCollection>
    </dx:ASPxCallbackPanel>
</div>