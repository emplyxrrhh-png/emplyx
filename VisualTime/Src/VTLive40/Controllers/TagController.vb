Imports System.Web.Mvc
Imports DevExpress.XtraSpreadsheet.Import.Xls
Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.Web.Base

Public Class TagController
    Inherits System.Web.Mvc.Controller
    Private oLanguage As roLanguageWeb

    <HttpGet>
    Public Function GetAvailableTags() As JsonResult
        Dim currentTags As Array = HttpContext.Session("dummyTags")

        If currentTags Is Nothing Then
            currentTags = {New With {.id = "tag1"}, New With {.id = "tag2"}, New With {.id = "tag3"}, New With {.id = "tag4"}, New With {.id = "tag5"}, New With {.id = "tag6"}}

            HttpContext.Session("dummyTags") = currentTags
        End If


        Return Json(currentTags, JsonRequestBehavior.AllowGet)
    End Function

    <HttpGet>
    Public Function GetCustomTagBox(type As String) As JsonResult
        Dim availableTags

        Select Case type
            Case "tags"
                Dim currentTags As Array = HttpContext.Session("dummyTags")

                If currentTags Is Nothing Then
                    currentTags = {New With {.id = "tag1"}, New With {.id = "tag2"}, New With {.id = "tag3"}, New With {.id = "tag4"}, New With {.id = "tag5"}, New With {.id = "tag6"}}

                    HttpContext.Session("dummyTags") = currentTags
                End If
                availableTags = currentTags

            Case "shifts"
                Dim lstShifts As New Generic.List(Of Object)
                Dim dtShifts As DataTable = API.ShiftServiceMethods.GetShifts(Nothing, ListObsoletes:=False)
                For Each row As DataRow In dtShifts.Rows
                    lstShifts.Add(New With {.Id = roTypes.Any2Integer(row("Id")), .Name = row("Name"), .GroupId = row("IDGroup")})
                Next
                availableTags = lstShifts.ToArray()

            Case "shiftgroups"
                Dim lstShiftGroups As New Generic.List(Of IdNamePair)
                Dim dtShiftGroups As DataTable = API.ShiftServiceMethods.GetShiftGroups(Nothing)
                For Each row As DataRow In dtShiftGroups.Rows
                    lstShiftGroups.Add(New IdNamePair With {.Id = row("Id"), .Name = row("Name")})
                Next
                availableTags = lstShiftGroups.ToArray()

            Case "rulettypes"
                availableTags = {New With {.Id = 2, .Name = "Justificacion", .Description = "Descripción de la regla de justificación"}, New With {.Id = 3, .Name = "Diaria", .Description = "Descripción de la regla de justifiación diaria"}}
            Case Else
                availableTags = {}
        End Select

        Return Json(availableTags, JsonRequestBehavior.AllowGet)
    End Function


End Class