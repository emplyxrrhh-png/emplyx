<%@ Page Language="VB" MasterPageFile="~/Base/mastEmp.master" AutoEventWireup="false" Inherits="VTLive40.ConfigurationOptions" Title="Opciones de configuración" EnableEventValidation="True" EnableViewState="True" EnableSessionState="True" CodeBehind="ConfigurationOptions.aspx.vb" %>

<%@ Register Src="~/Datalink/WebUserControls/WSAdministration.ascx" TagName="frmWsAdmin" TagPrefix="roForms" %>

<asp:Content ID="cMainBody" ContentPlaceHolderID="contentMainBody" runat="Server">

    <script language="javascript" type="text/javascript">        

        var chkDisableBiometricData;
        var chkDisableBiometricDataChanged;
        var chkA3PayrollChanged;
        var BASE_URL = "<%= ConfigurationManager.AppSettings("RootUrl") %>";

        function PageBase_Load() {
            ConvertControls();
            var jsGridIPs = null;

            $get('panTimeFormatOptions').style.display = 'block';
            $get('panDatabaseOptions').style.display = 'none';
            $get('panMovesOptions').style.display = 'none';
            $get('panStartYearOptions').style.display = 'none';
            $get('panWsConfig').style.display = 'none';       
            $get('panWebLinks').style.display = 'none';
            $get('panDocumentOptions').style.display = 'none';

            // Reestablezco el tab activo
            if ($get('<%= ConfigurationOptions_TabVisibleName.ClientID %>').value != '') {
                SelectTab($get('<%= ConfigurationOptions_TabVisibleName.ClientID %>').value);
            }
            else {
                SelectTab('panTimeFormatOptions');
            }

            checkOPCPanelClients();

            //Enllaç dels OptionPanelClients
            linkOPCItems('<%= optTimeFormatNumeric.ClientID %>,<%= optTimeFormatStandard.ClientID %>');
            checkOnChangeOPanel('<%= optTimeFormatNumeric.ClientID %>', '0', 'false');
            checkOnChangeOPanel('<%= optTimeFormatStandard.ClientID %>', '0', 'false');

            linkOPCItems('<%= rbESDetectionAdvanced.ClientID %>,<%= rbESDetectionClassic.ClientID %>');

            if ($get('<%= hdnChanged.ClientID %>').value == '0' || $get('<%= hdnChanged.ClientID %>').value == '') {
                hasChanges(false);
            } else {
                hasChanges(true);
            }

            top.focus();

            chkDisableBiometricData = document.getElementById("ctl00_contentMainBody_tcMovesOptions_tpMovesOptions_BiometricDate_chkDisableBiometricData");
            chkDisableBiometricData.onchange = function () {
                chkDisableBiometricDataChanged = true;
                hasChanges(true);
            };
            chkA3PayrollChanged = false;
            chkA3Payroll = document.getElementById("ctl00_contentMainBody_chkA3Payroll");
            chkA3Payroll.onchange = function () {
                chkA3PayrollChanged = true;
                hasChanges(true);
            };
        }

        function showDisableBiometricDataConfirmation(callback) {

            var url = "Security/srvMsgBoxSecurity.aspx?action=Message&TitleKey=DisableBiometricData.Confirm.Text&" +
                "DescriptionKey=DisableBiometricData.Confirm.Description&" +
                "Option1TextKey=DisableBiometricData.Confirm.Option1Text&" +
                "Option1DescriptionKey=DisableBiometricData.Confirm.Option1Description&" +
                "Option2TextKey=DisableBiometricData.Confirm.Option2Text&" +
                "Option2DescriptionKey=DisableBiometricData.Confirm.Option2Description&" +
                "Option1OnClickScript=parent.showLoader(true);DisableBiometricDataConfirmation(); return false;&" +
                "Option2OnClickScript=HideMsgBoxForm();return false;&" +
                "IconUrl=~/Base/Images/MessageFrame/dialog-question.png";
            if (parent.ShowMsgBoxForm !== undefined) {
                parent.ShowMsgBoxForm(url, 400, 300, '');
            } else {
                parent.parent.ShowMsgBoxForm(url, 400, 300, '');
            }
        }

        function showUnregisterA3PayrollIfNeeded(callback) {
            if (!chkA3Payroll.checked) {
                var url = "Security/srvMsgBoxSecurity.aspx?action=Message&TitleKey=UnregisterA3Payroll.Confirm.Text&" +
                    "DescriptionKey=UnregisterA3Payroll.Confirm.Description&" +
                    "Option1TextKey=UnregisterA3Payroll.Confirm.Option1Text&" +
                    "Option1DescriptionKey=UnregisterA3Payroll.Confirm.Option1Description&" +
                    "Option1OnClickScript=DisableUnregisterA3PayrollConfirmation(); return false;&" +
                    "IconUrl=~/Base/Images/MessageFrame/dialog-warning.png";
                if (parent.ShowMsgBoxForm !== undefined) {
                    parent.ShowMsgBoxForm(url, 400, 300, '');
                } else {
                    parent.parent.ShowMsgBoxForm(url, 400, 300, '');
                }
            }
        }


        function SelectTab(SelectedTab) {

            // Hacer invisibles los panels
            $get('panTimeFormatOptions').style.display = 'none';
            $get('panDatabaseOptions').style.display = 'none';
            $get('panMovesOptions').style.display = 'none';
            $get('panStartYearOptions').style.display = 'none';
            $get('panWsConfig').style.display = 'none';    
            $get('panWebLinks').style.display = 'none'; 
            $get('panDocumentOptions').style.display = 'none';    

            // Desmarcar los botones de la barra
            $get('<%= TABBUTTON_TimeFormatOptions.ClientID %>').className = 'bTab';
            $get('<%= TABBUTTON_DatabaseOptions.ClientID %>').className = 'bTab';
            $get('<%= TABBUTTON_MovesOptions.ClientID %>').className = 'bTab';
            $get('<%= TABBUTTON_StartYearOptions.ClientID %>').className = 'bTab';
            $get('<%= TABBUTTON_WsConfig.ClientID %>').className = 'bTab';     
            $get('<%= TABBUTTON_WebLinks.ClientID %>').className = 'bTab';     
            $get('<%= TABBUTTON_Document.ClientID %>').className = 'bTab';  

            var TabID;
            if (SelectedTab == 'panTimeFormatOptions') {
                TabID = 'panTimeFormatOptions';
                $get('<%= TABBUTTON_TimeFormatOptions.ClientID %>').className = 'bTab-active';
            } else if (SelectedTab == 'panDatabaseOptions') {
                TabID = 'panDatabaseOptions';
                $get('<%= TABBUTTON_DatabaseOptions.ClientID %>').className = 'bTab-active';
            } else if (SelectedTab == 'panMovesOptions') {
                TabID = 'panMovesOptions';
                $get('<%= TABBUTTON_MovesOptions.ClientID %>').className = 'bTab-active';
                loadMovesRelatedInfo();
            } else if (SelectedTab == 'panStartYearOptions') {
                TabID = 'panStartYearOptions';
                $get('<%= TABBUTTON_StartYearOptions.ClientID %>').className = 'bTab-active';
            } else if (SelectedTab == 'panWsConfig') {
                TabID = 'panWsConfig';
                $get('<%= TABBUTTON_WsConfig.ClientID %>').className = 'bTab-active';
                createWSElements(null, 'ctl00_contentMainBody_ASPxCallbackPanelContenido_frmWsAdmin');
            } else if (SelectedTab == 'panWebLinks') {
                TabID = 'panWebLinks';
                $get('<%= TABBUTTON_WebLinks.ClientID %>').className = 'bTab-active';
                loadWebLinks();
            } else if (SelectedTab == 'panDocumentOptions') {
                TabID = 'panDocumentOptions';
                $get('<%= TABBUTTON_Document.ClientID %>').className = 'bTab-active';
                //loadWebLinks();
            }
            $get(TabID).style.display = 'block';
            $get('<%= ConfigurationOptions_TabVisibleName.ClientID %>').value = TabID;

        }

        function RefreshScreen(DataType) {
            ButtonClick($get('<%= btRefresh.ClientID %>'));
        }

        function checkOPCPanelClients() {
            venableOPC('<%= optTimeFormatNumeric.ClientID %>');
            venableOPC('<%= optTimeFormatStandard.ClientID %>');

            venableOPC('<%= rbESDetectionAdvanced.ClientID %>');
            venableOPC('<%= rbESDetectionClassic.ClientID %>');
        }

        //Llença aquest script al recarregar els updatepanels per poder controlar per js els opclient
        function endRequestHandler() {
            checkOPCPanelClients();
        }

        function SaveChanges() {
            showLoadingGrid(true);

            if (typeof jsGridIPs != 'undefined' && jsGridIPs != null) {
                //Bucle per totes les composicions
                var ipRows = jsGridIPs.getRows();
                if (ipRows != null) {
                    var arrAllowdIp = "";
                    for (var x = 0; x < ipRows.length; x++) {
                        //Bucle per els camps
                        if (x != 0) arrAllowdIp = arrAllowdIp + "#";
                        arrAllowdIp = arrAllowdIp + jsGridIPs.retRowJSON(ipRows[x].id)[1].value
                    }
                    if (arrAllowdIp != "") {
                        document.getElementById("ctl00_contentMainBody_txtAllowedIPs").value = arrAllowdIp;
                    }
                }
            }
            __doPostBack('<%= btSave.ClientID %>');
        }

        function beforeSaveChanges() {
            try {
                if (CheckSave() == true) {
                    if (chkDisableBiometricDataChanged && chkDisableBiometricData.checked)
                        showDisableBiometricDataConfirmation();
                    else
                        SaveChanges();
                }

            } catch (e) { showError("saveChanges", e); }
        }

        function undoChanges() {
            try {
                __doPostBack('<%= btCancel.ClientID %>');
                //hasChanges(false);
            } catch (e) { showError("undoChanges", e); }
        }

        function DeleteBiometricData() {
            try {
                var stamp = '&StampParam=' + new Date().getMilliseconds();

                ajax = nuevoAjax();
                ajax.open("GET", "../Employees/Handlers/srvEmployees.ashx?action=DeleteBiometricsAllEmployees" + stamp, true);
                ajax.onreadystatechange = function () {
                    if (ajax.readyState == 4) {
                        parent.showLoader(false);
                        var strResponse = ajax.responseText;

                        if (strResponse.substr(0, 7) == 'MESSAGE') {
                            var url = "Employees/srvMsgBoxEmployees.aspx?action=Message&Parameters=" + encodeURIComponent(strResponse.substr(7, strResponse.length - 7));
                            parent.ShowMsgBoxForm(url, 500, 300, '');
                        }
                        else {
                            if (strResponse == 'OK') {
                                loadMovesRelatedInfo();
                                //TODO HA IDO BIEN
                                var message = "TitleKey=DeleteBiometricsAllEmployees.OKProcess.Text&" +
                                    "DescriptionKey=DeleteBiometricsAllEmployees.OKProcess.Description&" +
                                    "Option1TextKey=DeleteBiometricsAllEmployees.OKProcess.Option1Text&" +
                                    "Option1DescriptionKey=DeleteBiometricsAllEmployees.OKProcess.Option1Description&" +
                                    "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                                    "IconUrl=~/Base/Images/MessageFrame/dialog-information.png"
                                var url = "Employees/srvMsgBoxEmployees.aspx?action=Message&Parameters=" + encodeURIComponent(message);
                                parent.ShowMsgBoxForm(url, 500, 300, '');
                            } else {
                                var url = "Employees/srvMsgBoxEmployees.aspx?action=Message&Parameters=" + encodeURIComponent(strResponse.split("MESSAGE")[1]);
                                parent.ShowMsgBoxForm(url, 500, 300, '');
                            }
                        }
                    }
                }
                ajax.send(null)
            }
            catch (e) {
                showError("DeleteBiometricsAllEmployees: ", e);
            }
        }

        function showDeleteBiometricDataConfirmation(callback) {

            var url = "Security/srvMsgBoxSecurity.aspx?action=Message&TitleKey=DeleteBiometricData.Confirm.Text&" +
                "DescriptionKey=DeleteBiometricData.Confirm.Description&" +
                "Option1TextKey=DeleteBiometricData.Confirm.Option1Text&" +
                "Option1DescriptionKey=DeleteBiometricData.Confirm.Option1Description&" +
                "Option2TextKey=DeleteBiometricData.Confirm.Option2Text&" +
                "Option2DescriptionKey=DeleteBiometricData.Confirm.Option2Description&" +
                "Option1OnClickScript=parent.showLoader(true);DeleteBiometricDataConfirmation(); return false;&" +
                "Option2OnClickScript=HideMsgBoxForm();return false;&" +
                "IconUrl=~/Base/Images/MessageFrame/dialog-question.png";
            if (parent.ShowMsgBoxForm !== undefined) {
                parent.ShowMsgBoxForm(url, 400, 300, '');
            } else {
                parent.parent.ShowMsgBoxForm(url, 400, 300, '');
            }
        }
    </script>

    <input type="hidden" runat="server" id="hdnModeEdit" value="" />
    <input type="hidden" id="msgHasChanges" value="" runat="server" clientidmode="Static" />
    <input type="hidden" id="msgHasErrors" value="" runat="server" clientidmode="Static" />
    <input type="hidden" runat="server" id="hdnValueGridName" />
    <input type="hidden" runat="server" id="txtAllowedIPs" />
    <input type="hidden" runat="server" id="txtCertificateHeader" />
    <input type="hidden" runat="server" id="txtCreatedByHeader" />
    <input type="hidden" runat="server" id="txtCreatedAtHeader" />
    <input type="hidden" runat="server" id="txtLinkHeader" />

    <div id="divMainBody">
        <!-- TAB SUPERIOR -->
        <div id="divTabInfo" class="divDataCells">
            <div style="min-height: 10px"></div>
            <div id="divStartRibbon" class="blackRibbonTitle" style="">
                <div class="blackRibbonIcon">
                    <asp:Image ID="imgConfigurationOptions" ImageUrl="Images/AttOptions90.png" runat="server" />
                </div>
                <div class="blackRibbonDescription">
                    <asp:Label ID="lblHeader" runat="server" Text="Opciones generales" CssClass="NameText"></asp:Label>
                    <br />
                    <asp:Label ID="lblInfo" runat="server" Text="Esta es la pantalla de opciones generales. Elija la categoría de configuración en la lista de la derecha y las opciones aparecerán aqui debajo. Tenga en cuenta que algunas opciones no pueden modificarse o activarse según la configuración del servidor de VisualTime o la versión utilizada."></asp:Label>
                </div>
                <div class="blackRibbonButtons" style="">
                    <table border="0" cellpadding="0" cellspacing="0" width="100%" style="padding: 2px 5px 5px 5px;">
                        <tr>
                            <td style="width: 100px;" valign="middle"></td>
                            <td valign="top" style="padding-top: 10px; padding-bottom: 20px;"></td>

                            <td id="rowTabButtons1" runat="server" align="right" valign="top" style="width: 140px; padding-top: 0px; padding-right: 1px;">
                                <a id="TABBUTTON_TimeFormatOptions" href="javascript: void(0);" class="bTab" onclick="SelectTab('panTimeFormatOptions');" runat="server">
                                    <asp:Label ID="lblTimeFormatOptionsTabButton" Text="Formatos de tiempos" runat="server" /></a>
                            </td>
                            <td id="rowTabButtons2" runat="server" align="right" valign="top" style="width: 140px; padding-top: 0px; padding-right: 1px;">
                                <a id="TABBUTTON_WsConfig" href="javascript: void(0);" class="bTab" onclick="SelectTab('panWsConfig');" runat="server">
                                    <asp:Label ID="lblWSOptionsTabButton" Text="Gestión de secretos" runat="server" /></a>    
                                <a id="TABBUTTON_WebLinks" href="javascript: void(0);" class="bTab" onclick="SelectTab('panWebLinks');" runat="server">
                                    <asp:Label ID="lblWebLinks" Text="Enlaces" runat="server" /></a>
                                <a id="TABBUTTON_DatabaseOptions" href="javascript: void(0);" class="bTab" onclick="SelectTab('panDatabaseOptions');" runat="server">
                                    <asp:Label ID="lblAuditOptionsTabButton" Text="Auditoría de datos" runat="server" /></a>
                            </td>
                            <td id="rowTabButtons3" runat="server" align="right" valign="top" style="width: 140px; padding-top: 0px; padding-right: 1px;">
                                <a id="TABBUTTON_MovesOptions" href="javascript: void(0);" class="bTab" onclick="SelectTab('panMovesOptions');" runat="server">
                                    <asp:Label ID="lblMovesOptionsTabButton" Text="Opciones de ${Punches}" runat="server" /></a>
                                <a id="TABBUTTON_StartYearOptions" href="javascript: void(0);" class="bTab" onclick="SelectTab('panStartYearOptions');" runat="server">
                                    <asp:Label ID="lblStartYearOptionTabButton" Text="Inicio de año" runat="server" /></a>
                                <a id="TABBUTTON_Document" href="javascript: void(0);" class="bTab" onclick="SelectTab('panDocumentOptions');" runat="server">
                                    <asp:Label ID="lblDocumentOptionsTabButton" Text="Documentos" runat="server" /></a>
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
                                                            <a href="javascript: void(0);" class="aMsg" onclick="beforeSaveChanges();"><span id="lblSaveChanges" runat="server" /></a>
                                                            &nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;
                                                            <a href="javascript: void(0);" onclick="undoChanges();" class="aMsg"><span id="lblUndoChanges" runat="server" /></a>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>
                                        </td>
                                    </tr>

                                    <tr>
                                        <td valign="top">
                                            <div id="panDocumentOptions" style="width: 100%;">
                                                <table id="tbDocumentsOptions" runat="server" cellpadding="0" cellspacing="0" width="100%">
                                                    <tr>
                                                        <td>
                                                            <div class="panHeader2">
                                                                <span style="">
                                                                    <asp:Label ID="lblDocumentsOptionsTitle" runat="server" Text="Carpetas especiales" Font-Bold="true" /></span>
                                                            </div>
                                                            <br />
                                                        </td>
                                                    </tr>
                                                    <tr style="padding-top: 5px">
                                                        <td style="padding-left: 20px">
                                                            <asp:Label ID="lblA3PayrrollInfo" Text="Desde este formulario definirá el formato de tiempos que aparecerá cuando visualize tiempos." runat="server"></asp:Label>
                                                            <br />
                                                            <br />
                                                        </td>
                                                    </tr>
                                                    <tr style="padding-top: 5px">
                                                        <td style="padding-left: 15px; padding-bottom: 5px; padding-top: 5px;">
                                                                <asp:CheckBox ID="chkA3Payroll" runat="server" Text="Importación de Nóminas A3 " OnClick="showUnregisterA3PayrollIfNeeded();" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>

                                            <div id="panTimeFormatOptions" style="width: 100%;">

                                                <table id="tbTimeFormatOptions" runat="server" cellpadding="0" cellspacing="0" style="width: 100%;">
                                                    <tr>
                                                        <td>
                                                            <div class="panHeader2">
                                                                <span style="">
                                                                    <asp:Label ID="Label1" runat="server" Text="Opciones de visualización de tiempos" Font-Bold="true" /></span>
                                                            </div>
                                                            <br />
                                                        </td>
                                                    </tr>
                                                    <tr style="padding-top: 5px">
                                                        <td style="padding-left: 20px">
                                                            <asp:Label ID="lblTimeFormatOptionsInfo" Text="Desde este formulario definirá el formato de tiempos que aparecerá cuando visualize tiempos." runat="server"></asp:Label>
                                                            <br />
                                                            <br />
                                                        </td>
                                                    </tr>
                                                    <tr style="padding-top: 20px; padding-bottom: 20px; vertical-align: top">
                                                        <td valign="top" style="padding-left: 20px;">

                                                            <table cellpadding="2" cellspacing="2" width="50%">
                                                                <tr>
                                                                    <td>

                                                                        <roUserControls:roOptionPanelClient ID="optTimeFormatNumeric" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Checked="False" Enabled="True">
                                                                            <Title>
                                                                                <asp:Label ID="lblTimeFormatNumericTitle" runat="server" Text="Formato numérico: Horas y centésimas de hora"></asp:Label>
                                                                            </Title>
                                                                            <Description>
                                                                                <asp:Label ID="Label3" Text="Por ejemplo, 2h 30min se verán como 2,5 horas." runat="server" CssClass="OptionPanelDescStyle" />
                                                                            </Description>
                                                                            <Content>
                                                                            </Content>
                                                                        </roUserControls:roOptionPanelClient>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>

                                                                        <roUserControls:roOptionPanelClient ID="optTimeFormatStandard" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Checked="False" Enabled="True">
                                                                            <Title>
                                                                                <asp:Label ID="lblTimeFormatStandardTitle" runat="server" Text="Formato convencional: Horas y minutos"></asp:Label>
                                                                            </Title>
                                                                            <Description>
                                                                                <asp:Label ID="lblTimeFormatStandard" Text="Por ejemplo, 2h 30min se verán como 2:30." runat="server" CssClass="OptionPanelDescStyle" />
                                                                            </Description>
                                                                            <Content>
                                                                            </Content>
                                                                        </roUserControls:roOptionPanelClient>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                            <roUserControls:roOptPanelClientGroup ID="optGroup" runat="server" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>

                                            <div id="panWsConfig" style="width: 100%; display: none">
                                                <roForms:frmWsAdmin ID="frmWsAdmin" runat="server" />
                                            </div>       
                                            
                                            <div id="panWebLinks" style="width: 100%; display: none">
                                                <table id="tbWebLinks" runat="server" cellpadding="0" cellspacing="0" width="100%">
                                                    <tr>
                                                        <td>
                                                            <div class="panHeader2">
                                                                <span style="">
                                                                    <asp:Label ID="lblWebLinksTitle" runat="server" Text="Enlaces" Font-Bold="true" /></span>
                                                            </div>
                                                            <br />
                                                        </td>
                                                    </tr>
                                                    <tr style="padding-top: 5px">
                                                        <td style="padding-left: 20px">
                                                            <asp:Label ID="lblWebLinksDesc" Text="En esta sección configuraremos los bloques de enlaces que aparecerán en la pantalla de inicio de VTLive y en VTPortal." runat="server"></asp:Label>
                                                            <div id="gridWebLinks" style="max-height: 76vh;overflow-y: auto;"></div>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>

                                            <div id="panDatabaseOptions" style="width: 100%; display: none;">
                                                <table id="tbDatabaseOptions" runat="server" cellpadding="0" cellspacing="0" width="100%">
                                                    <tr>
                                                        <td>
                                                            <div class="panHeader2">
                                                                <span style="">
                                                                    <asp:Label ID="lblAuditTitle" runat="server" Text="Auditoría de datos" Font-Bold="true" /></span>
                                                            </div>
                                                            <br />
                                                        </td>
                                                    </tr>
                                                    <tr style="padding-top: 5px">
                                                        <td style="padding-left: 20px">
                                                            <asp:Label ID="lblDatabaseOptionsInfo" Text="En este formulario, configuraremos los distintos parámetros que nos indicarán la permanéncia de los datos de auditoría en el sistema." runat="server"></asp:Label>
                                                            <br />
                                                            <br />
                                                        </td>
                                                    </tr>
                                                    <tr id="trBiometricData" style="padding-top: 20px; padding-bottom: 20px; padding-left: 60px; vertical-align: top;">
                                                        <td style="padding-left: 20px">
                                                            <ajaxToolkit:TabContainer ID="TabBiometricData2" CssClass="" runat="server">
                                                                <ajaxToolkit:TabPanel ID="TabPanelBiometricData2" runat="server" ScrollBars="Auto">
                                                                    <HeaderTemplate>
                                                                        <asp:Label ID="BiometricData2" Text="Permanencia de datos biométricos" runat="server" />
                                                                    </HeaderTemplate>
                                                                    <ContentTemplate>
                                                                        <table cellpadding="0" cellspacing="0" style="padding-left: 10px; padding-top: 20px">
                                                                            <tr>
                                                                                <td>
                                                                                    <asp:Label ID="Label4BD" Text="En cumplimiento de las leyes de RGPD los datos biométricos de los empleados no se pueden almacenar indefinidamente." ForeColor="blue" runat="server" />
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td style="padding-top: 15px;">
                                                                                    <table>
                                                                                        <tr>
                                                                                            <td>
                                                                                                <asp:Label ID="Label5BD" Text="Días que permanecerán los datos en el sistema:" runat="server" />
                                                                                            </td>
                                                                                            <td style="padding-left: 10px;">
                                                                                                <dx:ASPxTextBox runat="server" ID="txtBiometricData" Width="250px" ClientInstanceName="txtBiometricDataDeleteClient">
                                                                                                    <ClientSideEvents ValueChanged="function(s,e){ hasChanges(true)}" />
                                                                                                </dx:ASPxTextBox>
                                                                                            </td>
                                                                                        </tr>
                                                                                    </table>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </ContentTemplate>
                                                                </ajaxToolkit:TabPanel>
                                                            </ajaxToolkit:TabContainer>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>&nbsp;</td>
                                                    </tr>
                                                    <tr id="RowInfo2" style="padding-top: 20px; padding-bottom: 20px; padding-left: 60px; vertical-align: top;">
                                                        <td style="padding-left: 20px">
                                                            <ajaxToolkit:TabContainer ID="tcDatabaseOptions" CssClass="" runat="server">
                                                                <ajaxToolkit:TabPanel ID="tpDatabaseOptions_FreezeDate" runat="server" ScrollBars="Auto">
                                                                    <HeaderTemplate>
                                                                        <asp:Label ID="lblPhotoDeleteTitle" Text="Permanencia de fotos" runat="server" />
                                                                    </HeaderTemplate>
                                                                    <ContentTemplate>
                                                                        <table cellpadding="0" cellspacing="0" style="padding-left: 10px; padding-top: 20px">
                                                                            <tr>
                                                                                <td>
                                                                                    <asp:Label ID="lblPhotoDeleteInfo" Text="En cumplimiento del RGPD las fotos de los fichajes de los empleados no se pueden almacenar indefinidamente." ForeColor="blue" runat="server" />
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td style="padding-top: 15px;">
                                                                                    <table>
                                                                                        <tr>
                                                                                            <td>
                                                                                                <asp:Label ID="lblPhotoDeleteDatys" Text="Días que permanecerán los datos en el sistema:" runat="server" />
                                                                                            </td>
                                                                                            <td style="padding-left: 10px;">
                                                                                                <dx:ASPxTextBox runat="server" ID="txtPhotoDelete" Width="250px" ClientInstanceName="txtPhotoDeleteClient">
                                                                                                    <ClientSideEvents ValueChanged="function(s,e){ hasChanges(true)}" />
                                                                                                </dx:ASPxTextBox>
                                                                                            </td>
                                                                                        </tr>
                                                                                    </table>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </ContentTemplate>
                                                                </ajaxToolkit:TabPanel>
                                                            </ajaxToolkit:TabContainer>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>&nbsp;</td>
                                                    </tr>
                                                    <tr id="trDocumentsAudit" style="padding-top: 20px; padding-bottom: 20px; padding-left: 60px; vertical-align: top;" runat="server">
                                                        <td style="padding-left: 20px">
                                                            <ajaxToolkit:TabContainer ID="tcDatabaseDocOptions" CssClass="" runat="server">
                                                                <ajaxToolkit:TabPanel ID="tpDatabaseDocOptions_FreezeDate" runat="server" ScrollBars="Auto">
                                                                    <HeaderTemplate>
                                                                        <asp:Label ID="lblDocDeleteTitle" Text="Permanencia de Documentos" runat="server" />
                                                                    </HeaderTemplate>
                                                                    <ContentTemplate>
                                                                        <table cellpadding="0" cellspacing="0" style="padding-left: 10px; padding-top: 20px">
                                                                            <tr>
                                                                                <td>
                                                                                    <asp:Label ID="lblDocDeleteInfo" Text="En cumplimiento del RGPD los documentos de los empleados no se pueden almacenar indefinidamente." ForeColor="blue" runat="server" />
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td style="padding-top: 15px;">
                                                                                    <table>
                                                                                        <tr>
                                                                                            <td>
                                                                                                <asp:Label ID="lblDocDeleteDays" Text="Días que permanecerán los datos en el sistema:" runat="server" />
                                                                                            </td>
                                                                                            <td style="padding-left: 10px;">
                                                                                                <dx:ASPxTextBox runat="server" ID="txtDocDelete" Width="250px" ClientInstanceName="txtDocDeleteClient">
                                                                                                    <ClientSideEvents ValueChanged="function(s,e){ hasChanges(true)}" />
                                                                                                </dx:ASPxTextBox>
                                                                                            </td>
                                                                                        </tr>
                                                                                    </table>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </ContentTemplate>
                                                                </ajaxToolkit:TabPanel>
                                                            </ajaxToolkit:TabContainer>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>&nbsp;</td>
                                                    </tr>
                                                    <tr id="trAccessMoves" style="padding-top: 20px; padding-bottom: 20px; padding-left: 60px; vertical-align: top;" runat="server">
                                                        <td style="padding-left: 20px">
                                                            <ajaxToolkit:TabContainer ID="TabContainer1" CssClass="" runat="server">
                                                                <ajaxToolkit:TabPanel ID="TabPanel1" runat="server" ScrollBars="Auto">
                                                                    <HeaderTemplate>
                                                                        <asp:Label ID="lblAccessMovesPermanence" Text="Permanencia de Fichajes de accesos" runat="server" />
                                                                    </HeaderTemplate>
                                                                    <ContentTemplate>
                                                                        <table cellpadding="0" cellspacing="0" style="padding-left: 10px; padding-top: 20px">
                                                                            <tr>
                                                                                <td>
                                                                                    <asp:Label ID="lblAccessMovesPermanenceDesc" Text="Si lo desea puede eliminar los fichajes de accesos de todos los empleados pasados los meses especificados. (0 para no borrar nunca)" ForeColor="blue" runat="server" />
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td style="padding-top: 15px;">
                                                                                    <table>
                                                                                        <tr>
                                                                                            <td>
                                                                                                <asp:Label ID="lblAccessMovesDeleteDays" Text="Meses que permanecerán los datos en el sistema:" runat="server" />
                                                                                            </td>
                                                                                            <td style="padding-left: 10px;">
                                                                                                <dx:ASPxTextBox runat="server" ID="txtAccessMovesDelete" Width="250px" ClientInstanceName="txtAccessMovesDeleteClient">
                                                                                                    <ClientSideEvents ValueChanged="function(s,e){ hasChanges(true)}" />
                                                                                                </dx:ASPxTextBox>
                                                                                            </td>
                                                                                        </tr>
                                                                                    </table>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </ContentTemplate>
                                                                </ajaxToolkit:TabPanel>
                                                            </ajaxToolkit:TabContainer>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>

                                            <div id="panMovesOptions" style="width: 100%;">
                                                <table id="tbMoveOptions" runat="server" cellpadding="0" cellspacing="0" style="width: 100%; height: 100%;">
                                                    <tr>
                                                        <td>
                                                            <div class="panHeader2">
                                                                <span style="">
                                                                    <asp:Label ID="lblMoveOptionsTitle" runat="server" Text="Opciones de fichajes"></asp:Label></span>
                                                            </div>
                                                            <br />
                                                        </td>
                                                    </tr>
                                                    <tr valign="top" style="padding-top: 5px">
                                                        <td style="padding-left: 20px; padding-bottom: 10px;">
                                                            <asp:Label ID="lblMovesOptionsInfo" Text="Desde este formulario podemos configurar los tiempos de detección entre entradas y salidas, los fichajes repetidos y la opción de desactivar los datos biométricos." CssClass="editTextFormat" runat="server"></asp:Label>
                                                            <br />
                                                            <br />
                                                        </td>
                                                    </tr>
                                                    <tr id="RowInfo1" valign="top" style="padding-top: 20px; padding-bottom: 20px; vertical-align: top">
                                                        <td style="padding-left: 20px">
                                                            <ajaxToolkit:TabContainer ID="tcMovesOptions" Width="99%" runat="server">
                                                                <ajaxToolkit:TabPanel ID="tpMovesOptions_ESDetection" runat="server" ScrollBars="Auto">
                                                                    <HeaderTemplate>
                                                                        <asp:Label ID="lblESDetectionTitle" Text="Detección entrada/salida" runat="server" />
                                                                    </HeaderTemplate>
                                                                    <ContentTemplate>
                                                                        <roUserControls:roOptionPanelClient ID="rbESDetectionAdvanced" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Checked="False" Enabled="False">
                                                                            <Title>
                                                                                <asp:Label ID="lblESDetectionAdvancedTitle" runat="server" Text="Usar sistema de detección de entradas y salidas avanzado (no instalado)"></asp:Label>
                                                                            </Title>
                                                                            <Description>
                                                                            </Description>
                                                                            <Content>
                                                                                <table cellpadding="2" cellspacing="2">
                                                                                    <tr>
                                                                                        <td style="padding-left: 15px">
                                                                                            <roUserControls:roOptionPanelClient ID="chkESDetection1" runat="server" TypeOPanel="CheckboxOption" Width="100%" Height="Auto" Checked="False" Enabled="False">
                                                                                                <Title>
                                                                                                    <asp:Label ID="lblESDetection1Title" runat="server" Text="Detectar las entradas y salidas automáticamente según el horario"></asp:Label>
                                                                                                </Title>
                                                                                                <Description>
                                                                                                    <asp:Label ID="lblESDetection1Info" Text="Visual time determina automáticamente si se trata de una entrada ..." ForeColor="blue" runat="server" Enabled="false" />
                                                                                                </Description>
                                                                                                <Content>
                                                                                                    <asp:Label ID="lblESDetection1Time" Text="El tiempo máximo permitido entre una entrada y una salida es de:" runat="server" Enabled="false" />
                                                                                                    <input type="text" id="txtESDetection1Time" runat="server" class="textClass" convertcontrol="TextField" cctime="true" ccallowblank="false" style="width: 40px;" disabled="disabled" />
                                                                                                </Content>
                                                                                            </roUserControls:roOptionPanelClient>
                                                                                        </td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td style="padding-left: 15px;">
                                                                                            <roUserControls:roOptionPanelClient ID="chkESDetection2" runat="server" TypeOPanel="CheckboxOption" Width="100%" Height="Auto" Checked="False" Enabled="False">
                                                                                                <Title>
                                                                                                    <asp:Label ID="lblESDetection2Title" runat="server" Text="Activar corrección interactiva de fichajes olvidados"></asp:Label>
                                                                                                </Title>
                                                                                                <Description>
                                                                                                    <asp:Label ID="lblESDetection2Info" Text="Si un empleado ha olvidado ..." ForeColor="blue" runat="server" Enabled="false" />
                                                                                                </Description>
                                                                                                <Content>
                                                                                                </Content>
                                                                                            </roUserControls:roOptionPanelClient>
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                            </Content>
                                                                        </roUserControls:roOptionPanelClient>

                                                                        <roUserControls:roOptionPanelClient ID="rbESDetectionClassic" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Checked="True" Enabled="True">
                                                                            <Title>
                                                                                <asp:Label ID="lblESDetectionClassicTitle" runat="server" Text="Usar detección de entradas y salidas clásica"></asp:Label>
                                                                            </Title>
                                                                            <Description>
                                                                            </Description>
                                                                            <Content>
                                                                                <table cellpadding="2" cellspacing="2">
                                                                                    <tr>
                                                                                        <td style="padding-left: 25px">
                                                                                            <asp:Label ID="lblESDetectionClassicTime" Text="El tiempo máximo permitido entre una entrada y una salida es de:" runat="server" />
                                                                                            <input type="text" id="txtESDetectionClassicTime" runat="server" class="textClass" convertcontrol="TextField" cctime="true" ccallowblank="false" style="width: 40px;" cconchange="hasChanges(true);" />
                                                                                            <br />
                                                                                            <asp:Label ID="lblESDetectionClassicInfo" Text="El tiempo transcurrido entre una salida y una entrada es mayor o igual al tiempo indicado, entonces VisualTime considerará que se trata de una nueva entrada." ForeColor="blue" runat="server" />
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                            </Content>
                                                                        </roUserControls:roOptionPanelClient>

                                                                        <roUserControls:roOptPanelClientGroup ID="RoOptPanelClientGroup1" runat="server" />
                                                                    </ContentTemplate>
                                                                </ajaxToolkit:TabPanel>
                                                                <ajaxToolkit:TabPanel ID="tpMovesOptions_RepitedMoves" runat="server">
                                                                    <HeaderTemplate>
                                                                        <asp:Label ID="lblRepitedMovesTitle" Text="${Punches} repetidos" runat="server" />
                                                                    </HeaderTemplate>
                                                                    <ContentTemplate>
                                                                        <table cellpadding="0" cellspacing="0" width="100%">
                                                                            <tr>
                                                                                <td style="padding-top: 20px">
                                                                                    <asp:Label ID="lblPunchPeriodRTIn" Text="El tiempo mínimo permitido entre una entrada y una salida es de:" runat="server" />
                                                                                    <input type="text" id="txtPunchPeriodRTIn" runat="server" class="textClass" convertcontrol="TextField" cctime="true" ccallowblank="false" style="width: 40px;" cconchange="hasChanges(true);" />
                                                                                    <br />
                                                                                    <asp:Label ID="lblPunchPeriodRTInInfo" Text="Si el tiempo entre una entrada y una salida es mayor o igual al tiempo indicado, entonces VisualTime considerará que se trata de una nueva salida." ForeColor="blue" runat="server" />
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td style="padding-top: 20px">
                                                                                    <asp:Label ID="lblPunchPeriodRTOut" Text="El tiempo mínimo permitido entre una salida y una entrada es de:" runat="server" />
                                                                                    <input type="text" id="txtPunchPeriodRTOut" runat="server" class="textClass" convertcontrol="TextField" cctime="true" ccallowblank="false" style="width: 40px;" cconchange="hasChanges(true);" />
                                                                                    <br />
                                                                                    <asp:Label ID="lblPunchPeriodRTOutInfo" Text="Si el tiempo entre una salida y una entrada es mayor o igual al tiempo indicado, entonces VisualTime considerará que se trata de una nueva entrada. En caso contrario, el terminal indicará al empleado que acaba de realizar un fichaje y si desea cambiarlo." ForeColor="blue" runat="server" />
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </ContentTemplate>
                                                                </ajaxToolkit:TabPanel>
                                                                <ajaxToolkit:TabPanel ID="tpMovesOptions_BiometricDate" runat="server">
                                                                    <HeaderTemplate>
                                                                        <asp:Label ID="lblBiometricDateTitle" Text="Datos biométricos" runat="server" />
                                                                    </HeaderTemplate>
                                                                    <ContentTemplate>
                                                                        <table cellpadding="0" cellspacing="0" width="100%">
                                                                            <tr>
                                                                                <td width="25%">
                                                                                    <table cellpadding="2" cellspacing="2">
                                                                                        <tr>
                                                                                            <td style="padding-left: 15px; padding-bottom: 5px; padding-top: 5px;">
                                                                                                <asp:CheckBox ID="chkDisableBiometricData" runat="server" Text="Desactivar la utilización de datos biométricos" />

                                                                                            </td>
                                                                                        </tr>
                                                                                        <tr>
                                                                                            <td style="padding-left: 15px; padding-bottom: 5px; padding-top: 5px;">
                                                                                                <div id="deleteBiometricData" runat="server" class="btnFlat" style="padding: 1px 5px 1px 5px !important">
                                                                                                    <a href="javascript: void(0)" id="btnDeleteBiometricData" runat="server" onclick="showDeleteBiometricDataConfirmation();">
                                                                                                        <span class="btnIconAdd"></span>
                                                                                                        <asp:Label ID="lblDeleteBiometricData" runat="server" Text="Borrar datos biométricos"></asp:Label>
                                                                                                    </a>
                                                                                                </div>
                                                                                            </td>
                                                                                        </tr>
                                                                                    </table>
                                                                                </td>
                                                                                <td>
                                                                                    <div id="availableCertificates" style="margin-top:10px;"></div>
                                                                                </td>
                                                                            <tr>
                                                                        </table>
                                                                    </ContentTemplate>
                                                                </ajaxToolkit:TabPanel>
                                                            </ajaxToolkit:TabContainer>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>

                                            <div id="panStartYearOptions" style="width: 100%; display: none;">
                                                <table id="tbStartYearOption" runat="server" cellpadding="0" cellspacing="0" style="width: 100%; height: 100%;">
                                                    <tr>
                                                        <td colspan="4">
                                                            <div class="panHeader2">
                                                                <span style="">
                                                                    <asp:Label ID="lblYearPeriodTitle" runat="server" Text="Periodo anual"></asp:Label></span>
                                                            </div>
                                                            <br />
                                                        </td>
                                                    </tr>
                                                    <tr valign="top" style="padding-top: 5px">
                                                        <td style="padding-left: 20px;">
                                                            <img id="imbYearPeriod" src="Images/YearPeriod32.png" />
                                                        </td>
                                                        <td style="padding-left: 20px; padding-bottom: 10px;" colspan="3">
                                                            <asp:Label ID="lblYearPeriodDescription" Text="Podemos especificar el periodo anual que vamos a utilizar, marcando el mes que vamos a contabilizar como el primero del año. A partir de ese mes se contará un año natural (Si empezamos el mes de febrero, el periodo anual será del mes de febrero del año actual al mes de enero del año siguiente)." CssClass="editTextFormat" runat="server"></asp:Label>
                                                            <br />
                                                            <br />
                                                        </td>
                                                    </tr>
                                                    <tr valign="top" style="padding-top: 20px; padding-bottom: 20px; vertical-align: top">
                                                        <td></td>
                                                        <td style="padding-left: 20px;">
                                                            <table>
                                                                <tr>
                                                                    <td>
                                                                        <asp:Label ID="lblYearPeriod1" Text="El mes " runat="server"></asp:Label></td>
                                                                    <td>
                                                                        <input type="text" id="txtYearPeriod" runat="server" class="textClass" convertcontrol="NumberField" ccallowblank="false" ccminvalue="1" ccmaxvalue="12" ccallownegative="false" ccallowdecimals="false" style="width: 40px;" cconchange="hasChanges(true);" /></td>
                                                                    <td>
                                                                        <asp:Label ID="lblYearPeriod2" Text=", contará como el primer mes del año." runat="server"></asp:Label></td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                    <tr style="padding-top: 30px">
                                                        <td colspan="4" style="padding-top: 30px">
                                                            <div class="panHeader2">
                                                                <span style="">
                                                                    <asp:Label ID="lblMonthPeriodTitle" runat="server" Text="Periodo mensual"></asp:Label></span>
                                                            </div>
                                                            <br />
                                                        </td>
                                                    </tr>
                                                    <tr valign="top" style="padding-top: 5px">
                                                        <td style="padding-left: 20px;">
                                                            <img id="imgMonthPeriod" src="Images/MonthPeriod32.png" />
                                                        </td>
                                                        <td style="padding-left: 20px; padding-bottom: 10px;" colspan="3">
                                                            <asp:Label ID="lblMonthPeriodDescription" Text="Podemos especificar el periodo mensual que vamos a utilizar, marcando el día que vamos a contabilizar como el primero del mes. A partir de ese día se contará un mes natural (Si empezamos el dia 20, el periodo mensual será del 20 del mes actual al 19 del més siguiente)." CssClass="editTextFormat" runat="server"></asp:Label>
                                                            <br />
                                                            <br />
                                                        </td>
                                                    </tr>
                                                    <tr valign="top" style="padding-top: 20px; padding-bottom: 20px; vertical-align: top">
                                                        <td></td>
                                                        <td style="padding-left: 20px;">
                                                            <table>
                                                                <tr>
                                                                    <td>
                                                                        <asp:Label ID="lblMonthPeriod1" Text="El día " runat="server"></asp:Label></td>
                                                                    <td>
                                                                        <input type="text" id="txtMonthPeriod" runat="server" class="textClass" convertcontrol="NumberField" ccallowblank="false" ccminvalue="1" ccmaxvalue="31" ccallownegative="false" ccallowdecimals="false" style="width: 40px;" cconchange="hasChanges(true);" /></td>
                                                                    <td>
                                                                        <asp:Label ID="lblMonthPeriod2" Text=", contará como el primer día del mes." runat="server"></asp:Label></td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                    <tr style="padding-top: 30px">
                                                        <td colspan="4" style="padding-top: 30px">
                                                            <div class="panHeader2">
                                                                <span style="">
                                                                    <asp:Label ID="lblWeekPeriod" runat="server" Text="Periodo semanal"></asp:Label></span>
                                                            </div>
                                                            <br />
                                                        </td>
                                                    </tr>
                                                    <tr valign="top" style="padding-top: 5px">
                                                        <td style="padding-left: 20px;">
                                                            <img id="imgWeekPeriod" src="Images/WeekPeriod32.png" />
                                                        </td>
                                                        <td style="padding-left: 20px; padding-bottom: 10px;" colspan="3">
                                                            <asp:Label ID="lblWeekPeriodDescription" Text="Podemos establecer el período semanal que vamos a utilizar, indicando el que consideramos primer día de la semana. A partir de este día, se contarán 7 días naturales." CssClass="editTextFormat" runat="server"></asp:Label>
                                                            <br />
                                                            <br />
                                                        </td>
                                                    </tr>
                                                    <tr valign="top" style="padding-top: 20px; padding-bottom: 20px; vertical-align: top">
                                                        <td></td>
                                                        <td style="padding-left: 20px;">
                                                            <table>
                                                                <tr>
                                                                    <td>
                                                                        <asp:Label ID="lblWeekStartDay" Text="El  " runat="server"></asp:Label></td>
                                                                    <td>
                                                                        <dx:ASPxComboBox ID="cmbWeekDay" runat="server" Width="200px" Font-Size="11px" ForeColor="#2D4155"
                                                                            Font-Names="Arial;Verdana;Sans-Serif" IncrementalFilteringMode="Contains" ClientInstanceName="cmbWeekDayClient">
                                                                            <ClientSideEvents SelectedIndexChanged="function(s,e){hasChanges(true)}" />
                                                                        </dx:ASPxComboBox>
                                                                        <td>
                                                                            <asp:Label ID="lblWeekStartDayExplanation" Text=", será el primer día de la semana." runat="server"></asp:Label></td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" valign="bottom" class="DetailFrame_Background" style="padding-right: 20px; height: 100%; vertical-align: bottom;">
                                            <table>
                                                <tr align="right">
                                                    <td>
                                                        <asp:Button ID="btSave" Text="${Button.ApplyChanges}" runat="server" OnClientClick="return CheckSave();" Visible="false" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                                    </td>
                                                    <td>
                                                        <asp:Button ID="btCancel" Text="${Button.UndoChanges}" runat="server" Visible="false" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                                    </td>
                                                </tr>
                                            </table>
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
                                                            <a href="javascript: void(0);" class="aMsg" onclick="beforeSaveChanges();"><span id="lblSaveChangesBottom" runat="server" /></a>
                                                            &nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;
                                                            <a href="javascript: void(0);" onclick="undoChanges();" class="aMsg"><span id="lblUndoChangesBottom" runat="server" /></a>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>
                                        </td>
                                    </tr>
                                </table>

                                <asp:Button ID="btRefresh" runat="server" Style="display: none;" />

                                <asp:HiddenField ID="hdnChanged" runat="server" />

                                <asp:HiddenField ID="hdnIsPostBack_PageBase" runat="server" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>
    <asp:HiddenField ID="ConfigurationOptions_TabVisibleName" Value="" runat="server" />

    <!-- POPUP NEW OBJECT -->
    <dx:ASPxPopupControl ID="AspxLoadingPopup" runat="server" AllowDragging="False" CloseAction="None" Modal="True" ContentUrl="~/Base/Popups/PerformingActionAndCancel.aspx"
        PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter" ModalBackgroundStyle-Opacity="0" Width="460" Height="260"
        ShowHeader="False" ScrollBars="Auto" ShowPageScrollbarWhenModal="True" ClientInstanceName="AspxLoadingPopup_Client" PopupAnimationType="None" BackColor="Transparent" ContentStyle-Paddings-Padding="0px" Border-BorderColor="Transparent" ShowShadow="false">
        <SettingsLoadingPanel Enabled="false" />
    </dx:ASPxPopupControl>

    <Local:MessageFrame ID="MessageFrame1" runat="server" />
</asp:Content>
