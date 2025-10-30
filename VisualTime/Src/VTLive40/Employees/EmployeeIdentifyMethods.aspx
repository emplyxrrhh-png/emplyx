<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Forms_EmployeeIdentifyMethods" Culture="auto" UICulture="auto" EnableViewState="True" CodeBehind="EmployeeIdentifyMethods.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Métodos de Identificación</title>
</head>
<body class="bodyPopup" style="background-attachment: fixed;">

    <form id="frmEmployeeIdentifyMethods" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <div class="DetailFrame_TopMid">

            <script type="text/javascript">

                function PageBase_Load() {

                    ConvertControls('');
                    IdentifyMethodsLoad();

                }

                function CheckFields() {

                    var bolRet = true; 

                    if (CheckConvertControls('') == true) {
                        //Show loading popup
                        bolRet = true;
                    }
                    else {
                        bolRet = false;
                        var message = "TitleKey=SaveEmployeeIdentifyMethods.Check.Invalid.Text&" +
                            "DescriptionKey=SaveEmployeeIdentifyMethods.Check.Invalid.Description&" +
                            "Option1TextKey=SaveEmployeeIdentifyMethods.Check.Invalid.Option1Text&" +
                            "Option1DescriptionKey=SaveEmployeeIdentifyMethods.Check.Invalid.Option1Description&" +
                            "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                                  'IconUrl=<%= Robotics.Web.Base.HelperWeb.MsgBoxIconsUrl(Robotics.Web.Base.HelperWeb.MsgBoxIcons.ErrorIcon) %>'
                        var url = "Employees/srvMsgBoxEmployees.aspx?action=Message&Parameters=" + encodeURIComponent(message);
                        parent.ShowMsgBoxForm(url, 500, 300, '');
                    }

                    return bolRet;
                }

                function RestorePwd() {
                    parent.showLoader(true);
                    var stamp = '&StampParam=' + new Date().getMilliseconds();

                    var ajax = nuevoAjax();
                    ajax.open("GET", "../Security/Handlers/srvSupervisorsV3.ashx?action=resetPassport&ID=" + document.getElementById('<%= hdnIDEmployee.ClientID %>').value + "&PassportType=E" + stamp, true);

                    ajax.onreadystatechange = function () {
                        if (ajax.readyState == 4) {
                            parent.showLoader(false);
                            if (ajax.responseText == 'OK') {

                            } else {
                                if (ajax.responseText.substr(0, 7) == 'MESSAGE') {
                                    var url = "Security/srvMsgBoxPassports.aspx?action=Message&Parameters=" + encodeURIComponent(ajax.responseText.substr(7, ajax.responseText.length - 7));
                                    parent.ShowMsgBoxForm(url, 500, 300, '');
                                } else if (ajax.responseText.substr(0, 7) == 'INFOMSG') {
                                    var url = "Security/srvMsgBoxPassports.aspx?action=Message&Parameters=" + encodeURIComponent(ajax.responseText.substr(7, ajax.responseText.length - 7));
                                    parent.ShowMsgBoxForm(url, 500, 300, '');
                                }
                            }
                        }
                    }

                    ajax.send(null)
                }

                function RestoreCegidID() {
                    parent.showLoader(true);
                    var stamp = '&StampParam=' + new Date().getMilliseconds();

                    var ajax = nuevoAjax();
                    ajax.open("GET", "../Security/Handlers/srvSupervisorsV3.ashx?action=restoreCegidID&ID=" + document.getElementById('<%= hdnIDEmployee.ClientID %>').value + "&PassportType=E" + stamp, true);

                    ajax.onreadystatechange = function () {
                        if (ajax.readyState == 4) {
                            parent.showLoader(false);
                            if (ajax.responseText == 'OK') {
                                Close()
                            } else {
                                if (ajax.responseText.substr(0, 7) == 'MESSAGE') {
                                    var url = "Security/srvMsgBoxPassports.aspx?action=Message&Parameters=" + encodeURIComponent(ajax.responseText.substr(7, ajax.responseText.length - 7));
                                    parent.ShowMsgBoxForm(url, 500, 300, '');
                                } else if (ajax.responseText.substr(0, 7) == 'INFOMSG') {
                                    var url = "Security/srvMsgBoxPassports.aspx?action=Message&Parameters=" + encodeURIComponent(ajax.responseText.substr(7, ajax.responseText.length - 7));
                                    parent.ShowMsgBoxForm(url, 500, 300, '');
                                }
                            }
                        }
                    }

                    ajax.send(null)
                }

                function SendUsername() {
                    parent.showLoader(true);
                    var stamp = '&StampParam=' + new Date().getMilliseconds();

                    var ajax = nuevoAjax();
                    ajax.open("GET", "../Security/Handlers/srvSupervisorsV3.ashx?action=SendUsername&ID=" + document.getElementById('<%= hdnIDEmployee.ClientID %>').value + "&PassportType=E" + stamp, true);

                    ajax.onreadystatechange = function () {
                        if (ajax.readyState == 4) {
                            parent.showLoader(false);
                            if (ajax.responseText == 'OK') {

                            } else {
                                if (ajax.responseText.substr(0, 7) == 'MESSAGE') {
                                    var url = "Security/srvMsgBoxPassports.aspx?action=Message&Parameters=" + encodeURIComponent(ajax.responseText.substr(7, ajax.responseText.length - 7));
                                    parent.ShowMsgBoxForm(url, 500, 300, '');
                                } else if (ajax.responseText.substr(0, 7) == 'INFOMSG') {
                                    var url = "Security/srvMsgBoxPassports.aspx?action=Message&Parameters=" + encodeURIComponent(ajax.responseText.substr(7, ajax.responseText.length - 7));
                                    parent.ShowMsgBoxForm(url, 500, 300, '');
                                }
                            }
                        }
                    }

                    ajax.send(null)
                }
            </script>
            <div style="width: 100%;">
                <div class="panHeader2">
                    <span style="">
                        <asp:Label ID="lblTitle" runat="server" Text="Metodos de identificación"></asp:Label></span>
                </div>
            </div>
            <div style="background-color: Transparent">
                <table cellpadding="0" cellspacing="0" style="padding-top: 10px;">
                    <tr>
                        <td style="padding-left: 10px;">
                            <span id="lblErrorMsg" style="color: red;" runat="server"></span>
                        </td>
                    </tr>
                    <tr>
                        <td style="height: 435px; padding-left: 8px; padding-top: 5px; vertical-align: top;">
                            <asp:UpdatePanel ID="updEmployeeIdentifyMethods" runat="server">
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="btSave" EventName="Click" />
                                </Triggers>
                                <ContentTemplate>
                                    <asp:HiddenField ID="hdnCanClose_PageBase" runat="server" />
                                    <asp:HiddenField ID="hdnMustRefresh_PageBase" Value="0" runat="server" />
                                    <asp:HiddenField ID="hdnIDEmployee" Value="0" runat="server" />
                                    <div class="RoundCorner" style="width: 1050px; height: auto;">
                                        <roUserControls:IdentifyMethods ID="cnIdentifyMethods" ModoWizardNew="ModeNormal" Type="Employee" runat="server" />
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                    <tr>
                        <td valign="bottom">
                            <table width="95%" border="0">
                                <tr>
                                    <td align="right">
                                        <table>
                                            <tr>
                                                <td>
                                                    <asp:Button ID="btSave" Text="${Button.Accept}" runat="server" OnClientClick="return CheckFields();" CssClass="btnFlat btnFlatBlack btnFlatAsp" /></td>
                                                <td>
                                                    <asp:Button ID="btCancel" Text="${Button.Cancel}" runat="server" OnClientClick="Close(); return false;" CssClass="btnFlat btnFlatBlack btnFlatAsp" /></td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>

                <Local:MessageFrame ID="MessageFrame1" runat="server" />
            </div>
        </div>
    </form>
</body>
</html>