<%@ Control Language="VB" AutoEventWireup="false" Inherits="VTLive40.Base_WebUserControls_roMsgBoxContentV2" CodeBehind="roMsgBoxContentV2.ascx.vb" %>
<div id="popupBody" runat="server" class="bodyPopupExtended">
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
    <div class="panBottomMargin popupBreakLine">
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