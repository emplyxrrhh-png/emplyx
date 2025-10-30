@Code
    Dim CommuniqueController As VTLive40.CommuniqueController = New VTLive40.CommuniqueController()
End Code
<div class="aParent saveOrDiscartDiv" style="float: right;margin-top: 1px;display: none; margin-right:10px;">
    <div id="communiqueeSave" class="commSaveBtn" style="display: inline-block;margin-left:10px;float:right;">
        @(Html.DevExtreme().Button() _
                                                .ID("saveCommuniquee") _
                                                .Icon("todo") _
            .Type(ButtonType.Default) _
            )
    </div>
    <div id="communiqueeCancel" class="commDiscartBtn" style="display: inline-block; margin-left: 10px;float:right;">
        @(Html.DevExtreme().Button() _
                        .ID("btnCancelCommuniquee") _
                        .Icon("close") _
                        .Type(ButtonType.Danger) _
            )
    </div>
</div> 