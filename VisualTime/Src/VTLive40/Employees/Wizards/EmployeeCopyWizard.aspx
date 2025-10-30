<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Wizards_EmployeeCopyWizard" Culture="auto" UICulture="auto" EnableEventValidation="false" CodeBehind="EmployeeCopyWizard.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Asistente para la copia masica de datos</title>
</head>
<body class="bodyPopup" style="background-attachment: fixed;">

    <form id="frmEmployeeCopyWizard" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <input type="hidden" id="hdnUserFields" runat="server" />
        <input type="hidden" id="hdnUserFieldsHistory" runat="server" />
        <input type="hidden" id="hdnAssignments" runat="server" />
        <input type="hidden" id="hdnCenters" runat="server" />
        <input type="hidden" id="hdnActivities" runat="server" />

        <div>

            <script language="javascript" type="text/javascript">

                var bolLoaded = false;

                async function PageBase_Load() {
                    if (!bolLoaded) {
                        await getroTreeState('objContainerTreeV3_treeEmpEmployeeCopyWizard').then(roState => roState.reset());
                        await getroTreeState('objContainerTreeV3_treeEmpEmployeeCopyWizardGrid').then(roState => roState.reset());
                    }

                    var oActiveFrameIndex = document.getElementById('<%= hdnActiveFrame.ClientID %>').value;
                    ConvertControls('divStep' + oActiveFrameIndex);

                    checkOPCPanelClients();

                    if (!bolLoaded) bolLoaded = true;
                }

                function zas() {
                    var chkButton = document.getElementById("chkUserFields_chkButton");
                    var arrChks = getElementsByPartialName("chkUserFields", document.getElementById('chkUserFields_panUserFields'));

                    var arrCollection = null;
                    try {
                        arrCollection = ASPxClientControl.GetControlCollection();
                    }
                    catch (e) { }

                    for (var n = 0; n < arrChks.length; n++) {
                        var strFieldName = arrChks[n].id;
                        strFieldName = strFieldName.replace("chkUserFields_", "");
                        strFieldName = strFieldName.substring(3);

                        //combos DevExpress de tipo History
                        if (arrCollection != null) {
                            var clientObject = arrCollection.GetByName("cmbDevClient_" + strFieldName);
                            if (clientObject != null) {
                                clientObject.SetEnabled(chkButton.checked && arrChks[n].checked);
                            }
                        }
                    }
                }

                function checkOPCPanelClients() {
                    venableOPC('<%= chkUserFields.ClientID %>');
                    venableOPC('<%= chkTerminalQuerys.ClientID %>');
                    venableOPC('<%= chkTerminalSol.ClientID %>');
                    venableOPC('<%= chkTerminalPeriods.ClientID %>');
                    venableOPC('<%= chkTerminalMessages.ClientID %>');
                    venableOPC('<%= chkConceptsData.ClientID %>');
                    venableOPC('<%= chkConceptsDataDetail.ClientID %>');
                    venableOPC('<%= chkSchedule.ClientID %>');
                    venableOPC('<%= chkRules.ClientID %>');
                    venableOPC('<%= chkRulesDetail.ClientID %>');
                    venableOPC('<%= chkAssignments.ClientID %>');
                    venableOPC('<%= chkCenters.ClientID%>');

                }

                //Llença aquest script al recarregar els updatepanels per poder controlar per js els opclient
                function endRequestHandler() {
                    checkOPCPanelClients();
                    hidePopupLoader();
                }

                function showPopupLoader() {
                    window.parent.frames["ifPrincipal"].showLoadingGrid(true);
                }

                function hidePopupLoader() {
                    window.parent.frames["ifPrincipal"].showLoadingGrid(false);
                }

            //-->Obsoleta por el TreeV3
            //function EmployeesSelected(Nodes, strSelected, strSelectedAll) {
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

                function NextFrame() {

                    var intActiveFrameIndex = parseInt(document.getElementById('<%= hdnActiveFrame.ClientID %>').value);
                    var Frames = document.getElementById('<%= hdnFrames.ClientID %>').value.split('*');
                    var FramesOnlyClient = document.getElementById('<%= hdnFramesOnlyClient.ClientID %>').value.split('*');

                    if (CheckFrame(intActiveFrameIndex)) {

                        var intOldFrameIndex = intActiveFrameIndex;
                        if (Frames.length > (intActiveFrameIndex + 1)) {
                            intActiveFrameIndex = intActiveFrameIndex + 1;
                        }

                        AsyncCall('GET', 'EmployeeCopyWizard.aspx?action=CheckFrame&FrameIndex=' + intFrameIndex, 'JSON', 'oCheckError', 'CheckFrameAsync(' + intOldFrameIndex + ',' + intActiveFrameIndex + ');');
                        bolRet = false;
                    }

                    if (bolRet) {

                        if (FramesOnlyClient[intActiveFrameIndex] == '1') {

                            var intOldFrameIndex = intActiveFrameIndex;
                            if (Frames.length > (intActiveFrameIndex + 1)) {
                                intActiveFrameIndex = intActiveFrameIndex + 1;
                            }

                            FrameChange(intOldFrameIndex, intActiveFrameIndex, Frames);

                            bolRet = false;

                        }
                        else {
                            bolRet = true;
                        }

                    }
                    else {
                        bolRet = false;
                    }

                    if (!bolRet) hidePopupLoader();

                    return bolRet;
                }

                function PrevFrame() {

                    var intActiveFrameIndex = parseInt(document.getElementById('<%= hdnActiveFrame.ClientID %>').value);
                    var Frames = document.getElementById('<%= hdnFrames.ClientID %>').value.split('*');
                    var FramesOnlyClient = document.getElementById('<%= hdnFramesOnlyClient.ClientID %>').value.split('*');

                    //if (FramesOnlyClient[intActiveFrameIndex] == '1') {

                    var intOldFrameIndex = intActiveFrameIndex;
                    intActiveFrameIndex = intActiveFrameIndex - 1;

                    FrameChange(intOldFrameIndex, intActiveFrameIndex, Frames);

                    return false;

                    /*}
                    else {
                        return true;
                    }*/

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
                        if (intFrameIndex == '2') {
                            //Verificar que haya algún empleado seleccionado
                            bolRet = (document.getElementById('<%= hdnEmployeesSelected.ClientID %>').value != '');
                        if (!bolRet) {
                            document.getElementById('<%= lblStep1Error.ClientID %>').innerHTML = '<%= Me.Language.TranslateJavascript("CheckPage.IncorrectEmployeesSelected", Me.DefaultScope) %>';
                        }
                        else {
                            document.getElementById('<%= lblStep1Error.ClientID %>').innerHTML = '';
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

                function checkAll() {
                    var arrChks = new Array();
                    arrChks = getElementsByPartialName("chkUserFields", document.getElementById('chkUserFields_panUserFields'));

                    for (var n = 0; n < arrChks.length; n++) {
                        arrChks[n].checked = document.getElementById('chkAllUserFields').checked;
                    }
                    retrieveChecks();
                }

                function checkAllAssignments() {
                    var arrChks = new Array();
                    arrChks = getElementsByPartialName("chkAssignments", document.getElementById('chkAssignments_panAssignments'));

                    for (var n = 0; n < arrChks.length; n++) {
                        arrChks[n].checked = document.getElementById('chkAllAssignments').checked;
                    }
                    retrieveChecksAssignments();
                }

                function checkAllCenters() {
                    var arrChks = new Array();
                    arrChks = getElementsByPartialName("chkCenters", document.getElementById('chkCenters_panCenters'));

                    for (var n = 0; n < arrChks.length; n++) {
                        arrChks[n].checked = document.getElementById('chkAllCenters').checked;
                    }
                    retrieveChecksCenters();
                }

                function retrieveChecksAssignments() {
                    try {
                        var arrChks = new Array();
                        arrChks = getElementsByPartialName("chkAssignments", document.getElementById('chkAssignments_panAssignments'));

                        var hdnAssignments = document.getElementById('<%= hdnAssignments.ClientID %>');

                        var arrAssignments = "";

                        for (var n = 0; n < arrChks.length; n++) {
                            if (arrChks[n].checked) {
                                var strAssignmentName = arrChks[n].id;
                                strAssignmentName = strAssignmentName.replace("chkAssignments_", "");
                                strAssignmentName = strAssignmentName.substring(3);
                                arrAssignments += strAssignmentName + ",";

                            } //end if
                        } //end for

                        if (arrAssignments != "") { arrAssignments = arrAssignments.substr(0, arrAssignments.length - 1); }

                        hdnAssignments.value = arrAssignments;
                    } catch (e) { showError("retrieveChecksAssignments", e); } //end try
                }

                function retrieveChecksCenters() {
                    try {
                        var arrChks = new Array();
                        arrChks = getElementsByPartialName("chkCenters", document.getElementById('chkCenters_panCenters'));

                        var hdnCenters = document.getElementById('<%= hdnCenters.ClientID%>');

                        var arrCenters = "";

                        for (var n = 0; n < arrChks.length; n++) {
                            if (arrChks[n].checked) {
                                var strCenterName = arrChks[n].id;
                                strCenterName = strCenterName.replace("chkCenters_", "");
                                strCenterName = strCenterName.substring(3);
                                arrCenters += strCenterName + "@";

                            } //end if
                        } //end for

                        if (arrCenters != "") { arrCenters = arrCenters.substr(0, arrCenters.length - 1); }

                        hdnCenters.value = arrCenters;
                    } catch (e) { showError("retrieveChecksCenters", e); } //end try
                }

                function retrieveChecks() {
                    try {
                        var arrChks = new Array();
                        arrChks = getElementsByPartialName("chkUserFields", document.getElementById('chkUserFields_panUserFields'));

                        var hdnUserFields = document.getElementById('<%= hdnUserFields.ClientID %>');
                        var hdnUserFieldsHistory = document.getElementById('<%= hdnUserFieldsHistory.ClientID  %>');

                        var arrUserFields = "";
                        var arrUserFieldsHistory = "";

                        var arrCollection = null;
                        try {
                            arrCollection = ASPxClientControl.GetControlCollection();
                        }
                        catch (e) { }

                        for (var n = 0; n < arrChks.length; n++) {

                            var strFieldName = arrChks[n].id;
                            strFieldName = strFieldName.replace("chkUserFields_", "");
                            strFieldName = strFieldName.substring(3);

                            if (arrChks[n].checked == true) {
                                arrUserFields += strFieldName + ",";
                            }

                            //combos history?
                            //var cmbHistory = document.getElementById('chkUserFields_cmb' + strFieldName + '_ComboBoxLabel');
                            //if (cmbHistory != null) {
                            //    arrUserFieldsHistory += strFieldName + "=" + cmbHistory.getAttribute("value") + ",";
                            //}

                            //combos DevExpress de tipo History
                            if (arrCollection != null) {
                                var clientObject = arrCollection.GetByName("cmbDevClient_" + strFieldName);
                                if (clientObject != null) {
                                    clientObject.SetEnabled(arrChks[n].checked);
                                    if (arrChks[n].checked == true) {
                                        var tmpValue = clientObject.GetValue();
                                        tmpValue = (tmpValue == 0 ? "" : tmpValue);
                                        arrUserFieldsHistory += strFieldName + "=" + tmpValue + ",";
                                    }
                                }
                            }

                        }

                        if (arrUserFields != "") { arrUserFields = arrUserFields.substr(0, arrUserFields.length - 1); }
                        if (arrUserFieldsHistory != "") { arrUserFieldsHistory = arrUserFieldsHistory.substr(0, arrUserFieldsHistory.length - 1); }

                        hdnUserFields.value = arrUserFields;
                        hdnUserFieldsHistory.value = arrUserFieldsHistory;
                    } catch (e) { showError("retrieveChecks", e); } //end try
                }

                function devKeyPress(s, e) {
                    return onEnterPress(e, 'if (CheckFrame() == true) {__doPostBack(\'btNext\',\'\');}');
                }

                var monitor = -1;

                function showCaptcha() {
                    var contentUrl = "../../Base/Popups/GenericCaptchaValidator.aspx?Action=COPYPLAN";
                    CaptchaObjectPopup_Client.SetContentUrl(contentUrl);
                    CaptchaObjectPopup_Client.Show();
                }

                function captchaCallback(action) {
                    switch (action) {
                        case "COPYPLAN":
                            AspxLoadingPopup_Client.Show();
                            PerformAction();
                            break;
                        case "ERROR":
                            window.parent.frames["ifPrincipal"].showErrorPopup("Error.ValidationFailed", "ERROR", "Error.ValidationFailedDesc", "Error.OK", "Error.OKDesc", "");
                            break;
                    }
                }

                function PerformValidation() {
                    PerformActionCallbackClient.PerformCallback("VALIDATE");
                }

                function PerformAction() {
                    PerformActionCallbackClient.PerformCallback("PERFORM_ACTION");
                }

                function PerformActionCallback_CallbackComplete(s, e) {
                    if (s.cpAction == "VALIDATE" && s.cpResult == true) {
                        showCaptcha();
                    } else if (s.cpAction == "PERFORM_ACTION") {
                        monitor = setInterval(function () { PerformActionCallbackClient.PerformCallback("CHECKPROGRESS"); }, 5000);
                    } else if (s.cpAction == "ERROR") {
                        clearInterval(monitor);
                        AspxLoadingPopup_Client.Hide();
                        __doPostBack('<%= btResume.ClientID%>', '');
                    } else if (s.cpAction == "CHECKPROGRESS") {
                        if (s.cpActionResult == "OK") {
                            clearInterval(monitor);
                            AspxLoadingPopup_Client.Hide();
                            __doPostBack('<%= btResume.ClientID%>', '');
                        }
                    }

                }
            </script>

            <table style="width: 800px; display: block;" cellpadding="0" cellspacing="0">
                <tr>
                    <td style="border: none 1px black;">

                        <dx:ASPxCallback ID="PerformActionCallback" runat="server" ClientInstanceName="PerformActionCallbackClient" ClientSideEvents-CallbackComplete="PerformActionCallback_CallbackComplete"></dx:ASPxCallback>

                        <asp:UpdatePanel ID="updStep0" runat="server" RenderMode="Inline">
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="btPrev" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="btNext" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="btEnd" EventName="Click" />
                            </Triggers>
                            <ContentTemplate>

                                <%-- WELCOME --%>
                                <div id="divStep0" runat="server" style="display: block;">
                                    <table id="tbStep0" style="width: 100%;" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td style="height: 430px">
                                                <asp:Image ID="imgNewMultiEmployeeWizard" runat="server" Style="border-radius: 5px;" ImageUrl="~/Base/Images/Wizards/wzmens.gif" />
                                            </td>
                                            <td style="padding-left: 20px; padding-right: 20px; padding-top: 50px" valign="top">

                                                <asp:Label ID="lblWelcome1" runat="server" Text="Bienvenido al asistente para copia masiva de datos."
                                                    Font-Bold="True" Font-Size="Large"></asp:Label>
                                                <br />
                                                <br />
                                                <br />
                                                <asp:Label ID="lblWelcome2" runat="server" Text="Este asistente le permite realizar la copia masiva de datos a partir del ${Employee} '{1}' y uno o más ${Employees} destino." Font-Bold="true"></asp:Label>
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

                        <asp:Label ID="hdnStepTitle" Text="Asistente para la copia masiva de datos. " runat="server" Style="display: none; visibility: hidden" />
                        <asp:Label ID="hdnStepTitle2" Text="Paso {0} de {1}." runat="server" Style="display: none; visibility: hidden" />
                        <asp:Label ID="hdnSetpInfo" Text="Este asistente le permite copiar información u opciones que ha aplicado a un ${Employee} a otros ${Employees} que seleccione para evitar trabajo manual." runat="server" Style="display: none; visibility: hidden" />

                        <asp:UpdatePanel ID="updStep1" runat="server" RenderMode="Inline">
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="btPrev" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="btNext" EventName="Click" />
                            </Triggers>
                            <ContentTemplate>

                                <div id="divStep1" runat="server" style="display: none; width: 800px;">
                                    <table style="width: 790px; height: 500px;" cellspacing="0" cellpadding="0">
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
                                            <td class="NewEmployeeWizards_StepContent">

                                                <table style="width: 100%;" cellspacing="0" cellpadding="0">
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lblStep1Info2" runat="server" Text="Seleccione el empleado del que desea copiar los datos origen." />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding-left: 10px; padding-top: 5px" valign="top">

                                                            <content>
                                                                <div style="padding-left: 20px">
                                                                    <dx:ASPxComboBox ID="cmbEmployeeOrigin" runat="server" Width="400px" ClientInstanceName="cmbEmployeeOriginClient">
                                                                        <ClientSideEvents SelectedIndexChanged="function(s,e){}" />
                                                                    </dx:ASPxComboBox>
                                                                </div>
                                                            </content>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="NewEmployeeWizards_StepError">
                                                <asp:Label ID="lblStep1Error" runat="server" CssClass="errorText" />
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

                                <div id="divStep2" runat="server" style="display: none; width: 800px;">
                                    <table style="width: 790px; height: 500px;" cellspacing="0" cellpadding="0">
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
                                            <td class="NewEmployeeWizards_StepContent">

                                                <table style="width: 100%; height: 100%;" cellspacing="0" cellpadding="0">
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lblStep2Info2" runat="server" Text="Ahora seleccione los ${Employees} sobre los que desea copiar los datos del ${Employee} origen." />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding-left: 10px; padding-top: 5px" valign="top">

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
                                        <tr>
                                            <td class="NewEmployeeWizards_StepError">
                                                <asp:Label ID="lblStep2Error" runat="server" CssClass="errorText" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>

                        <asp:UpdatePanel ID="updStep3" runat="server" RenderMode="Inline">
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="btPrev" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="btNext" EventName="Click" />
                            </Triggers>
                            <ContentTemplate>

                                <div id="divStep3" runat="server" style="display: none; width: 800px;">
                                    <table style="width: 790px; height: 500px;" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td class="NewEmployeeWizards_StepTitle">
                                                <table style="width: 100%">
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lblStep3Title" runat="server" Text="" Font-Bold="True" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding-left: 20px; padding-right: 50px;">
                                                            <asp:Label ID="lblStep3Info" runat="server" Text="" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="NewEmployeeWizards_StepContent">

                                                <table style="width: 100%; height: 100%;" cellspacing="0" cellpadding="0">
                                                    <tr>
                                                        <td valign="top">
                                                            <asp:Label ID="lblStep3Info2" runat="server" Text="Indique si desea copiar datos de la ficha del ${Employee} origen." />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" style="padding-top: 5px; padding-top: 5px;" valign="top">
                                                            <roUserControls:roOptionPanelClient ID="chkUserFields" runat="server" TypeOPanel="CheckboxOption" ClientScript="zas();" Width="100%" Height="250" Enabled="True">
                                                                <Title>
                                                                    <asp:Label ID="lblUserFieldsTitle" runat="server" Text="Copiar campos de la ficha del ${Employee}"></asp:Label>
                                                                </Title>
                                                                <Description>
                                                                </Description>
                                                                <Content>
                                                                    <table width="100%" style="padding-left: 20px; padding-top: 10px;">
                                                                        <tr>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblUserFieldsList" runat="server" Text="¿Qué campos desea copiar?"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td align="left">
                                                                                <asp:Panel ID="panUserFields" Height="150" ScrollBars="Vertical" BackColor="white" runat="server"></asp:Panel>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td style="padding-top: 10px;">
                                                                                <table>
                                                                                    <tr>
                                                                                        <td>
                                                                                            <input type="checkbox" id="chkAllUserFields" onclick="checkAll();" />
                                                                                        </td>
                                                                                        <td>
                                                                                            <a href="javascript: void(0);" onclick="CheckLinkClick('chkAllUserFields');">
                                                                                                <asp:Label ID="lblchkAllUserFields" runat="server" Text="Seleccionar todos los campos de la ficha"></asp:Label>
                                                                                            </a>
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </Content>
                                                            </roUserControls:roOptionPanelClient>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="NewEmployeeWizards_StepError">
                                                <asp:Label ID="lblStep3Error" runat="server" CssClass="errorText" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>

                        <asp:UpdatePanel ID="updStep4" runat="server" RenderMode="Inline">
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="btPrev" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="btNext" EventName="Click" />
                            </Triggers>
                            <ContentTemplate>

                                <div id="divStep4" runat="server" style="display: none; width: 800px;">
                                    <table style="width: 790px; height: 500px;" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td class="NewEmployeeWizards_StepTitle">
                                                <table style="width: 100%">
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lblStep4Title" runat="server" Text="" Font-Bold="True" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding-left: 20px; padding-right: 50px;">
                                                            <asp:Label ID="lblStep4Info" runat="server" Text="" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="NewEmployeeWizards_StepContent">

                                                <table style="width: 100%;" cellspacing="0" cellpadding="0">
                                                    <tr>
                                                        <td valign="top">
                                                            <asp:Label ID="lblStep4Info2" runat="server" Text="Seleccione la información relativa a la operativa desde ${Terminals} que desea copiar." />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding-top: 5px; padding-top: 20px;">
                                                            <roUserControls:roOptionPanelClient ID="chkTerminalQuerys" runat="server" TypeOPanel="CheckboxOption" Width="100%" Height="Auto" Enabled="True">
                                                                <Title>
                                                                    <asp:Label ID="lblTerminalQuerysTitle" runat="server" Text="Copiar permisos de consultas por el ${Terminal}"></asp:Label>
                                                                </Title>
                                                                <Description>
                                                                    <asp:Label ID="lblTerminalQuerysDescription" runat="server" Text=""></asp:Label>
                                                                </Description>
                                                                <Content>
                                                                </Content>
                                                            </roUserControls:roOptionPanelClient>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding-top: 5px; padding-top: 10px;">
                                                            <roUserControls:roOptionPanelClient ID="chkTerminalSol" runat="server" TypeOPanel="CheckboxOption" Width="100%" Height="Auto" Enabled="True">
                                                                <Title>
                                                                    <asp:Label ID="lblTerminalSolTitle" runat="server" Text="Copiar permisos de solicitudes por el ${Terminal}"></asp:Label>
                                                                </Title>
                                                                <Description>
                                                                    <asp:Label ID="lblTerminalSolDescription" runat="server" Text=""></asp:Label>
                                                                </Description>
                                                                <Content>
                                                                </Content>
                                                            </roUserControls:roOptionPanelClient>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding-top: 5px; padding-top: 10px;">
                                                            <roUserControls:roOptionPanelClient ID="chkTerminalPeriods" runat="server" TypeOPanel="CheckboxOption" Width="100%" Height="Auto" Enabled="True">
                                                                <Title>
                                                                    <asp:Label ID="lblTerminalPeriodsTitle" runat="server" Text="Copiar períodos de consultas y solicitudes por el ${Terminal}"></asp:Label>
                                                                </Title>
                                                                <Description>
                                                                    <asp:Label ID="lblTerminalPeriodsDescription" runat="server" Text=""></asp:Label>
                                                                </Description>
                                                                <Content>
                                                                </Content>
                                                            </roUserControls:roOptionPanelClient>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding-top: 5px; padding-top: 10px;">
                                                            <roUserControls:roOptionPanelClient ID="chkTerminalMessages" runat="server" TypeOPanel="CheckboxOption" Width="100%" Height="Auto" Enabled="True">
                                                                <Title>
                                                                    <asp:Label ID="lblTerminalMessagesTitle" runat="server" Text="Copiar mensajes a través del ${Terminal}"></asp:Label>
                                                                </Title>
                                                                <Description>
                                                                    <asp:Label ID="lblTerminalMessagesDescription" runat="server" Text=""></asp:Label>
                                                                </Description>
                                                                <Content>
                                                                </Content>
                                                            </roUserControls:roOptionPanelClient>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="NewEmployeeWizards_StepError">
                                                <asp:Label ID="lblStep4Error" runat="server" CssClass="errorText" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>

                        <asp:UpdatePanel ID="updStep5" runat="server" RenderMode="Inline">
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="btPrev" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="btNext" EventName="Click" />
                            </Triggers>
                            <ContentTemplate>

                                <div id="divStep5" runat="server" style="display: none; width: 800px;">
                                    <table style="width: 790px; height: 500px;" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td class="NewEmployeeWizards_StepTitle">
                                                <table style="width: 100%">
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lblStep5Title" runat="server" Text="" Font-Bold="True" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding-left: 20px; padding-right: 50px;">
                                                            <asp:Label ID="lblStep5Info" runat="server" Text="" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="NewEmployeeWizards_StepContent">

                                                <table style="width: 100%;" cellspacing="0" cellpadding="0">
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lblStep5Info2" runat="server" Text="Indique si desea copiar la información relativa a límites y valores iniciales." />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding-left: 10px; padding-top: 20px">
                                                            <roUserControls:roOptionPanelClient ID="chkConceptsData" runat="server" TypeOPanel="CheckboxOption" Width="100%" Height="100%" Enabled="True">
                                                                <Title>
                                                                    <asp:Label ID="lblConceptsDataTitle" runat="server" Text="Copiar límites y valores iniciales de ${Concepts}"></asp:Label>
                                                                </Title>
                                                                <Description>
                                                                    <asp:Label ID="lblConceptsDataDescription" runat="server" Text=""></asp:Label>
                                                                </Description>
                                                                <Content>
                                                                    <div style="padding-top: 40px;">
                                                                        <roUserControls:roOptionPanelClient ID="chkConceptsDataDetail" runat="server" TypeOPanel="CheckboxOption" Width="100%" Height="Auto" Enabled="True">
                                                                            <Title>
                                                                                <asp:Label ID="lblConceptsDataDetailTitle" runat="server" Text="Descartar límites y valores iniciales coincidentes"></asp:Label>
                                                                            </Title>
                                                                            <Description>
                                                                                <asp:Label ID="lblConceptsDataDetailDescription" runat="server" Text="Si escoge esta opción, en caso de coincidencia se descartarán los límites y valores iniciales que tengan definidos los ${Employees} destino de la copia."></asp:Label>
                                                                            </Description>
                                                                            <Content>
                                                                            </Content>
                                                                        </roUserControls:roOptionPanelClient>
                                                                    </div>
                                                                </Content>
                                                            </roUserControls:roOptionPanelClient>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="NewEmployeeWizards_StepError">
                                                <asp:Label ID="lblStep5Error" runat="server" CssClass="errorText" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>

                        <asp:UpdatePanel ID="updStep6" runat="server" RenderMode="Inline">
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="btPrev" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="btNext" EventName="Click" />
                            </Triggers>
                            <ContentTemplate>

                                <div id="divStep6" runat="server" style="display: none; width: 800px;">
                                    <table style="width: 790px; height: 500px;" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td class="NewEmployeeWizards_StepTitle">
                                                <table style="width: 100%">
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lblStep6Title" runat="server" Text="" Font-Bold="True" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding-left: 20px; padding-right: 50px;">
                                                            <asp:Label ID="lblStep6Info" runat="server" Text="" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="NewEmployeeWizards_StepContent">

                                                <table style="width: 100%;" cellspacing="0" cellpadding="0">
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lblStep6Info2" runat="server" Text="Indique si desea copiar la información relativa a la planificación." />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding-left: 10px; padding-top: 20px">
                                                            <roUserControls:roOptionPanelClient ID="chkSchedule" runat="server" TypeOPanel="CheckboxOption" Width="100%" Height="Auto" Enabled="True">
                                                                <Title>
                                                                    <asp:Label ID="lblScheduleTitle" runat="server" Text="Copiar planificación"></asp:Label>
                                                                </Title>
                                                                <Description>
                                                                    <asp:Label ID="lblScheduleDescription" runat="server" Text=""></asp:Label>
                                                                </Description>
                                                                <Content>
                                                                    <table width="100%" style="padding-top: 20px;">
                                                                        <tr>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblSchedulePeriod" Text="Seleccione el intervalo de fechas entre las que desea copiar la planificación." runat="server"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td align="center">
                                                                                <table>
                                                                                    <tr>
                                                                                        <td align="right">
                                                                                            <asp:Label ID="lblScheduleBeginDate" Text="Copiar la planificación desde el día" runat="server"></asp:Label>
                                                                                        </td>
                                                                                        <td align="left" style="padding-left: 10px;">
                                                                                            <%--<input type="text" id="txtScheduleBeginDate" runat="server" ConvertControl="DatePicker" CCallowBlank="false" class="textClass" style="width:75px;" onkeypress="return onEnterPress(event,'if (CheckFrame() == true) {__doPostBack(\'btNext$btButton\',\'\');}');" />--%>
                                                                                            <dx:ASPxDateEdit ID="txtScheduleBeginDate" PopupVerticalAlign="WindowCenter" runat="server" ClientInstanceName="txtScheduleBeginDateClient">
                                                                                                <ClientSideEvents KeyPress="devKeyPress" />
                                                                                            </dx:ASPxDateEdit>
                                                                                        </td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td align="right">
                                                                                            <asp:Label ID="lblScheduleEndDate" Text="hasta el día" runat="server"></asp:Label>
                                                                                        </td>
                                                                                        <td align="left" style="padding-left: 10px;">
                                                                                            <%--<input type="text" id="txtScheduleEndDate" runat="server" ConvertControl="DatePicker" CCallowBlank="false" class="textClass" style="width:75px;" onkeypress="return onEnterPress(event,'if (CheckFrame() == true) {__doPostBack(\'btNext$btButton\',\'\');}');" />--%>
                                                                                            <dx:ASPxDateEdit ID="txtScheduleEndDate" PopupVerticalAlign="WindowCenter" runat="server" ClientInstanceName="txtScheduleEndDateClient">
                                                                                                <ClientSideEvents KeyPress="devKeyPress" />
                                                                                            </dx:ASPxDateEdit>
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td>
                                                                                <div style="padding-top: 10px">
                                                                                    <div class="EmployeeCopyLeft">
                                                                                        <div class="panBottomMargin">
                                                                                            <div class="panHeader2 panBottomMargin">
                                                                                                <span class="panelTitleSpan">
                                                                                                    <asp:Label runat="server" ID="lblCopyTitle" Text="¿Qué desea copiar?"></asp:Label>
                                                                                                </span>
                                                                                            </div>
                                                                                        </div>
                                                                                        <div class="panBottomMargin">
                                                                                            <div class="divRow">
                                                                                                <div class="">
                                                                                                    <dx:ASPxCheckBox ID="ckCopyMainShifts" runat="server" Checked="true" Text="Copiar horarios principales" />
                                                                                                </div>
                                                                                            </div>
                                                                                        </div>
                                                                                        <div class="panBottomMargin">
                                                                                            <div class="divRow">
                                                                                                <div class="">
                                                                                                    <dx:ASPxCheckBox ID="ckCopyAlternativeShifts" runat="server" Checked="true" Text="Copiar horarios alternativos" />
                                                                                                </div>
                                                                                            </div>
                                                                                        </div>
                                                                                        <div class="panBottomMargin">
                                                                                            <div class="divRow">
                                                                                                <div class="">
                                                                                                    <dx:ASPxCheckBox ID="ckCopyHolidays" runat="server" Checked="false" Text="Copiar vacaciones" />
                                                                                                </div>
                                                                                            </div>
                                                                                        </div>
                                                                                    </div>

                                                                                    <div class="EmployeeCopyRight">
                                                                                        <div class="panBottomMargin">
                                                                                            <div class="panHeader2 panBottomMargin">
                                                                                                <span class="panelTitleSpan">
                                                                                                    <asp:Label runat="server" ID="lblMaintainTitle" Text="¿Qué desea mantener?"></asp:Label>
                                                                                                </span>
                                                                                            </div>
                                                                                        </div>
                                                                                        <div class="panBottomMargin">
                                                                                            <div class="divRow">
                                                                                                <div class="">
                                                                                                    <dx:ASPxCheckBox ID="ckKeepHolidays" runat="server" Checked="true" Text="Vacaciones actuales" />
                                                                                                </div>
                                                                                            </div>
                                                                                        </div>
                                                                                        <div class="panBottomMargin">
                                                                                            <div class="divRow">
                                                                                                <div class="">
                                                                                                    <dx:ASPxCheckBox ID="ckKeepBloquedDays" runat="server" Checked="true" Text="Días bloqueados" />
                                                                                                </div>
                                                                                            </div>
                                                                                        </div>
                                                                                    </div>
                                                                                </div>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </Content>
                                                            </roUserControls:roOptionPanelClient>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="NewEmployeeWizards_StepError">
                                                <asp:Label ID="lblStep6Error" runat="server" CssClass="errorText" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>

                        <asp:UpdatePanel ID="updStep7" runat="server" RenderMode="Inline">
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="btPrev" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="btNext" EventName="Click" />
                            </Triggers>
                            <ContentTemplate>

                                <div id="divStep7" runat="server" style="display: none; width: 800px;">
                                    <table style="width: 790px; height: 500px;" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td class="NewEmployeeWizards_StepTitle">
                                                <table style="width: 100%">
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lblStep7Title" runat="server" Text="" Font-Bold="True" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding-left: 20px; padding-right: 50px;">
                                                            <asp:Label ID="lblStep7Info" runat="server" Text="" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="NewEmployeeWizards_StepContent">

                                                <table style="width: 100%;" cellspacing="0" cellpadding="0">
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lblStep7Info2" runat="server" Text="Indique si desea copiar las reglas de ${Concepts}." />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding-left: 10px; padding-top: 5px">
                                                            <roUserControls:roOptionPanelClient ID="chkRules" runat="server" TypeOPanel="CheckboxOption" Width="100%" Height="Auto" Enabled="True">
                                                                <Title>
                                                                    <asp:Label ID="lblRulesTitle" runat="server" Text="Copiar las reglas de ${Concepts}"></asp:Label>
                                                                </Title>
                                                                <Description>
                                                                    <asp:Label ID="lblRulesDescription" runat="server" Text=""></asp:Label>
                                                                </Description>
                                                                <Content>
                                                                    <div style="padding-top: 40px;">
                                                                        <roUserControls:roOptionPanelClient ID="chkRulesDetail" runat="server" TypeOPanel="CheckboxOption" Width="100%" Height="Auto" Enabled="True">
                                                                            <Title>
                                                                                <asp:Label ID="lblRulesDetailTitle" runat="server" Text="Descartar las reglas de ${Concepts} actuales"></asp:Label>
                                                                            </Title>
                                                                            <Description>
                                                                                <asp:Label ID="lblRulesDetailDescription" runat="server" Text="Si escoge esta opción, se descartarán las reglas de ${Concepts} que tengan definidas los ${Employees} destino de la copia. De esta manera, tras la copia, los ${Employees} destino tendrán exactamente las mismas reglas de ${Concepts} que el ${Employee} seleccionado como referencia."></asp:Label>
                                                                            </Description>
                                                                            <Content>
                                                                            </Content>
                                                                        </roUserControls:roOptionPanelClient>
                                                                    </div>
                                                                </Content>
                                                            </roUserControls:roOptionPanelClient>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="NewEmployeeWizards_StepError">
                                                <asp:Label ID="lblStep7Error" runat="server" CssClass="errorText" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>

                        <asp:UpdatePanel ID="updStep8" runat="server" RenderMode="Inline">
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="btPrev" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="btNext" EventName="Click" />
                            </Triggers>
                            <ContentTemplate>

                                <div id="divStep8" runat="server" style="display: none; width: 800px;">
                                    <table style="width: 790px; height: 500px;" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td class="NewEmployeeWizards_StepTitle">
                                                <table style="width: 100%">
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lblStep8Title" runat="server" Text="" Font-Bold="True" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding-left: 20px; padding-right: 50px;">
                                                            <asp:Label ID="lblStep8Info" runat="server" Text="" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="NewEmployeeWizards_StepContent">

                                                <table style="width: 100%;" cellspacing="0" cellpadding="0">
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lblStep8Info2" runat="server" Text="Indique si desea asignar el convenio del contrato activo." />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding-left: 10px; padding-top: 5px">
                                                            <roUserControls:roOptionPanelClient ID="chkLabAgree" runat="server" TypeOPanel="CheckboxOption" Width="100%" Height="Auto" Enabled="True">
                                                                <Title>
                                                                    <asp:Label ID="lblLabAgreeTitle" runat="server" Text="Asignar el convenio del contrato activo."></asp:Label>
                                                                </Title>
                                                                <Description>
                                                                    <asp:Label ID="lblLabAgreeDesc" runat="server" Text="Se asignará el contrato del contrato activo del ${Employee} origen al contrato activo de los ${Employees} destino."></asp:Label>
                                                                </Description>
                                                                <Content>
                                                                </Content>
                                                            </roUserControls:roOptionPanelClient>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="NewEmployeeWizards_StepError">
                                                <asp:Label ID="lblStep8Error" runat="server" CssClass="errorText" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>

                        <asp:UpdatePanel ID="updStep9" runat="server" RenderMode="Inline">
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="btPrev" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="btNext" EventName="Click" />
                            </Triggers>
                            <ContentTemplate>

                                <div id="divStep9" runat="server" style="display: none; width: 800px;">
                                    <table style="width: 790px; height: 500px;" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td class="NewEmployeeWizards_StepTitle">
                                                <table style="width: 100%">
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lblStep9Title" runat="server" Text="" Font-Bold="True" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding-left: 20px; padding-right: 50px;">
                                                            <asp:Label ID="lblStep9Info" runat="server" Text="" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="NewEmployeeWizards_StepContent">

                                                <table style="width: 100%; height: 100%;" cellspacing="0" cellpadding="0">
                                                    <tr>
                                                        <td valign="top">
                                                            <asp:Label ID="lblStep9Info2" runat="server" Text="Indique si desea copiar datos de los ${Assignments} del ${Employee} origen." />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" style="padding-top: 5px; padding-top: 5px;" valign="top">
                                                            <roUserControls:roOptionPanelClient ID="chkAssignments" runat="server" TypeOPanel="CheckboxOption" Width="100%" Height="250" Enabled="True">
                                                                <Title>
                                                                    <asp:Label ID="Label2" runat="server" Text="Copiar información de los ${Assignments} del ${Employee}"></asp:Label>
                                                                </Title>
                                                                <Description>
                                                                </Description>
                                                                <Content>
                                                                    <table width="100%" style="padding-left: 20px; padding-top: 10px;">
                                                                        <tr>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblAssignmentsList" runat="server" Text="¿Qué ${Assignments} desea copiar?"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td align="left">
                                                                                <asp:Panel ID="panAssignments" Height="150" ScrollBars="Vertical" BackColor="white" runat="server"></asp:Panel>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td style="padding-top: 10px;">
                                                                                <table>
                                                                                    <tr>
                                                                                        <td>
                                                                                            <input type="checkbox" id="chkAllAssignments" onclick="checkAllAssignments();" />
                                                                                        </td>
                                                                                        <td>
                                                                                            <a href="javascript: void(0);" onclick="CheckLinkClick('chkAllAssignments');">
                                                                                                <asp:Label ID="Label3" runat="server" Text="Seleccionar todos los ${Assignments}"></asp:Label>
                                                                                            </a>
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </Content>
                                                            </roUserControls:roOptionPanelClient>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="NewEmployeeWizards_StepError">
                                                <asp:Label ID="lblStep9Error" runat="server" CssClass="errorText" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>

                        <asp:UpdatePanel ID="updStep10" runat="server" RenderMode="Inline">
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="btPrev" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="btNext" EventName="Click" />
                            </Triggers>
                            <ContentTemplate>

                                <div id="divStep10" runat="server" style="display: none; width: 800px;">
                                    <table style="width: 790px; height: 500px;" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td class="NewEmployeeWizards_StepTitle">
                                                <table style="width: 100%">
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lblStep10Title" runat="server" Text="" Font-Bold="True" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding-left: 20px; padding-right: 50px;">
                                                            <asp:Label ID="lblStep10Info" runat="server" Text="" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="NewEmployeeWizards_StepContent">

                                                <table style="width: 100%; height: 100%;" cellspacing="0" cellpadding="0">
                                                    <tr>
                                                        <td valign="top">
                                                            <asp:Label ID="lblStep10Info2" runat="server" Text="Indique si desea copiar datos de las cesiones del ${Employee} origen." />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" style="padding-top: 5px; padding-top: 5px;" valign="top">
                                                            <roUserControls:roOptionPanelClient ID="chkCenters" runat="server" TypeOPanel="CheckboxOption" Width="100%" Height="250" Enabled="True">
                                                                <Title>
                                                                    <asp:Label ID="Label4" runat="server" Text="Copiar información de las cesiones del ${Employee}"></asp:Label>
                                                                </Title>
                                                                <Description>
                                                                </Description>
                                                                <Content>
                                                                    <table width="100%" style="padding-left: 20px; padding-top: 10px;">
                                                                        <tr>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblCentersList" runat="server" Text="¿Qué ${BusinessCenters} desea copiar?"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td align="left">
                                                                                <asp:Panel ID="panCenters" Height="150" ScrollBars="Vertical" BackColor="white" runat="server"></asp:Panel>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td style="padding-top: 10px;">
                                                                                <table>
                                                                                    <tr>
                                                                                        <td>
                                                                                            <input type="checkbox" id="chkAllCenters" onclick="checkAllCenters();" />
                                                                                        </td>
                                                                                        <td>
                                                                                            <a href="javascript: void(0);" onclick="CheckLinkClick('chkAllCenters');">
                                                                                                <asp:Label ID="Label5" runat="server" Text="Seleccionar todos los ${BusinessCenters}"></asp:Label>
                                                                                            </a>
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </Content>
                                                            </roUserControls:roOptionPanelClient>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="NewEmployeeWizards_StepError">
                                                <asp:Label ID="lblStep10Error" runat="server" CssClass="errorText" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>

                        <asp:UpdatePanel ID="updStep11" runat="server" RenderMode="Inline">
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="btPrev" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="btNext" EventName="Click" />
                            </Triggers>
                            <ContentTemplate>

                                <div id="divStep11" runat="server" style="display: none; width: 800px;">
                                    <table style="width: 790px; height: 500px;" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td class="NewEmployeeWizards_StepTitle">
                                                <table style="width: 100%">
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lblStep11Title" runat="server" Text="" Font-Bold="True" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding-left: 20px; padding-right: 50px;">
                                                            <asp:Label ID="lblStep11Info" runat="server" Text="" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="NewEmployeeWizards_StepContent">

                                                <table style="width: 100%;" cellspacing="0" cellpadding="0">
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lblStep11Info2" runat="server" Text="El asistente ya dispone de suficientes datos para poder iniciar la copia. Pulse Copiar para iniciar el proceso." />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="NewEmployeeWizards_StepError">
                                                <asp:Label ID="lblStep11Error" runat="server" CssClass="errorText" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <td>

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
                                            <asp:Button ID="btEnd" Text="${Button.End}" runat="server" OnClientClick="PerformValidation();return false;" Visible="false" TabIndex="4" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                            <asp:Button ID="btResume" Text="" runat="server" Visible="false" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                        </td>
                                        <td>
                                            <asp:Button ID="btClose" Text="${Button.Cancel}" runat="server" OnClientClick="Close(); return false;" TabIndex="5" CssClass="btnFlat btnFlatBlack btnFlatAsp" />

                                            <asp:HiddenField ID="hdnCanClose_PageBase" runat="server" />
                                            <asp:HiddenField ID="hdnMustRefresh_PageBase" Value="0" runat="server" />
                                        </td>
                                    </tr>
                                </table>

                                <input type="hidden" id="hdnActiveFrame" value="0" runat="server" />
                                <input type="hidden" id="hdnIDEmployeeSource" value="" runat="server" />
                                <input type="hidden" id="hdnFrames" value="" runat="server" />
                                <input type="hidden" id="hdnFramesOnlyClient" value="" runat="server" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </div>

        <!-- POPUP NEW OBJECT -->
        <dx:ASPxPopupControl ID="CaptchaObjectPopup" runat="server" AllowDragging="False" CloseAction="None" Modal="True" ContentUrl="~/Base/Popups/GenericCaptchaValidator.aspx"
            PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter" ModalBackgroundStyle-Opacity="0" Width="500" Height="320"
            ShowHeader="False" ScrollBars="Auto" ShowPageScrollbarWhenModal="True" ClientInstanceName="CaptchaObjectPopup_Client" PopupAnimationType="None" BackColor="Transparent" ContentStyle-Paddings-Padding="0px" Border-BorderColor="Transparent" ShowShadow="false">
            <SettingsLoadingPanel Enabled="false" />
        </dx:ASPxPopupControl>

        <!-- POPUP NEW OBJECT -->
        <dx:ASPxPopupControl ID="AspxLoadingPopup" runat="server" AllowDragging="False" CloseAction="None" Modal="True" ContentUrl="~/Base/Popups/PerformingAction.aspx"
            PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter" ModalBackgroundStyle-Opacity="0" Width="460" Height="260"
            ShowHeader="False" ScrollBars="Auto" ShowPageScrollbarWhenModal="True" ClientInstanceName="AspxLoadingPopup_Client" PopupAnimationType="None" BackColor="Transparent" ContentStyle-Paddings-Padding="0px" Border-BorderColor="Transparent" ShowShadow="false">
            <SettingsLoadingPanel Enabled="false" />
        </dx:ASPxPopupControl>
    </form>
</body>
</html>