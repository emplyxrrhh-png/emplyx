<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Base_CurrentLoggedUsers" CodeBehind="CurrentLoggedUsers.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">

        function CloseClick() {
            window.SetContentUrl('');
            window.Hide();
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <dx:ASPxCallbackPanel ID="currentUsersContent" runat="server" Width="100%" Height="100%" ClientInstanceName="currentUsersContentClient">
            <SettingsLoadingPanel Enabled="false" />
            <PanelCollection>
                <dx:PanelContent ID="PopupUploadPanelContent" runat="server">
                    <div class="bodyPopupExtended" style="table-layout: fixed; height: 100%;">

                        <table width="100%">
                            <tr>
                                <td style="padding-top: 5px; padding-bottom: 10px;" height="20px" valign="top">
                                    <div class="panHeader2">
                                        <span style="">
                                            <asp:Label ID="lblTitle" runat="server" Text="Usuarios conectados"></asp:Label>
                                        </span>
                                    </div>
                                </td>
                            </tr>
                            <tr valign="top">
                                <td style="height: 90%;">
                                    <table width="100%;">
                                        <tr>
                                            <td valign="top" style="padding-left: 15px; padding-right: 15px;">
                                                <dx:ASPxGridView ID="GridUsers" runat="server"
                                                    Width="100%" AutoGenerateColumns="False">
                                                    <Settings ShowTitlePanel="False" VerticalScrollBarMode="Auto" UseFixedTableLayout="True" VerticalScrollableHeight="210" />
                                                    <Styles>
                                                        <CommandColumn Spacing="5px" />
                                                        <Header CssClass="jsGridHeaderCell" />
                                                        <Cell Wrap="False" />
                                                    </Styles>
                                                    <%--  <ClientSideEvents Init="grid_Init" BeginCallback="grid_BeginCallback" EndCallback="grid_EndCallback" />--%>
                                                    <Columns>
                                                        <dx:GridViewDataTextColumn Caption="ID" FieldName="UserId" HeaderStyle-Font-Bold="true" />
                                                        <dx:GridViewDataTextColumn Caption="Nombre" FieldName="Name" HeaderStyle-Font-Bold="true">
                                                            <PropertiesTextEdit DisplayFormatString="0.00">
                                                            </PropertiesTextEdit>
                                                        </dx:GridViewDataTextColumn>
                                                        <dx:GridViewDataTextColumn Caption="Descripción" FieldName="Description" HeaderStyle-Font-Bold="true">
                                                        </dx:GridViewDataTextColumn>
                                                        <dx:GridViewDataTextColumn Caption="IP" FieldName="ClientLocation" HeaderStyle-Font-Bold="true">
                                                            <PropertiesTextEdit DisplayFormatString="0.00">
                                                            </PropertiesTextEdit>
                                                        </dx:GridViewDataTextColumn>
                                                        <dx:GridViewDataTextColumn Caption="Origen" FieldName="ApplicationName" HeaderStyle-Font-Bold="true">
                                                            <PropertiesTextEdit DisplayFormatString="0.00">
                                                            </PropertiesTextEdit>
                                                        </dx:GridViewDataTextColumn>
                                                    </Columns>
                                                    <Styles>
                                                        <Header HorizontalAlign="Center" />
                                                    </Styles>
                                                    <Settings />
                                                    <SettingsBehavior AllowDragDrop="true" AllowSort="true" AllowGroup="true" />
                                                    <SettingsPager PageSize="10">
                                                        <%--  <PageSizeItemSettings Visible="true" />--%>
                                                    </SettingsPager>
                                                    <%--                                                    <SettingsLoadingPanel Mode="ShowOnStatusBar" />--%>
                                                </dx:ASPxGridView>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>

                        <br />
                        <table style="margin-left: auto; margin-right: auto; vertical-align: bottom">
                            <tr>
                                <td valign="bottom">
                                    <asp:Button ID="btnAccept" Text="${Button.Accept}" runat="server" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                </td>
                                <td valign="bottom">
                                    <asp:Button ID="btnCancel" Text="${Button.Cancel}" runat="server" OnClientClick="Close(); return false;" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                </td>
                            </tr>
                        </table>
                    </div>
                </dx:PanelContent>
            </PanelCollection>
        </dx:ASPxCallbackPanel>
    </form>
</body>
</html>