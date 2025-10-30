<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.roOptionPanelContainer" CodeBehind="roOptionPanelContainer.ascx.vb" %>
<%@ Register Src="roOptionPanel.ascx" TagName="roOptionPanel" TagPrefix="uc1" %>

<div id="tblContainer" class="optionPanelRoboticsV2 innerMargin" style="width: 300px; height: 100px" onmouseover="this.className='optionPanelRoboticsV2-hover innerMargin';" onmouseout="this.className='optionPanelRoboticsV2 innerMargin';" runat="server">
    <uc1:roOptionPanel ID="OptionPanel1" EnableViewState="true" runat="server" />
    <asp:Panel ID="Panell" runat="server">
        <asp:PlaceHolder ID="externalContent" runat="server" />
    </asp:Panel>
</div>