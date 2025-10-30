<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Requests_srvRequests" CodeBehind="srvRequests.aspx.vb" %>

<%@ Register Src="~/Requests/WebUserForms/frmComments.ascx" TagName="frmComments" TagPrefix="roForms" %>
<%@ Register Src="~/Requests/WebUserForms/frmDayDetails.ascx" TagName="frmDayDetails" TagPrefix="roForms" %>
<%@ Register Src="~/Requests/WebUserControls/frmRequestResume.ascx" TagName="frmRequestResume" TagPrefix="roForms" %>

<!-- Pantalla Llista Terminals -->
<table runat="server" id="tblRequests" border="0" cellpadding="0" cellspacing="0" style="width: 100%;" class="defaultContrastColor">
</table>

<div id="divGeneral" style="width: 100%; height: 100%; padding: 0px; display: none;" runat="server" name="menuPanel">

    <div class="panHeader2">
        <span style="">
            <asp:label runat="server" id="lblEmployeeTitle" Text="${Employee}"></asp:label>
        </span>
    </div>
    <br />
    <table border="0" style="padding-top: 0px; padding-bottom: 10px;">
        <tr>
            <td rowspan="2" align="center" style="padding-left: 30px;">
                <img id="imgEmployee" style="cursor: pointer; border-radius: 50%;" height="90" runat="server" />
            </td>
            <td style="padding-left: 10px;" align="right">
                <asp:Label id="lblEmployeeName" text="${Employee}: " runat="server" class="spanEmp-Class"></asp:Label>
            </td>
            <td colspan="2" style="padding-left: 5px;">
                <a id="lnkEmployeeName" onclick="" href="javascript: void(0)" runat="server">
                    <asp:Label id="txtEmployeeName" text="" runat="server" Font-Bold="true" class="spanEmp-Class"></asp:Label>
                </a>
            </td>
        </tr>
        <tr>
            <td style="padding-left: 10px;" align="right">
                <asp:Label id="lblGroupName" text="${Group}: " runat="server" class="spanEmp-Class"></asp:Label>
            </td>
            <td style="padding-left: 5px;">
                <a id="lnkGroupName" onclick="" href="javascript: void(0)" runat="server">
                    <asp:Label id="txtGroupName" text="" runat="server" Font-Bold="true" class="spanEmp-Class"></asp:Label>
                </a>
            </td>
        </tr>
    </table>
    <div class="panHeader2" style="text-align: left;">
        <span style="">
            <asp:label runat="server" id="lblRequestTitle" Text="Solicitud"></asp:label>
        </span>
    </div>
    <br />
    <table border="0" style="width: 98%; padding-top: 0px; padding-bottom: 10px;">
        <tr>
            <td style="padding-left: 10px; padding-top: 5px;" align="right">
                <asp:Label id="lblRequestType" text="Tipo solicitud: " runat="server" class="spanEmp-Class"></asp:Label>
            </td>
            <td style="padding-top: 5px; padding-left: 5px;">
                <table cellpadding="0" cellspacing="0">
                    <tr>
                        <td>
                            <asp:Label id="txtRequestType" text="" runat="server" class="spanEmp-Class"></asp:Label>
                        </td>
                        <td style="padding-left: 5px;">
                            <img id="imgRequestType" height="32" runat="server" /></td>
                    </tr>
                </table>
            </td>
            <td rowspan="2" style="padding-left: 5px;">
                <table>
                    <tr>
                        <td style="padding-left: 5px;">
                            <a id="lnkApprove" onclick="" href="javascript: void(0)" runat="server">
                                <table>
                                    <tr>
                                        <td>
                                            <div id="divApproveLink" class="buttonApproveRequest" runat="server"></div>
                                        </td>
                                        <td>
                                            <asp:Label id="lblApproveLink" text="Aprobar solicitud" runat="server" class="spanEmp-Class"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </a>
                        </td>
                    </tr>
                    <tr>
                        <td style="padding-left: 5px;">
                            <a id="lnkRefuse" onclick="" href="javascript: void(0)" runat="server">
                                <table>
                                    <tr>
                                        <td>
                                            <div id="divRefuseLink" class="buttonRefuseRequest" runat="server"></div>
                                        </td>
                                        <td>
                                            <asp:Label id="lblRefuseLink" text="Denegar solicitud" runat="server" class="spanEmp-Class"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </a>
                        </td>
                    </tr>
                </table>
                <roForms:frmComments ID="frmComments1" runat="server" />
                <roForms:frmDayDetails ID="frmDayDetails1" runat="server" />
            </td>
        </tr>
        <tr>
            <td style="padding-left: 10px; padding-top: 5px;" align="right">
                <asp:Label id="lblRequestDetail" text="Detalle: " runat="server" class="spanEmp-Class"></asp:Label>
            </td>
            <td style="padding-top: 5px; padding-left: 5px;">
                <asp:Label id="lblRequestInfo" text="" runat="server" class="spanEmp-Class"></asp:Label>
            </td>
        </tr>

        <tr id="trDocumentsWithoutPermissions" runat="server" visible="false">
            <td style="padding-left: 10px; padding-top: 5px;" align="right">
                <asp:Label id="lblRequestDocumentTitle" text="Documento: " runat="server" class="spanEmp-Class"></asp:Label>
            </td>
            <td style="padding-left: 10px; padding-top: 5px;" align="left">
                <asp:Label id="lblRequestDocumentDenied" text="Documento: " runat="server" class="spanEmp-Class"></asp:Label>
            </td>
        </tr>

        <tr id="trDocuments" runat="server" visible="false">
            <td style="padding-left: 10px; padding-top: 5px;" align="right">
                <asp:Label id="lblRequestDocument" text="Documento: " runat="server" class="spanEmp-Class"></asp:Label>
            </td>

            <td style="padding-top: 5px; padding-left: 5px;">
                <div id="document1" runat="server" style="display: none; float: left;">
                    <div class="btnFlat" style="border-radius: 5px !important; height: 15px !important;">
                        <asp:Label ID="lbldocument1" runat="server" Text=""></asp:Label>
                    </div>
                </div>
                <div id="document2" runat="server" style="display: none; float: left;">
                    <div class="btnFlat" style="border-radius: 5px !important; height: 15px !important;">
                        <asp:Label ID="lbldocument2" runat="server" Text=""></asp:Label>
                    </div>
                </div>
                <div id="document3" runat="server" style="display: none; float: left;">
                    <div class="btnFlat" style="border-radius: 5px !important; height: 15px !important;">
                        <asp:Label ID="lbldocument3" runat="server" Text=""></asp:Label>
                    </div>
                </div>
                <div id="document4" runat="server" style="display: none; float: left;">
                    <div class="btnFlat" style="border-radius: 5px !important; height: 15px !important;">
                        <asp:Label ID="lbldocument4" runat="server" Text=""></asp:Label>
                    </div>
                </div>
                <div id="document5" runat="server" style="display: none; float: left;">
                    <div class="btnFlat" style="border-radius: 5px !important; height: 15px !important;">
                        <asp:Label ID="lbldocument5" runat="server" Text=""></asp:Label>
                    </div>
                </div>
            </td>
        </tr>

        <tr id="trUserFieldsChange" runat="server" visible="false">
            <td style="padding-left: 5px; padding-top: 5px;" colspan="4">
                <table width="100%" cellpadding="0" cellspacing="0">
                    <tr>
                        <td align="left" style="padding-left: 15px;">
                            <asp:Label id="lblUserFieldsChangeTitle" text="Estado actual campo de la ficha:" runat="server" class="spanEmp-Class"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td id="tdUserFields" align="left" runat="server" style="padding-left: 15px; padding-top: 5px;"></td>
                    </tr>
                    <tr>
                        <td align="left" style="padding-left: 15px; padding-top: 5px;">
                            <a id="lnkEmployeeUserFields" onclick="" href="javascript: void(0)" runat="server">
                                <table>
                                    <tr>
                                        <td>
                                            <div class="icoEmpViewUserFieldsDetail"></div>
                                        </td>
                                        <td>
                                            <asp:Label id="lblEmployeeUserFieldsLink" text="Consulta ficha del empleado" runat="server" class="spanEmp-Class"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </a>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr id="trForbiddenPunch" runat="server" visible="false">
            <td style="padding-left: 5px; padding-top: 5px;" colspan="4">
                <table width="100%" cellpadding="0" cellspacing="0">
                    <tr>
                        <td align="left" style="padding-left: 15px; padding-top: 5px;">
                            <a id="lnkEmployeeMoves" onclick="" href="javascript: void(0)" runat="server">
                                <table>
                                    <tr>
                                        <td>
                                            <div class="icoEmpViewMovesDetail"></div>
                                        </td>
                                        <td>
                                            <asp:Label id="lblEmployeeMovesLink" text="Consulta ${Punches} del día del ${Employee}" runat="server" class="spanEmp-Class"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </a>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr id="trTelecommuteResume" runat="server" visible="false">
            <td colspan="3">
                <div style="padding: 20px">
                    <div class="panHeader3 panBottomMargin">
                        <span class="panelTitleSpan">
                            <asp:Label id="lblTCResume" text="Estado de teletrabajo: " runat="server"></asp:Label>
                        </span>
                    </div>
                    <div style="padding-left: 25px">
                        <asp:Label id="lblResumePeriod" text="Periodo entre <b>01/09/2022</b> y <b>30/09/2022</b> con un máximo de <b>30%/mes</b> de teletrabajo" class="spanEmp-Class" runat="server"></asp:Label>
                        <br />
                        <asp:Label id="lblTCResumeType" text="El usuario tiene planificados <b>8</b> días de su acuerdo de teletrabajo (<b>56</b> horas de un total de <b>460</b>)" class="spanEmp-Class" runat="server"></asp:Label>
                    </div>
                    <%--<br />
                    <div class="panHeader3 panBottomMargin">
                        <span class="panelTitleSpan">
                            <asp:Label id="lblTCStats" text="Estado de teletrabajo: " runat="server"></asp:Label>
                        </span>
                    </div>
                    <div style="padding-left:25px">
                        <asp:Label id="lblStatsPeriod" text="En el periodo: <b>01/09/2022</b> y <b>30/09/2022</b>" class="spanEmp-Class" runat="server"></asp:Label>
                        <br />
                        <asp:Label id="lblStatsHours" text="Se han realizado un total de <b>35</b> horas de las <b>460</b> de teletrabajo (<b>8%</b>)" class="spanEmp-Class" runat="server"></asp:Label>
                    </div>--%>
                </div>
            </td>
        </tr>

        <tr id="trDailyRecordResume" runat="server" visible="false">
            <td style="padding-left: 5px; padding-top: 5px;" colspan="4">
                <table width="100%" cellpadding="0" cellspacing="0">
                    <tr>
                        <td align="left" style="padding-left: 15px; padding-top: 5px;">
                            <a id="lnkEmployeeMovesDR" onclick="" href="javascript: void(0)" runat="server">
                                <table>
                                    <tr>
                                        <td>
                                            <div class="icoEmpViewMovesDetail"></div>
                                        </td>
                                        <td>
                                            <asp:Label id="lblEmployeeMovesLinkDR" text="Consulta fichajes del día del usuario" runat="server" class="spanEmp-Class"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </a>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>

        <tr id="trPlanification" runat="server" visible="false">
            <td style="padding-left: 5px; padding-top: 5px;" colspan="4">
                <table width="100%" cellpadding="0" cellspacing="0">
                    <tr>
                        <td align="left" style="display: none;">
                            <asp:Label id="lblPlanificationTitle" text="Estado actual planificación:" runat="server" class="spanEmp-Class"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td id="tdPlanification" align="left" runat="server" style="padding-left: 15px; padding-top: 5px;"></td>
                    </tr>
                    <tr id="trVacationsResume" runat="server" style="display: none;">
                        <td>
                            <table width="100%">
                                <tr>
                                    <td align="left">
                                        <asp:Label id="lblVacationsResumeTitle" text="Resumen estado días de vacaciones o permisos:" runat="server" class="spanEmp-Class"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td id="tdVacationsResume" align="left" runat="server" style="padding-left: 15px; padding-top: 5px;"></td>
                                </tr>
                            </table>
                        </td>
                    </tr>

                    <tr>
                        <td align="left" style="padding-left: 15px; padding-top: 5px;">
                            <a id="lnkPlanificationAnual" onclick="" href="javascript: void(0)" runat="server">
                                <table>
                                    <tr>
                                        <td>
                                            <div class="icoEmpViewAnnualDetail"></div>
                                        </td>
                                        <td>
                                            <asp:Label id="lblPlanificationAnualLink" text="Consulta de la planificación anual del ${Employee}" runat="server" class="spanEmp-Class"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </a>
                        </td>
                    </tr>
                    <tr>
                        <td align="left" style="padding-left: 15px; padding-top: 5px;">
                            <a id="lnkPlanificationGroup" onclick="" href="javascript: void(0)" runat="server">
                                <table>
                                    <tr>
                                        <td>
                                            <div class="icoEmpViewPlanificationDetail"></div>
                                        </td>
                                        <td>
                                            <asp:Label id="lblPlanificationGroupLink" text="Consulta de la planificación del ${Group} {0}" runat="server" class="spanEmp-Class"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </a>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td style="padding-left: 20px; padding-top: 5px;" colspan="4">
                <table>
                    <tr id="trHolidaysResume" runat="server" style="display: none;">
                        <td colspan="2" align="left">
                            <a id="lnkRequestDaysResume" onclick="" href="javascript: void(0)" runat="server">
                                <table>
                                    <tr>
                                        <td>
                                            <div class="icoRequestDaysResume"></div>
                                        </td>
                                        <td>
                                            <asp:Label id="lblRequestDaysResume" text="Consulta el detalle de días solicitados" runat="server" class="spanEmp-Class"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </a>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <div class="icoEmpViewUserFieldsDetail"></div>
                        </td>
                        <td>
                            <div id="divShowSupervisorsPending" runat="server" style="display: none">
                                <div class="btnFlat">
                                    <a href="javascript: void(0)" id="aAcept" runat="server" onclick="">
                                        <asp:Label ID="btnShowSupervisors" runat="server" Text="Mostrar supervisores pendientes"></asp:Label>
                                    </a>
                                </div>
                            </div>
                            <div id="divLblSupervisorsPending" runat="server" style="display: none">
                                <asp:Label id="lblNextLevelPassports" style="font-size: 11px;" runat="server" class="spanEmp-Class"></asp:Label>
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>

    <div class="panHeader2">
        <span style="">
            <asp:label runat="server" id="lblHistoryTitle" Text="Histórico"></asp:label>
        </span>
    </div>
    <br />
    <table border="0" style="padding-top: 5px">
        <tr>
            <td style="padding-left: 10px;" align="right">
                <asp:Label id="lblRequestDate" text="Solicitado: " runat="server" class="spanEmp-Class"></asp:Label>
            </td>
            <td colspan="3" style="padding-left: 5px;">
                <table cellpadding="0" cellspacing="0">
                    <tr>
                        <td>
                            <asp:Label id="txtRequestDate" text="" runat="server" Font-Bold="true" class="spanEmp-Class"></asp:Label>
                        </td>
                        <td>
                            <asp:Label id="txtRequestDateDays" text="" runat="server" Font-Bold="true" class="spanEmp-Class"></asp:Label>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td style="padding-left: 10px;" align="right">
                <asp:Label id="lblRequestState" text="Estado: " runat="server" class="spanEmp-Class"></asp:Label>
            </td>
            <td style="padding-left: 5px;">
                <table cellpadding="0" cellspacing="0">
                    <tr>
                        <td style="padding-left: 5px;">
                            <img id="imgRequestState" height="16" runat="server" /></td>
                        <td>
                            <asp:Label id="txtRequestState" text="" runat="server" class="spanEmp-Class"></asp:Label>
                        </td>
                    </tr>
                </table>
            </td>
            <td align="right" style="padding-left: 10px;">
                <a id="lnkRequestApprovals" onclick="" href="javascript: void(0)" runat="server">
                    <table>
                        <tr>
                            <td>
                                <img id="imgRequestApprovals" src="/Requests/Images/ApprovalsHistory16.png" height="16" runat="server" /></td>
                            <td>
                                <asp:Label id="lblRequestApprovals" text="Histórico aprobaciones" runat="server" class="spanEmp-Class"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </a>
            </td>
            <td align="right" style="padding-left: 10px;">
                <a id="lnkRequestOrgChart" onclick="" href="javascript: void(0)" runat="server">
                    <table>
                        <tr>
                            <td>
                                <img id="img1" src="/SecurityChart/Images/SecurityChart.png" height="16" runat="server" /></td>
                            <td>
                                <asp:Label id="lblRequestOrgChart" text="Organigrama de solicitud" runat="server" class="spanEmp-Class"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </a>
            </td>
        </tr>
        <tr>
            <td style="padding-left: 10px;" align="right">
                <asp:Label id="lblLastApproval" text="Último cambio: " runat="server" class="spanEmp-Class"></asp:Label>
            </td>
            <td style="padding-left: 5px;" colspan="3">
                <asp:Label id="txtLastApproval" text="" runat="server" class="spanEmp-Class"></asp:Label>
            </td>
            <td></td>
        </tr>
        <tr>
            <td valign="top" style="padding-left: 10px;" align="right">
                <asp:Label id="lblComments" text="Comentarios empleado: " runat="server" class="spanEmp-Class"></asp:Label>
            </td>
            <td colspan="3" valign="top" style="padding-left: 5px;">
                <asp:Label id="txtComments" text="" runat="server" class="spanEmp-Class"></asp:Label>
                <%--<textarea id="txtComments" runat="server" rows="5" style="width: 475px; height: 100px;" class="textClass" ConvertControl="TextArea" CCallowBlank="true"  readonly="readonly" ></textarea> --%>
            </td>
        </tr>
    </table>
</div>