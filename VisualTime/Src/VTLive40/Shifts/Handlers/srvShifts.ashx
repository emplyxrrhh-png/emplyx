<%@ WebHandler Language="VB" Class="srvShifts" %>

Imports System
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports System.Data
Imports Robotics.Base.DTOs
Imports Robotics.Web.Base.API
Imports Robotics.Base.VTBusiness.Shift

Public Class srvShifts
    Inherits handlerBase

    Private oPermission As Permission

    Public Overrides Sub ProcessRoboticsRequest(ByVal context As HttpContext)
        Me.oPermission = Me.GetFeaturePermission("Shifts.Definition")

        If Me.oPermission > Permission.None Then
            Select Case Request("action")
                Case "getShiftTab" ' Retorna la capcelera de la plana
                    LoadShiftDataTab(Request("ID"))
                Case "getBarButtons"
                    GetBarButtons(Request("ID"))
                Case "retRuleDesc" 'Retorna la descripcio de la regla de justificacio pasada
                    Me.retRuleDescription()
                Case "reloadTypeZones"
                    ReloadTypeZones()
                Case "updateTypeZone" 'Actualiza / crea TypeZone
                    UpdateTypeZone()
                Case "loadTypeZone" 'Carrega un TimeZone (JSON)
                    LoadTypeZone(Request("IDTypeZone"))
                Case "deleteTypeZone" 'Carrega un TimeZone (JSON)
                    DeleteTypeZone(Request("IDTypeZone"))
                Case "shiftIsUsed"
                    CheckIfShiftIsUsed(Request("ID"))
                Case "copyXShift" ' copia a un nou Shift (JSON)
                    CopyShiftDataX(Request("ID"))
                Case "shiftIsUsedForDel"
                    CheckIfShiftIsUsedforDelete(Request("ID"))
                Case "deleteXShift" ' Elimina un nou Shift (JSON)
                    DeleteShiftDataX(Request("ID"))
                Case "selectedHolidayConceptIsAnualWork"
                    CheckIfSelectedHolidayConceptIsAnualWork(Request("ID"))
            End Select
        Else
            ' Si el passport actual no tiene permisos, devuelve un msgbox y redirecciona a la página principal al aceptar el mensaje.
            Dim strResponse As String = "MESSAGE" &
                          "TitleKey=CheckPermission.Denied.Title&" +
                          "DescriptionKey=CheckPermission.Denied.Description&" +
                          "Option1TextKey=CheckPermission.Denied.Option1Text&" +
                          "Option1DescriptionKey=CheckPermission.Denied.Option1Description&" +
                          "Option1OnClickScript=HideMsgBoxForm(); window.location = '" & WLHelperWeb.DefaultRedirectUrl & "' return false;&" +
                          "IconUrl=" & HelperWeb.MsgBoxIconsUrl(HelperWeb.MsgBoxIcons.AlertIcon)
            Response.Write(strResponse)
        End If


    End Sub

#Region "Methods"
    ''' <summary>
    ''' Elimina el Shift
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DeleteShiftDataX(ByVal oID As Integer)
        Try
            Dim rError As roJSON.JSONError

            'Check Permissions
            If Me.oPermission < Permission.Admin Then
                rError = New roJSON.JSONError(True, Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope))
                Response.Write(rError.toJSON)
                Exit Sub
            End If

            If oID = -1 Then Exit Sub

            If API.ShiftServiceMethods.DeleteShift(Nothing, oID, True) = False Then
                rError = New roJSON.JSONError(True, roWsUserManagement.SessionObject.States.ShiftState.ErrorText)
            Else
                rError = New roJSON.JSONError(False, "OK")
            End If
            Response.Write(rError.toJSON)
        Catch ex As Exception
            Dim rError As New roJSON.JSONError(True, ex.Message.ToString)
            Response.Write(rError.toJSON)
        End Try
    End Sub

    Private Sub CheckIfShiftIsUsedforDelete(ByVal oID As String)
        Dim rError As roJSON.JSONError
        Dim bolShiftUsed As Boolean = False
        Try
            'Check Permissions
            If Me.oPermission < Permission.Admin Then
                rError = New roJSON.JSONError(True, Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope))
                Response.Write(rError.toJSON)
                Exit Sub
            End If

            If CInt(oID) = -1 Then
                rError = New roJSON.JSONError(False, "ShiftUsedNo")
            Else
                If API.ShiftServiceMethods.ShiftIsUsed(Nothing, CInt(oID)) = True Then
                    bolShiftUsed = True
                End If
            End If

            If bolShiftUsed = True Then
                rError = New roJSON.JSONError(False, "ShiftUsedYES")
            Else
                rError = New roJSON.JSONError(False, "ShiftUsedNO")
            End If

            Response.Write(rError.toJSON)
        Catch ex As Exception
            rError = New roJSON.JSONError(True, ex.Message.ToString & " " & ex.StackTrace)
            Response.Write(rError.toJSON)
        End Try
    End Sub
    ''' <summary>
    ''' Copia el Shift
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CopyShiftDataX(ByVal oID As Integer)
        Try
            Dim rError As roJSON.JSONError

            'Check Permissions
            If Me.oPermission < Permission.Admin Then
                rError = New roJSON.JSONError(True, Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope))
                Response.Write(rError.toJSON)
                Exit Sub
            End If

            If oID = -1 Then Exit Sub

            Dim oNewShift As roShift = API.ShiftServiceMethods.CopyShift(Nothing, oID)

            If oNewShift Is Nothing Then
                rError = New roJSON.JSONError(True, roWsUserManagement.SessionObject.States.ShiftState.ErrorText)
            Else
                rError = New roJSON.JSONError(False, "OK:" & oNewShift.ID)
            End If

            If rError.Error = False Then
                Dim treePath As String = "/source/"
                If oNewShift.IDGroup > 0 Then treePath &= "A" & oNewShift.IDGroup & "/"
                treePath &= "B" & oNewShift.ID
                HelperWeb.roSelector_SetSelection("B" & oNewShift.ID.ToString, treePath, "ctl00_contentMainBody_roTreesShifts")
            End If
            Response.Write(rError.toJSON)
        Catch ex As Exception
            Dim rError As New roJSON.JSONError(True, ex.Message.ToString)
            Response.Write(rError.toJSON)
        End Try
    End Sub

    Private Sub CheckIfShiftIsUsed(ByVal oID As String)
        Dim rError As roJSON.JSONError
        Dim bolShiftUsed As Boolean = False
        Dim xSpecificDate As Date
        Dim intAllDateEnabled As Integer = "1"
        Dim xFirstDate As Date
        Try
            'Check Permissions
            If Me.oPermission < Permission.Admin Then
                rError = New roJSON.JSONError(True, Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope))
                Response.Write(rError.toJSON)
                Exit Sub
            End If

            If CInt(oID) = -1 Then
                rError = New roJSON.JSONError(False, "ShiftUsedNo")
            Else
                If API.ShiftServiceMethods.ShiftIsUsed(Nothing, CInt(oID)) = True Then
                    bolShiftUsed = True
                    'Muestra el formulario de selección de fecha
                    'Recuperamos la primera fecha desde la que podemos modificar los datos
                    xFirstDate = API.ConnectorServiceMethods.GetFirstDate(Nothing)

                    'Miramos si hemos de mostar el diálogo
                    'Si el día más antiguo que diene un horario es del mismo mes (y año) dónde estamos
                    Dim xOldestDate As Date = API.ShiftServiceMethods.GetShiftOldestDate(Nothing, CInt(oID))

                    'Si el horario tiene asignados días que no se pueden modificar
                    'If xOldestDate > xFirstDate Then
                    '    If xOldestDate.Year = Now.Year And xOldestDate.Month = Now.Month Then
                    '        'Queremos recalcular todos los dias
                    '        bolShiftUsed = False
                    '    End If
                    'End If

                    'Inicializamos la fecha del control de fecha
                    If xOldestDate >= xFirstDate Then
                        xSpecificDate = Format(New Date(Now.Year, Now.Month, 1), HelperWeb.GetShortDateFormat)
                    Else
                        xSpecificDate = xFirstDate
                        intAllDateEnabled = "0"
                    End If
                End If
            End If

            If bolShiftUsed = True Then
                rError = New roJSON.JSONError(False, "ShiftUsedYES:" & Format(xSpecificDate, "yyyy/MM/dd") & ":" & intAllDateEnabled & ":" & Format(xFirstDate, HelperWeb.GetShortDateFormat))
            Else
                rError = New roJSON.JSONError(False, "ShiftUsedNO")
            End If

            Response.Write(rError.toJSON)
        Catch ex As Exception
            rError = New roJSON.JSONError(True, ex.Message.ToString & " " & ex.StackTrace)
            Response.Write(rError.toJSON)
        End Try
    End Sub

    Private Sub DeleteTypeZone(ByVal oId As Integer)
        Try
            Dim rError As roJSON.JSONError

            'Check Permissions
            If Me.oPermission < Permission.Admin Then
                rError = New roJSON.JSONError(True, Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope))
                Response.Write(rError.toJSON)
                Exit Sub
            End If

            If oId = -1 Then Exit Sub

            If API.ShiftServiceMethods.DeleteTimeZone(Nothing, oId, True) = False Then
                rError = New roJSON.JSONError(True, roWsUserManagement.SessionObject.States.ShiftState.ErrorText)
            Else
                rError = New roJSON.JSONError(False, "OK")
            End If
            Response.Write(rError.toJSON)

        Catch ex As Exception
            Dim rError As New roJSON.JSONError(True, ex.Message.ToString & " " & ex.StackTrace)
            Response.Write(rError.toJSON)
        End Try
    End Sub

    Private Sub LoadTypeZone(ByVal oId As Integer)
        Try
            Dim jsonFields As roJSON = New roJSON
            Dim rError As roJSON.JSONError

            'Check Permissions
            If Me.oPermission < Permission.Admin Then
                rError = New roJSON.JSONError(True, Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope))
                Response.Write(rError.toJSON)
                Exit Sub
            End If

            If oId = -1 Then Exit Sub

            Dim dTbl As DataTable = API.ShiftServiceMethods.GetTimeZones(Nothing)

            If dTbl IsNot Nothing Then
                For Each dRow As DataRow In dTbl.Rows
                    If dRow("ID") = oId Then
                        jsonFields.addField("IDTYPEZONE", dRow("ID").ToString, New String() {"hdnNewTypeZoneLayer"}, roJSON.JSONType.Text_JSON)
                        jsonFields.addField("NAME", dRow("Name").ToString, New String() {"txtNewZoneName"}, roJSON.JSONType.Text_JSON)
                        jsonFields.addField("DESCRIPTION", dRow("Description").ToString, New String() {"txtNewZoneDesc"}, roJSON.JSONType.Text_JSON)
                        Exit For
                    End If
                Next
            End If

            Response.Write(jsonFields.CreateJSON)
        Catch ex As Exception
            Dim rError As New roJSON.JSONError(True, ex.Message.ToString & " " & ex.StackTrace)
            Response.Write(rError.toJSON)
        End Try
    End Sub

    ''' <summary>
    ''' Actualitza / crea nou tipus de zona
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub UpdateTypeZone()
        Try
            Dim rError As roJSON.JSONError

            Dim oID As Integer = Request("IDTypeZone")
            Dim oName As String = Request("Name")
            Dim oDesc As String = Request("Desc")

            Dim oTimeZone As New roTimeZone()

            oTimeZone.ID = oID
            oTimeZone.Name = oName
            oTimeZone.Description = oDesc

            'Check Permissions
            If Me.oPermission < Permission.Admin Then
                rError = New roJSON.JSONError(True, Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope))
                Response.Write(rError.toJSON)
                Exit Sub
            End If

            If API.ShiftServiceMethods.SaveTimeZone(Nothing, oTimeZone, True) = False Then
                rError = New roJSON.JSONError(True, roWsUserManagement.SessionObject.States.ShiftState.ErrorText)
            Else
                rError = New roJSON.JSONError(False, "OK")
            End If
            Response.Write(rError.toJSON)
        Catch ex As Exception
            Dim rError As New roJSON.JSONError(True, ex.Message.ToString & " " & ex.StackTrace)
            Response.Write(rError.toJSON)
        End Try
    End Sub

    Private Sub ReloadTypeZones()
        Try
            Dim dTblZ As DataTable = API.ShiftServiceMethods.GetTimeZones(Nothing)

            Dim JSONFields As New roJSON
            Dim JSONCombo As New Generic.List(Of roJSON.JSONComboItem)

            For Each dRowZone As DataRow In dTblZ.Rows
                JSONCombo.Add(New roJSON.JSONComboItem(dRowZone("Name"), dRowZone("ID"), ""))
            Next

            JSONFields.addField("CMBTYPEZONE", "", New String() {"frmAddZone1_cmbType"}, roJSON.JSONType.ComboBox_JSON, JSONCombo)
            JSONFields.addField("cmbRuleZone1", "", New String() {"cmbRuleZone1"}, roJSON.JSONType.ComboBox_JSON, JSONCombo)

            Response.Write(JSONFields.CreateJSON)
        Catch ex As Exception
            Dim rError As New roJSON.JSONError(True, ex.Message.ToString & " " & ex.StackTrace)
            Response.Write(rError.toJSON)
        End Try
    End Sub

    Private Sub retRuleDescription()
        Try
            Dim rError As roJSON.JSONError

            Dim oShiftRule As New roShiftRule

            oShiftRule.IDShift = Request("IDSHIFT")
            oShiftRule.ID = Request("IDRULE")
            oShiftRule.IDIncidence = Request("INCIDENCE")
            oShiftRule.IDZone = Request("ZONE")
            oShiftRule.ConditionValueType = roTypes.Any2Integer(Request("CONDITIONVALUETYPE"))
            oShiftRule.FromTime = Request("FROMTIME")
            oShiftRule.ToTime = Request("TOTIME")
            oShiftRule.FromValueUserFieldName = Request("FROMVALUEUSERFIELD")
            oShiftRule.ToValueUserFieldName = Request("TOVALUEUSERFIELD")
            oShiftRule.BetweenValueUserFieldName = Request("BETWEENVALUEUSERFIELD")
            oShiftRule.IDCause = Request("CAUSE")
            oShiftRule.ActionValueType = roTypes.Any2Integer(Request("ACTIONVALUETYPE"))
            oShiftRule.MaxTime = Request("MAXTIME")
            oShiftRule.MaxValueUserFieldName = Request("MAXVALUEUSERFIELD")

            Dim oDescription As String = API.ShiftServiceMethods.GetShiftRuleDescription(Nothing, oShiftRule)

            rError = New roJSON.JSONError(False, oDescription)
            Response.Write(rError.toJSON)
        Catch ex As Exception
            Dim rError As New roJSON.JSONError(True, ex.Message.ToString)
            Response.Write(rError.toJSON)
        End Try
    End Sub

    ''' <summary>
    ''' Carrega roShift per ID (sols el Tab superior)
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadShiftDataTab(ByVal ShiftId As Integer)
        Try

            Dim oCurrentShift As roShift = API.ShiftServiceMethods.GetShift(Nothing, ShiftId, False)

            If oCurrentShift Is Nothing Then Exit Sub
            Dim intActiveTab As Integer = roTypes.Any2Integer(Request("aTab"))

            Dim oMainDiv As New HtmlGenericControl("div")

            Dim oImageDiv As New HtmlGenericControl("div")
            oImageDiv.Attributes("class") = "blackRibbonIcon"

            Dim img As HtmlImage = New HtmlImage
            img.Src = "Images/Shifts80.png"
            oImageDiv.Controls.Add(img)

            Dim oTextDiv As New HtmlGenericControl("div")
            oTextDiv.Attributes("class") = "blackRibbonDescription"
            oTextDiv.InnerHtml = "<div id=""ShiftNameReadOnly""><span id=""readOnlyNameShift"" class=""NameText""></span></div>"

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
    ''' Crea la barra d'eines que va al TAB de la capcelera
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub GetBarButtons(ByVal sID As String)
        Try
            Dim guiActions As Generic.List(Of roGuiAction) = PortalServiceMethods.GetGuiActions(Nothing, "Portal\ShiftManagement\Shifts\management", WLHelperWeb.CurrentPassportID)

            Dim oResponse As roTools.PageCustomActions = roTools.BuildPageCustomActions(guiActions, sID, Me.Language, Me.DefaultScope, "Shifts")
            Response.Write(roJSONHelper.SerializeNewtonSoft(oResponse))
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
        oTabButtons(1) = CreateNewHtmlAnchor("TABBUTTON_01", Me.Language.Translate("tabType", Me.DefaultScope), "bTab")
        hTableCellButtons.Controls.Add(oTabButtons(1))
        hTableCellButtons.Style("height") = "26px"
        hTableRowButtons.Cells.Add(hTableCellButtons)
        hTableButtons.Rows.Add(hTableRowButtons)
        hTableCellButtons = New HtmlTableCell
        hTableRowButtons = New HtmlTableRow

        ReDim Preserve oTabButtons(2)
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
        oTabButtons(3) = CreateNewHtmlAnchor("TABBUTTON_03", Me.Language.Translate("tabHourZone", Me.DefaultScope), "bTab")
        hTableCellButtons.Controls.Add(oTabButtons(3))
        hTableCellButtons.Style("height") = "26px"
        hTableRowButtons.Cells.Add(hTableCellButtons)
        hTableButtons.Rows.Add(hTableRowButtons)
        hTableCellButtons = New HtmlTableCell
        hTableRowButtons = New HtmlTableRow

        ReDim Preserve oTabButtons(4)
        oTabButtons(4) = CreateNewHtmlAnchor("TABBUTTON_04", Me.Language.Translate("tabRules", Me.DefaultScope), "bTab")
        hTableCellButtons.Controls.Add(oTabButtons(4))
        hTableCellButtons.Style("height") = "26px"
        hTableRowButtons.Cells.Add(hTableCellButtons)
        hTableButtons.Rows.Add(hTableRowButtons)
        hTableCellButtons = New HtmlTableCell
        hTableRowButtons = New HtmlTableRow

        ReDim Preserve oTabButtons(5)
        oTabButtons(5) = CreateNewHtmlAnchor("TABBUTTON_05", Me.Language.Translate("tabVisibility", Me.DefaultScope), "bTab")
        hTableCellButtons.Controls.Add(oTabButtons(5))
        hTableCellButtons.Style("height") = "26px"
        hTableRowButtons.Cells.Add(hTableCellButtons)
        hTableButtons.Rows.Add(hTableRowButtons)
        hTableCellButtons = New HtmlTableCell
        hTableRowButtons = New HtmlTableRow

        '================================
        'Aqui partim en 2 columnes els TABS...
        hCellGen.Controls.Add(hTableButtons)
        hRowGen.Cells.Add(hCellGen)

        If Not HelperSession.GetFeatureIsInstalledFromApplication("Feature\ONE") Then

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

            ReDim Preserve oTabButtons(6)
            oTabButtons(6) = CreateNewHtmlAnchor("TABBUTTON_06", Me.Language.Translate("tabAdvanced", Me.DefaultScope), "bTab")
            hTableCellButtons.Controls.Add(oTabButtons(6))
            hTableCellButtons.Style("height") = "26px"
            hTableRowButtons.Cells.Add(hTableCellButtons)
            hTableButtons.Rows.Add(hTableRowButtons)
            hTableCellButtons = New HtmlTableCell
            hTableRowButtons = New HtmlTableRow

            ReDim Preserve oTabButtons(7)
            oTabButtons(7) = CreateNewHtmlAnchor("TABBUTTON_07", Me.Language.Translate("tabCost", Me.DefaultScope), "bTab")
            hTableCellButtons.Controls.Add(oTabButtons(7))
            hTableCellButtons.Style("height") = "26px"
            hTableRowButtons.Cells.Add(hTableCellButtons)
            hTableButtons.Rows.Add(hTableRowButtons)
            hTableCellButtons = New HtmlTableCell
            hTableRowButtons = New HtmlTableRow

            If HelperSession.GetFeatureIsInstalledFromApplication("Feature\HRScheduling") Then
                ReDim Preserve oTabButtons(8)
                oTabButtons(8) = CreateNewHtmlAnchor("TABBUTTON_08", Me.Language.Translate("tabAssignments", Me.DefaultScope), "bTab")
                hTableCellButtons.Controls.Add(oTabButtons(8))
                hTableCellButtons.Style("height") = "26px"
                hTableRowButtons.Cells.Add(hTableCellButtons)
                hTableButtons.Rows.Add(hTableRowButtons)
                hTableCellButtons = New HtmlTableCell
                hTableRowButtons = New HtmlTableRow
            End If

            hTableRowButtons.Cells.Add(hTableCellButtons)
            hTableButtons.Rows.Add(hTableRowButtons)
            hTableCellButtons = New HtmlTableCell
            hTableRowButtons = New HtmlTableRow

            'Afegim a la taula principal
            hCellGen.Controls.Add(hTableButtons)
            hRowGen.Cells.Add(hCellGen)

        End If


        hTableGen.Rows.Add(hRowGen)

        oTabButtons(0).Attributes.Add("OnClick", "javascript: changeTabs(0);")
        oTabButtons(1).Attributes.Add("OnClick", "javascript: changeTabs(1);")
        oTabButtons(2).Attributes.Add("OnClick", "javascript: changeTabs(2);")
        oTabButtons(3).Attributes.Add("OnClick", "javascript: changeTabs(3);")
        oTabButtons(4).Attributes.Add("OnClick", "javascript: changeTabs(4);")
        oTabButtons(5).Attributes.Add("OnClick", "javascript: changeTabs(5);")
        If Not HelperSession.GetFeatureIsInstalledFromApplication("Feature\ONE") Then
            oTabButtons(6).Attributes.Add("OnClick", "javascript: changeTabs(6);")
            oTabButtons(7).Attributes.Add("OnClick", "javascript: changeTabs(7);")

            If HelperSession.GetFeatureIsInstalledFromApplication("Feature\HRScheduling") Then
                oTabButtons(8).Attributes.Add("OnClick", "javascript: changeTabs(8);")
            End If
        End If
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

    Private Sub CheckIfSelectedHolidayConceptIsAnualWork(ByVal oID As String)
        Dim rError As roJSON.JSONError = New roJSON.JSONError(False, "")

        Try
            If oID IsNot Nothing AndAlso CInt(oID) > -1 Then
                Dim oConcept As Robotics.Base.VTBusiness.Concept.roConcept = API.ConceptsServiceMethods.GetConceptByID(Nothing, CInt(oID), False)
                If oConcept.DefaultQuery = "L" Then
                    rError = New roJSON.JSONError(False, "ISANUALWORK")
                End If
            End If

            Response.Write(rError.toJSON)
        Catch ex As Exception
            rError = New roJSON.JSONError(True, ex.Message.ToString & " " & ex.StackTrace)
            Response.Write(rError.toJSON)
        End Try
    End Sub
#End Region


End Class