<%@ WebHandler Language="VB" Class="srvCauses" %>

Imports System
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Cause

Public Class srvCauses
    Inherits handlerBase

    Private oPermission As Permission
    Private strActiveTab As String = "TABBUTTON_General"

    Public Overrides Sub ProcessRoboticsRequest(ByVal context As HttpContext)
        Me.oPermission = Me.GetFeaturePermission("Causes.Definition")

        If Me.oPermission > Permission.None Then
            Select Case Request("action")
                Case "getCauseTab" ' Retorna la capcelera de la plana
                    LoadCauseDataTab()
                Case "getBarButtons"
                    GetBarButtons(Request("ID"))
                Case "deleteXCause" ' Elimina un Cause (JSON)
                    DeleteCauseDataX(Request("ID"))
                Case "causeIsUsed"
                    CheckIfCauseIsUsed(Request("ID"))
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

    Private Sub CheckIfCauseIsUsed(ByVal oID As String)
        Dim rError As roJSON.JSONError
        Dim bolCauseUsed As Boolean = False

        Try
            'Check Permissions
            If Me.oPermission < Permission.Write Then
                rError = New roJSON.JSONError(True, Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope))
                Response.Write(rError.toJSON)
                Exit Sub
            End If

            If CInt(oID) = -1 Then
                rError = New roJSON.JSONError(False, "CauseUsedNo")
            Else
                'If CausesServiceMethods.CauseIsUsed(Nothing, oID) = True Then
                bolCauseUsed = True
                'End If
            End If

            If bolCauseUsed = True Then
                rError = New roJSON.JSONError(False, "CauseUsedYES")
            Else
                rError = New roJSON.JSONError(False, "CauseUsedNo")
            End If

            Response.Write(rError.toJSON)
        Catch ex As Exception
            rError = New roJSON.JSONError(True, ex.Message.ToString & " " & ex.StackTrace)
            Response.Write(rError.toJSON)
        End Try
    End Sub

    Private Sub DeleteCauseDataX(ByVal oID As Integer)
        Try
            Dim rError As roJSON.JSONError

            'Check Permissions
            If Me.oPermission < Permission.Admin Then
                rError = New roJSON.JSONError(True, Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope))
                Response.Write(rError.toJSON)
                Exit Sub
            End If

            If oID = -1 Then Exit Sub

            If CausesServiceMethods.DeleteCause(Nothing, oID, True) = False Then
                rError = New roJSON.JSONError(True, roWsUserManagement.SessionObject.States.CauseState.ErrorText)
            Else
                rError = New roJSON.JSONError(False, "OK")
            End If

            Response.Write(rError.toJSON)
        Catch ex As Exception
            Dim rError As New roJSON.JSONError(True, ex.Message.ToString)
            Response.Write(rError.toJSON)
        End Try
    End Sub

    ''' <summary>
    ''' Carrega Exportacio per ID (sols el Tab superior)
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadCauseDataTab()
        Try

            Dim oCurrentCause As roCause

            If Request("ID") = "-1" Then
                oCurrentCause = New roCause
            Else
                oCurrentCause = CausesServiceMethods.GetCauseByID(Nothing, Request("ID"), False)
            End If

            If oCurrentCause Is Nothing Then Exit Sub

            Dim intActiveTab As Integer = roTypes.Any2Integer(Request("aTab"))
            If intActiveTab < 0 Then intActiveTab = 0


            Dim oMainDiv As New HtmlGenericControl("div")

            Dim oImageDiv As New HtmlGenericControl("div")
            oImageDiv.Attributes("class") = "blackRibbonIcon"

            Dim img As HtmlImage = New HtmlImage
            img.Src = "Images/Causes80.png"
            oImageDiv.Controls.Add(img)

            Dim oTextDiv As New HtmlGenericControl("div")
            oTextDiv.Attributes("class") = "blackRibbonDescription"
            oTextDiv.InnerHtml = "<div id=""NameReadOnly""><span id=""readOnlyNameCause"" class=""NameText"">" & oCurrentCause.Name & " </span></div>"

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

        Dim oTabButtons() As HtmlAnchor = {Nothing}
        Dim iNextTabIndex As Integer = 0

        oTabButtons(iNextTabIndex) = CreateNewHtmlAnchor("TABBUTTON_00", Me.Language.Translate("tabGeneral", Me.DefaultScope), "bTab")
        oTabButtons(iNextTabIndex).Attributes.Add("OnClick", "javascript: changeTabs(" & iNextTabIndex.ToString & ");")
        hTableCellButtons.Controls.Add(oTabButtons(iNextTabIndex))
        hTableRowButtons.Cells.Add(hTableCellButtons)
        hTableButtons.Rows.Add(hTableRowButtons)
        hTableCellButtons = New HtmlTableCell
        hTableRowButtons = New HtmlTableRow
        iNextTabIndex = iNextTabIndex + 1

        ReDim Preserve oTabButtons(1)
        oTabButtons(iNextTabIndex) = CreateNewHtmlAnchor("TABBUTTON_01", Me.Language.Translate("tabTypes", Me.DefaultScope), "bTab")
        oTabButtons(iNextTabIndex).Attributes.Add("OnClick", "javascript: changeTabs(" & iNextTabIndex.ToString & ");")
        hTableCellButtons.Controls.Add(oTabButtons(iNextTabIndex))
        hTableCellButtons.Controls.Add(oTabButtons(1))
        hTableRowButtons.Cells.Add(hTableCellButtons)
        hTableButtons.Rows.Add(hTableRowButtons)
        hTableCellButtons = New HtmlTableCell
        hTableRowButtons = New HtmlTableRow
        iNextTabIndex = iNextTabIndex + 1

        If HelperSession.GetFeatureIsInstalledFromApplication("Feature\Absences") = True Then
            ReDim Preserve oTabButtons(2)
            oTabButtons(iNextTabIndex) = CreateNewHtmlAnchor("TABBUTTON_02", Me.Language.Translate("tabDocuments", Me.DefaultScope), "bTab")
            oTabButtons(iNextTabIndex).Attributes.Add("OnClick", "javascript: changeTabs(" & iNextTabIndex.ToString & ");")
            hTableCellButtons.Controls.Add(oTabButtons(iNextTabIndex))
            hTableRowButtons.Cells.Add(hTableCellButtons)
            hTableButtons.Rows.Add(hTableRowButtons)
            hTableCellButtons = New HtmlTableCell
            hTableRowButtons = New HtmlTableRow
        End If

        iNextTabIndex = iNextTabIndex + 1


        '==========================================
        'Aqui partim en 2 columnes els TABS...
        hCellGen.Controls.Add(hTableButtons)
        hRowGen.Cells.Add(hCellGen)

        hCellGen = New HtmlTableCell
        hCellGen.Attributes("valign") = "top"

        'Regenerem la taula
        hTableButtons = New HtmlTable
        hTableRowButtons = New HtmlTableRow
        hTableCellButtons = New HtmlTableCell

        hTableButtons.Border = 0
        hTableButtons.CellSpacing = 0
        hTableButtons.CellPadding = 0
        '==========================================



        ReDim Preserve oTabButtons(3)
        oTabButtons(iNextTabIndex) = CreateNewHtmlAnchor("TABBUTTON_03", Me.Language.Translate("tabVisibility", Me.DefaultScope), "bTab")
        oTabButtons(iNextTabIndex).Attributes.Add("OnClick", "javascript: changeTabs(" & iNextTabIndex.ToString & ");")
        hTableCellButtons.Controls.Add(oTabButtons(iNextTabIndex))
        hTableRowButtons.Cells.Add(hTableCellButtons)
        hTableButtons.Rows.Add(hTableRowButtons)
        hTableCellButtons = New HtmlTableCell
        hTableRowButtons = New HtmlTableRow
        iNextTabIndex = iNextTabIndex + 1

        ReDim Preserve oTabButtons(4)
        oTabButtons(iNextTabIndex) = CreateNewHtmlAnchor("TABBUTTON_04", Me.Language.Translate("tabRounds", Me.DefaultScope), "bTab")
        oTabButtons(iNextTabIndex).Attributes.Add("OnClick", "javascript: changeTabs(" & iNextTabIndex.ToString & ");")
        hTableCellButtons.Controls.Add(oTabButtons(iNextTabIndex))
        hTableRowButtons.Cells.Add(hTableCellButtons)
        hTableButtons.Rows.Add(hTableRowButtons)
        hTableCellButtons = New HtmlTableCell
        hTableRowButtons = New HtmlTableRow
        iNextTabIndex = iNextTabIndex + 1


        ReDim Preserve oTabButtons(5)
        oTabButtons(iNextTabIndex) = CreateNewHtmlAnchor("TABBUTTON_05", Me.Language.Translate("tabBehaviour", Me.DefaultScope), "bTab")
        oTabButtons(iNextTabIndex).Attributes.Add("OnClick", "javascript: changeTabs(" & iNextTabIndex.ToString & ");")
        hTableCellButtons.Controls.Add(oTabButtons(iNextTabIndex))
        hTableRowButtons.Cells.Add(hTableCellButtons)
        hTableButtons.Rows.Add(hTableRowButtons)
        hTableCellButtons = New HtmlTableCell
        hTableRowButtons = New HtmlTableRow
        iNextTabIndex = iNextTabIndex + 1


        'Afegim a la taula principal
        hCellGen.Controls.Add(hTableButtons)
        hRowGen.Cells.Add(hCellGen)
        hTableGen.Rows.Add(hRowGen)

        For n As Integer = 0 To oTabButtons.Length - 1

        Next

        oTabButtons(intActiveTab).Attributes("class") = "bTab-active"

        Return hTableGen ' Retorna el HTMLTable

    End Function

    Private Sub GetBarButtons(ByVal sID As String)
        Try

            Dim guiActions As Generic.List(Of roGuiAction) = PortalServiceMethods.GetGuiActions(Nothing, "Portal\ShiftManagement\Causes\Management", WLHelperWeb.CurrentPassportID)

            Dim oResponse As roTools.PageCustomActions = roTools.BuildPageCustomActions(guiActions, sID, Me.Language, Me.DefaultScope, "Causes")
            Response.Write(roJSONHelper.SerializeNewtonSoft(oResponse))
        Catch ex As Exception
            Response.Write(ex.StackTrace.ToString)
        End Try
    End Sub

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