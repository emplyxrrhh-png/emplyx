<%@ Page Language="vb" AutoEventWireup="false" Inherits="VTLive40.DocumentTemplateWizard" Title="Documentos" CodeBehind="DocumentTemplateWizard.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<%@ Register Src="~/Base/WebUserControls/frmBusinessCenterSelector.ascx" TagPrefix="roForms" TagName="frmBusinessCenterSelector" %>

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Asistente para modificar parámetros de seguridad</title>
</head>
<body class="bodyPopup" style="background-attachment: fixed;">

    <form id="frmSecurityActions" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />
        <input type="hidden" runat="server" id="IDLoadDocumentTemplate" value="-1" />

        <div>

            <script language="javascript" type="text/javascript">

                function PageBase_Load() {
                    cargaDocumentTemplate(document.getElementById('IDLoadDocumentTemplate').value);
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
            </script>

            <div class="popupWizardContentDocuments">
                <input type="hidden" id="hdnCaptionGrid" value="<%= Me.Language.Translate("CaptionGrid", Me.DefaultScope) %>" />
                <input type="hidden" id="msgHasChanges" value="" runat="server" clientidmode="Static" />
                <input type="hidden" id="msgHasErrors" value="" runat="server" clientidmode="Static" />

                <div id="divModalBgDisabled" class="modalBackground" style="position: absolute; left: 0px; top: 0px; z-index: 990; width: 1680px; height: 900px; display: none;"></div>
                <div style="width: 100%; height: 100%; vertical-align: top;">

                    <!-- ARBOL Y DETALLE -->
                    <div id="divTabData" class="divDataCells">
                        <div id="divContenido" class="divAllContent">
                            <div id="divContent" style="height: initial" class="maxHeight">

                                <div id="documentRow">
                                    <div id="divMenuTask" runat="server" style="width: 100%; margin: auto; display: flex; justify-content: center; margin-top: 1vw; margin-bottom: 1vw;">
                                        <table border="0" cellpadding="0" cellspacing="0">
                                            <tr>
                                                <td>
                                                    <div class="RoundCornerFrame">
                                                        <table>
                                                            <tr>
                                                                <td>
                                                                    <a id="TABBUTTON_00" href="javascript: void(0);" class="bTabDocuments-active" onclick="javascript: changeTabs(0);">
                                                                        <%=Me.Language.Translate("tabEstado", Me.DefaultScope)%></a>
                                                                </td>
                                                                <td>
                                                                    <a id="TABBUTTON_02" href="javascript: void(0);" class="bTabDocuments" onclick="javascript: changeTabs(2);">
                                                                        <%=Me.Language.Translate("tabScope", Me.DefaultScope)%></a>
                                                                </td>
                                                                <td>
                                                                    <a id="TABBUTTON_01" href="javascript: void(0);" class="bTabDocuments" onclick="javascript: changeTabs(1);">
                                                                        <%=Me.Language.Translate("tabControl", Me.DefaultScope)%></a>
                                                                </td>

                                                                <td>
                                                                    <a id="TABBUTTON_05" href="javascript: void(0);" class="bTabDocuments" onclick="javascript: changeTabs(5);">
                                                                        <%=Me.Language.Translate("tabLopd", Me.DefaultScope)%></a>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                    <dx:ASPxCallbackPanel ID="ASPxCallbackPanelContenido" runat="server" Width="100%" Height="100%" ClientInstanceName="ASPxCallbackPanelContenidoClient" CssClass="defaultContrastColor"
                                        Style="overflow: auto; min-height: 680px; width: 90%; height: 85%; min-width: 1100px; margin-top: 5px; vertical-align: top; margin-left: auto; margin-right: auto;">
                                        <SettingsLoadingPanel Enabled="false" />
                                        <ClientSideEvents EndCallback="ASPxCallbackPanelContenidoClient_EndCallBack" />
                                        <PanelCollection>
                                            <dx:PanelContent ID="PanelContent2" runat="server">
                                                <div id="divContentPanels" class="divContentPanelsWithOutMessage" style="min-height: 600px">
                                                    <!-- PANEL GENERAL -->
                                                    <div id="panDocGeneral" class="contentPanel" runat="server">
                                                        <!-- Este div es el header General -->
                                                        <div class="panBottomMargin">

                                                            <div class="divEmployee-Class">
                                                                <table border="0" width="100%" height="50px">
                                                                    <tr>
                                                                        <td width="48px" height="48px">
                                                                            <img id="Img3" src="~/Base/Images/StartMenuIcos/TaskTemplates.png" style="border: 0;" runat="server" /></td>
                                                                        <td valign="top" align="left">
                                                                            <span id="span1" runat="server" class="spanEmp-Class">
                                                                                <asp:Label ID="lblDocTemplateDesc" runat="server" Text="En esta sección puedes crear los documentos que serán exigibles a tus usuarios o empresas."></asp:Label>
                                                                            </span>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </div>
                                                        </div>
                                                        <!-- Este div es un formulario -->
                                                        <div class="panBottomMargin">
                                                            <div class="divRow">
                                                                <!-- Nombre-->
                                                                <div class="divRowDescription">
                                                                    <asp:Label ID="lblDocNameDescription" runat="server" Text="Nombre identificativo de la plantilla"></asp:Label>
                                                                </div>
                                                                <div class="panHeaderListOptionalGenius">
                                                                    <asp:Label ID="lblDocName" runat="server" Text="¿Cómo se llama el documento?"></asp:Label>
                                                                </div>

                                                                <div style="width: 95%; margin: 0 auto;">
                                                                    <div style="display: flex; justify-content: center;">
                                                                        <div class="componentForm">
                                                                            <dx:ASPxHiddenField ID="hiddenDocumentTemplateID" runat="server" ClientInstanceName="hiddenDocumentTemplateID_Client" />
                                                                            <dx:ASPxTextBox ID="txtDocName" MaxLength="50" runat="server" ClientInstanceName="txtDocName_Client" NullText="_____">
                                                                                <ClientSideEvents Validation="LengthValidation" TextChanged="function(s,e){}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                                <ValidationSettings SetFocusOnError="True" ValidationGroup="Document">
                                                                                    <RequiredField IsRequired="True" ErrorText="(*)" />
                                                                                </ValidationSettings>
                                                                            </dx:ASPxTextBox>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="divRow">
                                                                <div class="divRowDescription">
                                                                    <asp:Label ID="lblAreaDescription" runat="server" Text="El documento aplica a la siguiente área. El área de un documento fija quien supervisará los documentos de este tipo."></asp:Label>
                                                                </div>
                                                                <div class="panHeaderListOptionalGenius">
                                                                    <asp:Label ID="lblArea" runat="server" Text="¿Qué categoría tiene?"></asp:Label>
                                                                </div>

                                                                <div style="width: 95%; margin: 0 auto;">
                                                                    <div style="display: flex; justify-content: center;">
                                                                        <div class="componentForm" style="padding: 0px 17px 9px 0px !important">

                                                                            <dx:ASPxRadioButtonList ID="rblDocumentArea" runat="server" Border-BorderStyle="None" ValueField="ID" TextField="Name" RepeatColumns="8" RepeatLayout="Table" ClientInstanceName="rblDocumentArea_Client">
                                                                                <ClientSideEvents SelectedIndexChanged="function(s,e){ }" />
                                                                                <ValidationSettings SetFocusOnError="True" ValidationGroup="Document">
                                                                                    <RequiredField IsRequired="True" ErrorText="(*)" />
                                                                                </ValidationSettings>
                                                                            </dx:ASPxRadioButtonList>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div id="panDocControl" class="contentPanel" runat="server" style="display: none">
                                                        <!-- Este div es el header Criticidad-->

                                                        <!-- Este div es un formulario -->
                                                        <div class="panBottomMargin">
                                                            <div class="divRow">
                                                                <asp:Label ID="lblMandatoryDesc" runat="server" Text="Los documentos de esta carpeta son" CssClass="labelForm"></asp:Label>
                                                                <div class="componentForm">
                                                                    <roUserControls:roGroupBox ID="RoGroupBox8" runat="server">
                                                                        <Content>
                                                                            <dx:ASPxRadioButtonList ID="rblMandatory" runat="server" Border-BorderStyle="None"
                                                                                RepeatColumns="1" RepeatLayout="Table" RepeatDirection="Horizontal" ItemSpacing="10px" ClientInstanceName="rblMandatory_Client">
                                                                                <ClientSideEvents SelectedIndexChanged="rblMandatory_SelectedIndexChanged" />
                                                                                <ValidationSettings SetFocusOnError="True" ValidationGroup="Document">
                                                                                    <RequiredField IsRequired="True" ErrorText="(*)" />
                                                                                </ValidationSettings>
                                                                            </dx:ASPxRadioButtonList>
                                                                            <div class="divRow">
                                                                                <asp:Label ID="lblDocValidity" runat="server" Text="Todos los usuarios / empresas deben disponer de un documento válido en esta carpeta. Si no es así, se generará una alerta." CssClass="labelForm"></asp:Label>
                                                                                <div class="componentForm">
                                                                                    <roUserControls:roGroupBox ID="RoGroupBox4" runat="server">
                                                                                        <Content>
                                                                                            <div class="validate-div">
                                                                                                <dx:ASPxCheckBox ID="ckValidPeriod" runat="server" ClientInstanceName="ckValidPeriod_Client" Text="Aplicar sólo a partir de la fecha " CssClass="inline-ck">
                                                                                                    <ClientSideEvents CheckedChanged="ckValidPeriod_Checked" />
                                                                                                </dx:ASPxCheckBox>
                                                                                                <dx:ASPxDateEdit ID="dpPeriodStart" runat="server" Width="105" ClientInstanceName="dpPeriodStart_Client" CssClass="inline-dp" AllowNull="true">
                                                                                                    <ClientSideEvents DateChanged="function(s,e){}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                                                </dx:ASPxDateEdit>
                                                                                            </div>
                                                                                            <dx:ASPxCheckBox ID="ckNotification701" runat="server" ClientInstanceName="ckNotification701_Client">
                                                                                                <ClientSideEvents CheckedChanged="function(s,e){ }" />
                                                                                            </dx:ASPxCheckBox>
                                                                                            <span class="help"><%=Language.Translate("cknotification701.tooltip", Me.DefaultScope) %></span>
                                                                                            <dx:ASPxCheckBox ID="ckNotification702" runat="server" ClientInstanceName="ckNotification702_Client">
                                                                                                <ClientSideEvents CheckedChanged="function(s,e){ }" />
                                                                                            </dx:ASPxCheckBox>
                                                                                        </Content>
                                                                                    </roUserControls:roGroupBox>
                                                                                </div>
                                                                            </div>
                                                                        </Content>
                                                                    </roUserControls:roGroupBox>
                                                                </div>
                                                            </div>
                                                        </div>

                                                            <div class="divRow">
                                                                <div class="componentForm">
                                                                    <roUserControls:roGroupBox ID="RoGroupBox10" runat="server">
                                                                    <Content>
                                                                        <table>
                                                                            <tr>
                                                                                <td colspan="2">
                                                                                <dx:ASPxCheckBox ID="ckApproveRequiered" runat="server" ClientInstanceName="ckApproveRequiered_Client" Text="Los documentos de la carpeta requieren ser validados por un supervisor de nivel " CssClass="inline-ck">
                                                                                    <ClientSideEvents CheckedChanged="ckApproveRequiered_Checked" />
                                                                                </dx:ASPxCheckBox>
                                                                                <dx:ASPxTextBox runat="server" ID="txtRequieredSupervisorLevel" MaxLength="2" Width="50px" ClientInstanceName="txtRequieredSupervisorLevelClient" CssClass="inline-tx">
                                                                                    <MaskSettings Mask="<1..10>" />
                                                                                    <ClientSideEvents TextChanged="function(s,e){ }" />
                                                                                    <ValidationSettings ErrorDisplayMode="None" />
                                                                                </dx:ASPxTextBox>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td width="25"></td>
                                                                                <td>
                                                                                 <dx:ASPxCheckBox ID="ckNotification703" runat="server" ClientInstanceName="ckNotification703_Client" Text="Notificar al supervisor directo y al empleado cuando un documento sea rechazado" CssClass="inline-ck">
                                                                                    <ClientSideEvents CheckedChanged="function(s,e){ }" />
                                                                                </dx:ASPxCheckBox>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                     </Content>
                                                                     </roUserControls:roGroupBox>
                                                                </div>
                                                            </div>

                                                        <div class="divRow">
                                                            <asp:Label ID="Label3" runat="server" Text="Los documentos de la carpeta son válidos" CssClass="labelForm"></asp:Label>
                                                            <div class="componentForm">
                                                                <roUserControls:roGroupBox ID="RoGroupBox5" runat="server">
                                                                    <Content>
                                                                        <div class="validate-div">
                                                                            <dx:ASPxRadioButton ID="rbAlways" runat="server" ClientInstanceName="rbAlways_Client" Text="Siempre" GroupName="rbgValid">
                                                                                <ClientSideEvents CheckedChanged="rbValidUntil_Checked" />
                                                                            </dx:ASPxRadioButton>
                                                                            <div class="validate-div" style="padding-left: 0;">
                                                                                <dx:ASPxRadioButton ID="rbValidUntil" runat="server" ClientInstanceName="rbValidUntil_Client" Text="Durante " GroupName="rbgValid" CssClass="inline-ck">
                                                                                    <ClientSideEvents CheckedChanged="rbValidUntil_Checked" />
                                                                                </dx:ASPxRadioButton>
                                                                                <div class="inline-ck">
                                                                                    <dx:ASPxTextBox runat="server" ID="txtExpireDays" MaxLength="5" Width="50px" ClientInstanceName="txtExpireDaysClient" CssClass="inline-dp">
                                                                                        <MaskSettings Mask="<0..99999>" />
                                                                                        <ClientSideEvents TextChanged="function(s,e){ }" />
                                                                                        <ValidationSettings ErrorDisplayMode="None" />
                                                                                    </dx:ASPxTextBox>
                                                                                </div>
                                                                                <div>
                                                                                    <asp:Label ID="lblDaysAfter" CssClass="inline-ck" Style="padding-left: 10px" runat="server" Text="días a partir de la fecha de aprobación."></asp:Label>
                                                                                </div>
                                                                            </div>
                                                                            <dx:ASPxCheckBox ID="ckExpireOld" runat="server" ClientInstanceName="ckExpireOld_Client" Text="Al guardar un documento para un usuario, caduca el resto de documentos que tenga">
                                                                                <ClientSideEvents CheckedChanged="function(s,e){ }" />
                                                                            </dx:ASPxCheckBox>
                                                                            <br />
                                                                            <dx:ASPxCheckBox ID="ckRequieresSign" runat="server" ClientInstanceName="ckRequieresSign_Client" Text="Los documentos de la carpeta requieren ser firmados digitalmente">
                                                                                <ClientSideEvents CheckedChanged="function(s,e){ }" />
                                                                            </dx:ASPxCheckBox>
                                                                        </div>
                                                                    </Content>
                                                                </roUserControls:roGroupBox>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div id="panDocScope" class="contentPanel" runat="server" style="display: none">
                                                        <!-- Este div es el header Alcance-->

                                                        <!-- Este div es un formulario -->
                                                        <div class="panBottomMargin">
                                                            <div class="divRow">
                                                                <asp:Label ID="lblAmbit" runat="server" Text="Contiene información de:" CssClass="labelForm"></asp:Label>
                                                                <div class="componentForm">
                                                                    <dx:ASPxRadioButtonList ID="rblAmbit" runat="server" Border-BorderStyle="None"
                                                                        RepeatColumns="2" RepeatLayout="Table" RepeatDirection="Horizontal" ItemSpacing="10px" ClientInstanceName="rblAmbit_Client">
                                                                        <ClientSideEvents SelectedIndexChanged="rblAmbit_SelectedIndexChanged" />
                                                                        <ValidationSettings SetFocusOnError="True" ValidationGroup="Document">
                                                                            <RequiredField IsRequired="True" ErrorText="(*)" />
                                                                        </ValidationSettings>
                                                                    </dx:ASPxRadioButtonList>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <!-- Este div es un formulario -->
                                                        <div class="panBottomMargin">
                                                            <div class="divRow">
                                                                <div class="divRowDescription">
                                                                    <asp:Label ID="lblAlcanceDes" runat="server" Text="El alcance fija a quién se requerirá el documento, así como la actividad para el desarrollo de la cual será necesario."></asp:Label>
                                                                </div>
                                                                <asp:Label ID="lblDocScope" runat="server" Text="Alcance:" CssClass="labelForm"></asp:Label>
                                                                <div class="componentForm">
                                                                    <asp:Panel ID="pnlScope" runat="server" ClientIDMode="Static">
                                                                        <dx:ASPxRadioButton ID="rblCompany" ClientInstanceName="rblCompany_Client" runat="server" GroupName="rbgScope" CssClass="rbgScopeCompany">
                                                                        </dx:ASPxRadioButton>
                                                                        <span class="rbgScopeCompany help"><%=Language.Translate("rblcompany.tooltip", Me.DefaultScope) %></span>
                                                                        <dx:ASPxRadioButton ID="rblCompanyAccessAuthorization" ClientInstanceName="rblCompanyAccessAuthorization_Client" runat="server" GroupName="rbgScope" CssClass="rbgScopeCompany">
                                                                            <ClientSideEvents CheckedChanged="rblAccessAuthorization_Checked" />
                                                                        </dx:ASPxRadioButton>
                                                                        <span class="rbgScopeCompany help"><%=Language.Translate("rblcompanyaccessauthorization.tooltip", Me.DefaultScope) %></span>
                                                                        <dx:ASPxRadioButton ID="rblEmployeeContract" ClientInstanceName="rblEmployeeContract_Client" runat="server" GroupName="rbgScope" CssClass="rbgScopeEmployee">
                                                                        </dx:ASPxRadioButton>
                                                                        <span class="rbgScopeEmployee help"><%=Language.Translate("rblemployeecontract.tooltip", Me.DefaultScope) %></span>

                                                                        <div id="divDocumentAccess" runat="server">
                                                                            <dx:ASPxRadioButton ID="rblEmployeeAccessAuthorization" ClientInstanceName="rblEmployeeAccessAuthorization_Client" runat="server" GroupName="rbgScope" CssClass="rbgScopeEmployee">
                                                                                <ClientSideEvents CheckedChanged="rblAccessAuthorization_Checked" />
                                                                            </dx:ASPxRadioButton>
                                                                            <span class="rbgScopeEmployee help"><%=Language.Translate("rblemployeeaccessauthorization.tooltip", Me.DefaultScope) %></span>
                                                                            <div class="divRow">
                                                                                <div class="divRowDescription">
                                                                                    <asp:Label ID="lblCriticalityDes" runat="server" Text="Indique la criticidad de la no conformidad de este documento."></asp:Label>
                                                                                </div>
                                                                                <asp:Label ID="lblCriticality" runat="server" Text="Criticidad:" CssClass="labelForm"></asp:Label>
                                                                                <div class="componentForm">
                                                                                    <roUserControls:roGroupBox ID="RoGroupBox3" runat="server">
                                                                                        <Content>
                                                                                            <dx:ASPxRadioButton ID="rbnNonCriticality" ClientInstanceName="rbnNonCriticality_Client" runat="server" Text="No critico" ToolTip="No se tomará ninguna acción ante la no conformidad del documento" GroupName="Criticality">
                                                                                                <ClientSideEvents CheckedChanged="function(s,e){}" />
                                                                                            </dx:ASPxRadioButton>
                                                                                            <dx:ASPxRadioButton ID="rbnAdviceCriticality" ClientInstanceName="rbnAdviceCriticality_Client" runat="server" Text="Avisar" ToolTip="Se avisará de la no conformidad. Si existe un control de acceso, se permitirá el acceso" GroupName="Criticality">
                                                                                                <ClientSideEvents CheckedChanged="function(s,e){}" />
                                                                                            </dx:ASPxRadioButton>
                                                                                            <dx:ASPxRadioButton ID="rbnDeniedCriticality" ClientInstanceName="rbnDeniedCriticality_Client" runat="server" Text="Denegar acceso" ToolTip="si existe un control de acceso, no se permitirá el acceso" GroupName="Criticality">
                                                                                                <ClientSideEvents CheckedChanged="function(s,e){}" />
                                                                                            </dx:ASPxRadioButton>
                                                                                        </Content>
                                                                                    </roUserControls:roGroupBox>
                                                                                </div>
                                                                            </div>
                                                                        </div>

                                                                        <dx:ASPxRadioButton ID="rblLeaveOrPermission" ClientInstanceName="rblLeaveOrPermission_Client" runat="server" GroupName="rbgScope" CssClass="rbgScopeEmployee">
                                                                        </dx:ASPxRadioButton>
                                                                        <span class="rbgScopeEmployee help"><%=Language.Translate("rblleaveorpermission.tooltip", Me.DefaultScope) %></span>
                                                                        <dx:ASPxRadioButton ID="rblCauseNote" ClientInstanceName="rblCauseNote_Client" runat="server" GroupName="rbgScope" CssClass="rbgScopeEmployee">
                                                                        </dx:ASPxRadioButton>
                                                                        <span class="rbgScopeEmployee help"><%=Language.Translate("rblcausenote.tooltip", Me.DefaultScope) %></span>
                                                                    </asp:Panel>
                                                                </div>
                                                            </div>
                                                        </div>

                                                        <!-- Este div es un formulario -->
                                                        <div class="panBottomMargin">
                                                            <div class="divRow">
                                                                <div class="divRowDescription">
                                                                    <asp:Label ID="Label1" runat="server" Text="Indica quien puede aportar el documento y su visibilidad."></asp:Label>
                                                                </div>
                                                                <asp:Label ID="Label2" runat="server" Text="Alcance:" CssClass="labelForm"></asp:Label>
                                                                <div class="componentForm">

                                                                    <table>
                                                                        <tr>
                                                                            <td colspan="2">
                                                                                <dx:ASPxCheckBox ID="ckCanAddDocumentEmployee" runat="server" ClientInstanceName="ckCanAddDocumentEmployeeClient" Text="Lo pueden presentar electrónicamente los empleados">
                                                                                    <ClientSideEvents CheckedChanged="function(s,e){ ckNotification700_Client.SetEnabled(s.GetChecked()); if(!s.GetChecked())ckNotification700_Client.SetChecked(false) }" />
                                                                                </dx:ASPxCheckBox>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td width="25"></td>
                                                                            <td>
                                                                                <dx:ASPxCheckBox ID="ckNotification700" runat="server" ClientInstanceName="ckNotification700_Client">
                                                                                    <ClientSideEvents CheckedChanged="function(s,e){ }" />
                                                                                </dx:ASPxCheckBox>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td colspan="2">
                                                                                <dx:ASPxCheckBox ID="ckCanAddDocumentSupervisor" runat="server" ClientInstanceName="ckCanAddDocumentSupervisorClient" Text="Lo pueden presentar electrónicamente los supervisores">
                                                                                    <ClientSideEvents CheckedChanged="function(s,e){ }" />
                                                                                </dx:ASPxCheckBox>
                                                                            </td>
                                                                        </tr>
                                                                        <tr style="display: none;">
                                                                            <td colspan="2">
                                                                                <dx:ASPxCheckBox ID="ckSystemDocument" runat="server" ClientInstanceName="ckSystemDocumentClient" Text="Es un documento de sistema" Enabled="false">
                                                                                    <ClientSideEvents CheckedChanged="function(s,e){ }" />
                                                                                </dx:ASPxCheckBox>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div id="panDocLOPD" class="contentPanel" runat="server" style="display: none">
                                                        <!-- Este div es el header Notificaciones-->

                                                        <!-- Este div es un formulario -->
                                                        <div class="panBottomMargin">
                                                            <div class="divRow">
                                                                <div class="divRowDescription">
                                                                    <asp:Label ID="lblLopdLevelDescription" runat="server" Text="Indique el nivel de la información que contiene el documento."></asp:Label>
                                                                </div>
                                                                <asp:Label ID="lblLopdLevel" runat="server" Text="Nivel" CssClass="labelForm"></asp:Label>
                                                                <div class="componentForm">
                                                                    <dx:ASPxComboBox ID="cmbLopdLevel" runat="server" ValueField="ID" TextField="Name" RepeatColumns="1" RepeatLayout="Table" ClientInstanceName="cmbLopdLevel_Client">
                                                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ }" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                    </dx:ASPxComboBox>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <!-- Este div es un formulario -->
                                                        <div class="panBottomMargin">
                                                            <div class="divRow">
                                                                <asp:Label ID="lblDocumentExpires" runat="server" Text="Dias" CssClass="labelForm"></asp:Label>
                                                                <div class="componentForm">
                                                                    <roUserControls:roGroupBox ID="RoGroupBox9" runat="server">
                                                                        <Content>
                                                                            <dx:ASPxRadioButton ID="rbnPanelExpireOnServer" runat="server" GroupName="RoGroupBox9" Text="El número de días indicado en la sección de configuración de Auditoría de datos"></dx:ASPxRadioButton>
                                                                            <dx:ASPxRadioButton ID="rbnPanelnoExpire" runat="server" GroupName="RoGroupBox9" Text="Siempre. Nunca se eliminan"></dx:ASPxRadioButton>
                                                                        </Content>
                                                                    </roUserControls:roGroupBox>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </dx:PanelContent>
                                        </PanelCollection>
                                    </dx:ASPxCallbackPanel>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="popupWizardButtons">
                <table align="right" cellpadding="0" cellspacing="0">
                    <tr class="SchedulerWizards_ButtonsPanel" style="height: 44px">
                        <td>&nbsp
                        </td>
                        <td>
                            <asp:Button ID="btNext" Text="${Button.Next}" runat="server" OnClientClick="saveChangesDocument();return false;" TabIndex="2" CssClass="btnFlat btnFlatBlack btnFlatAsp" />
                        </td>
                        <td>
                            <asp:Button ID="btClose" Text="${Button.Cancel}" runat="server" OnClientClick="Close(); return false;" TabIndex="5" CssClass="btnFlat btnFlatBlack btnFlatAsp" />

                            <asp:HiddenField ID="hdnCanClose_PageBase" runat="server" />
                            <asp:HiddenField ID="hdnMustRefresh_PageBase" Value="0" runat="server" />
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </form>
</body>
</html>