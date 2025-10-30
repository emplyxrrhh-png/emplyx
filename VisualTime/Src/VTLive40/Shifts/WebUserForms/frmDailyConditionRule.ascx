<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.WebUserForms_frmDailyConditionRule" CodeBehind="frmDailyConditionRule.ascx.vb" %>

<div style="">
    <table style="width: 100%; padding-top: 5px;" border="0">
        <tr>
            <td>
                <div style="float: left">
                    <div id="divListConditions" style="background-color: #E8EEF7; width: 100%; height: 25px; text-align: right; vertical-align: middle;" runat="server">
                        <table border="0" style="width: 100%;">
                            <tr>
                                <td style="padding-left: 5px;">
                                    <asp:Label ID="lblCauses" runat="server" Text="Justificaciones"></asp:Label>
                                </td>
                                <td align="right">
                                    <div style="padding-top: 5px; padding-right: 10px;">
                                        <img id="imgAddListValue" src="" visible="true" runat="server" title='<%# Me.Language.Translate("addListValue",Me.DefaultScope) %>' style="cursor: pointer;" />
                                        <img id="imgRemoveListValue" src="" visible="true" runat="server" title='<%# Me.Language.Translate("delListValue",Me.DefaultScope) %>' style="cursor: pointer;" />
                                    </div>
                                    <div style="position: absolute; z-index: 15010;">
                                        <!-- ModalPopup para Nueva Causa -->
                                        <div id="divNewCauseConditions_<%= Me.Instance %>" style="position: absolute; width: 400px; z-index: 15050; display: none;">
                                            <div class="bodyPopupExtended" style="">
                                                <table width="100%" cellspacing="0" border="0">
                                                    <tr id="panNewCauseDragHandle" style="height: 20px;">
                                                        <td align="center"></td>
                                                        <td style="padding-left: 10px;">
                                                            <asp:Label ID="lblNewCauseTitle" Text="Nueva justificación" Font-Bold="true" ForeColor="#485A6B" runat="server" />
                                                        </td>
                                                        <td align="right">
                                                            <img id="ibtNewCauseOK" runat="server" src="" style="cursor: pointer;" />
                                                            <img id="ibtNewCauseCancel" style="cursor: pointer;" src="" runat="server" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="3" style="padding-left: 10px; padding-top: 5px; vertical-align: middle">
                                                            <table style="width: 100%;" border="0">
                                                                <tr>
                                                                    <td colspan="2" style="padding-bottom: 2px;">
                                                                        <asp:Label ID="lblCauseTitle2" CssClass="spanEmp-Class" runat="server" Text="Seleccione la justificación" />
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td colspan="2" style="padding-bottom: 5px; text-align: center; padding-left: 10px;">
                                                                        <dx:ASPxComboBox ID="cmbCauseAdd" runat="server" Width="300px" />
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td valign="bottom" style="height: 19px;">
                                                                        <input id="opPlus_<%= Me.Instance %>" type="radio" name="optPlusMinus_<%= Me.Instance %>" value="0" checked="true" />&nbsp;
                                                                        <a href="javascript: void(0);" onclick="document.getElementById('opPlus_<%= Me.Instance %>').checked=true;">
                                                                            <asp:Label ID="lblopPlus" runat="server" Text="Se sumará la justificación"></asp:Label>
                                                                        </a>
                                                                    </td>
                                                                    <td valign="bottom" style="height: 19px;">
                                                                        <input id="opMinus_<%= Me.Instance %>" type="radio" name="optPlusMinus_<%= Me.Instance %>" value="1" />&nbsp;
                                                                        <a href="javascript: void(0);" onclick="document.getElementById('opMinus_<%= Me.Instance %>').checked=true;">
                                                                            <asp:Label ID="lblopMinus" runat="server" Text="Se restará la justificación"></asp:Label>
                                                                        </a>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>
                                        </div>
                                        <!-- Fin ModalPopup -->
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <input type="hidden" id="selectedIdx_<%= Me.Instance %>" value="" />
                                    <div id="divConditionsCauses_<%= Me.Instance %>" style="width: 240px; height: 100px; display: block; border: solid 1px silver; overflow: auto;">
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>

                <div style="float: left">
                    <div id="divTimeZones" style="background-color: #E8EEF7; width: 100%; height: 25px; text-align: right; vertical-align: middle;" runat="server">
                        <table border="0" style="width: 100%;">
                            <tr>
                                <td style="padding-left: 5px;">
                                    <asp:Label ID="lblTimezones" runat="server" Text="Zonas horarias"></asp:Label>
                                </td>
                                <td align="right">
                                    <div style="padding-top: 5px; padding-right: 10px;">
                                        <img id="imgAddTimeZoneValue" src="" visible="true" runat="server" title='<%# Me.Language.Translate("addTimezoneValue", Me.DefaultScope) %>' style="cursor: pointer;" />
                                        <img id="imgRemoveTimeZoneValue" src="" visible="true" runat="server" title='<%# Me.Language.Translate("delTimeZoneValue", Me.DefaultScope) %>' style="cursor: pointer;" />
                                    </div>
                                    <div style="position: absolute; z-index: 15010;">
                                        <!-- ModalPopup para Nueva Causa -->
                                        <div id="divNewTimeZone_<%= Me.Instance %>" style="position: absolute; width: 400px; z-index: 15050; display: none;">
                                            <div class="bodyPopupExtended" style="">
                                                <table width="100%" cellspacing="0" border="0">
                                                    <tr id="panNewTimezoneDragHandle" style="height: 20px;">
                                                        <td align="center"></td>
                                                        <td style="padding-left: 10px;">
                                                            <asp:Label ID="Label2" Text="Nueva zona horaria" Font-Bold="true" ForeColor="#485A6B" runat="server" />
                                                        </td>
                                                        <td align="right">
                                                            <img id="ibtNewTimezoneOK" runat="server" src="" style="cursor: pointer;" />
                                                            <img id="ibtNewTimezoneCancel" style="cursor: pointer;" src="" runat="server" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="3" style="padding-left: 10px; padding-top: 5px; vertical-align: middle">
                                                            <table style="width: 100%;" border="0">
                                                                <tr>
                                                                    <td colspan="2" style="padding-bottom: 2px;">
                                                                        <asp:Label ID="lblTimeZoneTitle" CssClass="spanEmp-Class" runat="server" Text="Seleccione la zona horaria" />
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td colspan="2" style="padding-bottom: 5px; text-align: center; padding-left: 10px;">
                                                                        <dx:ASPxComboBox ID="cmbTimeZoneAdd" runat="server" Width="300px" />
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>
                                        </div>
                                        <!-- Fin ModalPopup -->
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <input type="hidden" id="selectedIdxTimeZone_<%= Me.Instance %>" value="" />
                                    <div id="divConditionTimeZones_<%= Me.Instance %>" style="width: 160px; height: 100px; display: block; border: solid 1px silver; overflow: auto;">
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>

                <div style="float: left">
                    <div id="divCompare" style="background-color: #E8EEF7; width: 100%; height: 25px; text-align: right; vertical-align: middle;" runat="server">
                        <table cellpadding="0" cellspacing="0">
                            <tr>
                                <td>
                                    <div style="background-color: #E8EEF7; width: 100%; height: 25px; text-align: right; vertical-align: middle;">
                                        <table border="0" style="width: 100%;">
                                            <tr>
                                                <td style="padding-left: 5px; padding-top: 5px;">
                                                    <asp:Label ID="lblCompares" runat="server" Text="Comparación"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <dx:ASPxComboBox ID="cmbCompare" runat="server" Width="150px" />
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>

                <div style="float: left">
                    <div id="divType" style="background-color: #E8EEF7; width: 100%; height: 25px; text-align: right; vertical-align: middle;" runat="server">
                        <table cellpadding="0" cellspacing="0">
                            <tr>
                                <td>
                                    <div style="background-color: #E8EEF7; width: 100%; height: 25px; text-align: right; vertical-align: middle;">
                                        <table border="0" style="width: 100%;">
                                            <tr>
                                                <td style="padding-left: 5px; padding-top: 5px;">
                                                    <asp:Label ID="lblTypeValues" runat="server" Text="Tipo"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <dx:ASPxComboBox ID="cmbTypeValue" runat="server" Width="150px" />
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
                <div style="float: left;">
                    <div style="float: left; display: none" id="uniqueValue_<%= Me.Instance %>">
                        <div style="background-color: #E8EEF7; width: 100%; height: 25px; text-align: right; vertical-align: middle;">
                            <table cellpadding="0" cellspacing="0">
                                <tr>
                                    <td>
                                        <div style="background-color: #E8EEF7; width: 100%; height: 25px; text-align: right; vertical-align: middle;">
                                            <table border="0" style="width: 100%;">
                                                <tr>
                                                    <td style="padding-left: 5px; padding-top: 5px;">
                                                        <asp:Label ID="lblValues" runat="server" Text="Valor"></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right">
                                        <div id="directValue_<%=Me.Instance %>" style="float: left">
                                            <dx:ASPxTextBox ID="txtValueType" runat="server" Width="75px">
                                                <ValidationSettings ErrorDisplayMode="None" />
                                                <MaskSettings Mask="<-999..999>:<00..59>" />
                                            </dx:ASPxTextBox>
                                        </div>
                                        <div id="between_<%=Me.Instance %>" style="float: left; display: none">
                                            <div style="float: left">&nbsp;<asp:Label ID="lblTo" runat="server" Text="y"></asp:Label>&nbsp;</div>
                                            <div style="float: left">
                                                <dx:ASPxTextBox ID="txtValueTypeTo" runat="server" Width="75px">
                                                    <ValidationSettings ErrorDisplayMode="None" />
                                                    <MaskSettings Mask="<-999..999>:<00..59>" />
                                                </dx:ASPxTextBox>
                                            </div>
                                        </div>
                                        <div id="userField_<%=Me.Instance %>" style="float: left">
                                            <dx:ASPxComboBox ID="cmbCauseUField" runat="server" Width="150px" />
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>

                    <div style="float: left; display: none" id="causeValue_<%= Me.Instance %>">
                        <div style="background-color: #E8EEF7; width: 100%; height: 25px; text-align: right; vertical-align: middle;">
                            <table border="0" style="width: 100%;">
                                <tr>
                                    <td style="padding-left: 5px;">
                                        <asp:Label ID="lblValueCauses" runat="server" Text="Justificaciones"></asp:Label>
                                    </td>
                                    <td align="right">
                                        <div style="padding-top: 5px; padding-right: 10px;">
                                            <img id="imgAddValueCauseValue" src="" visible="true" runat="server" title='<%# Me.Language.Translate("addListValue", Me.DefaultScope) %>' style="cursor: pointer;" />
                                            <img id="imgRemoveValueCauseValue" src="" visible="true" runat="server" title='<%# Me.Language.Translate("delListValue", Me.DefaultScope) %>' style="cursor: pointer;" />
                                        </div>
                                        <div style="position: absolute; z-index: 15010;">
                                            <!-- ModalPopup para Nueva Causa -->
                                            <div id="divValueCauseConditions_<%= Me.Instance %>" style="position: absolute; width: 400px; z-index: 15050; display: none;">
                                                <div class="bodyPopupExtended" style="">
                                                    <table width="100%" cellspacing="0" border="0">
                                                        <tr id="panValueCauseDragHandle" style="height: 20px;">
                                                            <td align="center"></td>
                                                            <td style="padding-left: 10px;">
                                                                <asp:Label ID="lblCauseValueTitle" Text="Nueva justificación" Font-Bold="true" ForeColor="#485A6B" runat="server" />
                                                            </td>
                                                            <td align="right">
                                                                <img id="ibtValueCauseOK" runat="server" src="" style="cursor: pointer;" />
                                                                <img id="ibtValueCauseCancel" style="cursor: pointer;" src="" runat="server" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="3" style="padding-left: 10px; padding-top: 5px; vertical-align: middle">
                                                                <table style="width: 100%;" border="0">
                                                                    <tr>
                                                                        <td colspan="2" style="padding-bottom: 2px;">
                                                                            <asp:Label ID="lblCauseValueTitle2" CssClass="spanEmp-Class" runat="server" Text="Seleccione la justificación" />
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td colspan="2" style="padding-bottom: 5px; text-align: center; padding-left: 10px;">
                                                                            <dx:ASPxComboBox ID="cmbAddValueCause" runat="server" Width="300px" />
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td valign="bottom" style="height: 19px;">
                                                                            <input id="opPlusValue_<%= Me.Instance %>" type="radio" name="optPlusMinusValue_<%= Me.Instance %>" value="0" checked="true" />&nbsp;
                                                                            <a href="javascript: void(0);" onclick="document.getElementById('opPlusValue_<%= Me.Instance %>').checked=true;">
                                                                                <asp:Label ID="Label5" runat="server" Text="Se sumará la justificación"></asp:Label>
                                                                            </a>
                                                                        </td>
                                                                        <td valign="bottom" style="height: 19px;">
                                                                            <input id="opMinusValue_<%= Me.Instance %>" type="radio" name="optPlusMinusValue_<%= Me.Instance %>" value="1" />&nbsp;
                                                                            <a href="javascript: void(0);" onclick="document.getElementById('opMinusValue_<%= Me.Instance %>').checked=true;">
                                                                                <asp:Label ID="Label6" runat="server" Text="Se restará la justificación"></asp:Label>
                                                                            </a>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                            </div>
                                            <!-- Fin ModalPopup -->
                                        </div>
                                    </td>
                                    <td style="padding-left: 5px;">
                                        <asp:Label ID="lblTimezoneValue" runat="server" Text="Zonas horarias"></asp:Label>
                                    </td>
                                    <td align="right">
                                        <div style="padding-top: 5px; padding-right: 10px;">
                                            <img id="imgAddTimeZoneValueValue" src="" visible="true" runat="server" title='<%# Me.Language.Translate("addTimezoneValue", Me.DefaultScope) %>' style="cursor: pointer;" />
                                            <img id="imgRemoveTimeZoneValueValue" src="" visible="true" runat="server" title='<%# Me.Language.Translate("delTimeZoneValue", Me.DefaultScope) %>' style="cursor: pointer;" />
                                        </div>
                                        <div style="position: absolute; z-index: 15010;">
                                            <!-- ModalPopup para Nueva Causa -->
                                            <div id="divNewTimeZoneValue_<%= Me.Instance %>" style="position: absolute; width: 400px; z-index: 15050; display: none;">
                                                <div class="bodyPopupExtended" style="">
                                                    <table width="100%" cellspacing="0" border="0">
                                                        <tr id="panNewTimezoneValueDragHandle" style="height: 20px;">
                                                            <td align="center"></td>
                                                            <td style="padding-left: 10px;">
                                                                <asp:Label ID="lblTimeZoneValueTitle" Text="Nueva zona horaria" Font-Bold="true" ForeColor="#485A6B" runat="server" />
                                                            </td>
                                                            <td align="right">
                                                                <img id="ibtNewTimezoneValueOK" runat="server" src="" style="cursor: pointer;" />
                                                                <img id="ibtNewTimezoneValueCancel" style="cursor: pointer;" src="" runat="server" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="3" style="padding-left: 10px; padding-top: 5px; vertical-align: middle">
                                                                <table style="width: 100%;" border="0">
                                                                    <tr>
                                                                        <td colspan="2" style="padding-bottom: 2px;">
                                                                            <asp:Label ID="lblTimeZoneValueTitle2" CssClass="spanEmp-Class" runat="server" Text="Seleccione la zona horaria" />
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td colspan="2" style="padding-bottom: 5px; text-align: center; padding-left: 10px;">
                                                                            <dx:ASPxComboBox ID="cmbTimeZoneValueAdd" runat="server" Width="300px" />
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                            </div>
                                            <!-- Fin ModalPopup -->
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <input type="hidden" id="selectedIdxValue_<%= Me.Instance %>" value="" />
                                        <div id="divConditionsCausesValue_<%= Me.Instance %>" style="width: 240px; height: 100px; display: block; border: solid 1px silver; overflow: auto;">
                                        </div>
                                    </td>
                                    <td colspan="2">
                                        <input type="hidden" id="selectedIdxTimeZoneValue_<%= Me.Instance %>" value="" />
                                        <div id="divConditionTimeZonesValue_<%= Me.Instance %>" style="width: 160px; height: 100px; display: block; border: solid 1px silver; overflow: auto;">
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>
                </div>
            </td>
        </tr>
    </table>
</div>