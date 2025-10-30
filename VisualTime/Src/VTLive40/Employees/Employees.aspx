<%@ Page Language="VB" MasterPageFile="~/Base/mastEmp.master" AutoEventWireup="false" Inherits="VTLive40.Employees" Title="${Employees} y ${Groups}" EnableEventValidation="false" CodeBehind="Employees.aspx.vb" %>

<%@ Register Src="~/Scheduler/Controls/LocalizationMapControl.ascx" TagPrefix="uc1" TagName="LocalizationMapControl" %>
<%@ Register Src="~/Employees/WebUserControls/DocumentManagment.ascx" TagPrefix="roForms" TagName="DocumentManagment" %>
<%@ Register Src="~/Employees/WebUserControls/DocumentCompany.ascx" TagPrefix="roForms" TagName="DocumentCompany" %>
<%@ Register Src="~/Employees/WebUserControls/DocumentPendingManagment.ascx" TagPrefix="roForms" TagName="DocumentPendingManagment" %>
<%@ Register Src="~/Employees/WebUserControls/DocumentEmployees.ascx" TagPrefix="roForms" TagName="DocumentEmployees" %>
<%@ Register Src="~/Base/WebUserControls/roTreeV3.ascx" TagName="roTreeV3" TagPrefix="rws" %>
<%@ Register Src="~/Employees/WebUserControls/QueryEmployeeScheduleRules.ascx" TagPrefix="roForms" TagName="QueryEmployeeScheduleRules" %>
<%@ Register Src="~/Employees/WebUserControls/ContractScheduleRules.ascx" TagName="frmContractScheduleRules" TagPrefix="roForms" %>

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

    <dx:aspxhiddenfield id="hdnEmployeeDocumentsConfig" runat="server" clientinstancename="hdnEmployeeDocumentsConfigClient"></dx:aspxhiddenfield>

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
                    <rws:rotreesselector id="roTrees1" runat="server" filterfloat="true" prefixtree="roTrees1"
                        tree1visible="true" tree1multisel="false" tree1showonlygroups="false" tree1function="cargaNodo"
                        tree2visible="true" tree2multisel="false" tree2showonlygroups="false" tree2function="cargaNodo"
                        tree3visible="true" tree3multisel="false" tree3showonlygroups="false" tree3function="cargaNodo"
                        featurealias="Employees" featuretype="U" showtreecaption="true">
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

                                    <div id="employeeRow" runat="server" class="contentPanel" style="display: none">

                                        <!-- PANELL EMPLEAT GENERAL -->
                                        <div id="panEmpGeneral" class="contentPanel" style="display: none" runat="server">
                                            <!-- Este div es el header -->
                                            <div class="divSummary">
                                                <div class="panBottomMargin">
                                                    <div class="panBottomMargin" style="display: flex;">
                                                        <div style="width: 100%;" class="panHeader2 panBottomMargin">
                                                            <span class="panelTitleSpan">
                                                                <asp:Label runat="server" ID="lblTypeEmployee" Text="Perfil"></asp:Label>
                                                            </span>
                                                        </div>
                                                        <div id="divProfileClick" class="panHeader2" style="padding: 0px; cursor: pointer; margin-left: 0.5vw !important; padding-left: 5px; padding-right: 5px;">
                                                            <div style="position: relative; margin-left: 4px; margin-right: 4px;">
                                                                <asp:Label runat="server" Text=" v "></asp:Label>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <!-- La descripción es opcional -->
                                                </div>
                                                <div id="collapsableProfile">
                                                    <!-- Este div es un formulario -->
                                                    <div class="panBottomMargin">
                                                        <div class="panelHeaderContent">
                                                            <div class="panelDescriptionImage">
                                                                <div id="divTypeEmp" runat="server" style="height: 48px;"></div>
                                                            </div>
                                                            <div class="panelDescriptionText">
                                                                <asp:Label ID="lblTypeEmployeeDesc" runat="server" Text="Datos generales del empleado."></asp:Label>
                                                            </div>
                                                        </div>
                                                        <div class="divRow">
                                                            <div class="divRowDescription">
                                                                <asp:Label ID="lblNameDescription" runat="server" Text="Nombre del empleado utilizado en VisualTime Live"></asp:Label>
                                                            </div>
                                                            <asp:Label ID="lblName" runat="server" Text="Nombre:" CssClass="labelForm"></asp:Label>
                                                            <div class="componentForm">
                                                                <dx:aspxtextbox id="txtName" runat="server" width="350px" clientinstancename="txtEmpName_Client" nulltext="_____">
                                                                    <clientsideevents validation="LengthValidation" textchanged="function(s,e){checkEmployeeEmptyName(s.GetValue());}" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                                    <validationsettings setfocusonerror="True" validationgroup="employeenameGroup">
                                                                        <requiredfield isrequired="True" errortext="(*)" />
                                                                    </validationsettings>
                                                                </dx:aspxtextbox>
                                                            </div>
                                                        </div>
                                                        <div class="divRow">
                                                            <div class="divRowDescription">
                                                                <asp:Label ID="lblLanguageDescription" runat="server" Text="Idioma por defecto del empleado dentro de la plataforma"></asp:Label>
                                                            </div>
                                                            <asp:Label ID="lblLanguage" runat="server" Text="Idioma:" CssClass="labelForm"></asp:Label>
                                                            <div class="componentForm">
                                                                <dx:aspxcombobox id="cmbLanguage" runat="server" width="170px" clientinstancename="cmbLanguage_Client">
                                                                    <clientsideevents valuechanged="function(s,e){ checkEmployeeEmptyName(s.GetValue()); }" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                                </dx:aspxcombobox>
                                                            </div>
                                                        </div>

                                                        <br />

                                                        <div class="divRow" runat="server" id="divEmployeeProductiv">
                                                            <div class="divRowDescription">
                                                                <asp:Label ID="lblsProductiveDesc" runat="server" Text="Solo con licencia ProductiV"></asp:Label>
                                                            </div>
                                                            <asp:Label ID="lblIsProductive" runat="server" Text="¿Debe imputar horas?" CssClass="labelForm"></asp:Label>
                                                            <div class="componentForm" style="margin-top: -3px; margin-bottom: 5px;">

                                                                <div style="clear: both; padding-bottom: 10px;">
                                                                    <div style="float: left; padding-right: 20px">
                                                                        <dx:aspxradiobutton groupname="IsProductiv" id="ckProductiveYes" runat="server" clientinstancename="ckProductiveYes_client" text="Sí">
                                                                            <clientsideevents valuechanged="function(s,e){ checkEmployeeEmptyName(s.GetValue()); }" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                                        </dx:aspxradiobutton>
                                                                    </div>
                                                                    <div style="float: left; padding-right: 20px;">
                                                                        <dx:aspxradiobutton groupname="IsProductiv" id="ckProductiveNo" runat="server" checked="true" clientinstancename="ckProductiveNo_client" text="No">
                                                                            <clientsideevents valuechanged="function(s,e){ checkEmployeeEmptyName(s.GetValue()); }" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                                        </dx:aspxradiobutton>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <br />
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <br />
                                            <div id="divLastPunch" runat="server" class="divSummary">
                                                <div class="panBottomMargin" style="display: flex;">
                                                    <div style="width: 100%;" class="panHeader2 panBottomMargin">
                                                        <span class="panelTitleSpan">
                                                            <asp:Label runat="server" ID="lblLastPunchSummary" Text="Ultimo Fichaje"></asp:Label>
                                                        </span>
                                                    </div>
                                                    <div id="divPunchClick" class="panHeader2" style="padding: 0px; cursor: pointer; margin-left: 0.5vw !important; padding-left: 5px; padding-right: 5px;">
                                                        <div style="position: relative; margin-left: 4px; margin-right: 4px;">
                                                            <asp:Label runat="server" Text=" v "></asp:Label>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div id="collapsablePunch">
                                                    <div style="width: 90%; margin-left: 25px; min-height: 65px;">
                                                        <div id="divSummaryPunch" runat="server">
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <br />
                                            <div id="divPresenceSummary" runat="server" class="divSummary">
                                                <div class="panBottomMargin" style="display: flex;">
                                                    <div style="width: 100%;" class="panHeader2 panBottomMargin">
                                                        <span class="panelTitleSpan">
                                                            <asp:Label runat="server" ID="lblPresenceSummary" Text="Planificación"></asp:Label>
                                                        </span>
                                                    </div>
                                                    <div id="divPlanClick" class="panHeader2" style="padding: 0px; cursor: pointer; margin-left: 0.5vw !important; padding-left: 5px; padding-right: 5px;">
                                                        <div style="position: relative; margin-left: 4px; margin-right: 4px;">
                                                            <asp:Label runat="server" Text=" v "></asp:Label>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div id="collapsablePlan">
                                                    <div style="display: flex; flex-direction: column; padding: 1rem 0; gap: 1rem;">
                                                        <div style="margin-left: 45px; display: table-cell" id="divPlanification" runat="server">
                                                        </div>
                                                        <div style="margin-left: 45px; display: flex; gap: 3rem;" id="divPlanificationLinks" runat="server">
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <br />
                                            <dx:aspxcallbackpanel clientsideevents-begincallback="holidaysSummaryEndCallback" id="divHolidaysSummary" cssclass="divSummary" runat="server" visible="false" class="divSummary" clientinstancename="CallbackPanel">
                                                <panelcollection>
                                                    <dx:panelcontent runat="server">
                                                        <div class="panBottomMargin" style="display: flex;">
                                                            <div style="width: 100%;" class="panHeader2 panBottomMargin">
                                                                <span class="panelTitleSpan">
                                                                    <asp:Label runat="server" ID="lblHolidaysSummary" Text="Resumen de vacaciones"></asp:Label>
                                                                </span>
                                                            </div>
                                                            <div id="divHolidaysSummaryClick" class="panHeader2" style="padding: 0px; cursor: pointer; margin-left: 0.5vw !important; padding-left: 5px; padding-right: 5px;">
                                                                <div style="position: relative; margin-left: 4px; margin-right: 4px;">
                                                                    <asp:Label runat="server" Text=" v "></asp:Label>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div id="collapsableHolidaysSummary">
                                                            <asp:Label ID="lblHolidayShift" runat="server" Text="Horario:" CssClass="labelForm"></asp:Label>
                                                            <div class="componentForm" style="margin-bottom: 20px;">
                                                                <div style="width: 100%; line-height: 0px;">&nbsp;</div>
                                                                <div style="float: left;">
                                                                    <dx:aspxcombobox runat="server" id="cmbHolidayShifts" width="300px" helptextsettings-popupmargins-margintop="5px" clientinstancename="cmbHolidayConceptsClient">
                                                                        <clientsideevents gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" valuechanged="function(s, e) { cargaEmpleado(actualEmployee); }" />
                                                                    </dx:aspxcombobox>
                                                                </div>
                                                            </div>
                                                            <div style="float: left; width: 100%">

                                                                <div style="float: left; width: 100%; margin-bottom:20px; margin-left: 130px; margin-right: 4%;">
                                                                    <dx:aspxgridview id="GridHolidaysSummary" visible="false" runat="server" autogeneratecolumns="False" keyboardsupport="False">
                                                                        <settings showtitlepanel="False" showcolumnheaders="true" verticalscrollbarmode="Hidden" usefixedtablelayout="True" />
                                                                        <settingspager mode="ShowAllRecords" showemptydatarows="false" />
                                                                        <settings showfilterbar="Hidden" />
                                                                        <styles>
                                                                            <header cssclass="jsGridHeaderCell" horizontalalign="Center" />
                                                                            <cell wrap="False" horizontalalign="Center" />                                                                            
                                                                        </styles>
                                                                    </dx:aspxgridview>
                                                                </div>
                                                                <div style="float: left; width: 70%; margin-bottom: 10px; margin-right: 10px; margin-left:130px;">
                                                                    <dx:aspxgridview id="GridHolidaysDetail" visible="false" runat="server" autogeneratecolumns="False" keyboardsupport="False">
                                                                        <settings showtitlepanel="False" showcolumnheaders="true" usefixedtablelayout="True" verticalscrollableheight="245" verticalscrollbarmode="Hidden" />
                                                                        <settingspager mode="ShowPager" pagesize="100" showemptydatarows="false" />
                                                                        <styles>
                                                                            <header cssclass="jsGridHeaderCellWithSeparator" horizontalalign="Center" />
                                                                            <cell wrap="False" horizontalalign="Center" />
                                                                        </styles>
                                                                    </dx:aspxgridview>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </dx:panelcontent>
                                                </panelcollection>
                                            </dx:aspxcallbackpanel>
                                            <br />
                                            <div id="divEmployeeSummary" style="float: left;" runat="server" class="divSummary">
                                                <div class="panBottomMargin" style="display: flex;">
                                                    <div style="width: 100%;" class="panHeader2 panBottomMargin">
                                                        <span class="panelTitleSpan">
                                                            <asp:Label runat="server" ID="lblEmployeeSummary" Text="Resumen / Gráficos"></asp:Label>
                                                        </span>
                                                    </div>
                                                    <div id="divSummaryClick" class="panHeader2" style="padding: 0px; cursor: pointer; margin-left: 0.5vw !important; padding-left: 5px; padding-right: 5px;">
                                                        <div style="position: relative; margin-left: 4px; margin-right: 4px;">
                                                            <asp:Label runat="server" Text=" v "></asp:Label>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div id="collapsableSummary">
                                                    <div style="width: 50%">
                                                        <div class="divRow">
                                                            <div class="divRowDescription" style="width: 100%; margin-bottom: 10px;">
                                                                <asp:Label ID="Label2lblEmployeeSummaryDesc" runat="server" Text="Puede consultar los distintos saldos, justificaciones, datos de productiv y centros de coste del empleado en el periodo seleccionado."></asp:Label>
                                                            </div>
                                                            <asp:Label ID="lblEmployeSummaryPeriod" runat="server" Text="Periodo:" CssClass="labelForm"></asp:Label>
                                                            <div class="componentForm">
                                                                <div style="width: 100%; line-height: 0px;">&nbsp;</div>
                                                                <div style="float: left;">
                                                                    <dx:aspxcombobox runat="server" id="cmbSummaryPeriod" width="300px" clientinstancename="cmbSummaryPeriodClient">
                                                                        <clientsideevents selectedindexchanged="function(s,e){ LoadEmployeeSummary(cmbSummaryPeriodClient.GetSelectedItem().value);}" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                                    </dx:aspxcombobox>
                                                                </div>
                                                                <div class="btnFlat" id="divGoToGenius" runat="server" style="float: left; margin-left: 20px; margin-top: 10px;">
                                                                    <a href="javascript: void(0)" id="btnGoToGenius" runat="server" onclick="">
                                                                        <span class="btnIconGenius"></span>
                                                                        <asp:Label ID="goToGenius" runat="server" Text="Otros análisis"></asp:Label>
                                                                    </a>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div id="noDataRow" runat="server" class="divRow" style="display: none">
                                                        <div class="panBottomMargin">
                                                            <br />
                                                            <br />
                                                            <div class="panHeader3 panBottomMargin">
                                                                <span class="panelTitleSpan">
                                                                    <asp:Label ID="lblSummaryNoData" Text="No hay datos en el periodo seleccionado o no dispone de permisos para consultarlos." runat="server" />
                                                                </span>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="divRow">
                                                        <div id="divAccrualsSummary" runat="server" class="divSummary">
                                                            <div class="panBottomMargin">
                                                                <div class="panHeader3 panBottomMargin">
                                                                    <span class="panelTitleSpan">
                                                                        <asp:Label runat="server" ID="lblAccrualsSummary" Text="Saldos"></asp:Label>
                                                                    </span>
                                                                </div>
                                                            </div>
                                                            <div id="divAccualDraw" runat="server" class="accrualSumary" style="width: 96%">
                                                                <!-- Dibuja los saldos -->
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="divRow">
                                                        <div id="divCausesSummary" runat="server" class="divSummary">
                                                            <div class="panBottomMargin">
                                                                <div class="panHeader3 panBottomMargin">
                                                                    <span class="panelTitleSpan">
                                                                        <asp:Label runat="server" ID="lblCausesSummary" Text="Justificaciones"></asp:Label>
                                                                    </span>
                                                                </div>
                                                            </div>

                                                            <div id="divCausesDraw" runat="server" class="accrualSumary" style="width: 96%">
                                                                <!-- Dibuja los saldos -->
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="divRow">
                                                        <div id="divTasksSummary" runat="server" style="margin-bottom: 35px; width: 100%">
                                                            <div class="panBottomMargin">
                                                                <div class="panHeader3 panBottomMargin">
                                                                    <span class="panelTitleSpan">
                                                                        <asp:Label runat="server" ID="lblTasksSummary" Text="ProductiV"></asp:Label>
                                                                    </span>
                                                                </div>
                                                            </div>

                                                            <div id="divCanvas" style="height: 400px; margin-left: 45px;" runat="server">
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="divRow">
                                                        <div id="divBussinessCentersSummary" runat="server" style="margin-bottom: 35px; width: 100%">
                                                            <div class="panBottomMargin">
                                                                <div class="panHeader3 panBottomMargin">
                                                                    <span class="panelTitleSpan">
                                                                        <asp:Label runat="server" ID="lblBussinessCentersSummary" Text="Centros de Coste"></asp:Label>
                                                                    </span>
                                                                </div>
                                                            </div>
                                                            <div class="contenedorSummaryCenters" style="margin: 0 auto; width: 100%;" id="divCentersCanvas" runat="server">
                                                                <div class="summaryCentersRow">
                                                                    <div class="summaryCenterCell" style="vertical-align: top;">
                                                                        <div id="centersSummary"></div>
                                                                    </div>
                                                                    <div class="summaryCenterCell">
                                                                        <div id="presenceSummary"></div>
                                                                        <div id="absencesSummary"></div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <!-- PANEL CAMPOS DE LA FICHA EMPLEADOS-->
                                        <div id="panEmpUserFields" class="contentPanel" style="display: none" runat="server">

                                            <div id="highlightDiv" runat="server" style="display: none">
                                                <div class="panHeader2">
                                                    <span style="">
                                                        <asp:Label runat="server" ID="lblHighlightDiv" Text="Color de resalte del empleado"></asp:Label></span>
                                                </div>
                                                <br />
                                                <table border="0" style="margin: 0px; width: 100%; width: 75%; margin-right: auto; margin-left: 20px">
                                                    <tr>
                                                        <td width="150px" align="right" style="padding-right: 5px;">
                                                            <asp:Label ID="lblColorDesc" runat="server" Text="Color identificativo:" class="spanEmp-Class"></asp:Label>
                                                        </td>
                                                        <td>
                                                            <dx:aspxcoloredit id="dxColorPicker" runat="server" clientinstancename="dxColorPickerClient" enablecustomcolors="true" width="14px">
                                                                <clientsideevents colorchanged="enableSaveHighlight" />
                                                            </dx:aspxcoloredit>
                                                        </td>
                                                        <td align="right">
                                                            <div class="btnFlat">
                                                                <a href="javascript: void(0)" id="btSaveHighlight" runat="server" onclick="saveHighlight()">
                                                                    <span class="btnIconAdd"></span>
                                                                    <asp:Label ID="lblSaveHighlight" runat="server" Text="Guardar"></asp:Label>
                                                                </a>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>
                                            <div id="divEmployeeUserFields" style="margin-bottom: 14px;" runat="server">
                                                <!-- Este div es el header -->

                                                <div class="panBottomMargin" style="display: flex;">
                                                    <div style="width: 100%;" class="panHeader2 panBottomMargin">
                                                        <span class="panelTitleSpan">
                                                            <asp:Label runat="server" ID="lblFieldsEmployeeHeader" Text="Ficha de empleado"></asp:Label>
                                                        </span>
                                                    </div>
                                                    <div id="divUFClick" class="panHeader2" style="padding: 0px; cursor: pointer; margin-left: 0.5vw !important; padding-left: 5px; padding-right: 5px;">
                                                        <div style="position: relative; margin-left: 4px; margin-right: 4px;">
                                                            <asp:Label runat="server" ID="lblFieldsEmployeeHeader2" Text=" v "></asp:Label>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div id="collapsableUserFields" style="margin-right: 25px; margin-left: 25px;">

                                                    <div class="divEmployee-Class">
                                                        <table border="0" width="100%" height="50px">
                                                            <tr>
                                                                <td width="48px" height="48px">
                                                                    <img id="Img18" src="~/Base/Images/StartMenuIcos/UserFields.png" style="border: 0;" runat="server" /></td>
                                                                <td valign="top" align="left">
                                                                    <span id="suserFieldDesc" runat="server" class="spanEmp-Class">
                                                                        <asp:Label ID="userFieldDesc" runat="server" Text="En esta sección puedes consultar los campos del perfil del empleado"></asp:Label>
                                                                    </span>
                                                                </td>
                                                                <td>
                                                                    <div class="">
                                                                        <asp:Label ID="lblFieldsEmployee" runat="server" CssClass="jsGridTitle" Text="Ficha de empleado"></asp:Label>
                                                                        <div id="btn1Fields" runat="server" class="jsgridButton" style="margin-top: 0px">
                                                                            <div class="btnFlat" style="height: 15px;">
                                                                                <a href="javascript: void(0)" id="editGridEmp" runat="server" onclick="" style="line-height: 1px;">
                                                                                    <span class="btnIconEdit"></span>
                                                                                    <asp:Label ID="lblEdit" runat="server" Text="Editar"></asp:Label>
                                                                                </a>
                                                                            </div>
                                                                        </div>
                                                                        <div id="btn3Fields" runat="server" class="jsgridButton" style="margin-top: 0px" visible="false">
                                                                            <div class="btnFlat" style="height: 15px;">
                                                                                <a href="javascript: void(0)" id="cancelEditGridEmp" runat="server" onclick="" style="line-height: 1px;">
                                                                                    <span class="btnIconCancel"></span>
                                                                                    <asp:Label ID="lblCancel" runat="server" Text="Cancelar"></asp:Label>
                                                                                </a>
                                                                            </div>
                                                                        </div>
                                                                        <div id="btn2Fields" runat="server" class="jsgridButton" style="margin-top: 0px" visible="false">
                                                                            <div class="btnFlat" style="height: 15px;">
                                                                                <a href="javascript: void(0)" id="saveEditGridEmp" runat="server" onclick="" style="line-height: 1px;">
                                                                                    <span class="btnIconSave"></span>
                                                                                    <asp:Label ID="lblSave" runat="server" Text="Guardar"></asp:Label>
                                                                                </a>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </div>

                                                    <input type="hidden" id="hdnEmployeeFieldsIDs" runat="server" value="" />
                                                    <input type="hidden" id="hdnDaysChanged" runat="server" value="" />
                                                    <input type="hidden" id="hdnSelectedGroup" runat="server" value="" />
                                                    <div id="divGrid" runat="server" class="jsGridContent" style="max-height:750px;overflow:auto">
                                                        <!-- Carrega del Grid Usuari General -->
                                                    </div>
                                                </div>
                                            </div>
                                            <br />
                                            <div id="divOnboarding" runat="server" style="clear: both;" visible="false">
                                                <div class="panBottomMargin" style="display: flex;">
                                                    <div style="width: 100%;" class="panHeader2 panBottomMargin">
                                                        <span style="">
                                                            <asp:Label ID="lblOnboarding" runat="server" Text="OnBoarding"></asp:Label></span>
                                                    </div>
                                                    <div id="divOnboardingClick" class="panHeader2" style="padding: 0px; cursor: pointer; margin-left: 0.5vw !important; padding-left: 5px; padding-right: 5px;">
                                                        <div style="position: relative; margin-left: 4px; margin-right: 4px;">
                                                            <asp:Label runat="server" Text=" v "></asp:Label>
                                                        </div>
                                                    </div>
                                                </div>
                                                <br />
                                                <div id="collapsableOnboarding" style="display:none">
                                                    <div class="divEmployee-Class">
                                                        <table border="0" width="100%" height="50px">
                                                            <tr>
                                                                <td width="48px" height="48px">
                                                                    <img id="Img19" src="~/Base/Images/StartMenuIcos/OnBoarding96.png" style="border: 0;" runat="server" width="48" height="48" /></td>
                                                                <td valign="top" align="left">
                                                                    <span id="span5" runat="server" class="spanEmp-Class">
                                                                        <asp:Label ID="lblOnboardingDesc" runat="server" Text="En esta sección puede consultar el onboarding del usuario."></asp:Label>
                                                                    </span>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </div>
                                                    <br />
                                                    <div style="margin-left: 25px; margin-right: 25px;">
                                                        <div id="divOnboardingGrid" runat="server" class="jsGridContent dextremeGrid">
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <br />
                                            <div id="divDelivered" runat="server" style="clear: both;">
                                                <div class="panBottomMargin" style="display: flex;">
                                                    <div style="width: 100%;" class="panHeader2 panBottomMargin">
                                                        <span style="">
                                                            <asp:Label ID="lblDeliveredDocs" runat="server" Text="Documentos de empleado"></asp:Label></span>
                                                    </div>
                                                    <div id="divDocClick" class="panHeader2" style="padding: 0px; cursor: pointer; margin-left: 0.5vw !important; padding-left: 5px; padding-right: 5px;">
                                                        <div style="position: relative; margin-left: 4px; margin-right: 4px;">
                                                            <asp:Label runat="server" Text=" v "></asp:Label>
                                                        </div>
                                                    </div>
                                                </div>
                                                <br />
                                                <div id="collapsableDocuments" style="display:none">
                                                    <div class="divEmployee-Class">
                                                        <table border="0" width="100%" height="50px">
                                                            <tr>
                                                                <td width="48px" height="48px">
                                                                    <img id="Img13" src="~/Base/Images/Documents/Documents.png" style="border: 0;" runat="server" /></td>
                                                                <td valign="top" align="left">
                                                                    <span id="span7" runat="server" class="spanEmp-Class">
                                                                        <asp:Label ID="lblDeliveredDocsDesc" runat="server" Text="En esta sección puede consultar los documentos entregados por el empleado."></asp:Label>
                                                                    </span>
                                                                </td>
                                                                <td valign="top" align="right">
                                                                    <%--<dx:ASPxButton ID="btnOpenPopupAlertDocuments" runat="server" AutoPostBack="False" CausesValidation="False" Text="" ToolTip="Ver alertas">
                                                                        <Image Url="~/Base/Images/PortalAlerts/ico_Task_With_ALerts.png" Width="48"></Image>
                                                                        <ClientSideEvents Click="btnOpenPopupAlertDocumentsClient_Click" />
                                                                    </dx:ASPxButton>--%>
                                                                    <div id="imgAlerts" runat="server" style="background: #0046FE; border-radius: 50%; width: 53px; height: 53px; text-align: center; cursor: pointer;">
                                                                        <dx:aspximage id="btnOpenPopupAlertDocuments" runat="server" imageurl="~/Base/Images/PortalAlerts/ico_Task_With_ALerts.png" width="48">
                                                                            <clientsideevents click="btnOpenPopupAlertDocumentsClient_Click" />
                                                                        </dx:aspximage>
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </div>
                                                    <br />
                                                    <!-- Este div es un formulario -->
                                                    <div class="panBottomMargin" style="margin-right: 25px; margin-left: 25px;">
                                                        <roforms:documentmanagment runat="server" id="DocumentManagment" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <!-- PANELL CONTRATOS -->
                                        <div id="panEmpContratos" class="contentPanel" style="display: none" runat="server">

                                            <div id="divContracts" runat="server" style="margin-bottom: 35px;">
                                                <div class="panBottomMargin" style="display: flex;">

                                                    <div style="width: 100%;" class="panHeader2 panBottomMargin">
                                                        <span class="panelTitleSpan">
                                                            <asp:Label runat="server" ID="lblContractsTitle" Text="Contratos"></asp:Label>
                                                        </span>
                                                    </div>
                                                    <div id="divContractsClick" class="panHeader2" style="padding: 0px; cursor: pointer; margin-left: 0.5vw !important; padding-left: 5px; padding-right: 5px;">
                                                        <div style="position: relative; margin-left: 4px; margin-right: 4px;">
                                                            <asp:Label runat="server" Text=" v "></asp:Label>
                                                        </div>
                                                    </div>
                                                </div>
                                                <br />
                                                <div id="collapsibleContracts" class="divEmployee-Class" style="position: relative">
                                                    <div style="position: absolute">
                                                        <div style="float: left; width: 60px; height: 48px">
                                                            <img id="Img3" src="~/Base/Images/Employees/Contratos.png" style="border: 0;" runat="server" alt="" />
                                                        </div>
                                                        <div style="float: left; height: 48px;">
                                                            <span id="spanContracts" runat="server" class="spanEmp-Class" style="line-height: 42px;"></span>
                                                        </div>
                                                    </div>

                                                    <div>
                                                        <div class="jsGridContent">
                                                            <dx:aspxgridview id="GridContracts" runat="server" autogeneratecolumns="False" clientinstancename="GridContractsClient" keyboardsupport="True" width="100%" clientsideevents-begincallback="GridContracts_BeginCallback" theme="Robo">
                                                                <toolbars>
                                                                    <dx:gridviewtoolbar>
                                                                        <items>
                                                                            <dx:gridviewtoolbaritem name="New" command="New" alignment="Right" />
                                                                        </items>
                                                                    </dx:gridviewtoolbar>
                                                                </toolbars>

                                                                <settings showtitlepanel="False" verticalscrollbarmode="Auto" usefixedtablelayout="True" verticalscrollableheight="200" />
                                                                <settingsbehavior allowfocusedrow="false" />
                                                                <clientsideevents custombuttonclick="GridContracts_CustomButtonClick" endcallback="GridContracts_EndCallback" rowdblclick="GridContracts_OnRowDblClick" focusedrowchanged="GridContracts_FocusedRowChanged" />
                                                                <settingscommandbutton>
                                                                    <deletebutton image-url="~/Base/Images/Grid/remove.png" image-tooltip="" />
                                                                    <updatebutton image-url="~/Base/Images/Grid/save.png" image-tooltip="" />
                                                                    <cancelbutton image-url="~/Base/Images/Grid/cancel.png" image-tooltip="" />
                                                                    <editbutton image-url="~/Base/Images/Grid/edit.png" image-tooltip="" />
                                                                </settingscommandbutton>
                                                                <styles>
                                                                    <cell wrap="False" />
                                                                </styles>
                                                                <settingspager mode="ShowAllRecords" showemptydatarows="false">
                                                                </settingspager>
                                                            </dx:aspxgridview>
                                                            <roforms:frmcontractschedulerules id="frmContractScheduleRules" runat="server" />
                                                        </div>
                                                    </div>
                                                    <div class="divRow">
                                                        <span id="span10" runat="server" class="spanEmp-Class" style="line-height: 42px;"></span>
                                                        <asp:Label ID="lblForgottenRight" runat="server" Text="¿Eliminar los fichajes del usuario anteriores a cuatro años cuando no tenga contrato en vigor?"></asp:Label>
                                                        <div class="componentForm" style="margin-top: -3px; margin-bottom: 5px;">

                                                            <div style="clear: both; padding-bottom: 10px;">
                                                                <div style="float: left; padding-right: 20px">
                                                                    <dx:aspxradiobutton groupname="grpForgottenRight" id="btnForgottenRightTrue" runat="server" clientinstancename="ckForgottenRightYes_client" text="Sí">
                                                                        <clientsideevents valuechanged="function(s,e){ chkForgottenRightValueChanged(); }" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                                    </dx:aspxradiobutton>
                                                                </div>
                                                                <div style="float: left; padding-right: 20px;">
                                                                    <dx:aspxradiobutton groupname="grpForgottenRight" id="btnForgottenRightFalse" runat="server" checked="true" clientinstancename="ckForgottenRightNo_client" text="No">
                                                                        <clientsideevents valuechanged="function(s,e){ chkForgottenRightValueChanged(); }" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                                    </dx:aspxradiobutton>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div id="divLockDate" runat="server" class="divSummary">
                                                <div class="panBottomMargin">
                                                    <div class="panHeader2 panBottomMargin">
                                                        <span class="panelTitleSpan">
                                                            <asp:Label runat="server" ID="lblLockDateSummary" Text="Fecha de bloqueo"></asp:Label>
                                                        </span>
                                                    </div>
                                                </div>
                                                <div class="divEmployee-Class">
                                                    <table border="0" width="100%" height="50px">
                                                        <tr>
                                                            <td width="60px" height="48px">
                                                                <a href="javascript:void(0)" id="a7" runat="server" onclick="">
                                                                    <img id="Img16" src="~/Base/Images/StartMenuIcos/LockDB.png" style="border: 0;" runat="server" />
                                                                </a>
                                                            </td>
                                                            <td valign="middle" align="left">
                                                                <div id="divLockDateDetail" runat="server" class="divRow">
                                                                    <div class="divRowDescription" style="padding-left: 0px!important">
                                                                        <asp:Label ID="lblCLDDesc" runat="server" Text="Blablabla blablabla blablabla blalblabla"></asp:Label>
                                                                    </div>
                                                                    <asp:Label ID="lblLockDate" runat="server" Text="La fecha de bloqueo para este empleado es: " Style="margin-top: 15px; float: left;"></asp:Label>
                                                                    <div style="float: left; margin-top: 5px;">
                                                                        <div style="float: left; padding-left: 10px; padding-top: 5px;">
                                                                            <dx:aspxtextbox id="txtCurrentLockDate" runat="server" enabled="false" width="100px" clientinstancename="txtCurrentLockDate_Client">
                                                                            </dx:aspxtextbox>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="left" colspan="2" style="padding-left: 55px;">
                                                                <div style="float: left; padding-left: 25px;">
                                                                    <div class="btnFlat">
                                                                        <a href="javascript: void(0)" id="btnEditLockDate" runat="server" onclick="">
                                                                            <span class="btnIconEdit"></span>
                                                                            <asp:Label ID="lblEditLockDate" runat="server" Text="Editar"></asp:Label>
                                                                        </a>
                                                                    </div>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                            </div>

                                            <div id="divMobility" runat="server" style="margin-bottom: 35px;">
                                                <div class="panHeader2">
                                                    <span style="">
                                                        <asp:Label ID="lblMobilityTitle" runat="server" Text="Movilidad"></asp:Label></span>
                                                </div>
                                                <br />
                                                <div class="divEmployee-Class">
                                                    <table border="0" width="100%" height="50px">
                                                        <tr>
                                                            <td width="60px" height="48px">
                                                                <a href="javascript:void(0)" id="aMobility" runat="server" onclick="">
                                                                    <img id="Img4" src="~/Base/Images/Employees/Mobilidad.png" style="border: 0;" runat="server" />
                                                                </a>
                                                            </td>
                                                            <td valign="middle" align="left">
                                                                <a href="javascript:void(0)" onclick="document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_aMobility').onclick();">
                                                                    <span id="spanMobility" runat="server" class="spanEmp-Class" style="cursor: pointer;"></span>
                                                                </a>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="left" colspan="2" style="padding-left: 55px;">
                                                                <div class="btnFlatWithMaxTextWidth">
                                                                    <a href="javascript: void(0)" id="A1" runat="server" onclick="document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_aMobility').onclick();">
                                                                        <span class="btnIconEdit"></span>
                                                                        <asp:Label ID="aHistoryMobility" runat="server" Text="Ver histórico movilidad"></asp:Label>
                                                                    </a>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                            </div>

                                            <div id="divCost" runat="server" style="margin-bottom: 35px;">
                                                <div class="panHeader2">
                                                    <span style="">
                                                        <asp:Label ID="lblCostTitle" runat="server" Text="Costes"></asp:Label></span>
                                                </div>
                                                <br />
                                                <div class="divEmployee-Class">
                                                    <table border="0" width="100%" height="50px">
                                                        <tr>
                                                            <td width="60px" height="48px">
                                                                <a href="javascript:void(0)" id="aCost" runat="server" onclick="">
                                                                    <img id="ImgCostEmployee" src="~/Tasks/Images/BusinessCenters48.png" style="border: 0;" runat="server" />
                                                                </a>
                                                            </td>
                                                            <td valign="middle" align="left">
                                                                <a href="javascript:void(0)" onclick="document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_aCost').onclick();">
                                                                    <span id="spanCost" runat="server" class="spanEmp-Class" style="cursor: pointer;">
                                                                        <asp:Label ID="lblCostTitle2" runat="server" Text="Configurar los centros de coste sobre los que el empleado puede fichar una cesión"></asp:Label>
                                                                    </span>
                                                                </a>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="left" colspan="2" style="padding-left: 55px;">
                                                                <div class="btnFlatWithMaxTextWidth">
                                                                    <a href="javascript: void(0)" id="A6" runat="server" onclick="document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_aCost').onclick();">
                                                                        <span class="btnIconEdit"></span>
                                                                        <asp:Label ID="aCostTitle" runat="server" Text="Configurar cesiones"></asp:Label>
                                                                    </a>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                            </div>

                                            <div id="divMessages" runat="server" style="margin-bottom: 35px;">
                                                <div class="panHeader2">
                                                    <span style="">
                                                        <asp:Label ID="lblMessagesHeader" runat="server" Text="Mensajes"></asp:Label></span>
                                                </div>
                                                <br />
                                                <div class="divEmployee-Class">
                                                    <table border="0" width="100%" height="50px">
                                                        <tr>
                                                            <td width="60px" height="48px">
                                                                <a href="javascript:void(0)" id="aMessage" runat="server" onclick="">
                                                                    <img id="Img12" src="~/Employees/Images/email_48.png" style="border: 0;" runat="server" />
                                                                </a>
                                                            </td>
                                                            <td valign="middle" align="left">
                                                                <a href="javascript:void(0)" onclick="document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_aMessage').onclick();">
                                                                    <span id="span6" runat="server" class="spanEmp-Class" style="cursor: pointer;">
                                                                        <asp:Label ID="lblMessagesDescription" runat="server" Text="Puede visualizar los mensajes del empleados configurados para mostrarse en los terminales"></asp:Label>
                                                                    </span>
                                                                </a>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="left" colspan="2" style="padding-left: 55px;">
                                                                <div class="btnFlatWithMaxTextWidth">
                                                                    <a href="javascript: void(0)" id="A9" runat="server" onclick="document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_aMessage').onclick();">
                                                                        <span class="btnIconEdit"></span>
                                                                        <asp:Label ID="lblMessagesBtn" runat="server" Text="Mensajes"></asp:Label>
                                                                    </a>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                            </div>

                                            <div id="divRemoveEmployeeData" runat="server" style="margin-bottom: 35px;">
                                                <div class="panHeader2">
                                                    <span style="">
                                                        <asp:Label ID="lblRemoveEmployeeMessagesTitle" runat="server" Text="Borrado de datos de empleado"></asp:Label></span>
                                                </div>
                                                <br />
                                                <div class="divEmployee-Class">
                                                    <table border="0" width="100%" height="50px">
                                                        <tr>
                                                            <td width="60px" height="48px">
                                                                <a href="javascript:void(0)" id="aRemoveData" runat="server" onclick="">
                                                                    <img id="Img17" src="~/Employees/Images/employee_removeData.png" style="border: 0;" runat="server" />
                                                                </a>
                                                            </td>
                                                            <td valign="middle" align="left">
                                                                <a href="javascript:void(0)" onclick="document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_aRemoveData').onclick();">
                                                                    <span id="span12" runat="server" class="spanEmp-Class" style="cursor: pointer;">
                                                                        <asp:Label ID="lblRemoveEmployeeMessagesDesc" runat="server" Text="Puede seleccionar que datos desea borrar del empleado. Por ejemplo: fotos de fichajes o huellas guardadas en el sistema"></asp:Label>
                                                                    </span>
                                                                </a>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="left" colspan="2" style="padding-left: 55px;">
                                                                <div class="btnFlatWithMaxTextWidth">
                                                                    <a href="javascript: void(0)" id="A11" runat="server" onclick="document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_aRemoveData').onclick();">
                                                                        <span class="btnIconEdit"></span>
                                                                        <asp:Label ID="lblRemoveDataBtn" runat="server" Text="Seleccionar datos a borrar"></asp:Label>
                                                                    </a>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                            </div>
                                        </div>

                                        <!-- PANELL FICHAJES -->
                                        <div id="panEmpFichajes" class="contentPanel" style="display: none" runat="server">

                                            <div id="divIdentify" runat="server" style="margin-bottom: 35px;">
                                                <div class="panHeader2">
                                                    <span style="">
                                                        <asp:Label ID="lblIdentifyMethodsTitle" runat="server" Text="Medios de Identificación"></asp:Label></span>
                                                </div>
                                                <br />
                                                <div class="divEmployee-Class">
                                                    <table border="0" width="100%" height="50px">
                                                        <tr>
                                                            <td width="60px" height="48px">
                                                                <a href="javascript:void(0)" id="aIdentifyMethods" runat="server" onclick="">
                                                                    <img id="Img5" src="~/Base/Images/Employees/MediosIdentificacion.png" style="border: 0;" runat="server" />
                                                                </a>
                                                            </td>
                                                            <td valign="middle" align="left">
                                                                <a href="javascript:void(0)" onclick="document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_aIdentifyMethods').onclick();">
                                                                    <span id="spanIdentify" runat="server" class="spanEmp-Class" style="cursor: pointer;"></span>
                                                                </a>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="left" colspan="2" style="padding-left: 55px;">
                                                                <div class="btnFlatWithMaxTextWidth">
                                                                    <a href="javascript: void(0)" id="A5" runat="server" onclick="document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_aIdentifyMethods').onclick();">
                                                                        <span class="btnIconEdit"></span>
                                                                        <asp:Label ID="aIdentifyMethodsTitle" runat="server" Text="Configurar Medios de Identificación"></asp:Label>
                                                                    </a>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                            </div>

                                            <div id="divEmployeePermissions" runat="server" style="margin-bottom: 35px;">
                                                <div class="panHeader2">
                                                    <span style="">
                                                        <asp:Label ID="lblEmployeePermissionsTitle" runat="server" Text="Permisos ${Employee}"></asp:Label></span>
                                                </div>
                                                <br />
                                                <div class="divEmployee-Class">
                                                    <table border="0" width="100%" height="50px">
                                                        <tr>
                                                            <td width="60px" height="48px">
                                                                <a href="javascript:void(0)" id="aEmployeePermissions" runat="server" onclick="">
                                                                    <img id="imgEmployeePermissions" src="~/Employees/Images/EmployeePermissions_48.PNG" style="border: 0;" runat="server" />
                                                                </a>
                                                            </td>
                                                            <td valign="middle" align="left">
                                                                <a href="javascript:void(0)" onclick="document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_aEmployeePermissions').onclick();">
                                                                    <span id="spanEmployeePermissions" runat="server" class="spanEmp-Class" style="cursor: pointer;">
                                                                        <asp:Label ID="lblEmployeePermissionsTitle2" runat="server" Text="Permisos ${Employee}"></asp:Label>
                                                                    </span>
                                                                </a>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="left" colspan="2" style="padding-left: 55px;">
                                                                <div class="btnFlatWithMaxTextWidth">
                                                                    <a href="javascript: void(0)" id="A4" runat="server" onclick="document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_aEmployeePermissions').onclick();">
                                                                        <span class="btnIconEdit"></span>
                                                                        <asp:Label ID="aEmployeePermissionsTitle" runat="server" Text="Configurar funcionalidades ${Employee}"></asp:Label>
                                                                    </a>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                            </div>

                                            <div id="divApplications" runat="server" style="margin-bottom: 35px;">
                                                <div class="panHeader2">
                                                    <span style="">
                                                        <asp:Label ID="lblApplicationsTitle" runat="server" Text="Aplicaciones permitidas"></asp:Label></span>
                                                </div>
                                                <br />
                                                <div class="divEmployee-Class">
                                                    <table border="0" width="100%" height="50px">
                                                        <tr>
                                                            <td width="60px" height="48px">
                                                                <a href="javascript:void(0)" id="aApplications" runat="server" onclick="">
                                                                    <img alt="" id="Img6" src="~/Base/Images/Employees/Mobilidad.png" style="border: 0;" runat="server" />
                                                                </a>
                                                            </td>
                                                            <td valign="middle" align="left">
                                                                <a href="javascript:void(0)" onclick="document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_aApplications').onclick();">
                                                                    <span id="spanApplications" runat="server" class="spanEmp-Class" style="cursor: pointer;">
                                                                        <asp:Label ID="lblApplicationsTitle2" runat="server" Text="Configurar aplicaciones permitidas"></asp:Label>
                                                                    </span>
                                                                </a>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="left" colspan="2" style="padding-left: 55px;">
                                                                <div class="btnFlatWithMaxTextWidth">
                                                                    <a href="javascript: void(0)" id="A3" runat="server" onclick="document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_aApplications').onclick();">
                                                                        <span class="btnIconEdit"></span>
                                                                        <asp:Label ID="aEmployeeApplicationsTitle" runat="server" Text="Configurar aplicaciones permitidas"></asp:Label>
                                                                    </a>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                            </div>

                                            <!-- PANEL Autorizaciones -->
                                            <div id="divAuthorizations" runat="server" style="margin-bottom: 35px;">
                                                <div class="panHeader2">
                                                    <span style="">
                                                        <asp:Label ID="lblAuthorizationsTitle" runat="server" Text="Accesos"></asp:Label></span>
                                                </div>
                                                <br />
                                                <div class="divEmployee-Class">
                                                    <table border="0" width="100%" height="50px">
                                                        <tr>
                                                            <td width="60px" height="48px">
                                                                <a href="javascript:void(0)" id="aAccessAuthorizations" runat="server" onclick="">
                                                                    <img alt="" id="Img10" src="~/Access/Images/AccessGroup.png" style="border: 0;" runat="server" />
                                                                </a>
                                                            </td>
                                                            <td valign="middle" align="left">
                                                                <a href="javascript:void(0)" onclick="document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_aAccessAuthorizations').onclick();">
                                                                    <span id="span1" runat="server" class="spanEmp-Class" style="cursor: pointer;">
                                                                        <asp:Label ID="lblAuthorizationsTitle2" runat="server" Text="Configurar todas los autorizaciones a grupos de acceso donde el empleado dispone de permiso"></asp:Label>
                                                                    </span>
                                                                </a>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="left" colspan="2" style="padding-left: 55px;">
                                                                <div class="btnFlatWithMaxTextWidth">
                                                                    <a href="javascript: void(0)" id="A8" runat="server" onclick="document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_aAccessAuthorizations').onclick();">
                                                                        <span class="btnIconEdit"></span>
                                                                        <asp:Label ID="lblAuthorizationsTitle3" runat="server" Text="Configurar autorizaciones"></asp:Label>
                                                                    </a>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                            </div>
                                        </div>

                                        <!-- PANELL AUSENCIAS PREVISTAS -->
                                        <div id="panEmpAusPrev" class="contentPanel" style="display: none" runat="server">

                                            <div class="divRow">
                                                <div class="splitDivLeft">
                                                    <div class="panHeader2">
                                                        <span style="">
                                                            <asp:Label ID="lblAusPrevs" runat="server" Text="Ausencias previstas"></asp:Label></span>
                                                    </div>
                                                    <br />
                                                    <div class="divEmployee-Class">
                                                        <table border="0" width="100%" height="50px">
                                                            <tr>
                                                                <td width="48px" height="48px">
                                                                    <img id="Img7" alt="" src="~/Base/Images/Employees/ProgrammedAbsences.png" style="border: 0;" runat="server" /></td>
                                                                <td valign="top" align="left">
                                                                    <span id="span3" runat="server" class="spanEmp-Class">
                                                                        <asp:Label ID="lblAuseProlong" runat="server" Text="Desde esta página puede gestionar las ausencias prolongadas y las incidencias previstas de este empleado.<br /> Igualmente, podrá realizar esta gestión desde la plantilla de Calendario."></asp:Label>
                                                                    </span>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </div>

                                                    <div style="margin-bottom: 5px; margin-left: 30px; width: 95%;">
                                                        <table border="0" width="100%" height="50px">
                                                            <tr>
                                                                <td align="right">
                                                                    <div id="tblAddAus" runat="server" class="btnFlat">
                                                                        <a href="javascript: void(0)" id="btnAddAbsence" runat="server" onclick="">
                                                                            <span class="btnIconEdit"></span>
                                                                            <asp:Label ID="lblAddAus" runat="server" Text="Añadir"></asp:Label>
                                                                        </a>
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <div id="gridAusenciasPrevistas" style="height: 250px; overflow: auto;" runat="server">
                                                                        <!-- Aqui va el grid de Ausencias Previstas -->
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </div>
                                                </div>
                                                <div class="splitDivRight">
                                                    <div class="panHeader2">
                                                        <span style="">
                                                            <asp:Label ID="lblIncPrevs" runat="server" Text="Incidencias previstas"></asp:Label></span>
                                                    </div>
                                                    <br />
                                                    <div class="divEmployee-Class">
                                                        <table border="0" width="100%" height="50px">
                                                            <tr>
                                                                <td width="48px" height="48px">
                                                                    <img alt="" id="Img8" src="~/Base/Images/Employees/ProgrammedCauses.png" style="border: 0;" runat="server" /></td>
                                                                <td valign="top" align="left">
                                                                    <span id="span144" runat="server" class="spanEmp-Class">
                                                                        <asp:Label ID="lblIncPrevistas" runat="server" Text="Desde esta página puede gestionar las incidencias previstas de este ${Employee}."></asp:Label>
                                                                    </span>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </div>

                                                    <div style="margin-bottom: 5px; margin-left: 30px; width: 95%;">
                                                        <table border="0" width="100%" height="50px">
                                                            <tr>
                                                                <td align="right">
                                                                    <div id="tblAddInc" runat="server" class="btnFlat">
                                                                        <a href="javascript: void(0)" id="btnAddIncidence" runat="server" onclick="">
                                                                            <span class="btnIconEdit"></span>
                                                                            <asp:Label ID="lblAddInc" runat="server" Text="Añadir"></asp:Label>
                                                                        </a>
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <div id="gridIncidenciasPrevistas" style="height: 250px; overflow: auto;" runat="server">
                                                                        <!-- Aqui va el grid de Incidencias Previstas -->
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="divRow">
                                                <div class="splitDivLeft">
                                                    <div class="panHeader2">
                                                        <span style="">
                                                            <asp:Label ID="lblProgrammedHolidaysTitle" runat="server" Text="Vacaciones previstas"></asp:Label></span>
                                                    </div>
                                                    <br />
                                                    <div class="divEmployee-Class">
                                                        <table border="0" width="100%" height="50px">
                                                            <tr>
                                                                <td width="48px" height="48px">
                                                                    <img alt="" id="Img14" src="~/Base/Images/Employees/ProgrammedHolidays.png" style="border: 0;" runat="server" /></td>
                                                                <td valign="top" align="left">
                                                                    <span id="span8" runat="server" class="spanEmp-Class">
                                                                        <asp:Label ID="lblProgrammedHolidaysDesc" runat="server" Text="Desde esta página puede gestionar las previsiones de vacaciones de este ${Employee}."></asp:Label>
                                                                    </span>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </div>

                                                    <div style="margin-bottom: 5px; margin-left: 30px; width: 95%;">
                                                        <table border="0" width="100%" height="50px">
                                                            <tr>
                                                                <td align="right">
                                                                    <div id="tblAddHolidays" runat="server" class="btnFlat">
                                                                        <a href="javascript: void(0)" id="btnAddHolidays" runat="server" onclick="">
                                                                            <span class="btnIconEdit"></span>
                                                                            <asp:Label ID="lblAddHolidays" runat="server" Text="Añadir"></asp:Label>
                                                                        </a>
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <div id="gridProgrammedHolidays" style="height: 250px; overflow: auto;" runat="server">
                                                                        <!-- Aqui va el grid de previsiones de vacaciones  -->
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </div>
                                                </div>
                                                <div class="splitDivRight">
                                                    <div class="panHeader2">
                                                        <span style="">
                                                            <asp:Label ID="lblProgrammedOvertimesTitle" runat="server" Text="Horas de exceso"></asp:Label></span>
                                                    </div>
                                                    <br />
                                                    <div class="divEmployee-Class">
                                                        <table border="0" width="100%" height="50px">
                                                            <tr>
                                                                <td width="48px" height="48px">
                                                                    <img alt="" id="Img15" src="~/Base/Images/Employees/ProgrammedOvertimes.png" style="border: 0;" runat="server" /></td>
                                                                <td valign="top" align="left">
                                                                    <span id="span9" runat="server" class="spanEmp-Class">
                                                                        <asp:Label ID="lblProgrammedOvertimesDesc" runat="server" Text="Desde esta página puede gestionar las previsiones de horas de exceso para este ${Employee}."></asp:Label>
                                                                    </span>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </div>

                                                    <div style="margin-bottom: 5px; margin-left: 30px; width: 95%;">
                                                        <table border="0" width="100%" height="50px">
                                                            <tr>
                                                                <td align="right">
                                                                    <div id="tblAddOvertimes" runat="server" class="btnFlat">
                                                                        <a href="javascript: void(0)" id="btnAddOvertime" runat="server" onclick="">
                                                                            <span class="btnIconEdit"></span>
                                                                            <asp:Label ID="lblAddOvertimes" runat="server" Text="Añadir"></asp:Label>
                                                                        </a>
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <div id="gridProgrammedOvertimes" style="height: 250px; overflow: auto;" runat="server">
                                                                        <!-- Aqui va el grid de horas de exceso  -->
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <!-- PANELL PUESTOS -->
                                        <div id="panEmpAssignments" class="contentPanel" style="display: none" runat="server">
                                            <div class="panHeader2">
                                                <span style="">
                                                    <asp:Label ID="lblAssignments" runat="server" Text="Puestos"></asp:Label></span>
                                            </div>
                                            <br />
                                            <div class="divEmployee-Class">
                                                <table border="0" width="100%" height="50px">
                                                    <tr>
                                                        <td width="48px" height="48px">
                                                            <img id="Img9" src="~/Base/Images/Assignments/Assignments.png" style="border: 0;" runat="server" /></td>
                                                        <td valign="top" align="left">
                                                            <span id="span4" runat="server" class="spanEmp-Class">
                                                                <asp:Label ID="lblDescAssignments" runat="server" Text="Desde esta página puede gestionar los puestos asignados a los empleados."></asp:Label>
                                                            </span>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>

                                            <div id="divAssignments" runat="server" style="margin-bottom: 5px; margin-left: 30px; width: 45%;">
                                                <div class="jsGrid">
                                                    <asp:Label ID="lblEmployeeAssignments" runat="server" CssClass="jsGridTitle" Text=""></asp:Label>
                                                    <div class="jsgridButton">
                                                        <dx:aspxbutton id="btnAddNewSuitability" runat="server" autopostback="False" causesvalidation="False" text="Añadir" tooltip="Añadir nueva Idoneidad" hoverstyle-cssclass="btnFlat-hover" cssclass="btnFlat">
                                                            <image url="~/Base/Images/Grid/add.png"></image>
                                                            <clientsideevents click="AddNewSuitability" />
                                                        </dx:aspxbutton>
                                                    </div>
                                                </div>
                                                <div class="jsGridContent">
                                                    <dx:aspxgridview id="gridSuitability" runat="server" autogeneratecolumns="False" clientinstancename="GridSuitabilityClient" keyboardsupport="True" width="100%">
                                                        <settings showtitlepanel="False" verticalscrollbarmode="Auto" usefixedtablelayout="True" verticalscrollableheight="150" />
                                                        <clientsideevents begincallback="gridSuitability_BeginCallback" />
                                                        <settingscommandbutton>
                                                            <deletebutton image-url="~/Base/Images/Grid/remove.png" image-tooltip="" />
                                                            <updatebutton image-url="~/Base/Images/Grid/save.png" image-tooltip="" />
                                                            <cancelbutton image-url="~/Base/Images/Grid/cancel.png" image-tooltip="" />
                                                            <editbutton image-url="~/Base/Images/Grid/edit.png" image-tooltip="" />
                                                        </settingscommandbutton>
                                                        <styles>
                                                            <alternatingrow enabled="True" backcolor="#d7e5ea"></alternatingrow>
                                                            <commandcolumn spacing="5px" />
                                                            <header cssclass="jsGridHeaderCell" />
                                                            <cell wrap="False" />
                                                        </styles>
                                                        <settingspager mode="ShowAllRecords" showemptydatarows="false">
                                                        </settingspager>
                                                    </dx:aspxgridview>
                                                </div>
                                            </div>
                                        </div>

                                        <!-- PANEL SUPERVISORES -->
                                        <div id="panActiveNotifications" class="contentPanel" style="display: none" runat="server">
                                            <table cellpadding="0" cellspacing="0" width="100%" height="100%" border="0">
                                                <tr>
                                                    <td valign="top" style="padding-top: 2px;">
                                                        <div class="panHeader2">
                                                            <span style="">
                                                                <asp:Label ID="lblSupervisorsTitle" runat="server" Text="Supervisores"></asp:Label></span>
                                                        </div>
                                                        <br />
                                                        <div class="divEmployee-Class">
                                                            <table border="0" width="95%">
                                                                <tr>
                                                                    <td width="48px" height="48px">
                                                                        <img src="Images/Passport_48.png" style="border: 0;" />
                                                                    </td>
                                                                    <td valign="top" align="left">
                                                                        <span class="spanEmp-Class">
                                                                            <asp:Label ID="lblActiveSupervisors" runat="server" Text="A continuación se indican los supervisores directos que tiene el empleado sobre cada funcionalidad"></asp:Label></span>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td colspan="2" style="padding: 10px 10px 10px 10px;">
                                                                        <div id="activeNotificationsTable" runat="server"></div>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </div>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </div>

                                    <div id="companyRow" runat="server" class="contentPanel" style="display: none">
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
                                                                <img id="Img11" src="~/Base/Images/Documents/Documents.png" style="border: 0;" runat="server" /></td>
                                                            <td valign="top" align="left">
                                                                <span id="span2" runat="server" class="spanEmp-Class">
                                                                    <asp:Label ID="lblGroupDeliveredDocsDesc" runat="server" Text="En esta sección puede consultar los documentos asignados al grupo."></asp:Label>
                                                                </span>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                                <br />

                                                <br />
                                                <!-- Este div es un formulario -->
                                                <div class="panBottomMargin" style="margin-right: 25px;">
                                                    <roforms:documentemployees runat="server" id="GroupDocumentManagment" />
                                                </div>
                                            </div>
                                        </div>

                                        <!-- PANEL DOCUMENTOS GRUPO COMPANY -->
                                        <div id="panGroupDocsCompany" class="contentPanel" style="display: none" runat="server">
                                            <div id="divGroupDeliveredCompany" runat="server" style="clear: both; margin-bottom: 35px;">
                                                <div class="panHeader2">
                                                    <span style="">
                                                        <asp:Label ID="lblGroupDeliveredDocsCompany" runat="server" Text="Documentos de empresa"></asp:Label></span>
                                                </div>
                                                <br />
                                                <div class="divEmployee-Class">
                                                    <table border="0" width="100%" height="50px">
                                                        <tr>
                                                            <td width="48px" height="48px">
                                                                <img id="Img11Company" src="~/Base/Images/Documents/Documents.png" style="border: 0;" runat="server" /></td>
                                                            <td valign="top" align="left">
                                                                <span id="span2Company" runat="server" class="spanEmp-Class">
                                                                    <asp:Label ID="lblGroupDeliveredDocsDescCompany" runat="server" Text="En esta sección puede consultar todos los documentos presentados por los empleados de la compañía."></asp:Label>
                                                                </span>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                                <br />

                                                <!-- Este div es un formulario -->
                                                <div class="panBottomMargin" style="margin-right: 25px;">
                                                    <roforms:documentcompany runat="server" id="GroupDocumentManagmentCompany" />
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
                                <asp:HiddenField ID="hdnEmployeesSelected" runat="server" Value="0" />
                                <asp:HiddenField ID="hdnEmployees" runat="server" Value="" />
                                <asp:HiddenField ID="hdnFilter" runat="server" Value="" />
                                <asp:HiddenField ID="hdnFilterUser" runat="server" Value="" />

                                <dx:aspxpopupcontrol id="documentAlerts" runat="server" allowdragging="True" closeaction="None" modal="True" clientinstancename="documentAlertsClient"
                                    popupverticalalign="WindowCenter" popuphorizontalalign="WindowCenter" height="530px" width="1050px"
                                    showheader="False" scrollbars="Auto" showpagescrollbarwhenmodal="True" popupanimationtype="None" backcolor="Transparent" contentstyle-paddings-padding="0px" border-bordercolor="Transparent" showshadow="false">
                                    <contentcollection>
                                        <dx:popupcontrolcontentcontrol id="PopupConceptsControlContent" runat="server">
                                            <div class="bodyPopupExtended" style="table-layout: fixed; height: 375px; width: 950px;">
                                                <div id="divPending" runat="server" style="margin-bottom: 35px;">
                                                    <div class="panHeader2">
                                                        <span style="">
                                                            <asp:Label ID="lblPendingDocs" runat="server" Text="Alertas de documentación"></asp:Label></span>
                                                    </div>
                                                    <br />
                                                    <div class="panBottomMargin" style="margin-right: 25px;">
                                                        <roforms:documentpendingmanagment runat="server" id="DocumentPendingManagment" />
                                                    </div>
                                                    <!-- BOTONES -->
                                                    <table style="float: right;">
                                                        <tr>
                                                            <td>
                                                                <dx:aspxbutton id="btnCancelUserFields" runat="server" autopostback="False" causesvalidation="False" text="${Button.Accept}" tooltip="${Button.Accept}" hoverstyle-cssclass="btnFlat-hover btnFlatBlack-hover" cssclass="btnFlat btnFlatBlack">
                                                                    <clientsideevents click="function(s, e) { documentAlertsClient.Hide(); }" />
                                                                </dx:aspxbutton>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                            </div>
                                        </dx:popupcontrolcontentcontrol>
                                    </contentcollection>

                                    <clientsideevents popup="documentAlertsClient_PopUp"></clientsideevents>
                                </dx:aspxpopupcontrol>
                            </dx:panelcontent>
                        </panelcollection>
                    </dx:aspxcallbackpanel>
                </div>
            </div>
        </div>

        <!-- POPUP NEW OBJECT -->
        <dx:aspxpopupcontrol id="CaptchaObjectPopup" runat="server" allowdragging="False" closeaction="None" modal="True" contenturl="~/Base/Popups/GenericCaptchaValidator.aspx"
            popupverticalalign="WindowCenter" popuphorizontalalign="WindowCenter" modalbackgroundstyle-opacity="0" width="500" height="300"
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

        <!-- POPUP CAPTCHA -->
        <dx:aspxpopupcontrol id="PopupCaptcha" runat="server" allowdragging="False" closeaction="None" modal="True" contenturl="~/Employees/DeleteEmployeeContractCaptcha.aspx"
            popupverticalalign="WindowCenter" popuphorizontalalign="WindowCenter" width="470px" height="320px"
            showheader="False" scrollbars="Auto" showpagescrollbarwhenmodal="True" clientinstancename="PopupCaptcha_Client" popupanimationtype="None" backcolor="Transparent" contentstyle-paddings-padding="0px" border-bordercolor="Transparent" showshadow="false">
        </dx:aspxpopupcontrol>
    </div>

    <script language="javascript" type="text/javascript">

        function resizeTreeEmployees() {
            try {
                var ctlPrefix = "<%= roTrees1.ClientID %>";
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
