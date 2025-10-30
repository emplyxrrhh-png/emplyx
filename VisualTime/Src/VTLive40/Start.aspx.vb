Imports System.Net
Imports Robotics.Base.DTOs
Imports Robotics.Portal.Business
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class Start
    Inherits PageBase

    Private oMenuList As New wscMenuElementList
    Private oSelectedToolbar1Item As wscMenuElement

    Public Event OptionSelected()

    Private strMainImagesPath As String = "../Images/MainMenu/"

    Private oContext As WebCContext

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("jquery", "~/Base/jquery/jquery-3.7.1.min.js", , True)
    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'If Not Me.Request.IsAuthenticated OrElse WLHelperWeb.CurrentPassport Is Nothing Then
        If WLHelperWeb.CurrentPassport Is Nothing Then
            divContainerMenu.Controls.Clear()
            divContainerMenu.InnerHtml = "<div class=""panHeader2""><span style="""">&nbsp;</span></div><br />"
        Else

            If Not IsPostBack Then
                divContainerMenu.Controls.Clear()
                CreaMenuIcons()
                rbClientsLink.HRef = HelperSession.AdvancedParametersCache("RoboticsUrl")

                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "ReloadRoboticsFeed", "loadNewFeed();", True)
            End If

            ';
        End If
    End Sub

    Private Sub CreaMenuIcons()
        'Si esta logat, carrega la botonera de menús...
        If WLHelperWeb.CurrentPassport IsNot Nothing Then

            Me.oContext = WLHelperWeb.Context(Me.Request)

            Dim oPassport As roPassportTicket = WLHelperWeb.CurrentPassport

            'Recupera dels webservices els menús PRINCIPAL (el que s'amaga de l'icona)
            oMenuList = PortalServiceMethods.GetMainMenu(Me.Page, "Portal", oPassport.ID, FeatureTypes.ManageFeature, WLHelperWeb.ServerLicense, oPassport.Language.Key)

            Dim companyList As New Hashtable
            Dim leftList As New Hashtable
            Dim rightList As New Hashtable

            For Each oMenuElement In oMenuList.List
                Dim strPriority As String = oMenuElement.Priority.ToString()
                Dim strID As String = strPriority.Substring(0, 3)

                'If oMenuElement.Path = "Portal\Company" Then
                '    companyList.Add(strID, oMenuElement)
                'Else
                '    If oMenuElement.Path <> "Portal\Home" Then
                '        If strPriority(3) = "1" Then
                '            leftList.Add(strID, oMenuElement)
                '        Else
                '            rightList.Add(strID, oMenuElement)
                '        End If
                '    End If
                ' End If

                If oMenuElement.Path <> "Portal\Home" AndAlso oMenuElement.Path <> "Portal\Company" Then
                    If strPriority(3) = "1" Then
                        leftList.Add(strID, oMenuElement)
                    Else
                        rightList.Add(strID, oMenuElement)
                    End If
                End If

            Next

            Dim leftControls As New Generic.List(Of HtmlGenericControl)
            Dim rightControls As New Generic.List(Of HtmlGenericControl)
            Dim companyControls As New Generic.List(Of HtmlGenericControl)

            Dim keysTmp(leftList.Count - 1) As String
            leftList.Keys.CopyTo(keysTmp, 0)
            Array.Sort(keysTmp)

            For Each strID As String In keysTmp
                If rightList.ContainsKey(strID) Then

                    Dim leftElement As wscMenuElement = CType(leftList(strID), wscMenuElement)

                    Dim boxLeftDiv As New HtmlGenericControl("div")

                    Dim htmlLeftCell As String = ""
                    htmlLeftCell = "<div float='left'>"
                    htmlLeftCell = htmlLeftCell & "<div class=""panHeader2""><span style="""">" & leftElement.Name & "</span></div>"
                    htmlLeftCell = htmlLeftCell & "</div>"
                    boxLeftDiv.InnerHtml = htmlLeftCell

                    Dim genericLeftDiv As New HtmlGenericControl("div")
                    genericLeftDiv.Attributes("class") = "MainMenu_box"
                    genericLeftDiv.Attributes("align") = "center"
                    genericLeftDiv.Attributes("float") = "left"
                    CreaIcos(leftElement.Name, genericLeftDiv)

                    boxLeftDiv.Controls.Add(genericLeftDiv)

                    leftControls.Add(boxLeftDiv)

                    leftList.Remove(strID)

                    Dim rightElem As wscMenuElement = CType(rightList(strID), wscMenuElement)

                    Dim boxRightDiv As New HtmlGenericControl("div")

                    Dim htmlRightCell As String = ""
                    htmlRightCell = "<div float='right'>"
                    htmlRightCell = htmlRightCell & "<div class=""panHeader2""><span style="""">" & rightElem.Name & "</span></div>"
                    htmlRightCell = htmlRightCell & "</div>"
                    boxRightDiv.InnerHtml = htmlRightCell

                    Dim genericRightDiv As New HtmlGenericControl("div")
                    genericRightDiv.Attributes("class") = "MainMenu_box"
                    genericRightDiv.Attributes("align") = "center"
                    genericRightDiv.Attributes("float") = "right"
                    CreaIcos(rightElem.Name, genericRightDiv)

                    boxRightDiv.Controls.Add(genericRightDiv)

                    rightControls.Add(boxRightDiv)

                    rightList.Remove(strID)
                End If
            Next

            Dim keysA(leftList.Count - 1) As String
            leftList.Keys.CopyTo(keysA, 0)
            Array.Sort(keysA)

            For Each strID As String In keysA

                Dim curElem As wscMenuElement = CType(leftList(strID), wscMenuElement)

                Dim boxDiv As New HtmlGenericControl("div")

                Dim htmlCell As String = ""
                htmlCell = "<div float='left'>"
                htmlCell = htmlCell & "<div class=""panHeader2""><span style="""">" & curElem.Name & "</span></div>"
                htmlCell = htmlCell & "</div>"
                boxDiv.InnerHtml = htmlCell

                Dim genericDiv As New HtmlGenericControl("div")
                genericDiv.Attributes("class") = "MainMenu_box"
                genericDiv.Attributes("align") = "center"
                genericDiv.Attributes("float") = "left"
                CreaIcos(curElem.Name, genericDiv)

                boxDiv.Controls.Add(genericDiv)

                leftControls.Add(boxDiv)
            Next

            Dim keysB(rightList.Count - 1) As String
            rightList.Keys.CopyTo(keysB, 0)
            Array.Sort(keysB)

            For Each strID As String In keysB
                Dim curElem As wscMenuElement = CType(rightList(strID), wscMenuElement)

                Dim boxDiv As New HtmlGenericControl("div")

                Dim htmlCell As String = ""
                htmlCell = "<div float='right'>"
                htmlCell = htmlCell & "<div class=""panHeader2""><span style="""">" & curElem.Name & "</span></div>"
                htmlCell = htmlCell & "</div>"
                boxDiv.InnerHtml = htmlCell

                Dim genericDiv As New HtmlGenericControl("div")
                genericDiv.Attributes("class") = "MainMenu_box"
                genericDiv.Attributes("align") = "center"
                genericDiv.Attributes("float") = "right"
                CreaIcos(curElem.Name, genericDiv)

                boxDiv.Controls.Add(genericDiv)

                rightControls.Add(boxDiv)
            Next

            Dim keysC(companyList.Count - 1) As String
            companyList.Keys.CopyTo(keysC, 0)
            Array.Sort(keysC)

            For Each strID As String In keysC
                Dim curElem As wscMenuElement = CType(companyList(strID), wscMenuElement)

                Dim boxDiv As New HtmlGenericControl("div")

                Dim htmlCell As String = ""
                htmlCell = "<div>"
                htmlCell = htmlCell & "<div class=""panHeader2""><span style="""">" & curElem.Name & "</span></div>"
                htmlCell = htmlCell & "</div>"
                boxDiv.InnerHtml = htmlCell

                Dim genericDiv As New HtmlGenericControl("div")
                genericDiv.Attributes("class") = "MainMenu_box"
                genericDiv.Attributes("align") = "center"
                genericDiv.Attributes("float") = "right"
                CreaIcos(curElem.Name, genericDiv)

                boxDiv.Controls.Add(genericDiv)

                companyControls.Add(boxDiv)
            Next

            Dim companyDiv As New HtmlGenericControl("div")
            companyDiv.Attributes("class") = "MainMenu_Shared"

            Dim columnsDiv As New HtmlGenericControl("div")
            columnsDiv.Style("clear") = "both"

            Dim leftColumDiv As New HtmlGenericControl("div")
            leftColumDiv.Attributes("class") = "MainMenu_Leftbox"

            For Each oLeftItem As HtmlGenericControl In leftControls
                leftColumDiv.Controls.Add(oLeftItem)
            Next

            Dim rightColumnDiv As New HtmlGenericControl("div")
            rightColumnDiv.Attributes("class") = "MainMenu_Rightbox"

            For Each oRightItem As HtmlGenericControl In rightControls
                rightColumnDiv.Controls.Add(oRightItem)
            Next

            For Each oCompanyControls As HtmlGenericControl In companyControls
                companyDiv.Controls.Add(oCompanyControls)
            Next

            columnsDiv.Controls.Add(leftColumDiv)
            columnsDiv.Controls.Add(rightColumnDiv)

            divContainerMenu.Controls.Add(companyDiv)
            divContainerMenu.Controls.Add(columnsDiv)
        End If

    End Sub

    Private Sub CreaIcos(ByVal oMenuName As String, ByRef oTableCell As HtmlGenericControl)
        Try
            Dim oMenuElement As wscMenuElement

            Dim hTable As New HtmlTable
            Dim hTableRow As New HtmlTableRow
            Dim hTableCell As HtmlTableCell

            ' Busco el elemento de la lista que han pulsado
            oSelectedToolbar1Item = Nothing
            For Each oMenuElement In oMenuList.List
                If oMenuName = oMenuElement.Name Then
                    oSelectedToolbar1Item = oMenuElement
                    Exit For
                End If
            Next

            If oSelectedToolbar1Item IsNot Nothing Then

                Dim oMenuButton As HtmlAnchor

                hTable.ID = "tbMainMenu"
                hTable.CellPadding = 0
                hTable.CellSpacing = 0
                hTable.Border = 0
                hTable.Rows.Add(hTableRow)

                For n As Integer = 0 To oSelectedToolbar1Item.Childs.List.Count - 1

                    oMenuElement = oSelectedToolbar1Item.Childs.List(n)

                    'Si es vol saltar el separador del menu...
                    If oMenuElement.URL = "" Then Continue For

                    hTableCell = New HtmlTableCell
                    oMenuButton = New HtmlAnchor

                    With oMenuButton
                        If oMenuElement.URL <> "" Then
                            .InnerHtml = "<img src=""Base/Images/StartMenuIcos/" & oMenuElement.ImageUrl & """ style=""border: 0;"" /><br />" & oMenuElement.Name
                            .Title = oMenuElement.Name
                            .Attributes("class") = "aMenuP"
                            .HRef = Request.ApplicationPath + "Main.aspx#/" + Configuration.RootUrl + "/" + oMenuElement.URL.Split(".")(0)
                            Dim strPath As String = oMenuElement.Path.Substring(0, oMenuElement.Path.LastIndexOf("\"))
                            .Attributes("onclick") = "top.reenviaFrame('" & String.Format("/{0}/" & oMenuElement.URL, Configuration.RootUrl) & "', '" & "MainMenu_" & oMenuName.Replace("'", "\'") & "_MainMenu_Button" & n.ToString & "','" & oMenuElement.Name.Replace("'", "\'") & "','" & strPath & "'); return false;"
                            .Attributes("linkPath") = strPath.Replace("\", "")

                            hTableCell.Attributes("class") = "mainScreenButtonWidth"
                            hTableCell.Align = "center"
                            hTableCell.Controls.Add(oMenuButton)

                        End If
                    End With

                    hTableRow.Cells.Add(hTableCell)

                Next

                oTableCell.Controls.Add(hTable)

            End If
        Catch ex As Exception
        End Try
    End Sub

End Class