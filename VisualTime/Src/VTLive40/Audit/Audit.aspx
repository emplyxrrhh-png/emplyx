<%@ Page Language="VB" MasterPageFile="~/Base/mastEmp.master" AutoEventWireup="false" Inherits="VTLive40.Audit" Title="Auditoría de acciones" EnableEventValidation="false" CodeBehind="Audit.aspx.vb" %>

<asp:Content ID="cMainBody" ContentPlaceHolderID="contentMainBody" runat="Server">    
    <script language="javascript" type="text/javascript">
        

        function PageBase_Load() {            
        }

        function ResizePage() {
        }

        window.onresize = function () { ResizePage() }
        
    </script>

    <dx:ASPxCallback ID="PerformActionCallback" runat="server" ClientInstanceName="PerformActionCallbackClient" ClientSideEvents-CallbackComplete="PerformActionCallback_CallbackComplete"></dx:ASPxCallback>

    <div id="divMainBody">
        <!-- TAB SUPERIOR -->
        <div id="divTabInfo" class="divDataCells">
            <div style="min-height: 10px"></div>
            <div id="divAccessGroup" class="blackRibbonTitle">
                <div class="blackRibbonIcon">
                    <asp:Image ID="imgAudit" ImageUrl="Images/Audit90.png" runat="server" />
                </div>
                <div class="blackRibbonDescription">
                    <asp:Label ID="lblHeader" runat="server" Text="Auditoría de acciones" CssClass="NameText"></asp:Label>
                    <br />
                    <asp:Label ID="lblInfo" runat="server" Text="Esta es la pantalla de la auditoría de acciones. Aquí podrá consultar todas las acciones que han realizado los diferentes usuarios de VisualTime Live. Tiene la posibilidad de filtrar sus consultas por varios criterios, de manera que pueda acotar más los resultados."></asp:Label>
                </div>
            </div>
            <div style="min-height: 10px"></div>
        </div>
        <!-- FIN TAB SUPERIOR -->
        <!-- DETALLE -->
        <div id="divTabData" class="divDataCells">
            <div id="divContenido" class="divAllContent">
                <div id="divContent" style="height: initial;" class="maxHeight">
                    <div>
                        <div class="RoundCornerFrame roundCorner" style="min-height: 70px;">
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
                                        <td align="Left" rowspan="2">
                                            <asp:ImageButton ID="ibtRefresh" ImageUrl="Images/BOOK_RELOAD_32.GIF" ToolTip="Actualizar datos" runat="server" OnClientClick="btnRefreshClient_Click();return false;" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td valign="middle" align="right" style="width: 99px">
                                            <asp:Label ID="lblEndDate" runat="server" Text="Fecha hasta:"></asp:Label>
                                        </td>
                                        <td valign="middle" style="width: 60px;">
                                            <dx:ASPxDateEdit runat="server" ID="txtEndDate" Width="150" ClientInstanceName="txtEndDateClient">
                                                <CalendarProperties ShowClearButton="false" />
                                            </dx:ASPxDateEdit>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <div style="float: right">
                                <div id="divExport" style="display:none;float: left; padding-right: 10px; padding-top: 15px;">
                                    <dx:ASPxButton ID="btnExportToXls" AutoPostBack="false" ClientInstanceName="btnExportToXlsClient" runat="server" CausesValidation="False" Text="Exportar" ToolTip="Exportar a Excel" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat">
                                        <Image Url="~/Base/Images/Grid/ExportToExcel16.png"></Image>
                                        <ClientSideEvents Click="function(s,e) { exportToExcel(); }" />
                                    </dx:ASPxButton>
                                </div>
                            </div>
                        </div>
                        <table style="text-align: left; width: 100%; height: 100%;">
                            <tr style="padding-top: 10px;">
                                <td colspan="8" style="height: 90%;" valign="top" align="center">
                                    <div id="flxAudit"></div>
                                </td>
                            </tr>
                        </table>
                        <dx:ASPxGridViewExporter ID="ASPxGridViewExporter1" runat="server">
                        </dx:ASPxGridViewExporter>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- POPUP NEW OBJECT -->
    <dx:ASPxPopupControl ID="AspxLoadingPopup" runat="server" AllowDragging="False" CloseAction="None" Modal="True" ContentUrl="~/Base/Popups/PerformingAction.aspx"
        PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter" ModalBackgroundStyle-Opacity="0" Width="460" Height="260"
        ShowHeader="False" ScrollBars="Auto" ShowPageScrollbarWhenModal="True" ClientInstanceName="AspxLoadingPopup_Client" PopupAnimationType="None" BackColor="Transparent" ContentStyle-Paddings-Padding="0px" Border-BorderColor="Transparent" ShowShadow="false">
        <SettingsLoadingPanel Enabled="false" />
    </dx:ASPxPopupControl>
</asp:Content>