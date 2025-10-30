Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Public Class DocumentUpload
    Inherits PageBase

    Public Property Forecast() As Integer
        Get
            Return ViewState("EditDocument_ForecastType")
        End Get

        Set(ByVal value As Integer)
            ViewState("EditDocument_ForecastType") = value
        End Set
    End Property

    Public Property IdRelatedObject() As Integer
        Get
            Return ViewState("EditDocument_IDEmployee")
        End Get

        Set(ByVal value As Integer)
            ViewState("EditDocument_IDEmployee") = value
        End Set
    End Property

    Public Property IdAbsence() As Integer
        Get
            Return ViewState("EditDocument_IDAbsence")
        End Get

        Set(ByVal value As Integer)
            ViewState("EditDocument_IDAbsence") = value
        End Set
    End Property

    Public Property IdDocument() As Integer
        Get
            Return ViewState("EditDocument_IdDocument")
        End Get

        Set(ByVal value As Integer)
            ViewState("EditDocument_IdDocument") = value
        End Set
    End Property

    Public Property Type() As Integer
        Get
            Return ViewState("EditDocument_Type")
        End Get

        Set(ByVal value As Integer)
            ViewState("EditDocument_Type") = value
        End Set
    End Property

    Public Property ControlClientId() As String
        Get
            Return ViewState("EditDocument_ControlClientId")
        End Get

        Set(ByVal value As String)
            ViewState("EditDocument_ControlClientId") = value
        End Set
    End Property

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Init
        InsertExtraJavascript("Cookies", "~/Base/Scripts/Cookies.js", , True)
        InsertExtraJavascript("Generic", "~/Base/Scripts/Generic.js", , True)
        InsertExtraJavascript("GridViewHelper", "~/Base/Scripts/GridViewHelper.js", , True)
        InsertExtraJavascript("EmployeeGroup", "~/Employees/Scripts/EmployeeGroups.js")
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InsertCssIncludes(Page)
        If Not IsPostBack Then
            Me.IdRelatedObject = roTypes.Any2Integer(Request("IdRelatedObject"))
            Me.IdDocument = roTypes.Any2Integer(Request("IdDocument"))
            Me.Type = CType([Enum].Parse(GetType(DocumentType), Request("Scope")), DocumentType)
            Me.ControlClientId = roTypes.Any2String(Request("ClientId"))
            Me.IdAbsence = roTypes.Any2Integer(Request("IdAbsence"))
            Me.Forecast = CType([Enum].Parse(GetType(ForecastType), Request("ForecatType")), ForecastType)
            hdnControlCaller.Value = ControlClientId
            LoadCombos()
        End If
    End Sub

    Private Sub LoadCombos()
        Dim oselectingList As List(Of roDocumentTemplate) = Nothing

        If Me.IdAbsence <= 0 Then
            oselectingList = DocumentsServiceMethods.GetAvailableDocumentTemplateList(Type, Me.IdRelatedObject, Permission.Write, Page, False)

            If oselectingList IsNot Nothing Then oselectingList = oselectingList.FindAll(Function(x) x.Scope <> DocumentScope.LeaveOrPermission AndAlso x.Scope <> DocumentScope.CauseNote)
        Else
            oselectingList = DocumentsServiceMethods.GetTemplateDocumentsAvailableByAbsence(Me.IdAbsence, Me.IdRelatedObject, False, Me.Forecast, Page, False)
        End If

        If oselectingList IsNot Nothing Then
            oselectingList = oselectingList.FindAll(Function(x) x.SupervisorDeliverAllowed = True)

            Me.cmbDocumentTemplate.Items.Clear()

            For Each oDocument In oselectingList
                Me.cmbDocumentTemplate.Items.Add(oDocument.Name, oDocument.Id & "_" & CInt(oDocument.LeaveDocumentType))
            Next

            'Me.cmbDocumentTemplate.DataSource = oselectingList
            'Me.cmbDocumentTemplate.ValueField = "ID"
            'Me.cmbDocumentTemplate.TextField = "Name"
            'Me.cmbDocumentTemplate.DataBind()

            If oselectingList.Count > 0 Then
                Me.cmbDocumentTemplate.SelectedItem = Me.cmbDocumentTemplate.Items(0)
                Me.txtDocTitle.Text = ""
            End If
        End If

        Me.detReceivedDate.Value = Nothing 'DateTime.Now.Date

        Dim bOptionalDocs As Boolean = roTypes.Any2Boolean(HelperSession.AdvancedParametersCache("VTLive.OptionalDocuments"))

        If Not bOptionalDocs Then
            Me.ckDocumentNotMandatory.Visible = False
            Me.ckDocumentNotMandatory.Checked = False
        Else
            Me.ckDocumentNotMandatory.Visible = True
        End If
    End Sub

    Protected Sub btOK_Click(ByVal source As Object, ByVal e As EventArgs) Handles btOK.Click

        Dim oDeliverdDocument As roDocument = DocumentsServiceMethods.GetDocumentById(Me.Page, Me.IdDocument, False)

        Dim bOptionalDocs As Boolean = roTypes.Any2Boolean(HelperSession.AdvancedParametersCache("VTLive.OptionalDocuments"))

        If txtDocTitle.Text <> String.Empty AndAlso cmbDocumentTemplate.SelectedItem IsNot Nothing AndAlso
            ((fUploader.PostedFile IsNot Nothing AndAlso fUploader.PostedFile.ContentLength > 0) OrElse (bOptionalDocs AndAlso Me.ckDocumentNotMandatory.Checked)) Then

            Dim oVal As String = HelperSession.AdvancedParametersCache("DocumentMaxSize")

            If cmbDocumentTemplate.SelectedItem IsNot Nothing Then
                Dim oDocTemplate As roDocumentTemplate = DocumentsServiceMethods.GetDocumentTemplateById(Me.Page, roTypes.Any2Integer(roTypes.Any2String(cmbDocumentTemplate.Value).Split("_")(0)), False)

                If oDocTemplate.LeaveDocumentType = LeaveDocumentType.ReturnReport AndAlso Me.detReceivedDate.Value Is Nothing Then
                    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "fileUploadError", "window.parent.parent.frames['ifPrincipal'].showErrorPopup('Error.Title', 'error', 'Error.DocumentUpload.DateRequiered', 'Error.OK', 'Error.OKDesc', '');", True)
                Else
                    Dim maxFileSize As Integer = roTypes.Any2Integer(HelperSession.AdvancedParametersCache("VTLive.MaxAllowedFileSize"))
                    If maxFileSize = 0 Then maxFileSize = 256

                    If (oVal = String.Empty AndAlso fUploader.PostedFile.ContentLength <= (maxFileSize * 1024)) OrElse (oVal <> String.Empty AndAlso roTypes.Any2Long(oVal) <= fUploader.PostedFile.ContentLength) Then
                        Dim file As IO.Stream = fUploader.PostedFile.InputStream()
                        Dim fileData As Byte() = New Byte(file.Length - 1) {}
                        file.Read(fileData, 0, file.Length)

                        With oDeliverdDocument
                            .Id = IdDocument
                            .Title = txtDocTitle.Text
                            .IdEmployee = If(Type.Equals(DocumentType.Employee), IdRelatedObject, -1)
                            .IdCompany = If(Type.Equals(DocumentType.Company), IdRelatedObject, -1)
                            .DocumentTemplate = oDocTemplate
                            If fUploader.FileName.IndexOf(".") > 0 Then
                                .DocumentType = fUploader.FileName.Substring(fUploader.FileName.LastIndexOf("."))
                            Else
                                .DocumentType = ".txt"
                            End If

                            Select Case Me.Forecast
                                Case ForecastType.AbsenceHours
                                    If Me.IdAbsence > 0 Then
                                        .IdHoursAbsence = Me.IdAbsence
                                    End If
                                Case ForecastType.AbsenceDays, ForecastType.Leave
                                    If Me.IdAbsence > 0 Then
                                        .IdDaysAbsence = Me.IdAbsence
                                    End If
                                Case ForecastType.OverWork
                                    If Me.IdAbsence > 0 Then
                                        .IdOvertimeForecast = Me.IdAbsence
                                    End If
                            End Select

                            .Document = fileData
                            .DeliveredDate = Date.Now
                            .DeliveryChannel = "VisualTimeLive"
                            .DeliveredBy = WLHelperWeb.CurrentPassport.Name
                            .Status = DocumentStatus.Pending
                            .LastStatusChange = Date.Now
                            .BeginDate = New Date(1900, 1, 1)

                            If .DocumentTemplate.LeaveDocumentType = LeaveDocumentType.ReturnReport Then
                                .EndDate = detReceivedDate.Value
                            Else
                                .EndDate = New Date(2079, 1, 1)
                            End If

                        End With
                        oDeliverdDocument = DocumentsServiceMethods.SaveDocument(oDeliverdDocument, Page, True)
                        If (oDeliverdDocument.Id = -1) Then
                        Else
                            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "closeOnOk", "Close_Response();", True)
                        End If
                    Else
                        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "fileUploadError", "window.parent.parent.frames['ifPrincipal'].showErrorPopup('Error.Title', 'error', 'Error.DocumentUpload.ExcededMaxSize', 'Error.OK', 'Error.OKDesc', '');", True)
                    End If
                End If
            Else
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "fileUploadError", "window.parent.parent.frames['ifPrincipal'].showErrorPopup('Error.Title', 'error', 'Error.DocumentUpload.NoTemplateSelected', 'Error.OK', 'Error.OKDesc', '');", True)
            End If
        Else
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "fileUploadError", "window.parent.parent.frames['ifPrincipal'].showErrorPopup('Error.Title', 'error', 'Error.DocumentUpload.MissingFields', 'Error.OK', 'Error.OKDesc', '');", True)
        End If

    End Sub

    Protected Sub CallbackSession_Callback(ByVal source As Object, ByVal e As DevExpress.Web.CallbackEventArgs) Handles CallbackSession.Callback
        e.Result = String.Empty
        Dim oDeliverdDocument As roDocument = DocumentsServiceMethods.GetDocumentById(Me.Page, Me.IdDocument, False)
        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Select Case strParameter.Trim.ToUpperInvariant
            Case "SAVEDOCUMENT"
                With oDeliverdDocument
                    .Id = IdDocument
                    .Title = txtDocTitle.Text
                    .IdEmployee = If(Type.Equals(DocumentType.Employee), IdRelatedObject, -1)
                    .IdCompany = If(Type.Equals(DocumentType.Company), IdRelatedObject, -1)
                    .DocumentTemplate = New roDocumentTemplate With {.Id = cmbDocumentTemplate.Value}
                    'If txtDocumentPath.Text.IndexOf(".") > 0 Then
                    '    .DocumentType = txtDocumentPath.Text.Substring(txtDocumentPath.Text.LastIndexOf("."))
                    'Else
                    '    .DocumentType = "txt"
                    'End If
                    .Document = CType(Session("DocumentFile"), Byte())
                    .DeliveredDate = detReceivedDate.Value
                    .DeliveryChannel = "VisualTimeLive"
                    .DeliveredBy = WLHelperWeb.CurrentPassport.Name
                    .Status = DocumentStatus.Pending
                    .LastStatusChange = Date.Now
                    .BeginDate = New Date(1900, 1, 1)
                    .EndDate = New Date(2079, 1, 1)
                End With
                oDeliverdDocument = DocumentsServiceMethods.SaveDocument(oDeliverdDocument, Page, True)
        End Select
    End Sub

End Class