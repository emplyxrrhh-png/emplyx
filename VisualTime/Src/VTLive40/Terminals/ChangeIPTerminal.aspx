<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.ChangeIPTerminal" CodeBehind="ChangeIPTerminal.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="frmChangeIPTerminal" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <script language="javascript" type="text/javascript">

            function PageBase_Load() {
                InitConvertControls();
                ConvertControls();
            }
        </script>

        <asp:HiddenField ID="hdnCanClose_PageBase" runat="server" />
        <asp:HiddenField ID="hdnMustRefresh_PageBase" Value="0" runat="server" />

        <div>
            <table width="100%" border="0">
                <tr>
                    <td>
                        <img runat="server" id="imgTerminal" src="~/Terminals/Images/Terminals90.png" style="width: 48px; border: solid 1px #EFEFEF;" /></td>
                    <td>
                        <asp:Label ID="lblDescChangeIP" runat="server" Text="Inserte la nueva IP para el terminal."></asp:Label><br />
                        <asp:Label ID="lblDescError" runat="server" Text="" Font-Bold="true" CssClass="errorText" Visible="false"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="padding-right: 2px; padding-top: 5px; padding-left: 10px;">
                        <asp:Label ID="lblIP" runat="server" Text="IP Terminal:" Font-Bold="true"></asp:Label>
                    </td>
                    <td style="padding-top: 5px;">
                        <input type="text" convertcontrol="NumberField" style="width: 50px; text-align: right;" class="textClass x-form-text x-form-field" id="txtIP1" maxlength="3" align="right" ccallowdecimals="false" ccallowblank="false" ccmaxvalue="255" ccminvalue="0" runat="server" />
                        .
                        <input type="text" convertcontrol="NumberField" style="width: 50px; text-align: right;" class="textClass x-form-text x-form-field" id="txtIP2" maxlength="3" align="right" ccallowdecimals="false" ccallowblank="false" ccmaxvalue="255" ccminvalue="0" runat="server" />
                        .
                        <input type="text" convertcontrol="NumberField" style="width: 50px; text-align: right;" class="textClass x-form-text x-form-field" id="txtIP3" maxlength="3" align="right" ccallowdecimals="false" ccallowblank="false" ccmaxvalue="255" ccminvalue="0" runat="server" />
                        .
                        <input type="text" convertcontrol="NumberField" style="width: 50px; text-align: right;" class="textClass x-form-text x-form-field" id="txtIP4" maxlength="3" align="right" ccallowdecimals="false" ccallowblank="false" ccmaxvalue="255" ccminvalue="0" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="right" style="padding-top: 10px;">
                        <table>
                            <tr>
                                <td>
                                    <asp:Button ID="btOK" Text="${Button.Accept}" runat="server" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                </td>
                                <td>
                                    <asp:Button ID="btCancel" Text="${Button.Cancel}" runat="server" OnClientClick="Close(); return false;" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>