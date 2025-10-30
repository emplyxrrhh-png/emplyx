<%@ Page Language="VB" MasterPageFile="~/Base/mastEmp.master" AutoEventWireup="false" Inherits="VTLive40.Start" Culture="auto" UICulture="auto" CodeBehind="Start.aspx.vb" %>

<asp:Content ID="cMainBody" ContentPlaceHolderID="contentMainBody" runat="Server">

    <script language="javascript" type="text/javascript">
        function loadNewFeed() {
            try {
                var parms = "";
                parms = { "action": "LoadXmlFeed" };
                AjaxCall("POST", "json", "Base/Handlers/srvMain.ashx", parms, "CONTAINER", "newsXmlFeed", "", 0);

                window.parent.setUPReportsAndWizards({ HasReports: false, HasAssistants: false });
            } catch (e) { }
        }
    </script>

    <div id="divMainBody">
        <!-- TAB SUPERIOR -->
        <div id="divTabInfo" class="divDataCells" style="">
            <div style="min-height: 10px"></div>
            <div id="divStartRibbon" class="blackRibbonTitle">
                <div class="blackRibbonIcon">
                    <asp:Image ID="imgPrincipal" runat="server" ImageUrl="~/Base/Images/IcoHome90_2.png"></asp:Image>
                </div>
                <div class="blackRibbonDescription">
                    <asp:Label ID="lblName" runat="server" Text="Principal" CssClass="NameText"></asp:Label><br />
                    <asp:Label ID="lblNameDescriptor" runat="server" Text="Bienvenido a VisualTime Live" CssClass="barDescription" />
                </div>
            </div>
            <div style="min-height: 10px"></div>
        </div>

        <div id="divTabData" class="divDataCells newsColumn">
            <div class="headerNewsDiv">
                <div class="NewsHeaderPosition">
                    <asp:Label ForeColor="white" ID="lblTitleSupport" runat="server" Text="Soporte" Font-Bold="true"></asp:Label>
                </div>
            </div>
            <div class="contentNewsDiv">
                <div class="NewsSupportPosition">
                    <asp:Label ID="lblSupportText" runat="server" Font-Bold="true">Obtenga información sobre el producto, actualizaciones y mejoras desde</asp:Label><br />
                    <br />
                    <a id="rbClientsLink" runat="server" href="http://www.robotics.es/clientes.asp" target="_blank" style="font-size: 14px; font-weight: bold;">www.robotics.es</a>
                    <br />
                    <br />
                </div>
            </div>
            <br />

            <div class="headerNewsDiv">
                <div class="NewsHeaderPosition">
                    <asp:Label ForeColor="white" ID="lblTitleNews" runat="server" Text="Noticias" Font-Bold="true"></asp:Label>
                </div>
            </div>
            <div id="newsXmlFeed" class="contentNewsDiv feedSize">
                <div id="loadingFeed" runat="server" class="feedLoading"></div>
            </div>
        </div>

        <div id="divContenido" class="divRightContent StartHeight">
            <div id="divContainerMenu" runat="server" class="maxHeight selectionMenucontainer">
                <!-- Area CAMPOS FICHA EMPLEADO -->
                <div class="panHeader2">
                    <span style="">
                        <asp:Label runat="server" ID="lblPresenciaTitle" Text="Presencia"></asp:Label></span>
                </div>
                <br />
                <table cellpadding="0" cellspacing="0" style="width: 100%; height: 70px" border="0">
                    <tr>
                        <td align="center"></td>
                    </tr>
                </table>
                <br />
                <!-- Area CAMPOS FICHA EMPLEADO -->
                <div class="panHeader2">
                    <span style="">
                        <asp:Label runat="server" ID="lblConfiguracionTitle" Text="Configuración"></asp:Label></span>
                </div>
                <br />
                <table cellpadding="0" cellspacing="0" style="width: 100%; height: 70px" border="0">
                    <tr>
                        <td align="center"></td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
</asp:Content>