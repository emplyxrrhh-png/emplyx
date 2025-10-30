Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Public Class _Default
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Robotics.Web.Base.HelperWeb.EraseCookie("VTPortalApi")

        If WLHelperWeb.ADFSEnabled AndAlso roTypes.Any2Boolean(Request.Params("FORCE_LOAD")) = True Then
            ResponseHelper.Redirect("index.aspx?v=" & Date.Now.Ticks, "_blank", "toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=yes, resizable=yes, copyhistory=yes, width=470, height=770")
        Else
            Dim portalToken As String = Request.Params("sToken")
            Dim iRequest As String = Request.Params("VTPortalRequest")

            If portalToken IsNot Nothing Then
                Robotics.Web.Base.HelperWeb.EraseCookie("VTPortalToken")
                Robotics.Web.Base.HelperWeb.CreateCookie("VTPortalToken", portalToken, False)
            End If

            If iRequest IsNot Nothing Then
                Robotics.Web.Base.HelperWeb.EraseCookie("VTPortalRequest")
                Robotics.Web.Base.HelperWeb.CreateCookie("VTPortalRequest", iRequest, False)
            End If

            ResponseHelper.Redirect("index.aspx?v=" & Date.Now.Ticks, "_blank", "toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=yes, resizable=yes, copyhistory=yes, width=470, height=770")
        End If

        ResponseHelper.Redirect("index.aspx?v=" & Date.Now.Ticks, "_blank", "toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=yes, resizable=yes, copyhistory=yes, width=470, height=770")
    End Sub

    Protected Sub btOpen_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btOpen.Click
        ResponseHelper.Redirect("index.aspx?v=" & Date.Now.Ticks, "_blank", "toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=yes, resizable=yes, copyhistory=yes, width=470, height=770")
    End Sub

End Class

Public NotInheritable Class ResponseHelper

    Private Sub New()
    End Sub

    Public Shared Sub Redirect(ByVal url As String, ByVal target As String, ByVal windowFeatures As String)
        Dim context As HttpContext = HttpContext.Current

        If ([String].IsNullOrEmpty(target) OrElse target.Equals("_self", StringComparison.OrdinalIgnoreCase)) AndAlso [String].IsNullOrEmpty(windowFeatures) Then
            context.Response.Redirect(url)
        Else
            Dim page As Page = DirectCast(context.Handler, Page)
            If page Is Nothing Then
                Throw New InvalidOperationException("Cannot redirect to new window outside Page context.")
            End If

            url = page.ResolveClientUrl(url)
            Dim script As String

            If Not [String].IsNullOrEmpty(windowFeatures) Then
                script = "window.open(""{0}"", ""{1}"", ""{2}"");"
            Else
                script = "window.open(""{0}"", ""{1}"");"
            End If

            script = [String].Format(script, url, target, windowFeatures)

            ScriptManager.RegisterStartupScript(page, GetType(Page), "Redirect", script, True)
        End If

    End Sub

End Class