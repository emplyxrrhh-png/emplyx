@Code
    Dim ReportController As VTLive40.ReportController = New VTLive40.ReportController()
End Code

<a class="reportActionBtn viewTab activeTab" id="generalTab" href="#">
    @ReportController.GetServerLanguage().Translate("General", "ReportsDX")
</a>

<a class="reportActionBtn viewTab" id="planificationsTab" href="#">
    @ReportController.GetServerLanguage().Translate("Planificaciones", "ReportsDX")
</a>