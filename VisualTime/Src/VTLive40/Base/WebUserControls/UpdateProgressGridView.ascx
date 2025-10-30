<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.WebUserControls_UpdateProgressGridView" CodeBehind="UpdateProgressGridView.ascx.vb" %>

<roWebControls:roPopupFrameV2 ID="UpdateProgressContentFrame" runat="server" Width="200px" Height="50px">
    <FrameContentTemplate>
        <div align="center" style="margin-top: 13px; width: 185px; height: 35px">
            <asp:Image ID="imgLoading" ImageUrl="~/Base/Images/Progress/Loading.gif" runat="server" />
            <%-- <span class="UpdateProgressMessage">Cargando ...</span> --%>
            <asp:Label ID="lblUpdateProgressMessage" Text="${Loading.Message}" CssClass="UpdateProgressMessage" runat="server" />
        </div>
    </FrameContentTemplate>
</roWebControls:roPopupFrameV2>