<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.WebUserForms_frmFilterCalendar" CodeBehind="frmFilterCalendar.ascx.vb" %>

<div id="<%= Me.ClientID %>_frm" class="ui-dialog-content">
    <form id="<%= Me.ClientID %>_attr">
        <table width="100%" cellspacing="0" class="bodyPopup">
            <tr style="height: 20px;">
                <td colspan="3">
                    <div class="panHeader2">
                        <span style="">
                            <asp:Label ID="lblFilterTitle" runat="server" Text="Visualización de puestos" />
                        </span>
                    </div>
                </td>
            </tr>
            <tr style="display: none">
                <td colspan="3" style="padding-right: 15px; padding-top: 15px; padding-bottom: 0px;">
                    <asp:Label ID="lblAssignmentAlerts" runat="server" Text="Selecciones que tipo de alertas desea mostrar." CssClass="editTextFormat" />
                </td>
            </tr>
            <tr style="display: none">
                <td colspan="3" style="padding-left: 15px; padding-right: 15px; padding-bottom: 0px;">
                    <div class="panBottomMargin">
                        <roUserControls:roGroupBox ID="RoGroupBox6" runat="server">
                            <Content>
                                <div class="divRow">
                                    <div style="float: left">
                                        <dx:ASPxRadioButton ID="rbPlannedView" runat="server" Checked="true" GroupName="rbAssignmentsView" Text="Alertas sobre dotación planificada" />
                                    </div>
                                </div>

                                <div class="divRow">
                                    <div style="float: left">
                                        <dx:ASPxRadioButton ID="rbRealView" runat="server" Checked="true" GroupName="rbAssignmentsView" Text="Alertas sobre dotación real" />
                                    </div>
                                </div>
                            </Content>
                        </roUserControls:roGroupBox>
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan="3" style="padding-right: 15px; padding-top: 5px; padding-bottom: 0px;">
                    <asp:Label ID="lblAssignmentDescription" runat="server" Text="Selecciones los puestos que desea mostrar." CssClass="editTextFormat" />
                </td>
            </tr>
            <tr>
                <td>
                    <div style="height: 300px; overflow: auto">
                        <dx:ASPxGridView ID="grdAssignments" runat="server" Cursor="pointer" AutoGenerateColumns="False" KeyboardSupport="True" Width="100%" Settings-ShowTitlePanel="True">
                            <SettingsBehavior AllowFocusedRow="True" AllowSelectSingleRowOnly="false" AllowSort="False" />
                            <Styles>
                                <Cell Wrap="False"></Cell>
                                <TitlePanel CssClass="TitlePanelClass"></TitlePanel>
                            </Styles>
                            <SettingsPager Mode="ShowAllRecords" ShowEmptyDataRows="false"></SettingsPager>
                            <Border BorderColor="#CDCDCD" />
                            <SettingsLoadingPanel Text=""></SettingsLoadingPanel>
                            <Settings VerticalScrollBarMode="Auto" UseFixedTableLayout="True" VerticalScrollableHeight="225" />
                        </dx:ASPxGridView>
                    </div>
                </td>
            </tr>
        </table>
    </form>
</div>