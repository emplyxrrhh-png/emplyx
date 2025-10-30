Imports DevExpress.Web
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTSelectorManager
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions.VTLiveTasks
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class Forms_IncidencesWizard
    Inherits PageBase

    Private Const FeatureAlias As String = "Calendar.JustifyIncidences"
    Private Const IMAGE_INCIDENCE As String = "~/Reports/Images/Information_16.png"

#Region "Declarations"

    Private intActivePage As Integer

    Private intIDEmployee As Integer

    Private oPermission As Permission
    Private oCurrentPermission As Permission

#End Region

#Region "Properties"

    Private Enum SaveReturnTypes
        Undefined
        ChangesSaved
        NoChangesSaved
        IsError
    End Enum

    Private Property iCurrentTask As Integer
        Get
            Dim val As Object = HttpContext.Current.Session("Inc_iCurrentTask")
            If val IsNot Nothing Then
                Return roTypes.Any2Integer(val)
            Else
                HttpContext.Current.Session("Inc_iCurrentTask") = -1
                Return -1
            End If
        End Get
        Set(value As Integer)
            HttpContext.Current.Session("Inc_iCurrentTask") = value
        End Set
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

    Public Property FeatureCostCenter() As Boolean
        Get
            Return ViewState("IncidencesWizard_FeatureCostCenter")
        End Get
        Set(ByVal value As Boolean)
            ViewState("IncidencesWizard_FeatureCostCenter") = value
        End Set
    End Property

    Public Property IncidencesWizardCostCenterAvailable() As Boolean
        Get
            Return ViewState("IncidencesWizard_IncidencesWizardCostCenterAvailable")
        End Get
        Set(ByVal value As Boolean)
            ViewState("IncidencesWizard_IncidencesWizardCostCenterAvailable") = value
        End Set
    End Property

#End Region

#Region "Events"

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("BrowserDetect", "~/Base/Scripts/BrowserDetect.js", , True)
        Me.InsertExtraJavascript("Cookies", "~/Base/Scripts/Cookies.js", , True)
        Me.InsertExtraJavascript("Generic", "~/Base/Scripts/Generic.js", , True)
        Me.InsertExtraJavascript("roTreeState", "~/Base/Scripts/rocontrols/roTrees/roTreeState.js", , True)
        Me.InsertExtraJavascript("roOptionPanelClient", "~/Base/Scripts/roOptionPanelClient.js")
        Me.InsertExtraJavascript("roOptPanelClientGroup", "~/Base/Scripts/roOptPanelClientGroup.js")
        Me.InsertExtraJavascript("roIncidencesWizard", "~/Scheduler/Scripts/wizards.js", , True)

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertCssIncludes(Me.Page)

        Server.ScriptTimeout = 1000

        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)

        If Me.oPermission >= Permission.Write Then

            If Not Me.IsPostBack AndAlso Not IsCallback Then

                CreateColumnsIncidences()
                CreateColumnsCenters()
                CreateColumnsCauses()

                Me.CostCenterPermission = Me.GetFeaturePermission("BusinessCenters.Punches")
                FeatureCostCenter = HelperSession.GetFeatureIsInstalledFromApplication("Feature\CostControl")

                Dim oParameter As String = HelperSession.AdvancedParametersCache("IncidencesWizardCostCenterAvailable")
                Me.IncidencesWizardCostCenterAvailable = IIf(oParameter = "1", True, False)
                If Not FeatureCostCenter Then
                    Me.CostCenterPermission = Permission.None
                End If

                If Not IncidencesWizardCostCenterAvailable Then
                    Me.CostCenterPermission = Permission.None
                End If

                FillCombos()

                Me.FreezingDatePage = API.ConnectorServiceMethods.GetFirstDate(Me.Page)

                Me.btClose.Visible = Not Me.IsPopup

                Me.lblStep2Title.Text = Me.hdnStepTitle.Text & Me.lblStep2Title.Text
                Me.lblStep3Title.Text = Me.hdnStepTitle.Text & Me.lblStep3Title.Text
                Me.lblStep4Title.Text = Me.hdnStepTitle.Text & Me.lblStep4Title.Text
                Me.lblStep5Title.Text = Me.hdnStepTitle.Text & Me.lblStep5Title.Text
                Me.lblStep6Title.Text = Me.hdnStepTitle.Text & Me.lblStep6Title.Text
                Me.lblStep7Title.Text = Me.hdnStepTitle.Text & Me.lblStep6Title.Text

                Me.txtBeginDate.Date = Now.Date
                Me.txtEndDate.Date = Now.Date

                Me.cmbCauses.SelectedItem = Me.cmbCauses.Items.FindByValue(CShort(0))

                Me.intActivePage = 0

                'Inicializamos el selector de empleados
                HelperWeb.roSelector_Initialize("objContainerTreeV3_treeEmpIncidencesWizard")
                HelperWeb.roSelector_Initialize("objContainerTreeV3_treeEmpIncidencesWizardGrid")

                Me.ifEmployeeSelector.Attributes("src") = Me.ResolveUrl("~/Base/BlankPage.aspx")
                Me.ifEmployeeSelector.Disabled = True

                With cmbIncPlusAccSup
                    .Items.Clear()
                    .Items.Add(Me.Language.Translate("IncPlusAcc.ddlIncPlusAccSup.ex", Me.DefaultScope), 0)
                    .Items.Add(Me.Language.Translate("IncPlusAcc.ddlIncPlusAccSup.no", Me.DefaultScope), 1)
                    .SelectedIndex = 0
                End With
            Else

                If Me.divStep0.Style("display") <> "none" Then Me.intActivePage = 0
                If Me.divStep1.Style("display") <> "none" Then Me.intActivePage = 1
                If Me.DivStep2.Style("display") <> "none" Then Me.intActivePage = 2
                If Me.divStep3.Style("display") <> "none" Then Me.intActivePage = 3
                If Me.divStep4.Style("display") <> "none" Then Me.intActivePage = 4
                If Me.divStep5.Style("display") <> "none" Then Me.intActivePage = 5
                If Me.divStep6.Style("display") <> "none" Then Me.intActivePage = 6
                If Me.divStep7.Style("display") <> "none" Then Me.intActivePage = 7

            End If

            If Me.intActivePage = 3 Then
                InitList(Me.treeIncidences, Me.IncidencesData, "ID", "ID", IMAGE_INCIDENCE, "Incidence")
            End If

            BindGridIncidences(False)
            GetJustCausesData()
            GetJustCausesDataAll()

            BindGridCenters(False)
            BindGridCauses(False)
            GetCentersDataAll()
        Else
            WLHelperWeb.RedirectAccessDenied(True)
        End If

    End Sub

    Private Sub BindGridIncidences(Optional ByVal bolReload As Boolean = False)
        Me.GridIncidences.DataSource = Me.IncidencesDataView(bolReload)
        Me.GridIncidences.DataBind()
    End Sub

    Private Sub BindGridCenters(Optional ByVal bolReload As Boolean = False)
        Me.grdCenters.DataSource = Me.GetCentersDataAll(bolReload)
        Me.grdCenters.DataBind()
    End Sub

    Private Sub BindGridCauses(Optional ByVal bolReload As Boolean = False)
        Me.grdCauses.DataSource = Me.GetJustCausesData(bolReload)
        Me.grdCauses.DataBind()
    End Sub

    Private Sub FillCombos()
        cmbCauses.TextField = "Name"
        cmbCauses.ValueField = "ID"
        cmbCauses.ValueType = GetType(Short)
        cmbCauses.DataSource = Me.GetJustCausesData()
        cmbCauses.DataBind()
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

            Dim tbIncidences As DataTable = Session("IncidencesWizard_IncidencesDatatable")

            If bolReload Then

                Dim DateIni As DateTime = txtBeginDate.Date
                Dim DateFin As DateTime = txtEndDate.Date

                Dim strIncidences As String = Me.GetList(Me.treeIncidences).Split("~")(1)
                Dim lstEmployeesDest As New Generic.List(Of Integer)
                Dim lstGroups As New Generic.List(Of Integer)

                'obtener todos los empleados de los grupos seleccionados en el arbol v3
                roSelectorManager.ExtractIdsFromSelectionString(Me.hdnEmployeesSelected.Value, lstEmployeesDest, lstGroups)
                If lstGroups IsNot Nothing Then
                    Dim strFilter As String = Me.hdnFilter.Value
                    Dim strFilterUser As String = Me.hdnFilterUser.Value
                    Dim tmp As Generic.List(Of Integer) = API.EmployeeGroupsServiceMethods.GetEmployeeListFromGroupRecursive(Me, lstGroups.ToArray, FeatureAlias, "U",
                                                                                                                                               strFilter, strFilterUser)
                    lstEmployeesDest.AddRange(tmp)
                End If

                Dim arrEmployeesDest As String = "-1"
                For Each intID As Integer In lstEmployeesDest
                    arrEmployeesDest += "," & intID.ToString
                Next

                Dim oCentersList As Generic.List(Of Object) = grdCenters.GetSelectedFieldValues("ID")
                Dim strCentersFilter As String = "-1"
                If oCentersList.Count > 0 Then strCentersFilter = String.Join(",", oCentersList)

                Dim oCausesList As Generic.List(Of Object) = grdCauses.GetSelectedFieldValues("ID")

                Dim strCausesFilter As String = "-1"
                If oCausesList.Count > 0 Then strCausesFilter = String.Join(",", oCausesList)

                Dim strCausesValueFilter As String = String.Empty
                If chkApplyFilter.Checked Then
                    strCausesValueFilter = cmbIncPlusAccSup.SelectedItem.Value & "#" & roTypes.Any2String(roConversions.ConvertTimeToHours(txtIncPlusAccValue.Text)).Replace(",", ".")
                End If

                tbIncidences = API.EmployeeServiceMethods.GetMassIncidences(Me.Page, arrEmployeesDest, DateIni, DateFin, strIncidences, False, strCentersFilter, strCausesFilter, strCausesValueFilter)

                'añado columna de hora en formato hora desde formato double
                tbIncidences.Columns.Add(New DataColumn("ValueHora", GetType(String)))
                tbIncidences.Columns.Add(New DataColumn("ValueHoraEditable", GetType(String)))

                'añado columna de texto de incidencia
                tbIncidences.Columns.Add(New DataColumn("TextoIncidencia", GetType(String)))

                'añado columna de la clave
                tbIncidences.Columns.Add(New DataColumn("Clave", GetType(String)))

                ' añadimos columna idcauseold

                tbIncidences.Columns.Add(New DataColumn("IDCauseOld", GetType(Double)))

                Dim IncidenceText As String

                For Each r As DataRow In tbIncidences.Rows
                    r("ValueHora") = roConversions.ConvertHoursToTime(CDbl(r("Value")))
                    r("ValueHoraEditable") = roConversions.ConvertHoursToTime(CDbl(r("Value")))
                    r("IDCauseOld") = CDbl(r("IDCause"))
                    'r("TextoIncidencia") = IIf(roTypes.Any2String(r("IDType")) <> String.Empty, Me.Language.Keyword("Incidence." & roTypes.Any2String(r("IDType"))).ToUpper(), "")
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
                        'Dim oNewRow As DataRow = tbIncidences.NewRow
                        'oNewRow.Item("Clave") = Guid.NewGuid()
                        'oNewRow.Item("IDEmployee") = Me.IDEmployeePage
                        'oNewRow.Item("Date") = Me.DatePage
                        'oNewRow.Item("IDCause") = 0
                        'oNewRow.Item("Manual") = 0
                        'tbIncidences.Rows.Add(oNewRow)
                    End If
                    tbIncidences.AcceptChanges()

                End If

                Session("IncidencesWizard_IncidencesDatatable") = tbIncidences
            End If

            Return tbIncidences

        End Get
        Set(ByVal value As DataTable)
            Session("IncidencesWizard_IncidencesDatatable") = value
        End Set
    End Property

    Private Function GetJustCausesDataAll(Optional ByVal bolReload As Boolean = False) As DataView

        If bolReload Or Session("IncidencesWizard_JustCausesDataAll") Is Nothing Then
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

                Session("IncidencesWizard_JustCausesDataAll") = returnTb

                dv = New DataView(returnTb)
                dv.Sort = "Name ASC"
            End If

            Return dv
        Else
            Dim dtReturn As DataTable = Session("IncidencesWizard_JustCausesDataAll")
            Dim dv As DataView = New DataView(dtReturn)
            dv.Sort = "Name ASC"
            Return dv
        End If
    End Function

    Private Function GetJustCausesData(Optional ByVal bolReload As Boolean = False) As DataView

        If bolReload Or Session("IncidencesWizard_JustCausesData") Is Nothing Then
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

                Session("IncidencesWizard_JustCausesData") = returnTb

                dv = New DataView(returnTb)
                dv.Sort = "Name ASC"
            End If

            Return dv
        Else
            Dim dtReturn As DataTable = Session("IncidencesWizard_JustCausesData")
            Dim dv As DataView = New DataView(dtReturn)
            dv.Sort = "Name ASC"
            Return dv
        End If
    End Function

    Private Function GetList(ByVal oList As TreeView, Optional ByVal pChkAll As CheckBox = Nothing) As String

        Dim sValue As String = ""

        For Each oNode As TreeNode In oList.Nodes
            If oNode.Checked Then
                sValue = sValue & IIf(sValue = "", "", ",") & oNode.Value
            End If
        Next

        If sValue.Length = 0 Then sValue = "-1"
        If Not pChkAll Is Nothing Then
            sValue = IIf(pChkAll.Checked, "1", "0") & "~" & sValue
        Else
            sValue = "0~" & sValue
        End If

        Return sValue

    End Function

    Private Property IncidencesData() As DataTable
        Get
            Dim tb As DataTable = Session("IncidencesWizard_Incidences")

            If tb Is Nothing Then
                tb = API.IncidenceServiceMethods.GetIncidences(Me, "ID>0")
                Session("IncidencesWizard_Incidences") = tb
            End If

            Return tb

        End Get
        Set(ByVal value As DataTable)
            Session("IncidencesWizard_Incidences") = value
        End Set
    End Property

    Private Sub InitList(ByVal oList As TreeView, ByVal tbData As DataTable, ByVal strIDField As String, ByVal strNameField As String, ByVal strImage As String, Optional ByVal strToken As String = "")

        Dim oNode As TreeNode
        Dim strText As String
        oList.Nodes.Clear()
        For Each oRow As DataRow In tbData.Rows
            If strToken = "" Then
                strText = oRow(strNameField)
            Else
                strText = Me.Language.Keyword(strToken & "." & oRow(strNameField))
            End If
            oNode = New TreeNode(strText, oRow(strIDField), strImage)
            oList.Nodes.Add(oNode)
        Next

    End Sub

    Protected Sub btNext_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btNext.Click

        If Me.CheckPage(Me.intActivePage) Then

            Dim intOldPage As Integer = Me.intActivePage
            If Me.intActivePage = 5 Then
                If Me.CostCenterPermission = Permission.None Then
                    Me.intActivePage = 7
                Else
                    Me.intActivePage += 1
                End If
            Else
                Me.intActivePage += 1
            End If

            'Ens saltem la primera pantalla del filtre del arbre, ja que el porta incorporat
            If Me.intActivePage = 1 Then Me.intActivePage += 1

            Me.PageChange(intOldPage, Me.intActivePage)
        Else

        End If

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

        'Empleado
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridIncidences.Column.Employee", DefaultScope)
        GridColumn.FieldName = "EmployeesName"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Width = 35
        Me.GridIncidences.Columns.Add(GridColumn)

        'Contrato
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridIncidences.Column.Contract", DefaultScope)
        GridColumn.FieldName = "IDContract"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
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
        GridColumn.Width = 25
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
        GridColumn.Width = 30
        Me.GridIncidences.Columns.Add(GridColumn)

        'Zona Horaria
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridIncidences.Column.TimeZoneName", DefaultScope) '"Zona Horaria"
        GridColumn.FieldName = "TimeZoneName"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
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
        GridComboCommand.Width = 35
        GridComboCommand.ReadOnly = False
        GridComboCommand.PropertiesComboBox.IncrementalFilteringMode = IncrementalFilteringMode.Contains
        Me.GridIncidences.Columns.Add(GridComboCommand)

        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "IDCauseOld"
        GridColumn.FieldName = "IDCauseOld"
        GridColumn.ReadOnly = True
        GridColumn.Visible = False
        Me.GridIncidences.Columns.Add(GridColumn)

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

        'IDRelatedIncidence
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "IDRelatedIncidence"
        GridColumn.FieldName = "IDRelatedIncidence"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.Visible = False
        Me.GridIncidences.Columns.Add(GridColumn)

        'Centro
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridIncidences.Column.Center", DefaultScope) '"Centro"
        GridColumn.FieldName = "CenterName"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.Visible = IIf(Me.CostCenterPermission = Permission.None, False, True)
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Width = 25
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
        GridColumnCommand.Width = 15
        VisibleIndex = VisibleIndex + 1

        Me.GridIncidences.Columns.Add(GridColumnCommand)
    End Sub

    Private Sub GridIncidences_DataBinding(sender As Object, e As EventArgs) Handles GridIncidences.DataBinding
        Dim oCombo As GridViewDataComboBoxColumn = GridIncidences.Columns("IDCause")
        oCombo.PropertiesComboBox.DataSource = GetJustCausesDataAll()
        oCombo.PropertiesComboBox.TextField = "Name"
        oCombo.PropertiesComboBox.ValueField = "ID"
        oCombo.PropertiesComboBox.ValueType = GetType(Short)
    End Sub

    Private Sub CreateColumnsCenters()
        Dim GridColumn As GridViewDataTextColumn
        Dim GridColumnCommand As GridViewCommandColumn

        Dim VisibleIndex As Integer = 0

        Me.grdCenters.Columns.Clear()
        Me.grdCenters.KeyFieldName = "ID"
        Me.grdCenters.SettingsText.EmptyDataRow = " "

        'Clave
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "ID"
        GridColumn.FieldName = "ID"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.Visible = False
        GridColumn.Width = 40
        Me.grdCenters.Columns.Add(GridColumn)

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
        GridColumnCommand.Width = Unit.Percentage(5)
        VisibleIndex = VisibleIndex + 1
        Me.grdCenters.Columns.Add(GridColumnCommand)

        'Nombre
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("grdCenters.Column.Name", DefaultScope)
        GridColumn.FieldName = "Name"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Width = Unit.Percentage(95)
        Me.grdCenters.Columns.Add(GridColumn)

    End Sub

    Private Sub CreateColumnsCauses()
        Dim GridColumn As GridViewDataTextColumn
        Dim GridColumnCommand As GridViewCommandColumn

        Dim VisibleIndex As Integer = 0

        Me.grdCauses.Columns.Clear()
        Me.grdCauses.KeyFieldName = "ID"
        Me.grdCauses.SettingsText.EmptyDataRow = " "

        'Clave
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "ID"
        GridColumn.FieldName = "ID"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.Visible = False
        GridColumn.Width = 40
        Me.grdCauses.Columns.Add(GridColumn)

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
        GridColumnCommand.Width = Unit.Percentage(5)
        VisibleIndex = VisibleIndex + 1
        Me.grdCauses.Columns.Add(GridColumnCommand)

        'Nombre
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridCauses.Column.Name", DefaultScope)
        GridColumn.FieldName = "Name"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Width = Unit.Percentage(95)
        Me.grdCauses.Columns.Add(GridColumn)

    End Sub

    Protected Sub lblIncidencesCaption_Init(ByVal sender As Object, ByVal e As EventArgs)
        Dim txtLabel As ASPxLabel = CType(sender, ASPxLabel)
        txtLabel.CssClass = ""
        txtLabel.Text = Me.Language.Translate("lblIncidencesCaptionText", Me.DefaultScope)
    End Sub

    Protected Sub btPrev_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btPrev.Click

        Dim intOldPage As Integer = Me.intActivePage

        If Me.intActivePage = 7 Then
            If Me.CostCenterPermission = Permission.None Then
                Me.intActivePage = 5
            Else
                Me.intActivePage -= 1
            End If
        Else
            Me.intActivePage -= 1
        End If

        'Ens saltem la primera pantalla del filtre del arbre, ja que el porta incorporat
        If Me.intActivePage = 1 Then Me.intActivePage -= 1

        Me.PageChange(intOldPage, Me.intActivePage)

    End Sub

    Protected Sub btResume_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btResume.Click
        CloseWizard()
    End Sub

    Protected Sub PerformActionCallback_Callback(ByVal source As Object, ByVal e As DevExpress.Web.CallbackEventArgs) Handles PerformActionCallback.Callback
        e.Result = String.Empty

        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Select Case strParameter.Trim.ToUpperInvariant
            Case "VALIDATE"
                PerformActionCallback.JSProperties.Add("cpAction", "VALIDATE")
                PerformActionCallback.JSProperties.Add("cpResult", Me.CheckPage(Me.intActivePage))
            Case "PERFORM_ACTION"
                PerformActionCallback.JSProperties.Add("cpAction", "PERFORM_ACTION")
                iCurrentTask = Me.AssignCauses()
            Case "CHECKPROGRESS"
                If iCurrentTask >= 0 Then
                    Dim oTask As roLiveTask = API.LiveTasksServiceMethods.GetLiveTaskStatus(Me.Page, iCurrentTask)
                    If oTask IsNot Nothing Then
                        Select Case oTask.Status
                            Case 0, 1
                                PerformActionCallback.JSProperties.Add("cpAction", "CHECKPROGRESS")
                                PerformActionCallback.JSProperties.Add("cpActionResult", "")
                            Case 2
                                PerformActionCallback.JSProperties.Add("cpAction", "CHECKPROGRESS")
                                PerformActionCallback.JSProperties.Add("cpActionResult", "OK")
                            Case 3
                                PerformActionCallback.JSProperties.Add("cpAction", "ERROR")
                                PerformActionCallback.JSProperties.Add("cpActionResult", "KO")
                                iCurrentTask = -1
                                ErrorExists = True
                                ErrorDescription = oTask.ErrorCode
                        End Select
                    Else
                        iCurrentTask = -1
                        ErrorExists = True
                        ErrorDescription = roWsUserManagement.SessionObject.States.LiveTaskState.ErrorText
                        PerformActionCallback.JSProperties.Add("cpAction", "ERROR")
                    End If
                Else
                    iCurrentTask = -1
                    ErrorExists = True
                    ErrorDescription = Me.Language.Translate("Error.CouldNotRetrieveTask.Text", Me.DefaultScope)
                    PerformActionCallback.JSProperties.Add("cpAction", "ERROR")
                End If
        End Select

    End Sub

#End Region

#Region "Methods"

    Protected Sub btnExportToXls_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnExportToXls.Click

        Me.GridIncidences.CancelEdit()
        Me.ASPxGridViewExporter1.GridViewID = Me.GridIncidences.ID

        ' Diferenciamos si estamos en Cloud
        Dim strFileName As String = String.Empty
        Dim strPrefix = "grdIncidences_" & WLHelperWeb.CurrentPassport().ID.ToString
        Dim strExtension = "xlsx"

        Dim strPathTemplates As String = API.ConnectorServiceMethods.GetSetting(Me.Page, eKeys.DataLink)
        ' Obtener nombre del fichero temporal
        'strFileName = System.IO.Path.Combine(strPathTemplates, strPrefix & "." & strExtension)
        strFileName = System.IO.Path.GetTempFileName()

        If System.IO.File.Exists(strFileName) Then
            System.IO.File.Delete(strFileName)
        End If

        Dim ostream As System.IO.Stream = New System.IO.FileStream(strFileName, System.IO.FileMode.Create)
        Me.ASPxGridViewExporter1.WriteXlsx(ostream)

        ostream.Flush()
        ostream.Close()
        ostream = Nothing
        HttpContext.Current.Session("ExportFileName") = strFileName
        HttpContext.Current.Session("ExportRealFileName") = strPrefix & "." & strExtension
        HttpContext.Current.Session("ExportIsTMPFile") = True

        If System.IO.File.Exists(strFileName) Then
            Me.btnDownload.Visible = True
        End If

    End Sub

    Private Function CheckPage(ByVal intPage As Integer) As Boolean

        Dim bolRet As Boolean = True
        Dim strMsg As String = ""

        Select Case intPage
            Case 1

                If strMsg <> "" Then bolRet = False

            Case 2

                ' Hemos de tener algún empleado seleccionado
                If Me.hdnEmployeesSelected.Value = "" Then
                    strMsg = Me.Language.Translate("CheckPage.Page2.NoEmployeeSelected", Me.DefaultScope)
                Else
                    If Me.hdnEmployeesSelected.Value.Split(",").Length = 1 Then
                        If Me.hdnEmployeesSelected.Value.Substring(1) = Me.intIDEmployee Then
                            strMsg = Me.Language.Translate("CheckPage.Page2.IncorrectSelection", Me.DefaultScope)
                        End If
                    End If
                End If
                If strMsg <> "" Then bolRet = False
                Me.lblStep2Error.Text = strMsg

            Case 3

                If txtBeginDate.Value Is Nothing OrElse txtEndDate.Value Is Nothing Then
                    strMsg = Me.Language.Translate("CheckPage.Page3.IncorrectDates", Me.DefaultScope)
                ElseIf Me.txtBeginDate.Date > Me.txtEndDate.Date Then
                    strMsg = Me.Language.Translate("CheckPage.Page3.IncorrectPeriod", Me.DefaultScope)
                ElseIf Me.CheckPeriodOfFreezingPage(Me.txtBeginDate.Date) Then
                    strMsg = Me.Language.Translate("CheckPage.Page3.FreezingDate", Me.DefaultScope)
                ElseIf Me.CheckPeriodOfFreezingPage(Me.txtEndDate.Date) Then
                    strMsg = Me.Language.Translate("CheckPage.Page3.FreezingDate", Me.DefaultScope)
                ElseIf Math.Abs(DateDiff(DateInterval.Day, Me.txtEndDate.Date, txtBeginDate.Date)) > 31 Then
                    strMsg = Me.Language.Translate("CheckPage.Page3.LongPeriod", Me.DefaultScope)
                Else
                    ' Validamos si alguno de los empleados seleccionados tiene fecha de cierre dentro del periodo seleccionado
                    Dim DateIni As DateTime = txtBeginDate.Date
                    Dim DateFin As DateTime = txtEndDate.Date

                    Dim strIncidences As String = Me.GetList(Me.treeIncidences).Split("~")(1)
                    Dim lstEmployeesDest As New Generic.List(Of Integer)
                    Dim lstGroups As New Generic.List(Of Integer)

                    'obtener todos los empleados de los grupos seleccionados en el arbol v3
                    roSelectorManager.ExtractIdsFromSelectionString(Me.hdnEmployeesSelected.Value, lstEmployeesDest, lstGroups)
                    If lstGroups IsNot Nothing Then
                        Dim strFilter As String = Me.hdnFilter.Value
                        Dim strFilterUser As String = Me.hdnFilterUser.Value
                        Dim tmp As Generic.List(Of Integer) = API.EmployeeGroupsServiceMethods.GetEmployeeListFromGroupRecursiveInDates(Me, lstGroups.ToArray, FeatureAlias, "U",
                                                                                                                                               strFilter, strFilterUser, Me.txtBeginDate.Date, Me.txtEndDate.Date)
                        lstEmployeesDest.AddRange(tmp)
                    End If

                    Dim arrEmployeesDest As String = "-1"
                    For Each intID As Integer In lstEmployeesDest
                        arrEmployeesDest += "," & intID.ToString
                    Next

                    Dim tmp_Employees As DataTable = API.EmployeeServiceMethods.GetEmployeesOnLockDate(Me, arrEmployeesDest, Me.txtEndDate.Date)
                    If tmp_Employees IsNot Nothing AndAlso tmp_Employees.Rows.Count > 0 Then
                        strMsg = Me.Language.Translate("CheckPage.Page3.FreezingDate", Me.DefaultScope)
                    Else
                        tmp_Employees = API.EmployeeServiceMethods.GetEmployeesOnLockDate(Me, arrEmployeesDest, Me.txtBeginDate.Date)
                        If tmp_Employees IsNot Nothing AndAlso tmp_Employees.Rows.Count > 0 Then
                            strMsg = Me.Language.Translate("CheckPage.Page3.FreezingDate", Me.DefaultScope)
                        End If
                    End If
                End If

                If strMsg <> "" Then bolRet = False
                Me.lblStep3Error.Text = strMsg

            Case 4
                ' revisar que haya algun fitro seleccionado
                Dim strIncidences As String = Me.GetList(Me.treeIncidences).Split("~")(1)
                If strIncidences = "-1" Then
                    strMsg = Me.Language.Translate("CheckPage.Page4.IncidencesType", Me.DefaultScope)
                End If

                If strMsg <> "" Then bolRet = False
                Me.lblStep4Error.Text = strMsg

            Case 5

            Case 6
                ' revisar que haya algun fitro seleccionado
                Dim oCentersList As Generic.List(Of Object) = grdCenters.GetSelectedFieldValues("ID")
                If oCentersList.Count = 0 Then
                    strMsg = Me.Language.Translate("CheckPage.Page5.EmptyData", Me.DefaultScope)
                End If

                If strMsg <> "" Then bolRet = False
                Me.lblStep5Error.Text = strMsg

            Case 7
                ' Revisar que no haya ningún registro en edición y que haya registros
                If Me.IncidencesDataTable Is Nothing Then
                    strMsg = Me.Language.Translate("CheckPage.Page6.EmtyData", Me.DefaultScope)
                ElseIf Me.IncidencesDataTable.Rows.Count = 0 Then
                    strMsg = Me.Language.Translate("CheckPage.Page6.EmtyData", Me.DefaultScope)
                ElseIf Me.GridIncidences.IsEditing Then
                    strMsg = Me.Language.Translate("CheckPage.Page6.EditingData", Me.DefaultScope)
                End If
                If strMsg <> "" Then bolRet = False
                Me.lblStep7Error.Text = strMsg

        End Select

        Return bolRet

    End Function

    Private Sub PageChange(ByVal intOldPage As Integer, ByVal intActivePage As Integer)

        Select Case intOldPage
            Case 1

            Case 2
                ' Desactivar el iframe del selector de grupos
                Me.ifEmployeeSelector.Attributes("src") = Me.ResolveUrl("~/Base/BlankPage.aspx")
                Me.ifEmployeeSelector.Disabled = True
        End Select

        Select Case intActivePage
            Case 2
                ' Inicializar selección del selector de empleados
                Dim strAux As String = "~/Base/WebUserControls/roWizardSelectorContainerMultiSelectV3.aspx?" &
                                       "PrefixTree=treeEmpIncidencesWizard&FeatureAlias=Calendar.JustifyIncidences&PrefixCookie=objContainerTreeV3_treeEmpIncidencesWizardGrid&" &
                                       "AfterSelectFuncion=parent.GetSelectedTreeV3"
                Me.ifEmployeeSelector.Attributes("src") = Me.ResolveUrl(strAux)
                Me.ifEmployeeSelector.Disabled = False
            Case 5
                GetJustCausesData(True)
                GetJustCausesDataAll(True)
                BindGridCauses(True)
            Case 6
                BindGridCenters(True)
                GetCentersDataAll(True)
                GetCentersDataAllDetail(True)
            Case 7
                GetJustCausesData(True)
                GetJustCausesDataAll(True)
                BindGridIncidences(True)
        End Select

        Me.hdnActiveFrame.Value = intActivePage

        ' Hacer invisible página anterior
        Dim oPage As HtmlGenericControl = HelperWeb.GetControl(Me.Page.Controls, "divStep" & intOldPage)
        If oPage IsNot Nothing Then
            oPage.Style("display") = "none"
        End If
        ' Hacer visible página actual
        oPage = HelperWeb.GetControl(Me.Page.Controls, "divStep" & intActivePage)
        If oPage IsNot Nothing Then
            oPage.Style("display") = "block"
        End If

        If intOldPage = 7 And intActivePage = 0 Then
            Me.btPrev.Visible = False '.Style("display") = "none"
            Me.btNext.Visible = False '.Style("display") = "none"
            Me.btEnd.Visible = False '.Style("display") = "none"
        Else
            Me.btPrev.Visible = IIf(intActivePage > 0 And intActivePage < 7, True, False) '.Style("display") = IIf(Me.FramePos(oActiveFrame) > 0, "block", "none")
            Me.btNext.Visible = IIf(intActivePage < 7, True, False) '.Style("display") = IIf(Me.FramePos(oActiveFrame) < Me.Frames.Count - 1, "block", "none")
            Me.btEnd.Visible = IIf(intActivePage = 7, True, False) '.Style("display") = IIf(Me.FramePos(oActiveFrame) = Me.Frames.Count - 1, "block", "none")
            If intActivePage = 7 Then
                If Me.IncidencesDataTable Is Nothing Then
                    Me.btEnd.Visible = False
                ElseIf Me.IncidencesDataTable.Rows.Count = 0 Then
                    Me.btEnd.Visible = False
                End If
                If Not Me.btEnd.Visible Then
                    Me.btPrev.Visible = True
                End If
            End If
        End If

    End Sub

    Private Sub CloseWizard()
        If Not ErrorExists Then
            Me.MustRefresh = "1"
            Me.lblCopyScheduleWelcome1.Text = Me.Language.Translate("End.Ok.AssignCausesWelcome1.Text", Me.DefaultScope)
            Me.lblCopyScheduleWelcome2.Text = Me.Language.Translate("End.Ok.AssignCausesWelcome2.Text", Me.DefaultScope)
            Me.lblCopyScheduleWelcome3.Text = ""
            Me.btClose.Text = Me.Language.Keyword("Button.Close")
        Else
            Me.lblCopyScheduleWelcome1.Text = Me.Language.Translate("End.Error.AssignCausesWelcome1.Text", Me.DefaultScope)
            Me.lblCopyScheduleWelcome2.Text = Me.Language.Translate("End.Error.AssignCausesWelcome2.Text", Me.DefaultScope)
            Me.lblCopyScheduleWelcome3.Text = ErrorDescription
            Me.lblCopyScheduleWelcome3.ForeColor = Drawing.Color.Red
            Me.btClose.Text = Me.Language.Keyword("Button.Close")
        End If

        ErrorDescription = Nothing
        ErrorExists = Nothing
        iCurrentTask = Nothing

        Me.PageChange(7, 0)
    End Sub

    Private Function AssignCauses() As Integer

        Dim iTask As Integer = -1

        Me.lblCopyScheduleWelcome1.Text = Me.Language.Translate("End.AssignCausesWelcome1.Text", Me.DefaultScope)

        Dim tbIncidences As DataTable = Me.IncidencesDataTable

        If ckMoveAllTo.Checked Then
            Dim oListIDs As Generic.List(Of Object) = GridIncidences.GetSelectedFieldValues("Clave")

            For Each oGuid As Object In oListIDs
                Dim oRows() As DataRow = tbIncidences.Select("Clave='" & roTypes.Any2String(oGuid) & "'")
                If oRows.Length > 0 Then
                    If cmbCauses.SelectedItem IsNot Nothing Then

                        ' revismos si ha cambiado el centro
                        If CShort(cmbCauses.SelectedItem.Value) <> CShort(oRows(0)("IDCause")) Then
                            oRows(0)("IDCause") = cmbCauses.SelectedItem.Value
                            oRows(0)("Manual") = 1
                        End If
                    End If
                End If
            Next
        End If

        iTask = API.LiveTasksServiceMethods.JustifiedIncidencesInBackground(Me, tbIncidences, Me.txtBeginDate.Value, Me.txtEndDate.Value, True)
        If iTask > 0 Then
            Try
                Dim lstAuditParameterNames As New List(Of String)
                Dim lstAuditParameterValues As New List(Of String)

                lstAuditParameterNames.Add("{iTask}")
                lstAuditParameterValues.Add(iTask)

                lstAuditParameterNames.Add("{DateIni}")
                lstAuditParameterValues.Add(CDate(Me.txtBeginDate.Value).ToShortDateString())

                lstAuditParameterNames.Add("{DateEnd}")
                lstAuditParameterValues.Add(CDate(Me.txtEndDate.Value).ToShortDateString())

                API.AuditServiceMethods.Audit(Robotics.VTBase.Audit.Action.aUpdate, Robotics.VTBase.Audit.ObjectType.tJustifiedIncidences, "", lstAuditParameterNames, lstAuditParameterValues, Me.Page)
            Catch ex As Exception
            End Try
        End If
        Return iTask
    End Function

#End Region

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
        CType(GridIncidences.Columns("CenterName"), GridViewDataColumn).EditItemTemplate = template
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

            txt.MaskSettings.Mask = "HH:mm"
        End If
    End Sub

    Protected Sub grdCenters_DataBound(ByVal sender As Object, ByVal e As EventArgs) Handles grdCenters.DataBound
        Dim grid As ASPxGridView = TryCast(sender, ASPxGridView)

    End Sub

    Private Function GetCentersDataAll(Optional ByVal bolReload As Boolean = False, Optional ByVal bolCheckStatus As Boolean = False) As DataView

        If bolReload Or Session("IncidencesWizard_CentersDataAll") Is Nothing Then
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

                Session("IncidencesWizard_CentersDataAll") = returnTb

                dv = New DataView(returnTb)
                dv.Sort = "Name ASC"
            End If

            Return dv
        Else
            Dim dtReturn As DataTable = Session("IncidencesWizard_CentersDataAll")
            Dim dv As DataView = New DataView(dtReturn)
            dv.Sort = "Name ASC"
            Return dv
        End If
    End Function

    Private Function GetCentersDataAllDetail(Optional ByVal bolReload As Boolean = False, Optional ByVal bolCheckStatus As Boolean = False) As DataView

        If bolReload Or Session("IncidencesWizard_CentersDataAllDetail") Is Nothing Then
            Dim dv As DataView = Nothing
            Dim returnTb As New DataTable
            returnTb.Columns.Add(New DataColumn("ID", GetType(Short)))
            returnTb.Columns.Add(New DataColumn("Name", GetType(String)))
            Dim oNewRow As DataRow = returnTb.NewRow

            Dim tb As DataTable = API.TasksServiceMethods.GetBusinessCenters(Me.Page, bolCheckStatus)
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

                Session("IncidencesWizard_CentersDataAllDetail") = returnTb

                dv = New DataView(returnTb)
                dv.Sort = "Name ASC"
            End If

            Return dv
        Else
            Dim dtReturn As DataTable = Session("IncidencesWizard_CentersDataAllDetail")
            Dim dv As DataView = New DataView(dtReturn)
            dv.Sort = "Name ASC"
            Return dv
        End If
    End Function

    Public Property FreezingDatePage() As Date
        Get
            Return CDate(ViewState("Incidences_FreezingDate"))
        End Get
        Set(ByVal value As Date)
            ViewState("Incidences_FreezingDate") = value
        End Set
    End Property

    Public Property CostCenterPermission() As Permission
        Get
            Return ViewState("IncidencesWizard_CostCenterPermission")
        End Get
        Set(ByVal value As Permission)
            ViewState("IncidencesWizard_CostCenterPermission") = value
        End Set
    End Property

    Private Function CheckPeriodOfFreezing(ByVal xDate As Date) As Boolean
        Dim bolRet As Boolean = False

        'If (Me.FreezingDatePage >= xDate) Then
        ' bolRet = True
        ' End If

        If Not bolRet Then
            ' Verificamos que no estemos intentando modificar un día futuro.
            bolRet = Not (xDate <= Now.Date)
        End If

        Return bolRet
    End Function

    Private Function CheckPeriodOfFreezingPage(ByVal xDate As Date) As Boolean
        Dim bolFreeze As Boolean = False
        bolFreeze = CheckPeriodOfFreezing(xDate)
        Return bolFreeze
    End Function

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

    Protected Sub GridIncidences_CustomCallback(sender As Object, e As ASPxGridViewCustomCallbackEventArgs) Handles GridIncidences.CustomCallback
        If e.Parameters = "RELOAD" Then
            Dim tbIncidences As DataTable = Me.IncidencesDataTable
            If Not IncidencesHasChanges(tbIncidences) Then
                BindGridIncidences(True)
            End If

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

    Protected Sub GridIncidences_InitNewRow(sender As Object, e As Data.ASPxDataInitNewRowEventArgs) Handles GridIncidences.InitNewRow
        AssignTemplatesInReadOnlyColumns()
    End Sub

    Protected Sub GridIncidences_RowUpdating(sender As Object, e As Data.ASPxDataUpdatingEventArgs) Handles GridIncidences.RowUpdating
        Dim tb As DataTable = Me.IncidencesDataTable()
        Dim dr As DataRow = tb.Rows.Find(e.Keys(GridIncidences.KeyFieldName))
        Dim grid As ASPxGridView = CType(sender, ASPxGridView)

        Dim PruebaMinutos As Integer = 0
        Dim RowTime As Double = 0
        Dim GridTime As Double = 0

        'obtener la hora guardada
        If Not dr("ValueHoraEditable") Is System.DBNull.Value Then
            PruebaMinutos = roConversions.ConvertTimeToMinutes(dr("ValueHoraEditable"))
            RowTime = roConversions.ConvertTimeToHours(dr("ValueHoraEditable"))
        End If

        Dim enumerator As IDictionaryEnumerator = e.NewValues.GetEnumerator()
        enumerator.Reset()
        While enumerator.MoveNext()
            If enumerator.Key.ToString() = "ValueHoraEditable" Then
                GridTime = roConversions.ConvertTimeToHours(enumerator.Value)
            End If
            dr(enumerator.Key.ToString()) = enumerator.Value
        End While

        'Comprobar si es direct cause
        Dim IsDirectCause As Boolean = (roTypes.Any2Integer(dr("IDRelatedIncidence")) <> 0)

        If RowTime <> 0 AndAlso RowTime > GridTime AndAlso IsDirectCause Then
            'la hora anterior es mas grande que la nueva
            Dim oNewRow As DataRow = tb.NewRow

            oNewRow.Item("EmployeesName") = dr("EmployeesName")
            oNewRow.Item("IDEmployee") = dr("IDEmployee")
            oNewRow.Item("IDContract") = dr("IDContract")
            oNewRow.Item("Date") = dr("Date")
            oNewRow.Item("IDRelatedIncidence") = dr("IDRelatedIncidence")
            oNewRow.Item("IDCause") = 0
            oNewRow.Item("AccrualsRules") = 0
            oNewRow.Item("DailyRule") = 0
            oNewRow.Item("AccruedRule") = 0
            oNewRow.Item("Manual") = 1
            oNewRow.Item("IDCauseOld") = 0
            oNewRow.Item("PunchesLst") = dr("PunchesLst")

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

            oNewRow.Item("ManualCenter") = dr("ManualCenter")
            oNewRow.Item("IDCenter") = dr("IDCenter")
            oNewRow.Item("DefaultCenter") = dr("DefaultCenter")
            oNewRow.Item("CenterName") = dr("CenterName")

            oNewRow.Item("Clave") = Guid.NewGuid()

            'corregir la fila modificada
            dr.Item("Value") = GridTime
            dr.Item("Manual") = 1
            dr.Item("ValueHora") = roConversions.ConvertHoursToTime(dr.Item("Value"))
            dr.Item("ValueHoraEditable") = dr.Item("ValueHora")

            tb.Rows.Add(oNewRow)
        Else

            'corregir la fila modificada
            dr.Item("Value") = GridTime
            dr.Item("Manual") = 1
            dr.Item("ValueHora") = roConversions.ConvertHoursToTime(dr.Item("Value"))
            dr.Item("ValueHoraEditable") = dr.Item("ValueHora")

        End If

        Me.IncidencesDataTable = tb
        e.Cancel = True
        grid.CancelEdit()
    End Sub

    Protected Sub GridIncidences_StartRowEditing(sender As Object, e As Data.ASPxStartRowEditingEventArgs) Handles GridIncidences.StartRowEditing
        AssignTemplatesInReadOnlyColumns()
    End Sub

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