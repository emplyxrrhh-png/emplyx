<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Emergency_Emergency" Title="Emergency" CodeBehind="Emergency.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

    <script language="javascript" type="text/javascript">
        function PageBase_Load() {
            ConvertControls();
        };
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <input type="hidden" id="hdnReportNameEmpty" value="<%= Me.Language.Translate("ReportNameEmpty",DefaultScope.ToLowerInvariant()) %>" />

        <div id="div1" style="height: 60px;"></div>

        <!-- TAB SUPERIOR -->
        <div id="divTabInfo" style="border: thin solid #B5B8C8; width: 900px; margin-left: auto; margin-right: auto;" class="RoundCorner defaultBackgroundColor">
            <table border="0" cellpadding="0" cellspacing="0" width="100%">
                <tr>
                    <td width="130px">
                        <img alt="" height="128" src="Images/Emergency128.png">
                    </td>
                    <td style="vertical-align: middle;" class="editTextFormat">
                        <asp:Label ID="lblHeader" runat="server" Text="INFORMES DE EMERGENCIA" Font-Size="26px" Font-Bold="True"></asp:Label>
                    </td>
                </tr>
            </table>
        </div>
        <!-- FIN TAB SUPERIOR -->

        <div style="border: thin solid #B5B8C8; width: 900px; margin-left: auto; margin-right: auto; margin-top: 10px;" class="RoundCorner defaultBackgroundColor">
            <div class="panHeader2" style="width: 96%; margin-left: auto; margin-right: auto; margin-top: 10px;">
                <span style="">
                    <asp:Label runat="server" ID="lblEmergencyTitleGeneral" Text="Parámetros de selección"></asp:Label></span>
            </div>
            <br />
            <table style="width: 100%; padding-left: 20px;">
                <tr>
                    <td id="trWithKey" runat="server">
                        <asp:Label ID="lblEmergencyDesc1" runat="server" Font-Size="20px" Text="Introduzca la clave, seleccione un informe de la lista, y pulse el botón para lanzarlo." CssClass="spanEmp-Class"></asp:Label>
                    </td>
                    <td id="trWithoutKey" runat="server" style="display: none;">
                        <asp:Label ID="lblEmergencyDesc2" runat="server" Font-Size="20px" Text="Seleccione un informe de la lista y pulse el botón para lanzarlo." CssClass="spanEmp-Class"></asp:Label>
                    </td>
                </tr>
            </table>
            <table style="width: 100%;">
                <tr style="height: 60px;">
                    <td style="width: 200px; padding-right: 10px;" align="right" valign="middle">
                        <asp:Label ID="lblIP" Text="Está usted en:" runat="server" Font-Size="16px" CssClass="editTextFormat"></asp:Label>
                    </td>
                    <td align="left" valign="middle">
                        <asp:Label ID="lblIPText" Text="192.168.192.168" runat="server" Font-Size="16px" CssClass="editTextFormat"></asp:Label>
                    </td>
                </tr>
                <tr id="trCompany" runat="server" style="height: 60px;">
                    <td style="width: 200px; padding-right: 10px;" align="right" valign="middle">
                        <asp:Label ID="lblServer" runat="server" Text="Cliente:" Font-Size="16px" CssClass="editTextFormat"></asp:Label>
                    </td>
                    <td align="left" valign="middle">
                        <input id="txtCompany" clientidmode="Static" value="" runat="server" maxlength="15" class="RoundCorner editTextFormat" convertcontrol="textField" ccallowblank="false"
                            style="width: 100px; border: thin Solid #B5B8C8; font-weight: bold; font-size: x-large; height: 30px; width: 200px; text-align: center;" />
                    </td>
                </tr>
                <tr id="trSetCompany" runat="server" style="height: 100px;">
                    <td colspan="2" align="center">
                        <dx:ASPxButton ID="btnSetCompany" runat="server" AutoPostBack="False" CausesValidation="False" Text="Conectar" ToolTip="Conectar"
                            HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                            <Image Url="~/Base/Images/Grid/Print16.png"></Image>
                        </dx:ASPxButton>
                    </td>
                </tr>
                <tr id="trKey" runat="server" style="height: 60px;">
                    <td style="width: 200px; padding-right: 10px;" align="right" valign="middle">
                        <asp:Label ID="lblClave" runat="server" Text="Clave:" Font-Size="16px" CssClass="editTextFormat"></asp:Label>
                    </td>
                    <td align="left" valign="middle">
                        <input type="password" id="txtEmergencyReportKey" value="" runat="server" maxlength="15" class="RoundCorner editTextFormat" convertcontrol="textField" ccallowblank="false"
                            style="width: 100px; border: thin Solid #B5B8C8; font-weight: bold; font-size: x-large; height: 30px; width: 200px; text-align: center;" />
                    </td>
                </tr>
                <tr id="trGrid" runat="server">
                    <td style="padding-top: 15px; width: 200px; padding-right: 10px;" align="right" valign="top">
                        <asp:Label ID="lblInf" Text="Informes disponibles:" runat="server" Font-Size="16px" CssClass="editTextFormat"></asp:Label>
                    </td>
                    <td style="padding-top: 15px;" align="left" valign="top">
                        <dx:ASPxGridView ID="GridReports" runat="server" AutoGenerateColumns="False" KeyFieldName="ID" Width="582px" ClientInstanceName="GridReportsClient"
                            allowselectsinglerowonly="false" SettingsPager-Mode="ShowAllRecords">
                            <SettingsPager Mode="ShowAllRecords"></SettingsPager>
                            <Columns>
                                <dx:GridViewCommandColumn ShowSelectButton="false" ShowClearFilterButton="true" ShowSelectCheckbox="True" VisibleIndex="0">
                                </dx:GridViewCommandColumn>
                                <dx:GridViewDataTextColumn FieldName="ID" VisibleIndex="1">
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataTextColumn FieldName="Name" VisibleIndex="2">
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataTextColumn FieldName="ProfileName" VisibleIndex="3">
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataTextColumn FieldName="ReportName" VisibleIndex="4">
                                </dx:GridViewDataTextColumn>
                            </Columns>
                        </dx:ASPxGridView>
                    </td>
                </tr>
                <tr id="trButtonLanzar" runat="server" style="height: 100px;">
                    <td colspan="2" align="center">
                        <dx:ASPxButton ID="btnLanzarInforme" runat="server" AutoPostBack="False" CausesValidation="False" Text="LANZAR" ToolTip="LANZAR"
                            HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                            <Image Url="~/Base/Images/Grid/Print16.png"></Image>
                            <ClientSideEvents Click="btnLanzarInforme_Click" />
                        </dx:ASPxButton>
                    </td>
                </tr>
            </table>
        </div>

        <Local:MsgBoxForm ID="MsgBoxForm1" runat="server" />
    </form>
</body>
</html>