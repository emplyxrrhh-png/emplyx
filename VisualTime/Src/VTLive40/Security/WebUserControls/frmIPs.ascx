<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.Security_WebUserForms_frmIPs" CodeBehind="frmIPs.ascx.vb" %>

<div id="<%= Me.ClientID %>_frm" style="position: fixed; z-index: 999; width: 400px; display: none; top: 50%; left: 50%;">
    <div id="<%= Me.ClientID %>_BgS" style="position: absolute; top: 0; left: 0; display: none; z-index: 998;"></div>
    <div class="bodyPopupExtended" style="">
        <div id="divDocumentAdvice" runat="server" style="display: ;">
            <div style="width: 100%; height: 100%; background-color: White;" class="bodyPopup">

                <table style="width: 100%; padding-top: 5px;" border="0">
                    <tr>
                        <td>
                            <div class="panHeader2">
                                <span style="">
                                    <asp:Label runat="server" ID="lblCompTit" Text="Rango de Ip's"></asp:Label></span>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <div style="height: 20px;"></div>

                            <table border="0" style="width: 100%; text-align: left;">
                                <tr>
                                    <td colspan="5" style="padding-bottom: 10px;">
                                        <input type="hidden" runat="server" id="hdnIpID" value="-1" />
                                        <asp:Label runat="server" ID="lblIpDescription" Text="Indroduzca la ip o rango que desea permitir"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="width: 50px">
                                        <input type="text" runat="server" id="txtIpMin1" style="text-align: center; text-align: center; width: 40px;"
                                            class="textEdit x-form-text x-form-field" convertcontrol="TextField"
                                            ccregex="/^([0-9]?[0-9]?[0-9])$/" ccmaxlength="3" cctime="false" ccallowblank="true" />
                                    </td>
                                    <td style="width: 50px;">.&nbsp;<input type="text" runat="server" id="txtIpMin2" style="text-align: center; text-align: center; width: 40px;"
                                        class="textEdit x-form-text x-form-field" convertcontrol="TextField"
                                        ccregex="/^([0-9]?[0-9]?[0-9])$/" ccmaxlength="3" cctime="false" ccallowblank="true" />
                                    </td>
                                    <td style="width: 50px;">.&nbsp;<input type="text" runat="server" id="txtIpMin3" style="text-align: center; text-align: center; width: 40px;"
                                        class="textEdit x-form-text x-form-field" convertcontrol="TextField"
                                        ccregex="/^([0-9]?[0-9]?[0-9])$/" ccmaxlength="3" cctime="false" ccallowblank="true" />
                                    </td>
                                    <td style="width: 50px;">.&nbsp;<input type="text" runat="server" id="txtIpMin4" style="text-align: center; text-align: center; width: 40px;"
                                        class="textEdit x-form-text x-form-field" convertcontrol="TextField"
                                        ccregex="/^([0-9]?[0-9]?[0-9])$/" ccmaxlength="3" cctime="false" ccallowblank="true" />
                                    </td>
                                    <td style="width: 50px;">/&nbsp;<input type="text" runat="server" id="txtIpMax4" style="text-align: center; text-align: center; width: 40px;"
                                        class="textEdit x-form-text x-form-field" convertcontrol="TextField"
                                        ccregex="/^([0-9]?[0-9]?[0-9])$/" ccmaxlength="3" cctime="false" />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="5" style="padding-top: 10px;">
                                        <input type="checkbox" id="ckInputRange" runat="server" onchange="showHideRange();" />&nbsp;<asp:Label ID="lblBlockSupervisor" runat="server" Text="Rango de IPs"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                            <div style="height: 20px;"></div>
                        </td>
                    </tr>
                </table>

                <input type="hidden" id="hdnMustRefresh_PageBase" value="0" runat="server" />

                <table border="0" style="width: 100%;">
                    <tr>
                        <td colspan="3" align="right">
                            <table>
                                <tr>
                                    <td style="width: 100px;" align="right">
                                        <asp:Button ID="btnOk" Text="Aceptar" runat="server" TabIndex="4" OnClientClick="SaveAllowedIP(); return false;" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                    </td>
                                    <td style="width: 110px" align="left">
                                        <asp:Button ID="btnCancel" Text="Cancelar" runat="server" TabIndex="4" OnClientClick="CancelAllowIP(); return false;" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
</div>