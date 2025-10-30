<%@ Page Language="VB" MasterPageFile="~/Base/mastEmp.master" AutoEventWireup="false" Inherits="VTLive40.Status" Title="Opciones de configuración" EnableEventValidation="True" EnableViewState="True" EnableSessionState="True" CodeBehind="Status.aspx.vb" %>

<%@ Register Src="~/Datalink/WebUserControls/WSAdministration.ascx" TagName="frmWsAdmin" TagPrefix="roForms" %>

<asp:Content ID="cMainBody" ContentPlaceHolderID="contentMainBody" runat="Server">

    <script language="javascript" type="text/javascript">
        function PageBase_Load() {
            ConvertControls();

            SelectTab('panDiagnostics');

            top.focus();

            loadDisableHTTPS();
        }

        function SelectTab(SelectedTab) {

            // Hacer invisibles los panels
            $get('panDiagnostics').style.display = 'none';
            $get('panTerminals').style.display = 'none';
            $get('panQueries').style.display = 'none';

            // Desmarcar los botones de la barra
            $get('<%= TABBUTTON_Diagnostics.ClientID %>').className = 'bTab';
            $get('<%= TABBUTTON_Terminals.ClientID %>').className = 'bTab';
            $get('<%= TABBUTTON_Queries.ClientID %>').className = 'bTab';

            var TabID;
            if (SelectedTab == 'panDiagnostics') {
                TabID = 'panDiagnostics';
                $get('<%= TABBUTTON_Diagnostics.ClientID %>').className = 'bTab-active';
                activateTab('Status');
                loadStateServer_Start();
            } else if (SelectedTab == 'panTerminals') {
                TabID = 'panTerminals';
                $get('<%= TABBUTTON_Terminals.ClientID %>').className = 'bTab-active';
                activateTab('Actions');
                loadActions_Start();
            } else if (SelectedTab == 'panQueries') {
                TabID = 'panQueries';
                $get('<%= TABBUTTON_Queries.ClientID %>').className = 'bTab-active';
                activateTab('Queries');
                loadQuerySelector_Start();
            }

            $get(TabID).style.display = 'block';
            $get('<%= ConfigurationOptions_TabVisibleName.ClientID %>').value = TabID;

        }

        function checkOPCPanelClients() {

        }

        //Llença aquest script al recarregar els updatepanels per poder controlar per js els opclient
        function endRequestHandler() {
            checkOPCPanelClients();
        }
    </script>

    <div id="divMainBody">
        <!-- TAB SUPERIOR -->
        <div id="divTabInfo" class="divDataCells">
            <div style="min-height: 10px"></div>
            <div id="divStartRibbon" class="blackRibbonTitle" style="">
                <div class="blackRibbonIcon">
                    <asp:Image ID="imgConfigurationOptions" ImageUrl="Images/AttOptions90.png" runat="server" />
                </div>
                <div class="blackRibbonDescription">
                    <asp:Label ID="lblHeader" runat="server" Text="Diagnosticos VT" CssClass="NameText"></asp:Label>
                    <br />
                    <asp:Label ID="lblInfo" runat="server" Text="Aquí puede revisar el estado de los distintos procesos de la solución."></asp:Label>
                </div>
                <div class="blackRibbonButtons" style="">
                    <table border="0" cellpadding="0" cellspacing="0" width="100%" style="padding: 2px 5px 5px 5px;">
                        <tr>
                            <td style="width: 100px;" valign="middle"></td>
                            <td valign="top" style="padding-top: 10px; padding-bottom: 20px;"></td>
                            <td id="rowTabButtons1" runat="server" align="right" valign="top" style="width: 140px; padding-top: 0px; padding-right: 1px;">
                                <a id="TABBUTTON_Diagnostics" href="javascript: void(0);" class="bTab" onclick="SelectTab('panDiagnostics');" runat="server">
                                    <asp:Label ID="lblTimeFormatOptionsTabButton" Text="Diagnosticos" runat="server" /></a>
                                <a id="TABBUTTON_Terminals" href="javascript: void(0);" class="bTab" onclick="SelectTab('panTerminals');" runat="server">
                                    <asp:Label ID="lblAuditOptionsTabButton" Text="Terminales" runat="server" /></a>
                                <a id="TABBUTTON_Queries" href="javascript: void(0);" class="bTab" onclick="SelectTab('panQueries');" runat="server">
                                    <asp:Label ID="lblWSOptionsTabButton" Text="Consultas bb.dd" runat="server" /></a>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
            <div style="min-height: 10px"></div>
        </div>
        <!-- FIN TAB SUPERIOR -->
        <!-- DETALLE -->
        <div id="divTabData" class="divDataCells">
            <div id="divContenido" class="divAllContent">
                <div id="divContent" style="height: initial;" class="maxHeight">
                    <asp:UpdatePanel ID="upBody" runat="server">
                        <ContentTemplate>
                            <div style="margin: 5px;">
                                <table cellpadding="0" cellspacing="0" style="width: 100%; height: 100%; padding-left: 10px; padding-right: 10px;">
                                    <tr>
                                        <td valign="top" style="padding-top: 2px;">
                                            <!-- Mensajes -->
                                            <div id="divMsgTop" class="divMsg" style="display: none;">
                                                <table style="width: 100%;">
                                                    <tr>
                                                        <td align="center" style="width: 20px; height: 16px;">
                                                            <img id="Img1" src="~/Base/Images/MessageFrame/Alert16.png" runat="server" /></td>
                                                        <td align="left" style="padding-left: 10px; color: white;"><span id="msgTop"></span></td>
                                                        <td align="right" style="color: White; padding-right: 10px;">
                                                            <a href="javascript: void(0);" class="aMsg" onclick=""><span id="lblSaveChanges" runat="server" /></a>
                                                            &nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;
                                                            <a href="javascript: void(0);" onclick="" class="aMsg"><span id="lblUndoChanges" runat="server" /></a>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>
                                        </td>
                                    </tr>

                                    <tr>
                                        <td valign="top">

                                            <div id="panDiagnostics" style="width: 100%;">

                                                <dx:ASPxCallbackPanel ID="diagnosticsPanel" runat="server" ClientInstanceName="diagnosticsPanelClient" ClientSideEvents-EndCallback="diagnosticsPanel_endCallback">
                                                    <ClientSideEvents EndCallback="diagnosticsPanel_endCallback"></ClientSideEvents>
                                                    <PanelCollection>
                                                        <dx:PanelContent ID="ASPxCallbackMainMenuContent" runat="server">
                                                            <div id="div01" class="contentPanel" style="overflow: auto" runat="server" name="menuPanel">
                                                                <!-- Fila con todos los datos númericos-->
                                                                <div style="width: 98%; padding-left: 5px;">
                                                                    <div style="width: calc(50%); float: left">
                                                                        <div class="divRow" style="clear: both">
                                                                            <!-- Conexión con visualtime-->
                                                                            <div style="width: calc(49%); float: left">
                                                                                <div style="width: 150px; float: left; padding-top: 8px">
                                                                                    <asp:Label ID="lblVisualTimeConnection" Font-Bold="true" runat="server" Text="Servidor VisualTime:"></asp:Label>
                                                                                </div>
                                                                                <div style="float: left">
                                                                                    <img id="imgVisualTimeConnection" runat="server" alt="" src="~/Base/Images/Diagnostics/ok.png" width="32" height="32" />
                                                                                </div>
                                                                            </div>
                                                                            <!-- Conexión con sql server-->
                                                                            <div style="width: calc(49%); float: right">
                                                                                <div style="width: 150px; float: left; padding-top: 8px">
                                                                                    <asp:Label ID="lblSqlServerConnection" Font-Bold="true" runat="server" Text="Servidor SQLServer:"></asp:Label>
                                                                                </div>
                                                                                <div style="width: 40px; float: left">
                                                                                    <img id="imgSqlServerConnection" runat="server" alt="" src="~/Base/Images/Diagnostics/ok.png" width="32" height="32" />
                                                                                </div>
                                                                                <div style="padding-top: 8px; float: left">
                                                                                    <asp:TextBox ID="txtSqlServerConnection" ReadOnly="true" runat="server" Text="Si"></asp:TextBox>
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                        <div class="divRow" style="clear: both">
                                                                            <!-- Permisos en carpeta de robotics-->
                                                                            <div style="width: calc(49%); float: left">
                                                                                <div style="width: 250px; float: left; padding-top: 8px">
                                                                                    <asp:Label ID="lblConnectionLost24" Font-Bold="true" runat="server" Text="Conexión SQL estable en las últimas 24 horas:"></asp:Label>
                                                                                </div>
                                                                                <div style="float: left">
                                                                                    <img id="imgConnectionLost24" runat="server" alt="" src="~/Base/Images/Diagnostics/ok.png" width="32" height="32" />
                                                                                </div>
                                                                            </div>
                                                                            <div style="width: calc(49%); float: right">
                                                                                <div style="width: 250px; float: left; padding-top: 8px">
                                                                                    <asp:Label ID="lblHddPermissions" Font-Bold="true" runat="server" Text="Permisos en la carpeta de robotics:"></asp:Label>
                                                                                </div>
                                                                                <div style="float: left">
                                                                                    <img id="imgHddPermissions" runat="server" alt="" src="~/Base/Images/Diagnostics/ok.png" width="32" height="32" />
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                    <div style="width: calc(50%); float: left">
                                                                        <div class="divRow" style="clear: both">
                                                                            <!-- Empleados activos de gestión horaria-->
                                                                            <div style="width: calc(49%); float: left">
                                                                                <div style="width: 250px; float: left; padding-top: 8px">
                                                                                    <asp:Label ID="lblEmployeeCount" Font-Bold="true" runat="server" Text="Empleados activos de gestión horaria:"></asp:Label>
                                                                                </div>
                                                                                <div style="float: left; padding-top: 8px">
                                                                                    <asp:TextBox ID="txtEmployeeCount" runat="server" ReadOnly="true" Text=""></asp:TextBox>
                                                                                </div>
                                                                            </div>
                                                                            <!-- Empleados activos de tareas-->
                                                                            <div style="width: calc(49%); float: right">
                                                                                <div style="width: 250px; float: left; padding-top: 8px">
                                                                                    <asp:Label ID="lblTaskEmployeeCount" Font-Bold="true" runat="server" Text="Empleados activos de tareas:"></asp:Label>
                                                                                </div>
                                                                                <div style="float: left; padding-top: 8px">
                                                                                    <asp:TextBox ID="txtTaskEmployeeCount" runat="server" ReadOnly="true" Text=""></asp:TextBox>
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                        <div class="divRow" style="clear: both">
                                                                            <!-- Solicitudes sin procesar-->
                                                                            <div style="width: calc(49%); float: left">
                                                                                <div style="width: 250px; float: left; padding-top: 8px">
                                                                                    <asp:Label ID="lblRequestCount" Font-Bold="true" runat="server" Text="Solicitudes pendientes de tratar:"></asp:Label>
                                                                                </div>
                                                                                <div style="float: left; padding-top: 8px">
                                                                                    <asp:TextBox ID="txtRequestCount" runat="server" ReadOnly="true" Text=""></asp:TextBox>
                                                                                </div>
                                                                            </div>
                                                                            <!-- Solicitudes sin leer -->
                                                                            <div style="width: calc(49%); float: right">
                                                                                <div style="width: 250px; float: left; padding-top: 8px">
                                                                                    <asp:Label ID="lblSupervisorCount" Font-Bold="true" runat="server" Text="Solicitudes sin leer:"></asp:Label>
                                                                                </div>
                                                                                <div style="float: left; padding-top: 8px">
                                                                                    <asp:TextBox ID="txtSupervisorCount" runat="server" ReadOnly="true" Text=""></asp:TextBox>
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                        <div class="divRow" style="clear: both">
                                                                            <!-- Solicitudes sin enviar -->
                                                                            <div style="width: calc(49%); float: left">
                                                                                <div style="width: 250px; float: left; padding-top: 8px">
                                                                                    <asp:Label ID="lblEmailPending" Font-Bold="true" runat="server" Text="Solicitudes pendientes de enviar:"></asp:Label>
                                                                                </div>
                                                                                <div style="float: left; padding-top: 8px">
                                                                                    <asp:TextBox ID="txtEmailPending" runat="server" ReadOnly="true" Text=""></asp:TextBox>
                                                                                </div>
                                                                            </div>
                                                                            <!-- Abrir dialogo para testMail -->
                                                                            <div style="width: calc(49%); float: right" id="divCheckMail" runat="server">
                                                                                <dx:ASPxButton ID="btTestMail" runat="server" AutoPostBack="False" CausesValidation="False" Text="Comprobar configuración correo" ToolTip="Comprobar configuración correo">
                                                                                    <ClientSideEvents Click="function(s,e){ openTestEmail(); }" />
                                                                                </dx:ASPxButton>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>

                                                                <!--  Fila con dos tablas de procesos y tareas -->
                                                                <div style="width: 98%; padding-left: 5px; padding-top: 10px; clear: both;">
                                                                    <div style="width: calc(50%); float: left">
                                                                        <div class="jsGrid" style="width: calc(100% - 14px)">
                                                                            <asp:Label ID="Label17" runat="server" CssClass="jsGridTitle" Text="Procesos de VisualTime"></asp:Label>
                                                                        </div>
                                                                        <div class="jsGridContent">
                                                                            <dx:ASPxGridView ID="GridProcess" runat="server" AutoGenerateColumns="False" ClientInstanceName="GridProcessClient" KeyboardSupport="True" Width="100%">
                                                                                <Settings ShowTitlePanel="False" VerticalScrollBarMode="Auto" />
                                                                                <SettingsCommandButton>
                                                                                    <DeleteButton Image-Url="~/Base/Images/Grid/remove.png" Image-ToolTip="">
                                                                                        <Image Url="~/Base/Images/Grid/remove.png"></Image>
                                                                                    </DeleteButton>
                                                                                    <UpdateButton Image-Url="~/Base/Images/Grid/save.png" Image-ToolTip="">
                                                                                        <Image Url="~/Base/Images/Grid/save.png"></Image>
                                                                                    </UpdateButton>
                                                                                    <CancelButton Image-Url="~/Base/Images/Grid/cancel.png" Image-ToolTip="">
                                                                                        <Image Url="~/Base/Images/Grid/cancel.png"></Image>
                                                                                    </CancelButton>
                                                                                    <EditButton Image-Url="~/Base/Images/Grid/edit.png" Image-ToolTip="">
                                                                                        <Image Url="~/Base/Images/Grid/edit.png"></Image>
                                                                                    </EditButton>
                                                                                </SettingsCommandButton>
                                                                                <Styles>
                                                                                    <CommandColumn Spacing="5px" />
                                                                                    <Header CssClass="jsGridHeaderCell" />
                                                                                    <Cell Wrap="False" />
                                                                                </Styles>
                                                                                <SettingsPager AlwaysShowPager="true" />
                                                                            </dx:ASPxGridView>
                                                                        </div>
                                                                    </div>

                                                                    <div style="width: calc(50%); float: right">
                                                                        <div class="jsGrid" style="width: calc(100% - 14px)">
                                                                            <asp:Label ID="Label1" runat="server" CssClass="jsGridTitle" Text="Tareas pendientes de VisualTime"></asp:Label>
                                                                        </div>
                                                                        <div class="jsGridContent">
                                                                            <dx:ASPxGridView ID="GridTasks" runat="server" AutoGenerateColumns="False" ClientInstanceName="GridTasksClient" KeyboardSupport="True" Width="100%">
                                                                                <Settings ShowTitlePanel="False" VerticalScrollBarMode="Auto" />
                                                                                <SettingsCommandButton>
                                                                                    <DeleteButton Image-Url="~/Base/Images/Grid/remove.png" Image-ToolTip="">
                                                                                        <Image Url="~/Base/Images/Grid/remove.png"></Image>
                                                                                    </DeleteButton>
                                                                                    <UpdateButton Image-Url="~/Base/Images/Grid/save.png" Image-ToolTip="">
                                                                                        <Image Url="~/Base/Images/Grid/save.png"></Image>
                                                                                    </UpdateButton>
                                                                                    <CancelButton Image-Url="~/Base/Images/Grid/cancel.png" Image-ToolTip="">
                                                                                        <Image Url="~/Base/Images/Grid/cancel.png"></Image>
                                                                                    </CancelButton>
                                                                                    <EditButton Image-Url="~/Base/Images/Grid/edit.png" Image-ToolTip="">
                                                                                        <Image Url="~/Base/Images/Grid/edit.png"></Image>
                                                                                    </EditButton>
                                                                                </SettingsCommandButton>
                                                                                <Styles>
                                                                                    <CommandColumn Spacing="5px" />
                                                                                    <Header CssClass="jsGridHeaderCell" />
                                                                                    <Cell Wrap="False" />
                                                                                </Styles>
                                                                                <SettingsPager AlwaysShowPager="true" />
                                                                            </dx:ASPxGridView>
                                                                        </div>
                                                                    </div>
                                                                </div>

                                                                <!--  Fila con dos tablas de terminales y tareas en ejecución -->
                                                                <div style="width: 98%; padding-left: 5px; padding-top: 10px; clear: both;">
                                                                    <div style="width: calc(50%); float: left">
                                                                        <div class="jsGrid" style="width: calc(100% - 14px)">
                                                                            <asp:Label ID="Label2" runat="server" CssClass="jsGridTitle" Text="Terminales"></asp:Label>
                                                                        </div>
                                                                        <div class="jsGridContent">
                                                                            <dx:ASPxGridView ID="GridTerminales" runat="server" AutoGenerateColumns="False" ClientInstanceName="GridTerminalesClient" KeyboardSupport="True" Width="100%">
                                                                                <Settings ShowTitlePanel="False" VerticalScrollBarMode="Auto" />
                                                                                <SettingsCommandButton>
                                                                                    <DeleteButton Image-Url="~/Base/Images/Grid/remove.png" Image-ToolTip="">
                                                                                        <Image Url="~/Base/Images/Grid/remove.png"></Image>
                                                                                    </DeleteButton>
                                                                                    <UpdateButton Image-Url="~/Base/Images/Grid/save.png" Image-ToolTip="">
                                                                                        <Image Url="~/Base/Images/Grid/save.png"></Image>
                                                                                    </UpdateButton>
                                                                                    <CancelButton Image-Url="~/Base/Images/Grid/cancel.png" Image-ToolTip="">
                                                                                        <Image Url="~/Base/Images/Grid/cancel.png"></Image>
                                                                                    </CancelButton>
                                                                                    <EditButton Image-Url="~/Base/Images/Grid/edit.png" Image-ToolTip="">
                                                                                        <Image Url="~/Base/Images/Grid/edit.png"></Image>
                                                                                    </EditButton>
                                                                                </SettingsCommandButton>
                                                                                <Styles>
                                                                                    <CommandColumn Spacing="5px" />
                                                                                    <Header CssClass="jsGridHeaderCell" />
                                                                                    <Cell Wrap="False" />
                                                                                </Styles>
                                                                                <SettingsPager AlwaysShowPager="true" />
                                                                            </dx:ASPxGridView>
                                                                        </div>
                                                                    </div>

                                                                    <div style="width: calc(50%); float: right">
                                                                        <div class="jsGrid" style="width: calc(100% - 14px)">
                                                                            <asp:Label ID="Label3" runat="server" CssClass="jsGridTitle" Text="Tareas en ejecución"></asp:Label>
                                                                        </div>
                                                                        <div class="jsGridContent">
                                                                            <dx:ASPxGridView ID="GridBackgroundTasks" runat="server" AutoGenerateColumns="False" ClientInstanceName="GridBackgroundTasksClient" KeyboardSupport="True" Width="100%">
                                                                                <Settings ShowTitlePanel="False" VerticalScrollBarMode="Auto" />
                                                                                <SettingsCommandButton>
                                                                                    <DeleteButton Image-Url="~/Base/Images/Grid/remove.png" Image-ToolTip="">
                                                                                        <Image Url="~/Base/Images/Grid/remove.png"></Image>
                                                                                    </DeleteButton>
                                                                                    <UpdateButton Image-Url="~/Base/Images/Grid/save.png" Image-ToolTip="">
                                                                                        <Image Url="~/Base/Images/Grid/save.png"></Image>
                                                                                    </UpdateButton>
                                                                                    <CancelButton Image-Url="~/Base/Images/Grid/cancel.png" Image-ToolTip="">
                                                                                        <Image Url="~/Base/Images/Grid/cancel.png"></Image>
                                                                                    </CancelButton>
                                                                                    <EditButton Image-Url="~/Base/Images/Grid/edit.png" Image-ToolTip="">
                                                                                        <Image Url="~/Base/Images/Grid/edit.png"></Image>
                                                                                    </EditButton>
                                                                                </SettingsCommandButton>
                                                                                <Styles>
                                                                                    <CommandColumn Spacing="5px" />
                                                                                    <Header CssClass="jsGridHeaderCell" />
                                                                                    <Cell Wrap="False" />
                                                                                </Styles>
                                                                                <SettingsPager AlwaysShowPager="true" />
                                                                            </dx:ASPxGridView>
                                                                        </div>
                                                                    </div>
                                                                </div>

                                                                <!--  Fila con dos sesiones y tareas -->
                                                                <div style="width: 98%; padding-left: 5px; padding-top: 10px; clear: both;">
                                                                    <div style="width: calc(50%); float: left">
                                                                        <div class="jsGrid" style="width: calc(100% - 14px)">
                                                                            <asp:Label ID="Label4" runat="server" CssClass="jsGridTitle" Text="Estado procesos de cálculo"></asp:Label>
                                                                        </div>
                                                                        <div class="jsGridContent">
                                                                            <dx:ASPxGridView ID="GridProcCounters" runat="server" AutoGenerateColumns="False" ClientInstanceName="GridProcCountersClient" KeyboardSupport="True" Width="100%">
                                                                                <Settings ShowTitlePanel="False" VerticalScrollBarMode="Auto" />
                                                                                <SettingsCommandButton>
                                                                                    <DeleteButton Image-Url="~/Base/Images/Grid/remove.png" Image-ToolTip="">
                                                                                        <Image Url="~/Base/Images/Grid/remove.png"></Image>
                                                                                    </DeleteButton>
                                                                                    <UpdateButton Image-Url="~/Base/Images/Grid/save.png" Image-ToolTip="">
                                                                                        <Image Url="~/Base/Images/Grid/save.png"></Image>
                                                                                    </UpdateButton>
                                                                                    <CancelButton Image-Url="~/Base/Images/Grid/cancel.png" Image-ToolTip="">
                                                                                        <Image Url="~/Base/Images/Grid/cancel.png"></Image>
                                                                                    </CancelButton>
                                                                                    <EditButton Image-Url="~/Base/Images/Grid/edit.png" Image-ToolTip="">
                                                                                        <Image Url="~/Base/Images/Grid/edit.png"></Image>
                                                                                    </EditButton>
                                                                                </SettingsCommandButton>
                                                                                <Styles>
                                                                                    <CommandColumn Spacing="5px" />
                                                                                    <Header CssClass="jsGridHeaderCell" />
                                                                                    <Cell Wrap="False" />
                                                                                </Styles>
                                                                                <SettingsPager AlwaysShowPager="true" />
                                                                            </dx:ASPxGridView>
                                                                        </div>
                                                                    </div>

                                                                    <div style="width: calc(50%); float: right">
                                                                        <div class="jsGrid" style="width: calc(100% - 14px)">
                                                                            <asp:Label ID="Label5" runat="server" CssClass="jsGridTitle" Text="Estado sesiones concurrentes VisualTime"></asp:Label>
                                                                        </div>
                                                                        <div class="jsGridContent">
                                                                            <dx:ASPxGridView ID="GridActiveSessions" runat="server" AutoGenerateColumns="False" ClientInstanceName="GridActiveSessionsClient" KeyboardSupport="True" Width="100%">
                                                                                <Settings ShowTitlePanel="False" VerticalScrollBarMode="Auto" />
                                                                                <SettingsCommandButton>
                                                                                    <DeleteButton Image-Url="~/Base/Images/Grid/remove.png" Image-ToolTip="">
                                                                                        <Image Url="~/Base/Images/Grid/remove.png"></Image>
                                                                                    </DeleteButton>
                                                                                    <UpdateButton Image-Url="~/Base/Images/Grid/save.png" Image-ToolTip="">
                                                                                        <Image Url="~/Base/Images/Grid/save.png"></Image>
                                                                                    </UpdateButton>
                                                                                    <CancelButton Image-Url="~/Base/Images/Grid/cancel.png" Image-ToolTip="">
                                                                                        <Image Url="~/Base/Images/Grid/cancel.png"></Image>
                                                                                    </CancelButton>
                                                                                    <EditButton Image-Url="~/Base/Images/Grid/edit.png" Image-ToolTip="">
                                                                                        <Image Url="~/Base/Images/Grid/edit.png"></Image>
                                                                                    </EditButton>
                                                                                </SettingsCommandButton>
                                                                                <Styles>
                                                                                    <CommandColumn Spacing="5px" />
                                                                                    <Header CssClass="jsGridHeaderCell" />
                                                                                    <Cell Wrap="False" />
                                                                                </Styles>
                                                                                <SettingsPager AlwaysShowPager="true" />
                                                                            </dx:ASPxGridView>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </dx:PanelContent>
                                                    </PanelCollection>
                                                </dx:ASPxCallbackPanel>
                                            </div>

                                            <div id="panTerminals" style="width: 100%; display: none">
                                                <div id="divActions" style="width: 98%; padding-left: 5px">
                                                    <div>
                                                        <div class="divRow" style="clear: both">
                                                            <div style="float: left" id="divDisableHttps" runat="server">
                                                                <!-- Botón para eliminar https VTLiveApi -->
                                                                <div style="width: 250px; float: left; padding-top: 8px">
                                                                    <span id="lblDisableHTTPS">Deshabilitar HTTPS en VTLiveApi</span>
                                                                </div>
                                                                <div style="padding-top: 8px; float: left">
                                                                    <input type="button" id="btnDisableHTTPS" value="Deshabilitar" onclick="btnDisableHTTPS_Click()" />
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="divRow" style="clear: both" id="divDownloadLogs" runat="server">
                                                            <div style="float: left">
                                                                <!-- Botón para descargar logs -->
                                                                <div style="width: 250px; float: left; padding-top: 8px">
                                                                    <asp:Label ID="lblDescargarLogs" Font-Bold="true" runat="server" Text="Descargar logs"></asp:Label>
                                                                </div>
                                                                <div style="padding-top: 8px; float: left">
                                                                    <asp:Button ID="btnDescargarLogs" runat="server" Text="Descargar" OnClientClick="btnDescargarLogs_Click();return false;"></asp:Button>
                                                                </div>
                                                            </div>
                                                        </div>

                                                        <div class="divRow" style="clear: both" id="divRegisterMTTerminal" runat="server">
                                                            <div style="float: left">
                                                                <!-- Botón para descargar logs -->
                                                                <div style="width: 250px; float: left; padding-top: 8px">
                                                                    <asp:Label ID="Label13" Font-Bold="true" runat="server" Text="Registrar terminal en MT"></asp:Label>
                                                                </div>
                                                                <div style="width: 750px; float: left; padding-top: 8px">
                                                                    <div style="float: left">
                                                                        <dx:ASPxComboBox ID="cmbTerminalType" runat="server" Width="170px" ClientInstanceName="cmbTerminalTypeClient">
                                                                            <ValidationSettings Display="None" />
                                                                        </dx:ASPxComboBox>
                                                                    </div>
                                                                    <div style="padding-left: 8px; float: left">
                                                                        <dx:ASPxTextBox runat="server" ID="txtSerialNumberId" Width="150" ClientInstanceName="txtSerialNumberIdClient">
                                                                            <ValidationSettings Display="None" />
                                                                        </dx:ASPxTextBox>
                                                                    </div>
                                                                    <div style="padding-left: 8px; float: left">
                                                                        <dx:ASPxButton ID="btnRegisterTerminal" runat="server" AutoPostBack="False" CausesValidation="False" Text="Registrar" ToolTip="Registrar">
                                                                            <ClientSideEvents Click="function(s,e){ registerTerminalMT(); }" />
                                                                        </dx:ASPxButton>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>

                                                        <div class="divRow" style="clear: both" id="divMx9Parameter" runat="server">
                                                            <div style="float: left">
                                                                <!-- Botón para descargar logs -->
                                                                <div style="width: 250px; float: left; padding-top: 8px">
                                                                    <asp:Label ID="Label16" Font-Bold="true" runat="server" Text="Guardar parámetro mx9"></asp:Label>
                                                                </div>
                                                                <div style="width: 750px; float: left; padding-top: 8px">
                                                                    <div style="padding-left: 8px; float: left">
                                                                        <dx:ASPxSpinEdit SpinButtons-ClientVisible="true" ID="mx9IdTerminal" ClientInstanceName="mx9IdTerminalClient" runat="server" Width="100px"></dx:ASPxSpinEdit>
                                                                    </div>
                                                                    <div style="padding-left: 8px; float: left">
                                                                        <asp:Label ID="Label18" Font-Bold="true" runat="server" Text="Nombre parámetro:"></asp:Label>
                                                                    </div>
                                                                    <div style="padding-left: 8px; float: left">
                                                                        <dx:ASPxTextBox runat="server" ID="txtMx9ParameterName" Width="150" ClientInstanceName="txtMx9ParameterNameClient">
                                                                            <ValidationSettings Display="None" />
                                                                        </dx:ASPxTextBox>
                                                                    </div>
                                                                    <div style="float: left">
                                                                        <asp:Label ID="Label19" Font-Bold="true" runat="server" Text="Valor parámetro:"></asp:Label>
                                                                    </div>
                                                                    <div style="padding-left: 8px; float: left">
                                                                        <dx:ASPxTextBox runat="server" ID="txtMx9ParameterValue" Width="150" ClientInstanceName="txtMx9ParameterValueClient">
                                                                            <ValidationSettings Display="None" />
                                                                        </dx:ASPxTextBox>
                                                                    </div>
                                                                    <div style="padding-left: 8px; float: left">
                                                                        <dx:ASPxButton ID="ASPxButton3" runat="server" AutoPostBack="False" CausesValidation="False" Text="Registrar" ToolTip="Registrar">
                                                                            <ClientSideEvents Click="function(s,e){ saveMx9Parameter(); }" />
                                                                        </dx:ASPxButton>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>

                                                        <div class="divRow" style="clear: both">
                                                            <div style="float: left">
                                                                <!-- Botón para realizar volcado de terminales -->
                                                                <div style="width: 250px; float: left; padding-top: 8px">
                                                                    <asp:Label ID="Label6" Font-Bold="true" runat="server" Text="Realizar volcado completo de terminal"></asp:Label>
                                                                </div>
                                                                <div style="padding-top: 8px; float: left">
                                                                    <div style="float: left">
                                                                        <dx:ASPxSpinEdit SpinButtons-ClientVisible="false" ID="txtTerminal" runat="server" Width="100px" ClientIDMode="Predictable"></dx:ASPxSpinEdit>
                                                                    </div>
                                                                    <div style="float: left">
                                                                        <dx:ASPxButton ID="btnResetTerminal" runat="server" AutoPostBack="False" CausesValidation="False" Text="Iniciar" ToolTip="Registrar">
                                                                            <ClientSideEvents Click="function(s,e){ btnResetTerminal_Click(); }" />
                                                                        </dx:ASPxButton>
                                                                    </div>
                                                                </div>
                                                                <!-- Botón para reiniciar terminales rx -->
                                                                <div style="width: 250px; float: left; padding-top: 8px">
                                                                    <asp:Label ID="lblRebootTerminal" Font-Bold="true" runat="server" Text="Reiniciar terminal rx"></asp:Label>
                                                                </div>
                                                                <div style="padding-top: 8px; float: left">
                                                                    <div style="float: left">
                                                                        <dx:ASPxSpinEdit SpinButtons-ClientVisible="false" ID="txtTerminalReboot" runat="server" Width="100px" ClientIDMode="Predictable"></dx:ASPxSpinEdit>
                                                                    </div>
                                                                    <div style="float: left">
                                                                        <dx:ASPxButton ID="btnRebootTerminal" runat="server" AutoPostBack="False" CausesValidation="False" Text="Iniciar" ToolTip="Reiniciar">
                                                                            <ClientSideEvents Click="function(s,e){ btnRebootTerminal_Click(); }" />
                                                                        </dx:ASPxButton>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>

                                                        <div class="divRow" style="clear: both" id="divVTCFile" runat="server">
                                                            <div style="float: left">
                                                                <!-- Botón para descargar logs -->
                                                                <div style="width: 250px; float: left; padding-top: 8px">
                                                                    <asp:Label ID="Label14" Font-Bold="true" runat="server" Text="Importar fichero de correlación"></asp:Label>
                                                                </div>
                                                                <div style="padding-top: 8px; float: left">
                                                                    <div>
                                                                        <asp:Label ID="Label15" Font-Bold="true" runat="server" Text="Abra el fichero VTC con el editor de texto preferido y pegue el contenido en el siguiente espacio:"></asp:Label>
                                                                    </div>
                                                                    <div>
                                                                        <dx:ASPxMemo ID="vtcContent" runat="server" ClientInstanceName="vtcContentClient" Text="" Width="350px" Rows="35" />
                                                                    </div>
                                                                    <div>
                                                                        <dx:ASPxButton ID="ASPxButton2" runat="server" AutoPostBack="False" CausesValidation="False" Text="Importar" ToolTip="Importar">
                                                                            <ClientSideEvents Click="function(s,e){ importVTCFile(); }" />
                                                                        </dx:ASPxButton>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div id="panQueries" style="width: 100%; display: none;">
                                                <!-- Fila con todos los datos númericos-->
                                                <div style="width: 98%; padding-left: 5px;">
                                                    <div class="query">
                                                        <div id="querySelectorPlaceholder" class="dx-fieldset">
                                                            <div class="divRow">
                                                                <div class="divRowDescription"></div>
                                                                <span class="labelForm" style="margin-top: 4px;">Consulta: </span>
                                                                <div class="componentForm" id="querySelector"></div>
                                                            </div>
                                                            <div class="divRow">
                                                                <div class="divRowDescription"></div>
                                                                <span class="labelForm" style="margin-top: 4px;">Descripción: </span>
                                                                <div class="componentForm"><span id="descQuery" /></div>
                                                            </div>
                                                            <div>
                                                                <div id="parameters"></div>
                                                            </div>
                                                            <div style="float: right; padding-right: 30px;">
                                                                <div id="sendQuery" class="dxbButton btnFlat btnFlatBlack"></div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div id="grid" style="clear: both; padding-top: 20px;">
                                                        <div id="dxGrid" />
                                                    </div>
                                                </div>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <!-- Mensajes -->
                                            <div id="divMsgBottom" class="divMsg" style="display: none;">
                                                <table style="width: 100%;">
                                                    <tr>
                                                        <td align="center" style="width: 20px; height: 16px;">
                                                            <img id="Img2" src="~/Base/Images/MessageFrame/Alert16.png" runat="server" /></td>
                                                        <td align="left" style="padding-left: 10px; color: white;"><span id="msgBottom"></span></td>
                                                        <td align="right" style="color: White; padding-right: 10px;">
                                                            <a href="javascript: void(0);" class="aMsg" onclick="saveChanges();"><span id="lblSaveChangesBottom" runat="server" /></a>
                                                            &nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;
                                                            <a href="javascript: void(0);" onclick="undoChanges();" class="aMsg"><span id="lblUndoChangesBottom" runat="server" /></a>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>
    <asp:HiddenField ID="ConfigurationOptions_TabVisibleName" Value="" runat="server" />

    <dx:ASPxPopupControl ID="CheckEmailPopup" runat="server" AllowDragging="False" CloseAction="None" Modal="True"
        PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter" Width="500px" Height="350px"
        ShowHeader="False" ScrollBars="Auto" ShowPageScrollbarWhenModal="True" ClientInstanceName="CheckEmailPopupClient"
        PopupAnimationType="None" BackColor="Transparent" ContentStyle-Paddings-Padding="0px" Border-BorderColor="Transparent" ShowShadow="false">
        <SettingsLoadingPanel Enabled="false" />
        <ContentCollection>
            <dx:PopupControlContentControl ID="PopupSaveViewControlContent" runat="server">
                <dx:ASPxCallbackPanel ID="testMailPanel" runat="server" ClientInstanceName="testMailPanelClient" ClientSideEvents-EndCallback="testMailPanel_endCallback">
                    <PanelCollection>
                        <dx:PanelContent ID="PanelContent1" runat="server">
                            <div class="bodyPopupExtended" style="table-layout: fixed; height: 225px; width: 350px;">
                                <div style="float: left">
                                    <div class="divRow">
                                        <div style="width: 100px; float: left;">
                                            <asp:Label ID="Label7" Font-Bold="true" runat="server" Text="Destinación:"></asp:Label>
                                        </div>
                                        <div style="float: left">
                                            <asp:TextBox ID="txtDestination" runat="server" Text=""></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="divRow">
                                        <div style="width: 100px; float: left;">
                                            <asp:Label ID="Label8" Font-Bold="true" runat="server" Text="Cabecera:"></asp:Label>
                                        </div>
                                        <div style="float: left">
                                            <asp:TextBox ID="txtHeader" runat="server" Text=""></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="divRow">
                                        <div style="width: 100px; float: left;">
                                            <asp:Label ID="Label9" Font-Bold="true" runat="server" Text="Cuerpo:"></asp:Label>
                                        </div>
                                        <div style="float: left">
                                            <asp:TextBox ID="txtBody" runat="server" Text=""></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="divRow">
                                        <asp:Label ID="Label10" Font-Bold="true" runat="server" Text="Introduce el contracódigo subministrado por robotics:"></asp:Label>
                                    </div>
                                    <div class="divRow" align="center">
                                        <asp:Label ID="lblRoboticsCode" Font-Bold="true" Font-Size="XX-Large" runat="server" Text=""></asp:Label>
                                    </div>
                                    <div class="divRow" align="center">
                                        <div style="float: left; width: 300px;">
                                            <asp:TextBox ID="txtValidationCode" runat="server" Text=""></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="divRow" style="width: 300px;">
                                        <dx:ASPxButton ID="ASPxButton1" ClientInstanceName="btTestMailClient" runat="server" AutoPostBack="False" CausesValidation="False" Text="Enviar correo" ToolTip="Enviar correo"
                                            HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                            <ClientSideEvents Click="function(s,e){ checkEmail(); }" />
                                        </dx:ASPxButton>

                                        <dx:ASPxButton ID="btCloseTestMail" ClientInstanceName="btCloseTestMailClient" runat="server" AutoPostBack="False" CausesValidation="False" Text="Cerrar" ToolTip="Cerrar"
                                            HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                            <ClientSideEvents Click="function(s,e){ closeTestEmail(); }" />
                                        </dx:ASPxButton>
                                    </div>
                                    <div class="divRow">
                                        <div style="width: 100px; float: left;">
                                            <asp:Label ID="Label11" Font-Bold="true" runat="server" Text="Estado:"></asp:Label>
                                        </div>
                                        <div style="float: left">
                                            <asp:TextBox ID="txtMailWorking" Width="100%" runat="server" Text=""></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </dx:PanelContent>
                    </PanelCollection>
                </dx:ASPxCallbackPanel>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>

    <dx:ASPxCallback ID="CallbackSession" runat="server" ClientInstanceName="CallbackSessionClient" ClientSideEvents-CallbackComplete="diagnosticsPanel_endCallback"></dx:ASPxCallback>
</asp:Content>