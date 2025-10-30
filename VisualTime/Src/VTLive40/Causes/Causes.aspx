<%@ Page Language="VB" MasterPageFile="~/Base/mastEmp.master" AutoEventWireup="false" Inherits="VTLive40.Causes" Title="${Causes}" CodeBehind="Causes.aspx.vb" %>

<%@ Register Src="~/Causes/WebUserForms/frmDocumentTrace.ascx" TagName="frmDocumentTrace" TagPrefix="roForms" %>

<asp:Content ID="cMainBody" ContentPlaceHolderID="contentMainBody" runat="Server">

    <script language="javascript" type="text/javascript">

        /* funcio per bloquejar sols l'area menu */
        function disableScreen(bol) {
            var divBg = document.getElementById('divModalBgDisabled');
            if (divBg != null) {
                if (bol == true) {
                    document.body.style.overflow = "hidden";
                    divBg.style.height = 2000;  //document.body.offsetHeight;
                    divBg.style.width = 3000;  //document.body.offsetWidth;

                    divBg.style.display = '';
                }
                else {
                    document.body.style.overflow = "";
                    divBg.style.display = 'none';
                }
            }
        }

        function PageBase_Load() {
            resizeFrames();
            resizeTreeCauses();
        }
    </script>

    <input type="hidden" runat="server" id="hdnModeEdit" value="" />
    <input type="hidden" runat="server" id="noRegs" value="" />
    <div id="divModalBgDisabled" class="modalBackground" style="position: absolute; left: 0px; top: 0px; z-index: 990; width: 1680px; height: 900px; display: none;"></div>
    <input type="hidden" id="dateFormatValue" runat="server" value="" />
    <input type="hidden" id="msgHasChanges" value="" runat="server" clientidmode="Static" />
    <input type="hidden" id="msgHasErrors" value="" runat="server" clientidmode="Static" />

    <input type="hidden" id="hdnLabAgreeTitle" value="" runat="server" clientidmode="Static" />
    <input type="hidden" id="hdnDocumentTitle" value="" runat="server" clientidmode="Static" />
    <input type="hidden" id="hdnIntervalTitle" value="" runat="server" clientidmode="Static" />
    <input type="hidden" id="hdnAbsencesAtBegin" value="" runat="server" clientidmode="Static" />
    <input type="hidden" id="hdnAbsencesAtEnd" value="" runat="server" clientidmode="Static" />
    <input type="hidden" id="hdnAbsencesEveryDays" value="" runat="server" clientidmode="Static" />
    <input type="hidden" id="hdnAbsencesEveryWeeks" value="" runat="server" clientidmode="Static" />
    <input type="hidden" id="hdnAbsencesEveryMonths" value="" runat="server" clientidmode="Static" />
    <input type="hidden" id="hdnAbsencesEveryFlexible1" value="" runat="server" clientidmode="Static" />
    <input type="hidden" id="hdnAbsencesEveryFlexible2" value="" runat="server" clientidmode="Static" />

    <div id="divMainBody">
        <!-- TAB SUPERIOR -->
        <div id="divTabInfo" class="divDataCells">
            <div style="min-height: 10px"></div>
            <div id="divCause" class="blackRibbonTitle">
            </div>
            <div style="min-height: 10px"></div>
        </div>
        <!-- FIN TAB SUPERIOR -->

        <!-- ARBOL Y DETALLE -->
        <div id="divTabData" class="divDataCells">

            <div id="divTree" class="treeSize">
                <div id="ctlTreeDiv" style="height: 500px;">
                    <rws:roTreesSelector ID="roTreesCauses" runat="server" ShowEmployeeFilters="false" PrefixTree="roTreesCauses"
                        Tree1Visible="true" Tree1MultiSel="false" Tree1ShowOnlyGroups="false" Tree1Function="cargaNodo"
                        Tree1ImagePath="images/CauseSelector"
                        Tree1SelectorPage="../../Causes/CauseSelectorData.aspx"
                        ShowTreeCaption="true"></rws:roTreesSelector>
                </div>
            </div>

            <div id="divButtons" class="divMiddleButtons">
                <div id="divBarButtons" class="maxHeight">&nbsp</div>
            </div>

            <div id="divContenido" class="divRightContent">
                <div id="divContent" class="maxHeight">
                    <dx:ASPxCallbackPanel ID="ASPxCallbackPanelContenido" runat="server" Width="100%" Height="100%" ClientInstanceName="ASPxCallbackPanelContenidoClient">
                        <SettingsLoadingPanel Enabled="false" />
                        <ClientSideEvents EndCallback="ASPxCallbackPanelContenidoClient_EndCallBack" />
                        <PanelCollection>
                            <dx:PanelContent ID="PanelContent1" runat="server">
                                <input type="hidden" id="hdnRecalcChanges" value="0" runat="server" />
                                <input type="hidden" id="hdnReadOnlyMode" value="0" runat="server" />

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
                                    <%--GENERAL--%>
                                    <div id="div00" class="contentPanel" style="display: none;" runat="server">
                                        <!-- Este div es el header -->
                                        <div class="panBottomMargin">
                                            <div class="panHeader2 panBottomMargin">
                                                <span class="panelTitleSpan">
                                                    <asp:Label runat="server" ID="lblCausesGeneral" Text="General"></asp:Label>
                                                </span>
                                            </div>
                                            <!-- La descripción es opcional -->
                                            <div class="panelHeaderContent">
                                                <div class="panelDescriptionImage">
                                                    <img alt="" src="<%=Me.Page.ResolveUrl("~/Causes/Images/Causes.png")%>" />
                                                </div>
                                                <div class="panelDescriptionText">
                                                    <asp:Label ID="lblCausesDescription" runat="server" Text="Datos específicos de la justificación."></asp:Label>
                                                </div>
                                            </div>
                                        </div>

                                        <!-- Este div es un formulario -->
                                        <div class="panBottomMargin">
                                            <div class="divRow">
                                                <div class="divRowDescription">
                                                    <asp:Label ID="lblNameDescription" runat="server" Text="Nombre identificativo de la justificación"></asp:Label>
                                                </div>
                                                <asp:Label ID="lblName" runat="server" Text="Nombre:" CssClass="labelForm"></asp:Label>
                                                <div class="componentForm">
                                                    <dx:ASPxTextBox ID="txtName" runat="server" ClientInstanceName="txtName_Client" NullText="_____">
                                                        <ClientSideEvents Validation="LengthValidation" TextChanged="function(s,e){checkCauseEmptyName(s.GetValue());}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                        <ValidationSettings SetFocusOnError="True">
                                                            <RequiredField IsRequired="True" ErrorText="(*)" />
                                                        </ValidationSettings>
                                                    </dx:ASPxTextBox>
                                                </div>
                                            </div>
                                            <div class="divRow">
                                                <div class="divRowDescription">
                                                    <asp:Label ID="lblShortNameDescription" runat="server" Text="Nombre corto de la justificación."></asp:Label>
                                                </div>
                                                <asp:Label ID="lblShortName" runat="server" Text="Nombre abreviado:" CssClass="labelForm"></asp:Label>
                                                <div class="componentForm">
                                                    <dx:ASPxTextBox ID="txtShortName" runat="server" MaxLength="3" Width="50" NullText="___">
                                                        <ClientSideEvents Validation="LengthValidation" TextChanged="function(s,e){ hasChanges(true, false)}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                        <ValidationSettings SetFocusOnError="True">
                                                            <RequiredField IsRequired="True" ErrorText="(*)" />
                                                        </ValidationSettings>
                                                    </dx:ASPxTextBox>
                                                </div>
                                            </div>
                                            <div class="divRow">
                                                <div class="divRowDescription">
                                                    <asp:Label ID="Label8" runat="server" Text="Descripción de la justificación."></asp:Label>
                                                </div>
                                                <asp:Label ID="lblDescription" runat="server" Text="Descripción:" CssClass="labelForm"></asp:Label>
                                                <div class="componentForm">
                                                    <dx:ASPxMemo ID="txtDescription" runat="server" Rows="2" Width="100%" Height="40">
                                                        <ClientSideEvents TextChanged="function(s,e){ hasChanges(true, false)}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                    </dx:ASPxMemo>
                                                </div>
                                            </div>

                                            <div class="divRow" id="divColorCause" runat="server" style="display: none">
                                                <div class="divRowDescription">
                                                    <asp:Label ID="lblColorDesc" runat="server" Text="Color usado para identificar la justificación en las exportaciones."></asp:Label>
                                                </div>
                                                <asp:Label ID="lblColorTitle" runat="server" Text="Color identificativo:" CssClass="labelForm"></asp:Label>
                                                <div class="componentForm">
                                                    <dx:ASPxColorEdit ID="colorCause" runat="server" EnableCustomColors="true" Width="14px">
                                                        <ClientSideEvents ColorChanged="function(s,e){hasChanges(true, false);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                    </dx:ASPxColorEdit>
                                                </div>
                                            </div>

                                            <div class="divRow">
                                                <div class="divRowDescription">
                                                    <asp:Label ID="lblFactorDes" runat="server" Text="Equivalencia."></asp:Label>
                                                </div>
                                                <asp:Label ID="lblFactor" runat="server" Text="Equivalencia:" CssClass="labelForm"></asp:Label>
                                                <div class="componentForm">
                                                    <dx:ASPxTextBox ID="txtFactor" runat="server">
                                                        <ClientSideEvents TextChanged="function(s,e){hasChanges(true, false);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                        <MaskSettings IncludeLiterals="None" />
                                                    </dx:ASPxTextBox>
                                                </div>
                                            </div>
                                        </div>

                                        <!-- Centro de costes -->
                                        <div class="panBottomMargin">
                                            <div class="panHeader2 panBottomMargin">
                                                <span class="panelTitleSpan">
                                                    <asp:Label runat="server" ID="lblCaptionCost" Text="Costes"></asp:Label>
                                                </span>
                                            </div>
                                            <!-- La descripción es opcional -->
                                            <div class="panelHeaderContent">
                                                <div class="panelDescriptionImage">
                                                    <img alt="" src="<%=Me.Page.ResolveUrl("~/Tasks/Images/BusinessCenters48.png")%>" />
                                                </div>
                                                <div class="panelDescriptionText">
                                                    <asp:Label ID="lblDetailDescr" runat="server" Text="Indique el factor que se aplicará al coste del empleado en el caso de asignarle esta justificación."></asp:Label>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="panBottomMargin">
                                            <div id="divCostFactor" runat="server" class="divRow">
                                                <div class="divRowDescription">
                                                    <asp:Label ID="lblCostFactorDetail" runat="server" Text="Factor de coste a aplicar sobre el coste del empleado."></asp:Label>
                                                </div>
                                                <asp:Label ID="lblCostFactorCaption" runat="server" Text="Factor de coste:" CssClass="labelForm"></asp:Label>
                                                <div class="componentForm">
                                                    <dx:ASPxTextBox runat="server" ID="txtCostFactor" MaxLength="9" Width="75" ClientInstanceName="txtCostFactorClient">
                                                        <ClientSideEvents TextChanged="function(s,e){hasChanges(true, false);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                        <MaskSettings Mask="<-99..99>.<000..999>" IncludeLiterals="DecimalSymbol" />
                                                    </dx:ASPxTextBox>
                                                    <%--<dx:ASPxSpinEdit ID="txtCostFactor" runat="server" MaxLength="9" Width="75" DisplayFormatString="N">
                                                  <ClientSideEvents ValueChanged="function(s,e){hasChanges(true, false);}" GotFocus="function(s, e){ s.SelectAll() }" />
                                                  <SpinButtons ShowIncrementButtons="true" ShowLargeIncrementButtons="false"></SpinButtons>
                                                </dx:ASPxSpinEdit>--%>
                                                </div>
                                            </div>
                                            <div id="divCostDetailNoLicense" style="clear: both" runat="server">
                                                <div class="divRowDescription">
                                                    <asp:Label ID="lblNotLicenseCostControl" ForeColor="#2D4155" Font-Bold="true" runat="server" Text="Debe aquirir la licencia de control de costes para poder utilizar dicha funcionalidad."></asp:Label>
                                                </div>
                                            </div>
                                        </div>

                                        <!-- Grupo de negocio -->
                                        <div class="panBottomMargin">
                                            <div class="panHeader2 panBottomMargin">
                                                <span class="panelTitleSpan">
                                                    <asp:Label runat="server" ID="lblBusinessGroupDesc" Text="Grupo de Negocio"></asp:Label>
                                                </span>
                                            </div>
                                        </div>

                                        <div class="panBottomMargin">
                                            <div class="divRow">
                                                <div class="divRowDescription">
                                                    <asp:Label ID="lblBusinessGroupDescPpal" runat="server" Text="Los grupos de negocio se utilizan para limitar las justificaciones cada usuario en la pantalla de edición de fichajes."></asp:Label>
                                                </div>
                                                <asp:Label ID="lblBusinessGroup" runat="server" Text="Grupo de Negocio:" CssClass="labelForm"></asp:Label>
                                                <div class="componentForm">
                                                    <dx:ASPxTextBox ID="txtBusinessGroup" runat="server">
                                                        <ClientSideEvents TextChanged="function(s,e){ hasChanges(true, false);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                    </dx:ASPxTextBox>
                                                </div>
                                            </div>
                                        </div>

                                        <div id="causesV3Config" runat="server" style="display: none">
                                            <div class="panBottomMargin">
                                                <div class="panHeader2 panBottomMargin">
                                                    <span class="panelTitleSpan">
                                                        <asp:Label runat="server" ID="lblAuthorizationsTitle" Text="Flujo de autorizaciones"></asp:Label>
                                                    </span>
                                                </div>
                                            </div>

                                            <div class="panBottomMargin">
                                                <div class="divRow">
                                                    <div class="divRowDescription">
                                                        <asp:Label ID="lblCategoriesDesc" runat="server" Text="Indique la categoría a la que pertenece la justificación para determinar el flujo de autorizaciones que se aplicará en el momento de realizar una solicitud de este tipo"></asp:Label>
                                                    </div>
                                                    <asp:Label ID="lblCategory" runat="server" Text="Categoría:" CssClass="labelForm"></asp:Label>
                                                    <div class="componentForm">
                                                        <dx:ASPxComboBox ID="cmbRequestCategory" runat="server" Width="200px" ClientInstanceName="cmbRequestCategoryClient">
                                                            <ClientSideEvents SelectedIndexChanged="function(s,e){hasChanges(true);}" />
                                                        </dx:ASPxComboBox>

                                                        <!-- Aquí comença -->
                                                        <table>
                                                            <tr id="trMinLevel" runat="server">
                                                                <td align="left" style="padding-left: 10px;">
                                                                    <div style="float: left; padding-left: 10px">
                                                                        <input id="chkMinLevel" type="checkbox" runat="server" onchange="hasChanges(true);"
                                                                            class="textClass x-form-text x-form-field" onblur="this.className='textClass x-form-text x-form-field';" onfocus="this.className='textClass x-form-text x-form-field x-form-focus';" />
                                                                        <asp:Label ID="lblMinLevel" Text="Empezar en el nivel de autorización " runat="server"></asp:Label>
                                                                    </div>
                                                                    <div style="float: left; padding-left: 10px">
                                                                        <dx:ASPxComboBox ID="cmbMinLevel" runat="server" Width="55px" ClientInstanceName="cmbMinLevelClient">
                                                                            <ClientSideEvents SelectedIndexChanged="function(s,e){hasChanges(true);}" />
                                                                        </dx:ASPxComboBox>
                                                                    </div>
                                                                    <div style="float: left; padding-left: 10px">
                                                                        <asp:Label ID="lblMinLevelBis" Text=" en lugar de 11 (el más bajo) " runat="server"></asp:Label>
                                                                    </div>
                                                                </td>
                                                            </tr>

                                                            <tr id="trAtLevel" runat="server">
                                                                <td align="left" style="padding-left: 10px;">
                                                                    <div style="float: left; padding-left: 10px">
                                                                        <input id="chkAtLevel" type="checkbox" runat="server" onchange="hasChanges(true);"
                                                                            class="textClass x-form-text x-form-field" onblur="this.className='textClass x-form-text x-form-field';" onfocus="this.className='textClass x-form-text x-form-field x-form-focus';" />
                                                                        <asp:Label ID="lblAtLevel" Text="Considera aprobada al llegar a nivel " runat="server"></asp:Label>
                                                                    </div>
                                                                    <div style="float: left; padding-left: 10px">
                                                                        <dx:ASPxComboBox ID="cmbAtLevel" runat="server" Width="55px" ClientInstanceName="cmbAtLevelClient">
                                                                            <ClientSideEvents SelectedIndexChanged="function(s,e){hasChanges(true);}" />
                                                                        </dx:ASPxComboBox>
                                                                    </div>
                                                                    <div style="float: left; padding-left: 10px">
                                                                        <asp:Label ID="lblAtLevelBis" Text=" en lugar de 1 (el más alto) " runat="server"></asp:Label>
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                        <!-- Aquí acaba -->
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <%-- TYPES --%>
                                    <div id="div01" class="contentPanel" style="display: none;" runat="server">
                                        <!-- Este div es el header -->
                                        <div class="panBottomMargin">
                                            <div class="panHeader2 panBottomMargin">
                                                <span class="panelTitleSpan">
                                                    <asp:Label runat="server" ID="lblHeaderTypes" Text="Tipos"></asp:Label>
                                                </span>
                                            </div>
                                            <!-- La descripción es opcional -->
                                            <div class="panelHeaderContent">
                                                <div class="panelDescriptionImage">
                                                    <img alt="" src="<%=Me.Page.ResolveUrl("~/Causes/Images/cause_type.png")%>" />
                                                </div>
                                                <div class="panelDescriptionText">
                                                    <div>
                                                        <asp:Label ID="lblCauseTypeDescription" runat="server" Text="Especifique el tipo de ${Cause}, si aplica a horas, días o un valor personalizado."></asp:Label>
                                                    </div>
                                                    <div>
                                                        <table style="padding-top: 10px; padding-left: 10px;">
                                                            <tr>
                                                                <td>
                                                                    <asp:Label ID="lblCauseType" runat="server" Text="Tipo de justificación:"></asp:Label></td>
                                                                <td style="width: 175px;" align="left" colspan="2">
                                                                    <dx:ASPxComboBox runat="server" ID="cmbCauseType" Width="220px" ClientInstanceName="cmbCauseTypeClient">
                                                                        <ClientSideEvents SelectedIndexChanged="function(s,e){hasChanges(true,true);causeTypeVisible(s.GetSelectedItem()); }" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                        <ValidationSettings ErrorDisplayMode="None" />
                                                                    </dx:ASPxComboBox>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div style="display: none" id="rowCauseHours" class="divRow">
                                            <roUserControls:roOptionPanelClient ID="optHoursProductive" runat="server" TypeOPanel="RadioOption" width="100%" height="Auto" Checked="False" Enabled="True" CConClick="causeTypeChange();">
                                                <Title>
                                                    <asp:Label ID="optHoursProductiveTitle" runat="server" Text="Horas productivas"></asp:Label>
                                                </Title>
                                                <Description>
                                                    <asp:Label ID="optHoursProductiveDesc" runat="server" Text="Esta ${Cause} representa horas productivas del ${Employee}. Por ejemplo, horas trabajadas, trabajo externo u horas recuperadas."></asp:Label>
                                                </Description>
                                                <Content>
                                                    <div style="padding-top: 10px; padding-left: 25px;">
                                                        <roUserControls:roOptionPanelClient ID="optProductiveHoursOnCC" runat="server" TypeOPanel="RadioOption" width="100%" height="Auto" Checked="False" Enabled="True" CConClick="causeProductive();">
                                                            <Title>
                                                                <asp:Label ID="optProductiveHoursOnCCTitle" runat="server" Text="En el centro de trabajo"></asp:Label>
                                                            </Title>
                                                            <Description>
                                                                <asp:Label ID="optProductiveHoursOnCCDesc" runat="server" Text="Esta ${Cause} se genera cuando se esta trabajando en la empresa."></asp:Label>
                                                            </Description>
                                                            <Content>
                                                                <table style="padding-top: 5px; padding-left: 20px;">
                                                                    <tr>
                                                                        <td>

                                                                            <div style="float: left; line-height: 20px;">
                                                                                <asp:Label ID="lblAbsenceConeptBalanceProductive" runat="server" Text="Al aprobar una solicitud de presencia por días o por horas, el saldo utilizado como saldo actual será el siguiente: "></asp:Label>
                                                                            </div>
                                                                            <div style="float: left; padding-left: 15px">
                                                                                <dx:ASPxComboBox runat="server" ID="cmbConceptBalanceProductive" Width="220px" ClientInstanceName="cmbConceptBalanceProductiveClient">
                                                                                    <ClientSideEvents SelectedIndexChanged="function(s,e){hasChanges(true,false);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                                    <ValidationSettings ErrorDisplayMode="None" />
                                                                                </dx:ASPxComboBox>
                                                                            </div>

                                                                            <div style="clear: both; float: left; padding-top: 5px; line-height: 20px;">
                                                                                <asp:Label ID="lblAbsenceConceptBalanceProductiveDays" runat="server" Text="Y en ese caso solo se tendrán en cuenta los días: "></asp:Label>
                                                                            </div>
                                                                            <div style="float: left; padding-left: 15px; padding-top: 5px;">
                                                                                <dx:ASPxComboBox runat="server" ID="cmbConceptBalanceProductiveDays" Width="220px" ClientInstanceName="cmbConceptBalanceProductiveDaysClient">
                                                                                    <ClientSideEvents SelectedIndexChanged="function(s,e){hasChanges(true,false);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                                    <ValidationSettings ErrorDisplayMode="None" />
                                                                                </dx:ASPxComboBox>
                                                                            </div>
                                                                        </td>
                                                                        <%--    <td style="width: 175px;" align="left" colspan="2">
                                                                            <dx:ASPxComboBox runat="server" ID="cmbConceptBalanceProductive" Width="220px" ClientInstanceName="cmbConceptBalanceProductiveClient">
                                                                                <ClientSideEvents SelectedIndexChanged="function(s,e){hasChanges(true,false);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                                <ValidationSettings ErrorDisplayMode="None" />
                                                                            </dx:ASPxComboBox>
                                                                        </td>--%>
                                                                    </tr>
                                                                </table>
                                                            </Content>
                                                        </roUserControls:roOptionPanelClient>

                                                        <roUserControls:roOptionPanelClient ID="optExternalProductiveHours" runat="server" TypeOPanel="RadioOption" width="100%" height="Auto" Checked="False" Enabled="True" CConClick="causeProductive();">
                                                            <Title>
                                                                <asp:Label ID="optExternalProductiveHoursTitle" runat="server" Text="Trabajo externo"></asp:Label>
                                                            </Title>
                                                            <Description>
                                                                <asp:Label ID="optExternalProductiveHoursDesc" runat="server" Text="Esta ${Cause} se genera cuando no se esta trabajando en la empresa."></asp:Label>
                                                            </Description>
                                                            <Content>
                                                            </Content>
                                                        </roUserControls:roOptionPanelClient>
                                                    </div>
                                                </Content>
                                            </roUserControls:roOptionPanelClient>
                                            <br />
                                            <roUserControls:roOptionPanelClient ID="optHoursNonProductive" runat="server" TypeOPanel="RadioOption" width="100%" height="Auto" Checked="False" Enabled="True" CConClick="causeTypeChange();">
                                                <Title>
                                                    <asp:Label ID="optHoursNonProductiveTitle" runat="server" Text="Horas no productivas."></asp:Label>
                                                </Title>
                                                <Description>
                                                    <asp:Label ID="optHoursNonProductiveDesc" runat="server" Text="Esta ${Cause} se genera cuando el ${Employee} está ausente. Por ejemplo, visita médico u horas de asuntos propios."></asp:Label>
                                                </Description>
                                                <Content>
                                                    <div style="padding-top: 10px; padding-left: 25px;">
                                                        <roUserControls:roOptionPanelClient ID="optHoursNonProductiveHoliday" runat="server" TypeOPanel="RadioOption" width="100%" height="Auto" Checked="False" Enabled="True" CConClick="causeNonProductive();">
                                                            <Title>
                                                                <asp:Label ID="optHoursNonProductiveHolidayTitle" runat="server" Text="Vacaciones o permisos pre-autorizados"></asp:Label>
                                                            </Title>
                                                            <Description>
                                                                <asp:Label ID="optHoursNonProductiveHolidayDesc" runat="server" Text="Esta ${Cause} se genera cuando el ${Employee} esta disfrutando de un permiso o vacaciones."></asp:Label>
                                                            </Description>
                                                            <Content>
                                                                <table style="padding-top: 5px; padding-left: 20px;">
                                                                    <tr>
                                                                        <td>
                                                                            <asp:Label ID="lblConceptBalance" runat="server" Text="Al aprobar una solicitud de vacaciones o permisos, el saldo utilizado como saldo actual será el siguiente: "></asp:Label>
                                                                            &nbsp;&nbsp;
                                                                        </td>
                                                                        <td style="width: 175px;" align="left" colspan="2">
                                                                            <dx:ASPxComboBox runat="server" ID="cmbConceptBalance" Width="220px" ClientInstanceName="cmbConceptBalanceClient">
                                                                                <ClientSideEvents SelectedIndexChanged="function(s,e){hasChanges(true,false);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                                <ValidationSettings ErrorDisplayMode="None" />
                                                                            </dx:ASPxComboBox>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </Content>
                                                        </roUserControls:roOptionPanelClient>

                                                        <roUserControls:roOptionPanelClient ID="optHoursNonProductiveCause" runat="server" TypeOPanel="RadioOption" width="100%" height="Auto" Checked="False" Enabled="True" CConClick="causeNonProductive();">
                                                            <Title>
                                                                <asp:Label ID="optHoursNonProductiveCauseTitle" runat="server" Text="Ausencia por otros motivos."></asp:Label>
                                                            </Title>
                                                            <Description>
                                                                <asp:Label ID="optHoursNonProductiveCauseDesc" runat="server" Text="Por ejemplo, una salida al médico o un retraso.."></asp:Label>
                                                            </Description>
                                                            <Content>
                                                                <table style="padding-top: 5px; padding-left: 20px; width: 100%;">
                                                                    <tr>
                                                                        <td>
                                                                            <div style="float: left; line-height: 20px;">
                                                                                <asp:Label ID="lblAbsenceConeptBalance" runat="server" Text="Al aprobar una solicitud de ausencia por días o horas, el saldo utilizado como saldo actual será el siguiente: "></asp:Label>
                                                                            </div>
                                                                            <div style="float: left; padding-left: 15px">
                                                                                <dx:ASPxComboBox runat="server" ID="cmbAbsenceConceptBalance" Width="220px" ClientInstanceName="cmbAbsenceConceptBalanceClient">
                                                                                    <ClientSideEvents SelectedIndexChanged="function(s,e){hasChanges(true,false);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                                    <ValidationSettings ErrorDisplayMode="None" />
                                                                                </dx:ASPxComboBox>
                                                                            </div>
                                                                            <div style="clear: both; float: left; padding-top: 5px; line-height: 20px;">
                                                                                <asp:Label ID="lblAbsenceConeptBalanceDays" runat="server" Text="Solo se tendrán en cuenta los días: "></asp:Label>
                                                                            </div>
                                                                            <div style="float: left; padding-left: 15px; padding-top: 5px;">
                                                                                <dx:ASPxComboBox runat="server" ID="cmbAbsenceConceptBalanceDays" Width="220px" ClientInstanceName="cmbAbsenceConceptBalanceDaysClient">
                                                                                    <ClientSideEvents SelectedIndexChanged="function(s,e){hasChanges(true,false);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                                    <ValidationSettings ErrorDisplayMode="None" />
                                                                                </dx:ASPxComboBox>
                                                                            </div>
                                                                        </td>
                                                                        <td style="width: 175px;" align="left" colspan="2"></td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>
                                                                            <div id="causeDefaultParameters" runat="server" style="display: none">
                                                                                <roUserControls:roGroupBox ID="GroupBox2" runat="server">
                                                                                    <Content>
                                                                                        <div class="panHeader2" style="margin-top: 8px; margin-bottom: 5px; width: 100%">
                                                                                            <span style="">
                                                                                                <asp:Label ID="lblHeaderTypesAus" Text="Valores por defecto" runat="server" /></span>
                                                                                        </div>
                                                                                        <table style="height: 163px;">
                                                                                            <tr style="height: 23px;">
                                                                                                <td style="padding-left: 10px;" colspan="2">
                                                                                                    <asp:Label ID="lblCauseDefaultInfo" runat="server" Text="Si lo desea, puede indicar los valores por defecto al crear una previsión de ausencia:"></asp:Label>
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr style="height: 35px;">
                                                                                                <td style="padding-left: 20px;">
                                                                                                    <input id="chkMaxDays" runat="server" type="checkbox" onclick="ActivateMaxDays(); hasChanges(true, false);" />
                                                                                                </td>
                                                                                                <td>
                                                                                                    <table>
                                                                                                        <tr>
                                                                                                            <td>
                                                                                                                <asp:Label ID="lblDaysDuration" runat="server" Text="Nº máximo de días que puede durar la ausencia" Style="padding-right: 8px;"></asp:Label>
                                                                                                            </td>
                                                                                                            <td>
                                                                                                                <dx:ASPxTextBox ID="txtDaysDuration" runat="server" Width="30px" MaxLength="3" ClientInstanceName="txtDaysDurationClient">
                                                                                                                    <ClientSideEvents TextChanged="function(s,e){ hasChanges(true, false); }" />
                                                                                                                    <MaskSettings Mask="<0..999>" IncludeLiterals="None" />
                                                                                                                    <ValidationSettings ErrorDisplayMode="None" />
                                                                                                                </dx:ASPxTextBox>
                                                                                                            </td>
                                                                                                        </tr>
                                                                                                    </table>
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr style="height: 35px;">
                                                                                                <td style="padding-left: 20px;">
                                                                                                    <input id="chkTimePeriodAllowed" runat="server" type="checkbox" onclick="ActivateTimePeriodAllowed(); hasChanges(true, false);" />
                                                                                                </td>
                                                                                                <td>
                                                                                                    <table border="0">
                                                                                                        <tr>
                                                                                                            <td valign="middle" style="padding-right: 8px;">
                                                                                                                <asp:Label ID="lblFromPeriod" runat="server" Text="Período permitido de ausencia entre las"></asp:Label>
                                                                                                            </td>
                                                                                                            <td valign="top" style="width: 60px;">
                                                                                                                <dx:ASPxTimeEdit ID="txtFromPeriod" EditFormatString="HH:mm" EditFormat="Custom" runat="server" Width="85px" ClientInstanceName="txtFromPeriodClient">
                                                                                                                    <ClientSideEvents DateChanged="function(s,e){ hasChanges(true, false); }" />
                                                                                                                    <ValidationSettings ErrorDisplayMode="None" />
                                                                                                                </dx:ASPxTimeEdit>
                                                                                                            </td>
                                                                                                            <td valign="middle" style="padding-left: 10px; padding-right: 5px;">
                                                                                                                <asp:Label ID="lblToPeriod" runat="server" Text="y las"></asp:Label>
                                                                                                            </td>
                                                                                                            <td valign="top" style="width: 60px;">
                                                                                                                <dx:ASPxTimeEdit ID="txtToPeriod" EditFormatString="HH:mm" EditFormat="Custom" runat="server" Width="85px" ClientInstanceName="txtToPeriodClient">
                                                                                                                    <ClientSideEvents DateChanged="function(s,e){ hasChanges(true, false); }" />
                                                                                                                    <ValidationSettings ErrorDisplayMode="None" />
                                                                                                                </dx:ASPxTimeEdit>
                                                                                                            </td>
                                                                                                        </tr>
                                                                                                    </table>
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr style="height: 35px;">
                                                                                                <td style="padding-left: 20px;">
                                                                                                    <input id="chkTimeBetween" runat="server" type="checkbox" onclick="ActivateTimeBetween(); hasChanges(true, false);" />
                                                                                                </td>
                                                                                                <td>
                                                                                                    <table border="0">
                                                                                                        <tr>
                                                                                                            <td valign="middle" style="padding-right: 5px;">
                                                                                                                <asp:Label ID="lblMinDuration" runat="server" Text="Se debe ausentar un mínimo de"></asp:Label>
                                                                                                                &nbsp;
                                                                                                            </td>
                                                                                                            <td valign="top" style="width: 60px">
                                                                                                                <dx:ASPxTimeEdit ID="txtMinDuration" EditFormatString="HH:mm" EditFormat="Custom" runat="server" Width="85px" ClientInstanceName="txtMinDurationClient">
                                                                                                                    <ClientSideEvents DateChanged="function(s,e){ hasChanges(true, false); }" />
                                                                                                                    <ValidationSettings ErrorDisplayMode="None" />
                                                                                                                </dx:ASPxTimeEdit>
                                                                                                            </td>
                                                                                                            <td valign="middle" style="">
                                                                                                                <asp:Label ID="lblMaxDuration" runat="server" Text="y un máximo de"></asp:Label>
                                                                                                            </td>
                                                                                                            <td valign="top" style="width: 60px;">
                                                                                                                <dx:ASPxTimeEdit ID="txtMaxDuration" EditFormatString="HH:mm" EditFormat="Custom" runat="server" Width="85px" ClientInstanceName="txtMaxDurationClient">
                                                                                                                    <ClientSideEvents DateChanged="function(s,e){ hasChanges(true, false); }" />
                                                                                                                    <ValidationSettings ErrorDisplayMode="None" />
                                                                                                                </dx:ASPxTimeEdit>
                                                                                                            </td>
                                                                                                        </tr>
                                                                                                    </table>
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr style="height: 54px">
                                                                                                <td>&nbsp;</td>
                                                                                            </tr>
                                                                                        </table>
                                                                                    </Content>
                                                                                </roUserControls:roGroupBox>
                                                                            </div>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </Content>
                                                        </roUserControls:roOptionPanelClient>
                                                    </div>
                                                </Content>
                                            </roUserControls:roOptionPanelClient>
                                        </div>

                                        <div style="display: none" id="rowCauseDays" class="divRow">
                                            <roUserControls:roOptionPanelClient ID="optDaysCause" runat="server" TypeOPanel="RadioOption" width="100%" height="Auto" Checked="True" Enabled="True" CConClick="causeTypeChange();">
                                                <Title>
                                                    <asp:Label ID="optDaysCauseTitle" runat="server" Text="Días"></asp:Label>
                                                </Title>
                                                <Description>
                                                    <asp:Label ID="optDaysCauseDesc" runat="server" Text="Esta ${Cause} se generará diariamente en función de su composición."></asp:Label>
                                                </Description>
                                                <Content>
                                                    <roUserControls:roOptionPanelClient ID="optDaysFromCause" runat="server" TypeOPanel="CheckboxOption" width="100%" height="Auto" Checked="False" Enabled="True" CConClick="causeTypeChange();">
                                                        <Title>
                                                            <asp:Label ID="optDaysFromCauseTitle" runat="server" Text="Generar a partir de una justificación por horas"></asp:Label>
                                                        </Title>
                                                        <Description>
                                                            <asp:Label ID="optDaysFromCauseDesc" runat="server" Text="Esta ${Cause} se generará automáticamente a partir de una justificación por horas según los criterios especificados."></asp:Label>
                                                        </Description>
                                                        <Content>
                                                            <div class="divRow">
                                                                <div class="divRowDescription">
                                                                    <asp:Label ID="lblCauseFromDesc" runat="server" Text=""></asp:Label>
                                                                </div>
                                                                <asp:Label ID="lblCauseFromTitle" runat="server" Text="Justificación:" CssClass="labelForm"></asp:Label>
                                                                <div class="componentForm">
                                                                    <dx:ASPxComboBox runat="server" ID="cmbDayCauses" Width="220px" ClientInstanceName="cmbDayCausesClient">
                                                                        <ClientSideEvents SelectedIndexChanged="function(s,e){hasChanges(true,true); }" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                        <ValidationSettings ErrorDisplayMode="None" />
                                                                    </dx:ASPxComboBox>
                                                                </div>
                                                            </div>
                                                            <div class="divRow">
                                                                <asp:Label ID="lblDayFactor" runat="server" Text="Factor:" CssClass="labelForm"></asp:Label>
                                                                <div class="componentForm">
                                                                    <div style="float: left; width: 20%; clear: both">
                                                                        <dx:ASPxRadioButton GroupName="DaysFromCauseSource" ID="ckCauseFromWorkingHours" runat="server" ClientInstanceName="ckCauseFromWorkingHoursClient" Text="Las horas teóricas del día">
                                                                            <ClientSideEvents CheckedChanged="function(s,e){hasChanges(true,true); }" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                        </dx:ASPxRadioButton>
                                                                    </div>

                                                                    <div style="float: left; width: 20%; clear: both">
                                                                        <dx:ASPxRadioButton GroupName="DaysFromCauseSource" ID="ckCauseFromUserField" runat="server" ClientInstanceName="ckCauseFromUserFieldClient" Text="Las horas teóricas indicadas en el campo">
                                                                            <ClientSideEvents CheckedChanged="function(s,e){hasChanges(true,true); }" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                        </dx:ASPxRadioButton>
                                                                    </div>
                                                                    <div style="float: right; width: calc(80% - 4px)">
                                                                        <dx:ASPxComboBox ID="cmbCauseFromUserField" runat="server" ClientInstanceName="cmbCauseFromUserFieldClient" Width="250px">
                                                                            <ClientSideEvents SelectedIndexChanged="function(s,e){hasChanges(true,true); }" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                        </dx:ASPxComboBox>
                                                                    </div>
                                                                    <div style="float: left; width: 20%; clear: both">
                                                                        <dx:ASPxRadioButton GroupName="DaysFromCauseSource" ID="ckCauseFromFactor" runat="server" ClientInstanceName="ckCauseFromFactorClient" Text="Factor multiplicador fijo">
                                                                            <ClientSideEvents CheckedChanged="function(s,e){hasChanges(true,true); }" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                        </dx:ASPxRadioButton>
                                                                    </div>
                                                                    <div style="float: right; width: calc(80% - 4px)">
                                                                        <dx:ASPxTextBox runat="server" ID="txtCauseFromFactor" MaxLength="12" Width="75" ClientInstanceName="txtCauseFromFactor">
                                                                            <ClientSideEvents TextChanged="function(s,e){hasChanges(true,true); }" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                            <MaskSettings Mask="<-9999..9999>.<000000..999999>" />
                                                                        </dx:ASPxTextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </Content>
                                                    </roUserControls:roOptionPanelClient>
                                                </Content>
                                            </roUserControls:roOptionPanelClient>
                                            <br />
                                        </div>

                                        <div style="display: none" id="rowCauseCustom" class="divRow">
                                            <roUserControls:roOptionPanelClient ID="optCustomized" runat="server" TypeOPanel="RadioOption" width="100%" height="Auto" Checked="True" Enabled="True" CConClick="causeTypeChange();">
                                                <Title>
                                                    <asp:Label ID="optCustomizedTitle" runat="server" Text="Personalizado"></asp:Label>
                                                </Title>
                                                <Description>
                                                    <asp:Label ID="optCustomizedDescription" runat="server" Text="Esta ${Cause} puede representar valores como un número de veces o número de tickets."></asp:Label>
                                                </Description>
                                                <Content>
                                                </Content>
                                            </roUserControls:roOptionPanelClient>
                                        </div>
                                    </div>

                                    <%--ROUNDS--%>
                                    <div id="div04" class="contentPanel" style="display: none;" runat="server">
                                        <div class="panHeader2">
                                            <span style="">
                                                <asp:Label ID="lblHeaderRounds" Text="Redondeo" runat="server" /></span>
                                        </div>
                                        <br />
                                        <table cellpadding="2" cellspacing="2" style="width: 90%; padding-left: 20px;">
                                            <tr style="height: 48px">
                                                <td align="center" width="60px">
                                                    <asp:Image ID="imgRounds" ImageUrl="Images/cause_round.png" runat="server" />
                                                </td>
                                                <td align="left" width="100%" style="padding-left: 10px;">
                                                    <asp:Label ID="lblRoundsDescription" runat="server" Text="Las opciones de redondeo se utilizan para ajustar la duración de la ${Cause}. Si desea redondear esta ${Cause} debe indicar el tiempo de redondeo al que se ajustará el valor."></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2" valign="top" style="padding: 0px;">
                                                    <table border="0" style="width: 100%; padding-top: 5px; padding-left: 5px;">
                                                        <tr style="height: 40px;">
                                                            <td>
                                                                <table>
                                                                    <tr>
                                                                        <td>
                                                                            <asp:Label ID="lblRoundValueDesc" runat="server" Text="Redondear el valor a" Style="padding-right: 10px"></asp:Label>
                                                                        </td>
                                                                        <td>
                                                                            <dx:ASPxTextBox ID="txtRoundBy" runat="server" Width="35px" MaxLength="2">
                                                                                <ClientSideEvents TextChanged="function(s,e){ hasChanges(true, true); }" />
                                                                                <MaskSettings Mask="<0..99>" IncludeLiterals="None" />
                                                                                <ValidationSettings ErrorDisplayMode="None" />
                                                                            </dx:ASPxTextBox>
                                                                        </td>
                                                                        <td>
                                                                            <asp:Label ID="lblDescMinutes" runat="server" Text="minutos" Style="padding-left: 5px"></asp:Label>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td style="padding: 0px; vertical-align: top;">
                                                                <table style="width: 100%;">
                                                                    <tr>
                                                                        <td style="padding-bottom: 10px;">
                                                                            <roUserControls:roOptionPanelClient ID="roOptValueUp" runat="server" TypeOPanel="RadioOption" width="100%" height="Auto" Checked="False" Enabled="True" CConClick="hasChanges(true, true);">
                                                                                <Title>
                                                                                    <asp:Label ID="lblOptValUp" runat="server" Text="Redondear por arriba"></asp:Label>
                                                                                </Title>
                                                                                <Description>
                                                                                </Description>
                                                                                <Content>
                                                                                </Content>
                                                                            </roUserControls:roOptionPanelClient>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td style="padding-bottom: 10px;">
                                                                            <roUserControls:roOptionPanelClient ID="roOptValueDown" runat="server" TypeOPanel="RadioOption" width="100%" height="Auto" Checked="False" Enabled="True" CConClick="hasChanges(true, true);">
                                                                                <Title>
                                                                                    <asp:Label ID="lblOptValDown" runat="server" Text="Redondear por abajo"></asp:Label>
                                                                                </Title>
                                                                                <Description>
                                                                                </Description>
                                                                                <Content>
                                                                                </Content>
                                                                            </roUserControls:roOptionPanelClient>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>
                                                                            <roUserControls:roOptionPanelClient ID="roOptValueAprox" runat="server" TypeOPanel="RadioOption" width="100%" height="Auto" Checked="False" Enabled="True" CConClick="hasChanges(true, true);">
                                                                                <Title>
                                                                                    <asp:Label ID="lblOptValAprox" runat="server" Text="Redondear por aproximación"></asp:Label>
                                                                                </Title>
                                                                                <Description>
                                                                                </Description>
                                                                                <Content>
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
                                        </table>

                                        <div class="panHeader2">
                                            <span style="">
                                                <asp:Label ID="lblScope" Text="Ámbito" runat="server" /></span>
                                        </div>
                                        <br />
                                        <table cellpadding="2" cellspacing="2" style="width: 90%; padding-left: 20px;">
                                            <tr style="height: 48px">
                                                <td align="center" width="60px">
                                                    <asp:Image ID="Image1" ImageUrl="Images/cause_round.png" runat="server" />
                                                </td>
                                                <td align="left" width="100%" style="padding-left: 10px;">
                                                    <asp:Label ID="lblScopeDesc" runat="server" Text="Puede definir el ábmito en el que se aplica el redondeo, si se aplica individualmente o a nivel diario."></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2" valign="top" style="padding: 0px;">
                                                    <table border="0" style="width: 100%; padding-top: 5px; padding-left: 5px;">
                                                        <tr>
                                                            <td style="padding: 0px; vertical-align: top;">
                                                                <table style="width: 100%;">
                                                                    <tr>
                                                                        <td style="padding-bottom: 10px;">
                                                                            <roUserControls:roOptionPanelClient ID="roOptIndivRound" runat="server" TypeOPanel="RadioOption" width="100%" height="Auto" Checked="False" Enabled="True" CConClick="hasChanges(true, true);">
                                                                                <Title>
                                                                                    <asp:Label ID="lblOptIndRoundTitle" runat="server" Text="Redondear individualmente"></asp:Label>
                                                                                </Title>
                                                                                <Description>
                                                                                </Description>
                                                                                <Content>
                                                                                </Content>
                                                                            </roUserControls:roOptionPanelClient>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td style="padding-bottom: 10px;">
                                                                            <roUserControls:roOptionPanelClient ID="roOptOneInDay" runat="server" TypeOPanel="RadioOption" width="100%" height="Auto" Checked="False" Enabled="True" CConClick="hasChanges(true, true);">
                                                                                <Title>
                                                                                    <asp:Label ID="lblOptOneatDayTitle" runat="server" Text="Redondear una vez al día"></asp:Label>
                                                                                </Title>
                                                                                <Description>
                                                                                </Description>
                                                                                <Content>
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
                                        </table>
                                    </div>

                                    <%--VISIBILITY--%>
                                    <div id="div03" class="contentPanel" style="display: none;" runat="server">
                                        <!-- Quien puede utilizar las justificaciones -->
                                        <div class="panBottomMargin">
                                            <div class="panHeader2 panBottomMargin" style="width: calc(100% - 5px)">
                                                <span class="panelTitleSpan">
                                                    <asp:Label runat="server" ID="lblWhoCanUseTitle" Text="Quién puede utilizar la ${Cause}"></asp:Label>
                                                </span>
                                            </div>

                                            <div class="panBottomMargin" style="margin-top: 10px;">
                                                <div class="divRow">
                                                    <div class="splitDivLeft">
                                                        <div class="panHeader2" style="width: calc(99% - 7px); padding: 10px 0 !important;">
                                                            <span style="padding: 0 10px;">
                                                                <asp:Label runat="server" ID="lblWhoCanUsePunch" Text="Uso en fichajes"></asp:Label>
                                                            </span>
                                                        </div>
                                                        <div style="width: calc(99% - 9px); margin-top: 1px; padding-top: 10px; border: 1px solid #CDCDCD">
                                                            <div class="OptionPanelDescStyle" style="padding-bottom: 5px">
                                                                <asp:Label ID="lblWhoCanUsePunchDesc" runat="server" Text="Desde este apartado, puede definir quién puede puede utilizar la ${Cause} seleccionada a través del terminal o el portal del empleado."></asp:Label>
                                                            </div>

                                                            <table style="width: 100%;">
                                                                <tr>
                                                                    <td style="padding-left: 5px;">
                                                                        <roUserControls:roOptionPanelClient ID="opPunchesAll" runat="server" TypeOPanel="RadioOption" width="100%" height="Auto" Checked="False" Enabled="True" CConClick="hasChanges(true, false);">
                                                                            <Title>
                                                                                <asp:Label ID="lbloptPunchesAll" runat="server" Text="Todos"></asp:Label>
                                                                            </Title>
                                                                            <Description>
                                                                                <asp:Label ID="lbloptPunchesAllDesc" runat="server" Text="Todos los ${Employees} podrán realizar fichajes introduciendo esta ${Cause}."></asp:Label>
                                                                            </Description>
                                                                            <Content>
                                                                            </Content>
                                                                        </roUserControls:roOptionPanelClient>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td style="padding-left: 5px; padding-top: 5px">
                                                                        <roUserControls:roOptionPanelClient ID="opPunchesNobody" runat="server" TypeOPanel="RadioOption" width="100%" height="Auto" Checked="False" Enabled="True" CConClick="hasChanges(true, false);">
                                                                            <Title>
                                                                                <asp:Label ID="lbloptPunchesNobody" runat="server" Text="Nadie"></asp:Label>
                                                                            </Title>
                                                                            <Description>
                                                                                <asp:Label ID="lbloptPunchesNobodyDesc" runat="server" Text="Ningún ${Employee} podrá realizar fichajes introduciendo esta ${Cause}."></asp:Label>
                                                                            </Description>
                                                                            <Content>
                                                                            </Content>
                                                                        </roUserControls:roOptionPanelClient>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td style="padding-left: 5px; padding-top: 5px">
                                                                        <roUserControls:roOptionPanelClient ID="opPunchesCriteria" runat="server" TypeOPanel="RadioOption" width="100%" height="Auto" Checked="False" Enabled="True" CConClick="hasChanges(true, false);checkPunchesVisibility();">
                                                                            <Title>
                                                                                <asp:Label ID="lbloptPunchesCriteria" runat="server" Text="Según criterio"></asp:Label>
                                                                            </Title>
                                                                            <Description>
                                                                                <asp:Label ID="optPunchesCriteriaDesc" runat="server" Text="Los ${Employees} que cumplan el siguiente criterio podrán realizar fichajes introduciendo esta ${Cause}."></asp:Label>
                                                                            </Description>
                                                                            <Content>
                                                                                <table border="0" width="100%" style="padding: 20px; padding-top: 5px;" align="center">
                                                                                    <tr>
                                                                                        <td id="punchesCell" align="left" style="display: none; padding-left: 12px;">
                                                                                            <roUserControls:roUserCtlFieldCriteria2 Prefix="ctl00_contentMainBody_ASPxCallbackPanelContenido_opPunchesCriteria_punchesCriteria" ID="punchesCriteria" runat="server" />
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

                                                    <div class="splitDivRight">
                                                        <div class="panHeader2" style="width: calc(99% - 7px); padding: 10px 0 !important;">
                                                            <span class="panelTitleSpan" style="padding: 0 10px;">
                                                                <asp:Label runat="server" ID="lblWhoCanUseRequests" Text="Uso en solicitudes o tramites"></asp:Label>
                                                            </span>
                                                        </div>

                                                        <div style="width: calc(99% - 9px); margin-top: 1px; padding-top: 10px; border: 1px solid #CDCDCD">
                                                            <div class="OptionPanelDescStyle" style="padding-bottom: 5px">
                                                                <asp:Label ID="lblWhoCanUseRequestsDescription" runat="server" Text="Desde este apartado, podrá seleccionar que ${Employees} podrá/n utilizar la ${Cause} seleccionada desde el Portal del ${Employee} en solicitudes y trámites."></asp:Label>
                                                            </div>

                                                            <table style="width: 100%;">
                                                                <tr>
                                                                    <td style="padding-left: 5px;">
                                                                        <roUserControls:roOptionPanelClient ID="opVisibilityAll" runat="server" TypeOPanel="RadioOption" width="100%" height="Auto" Checked="False" Enabled="True" CConClick="hasChanges(true, false);">
                                                                            <Title>
                                                                                <asp:Label ID="lblVisibilityAllTitle" runat="server" Text="Todos"></asp:Label>
                                                                            </Title>
                                                                            <Description>
                                                                                <asp:Label ID="lblVisibilityAllDesc" runat="server" Text="Todos los ${Employees} podrán solicitar esta ${Cause}."></asp:Label>
                                                                            </Description>
                                                                            <Content>
                                                                            </Content>
                                                                        </roUserControls:roOptionPanelClient>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td style="padding-left: 5px; padding-top: 5px">
                                                                        <roUserControls:roOptionPanelClient ID="opVisibilityNobody" runat="server" TypeOPanel="RadioOption" width="100%" height="Auto" Checked="False" Enabled="True" CConClick="hasChanges(true, false);">
                                                                            <Title>
                                                                                <asp:Label ID="lblVisibilityNobodyTitle" runat="server" Text="Nadie"></asp:Label>
                                                                            </Title>
                                                                            <Description>
                                                                                <asp:Label ID="lblVisibilityNobodyDesc" runat="server" Text="Ningún ${Employee} podrá solicitar esta ${Cause}."></asp:Label>
                                                                            </Description>
                                                                            <Content>
                                                                            </Content>
                                                                        </roUserControls:roOptionPanelClient>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td style="padding-left: 5px; padding-top: 5px">
                                                                        <roUserControls:roOptionPanelClient ID="opVisibilityCriteria" runat="server" TypeOPanel="RadioOption" width="100%" height="Auto" Checked="False" Enabled="True" CConClick="hasChanges(true, false);checkCriteriaVisibility();">
                                                                            <Title>
                                                                                <asp:Label ID="lblVisibilityCriteriaTitle" runat="server" Text="Según criterio"></asp:Label>
                                                                            </Title>
                                                                            <Description>
                                                                                <asp:Label ID="lblVisibilityCriteriaDesc" runat="server" Text="Los ${Employees} que cumplan los siguientes criterios podrán solicitar esta ${Cause}."></asp:Label>
                                                                            </Description>
                                                                            <Content>
                                                                                <table border="0" width="100%" style="padding: 20px; padding-top: 5px;" align="center">
                                                                                    <tr>
                                                                                        <td id="visibilityCell" align="left" style="display: none; padding-left: 12px;">
                                                                                            <roUserControls:roUserCtlFieldCriteria2 Prefix="ctl00_contentMainBody_ASPxCallbackPanelContenido_opVisibilityCriteria_visibilityCriteria" ID="visibilityCriteria" runat="server" />
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

                                        <!-- Quien puede utilizar las justificaciones -->
                                        <div class="panBottomMargin">
                                            <div class="panHeader2 panBottomMargin" style="width: calc(100% - 5px)">
                                                <span class="panelTitleSpan">
                                                    <asp:Label runat="server" ID="lblCanUseFrom" Text="Indica las solicitudes en las que utilizar la justificación"></asp:Label>
                                                </span>
                                            </div>
                                        </div>

                                        <div style="width: calc(99% - 9px); margin-top: 1px; padding-top: 10px; border: 1px solid #CDCDCD">
                                            <div class="OptionPanelDescStyle" style="padding-bottom: 5px">
                                                <asp:Label ID="lblWhereCanUse" runat="server" Text="Desde este apartado, podrá seleccionar en que solicitudes se puede utilizar la ${Cause} seleccionada desde el Portal del ${Employee}."></asp:Label>
                                            </div>
                                            <input type="hidden" id="hdHasLeaveDocuments" runat="server" />
                                            <table style="width: 100%;">
                                                <tr>
                                                    <td style="padding-left: 5px;">
                                                        <roUserControls:roOptionPanelClient ID="roOptAllRequestTypes" runat="server" TypeOPanel="RadioOption" width="100%" height="Auto" Checked="False" Enabled="True" CConClick="hasChanges(true, false);">
                                                            <Title>
                                                                <asp:Label ID="lblAllRequestTypes" runat="server" Text="Todas"></asp:Label>
                                                            </Title>
                                                            <Description>
                                                                <asp:Label ID="lblAllRequestTypesDesc" runat="server" Text="Todas los solicitudes en las que aplica la ${Cause} según su tipo."></asp:Label>
                                                            </Description>
                                                            <Content>
                                                            </Content>
                                                        </roUserControls:roOptionPanelClient>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="padding-left: 5px; padding-top: 5px">
                                                        <roUserControls:roOptionPanelClient ID="roOptLeaveRequest" runat="server" TypeOPanel="RadioOption" width="100%" height="Auto" Checked="False" Enabled="True" CConClick="hasChanges(true, false);">
                                                            <Title>
                                                                <asp:Label ID="lblOptLeaveRequest" runat="server" Text="Solo en bajas"></asp:Label>
                                                            </Title>
                                                            <Description>
                                                                <asp:Label ID="lblOptLeaveRequestDesc" runat="server" Text="Debido a que la solicitud requiere de documentos de tipo baja solo se puede utilizar en este tipo de solicitud."></asp:Label>
                                                            </Description>
                                                            <Content>
                                                            </Content>
                                                        </roUserControls:roOptionPanelClient>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="padding-left: 5px; padding-top: 5px">
                                                        <roUserControls:roOptionPanelClient ID="roOptNoneRequestList" runat="server" TypeOPanel="RadioOption" width="100%" height="Auto" Checked="False" Enabled="True" CConClick="hasChanges(true, false);">
                                                            <Title>
                                                                <asp:Label ID="lblOptNoneRequestList" runat="server" Text="Ninguna"></asp:Label>
                                                            </Title>
                                                            <Description>
                                                                <asp:Label ID="lblOptNoneRequestListDesc" runat="server" Text="No se podrá utilizar en ninguna solicitud."></asp:Label>
                                                            </Description>
                                                            <Content>
                                                            </Content>
                                                        </roUserControls:roOptionPanelClient>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="padding-left: 5px; padding-top: 5px">
                                                        <roUserControls:roOptionPanelClient ID="roOptCustomRequestList" runat="server" TypeOPanel="RadioOption" width="100%" height="Auto" Checked="False" Enabled="True" CConClick="hasChanges(true, false);">
                                                            <Title>
                                                                <asp:Label ID="lblOptCustomRequestList" runat="server" Text="Selección específica"></asp:Label>
                                                            </Title>
                                                            <Description>
                                                                <asp:Label ID="lblOptCustomRequestListDesc" runat="server" Text="Solo se podrá utilizar en el siguiente listado de solicitudes"></asp:Label>
                                                            </Description>
                                                            <Content>
                                                                <div id="divAvailableAbsenceRequests">
                                                                    <dx:ASPxTokenBox ID="tbAvailableAbsenceRequests" runat="server" Width="100%" AllowCustomTokens="false">
                                                                        <ClientSideEvents TextChanged="function(s,e){ hasChanges(true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                    </dx:ASPxTokenBox>
                                                                </div>
                                                                <div id="divAvailableOverWorkRequests" style="display: none">
                                                                    <dx:ASPxTokenBox ID="tbAvailableOverWorkRequests" runat="server" Width="100%" AllowCustomTokens="false">
                                                                        <ClientSideEvents TextChanged="function(s,e){ hasChanges(true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                    </dx:ASPxTokenBox>
                                                                </div>
                                                                <div id="divAvailableHolidayRequests" style="display: none">
                                                                    <dx:ASPxTokenBox ID="tbAvailableHolidayRequests" runat="server" Width="100%" AllowCustomTokens="false" o>
                                                                        <ClientSideEvents TextChanged="function(s,e){ hasChanges(true);}" GotFocus="HightlightOnGotFocus" LostFocus="FadeOnLostFocus" />
                                                                    </dx:ASPxTokenBox>
                                                                </div>
                                                            </Content>
                                                        </roUserControls:roOptionPanelClient>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </div>

                                    <%-- INCIDENCES --%>
                                    <div id="div05" class="contentPanel" style="display: none;" runat="server">

                                        <div class="panHeader2" style="width: calc(100% - 5px)">
                                            <span style="">
                                                <asp:Label runat="server" ID="lblPunchBehaviourTitle" Text="Comportamiento"></asp:Label>
                                            </span>
                                        </div>

                                        <div style="width: calc(100% - 7px); margin-top: 1px; padding-top: 10px; border: 1px solid #CDCDCD">
                                            <div class="OptionPanelDescStyle" style="padding-bottom: 5px">
                                                <asp:Label ID="lblPunchBehaviour" runat="server" Text="Desde este apartado, puede definir el comportamiento de la justificación al indicarse en un fichaje."></asp:Label>
                                            </div>
                                            <div>

                                                <table style="width: 100%;">
                                                    <tr>
                                                        <td style="padding-left: 5px;">
                                                            <roUserControls:roGroupBox ID="RoGroupBox2" runat="server" style="width: 100%">
                                                                <Content>
                                                                    <div style="float: left; width: 375px; padding-top: 5px; padding-left: 5px;">
                                                                        <asp:Label ID="lblInputCode" runat="server" Text="La ${Cause} se puede indicar en el terminal mediante el codigo:"></asp:Label>&nbsp;
                                                                    </div>
                                                                    <div style="float: left">
                                                                        <dx:ASPxTextBox ID="txtInputCode" runat="server" Width="50px" MaxLength="3" ClientInstanceName="txtInputCodeClient">
                                                                            <ClientSideEvents TextChanged="function(s,e){ hasChanges(true, false); }" />
                                                                            <MaskSettings Mask="<0..999>" IncludeLiterals="None" />
                                                                            <ValidationSettings ErrorDisplayMode="None" />
                                                                        </dx:ASPxTextBox>
                                                                    </div>
                                                                    <div style="clear: both"></div>
                                                                </Content>
                                                            </roUserControls:roGroupBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding-left: 5px;">
                                                            <roUserControls:roOptionPanelClient ID="optAbsenceMandatoryDays" runat="server" TypeOPanel="CheckboxOption" width="100%" height="Auto" Checked="False" Enabled="True" CConClick="hasChanges(true, false);">
                                                                <Title>
                                                                    <asp:Label ID="lblMandatoryDaysOnPortalRequestTitle" runat="server" Text="Esta ${Cause} tiene una duración obligatoria."></asp:Label>
                                                                </Title>
                                                                <Description>
                                                                    <asp:Label ID="lblMandatoryDaysOnPortalRequestDesc" runat="server" Text="Cuando se utilice la ${Cause} desde el portal del empleado, esta debe tener una duración de obligatoria de los días configurados."></asp:Label>
                                                                </Description>
                                                                <Content>
                                                                    <table>
                                                                        <tr>
                                                                            <td>
                                                                                <table>
                                                                                    <tr>
                                                                                        <td>
                                                                                            <asp:Label ID="lblMandatoryDaysOnPortalRequest" runat="server" Text="La ausencia tendrá una duración de" Style="padding-right: 5px; padding-left: 35px;"></asp:Label>
                                                                                        </td>
                                                                                        <td>
                                                                                            <dx:ASPxTextBox ID="txtMandatoryDaysOnPortalRequest" runat="server" Width="60px" MaxLength="4" ClientInstanceName="txtMaxsDaysAbsenceClient">
                                                                                                <ClientSideEvents TextChanged="function(s,e){ hasChanges(true, true); }" />
                                                                                                <MaskSettings Mask="<0..9999>" IncludeLiterals="None" />
                                                                                                <ValidationSettings ErrorDisplayMode="None" />
                                                                                            </dx:ASPxTextBox>
                                                                                        </td>
                                                                                        <td>
                                                                                            <asp:Label ID="lblMandatoryDaysOnPortalRequestDays" runat="server" Text="días" Style="padding-left: 5px;"></asp:Label>
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </Content>
                                                            </roUserControls:roOptionPanelClient>

                                                            <%--<roUserControls:roGroupBox ID="RoGroupBox1" runat="server" style="width: 100%">
                                                                    <Content>
                                                                        <div style="float: left; width: 530px; padding-top: 5px; padding-left: 5px;">
                                                                            <asp:Label ID="lblMandatoryDaysOnPortalRequest" runat="server" Text="Cuando se utilice la ${Cause} desde el portal del empleado, esta debe tener una duración de:"></asp:Label>&nbsp;
                                                                        </div>
                                                                        <div style="float: left">
                                                                            <dx:ASPxTextBox ID="txtMandatoryDaysOnPortalRequest" runat="server" Width="50px" MaxLength="3" ClientInstanceName="txtInputCodeClient">
                                                                                <ClientSideEvents TextChanged="function(s,e){ hasChanges(true, false); }" />
                                                                                <MaskSettings Mask="<0..999>" IncludeLiterals="None" />
                                                                                <ValidationSettings ErrorDisplayMode="None" />
                                                                            </dx:ASPxTextBox>
                                                                        </div>
                                                                        <div style="float: left; padding-top: 5px; padding-left: 5px;">
                                                                            <asp:Label ID="lblMandatoryDaysOnPortalRequest2" runat="server" Text="días"></asp:Label>&nbsp;
                                                                        </div>
                                                                        <div style="clear: both"></div>
                                                                    </Content>
                                                                </roUserControls:roGroupBox>--%>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding-left: 5px;">
                                                            <roUserControls:roOptionPanelClient ID="optCauseAbsences" runat="server" TypeOPanel="CheckboxOption" width="100%" height="Auto" Checked="False" Enabled="True" CConClick="hasChanges(true, true);">
                                                                <Title>
                                                                    <asp:Label ID="lblTypeStartAbsence" runat="server" Text="Esta ${Cause} inicia una previsión de días de ausencia."></asp:Label>
                                                                </Title>
                                                                <Description>
                                                                    <asp:Label ID="lblTypeStartAbsenceDescrip" runat="server" Text="Cuando el ${Employee} fiche, automáticamente, se abrirá una ausencia prevista de días."></asp:Label>
                                                                </Description>
                                                                <Content>
                                                                    <table>
                                                                        <tr>
                                                                            <td>
                                                                                <table>
                                                                                    <tr>
                                                                                        <td>
                                                                                            <asp:Label ID="lblTypeMaxDuration" runat="server" Text="La ausencia durará como máximo" Style="padding-right: 5px; padding-left: 35px;"></asp:Label>
                                                                                        </td>
                                                                                        <td>
                                                                                            <dx:ASPxTextBox ID="txtMaxsDaysAbsence" runat="server" Width="60px" MaxLength="4" ClientInstanceName="txtMaxsDaysAbsenceClient">
                                                                                                <ClientSideEvents TextChanged="function(s,e){ hasChanges(true, true); }" />
                                                                                                <MaskSettings Mask="<0..9999>" IncludeLiterals="None" />
                                                                                                <ValidationSettings ErrorDisplayMode="None" />
                                                                                            </dx:ASPxTextBox>
                                                                                        </td>
                                                                                        <td>
                                                                                            <asp:Label ID="lblDays" runat="server" Text="días" Style="padding-left: 5px;"></asp:Label>
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
                                                    <tr>
                                                        <td style="padding-left: 5px;">
                                                            <roUserControls:roOptionPanelClient ID="optCauseCloseAbsences" runat="server" TypeOPanel="CheckboxOption" width="100%" height="Auto" Checked="False" Enabled="True" CConClick="hasChanges(true, true);">
                                                                <Title>
                                                                    <asp:Label ID="lblTypeCauseCloseAbs" runat="server" Text="Cerrar ausencia al fichar el ${Employee}"></asp:Label>
                                                                </Title>
                                                                <Description>
                                                                    <asp:Label ID="lblTypeCauseCloseAbsDesc" runat="server" Text="Cuando fiche el ${Employee}, se cerrará la previsión de días de ausencia automáticamente."></asp:Label>
                                                                </Description>
                                                                <Content>
                                                                </Content>
                                                            </roUserControls:roOptionPanelClient>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding-left: 5px;">
                                                            <roUserControls:roOptionPanelClient ID="optCauseAutomaticValidation" runat="server" TypeOPanel="CheckboxOption" width="100%" height="Auto" Checked="False" Enabled="True" CConClick="hasChanges(true, true);">
                                                                <Title>
                                                                    <asp:Label ID="lblCauseAutomaticValidationTitle" runat="server" Text="Tiempo máximo validado automáticamente."></asp:Label>
                                                                </Title>
                                                                <Description>
                                                                    <asp:Label ID="lblCauseAutomaticValidationDesc" runat="server" Text="Al utilizar-se en un fichaje se justificará como máximo el tiempo indicado."></asp:Label>
                                                                </Description>
                                                                <Content>
                                                                    <table>
                                                                        <tr>
                                                                            <td>
                                                                                <table>
                                                                                    <tr>
                                                                                        <td>
                                                                                            <asp:Label ID="lblCauseAutomaticValidation" runat="server" Text="Hasta un máximo de" Style="padding-right: 5px; padding-left: 35px;"></asp:Label>
                                                                                        </td>
                                                                                        <td>
                                                                                            <dx:ASPxTimeEdit ID="txtCauseAutomaticValidation" runat="server" EditFormatString="HH:mm" EditFormat="Custom" ClientInstanceName="txtCauseAutomaticValidationClient">
                                                                                                <ClientSideEvents ValueChanged="function(s,e){ hasChanges(true, true); }" />
                                                                                                <ValidationSettings ErrorDisplayMode="None" />
                                                                                            </dx:ASPxTimeEdit>
                                                                                        </td>
                                                                                        <td>
                                                                                            <asp:Label ID="lblCauseAutomaticValidationHours" runat="server" Text="horas" Style="padding-left: 5px;"></asp:Label>
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
                                            </div>
                                        </div>

                                        <br />
                                        <div id="incidenceConfigurationZone" style="display: none;" runat="server">
                                            <div class="panHeader2">
                                                <span style="">
                                                    <asp:Label ID="lblTitleIncidences" Text="Incidencias" runat="server" /></span>
                                            </div>
                                            <br />
                                            <table cellpadding="2" cellspacing="2" style="width: 90%; padding-left: 20px;">
                                                <tr style="height: 48px">
                                                    <td align="center" width="60px">
                                                        <asp:Image ID="imgIncidences" ImageUrl="Images/Causes.png" runat="server" />
                                                    </td>
                                                    <td align="left" width="100%" style="padding-left: 10px;">
                                                        <asp:Label ID="lblIncidencesDesc" runat="server" Text="Desde este apartado, podrá establecer un tiempo mínimo y un tiempo máximo de las incidencias para aplicar la ${Cause} seleccionada."></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="2" valign="top" style="padding: 0px;">
                                                        <table border="0" style="width: 100%;">
                                                            <tr>
                                                                <td style="padding-top: 15px;">
                                                                    <roUserControls:roOptionPanelClient ID="opCheckIncidence" runat="server" TypeOPanel="CheckboxOption" width="100%" height="Auto" Checked="False" Enabled="True" CConClick="hasChanges(true, true);ActivateJustifyPanel();">
                                                                        <Title>
                                                                            <asp:Label ID="opCheckIncidenceTitle" runat="server" Text="Justificar según criterio de tiempo"></asp:Label>
                                                                        </Title>
                                                                        <Description>
                                                                        </Description>
                                                                        <Content>
                                                                            <table border="0" style="width: 100%;">
                                                                                <tr>
                                                                                    <td style="height: 50px;">
                                                                                        <table>
                                                                                            <tr>
                                                                                                <td>
                                                                                                    <asp:Label ID="lbloptBetween" runat="server" Text="Entre las" Style="padding-right: 8px;"></asp:Label>
                                                                                                </td>
                                                                                                <td>
                                                                                                    <dx:ASPxTextBox ID="mskJustifyPeriodsStart" runat="server" Width="50px" MaxLength="6" ClientInstanceName="mskJustifyPeriodsStartClient">
                                                                                                        <ClientSideEvents TextChanged="function(s,e){ hasChanges(true, true); }" />
                                                                                                        <MaskSettings Mask="HH:mm" />
                                                                                                        <ValidationSettings ErrorDisplayMode="None" />
                                                                                                    </dx:ASPxTextBox>
                                                                                                </td>
                                                                                                <td>
                                                                                                    <asp:Label ID="lbloptAnd" runat="server" Text="y" Style="padding-right: 5px; padding-left: 5px"></asp:Label>
                                                                                                </td>
                                                                                                <td>
                                                                                                    <dx:ASPxTextBox ID="mskJustifyPeriodsEnd" runat="server" Width="50px" MaxLength="6" ClientInstanceName="mskJustifyPeriodsEndClient">
                                                                                                        <ClientSideEvents TextChanged="function(s,e){ hasChanges(true, true); }" />
                                                                                                        <MaskSettings Mask="HH:mm" />
                                                                                                        <ValidationSettings ErrorDisplayMode="None" />
                                                                                                    </dx:ASPxTextBox>
                                                                                                </td>
                                                                                            </tr>
                                                                                        </table>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td style="padding-top: 5px; padding-bottom: 10px;">
                                                                                        <asp:Label ID="lblIfpassed" runat="server" Text="Si el tiempo de la incidencia excede el valor máximo indicado," Font-Bold="true" Style="padding-bottom: 10px;"></asp:Label>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td style="padding-bottom: 10px;">
                                                                                        <roUserControls:roOptionPanelClient ID="opJustifyNothing" runat="server" TypeOPanel="RadioOption" width="100%" height="Auto" Checked="False" Enabled="True" CConClick="hasChanges(true, true);">
                                                                                            <Title>
                                                                                                <asp:Label ID="lblJustifyNothing" runat="server" Text="No justifica nada"></asp:Label>
                                                                                            </Title>
                                                                                            <Description>
                                                                                            </Description>
                                                                                            <Content>
                                                                                            </Content>
                                                                                        </roUserControls:roOptionPanelClient>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td style="padding-bottom: 10px;">
                                                                                        <roUserControls:roOptionPanelClient ID="opJustifyPeriod" runat="server" TypeOPanel="RadioOption" width="100%" height="Auto" Checked="False" Enabled="True" CConClick="hasChanges(true, true);">
                                                                                            <Title>
                                                                                                <asp:Label ID="lblJustifyPeriod" runat="server" Text="Justificar el período"></asp:Label>
                                                                                            </Title>
                                                                                            <Description>
                                                                                            </Description>
                                                                                            <Content>
                                                                                            </Content>
                                                                                        </roUserControls:roOptionPanelClient>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </Content>
                                                                    </roUserControls:roOptionPanelClient>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                        <br />
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </div>

                                    <%-- DOCUMENTS --%>
                                    <div id="div02" class="contentPanel" style="display: none;" runat="server">
                                        <!-- Este div es el header -->
                                        <div class="panBottomMargin">
                                            <div class="panHeader2 panBottomMargin">
                                                <span class="panelTitleSpan">
                                                    <asp:Label runat="server" ID="lblCauseDocumentsTitle" Text="Documentos"></asp:Label>
                                                </span>
                                            </div>
                                            <!-- La descripción es opcional -->
                                            <div class="panelHeaderContent">
                                                <div class="panelDescriptionImage">
                                                    <img alt="" src="<%=Me.Page.ResolveUrl("~/Documents/Images/DocumentTemplate.png")%>" />
                                                </div>
                                                <div class="panelDescriptionText">
                                                    <asp:Label ID="lblCauseDocumentsDesc" runat="server" Text="Especifique los doucmentos que se requerirán en la justificación."></asp:Label>
                                                </div>
                                            </div>
                                        </div>

                                        <table style="width: 100%; height: 261px;">
                                            <tr>
                                                <td style="vertical-align: top;">
                                                    <table style="width: 100%;">
                                                        <tr>
                                                            <td style="padding: 0px;" align="right">
                                                                <!-- Barra Herramientas -->
                                                                <div id="panTbDocumentTrace" runat="server">
                                                                    <table style="width: 100%;">
                                                                        <tr>
                                                                            <td style="padding: 0px;" align="right">
                                                                                <div class="btnFlat">
                                                                                    <a href="javascript: void(0)" id="btnAddDocumentTrace" onclick="EditDocumentTrace(true, null);">
                                                                                        <asp:Label ID="lblAddDocumentTrace" runat="server" Text="Añadir Documento"></asp:Label>
                                                                                    </a>
                                                                                </div>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                    <div id="gridDocumentTrace" runat="server" style="height: 195px; overflow: auto;">
                                                        <!-- grid de documentos -->
                                                    </div>
                                                    <!-- form Compositions -->
                                                    <roForms:frmDocumentTrace ID="frmDocumentTrace1" runat="server" />
                                                </td>
                                            </tr>
                                        </table>
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
            </div>

            <!-- POPUP NEW OBJECT -->
            <dx:ASPxPopupControl ID="NewObjectPopup" runat="server" AllowDragging="False" CloseAction="None" Modal="True" ContentUrl="~/Base/Popups/CreateObjectPopup.aspx"
                PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter" Width="470px" Height="300px"
                ShowHeader="False" ScrollBars="Auto" ShowPageScrollbarWhenModal="True" ClientInstanceName="NewObjectPopup_Client" PopupAnimationType="None" BackColor="Transparent" ContentStyle-Paddings-Padding="0px" Border-BorderColor="Transparent" ShowShadow="false">
                <SettingsLoadingPanel Enabled="false" />
            </dx:ASPxPopupControl>

            <!-- POPUP CAPTCHA -->
            <dx:ASPxPopupControl ID="CaptchaObjectPopup" runat="server" AllowDragging="False" CloseAction="None" Modal="True" ContentUrl="~/Base/Popups/GenericCaptchaValidator.aspx"
                PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter" ModalBackgroundStyle-Opacity="0" Width="500" Height="320"
                ShowHeader="False" ScrollBars="Auto" ShowPageScrollbarWhenModal="True" ClientInstanceName="CaptchaObjectPopup_Client" PopupAnimationType="None" BackColor="Transparent" ContentStyle-Paddings-Padding="0px" Border-BorderColor="Transparent" ShowShadow="false">
                <SettingsLoadingPanel Enabled="false" />
            </dx:ASPxPopupControl>
        </div>
    </div>

    <script language="javascript" type="text/javascript">

        function resizeTreeCauses() {
            try {
                var ctlPrefix = "<%= roTreesCauses.ClientID %>";
                eval(ctlPrefix + "_resizeTrees();");
            }
            catch (e) {
                showError("resizeTreeCauses", e);
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
            resizeTreeCauses();
        }
    </script>
</asp:Content>