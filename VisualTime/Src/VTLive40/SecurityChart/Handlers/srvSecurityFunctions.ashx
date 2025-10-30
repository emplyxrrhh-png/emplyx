<%@ WebHandler Language="VB" Class="srvSecurityFunctions" %>

Imports System
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Base.DTOs
Imports Robotics.Web.Base.API

Public Class srvSecurityFunctions
    Inherits handlerBase

    Private oPermission As Permission

    Public Overrides Sub ProcessRoboticsRequest(ByVal context As HttpContext)

        Me.oPermission = Me.GetFeaturePermission("Administration.Security")

        If oPermission > Permission.None Then
            Select Case context.Request("action")
                Case "getSecurityFunctionTab"
                    LoadSecurityFunctionDataTab()
                Case "deleteSecurityFunction"
                    DeleteSecurityFunction(Request("ID"))
                Case "getBarButtons"
                    GetBarButtons(Request("ID"))
                Case "copySecurityFunction"
                    CopySecurityFunction(Request("ID"))
            End Select

        Else
            Dim strResponse As String = "MESSAGE" & _
                          "TitleKey=CheckPermission.Denied.Title&" + _
                          "DescriptionKey=CheckPermission.Denied.Description&" + _
                          "Option1TextKey=CheckPermission.Denied.Option1Text&" + _
                          "Option1DescriptionKey=CheckPermission.Denied.Option1Description&" + _
                          "Option1OnClickScript=HideMsgBoxForm(); window.location = '" & WLHelperWeb.DefaultRedirectUrl & "' return false;&" + _
                          "IconUrl=" & HelperWeb.MsgBoxIconsUrl(HelperWeb.MsgBoxIcons.AlertIcon)
            Dim rError As New roJSON.JSONError(True, Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope))
            Response.Write(rError.toJSON)
        End If

    End Sub

    Private Sub LoadSecurityFunctionDataTab()
        Try

            Dim oCurrentSecuritytab As roGroupFeature

            If Request("ID") = "-1" Then
                oCurrentSecuritytab = New roGroupFeature
            Else
                oCurrentSecuritytab = API.SecurityChartServiceMethods.GetGroupFeatureByID(Request("ID"), Nothing)
            End If

            If oCurrentSecuritytab Is Nothing Then Exit Sub

            Dim intActiveTab As Integer = roTypes.Any2Integer(Request("aTab"))
            If intActiveTab > 1 Then intActiveTab = 0

            Dim oMainDiv As New HtmlGenericControl("div")

            Dim oImageDiv As New HtmlGenericControl("div")
            oImageDiv.Attributes("class") = "blackRibbonIcon"

            Dim img As HtmlImage = New HtmlImage
            img.Src = "Images/SecurityFunctions80.png"
            oImageDiv.Controls.Add(img)

            Dim oTextDiv As New HtmlGenericControl("div")
            oTextDiv.Attributes("class") = "blackRibbonDescription"
            oTextDiv.InnerHtml = "<div id=""NameReadOnly""><span id=""readOnlyNameSecurityFunction"" class=""NameText"">" & oCurrentSecuritytab.Name & "</span></div>"

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
            Response.Write("MESSAGE" & _
                           "TitleKey=SaveName.Error.Text&" & _
                           "DescriptionText=" & ex.ToString & "&" & _
                           "Option1TextKey=SaveName.Error.Option1Text&" & _
                           "Option1DescriptionKey=SaveName.Error.Option1Description&" & _
                           "Option1OnClickScript=HideMsgBoxForm(); return false;&" & _
                           "IconUrl=" & HelperWeb.MsgBoxIconsUrl(HelperWeb.MsgBoxIcons.ErrorIcon))
        End Try
    End Sub

    Private Function CreateTabs(ByRef intActiveTab As Integer) As HtmlTable
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

        Dim oTabButtons() As HtmlAnchor = {Nothing, Nothing}

        oTabButtons(0) = CreateNewHtmlAnchor("TABBUTTON_00", Me.Language.Translate("tabGeneral", Me.DefaultScope), "bTab")
        hTableCellButtons.Controls.Add(oTabButtons(0))
        hTableRowButtons.Cells.Add(hTableCellButtons)
        hTableButtons.Rows.Add(hTableRowButtons)
        hTableCellButtons = New HtmlTableCell
        hTableRowButtons = New HtmlTableRow

        oTabButtons(1) = CreateNewHtmlAnchor("TABBUTTON_01", Me.Language.Translate("tabPermissions", Me.DefaultScope), "bTab")
        hTableCellButtons.Controls.Add(oTabButtons(1))
        hTableRowButtons.Cells.Add(hTableCellButtons)
        hTableButtons.Rows.Add(hTableRowButtons)
        hTableCellButtons = New HtmlTableCell
        hTableRowButtons = New HtmlTableRow

        'Afegim a la taula principal
        hCellGen.Controls.Add(hTableButtons)
        hRowGen.Cells.Add(hCellGen)
        hTableGen.Rows.Add(hRowGen)

        For n As Integer = 0 To oTabButtons.Length - 1
            If n = 0 Then
                oTabButtons(n).Attributes.Add("OnClick", "javascript: changeTabs(" & n.ToString & ");")
            Else
                oTabButtons(n).Attributes.Add("OnClick", "javascript: changeTabs(" & n.ToString & ");")
            End If
        Next

        oTabButtons(intActiveTab).Attributes("class") = "bTab-active"

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
            Dim guiActions As Generic.List(Of roGuiAction) = PortalServiceMethods.GetGuiActions(Nothing, "Portal\Security\SecurityChart\SecurityFunctions", WLHelperWeb.CurrentPassportID)

            Dim oResponse As roTools.PageCustomActions = roTools.BuildPageCustomActions(guiActions, sID, Me.Language, Me.DefaultScope, "SecurityFunctions")
            Response.Write(roJSONHelper.SerializeNewtonSoft(oResponse))
        Catch ex As Exception
            Response.Write(ex.StackTrace.ToString)
        End Try
    End Sub

    Private Sub DeleteSecurityFunction(ByVal oID As Integer)
        Try
            Dim rError As roJSON.JSONError

            'Check Permissions
            If Me.oPermission < Permission.Admin Then
                rError = New roJSON.JSONError(True, Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope))
                Response.Write(rError.toJSON)
                Exit Sub
            End If

            Dim oCurrentSecuritytab As roGroupFeature

            If Request("ID") = "-1" Then
                oCurrentSecuritytab = New roGroupFeature
            Else
                oCurrentSecuritytab = API.SecurityChartServiceMethods.GetGroupFeatureByID(Request("ID"), Nothing)
            End If

            If oCurrentSecuritytab Is Nothing Then Exit Sub

            If API.SecurityChartServiceMethods.DeleteGroupFeature(oCurrentSecuritytab, Nothing) = False Then
                rError = New roJSON.JSONError(True, roWsUserManagement.SessionObject.States.GroupFeatureState.ErrorText)
            Else
                rError = New roJSON.JSONError(False, "OK")
            End If


            Response.Write(rError.toJSON)

        Catch ex As Exception
            Dim rError As New roJSON.JSONError(True, ex.Message.ToString)
            Response.Write(rError.toJSON)
        End Try
    End Sub

    Private Sub CopySecurityFunction(ByVal oID As Integer)
        Try
            Dim rError As roJSON.JSONError

            'Check Permissions
            If Me.oPermission < Permission.Admin Then
                rError = New roJSON.JSONError(True, Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope))
                Response.Write(rError.toJSON)
                Exit Sub
            End If

            If API.SecurityChartServiceMethods.CopyGroupFeature(Nothing, oID) = False Then
                rError = New roJSON.JSONError(True, roWsUserManagement.SessionObject.States.GroupFeatureState.ErrorText)
            Else
                rError = New roJSON.JSONError(False, "OK")
            End If


            Response.Write(rError.toJSON)

        Catch ex As Exception
            Dim rError As New roJSON.JSONError(True, ex.Message.ToString)
            Response.Write(rError.toJSON)
        End Try
    End Sub

End Class