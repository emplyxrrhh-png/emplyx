Imports System.Web.Mvc
Imports DevExpress.XtraSpreadsheet.Import.Xls
Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.Web.Base

Public Class CookieController
    Inherits System.Web.Mvc.Controller
    Private oLanguage As roLanguageWeb

    <HttpPost>
    Public Function GetCookie(sCookieName As String) As JsonResult
        Dim oCookieValue As String = String.Empty

        oCookieValue = API.SecurityV3ServiceMethods.GetTreeState(Nothing, WLHelperWeb.CurrentPassportID, sCookieName)

        Return Json(oCookieValue)
    End Function

    <HttpPost>
    Public Function SetCookie(sCookieName As String, sCookieValue As String) As JsonResult
        Dim bRes As Boolean

        Try
            sCookieValue = HttpUtility.UrlDecode(sCookieValue)
            bRes = API.SecurityV3ServiceMethods.SaveTreeState(Nothing, WLHelperWeb.CurrentPassportID, sCookieName, sCookieValue)
        Catch ex As Exception
            bRes = False
        End Try

        Return Json(bRes)
    End Function

    <HttpPost>
    Public Function EraseCookie(sCookieName As String) As JsonResult
        Dim bRes As Boolean
        Try
            bRes = API.SecurityV3ServiceMethods.DeleteTreeState(Nothing, WLHelperWeb.CurrentPassportID, sCookieName)
        Catch ex As Exception
            bRes = False
        End Try

        Return Json(bRes)
    End Function


    <HttpPost>
    Public Function GetUniversalSelector(sSelectorName As String) As JsonResult
        Dim oCookieValue As String = String.Empty

        oCookieValue = API.SecurityV3ServiceMethods.GetUniversalSelector(Nothing, WLHelperWeb.CurrentPassportID, sSelectorName)

        Return Json(oCookieValue)
    End Function

    <HttpPost>
    Public Function SetUniversalSelector(sSelectorName As String, sSelectorValue As String) As JsonResult
        Dim bRes As Boolean

        Try
            sSelectorValue = HttpUtility.UrlDecode(sSelectorValue)
            bRes = API.SecurityV3ServiceMethods.SaveUniversalSelector(Nothing, WLHelperWeb.CurrentPassportID, sSelectorName, sSelectorValue)
        Catch ex As Exception
            bRes = False
        End Try

        Return Json(bRes)
    End Function

    <HttpPost>
    Public Function EraseUniversalSelector(sSelectorName As String) As JsonResult
        Dim bRes As Boolean
        Try
            bRes = API.SecurityV3ServiceMethods.DeleteUniversalSelector(Nothing, WLHelperWeb.CurrentPassportID, sSelectorName)
        Catch ex As Exception
            bRes = False
        End Try

        Return Json(bRes)
    End Function

End Class