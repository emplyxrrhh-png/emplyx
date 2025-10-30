Imports DevExpress.Web
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.VTBase
Imports Robotics.Web.Base

Public Class PartesTrabajo
    Inherits PageBase

#Region "Events"

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("Ajax", "~/Base/Scripts/Ajax.js")
        Me.InsertExtraJavascript("GenericData", "~/Base/Scripts/GenericData.js")

        Me.InsertExtraJavascript("Task", "~/Tasks/Scripts/PartesTrabajo.js")

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Not HelperSession.GetFeatureIsInstalledFromApplication("Feature\Productiv") Then
            WLHelperWeb.RedirectAccessDenied(False)
            Return
        End If


        If Not Me.IsPostBack AndAlso Not Me.IsCallback Then
            Dim DateEnd As Date = Now
            Dim DateBeg As Date = DateEnd.AddDays(-7)

            ' Fecha inicial y final
            Me.txtBeginDate.Value = DateBeg
            Me.txtEndDate.Value = DateEnd

            ' Carga empleados
            LoadEmployeesCombo(True)

            ' Crea las columnas del grid
            CreateColumnsWoorkSheets()

            ' Obtiene datos
            GetWorkSheetsData(True)
        Else
            LoadEmployeesCombo(False)
            GetWorkSheetsData(False)
        End If
    End Sub

#End Region

#Region "Methods"
    Private Function LoadEmployeesCombo(Optional ByVal bolReload As Boolean = False) As DataView

        Dim tb As DataTable = Session("Partes_PartesEmployeesNames")
        Dim dv As DataView = Nothing

        If bolReload OrElse tb Is Nothing Then
            tb = API.EmployeeServiceMethods.GetEmployees(Me.Page, "")
            If tb IsNot Nothing Then
                Session("Partes_PartesEmployeesNames") = tb
                dv = New DataView(tb)
            End If
            Return dv
        Else
            dv = New DataView(tb)
            Return dv
        End If
    End Function

    Private Sub CreateColumnsWoorkSheets()

        Dim GridColumn As GridViewDataTextColumn
        Dim GridColumnCombo As GridViewDataComboBoxColumn
        Dim GridColumnDate As GridViewDataDateColumn
        Dim GridColumnCheck As GridViewDataCheckColumn

        Me.LinqPartesDataSource.TableName = "ESP_PARTESTRABAJO"
        Me.LinqPartesDataSource.ContextTypeName = "PartesTrabajo.PartesTrabajoLINQDataContext"
        Me.LinqPartesDataSource.ContextTypeName = "PartesTrabajo.PartesTrabajoLINQDataContext, PartesTrabajo, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"

        Dim VisibleIndex As Integer = 0

        Me.grdPartes.Settings.ShowTitlePanel = True
        Me.grdPartes.Columns.Clear()
        Me.grdPartes.KeyFieldName = "IDEmpleado;Fecha;Parte"
        Me.grdPartes.SettingsText.EmptyDataRow = " "
        Me.grdPartes.Settings.VerticalScrollBarMode = ScrollBarMode.Auto
        Me.grdPartes.EnableRowsCache = False
        Me.grdPartes.SettingsEditing.Mode = GridViewEditingMode.PopupEditForm
        Me.grdPartes.SettingsPager.PageSize = 24
        Me.grdPartes.SettingsBehavior.ConfirmDelete = True

        ' Columna edición
        Dim GridColumnCommand As GridViewCommandColumn
        GridColumnCommand = New GridViewCommandColumn()
        GridColumnCommand.ButtonRenderMode = GridCommandButtonRenderMode.Image
        GridColumnCommand.ShowDeleteButton = False
        GridColumnCommand.ShowEditButton = True
        GridColumnCommand.ShowCancelButton = True
        GridColumnCommand.ShowUpdateButton = True
        GridColumnCommand.Caption = " "
        GridColumnCommand.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnCommand.VisibleIndex = VisibleIndex
        GridColumnCommand.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCommand.Width = 35
        Me.grdPartes.Columns.Add(GridColumnCommand)

        'Empleado
        GridColumnCombo = New GridViewDataComboBoxColumn()
        GridColumnCombo.Caption = "Empleado"
        GridColumnCombo.FieldName = "IDEmpleado"
        GridColumnCombo.VisibleIndex = VisibleIndex
        GridColumnCombo.PropertiesComboBox.DataSource = LoadEmployeesCombo()
        GridColumnCombo.PropertiesComboBox.TextField = "EmployeeName"
        GridColumnCombo.PropertiesComboBox.ValueField = "IDEmployee"
        GridColumnCombo.PropertiesComboBox.ValueType = GetType(Integer)
        GridColumnCombo.PropertiesComboBox.RequireDataBinding()

        GridColumnCombo.PropertiesComboBox.ClearButton.DisplayMode = ClearButtonDisplayMode.Always
        VisibleIndex = VisibleIndex + 1
        GridColumnCombo.ReadOnly = True
        GridColumnCombo.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnCombo.CellStyle.VerticalAlign = VerticalAlign.Top
        GridColumnCombo.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnCombo.Width = 200
        GridColumnCombo.Settings.SortMode = DevExpress.XtraGrid.ColumnSortMode.DisplayText
        GridColumnCombo.SortIndex = 0
        GridColumnCombo.SortAscending()

        Me.grdPartes.Columns.Add(GridColumnCombo)

        'Fecha
        GridColumnDate = New GridViewDataDateColumn()
        GridColumnDate.Caption = "Fecha"
        GridColumnDate.FieldName = "Fecha"
        GridColumnDate.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnDate.ReadOnly = True
        GridColumnDate.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnDate.CellStyle.VerticalAlign = VerticalAlign.Top
        GridColumnDate.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnDate.Width = 100
        GridColumnDate.PropertiesDateEdit.DisplayFormatString = "dd/MM/yyyy"
        GridColumnDate.SortIndex = 1
        GridColumnDate.SortDescending()
        Me.grdPartes.Columns.Add(GridColumnDate)

        'Parte
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "Parte"
        GridColumn.FieldName = "Parte"
        GridColumn.VisibleIndex = VisibleIndex
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.CellStyle.VerticalAlign = VerticalAlign.Top
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.Width = 60
        Me.grdPartes.Columns.Add(GridColumn)

        'Proyecto
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "Proyecto"
        GridColumn.FieldName = "Proyecto"
        GridColumn.VisibleIndex = VisibleIndex
        GridColumn.ReadOnly = False
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.CellStyle.VerticalAlign = VerticalAlign.Top
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.Width = 80
        Me.grdPartes.Columns.Add(GridColumn)

        'Cliente
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "Cliente"
        GridColumn.FieldName = "Cliente"
        GridColumn.VisibleIndex = VisibleIndex
        GridColumn.ReadOnly = False
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.CellStyle.VerticalAlign = VerticalAlign.Top
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        'GridColumn.Width = 35
        Me.grdPartes.Columns.Add(GridColumn)


        'Kms
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "KMs"
        GridColumn.FieldName = "Kms"
        GridColumn.VisibleIndex = VisibleIndex
        GridColumn.ReadOnly = False
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.CellStyle.VerticalAlign = VerticalAlign.Top
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.PropertiesEdit.DisplayFormatString = "0.0;-0.0;''"
        GridColumn.PropertiesTextEdit.MaskSettings.Mask = "<0..9999>"
        GridColumn.Width = 60
        Me.grdPartes.Columns.Add(GridColumn)

        'Horas Normales
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "H.N."
        GridColumn.FieldName = "HorasNormales"
        GridColumn.VisibleIndex = VisibleIndex
        GridColumn.ReadOnly = False
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.CellStyle.VerticalAlign = VerticalAlign.Top
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.PropertiesEdit.DisplayFormatString = "0.00;-0.00;''"
        GridColumn.ToolTip = "Horas Normales"
        GridColumn.PropertiesTextEdit.MaskSettings.Mask = "<0..23>.<00..99>"
        GridColumn.Width = 60
        Me.grdPartes.Columns.Add(GridColumn)

        'Horas Extras Normales
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "H.E.N."
        GridColumn.FieldName = "HorasExtrasNormales"
        GridColumn.VisibleIndex = VisibleIndex
        GridColumn.ReadOnly = False
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.CellStyle.VerticalAlign = VerticalAlign.Top
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.PropertiesEdit.DisplayFormatString = "0.00;-0.00;''"
        GridColumn.ToolTip = "Horas Extras Normales"
        GridColumn.PropertiesTextEdit.MaskSettings.Mask = "<0..23>.<00..99>"
        GridColumn.Width = 60
        Me.grdPartes.Columns.Add(GridColumn)

        'Horas Extras Sábados
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "H.E.S."
        GridColumn.FieldName = "HorasExtrasSabados"
        GridColumn.VisibleIndex = VisibleIndex
        GridColumn.ReadOnly = False
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.CellStyle.VerticalAlign = VerticalAlign.Top
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.PropertiesEdit.DisplayFormatString = "0.00;-0.00;''"
        GridColumn.ToolTip = "Horas Extras Sábados"
        GridColumn.PropertiesTextEdit.MaskSettings.Mask = "<0..23>.<00..99>"
        GridColumn.Width = 60
        Me.grdPartes.Columns.Add(GridColumn)

        'Horas Extras Festivo
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "H.E.F."
        GridColumn.FieldName = "HorasExtrasFestivo"
        GridColumn.VisibleIndex = VisibleIndex
        GridColumn.ReadOnly = False
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.CellStyle.VerticalAlign = VerticalAlign.Top
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.PropertiesEdit.DisplayFormatString = "0.00;-0.00;''"
        GridColumn.ToolTip = "Horas Extras Festivos"
        GridColumn.PropertiesTextEdit.MaskSettings.Mask = "<0..23>.<00..99>"
        GridColumn.Width = 60
        Me.grdPartes.Columns.Add(GridColumn)

        'Dietas Primera
        GridColumnCheck = New GridViewDataCheckColumn()
        GridColumnCheck.Caption = "D.P."
        GridColumnCheck.FieldName = "DietasPrimera"
        GridColumnCheck.VisibleIndex = VisibleIndex
        GridColumnCheck.ReadOnly = False
        GridColumnCheck.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCheck.CellStyle.VerticalAlign = VerticalAlign.Top
        GridColumnCheck.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCheck.ToolTip = "Dietas Primera"
        GridColumnCheck.Width = 60
        Me.grdPartes.Columns.Add(GridColumnCheck)

        'Dietas Media
        GridColumnCheck = New GridViewDataCheckColumn()
        GridColumnCheck.Caption = "D.M."
        GridColumnCheck.FieldName = "DietasMedia"
        GridColumnCheck.VisibleIndex = VisibleIndex
        GridColumnCheck.ReadOnly = False
        GridColumnCheck.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCheck.CellStyle.VerticalAlign = VerticalAlign.Top
        GridColumnCheck.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCheck.ToolTip = "Dietas Media"
        GridColumnCheck.Width = 60
        Me.grdPartes.Columns.Add(GridColumnCheck)

        'Dietas Comida
        GridColumnCheck = New GridViewDataCheckColumn()
        GridColumnCheck.Caption = "D.C."
        GridColumnCheck.FieldName = "DietasComida"
        GridColumnCheck.VisibleIndex = VisibleIndex
        GridColumnCheck.ReadOnly = False
        GridColumnCheck.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCheck.CellStyle.VerticalAlign = VerticalAlign.Top
        GridColumnCheck.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCheck.ToolTip = "Dietas Comida"
        GridColumnCheck.Width = 60
        Me.grdPartes.Columns.Add(GridColumnCheck)

        'Dietas Plus Altura
        GridColumnCheck = New GridViewDataCheckColumn()
        GridColumnCheck.Caption = "P.A."
        GridColumnCheck.FieldName = "DietasPlusAltura"
        GridColumnCheck.VisibleIndex = VisibleIndex
        GridColumnCheck.ReadOnly = False
        GridColumnCheck.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCheck.CellStyle.VerticalAlign = VerticalAlign.Top
        GridColumnCheck.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCheck.ToolTip = "Plus Altura"
        GridColumnCheck.Width = 60
        Me.grdPartes.Columns.Add(GridColumnCheck)

        'Dietas Trans
        GridColumnCheck = New GridViewDataCheckColumn()
        GridColumnCheck.Caption = "D.T."
        GridColumnCheck.FieldName = "DietasTrans"
        GridColumnCheck.VisibleIndex = VisibleIndex
        GridColumnCheck.ReadOnly = False
        GridColumnCheck.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCheck.CellStyle.VerticalAlign = VerticalAlign.Top
        GridColumnCheck.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCheck.ToolTip = "Dietas Transporte"
        GridColumnCheck.Width = 60
        Me.grdPartes.Columns.Add(GridColumnCheck)


        'Trabajo 1
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "Trabajo 1"
        GridColumn.FieldName = "Trabajo1"
        GridColumn.VisibleIndex = VisibleIndex
        GridColumn.ReadOnly = False
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.CellStyle.VerticalAlign = VerticalAlign.Top
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        'GridColumn.Width = 35
        Me.grdPartes.Columns.Add(GridColumn)

        'Trabajo 2
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "Trabajo 2"
        GridColumn.FieldName = "Trabajo2"
        GridColumn.VisibleIndex = VisibleIndex
        GridColumn.ReadOnly = False
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.CellStyle.VerticalAlign = VerticalAlign.Top
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        'GridColumn.Width = 35
        Me.grdPartes.Columns.Add(GridColumn)

        'Trabajo 3
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "Trabajo 3"
        GridColumn.FieldName = "Trabajo3"
        GridColumn.VisibleIndex = VisibleIndex
        GridColumn.ReadOnly = False
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.CellStyle.VerticalAlign = VerticalAlign.Top
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        'GridColumn.Width = 35
        Me.grdPartes.Columns.Add(GridColumn)

        'Trabajo 4
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "Trabajo 4"
        GridColumn.FieldName = "Trabajo4"
        GridColumn.VisibleIndex = VisibleIndex
        GridColumn.ReadOnly = False
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.CellStyle.VerticalAlign = VerticalAlign.Top
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        'GridColumn.Width = 35
        Me.grdPartes.Columns.Add(GridColumn)

        ' Columna borrar
        GridColumnCommand = New GridViewCommandColumn()
        GridColumnCommand.ButtonRenderMode = GridCommandButtonRenderMode.Image
        GridColumnCommand.ShowDeleteButton = True
        GridColumnCommand.ShowEditButton = False
        GridColumnCommand.ShowCancelButton = False
        GridColumnCommand.ShowUpdateButton = False
        GridColumnCommand.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnCommand.Caption = " "
        GridColumnCommand.VisibleIndex = VisibleIndex
        GridColumnCommand.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCommand.Width = 35
        Me.grdPartes.Columns.Add(GridColumnCommand)
    End Sub

    Protected Sub CallbackSession_Callback(ByVal source As Object, ByVal e As DevExpress.Web.CallbackEventArgs) Handles CallbackSession.Callback
        e.Result = String.Empty

        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Select Case strParameter.Trim.ToUpperInvariant
            Case "REFRESHGRID"
                If Me.txtBeginDate.Value IsNot Nothing AndAlso Me.txtEndDate.Value IsNot Nothing Then
                    ' Comprueba fechas
                    If Me.txtBeginDate.Value > Me.txtEndDate.Value Then
                        DevExpress.XtraEditors.XtraMessageBox.Show("El intervalo de fechas no es correcto", "Comprobación de fechas", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation)
                    Else
                        ' Selecciona datos
                        CreateColumnsWoorkSheets()
                        GetWorkSheetsData(True)
                    End If
                End If
        End Select

    End Sub

    Protected Sub LinqPartesDataSource_Selecting(ByVal sender As Object, ByVal e As DevExpress.Data.Linq.LinqServerModeDataSourceSelectEventArgs) Handles LinqPartesDataSource.Selecting
        'enlazar con datatable en runtime http://www.devexpress.com/Support/Center/p/E168.aspx
        If GetWorkSheetsData() IsNot Nothing Then
            e.QueryableSource = GetWorkSheetsData()
        End If
    End Sub

    Private Function GetWorkSheetsData(Optional ByVal bolReload As Boolean = False) As Object
        Dim oCombo As GridViewDataComboBoxColumn = grdPartes.Columns("IDEmpleado")
        If oCombo IsNot Nothing Then
            oCombo.PropertiesComboBox.DataSource = Me.LoadEmployeesCombo()
            oCombo.PropertiesComboBox.TextField = "EmployeeName"
            oCombo.PropertiesComboBox.ValueField = "IDEmployee"
            oCombo.PropertiesComboBox.ValueType = GetType(Short)
        End If

        Dim qry As IQueryable(Of ESP_PARTESTRABAJO) = Session("PartesLiveFilter_PartesLiveDataTable")
        Dim LastDateInfUsedZZ As Nullable(Of DateTime) = Session("PartesLiveFilter_LastDateInfUsed")
        Dim LastDateSupUsedZZ As Nullable(Of DateTime) = Session("PartesLiveFilter_LastDateSupUsed")

        If bolReload Then
            Dim strConn As String = API.UserAdminServiceMethods.GetConnectionString(Me, WLHelperWeb.CurrentPassport().ID)
            Dim ContextDB As PartesTrabajoLINQDataContext = New PartesTrabajoLINQDataContext(strConn)
            Dim xDateBeg As Date = CDate(Me.txtBeginDate.Value).Date
            Dim xDateEnd As Date = CDate(Me.txtEndDate.Value).Date

            ContextDB.CommandTimeout = 0

            qry = ContextDB.ESP_PARTESTRABAJOs

            qry = qry.Where(Function(a) (a.Fecha >= xDateBeg AndAlso a.Fecha <= xDateEnd))

            Session("PartesLiveFilter_PartesLiveDataTable") = qry
            Session("PartesLiveFilter_LastDateInfUsed") = xDateBeg
            Session("PartesLiveFilter_LastDateSupUsed") = xDateEnd
        End If

        Return qry
    End Function
#End Region

    Private Sub grdPartes_DataBinding(sender As Object, e As EventArgs) Handles grdPartes.DataBinding
        ' Obtiene datos
        GetWorkSheetsData()
    End Sub

    Private Sub grdPartes_RowDeleting(sender As Object, e As Data.ASPxDataDeletingEventArgs) Handles grdPartes.RowDeleting
        Dim strConn As String = API.UserAdminServiceMethods.GetConnectionString(Me, WLHelperWeb.CurrentPassport().ID)
        Dim ContextDB As PartesTrabajoLINQDataContext = New PartesTrabajoLINQDataContext(strConn)
        ContextDB.CommandTimeout = 0

        Dim lstPartes As Generic.List(Of ESP_PARTESTRABAJO) = ContextDB.ESP_PARTESTRABAJOs.Where(Function(a) (a.IDEmpleado = roTypes.Any2Integer(e.Values("IDEmpleado")) AndAlso a.Fecha = roTypes.Any2DateTime(e.Values("Fecha")) AndAlso a.Parte = roTypes.Any2Double(e.Values("Parte")))).ToList()

        If lstPartes.Count = 1 Then
            ContextDB.ESP_PARTESTRABAJOs.DeleteOnSubmit(lstPartes(0))
            Dim oVSLsystem As New VSL.VSLSystem

            oVSLsystem.VSL_UpdateDailyCauses(Nothing, Nothing, roTypes.Any2String(e.Values("IDEmpleado")), roTypes.Any2DateTime(e.Values("Fecha")),
                                             0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, New roDataLinkState())

            oVSLsystem.VSL_UpdateDailySchedule(Nothing, roTypes.Any2String(e.Values("IDEmpleado")), roTypes.Any2DateTime(e.Values("Fecha")), New roDataLinkState())

            oVSLsystem.VSL_InitCalcProcess()
        End If


        ContextDB.SubmitChanges()

        e.Cancel = True
        'grdPartes.CancelEdit()

        GetWorkSheetsData(True)
    End Sub

    Private Sub grdPartes_RowUpdating(sender As Object, e As Data.ASPxDataUpdatingEventArgs) Handles grdPartes.RowUpdating
        Dim strConn As String = API.UserAdminServiceMethods.GetConnectionString(Me, WLHelperWeb.CurrentPassport().ID)
        Dim ContextDB As PartesTrabajoLINQDataContext = New PartesTrabajoLINQDataContext(strConn)
        ContextDB.CommandTimeout = 0

        If IsNothing(e.OldValues("Fecha")) Then
            e.Cancel = True
            Exit Sub
        End If

        Dim lstPartes As Generic.List(Of ESP_PARTESTRABAJO) = ContextDB.ESP_PARTESTRABAJOs.Where(Function(a) (a.IDEmpleado = roTypes.Any2Integer(e.OldValues("IDEmpleado")) AndAlso a.Fecha = roTypes.Any2DateTime(e.OldValues("Fecha")) AndAlso a.Parte = roTypes.Any2Double(e.OldValues("Parte")))).ToList()

        For Each parte As ESP_PARTESTRABAJO In lstPartes
            parte.Proyecto = e.NewValues("Proyecto")
            parte.BolsaHEFestivas = e.NewValues("BolsaHEFestivas")
            parte.BolsaHENormales = e.NewValues("BolsaHENormales")
            parte.Cliente = e.NewValues("Cliente")
            parte.DietasComida = e.NewValues("DietasComida")
            parte.DietasMedia = e.NewValues("DietasMedia")
            parte.DietasPlusAltura = e.NewValues("DietasPlusAltura")
            parte.DietasPrimera = e.NewValues("DietasPrimera")
            parte.DietasTrans = e.NewValues("DietasTrans")
            parte.HorasExtrasFestivo = e.NewValues("HorasExtrasFestivo")
            parte.HorasExtrasNormales = e.NewValues("HorasExtrasNormales")
            parte.HorasExtrasSabados = e.NewValues("HorasExtrasSabados")
            parte.HorasNormales = e.NewValues("HorasNormales")
            parte.Kms = e.NewValues("Kms")
            parte.Trabajo1 = e.NewValues("Trabajo1")
            parte.Trabajo2 = e.NewValues("Trabajo2")
            parte.Trabajo3 = e.NewValues("Trabajo3")
            parte.Trabajo4 = e.NewValues("Trabajo4")


            Dim oVSLsystem As New VSL.VSLSystem
            oVSLsystem.VSL_UpdateDailyCauses(Nothing, Nothing, roTypes.Any2String(e.OldValues("IDEmpleado")), roTypes.Any2DateTime(e.OldValues("Fecha")),
                                             IIf(parte.HorasNormales Is Nothing, 0, parte.HorasNormales), IIf(parte.HorasExtrasNormales Is Nothing, 0, parte.HorasExtrasNormales),
                                             IIf(parte.HorasExtrasSabados Is Nothing, 0, parte.HorasExtrasSabados),
                                             IIf(parte.HorasExtrasFestivo Is Nothing, 0, parte.HorasExtrasFestivo), IIf(parte.BolsaHENormales Is Nothing, 0, parte.BolsaHENormales),
                                             IIf(parte.BolsaHEFestivas Is Nothing, 0, parte.BolsaHEFestivas), IIf(parte.DietasPrimera Is Nothing, 0, parte.DietasPrimera),
                                             IIf(parte.DietasMedia Is Nothing, 0, parte.DietasMedia), IIf(parte.DietasComida Is Nothing, 0, parte.DietasComida),
                                             IIf(parte.DietasPlusAltura Is Nothing, 0, parte.DietasPlusAltura), IIf(parte.DietasTrans Is Nothing, 0, parte.DietasTrans),
                                             New roDataLinkState())

            oVSLsystem.VSL_UpdateDailySchedule(Nothing, roTypes.Any2String(e.OldValues("IDEmpleado")), roTypes.Any2DateTime(e.OldValues("Fecha")), New roDataLinkState())

            oVSLsystem.VSL_InitCalcProcess()
        Next

        ContextDB.SubmitChanges()

        e.Cancel = True
        grdPartes.CancelEdit()

        GetWorkSheetsData(True)
    End Sub
End Class