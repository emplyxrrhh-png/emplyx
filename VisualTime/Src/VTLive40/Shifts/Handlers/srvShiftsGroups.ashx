<%@ WebHandler Language="VB" Class="srvShiftsGroups" %>

Imports System
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports System.Data
Imports Robotics.Base.DTOs
Imports Robotics.Web.Base.API
Imports Robotics.Base.VTBusiness.Shift

Public Class srvShiftsGroups
    Inherits handlerBase

    Private Const FeatureAlias As String = "Shifts.Definition"
    Private oPermission As Permission

    Public Overrides Sub ProcessRoboticsRequest(ByVal context As HttpContext)
        Me.scope = "Shifts"
        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)

        If Me.oPermission > Permission.None Then

            Select Case context.Request("action")
                Case "getShiftGroupsTab" ' Retorna un Grup (Tab Superior)
                    LoadShiftGroupDataTab(Request("ID"))
                Case "getBarButtons"
                    GetBarButtons(Request("ID"))
                Case "chgNameShiftGroup" 'Cambia el Nom del grup seleccionat
                    changeNameShiftGroup(Request("ID"))
                Case "deleteShiftGroup" 'Borra el grup seleccionat
                    DeleteShiftGroup(Request("ID"))
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
    ''' Cambia el nom del grup
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub changeNameShiftGroup(ByVal IDShiftGroup As Integer)
        Dim bolRet As Boolean = False
        Dim strErrorInfo As String = ""
        Try
            If Me.oPermission >= Permission.Write Then
                Dim oShiftGroup As roShiftGroup = API.ShiftServiceMethods.GetShiftGroup(Nothing, IDShiftGroup, False)

                If oShiftGroup Is Nothing Then Exit Sub
                Dim intIDShiftGroup As Integer = roTypes.Any2Integer(Request("ID"))

                Dim NouName As String = Request("NewName")
                If NouName = "" Then
                    strErrorInfo = Me.Language.Translate("NameInvalid", DefaultScope) 'Nombre no valido
                Else
                    oShiftGroup.Name = NouName
                    bolRet = API.ShiftServiceMethods.SaveShiftGroup(Nothing, oShiftGroup, True)
                    If Not bolRet Then
                        strErrorInfo = roWsUserManagement.SessionObject.States.ShiftState.ErrorText
                    End If
                End If
            Else
                strErrorInfo = Me.Language.Translate("CheckPermission.Denied.Message", Me.DefaultScope)
            End If
        Catch ex As Exception
            Response.Write(ex.Message.ToString & ex.StackTrace.ToString)
        Finally
            Dim strResponse As String
            If bolRet Then
                strResponse = "OK"
            Else
                strResponse = "MESSAGE" & _
                              "TitleKey=ChangeNameShiftGroup.Error.Title&" + _
                              "DescriptionText=" + strErrorInfo + "&" + _
                              "Option1TextKey=ChangeNameShiftGroup.Error.Option1Text&" + _
                              "Option1DescriptionKey=ChangeNameShiftGroup.Error.Option1Description&" + _
                              "Option1OnClickScript=HideMsgBoxForm(); return false;&" + _
                              "IconUrl=" & HelperWeb.MsgBoxIconsUrl(HelperWeb.MsgBoxIcons.ErrorIcon)
            End If
            Response.Write(strResponse)

        End Try
    End Sub

    ''' <summary>
    ''' Borra el grup d'horaris seleccionat
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DeleteShiftGroup(ByVal IDShiftGroup As Integer)
        Dim bolRet As Boolean = False
        Dim strErrorInfo As String = ""
        Try

            Dim oShiftGroup As roShiftGroup = API.ShiftServiceMethods.GetShiftGroup(Nothing, IDShiftGroup, False)
            If oShiftGroup Is Nothing Then Exit Sub

            Dim bContinuar As Boolean = True

            If oShiftGroup.BusinessGroup <> String.Empty Then
                If Not API.ShiftServiceMethods.BusinessGroupListInUse(Nothing, oShiftGroup.BusinessGroup, oShiftGroup.ID) Then
                    If API.UserAdminServiceMethods.BusinessGroupListInUse(Nothing, oShiftGroup.BusinessGroup) Then
                        bContinuar = False
                    End If
                End If
            End If

            If bContinuar Then
                bolRet = API.ShiftServiceMethods.DeleteShiftGroup(Nothing, IDShiftGroup, True)
                If Not bolRet Then
                    strErrorInfo = roWsUserManagement.SessionObject.States.ShiftState.ErrorText
                End If
            Else
                strErrorInfo = Me.Language.Translate("BusinessGroupInUse.Description", DefaultScope)
            End If

        Catch ex As Exception
            strErrorInfo = ex.Message
        Finally
            Dim strResponse As String
            If bolRet Then
                strResponse = "OK"
            Else
                strResponse = "MESSAGE" & _
                              "TitleKey=DeleteShiftGroup.Error.Title&" + _
                              "DescriptionText=" + strErrorInfo + "&" + _
                              "Option1TextKey=DeleteShiftGroup.Error.Option1Text&" + _
                              "Option1DescriptionKey=DeleteShiftGroup.Error.Option1Description&" + _
                              "Option1OnClickScript=HideMsgBoxForm(); return false;&" + _
                              "IconUrl=" & HelperWeb.MsgBoxIconsUrl(HelperWeb.MsgBoxIcons.ErrorIcon)
            End If
            Response.Write(strResponse)
        End Try
    End Sub

    ''' <summary>
    ''' Carrega el grup per ID (sols el Tab superior)
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadShiftGroupDataTab(ByVal IDShiftGroup As Integer)
        Try

            Dim oShiftGroup As roShiftGroup

            If Request("ID") = "-1" Then
                oShiftGroup = New roShiftGroup
            Else
                oShiftGroup = API.ShiftServiceMethods.GetShiftGroup(Nothing, IDShiftGroup, False)
            End If

            If oShiftGroup Is Nothing Then Exit Sub
            Dim intActiveTab As Integer = roTypes.Any2Integer(Request("aTab"))


            Dim oMainDiv As New HtmlGenericControl("div")

            Dim oImageDiv As New HtmlGenericControl("div")
            oImageDiv.Attributes("class") = "blackRibbonIcon"

            Dim img As HtmlImage = New HtmlImage
            img.Src = System.Web.VirtualPathUtility.ToAbsolute("~/Base/Images/groupDefault.png")
            img.Height = 80
            oImageDiv.Controls.Add(img)

            Dim strChangeNameJS As String = "onclick=""EditNameShiftGroup('true');"""
            ' Miramos si el pasport acutal tiene permisos para modificar el nombre
            If Me.oPermission < Permission.Write Then
                strChangeNameJS = ""
            End If

            Dim dtblCurrent As DataTable = API.ShiftServiceMethods.GetShiftsFromGroup(Nothing, oShiftGroup.ID)

            Dim oTextDiv As New HtmlGenericControl("div")
            oTextDiv.Attributes("class") = "blackRibbonDescription"
            oTextDiv.InnerHtml = "<div id=""ShiftGroupNameReadOnly""><span id=""readOnlyNameShiftGroup"" class=""NameText"">  " & oShiftGroup.Name & "</span></div>" & _
                                    Me.Language.Translate("ShiftInGroup", DefaultScope) & " " & dtblCurrent.Rows.Count

            Dim oButtonsDiv As New HtmlGenericControl("div")
            oButtonsDiv.Attributes("class") = "blackRibbonButtons"
            oButtonsDiv.Controls.Add(Me.CreateShiftGroupTabs(intActiveTab))

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
    ''' Crea la barra d'eines que va al TAB de la capcelera
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub GetBarButtons(ByVal sID As String)
        Try
            Dim guiActions As Generic.List(Of roGuiAction) = PortalServiceMethods.GetGuiActions(Nothing, "Portal\ShiftManagement\Shifts\GroupManagement", WLHelperWeb.CurrentPassportID)

            Dim oResponse As roTools.PageCustomActions = roTools.BuildPageCustomActions(guiActions, sID, Me.Language, Me.DefaultScope, "Shifts")
            Response.Write(roJSONHelper.SerializeNewtonSoft(oResponse))
        Catch ex As Exception
            Response.Write(ex.StackTrace.ToString)
        End Try
    End Sub

    ''' <summary>
    ''' Genera els botons de Usuaris de la dreta (General, Contratos, Fichajes, Ausencias Previstas...)
    ''' </summary>
    ''' <returns>Retorna un HTML Table amb els botons en format columna</returns>
    ''' <remarks></remarks>
    Private Function CreateShiftGroupTabs(ByVal intActiveTab As Integer) As HtmlTable
        Dim hTableButtons As New HtmlTable
        Dim hTableRowButtons As New HtmlTableRow
        Dim hTableCellButtons As New HtmlTableCell

        hTableButtons.Border = 0
        hTableButtons.CellSpacing = 0
        hTableButtons.CellPadding = 0
        'hTableButtons.Height = "105px"

        Dim oTabButtons() As HtmlAnchor = {Nothing, Nothing}

        oTabButtons(0) = CreateNewHtmlAnchor("TABBUTTON_GeneralGroups", Me.Language.Translate("tabGeneral", Me.DefaultScope), "bTab")
        hTableCellButtons.Controls.Add(oTabButtons(0))
        hTableRowButtons.Cells.Add(hTableCellButtons)
        hTableRowButtons.VAlign = "top"
        hTableButtons.Rows.Add(hTableRowButtons)
        hTableCellButtons = New HtmlTableCell
        hTableRowButtons = New HtmlTableRow

        oTabButtons(0).Attributes.Add("OnClick", "javascript: changeTabs(0);")

        oTabButtons(0).Attributes("class") = "bTab-active"

        Return hTableButtons ' Retorna el HTMLTable

    End Function

    ''' <summary>
    ''' Genera automaticament HtmlButtons
    ''' </summary>
    ''' <param name="Name">Nom del boton (ID)</param>
    ''' <param name="Text">Texte (InnerText)</param>
    ''' <param name="Transparente">No es fa servir...</param>
    ''' <returns>un HTMLButton</returns>
    ''' <remarks></remarks>
    Private Function CreateNewHtmlButton(ByVal Name As String, ByVal Text As String, ByVal Transparente As Boolean) As HtmlButton
        Dim obutton As New HtmlButton
        obutton.ID = Name
        obutton.InnerText = Text
        Return obutton
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