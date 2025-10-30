<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Terminals_TerminalSirens" Culture="auto" UICulture="auto" CodeBehind="TerminalSirens.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<%@ Register Src="~/Terminals/WebUserForms/frmAddSiren.ascx" TagName="frmAddSiren" TagPrefix="roForms" %>

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Configuración sirenas del ${Terminal}</title>
</head>
<body class="bodyPopup" style="background-attachment: fixed;">

    <form id="frmTerminalSirens" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <div>

            <script language="javascript" type="text/javascript">
                function PageBase_Load() {

                    ConvertControls();

                    hdnAddSirenModeEditId = 'hdnModeEdit';

                    getSirensGrid(getUrlParameter('IDTerminal'));

                }
            </script>

            <input type="hidden" runat="server" id="hdnModeEdit" value="" />
            <input type="hidden" id="hdnLngWeekdayName" value="<%= Me.Language.Translate("gridHeader.WeekDayName", "Terminals") %>" />
            <input type="hidden" id="hdnLngHour" value="<%= Me.Language.Translate("gridHeader.Hour", "Terminals") %>" />
            <input type="hidden" id="hdnLngDuration" value="<%= Me.Language.Translate("gridHeader.Duration","Terminals") %>" />

            <input type="hidden" id="msgHasChanges" value="" runat="server" clientidmode="Static" />
            <input type="hidden" id="msgHasErrors" value="" runat="server" clientidmode="Static" />

            <div id="divModalBgDisabled" class="modalBackground" style="position: absolute; left: 0px; top: 0px; z-index: 9000; width: 1680px; height: 900px; display: none;"></div>
            <div id="div03" style="width: 100%; height: 100%; padding: 0px;" runat="server" name="menuPanel">
                <div class="panHeader2">
                    <span style="">
                        <asp:Label runat="server" ID="lblSirensTitle" Text="Sirenas ({1})"></asp:Label></span>
                </div>
                <br />
                <table width="100%" border="0" style="padding-top: 5px; padding-left: 10px; padding-right: 10px;">
                    <tr>
                        <td style="width: 50px;">
                            <img src="Images/Sirens48.png" /></td>
                        <td>
                            <asp:Label ID="lblSirens" runat="server" Text="Información de los toques de sirena que hay programados para este Terminal. Desde aquí, puede darlos de alta, baja y modificarlos."></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <div id="divMsgTop" class="divMsg" style="display: none;">
                                <table style="width: 100%;">
                                    <tr>
                                        <td align="center" style="width: 20px; height: 16px;">
                                            <img id="Img1" src="~/Base/Images/MessageFrame/Alert16.png" runat="server" /></td>
                                        <td align="left" style="padding-left: 10px; color: white;"><span id="msgTop"></span></td>
                                        <td align="right" style="color: White; padding-right: 10px;">
                                            <a href="javascript: void(0);" class="aMsg" onclick="saveChanges();"><span id="lblSaveChanges" runat="server" /></a>
                                            &nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;
                                        <a href="javascript: void(0);" onclick="undoChanges();" class="aMsg"><span id="lblUndoChanges" runat="server" /></a>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <div id="divContent" style="height: 320px; overflow: auto;">
                                <table width="100%" border="0">
                                    <tr>
                                        <td>
                                            <table>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lblActRelay" runat="server" Text="Las sirenas activarán el relé "></asp:Label></td>
                                                    <td style="padding-left: 10px;">
                                                        <roWebControls:roComboBox ID="cmbRelay" runat="server" EnableViewState="true" AutoResizeChildsWidth="True" ParentWidth="50px" ChildsVisible="3" ItemsRunAtServer="false" HiddenText="cmbRelay_Text" HiddenValue="cmbRelay_Value"></roWebControls:roComboBox>
                                                        <input type="hidden" id="cmbRelay_Text" runat="server" />
                                                        <input type="hidden" id="cmbRelay_Value" runat="server" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right">
                                            <!-- Barra Herramientas Sirenas -->
                                            <div id="panTbSirens" runat="server">
                                                <table style="margin-bottom: 0pt; margin-top: 0pt; margin-right: 0pt; width: 100%;" border="0" cellpadding="0" cellspacing="0">
                                                    <tbody>
                                                        <tr>
                                                            <td colspan="2" style="padding: 2px 5px 2px 2px;" align="right">
                                                                <div class="btnFlat">
                                                                    <a href="javascript: void(0)" id="btnAddSiren" runat="server" onclick="AddNewSiren()">
                                                                        <span class="btnIconAdd"></span>
                                                                        <span id="lblAddSiren"><%= Me.Language.Translate("addNew",DefaultScope) %></span>
                                                                    </a>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </div>
                                            <!-- Fin Barra Herramientas Sirenas -->
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <roForms:frmAddSiren ID="frmAddSiren1" runat="server" />
                                            <div id="grdSirens" style="width: 90%;"></div>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" style="display: none;">
                            <div id="divMsgBottom" class="divMsg" style="display: none;">
                                <table style="width: 100%;">
                                    <tr>
                                        <td align="center" style="width: 20px; height: 16px;">
                                            <img id="Img2" src="~/Base/Images/MessageFrame/Alert16.png" runat="server" /></td>
                                        <td align="left" style="padding-left: 10px; color: white;"><span id="msgBottom"></span></td>
                                        <td align="right" style="color: White; padding-right: 10px;">
                                            <a href="javascript: void(0);" class="aMsg" onclick="saveChanges();"><span id="lblSaveChangesBottom" runat="server" /></a>
                                            &nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;
                                        <a href="javascript: void(0);" onclick="undoChanges();" class="aMsg"><span id="lblUndoChangesBottom" runat="server" /></a>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>
                </table>
            </div>

            <Local:MsgBoxForm ID="MsgBoxForm1" runat="server" />
        </div>
    </form>
</body>
</html>