Imports Robotics.Web.Base

Partial Class Scheduler_Controls_LocalizationMapPage
    Inherits Robotics.Web.Base.PageBase

    Private Sub Scheduler_Controls_LocalizationMapPage_Load(sender As Object, e As EventArgs) Handles Me.Load

        Me.InsertExtraJavascript("gmaps", $"https://maps.googleapis.com/maps/api/js?key={Me.MapsKey}&callback=Function.prototype&language={HelperWeb.GetCookie("VTLive_Language")}&loading=async", , , False)

    End Sub

End Class