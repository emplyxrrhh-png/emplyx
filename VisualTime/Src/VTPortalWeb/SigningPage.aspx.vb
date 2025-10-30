Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Public Class SigningPage
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim idDocument As Integer = roTypes.Any2Integer(Request.Params("IdDocument"))
        Dim isApp As Boolean = roTypes.Any2Boolean(Request.Params("App"))
        Dim MTClientId As String = roTypes.Any2String(Request.Params("MTClient"))
        Dim GUID As String = roTypes.Any2String(Request.Params("GUID"))

        Me.signResult.Value = "Not signed"
        If idDocument > 0 AndAlso GUID.Length > 0 Then

            If HttpContext.Current.Request IsNot Nothing AndAlso roTypes.Any2String(HttpContext.Current.Request.Headers("roCompanyID")) = String.Empty Then HttpContext.Current.Request.Headers.Add("roCompanyID", MTClientId.Split("-")(0).ToLower().Trim)
            Global_asax.ReloadSharedData()

            Dim bolret As Boolean = API.DocumentsServiceMethods.SignStatusDocumentInProgress(Me.Page, idDocument, GUID, True)

            HttpContext.Current.Session("roMultiCompanyId") = String.Empty

            Me.signResult.Value = bolret.ToString()
        End If

        Me.processFinished.Value = "1"

        If Not isApp Then
            Robotics.Web.Base.HelperWeb.EraseCookie("VTPortalSign")
            Robotics.Web.Base.HelperWeb.CreateCookie("VTPortalSign", idDocument, False)

            Robotics.Web.Base.HelperWeb.CreateCookie("VTSignResult", Me.signResult.Value, True)

            Response.Redirect("2/indexv2.aspx")
        End If

    End Sub

End Class