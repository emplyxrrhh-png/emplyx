Imports DevExpress.Web
Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class Alerts_Default
    Inherits PageBase

    Private oPermission As Permission
    Private dsTable As DataSet

    Private Property DocumentAlertType() As roAlertsCommon.eDocumentAlertType
        Get
            Dim mode As Object = Session("AlertDetails_DocumentAlertType")
            If mode IsNot Nothing Then
                mode = Session("AlertDetails_DocumentAlertType")
            Else
                mode = roAlertsCommon.eDocumentAlertType.DocumentsValidation
                Session("AlertDetails_DocumentAlertType") = mode
            End If

            Return mode
        End Get
        Set(ByVal value As roAlertsCommon.eDocumentAlertType)
            Session("AlertDetails_DocumentAlertType") = value
        End Set
    End Property

    Private Property IdRelatedObject() As Integer
        Get
            Dim mode As Object = Session("AlertDetails_IdRelatedObject")
            If mode IsNot Nothing Then
                mode = Session("AlertDetails_IdRelatedObject")
            Else
                mode = -1
                Session("AlertDetails_IdRelatedObject") = mode
            End If

            Return mode
        End Get
        Set(ByVal value As Integer)
            Session("AlertDetails_IdRelatedObject") = value
        End Set
    End Property

    Private Property AlertsType() As eMode
        Get
            Dim mode As Object = Session("AlertDetails_AlertsType")
            If mode IsNot Nothing Then
                mode = CType(Session("AlertDetails_AlertsType"), eMode)
            Else
                mode = eMode.Alerts
                Session("AlertDetails_AlertsType") = mode
            End If

            Return mode
        End Get
        Set(ByVal value As eMode)
            Session("AlertDetails_AlertsType") = value
        End Set
    End Property

    Private Property DocumentAlertsData(ByVal docType As DocumentType, ByVal iIdRelatedObject As Integer, ByVal bForceLoad As Boolean) As DocumentAlerts
        Get
            Dim eDocAlerts As DocumentAlerts = Nothing

            If (iIdRelatedObject >= 0) Then
                eDocAlerts = Session("AlertDetails_DocumentAlertsData")
            Else
                If docType = DocumentType.Company Then
                    eDocAlerts = WLHelperWeb.CompanyDocumentationAlerts
                Else
                    eDocAlerts = WLHelperWeb.EmployeeDocumentationAlerts
                End If
            End If

            If eDocAlerts Is Nothing OrElse (bForceLoad AndAlso iIdRelatedObject <> -1) Then
                eDocAlerts = DocumentsServiceMethods.GetDocumentationFaults(Me.Page, docType, iIdRelatedObject)

                Session("AlertDetails_DocumentAlertsData") = eDocAlerts
            End If
            Return eDocAlerts
        End Get
        Set(ByVal value As DocumentAlerts)
            If value IsNot Nothing Then
                Session("AlertDetails_DocumentAlertsData") = value
            Else
                Session("AlertDetails_DocumentAlertsData") = Nothing
            End If
        End Set
    End Property

    Private Property AlertsData(ByVal NotifTypea As Integer, ByVal bForceLoad As Boolean) As DataView
        Get
            Dim tbCauses As DataTable = Session("AlertDetails_AlertsData")

            Dim dv As DataView = Nothing
            If tbCauses IsNot Nothing Then
                dv = New DataView(tbCauses)
                dv.Sort = "Column1Detail ASC"
            End If

            If dv Is Nothing OrElse bForceLoad Then

                Dim alertsDS As DataSet = API.NotificationServiceMethods.GetDesktopAlerts(NotifTypea, Me)

                dv = New DataView(alertsDS.Tables(0))
                dv.Sort = "Column1Detail ASC"
                Session("AlertDetails_AlertsData") = dv.Table

            End If
            Return dv
        End Get
        Set(ByVal value As DataView)
            If value IsNot Nothing Then
                Session("AlertDetails_AlertsData") = value.Table
            Else
                Session("AlertDetails_AlertsData") = Nothing
            End If
        End Set
    End Property

    Public Enum eMode
        Alerts = 0
        Documents = 1
    End Enum

    Public Enum eAlertType
        Employees = 0
        Calendar = 1
        Tasks = 2
        AIScheduler = 3
        Department = 4
    End Enum

    Protected Function GetAlertEnumType() As eAlertType
        If Not Request.QueryString("NotificationType") Is Nothing Then
            Dim NotifType As Integer = roTypes.Any2Integer(Request.QueryString("NotificationType"))
            Select Case NotifType
                Case DTOs.eNotificationType.Task_Close_to_Finish, DTOs.eNotificationType.Task_Close_to_Start, DTOs.eNotificationType.Task_Exceeding_Finished_Date,
                     DTOs.eNotificationType.Task_Exceeding_Planned_Time, DTOs.eNotificationType.Task_exceeding_Started_Date, DTOs.eNotificationType.Task_With_ALerts,
                     DTOs.eNotificationType.Tasks_Request_complete
                    Return eAlertType.Tasks
                Case DTOs.eNotificationType.LabAgree_Min_Reached, DTOs.eNotificationType.LabAgree_Max_Exceeded
                    Return eAlertType.Calendar
                Case DTOs.eNotificationType.Productive_Unit_Under_Coverage
                    Return eAlertType.AIScheduler
                Case eNotificationType.NonSupervisedDepartments
                    Return eAlertType.Department
                Case Else
                    Return eAlertType.Employees
            End Select
        Else
            Return eAlertType.Employees
        End If
    End Function

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        Me.oPermission = Me.GetFeaturePermission("Employees")

        Me.InsertExtraJavascript("Ajax", "~/Base/Scripts/Ajax.js")
        Me.InsertExtraJavascript("GenericData", "~/Base/Scripts/GenericData.js")
        Me.InsertExtraJavascript("jsGrid", "~/Base/Scripts/jsGrid.js")
        Me.InsertExtraJavascript("roOptionPanelClient", "~/Base/Scripts/roOptionPanelClient.js")
        Me.InsertExtraJavascript("roOptPanelClientGroup", "~/Base/Scripts/roOptPanelClientGroup.js")
        Me.InsertExtraJavascript("roTreeState", "~/Base/Scripts/rocontrols/roTrees/roTreeState.js")
        Me.InsertExtraJavascript("Alerts", "~/Alerts/Scripts/Alerts.js")

    End Sub

    Protected Sub Alerts_Default_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Me.oPermission = Permission.None Then
            WLHelperWeb.RedirectAccessDenied(True)
            Exit Sub
        ElseIf Me.oPermission = Permission.Read Then
            DisableControls()
        End If

        CompanyURI.Value = Request.ApplicationPath + "#/" + Configuration.RootUrl + "/Employees/Groups"
        EmployeeURI.Value = Request.ApplicationPath + "#/" + Configuration.RootUrl + "/Employees/Employees"
        TasksURI.Value = Request.ApplicationPath + "#/" + Configuration.RootUrl + "/Tasks/Tasks?IDTask="
        AbsencesURI.Value = Request.ApplicationPath + "#/" + Configuration.RootUrl + "/Absences/AbsencesStatus"
        AISchedulerURI.Value = Request.ApplicationPath + "#/" + Configuration.RootUrl + "/AIScheduler/Budget"
        CalendarURI.Value = Request.ApplicationPath + "#/" + Configuration.RootUrl + "/Scheduler/Calendar?IDEmployee="
        SupervisorURI.Value = Request.ApplicationPath + "#/" + Configuration.RootUrl + "//Supervisors"

        If Not Me.IsPostBack And Not Me.IsCallback Then
            Dim DocAlertType As Integer = If(Request.QueryString("DocumentAlertType") IsNot Nothing, roTypes.Any2Integer(Request.QueryString("DocumentAlertType")), -1)
            Dim oDocType As DocumentType = If((Request.QueryString("DocumentType") Is Nothing OrElse Request.QueryString("DocumentType").ToUpper = "EMPLOYEE"), DocumentType.Employee, DocumentType.Company)
            Dim iIdRelatedObject As Integer = If(Request.QueryString("IdRelatedObject") IsNot Nothing, roTypes.Any2Integer(Request.QueryString("IdRelatedObject")), -1)
            Dim NotifType As Integer = If(Request.QueryString("NotificationType") IsNot Nothing, roTypes.Any2Integer(Request.QueryString("NotificationType")), -1)

            If NotifType >= 0 Then
                Me.IdRelatedObject = -1
                Me.AlertsType = eMode.Alerts
                Me.hdnNotifType.Value = NotifType

            ElseIf DocAlertType >= 0 Then
                Me.DocumentAlertType = If(Request.QueryString("DocumentAlertType") IsNot Nothing, roTypes.Any2Integer(Request.QueryString("DocumentAlertType")), CInt(roAlertsCommon.eDocumentAlertType.DocumentsValidation))

                Me.IdRelatedObject = iIdRelatedObject
                Me.AlertsType = eMode.Documents

                Me.hdnDocumentAlertType.Value = Me.DocumentAlertType
                Me.hdnDocAlertType.Value = DocAlertType
                Me.hdnIdRelatedObject.Value = Me.IdRelatedObject
                Me.hdnDocType.Value = CInt(oDocType)
                Me.hdnNotifType.Value = NotifType
            End If

            CreateColumnsAlertsDetail(Me.AlertsType)
            Me.LoadData(Me.AlertsType, If(Me.AlertsType = eMode.Documents, DocAlertType, NotifType), Me.IdRelatedObject, oDocType, True)
        Else
            CreateColumnsAlertsDetail(Me.AlertsType)
            LoadData(Me.AlertsType, roTypes.Any2Integer(Me.hdnDocAlertType.Value), roTypes.Any2Integer(Me.hdnIdRelatedObject.Value), roTypes.Any2Integer(Me.hdnDocType.Value), False)
        End If
        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "refreshInitGrid", "window.parent.parent.showLoader(false);", True)
    End Sub

    Private Sub LoadData(ByVal popupMode As eMode, ByVal NotifType As Integer, ByVal iIdRelatedObject As Integer, ByVal docType As DocumentType, ByVal bForceLoad As Boolean)
        If popupMode = eMode.Alerts Then
            Dim ds As DataView = Me.AlertsData(NotifType, bForceLoad)

            If ds.Table IsNot Nothing Then
                If Not ds.Table Is Nothing Then
                    Me.GridDetails.DataSource = ds.Table
                    Me.GridDetails.DataBind()
                End If
            End If
        Else
            Dim docAlerts As DocumentAlerts = Me.DocumentAlertsData(docType, iIdRelatedObject, bForceLoad)

            If docAlerts IsNot Nothing Then
                Select Case NotifType
                    Case roAlertsCommon.eDocumentAlertType.DocumentsValidation
                        Me.GridDetails.DataSource = docAlerts.DocumentsValidation
                    Case roAlertsCommon.eDocumentAlertType.LeaveDocuments
                        Me.GridDetails.DataSource = docAlerts.AbsenteeismDocuments
                    Case roAlertsCommon.eDocumentAlertType.LeaveDocumentsValidationPending
                        Me.GridDetails.DataSource = docAlerts.AbsenteeismDocuments.ToList.FindAll(Function(x) x.IDDocument > 0)
                    Case roAlertsCommon.eDocumentAlertType.LeaveDocumentsUndelivered
                        Me.GridDetails.DataSource = docAlerts.AbsenteeismDocuments.ToList.FindAll(Function(x) x.IDDocument = 0)
                    Case roAlertsCommon.eDocumentAlertType.MandatoryDocuments
                        Me.GridDetails.DataSource = docAlerts.MandatoryDocuments
                    Case roAlertsCommon.eDocumentAlertType.WorkForecastDocuments
                        Me.GridDetails.DataSource = docAlerts.WorkForecastDocuments
                    Case roAlertsCommon.eDocumentAlertType.WorkForecastDocumentsValidationPending
                        Me.GridDetails.DataSource = docAlerts.WorkForecastDocuments.ToList.FindAll(Function(x) x.IDDocument > 0)
                    Case roAlertsCommon.eDocumentAlertType.WorkForecastDocumentsUndelivered
                        Me.GridDetails.DataSource = docAlerts.WorkForecastDocuments.ToList.FindAll(Function(x) x.IDDocument = 0)
                    Case roAlertsCommon.eDocumentAlertType.AccessAuthorizationDocumentsValidationsPending
                        Me.GridDetails.DataSource = docAlerts.AccessAuthorizationDocuments.ToList.FindAll(Function(x) x.IDDocument > 0)
                    Case roAlertsCommon.eDocumentAlertType.AccessAuthorizationDocumentsUndelivered
                        Me.GridDetails.DataSource = docAlerts.AccessAuthorizationDocuments.ToList.FindAll(Function(x) x.IDDocument = 0)
                End Select

                Me.GridDetails.DataBind()
            End If
        End If

    End Sub

    Private Sub CreateColumnsAlertsDetail(ByVal popupMode As eMode)

        Dim GridColumn As GridViewDataTextColumn
        Dim GridColumnCommand As GridViewCommandColumn

        Dim VisibleIndex As Integer = 0

        Me.GridDetails.Columns.Clear()
        If popupMode = eMode.Alerts Then
            Me.GridDetails.KeyFieldName = "ID"
        Else
            Me.GridDetails.KeyFieldName = "IDDocument"
        End If

        'Clave
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "ID"
        GridColumn.FieldName = "ID"
        GridColumn.ReadOnly = False
        GridColumn.Visible = False
        Me.GridDetails.Columns.Add(GridColumn)

        'IDEmployee
        GridColumn = New GridViewDataTextColumn()
        If popupMode = eMode.Alerts Then
            GridColumn.Caption = "IDEmployee"
            GridColumn.FieldName = "IDEmployee"
        Else
            GridColumn.Caption = "IdRelatedObject"
            GridColumn.FieldName = "IdRelatedObject"
        End If

        GridColumn.ReadOnly = False
        GridColumn.Visible = False
        Me.GridDetails.Columns.Add(GridColumn)

        If popupMode = eMode.Documents Then
            'IDEmployee
            GridColumn = New GridViewDataTextColumn()
            GridColumn.Caption = "IDDocument"
            GridColumn.FieldName = "IDDocument"
            GridColumn.ReadOnly = False
            GridColumn.Visible = False
            Me.GridDetails.Columns.Add(GridColumn)
        End If

        If GetAlertEnumType() <> eAlertType.AIScheduler AndAlso (Me.IdRelatedObject = -1 OrElse popupMode = eMode.Alerts) Then

            GridColumn = New GridViewDataTextColumn
            If GetAlertEnumType() = eAlertType.Tasks Then
                GridColumn.Caption = Me.Language.Translate("GridDetails.Column.Detail1Task", DefaultScope) '"Valor"
            ElseIf GetAlertEnumType() = eAlertType.Department Then
                GridColumn.Caption = Me.Language.Translate("GridDetails.Column.Detail1Department", DefaultScope) '"Valor"
            Else
                GridColumn.Caption = Me.Language.Translate("GridDetails.Column.Detail1", DefaultScope) '"Valor"
            End If

            If popupMode = eMode.Alerts Then
                GridColumn.FieldName = "Column1Detail"
            Else
                GridColumn.FieldName = "ObjectName"
            End If

            GridColumn.VisibleIndex = VisibleIndex
            VisibleIndex = VisibleIndex + 1
            GridColumn.CellStyle.Wrap = DevExpress.Utils.DefaultBoolean.True
            GridColumn.ReadOnly = False
            GridColumn.Width = 200
            GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
            GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
            Me.GridDetails.Columns.Add(GridColumn)
        End If

        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = Me.Language.Translate("GridDetails.Column.Detail2", DefaultScope) '"Valor"

        If popupMode = eMode.Alerts Then
            If GetAlertEnumType() <> eAlertType.AIScheduler Then
                GridColumn.FieldName = "Column2Detail"
            Else
                GridColumn.FieldName = "Column1Detail"
            End If
        Else
            GridColumn.FieldName = "Description"
        End If

        GridColumn.VisibleIndex = VisibleIndex
        GridColumn.CellStyle.Wrap = DevExpress.Utils.DefaultBoolean.True
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = False
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        Me.GridDetails.Columns.Add(GridColumn)

        If GetAlertEnumType() = eAlertType.AIScheduler Then
            'IDEmployee
            GridColumn = New GridViewDataTextColumn()
            GridColumn.Caption = "Column3Detail"
            GridColumn.FieldName = "Column3Detail"
            GridColumn.ReadOnly = False
            GridColumn.Visible = False
            Me.GridDetails.Columns.Add(GridColumn)
        End If

        If popupMode = eMode.Alerts OrElse Me.IdRelatedObject = -1 OrElse Me.DocumentAlertType = roAlertsCommon.eDocumentAlertType.GpaAlerts Then

            'Command buttons
            GridColumnCommand = New GridViewCommandColumn()
            GridColumnCommand.ButtonRenderMode = GridCommandButtonRenderMode.Image

            Dim employeeButton As GridViewCommandColumnCustomButton = New GridViewCommandColumnCustomButton()
            employeeButton.ID = "ShowEmployeeButton"
            If popupMode = eMode.Alerts Then
                If GetAlertEnumType() = eAlertType.Tasks Then
                    employeeButton.Image.Url = "~/Alerts/Images/Task16.png"
                    employeeButton.Text = Me.Language.Translate("AbsenceTypes.ShowTask", DefaultScope) 'Ficha empleado"
                ElseIf GetAlertEnumType() = eAlertType.AIScheduler Then
                    employeeButton.Image.Url = "~/AIScheduler/Images/ProductiveUnit16x16.png"
                    employeeButton.Text = Me.Language.Translate("AbsenceTypes.ShowCoverages", DefaultScope) 'Ficha empleado"
                ElseIf GetAlertEnumType() = eAlertType.AIScheduler Then
                    employeeButton.Image.Url = "~/AIScheduler/Images/ProductiveUnit16x16.png"
                    employeeButton.Text = Me.Language.Translate("AbsenceTypes.ShowCoverages", DefaultScope) 'Ficha empleado"
                ElseIf GetAlertEnumType() = eAlertType.Department Then
                    employeeButton.Image.Url = "~/Base/Images/EmployeeSelector/Company_16.png"
                    employeeButton.Text = Me.Language.Translate("AbsenceTypes.DepartmentSupervision", DefaultScope)
                Else
                    employeeButton.Image.Url = "~/Base/Images/EmployeeSelector/Empleado-16x16.gif"
                    employeeButton.Text = Me.Language.Translate("AbsenceTypes.ShowEmployee", DefaultScope) 'Ficha empleado"
                End If
            Else
                employeeButton.Image.Height = 16
                employeeButton.Image.Width = 16
                employeeButton.Image.Url = "~/Base/Images/StartMenuIcos/Documents.png"
                employeeButton.Text = Me.Language.Translate("AbsenceTypes.ShowDocument", DefaultScope) 'Ficha empleado"
            End If

            GridColumnCommand.CustomButtons.Add(employeeButton)
            GridColumnCommand.ShowEditButton = False
            GridColumnCommand.ShowDeleteButton = False
            GridColumnCommand.Caption = " "
            GridColumnCommand.VisibleIndex = VisibleIndex
            GridColumnCommand.CellStyle.HorizontalAlign = HorizontalAlign.Center
            GridColumnCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
            GridColumnCommand.Width = 25
            VisibleIndex = VisibleIndex + 1

            Me.GridDetails.Columns.Add(GridColumnCommand)
        End If
    End Sub

End Class