Imports Robotics.Base.DTOs
Imports Robotics.Base.VTEmployees.LabAgree
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class WebUserForms_frmEditRequestValidation
    Inherits UserControlBase

    <Runtime.Serialization.DataContract()>
    Private Class ObjectCallbackRequest

        <Runtime.Serialization.DataMember(Name:="aTab")>
        Public aTab As Integer

        <Runtime.Serialization.DataMember(Name:="ID")>
        Public ID As Integer

        <Runtime.Serialization.DataMember(Name:="IDLabAgree")>
        Public IDLabAgree As Integer

        <Runtime.Serialization.DataMember(Name:="Name")>
        Public Name As String

        <Runtime.Serialization.DataMember(Name:="action")>
        Public Action As String

        <Runtime.Serialization.DataMember(Name:="Tokens")>
        Public Tokens As String

        <Runtime.Serialization.DataMember(Name:="IDRequestType")>
        Public IDRequestType As Integer

        <Runtime.Serialization.DataMember(Name:="StampParam")>
        Public StampParam As Double

        <Runtime.Serialization.DataMember(Name:="idtype")>
        Public idtype As String

    End Class

#Region "Helper private methods"

    Private Property LabAgreeRequestValidationValuesData(Optional ByVal bolReload As Boolean = False) As Generic.List(Of roRequestRule)
        Get

            Dim tbValues As Generic.List(Of roRequestRule) = Session("LabAgree_LabAgreedRequestValidations")

            If bolReload Or tbValues Is Nothing Then

                Dim oList As New Generic.List(Of roRequestRule)

                If roTypes.Any2Integer(Session("LabAgree_SelectedID")) > -1 Then
                    Dim oCurrentLabAgree As roLabAgree = API.LabAgreeServiceMethods.GetLabAgreeByID(Me.Page, Session("LabAgree_SelectedID"), True)
                    oList.AddRange(oCurrentLabAgree.LabAgreeRequestRules())
                End If

                tbValues = oList
                Session("LabAgree_LabAgreedRequestValidations") = tbValues

            End If
            Return tbValues

        End Get
        Set(ByVal value As Generic.List(Of roRequestRule))
            If value IsNot Nothing Then
                Session("LabAgree_LabAgreedRequestValidations") = value
            Else
                Session("LabAgree_LabAgreedRequestValidations") = Nothing
            End If
        End Set
    End Property

    Private Function getMinStartupID() As Integer
        Dim newId As Integer = 0

        Dim oList As Generic.List(Of roRequestRule) = LabAgreeRequestValidationValuesData()

        If oList IsNot Nothing AndAlso oList.Count > 0 Then
            Dim index As Integer = 0
            For Each oElem As roRequestRule In oList
                If index = 0 Then
                    newId = oElem.IDRule
                Else
                    If oElem.IDRule < newId Then
                        newId = oElem.IDRule
                    End If
                End If
                index = index + 1
            Next
        End If

        If newId >= 0 Then
            Return -2
        Else
            Return newId - 1
        End If
    End Function

    'Private Function ExistsConceptInList(ByVal oId As Integer, ByVal oIdConcept As Integer) As Boolean
    '    Dim bExists As Boolean = False

    '    Dim oList As Generic.List(Of roStartupValue) = LabAgreeStartUpValuesData()

    '    If oList IsNot Nothing AndAlso oList.Count > 0 Then
    '        For Each oElem As roStartupValue In oList
    '            If oElem.ID <> oId AndAlso oElem.IDConcept = oIdConcept Then
    '                bExists = True
    '                Exit For
    '            End If
    '        Next
    '    End If

    '    Return bExists
    'End Function

    Private Function getRequestValidation(ByVal oId As Integer) As roRequestRule
        Dim oObject As roRequestRule = Nothing

        Dim oList As Generic.List(Of roRequestRule) = LabAgreeRequestValidationValuesData()

        If oList IsNot Nothing AndAlso oList.Count > 0 Then
            For Each oElem As roRequestRule In oList
                If oElem.IDRule = oId Then
                    oObject = roSupport.DeepClone(Of roRequestRule)(oElem)

                    Exit For
                End If
            Next
        End If

        Return oObject
    End Function

    Private Function setRequestValidation(ByVal oObject As roRequestRule, ByVal bIsNew As Boolean) As Boolean
        Dim bResult As Boolean = False

        Dim oList As Generic.List(Of roRequestRule) = LabAgreeRequestValidationValuesData()

        If Not bIsNew Then
            Dim index As Integer = 0
            If oList IsNot Nothing AndAlso oList.Count > 0 Then
                For Each oElem As roRequestRule In oList
                    If oElem.IDRule = oObject.IDRule Then
                        oList(index) = oObject
                        bResult = True
                        LabAgreeRequestValidationValuesData = oList
                        Exit For
                    End If
                    index = index + 1
                Next
            End If
        Else
            If oList Is Nothing Then oList = New Generic.List(Of roRequestRule)
            oList.Add(oObject)
            LabAgreeRequestValidationValuesData = oList
        End If

        Return bResult
    End Function

#End Region

    Private Sub WebUserForms_frmEditRequestValidation_Init(sender As Object, e As EventArgs) Handles Me.Init
        Dim cacheManager As New Robotics.Web.Base.NoCachePageBase
        cacheManager.InsertExtraJavascript("roOptionPanelClient", "~/Base/Scripts/roOptionPanelClient.js")
        cacheManager.InsertExtraJavascript("roOptPanelClientGroup", "~/Base/Scripts/roOptPanelClientGroup.js")
    End Sub

    Private Sub Page_Load1(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            LoadDefaultCombos()

        End If
    End Sub

    Private Sub LoadDefaultCombos()
        Dim dTblRequests As DataTable = API.LabAgreeServiceMethods.GetAvailableRequestType(Me.Page)
        Me.cmbAvailableRequests.Items.Clear()
        Me.cmbAvailableRequests.ValueType = GetType(Integer)

        For Each dRowReqType As DataRow In dTblRequests.Rows
            Me.cmbAvailableRequests.Items.Add(dRowReqType("reqType"), dRowReqType("IdType"))
        Next

        Me.cmbAvailableRequests.SelectedItem = Me.cmbAvailableRequests.Items(0)

        Me.cmbDayType.Items.Clear()
        Me.cmbDayType.ValueType = GetType(Integer)
        Me.cmbDayType.Items.Add(Me.Language.Translate("MaxDays.Natural", Me.DefaultScope), 0)
        Me.cmbDayType.Items.Add(Me.Language.Translate("MaxDays.Working", Me.DefaultScope), 1)
        Me.cmbDayType.SelectedItem = Me.cmbDayType.Items(0)

        Me.cmbMinDaysType.Items.Clear()
        Me.cmbMinDaysType.ValueType = GetType(Integer)
        Me.cmbMinDaysType.Items.Add(Me.Language.Translate("MaxDays.Natural", Me.DefaultScope), 0)
        Me.cmbMinDaysType.Items.Add(Me.Language.Translate("MaxDays.Working", Me.DefaultScope), 1)
        Me.cmbMinDaysType.SelectedItem = Me.cmbDayType.Items(0)

        'Dim dTblCauses As DataTable = CausesService.CausesServiceMethods.GetCausesShortList(Me.Page)

        'Me.tbMaxDayCause.Items.Clear()
        'Me.tbMinDayCause.Items.Clear()
        'Me.tbLimitDateRequested.Items.Clear()
        'For Each dRow As DataRow In dTblCauses.Rows
        '    Me.tbMaxDayCause.Items.Add(dRow("Name"), dRow("ID"))
        '    Me.tbMinDayCause.Items.Add(dRow("Name"), dRow("ID"))
        '    Me.tbLimitDateRequested.Items.Add(dRow("Name"), dRow("ID"))
        'Next

        'Me.cmbMaxDayCause.SelectedItem = Me.cmbMaxDayCause.Items(0)

        Me.dtAllowPeriodInitial.Date = New DateTime(DateTime.Now.Year, 1, 1)
        Me.dtAllowPeriodEnd.Date = Me.dtAllowPeriodInitial.Date.AddYears(1).AddDays(-1)

        Me.cmbAllowPeriodYearInitial.Items.Clear()
        Me.cmbAllowPeriodYearEnd.Items.Clear()
        Me.cmbAllowPeriodYearInitial.ValueType = GetType(Integer)
        Me.cmbAllowPeriodYearEnd.ValueType = GetType(Integer)

        Me.cmbAllowPeriodYearInitial.Items.Add(Me.Language.Translate("AllowPeriod.ActualYear", Me.DefaultScope), 0)
        Me.cmbAllowPeriodYearInitial.Items.Add(Me.Language.Translate("AllowPeriod.NextYear", Me.DefaultScope), 1)
        Me.cmbAllowPeriodYearEnd.Items.Add(Me.Language.Translate("AllowPeriod.ActualYear", Me.DefaultScope), 0)
        Me.cmbAllowPeriodYearEnd.Items.Add(Me.Language.Translate("AllowPeriod.NextYear", Me.DefaultScope), 1)

        Me.cmbAllowPeriodYearInitial.SelectedItem = Me.cmbAllowPeriodYearInitial.Items(0)
        Me.cmbAllowPeriodYearEnd.SelectedItem = Me.cmbAllowPeriodYearEnd.Items(0)

        Me.cmbAutomaticDayType.Items.Clear()
        Me.cmbAutomaticDayType.ValueType = GetType(Integer)
        Me.cmbAutomaticDayType.Items.Add(Me.Language.Translate("MaxDays.Natural", Me.DefaultScope), 0)
        Me.cmbAutomaticDayType.Items.Add(Me.Language.Translate("MaxDays.Working", Me.DefaultScope), 1)
        Me.cmbAutomaticDayType.SelectedItem = Me.cmbAutomaticDayType.Items(0)

        ReloadRuleTypes(True)
    End Sub

    Private Sub ReloadRuleTypes(Optional ByVal bSetSelectedItem As Boolean = False)
        Dim tbTypes As DataTable = API.LabAgreeServiceMethods.GetRuleTypesByRequestType(Me.Page, Me.cmbAvailableRequests.SelectedItem.Value)

        Me.cmbRuleType.Items.Clear()
        Me.cmbRuleType.ValueType = GetType(Integer)
        For Each dRowReqType As DataRow In tbTypes.Rows
            Me.cmbRuleType.Items.Add(dRowReqType("RuleTypeTranslate"), dRowReqType("Id"))
        Next
        If bSetSelectedItem Then Me.cmbRuleType.SelectedItem = Me.cmbRuleType.Items(0)
    End Sub

    Protected Sub ASPxRequestValidationCallbackContenido_Callback(ByVal source As Object, ByVal e As DevExpress.Web.CallbackEventArgs) Handles ASPxRequestValidationCallbackContenido.Callback
        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Dim oParameters As New ObjectCallbackRequest()
        oParameters = roJSONHelper.Deserialize(strParameter, oParameters.GetType())

        Select Case oParameters.Action
            Case "CHECKAUTOMATICRULES"
                GetRulesForAutomaticValidation(oParameters)
        End Select

    End Sub

    Protected Sub ASPxCallbackPanelContenido_Callback(ByVal sender As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase) Handles ASPxRequestValidationCallbackPanelContenido.Callback

        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Dim oParameters As New ObjectCallbackRequest()
        oParameters = roJSONHelper.Deserialize(strParameter, oParameters.GetType())

        Dim bRet As Boolean = False

        If Me.cmbAvailableRequests.SelectedItem IsNot Nothing Then
            ReloadRuleTypes()
        End If
        ASPxRequestValidationCallbackPanelContenido.JSProperties.Clear()

        Select Case oParameters.Action
            Case "GETLABAGREEREQUUESTVALIDATION"
                LoadLabAgreeRequestValidation(oParameters)
            Case "SAVELABAGREEREQUESTVALIDATION"
                SaveLabAgreeRequestValidation(oParameters)
            Case "LOADRULECONFIGURATION"
                LoadDefaultRuleConfiguration()
            Case "RELOADRULETYPES"
                ReloadRuleTypes()
                Me.cmbRuleType.SelectedItem = Me.cmbRuleType.Items(0)
                LoadDefaultRuleConfiguration()

                ASPxRequestValidationCallbackPanelContenido.JSProperties.Add("cpActionRO", "RELOADRULETYPES")
                ASPxRequestValidationCallbackPanelContenido.JSProperties.Add("cpResultRO", "OK")
                ASPxRequestValidationCallbackPanelContenido.JSProperties.Add("cpIsNewRO", False)
            Case Else
                ASPxRequestValidationCallbackPanelContenido.JSProperties.Add("cpAction", "UNKNOW")
                ASPxRequestValidationCallbackPanelContenido.JSProperties.Add("cpResult", "NOK")
                ASPxRequestValidationCallbackPanelContenido.JSProperties.Add("cpMessage", "Unknow call")
        End Select
    End Sub

    Private Sub LoadLabAgreeRequestValidation(ByVal oParameters As ObjectCallbackRequest, Optional ByVal eStartupValue As roRequestRule = Nothing)
        Dim oCurrentRule As roRequestRule = Nothing
        Dim result As String = "OK"
        Dim oUsedInRules As New Generic.List(Of String)
        Dim strcombosIndex As String = ""

        Try

            If eStartupValue IsNot Nothing Then
                oCurrentRule = eStartupValue
            Else
                If oParameters.ID = "-1" Then
                    oCurrentRule = New roRequestRule
                    oCurrentRule.IDRule = getMinStartupID()
                    oCurrentRule.IDLabAgree = roTypes.Any2Integer(Session("LabAgree_SelectedID"))
                    oCurrentRule.Activated = True
                    oCurrentRule.Name = String.Empty
                    oCurrentRule.Description = String.Empty

                    oCurrentRule.Definition = New roRequestRuleDefinition
                    oCurrentRule.Definition.Action = eActionType.Denied_Action
                    oCurrentRule.Definition.EmployeeValidation = True
                    oCurrentRule.Definition.IDReasons = New List(Of Integer)
                Else
                    oCurrentRule = getRequestValidation(oParameters.ID)
                End If
            End If

            If oCurrentRule Is Nothing Then Exit Sub

            Me.txtName.Text = oCurrentRule.Name
            Me.txtDescription.Text = oCurrentRule.Description

            Me.cmbAvailableRequests.SelectedItem = Me.cmbAvailableRequests.Items.FindByValue(CInt(oCurrentRule.IDRequestType))
            If Me.cmbAvailableRequests.SelectedItem IsNot Nothing Then
                ReloadRuleTypes()
                Me.cmbRuleType.SelectedItem = Me.cmbRuleType.Items.FindByValue(CInt(oCurrentRule.IDRuleType))
            Else
                Me.cmbRuleType.SelectedItem = Nothing
            End If

            Me.ckRuleActive.Checked = oCurrentRule.Activated

            If oCurrentRule.Definition.Action = eActionType.Denied_Action Then
                Me.ckActionForbid.Checked = True
                Me.ckActionAsk.Checked = False
            Else
                Me.ckActionForbid.Checked = False
                Me.ckActionAsk.Checked = True
            End If

            If oCurrentRule.Definition.EmployeeValidation Then
                Me.ckCheckOnEmployeeRequest.Checked = True
                Me.ckCheckOnSupervisorApprove.Checked = False
            Else
                Me.ckCheckOnSupervisorApprove.Checked = True
                Me.ckCheckOnEmployeeRequest.Checked = False
            End If

            LoadDefaultRuleConfiguration()

            If Me.cmbRuleType.SelectedItem IsNot Nothing Then
                Select Case Me.cmbRuleType.SelectedItem.Value
                    Case eRequestRuleType.NegativeAccrual 'PositiveAccrual
                        Select Case Me.cmbAvailableRequests.SelectedItem.Value
                            Case eRequestType.VacationsOrPermissions, eRequestType.PlannedHolidays, eRequestType.PlannedAbsences, eRequestType.PlannedCauses, eRequestType.PlannedOvertimes
                                If oCurrentRule.Definition.IDReasons IsNot Nothing Then
                                    Me.tbPositiveAccrualReason.Value = String.Join(",", oCurrentRule.Definition.IDReasons)
                                Else
                                    Me.tbPositiveAccrualReason.Value = String.Empty
                                End If

                        End Select
                    Case eRequestRuleType.MaxNumberDays 'MaxNumberDays
                        If oCurrentRule.Definition.IDReasons IsNot Nothing Then
                            Me.tbMaxDayCause.Value = String.Join(",", oCurrentRule.Definition.IDReasons)
                        Else
                            Me.tbMaxDayCause.Value = String.Empty
                        End If

                        Me.txtMaxDaysValue.Value = oCurrentRule.Definition.MaxDays.Value
                        Me.cmbDayType.Value = CInt(oCurrentRule.Definition.TypePlannedDay.Value)

                    Case eRequestRuleType.MinimumConsecutiveDays 'MinimumConsecutiveDays
                        If oCurrentRule.Definition.IDReasons IsNot Nothing Then
                            Me.tbMinConsecutiveDayCause.Value = String.Join(",", oCurrentRule.Definition.IDReasons)
                        Else
                            Me.tbMinConsecutiveDayCause.Value = String.Empty
                        End If

                        Me.txtMinConsecutiveDaysValue.Value = oCurrentRule.Definition.MinDays.Value

                    Case eRequestRuleType.PeriodEnjoyment 'PeriodEnjoyment
                        Me.tbAppliPeriodReason.Value = String.Join(",", oCurrentRule.Definition.IDReasons) 'Me.cmbAppliPeriodReason.Items.FindByValue(oCurrentRule.Definition.IDReason)
                        Me.dtAllowPeriodInitial.Date = New DateTime(DateTime.Now.Year, oCurrentRule.Definition.BeginPeriod.Value.Month, oCurrentRule.Definition.BeginPeriod.Value.Day)
                        Me.dtAllowPeriodEnd.Date = New DateTime(DateTime.Now.Year, oCurrentRule.Definition.EndPeriod.Value.Month, oCurrentRule.Definition.EndPeriod.Value.Day)


                        Me.cmbAllowPeriodYearInitial.SelectedItem = Me.cmbAllowPeriodYearInitial.Items.FindByValue(If(oCurrentRule.Definition.BeginPeriod.Value.Year = 1900, 0, 1))
                        Me.cmbAllowPeriodYearEnd.SelectedItem = Me.cmbAllowPeriodYearEnd.Items.FindByValue(If(oCurrentRule.Definition.EndPeriod.Value.Year = 1900, 0, 1))

                    Case eRequestRuleType.AutomaticValidation 'AutomaticValidation
                        If oCurrentRule.Definition.IDReasons IsNot Nothing Then
                            Me.tbAutomaticValidationReason.Value = String.Join(",", oCurrentRule.Definition.IDReasons)
                        Else
                            Me.tbAutomaticValidationReason.Value = String.Empty
                        End If

                        Me.txtAutomaticValidationDays.Value = oCurrentRule.Definition.MaxDays
                        Me.ckRuleActive.Checked = True
                        Me.cmbAutomaticDayType.Value = CInt(oCurrentRule.Definition.TypePlannedDay.Value)
                        For Each oRule In LabAgreeRequestValidationValuesData()
                            'Buscamos si existe otra regla para el mismo tipo de solicitud de tipo automatica
                            If oRule.IDRequestType = oCurrentRule.IDRequestType AndAlso oRule.IDRuleType <> 4 AndAlso oRule.IDRule <> oCurrentRule.IDRule AndAlso
                                oRule.Definition.EmployeeValidation = False AndAlso oRule.Activated Then

                                If oRule.Definition.IDReasons.Intersect(oCurrentRule.Definition.IDReasons).Count > 0 Then
                                    oUsedInRules.Add(oRule.Name)
                                End If
                            End If
                        Next
                    Case eRequestRuleType.LimitDateRequested
                        If oCurrentRule.Definition.IDReasons IsNot Nothing Then
                            Me.tbLimitDateRequested.Value = String.Join(",", oCurrentRule.Definition.IDReasons)
                        Else
                            Me.tbLimitDateRequested.Value = String.Empty
                        End If

                        Me.txtLimitDateRequestedValue.Value = oCurrentRule.Definition.MaxDays.Value
                        If oCurrentRule.Definition.TypeCriteriaEmployee.HasValue AndAlso oCurrentRule.Definition.TypeCriteriaEmployee.Value = eTypeEmployeeCriteria.FilteredEmployees Then
                            Me.opVisibilityAll.Checked = False
                            Me.opVisibilityCriteria.Checked = True

                            If oCurrentRule.RuleConditions IsNot Nothing AndAlso oCurrentRule.RuleConditions.Count > 0 Then
                                strcombosIndex = Me.visibilityCriteria.SetFilterValue(oCurrentRule.RuleConditions(0))
                            End If
                        Else
                            Me.opVisibilityAll.Checked = True
                            Me.opVisibilityCriteria.Checked = False
                        End If
                    Case eRequestRuleType.MinimumDaysBeforeRequestedDate 'MinDeaysBefore
                        If oCurrentRule.Definition.IDReasons IsNot Nothing Then
                            Me.tbMinDayCause.Value = String.Join(",", oCurrentRule.Definition.IDReasons)
                        Else
                            Me.tbMinDayCause.Value = String.Empty
                        End If

                        Me.txtMinDaysValue.Value = oCurrentRule.Definition.MaxDays.Value
                        Me.cmbMinDaysType.Value = CInt(oCurrentRule.Definition.TypePlannedDay.Value)
                    Case eRequestRuleType.MaxNotScheduledDays 'MaxNotScheduledDays
                        If oCurrentRule.Definition.IDReasons IsNot Nothing Then
                            Me.tbMaxNotScheduledDays.Value = String.Join(",", oCurrentRule.Definition.IDReasons)
                        Else
                            Me.tbMaxNotScheduledDays.Value = String.Empty
                        End If
                    Case eRequestRuleType.MinCoverageRequiered 'MinCoverageRequiered
                        If oCurrentRule.Definition.IDReasons IsNot Nothing Then
                            Me.tbMinCoverageRequiered.Value = String.Join(",", oCurrentRule.Definition.IDReasons)
                        Else
                            Me.tbMinCoverageRequiered.Value = String.Empty
                        End If
                    Case eRequestRuleType.AutomaticRejection 'AutomaticRejection
                        If oCurrentRule.Definition.IDReasons IsNot Nothing Then
                            Me.tbAutomaticRejectionReason.Value = String.Join(",", oCurrentRule.Definition.IDReasons)
                        Else
                            Me.tbAutomaticRejectionReason.Value = String.Empty
                        End If

                        Me.txtAutomaticRejectionDays.Value = oCurrentRule.Definition.MaxDays
                        Me.ckRuleActive.Checked = True
                        'For Each oRule In LabAgreeRequestRejectionValuesData()
                        '    'Buscamos si existe otra regla para el mismo tipo de solicitud de tipo automatica
                        '    If oRule.IDRequestType = oCurrentRule.IDRequestType AndAlso oRule.IDRuleType <> 4 AndAlso oRule.IDRule <> oCurrentRule.IDRule AndAlso
                        '        oRule.Definition.EmployeeRejection = False AndAlso oRule.Activated Then

                        '        If oRule.Definition.IDReasons.Intersect(oCurrentRule.Definition.IDReasons).Count > 0 Then
                        '            oUsedInRules.Add(oRule.Name)
                        '        End If
                        '    End If
                        'Next
                    Case eRequestRuleType.ScheduleRuleToValidate
                        If oCurrentRule.Definition.IDReasons IsNot Nothing Then
                            Me.tbPlanificationRulesShifts.Value = String.Join(",", oCurrentRule.Definition.IDReasons)
                        Else
                            Me.tbPlanificationRulesShifts.Value = String.Empty
                        End If

                        If oCurrentRule.Definition.IDPlanificationRules IsNot Nothing Then
                            Me.tbPlanificationRules.Value = String.Join(",", oCurrentRule.Definition.IDPlanificationRules)
                        Else
                            Me.tbPlanificationRules.Value = String.Empty
                        End If
                End Select
            End If
        Catch ex As Exception
            result = "KO"
        Finally
            ASPxRequestValidationCallbackPanelContenido.JSProperties.Add("cpActionRO", "GETLABAGREEREQUUESTVALIDATION")
            ASPxRequestValidationCallbackPanelContenido.JSProperties.Add("cpNameRO", oCurrentRule.Name)
            ASPxRequestValidationCallbackPanelContenido.JSProperties.Add("cpUsedInRules", oUsedInRules.ToArray)
            ASPxRequestValidationCallbackPanelContenido.JSProperties.Add("cpCombosIndexRO", strcombosIndex)
            ASPxRequestValidationCallbackPanelContenido.JSProperties.Add("cpResultRO", result)
            ASPxRequestValidationCallbackPanelContenido.JSProperties.Add("cpIsNewRO", False)
        End Try

    End Sub

    Private Sub LoadDefaultRuleConfiguration(Optional ByVal bResetItem As Boolean = True)
        Dim result As String = "OK"
        Try
            Me.tbPositiveAccrualReason.Items.Clear()
            Me.tbAppliPeriodReason.Items.Clear()
            Me.tbAutomaticValidationReason.Items.Clear()
            Me.tbMaxNotScheduledDays.Items.Clear()
            Me.tbMinCoverageRequiered.Items.Clear()
            Me.tbAutomaticRejectionReason.Items.Clear()
            Me.tbMaxDayCause.Items.Clear()
            Me.tbMinConsecutiveDayCause.Items.Clear()
            Me.tbMinDayCause.Items.Clear()
            Me.tbLimitDateRequested.Items.Clear()
            Me.tbPlanificationRules.Items.Clear()
            Me.tbPlanificationRulesShifts.Items.Clear()
            'Me.tbAppliPeriodReason.ValueType = GetType(Integer)

            If bResetItem Then
                Me.tbPositiveAccrualReason.Tokens.Clear()
                Me.tbAppliPeriodReason.Tokens.Clear()
                Me.tbAutomaticValidationReason.Tokens.Clear()
                Me.tbMaxNotScheduledDays.Tokens.Clear()
                Me.tbMinCoverageRequiered.Tokens.Clear()
                Me.tbAutomaticRejectionReason.Tokens.Clear()
                Me.tbMaxDayCause.Tokens.Clear()
                Me.tbMinConsecutiveDayCause.Tokens.Clear()
                Me.tbMinDayCause.Tokens.Clear()
                Me.tbLimitDateRequested.Tokens.Clear()
                Me.tbPlanificationRules.Items.Clear()
                Me.tbPlanificationRulesShifts.Items.Clear()
            End If

            Dim dtReasons As DataTable = Nothing

            If Me.cmbAvailableRequests.SelectedItem Is Nothing Then
                dtReasons = CausesServiceMethods.GetCausesShortList(Me.Page)
            ElseIf Me.cmbAvailableRequests.SelectedItem.Value = eRequestType.VacationsOrPermissions Then
                dtReasons = API.ShiftServiceMethods.GetHolidayShifts(Me.Page)
            ElseIf Me.cmbAvailableRequests.SelectedItem.Value = eRequestType.PlannedHolidays Then
                dtReasons = CausesServiceMethods.GetCauses(Me.Page, "IsHoliday = 1")
            ElseIf Me.cmbAvailableRequests.SelectedItem.Value = eRequestType.ChangeShift OrElse Me.cmbAvailableRequests.SelectedItem.Value = eRequestType.ExchangeShiftBetweenEmployees Then
                dtReasons = API.ShiftServiceMethods.GetShifts(Me.Page)
            ElseIf Me.cmbAvailableRequests.SelectedItem.Value = eRequestType.PlannedOvertimes Then
                dtReasons = CausesServiceMethods.GetCauses(Me.Page, "WorkingType = 1 AND IsHoliday = 0 AND DayType = 0 AND CustomType=0")
            Else
                dtReasons = CausesServiceMethods.GetCausesShortList(Me.Page)
            End If

            If Me.cmbRuleType.SelectedItem IsNot Nothing Then
                Select Case Me.cmbRuleType.SelectedItem.Value
                    Case eRequestRuleType.NegativeAccrual 'PositiveAccrual
                        For Each oRow As DataRow In dtReasons.Rows
                            Me.tbPositiveAccrualReason.Items.Add(oRow("Name"), oRow("ID"))
                        Next
                        If bResetItem Then tbPositiveAccrualReason.Tokens.Clear()
                    Case eRequestRuleType.MaxNumberDays 'MaxNumberDays
                        For Each oRow As DataRow In dtReasons.Rows
                            Me.tbMaxDayCause.Items.Add(oRow("Name"), oRow("ID"))
                        Next
                        If bResetItem Then tbMaxDayCause.Tokens.Clear()
                    Case eRequestRuleType.MinimumConsecutiveDays 'MinimumConsecutiveDays
                        For Each oRow As DataRow In dtReasons.Rows
                            Me.tbMinConsecutiveDayCause.Items.Add(oRow("Name"), oRow("ID"))
                        Next
                        If bResetItem Then tbMinConsecutiveDayCause.Tokens.Clear()
                    Case eRequestRuleType.PeriodEnjoyment 'PeriodEnjoyment

                        For Each oRow As DataRow In dtReasons.Rows
                            Me.tbAppliPeriodReason.Items.Add(oRow("Name"), oRow("ID"))
                        Next
                        If bResetItem Then tbAppliPeriodReason.Tokens.Clear()
                    Case eRequestRuleType.AutomaticValidation ' Validación automática
                        For Each oRow As DataRow In dtReasons.Rows
                            Me.tbAutomaticValidationReason.Items.Add(oRow("Name"), oRow("ID"))
                        Next
                        If bResetItem Then Me.txtAutomaticValidationDays.Value = "2"
                    Case eRequestRuleType.LimitDateRequested
                        For Each oRow As DataRow In dtReasons.Rows
                            Me.tbLimitDateRequested.Items.Add(oRow("Name"), oRow("ID"))
                        Next

                        If bResetItem Then
                            Me.opVisibilityAll.Checked = True
                            Me.opVisibilityCriteria.Checked = False
                            Me.visibilityCriteria.ClearFilterValue()
                            Me.tbLimitDateRequested.Tokens.Clear()
                        End If
                    Case eRequestRuleType.MinimumDaysBeforeRequestedDate
                        For Each oRow As DataRow In dtReasons.Rows
                            Me.tbMinDayCause.Items.Add(oRow("Name"), oRow("ID"))
                        Next

                        If bResetItem Then
                            Me.tbMinDayCause.Tokens.Clear()
                        End If
                    Case eRequestRuleType.MaxNotScheduledDays
                        For Each oRow As DataRow In dtReasons.Rows
                            Me.tbMaxNotScheduledDays.Items.Add(oRow("Name"), oRow("ID"))
                        Next
                        If bResetItem Then tbMaxNotScheduledDays.Tokens.Clear()
                    Case eRequestRuleType.MinCoverageRequiered
                        For Each oRow As DataRow In dtReasons.Rows
                            Me.tbMinCoverageRequiered.Items.Add(oRow("Name"), oRow("ID"))
                        Next
                        If bResetItem Then tbMinCoverageRequiered.Tokens.Clear()

                    Case eRequestRuleType.AutomaticRejection
                        For Each oRow As DataRow In dtReasons.Rows
                            Me.tbAutomaticRejectionReason.Items.Add(oRow("Name"), oRow("ID"))
                        Next
                        If bResetItem Then
                            Me.tbAutomaticRejectionReason.Tokens.Clear()
                            Me.txtAutomaticRejectionDays.Value = "2"
                        End If

                    Case eRequestRuleType.ScheduleRuleToValidate
                        Me.tbPlanificationRulesShifts.Items.Add(Me.Language.Translate("planificationrule.allvalueoption", Me.DefaultScope), -1)

                        For Each oRow As DataRow In dtReasons.Rows
                            Me.tbPlanificationRulesShifts.Items.Add(oRow("Name"), oRow("ID"))
                        Next
                        'TODO Aqui mostramos las reglas de planificacion que estan guardadas en base de datos, lo suyo seria recoger los datos directamente del GRID o alguna alternativa, porque ahora debemos añadir una regla de planificacion a su grid, guardar cambios y venir a este combo
                        Dim lScheduleRules = API.ScheduleRulesServiceMethods.GetLabAgreeScheduleRules(Me.Page, roTypes.Any2Integer(Session("LabAgree_SelectedID"))).ToList
                        'Añadimos el valor TODOS
                        Me.tbPlanificationRules.Items.Add(Me.Language.Translate("planificationrule.allvalueoptionrule", Me.DefaultScope), -1)
                        For Each requestRule As roScheduleRule In lScheduleRules
                            If requestRule.RuleType = ScheduleRuleBaseType.System Then
                                Dim translatedName = Me.Language.Translate("ScheduleRuleType." & System.Enum.GetName(GetType(ScheduleRuleType), requestRule.IDRule), Me.DefaultScope)

                                Me.tbPlanificationRules.Items.Add(translatedName, CInt(requestRule.Id))
                            Else
                                Me.tbPlanificationRules.Items.Add(requestRule.RuleName, CInt(requestRule.Id))
                            End If
                        Next
                        If bResetItem Then
                            tbPlanificationRulesShifts.Tokens.Clear()
                            tbPlanificationRules.Tokens.Clear()
                        End If
                End Select
            End If
        Catch ex As Exception
            result = "KO"
        End Try

    End Sub

    Private Sub SaveLabAgreeRequestValidation(ByVal oParameters As ObjectCallbackRequest)
        Dim rError As New roJSON.JSONError(False, "")
        Dim oCurrentRule As roRequestRule = Nothing
        Dim bInclompleteData As Boolean = False
        Dim oUsedInRules As New Generic.List(Of String)

        Try
            Dim bolIsNew As Boolean = False
            If oParameters.ID = "-1" Then bolIsNew = True

            If bolIsNew Then
                oCurrentRule = New roRequestRule
                oCurrentRule.IDRule = getMinStartupID()
                oCurrentRule.Definition = New roRequestRuleDefinition()
            Else
                oCurrentRule = getRequestValidation(oParameters.ID)
            End If

            If oCurrentRule Is Nothing Then Exit Sub

            oCurrentRule.Name = Me.txtName.Text
            oCurrentRule.Description = Me.txtDescription.Text
            oCurrentRule.Activated = Me.ckRuleActive.Checked
            oCurrentRule.IDRequestType = Me.cmbAvailableRequests.SelectedItem.Value

            ReloadRuleTypes()
            oCurrentRule.IDRuleType = Me.cmbRuleType.SelectedItem.Value

            If Me.ckActionForbid.Checked = True Then
                oCurrentRule.Definition.Action = eActionType.Denied_Action
            Else
                oCurrentRule.Definition.Action = eActionType.WarnAsk_Value
            End If

            If Me.ckCheckOnEmployeeRequest.Checked Then
                oCurrentRule.Definition.EmployeeValidation = True
            Else
                oCurrentRule.Definition.EmployeeValidation = False
            End If

            LoadDefaultRuleConfiguration(False)

            If Me.cmbRuleType.SelectedItem IsNot Nothing Then
                Select Case Me.cmbRuleType.SelectedItem.Value
                    Case eRequestRuleType.NegativeAccrual 'PositiveAccrual
                        If tbPositiveAccrualReason.Tokens.Count > 0 Then
                            oCurrentRule.Definition.IDReasons = (From s As Integer In roTypes.Any2String(tbPositiveAccrualReason.Value).Split(","c)).ToList
                        Else
                            bInclompleteData = True
                        End If

                        oCurrentRule.Definition.TypePlannedDay = Nothing
                        oCurrentRule.Definition.MaxDays = Nothing
                        oCurrentRule.Definition.BeginPeriod = Nothing
                        oCurrentRule.Definition.EndPeriod = Nothing
                    Case eRequestRuleType.MaxNumberDays 'MaxNumberDays
                        If tbMaxDayCause.Tokens.Count > 0 Then
                            oCurrentRule.Definition.IDReasons = (From s As Integer In roTypes.Any2String(tbMaxDayCause.Value).Split(","c)).ToList
                        Else
                            bInclompleteData = True
                        End If
                        'oCurrentRule.Definition.IDReason = Me.cmbMaxDayCause.SelectedItem.Value
                        oCurrentRule.Definition.MaxDays = roTypes.Any2Integer(Me.txtMaxDaysValue.Value)

                        oCurrentRule.Definition.TypePlannedDay = If(Me.cmbDayType.SelectedItem.Value = 0, eTypePlannedDay.NaturalDay, eTypePlannedDay.LaboralDay)

                        oCurrentRule.Definition.BeginPeriod = Nothing
                        oCurrentRule.Definition.EndPeriod = Nothing

                    Case eRequestRuleType.MinimumConsecutiveDays 'MinimumConsecutiveDays
                        If tbMinConsecutiveDayCause.Tokens.Count > 0 Then
                            oCurrentRule.Definition.IDReasons = (From s As Integer In roTypes.Any2String(tbMinConsecutiveDayCause.Value).Split(","c)).ToList
                        Else
                            bInclompleteData = True
                        End If
                        oCurrentRule.Definition.MinDays = roTypes.Any2Integer(Me.txtMinConsecutiveDaysValue.Value)

                        oCurrentRule.Definition.TypePlannedDay = Nothing

                        oCurrentRule.Definition.BeginPeriod = Nothing
                        oCurrentRule.Definition.EndPeriod = Nothing

                    Case eRequestRuleType.PeriodEnjoyment 'PeriodEnjoyment
                        If tbAppliPeriodReason.Tokens.Count > 0 Then
                            oCurrentRule.Definition.IDReasons = (From s As Integer In roTypes.Any2String(tbAppliPeriodReason.Value).Split(","c)).ToList
                        Else
                            bInclompleteData = True
                        End If

                        oCurrentRule.Definition.BeginPeriod = New DateTime(If(Me.cmbAllowPeriodYearInitial.SelectedItem.Value = 0, 1900, 1901), dtAllowPeriodInitial.Date.Month, dtAllowPeriodInitial.Date.Day)
                        oCurrentRule.Definition.EndPeriod = New DateTime(If(Me.cmbAllowPeriodYearEnd.SelectedItem.Value = 0, 1900, 1901), dtAllowPeriodEnd.Date.Month, dtAllowPeriodEnd.Date.Day)

                        oCurrentRule.Definition.TypePlannedDay = Nothing
                        oCurrentRule.Definition.MaxDays = Nothing
                    Case eRequestRuleType.AutomaticValidation 'AutomaticValidation
                        If tbAutomaticValidationReason.Tokens.Count > 0 Then
                            oCurrentRule.Definition.IDReasons = (From s As Integer In roTypes.Any2String(tbAutomaticValidationReason.Value).Split(","c)).ToList
                        Else
                            bInclompleteData = True
                        End If

                        oCurrentRule.Definition.MaxDays = roTypes.Any2Integer(Me.txtAutomaticValidationDays.Value)
                        oCurrentRule.Definition.EmployeeValidation = True
                        oCurrentRule.Activated = True
                        oCurrentRule.Definition.TypePlannedDay = If(Me.cmbAutomaticDayType.SelectedItem.Value = 0, eTypePlannedDay.NaturalDay, eTypePlannedDay.LaboralDay)

                        For Each oRule In LabAgreeRequestValidationValuesData()
                            'Buscamos si existe otra regla para el mismo tipo de solicitud de tipo automatica
                            If oRule.IDRequestType = oCurrentRule.IDRequestType AndAlso oRule.IDRuleType <> 4 AndAlso oRule.IDRule <> oCurrentRule.IDRule AndAlso
                                oRule.Definition.EmployeeValidation = False AndAlso oRule.Activated Then

                                If oRule.Definition.IDReasons.Intersect(oCurrentRule.Definition.IDReasons).Count > 0 Then
                                    oUsedInRules.Add(oRule.Name)
                                End If
                            End If
                        Next
                    Case eRequestRuleType.LimitDateRequested
                        If tbLimitDateRequested.Tokens.Count > 0 Then
                            oCurrentRule.Definition.IDReasons = (From s As Integer In roTypes.Any2String(tbLimitDateRequested.Value).Split(","c)).ToList
                        Else
                            bInclompleteData = True
                        End If
                        oCurrentRule.Definition.MaxDays = roTypes.Any2Integer(Me.txtLimitDateRequestedValue.Value)
                        oCurrentRule.Definition.BeginPeriod = Nothing
                        oCurrentRule.Definition.EndPeriod = Nothing

                        If Me.opVisibilityAll.Checked Then
                            oCurrentRule.Definition.TypeCriteriaEmployee = eTypeEmployeeCriteria.AllEmployees
                            oCurrentRule.RuleConditions = New List(Of roUserFieldCondition)
                        Else
                            oCurrentRule.Definition.TypeCriteriaEmployee = eTypeEmployeeCriteria.FilteredEmployees
                            If visibilityCriteria.IsValidFilter Then
                                Dim xList As New Generic.List(Of roUserFieldCondition)
                                xList.Add(visibilityCriteria.OConditionValue)
                                oCurrentRule.RuleConditions = xList
                            Else
                                rError = New roJSON.JSONError(True, Me.Language.Translate("Error.NoCorrectVisibilityFilter", DefaultScope))
                            End If
                        End If
                    Case eRequestRuleType.MinimumDaysBeforeRequestedDate 'MinDaysbefore
                        If tbMinDayCause.Tokens.Count > 0 Then
                            oCurrentRule.Definition.IDReasons = (From s As Integer In roTypes.Any2String(tbMinDayCause.Value).Split(","c)).ToList
                        Else
                            bInclompleteData = True
                        End If
                        'oCurrentRule.Definition.IDReason = Me.cmbMaxDayCause.SelectedItem.Value
                        oCurrentRule.Definition.MaxDays = roTypes.Any2Integer(Me.txtMinDaysValue.Value)
                        oCurrentRule.Definition.TypePlannedDay = If(Me.cmbMinDaysType.SelectedItem.Value = 0, eTypePlannedDay.NaturalDay, eTypePlannedDay.LaboralDay)

                        oCurrentRule.Definition.BeginPeriod = Nothing
                        oCurrentRule.Definition.EndPeriod = Nothing

                    Case eRequestRuleType.MaxNotScheduledDays 'MaxNotScheduledDays
                        If tbMaxNotScheduledDays.Tokens.Count > 0 Then
                            oCurrentRule.Definition.IDReasons = (From s As Integer In roTypes.Any2String(tbMaxNotScheduledDays.Value).Split(","c)).ToList
                        Else
                            bInclompleteData = True
                        End If

                    Case eRequestRuleType.MinCoverageRequiered 'MinCoverageRequiered
                        If tbMinCoverageRequiered.Tokens.Count > 0 Then
                            oCurrentRule.Definition.IDReasons = (From s As Integer In roTypes.Any2String(tbMinCoverageRequiered.Value).Split(","c)).ToList
                        Else
                            bInclompleteData = True
                        End If
                    Case eRequestRuleType.AutomaticRejection 'AutomaticRejection
                        If tbAutomaticRejectionReason.Tokens.Count > 0 Then
                            oCurrentRule.Definition.IDReasons = (From s As Integer In roTypes.Any2String(tbAutomaticRejectionReason.Value).Split(","c)).ToList
                        Else
                            bInclompleteData = True
                        End If

                        oCurrentRule.Definition.MaxDays = roTypes.Any2Integer(Me.txtAutomaticRejectionDays.Value)
                        oCurrentRule.Activated = True
                    Case eRequestRuleType.ScheduleRuleToValidate
                        If tbPlanificationRulesShifts.Tokens.Count > 0 Then
                            oCurrentRule.Definition.IDReasons = (From s As Integer In roTypes.Any2String(tbPlanificationRulesShifts.Value).Split(","c)).ToList
                        Else
                            bInclompleteData = True
                        End If

                        If tbPlanificationRules.Tokens.Count > 0 Then
                            oCurrentRule.Definition.IDPlanificationRules = (From s As Integer In roTypes.Any2String(tbPlanificationRules.Value).Split(","c)).ToList
                        Else
                            bInclompleteData = True
                        End If
                End Select
            End If

            'modificar el startup value de la variable de sesion
            If rError.Error = False AndAlso bInclompleteData Then
                rError = New roJSON.JSONError(True, Me.Language.Translate("Validation.FieldsEmpty", Me.DefaultScope))
            Else
                For Each oRule In LabAgreeRequestValidationValuesData()
                    ' Check for duplicate or overlapping rules
                    If oRule.IDRequestType = oCurrentRule.IDRequestType AndAlso oRule.IDRuleType = oCurrentRule.IDRuleType AndAlso oRule.IDRule <> oCurrentRule.IDRule Then
                        Dim intersectReasons = oRule.Definition.IDReasons.Intersect(oCurrentRule.Definition.IDReasons).ToList()
                        Dim hasAllInCurrent = oCurrentRule.Definition.IDReasons.Contains(-1)
                        Dim hasAllInExisting = oRule.Definition.IDReasons.Contains(-1)

                        ' If either rule has -1 (all), or there is an intersection, it's a conflict
                        If hasAllInCurrent OrElse hasAllInExisting OrElse intersectReasons.Count > 0 Then
                            Dim errMsg As String = Me.Language.Translate("Validation.AutomaticReasonsIntersect", Me.DefaultScope)

                            Dim tmpTokenBox As DevExpress.Web.ASPxTokenBox = Nothing

                            If Me.cmbRuleType.SelectedItem IsNot Nothing Then
                                Select Case Me.cmbRuleType.SelectedItem.Value
                                    Case eRequestRuleType.NegativeAccrual 'PositiveAccrual
                                        tmpTokenBox = tbPositiveAccrualReason
                                    Case eRequestRuleType.MaxNumberDays 'MaxNumberDays
                                        tmpTokenBox = tbMaxDayCause
                                    Case eRequestRuleType.MinimumConsecutiveDays 'MinimumConsecutiveDays
                                        tmpTokenBox = tbMinConsecutiveDayCause
                                    Case eRequestRuleType.PeriodEnjoyment 'PeriodEnjoyment
                                        tmpTokenBox = tbAppliPeriodReason
                                    Case eRequestRuleType.AutomaticValidation 'AutomaticValidation
                                        tmpTokenBox = tbAutomaticValidationReason
                                    Case eRequestRuleType.LimitDateRequested
                                        tmpTokenBox = tbLimitDateRequested
                                    Case eRequestRuleType.MinimumDaysBeforeRequestedDate 'Min Days before
                                        tmpTokenBox = tbMinDayCause
                                    Case eRequestRuleType.MaxNotScheduledDays 'MaxNotScheduledDays
                                        tmpTokenBox = tbMaxNotScheduledDays
                                    Case eRequestRuleType.MinCoverageRequiered 'MinCoverageRequiered
                                        tmpTokenBox = tbMinCoverageRequiered
                                    Case eRequestRuleType.AutomaticRejection 'AutomaticRejection
                                        tmpTokenBox = tbAutomaticRejectionReason
                                    Case eRequestRuleType.ScheduleRuleToValidate
                                        tmpTokenBox = tbPlanificationRules
                                End Select
                            End If

                            If tmpTokenBox IsNot Nothing Then
                                If hasAllInCurrent OrElse hasAllInExisting Then
                                    Dim sItem = tmpTokenBox.Items.FindByValue("-1")
                                    If Not errMsg.EndsWith(":") Then errMsg &= ","
                                    If sItem IsNot Nothing Then
                                        errMsg &= sItem.Text
                                    Else
                                        errMsg &= "-1"
                                    End If
                                Else
                                    For Each oId In intersectReasons
                                        Dim sItem = tmpTokenBox.Items.FindByValue(roTypes.Any2String(oId))
                                        If Not errMsg.EndsWith(":") Then errMsg &= ","
                                        If sItem IsNot Nothing Then
                                            errMsg &= sItem.Text
                                        End If
                                    Next
                                End If
                            End If

                            rError = New roJSON.JSONError(True, errMsg)
                        End If
                    End If
                Next
            End If

            If rError.Error = False Then
                setRequestValidation(oCurrentRule, bolIsNew)
            End If
        Catch ex As Exception
            rError = New roJSON.JSONError(True, ex.Message.ToString & " " & ex.StackTrace)
        Finally
            If rError.Error = False Then
                ASPxRequestValidationCallbackPanelContenido.JSProperties.Add("cpIsNewRO", True)
                ASPxRequestValidationCallbackPanelContenido.JSProperties.Add("cpActionRO", "SAVELABAGREEREQUESTVALIDATION")
                ASPxRequestValidationCallbackPanelContenido.JSProperties.Add("cpNameRO", oCurrentRule.Name)
                ASPxRequestValidationCallbackPanelContenido.JSProperties.Add("cpUsedInRules", oUsedInRules.ToArray)
                ASPxRequestValidationCallbackPanelContenido.JSProperties.Add("cpResultRO", "OK")
            Else
                LoadLabAgreeRequestValidation(oParameters, oCurrentRule)
                ASPxRequestValidationCallbackPanelContenido.JSProperties("cpActionRO") = "SAVELABAGREEREQUESTVALIDATION"
                ASPxRequestValidationCallbackPanelContenido.JSProperties("cpUsedInRules") = oUsedInRules.ToArray
                ASPxRequestValidationCallbackPanelContenido.JSProperties("cpResultRO") = "KO"
                ASPxRequestValidationCallbackPanelContenido.JSProperties.Add("cpErrorRO", rError)
            End If
        End Try
    End Sub

    Private Sub GetRulesForAutomaticValidation(ByVal oParameters As ObjectCallbackRequest)
        Dim rError As New roJSON.JSONError(False, "")
        Dim oUsedInRules As New Generic.List(Of String)

        Try

            Dim reasons As Integer() = (From s As Integer In roTypes.Any2String(oParameters.Tokens).Split(","c)).ToArray

            For Each oRule In LabAgreeRequestValidationValuesData()
                'Buscamos si existe otra regla para el mismo tipo de solicitud de tipo automatica
                If oRule.IDRequestType = oParameters.IDRequestType AndAlso oRule.IDRuleType <> 4 AndAlso oRule.IDRule <> oParameters.ID AndAlso
                    oRule.Definition.EmployeeValidation = False AndAlso oRule.Activated Then

                    If oRule.Definition.IDReasons.Intersect(reasons).Count > 0 Then
                        oUsedInRules.Add(oRule.Name)
                    End If
                End If
            Next
        Catch ex As Exception
            rError = New roJSON.JSONError(True, ex.Message.ToString & " " & ex.StackTrace)
        Finally
            If rError.Error = False Then
                ASPxRequestValidationCallbackContenido.JSProperties.Add("cpIsNewRO", True)
                ASPxRequestValidationCallbackContenido.JSProperties.Add("cpActionRO", "CHECKAUTOMATICRULES")
                ASPxRequestValidationCallbackContenido.JSProperties.Add("cpUsedInRules", oUsedInRules.ToArray)
                ASPxRequestValidationCallbackContenido.JSProperties.Add("cpResultRO", "OK")
            Else
                ASPxRequestValidationCallbackContenido.JSProperties.Add("cpActionRO", "CHECKAUTOMATICRULES")
                ASPxRequestValidationCallbackContenido.JSProperties.Add("cpResultRO", "KO")
                ASPxRequestValidationCallbackContenido.JSProperties.Add("cpUsedInRules", {})
                ASPxRequestValidationCallbackContenido.JSProperties.Add("cpErrorRO", rError)
            End If
        End Try
    End Sub
        Public ReadOnly Property PlanificationRulesShiftsItems As List(Of Integer)
        Get
            Return (From s As Integer In roTypes.Any2String(tbPlanificationRulesShifts.Value).Split(","c)).ToList
        End Get
    End Property

End Class