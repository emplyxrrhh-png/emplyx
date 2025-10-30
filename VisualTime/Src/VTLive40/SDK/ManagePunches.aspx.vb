Imports DevExpress.Web
Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class ManagePunches
    Inherits PageBase

    <Runtime.Serialization.DataContract()>
    Private Class ManagePunchesCallbackRequest

        <Runtime.Serialization.DataMember(Name:="aTab")>
        Public aTab As Integer

        <Runtime.Serialization.DataMember(Name:="ID")>
        Public ID As Integer

        <Runtime.Serialization.DataMember(Name:="action")>
        Public Action As String

        <Runtime.Serialization.DataMember(Name:="StampParam")>
        Public StampParam As Double

        <Runtime.Serialization.DataMember(Name:="resultClientAction")>
        Public resultClientAction As String

    End Class

    Private Property VTXFile() As DataTable
        Get
            Dim dtpunches As DataTable = Session("dtImportVtr")
            If dtpunches Is Nothing Then
                dtpunches = New DataTable()

                Session("dtImportVtr") = dtpunches
            End If

            Return dtpunches
        End Get
        Set(ByVal value As DataTable)
            If value IsNot Nothing Then
                Session("dtImportVtr") = value
            Else
                Session("dtImportVtr") = Nothing
            End If
        End Set
    End Property

    Private Property EmployeesData(Optional ByVal bolReload As Boolean = False) As DataView
        Get
            Dim tbCauses As DataTable = Session("SDK_Employees")
            Dim dv As DataView = Nothing
            If tbCauses IsNot Nothing Then
                dv = New DataView(tbCauses)
                dv.Sort = "EmployeeName ASC"
            End If

            If bolReload OrElse dv Is Nothing Then
                Dim tb As DataTable = API.EmployeeServiceMethods.GetAllEmployees(Me.Page, "", "Employees")
                If tb IsNot Nothing Then
                    dv = New DataView(tb)
                    dv.Sort = "EmployeeName ASC"

                    Session("SDK_Employees") = dv.Table
                End If
            End If

            Return dv
        End Get
        Set(ByVal value As DataView)
            If value IsNot Nothing Then
                Session("SDK_Employees") = value.Table
            Else
                Session("SDK_Employees") = Nothing
            End If
        End Set
    End Property

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("Ajax", "~/Base/Scripts/Ajax.js")

        Me.InsertExtraJavascript("roTabContainerClient", "~/Base/Scripts/roTabContainerClient.js")
        Me.InsertExtraJavascript("webUserForms", "~/Base/Scripts/webUserForms.js")

        Me.InsertExtraJavascript("roSDK", "~/SDK/Scripts/roSDK.js")
    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If HelperSession.GetFeatureIsInstalledFromApplication("Feature\SDK") = False Then
            WLHelperWeb.RedirectAccessDenied(True)
            Exit Sub
        End If

        If Not Me.IsPostBack Then
            EmployeesData = Nothing
            VTXFile = Nothing
        End If

        CreateImportPunchesColumns(GridPunchesToImport)
        GridPunchesToImport.DataSource = VTXFile
        GridPunchesToImport.DataBind()
    End Sub

    Protected Sub ASPxCallbackPanelContenido_Callback(ByVal sender As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase) Handles ASPxCallbackPanelContenido.Callback
        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Dim oParameters As New ManagePunchesCallbackRequest()
        oParameters = roJSONHelper.Deserialize(strParameter, oParameters.GetType())

        ASPxCallbackPanelContenido.JSProperties.Add("cpActionRO", oParameters.Action)

        Select Case oParameters.Action

            Case "uploadFileVTX"
                ASPxCallbackPanelContenido.JSProperties.Add("cpImportRowCount", VTXFile.Rows.Count)
                ASPxCallbackPanelContenido.JSProperties.Add("cpResultRO", "OK")
            Case "execute"
                Execute(oParameters)
        End Select

        Dim params As New Generic.List(Of String)
        params.Add(VTXFile.Rows.Count & "")
        lblCountToImport.Text = Me.Language.Translate("SDKPunches.PunchesCount", Me.DefaultScope, params)
    End Sub

    Private Sub Execute(ByVal oParameters As ManagePunchesCallbackRequest)
        Dim response As roSDKGenericResponse
        Dim ids As String = Session("idsToDelete")

        Dim dt As DataTable = VTXFile

        If dt IsNot Nothing Then
            Dim dv As DataView = dt.DefaultView

            'el VTR es un format de Robotics. EL cause a la mateixa exportacio de SDK de excel no s'informa, per tant aqui de moment tb l'ignorem
            Dim punches = New List(Of roSDKPunch)
            For Each p As DataRowView In dv
                Dim punch = New roSDKPunch
                punch.Terminal = p("Terminal")
                punch.IdEmployee = p("IdEmpleado")
                punch.DateTime = p("Fecha")
                punch.ShiftDate = DateTime.Parse(p("Fecha").ToString).Date 'shiftdate is the day of fecha without the hours
                punch.Type = p("PunchType")
                punch.Cause = roTypes.Any2String(p("Cause"))
                punches.Add(punch)
            Next
            response = SDKServiceMethods.ProcessPunches(Me.Page, punches, ids, True)
            If response.oState.Result <> SDKResultEnum.NoError Then
                ASPxCallbackPanelContenido.JSProperties.Add("cpResultRO", response.oState.ErrorText)
            Else
                VTXFile = Nothing
                ASPxCallbackPanelContenido.JSProperties.Add("cpResultRO", "OK")
            End If
        Else
            ASPxCallbackPanelContenido.JSProperties.Add("cpResultRO", Me.Language.Translate("SDKPunches.NoFileSelected", Me.DefaultScope))
        End If

    End Sub

    Private Sub GridSettings(ByVal grid As ASPxGridView)
        grid.Settings.ShowFilterRow = True
        grid.Settings.ShowFilterRowMenu = True
        grid.SettingsPager.Mode = GridViewPagerMode.ShowPager
        grid.SettingsPager.PageSize = 15
        grid.SettingsBehavior.FilterRowMode = GridViewFilterRowMode.Auto
    End Sub

    Private Sub CreateImportPunchesColumns(ByVal grid As ASPxGridView)
        GridSettings(grid)

        grid.Columns.Clear()
        grid.KeyFieldName = "Fecha;IdEmpleado"

        Dim GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("Grid.Column.EmployeeID", DefaultScope) ' "ID Empleado"
        GridColumn.FieldName = "IdEmpleado"
        GridColumn.ReadOnly = True
        GridColumn.Visible = True
        GridColumn.Width = 20
        grid.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("Grid.Column.EmployeeName", DefaultScope) ' "Empleado"
        GridColumn.FieldName = "EmployeeNameUnbound"
        GridColumn.ReadOnly = True
        GridColumn.Width = 40
        GridColumn.UnboundType = DevExpress.Data.UnboundColumnType.String
        grid.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("Grid.Column.DateTime", DefaultScope) ' "Fecha"
        GridColumn.FieldName = "Fecha"
        GridColumn.ReadOnly = True
        GridColumn.Width = 20
        grid.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("Grid.Column.Type", DefaultScope) ' "Tipo"
        GridColumn.FieldName = "TypeUnbound"
        GridColumn.ReadOnly = True
        GridColumn.Width = 20
        GridColumn.UnboundType = DevExpress.Data.UnboundColumnType.String
        grid.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("Grid.Column.Terminal", DefaultScope) '"Terminal"
        GridColumn.FieldName = "Terminal"
        GridColumn.ReadOnly = True
        GridColumn.Visible = True
        GridColumn.Width = 20
        grid.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("Grid.Column.Cause", DefaultScope) '"Justificación"
        GridColumn.FieldName = "Cause"
        GridColumn.ReadOnly = True
        GridColumn.Visible = True
        GridColumn.Width = 10
        grid.Columns.Add(GridColumn)
    End Sub

    Protected Sub GridPunchesToImport_CustomUnboundColumnData(sender As Object, e As ASPxGridViewColumnDataEventArgs) Handles GridPunchesToImport.CustomUnboundColumnData

        Select Case e.Column.FieldName
            Case "TypeUnbound"
                If e.IsGetData Then
                    If e.GetListSourceFieldValue("PunchType") = "E" Then
                        e.Value = Me.Language.Translate("SDKPunches.Input", Me.DefaultScope)
                    Else
                        e.Value = Me.Language.Translate("SDKPunches.Output", Me.DefaultScope)

                    End If
                End If
            Case "EmployeeNameUnbound"
                If e.IsGetData Then
                    Dim EmployeeList As New DataView

                    EmployeeList = EmployeesData
                    EmployeeList.RowFilter = "IdEmployee = " & e.GetListSourceFieldValue("IdEmpleado")

                    If EmployeeList IsNot Nothing AndAlso EmployeeList.Count > 0 Then
                        e.Value = EmployeeList(0)("EmployeeName").ToString
                    End If
                End If
        End Select

    End Sub

End Class