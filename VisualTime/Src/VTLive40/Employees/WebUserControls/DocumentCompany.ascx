<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="DocumentCompany.ascx.vb" Inherits="VTLive40.DocumentCompany" %>

<script type="text/javascript">
    var <%= Me.ClientID %>_type = null;
    var <%= Me.ClientID %>_idType = null;
    var <%= Me.ClientID %>_commandCallback = false;

    //============ SELECTOR EMPLEADOS =================================
    function PopupSelectorEmployeesClient_PopUp(s, e) {
        try {
            s.SetHeaderText("");
            var iFrm = document.getElementById('<%= GroupSelectorFrame.ClientID %>');

            var strBase = '<%= Me.Page.ResolveURL("~/Base/") %>' + "WebUserControls/roWizardSelectorContainerMultiSelectV3.aspx?" +
                "PrefixTree=treeEmp&FeatureAlias=Employee&PrefixCookie=objContainerTreeV3_treeEmpGrid&" +
                "AfterSelectFuncion=parent.GetSelectedTreeV3";
            iFrm.src = strBase;
        }
        catch (e) {
            showError("PopupSelectorEmployeesClient_PopUp", e);
        }
    }
    //==================================================================

    function <%= Me.ClientID %>_GridDocsClient_CustomButtonClick(s, e) {
        <%= Me.ClientID %>_commandCallback = true;
        if (e.buttonID == "EditDocument") {
            <%= Me.ClientID %>_GridDocsClient.GetRowValues(e.visibleIndex, 'Id',  <%= Me.ClientID %>_EditDocumentRow);
        } else if (e.buttonID == "ViewDocument") {
            <%= Me.ClientID %>_GridDocsClient.GetRowValues(e.visibleIndex, 'Id',  <%= Me.ClientID %>_ViewDocument);
        } else if (e.buttonID == "DeleteDocument") {
            <%= Me.ClientID %>_GridDocsClient.GetRowValues(e.visibleIndex, 'Id',  <%= Me.ClientID %>_DeleteDocument);
        }
    }

    function <%= Me.ClientID %>_GridDocsClient_BeginCallback(e, c) {
    }

    function <%= Me.ClientID %>_GridDocsClient_EndCallback(s, e) {
        showLoader(false);
    }

    function  <%= Me.ClientID %>_EditDocumentRow(IdDocument) {
        var controlerId = "<%= Me.ClientID %>";
        <%= Me.ClientID %>_type =  <%= Me.ClientID %>_hdnScopeInfoClient.Get("Type");
        <%= Me.ClientID %>_idRelatedObject =  <%= Me.ClientID %>_hdnScopeInfoClient.Get("IdRelatedObject");
        var <%= Me.ClientID %>_url = '../Employees/EditDocument.aspx?IdRelatedObject=' + <%= Me.ClientID %>_idRelatedObject + '&IdDocument=' + IdDocument + '&Scope=' + <%= Me.ClientID %>_type + "&ClientId=" + controlerId;
        var Title = '';

        <%= Me.ClientID %>_EditDocumentPopup.SetContentUrl(<%= Me.ClientID %>_url);
        <%= Me.ClientID %>_EditDocumentPopup.Show()
    }
    function  <%= Me.ClientID %>_ViewDocument(idDocument) {
        var url = 'DocumentVisualize.aspx?DeliveredDocument=' + idDocument;
        var Title = 'Documentos';
        window.open(url);

    }

    function <%= Me.ClientID %>_showCaptcha() {
        var contentUrl = "../Base/Popups/GenericCaptchaValidator.aspx?Action=DELETEDOCUMENT&PopupName=<%= Me.ClientID %>_CaptchaObjectPopup&CallbackName=<%= Me.ClientID %>_CaptchaCallback";

        var captchapopup = eval("<%= Me.ClientID %>_CaptchaObjectPopup");

        captchapopup.SetContentUrl(contentUrl);
        captchapopup.Show();
    }

    function <%= Me.ClientID %>_CaptchaCallback(action) {
        switch (action) {
            case "DELETEDOCUMENT":
                <%= Me.ClientID %>_PerformAction();
                <%--var captchapopup = eval("<%= Me.ClientID %>_CaptchaObjectPopup");
                captchapopup.SetContentUrl('');
                captchapopup.Hide();--%>
                break;
            case "CLOSE":
                <%--var captchapopup = eval("<%= Me.ClientID %>_CaptchaObjectPopup");
                captchapopup.SetContentUrl('');
                captchapopup.Hide();--%>
                break;
            case "ERROR":
                window.parent.frames["ifPrincipal"].showErrorPopup("Error.ValidationFailed", "ERROR", "Error.ValidationFailedDesc", "Error.OK", "Error.OKDesc", "");
                break;
        }
    }

    var selectedDocument = -1

    function  <%= Me.ClientID %>_DeleteDocument(idDocument) {
        selectedDocument = idDocument;
        <%= Me.ClientID %>_showCaptcha();
    }
    function <%= Me.ClientID %>_PerformAction() {
        showLoader(true);
        <%= Me.ClientID %>_GridDocsClient.PerformCallback("DELETE;" + selectedDocument);
        var pendingDocName = "<%= Me.ClientID %>";
        var pendingControl = pendingDocName.replace("DocumentEmployees", "DocumentPendingManagment");
        var gridPending = pendingControl + "_CallbackSessionDocPenClient";
        window[gridPending].PerformCallback("RELOAD");

    }

    function <%= Me.ClientID %>_AddNewDocument(idRelatedObject, IdDocument, scope, IdAbsence, eForecast) {
        var controlerId = "<%= Me.ClientID %>";
        var url = "../Employees/DocumentUpload.aspx?IdRelatedObject=" + idRelatedObject + "&IdDocument=" + IdDocument + "&Scope=" + scope + "&ClientId=" + controlerId + "&IdAbsence=" + IdAbsence + "&ForecatType=" + eForecast;
        var Title = '';

        <%= Me.ClientID %>_NewDocumentPopup.SetContentUrl(url);
        <%= Me.ClientID %>_NewDocumentPopup.Show()
    }

    function <%= Me.ClientID %>_Download() {
        var keys = <%= Me.ClientID %>_divValuesGrid.getSelectedRowKeys();
        if (keys.toString() != "") {
            var url = 'DocumentVisualize.aspx?DeliveredDocument=' + keys.toString();
            var Title = 'Documentos';
            window.open(url);
        }
    }
</script>
<dx:ASPxCallbackPanel ID="CallbackSessionP" runat="server" ClientInstanceName="CallbackSessionClient">
    <SettingsLoadingPanel Enabled="false" />
    <ClientSideEvents EndCallback="ASPxCallbackSessionPClient_EndCallBack" />
    <PanelCollection>

        <dx:PanelContent ID="PanelContent1" runat="server">
            <asp:HiddenField ID="hdnEmployeesSelected" runat="server" Value="0" />
            <asp:HiddenField ID="hdnEmployees" runat="server" Value="" />
            <asp:HiddenField ID="hdnFilter" runat="server" Value="" />
            <asp:HiddenField ID="hdnFilterUser" runat="server" Value="" />

            <div style="padding-right: 10px; padding-top: 5px; float: left;">
                <dx:ASPxButton ID="btnOpenPopupSelectorEmployees" runat="server" AutoPostBack="False" CausesValidation="False" Text="Seleccionar" ToolTip="Empleados..." HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat">
                    <Image Url="~/Scheduler/Images/EmployeeSelector16.png"></Image>
                    <ClientSideEvents Click="btnOpenPopupSelectorEmployeesClient_Click" />
                </dx:ASPxButton>
            </div>
        </dx:PanelContent>
    </PanelCollection>
</dx:ASPxCallbackPanel>
<div class="jsGrid" style="">
    <div class="divRow" style="margin-left: 25px; margin-right: 25px;">
        <asp:Label ID="lblDeliveredDocuments" runat="server" CssClass="jsGridTitle" Text="Documentos Entregados"></asp:Label>
        <div class="jsgridButton">

            <div style="float: right">
                <div style="float: left; padding-right: 10px; padding-top: 15px;">
                    <dx:ASPxButton ID="btnDownloadAll" runat="server" AutoPostBack="False" CausesValidation="False" Text="Descargar seleccionados" ToolTip="Descargar seleccionados" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat">
                        <Image Url="~/Base/Images/Grid/button_save.png"></Image>
                        <ClientSideEvents Click="btnDownloadClient_Click" />
                    </dx:ASPxButton>
                </div>
            </div>
        </div>
    </div>
</div>

<div id="divValuesGrid" runat="server" class="jsGridContent dextremeGrid">
    <!-- Carrega del Grid Usuari General -->
</div>

<!-- POPUP DEL SELECTOR DE EMPLEADOS -->
<dx:ASPxPopupControl ID="PopupSelectorEmployees" runat="server" AllowDragging="True" CloseAction="None" Modal="True" ClientInstanceName="PopupSelectorEmployeesClient" ClientSideEvents-PopUp="PopupSelectorEmployeesClient_PopUp"
    PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter" Height="560px" Width="830px"
    ShowHeader="False" ScrollBars="Auto" ShowPageScrollbarWhenModal="True" PopupAnimationType="None" BackColor="Transparent" ContentStyle-Paddings-Padding="0px" Border-BorderColor="Transparent" ShowShadow="false">
    <ContentCollection>
        <dx:PopupControlContentControl ID="PopupControlContentControl1" runat="server">
            <dx:ASPxPanel ID="ASPxPanel3" runat="server" Width="0px" Height="0px">
                <PanelCollection>
                    <dx:PanelContent ID="PanelContent3" runat="server">
                        <div class="bodyPopupExtended" style="table-layout: fixed; height: 510px; width: 775px;">
                            <div style="width: 100%;">
                                <div class="panHeader2" style="margin-right: 20px !important;">
                                    <span style="">
                                        <asp:Label ID="lblTitle" runat="server" Text="Seleccionar"></asp:Label></span>
                                </div>
                            </div>
                            <table id="tbPopupFrame" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td valign="top">
                                        <iframe id="GroupSelectorFrame" runat="server" style="background-color: Transparent;" height="420" width="775" scrolling="no"
                                            frameborder="0" marginheight="0" marginwidth="0" src="" />
                                    </td>
                                </tr>
                                <tr style="height: 35px;">
                                    <td align="right">
                                        <table>
                                            <tr>
                                                <td>
                                                    <dx:ASPxButton ID="btnPopupSelectorEmployeesAccept" runat="server" AutoPostBack="False" CausesValidation="False" Text="${Button.Accept}" ToolTip="${Button.Accept}" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                                        <ClientSideEvents Click="btnPopupSelectorEmployeesAcceptClient_Click" />
                                                    </dx:ASPxButton>
                                                </td>
                                                <td>
                                                    <dx:ASPxButton ID="btCancel" Text="${Button.Cancel}" AutoPostBack="False" runat="server" HoverStyle-CssClass="btnFlat-hover btnFlatBlack-hover" CssClass="btnFlat btnFlatBlack">
                                                        <ClientSideEvents Click="btnPopupSelectorEmployeesCancelClient_Click" />
                                                    </dx:ASPxButton>
                                                </td>
                                            </tr>
                                        </table>
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