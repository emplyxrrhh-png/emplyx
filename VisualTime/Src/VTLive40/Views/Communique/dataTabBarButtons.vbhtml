@imports Robotics.Web.Base.API

@Code
    Dim CommuniqueController As VTLive40.CommuniqueController = New VTLive40.CommuniqueController()
    Dim reportManagerPermissionsByUser = SecurityServiceMethods.GetPermissionOverFeature(Nothing, "Communique", "U")
    Dim baseURL = Url.Content("~")
    Dim CommuniqueIcon = baseURL & "Base/Images/StartMenuIcos/Communique.png"
    Dim ShrinkIcon = baseURL & "Base/Images/tbt/maximize32.png"
    Dim ExpandIcon = baseURL & "Base/Images/tbt/minimize32.png"
    Dim NewIcon = baseURL & "Base/Images/tbt/new32.png"
    Dim DeleteIcon = baseURL & "Base/Images/tbt/delete32.png"
    Dim SendIcon = baseURL & "Base/Images/tbt/employeeMessage32.png"
    Dim ArchiveIcon = baseURL & "Base/Images/tbt/archive32.png"
    Dim UnarchiveIcon = baseURL & "Base/Images/tbt/archive32.png"
    Dim CancelIcon = baseURL & "Base/Images/tbt/cancel.png"
    Dim UncancelIcon = baseURL & "Base/Images/tbt/uncancel.png"
End Code

<div id="divBarButtons" class="maxHeight">
    <div id="divBarButtons" class="maxHeight">
        <div class="middleBarButtonsMain">
            <div style="height:50%;vertical-align:top;">
                <a class='mainActionBtn' id='commShrinkBtn' href="#" title="@CommuniqueController.GetServerLanguage().Translate("roHideCommuniques", "Communique")">
                    <img src="@ShrinkIcon" alt="@CommuniqueController.GetServerLanguage().Translate("roHideCommuniques", "Communique")." />
                </a>
                <a class='mainActionBtn' id='commExpandBtn' href="#" title="@CommuniqueController.GetServerLanguage().Translate("roShowCommuniques", "Communique")" style="display:none">
                    <img src="@ExpandIcon" alt="@CommuniqueController.GetServerLanguage().Translate("roShowCommuniques", "Communique")." />
                </a>
                @If reportManagerPermissionsByUser >= 9 OrElse True Then
                @<a Class='mainActionBtn' id='commCreateBtn' href="#" title="@CommuniqueController.GetServerLanguage().Translate("roCreateCommunique", "Communique")">
                    <img src="@NewIcon" alt="@CommuniqueController.GetServerLanguage().Translate("roCreateCommunique", "Communique")." />
                </a>
                @<a Class='mainActionBtn' id='commArchiveBtn' href="#" title="@CommuniqueController.GetServerLanguage().Translate("roArchiveCommunique", "Communique")" style="display:none">
                    <img src="@ArchiveIcon" alt="@CommuniqueController.GetServerLanguage().Translate("roArchiveCommunique", "Communique")." />
                </a>
                @<a Class='mainActionBtn' id='commUnarchiveBtn' href="#" title="@CommuniqueController.GetServerLanguage().Translate("roUnArchiveCommunique", "Communique")" style="display:none">
                    <img src="@UnarchiveIcon" alt="@CommuniqueController.GetServerLanguage().Translate("roUnArchiveCommunique", "Communique")." />
                </a>
                @<a Class='mainActionBtn' id='commRemoveBtn' href="#" title="@CommuniqueController.GetServerLanguage().Translate("roRemoveCommunique", "Communique")" style="display:none">
                    <img src="@DeleteIcon" alt="@CommuniqueController.GetServerLanguage().Translate("roRemoveCommunique", "Communique")." />
                </a>
                @<a Class='mainActionBtn' id='commSendBtn' href="#" title="@CommuniqueController.GetServerLanguage().Translate("roSendCommunique", "Communique")" style="display:none">
                    <img src="@SendIcon" alt="@CommuniqueController.GetServerLanguage().Translate("roSendCommunique", "Communique")." />
                </a>
                @<a Class='mainActionBtn' id='commCancelBtn' href="#" title="@CommuniqueController.GetServerLanguage().Translate("roCancelCommunique", "Communique")." style="display:none">
                    <img src="@CancelIcon" alt="@CommuniqueController.GetServerLanguage().Translate("roCancelCommunique", "Communique")." />
                </a>
                @<a Class='mainActionBtn' id='commUncancelBtn' href="#" title="@CommuniqueController.GetServerLanguage().Translate("roUnCancelCommunique", "Communique")" style="display:none">
                    <img src="@UncancelIcon" alt="@CommuniqueController.GetServerLanguage().Translate("roUnCancelCommunique", "Communique")." />
                </a>
                End If
            </div>
            <div style="height:50%;display:flex;flex-direction:column;justify-content:flex-end;"></div>
        </div>
    </div>
</div> 