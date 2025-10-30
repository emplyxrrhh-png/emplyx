<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Tasks_Wizards_CreateTaskWizard" CodeBehind="CreateTaskWizard.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Asistente para crear Tareas</title>
</head>
<body class="bodyPopup" style="background-attachment: fixed;">

    <form id="frmCreateTaskWizard" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <div>

            <script language="javascript" type="text/javascript">

                function PageBase_Load() {
                    var oActiveFrameIndex = document.getElementById('<%= hdnActiveFrame.ClientID %>').value;
                    ConvertControls('divStep' + oActiveFrameIndex);

                    //Enllaç dels OptionPanelClients
                    linkOPCItems('<%= optEmployeesTree.ClientID %>,<%= optEmployeesAssignments.ClientID %>');

                    checkOPCPanelClients();
                }

                function checkOPCPanelClients() {
                    venableOPC('<%= optEmployeesTree.ClientID %>');
                    venableOPC('<%= optEmployeesAssignments.ClientID %>');
                }

                //Llença aquest script al recarregar els updatepanels per poder controlar per js els opclient
                function endRequestHandler() {
                    checkOPCPanelClients();
                    hidePopupLoader();
                }

                function showPopupLoader() {
                    if (typeof (window.parent.frames["ifPrincipal"]) != "undefined") {
                        window.parent.frames["ifPrincipal"].showLoadingGrid(true);
                    } else {
                        window.parent.parent.frames["ifPrincipal"].showLoadingGrid(true);
                    }
                }

                function hidePopupLoader() {
                    if (typeof (window.parent.frames["ifPrincipal"]) != "undefined") {
                        window.parent.frames["ifPrincipal"].showLoadingGrid(false);
                    } else {
                        window.parent.parent.frames["ifPrincipal"].showLoadingGrid(false);
                    }
                }

                function CheckFrame() {
                    var bolRet = true;
                    var oActiveFrameIndex = document.getElementById('<%= hdnActiveFrame.ClientID %>').value;

                    if (CheckConvertControls('divStep' + oActiveFrameIndex) == false) {
                        bolRet = false;
                    } else {
                        bolRet = true;
                    }
                    if (!bolRet) hidePopupLoader();
                    return bolRet;
                }

                function showDatePicker(bol) {
                    try {
                        var divDP = document.getElementById('dtPicker');
                        if (divDP == null) { return; }
                        if (bol == null) {
                            if (divDP.style.display == "") {
                                divDP.style.display = 'none';
                            } else {
                                divDP.style.display = '';
                            }
                        } else {
                            if (bol) {
                                divDP.style.display = '';
                            } else {
                                divDP.style.display = 'none';
                            }
                        }
                    } catch (e) { showError("showDatePicker", e); }
                }

                function ChangeColor(color) {
                    try {
                        var divCol = document.getElementById('colorShift');
                        if (divCol == null) { return; }
                        divCol.style.backgroundColor = color;
                        var hdnColor = document.getElementById('hdnColor');
                        hdnColor.value = color;

                    }
                    catch (e) {
                        showError("Changecolor", e);
                    }
                }

                //==========================================================================
                //Muestra ventana modal con el selector y botones de aceptar/cancelar
                //==========================================================================
                function ShowGroupSelector() {
                    $find('RoPopupFrame1Behavior').show();
                    $get('<%= RoPopupFrame1.ClientID %>').style.display = '';
                }

                //==========================================================================
                //Oculta ventana modal con el selector y botones de aceptar/cancelar
                //==========================================================================
                function HideGroupSelector(IsOk) {
                    if (IsOk == false) {
                        document.getElementById('hdnEmployees').value = "ALL";
                        document.getElementById('hdnFilter').value = "";
                        document.getElementById('hdnFilterUser').value = "";
                    }

                    $find('RoPopupFrame1Behavior').hide();

                    $get('<%= RoPopupFrame1.ClientID %>').style.display = 'none';
                }

                //==========================================================================
                //Muestra el selector de zonas o el selector de empleados con TreeV3
                //==========================================================================
                function ShowSelector(oSel) {
                    try {
                        var Title = '';
                        var iFrm = document.getElementById('<%= GroupSelectorFrame.ClientID %>');
                        iFrm.style.width = "475px";
                        iFrm.style.height = "290px";

                        iFrm.style.top = "5px";
                        iFrm.style.left = "5px";

                        if (oSel == 1) {
                            document.getElementById('hdnEmployees').value = "ALL";
                            document.getElementById('hdnFilter').value = "";
                            document.getElementById('hdnFilterUser').value = "";
                        }
                        else {

                            var strBase = '<%= Me.Page.ResolveURL("~/Base/") %>' + "WebUserControls/roWizardSelectorContainerMultiSelectV3.aspx?" +
                                "PrefixTree=treeEmpCreateTaskWizard&FeatureAlias=Employees&PrefixCookie=objContainerTreeV3_treeEmpCreateTaskWizardGrid&" +
                                "AfterSelectFuncion=parent.GetSelectedTreeV3";
                            iFrm.src = strBase;

                        }

                        //Mostrar popup
                        if (oSel != 1) {
                            ShowGroupSelector();
                        }
                    }
                    catch (e) {
                        showError("ShowSelector", e);
                    }
                }

                //==========================================================================
                //Guarda los empleados seleccionados en el TreeV3
                //==========================================================================
                function GetSelectedTreeV3(oParm1, oParm2, oParm3) {
                    if (oParm1 == "") {
                        document.getElementById('hdnEmployees').value = "ALL";
                        document.getElementById('hdnFilter').value = "";
                        document.getElementById('hdnFilterUser').value = "";
                    }
                    else {
                        document.getElementById('hdnEmployees').value = oParm1;
                        document.getElementById('hdnFilter').value = oParm2;
                        document.getElementById('hdnFilterUser').value = oParm3;
                    }
                }
            </script>

            <input type='hidden' id='hdnColor' runat='server' value="#FFFFFF" />
            <input type='hidden' id='hdnEmployees' runat='server' value="" />
            <input type="hidden" id="hdnFilter" runat="server" value="" />
            <input type="hidden" id="hdnFilterUser" runat="server" value="" />

            <table style="width: 500px; height: 400px; display: block;" cellpadding="0" cellspacing="0">
                <tr>
                    <td>
                        <%-- WELCOME --%>
                        <div id="divStep0" runat="server" style="display: block">
                            <table id="tbStep0" style="width: 100%; height: 100%;" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td style="height: 360px">
                                        <asp:Image ID="imgCreateTaskWizard" runat="server" Style="border-radius: 5px;" ImageUrl="~/Base/Images/Wizards/wzschedule.gif" />
                                    </td>
                                    <td style="padding-left: 20px; padding-right: 20px; padding-top: 50px" valign="top">
                                        <asp:Label ID="lblCreateTaskWelcome1" runat="server" Text="Bienvenido al asistente para crear una Tarea."
                                            Font-Bold="True" Font-Size="Large"></asp:Label>
                                        <br />
                                        <br />
                                        <br />
                                        <asp:Label ID="lblCreateTaskWelcome2" runat="server" Text="Este asistente le ayudará a crear una Tarea."
                                            Font-Bold="true"></asp:Label>
                                        <br />
                                        <br />
                                        <asp:Label ID="lblCreateTaskWelcome3" runat="server" Text="Para continuar, haga clic en siguiente."></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </div>

                        <asp:Label ID="hdnStepTitle" Text="Asistente para crear tareas. " runat="server" Style="display: none; visibility: hidden" />

                        <div id="divStep1" runat="server" style="display: none">
                            <table id="tbStep1" style="width: 100%; height: 100%" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td class="TaskWizards_StepTitle">
                                        <table style="width: 100%">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblStep1Title" runat="server" Text="Paso 1 de 5." Font-Bold="True" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 20px; padding-right: 40px">
                                                    <asp:Label ID="lblSetp1Info" runat="server" Text="Ahora debe seleccionar la plantilla que va a utilizar para la creación de la nueva tarea." />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="TaskWizards_StepContent">
                                        <table>
                                            <tr>
                                                <td colspan="2">
                                                    <asp:Label ID="lblStep1Info2" runat="server" Text="Seleccione la plantilla que va a utilizar." />
                                                </td>
                                            </tr>
                                            <tr style="height: 180px;">
                                                <td style="text-align: right; width: 120px; padding-right: 10px;">
                                                    <asp:Label ID="lblTemplate" runat="server" Text="Plantilla:"></asp:Label>
                                                </td>
                                                <td>
                                                    <roWebControls:roComboBox ID="cmbTemplates" runat="server" ItemsRunAtServer="False" ParentWidth="240px" EnableViewState="true" AutoResizeChildsWidth="False"
                                                        HiddenText="cmbTemplates_Text" HiddenValue="cmbTemplates_Value" />
                                                    <asp:HiddenField ID="cmbTemplates_Text" runat="server" />
                                                    <asp:HiddenField ID="cmbTemplates_Value" runat="server" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="TaskWizards_StepError">
                                        <asp:Label ID="lblStep1Error" runat="server" CssClass="errorText" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <div id="divStep2" runat="server" style="display: none">
                            <table id="btStep2" style="width: 100%; height: 100%" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td class="TaskWizards_StepTitle">
                                        <table style="width: 100%">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblStep2Title" runat="server" Text="Paso 2 de 5." Font-Bold="True" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 20px; padding-right: 40px">
                                                    <asp:Label ID="lblStep2Info" runat="server" Text="Ahora debe introducir los datos generales de la tarea." />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="TaskWizards_StepContent">
                                        <table>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblStep2Info2" runat="server" Text="Indique los datos generales de la tarea." />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <table width="100%" border="0" cellpadding="0" cellspacing="0" style="margin-top: 35px;">
                                                        <tr>
                                                            <td width="150px" align="right" style="padding-right: 10px;">
                                                                <asp:Label ID="lblName" runat="server" Text="Nombre:" class="spanEmp-Class"></asp:Label>
                                                            </td>
                                                            <td align="left">
                                                                <input type="text" runat="server" id="txtName" class="textClass x-form-text x-form-field" style="width: 370px;" convertcontrol="TextField" ccallowblank="false" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td width="150px" align="right" style="padding-right: 10px;">
                                                                <asp:Label ID="lblShortName" runat="server" Text="Nombre abrev.:" class="spanEmp-Class"></asp:Label>
                                                            </td>
                                                            <td align="left">
                                                                <input type="text" runat="server" id="txtShortName" class="textClass x-form-text x-form-field" maxlength="3" style="width: 50px;" convertcontrol="TextField" ccallowblank="false" />
                                                            </td>
                                                        </tr>
                                                        <tr style="height: 32px;" valign="top">
                                                            <td width="150px" align="right" style="padding-right: 10px;">
                                                                <asp:Label ID="lblColorDesc" runat="server" Text="Color:" class="spanEmp-Class"></asp:Label>
                                                            </td>
                                                            <td align="left">
                                                                <a runat="server" id="aDatePicker" href="javascript: void(0);" onclick="showDatePicker();" style="display: block; width: 50px;">
                                                                    <div id="colorShift" runat="server" style="display: block; border: dotted 1px #B5B8C8; background-color: #FFFFFF; width: 50px; height: 23px;">
                                                                    </div>
                                                                </a>
                                                                <div id="dtPicker" style="position: absolute; width: 430px; height: 110px; border: solid 2px silver; border-top: solid 1px silver; border-left: solid 1px silver; background-color: #EEEEEE; display: none; z-index: 1000;">
                                                                    <table id="TableColorPicker" runat="server" cellspacing="2" border="0" class="DetailFrame_TopMid">
                                                                    </table>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td width="150px" align="right" valign="top" style="padding-right: 10px;">
                                                                <asp:Label ID="lblDescription" runat="server" Text="Descripción:" class="spanEmp-Class"></asp:Label>
                                                            </td>
                                                            <td align="left" valign="top" style="padding-right: 30px;">
                                                                <textarea id="txtDescription" runat="server" rows="5" style="width: 370px; height: 80px;" class="textClass x-form-text x-form-field" convertcontrol="TextArea" ccallowblank="true"></textarea>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td width="150px" align="right" style="padding-right: 10px;">
                                                                <asp:Label ID="lblProject" runat="server" Text="Proyecto:" class="spanEmp-Class"></asp:Label>
                                                            </td>
                                                            <td align="left">
                                                                <input type="text" runat="server" id="txtProject" class="textClass x-form-text x-form-field" style="width: 370px;" convertcontrol="TextField" ccallowblank="true" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td width="150px" align="right" style="padding-right: 10px;">
                                                                <asp:Label ID="lblTag" runat="server" Text="Tags:" class="spanEmp-Class"></asp:Label>
                                                            </td>
                                                            <td align="left">
                                                                <input type="text" runat="server" id="txtTag" class="textClass x-form-text x-form-field" style="width: 370px;" convertcontrol="TextField" ccallowblank="true" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="TaskWizards_StepError">
                                        <asp:Label ID="lblStep2Error" runat="server" CssClass="errorText" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <div id="divStep3" runat="server" style="display: none">
                            <table id="tbStep3" style="width: 100%; height: 100%" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td class="TaskWizards_StepTitle">
                                        <table style="width: 100%">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblStep3Title" runat="server" Text="Paso 3 de 5." Font-Bold="True" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 20px; padding-right: 40px">
                                                    <asp:Label ID="lblStep3Info" runat="server" Text="Ahora debe introducir los datos teóricos de la tarea." />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="TaskWizards_StepContent" valign="middle">
                                        <table>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblStep3Info2" runat="server" Text="Indique los datos teóricos de la tarea." />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <table width="100%" border="0" cellpadding="0" cellspacing="0" style="margin-top: 35px;">
                                                        <tr style="height: 30px;">
                                                            <td width="200px" align="right" style="padding-right: 10px;">
                                                                <asp:Label ID="Label1" runat="server" Text="Duración asignada:" class="spanEmp-Class"></asp:Label>
                                                            </td>
                                                            <td align="left">
                                                                <input type="text" runat="server" id="txtDuracionAsignada" style="width: 50px;" convertcontrol="TimeField"
                                                                    class="textClass x-form-text x-form-field" ccallowblank="false" />
                                                            </td>
                                                        </tr>
                                                        <tr style="height: 30px;">
                                                            <td width="200px" align="right" style="padding-right: 10px;">
                                                                <asp:Label ID="lblPriority" runat="server" Text="Prioridad:" class="spanEmp-Class"></asp:Label>
                                                            </td>
                                                            <td align="left">
                                                                <input type="text" runat="server" id="txtPriority" maxlength="3" size="3" class="textClass"
                                                                    style="text-align: right;" convertcontrol="NumberField" ccallowblank="true" ccdecimalprecision="0" ccallowdecimals="false" />
                                                            </td>
                                                        </tr>
                                                        <tr style="height: 30px;">
                                                            <td width="200px" align="right" style="padding-right: 10px;">
                                                                <asp:Label ID="lblFechaIniPrev" runat="server" Text="Fecha y hora de inicio prevista:" class="spanEmp-Class"></asp:Label>
                                                            </td>
                                                            <td width="100px" align="left">
                                                                <input type="text" runat="server" id="txtExpectedStartDate" style="width: 75px;" convertcontrol="DatePicker" class="textClass" ccallowblank="false" />
                                                            </td>
                                                            <td align="left">
                                                                <input type="text" runat="server" id="txtExpectedStartTime" style="width: 50px;" convertcontrol="TimeField" class="textClass x-form-text x-form-field" ccallowblank="false" />
                                                            </td>
                                                        </tr>
                                                        <tr style="height: 30px;">
                                                            <td width="200px" align="right" style="padding-right: 10px;">
                                                                <asp:Label ID="lblFechaFinPrev" runat="server" Text="Fecha y hora de finalización prevista:" class="spanEmp-Class"></asp:Label>
                                                            </td>
                                                            <td width="100px" align="left">
                                                                <input type="text" runat="server" id="txtExpectedEndDate" style="width: 75px;" convertcontrol="DatePicker" class="textClass" ccallowblank="false" />
                                                            </td>
                                                            <td align="left">
                                                                <input type="text" runat="server" id="txtExpectedEndTime" style="width: 50px;" convertcontrol="TimeField" class="textClass x-form-text x-form-field" ccallowblank="false" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="TaskWizards_StepError">
                                        <asp:Label ID="lblStep3Error" runat="server" CssClass="errorText" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <div id="divStep4" runat="server" style="display: none">
                            <table id="tbStep4" style="width: 100%; height: 100%" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td class="TaskWizards_StepTitle">
                                        <table style="width: 100%">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblStep4Title" runat="server" Text="Paso 4 de 5." Font-Bold="True" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 20px; padding-right: 40px">
                                                    <asp:Label ID="lblStep4Info" runat="server" Text="Ahora debe seleccionar los empleados de la tarea." />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="TaskWizards_StepContent">
                                        <table>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblStep4Info2" runat="server" Text="Indique los empleados asignados a la tarea." />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <table>
                                                        <tr>
                                                            <td style="padding-left: 10px; padding-top: 25px">
                                                                <roUserControls:roOptionPanelClient ID="optEmployeesTree" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Checked="False" Enabled="True" Border="True" Value="0">
                                                                    <Title>
                                                                        <asp:Label ID="lblEmployeesTree" runat="server" Text="Empleados seleccionados"></asp:Label>
                                                                    </Title>
                                                                    <Description>
                                                                        <asp:Label ID="lblEmployeesTreeDesc" runat="server" Text="Sólo los empleados selccionados de la lista podrán trabajar en la tarea."></asp:Label>
                                                                    </Description>
                                                                    <Content>
                                                                        <a href="javascript: void(0)" id="aEmpSelect" class="btnMode" style="width: 100%;">
                                                                            <asp:Label ID="lblEmpSelect" runat="server" Text="Seleccionar..." onclick="ShowSelector(2);"></asp:Label>
                                                                        </a>
                                                                    </Content>
                                                                </roUserControls:roOptionPanelClient>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td style="padding-left: 10px; padding-top: 25px">
                                                                <roUserControls:roOptionPanelClient ID="optEmployeesAssignments" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto" Checked="False" Enabled="True" Border="True" Value="1">
                                                                    <Title>
                                                                        <asp:Label ID="lblEmployeesAssignments" runat="server" Text="Empleados que cubran un puesto"></asp:Label>
                                                                    </Title>
                                                                    <Description>
                                                                        <asp:Label ID="lblEmployeesAssignmentsDesc" runat="server" Text="Sólo los empleados que cubran el puesto indicado podrán trabajar en la tarea."></asp:Label>
                                                                    </Description>
                                                                    <Content>
                                                                        <table border="0" width="100%" style="padding: 20px;" align="center">
                                                                            <tr>
                                                                                <td align="left" style="padding-left: 20px;">
                                                                                    <table>
                                                                                        <tr>
                                                                                            <td style="white-space: nowrap;">
                                                                                                <asp:Label ID="lblAddAssignmentDesc" runat="server" Text="Puesto:"></asp:Label>
                                                                                            </td>
                                                                                            <td style="width: 150px;">
                                                                                                <roWebControls:roComboBox ID="cmbAssignment" runat="server" EnableViewState="true" AutoResizeChildsWidth="True"
                                                                                                    ParentWidth="150px" ChildsVisible="7" ItemsRunAtServer="false"
                                                                                                    HiddenText="cmbAssignment_Text" HiddenValue="cmbAssignment_Value">
                                                                                                </roWebControls:roComboBox>
                                                                                                <input type="hidden" id="cmbAssignment_Text" runat="server" />
                                                                                                <input type="hidden" id="cmbAssignment_Value" runat="server" />
                                                                                            </td>
                                                                                            <td></td>
                                                                                        </tr>
                                                                                        <tr>
                                                                                            <td style="white-space: nowrap;">
                                                                                                <asp:Label ID="lblIdoneidad" runat="server" Text="Idoneidad:"></asp:Label>
                                                                                            </td>
                                                                                            <td style="width: 150px">
                                                                                                <roWebControls:roComboBox Width="120px" ID="cmbComparation" runat="server" EnableViewState="true" AutoResizeChildsWidth="True"
                                                                                                    ParentWidth="150px" ChildsVisible="7" ItemsRunAtServer="false"
                                                                                                    HiddenText="cmbComparation_Text" HiddenValue="cmbWeeklyTo_Value">
                                                                                                </roWebControls:roComboBox>
                                                                                                <input type="hidden" id="cmbComparation_Text" runat="server" />
                                                                                                <input type="hidden" id="cmbComparation_Value" runat="server" />
                                                                                            </td>
                                                                                            <td>
                                                                                                <asp:Label ID="lblQue" runat="server" Text="que" Style="margin-left: 5px; margin-right: 5px;"></asp:Label>
                                                                                                <input type="text" runat="server" id="txtValue" class="textClass" maxlength="3" style="width: 40px; text-align: right;"
                                                                                                    convertcontrol="NumberField" ccallowblank="true" ccdecimalprecision="0" ccallowdecimals="false" />
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
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="TaskWizards_StepError">
                                        <asp:Label ID="lblStep4Error" runat="server" CssClass="errorText" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <div id="divStep5" runat="server" style="display: none">
                            <table id="tbStep5" style="width: 100%; height: 100%" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td class="TaskWizards_StepTitle">
                                        <table style="width: 100%">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblStep5Title" runat="server" Text="Paso 5 de 5." Font-Bold="True" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 20px; padding-right: 40px">
                                                    <asp:Label ID="lblStep5Info" runat="server" Text="Ahora debe indicar el período de validez de la tarea" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="TaskWizards_StepContent">
                                        <table>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblStep5Info2" runat="server" Text="Indique el período de validez de la tarea." />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <table>
                                                        <tr>
                                                            <td colspan="3" width="150px" align="left" style="padding-left: 30px; height: 50px;">
                                                                <asp:Label ID="Label3" runat="server" Text="Inicio del período." />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td width="150px" align="right" style="padding-right: 10px;">
                                                                <asp:Label ID="Label2" runat="server" Text="Fecha y hora fija:" class="spanEmp-Class"></asp:Label>
                                                            </td>
                                                            <td align="left" width="100px">
                                                                <input type="text" runat="server" id="Text1" style="width: 75px;" convertcontrol="DatePicker" class="textClass" ccallowblank="false" />
                                                            </td>
                                                            <td align="left">
                                                                <input type="text" runat="server" id="Text2" style="width: 50px;" convertcontrol="TimeField" class="textClass x-form-text x-form-field" ccallowblank="false" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td width="150px" align="right" style="padding-right: 10px;">
                                                                <asp:Label ID="Label6" runat="server" Text="Al finalizar la tarea:" class="spanEmp-Class"></asp:Label>
                                                            </td>
                                                            <td colspan="2">
                                                                <roWebControls:roComboBox ID="cmbFinTarea" runat="server" ItemsRunAtServer="False"
                                                                    ParentWidth="240px" EnableViewState="true" HiddenText="cmbFinTarea_Text" AutoResizeChildsWidth="False"
                                                                    HiddenValue="cmbFinTarea_Value" />
                                                                <asp:HiddenField ID="cmbFinTarea_Text" runat="server" />
                                                                <asp:HiddenField ID="cmbFinTarea_Value" runat="server" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td width="150px" align="right" style="padding-right: 10px;">
                                                                <asp:Label ID="Label5" runat="server" Text="Al iniciar la tarea:" class="spanEmp-Class"></asp:Label>
                                                            </td>
                                                            <td colspan="2">
                                                                <roWebControls:roComboBox ID="cmbIniTarea" runat="server" ItemsRunAtServer="False"
                                                                    ParentWidth="240px" EnableViewState="true" HiddenText="cmbIniTarea_Text" AutoResizeChildsWidth="False"
                                                                    HiddenValue="cmbIniTarea_Value" />
                                                                <asp:HiddenField ID="cmbIniTarea_Text" runat="server" />
                                                                <asp:HiddenField ID="cmbIniTarea_Value" runat="server" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="3" width="150px" align="left" style="padding-left: 30px; height: 50px;">
                                                                <asp:Label ID="Label4" runat="server" Text="Final del período." />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td width="150px" align="right" style="padding-right: 10px;">
                                                                <asp:Label ID="Label7" runat="server" Text="Fecha y hora fija:" class="spanEmp-Class"></asp:Label>
                                                            </td>
                                                            <td align="left" width="100px">
                                                                <input type="text" runat="server" id="Text3" style="width: 75px;" convertcontrol="DatePicker" class="textClass" ccallowblank="false" />
                                                            </td>
                                                            <td align="left">
                                                                <input type="text" runat="server" id="Text4" style="width: 50px;" convertcontrol="TimeField" class="textClass x-form-text x-form-field" ccallowblank="false" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="TaskWizards_StepError">
                                        <asp:Label ID="lblStep5Error" runat="server" CssClass="errorText" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </td>
                </tr>
            </table>

            <table align="right" cellpadding="0" cellspacing="0">
                <tr class="TaskWizards_ButtonsPanel" style="height: 44px">
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
                    </td>
                </tr>
            </table>

            <input type="hidden" id="hdnActiveFrame" value="0" runat="server" />

            <roWebControls:roPopupFrameV2 ID="RoPopupFrame1" runat="server" ShowTitleBar="true" BehaviorID="RoPopupFrame1Behavior" CssClassPopupExtenderBackground="modalBackgroundTransparent">
                <FrameContentTemplate>
                    <table cellpadding="0" cellspacing="0">
                        <tr>
                            <td>
                                <asp:Label ID="lblGroupSelection" Text="Seletor de Empleados" runat="server" />
                            </td>
                            <td align="right">
                                <asp:ImageButton ID="btSelectorOk" runat="server" ImageUrl="~/Base/Images/ButtonOK_16.png" Style="cursor: pointer;" OnClientClick='SelectorAction(true); SelectorOk(); HideGroupSelector(true); return false;' />
                                <asp:ImageButton ID="btSelectorCancel" runat="server" ImageUrl="~/Base/Images/ButtonCancel_16.png" Style="cursor: pointer;" OnClientClick='HideGroupSelector(false); return false;' />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" valign="top">
                                <asp:HiddenField ID="hdnIDGroupSelected" runat="server" Value="" />
                                <asp:HiddenField ID="hdnIDGroupSelectedName" runat="server" Value="" />
                                <iframe id="GroupSelectorFrame" runat="server" style="background-color: Transparent;" height="200" width="200"
                                    scrolling="no" frameborder="0" marginheight="0" marginwidth="0" src="" />
                            </td>
                        </tr>
                    </table>
                </FrameContentTemplate>
            </roWebControls:roPopupFrameV2>

            <Local:MessageFrame ID="MessageFrame1" runat="server" />
        </div>
    </form>
</body>
</html>