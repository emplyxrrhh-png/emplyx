<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.WebUserForms_frmDailyActionRule" CodeBehind="frmDailyActionRule.ascx.vb" %>

<div style="">
    <div style="width: 100%; height: 100%; background-color: White;" class="bodyPopup">
        <table style="width: 100%; padding-top: 5px;" border="0">
            <tr>
                <td>
                    <div style="float: left; padding-right: 10px;">
                        <div id="divCompare" style="width: 100%; height: 25px; text-align: right; vertical-align: middle;" runat="server">
                            <table cellpadding="0" cellspacing="0">
                                <tr>
                                    <td>
                                        <dx:ASPxComboBox ID="cmbActions" runat="server" Width="150px" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>

                    <div id="carryover_<%=Me.Instance %>" style="float: left; padding-right: 5px; display: none">
                        <div id="carryOverStep0_<%=Me.Instance %>" style="float: left">
                            <div style="width: 100%; height: 25px; text-align: right; vertical-align: middle;" runat="server">
                                <table cellpadding="0" cellspacing="0">
                                    <tr id="carryOverAction_<%=Me.Instance %>">
                                        <td>
                                            <div style="float: left; padding-bottom: 5px;">
                                                <dx:ASPxComboBox ID="cmbCarryOverAction" runat="server" Width="250px" />
                                            </div>
                                        </td>
                                    </tr>
                                    <tr id="carryOverActionResult_<%=Me.Instance %>">
                                        <td>
                                            <div style="float: left; padding-bottom: 5px;">
                                                <dx:ASPxComboBox ID="cmbCarryOverActionResult" runat="server" Width="250px" />
                                            </div>
                                        </td>
                                    </tr>

                                    <tr id="carryOverCauseFrom_<%=Me.Instance %>">
                                        <td>
                                            <div style="float: left">
                                                <dx:ASPxComboBox ID="cmbCarryOverCauseFrom" runat="server" Width="250px" />
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div>

                        <div id="carryOverStep1_<%=Me.Instance %>" style="float: left">
                            <div style="width: 100%; height: 25px; text-align: right; vertical-align: middle;" runat="server">
                                <table cellpadding="0" cellspacing="0">
                                    <tr id="carryOver2Value_<%=Me.Instance %>">
                                        <td>
                                            <div id="CarryOverDirectValue_<%=Me.Instance %>" style="float: left; padding-bottom: 4px;">
                                                <dx:ASPxTextBox ID="txtCarryOverValue" runat="server" Width="150px">
                                                    <ValidationSettings ErrorDisplayMode="None" />
                                                    <MaskSettings Mask="<-999..999>:<00..59>" />
                                                </dx:ASPxTextBox>
                                            </div>

                                            <div id="CarryOverDifference_<%=Me.Instance %>" style="float: left; padding-bottom: 5px;">
                                                <div style="float: left">
                                                    <dx:ASPxComboBox ID="cmbCarryOverConditionPart" runat="server" Width="150px" />
                                                </div>
                                                <div style="float: left">
                                                    <dx:ASPxComboBox ID="cmbCarryOverConditionNumber" runat="server" Width="150px" />
                                                </div>
                                            </div>

                                            <div id="CarryOverUserField_<%=Me.Instance %>" style="float: left; padding-bottom: 5px;">
                                                <dx:ASPxComboBox ID="cmbCauseUFieldCarryOver" runat="server" Width="150px" />
                                            </div>
                                        </td>
                                    </tr>
                                    <tr id="carryOver2ValueResult_<%=Me.Instance %>">
                                        <td>
                                            <div id="CarryOverDirectValueResult_<%=Me.Instance %>" style="float: left; padding-bottom: 4px;">
                                                <dx:ASPxTextBox ID="txtCarryOverResultValue" runat="server" Width="150px">
                                                    <ValidationSettings ErrorDisplayMode="None" />
                                                    <MaskSettings Mask="<-999..999>:<00..59>" />
                                                </dx:ASPxTextBox>
                                            </div>
                                            <div id="CarryOverDifferenceResult_<%=Me.Instance %>" style="float: left; padding-bottom: 5px;">
                                                <div style="float: left">
                                                    <dx:ASPxComboBox ID="cmbCarryOverConditionPartResult" runat="server" Width="150px" />
                                                </div>
                                                <div style="float: left">
                                                    <dx:ASPxComboBox ID="cmbCarryOverConditionNumberResult" runat="server" Width="150px" />
                                                </div>
                                            </div>
                                            <div id="CarryOverUserFieldResult_<%=Me.Instance %>" style="float: left; padding-bottom: 5px;">
                                                <dx:ASPxComboBox ID="cmbCauseUFieldCarryOverResult" runat="server" Width="150px" />
                                            </div>
                                        </td>
                                    </tr>
                                    <tr id="carryOverCauseTo_<%=Me.Instance %>">
                                        <td>
                                            <div style="float: left">
                                                <dx:ASPxComboBox ID="cmbCarryOverCauseTo" runat="server" Width="250px" />
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                    </div>

                    <div id="plus_<%=Me.Instance %>" style="float: left; display: none" class="plusConfig">
                        <div id="plusStep0_<%=Me.Instance %>" style="padding-right: 10px; float: left">
                            <div style="width: 100%; height: 25px; text-align: right; vertical-align: middle;" runat="server">
                                <table cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td>
                                            <dx:ASPxComboBox ID="cmbPlusCause" runat="server" Width="250px" />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div>

                        <div id="plusStep1_<%=Me.Instance %>" style="float: left">
                            <div style="width: 100%; height: 25px; text-align: right; vertical-align: middle;" runat="server">
                                <table cellpadding="0" cellspacing="0">
                                    <tr id="plusStep1PlusAction<%=Me.Instance %>">
                                        <td>
                                            <div style="float: left; padding-bottom: 5px">
                                                <dx:ASPxComboBox ID="cmbPlusActions" runat="server" Width="220px" />
                                            </div>
                                        </td>
                                    </tr>
                                    <tr id="plusStep1PlusActionResult<%=Me.Instance %>">
                                        <td>
                                            <div style="float: left; padding-bottom: 5px">
                                                <dx:ASPxComboBox ID="cmbPlusResultActions" runat="server" Width="220px" />
                                            </div>
                                        </td>
                                    </tr>
                                    <tr id="plusStep1Sign<%=Me.Instance %>">
                                        <td>
                                            <dx:ASPxComboBox ID="cmbPlusSign" runat="server" Width="220px" />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div>

                        <div id="plusStep2_<%=Me.Instance %>" style="float: left">
                            <div style="width: 100%; height: 25px; text-align: right; vertical-align: middle;" runat="server">
                                <table cellpadding="0" cellspacing="0">
                                    <tr id="plusStep2PlusValue_<%=Me.Instance %>">
                                        <td>
                                            <div id="ActionDirectValue_<%=Me.Instance %>" style="float: left; padding-bottom: 4px">
                                                <div id="ActionDirectValue_Time_<%=Me.Instance %>">
                                                    <dx:ASPxTextBox ID="txtPlusValueTime" runat="server" Width="150px">
                                                        <ValidationSettings ErrorDisplayMode="None" />
                                                        <MaskSettings Mask="<-999..999>:<00..59>" />
                                                    </dx:ASPxTextBox>
                                                </div>
                                                <div id="ActionDirectValue_Number_<%=Me.Instance %>" style="display: none">
                                                    <dx:ASPxTextBox ID="txtPlusValueNumber" runat="server" Width="150px">
                                                        <ValidationSettings ErrorDisplayMode="None" />
                                                        <MaskSettings Mask="<-999..999>.<000..999>" />
                                                    </dx:ASPxTextBox>
                                                </div>
                                            </div>

                                            <div id="ActionDifference_<%=Me.Instance %>" style="float: left; padding-bottom: 5px">
                                                <div style="float: left">
                                                    <dx:ASPxComboBox ID="cmbPlusConditionPart" runat="server" Width="150px" />
                                                </div>
                                                <div style="float: left">
                                                    <dx:ASPxComboBox ID="cmbPlusConditionNumber" runat="server" Width="150px" />
                                                </div>
                                            </div>

                                            <div id="ActionUserField_<%=Me.Instance %>" style="float: left; padding-bottom: 5px">
                                                <dx:ASPxComboBox ID="cmbCauseUFieldPlus" runat="server" Width="150px" />
                                            </div>
                                        </td>
                                    </tr>
                                    <tr id="plusStep2PlusValueResult_<%=Me.Instance %>">
                                        <td>
                                            <div id="ActionDirectValueResult_<%=Me.Instance %>" style="float: left; padding-bottom: 4px">
                                                <div id="ActionDirectValueResult_Time_<%=Me.Instance %>" style="">
                                                    <dx:ASPxTextBox ID="txtPlusValueResultTime" runat="server" Width="150px">
                                                        <ValidationSettings ErrorDisplayMode="None" />
                                                        <MaskSettings Mask="<-999..999>:<00..59>" />
                                                    </dx:ASPxTextBox>
                                                </div>
                                                <div id="ActionDirectValueResult_Number_<%=Me.Instance %>" style="display: none">
                                                    <dx:ASPxTextBox ID="txtPlusValueResultNumber" runat="server" Width="150px">
                                                        <ValidationSettings ErrorDisplayMode="None" />
                                                        <MaskSettings Mask="<-999..999>.<000..999>" />
                                                    </dx:ASPxTextBox>
                                                </div>
                                            </div>
                                            <div id="ActionDifferenceResult_<%=Me.Instance %>" style="float: left; padding-bottom: 5px">
                                                <div style="float: left">
                                                    <dx:ASPxComboBox ID="cmbPlusConditionPartResult" runat="server" Width="150px" />
                                                </div>
                                                <div style="float: left">
                                                    <dx:ASPxComboBox ID="cmbPlusConditionNumberResult" runat="server" Width="150px" />
                                                </div>
                                            </div>
                                            <div id="ActionUserFieldResult_<%=Me.Instance %>" style="float: left; padding-bottom: 5px">
                                                <dx:ASPxComboBox ID="cmbCauseUFieldPlusResult" runat="server" Width="150px" />
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                    </div>

                    <div id="carryoversingle_<%=Me.Instance %>" style="float: left; padding-right: 5px; display: none">
                        <div id="carryOverSingleStep0_<%=Me.Instance %>" style="float: left">
                            <div style="width: 100%; height: 25px; text-align: right; vertical-align: middle;" runat="server">
                                <table cellpadding="0" cellspacing="0">
                                    <tr id="carryOverSingleAction_<%=Me.Instance %>">
                                        <td>
                                            <div style="float: left; padding-bottom: 5px;">
                                                <dx:ASPxComboBox ID="cmbCarryOverSingleAction" runat="server" Width="250px" />
                                            </div>
                                        </td>
                                        <td>
                                            <div style="float: left; padding-bottom: 5px; margin-left: 10px;">
                                                <dx:ASPxComboBox ID="cmbCarryOverSingleActionCauses" runat="server" Width="250px" />
                                            </div>
                                        </td>
                                        <td style="float: left; padding-bottom: 5px; margin-left: 10px; width: 400px;">

                                            <div style="float: left">
                                                <div id="divListActions" style="width: 100%; height: 25px; text-align: right; vertical-align: middle;" runat="server">
                                                    <table border="0">

                                                        <tr>
                                                            <td>
                                                                <input type="hidden" id="selectedIdx_<%= Me.Instance %>" value="" />
                                                                <div id="divActionsCauses_<%= Me.Instance %>" style="width: 425px; height: 94px; display: block; border: solid 1px silver; overflow: auto;">
                                                                </div>
                                                            </td>
                                                            <td align="right">
                                                                <div style="padding-bottom: 60px; padding-right: 10px;">
                                                                    <img id="imgAddListValueAction" src="" visible="true" runat="server" title='<%# Me.Language.Translate("addListValue",Me.DefaultScope) %>' style="cursor: pointer;" />
                                                                    <img id="imgRemoveListValueAction" src="" visible="true" runat="server" title='<%# Me.Language.Translate("delListValue",Me.DefaultScope) %>' style="cursor: pointer;" />
                                                                </div>
                                                                <div style="position: absolute; z-index: 15010; bottom: 0; right: 0; margin-bottom: 15px; margin-right: 15px;">
                                                                    <!-- ModalPopup para Nueva Causa -->
                                                                    <div id="divNewCauseActions_<%= Me.Instance %>" style="width: 400px; z-index: 15050; display: none;">
                                                                        <div class="bodyPopupExtended" style="">
                                                                            <table width="100%" cellspacing="0" border="0">
                                                                                <tr id="panNewCauseDragHandle" style="height: 20px;">
                                                                                    <td align="center"></td>
                                                                                    <td style="padding-left: 10px;">
                                                                                        <asp:Label ID="lblNewCauseTitle" Text="Nueva justificación" Font-Bold="true" ForeColor="#485A6B" runat="server" />
                                                                                    </td>
                                                                                    <td align="right">
                                                                                        <img id="ibtNewCauseActionOK" runat="server" src="" style="cursor: pointer;" />
                                                                                        <img id="ibtNewCauseActionCancel" style="cursor: pointer;" src="" runat="server" />
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td colspan="3" style="padding-left: 10px; padding-top: 5px; vertical-align: middle">
                                                                                        <table style="width: 100%;" border="0">
                                                                                            <tr>
                                                                                                <td colspan="2" style="padding-bottom: 2px;">
                                                                                                    <asp:Label ID="lblCauseTitleFrom2" CssClass="spanEmp-Class" runat="server" Text="De:" />
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td colspan="2" style="padding-bottom: 5px; text-align: center; padding-left: 10px;">
                                                                                                    <dx:ASPxComboBox ID="cmbCauseActionAdd" runat="server" Width="300px" />
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td colspan="2" style="padding-bottom: 2px;">
                                                                                                    <asp:Label ID="lblCauseTitleTo2" CssClass="spanEmp-Class" runat="server" Text="A:" />
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td colspan="2" style="padding-bottom: 5px; text-align: center; padding-left: 10px;">
                                                                                                    <dx:ASPxComboBox ID="cmbCauseActionAdd2" runat="server" Width="300px" />
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
                                                    </table>
                                                </div>
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
</div>
<!-- End Div flotant AddZone -->