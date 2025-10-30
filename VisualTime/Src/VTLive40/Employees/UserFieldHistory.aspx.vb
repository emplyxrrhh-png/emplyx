Imports DevExpress.Web
Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class UserFieldHistory
    Inherits PageBase

    Private Const FeatureAlias As String = "Employees.UserFields.Information"

#Region "Declarations"

    Private strFeatureAlias As String = FeatureAlias
    Private oPermission As Permission
    Private oCurrentPermission As Permission

    Private intIDSource As Integer
    Private oType As Types
    Private strFieldName As String

#End Region

#Region "Properties"

    Private Property HistoryData(Optional ByVal bolReload As Boolean = False) As DataTable
        Get

            Dim tb As DataTable = Session("History_HistoryData")

            If bolReload OrElse tb Is Nothing Then

                Select Case Me.oType
                    Case Types.EmployeeField
                        tb = API.EmployeeServiceMethods.GetUserFieldHistoryDatatable(Me, Me.intIDSource, Me.strFieldName)
                    Case Types.GroupField
                        tb = Nothing
                End Select

                If tb IsNot Nothing Then
                    Select Case Me.UserFieldDefinition.FieldType
                        Case FieldTypes.tDecimal
                            tb.Columns.Add(New DataColumn("DecimalValue", GetType(String)))
                        Case FieldTypes.tDate
                            tb.Columns.Add(New DataColumn("DateValue", GetType(Date)))
                        Case FieldTypes.tTime
                            tb.Columns.Add(New DataColumn("TimeValue", GetType(String)))
                        Case FieldTypes.tDatePeriod
                            tb.Columns.Add(New DataColumn("InitialDateTime", GetType(Date)))
                            tb.Columns.Add(New DataColumn("EndDateTime", GetType(Date)))
                        Case FieldTypes.tTimePeriod
                            tb.Columns.Add(New DataColumn("StartHourPeriod", GetType(Date)))
                            tb.Columns.Add(New DataColumn("EndHourPeriod", GetType(Date)))
                        Case FieldTypes.tLink
                            tb.Columns.Add(New DataColumn("LinkShow", GetType(String)))
                            tb.Columns.Add(New DataColumn("LinkReal", GetType(String)))
                    End Select

                    If tb.Rows.Count > 0 Then
                        For Each dRow As DataRow In tb.Rows

                            Dim splitValues() As String = roTypes.Any2String(dRow("Value")).Split("*")

                            Select Case Me.UserFieldDefinition.FieldType
                                Case FieldTypes.tDecimal
                                    Dim strValue As String = roTypes.Any2String(dRow("Value"))
                                    If strValue <> "" Then
                                        dRow("DecimalValue") = strValue.Replace(".", HelperWeb.GetDecimalDigitFormat())
                                    End If
                                Case FieldTypes.tDate
                                    Dim DateValue As String = roTypes.Any2String(dRow("Value"))
                                    If (DateValue <> String.Empty) Then
                                        dRow("DateValue") = CDate(DateValue.ToString)
                                    End If
                                Case FieldTypes.tTime
                                    Dim timeValue As String = roTypes.Any2String(dRow("Value"))
                                    If timeValue <> "" AndAlso IsNumeric(timeValue) Then
                                        Dim strValue As String = roConversions.ConvertHoursToTime(CDbl(timeValue.Replace(".", HelperWeb.GetDecimalDigitFormat())))
                                        If strValue.StartsWith("00") Then strValue = "0" & strValue.Substring(2)

                                        dRow("TimeValue") = strValue
                                    End If
                                Case FieldTypes.tDatePeriod
                                    If splitValues.Length > 1 Then
                                        If splitValues(0) <> "" Then dRow("InitialDateTime") = New Date(splitValues(0).Substring(0, 4), splitValues(0).Substring(5, 2), splitValues(0).Substring(8, 2))
                                        If splitValues(1) <> "" Then dRow("EndDateTime") = New Date(splitValues(1).Substring(0, 4), splitValues(1).Substring(5, 2), splitValues(1).Substring(8, 2))
                                    End If
                                Case FieldTypes.tTimePeriod
                                    If splitValues.Length > 1 Then
                                        If splitValues(0) <> "" Then dRow("StartHourPeriod") = New Date(1900, 1, 1, splitValues(0).Substring(0, 2), splitValues(0).Substring(3, 2), 0)
                                        If splitValues(1) <> "" Then dRow("EndHourPeriod") = New Date(1900, 1, 1, splitValues(1).Substring(0, 2), splitValues(1).Substring(3, 2), 0)
                                    End If
                                Case FieldTypes.tLink
                                    If splitValues.Length > 1 Then
                                        dRow("LinkShow") = roTypes.Any2String(splitValues(0))
                                        dRow("LinkReal") = roTypes.Any2String(splitValues(1))
                                    End If
                            End Select

                        Next
                    End If
                End If

                If Not tb Is Nothing Then
                    tb.PrimaryKey = New DataColumn() {tb.Columns("ID")}
                    tb.AcceptChanges()
                End If

                Session("History_HistoryData") = tb

            End If

            Return tb

        End Get
        Set(ByVal value As DataTable)
            Session("History_HistoryData") = value
        End Set
    End Property

    Private Property UserFieldDefinition() As roUserField
        Get

            Dim oDefinition As roUserField = Session("History_UserFieldDefinition")

            If oDefinition Is Nothing Then

                oDefinition = API.UserFieldServiceMethods.GetUserField(Me.Page, Me.strFieldName, Me.oType, False, True)

                If oDefinition IsNot Nothing Then
                    Session("History_UserFieldDefinition") = oDefinition
                End If

            End If

            Return oDefinition

        End Get
        Set(ByVal value As roUserField)
            Session("History_UserFieldDefinition") = value
        End Set
    End Property

#End Region

#Region "Events"

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("BrowserDetect", "~/Base/Scripts/BrowserDetect.js", , True)
        Me.InsertExtraJavascript("Cookies", "~/Base/Scripts/Cookies.js", , True)
        Me.InsertExtraJavascript("Generic", "~/Base/Scripts/Generic.js", , True)
    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Me.InsertCssIncludes(Me.Page)

        If Request.Params("IDSource") IsNot Nothing AndAlso IsNumeric(Request.Params("IDSource")) Then
            Me.intIDSource = CInt(Request.Params("IDSource"))
        End If
        If Request.Params("Type") IsNot Nothing AndAlso IsNumeric(Request.Params("Type")) Then
            Me.oType = Val(Request.Params("Type"))
        End If
        If Request.Params("FieldName") IsNot Nothing Then
            Me.strFieldName = Request.Params("FieldName")
        End If

        If Not Me.IsPostBack Then
            Me.UserFieldDefinition = Nothing
            If Me.UserFieldDefinition IsNot Nothing Then
                Select Case Me.UserFieldDefinition.AccessLevel
                    Case AccessLevels.aLow
                        strFeatureAlias &= ".Low"
                    Case AccessLevels.aMedium
                        strFeatureAlias &= ".Medium"
                    Case AccessLevels.aHigh
                        strFeatureAlias &= ".High"
                End Select
            End If
        End If

        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)
        If Me.oType = Types.EmployeeField Then
            Me.oCurrentPermission = Me.GetFeaturePermissionByEmployee(Me.strFeatureAlias, Me.intIDSource)
        Else
            Me.oCurrentPermission = Me.GetFeaturePermissionByGroup(Me.strFeatureAlias, Me.intIDSource)
        End If

        If Me.oCurrentPermission >= Permission.Read Then

            If Not Me.IsPostBack Then
                CreateColumnsHistory()

                Me.HistoryData = Nothing

                Me.btAccept.Visible = (Me.oCurrentPermission >= Permission.Write)
                If Me.oCurrentPermission = Permission.Read Then
                    Me.btCancel.Text = Me.Language.Keyword("Button.Close")
                End If

                BindGridHistory(True)
            Else
                BindGridHistory(False)
            End If
        Else
            WLHelperWeb.RedirectAccessDenied(True)
        End If

    End Sub

    Private Sub BindGridHistory(ByVal bolReload As Boolean)
        Me.GridHistoric.DataSource = Me.HistoryData(bolReload)
        Me.GridHistoric.DataBind()
    End Sub

    Private Sub CreateColumnsHistory()

        Dim GridColumn As GridViewDataTextColumn
        Dim GridColumnCommand As GridViewCommandColumn
        Dim GridColumnDate As GridViewDataDateColumn
        Dim GridColumnTime As GridViewDataTimeEditColumn
        Dim GridColumnCombo As GridViewDataComboBoxColumn

        Dim VisibleIndex As Integer = 0

        Me.GridHistoric.Columns.Clear()
        Me.GridHistoric.KeyFieldName = "ID"
        Me.GridHistoric.SettingsText.EmptyDataRow = " "
        Me.GridHistoric.Settings.VerticalScrollBarMode = ScrollBarMode.Auto
        Me.GridHistoric.SettingsEditing.Mode = GridViewEditingMode.Inline

        'Command buttons
        GridColumnCommand = New GridViewCommandColumn()
        GridColumnCommand.ButtonRenderMode = GridCommandButtonRenderMode.Image
        GridColumnCommand.ShowDeleteButton = True
        GridColumnCommand.ShowEditButton = True
        GridColumnCommand.Caption = " "
        GridColumnCommand.VisibleIndex = VisibleIndex
        GridColumnCommand.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCommand.Width = 10
        VisibleIndex = VisibleIndex + 1
        Me.GridHistoric.Columns.Add(GridColumnCommand)

        'Clave
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "ID"
        GridColumn.FieldName = "ID"
        GridColumn.ReadOnly = False
        GridColumn.Visible = False
        GridColumn.Width = 40
        Me.GridHistoric.Columns.Add(GridColumn)

        'Date
        GridColumnDate = New GridViewDataDateColumn()
        GridColumnDate.Caption = Me.Language.Translate("GridHistoric.Column.Date", DefaultScope) '"Fecha"
        GridColumnDate.FieldName = "OriginalDate"
        GridColumnDate.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnDate.ReadOnly = False
        GridColumnDate.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnDate.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnDate.Width = 45
        Me.GridHistoric.Columns.Add(GridColumnDate)

        Select Case Me.UserFieldDefinition.FieldType
            Case FieldTypes.tDate
                'Value
                GridColumnDate = New GridViewDataDateColumn
                GridColumnDate.Caption = Me.Language.Translate("GridHistoric.Column.Date", DefaultScope) '"Valor"
                GridColumnDate.FieldName = "DateValue"
                GridColumnDate.VisibleIndex = VisibleIndex
                VisibleIndex = VisibleIndex + 1
                GridColumnDate.ReadOnly = False
                GridColumnDate.CellStyle.HorizontalAlign = HorizontalAlign.Left
                GridColumnDate.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
                GridColumnDate.Width = 43
                Me.GridHistoric.Columns.Add(GridColumnDate)
            Case FieldTypes.tText
                'Value
                GridColumn = New GridViewDataTextColumn
                GridColumn.Caption = Me.Language.Translate("GridHistoric.Column.Value", DefaultScope) '"Valor"
                GridColumn.FieldName = "Value"
                GridColumn.VisibleIndex = VisibleIndex
                VisibleIndex = VisibleIndex + 1
                GridColumn.ReadOnly = False
                GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
                GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
                GridColumn.Width = 43
                Me.GridHistoric.Columns.Add(GridColumn)
            Case FieldTypes.tNumeric
                'Value
                GridColumn = New GridViewDataTextColumn
                GridColumn.Caption = Me.Language.Translate("GridHistoric.Column.Value", DefaultScope) '"Valor"
                GridColumn.FieldName = "Value"
                GridColumn.VisibleIndex = VisibleIndex
                GridColumn.PropertiesTextEdit.MaskSettings.Mask = "<-9999999..9999999>"
                VisibleIndex = VisibleIndex + 1
                GridColumn.ReadOnly = False
                GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
                GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
                GridColumn.Width = 43
                Me.GridHistoric.Columns.Add(GridColumn)
            Case FieldTypes.tDecimal
                'Value
                GridColumn = New GridViewDataTextColumn
                GridColumn.Caption = Me.Language.Translate("GridHistoric.Column.Value", DefaultScope) '"Valor"
                GridColumn.FieldName = "DecimalValue"
                GridColumn.VisibleIndex = VisibleIndex
                GridColumn.PropertiesTextEdit.MaskSettings.Mask = "<-99999999999..99999999999>.<000000..999999>"
                GridColumn.PropertiesTextEdit.MaskSettings.IncludeLiterals = MaskIncludeLiteralsMode.DecimalSymbol
                VisibleIndex = VisibleIndex + 1
                GridColumn.ReadOnly = False
                GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
                GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
                GridColumn.Width = 43
                Me.GridHistoric.Columns.Add(GridColumn)
            Case FieldTypes.tTime
                'Value
                GridColumn = New GridViewDataTextColumn
                GridColumn.Caption = Me.Language.Translate("GridHistoric.Column.Hour", DefaultScope) '"Valor"
                GridColumn.FieldName = "TimeValue"
                GridColumn.VisibleIndex = VisibleIndex
                GridColumn.PropertiesTextEdit.MaskSettings.Mask = "<-9999..9999>:<00..59>"
                VisibleIndex = VisibleIndex + 1
                GridColumn.ReadOnly = False
                GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
                GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
                GridColumn.Width = 43
                Me.GridHistoric.Columns.Add(GridColumn)
            Case FieldTypes.tList
                GridColumnCombo = New GridViewDataComboBoxColumn
                GridColumnCombo.Caption = Me.Language.Translate("GridHistoric.Column.Value", DefaultScope) '"Valor"
                GridColumnCombo.FieldName = "Value"
                GridColumnCombo.VisibleIndex = VisibleIndex
                GridColumnCombo.PropertiesComboBox.DataSource = Me.UserFieldDefinition.ListValues
                VisibleIndex = VisibleIndex + 1
                GridColumnCombo.ReadOnly = False
                GridColumnCombo.CellStyle.HorizontalAlign = HorizontalAlign.Left
                GridColumnCombo.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
                GridColumnCombo.Width = 43
                Me.GridHistoric.Columns.Add(GridColumnCombo)
            Case FieldTypes.tDatePeriod
                GridColumnDate = New GridViewDataDateColumn
                GridColumnDate.Caption = Me.Language.Translate("GridHistoric.Column.DateInicial", DefaultScope) '"Valor"
                GridColumnDate.FieldName = "InitialDateTime"
                GridColumnDate.VisibleIndex = VisibleIndex
                VisibleIndex = VisibleIndex + 1
                GridColumnDate.ReadOnly = False
                GridColumnDate.CellStyle.HorizontalAlign = HorizontalAlign.Left
                GridColumnDate.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
                GridColumnDate.Width = 43
                Me.GridHistoric.Columns.Add(GridColumnDate)

                GridColumnDate = New GridViewDataDateColumn
                GridColumnDate.Caption = Me.Language.Translate("GridHistoric.Column.DateFinal", DefaultScope) '"Valor"
                GridColumnDate.FieldName = "EndDateTime"
                GridColumnDate.VisibleIndex = VisibleIndex
                VisibleIndex = VisibleIndex + 1
                GridColumnDate.ReadOnly = False
                GridColumnDate.CellStyle.HorizontalAlign = HorizontalAlign.Left
                GridColumnDate.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
                GridColumnDate.Width = 43
                Me.GridHistoric.Columns.Add(GridColumnDate)
            Case FieldTypes.tTimePeriod
                'Value
                GridColumnTime = New GridViewDataTimeEditColumn
                GridColumnTime.Caption = Me.Language.Translate("GridHistoric.Column.HourInicial", DefaultScope) '"Valor"
                GridColumnTime.FieldName = "StartHourPeriod"
                GridColumnTime.VisibleIndex = VisibleIndex
                VisibleIndex = VisibleIndex + 1
                GridColumnTime.ReadOnly = False
                GridColumnTime.CellStyle.HorizontalAlign = HorizontalAlign.Left
                GridColumnTime.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
                GridColumnTime.Width = 43
                Me.GridHistoric.Columns.Add(GridColumnTime)
                'Value
                GridColumnTime = New GridViewDataTimeEditColumn
                GridColumnTime.Caption = Me.Language.Translate("GridHistoric.Column.HourFinal", DefaultScope) '"Valor"
                GridColumnTime.FieldName = "EndHourPeriod"
                GridColumnTime.VisibleIndex = VisibleIndex
                VisibleIndex = VisibleIndex + 1
                GridColumnTime.ReadOnly = False
                GridColumnTime.CellStyle.HorizontalAlign = HorizontalAlign.Left
                GridColumnTime.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
                GridColumnTime.Width = 43
                Me.GridHistoric.Columns.Add(GridColumnTime)
            Case FieldTypes.tLink
                'Value
                GridColumn = New GridViewDataTextColumn
                GridColumn.Caption = Me.Language.Translate("GridHistoric.Column.ShowValue", DefaultScope) '"Valor"
                GridColumn.FieldName = "LinkShow"
                GridColumn.VisibleIndex = VisibleIndex
                VisibleIndex = VisibleIndex + 1
                GridColumn.ReadOnly = False
                GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
                GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
                GridColumn.Width = 43
                Me.GridHistoric.Columns.Add(GridColumn)

                GridColumn = New GridViewDataTextColumn
                GridColumn.Caption = Me.Language.Translate("GridHistoric.Column.LinkValue", DefaultScope) '"Valor"
                GridColumn.FieldName = "LinkReal"
                GridColumn.VisibleIndex = VisibleIndex
                VisibleIndex = VisibleIndex + 1
                GridColumn.ReadOnly = False
                GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
                GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
                GridColumn.Width = 43
                Me.GridHistoric.Columns.Add(GridColumn)
        End Select

    End Sub

    Private Sub GridHistoric_DataBinding(sender As Object, e As EventArgs) Handles GridHistoric.DataBinding

        Select Case Me.UserFieldDefinition.FieldType
            Case FieldTypes.tList
                Dim oCombo As GridViewDataComboBoxColumn = GridHistoric.Columns("Value")
                oCombo.PropertiesComboBox.DataSource = Me.UserFieldDefinition.ListValues
        End Select
    End Sub

    Protected Sub GridHistoric_InitNewRow(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataInitNewRowEventArgs) Handles GridHistoric.InitNewRow
        Dim tb As DataTable = Me.HistoryData()
        If (tb.Rows.Count > 0) Then
            e.NewValues("ID") = roTypes.Any2Integer(tb.Rows(tb.Rows.Count - 1)("ID")) + 1
        Else
            e.NewValues("ID") = 1
        End If
        e.NewValues("OriginalDate") = Date.Now.Date

    End Sub

    Protected Sub GridHistoric_RowInserting(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataInsertingEventArgs) Handles GridHistoric.RowInserting
        Dim grid As ASPxGridView = CType(sender, ASPxGridView)

        If Not e.NewValues Is Nothing Then
            Dim newID As Integer

            Dim tb As DataTable = Me.HistoryData()
            If (tb.Rows.Count > 0) Then
                newID = roTypes.Any2Integer(tb.Rows(tb.Rows.Count - 1)("ID")) + 1
            Else
                newID = 1
            End If

            Dim oNewRow As DataRow = tb.NewRow
            With oNewRow
                .Item("ID") = newID
                .Item("FieldName") = Me.UserFieldDefinition.FieldName
                .Item("OriginalDate") = roTypes.Any2DateTime(e.NewValues("OriginalDate")).Date
                Select Case Me.UserFieldDefinition.FieldType
                    Case FieldTypes.tDecimal
                        .Item("DecimalValue") = e.NewValues("DecimalValue")
                        .Item("Value") = e.NewValues("DecimalValue")
                    Case FieldTypes.tDate
                        .Item("DateValue") = e.NewValues("DateValue")
                        .Item("Value") = Format(roTypes.Any2DateTime(e.NewValues("DateValue")), "yyyy/MM/dd")
                    Case FieldTypes.tTime
                        .Item("TimeValue") = e.NewValues("TimeValue")
                        .Item("Value") = roConversions.ConvertTimeToHours(e.NewValues("TimeValue")).ToString().Replace(HelperWeb.GetDecimalDigitFormat(), ".")
                    Case FieldTypes.tDatePeriod
                        Dim newVal As String = ""
                        If roTypes.Any2String(e.NewValues("InitialDateTime")) <> "" Then newVal = Format(roTypes.Any2DateTime(e.NewValues("InitialDateTime")), "yyyy/MM/dd")
                        newVal = newVal & "*"
                        If roTypes.Any2String(e.NewValues("EndDateTime")) <> "" Then newVal = newVal & Format(roTypes.Any2DateTime(e.NewValues("EndDateTime")), "yyyy/MM/dd")
                        .Item("Value") = newVal
                        If e.NewValues("InitialDateTime") = Nothing Then
                            .Item("InitialDateTime") = DBNull.Value
                        Else
                            .Item("InitialDateTime") = e.NewValues("InitialDateTime")
                        End If

                        If e.NewValues("EndDateTime") = Nothing Then
                            .Item("EndDateTime") = DBNull.Value
                        Else
                            .Item("EndDateTime") = e.NewValues("EndDateTime")
                        End If
                    Case FieldTypes.tTimePeriod
                        Dim newVal As String = ""
                        If roTypes.Any2String(e.NewValues("StartHourPeriod")) <> "" Then newVal = Format(roTypes.Any2DateTime(e.NewValues("StartHourPeriod")), "HH:mm")
                        newVal = newVal & "*"
                        If roTypes.Any2String(e.NewValues("EndHourPeriod")) <> "" Then newVal = newVal & Format(roTypes.Any2DateTime(e.NewValues("EndHourPeriod")), "HH:mm")
                        .Item("Value") = newVal

                        If e.NewValues("StartHourPeriod") = Nothing Then
                            .Item("StartHourPeriod") = DBNull.Value
                        Else
                            .Item("StartHourPeriod") = e.NewValues("StartHourPeriod")
                        End If

                        If e.NewValues("EndHourPeriod") = Nothing Then
                            .Item("EndHourPeriod") = DBNull.Value
                        Else
                            .Item("EndHourPeriod") = e.NewValues("EndHourPeriod")
                        End If

                    Case FieldTypes.tLink
                        .Item("Value") = roTypes.Any2String(e.NewValues("LinkShow")) & "*" & roTypes.Any2String(e.NewValues("LinkReal"))
                        .Item("LinkShow") = e.NewValues("LinkShow")
                        .Item("LinkReal") = e.NewValues("LinkReal")
                    Case Else
                        .Item("Value") = e.NewValues("Value")
                End Select

            End With

            tb.Rows.Add(oNewRow)

            Me.HistoryData = tb
            e.Cancel = True
            grid.CancelEdit()

        End If
    End Sub

    Protected Sub GridHistoric_CustomErrorText(ByVal sender As Object, ByVal e As ASPxGridViewCustomErrorTextEventArgs) Handles GridHistoric.CustomErrorText
        If e.Exception IsNot Nothing Then
            e.ErrorText = e.Exception.Message
        End If
    End Sub

    Protected Sub GridHistoric_RowUpdating(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataUpdatingEventArgs) Handles GridHistoric.RowUpdating

        Dim tb As DataTable = Me.HistoryData()
        Dim dr As DataRow = tb.Rows.Find(e.Keys(GridHistoric.KeyFieldName))
        Dim grid As ASPxGridView = CType(sender, ASPxGridView)

        Dim enumerator As IDictionaryEnumerator = e.NewValues.GetEnumerator()
        enumerator.Reset()
        While enumerator.MoveNext()
            Dim currentkey As String = ""
            If enumerator.Key IsNot Nothing Then currentkey = enumerator.Key.ToString()
            Select Case currentkey
                Case "OriginalDate"
                    dr.Item("OriginalDate") = roTypes.Any2DateTime(enumerator.Value).Date
                Case "DecimalValue"
                    dr.Item("DecimalValue") = enumerator.Value
                    dr.Item("Value") = enumerator.Value.ToString().Replace(HelperWeb.GetDecimalDigitFormat(), ".")
                Case "DateValue"
                    If (enumerator.Value IsNot Nothing) Then
                        dr.Item("DateValue") = enumerator.Value
                        dr.Item("Value") = enumerator.Value
                    Else
                        dr.Item("DateValue") = System.DBNull.Value
                        dr.Item("Value") = System.DBNull.Value
                    End If

                Case "TimeValue"
                    If (enumerator.Value IsNot Nothing) Then
                        dr.Item("TimeValue") = enumerator.Value
                        dr.Item("Value") = roConversions.ConvertTimeToHours(enumerator.Value).ToString().Replace(HelperWeb.GetDecimalDigitFormat(), ".")
                    Else
                        dr.Item("TimeValue") = System.DBNull.Value
                        dr.Item("Value") = System.DBNull.Value
                    End If
                Case "InitialDateTime"
                    If (enumerator.Value IsNot Nothing) Then
                        dr.Item("InitialDateTime") = enumerator.Value
                        Dim newVal As String = ""
                        newVal = Format(CDate(enumerator.Value), "yyyy/MM/dd") & "*"
                        If dr.Item("EndDateTime") IsNot DBNull.Value Then newVal = newVal & Format(roTypes.Any2DateTime(dr.Item("EndDateTime")), "yyyy/MM/dd")
                        dr.Item("Value") = newVal
                    Else
                        dr.Item("InitialDateTime") = System.DBNull.Value
                        Dim newVal As String = "*"
                        If dr.Item("EndDateTime") IsNot DBNull.Value Then newVal = newVal & Format(roTypes.Any2DateTime(dr.Item("EndDateTime")), "yyyy/MM/dd")
                        dr.Item("Value") = newVal
                    End If
                Case "EndDateTime"
                    If (enumerator.Value IsNot Nothing) Then
                        dr.Item("EndDateTime") = enumerator.Value

                        Dim newVal As String = ""
                        If dr.Item("InitialDateTime") IsNot DBNull.Value Then newVal = newVal & Format(roTypes.Any2DateTime(dr.Item("InitialDateTime")), "yyyy/MM/dd")
                        newVal = newVal & "*" & Format(CDate(enumerator.Value), "yyyy/MM/dd")
                        dr.Item("Value") = newVal
                    Else
                        dr.Item("EndDateTime") = System.DBNull.Value
                        Dim newVal As String = "*"
                        If dr.Item("InitialDateTime") IsNot DBNull.Value Then newVal = Format(roTypes.Any2DateTime(dr.Item("InitialDateTime")), "yyyy/MM/dd") & "*"
                        dr.Item("Value") = newVal
                    End If
                Case "StartHourPeriod"
                    If (enumerator.Value IsNot Nothing) Then
                        dr.Item("StartHourPeriod") = enumerator.Value

                        Dim newVal As String = ""
                        newVal = Format(CDate(enumerator.Value), "HH:mm") & "*"
                        If dr.Item("EndHourPeriod") IsNot DBNull.Value Then newVal = newVal & Format(roTypes.Any2DateTime(dr.Item("EndHourPeriod")), "HH:mm")
                        dr.Item("Value") = newVal
                    Else
                        dr.Item("StartHourPeriod") = System.DBNull.Value

                        Dim newVal As String = "*"
                        If dr.Item("EndHourPeriod") IsNot DBNull.Value Then newVal = newVal & Format(roTypes.Any2DateTime(dr.Item("EndHourPeriod")), "HH:mm")
                        dr.Item("Value") = newVal
                    End If
                Case "EndHourPeriod"
                    If (enumerator.Value IsNot Nothing) Then
                        dr.Item("EndHourPeriod") = enumerator.Value

                        Dim newVal As String = ""
                        If dr.Item("StartHourPeriod") IsNot DBNull.Value Then newVal = newVal & Format(roTypes.Any2DateTime(dr.Item("StartHourPeriod")), "HH:mm")
                        newVal = newVal & "*" & Format(CDate(enumerator.Value), "HH:mm")
                        dr.Item("Value") = newVal
                    Else
                        dr.Item("EndHourPeriod") = System.DBNull.Value

                        Dim newVal As String = "*"
                        If dr.Item("StartHourPeriod") IsNot DBNull.Value Then newVal = Format(roTypes.Any2DateTime(dr.Item("StartHourPeriod")), "HH:mm") & "*"
                        dr.Item("Value") = newVal
                    End If
                Case "LinkShow"
                    dr.Item("LinkShow") = enumerator.Value
                    dr.Item("Value") = roTypes.Any2String(enumerator.Value) & "*" & roTypes.Any2String(dr.Item("LinkReal"))
                Case "LinkReal"
                    dr.Item("LinkReal") = enumerator.Value
                    dr.Item("Value") = roTypes.Any2String(dr.Item("LinkShow")) & "*" & roTypes.Any2String(enumerator.Value)
                Case "Value"
                    If (enumerator.Value IsNot Nothing) Then
                        dr.Item("Value") = enumerator.Value
                    Else
                        dr.Item("Value") = System.DBNull.Value
                    End If
            End Select

        End While

        Me.HistoryData = tb
        e.Cancel = True
        grid.CancelEdit()

    End Sub

    Protected Sub GridHistoric_RowDeleting(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataDeletingEventArgs) Handles GridHistoric.RowDeleting
        Dim tb As DataTable = Me.HistoryData()
        'Dim i As String = GridIncidences.FindVisibleIndexByKeyValue(e.Keys(GridIncidences.KeyFieldName))
        Dim dr As DataRow = tb.Rows.Find(e.Keys(GridHistoric.KeyFieldName))
        dr.Delete()
        Me.HistoryData = tb
        e.Cancel = True
    End Sub

    Protected Sub btAccept_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btAccept.Click
        Dim bolRet As Boolean = False

        Select Case Me.oType
            Case Types.EmployeeField
                bolRet = API.EmployeeServiceMethods.SaveUserFieldHistory(Me, Me.intIDSource, Me.HistoryData)
            Case Types.GroupField
                bolRet = False
        End Select

        If bolRet Then
            Me.CanClose = True
            Me.MustRefresh = "1"
        End If
    End Sub

#End Region

End Class