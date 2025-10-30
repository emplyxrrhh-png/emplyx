Imports DevExpress.Web
Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class Status
    Inherits PageBase

    Public Class DiagnosticsCallbackRequest
        Public Property Method As String
        Public Property Parameters As String
    End Class

#Region "Propiedades datatables grids"

    Private Property VisualTimeValidationCode(Optional ByVal bolReload As Boolean = False) As String
        Get
            Dim tbProcs As String = Session("Diagnostics_VisualTimeValidationCode")

            If tbProcs Is Nothing Then
                Session("Diagnostics_VisualTimeValidationCode") = ""
                tbProcs = ""
            End If
            Return tbProcs
        End Get
        Set(value As String)
            Session("Diagnostics_VisualTimeValidationCode") = value
        End Set
    End Property

    Private Property SQLConnectionActive(Optional ByVal bolReload As Boolean = False) As Boolean
        Get
            Dim tbProcs As Object = Session("Diagnostics_SQLConnectionActive")

            If tbProcs Is Nothing Then
                Session("Diagnostics_SQLConnectionActive") = True
                tbProcs = True
            End If
            Return tbProcs
        End Get
        Set(value As Boolean)
            Session("Diagnostics_SQLConnectionActive") = value
        End Set
    End Property

    Private Property VisualTimeProcess(Optional ByVal bolReload As Boolean = False) As DataTable
        Get
            Dim tbProcs As DataTable = Session("Diagnostics_VisualTimeProcess")

            If bolReload OrElse tbProcs Is Nothing Then
                If API.DiagMTServiceMethods.GetVisualTimeProcsInfo(tbProcs) Then

                    If tbProcs IsNot Nothing Then
                        tbProcs.PrimaryKey = New DataColumn() {tbProcs.Columns("ProcessName")}
                        tbProcs.AcceptChanges()
                    End If
                End If
                Session("Diagnostics_VisualTimeProcess") = tbProcs

            End If
            Return tbProcs
        End Get
        Set(value As DataTable)
            Session("Diagnostics_VisualTimeProcess") = value
        End Set
    End Property

    Private Property VisualTimeTasks(Optional ByVal bolReload As Boolean = False) As DataTable
        Get
            Dim tbTasks As DataTable = Session("Diagnostics_VisualTimeTasks")

            If bolReload OrElse tbTasks Is Nothing Then
                If API.DiagMTServiceMethods.GetUserTaskStatus(tbTasks) Then
                    If tbTasks IsNot Nothing Then
                        tbTasks.PrimaryKey = New DataColumn() {tbTasks.Columns("ID")}
                        tbTasks.AcceptChanges()
                    End If
                End If

                Session("Diagnostics_VisualTimeTasks") = tbTasks
            End If
            Return tbTasks
        End Get
        Set(value As DataTable)
            Session("Diagnostics_VisualTimeTasks") = value
        End Set
    End Property

    Private Property VisualTimeTerminals(Optional ByVal bolReload As Boolean = False) As DataTable
        Get
            Dim tbProcs As DataTable = Session("Diagnostics_VisualTimeTerminals")

            If bolReload OrElse tbProcs Is Nothing Then
                If API.DiagMTServiceMethods.GetTerminalsStatus(tbProcs) Then

                    If tbProcs IsNot Nothing Then
                        tbProcs.PrimaryKey = New DataColumn() {tbProcs.Columns("ID")}
                        tbProcs.AcceptChanges()
                    End If
                End If
                Session("Diagnostics_VisualTimeTerminals") = tbProcs

            End If
            Return tbProcs
        End Get
        Set(value As DataTable)
            Session("Diagnostics_VisualTimeTerminals") = value
        End Set
    End Property

    Private Property VisualTimeBackgroundTasks(Optional ByVal bolReload As Boolean = False) As DataTable
        Get
            Dim tbProcs As DataTable = Session("Diagnostics_VisualTimeBackground")

            If bolReload OrElse tbProcs Is Nothing Then
                If API.DiagMTServiceMethods.GetBackgroundTasksStatus(tbProcs) Then

                    If tbProcs IsNot Nothing Then
                        tbProcs.Columns.Add("InternalID", GetType(Integer))

                        Dim iIndex As Integer = 0
                        For Each oRow As DataRow In tbProcs.Rows
                            oRow("InternalID") = iIndex
                            iIndex = iIndex + 1
                        Next

                        tbProcs.PrimaryKey = New DataColumn() {tbProcs.Columns("InternalID")}
                        tbProcs.AcceptChanges()
                    End If
                End If
                Session("Diagnostics_VisualTimeBackground") = tbProcs

            End If
            Return tbProcs
        End Get
        Set(value As DataTable)
            Session("Diagnostics_VisualTimeBackground") = value
        End Set
    End Property

    Private Property VisualTimeProcessStatus(Optional ByVal bolReload As Boolean = False) As DataTable
        Get
            Dim tbProcs As DataTable = Session("Diagnostics_VisualTimeProcessStatus")

            If bolReload OrElse tbProcs Is Nothing Then
                If API.DiagMTServiceMethods.GetDailyScheduleStatus(tbProcs) Then

                    Dim lngEntriesCount As Long = 0
                    If API.DiagMTServiceMethods.GetEntriesCount(lngEntriesCount) Then
                        Dim oNewRow As DataRow = tbProcs.NewRow
                        oNewRow("Action") = "Registros en entries"
                        oNewRow("Counter") = lngEntriesCount

                        tbProcs.Rows.Add(oNewRow)
                    End If

                    If tbProcs IsNot Nothing Then
                        tbProcs.PrimaryKey = New DataColumn() {tbProcs.Columns("Action")}
                        tbProcs.AcceptChanges()
                    End If
                End If
                Session("Diagnostics_VisualTimeProcessStatus") = tbProcs

            End If
            Return tbProcs
        End Get
        Set(value As DataTable)
            Session("Diagnostics_VisualTimeProcessStatus") = value
        End Set
    End Property

    Private Property VisualTimeSessions(Optional ByVal bolReload As Boolean = False) As DataTable
        Get
            Dim tbProcs As DataTable = Session("Diagnostics_VisualTimeSessions")

            If bolReload OrElse tbProcs Is Nothing Then
                If API.DiagMTServiceMethods.GetActiveSessionsCount(tbProcs) Then

                    If tbProcs IsNot Nothing Then
                        tbProcs.PrimaryKey = New DataColumn() {tbProcs.Columns("ApplicationName")}
                        tbProcs.AcceptChanges()
                    End If
                End If
                Session("Diagnostics_VisualTimeSessions") = tbProcs

            End If
            Return tbProcs
        End Get
        Set(value As DataTable)
            Session("Diagnostics_VisualTimeSessions") = value
        End Set
    End Property

#End Region

#Region "Events"

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("Options", "~/Diagnostics/Scripts/diagnostics.js")

    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If WLHelperWeb.CurrentPassport Is Nothing OrElse (WLHelperWeb.CurrentPassport IsNot Nothing AndAlso Not WLHelperWeb.CurrentUserIsConsultantOrCegid) Then
            WLHelperWeb.RedirectAccessDenied(False)
            Exit Sub
        End If

        CreateColumnsTaskGrids()
        CreateColumnsTerminalsRow()
        CreateColumnsStatus()
        BindTerminalTypeCmb()

        Me.divDisableHttps.Style("display") = "none"
        Me.divDownloadLogs.Style("display") = "none"
        Me.divCheckMail.Style("display") = "none"

        If Not Me.IsPostBack Then
            Dim Generator As System.Random = New System.Random()

            Dim code As String = Generator.Next(0, 999999)
            If code.Length < 6 Then
                VisualTimeValidationCode = code.PadRight(6, "0")
            Else
                VisualTimeValidationCode = code
            End If

            lblRoboticsCode.Text = VisualTimeValidationCode
        End If

        LoadControlCheckInfo()
        LoadEmployeeAndRequestCount()
        LoadTaskRow(False)
        LoadTerminalRow(False)
        LoadProcStatusRow(False)

    End Sub

    Private Sub BindTerminalTypeCmb()
        Me.cmbTerminalType.Items.Clear()

        Me.cmbTerminalType.Items.Add(New ListEditItem("rx1", "rx1"))
        Me.cmbTerminalType.Items.Add(New ListEditItem("rxFP", "rxFP"))
        Me.cmbTerminalType.Items.Add(New ListEditItem("rxFPTD", "rxFPTD"))
        Me.cmbTerminalType.Items.Add(New ListEditItem("rxFL", "rxFL"))
        Me.cmbTerminalType.Items.Add(New ListEditItem("rxFe", "rxFe"))
        Me.cmbTerminalType.Items.Add(New ListEditItem("mxS", "mxS"))
        Me.cmbTerminalType.Items.Add(New ListEditItem("rxTe", "rxTe"))

        If Me.cmbTerminalType.SelectedItem Is Nothing Then
            Me.cmbTerminalType.SelectedItem = Me.cmbTerminalType.Items(0)
        End If
    End Sub

    Protected Sub diagnosticsPanel_Callback(sender As Object, e As CallbackEventArgsBase) Handles diagnosticsPanel.Callback
        Dim strParameters = String.Empty
        If (Not e.Parameter Is Nothing AndAlso Not String.IsNullOrWhiteSpace(e.Parameter.ToString())) Then strParameters = e.Parameter.ToString()
        strParameters = Server.UrlDecode(strParameters)
        Dim callbackRequest = roJSONHelper.Deserialize(Of DiagnosticsCallbackRequest)(strParameters)

        Select Case callbackRequest.Method
            Case "loadQuerySelector"
                CallbackSession_loadQuerySelector()
            Case "loadData"
                CallbackSession_loadData(callbackRequest.Parameters)
            Case "loadStateServer"
                diagnosticsPanel_loadStateServer()
            Case Else
                diagnosticsPanel.JSProperties.Add("cpActionRO", "UNKNOWN")
        End Select
    End Sub

    Protected Sub testMailPanel_Callback(sender As Object, e As CallbackEventArgsBase) Handles testMailPanel.Callback
        If e.Parameter = "CHECK_MAIL" Then
            txtMailWorking.Text = "-------Checking-------"

            If Robotics.VTBase.CryptographyHelper.GetRoboticsValidationCode(VisualTimeValidationCode) = txtValidationCode.Text Then
                Dim strWorkingMsg As String = String.Empty
                API.DiagMTServiceMethods.CanSendEmail(txtDestination.Text, txtHeader.Text, txtBody.Text, "", strWorkingMsg)
                txtMailWorking.Text = strWorkingMsg
            Else
                txtMailWorking.Text = "Contra código no válido"
            End If

            Dim Generator As System.Random = New System.Random()

            Dim code As String = Generator.Next(0, 999999)
            If code.Length < 6 Then
                VisualTimeValidationCode = code.PadRight(6, "0")
            Else
                VisualTimeValidationCode = code
            End If
            lblRoboticsCode.Text = VisualTimeValidationCode
        End If
    End Sub

#End Region

#Region "Methods"

    Private Sub diagnosticsPanel_loadStateServer()
        LoadControlCheckInfo()
        LoadEmployeeAndRequestCount()
        LoadTaskRow(True)
        LoadTerminalRow(True)
        LoadProcStatusRow(True)
        diagnosticsPanel.JSProperties.Add("cpActionRO", "loadStateServer")
    End Sub

    Private Sub CallbackSession_loadData(parameters As String)
        Try
            CallbackSession.JSProperties.Add("cpActionRO", "loadData")
            Dim params = roJSONHelper.Deserialize(Of Dictionary(Of String, String))(parameters)
            Dim id = Convert.ToInt32(params("id"))
            params.Remove("id")
            Dim data = API.DiagMTServiceMethods.ExecuteQuery(id, params)
            Dim formattedData = data.Rows.Cast(Of DataRow)().Select(Function(row)
                                                                        Dim dictionary = New Dictionary(Of String, String)()
                                                                        For Each column As DataColumn In row.Table.Columns
                                                                            dictionary.Add(column.ColumnName, DataValueToString(row(column.ColumnName)))
                                                                        Next
                                                                        Return dictionary
                                                                    End Function)
            CallbackSession.JSProperties.Add("cpData", New With {.Data = formattedData, .Columns = data.Columns.Cast(Of DataColumn)().Select(Function(o) o.ColumnName).ToArray()})
            CallbackSession.JSProperties.Add("cpStatus", New With {.Correct = True})
        Catch ex As Exception
            CallbackSession.JSProperties.Add("cpStatus", New With {.Correct = False, .Message = "No se ha podido cargar información desde el servidor.", .Exception = ex.Message})
        End Try
    End Sub

    Private Function DataValueToString(o As Object) As String
        If o Is Nothing Then Return String.Empty
        If o Is DBNull.Value Then Return String.Empty
        Return o.ToString()
    End Function

    Private Sub CallbackSession_loadQuerySelector()
        Try
            CallbackSession.JSProperties.Add("cpActionRO", "loadQuerySelector")
            CallbackSession.JSProperties.Add("cpData", API.DiagMTServiceMethods.GetQueries())
            CallbackSession.JSProperties.Add("cpStatus", New With {.Correct = True})
        Catch ex As Exception
            CallbackSession.JSProperties.Add("cpStatus", New With {.Correct = False, .Message = "No se ha podido cargar información desde el servidor.", .Exception = ex.Message})
        End Try
    End Sub

    Private Sub LoadControlCheckInfo()
        Dim bolControl As Boolean = False

        Dim dbVersion As Long = 0
        imgSqlServerConnection.Src = "~/Base/Images/Diagnostics/error.png"
        SQLConnectionActive = False
        If API.DiagMTServiceMethods.GetDBVersion(dbVersion) Then
            imgSqlServerConnection.Src = "~/Base/Images/Diagnostics/ok.png"
            txtSqlServerConnection.Text = "Versión:" & dbVersion
            SQLConnectionActive = True
        End If

        imgHddPermissions.Src = "~/Base/Images/Diagnostics/error.png"
        If API.DiagMTServiceMethods.CanWriteOnSystem(bolControl) Then
            If bolControl Then
                imgHddPermissions.Src = "~/Base/Images/Diagnostics/ok.png"
            End If
        End If

        imgVisualTimeConnection.Src = "~/Base/Images/Diagnostics/error.png"
        If API.DiagMTServiceMethods.IsVisualTimeRunning(bolControl) Then
            If bolControl Then
                imgVisualTimeConnection.Src = "~/Base/Images/Diagnostics/ok.png"
            End If
        End If

        imgConnectionLost24.Src = "~/Base/Images/Diagnostics/ok.png"
        If API.DiagMTServiceMethods.GetConnectionLostDuringLast24Hours(bolControl) Then
            If bolControl Then
                imgConnectionLost24.Src = "~/Base/Images/Diagnostics/error.png"
            End If
        End If

    End Sub

    Private Sub LoadEmployeeAndRequestCount()
        If SQLConnectionActive Then
            Dim dtControl As DataTable = Nothing

            txtEmployeeCount.ForeColor = Drawing.Color.Red
            txtEmployeeCount.Text = "KO"
            txtTaskEmployeeCount.ForeColor = Drawing.Color.Red
            txtTaskEmployeeCount.Text = "KO"
            If API.DiagMTServiceMethods.GetActiveEmployeeCount(dtControl) Then
                txtEmployeeCount.Text = dtControl.Rows(0).Item("TotalGH")
                txtEmployeeCount.ForeColor = Drawing.Color.Black
                txtTaskEmployeeCount.Text = dtControl.Rows(0).Item("TotalProd")
                txtTaskEmployeeCount.ForeColor = Drawing.Color.Black
            End If

            txtRequestCount.ForeColor = Drawing.Color.Red
            txtRequestCount.Text = "KO"
            txtSupervisorCount.ForeColor = Drawing.Color.Red
            txtSupervisorCount.Text = "KO"
            txtEmailPending.ForeColor = Drawing.Color.Red
            txtEmailPending.Text = "KO"

            If API.DiagMTServiceMethods.GetNotificationsCount(dtControl) Then

                txtRequestCount.Text = dtControl.Rows(0).Item("TotalRequest")
                txtRequestCount.ForeColor = Drawing.Color.Black
                txtSupervisorCount.Text = dtControl.Rows(0).Item("TotalSupervisor")
                txtSupervisorCount.ForeColor = Drawing.Color.Black
                txtEmailPending.Text = dtControl.Rows(0).Item("TotalNotifications")
                txtEmailPending.ForeColor = Drawing.Color.Black

            End If
        End If

    End Sub

    Private Sub CreateColumnsTaskGrids()
        Dim GridColumn As GridViewDataTextColumn
        Dim GridColumnDate As GridViewDataDateColumn

        Dim VisibleIndex As Integer = 0

        Me.GridProcess.Columns.Clear()
        Me.GridProcess.KeyFieldName = "ProcessName"
        Me.GridProcess.SettingsText.EmptyDataRow = "No hay ningún proceso en ejecución"
        Me.GridProcess.Settings.VerticalScrollBarMode = ScrollBarMode.Auto

        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = "Nombre de proceso"
        GridColumn.FieldName = "ProcessName"
        GridColumn.VisibleIndex = VisibleIndex
        GridColumn.Width = Unit.Percentage(20)
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        Me.GridProcess.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = "Estado"
        GridColumn.FieldName = "IsRunning"
        GridColumn.VisibleIndex = VisibleIndex
        GridColumn.Width = Unit.Percentage(10)
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        Me.GridProcess.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = "Detalles"
        GridColumn.FieldName = "ProcessInfo"
        GridColumn.VisibleIndex = VisibleIndex
        GridColumn.Width = Unit.Percentage(70)
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        Me.GridProcess.Columns.Add(GridColumn)

        VisibleIndex = 0
        Me.GridTasks.Columns.Clear()
        Me.GridTasks.KeyFieldName = "ID"
        Me.GridTasks.SettingsText.EmptyDataRow = "No hay ningúna tarea en espera de ejecución"
        Me.GridTasks.Settings.VerticalScrollBarMode = ScrollBarMode.Auto

        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = "ID"
        GridColumn.FieldName = "ID"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        Me.GridTasks.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = "ProcessID"
        GridColumn.FieldName = "ProcessID"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        Me.GridTasks.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = "Creador"
        GridColumn.FieldName = "CreatorID"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        Me.GridTasks.Columns.Add(GridColumn)

        GridColumnDate = New GridViewDataDateColumn
        GridColumnDate.Caption = "Fecha creación"
        GridColumnDate.FieldName = "DateCreated"
        GridColumnDate.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnDate.ReadOnly = True
        Me.GridTasks.Columns.Add(GridColumnDate)

        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = "Contexto"
        GridColumn.FieldName = "Context"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        Me.GridTasks.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = "Sistema"
        GridColumn.FieldName = "SystemTask"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        Me.GridTasks.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = "Externa"
        GridColumn.FieldName = "ExternalTask"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        Me.GridTasks.Columns.Add(GridColumn)

    End Sub

    Private Sub CreateColumnsTerminalsRow()
        Dim GridColumn As GridViewDataTextColumn
        Dim GridColumnDate As GridViewDataDateColumn

        Dim VisibleIndex As Integer = 0

        Me.GridTerminales.Columns.Clear()
        Me.GridTerminales.KeyFieldName = "ID"
        Me.GridTerminales.SettingsText.EmptyDataRow = "No hay ningún terminal en la base de datos"
        Me.GridTerminales.Settings.VerticalScrollBarMode = ScrollBarMode.Auto

        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = "Identificador"
        GridColumn.FieldName = "ID"
        GridColumn.VisibleIndex = VisibleIndex
        GridColumn.Width = Unit.Percentage(20)
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        Me.GridTerminales.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = "Modelo"
        GridColumn.FieldName = "Type"
        GridColumn.VisibleIndex = VisibleIndex
        GridColumn.Width = Unit.Percentage(10)
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        Me.GridTerminales.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = "Estado"
        GridColumn.FieldName = "LastStatus"
        GridColumn.VisibleIndex = VisibleIndex
        GridColumn.Width = Unit.Percentage(70)
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        Me.GridTerminales.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = "Dirección"
        GridColumn.FieldName = "Location"
        GridColumn.VisibleIndex = VisibleIndex
        GridColumn.Width = Unit.Percentage(70)
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        Me.GridTerminales.Columns.Add(GridColumn)

        GridColumnDate = New GridViewDataDateColumn
        GridColumnDate.Caption = "Última Actualización"
        GridColumnDate.FieldName = "LastUpdate"
        GridColumnDate.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnDate.ReadOnly = True
        Me.GridTerminales.Columns.Add(GridColumnDate)

        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = "Tareas pendientes"
        GridColumn.FieldName = "TerminalSyncTasks"
        GridColumn.VisibleIndex = VisibleIndex
        GridColumn.Width = Unit.Percentage(70)
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        Me.GridTerminales.Columns.Add(GridColumn)

        VisibleIndex = 0
        Me.GridBackgroundTasks.Columns.Clear()
        Me.GridBackgroundTasks.KeyFieldName = "InternalID"
        Me.GridBackgroundTasks.SettingsText.EmptyDataRow = "No hay ningúna tarea en espera de ejecución"
        Me.GridBackgroundTasks.Settings.VerticalScrollBarMode = ScrollBarMode.Auto

        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = "Tipo"
        GridColumn.FieldName = "Type"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        Me.GridBackgroundTasks.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = "Estado"
        GridColumn.FieldName = "Estado"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        Me.GridBackgroundTasks.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = "Nombre"
        GridColumn.FieldName = "Name"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        Me.GridBackgroundTasks.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = "Perfil"
        GridColumn.FieldName = "Perfil"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        Me.GridBackgroundTasks.Columns.Add(GridColumn)

        GridColumnDate = New GridViewDataDateColumn
        GridColumnDate.Caption = "Última ejecución"
        GridColumnDate.FieldName = "LastExecution"
        GridColumnDate.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnDate.ReadOnly = True
        Me.GridBackgroundTasks.Columns.Add(GridColumnDate)

    End Sub

    Private Sub CreateColumnsStatus()
        Dim GridColumn As GridViewDataTextColumn

        Dim VisibleIndex As Integer = 0

        Me.GridProcCounters.Columns.Clear()
        Me.GridProcCounters.KeyFieldName = "Action"
        Me.GridProcCounters.SettingsText.EmptyDataRow = "No hay ningún proceso en ejecución"
        Me.GridProcCounters.Settings.VerticalScrollBarMode = ScrollBarMode.Auto

        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = "Estado de proceso"
        GridColumn.FieldName = "Action"
        GridColumn.VisibleIndex = VisibleIndex
        GridColumn.Width = Unit.Percentage(20)
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        Me.GridProcCounters.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = "Contador"
        GridColumn.FieldName = "Counter"
        GridColumn.VisibleIndex = VisibleIndex
        GridColumn.Width = Unit.Percentage(10)
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        Me.GridProcCounters.Columns.Add(GridColumn)

        VisibleIndex = 0

        Me.GridActiveSessions.Columns.Clear()
        Me.GridActiveSessions.KeyFieldName = "ApplicationName"
        Me.GridActiveSessions.SettingsText.EmptyDataRow = "No hay sesiones activas"
        Me.GridActiveSessions.Settings.VerticalScrollBarMode = ScrollBarMode.Auto

        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = "Aplicacion"
        GridColumn.FieldName = "ApplicationName"
        GridColumn.VisibleIndex = VisibleIndex
        GridColumn.Width = Unit.Percentage(20)
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        Me.GridActiveSessions.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = "Contador"
        GridColumn.FieldName = "ActiveSessions"
        GridColumn.VisibleIndex = VisibleIndex
        GridColumn.Width = Unit.Percentage(10)
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        Me.GridActiveSessions.Columns.Add(GridColumn)
    End Sub

    Private Sub LoadTaskRow(Optional ByVal bolReload As Boolean = False)

        GridProcess.DataSource = VisualTimeProcess(bolReload)
        GridProcess.DataBind()

        If SQLConnectionActive Then
            GridTasks.DataSource = VisualTimeTasks(bolReload)
            GridTasks.DataBind()
        End If

    End Sub

    Private Sub LoadTerminalRow(Optional ByVal bolReload As Boolean = False)
        If SQLConnectionActive Then
            GridTerminales.DataSource = VisualTimeTerminals(bolReload)
            GridTerminales.DataBind()

            GridBackgroundTasks.DataSource = VisualTimeBackgroundTasks(bolReload)
            GridBackgroundTasks.DataBind()
        End If
    End Sub

    Private Sub LoadProcStatusRow(Optional ByVal bolReload As Boolean = False)
        If SQLConnectionActive Then
            GridProcCounters.DataSource = VisualTimeProcessStatus(bolReload)
            GridProcCounters.DataBind()

            GridActiveSessions.DataSource = VisualTimeSessions(bolReload)
            GridActiveSessions.DataBind()
        End If

    End Sub

    Private Sub CallbackSession_Callback(source As Object, e As CallbackEventArgs) Handles CallbackSession.Callback
        Dim strParameters = String.Empty
        If (Not e.Parameter Is Nothing AndAlso Not String.IsNullOrWhiteSpace(e.Parameter.ToString())) Then strParameters = e.Parameter.ToString()
        strParameters = Server.UrlDecode(strParameters)
        Dim callbackRequest = roJSONHelper.Deserialize(Of DiagnosticsCallbackRequest)(strParameters)

        Select Case callbackRequest.Method
            Case "loadQuerySelector"
                CallbackSession_loadQuerySelector()
            Case "loadData"
                CallbackSession_loadData(callbackRequest.Parameters)
            Case "btnDisableHTTPS_Click"
                btnDisableHTTPS_Click()
            Case "loadDisableHTTPS"
                loadDisableHTTPS()
            Case "btnResetTerminal_Click"
                btnResetTerminal_Click(callbackRequest.Parameters)
            Case "btnRebootTerminal_Click"
                btnRebootTerminal_Click(callbackRequest.Parameters)
            Case "registerTerminalMT"
                registerTerminalMT(callbackRequest.Parameters)
            Case "importVTCFile"
                importVTCFile(callbackRequest.Parameters)
            Case "saveMx9Parameter"
                saveMx9Parameter(callbackRequest.Parameters)
        End Select
    End Sub

    Private Sub saveMx9Parameter(ByVal strParams As String)
        Try
            CallbackSession.JSProperties.Add("cpActionRO", "saveMx9Parameter")

            Dim params = roJSONHelper.Deserialize(Of Dictionary(Of String, String))(strParams)
            Dim idTerminal As String = roTypes.Any2Integer(params("idTerminal"))
            Dim paramName As String = roTypes.Any2String(params("name"))
            Dim paramValue As String = roTypes.Any2String(params("value"))
            Dim strRes As Boolean = API.TerminalServiceMethods.SaveMx9Parameter(Nothing, idTerminal, paramName, paramValue)
            If strRes Then
                CallbackSession.JSProperties.Add("cpStatus", New With {.Correct = True})
            Else
                CallbackSession.JSProperties.Add("cpStatus", New With {.Correct = False, .Message = strRes, .Exception = String.Empty})
            End If
        Catch ex As Exception
            CallbackSession.JSProperties.Add("cpStatus", New With {.Correct = False, .Message = "No se ha podido dar de alta el terminal indicado.", .Exception = ex.Message})
        End Try
    End Sub

    Private Sub importVTCFile(ByVal strParams As String)
        Try
            CallbackSession.JSProperties.Add("cpActionRO", "importVTCFile")

            Dim params = roJSONHelper.Deserialize(Of Dictionary(Of String, String))(strParams)
            Dim vtcContent As String = roTypes.Any2String(params("content"))

            Dim strRes As Boolean = API.TerminalServiceMethods.ImportVTC(Nothing, vtcContent)
            If strRes Then
                CallbackSession.JSProperties.Add("cpStatus", New With {.Correct = True})
            Else
                CallbackSession.JSProperties.Add("cpStatus", New With {.Correct = False, .Message = strRes, .Exception = String.Empty})
            End If
        Catch ex As Exception
            CallbackSession.JSProperties.Add("cpStatus", New With {.Correct = False, .Message = "No se ha podido dar de alta el terminal indicado.", .Exception = ex.Message})
        End Try
    End Sub

    Private Sub registerTerminalMT(ByVal strParams As String)
        Try
            CallbackSession.JSProperties.Add("cpActionRO", "registerTerminalMT")

            Dim params = roJSONHelper.Deserialize(Of Dictionary(Of String, String))(strParams)
            Dim serialNumber As String = roTypes.Any2String(params("terminalId"))
            Dim terminalType As String = roTypes.Any2String(params("terminalType"))

            Dim strRes As String = API.TerminalServiceMethods.RegisterTerminalOnMT(Nothing, roTypes.Any2String(roConstants.GetGlobalEnvironmentParameter(Robotics.Base.DTOs.GlobalAsaxParameter.CompanyId)), serialNumber, terminalType)
            If strRes.ToUpper = "OK" Then
                CallbackSession.JSProperties.Add("cpStatus", New With {.Correct = True, .Message = "Terminal se ha creado con éxito"})
            Else
                CallbackSession.JSProperties.Add("cpStatus", New With {.Correct = False, .Message = strRes, .Exception = String.Empty})
            End If
        Catch ex As Exception
            CallbackSession.JSProperties.Add("cpStatus", New With {.Correct = False, .Message = "No se ha podido dar de alta el terminal indicado.", .Exception = ex.Message})
        End Try
    End Sub

    Protected Sub btnDisableHTTPS_Click()
        Try
            CallbackSession.JSProperties.Add("cpActionRO", "btnDisableHTTPS_Click")
            API.DiagMTServiceMethods.ChangeVTLiveApiHTTPSEnabled()
            Dim isCurrentlyEnabled = API.DiagMTServiceMethods.IsVTLiveApiHTTPSEnabled()
            CallbackSession.JSProperties.Add("cpData", New With {.ButtonText = IIf(isCurrentlyEnabled, "Deshabilitar", "Habilitar"), .LabelText = IIf(isCurrentlyEnabled, "Deshabilitar HTTPS en VTLiveApi", "Habilitar HTTPS en VTLiveApi")})
            CallbackSession.JSProperties.Add("cpStatus", New With {.Correct = True})
        Catch ex As Exception
            CallbackSession.JSProperties.Add("cpStatus", New With {.Correct = False, .Message = "No se ha podido cargar información desde el servidor.", .Exception = ex.Message})
        End Try
    End Sub

    Private Sub loadDisableHTTPS()
        CallbackSession.JSProperties.Add("cpActionRO", "loadDisableHTTPS")
        CallbackSession.JSProperties.Add("cpData", New With {.ButtonText = "Deshabilitar", .LabelText = "Deshabilitar HTTPS en VTLiveApi"})
        CallbackSession.JSProperties.Add("cpStatus", New With {.Correct = True})

    End Sub

    Protected Sub btnResetTerminal_Click(parameters As String)
        Try
            CallbackSession.JSProperties.Add("cpActionRO", "btnResetTerminal_Click")
            Dim params = roJSONHelper.Deserialize(Of Dictionary(Of String, String))(parameters)
            Dim terminalId = Convert.ToInt32(params("terminalId"))
            API.DiagMTServiceMethods.DeleteTerminalFolder(terminalId)
            'Parte de la lógica se tiene que realizar desde aquí ya que parte del negocio se encuetra en BaseWeb
            If Not API.DiagMTServiceMethods.LaunchBroadcasterForTerminal(terminalId) Then Throw New Exception("No se ha podido lanzar el volvado completo del terminal.")
            CallbackSession.JSProperties.Add("cpStatus", New With {.Correct = True})
        Catch ex As Exception
            CallbackSession.JSProperties.Add("cpStatus", New With {.Correct = False, .Message = "No se ha podido cargar información desde el servidor.", .Exception = ex.Message})
        End Try
    End Sub

    Protected Sub btnRebootTerminal_Click(parameters As String)
        Dim errorMessage As String = String.Empty
        Try
            CallbackSession.JSProperties.Add("cpActionRO", "btnRebootTerminal_Click")
            Dim params = roJSONHelper.Deserialize(Of Dictionary(Of String, String))(parameters)
            Dim terminalId = Convert.ToInt32(params("terminalId"))

            If Not API.DiagMTServiceMethods.RebootTerminal(terminalId, errorMessage) Then
                If errorMessage = String.Empty Then errorMessage = "No se ha podido lanzar el reinicio del terminal."
                Throw New Exception(errorMessage)
            End If
            CallbackSession.JSProperties.Add("cpStatus", New With {.Correct = True})
        Catch ex As Exception
            CallbackSession.JSProperties.Add("cpStatus", New With {.Correct = False, .Message = errorMessage, .Exception = ex.Message})
        End Try
    End Sub

#End Region

End Class