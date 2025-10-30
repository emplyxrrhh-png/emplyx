<%@ WebHandler Language="VB" Class="srvTaskTemplates" %>

Imports System
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports System.Data
Imports Robotics.Base.DTOs
Imports Robotics.Web.Base.API
Imports Robotics.Base.VTBusiness.Task

Public Class srvTaskTemplates
    Inherits handlerBase

    Private Const FeatureAlias As String = "Tasks.TemplatesDefinition"
    Private oPermission As Permission

    Public Overrides Sub ProcessRoboticsRequest(ByVal context As HttpContext)
        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)

        If Me.oPermission > Permission.None Then

            Select Case context.Request("action")
                Case "getTaskTemplateTab" ' Retorna la capcelera de la plana
                    LoadTaskTemplateDataTab()
                Case "getBarButtons"
                    GetBarButtons(Request("ID"))
                Case "getEmployeesSelected"
                    GetEmployeesSelected()
                Case "deleteXTaskTemplate" ' Elimina un nou Shift (JSON)
                    DeleteTaskTemplateDataX(Request("ID"))
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
    ''' Elimina el Task template
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DeleteTaskTemplateDataX(ByVal oID As Integer)
        Try
            Dim rError As roJSON.JSONError

            'Check Permissions
            If Me.oPermission < Permission.Admin Then
                rError = New roJSON.JSONError(True, Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope))
                Response.Write(rError.toJSON)
                Exit Sub
            End If

            If oID = -1 Then Exit Sub

            If API.TaskTemplatesServiceMethods.DeleteTaskTemplate(Nothing, oID, True) = False Then
                rError = New roJSON.JSONError(True, roWsUserManagement.SessionObject.States.TaskTemplateState.ErrorText)
            Else
                rError = New roJSON.JSONError(False, "OK")
            End If
            Response.Write(rError.toJSON)
        Catch ex As Exception
            Dim rError As New roJSON.JSONError(True, ex.Message.ToString)
            Response.Write(rError.toJSON)
        End Try
    End Sub

    Private Sub GetEmployeesSelected()
        Try

            'Check Permissions
            If Me.oPermission < Permission.Write Then
                Dim rError As roJSON.JSONError
                rError = New roJSON.JSONError(True, Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope))
                Response.Write(rError.toJSON)
                Exit Sub
            End If

            Dim strNodesSelected As String = roTypes.Any2String(Request("NodesSelected"))
            Dim actualTaskId As Integer = roTypes.Any2Integer(Request("IDTaskTemplate"))

            If strNodesSelected <> String.Empty Then

                Dim strEmployees As String = ""
                Dim strGroups As String = ""

                'A8,B122,B127,A9,B129,B140,B133,B147,A7
                Dim arrNodes = strNodesSelected.Split(",")
                For Each node As String In arrNodes
                    If node.StartsWith("B") Then
                        strEmployees &= node.Substring(1) & ","
                    ElseIf node.StartsWith("A") Then
                        strGroups &= node.Substring(1) & ","
                    End If
                Next

                If strEmployees.Length > 0 Then strEmployees = strEmployees.Substring(0, strEmployees.Length() - 1)
                If strGroups.Length > 0 Then strGroups = strGroups.Substring(0, strGroups.Length() - 1)

                Dim nEmployees As Double = API.TaskTemplatesServiceMethods.GetEmployeesFromTask(Nothing, 0, strEmployees, strGroups)
                Dim rOK As New roJSON.JSONError(False, "OK:" & nEmployees.ToString)
                Response.Write(rOK.toJSON)

            Else
                Dim rOK As New roJSON.JSONError(False, "OK:0")
            End If

        Catch ex As Exception
            Dim rError As New roJSON.JSONError(True, ex.Message.ToString)
            Response.Write(rError.toJSON)
        End Try
    End Sub
    ''' <summary>
    ''' Crea la barra d'eines que va al TAB de la capcelera
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub GetBarButtons(ByVal sID As String)
        Try
            Dim guiActions As Generic.List(Of roGuiAction) = PortalServiceMethods.GetGuiActions(Nothing, "Portal\Task\TaskTemplates\Task", WLHelperWeb.CurrentPassportID)

            Dim oResponse As roTools.PageCustomActions = roTools.BuildPageCustomActions(guiActions, sID, Me.Language, Me.DefaultScope, "TaskTemplates")
            Response.Write(roJSONHelper.SerializeNewtonSoft(oResponse))
        Catch ex As Exception
            Response.Write(ex.StackTrace.ToString)
        End Try
    End Sub

    ''' <summary>
    ''' Carrega roTaskTemplate per ID (sols el Tab superior)
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadTaskTemplateDataTab()
        Try
            Dim oCurrentTaskTemplate As roTaskTemplate = API.TaskTemplatesServiceMethods.GetTaskTemplate(Nothing, Request("ID"), False)

            If oCurrentTaskTemplate Is Nothing Then Exit Sub
            Dim intActiveTab As Integer = roTypes.Any2Integer(Request("aTab"))

            Dim oMainDiv As New HtmlGenericControl("div")

            Dim oImageDiv As New HtmlGenericControl("div")
            oImageDiv.Attributes("class") = "blackRibbonIcon"

            Dim img As HtmlImage = New HtmlImage
            img.Src = "Images/TaskTemplates80.png"
            oImageDiv.Controls.Add(img)

            Dim oTextDiv As New HtmlGenericControl("div")
            oTextDiv.Attributes("class") = "blackRibbonDescription"
            oTextDiv.InnerHtml = "<div id=""NameReadOnly""><span id=""readOnlyNameTaskTemplate"" class=""NameText""></span></div>"

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

        oTabButtons(0) = CreateNewHtmlAnchor("TABBUTTON_00", Me.Language.Translate("tabGeneral", Me.DefaultScope), "bTab")
        hTableCellButtons.Controls.Add(oTabButtons(0))
        hTableCellButtons.Style("height") = "26px"
        hTableRowButtons.Cells.Add(hTableCellButtons)
        hTableButtons.Rows.Add(hTableRowButtons)
        hTableCellButtons = New HtmlTableCell
        hTableRowButtons = New HtmlTableRow

        ReDim Preserve oTabButtons(1)
        oTabButtons(1) = Nothing
        oTabButtons(1) = CreateNewHtmlAnchor("TABBUTTON_01", Me.Language.Translate("tabType", Me.DefaultScope), "bTab")
        hTableCellButtons.Controls.Add(oTabButtons(1))
        hTableCellButtons.Style("height") = "26px"
        hTableRowButtons.Cells.Add(hTableCellButtons)
        hTableButtons.Rows.Add(hTableRowButtons)
        hTableCellButtons = New HtmlTableCell
        hTableRowButtons = New HtmlTableRow

        ReDim Preserve oTabButtons(2)
        oTabButtons(2) = Nothing
        oTabButtons(2) = CreateNewHtmlAnchor("TABBUTTON_02", Me.Language.Translate("tabDefinition", Me.DefaultScope), "bTab")
        hTableCellButtons.Controls.Add(oTabButtons(2))
        hTableCellButtons.Style("height") = "26px"
        hTableRowButtons.Cells.Add(hTableCellButtons)
        hTableButtons.Rows.Add(hTableRowButtons)
        hTableCellButtons = New HtmlTableCell
        hTableRowButtons = New HtmlTableRow

        '================================
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
        '================================

        ReDim Preserve oTabButtons(3)
        oTabButtons(3) = Nothing
        oTabButtons(3) = CreateNewHtmlAnchor("TABBUTTON_03", Me.Language.Translate("tabFields", Me.DefaultScope), "bTab")
        hTableCellButtons.Controls.Add(oTabButtons(3))
        hTableCellButtons.Style("height") = "26px"
        hTableRowButtons.Cells.Add(hTableCellButtons)
        hTableButtons.Rows.Add(hTableRowButtons)
        hTableCellButtons = New HtmlTableCell
        hTableRowButtons = New HtmlTableRow

        'Afegim a la taula principal
        hCellGen.Controls.Add(hTableButtons)
        hRowGen.Cells.Add(hCellGen)
        hTableGen.Rows.Add(hRowGen)

        oTabButtons(0).Attributes.Add("OnClick", "javascript: changeTabs(0);")
        oTabButtons(1).Attributes.Add("OnClick", "javascript: changeTabs(1);")
        oTabButtons(2).Attributes.Add("OnClick", "javascript: changeTabs(2);")
        oTabButtons(3).Attributes.Add("OnClick", "javascript: changeTabs(3);")


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

#End Region


End Class