Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Zone
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class PlanesWizard
    Inherits PageBase

    Private Const FeatureAlias As String = "Access.Zones.Definition"

#Region "Declarations"

    Private _Action_Edit As Integer = 0
    Private _Action_Remove As Integer = 1
    Private _Action_Accept As Integer = 2
    Private _Action_Cancel As Integer = 3

    Private _Planes_EditClickIndex As Integer = 0
    Private _Planes_RemoveClickIndex As Integer = 1
    Private _Planes_selectCellIndex As Integer = 4
    Private _Planes_ActionButtons() As String = {"imgEdit", "imgRemove", "imgEditAccept", "imgEditCancel"}
    Private _Planes_EditCellsIndex() As Integer = {3}
    Private _Planes_EditControls() As String = {"Name_TextBox"}
    Private _Planes_CaptionControls() As String = {"Name_Label"}
    Private _Planes_EditFields() As String = {"Name"}

    Private _Planes_IDFieldName As String = "ID_TextBox" 'Columna clau nom (ID)
    Private _Planes_IDField As Integer = 2 'Columna clau posicio (ID)

    Private oPermission As Permission

#End Region

#Region "Properties"

    Private Property PlanesData(Optional ByVal bolReload As Boolean = False) As DataTable
        Get

            Dim tb As DataTable = ViewState("Planes_PlanesData")

            If bolReload OrElse tb Is Nothing Then

                tb = API.ZoneServiceMethods.GetZonePlanes(Me)

                If tb IsNot Nothing AndAlso tb.Rows.Count = 0 Then
                    Dim oNewRow As DataRow = tb.NewRow
                    tb.Rows.Add(oNewRow)
                End If

                ViewState("Planes_PlanesData") = tb

                ' Reestablecer ínidices selección 'grdScheduler'
                ' ...
            End If

            Return tb

        End Get
        Set(ByVal value As DataTable)
            ViewState("Planes_PlanesData") = value
        End Set
    End Property

    Private Property CausesData() As DataView
        Get

            Dim tbCauses As DataTable = ViewState("Planes_CausesData")
            Dim dv As DataView = Nothing
            If tbCauses IsNot Nothing Then
                dv = New DataView(tbCauses)
                dv.Sort = "Name ASC"
            End If

            If dv Is Nothing Then

                Dim tb As DataTable = CausesServiceMethods.GetCauses(Me)

                If tb IsNot Nothing Then

                    dv = New DataView(tb)
                    dv.Sort = "Name ASC"

                    ViewState("Planes_CausesData") = dv.Table

                End If

            End If

            Return dv

        End Get
        Set(ByVal value As DataView)
            If value IsNot Nothing Then
                ViewState("Planes_CausesData") = value.Table
            Else
                ViewState("Planes_CausesData") = Nothing
            End If
        End Set
    End Property

#End Region

#Region "Events"

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("Generic", "~/Base/Scripts/Generic.js", , True)
        Me.InsertExtraJavascript("Cookies", "~/Base/Scripts/Cookies.js", , True)
        Me.InsertExtraJavascript("BrowserDetect", "~/Base/Scripts/BrowserDetect.js", , True)
        Me.InsertExtraJavascript("rgbcolor", "~/Base/Scripts/rgbcolor.js", , True)
    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Me.InsertCssIncludes()

        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)
        If Me.oPermission >= Permission.Read Then

            If Not Me.IsPostBack Then

                Me.PlanesData = Nothing
                Me.CausesData = Nothing

                Me.LoadData()

            End If

            Try
                If Request.Form("__EVENTTARGET").EndsWith(Me._Planes_ActionButtons(Me._Action_Accept)) Then
                    Me.grdPlanes_EditAccept(Val(Request.Form("__EVENTARGUMENT")))
                ElseIf Request.Form("__EVENTTARGET").EndsWith(Me._Planes_ActionButtons(Me._Action_Cancel)) Then
                    Me.grdPlanes_EditCancel(Val(Request.Form("__EVENTARGUMENT")))
                End If
            Catch
            End Try

            ' Establecer seguridad
        Else
            WLHelperWeb.RedirectAccessDenied(True)
        End If

    End Sub

    Protected Sub btAddPlane_ServerClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles btAddPlane.ServerClick

        If Not Me.EditingData() Then

            If Me.PlanesData.Rows.Count = 1 AndAlso HelperWeb.EmptyRow(Me.PlanesData.Rows(0)) Then
                Me.PlanesData.Rows.RemoveAt(0)
            End If
            Dim oNewRow As DataRow = Me.NewPlaneData(Me.PlanesData)
            oNewRow("ID") = -1
            oNewRow("Name") = ""
            Me.PlanesData.Rows.Add(oNewRow)
            Me.LoadData()

            ''Me.UpdateContext(2)

            Me.grdPlanes_EditBegin(Me.grdPlanes.Rows.Count - 1)

        End If

    End Sub

#Region "grdPlanes events"

    Protected Sub grdPlanes_RowCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles grdPlanes.RowCreated

        Dim _gridView As GridView = sender

        Select Case e.Row.RowType
            Case DataControlRowType.Header

            Case DataControlRowType.DataRow

        End Select

    End Sub

    Protected Sub grdPlanes_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles grdPlanes.RowDataBound

        Dim _gridView As GridView = sender

        Select Case e.Row.RowType
            Case DataControlRowType.DataRow

                ' Asignar eventos

                Dim _jsEdit As String
                Dim _jsRemove As String
                Dim js As String

                Dim _EditClickButton As LinkButton = e.Row.Cells(Me._Planes_EditClickIndex).Controls(0)
                _jsEdit = ClientScript.GetPostBackClientHyperlink(_EditClickButton, "")
                Dim _RemoveClickButton As LinkButton = e.Row.Cells(Me._Planes_RemoveClickIndex).Controls(0)
                _jsRemove = ClientScript.GetPostBackClientHyperlink(_RemoveClickButton, "")

                Dim _EditButton As HtmlImage = e.Row.Cells(Me._Planes_selectCellIndex).FindControl(Me._Planes_ActionButtons(Me._Action_Edit))
                _EditButton.Attributes("onclick") = _jsEdit
                _EditButton.Attributes("style") += "cursor:pointer;"

                Dim _RemoveButton As HtmlImage = e.Row.Cells(Me._Planes_selectCellIndex).FindControl(Me._Planes_ActionButtons(Me._Action_Remove))
                _RemoveButton.Attributes("onclick") = _jsRemove
                _RemoveButton.Attributes("style") += "cursor:pointer;"

                Dim _jsCommand As String

                Dim _EditAcceptButton As HtmlImage = e.Row.Cells(Me._Planes_selectCellIndex).FindControl(Me._Planes_ActionButtons(Me._Action_Accept))
                _jsCommand = ClientScript.GetPostBackClientHyperlink(_EditAcceptButton, "")
                js = _jsCommand.Insert(_jsCommand.Length - 2, e.Row.RowIndex)
                If js.StartsWith("javascript:") Then
                    js = "javascrip: if (CheckConvertControls('') == true) { " & js.Substring(CStr("javascript:").Length) & " }"
                End If
                _EditAcceptButton.Attributes("onclick") = js
                _EditAcceptButton.Attributes("style") += "cursor:pointer;"

                Dim _EditCancelButton As HtmlImage = e.Row.Cells(Me._Planes_selectCellIndex).FindControl(Me._Planes_ActionButtons(Me._Action_Cancel))
                _jsCommand = ClientScript.GetPostBackClientHyperlink(_EditCancelButton, "")
                js = _jsCommand.Insert(_jsCommand.Length - 2, e.Row.RowIndex)
                _EditCancelButton.Attributes("onclick") = js
                _EditCancelButton.Attributes("style") += "cursor:pointer;"

                Dim oPlaneData As DataRowView = e.Row.DataItem

                ' Establecemos controles
                Dim lblID As Label = e.Row.Cells(4).FindControl("ID_Label")
                Dim txtID As HtmlInputText = e.Row.Cells(4).FindControl("ID_TextBox")
                Dim lblName As Label = e.Row.Cells(4).FindControl("Name_Label")
                Dim txtName As HtmlInputText = e.Row.Cells(4).FindControl("Name_TextBox")

                If lblID IsNot Nothing Then
                    If Not IsDBNull(oPlaneData("ID")) Then
                        lblID.Text = oPlaneData("ID")
                    Else
                        lblID.Text = ""
                    End If
                End If
                If txtID IsNot Nothing Then
                    txtID.Value = lblID.Text
                End If

                If lblName IsNot Nothing Then
                    If Not IsDBNull(oPlaneData("Name")) Then
                        lblName.Text = oPlaneData("Name")
                    Else
                        lblName.Text = ""
                    End If
                End If
                If txtName IsNot Nothing Then
                    txtName.Value = lblName.Text
                End If

                For columnIndex As Integer = 0 To e.Row.Cells.Count - 1
                    ''e.Row.Cells(columnIndex).Attributes.Add("oncontextmenu", strContextMenuScript)
                    'js = _jsCommand.Insert(_jsCommand.Length - 2, columnIndex.ToString)
                    js = _jsEdit
                    e.Row.Cells(columnIndex).Attributes("onclick") = "showImage('" & oPlaneData("ID") & "');"
                    e.Row.Cells(columnIndex).Attributes("style") += "cursor:pointer;"
                Next

                ' Traducir manualmente controles de la grid
                _EditButton.Attributes("title") = Me.Language.Keyword("Button.Edit")
                _RemoveButton.Attributes("title") = Me.Language.Keyword("Button.Delete")
                _EditAcceptButton.Attributes("title") = Me.Language.Keyword("Button.Apply")
                _EditCancelButton.Attributes("title") = Me.Language.Keyword("Button.Cancel")

            Case DataControlRowType.Footer

        End Select

    End Sub

    Protected Sub grdPlanes_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles grdPlanes.RowCommand

        Dim _gridView As GridView = sender

        Select Case e.CommandName
            Case "EditClick"

                ' Get the row index
                Dim _rowIndex As Integer = Integer.Parse(e.CommandArgument.ToString())
                ' Parse the event argument (added in RowDataBound) to get the selected column index
                Dim _columnIndex As Integer = -1 ''Integer.Parse(Request.Form("__EVENTARGUMENT"))
                ' Set the Gridview selected index
                _gridView.SelectedIndex = _rowIndex
                ' Bind the Gridview
                _gridView.DataSource = Me.PlanesData
                _gridView.DataBind()

                Me.grdPlanes_EditBegin(_rowIndex, _columnIndex)

            Case "RemoveClick"
                ' Get the row index
                Dim _rowIndex As Integer = Integer.Parse(e.CommandArgument.ToString())

                grdPlanes_Remove(_rowIndex)

                'Case "UpClick"
                '    ' Get the row index
                '    Dim _rowIndex As Integer = Integer.Parse(e.CommandArgument.ToString())

                '    grdPlanes_Up(_rowIndex)

                'Case "DownClick"
                '    ' Get the row index
                '    Dim _rowIndex As Integer = Integer.Parse(e.CommandArgument.ToString())

                '    grdPlanes_Down(_rowIndex)

        End Select

    End Sub

    Private Sub grdPlanes_EditBegin(ByVal intRowIndex As Integer, Optional ByVal intColIndex As Integer = -1)

        Dim _gridView As GridView = Me.grdPlanes

        Dim oControl As Control
        Dim intCell As Integer
        For intIndex As Integer = 0 To Me._Planes_EditCellsIndex.Length - 1
            intCell = Me._Planes_EditCellsIndex(intIndex)
            ' Ocultar los controls no editables
            oControl = _gridView.Rows(intRowIndex).Cells(intCell).FindControl(Me._Planes_CaptionControls(intIndex))
            If oControl IsNot Nothing Then oControl.Visible = False
            ' Clear the attributes from the selected cell to remove the click event
            _gridView.Rows(intRowIndex).Cells(intCell).Attributes.Clear()
            ' Hacer visibles los controles editables
            oControl = _gridView.Rows(intRowIndex).Cells(intCell).FindControl(Me._Planes_EditControls(intIndex))
            If oControl IsNot Nothing Then oControl.Visible = True

        Next

        '_gridView.Columns(Me._Entries_selectCellIndex).HeaderStyle.Width = "50"
        '_gridView.Columns(Me._Entries_selectCellIndex).ItemStyle.Width = "50"

        ' Poner las imágenes de editar y eliminar invisibles
        Dim oEditButton As HtmlImage = _gridView.Rows(intRowIndex).Cells(Me._Planes_selectCellIndex).FindControl(Me._Planes_ActionButtons(Me._Action_Edit))
        oEditButton.Visible = False
        Dim oRemoveButton As HtmlImage = _gridView.Rows(intRowIndex).Cells(Me._Planes_selectCellIndex).FindControl(Me._Planes_ActionButtons(Me._Action_Remove))
        oRemoveButton.Visible = False
        ' Poner las imágenes de aceptar y cancelar visibles
        Dim oEditAcceptButton As HtmlImage = _gridView.Rows(intRowIndex).Cells(Me._Planes_selectCellIndex).FindControl(Me._Planes_ActionButtons(Me._Action_Accept))
        oEditAcceptButton.Visible = True
        Dim oEditCancelButton As HtmlImage = _gridView.Rows(intRowIndex).Cells(Me._Planes_selectCellIndex).FindControl(Me._Planes_ActionButtons(Me._Action_Cancel))
        oEditCancelButton.Visible = True
        ' Poner las imágenes de subir y bajar invisibles
        'Dim oUpButton As HtmlImage = _gridView.Rows(intRowIndex).Cells(Me._Planes_selectCellIndex + 1).FindControl(Me._Planes_ActionButtons(Me._Action_Up))
        'oUpButton.Visible = False
        'Dim oDownButton As HtmlImage = _gridView.Rows(intRowIndex).Cells(Me._Planes_selectCellIndex + 1).FindControl(Me._Planes_ActionButtons(Me._Action_Down))
        'oDownButton.Visible = False

    End Sub

    Private Sub grdPlanes_EditAccept(ByVal intRowIndex As Integer)

        Dim _gridView As GridView = Me.grdPlanes

        Try

            If _gridView.PageCount > 1 Then
                intRowIndex = (_gridView.PageIndex * _gridView.PageSize) + intRowIndex
            End If

            ' Insertamos los datos a la tabla
            Dim dt As DataTable = Me.PlanesData()
            Dim dr As DataRow = dt.Rows(intRowIndex)
            dr.BeginEdit()

            ' Obtenemos los valores de las columnas editables
            Dim oControl As Control
            Dim intCell As Integer
            For intIndex As Integer = 0 To Me._Planes_EditCellsIndex.Length - 1

                intCell = Me._Planes_EditCellsIndex(intIndex)

                oControl = _gridView.Rows(intRowIndex).Cells(intCell).FindControl(Me._Planes_EditControls(intIndex))
                If oControl IsNot Nothing Then

                    Dim oIDCtl As Control = _gridView.Rows(intRowIndex).Cells(_Planes_IDField).FindControl(_Planes_IDFieldName)
                    Dim IDZonePlane As String = CType(oIDCtl, HtmlInputText).Value
                    Dim oZonePlane As roZonePlane

                    If IDZonePlane <> "" And IDZonePlane <> "-1" Then
                        oZonePlane = API.ZoneServiceMethods.GetZonePlaneByID(Me.Page, IDZonePlane, False)
                    Else
                        oZonePlane = New roZonePlane
                        oZonePlane.ID = -1
                    End If

                    If TypeOf oControl Is HtmlInputText Then
                        dr(Me._Planes_EditFields(intIndex)) = CType(oControl, HtmlInputText).Value
                        oZonePlane.Name = CType(oControl, HtmlInputText).Value
                    End If

                    API.ZoneServiceMethods.SaveZonePlane(Me.Page, oZonePlane, True)
                    dr("ID") = oZonePlane.ID

                    'If Not API.ZoneServiceMethods.SaveZonePlane(Me.Page, oZonePlane) Then
                    '    HelperWeb.ShowError(Me.Page, API.ZoneServiceMethods.oState)
                    'End If

                End If

            Next

            dr.EndEdit()

            Me.PlanesData() = dt

            ' Refrescamos la grid
            _gridView.DataSource = dt
            _gridView.DataBind()
        Catch ex As Exception
            ' Repopulate the GridView
            _gridView.DataSource = Me.PlanesData()
            _gridView.DataBind()

        End Try

    End Sub

    Private Sub grdPlanes_EditCancel(ByVal intRowIndex As Integer)

        Dim _gridView As GridView = Me.grdPlanes

        _gridView.DataSource = Me.PlanesData()
        _gridView.DataBind()

    End Sub

    Private Sub grdPlanes_Remove(ByVal intRowIndex As Integer)

        ' Verificamos que no estemos en edición
        If Not Me.EditingData() Then

            If intRowIndex > -1 Then

                Dim Rows() As DataRow = Me.PlanesData.Select("", "", DataViewRowState.CurrentRows)
                If Rows.Length > intRowIndex Then
                    If Not Rows(intRowIndex)("ID") Is DBNull.Value AndAlso Rows(intRowIndex)("ID") <> "-1" Then
                        If API.ZoneServiceMethods.DeleteZonePlaneByID(Me.Page, Rows(intRowIndex)("ID"), True) Then
                            Rows(intRowIndex).Delete()
                        Else
                            HelperWeb.ShowError(Me.Page, roWsUserManagement.SessionObject.States.ZoneState)
                        End If
                    Else
                        Rows(intRowIndex).Delete()
                    End If
                End If

                'Dim oRow As DataRow = Me.EntriesData.Rows(intRowIndex)

                'If Me.EntriesData.Rows(intRowIndex).RowState <> DataRowState.Added Then
                '    oRow.Delete()
                'Else
                '    Me.EntriesData.Rows.RemoveAt(intRowIndex)
                'End If

                Dim bolEmpty As Boolean = True
                For Each oRow As DataRow In Me.PlanesData.Rows
                    If oRow.RowState <> DataRowState.Deleted Then
                        bolEmpty = False
                        Exit For
                    End If
                Next
                If bolEmpty Then
                    Me.PlanesData.Rows.Add(Me.PlanesData.NewRow)
                End If

                Me.LoadData()

            End If

        End If

    End Sub

    Private Sub grdPlanes_Up(ByVal intRowIndex As Integer)

        ' Verificamos que no estemos en edición
        If Not Me.EditingData() Then

            If intRowIndex > -1 Then

                If intRowIndex > 0 Then

                    Dim tb As DataTable = Me.PlanesData

                    Dim oRow As DataRow = tb.NewRow
                    oRow("ID") = tb.Rows(intRowIndex)("ID")
                    oRow("Name") = tb.Rows(intRowIndex)("Name")

                    tb.Rows.RemoveAt(intRowIndex)
                    tb.Rows.InsertAt(oRow, intRowIndex - 1)

                    Me.PlanesData = tb

                    Me.LoadData()

                End If

            End If

        End If

    End Sub

    Private Sub grdPlanes_Down(ByVal intRowIndex As Integer)

        ' Verificamos que no estemos en edición
        If Not Me.EditingData() Then

            If intRowIndex > -1 Then

                If intRowIndex < Me.PlanesData.Rows.Count - 1 Then

                    Dim tb As DataTable = Me.PlanesData

                    Dim oRow As DataRow = tb.NewRow
                    oRow("ID") = tb.Rows(intRowIndex)("ID")
                    oRow("Name") = tb.Rows(intRowIndex)("Name")

                    tb.Rows.InsertAt(oRow, intRowIndex + 2)
                    tb.Rows.RemoveAt(intRowIndex)

                    Me.PlanesData = tb

                    Me.LoadData()

                End If

            End If

        End If

    End Sub

#End Region

#End Region

#Region "Methods"

    Private Sub LoadData()

        With Me.grdPlanes
            .DataSourceID = ""
            .DataSource = Me.PlanesData
            .DataBind()
        End With
        HelperWeb.EmptyGridFix(Me.grdPlanes)

    End Sub

    Private Function EditingData() As Boolean

        Dim bolEditing As Boolean = False

        Dim oImage As HtmlImage
        For Each oRow As GridViewRow In Me.grdPlanes.Rows
            oImage = oRow.FindControl(Me._Planes_ActionButtons(Me._Action_Accept))
            If oImage IsNot Nothing Then
                bolEditing = oImage.Visible
                If bolEditing Then Exit For
            End If
        Next

        If bolEditing Then
            HelperWeb.ShowMessage(Me, "", Me.Language.Translate("DataEditing.Message", Me.DefaultScope))
        End If

        Return bolEditing

    End Function

    Private Function NewPlaneData(ByVal tbPlanes As DataTable) As DataRow

        Dim oNew As DataRow = tbPlanes.NewRow
        With oNew
            .Item("ID") = DBNull.Value
            .Item("Name") = DBNull.Value
        End With
        Return oNew

    End Function

    Private Sub SaveData()
        Dim bolRet As Boolean = True

        Dim oPlane As roZonePlane = Nothing
        For Each oRow As DataRow In Me.PlanesData.Rows
            If oRow.RowState <> DataRowState.Deleted Then
                oPlane = New roZonePlane
                oPlane.ID = oRow("ID")
                oPlane.Name = oRow("Name")
                'oPlane.PlaneImage = oRow("PlaneImage")

                If Not API.ZoneServiceMethods.SaveZonePlane(Me, oPlane, True) Then
                    bolRet = False
                    Exit For
                End If
            End If
        Next

        If bolRet Then
            Me.CanClose = True
            Me.MustRefresh = "1"
        End If

    End Sub

#End Region

    Protected Sub btClose_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btClose.Click
        Me.MustRefresh = "2"
        Me.CanClose = True
    End Sub

End Class