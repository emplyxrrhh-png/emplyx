@Code
    Dim oSerializer As System.Web.Script.Serialization.JavaScriptSerializer
    oSerializer = New System.Web.Script.Serialization.JavaScriptSerializer()
    Dim StartController As VTLive40.StartController = New VTLive40.StartController()
End Code
<div style='display: flex'>
    <div class='minisection1'>
        <img src='./Base/Images/PortalRequests/icons8-help-48.png' />
    </div>
    <div class='minisection2'>
        @StartController.GetServerLanguage().Translate("roWarningPopup", "Start")
    </div>

    <div style="position: fixed; left: 50%; bottom: 5px; transform: translate(-50%, -50%); margin: 0 auto;">
        @(Html.DevExtreme().Button() _
                                                                        .ID("closePopup") _
                                                                                            .Text(StartController.GetServerLanguage().Translate("roAccept", "Start")) _
                                                                        .OnClick("closePopupWarning") _
                                                                                    .Type(ButtonType.Normal) _
            )
    </div>
</div>