<%@ Page Language="VB" MasterPageFile="~/Base/mastEmp.master" AutoEventWireup="false" Inherits="VTLive40.Terminals" Title="${Terminals}" EnableEventValidation="false" CodeBehind="Terminals.aspx.vb" %>

<%@ Register Src="~/Terminals/WebUserControls/frmTerminalReaderV2.ascx" TagName="frmTerminalReader" TagPrefix="roForms" %>
<%@ Register Src="~/Terminals/WebUserForms/frmAddSiren.ascx" TagName="frmAddSiren" TagPrefix="roFormsSiren" %>

<asp:Content ID="cMainBody" ContentPlaceHolderID="contentMainBody" runat="Server">

    <script language="javascript" type="text/javascript">

        /* msg combos */
        var CriteriaEquals = '<%= Me.Language.KeywordJavaScript("Criteria.Equals")%>';
        var CriteriaDifferent = '<%= Me.Language.KeywordJavaScript("Criteria.Different")%>';
        var CriteriaStartsWith = '<%= Me.Language.KeywordJavaScript("Criteria.StartWith")%>';
        var CriteriaContains = '<%= Me.Language.KeywordJavaScript("Criteria.Contains")%>';
        var CriteriaNoContains = '<%= Me.Language.KeywordJavaScript("Criteria.NoContains")%>';
        var CriteriaMajor = '<%= Me.Language.KeywordJavaScript("Criteria.Major")%>';
        var CriteriaMajorOrEquals = '<%= Me.Language.KeywordJavaScript("Criteria.MajorOrEquals")%>';
        var CriteriaMinor = '<%= Me.Language.KeywordJavaScript("Criteria.Minor")%>';
        var CriteriaMinorOrEquals = '<%= Me.Language.KeywordJavaScript("Criteria.MinorOrEquals")%>';
        var CriteriaTheValue = '<%= Me.Language.KeywordJavaScript("Criteria.TheValue")%>';
        var CriteriaTheDate = '<%= Me.Language.KeywordJavaScript("Criteria.TheDate")%>';
        var CriteriaTheDateOfJustification = '<%= Me.Language.KeywordJavaScript("Criteria.TheDateActual")%>';
        var CriteriaTheTime = '<%= Me.Language.KeywordJavaScript("Criteria.TheTime")%>';
        var CriteriaTheTimeOfJustification = '<%= Me.Language.KeywordJavaScript("Criteria.TheTimeActual")%>';
        var CriteriaTheValues = '<%= Me.Language.KeywordJavaScript("Criteria.TheValues")%>';
        var CriteriaThePeriod = '<%= Me.Language.KeywordJavaScript("Criteria.ThePeriod") %>';

        function PageBase_Load() {
            resizeFrames();
            resizeTreeTerminals();
            ConvertControls();
        }

        function showPopUp(contentUrl) {
            try {
                parent.showLoader(true);
                PopupTerminalFile_Client.SetContentUrl('UploadDataFile.aspx?v=' + new Date().getTime());
                PopupTerminalFile_Client.Show();
            } catch (e) { showError('showPopUp', e); }
        }

        var showNoConfig = <%= showNoConfig%>;
        if (showNoConfig) showErrorPopup('Error.NoConfigTitle', 'error', 'Error.NoConfig', '', 'Error.OK', 'Error.OKDesc', '');

        var showNoUSBFile = <%= showNoUSBFile%>;
        if (showNoUSBFile) showErrorPopup('Error.NoUSBFileTitle', 'error', 'Error.NoUSBFile', '', 'Error.OK', 'Error.OKDesc', '');

        var monitor = -1;

        function showCaptcha() {
            var contentUrl = "../Base/Popups/GenericCaptchaValidator.aspx?Action=CREATEUSBTASK";
            CaptchaObjectPopup_Client.SetContentUrl(contentUrl);
            CaptchaObjectPopup_Client.Show();
        }

        function showCaptchaRestrictionWarning() {
            var contentUrl = "../Base/Popups/GenericCaptchaValidator.aspx?Action=LEGACYRESTRICTIONWARNING";
            CaptchaObjectPopup_Client.SetContentUrl(contentUrl);
            CaptchaObjectPopup_Client.Show();
        }

        function captchaCallback(action) {
            switch (action) {
                case "CREATEUSBTASK":
                    AspxLoadingPopup_Client.Show();
                    PerformAction();
                    break;
                case "LEGACYRESTRICTIONWARNING":
                    saveDefChangesTerminals();
                    break;
                case "ERROR":
                    window.parent.frames["ifPrincipal"].showErrorPopup("Error.ValidationFailed", "ERROR", "Error.ValidationFailedDesc", "Error.OK", "Error.OKDesc", "");
                    break;
            }
        }

        function PerformValidation() {
            PerformActionCallbackClient.PerformCallback("VALIDATE");
        }

        function PerformAction() {
            PerformActionCallbackClient.PerformCallback("PERFORM_ACTION");
        }

        function PerformActionCallback_CallbackComplete(s, e) {
            if (s.cpAction == "VALIDATE" && s.cpResult == true) {
                showCaptcha();
            } else if (s.cpAction == "PERFORM_ACTION") {
                monitor = setInterval(function () {
                    PerformActionCallbackClient.PerformCallback("CHECKPROGRESS");
                }, 10000);
            } else if (s.cpAction == "ERROR") {
                clearInterval(monitor);
                AspxLoadingPopup_Client.Hide();
            } else if (s.cpAction == "CHECKPROGRESS") {
                if (s.cpActionResult == "OK") {
                    clearInterval(monitor);
                    AspxLoadingPopup_Client.Hide();
                    downloadURL('../DataLink/Wizards/downloadFile.aspx');
                }
            }
        }

        function downloadURL(url) {
            var hiddenIFrameID = 'hiddenDownloader',
                iframe = document.getElementById(hiddenIFrameID);
            if (iframe === null) {
                iframe = document.createElement('iframe');
                iframe.id = hiddenIFrameID;
                iframe.style.display = 'none';
                document.body.appendChild(iframe);
            }
            iframe.src = url;
        }

        $(document).ready(function () {
            $(window).keydown(function (event) {
                if (event.keyCode == 13) {
                    event.preventDefault();
                    return false;
                }
            });
        });
    </script>
    <input type="hidden" runat="server" id="hdnModeEdit" value="" />
    <input type="hidden" runat="server" id="noRegs" value="" />
    <input type="hidden" id="ctlTreeSelectorPrefix" value="" />
    <input type="hidden" id="hdnLngWeekdayName" value="<%= Me.Language.Translate("gridHeader.WeekDayName",Me.DefaultScope) %>" />
    <input type="hidden" id="hdnLngHour" value="<%= Me.Language.Translate("gridHeader.Hour",Me.DefaultScope) %>" />
    <input type="hidden" id="hdnLngDuration" value="<%= Me.Language.Translate("gridHeader.Duration",Me.DefaultScope) %>" />
    <input type="hidden" id="lblAllEmp" value="<%=  Me.Language.Translate("lblAllEmp",DefaultScope) %>" />
    <input type="hidden" id="lblEmpSelect" value="<%=  Me.Language.Translate("lblEmpSelect",DefaultScope) %>" />
    <input type="hidden" id="TerminalConnected" value="<%=  Me.Language.Translate("TerminalConnected", DefaultScope) %>" />
    <input type="hidden" id="TerminalDisconnected" value="<%=  Me.Language.Translate("TerminalDisconnected",DefaultScope) %>" />
    <input type="hidden" id="SelectedEmployees" value="<%= Me.Language.Translate("SelectedEmployees",DefaultScope) %>" />

    <div id="divModalBgDisabled" class="modalBackground" style="position: absolute; left: 0px; top: 0px; z-index: 990; width: 1680px; height: 900px; display: none;"></div>

    <dx:aspxcallback id="PerformActionCallback" runat="server" clientinstancename="PerformActionCallbackClient" clientsideevents-callbackcomplete="PerformActionCallback_CallbackComplete"></dx:aspxcallback>

    <input type="hidden" id="msgHasChanges" value="" runat="server" clientidmode="Static" />
    <input type="hidden" id="msgHasErrors" value="" runat="server" clientidmode="Static" />

    <div id="divMainBody">
        <!-- TAB SUPERIOR -->
        <div id="divTabInfo" class="divDataCells">
            <div style="min-height: 10px"></div>
            <div id="divTerminals" class="blackRibbonTitle">
            </div>
            <div style="min-height: 10px"></div>
        </div>
        <!-- FIN TAB SUPERIOR -->

        <!-- ARBOL Y DETALLE -->
        <div id="divTabData" class="divDataCells">

            <div id="divTree" class="treeSize">
                <div id="ctlTreeDiv" style="height: 500px;">
                    <rws:rotreesselector id="roTreesTerminals" runat="server" showemployeefilters="false" prefixtree="roTreesTerminals"
                        tree1visible="true" tree1multisel="false" tree1showonlygroups="false" tree1function="cargaNodo" tree1imagepath="images/TerminalSelector" tree1selectorpage="../../Terminals/TerminalSelectorData.aspx"
                        showtreecaption="true">
                    </rws:rotreesselector>
                </div>
            </div>

            <div id="divButtons" class="divMiddleButtons">
                <div id="divBarButtons" class="maxHeight">&nbsp</div>
            </div>

            <div id="divContenido" class="divRightContent">
                <div id="divContent" class="maxHeight">
                    <dx:aspxcallbackpanel id="ASPxCallbackPanelContenido" runat="server" width="100%" height="100%" clientinstancename="ASPxCallbackPanelContenidoClient">
                        <settingsloadingpanel enabled="false" />
                        <clientsideevents endcallback="ASPxCallbackPanelContenidoClient_EndCallBack" />
                        <panelcollection>
                            <dx:panelcontent id="PanelContent1" runat="server">

                                <div id="divMsgTop" class="divMsg2 divMessageTop" style="display: none">
                                    <div class="divImageMsg">
                                        <img alt="" id="Img1" src="~/Base/Images/MessageFrame/Alert16.png" runat="server" />
                                    </div>
                                    <div class="messageText">
                                        <span id="msgTop"></span>
                                    </div>
                                    <div align="right" class="messageActions">
                                        <a href="javascript: void(0);" class="aMsg" onclick="saveChanges();"><span id="lblSaveChanges" runat="server" /></a>
                                        &nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;
                                        <a href="javascript: void(0);" onclick="undoChanges();" class="aMsg"><span id="lblUndoChanges" runat="server" /></a>
                                    </div>
                                </div>

                                <input type="hidden" id="legacyModeEnabled" value="false" runat="server" />

                                <div id="divContentPanels" class="divContentPanelsWithOutMessage">
                                    <div id="rowTerminalsList" class="contentPanel" runat="server" style="display: none">
                                        <div id="divList01" class="contentPanel" style="display: none;" runat="server" name="menuPanel">
                                            <div>
                                                <img id="imgCommsDisabledAlert" src="~/Base/Images/MessageFrame/Alert16.png" runat="server" alt="" />&nbsp;
                                                <asp:Label ID="lblCommsState" Style="color: Red; font-weight: bold; position: relative; top: -2px;" runat="server" Text="Prueba"></asp:Label>&nbsp;
                                            </div>
                                            <div id="gridListContainer" class="defaultContrastColor" style="background-position: 0; width: 100%; overflow-y: auto; border: 0px; margin: 0;">
                                                <table runat="server" id="tblTerminals" border="0" cellpadding="0" cellspacing="0" style="width: 100%;" class="defaultContrastColor">
                                                </table>
                                            </div>
                                        </div>

                                        <!-- Pantalla Conector de Fichajes -->
                                        <div id="divList02" class="contentPanel" style="display: none;" runat="server" name="menuPanel">
                                            <div class="panBottomMargin">
                                                <div class="panHeader2 panBottomMargin">
                                                    <span class="panelTitleSpan">
                                                        <asp:Label runat="server" ID="lblPunchImportTitle" Text="Importación de fichajes"></asp:Label>
                                                    </span>
                                                </div>
                                            </div>

                                            <div class="panBottomMargin" style="width: calc(100% - 10px);">
                                                <div class="divRow">
                                                    <div class="divRowDescription">
                                                        <asp:Label ID="lblImportDescription" runat="server" Text="Ruta donde el servidor debe ir a buscar los ficheros de importación de fichajes"></asp:Label>
                                                    </div>
                                                    <asp:Label ID="lblPathName" runat="server" Text="Orígen de datos:" class="labelForm"></asp:Label>
                                                    <div class="componentForm">
                                                        <dx:aspxcombobox runat="server" id="cmbSourceData" width="400px">
                                                            <clientsideevents selectedindexchanged="function(s,e){hasChanges(true);}" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                        </dx:aspxcombobox>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="panBottomMargin" style="width: calc(100% - 10px);">
                                                <div class="divRow">
                                                    <div class="divRowDescription">
                                                        <asp:Label ID="lblOriginFileNameDesc" runat="server" Text="Nombre del fichero origen que contiene los fichajes para ser importados a visualtime"></asp:Label>
                                                    </div>
                                                    <asp:Label ID="lblOriginFileName" runat="server" Text="Nombre origen:" class="labelForm"></asp:Label>
                                                    <div class="componentForm">
                                                        <dx:aspxtextbox id="txtConnectorOriginFileName" runat="server" width="400px">
                                                            <validationsettings errordisplaymode="None" />
                                                            <clientsideevents textchanged="function(s,e){ hasChanges(true); }" />
                                                        </dx:aspxtextbox>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="panBottomMargin" style="width: calc(100% - 10px);">
                                                <div class="divRow">
                                                    <div class="divRowDescription">
                                                        <asp:Label ID="lblDestinyFileNameDesc" runat="server" Text="Nombre del fichero que utiliza visualtime para importar los fichajes"></asp:Label>
                                                    </div>
                                                    <asp:Label ID="lblDestinyFileName" runat="server" Text="Nombre destino:" class="labelForm"></asp:Label>
                                                    <div class="componentForm">
                                                        <dx:aspxtextbox id="txtConnectorDestinyFileName" runat="server" width="400px">
                                                            <validationsettings errordisplaymode="None" />
                                                            <clientsideevents textchanged="function(s,e){ hasChanges(true); }" />
                                                        </dx:aspxtextbox>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="panBottomMargin">
                                                <div class="panHeader2 panBottomMargin">
                                                    <span class="panelTitleSpan">
                                                        <asp:Label runat="server" ID="lblSupremaTitle" Text="Suprema"></asp:Label>
                                                    </span>
                                                </div>
                                            </div>
                                            <div style="display: flex; align-items: flex-start;">
                                                <div>
                                                    <div class="panBottomMargin" style="width: calc(100% - 10px);">
                                                        <div class="divRow">
                                                            <div class="divRowDescription">
                                                                <asp:Label ID="lblURLDescription" runat="server" Text="URL de la API de Suprema"></asp:Label>
                                                            </div>
                                                            <asp:Label ID="lblURLSuprema" runat="server" Text="URL:" class="labelForm"></asp:Label>
                                                            <div class="componentForm">
                                                                <dx:aspxtextbox id="txtURLSuprema" runat="server" width="400px">
                                                                    <validationsettings errordisplaymode="None" />
                                                                    <clientsideevents textchanged="function(s,e){ hasChanges(true); }" />
                                                                </dx:aspxtextbox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="panBottomMargin" style="width: calc(100% - 10px);">
                                                        <div class="divRow">
                                                            <div class="divRowDescription">
                                                                <asp:Label ID="lblUserSupremaDescription" runat="server" Text="Usuario para acceso a la API de Suprema"></asp:Label>
                                                            </div>
                                                            <asp:Label ID="lblUserSuprema" runat="server" Text="Usuario:" class="labelForm"></asp:Label>
                                                            <div class="componentForm">
                                                                <dx:aspxtextbox id="txtUserSuprema" runat="server" width="400px">
                                                                    <validationsettings errordisplaymode="None" />
                                                                    <clientsideevents textchanged="function(s,e){ hasChanges(true); }" />
                                                                </dx:aspxtextbox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="panBottomMargin" style="width: calc(100% - 10px);">
                                                        <div class="divRow">
                                                            <div class="divRowDescription">
                                                                <asp:Label ID="lblPasswordSupremaDescription" runat="server" Text="Contraseña para acceso a la API de Suprema"></asp:Label>
                                                            </div>
                                                            <asp:Label ID="lblPasswordSuprema" runat="server" Text="Password:" class="labelForm"></asp:Label>
                                                            <div class="componentForm">
                                                                <dx:aspxtextbox id="txtPasswordSuprema" runat="server" width="400px" clientinstancename="txtPasswordSuprema_Client">
                                                                    <validationsettings errordisplaymode="None" />
                                                                    <clientsideevents textchanged="function(s,e){ hasChanges(true); }" init="function(s,e){ setPasswordPlaceholders(); }" gotfocus="function(s,e){ resetPasswordPlaceholders(); }" lostfocus="function(s,e){ setPasswordPlaceholders(); }" />
                                                                </dx:aspxtextbox>
                                                                <input type="hidden" runat="server" id="hasSupremaPassword" value="" />
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="panBottomMargin" style="width: calc(100% - 10px);">
                                                        <div class="divRow">
                                                            <div class="divRowDescription">
                                                                <asp:Label ID="lblIDEmployeeSupremaDescription" runat="server" Text="Campo del expediente en que se informará el identificador de usuario en Suprema"></asp:Label>
                                                            </div>
                                                            <asp:Label ID="lblIDEmployeeSuprema" runat="server" Text="Identificador de Usuario:" class="labelForm"></asp:Label>
                                                            <div class="componentForm">
                                                                <dx:aspxcombobox runat="server" id="cmbIDEmployeeSuprema" width="400px">
                                                                    <clientsideevents selectedindexchanged="function(s,e){hasChanges(true);}" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                                </dx:aspxcombobox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="panBottomMargin" style="width: calc(100% - 10px);">
                                                        <div class="divRow">
                                                            <div class="divRowDescription">
                                                                <asp:Label ID="lblInitialDateSupremaDescription" runat="server" Text="Fecha más antigua desde la que se importarán los fichajes"></asp:Label>
                                                            </div>
                                                            <asp:Label ID="lblInitialDateSuprema" runat="server" Text="Fecha inicial:" class="labelForm"></asp:Label>
                                                            <div class="componentForm">
                                                                <dx:aspxdateedit runat="server" id="txtDateInfSuprema" width="140px" allownull="True" cssclass="editTextFormat" font-size="11px" clientinstancename="txtDateInfSupremaClient" popupverticalalign="TopSides" popuphorizontalalign="OutsideRight">
                                                                    <clientsideevents datechanged="function(s,e){hasChanges(true);}" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                                </dx:aspxdateedit>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div style="padding-top: 35px; display: flex; align-items: center;">
                                                    <img runat="server" id="imgSupremaLight" src="" alt="" />
                                                    <asp:Label ID="lblSupremaStatusDesc" runat="server" Text="" class="labelForm" Style="float: none; padding-left: 10px; text-align: left; width: auto;"></asp:Label>
                                                </div>
                                            </div>

                                        </div>
                                    </div>

                                    <div id="rowTerminalInfo" class="contentPanel" runat="server" style="display: none">
                                        <!-- Panell Configuracio -->
                                        <div id="div01" class="contentPanel" style="display: none;" runat="server" name="menuPanel">
                                            <div class="panHeader2">
                                                <span style="">
                                                    <asp:Label runat="server" ID="lblConfigTitle" Text="Configuración"></asp:Label></span>
                                            </div>
                                            <br />
                                            <table width="100%" border="0" style="padding-top: 5px">
                                                <tr>
                                                    <td style="padding-left: 15px;" width="150px" align="center" valign="top">
                                                        <img runat="server" id="imgTerminal" src="" style="border: solid 1px #EFEFEF;" alt="" /></td>
                                                    <td>
                                                        <!-- taula configuracio -->
                                                        <table border="0">
                                                            <tr>
                                                                <td>
                                                                    <asp:Label ID="lblTerminalName" runat="server" Text="Nombre:" Font-Bold="true"></asp:Label>
                                                                </td>
                                                                <td style="padding-top: 5px; padding-bottom: 5px;">
                                                                    <dx:aspxtextbox id="txtName" runat="server" clientinstancename="txtName_Client" nulltext="_____">
                                                                        <clientsideevents validation="LengthValidation" textchanged="function(s,e){checkTerminalEmptyName(s.GetValue());}" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                                        <validationsettings setfocusonerror="True">
                                                                            <requiredfield isrequired="True" errortext="(*)" />
                                                                        </validationsettings>
                                                                    </dx:aspxtextbox>
                                                                </td>
                                                                <td colspan="2">
                                                                    <rousercontrols:rooptionpanelclient id="chkEnabled" runat="server" typeopanel="CheckboxOption" width="100%" height="Auto" checked="False" enabled="True" border="True" cconclick="hasChanges(true);showWarning();">
                                                                        <title>
                                                                            <asp:Label ID="lblEnabled" runat="server" Text="Activado"></asp:Label>
                                                                        </title>
                                                                    </rousercontrols:rooptionpanelclient>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <asp:Label ID="lblConfModel" runat="server" Text="Modelo:" Font-Bold="true"></asp:Label>
                                                                </td>
                                                                <td>
                                                                    <dx:aspxcombobox id="cmbModel" runat="server" width="175px">
                                                                        <clientsideevents selectedindexchanged="function(s,e){ hasChanges(true); }" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                                    </dx:aspxcombobox>
                                                                </td>
                                                                <td style="padding-left: 10px; padding-right: 2px;">
                                                                    <asp:Label ID="lblConfID" runat="server" Text="ID:" Font-Bold="true"></asp:Label></td>
                                                                <td>
                                                                    <dx:aspxtextbox id="txtID" runat="server" maxlength="5" width="50px">
                                                                        <validationsettings errordisplaymode="None" />
                                                                        <masksettings mask="<0..99999>" />
                                                                        <clientsideevents textchanged="function(s,e){ hasChanges(true); }" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                                    </dx:aspxtextbox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <asp:Label ID="lblConfStatus" runat="server" Text="Estado:" Font-Bold="true" Style="display: none;"></asp:Label>
                                                                </td>
                                                                <td colspan="3" style="padding-top: 5px; padding-bottom: 5px;">
                                                                    <asp:Label ID="txtStatus" runat="server" Style="display: none;" Font-Bold="true" ForeColor="green" Text="Conectado"></asp:Label>
                                                                </td>
                                                            </tr>
                                                            <tr id="trLocation" runat="server">
                                                                <td style="padding-right: 2px;">
                                                                    <asp:Label ID="Label1" runat="server" Text="IP Terminal:" Font-Bold="true"></asp:Label>
                                                                </td>
                                                                <td style="padding-right: 2px;">
                                                                    <dx:aspxtextbox id="txtTerminalAddress" runat="server" width="150px" maxlength="50">
                                                                        <clientsideevents textchanged="function(s,e){ hasChanges(true); }" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                                    </dx:aspxtextbox>
                                                                </td>
                                                            </tr>
                                                            <tr id="trFirmware" runat="server">
                                                                <td style="padding-right: 2px;">
                                                                    <asp:Label ID="lblFirmwareVersion" runat="server" Text="Firmware:" Font-Bold="true"></asp:Label>
                                                                </td>
                                                                <td style="padding-right: 2px;">
                                                                    <dx:aspxtextbox id="txtFirmware" runat="server" width="150px" maxlength="50" readonly="true">
                                                                        <clientsideevents textchanged="function(s,e){ hasChanges(true); }" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                                    </dx:aspxtextbox>
                                                                </td>
                                                            </tr>
                                                            <tr id="trAPKVersion" runat="server">
                                                                <td style="padding-right: 2px;">
                                                                    <asp:Label ID="lblAPKVersion" runat="server" Text="Versión APK:" Font-Bold="true"></asp:Label>
                                                                </td>
                                                                <td style="padding-right: 2px;">
                                                                    <dx:aspxtextbox id="txtAPKVersion" runat="server" width="150px" maxlength="50" readonly="true">
                                                                        <clientsideevents textchanged="function(s,e){ hasChanges(true); }" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                                    </dx:aspxtextbox>
                                                                </td>
                                                            </tr>
                                                            <tr id="trSerialNumber" runat="server">
                                                                <td style="padding-right: 2px;">
                                                                    <asp:Label ID="lblSerialNumber" runat="server" Text="Serial:" Font-Bold="true"></asp:Label>
                                                                </td>
                                                                <td style="padding-right: 2px;">
                                                                    <dx:aspxtextbox id="txtSerialNumber" runat="server" width="250px" maxlength="50" readonly="true">
                                                                        <clientsideevents textchanged="function(s,e){ hasChanges(true); }" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                                    </dx:aspxtextbox>
                                                                </td>
                                                            </tr>
                                                            <tr id="trInactivityTime" runat="server">
                                                                <td style="padding-right: 2px;">
                                                                    <asp:Label ID="lblInactivityTime" runat="server" Text="Tiempo de inactividad (en segundos):" Font-Bold="true"></asp:Label>
                                                                </td>
                                                                <td style="padding-right: 2px;">
                                                                    <dx:aspxtextbox id="txtInactivityTime" runat="server" clientinstancename="txtInactivityTime_Client" nulltext="10">
                                                                        <clientsideevents textchanged="function(s,e){ hasChanges(true); }" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                                    </dx:aspxtextbox>
                                                                    <asp:Label ID="lblOnSeconds" runat="server" Text="segundos" Font-Bold="true"></asp:Label>
                                                                </td>
                                                            </tr>

                                                            <tr>
                                                                <td id="tdCheckTimeZone" colspan="4" style="padding-top: 50px;" runat="server">
                                                                    <rousercontrols:rooptionpanelclient id="chkTimeZone" runat="server" typeopanel="CheckboxOption" width="100%" height="Auto" checked="False" enabled="True" border="True" cconclick="hasChanges(true);">
                                                                        <title>
                                                                            <asp:Label ID="lblTimeZoneTitle" runat="server" Text="Zona horaria ${Terminal}"></asp:Label>
                                                                        </title>
                                                                        <description>
                                                                            <asp:Label ID="lblTimeZoneDesc" runat="server" Text="El ${Terminal} utiliza una zona horaria distinta a la del servidor."></asp:Label>
                                                                        </description>
                                                                        <content>
                                                                            <table>
                                                                                <tr>
                                                                                    <td style="padding-left: 50px;">
                                                                                        <asp:Label ID="lblTimeZoneName" runat="server" Text="Zona horaria: "></asp:Label></td>
                                                                                    <td style="padding-left: 5px;">
                                                                                        <dx:aspxcombobox id="cmbTimeZones" runat="server" width="250px">
                                                                                            <clientsideevents selectedindexchanged="function(s,e){ timezoneChange(s,e); hasChanges(true); }" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                                                        </dx:aspxcombobox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td colspan="2" style="padding-left: 50px;">
                                                                                        <div id="divAutoDaylight" runat="server">
                                                                                            <input type="checkbox" runat="server" id="chkAutoDaylight" />&nbsp;<a href="javascript: void(0);" onclick="CheckLinkChange('<%= chkAutoDaylight.ClientID %>');"><asp:Label ID="lblAutoDaylight" runat="server" Text="Cambiar la hora automáticamente según el horario de verano"></asp:Label></a>
                                                                                        </div>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </content>
                                                                    </rousercontrols:rooptionpanelclient>
                                                                </td>
                                                            </tr>

                                                            <tr>
                                                                <td id="tdCaptureImage" colspan="4" style="display: none;" runat="server">
                                                                    <rousercontrols:rooptionpanelclient id="chkCaptureImage" runat="server" typeopanel="CheckboxOption" width="100%" height="Auto" checked="False" enabled="True" border="True" cconclick="hasChanges(true);">
                                                                        <title>
                                                                            <asp:Label ID="lblCaptureImageTitle" runat="server" Text="Capturar imagen al fichar"></asp:Label>
                                                                        </title>
                                                                        <description>
                                                                            <asp:Label ID="lblCaptureImageDesc" runat="server" Text="El ${Terminal} realiza una foto al fichar un ${Employee}."></asp:Label>
                                                                        </description>
                                                                        <content>
                                                                        </content>
                                                                    </rousercontrols:rooptionpanelclient>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="2">
                                                        <!-- Parametres conf. terminal -->
                                                    </td>
                                                </tr>
                                            </table>

                                            <div id="divNFC" runat="server">
                                                <div class="panHeader2">
                                                    <span style="">
                                                        <asp:Label runat="server" ID="titleNFC" Text="Tags NFC"></asp:Label></span>
                                                </div>
                                                <br />
                                                <div>
                                                    <span>En esta sección puedes gestionar los tags NFC.</span><br />
                                                    <br />
                                                    <%-- <div class="jsGrid">
                                                                        <asp:Label ID="NFCTagsTitle" runat="server" CssClass="jsGridTitle" Text="Tags NFC"></asp:Label>--%>
                                                    <%-- <div class="jsgridButton">
                                                                             <div class="btnFlat">
                                                                                 <a href="javascript: void(0)" id="addNewTag" runat="server" onclick="AddNewNFCTags();">
                                                                                     <span class="btnIconAdd"></span>
                                                                                     <asp:Label ID="addNewNFCTags" runat="server" Text="Añadir"></asp:Label>
                                                                                 </a>
                                                                             </div>
                                                                         </div>
                                                                     </div>--%>

                                                    <div id="divNFCTags" runat="server" class="jsGridContent dextremeGrid">
                                                    </div>
                                                </div>
                                            </div>

                                            <div id="panUSB" runat="server">
                                                <div class="panHeader2">
                                                    <span style="">
                                                        <asp:Label runat="server" ID="lblUSBTitle" Text="USB"></asp:Label></span>
                                                </div>
                                                <br />
                                                <div>
                                                    <span>En esta sección se gestiona la interacción del terminal mediante USB.</span><br />
                                                    <br />
                                                    <asp:Button ID="btUSBdownSoft" Text="Descargar programa" runat="server" TabIndex="5" CssClass="btnFlat btnFlatAsp" />
                                                    <asp:Button ID="btUSBdownConfig" Text="Descargar configuración" runat="server" TabIndex="5" CssClass="btnFlat btnFlatAsp" OnClientClick="PerformValidation();return false;" />
                                                    <asp:Button ID="btUSBupConfig" Text="Cargar configuración" runat="server" TabIndex="5" CssClass="btnFlat btnFlatAsp" OnClientClick="showPopUp('UploadDataFile.aspx'); return false;" />

                                                    <div id="subPanelUSBUpload" class="optionPanelRoboticsV2" style="display: none; margin-top: 5px;">
                                                        <span>Seleccione el fichero de datos del terminal para ser procesado.</span><br />
                                                        <asp:FileUpload ID="USBFileUpload" runat="server" Width="400" size="50" /><br />
                                                        <asp:Button ID="btUSBuploadFile" Text="Cargar configuración" runat="server" TabIndex="5" CssClass="btnFlat btnFlatAsp" Style="margin-top: 5px;" />
                                                    </div>
                                                </div>
                                                <!-- POPUP NEW OBJECT -->
                                                <dx:aspxpopupcontrol id="PopupTerminalFile" runat="server" allowdragging="False" closeaction="None" modal="True" contenturl="UploadDataFile.aspx"
                                                    popupverticalalign="WindowCenter" popuphorizontalalign="WindowCenter" width="460px" height="165px"
                                                    showheader="False" scrollbars="Auto" showpagescrollbarwhenmodal="True" clientinstancename="PopupTerminalFile_Client"
                                                    popupanimationtype="None" backcolor="Transparent" contentstyle-paddings-padding="0px" border-bordercolor="Transparent" showshadow="false">
                                                    <settingsloadingpanel enabled="false" />
                                                </dx:aspxpopupcontrol>
                                            </div>
                                            <br />
                                        </div>

                                        <!-- Panell Lectors -->
                                        <div id="div02" class="contentPanel" style="display: none;" runat="server" name="menuPanel">
                                            <div class="panHeader2">
                                                <span style="">
                                                    <asp:Label runat="server" ID="lblReadersTitle" Text="Lectores"></asp:Label></span>
                                            </div>
                                            <br />
                                            <table width="100%" border="0">
                                                <tr>
                                                    <td>
                                                        <rousercontrols:rotabcontainerclient id="tabCtl01" runat="server" oneventtabclick="ctlTabPositions">
                                                            <tabtitle1>
                                                            </tabtitle1>
                                                            <tabcontainer1>
                                                                <roforms:frmterminalreader id="frmTR1" runat="server" idreader="1" />
                                                            </tabcontainer1>
                                                            <tabtitle2>
                                                            </tabtitle2>
                                                            <tabcontainer2>
                                                                <roforms:frmterminalreader id="frmTR2" runat="server" idreader="2" />
                                                            </tabcontainer2>
                                                            <tabtitle3>
                                                            </tabtitle3>
                                                            <tabcontainer3>
                                                                <roforms:frmterminalreader id="frmTR3" runat="server" idreader="3" />
                                                            </tabcontainer3>
                                                            <tabtitle4>
                                                            </tabtitle4>
                                                            <tabcontainer4>
                                                                <roforms:frmterminalreader id="frmTR4" runat="server" idreader="4" />
                                                            </tabcontainer4>
                                                        </rousercontrols:rotabcontainerclient>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>

                                        <!-- Panell Sirenes -->
                                        <div id="div03" class="contentPanel" style="display: none;" runat="server" name="menuPanel">
                                            <div class="panHeader2">
                                                <span style="">
                                                    <asp:Label runat="server" ID="lblSirensTitle" Text="Sirenas"></asp:Label></span>
                                            </div>
                                            <br />
                                            <table width="100%" border="0" style="padding-top: 5px; padding-left: 50px; padding-right: 50px;">
                                                <tr>
                                                    <td style="width: 50px;">
                                                        <img src="Images/Sirens48.png" alt="" /></td>
                                                    <td>
                                                        <asp:Label ID="lblSirens" runat="server" Text="Información de los toques de sirena que hay programados para este Terminal. Desde aquí, puede darlos de alta, baja y modificarlos."></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="2">
                                                        <table>
                                                            <tr>
                                                                <td>
                                                                    <asp:Label ID="lblActRelay" runat="server" Text="Las sirenas activarán el relé "></asp:Label></td>
                                                                <td style="padding-left: 10px;">
                                                                    <dx:aspxcombobox id="cmbRelay" runat="server" width="50px">
                                                                        <clientsideevents selectedindexchanged="function(s,e){ hasChanges(true); }" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                                    </dx:aspxcombobox>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="2" align="center">
                                                        <!-- Grid JS -->
                                                        <div class="jsGrid">
                                                            <asp:Label ID="lblSirensGridTitle" runat="server" CssClass="jsGridTitle" Text="Sirenes"></asp:Label>
                                                            <div class="jsgridButton">
                                                                <div id="panTbSirens" runat="server" class="btnFlat">
                                                                    <a href="javascript: void(0)" id="btnAddSiren" runat="server" onclick="AddNewSiren();">
                                                                        <span class="btnIconAdd"></span>
                                                                        <asp:Label ID="lblAddSiren" runat="server" Text="Añadir"></asp:Label>
                                                                    </a>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div id="grdSirens" class="jsGridContent"></div>
                                                        <roformssiren:frmaddsiren id="frmAddSiren1" runat="server" />
                                                    </td>
                                                </tr>
                                            </table>
                                            <rousercontrols:rooptpanelclientgroup id="optClientGroup" runat="server" />
                                        </div>
                                    </div>
                                </div>

                                <div id="divMsgBottom" class="divMsg2 divMessageBottom" style="display: none">
                                    <div class="divImageMsg">
                                        <img alt="" id="Img2" src="~/Base/Images/MessageFrame/Alert16.png" runat="server" />
                                    </div>
                                    <div class="messageText">
                                        <span id="msgBottom"></span>
                                    </div>
                                    <div align="right" class="messageActions">
                                        <a href="javascript: void(0);" class="aMsg" onclick="saveChanges();"><span id="lblSaveChangesBottom" runat="server" /></a>
                                        &nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;
                                        <a href="javascript: void(0);" onclick="undoChanges();" class="aMsg"><span id="lblUndoChangesBottom" runat="server" /></a>
                                    </div>
                                </div>
                            </dx:panelcontent>
                        </panelcollection>
                    </dx:aspxcallbackpanel>
                </div>
            </div>
        </div>

        <!-- POPUP NEW OBJECT -->
        <dx:aspxpopupcontrol id="CaptchaObjectPopup" runat="server" allowdragging="False" closeaction="None" modal="True" contenturl="~/Base/Popups/GenericCaptchaValidator.aspx"
            popupverticalalign="WindowCenter" popuphorizontalalign="WindowCenter" modalbackgroundstyle-opacity="0" width="500" height="320"
            showheader="False" scrollbars="Auto" showpagescrollbarwhenmodal="True" clientinstancename="CaptchaObjectPopup_Client" popupanimationtype="None" backcolor="Transparent" contentstyle-paddings-padding="0px" border-bordercolor="Transparent" showshadow="false">
            <settingsloadingpanel enabled="false" />
        </dx:aspxpopupcontrol>

        <!-- POPUP NEW OBJECT -->
        <dx:aspxpopupcontrol id="AspxLoadingPopup" runat="server" allowdragging="False" closeaction="None" modal="True" contenturl="~/Base/Popups/PerformingAction.aspx"
            popupverticalalign="WindowCenter" popuphorizontalalign="WindowCenter" modalbackgroundstyle-opacity="0" width="460" height="260"
            showheader="False" scrollbars="Auto" showpagescrollbarwhenmodal="True" clientinstancename="AspxLoadingPopup_Client" popupanimationtype="None" backcolor="Transparent" contentstyle-paddings-padding="0px" border-bordercolor="Transparent" showshadow="false">
            <settingsloadingpanel enabled="false" />
        </dx:aspxpopupcontrol>
    </div>

    <script language="javascript" type="text/javascript">

        function resizeTreeTerminals() {
            try {
                var ctlPrefix = "ctl00_contentMainBody_roTreesTerminals";
                eval(ctlPrefix + "_resizeTrees();");
            }
            catch (e) {
                showError("resizeTreeTerminals", e);
            }
        }

        function resizeFrames() {
            var divMainBodyHeight = $("#divMainBody").outerHeight(true);
            var divHeight = 0;
            if (divMainBodyHeight < 525) {
                divHeight = 525 - $("#divTabInfo").outerHeight(true);
            }
            else {
                divHeight = divMainBodyHeight - $("#divTabInfo").outerHeight();
            }

            $("#divTabData").height(divHeight - 10);

            var divTreeHeight = $("#divTree").height();
            $("#ctlTreeDiv").height(divTreeHeight);
        }

        window.onresize = function () {
            resizeFrames();
            resizeTreeTerminals();
        }
    </script>
</asp:Content>
