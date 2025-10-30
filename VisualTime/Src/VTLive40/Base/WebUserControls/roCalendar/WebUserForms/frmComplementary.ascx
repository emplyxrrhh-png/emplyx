<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="frmComplementary.ascx.vb" Inherits="VTLive40.frmComplementary" %>

<div id="<%= Me.ClientID %>_frm" class="ui-dialog-content">
    <form id="<%= Me.ClientID %>_attr">
        <dx:ASPxCallbackPanel ID="shiftDefinitionCallback" runat="server" Width="0%" Height="0%">
        </dx:ASPxCallbackPanel>

        <table width="100%" cellspacing="0" class="bodyPopup">
            <tr id="panComplementaryAssistance" style="height: 20px;">
                <td colspan="3">
                    <div class="panHeader2">
                        <div class="copyMode">
                            <span style="">
                                <asp:Label ID="lblComplementaryTitle" runat="server" Text="Asistente de Planificación de Horarios" />
                            </span>
                        </div>
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan="3" style="padding: 15px 15px 0px 15px; padding-bottom: 0px;">
                    <div class="copyMode">
                        <asp:Label ID="lblComplementaryInfo" runat="server" Text="Este formulario le permite planificar los horarios con horas complementarias así como el inicio y duración de los mismos." CssClass="editTextFormat" />
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan="3" style="padding: 15px 15px 0px 15px" align="left">

                    <div id="<%= Me.ClientID %>_advShiftStartFloating" style="width: 100%;">
                        <div class="panBottomMargin">
                            <div class="panHeader2 panBottomMargin">
                                <span class="panelTitleSpan">
                                    <asp:Label runat="server" ID="lblFloatingShift" Text="Hora Inicio Horario Flotante"></asp:Label>
                                </span>
                            </div>
                        </div>
                        <div class="panBottomMargin">
                            <div class="divRow">
                                <asp:Label ID="lblShiftStartFloating" runat="server" Text="Hora inicio:" CssClass="labelForm bigLabel"></asp:Label>
                                <div class="componentForm">
                                    <div style="float: left">
                                        <dx:ASPxTimeEdit ID="txtShiftFloatingStart" EditFormatString="HH:mm" EditFormat="Custom" runat="server" Width="85px">
                                            <ClientSideEvents DateChanged="function(s,e){}" />
                                        </dx:ASPxTimeEdit>
                                    </div>

                                    <div style="float: left; padding-left: 15px;">
                                        <dx:ASPxComboBox runat="server" ID="cmbStartAtFloating" Width="180px">
                                            <ClientSideEvents SelectedIndexChanged="function(s,e){}" />
                                        </dx:ASPxComboBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div id="<%= Me.ClientID %>_advFirstLayer" style="width: 100%">
                        <div class="panBottomMargin">
                            <div class="panHeader2 panBottomMargin">
                                <span class="panelTitleSpan">
                                    <asp:Label runat="server" ID="lblFirstLayer" Text="Primera Franja"></asp:Label>
                                </span>
                            </div>
                        </div>
                        <div class="panBottomMargin">
                            <div class="divRow">
                                <asp:Label ID="lblInicio1" runat="server" Text="Hora inicio:" CssClass="labelForm bigLabel"></asp:Label>
                                <div class="componentForm">
                                    <div style="float: left">
                                        <dx:ASPxTimeEdit ID="txtShiftStart1" EditFormatString="HH:mm" EditFormat="Custom" runat="server" Width="85px">
                                            <ClientSideEvents DateChanged="function(s,e){}" />
                                        </dx:ASPxTimeEdit>
                                    </div>

                                    <div style="float: left; padding-left: 15px;">
                                        <dx:ASPxComboBox runat="server" ID="cmbStartAt1" Width="180px">
                                            <ClientSideEvents SelectedIndexChanged="function(s,e){}" />
                                        </dx:ASPxComboBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="panBottomMargin">
                            <div class="divRow" id="<%= Me.ClientID %>_firstLayerOrdinary">
                                <asp:Label ID="lblOrdinary1" runat="server" Text="Horas Ordinarias:" CssClass="labelForm bigLabel"></asp:Label>
                                <div class="componentForm">
                                    <dx:ASPxTimeEdit ID="txtShiftOrdinary1" EditFormatString="HH:mm" EditFormat="Custom" runat="server" Width="85px">
                                        <ClientSideEvents DateChanged="function(s,e){}" />
                                    </dx:ASPxTimeEdit>
                                </div>
                            </div>
                        </div>
                        <div class="panBottomMargin" id="<%= Me.ClientID %>_firstLayerComplementary">
                            <div class="divRow">
                                <asp:Label ID="lblComplementaryHours" runat="server" Text="Horas Complementarias:" CssClass="labelForm bigLabel"></asp:Label>
                                <div class="componentForm">
                                    <dx:ASPxTimeEdit ID="txtShiftComplementary1" EditFormatString="HH:mm" EditFormat="Custom" runat="server" Width="85px">
                                        <ClientSideEvents DateChanged="function(s,e){}" />
                                    </dx:ASPxTimeEdit>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div id="<%= Me.ClientID %>_advSecondLayer" style="width: 100%">
                        <div class="panBottomMargin">
                            <div class="panHeader2 panBottomMargin">
                                <span class="panelTitleSpan">
                                    <asp:Label runat="server" ID="lblSecondLayer" Text="Segunda Franja"></asp:Label>
                                </span>
                            </div>
                        </div>
                        <div class="panBottomMargin">
                            <div class="divRow">
                                <asp:Label ID="lblInicio2" runat="server" Text="Hora inicio:" CssClass="labelForm bigLabel"></asp:Label>
                                <div class="componentForm">
                                    <div style="float: left">
                                        <dx:ASPxTimeEdit ID="txtShiftStart2" EditFormatString="HH:mm" EditFormat="Custom" runat="server" Width="85px">
                                            <ClientSideEvents DateChanged="function(s,e){}" />
                                        </dx:ASPxTimeEdit>
                                    </div>

                                    <div style="float: left; padding-left: 15px;">
                                        <dx:ASPxComboBox runat="server" ID="cmbStartAt2" Width="180px">
                                            <ClientSideEvents SelectedIndexChanged="function(s,e){}" />
                                        </dx:ASPxComboBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="panBottomMargin">
                            <div class="divRow" id="<%= Me.ClientID %>_secondLayerOrdinary">
                                <asp:Label ID="lblOrdinary2" runat="server" Text="Horas Ordinarias:" CssClass="labelForm bigLabel"></asp:Label>
                                <div class="componentForm">
                                    <dx:ASPxTimeEdit ID="txtShiftOrdinary2" EditFormatString="HH:mm" EditFormat="Custom" runat="server" Width="85px">
                                        <ClientSideEvents DateChanged="function(s,e){}" />
                                    </dx:ASPxTimeEdit>
                                </div>
                            </div>
                        </div>
                        <div class="panBottomMargin" id="<%= Me.ClientID %>_secondLayerComplementary">
                            <div class="divRow">
                                <asp:Label ID="lblComplementaryHours2" runat="server" Text="Horas Complementarias:" CssClass="labelForm bigLabel"></asp:Label>
                                <div class="componentForm">
                                    <dx:ASPxTimeEdit ID="txtShiftComplementary2" EditFormatString="HH:mm" EditFormat="Custom" runat="server" Width="85px">
                                        <ClientSideEvents DateChanged="function(s,e){}" />
                                    </dx:ASPxTimeEdit>
                                </div>
                            </div>
                        </div>
                    </div>
                </td>
            </tr>
        </table>
    </form>
</div>