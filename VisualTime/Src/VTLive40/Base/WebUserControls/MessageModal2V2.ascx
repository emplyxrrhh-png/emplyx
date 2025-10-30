<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.Base_WebUserControls_MessageModal2V2" CodeBehind="MessageModal2V2.ascx.vb" %>

<script language="javascript" type="text/javascript">

    function HideMessage() {
        if ($get('<%= hdnCloseClient.ClientID %>').value == '1')
            hidePopup('MessageModal2PopupFormBehavior');
    }
    function EventServer(option) {
        var EventServer = $get('<%= hdnEventServer.ClientID %>').value;
        if (EventServer == '0')
            return false;
        else
            return true;
    }

    function HideOptionMessage(option) {
        var CloseClient;
        switch (option) {
            case 1:
                {
                    CloseClient = $get('<%= hdnOption1CloseClient.ClientID %>').value;
                    break;
                }
            case 2:
                {
                    CloseClient = $get('<%= hdnOption2CloseClient.ClientID %>').value;
                    break;
                }
            case 3:
                {
                    CloseClient = $get('<%= hdnOption3CloseClient.ClientID %>').value;
                    break;
                }
            case 4:
                {
                    CloseClient = $get('<%= hdnOption4CloseClient.ClientID %>').value;
                    break;
                }
        }
        if (CloseClient == '1')
            hidePopup('MessageModal2PopupFormBehavior');
    }

    function OptionEventServer(option) {
        var EventServer;
        switch (option) {
            case 1:
                {
                    EventServer = $get('<%= hdnOption1EventServer.ClientID %>').value;
                    break;
                }
            case 2:
                {
                    EventServer = $get('<%= hdnOption2EventServer.ClientID %>').value;
                    break;
                }
            case 3:
                {
                    EventServer = $get('<%= hdnOption3EventServer.ClientID %>').value;
                    break;
                }
            case 4:
                {
                    EventServer = $get('<%= hdnOption4EventServer.ClientID %>').value;
                    break;
                }
        }
        if (EventServer == '0')
            return false;
        else
            return true;
    }
</script>

<asp:Button ID="MyHideButton" runat="server" Text="Button" Style="display: none;" />

<ajaxToolkit:ModalPopupExtender ID="MyModalPopupExtender" runat="server" DropShadow="false" TargetControlID="MyHideButton"
    PopupControlID="MyPopupFrame_DIV" BehaviorID="MessageModal2PopupFormBehavior">
</ajaxToolkit:ModalPopupExtender>

<div id="MyPopupFrame_DIV" runat="server" style='display: none;'>
    <Local:roMsgBoxContent ID="MsgBoxContent" runat="server" />
</div>

<asp:HiddenField ID="hdnAcceptButtonKey" runat="server" Value="" />
<asp:HiddenField ID="hdnOption1Key" runat="server" Value="" />
<asp:HiddenField ID="hdnOption1CloseClient" runat="server" Value="1" />
<asp:HiddenField ID="hdnOption1EventServer" runat="server" Value="1" />
<asp:HiddenField ID="hdnOption2Key" runat="server" Value="" />
<asp:HiddenField ID="hdnOption2CloseClient" runat="server" Value="1" />
<asp:HiddenField ID="hdnOption2EventServer" runat="server" Value="1" />
<asp:HiddenField ID="hdnOption3Key" runat="server" Value="" />
<asp:HiddenField ID="hdnOption3CloseClient" runat="server" Value="1" />
<asp:HiddenField ID="hdnOption3EventServer" runat="server" Value="1" />
<asp:HiddenField ID="hdnOption4Key" runat="server" Value="" />
<asp:HiddenField ID="hdnOption4CloseClient" runat="server" Value="1" />
<asp:HiddenField ID="hdnOption4EventServer" runat="server" Value="1" />
<asp:HiddenField ID="hdnCloseClient" runat="server" Value="1" />
<asp:HiddenField ID="hdnEventServer" runat="server" Value="0" />