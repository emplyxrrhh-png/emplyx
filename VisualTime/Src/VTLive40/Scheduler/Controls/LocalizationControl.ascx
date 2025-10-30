<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.Scheduler_Controls_LocalizationControl" CodeBehind="LocalizationControl.ascx.vb" %>
<%@ Register Src="LocalizationMapControl.ascx" TagName="LocalizationMapControl" TagPrefix="uc1" %>
<link href="/Base/Styles/PopupFrame.css" rel="stylesheet" type="text/css" />

<table id="localization-content" style="height: 100%; vertical-align: top; width: 97%; text-align: left;"
    cellpadding="0" cellspacing="0">
    <tr valign="top" style="height: 24px;">
        <td colspan="2" valign="top">
            <div class="panHeader2">
                <div style="float: left; text-align: left; padding-right: 10px; margin-top: 5px;">
                    <asp:Label ID="lblLocalization" Text="Localización" runat="server" CssClass="panHeaderLabel" Style="margin-top: -5px;" />
                </div>
            </div>
        </td>
    </tr>
    <tr>
        <td colspan="2" style="padding-left: 10px; padding-top: 5px; padding-right: 10px;" valign="top">
            <uc1:LocalizationMapControl ID="LocalizationMapControl1" runat="server" />
        </td>
    </tr>
</table>