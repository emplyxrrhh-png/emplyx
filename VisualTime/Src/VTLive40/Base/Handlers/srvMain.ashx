<%@ WebHandler Language="VB" Class="srvMain" %>

Imports System.Data
Imports System.Data.Common
Imports Robotics.Web.Base
Imports System.Net
Imports System.IO
Imports System.Xml
Imports DevExpress.Web
Imports Robotics.Base.DTOs


Public Class srvMain
    Inherits handlerBase

    Private oPermission As Permission

    Public Overrides Sub ProcessRoboticsRequest(ByVal context As HttpContext)
        Select Case Request("action").ToUpper
            Case "LOADXMLFEED" ' Retorna la capcelera de la plana
                LoadXmlFeed()
        End Select
    End Sub

#Region "Methods"
    ''' <summary>
    ''' Carrega roShift per ID (sols el Tab superior)
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadXmlFeed()

        Try
            Dim bLoad As Boolean = True

            Dim MyRssDocument As XmlDocument = HelperSession.GetXmlFeedData()
            If MyRssDocument Is Nothing Then
                bLoad = False
            End If

            Dim tbl_Feed_Reader As New HtmlTable()
            If bLoad AndAlso MyRssDocument IsNot Nothing Then
                Dim MyRssList As XmlNodeList = MyRssDocument.SelectNodes("rss/channel/item")

                Dim sTitle As String = ""
                Dim sLink As String = ""
                Dim sDescription As String = ""
                Dim sThumbnail As String = ""

                Dim max As Integer = MyRssList.Count - 1
                If max > 5 Then max = 5
                'Iterate/Loop through RSS Feed items
                For i As Integer = 0 To max
                    Dim MyRssDetail As XmlNode

                    MyRssDetail = MyRssList.Item(i).SelectSingleNode("title")
                    If Not MyRssDetail Is Nothing Then
                        sTitle = MyRssDetail.InnerText
                    Else
                        sTitle = ""
                    End If

                    MyRssDetail = MyRssList.Item(i).SelectSingleNode("link")
                    If Not MyRssDetail Is Nothing Then
                        sLink = MyRssDetail.InnerText
                    Else
                        sLink = ""
                    End If

                    MyRssDetail = MyRssList.Item(i).SelectSingleNode("description")
                    If Not MyRssDetail Is Nothing Then
                        Dim strTmpText As String = MyRssDetail.InnerText
                        Dim iIndex As Integer = strTmpText.IndexOf(" ", CInt(strTmpText.Length / 4))
                        strTmpText = strTmpText.Substring(0, iIndex).Trim & " [&#8230;]"

                        sDescription = strTmpText.Replace("<p>The post", "")
                    Else
                        sDescription = ""
                    End If

                    MyRssDetail = MyRssList.Item(i).SelectSingleNode("thumbnail")
                    If Not MyRssDetail Is Nothing Then
                        sThumbnail = MyRssDetail.InnerText
                    Else
                        sThumbnail = ""
                    End If

                    'Now generating HTML table rows and cells based on Title,Link & Description
                    Dim block As HtmlTableCell = New HtmlTableCell()

                    Dim imageDiv As New HtmlGenericControl("div")
                    imageDiv.Attributes("style") = "width:16px;float:left"
                    If sThumbnail = String.Empty Then
                        imageDiv.InnerHtml = "<img alt='' style='width:16px;height:16px' src='" & VirtualPathUtility.ToAbsolute("~/Base/Images/rb_logo.gif") & "'/>"
                    Else
                        imageDiv.InnerHtml = "<img alt='' style='width:16px;height:16px' src='" & sThumbnail & "'/>"
                    End If

                    Dim titleDiv As New HtmlGenericControl("div")
                    titleDiv.Attributes("style") = "float:left;width:240px;padding-left: 5px; padding-bottom: 5px"
                    titleDiv.InnerHtml = "<span style='font-weight:bold'><a href='" + sLink + "' target='new'>" + sTitle + "</a></span>"

                    Dim firstRowDiv As New HtmlGenericControl("div")
                    firstRowDiv.Attributes("style") = "padding-left:5px"
                    firstRowDiv.Controls.Add(imageDiv)
                    firstRowDiv.Controls.Add(titleDiv)

                    Dim secondRowDiv As New HtmlGenericControl("div")
                    secondRowDiv.Attributes("style") = "clear:both;padding-left:5px"
                    secondRowDiv.InnerHtml = "<p align='justify'>" + sDescription + "</p>"

                    block.Controls.Add(firstRowDiv)
                    block.Controls.Add(secondRowDiv)

                    Dim row As HtmlTableRow = New HtmlTableRow()
                    row.Cells.Add(block)
                    tbl_Feed_Reader.Rows.Add(row)
                Next
            Else
                'Now generating HTML table rows and cells based on Title,Link & Description
                Dim block As HtmlTableCell = New HtmlTableCell()

                Dim imageDiv As New HtmlGenericControl("div")
                imageDiv.Attributes("style") = "width:16px;float:left"
                imageDiv.InnerHtml = "<img alt='' style='width:16px;height:16px' src='" & VirtualPathUtility.ToAbsolute("~/Base/Images/rb_logo.gif") & "'/>"

                Dim titleDiv As New HtmlGenericControl("div")
                titleDiv.Attributes("style") = "float:left;width:240px;padding-left: 5px;"
                titleDiv.InnerHtml = "<p align='justify'>" + Me.Language.Translate("noNews", Me.DefaultScope) + "</p>"

                Dim firstRowDiv As New HtmlGenericControl("div")
                firstRowDiv.Attributes("style") = "padding:5px"
                firstRowDiv.Controls.Add(imageDiv)
                firstRowDiv.Controls.Add(titleDiv)

                block.Controls.Add(firstRowDiv)

                Dim row As HtmlTableRow = New HtmlTableRow()
                row.Cells.Add(block)
                tbl_Feed_Reader.Rows.Add(row)
            End If

            Dim sw As New IO.StringWriter
            Dim htw As New HtmlTextWriter(sw)
            tbl_Feed_Reader.RenderControl(htw)

            Response.Write(sw.ToString)
        Catch ex As Exception
            Dim tbl_Feed_Reader As New HtmlTable()

            'Now generating HTML table rows and cells based on Title,Link & Description
            Dim block As HtmlTableCell = New HtmlTableCell()

            Dim imageDiv As New HtmlGenericControl("div")
            imageDiv.Attributes("style") = "width:16px;float:left"
            imageDiv.InnerHtml = "<img alt='' style='width:16px;height:16px' src='" & VirtualPathUtility.ToAbsolute("~/Base/Images/rb_logo.gif") & "'/>"

            Dim titleDiv As New HtmlGenericControl("div")
            titleDiv.Attributes("style") = "float:left;width:240px;padding-left: 5px;"
            titleDiv.InnerHtml = "<p align='justify'>" + Me.Language.Translate("noNews", Me.DefaultScope) + "</p>"

            Dim firstRowDiv As New HtmlGenericControl("div")
            firstRowDiv.Attributes("style") = "padding:5px"
            firstRowDiv.Controls.Add(imageDiv)
            firstRowDiv.Controls.Add(titleDiv)

            block.Controls.Add(firstRowDiv)

            Dim row As HtmlTableRow = New HtmlTableRow()
            row.Cells.Add(block)
            tbl_Feed_Reader.Rows.Add(row)

            Dim sw As New IO.StringWriter
            Dim htw As New HtmlTextWriter(sw)
            tbl_Feed_Reader.RenderControl(htw)

            Response.Write(sw.ToString)
        End Try
    End Sub


#End Region


End Class