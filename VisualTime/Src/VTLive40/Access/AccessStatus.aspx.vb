Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Zone
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class AccessStatus
    Inherits PageBase

    <Runtime.Serialization.DataContract()>
    Private Class ObjectCallbackRequest

        <Runtime.Serialization.DataMember(Name:="aTab")>
        Public aTab As Integer

        <Runtime.Serialization.DataMember(Name:="ID")>
        Public ID As Integer

        <Runtime.Serialization.DataMember(Name:="Name")>
        Public Name As String

        <Runtime.Serialization.DataMember(Name:="action")>
        Public Action As String

        <Runtime.Serialization.DataMember(Name:="StampParam")>
        Public StampParam As Double

    End Class

    Private oPermission As Permission

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        Me.oPermission = Me.GetFeaturePermission("Access.Zones.Supervision")

        Me.InsertExtraJavascript("Ajax", "~/Base/Scripts/Ajax.js")
        Me.InsertExtraJavascript("roTreeState", "~/Base/Scripts/rocontrols/roTrees/roTreeState.js")
        Me.InsertExtraJavascript("jsDate", "~/Base/Scripts/jsDate.js")
        Me.InsertExtraJavascript("flStatusMap", "~/Access/Scripts/flStatusMap.js")
        Me.InsertExtraJavascript("GenericData", "~/Base/Scripts/GenericData.js")
        Me.InsertExtraJavascript("roOptionPanelClient", "~/Base/Scripts/roOptionPanelClient.js")
        Me.InsertExtraJavascript("roOptPanelClientGroup", "~/Base/Scripts/roOptPanelClientGroup.js")
        Me.InsertExtraJavascript("jsDatePicker", "~/Base/Scripts/jsDatePicker.js")
        Me.InsertExtraJavascript("webUserForms", "~/Base/Scripts/webUserForms.js")
        Me.InsertExtraJavascript("AccessStatu", "~/Access/Scripts/AccessStatu.js")

    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Comprueba la licencia
        If HelperSession.GetFeatureIsInstalledFromApplication("Forms\Access") = False Then
            WLHelperWeb.RedirectAccessDenied(False)
            Exit Sub
        End If

        roTreesAccessStatus.TreeCaption = Me.Language.Translate("TreeCaptionAccessStatus", Me.DefaultScope)

        Me.lblSaveChanges.InnerText = Me.Language.Translate("msgSaveChangesbar", DefaultScope)
        Me.lblSaveChangesBottom.InnerText = Me.lblSaveChanges.InnerText
        Me.lblUndoChanges.InnerText = Me.Language.Translate("msgUndoChangesbar", DefaultScope)
        Me.lblUndoChangesBottom.InnerText = Me.lblUndoChanges.InnerText

        Me.msgHasChanges.Value = Me.Language.Translate("msgHasChanges", DefaultScope)
        Me.msgHasErrors.Value = Me.Language.Translate("msgHasErrors", DefaultScope)

        If Me.oPermission = Permission.None Then
            WLHelperWeb.RedirectAccessDenied(False)
            Exit Sub
        ElseIf Me.oPermission = Permission.Read Then
            'DisableControls()
            Me.cmbStatusPlaneMain.Enabled = True
        End If

        hdnModeEdit.Value = "false"

        If Not Me.IsPostBack And Not Me.IsCallback Then
            Me.LoadStatusPlaneMain()
        End If

    End Sub

    Private Sub LoadStatusPlaneMain(Optional nIndex As Integer = -1)
        Me.cmbStatusPlaneMain.Items.Clear()
        Me.cmbStatusPlaneMain.ValueType = GetType(Integer)
        Me.cmbStatusPlaneMain.Items.Add(New DevExpress.Web.ListEditItem("", 0))
        Dim dTblZP As DataTable = API.ZoneServiceMethods.GetZonePlanes(Me.Page)
        If dTblZP IsNot Nothing Then
            For Each dRow As DataRow In dTblZP.Rows
                Me.cmbStatusPlaneMain.Items.Add(New DevExpress.Web.ListEditItem(dRow("Name"), dRow("ID")))
            Next
        End If
        Me.cmbStatusPlaneMain.SelectedIndex = 0

        If nIndex > 0 Then
            Me.cmbStatusPlaneMain.SelectedIndex = nIndex
        End If
    End Sub

    Protected Sub CallbackHelper_Callback(source As Object, e As DevExpress.Web.CallbackEventArgs) Handles CallbackHelper.Callback
        e.Result = String.Empty
        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        If strParameter.IndexOf("SELECTEDPLANE") >= 0 Then
            Dim IdPlane As Integer = roTypes.Any2Integer(strParameter.Substring(strParameter.IndexOf("=") + 1))
            Dim ErrorInfo As String = String.Empty
            Dim PositionParam As String = String.Empty
            Dim ImageIdParam As String = String.Empty
            LoadAccessStatusMain(IdPlane, ErrorInfo, PositionParam, ImageIdParam)
            e.Result = PositionParam & "@" & ImageIdParam
        End If
    End Sub

    Protected Sub ASPxCallbackPanelContenido_Callback(ByVal sender As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase) Handles ASPxCallbackPanelContenido.Callback

        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Dim oParameters As New ObjectCallbackRequest()
        oParameters = roJSONHelper.Deserialize(strParameter, oParameters.GetType())

        Dim responseMessage = String.Empty
        Dim bRet As Boolean

        Select Case oParameters.Action

            Case "GETACCESSSTATUS"
                bRet = LoadAccessStatus(oParameters.ID, responseMessage)
                ASPxCallbackPanelContenido.JSProperties.Add("cpAction", "GETACCESSSTATUS")
                ASPxCallbackPanelContenido.JSProperties.Add("cpResult", IIf(bRet, "OK", "NOK"))
                If Not bRet Then
                    ASPxCallbackPanelContenido.JSProperties.Add("cpMessage", responseMessage)
                End If

            Case "GETACCESSSTATUSMAIN"
                bRet = True
                ASPxCallbackPanelContenido.JSProperties.Add("cpAction", "GETACCESSSTATUSMAIN")
                ASPxCallbackPanelContenido.JSProperties.Add("cpResult", IIf(bRet, "OK", "NOK"))

            Case Else
                ASPxCallbackPanelContenido.JSProperties.Add("cpAction", "UNKNOW")
                ASPxCallbackPanelContenido.JSProperties.Add("cpResult", "NOK")
                ASPxCallbackPanelContenido.JSProperties.Add("cpMessage", "Unknow call")
        End Select

    End Sub

    Private Function LoadAccessStatusMain(ByRef IdAccessPlane As Integer, ByRef ErrorInfo As String, ByRef PositionParam As String, ByRef ImageIdParam As String) As Boolean
        Dim bRet As Boolean = False

        ErrorInfo = String.Empty

        Try

            If Me.oPermission > Permission.None Then

                Dim strPosition As String = ""

                Dim oZones As New Generic.List(Of roZone)
                oZones = API.ZoneServiceMethods.GetZonesFromPlane(Me.Page, IdAccessPlane, True)

                For Each oZone As roZone In oZones
                    If oZone.X1 <> -1 Then
                        Dim IDZones As New Generic.List(Of Integer)
                        IDZones.Add(oZone.ID)

                        Dim auxColor As System.Drawing.Color = System.Drawing.ColorTranslator.FromWin32(oZone.Color)
                        Dim oHTMLColor As String = System.Drawing.ColorTranslator.ToHtml(auxColor)

                        Dim oAccessOKs As Integer = 0
                        Dim oAccessIncorrect As Integer = 0

                        Dim dSetControl As DataSet = API.PunchServiceMethods.GetAccessPunchesStatus(Me.Page, True, "Access", , Nothing, IDZones, True)

                        Dim dtblCurrent As DataTable = dSetControl.Tables(0)

                        If dtblCurrent IsNot Nothing AndAlso dtblCurrent.Rows.Count > 0 Then
                            oAccessOKs = dtblCurrent.Rows(0)("AccessOK")
                            oAccessIncorrect = dtblCurrent.Rows(0)("AccessIncorrects")
                        End If

                        strPosition &= oZone.ID & "," & oZone.Name & "," & oZone.X1 & "," & oZone.Y1 & "," & oZone.X2 & "," & oZone.Y2 & "," & oHTMLColor.Replace("#", "") & "," & oAccessOKs.ToString & "," & oAccessIncorrect.ToString & "," & oZone.IDCamera & ","
                    End If
                Next

                If strPosition <> "" Then strPosition = strPosition.Substring(0, Len(strPosition) - 1)

                PositionParam = strPosition
                ImageIdParam = IdAccessPlane

                bRet = True
            Else
                ErrorInfo = Me.Language.Translate("CheckPermission.Denied.Message", Me.DefaultScope)
            End If
        Catch ex As Exception
            ErrorInfo = Me.Language.Translate("Message.UnknowError", Me.DefaultScope)
        End Try

        Return bRet

    End Function

    Private Function LoadAccessStatus(ByRef IdAccessStatus As Integer, ByRef ErrorInfo As String) As Boolean
        Dim bRet As Boolean = False

        ErrorInfo = String.Empty

        Try

            If Me.oPermission > Permission.None Then

                Dim oCurrentAccessStatus As roZone

                If IdAccessStatus = -1 Then
                    oCurrentAccessStatus = New roZone()
                Else
                    oCurrentAccessStatus = ZoneServiceMethods.GetZoneByID(Me, IdAccessStatus, True)
                End If

                Dim strPosition As String = ""

                Dim advancedAccess = HelperSession.AdvancedParametersCache("AccessGroupsMode").Equals("1")
                Dim dtblZones As DataTable = ZoneServiceMethods.GetZones(Page, If(advancedAccess, WLHelperWeb.CurrentPassportID, 0))

                For Each dRowZone As DataRow In dtblZones.Rows
                    If Not dRowZone("IDParent") Is DBNull.Value Then
                        If dRowZone("IDParent") = oCurrentAccessStatus.ID Then
                            If dRowZone("X1") <> -1 Then
                                Dim auxColor As System.Drawing.Color = System.Drawing.ColorTranslator.FromWin32(dRowZone("Color"))
                                Dim oHTMLColor As String = System.Drawing.ColorTranslator.ToHtml(auxColor)
                                strPosition &= dRowZone("X1") & "," & dRowZone("Y1") & "," & dRowZone("X2") & "," & dRowZone("Y2") & "," & "false," & oHTMLColor.Replace("#", "") & ","
                            End If
                        End If
                    End If
                Next
                If strPosition <> "" Then strPosition = strPosition.Substring(0, Len(strPosition) - 1)
                Me.hdnManagePlane.Set("Position", strPosition)

                Dim imgID As Integer = 1

                If oCurrentAccessStatus.Image IsNot Nothing Then
                    If oCurrentAccessStatus.Image.Length = 0 Then
                        imgID = -1
                    Else
                        imgID = oCurrentAccessStatus.ID
                    End If
                Else
                    imgID = -1
                End If
                Me.hdnManagePlane.Set("ImageID", imgID)

                'Carga grids
                LoadAccessStatusData(IdAccessStatus)

                bRet = True
            Else
                ErrorInfo = Me.Language.Translate("CheckPermission.Denied.Message", Me.DefaultScope)
            End If
        Catch ex As Exception
            ErrorInfo = Me.Language.Translate("Message.UnknowError", Me.DefaultScope)
        End Try

        Return bRet

    End Function

    Private Sub LoadAccessStatusData(ByRef IdZone As Integer)
        Dim strCols() As String
        Dim sizeCols() As String
        Dim cssCols() As String

        Dim dSetControl As DataSet
        Dim dtblCurrent As DataTable
        Dim dtblLeave As DataTable
        Dim dtblIncorrect As DataTable

        Dim IDZones As New Generic.List(Of Integer)
        IDZones.Add(IdZone)

        dSetControl = API.PunchServiceMethods.GetAccessPunchesStatus(Me.Page, True, "Access", , Nothing, IDZones, True)

        'Carrega Grid Usuaris Actuals -------------------------------------
        dtblCurrent = dSetControl.Tables(1)
        dtblLeave = dSetControl.Tables(2)
        dtblIncorrect = dSetControl.Tables(3)

        Dim tblControlCurrent As DataTable
        If dtblCurrent.Rows.Count > 0 Then
            strCols = New String() {Me.Language.Translate("CurrentZone.ImageEmployee", Me.DefaultScope), Me.Language.Translate("CurrentZone.ColumnEmployee", Me.DefaultScope), Me.Language.Translate("CurrentZone.DateArrival", Me.DefaultScope), Me.Language.Translate("CurrentZone.SourceZone", Me.DefaultScope)}
            sizeCols = New String() {"45px", "250px", "110px", "150px"}
            cssCols = New String() {"GridStyle-cellheader GridStyle-cellheader-noend", "GridStyle-cellheader GridStyle-cellheader-noend", "GridStyle-cellheader GridStyle-cellheader-noend", "GridStyle-cellheader"}

            tblControlCurrent = New DataTable
            tblControlCurrent.Columns.Add("NomCamp") 'Nom del Camp
            tblControlCurrent.Columns.Add("NomParam") 'Parametre que es pasara
            tblControlCurrent.Columns.Add("Visible") 'Si es te que carregar en les columnes o no...

            tblControlCurrent.Rows.Add(createRowControl(tblControlCurrent.NewRow, "ImageEmployee", "", True))
            tblControlCurrent.Rows.Add(createRowControl(tblControlCurrent.NewRow, "IDEmployee", "", False))
            tblControlCurrent.Rows.Add(createRowControl(tblControlCurrent.NewRow, "IDZone", "", False))
            tblControlCurrent.Rows.Add(createRowControl(tblControlCurrent.NewRow, "NameZone", "", False))
            tblControlCurrent.Rows.Add(createRowControl(tblControlCurrent.NewRow, "SourceIDZone", "", False))

            Dim htmlHGridCurrent As HtmlTable = creaHeaderLists(strCols, sizeCols, cssCols)
            Me.divGridEmpInZoneHeader.Controls.Add(htmlHGridCurrent)

            Dim htmlTGridCurrent As HtmlTable = creaGridLists(dtblCurrent, strCols, sizeCols, tblControlCurrent, True)
            Me.divGridEmpInZone.Controls.Add(htmlTGridCurrent)
        Else
            Me.lblTitleEmpInZoneGrid.Text = Me.Language.Translate("NoEmployeesInZone", Me.DefaultScope) '"No hay empleados actualmente"
        End If

        'Carrega Grid Usuaris han sortit ultima hora -------------------------------------
        If dtblLeave.Rows.Count > 0 Then
            strCols = New String() {Me.Language.Translate("CurrentZone.ImageEmployee", DefaultScope), Me.Language.Translate("LeaveZone.ColumnEmployee", Me.DefaultScope), Me.Language.Translate("LeaveZone.DateExit", Me.DefaultScope), Me.Language.Translate("LeaveZone.DestZone", Me.DefaultScope)}
            sizeCols = New String() {"45px", "250px", "110px", "150px"}
            cssCols = New String() {"GridStyle-cellheader GridStyle-cellheader-noend", "GridStyle-cellheader GridStyle-cellheader-noend", "GridStyle-cellheader GridStyle-cellheader-noend", "GridStyle-cellheader"}

            Dim tblControlLeave As DataTable = New DataTable
            tblControlLeave.Columns.Add("NomCamp") 'Nom del Camp
            tblControlLeave.Columns.Add("NomParam") 'Parametre que es pasara
            tblControlLeave.Columns.Add("Visible") 'Si es te que carregar en les columnes o no...

            tblControlLeave.Rows.Add(createRowControl(tblControlLeave.NewRow, "ImageEmployee", "", True))
            tblControlLeave.Rows.Add(createRowControl(tblControlLeave.NewRow, "IDEmployee", "", False))
            tblControlLeave.Rows.Add(createRowControl(tblControlLeave.NewRow, "IDZone", "", False))
            tblControlLeave.Rows.Add(createRowControl(tblControlLeave.NewRow, "NameZone", "", False))
            tblControlLeave.Rows.Add(createRowControl(tblControlLeave.NewRow, "SourceIDZone", "", False))

            Dim htmlHGridFuture As HtmlTable = creaHeaderLists(strCols, sizeCols, cssCols)
            Me.divGridLeaveLastHourHeader.Controls.Add(htmlHGridFuture)

            Dim dDelRows As DataRow() = dtblLeave.Select("SourceIDZone = " & IdZone)
            For Each dDelRow As DataRow In dDelRows
                dDelRow.Delete()
            Next

            dDelRows = dtblLeave.Select("IDZone <> " & IdZone)
            For Each dDelRow As DataRow In dDelRows
                dDelRow.Delete()
            Next

            dtblLeave.AcceptChanges()
            If dtblLeave.Rows.Count > 0 Then
                Dim htmlTGridFuture As HtmlTable = creaGridLists(dtblLeave, strCols, sizeCols, tblControlLeave, True)
                Me.divGridLeaveLastHour.Controls.Add(htmlTGridFuture)
            Else
                Me.lblTitleLeaveLastHourGrid.Text = Me.Language.Translate("NoEmployeesLeaveLastHour", Me.DefaultScope) '"No hay empleados que hayan salido en la última hora"
            End If
        Else
            Me.lblTitleLeaveLastHourGrid.Text = Me.Language.Translate("NoEmployeesLeaveLastHour", Me.DefaultScope) '"No hay empleados que hayan salido en la última hora"
        End If

        'Carrega Grid Accesos incorrectes -------------------------------------
        If dtblIncorrect.Rows.Count > 0 Then
            strCols = New String() {Me.Language.Translate("CurrentZone.ImageEmployee", DefaultScope), Me.Language.Translate("IncAccess.ColumnEmployee", Me.DefaultScope), Me.Language.Translate("IncAccess.Date", Me.DefaultScope), Me.Language.Translate("IncAccess.Detail", Me.DefaultScope)}
            sizeCols = New String() {"45px", "250px", "110px", "205px"}
            cssCols = New String() {"GridStyle-cellheader GridStyle-cellheader-noend", "GridStyle-cellheader GridStyle-cellheader-noend", "GridStyle-cellheader GridStyle-cellheader-noend", "GridStyle-cellheader"}

            Dim tblControlInc As DataTable = New DataTable
            tblControlInc.Columns.Add("NomCamp") 'Nom del Camp
            tblControlInc.Columns.Add("NomParam") 'Parametre que es pasara
            tblControlInc.Columns.Add("Visible") 'Si es te que carregar en les columnes o no...

            tblControlInc.Rows.Add(createRowControl(tblControlInc.NewRow, "ImageEmployee", "", True))
            tblControlInc.Rows.Add(createRowControl(tblControlInc.NewRow, "IDEmployee", "", False))
            tblControlInc.Rows.Add(createRowControl(tblControlInc.NewRow, "IDZone", "", False))
            tblControlInc.Rows.Add(createRowControl(tblControlInc.NewRow, "NameZone", "", False))

            Dim htmlHGridOld As HtmlTable = creaHeaderLists(strCols, sizeCols, cssCols)
            Me.divGridIncorrectAccessHeader.Controls.Add(htmlHGridOld)

            Dim htmlTGridOld As HtmlTable = creaGridLists(dtblIncorrect, strCols, sizeCols, tblControlInc, False)
            Me.divGridIncorrectAccess.Controls.Add(htmlTGridOld)
        Else
            lblTitleIncorrectAccessGrid.Text = Me.Language.Translate("NoIncorrectAccess", Me.DefaultScope) '"No hay marcajes incorrectos en esta zona"
        End If

        'Mostra el TAB seleccionat
        Me.divZoneInfo.Style("display") = ""

    End Sub

    Private Function creaGridLists(ByVal dTable As DataTable, Optional ByVal nomCols() As String = Nothing, Optional ByVal sizeCols() As String = Nothing, Optional ByVal dTblControls As DataTable = Nothing, Optional ByVal editIcons As Boolean = False, Optional ByVal colSpanCol As Integer = 1, Optional ByVal moveIcon As Boolean = False) As HtmlTable
        Try
            Dim hTable As New HtmlTable
            Dim hTRow As New HtmlTableRow
            Dim hTCell As HtmlTableCell
            Dim altRow As String = "2"
            Dim noRegs As Boolean = False

            Dim strClickShow As String = ""         'Href onclick Mode edicio
            Dim strClickMove As String = ""       'Href onclick Mode eliminacio

            hTable.Border = 0
            hTable.CellPadding = 0
            hTable.CellSpacing = 0

            hTable.Attributes("class") = "GridStyle GridGrupos"
            hTable.Style("margin-bottom") = "0"

            Dim NumFoto As Byte = 0

            For n As Integer = 0 To dTable.Rows.Count - 1
                hTRow = New HtmlTableRow
                altRow = IIf(altRow = "1", "2", "1")

                Dim colsizeint As Integer = -1
                For y As Integer = 0 To dTable.Columns.Count - 1
                    'Comproba si es una columna que no es te de visualitzar
                    If dTblControls IsNot Nothing Then
                        Dim dRowSel() As DataRow = dTblControls.Select("NomCamp = '" & dTable.Columns(y).ColumnName & "'")
                        If dRowSel.Length > 0 Then
                            If dRowSel(0).Item("Visible") = False Then Continue For
                        End If
                    End If

                    colsizeint += 1
                    hTCell = New HtmlTableCell
                    hTCell.Width = sizeCols(colsizeint)

                    'Cambia el alternateRow
                    hTCell.Attributes("class") = "GridStyle-cell" & altRow
                    hTCell.Style("display") = "table-cell"

                    hTCell.InnerText = dTable.Rows(n)(y).ToString
                    If IsDate(dTable.Rows(n)(y)) Then
                        hTCell.InnerText = dTable.Rows(n)(y)
                    End If
                    If hTCell.InnerText = "" Then hTCell.InnerText = " "
                    'Si es la ultima columna (per tancar el row)
                    If y = dTable.Columns.Count - 1 Then hTCell.Attributes("class") = hTCell.Attributes("class") & "  GridStyle-endcell" & altRow

                    If dTable.Columns(y).ColumnName = "ImageEmployee" Then
                        If NumFoto < 20 Then
                            If Me.HasFeaturePermissionByEmployee("Employees.NameFoto", Permission.Read, dTable.Rows(n)("IDEmployee")) Then
                                Dim foto As String = roCachePhotoManager.GetPhoto(Me.Page, dTable.Rows(n)("IDEmployee"), "Status", 45)
                                If foto = String.Empty Then
                                    hTCell.InnerHtml = "<img src=""../Base/Images/userStart.png"" width=""45px"" />"
                                Else
                                    hTCell.InnerHtml = "<img src=""" & Me.ResolveUrl(foto) & """ width=""45px"" />"
                                End If
                            Else
                                hTCell.InnerHtml = "<img src=""../Base/Images/userStart.png"" width=""45px"" />"
                            End If
                            NumFoto = NumFoto + 1
                        Else
                            hTCell.InnerHtml = String.Empty
                        End If
                    End If
                    'Carrega la celda al row
                    hTRow.Cells.Add(hTCell)

                    'Dibuixem les columnes de les icones d'edicio
                    If editIcons = True Then
                        If y = dTable.Columns.Count - 1 Then
                            hTCell = New HtmlTableCell
                            hTCell.Width = "40px"
                            hTCell.Attributes("class") = "GridStyle-cellheader"
                            hTCell.Attributes("style") = "border: 0; background-color: #E8EEF7;border-right: solid 1px #D7D7D7;"
                            Dim hAnchorEdit As New HtmlAnchor
                            Dim hAnchorRemove As New HtmlAnchor

                            strClickShow = "showEmployee("
                            strClickMove = "showAccessFilter("
                            strClickShow &= "'', " & roTypes.Any2String(dTable.Rows(n)("IDEmployee")) & ","
                            strClickShow = strClickShow.Substring(0, strClickShow.Length - 1) & ",'" & Configuration.RootUrl & "');"
                            strClickMove &= roTypes.Any2String(dTable.Rows(n)("IDEmployee")) & "," & roTypes.Any2String(dTable.Rows(n)("IDZone")) & ");"

                            hAnchorEdit.Attributes("onclick") = strClickShow
                            hAnchorEdit.Title = Me.Language.Translate("ViewEmployee", Me.DefaultScope) '"Ver empleado"
                            hAnchorEdit.HRef = "javascript: void(0);"
                            hAnchorEdit.InnerHtml = "<img src=""" & Me.Page.ResolveUrl("~/Base/Images/Grid/showemp.png") & """>"

                            hAnchorRemove.Attributes("onclick") = strClickMove
                            hAnchorRemove.Title = Me.Language.Translate("AccessFilterEmp", Me.DefaultScope) '"Mover empleado"
                            hAnchorRemove.HRef = "javascript: void(0);"
                            hAnchorRemove.InnerHtml = "<img src=""" & Me.Page.ResolveUrl("~/Base/Images/Grid/emppunches.png") & """>"

                            hTCell.Controls.Add(hAnchorEdit)
                            hTCell.Controls.Add(hAnchorRemove)

                            hTRow.Cells.Add(hTCell)
                        End If
                    End If
                Next
                hTable.Rows.Add(hTRow)
            Next
            Return hTable
        Catch ex As Exception
            Dim htmlTableErr As New HtmlTable
            Dim htmlTableErrCell As New HtmlTableCell
            Dim htmlTableErrRow As New HtmlTableRow
            htmlTableErrCell.InnerHtml = ex.Message.ToString & " " & ex.StackTrace.ToString
            htmlTableErrRow.Cells.Add(htmlTableErrCell)
            htmlTableErr.Rows.Add(htmlTableErrRow)
            Return htmlTableErr
        End Try
    End Function

    Private Function createRowControl(ByVal nRowControl As DataRow, ByVal NomCamp As String, ByVal NomParam As String, ByVal Visible As Boolean) As DataRow
        nRowControl("NomCamp") = NomCamp
        nRowControl("NomParam") = NomParam
        nRowControl("Visible") = Visible
        Return nRowControl
    End Function

    Private Function creaHeaderLists(ByVal nomCols() As String, ByVal sizeCols() As String, ByVal cssCols() As String) As HtmlTable
        Try
            Dim hTable As New HtmlTable
            Dim hTRow As New HtmlTableRow
            Dim hTCell As HtmlTableCell

            hTable.Border = 0
            hTable.CellPadding = 0
            hTable.CellSpacing = 0

            hTable.Attributes("class") = "GridStyle GridGrupos"
            hTable.Style("border-bottom") = "0"
            hTable.Style("margin-bottom") = "0"

            'TODO: Creo la capcelera (de moment manual... aixo es pot fer GENERIC)
            hTRow = New HtmlTableRow

            For n As Integer = 0 To nomCols.Length - 1
                hTCell = New HtmlTableCell
                hTCell.Attributes("class") = cssCols(n) '"GridStyle-cellheader"
                hTCell.InnerText = nomCols(n)
                If nomCols(n) = "" Then hTCell.InnerText = " "
                hTCell.Width = sizeCols(n)
                hTRow.Cells.Add(hTCell)
            Next

            'Celda d'espai per edicio
            hTCell = New HtmlTableCell
            hTCell.InnerText = " "
            hTCell.Width = "40px"
            hTRow.Cells.Add(hTCell)

            hTable.Rows.Add(hTRow)

            Return hTable
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

End Class