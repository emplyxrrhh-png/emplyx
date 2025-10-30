<%@ WebHandler Language="VB" Class="srvCalendarImport" %>

Imports System
Imports System.Web
Imports System.Text
Imports System.Data
Imports System.Runtime.Serialization
Imports Robotics.Web.Base
Imports Robotics.VTBase

Public Class srvCalendarImport
    Inherits handlerBase


    Public Overrides Sub ProcessRoboticsRequest(ByVal context As HttpContext)
        Dim strMsgError As String = String.Empty

        If Request.Files.Count = 1 Then
            For Each filename As String In Request.Files

                Dim file As HttpPostedFile = Request.Files(filename)

                If file Is Nothing Then
                    Throw New System.IO.FileNotFoundException("Please upload a valid file")
                End If

                If strMsgError = String.Empty Then
                    Try
                        Dim bFileBytes() As Byte = New Byte((file.InputStream.Length) - 1) {}
                        file.InputStream.Read(bFileBytes, 0, file.InputStream.Length)

                        HttpContext.Current.Session("CALENDAR_IMPORT") = bFileBytes
                        'file.SaveAs(strFileName)
                        strMsgError = "Ok"
                    Catch ex As Exception
                        strMsgError = "Error"
                        HttpContext.Current.Session("CALENDAR_IMPORT") = New Byte()
                    End Try
                Else
                    strMsgError = "NoFile"
                End If

            Next
        Else
            strMsgError = "MultipleFiles"
        End If

        Response.Clear()
        Response.Write(roJSONHelper.SerializeNewtonSoft(strMsgError))

    End Sub


End Class
