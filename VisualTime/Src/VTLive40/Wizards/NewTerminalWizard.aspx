<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.Forms_NewTerminalWizard" Culture="auto" UICulture="auto" CodeBehind="NewTerminalWizard.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Asistente para el alta de ${Terminals}</title>
</head>
<body class="bodyPopup" style="background-attachment: fixed;">

    <form id="frmNewTerminalWizard" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <div>

            <script language="javascript" type="text/javascript">
                function PageBase_Load() { }

                /* addCssClass: Afegeix una nova Clase a un objecte */
                /*************************************************************************************************************/
                function addCssClass(obj, clsTxt) {
                    obj.className = obj.className + ' ' + clsTxt;
                }

                /* removeCssClass: Elimina una Clase a un objecte */
                /*************************************************************************************************************/
                function removeCssClass(obj, clsTxt) {
                    var parmCss = new Array();
                    parmCss = obj.className.split(" ");

                    obj.className = ''; //Reset dels CSS
                    //Carreguem tots els anteriors atributs
                    for (nCss = 0; nCss < parmCss.length; nCss++) {
                        if (parmCss[nCss] != clsTxt) {
                            obj.className = obj.className + ' ' + parmCss[nCss];
                        }
                    }
                }

                /* onmouseour Row (tr) */
                function rowOver(rowID) {
                    var table = document.getElementById(rowID);
                    var cells = table.getElementsByTagName("td");
                    for (var i = 0; i < cells.length; i++) {
                        addCssClass(cells[i], "gridRowOver");
                    }
                }

                /* onmouseout Row (tr) */
                function rowOut(rowID) {
                    var table = document.getElementById(rowID);
                    var cells = table.getElementsByTagName("td");
                    for (var i = 0; i < cells.length; i++) {
                        removeCssClass(cells[i], "gridRowOver");
                    }
                }

                /* selecció Row (tr) */
                function rowClick(rowID, ID, dTable) {
                    //alert('ID=' + ID);
                    document.getElementById('<%= TermToReplaceID.ClientID %>').value = ID;
                    var tParent = document.getElementById(dTable);
                    var tCells = tParent.getElementsByTagName("td");
                    for (var i = 0; i < tCells.length; i++) {
                        removeCssClass(tCells[i], "gridRowOver");
                        removeCssClass(tCells[i], "gridRowSelected");
                    }

                    var table = document.getElementById(rowID);
                    var cells = table.getElementsByTagName("td");
                    for (var i = 0; i < cells.length; i++) {
                        removeCssClass(cells[i], "gridRowOver");
                        addCssClass(cells[i], "gridRowSelected");
                    }
                }
            </script>

            <asp:UpdatePanel ID="upNewTerminalWizard" runat="server" RenderMode="Inline">
                <ContentTemplate>

                    <div class="popupWizardContent">
                        <%-- WELCOME --%>
                        <div id="divStep0" runat="server" style="display: block">
                            <table id="tbStep0" style="width: 100%; height: 100%;" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td style="height: 360px">
                                        <asp:Image ID="imgWelcome" runat="server" Style="border-radius: 5px;" ImageUrl="~/Base/Images/Wizards/wzconnect.gif" />
                                    </td>
                                    <td style="padding-left: 20px; padding-right: 20px; padding-top: 50px" valign="top">

                                        <asp:Label ID="lblWelcome1" runat="server" Text="Bienvenido al asistente para registro de ${Terminals}."
                                            Font-Bold="True" Font-Size="Large"></asp:Label>
                                        <br />
                                        <br />
                                        <br />
                                        <asp:Label ID="lblWelcome2" runat="server" Text="Este asistente le ayudará a registrar un ${Terminal}." Font-Bold="true"></asp:Label>
                                        <br />
                                        <br />
                                        <asp:Label ID="lblWelcome3" runat="server" Text="Para continuar, haga clic en siguiente."></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="height: 10px" colspan="2" class="NewTerminalWizards_ButtonsPanel"></td>
                                </tr>
                            </table>
                        </div>

                        <asp:Label ID="hdnStepTitle" Text="Asistente para el registro de ${Terminals}. " runat="server" Style="display: none; visibility: hidden" />

                        <div id="divStep1" runat="server" style="display: none;">
                            <table id="tbStep1" runat="server" style="" cellspacing="0" cellpadding="0" border="0">
                                <tr>
                                    <td class="NewTerminalWizards_StepTitle" valign="top" style="">
                                        <asp:Label ID="lblStep1Title" runat="server" Text="Paso 1 de 4." Font-Bold="True" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="NewTerminalWizards_StepError popupWizardError" style="">
                                        <asp:Label ID="lblStep1Error" runat="server" CssClass="errorText" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="NewTerminalWizards_StepContent" valign="top" style="padding-top: 10px;">
                                        <table border="0" style="width: 100%;">
                                            <tr>
                                                <td colspan="2" style="padding-left: 10px; padding-bottom: 10px;">
                                                    <asp:Label ID="lblTerminalInfoTitle" runat="server" Text="Información del Terminal" Font-Bold="true"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 15px; width: 50%;">
                                                    <asp:Label ID="lblTerminalSN" runat="server" Text="Núm. de Serie:"></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:Label ID="TerminalSN" runat="server" Text=""></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 15px; width: 50%;">
                                                    <asp:Label ID="lblTerminalModel" runat="server" Text="Módelo del Terminal:"></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:Label ID="TerminalModel" runat="server" Text=""></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-left: 15px; width: 50%;">
                                                    <asp:Label ID="lblTerminalIP" runat="server" Text="IP del Terminal:"></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:Label ID="TerminalIP" runat="server" Text=""></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2" style="padding-top: 30px; padding-left: 10px; padding-bottom: 10px;">
                                                    <asp:Label ID="lblTerminalInfoRegister" runat="server" Text="Núm. de Licencia del Terminal" Font-Bold="true"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2" align="center">
                                                    <table border="0" style="width: 90%; height: 100%;">
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="lblTerminalRegisterDesc" runat="server" Text="Inserte la licencia facilitada para poder activar el Terminal y continuar con el registro."></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td style="padding-top: 10px;" align="center">
                                                                <asp:TextBox ID="txtRegister" runat="server" Text="" Width="350px" CssClass="textClass" onkeypress="return onEnterPress(event,'__doPostBack(\'btNext\',\'\')');"></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </div>

                        <div id="DivStep2" runat="server" style="display: none">
                            <table id="btStep2" style="" cellspacing="0" cellpadding="0" border="0">
                                <tr>
                                    <td class="NewTerminalWizards_StepTitle" valign="top" style="">
                                        <asp:Label ID="lblStep2Title" runat="server" Text="Paso 2 de 4." Font-Bold="True" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="NewTerminalWizards_StepError popupWizardError" style="">
                                        <asp:Label ID="lblStep2Error" runat="server" CssClass="errorText" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="NewTerminalWizards_StepContent" valign="top" style="padding-top: 10px;">
                                        <table border="0" style="width: 100%;">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblTerminalTypeDesc" runat="server" Text="Seleccione la acción que corresponda realizar." />
                                                    <br />
                                                    <table>
                                                        <tr>
                                                            <td style="padding-top: 10px">
                                                                <roUserControls:roOptionPanelContainer ID="optNewTerminal" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto"
                                                                    Text="Es un nuevo ${Terminal}"
                                                                    Description="El ${Terminal} se crea nuevamente, no se sustituye otro terminal."
                                                                    Checked="true">
                                                                    <Content></Content>
                                                                </roUserControls:roOptionPanelContainer>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td style="padding-top: 10px">
                                                                <roUserControls:roOptionPanelContainer ID="optReplaceTerminal" runat="server" TypeOPanel="RadioOption" Width="100%" Height="Auto"
                                                                    Text="Se reemplaza un ${Terminal} existente"
                                                                    Description="Se sustituye un ${Terminal} existente por el nuevo Terminal. Se mantiene la configuración del anterior."
                                                                    Enabled="true">
                                                                    <Content></Content>
                                                                </roUserControls:roOptionPanelContainer>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                    <roUserControls:roOptionPanelGroup ID="optTerminalGroup" runat="server" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </div>

                        <div id="divStep3" runat="server" style="display: none">
                            <table id="tbStep3" runat="server" style="width: 100%; height: 100%" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td class="NewTerminalWizards_StepTitle" valign="top">
                                        <asp:Label ID="lblStep3Title" runat="server" Text="Paso 3 de 4." Font-Bold="True" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="NewTerminalWizards_StepContent" valign="top" style="padding-top: 10px;">
                                        <table border="0" style="width: 100%;">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblReplaceTermDesc" runat="server" Text="Seleccione el terminal que desea reemplazar." />
                                                    <br />
                                                    <table>
                                                        <tr>
                                                            <td style="padding-top: 10px; padding-left: 20px;" align="center">
                                                                <!-- Grid Terminals -->
                                                                <div style="width: 420px; height: 280px; display: block; text-align: left; border: solid 1px #D2DCE4;" runat="server" id="grdTerminales">
                                                                </div>
                                                                <asp:HiddenField ID="TermToReplaceID" runat="server" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="NewTerminalWizards_StepError" style="height: 20px;">
                                        <asp:Label ID="lblStep3Error" runat="server" CssClass="errorText" />
                                    </td>
                                </tr>
                            </table>
                        </div>

                        <div id="divStep4" runat="server" style="display: none;">
                            <table id="tbStep4" runat="server" style="" cellspacing="0" cellpadding="0" border="0">
                                <tr>
                                    <td class="NewTerminalWizards_StepTitle" valign="top" style="height: 20px;">
                                        <asp:Label ID="lblStep4Title" runat="server" Text="Paso 4 de 4." Font-Bold="True" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="NewTerminalWizards_StepContent" valign="top" style="padding-top: 10px;">
                                        <table border="0" style="width: 100%;">
                                            <tr>
                                                <td colspan="2" align="center">
                                                    <table border="0" style="width: 90%; height: 100%;">
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="lblInsertName" runat="server" Text="Inserte el nombre descriptivo que desea para el terminal."></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td style="padding-top: 10px;" align="center">
                                                                <asp:TextBox ID="txtNameTerminal" runat="server" Text="" Width="350px" CssClass="textClass" onkeypress="return onEnterPress(event,'__doPostBack(\'btEnd\',\'\')');"></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="NewTerminalWizards_StepError" style="height: 20px;">
                                        <asp:Label ID="lblStep4Error" runat="server" CssClass="errorText" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>

                    <div class="popupWizardButtons">
                        <table align="right" cellpadding="0" cellspacing="0">
                            <tr class="NewTerminalWizards_ButtonsPanel" style="height: 44px">
                                <td>&nbsp
                                </td>
                                <td>
                                    <asp:Button ID="btPrev" Text="${Button.Previous}" runat="server" OnClientClick="" Visible="false" TabIndex="1" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                </td>
                                <td>
                                    <asp:Button ID="btNext" Text="${Button.Next}" runat="server" OnClientClick="" TabIndex="2" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                    <asp:Button ID="btEnd" Text="${Button.End}" runat="server" Visible="false" TabIndex="4" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                                </td>
                                <td>
                                    <asp:Button ID="btClose" Text="${Button.Cancel}" runat="server" OnClientClick="Close(); return false;" TabIndex="5" CssClass="btnFlat btnFlatBlack btnFlatAsp" />

                                    <asp:HiddenField ID="hdnCanClose_PageBase" runat="server" />
                                    <asp:HiddenField ID="hdnMustRefresh_PageBase" Value="0" runat="server" />
                                </td>
                            </tr>
                        </table>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>

            <Local:MessageFrame ID="MessageFrame1" runat="server" />
        </div>
    </form>
</body>
</html>