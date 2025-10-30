Imports DevExpress.Web
Imports DevExpress.Web.Data
Imports Robotics.Base.VTBusiness.Group
Imports Robotics.Base.VTEmployees.Employee
Imports Robotics.Base.VTSelectorManager
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions.VTLiveTasks
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class Tasks_ReviewBusinessCenters
    Inherits PageBase

#Region "Properties"

    Private ReadOnly Property GetCauses(Optional ByVal bolReload As Boolean = False) As DataTable
        Get
            Dim tbCauses As DataTable = ViewState("ReviewBusinessCentersFilter_Causes")

            If bolReload OrElse tbCauses Is Nothing Then
                tbCauses = CausesServiceMethods.GetCauses(Me.Page, "")
                ViewState("ReviewBusinessCentersFilter_Causes") = tbCauses
            End If

            Return tbCauses
        End Get
    End Property

    Private ReadOnly Property GetGroups() As DataTable
        Get
            Dim Groups As DataTable = ViewState("ReviewBusinessCentersFilter_Groups")
            If Groups Is Nothing Then
                Groups = API.EmployeeGroupsServiceMethods.GetGroups(Me, "Calendar.JustifyIncidences")
                ViewState("ReviewBusinessCentersFilter_Groups") = Groups
            End If
            Return Groups
        End Get
    End Property

    Private Property ErrorExists As Boolean
        Get
            Dim val As Object = HttpContext.Current.Session("ErrorExists")
            If val IsNot Nothing Then
                Return roTypes.Any2Boolean(val)
            Else
                HttpContext.Current.Session("ErrorExists") = False
                Return False
            End If
        End Get
        Set(value As Boolean)
            HttpContext.Current.Session("ErrorExists") = value
        End Set
    End Property

    Private Property ErrorDescription As String
        Get
            Dim val As Object = HttpContext.Current.Session("ErrorDescription")
            If val IsNot Nothing Then
                Return roTypes.Any2String(val)
            Else
                HttpContext.Current.Session("ErrorDescription") = False
                Return False
            End If
        End Get
        Set(value As String)
            HttpContext.Current.Session("ErrorDescription") = value
        End Set
    End Property


#End Region

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("Ajax", "~/Base/Scripts/Ajax.js")
        Me.InsertExtraJavascript("GenericData", "~/Base/Scripts/GenericData.js")
        Me.InsertExtraJavascript("roTreeState", "~/Base/Scripts/rocontrols/roTrees/roTreeState.js")
        Me.InsertExtraJavascript("webUserForms", "~/Base/Scripts/webUserForms.js")
        Me.InsertExtraJavascript("jsDate", "~/Base/Scripts/jsDate.js")
        'Me.InsertExtraJavascript("Requests", "~/Requests/Scripts/Requests.js")
        Me.InsertExtraJavascript("roRequestListParams", "~/Requests/Scripts/roRequestListParams.js")
        Me.InsertExtraJavascript("SchedulerDates", "~/Scheduler/Scripts/SchedulerDates.js")
        Me.InsertExtraJavascript("ReviewBusinessCenters", "~/Tasks/Scripts/ReviewBusinessCenters.js")
        Me.InsertExtraJavascript("bcSelector", "~/Base/Scripts/frmBusinessCenterSelector.js")
    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Comprueba la licencia
        'If HelperSession.GetFeatureIsInstalledFromApplication("Feature\Absences") = False Then
        '    WLHelperWeb.RedirectAccessDenied(False)
        '    Exit Sub
        'End If

        If Not IsPostBack AndAlso Not IsCallback Then
            LoadCombos()

            'Dim oPermission As UserAdminService.Permission = Me.GetFeaturePermission("Employees")
            'If oPermission < UserAdminService.Permission.Write Then
            '    WLHelperWeb.RedirectAccessDenied(False)
            '    Exit Sub
            'End If

            'NO TOCAR: Cargar propiedades sin postback para añadirlas al viewstate y que esten disponibles mas adelante en callbacks
            Dim tb As DataTable = Me.GetCauses()
            tb = Me.GetGroups()
            ' FIN

            Session("ReviewBusinessCentersFilter_AbsencesDataTable") = Nothing
            Session("ReviewBusinessCentersFilter_LastEmployeesUsed") = Nothing
            Session("ReviewBusinessCentersFilter_LastDateInfUsed") = Nothing
            Session("ReviewBusinessCentersFilter_LastCausesUsed") = Nothing
            Session("ReviewBusinessCentersFilter_LastDateSupUsed") = Nothing

            'INICIALIZAR COOKIES DE USO PARA EL SELECTOR DE EMPLEADOS
            HelperWeb.roSelector_Initialize("objContainerTreeV3_treeEmpReviewBusinessCenters")
            HelperWeb.roSelector_Initialize("objContainerTreeV3_treeEmpReviewBusinessCentersGrid")

            txtDateInf.Value = DateTime.Today
            txtDateSup.Value = DateTime.Today

            Me.ListCauses.ValueField = "ID"
            Me.ListCauses.TextField = "Name"
            Me.ListCauses.DataSource = GetCauses(True)
            Me.ListCauses.DataBind()

            hdnAllEmployees.Value = Me.Language.Translate("AllEmployees", DefaultScope)
            hdnAllCauses.Value = Me.Language.Translate("AllCauses", DefaultScope)
            hdnCauseSelectedText.Value = Me.Language.Translate("SelectedCause", DefaultScope)
            hdnBusinessCentersSelectedText.Value = " " & Language.Translate("BusinessCentersSelected", DefaultScope)
            hdnBusinessCentersSelected.Value = "0"
            txtEmployees.Text = hdnAllEmployees.Value
            txtCauses.Text = hdnAllCauses.Value

            Dim strDateAux As String = HelperWeb.GetShortDateFormat.ToLower
            dtFormatText.Value = strDateAux
            strDateAux = Replace(strDateAux, "ddd", "d")
            strDateAux = Replace(strDateAux, "dd", "d")
            strDateAux = Replace(strDateAux, "mm", "m")
            strDateAux = Replace(strDateAux, "yyyy", "y")
            strDateAux = Replace(strDateAux, "yy", "y")
            strDateAux = Replace(strDateAux, "/", "")
            strDateAux = Replace(strDateAux, "-", "")
            strDateAux = Replace(strDateAux, "d", "0")
            strDateAux = Replace(strDateAux, "m", "1")
            strDateAux = Replace(strDateAux, "y", "2")
            dtFormat.Value = strDateAux

            CreateColumnsIncidences()
            IncidencesDataTable = Nothing

            FillCombos()

            frmBusinessCenterSelector.ClearSessionVariables()
            frmBusinessCenterSelector.SetPassport(WLHelperWeb.CurrentPassportID, True, False, False, False)

        End If

        GetJustCausesData()
        GetJustCausesDataAll()

        BindGridIncidences(False)
    End Sub

    Private Sub BindGridIncidences(Optional ByVal bolReload As Boolean = False)
        Me.GridIncidences.DataSource = Me.IncidencesDataView(bolReload)
        Me.GridIncidences.DataBind()
    End Sub

    Private ReadOnly Property IncidencesDataView(Optional ByVal bolReload As Boolean = False) As DataView
        Get
            Dim tb As DataTable = Me.IncidencesDataTable(bolReload)
            Dim dv As DataView = Nothing
            If tb IsNot Nothing Then
                dv = New DataView(tb)
                dv.Sort = "EmployeesName, Date, BeginTime"
            End If

            Return dv

        End Get
    End Property

    Private Property IncidencesDataTable(Optional ByVal bolReload As Boolean = False) As DataTable
        Get

            Dim tbIncidences As DataTable = Session("ReviewBusinessCenters_IncidencesDatatable")

            If bolReload Then

                CreateColumnsIncidences()
                IncidencesDataTable = Nothing

                Dim DateIni As DateTime = txtDateInf.Date
                Dim DateFin As DateTime = txtDateSup.Date

                Dim directEmployeesIds As New Generic.List(Of Integer)
                Dim directGroupsIds As New Generic.List(Of Integer)
                Dim totalEmployeesIds As List(Of Integer) = New List(Of Integer)

                'Dim oCausesList As Generic.List(Of Object) = grdCauses.GetSelectedFieldValues("ID")

                Dim strCausesFilter As String = ""
                'If oCausesList.Count > 0 Then strCausesFilter = String.Join(",", oCausesList)

                'obtener todos los empleados de los grupos seleccionados en el arbol v3
                roSelectorManager.ExtractIdsFromSelectionString(Me.hdnEmployees.Value, directEmployeesIds, directGroupsIds)
                totalEmployeesIds.AddRange(directEmployeesIds)
                If directGroupsIds IsNot Nothing Then
                    Dim strFilter As String = Me.hdnFilter.Value
                    Dim strFilterUser As String = Me.hdnFilterUser.Value
                    Dim indirectEmployeesIds As Generic.List(Of Integer) = API.EmployeeGroupsServiceMethods.GetEmployeeListFromGroupRecursive(Me, directGroupsIds.ToArray, "Calendar.JustifyIncidences", "U", strFilter, strFilterUser)
                    totalEmployeesIds.AddRange(indirectEmployeesIds)
                End If

                Dim commaSeparatedEmployeeIds As String
                If totalEmployeesIds.Count > 0 Then
                    commaSeparatedEmployeeIds = "-1," & String.Join(",", totalEmployeesIds.Select(Function(id) id.ToString()))
                Else
                    commaSeparatedEmployeeIds = "-1"
                End If

                '============ JUSTITICACIONES
                If hdnCausesSelected.Value <> String.Empty Then
                    strCausesFilter = hdnCausesSelected.Value
                End If
                '============ FIN JUSTITICACIONES

                'Centros de Coste
                Dim strBusinessCenterFilter As String = hdnBusinessCentersSelected.Value

                If strBusinessCenterFilter.Length > 0 Then
                    strBusinessCenterFilter += ",0"
                Else
                    strBusinessCenterFilter = "0"
                End If

                tbIncidences = API.EmployeeServiceMethods.GetMassCenters(Me.Page, commaSeparatedEmployeeIds, DateIni, DateFin, strCausesFilter, strBusinessCenterFilter, String.Join(",", directEmployeesIds.Select(Function(id) id.ToString())), String.Join(",", directGroupsIds.Select(Function(id) id.ToString())))

                'añado columna de hora en formato hora desde formato double
                tbIncidences.Columns.Add(New DataColumn("ValueHora", GetType(String)))
                tbIncidences.Columns.Add(New DataColumn("ValueHoraEditable", GetType(String)))

                'añado columna de texto de incidencia
                tbIncidences.Columns.Add(New DataColumn("TextoIncidencia", GetType(String)))

                'añado columna de la clave
                tbIncidences.Columns.Add(New DataColumn("Clave", GetType(String)))

                tbIncidences.Columns.Add(New DataColumn("IDCenterOld", GetType(Double)))

                Dim IncidenceText As String

                For Each r As DataRow In tbIncidences.Rows
                    r("ValueHora") = roConversions.ConvertHoursToTime(CDbl(r("Value")))
                    r("ValueHoraEditable") = roConversions.ConvertHoursToTime(CDbl(r("Value")))
                    r("IDCenterOld") = CDbl(r("IDCenter"))
                    IncidenceText = Me.Language.Keyword("Incidence." & roTypes.Any2String(r("IDType")))
                    If IncidenceText <> String.Empty Then
                        r("TextoIncidencia") = IncidenceText.Substring(0, 1).ToUpper & IncidenceText.Substring(1)
                    Else
                        r("TextoIncidencia") = String.Empty
                    End If
                    r("Clave") = Guid.NewGuid()
                Next

                'añade campo clave a la tabla
                If Not tbIncidences Is Nothing Then
                    tbIncidences.PrimaryKey = New DataColumn() {tbIncidences.Columns("Clave")}

                    If tbIncidences.Rows.Count = 0 Then 'Añadir fila vacía porque si el grid se carga sin filas, el combobox de incidencias no se abre (error de ASPXGriwView)
                    End If
                    tbIncidences.AcceptChanges()

                End If

                Session("ReviewBusinessCenters_IncidencesDatatable") = tbIncidences
            End If

            Return tbIncidences

        End Get
        Set(ByVal value As DataTable)
            Session("ReviewBusinessCenters_IncidencesDatatable") = value
        End Set
    End Property

    Private Function GetJustCausesData(Optional ByVal bolReload As Boolean = False) As DataView

        If bolReload Or Session("ReviewBusinessCenters_JustCausesData") Is Nothing Then
            Dim dv As DataView = Nothing
            Dim returnTb As New DataTable
            returnTb.Columns.Add(New DataColumn("ID", GetType(Short)))
            returnTb.Columns.Add(New DataColumn("Name", GetType(String)))
            Dim oNewRow As DataRow = returnTb.NewRow

            Dim tb As DataTable = CausesServiceMethods.GetCauses(Me.Page, "", True)
            If tb IsNot Nothing Then
                For Each oRow As DataRow In tb.Rows
                    oNewRow = returnTb.NewRow
                    With oNewRow
                        .Item("ID") = oRow("ID")
                        .Item("Name") = oRow("Name")
                    End With
                    returnTb.Rows.Add(oNewRow)
                Next

                Session("ReviewBusinessCenters_JustCausesData") = returnTb

                dv = New DataView(returnTb)
                dv.Sort = "Name ASC"
            End If

            Return dv
        Else
            Dim dtReturn As DataTable = Session("ReviewBusinessCenters_JustCausesData")
            Dim dv As DataView = New DataView(dtReturn)
            dv.Sort = "Name ASC"
            Return dv
        End If
    End Function

    Protected Sub ProcessQueryString()
    End Sub

    Private Sub CreateColumnsIncidences()

        Dim GridColumn As GridViewDataTextColumn
        Dim GridColumnCommand As GridViewCommandColumn
        Dim GridComboCommand As GridViewDataComboBoxColumn
        Dim GridColumnTime As GridViewDataTimeEditColumn
        Dim GridColumnsDate As GridViewDataDateColumn

        Dim VisibleIndex As Integer = 0

        Me.GridIncidences.Columns.Clear()
        Me.GridIncidences.KeyFieldName = "Clave"
        Me.GridIncidences.SettingsText.EmptyDataRow = " "

        Me.GridIncidences.SettingsCommandButton.DeleteButton.Image.Url = "~/Base/Images/Grid/remove.png"
        Me.GridIncidences.SettingsCommandButton.EditButton.Image.Url = "~/Base/Images/Grid/edit.png"
        Me.GridIncidences.SettingsCommandButton.UpdateButton.Image.Url = "~/Base/Images/Grid/save.png"
        Me.GridIncidences.SettingsCommandButton.CancelButton.Image.Url = "~/Base/Images/Grid/cancel.png"
        Me.GridIncidences.Settings.VerticalScrollBarMode = ScrollBarMode.Auto

        Me.GridIncidences.SettingsEditing.Mode = GridViewEditingMode.Inline

        'Clave
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "Clave"
        GridColumn.FieldName = "Clave"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.Visible = False
        GridColumn.Width = 40
        Me.GridIncidences.Columns.Add(GridColumn)

        'Command buttons
        GridColumnCommand = New GridViewCommandColumn()
        GridColumnCommand.ShowApplyFilterButton = False
        GridColumnCommand.ShowClearFilterButton = False
        GridColumnCommand.ShowCancelButton = False
        GridColumnCommand.ShowDeleteButton = False
        GridColumnCommand.ShowEditButton = False
        GridColumnCommand.SelectAllCheckboxMode = GridViewSelectAllCheckBoxMode.AllPages
        GridColumnCommand.ShowSelectCheckbox = True

        GridColumnCommand.Caption = " "
        GridColumnCommand.VisibleIndex = VisibleIndex
        GridColumnCommand.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCommand.Width = 10
        VisibleIndex = VisibleIndex + 1
        Me.GridIncidences.Columns.Add(GridColumnCommand)

        'Empleado
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridIncidences.Column.Employee", DefaultScope)
        GridColumn.FieldName = "EmployeesName"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Width = 30
        Me.GridIncidences.Columns.Add(GridColumn)

        'Contrato
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridIncidences.Column.Contract", DefaultScope)
        GridColumn.FieldName = "IDContract"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.Visible = False
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Width = 25
        Me.GridIncidences.Columns.Add(GridColumn)

        'Fecha
        GridColumnsDate = New GridViewDataDateColumn()
        GridColumnsDate.Caption = Me.Language.Translate("GridIncidences.Column.Date", DefaultScope)
        GridColumnsDate.FieldName = "Date"
        GridColumnsDate.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnsDate.ReadOnly = True
        GridColumnsDate.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnsDate.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnsDate.Width = 20
        Me.GridIncidences.Columns.Add(GridColumnsDate)

        ' Fichajes
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridIncidences.Column.Punches", DefaultScope)
        GridColumn.FieldName = "PunchesLst"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Width = 28
        Me.GridIncidences.Columns.Add(GridColumn)

        'Incidencia
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridIncidences.Column.Incidence", DefaultScope) '"Incidencia"
        GridColumn.FieldName = "TextoIncidencia"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Width = 23
        Me.GridIncidences.Columns.Add(GridColumn)

        'Zona Horaria
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridIncidences.Column.TimeZoneName", DefaultScope) '"Zona Horaria"
        GridColumn.FieldName = "TimeZoneName"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.Visible = False
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Width = 28
        Me.GridIncidences.Columns.Add(GridColumn)

        'Inicio
        GridColumnTime = New GridViewDataTimeEditColumn
        GridColumnTime.Caption = Me.Language.Translate("GridIncidences.Column.BeginTime", DefaultScope) '"Inicio"
        GridColumnTime.FieldName = "BeginTime"
        GridColumnTime.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnTime.ReadOnly = True
        GridColumnTime.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnTime.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnTime.Width = 12
        Me.GridIncidences.Columns.Add(GridColumnTime)

        'Final
        GridColumnTime = New GridViewDataTimeEditColumn
        GridColumnTime.Caption = Me.Language.Translate("GridIncidences.Column.EndTime", DefaultScope) '"Final"
        GridColumnTime.FieldName = "EndTime"
        GridColumnTime.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnTime.ReadOnly = True
        GridColumnTime.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnTime.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnTime.Width = 12
        Me.GridIncidences.Columns.Add(GridColumnTime)

        'Valor Hora NO Editable
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridIncidences.Column.ValueHora", DefaultScope) '"Valor"
        GridColumn.FieldName = "ValueHora"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.Width = 12
        Me.GridIncidences.Columns.Add(GridColumn)

        'Justificación
        GridComboCommand = New GridViewDataComboBoxColumn()
        GridComboCommand.Caption = Me.Language.Translate("GridIncidences.Column.Justification", DefaultScope) '"Justificación"
        GridComboCommand.FieldName = "IDCause"
        VisibleIndex = VisibleIndex + 1
        GridComboCommand.PropertiesComboBox.TextField = "Name"
        GridComboCommand.PropertiesComboBox.ValueField = "ID"
        GridComboCommand.PropertiesComboBox.ValueType = GetType(Short)
        GridComboCommand.PropertiesComboBox.DataSource = Me.GetJustCausesDataAll()
        GridComboCommand.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridComboCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridComboCommand.Width = 33
        GridComboCommand.ReadOnly = True
        GridComboCommand.PropertiesComboBox.IncrementalFilteringMode = IncrementalFilteringMode.Contains
        Me.GridIncidences.Columns.Add(GridComboCommand)

        'Valor Hora Editable
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridIncidences.Column.ValueHoraEditable", DefaultScope) '"Valor"
        GridColumn.FieldName = "ValueHoraEditable"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = False
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.Width = 12
        GridColumn.PropertiesTextEdit.MaskSettings.Mask = "<-9999..9999>:<00..59>"
        GridColumn.PropertiesTextEdit.Width = 60
        Me.GridIncidences.Columns.Add(GridColumn)

        'Centro de coste
        GridComboCommand = New GridViewDataComboBoxColumn()
        GridComboCommand.Caption = Me.Language.Translate("GridIncidences.Column.Center", DefaultScope) '"Centro de coste"
        GridComboCommand.FieldName = "IDCenter"
        VisibleIndex = VisibleIndex + 1
        GridComboCommand.PropertiesComboBox.TextField = "Name"
        GridComboCommand.PropertiesComboBox.ValueField = "ID"
        GridComboCommand.PropertiesComboBox.ValueType = GetType(Short)
        GridComboCommand.PropertiesComboBox.DataSource = Me.GetCentersDataAll(True)
        GridComboCommand.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridComboCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridComboCommand.Width = 25
        GridComboCommand.ReadOnly = False
        GridComboCommand.PropertiesComboBox.IncrementalFilteringMode = IncrementalFilteringMode.Contains
        Me.GridIncidences.Columns.Add(GridComboCommand)

        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "IDCenterOld"
        GridColumn.FieldName = "IDCenterOld"
        GridColumn.ReadOnly = True
        GridColumn.Visible = False
        Me.GridIncidences.Columns.Add(GridColumn)

        'IDRelatedIncidence
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "IDRelatedIncidence"
        GridColumn.FieldName = "IDRelatedIncidence"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.Visible = False
        Me.GridIncidences.Columns.Add(GridColumn)

        'Command buttons
        GridColumnCommand = New GridViewCommandColumn()
        GridColumnCommand.ButtonRenderMode = GridCommandButtonRenderMode.Image
        GridColumnCommand.ShowDeleteButton = False
        GridColumnCommand.ShowEditButton = True
        GridColumnCommand.Caption = " "
        GridColumnCommand.VisibleIndex = VisibleIndex
        GridColumnCommand.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCommand.Width = 10
        VisibleIndex = VisibleIndex + 1

        Me.GridIncidences.Columns.Add(GridColumnCommand)

    End Sub

    Private Function GetJustCausesDataAll(Optional ByVal bolReload As Boolean = False) As DataView

        If bolReload Or Session("ReviewBusinessCenters_JustCausesDataAll") Is Nothing Then
            Dim dv As DataView = Nothing
            Dim returnTb As New DataTable
            returnTb.Columns.Add(New DataColumn("ID", GetType(Short)))
            returnTb.Columns.Add(New DataColumn("Name", GetType(String)))
            Dim oNewRow As DataRow = returnTb.NewRow

            Dim tb As DataTable = CausesServiceMethods.GetCauses(Me.Page, "", False)
            If tb IsNot Nothing Then
                For Each oRow As DataRow In tb.Rows
                    oNewRow = returnTb.NewRow
                    With oNewRow
                        .Item("ID") = oRow("ID")
                        .Item("Name") = oRow("Name")
                    End With
                    returnTb.Rows.Add(oNewRow)
                Next

                Session("ReviewBusinessCenters_JustCausesDataAll") = returnTb

                dv = New DataView(returnTb)
                dv.Sort = "Name ASC"
            End If

            Return dv
        Else
            Dim dtReturn As DataTable = Session("ReviewBusinessCenters_JustCausesDataAll")
            Dim dv As DataView = New DataView(dtReturn)
            dv.Sort = "Name ASC"
            Return dv
        End If
    End Function

    Private Sub GridIncidences_DataBinding(sender As Object, e As EventArgs) Handles GridIncidences.DataBinding
        Dim oCombo As GridViewDataComboBoxColumn = GridIncidences.Columns("IDCause")
        oCombo.PropertiesComboBox.DataSource = Me.GetJustCausesDataAll()
        oCombo.PropertiesComboBox.TextField = "Name"
        oCombo.PropertiesComboBox.ValueField = "ID"
        oCombo.PropertiesComboBox.ValueType = GetType(Short)

        oCombo = GridIncidences.Columns("IDCenter")
        oCombo.PropertiesComboBox.DataSource = Me.GetCentersDataAll()
        oCombo.PropertiesComboBox.TextField = "Name"
        oCombo.PropertiesComboBox.ValueField = "ID"
        oCombo.PropertiesComboBox.ValueType = GetType(Short)
    End Sub

    Protected Sub CallbackSession_Callback(ByVal source As Object, ByVal e As DevExpress.Web.CallbackEventArgs) Handles CallbackSession.Callback
        e.Result = String.Empty

        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Select Case strParameter.Trim.ToUpperInvariant
            Case "GETINFOSELECTED"
                e.Result = GetInfoSelected()
            Case "REFRESHGRID"

            Case "UPDATECAUSETEXT"
                e.Result = CausesServiceMethods.GetCauseByID(Me.Page, roTypes.Any2Integer(hdnCausesSelected.Value), False).Name

            Case "SAVEDATA"
                iCurrentTask = Me.AssignCenters()
                If iCurrentTask > 0 Then
                    CallbackSession.JSProperties.Add("cpAction", "SAVEDATA")
                    CallbackSession.JSProperties.Add("cpActionResult", "OK")
                Else
                    If iCurrentTask = -2 Then
                        CallbackSession.JSProperties.Add("cpAction", "ERROR")
                        CallbackSession.JSProperties.Add("cpActionResult", "KO")
                        CallbackSession.JSProperties.Add("cpActionMessageResult", "Error.InFreezingDate")
                    Else
                        CallbackSession.JSProperties.Add("cpAction", "ERROR")
                        CallbackSession.JSProperties.Add("cpActionResult", "KO")
                        CallbackSession.JSProperties.Add("cpActionMessageResult", "Error.ValidationUnsuccessful")
                    End If

                End If
            Case "RELOADBC"
                frmBusinessCenterSelector.SetPassport(WLHelperWeb.CurrentPassportID, True, False, False, False)
            Case "CHECKPROGRESS"
                If iCurrentTask >= 0 Then
                    Dim oTask As roLiveTask = API.LiveTasksServiceMethods.GetLiveTaskStatus(Me.Page, iCurrentTask)
                    If oTask IsNot Nothing Then
                        Select Case oTask.Status
                            Case 0, 1
                                CallbackSession.JSProperties.Add("cpAction", "CHECKPROGRESS")
                                CallbackSession.JSProperties.Add("cpActionResult", "")
                            Case 2
                                CallbackSession.JSProperties.Add("cpAction", "CHECKPROGRESS")
                                CallbackSession.JSProperties.Add("cpActionResult", "OK")
                            Case 3
                                CallbackSession.JSProperties.Add("cpAction", "ERROR")
                                CallbackSession.JSProperties.Add("cpActionResult", "KO")
                                CallbackSession.JSProperties.Add("cpActionMessageResult", "Error.TaskNotCreated")
                                iCurrentTask = -1
                                ErrorExists = True
                                ErrorDescription = oTask.ErrorCode
                        End Select
                    Else
                        iCurrentTask = -1
                        ErrorExists = True
                        ErrorDescription = roWsUserManagement.SessionObject.States.LiveTaskState.ErrorText
                        CallbackSession.JSProperties.Add("cpAction", "ERROR")
                    End If
                Else
                    iCurrentTask = -1
                    ErrorExists = True
                    ErrorDescription = Me.Language.Translate("Error.CouldNotRetrieveTask.Text", Me.DefaultScope)
                    CallbackSession.JSProperties.Add("cpAction", "ERROR")
                End If

        End Select
    End Sub

    Private Function GetCentersDataAll(Optional ByVal bolReload As Boolean = False, Optional ByVal bolCheckStatus As Boolean = False) As DataView

        If bolReload Or Session("ReviewBusinessCenters_CentersDataAll") Is Nothing Then
            Dim dv As DataView = Nothing
            Dim returnTb As New DataTable
            returnTb.Columns.Add(New DataColumn("ID", GetType(Short)))
            returnTb.Columns.Add(New DataColumn("Name", GetType(String)))
            Dim oNewRow As DataRow = returnTb.NewRow

            'Dim tb As DataTable = API.TasksServiceMethods.GetBusinessCenters(Me.Page, bolCheckStatus)
            Dim tb As DataTable = API.TasksServiceMethods.GetBusinessCenterByPassportDataTable(Me.Page, WLHelperWeb.CurrentPassport.ID, bolCheckStatus)

            If tb IsNot Nothing Then

                'Dim oRows() As DataRow = tb.Select("ID IN (" & Me.hdnBusinessCentersSelected.Value.ToString & ")")

                For Each oRow As DataRow In tb.Rows
                    oNewRow = returnTb.NewRow
                    With oNewRow
                        .Item("ID") = oRow("ID")
                        .Item("Name") = oRow("Name")
                    End With
                    returnTb.Rows.Add(oNewRow)
                Next

                oNewRow = returnTb.NewRow
                oNewRow.Item("ID") = 0
                oNewRow.Item("Name") = Me.Language.Translate("CostCenter.DefaultCostCenter", Me.DefaultScope)
                returnTb.Rows.Add(oNewRow)

                Session("ReviewBusinessCenters_CentersDataAll") = returnTb

                dv = New DataView(returnTb)
                dv.Sort = "Name ASC"
            End If

            Return dv
        Else
            Dim dtReturn As DataTable = Session("ReviewBusinessCenters_CentersDataAll")
            Dim dv As DataView = New DataView(dtReturn)
            dv.Sort = "Name ASC"
            Return dv
        End If
    End Function

    Private Function GetCentersDataAllDetail(Optional ByVal bolReload As Boolean = False, Optional ByVal bolCheckStatus As Boolean = False) As DataView

        If bolReload Or Session("ReviewBusinessCenters_CentersDataAllDetail") Is Nothing Then
            Dim dv As DataView = Nothing
            Dim returnTb As New DataTable
            returnTb.Columns.Add(New DataColumn("ID", GetType(Short)))
            returnTb.Columns.Add(New DataColumn("Name", GetType(String)))
            Dim oNewRow As DataRow = returnTb.NewRow

            Dim tb As DataTable = API.TasksServiceMethods.GetBusinessCenterByPassportDataTable(Me.Page, WLHelperWeb.CurrentPassport.ID, bolCheckStatus)

            If tb IsNot Nothing Then
                For Each oRow As DataRow In tb.Rows
                    oNewRow = returnTb.NewRow
                    With oNewRow
                        .Item("ID") = oRow("ID")
                        .Item("Name") = oRow("Name")
                    End With
                    returnTb.Rows.Add(oNewRow)
                Next

                oNewRow = returnTb.NewRow
                oNewRow.Item("ID") = 0
                oNewRow.Item("Name") = Me.Language.Translate("CostCenter.DefaultCostCenter", Me.DefaultScope)
                returnTb.Rows.Add(oNewRow)

                Session("ReviewBusinessCenters_CentersDataAllDetail") = returnTb

                dv = New DataView(returnTb)
                dv.Sort = "Name ASC"
            End If

            Return dv
        Else
            Dim dtReturn As DataTable = Session("ReviewBusinessCenters_CentersDataAllDetail")
            Dim dv As DataView = New DataView(dtReturn)
            dv.Sort = "Name ASC"
            Return dv
        End If
    End Function

    Private Function GetCentersDataCombo(Optional ByVal bolReload As Boolean = False, Optional ByVal bolCheckStatus As Boolean = False) As DataView

        If bolReload Or Session("ReviewBusinessCenters_CentersDataCombo") Is Nothing Then
            Dim dv As DataView = Nothing
            Dim returnTb As New DataTable
            returnTb.Columns.Add(New DataColumn("ID", GetType(Short)))
            returnTb.Columns.Add(New DataColumn("Name", GetType(String)))
            Dim oNewRow As DataRow = returnTb.NewRow

            Dim tb As DataTable = API.TasksServiceMethods.GetBusinessCenterByPassportDataTable(Me.Page, WLHelperWeb.CurrentPassport.ID, bolCheckStatus)

            If tb IsNot Nothing Then
                For Each oRow As DataRow In tb.Rows
                    oNewRow = returnTb.NewRow
                    With oNewRow
                        .Item("ID") = oRow("ID")
                        .Item("Name") = oRow("Name")
                    End With
                    returnTb.Rows.Add(oNewRow)
                Next

                oNewRow = returnTb.NewRow
                oNewRow.Item("ID") = 0
                oNewRow.Item("Name") = Me.Language.Translate("CostCenter.DefaultCostCenter", Me.DefaultScope)
                returnTb.Rows.Add(oNewRow)

                Session("ReviewBusinessCenters_CentersDataCombo") = returnTb

                dv = New DataView(returnTb)
                dv.Sort = "Name ASC"
            End If

            Return dv
        Else
            Dim dtReturn As DataTable = Session("ReviewBusinessCenters_CentersDataCombo")
            Dim dv As DataView = New DataView(dtReturn)
            dv.Sort = "Name ASC"
            Return dv
        End If
    End Function

    Private Sub LoadCombos()

        'Me.txtDateInf.Value = DateTime.Today.AddDays(-6)
        'Me.txtDateSup.Value = DateTime.Today
    End Sub

#Region "SELECTOR DE EMPLEADOS Y INCIDENCIAS"

    Private Function GetInfoSelected() As String
        Dim strRet As String = "0; "

        Try
            If Not String.IsNullOrEmpty(Me.hdnEmployees.Value) Then

                Dim DateInf As DateTime = txtDateInf.Value
                Dim DateSup As DateTime = txtDateSup.Value

                If DateInf > DateSup Then
                    Dim aux As DateTime = DateSup
                    DateSup = DateInf
                    DateInf = aux
                End If

                Dim Selection() As String = Me.hdnEmployees.Value.Trim.Split(",")
                If Selection.Length = 1 Then

                    If Selection(0).Substring(0, 1) = "A" Then 'Grupo
                        'obtener el nombre del grupo
                        Dim oGroup As roGroup = API.EmployeeGroupsServiceMethods.GetGroup(Me.Page, Selection(0).Substring(1), False)
                        If Not oGroup Is Nothing Then
                            'Dim NumEmployees As Integer = GetNumOfEmployees()
                            Dim NumEmployees As Integer = HelperWeb.GetNumOfEmployeesAnalitics(Me.hdnEmployees.Value, Me.hdnFilter.Value, Me.hdnFilterUser.Value, "Calendar.JustifyIncidences", DateInf, DateSup)
                            strRet = NumEmployees & ";" & oGroup.Name
                        End If

                    ElseIf Selection(0).Substring(0, 1) = "B" Then 'Empleado
                        'obtener el nombre del empleado
                        Dim oEmployee As roEmployee = API.EmployeeServiceMethods.GetEmployee(Me.Page, Selection(0).Substring(1), False)
                        If Not oEmployee Is Nothing Then
                            strRet = "1;" & oEmployee.Name
                        End If
                    End If
                Else
                    'Dim NumEmployees As Integer = GetNumOfEmployees()
                    Dim NumEmployees As Integer = HelperWeb.GetNumOfEmployeesAnalitics(Me.hdnEmployees.Value, Me.hdnFilter.Value, Me.hdnFilterUser.Value, "Calendar.JustifyIncidences", DateInf, DateSup)
                    strRet = NumEmployees & ";" & NumEmployees.ToString & " " & Me.Language.Translate("GridAbsences.EmployeesSelected", DefaultScope)
                End If
            End If
        Catch
            strRet = "0; "
        End Try

        Return strRet

    End Function

#End Region

    Protected Sub btnExportToXls_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnExportToXls.Click
        Me.ASPxGridViewExporter1.GridViewID = Me.GridIncidences.ID
        Me.ASPxGridViewExporter1.WriteXlsxToResponse(True)
    End Sub

    Private Function FindWeekMonday(ByVal oDate As Date) As Date
        Dim rest As Integer = 0
        If oDate.DayOfWeek = 0 Then
            rest = 6
        Else
            rest = oDate.DayOfWeek - 1
        End If
        Return oDate.AddDays(rest * -1)
    End Function

    Protected Sub ASPxGridViewExporter1_RenderBrick(sender As Object, e As ASPxGridViewExportRenderingEventArgs) Handles ASPxGridViewExporter1.RenderBrick
    End Sub

    Private Sub GridIncidences_RowUpdating(sender As Object, e As ASPxDataUpdatingEventArgs) Handles GridIncidences.RowUpdating
        Dim tb As DataTable = Me.IncidencesDataTable()
        Dim dr As DataRow = tb.Rows.Find(e.Keys(GridIncidences.KeyFieldName))
        Dim grid As ASPxGridView = CType(sender, ASPxGridView)

        Dim PruebaMinutos As Integer = 0
        Dim RowTime As Double = 0
        Dim GridTime As Double = 0
        Dim GridCenter As Integer = 0
        Dim RowCenter As Integer = 0
        Dim bolModifytime As Boolean = False
        Dim bolModifyCenter As Boolean = False

        'obtener la hora guardada
        If Not dr("ValueHoraEditable") Is System.DBNull.Value Then
            PruebaMinutos = roConversions.ConvertTimeToMinutes(dr("ValueHoraEditable"))
            RowTime = roConversions.ConvertTimeToHours(dr("ValueHoraEditable"))
        End If

        RowCenter = roTypes.Any2Integer(dr.Item("IDCenter"))

        Dim enumerator As IDictionaryEnumerator = e.NewValues.GetEnumerator()
        enumerator.Reset()
        While enumerator.MoveNext()
            If enumerator.Key.ToString() = "ValueHoraEditable" Then
                GridTime = roConversions.ConvertTimeToHours(enumerator.Value)
                bolModifytime = True
            End If
            If enumerator.Key.ToString() = "IDCenter" Then
                GridCenter = enumerator.Value
                bolModifyCenter = True
            End If

            dr(enumerator.Key.ToString()) = enumerator.Value
        End While

        'Comprobar si es direct cause
        Dim IsDirectCause As Boolean = (roTypes.Any2Integer(dr("IDRelatedIncidence")) <> 0)

        ' revismos si ha cambiado el centro
        If GridCenter <> RowCenter AndAlso bolModifyCenter Then
            dr.Item("ManualCenter") = 1
            dr.Item("DefaultCenter") = 1
            ' Revisamos si es el centro por defecto
            Dim intIDCenterDefault = API.TasksServiceMethods.GetEmployeeDefaultBusinessCenter(Me, dr("IDEmployee"), CDate(dr("Date")))
            If dr.Item("IDCenter") <> intIDCenterDefault Then
                dr.Item("DefaultCenter") = 0
            End If
        End If

        If bolModifytime Then

            If RowTime <> 0 AndAlso RowTime > GridTime Then
                'la hora anterior es mas grande que la nueva
                Dim oNewRow As DataRow = tb.NewRow

                oNewRow.Item("EmployeesName") = dr("EmployeesName")
                oNewRow.Item("IDEmployee") = dr("IDEmployee")
                oNewRow.Item("IDContract") = dr("IDContract")
                oNewRow.Item("Date") = dr("Date")
                oNewRow.Item("IDRelatedIncidence") = dr("IDRelatedIncidence")
                oNewRow.Item("IDCause") = dr("IDCause")
                oNewRow.Item("AccrualsRules") = dr("AccrualsRules")
                oNewRow.Item("DailyRule") = dr("DailyRule")
                oNewRow.Item("AccruedRule") = dr("AccruedRule")
                oNewRow.Item("Manual") = 1
                oNewRow.Item("PunchesLst") = dr("PunchesLst")
                oNewRow.Item("IDCenterOld") = 0

                Try
                    If WLHelperWeb.CurrentPassport.IDUser.HasValue Then oNewRow.Item("CauseUser") = WLHelperWeb.CurrentPassport.IDUser
                Catch ex As Exception
                    Throw New Exception(Me.Language.Translate("AccessDenied.Message", Me.DefaultScope))
                End Try

                oNewRow.Item("CauseUserType") = 0
                oNewRow.Item("IsNotReliable") = 0
                oNewRow.Item("Value") = RowTime - GridTime
                oNewRow.Item("TimeZoneName") = dr("TimeZoneName")
                oNewRow.Item("BeginTime") = dr("BeginTime")
                oNewRow.Item("BeginTimeOrder") = dr("BeginTimeOrder")
                oNewRow.Item("EndTime") = dr("EndTime")
                oNewRow.Item("IncidenceValue") = dr("IncidenceValue")
                oNewRow.Item("IDType") = dr("IDType")

                'añadidos
                oNewRow.Item("ValueHora") = roConversions.ConvertHoursToTime(RowTime - GridTime)
                oNewRow.Item("ValueHoraEditable") = oNewRow.Item("ValueHora")
                oNewRow.Item("TextoIncidencia") = IIf(roTypes.Any2String(dr("IDType")) <> String.Empty, Me.Language.Keyword("Incidence." & roTypes.Any2String(dr("IDType"))), "")

                If GridCenter <> RowCenter AndAlso bolModifyCenter Then
                    oNewRow.Item("IDCenter") = GridCenter
                Else
                    oNewRow.Item("IDCenter") = dr("IDCenter")
                End If

                oNewRow.Item("DefaultCenter") = dr("DefaultCenter")
                oNewRow.Item("ManualCenter") = dr("ManualCenter")

                oNewRow.Item("Clave") = Guid.NewGuid()

                'corregir la fila modificada
                dr.Item("Value") = GridTime
                dr.Item("Manual") = 1
                dr.Item("ValueHora") = roConversions.ConvertHoursToTime(dr.Item("Value"))
                dr.Item("ValueHoraEditable") = dr.Item("ValueHora")

                tb.Rows.Add(oNewRow)
            Else
                If RowTime < GridTime Then
                    ' Si el tiempo supera el valor inicial
                    ' no dejamos modificarlo
                    dr.Item("Value") = RowTime
                    dr.Item("ValueHora") = roConversions.ConvertHoursToTime(dr.Item("Value"))
                    dr.Item("ValueHoraEditable") = dr.Item("ValueHora")

                    Throw New Exception(Me.Language.Translate("ModifyTimeDenied.Message", Me.DefaultScope))
                Else
                    'corregir la fila modificada
                    dr.Item("Value") = GridTime
                    dr.Item("Manual") = 1
                    dr.Item("ValueHora") = roConversions.ConvertHoursToTime(dr.Item("Value"))
                    dr.Item("ValueHoraEditable") = dr.Item("ValueHora")
                End If
            End If
        End If

        Me.IncidencesDataTable = tb
        e.Cancel = True
        grid.CancelEdit()
    End Sub

    Private Sub GridIncidences_StartRowEditing(sender As Object, e As ASPxStartRowEditingEventArgs) Handles GridIncidences.StartRowEditing
        AssignTemplatesInReadOnlyColumns()
    End Sub

    Private Sub AssignTemplatesInReadOnlyColumns()
        Dim template As Scheduler_ReadOnlyTemplate = New Scheduler_ReadOnlyTemplate()
        CType(GridIncidences.Columns("TextoIncidencia"), GridViewDataColumn).EditItemTemplate = template
        CType(GridIncidences.Columns("TimeZoneName"), GridViewDataColumn).EditItemTemplate = template
        CType(GridIncidences.Columns("BeginTime"), GridViewDataColumn).EditItemTemplate = template
        CType(GridIncidences.Columns("EndTime"), GridViewDataColumn).EditItemTemplate = template
        CType(GridIncidences.Columns("ValueHora"), GridViewDataColumn).EditItemTemplate = template
        CType(GridIncidences.Columns("EmployeesName"), GridViewDataColumn).EditItemTemplate = template
        CType(GridIncidences.Columns("Date"), GridViewDataColumn).EditItemTemplate = template
        CType(GridIncidences.Columns("IDContract"), GridViewDataColumn).EditItemTemplate = template
        CType(GridIncidences.Columns("PunchesLst"), GridViewDataColumn).EditItemTemplate = template
        CType(GridIncidences.Columns("IDCause"), GridViewDataColumn).EditItemTemplate = template
    End Sub

    Protected Sub GridIncidences_CellEditorInitialize(sender As Object, e As ASPxGridViewEditorEventArgs) Handles GridIncidences.CellEditorInitialize
        If e.Column.FieldName = "IDCause" Then
            Dim tb As DataTable = Me.IncidencesDataTable()
            If e.VisibleIndex >= 0 Then
                Dim dRow As DataRow = tb.Rows(e.VisibleIndex)

                Dim cmb As ASPxComboBox = CType(e.Editor, ASPxComboBox)
                cmb.Font.Size = 8
                cmb.Focus()

                Dim dtVisibleCauses As DataTable = GetJustCausesData().Table

                Dim removeItems As New Generic.List(Of ListEditItem)
                For Each oItem As ListEditItem In cmb.Items

                    Dim oRows() As DataRow = dtVisibleCauses.Select("ID = " & oItem.Value)

                    If oRows.Length = 0 Then
                        If roTypes.Any2Integer(dRow("IDCause")) <> oItem.Value Then
                            removeItems.Add(oItem)
                        End If
                    End If
                Next

                For Each oDeleteItem As ListEditItem In removeItems
                    cmb.Items.Remove(oDeleteItem)
                Next
            End If

        ElseIf e.Column.FieldName = "ValueHoraEditable" Then
            Dim txt As ASPxTextBox
            Dim grid As ASPxGridView = CType(sender, ASPxGridView)

            txt = CType(e.Editor, ASPxTextBox)
            txt.Width = 36
            txt.Font.Size = 8

            Dim num As String = roTypes.Any2String(GridIncidences.GetRowValues(e.VisibleIndex, "BeginTime"))

            If num.Length > 0 Then
                txt.MaskSettings.Mask = "HH:mm"
            Else
                txt.MaskSettings.Mask = "<-9999..9999>:<00..59>"
                'si el numero de horas es cero hacer una chapucilla porque el control no lo pinta bien
                Try
                    If txt.Text.Split(":")(0) = "00" Then
                        txt.Text = txt.Text.Substring(1)
                    End If
                Catch ex As Exception
                End Try

            End If

        ElseIf e.Column.FieldName = "IDCenter" Then

            'Dim tb As DataTable = Me.IncidencesDataTable()
            'If e.VisibleIndex >= 0 Then
            '    'Dim dRow As DataRow = tb.Rows(e.VisibleIndex)
            '    Dim dRow As DataRow = tb.Rows.Find(e.KeyValue)

            '    Dim cmb As ASPxComboBox = CType(e.Editor, ASPxComboBox)
            '    cmb.Font.Size = 8
            '    cmb.Focus()

            '    If Not dRow Is Nothing Then
            '        Dim dtVisibleCenters As DataTable = Me.GetCentersDataAllDetail(, True).Table

            '        Dim removeItems As New Generic.List(Of ListEditItem)
            '        For Each oItem As ListEditItem In cmb.Items

            '            Dim oRows() As DataRow = dtVisibleCenters.Select("ID = " & oItem.Value)

            '            If oRows.Length = 0 Then
            '                If roTypes.Any2Integer(dRow("IDCenter")) <> oItem.Value Then
            '                    removeItems.Add(oItem)
            '                End If
            '            End If
            '        Next

            '        For Each oDeleteItem As ListEditItem In removeItems
            '            cmb.Items.Remove(oDeleteItem)
            '        Next
            '    End If
            'Else
            '    Try
            '        Dim cmb As ASPxComboBox = CType(e.Editor, ASPxComboBox)
            '        cmb.Font.Size = 8
            '        cmb.Focus()

            '        Dim dtVisibleCenters As DataTable = Me.GetCentersDataAllDetail(, True).Table

            '        Dim removeItems As New Generic.List(Of ListEditItem)
            '        For Each oItem As ListEditItem In cmb.Items

            '            Dim oRows() As DataRow = dtVisibleCenters.Select("ID = " & oItem.Value)

            '            If oRows.Length = 0 Then
            '                removeItems.Add(oItem)
            '            End If
            '        Next

            '        For Each oDeleteItem As ListEditItem In removeItems
            '            cmb.Items.Remove(oDeleteItem)
            '        Next

            '    Catch ex As Exception

            '    End Try
            'End If

        End If
    End Sub

    Private Function IncidencesHasChanges(Optional ByVal tbIncidences As DataTable = Nothing) As Boolean
        Dim bolRet As Boolean = False
        If tbIncidences Is Nothing Then
            tbIncidences = Me.IncidencesDataTable
        End If
        Dim tbChanges As DataTable = tbIncidences.GetChanges()
        If Not tbChanges Is Nothing AndAlso tbChanges.Rows.Count > 0 Then
            bolRet = True
        End If
        Return bolRet
    End Function

    Private Property iCurrentTask As Integer
        Get
            Dim val As Object = HttpContext.Current.Session("Review_iCurrentTask")
            If val IsNot Nothing Then
                Return roTypes.Any2Integer(val)
            Else
                HttpContext.Current.Session("Review_iCurrentTask") = -1
                Return -1
            End If
        End Get
        Set(value As Integer)
            HttpContext.Current.Session("Review_iCurrentTask") = value
        End Set
    End Property

    Private Sub FillCombos()
        cmbCostCenters.TextField = "Name"
        cmbCostCenters.ValueField = "ID"
        cmbCostCenters.ValueType = GetType(Short)
        cmbCostCenters.DataSource = Me.GetCentersDataCombo(True, True)
        cmbCostCenters.DataBind()
    End Sub

    Private Sub GridIncidences_CustomCallback(sender As Object, e As ASPxGridViewCustomCallbackEventArgs) Handles GridIncidences.CustomCallback

        If e.Parameters = "RELOAD" Then
            BindGridIncidences(True)

        ElseIf e.Parameters = "SAVE_RELOAD" Then
            'Dim grid As ASPxGridView = CType(sender, ASPxGridView)
            'Dim enumRet As SaveReturnTypes = IncidencesSaveDataCallback()
            'If enumRet = SaveReturnTypes.ChangesSaved Then
            '    BindGridIncidences(True)
            '    grid.JSProperties("cpReturnValue") = "OK"
            'Else
            '    grid.JSProperties("cpReturnValue") = ""
            'End If
        End If

    End Sub

    Private Function AssignCenters() As Integer

        Dim iTask As Integer = -1

        Dim tbIncidences As DataTable = Me.IncidencesDataTable

        If tbIncidences Is Nothing Then
            Return iTask
            Exit Function
        End If

        If ckMoveAllTo.Checked Then
            Dim oListIDs As Generic.List(Of Object) = GridIncidences.GetSelectedFieldValues("Clave")

            For Each oGuid As Object In oListIDs
                Dim oRows() As DataRow = tbIncidences.Select("Clave='" & roTypes.Any2String(oGuid) & "'")
                If oRows.Length > 0 Then
                    If cmbCostCenters.SelectedItem IsNot Nothing Then

                        ' revismos si ha cambiado el centro
                        If CShort(cmbCostCenters.SelectedItem.Value) <> CShort(oRows(0)("IDCenter")) Then
                            oRows(0)("IDCenter") = cmbCostCenters.SelectedItem.Value
                            oRows(0)("ManualCenter") = 1
                            oRows(0)("DefaultCenter") = 1
                            ' Revisamos si es el centro por defecto
                            Dim intIDCenterDefault = API.TasksServiceMethods.GetEmployeeDefaultBusinessCenter(Me, oRows(0)("IDEmployee"), CDate(oRows(0)("Date")))
                            If oRows(0)("IDCenter") <> intIDCenterDefault Then
                                oRows(0)("DefaultCenter") = 0
                            End If
                        End If

                    End If
                End If
            Next

        End If

        ' Validamos si alguno de los empleados modificados tiene fecha de cierre dentro del periodo seleccionado,
        ' en caso afirmativo no seguimos
        Try
            Dim tbChanges As DataTable = tbIncidences.GetChanges()
            Dim lstRows As String = ""
            If Not tbChanges Is Nothing AndAlso tbChanges.Rows.Count > 0 Then
                For Each oRowNew As DataRow In tbChanges.Rows
                    If oRowNew.RowState = DataRowState.Added Or oRowNew.RowState = DataRowState.Modified Then
                        If CDate(oRowNew.Item("Date")) <= API.EmployeeServiceMethods.GetEmployeeLockDatetoApply(Me, roTypes.Any2Integer(oRowNew.Item("IDEmployee")), False, False) Then
                            iTask = -2
                            Exit For
                        End If
                    End If
                Next
            End If

            If iTask = -2 Then
                Return iTask
                Exit Function
            End If
        Catch ex As Exception
        End Try

        iTask = API.LiveTasksServiceMethods.AssignCentersInBackground(Me, tbIncidences, Me.txtDateInf.Value, Me.txtDateSup.Value, True)
        If iTask > 0 Then
            Try
                Dim lstAuditParameterNames As New List(Of String)
                Dim lstAuditParameterValues As New List(Of String)

                lstAuditParameterNames.Add("{iTask}")
                lstAuditParameterValues.Add(iTask)

                lstAuditParameterNames.Add("{DateIni}")
                lstAuditParameterValues.Add(CDate(Me.txtDateInf.Value).ToShortDateString())

                lstAuditParameterNames.Add("{DateEnd}")
                lstAuditParameterValues.Add(CDate(Me.txtDateSup.Value).ToShortDateString())

                API.AuditServiceMethods.Audit(Robotics.VTBase.Audit.Action.aUpdate, Robotics.VTBase.Audit.ObjectType.tAssignCenters, "", lstAuditParameterNames, lstAuditParameterValues, Me.Page)
            Catch ex As Exception
            End Try
        End If
        Return iTask
    End Function

    Public Class Scheduler_ReadOnlyTemplate
        Implements ITemplate

        Public Sub InstantiateIn1(ByVal container As System.Web.UI.Control) Implements System.Web.UI.ITemplate.InstantiateIn
            Dim Auxcontainer As GridViewEditItemTemplateContainer = CType(container, GridViewEditItemTemplateContainer)
            Dim lbl As New ASPxLabel()
            lbl.ID = "lbl"
            Auxcontainer.Controls.Add(lbl)
            lbl.Text = IIf(Auxcontainer.Text = "&nbsp;", "", HttpContext.Current.Server.HtmlDecode(Auxcontainer.Text))
            lbl.Width = 10
            lbl.Font.Size = 8
            lbl.Wrap = DevExpress.Utils.DefaultBoolean.False
        End Sub

    End Class

End Class