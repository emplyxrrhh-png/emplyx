<%@ WebHandler Language="VB" Class="srvTerminalsList" %>

Imports System
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports System.Data
Imports Robotics.Base.DTOs
Imports Robotics.Web.Base.API

Public Class srvTerminalsList
    Inherits handlerBase

    Private Const FeatureAlias As String = "Terminals.StatusInfo"
    Private Const DefinitionFeatureAlias As String = "Terminals.Definition"
    Private oPermissionDefinition As Permission
    Private oPermission As Permission


    Public Overrides Sub ProcessRoboticsRequest(ByVal context As HttpContext)
        Me.scope = "Terminals"
        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)
        Me.oPermissionDefinition = Me.GetFeaturePermission(DefinitionFeatureAlias)

        If Me.oPermission > Permission.None Then

            Select Case context.Request("action")
                Case "getTerminalsListTab" ' Retorna la capcelera de la plana
                    LoadTerminalsListDataTab()
                Case "getBarButtons"
                    GetBarButtons(Request("ID"))
                Case "launchBroadcaster"
                    LaunchBroadcaster()
                Case "getTerminalInfo"
                    RefreshTerminalStatus(Request("ID"))
            End Select

        Else
            ' Si el passport actual no tiene permisos, devuelve un msgbox y redirecciona a la página principal al aceptar el mensaje.
            Dim strResponse As String = "MESSAGE" & _
                          "TitleKey=CheckPermission.Denied.Title&" + _
                          "DescriptionKey=CheckPermission.Denied.Description&" + _
                          "Option1TextKey=CheckPermission.Denied.Option1Text&" + _
                          "Option1DescriptionKey=CheckPermission.Denied.Option1Description&" + _
                          "Option1OnClickScript=HideMsgBoxForm(); window.location = '" & WLHelperWeb.DefaultRedirectUrl & "' return false;&" + _
                          "IconUrl=" & HelperWeb.MsgBoxIconsUrl(HelperWeb.MsgBoxIcons.AlertIcon)
            Response.Write(strResponse)
        End If
    End Sub

#Region "Methods"

    Private Sub RefreshTerminalStatus(ByVal IDTerminal As Integer)
        Dim resultHtml As String = String.Empty

        Try

            Dim oTerminalList As DataTable = API.TerminalServiceMethods.GetTerminalsLiveStatus(Nothing, "t.ID=" & IDTerminal, False)

            If oTerminalList IsNot Nothing AndAlso oTerminalList.Rows.Count > 0 Then
                Dim oTerminal As DataRow = Nothing
                Dim oTerminalReader2 As DataRow = Nothing

                For Each oRow As DataRow In oTerminalList.Rows
                    If roTypes.Any2Integer(oRow("IDReader")) = 1 Then
                        oTerminal = oRow
                    Else
                        oTerminalReader2 = oRow
                    End If
                Next


                If oPermission > Permission.None Then
                    resultHtml = "<table>" &
                                        "<tr>" &
                                        "<td valign=""top"" nowrap=""nowrap"">" & Me.Language.Translate("TerminalList.LastPunches", Me.DefaultScope) & "&nbsp;" & Format(Date.Now, HelperWeb.GetShortDateFormat) & "</td>" &
                                        "<td align=""right"" valign=""bottom""><span style=""font-weight: bold;"">" & oTerminal("punchCountReader") & "</span></td>" &
                                        "</tr>" &
                                        "<tr>" &
                                        "<td valign=""top"" nowrap=""nowrap"">" & Me.Language.Translate("TerminalList.LastEmployee", Me.DefaultScope) & "</td>" &
                                        "<td align=""right"" valign=""bottom"" style=""width: 180px""><span style=""font-weight: bold;"">" & oTerminal("LastEmp") & "</span></td>" &
                                        "</tr>" &
                                         "<tr>" &
                                        "<td valign=""top"" nowrap=""nowrap"">" & Me.Language.Translate("TerminalList.PendingTasks", Me.DefaultScope) & "</td>" &
                                        "<td align=""right"" valign=""bottom"" style=""width: 180px""><span style=""font-weight: bold;"">" & oTerminal("PendingTasks") & "</span></td>" &
                                        "</tr>" &
                                        "<tr>"
                    If oTerminalReader2 IsNot Nothing AndAlso roTypes.Any2String(oTerminalReader2("MODE")) <> String.Empty Then
                        resultHtml &= "<tr>" & _
                                        "<td valign=""top"" nowrap=""nowrap"">" & Me.Language.Translate("TerminalList.LastPunches2", Me.DefaultScope) & "&nbsp;" & Format(Date.Now, HelperWeb.GetShortDateFormat) & "</td>" & _
                                        "<td align=""right"" valign=""bottom""><span style=""font-weight: bold;"">" & oTerminalReader2("punchCountReader") & "</span></td>" & _
                                        "</tr>" & _
                                        "<tr>" & _
                                        "<td valign=""top"" nowrap=""nowrap"">" & Me.Language.Translate("TerminalList.LastEmployee2", Me.DefaultScope) & "</td>" & _
                                        "<td align=""right"" valign=""bottom"" style=""width: 180px""><span style=""font-weight: bold;"">" & oTerminalReader2("LastEmp") & "</span></td>" & _
                                        "</tr>"

                    End If
                    resultHtml &= "<tr>" & _
                                        "<td valign=""top"" nowrap=""nowrap""></td>" & _
                                        "<td align=""right"" valign=""bottom""><span style=""font-weight: bold;""></span></td>" & _
                                        "</tr>" & _
                                        "</table>"
                Else
                    resultHtml = ""
                End If
            End If



        Catch ex As Exception
            Response.Write("")
        Finally
            Response.Write(resultHtml)
        End Try
    End Sub


    Private Sub LaunchBroadcaster()
        Dim bolRet As Boolean = False
        Try

            bolRet = Robotics.Web.Base.API.ConnectorServiceMethods.LaunchBroadcaster(Nothing)

        Catch ex As Exception
        Finally
            If bolRet Then
                Response.Write("OK")
            Else
                Response.Write("NOK")
            End If
        End Try

    End Sub

    Private Sub GetBarButtons(ByVal sID As String)
        Try
            Dim guiActions As Generic.List(Of roGuiAction) = PortalServiceMethods.GetGuiActions(Nothing, "Portal\GeneralManagement\Terminal\List", WLHelperWeb.CurrentPassportID)

            Dim oResponse As roTools.PageCustomActions = roTools.BuildPageCustomActions(guiActions, sID, Me.Language, Me.DefaultScope, "Zones")
            Response.Write(roJSONHelper.SerializeNewtonSoft(oResponse))
        Catch ex As Exception
            Response.Write(ex.StackTrace.ToString)
        End Try
    End Sub

    Private Sub LoadTerminalsListDataTab()
        Try

            Dim intActiveTab As Integer = roTypes.Any2Integer(Request("aTab"))

            Dim oMainDiv As New HtmlGenericControl("div")

            Dim oImageDiv As New HtmlGenericControl("div")
            oImageDiv.Attributes("class") = "blackRibbonIcon"

            Dim img As HtmlImage = New HtmlImage
            img.Src = "Images/Terminals80.png"
            oImageDiv.Controls.Add(img)

            Dim oTextDiv As New HtmlGenericControl("div")
            oTextDiv.Attributes("class") = "blackRibbonDescription"
            oTextDiv.InnerHtml = "<div id=""NameReadOnly""><a href=""javascript: void(0);"" onclick="""" style=""display: block;cursor: text;"" title=""""><span id=""readOnlyNameAccessZones"" class=""NameText"">" & Me.Language.Translate("TerminalsList.Terminal", Me.DefaultScope) & " </span></a></div>" & _
                                "<div id=""NameChange"" style=""display: none;""><table><tr><td><input type=""text"" id=""txtName"" style=""width: 350px;"" value="""" class=""inputNameText"" ConvertControl=""TextField"" CCallowBlank=""false"" onblur="""" ></td>" & _
                                "</tr></table></div>"

            Dim oButtonsDiv As New HtmlGenericControl("div")
            oButtonsDiv.Attributes("class") = "blackRibbonButtons"
            oButtonsDiv.Controls.Add(Me.CreateTabs(intActiveTab))

            oMainDiv.Controls.Add(oImageDiv)
            oMainDiv.Controls.Add(oTextDiv)
            oMainDiv.Controls.Add(oButtonsDiv)

            Dim sw As New IO.StringWriter
            Dim htw As New HtmlTextWriter(sw)
            oMainDiv.RenderControl(htw)

            Response.Write(sw.ToString)
        Catch ex As Exception
            Response.Write(ex.StackTrace.ToString)
        End Try
    End Sub

    Private Function CreateTabs(ByVal activeTab As Integer) As HtmlTable
        Dim hTableGen As New HtmlTable
        Dim hRowGen As New HtmlTableRow
        Dim hCellGen As New HtmlTableCell

        Dim hTableButtons As New HtmlTable
        Dim hTableRowButtons As New HtmlTableRow
        Dim hTableCellButtons As New HtmlTableCell

        hTableGen.Border = 0
        hTableGen.CellSpacing = 0
        hTableGen.CellPadding = 0

        hTableButtons.Border = 0
        hTableButtons.CellSpacing = 0
        hTableButtons.CellPadding = 0

        Dim oTabButtons() As HtmlAnchor = {Nothing}

        oTabButtons(0) = CreateNewHtmlAnchor("TABBUTTON_00", Me.Language.Translate("tabGeneral", Me.DefaultScope), "bTab")
        hTableCellButtons.Controls.Add(oTabButtons(0))
        hTableRowButtons.Cells.Add(hTableCellButtons)
        hTableButtons.Rows.Add(hTableRowButtons)
        hTableCellButtons = New HtmlTableCell
        hTableRowButtons = New HtmlTableRow

        ' Mostramos pestaña de conector de fichajes si tenemos licencia para ello
        If API.LicenseServiceMethods.FeatureIsInstalled("Feature\PunchConnector") Then
            ReDim Preserve oTabButtons(1)
            oTabButtons(1) = Nothing

            oTabButtons(1) = CreateNewHtmlAnchor("TABBUTTON_01", Me.Language.Translate("tabSalida", Me.DefaultScope), "bTab")
            hTableCellButtons.Controls.Add(oTabButtons(1))
            hTableRowButtons.Cells.Add(hTableCellButtons)
            hTableButtons.Rows.Add(hTableRowButtons)
            hTableCellButtons = New HtmlTableCell
            hTableRowButtons = New HtmlTableRow
        End If

        'Afegim a la taula principal
        hCellGen.Controls.Add(hTableButtons)
        hRowGen.Cells.Add(hCellGen)
        hTableGen.Rows.Add(hRowGen)

        For n As Integer = 0 To oTabButtons.Length - 1
            oTabButtons(n).Attributes.Add("OnClick", "javascript: changeTabs(" & n.ToString & ");")
        Next

        oTabButtons(activeTab).Attributes("class") = "bTab-active"

        Return hTableGen ' Retorna el HTMLTable

    End Function

    Private Function CreateNewHtmlAnchor(ByVal Name As String, ByVal Text As String, ByVal CssClassPrefix As String) As HtmlAnchor
        Dim obutton As New HtmlAnchor
        obutton.ID = Name
        obutton.HRef = "javascript: void(0);"
        obutton.Attributes("class") = CssClassPrefix
        obutton.InnerHtml = Text
        Return obutton
    End Function

#End Region


End Class