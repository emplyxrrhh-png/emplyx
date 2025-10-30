@Code
    Dim CommuniqueController As VTLive40.CommuniqueController = New VTLive40.CommuniqueController()
    Dim baseURL = Url.Content("~")
    Dim CommuniqueIcon = baseURL & "Base/Images/StartMenuIcos/Communique.png"
    Dim CommuniquesCounter = CommuniqueController.CountCommuniques()
End Code

<div id="divTabInfo" class="divDataCells">
    <div id="divTabCommuniques" class="blackRibbonTitle">
        <div class="blackRibbonIcon">
            <img src="@CommuniqueIcon" alt="" height="100" width="100">
        </div>
        <div class="blackRibbonDescription">
            <h1 id="ctl00_contentMainBody_lblHeader" class="NameText" style="margin:10px 0;font-size:20px;">@CommuniqueController.GetServerLanguage().Translate("roInternalCommunication", "Communique")</h1>
            <span id="ctl00_contentMainBody_lblInfo">@CommuniquesCounter("ActiveAmount") @CommuniqueController.GetServerLanguage().Translate("roActiveCommuniques", "Communique") @CommuniquesCounter("ActiveWeight") MB</span>
            <br>
            <span id="ctl00_contentMainBody_lblInfo">@CommuniquesCounter("ArchivedAmount") @CommuniqueController.GetServerLanguage().Translate("roArchiveCommuniques", "Communique") @CommuniquesCounter("ArchivedWeight") MB</span>
        </div>
        <div Class="switchMainViewTabs">
            @Html.Partial("switchMainViewTabs")
        </div>
    </div>
</div>