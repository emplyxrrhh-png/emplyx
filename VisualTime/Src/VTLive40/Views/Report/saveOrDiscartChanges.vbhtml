@Code
    Dim CommuniqueController As VTLive40.CommuniqueController = New VTLive40.CommuniqueController()
End Code
<div class="saveOrDiscartDiv" style="display:none">
    <span>@CommuniqueController.GetServerLanguage().Translate("roChangesFound", "Communique")</span>
    <span>
        <a Class='editingActionBtn commSaveBtn' href="#" title="@CommuniqueController.GetServerLanguage().Translate("roSaveChanges", "Communique")">
            @CommuniqueController.GetServerLanguage().Translate("roSaveChanges", "Communique")
        </a>
        <span>  |  </span>
        <a Class='editingActionBtn commDiscartBtn' href="#" title="@CommuniqueController.GetServerLanguage().Translate("roDiscartCommunique", "Communique")">
            @CommuniqueController.GetServerLanguage().Translate("roDiscartCommunique", "Communique")
        </a>
    </span>
</div>