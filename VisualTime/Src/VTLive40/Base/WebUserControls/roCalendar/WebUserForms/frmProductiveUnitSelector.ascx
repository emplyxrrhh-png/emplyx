<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="frmProductiveUnitSelector.ascx.vb" Inherits="VTLive40.frmProductiveUnitSelector" %>

<div id="<%= Me.ClientID %>_frm" class="ui-dialog-content">
    <form id="<%= Me.ClientID %>_attr">

        <table width="100%" cellspacing="0" class="bodyPopup">
            <tr id="panComplementaryAssistance" style="height: 20px;">
                <td colspan="3">
                    <div class="panHeader2">
                        <div class="copyMode">
                            <span style="">
                                <asp:Label ID="lblPUnitSelectorTitle" runat="server" Text="Seleccione la ${ProductiveUnit}" />
                            </span>
                        </div>
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan="3" style="padding: 15px 15px 0px 15px; padding-bottom: 0px;">
                    <div class="copyMode">
                        <asp:Label ID="lblPUnitInfo" runat="server" Text="Este formulario le permite añadir la ${ProductiveUnit} al ${Budget}." CssClass="editTextFormat" />
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan="3" style="padding: 15px 15px 0px 15px" align="left">
                    <div class="panBottomMargin">
                        <div class="divRow" style="height: 100px">
                            <asp:Label ID="lblPUnit" runat="server" Text="${ProductiveUnit}:" CssClass="labelForm" Width="75px"></asp:Label>
                            <div class="componentForm">
                                <div style="float: left; padding-left: 15px;">
                                    <dx:ASPxComboBox runat="server" ID="cmbAvailablePUnits" Width="280px">
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