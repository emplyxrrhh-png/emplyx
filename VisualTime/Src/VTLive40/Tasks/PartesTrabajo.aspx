<%@ Page Language="VB" MasterPageFile="~/Base/mastEmp.master" AutoEventWireup="false" Inherits="VTLive40.PartesTrabajo" CodeBehind="PartesTrabajo.aspx.vb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentMainBody" runat="Server">
    <dx:ASPxCallback ID="CallbackSession" runat="server" ClientInstanceName="CallbackSessionClient" ClientSideEvents-CallbackComplete="CallbackSession_CallbackComplete"></dx:ASPxCallback>

    <div style="clear: both">

        <div style="float: left">
            <table style="text-align: left; height: 50px;">
                <tr style="padding-top: 10px;">
                    <td style="height: 5px; width: 99px;" valign="middle" align="right">
                        <asp:Label ID="lblBeginDate" runat="server" Text="Fecha desde:"></asp:Label>
                    </td>
                    <td valign="middle" style="width: 60px;">
                        <dx:ASPxDateEdit runat="server" ID="txtBeginDate" Width="150" ClientInstanceName="txtBeginDateClient">
                            <CalendarProperties ShowClearButton="false" />
                        </dx:ASPxDateEdit>
                    </td>
                    <td valign="middle" align="right" style="width: 99px">
                        <asp:Label ID="lblEndDate" runat="server" Text="Fecha hasta:"></asp:Label>
                    </td>
                    <td valign="middle" style="width: 60px;">
                        <dx:ASPxDateEdit runat="server" ID="txtEndDate" Width="150" ClientInstanceName="txtEndDateClient">
                            <CalendarProperties ShowClearButton="false" />
                        </dx:ASPxDateEdit>
                    </td>
                    <td valign="middle" style="width: 60px;">
                        <dx:ASPxButton ID="btnRefresh" runat="server" AutoPostBack="False" CausesValidation="False" Text="Obtener" ToolTip="Obtener Datos" ClientInstanceName="btnRefreshClient">
                            <ClientSideEvents Click="btnRefreshClient_Click" />
                        </dx:ASPxButton>
                    </td>
                </tr>
            </table>
        </div>

    </div>
    <div style="clear: both">

        <dx:ASPxGridView ID="grdPartes" runat="server" AutoGenerateColumns="False" Width="100%" DataSourceID="LinqPartesDataSource" ClientInstanceName="grdPartesClient">
            <SettingsBehavior AllowFocusedRow="True" AllowSelectSingleRowOnly="True" AllowSort="False" />
            <Settings ShowFilterRow="True" ShowGroupPanel="True" ShowTitlePanel="false" VerticalScrollBarMode="Auto" UseFixedTableLayout="True" VerticalScrollableHeight="610" />
            <SettingsCommandButton>
                <DeleteButton Image-Url="~/Base/Images/Grid/remove.png" Image-ToolTip="" />
                <UpdateButton Image-Url="~/Base/Images/Grid/save.png" Image-ToolTip="" />
                <CancelButton Image-Url="~/Base/Images/Grid/cancel.png" Image-ToolTip="" />
                <EditButton Image-Url="~/Base/Images/Grid/edit.png" Image-ToolTip="" />
            </SettingsCommandButton>
            <ClientSideEvents BeginCallback="GridPartes_BeginCallback" CustomButtonClick="GridPartes_CustomButtonClick" EndCallback="GridPartes_EndCallback" RowDblClick="GridPartes_OnRowDblClick" FocusedRowChanged="GridPartes_FocusedRowChanged" />
        </dx:ASPxGridView>

        <dx:LinqServerModeDataSource ID="LinqPartesDataSource" runat="server" EnableUpdate="true" />
    </div>
</asp:Content>
