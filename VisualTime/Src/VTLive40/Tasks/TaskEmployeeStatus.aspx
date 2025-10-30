<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Tasks_TaskEmployeeStatus" CodeBehind="TaskEmployeeStatus.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <dx:ASPxPanel ID="panelUploadContent" runat="server" Width="0px" Height="0px">
            <PanelCollection>
                <dx:PanelContent ID="PopupUploadPanelContent" runat="server">
                    <div class="bodyPopupExtended" style="table-layout: fixed; height: 460px; width: 450px">
                        <div style="height: 400px; vertical-align: top">
                            <table width="100%">
                                <tr>
                                    <asp:HiddenField ID="hdnIDTaskSelected" runat="server" Value="" />
                                    <td style="padding-top: 5px; padding-bottom: 10px;" height="20px" valign="top">
                                        <div class="panHeader2">
                                            <span style="">
                                                <asp:Label ID="lblTitle" runat="server" Text="Estado de la tarea"></asp:Label></span>
                                        </div>
                                    </td>
                                </tr>
                                <!-- Contenido de la pagina -->
                                <tr>
                                    <td>
                                        <dx:ASPxGridView ID="GridCurrentEmployees" runat="server" Cursor="pointer" AutoGenerateColumns="False"
                                            ClientInstanceName="GridCurrentEmployeesClient" KeyboardSupport="True" Width="100%">
                                            <ClientSideEvents CustomButtonClick="GridCurrentEmployeesClientCustomButton_Click" />
                                            <Border BorderColor="#CDCDCD" />
                                        </dx:ASPxGridView>
                                    </td>
                                </tr>
                                <tr>
                                    <td>&nbsp;<br />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <dx:ASPxGridView ID="GridPastEmployees" runat="server" Cursor="pointer" AutoGenerateColumns="False"
                                            ClientInstanceName="GridPastEmployeesClient" KeyboardSupport="True" Width="100%">
                                            <ClientSideEvents CustomButtonClick="GridPastEmployeesClientCustomButton_Click" />
                                            <Border BorderColor="#CDCDCD" />
                                        </dx:ASPxGridView>
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <div style="height: 50px; vertical-align: bottom">
                            <table style="margin-left: auto; vertical-align: bottom">
                                <tr>
                                    <td>&nbsp;
                                    </td>
                                    <td>
                                        <dx:ASPxButton ID="btnCancel" runat="server" AutoPostBack="False" CausesValidation="False" Text="${Button.Close}" ToolTip="${Button.Close}" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                            <ClientSideEvents Click="CloseClick" />
                                        </dx:ASPxButton>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>
                </dx:PanelContent>
            </PanelCollection>
        </dx:ASPxPanel>
    </form>
</body>
</html>