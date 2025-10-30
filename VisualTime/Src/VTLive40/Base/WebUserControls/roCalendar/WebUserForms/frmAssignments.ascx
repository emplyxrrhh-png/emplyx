<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="frmAssignments.ascx.vb" Inherits="VTLive40.frmAssignments" %>

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
                                <asp:Label ID="lblAssignmentsTitle" runat="server" Text="Seleccione un puesto" />
                            </span>
                        </div>
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan="3" style="padding: 15px 15px 0px 15px; padding-bottom: 0px;">
                    <div class="copyMode">
                        <asp:Label ID="lblComplementaryInfo" runat="server" Text="Este formulario le permite asignar o modificar el puesto que desarrollará el empleado." CssClass="editTextFormat" />
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan="3" style="padding: 15px 15px 0px 15px" align="left">
                    <div class="panBottomMargin">
                        <div class="divRow" style="height: 100px">
                            <asp:Label ID="lblAssignment" runat="server" Text="Puesto:" CssClass="labelForm" Width="75px"></asp:Label>
                            <div class="componentForm">
                                <div style="float: left; padding-left: 15px;">
                                    <dx:ASPxComboBox runat="server" ID="cmbAvailableAssignments" Width="280px">
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){}" />
                                    </dx:ASPxComboBox>
                                </div>
                            </div>
                        </div>
                    </div>
                </td>
            </tr>
        </table>
    </form>
</div>