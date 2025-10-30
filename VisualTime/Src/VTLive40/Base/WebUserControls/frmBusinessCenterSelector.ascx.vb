Imports DevExpress.Web
Imports DevExpress.Web.ASPxTreeList
Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBusiness.BusinessCenter
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Public Class frmBusinessCenterSelector
    Inherits UserControlBase

    Public Enum BCWorkingMode
        Passport = 0
        SecurityGroup = 1
    End Enum

#Region "Propiedades"

    Public Property WorkMode() As BCWorkingMode
        Get
            Return Session("frmBusinessCenterSelector_BCWorkingMode")
        End Get

        Set(ByVal value As BCWorkingMode)
            Session("frmBusinessCenterSelector_BCWorkingMode") = value
        End Set
    End Property

    Public Property IDRelatedObject() As Integer
        Get
            Return Session("frmBusinessCenterSelector_IDRelatedObject")
        End Get

        Set(ByVal value As Integer)
            Session("frmBusinessCenterSelector_IDRelatedObject") = value
        End Set
    End Property

    Public Property LoadRelatedObjectBC() As Boolean
        Get
            Return Session("frmBusinessCenterSelector_LoadRelatedObjectBC")
        End Get

        Set(ByVal value As Boolean)
            Session("frmBusinessCenterSelector_LoadRelatedObjectBC") = value
        End Set
    End Property

    Public Property SaveRelatedObjectBC() As Boolean
        Get
            Return Session("frmBusinessCenterSelector_SavePassportBC")
        End Get

        Set(ByVal value As Boolean)
            Session("frmBusinessCenterSelector_SavePassportBC") = value
        End Set
    End Property

    Public Property SelectNodesInGrid() As Boolean
        Get
            Return Session("frmBusinessCenterSelector_SelectNodesInGrid")
        End Get

        Set(ByVal value As Boolean)
            Session("frmBusinessCenterSelector_SelectNodesInGrid") = value
        End Set
    End Property

    Public Property BusinessCentersTree As List(Of roBusinessCenter)
        Get
            Return Session("frmBusinessCenterSelector_BusinessCentersTree")
        End Get

        Set(ByVal value As List(Of roBusinessCenter))
            Session("frmBusinessCenterSelector_BusinessCentersTree") = value
        End Set
    End Property

    Public Property BusinessCentersGrid As List(Of roBusinessCenter)
        Get
            Return Session("frmBusinessCenterSelector_BusinessCentersGrid")
        End Get

        Set(ByVal value As List(Of roBusinessCenter))
            Session("frmBusinessCenterSelector_BusinessCentersGrid") = value
        End Set
    End Property

#End Region

#Region "Variables"

    ReadOnly _lstCriteria = New List(Of String())

#End Region

#Region "Eventos"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        CreateColumnsSelectedCenters()
        BindContainerControls()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            ClearDataSources()
            LoadCombos()
        End If
        lpTree.ContainerElementID = "loadPanelTree"
        lgGrid.ContainerElementID = "loadPanelGrid"
        lpSelector.ContainerElementID = "divSelectorBc"

    End Sub

    Protected Sub ASPxBusinessCentersCallbackPanelContenido_Callback(ByVal sender As Object, ByVal e As CallbackEventArgsBase) Handles ASPxBusinessCentersSelectorCallbackPanelContenido.Callback
        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Dim oParameters As New frmBusinessCenterSelectorCallbackRequest()
        oParameters = roJSONHelper.Deserialize(strParameter, oParameters.GetType())

        Select Case oParameters.Action
            Case "INIT"
                LoadBussinesCenters(True)
                ASPxBusinessCentersSelectorCallbackPanelContenido.JSProperties.Add("cp_RefreshScreen", "INIT")
            Case "LOADGRID"
                ' cuando pasan centros del arbol al grid
                AddBusinessCentersToGrid(oParameters.GridCenters, False)
                BindContainerControls()
            Case "FILTER"
                'cuando viene del botón filtrar
                LoadFilteredCenters(oParameters.Centerfilter)
                BindContainerControls()
                ClearFilterData()
                UnSelectNodes()
            Case "DELETEGRID"
                ' cuando borro todos los centros de coste del grid
                BusinessCentersGrid = New List(Of roBusinessCenter)
                BindContainerControls()
            Case "SAVEBC"
                ' guardar o enviar los datos de los centros de coste seleccionados
                SaveBusinessCenters()
                ClearAndReloadControlData(If(Me.SaveRelatedObjectBC, False, True))
                ClearFilterData()
            Case "CANCEL"
                ' cancelar
                ClearAndReloadControlData(If(Me.SaveRelatedObjectBC, False, True))
                ClearFilterData()
                ASPxBusinessCentersSelectorCallbackPanelContenido.JSProperties.Add("cp_RefreshScreen", "CANCEL")
        End Select
    End Sub

    Protected Sub GridCenters_RowDeleting(sender As Object, e As Data.ASPxDataDeletingEventArgs)
        Dim deleteBusinessCenter = BusinessCentersGrid.SingleOrDefault(Function(s) s.ID.Equals(e.Keys(GridCenters.KeyFieldName)))
        If (deleteBusinessCenter IsNot Nothing) Then
            e.Cancel = True
            BusinessCentersGrid.Remove(deleteBusinessCenter)
        End If
        BindContainerControls()
    End Sub

#End Region

#Region "Métodos Control"

    Public Sub SetPassport(idPassport As Integer, loadPassportBC As Boolean, savePassportBC As Boolean, selectNodesInGrid As Boolean, ByVal initData As Boolean)
        Dim oPassport As roPassport = API.UserAdminServiceMethods.GetPassport(Page, roTypes.Any2Integer(idPassport), LoadType.Passport)

        Me.IDRelatedObject = oPassport.ID

        Me.LoadRelatedObjectBC = loadPassportBC
        Me.SaveRelatedObjectBC = savePassportBC
        Me.SelectNodesInGrid = selectNodesInGrid

        Me.WorkMode = BCWorkingMode.Passport

        ClearDataSources()
        If initData Then LoadBussinesCenters(True)

    End Sub

    Public Sub SetSecurityGroup(idSecurityGroup As Integer, loadSecurityGroupBC As Boolean, saveSecurityGroupBC As Boolean, selectNodesInGrid As Boolean, ByVal initData As Boolean)
        Me.IDRelatedObject = idSecurityGroup

        Me.LoadRelatedObjectBC = loadSecurityGroupBC
        Me.SaveRelatedObjectBC = saveSecurityGroupBC
        Me.SelectNodesInGrid = selectNodesInGrid

        Me.WorkMode = BCWorkingMode.SecurityGroup

        ClearDataSources()

        If initData Then LoadBussinesCenters(True)

    End Sub

    Private Sub SaveBusinessCenters()
        Dim lstIntegerCenters = New List(Of Integer)
        If (BusinessCentersGrid IsNot Nothing) Then
            For Each businessCenter In BusinessCentersGrid
                lstIntegerCenters.Add(businessCenter.ID)
            Next
        End If
        If (SaveRelatedObjectBC) Then

            If Me.WorkMode = BCWorkingMode.Passport Then
                TasksServiceMethods.SaveBusinessCenterByPassport(Page, Me.IDRelatedObject, lstIntegerCenters.ToArray(), False)
            Else
                TasksServiceMethods.SaveBusinessCenterBySecurityGroup(Page, Me.IDRelatedObject, lstIntegerCenters.ToArray(), False)
            End If
            ASPxBusinessCentersSelectorCallbackPanelContenido.JSProperties.Add("cp_RefreshScreen", "CLOSEWITHSAVE")
        Else
            ASPxBusinessCentersSelectorCallbackPanelContenido.JSProperties.Add("cp_RefreshScreen", "CLOSECALLPARENT")
            ASPxBusinessCentersSelectorCallbackPanelContenido.JSProperties.Add("cp_BcValues", String.Join(",", lstIntegerCenters).Split(","))
        End If
    End Sub

#End Region

#Region "Métodos Arbol"

    Public Sub LoadTree(oCenters As List(Of roBusinessCenter))
        tltBusinessCenters.ClearNodes()
        tltBusinessCenters.DataSource = oCenters
        tltBusinessCenters.Caption = Language.Translate("BusinessCenterSelector.Names", DefaultScope)
        tltBusinessCenters.KeyFieldName = "ID"
        tltBusinessCenters.DataBind()
    End Sub

#End Region

#Region "Métodos Grid"

    Private Sub CreateColumnsSelectedCenters()

        Dim GridColumn As GridViewDataTextColumn
        Dim GridColumnCommand As GridViewCommandColumn

        Dim VisibleIndex As Integer = 0

        Me.GridCenters.Columns.Clear()
        Me.GridCenters.KeyFieldName = "ID"

        'Clave
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "ID"
        GridColumn.FieldName = "ID"
        GridColumn.ReadOnly = False
        GridColumn.Visible = False
        GridCenters.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = Me.Language.Translate("GridCenters.Column.Center", DefaultScope) '"Valor"
        GridColumn.FieldName = "Name"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.CellStyle.Wrap = DevExpress.Utils.DefaultBoolean.True
        GridColumn.ReadOnly = False
        GridColumn.Width = 80
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridCenters.Columns.Add(GridColumn)

        'Command buttons
        GridColumnCommand = New GridViewCommandColumn()
        GridColumnCommand.ButtonRenderMode = GridCommandButtonRenderMode.Image
        GridColumnCommand.ShowDeleteButton = True
        GridColumnCommand.ShowEditButton = False
        GridColumnCommand.Caption = " "
        GridColumnCommand.VisibleIndex = VisibleIndex
        GridColumnCommand.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCommand.Width = 16
        VisibleIndex = VisibleIndex + 1
        GridCenters.Columns.Add(GridColumnCommand)

        'GridCenters.SettingsPager.PageSize = 22
        GridCenters.Styles.Cell.Wrap = DevExpress.Utils.DefaultBoolean.True
        GridCenters.PageIndex = -1
        GridCenters.SettingsPager.Mode = GridViewPagerMode.ShowAllRecords
    End Sub

    Public Sub BindCentersGrid(lstParsedCenters As List(Of roBusinessCenter))
        GridCenters.DataSource = lstParsedCenters
        GridCenters.DataBind()
    End Sub

#End Region

#Region "Auxiliares"

    Private Sub LoadBussinesCenters(bolReload As Boolean)
        Dim oCenters = New List(Of roBusinessCenter)
        Dim oCentersAssigned = New List(Of roBusinessCenter)
        Dim tbBusinessCenter = New DataTable
        Dim tbBusinessCenterAssigned = New DataTable

        'si se han de mostrar los centros de coste en el grid busco el listado de centros de coste del passport y los cargo en el grid
        If (SelectNodesInGrid) Then
            If Me.WorkMode = BCWorkingMode.Passport Then
                tbBusinessCenter = TasksServiceMethods.GetBusinessCenterByPassportDataTable(Page, Me.IDRelatedObject, False)
            Else
                tbBusinessCenter = TasksServiceMethods.GetBusinessCenterBySecurityGroupDataTable(Page, Me.IDRelatedObject, False)
            End If

            Dim lstPassportBc = GetBusinessCenterFromDataTable(tbBusinessCenter)
            BusinessCentersGrid = lstPassportBc
            BindCentersGrid(lstPassportBc)
            LoadTree(New List(Of roBusinessCenter))
        Else
            If (BusinessCentersGrid IsNot Nothing AndAlso BusinessCentersGrid.Count > 0) Then
                BindCentersGrid(BusinessCentersGrid)
            Else
                BindCentersGrid(New List(Of roBusinessCenter))
            End If
            LoadTree(New List(Of roBusinessCenter))
        End If
    End Sub

    Private Sub ClearDataSources()
        GridCenters.DataSource = Nothing
        tltBusinessCenters.DataSource = Nothing
    End Sub

    Private Function GetBusinessCenterFromDataTable(oCentersDt As DataTable) As List(Of roBusinessCenter)
        Dim oCentersList As New List(Of roBusinessCenter)
        For Each oCenterRow In oCentersDt.Rows
            Dim oCenter = New roBusinessCenter
            oCenter.Name = roTypes.Any2String(oCenterRow("Name"))
            oCenter.ID = roTypes.Any2Integer(oCenterRow("ID"))
            oCenter.Field1 = roTypes.Any2String(oCenterRow("Field1"))
            oCenter.Field2 = roTypes.Any2String(oCenterRow("Field2"))
            oCenter.Field3 = roTypes.Any2String(oCenterRow("Field3"))
            oCenter.Field4 = roTypes.Any2String(oCenterRow("Field4"))
            oCenter.Field5 = roTypes.Any2String(oCenterRow("Field5"))
            oCentersList.Add(oCenter)
        Next
        Return oCentersList
    End Function

    Public Sub BindContainerControls()
        If (BusinessCentersGrid IsNot Nothing) Then
            BindCentersGrid(BusinessCentersGrid)
        Else
            BindCentersGrid(New List(Of roBusinessCenter))
        End If

        If (BusinessCentersTree IsNot Nothing) Then
            LoadTree(BusinessCentersTree)
        Else
            LoadTree(New List(Of roBusinessCenter))
        End If
    End Sub

    Public Sub AddBusinessCentersToGrid(gridCenters As String, externalSource As Boolean)
        Dim tbBusinessCenter = New DataTable
        Dim lstPassportCenters = New List(Of roBusinessCenter)
        If Not (String.IsNullOrEmpty(gridCenters)) Then
            Dim centersArray = gridCenters.Split(",")
            If (externalSource) Then
                If (LoadRelatedObjectBC) Then

                    If Me.WorkMode = BCWorkingMode.Passport Then
                        tbBusinessCenter = TasksServiceMethods.GetBusinessCenterByPassportDataTable(Page, Me.IDRelatedObject, False)
                    Else
                        tbBusinessCenter = TasksServiceMethods.GetBusinessCenterBySecurityGroupDataTable(Page, Me.IDRelatedObject, False)
                    End If
                    lstPassportCenters = GetBusinessCenterFromDataTable(tbBusinessCenter)
                Else
                    tbBusinessCenter = TasksServiceMethods.GetBusinessCenters(Page, False)
                    lstPassportCenters = GetBusinessCenterFromDataTable(tbBusinessCenter)
                End If
                BusinessCentersTree = lstPassportCenters
            End If

            For Each center In centersArray
                Dim newBusinessCenterGrid = BusinessCentersTree.SingleOrDefault(Function(bc) bc.ID.Equals(roTypes.Any2Integer(center)))
                If (newBusinessCenterGrid IsNot Nothing) Then
                    If (BusinessCentersGrid IsNot Nothing) Then
                        If Not (BusinessCentersGrid.Exists(Function(bc) bc.ID.Equals(newBusinessCenterGrid.ID))) Then
                            BusinessCentersGrid.Add(newBusinessCenterGrid)
                        End If
                    Else
                        BusinessCentersGrid = New List(Of roBusinessCenter)
                        BusinessCentersGrid.Add(newBusinessCenterGrid)
                    End If
                End If
            Next
            If (externalSource) Then BusinessCentersTree = Nothing
        End If
    End Sub

    Private Sub ClearAndReloadControlData(bolPreserveNodesInGrid As Boolean)
        Dim idBCPassport = Me.IDRelatedObject
        Dim loadpassport = Me.LoadRelatedObjectBC
        Dim savePassport = Me.SaveRelatedObjectBC
        Dim selectNodes = SelectNodesInGrid
        Dim nodesInGrid = BusinessCentersGrid
        ClearSessionVariables()
        If (bolPreserveNodesInGrid) Then BusinessCentersGrid = nodesInGrid
        BindContainerControls()
        If Me.WorkMode = BCWorkingMode.Passport Then
            SetPassport(idBCPassport, loadpassport, savePassport, selectNodes, False)
        Else
            SetSecurityGroup(idBCPassport, loadpassport, savePassport, selectNodes, False)
        End If

    End Sub

    Private Sub LoadCombos()
        Dim businessCenterFields As DataTable = API.UserFieldServiceMethods.GetBusinessCenterFields(Page, Types.TaskField)
        For Each dRowUf As DataRow In businessCenterFields.Rows
            cmbBCFieldsValues1.Items.Add(dRowUf("Name"), dRowUf("ID"))
        Next

        _lstCriteria.Add({"Criteria.None", " "})
        _lstCriteria.Add({"Criteria.Equal", "="})
        _lstCriteria.Add({"Criteria.Different", "<>"})
        _lstCriteria.Add({"Criteria.StartsWith", "*"})
        _lstCriteria.Add({"Criteria.Contains", "*X*"})

        For Each criteria As String() In _lstCriteria
            cmbBCCriteria1.Items.Add(Language.Translate(criteria(0), DefaultScope), criteria(1))
        Next
    End Sub

    Private Sub LoadFilteredCenters(oFilterParameter As CenterFilterCallbackRequest)
        Dim tbBusinessCenter = New DataTable
        Dim lstPassportCenters = New List(Of roBusinessCenter)

        'si ya he cargado los centros de coste del passport para popular el arbol

        If (LoadRelatedObjectBC) Then
            If Me.WorkMode = BCWorkingMode.Passport Then
                tbBusinessCenter = TasksServiceMethods.GetBusinessCenterByPassportDataTable(Page, Me.IDRelatedObject, False)
            Else
                tbBusinessCenter = TasksServiceMethods.GetBusinessCenterBySecurityGroupDataTable(Page, Me.IDRelatedObject, False)
            End If

            lstPassportCenters = GetBusinessCenterFromDataTable(tbBusinessCenter)
        Else
            tbBusinessCenter = TasksServiceMethods.GetBusinessCenters(Page, False)
            lstPassportCenters = GetBusinessCenterFromDataTable(tbBusinessCenter)
        End If

        'Si no hay filtros cargo todos
        If (String.IsNullOrEmpty(oFilterParameter.Value) AndAlso String.IsNullOrEmpty(oFilterParameter.CenterName)) Then
            BusinessCentersTree = lstPassportCenters
            'Si ha escrito un nombre para el filtro
        ElseIf Not (String.IsNullOrEmpty(oFilterParameter.CenterName)) Then
            'si ha puesto filtro de campo de la ficha
            If (Not String.IsNullOrEmpty(oFilterParameter.Field) AndAlso Not String.IsNullOrEmpty(oFilterParameter.Criteria) AndAlso Not String.IsNullOrEmpty(oFilterParameter.Value)) Then
                ' busco los centros de negocio en base al filtro del campo de la ficha
                Dim lstfieldcentersfiltered = GetCentersByCriteria(oFilterParameter, lstPassportCenters)
                ' si encontro algún centro con el filtro del campo, luego los filtro por el nombre
                If (lstfieldcentersfiltered IsNot Nothing AndAlso lstfieldcentersfiltered.Count > 0) Then
                    BusinessCentersTree = lstfieldcentersfiltered.Where(Function(fn) fn.Name.ToUpper.Contains(oFilterParameter.CenterName.ToUpper)).ToList()
                End If
            Else
                ' si no se ha escrito filtro de campo de la ficha, busco solo los que coincidan con el nombre
                BusinessCentersTree = lstPassportCenters.Where(Function(ce) ce.Name.ToUpper.Contains(oFilterParameter.CenterName.ToUpper)).ToList()
            End If
        Else
            ' si solo se ha ingresado filtro por el campo de la ficha
            BusinessCentersTree = GetCentersByCriteria(oFilterParameter, lstPassportCenters)
        End If
    End Sub

    Private Function GetCentersByCriteria(oFilter As CenterFilterCallbackRequest, lstCenters As List(Of roBusinessCenter)) As List(Of roBusinessCenter)
        Try
            Select Case oFilter.Criteria
                Case "="
                    Select Case oFilter.Field
                        Case "1"
                            Return lstCenters.Where(Function(fi) fi.Field1.ToUpper.Equals(oFilter.Value.ToUpper)).ToList
                        Case "2"
                            Return lstCenters.Where(Function(fi) fi.Field2.ToUpper.Equals(oFilter.Value.ToUpper)).ToList
                        Case "3"
                            Return lstCenters.Where(Function(fi) fi.Field3.ToUpper.Equals(oFilter.Value.ToUpper)).ToList
                        Case "4"
                            Return lstCenters.Where(Function(fi) fi.Field4.ToUpper.Equals(oFilter.Value.ToUpper)).ToList
                        Case "5"
                            Return lstCenters.Where(Function(fi) fi.Field5.ToUpper.Equals(oFilter.Value.ToUpper)).ToList
                    End Select
                Case ">"
                    Select Case oFilter.Field
                        Case "1"
                            Return lstCenters.Where(Function(fi) Integer.Parse(fi.Field1) > Integer.Parse(oFilter.Value)).ToList
                        Case "2"
                            Return lstCenters.Where(Function(fi) Integer.Parse(fi.Field2) > Integer.Parse(oFilter.Value)).ToList
                        Case "3"
                            Return lstCenters.Where(Function(fi) Integer.Parse(fi.Field3) > Integer.Parse(oFilter.Value)).ToList
                        Case "4"
                            Return lstCenters.Where(Function(fi) Integer.Parse(fi.Field4) > Integer.Parse(oFilter.Value)).ToList
                        Case "5"
                            Return lstCenters.Where(Function(fi) Integer.Parse(fi.Field5) > Integer.Parse(oFilter.Value)).ToList
                    End Select
                Case ">="
                    Select Case oFilter.Field
                        Case "1"
                            Return lstCenters.Where(Function(fi) Integer.Parse(fi.Field1) >= Integer.Parse(oFilter.Value)).ToList
                        Case "2"
                            Return lstCenters.Where(Function(fi) Integer.Parse(fi.Field2) >= Integer.Parse(oFilter.Value)).ToList
                        Case "3"
                            Return lstCenters.Where(Function(fi) Integer.Parse(fi.Field3) >= Integer.Parse(oFilter.Value)).ToList
                        Case "4"
                            Return lstCenters.Where(Function(fi) Integer.Parse(fi.Field4) >= Integer.Parse(oFilter.Value)).ToList
                        Case "5"
                            Return lstCenters.Where(Function(fi) Integer.Parse(fi.Field5) >= Integer.Parse(oFilter.Value)).ToList
                    End Select
                Case "<"
                    Select Case oFilter.Field
                        Case "1"
                            Return lstCenters.Where(Function(fi) Integer.Parse(fi.Field1) < Integer.Parse(oFilter.Value)).ToList
                        Case "2"
                            Return lstCenters.Where(Function(fi) Integer.Parse(fi.Field2) < Integer.Parse(oFilter.Value)).ToList
                        Case "3"
                            Return lstCenters.Where(Function(fi) Integer.Parse(fi.Field3) < Integer.Parse(oFilter.Value)).ToList
                        Case "4"
                            Return lstCenters.Where(Function(fi) Integer.Parse(fi.Field4) < Integer.Parse(oFilter.Value)).ToList
                        Case "5"
                            Return lstCenters.Where(Function(fi) Integer.Parse(fi.Field5) < Integer.Parse(oFilter.Value)).ToList
                    End Select
                Case "<="
                    Select Case oFilter.Field
                        Case "1"
                            Return lstCenters.Where(Function(fi) Integer.Parse(fi.Field1) <= Integer.Parse(oFilter.Value)).ToList
                        Case "2"
                            Return lstCenters.Where(Function(fi) Integer.Parse(fi.Field2) <= Integer.Parse(oFilter.Value)).ToList
                        Case "3"
                            Return lstCenters.Where(Function(fi) Integer.Parse(fi.Field3) <= Integer.Parse(oFilter.Value)).ToList
                        Case "4"
                            Return lstCenters.Where(Function(fi) Integer.Parse(fi.Field4) <= Integer.Parse(oFilter.Value)).ToList
                        Case "5"
                            Return lstCenters.Where(Function(fi) Integer.Parse(fi.Field5) <= Integer.Parse(oFilter.Value)).ToList
                    End Select
                Case "<>"
                    Select Case oFilter.Field
                        Case "1"
                            Return lstCenters.Where(Function(fi) Not fi.Field1.Equals(oFilter.Value)).ToList
                        Case "2"
                            Return lstCenters.Where(Function(fi) Not fi.Field2.Equals(oFilter.Value)).ToList
                        Case "3"
                            Return lstCenters.Where(Function(fi) Not fi.Field3.Equals(oFilter.Value)).ToList
                        Case "4"
                            Return lstCenters.Where(Function(fi) Not fi.Field4.Equals(oFilter.Value)).ToList
                        Case "5"
                            Return lstCenters.Where(Function(fi) Not fi.Field5.Equals(oFilter.Value)).ToList
                    End Select
                Case "*"
                    Select Case oFilter.Field
                        Case "1"
                            Return lstCenters.Where(Function(fi) fi.Field1.ToUpper.StartsWith(oFilter.Value.ToUpper)).ToList
                        Case "2"
                            Return lstCenters.Where(Function(fi) fi.Field2.ToUpper.StartsWith(oFilter.Value.ToUpper)).ToList
                        Case "3"
                            Return lstCenters.Where(Function(fi) fi.Field3.ToUpper.StartsWith(oFilter.Value.ToUpper)).ToList
                        Case "4"
                            Return lstCenters.Where(Function(fi) fi.Field4.ToUpper.StartsWith(oFilter.Value.ToUpper)).ToList
                        Case "5"
                            Return lstCenters.Where(Function(fi) fi.Field5.ToUpper.StartsWith(oFilter.Value.ToUpper)).ToList
                    End Select
                Case "*X*"
                    Select Case oFilter.Field
                        Case "1"
                            Return lstCenters.Where(Function(fi) fi.Field1.ToUpper.Contains(oFilter.Value.ToUpper)).ToList
                        Case "2"
                            Return lstCenters.Where(Function(fi) fi.Field2.ToUpper.Contains(oFilter.Value.ToUpper)).ToList
                        Case "3"
                            Return lstCenters.Where(Function(fi) fi.Field3.ToUpper.Contains(oFilter.Value.ToUpper)).ToList
                        Case "4"
                            Return lstCenters.Where(Function(fi) fi.Field4.ToUpper.Contains(oFilter.Value.ToUpper)).ToList
                        Case "5"
                            Return lstCenters.Where(Function(fi) fi.Field5.ToUpper.Contains(oFilter.Value.ToUpper)).ToList
                    End Select
            End Select
            Return Nothing
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Sub ClearSessionVariables()
        Session("frmBusinessCenterSelector_IDRelatedObject") = Nothing
        Session("frmBusinessCenterSelector_LoadRelatedObjectBC") = Nothing
        Session("frmBusinessCenterSelector_SavePassportBC") = Nothing
        Session("frmBusinessCenterSelector_SelectNodesInGrid") = Nothing
        Session("frmBusinessCenterSelector_BusinessCentersTree") = Nothing
        Session("frmBusinessCenterSelector_BusinessCentersGrid") = Nothing
    End Sub

    Private Sub ClearFilterData()
        txtCenterName.Text = String.Empty
        cmbBCFieldsValues1.SelectedIndex = -1
        cmbBCCriteria1.SelectedIndex = -1
        txtValue1.Text = String.Empty
    End Sub

    Private Sub UnSelectNodes()
        For Each node As TreeListNode In tltBusinessCenters.Nodes
            node.Selected = False
        Next
        chkCenters.Checked = False
    End Sub

#End Region

#Region "Clases Auxiliares"

    <Runtime.Serialization.DataContract()>
    Private Class frmBusinessCenterSelectorCallbackRequest

        <Runtime.Serialization.DataMember(Name:="action")>
        Public Action As String

        <Runtime.Serialization.DataMember(Name:="gridCenters")>
        Public GridCenters As String

        <Runtime.Serialization.DataMember(Name:="filter")>
        Public Centerfilter As CenterFilterCallbackRequest

    End Class

    <Runtime.Serialization.DataContract()>
    Private Class CenterFilterCallbackRequest

        <Runtime.Serialization.DataMember(Name:="centerName")>
        Public CenterName As String

        <Runtime.Serialization.DataMember(Name:="field")>
        Public Field As String

        <Runtime.Serialization.DataMember(Name:="criteria")>
        Public Criteria As String

        <Runtime.Serialization.DataMember(Name:="value")>
        Public Value As String

    End Class

#End Region

End Class