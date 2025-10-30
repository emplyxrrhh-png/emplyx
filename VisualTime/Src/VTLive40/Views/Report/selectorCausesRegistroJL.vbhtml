@Code
    Dim ReportController As VTLive40.ReportController = New VTLive40.ReportController()
End Code
<div style="height:400px;" id="editView">
    <form id="selectorCausesRegistroJL" onsubmit="preventDefaultOnSubmit(event)" style="">

        <div>
            <p>@ReportController.GetServerLanguage.Translate("roReportDesignerTitleOpcionalCauses", "ReportsDX")</p>
            <div id="showCauses"></div>
        </div>
        <div class="selectores">
            <span class="comboHelperText">
                @ReportController.GetServerLanguage().Translate("roReportDesignerJustifications", "ReportsDX")
            </span>
            <div id="selectCauses"></div>
        </div>
    </form>
</div>