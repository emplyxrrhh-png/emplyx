<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="frmStarterShift.ascx.vb" Inherits="VTLive40.frmStarterShift" %>

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
                                <asp:Label ID="lblStarterTitle" runat="server" Text="Asistente de Planificación de Horarios" />
                            </span>
                        </div>
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan="3" style="padding: 15px 15px 0px 15px; padding-bottom: 0px;">
                    <div class="copyMode">
                        <asp:Label ID="lblStarterInfo" runat="server" Text="Este formulario le permite indicar las distintas opciones del horario así como la hora de inicio, fin y duración de los mismos." CssClass="editTextFormat" />
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan="3" style="padding: 15px 15px 0px 15px" align="left">

                    <div id="starterLayerDefinition" style="width: 100%">
                        <div class="panBottomMargin">
                            <div class="divRow">
                                <asp:Label ID="lblInicio1" runat="server" Text="Hora inicio:" CssClass="labelForm bigLabel"></asp:Label>
                                <div class="componentForm">
                                    <div style="float: left">
                                        <dx:ASPxTimeEdit ID="txtShiftStart1" EditFormatString="HH:mm" EditFormat="Custom" runat="server" Width="85px">
                                            <ClientSideEvents DateChanged="function(s,e){}" />
                                        </dx:ASPxTimeEdit>
                                    </div>
                                    <div style="float: left">
                                        <asp:Label ID="lblEnd1" runat="server" Text="Hora fin:" Style="width: 75px !important;" CssClass="labelForm bigLabel"></asp:Label>
                                    </div>
                                    <div style="float: left">
                                        <dx:ASPxTimeEdit ID="txtShiftEnd1" EditFormatString="HH:mm" EditFormat="Custom" runat="server" Width="85px">
                                            <ClientSideEvents DateChanged="function(s,e){}" />
                                        </dx:ASPxTimeEdit>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="panBottomMargin">
                            <div class="divRow" id="<%= Me.ClientID %>_firstLayerOrdinary">
                                <asp:Label ID="lblOrdinary1" runat="server" Text="Horas teóricas:" CssClass="labelForm bigLabel"></asp:Label>
                                <div class="componentForm">
                                    <dx:ASPxTimeEdit ID="txtShiftOrdinary1" EditFormatString="HH:mm" EditFormat="Custom" runat="server" Width="85px">
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