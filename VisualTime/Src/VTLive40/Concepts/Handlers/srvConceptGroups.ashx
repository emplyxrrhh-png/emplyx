<%@ WebHandler Language="VB" Class="srvConceptGroups" %>

Imports System
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Base.DTOs
Imports Robotics.Web.Base.API
Imports Robotics.Base.VTBusiness.Concept

Public Class srvConceptGroups
    Inherits handlerBase

    Private Const FeatureAlias As String = "Concepts.Definition"
    Private oPermission As Permission

    Public Overrides Sub ProcessRoboticsRequest(ByVal context As HttpContext)
        Me.scope = "Concepts"
        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)

        If Me.oPermission > Permission.None Then

            Select Case context.Request("action")
                Case "getConceptGroupTab" ' Retorna la capcelera de la plana
                    LoadConceptGroupDataTab()
                Case "getBarButtons"
                    GetBarButtons(Request("ID"))
                Case "chgName"
                    changeConceptGroupName(Request("ID"))
                Case "deleteXConceptGroup" ' Elimina un nou Concept (JSON)
                    DeleteConceptGroupDataX(Request("ID"))
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

    ''' <summary>
    ''' Cambia el nom del Concept
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub changeConceptGroupName(ByVal IDConceptGroup As Integer)
        Try
            Dim rError As roJSON.JSONError

            Dim oCurrentConceptGroup As roConceptGroup = API.ConceptsServiceMethods.GetConceptGroupByID(Nothing, IDConceptGroup, False)

            If oCurrentConceptGroup Is Nothing Then Exit Sub

            Dim NouName As String = Request("NewName")
            If NouName = "" Then
                rError = New roJSON.JSONError(True, Me.Language.Translate("NameInvalid", Me.DefaultScope))
                Response.Write(rError.toJSON)
                Exit Sub
            End If
            oCurrentConceptGroup.Name = NouName
            API.ConceptsServiceMethods.SaveConceptGroup(Nothing, oCurrentConceptGroup, True)
            Response.Write("OK")
        Catch ex As Exception


        End Try
    End Sub

    ''' <summary>
    ''' Elimina el concept
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DeleteConceptGroupDataX(ByVal oID As Integer)
        Dim strResponse As String = "OK"
        Dim strError As String = ""

        Try
            If oID = -1 Then strError = "KO"

            If strError = String.Empty Then
                If API.ConceptsServiceMethods.DeleteConceptGroup(Nothing, oID, True) = False Then
                    strError = API.ConceptsServiceMethods.LastErrorText
                Else
                    strResponse = "OK"
                End If
            End If

        Catch ex As Exception
            strError = ex.Message.ToString
        End Try

        If strError <> String.Empty Then
            strResponse = "MESSAGE" + _
                          "TitleKey=SaveConceptGroup.Error.Title&" + _
                          "DescriptionText=" + strError + "&" + _
                          "Option1TextKey=SaveConceptGroup.Error.Option1Text&" + _
                          "Option1DescriptionKey=SaveConceptGroup.Error.Option1Description&" + _
                          "Option1OnClickScript=HideMsgBoxForm(); return false;&" + _
                          "IconUrl=" & HelperWeb.MsgBoxIconsUrl(HelperWeb.MsgBoxIcons.ErrorIcon)
        End If

        Response.Write(strResponse)
    End Sub

    ''' <summary>
    ''' Carrega roConcept per ID (sols el Tab superior)
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadConceptGroupDataTab()
        Try

            Dim oMainDiv As New HtmlGenericControl("div")

            Dim oImageDiv As New HtmlGenericControl("div")
            oImageDiv.Attributes("class") = "blackRibbonIcon"

            Dim img As HtmlImage = New HtmlImage
            img.Src = VirtualPathUtility.ToAbsolute("~/Base/Images/StartMenuIcos/ConceptGroups.png")
            oImageDiv.Controls.Add(img)

            Dim oTextDiv As New HtmlGenericControl("div")
            oTextDiv.Attributes("class") = "blackRibbonDescription"
            oTextDiv.InnerHtml = "<div id=""NameReadOnlyConceptGroup""><span id=""readOnlyNameConceptGroup"" class=""NameText""></span></div>"

            Dim oButtonsDiv As New HtmlGenericControl("div")
            oButtonsDiv.Attributes("class") = "blackRibbonButtons"
            oButtonsDiv.Controls.Add(Me.CreateTabs)

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
    Private Function CreateTabs() As HtmlTable
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

        oTabButtons(0) = CreateNewHtmlAnchor("TABBUTTON_20", Me.Language.Translate("tabGeneral", Me.DefaultScope), "bTab")
        hTableCellButtons.Controls.Add(oTabButtons(0))
        hTableCellButtons.Attributes("valign") = "top"
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
            oTabButtons(n).Attributes.Add("OnClick", "javascript: changeTabs(" & n.ToString & ");")
        Next

        oTabButtons(0).Attributes("class") = "bTab-active"

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
            Dim guiActions As Generic.List(Of roGuiAction) = PortalServiceMethods.GetGuiActions(Nothing, "Portal\ShiftManagement\Accruals\Groups", WLHelperWeb.CurrentPassportID)


            Dim oResponse As roTools.PageCustomActions = roTools.BuildPageCustomActions(guiActions, sID, Me.Language, Me.DefaultScope, "Concepts")
            Response.Write(roJSONHelper.SerializeNewtonSoft(oResponse))
        Catch ex As Exception
            Response.Write(ex.StackTrace.ToString)
        End Try
    End Sub

#End Region


End Class