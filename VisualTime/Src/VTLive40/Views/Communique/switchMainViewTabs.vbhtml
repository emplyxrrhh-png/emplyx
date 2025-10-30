@Code
    Dim CommuniqueController As VTLive40.CommuniqueController = New VTLive40.CommuniqueController()
End Code

<a Class='mainActionBtn viewTab activeTab' id='filterActiveCommBtn' href="#" title="@CommuniqueController.GetServerLanguage().Translate("roSeeActiveCommuniques", "Communique")">
    @CommuniqueController.GetServerLanguage().Translate("roActives", "Communique")
</a>
<a Class='mainActionBtn viewTab' id='filterArchiveCommBtn' href="#" title="@CommuniqueController.GetServerLanguage().Translate("roSeeArchivedCommuniques", "Communique")">
    @CommuniqueController.GetServerLanguage().Translate("roArchives", "Communique")
</a>