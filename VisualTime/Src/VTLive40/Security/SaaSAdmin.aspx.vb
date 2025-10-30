Imports DevExpress.Web
Imports Robotics
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common.AdvancedParameter
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class SaaSAdmin
    Inherits PageBase

    <Runtime.Serialization.DataContract()>
    Private Class CallbackRequestObject

        <Runtime.Serialization.DataMember(Name:="action")>
        Public Action As String

        <Runtime.Serialization.DataMember(Name:="StampParam")>
        Public StampParam As Double

        <Runtime.Serialization.DataMember(Name:="newTemplateName")>
        Public NewTemplateName As String

    End Class

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        If WLHelperWeb.CurrentPassport IsNot Nothing AndAlso WLHelperWeb.CurrentUserIsConsultantOrCegid OrElse WLHelperWeb.CurrentPassportID = 1 Then
            Me.InsertExtraJavascript("SaaSAdmin", "~/Security/Scripts/SaaSAdmin.js")
        End If
    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If WLHelperWeb.CurrentPassport Is Nothing Then
            WLHelperWeb.RedirectAccessDenied(False)
            Return
        Else
            'advTemplate.RibbonMode = ASPxSpreadsheet.SpreadsheetRibbonMode.None

            If WLHelperWeb.CurrentUserIsConsultantOrCegid OrElse WLHelperWeb.CurrentPassportID = 1 Then

                If Not Me.IsPostBack AndAlso Not Me.IsCallback Then
                    advTemplate.CreateDefaultRibbonTabs(True)
                    HideribbonTab(advTemplate, 0)
                    HideribbonTab(advTemplate, 1)
                    advTemplate.Document.CreateNewDocument()
                End If

                If Not Me.IsPostBack Then

                    cmbLevel.Items.Clear()
                    For i As Integer = 0 To 10
                        cmbLevel.Items.Add(i, i.ToString)
                    Next
                    Me.cmbLevel.SelectedIndex = 1

                    Dim x As DataTable = Nothing
                    x = API.UserAdminServiceMethods.GetPassportsLite(Me.Page, LoadType.User)
                    cmbPassports.Items.Clear()
                    If x IsNot Nothing AndAlso x.Rows.Count > 0 Then
                        For Each z As DataRow In x.Rows
                            If Not roTypes.Any2String(z("Description")).Contains("@@ROBOTICS@@") Then
                                cmbPassports.Items.Add(z("UserName"), z("UserID"))
                            End If
                        Next
                    End If

                    If cmbPassports.Items.Count > 0 Then
                        cmbPassports.SelectedIndex = 0
                    End If

                    Dim serviceStatus As Boolean = False
                    API.EmployeeGroupsServiceMethods.GetServiceStatus(serviceStatus, Me.Page)

                    If serviceStatus Then
                        lblStatusResponse.Text = Me.Language.Translate("Status.Active", Me.DefaultScope)
                    Else
                        lblStatusResponse.Text = Me.Language.Translate("Status.Inactive", Me.DefaultScope)
                    End If

                    lblChangeModeInfoV1_V3a.Visible = False
                    lblChangeModeInfoV2_V3a.Visible = False
                    lblChangeModeInfoV2_V3.Visible = False

                    btnCreateChart.Visible = False
                    lblLevelTitle.Visible = False
                    cmbLevel.Visible = False
                    btnActivateMode.Visible = False
                    lblPassportTitle.Visible = False
                    cmbPassports.Visible = False
                    btnEliminateExceptions.Visible = False
                    securitydesc.Style("display") = "none"

                    lblModeResponse.Text = "V3 (Seguridad con Categorías)"

                    lblCalendarMode.Text = "V2 (Nuevo Calendario)"

                    Dim requestsPunch As String = HelperSession.AdvancedParametersCache("VTPortal.RequestOnForgottenPunch")
                    If (requestsPunch.ToUpper = "TRUE") Then
                        lblPunchRequestsResponse.Text = "Activo: los fichajes olvidados se crean a través de solicitud"
                        btnDectivatePunchRequests.Visible = True
                        btnActivatePunchRequests.Visible = False
                        lblChangePunchRequests.Visible = True
                        lblChangePunchRequests2.Visible = True
                    Else
                        lblPunchRequestsResponse.Text = "Desactivado: los fichajes olvidados se crean sin necesidad de tramitar solicitud"
                        btnActivatePunchRequests.Visible = True
                        btnDectivatePunchRequests.Visible = False
                        lblChangePunchRequests.Visible = False
                        lblChangePunchRequests2.Visible = False
                    End If


                    lblGeniusResponse.Text = "Activo"
                    btnActivateGenius.Visible = False
                    lblChangeGenius.Visible = False
                    lblChangeGenius2.Visible = False

                    Dim hasAlternativeShiftsResult = HasAlternativeShifts()
                    btnDeleteAlternativeShifts.Visible = hasAlternativeShiftsResult
                    lblHasAlternativeShifts.Text = Language.Translate(IIf(hasAlternativeShiftsResult, "WithAlternativeShifts", "WithoutAlternativeShifts"), DefaultScope)

                End If
            Else
                WLHelperWeb.RedirectAccessDenied(False)
                Return
            End If

            Dim oGroupFeatures = API.SecurityChartServiceMethods.GetGroupFeatures(Me.Page)
            With Me.cmbSecurityFunctions
                .Items.Clear()
                .ValueType = GetType(Integer)
                For Each oGroupFeature In oGroupFeatures
                    .Items.Add(oGroupFeature.Name, oGroupFeature.ID)
                Next
            End With
        End If

    End Sub

    Protected Sub ASPxCallbackPanelContenido_Callback(ByVal sender As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase) Handles ASPxCallbackPanelContenido.Callback
        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Dim oParameters As New CallbackRequestObject()
        oParameters = roJSONHelper.Deserialize(strParameter, oParameters.GetType())

        Dim result As String = String.Empty

        If WLHelperWeb.CurrentPassport IsNot Nothing AndAlso WLHelperWeb.CurrentUserIsConsultantOrCegid OrElse WLHelperWeb.CurrentPassportID = 1 Then
            Select Case oParameters.Action
                Case "ActivateService"
                    result = ActivateServie()
                Case "CancelService"
                    result = CancelService()
                Case "RegeneratePwd"
                    result = RegenerateAllPwd()
                Case "DoSimpleBackup"
                    result = DoSimpleBackup()
                Case "DoFullBackup"
                    result = DoFullBackup()
                Case "ChangeMode"
                    result = ChangeMode()
                Case "DeactivatePunchRequests"
                    result = DeactivatePunchRequests()
                Case "ActivatePunchRequests"
                    result = ActivatePunchRequests()
                Case "GenerateNode"
                    result = GenerateNode()
                Case "queryParameter"
                    result = QueryAdvancedParameter()
                Case "SaveParameter"
                    result = SaveAdvancedParameter()
                Case "SaveUser"
                    result = SaveUser()
                Case "DeleteAlternativeShifts"
                    result = DeleteAlternativeShifts()
                Case "DeleteExceptions"
                    result = DeleteSupervisorExceptions()
                Case "LoadExportTemplates"
                    result = LoadExportTemplates()
                Case "LoadExportTemplatesAvailable"
                    result = LoadExportTemplatesAvailable(roTypes.Any2String(oParameters.NewTemplateName))
                Case "LoadFileTemplateContent"
                    result = LoadFileTemplateContent()
            End Select
        Else
            result = "ERRORNO_ROBOTICS_USER"
        End If

        Dim parameters As New Generic.List(Of String)
        parameters.Add(WLHelperWeb.CurrentPassport.Name)

        Dim tagMessage As String = result
        Dim msg As String = String.Empty
        If tagMessage.StartsWith("OK") Then
            tagMessage = tagMessage.Substring(3)
            msg = Me.Language.Translate("SaaSAction.Result." & tagMessage, Me.DefaultScope, parameters)
        ElseIf tagMessage.StartsWith("ERROR") Then
            tagMessage = tagMessage.Substring(6)

            If tagMessage = "ERROR" Then
                msg = Me.Language.Translate("SaaSAction.Result." & tagMessage, Me.DefaultScope, parameters)
            Else
                msg = tagMessage
            End If
        ElseIf tagMessage.StartsWith("ACTION") Then
            result = "OK"
        End If

        If msg <> String.Empty Then
            result = "MESSAGE" &
              "TitleKey=SaaSAction.Result.Title&" +
              "DescriptionText=" + msg + "&" +
              "Option1TextKey=SaaSAction.Result.Option1Text&" +
              "Option1DescriptionKey=SaaSAction.Result.Option1Description&" +
              "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
              "IconUrl=" & IIf(result.ToUpper.Trim.StartsWith("ERROR") = False, HelperWeb.MsgBoxIconsUrl(HelperWeb.MsgBoxIcons.InformationIcon), HelperWeb.MsgBoxIconsUrl(HelperWeb.MsgBoxIcons.ErrorIcon))
        End If

        ASPxCallbackPanelContenido.JSProperties.Add("cpActionRO", oParameters.Action)
        ASPxCallbackPanelContenido.JSProperties.Add("cpResultRO", result)
    End Sub

#Region "Templates"

    Public Shared Sub HideribbonTab(ByVal spreadsheet As ASPxSpreadsheet.ASPxSpreadsheet, ByVal tabIndex As Integer)
        Dim fileTab As RibbonTab = spreadsheet.RibbonTabs(tabIndex)
        spreadsheet.RibbonTabs.Remove(fileTab)
    End Sub

    Public Shared Sub HideRibbonItem(ByVal spreadsheet As ASPxSpreadsheet.ASPxSpreadsheet, ByVal tabIndex As Integer, ByVal groupIndex As Integer, ByVal itemIndex As Integer)
        Dim group As RibbonGroup = spreadsheet.RibbonTabs(tabIndex).Groups(groupIndex)
        Dim ribbonItem As RibbonItemBase = group.Items(itemIndex)
        group.Items.Remove(ribbonItem)
    End Sub

    Public Function LoadExportTemplates() As String
        Dim bResult As String = "ACTION"

        Dim oList As Generic.List(Of roDatalinkGuide) = API.DataLinkGuideServiceMethods.GetDatalinkGuides(Nothing, False)

        cmbAdvTemplateType.Items.Clear()
        For Each oElement In oList
            Dim bInclude As Boolean = False
            For Each oTemplate In oElement.Export.Templates
                If oTemplate.TemplateFile <> String.Empty Then bInclude = True
            Next

            If bInclude Then
                If oElement.Export IsNot Nothing AndAlso oElement.Export.Id > 0 Then
                    cmbAdvTemplateType.Items.Add(New ListEditItem(oElement.Name, oElement.Export.Id))
                End If
            End If
        Next

        If cmbAdvTemplateType.Items.Count > 0 Then
            cmbAdvTemplateType.SelectedItem = cmbAdvTemplateType.Items(0)
            cmbAdvTemplateName.Items.Clear()

            Dim oGuide As roDatalinkGuide = oList.Find(Function(x) x.Export.Id = cmbAdvTemplateType.Items(0).Value)
            If oGuide IsNot Nothing Then
                For Each oTemplate In oGuide.Export.Templates
                    If oTemplate.TemplateFile <> String.Empty Then cmbAdvTemplateName.Items.Add(oTemplate.Name, oTemplate.ID)
                Next
            End If

            If cmbAdvTemplateName.Items.Count > 0 AndAlso cmbAdvTemplateName.SelectedItem Is Nothing Then
                cmbAdvTemplateName.SelectedItem = cmbAdvTemplateName.Items(0)
            End If
        End If

        LoadTemplateIntoExcel()

        Return bResult
    End Function

    Public Function LoadExportTemplatesAvailable(ByVal overrideTemplateSelected As String) As String
        Dim bResult As String = "ACTION"

        Dim oList As Generic.List(Of roDatalinkGuide) = API.DataLinkGuideServiceMethods.GetDatalinkGuides(Nothing, False)

        For Each oElement In oList
            Dim bInclude As Boolean = False
            For Each oTemplate In oElement.Export.Templates
                If oTemplate.TemplateFile <> String.Empty Then bInclude = True
            Next

            If bInclude Then
                If oElement.Export IsNot Nothing AndAlso oElement.Export.Id > 0 Then
                    cmbAdvTemplateType.Items.Add(New ListEditItem(oElement.Name, oElement.Export.Id))
                End If
            End If
        Next

        Dim selTemplateType As String = String.Empty
        If cmbAdvTemplateType.SelectedItem IsNot Nothing Then
            selTemplateType = cmbAdvTemplateType.SelectedItem.Value
        End If

        If cmbAdvTemplateType.Items.Count > 0 AndAlso selTemplateType <> String.Empty Then
            cmbAdvTemplateName.Items.Clear()

            Dim oGuide As roDatalinkGuide = oList.Find(Function(x) x.Export.Id = selTemplateType)
            If oGuide IsNot Nothing Then
                For Each oTemplate In oGuide.Export.Templates
                    If oTemplate.TemplateFile <> String.Empty Then cmbAdvTemplateName.Items.Add(oTemplate.Name, oTemplate.ID)
                Next
            End If

            If overrideTemplateSelected <> String.Empty Then
                cmbAdvTemplateName.SelectedItem = cmbAdvTemplateName.Items.FindByText(overrideTemplateSelected)
            Else
                If cmbAdvTemplateName.Items.Count > 0 AndAlso cmbAdvTemplateName.SelectedItem Is Nothing Then
                    cmbAdvTemplateName.SelectedItem = cmbAdvTemplateName.Items(0)
                End If
            End If
        End If

        LoadTemplateIntoExcel()

        Return bResult
    End Function

    Public Function LoadFileTemplateContent() As String
        Dim bResult As String = "ACTION"

        Dim oList As Generic.List(Of roDatalinkGuide) = API.DataLinkGuideServiceMethods.GetDatalinkGuides(Nothing, False)

        For Each oElement In oList

            Dim bInclude As Boolean = False
            For Each oTemplate In oElement.Export.Templates
                If oTemplate.TemplateFile <> String.Empty Then bInclude = True
            Next

            If bInclude Then
                If oElement.Export IsNot Nothing AndAlso oElement.Export.Id > 0 Then
                    cmbAdvTemplateType.Items.Add(New ListEditItem(oElement.Name, oElement.Export.Id))
                End If
            End If
        Next

        Dim selTemplateType As String = String.Empty
        If cmbAdvTemplateType.SelectedItem IsNot Nothing Then
            selTemplateType = cmbAdvTemplateType.SelectedItem.Value
        End If

        If cmbAdvTemplateType.Items.Count > 0 AndAlso selTemplateType <> String.Empty Then

            Dim oGuide As roDatalinkGuide = oList.Find(Function(x) x.Export.Id = selTemplateType)
            If oGuide IsNot Nothing Then
                For Each oTemplate In oGuide.Export.Templates
                    If oTemplate.TemplateFile <> String.Empty Then cmbAdvTemplateName.Items.Add(oTemplate.Name, oTemplate.ID)
                Next
            End If
        End If

        LoadTemplateIntoExcel()

        Return bResult
    End Function

    Public Sub LoadTemplateIntoExcel()
        If cmbAdvTemplateType.SelectedItem IsNot Nothing AndAlso cmbAdvTemplateName.SelectedItem IsNot Nothing Then
            Dim bContent As Byte() = {}
            bContent = API.DataLinkGuideServiceMethods.GetExportTemplateBytes(Nothing, roTypes.Any2Integer(cmbAdvTemplateType.SelectedItem.Value), roTypes.Any2Integer(cmbAdvTemplateName.SelectedItem.Value))

            If bContent IsNot Nothing AndAlso bContent.Length > 0 Then
                Dim strTempFileName = System.IO.Path.GetTempFileName()

                System.IO.File.WriteAllBytes(strTempFileName, bContent)
                DevExpress.Web.Office.DocumentManager.CloseAllDocuments()
                advTemplate.Open(strTempFileName)
            End If

        End If

    End Sub

    Public Sub CallbackExcel_Callback(ByVal sender As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase) Handles CallbackExcel.Callback
        Dim bRet As Boolean = False
        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Dim oParameters As New CallbackRequestObject()
        oParameters = roJSONHelper.Deserialize(strParameter, oParameters.GetType())

        Dim oList As Generic.List(Of roDatalinkGuide) = API.DataLinkGuideServiceMethods.GetDatalinkGuides(Nothing, False)

        For Each oElement In oList
            Dim bInclude As Boolean = False
            For Each oTemplate In oElement.Export.Templates
                If oTemplate.TemplateFile <> String.Empty Then bInclude = True
            Next

            If bInclude Then
                If oElement.Export IsNot Nothing AndAlso oElement.Export.Id > 0 Then
                    cmbAdvTemplateType.Items.Add(New ListEditItem(oElement.Name, oElement.Export.Id))
                End If
            End If

        Next

        Dim selTemplateType As String = String.Empty
        If cmbAdvTemplateType.SelectedItem IsNot Nothing Then
            selTemplateType = cmbAdvTemplateType.SelectedItem.Value
        End If

        If cmbAdvTemplateType.Items.Count > 0 AndAlso selTemplateType <> String.Empty Then

            Dim oGuide As roDatalinkGuide = oList.Find(Function(x) x.Export.Id = selTemplateType)
            If oGuide IsNot Nothing Then
                For Each oTemplate In oGuide.Export.Templates
                    If oTemplate.TemplateFile <> String.Empty Then cmbAdvTemplateName.Items.Add(oTemplate.Name, oTemplate.ID)
                Next
            End If
        End If

        If oParameters.Action = "SAVEEDITOR" Then
            If cmbAdvTemplateType.SelectedItem IsNot Nothing AndAlso cmbAdvTemplateName.SelectedItem IsNot Nothing Then

                advTemplate.Document.SaveDocument(advTemplate.Document.Path)
                Dim bTemplateContent As Byte() = System.IO.File.ReadAllBytes(advTemplate.Document.Path)
                bRet = API.DataLinkGuideServiceMethods.SaveExportTemplateBytes(Nothing, bTemplateContent, roTypes.Any2Integer(cmbAdvTemplateType.SelectedItem.Value), roTypes.Any2Integer(cmbAdvTemplateName.SelectedItem.Value))
            End If
        ElseIf oParameters.Action = "DUPLICATE" Then
            If cmbAdvTemplateType.SelectedItem IsNot Nothing AndAlso cmbAdvTemplateName.SelectedItem IsNot Nothing Then
                advTemplate.Document.SaveDocument(advTemplate.Document.Path)
                Dim bTemplateContent As Byte() = System.IO.File.ReadAllBytes(advTemplate.Document.Path)
                bRet = API.DataLinkGuideServiceMethods.DuplicateExportTemplateBytes(Nothing, bTemplateContent, roTypes.Any2Integer(cmbAdvTemplateType.SelectedItem.Value), roTypes.Any2Integer(cmbAdvTemplateName.SelectedItem.Value), oParameters.NewTemplateName)
            End If
        End If

        CallbackExcel.JSProperties.Add("cpActionRO", oParameters.Action)
        CallbackExcel.JSProperties.Add("cpResultRO", If(bRet, "OK", "ERROR"))

    End Sub

#End Region

#Region "Server management"


    Private Function DeactivatePunchRequests() As String

        If WLHelperWeb.CurrentUserIsConsultant Then
            Dim oAdvancedParameter As roAdvancedParameter = Nothing
            oAdvancedParameter = API.CommonServiceMethods.GetAdvancedParameter(Nothing, "VTPortal.RequestOnForgottenPunch")
            If (oAdvancedParameter IsNot Nothing) Then

                oAdvancedParameter.Value = "false"

                Dim bResult As Boolean = API.CommonServiceMethods.SaveAdvancedParameter(Me.Page, oAdvancedParameter, True)

                If bResult Then
                    bResult = Robotics.DataLayer.AccessHelper.ExecuteSql("@update# requests set status=2, comments='Aprobada automáticamente por VisualTime' where requesttype=2 and status in (0,1)")

                    If bResult Then
                        DataLayer.roCacheManager.GetInstance.UpdateParamCache()
                        Global_asax.ResetSystemCache()
                        Dim lstAuditParameterNames As New List(Of String)
                        Dim lstAuditParameterValues As New List(Of String)
                        lstAuditParameterNames.Add("{Requests}")
                        lstAuditParameterValues.Add("")
                        API.AuditServiceMethods.Audit(Robotics.VTBase.Audit.Action.aUpdate, Robotics.VTBase.Audit.ObjectType.tRequest, "Aprobación masiva de solicitudes", lstAuditParameterNames, lstAuditParameterValues, Me.Page)
                    End If
                End If

                Return If(bResult, "OK#OK", "ERROR#ERROR")
            Else
                Return "ERROR#ERROR"
            End If
        Else
            Return "ERROR#NO_AUTHORIZED"
        End If
    End Function

    Private Function ActivatePunchRequests() As String

        If WLHelperWeb.CurrentUserIsConsultant Then
            Dim oAdvancedParameter As roAdvancedParameter = Nothing
            oAdvancedParameter = API.CommonServiceMethods.GetAdvancedParameter(Nothing, "VTPortal.RequestOnForgottenPunch")
            If (oAdvancedParameter IsNot Nothing) Then
                oAdvancedParameter.Value = "true"
                Dim bResult As Boolean = API.CommonServiceMethods.SaveAdvancedParameter(Me.Page, oAdvancedParameter, True)
                If bResult Then
                    DataLayer.roCacheManager.GetInstance.UpdateParamCache()
                    Global_asax.ResetSystemCache()
                    Dim lstAuditParameterNames As New List(Of String)
                    Dim lstAuditParameterValues As New List(Of String)
                    lstAuditParameterNames.Add("{Requests}")
                    lstAuditParameterValues.Add("")
                    API.AuditServiceMethods.Audit(Robotics.VTBase.Audit.Action.aUpdate, Robotics.VTBase.Audit.ObjectType.tRequest, "Activación solicitudes fichajes olvidados", lstAuditParameterNames, lstAuditParameterValues, Me.Page)
                End If
                Return If(bResult, "OK#OK", "ERROR#ERROR")
            Else
                Return "ERROR#ERROR"
            End If
        Else
            Return "ERROR#NO_AUTHORIZED"
        End If
    End Function

    Private Function SaveAdvancedParameter() As String
        Dim sOldValue As String = String.Empty
        If WLHelperWeb.CurrentUserIsConsultant Then
            Dim oAdvancedParameter As roAdvancedParameter = API.CommonServiceMethods.GetAdvancedParameter(Nothing, txtParameterName.Text)
            If (oAdvancedParameter IsNot Nothing) Then
                sOldValue = oAdvancedParameter.Value
                oAdvancedParameter.Value = txtParameterValue.Text

                Dim bResult As Boolean = API.CommonServiceMethods.SaveAdvancedParameter(Me.Page, oAdvancedParameter, True)

                If bResult Then
                    DataLayer.roCacheManager.GetInstance.UpdateParamCache()
                    Global_asax.ResetSystemCache()
                End If

                Return If(bResult, "OK#AdvancedParameter.Save.Ok", "ERROR#AdvancedParameter.Save.Error")
            Else
                Return "ERROR#ERROR"
            End If
        Else
            Return "ERROR#NO_AUTHORIZED"
        End If
    End Function

    Private Function SaveUser() As String

        If (String.IsNullOrEmpty(Me.txtPassword.Value) = False) Then

            Dim oNewPassport As New roPassport

            oNewPassport.Name = Me.txtUser.Value
            oNewPassport.Description = Me.txtPassword.Value

            Dim strErrorInfo As String = ""

            oNewPassport.GroupType = ""

            oNewPassport.StartDate = DateTime.Today
            oNewPassport.ExpirationDate = DateTime.Today.AddYears(1)

            oNewPassport.IDEmployee = Nothing
            oNewPassport.Language = WLHelperWeb.CurrentPassport.Language
            oNewPassport.State = 1

            oNewPassport.EnabledVTDesktop = True
            oNewPassport.EnabledVTPortal = True
            oNewPassport.EnabledVTPortalApp = True
            oNewPassport.EnabledVTVisits = True
            oNewPassport.EnabledVTVisitsApp = True
            oNewPassport.LocationRequiered = True
            oNewPassport.PhotoRequiered = True
            oNewPassport.IsSupervisor = True

            oNewPassport.AuthenticationMethods = New roPassportAuthenticationMethods

            Dim oNewMethod = New roPassportAuthenticationMethodsRow
            With oNewMethod
                .IDPassport = oNewPassport.ID
                .Method = AuthenticationMethod.Password
                .Version = ""
                .BiometricID = 0
                .Credential = txtUser.Value
                .Password = CryptographyHelper.EncryptMD5(txtPassword.Value)
                .Enabled = True
                .RowState = RowState.NewRow
            End With
            oNewPassport.AuthenticationMethods.PasswordRow = oNewMethod

            oNewPassport.IDGroupFeature = Me.cmbSecurityFunctions.SelectedItem.Value
            oNewPassport.Categories = New roPassportCategories
            Dim xCategories = New roPassportCategories
            Dim xPassportCategories As New Generic.List(Of roPassportCategoryRow)
            For i As Integer = 0 To 6
                Dim xPassportCategoryRow As New roPassportCategoryRow
                xPassportCategoryRow.IDCategory = i
                xPassportCategoryRow.IDPassport = -1
                xPassportCategoryRow.LevelOfAuthority = 1
                xPassportCategoryRow.ShowFromLevel = 11
                xPassportCategoryRow.RowState = RowState.NewRow
                xPassportCategories.Add(xPassportCategoryRow)
            Next
            oNewPassport.Categories.CategoryRows = xPassportCategories.ToArray

            Dim xPassportGroups As New roPassportGroups
            Dim xPassportGroupRow As New roPassportGroupRow
            xPassportGroupRow.IDGroup = 1
            xPassportGroupRow.IDPassport = -1
            xPassportGroupRow.RowState = RowState.NewRow

            Dim xListGroups As New Generic.List(Of roPassportGroupRow)
            xListGroups.Add(xPassportGroupRow)
            xPassportGroups.GroupRows = xListGroups.ToArray
            xPassportGroups.idPassport = -1

            oNewPassport.Groups = xPassportGroups

            ' Guardamos el passport
            If Not API.UserAdminServiceMethods.SavePassport(Me, oNewPassport, True) Then
                Return "ERROR#ERROR"
            Else
                Return "OK#OK"
            End If
        Else
            Return "ERROR#ERROR"
        End If

    End Function

    Private Function QueryAdvancedParameter() As String
        If WLHelperWeb.CurrentUserIsConsultant Then
            Dim oAdvancedParameter As roAdvancedParameter = Nothing
            oAdvancedParameter = API.CommonServiceMethods.GetAdvancedParameter(Nothing, txtParameterQueryValue.Text, True)
            If (oAdvancedParameter IsNot Nothing) Then
                If oAdvancedParameter.Exists Then
                    txtParameterQueryResult.Text = oAdvancedParameter.Value
                    Return "OK#AdvancedParameter.Query.Result"
                Else
                    txtParameterQueryResult.Text = ""
                    Return "OK#AdvancedParameter.Query.NotExists"
                End If
            Else
                Return "ERROR#ERROR"
            End If
        Else
            Return "NO_AUTHORIZED"
        End If
    End Function

    Private Function GenerateNode() As String
        If WLHelperWeb.CurrentUserIsConsultant Then
            Return "ERROR#ERROR"
        Else
            Return "NO_AUTHORIZED"
        End If

    End Function

    Private Function ChangeMode() As String

        Return "NO_AUTHORIZED"
    End Function

    Private Function ActivateServie() As String
        If WLHelperWeb.CurrentUserIsConsultant Then
            If API.EmployeeGroupsServiceMethods.ActivateService(Me.Page) Then
                lblStatusResponse.Text = Me.Language.Translate("Status.Active", Me.DefaultScope)
                Return "OK#OK"
            Else
                Return "ERROR#ERROR"
            End If
        Else
            Return "NO_AUTHORIZED"
        End If
    End Function

    Private Function CancelService() As String
        If WLHelperWeb.CurrentUserIsConsultant Then
            If API.EmployeeGroupsServiceMethods.CancelService(Me.Page) Then
                lblStatusResponse.Text = Me.Language.Translate("Status.Inactive", Me.DefaultScope)
                Return "OK#OK"
            Else
                Return "ERROR#ERROR"
            End If
        Else
            Return "NO_AUTHORIZED"
        End If
    End Function

    Private Function RegenerateAllPwd() As String
        If WLHelperWeb.CurrentUserIsConsultant Then
            If API.EmployeeGroupsServiceMethods.RegenerateAllPasswords(Me.Page) Then
                Return "OK#OK"
            Else
                Return "ERROR#ERROR"
            End If
        Else
            Return "NO_AUTHORIZED"
        End If
    End Function

    Private Function DoFullBackup() As String
        Return "OK#OK"
    End Function

    Private Function DoSimpleBackup() As String
        Return "OK#OK"
    End Function

    Private Function HasAlternativeShifts() As Boolean
        Return CalendarServiceMethods.HasAlternativeShifts(Page)
    End Function

    Private Function DeleteAlternativeShifts() As String
        Dim result = IIf(CalendarServiceMethods.DeleteAlternativeShifts(Page), "OK#OK", "ERROR#ERROR")
        Dim hasAlternativeShiftsResult = HasAlternativeShifts()
        btnDeleteAlternativeShifts.Visible = hasAlternativeShiftsResult
        lblHasAlternativeShifts.Text = Language.Translate(IIf(hasAlternativeShiftsResult, "WithAlternativeShifts", "WithoutAlternativeShifts"), DefaultScope)
        Return result
    End Function

    Private Function HasSupervisorExceptions() As Boolean
        Return False
    End Function

    Private Function DeleteSupervisorExceptions() As String
        Dim result = "ERROR#ERROR"
        Dim hasExceptions As Boolean = HasSupervisorExceptions()
        btnEliminateExceptions.ClientEnabled = hasExceptions
        btnEliminateExceptions.Text = Language.Translate(IIf(hasExceptions, "WithSupervisorExceptions", "WithoutSupervisorExceptions"), DefaultScope)
        Return result
    End Function

#End Region

End Class