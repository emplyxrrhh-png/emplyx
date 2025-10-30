@Code
    Dim barButtonData = ViewData(VTLive40.Helpers.Constants.DefaultBarButtonData)
    Dim labels = ViewData(VTLive40.Helpers.Constants.DefaultLanguagesEntries)
End Code

<div>
    <div Class="dx-field rigthTabButtons active divSaveGeneric" style="text-align:right; margin: 10px 10px 5px 10px;display:none">
        <div class="divImageMsg left">
            <img class="saveBarComponent" alt="" id="Img1" src="@Url.Content("~/Base/Images/MessageFrame/Alert16.png")" />
        </div>
        <div class="messageText left">
            <span class="saveBarComponent" id="msgTop">@Html.Raw(labels("Common#DataModified"))</span>
        </div>
        <div align="right" class="messageActions right">
            <a href="javascript: void(0);" class="aMsg saveBarComponent" onclick="saveData();"><span id="lblSaveChanges">@Html.Raw(labels("Common#SaveChanges"))</span></a>
            &nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;
            <a href="javascript: void(0);" onclick="undoChanges();" class="aMsg saveBarComponent"><span id="lblUndoChanges">@Html.Raw(labels("Common#UndoChanges"))</span></a>
        </div>
    </div>
</div>