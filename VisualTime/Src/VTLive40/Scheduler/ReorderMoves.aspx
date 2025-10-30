<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.ReorderMoves" CodeBehind="ReorderMoves.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Reorder Moves</title>
</head>
<body class="bodyPopup">
    <form id="frmReorderMoves" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <div>

            <script language="javascript" type="text/javascript">

                var _generalUpdateProgressDiv;
                var _generalBackgroundDiv;

                function PageBase_Load() {

                }

                function PageBase_Unload() {
                    var IDShiftSelected = $get('<%= Me.hdnIDShiftSelected.ClientID %>').value;
                    if (IDShiftSelected != '0') {
                        try {
                            parent.ShiftChange(IDShiftSelected);
                        } catch (e) { }
                    }
                }
            </script>

            <asp:UpdatePanel ID="updShiftSelector" runat="server" RenderMode="inline">
                <ContentTemplate>
                    <table cellpadding="0" cellspacing="0" border="0" width="475px">
                        <tr>
                            <td colspan="2">
                                <div class="panHeader2">
                                    <asp:Label ID="lblReorderMovesTitle" Text="Reorganizar Marcajes" CssClass="panHeaderLabel" runat="server" />
                                    <asp:HiddenField ID="hdnEmployeeID" Value="0" runat="server" />
                                    <asp:HiddenField ID="hdnDate" Value="" runat="server" />
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" style="padding-top: 10px">
                                <asp:Label ID="lblReorderInfo" Text="Organizar marcajes" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" style="padding-top: 5px" align="left">
                                <asp:Panel ID="panGridMoves" Height="200" Width="475" ScrollBars="Auto" runat="server" Style="padding-top: 5px; height: 145px; *height: 100px;">
                                    <asp:ObjectDataSource ID="BlankData2" runat="server" SelectMethod="_Select" TypeName="BlankDataObject" />
                                    <roWebControls:roGridViewControl ID="grdMoves" runat="server" AutoGenerateColumns="false" DataSourceID="BlankData2"
                                        GridLines="None" CellPadding="4"
                                        ShowFooter="false" AllowPaging="false" PageSize="7"
                                        CssClass="yui-datatable-small-theme"
                                        Width="100%" ScrollWidth="80%" Height="100%" FreezeHeader="true" FreezeColumn="0" Scrolling="Auto"
                                        CellHoverCssClass="row-over" CellSelectCssClass="row-select"
                                        RowSelectedControlID='<%# hdnMovesSelectedRowIndex.ClientID%>'
                                        ColSelectedControlID='<%# hdnMovesSelectedColIndex.ClientID%>'>
                                        <RowStyle CssClass="data-row" />
                                        <AlternatingRowStyle CssClass="alt-data-row" />
                                        <Columns>
                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                </ItemTemplate>
                                                <ItemStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="fix-data-row" />
                                                <HeaderStyle CssClass=".fix-header" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Entrada">
                                                <ItemTemplate>
                                                    <asp:Label ID="InDateTime_Label" Visible="true" runat="server" Width="35" />
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                </FooterTemplate>
                                                <ItemStyle HorizontalAlign="Center" />
                                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="${Cause}" HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="130px" ItemStyle-HorizontalAlign="Left">
                                                <ItemTemplate>
                                                    <asp:Label ID="InIDCause_Label" Text='<%# Eval("InIDCause") %>' Visible="false" runat="server" />
                                                    <asp:Label ID="InIDCause_LabelName" Text='<%# Eval("InIDCauseName") %>' Visible="true" runat="server" Width="130px" />
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                </FooterTemplate>
                                                <ItemStyle HorizontalAlign="Center" />
                                                <HeaderStyle HorizontalAlign="Center" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Salida">
                                                <ItemTemplate>
                                                    <asp:Label ID="OutDateTime_Label" Text="" Visible="true" runat="server" Width="35" />
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                </FooterTemplate>
                                                <ItemStyle HorizontalAlign="Center" />
                                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="${Cause}" HeaderStyle-HorizontalAlign="left" HeaderStyle-Width="130px" ItemStyle-HorizontalAlign="Left">
                                                <ItemTemplate>
                                                    <asp:Label ID="OutIDCause_Label" Text='<%# Eval("OutIDCause") %>' Visible="false" runat="server" />
                                                    <asp:Label ID="OutIDCause_LabelName" Text='<%# Eval("OutIDCauseName") %>' Visible="true" runat="server" Width="130px" />
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                </FooterTemplate>
                                                <ItemStyle HorizontalAlign="Center" />
                                                <HeaderStyle HorizontalAlign="Center" />
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="ShiftDate" HeaderText="Fecha_real" DataFormatString="{0:d}" HtmlEncode="false"
                                                ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ItemStyle-Width="85" ItemStyle-CssClass="yui-datatable-moves-theme" />
                                        </Columns>
                                    </roWebControls:roGridViewControl>
                                    <asp:HiddenField ID="hdnMovesSelectedRowIndex" runat="server" Value="-1" />
                                    <asp:HiddenField ID="hdnMovesSelectedColIndex" runat="server" Value="-1" />
                                    <asp:HiddenField ID="hdnMovesScrollLeft" runat="server" Value="0" />
                                    <asp:HiddenField ID="hdnMovesScrollTop" runat="server" Value="0" />
                                </asp:Panel>
                                <tr>
                                    <td align="left">
                                        <asp:Label ID="lblError" Text="" CssClass="errorText" runat="server" Visible="false"></asp:Label>
                                    </td>
                                    <td align="right">
                                        <table>
                                            <tr>
                                                <td>
                                                    <asp:Button ID="btAccept" Text="${Button.Accept}" runat="server" OnClientClick="" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                                </td>
                                                <td>
                                                    <asp:Button ID="btCancel" Text="${Button.Cancel}" runat="server" OnClientClick="Close(); return false;" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                    </table>
                    <asp:HiddenField ID="hdnIDShiftSelected" Value="0" runat="server" />
                    <asp:HiddenField ID="hdnCanClose_PageBase" runat="server" />
                    <asp:HiddenField ID="hdnMustRefresh_PageBase" Value="0" runat="server" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </form>
</body>
</html>