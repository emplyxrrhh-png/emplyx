Imports DevExpress.Web
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTNotifications.Notifications
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Public Class DocumentTemplate
    Inherits PageBase

    <Runtime.Serialization.DataContract()>
    Private Class DocumentsCallbackRequest

        <Runtime.Serialization.DataMember(Name:="aTab")>
        Public aTab As Integer

        <Runtime.Serialization.DataMember(Name:="ID")>
        Public ID As Integer

        <Runtime.Serialization.DataMember(Name:="oType")>
        Public Type As String

        <Runtime.Serialization.DataMember(Name:="Name")>
        Public Name As String

        <Runtime.Serialization.DataMember(Name:="action")>
        Public Action As String

        <Runtime.Serialization.DataMember(Name:="StampParam")>
        Public StampParam As Double

    End Class

    Private Const FeatureAlias As String = "Documents.DocumentsDefinition"
    Private Const FeatureEmployees As String = ""
    Private oPermission As Permission

#Region "Properties"

    Private ReadOnly Property NotificationsDataTable(Optional ByVal bolReload As Boolean = False) As List(Of roNotification)
        Get
            Dim tbAvailableNotifications As List(Of roNotification) = Session("DocumentTemplates_AvailableNotifications")

            If bolReload OrElse tbAvailableNotifications Is Nothing Then

                tbAvailableNotifications = API.NotificationServiceMethods.GetNotifications(Me.Page, " ID between 700 and 750 ", True, False)
                Session("DocumentTemplates_AvailableNotifications") = tbAvailableNotifications
            End If

            Return tbAvailableNotifications

        End Get
    End Property

#End Region

#Region "Grid document templates"

    Protected Sub ASPxbtnExportGrid_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Me.ASPxGridViewExporter1.GridViewID = Me.GridDocumentTemplate.ID
        Me.ASPxGridViewExporter1.WriteXlsxToResponse(True)
    End Sub

    Private Sub GridDocumentTemplate_DataBinding(sender As Object, e As EventArgs) Handles GridDocumentTemplate.DataBinding
        Dim oCombo As GridViewDataComboBoxColumn = GridDocumentTemplate.Columns("Scope")
        oCombo.PropertiesComboBox.DataSource = Me.ScopeData()
        oCombo.PropertiesComboBox.TextField = "Name"
        oCombo.PropertiesComboBox.ValueField = "ID"
        oCombo.PropertiesComboBox.ValueType = GetType(Short)

        oCombo = GridDocumentTemplate.Columns("Area")
        oCombo.PropertiesComboBox.DataSource = Me.AreaData()
        oCombo.PropertiesComboBox.TextField = "Name"
        oCombo.PropertiesComboBox.ValueField = "ID"
        oCombo.PropertiesComboBox.ValueType = GetType(Short)
    End Sub

    Protected Sub LinqServerModeDataSource1_Selecting(ByVal sender As Object, ByVal e As DevExpress.Data.Linq.LinqServerModeDataSourceSelectEventArgs) Handles LinqServerModeDataSource1.Selecting
        'enlazar con datatable en runtime http://www.devexpress.com/Support/Center/p/E168.aspx
        e.QueryableSource = GetDocumentTemplates()
    End Sub

    Private Function GetDocumentTemplates() As IQueryable
        Dim qry As IQueryable = Nothing
        Try
            Dim sAux As String = String.Empty

            If hdnFilterStatus.Contains("StatusFilter") Then
                sAux = hdnFilterStatus("StatusFilter").Substring(0, 5)
            Else
                sAux = "11111"
            End If

            Dim VerPendientes As Boolean = (sAux(0) = "1")
            Dim VerValidadas As Boolean = (sAux(1) = "1")
            Dim VerExpiradas As Boolean = (sAux(2) = "1")
            Dim VerRechazdas As Boolean = (sAux(3) = "1")
            Dim VerInvalidaddas As Boolean = (sAux(4) = "1")
            Dim oPassport As roPassportTicket = WLHelperWeb.CurrentPassport()

            If oPassport IsNot Nothing Then
                Dim oList() As Integer = Nothing

                Dim tmpList As List(Of roDocumentTemplate) = DocumentsServiceMethods.GetTemplateDocumentsList(False, DocumentScope.EmployeeContract, Me.Page, True).FindAll(Function(x) x.IsSystem = False)
                qry = tmpList.AsQueryable
            End If
        Catch ex As Exception
        End Try
        Return qry
    End Function

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
                    If eType <> DocumentScope.Communique AndAlso eType <> DocumentScope.BioCertificate Then
                        Dim oNewRow As DataRow = tb.NewRow()
                        oNewRow("Name") = Language.Translate("Scope.Short." & eType.ToString, DefaultScope)
                        oNewRow("ID") = CInt(eType)

                        tb.Rows.Add(oNewRow)
                    End If

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

    Private Sub CreateColumns()

        Dim GridColumn As GridViewDataColumn
        Dim GridColumnCombo As GridViewDataComboBoxColumn
        Dim GridColumnDate As GridViewDataDateColumn
        Dim GridColumncheck As GridViewDataCheckColumn
        Dim GridColumnCommand As GridViewCommandColumn

        Dim VisibleIndex As Integer = 0

        Me.GridDocumentTemplate.Columns.Clear()
        Me.GridDocumentTemplate.KeyFieldName = "Id"

        Me.GridDocumentTemplate.Settings.ShowFilterRow = True
        Me.GridDocumentTemplate.Settings.ShowFooter = True
        Me.GridDocumentTemplate.Settings.UseFixedTableLayout = True

        Me.GridDocumentTemplate.SettingsBehavior.AllowSelectSingleRowOnly = True
        Me.GridDocumentTemplate.SettingsBehavior.AllowFocusedRow = True

        Me.GridDocumentTemplate.SettingsPager.PageSize = 18
        Me.GridDocumentTemplate.SettingsPager.ShowEmptyDataRows = True

        'Me.GridTareas.Settings.ShowVerticalScrollBar = True
        'Me.GridTareas.Settings.VerticalScrollableHeight = 0

        Me.GridDocumentTemplate.ClientSideEvents.RowDblClick = "function(s, e) { GetDocumentTemplateDetails(s,e); }"

        'Me.GridTareas.ClientSideEvents.Init = "Function(s, e) { adjustSize(); }"
        'Me.GridTareas.ClientSideEvents.EndCallback = "Function(s, e) { adjustSize(); }"

        'ID
        GridColumn = New DevExpress.Web.GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("DocumentTemplatesGrid.Column.ID", DefaultScope)
        GridColumn.FieldName = "Id"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.Width = 30
        Me.GridDocumentTemplate.Columns.Add(GridColumn)

        'Ambito
        GridColumnCombo = New DevExpress.Web.GridViewDataComboBoxColumn()
        GridColumnCombo.Caption = Me.Language.Translate("DocumentTemplatesGrid.Column.Scope", DefaultScope) '"Scope"
        GridColumnCombo.FieldName = "Scope"
        GridColumnCombo.VisibleIndex = VisibleIndex

        GridColumnCombo.PropertiesComboBox.DataSource = ScopeData
        GridColumnCombo.PropertiesComboBox.TextField = "Name"
        GridColumnCombo.PropertiesComboBox.ValueField = "ID"
        GridColumnCombo.PropertiesComboBox.ValueType = GetType(Integer)
        GridColumnCombo.PropertiesComboBox.RequireDataBinding()

        VisibleIndex = VisibleIndex + 1
        GridColumnCombo.ReadOnly = True
        GridColumnCombo.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnCombo.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnCombo.Width = 65
        GridColumnCombo.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnCombo.Settings.AutoFilterCondition = AutoFilterCondition.Contains
        Me.GridDocumentTemplate.Columns.Add(GridColumnCombo)

        'Area
        GridColumnCombo = New DevExpress.Web.GridViewDataComboBoxColumn()
        GridColumnCombo.Caption = Me.Language.Translate("DocumentTemplatesGrid.Column.Area", DefaultScope) '"Area"
        GridColumnCombo.FieldName = "Area"
        GridColumnCombo.VisibleIndex = VisibleIndex

        GridColumnCombo.PropertiesComboBox.DataSource = AreaData
        GridColumnCombo.PropertiesComboBox.TextField = "Name"
        GridColumnCombo.PropertiesComboBox.ValueField = "ID"
        GridColumnCombo.PropertiesComboBox.ValueType = GetType(Integer)
        GridColumnCombo.PropertiesComboBox.RequireDataBinding()

        VisibleIndex = VisibleIndex + 1
        GridColumnCombo.ReadOnly = True
        GridColumnCombo.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnCombo.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnCombo.Width = 65
        GridColumnCombo.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnCombo.Settings.AutoFilterCondition = AutoFilterCondition.Contains
        Me.GridDocumentTemplate.Columns.Add(GridColumnCombo)

        'ShortName
        GridColumn = New DevExpress.Web.GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("DocumentTemplatesGrid.Column.ShortName", DefaultScope) '"Abrev."
        GridColumn.FieldName = "ShortName"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.CellStyle.Wrap = DevExpress.Utils.DefaultBoolean.False
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.Width = 20
        GridColumn.Settings.AutoFilterCondition = AutoFilterCondition.Contains
        Me.GridDocumentTemplate.Columns.Add(GridColumn)

        'Name
        GridColumn = New DevExpress.Web.GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("DocumentTemplatesGrid.Column.DocumentTemplate", DefaultScope) '"Tarea"
        GridColumn.FieldName = "Name"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.CellStyle.Wrap = DevExpress.Utils.DefaultBoolean.False
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Width = 50
        GridColumn.Settings.AutoFilterCondition = AutoFilterCondition.Contains
        Me.GridDocumentTemplate.Columns.Add(GridColumn)

        'Proyecto
        GridColumn = New DevExpress.Web.GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("DocumentTemplatesGrid.Column.Description", DefaultScope) '"Proyecto"
        GridColumn.FieldName = "Description"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Width = 100
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Settings.AutoFilterCondition = AutoFilterCondition.Contains
        Me.GridDocumentTemplate.Columns.Add(GridColumn)

        'FechaIni y hora ini
        GridColumnDate = New DevExpress.Web.GridViewDataDateColumn
        GridColumnDate.Caption = Me.Language.Translate("DocumentTemplatesGrid.Column.ExpectedStartDate", DefaultScope) '"Inicio"
        GridColumnDate.FieldName = "BeginValidity"
        GridColumnDate.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnDate.ReadOnly = True
        GridColumnDate.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnDate.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnDate.Width = 75
        Me.GridDocumentTemplate.Columns.Add(GridColumnDate)

        'FechaFin y hora fin
        GridColumnDate = New DevExpress.Web.GridViewDataDateColumn
        GridColumnDate.Caption = Me.Language.Translate("DocumentTemplatesGrid.Column.ExpectedEndDate", DefaultScope)  '"Fin"
        GridColumnDate.FieldName = "EndValidity"
        GridColumnDate.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnDate.ReadOnly = True
        GridColumnDate.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnDate.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnDate.Width = 75
        Me.GridDocumentTemplate.Columns.Add(GridColumnDate)

        'Obligatorio
        GridColumncheck = New GridViewDataCheckColumn
        GridColumncheck.Caption = Me.Language.Translate("DocumentTemplatesGrid.Column.Mandatory", DefaultScope) 'Fecha"
        GridColumncheck.FieldName = "Compulsory"
        GridColumncheck.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumncheck.ReadOnly = True
        GridColumncheck.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumncheck.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumncheck.Settings.AllowHeaderFilter = DevExpress.Utils.DefaultBoolean.True
        GridColumncheck.Settings.ShowFilterRowMenu = DevExpress.Utils.DefaultBoolean.False
        GridColumncheck.Width = 25
        Me.GridDocumentTemplate.Columns.Add(GridColumncheck)

        'Puede empleado
        GridColumncheck = New GridViewDataCheckColumn
        GridColumncheck.Caption = Me.Language.Translate("DocumentTemplatesGrid.Column.EmployeeDeliverAllowed", DefaultScope) 'Fecha"
        GridColumncheck.FieldName = "EmployeeDeliverAllowed"
        GridColumncheck.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumncheck.ReadOnly = True
        GridColumncheck.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumncheck.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumncheck.Settings.AllowHeaderFilter = DevExpress.Utils.DefaultBoolean.True
        GridColumncheck.Settings.ShowFilterRowMenu = DevExpress.Utils.DefaultBoolean.False
        GridColumncheck.Width = 25
        Me.GridDocumentTemplate.Columns.Add(GridColumncheck)

        'Puede supervisor
        GridColumncheck = New GridViewDataCheckColumn
        GridColumncheck.Caption = Me.Language.Translate("DocumentTemplatesGrid.Column.SupervisorDeliverAllowed", DefaultScope) 'Fecha"
        GridColumncheck.FieldName = "SupervisorDeliverAllowed"
        GridColumncheck.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumncheck.ReadOnly = True
        GridColumncheck.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumncheck.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumncheck.Settings.AllowHeaderFilter = DevExpress.Utils.DefaultBoolean.True
        GridColumncheck.Settings.ShowFilterRowMenu = DevExpress.Utils.DefaultBoolean.False
        GridColumncheck.Width = 25
        Me.GridDocumentTemplate.Columns.Add(GridColumncheck)

        If Me.oPermission > Permission.Read Then
            GridColumnCommand = New GridViewCommandColumn()
            GridColumnCommand.ButtonRenderMode = GridCommandButtonRenderMode.Image

            Dim customDeleteButton As GridViewCommandColumnCustomButton = New GridViewCommandColumnCustomButton()
            customDeleteButton.ID = "GridDocuments_DeleteSelected"
            customDeleteButton.Image.Url = "~/Base/Images/Grid/remove.png"
            customDeleteButton.Text = Me.Language.Translate("AbsenceTypes.ShowDetails", DefaultScope) 'Mostrar detalles"

            GridColumnCommand.CustomButtons.Add(customDeleteButton)
            GridColumnCommand.ShowEditButton = False
            GridColumnCommand.ShowDeleteButton = False
            GridColumnCommand.Caption = " "
            GridColumnCommand.VisibleIndex = VisibleIndex
            GridColumnCommand.CellStyle.HorizontalAlign = HorizontalAlign.Center
            GridColumnCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
            GridColumnCommand.Width = 15
            VisibleIndex = VisibleIndex + 1

            Me.GridDocumentTemplate.Columns.Add(GridColumnCommand)
        End If

    End Sub

#End Region

#Region "Eventos"

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Init
        InsertExtraJavascript("Ajax", "~/Base/Scripts/Ajax.js")
        InsertExtraJavascript("jsDate", "~/Base/Scripts/jsDate.js")
        InsertExtraJavascript("GenericData", "~/Base/Scripts/GenericData.js")
        InsertExtraJavascript("webUserForms", "~/Base/Scripts/webUserForms.js")
        InsertExtraJavascript("jsGrid", "~/Base/Scripts/jsGrid.js")
        InsertExtraJavascript("roOptionPanelClient", "~/Base/Scripts/roOptionPanelClient.js")
        InsertExtraJavascript("roOptPanelClientGroup", "~/Base/Scripts/roOptPanelClientGroup.js")
        InsertExtraJavascript("jsDatePicker", "~/Base/Scripts/jsDatePicker.js")
        InsertExtraJavascript("roTabContainerClient", "~/Base/Scripts/roTabContainerClient.js")

        InsertExtraJavascript("Documents", "~/Documents/Scripts/Documents.js")
        InsertExtraJavascript("DocumentTemplate", "~/Documents/Scripts/DocumentTemplate.js")
        InsertExtraJavascript("GridDocuments", "~/Documents/Scripts/GridDocuments.js")

    End Sub

    Private Sub LoadDocumentTemplateDataTab()
        Try
            Dim guiActions As Generic.List(Of roGuiAction) = PortalServiceMethods.GetGuiActions(Nothing, "Portal\GeneralManagement\Documents\Template", WLHelperWeb.CurrentPassportID)
            Dim destDiv As HtmlGenericControl = roTools.BuildCentralBar(guiActions, roTypes.Any2Integer(Me.IDLoadDocumentTemplate.Value), Me.Language, Me.DefaultScope, "",, roTools.ToolBarDirection.Horizontal)
            Me.tbButtons.Controls.Clear()
            Me.tbButtons.Controls.Add(destDiv)
        Catch ex As Exception
            Me.tbButtons.InnerHtml = ex.Message.ToString
        End Try
    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        ' Comprueba la licencia
        If HelperSession.GetFeatureIsInstalledFromApplication("Feature\Documents") = False AndAlso HelperSession.GetFeatureIsInstalledFromApplication("Feature\Absences") = False Then
            WLHelperWeb.RedirectAccessDenied(False)
            Exit Sub
        End If

        oPermission = GetFeaturePermission(FeatureAlias)
        If Me.oPermission = Permission.None Then
            WLHelperWeb.RedirectAccessDenied(False)
            Exit Sub
        End If

        Me.lblSaveChanges.InnerText = Me.Language.Translate("msgSaveChangesbar", DefaultScope)
        Me.lblUndoChanges.InnerText = Me.Language.Translate("msgUndoChangesbar", DefaultScope)

        Me.msgHasChanges.Value = Me.Language.Translate("msgHasChanges", DefaultScope)
        Me.msgHasErrors.Value = Me.Language.Translate("msgHasErrors", DefaultScope)

        If Not hdnFilterStatus.Contains("StatusFilter") Then
            hdnFilterStatus.Set("StatusFilter", "11111")
        End If

        lblArea.Style("margin-top") = "11px;"
        lblDocValidity.Style("margin-top") = "11px;"
        lblDocumentNotifications.Style("margin-top") = "11px;"

        If Not IsPostBack AndAlso Not IsCallback Then
            If Not Request.QueryString("IDdocumentTemplate") Is Nothing Then
                Me.IDLoadDocumentTemplate.Value = Request.QueryString("IDdocumentTemplate")
            End If

            LoadRadioButtonList()

            CreateColumns()
            LoadDocumentTemplateDataTab()
        End If
    End Sub

    Protected Sub ASPxCallbackPanelContenido_Callback(ByVal sender As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase) Handles ASPxCallbackPanelContenido.Callback
        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Dim oParameters As New DocumentsCallbackRequest()
        oParameters = roJSONHelper.Deserialize(strParameter, oParameters.GetType())

        Select Case oParameters.Type
            Case "D"
                ProcessDocumentRequest(oParameters)
        End Select
    End Sub

#End Region

#Region "Plantillas de documentos"

    Private Sub ProcessDocumentRequest(ByVal oParameters As DocumentsCallbackRequest)
        oPermission = GetFeaturePermission(FeatureAlias)
        If oPermission > Permission.None Then
            Select Case oParameters.Action
                Case "GETDOCUMENT"
                    LoadDocumentTemplate(oParameters)
                Case "SAVEDOCUMENT"
                    SaveDocumentTemplate(oParameters)
            End Select

            ProcessDocumentSelectedTabVisible(oParameters)
        End If
    End Sub

    Private Sub ProcessDocumentSelectedTabVisible(ByVal oParameters As DocumentsCallbackRequest)

        panDocGeneral.Style("display") = "none"
        panDocControl.Style("display") = "none"
        panDocScope.Style("display") = "none"
        panDocApprove.Style("display") = "none"
        panDocNotifications.Style("display") = "none"
        panDocLOPD.Style("display") = "none"

        Select Case oParameters.aTab
            Case 0
                panDocGeneral.Style("display") = ""
            Case 1
                panDocControl.Style("display") = ""
            Case 2
                panDocScope.Style("display") = ""
            Case 3
                panDocApprove.Style("display") = ""
            Case 4
                panDocNotifications.Style("display") = ""
            Case 5
                panDocLOPD.Style("display") = ""
        End Select
    End Sub

    Private Sub LoadDocumentTemplate(oParameters As DocumentsCallbackRequest, Optional ByVal eDocument As roDocumentTemplate = Nothing)
        Dim strError As String = ""
        Dim strMessage As String = ""
        Dim oCurrentDocument As roDocumentTemplate = Nothing

        Try
            If eDocument Is Nothing Then
                oCurrentDocument = DocumentsServiceMethods.GetDocumentTemplateById(Me, oParameters.ID, False)
            Else
                oCurrentDocument = eDocument
            End If

            If oCurrentDocument Is Nothing Then Exit Sub

            txtDocName.Text = oCurrentDocument.Name
            txtDocShortName.Text = oCurrentDocument.ShortName
            txtDocDescription.Text = oCurrentDocument.Description

            rblDocumentArea.Items.FindByValue(oCurrentDocument.Area.ToString).Selected = True

            If oParameters.ID = -1 Then
                rblScope.Enabled = True
                oCurrentDocument.BeginValidity = DateTime.Now.Date
            Else
                rblScope.Enabled = False
            End If

            rblScope.Items.FindByValue(oCurrentDocument.Scope.ToString).Selected = True

            optMandatoryDocument.Checked = oCurrentDocument.Compulsory
            ckCanAddDocumentEmployee.Checked = oCurrentDocument.EmployeeDeliverAllowed
            ckCanAddDocumentSupervisor.Checked = oCurrentDocument.SupervisorDeliverAllowed
            ckSystemDocument.Checked = oCurrentDocument.IsSystem

            If Not (roTypes.Any2DateTime(oCurrentDocument.BeginValidity).Equals(New Date(1900, 1, 1))) Then
                dpPeriodStart.Value = roTypes.Any2DateTime(oCurrentDocument.BeginValidity)
            Else
                dpPeriodStart.Value = Nothing
            End If

            If roTypes.Any2DateTime(oCurrentDocument.EndValidity).Equals(New Date(2079, 1, 1)) OrElse roTypes.Any2DateTime(oCurrentDocument.EndValidity) = Date.MinValue Then
                dpPeriodEnd.Value = Nothing
            Else
                dpPeriodEnd.Value = roTypes.Any2DateTime(oCurrentDocument.EndValidity)
            End If

            Dim oSelValues As String() = {}

            If oCurrentDocument.Notifications IsNot Nothing Then oSelValues = oCurrentDocument.Notifications.Split(",")

            For Each ovalue As ListEditItem In rbNotifications.Items
                If (oSelValues.Contains(ovalue.Value.ToString)) Then
                    ovalue.Selected = True
                Else
                    ovalue.Selected = False
                End If

            Next

            cmbLopdLevel.Items.FindByValue(oCurrentDocument.LOPDAccessLevel).Selected = True

            roOptPanelExpireOnDate.Checked = False
            roOptPanelExpireOnServer.Checked = False
            roOptPanelnoExpire.Checked = False

            If oCurrentDocument.DaysBeforeDelete = -1 Then
                roOptPanelExpireOnServer.Checked = True
            ElseIf oCurrentDocument.DaysBeforeDelete = 0 Then
                roOptPanelnoExpire.Checked = True
            ElseIf oCurrentDocument.DaysBeforeDelete > 0 Then
                roOptPanelExpireOnDate.Checked = True
            End If

            txtExpireLOPDDays.Value = oCurrentDocument.DaysBeforeDelete

            If (oCurrentDocument.ApprovalLevelRequired > 0) Then
                optApproveRequiered.Checked = True
                optNoApprove.Checked = False
                txtRequieredSupervisorLevel.Value = oCurrentDocument.ApprovalLevelRequired
            Else
                optApproveRequiered.Checked = False
                optNoApprove.Checked = True
                txtRequieredSupervisorLevel.Value = 0
            End If

            optExpireOld.Checked = oCurrentDocument.ExpirePrevious

            If (oCurrentDocument.DefaultExpiration Is Nothing OrElse oCurrentDocument.DefaultExpiration = "0") Then
                optAllwaysValid.Checked = True
                optValidUntil.Checked = False
                txtExpireDays.Value = "0"
            Else
                optAllwaysValid.Checked = False
                optValidUntil.Checked = True
                txtExpireDays.Value = oCurrentDocument.DefaultExpiration.Split("@")(1)

            End If

            rbnNonCriticality.ReadOnly = False
            rbnAdviceCriticality.ReadOnly = False
            rbnDeniedCriticality.ReadOnly = False
            rbnNonCriticality.Checked = False
            rbnAdviceCriticality.Checked = False
            rbnDeniedCriticality.Checked = False

            If oCurrentDocument.Scope = DocumentScope.CompanyAccessAuthorization OrElse oCurrentDocument.Scope = DocumentScope.EmployeeAccessAuthorization Then
                rbnNonCriticality.ReadOnly = False
                rbnAdviceCriticality.ReadOnly = False
                rbnDeniedCriticality.ReadOnly = False
            End If

            Select Case oCurrentDocument.AccessValidation
                Case DocumentAccessValidation.NonCritical
                    rbnNonCriticality.Checked = True
                Case DocumentAccessValidation.Advise
                    rbnAdviceCriticality.Checked = True
                Case DocumentAccessValidation.AccessDenied
                    rbnDeniedCriticality.Checked = True
            End Select

            'If (rblScope.SelectedItem.Value = DocumentScope.AbsenceNote.ToString() Or rblScope.SelectedItem.Value = DocumentScope.LeaveOrPermission.ToString()) Then
            If rblScope.SelectedItem.Value = DocumentScope.LeaveOrPermission.ToString() Then
                'optNoApprove.Checked = True 'no cal, ja estava aixi
                'Me.optNoApprove.Enabled = False
                'Me.optApproveRequiered.Enabled = False
                'XIS Ticket 87033
                'Me.optValidUntil.Enabled = False
                'Me.optAllwaysValid.Enabled = False
                'Me.optExpireOld.Enabled = False
            End If

            'Check Permissions
            Dim disControls As Boolean = False
            If oPermission < Permission.Write Then
                DisableControls(Controls)
            End If
        Catch ex As Exception
        Finally
            ASPxCallbackPanelContenido.JSProperties.Add("cpActionRO", "GETDOCUMENT")
            If strError = String.Empty Then
                ASPxCallbackPanelContenido.JSProperties.Add("cpResultRO", "OK")
                ASPxCallbackPanelContenido.JSProperties.Add("cpNameRO", oCurrentDocument.Name)
                ASPxCallbackPanelContenido.JSProperties.Add("cpIsNewRO", False)
            Else
                ASPxCallbackPanelContenido.JSProperties.Add("cpResultRO", "KO")
                ASPxCallbackPanelContenido.JSProperties.Add("cpMessageRO", strMessage)
            End If
        End Try
    End Sub

    Private Sub SaveDocumentTemplate(ByVal oParameters As DocumentsCallbackRequest)
        Dim strError As String = ""
        Dim strMessage As String = ""
        Dim oCurrentDocument As roDocumentTemplate = Nothing
        Dim idCurrentDocument = -1

        Dim bIsnew As Boolean = False

        Try
            If (oParameters.ID = -1) Then bIsnew = True
            oCurrentDocument = DocumentsServiceMethods.GetDocumentTemplateById(Me, oParameters.ID, False)

            Dim disControls As Boolean = False
            If oPermission < Permission.Write Then
                strError = Me.Language.Translate("Error.NoPermission", Me.DefaultScope)
                oCurrentDocument = Nothing
            End If

            If oCurrentDocument Is Nothing Then Exit Sub
            oCurrentDocument.Name = txtDocName.Text
            oCurrentDocument.ShortName = txtDocShortName.Text
            oCurrentDocument.Description = txtDocDescription.Text

            oCurrentDocument.Compulsory = optMandatoryDocument.Checked
            oCurrentDocument.EmployeeDeliverAllowed = ckCanAddDocumentEmployee.Checked
            oCurrentDocument.SupervisorDeliverAllowed = ckCanAddDocumentSupervisor.Checked
            oCurrentDocument.IsSystem = ckSystemDocument.Checked

            oCurrentDocument.Area = System.Enum.Parse(GetType(DocumentArea), rblDocumentArea.SelectedItem.Value)
            oCurrentDocument.Scope = System.Enum.Parse(GetType(DocumentScope), rblScope.SelectedItem.Value)

            oCurrentDocument.BeginValidity = If(dpPeriodStart.Value Is Nothing, New Date(1900, 1, 1), dpPeriodStart.Value)
            oCurrentDocument.EndValidity = If(dpPeriodEnd.Value Is Nothing, New Date(2079, 1, 1), dpPeriodEnd.Value)

            oCurrentDocument.Notifications = ""
            For i As Integer = 0 To rbNotifications.SelectedValues.Count - 1
                oCurrentDocument.Notifications &= rbNotifications.SelectedValues.Item(i) & ","
            Next

            If oCurrentDocument.Notifications <> String.Empty Then oCurrentDocument.Notifications = oCurrentDocument.Notifications.Substring(0, oCurrentDocument.Notifications.Length - 1)

            oCurrentDocument.LOPDAccessLevel = cmbLopdLevel.SelectedItem.Value

            If roOptPanelExpireOnServer.Checked Then
                oCurrentDocument.DaysBeforeDelete = -1
            ElseIf roOptPanelnoExpire.Checked Then
                oCurrentDocument.DaysBeforeDelete = 0
            ElseIf roOptPanelExpireOnDate.Checked Then
                oCurrentDocument.DaysBeforeDelete = If(roTypes.Any2Integer(txtExpireLOPDDays.Value) < 0, 0, roTypes.Any2Integer(txtExpireLOPDDays.Value))
            End If

            oCurrentDocument.AccessValidation = GetCriticality()

            oCurrentDocument.ApprovalLevelRequired = If(optNoApprove.Checked, 0, roTypes.Any2Integer(txtRequieredSupervisorLevel.Value))
            oCurrentDocument.ExpirePrevious = optExpireOld.Checked

            oCurrentDocument.DefaultExpiration = If(optAllwaysValid.Checked, "0", "1@" & txtExpireDays.Value)

            oCurrentDocument = DocumentsServiceMethods.SaveDocumentTemplate(oCurrentDocument, Me, True)
            If oCurrentDocument.Id > 0 AndAlso DocumentsServiceMethods.LastResult = DocumentResultEnum.NoError Then
                oParameters.ID = oCurrentDocument.Id
                Dim rOK As New roJSON.JSONError(False, "OK:" & oCurrentDocument.Id)
                strMessage = rOK.toJSON
            Else
                Dim rError As New roJSON.JSONError(True, DocumentsServiceMethods.LastErrorText)
                strMessage = rError.toJSON
                strError = "KO"
            End If
        Catch ex As Exception
            Dim rError As New roJSON.JSONError(True, DocumentsServiceMethods.LastErrorText)
            strMessage = rError.toJSON
            strError = "KO"
        Finally
            If strError = String.Empty Then
                LoadDocumentTemplate(oParameters)
                ASPxCallbackPanelContenido.JSProperties("cpIsNewRO") = bIsnew
                ASPxCallbackPanelContenido.JSProperties("cpNewObjId") = oCurrentDocument.Id
            Else
                LoadDocumentTemplate(oParameters, oCurrentDocument)
                ASPxCallbackPanelContenido.JSProperties("cpResultRO") = "KO"
                ASPxCallbackPanelContenido.JSProperties("cpActionRO") = "SAVEDOCUMENT"
                ASPxCallbackPanelContenido.JSProperties.Add("cpMessageRO", strMessage)

            End If

        End Try
    End Sub

#End Region

#Region "Métodos Generales"

    Private Sub LoadRadioButtonList()
        rblScope.Items.Clear()

        Dim GPALegacyMode As Boolean = roTypes.Any2Boolean(HelperSession.AdvancedParametersCache("GPADocumentation.LegacyMode"))

        For Each eType As DocumentScope In System.Enum.GetValues(GetType(DocumentScope))
            'If (eType = DocumentScope.AbsenceNote Or eType = DocumentScope.LeaveOrPermission) Then
            If eType <> DocumentScope.Communique AndAlso eType <> DocumentScope.BioCertificate Then
                If eType = DocumentScope.LeaveOrPermission Then
                    If (Not GPALegacyMode) Then
                        rblScope.Items.Add(Language.Translate("Scope." & eType.ToString, DefaultScope), eType) 'en LegacyMode, no s'ensenyen els 2 tipus Absencia
                    End If
                Else
                    rblScope.Items.Add(Language.Translate("Scope." & eType.ToString, DefaultScope), eType)
                End If
            End If
        Next

        cmbLopdLevel.ValueType = GetType(Integer)
        cmbLopdLevel.Items.Clear()

        cmbLopdLevel.Items.Add(Language.Translate("LOPD.Low", DefaultScope), 0)
        cmbLopdLevel.Items.Add(Language.Translate("LOPD.Medium", DefaultScope), 1)
        cmbLopdLevel.Items.Add(Language.Translate("LOPD.High", DefaultScope), 2)

        rblDocumentArea.Items.Clear()

        For Each eArea As DocumentArea In System.Enum.GetValues(GetType(DocumentArea))
            rblDocumentArea.Items.Add(Language.Translate("Area." & eArea.ToString, DefaultScope), eArea)
        Next

        rbNotifications.Items.Clear()
        For Each oNotification As roNotification In NotificationsDataTable
            'rbNotifications.Items.Add(oNotification.Name, oNotification.ID)
            rbNotifications.Items.Add(Language.Translate("Communications." & oNotification.ID.ToString, DefaultScope), oNotification.ID)
        Next

    End Sub

    Private Function GetCriticality() As DocumentAccessValidation
        If rbnNonCriticality.Checked Then
            Return DocumentAccessValidation.NonCritical
        ElseIf rbnAdviceCriticality.Checked Then
            Return DocumentAccessValidation.Advise
        ElseIf rbnDeniedCriticality.Checked Then
            Return DocumentAccessValidation.AccessDenied
        Else
            Return Nothing
        End If
    End Function

#End Region

End Class