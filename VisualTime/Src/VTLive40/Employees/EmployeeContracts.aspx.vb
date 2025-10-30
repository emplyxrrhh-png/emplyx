Imports DevExpress.Web
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTEmployees.Contract
Imports Robotics.Base.VTEmployees.LabAgree
Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class Forms_EmployeeContracts
    Inherits PageBase

    Private Const FeatureAlias As String = "Employees.Contract"

#Region "Declarations"

    Private intEmployeeID As Integer

    Private oPermission As Permission
    Private oCurrentPermission As Permission

    Private isLabAgree As Boolean = False
    Private isDocuments As Boolean = False

#End Region

#Region "Properties"

    Private Property ContractsData(Optional ByVal bolReload As Boolean = False) As DataTable
        Get
            Dim tb As DataTable = Session("EmployeeContracts_EmployeeContractsData")

            If bolReload OrElse tb Is Nothing Then
                tb = API.ContractsServiceMethods.GetContractsByIDEmployee(Me, intEmployeeID, True)

                If tb IsNot Nothing Then
                    tb.Columns.Add(New DataColumn("ID", GetType(Integer)))
                    tb.Columns.Add(New DataColumn("EndDateVisible", GetType(Date)))
                    tb.Columns.Add(New DataColumn("ImagePath", GetType(String)))
                    tb.Columns.Add(New DataColumn("InitialIDContract", GetType(String)))
                End If

                If tb IsNot Nothing AndAlso tb.Rows.Count = 0 Then
                    Dim oRow As DataRow = tb.NewRow
                    oRow("ID") = 1
                    oRow("BeginDate") = New Date(2079, 1, 1)
                    oRow("EndDate") = New Date(2079, 1, 1)
                    oRow("EndDateVisible") = New Date(2079, 1, 1)
                    oRow("IDCard") = -999999
                    oRow("IDLabAgree") = -1
                    oRow("InitialIDContract") = ""
                    oRow("IDEmployee") = Me.intEmployeeID
                    tb.Rows.Add(oRow)
                Else
                    Dim index As Integer = 1
                    For Each oRow As DataRow In tb.Rows
                        oRow("ID") = index
                        oRow("InitialIDContract") = oRow("IDContract")
                        If oRow("EndDate") = New Date(2079, 1, 1) Then
                            oRow("EndDateVisible") = DBNull.Value
                        Else
                            oRow("EndDateVisible") = oRow("EndDate")
                        End If

                        If CDate(oRow("BeginDate")).Date <= Now.Date AndAlso CDate(oRow("EndDate")).Date >= Now.Date Then
                            oRow("ImagePath") = Me.Page.ResolveUrl("~/Base/Images/Grid/Play.gif")
                        Else
                            oRow("ImagePath") = Me.Page.ResolveUrl("~/Base/Images/Grid/Stop.gif")
                        End If

                        index = index + 1
                    Next
                End If

                If tb IsNot Nothing Then
                    tb.PrimaryKey = New DataColumn() {tb.Columns("ID")}
                    tb.AcceptChanges()
                End If

                Session("EmployeeContracts_EmployeeContractsData") = tb
            End If
            Return tb
        End Get
        Set(value As DataTable)
            Session("EmployeeContracts_EmployeeContractsData") = value
        End Set
    End Property

    Private Property LabAgreedData() As DataView
        Get

            Dim tbCauses As DataTable = Session("EmployeeContracts_LabAgreedData")
            Dim dv As DataView = Nothing
            If tbCauses IsNot Nothing Then
                dv = New DataView(tbCauses)
                dv.Sort = "Name ASC"
            End If

            If dv Is Nothing Then

                Dim tb As DataTable = API.LabAgreeServiceMethods.GetLabAgrees(Me.Page)

                If tb IsNot Nothing Then

                    Dim oNewRow As DataRow = tb.NewRow()
                    oNewRow("Name") = ""
                    oNewRow("ID") = -1
                    tb.Rows.Add(oNewRow)

                    dv = New DataView(tb)
                    dv.Sort = "Name ASC"

                    Session("EmployeeContracts_LabAgreedData") = dv.Table

                End If

            End If

            Return dv

        End Get
        Set(ByVal value As DataView)
            If value IsNot Nothing Then
                Session("EmployeeContracts_LabAgreedData") = value.Table
            Else
                Session("EmployeeContracts_LabAgreedData") = Nothing
            End If
        End Set
    End Property

#End Region

#Region "Events"

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("BrowserDetect", "~/Base/Scripts/BrowserDetect.js", , True)
        Me.InsertExtraJavascript("Cookies", "~/Base/Scripts/Cookies.js", , True)
        Me.InsertExtraJavascript("Generic", "~/Base/Scripts/Generic.js", , True)

        Me.InsertExtraJavascript("ContractScheduleRules", "~/Employees/Scripts/ContractScheduleRules.js", , True)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Me.InsertCssIncludes()
        Me.InsertExtraCssIncludes("~/Base/ext-3.4.0/resources/css/ext-all.css")

        Dim EmployeeID As String = Request.Params("EmployeeID")
        If EmployeeID IsNot Nothing AndAlso EmployeeID.Length > 0 Then
            Me.intEmployeeID = CInt(EmployeeID)
        End If

        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)
        Me.oCurrentPermission = Me.GetFeaturePermissionByEmployee(FeatureAlias, Me.intEmployeeID)

        If Me.oCurrentPermission > Permission.None Then

            Me.isLabAgree = isLabAgreeVisible()
            'isDocuments = isDocumentsVisible()

            If Not Me.IsPostBack Then
                CreateColumnsContracts()

                Me.ContractsData = Nothing
                Me.LabAgreedData = Nothing
                BindGridContracts(True)
            Else
                BindGridContracts(False)
            End If

            AddHandler Me.MessageFrame1.OptionOnClick, AddressOf OnMessageClick
        Else
            WLHelperWeb.RedirectAccessDenied(True)
        End If

    End Sub

    Protected Sub OnMessageClick(ByVal strButtonKey As String)

    End Sub

#End Region

#Region "Grid Contracts"

    Private Sub BindGridContracts(ByVal bolReload As Boolean)
        Me.GridContracts.DataSource = Me.ContractsData(bolReload)
        Me.GridContracts.DataBind()

        Try
            If Not GridContracts.JSProperties.ContainsKey("cpActionRO") Then GridContracts.JSProperties.Add("cpActionRO", "")
        Catch ex As Exception
            GridContracts.JSProperties("cpActionRO") = ""
        End Try

    End Sub

    Protected Sub btnAddNewContract_Init(ByVal sender As Object, ByVal e As EventArgs)
        Dim txtLabel As ASPxButton = CType(sender, ASPxButton)
        txtLabel.Text = Me.Language.Translate("Button.Title.NewContract", DefaultScope)
        txtLabel.ToolTip = ""
    End Sub

    Protected Sub lblContractsCaption_Init(ByVal sender As Object, ByVal e As EventArgs)
        Dim txtLabel As ASPxLabel = CType(sender, ASPxLabel)
        txtLabel.Text = Me.Language.Translate("grdContracts.Title", DefaultScope)
        txtLabel.ToolTip = ""
    End Sub

    Private Sub CreateColumnsContracts()

        Dim GridColumn As GridViewDataTextColumn
        Dim GridColumnCommand As GridViewCommandColumn
        Dim GridColumnCombo As GridViewDataComboBoxColumn
        Dim GridColumnDate As GridViewDataDateColumn
        Dim GridColumnImage As GridViewDataImageColumn
        Dim CustomButton As GridViewCommandColumnCustomButton

        Dim VisibleIndex As Integer = 0

        Me.GridContracts.Columns.Clear()
        Me.GridContracts.KeyFieldName = "ID"
        Me.GridContracts.SettingsText.EmptyDataRow = " "
        Me.GridContracts.Settings.VerticalScrollBarMode = ScrollBarMode.Auto

        Dim uiControl As DevExpress.Web.ASPxButton = CType(Me.GridContracts.FindTitleTemplateControl("btnAddNewContract"), DevExpress.Web.ASPxButton)

        If Me.oPermission = Permission.Admin Or Me.oPermission >= Permission.Write Then
            Me.GridContracts.SettingsEditing.Mode = GridViewEditingMode.Inline
        Else
            If uiControl IsNot Nothing Then
                uiControl.Visible = False
            End If
        End If

        If Me.oPermission >= Permission.Write Then
            'Command buttons
            GridColumnCommand = New GridViewCommandColumn()
            GridColumnCommand.ButtonRenderMode = GridCommandButtonRenderMode.Image
            GridColumnCommand.ShowDeleteButton = False
            GridColumnCommand.ShowEditButton = True

            GridColumnCommand.ShowCancelButton = False
            GridColumnCommand.ShowUpdateButton = False

            GridColumnCommand.Caption = " "
            GridColumnCommand.VisibleIndex = VisibleIndex
            GridColumnCommand.CellStyle.HorizontalAlign = HorizontalAlign.Center
            GridColumnCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
            GridColumnCommand.Width = 16
            VisibleIndex = VisibleIndex + 1

            CustomButton = New GridViewCommandColumnCustomButton()
            CustomButton.ID = "UpdateContractRow"
            CustomButton.Image.Url = "~/Base/Images/Grid/save.png"
            CustomButton.Image.ToolTip = " " 'Me.Language.Translate("GridContracts.Column.SaveRow", DefaultScope)
            CustomButton.Image.Height = New Unit(16)
            CustomButton.Image.Width = New Unit(16)
            CustomButton.Visibility = GridViewCustomButtonVisibility.EditableRow
            GridColumnCommand.CustomButtons.Add(CustomButton)

            If Me.isLabAgree Then
                CustomButton = New GridViewCommandColumnCustomButton()
                CustomButton.ID = "EditScheduleRowsRow"
                CustomButton.Image.Url = "~/Base/Images/Grid/Attachment.png"
                CustomButton.Image.ToolTip = Me.Language.Translate("GridContracts.Column.EditLabAgreeScheduleRules", DefaultScope)
                CustomButton.Image.Height = New Unit(16)
                CustomButton.Image.Width = New Unit(16)
                GridColumnCommand.CustomButtons.Add(CustomButton)
            End If

            Me.GridContracts.Columns.Add(GridColumnCommand)
        End If

        'Clave
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "ID"
        GridColumn.FieldName = "ID"
        GridColumn.ReadOnly = False
        GridColumn.Visible = False
        Me.GridContracts.Columns.Add(GridColumn)

        GridColumnImage = New GridViewDataImageColumn
        GridColumnImage.Caption = " "
        GridColumnImage.FieldName = "ImagePath"
        GridColumnImage.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnImage.ReadOnly = True
        GridColumnImage.PropertiesImage.ImageWidth = 10
        GridColumnImage.PropertiesImage.ImageHeight = 10
        GridColumnImage.EditFormSettings.Visible = DevExpress.Utils.DefaultBoolean.False
        GridColumnImage.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnImage.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnImage.Width = 10
        Me.GridContracts.Columns.Add(GridColumnImage)

        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = Me.Language.Translate("GridContracts.Column.IDContract", DefaultScope) '"Valor"
        GridColumn.FieldName = "IDContract"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = False
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Width = 20
        Me.GridContracts.Columns.Add(GridColumn)

        'Date
        GridColumnDate = New GridViewDataDateColumn()
        GridColumnDate.Caption = Me.Language.Translate("GridContracts.Column.BeginDate", DefaultScope) '"Fecha"
        GridColumnDate.FieldName = "BeginDate"
        GridColumnDate.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnDate.ReadOnly = False
        GridColumnDate.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnDate.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnDate.Width = 25
        Me.GridContracts.Columns.Add(GridColumnDate)

        'Date
        GridColumnDate = New GridViewDataDateColumn()
        GridColumnDate.Caption = Me.Language.Translate("GridContracts.Column.EndDate", DefaultScope) '"Fecha"
        GridColumnDate.FieldName = "EndDateVisible"
        GridColumnDate.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnDate.ReadOnly = False
        GridColumnDate.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnDate.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnDate.Width = 25
        Me.GridContracts.Columns.Add(GridColumnDate)

        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = Me.Language.Translate("GridContracts.Column.Enterprise", DefaultScope) '"Valor"
        GridColumn.FieldName = "Enterprise"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = False
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Width = 35
        Me.GridContracts.Columns.Add(GridColumn)

        If Me.isLabAgree Then
            'Date
            GridColumnCombo = New GridViewDataComboBoxColumn()
            GridColumnCombo.Caption = Me.Language.Translate("GridContracts.Column.LabAgree", DefaultScope) '"Fecha"
            GridColumnCombo.FieldName = "IDLabAgree"
            GridColumnCombo.VisibleIndex = VisibleIndex
            GridColumnCombo.PropertiesComboBox.DataSource = LabAgreedData
            GridColumnCombo.PropertiesComboBox.TextField = "Name"
            GridColumnCombo.PropertiesComboBox.ValueField = "ID"
            GridColumnCombo.PropertiesComboBox.ValueType = GetType(Integer)
            GridColumnCombo.PropertiesComboBox.RequireDataBinding()

            VisibleIndex = VisibleIndex + 1
            GridColumnCombo.ReadOnly = Not isLabAgree
            GridColumnCombo.CellStyle.HorizontalAlign = HorizontalAlign.Left
            GridColumnCombo.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
            GridColumnCombo.Width = 35
            Me.GridContracts.Columns.Add(GridColumnCombo)
        End If
        If Me.oPermission >= Permission.Write Then
            'Command buttons
            GridColumnCommand = New GridViewCommandColumn()
            GridColumnCommand.ButtonRenderMode = GridCommandButtonRenderMode.Image
            GridColumnCommand.ShowDeleteButton = False
            GridColumnCommand.ShowEditButton = False

            GridColumnCommand.ShowCancelButton = True
            GridColumnCommand.ShowUpdateButton = False

            GridColumnCommand.Caption = " "
            GridColumnCommand.VisibleIndex = VisibleIndex
            GridColumnCommand.CellStyle.HorizontalAlign = HorizontalAlign.Center
            GridColumnCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
            GridColumnCommand.Width = 10
            VisibleIndex = VisibleIndex + 1

            CustomButton = New GridViewCommandColumnCustomButton()
            CustomButton.ID = "DeleteContractRow"
            CustomButton.Image.Url = "~/Base/Images/Grid/remove.png"
            CustomButton.Image.ToolTip = " " 'Me.Language.Translate("GridContracts.Column.DeleteRow", DefaultScope)
            CustomButton.Image.Height = New Unit(16)
            CustomButton.Image.Width = New Unit(16)
            GridColumnCommand.CustomButtons.Add(CustomButton)

            Me.GridContracts.Columns.Add(GridColumnCommand)
        End If
    End Sub

    Private Sub GridContracts_DataBinding(sender As Object, e As EventArgs) Handles GridContracts.DataBinding
        Dim oCombo As GridViewDataComboBoxColumn = GridContracts.Columns("IDLabAgree")
        If oCombo IsNot Nothing Then
            oCombo.PropertiesComboBox.DataSource = LabAgreedData
            oCombo.PropertiesComboBox.TextField = "Name"
            oCombo.PropertiesComboBox.ValueField = "ID"
            oCombo.PropertiesComboBox.ValueType = GetType(Integer)
        End If
    End Sub

    Protected Sub GridContracts_CustomCallback(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxGridViewCustomCallbackEventArgs) Handles GridContracts.CustomCallback
        If e.Parameters = "REFRESH" Then
            BindGridContracts(False)

            GridContracts.JSProperties("cpActionRO") = "REFRESH"
        ElseIf e.Parameters = "RELOAD" Then
            BindGridContracts(True)

            GridContracts.JSProperties("cpActionRO") = "RELOAD"
        End If
    End Sub

    Protected Sub GridContracts_StartRowEditing(sender As Object, e As Data.ASPxStartRowEditingEventArgs) Handles GridContracts.StartRowEditing

    End Sub

    Protected Sub GridContracts_InitNewRow(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataInitNewRowEventArgs) Handles GridContracts.InitNewRow
        Dim tb As DataTable = Me.ContractsData()
        If (tb.Rows.Count > 0) Then
            e.NewValues("ID") = roTypes.Any2Integer(tb.Rows(tb.Rows.Count - 1)("ID")) + 1
        Else
            e.NewValues("ID") = 1
        End If

        e.NewValues("BeginDate") = New Date
        e.NewValues("EndDate") = New Date(2079, 1, 1)
        e.NewValues("IDCard") = -999999
        e.NewValues("IDLabAgree") = -1

        GridContracts.JSProperties("cpActionRO") = "INITROW"
    End Sub

    Protected Sub GridContracts_RowInserting(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataInsertingEventArgs) Handles GridContracts.RowInserting
        Dim grid As ASPxGridView = CType(sender, ASPxGridView)

        If Not e.NewValues Is Nothing Then
            Dim newID As Integer

            Dim tb As DataTable = Me.ContractsData()
            If (tb.Rows.Count > 0) Then
                newID = roTypes.Any2Integer(tb.Rows(tb.Rows.Count - 1)("ID")) + 1
            Else
                newID = 1
            End If

            Dim oNewRow As DataRow = tb.NewRow
            With oNewRow
                .Item("ID") = newID
                .Item("IDEmployee") = Me.intEmployeeID
                .Item("IDContract") = roTypes.Any2String(e.NewValues("IDContract")).Trim
                .Item("BeginDate") = e.NewValues("BeginDate")
                If e.NewValues("EndDateVisible") IsNot Nothing Then
                    .Item("EndDateVisible") = e.NewValues("EndDateVisible")
                    .Item("EndDate") = e.NewValues("EndDateVisible")
                Else
                    .Item("EndDateVisible") = DBNull.Value
                    .Item("EndDate") = New Date(2079, 1, 1)
                End If

                If e.NewValues("Enterprise") IsNot Nothing Then
                    .Item("Enterprise") = e.NewValues("Enterprise")
                Else
                    .Item("Enterprise") = String.Empty
                End If

                If e.NewValues("IDLabAgree") Is Nothing Then
                    .Item("IdLabAgree") = DBNull.Value
                Else
                    .Item("IDLabAgree") = e.NewValues("IDLabAgree")
                End If

                If CDate(.Item("BeginDate")).Date <= Now.Date AndAlso CDate(.Item("EndDate")).Date >= Now.Date Then
                    .Item("ImagePath") = Me.Page.ResolveUrl("~/Base/Images/Grid/Play.gif")
                Else
                    .Item("ImagePath") = Me.Page.ResolveUrl("~/Base/Images/Grid/Stop.gif")
                End If
            End With

            Dim oContract As New roContract
            oContract.BeginDate = oNewRow("BeginDate")
            oContract.EndDate = oNewRow("EndDate")
            oContract.IDContract = oNewRow("IDContract")
            oContract.Enterprise = oNewRow("Enterprise")
            oContract.IDCard = -999999
            oContract.IDEmployee = oNewRow("IDEmployee")
            oContract.OriginalIDContract = "#######"

            If oNewRow("IDLabAgree") IsNot Nothing AndAlso Not IsDBNull(oNewRow("IDLabAgree")) AndAlso oNewRow("IDLabAgree") <> -1 Then
                Dim oLabAgree As New roLabAgree
                oLabAgree.ID = oNewRow("IDLabAgree")
                oContract.LabAgree = oLabAgree
            Else
                oContract.LabAgree = Nothing
            End If

            If API.ContractsServiceMethods.SaveContract(Me, oContract, True) Then
                tb.Rows.Add(oNewRow)
                Me.ContractsData = tb
                e.Cancel = True
                grid.CancelEdit()
            Else
                Throw New Exception(roWsUserManagement.SessionObject.States.ContractState.ErrorText)
            End If

        End If

        GridContracts.JSProperties("cpActionRO") = "ROWINSERTING"
    End Sub

    Protected Sub GridContracts_RowUpdating(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataUpdatingEventArgs) Handles GridContracts.RowUpdating

        Dim tb As DataTable = Me.ContractsData()
        Dim dr As DataRow = tb.Rows.Find(e.Keys(GridContracts.KeyFieldName))
        Dim grid As ASPxGridView = CType(sender, ASPxGridView)

        Dim enumerator As IDictionaryEnumerator = e.NewValues.GetEnumerator()
        enumerator.Reset()
        While enumerator.MoveNext()
            Dim currentkey As String = ""
            If enumerator.Key IsNot Nothing Then currentkey = enumerator.Key.ToString()
            Select Case currentkey
                Case "IDContract"
                    If enumerator.Value IsNot Nothing Then
                        dr.Item("IDContract") = roTypes.Any2String(enumerator.Value).Trim
                    Else
                        dr.Item("IDContract") = String.Empty
                    End If
                Case "BeginDate"
                    dr.Item("BeginDate") = enumerator.Value
                    If CDate(dr.Item("BeginDate")).Date <= Now.Date AndAlso CDate(dr.Item("EndDate")).Date >= Now.Date Then
                        dr.Item("ImagePath") = Me.Page.ResolveUrl("~/Base/Images/Grid/Play.gif")
                    Else
                        dr.Item("ImagePath") = Me.Page.ResolveUrl("~/Base/Images/Grid/Stop.gif")
                    End If
                Case "EndDateVisible"
                    If enumerator.Value Is Nothing Then
                        dr.Item("EndDateVisible") = DBNull.Value
                        dr.Item("EndDate") = New Date(2079, 1, 1)
                    Else
                        dr.Item("EndDateVisible") = enumerator.Value
                        dr.Item("EndDate") = enumerator.Value
                    End If

                    If CDate(dr.Item("BeginDate")).Date <= Now.Date AndAlso CDate(dr.Item("EndDate")).Date >= Now.Date Then
                        dr.Item("ImagePath") = Me.Page.ResolveUrl("~/Base/Images/Grid/Play.gif")
                    Else
                        dr.Item("ImagePath") = Me.Page.ResolveUrl("~/Base/Images/Grid/Stop.gif")
                    End If

                Case "Enterprise"
                    If enumerator.Value IsNot Nothing Then
                        dr.Item("Enterprise") = enumerator.Value
                    Else
                        dr.Item("Enterprise") = String.Empty
                    End If

                Case "IDLabAgree"
                    If enumerator.Value IsNot Nothing Then
                        dr.Item("IDLabAgree") = enumerator.Value
                    Else
                        dr.Item("IDLabAgree") = -1
                    End If

            End Select

        End While

        Dim oContract As New roContract
        oContract.BeginDate = dr.Item("BeginDate")
        oContract.EndDate = dr.Item("EndDate")

        If dr.Item("IDContract") <> String.Empty Then
            oContract.IDContract = dr.Item("IDContract")
        End If

        oContract.Enterprise = dr.Item("Enterprise")
        oContract.IDCard = roTypes.Any2Long(dr.Item("IDCard"))
        oContract.IDEmployee = dr.Item("IDEmployee")

        If Not IsDBNull(dr.Item("InitialIDContract")) Then
            oContract.OriginalIDContract = dr.Item("InitialIDContract")
        Else
            oContract.OriginalIDContract = "#######"
        End If

        If dr.Item("IDLabAgree") IsNot Nothing AndAlso Not IsDBNull(dr.Item("IDLabAgree")) AndAlso dr.Item("IDLabAgree") <> -1 Then
            Dim oLabAgree As New roLabAgree
            oLabAgree.ID = dr.Item("IDLabAgree")
            oContract.LabAgree = oLabAgree
        Else
            oContract.LabAgree = Nothing
        End If

        If API.ContractsServiceMethods.SaveContract(Me, oContract, True) Then
            dr.Item("InitialIDContract") = oContract.IDContract
            Me.ContractsData = tb
            e.Cancel = True
            grid.CancelEdit()
        Else
            oContract.State.Result = ContractsResultEnum.InvalidIDContract
            'Throw New Exception("Mojon") 'roWsUserManagement.SessionObject.States.ContractState.ErrorText)
        End If

        GridContracts.JSProperties("cpActionRO") = "ROWUPDATING"
    End Sub

    Protected Sub GridContracts_CustomErrorText(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxGridViewCustomErrorTextEventArgs) Handles GridContracts.CustomErrorText
        e.ErrorText = roWsUserManagement.SessionObject.States.ContractState.ErrorText
    End Sub

    Protected Sub GridContracts_CommandButtonInitialize(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxGridViewCommandButtonEventArgs) Handles GridContracts.CommandButtonInitialize
        Dim tb As DataTable = Me.ContractsData()

        If CountActiveRows(tb) = 1 AndAlso e.ButtonType = ColumnCommandButtonType.Delete Then
            e.Visible = False
        End If

        If e.ButtonType = DevExpress.Web.ColumnCommandButtonType.Update Then
            e.Visible = False
        End If

        GridContracts.JSProperties("cpActionRO") = "CUSTOMBUTTON"
    End Sub

    Protected Sub GridContracts_CustomButtonInitialize(sender As Object, e As ASPxGridViewCustomButtonEventArgs) Handles GridContracts.CustomButtonInitialize
        Dim tb As DataTable = Me.ContractsData()

        If e.IsEditingRow AndAlso e.ButtonID = "DeleteContractRow" Then
            e.Visible = DevExpress.Utils.DefaultBoolean.False
            e.Enabled = False
        End If

        If CountActiveRows(tb) = 1 AndAlso e.ButtonID = "DeleteContractRow" Then
            e.Visible = DevExpress.Utils.DefaultBoolean.False
            e.Enabled = False
        End If

        GridContracts.JSProperties("cpActionRO") = "CUSTOMBUTTON"
    End Sub

    Protected Sub GridContracts_RowDeleting(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataDeletingEventArgs) Handles GridContracts.RowDeleting
        Dim tb As DataTable = Me.ContractsData()
        'Dim i As String = GridIncidences.FindVisibleIndexByKeyValue(e.Keys(GridIncidences.KeyFieldName))
        Dim dr As DataRow = tb.Rows.Find(e.Keys(GridContracts.KeyFieldName))

        If tb.Rows.Count > 1 Then
            If API.ContractsServiceMethods.DeleteContract(Me, dr("IDContract"), True) Then
                dr.Delete()
                tb.AcceptChanges()
                Me.ContractsData = tb
                e.Cancel = True
            Else
                Throw New Exception(roWsUserManagement.SessionObject.States.ContractState.ErrorText)
            End If
        End If

        GridContracts.JSProperties("cpActionRO") = "DELETE"
    End Sub

#End Region

#Region "Methods"

    Private Function CountActiveRows(ByVal dData As DataTable) As Integer
        Dim iActiveRows As Integer = 0

        For Each oRow As DataRow In dData.Rows
            If oRow.RowState <> DataRowState.Deleted Then
                iActiveRows = iActiveRows + 1
            End If
        Next

        Return iActiveRows
    End Function

    Public Function isLabAgreeVisible() As Boolean
        ' Miramos si hay la licencia de convenios activada
        Dim oPermission As Permission = Permission.None
        ' Miramos si el passport actual tiene permisos para visualizar los convenios del empleado
        If Me.HasFeaturePermissionByEmployee("LabAgree", Permission.Read, Me.intEmployeeID) Then
            ' Miramos si el passport actual tiene permisos para modificar la asignación del convenio para el empleado actual.
            oPermission = Me.GetFeaturePermissionByEmployee("LabAgree.EmployeeAssign", Me.intEmployeeID)
        End If

        Return oPermission >= Permission.Read
    End Function

#End Region

End Class