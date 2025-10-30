<%@ WebHandler Language="VB" Class="srvConcepts" %>

Imports System
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Base.DTOs
Imports Robotics.Web.Base.API
Imports Robotics.Base.VTBusiness.Concept

Public Class srvConcepts
    Inherits handlerBase

    Private oPermission As Permission
    Private strActiveTab As String = "TABBUTTON_General"

    Public Overrides Sub ProcessRoboticsRequest(ByVal context As HttpContext)
        Me.oPermission = Me.GetFeaturePermission("Concepts.Definition")

        If Me.oPermission > Permission.None Then
            Select Case Request("action")
                Case "getConceptTab" ' Retorna la capcelera de la plana
                    LoadConceptDataTab()
                Case "getBarButtons"
                    GetBarButtons(Request("ID"))
                Case "deleteXConcept"
                    DeleteConceptDataX(Request("ID"))
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
    Private Sub DeleteConceptDataX(ByVal oID As Integer)
        Dim strResponse As String = "OK"
        Dim strError As String = ""

        Try
            'Check Permissions
            If Me.oPermission < Permission.Admin Then
                strError = Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope)
            End If

            If oID = -1 Then strError = "KO"

            If strError = String.Empty Then
                If API.ConceptsServiceMethods.DeleteConcept(Nothing, oID, True) = False Then
                    strError = roWsUserManagement.SessionObject.States.ConceptsState.ErrorText
                Else
                    strResponse = "OK"
                End If
            End If

        Catch ex As Exception
            strError = ex.Message.ToString
        End Try

        If strError <> String.Empty Then
            strResponse = "MESSAGE" + _
                          "TitleKey=SaveName.Error.Title&" + _
                          "DescriptionText=" + strError + "&" + _
                          "Option1TextKey=SaveName.Error.Option1Text&" + _
                          "Option1DescriptionKey=SaveName.Error.Option1Description&" + _
                          "Option1OnClickScript=HideMsgBoxForm(); return false;&" + _
                          "IconUrl=" & HelperWeb.MsgBoxIconsUrl(HelperWeb.MsgBoxIcons.ErrorIcon)
        End If

        Response.Write(strResponse)
    End Sub

    ''' <summary>
    ''' Carrega roConcept per ID (sols el Tab superior)
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadConceptDataTab()
        Try

            Dim intActiveTab As Integer = roTypes.Any2Integer(Request("aTab"))

            Dim oMainDiv As New HtmlGenericControl("div")

            Dim oImageDiv As New HtmlGenericControl("div")
            oImageDiv.Attributes("class") = "blackRibbonIcon"

            Dim img As HtmlImage = New HtmlImage
            img.Src = "Images/acumulados80.png"
            oImageDiv.Controls.Add(img)

            Dim oTextDiv As New HtmlGenericControl("div")
            oTextDiv.Attributes("class") = "blackRibbonDescription"
            oTextDiv.InnerHtml = "<div id=""NameReadOnlyConcept""><span id=""readOnlyNameConcept"" class=""NameText""></span></div>"

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

    ''' <summary>
    ''' Genera els botons de la dreta (General, ...)
    ''' </summary>
    ''' <returns>Retorna un HTML Table amb els botons en format columna</returns>
    ''' <remarks></remarks>
    Private Function CreateTabs(ByVal intActiveTab As Integer) As HtmlTable
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

        Dim NumButtons As Byte = 0
        oTabButtons(NumButtons) = CreateNewHtmlAnchor("TABBUTTON_0" & NumButtons, Me.Language.Translate("tabGeneral", Me.DefaultScope), "bTab")
        hTableCellButtons.Controls.Add(oTabButtons(NumButtons))
        hTableRowButtons.Cells.Add(hTableCellButtons)
        hTableButtons.Rows.Add(hTableRowButtons)
        hTableCellButtons = New HtmlTableCell
        hTableRowButtons = New HtmlTableRow

        NumButtons = NumButtons + 1
        ReDim Preserve oTabButtons(NumButtons)
        oTabButtons(NumButtons) = CreateNewHtmlAnchor("TABBUTTON_0" & NumButtons, Me.Language.Translate("tabType", Me.DefaultScope), "bTab")
        hTableCellButtons.Controls.Add(oTabButtons(NumButtons))
        hTableRowButtons.Cells.Add(hTableCellButtons)
        hTableButtons.Rows.Add(hTableRowButtons)
        hTableCellButtons = New HtmlTableCell
        hTableRowButtons = New HtmlTableRow

        NumButtons = NumButtons + 1
        ReDim Preserve oTabButtons(NumButtons)
        oTabButtons(NumButtons) = CreateNewHtmlAnchor("TABBUTTON_0" & NumButtons, Me.Language.Translate("tabComposition", Me.DefaultScope), "bTab")
        hTableCellButtons.Controls.Add(oTabButtons(NumButtons))
        hTableRowButtons.Cells.Add(hTableCellButtons)
        hTableButtons.Rows.Add(hTableRowButtons)
        hTableCellButtons = New HtmlTableCell
        hTableRowButtons = New HtmlTableRow

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

        NumButtons = NumButtons + 1
        ReDim Preserve oTabButtons(NumButtons)
        oTabButtons(NumButtons) = CreateNewHtmlAnchor("TABBUTTON_0" & NumButtons, Me.Language.Translate("tabRound", Me.DefaultScope), "bTab")
        hTableCellButtons.Controls.Add(oTabButtons(NumButtons))
        hTableRowButtons.Cells.Add(hTableCellButtons)
        hTableButtons.Rows.Add(hTableRowButtons)
        hTableCellButtons = New HtmlTableCell
        hTableRowButtons = New HtmlTableRow

        NumButtons = NumButtons + 1
        ReDim Preserve oTabButtons(NumButtons)
        oTabButtons(NumButtons) = CreateNewHtmlAnchor("TABBUTTON_0" & NumButtons, Me.Language.Translate("tabQuerys", Me.DefaultScope), "bTab")
        hTableCellButtons.Controls.Add(oTabButtons(NumButtons))
        hTableRowButtons.Cells.Add(hTableCellButtons)
        hTableButtons.Rows.Add(hTableRowButtons)
        hTableCellButtons = New HtmlTableCell
        hTableRowButtons = New HtmlTableRow

        NumButtons = NumButtons + 1
        ReDim Preserve oTabButtons(NumButtons)
        oTabButtons(NumButtons) = CreateNewHtmlAnchor("TABBUTTON_0" & NumButtons, Me.Language.Translate("tabPays", Me.DefaultScope), "bTab")
        hTableCellButtons.Controls.Add(oTabButtons(NumButtons))
        hTableRowButtons.Cells.Add(hTableCellButtons)
        hTableButtons.Rows.Add(hTableRowButtons)
        hTableCellButtons = New HtmlTableCell
        hTableRowButtons = New HtmlTableRow


        'Aqui partim en 3 columnes els TABS...
        hCellGen.Controls.Add(hTableButtons)
        hRowGen.Cells.Add(hCellGen)

        hCellGen = New HtmlTableCell

        'Regenerem la taula
        hTableButtons = New HtmlTable
        hTableRowButtons = New HtmlTableRow
        hTableCellButtons = New HtmlTableCell

        hTableButtons.Border = 0
        hTableButtons.CellSpacing = 0
        hTableButtons.CellPadding = 0

        NumButtons = NumButtons + 1
        ReDim Preserve oTabButtons(NumButtons)
        oTabButtons(NumButtons) = CreateNewHtmlAnchor("TABBUTTON_0" & NumButtons, Me.Language.Translate("tabAbsentism", Me.DefaultScope), "bTab")
        hTableCellButtons.Controls.Add(oTabButtons(NumButtons))
        hTableRowButtons.Cells.Add(hTableCellButtons)
        hTableButtons.Rows.Add(hTableRowButtons)
        hTableCellButtons = New HtmlTableCell
        hTableRowButtons = New HtmlTableRow

        NumButtons = NumButtons + 1
        ReDim Preserve oTabButtons(NumButtons)
        oTabButtons(NumButtons) = CreateNewHtmlAnchor("TABBUTTON_0" & NumButtons, Me.Language.Translate("tabValidationPeriod", Me.DefaultScope), "bTab")
        hTableCellButtons.Controls.Add(oTabButtons(NumButtons))
        hTableRowButtons.Cells.Add(hTableCellButtons)
        hTableButtons.Rows.Add(hTableRowButtons)
        hTableCellButtons = New HtmlTableCell
        hTableRowButtons = New HtmlTableRow

        NumButtons = NumButtons + 1
        ReDim Preserve oTabButtons(NumButtons)
        oTabButtons(NumButtons) = CreateNewHtmlAnchor("TABBUTTON_0" & NumButtons, Me.Language.Translate("tabAutomaticAccrual", Me.DefaultScope), "bTab")
        hTableCellButtons.Controls.Add(oTabButtons(NumButtons))
        hTableRowButtons.Cells.Add(hTableCellButtons)
        hTableButtons.Rows.Add(hTableRowButtons)
        hTableCellButtons = New HtmlTableCell
        hTableRowButtons = New HtmlTableRow

        'Afegim a la taula principal
        hCellGen.Attributes("valign") = "top"
        hCellGen.Controls.Add(hTableButtons)
        hRowGen.Cells.Add(hCellGen)
        hTableGen.Rows.Add(hRowGen)

        For n As Integer = 0 To oTabButtons.Length - 1
            If Not oTabButtons(n) Is Nothing Then
                oTabButtons(n).Attributes.Add("OnClick", "javascript: changeTabs(" & n.ToString & ");")
            End If
        Next

        oTabButtons(intActiveTab).Attributes("class") = "bTab-active"

        Return hTableGen ' Retorna el HTMLTable

    End Function

    ''' <summary>
    ''' Genera automaticament HtmlAnchors
    ''' </summary>
    ''' <param name="Name">Nom del boton (ID)</param>
    ''' <param name="Text">Texte (InnerText)</param>
    ''' <param name="CssClassPrefix">No es fa servir...</param>
    ''' <returns>un HTMLButton</returns>
    ''' <remarks></remarks>
    Private Function CreateNewHtmlAnchor(ByVal Name As String, ByVal Text As String, ByVal CssClassPrefix As String) As HtmlAnchor
        Dim obutton As New HtmlAnchor
        obutton.ID = Name
        obutton.HRef = "javascript: void(0);"
        obutton.Attributes("class") = CssClassPrefix
        obutton.InnerHtml = Text
        Return obutton
    End Function

    ''' <summary>
    ''' Crea la barra d'eines que va al TAB de la capcelera
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub GetBarButtons(ByVal sID As String)
        Try

            Dim guiActions As Generic.List(Of roGuiAction) = PortalServiceMethods.GetGuiActions(Nothing, "Portal\ShiftManagement\Accruals\Accruals", WLHelperWeb.CurrentPassportID)


            Dim oResponse As roTools.PageCustomActions = roTools.BuildPageCustomActions(guiActions, sID, Me.Language, Me.DefaultScope, "Concepts")
            Response.Write(roJSONHelper.SerializeNewtonSoft(oResponse))
        Catch ex As Exception
            Response.Write(ex.StackTrace.ToString)
        End Try
    End Sub
#End Region


End Class