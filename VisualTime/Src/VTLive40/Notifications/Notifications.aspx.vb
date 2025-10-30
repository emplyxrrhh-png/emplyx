Imports DevExpress.Web.ASPxHtmlEditor
Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBusiness.Shift
Imports Robotics.Base.VTNotifications.Notifications
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class Notifications
    Inherits PageBase

    Private Const FeatureAlias As String = "Administration.Notifications.Definition"
    Private oPermission As Permission

    <Runtime.Serialization.DataContract()>
    Private Class ObjectCallbackRequest

        <Runtime.Serialization.DataMember(Name:="aTab")>
        Public aTab As Integer

        <Runtime.Serialization.DataMember(Name:="ID")>
        Public ID As Integer

        <Runtime.Serialization.DataMember(Name:="Name")>
        Public Name As String

        <Runtime.Serialization.DataMember(Name:="action")>
        Public Action As String

        <Runtime.Serialization.DataMember(Name:="clientControlsData")>
        Public clientControlsData As ClientControlsData

        <Runtime.Serialization.DataMember(Name:="StampParam")>
        Public StampParam As Double

    End Class

    <Runtime.Serialization.DataContract()>
    Private Class ClientControlsData

        <Runtime.Serialization.DataMember(Name:="selectedShifts")>
        Public selectedShifts As Integer()

        <Runtime.Serialization.DataMember(Name:="shifts")>
        Public shifts As SelectListItem()

        <Runtime.Serialization.DataMember(Name:="rigidShifts")>
        Public rigidShifts As SelectListItem()

        <Runtime.Serialization.DataMember(Name:="shiftsTimeLimit")>
        Public shiftsTimeLimit As ShiftTimeLimitGridItem()

        <Runtime.Serialization.DataMember(Name:="selectedShiftsTimeLimit")>
        Public selectedShiftsTimeLimit As ShiftTimeLimitGridItem()

        <Runtime.Serialization.DataMember(Name:="clientEnabled")>
        Public clientEnabled As Boolean

    End Class

    <Runtime.Serialization.DataContract()>
    Private Class SelectListItem

        <Runtime.Serialization.DataMember(Name:="ID")>
        Public ID As Integer

        <Runtime.Serialization.DataMember(Name:="Name")>
        Public Name As String

    End Class

    <Runtime.Serialization.DataContract()>
    Private Class ShiftTimeLimitGridItem

        <Runtime.Serialization.DataMember(Name:="ID")>
        Public ID As Integer

        <Runtime.Serialization.DataMember(Name:="TimeLimit")>
        Public TimeLimit As String

    End Class

    <Runtime.Serialization.DataContract()>
    Private Class LanguageCallbackRequest

        <Runtime.Serialization.DataMember(Name:="Scenarios")>
        Public Scenarios As roNotificationLanguage

        <Runtime.Serialization.DataMember(Name:="action")>
        Public Action As String

        <Runtime.Serialization.DataMember(Name:="StampParam")>
        Public StampParam As Double

    End Class

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("Ajax", "~/Base/Scripts/Ajax.js")
        Me.InsertExtraJavascript("jsDate", "~/Base/Scripts/jsDate.js")
        Me.InsertExtraJavascript("Notifications", "~/Notifications/Scripts/Notificationsv2.js")
        Me.InsertExtraJavascript("GenericData", "~/Base/Scripts/GenericData.js")
        Me.InsertExtraJavascript("roOptionPanelClient", "~/Base/Scripts/roOptionPanelClient.js")
        Me.InsertExtraJavascript("roOptPanelClientGroup", "~/Base/Scripts/roOptPanelClientGroup.js")
        Me.InsertExtraJavascript("jsGrid", "~/Base/Scripts/jsGrid.js")
        Me.InsertExtraJavascript("jsDatePicker", "~/Base/Scripts/jsDatePicker.js")
        Me.InsertExtraJavascript("webUserForms", "~/Base/Scripts/webUserForms.js")
        Me.InsertExtraJavascript("liveutils", "~/Base/Scripts/Live/utils.min.js")

    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim strAux As String = "~/Employee/EmployeeSelector?config=111&pagename&unionType=or&advancedMode=0&advancedFilter=0&allowAll=1&allowNone=0"
        Me.ifEmployeeSelector.Attributes("src") = Me.ResolveUrl(strAux)

        roTreesNotifications.TreeCaption = Me.Language.Translate("TreeCaptionNotifications", Me.DefaultScope)

        Me.lblSaveChanges.InnerText = Me.Language.Translate("msgSaveChangesbar", DefaultScope)
        Me.lblSaveChangesBottom.InnerText = Me.lblSaveChanges.InnerText
        Me.lblUndoChanges.InnerText = Me.Language.Translate("msgUndoChangesbar", DefaultScope)
        Me.lblUndoChangesBottom.InnerText = Me.lblUndoChanges.InnerText

        Me.msgHasChanges.Value = Me.Language.Translate("msgHasChanges", DefaultScope)
        Me.msgHasErrors.Value = Me.Language.Translate("msgHasErrors", DefaultScope)

        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)
        If Me.oPermission = Permission.None Then
            WLHelperWeb.RedirectAccessDenied(False)
            Exit Sub
        ElseIf Me.oPermission = Permission.Read Then
            DisableControls()
        End If

        If Me.oPermission < Permission.Write Then
            hdnModeEdit.Value = "true"
        Else
            hdnModeEdit.Value = "false"
        End If

        dateFormatValue.Value = HelperWeb.GetShortDateFormat

        Dim oRows As Generic.List(Of roNotification) = API.NotificationServiceMethods.GetNotifications(Me.Page)
        If oRows IsNot Nothing Then
            If oRows.Capacity = 0 Then
                Me.noRegs.Value = "1"
            Else
                Me.noRegs.Value = ""
            End If
        Else
            Me.noRegs.Value = "1"
        End If

        If Not IsPostBack AndAlso Not IsCallback Then
            dxNotificationLanguageEditor.Settings.AllowPreview = False
            dxNotificationLanguageEditor.Settings.AllowHtmlView = False
            dxNotificationHeaderEditor.Settings.AllowPreview = False
            dxNotificationHeaderEditor.Settings.AllowHtmlView = False
            dxNotificationLanguageEditor.SettingsHtmlEditing.EnablePasteOptions = False
            dxNotificationLanguageEditor.SettingsHtmlEditing.PasteMode = HtmlEditorPasteMode.PlainText
            dxNotificationHeaderEditor.SettingsHtmlEditing.EnablePasteOptions = False
            dxNotificationHeaderEditor.SettingsHtmlEditing.PasteMode = HtmlEditorPasteMode.PlainText
            dxNotificationHeaderEditor.Shortcuts.Clear()
            dxNotificationHeaderEditor.SettingsHtmlEditing.AllowStyleAttributes = False
            LoadNotificationsData()
        End If

    End Sub

    Private Sub LoadNotificationsData()
        ' Selecciona las diferentes notificaciones
        Dim Values As New List(Of Array)
        Me.cmbType.Items.Clear()
        Me.cmbNotificationType.Items.Clear()
        Me.cmbAvailableLanguages.Items.Clear()

        Dim dtNotificacions As DataTable = NotificationServiceMethods.GetNotificationsTypes(Me, False)

        For Each row As DataRow In dtNotificacions.Rows
            Values.Add({Language.Translate("NotifyType." & row(1), DefaultScope) & " (" & Language.Translate("CategoryType." & CInt(row(4)), "CategoryType") & ")", row(0).ToString.ToUpper})
        Next

        ' Las ordena
        SortValues(Values)

        Dim oLst = NotificationServiceMethods.LoadCustomizableNotifications(Me.Page)

        ' Las añade al combo
        For Each oNotificationType In oLst
            Me.cmbNotificationType.Items.Add(New DevExpress.Web.ListEditItem(Language.Translate("NotifyType." & oNotificationType.Name, DefaultScope), oNotificationType.Id))
        Next

        Dim collection = Me.cmbNotificationType.Items.Cast(Of DevExpress.Web.ListEditItem)().OrderBy(Function(item) item.Text).ToArray()
        Me.cmbNotificationType.Items.Clear()
        Me.cmbNotificationType.Items.AddRange(collection)

        ' Las añade al combo
        For i = 0 To Values.Count - 1
            Me.cmbType.Items.Add(New DevExpress.Web.ListEditItem(Values(i)(0), Values(i)(1)))
        Next

        Dim oLanguages() As roPassportLanguage = API.UserAdminServiceMethods.GetLanguages(Me)
        Dim bolInstalled As Boolean = False
        With Me.cmbAvailableLanguages
            .Items.Clear()
            .ValueType = GetType(String)
            For Each oLanguage As roPassportLanguage In oLanguages
                bolInstalled = IIf(oLanguage.Key = "ESP", True, False)
                If oLanguage.Installed Or bolInstalled Then
                    .Items.Add(New DevExpress.Web.ListEditItem(Language.Translate("Language." & oLanguage.Key, Me.DefaultScope), oLanguage.Key))
                End If
            Next
        End With

        Dim dTbl As DataTable = CausesServiceMethods.GetCauses(Me.Page)
        Me.cmbCausesNotification3.Items.Clear()
        Me.cmbCausesNotification4.Items.Clear()
        Me.cmbCausesNotification5.Items.Clear()
        Me.cmbCausesNotification8.Items.Clear()
        Me.cmbCausesNotification56.Items.Clear()
        Me.cmbCausesNotification56.Items.Add(New DevExpress.Web.ListEditItem("", 0))
        Me.cmbCausesNotification56.ValueType = GetType(Integer)
        Me.cmbCausesNotification65.Items.Clear()
        Me.cmbCausesNotification65.Items.Add(New DevExpress.Web.ListEditItem("", 0))
        Me.cmbCausesNotification65.ValueType = GetType(Integer)

        If dTbl IsNot Nothing Then
            For Each oRow As DataRow In dTbl.Rows
                'cmbCausesNotification3.AddItem(oRow("Name"), oRow("ID"), "")
                Me.cmbCausesNotification3.Items.Add(New DevExpress.Web.ListEditItem(oRow("Name"), oRow("ID").ToString.ToUpper))
                Me.cmbCausesNotification4.Items.Add(New DevExpress.Web.ListEditItem(oRow("Name"), oRow("ID").ToString.ToUpper))
                Me.cmbCausesNotification5.Items.Add(New DevExpress.Web.ListEditItem(oRow("Name"), oRow("ID").ToString.ToUpper))
                Me.cmbCausesNotification8.Items.Add(New DevExpress.Web.ListEditItem(oRow("Name"), oRow("ID").ToString.ToUpper))
                Me.cmbCausesNotification56.Items.Add(New DevExpress.Web.ListEditItem(oRow("Name"), oRow("ID").ToString.ToUpper))
                Me.cmbCausesNotification65.Items.Add(New DevExpress.Web.ListEditItem(oRow("Name"), oRow("ID").ToString.ToUpper))
            Next
        End If

        Dim tbConcepts As DataTable = API.ConceptsServiceMethods.GetConcepts(Me.Page)
        Me.cmbConcepts.Items.Clear()
        Me.cmbConcepts.Items.Add(New DevExpress.Web.ListEditItem("", ""))
        For Each dRow As DataRow In tbConcepts.Rows
            Me.cmbConcepts.Items.Add(New DevExpress.Web.ListEditItem(dRow("Name"), dRow("ID").ToString))
        Next

        Dim strEnums() As String = System.Enum.GetNames(GetType(ConditionCompareType))
        Me.cmbCompare.Items.Clear()
        For n As Integer = 0 To strEnums.Length - 1
            Me.cmbCompare.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate(strEnums(n), DefaultScope), n.ToString))
        Next

        Me.cmbTypeValue.Items.Clear()
        Me.cmbTypeValue.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ConditionType.DirectValue", DefaultScope), "DirectValue"))
        'cmbTypeValue.AddItem(Me.Language.Translate("ConditionType.DirectValue", DefaultScope), "DirectValue", "showTypeValue('DirectValue');hasChanges(true,false);")
        Me.cmbTypeValue.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ConditionType.UserField", DefaultScope), "UserField"))
        'cmbTypeValue.AddItem(Me.Language.Translate("ConditionType.UserField", DefaultScope), "UserField", "showTypeValue('UserField');hasChanges(true,false);")

        'Combo campos de la ficha
        Me.cmbConceptUserField.Items.Clear()
        Dim tbUserFields As DataTable = API.UserFieldServiceMethods.GetUserFields(Me.Page, Types.EmployeeField, "FieldType IN(1,3) AND Used=1", False)
        For Each oRow As DataRow In tbUserFields.Select("", "FieldName")
            Me.cmbConceptUserField.Items.Add(New DevExpress.Web.ListEditItem(oRow("FieldName"), oRow("FieldName")))
        Next

        ' Combo de tipos de comparación en notificaciones de incumplimiento de horas de ausencia y/o exceso
        Me.cmbBreachCompare.Items.Clear()
        Me.cmbBreachCompare.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("BreachControlType.Both", DefaultScope), 0))
        Me.cmbBreachCompare.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("BreachControlType.Minimum", DefaultScope), 1))
        Me.cmbBreachCompare.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("BreachControlType.Maximum", DefaultScope), 2))

        If Me.oPermission <> Permission.Admin Then
        End If
        LoadStaticInfo()
        LoadUserFields()
    End Sub

    Public Sub SortValues(ByRef Values As List(Of Array))
        Dim tmp As Array

        For i = 0 To Values.Count - 2
            For n = i To Values.Count - 1
                If UCase(Values(n)(0)).ToString.CompareTo(UCase(Values(i)(0))) < 0 Then
                    tmp = Values(i)
                    Values(i) = Values(n)
                    Values(n) = tmp
                End If
            Next
        Next
    End Sub

    Private Sub LoadUserFields()

        Dim dtblUserFieldsEmployee As DataTable = API.UserFieldServiceMethods.GetUserFields(Me, Types.EmployeeField, "Used <> 0", False)
        Dim dRowsEmployees() As DataRow = dtblUserFieldsEmployee.Select("", "FieldName")

        Dim dtblUserFieldsEnterprise As DataTable = API.UserFieldServiceMethods.GetUserFields(Me, Types.GroupField, "Used <> 0", False)
        Dim dRowsEnterprise() As DataRow = dtblUserFieldsEnterprise.Select("", "FieldName")

        Me.cmbUserFieldsNewDesign.Items.Clear()
        Me.cmbUserFieldsNewDesign.Items.Add(New DevExpress.Web.ListEditItem("", ""))

        For Each dRow As DataRow In dRowsEmployees
            If dRow("FieldType") = 0 Then
                Me.cmbUserFieldsNewDesign.Items.Add(New DevExpress.Web.ListEditItem(dRow("FieldName"), dRow("FieldName")))
            End If
        Next

        Me.cmbDatePeriodUserFieldEnter.Items.Clear()
        Me.cmbDatePeriodUserFieldEnter.Items.Add(New DevExpress.Web.ListEditItem("", ""))
        For Each dRow As DataRow In dRowsEnterprise
            If dRow("FieldType") = 6 Then 'Periodo de fechas
                'Me.cmbDatePeriodUserFieldEnter.AddItem(dRow("FieldName"), dRow("FieldName"), "hasChanges(true,false);")
                Me.cmbDatePeriodUserFieldEnter.Items.Add(New DevExpress.Web.ListEditItem(dRow("FieldName"), dRow("FieldName")))
            End If
        Next

        Me.cmbDatePeriodUserField.Items.Clear()
        Me.cmbDatePeriodUserField.Items.Add(New DevExpress.Web.ListEditItem("", ""))
        For Each dRow As DataRow In dRowsEmployees
            If dRow("FieldType") = 6 Then 'Periodo de fechas
                Me.cmbDatePeriodUserField.Items.Add(New DevExpress.Web.ListEditItem(dRow("FieldName"), dRow("FieldName")))
            End If
        Next

    End Sub

    Private Sub LoadStaticInfo()

        tknConditionRole.Items.Clear()
        For Each Roles In API.SecurityChartServiceMethods.GetGroupFeatures(Nothing)
            tknConditionRole.Items.Add(Roles.Name, Roles.ID)
        Next

    End Sub

    Protected Sub ASPxCallbackPanelContenido_Callback(sender As Object, e As DevExpress.Web.CallbackEventArgsBase) Handles ASPxCallbackPanelContenido.Callback
        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Dim oParameters As New ObjectCallbackRequest()
        oParameters = roJSONHelper.Deserialize(strParameter, oParameters.GetType())

        Dim responseMessage = String.Empty
        Dim bRet As Boolean = False
        Dim oClientControls As New ClientControlsData
        Dim tbShifts As DataTable = API.ShiftServiceMethods.GetShifts(Me.Page,,,, True, True)
        oClientControls.shifts = CreateShiftsTagBoxJSON(tbShifts)

        oClientControls.rigidShifts = CreateRigidShiftsTagBoxJSON(tbShifts)
        oClientControls.shiftsTimeLimit = CreateShiftTimeLimitGrid(tbShifts)
        If oPermission > Permission.Read Then
            oClientControls.clientEnabled = True
        Else
            oClientControls.clientEnabled = False
        End If

        Select Case oParameters.Action

            Case "GETNOTIFICATION"

                bRet = LoadNotification(oParameters.ID, responseMessage, oClientControls)
                If Not bRet Then
                    ASPxCallbackPanelContenido.JSProperties.Add("cpMessage", responseMessage)
                    ASPxCallbackPanelContenido.JSProperties.Add("cpResult", "NOK")
                Else
                    ASPxCallbackPanelContenido.JSProperties.Add("cpResult", "OK")
                End If

                ASPxCallbackPanelContenido.JSProperties.Add("cpClientControlsRO", roJSONHelper.Serialize(oClientControls)) 'roJSONHelper.Serialize(CreateShiftsTagBoxJSON()))

                ASPxCallbackPanelContenido.JSProperties.Add("cpIsNew", False)
                ASPxCallbackPanelContenido.JSProperties.Add("cpAction", "GETNOTIFICATION")
                ASPxCallbackPanelContenido.JSProperties.Add("cpNameRO", txtName.Text)

            Case "SAVENOTIFICATION"
                ASPxCallbackPanelContenido.JSProperties.Add("cpAction", "SAVENOTIFICATION")
                ASPxCallbackPanelContenido.JSProperties.Add("cpNameRO", txtName.Text)
                Dim oCurrentNotification As roNotification = Nothing
                bRet = SaveNotificationData(oParameters, responseMessage, oCurrentNotification)

                If Not bRet Then
                    ASPxCallbackPanelContenido.JSProperties.Add("cpResult", "NOK")
                    ASPxCallbackPanelContenido.JSProperties.Add("cpMessage", responseMessage)
                    bRet = LoadNotification("-1", responseMessage, oClientControls, oCurrentNotification)
                Else
                    bRet = LoadNotification(oCurrentNotification.ID, responseMessage, oClientControls)

                    If Not bRet Then
                        ASPxCallbackPanelContenido.JSProperties.Add("cpResult", "NOK")
                        ASPxCallbackPanelContenido.JSProperties.Add("cpMessage", responseMessage)
                    Else
                        ASPxCallbackPanelContenido.JSProperties("cpAction") = "GETNOTIFICATION"
                        ASPxCallbackPanelContenido.JSProperties.Add("cpResult", "OK")
                        ASPxCallbackPanelContenido.JSProperties.Add("cpIsNew", True)
                    End If
                End If

                ASPxCallbackPanelContenido.JSProperties.Add("cpClientControlsRO", roJSONHelper.Serialize(oClientControls))

            Case "GETNOTIFICATIONSCENARIOS"
                InitScenarios(oParameters)
        End Select

        ProcessSelectedTabVisible(oParameters)
    End Sub

    Private Sub ProcessSelectedTabVisible(ByVal oParameters As ObjectCallbackRequest)
        'Mostra el TAB seleccionat
        Me.div00.Style("display") = "none"
        Me.div01.Style("display") = "none"
        Select Case oParameters.aTab
            Case 0
                Me.div00.Style("display") = ""
            Case 1
                Me.div01.Style("display") = ""
        End Select

    End Sub

    Protected Sub SaveLanguageCallback_Callback(sender As Object, e As DevExpress.Web.CallbackEventArgsBase) Handles SaveLanguageCallback.Callback
        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Dim oParameters As New LanguageCallbackRequest()
        oParameters = roJSONHelper.DeserializeNewtonSoft(strParameter, oParameters.GetType())

        Dim responseMessage = String.Empty
        Dim bRet As Boolean = False
        Dim tbShifts As DataTable = API.ShiftServiceMethods.GetShifts(Me.Page,,,, True, True)
        Dim oClientControls As New ClientControlsData
        oClientControls.shifts = CreateShiftsTagBoxJSON(tbShifts)
        oClientControls.rigidShifts = CreateRigidShiftsTagBoxJSON(tbShifts)
        oClientControls.shiftsTimeLimit = CreateShiftTimeLimitGrid(tbShifts)
        If oPermission > Permission.Read Then
            oClientControls.clientEnabled = True
        Else
            oClientControls.clientEnabled = False
        End If

        Select Case oParameters.Action
            Case "SAVELANGUAGETEXT"
                SaveLanguageText(oParameters)
        End Select

    End Sub

#Region "Language definition"

    Private Function InitScenarios(ByVal oParameters As ObjectCallbackRequest) As Boolean
        Dim bRet As Boolean = False
        Try
            If Not ASPxCallbackPanelContenido.JSProperties.ContainsKey("cpResult") Then ASPxCallbackPanelContenido.JSProperties.Add("cpResult", "")
            If Not ASPxCallbackPanelContenido.JSProperties.ContainsKey("cpObject") Then ASPxCallbackPanelContenido.JSProperties.Add("cpObject", "")
            If Not ASPxCallbackPanelContenido.JSProperties.ContainsKey("cpAction") Then ASPxCallbackPanelContenido.JSProperties.Add("cpAction", "")

            If Me.cmbAvailableLanguages.SelectedItem Is Nothing Then Me.cmbAvailableLanguages.SelectedItem = Me.cmbAvailableLanguages.Items(0)
            If Me.cmbNotificationType.SelectedItem Is Nothing Then Me.cmbNotificationType.SelectedItem = Me.cmbNotificationType.Items(0)

            Dim availableScenarios As roNotificationLanguage = NotificationServiceMethods.LoadNotificationLanguage(Me.Page, roTypes.Any2Integer(Me.cmbNotificationType.SelectedItem.Value), Me.cmbAvailableLanguages.SelectedItem.Value)

            Me.cmbAvailableScenarios.Items.Clear()
            Me.cmbAvailableScenarios.ValueType = GetType(Integer)
            For Each oScenario In availableScenarios.Scenarios
                Me.cmbAvailableScenarios.Items.Add(If(availableScenarios.Scenarios.Length = 1, "", oScenario.Name), oScenario.IDScenario)
            Next

            Dim curScenario As NotificationLanguageScenario = Nothing
            If availableScenarios.Scenarios.Length > 0 Then
                If oParameters.ID = -1 Then
                    Me.cmbAvailableScenarios.SelectedItem = Me.cmbAvailableScenarios.Items(0)
                    curScenario = availableScenarios.Scenarios(0)
                Else
                    Dim bFound As Boolean = False
                    For Each tmpScenario In availableScenarios.Scenarios
                        If tmpScenario.IDScenario = oParameters.ID Then
                            Me.cmbAvailableScenarios.Value = tmpScenario.IDScenario
                            curScenario = tmpScenario
                            bFound = True
                        End If
                    Next

                    If Not bFound Then
                        Me.cmbAvailableScenarios.SelectedItem = Me.cmbAvailableScenarios.Items(0)
                        curScenario = availableScenarios.Scenarios(0)
                    End If

                End If
            End If

            If curScenario IsNot Nothing Then
                Me.dxNotificationHeaderEditor.Html = curScenario.Subject
                Me.dxNotificationHeaderEditor.Placeholders.Clear()
                For Each oTag In curScenario.SubjectParameters
                    If oTag.ParameterLanguageKey <> String.Empty Then
                        Me.dxNotificationHeaderEditor.Placeholders.Add(oTag.Name)
                    End If
                Next

                Me.dxNotificationLanguageEditor.Html = curScenario.Body
                Me.dxNotificationLanguageEditor.Placeholders.Clear()
                For Each oTag In curScenario.BodyParameters
                    If oTag.ParameterLanguageKey <> String.Empty Then
                        Me.dxNotificationLanguageEditor.Placeholders.Add(oTag.Name)
                    End If
                Next
            Else

                Me.dxNotificationLanguageEditor.Html = String.Empty
                Me.dxNotificationLanguageEditor.Placeholders.Clear()
                Me.dxNotificationHeaderEditor.Html = String.Empty
                Me.dxNotificationHeaderEditor.Placeholders.Clear()
            End If

            ASPxCallbackPanelContenido.JSProperties("cpResult") = "OK"
            ASPxCallbackPanelContenido.JSProperties("cpObject") = roJSONHelper.SerializeNewtonSoft(availableScenarios)
            ASPxCallbackPanelContenido.JSProperties("cpAction") = "GETNOTIFICATIONSCENARIOS"
            bRet = True
        Catch ex As Exception
            ASPxCallbackPanelContenido.JSProperties("cpResult") = "NOK"
            ASPxCallbackPanelContenido.JSProperties("cpObject") = ""
            ASPxCallbackPanelContenido.JSProperties("cpAction") = "GETNOTIFICATIONSCENARIOS"

            Dim wsState As New Robotics.Base.DTOs.roWsState
            wsState.ReturnCode = "5-BA01-001"
            wsState.ErrorText = Me.Language.Translate("ErrorCodeMesage." & wsState.ReturnCode, Me.DefaultScope)
            HelperWeb.ShowError(Me.Page, wsState)
        End Try

        Return bRet
    End Function

    Private Function SaveLanguageText(ByVal oParameters As LanguageCallbackRequest) As Boolean
        Dim bRet As Boolean = False
        Try
            If Not SaveLanguageCallback.JSProperties.ContainsKey("cpResult") Then SaveLanguageCallback.JSProperties.Add("cpResult", "")
            If Not SaveLanguageCallback.JSProperties.ContainsKey("cpAction") Then SaveLanguageCallback.JSProperties.Add("cpAction", "")

            SaveLanguageCallback.JSProperties("cpAction") = "SAVELANGUAGETEXT"
            bRet = NotificationServiceMethods.SaveNotificationLanguage(Me.Page, oParameters.Scenarios)

            If bRet Then
                SaveLanguageCallback.JSProperties("cpResult") = "OK"
            Else
                Dim wsState As New Robotics.Base.DTOs.roWsState
                wsState.ReturnCode = "5-BA01-002"
                wsState.ErrorText = Me.Language.Translate("ErrorCodeMesage." & wsState.ReturnCode, Me.DefaultScope)
                HelperWeb.ShowError(Me.Page, wsState)
                SaveLanguageCallback.JSProperties("cpResult") = "NOK"
            End If
        Catch ex As Exception
            SaveLanguageCallback.JSProperties("cpResult") = "NOK"
            SaveLanguageCallback.JSProperties("cpAction") = "SAVELANGUAGETEXT"

            Dim wsState As New Robotics.Base.DTOs.roWsState
            wsState.ReturnCode = "5-BA01-001"
            wsState.ErrorText = Me.Language.Translate("ErrorCodeMesage." & wsState.ReturnCode, Me.DefaultScope)
            HelperWeb.ShowError(Me.Page, wsState)
        End Try

        Return bRet
    End Function

#End Region

#Region "Notification section"

    Private Function CreateShiftsTagBoxJSON(ByVal tbShifts As DataTable) As SelectListItem()
        Dim oLst As SelectListItem() = {}
        Try
            Dim lstItems As New Generic.List(Of SelectListItem)
            For Each oRow As DataRow In tbShifts.Select("", "Name")
                If roTypes.Any2Double(oRow("ExpectedWorkingHours")) > 0 Then
                    Dim oTemp As New SelectListItem
                    oTemp.ID = oRow("Id")
                    oTemp.Name = oRow("Name")
                    lstItems.Add(oTemp)
                End If
            Next

            oLst = lstItems.ToArray()
        Catch ex As Exception
            Dim rError As New roJSON.JSONError(True, ex.Message.ToString & ex.StackTrace.ToString)
        End Try
        Return oLst
    End Function

    Private Function CreateShiftTimeLimitGrid(ByVal tbShifts As DataTable) As ShiftTimeLimitGridItem()
        Dim oLst As ShiftTimeLimitGridItem() = {}
        Try
            Dim lstItems As New Generic.List(Of ShiftTimeLimitGridItem)
            If tbShifts IsNot Nothing Then
                For Each oRow As DataRow In tbShifts.Select("", "Name")
                    If roTypes.Any2Integer(oRow("NotifyEmployeeExitAt")) <> 0 Then
                        Dim oTemp As New ShiftTimeLimitGridItem
                        oTemp.ID = oRow("Id")
                        Dim hours As Integer = Math.Floor(roTypes.Any2Integer(oRow("NotifyEmployeeExitAt")) / 60)
                        Dim minutes As Integer = (roTypes.Any2Integer(oRow("NotifyEmployeeExitAt")) / 60 - hours) * 60
                        oTemp.TimeLimit = New DateTime(1, 1, 1, hours, minutes, 0).ToString("HH:mm")
                        lstItems.Add(oTemp)
                    End If
                Next
                oLst = lstItems.ToArray()
            End If
        Catch ex As Exception
            Dim rError As New roJSON.JSONError(True, ex.Message.ToString & ex.StackTrace.ToString)
        End Try
        Return oLst
    End Function

    Private Function CreateRigidShiftsTagBoxJSON(ByVal tbShifts As DataTable) As SelectListItem()
        Dim oLst As SelectListItem() = {}
        Try
            Dim lstItems As New Generic.List(Of SelectListItem)
            If tbShifts IsNot Nothing Then
                Dim filteredRows As DataRow() = tbShifts.Select("IsRigid = 1")
                For Each oRow As DataRow In filteredRows
                    Dim oTemp As New SelectListItem
                    oTemp.ID = oRow("Id")
                    oTemp.Name = oRow("Name")
                    lstItems.Add(oTemp)
                Next
                oLst = lstItems.ToArray()
            End If
        Catch ex As Exception
            Dim rError As New roJSON.JSONError(True, ex.Message.ToString & ex.StackTrace.ToString)
        End Try
        Return oLst
    End Function


    Private Function SaveNotificationData(ByRef oParameters As ObjectCallbackRequest, ByRef ErrorInfo As String, ByRef oCurrentNotification As roNotification) As Boolean
        Dim bolret As Boolean = False
        Try

            'Check Permissions
            If Me.oPermission < Permission.Write Then
                ErrorInfo = Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope)
                Return False
            End If

            Dim bolIsNew As Boolean = False
            If oParameters.ID = 0 Or oParameters.ID = -1 Then bolIsNew = True

            If Me.oPermission = Permission.Write And bolIsNew Then
                ErrorInfo = Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope)
                Return False
            End If

            If bolIsNew Then
                oCurrentNotification = New roNotification
                oCurrentNotification.ID = -1
            Else
                oCurrentNotification = API.NotificationServiceMethods.GetNotificationByID(Me, oParameters.ID, False)
            End If

            If oCurrentNotification Is Nothing Then Return False

            Dim oNotificationCondition As New roNotificationCondition()
            Dim oDestination As New roNotificationDestination()
            oDestination.Destination = String.Empty

            oNotificationCondition.DaysBefore = 0
            oNotificationCondition.IDCause = 0
            oNotificationCondition.IDConcept = 0

            oNotificationCondition.IDShifts = {}
            oNotificationCondition.MailListUserfield = ""
            oNotificationCondition.DatePeriodUserfield = ""
            oNotificationCondition.TypeToIgnore = ""
            oNotificationCondition.ConditionRole = {}
            oNotificationCondition.TargetTypeConcept = ""
            oNotificationCondition.TargetConcept = ""

            Dim bolMailList As Boolean = False
            Dim strDestination As String = String.Empty
            Dim bolEmployeeField As Boolean = False
            Dim strMailListUserfield As String = String.Empty

            If Me.cmbType.SelectedItem Is Nothing Then
                ErrorInfo = Me.Language.Translate("ValidateNotification.InvalidData.Description", DefaultScope)
                Return False
            End If

            oCurrentNotification.IDType = roTypes.Any2Integer(Me.cmbType.SelectedItem.Value)
            oCurrentNotification.Name = txtName.Text
            oCurrentNotification.Activated = Me.chkActivated.Checked
            oCurrentNotification.Schedule = ""

            If oCurrentNotification.IDType = 1 Then oNotificationCondition.DaysBefore = roTypes.Any2Integer(Me.txtDaysNotification1.Value)
            If oCurrentNotification.IDType = 2 Then oNotificationCondition.DaysBefore = roTypes.Any2Integer(Me.txtDaysNotification2.Value)
            If oCurrentNotification.IDType = 3 Then oNotificationCondition.DaysBefore = roTypes.Any2Integer(Me.txtDaysNotification3.Value)
            If oCurrentNotification.IDType = 5 Then oNotificationCondition.DaysBefore = roTypes.Any2Integer(Me.txtDaysNotification5.Value)
            If oCurrentNotification.IDType = 6 Then oNotificationCondition.DaysBefore = roTypes.Any2Integer(Me.txtDaysNotification6.Value)
            If oCurrentNotification.IDType = 16 Then oNotificationCondition.DaysBefore = roTypes.Any2Integer(Me.txtDaysNotification16.Value)
            If oCurrentNotification.IDType = 17 Then oNotificationCondition.DaysBefore = roTypes.Any2Integer(Me.txtDaysNotification17.Value)

            If oCurrentNotification.IDType = 3 Then
                If Me.cmbCausesNotification3.SelectedItem Is Nothing Then
                    ErrorInfo = Me.Language.Translate("ValidateNotification.InvalidData.Description", DefaultScope)
                    Return False
                End If
                oNotificationCondition.IDCause = roTypes.Any2Integer(Me.cmbCausesNotification3.SelectedItem.Value)
            End If

            If oCurrentNotification.IDType = 4 Then
                If Me.cmbCausesNotification4.SelectedItem Is Nothing Then
                    ErrorInfo = Me.Language.Translate("ValidateNotification.InvalidData.Description", DefaultScope)
                    Return False
                End If

                oNotificationCondition.IDCause = roTypes.Any2Integer(Me.cmbCausesNotification4.SelectedItem.Value)
            End If

            If oCurrentNotification.IDType = 5 Then
                If Me.cmbCausesNotification5.SelectedItem Is Nothing Then
                    ErrorInfo = Me.Language.Translate("ValidateNotification.InvalidData.Description", DefaultScope)
                    Return False
                End If

                oNotificationCondition.IDCause = roTypes.Any2Integer(Me.cmbCausesNotification5.SelectedItem.Value)
            End If

            If oCurrentNotification.IDType = 8 Then
                If Me.cmbCausesNotification8.SelectedItem Is Nothing Then
                    ErrorInfo = Me.Language.Translate("ValidateNotification.InvalidData.Description", DefaultScope)
                    Return False
                End If

                oNotificationCondition.IDCause = roTypes.Any2Integer(Me.cmbCausesNotification8.SelectedItem.Value)
            End If

            If oCurrentNotification.IDType = 18 Then

                If Me.cmbConcepts.SelectedItem Is Nothing OrElse Me.cmbCompare.SelectedItem Is Nothing OrElse Me.cmbTypeValue.SelectedItem Is Nothing Then
                    ErrorInfo = Me.Language.Translate("ValidateNotification.InvalidData.Description", DefaultScope)
                    Return False
                End If

                If Me.cmbConcepts.SelectedItem.Value = "" OrElse Me.cmbCompare.SelectedItem.Value = "" OrElse Me.cmbTypeValue.SelectedItem.Value = "" Then
                    ErrorInfo = Me.Language.Translate("ValidateNotification.InvalidData.Description", DefaultScope)
                    Return False
                End If

                oNotificationCondition.IDConcept = roTypes.Any2Integer(Me.cmbConcepts.SelectedItem.Value)

                oNotificationCondition.tCompareConceptType = roTypes.Any2Integer(Me.cmbCompare.SelectedItem.Value)

                oNotificationCondition.TargetTypeConcept = roTypes.Any2String(Me.cmbTypeValue.SelectedItem.Value)

                If oNotificationCondition.TargetTypeConcept = "DirectValue" Then
                    oNotificationCondition.TargetConcept = roTypes.Any2Time(txtValueType.Text).TimeOnly
                End If

                If oNotificationCondition.TargetTypeConcept = "UserField" Then
                    If Me.cmbConceptUserField.SelectedItem Is Nothing Then
                        ErrorInfo = Me.Language.Translate("ValidateNotification.InvalidData.Description", DefaultScope)
                        Return False
                    End If

                    oNotificationCondition.TargetConcept = roTypes.Any2String(Me.cmbConceptUserField.SelectedItem.Value)
                End If

            End If

            If oCurrentNotification.IDType = 16 Or oCurrentNotification.IDType = 17 Then
                If Me.cmbDatePeriodUserField.SelectedItem Is Nothing Then
                    ErrorInfo = Me.Language.Translate("ValidateNotification.InvalidData.Description", DefaultScope)
                    Return False
                End If
                oNotificationCondition.DatePeriodUserfield = roTypes.Any2String(Me.cmbDatePeriodUserField.SelectedItem.Value)
            End If

            If oCurrentNotification.IDType = 17 Then
                If Me.cmbDatePeriodUserFieldEnter.SelectedItem Is Nothing Then
                    ErrorInfo = Me.Language.Translate("ValidateNotification.InvalidData.Description", DefaultScope)
                    Return False
                End If
                oNotificationCondition.DatePeriodUserfield = roTypes.Any2String(Me.cmbDatePeriodUserFieldEnter.SelectedItem.Value)
            End If

            If oCurrentNotification.IDType = 50 OrElse oCurrentNotification.IDType = 70 Then
                If Me.seNotificationPeriod.Value <> 0 Then
                    oNotificationCondition.NotificationPeriod = roTypes.Any2Integer(Me.seNotificationPeriod.Value)
                End If
                If Me.seNotificationRepeat.Value <> 0 Then
                    oNotificationCondition.NotificationRepeat = roTypes.Any2Integer(Me.seNotificationRepeat.Value)
                End If

            End If

            Dim oControl As Object = Me.optSupervisorByPortal.FindControl("chkButton")

            oCurrentNotification.AllowPortal = oControl.Checked

            oControl = Me.optEmployeeByPortal.FindControl("chkButton")
            oCurrentNotification.AllowVTPortal = oControl.Checked

            oControl = Me.optSupervisorByMail.FindControl("chkButton")

            oCurrentNotification.AllowMail = oControl.Checked
            strDestination = Me.txtMailList.Text

            oControl = Me.optMailList.FindControl("chkButton")

            If oControl.Checked Then
                bolMailList = True
            End If

            oNotificationCondition.EmployeeFilter = Me.EmployeeFilter.Value

            oControl = Me.optEmployeeField.FindControl("chkButton")
            If oControl.Checked Then
                bolEmployeeField = True
                If Me.cmbUserFieldsNewDesign.SelectedItem Is Nothing Then
                    ErrorInfo = Me.Language.Translate("ValidateNotification.InvalidData.Description", DefaultScope)
                    Return False
                End If

                strMailListUserfield = roTypes.Any2String(cmbUserFieldsNewDesign.SelectedItem.Value)
            End If

            If Not bolMailList Then
                strDestination = String.Empty
            End If

            If Not bolEmployeeField Then
                strMailListUserfield = String.Empty
            End If

            oDestination.Destination = strDestination
            oNotificationCondition.MailListUserfield = strMailListUserfield

            If oCurrentNotification.IDType = 51 Then
                oNotificationCondition.IDShifts = oParameters.clientControlsData.selectedShifts
                oNotificationCondition.OnlyFromCalendar = ckOnlyCalendar.Checked
            End If

            If oCurrentNotification.IDType = 83 Then
                Dim isvalid As Boolean = True
                If Me.timeLimitNotification8384.Value Is Nothing OrElse Me.timeLimitNotification8384.Value = DateTime.MinValue Then
                    isvalid = False
                    ErrorInfo = Me.Language.Translate("ValidateNotification.InvalidData.Description", DefaultScope)
                Else
                    oNotificationCondition.PunchBeforeStartTimeLimit = roTypes.Any2DateTime(Me.timeLimitNotification8384.Value)
                End If
                If oParameters.clientControlsData.selectedShifts.Length = 0 Then
                    ErrorInfo = Me.Language.Translate("ValidateNotification.InvalidData.Description", DefaultScope)
                    isvalid = False
                Else
                    oNotificationCondition.IDShifts = oParameters.clientControlsData.selectedShifts
                End If
                If Not isvalid Then Return False
            End If
            If oCurrentNotification.IDType = 84 Then
                Dim isvalid As Boolean = True
                If Me.timeLimitNotification8384.Value Is Nothing OrElse roTypes.Any2DateTime(Me.timeLimitNotification8384.Value) <= DateTime.MinValue Then
                    oNotificationCondition.PunchToleranceTime = DateTime.MinValue
                Else
                    oNotificationCondition.PunchToleranceTime = roTypes.Any2DateTime(Me.timeLimitNotification8384.Value)
                End If
                If oParameters.clientControlsData.selectedShifts.Length = 0 Then
                    ErrorInfo = Me.Language.Translate("ValidateNotification.InvalidData.Description", DefaultScope)
                    isvalid = False
                Else
                    oNotificationCondition.IDShifts = oParameters.clientControlsData.selectedShifts
                End If
                If Not isvalid Then Return False
            End If
            If oCurrentNotification.IDType = 72 Then
                Dim isvalid As Boolean = True
                If oParameters.clientControlsData.shiftsTimeLimit.Length = 0 Then
                    ErrorInfo = Me.Language.Translate("ValidateNotification.InvalidData.Description", DefaultScope)
                    isvalid = False
                Else
                    Dim existingNotifications As List(Of roNotification) = API.NotificationServiceMethods.GetNotifications(Me)
                    Dim notificationIDType As Integer = oCurrentNotification.IDType
                    Dim notificationID As Integer = oCurrentNotification.ID
                    If existingNotifications.Any(Function(n) n.IDType = notificationIDType And n.ID <> notificationID) Then
                        ErrorInfo = Me.Language.Translate("ValidateNotification.DuplicateNotification.Description", DefaultScope)
                        isvalid = False
                    Else
                        ' Horarios a los que se les ha eliminado la notificación
                        Dim shiftsTimeLimit As ShiftTimeLimitGridItem() = oParameters.clientControlsData.shiftsTimeLimit
                        Dim selectedShiftsTimeLimit As ShiftTimeLimitGridItem() = oParameters.clientControlsData.selectedShiftsTimeLimit

                        If selectedShiftsTimeLimit IsNot Nothing Then

                            Dim shiftsNotInSelected As IEnumerable(Of ShiftTimeLimitGridItem) = shiftsTimeLimit.Where(Function(stl) Not selectedShiftsTimeLimit.Any(Function(sstl) sstl.ID = stl.ID)).ToArray()

                            For Each element In shiftsNotInSelected
                                Dim shift As roShift = API.ShiftServiceMethods.GetShift(Me, element.ID, False)
                                shift.EnableNotifyExit = False
                                shift.Save()
                            Next

                            For Each element In selectedShiftsTimeLimit
                                Dim shift As roShift = API.ShiftServiceMethods.GetShift(Me, element.ID, False)
                                shift.EnableNotifyExit = True
                                If roTypes.Any2Integer(shift.NotifyEmployeeExitAt) <> roTypes.Any2Integer(element.TimeLimit.Split(":")(0) * 60 + element.TimeLimit.Split(":")(1)) Then
                                    shift.NotifyEmployeeExitAt = element.TimeLimit.Split(":")(0) * 60 + element.TimeLimit.Split(":")(1)
                                    shift.Save()
                                End If
                            Next
                        End If

                    End If
                    End If
                If Not isvalid Then Return False
            End If

            If oCurrentNotification.IDType = 56 Then
                If Me.cmbCausesNotification56.SelectedItem Is Nothing Then
                    ErrorInfo = Me.Language.Translate("ValidateNotification.InvalidData.Description", DefaultScope)
                    Return False
                End If

                If tknConditionRole.Tokens.Count > 0 Then
                    oNotificationCondition.ConditionRole = (From s As String In roTypes.Any2String(tknConditionRole.Value).Split(","c)).ToArray
                End If

                If Not optConditionRole.Checked Then
                    oNotificationCondition.ConditionRole = Nothing
                End If

                If optTypeToIgnore.Checked Then
                    oNotificationCondition.TypeToIgnore = "5,6"
                Else
                    oNotificationCondition.TypeToIgnore = ""
                End If
                oNotificationCondition.IDCause = roTypes.Any2Integer(Me.cmbCausesNotification56.SelectedItem.Value)
            End If

            If oCurrentNotification.IDType = 65 Then
                If Me.cmbCausesNotification65.SelectedItem Is Nothing Then
                    ErrorInfo = Me.Language.Translate("ValidateNotification.InvalidData.Description", DefaultScope)
                    Return False
                End If

                If tknConditionRole.Tokens.Count > 0 Then
                    oNotificationCondition.ConditionRole = (From s As String In roTypes.Any2String(tknConditionRole.Value).Split(","c)).ToArray
                End If

                If Not optConditionRole.Checked Then
                    oNotificationCondition.ConditionRole = Nothing
                End If

                oNotificationCondition.IDCause = roTypes.Any2Integer(Me.cmbCausesNotification65.SelectedItem.Value)
            End If

            oCurrentNotification.Condition = oNotificationCondition
            oCurrentNotification.Destination = oDestination

            If oCurrentNotification.IDType = 18 Then
                If Not String.IsNullOrEmpty(oCurrentNotification.Condition.TargetTypeConcept) AndAlso Not String.IsNullOrEmpty(oCurrentNotification.Condition.TargetConcept) Then
                    oCurrentNotification.Condition.TargetConcept = oCurrentNotification.Condition.TargetConcept
                End If
            End If

            If oCurrentNotification.IDType = 77 OrElse oCurrentNotification.IDType = 78 Then
                If Me.cmbBreachCompare.SelectedItem Is Nothing Then
                    ErrorInfo = Me.Language.Translate("ValidateNotification.InvalidData.Description", DefaultScope)
                    Return False
                End If
                If txtBreachCourtesyMinutes.Text = "" Then
                    ErrorInfo = Me.Language.Translate("ValidateNotification.InvalidData.Description", DefaultScope)
                    Return False
                End If
                oNotificationCondition.BreachCompareType = roTypes.Any2Integer(Me.cmbBreachCompare.SelectedItem.Value)
                oNotificationCondition.BreachCourtesy = roTypes.Any2Integer(Me.txtBreachCourtesyMinutes.Text)
            End If

            If API.NotificationServiceMethods.SaveNotification(Me, oCurrentNotification, True) = True Then
                Dim treePath As String = "/source/" & oCurrentNotification.ID
                HelperWeb.roSelector_SetSelection(oCurrentNotification.ID.ToString, treePath, "ctl00_contentMainBody_roTreesNotifications")
                bolret = True
            Else
                ErrorInfo = API.NotificationServiceMethods.LastErrorText
                bolret = False

            End If
        Catch ex As Exception
            ErrorInfo = ex.Message.ToString & " " & ex.StackTrace
            bolret = False
        End Try

        Return bolret
    End Function

    Private Function LoadNotification(ByVal oID As String, ByRef ErrorInfo As String, ByRef oClientControls As ClientControlsData, Optional ByVal CNotification As roNotification = Nothing) As Boolean
        Dim bRet As Boolean = False
        Dim isNew As Boolean = False
        Try

            Dim oCurrentNotification As roNotification

            If CNotification Is Nothing Then
                If oID = "-1" Then
                    isNew = True
                    oCurrentNotification = New roNotification
                Else
                    oCurrentNotification = API.NotificationServiceMethods.GetNotificationByID(Me, CInt(oID), True)
                End If
            Else
                oCurrentNotification = CNotification
            End If

            If oCurrentNotification Is Nothing Then Return False

            Dim oDisable As Boolean = True
            If oPermission > Permission.Read Then
                oDisable = False
            End If

            '============================= GENERAL ==========================

            Me.txtName.Text = oCurrentNotification.Name

            Me.cmbType.SelectedItem = Me.cmbType.Items.FindByValue(oCurrentNotification.IDType.ToString)

            Me.chkActivated.Checked = oCurrentNotification.Activated

            '================================================================

            '============================= CUANDO ============================
            Dim intDias As Integer = 0
            If (oCurrentNotification.IDType = 1 OrElse oCurrentNotification.IDType = 2 OrElse oCurrentNotification.IDType = 3 OrElse oCurrentNotification.IDType = 5 OrElse oCurrentNotification.IDType = 6 OrElse
               oCurrentNotification.IDType = 16 OrElse oCurrentNotification.IDType = 17) AndAlso oCurrentNotification.Condition IsNot Nothing AndAlso oCurrentNotification.Condition.DaysBefore.HasValue Then
                intDias = oCurrentNotification.Condition.DaysBefore
            End If
            txtDaysNotification1.Value = IIf(oCurrentNotification.IDType = 1, intDias, 0)

            txtDaysNotification2.Value = IIf(oCurrentNotification.IDType = 2, intDias, 0)

            txtDaysNotification3.Value = IIf(oCurrentNotification.IDType = 3, intDias, 0)
            txtDaysNotification5.Value = IIf(oCurrentNotification.IDType = 5, intDias, 0)
            txtDaysNotification6.Value = IIf(oCurrentNotification.IDType = 6, intDias, 0)
            txtDaysNotification16.Value = IIf(oCurrentNotification.IDType = 16, intDias, 0)
            txtDaysNotification17.Value = IIf(oCurrentNotification.IDType = 17, intDias, 0)

            Dim intIdCause As Integer = 0
            If (oCurrentNotification.IDType = 3 OrElse oCurrentNotification.IDType = 4 OrElse oCurrentNotification.IDType = 5 OrElse oCurrentNotification.IDType = 5 OrElse oCurrentNotification.IDType = 8) AndAlso
                oCurrentNotification.Condition IsNot Nothing AndAlso oCurrentNotification.Condition.IDCause.HasValue Then
                intIdCause = oCurrentNotification.Condition.IDCause
            End If
            Me.cmbCausesNotification3.SelectedItem = Me.cmbCausesNotification3.Items.FindByValue(IIf(oCurrentNotification.IDType = 3, intIdCause.ToString, 0))
            Me.cmbCausesNotification4.SelectedItem = Me.cmbCausesNotification4.Items.FindByValue(IIf(oCurrentNotification.IDType = 4, intIdCause.ToString, 0))
            Me.cmbCausesNotification5.SelectedItem = Me.cmbCausesNotification5.Items.FindByValue(IIf(oCurrentNotification.IDType = 5, intIdCause.ToString, 0))
            Me.cmbCausesNotification8.SelectedItem = Me.cmbCausesNotification8.Items.FindByValue(IIf(oCurrentNotification.IDType = 8, intIdCause.ToString, 0))

            '============ 56 ============
            If oCurrentNotification.IDType = 56 AndAlso oCurrentNotification.Condition IsNot Nothing Then
                If oCurrentNotification.Condition.ConditionRole IsNot Nothing Then
                    Me.tknConditionRole.Value = String.Join(",", oCurrentNotification.Condition.ConditionRole)
                    Me.optConditionRole.Checked = (oCurrentNotification.Condition.ConditionRole.Any AndAlso oCurrentNotification.Condition.ConditionRole(0) <> "")
                End If
                If oCurrentNotification.Condition.TypeToIgnore Is Nothing Then
                    Me.optTypeToIgnore.Checked = False
                Else
                    Me.optTypeToIgnore.Checked = (oCurrentNotification.Condition.TypeToIgnore <> "")
                End If

                If oCurrentNotification.Condition.IDCause IsNot Nothing Then
                    Me.cmbCausesNotification56.Value = Convert.ToInt32(oCurrentNotification.Condition.IDCause)
                Else
                    Me.cmbCausesNotification56.Value = 0
                End If

            End If

            '============ 65 ============
            If oCurrentNotification.IDType = 65 AndAlso oCurrentNotification.Condition IsNot Nothing Then
                If oCurrentNotification.Condition.ConditionRole IsNot Nothing Then
                    Me.tknConditionRole.Value = String.Join(",", oCurrentNotification.Condition.ConditionRole)
                    Me.optConditionRole.Checked = (oCurrentNotification.Condition.ConditionRole.Any AndAlso oCurrentNotification.Condition.ConditionRole(0) <> "")
                End If

                If oCurrentNotification.Condition.IDCause IsNot Nothing Then
                    Me.cmbCausesNotification65.Value = Convert.ToInt32(oCurrentNotification.Condition.IDCause)
                Else
                    Me.cmbCausesNotification65.Value = 0
                End If

            End If

            '============ 16 o 17 ============
            Dim strDatePeriodUserfield As String = String.Empty
            If (oCurrentNotification.IDType = 16 OrElse oCurrentNotification.IDType = 17) AndAlso oCurrentNotification.Condition IsNot Nothing Then
                strDatePeriodUserfield = oCurrentNotification.Condition.DatePeriodUserfield
            End If

            Me.cmbDatePeriodUserField.SelectedItem = Me.cmbDatePeriodUserField.Items.FindByValue(IIf(oCurrentNotification.IDType = 16, strDatePeriodUserfield, String.Empty))

            Me.cmbDatePeriodUserFieldEnter.SelectedItem = Me.cmbDatePeriodUserFieldEnter.Items.FindByValue(IIf(oCurrentNotification.IDType = 17, strDatePeriodUserfield, String.Empty))

            '============ 18 ============
            Dim intIdConcept As Integer = 0
            Dim iCompareConceptType As ConditionCompareType
            Dim strTargetTypeConcept As String = String.Empty
            Dim strTargetConcept As String = String.Empty

            If oCurrentNotification.IDType = 18 AndAlso oCurrentNotification.Condition IsNot Nothing Then
                If oCurrentNotification.Condition.IDConcept.HasValue Then
                    intIdConcept = oCurrentNotification.Condition.IDConcept
                End If
                If oCurrentNotification.Condition.tCompareConceptType.HasValue Then
                    iCompareConceptType = oCurrentNotification.Condition.tCompareConceptType
                End If

                strTargetTypeConcept = oCurrentNotification.Condition.TargetTypeConcept
                If strTargetTypeConcept <> "DirectValue" Then
                    strTargetTypeConcept = "UserField"
                End If
                strTargetConcept = oCurrentNotification.Condition.TargetConcept
            End If

            '============ 34 ============
            If oCurrentNotification.IDType = 50 OrElse oCurrentNotification.IDType = 70 Then
                Me.seNotificationPeriod.Value = oCurrentNotification.Condition.NotificationPeriod
                Me.seNotificationRepeat.Value = oCurrentNotification.Condition.NotificationRepeat
            End If
            Me.cmbConcepts.SelectedItem = Me.cmbConcepts.Items.FindByValue(intIdConcept.ToString)

            If intIdConcept > 0 Then
                Me.cmbCompare.SelectedItem = Me.cmbCompare.Items.FindByValue(roTypes.Any2Integer(iCompareConceptType).ToString)

                Me.cmbTypeValue.SelectedItem = Me.cmbTypeValue.Items.FindByValue(strTargetTypeConcept)

                If strTargetTypeConcept = "DirectValue" Then
                    If IsDate(strTargetConcept) Then
                        Me.txtValueType.DateTime = roTypes.Any2Time(strTargetConcept).TimeOnly
                    Else
                        Me.txtValueType.DateTime = roTypes.Any2Time("00:00").TimeOnly
                    End If

                    Me.cmbConceptUserField.SelectedItem = Me.cmbConceptUserField.Items.FindByValue("")
                Else
                    txtValueType.Value = roTypes.Any2Time("00:00").TimeOnly
                    Me.cmbConceptUserField.SelectedItem = Me.cmbConceptUserField.Items.FindByValue(strTargetConcept)
                End If
            Else
                Me.cmbCompare.SelectedItem = Me.cmbCompare.Items.FindByValue("")

                Me.cmbTypeValue.SelectedItem = Me.cmbTypeValue.Items.FindByValue("")

                txtValueType.Value = roTypes.Any2Time("00:00").TimeOnly

                Me.cmbConceptUserField.SelectedItem = Me.cmbConceptUserField.Items.FindByValue("")

            End If

            '============ 51 / 83============

            oClientControls.selectedShifts = {}
            Me.timeLimitNotification8384.Value = 0
            Me.EmployeeFilter.Value = "None"
            If oCurrentNotification.IDType = 51 AndAlso oCurrentNotification.Condition IsNot Nothing Then
                oClientControls.selectedShifts = oCurrentNotification.Condition.IDShifts
                ckOnlyCalendar.Checked = oCurrentNotification.Condition.OnlyFromCalendar
            ElseIf oCurrentNotification.IDType = 83 AndAlso oCurrentNotification.Condition IsNot Nothing Then
                oClientControls.selectedShifts = oCurrentNotification.Condition.IDShifts
                Me.timeLimitNotification8384.Value = roTypes.Any2Time(oCurrentNotification.Condition.PunchBeforeStartTimeLimit).TimeOnly

                Me.EmployeeFilter.Value = If(String.IsNullOrEmpty(oCurrentNotification.Condition.EmployeeFilter), "None", oCurrentNotification.Condition.EmployeeFilter)
            ElseIf oCurrentNotification.IDType = 84 AndAlso oCurrentNotification.Condition IsNot Nothing Then
                oClientControls.selectedShifts = oCurrentNotification.Condition.IDShifts
                If roTypes.Any2Time(oCurrentNotification.Condition.PunchToleranceTime).TimeOnly <> DateTime.MinValue Then
                    Me.timeLimitNotification8384.Value = oCurrentNotification.Condition.PunchToleranceTime
                Else
                    Me.timeLimitNotification8384.Value = 0
                End If

                Me.EmployeeFilter.Value = If(String.IsNullOrEmpty(oCurrentNotification.Condition.EmployeeFilter), "None", oCurrentNotification.Condition.EmployeeFilter)
            ElseIf oCurrentNotification.IDType = 72 AndAlso oCurrentNotification.Condition IsNot Nothing Then
                Me.EmployeeFilter.Value = If(String.IsNullOrEmpty(oCurrentNotification.Condition.EmployeeFilter), "None", oCurrentNotification.Condition.EmployeeFilter)
            End If

            '================================================================

            '============ Incumplimientos de previsiones (77 y 78) ============
            If (oCurrentNotification.IDType = 77 OrElse oCurrentNotification.IDType = 78) AndAlso oCurrentNotification.Condition IsNot Nothing Then
                Me.cmbBreachCompare.SelectedIndex = oCurrentNotification.Condition.BreachCompareType
                Me.txtBreachCourtesyMinutes.Text = oCurrentNotification.Condition.BreachCourtesy.ToString
            End If
            '================================================================

            '============  83============
            'Me.timeLimitNotification83.Value = roTypes.Any2Time("00:00").TimeOnly
            '===============================

            '============================= QUIEN ============================
            Dim sTypes As Integer() = {1, 2, 6, 9, 11, 13, 14, 15, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30}
            Dim systemNotifications As New Generic.List(Of Integer)
            systemNotifications.AddRange(sTypes)

            'SUPERVISOR PORTAL
            Me.optSupervisorByPortal.Checked = oCurrentNotification.AllowPortal

            Me.optEmployeeByPortal.Checked = oCurrentNotification.AllowVTPortal

            'SUPERVISOR MAIL
            Me.optSupervisorByMail.Checked = oCurrentNotification.AllowMail

            'LISTA DE MAILS
            Dim strDestination As String = ""
            If oCurrentNotification.Destination IsNot Nothing Then
                strDestination = roTypes.Any2String(oCurrentNotification.Destination.Destination)
            End If

            Me.optMailList.Checked = (strDestination <> String.Empty)

            Me.txtMailList.Value = strDestination

            'CAMPO DE LA FICHA
            Dim strMailUserfield As String = ""
            If oCurrentNotification.Condition IsNot Nothing Then
                strMailUserfield = oCurrentNotification.Condition.MailListUserfield
            End If
            Me.optEmployeeField.Checked = (strMailUserfield <> String.Empty)

            Me.cmbUserFieldsNewDesign.SelectedItem = Me.cmbUserFieldsNewDesign.Items.FindByValue(strMailUserfield)

            '================================================================

            bRet = True
        Catch ex As Exception
            ErrorInfo = Me.Language.Translate("Message.UnknowError", Me.DefaultScope)
        End Try

        Return bRet

    End Function

#End Region

End Class