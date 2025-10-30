@Code
    Dim ReportController As VTLive40.ReportController = New VTLive40.ReportController()
End Code

<div style="height:470px;" id="editView">
    <form id="selectorPeriodTime"
          onchange="collectParamValues()"
          onsubmit="(event) => event.preventDefault()"
          action="#"
          style="height: 100%;">
        <div id="datetimeSelects">
            <div>
                <input id="libre0" type="radio" value="0" name="period" />
                <label for="libre0" class="dx-wrap">@ReportController.GetServerLanguage().Translate("roReportSelectorFreeIntroduceExactDates", "ReportsDX")</label>
            </div>
            <div>
                <input id="mañana1" type="radio" value="1" name="period" />
                <label for="mañana1" class="dx-wrap">@ReportController.GetServerLanguage().Translate("roReportSelectorTomorrow", "ReportsDX")</label>
            </div>
            <div>
                <input id="hoy2" type="radio" value="2" name="period" checked />
                <label for="hoy2" class="dx-wrap">@ReportController.GetServerLanguage().Translate("roReportSelectorToday", "ReportsDX")</label>
            </div>
            <div>
                <input id="ayer3" type="radio" value="3" name="period" />
                <label for="ayer3" class="dx-wrap">@ReportController.GetServerLanguage().Translate("roReportSelectorYesterday", "ReportsDX")</label>
            </div>
            <div>
                <input id="semanaActual4" type="radio" value="4" name="period" />
                <label for="semanaActual4" class="dx-wrap">@ReportController.GetServerLanguage().Translate("roReportSelectorCurrentWeek", "ReportsDX")</label>
            </div>
            <div>
                <input id="semanaPasada5" type="radio" value="5" name="period" />
                <label for="semanaPasada5" class="dx-wrap">@ReportController.GetServerLanguage().Translate("roReportSelectorLastWeek", "ReportsDX")</label>
            </div>
            <div>
                <input id="mesActual6" type="radio" value="6" name="period" />
                <label for="mesActual6" class="dx-wrap">@ReportController.GetServerLanguage().Translate("roReportSelectorCurrentMonth", "ReportsDX")</label>
            </div>
            <div>
                <input id="mesAnterior7" type="radio" value="7" name="period" />
                <label for="mesAnterior7" class="dx-wrap">@ReportController.GetServerLanguage().Translate("roReportSelectorLastMonth", "ReportsDX")</label>
            </div>
            <div>
                <input id="añoActual8" type="radio" value="8" name="period" />
                <label for="añoActual8" class="dx-wrap">@ReportController.GetServerLanguage().Translate("roReportSelectorCurrentYear", "ReportsDX")</label>
            </div>
            <div>
                <input id="semanaSiguiente9" type="radio" value="9" name="period" />
                <label for="semanaSiguiente9" class="dx-wrap">@ReportController.GetServerLanguage().Translate("roReportSelectorNextWeek", "ReportsDX")</label>
            </div>
            <div>
                <input id="mesSiguiente10" type="radio" value="10" name="period" />
                <label for="mesSiguiente10" class="dx-wrap">@ReportController.GetServerLanguage().Translate("roReportSelectorNextMonth", "ReportsDX")</label>
            </div>
        </div>
        <div>
            <div>
                <label for="desdeDateTime">@ReportController.GetServerLanguage().Translate("roReportSelectorFrom", "ReportsDX"): </label><div id="desdeDateTime" />
            </div>
            <div>
                <label for="hastaDateTime">@ReportController.GetServerLanguage().Translate("roReportSelectorUntil", "ReportsDX"):   </label><div id="hastaDateTime" />
            </div>
        </div>
    </form>
</div>