<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Wizards_AccGroupEmployeeWizard" Culture="auto" UICulture="auto" CodeBehind="AccGroupEmployeeWizard.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Asistente para la asignación de empleados a grupos de acceso</title>
</head>
<body class="bodyPopup" style="background-attachment: fixed;">

    <form id="frmAssignEmployeeInGroups" runat="server">

        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <script language="javascript" type="text/javascript">

            var bolLoaded = false;

            async function PageBase_Load() {
                if (!bolLoaded) {
                    await getroTreeState('objContainerTreeV3_treeEmpAssignEmployees').then(roState => roState.reset());
                    await getroTreeState('objContainerTreeV3_treeEmpAssignEmployeesGrid').then(roState => roState.reset());
                }

                var oActiveFrameIndex = document.getElementById('<%= hdnActiveFrame.ClientID %>').value;
                ConvertControls('divStep' + oActiveFrameIndex);

                if (!bolLoaded) bolLoaded = true;

            }

            //Llença aquest script al recarregar els updatepanels per poder controlar per js els opclient
            function endRequestHandler() {
                hidePopupLoader();
            }

            function showPopupLoader() {
                window.parent.frames["ifPrincipal"].showLoadingGrid(true);
            }

            function hidePopupLoader() {
                window.parent.frames["ifPrincipal"].showLoadingGrid(false);
            }

            //-->Obsoleta por el TreeV3
            //function EmployeesSelected(Nodes,strSelected, strSelectedAll){
            //    var hdnSelected = document.getElementById('<%= Me.hdnEmployeesSelected.ClientID %>');
            //    hdnSelected.value = strSelectedAll;
            //}
            function GetSelectedTreeV3(oParm1, oParm2, oParm3) {
                var hdnEmployeesSelected = document.getElementById('<%= me.hdnEmployeesSelected.ClientID %>');
                hdnEmployeesSelected.value = oParm1;

                var hdnFilter = document.getElementById('<%= me.hdnFilter.ClientID %>');
                hdnFilter.value = oParm2;

                var hdnFilterUser = document.getElementById('<%= me.hdnFilterUser.ClientID %>');
                hdnFilterUser.value = oParm3;
            }

            function CheckFrame(intFrameIndex) {
                var bolRet = true;

                if (intFrameIndex == null) {
                    intFrameIndex = parseInt(document.getElementById('<%= hdnActiveFrame.ClientID %>').value);
                }

                if (CheckConvertControls('divStep' + intFrameIndex) == false) {
                    bolRet = false;
                }

                if (bolRet) {

                    if (intFrameIndex == '1') {
                        //Verificar que haya algún empleado seleccionado
                        bolRet = (document.getElementById('<%= hdnEmployeesSelected.ClientID %>').value != '');
                        if (!bolRet) {
                            document.getElementById('<%= lblStep1Error.ClientID %>').textContent = '<%= Me.Language.TranslateJavaScript("CheckPage.IncorrectEmployeesSelected", Me.DefaultScope) %>';
                        }
                        else {
                            document.getElementById('<%= lblStep1Error.ClientID %>').textContent = '';
                        }
                    }

                }

                if (!bolRet) hidePopupLoader();

                return bolRet;
            }

            var oCheckError;
            function CheckFrameAsync(intOldFrameIndex, intActiveFrameIndex) {

                alert(oCheckError);

                var Frames = document.getElementById('<%= hdnFrames.ClientID %>').value.split('*');

                FrameChange(intOldFrameIndex, intActiveFrameIndex, Frames);

                return false;

            }

            function FrameChange(intOldFrameIndex, intActiveFrameIndex, Frames) {

                document.getElementById('<%= hdnActiveFrame.ClientID %>').value = intActiveFrameIndex;

                document.getElementById('divStep' + intOldFrameIndex).style.display = 'none';
                document.getElementById('divStep' + intActiveFrameIndex).style.display = 'block';

                if (intOldFrameIndex == (Frames.length - 1) && intActiveFrameIndex == 0) {
                    document.getElementById('<%= btPrev.ClientID %>').style.display = 'none';
                    document.getElementById('<%= btNext.ClientID %>').style.display = 'none';
                    document.getElementById('<%= btEnd.ClientID %>').style.display = 'none';
                }
                else {
                    if (intActiveFrameIndex > 0) {
                        document.getElementById('<%= btPrev.ClientID %>').style.display = 'block';
                    }
                    else {
                        document.getElementById('<%= btPrev.ClientID %>').style.display = 'none';
                    }
                    if (intActiveFrameIndex < (Frames.length - 1)) {
                        document.getElementById('<%= btNext.ClientID %>').style.display = 'block';
                    }
                    else {
                        document.getElementById('<%= btNext.ClientID %>').style.display = 'none';
                    }
                    if (intActiveFrameIndex == (Frames.length - 1)) {
                        document.getElementById('<%= btEnd.ClientID %>').style.display = 'block';
                    }
                    else {
                        document.getElementById('<%= btEnd.ClientID %>').style.display = 'none';
                    }
                }

            }
        </script>

        <div class="popupWizardContent">

            <asp:UpdatePanel ID="updStep0" runat="server" RenderMode="Inline">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btPrev" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="btNext" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="btEnd" EventName="Click" />
                </Triggers>
                <ContentTemplate>

                    <%-- WELCOME --%>
                    <div id="divStep0" runat="server" style="display: block;">
                        <table id="tbStep0" style="" cellpadding="0" cellspacing="0">
                            <tr>
                                <td style="height: 360px">
                                    <asp:Image ID="imgNewMultiEmployeeWizard" runat="server" Style="border-radius: 5px;" ImageUrl="~/Base/Images/Wizards/wzmens.gif" />
                                </td>
                                <td style="padding-left: 20px; padding-right: 20px; padding-top: 50px" valign="top">

                                    <asp:Label ID="lblWelcome1" runat="server" Text="Bienvenido al asistente para transferir empleados al grupo seleccionado."
                                        Font-Bold="True" Font-Size="Large"></asp:Label>
                                    <br />
                                    <br />
                                    <br />
                                    <asp:Label ID="lblWelcome2" runat="server" Text="Este asistente le ayudará a asignar los empleados a este grupo de acceso. Un empleado solamente puede estar en un grupo de acceso. La asignación de un empleado a un grupo de acceso implica que si anteriormente estuviera en otro, este dejaría de pertenecer a ese grupo de acceso." Font-Bold="true"></asp:Label>
                                    <br />
                                    <br />
                                    <asp:Label ID="lblWelcome3" runat="server" Text="Para continuar, haga clic en siguiente."></asp:Label>
                                    <textarea id="txtErrors" runat="server" class="textClass" style="height: 200px; width: 300px; color: Red;" visible="false"></textarea>
                                </td>
                            </tr>
                            <tr>
                                <td style="height: 10px" colspan="2" class="NewEmployeeWizards_ButtonsPanel"></td>
                            </tr>
                        </table>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>

            <asp:Label ID="hdnStepTitle" Text="Asistente para la asignación de empleados a grupos de acceso. " runat="server" Style="display: none; visibility: hidden" />
            <asp:Label ID="hdnStepTitle2" Text="Paso {0} de {1}." runat="server" Style="display: none; visibility: hidden" />
            <asp:Label ID="hdnSetpInfo" Text="Seleccione los empleados o grupos que desea asignar al grupo de acceso." runat="server" Style="display: none; visibility: hidden" />

            <asp:UpdatePanel ID="updStep1" runat="server" RenderMode="Inline">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btPrev" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="btNext" EventName="Click" />
                </Triggers>
                <ContentTemplate>

                    <div id="divStep1" runat="server" style="display: none">
                        <table style="" cellspacing="0" cellpadding="0">
                            <tr>
                                <td class="NewEmployeeWizards_StepTitle">
                                    <table style="width: 100%">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblStep1Title" runat="server" Text="" Font-Bold="True" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="padding-left: 20px; padding-right: 50px;">
                                                <asp:Label ID="lblStep1Info" runat="server" Text="" />
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
                                <td class="NewEmployeeWizards_StepContent">

                                    <table style="width: 100%; height: 100%;" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblStep1Info2" runat="server" Text="Ahora seleccione los ${Employees} que desea asignar al grupo de acceso." />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="padding-top: 3px" valign="top">

                                                <div style="height: 100%;">
                                                    <input type="hidden" id="hdnEmployeesSelected" runat="server" value="" />
                                                    <input type="hidden" id="hdnFilter" runat="server" value="" />
                                                    <input type="hidden" id="hdnFilterUser" runat="server" value="" />
                                                    <iframe id="ifEmployeesSelector" runat="server" style="background-color: Transparent" height="290" width="100%"
                                                        scrolling="no" frameborder="0" marginheight="0" marginwidth="0" src="" />
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>

            <asp:UpdatePanel ID="updStep2" runat="server" RenderMode="Inline">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btPrev" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="btNext" EventName="Click" />
                </Triggers>
                <ContentTemplate>

                    <div id="divStep2" runat="server" style="display: none;">
                        <table style="" cellspacing="0" cellpadding="0">
                            <tr>
                                <td class="NewEmployeeWizards_StepTitle">
                                    <table style="width: 100%">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblStep2Title" runat="server" Text="" Font-Bold="True" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="padding-left: 20px; padding-right: 50px;">
                                                <asp:Label ID="lblStep2Info" runat="server" Text="" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td class="NewEmployeeWizards_StepError popupWizardError">
                                    <asp:Label ID="lblStep2Error" runat="server" CssClass="errorText" />
                                </td>
                            </tr>
                            <tr>
                                <td class="NewEmployeeWizards_StepContent">

                                    <table style="width: 100%;" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblStep2Info2" runat="server" Text="El asistente ya dispone de suficientes datos. Pulse Finalizar para realizar la asignación." />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>

        <div class="popupWizardButtons">

            <asp:UpdatePanel ID="updButtons" runat="server" RenderMode="Inline">
                <ContentTemplate>

                    <table align="right" cellpadding="0" cellspacing="0">
                        <tr class="NewEmployeeWizards_ButtonsPanel" style="height: 44px">
                            <td>&nbsp
                            </td>
                            <td>
                                <asp:Button ID="btPrev" Text="${Button.Previous}" runat="server" OnClientClick="showPopupLoader();" Visible="false" TabIndex="1" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                            </td>
                            <td>
                                <asp:Button ID="btNext" Text="${Button.Next}" runat="server" OnClientClick="showPopupLoader();return CheckFrame();" TabIndex="2" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                <asp:Button ID="btEnd" Text="${Button.End}" runat="server" Visible="false" TabIndex="4" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                            </td>
                            <td>
                                <asp:Button ID="btClose" Text="${Button.Cancel}" runat="server" OnClientClick="Close(); return false;" TabIndex="5" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                <asp:HiddenField ID="hdnCanClose_PageBase" runat="server" />
                                <asp:HiddenField ID="hdnMustRefresh_PageBase" Value="0" runat="server" />
                                <asp:HiddenField ID="hdnParams_PageBase" runat="server" />
                            </td>
                        </tr>
                    </table>

                    <input type="hidden" id="hdnActiveFrame" value="0" runat="server" />
                    <input type="hidden" id="hdnIDEmployeeSource" value="" runat="server" />
                    <input type="hidden" id="hdnFrames" value="" runat="server" />
                    <input type="hidden" id="hdnFramesOnlyClient" value="" runat="server" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </form>
</body>
</html>