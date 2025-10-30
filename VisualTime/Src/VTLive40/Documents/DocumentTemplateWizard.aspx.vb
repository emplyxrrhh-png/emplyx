Imports DevExpress.Web
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTNotifications.Notifications
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Public Class DocumentTemplateWizard
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

                tbAvailableNotifications = API.NotificationServiceMethods.GetNotifications(Me.Page, " ID between 701 and 750 ", True, False)
                Session("DocumentTemplates_AvailableNotifications") = tbAvailableNotifications
            End If

            Return tbAvailableNotifications

        End Get
    End Property

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
        InsertExtraJavascript("Documents", "~/Documents/Scripts/DocumentsWizard.js")
        InsertExtraJavascript("DocumentTemplate", "~/Documents/Scripts/DocumentTemplateWizard.js")
        InsertExtraCssIncludes("~/Documents/Styles/DocumentTemplateWizard.css")

    End Sub

    Public Sub New()
        Me.OverrrideDefaultScope = "DocumentTemplate"
        Me.OverrrideLanguageFile = "LiveDocumentaryManagement"
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

        Me.msgHasChanges.Value = Me.Language.Translate("msgHasChanges", DefaultScope)
        Me.msgHasErrors.Value = Me.Language.Translate("msgHasErrors", DefaultScope)

        lblArea.Style("margin-top") = "11px;"
        lblDocValidity.Style("margin-top") = "11px;"

        If Not IsPostBack AndAlso Not IsCallback Then
            If Not Request.QueryString("IDdocumentTemplate") Is Nothing Then
                Me.IDLoadDocumentTemplate.Value = Request.QueryString("IDdocumentTemplate")
            End If

            LoadAmbitRadioButtonList()
            LoadRadioButtonList()

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
        panDocLOPD.Style("display") = "none"

        Select Case oParameters.aTab
            Case 0
                panDocGeneral.Style("display") = ""
            Case 1
                panDocControl.Style("display") = ""
            Case 2
                panDocScope.Style("display") = ""
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

            hiddenDocumentTemplateID("ID") = oParameters.ID
            txtDocName.Text = oCurrentDocument.Name

            rblDocumentArea.Items.FindByValue(oCurrentDocument.Area.ToString).Selected = True

            If oParameters.ID = -1 Then
                oCurrentDocument.BeginValidity = DateTime.Now.Date
            End If
            ckValidPeriod.Checked = oCurrentDocument.BeginValidity <> New Date(1900, 1, 1)

            Dim lstCompany = GetDocumentScopes(True, False)

            If lstCompany.Any(Function(x) x = oCurrentDocument.Scope) Then
                rblAmbit.Items.FindByValue("1").Selected = True
            Else
                rblAmbit.Items.FindByValue("0").Selected = True
            End If

            If rblAmbit.SelectedItem.Value = "0" Then

                rblEmployeeContract.Checked = DocumentScope.EmployeeContract = oCurrentDocument.Scope
                rblEmployeeAccessAuthorization.Checked = DocumentScope.EmployeeAccessAuthorization = oCurrentDocument.Scope
                rblLeaveOrPermission.Checked = DocumentScope.LeaveOrPermission = oCurrentDocument.Scope
                rblCauseNote.Checked = DocumentScope.CauseNote = oCurrentDocument.Scope
            Else
                rblCompany.Checked = DocumentScope.Company = oCurrentDocument.Scope
                rblCompanyAccessAuthorization.Checked = DocumentScope.CompanyAccessAuthorization = oCurrentDocument.Scope

            End If

            rblMandatory.Items.FindByValue(oCurrentDocument.Compulsory.ToString).Selected = True

            ckCanAddDocumentEmployee.Checked = oCurrentDocument.EmployeeDeliverAllowed
            ckCanAddDocumentSupervisor.Checked = oCurrentDocument.SupervisorDeliverAllowed
            ckSystemDocument.Checked = oCurrentDocument.IsSystem

            If Not roTypes.Any2DateTime(oCurrentDocument.BeginValidity).Equals(New Date(1900, 1, 1)) AndAlso ckValidPeriod.Checked Then
                dpPeriodStart.Value = roTypes.Any2DateTime(oCurrentDocument.BeginValidity)
            Else
                dpPeriodStart.Value = Nothing
            End If

            Dim oSelValues As String() = {}

            If oCurrentDocument.Notifications IsNot Nothing Then oSelValues = oCurrentDocument.Notifications.Split(",")

            ckNotification700.Checked = oSelValues.Contains("700")
            ckNotification701.Checked = oSelValues.Contains("701")
            ckNotification702.Checked = oSelValues.Contains("702")
            ckNotification703.Checked = oSelValues.Contains("703")

            cmbLopdLevel.Items.FindByValue(oCurrentDocument.LOPDAccessLevel).Selected = True

            rbnPanelExpireOnServer.Checked = False
            rbnPanelnoExpire.Checked = False

            If oCurrentDocument.DaysBeforeDelete = -1 Then
                rbnPanelExpireOnServer.Checked = True
            ElseIf oCurrentDocument.DaysBeforeDelete = 0 Then
                rbnPanelnoExpire.Checked = True
            End If

            If (oCurrentDocument.ApprovalLevelRequired > 0) Then
                ckApproveRequiered.Checked = True
                txtRequieredSupervisorLevel.Value = oCurrentDocument.ApprovalLevelRequired
            Else
                ckApproveRequiered.Checked = False
                txtRequieredSupervisorLevel.Value = 0
            End If

            ckExpireOld.Checked = oCurrentDocument.ExpirePrevious

            If HelperSession.AdvancedParametersCache("VTLive.Edition").ToString.ToLower = roServerLicense.roVisualTimeEdition.Starter.ToString.ToLower Then
                divDocumentAccess.Style("display") = "none"
                rblLeaveOrPermission.Enabled = False
                rblMandatory.Enabled = False
                rblCauseNote.Enabled = False
                ckValidPeriod.Enabled = False
                dpPeriodStart.Enabled = False
                rblCompany.Checked = True
                rblCompanyAccessAuthorization.Enabled = False
                ckNotification701.Enabled = False
                ckNotification702.Enabled = False
            End If
            If HelperSession.GetFeatureIsInstalledFromApplication("Feature\Signdocument") = False Then
                ckRequieresSign.Enabled = False
            Else
                ckRequieresSign.Enabled = True
                ckRequieresSign.Checked = oCurrentDocument.RequiresSigning
            End If

            If (oCurrentDocument.DefaultExpiration Is Nothing OrElse oCurrentDocument.DefaultExpiration = "0") Then
                rbValidUntil.Checked = False
                rbAlways.Checked = True
                txtExpireDays.Value = "0"
            Else
                rbValidUntil.Checked = True
                rbAlways.Checked = False
                txtExpireDays.Value = oCurrentDocument.DefaultExpiration.Split("@")(1)

            End If

            rbnNonCriticality.Checked = False
            rbnAdviceCriticality.Checked = False
            rbnDeniedCriticality.Checked = False

            Select Case oCurrentDocument.AccessValidation
                Case DocumentAccessValidation.NonCritical
                    rbnNonCriticality.Checked = True
                Case DocumentAccessValidation.Advise
                    rbnAdviceCriticality.Checked = True
                Case DocumentAccessValidation.AccessDenied
                    rbnDeniedCriticality.Checked = True
            End Select

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

            oCurrentDocument.Compulsory = rblMandatory.SelectedItem.Value
            oCurrentDocument.EmployeeDeliverAllowed = ckCanAddDocumentEmployee.Checked
            oCurrentDocument.IsSystem = ckSystemDocument.Checked

            oCurrentDocument.Area = System.Enum.Parse(GetType(DocumentArea), rblDocumentArea.SelectedItem.Value)

            If rblAmbit.SelectedItem.Value = "0" Then
                oCurrentDocument.SupervisorDeliverAllowed = True
            Else
                oCurrentDocument.SupervisorDeliverAllowed = ckCanAddDocumentSupervisor.Checked
            End If

            If bIsnew Then
                If rblEmployeeContract.Checked Then
                    oCurrentDocument.Scope = DocumentScope.EmployeeContract
                ElseIf rblEmployeeAccessAuthorization.Checked Then
                    oCurrentDocument.Scope = DocumentScope.EmployeeAccessAuthorization
                ElseIf rblLeaveOrPermission.Checked Then
                    oCurrentDocument.Scope = DocumentScope.LeaveOrPermission
                ElseIf rblCauseNote.Checked Then
                    oCurrentDocument.Scope = DocumentScope.CauseNote
                ElseIf rblCompany.Checked Then
                    oCurrentDocument.Scope = DocumentScope.Company
                ElseIf rblCompanyAccessAuthorization.Checked Then
                    oCurrentDocument.Scope = DocumentScope.CompanyAccessAuthorization
                End If
            End If

            If dpPeriodStart.Value Is Nothing OrElse Not ckValidPeriod.Checked Then
                oCurrentDocument.BeginValidity = New Date(1900, 1, 1)
            Else
                oCurrentDocument.BeginValidity = dpPeriodStart.Value
            End If
            'oCurrentDocument.BeginValidity = If(dpPeriodStart.Value Is Nothing, New Date(1900, 1, 1), dpPeriodStart.Value)
            oCurrentDocument.EndValidity = New Date(2079, 1, 1)

            oCurrentDocument.Notifications = ""

            If ckNotification700.Checked Then
                oCurrentDocument.Notifications &= "700,"
            End If
            If ckNotification701.Checked Then
                oCurrentDocument.Notifications &= "701,"
            End If
            If ckNotification702.Checked Then
                oCurrentDocument.Notifications &= "702,"
            End If
            If ckNotification703.Checked Then
                oCurrentDocument.Notifications &= "703,"
            End If

            If oCurrentDocument.Notifications <> String.Empty Then oCurrentDocument.Notifications = oCurrentDocument.Notifications.Substring(0, oCurrentDocument.Notifications.Length - 1)

            oCurrentDocument.LOPDAccessLevel = cmbLopdLevel.SelectedItem.Value

            If rbnNonCriticality.Checked Then
                oCurrentDocument.AccessValidation = DocumentAccessValidation.NonCritical
            ElseIf rbnAdviceCriticality.Checked Then
                oCurrentDocument.AccessValidation = DocumentAccessValidation.Advise
            ElseIf rbnDeniedCriticality.Checked Then
                oCurrentDocument.AccessValidation = DocumentAccessValidation.AccessDenied

            End If

            If rbnPanelExpireOnServer.Checked Then
                oCurrentDocument.DaysBeforeDelete = -1
            ElseIf rbnPanelnoExpire.Checked Then
                oCurrentDocument.DaysBeforeDelete = 0
            End If

            oCurrentDocument.AccessValidation = GetCriticality()

            oCurrentDocument.ApprovalLevelRequired = If(Not ckApproveRequiered.Checked, 0, roTypes.Any2Integer(txtRequieredSupervisorLevel.Value))
            oCurrentDocument.ExpirePrevious = ckExpireOld.Checked

            oCurrentDocument.RequiresSigning = ckRequieresSign.Checked

            oCurrentDocument.DefaultExpiration = If(rbAlways.Checked, "0", "1@" & txtExpireDays.Value)
            oCurrentDocument.BeginValidity = CDate(oCurrentDocument.BeginValidity.ToShortDateString)

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
                'LoadDocumentTemplate(oParameters, oCurrentDocument)
                ASPxCallbackPanelContenido.JSProperties("cpIsNewRO") = bIsnew
                'ASPxCallbackPanelContenido.JSProperties("cpNewObjId") = oCurrentDocument.Id
                ASPxCallbackPanelContenido.JSProperties("cpObjectId") = oCurrentDocument.Id
                ASPxCallbackPanelContenido.JSProperties("cpActionRO") = "SAVEDOCUMENT"
            Else
                'LoadDocumentTemplate(oParameters, oCurrentDocument)
                ASPxCallbackPanelContenido.JSProperties("cpResultRO") = "KO"
                ASPxCallbackPanelContenido.JSProperties("cpActionRO") = "SAVEDOCUMENT"
                ASPxCallbackPanelContenido.JSProperties.Add("cpMessageRO", strMessage)

            End If

        End Try
    End Sub

#End Region

#Region "Métodos Generales"

    Private Function GetDocumentScopes(addCompanyScopes As Boolean, addEmployeeScopes As Boolean) As List(Of DocumentScope)

        Dim result As New List(Of DocumentScope)

        Dim companyScopes As New List(Of DocumentScope) From {
            DocumentScope.Company,
            DocumentScope.CompanyAccessAuthorization
        }
        Dim employeeScopes As New List(Of DocumentScope) From {
            DocumentScope.EmployeeField,
            DocumentScope.EmployeeContract,
            DocumentScope.EmployeeAccessAuthorization,
            DocumentScope.LeaveOrPermission,
            DocumentScope.CauseNote
        }

        If addCompanyScopes Then
            result.AddRange(companyScopes)
        End If

        If addEmployeeScopes Then
            result.AddRange(employeeScopes)
        End If

        Return result
    End Function

    Private Sub LoadAmbitRadioButtonList()
        rblAmbit.Items.Clear()
        rblAmbit.Items.Add(Language.Translate("Employee", DefaultScope), 0)
        rblAmbit.Items.Add(Language.Translate("Company", DefaultScope), 1)
    End Sub

    Private Sub LoadScopeRadioButtonList(ByRef rblScope As ASPxRadioButtonList, addCompanyScopes As Boolean, addEmployeeScopes As Boolean)
        rblScope.Items.Clear()

        Dim lst = GetDocumentScopes(addCompanyScopes, addEmployeeScopes)

        Dim GPALegacyMode As Boolean = roTypes.Any2Boolean(HelperSession.AdvancedParametersCache("GPADocumentation.LegacyMode"))

        For Each eType As DocumentScope In lst
            Dim translatedType = Language.Translate("Scope." & eType.ToString, DefaultScope)
            If eType = DocumentScope.LeaveOrPermission Then
                If (GPALegacyMode = False) Then
                    rblScope.Items.Add(translatedType, eType)
                End If
            Else
                rblScope.Items.Add(translatedType, eType)
            End If
        Next

    End Sub

    Private Sub LoadRadioButtonList()

        rblEmployeeContract.Text = Language.Translate("Scope." & DocumentScope.EmployeeContract.ToString, DefaultScope)
        rblEmployeeAccessAuthorization.Text = Language.Translate("Scope." & DocumentScope.EmployeeAccessAuthorization.ToString, DefaultScope)
        rblLeaveOrPermission.Text = Language.Translate("Scope." & DocumentScope.LeaveOrPermission.ToString, DefaultScope)
        rblCauseNote.Text = Language.Translate("Scope." & DocumentScope.CauseNote.ToString, DefaultScope)
        rblCompany.Text = Language.Translate("Scope." & DocumentScope.Company.ToString, DefaultScope)
        rblCompanyAccessAuthorization.Text = Language.Translate("Scope." & DocumentScope.CompanyAccessAuthorization.ToString, DefaultScope)

        cmbLopdLevel.ValueType = GetType(Integer)
        cmbLopdLevel.Items.Clear()

        cmbLopdLevel.Items.Add(Language.Translate("LOPD.Low", DefaultScope), 0)
        cmbLopdLevel.Items.Add(Language.Translate("LOPD.Medium", DefaultScope), 1)
        cmbLopdLevel.Items.Add(Language.Translate("LOPD.High", DefaultScope), 2)

        rblMandatory.Items.Clear()

        rblMandatory.Items.Add(Language.Translate("Optionals", DefaultScope), False)
        rblMandatory.Items.Add(Language.Translate("Mandatories", DefaultScope), True)

        rblDocumentArea.Items.Clear()

        For Each eArea As DocumentArea In System.Enum.GetValues(GetType(DocumentArea))
            rblDocumentArea.Items.Add(Language.Translate("Area." & eArea.ToString, DefaultScope), eArea)
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