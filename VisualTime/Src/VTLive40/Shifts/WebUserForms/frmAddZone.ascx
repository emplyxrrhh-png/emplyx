<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.WebUserForms_frmAddZone" CodeBehind="frmAddZone.ascx.vb" %>

<%@ Register Src="frmNewTypeZone.ascx" TagName="frmNewTypeZone" TagPrefix="roForms" %>

<!-- Div flotant AddZone -->
<input type="hidden" id="hdnAddZoneLayer" />
<input type="hidden" id="hdnAddZoneType" />

<div id="<%= Me.ClientID %>_frm" style="position: fixed; *position: absolute; z-index: 9010; display: none; top: 50%; left: 50%; *width: 900px;">
    <div id="<%= Me.ClientID %>_BgS" style="position: absolute; top: 0; left: 0; display: none; z-index: 9009;"></div>

    <roForms:frmNewTypeZone ID="frmNewTypeZone" runat="server" />
    <div class="bodyPopupExtended" style="">
        <div style="">
            <div style="width: 100%; height: 100%; background-color: White;" class="bodyPopup">
                <table style="width: 100%; padding-top: 5px;" border="0">
                    <tr>
                        <td colspan="2">
                            <div class="panHeader2">
                                <span style="">
                                    <asp:Label runat="server" ID="lblAddZone" Text="Nueva franja horaria"></asp:Label></span>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" style="padding: 2px;">
                            <table border="0" style="width: 100%;">
                                <tr>
                                    <td></td>
                                    <td>
                                        <asp:Label ID="lblTitleFormAddZone" runat="server" CssClass="spanEmp-class" Text="Use este diálogo para crear una nueva franja horaria para el horario que esta editando" Width="445px"></asp:Label></td>
                                </tr>
                                <tr>
                                    <td colspan="3" style="padding-top: 5px; padding-bottom: 10px;" align="center">
                                        <table width="100%" border="0">
                                            <tr>
                                                <td colspan="3" align="center">
                                                    <table width="100%">
                                                        <tr>
                                                            <td style="padding-bottom: 10px;">
                                                                <div id="panTypeCmb" style="display: none;">
                                                                    <dx:ASPxComboBox runat="server" ID="cmbType" Width="175px" ClientInstanceName="cmbTypeAddZoneClient">
                                                                        <ClientSideEvents SelectedIndexChanged="function(s,e){}" />
                                                                    </dx:ASPxComboBox>
                                                                </div>
                                                                <div id="panTypeRO" style="width: 100%; border: solid 1px #B5B8C8; display: ; text-align: left;">
                                                                    <asp:Label ID="lblCmbTypeDesc" runat="server" Text="" class="spanEmp-class"></asp:Label>
                                                                </div>
                                                            </td>
                                                            <td style="padding-bottom: 10px;">
                                                                <div id="divZoneListActions" style="background-color: #E8EEF7; width: 60px; height: 25px; text-align: center; vertical-align: middle;">
                                                                    <div style="padding-top: 5px;">
                                                                        <img id="imgZoneAddListValue" src="../Base/Images/Grid/add.png" visible="true" title='<%# Me.Language.Translate("addListValue",Me.DefaultScope) %>' style="cursor: pointer;" onclick="frmNewTypeZone_ShowAddListValue(); " />
                                                                        <img id="imgZoneEditListValue" src="../Base/Images/Grid/edit.png" visible="true" title='<%# Me.Language.Translate("editListValue",Me.DefaultScope) %>' style="cursor: pointer;" onclick="frmNewTypeZone_ShowEditListValue(cmbTypeAddZoneClient.GetValue()); " />
                                                                        <img id="imgZoneRemoveListValue" src="../Base/Images/Grid/remove.png" visible="true" title='<%# Me.Language.Translate("delListValue",Me.DefaultScope) %>' style="cursor: pointer;" onclick="frmNewTypeZone_RemoveListValue(cmbTypeAddZoneClient.GetValue());" />
                                                                    </div>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 5px;">
                                                    <asp:Label ID="lblShiftFromTime" runat="server" Text="Desde las " CssClass="spanEmp-class"></asp:Label></td>
                                                <td style="width: 75px; text-align: right;">
                                                    <dx:ASPxTimeEdit ID="txtFromTime" runat="server" Width="85px" EditFormatString="HH:mm" EditFormat="Custom" ClientInstanceName="txtFromTimeAddZoneClient">
                                                        <ClientSideEvents DateChanged="function(s,e){}" />
                                                    </dx:ASPxTimeEdit>
                                                </td>
                                                <td style="width: 212px; text-align: right;">
                                                    <dx:ASPxComboBox runat="server" ID="cmbShiftFromTime" Width="175px" ClientInstanceName="cmbShiftFromTimeAddZoneClient">
                                                        <ClientSideEvents SelectedIndexChanged="function(s,e){}" />
                                                    </dx:ASPxComboBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 5px;">
                                                    <asp:Label ID="lblShiftToTime" runat="server" Text="y hasta las " CssClass="spanEmp-class"></asp:Label></td>
                                                <td style="width: 75px; text-align: right;">
                                                    <dx:ASPxTimeEdit ID="txtToTime" runat="server" Width="85px" EditFormatString="HH:mm" EditFormat="Custom" ClientInstanceName="txtToTimeAddZoneClient">
                                                        <ClientSideEvents DateChanged="function(s,e){}" />
                                                    </dx:ASPxTimeEdit>
                                                </td>
                                                <td style="width: 212px; text-align: right;">
                                                    <dx:ASPxComboBox runat="server" ID="cmbShiftToTime" Width="175px" ClientInstanceName="cmbShiftToTimeAddZoneClient">
                                                        <ClientSideEvents SelectedIndexChanged="function(s,e){}" />
                                                    </dx:ASPxComboBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="3" style="padding-left: 5px; padding-top: 5px;">
                                                    <table cellpadding="0" cellspacing="0" border="0" style="height: 12px;">
                                                        <tr>
                                                            <td>
                                                                <input type="checkbox" id="chkIsLocked" runat="server" />&nbsp;</td>
                                                            <td style="vertical-align: middle;">
                                                                <asp:Label ID="lblIsLocked" runat="server" Text="Bloquear la zona horaria." CssClass="spanEmp-class"></asp:Label></td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
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
                                <ClientSideEvents Click="function(s,e){ frmAddZone_Save(); }" />
                            </dx:ASPxButton>
                        </td>
                        <td style="width: 110px;" align="left">
                            <dx:ASPxButton ID="btnCancel" runat="server" AutoPostBack="False" CausesValidation="False" Text="Cancelar" ToolTip="Cancelar" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                <ClientSideEvents Click="function(s,e){ frmAddZone_Close(); }" />
                            </dx:ASPxButton>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
</div>
<!-- End Div flotant AddZone -->