<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.Base_Popups_roMessageBoxV2" CodeBehind="roMessageBoxV2.ascx.vb" %>

<dx:ASPxPopupControl ID="ObjectPopup" runat="server" AllowDragging="False" CloseAction="None" Modal="True" PopupVerticalAlign="WindowCenter" PopupHorizontalAlign="WindowCenter"
    ShowHeader="False" ScrollBars="None" ShowPageScrollbarWhenModal="false" PopupAnimationType="Fade" BackColor="Transparent" ContentStyle-Paddings-Padding="0px" Border-BorderColor="Transparent" ShowShadow="false">
    <SettingsLoadingPanel Enabled="false" />
    <ContentCollection>
        <dx:PopupControlContentControl>
            <div class="bodyPopupExtended">
                <!-- Titol -->
                <div class="panBottomMargin">
                    <div class="popupHeaderContent">
                        <div class="popupDescriptionImage">
                            <img id="imgTop" runat="server" alt="" src="~/Base/Images/MessageFrame/dialog-information.png" />
                        </div>
                        <div class="popupDescriptionText">
                            <span id="lblTop" runat="server" class="lblTitle"></span>
                        </div>
                    </div>
                </div>
                <!-- Nota AVISO -->
                <div id="divAviso" style="display: none;" class="panBottomMargin" runat="server">
                    <div class="popupHeaderContent">
                        <div class="popupDescriptionImage">
                            <img alt="" src="~/Base/Images/MessageFrame/Alert32.png" id="imgAviso" runat="server" style="padding: 3px;" />
                        </div>
                        <div class="popupDescriptionText">
                            <span id="lblAviso" runat="server"></span>
                        </div>
                    </div>
                </div>
                <div class="panBottomMargin">
                    <span id="lblDescription" runat="server"></span>
                </div>
                <div>
                    <a href="javascript: void(0);" id="btnOption1" runat="server" class="btnMsgBox" onclick="">
                        <b><span id="lblOption1Text" runat="server"></span></b>
                        <br />
                        <span id="lblOption1Description" runat="server"></span>
                    </a>

                    <a href="javascript: void(0);" id="btnOption2" runat="server" onclick="HideMsgBoxForm();" class="btnMsgBox">
                        <b><span id="lblOption2Text" runat="server"></span></b>
                        <br />
                        <span id="lblOption2Description" runat="server"></span>
                    </a>

                    <a href="javascript: void(0);" id="btnOption3" runat="server" onclick="HideMsgBoxForm();" class="btnMsgBox">
                        <b><span id="lblOption3Text" runat="server"></span></b>
                        <br />
                        <span id="lblOption3Description" runat="server"></span>
                    </a>

                    <a href="javascript: void(0);" id="btnOption4" runat="server" onclick="HideMsgBoxForm();" class="btnMsgBox" visible="false">
                        <b><span id="lblOption4Text" runat="server"></span></b>
                        <br />
                        <span id="lblOption4Description" runat="server"></span>
                    </a>
                </div>
            </div>
        </dx:PopupControlContentControl>
    </ContentCollection>
</dx:ASPxPopupControl>