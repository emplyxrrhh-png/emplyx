<%@ WebHandler Language="VB" Class="srvLabAgree" %>

Imports System
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTEmployees.LabAgree

Public Class srvLabAgree
    Inherits handlerBase

    Private intActiveTab As Integer = 0
    Private oPermission As Permission
    Private strActiveTab As String = "TABBUTTON_General"
    Private Const FeatureAlias As String = "LabAgree.Definition"

    Public Overrides Sub ProcessRoboticsRequest(ByVal context As HttpContext)
        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)

        If Me.oPermission > Permission.None Then

            Select Case context.Request("action")
                Case "getLabAgreesTab" ' Retorna la capcelera de la plana
                    LoadLabAgreesDataTab()
                Case "deleteXLabAgree" ' Elimina un LabAgree (JSON)
                    DeleteLabAgreeDataX(Request("ID"))
                Case "getBarButtons"
                    GetBarButtons(Request("ID"))
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

            Dim rError As New roJSON.JSONError(True, Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope))
            Response.Write(rError.toJSON)

        End If
    End Sub

#Region "Methods"

    ''' <summary>
    ''' Carrega Exportacio per ID (sols el Tab superior)
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadLabAgreesDataTab()
        Try

            Dim oCurrentLabAgree As roLabAgree

            If Request("ID") = "-1" Or Request("ID") = "source" Then
                oCurrentLabAgree = New roLabAgree
            Else
                oCurrentLabAgree = API.LabAgreeServiceMethods.GetLabAgreeByID(Nothing, Request("ID"), False)
            End If

            If oCurrentLabAgree Is Nothing Then Exit Sub
            Me.intActiveTab = Request("aTab")

            Dim oMainDiv As New HtmlGenericControl("div")

            Dim oImageDiv As New HtmlGenericControl("div")
            oImageDiv.Attributes("class") = "blackRibbonIcon"

            Dim img As HtmlImage = New HtmlImage
            img.Src = VirtualPathUtility.ToAbsolute("~/LabAgree/Images/LabAgree80.png")
            oImageDiv.Controls.Add(img)

            Dim oTextDiv As New HtmlGenericControl("div")
            oTextDiv.Attributes("class") = "blackRibbonDescription"
            oTextDiv.InnerHtml = "<div id=""NameReadOnly""><span id=""readOnlyNameLabAgrees"" class=""NameText"">" & oCurrentLabAgree.Name & " </span></div>"

            Dim oButtonsDiv As New HtmlGenericControl("div")
            oButtonsDiv.Attributes("class") = "blackRibbonButtons"
            oButtonsDiv.Controls.Add(Me.CreateTabs())

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

        oTabButtons(0) = CreateNewHtmlAnchor("TABBUTTON_00", Me.Language.Translate("tabGeneral", Me.DefaultScope), "bTab")
        hTableCellButtons.Controls.Add(oTabButtons(0))
        hTableRowButtons.Cells.Add(hTableCellButtons)
        hTableButtons.Rows.Add(hTableRowButtons)
        hTableCellButtons = New HtmlTableCell
        hTableRowButtons = New HtmlTableRow

        ReDim Preserve oTabButtons(1)
        oTabButtons(1) = Nothing

        oTabButtons(1) = CreateNewHtmlAnchor("TABBUTTON_01", Me.Language.Translate("tabRules", Me.DefaultScope), "bTab")
        hTableCellButtons.Controls.Add(oTabButtons(1))
        hTableRowButtons.Cells.Add(hTableCellButtons)
        hTableButtons.Rows.Add(hTableRowButtons)
        hTableCellButtons = New HtmlTableCell
        hTableRowButtons = New HtmlTableRow


        Dim bolHRSchedulingLicense As Boolean = HelperSession.GetFeatureIsInstalledFromApplication("Feature\HRScheduling") OrElse HelperSession.GetFeatureIsInstalledFromApplication("Feature\CostControl")
        If bolHRSchedulingLicense Then
            ReDim Preserve oTabButtons(2)
            oTabButtons(2) = Nothing

            oTabButtons(2) = CreateNewHtmlAnchor("TABBUTTON_02", Me.Language.Translate("tabScheuleRules", Me.DefaultScope), "bTab")
            hTableCellButtons.Controls.Add(oTabButtons(2))
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
    ''' Elimina LabAgree
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DeleteLabAgreeDataX(ByVal oID As Integer)
        Dim strResponse As String = "OK"
        Dim strErrorInfo As String = ""

        Dim rError As New roJSON.JSONError

        Try

            'Check Permissions
            If Me.oPermission < Permission.Admin Then
                strErrorInfo = Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope)
            End If
            If strErrorInfo = String.Empty Then
                If API.LabAgreeServiceMethods.DeleteLabAgree(Nothing, oID, True) = False Then
                    strErrorInfo = roWsUserManagement.SessionObject.States.LabAgreeState.ErrorText
                End If
            End If

            If strErrorInfo = String.Empty Then
                rError = New roJSON.JSONError(False, "OK")
            Else
                rError = New roJSON.JSONError(True, strErrorInfo, DefaultScope)
            End If
            Response.Write(rError.toJSON)
            Exit Sub



        Catch ex As Exception
            strErrorInfo = ex.Message.ToString
            rError = New roJSON.JSONError(True, strErrorInfo, DefaultScope)
        End Try

        If strErrorInfo <> "" Then
            strResponse = "MESSAGE" & _
                          "TitleKey=RemoveLabAgree.Error.Title&" + _
                          "DescriptionText=" + strErrorInfo + "&" + _
                          "Option1TextKey=RemoveLabAgree.Error.Option1Text&" + _
                          "Option1DescriptionKey=RemoveLabAgree.Error.Option1Description&" + _
                          "Option1OnClickScript=HideMsgBoxForm(); return false;&" + _
                          "IconUrl=" & HelperWeb.MsgBoxIconsUrl(HelperWeb.MsgBoxIcons.ErrorIcon)

            Response.Write(rError.toJSON)
            Exit Sub
        End If





    End Sub

    ''' <summary>
    ''' Crea la barra d'eines que va al TAB de la capcelera
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub GetBarButtons(ByVal sID As String)
        Try
            Dim guiActions As Generic.List(Of roGuiAction) = PortalServiceMethods.GetGuiActions(Nothing, "Portal\LabAgree\LabAgree\Management", WLHelperWeb.CurrentPassportID)

            Dim oResponse As roTools.PageCustomActions = roTools.BuildPageCustomActions(guiActions, sID, Me.Language, Me.DefaultScope, "LabAgrees")
            Response.Write(roJSONHelper.SerializeNewtonSoft(oResponse))
        Catch ex As Exception
            Response.Write(ex.StackTrace.ToString)
        End Try
    End Sub

#End Region


End Class