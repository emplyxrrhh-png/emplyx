<%@ Page Language="vb" MasterPageFile="~/Base/mastEmp.master" AutoEventWireup="false" Inherits="VTLive40.DocumentTemplate" Title="Documentos" CodeBehind="DocumentTemplate.aspx.vb" %>

<asp:Content ID="cMainBody" ContentPlaceHolderID="contentMainBody" runat="Server">

    <script language="javascript" type="text/javascript">

        function PageBase_Load() {
            resizeFrames();
        }
    </script>
    <input type="hidden" runat="server" id="IDLoadDocumentTemplate" value="-1" />
    <input type="hidden" id="hdnCaptionGrid" value="<%= Me.Language.Translate("CaptionGrid", Me.DefaultScope) %>" />
    <input type="hidden" id="msgHasChanges" value="" runat="server" clientidmode="Static" />
    <input type="hidden" id="msgHasErrors" value="" runat="server" clientidmode="Static" />

    <div id="divModalBgDisabled" class="modalBackground" style="position: absolute; left: 0px; top: 0px; z-index: 990; width: 1680px; height: 900px; display: none;"></div>
    <div id="divMainBody">
        <!-- TAB SUPERIOR -->
        <div id="divTabInfo" class="divDataCells">
            <div style="min-height: 10px">&nbsp;</div>
            <div id="divAccessGroup" class="blackRibbonTitle">
                <div class="blackRibbonIcon">
                    <img src="Images/DocumentTemplate.png" height="50px" alt="" />
                </div>
                <div class="blackRibbonDescription">
                    <span id="readOnlyNameDocumentTemplate" class="NameText"><%=Me.Language.Translate("CaptionGrid", Me.DefaultScope)%></span>
                </div>
                <div id="tbButtons" runat="server" class="blackRibbonButtons" style="padding-top: 25px">
                </div>
            </div>
            <div style="min-height: 10px">&nbsp;</div>
        </div>
        <!-- FIN TAB SUPERIOR -->

        <!-- ARBOL Y DETALLE -->
        <div id="divTabData" class="divDataCells">
            <div id="divContenido" class="divAllContent">
                <div id="divContent" style="height: initial" class="maxHeight">

                    <div id="divGridDocumentTemplates" runat="server" style="text-align: left; vertical-align: top; padding: 0px; height: 95%; display: block;">
                        <div style="width: 100%; height: 100%; padding: 0px;">
                            <div class="RoundCornerFrame roundCorner">
                                <dx:ASPxHiddenField ID="hdnFilterStatus" runat="server" ClientInstanceName="hdnFilterStatusClient" />
                                <dx:ASPxGridView ID="GridDocumentTemplate" runat="server" AutoGenerateColumns="False" Width="100%" Cursor="pointer" DataSourceID="LinqServerModeDataSource1" ClientInstanceName="GridDocumentTemplate">
                                    <ClientSideEvents CustomButtonClick="GridDocumentTemplatesClientCustomButton_Click" />
                                    <Templates>
                                        <TitlePanel>
                                            <!-- Botón de exportación a excel -->
                                            <div class="RoundCornerFrame" style="float: right; padding: 2px; border: 1px solid #595959;">
                                                <dx:ASPxButton ID="ASPxbtnExportGrid" runat="server" AutoPostBack="true" CausesValidation="False"
                                                    Image-Url="Images/ExportToExcel.png" OnClick="ASPxbtnExportGrid_Click" Width="39px" Height="30px">
                                                    <Image Height="19px" Width="19px" />
                                                    <Paddings PaddingTop="3px" />
                                                    <FocusRectBorder BorderStyle="Groove" BorderWidth="1px" BorderColor="ActiveBorder" />
                                                </dx:ASPxButton>
                                            </div>
                                        </TitlePanel>
                                    </Templates>

                                    <SettingsLoadingPanel Text="Cargando&amp;hellip;"></SettingsLoadingPanel>

                                    <SettingsFilterControl ViewMode="Visual" ShowAllDataSourceColumns="True" MaxHierarchyDepth="1" />
                                    <Settings ShowFilterBar="Visible" />
                                    <Settings ShowTitlePanel="True" />
                                </dx:ASPxGridView>
                                <dx:LinqServerModeDataSource ID="LinqServerModeDataSource1" runat="server" />
                            </div>
                        </div>
                    </div>

                    <div id="documentRow" style="display: none">
                        <div id="divMenuTask" runat="server" style="width: 90%; margin-top: 10px; margin-left: auto; margin-right: auto; padding: 4px;">
                            <table border="0" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td>
                                        <div id="tdBackToGrid" style="padding: 2px; margin-right: 5px;"
                                            class="RoundCornerFrame">
                                            <dx:ASPxButton ID="ASPxButton2" runat="server" AutoPostBack="False" CausesValidation="False"
                                                Image-Url="Images/BackToGridSmall.png">
                                                <ClientSideEvents Click="function(s, e) { BackToGrid(); }" />
                                                <FocusRectBorder BorderStyle="Groove" BorderWidth="1px" BorderColor="ActiveBorder" />
                                            </dx:ASPxButton>
                                        </div>
                                    </td>
                                    <td>
                                        <div class="RoundCornerFrame">
                                            <table>
                                                <tr>
                                                    <td>
                                                        <a id="TABBUTTON_00" href="javascript: void(0);" class="bTab-active" onclick="javascript: changeTabs(0);">
                                                            <%=Me.Language.Translate("tabEstado", Me.DefaultScope)%></a>
                                                    </td>
                                                    <td>
                                                        <a id="TABBUTTON_02" href="javascript: void(0);" class="bTab" onclick="javascript: changeTabs(2);">
                                                            <%=Me.Language.Translate("tabScope", Me.DefaultScope)%></a>
                                                    </td>
                                                    <td>
                                                        <a id="TABBUTTON_01" href="javascript: void(0);" class="bTab" onclick="javascript: changeTabs(1);">
                                                            <%=Me.Language.Translate("tabControl", Me.DefaultScope)%></a>
                                                    </td>
                                                    <td>
                                                        <a id="TABBUTTON_03" href="javascript: void(0);" class="bTab" onclick="javascript: changeTabs(3);">
                                                            <%=Me.Language.Translate("tabApprove", Me.DefaultScope)%></a>
                                                    </td>

                                                    <td>
                                                        <a id="TABBUTTON_04" href="javascript: void(0);" class="bTab" onclick="javascript: changeTabs(4);">
                                                            <%=Me.Language.Translate("tabNotifications", Me.DefaultScope)%></a>
                                                    </td>

                                                    <td>
                                                        <a id="TABBUTTON_05" href="javascript: void(0);" class="bTab" onclick="javascript: changeTabs(5);">
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
                                <dx:PanelContent ID="PanelContent1" runat="server">
                                    <div id="divMsgTop" class="divMsg2 divMessageTop" style="display: none; width: calc(100% - 2px) !important">
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

                                    <div id="divContentPanels" class="divContentPanelsWithOutMessage" style="min-height: 600px">
                                        <!-- PANEL GENERAL -->
                                        <div id="panDocGeneral" class="contentPanel" runat="server">
                                            <!-- Este div es el header General -->
                                            <div class="panBottomMargin">
                                                <div class="panHeader2 panBottomMargin">
                                                    <span class="panelTitleSpan">
                                                        <asp:Label runat="server" ID="lblDocumentGeneral" Text="General"></asp:Label>
                                                    </span>
                                                </div>
                                                <br />
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
                                                    <asp:Label ID="lblDocName" runat="server" Text="Nombre:" CssClass="labelForm"></asp:Label>
                                                    <div class="componentForm">
                                                        <dx:ASPxTextBox ID="txtDocName" MaxLength="50" runat="server" ClientInstanceName="txtDocName_Client" NullText="_____">
                                                            <ClientSideEvents Validation="LengthValidation" TextChanged="function(s,e){checkDocumentEmptyName(s.GetValue());}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                            <ValidationSettings SetFocusOnError="True" ValidationGroup="Document">
                                                                <RequiredField IsRequired="True" ErrorText="(*)" />
                                                            </ValidationSettings>
                                                        </dx:ASPxTextBox>
                                                    </div>
                                                </div>
                                                <div class="divRow">
                                                    <!-- NombreCorto-->
                                                    <div class="divRowDescription">
                                                        <asp:Label ID="lblDocShortNameDescription" runat="server" Text="Nombre corto de la plantilla."></asp:Label>
                                                    </div>
                                                    <asp:Label ID="lblDocShortName" runat="server" Text="Nombre abreviado:" CssClass="labelForm"></asp:Label>
                                                    <div class="componentForm">
                                                        <dx:ASPxTextBox ID="txtDocShortName" runat="server" ClientInstanceName="txtDocShortName_Client" MaxLength="5" Width="30" NullText="___">
                                                            <ClientSideEvents Validation="LengthValidation" TextChanged="function(s,e){ hasChanges(true)}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                            <ValidationSettings SetFocusOnError="True" ValidationGroup="Document">
                                                                <RequiredField IsRequired="True" ErrorText="(*)" />
                                                            </ValidationSettings>
                                                        </dx:ASPxTextBox>
                                                    </div>
                                                </div>
                                                <div class="divRow">
                                                    <!-- Descripción-->
                                                    <div class="divRowDescription">
                                                        <asp:Label ID="lblDocDescriptionDesc" runat="server" Text="Descripción de la plantilla."></asp:Label>
                                                    </div>
                                                    <asp:Label ID="lblDocDescription" runat="server" Text="Descripción:" CssClass="labelForm"></asp:Label>
                                                    <div class="componentForm">
                                                        <dx:ASPxMemo ID="txtDocDescription" runat="server" ClientInstanceName="txtDocDescription_Client" Rows="2" Width="100%" Height="40">
                                                            <ClientSideEvents TextChanged="function(s,e){ hasChanges(true)}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                        </dx:ASPxMemo>
                                                    </div>
                                                </div>
                                                <div class="divRow">
                                                    <div class="divRowDescription">
                                                        <asp:Label ID="lblAreaDescription" runat="server" Text="El documento aplica a la siguiente área. El área de un documento fija quien supervisará los documentos de este tipo."></asp:Label>
                                                    </div>
                                                    <asp:Label ID="lblArea" runat="server" Text="Area:" CssClass="labelForm"></asp:Label>
                                                    <div class="componentForm">
                                                        <roUserControls:roGroupBox ID="RoGroupBox2" runat="server">
                                                            <Content>
                                                                <dx:ASPxRadioButtonList ID="rblDocumentArea" runat="server" Border-BorderStyle="None" ValueField="ID" TextField="Name" RepeatColumns="8" RepeatLayout="Table" ClientInstanceName="rblDocumentArea_Client">
                                                                    <ClientSideEvents SelectedIndexChanged="function(s,e){ hasChanges(true)}" />
                                                                    <ValidationSettings SetFocusOnError="True" ValidationGroup="Document">
                                                                        <RequiredField IsRequired="True" ErrorText="(*)" />
                                                                    </ValidationSettings>
                                                                </dx:ASPxRadioButtonList>
                                                            </Content>
                                                        </roUserControls:roGroupBox>
                                                    </div>
                                                </div>
                                                <div class="divRow">
                                                    <div class="divRowDescription">
                                                        <asp:Label ID="lblValidityDes" runat="server" Text="Indique el periodo en el que el documento será exigible. Si aplica desde siempre, deje la fecha de inicio de validez en blanco. Si aplica para siempre, deje la fecha de fin de validez en blanco."></asp:Label>
                                                    </div>
                                                    <asp:Label ID="lblDocValidity" runat="server" Text="Validez:" CssClass="labelForm"></asp:Label>
                                                    <div class="componentForm">
                                                        <roUserControls:roGroupBox ID="RoGroupBox4" runat="server">
                                                            <Content>
                                                                <table border="0">
                                                                    <tr>
                                                                        <td valign="top">
                                                                            <asp:Label ID="lblValidPeriod" runat="server" Text="Desde" Style="padding-right: 10px;"></asp:Label>
                                                                        </td>
                                                                        <td valign="top" style="width: 90px;">
                                                                            <dx:ASPxDateEdit ID="dpPeriodStart" runat="server" Width="105" ClientInstanceName="dpPeriodStart_Client">
                                                                                <ClientSideEvents DateChanged="function(s,e){hasChanges(true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                            </dx:ASPxDateEdit>
                                                                        </td>
                                                                        <td valign="top">
                                                                            <asp:Label ID="lblPeriodTo" runat="server" Text="Hasta" Style="padding-right: 10px; padding-left: 10px;"></asp:Label>
                                                                        </td>
                                                                        <td valign="top" style="width: 90px;">
                                                                            <dx:ASPxDateEdit ID="dpPeriodEnd" runat="server" Width="105" ClientInstanceName="dpPeriodEnd_Client">
                                                                                <ClientSideEvents DateChanged="function(s,e){hasChanges(true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                            </dx:ASPxDateEdit>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </Content>
                                                        </roUserControls:roGroupBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div id="panDocControl" class="contentPanel" runat="server" style="display: none">
                                            <!-- Este div es el header Criticidad-->
                                            <div class="panBottomMargin">
                                                <div class="panHeader2 panBottomMargin">
                                                    <span class="panelTitleSpan">
                                                        <asp:Label runat="server" ID="lblCriticidad" Text="Criticidad"></asp:Label>
                                                    </span>
                                                </div>
                                            </div>
                                            <!-- Este div es un formulario -->
                                            <div class="panBottomMargin">
                                                <div class="divRow">
                                                    <div class="divRowDescription">
                                                        <asp:Label ID="lblMandatoryDesc" runat="server" Text="Indique si el documento es obligatorio."></asp:Label>
                                                    </div>
                                                    <asp:Label ID="lblMandatory" runat="server" Text="Obligatorio:" CssClass="labelForm"></asp:Label>
                                                    <div class="componentForm">
                                                        <roUserControls:roOptionPanelClient ID="optMandatoryDocument" runat="server" TypeOPanel="CheckboxOption"
                                                            width="100%" height="Auto" Checked="False" Enabled="True" Border="True" Value="0" CConClick="hasChanges(true)">
                                                            <Title>
                                                                <asp:Label ID="Label3" runat="server" Text="El documento es obligatorio"></asp:Label>
                                                            </Title>
                                                            <Description>
                                                            </Description>
                                                            <Content>
                                                            </Content>
                                                        </roUserControls:roOptionPanelClient>
                                                    </div>
                                                </div>

                                                <div class="divRow">
                                                    <div class="divRowDescription">
                                                        <asp:Label ID="lblCriticalityDes" runat="server" Text="Indique la criticidad de la no conformidad de este documento."></asp:Label>
                                                    </div>
                                                    <asp:Label ID="lblCriticality" runat="server" Text="Criticidad:" CssClass="labelForm"></asp:Label>
                                                    <div class="componentForm">
                                                        <roUserControls:roGroupBox ID="RoGroupBox3" runat="server">
                                                            <Content>
                                                                <dx:ASPxRadioButton ID="rbnNonCriticality" ClientInstanceName="rbnNonCriticality_Client" runat="server" Text="No critico" ToolTip="No se tomará ninguna acción ante la no conformidad del documento" GroupName="Criticality">
                                                                    <ClientSideEvents CheckedChanged="function(s,e){hasChanges(true);}" />
                                                                </dx:ASPxRadioButton>
                                                                <dx:ASPxRadioButton ID="rbnAdviceCriticality" ClientInstanceName="rbnAdviceCriticality_Client" runat="server" Text="Avisar" ToolTip="Se avisará de la no conformidad. Si existe un control de acceso, se permitirá el acceso" GroupName="Criticality">
                                                                    <ClientSideEvents CheckedChanged="function(s,e){hasChanges(true);}" />
                                                                </dx:ASPxRadioButton>
                                                                <dx:ASPxRadioButton ID="rbnDeniedCriticality" ClientInstanceName="rbnDeniedCriticality_Client" runat="server" Text="Denegar acceso" ToolTip="si existe un control de acceso, no se permitirá el acceso" GroupName="Criticality">
                                                                    <ClientSideEvents CheckedChanged="function(s,e){hasChanges(true);}" />
                                                                </dx:ASPxRadioButton>
                                                            </Content>
                                                        </roUserControls:roGroupBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div id="panDocScope" class="contentPanel" runat="server" style="display: none">
                                            <!-- Este div es el header Alcance-->
                                            <div class="panBottomMargin">
                                                <div class="panHeader2 panBottomMargin">
                                                    <span class="panelTitleSpan">
                                                        <asp:Label runat="server" ID="lblDocumentScope" Text="Alcance"></asp:Label>
                                                    </span>
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
                                                        <roUserControls:roGroupBox ID="RoGroupBox1" runat="server">
                                                            <Content>
                                                                <table>
                                                                    <tr>
                                                                        <td>
                                                                            <dx:ASPxRadioButtonList ID="rblScope" runat="server" Border-BorderStyle="None"
                                                                                RepeatColumns="1" RepeatLayout="Table" RepeatDirection="Vertical" ItemSpacing="10px" ClientInstanceName="rblScope_Client">
                                                                                <ClientSideEvents SelectedIndexChanged="rblScope_SelectedIndexChanged" />
                                                                                <ValidationSettings SetFocusOnError="True" ValidationGroup="Document">
                                                                                    <RequiredField IsRequired="True" ErrorText="(*)" />
                                                                                </ValidationSettings>
                                                                            </dx:ASPxRadioButtonList>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </Content>
                                                        </roUserControls:roGroupBox>
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
                                                        <roUserControls:roGroupBox ID="RoGroupBox6" runat="server">
                                                            <Content>
                                                                <table>
                                                                    <tr>
                                                                        <td>
                                                                            <dx:ASPxCheckBox ID="ckCanAddDocumentEmployee" runat="server" ClientInstanceName="ckCanAddDocumentEmployeeClient" Text="Lo pueden presentar electrónicamente los empleados">
                                                                                <ClientSideEvents CheckedChanged="function(s,e){ hasChanges(true)}" />
                                                                            </dx:ASPxCheckBox>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>
                                                                            <dx:ASPxCheckBox ID="ckCanAddDocumentSupervisor" runat="server" ClientInstanceName="ckCanAddDocumentSupervisorClient" Text="Lo pueden presentar electrónicamente los supervisores">
                                                                                <ClientSideEvents CheckedChanged="function(s,e){ hasChanges(true)}" />
                                                                            </dx:ASPxCheckBox>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>
                                                                            <dx:ASPxCheckBox ID="ckSystemDocument" runat="server" ClientInstanceName="ckSystemDocumentClient" Text="Es un documento de sistema" Enabled="false">
                                                                                <ClientSideEvents CheckedChanged="function(s,e){ hasChanges(true)}" />
                                                                            </dx:ASPxCheckBox>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </Content>
                                                        </roUserControls:roGroupBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div id="panDocApprove" class="contentPanel" runat="server" style="display: none">
                                            <!-- Este div es el header Alcance-->
                                            <div class="panBottomMargin">
                                                <div class="panHeader2 panBottomMargin">
                                                    <span class="panelTitleSpan">
                                                        <asp:Label runat="server" ID="lblApproveTitle" Text="Aprobación"></asp:Label>
                                                    </span>
                                                </div>
                                            </div>
                                            <!-- Este div es un formulario -->
                                            <div id="aprobaciones" class="panBottomMargin">
                                                <div class="divRow">
                                                    <!-- Descripción-->
                                                    <div class="divRowDescription">
                                                        <asp:Label ID="lblApproveDesc" runat="server" Text="Al recibir un documento:"></asp:Label>
                                                    </div>
                                                    <asp:Label ID="lblApprove" runat="server" Text="Requiere aprobación:" CssClass="labelForm"></asp:Label>
                                                    <div class="componentForm">

                                                        <table cellpadding="0" cellspacing="0" border="0" style="width: 100%; height: 150px; padding-right: 10px;">
                                                            <tr>
                                                                <td style="" valign="top">
                                                                    <roUserControls:roOptionPanelClient ID="optNoApprove" runat="server" TypeOPanel="RadioOption"
                                                                        width="100%" height="Auto" Checked="False" Enabled="True" Border="True" Value="0" CConClick="hasChanges(true)">
                                                                        <Title>
                                                                            <asp:Label ID="lbloptNoApproveTitle" runat="server" Text="No requiere aprobación"></asp:Label>
                                                                        </Title>
                                                                        <Description>
                                                                            <asp:Label ID="lbloptNoApproveDesc" runat="server" Text="El documento se guardará y validará automáticamente una vez recibido."></asp:Label>
                                                                        </Description>
                                                                        <Content>
                                                                        </Content>
                                                                    </roUserControls:roOptionPanelClient>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td style="" valign="top">
                                                                    <roUserControls:roOptionPanelClient ID="optApproveRequiered" runat="server" TypeOPanel="RadioOption"
                                                                        width="100%" height="Auto" Checked="False" Enabled="True" Border="True" Value="1" CConClick="hasChanges(true)">
                                                                        <Title>
                                                                            <asp:Label ID="lbloptApproveRequieredTitle" runat="server" Text="Requiere aprobación por parte de un supervisor"></asp:Label>
                                                                        </Title>
                                                                        <Description>
                                                                        </Description>
                                                                        <Content>
                                                                            <table style="padding-left: 10px;">
                                                                                <tr>
                                                                                    <td style="padding-right: 3px;" align="left">
                                                                                        <asp:Label ID="lblSupervisorLevelRequiered" CssClass="OptionPanelDescStyle" runat="server" Text="Nivel de supervisión requerido:"></asp:Label>
                                                                                    </td>
                                                                                    <td align="left" width="70px">
                                                                                        <dx:ASPxTextBox runat="server" ID="txtRequieredSupervisorLevel" MaxLength="2" Width="50px" ClientInstanceName="txtRequieredSupervisorLevelClient">
                                                                                            <MaskSettings Mask="<1..10>" />
                                                                                            <ClientSideEvents TextChanged="function(s,e){ hasChanges(true)}" />
                                                                                            <ValidationSettings ErrorDisplayMode="None" />
                                                                                        </dx:ASPxTextBox>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </Content>
                                                                    </roUserControls:roOptionPanelClient>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </div>
                                                </div>
                                            </div>

                                            <!-- Este div es el header Alcance-->
                                            <div class="panBottomMargin">
                                                <div class="panHeader2 panBottomMargin">
                                                    <span class="panelTitleSpan">
                                                        <asp:Label runat="server" ID="lblValidPeriodApprove" Text="Validez"></asp:Label>
                                                    </span>
                                                </div>
                                            </div>

                                            <div id="validity">
                                                <div class="divRow">
                                                    <!-- Descripción-->
                                                    <div class="divRowDescription">
                                                        <asp:Label ID="lblValidUltilDesc" runat="server" Text="Al aprobarse un documento de este tipo, ese será válido durante el siguiente periodo:"></asp:Label>
                                                    </div>
                                                    <asp:Label ID="lblValidUltil" runat="server" Text="Validez:" CssClass="labelForm"></asp:Label>
                                                    <div class="componentForm">
                                                        <table cellpadding="0" cellspacing="0" border="0" style="width: 100%; height: 150px; padding-right: 10px;">
                                                            <tr>
                                                                <td style="" valign="top">
                                                                    <roUserControls:roOptionPanelClient ID="optAllwaysValid" runat="server" TypeOPanel="RadioOption"
                                                                        width="100%" height="Auto" Checked="False" Enabled="True" Border="True" Value="0" CConClick="hasChanges(true)">
                                                                        <Title>
                                                                            <asp:Label ID="lblAllwaysValidTitle" runat="server" Text="Siempre"></asp:Label>
                                                                        </Title>
                                                                        <Description>
                                                                            <asp:Label ID="lblAllwaysValidDesc" runat="server" Text="El documento no expirará nunca."></asp:Label>
                                                                        </Description>
                                                                        <Content>
                                                                        </Content>
                                                                    </roUserControls:roOptionPanelClient>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td style="" valign="top">
                                                                    <roUserControls:roOptionPanelClient ID="optValidUntil" runat="server" TypeOPanel="RadioOption"
                                                                        width="100%" height="Auto" Checked="False" Enabled="True" Border="True" Value="1" CConClick="hasChanges(true)">
                                                                        <Title>
                                                                            <asp:Label ID="roOptValidUntilTitle" runat="server" Text="Durante un periodo de tiempo"></asp:Label>
                                                                        </Title>
                                                                        <Description>
                                                                        </Description>
                                                                        <Content>
                                                                            <table>
                                                                                <tr>
                                                                                    <td style="padding-right: 3px;" align="left">
                                                                                        <asp:Label ID="Label6" CssClass="OptionPanelDescStyle" runat="server" Text="Durante:"></asp:Label>
                                                                                    </td>
                                                                                    <td align="left" width="70px">
                                                                                        <dx:ASPxTextBox runat="server" ID="txtExpireDays" MaxLength="5" Width="50px" ClientInstanceName="txtRequieredSupervisorLevelClient">
                                                                                            <MaskSettings Mask="<0..99999>" />
                                                                                            <ClientSideEvents TextChanged="function(s,e){ hasChanges(true)}" />
                                                                                            <ValidationSettings ErrorDisplayMode="None" />
                                                                                        </dx:ASPxTextBox>
                                                                                    </td>
                                                                                    <td style="padding-right: 3px;" align="left">
                                                                                        <asp:Label ID="lblDaysAfter" CssClass="OptionPanelDescStyle" runat="server" Text="días a partir de la fecha de aprobación."></asp:Label>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </Content>
                                                                    </roUserControls:roOptionPanelClient>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </div>
                                                </div>

                                                <div class="divRow">
                                                    <!-- Descripción-->
                                                    <div class="divRowDescription">
                                                        <asp:Label ID="lblExpireDocumentsTitle" runat="server" Text="Caducidad de documentos."></asp:Label>
                                                    </div>
                                                    <asp:Label ID="lblExpireDocuments" runat="server" Text="Caducidad" CssClass="labelForm"></asp:Label>
                                                    <div class="componentForm">
                                                        <roUserControls:roOptionPanelClient ID="optExpireOld" runat="server" TypeOPanel="CheckboxOption"
                                                            width="100%" height="Auto" Checked="False" Enabled="True" Border="True" Value="0" CConClick="hasChanges(true)">
                                                            <Title>
                                                                <asp:Label ID="lblExpireOldDocuments" runat="server" Text="La aprobación provoca que el resto de documentos de este tipo se marquen como caducados"></asp:Label>
                                                            </Title>
                                                            <Description>
                                                            </Description>
                                                            <Content>
                                                            </Content>
                                                        </roUserControls:roOptionPanelClient>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div id="panDocNotifications" class="contentPanel" runat="server" style="display: none">

                                            <!-- Este div es el header Notificaciones-->
                                            <div class="panBottomMargin">
                                                <div class="panHeader2 panBottomMargin">
                                                    <span class="panelTitleSpan">
                                                        <asp:Label runat="server" ID="lblNotificationsEnabled" Text="Notificaciones"></asp:Label>
                                                    </span>
                                                </div>
                                            </div>
                                            <!-- Este div es un formulario -->
                                            <div class="panBottomMargin">
                                                <div class="divRow">
                                                    <div class="divRowDescription">
                                                        <asp:Label ID="lblNotificationsEnabledOnDocument" runat="server" Text="Indique las notificaciones que se generarán."></asp:Label>
                                                    </div>
                                                    <asp:Label ID="lblDocumentNotifications" runat="server" Text="Notificaciones" CssClass="labelForm"></asp:Label>
                                                    <div class="componentForm">
                                                        <roUserControls:roGroupBox ID="RoGroupBox5" runat="server">
                                                            <Content>
                                                                <dx:ASPxCheckBoxList ID="rbNotifications" runat="server" Border-BorderStyle="None" RepeatColumns="1" RepeatLayout="Table" ClientInstanceName="rbNotifications_Client">
                                                                    <ClientSideEvents SelectedIndexChanged="function(s,e){ hasChanges(true)}" />
                                                                </dx:ASPxCheckBoxList>
                                                            </Content>
                                                        </roUserControls:roGroupBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div id="panDocLOPD" class="contentPanel" runat="server" style="display: none">
                                            <!-- Este div es el header Notificaciones-->
                                            <div class="panBottomMargin">
                                                <div class="panHeader2 panBottomMargin">
                                                    <span class="panelTitleSpan">
                                                        <asp:Label runat="server" ID="lblLopdTitle" Text="RGPD"></asp:Label>
                                                    </span>
                                                </div>
                                            </div>
                                            <!-- Este div es un formulario -->
                                            <div class="panBottomMargin">
                                                <div class="divRow">
                                                    <div class="divRowDescription">
                                                        <asp:Label ID="lblLopdLevelDescription" runat="server" Text="Indique el nivel de la información que contiene el documento."></asp:Label>
                                                    </div>
                                                    <asp:Label ID="lblLopdLevel" runat="server" Text="Nivel" CssClass="labelForm"></asp:Label>
                                                    <div class="componentForm">
                                                        <dx:ASPxComboBox ID="cmbLopdLevel" runat="server" ValueField="ID" TextField="Name" repeatcolumns="1" repeatlayout="Table" ClientInstanceName="cmbLopdLevel_Client">
                                                            <ClientSideEvents SelectedIndexChanged="function(s,e){ hasChanges(true)}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                        </dx:ASPxComboBox>
                                                    </div>
                                                </div>
                                            </div>
                                            <!-- Este div es un formulario -->
                                            <div class="panBottomMargin">
                                                <div class="divRow">
                                                    <div class="divRowDescription">
                                                        <asp:Label ID="lblDocumentExpiresDescription" runat="server" Text="Indique el número de dias pasados los cuales se eliminará el documento."></asp:Label>
                                                    </div>
                                                    <asp:Label ID="lblDocumentExpires" runat="server" Text="Dias" CssClass="labelForm"></asp:Label>
                                                    <div class="componentForm">

                                                        <table cellpadding="0" cellspacing="0" border="0" style="width: 100%; height: 150px; padding-right: 10px;">
                                                            <tr>
                                                                <td style="" valign="top">
                                                                    <roUserControls:roOptionPanelClient ID="roOptPanelExpireOnServer" runat="server" TypeOPanel="RadioOption"
                                                                        width="100%" height="Auto" Checked="False" Enabled="True" Border="True" Value="0" CConClick="hasChanges(true)">
                                                                        <Title>
                                                                            <asp:Label ID="lblExpireOnServer" runat="server" Text="Valor predeterminado"></asp:Label>
                                                                        </Title>
                                                                        <Description>
                                                                            <asp:Label ID="lblExpireOnServerDescription" runat="server" Text="El documento se borrará pasados los días configurados por defecto en el sistema."></asp:Label>
                                                                        </Description>
                                                                        <Content>
                                                                        </Content>
                                                                    </roUserControls:roOptionPanelClient>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td style="" valign="top">
                                                                    <roUserControls:roOptionPanelClient ID="roOptPanelnoExpire" runat="server" TypeOPanel="RadioOption"
                                                                        width="100%" height="Auto" Checked="False" Enabled="True" Border="True" Value="1" CConClick="hasChanges(true)">
                                                                        <Title>
                                                                            <asp:Label ID="lblDocNoDelete" runat="server" Text="No borrar"></asp:Label>
                                                                        </Title>
                                                                        <Description>
                                                                            <asp:Label ID="lblDocNoDeleteDesc" runat="server" Text="El documento no se borrará nunca."></asp:Label>
                                                                        </Description>
                                                                        <Content>
                                                                        </Content>
                                                                    </roUserControls:roOptionPanelClient>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td style="" valign="top">
                                                                    <roUserControls:roOptionPanelClient ID="roOptPanelExpireOnDate" runat="server" TypeOPanel="RadioOption"
                                                                        width="100%" height="Auto" Checked="False" Enabled="True" Border="True" Value="1" CConClick="hasChanges(true)">
                                                                        <Title>
                                                                            <asp:Label ID="lblOxpireOnDate" runat="server" Text="Pasados unos días"></asp:Label>
                                                                        </Title>
                                                                        <Description>
                                                                        </Description>
                                                                        <Content>
                                                                            <table>
                                                                                <tr>
                                                                                    <td style="padding-right: 3px;" align="left">
                                                                                        <asp:Label ID="lblOxpireOnDateDesc" CssClass="OptionPanelDescStyle" runat="server" Text="El documento se borrará pasados "></asp:Label>
                                                                                    </td>
                                                                                    <td align="left" width="70px">
                                                                                        <dx:ASPxTextBox runat="server" ID="txtExpireLOPDDays" MaxLength="5" Width="50px" ClientInstanceName="txtExpireLOPDDays_Client">
                                                                                            <MaskSettings Mask="<0..99999>" />
                                                                                            <ClientSideEvents TextChanged="function(s,e){ hasChanges(true)}" />
                                                                                            <ValidationSettings ErrorDisplayMode="None" />
                                                                                        </dx:ASPxTextBox>
                                                                                    </td>
                                                                                    <td style="padding-right: 3px;" align="left">
                                                                                        <asp:Label ID="Label10" CssClass="OptionPanelDescStyle" runat="server" Text="días a partir de la fecha de aprobación."></asp:Label>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </Content>
                                                                    </roUserControls:roOptionPanelClient>
                                                                </td>
                                                            </tr>
                                                        </table>
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
    <!-- POPUP NEW OBJECT -->
    <dx:ASPxPopupControl ID="NewObjectPopup" runat="server" AllowDragging="False" CloseAction="None" Modal="True" ContentUrl="~/Base/Popups/CreateObjectPopup.aspx"
        PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter" Width="470px" Height="300px"
        ShowHeader="False" ScrollBars="Auto" ShowPageScrollbarWhenModal="True" ClientInstanceName="NewObjectPopup_Client" PopupAnimationType="None" BackColor="Transparent" ContentStyle-Paddings-Padding="0px" Border-BorderColor="Transparent" ShowShadow="false">
        <SettingsLoadingPanel Enabled="false" />
    </dx:ASPxPopupControl>

    <dx:ASPxGridViewExporter ID="ASPxGridViewExporter1" runat="server">
    </dx:ASPxGridViewExporter>
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

            $("#divTabData").height(divHeight - 20);
        }

        window.onresize = function () {
            resizeFrames();
        }

        <%If Not String.IsNullOrWhiteSpace(Me.IDLoadDocumentTemplate.Value) AndAlso Not Me.IDLoadDocumentTemplate.Value.Equals("-1") Then %>
        $(document).ready(function () {
            AutoEditDocumentTemplate('<%= Me.IDLoadDocumentTemplate.Value %>');
        });
        <% End If %>
    </script>
</asp:Content>