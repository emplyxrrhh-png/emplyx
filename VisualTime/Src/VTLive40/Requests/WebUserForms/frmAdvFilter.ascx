<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.frmAdvFilter" CodeBehind="frmAdvFilter.ascx.vb" %>

<div class="RoundCornerFrame roundCorner">
    <table cellpadding="0" cellspacing="0" width="100%" style="display: ;">
        <tr>
            <td id="mode3Inf" runat="server" style="display: none">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblRequestStateTitle" Text="Estado:" runat="server"></asp:Label></td>
                        <td width="16px" style="padding-left: 5px;"><a id="icoStatePending" href="javascript: void(0)" class="RequestListIcoStatePending RequestListIcoPressed" title="" runat="server" style="display: ;"></a></td>
                        <td width="16px"><a id="icoStateOnGoing" href="javascript: void(0)" class="RequestListIcoStateOnGoing RequestListIcoPressed" title="" runat="server" style="display: ;"></a></td>
                        <td width="16px"><a id="icoStateAccepted" href="javascript: void(0)" class="RequestListIcoStateAccepted RequestListIcoPressed" title="" runat="server" style="display: ;"></a></td>
                        <td width="16px"><a id="icoStateDenied" href="javascript: void(0)" class="RequestListIcoStateDenied RequestListIcoPressed" title="" runat="server" style="display: ;"></a></td>
                        <td width="16px"><a id="icoStateCanceled" href="javascript: void(0)" class="RequestListIcoStateCanceled RequestListIcoPressed" title="" runat="server" style="display: ;"></a></td>
                    </tr>
                </table>
            </td>
            <td>
                <table cellpadding="0" cellspacing="0">
                    <tr>
                        <td>
                            <asp:Label ID="lblOrder" Text="Orden:" runat="server"></asp:Label></td>
                        <td width="100px" style="padding-left: 5px;">
                            <dx:ASPxComboBox ID="cmbOrder" runat="server" Width="90px" />
                        </td>
                        <td width="16px"><a id="icoAscending" href="javascript: void(0)" class="RequestListIcoAscending RequestListIcoPressed" title="" runat="server"></a></td>
                        <td width="16px"><a id="icoDescending" href="javascript: void(0)" class="RequestListIcoDescending RequestListIcoUnPressed" title="" runat="server"></a></td>
                        <td style="padding-left: 10px; padding-right: 5px;">
                            <asp:Label ID="lblVerSolicitudes1" runat="server" Text="Ver"></asp:Label></td>
                        <td width="40px">
                            <dx:ASPxComboBox ID="cmbNumRequests" runat="server" />
                        </td>
                    </tr>
                </table>
            </td>
            <td width="16px">
                <a href="javascript: void(0)" class="icoFilter icoClass" id="advFilterButtonShow" runat="server" title="" style=""></a>
                <!-- Filtre Avançat PEND-->
                <div id="divFiltreAvan" class="AdvancedFilterPopup" style="display: none;" runat="server">

                    <div class="bodyPopupExtended" style="width: 100%">
                        <!-- selectors de criteris -->
                        <table border="0" width="100%">
                            <tr>
                                <td>
                                    <div id="divFilterContainer" runat="server" style="width: 99%; height: 100%;">
                                        <table cellpadding="0" cellspacing="0" width="100%">
                                            <tr>
                                                <td>
                                                    <div class="panHeader2"><span class="AdvancedHeaderTitleStyle"><%=Me.Language.Translate("AdvancedFilter.Title", Me.DefaultScope)%></span></div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-top: 10px;">
                                                    <table>
                                                        <tr>
                                                            <td style="white-space: nowrap;">
                                                                <asp:Label ID="lblRequestDateBegin" runat="server" Text="Solicitada desde: "></asp:Label></td>
                                                            <td style="width: 75px;">
                                                                <dx:ASPxDateEdit ID="txtRequestDateBegin" PopupVerticalAlign="WindowCenter" runat="server" Width="105px">
                                                                    <ValidationSettings ErrorDisplayMode="None" />
                                                                </dx:ASPxDateEdit>
                                                            </td>
                                                            <td style="padding-left: 20px; padding-right: 20px;">&nbsp;</td>
                                                            <td style="white-space: nowrap;">
                                                                <asp:Label ID="lblRequestDateEnd" runat="server" Text="Hasta: "></asp:Label></td>
                                                            <td style="width: 75px;">
                                                                <dx:ASPxDateEdit ID="txtRequestDateEnd" PopupVerticalAlign="WindowCenter" runat="server" Width="105px">
                                                                    <ValidationSettings ErrorDisplayMode="None" />
                                                                </dx:ASPxDateEdit>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-top: 10px;">
                                                    <table>
                                                        <tr>
                                                            <td style="white-space: nowrap;">
                                                                <asp:Label ID="lblRequestedDateBegin" runat="server" Text="Fecha efecto desde: "></asp:Label></td>
                                                            <td style="width: 75px;">
                                                                <dx:ASPxDateEdit ID="txtRequestedDateBegin" PopupVerticalAlign="WindowCenter" runat="server" Width="105px">
                                                                    <ValidationSettings ErrorDisplayMode="None" />
                                                                </dx:ASPxDateEdit>
                                                            </td>
                                                            <td style="padding-left: 20px; padding-right: 20px;">&nbsp;</td>
                                                            <td style="white-space: nowrap;">
                                                                <asp:Label ID="lblRequestedDateEnd" runat="server" Text="Hasta: "></asp:Label></td>
                                                            <td style="width: 75px;">
                                                                <dx:ASPxDateEdit ID="txtRequestedDateEnd" PopupVerticalAlign="WindowCenter" runat="server" Width="105px">
                                                                    <ValidationSettings ErrorDisplayMode="None" />
                                                                </dx:ASPxDateEdit>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-top: 10px;">
                                                    <table>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="lblRequestType" runat="server" Text="Tipo de solicitud: "></asp:Label></td>
                                                            <td width="16px"><a id="icoTypeUserFieldsChange" href="javascript: void(0)" class="RequestListIcoTypeUserFieldsChange RequestListIcoPressed" title="" runat="server"></a></td>
                                                            <td width="16px"><a id="icoTypeForbiddenPunch" href="javascript: void(0)" class="RequestListIcoTypeForbiddenPunch RequestListIcoPressed" title="" runat="server"></a></td>
                                                            <td width="16px"><a id="icoTypeJustifyPunch" href="javascript: void(0)" class="RequestListIcoTypeJustifyPunch RequestListIcoPressed" title="" runat="server"></a></td>
                                                            <td width="16px"><a id="icoTypeExternalWorkResumePart" href="javascript: void(0)" class="RequestListIcoTypeExternalWorkResumePart RequestListIcoPressed" title="" runat="server"></a></td>
                                                            <td width="16px"><a id="icoTypeExternalWorkWeekResume" href="javascript: void(0)" class="RequestListIcoTypeExternalWorkWeekResume RequestListIcoPressed" title="" runat="server"></a></td>
                                                            <td width="16px"><a id="icoTypeChangeShift" href="javascript: void(0)" class="RequestListIcoTypeChangeShift RequestListIcoPressed" title="" runat="server"></a></td>
                                                            <td width="16px"><a id="icoTypeVacationsOrPermissions" href="javascript: void(0)" class="RequestListIcoTypeVacationsOrPermissions RequestListIcoPressed" title="" runat="server"></a></td>
                                                            <td width="16px"><a id="icoTypeCancelHolidays" href="javascript: void(0)" class="RequestListIcoTypeCancelHolidays RequestListIcoPressed" title="" runat="server"></a></td>
                                                            <td width="16px"><a id="icoTypePlannedAbsences" href="javascript: void(0)" class="RequestListIcoTypePlannedAbsences RequestListIcoPressed" title="" runat="server"></a></td>
                                                            <td width="16px"><a id="icoTypePlannedHolidays" href="javascript: void(0)" class="RequestListIcoTypePlannedHolidays RequestListIcoPressed" title="" runat="server"></a></td>
                                                            <td width="16px"><a id="icoTypePlannedOvertimes" href="javascript: void(0)" class="RequestListIcoTypePlannedOvertimes RequestListIcoPressed" title="" runat="server"></a></td>
                                                            <td width="16px"><a id="icoTypeTelecommute" href="javascript: void(0)" class="RequestListIcoTypeTelecommute RequestListIcoPressed" title="" runat="server"></a></td>
                                                            <td width="16px"><a id="icoTypeDailyRecord" href="javascript: void(0)" class="RequestListIcoTypeDailyRecord RequestListIcoPressed" title="" runat="server"></a></td>
                                                            <td width="16px"><a id="icoTypeExchangeShiftBetweenEmployees" href="javascript: void(0)" class="RequestListIcoTypeExchangeShiftBetweenEmployees RequestListIcoPressed" title="" runat="server"></a></td>
                                                            <td width="16px"><a id="icoTypePlannedCauses" href="javascript: void(0)" class="RequestListIcoTypePlannedCauses RequestListIcoPressed" title="" runat="server"></a></td>
                                                            <td width="16px"><a id="icoTypeForbiddenTaskPunch" href="javascript: void(0)" class="RequestListIcoTypeForbiddenTaskPunch RequestListIcoPressed" title="" runat="server"></a></td>
                                                            <td width="16px"><a id="icoTypeForgottenCostCenterPunch" href="javascript: void(0)" class="RequestListIcoTypeForgottenCostCenterPunch RequestListIcoPressed" title="" runat="server"></a></td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>

                                            <tr>
                                                <td style="padding-top: 10px;">
                                                    <table border="0">
                                                        <tr>
                                                            <td style="width: 70px;">
                                                                <asp:Label ID="lblSupervisor" runat="server" Text="Supervisor:"></asp:Label>
                                                            </td>
                                                            <td>
                                                                <dx:ASPxComboBox ID="cmbSupervisor" runat="server" Width="250px" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>

                                            <tr>
                                                <td style="padding-top: 10px;">
                                                    <table border="0">
                                                        <tr>
                                                            <td style="width: 70px;">
                                                                <asp:Label ID="lblFEmployees" runat="server" Text="Empleados:"></asp:Label>
                                                            </td>
                                                            <td style="width: 200px; border: solid 1px #CCCCCC; display: block;" class="defaultBackgroundColor">
                                                                <a href="javascript: void(0)" id="aFEmployees" runat="server" class="btnDDownMode"></a>
                                                                <div id="divFloatMenuE" class="AdvancedFilterEmployeeFloating" runat="server" style="display: none;">
                                                                    <table border="0" style="">
                                                                        <tr>
                                                                            <td nowrap="nowrap">
                                                                                <a href="javascript: void(0)" id="aEmpAll" runat="server" class="btnMode" style="width: 100%;">
                                                                                    <asp:Label ID="lblAllEmp" runat="server" Text="Todos los empleados"></asp:Label>
                                                                                </a>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td nowrap="nowrap">
                                                                                <a href="javascript: void(0)" id="aEmpSelect" runat="server" class="btnMode" style="width: 100%;">
                                                                                    <asp:Label ID="lblEmpSelect" runat="server" Text="Seleccionar.."></asp:Label>
                                                                                </a>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-top: 10px;">
                                                    <table border="0">
                                                        <tr>
                                                            <td style="width: 70px;">
                                                                <asp:Label ID="lblJustification" runat="server" Text="Justificación:"></asp:Label>
                                                            </td>
                                                            <td>
                                                                <dx:ASPxComboBox ID="cmbCause" runat="server" Width="250px" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" style="padding: 5px;">
                                    <a href="javascript: void(0);" id="btnApply" runat="server"></a>
                                    &nbsp;|&nbsp;
                                    <a href="javascript: void(0);" id="btnCancel" runat="server"></a>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <input id="chkSaveFilter" runat="server" type="checkbox" /><asp:Label ID="lblSaveFilter" runat="server" Text="Guardar filtro" Style="vertical-align: top;"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
            </td>
            <td width="16px">
                <a href="javascript: void(0)" class="icoRefresh icoClass" id="btnRefresh" runat="server"></a>
            </td>
        </tr>
    </table>

    <table runat="server" id="mode2Inf" cellpadding="0" cellspacing="0" width="100%" style="padding-top: 5px; display: none">
        <tr>
            <td>
                <table cellpadding="0" cellspacing="0">
                    <tr>
                        <td style="padding-left: 5px;" id="divSecurityV3Hidden" runat="server">
                            <roWebControls:roComboBox ID="OtherRequests_cmbLevels" runat="server" EnableViewState="true" AutoResizeChildsWidth="True" ParentWidth="10px" ChildsVisible="10" ItemsRunAtServer="false" HiddenText="OtherRequests_cmbLevels_Text" HiddenValue="OtherRequests_cmbLevels_Value"></roWebControls:roComboBox>
                            <input type="hidden" id="OtherRequests_cmbLevels_Text" runat="server" />
                            <input type="hidden" id="OtherRequests_cmbLevels_Value" runat="server" />
                        </td>
                        <td style="padding-left: 5px;" id="lblSecurityV3Hidden" runat="server">
                            <asp:Label ID="OtherRequests_lblLevels" Text="niveles por debajo." runat="server"></asp:Label>
                        </td>
                        <td style="padding-left: 15px;">
                            <input type="text" id="OtherRequests_txtDaysFrom" value="10" runat="server" class="textClass" convertcontrol="NumberField" ccallowblank="true" ccallowdecimals="false" style="width: 25px;" />
                        </td>
                        <td style="padding-left: 5px;">
                            <asp:Label ID="OtherRequests_lblDaysFrom" Text="días de antigüedad." runat="server"></asp:Label>
                        </td>
                    </tr>
                </table>
            </td>
            <td align="right">&nbsp;
            </td>
        </tr>
    </table>
</div>