Imports Robotics.Web.Base

Partial Class Scheduler_SetLocalization
    Inherits Robotics.Web.Base.PageBase

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertCssIncludes(Me.Page)
        Me.InsertExtraJavascript("gmaps", $"https://maps.googleapis.com/maps/api/js?key={Me.MapsKey}&callback=Function.prototype&language={HelperWeb.GetCookie("VTLive_Language")}&loading=async", , , False)

        Me.hdnCity.Value = HttpUtility.HtmlDecode(Request.QueryString("city"))
    End Sub

End Class