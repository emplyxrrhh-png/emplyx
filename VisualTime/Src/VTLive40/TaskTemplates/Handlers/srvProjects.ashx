<%@ WebHandler Language="VB" Class="srvProjects" %>

Imports System
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports System.Data
Imports Robotics.Base.DTOs
Imports Robotics.Web.Base.API
Imports Robotics.Base.VTBusiness.Task

Public Class srvProjects
    Inherits handlerBase

    Private oPermission As Permission
    Private Const FeatureAlias As String = "Tasks.TemplatesDefinition"

    Public Overrides Sub ProcessRoboticsRequest(ByVal context As HttpContext)
        Me.scope = "TaskTemplates"
        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)

        If Me.oPermission > Permission.None Then
            Select Case Request("action")
                Case "getProjectsTab" ' Retorna un Grup (Tab Superior)
                    LoadProjectsDataTab()
                Case "getBarButtons"
                    GetBarButtons(Request("ID"))
                Case "deleteProject" 'Borra el grup seleccionat
                    DeleteProject()
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
    ''' Borra el grup d'horaris seleccionat
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DeleteProject()
        Try
            Dim rError As roJSON.JSONError
            'Check Permissions
            If Me.oPermission < Permission.Admin Then
                rError = New roJSON.JSONError(True, Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope))
                Response.Write(rError.toJSON)
                Exit Sub
            End If

            Dim oProjectTemplate As roProjectTemplates = API.TaskTemplatesServiceMethods.GetProjectTemplate(Nothing, Request("ID"), False)
            If oProjectTemplate Is Nothing Then Exit Sub


            Dim bolRet As Boolean = API.TaskTemplatesServiceMethods.DeleteProjectTemplate(Nothing, Request("ID"), True)
            If Not bolRet Then
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


    ''' <summary>
    ''' Crea la barra d'eines que va al TAB de la capcelera
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub GetBarButtons(ByVal sID As String)
        Try
            Dim guiActions As Generic.List(Of roGuiAction) = PortalServiceMethods.GetGuiActions(Nothing, "Portal\Task\TaskTemplates\Project", WLHelperWeb.CurrentPassportID)

            Dim oResponse As roTools.PageCustomActions = roTools.BuildPageCustomActions(guiActions, sID, Me.Language, Me.DefaultScope, "Projects")
            Response.Write(roJSONHelper.SerializeNewtonSoft(oResponse))
        Catch ex As Exception
            Response.Write(ex.StackTrace.ToString)
        End Try
    End Sub

    ''' <summary>
    ''' Carrega el projecte per ID (sols el Tab superior)
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadProjectsDataTab()
        Try
            Dim oProjectTemplate As roProjectTemplates

            If Request("ID") = "-1" Then
                oProjectTemplate = New roProjectTemplates
            Else
                oProjectTemplate = API.TaskTemplatesServiceMethods.GetProjectTemplate(Nothing, Request("ID"), False)
            End If

            If oProjectTemplate Is Nothing Then Exit Sub
            Dim intActiveTab As Integer = roTypes.Any2Integer(Request("aTab"))


            Dim oMainDiv As New HtmlGenericControl("div")

            Dim oImageDiv As New HtmlGenericControl("div")
            oImageDiv.Attributes("class") = "blackRibbonIcon"

            Dim img As HtmlImage = New HtmlImage
            img.Src = System.Web.VirtualPathUtility.ToAbsolute("~/Base/Images/projectDefault.png")
            oImageDiv.Controls.Add(img)

            Dim strChangeNameJS As String = "onclick=""EditProjectName('true');"""
            ' Miramos si el pasport acutal tiene permisos para modificar el nombre
            If Me.oPermission < Permission.Write Then
                strChangeNameJS = ""
            End If

            Dim dtblCurrent As DataTable = API.TaskTemplatesServiceMethods.GetTaskTemplatesDataTable(oProjectTemplate.ID, "", Nothing, "", False)

            Dim oTextDiv As New HtmlGenericControl("div")
            oTextDiv.Attributes("class") = "blackRibbonDescription"
            oTextDiv.InnerHtml = "<div id=""NameReadOnly""><span id=""readOnlyNameProject"" class=""NameText"">  " & oProjectTemplate.Project & "</span></div>"
            If (dtblCurrent IsNot Nothing) Then
                oTextDiv.InnerHtml &= Me.Language.Translate("TemplatesInGroup", DefaultScope) & " " & dtblCurrent.Rows.Count
            Else
                oTextDiv.InnerHtml &= Me.Language.Translate("TemplatesInGroup", DefaultScope) & "0"
            End If

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

    Private Function CreateTabs(ByVal iActiveTab As Integer) As HtmlTable
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

        'Afegim a la taula principal
        hCellGen.Controls.Add(hTableButtons)
        hRowGen.Cells.Add(hCellGen)
        hTableGen.Rows.Add(hRowGen)

        oTabButtons(0).Attributes.Add("OnClick", "javascript: changeTabs(0);")
        oTabButtons(1).Attributes.Add("OnClick", "javascript: changeTabs(1);")


        oTabButtons(iActiveTab).Attributes("class") = "bTab-active"

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