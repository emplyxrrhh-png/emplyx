<%@ Page Language="VB" AutoEventWireup="false" Inherits="VTLive40.SupervisorsContent" Title="Supervisores" ValidateRequest="false" CodeBehind="SupervisorsContent.aspx.vb" %>

<%@ Register Src="~/Security/WebUserControls/frmIPs.ascx" TagName="frmIPs" TagPrefix="roForms" %>
<%@ Register Src="~/Base/WebUserControls/roTreeV3b.ascx" TagName="roTreeV3" TagPrefix="rws" %>
<%@ Register Src="~/Security/WebUserControls/frmNewRequestCategory.ascx" TagName="frmNewRequestCategory" TagPrefix="roFormsRC" %>
<%@ Register Src="~/Security/WebUserControls/frmnewUserException.ascx" TagName="frmNewUserException" TagPrefix="roFormsUE" %>

<html lang="en" class="notranslate" translate="no" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="robots" content="noindex" />
    <meta name="googlebot" content="noindex" />
    <meta name="robots" content="nofollow" />
    <meta name="robots" content="nosnippet" />
    <meta name="robots" content="noarchive" />

    <title><span class="notranslate">Supervisores</span></title>
    <link rel="shortcut icon" href="~/Base/Images/logovtl.ico" />

    <script language="javascript" type="text/javascript">
        var supervisorLoaded = false;
    </script>
    <style>
        .supervisorsContent div#ASPxCallbackPanelContenido_objContainerTreeV3_roTrees1GroupTree_tree-div {
            max-width: 30rem;
        }
    </style>
</head>
<body>
    <form id="form1" class="supervisorsContent" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True" />

        <div id="divModalBgDisabled" class="modalBackground" style="position: absolute; left: 0px; top: 0px; z-index: 990; width: 1680px; height: 900px; display: none;"></div>
        <input type="hidden" runat="server" id="hdnValueGridName" />

        <input type="hidden" id="msgHasChanges" value="" runat="server" clientidmode="Static" />
        <input type="hidden" id="msgHasErrors" value="" runat="server" clientidmode="Static" />
        <input type="hidden" runat="server" clientidmode="Static" id="hdnStrEmpName" value="" />
        <input type="hidden" runat="server" clientidmode="Static" id="hdnStrCategoryName" value="" />
        <input type="hidden" runat="server" clientidmode="Static" id="hdnStrLevelName" value="" />
        <input type="hidden" runat="server" clientidmode="Static" id="hdnStrNextLevelName" value="" />
        <dx:ASPxHiddenField ID="hdnGroupsConfig" runat="server" ClientInstanceName="hdnGroupsConfigClient"></dx:ASPxHiddenField>
        <div id="divMainBody" style="min-height: inherit;">

            <!-- ARBOL Y DETALLE -->
            <div id="divTabData" class="divDataCells">
                <div id="divContenido">
                    <!-- WIDTH 100%, Y REVISAR ESTRUCTURA HTML -->
                    <!-- AL CREAR NUEVO USER, QUE SE ACTUALIZE ARBOL SUPERIOR -->
                    <div id="divContent" class="maxHeight">
                        <dx:ASPxCallbackPanel ID="ASPxCallbackPanelContenido" runat="server" Width="100%" Height="100%" ClientInstanceName="ASPxCallbackPanelContenidoClient">
                            <SettingsLoadingPanel Enabled="false" />
                            <ClientSideEvents EndCallback="ASPxCallbackPanelContenidoClient_EndCallBack" />
                            <PanelCollection>
                                <dx:PanelContent ID="PanelContent1" runat="server">
                                    <input type="hidden" runat="server" id="hdnTreeGroups" clientidmode="Static" />
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
                                        <!-- PANEL GENERAL -->
                                        <div id="divGeneral" class="contentPanel" style="display: none;" runat="server">
                                            <table cellpadding="0" cellspacing="0" width="100%" border="0" style="height: 100%;">
                                                <tr>
                                                    <td valign="top" style="padding-top: 2px;">

                                                        <div id="supervisorDefinition">

                                                            <!-- Area de DEFINICIÓN -->
                                                            <table width="100%" style="margin-bottom: 13px;" class="supervisorsDefinition">
                                                                <tr>
                                                                    <td>
                                                                        <div class="panHeader2">
                                                                            <span style="">
                                                                                <asp:Label runat="server" ID="lblDefTitle" Text="Definición"></asp:Label></span>
                                                                        </div>
                                                                        <br />
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td valign="top" align="left" style="padding-top: 5px;">
                                                                        <div class="RoundCornerFrame roundCorner">
                                                                            <table style="float: left;">
                                                                                <tr style="height: 50px;">
                                                                                    <td align="right" valign="top" style="padding-left: 20px;">
                                                                                        <asp:Label ID="lblName" CssClass="editTextFormat" runat="server" Text="Nombre"></asp:Label>
                                                                                    </td>
                                                                                    <td align="left" valign="top" style="padding-left: 10px; padding-right: 10px;">
                                                                                        <dx:ASPxTextBox ID="txtName" runat="server" ClientInstanceName="txtName_Client" NullText="_____">
                                                                                            <ClientSideEvents Validation="LengthValidation" TextChanged="function(s,e){checkSupervisorEmptyName(s.GetValue());}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                                            <ValidationSettings SetFocusOnError="True">
                                                                                                <RequiredField IsRequired="True" ErrorText="(*)" />
                                                                                            </ValidationSettings>
                                                                                        </dx:ASPxTextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr style="height: 50px;">
                                                                                    <td align="right" valign="top" style="padding-left: 20px;">
                                                                                        <asp:Label ID="lblDescription" CssClass="editTextFormat" runat="server" Text="Descripción"></asp:Label>
                                                                                    </td>
                                                                                    <td align="left" valign="top" style="padding-left: 10px; padding-right: 10px;">
                                                                                        <dx:ASPxMemo ID="txtDescription" runat="server" Rows="4" Width="100%" Height="30px">
                                                                                            <ClientSideEvents TextChanged="function(s,e){ hasChanges(true)}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                                        </dx:ASPxMemo>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr id="trSecurityFunction" runat="server" style="height: 40px;">
                                                                                    <td align="right" style="padding-left: 20px;">
                                                                                        <asp:Label ID="lblSecurityFunction" CssClass="editTextFormat" runat="server" Text="Rol"></asp:Label>
                                                                                    </td>
                                                                                    <td align="left" style="padding-left: 10px;">
                                                                                        <div style="float: left">
                                                                                            <dx:ASPxComboBox ID="cmbSecurityFunctions" runat="server" Width="170px">
                                                                                                <ClientSideEvents ValueChanged="function(s,e){ hasChanges(true); }" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                                            </dx:ASPxComboBox>
                                                                                        </div>
                                                                                        <%-- <div style="float: left">
                                                                                    <dx:ASPxCheckBox ID="ckIsSelfSupervised" Text="Puede supervisarse a si mismo" runat="server" Width="100%" ClientEnabled="True" Visible="false">
                                                                                        <ClientSideEvents ValueChanged="function(s,e){ hasChanges(true); }" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                                    </dx:ASPxCheckBox>
                                                                                </div>--%>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                            <table style="padding-left: 10%;">

                                                                                <tr id="trLanguage" runat="server" style="height: 40px;">
                                                                                    <td align="right" style="padding-left: 20px;">
                                                                                        <asp:Label ID="lblLanguage" CssClass="editTextFormat" runat="server" Text="Idioma predeterminado"></asp:Label>
                                                                                    </td>
                                                                                    <td align="left" style="padding-left: 10px;">
                                                                                        <dx:ASPxComboBox ID="cmbLanguage" runat="server" Width="170px">
                                                                                            <ClientSideEvents ValueChanged="function(s,e){ hasChanges(true); }" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                                        </dx:ASPxComboBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr id="trDate" runat="server" style="height: 40px;">
                                                                                    <td align="right" style="padding-left: 20px;">
                                                                                        <asp:Label ID="lblStartDate" CssClass="editTextFormat" runat="server" Text="Válido desde el "></asp:Label>
                                                                                    </td>
                                                                                    <td colspan="2">
                                                                                        <table cellpadding="0" cellspacing="0" border="0">
                                                                                            <tr>
                                                                                                <td align="left" style="padding-left: 10px; width: 100px;">
                                                                                                    <dx:ASPxDateEdit ID="txtStartDate" Width="105px" runat="server" AllowNull="true">
                                                                                                        <ClientSideEvents DateChanged="function(s,e){hasChanges(true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                                                    </dx:ASPxDateEdit>
                                                                                                </td>
                                                                                                <td align="left" style="padding-left: 10px; width: 50px;">
                                                                                                    <asp:Label ID="lblExpirationDate" CssClass="editTextFormat" runat="server" Text=" hasta el "></asp:Label>
                                                                                                </td>
                                                                                                <td>
                                                                                                    <dx:ASPxDateEdit ID="txtFinishDate" Width="105px" runat="server" AllowNull="true">
                                                                                                        <ClientSideEvents DateChanged="function(s,e){hasChanges(true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                                                    </dx:ASPxDateEdit>
                                                                                                </td>
                                                                                            </tr>
                                                                                        </table>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr id="trState" runat="server" style="height: 40px;">
                                                                                    <td align="right" style="padding-left: 20px;">
                                                                                        <asp:Label ID="lblState" Text="${Passport} activo" CssClass="editTextFormat" runat="server"></asp:Label>
                                                                                    </td>
                                                                                    <td align="left" style="padding-left: 10px;">
                                                                                        <input id="chkState" type="checkbox" runat="server" onchange="hasChanges(true);"
                                                                                            class="textClass x-form-text x-form-field" onblur="this.className='textClass x-form-text x-form-field';" onfocus="this.className='textClass x-form-text x-form-field x-form-focus';" />
                                                                                    </td>
                                                                                </tr>
                                                                                <tr id="trMail" runat="server">
                                                                                    <td align="right" style="padding-left: 20px;" valign="middle">
                                                                                        <asp:Label ID="lblEmailAddress" CssClass="editTextFormat" Text="Correo electrónico" runat="server"></asp:Label>
                                                                                    </td>
                                                                                    <td align="left" valign="top" style="padding-left: 10px;">
                                                                                        <dx:ASPxTextBox ID="txtEmailAddress" Width="300px" runat="server" ClientInstanceName="txtEmailAddress_Client">
                                                                                            <ClientSideEvents TextChanged="function(s,e){hasChanges(true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                                            <ValidationSettings ErrorDisplayMode="None">
                                                                                                <RequiredField IsRequired="false" />
                                                                                            </ValidationSettings>
                                                                                        </dx:ASPxTextBox>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </div>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </div>


                                                        <div id="usersSelector">
                                                            <div id="usersSelectorTitle" style="width:100%;padding-bottom:20px;min-width: 1480px;">
                                                                 <table width="100%">
                                                                     <tr>
                                                                         <td>
                                                                            <div class="panHeader2">
                                                                                <span style="">
                                                                                    <asp:Label runat="server" ID="lblUsersSecurityTitle" Text="Usuarios y grupos supervisados"></asp:Label></span>
                                                                            </div>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>
                                                                             <table border="0">
                                                                                 <tr>
                                                                                     <td valign="top" align="left">
                                                                                         <span class="spanEmp-Class">
                                                                                             <asp:Label ID="lblUsersSecurityDescription" runat="server" Text="A continuación podrá seleccionar los usuarios y grupos que el supervisor puede gestionar."></asp:Label></span>
                                                                                     </td>
                                                                                 </tr>
                                                                                 <tr>
                                                                                    <td valign="top" align="left">
                                                                                        <span class="spanEmp-Class">
                                                                                            <asp:Label ID="lblUserExceptionDescription" runat="server" Text="También puede añadir excepciones sobre usuarios. Por ejemplo, si el supervisor tiene un superior que gestione sus solicitudes, calendario, etc..."></asp:Label></span>
                                                                                    </td>
                                                                                </tr>
                                                                             </table>
                                                                        </td>
                                                                    </tr>
                                                                </table> 
                                                            </div>

                                                            <div id="groupsSelector" style="clear:both;width: calc(50% - 45px); margin-bottom: 13px; margin-left:20px; margin-right: 20px; float: left">

                                                                <table width="100%">
                                                                    <tr>
                                                                        <td>
                                                                            <div class="panHeader2" style="background-color:#85a6ff !important">
                                                                                <span style="">
                                                                                    <asp:Label runat="server" ID="lblGroupAndUsersTree" Text="Árbol de grupos y usuarios"></asp:Label></span>
                                                                            </div>
                                                                            <br />
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>
                                                                            <table border="0">
                                                                                <tr>
                                                                                    <td valign="top" align="left">
                                                                                        <span class="spanEmp-Class">
                                                                                            <asp:Label ID="lblGroupTreeDesc" runat="server" Text="Selecciones los usuarios y grupos a incluir."></asp:Label></span>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td valign="top">
                                                                                        <div style="border: thin solid silver; height: 320px; width: 700px; padding: 3px;" class="defaultBackgroundColor">
                                                                                            <rws:roTreeV3 ID="objContainerTreeV3" OnlyGroups="false" Embedded="True" PrefixTree="roTrees1GroupTree" PrefixCookie="ASPxCallbackPanelContenido_objContainerTreeV3_roTrees1GroupTree" AfterSelectFuncion="GetSelectedSupervisorGroupTreeV3" runat="server" />
                                                                                        </div>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </div>

                                                            <div id="exceptionsSelector" style="width: calc(50% ); margin-bottom: 13px; float: right">
                                                                <table width="98%">
                                                                    <tr>
                                                                        <td>
                                                                            <div class="panHeader2" style="background-color:#85a6ff !important">
                                                                                <span style="">
                                                                                    <asp:Label runat="server" ID="lblExceptionsHeader" Text="Excepciones"></asp:Label></span>
                                                                            </div>
                                                                            <br />
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>
                                                                            <table border="0" width="100%">
                                                                                <tr>
                                                                                    <td valign="top" align="left">
                                                                                        <span class="spanEmp-Class">
                                                                                            <asp:Label ID="lblExceptionsTitle" runat="server" Text="Selecciones los usurios a excluir."></asp:Label></span>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>
                                                                                        <!-- Este div es un formulario -->
                                                                                        <div style="margin-top:-40px">
                                                                                            <asp:Label ID="lblExceptionsDescription" runat="server" CssClass="jsGridTitle" Text=""></asp:Label>
                                                                                            <div class="jsgridButton" style="margin-right:0px">
                                                                                                <div class="btnFlat">
                                                                                                    <a href="javascript: void(0)" id="btnAddException" runat="server" onclick="editgridUsers()">
                                                                                                        <span class="btnIconAdd"></span>
                                                                                                        <asp:Label ID="lblAddException" runat="server" Text="Añadir"></asp:Label>
                                                                                                    </a>
                                                                                                </div>
                                                                                            </div>
                                                                                        </div>

                                                                                        <div id="divExceptionsSelector" runat="server" class="jsGridContent dextremeGrid" style="min-height:327px">
                                                                                            <!-- Carrega del Grid Usuari General -->
                                                                                        </div>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </div>
                                                        </div>


                                                        <div id="categoriesSelector" style="clear: both; width: 100%; padding-top: 20px">
                                                            <table width="99%">
                                                                <tr>
                                                                    <td>
                                                                        <div class="panHeader2">
                                                                            <span style="">
                                                                                <asp:Label runat="server" ID="lblRequestCategories" Text="Categorías de solicitud"></asp:Label></span>
                                                                        </div>
                                                                        <br />
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <table border="0" width="100%">
                                                                            <tr>
                                                                                <td valign="top" align="left">
                                                                                    <span class="spanEmp-Class">
                                                                                        <asp:Label ID="lblRequestCategoriesDesc" runat="server" Text="A continuación indique la configuración que aplica a las diferentes categorías de solicitud"></asp:Label></span>
                                                                                </td>
                                                                            </tr>

                                                                            <tr>
                                                                                <td>
                                                                                    <!-- Este div es un formulario -->
                                                                                    <div class="jsGrid">
                                                                                        <asp:Label ID="requestCategoryTitle" runat="server" CssClass="jsGridTitle" Text=""></asp:Label>
                                                                                        <div class="jsgridButton">
                                                                                            <div class="btnFlat">
                                                                                                <a href="javascript: void(0)" id="A1" runat="server" onclick="AddNewRequestCategory();">
                                                                                                    <span class="btnIconAdd"></span>
                                                                                                    <asp:Label ID="addNewRequestCategory" runat="server" Text="Añadir"></asp:Label>
                                                                                                </a>
                                                                                            </div>
                                                                                        </div>
                                                                                    </div>

                                                                                    <div id="divRequestCategory" runat="server" class="jsGridContent dextremeGrid">
                                                                                        <!-- Carrega del Grid Usuari General -->
                                                                                    </div>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </div>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                        <roFormsUE:frmNewUserException ID="frmNewUserException1" runat="server" />
                                        <roFormsRC:frmNewRequestCategory ID="frmNewRequestCategory1" runat="server" />
                                        <!-- PANEL Notificaciones activas -->
                                        <div id="divActiveNotifications" class="contentPanel" style="display: none;" runat="server">
                                            <table cellpadding="0" cellspacing="0" width="99%" border="0">
                                                <tr>
                                                    <td valign="top" style="padding-top: 2px;">
                                                        <div class="panHeader2">
                                                            <span style="">
                                                                <asp:Label ID="lblNotificationsTitle" runat="server" Text="Notificaciones"></asp:Label></span>
                                                        </div>
                                                        <br />
                                                        <div class="divEmployee-Class">
                                                            <table border="0" width="99%">
                                                                <tr>
                                                                    <td width="48px" height="48px">
                                                                        <img src="Images/Notifications_48.png" style="border: 0;" />
                                                                    </td>
                                                                    <td valign="top" align="left">
                                                                        <span class="spanEmp-Class">
                                                                            <asp:Label ID="lblActiveNotifications" runat="server" Text="En esta pantalla se muestran los diferentes tipos de notificaciones y si el grupo tiene permisos para gestionarlas basándose en la configuración sobre las funcionalidades."></asp:Label></span>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td colspan="2" style="padding: 10px 10px 10px 10px;">
                                                                        <div id="activeNotificationsTable" runat="server"></div>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </div>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>

                                        <!-- PANEL Aplicaciones permitidas -->
                                        <div id="divAllowedApplications" class="contentPanel" style="display: none;" runat="server">
                                            <table cellpadding="0" cellspacing="0" width="99%" height="100%" border="0">
                                                <tr>
                                                    <td valign="top" style="padding-top: 2px;">
                                                        <div class="panHeader2">
                                                            <span style="">
                                                                <asp:Label ID="lblAllowedApplications" runat="server" Text="Aplicaciones permitidas"></asp:Label></span>
                                                        </div>
                                                        <br />
                                                        <div class="divEmployee-Class">
                                                            <table border="0" width="750px">
                                                                <tr>
                                                                    <td width="48px" height="48px">
                                                                        <img src="Images/PassportIdentifyMethods_48.png" style="border: 0;" />
                                                                    </td>
                                                                    <td valign="top" align="left">
                                                                        <span class="spanEmp-Class">
                                                                            <asp:Label ID="Label2" runat="server" Text="Desde esta página puede gestionar los aplicaciones en que cada usuario puede iniciar sesión."></asp:Label></span>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td colspan="2">
                                                                        <div style="margin-top: 20px;" class="RoundCorner">
                                                                            <roUserControls:AllowedApplications ID="cnAllowedApplications" modowizardnew="ModeNormal" Type="Passport" runat="server" />
                                                                        </div>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </div>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>

                                        <!-- PANEL RESTRICCIONES -->
                                        <div id="divRestricciones" class="contentPanel" style="display: none;" runat="server">
                                            <div class="panHeader2">
                                                <span style="">
                                                    <asp:Label runat="server" ID="lblRestrictions" Text="Restricciones de acceso"></asp:Label></span>
                                            </div>
                                            <br />
                                            <table border="0" width="99%" style="height: 50px; table-layout: fixed;">
                                                <tr style="padding-top: 5px">
                                                    <td style="padding-left: 20px">
                                                        <asp:Label ID="lblGlobalAccesRestrictionDescription" Text="En este formulario se definen las restricciones de acceso globales a la aplicación y bloqueos totales en caso de fallos de seguridad." runat="server"></asp:Label>
                                                        <br />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="padding-left: 20px;">
                                                        <roUserControls:roGroupBox ID="groupAccessRestrictions" runat="server">
                                                            <Content>
                                                                <table>
                                                                    <tr>
                                                                        <td style="vertical-align: top; width: 60%; padding-right: 5px">
                                                                            <div>
                                                                                <roUserControls:roOptionPanelClient ID="ckRequiereKey" runat="server" TypeOPanel="CheckboxOption" width="100%" height="Auto" Checked="False" Enabled="True" CConClick="hasChanges(true);">
                                                                                    <Title>
                                                                                        <asp:Label ID="lblRequiereKey" runat="server" Text="En IPs nuevas solicitar confirmación mediante clave temportal"></asp:Label>
                                                                                    </Title>
                                                                                    <Description>
                                                                                        <asp:Label ID="lblRequiereKeyDesc" runat="server" Text="La clave temporal estará activa durante los 10 minutos siguientes al envío de la misma"></asp:Label>
                                                                                    </Description>
                                                                                    <Content>
                                                                                        <table id="tbRequiereKey" runat="server" cellpadding="0" cellspacing="5" style="height: 75px;">
                                                                                            <tr>
                                                                                                <td colspan="4">
                                                                                                    <asp:Label ID="lblSaveIp" Text="Guardar la IP autorizada para un usuario concreto:" runat="server"></asp:Label>
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td align="left" valign="middle" style="width: 10px;">&nbsp;</td>
                                                                                                <td>
                                                                                                    <asp:Label ID="lblElapsed" runat="server" Text="Duración"></asp:Label>
                                                                                                </td>
                                                                                                <td>
                                                                                                    <input type="text" runat="server" id="txtSaveKeyTime" style="text-align: center; text-align: center; width: 40px;"
                                                                                                        class="textEdit x-form-text x-form-field" convertcontrol="TextField"
                                                                                                        ccregex="/^([0-9]?[0-9])$/" ccmaxlength="2" cctime="false" cconchange="hasChanges(true);" />
                                                                                                </td>
                                                                                                <td>
                                                                                                    <asp:Label ID="lblDays" runat="server" Text="dias"></asp:Label>
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td align="left" valign="middle" style="width: 10px;">&nbsp;</td>
                                                                                                <td>
                                                                                                    <asp:Label ID="lblTimes" runat="server" Text="Hasta que haya accedido desde"></asp:Label>
                                                                                                </td>
                                                                                                <td>
                                                                                                    <input type="text" runat="server" id="txtAccessDiferentIps" style="text-align: center; text-align: center; width: 40px;"
                                                                                                        class="textEdit x-form-text x-form-field" convertcontrol="TextField"
                                                                                                        ccregex="/^([0-9]?[0-9])$/" ccmaxlength="2" cctime="false" cconchange="hasChanges(true);" />
                                                                                                </td>
                                                                                                <td>
                                                                                                    <asp:Label ID="lblDifferentIps" runat="server" Text="IPs distintas a esta"></asp:Label>
                                                                                                </td>
                                                                                            </tr>
                                                                                        </table>
                                                                                    </Content>
                                                                                </roUserControls:roOptionPanelClient>
                                                                            </div>
                                                                            <div style="padding-top: 5px">
                                                                                <roUserControls:roOptionPanelClient ID="chkVersion" runat="server" TypeOPanel="CheckboxOption" width="100%" height="Auto" Checked="False" Enabled="True" CConClick="hasChanges(true);">
                                                                                    <Title>
                                                                                        <asp:Label ID="lblAppCheckVersion" runat="server" Text="Únicamente si están usando la misma versión de la App que en servidor de VisualTime"></asp:Label>
                                                                                    </Title>
                                                                                    <Description>
                                                                                        <asp:Label ID="lblAppCheckVersionDesc" runat="server" Text=""></asp:Label>
                                                                                    </Description>
                                                                                    <Content>
                                                                                    </Content>
                                                                                </roUserControls:roOptionPanelClient>
                                                                            </div>
                                                                        </td>
                                                                        <td style="vertical-align: top; width: 40%">
                                                                            <roUserControls:roOptionPanelClient ID="ChkRestrictedIP" runat="server" TypeOPanel="CheckboxOption" width="100%" height="Auto" Checked="False" Enabled="True" CConClick="ChangeRestrictedIP();hasChanges(true);">
                                                                                <Title>
                                                                                    <asp:Label ID="lblOnlyIps" runat="server" Text="Listado de IPs"></asp:Label>
                                                                                </Title>
                                                                                <Description>
                                                                                    <asp:Label ID="lblOnlyIpsDesc" runat="server" Text="Solo las IPs incluidas en la lista podran acceder a VisualTime"></asp:Label>
                                                                                </Description>
                                                                                <Content>
                                                                                    <table width="100%">
                                                                                        <tr>
                                                                                            <td style="width: auto; text-align: right; padding-left: 20px; padding-right: 20px;">
                                                                                                <div id="tableAddIP" runat="server" class="btnFlat">
                                                                                                    <a href="javascript: void(0)" id="btnAddIPs" runat="server" onclick="EnableIpOpc();EditAllowedIP(true, null);">
                                                                                                        <span class="btnIconAdd"></span>
                                                                                                        <asp:Label ID="lblAddIP" runat="server" Text="Añadir IP"></asp:Label>
                                                                                                    </a>
                                                                                                </div>
                                                                                            </td>
                                                                                        </tr>
                                                                                        <tr>
                                                                                            <td>
                                                                                                <input type="hidden" runat="server" id="txtAllowedIPs" />
                                                                                                <div id="gridAllowedIPs" runat="server" style="height: 100px; overflow: auto;">
                                                                                                    <!-- grid de IPs -->
                                                                                                </div>

                                                                                                <!-- form Compositions -->
                                                                                                <roForms:frmIPs ID="frmIPs1" runat="server" />
                                                                                            </td>
                                                                                        </tr>
                                                                                    </table>
                                                                                </Content>
                                                                            </roUserControls:roOptionPanelClient>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </Content>
                                                        </roUserControls:roGroupBox>
                                                    </td>
                                                </tr>

                                                <tr>
                                                    <td>
                                                        <br />
                                                        <br />
                                                        <div class="panHeader2">
                                                            <span style="">
                                                                <asp:Label ID="lblLockCalendarDate" runat="server" Text="Fecha de bloqueo de calendario" Font-Bold="true" /></span>
                                                        </div>
                                                        <br />
                                                    </td>
                                                </tr>
                                                <tr style="padding-top: 5px">
                                                    <td style="padding-left: 20px">
                                                        <asp:Label ID="lblLockCalendarDesc" Text="Puede indicar si a los usuarios de este grupo les aplica la fecha de bloqueo de datos de calendario definida para la empresa de los empleados gestionados." runat="server"></asp:Label>
                                                        <br />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="padding-left: 20px;">
                                                        <roUserControls:roGroupBox ID="RoGroupBox1" runat="server">
                                                            <Content>
                                                                <table>
                                                                    <tr>
                                                                        <td>
                                                                            <input type="checkbox" id="ckCalendarLock" runat="server" onchange="hasChanges(true);" />&nbsp;<asp:Label ID="lblLockCalendarCK" runat="server" Text="No permitir modificar datos de calendario para fechas inferiores a la fecha de bloqueo definida para la empresa del empleado."></asp:Label>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </Content>
                                                        </roUserControls:roGroupBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>

                                        <!-- PANEL MÉTODOS DE IDENTIFICACIÓN -->
                                        <div id="divIdentifyMethods" class="contentPanel" style="display: none;" runat="server">
                                            <table cellpadding="0" cellspacing="0" width="99%" height="100%" border="0">
                                                <tr>
                                                    <td valign="top" style="padding-top: 2px;">
                                                        <div class="panHeader2">
                                                            <span style="">
                                                                <asp:Label ID="lblIdentifyMethodsTitle" runat="server" Text="Métodos de identificación"></asp:Label></span>
                                                        </div>
                                                        <br />
                                                        <div class="divEmployee-Class">
                                                            <table border="0" width="1100px">
                                                                <tr>
                                                                    <td width="48px" height="48px">
                                                                        <img src="Images/PassportIdentifyMethods_48.png" style="border: 0;" />
                                                                    </td>
                                                                    <td valign="top" align="left">
                                                                        <span class="spanEmp-Class">
                                                                            <asp:Label ID="lblIdentifyMethodsInfo" runat="server" Text="Desde esta página puede gestionar los métodos de identificación del ${Passport}."></asp:Label></span>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td colspan="2">
                                                                        <div style="border: margin-top: 20px;" class="RoundCorner">
                                                                            <roUserControls:IdentifyMethods ID="cnIdentifyMethods" ModoWizardNew="ModeNormal" Type="Passport" runat="server" />
                                                                        </div>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </div>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>

                                        <!-- PANEL CONSENTIMIENTOS -->
                                        <div id="divConsents" class="contentPanel" style="display: none" runat="server">
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
                                </dx:PanelContent>
                            </PanelCollection>
                        </dx:ASPxCallbackPanel>
                    </div>

                    <!-- POPUP NEW OBJECT -->
                    <dx:ASPxPopupControl ID="CaptchaObjectPopup" runat="server" AllowDragging="False" CloseAction="None" Modal="True" ContentUrl="~/Base/Popups/GenericCaptchaValidator.aspx"
                        PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter" ModalBackgroundStyle-Opacity="0" Width="500" Height="320"
                        ShowHeader="False" ScrollBars="Auto" ShowPageScrollbarWhenModal="True" ClientInstanceName="CaptchaObjectPopup_Client" PopupAnimationType="None" BackColor="Transparent" ContentStyle-Paddings-Padding="0px" Border-BorderColor="Transparent" ShowShadow="false">
                        <SettingsLoadingPanel Enabled="false" />
                    </dx:ASPxPopupControl>
                </div>
            </div>
        </div>

        <script language="javascript" type="text/javascript">

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
            }
        </script>
    </form>
</body>
</html>
