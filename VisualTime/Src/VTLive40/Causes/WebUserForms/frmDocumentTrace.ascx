<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.Causes_WebUserForms_frmDocumentTrace" CodeBehind="frmDocumentTrace.ascx.vb" %>

<script type="text/javascript">
    function showDocObjects(index) {
        if (index == 0 || index == 1) {
            ChangeDocumentFirstTime(0);
        } else {
            ChangeDocumentFirstTime(1);
        }
    }

    function changeOptionsVisibility(s, e) {

        var docType = parseInt(s.GetSelectedItem().value.split("_")[1], 10);

        if (docType == 0 || docType == 1) {
            var optDocumentOptions1 = document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_frmDocumentTrace1_optDocumentOptions1_rButton").checked = true;
            var optDocumentOptions1 = document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_frmDocumentTrace1_optDocumentOptions2_rButton").checked = false;
            activateDocumentOptions();

            document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmDocumentTrace1_optDocumentOptions1_panOptionPanel').setAttribute('venabled', 'True');
            document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmDocumentTrace1_optDocumentOptions2_panOptionPanel').setAttribute('venabled', 'False');

        } else if (docType == 2) {
            var optDocumentOptions1 = document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_frmDocumentTrace1_optDocumentOptions1_rButton").checked = false;
            var optDocumentOptions1 = document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_frmDocumentTrace1_optDocumentOptions2_rButton").checked = true;
            activateDocumentOptions2();

            document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmDocumentTrace1_optDocumentOptions1_panOptionPanel').setAttribute('venabled', 'False');
            document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmDocumentTrace1_optDocumentOptions2_panOptionPanel').setAttribute('venabled', 'True');
        } else {
            var optDocumentOptions1 = document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_frmDocumentTrace1_optDocumentOptions1_rButton").checked = true;
            var optDocumentOptions1 = document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_frmDocumentTrace1_optDocumentOptions2_rButton").checked = false;
            activateDocumentOptions();

            document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmDocumentTrace1_optDocumentOptions1_panOptionPanel').setAttribute('venabled', 'True');
            document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmDocumentTrace1_optDocumentOptions2_panOptionPanel').setAttribute('venabled', 'True');
        }

        venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmDocumentTrace1_optDocumentOptions1');
        venableOPC('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmDocumentTrace1_optDocumentOptions2');
        linkOPCItems('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmDocumentTrace1_optDocumentOptions1,ctl00_contentMainBody_ASPxCallbackPanelContenido_frmDocumentTrace1_optDocumentOptions2');

    }

    function activateDocumentOptions() {
        var optDocumentOptions1 = document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_frmDocumentTrace1_optDocumentOptions1_rButton").checked;

        if (optDocumentOptions1 == true) {
            cmbDocumentFirstTimeClient.SetEnabled(true);
            enableElement(document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmDocumentTrace1_optDocumentOptions1_txtDocumentFirstTimeDuration'));
        } else {
            cmbDocumentFirstTimeClient.SetEnabled(false);
            disableElement(document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmDocumentTrace1_optDocumentOptions1_txtDocumentFirstTimeDuration'));
        }
    }

    function activateDocumentOptions2() {
        var optDocumentOptions2 = document.getElementById("ctl00_contentMainBody_ASPxCallbackPanelContenido_frmDocumentTrace1_optDocumentOptions2_rButton").checked;

        if (optDocumentOptions2 == true) {
            //cmbDayWeekMonthClient.SetEnabled(true);
            //cmbBeginEndNextClient.SetEnabled(true);
            enableElement(document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmDocumentTrace1_optDocumentOptions2_txtDocumentEveryTimeDuration'));
            enableElement(document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmDocumentTrace1_optDocumentOptions2_txtDocumentEveryTimeDuration2'));
        } else {
            //cmbDayWeekMonthClient.SetEnabled(false);
            //cmbBeginEndNextClient.SetEnabled(false);
            disableElement(document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmDocumentTrace1_optDocumentOptions2_txtDocumentEveryTimeDuration'));
            disableElement(document.getElementById('ctl00_contentMainBody_ASPxCallbackPanelContenido_frmDocumentTrace1_optDocumentOptions2_txtDocumentEveryTimeDuration2'));
        }
    }
</script>

<div id="<%= Me.ClientID %>_frm" style="position: fixed; z-index: 10999; display: none; top: 50%; left: 50%;">

    <div id="<%= Me.ClientID %>_BgS" style="position: absolute; top: 0; left: 0; display: none; z-index: 10998;"></div>

    <input type="hidden" id="IdDocumentTrace" runat="server" value="" />
    <input type="hidden" id="IdRow" runat="server" value="" />

    <div class="bodyPopupExtended" style="">

        <div style="width: 98%; height: 100%; background-color: White;" class="bodyPopup">

            <table style="width: 100%; padding-top: 5px;" border="0">
                <tr>
                    <td>
                        <div class="panHeader2">
                            <span style="">
                                <asp:Label runat="server" ID="lblCompTit" Text="Configurar Documento"></asp:Label></span>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div style="height: 15px;"></div>
                        <table border="0" style="width: 100%; text-align: left;">
                            <tr>
                                <td style="padding-left: 10px; width: 15%;">
                                    <asp:Label ID="lblDocument" runat="server" Text="Configurar seguimiento del Documento"></asp:Label>
                                </td>
                                <td>
                                    <dx:ASPxComboBox ID="cmbDocument" runat="server" Width="250px" ClientInstanceName="cmbDocumentClient">
                                        <ClientSideEvents SelectedIndexChanged="changeOptionsVisibility" />
                                    </dx:ASPxComboBox>
                                </td>
                            </tr>
                            <tr>
                                <td style="padding-left: 10px; width: 15%;">
                                    <asp:Label ID="lblLabAgree" runat="server" Text="Convenio"></asp:Label>
                                </td>
                                <td>
                                    <dx:ASPxComboBox ID="cmbLabAgree" runat="server" Width="250px" ClientInstanceName="cmbLabAgreeClient">
                                    </dx:ASPxComboBox>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2" style="padding-left: 10px; padding-top: 15px; width: 100%;">
                                    <asp:Label ID="lblRequest" runat="server" Text="El documento se requiere:"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2" style="padding-top: 5px; width: 100%;">

                                    <roUserControls:roOptionPanelClient ID="optDocumentOptions1" runat="server" TypeOPanel="RadioOption" width="100%" height="Auto" Checked="True" Enabled="True" Border="True" CConClick="activateDocumentOptions();">
                                        <Title>
                                            <asp:Label ID="Label1" runat="server" Text="Una única vez"></asp:Label>
                                        </Title>
                                        <Description>
                                        </Description>
                                        <Content>
                                            <table border="0" style="text-align: left;">
                                                <tr>
                                                    <td style="padding-left: 10px; width: auto; padding-right: 8px;">
                                                        <asp:Label ID="lblDocumentInfo1" runat="server" Text="El documento se requiere"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <dx:ASPxComboBox ID="cmbDocumentFirstTime" runat="server" Width="250px" ClientInstanceName="cmbDocumentFirstTimeClient">
                                                            <ClientSideEvents SelectedIndexChanged="function(s,e){ showDocObjects(s.GetSelectedIndex()); }" />
                                                        </dx:ASPxComboBox>
                                                    </td>
                                                    <td id="tdDocumentFirstTimeDaysDuration" style="padding-left: 10px; display: none;">
                                                        <input type="text" id="txtDocumentFirstTimeDuration" runat="server" maxlength="3" convertcontrol="NumberField" ccmaxvalue="999" ccallowdecimals="false" ccallownegative="false"
                                                            class="textClass" style="width: 25px; text-align: right;" value="" />
                                                        <asp:Label ID="lblDocumentFirstTimeDaysDuration" runat="server" Text="días después de la ausencia." Style="padding-left: 5px;"></asp:Label>
                                                    </td>
                                                    <td id="tdDocumentFirstTimeDaysDuration2">
                                                        <asp:Label ID="Label5" runat="server" Text="la ausencia." Style="padding-left: 5px;"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                </tr>
                                            </table>
                                        </Content>
                                    </roUserControls:roOptionPanelClient>

                                    <div style="height: 5px;"></div>

                                    <roUserControls:roOptionPanelClient ID="optDocumentOptions2" runat="server" TypeOPanel="RadioOption" width="100%" height="Auto" Checked="False" Enabled="True" Border="True" CConClick="activateDocumentOptions2();">
                                        <Title>
                                            <asp:Label ID="Label2" runat="server" Text="De forma periódica"></asp:Label>
                                        </Title>
                                        <Description>
                                        </Description>
                                        <Content>
                                            <table border="0" style="text-align: left;">
                                                <tr>
                                                    <td style="padding-left: 10px; width: auto; padding-right: 8px;">
                                                        <asp:Label ID="lblDocumentInfo2" runat="server" Text="El documento se requiere cada"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <input type="text" id="txtDocumentEveryTimeDuration" runat="server" maxlength="3" convertcontrol="NumberField" ccmaxvalue="999" ccallowdecimals="false" ccallownegative="false"
                                                            class="textClass" style="width: 25px; text-align: right;" value="" />
                                                    </td>
                                                    <%--<td style="padding-left: 8px;">
                                                        <dx:ASPxComboBox ID="cmbDayWeekMonth" runat="server" Width="250px" ClientInstanceName="cmbDayWeekMonthClient">
                                                        </dx:ASPxComboBox>
                                                    </td>--%>
                                                    <td>
                                                        <asp:Label ID="lblDocumentsDaysAfter" runat="server" Text="días a partir de" Style="padding-left: 5px; padding-right: 5px;"></asp:Label>
                                                        <input type="text" id="txtDocumentEveryTimeDuration2" runat="server" maxlength="3" convertcontrol="NumberField" ccmaxvalue="999" ccallowdecimals="false" ccallownegative="false"
                                                            class="textClass" style="width: 25px; text-align: right;" value="" />
                                                        <asp:Label ID="lblDocumentsDaysAfterStart" runat="server" Text="días del inicio de la ausencia" Style="padding-left: 5px; padding-right: 5px;"></asp:Label>
                                                    </td>

                                                    <%--<td>
                                                        <dx:ASPxComboBox ID="cmbBeginEndNext" runat="server" Width="250px" ClientInstanceName="cmbBeginEndNextClient">
                                                        </dx:ASPxComboBox>
                                                    </td>--%>
                                                    <%--<td style="padding-left: 8px;">
                                                        <asp:Label ID="lblDocumentEveryTimeDuration2" runat="server" Text="de la ausencia."></asp:Label>
                                                    </td>--%>
                                                </tr>
                                            </table>
                                        </Content>
                                    </roUserControls:roOptionPanelClient>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>

            <input type="hidden" id="hdnMustRefresh_PageBase" value="0" runat="server" />

            <div style="height: 10px;"></div>

            <table border="0" style="width: 100%;">
                <tr>
                    <td colspan="3" align="right">
                        <table>
                            <tr>
                                <td style="width: 110px;" align="right">
                                    <dx:ASPxButton ID="ASPxButton1" ClientInstanceName="btnAcceptClient" runat="server" AutoPostBack="False" CausesValidation="False" Text="Aceptar" ToolTip="Aceptar"
                                        HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                        <ClientSideEvents Click="function(s, e) { saveDocumentTrace(); }" />
                                    </dx:ASPxButton>
                                </td>
                                <td style="width: 110px;" align="left">
                                    <dx:ASPxButton ID="ASPxButton2" ClientInstanceName="btnCancelClient" runat="server" AutoPostBack="False" CausesValidation="False" Text="Cancelar" ToolTip="Cancelar"
                                        HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                        <ClientSideEvents Click="function(s, e) { cancelDocumentTrace(); }" />
                                    </dx:ASPxButton>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
    </div>
</div>