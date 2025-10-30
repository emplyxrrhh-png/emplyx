<%@ WebHandler Language="VB" Class="srvTerminals" %>

Imports System
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports System.Data
Imports Robotics.Base.DTOs
Imports Robotics.Web.Base.API
Imports Robotics.Base.VTBusiness.Terminal

Public Class srvTerminals
    Inherits handlerBase

    Private oPermission As Permission
    Private Const FeatureAlias As String = "Terminals.Definition"

    Public Overrides Sub ProcessRoboticsRequest(ByVal context As HttpContext)
        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)

        If Me.oPermission > Permission.None Then
            Select Case Request("action")
                Case "getTerminalTab" ' Retorna la capcelera de la plana
                    LoadTerminalDataTab()
                Case "getBarButtons"
                    GetBarButtons(Request("ID"))
                Case "deleteXTerminal"
                    Me.DeleteTerminal(Request("ID"))
                Case "launchBroadcaster"
                    LaunchBroadcaster()

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

    Private Sub LaunchBroadcaster()
        Dim bolRet As Boolean = False
        Try

            Dim IDTerminal As Integer = roTypes.Any2Integer(Request("IDTerminal"))

            bolRet = Robotics.Web.Base.API.ConnectorServiceMethods.LaunchBroadcasterForTerminal(Nothing, IDTerminal)

        Catch ex As Exception
        Finally
            If bolRet Then
                Response.Write("OK")
            Else
                Response.Write("NOK")
            End If
        End Try

    End Sub

    ''' <summary>
    ''' Eliminacio del terminal
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DeleteTerminal(ByVal oID As Integer)
        Try
            Dim rError As roJSON.JSONError

            'Check Permissions
            If Me.oPermission < Permission.Admin Then
                rError = New roJSON.JSONError(True, Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope))
                Response.Write(rError.toJSON)
                Exit Sub
            End If

            If oID = -1 Then Exit Sub

            If API.TerminalServiceMethods.DeleteTerminal(Nothing, oID, True) = False Then
                rError = New roJSON.JSONError(True, roWsUserManagement.SessionObject.States.TerminalState.ErrorText)
            Else
                rError = New roJSON.JSONError(False, "OK")
            End If
            Response.Write(rError.toJSON)
        Catch ex As Exception
            Dim rError As New roJSON.JSONError(True, ex.Message.ToString & ex.StackTrace.ToString)
            Response.Write(rError.toJSON)
        End Try
    End Sub

    ''' <summary>
    ''' Carrega Exportacio per ID (sols el Tab superior)
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadTerminalDataTab()
        Try

            Dim oCurrentTerminal As roTerminal

            If Request("ID") = "-1" Then
                oCurrentTerminal = New roTerminal
            Else
                oCurrentTerminal = API.TerminalServiceMethods.GetTerminal(Nothing, Request("ID"), False)
            End If

            If oCurrentTerminal Is Nothing Then Exit Sub
            Dim intActiveTab As Integer = roTypes.Any2Integer(Request("aTab"))

            Dim oMainDiv As New HtmlGenericControl("div")

            Dim oImageDiv As New HtmlGenericControl("div")
            oImageDiv.Attributes("class") = "blackRibbonIcon"

            Dim img As HtmlImage = New HtmlImage

            Dim pathImg As String = "Images/TerminalIcos/"
            Dim strImg As String = "BLANK"
            If oCurrentTerminal.Type <> "" Then strImg = oCurrentTerminal.Type
            If oCurrentTerminal.LastStatus.ToUpper <> "OK" Then strImg &= "_DIS"

            'Permisos Info. Estat
            If oPermission = Permission.None Then strImg = strImg.Replace("_DIS", "")

            img.Src = pathImg & strImg & ".png"
            img.Height = 80
            oImageDiv.Controls.Add(img)

            Dim oTextDiv As New HtmlGenericControl("div")
            oTextDiv.Attributes("class") = "blackRibbonDescription"
            oTextDiv.InnerHtml = "<div id=""NameReadOnly""><span id=""readOnlyNameTerminal"" class=""NameText"">" & oCurrentTerminal.Description & " </span></div>"

            Dim oButtonsDiv As New HtmlGenericControl("div")
            oButtonsDiv.Attributes("class") = "blackRibbonButtons"
            oButtonsDiv.Controls.Add(Me.CreateTabs(oCurrentTerminal, intActiveTab))

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

    Private Function CreateTabs(ByVal oCurrentTerminal As roTerminal, ByVal activeTab As Integer) As HtmlTable
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

        oTabButtons(0) = CreateNewHtmlAnchor("TABBUTTON_00", Me.Language.Translate("tabConfiguration", Me.DefaultScope), "bTab")
        hTableCellButtons.Controls.Add(oTabButtons(0))
        hTableRowButtons.Cells.Add(hTableCellButtons)
        hTableButtons.Rows.Add(hTableRowButtons)
        hTableCellButtons = New HtmlTableCell
        hTableRowButtons = New HtmlTableRow

        If oCurrentTerminal.Type <> "NFC" Then

            ReDim Preserve oTabButtons(1)
            oTabButtons(1) = Nothing

            oTabButtons(1) = CreateNewHtmlAnchor("TABBUTTON_01", Me.Language.Translate("tabReaders", Me.DefaultScope), "bTab")
            hTableCellButtons.Controls.Add(oTabButtons(1))
            hTableRowButtons.Cells.Add(hTableCellButtons)
            hTableButtons.Rows.Add(hTableRowButtons)
            hTableCellButtons = New HtmlTableCell
            hTableRowButtons = New HtmlTableRow

            If oCurrentTerminal.SupportedSirens IsNot Nothing AndAlso oCurrentTerminal.SupportedSirens <> "" Then

                ReDim Preserve oTabButtons(2)
                oTabButtons(2) = Nothing

                oTabButtons(2) = CreateNewHtmlAnchor("TABBUTTON_02", Me.Language.Translate("tabSirens", Me.DefaultScope), "bTab")
                hTableCellButtons.Controls.Add(oTabButtons(2))
                hTableRowButtons.Cells.Add(hTableCellButtons)
                hTableButtons.Rows.Add(hTableRowButtons)
                hTableCellButtons = New HtmlTableCell
                hTableRowButtons = New HtmlTableRow

            Else
                If activeTab = 2 Then activeTab = 0
            End If

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

    Private Sub GetBarButtons(ByVal sID As String)
        Try
            Dim guiActions As Generic.List(Of roGuiAction) = PortalServiceMethods.GetGuiActions(Nothing, "Portal\GeneralManagement\Terminal\Terminals", WLHelperWeb.CurrentPassportID)

            Dim oResponse As roTools.PageCustomActions = roTools.BuildPageCustomActions(guiActions, sID, Me.Language, Me.DefaultScope, "Employees")
            Response.Write(roJSONHelper.SerializeNewtonSoft(oResponse))
        Catch ex As Exception
            Response.Write(ex.StackTrace.ToString)
        End Try
    End Sub
#End Region


End Class