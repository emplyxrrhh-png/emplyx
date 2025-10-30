<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="DocumentEmployees.ascx.vb" Inherits="VTLive40.DocumentEmployees" %>

<script type="text/javascript">
    var <%= Me.ClientID %>_type = null;
    var <%= Me.ClientID %>_idType = null;
    var <%= Me.ClientID %>_commandCallback = false;

    function <%= Me.ClientID %>_GridDocsClient_CustomButtonClick(s, e) {
        <%= Me.ClientID %>_commandCallback = true;
        if (e.buttonID == "EditDocument") {
            <%= Me.ClientID %>_GridDocsClient.GetRowValues(e.visibleIndex, 'Id',  <%= Me.ClientID %>_EditDocumentRow);
        } else if (e.buttonID == "ViewDocument") {
            <%= Me.ClientID %>_GridDocsClient.GetRowValues(e.visibleIndex, 'Id',  <%= Me.ClientID %>_ViewDocument);
        } else if (e.buttonID == "ViewSign") {
            <%= Me.ClientID %>_GridDocsClient.GetRowValues(e.visibleIndex, 'Id',  <%= Me.ClientID %>_ViewSign);
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

    function  <%= Me.ClientID %>_ViewSign(idDocument) {
        var url = 'DocumentVisualize.aspx?DeliveredSign=' + idDocument;
        var Title = 'Firma';
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

    function <%= Me.ClientID %>_DownloadAll() {
        var keys = <%= Me.ClientID %>_GridDocsClient.GetSelectedKeysOnPage();

        if (keys.toString() != "") {
            var url = 'DocumentVisualize.aspx?DeliveredDocument=' + keys.toString();
            var Title = 'Documentos';
            window.open(url);
        }

    }
</script>

<div class="jsGrid" style="">
    <div class="divRow" style="margin-left: 25px; margin-right: 25px;">
        <asp:Label ID="lblDeliveredDocuments" runat="server" CssClass="jsGridTitle" Text="Documentos Entregados"></asp:Label>
        <div class="jsgridButton">

            <div style="float: right">
                <div style="float: left; padding-right: 10px; padding-top: 15px;">
                    <dx:ASPxButton ID="btnExportToXls" runat="server" CausesValidation="False" Text="Exportar" ToolTip="Exportar a Excel" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat">
                        <Image Url="~/Base/Images/Grid/ExportToExcel16.png"></Image>
                    </dx:ASPxButton>

                    <dx:ASPxButton ID="btnAddDoc" runat="server" AutoPostBack="False" CausesValidation="False" Text="Añadir" ToolTip="Añadir nuevo documento" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat">
                        <Image Url="~/Base/Images/Grid/add.png"></Image>
                    </dx:ASPxButton>

                    <dx:ASPxButton ID="btnDownloadAll" runat="server" AutoPostBack="False" CausesValidation="False" Text="Descargar seleccionados" ToolTip="Descargar seleccionados" HoverStyle-CssClass="btnFlat-hover" CssClass="btnFlat">
                        <Image Url="~/Base/Images/Grid/button_save.png"></Image>
                    </dx:ASPxButton>
                </div>
            </div>
        </div>
    </div>
</div>

<div class="jsGridContent">
    <div class="divRow" style="margin-left: 0px; width: 100%;">
        <dx:ASPxHiddenField ID="hdnScopeInfo" runat="server"></dx:ASPxHiddenField>
        <dx:ASPxHiddenField ID="hdnEmployeeDocumentsConfig" runat="server" ClientInstanceName="hdnEmployeeDocumentsConfigClient"></dx:ASPxHiddenField>
        <dx:ASPxGridView ID="GridDeliveredDocs" runat="server" AutoGenerateColumns="False" KeyboardSupport="True" Width="100%">
            <Toolbars>
                <%--            <dx:GridViewToolbar ItemAlign="Right">
                <SettingsAdaptivity Enabled="true" EnableCollapseRootItemsToIcons="true" />
                <Items>
                    <dx:GridViewToolbarItem Command="ExportToPdf" Text="Exportar a PDF" />
                    <dx:GridViewToolbarItem Command="ExportToXls" Text="Exportar a Excel" />
                    <dx:GridViewToolbarItem Command="ExportToDocx" Text="Exportar a Word" />
                </Items>
            </dx:GridViewToolbar>--%>
            </Toolbars>
            <SettingsLoadingPanel Text="Cargando&amp;hellip;"></SettingsLoadingPanel>
            <SettingsFilterControl ViewMode="Visual" ShowAllDataSourceColumns="True" MaxHierarchyDepth="1" />
            <Settings ShowTitlePanel="False" VerticalScrollBarMode="Auto" UseFixedTableLayout="True" VerticalScrollableHeight="150" />
            <SettingsPager Mode="ShowAllRecords" ShowEmptyDataRows="false" />
            <Settings ShowFilterBar="Visible" />
            <SettingsExport EnableClientSideExportAPI="true" ExcelExportMode="WYSIWYG" />
            <SettingsCommandButton>
                <DeleteButton Image-Url="~/Base/Images/Grid/remove.png" Image-ToolTip="" />
                <UpdateButton Image-Url="~/Base/Images/Grid/save.png" Image-ToolTip="" />
                <CancelButton Image-Url="~/Base/Images/Grid/cancel.png" Image-ToolTip="" />
                <EditButton Image-Url="~/Base/Images/Grid/edit.png" Image-ToolTip="" />
            </SettingsCommandButton>
            <Styles>
                <Header CssClass="jsGridHeaderCell" />
                <Cell Wrap="False" />
            </Styles>
        </dx:ASPxGridView>
        <dx:ASPxGridViewExporter ID="GridExporter" runat="server" GridViewID="GridDeliveredDocs" />
    </div>
</div>

<!-- POPUP NEW OBJECT -->
<dx:ASPxPopupControl ID="EditDocumentPopup" runat="server" AllowDragging="False" CloseAction="None" Modal="True" ContentUrl="~/Employees/EditDocument.aspx"
    PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter" Width="900px" Height="600px"
    ShowHeader="False" ScrollBars="Auto" ShowPageScrollbarWhenModal="True" PopupAnimationType="None" BackColor="Transparent" ContentStyle-Paddings-Padding="0px" Border-BorderColor="Transparent" ShowShadow="false">
    <SettingsLoadingPanel Enabled="false" />
</dx:ASPxPopupControl>

<!-- POPUP NEW OBJECT -->
<dx:ASPxPopupControl ID="NewDocumentPopup" runat="server" AllowDragging="False" CloseAction="None" Modal="True" ContentUrl="~/Employees/DocumentUpload.aspx"
    PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter" Width="540px" Height="350px"
    ShowHeader="False" ScrollBars="Auto" ShowPageScrollbarWhenModal="True" PopupAnimationType="None" BackColor="Transparent" ContentStyle-Paddings-Padding="0px" Border-BorderColor="Transparent" ShowShadow="false">
    <SettingsLoadingPanel Enabled="false" />
</dx:ASPxPopupControl>

<!-- POPUP NEW OBJECT -->
<dx:ASPxPopupControl ID="CaptchaObjectPopup" runat="server" AllowDragging="False" CloseAction="None" Modal="True" ContentUrl="~/Base/Popups/GenericCaptchaValidator.aspx"
    PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter" ModalBackgroundStyle-Opacity="0" Width="500" Height="320"
    ShowHeader="False" ScrollBars="Auto" ShowPageScrollbarWhenModal="True" PopupAnimationType="None" BackColor="Transparent" ContentStyle-Paddings-Padding="0px" Border-BorderColor="Transparent" ShowShadow="false">
    <SettingsLoadingPanel Enabled="false" />
</dx:ASPxPopupControl>