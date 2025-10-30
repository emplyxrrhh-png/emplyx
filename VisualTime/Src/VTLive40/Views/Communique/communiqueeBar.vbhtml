@Code
    Dim CommuniqueController As VTLive40.CommuniqueController = New VTLive40.CommuniqueController()
    Dim baseURL = Url.Content("~")
    Dim CommuniqueIcon = baseURL & "Base/Images/StartMenuIcos/Communique.png"
    Dim CommuniquesCounter = CommuniqueController.CountCommuniques()
End Code

<div class="aParent" style="float:left;width:80%;">
    <div id="communiqueeConfiguration" class="bTabCommuniquees bTabCommuniquees-active" style="height:20px !important; cursor: pointer;">
        @CommuniqueController.GetServerLanguage().Translate("roconfiguracion", "Communique")
    </div>
    <div id="communiqueeDesign" class="bTabCommuniquees" style="height: 20px !important; cursor: pointer;">
        @CommuniqueController.GetServerLanguage().Translate("rodesign", "Communique")
    </div>
    <div id="communiqueeResults" class="bTabCommuniquees" style="height: 20px !important; cursor: pointer;">
        @CommuniqueController.GetServerLanguage().Translate("roresults", "Communique")
    </div>
</div>