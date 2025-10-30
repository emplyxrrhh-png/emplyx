Imports System.Web.Mvc
Imports System.Web.Script.Serialization
Imports System.Web.Services
Imports DevExpress.XtraSpreadsheet.Import.Xls
Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Public Class WebLinksController
    Inherits System.Web.Mvc.Controller
    Private oLanguage As roLanguageWeb

    <HttpPost>
    Public Function GetWebLinks() As JsonResult
        Dim bRes As String
        Try
            Dim webLinks As List(Of roWebLink) = WebLinkServiceMethods.GetAllWebLinks(Nothing)
            Dim serializer As New JavaScriptSerializer()
            bRes = serializer.Serialize(webLinks)
        Catch ex As Exception
            bRes = Nothing
        End Try

        Return Json(bRes)
    End Function

    <HttpPost>
    Public Function SaveWebLink(ByVal webLink As roWebLink) As JsonResult
        Dim bRes As Integer

        Try
            bRes = WebLinkServiceMethods.CreateOrUpdateWebLink(webLink, Nothing)
        Catch ex As Exception
            bRes = 0
        End Try

        Return Json(bRes)
    End Function

    <HttpPost>
    Public Function DeleteWebLink(ByVal webLink As roWebLink) As JsonResult
        Dim bRes As Boolean

        Try
            WebLinkServiceMethods.DeleteWebLink(webLink, Nothing)
            bRes = True
        Catch ex As Exception
            bRes = False
        End Try

        Return Json(bRes)
    End Function

    <HttpPost>
    Public Function SaveNewOrder(newOrder As List(Of roWebLink)) As JsonResult
        Dim bRes As Boolean

        Try
            For Each link As roWebLink In newOrder
                WebLinkServiceMethods.CreateOrUpdateWebLink(link, Nothing)
            Next
            bRes = True
        Catch ex As Exception
            bRes = False
        End Try

        Return Json(bRes)
    End Function

End Class