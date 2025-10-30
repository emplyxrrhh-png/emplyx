<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Wizards_CardAssignWizard" CodeBehind="CardAssignWizard.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Asistente para asignar códigos de ${Card}</title>
</head>
<body class="bodyPopup" style="background-attachment: fixed;">

    <form id="frmCardAssignWizard" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />
        <div>

            <script language="javascript" type="text/javascript">

                function PageBase_Load() {

                }

                function cargaEmp(Nodo) {
                    var hdnEmp = document.getElementById('<%= Me.hdnEmpToCopy.ClientID %>');
                    hdnEmp.value = Nodo.id;
                }

                function endRequestHandler() {
                    hidePopupLoader();
                }

                function showPopupLoader() {

                    var bolFound = false;
                    var curFrame = window;
                    while (bolFound == false) {
                        if (typeof (curFrame.frames["ifPrincipal"]) != "undefined") {
                            curFrame.frames["ifPrincipal"].parent.showLoadingGrid(true);
                            bolFound = true;
                        }

                        if (curFrame == window.top) {
                            bolFound = true;
                        } else {
                            curFrame = curFrame.parent;
                        }
                    }

                    //if (typeof (window.frames["ifPrincipal"]) != "undefined") {
                    //    window.frames["ifPrincipal"].showLoadingGrid(true);
                    //} else {
                    //    if (typeof (window.parent.frames["ifPrincipal"]) != "undefined") {
                    //        window.parent.frames["ifPrincipal"].showLoadingGrid(true);
                    //    } else {
                    //        if (typeof (window.parent.parent.frames["ifPrincipal"]) != "undefined") {
                    //            window.parent.parent.frames["ifPrincipal"].showLoadingGrid(true);
                    //        } else {
                    //            window.parent.parent.parent.frames["ifPrincipal"].showLoadingGrid(true);
                    //        }
                    //    }
                    //}
                }

                function hidePopupLoader() {
                    var bolFound = false;
                    var curFrame = window;
                    while (bolFound == false) {
                        if (typeof (curFrame.frames["ifPrincipal"]) != "undefined") {
                            curFrame.frames["ifPrincipal"].parent.showLoadingGrid(false);
                            bolFound = true;
                        }

                        if (curFrame == window.top) {
                            bolFound = true;
                        } else {
                            curFrame = curFrame.parent;
                        }
                    }

                    //if (typeof (window.frames["ifPrincipal"]) != "undefined") {
                    //    window.frames["ifPrincipal"].showLoadingGrid(false);
                    //} else {
                    //    if (typeof (window.parent.frames["ifPrincipal"]) != "undefined") {
                    //        window.parent.frames["ifPrincipal"].showLoadingGrid(false);
                    //    } else {
                    //        if (typeof (window.parent.parent.frames["ifPrincipal"]) != "undefined") {
                    //            window.parent.parent.frames["ifPrincipal"].showLoadingGrid(false);
                    //        } else {
                    //            window.parent.parent.parent.frames["ifPrincipal"].showLoadingGrid(false);
                    //        }
                    //    }
                    //}
                }
            </script>

            <asp:UpdatePanel ID="upCardAssignWizard" runat="server" RenderMode="Inline">
                <ContentTemplate>

                    <div class="popupWizardContent">
                        <%-- WELCOME --%>
                        <div id="divStep0" runat="server" style="display: block">
                            <table id="tbStep0" style="width: 100%; height: 100%;" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td style="height: 360px">
                                        <asp:Image ID="imgWelcome" runat="server" Style="border-radius: 5px;" ImageUrl="~/Base/Images/Wizards/wzRegCard.gif" />
                                    </td>
                                    <td style="padding-left: 20px; padding-right: 20px; padding-top: 50px" valign="top">

                                        <asp:Label ID="lblWelcome1" runat="server" Text="Bienvenido al asistente para asignar códigos de ${Card}."
                                            Font-Bold="True" Font-Size="Large"></asp:Label>
                                        <br />
                                        <br />
                                        <br />
                                        <asp:Label ID="lblWelcome2" runat="server" Text="Este asistente le permite asignar la ${Card} {0} a un ${Employee}." Font-Bold="true"></asp:Label>
                                        <br />
                                        <br />
                                        <asp:Label ID="lblWelcome3" runat="server" Text="Para continuar, haga clic en siguiente."></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="height: 10px" colspan="2" class="CardAssignWizards_ButtonsPanel"></td>
                                </tr>
                            </table>
                        </div>

                        <asp:Label ID="hdnStepTitle" Text="Asistente para asignar códigos de ${Card}. " runat="server" Style="display: none; visibility: hidden" />

                        <div id="DivStep1" runat="server" style="display: none">
                            <table id="btStep1" style="width: 100%; height: 100%" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td class="CardAssignWizards_StepTitle">
                                        <table style="width: 100%">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblStep1Title" runat="server" Text="Paso 1 de 1." Font-Bold="True" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 20px">
                                                    <asp:Label ID="lblStep1Info" runat="server" Text="Ahora seleccione el ${Employee} al que desea asignar la ${Card}." />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="NewEmployeeWizards_StepError popupWizardError">
                                        <asp:Label ID="lblStep1Error" runat="server" CssClass="errorText" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="CardAssignWizards_StepContent" style="padding-top: 10px; width: 490px;">
                                        <div style="width: 100%; height: 240px; text-align: center; padding-left: 1%; z-index: 9999999;">
                                            <asp:Label ID="lblStep1Info2" runat="server" Text="Ahora seleccione el ${Employee} al que desea asignar la ${Card} seleccionada." /><br />
                                            <br />
                                            <br />
                                            <iframe id="ifEmployeeSelector" runat="server" src="" width="100%" height="290px"
                                                scrolling="no" frameborder="0" marginheight="0" marginwidth="0" style="display: block; width: 100%; height: 290px;" />
                                        </div>
                                        <asp:HiddenField ID="hdnEmpToCopy" runat="server" Value="" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>

                    <div class="popupWizardButtons">
                        <table align="right" cellpadding="0" cellspacing="0">
                            <tr class="NewEmployeeWizards_ButtonsPanel" style="height: 44px">
                                <td>&nbsp
                                </td>
                                <td>
                                    <asp:Button ID="btPrev" Text="${Button.Previous}" runat="server" OnClientClick="showPopupLoader();" Visible="false" TabIndex="1" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                </td>
                                <td>
                                    <asp:Button ID="btNext" Text="${Button.Next}" runat="server" OnClientClick="showPopupLoader();" TabIndex="2" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                    <asp:Button ID="btEnd" Text="${Button.End}" runat="server" Visible="false" TabIndex="4" OnClientClick="showPopupLoader();" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                </td>
                                <td>
                                    <asp:Button ID="btClose" Text="${Button.Cancel}" runat="server" OnClientClick="Close(); return false;" TabIndex="5" CssClass="btnFlat btnFlatBlack btnFlatAsp" />

                                    <asp:HiddenField ID="hdnCanClose_PageBase" runat="server" />
                                    <asp:HiddenField ID="hdnMustRefresh_PageBase" Value="0" runat="server" />
                                </td>
                            </tr>
                        </table>
                    </div>

                    <Local:MessageFrame ID="MessageForm" runat="server" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </form>
</body>
</html>