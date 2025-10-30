Imports System.Web.Mvc
Imports Robotics.Web.Base
Imports VTLive40.Controllers.Base

Public Class CardsTreeController
    Inherits BaseController

    Private oLanguage As roLanguageWeb

    Function Index() As ActionResult
        'ViewData("CardBaseIcon") = ""
        'ViewData("CardClickEvent") = ""
        'ViewData("TreeCaption") = "TreeCaption"
        'ViewData("Model") = ""
        'Return PartialView("CardTree/cardTree")
        Return View("Index")
    End Function

    <HttpPost>
    Function UpdateCardView(modelName As String) As JsonResult

        Dim myEnum As CardTreeTypes = DirectCast([Enum].Parse(GetType(CardTreeTypes), modelName), CardTreeTypes)

        Return Json(Me.GetCardViewData(myEnum))
    End Function

End Class