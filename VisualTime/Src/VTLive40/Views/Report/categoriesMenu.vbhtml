@imports System.Web.Mvc
@imports Robotics.Web.VTLiveMvC.Controllers

@code
        Dim ReportController As VTLive40.ReportController = New VTLive40.ReportController()
        Dim CategoriesList As String = ReportController.GetReportCategoriesAsJson()
    @<p>@CategoriesList</p>
End Code