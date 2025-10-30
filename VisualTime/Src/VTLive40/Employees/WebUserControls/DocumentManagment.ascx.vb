Imports DevExpress.Web
Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Public Class DocumentManagment
    Inherits UserControlBase

    Public Sub New()

    End Sub

    Private Property ScopeData() As DataView
        Get
            Dim tbCauses As DataTable = Session("DocumentTemplates_ScopeData")
            Dim dv As DataView = Nothing
            If tbCauses IsNot Nothing Then
                dv = New DataView(tbCauses)
                dv.Sort = "Name ASC"
            End If
            If dv Is Nothing Then
                Dim tb As New DataTable
                tb.Columns.Add(New DataColumn("ID", GetType(Integer)))
                tb.Columns.Add(New DataColumn("Name", GetType(String)))
                tb.AcceptChanges()

                For Each eType As DocumentScope In System.Enum.GetValues(GetType(DocumentScope))
                    Dim oNewRow As DataRow = tb.NewRow()
                    oNewRow("Name") = Language.Translate("Scope.Short." & eType.ToString, DefaultScope)
                    oNewRow("ID") = CInt(eType)

                    tb.Rows.Add(oNewRow)

                    dv = New DataView(tb)
                    dv.Sort = "Name ASC"
                    Session("DocumentTemplates_ScopeData") = dv.Table
                Next
            End If
            Return dv
        End Get
        Set(ByVal value As DataView)
            If value IsNot Nothing Then
                Session("DocumentTemplates_ScopeData") = value.Table
            Else
                Session("DocumentTemplates_ScopeData") = Nothing
            End If
        End Set
    End Property

    Private Property AreaData() As DataView
        Get
            Dim tbCauses As DataTable = Session("DocumentTemplates_TypeData")
            Dim dv As DataView = Nothing
            If tbCauses IsNot Nothing Then
                dv = New DataView(tbCauses)
                dv.Sort = "Name ASC"
            End If
            If dv Is Nothing Then
                Dim tb As New DataTable
                tb.Columns.Add(New DataColumn("ID", GetType(Integer)))
                tb.Columns.Add(New DataColumn("Name", GetType(String)))
                tb.AcceptChanges()

                For Each eArea As DocumentArea In System.Enum.GetValues(GetType(DocumentArea))
                    Dim oNewRow As DataRow = tb.NewRow()
                    oNewRow("Name") = Language.Translate("Area." & eArea.ToString, DefaultScope)
                    oNewRow("ID") = CInt(eArea)

                    tb.Rows.Add(oNewRow)

                    dv = New DataView(tb)
                    dv.Sort = "Name ASC"
                    Session("DocumentTemplates_TypeData") = dv.Table
                Next
            End If
            Return dv
        End Get
        Set(ByVal value As DataView)
            If value IsNot Nothing Then
                Session("DocumentTemplates_TypeData") = value.Table
            Else
                Session("DocumentTemplates_TypeData") = Nothing
            End If
        End Set
    End Property

    Public Property IdAbsence() As Integer
        Get
            Return Session(Me.ClientID & "DocumentManagment_IdAbsence")
        End Get

        Set(ByVal value As Integer)
            Session(Me.ClientID & "DocumentManagment_IdAbsence") = value
        End Set
    End Property

    Public Property Forecast() As ForecastType
        Get
            Return Session(Me.ClientID & "DocumentManagment_ForecastType")
        End Get

        Set(ByVal value As ForecastType)
            Session(Me.ClientID & "DocumentManagment_ForecastType") = value
        End Set
    End Property

    Public Property IdRelatedObject() As Integer
        Get
            Return Session(Me.ClientID & "DocumentManagment_IdEmployee")
        End Get

        Set(ByVal value As Integer)
            Session(Me.ClientID & "DocumentManagment_IdEmployee") = value
        End Set
    End Property

    Public Property Type() As DocumentType
        Get
            Return Session(Me.ClientID & "DocumentManagment_Type")
        End Get

        Set(ByVal value As DocumentType)
            Session(Me.ClientID & "DocumentManagment_Type") = value
        End Set
    End Property

    Public ReadOnly Property HasDocuments() As Boolean
        Get
            Return (Me.Documents IsNot Nothing AndAlso Me.Documents.Count > 0)
        End Get
    End Property

    Public Property Documents() As List(Of roDocument)
        Get
            Return Session(Me.ClientID & "DocumentManagment_Documents")
        End Get

        Set(ByVal value As List(Of roDocument))
            Session(Me.ClientID & "DocumentManagment_Documents") = value
        End Set
    End Property

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        hdnScopeInfo.ClientInstanceName = ClientID & "_hdnScopeInfoClient"
        'CreateDeliveredDocsColumns()
        'BindGridDocuments(False)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            If (Me.IdAbsence <= 0) Then Documents = Nothing
            CreateDeliveredDocsColumns()
            Dim gridDocumentsName = ClientID & "_GridDocsClient"
            GridDeliveredDocs.ClientInstanceName = gridDocumentsName
            GridDeliveredDocs.ClientSideEvents.CustomButtonClick = gridDocumentsName & "_CustomButtonClick"
            GridDeliveredDocs.ClientSideEvents.EndCallback = gridDocumentsName & "_EndCallback"
            GridDeliveredDocs.ClientSideEvents.BeginCallback = gridDocumentsName & "_BeginCallback"
            Me.EditDocumentPopup.ClientInstanceName = ClientID & "_EditDocumentPopup"
            Me.NewDocumentPopup.ClientInstanceName = ClientID & "_NewDocumentPopup"
            Me.CaptchaObjectPopup.ClientInstanceName = ClientID & "_CaptchaObjectPopup"
        End If

        BindGridDocuments(False)
    End Sub

    Public Sub SetScope(idRelatedObject As Integer, typeCaller As DocumentType, ByVal idAbsence As Integer, ByVal eForecast As ForecastType)
        Me.IdAbsence = idAbsence
        Me.IdRelatedObject = idRelatedObject
        Me.Forecast = eForecast

        Type = typeCaller
        If (hdnScopeInfo.Contains("IdRelatedObject")) Then
            hdnScopeInfo("IdRelatedObject") = Me.IdRelatedObject
        Else
            hdnScopeInfo.Add("IdRelatedObject", Me.IdRelatedObject)
        End If

        If (hdnScopeInfo.Contains("Type")) Then
            hdnScopeInfo("Type") = Type
        Else
            hdnScopeInfo.Add("Type", Type)
        End If

        If (hdnScopeInfo.Contains("eForecast")) Then
            hdnScopeInfo("eForecast") = eForecast
        Else
            hdnScopeInfo.Add("eForecast", eForecast)
        End If

        Me.btnAddDoc.Attributes("onclick") = ClientID & "_AddNewDocument('" & Me.IdRelatedObject & "',-1," & Type & "," & Me.IdAbsence & ", " & Me.Forecast & ");"

        CreateDeliveredDocsColumns()
        BindGridDocuments(True)
    End Sub

    Private Sub CreateDeliveredDocsColumns()

        Dim GridColumn As GridViewDataColumn
        Dim GridColumnDate As GridViewDataDateColumn
        Dim GridColumnCommand As GridViewCommandColumn
        Dim CustomButton As GridViewCommandColumnCustomButton
        Dim GridColumnImage As GridViewDataImageColumn
        Dim GridColumnCombo As GridViewDataComboBoxColumn
        Dim VisibleIndex As Integer = 0

        Me.GridDeliveredDocs.Columns.Clear()

        Me.GridDeliveredDocs.KeyFieldName = "Id"
        Me.GridDeliveredDocs.SettingsText.EmptyDataRow = " "
        Me.GridDeliveredDocs.Settings.VerticalScrollBarMode = ScrollBarMode.Auto

        If (Me.Forecast = ForecastType.Any OrElse Me.Forecast = ForecastType.AnyAbsence) Then
            Me.GridDeliveredDocs.Settings.ShowFilterRow = True
            Me.GridDeliveredDocs.Settings.VerticalScrollableHeight = 200
        Else
            Me.GridDeliveredDocs.Settings.ShowFilterRow = False
            Me.GridDeliveredDocs.Settings.VerticalScrollableHeight = 85
        End If

        'ID
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Language.Translate("DeliveredDocs.Column.ID", DefaultScope)
        GridColumn.FieldName = "Id"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.Visible = False
        Me.GridDeliveredDocs.Columns.Add(GridColumn)

        'Status
        GridColumnImage = New GridViewDataImageColumn
        GridColumnImage.Caption = Me.Language.Translate("DeliveredDocs.Column.State", DefaultScope) '"Estado"
        GridColumnImage.FieldName = "ImgStatus"
        GridColumnImage.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnImage.ReadOnly = True
        GridColumnImage.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnImage.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnImage.Width = Unit.Percentage(5)
        GridColumnImage.UnboundType = DevExpress.Data.UnboundColumnType.String
        GridColumnImage.PropertiesImage.ImageAlign = ImageAlign.Middle
        GridColumnImage.PropertiesImage.ImageHeight = 19
        GridColumnImage.PropertiesImage.ImageWidth = 19
        GridColumnImage.PropertiesImage.ImageUrlFormatString = "../../Base/Images/{0}"
        Me.GridDeliveredDocs.Columns.Add(GridColumnImage)

        'Título
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Language.Translate("DeliveredDocs.Column.Title", DefaultScope)
        GridColumn.FieldName = "Title"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.Width = Unit.Percentage(11)
        Me.GridDeliveredDocs.Columns.Add(GridColumn)

        'Tipo
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("DeliveredDocs.Column.Type", DefaultScope) '"Centro"
        GridColumn.FieldName = "DocumentTemplate.Name"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.Width = Unit.Percentage(9)
        GridColumn.CellStyle.Wrap = False
        GridColumn.Settings.AutoFilterCondition = AutoFilterCondition.Contains
        Me.GridDeliveredDocs.Columns.Add(GridColumn)

        'Ambito
        GridColumnCombo = New DevExpress.Web.GridViewDataComboBoxColumn()
        GridColumnCombo.Caption = Me.Language.Translate("DocumentTemplatesGrid.Column.Scope", DefaultScope) '"Scope"
        GridColumnCombo.FieldName = "DocumentTemplate.Scope"
        GridColumnCombo.VisibleIndex = VisibleIndex

        GridColumnCombo.PropertiesComboBox.DataSource = ScopeData
        GridColumnCombo.PropertiesComboBox.TextField = "Name"
        GridColumnCombo.PropertiesComboBox.ValueField = "ID"
        GridColumnCombo.PropertiesComboBox.RequireDataBinding()

        VisibleIndex = VisibleIndex + 1
        GridColumnCombo.ReadOnly = True
        GridColumnCombo.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnCombo.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnCombo.Width = Unit.Percentage(7)
        GridColumnCombo.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnCombo.Settings.AutoFilterCondition = AutoFilterCondition.Contains
        Me.GridDeliveredDocs.Columns.Add(GridColumnCombo)

        'Area
        GridColumnCombo = New DevExpress.Web.GridViewDataComboBoxColumn()
        GridColumnCombo.Caption = Me.Language.Translate("DocumentTemplatesGrid.Column.Area", DefaultScope) '"Area"
        GridColumnCombo.FieldName = "DocumentTemplate.Area"
        GridColumnCombo.VisibleIndex = VisibleIndex

        GridColumnCombo.PropertiesComboBox.DataSource = AreaData
        GridColumnCombo.PropertiesComboBox.TextField = "Name"
        GridColumnCombo.PropertiesComboBox.ValueField = "ID"
        GridColumnCombo.PropertiesComboBox.RequireDataBinding()

        VisibleIndex = VisibleIndex + 1
        GridColumnCombo.ReadOnly = True
        GridColumnCombo.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnCombo.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnCombo.Width = Unit.Percentage(7)
        GridColumnCombo.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnCombo.Settings.AutoFilterCondition = AutoFilterCondition.Contains
        Me.GridDeliveredDocs.Columns.Add(GridColumnCombo)

        'Entregado
        GridColumn = New GridViewDataDateColumn
        GridColumn.Caption = Language.Translate("DeliveredDocs.Column.DeliveredDate", DefaultScope) '"Tarea"
        GridColumn.FieldName = "DeliveredDate"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.Width = Unit.Percentage(7)
        GridColumn.CellStyle.Wrap = False
        GridColumn.Settings.AutoFilterCondition = AutoFilterCondition.Contains
        Me.GridDeliveredDocs.Columns.Add(GridColumn)

        'Estado
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Language.Translate("DeliveredDocs.Column.Status", DefaultScope) ' "Duración"
        GridColumn.FieldName = "Status"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.Width = Unit.Percentage(6)
        GridColumn.Settings.FilterMode = ColumnFilterMode.DisplayText
        Me.GridDeliveredDocs.Columns.Add(GridColumn)

        'Estado
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Language.Translate("DeliveredDocs.Column.SignStatus", DefaultScope) ' "Duración"
        GridColumn.FieldName = "SignStatus"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.Width = Unit.Percentage(6)
        GridColumn.Settings.FilterMode = ColumnFilterMode.DisplayText
        Me.GridDeliveredDocs.Columns.Add(GridColumn)

        'Nivel
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Language.Translate("DeliveredDocs.Column.Level", DefaultScope) ' "Duración"
        GridColumn.FieldName = "StatusLevel"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.Width = Unit.Percentage(5)
        GridColumn.Settings.FilterMode = ColumnFilterMode.DisplayText
        Me.GridDeliveredDocs.Columns.Add(GridColumn)

        'ValidatedDate
        GridColumnDate = New GridViewDataDateColumn
        GridColumnDate.Caption = Language.Translate("DeliveredDocs.Column.ValidatedDate", DefaultScope) '"Inicio"
        GridColumnDate.FieldName = "LastStatusChange"
        GridColumnDate.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnDate.ReadOnly = True
        GridColumnDate.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnDate.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnDate.Width = Unit.Percentage(7)
        Me.GridDeliveredDocs.Columns.Add(GridColumnDate)

        'Valido desde
        GridColumnDate = New GridViewDataDateColumn
        GridColumnDate.Caption = Language.Translate("DeliveredDocs.Column.BeginDate", DefaultScope)  '"Fin"
        GridColumnDate.FieldName = "BeginDate"
        GridColumnDate.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnDate.ReadOnly = True
        GridColumnDate.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnDate.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnDate.Width = Unit.Percentage(7)
        Me.GridDeliveredDocs.Columns.Add(GridColumnDate)

        'Valido hasta
        GridColumnDate = New GridViewDataDateColumn
        GridColumnDate.Caption = Language.Translate("DeliveredDocs.Column.EndDate", DefaultScope)  '"Fin"
        GridColumnDate.FieldName = "EndDate"
        GridColumnDate.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnDate.ReadOnly = True
        GridColumnDate.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnDate.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnDate.Width = Unit.Percentage(7)
        Me.GridDeliveredDocs.Columns.Add(GridColumnDate)

        'Obligatorio
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Language.Translate("DeliveredDocs.Column.CurrentlyValid", DefaultScope) ' "Duración"
        GridColumn.FieldName = "Validity"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.Width = Unit.Percentage(6)
        GridColumn.Settings.FilterMode = ColumnFilterMode.DisplayText
        Me.GridDeliveredDocs.Columns.Add(GridColumn)

        ''Botonos de acciones
        'Command buttons
        GridColumnCommand = New GridViewCommandColumn()
        GridColumnCommand.ButtonRenderMode = GridCommandButtonRenderMode.Image
        GridColumnCommand.ShowDeleteButton = False
        GridColumnCommand.ShowEditButton = False
        GridColumnCommand.ShowCancelButton = False
        GridColumnCommand.ShowUpdateButton = True
        GridColumnCommand.Caption = " "
        GridColumnCommand.VisibleIndex = VisibleIndex
        GridColumnCommand.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCommand.Width = Unit.Percentage(10)
        VisibleIndex = VisibleIndex + 1

        CustomButton = New GridViewCommandColumnCustomButton()
        CustomButton.ID = "EditDocument"
        CustomButton.Image.Url = "~/Base/Images/Grid/edit.png"
        CustomButton.Image.ToolTip = Me.Language.Translate("GridDeliveredDocs.Column.EditRow", DefaultScope)
        CustomButton.Image.Height = New Unit(16)
        CustomButton.Image.Width = New Unit(16)
        GridColumnCommand.CustomButtons.Add(CustomButton)

        CustomButton = New GridViewCommandColumnCustomButton()
        CustomButton.ID = "ViewDocument"
        CustomButton.Image.Url = "~/Base/Images/Grid/download.png"
        CustomButton.Image.ToolTip = Me.Language.Translate("GridDeliveredDocs.Column.ViewDoc", DefaultScope)
        CustomButton.Image.Height = New Unit(16)
        CustomButton.Image.Width = New Unit(16)
        GridColumnCommand.CustomButtons.Add(CustomButton)

        CustomButton = New GridViewCommandColumnCustomButton()
        CustomButton.ID = "ViewSign"
        CustomButton.Image.Url = "~/Base/Images/Grid/icons8-sign-document-64.png"
        CustomButton.Image.ToolTip = Me.Language.Translate("GridDeliveredDocs.Column.ViewDoc", DefaultScope)
        CustomButton.Image.Height = New Unit(16)
        CustomButton.Image.Width = New Unit(16)
        GridColumnCommand.CustomButtons.Add(CustomButton)

        CustomButton = New GridViewCommandColumnCustomButton()
        CustomButton.ID = "DeleteDocument"
        CustomButton.Image.Url = "~/Base/Images/Grid/remove.png"
        CustomButton.Image.ToolTip = Me.Language.Translate("GridDeliveredDocs.Column.DeleteRow", DefaultScope)
        CustomButton.Image.Height = New Unit(16)
        CustomButton.Image.Width = New Unit(16)
        GridColumnCommand.CustomButtons.Add(CustomButton)

        Me.GridDeliveredDocs.Columns.Add(GridColumnCommand)

        Me.GridDeliveredDocs.SettingsResizing.ColumnResizeMode = ColumnResizeMode.NextColumn
        Me.GridDeliveredDocs.SettingsResizing.Visualization = ResizingMode.Live

    End Sub

    Private Sub BindGridDocuments(ByVal bolReload As Boolean)
        If (bolReload) Then
            Documents = DocumentsServiceMethods.GetDocumentsByType(Me.IdRelatedObject, Type, Page, False)

            If (Me.Forecast = ForecastType.AbsenceDays) Then
                Documents = Documents.FindAll(Function(x) x.IdDaysAbsence = Me.IdAbsence)
            ElseIf (Me.Forecast = ForecastType.AbsenceHours) Then
                Documents = Documents.FindAll(Function(x) x.IdHoursAbsence = Me.IdAbsence)
            ElseIf (Me.Forecast = ForecastType.OverWork) Then
                Documents = Documents.FindAll(Function(x) x.IdOvertimeForecast = Me.IdAbsence)
            ElseIf (Me.Forecast = ForecastType.Any) Then
            Else
                Documents = New List(Of roDocument)
            End If

            GridDeliveredDocs.DataSource = Documents
        Else
            GridDeliveredDocs.DataSource = Documents
        End If
        GridDeliveredDocs.DataBind()
    End Sub

    Private Sub GridDeliveredDocs_DataBinding(sender As Object, e As EventArgs) Handles GridDeliveredDocs.DataBinding
        Dim oCombo As GridViewDataComboBoxColumn = GridDeliveredDocs.Columns("DocumentTemplate.Scope")
        oCombo.PropertiesComboBox.DataSource = Me.ScopeData()
        oCombo.PropertiesComboBox.TextField = "Name"
        oCombo.PropertiesComboBox.ValueField = "ID"

        oCombo = GridDeliveredDocs.Columns("DocumentTemplate.Area")
        oCombo.PropertiesComboBox.DataSource = Me.AreaData()
        oCombo.PropertiesComboBox.TextField = "Name"
        oCombo.PropertiesComboBox.ValueField = "ID"
    End Sub

    Protected Sub GridDeliveredDocs_CustomCallback(ByVal sender As Object, ByVal e As ASPxGridViewCustomCallbackEventArgs) Handles GridDeliveredDocs.CustomCallback
        Dim strParameters = e.Parameters.Split(";")
        If strParameters(0) = "RELOAD" Then
            Me.IdRelatedObject = hdnScopeInfo("IdRelatedObject")
            Type = CType([Enum].Parse(GetType(DocumentType), hdnScopeInfo("Type")), DocumentType)
            BindGridDocuments(True)
        ElseIf strParameters(0).Equals("DELETE") Then
            Me.IdRelatedObject = hdnScopeInfo("IdRelatedObject")
            Type = CType([Enum].Parse(GetType(DocumentType), hdnScopeInfo("Type")), DocumentType)
            If (DocumentsServiceMethods.DeleteDocument(Me.Page, roTypes.Any2Integer(strParameters(1)), True)) Then
                BindGridDocuments(True)
            End If
        End If
    End Sub

    Protected Sub GridDeliveredDocs_CustomButtonInitialize(ByVal sender As Object, ByVal e As ASPxGridViewCustomButtonEventArgs) Handles GridDeliveredDocs.CustomButtonInitialize

        If e.ButtonID = "EditDocument" OrElse e.ButtonID = "DeleteDocument" Then
            Dim IDDocument As Integer = roTypes.Any2Integer(GridDeliveredDocs.GetRowValues(e.VisibleIndex, "Id"))

            If Not DocumentsServiceMethods.CanEditDocument(IDDocument, Me.Page, False) Then
                e.Visible = DevExpress.Utils.DefaultBoolean.False
            End If

        End If

        If e.ButtonID = "ViewSign" Then
            Dim status As Integer = GridDeliveredDocs.GetRowValues(e.VisibleIndex, "SignStatus")

            If status = SignStatusEnum.Signed Then
                e.Visible = DevExpress.Utils.DefaultBoolean.True
            Else
                e.Visible = DevExpress.Utils.DefaultBoolean.False
            End If

        End If

    End Sub

    Protected Sub GridDeliveredDocs_CustomColumnDisplayText(ByVal sender As Object, ByVal e As ASPxGridViewColumnDisplayTextEventArgs) Handles GridDeliveredDocs.CustomColumnDisplayText
        If e.Column.FieldName = "Status" Then
            Select Case roTypes.Any2String(e.Value)
                Case DocumentStatus.Pending.ToString
                    e.DisplayText = Language.Translate(DocumentStatus.Pending.ToString, DefaultScope)
                Case DocumentStatus.Validated.ToString
                    e.DisplayText = Language.Translate(DocumentStatus.Validated.ToString, DefaultScope)
                Case DocumentStatus.Invalidated.ToString
                    e.DisplayText = Language.Translate(DocumentStatus.Invalidated.ToString, DefaultScope)
                Case DocumentStatus.Rejected.ToString
                    e.DisplayText = Language.Translate(DocumentStatus.Rejected.ToString, DefaultScope)
            End Select
        ElseIf e.Column.FieldName = "Validity" Then
            Select Case roTypes.Any2String(e.Value)
                Case DocumentValidity.CheckPending.ToString
                    e.DisplayText = Language.Translate(DocumentValidity.CheckPending.ToString, DefaultScope)
                Case DocumentValidity.CurrentlyValid.ToString
                    e.DisplayText = Language.Translate(DocumentValidity.CurrentlyValid.ToString, DefaultScope)
                Case DocumentValidity.NotEnoughAuthorityLevel.ToString
                    e.DisplayText = Language.Translate(DocumentValidity.NotEnoughAuthorityLevel.ToString, DefaultScope)
                Case DocumentValidity.ValidOnFuture.ToString
                    e.DisplayText = Language.Translate(DocumentValidity.ValidOnFuture.ToString, DefaultScope)
                Case DocumentValidity.Invalid.ToString
                    e.DisplayText = Language.Translate(DocumentValidity.Invalid.ToString, DefaultScope)
            End Select
        ElseIf e.Column.FieldName = "SignStatus" Then
            Select Case roTypes.Any2String(e.Value)
                Case SignStatusEnum.Pending.ToString
                    e.DisplayText = Language.Translate(SignStatusEnum.Pending.ToString, DefaultScope)
                Case SignStatusEnum.InProgress.ToString
                    e.DisplayText = Language.Translate(SignStatusEnum.InProgress.ToString, DefaultScope)
                Case SignStatusEnum.Rejected.ToString
                    e.DisplayText = Language.Translate(SignStatusEnum.Rejected.ToString, DefaultScope)
                Case SignStatusEnum.Signed.ToString
                    e.DisplayText = Language.Translate(SignStatusEnum.Signed.ToString, DefaultScope)
                Case SignStatusEnum.NA.ToString
                    e.DisplayText = ""
            End Select

        ElseIf e.Column.FieldName = "BeginDate" Then
            If (roTypes.Any2DateTime(e.Value).Equals(New Date(1900, 1, 1))) Then
                e.DisplayText = String.Empty
            End If
        ElseIf e.Column.FieldName = "EndDate" Then
            If (roTypes.Any2DateTime(e.Value).Equals(New Date(2079, 1, 1))) Then
                e.DisplayText = String.Empty
            End If
        ElseIf e.Column.FieldName = "StatusLevel" Then
            If roTypes.Any2Integer(e.Value) = 0 OrElse roTypes.Any2Integer(e.Value) >= 13 Then
                e.DisplayText = String.Empty
            End If
        End If
    End Sub

    Protected Sub GridDeliveredDocs_CustomUnboundColumnData(ByVal sender As Object, ByVal e As ASPxGridViewColumnDataEventArgs) Handles GridDeliveredDocs.CustomUnboundColumnData
        Select Case e.Column.FieldName
            Case "ImgStatus"
                If e.IsGetData Then
                    If e.GetListSourceFieldValue("Status") IsNot DBNull.Value Then
                        Select Case roTypes.Any2String(e.GetListSourceFieldValue("Status"))
                            Case DocumentStatus.Pending.ToString
                                e.Value = "PendienteConfirmacion.png"
                            Case DocumentStatus.Validated.ToString
                                If e.GetListSourceFieldValue("Validity") IsNot DBNull.Value Then
                                    If e.GetListSourceFieldValue("Validity") = DocumentValidity.NotEnoughAuthorityLevel Then
                                        e.Value = "PendienteConfirmacion.png"
                                    Else
                                        e.Value = "Finalizada.png"
                                    End If
                                Else
                                    e.Value = "Finalizada.png"
                                End If
                            Case DocumentStatus.Invalidated.ToString
                                e.Value = "Cancelada.png"
                            Case DocumentStatus.Rejected.ToString
                                e.Value = "Rechazado.png"
                            Case DocumentStatus.Expired.ToString
                                If e.GetListSourceFieldValue("Validity") IsNot DBNull.Value Then
                                    If e.GetListSourceFieldValue("Validity") = DocumentValidity.CurrentlyValid Then
                                        e.Value = "Finalizada.png"
                                    Else
                                        e.Value = "Expirada.png"
                                    End If
                                Else
                                    e.Value = "Expirada.png"
                                End If
                        End Select
                    Else
                        e.Value = "PendienteConfirmacion.png"
                    End If
                End If
        End Select
    End Sub

End Class