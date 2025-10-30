<%@ Page Language="VB" MasterPageFile="~/Base/mastEmp.master" AutoEventWireup="false" Inherits="VTLive40.Groups" Title="${Employees} y ${Groups}" EnableEventValidation="false" CodeBehind="Groups.aspx.vb" %>

<%@ Register Src="~/Employees/WebUserControls/DocumentManagment.ascx" TagPrefix="roForms" TagName="DocumentManagment" %>
<%@ Register Src="~/Employees/WebUserControls/DocumentPendingManagment.ascx" TagPrefix="roForms" TagName="DocumentPendingManagment" %>

<asp:Content ID="cMainBody" ContentPlaceHolderID="contentMainBody" runat="Server">

    <script language="javascript" type="text/javascript">

        function PageBase_Load() {
            ConvertControls('divContenido');
            resizeFrames();
            resizeTreeEmployees();

            //PPR desactivado temporalmente NO ELIMINAR--> loadInitialPageValues();
        }
    </script>
    <input type="hidden" id="msgHasChanges" value="" runat="server" clientidmode="Static" />
    <input type="hidden" id="msgHasErrors" value="" runat="server" clientidmode="Static" />
    <input type="hidden" value="/#/./Employees/Employees" runat="server" id="EmployeeURI" />
    <input type="hidden" id="hdnSelectedGroup" runat="server" value="" />
    <div id="divMainBody">

        <!-- TAB SUPERIOR -->
        <div id="divTabInfo" class="divDataCells">
            <div style="min-height: 10px"></div>
            <div id="divEmpleados" class="blackRibbonTitle">
            </div>
            <div style="min-height: 10px"></div>
        </div>
        <!-- FIN TAB SUPERIOR -->

        <!-- ARBOL Y DETALLE -->
        <div id="divTabData" class="divDataCells">

            <div id="divTree" class="treeSize">
                <div id="ctlTreeDiv" style="height: 500px;">
                    <rws:rotreesselector id="roTreeGroups1" runat="server" filterfloat="false" prefixtree="roTreeGroups1"
                        tree1visible="true" tree1multisel="false" tree1showonlygroups="true" tree1function="cargaNodo"
                        featurealias="Employees" featuretype="U" showtreecaption="true" showemployeefilters="false">
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

                                <div id="divContentPanels" class="divContentPanelsWithOutMessage">
                                    <div id="companyRow" runat="server" class="contentPanel">
                                        <!-- Panell General -->
                                        <div id="panCompanyGeneral" class="contentPanel" style="display: none" runat="server">
                                            <div class="panHeader2">
                                                <span style="">
                                                    <asp:Label runat="server" ID="lblTitleDescriptionGroup" Text="General"></asp:Label></span>
                                            </div>

                                            <div id="divAdviceCompany" runat="server" style="margin: 12px; width: 95%;">
                                                <asp:Label ID="lblAdviceCompany" runat="server" Text="" CssClass="editTextFormat" Style="font-weight: bold;"></asp:Label>
                                            </div>

                                            <div class="panBottomMargin">
                                                <div class="divRow">
                                                    <div class="divRowDescription">
                                                        <asp:Label ID="lblCompanyNameDescription" runat="server" Text="Nombre identificativo del grupo de empleados"></asp:Label>
                                                    </div>
                                                    <asp:Label ID="lblCompanyName" runat="server" Text="Nombre:" class="labelForm"></asp:Label>
                                                    <div class="componentForm">
                                                        <dx:aspxtextbox id="txtNameCompany" runat="server" width="350px" clientinstancename="txtNameCompany_Client" nulltext="_____">
                                                            <clientsideevents validation="LengthValidation" textchanged="function(s,e){checkCompanyEmptyName(s.GetValue());}" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                            <validationsettings setfocusonerror="True" validationgroup="employeenameGroup">
                                                                <requiredfield isrequired="True" errortext="(*)" />
                                                            </validationsettings>
                                                        </dx:aspxtextbox>
                                                    </div>
                                                </div>
                                                <div id="descriptionGroupRow" runat="server" class="divRow">
                                                    <div class="divRowDescription">
                                                        <asp:Label ID="grouplblDescriptionDesc" runat="server" Text="Descripción del grupo de trabajo"></asp:Label>
                                                    </div>
                                                    <asp:Label ID="grouplblDescription" runat="server" Text="Descripción:" class="labelForm"></asp:Label>
                                                    <div class="componentForm">
                                                        <dx:aspxmemo id="txtDescription" runat="server" rows="2" width="545" height="40">
                                                            <clientsideevents textchanged="function(s,e){ hasChanges(true)}" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                        </dx:aspxmemo>
                                                    </div>
                                                </div>
                                                <div id="exportGroupRow" runat="server" class="divRow">
                                                    <div class="divRowDescription">
                                                        <asp:Label ID="grouplblExportDesc" runat="server" Text="Equivalencia."></asp:Label>
                                                    </div>
                                                    <asp:Label ID="grouplblExport" runat="server" Text="Equivalencia:" CssClass="labelForm"></asp:Label>
                                                    <div class="componentForm">
                                                        <dx:aspxtextbox id="txtExport" runat="server">
                                                            <clientsideevents textchanged="function(s,e){hasChanges(true, false);}" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                            <masksettings includeliterals="None" />
                                                        </dx:aspxtextbox>
                                                    </div>
                                                </div>
                                            </div>

                                            <div id="divMultiTimezone" runat="server" style="display: none">
                                                <div class="panHeader2">
                                                    <span style="">
                                                        <asp:Label runat="server" ID="lblDefaultZone" Text="Zonas por defecto"></asp:Label></span>
                                                </div>

                                                <div class="panBottomMargin">
                                                    <div class="divRow">
                                                        <div class="divRowDescription">
                                                            <asp:Label ID="lblWorkingZoneDescription" runat="server" Text="Zona utilizada por defecto para los fichajes que se origen sea el portal del empleado."></asp:Label>
                                                        </div>
                                                        <asp:Label ID="lblWoringZoneTitle" runat="server" Text="Zona de trabajo:" CssClass="labelForm"></asp:Label>
                                                        <div class="componentForm">
                                                            <dx:aspxcombobox id="cmbWorkingZone" runat="server" width="200px" clientinstancename="cmbWorkingZoneClient">
                                                                <clientsideevents selectedindexchanged="function(s,e){ hasChanges(true)}" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                                <validationsettings errordisplaymode="None">
                                                                </validationsettings>
                                                            </dx:aspxcombobox>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div id="divCompanyCloseDate" runat="server">
                                                <div class="panHeader2">
                                                    <span style="">
                                                        <asp:Label runat="server" ID="lblCompanyCloseDate" Text="Fecha de bloqueo de restaurante"></asp:Label></span>
                                                </div>

                                                <div class="panBottomMargin">
                                                    <div class="divRow">
                                                        <div class="divRowDescription">
                                                            <asp:Label ID="lblCompanyCloseInfo" runat="server" Text="Aquí puede definir la fecha de bloqueo de funcionalidad. Para fechas anteriores a la indicada, los permisos sobre el calendario serán como mucho de lectura."></asp:Label>
                                                        </div>
                                                        <asp:Label ID="lblCompanyClose" runat="server" Text="Fecha:" CssClass="labelForm"></asp:Label>
                                                        <div class="componentForm">
                                                            <dx:aspxdateedit allownull="true" id="txtCloseDate" runat="server" width="200px" clientinstancename="cmbWorkingZoneClient">
                                                                <clientsideevents datechanged="function(s,e){ hasChanges(true)}" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                                <validationsettings errordisplaymode="None">
                                                                </validationsettings>
                                                            </dx:aspxdateedit>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                            <br />

                                            <div id="divCompanyUserFields" class="panBottomMargin" runat="server">

                                                <!-- Este div es el header -->
                                                <div class="panBottomMargin">
                                                    <div class="panHeader2 panBottomMargin">
                                                        <span class="panelTitleSpan">
                                                            <asp:Label runat="server" ID="lblFieldsCompany" Text="Ficha de empresa"></asp:Label>
                                                        </span>
                                                    </div>
                                                </div>
                                                <div class="jsGrid">
                                                    <asp:Label ID="groupLabel6" runat="server" CssClass="jsGridTitle" Text="Ficha de empleado"></asp:Label>
                                                    <div id="btn1FieldsCompany" runat="server" class="jsgridButton">
                                                        <div class="btnFlat">
                                                            <a href="javascript: void(0)" id="editFieldsGrid" runat="server" onclick="">
                                                                <span class="btnIconEdit"></span>
                                                                <asp:Label ID="groupLabel7" runat="server" Text="Editar"></asp:Label>
                                                            </a>
                                                        </div>
                                                    </div>
                                                    <div id="btn3FieldsCompany" runat="server" class="jsgridButton" visible="false">
                                                        <div class="btnFlat">
                                                            <a href="javascript: void(0)" id="cancelFieldsGrid" runat="server" onclick="">
                                                                <span class="btnIconCancel"></span>
                                                                <asp:Label ID="groupLabel9" runat="server" Text="Cancelar"></asp:Label>
                                                            </a>
                                                        </div>
                                                    </div>
                                                    <div id="btn2FieldsCompany" runat="server" class="jsgridButton" visible="false">
                                                        <div class="btnFlat">
                                                            <a href="javascript: void(0)" id="saveFieldsGrid" runat="server" onclick="">
                                                                <span class="btnIconSave"></span>
                                                                <asp:Label ID="groupLabel8" runat="server" Text="Guardar"></asp:Label>
                                                            </a>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div id="divCompanyFields" runat="server" class="jsGridContent">
                                                    <!-- Campos de la ficha de la empresa actual -->
                                                </div>
                                            </div>
                                        </div>

                                        <!-- Area CAMPOS FICHA EMPRESA -->
                                        <div id="panEmployees" class="contentPanel" style="display: none" runat="server">
                                            <!-- Grid ACTUAL -->
                                            <div class="panHeader2">
                                                <span style="">
                                                    <asp:Label runat="server" ID="lblTitleEmpActGroup" Text="Empleados que actualmente estan en el grupo"></asp:Label></span>
                                            </div>

                                            <table style="margin: 10px; width: 95%;">
                                                <tr>
                                                    <td>
                                                        <table width="100%" border="0" cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td height="20px" valign="top">
                                                                    <div id="divHeaderGrupActual" runat="server" style="width: auto; height: auto; overflow: auto;">
                                                                        <!-- Carrega la capcelera del Grid Grup actualment -->
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td valign="top">
                                                                    <div id="divGridGrupActual" runat="server" style="width: 640px; height: 165px; overflow: auto;">
                                                                        <!-- Carrega del Grid Grup actualment -->
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>

                                            <br />
                                            <!-- Grid TRANSIT -->
                                            <div class="panHeader2">
                                                <span style="">
                                                    <asp:Label ID="lblTitleEmpMove" runat="server" Text="Empleados en tránsito hacia el grupo"></asp:Label>
                                                </span>
                                            </div>

                                            <table style="margin: 10px; width: 95%;">
                                                <tr>
                                                    <td>
                                                        <table width="100%" border="0" cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td height="20px" valign="top">
                                                                    <div id="divHeaderGrupTransit" runat="server" style="width: auto; height: auto; overflow: auto;">
                                                                        <!-- Carrega la capcelera del Grid Grup actualment -->
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td valign="top">
                                                                    <div id="divGridGrupTransit" runat="server" style="width: 640px; height: 165px; overflow: auto;">
                                                                        <!-- Carrega del Grid Grup actualment -->
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>

                                            <br />
                                            <!-- Grid PASSAT -->
                                            <div class="panHeader2">
                                                <span style="">
                                                    <asp:Label ID="lblTitleEmpPast" runat="server" Text="Empleados que estuvieron en este grupo"></asp:Label>
                                                </span>
                                            </div>

                                            <table style="margin: 10px; width: 95%;">
                                                <tr>
                                                    <td>
                                                        <table width="100%" border="0" cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td height="20px" valign="top">
                                                                    <div id="divHeaderGrupPasat" runat="server" style="width: auto; height: auto; overflow: auto;">
                                                                        <!-- Carrega la capcelera del Grid Grup actualment -->
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td valign="top">
                                                                    <div id="divGridGrupPasat" runat="server" style="width: 640px; height: 165px; overflow: auto;">
                                                                        <!-- Carrega del Grid Grup actualment -->
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>
                                            <!-- Fin GRID ACTUAL-->
                                        </div>

                                        <div id="panGrpIndicators" class="contentPanel" style="display: none" runat="server">
                                            <!-- PANELL INDICADORES -->
                                            <div id="divCompanyIndicators" runat="server">
                                                <div class="panHeader2">
                                                    <span style="">
                                                        <asp:Label ID="lblIndicadores" runat="server" Text="Indicadores"></asp:Label></span>
                                                </div>
                                                <br />
                                                <div class="divEmployee-Class">
                                                    <table border="0" width="100%" height="50px">
                                                        <tr>
                                                            <td width="48px" height="48px">
                                                                <img id="Img11" src="~/Base/Images/Indicators/Indicators.png" style="border: 0;" runat="server" /></td>
                                                            <td valign="top" align="left">
                                                                <span id="span2" runat="server" class="spanEmp-Class">
                                                                    <asp:Label ID="groupLabel4" runat="server" Text="Desde esta página puede gestionar los indicadores asignados a los grupos."></asp:Label>
                                                                </span>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>

                                                <div id="div4" runat="server" style="margin-bottom: 5px; margin-left: 30px; width: 45%;">
                                                    <table border="0" width="100%" height="50px">
                                                        <tr>
                                                            <td align="right">
                                                                <table id="tblAddInd" runat="server" border="0" cellpadding="0" cellspacing="0" style="width: 100%;">
                                                                    <tr>
                                                                        <td align="right">
                                                                            <div class="btnFlat">
                                                                                <a href="javascript: void(0)" id="btnAddIndicator" runat="server" onclick="">
                                                                                    <asp:Label ID="lblEditIndicator" runat="server" Text="Gestionar"></asp:Label>
                                                                                </a>
                                                                            </div>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <div id="gridIndicators" runat="server">
                                                                    <!-- Aqui va el grid de Indicadores -->
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                            </div>
                                        </div>

                                        <div id="panGrpCostCenters" class="contentPanel" style="display: none" runat="server">
                                            <!-- PANELL CENTROS DE COSTE -->
                                            <div id="divGrpCostCenters" runat="server">
                                                <div class="panHeader2">
                                                    <span style="">
                                                        <asp:Label ID="lblCentrosCoste" runat="server" Text="Centros de Coste"></asp:Label></span>
                                                </div>
                                                <br />
                                                <div class="divEmployee-Class">
                                                    <table border="0" width="100%" height="50px">
                                                        <tr>
                                                            <td width="48px" height="48px">
                                                                <img id="ImgBusinessCenter" src="~/Tasks/Images/BusinessCenters48.png" style="border: 0;" runat="server" /></td>
                                                            <td valign="top" align="left">
                                                                <span id="span5" runat="server" class="spanEmp-Class">
                                                                    <asp:Label ID="lblCostControlDescr" runat="server" Text="Desde esta página puede indicar el centro de coste al que estará asignado el grupo."></asp:Label>
                                                                </span>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                                <div class="panBottomMargin">
                                                    <div class="divRow">
                                                        <div class="divRowDescription" style="padding-left: 48px !important;">
                                                            <asp:Label ID="lblCostCenterDescription" ForeColor="#76859F" Font-Bold="true" runat="server" Text="Aquí puede indicar el Centro de Coste por defecto de los empleados que se encuentren en este grupo. Las horas se imputarán a este Centro de Coste siempre que no haya cesiones u horarios que fuercen un centro distinto."></asp:Label>
                                                        </div>
                                                        <div id="divCostDetail" runat="server">
                                                            <div id="divCostopt1" style="width: 60%; height: 100%; padding-left: 40px; padding-top: 10px" runat="server">
                                                                <rousercontrols:rooptionpanelclient id="optHere" runat="server" typeopanel="RadioOption" width="80%" height="Auto" enabled="True" border="True" cconclick="hasChanges(true);">
                                                                    <title>
                                                                        <asp:Label ID="lblHere" runat="server" Text="Usar el mismo Centro de Coste que el del grupo superior"></asp:Label>
                                                                    </title>
                                                                    <description>
                                                                        <table width="100%">
                                                                            <tr>
                                                                                <td align="left" width="70%" style="padding-right: 10px;" class="spanEmp-Class">
                                                                                    <asp:Label ID="lblHereDesc" runat="server" Text=""></asp:Label>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </description>
                                                                    <content>
                                                                    </content>
                                                                </rousercontrols:rooptionpanelclient>

                                                                <rousercontrols:rooptionpanelclient id="optOneCost" runat="server" typeopanel="RadioOption" width="80%" height="Auto" enabled="True" border="True" cconclick="hasChanges(true);">
                                                                    <title>
                                                                        <asp:Label ID="lblOneCost" runat="server" Text="Usar un Centro de Coste distinto:"></asp:Label>
                                                                    </title>
                                                                    <content>
                                                                        <table width="100%">
                                                                            <tr>
                                                                                <td align="left" width="70%" style="">
                                                                                    <div class="componentForm" style="padding-left: 30px;">
                                                                                        <dx:aspxcombobox id="cmbCostCenter" runat="server" width="300px" clientinstancename="cmbCostCenterClient">
                                                                                            <clientsideevents selectedindexchanged="function(s,e){ hasChanges(true)}" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                                                            <validationsettings errordisplaymode="None">
                                                                                            </validationsettings>
                                                                                        </dx:aspxcombobox>
                                                                                    </div>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </content>
                                                                </rousercontrols:rooptionpanelclient>
                                                            </div>

                                                            <div class="divRowDescription" style="padding-top: 25px; clear: both; padding-left: 48px !important;">
                                                                <asp:Label ID="lblFreezerDateCost" ForeColor="#76859F" Font-Bold="true" runat="server" Text="IMPORTANTE: En el caso que se modifique el centro de coste se recalcularán todos los datos a partir de la fecha de congelación."></asp:Label>
                                                            </div>
                                                        </div>
                                                        <div id="divCostDetailNoLicense" runat="server">
                                                            <div class="divRowDescription">
                                                                <asp:Label ID="lblNotLicenseCostControl" ForeColor="#2D4155" Font-Bold="true" runat="server" Text="Debe aquirir la licencia de control de costes para poder utilizar dicha funcionalidad."></asp:Label>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <!-- PANEL DOCUMENTOS -->
                                        <div id="panCompanyDocs" class="contentPanel" style="display: none" runat="server">
                                            <div id="divCompanyPending" runat="server" style="margin-bottom: 35px;">
                                                <div class="panHeader2">
                                                    <span style="">
                                                        <asp:Label ID="lblCompanyPendingDocs" runat="server" Text="Alertas de documentación"></asp:Label></span>
                                                </div>
                                                <br />
                                                <!-- Este div es un formulario -->
                                                <div class="panBottomMargin" style="margin-right: 25px;">
                                                    <roforms:documentpendingmanagment runat="server" id="CompanyDocumentPendingManagment" />
                                                </div>
                                            </div>

                                            <div id="divCompanyDelivered" runat="server" style="clear: both; margin-bottom: 35px;">
                                                <div class="panHeader2">
                                                    <span style="">
                                                        <asp:Label ID="lblCompanyDeliveredDocs" runat="server" Text="Documentos de empresa"></asp:Label></span>
                                                </div>
                                                <br />
                                                <div class="divEmployee-Class">
                                                    <table border="0" width="100%" height="50px">
                                                        <tr>
                                                            <td width="48px" height="48px">
                                                                <img id="Img16" src="~/Base/Images/Documents/Documents.png" style="border: 0;" runat="server" /></td>
                                                            <td valign="top" align="left">
                                                                <span id="span10" runat="server" class="spanEmp-Class">
                                                                    <asp:Label ID="lblCompanyDeliveredDocsDesc" runat="server" Text="En esta sección puede consultar los documentos asignados a la empresa."></asp:Label>
                                                                </span>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                                <br />
                                                <!-- Este div es un formulario -->
                                                <div class="panBottomMargin" style="margin-right: 25px;">
                                                    <roforms:documentmanagment runat="server" id="CompanyDocumentManagment" />
                                                </div>
                                            </div>
                                        </div>

                                        <!-- PANEL DOCUMENTOS GRUPO -->
                                        <div id="panGroupDocs" class="contentPanel" style="display: none" runat="server">
                                            <div id="divGroupPending" runat="server" style="margin-bottom: 35px;">
                                                <div class="panHeader2">
                                                    <span style="">
                                                        <asp:Label ID="lblGroupPendingDocs" runat="server" Text="Alertas de documentación"></asp:Label></span>
                                                </div>
                                                <br />
                                                <!-- Este div es un formulario -->
                                                <div class="panBottomMargin" style="margin-right: 25px;">
                                                    <roforms:documentpendingmanagment runat="server" id="GroupDocumentPendingManagment" />
                                                </div>
                                            </div>

                                            <div id="divGroupDelivered" runat="server" style="clear: both; margin-bottom: 35px;">
                                                <div class="panHeader2">
                                                    <span style="">
                                                        <asp:Label ID="lblGroupDeliveredDocs" runat="server" Text="Documentos de grupo"></asp:Label></span>
                                                </div>
                                                <br />
                                                <div class="divEmployee-Class">
                                                    <table border="0" width="100%" height="50px">
                                                        <tr>
                                                            <td width="48px" height="48px">
                                                                <img id="Img3" src="~/Base/Images/Documents/Documents.png" style="border: 0;" runat="server" /></td>
                                                            <td valign="top" align="left">
                                                                <span id="span1" runat="server" class="spanEmp-Class">
                                                                    <asp:Label ID="lblGroupDeliveredDocsDesc" runat="server" Text="En esta sección puede consultar los documentos asignados al grupo."></asp:Label>
                                                                </span>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                                <br />
                                                <!-- Este div es un formulario -->
                                                <div class="panBottomMargin" style="margin-right: 25px;">
                                                    <roforms:documentmanagment runat="server" id="GroupDocumentManagment" />
                                                </div>
                                            </div>
                                        </div>

                                        <!-- Panel accesos -->
                                        <div id="panCompanyAccess" class="contentPantel" style="display: none" runat="server">
                                            <!-- PANEL Autorizaciones -->
                                            <div id="divGroupAuthorizations" runat="server" style="margin-bottom: 35px;">
                                                <div class="panHeader2">
                                                    <span style="">
                                                        <asp:Label ID="lblGroupAuthorizations" runat="server" Text="Autorizaciones de acceso"></asp:Label></span>
                                                </div>
                                                <br />
                                                <div class="divEmployee-Class">
                                                    <table border="0" width="100%" height="50px">
                                                        <tr>
                                                            <td width="60px" height="48px">
                                                                <a href="javascript:void(0)" id="aCompanyAccessAuthorizations" runat="server" onclick="">
                                                                    <img alt="" id="imgGroupAccessAuthorization" src="~/Access/Images/AccessGroup.png" style="border: 0;" runat="server" />
                                                                </a>
                                                            </td>
                                                            <td valign="middle" align="left">
                                                                <a href="javascript:void(0)" onclick="document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_aCompanyAccessAuthorizations').onclick();">
                                                                    <span id="span11" runat="server" class="spanEmp-Class" style="cursor: pointer;">
                                                                        <asp:Label ID="lblCompanyAuthorizationsDesc" runat="server" Text="Configurar todas los autorizaciones a grupos de acceso donde los empleados del grupo puede acceder"></asp:Label>
                                                                    </span>
                                                                </a>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="left" colspan="2" style="padding-left: 55px;">
                                                                <div class="btnFlatWithMaxTextWidth">
                                                                    <a href="javascript: void(0)" id="A10" runat="server" onclick="document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_aCompanyAccessAuthorizations').onclick();">
                                                                        <span class="btnIconEdit"></span>
                                                                        <asp:Label ID="lblConfigCompanyAuthorizations" runat="server" Text="Configurar autorizaciones"></asp:Label>
                                                                    </a>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                            </div>
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
        <dx:aspxpopupcontrol id="AlertDetailsPopup" runat="server" allowdragging="False" closeaction="None" modal="True" contenturl="~/Alerts/AlertsDetails.aspx"
            popupverticalalign="WindowCenter" popuphorizontalalign="WindowCenter" minwidth="870px" width="870px" minheight="370px" height="370px" cssclass="bodyPopupExtended"
            showheader="False" scrollbars="Auto" showpagescrollbarwhenmodal="True" clientinstancename="AlertDetailsPopup_Client" popupanimationtype="None" contentstyle-paddings-padding="0px" showshadow="false">
            <clientsideevents shown="function(s,e){ s.SetWidth(870); }" />
            <settingsloadingpanel enabled="false" />
        </dx:aspxpopupcontrol>
    </div>

    <script language="javascript" type="text/javascript">

        function resizeTreeEmployees() {
            try {
                var ctlPrefix = "<%= roTreeGroups1.ClientID %>";
                eval(ctlPrefix + "_resizeTrees();");
            }
            catch (e) {
                showError("resizeTreeEmployees", e);
            }
        }

        function resizeFrames() {
            //var viewportWidth = $(window).width();
            //var viewportHeight = $(window).height();

            //ALTO
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

        //PPR desactivado temporalmente NO ELIMINAR-->
        //    function loadInitialPageValues() {
        //        var oQueryStringState = new roQueryStringState("Employees");

        //        if (oQueryStringState.ActiveTab != "")
        //            actualTab = oQueryStringState.ActiveTab;

        //        if (oQueryStringState.Show != "")
        //            eval(oQueryStringState.Show);
        //
        //        oQueryStringState.clear();
        //    }

        window.onresize = function () {
            resizeFrames();
            resizeTreeEmployees();
        }
    </script>
</asp:Content>