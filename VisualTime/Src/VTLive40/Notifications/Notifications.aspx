<%@ Page Language="VB" MasterPageFile="~/Base/mastEmp.master" AutoEventWireup="false" Inherits="VTLive40.Notifications" Title="${Notifications}" CodeBehind="Notifications.aspx.vb" %>

<%@ Register TagPrefix="dx" Namespace="DevExpress.Web.ASPxSpellChecker" Assembly="DevExpress.Web.ASPxSpellChecker.v23.1, Version=23.1.5.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" %>
<asp:Content ID="cMainBody" ContentPlaceHolderID="contentMainBody" runat="Server">

    <script language="javascript" type="text/javascript">

        function PageBase_Load() {
            resizeFrames();
            resizeTreeNotifications();

            if (document.getElementById('<%= noRegs.ClientID %>').value != '') {
                setTimeout(function () { newNotification(); }, 500);
            }
        }
    </script>

    <input type="hidden" runat="server" id="hdnModeEdit" value="" />
    <input type="hidden" runat="server" id="noRegs" value="" />
    <input type="hidden" id="hdnLngStartupValues" value="<%= Me.Language.Translate("gridHeader.StartupValues", Me.DefaultScope) %>" />
    <input type="hidden" id="hdnLngRuleName" value="<%= Me.Language.Translate("gridHeader.RuleName",Me.DefaultScope) %>" />
    <input type="hidden" id="hdnLngDateBegin" value="<%= Me.Language.Translate("gridHeader.DateBegin",Me.DefaultScope) %>" />
    <input type="hidden" id="hdnLngDateEnd" value="<%= Me.Language.Translate("gridHeader.DateEnd",Me.DefaultScope) %>" />
    <input type="hidden" id="dateFormatValue" runat="server" value="" />

    <input type="hidden" id="msgHasChanges" value="" runat="server" clientidmode="Static" />
    <input type="hidden" id="msgHasErrors" value="" runat="server" clientidmode="Static" />
    


    <div id="divMainBody">
        <!-- TAB SUPERIOR -->
        <div id="divTabInfo" class="divDataCells">
            <div style="min-height: 10px"></div>
            <div id="divNotification" class="blackRibbonTitle">
            </div>
            <div style="min-height: 10px"></div>
        </div>
        <!-- FIN TAB SUPERIOR -->

        <!-- ARBOL Y DETALLE -->
        <div id="divTabData" class="divDataCells">

            <div id="divTree" class="treeSize">
                <div id="ctlTreeDiv" style="height: 500px;">
                    <rws:rotreesselector id="roTreesNotifications" runat="server" showemployeefilters="false" prefixtree="roTreesNotifications"
                        tree1visible="true" tree1multisel="false" tree1showonlygroups="false" tree1function="cargaNodo" tree1imagepath="images/NotificationSelector" tree1selectorpage="../../Notifications/NotificationSelectorData.aspx"
                        showtreecaption="true">
                    </rws:rotreesselector>
                </div>
            </div>

            <div id="divButtons" class="divMiddleButtons">
                <div id="divBarButtons" class="maxHeight">&nbsp</div>
            </div>

            <div id="divContenido" class="divRightContent">
                <div id="divContent" class="maxHeight">
                    <dx:aspxcallback id="SaveLanguageCallback" runat="server" clientinstancename="SaveLanguageCallbackClient" clientsideevents-callbackcomplete="SaveLanguageCallbackClientEndCallBack"></dx:aspxcallback>
                    <dx:aspxcallbackpanel id="ASPxCallbackPanelContenido" runat="server" width="100%" height="100%" clientinstancename="ASPxCallbackPanelContenidoClient">
                        <settingsloadingpanel enabled="false" />
                        <clientsideevents endcallback="ASPxCallbackPanelContenidoClient_EndCallBack" />
                        <panelcollection>
                            <dx:panelcontent id="PanelContent1" runat="server">
                                <input type="hidden" id="EmployeeFilter" value="" runat="server" clientidmode="Static" />
                                <div id="divMsgTop" class="divMsg2 divMessageTop" style="display: none">
                                    <div class="divImageMsg">
                                        <img alt="" id="Img1" src="~/Base/Images/MessageFrame/Alert16.png" runat="server" />
                                    </div>
                                    <div class="messageText">
                                        <span id="msgTop"></span>
                                    </div>
                                    <div align="right" class="messageActions">
                                        <a href="javascript: void(0);" class="aMsg" onclick="saveChanges();"><span id="lblSaveChanges" runat="server" /></a>
                                        &nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;
                                    <a href="javascript: void(0);" onclick="undoChanges();" class="aMsg"><span id="lblUndoChanges" runat="server" /></a>
                                    </div>
                                </div>

                                <div id="divContentPanels" class="divContentPanelsWithOutMessage">
                                    <div id="div00" class="contentPanel" runat="server" name="menuPanel">
                                        <div class="panHeader2">
                                            <span style="">
                                                <asp:Label runat="server" ID="lblNotificationsTitleGeneral" Text="General"></asp:Label></span>
                                        </div>
                                        <br />
                                        <!-- Tabla GENERAL -->
                                        <table style="padding-top: 5px; padding-bottom: 30px; padding-left: 40px;">
                                            <tr style="height: 30px;" valign="middle">
                                                <td style="width: 97px; white-space: nowrap;">
                                                    <asp:Label ID="lblName" runat="server" Text="Nombre" class="spanEmp-class"></asp:Label>
                                                </td>
                                                <td>
                                                    <dx:aspxtextbox id="txtName" width="250px" runat="server" clientinstancename="txtName_Client" nulltext="_____">
                                                        <clientsideevents validation="LengthValidation" textchanged="function(s,e){checkNotificationEmptyName(s.GetValue());}" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                        <validationsettings setfocusonerror="True"  ValidationGroup="CreateNotification">
                                                            <requiredfield isrequired="True" errortext="(*)" />
                                                        </validationsettings>
                                                    </dx:aspxtextbox>
                                                </td>
                                                <td style="width: 55px; white-space: nowrap; padding-left: 30px;">
                                                    <asp:Label ID="lblchkActivated" runat="server" Text="Activada" class="spanEmp-class" />
                                                </td>
                                                <td style="">
                                                    <input type="checkbox" id="chkActivated" runat="server" onclick="hasChanges(true);" />
                                                </td>
                                            </tr>
                                            <tr style="height: 30px;" valign="middle">
                                                <td style="width: 100px; white-space: nowrap;">
                                                    <asp:Label ID="lblType" runat="server" Text="Tipo de notificación" class="spanEmp-class"></asp:Label>
                                                </td>
                                                <td>
                                                    <dx:aspxcombobox id="cmbType" runat="server" width="350px" font-size="11px" cssclass="editTextFormat"
                                                        font-names="Arial;Verdana;Sans-Serif" incrementalfilteringmode="Contains" clientinstancename="cmbTypeClient">
                                                        <clientsideevents selectedindexchanged="function(s,e){ hasChanges(true); ShowSelectedDiv(s.GetSelectedItem().value); cleanFields(s.GetSelectedItem().value);}" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                    </dx:aspxcombobox>
                                                </td>
                                                <td colspan="2"></td>
                                            </tr>
                                        </table>

                                        <div class="panHeader2">
                                            <span style="">
                                                <asp:Label runat="server" ID="lblNotificationsTitleWhen" Text="Cuándo"></asp:Label></span>
                                        </div>
                                        <br />
                                        <!-- Tabla WHEN -->
                                        <table width="100%" style="padding-top: 5px; padding-bottom: 30px; padding-left: 40px;">
                                            <tr valign="top">
                                                <td>
                                                    <div id="divNotification1" style="display: none;">
                                                        <asp:Label ID="lblNotificationDescr1" runat="server" class="spanEmp-class" Text="Avisar X días antes de inicio de contrato"></asp:Label>
                                                        <rousercontrols:rogroupbox id="grNotification1" runat="server">
                                                            <content>
                                                                <table border="0" style="padding-top: 10px; padding-left: 20px;">
                                                                    <tr>
                                                                        <td>
                                                                            <asp:Label ID="lblNotification1_1" runat="server" Text="Avisar"></asp:Label>
                                                                        </td>
                                                                        <td style="width: 50px; padding-left: 5px; padding-right: 5px;" align="center">
                                                                            <dx:aspxtextbox runat="server" id="txtDaysNotification1" maxlength="5" width="50px" clientinstancename="txtDaysNotification1Client">
                                                                                <masksettings mask="<0..99999>" includeliterals="None" />
                                                                                <clientsideevents textchanged="function(s,e){ hasChanges(true);}" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                                                <validationsettings errordisplaymode="None"  ValidationGroup="CreateNotification"/>
                                                                            </dx:aspxtextbox>
                                                                        </td>
                                                                        <td>
                                                                            <asp:Label ID="lblNotification1_2" runat="server" Text="días antes de inicio de contrato."></asp:Label>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </content>
                                                        </rousercontrols:rogroupbox>
                                                    </div>
                                                    <div id="divNotification2" style="display: none;">
                                                        <asp:Label ID="lblNotificationDescr2" runat="server" class="spanEmp-class" Text="Avisar X días antes final de contrato"></asp:Label>
                                                        <rousercontrols:rogroupbox id="grNotification2" runat="server">
                                                            <content>
                                                                <table border="0" style="padding-top: 10px; padding-left: 20px;">
                                                                    <tr>
                                                                        <td>
                                                                            <asp:Label ID="lblNotification2_1" runat="server" Text="Avisar"></asp:Label>
                                                                        </td>
                                                                        <td style="width: 50px; padding-left: 5px; padding-right: 5px;" align="center">
                                                                            <dx:aspxtextbox runat="server" id="txtDaysNotification2" maxlength="5" width="50px" clientinstancename="txtDaysNotification2Client">
                                                                                <masksettings mask="<0..99999>" includeliterals="None" />
                                                                                <clientsideevents textchanged="function(s,e){ hasChanges(true);}" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                                                <validationsettings errordisplaymode="None" ValidationGroup="CreateNotification"/>
                                                                            </dx:aspxtextbox>
                                                                        </td>
                                                                        <td>
                                                                            <asp:Label ID="lblNotification2_2" runat="server" Text="días antes de la finalización del contrato."></asp:Label>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </content>
                                                        </rousercontrols:rogroupbox>
                                                    </div>
                                                    <div id="divNotification3" style="display: none;">
                                                        <asp:Label ID="lblNotificationDescr3" runat="server" class="spanEmp-class" Text="Aviso X días antes Inicio de ausencia prolongada "></asp:Label>
                                                        <rousercontrols:rogroupbox id="grNotification3" runat="server">
                                                            <content>
                                                                <table border="0" style="padding-top: 10px; padding-left: 20px;">
                                                                    <tr>
                                                                        <td style="white-space: nowrap;" width="70">
                                                                            <asp:Label ID="lblNotification3_1" runat="server" Text="Avisar"></asp:Label>
                                                                        </td>
                                                                        <td style="width: 50px; padding-right: 5px;" align="center">
                                                                            <dx:aspxtextbox runat="server" id="txtDaysNotification3" maxlength="5" width="50px" clientinstancename="txtDaysNotification3Client">
                                                                                <masksettings mask="<0..99999>" includeliterals="None" />
                                                                                <clientsideevents textchanged="function(s,e){ hasChanges(true);}" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                                                <validationsettings errordisplaymode="None"  ValidationGroup="CreateNotification"/>
                                                                            </dx:aspxtextbox>
                                                                        </td>
                                                                        <td>
                                                                            <asp:Label ID="lblNotification3_2" runat="server" Text="días antes de inicio de ausencia prolongada."></asp:Label>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                                <table border="0" style="padding-top: 10px; padding-left: 20px;">
                                                                    <tr>
                                                                        <td style="white-space: nowrap;" width="70">
                                                                            <asp:Label ID="lblNotification3_3" runat="server" Text="Justificación"></asp:Label>
                                                                        </td>
                                                                        <td>
                                                                            <dx:aspxcombobox id="cmbCausesNotification3" runat="server" width="300px" font-size="11px" cssclass="editTextFormat"
                                                                                font-names="Arial;Verdana;Sans-Serif" incrementalfilteringmode="Contains" clientinstancename="cmbCausesNotification3Client">
                                                                                <clientsideevents selectedindexchanged="function(s,e){ hasChanges(true)}" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                                            </dx:aspxcombobox>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </content>
                                                        </rousercontrols:rogroupbox>
                                                    </div>
                                                    <div id="divNotification4" style="display: none;">
                                                        <asp:Label ID="lblNotificationDescr4" runat="server" class="spanEmp-class" Text="Aviso Inicio de ausencia prolongada "></asp:Label>
                                                        <rousercontrols:rogroupbox id="grNotification4" runat="server">
                                                            <content>
                                                                <table border="0" style="padding-top: 10px; padding-left: 20px;">
                                                                    <tr>
                                                                        <td style="width: 120px; white-space: nowrap;">
                                                                            <asp:Label ID="lblNotification4_1" runat="server" Text="Ausencia prolongada:"></asp:Label>
                                                                        </td>
                                                                        <td style="padding-right: 5px;">
                                                                            <dx:aspxcombobox id="cmbCausesNotification4" runat="server" width="300px" font-size="11px" cssclass="editTextFormat"
                                                                                font-names="Arial;Verdana;Sans-Serif" incrementalfilteringmode="Contains" clientinstancename="cmbCausesNotification4Client">
                                                                                <clientsideevents selectedindexchanged="function(s,e){ hasChanges(true)}" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                                            </dx:aspxcombobox>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </content>
                                                        </rousercontrols:rogroupbox>
                                                    </div>
                                                    <div id="divNotification5" style="display: none;">
                                                        <asp:Label ID="lblNotificationDescr5" runat="server" class="spanEmp-class" Text="Avisar X días antes del fin ausencia prolongada"></asp:Label>
                                                        <rousercontrols:rogroupbox id="grNotification5" runat="server">
                                                            <content>
                                                                <table border="0" style="padding-top: 10px; padding-left: 20px;">
                                                                    <tr>
                                                                        <td style="white-space: nowrap;" width="70">
                                                                            <asp:Label ID="lblNotification5_1" runat="server" Text="Avisar"></asp:Label>
                                                                        </td>
                                                                        <td style="width: 50px; padding-right: 5px;" align="center">
                                                                            <dx:aspxtextbox runat="server" id="txtDaysNotification5" maxlength="5" width="50px" clientinstancename="txtDaysNotification5Client">
                                                                                <masksettings mask="<0..99999>" includeliterals="None" />
                                                                                <clientsideevents textchanged="function(s,e){ hasChanges(true);}" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                                                <validationsettings errordisplaymode="None" ValidationGroup="CreateNotification"/>
                                                                            </dx:aspxtextbox>
                                                                        </td>
                                                                        <td>
                                                                            <asp:Label ID="lblNotification5_2" runat="server" Text="días antes de la finalización de la ausencia prolongada."></asp:Label>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                                <table border="0" style="padding-top: 10px; padding-left: 20px;">
                                                                    <tr>
                                                                        <td style="white-space: nowrap;" width="70">
                                                                            <asp:Label ID="lblNotification5_3" runat="server" Text="Justificación"></asp:Label>
                                                                        </td>
                                                                        <td>
                                                                            <dx:aspxcombobox id="cmbCausesNotification5" runat="server" width="300px" font-size="11px" cssclass="editTextFormat"
                                                                                font-names="Arial;Verdana;Sans-Serif" incrementalfilteringmode="Contains" clientinstancename="cmbCausesNotification5Client">
                                                                                <clientsideevents selectedindexchanged="function(s,e){ hasChanges(true)}" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                                            </dx:aspxcombobox>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </content>
                                                        </rousercontrols:rogroupbox>
                                                    </div>
                                                    <div id="divNotification6" style="display: none;">
                                                        <asp:Label ID="lblNotificationDescr6" runat="server" class="spanEmp-class" Text="Aviso si durante X días no ha no ha venido el ${Employee} "></asp:Label>
                                                        <rousercontrols:rogroupbox id="grNotification6" runat="server">
                                                            <content>
                                                                <table border="0" style="padding-top: 10px; padding-left: 20px;">
                                                                    <tr>
                                                                        <td>
                                                                            <asp:Label ID="lblNotification6_1" runat="server" Text="Avisar si durante"></asp:Label>
                                                                        </td>
                                                                        <td style="width: 50px; padding-left: 5px; padding-right: 5px;" align="center">
                                                                            <dx:aspxtextbox runat="server" id="txtDaysNotification6" maxlength="5" width="50px" clientinstancename="txtDaysNotification6Client">
                                                                                <masksettings mask="<0..99999>" includeliterals="None" />
                                                                                <clientsideevents textchanged="function(s,e){ hasChanges(true);}" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                                                <validationsettings errordisplaymode="None" ValidationGroup="CreateNotification"/>
                                                                            </dx:aspxtextbox>
                                                                        </td>
                                                                        <td>
                                                                            <asp:Label ID="lblNotification6_2" runat="server" Text="días no ha venido el ${Employee}."></asp:Label>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </content>
                                                        </rousercontrols:rogroupbox>
                                                    </div>
                                                    <div id="divNotification7" style="display: none;">
                                                        <asp:Label ID="lblNotificationDescr7" runat="server" class="spanEmp-class" Text="Corte ausencia prolongada"></asp:Label>
                                                    </div>
                                                    <div id="divNotification8" style="display: none;">
                                                        <asp:Label ID="lblNotificationDescr8" runat="server" class="spanEmp-class" Text="Avisar fichajes con ${Causes}"></asp:Label>
                                                        <rousercontrols:rogroupbox id="grNotification8" runat="server">
                                                            <content>
                                                                <table border="0" style="padding-top: 10px; padding-left: 20px;">
                                                                    <tr>
                                                                        <td style="width: 170px; white-space: nowrap;">
                                                                            <asp:Label ID="lblNotification8_1" runat="server" Text="Avisar fichajes con ${Cause}"></asp:Label>
                                                                        </td>
                                                                        <td style="padding-right: 5px;">
                                                                            <dx:aspxcombobox id="cmbCausesNotification8" runat="server" width="300px" font-size="11px" cssclass="editTextFormat"
                                                                                font-names="Arial;Verdana;Sans-Serif" incrementalfilteringmode="Contains" clientinstancename="cmbCausesNotification8Client">
                                                                                <clientsideevents selectedindexchanged="function(s,e){ hasChanges(true)}" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                                            </dx:aspxcombobox>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </content>
                                                        </rousercontrols:rogroupbox>
                                                    </div>
                                                    <div id="divNotification9" style="display: none;">
                                                        <asp:Label ID="lblNotificationDescr9" runat="server" class="spanEmp-class" Text="Intentos de acceso inválidos: Un aviso de los intentos inválidos de acceso de los ${Employees} por cualquier motivo."></asp:Label>
                                                    </div>
                                                    <div id="divNotification10" style="display: none;">
                                                        <asp:Label ID="lblNotificationDescr10" runat="server" class="spanEmp-class" Text="Mensajes de error de la aplicación "></asp:Label>
                                                    </div>
                                                    <div id="divNotification11" style="display: none;">
                                                        <asp:Label ID="lblNotificationDescr11" runat="server" class="spanEmp-class" Text="Terminales desconectados"></asp:Label>
                                                    </div>
                                                    <div id="divNotification12" style="display: none;">
                                                        <asp:Label ID="lblNotificationDescr12" runat="server" class="spanEmp-class" Text="Intentos de acceso inválidos a Visualtime Live"></asp:Label>
                                                    </div>
                                                    <div id="divNotification15" style="display: none;">
                                                        <asp:Label ID="lblNotificationDescr15" runat="server" class="spanEmp-class" Text="El ${Employee} debería estar o aún no ha llegado"></asp:Label>
                                                    </div>
                                                    <div id="divNotification16" style="display: none;">
                                                        <asp:Label ID="lblNotificationDescr16" runat="server" class="spanEmp-class" Text="Avisar X días antes del fin del período según campo especificado de la ficha del empleado"></asp:Label>
                                                        <br />
                                                        <rousercontrols:rogroupbox id="grNotification16" runat="server">
                                                            <content>
                                                                <table border="0" style="padding-top: 10px; padding-left: 20px;">
                                                                    <tr>
                                                                        <td>
                                                                            <asp:Label ID="lblNotification16_1" runat="server" Text="Avisar"></asp:Label>
                                                                        </td>
                                                                        <td style="width: 50px; padding-left: 5px; padding-right: 5px;" align="center">
                                                                            <dx:aspxtextbox runat="server" id="txtDaysNotification16" maxlength="5" width="50px" clientinstancename="txtDaysNotification16Client">
                                                                                <masksettings mask="<0..99999>" includeliterals="None" />
                                                                                <clientsideevents textchanged="function(s,e){ hasChanges(true);}" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                                                <validationsettings errordisplaymode="None"  ValidationGroup="CreateNotification"/>
                                                                            </dx:aspxtextbox>
                                                                        </td>
                                                                        <td>
                                                                            <asp:Label ID="lblNotification16_2" runat="server" Text="días antes del final del período."></asp:Label>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                                <table border="0" style="padding-top: 10px; padding-left: 20px;">
                                                                    <tr>
                                                                        <td width="150">
                                                                            <asp:Label ID="lblNotification16_3" runat="server" Text="Utilizar el campo de la ficha"></asp:Label>
                                                                        </td>
                                                                        <td>
                                                                            <dx:aspxcombobox id="cmbDatePeriodUserField" runat="server" width="200px" font-size="11px" cssclass="editTextFormat"
                                                                                font-names="Arial;Verdana;Sans-Serif" incrementalfilteringmode="Contains" clientinstancename="cmbDatePeriodUserFieldClient">
                                                                                <clientsideevents selectedindexchanged="function(s,e){ hasChanges(true)}" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                                            </dx:aspxcombobox>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </content>
                                                        </rousercontrols:rogroupbox>
                                                    </div>
                                                    <div id="divNotification17" style="display: none;">
                                                        <asp:Label ID="lblNotificationDescr17" runat="server" class="spanEmp-class" Text="Avisar X días antes del fin del período según campo especificado de la ficha de la empresa"></asp:Label>
                                                        <br />
                                                        <rousercontrols:rogroupbox id="grNotification17" runat="server">
                                                            <content>
                                                                <table border="0" style="padding-top: 10px; padding-left: 20px;">
                                                                    <tr>
                                                                        <td>
                                                                            <asp:Label ID="lblNotification17_1" runat="server" Text="Avisar"></asp:Label>
                                                                        </td>
                                                                        <td style="width: 50px; padding-left: 5px; padding-right: 5px;" align="center">
                                                                            <dx:aspxtextbox runat="server" id="txtDaysNotification17" maxlength="5" width="50px" clientinstancename="txtDaysNotification17Client">
                                                                                <masksettings mask="<0..99999>" includeliterals="None" />
                                                                                <clientsideevents textchanged="function(s,e){ hasChanges(true);}" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                                                <validationsettings errordisplaymode="None" ValidationGroup="CreateNotification"/>
                                                                            </dx:aspxtextbox>
                                                                        </td>
                                                                        <td>
                                                                            <asp:Label ID="lblNotification17_2" runat="server" Text="días antes del final del período."></asp:Label>
                                                                        </td>
                                                                    </tr>
                                                                </table>

                                                                <table border="0" style="padding-top: 10px; padding-left: 20px;">
                                                                    <tr>
                                                                        <td width="150">
                                                                            <asp:Label ID="lblNotification17_3" runat="server" Text="Utilizar el campo de la ficha"></asp:Label>
                                                                        </td>
                                                                        <td>
                                                                            <dx:aspxcombobox id="cmbDatePeriodUserFieldEnter" runat="server" width="200px" font-size="11px" cssclass="editTextFormat"
                                                                                font-names="Arial;Verdana;Sans-Serif" incrementalfilteringmode="Contains" clientinstancename="cmbDatePeriodUserFieldEnterClient">
                                                                                <clientsideevents selectedindexchanged="function(s,e){ hasChanges(true)}" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                                            </dx:aspxcombobox>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </content>
                                                        </rousercontrols:rogroupbox>
                                                    </div>
                                                    <div id="divNotification18" style="display: none;">
                                                        <asp:Label ID="lblNotificationDescr18" runat="server" class="spanEmp-class" Text="Avisar si en el día de ayer, el empleado no cumplió una condición"></asp:Label>
                                                        <br />
                                                        <rousercontrols:rogroupbox id="grNotification18" runat="server">
                                                            <content>
                                                                <table border="0" cellpadding="0" cellspacing="5">
                                                                    <tr>
                                                                        <!-- COLUMNA COMBO SALDOS -->
                                                                        <td valign="top">
                                                                            <table cellpadding="0" cellspacing="0">
                                                                                <tr>
                                                                                    <td>
                                                                                        <div style="height: 25px; text-align: left; vertical-align: middle;">
                                                                                            <table border="0" style="width: 100%;">
                                                                                                <tr>
                                                                                                    <td style="padding-left: 5px; padding-top: 5px;">
                                                                                                        <asp:Label ID="lblNotification18_1" runat="server" Text="Utilizar el saldo"></asp:Label>
                                                                                                    </td>
                                                                                                </tr>
                                                                                            </table>
                                                                                        </div>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>
                                                                                        <dx:aspxcombobox id="cmbConcepts" runat="server" width="200px" font-size="11px" cssclass="editTextFormat"
                                                                                            font-names="Arial;Verdana;Sans-Serif" incrementalfilteringmode="Contains" clientinstancename="cmbConceptsClient">
                                                                                            <clientsideevents selectedindexchanged="function(s,e){ hasChanges(true)}" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                                                        </dx:aspxcombobox>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </td>

                                                                        <!-- COLUMNA OPERADORES DE COMPARACION-->
                                                                        <td valign="top">
                                                                            <table cellpadding="0" cellspacing="0">
                                                                                <tr>
                                                                                    <td>
                                                                                        <div style="height: 25px; text-align: left; vertical-align: middle;">
                                                                                            <table border="0" style="width: 100%;">
                                                                                                <tr>
                                                                                                    <td style="padding-left: 5px; padding-top: 5px;">
                                                                                                        <asp:Label ID="lblNotification18_2" runat="server" Text="Comparación"></asp:Label>
                                                                                                    </td>
                                                                                                </tr>
                                                                                            </table>
                                                                                        </div>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>
                                                                                        <dx:aspxcombobox id="cmbCompare" runat="server" width="120px" font-size="11px" cssclass="editTextFormat"
                                                                                            font-names="Arial;Verdana;Sans-Serif" incrementalfilteringmode="Contains" clientinstancename="cmbCompareClient">
                                                                                            <clientsideevents selectedindexchanged="function(s,e){ hasChanges(true)}" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                                                        </dx:aspxcombobox>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </td>

                                                                        <!-- COLUMNA SELECCIONA VALOR O CAMPO DE LA FICHA-->
                                                                        <td valign="top">
                                                                            <table cellpadding="0" cellspacing="0">
                                                                                <tr>
                                                                                    <td>
                                                                                        <div style="height: 25px; text-align: left; vertical-align: middle;">
                                                                                            <table border="0" style="width: 100%;">
                                                                                                <tr>
                                                                                                    <td style="padding-left: 5px; padding-top: 5px;">
                                                                                                        <asp:Label ID="lblTypeValues" runat="server" Text="Valor"></asp:Label>
                                                                                                    </td>
                                                                                                </tr>
                                                                                            </table>
                                                                                        </div>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>
                                                                                        <dx:aspxcombobox id="cmbTypeValue" runat="server" width="150px" font-size="11px" cssclass="editTextFormat"
                                                                                            font-names="Arial;Verdana;Sans-Serif" incrementalfilteringmode="Contains" clientinstancename="cmbTypeValueClient">
                                                                                            <clientsideevents selectedindexchanged="function(s,e){ showTypeValue(s.GetSelectedItem().value);hasChanges(true);}" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                                                        </dx:aspxcombobox>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </td>

                                                                        <!-- COLUMNA CAMPO VALOR O SELECTOR CAMPO DE LA FICHA-->
                                                                        <td valign="top">
                                                                            <table cellpadding="0" cellspacing="0">
                                                                                <tr>
                                                                                    <td>
                                                                                        <div style="height: 25px; text-align: left; vertical-align: middle;">
                                                                                            <table border="0" style="width: 100%;">
                                                                                                <tr>
                                                                                                    <td style="padding-left: 5px; padding-top: 5px;">
                                                                                                        <asp:Label ID="lblValues" runat="server" Text="Horas"></asp:Label>
                                                                                                    </td>
                                                                                                </tr>
                                                                                            </table>
                                                                                        </div>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align="left">
                                                                                        <div id="divValueType" style="display: none;">
                                                                                            <dx:aspxtimeedit id="txtValueType" editformatstring="HH:mm" editformat="Custom" runat="server" width="85" clientinstancename="txtValueTypeClient">
                                                                                                <clientsideevents datechanged="function(s,e){ hasChanges(true);}" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                                                            </dx:aspxtimeedit>
                                                                                        </div>
                                                                                        <div id="divConceptUserField">
                                                                                            <dx:aspxcombobox id="cmbConceptUserField" runat="server" width="150px" font-size="11px" cssclass="editTextFormat"
                                                                                                font-names="Arial;Verdana;Sans-Serif" incrementalfilteringmode="Contains" clientinstancename="cmbConceptUserFieldClient">
                                                                                                <clientsideevents selectedindexchanged="function(s,e){ hasChanges(true)}" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                                                            </dx:aspxcombobox>
                                                                                        </div>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </content>
                                                        </rousercontrols:rogroupbox>
                                                    </div>
                                                    <div id="divNotification19" style="display: none;">
                                                        <asp:Label ID="lblTypeDescr19" runat="server" class="spanEmp-class" Text="Día con fichajes impares"></asp:Label>
                                                    </div>
                                                    <div id="divNotification20" style="display: none;">
                                                        <asp:Label ID="lblTypeDescr20" runat="server" class="spanEmp-class" Text="Día con fichajes no fiables"></asp:Label>
                                                    </div>
                                                    <div id="divNotification21" style="display: none;">
                                                        <asp:Label ID="lblTypeDescr21" runat="server" class="spanEmp-class" Text="Día con incidencias no justificadas"></asp:Label>
                                                    </div>
                                                    <div id="divNotification22" style="display: none;">
                                                        <asp:Label ID="lblTypeDescr22" runat="server" class="spanEmp-class" Text="Tarjeta no asignada a ningún ${Employee}"></asp:Label>
                                                    </div>
                                                    <div id="divNotification23" style="display: none;">
                                                        <asp:Label ID="lblTypeDescr23" runat="server" class="spanEmp-class" Text="Tareas que finalizan hoy"></asp:Label>
                                                    </div>
                                                    <div id="divNotification24" style="display: none;">
                                                        <asp:Label ID="lblTypeDescr24" runat="server" class="spanEmp-class" Text="Tareas que empiezan hoy"></asp:Label>
                                                    </div>
                                                    <div id="divNotification25" style="display: none;">
                                                        <asp:Label ID="lblTypeDescr25" runat="server" class="spanEmp-class" Text="Tareas que han excedido el tiempo inicial"></asp:Label>
                                                    </div>
                                                    <div id="divNotification26" style="display: none;">
                                                        <asp:Label ID="lblTypeDescr26" runat="server" class="spanEmp-class" Text="Tareas que han sobrepasado la fecha de finalización prevista"></asp:Label>
                                                    </div>
                                                    <div id="divNotification29" style="display: none;">
                                                        <asp:Label ID="lblTypeDescr29" runat="server" class="spanEmp-class" Text="">Tareas que han excedido la fecha de finalización prevista</asp:Label>
                                                    </div>
                                                    <div id="divNotification34" style="display: none;">
                                                        <asp:Label ID="lblTypeDescr34" runat="server" class="spanEmp-class" Text="Documento no entregado en la fecha de seguimiento"></asp:Label>
                                                    </div>
                                                    <div id="divNotification40" style="display: none;">
                                                        <asp:Label ID="lblTypeDescr40" runat="server" class="spanEmp-class" Text="Aviso de solicitud"></asp:Label>
                                                    </div>
                                                    <div id="divNotification43" style="display: none;">
                                                        <asp:Label ID="lblTypeDescr43" runat="server" class="spanEmp-class" Text="Cuando un usuario modifique o cree cualquier movilidad de un empleado."></asp:Label>
                                                    </div>
                                                    <div id="divNotification44" style="display: none;">
                                                        <asp:Label ID="lblTypeDescr44" runat="server" class="spanEmp-class" Text="Cuando se haga efectiva la movilidad de un empleado."></asp:Label>
                                                    </div>
                                                    <div id="divNotification51" style="display: none;">
                                                        <asp:Label ID="lblShiftSchedule" runat="server" class="spanEmp-class" Text="Cuando se planifique uno de los siguentes horarios"></asp:Label>
                                                        <div class="divRow">
                                                            <div id="lstShiftsAssignSchedule">
                                                            </div>
                                                        </div>
                                                        <div class="divRow">
                                                            <dx:aspxcheckbox id="ckOnlyCalendar" runat="server" text="Solo cuando la modificación se realice desde la pantalla de calendario">
                                                                <clientsideevents checkedchanged="function(s,e){ hasChanges(true)}" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                            </dx:aspxcheckbox>
                                                        </div>
                                                    </div>
                                                    <div id="divNotification8384" style="display: none; ">
                                                        <div style="display: flex; flex-direction: column; justify-content: flex-start;gap: 5px;width: 100%; margin-bottom: 10px;">
                                                            <asp:Label ID="lblPunchBeforeStart" runat="server" class="spanEmp-class" Text="si al realizar el primer fichaje en uno de los siguientes horarios"></asp:Label>
                                                            <asp:Label ID="lblPunchNotDone" runat="server" class="spanEmp-class" Text="Al llegar la hora de inicio de los siguientes horarios sumando el tiempo de tolerancia, no existe fichaje de entrada." ></asp:Label>
                                                            <div id="lstShiftsToCheckFirstPunch"></div>                                                            
                                                            <div id="lstRigidShiftsToCheckFirstPunch"></div>                                                            

                                                        </div>
                                                        <div style="display: flex; align-items: center; justify-content: flex-start;gap: 5px; width: 100%; margin-bottom: 10px;">
                                                            <asp:Label ID="lblTimeLimit" runat="server" class="spanEmp-class" Text="lo hacen antes de las "></asp:Label>
                                                            <asp:Label ID="lblTimeTolerance" runat="server" class="spanEmp-class" Text="Tolerancia "></asp:Label>
                                                            <dx:aspxtimeedit id="timeLimitNotification8384" editformatstring="HH:mm" editformat="Custom" runat="server" width="85" clientinstancename="timeLimitNotification83">
                                                                <clientsideevents datechanged="function(s,e){ hasChanges(true);}" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                            </dx:aspxtimeedit>
                                                            <asp:Label ID="lblMinutesBefore" runat="server" class="spanEmp-class" Text=" antes de la hora prevista "></asp:Label>
                                                        </div>
                                                        <div style="display: flex; align-items: center; justify-content: flex-start;gap: 5px;width: 100%; ">
                                                            <asp:Label ID="lblEmployeesNotification8384" runat="server" class="spanEmp-class" Text="Aplica a "></asp:Label>
                                                            <dx:ASPxTextBox ID="txtEmployees" runat="server" ReadOnly="true" Font-Size="11px" Height="25px" CssClass="editTextFormat" Width="300px" ClientInstanceName="txtEmployeesClient">
                                                                <ClientSideEvents GotFocus="btnOpenPopupSelectorEmployeesClient_Click" />
                                                            </dx:ASPxTextBox>
                                                        </div>
                                                    </div>
                                                    <div id="divNotification72" style="display: none; ">    
    <div style="display: flex; flex-direction: column; justify-content: flex-start;gap: 5px;width: 100%; margin-bottom: 10px;">
            <asp:Label ID="lblEmployeeForgotExitPunch" runat="server" class="spanEmp-class" Text="Al llegar la hora indicada, la jornada debe haberse finalizado."></asp:Label>        
        <div id="grdShiftsNotifyAt"></div>                                                            
    </div>
    <div style="margin-top:20px;display: flex; align-items: center; justify-content: flex-start;gap: 5px;width: 100%; ">
        <asp:Label ID="lblEmployeesNotification72" runat="server" class="spanEmp-class" Text="Aplica a "></asp:Label>
        <dx:ASPxTextBox ID="txtEmployees72" runat="server" ReadOnly="true" Font-Size="11px" Height="25px" CssClass="editTextFormat" Width="300px" ClientInstanceName="txtEmployees72Client">
            <ClientSideEvents GotFocus="btnOpenPopupSelectorEmployeesClient_Click" />
        </dx:ASPxTextBox>
    </div>
</div>
                                                    <div id="divNotification56" style="display: none;">
                                                        <asp:Label ID="lblPunchDuringProgrammedAbsence" runat="server" class="spanEmp-class" Text="Se produzca un fichaje durante una ausencia prolongada. (1 vez por día)"></asp:Label>
                                                        <rousercontrols:rogroupbox id="grNotification56" runat="server">
                                                            <content>

                                                                <table border="0" style="padding-top: 10px; padding-left: 20px;">
                                                                    <tr>
                                                                        <td style="white-space: nowrap;" width="70">
                                                                            <asp:Label ID="lblNotification56_3" runat="server" Text="Justificación"></asp:Label>
                                                                        </td>
                                                                        <td>
                                                                            <dx:aspxcombobox id="cmbCausesNotification56" runat="server" width="300px" font-size="11px" cssclass="editTextFormat"
                                                                                font-names="Arial;Verdana;Sans-Serif" incrementalfilteringmode="Contains" clientinstancename="cmbCausesNotification56Client">
                                                                                <clientsideevents selectedindexchanged="function(s,e){ hasChanges(true)}" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                                            </dx:aspxcombobox>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                                <rousercontrols:rooptionpanelclient id="optTypeToIgnore" runat="server" typeopanel="CheckboxOption" width="100%" cconclick="hasChanges(true);" height="Auto" checked="False" enabled="True">
                                                                    <title>
                                                                        <asp:Label ID="lblTypeToIgnore" runat="server" Text="Solo notificar los fichajes distintos a accesos."></asp:Label>
                                                                    </title>
                                                                    <description>
                                                                        <asp:Label ID="lblTypeToIgnoreDesc" runat="server" Text="Solo se enviará una notificación cuando se produzca un fichaje diferente a accesos."></asp:Label>
                                                                    </description>
                                                                    <content>
                                                                    </content>
                                                                </rousercontrols:rooptionpanelclient>
                                                            </content>
                                                        </rousercontrols:rogroupbox>
                                                    </div>
                                                    <div id="divNotification61" style="display: none;">
                                                        <asp:Label ID="lblImportResult" runat="server" class="spanEmp-class" Text="Cuando se ejecute una importación en el sistema ya sea manual o automática"></asp:Label>
                                                    </div>
                                                    <div id="divNotification65" style="display: none;">
                                                        <asp:Label ID="lblPunchDuringProgrammedAbsence65" runat="server" class="spanEmp-class" Text="Se produzca un fichaje durante una ausencia prolongada. (1 vez por día)"></asp:Label>
                                                        <rousercontrols:rogroupbox id="grNotification65" runat="server">
                                                            <content>

                                                                <table border="0" style="padding-top: 10px; padding-left: 20px;">
                                                                    <tr>
                                                                        <td style="white-space: nowrap;" width="70">
                                                                            <asp:Label ID="lblNotification65_3" runat="server" Text="Justificación"></asp:Label>
                                                                        </td>
                                                                        <td>
                                                                            <dx:aspxcombobox id="cmbCausesNotification65" runat="server" width="300px" font-size="11px" cssclass="editTextFormat"
                                                                                font-names="Arial;Verdana;Sans-Serif" incrementalfilteringmode="Contains" clientinstancename="cmbCausesNotification65Client">
                                                                                <clientsideevents selectedindexchanged="function(s,e){ hasChanges(true)}" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                                            </dx:aspxcombobox>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </content>
                                                        </rousercontrols:rogroupbox>
                                                    </div>
                                                    <div id="divNotification69" style="display: none;">
                                                        <asp:Label ID="lblMaskAlert" runat="server" class="spanEmp-class" Text="Cuando el empleado realice un fichaje sin vestir mascarilla, y esto sea requerido"></asp:Label>
                                                    </div>
                                                    <div id="divNotification73" style="display: none;">
                                                        <asp:Label ID="lblTemperatureAlert" runat="server" class="spanEmp-class" Text="Cuando el empleado realice un fichaje superando el límite de temperatura, y esto sea requerido"></asp:Label>
                                                    </div>
                                                    <div id="divNotification76" style="display: none;">
                                                        <asp:Label ID="lblRequestCanceled" runat="server" class="spanEmp-class" Text="Cuando se produzca"></asp:Label>
                                                    </div>
                                                    <div id="divNotification70" style="display: none;">
                                                        <div style="float: left;">
                                                            <asp:Label ID="lblTypeDescr70" runat="server" class="spanEmp-class" Text="Justificación Pendiente"></asp:Label>
                                                        </div>
                                                        <div style="float: left;">
                                                            <div style="float: left; padding-left: 10px; padding-right: 10px;">
                                                                <dx:aspxspinedit id="seNotificationPeriod" runat="server" number="0" width="100%" clientinstancename="seNotificationPeriod70_Client">
                                                                    <spinbuttons showincrementbuttons="True" showlargeincrementbuttons="False" />
                                                                    <clientsideevents valuechanged="function(s,e){ hasChanges(true);}" />
                                                                </dx:aspxspinedit>
                                                            </div>
                                                        </div>
                                                        <div style="float: left;">
                                                            <asp:Label ID="lblTypeDescr70_2" runat="server" class="spanEmp-class" Text="días máximo"></asp:Label>
                                                        </div>
                                                        <div style="float: left;">
                                                            <div style="float: left; padding-left: 10px; padding-right: 10px;">
                                                                <dx:aspxspinedit id="seNotificationRepeat" runat="server" number="0" width="100%" clientinstancename="seNotificationRepeat70_Client">
                                                                    <spinbuttons showincrementbuttons="True" showlargeincrementbuttons="False" />
                                                                    <clientsideevents valuechanged="function(s,e){ hasChanges(true);}" />
                                                                </dx:aspxspinedit>
                                                            </div>
                                                        </div>
                                                        <div style="float: left;">
                                                            <asp:Label ID="lblTypeDescr70_3" runat="server" class="spanEmp-class" Text="veces más"></asp:Label>
                                                        </div>
                                                    </div>

                                                    <div id="divBreach" style="display: none;">
                                                        <asp:Label ID="lblBreachNotifyWhen" runat="server" class="spanEmp-class" Text="Cuando el tiempo justificado por la previsión cumpla la siguiente condición "></asp:Label>
                                                        <rousercontrols:rogroupbox id="roGroupBox71" runat="server">
                                                            <content>
                                                                <table border="0" style="padding-top: 10px; padding-left: 20px;">
                                                                    <tr>
                                                                        <td style="white-space: nowrap;" width="70">
                                                                            <asp:Label ID="lblBreachCondition" runat="server" Text="El valor justificado "></asp:Label>
                                                                        </td>
                                                                        <td>
                                                                            <dx:aspxcombobox id="cmbBreachCompare" runat="server" width="300px" font-size="11px" cssclass="editTextFormat"
                                                                                font-names="Arial;Verdana;Sans-Serif" incrementalfilteringmode="Contains" clientinstancename="cmbBreachCompareClient">
                                                                                <clientsideevents selectedindexchanged="function(s,e){ hasChanges(true)}" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                                            </dx:aspxcombobox>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                                <table border="0" style="padding-top: 10px; padding-left: 20px;">
                                                                    <tr>
                                                                        <td style="white-space: nowrap;" width="70">
                                                                            <asp:Label ID="lblBreachCourtesyMinutes" runat="server" Text="Cortesía "></asp:Label>
                                                                        </td>
                                                                        <td style="width: 50px; padding-right: 5px;" align="center">
                                                                            <dx:aspxtextbox runat="server" id="txtBreachCourtesyMinutes" maxlength="5" width="50px" clientinstancename="txtCourtesyMinutesClient">
                                                                                <masksettings mask="<0..99999>" includeliterals="None" />
                                                                                <clientsideevents textchanged="function(s,e){ hasChanges(true);}" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                                                <validationsettings errordisplaymode="None" ValidationGroup="CreateNotification"/>
                                                                            </dx:aspxtextbox>
                                                                        </td>
                                                                        <td>
                                                                            <asp:Label ID="lblBreachMinutes" runat="server" Text="minutos."></asp:Label>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </content>
                                                        </rousercontrols:rogroupbox>
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>

                                        <div class="panHeader2">
                                            <span style="">
                                                <asp:Label runat="server" ID="lblTitleWhoNewDesign" Text="Quién recibirá la notificación"></asp:Label></span>
                                        </div>
                                        <br />
                                        <!-- Tabla Who New Design-->
                                        <table id="TableWhoNewDesign" width="100%" style="padding-bottom: 5px; padding-left: 40px;">
                                            <tr style="height: 30px;">
                                                <td style="width: 150px; white-space: nowrap;">

                                                    <div id="divSupervisorByPortal">
                                                        <rousercontrols:rooptionpanelclient id="optSupervisorByPortal" runat="server" typeopanel="CheckboxOption" width="100%" cconclick="hasChanges(true);" height="Auto" checked="False" enabled="True">
                                                            <title>
                                                                <asp:Label ID="lblSupervisorByPortal" runat="server" Text="Al supervisor directo del ${Employee} a través de VisualTime Supervisor Portal"></asp:Label>
                                                            </title>
                                                            <description>
                                                                <asp:Label ID="lblSupervisorByPortalDesc" runat="server" Text="Al supervisor directo del ${Employee} a través de VisualTime Supervisor Portal."></asp:Label>
                                                            </description>
                                                            <content>
                                                            </content>
                                                        </rousercontrols:rooptionpanelclient>
                                                    </div>
                                                    <div id="divEmployeeByPortal">
                                                        <rousercontrols:rooptionpanelclient id="optEmployeeByPortal" runat="server" typeopanel="CheckboxOption" width="100%" cconclick="hasChanges(true);" height="Auto" checked="False" enabled="True">
                                                            <title>
                                                                <asp:Label ID="lblEmployeeByPortal" runat="server" Text="Al empleado mediante el Portal del empleado"></asp:Label>
                                                            </title>
                                                            <description>
                                                                <asp:Label ID="lblEmployeeByPortalDesc" runat="server" Text="Se mostará una notificación en el portal del empleado con el detalle de la misma"></asp:Label>
                                                            </description>
                                                            <content>
                                                            </content>
                                                        </rousercontrols:rooptionpanelclient>
                                                    </div>

                                                    <div id="divSupervisorByMail">
                                                        <rousercontrols:rooptionpanelclient id="optSupervisorByMail" runat="server" typeopanel="CheckboxOption" width="100%" cconclick="hasChanges(true);" height="Auto" checked="False" enabled="True">
                                                            <title>
                                                                <asp:Label ID="lblSupervisorByMail" runat="server" Text="Al supervisor directo del ${Employee} mediante correo electrónico"></asp:Label>
                                                            </title>
                                                            <description>
                                                                <asp:Label ID="lblSupervisorByMailDesc" runat="server" Text="Al supervisor directo del ${Employee} mediante correo electrónico."></asp:Label>
                                                            </description>
                                                            <content>
                                                            </content>
                                                        </rousercontrols:rooptionpanelclient>
                                                    </div>

                                                    <div id="divEmployeeField">
                                                        <rousercontrols:rooptionpanelclient id="optEmployeeField" runat="server" typeopanel="CheckboxOption" width="100%" cconclick="hasChanges(true);" height="Auto" checked="False" enabled="True">
                                                            <title>
                                                                <asp:Label ID="lblEmployeeField" runat="server" Text="A la dirección de correo indicada en el campo de la ficha"></asp:Label>
                                                            </title>
                                                            <description>
                                                                <asp:Label ID="lblEmployeeFieldDesc" runat="server" Text="Seleccione el campo de la ficha que tiene guardada la dirección de correo."></asp:Label>
                                                            </description>
                                                            <content>
                                                                <table border="0" style="padding-left: 20px; padding-top: 5px; padding-bottom: 10px;">
                                                                    <tr>
                                                                        <td style="width: 100px;">
                                                                            <asp:Label ID="lblEmployeeFieldText1" runat="server" Text="Campo de la ficha "></asp:Label>
                                                                        </td>
                                                                        <td>
                                                                            <dx:aspxcombobox id="cmbUserFieldsNewDesign" runat="server" width="200px" font-size="11px" cssclass="editTextFormat"
                                                                                font-names="Arial;Verdana;Sans-Serif" incrementalfilteringmode="Contains" clientinstancename="cmbUserFieldsNewDesignClient">
                                                                                <clientsideevents selectedindexchanged="function(s,e){ hasChanges(true)}" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                                            </dx:aspxcombobox>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </content>
                                                        </rousercontrols:rooptionpanelclient>
                                                    </div>

                                                    <div id="divMailList">
                                                        <rousercontrols:rooptionpanelclient id="optMailList" runat="server" typeopanel="CheckboxOption" width="100%" height="Auto" cconclick="hasChanges(true);" checked="False" enabled="True">
                                                            <title>
                                                                <asp:Label ID="lblMailList" runat="server" Text="A las direcciones de correo aquí especificadas"></asp:Label>
                                                            </title>
                                                            <description>
                                                                <asp:Label ID="lblMailListDesc" runat="server" Text="Puede especificar múltiples direcciones de mail si se separan mediante el carácter ;"></asp:Label>
                                                            </description>
                                                            <content>
                                                                <table border="0" width="100%" style="padding: 20px; padding-top: 5px;" align="center">
                                                                    <tr>
                                                                        <td>
                                                                            <asp:Label ID="lblMailListText1" runat="server" Text="E-mails"></asp:Label>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>
                                                                            <dx:aspxtextbox runat="server" id="txtMailList" maxlength="800" width="800px" clientinstancename="txtMailListClient">
                                                                                <clientsideevents textchanged="function(s,e){ hasChanges(true);}" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                                                <validationsettings errordisplaymode="None" />
                                                                            </dx:aspxtextbox>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </content>
                                                        </rousercontrols:rooptionpanelclient>
                                                    </div>

                                                    <div id="divConditionRole">
                                                        <rousercontrols:rooptionpanelclient id="optConditionRole" runat="server" typeopanel="CheckboxOption" width="100%" height="Auto" cconclick="hasChanges(true);" checked="False" enabled="True">
                                                            <title>
                                                                <asp:Label ID="lblConditionRole" runat="server" Text="A los supervisores con los siguientes roles"></asp:Label>
                                                            </title>
                                                            <description>
                                                                <asp:Label ID="lblConditionRoleDesc" runat="server" Text="Solo recibirán la notificación los supervisores con los siguientes roles"></asp:Label>
                                                            </description>
                                                            <content>
                                                                <table border="0" width="100%" style="padding: 20px; padding-top: 5px;" align="center">
                                                                    <tr>
                                                                        <td>
                                                                            <asp:Label ID="lblConditionRoleText1" runat="server" Text="Roles"></asp:Label>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>
                                                                            <dx:aspxtokenbox runat="server" id="tknConditionRole" maxlength="800" width="800px" clientinstancename="tknConditionRoleClient">
                                                                                <clientsideevents textchanged="function(s,e){ hasChanges(true);}" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                                                <validationsettings errordisplaymode="None" />
                                                                            </dx:aspxtokenbox>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </content>
                                                        </rousercontrols:rooptionpanelclient>
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                    <div id="div01" class="contentPanel" runat="server" style="display: none" name="menuPanel">
                                        <div class="panHeader2">
                                            <span style="">
                                                <asp:Label runat="server" ID="lblNotificationHeaderText" Text="Textos de notificaciones"></asp:Label></span>
                                        </div>
                                        <br />

                                        <div>
                                            <div class="panBottomMargin">
                                                <div class="divRow">
                                                    <div class="divRowDescription">
                                                        <asp:Label ID="lblNotificationTypeDesc" runat="server" Text="Notificación a la que desea personalizar el texto"></asp:Label>
                                                    </div>
                                                    <asp:Label ID="lblNotificationType" runat="server" Text="Notificación:" class="labelForm"></asp:Label>
                                                    <div class="componentForm">
                                                        <div style="float: left">
                                                            <dx:aspxcombobox id="cmbNotificationType" runat="server" width="300px" clientinstancename="cmbNotificationTypeClient">
                                                                <clientsideevents selectedindexchanged="function(s,e){ hasChangesHtmlEditors(true, false) }" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                                <validationsettings errordisplaymode="None">
                                                                </validationsettings>
                                                            </dx:aspxcombobox>
                                                        </div>
                                                        <div style="float: left">
                                                            <asp:Label ID="lblScenarioDesc" runat="server" Text="Escenario:" class="labelForm"></asp:Label>
                                                        </div>
                                                        <div style="margin-left: 10px; float: left">
                                                            <dx:aspxcombobox id="cmbAvailableScenarios" runat="server" width="300px" clientinstancename="cmbAvailableScenariosClient">
                                                                <clientsideevents selectedindexchanged="function(s,e){ hasChangesHtmlEditors(true, false) }" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                                <validationsettings errordisplaymode="None">
                                                                </validationsettings>
                                                            </dx:aspxcombobox>
                                                        </div>
                                                        <div style="float: left">
                                                            <asp:Label ID="lblLanguageSelection" runat="server" Text="Idioma:" class="labelForm"></asp:Label>
                                                        </div>
                                                        <div style="margin-left: 10px; float: left">
                                                            <dx:aspxcombobox id="cmbAvailableLanguages" runat="server" width="300px" clientinstancename="cmbAvailableLanguagesClient">
                                                                <clientsideevents selectedindexchanged="function(s,e){ hasChangesHtmlEditors(true, false) }" gotfocus="HightlightOnGotFocus" lostfocus="FadeOnLostFocus" />
                                                                <validationsettings errordisplaymode="None">
                                                                </validationsettings>
                                                            </dx:aspxcombobox>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div id="divNotificationHeader" class="panBottomMargin" style="margin-bottom: 5px; margin-left: 30px; margin-right: 30px;">
                                                <div class="jsGrid">
                                                    <asp:Label ID="lblNotificationHeaderTitle" runat="server" CssClass="jsGridTitle" Text="Cabecera del mensaje"></asp:Label>
                                                </div>
                                                <div class="jsGridContent">
                                                    <dx:aspxhtmleditor id="dxNotificationHeaderEditor" clientinstancename="dxNotificationHeaderEditorClient" runat="server" width="100%" height="100px">
                                                        <clientsideevents htmlchanged="function(s,e){ hasChangesHtmlEditors(true, true); }" />
                                                        <toolbars>
                                                            <dx:htmleditortoolbar name="StandardToolbar1">
                                                                <items>
                                                                    <dx:toolbarinsertplaceholderdialogbutton begingroup="True">
                                                                    </dx:toolbarinsertplaceholderdialogbutton>
                                                                </items>
                                                            </dx:htmleditortoolbar>
                                                        </toolbars>
                                                    </dx:aspxhtmleditor>
                                                </div>
                                            </div>

                                            <div id="divNotificationBody" class="panBottomMargin" style="margin-bottom: 5px; margin-left: 30px; margin-right: 30px;">
                                                <div class="jsGrid">
                                                    <asp:Label ID="lblNotificationBodyTitle" runat="server" CssClass="jsGridTitle" Text="Cuerpo del mensaje"></asp:Label>
                                                    <%--<div class="jsgridButton">
                                                        <dx:ASPxButton ID="btnAlterPreviewClient" runat="server" AutoPostBack="False" CausesValidation="False" Text="Editar/Previsualizar" ToolTip="Editar/Previsualizar" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat">
                                                            <ClientSideEvents Click="onBtnAlterPreview" />
                                                        </dx:ASPxButton>
                                                    </div>--%>
                                                </div>
                                                <div class="jsGridContent">
                                                    <dx:aspxhtmleditor id="dxNotificationLanguageEditor" clientinstancename="dxNotificationLanguageEditorClient" runat="server" width="100%" height="360px">
                                                        <clientsideevents htmlchanged="function(s,e){ hasChangesHtmlEditors(true, true); }" />
                                                        <toolbars>
                                                            <dx:htmleditortoolbar name="StandardToolbar1">
                                                                <items>
                                                                    <dx:toolbarundobutton begingroup="True">
                                                                    </dx:toolbarundobutton>
                                                                    <dx:toolbarredobutton>
                                                                    </dx:toolbarredobutton>
                                                                    <dx:toolbarboldbutton begingroup="True">
                                                                    </dx:toolbarboldbutton>
                                                                    <dx:toolbaritalicbutton>
                                                                    </dx:toolbaritalicbutton>
                                                                    <dx:toolbarunderlinebutton>
                                                                    </dx:toolbarunderlinebutton>
                                                                    <dx:toolbarstrikethroughbutton>
                                                                    </dx:toolbarstrikethroughbutton>
                                                                    <dx:toolbarjustifyleftbutton begingroup="True">
                                                                    </dx:toolbarjustifyleftbutton>
                                                                    <dx:toolbarjustifycenterbutton>
                                                                    </dx:toolbarjustifycenterbutton>
                                                                    <dx:toolbarjustifyrightbutton>
                                                                    </dx:toolbarjustifyrightbutton>
                                                                    <dx:toolbarbackcolorbutton begingroup="True">
                                                                    </dx:toolbarbackcolorbutton>
                                                                    <dx:toolbarfontcolorbutton>
                                                                    </dx:toolbarfontcolorbutton>
                                                                    <dx:toolbarinsertplaceholderdialogbutton begingroup="True">
                                                                    </dx:toolbarinsertplaceholderdialogbutton>
                                                                </items>
                                                            </dx:htmleditortoolbar>
                                                        </toolbars>
                                                    </dx:aspxhtmleditor>
                                                    <dx:aspxroundpanel id="dxNotificationLanguagePreview" runat="server" clientinstancename="dxNotificationLanguagePreviewClient" clientvisible="false"
                                                        headertext="Document Preview" height="360px" width="100%">
                                                        <headerstyle horizontalalign="Center" />
                                                    </dx:aspxroundpanel>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div id="divMsgBottom" class="divMsg2 divMessageBottom" style="display: none">
                                    <div class="divImageMsg">
                                        <img alt="" id="Img2" src="~/Base/Images/MessageFrame/Alert16.png" runat="server" />
                                    </div>
                                    <div class="messageText">
                                        <span id="msgBottom"></span>
                                    </div>
                                    <div align="right" class="messageActions">
                                        <a href="javascript: void(0);" class="aMsg" onclick="saveChanges();"><span id="lblSaveChangesBottom" runat="server" /></a>
                                        &nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;
                                        <a href="javascript: void(0);" onclick="undoChanges();" class="aMsg"><span id="lblUndoChangesBottom" runat="server" /></a>
                                    </div>
                                </div>
                            </dx:panelcontent>
                        </panelcollection>
                    </dx:aspxcallbackpanel>
                </div>
            </div>
            <!-- POPUP NEW OBJECT -->
            <dx:aspxpopupcontrol id="NewObjectPopup" runat="server" allowdragging="False" closeaction="None" modal="True" contenturl="~/Base/Popups/CreateObjectPopup.aspx"
                popupverticalalign="WindowCenter" popuphorizontalalign="WindowCenter" width="470px" height="300px"
                showheader="False" scrollbars="Auto" showpagescrollbarwhenmodal="True" clientinstancename="NewObjectPopup_Client" popupanimationtype="None" backcolor="Transparent" contentstyle-paddings-padding="0px" border-bordercolor="Transparent" showshadow="false">
                <settingsloadingpanel enabled="false" />
            </dx:aspxpopupcontrol>
        </div>
    </div>

        <dx:ASPxPopupControl ID="PopupSelectorEmployees" runat="server" AllowDragging="True" CloseAction="OuterMouseClick" Modal="True" ClientInstanceName="PopupSelectorEmployeesClient"
        PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter" Height="660px" Width="990px" ClientSideEvents-Init="OnInitGroupSelector" CloseOnEscape="false"
        ShowHeader="False" ScrollBars="Auto" ShowPageScrollbarWhenModal="True" PopupAnimationType="None" BackColor="Transparent" ContentStyle-Paddings-Padding="0px" Border-BorderColor="Transparent" ShowShadow="false">
        <ContentCollection>
            <dx:PopupControlContentControl ID="PopupControlContentControl1" runat="server">
                <dx:ASPxPanel ID="ASPxPanel3" runat="server" Width="0px" Height="0px">
                    <PanelCollection>
                        <dx:PanelContent ID="PanelContent3" runat="server">
                            <div class=".transparentPopupExtended" style="width: 980px; height: 650px">
                                <table id="tbPopupFrame" cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td>
                                            <iframe id="ifEmployeeSelector" name="ifEmployeeSelector" runat="server" style="background-color: Transparent;" height="640" width="940" scrolling="no"
                                                frameborder="0" marginheight="0" marginwidth="0" src="" />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </dx:PanelContent>
                    </PanelCollection>
                </dx:ASPxPanel>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>

    <script language="javascript" type="text/javascript">

        function resizeTreeNotifications() {
            try {
                var ctlPrefix = "<%= roTreesNotifications.ClientID %>";
                eval(ctlPrefix + "_resizeTrees();");
            }
            catch (e) {
                showError("resizeTreeNotifications", e);
            }
        }

        function resizeFrames() {
            var divMainBodyHeight = $("#divMainBody").outerHeight(true);
            var divHeight = 0;
            if (divMainBodyHeight < 525) {
                divHeight = 525 - $("#divTabInfo").outerHeight(true);
            }
            else {
                divHeight = divMainBodyHeight - $("#divTabInfo").outerHeight();
            }

            $("#divTabData").height(divHeight - 10);

            var divTreeHeight = $("#divTree").height();
            $("#ctlTreeDiv").height(divTreeHeight);
        }

        window.onresize = function () {
            resizeFrames();
            resizeTreeNotifications();
        }
    </script>
</asp:Content>