<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Base_EmployeeSelectorPopup" CodeBehind="EmployeeSelectorPopup.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
        function closeSelector() {
            eval(document.getElementById("endEvalFunc").value + "('','','')");

            var oTreeState = getroTreeState(document.getElementById("endEvalCookie").value); 
            oTreeState.clear();

            Close();
        }
        function applySelector() {
            Close();
        }
    </script>
</head>
<body>
    <input type="hidden" runat="server" id="hdnCaptchaAction" />
    <form id="form1" runat="server">
        <div class="popupWizardContent">
            <iframe id="ifEmployeeSelector" runat="server" style="background-color: Transparent" height="350px" width="800px"
                scrolling="no" frameborder="0" marginheight="0" marginwidth="0" src="" />
        </div>
        <div class="popupWizardButtons">
            <table align="right" cellpadding="0" cellspacing="0">
                <tr class="NewShiftGroupWizards_ButtonsPanel" style="height: 44px">
                    <td>
                        <asp:HiddenField ID="endEvalFunc" runat="server" Value="" />
                        <asp:HiddenField ID="endEvalCookie" runat="server" Value="" />
                    </td>
                    <td>&nbsp;
                    </td>
                    <td>
                        <asp:Button ID="btEnd" Text="${Button.Save}" runat="server" OnClientClick="applySelector();" TabIndex="4" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                    </td>
                    <td>
                        <asp:Button ID="btClose" Text="${Button.Clear}" runat="server" OnClientClick="closeSelector();" TabIndex="5" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>