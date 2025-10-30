<%@ Page Language="VB" MasterPageFile="~/Base/mastEmp.master" AutoEventWireup="false" Inherits="VTLive40.SaaSAdmin" Title="" CodeBehind="SaaSAdmin.aspx.vb" %>

<%@ Register Src="~/Security/WebUserControls/frmIPs.ascx" TagName="frmIPs" TagPrefix="roForms" %>

<asp:Content ID="cMainBody" ContentPlaceHolderID="contentMainBody" runat="Server">

    <script language="javascript" type="text/javascript">
        var passportLoaded = false;

        function PageBase_Load() {
            resizeFrames();

            if ($get('<%= SaaSAdmin_TabVisibleName.ClientID %>').value != '') {
                SelectTab($get('<%= SaaSAdmin_TabVisibleName.ClientID %>').value);
            }
            else {
                SelectTab('config');
            }
        }

        function SelectTab(SelectedTab) {

            // Hacer invisibles los panels
            $get('ctl00_contentMainBody_ASPxCallbackPanelContenido_saasConfig').style.display = 'none';
            $get('ctl00_contentMainBody_ASPxCallbackPanelContenido_templateAdmin').style.display = 'none';

            // Desmarcar los botones de la barra
            $get('<%= TABBUTTON_Common.ClientID %>').className = 'bTab';
            $get('<%= TABBUTTON_Templates.ClientID %>').className = 'bTab';

            var TabID;
            if (SelectedTab == 'config') {
                TabID = 'ctl00_contentMainBody_ASPxCallbackPanelContenido_saasConfig';
                $get('<%= TABBUTTON_Common.ClientID %>').className = 'bTab-active';

                $get(TabID).style.display = 'block';
            }
            else if (SelectedTab == 'template') {
                TabID = 'ctl00_contentMainBody_ASPxCallbackPanelContenido_templateAdmin';
                $get('<%= TABBUTTON_Templates.ClientID %>').className = 'bTab-active';
                loadExportTemplates();
            }

            $get('<%= SaaSAdmin_TabVisibleName.ClientID %>').value = TabID;

        }
    </script>

    <div id="divModalBgDisabled" class="modalBackground" style="position: absolute; left: 0px; top: 0px; z-index: 990; width: 1680px; height: 900px; display: none;"></div>

    <div id="divMainBody">
        <!-- TAB SUPERIOR -->
        <div id="divTabInfo" class="divDataCells">
            <div style="min-height: 10px"></div>
            <div id="divAccessGroup" class="blackRibbonTitle">
                <div class="blackRibbonIcon">
                    <img runat="server" id="SaaSLogo" src="~/Security/Images/VTSaaS.png" alt="" />
                </div>
                <div class="blackRibbonDescription">
                    <asp:Label ID="lblSaaSTitle" runat="server" Text="Administración de servicio SaaS" class="NameText"></asp:Label>
                </div>
                <div class="blackRibbonButtons">
                    <table border="0" cellpadding="0" cellspacing="0" width="100%" style="padding: 2px 5px 5px 5px;">
                        <tr>
                            <td id="rowTabButtons3" runat="server" align="right" valign="top" style="width: 140px; padding-top: 0px; padding-right: 5px;">
                                <a id="TABBUTTON_Common" href="javascript: void(0);" class="bTab bTab-active" onclick="SelectTab('config');" runat="server">
                                    <asp:Label ID="lblAdminSystem" Text="Sistema" runat="server" /></a>
                                <a id="TABBUTTON_Templates" href="javascript: void(0);" class="bTab" onclick="SelectTab('template');" runat="server">
                                    <asp:Label ID="lblTemplateManage" Text="Plantillas" runat="server" /></a>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
            <div style="min-height: 10px"></div>
        </div>
        <!-- FIN TAB SUPERIOR -->
        <!-- DETALLE -->
        <div id="divTabData" class="divDataCells" style="overflow-y: auto; overflow-x: hidden; position: relative;">
            <div id="divContenido" class="divAllContent">
                <div id="divContent" style="height: initial;" class="maxHeight">
                    <dx:ASPxCallback ID="CallbackExcel" runat="server" ClientInstanceName="CallbackExcelClient" ClientSideEvents-CallbackComplete="CallbackExcel_CallbackComplete"></dx:ASPxCallback>
                    <dx:ASPxCallbackPanel ID="ASPxCallbackPanelContenido" runat="server" Width="100%" ClientInstanceName="ASPxCallbackPanelContenidoClient">
                        <SettingsLoadingPanel Enabled="false" />
                        <ClientSideEvents EndCallback="ASPxCallbackPanelContenidoClient_EndCallBack" />
                        <PanelCollection>
                            <dx:PanelContent ID="PanelContent1" runat="server">
                                <div id="saasConfig" class="contentPanel" runat="server" name="menuPanel">
                                    <div style="padding: 15px">
                                        <div class="panHeader2">
                                            <span style="">
                                                <asp:Label runat="server" ID="lblServiceAdmin" Text="Administración del servicio"></asp:Label>
                                            </span>
                                        </div>
                                        <br />
                                        <div>
                                            <div style="display: inline-block; position: absolute; margin-left: 20px;">
                                                <img runat="server" id="Img2" src="~/Base/Images/StartMenuIcos/Routes.png" alt="" />
                                            </div>
                                            <div style="display: inline-block;">
                                                <table border="0" style="margin: 0px; width: 100%; width: 75%; margin-right: auto; margin-left: 80px">
                                                    <tr>
                                                        <td>
                                                            <table>
                                                                <tr>
                                                                    <td style="min-width: 75px; text-align: right">
                                                                        <asp:Label ID="lblActualStatus" runat="server" Text="Estado:" class="spanEmp-Class"></asp:Label>
                                                                    </td>
                                                                    <td style="padding-left: 15px; text-align: left">
                                                                        <asp:Label ID="lblStatusResponse" runat="server" Text="" class="NameText"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <table>
                                                                <tr>
                                                                    <td>
                                                                        <dx:ASPxButton ID="btnActivateService" runat="server" ClientInstanceName="btnActivateServiceClient"
                                                                            Text="Añadir" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat" AutoPostBack="False" CausesValidation="False">
                                                                            <ClientSideEvents Click="function(s,e){ activateService(); }" />
                                                                        </dx:ASPxButton>
                                                                    </td>
                                                                    <td>
                                                                        <dx:ASPxButton ID="btnCancelService" runat="server" ClientInstanceName="btnCancelServiceClient"
                                                                            Text="Añadir" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat" AutoPostBack="False" CausesValidation="False">
                                                                            <ClientSideEvents Click="function(s,e){ cancelService(); }" />
                                                                        </dx:ASPxButton>
                                                                    </td>
                                                                    <td>
                                                                        <dx:ASPxButton ID="btnRegeneratePwd" runat="server" ClientInstanceName="btnRegeneratePwdClient"
                                                                            Text="Añadir" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat" AutoPostBack="False" CausesValidation="False">
                                                                            <ClientSideEvents Click="function(s,e){ regeneratePwd(); }" />
                                                                        </dx:ASPxButton>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                        <td>&nbsp;</td>
                                                    </tr>
                                                </table>
                                            </div>
                                        </div>
                                    </div>
                                    <div style="padding: 15px" id="geniusDiv" runat="server">
                                        <div class="panHeader2">
                                            <span style="">
                                                <asp:Label runat="server" ID="geniusTitle" Text="Activar Genius"></asp:Label>
                                            </span>
                                        </div>
                                        <br />
                                        <div>
                                            <div style="display: inline-block; position: absolute; margin-left: 20px;">
                                                <img runat="server" id="Img1" src="~/Base/Images/PortalRequests/Genius01.png" alt="" />
                                            </div>
                                            <div style="display: inline-block;">
                                                <table border="0" style="margin: 0px; width: 100%; width: 75%; margin-right: auto; margin-left: 80px">

                                                    <tr>
                                                        <td>
                                                            <table>
                                                                <tr>
                                                                    <td style="min-width: 75px; text-align: right">
                                                                        <asp:Label ID="lblActualGenius" runat="server" Text="Estado:" class="spanEmp-Class"></asp:Label>
                                                                    </td>
                                                                    <td style="padding-left: 15px; text-align: left">
                                                                        <asp:Label ID="lblGeniusResponse" runat="server" Text="" class="NameText"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <table>
                                                                <tr runat="server" id="geniusdesc">
                                                                    <td colspan="4">
                                                                        <div style="padding-top: 10px; padding-bottom: 10px;">
                                                                            <asp:Label ID="lblChangeGenius" runat="server" Text="Haz click en el siguiente botón para activar VisualTime Genius en este entorno." class="spanEmp-Class"></asp:Label>
                                                                            <asp:Label ID="lblChangeGenius2" runat="server" Text="IMPORTANTE: Al activar Genius, se perderán TODAS las analíticas creadas y planificadas actualmente en el entorno, al no ser compatibles con la nueva funcionalidad" class="spanEmp-Class"></asp:Label>
                                                                        </div>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <dx:ASPxButton ID="btnActivateGenius" runat="server" ClientInstanceName="btnActivateGeniusClient"
                                                                            Text="Activar Genius" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat" AutoPostBack="False" CausesValidation="False">
                                                                            <ClientSideEvents Click="function(s,e){ changeGenius(); }" />
                                                                        </dx:ASPxButton>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                        <td>&nbsp;</td>
                                                    </tr>
                                                </table>
                                            </div>
                                        </div>
                                    </div>

                                    <div style="padding: 15px" id="PunchRequestsDiv" runat="server">
                                        <div class="panHeader2">
                                            <span style="">
                                                <asp:Label runat="server" ID="PunchRequestsTitle" Text="Desactivar solicitudes al realizar fichajes olvidados"></asp:Label>
                                            </span>
                                        </div>
                                        <br />
                                        <div>
                                            <div style="display: inline-block; position: absolute; margin-left: 20px;">
                                                <img runat="server" id="Img5" width="48" src="~/Base/Images/PortalRequests/ico_ForbiddenPunch.png" alt="" />
                                            </div>
                                            <div style="display: inline-block;">
                                                <table border="0" style="margin: 0px; width: 100%; width: 75%; margin-right: auto; margin-left: 80px">

                                                    <tr>
                                                        <td>
                                                            <table>
                                                                <tr>
                                                                    <td style="min-width: 75px; text-align: right">
                                                                        <asp:Label ID="lblActualPunchRequests" runat="server" Text="Estado:" class="spanEmp-Class"></asp:Label>
                                                                    </td>
                                                                    <td style="padding-left: 15px; text-align: left">
                                                                        <asp:Label ID="lblPunchRequestsResponse" runat="server" Text="" class="NameText"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <table>
                                                                <tr runat="server" id="PunchRequestsdesc">
                                                                    <td colspan="4">
                                                                        <div style="padding-top: 10px; padding-bottom: 10px;">
                                                                            <asp:Label ID="lblChangePunchRequests" runat="server" Text="Haz click en el siguiente botón para desactivar las solicitudes al realizar fichajes olvidados. Los fichajes se crearán directamente como no fiables sin tener que aprobarlos previamente." class="spanEmp-Class"></asp:Label>
                                                                            <asp:Label ID="lblChangePunchRequests2" runat="server" Text="IMPORTANTE: Al realizar esta acción, se aprobarán TODAS las solicitudes pendientes de fichajes olvidados y se eliminarán las notificaciones correspondientes." class="spanEmp-Class"></asp:Label>
                                                                        </div>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <dx:ASPxButton ID="btnDectivatePunchRequests" runat="server" ClientInstanceName="btnDectivatePunchRequestsClient"
                                                                            Text="Desactivar solicitudes para fichajes olvidados" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat" AutoPostBack="False" CausesValidation="False">
                                                                            <ClientSideEvents Click="function(s,e){ deactivatePunchRequests(); }" />
                                                                        </dx:ASPxButton>
                                                                    </td>
                                                                    <td>
                                                                        <dx:ASPxButton ID="btnActivatePunchRequests" runat="server" ClientInstanceName="btnActivatePunchRequestsClient"
                                                                            Text="Activar solicitudes para fichajes olvidados" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat" AutoPostBack="False" CausesValidation="False">
                                                                            <ClientSideEvents Click="function(s,e){ activatePunchRequests(); }" />
                                                                        </dx:ASPxButton>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                        <td>&nbsp;</td>
                                                    </tr>
                                                </table>
                                            </div>
                                        </div>
                                    </div>

                                    <div style="padding: 15px">
                                        <div class="panHeader2">
                                            <span style="">
                                                <asp:Label runat="server" ID="Label1" Text="Administración de seguridad"></asp:Label>
                                            </span>
                                        </div>
                                        <br />
                                        <div>
                                            <div style="display: inline-block; position: absolute; margin-left: 20px;">
                                                <img runat="server" id="Img3" src="~/Base/Images/StartMenuIcos/SecurityChart.png" alt="" />
                                            </div>
                                            <div style="display: inline-block;">
                                                <table border="0" style="margin: 0px; width: 100%; width: 75%; margin-right: auto; margin-left: 80px">
                                                    <tr>
                                                        <td>
                                                            <table>
                                                                <tr>
                                                                    <td style="min-width: 75px; text-align: right">
                                                                        <asp:Label ID="lblActualMode" runat="server" Text="Modo:" class="spanEmp-Class"></asp:Label>
                                                                    </td>
                                                                    <td style="padding-left: 15px; text-align: left">
                                                                        <asp:Label ID="lblModeResponse" runat="server" Text="" class="NameText"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <table>
                                                                <tr runat="server" id="securitydesc">
                                                                    <td colspan="4">
                                                                        <div style="padding-top: 10px; padding-bottom: 10px;">
                                                                            <asp:Label ID="lblChangeModeInfoV1_V3a" runat="server" Text="Para activar el modo de permisos V3, recuerda que antes de realizar el proceso no deben existir excepciones en los supervisores" class="spanEmp-Class"></asp:Label>
                                                                            <asp:Label ID="lblChangeModeInfoV2_V3a" runat="server" Text="Para activar el modo de permisos V3, recuerda que antes de realizar el proceso no deben existir excepciones en los supervisores, salvo la excepción de 'Puede supervisarse a sí mismo'" class="spanEmp-Class"></asp:Label>
                                                                            <asp:Label ID="lblChangeModeInfoV2_V3" runat="server" Text="Importante. Una vez finalizado el proceso debes cerrar sesión y esperar unos minutos a que el cambio de permisos se haya efectuado por completo" class="spanEmp-Class"></asp:Label>
                                                                        </div>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <dx:ASPxButton ID="btnActivateMode" runat="server" ClientInstanceName="btnActivateModeClient"
                                                                            Text="Cambiar modo" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat" AutoPostBack="False" CausesValidation="False">
                                                                            <ClientSideEvents Click="function(s,e){ changeMode(); }" />
                                                                        </dx:ASPxButton>
                                                                    </td>
                                                                    <td style="min-width: 75px; text-align: left">
                                                                        <asp:Label ID="lblPassportTitle" runat="server" Text="Supervisor:" class="spanEmp-Class"></asp:Label>
                                                                    </td>
                                                                    <td style="width: 310px;">
                                                                        <dx:ASPxComboBox ID="cmbPassports" runat="server" Visible="true" CssClass="editTextFormat"
                                                                            Font-Size="11px" Width="302px" ClientInstanceName="cmbPassportsClient" ClientSideEvents-ValueChanged="cmbPassportsClient_ValueChanged">
                                                                        </dx:ASPxComboBox>

                                                                        <dx:ASPxButton ID="btnEliminateExceptions" runat="server" ClientInstanceName="btnEliminateExceptionsClient"
                                                                            Text="Eliminar excepciones de usuarios" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat" AutoPostBack="False" CausesValidation="False">
                                                                            <ClientSideEvents Click="function(s,e){ deleteSupervisorExceptions(); }" />
                                                                        </dx:ASPxButton>
                                                                    </td>
                                                                    <td>
                                                                        <dx:ASPxButton ID="btnCreateChart" runat="server" ClientInstanceName="btnCreateChartClient"
                                                                            Text="Crear organigrama a partir de la estructura de grupos" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat" AutoPostBack="False" CausesValidation="False">
                                                                            <ClientSideEvents Click="function(s,e){ generateNodes(); }" />
                                                                        </dx:ASPxButton>
                                                                    </td>
                                                                    <td style="min-width: 75px; text-align: left">
                                                                        <asp:Label ID="lblLevelTitle" runat="server" Text="Nivel:" class="spanEmp-Class"></asp:Label>
                                                                    </td>

                                                                    <td style="width: 310px;">
                                                                        <dx:ASPxComboBox ID="cmbLevel" runat="server" Visible="true" CssClass="editTextFormat"
                                                                            Font-Size="11px" Width="302px" ClientInstanceName="cmbLevelClient" ClientSideEvents-ValueChanged="cmbLevelClient_ValueChanged">
                                                                        </dx:ASPxComboBox>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                        <td>&nbsp;</td>
                                                    </tr>
                                                </table>
                                            </div>
                                        </div>
                                    </div>

                                    <div style="padding: 15px">
                                        <div class="panHeader2">
                                            <span style="">
                                                <asp:Label runat="server" ID="Label2" Text="Utilización de calendario"></asp:Label>
                                            </span>
                                        </div>
                                        <br />
                                        <div>
                                            <div style="display: inline-block; position: absolute; margin-left: 20px;">
                                                <img runat="server" id="Img4" src="~/Base/Images/StartMenuIcos/Scheduler.png" alt="" />
                                            </div>
                                            <div style="display: inline-block;">
                                                <table border="0" style="margin: 0px; width: 100%; margin-right: auto; margin-left: 80px">
                                                    <tr>
                                                        <td>
                                                            <table>
                                                                <tr>
                                                                    <td style="min-width: 75px; text-align: right">
                                                                        <asp:Label ID="lblActualCalendarMode" runat="server" Text="Modo:" class="spanEmp-Class"></asp:Label>
                                                                    </td>
                                                                    <td style="padding-left: 15px; text-align: left">
                                                                        <asp:Label ID="lblCalendarMode" runat="server" Text="" class="NameText"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <table>
                                                                <tr>
                                                                    <td>
                                                                        <asp:Label ID="lblHasAlternativeShifts" runat="server" Text="" class="NameText"></asp:Label>
                                                                        <dx:ASPxButton ID="btnDeleteAlternativeShifts" runat="server" ClientInstanceName="btnDeleteAlternativeShifts"
                                                                            Text="Eliminar horarios alternativos" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat" AutoPostBack="False" CausesValidation="False">
                                                                            <ClientSideEvents Click="function(s,e){ deleteAlternativeShifts(); }" />
                                                                        </dx:ASPxButton>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                        <td>&nbsp;</td>
                                                    </tr>
                                                </table>
                                            </div>
                                        </div>
                                    </div>

                                    <div style="padding: 15px">
                                        <div class="panHeader2">
                                            <span style="">
                                                <asp:Label runat="server" ID="lblAdvancedParametersTitle" Text="Parámetros avanzados"></asp:Label>
                                            </span>
                                        </div>
                                        <br />
                                        <table border="0" style="margin: 0px; width: 100%; width: 75%; margin-right: auto; margin-left: 20px">
                                            <tr>
                                                <td>
                                                    <table>
                                                        <tr>
                                                            <td style="min-width: 75px; text-align: right" colspan="3">
                                                                <asp:Label ID="lblSaveParameter" runat="server" Text="Guardar parámetro avanzado:" class="NameText"></asp:Label>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <table>
                                                        <tr>
                                                            <td style="min-width: 75px; text-align: left">
                                                                <asp:Label ID="lblParameterName" runat="server" Text="Parámetro:" class="spanEmp-Class"></asp:Label>
                                                            </td>
                                                            <td colspan="2">
                                                                <dx:ASPxTextBox ID="txtParameterName" runat="server" ClientInstanceName="txtParameterNameClient" AutoPostBack="False" CausesValidation="False">
                                                                </dx:ASPxTextBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td style="min-width: 75px; text-align: left">
                                                                <asp:Label ID="lblParameterValue" runat="server" Text="Valor:" class="spanEmp-Class"></asp:Label>
                                                            </td>
                                                            <td>
                                                                <dx:ASPxTextBox ID="txtParameterValue" runat="server" ClientInstanceName="txtParameterValueClient" AutoPostBack="False" CausesValidation="False">
                                                                </dx:ASPxTextBox>
                                                            </td>
                                                            <td>
                                                                <dx:ASPxButton ID="btnSaveParameter" runat="server" ClientInstanceName="btnSaveParameterClient"
                                                                    Text="Grabar" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat" AutoPostBack="False" CausesValidation="False">
                                                                    <ClientSideEvents Click="function(s,e){ saveParameter(); }" />
                                                                </dx:ASPxButton>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                        <br />
                                        <table border="0" style="margin: 0px; width: 100%; width: 75%; margin-right: auto; margin-left: 20px">
                                            <tr>
                                                <td>
                                                    <table>
                                                        <tr>
                                                            <td style="min-width: 75px; text-align: right" colspan="3">
                                                                <asp:Label ID="lblParameterRequest" runat="server" Text="Consulta parámetro avanzado:" class="NameText"></asp:Label>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <table>
                                                        <tr>
                                                            <td style="min-width: 75px; text-align: left">
                                                                <asp:Label ID="lblParameterQueryValue" runat="server" Text="Parámetro:" class="spanEmp-Class"></asp:Label>
                                                            </td>
                                                            <td colspan="2">
                                                                <dx:ASPxTextBox ID="txtParameterQueryValue" runat="server" ClientInstanceName="txtParameterQueryValueClient" AutoPostBack="False" CausesValidation="False">
                                                                </dx:ASPxTextBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td style="min-width: 75px; text-align: left">
                                                                <asp:Label ID="lblQueryParameterValue" runat="server" Text="Valor:" class="spanEmp-Class"></asp:Label>
                                                            </td>
                                                            <td>
                                                                <dx:ASPxTextBox ID="txtParameterQueryResult" runat="server" ReadOnly="true" ClientInstanceName="txtParameterQueryResultClient" AutoPostBack="False" CausesValidation="False">
                                                                </dx:ASPxTextBox>
                                                            </td>
                                                            <td>
                                                                <dx:ASPxButton ID="btnQueryValue" runat="server" ClientInstanceName="btnQueryValueClient"
                                                                    Text="Consultar" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat" AutoPostBack="False" CausesValidation="False">
                                                                    <ClientSideEvents Click="function(s,e){ queryParameter(); }" />
                                                                </dx:ASPxButton>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>

                                    <div style="padding: 15px">
                                        <div class="panHeader2">
                                            <span style="">
                                                <asp:Label runat="server" ID="lblUsersTitle" Text="Crear usuario de formación"></asp:Label>
                                            </span>
                                        </div>
                                        <br />
                                        <table border="0" style="margin: 0px; width: 100%; width: 75%; margin-right: auto; margin-left: 20px">
                                            <tr>
                                                <td>
                                                    <table>
                                                        <tr>
                                                            <td style="min-width: 75px; text-align: right" colspan="3">
                                                                <asp:Label ID="lblSaveUser" runat="server" Text="Guardar usuario de formación:" class="NameText"></asp:Label>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <table>
                                                        <tr>
                                                            <td style="min-width: 75px; text-align: left">
                                                                <asp:Label ID="lblSecurityFunction" class="spanEmp-Class" runat="server" Text="Rol"></asp:Label>
                                                            </td>
                                                            <td colspan="2">
                                                                <div style="float: left">
                                                                    <dx:ASPxComboBox ID="cmbSecurityFunctions" runat="server" Width="170px">
                                                                        <ClientSideEvents GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                    </dx:ASPxComboBox>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td style="min-width: 75px; text-align: left">
                                                                <asp:Label ID="lblUser" runat="server" Text="Usuario:" class="spanEmp-Class"></asp:Label>
                                                            </td>
                                                            <td colspan="2">
                                                                <dx:ASPxTextBox ID="txtUser" runat="server" ClientInstanceName="txtUserClient" AutoPostBack="False" CausesValidation="False">
                                                                </dx:ASPxTextBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td style="min-width: 75px; text-align: left">
                                                                <asp:Label ID="lblPassword" runat="server" Text="Contraseña:" class="spanEmp-Class"></asp:Label>
                                                            </td>
                                                            <td>
                                                                <dx:ASPxTextBox ID="txtPassword" runat="server" ClientInstanceName="txtPasswordClient" AutoPostBack="False" CausesValidation="False" AutoCompleteType="Disabled">
                                                                </dx:ASPxTextBox>
                                                            </td>
                                                            <td>
                                                                <dx:ASPxButton ID="btnSaveUser" runat="server" ClientInstanceName="btnSaveUserClient"
                                                                    Text="Crear Usuario" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat" AutoPostBack="False" CausesValidation="False">
                                                                    <ClientSideEvents Click="function(s,e){ saveUser(); }" />
                                                                </dx:ASPxButton>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                        <br />
                                    </div>

                                    <br />
                                    <div style="display: none">
                                        <div class="panHeader2">
                                            <span style="">
                                                <asp:Label runat="server" ID="lblBackups" Text="Backups"></asp:Label></span>
                                        </div>
                                        <br />
                                        <table border="0" style="margin: 0px; width: 100%; width: 75%; margin-right: auto; margin-left: 20px">
                                            <tr>
                                                <td>
                                                    <table>
                                                        <tr>
                                                            <td>
                                                                <dx:ASPxButton ID="btnCopiaTecnica" runat="server" ClientInstanceName="btnCopiaTecnicaClient"
                                                                    Text="Añadir" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat" AutoPostBack="False" CausesValidation="False">
                                                                    <ClientSideEvents Click="function(s,e){ doSimpleBackup(); }" />
                                                                </dx:ASPxButton>
                                                            </td>
                                                            <td>
                                                                <dx:ASPxButton ID="btnCopiaCompleta" runat="server" ClientInstanceName="btnCopiaTecnicaClient"
                                                                    Text="Añadir" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat" AutoPostBack="False" CausesValidation="False">
                                                                    <ClientSideEvents Click="function(s,e){ doFullBackup(); }" />
                                                                </dx:ASPxButton>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                                <td>&nbsp;</td>
                                            </tr>
                                        </table>
                                    </div>
                                </div>

                                <div id="templateAdmin" class="contentPanel" style="display: none" runat="server" name="menuPanel">
                                    <!-- Este div es el header -->
                                    <div class="panBottomMargin">
                                        <div class="panHeader2 panBottomMargin">
                                            <span class="panelTitleSpan">
                                                <asp:Label runat="server" ID="lblTemplatesEditTitle" Text="Planillas"></asp:Label>
                                            </span>
                                        </div>
                                    </div>

                                    <!-- Este div es un formulario -->
                                    <div class="panBottomMargin">
                                        <div class="divRow" style="padding-top: 10px;">
                                            <div class="componentForm" style="width: 1280px">
                                                <div style="float: left">
                                                    <asp:Label ID="lblAdvTemplateEdit" runat="server" Text="Tipo de plantilla:"></asp:Label>
                                                </div>
                                                <div style="float: left; padding-left: 10px;">
                                                    <dx:ASPxComboBox runat="server" ID="cmbAdvTemplateType" Width="250px" NullText="_____" ClientInstanceName="cmbAdvTemplateTypeClient">
                                                        <ClientSideEvents SelectedIndexChanged="function(s, e) { loadAdvTemplateNames(); }" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                    </dx:ASPxComboBox>
                                                </div>
                                                <div style="float: left; padding-left: 20px;">
                                                    <asp:Label ID="lblTemplateName" runat="server" Text="Nombre de la plantilla:"></asp:Label>
                                                </div>
                                                <div style="float: left; padding-left: 10px;">
                                                    <dx:ASPxComboBox runat="server" ID="cmbAdvTemplateName" Width="250px" NullText="_____" ClientInstanceName="cmbAdvTemplateNameClient">
                                                        <ClientSideEvents SelectedIndexChanged="function(s, e) { loadAdvTemplateContent(); }" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                    </dx:ASPxComboBox>
                                                </div>
                                                <div style="float: left; margin: -4px; padding-left: 14px;">
                                                    <dx:ASPxButton ID="btSaveChanges" runat="server" AutoPostBack="False" CausesValidation="False" Text="Guardar cambios" ToolTip="Guardar cambios"
                                                        HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat">
                                                        <ClientSideEvents Click="function(s, e) { saveCurrentTemplateFile(); }" />
                                                    </dx:ASPxButton>
                                                </div>
                                                <div style="float: left; margin: -4px; padding-left: 10px;">
                                                    <dx:ASPxButton ID="btNewDocument" runat="server" AutoPostBack="False" CausesValidation="False" Text="Guardar como..." ToolTip="Guardar como..."
                                                        HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat">
                                                        <ClientSideEvents Click="function(s, e) { duplicateCurrentTemplate(); }" />
                                                    </dx:ASPxButton>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <!-- Este div es un formulario -->
                                    <div class="panBottomMargin">
                                        <dx:ASPxSpreadsheet ID="advTemplate" runat="server" Width="100%" Height="570px" ActiveTabIndex="0" ClientInstanceName="advTemplateClient">
                                            <ClientSideEvents DocumentChanged="onDocumentChanged" EndSynchronization="onEndSynchronization" />
                                        </dx:ASPxSpreadsheet>
                                    </div>
                                </div>
                            </dx:PanelContent>
                        </PanelCollection>
                    </dx:ASPxCallbackPanel>
                </div>
            </div>
        </div>
    </div>
    <asp:HiddenField ID="SaaSAdmin_TabVisibleName" Value="" runat="server" />
    <Local:MessageFrame ID="MessageFrame1" runat="server" />
    <Local:ExternalForm ID="externalform1" runat="server" />

    <dx:ASPxPopupControl ID="PopupNewTemplateName" runat="server" AllowDragging="True" CloseAction="None" Modal="True" ClientInstanceName="PopupNewTemplateNameClient"
        PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter" Height="225px" Width="420px"
        ShowHeader="False" ScrollBars="Auto" ShowPageScrollbarWhenModal="True" PopupAnimationType="None" BackColor="Transparent" ContentStyle-Paddings-Padding="0px" Border-BorderColor="Transparent" ShowShadow="false">
        <ContentCollection>
            <dx:PopupControlContentControl ID="PopupControlContentControl1" runat="server">
                <dx:ASPxPanel ID="ASPxPanel3" runat="server" Width="0px" Height="0px">
                    <PanelCollection>
                        <dx:PanelContent ID="PanelContent3" runat="server">
                            <div class="bodyPopupExtended" style="table-layout: fixed; height: 185px; width: 375px;">
                                <table width="100%">
                                    <tr>

                                        <td style="padding-bottom: 10px;" height="20px" valign="top">
                                            <div class="panHeader2">
                                                <span style="">
                                                    <asp:Label ID="lblTitle" runat="server" Text="Guardar como..."></asp:Label>
                                                </span>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr valign="top">
                                        <td>
                                            <table>
                                                <tr>
                                                    <td valign="top" style="padding-left: 15px; padding-right: 15px;">
                                                        <img style="width: 32px" alt="" id="Img6" src="~/Base/Images/logovtl.ico" runat="server" />
                                                    </td>
                                                    <td align="left" valign="middle">
                                                        <asp:Label ID="lblDescription1" runat="server" CssClass="editTextFormat" Text="Introduzca el nombre de la plantilla"></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr valign="top">
                                        <td>
                                            <table style="padding-left: 95px;">
                                                <tr>
                                                    <td>
                                                        <table style="width: 100%" cellspacing="0" cellpadding="0" border="0">
                                                            <tr>
                                                                <td>
                                                                    <asp:Label ID="lblName" runat="server" CssClass="editTextFormat" Text="Nombre"></asp:Label>
                                                                </td>
                                                                <td style="padding-left: 5px">
                                                                    <dx:ASPxTextBox ID="newObjectName" runat="server" ClientInstanceName="newObjectName_Client" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                                <table style="margin-left: auto;">
                                    <tr>
                                        <td>
                                            <dx:ASPxButton ID="btnAccept" ClientInstanceName="btnAcceptClient" runat="server" AutoPostBack="False" CausesValidation="False" Text="Guardar" ToolTip="Guardar"
                                                HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                                <ClientSideEvents Click="AcceptTemplateNameClick" />
                                            </dx:ASPxButton>
                                        </td>
                                        <td>
                                            <dx:ASPxButton ID="btnCancel" ClientInstanceName="btnCancelClient" runat="server" AutoPostBack="False" CausesValidation="False" Text="Cancelar" ToolTip="Cancelar"
                                                HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                                <ClientSideEvents Click="CancelTemplateNameClick" />
                                            </dx:ASPxButton>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </dx:PanelContent>
                    </PanelCollection>
                </dx:ASPxPanel>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>

    <!-- POPUP CAPTCHA -->
    <dx:ASPxPopupControl ID="PopupCaptcha" runat="server" AllowDragging="False" CloseAction="None" Modal="True" ContentUrl="~/Security/SaaSAdminCaptcha.aspx"
        PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter" Width="470px" Height="320px"
        ShowHeader="False" ScrollBars="Auto" ShowPageScrollbarWhenModal="True" ClientInstanceName="PopupCaptcha_Client" PopupAnimationType="None" BackColor="Transparent" ContentStyle-Paddings-Padding="0px" Border-BorderColor="Transparent" ShowShadow="false">
    </dx:ASPxPopupControl>

    <script language="javascript" type="text/javascript">
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
        }
    </script>
</asp:Content>