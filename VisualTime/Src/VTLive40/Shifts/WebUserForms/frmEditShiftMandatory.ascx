<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.WebUserForms_frmEditShiftMandatory" CodeBehind="frmEditShiftMandatory.ascx.vb" %>

<input type="hidden" id="hdnMandatoryLayerID" />
<input type="hidden" id="hdnMandatoryParentID" />
<input type="hidden" id="hdnMandatory1020LayerID" />
<input type="hidden" id="hdnMandatory1020ParentID" />
<input type="hidden" id="hdnMandatory1021LayerID" />
<input type="hidden" id="hdnMandatory1021ParentID" />
<input type="hidden" id="hdnMandatory1022LayerID" />
<input type="hidden" id="hdnMandatory1022ParentID" />

<!-- Div flotant EditShiftFlexible -->
<input type="hidden" id="<%= Me.ClientID %>_hdnRuleChanges" value="0" />
<script type="text/javascript">
    function StartSelSelectedIndexChanged(s) {
        if (s.GetSelectedIndex() == 0) {
            document.getElementById('panEntrance1').style.display = '';
            document.getElementById('panEntrance2').style.display = 'none';
            cmbEndSelMandatoryClient.SetValue(0);
            document.getElementById('panExit1').style.display = '';
            document.getElementById('panExit2').style.display = 'none';
            cmbEndSelMandatoryClient.SetEnabled(true);

        } else {
            document.getElementById('panEntrance1').style.display = 'none';
            document.getElementById('panEntrance2').style.display = '';
            cmbEndSelMandatoryClient.SetEnabled(true);

        }
    }

    function EndSelSelectedIndexChanged(s) {
        if (s.GetSelectedIndex() == 0) {
            document.getElementById('panExit1').style.display = '';
            document.getElementById('panExit2').style.display = 'none';
        } else {
            document.getElementById('panExit1').style.display = 'none';
            document.getElementById('panExit2').style.display = '';
        }
    }
</script>
<div id="<%= Me.ClientID %>_frm" style="position: fixed; *position: absolute; z-index: 9010; display: none; top: 50%; left: 50%; *width: 900px;">

    <div id="<%= Me.ClientID %>_BgS" style="position: absolute; top: 0; left: 0; display: none; z-index: 9009;"></div>
    <div class="bodyPopupExtended" style="">
        <div style="">
            <div style="width: 100%; height: 100%; background-color: White;" class="bodyPopup">
                <table style="width: 100%; padding-top: 5px;" border="0">
                    <tr>
                        <td colspan="2">
                            <div class="panHeader2">
                                <span style="">
                                    <asp:Label runat="server" ID="lblEditShiftMandatory" Text="Editar horario rigido"></asp:Label></span>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" style="padding: 2px;">
                            <table border="0" style="width: 520px;">
                                <tr>
                                    <td style="padding-top: 5px;">
                                        <roUserControls:roTabContainerClient ID="tbCont1" runat="server">
                                            <TabTitle1>
                                                <asp:Label ID="lblTitInterval" runat="server" Text="Intervalo"></asp:Label>
                                            </TabTitle1>
                                            <TabContainer1>
                                                <table style="margin: 5px; height: 80px;" border="0">
                                                    <tr>
                                                        <td valign="top">
                                                            <table border="0" cellpadding="1" cellspacing="1">
                                                                <tr>
                                                                    <td valign="top">
                                                                        <asp:Label ID="lblStart" runat="server" Text="Entrada:" CssClass="spanEmp-class"></asp:Label>
                                                                    </td>
                                                                    <td valign="top">
                                                                        <dx:ASPxComboBox runat="server" ID="cmbStartSel" Width="175px" ClientInstanceName="cmbStartSelMandatoryClient">
                                                                            <ClientSideEvents SelectedIndexChanged="function(s,e){ StartSelSelectedIndexChanged(s);ShowAdvancedCheckIni(s); }" />
                                                                        </dx:ASPxComboBox>
                                                                    </td>
                                                                    <td valign="top">
                                                                        <div id="panEntrance1" style="width: 100%; display: ;">
                                                                            <table border="0" cellpadding="1" cellspacing="1">
                                                                                <tr>
                                                                                    <td>
                                                                                        <asp:Label ID="lblStartAtA1" runat="server" CssClass="spanEmp-class" Text="A las"></asp:Label></td>
                                                                                    <td style="width: 75px;">
                                                                                        <dx:ASPxTimeEdit ID="txtStartAt1" runat="server" Width="85px" EditFormatString="HH:mm" EditFormat="Custom" ClientInstanceName="txtStartAt1MandatoryClient">
                                                                                            <ClientSideEvents DateChanged="function(s,e){}" />
                                                                                        </dx:ASPxTimeEdit>
                                                                                    </td>
                                                                                    <td>
                                                                                        <dx:ASPxComboBox runat="server" ID="cmbStartAt1" Width="175px" ClientInstanceName="cmbStartAt1MandatoryClient">
                                                                                            <ClientSideEvents SelectedIndexChanged="function(s,e){}" />
                                                                                        </dx:ASPxComboBox>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </div>
                                                                        <div id="panEntrance2" style="width: 100%; display: none;">
                                                                            <table border="0" cellpadding="1" cellspacing="1">
                                                                                <tr>
                                                                                    <td valign="middle">
                                                                                        <asp:Label ID="lblStartAtA2From" runat="server" CssClass="spanEmp-class" Text="Entre las"></asp:Label></td>
                                                                                    <td style="width: 75px;">
                                                                                        <dx:ASPxTimeEdit ID="txtStartAt2From" runat="server" Width="85px" EditFormatString="HH:mm" EditFormat="Custom" ClientInstanceName="txtStartAt2FromMandatoryClient">
                                                                                            <ClientSideEvents DateChanged="function(s,e){}" />
                                                                                        </dx:ASPxTimeEdit>
                                                                                    </td>
                                                                                    <td>
                                                                                        <dx:ASPxComboBox runat="server" ID="cmbStartAt2AFrom" Width="175px" ClientInstanceName="cmbStartAt2AFromMandatoryClient">
                                                                                            <ClientSideEvents SelectedIndexChanged="function(s,e){ EndSelSelectedIndexChanged(s); }" />
                                                                                        </dx:ASPxComboBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td valign="middle">
                                                                                        <asp:Label ID="lblStartAtA2To" runat="server" CssClass="spanEmp-class" Text="y las"></asp:Label></td>
                                                                                    <td style="width: 75px;">
                                                                                        <dx:ASPxTimeEdit ID="txtStartAt2To" runat="server" Width="85px" EditFormatString="HH:mm" EditFormat="Custom" ClientInstanceName="txtStartAt2ToMandatoryClient">
                                                                                            <ClientSideEvents DateChanged="function(s,e){}" />
                                                                                        </dx:ASPxTimeEdit>
                                                                                    </td>
                                                                                    <td>
                                                                                        <dx:ASPxComboBox runat="server" ID="cmbStartAt2ATo" Width="175px" ClientInstanceName="cmbStartAt2AToMandatoryClient">
                                                                                            <ClientSideEvents SelectedIndexChanged="function(s,e){}" />
                                                                                        </dx:ASPxComboBox>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </div>
                                                                    </td>
                                                                </tr>
                                                                <tr style="width: 100%; display: none;" id="trChkmodifyini">
                                                                    <td style="vertical-align: text-top;">
                                                                        <dx:ASPxCheckBox ID="chkModiIni" runat="server" ClientInstanceName="chkmodifyini" Checked="false">
                                                                            <ClientSideEvents CheckedChanged="function(s, e) {  }" />
                                                                        </dx:ASPxCheckBox>
                                                                    </td>
                                                                    <td style="vertical-align: text-top;" colspan="2">
                                                                        <asp:Label ID="lblDateIni" runat="server" Text="La hora de inicio de la franja podrá ser modificar al planificar el horario" CssClass="spanEmp-class"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td valign="top">
                                                                        <asp:Label ID="lblEnd" runat="server" Text="Salida:" CssClass="spanEmp-class"></asp:Label>
                                                                    </td>
                                                                    <td valign="top">
                                                                        <dx:ASPxComboBox runat="server" ID="cmbEndSel" Width="175px" ClientInstanceName="cmbEndSelMandatoryClient">
                                                                            <ClientSideEvents SelectedIndexChanged="function(s,e){EndSelSelectedIndexChanged(s);ShowAdvancedCheckEnd(s);}" />
                                                                        </dx:ASPxComboBox>
                                                                    </td>
                                                                    <td valign="top">
                                                                        <div id="panExit1" style="width: 100%; display: ;">
                                                                            <table border="0" cellpadding="1" cellspacing="1">
                                                                                <tr>
                                                                                    <td>
                                                                                        <asp:Label ID="lblEndAtA1" runat="server" CssClass="spanEmp-class" Text="A las"></asp:Label></td>
                                                                                    <td style="width: 75px;">
                                                                                        <dx:ASPxTimeEdit ID="txtEndAt1" runat="server" Width="85px" EditFormatString="HH:mm" EditFormat="Custom" ClientInstanceName="txtEndAt1MandatoryClient">
                                                                                            <ClientSideEvents DateChanged="function(s,e){}" />
                                                                                        </dx:ASPxTimeEdit>
                                                                                    </td>
                                                                                    <td>
                                                                                        <dx:ASPxComboBox runat="server" ID="cmbEndAt1" Width="175px" ClientInstanceName="cmbEndAt1MandatoryClient">
                                                                                            <ClientSideEvents SelectedIndexChanged="function(s,e){}" />
                                                                                        </dx:ASPxComboBox>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </div>
                                                                        <div id="panExit2" style="width: 100%; display: none;">
                                                                            <table border="0" cellpadding="1" cellspacing="1">
                                                                                <tr>
                                                                                    <td style="width: 75px;">
                                                                                        <dx:ASPxTimeEdit ID="txtEndAt2" runat="server" Width="85px" EditFormatString="HH:mm" EditFormat="Custom" ClientInstanceName="txtEndAt2MandatoryClient">
                                                                                            <ClientSideEvents DateChanged="function(s,e){}" />
                                                                                        </dx:ASPxTimeEdit>
                                                                                    </td>
                                                                                    <td>
                                                                                        <asp:Label ID="lblEndAt2" runat="server" CssClass="spanEmp-class" Text="despues de la entrada"></asp:Label>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </div>
                                                                    </td>
                                                                </tr>
                                                                <tr style="width: 100%; display: none;" id="trchkmodifyduration">
                                                                    <td style="vertical-align: text-top;">
                                                                        <dx:ASPxCheckBox ID="chkModiDuration" runat="server" ClientInstanceName="chkmodifyduration" Checked="false">
                                                                            <ClientSideEvents CheckedChanged="function(s, e) {  }" />
                                                                        </dx:ASPxCheckBox>
                                                                    </td>
                                                                    <td style="vertical-align: text-top;" colspan="2">
                                                                        <asp:Label ID="lblModifyDuration" runat="server" Text="La duración de la franja se podrá modificar al planificar el horario" CssClass="spanEmp-class"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </TabContainer1>
                                            <TabTitle2>
                                                <asp:Label ID="lblTitFilters" runat="server" Text="Filtros"></asp:Label>
                                            </TabTitle2>
                                            <TabContainer2>
                                                <table style="margin: 5px; height: 80px;" border="0" cellpadding="1" cellspacing="1">
                                                    <tr>
                                                        <td>
                                                            <input type="checkbox" id="chkRetInf" /></td>
                                                        <td>
                                                            <asp:Label ID="lblRetInf" runat="server" Text="Retrasos inferiores a:" CssClass="spanEmp-class"></asp:Label></td>
                                                        <td style="width: 75px;">
                                                            <dx:ASPxTimeEdit ID="txtRetInf" runat="server" Width="85px" EditFormatString="HH:mm" EditFormat="Custom" ClientInstanceName="txtRetInfMandatoryClient">
                                                                <ClientSideEvents DateChanged="function(s,e){}" />
                                                            </dx:ASPxTimeEdit>
                                                        </td>
                                                        <td>
                                                            <dx:ASPxComboBox runat="server" ID="cmbRetInf" Width="175px" ClientInstanceName="cmbRetInfMandatoryClient">
                                                                <ClientSideEvents SelectedIndexChanged="function(s,e){}" />
                                                            </dx:ASPxComboBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <input type="checkbox" id="chkIntInf" /></td>
                                                        <td>
                                                            <asp:Label ID="lblIntInf" runat="server" Text="Interrupciones inferiores a:" CssClass="spanEmp-class"></asp:Label></td>
                                                        <td style="width: 75px;">
                                                            <dx:ASPxTimeEdit ID="txtIntInf" runat="server" Width="85px" EditFormatString="HH:mm" EditFormat="Custom" ClientInstanceName="txtIntInfMandatoryClient">
                                                                <ClientSideEvents DateChanged="function(s,e){}" />
                                                            </dx:ASPxTimeEdit>
                                                        </td>
                                                        <td>
                                                            <dx:ASPxComboBox runat="server" ID="cmbIntInf" Width="175px" ClientInstanceName="cmbIntInfMandatoryClient">
                                                                <ClientSideEvents SelectedIndexChanged="function(s,e){}" />
                                                            </dx:ASPxComboBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <input type="checkbox" id="chkSalAnt" /></td>
                                                        <td>
                                                            <asp:Label ID="lblSalAnt" runat="server" Text="Salidas anticipadas inferiores a:" CssClass="spanEmp-class"></asp:Label></td>
                                                        <td style="width: 75px;">
                                                            <dx:ASPxTimeEdit ID="txtSalAnt" runat="server" Width="85px" EditFormatString="HH:mm" EditFormat="Custom" ClientInstanceName="txtSalAntMandatoryClient">
                                                                <ClientSideEvents DateChanged="function(s,e){}" />
                                                            </dx:ASPxTimeEdit>
                                                        </td>
                                                        <td>
                                                            <dx:ASPxComboBox runat="server" ID="cmbSalAnt" Width="175px" ClientInstanceName="cmbSalAntMandatoryClient">
                                                                <ClientSideEvents SelectedIndexChanged="function(s,e){}" />
                                                            </dx:ASPxComboBox>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </TabContainer2>
                                        </roUserControls:roTabContainerClient>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <table border="0" style="width: 100%;">
                    <tr>
                        <td>&nbsp;</td>
                        <td style="width: 110px;" align="right">
                            <dx:ASPxButton ID="btnOk" runat="server" AutoPostBack="False" CausesValidation="False" Text="Aceptar" ToolTip="Aceptar" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                <ClientSideEvents Click="function(s,e){ frmEditShiftMandatory_Save(); }" />
                            </dx:ASPxButton>
                        </td>
                        <td style="width: 110px;" align="left">
                            <dx:ASPxButton ID="btnCancel" runat="server" AutoPostBack="False" CausesValidation="False" Text="Cancelar" ToolTip="Cancelar" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                <ClientSideEvents Click="function(s,e){ frmEditShiftMandatory_Close(); }" />
                            </dx:ASPxButton>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
</div>
<!-- End Div flotant Addshift -->