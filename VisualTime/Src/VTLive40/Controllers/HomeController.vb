Imports System.Web.Mvc

Public Class HomeController
    Inherits System.Web.Mvc.Controller

    Function Index() As ActionResult

        Return Redirect("~/Default.aspx")
    End Function

End Class