Imports DevExpress.Web
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Group
Imports Robotics.Base.VTEmployees.Employee
Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class Access_AccessFilterPlates
    Inherits PageBase

#Region "Properties de Clase"

    <Runtime.Serialization.DataContract()>
    Private Class ViewDetails

        <Runtime.Serialization.DataMember(Name:="nameView")>
        Public NameView As String

        <Runtime.Serialization.DataMember(Name:="description")>
        Public Description As String

        <Runtime.Serialization.DataMember(Name:="employees")>
        Public Employees As String

        <Runtime.Serialization.DataMember(Name:="dateInf")>
        Public DateInf As String

        <Runtime.Serialization.DataMember(Name:="dateSup")>
        Public DateSup As String

        <Runtime.Serialization.DataMember(Name:="filterData")>
        Public FilterData As String

    End Class

    Private ReadOnly Property GetGroups() As Generic.List(Of Integer)
        Get
            Dim Groups As Generic.List(Of Integer) = ViewState("AbsencesStatusFilter_Groups")
            If Groups Is Nothing Then
                Dim tempGroupsTb As DataTable = API.EmployeeGroupsServiceMethods.GetGroups(Me, "Access")
                Dim fieldValues As IEnumerable(Of Integer) = tempGroupsTb.AsEnumerable().Select(Function(row) roTypes.Any2Integer(row("ID")))

                ViewState("AbsencesStatusFilter_Groups") = fieldValues.ToList()
            End If
            Return Groups
        End Get
    End Property

    Private ReadOnly Property GetEmployees() As Generic.List(Of Integer)
        Get
            Dim EmployeesNotAllowed As Generic.List(Of Integer) = ViewState("AbsencesStatusFilter_Employees")
            If EmployeesNotAllowed Is Nothing Then
                Dim tempEmpTb As DataTable = API.EmployeeServiceMethods.GetAllEmployees(Nothing, "", "Access")
                Dim fieldValues As IEnumerable(Of Integer) = tempEmpTb.AsEnumerable().Select(Function(row) roTypes.Any2Integer(row("IDEmployee")))

                ViewState("AbsencesStatusFilter_Employees") = fieldValues.ToList()
            End If
            Return EmployeesNotAllowed
        End Get
    End Property

#End Region

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("AccessFilterPlates", "~/Access/Scripts/AccessFilterPlates.js")
        Me.InsertExtraJavascript("roTreeState", "~/Base/Scripts/rocontrols/roTrees/roTreeState.js")
    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Not Me.IsPostBack Then
            HelperSession.DeleteEmployeeGroupsFromApplication()
        End If

        If Not IsPostBack AndAlso Not IsCallback Then

            Me.sectionName.InnerText = Me.Language.Translate("FilterPlatesTitle", Me.DefaultScope)

            CreateColumns()

            Dim oPermission As Permission = Me.GetFeaturePermission("Access.Zones.Supervision")
            If oPermission < Permission.Read Then
                WLHelperWeb.RedirectAccessDenied(True)
                Exit Sub
            End If

            'INICIALIZAR COOKIES DE USO PARA EL SELECTOR DE EMPLEADOS
            HelperWeb.roSelector_Initialize("objContainerTreeV3_treeEmpFilterPlates")
            HelperWeb.roSelector_Initialize("objContainerTreeV3_treeEmpFilterPlatesGrid")

            txtDateInf.Value = DateTime.Today
            txtDateSup.Value = DateTime.Today

            'NO TOCAR: Cargar propiedades sin postback para añadirlas al viewstate y que esten disponibles mas adelante en callbacks
            Dim lst As Generic.List(Of Integer) = Me.GetEmployees()
            lst = Me.GetGroups()
            ' FIN

            Me.GridFilterPlates.DataSource = GetDataAccess(True)
            Me.GridFilterPlates.DataBind()

            hdnAllEmployees.Value = Me.Language.Translate("AllEmployees", DefaultScope)
            txtEmployees.Text = hdnAllEmployees.Value

        End If

        Me.GridFilterPlates.DataSource = GetDataAccess()
        Me.GridFilterPlates.DataBind()

        BindGridViews()

    End Sub

    Protected Sub CallbackSession_Callback(ByVal source As Object, ByVal e As DevExpress.Web.CallbackEventArgs) Handles CallbackSession.Callback
        e.Result = String.Empty

        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Select Case strParameter.Trim.ToUpperInvariant
            Case "GETINFOSELECTED"
                e.Result = GetInfoSelected()
        End Select

        If strParameter.Substring(0, 8).ToUpperInvariant = "SAVEVIEW" Then
            e.Result = SaveViewCube(strParameter.Substring(9))
        End If

    End Sub

    Protected Sub CallbackPanelPivot_Callback(ByVal sender As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase) Handles CallbackPanelPivot.Callback

        If e.Parameter.Substring(0, 8).ToUpperInvariant = "LOADVIEW" Then
            LoadViewCube(e.Parameter.Substring(9))
        End If

        If e.Parameter.ToUpperInvariant = "LOADLAYOUT" Then
            LoadDefaultCube()
        End If

    End Sub

    Private Function SaveViewCube(ByVal strParameters As String) As Boolean

        Dim bolRet As Boolean = False

        Try

            Dim oViewDetails As New ViewDetails()
            oViewDetails = roJSONHelper.Deserialize(strParameters, oViewDetails.GetType())

            Dim tmpDateInf As DateTime = GetDateTimeObject(oViewDetails.DateInf)
            Dim tmpDateSup As DateTime = GetDateTimeObject(oViewDetails.DateSup)

            If oViewDetails.NameView Is Nothing Then oViewDetails.NameView = DateTime.Today.ToShortDateString
            If oViewDetails.Description Is Nothing Then oViewDetails.Description = String.Empty
            If oViewDetails.Employees Is Nothing Then oViewDetails.Employees = String.Empty

            Dim strCubeLayout As String = Me.GridFilterPlates.SaveClientLayout()
            If strCubeLayout Is Nothing Then strCubeLayout = String.Empty

            If oViewDetails.FilterData Is Nothing Then oViewDetails.FilterData = String.Empty

            bolRet = API.AccessMoveServiceMethods.NewAccessPlatesView(Me.Page, 1, WLHelperWeb.CurrentPassport.ID, oViewDetails.NameView, oViewDetails.Description,
                                                                                    DateTime.Today, oViewDetails.Employees, tmpDateInf, tmpDateSup, strCubeLayout, oViewDetails.FilterData)
        Catch ex As Exception
        End Try

        Return bolRet

    End Function

    Private Function LoadViewCube(ByVal strID As String) As Boolean

        Dim bolRet As Boolean = False

        Try

            Dim tb As DataTable = API.AccessMoveServiceMethods.GetAccessPlatesViewbyID(Me.Page, strID, WLHelperWeb.CurrentPassport.ID)
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                Dim vd As New ViewDetails
                Dim oRow As DataRow = tb.Rows(0)

                txtDateInf.Value = oRow("DateInf")
                txtDateSup.Value = oRow("DateSup")

                Dim arrEmployees As Array = roTypes.Any2String(oRow("Employees")).Split("#")
                If arrEmployees.Length <> 3 Then
                    hdnEmployees.Value = String.Empty
                    hdnFilter.Value = String.Empty
                    hdnFilterUser.Value = String.Empty
                Else
                    hdnEmployees.Value = arrEmployees(0)
                    hdnFilter.Value = arrEmployees(1)
                    hdnFilterUser.Value = arrEmployees(2)
                End If

                Dim CookiePlatesGrid As roTreeState = HelperWeb.roSelector_GetTreeState("objContainerTreeV3_treeEmpFilterPlatesGrid")
                CookiePlatesGrid.ActiveTree = "1"
                CookiePlatesGrid.Selected1 = hdnEmployees.Value
                HelperWeb.roSelector_SetTreeState(CookiePlatesGrid)

                Dim CookiePlates As roTreeState = HelperWeb.roSelector_GetTreeState("objContainerTreeV3_treeEmpFilterPlates")
                CookiePlates.ActiveTree = "1"
                CookiePlates.Filter = hdnFilter.Value
                CookiePlates.UserFieldFilter = hdnFilterUser.Value
                HelperWeb.roSelector_SetTreeState(CookiePlates)

                Dim arrPlates As Array = roTypes.Any2String(oRow("FilterData")).Split("¬")
                If arrPlates.Length = 4 Then
                    txtPlate1.Text = arrPlates(0)
                    txtPlate2.Text = arrPlates(1)
                    txtPlate3.Text = arrPlates(2)
                    txtPlate4.Text = arrPlates(3)
                Else
                    txtPlate1.Text = String.Empty
                    txtPlate2.Text = String.Empty
                    txtPlate3.Text = String.Empty
                    txtPlate4.Text = String.Empty
                End If

                Me.GridFilterPlates.DataSource = GetDataAccess(True)
                Me.GridFilterPlates.DataBind()
                CreateColumns()
                Dim sLayout As String = roTypes.Any2String(oRow("CubeLayout"))
                If sLayout <> String.Empty Then
                    Me.GridFilterPlates.LoadClientLayout(sLayout)
                End If

                Try
                    hdnEmployeesSelected.Value = "0"
                    txtEmployees.Text = hdnAllEmployees.Value
                    Dim strInfo As String = GetInfoSelected()
                    hdnEmployeesSelected.Value = strInfo.Split(";")(0)
                    txtEmployees.Text = strInfo.Split(";")(1)
                Catch ex As Exception
                End Try

                bolRet = True

            End If
        Catch ex As Exception
        End Try

        Return bolRet

    End Function

    Private Function LoadDefaultCube() As Boolean

        Dim bolRet As Boolean = False

        Try

            CreateColumns()

            Try
                hdnEmployeesSelected.Value = "0"
                txtEmployees.Text = hdnAllEmployees.Value
                Dim strInfo As String = GetInfoSelected()
                hdnEmployeesSelected.Value = strInfo.Split(";")(0)
                txtEmployees.Text = strInfo.Split(";")(1)
            Catch ex As Exception
            End Try

            bolRet = True
        Catch ex As Exception
        End Try

        Return bolRet

    End Function

#Region "GESTION DEL GRID DE VISTAS GUARDADAS"

    '=========================================================================
    '=========== GESTION DEL GRID DE VISTAS GUARDADAS =========================
    '=========================================================================
    Protected Sub GridViews_CustomCallback(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxGridViewCustomCallbackEventArgs) Handles GridViews.CustomCallback
        If e.Parameters = "RELOAD" Then
            BindGridViews()
        End If
    End Sub

    Private Sub BindGridViews()
        CreateColumnsGridViews()
        Me.GridViews.DataSource = API.AccessMoveServiceMethods.GetAccessPlatesViews(Me.Page, WLHelperWeb.CurrentPassport.ID)
        Me.GridViews.DataBind()
    End Sub

    Private Function GetDateTimeObject(ByVal strDate As String) As DateTime
        Dim tmpDate As DateTime = New DateTime(strDate.Substring(0, 4), strDate.Substring(5, 2), strDate.Substring(8, 2))
        Return tmpDate
    End Function

    Private Function SetDateTimeObject(ByVal Fecha As DateTime) As String
        Dim tmpDate As String = Fecha.Year & "#" & Fecha.Month.ToString.PadLeft(2, "0") & "#" & Fecha.Day.ToString.PadLeft(2, "0")
        Return tmpDate
    End Function

    Private Sub CreateColumnsGridViews()

        Dim GridColumn As GridViewDataTextColumn
        Dim GridColumnCommand As GridViewCommandColumn
        Dim GridColumnDate As GridViewDataDateColumn

        Dim VisibleIndex As Integer = 0

        Me.GridViews.Columns.Clear()
        Me.GridViews.KeyFieldName = "ID"
        Me.GridViews.Settings.ShowFilterRow = True
        Me.GridViews.SettingsText.EmptyDataRow = " "
        Me.GridViews.SettingsCommandButton.DeleteButton.Image.Url = "~/Base/Images/Grid/remove.png"
        Me.GridViews.SettingsCommandButton.EditButton.Image.Url = "~/Base/Images/Grid/edit.png"
        Me.GridViews.SettingsCommandButton.UpdateButton.Image.Url = "~/Base/Images/Grid/save.png"
        Me.GridViews.SettingsCommandButton.CancelButton.Image.Url = "~/Base/Images/Grid/cancel.png"
        Me.GridViews.Settings.VerticalScrollBarMode = ScrollBarMode.Auto

        Me.GridViews.SettingsPager.PageSize = 10
        Me.GridViews.SettingsPager.ShowEmptyDataRows = True

        'Command buttons
        GridColumnCommand = New GridViewCommandColumn()
        GridColumnCommand.ButtonRenderMode = GridCommandButtonRenderMode.Image
        GridColumnCommand.ShowDeleteButton = True
        GridColumnCommand.Caption = " "
        GridColumnCommand.VisibleIndex = VisibleIndex
        GridColumnCommand.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCommand.Width = 15
        VisibleIndex = VisibleIndex + 1

        Me.GridViews.Columns.Add(GridColumnCommand)

        'ID (Clave)
        GridColumn = New GridViewDataTextColumn()
        GridColumn.FieldName = "ID"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.Visible = False
        Me.GridViews.Columns.Add(GridColumn)

        'IdView
        GridColumn = New GridViewDataTextColumn()
        GridColumn.FieldName = "IdView"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.Visible = False
        Me.GridViews.Columns.Add(GridColumn)

        'NameView
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridViews.Column.Name", DefaultScope) 'Nombre
        GridColumn.FieldName = "NameView"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.Width = 40
        Me.GridViews.Columns.Add(GridColumn)

        'Description
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridViews.Column.Description", DefaultScope) 'Descripcion"
        GridColumn.FieldName = "Description"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.Width = 100
        Me.GridViews.Columns.Add(GridColumn)

        'DateView
        GridColumnDate = New GridViewDataDateColumn
        GridColumnDate.Caption = Me.Language.Translate("GridViews.Column.Date", DefaultScope) 'Fecha"
        GridColumnDate.FieldName = "DateView"
        GridColumnDate.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnDate.ReadOnly = True
        GridColumnDate.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnDate.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnDate.Width = 15
        Me.GridViews.Columns.Add(GridColumnDate)

    End Sub

    Protected Sub GridViews_RowDeleting(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataDeletingEventArgs) Handles GridViews.RowDeleting

        Try

            Dim tb As DataTable = CType(GridViews.DataSource, DataTable)
            If Not tb Is Nothing Then
                Dim dr As DataRow = tb.Rows.Find(e.Keys(GridViews.KeyFieldName))
                API.AccessMoveServiceMethods.DeleteAccessPlatesView(Me.Page, roTypes.Any2Integer(dr("ID")))
                dr.Delete()
                e.Cancel = True
                tb.AcceptChanges()
            End If
        Catch ex As Exception
        End Try

    End Sub

#End Region

#Region "OBTENCION DE DATOS DE LINQ"

    Private Function GetDataAccess(Optional ByVal bolReload As Boolean = False) As Object

        Dim objPlates As Object = Session("AccessFilterPlates_PlatesData")
        Dim LastPlatesUsed As String = Session("AccessFilterPlates_LastPlatesUsed")
        Dim LastEmployeesUsed As String = Session("AccessFilterPlates_LastEmployeesUsed")
        Dim LastDateInfUsed As Nullable(Of DateTime) = Session("AccessFilterPlates_LastDateInfUsed")
        Dim LastDateSupUsed As Nullable(Of DateTime) = Session("AccessFilterPlates_LastDateSupUsed")

        Dim strEmployeeFilters As String = hdnEmployees.Value & "|" & hdnFilter.Value & "|" & hdnFilterUser.Value
        Dim ActualPlates As String = txtPlate1.Text.Trim() & txtPlate2.Text.Trim() & txtPlate3.Text.Trim() & txtPlate4.Text.Trim()

        If bolReload OrElse objPlates Is Nothing OrElse
           LastPlatesUsed <> ActualPlates OrElse
           LastEmployeesUsed <> strEmployeeFilters OrElse
           Not LastDateInfUsed.HasValue OrElse LastDateInfUsed.Value <> txtDateInf.Text OrElse
           Not LastDateSupUsed.HasValue OrElse LastDateSupUsed.Value <> txtDateSup.Text Then

            Dim DateInf As DateTime = txtDateInf.Value
            Dim DateSup As DateTime = txtDateSup.Value

            If DateInf > DateSup Then
                Dim aux As DateTime = DateSup
                DateSup = DateInf
                DateInf = aux
            End If

            Dim lstEmployees As Generic.List(Of Integer) = Nothing
            Dim lstGroups As Generic.List(Of Integer) = Nothing

            Robotics.Security.roSelector.GetExpandedGroupsAndEmployees(hdnEmployees.Value, lstEmployees, lstGroups)

            Dim strConn As String = API.UserAdminServiceMethods.GetConnectionString(Me, WLHelperWeb.CurrentPassport.ID)
            Dim ContextDB As AccessDataContext = New AccessDataContext(strConn)
            ContextDB.CommandTimeout = 900

            'Matriculas
            Dim lstPlates As New Generic.List(Of String)
            If txtPlate1.Text.Trim <> String.Empty Then lstPlates.Add(txtPlate1.Text.Trim)
            If txtPlate2.Text.Trim <> String.Empty Then lstPlates.Add(txtPlate2.Text.Trim)
            If txtPlate3.Text.Trim <> String.Empty Then lstPlates.Add(txtPlate3.Text.Trim)
            If txtPlate4.Text.Trim <> String.Empty Then lstPlates.Add(txtPlate4.Text.Trim)

            If lstEmployees.Count > 0 OrElse lstGroups.Count > 0 Then

                If lstEmployees.Count = 0 Then lstEmployees.Add(-1)

                Dim qry As IQueryable(Of sysroAccessPlates) = From AccessData In ContextDB.sysroAccessPlates Select AccessData Where AccessData.DatePunche >= DateInf And AccessData.DatePunche <= DateSup

                If lstPlates.Count > 0 Then 'SI MATRICULAS
                    qry = From AccessData In qry Select AccessData Where lstPlates.Contains(AccessData.TypeDetails)
                End If

                'qry = qry.Where(BuildPredicate(lstEmployees, lstGroups, Me.GetGroups, Me.GetEmployees))

                If hdnFilter.Value <> "" AndAlso hdnFilter.Value.Length = 5 AndAlso hdnFilter.Value <> "11110" Then 'Filtros de Empleados
                    qry = BuildEmployeeFilter(qry)
                End If

                Dim tmpList As Generic.List(Of sysroAccessPlates) = qry.ToList()
                tmpList = tmpList.FindAll(Function(item) (lstEmployees.Contains(item.IDEmployee) AndAlso Me.GetEmployees().Contains(item.IDEmployee)) OrElse
                    (lstGroups.Contains(item.IDGroup) AndAlso Me.GetGroups.Contains(item.IDGroup) AndAlso Me.GetEmployees.Contains(item.IDEmployee)))

                qry = tmpList.AsQueryable
                'qry = (From p In qry Select p.ID, p.IDEmployee, p.DatePunche, p.TimePunche, p.TypeDetails, p.NameEmployee, p.NameZone).ToList
                objPlates = (From p In tmpList Select p.ID, p.IDEmployee, p.DatePunche, p.TimePunche, p.TypeDetails, p.NameEmployee, p.NameZone).ToList



            Else

                If lstPlates.Count > 0 Then 'NO EMPLEADOS SI MATRICULAS

                    Dim qry As IQueryable(Of sysroAccessPlates) = From AccessData In ContextDB.sysroAccessPlates
                                                                  Select AccessData Where lstPlates.Contains(AccessData.TypeDetails) AndAlso AccessData.DatePunche.Date >= DateInf AndAlso AccessData.DatePunche.Date <= DateSup

                    qry = qry.Where(BuildPredicate(New List(Of Integer), New List(Of Integer), Me.GetGroups, Me.GetEmployees))
                    'Dim qry = From p In ContextDB.sysroAccessPlates  Where lstPlates.Contains(p.TypeDetails) And p.DatePunche.Date >= DateInf And p.DatePunche.Date <= DateSup Select p.ID, p.IDEmployee, p.DatePunche, p.TimePunche, p.TypeDetails, p.NameEmployee, p.NameZone
                    objPlates = (From p In qry Select p.ID, p.IDEmployee, p.DatePunche, p.TimePunche, p.TypeDetails, p.NameEmployee, p.NameZone).ToList()
                Else 'NO EMPLEADOS NO MATRICULAS

                    Dim qry As IQueryable(Of sysroAccessPlates) = From AccessData In ContextDB.sysroAccessPlates
                                                                  Select AccessData Where AccessData.DatePunche.Date >= DateInf AndAlso AccessData.DatePunche.Date <= DateSup


                    'qry = qry.Where(BuildPredicate(New List(Of Integer), New List(Of Integer), Me.GetGroups, Me.GetEmployees))

                    Dim tmpList As Generic.List(Of sysroAccessPlates) = qry.ToList()
                    tmpList = tmpList.FindAll(Function(item) (Me.GetEmployees().Contains(item.IDEmployee)) OrElse
                    (Me.GetGroups.Contains(item.IDGroup) AndAlso Me.GetEmployees.Contains(item.IDEmployee)))

                    qry = tmpList.AsQueryable
                    objPlates = (From p In qry Select p.ID, p.IDEmployee, p.DatePunche, p.TimePunche, p.TypeDetails, p.NameEmployee, p.NameZone).ToList()

                End If

            End If

            Session("AccessFilterPlates_PlatesData") = objPlates
            Session("AccessFilterPlates_LastPlatesUsed") = ActualPlates
            Session("AccessFilterPlates_LastEmployeesUsed") = strEmployeeFilters
            Session("AccessFilterPlates_LastDateInfUsed") = DateInf
            Session("AccessFilterPlates_LastDateSupUsed") = DateSup

        End If

        Return objPlates

    End Function

    Private Function BuildEmployeeFilter(ByVal qryParam As IQueryable(Of sysroAccessPlates)) As IQueryable(Of sysroAccessPlates)

        Dim qryFilter As IQueryable(Of sysroAccessPlates) = qryParam
        Dim lstEmployeesFilter As New Generic.List(Of Integer)

        If Mid(hdnFilter.Value, 1, 1) = "0" Then  'Empleado actual
            qryFilter = qryFilter.Where(Function(item) item.CurrentEmployee <> 1 Or item.EndDate < roTypes.Any2Time("1/1/2079").ValueDateTime)
        End If
        If Mid(hdnFilter.Value, 2, 1) = "0" Then  'Empleado en movimiento
            qryFilter = qryFilter.Where(Function(item) item.EndDate >= roTypes.Any2Time("1/1/2079").ValueDateTime)
        End If
        If Mid(hdnFilter.Value, 3, 1) = "0" Then  'Empleado de baja
            qryFilter = qryFilter.Where(Function(item) item.CurrentEmployee <> 0 Or item.BeginDate > roTypes.Any2Time("1/1/2079").ValueDateTime)
        End If
        If Mid(hdnFilter.Value, 4, 1) = "0" Then  'Empleado futur
            qryFilter = qryFilter.Where(Function(item) item.BeginDate <= roTypes.Any2Time("1/1/2079").ValueDateTime)
        End If

        If Mid(hdnFilter.Value, 5, 1) = "1" AndAlso hdnFilterUser.Value <> String.Empty Then
            Dim sFilterUserSQL As String = HelperWeb.ParseFilterUserFields(hdnFilterUser.Value)
            If sFilterUserSQL <> String.Empty Then
                Dim sLista As String = API.CommonServiceMethods.GetEmployeeListFromFilter(Me, "@SELECT# ID FROM EMPLOYEES WHERE " & sFilterUserSQL)
                For Each item In sLista.Split(",")
                    lstEmployeesFilter.Add(item)
                Next
                If lstEmployeesFilter.Count > 0 Then
                    qryFilter = From AccessData In qryFilter Select AccessData Where lstEmployeesFilter.Contains(AccessData.IDEmployee)
                End If
            End If
        End If

        Return qryFilter

    End Function

    Private Function BuildPredicate(ByVal lstEmployees As Generic.List(Of Integer),
                                    ByVal lstGroups As Generic.List(Of Integer),
                                    ByVal lstGroupsWithPerm As Generic.List(Of Integer),
                                    ByVal lstEmpWithPerm As Generic.List(Of Integer)) As Expressions.Expression(Of System.Func(Of sysroAccessPlates, Boolean))

        'outer 1 and de ID groups
        Dim predicateIDGroup As Expressions.Expression(Of System.Func(Of sysroAccessPlates, Boolean)) = Nothing
        If lstGroups.Count > 0 Then
            predicateIDGroup = PredicateBuilder.True(Of sysroAccessPlates)()
            predicateIDGroup = predicateIDGroup.And((Function(p1 As sysroAccessPlates) lstGroups.Contains(p1.IDGroup) AndAlso lstGroupsWithPerm.Contains(p1.IDGroup) AndAlso lstEmpWithPerm.Contains(p1.IDEmployee)))
        Else
            predicateIDGroup = PredicateBuilder.True(Of sysroAccessPlates)()
            predicateIDGroup = predicateIDGroup.And((Function(p1 As sysroAccessPlates) lstGroupsWithPerm.Contains(p1.IDGroup) AndAlso lstEmpWithPerm.Contains(p1.IDEmployee)))
        End If

        Dim predicateIDEmployee As Expressions.Expression(Of System.Func(Of sysroAccessPlates, Boolean)) = Nothing
        If lstGroups.Count > 0 Then
            predicateIDEmployee = PredicateBuilder.True(Of sysroAccessPlates)()
            predicateIDEmployee = predicateIDEmployee.And((Function(p1 As sysroAccessPlates) lstEmployees.Contains(p1.IDEmployee) AndAlso lstEmpWithPerm.Contains(p1.IDEmployee)))
        Else
            predicateIDEmployee = PredicateBuilder.True(Of sysroAccessPlates)()
            predicateIDEmployee = predicateIDEmployee.And((Function(p1 As sysroAccessPlates) lstEmpWithPerm.Contains(p1.IDEmployee)))
        End If

        If predicateIDGroup IsNot Nothing OrElse predicateIDEmployee IsNot Nothing Then
            'outer ppal and de employees
            Dim predicate = PredicateBuilder.False(Of sysroAccessPlates)()

            If predicateIDGroup IsNot Nothing Then predicate = predicate.Or(predicateIDGroup)

            If predicateIDEmployee IsNot Nothing Then predicate = predicate.Or(predicateIDEmployee)

            Return predicate
        Else
            Return PredicateBuilder.False(Of sysroAccessPlates)()
        End If

    End Function

#End Region

    Private Sub CreateColumns()

        Dim GridColumn As GridViewDataColumn
        Dim GridColumnDate As GridViewDataDateColumn

        Dim VisibleIndex As Integer = 0

        Me.GridFilterPlates.Columns.Clear()
        Me.GridFilterPlates.KeyFieldName = "ID"
        Me.GridFilterPlates.Settings.UseFixedTableLayout = True

        Me.GridFilterPlates.SettingsBehavior.AllowSelectSingleRowOnly = True
        Me.GridFilterPlates.SettingsBehavior.AllowFocusedRow = True

        Me.GridFilterPlates.SettingsPager.PageSize = 14
        Me.GridFilterPlates.SettingsPager.ShowEmptyDataRows = True

        'ID
        GridColumn = New DevExpress.Web.GridViewDataTextColumn()
        GridColumn.FieldName = "ID"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.Visible = False
        Me.GridFilterPlates.Columns.Add(GridColumn)

        'DatePunche
        GridColumnDate = New DevExpress.Web.GridViewDataDateColumn
        GridColumnDate.Caption = Me.Language.Translate("GridFilterPlates.Column.DatePunche", DefaultScope)
        GridColumnDate.FieldName = "DatePunche"
        GridColumnDate.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnDate.ReadOnly = True
        GridColumnDate.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnDate.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnDate.Width = 50
        Me.GridFilterPlates.Columns.Add(GridColumnDate)

        'Time
        GridColumn = New DevExpress.Web.GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridFilterPlates.Column.TimePunche", DefaultScope)
        GridColumn.FieldName = "TimePunche"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.Width = 40
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.Settings.AutoFilterCondition = AutoFilterCondition.Contains
        GridColumn.SortAscending()
        GridColumn.SortIndex = 2
        Me.GridFilterPlates.Columns.Add(GridColumn)

        'TypeDetails (matricula)
        GridColumn = New DevExpress.Web.GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridFilterPlates.Column.TypeDetails", DefaultScope)
        GridColumn.FieldName = "TypeDetails"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Width = 65
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Settings.AutoFilterCondition = AutoFilterCondition.Contains
        GridColumn.SortAscending()
        GridColumn.SortIndex = 1
        Me.GridFilterPlates.Columns.Add(GridColumn)

        'IDEmployee
        GridColumn = New DevExpress.Web.GridViewDataTextColumn()
        GridColumn.FieldName = "IDEmployee"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.Visible = False
        Me.GridFilterPlates.Columns.Add(GridColumn)

        'Name Employee
        GridColumn = New DevExpress.Web.GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridFilterPlates.Column.NameEmployee", DefaultScope)
        GridColumn.FieldName = "NameEmployee"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Width = 125
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Settings.AutoFilterCondition = AutoFilterCondition.Contains
        GridColumn.SortAscending()
        GridColumn.SortIndex = 2
        Me.GridFilterPlates.Columns.Add(GridColumn)

        'Name Zone
        GridColumn = New DevExpress.Web.GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridFilterPlates.Column.NameZone", DefaultScope)
        GridColumn.FieldName = "NameZone"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Width = 90
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Settings.AutoFilterCondition = AutoFilterCondition.Contains
        GridColumn.SortAscending()
        GridColumn.SortIndex = 2
        Me.GridFilterPlates.Columns.Add(GridColumn)

    End Sub

#Region "SELECTOR DE EMPLEADOS"

    Private Function GetInfoSelected() As String
        Dim strRet As String = String.Empty

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
                            Dim NumEmployees As Integer = HelperWeb.GetNumOfEmployeesAnalitics(Me.hdnEmployees.Value, Me.hdnFilter.Value, Me.hdnFilterUser.Value, "Access", DateInf, DateSup)
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
                    Dim NumEmployees As Integer = HelperWeb.GetNumOfEmployeesAnalitics(Me.hdnEmployees.Value, Me.hdnFilter.Value, Me.hdnFilterUser.Value, "Access", DateInf, DateSup)
                    strRet = NumEmployees & ";" & NumEmployees.ToString & " " & Me.Language.Translate("GridPlates.EmployeesSelected", DefaultScope)
                End If
            End If
        Catch
            strRet = "0; "
        End Try

        Return strRet

    End Function

#End Region

    Protected Sub btnExportToXls_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnExportToXls.Click
        Me.ASPxGridViewExporter1.GridViewID = Me.GridFilterPlates.ID
        Me.ASPxGridViewExporter1.WriteXlsxToResponse(True)
    End Sub

End Class