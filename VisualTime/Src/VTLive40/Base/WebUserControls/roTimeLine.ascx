<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.Base_WebUserControls_roTimeLine" CodeBehind="roTimeLine.ascx.vb" %>
<div id="<%= Me.ClientID %>_Bubble" style="top: 0; left: 0; display: none;" class="bgBubble">&nbsp;</div>
<input type="hidden" id="<%= Me.ClientID %>_TitleMandatory" value="<%= Me.Language.Translate("TimeLine.TitleMandatory", DefaultScope)%>" />
<input type="hidden" id="<%= Me.ClientID %>_TitleBreak" value="<%= Me.Language.Translate("TimeLine.TitleBreak", DefaultScope)%>" />
<input type="hidden" id="<%= Me.ClientID %>_TitleWorking" value="<%= Me.Language.Translate("TimeLine.TitleWorking", DefaultScope) %>" />
<div style="width: 850px">
    <table width="850px" border="0" cellpadding="0" cellspacing="0" style="border: solid 1px silver;">
        <tr>
            <td colspan="2" style="background-color: #0046fe78;">
                <div style="padding: 2px; padding-left: 5px; color: White;">
                    <asp:PlaceHolder ID="tlTitle" runat="server"></asp:PlaceHolder>
                </div>
            </td>
        </tr>
        <tr>
            <td style="padding-bottom: 35px; border-right: solid 1px silver; min-width: 128px" id="<%= Me.ClientID %>_TLHeaders"></td>
            <td style="width: 100%;" valign="top">
                <div id="<%= Me.ClientID %>_divTLScroll" class="tlScroll" style="width: 850px;">
                    <table id="<%= Me.ClientID %>_tableTL" width="1368px" border="0" cellpadding="0" cellspacing="0">
                        <tr>
                            <td style="width: 100%; height: 15px; background-color: #efefef;" class="bgTimeLineNumbers"></td>
                        </tr>
                        <tr>
                            <td></td>
                        </tr>
                    </table>
                </div>
            </td>
        </tr>
    </table>
</div>