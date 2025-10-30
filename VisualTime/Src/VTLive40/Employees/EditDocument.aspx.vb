Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Public Class EditDocument
    Inherits PageBase

    Public Property IdRelatedObject() As Integer
        Get
            Return ViewState("EditDocument_IDEmployee")
        End Get

        Set(ByVal value As Integer)
            ViewState("EditDocument_IDEmployee") = value
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

    Public Property DocState() As DocumentStatus
        Get
            Return ViewState("EditDocument_DocState")
        End Get

        Set(ByVal value As DocumentStatus)
            ViewState("EditDocument_DocState") = value
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
            hdnControlCaller.Value = ControlClientId
            LoadCombos()

            If (IdDocument.Equals(-1)) Then
                txtSupervisor.Text = WLHelperWeb.CurrentPassport.Name
                DocState = DocumentStatus.Pending
                detStatusDate.Value = Date.Now
                txtDocStatus.Text = Language.Translate(DocumentStatus.Pending.ToString, DefaultScope)
                'Page.ClientScript.RegisterStartupScript([GetType](), "Validate", "ValidateStateButtons('NOTHING','" & Language.Translate(DocumentStatus.Pending.ToString, DefaultScope) & "')", True)
            End If
            If (IdDocument > 0) Then
                LoadDocumentData(IdDocument)
            End If
        End If
        Me.detReceivedDate.ReadOnly = True

    End Sub

    Private Sub LoadDocumentData(idDeliveredDocument As Integer)
        Dim oDeliveredDoc = DocumentsServiceMethods.GetDocumentById(Page, idDeliveredDocument, False)
        SetButtonsVisibility(oDeliveredDoc)

        Me.txtDocTitle.Text = roTypes.Any2String(oDeliveredDoc.Title)

        Me.cmbDocumentTemplate.SelectedItem = cmbDocumentTemplate.Items.FindByValue(roTypes.Any2String(oDeliveredDoc.DocumentTemplate.Id))
        Me.txtDocumentPath.Text = roTypes.Any2String(oDeliveredDoc.Title) & oDeliveredDoc.DocumentType
        Me.detReceivedDate.Value = roTypes.Any2DateTime(oDeliveredDoc.DeliveredDate)
        Me.txtSupervisor.Text = roTypes.Any2String(oDeliveredDoc.DeliveredBy)
        Me.txtDocStatus.Text = roTypes.Any2String(Language.Translate(oDeliveredDoc.Status.ToString, DefaultScope))
        Me.txtLevel.Text = roTypes.Any2String(oDeliveredDoc.StatusLevel)
        Me.detStatusDate.Value = roTypes.Any2DateTime(oDeliveredDoc.LastStatusChange)

        If (Not roTypes.Any2DateTime(oDeliveredDoc.BeginDate).Equals(New Date(1900, 1, 1))) Then
            Me.detValidityBeginDate.Value = roTypes.Any2DateTime(oDeliveredDoc.BeginDate)
        Else
            Me.detValidityBeginDate.Value = Nothing
        End If

        If (Not roTypes.Any2DateTime(oDeliveredDoc.EndDate).Equals(New Date(2079, 1, 1))) Then
            Me.detValidityEndDate.Value = roTypes.Any2DateTime(oDeliveredDoc.EndDate)
        Else
            Me.detValidityEndDate.Value = Nothing
        End If
        Me.DocState = oDeliveredDoc.Status

        Dim strParameters As New Generic.List(Of String)
        strParameters.Add(oDeliveredDoc.DeliveredBy)
        strParameters.Add(oDeliveredDoc.DeliveredDate.ToString("dd/MM/yyyy HH:mm"))

        Me.lblUploadedByDescription.InnerHtml = Me.Language.Translate("Document.DeliveredByDescription", Me.DefaultScope, strParameters)

        strParameters.Clear()
        strParameters.Add(roTypes.Any2String(Language.Translate(oDeliveredDoc.Status.ToString, DefaultScope)))
        strParameters.Add(oDeliveredDoc.LastStatusChange.ToString("dd/MM/yyyy HH:mm"))

        Me.lblActualStateDescription.InnerHtml = Me.Language.Translate("Document.ActualDescription", Me.DefaultScope, strParameters)

        If oDeliveredDoc.Status = DocumentStatus.Pending Then
            Me.lblApprovedByDescription.InnerHtml = Me.Language.Translate("Document.WaitingApproval", Me.DefaultScope)
        Else
            strParameters.Clear()
            strParameters.Add(Me.Language.Translate("Document.Action" & oDeliveredDoc.Status.ToString, DefaultScope))

            Dim oPassport As roPassport = API.UserAdminServiceMethods.GetPassport(Me.Page, oDeliveredDoc.IdLastStatusSupervisor, LoadType.Passport)
            If oPassport IsNot Nothing Then
                strParameters.Add(oPassport.Name)
            Else
                strParameters.Add(Me.Language.Translate("Document.ApprovedByDeleted", Me.DefaultScope, strParameters))
            End If

            strParameters.Add(oDeliveredDoc.StatusLevel)

            Me.lblApprovedByDescription.InnerHtml = Me.Language.Translate("Document.ApprovedByDescription", Me.DefaultScope, strParameters)
        End If

        'Page.ClientScript.RegisterStartupScript([GetType](), "Validate", "ValidateStateButtons('" & oDeliveredDoc.Status.ToString.ToUpper & "','" & Language.Translate(oDeliveredDoc.Status.ToString, DefaultScope) & "')", True)
    End Sub

    Private Sub SetButtonsVisibility(ByVal oDeliveredDoc As roDocument)

        btnInvalidateDoc.Visible = True
        btnReject.Visible = True
        btnValidate.Visible = True

        Select Case oDeliveredDoc.Status
            Case DocumentStatus.Expired, DocumentStatus.Invalidated
                btnInvalidateDoc.Visible = False
                btnReject.Visible = False
                btnValidate.Visible = False
            Case DocumentStatus.Rejected
                btnReject.Visible = False
                btnInvalidateDoc.Visible = False
            Case DocumentStatus.Validated

                If oDeliveredDoc.Validity = DocumentValidity.NotEnoughAuthorityLevel Then
                    'Revisar amb Xavi
                    'If oDeliveredDoc.StatusLevel <= WLHelperWeb.CurrentPassport.LevelOfAuthority Then
                    '    btnValidate.Visible = False
                    'End If

                    btnInvalidateDoc.Visible = False
                Else
                    btnReject.Visible = False
                    btnValidate.Visible = False
                End If

            Case DocumentStatus.Pending
                btnInvalidateDoc.Visible = False
        End Select
    End Sub

    Private Sub LoadCombos()
        Dim oselectingList As List(Of roDocumentTemplate) = DocumentsServiceMethods.GetDocumentTemplateListbyType(Type, Page, False)

        If oselectingList IsNot Nothing Then
            cmbDocumentTemplate.DataSource = oselectingList
            cmbDocumentTemplate.ValueField = "ID"
            cmbDocumentTemplate.TextField = "Name"
            cmbDocumentTemplate.DataBind()
        End If
    End Sub

    Protected Sub CallbackSession_Callback(ByVal source As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase) Handles CallbackSession.Callback

        Dim oDeliverdDocument As roDocument = DocumentsServiceMethods.GetDocumentById(Me.Page, Me.IdDocument, False)
        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Dim strcomment As String = String.Empty

        If strParameter.IndexOf("@@") <> -1 Then
            strcomment = strParameter.Substring(strParameter.IndexOf("@@") + 2)
            strParameter = strParameter.Substring(0, strParameter.IndexOf("@@"))
        End If

        Select Case strParameter.Trim.ToUpperInvariant
            Case "UPDATEDOCUMENT"
                With oDeliverdDocument
                    .BeginDate = If(detValidityBeginDate.Value Is Nothing, New Date(1900, 1, 1), detValidityBeginDate.Value)
                    .EndDate = If(detValidityEndDate.Value Is Nothing, New Date(2079, 1, 1), detValidityEndDate.Value)
                End With
                oDeliverdDocument = DocumentsServiceMethods.SaveDocument(oDeliverdDocument, Page, True)
                If Me.CallbackSession.JSProperties.ContainsKey("cpResultRO") Then
                    Me.CallbackSession.JSProperties("cpResultRO") = String.Empty
                Else
                    Me.CallbackSession.JSProperties.Add("cpResultRO", String.Empty)
                End If
            Case "VALIDATED", "REJECTED", "INVALIDATED"
                Dim newStatus As DocumentStatus = DocumentStatus.Pending

                If (strParameter.Trim.ToUpperInvariant.Equals("VALIDATED")) Then
                    newStatus = DocumentStatus.Validated
                ElseIf (strParameter.Trim.ToUpperInvariant.Equals("REJECTED")) Then
                    newStatus = DocumentStatus.Rejected
                ElseIf (strParameter.Trim.ToUpperInvariant.Equals("INVALIDATED")) Then
                    newStatus = DocumentStatus.Invalidated
                End If

                If (DocumentsServiceMethods.ChangeDeliveredDocumentState(Me.IdDocument, newStatus, strcomment, Date.Now, Page, True)) Then

                    Dim documentActualizado = DocumentsServiceMethods.GetDocumentById(Me.Page, Me.IdDocument, False)
                    SetButtonsVisibility(documentActualizado)

                    Dim strParameters As New Generic.List(Of String)
                    strParameters.Add(Me.Language.Translate(newStatus.ToString, DefaultScope))
                    strParameters.Add(Date.Now.ToString("dd/MM/yyyy"))

                    Me.lblActualStateDescription.InnerHtml = Me.Language.Translate("Document.ActualDescription", Me.DefaultScope, strParameters)

                    If newStatus = DocumentStatus.Pending Then
                        Me.lblApprovedByDescription.InnerHtml = Me.Language.Translate("Document.WaitingApproval", Me.DefaultScope)
                    Else
                        strParameters.Clear()
                        strParameters.Add(Me.Language.Translate("Document.Action" & newStatus.ToString, DefaultScope))
                        strParameters.Add(WLHelperWeb.CurrentPassport.Name)

                        strParameters.Add(documentActualizado.StatusLevel)

                        Me.lblApprovedByDescription.InnerHtml = Me.Language.Translate("Document.ApprovedByDescription", Me.DefaultScope, strParameters)
                    End If
                End If

                If Me.CallbackSession.JSProperties.ContainsKey("cpResultRO") Then
                    Me.CallbackSession.JSProperties("cpResultRO") = strParameter.Trim.ToUpperInvariant
                Else
                    Me.CallbackSession.JSProperties.Add("cpResultRO", strParameter.Trim.ToUpperInvariant)
                End If
        End Select
    End Sub

End Class