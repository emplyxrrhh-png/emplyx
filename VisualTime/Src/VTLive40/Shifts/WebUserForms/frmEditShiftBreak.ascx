<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.WebUserForms_frmEditShiftBreak" CodeBehind="frmEditShiftBreak.ascx.vb" %>

<input type="hidden" id="hdnBreakLayerID" />
<input type="hidden" id="hdnBreakParentID" />

<!-- Div flotant EditShiftFlexible -->
<input type="hidden" id="<%= Me.ClientID %>_hdnRuleChanges" value="0" />

<div id="<%= Me.ClientID %>_frm" style="position: fixed; *position: absolute; z-index: 9010; display: none; top: 50%; left: 50%; width: 1100px;">
    <div id="<%= Me.ClientID %>_BgS" style="position: absolute; top: 0; left: 0; display: none; z-index: 9009;"></div>
    <div class="bodyPopupExtended" style="">
        <div style="">
            <div style="width: 100%; height: 100%; background-color: White;" class="bodyPopup">

                <div id="tbCont1">
                    <div class="panHeader4" style="padding: 10px;">
                        <asp:Label ID="lblTitDefinition" runat="server" Text="Definición"></asp:Label>
                    </div>
                    <table style="margin: 5px; height: 60px;" border="0" cellpadding="1" cellspacing="1">

                        <tr>
                            <td style="height: 20px;">
                                <asp:Label ID="lblEmpCanAus" runat="server" Text="Hay que realizar el descanso entre las" CssClass="spanEmp-class"></asp:Label></td>
                            <td style="height: 20px; width: 75px;">
                                <dx:ASPxTimeEdit ID="txtCanAbsFrom" runat="server" Width="85px" EditFormatString="HH:mm" EditFormat="Custom" ClientInstanceName="txtCanAbsFromBreakClient">
                                    <ClientSideEvents DateChanged="function(s,e){}" />
                                </dx:ASPxTimeEdit>
                            </td>
                            <td style="height: 20px;">
                                <dx:ASPxComboBox runat="server" ID="cmbCanAbsFrom" Width="225px" ClientInstanceName="cmbCanAbsFromBreakClient">
                                    <ClientSideEvents SelectedIndexChanged="function(s,e){}" />
                                </dx:ASPxComboBox>
                            </td>
                            <td style="padding-left: 15px; height: 20px;">
                                <asp:Label ID="lblEmpCanAusTo" runat="server" Text="y hasta las " CssClass="spanEmp-class"></asp:Label></td>
                            <td style="height: 20px; width: 75px;">
                                <dx:ASPxTimeEdit ID="txtCanAbsTo" runat="server" Width="85px" EditFormatString="HH:mm" EditFormat="Custom" ClientInstanceName="txtCanAbsToBreakClient">
                                    <ClientSideEvents DateChanged="function(s,e){}" />
                                </dx:ASPxTimeEdit>
                            </td>
                            <td style="height: 20px;">
                                <dx:ASPxComboBox runat="server" ID="cmbCanAbsTo" Width="225px" ClientInstanceName="cmbCanAbsToBreakClient">
                                    <ClientSideEvents SelectedIndexChanged="function(s,e){}" />
                                </dx:ASPxComboBox>
                            </td>
                        </tr>
                    </table>

                    <%--  <div class="panHeader4" style="padding: 10px;">
                        <asp:Label ID="lblTitPermTimes" runat="server" Text="Tiempos permitidos"></asp:Label>
                    </div>--%>
                    <table style="margin: 5px; height: 95px;">
                        <tr>
                            <td style="height: 20px;">
                                <input type="checkbox" id="chkCanAbs" /></td>
                            <td>
                                <asp:Label ID="lblChkCanAbs" runat="server" Text="Puede ausentarse un máximo de " CssClass="spanEmp-class"></asp:Label></td>
                            <td style="width: 75px;">
                                <dx:ASPxTimeEdit ID="txtChkCanAbs" runat="server" Width="85px" EditFormatString="HH:mm" EditFormat="Custom" ClientInstanceName="txtChkCanAbsBreakClient">
                                    <ClientSideEvents DateChanged="function(s,e){}" />
                                </dx:ASPxTimeEdit>
                            </td>
                            <td>&nbsp;</td>
                        </tr>
                        <tr>
                            <td style="height: 20px;">
                                <input type="checkbox" id="chkDebAbs" /></td>
                            <td>
                                <asp:Label ID="lblDebAbs" runat="server" Text="Debe ausentarse un mínimo de " CssClass="spanEmp-class"></asp:Label></td>
                            <td style="width: 75px;">
                                <dx:ASPxTimeEdit ID="txtChkDebAbs" runat="server" Width="85px" EditFormatString="HH:mm" EditFormat="Custom" ClientInstanceName="txtChkDebAbsBreakClient">
                                    <ClientSideEvents DateChanged="function(s,e){}" />
                                </dx:ASPxTimeEdit>
                            </td>
                            <td>
                                <asp:Label ID="lblDebCont" runat="server" Text=", en caso contrario:" CssClass="spanEmp-class"></asp:Label></td>
                            <td colspan="4" style="padding-left: 15px; height: 20px;">
                                <table>
                                    <tr>
                                        <td>
                                            <input type="radio" id="optDebInc" name="optDebGroup" />&nbsp;</td>
                                        <td>
                                            <asp:Label ID="lblOptDebInc" runat="server" Text="Se genera una incidencia" CssClass="spanEmp-class"></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <input type="radio" id="optDebDesc" name="optDebGroup" />&nbsp;</td>
                                        <td>
                                            <asp:Label ID="lblOptDebDesc" runat="server" Text="Se le descuenta el tiempo necesario para llegar al mínimo del total presente." CssClass="spanEmp-class"></asp:Label></td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>

                    <%-- <div class="panHeader4" style="padding: 10px;">
                        <asp:Label ID="lblTitAbTimes" runat="server" Text="Tiempos abonados"></asp:Label>
                    </div>--%>
                    <table style="margin: 5px; height: 70px;">
                        <tr>
                            <td style="height: 20px;">
                                <input type="checkbox" id="chkTimeAbon" /></td>
                            <td>
                                <asp:Label ID="lblchkTimeAbon" runat="server" Text="El tiempo ausentado contará como presente hasta un máximo de " CssClass="spanEmp-class"></asp:Label></td>
                            <td style="width: 75px;">
                                <dx:ASPxTimeEdit ID="txtChkTimeAbon" runat="server" Width="85px" EditFormatString="HH:mm" EditFormat="Custom" ClientInstanceName="txtChkTimeAbonBreakClient">
                                    <ClientSideEvents DateChanged="function(s,e){}" />
                                </dx:ASPxTimeEdit>
                            </td>
                        </tr>
                    </table>
                    <%--  <div class="panHeader4" style="padding: 10px;">
                        <asp:Label ID="lblTitPenal" runat="server" Text="Penalizaciones"></asp:Label>
                    </div>--%>
                    <table style="margin: 5px; height: 70px;">
                        <tr>
                            <td style="height: 20px;">
                                <input type="checkbox" id="chkBreakTime" /></td>
                            <td>
                                <asp:Label ID="lblChkBreakTime" runat="server" Text="Si no se hace descanso, en lugar del tiempo mínimo contar:" CssClass="spanEmp-class"></asp:Label></td>
                            <td style="width: 75px;">
                                <dx:ASPxTimeEdit ID="txtChkBreakTime" runat="server" Width="85px" EditFormatString="HH:mm" EditFormat="Custom" ClientInstanceName="txtChkBreakTimeBreakClient">
                                    <ClientSideEvents DateChanged="function(s,e){}" />
                                </dx:ASPxTimeEdit>
                            </td>
                        </tr>
                    </table>

                    <%--      <div class="panHeader4" style="padding: 10px;">
                        <asp:Label ID="lblTitNotifications" runat="server" Text="Notificaciones"></asp:Label>
                    </div>--%>
                    <div runat="server" id="divNotifications">
                        <table style="margin: 5px; height: 70px;">
                            <tr>
                                <td style="height: 20px;">
                                    <input type="checkbox" id="chkNotificationUser" /></td>
                                <td>
                                    <asp:Label ID="lblNotificationUser" runat="server" Text="Recordar al usuario que debe realizar el descanso (Si tras " CssClass="spanEmp-class"></asp:Label></td>
                                <td style="width: 75px;">
                                    <dx:ASPxTimeEdit ID="txtNotificationUserBefore" runat="server" Width="85px" EditFormatString="HH:mm" EditFormat="Custom" ClientInstanceName="txtNotificationUserBeforeClient">
                                        <ClientSideEvents DateChanged="function(s,e){}" />
                                    </dx:ASPxTimeEdit>
                                </td>
                                <td>
                                    <asp:Label ID="lblNotificationUser2" runat="server" Text=" después del inicio del periodo de descanso aún no lo ha realizado, o bien si tras " CssClass="spanEmp-class"></asp:Label></td>
                                <td style="width: 75px;">
                                    <dx:ASPxTimeEdit ID="txtNotificationUserAfter" runat="server" Width="85px" EditFormatString="HH:mm" EditFormat="Custom" ClientInstanceName="txtNotificationUserAfterClient">
                                        <ClientSideEvents DateChanged="function(s,e){}" />
                                    </dx:ASPxTimeEdit>
                                </td>
                                <td>
                                    <asp:Label ID="lblNotificationUser3" runat="server" Text=" después del final del periodo sigue descansando)." CssClass="spanEmp-class"></asp:Label></td>
                            </tr>
                            <tr>
                                <td style="height: 20px;">
                                    <input type="checkbox" id="chkNotificationSupervisor" /></td>
                                <td colspan="5">
                                    <asp:Label ID="lblNotificationSupervisor" runat="server" Text="Recordar al supervisor si el usuario no ha realizado ningún descanso (la notificación se enviará al final del periodo permitido de descanso, en caso necesario)" CssClass="spanEmp-class"></asp:Label></td>
                            </tr>
                        </table>
                    </div>
                </div>

                <table border="0" style="width: 100%;">
                    <tr>
                        <td>&nbsp;</td>
                        <td style="width: 110px;" align="right">
                            <dx:ASPxButton ID="btnOk" runat="server" AutoPostBack="False" CausesValidation="False" Text="Aceptar" ToolTip="Aceptar" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                <ClientSideEvents Click="function(s,e){ frmEditShiftBreak_Save(); }" />
                            </dx:ASPxButton>
                        </td>
                        <td style="width: 110px;" align="left">
                            <dx:ASPxButton ID="btnCancel" runat="server" AutoPostBack="False" CausesValidation="False" Text="Cancelar" ToolTip="Cancelar" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                <ClientSideEvents Click="function(s,e){ frmEditShiftBreak_Close(); }" />
                            </dx:ASPxButton>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
</div>
<!-- End Div flotant Addshift -->