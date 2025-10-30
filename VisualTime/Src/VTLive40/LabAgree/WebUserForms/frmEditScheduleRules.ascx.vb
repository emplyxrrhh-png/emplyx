Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class WebUserForms_frmEditScheduleRules
    Inherits UserControlBase

    Public Enum WorkingMode
        LabAgree = 0
        Contract = 1
        Query = 2
    End Enum

    <Runtime.Serialization.DataContract()>
    Private Class ObjectCallbackRequest

        <Runtime.Serialization.DataMember(Name:="aTab")>
        Public aTab As Integer

        <Runtime.Serialization.DataMember(Name:="ID")>
        Public ID As Integer

        <Runtime.Serialization.DataMember(Name:="IDLabAgree")>
        Public IDLabAgree As String

        <Runtime.Serialization.DataMember(Name:="Name")>
        Public Name As String

        <Runtime.Serialization.DataMember(Name:="action")>
        Public Action As String

        <Runtime.Serialization.DataMember(Name:="StampParam")>
        Public StampParam As Double

        <Runtime.Serialization.DataMember(Name:="idtype")>
        Public idtype As String

    End Class

#Region "Helper private methods"

    Public Property Mode As WorkingMode
        Get
            Return roTypes.Any2Integer(ViewState("EditScheduleRules_WorkingMode"))
        End Get
        Set(value As WorkingMode)
            ViewState("EditScheduleRules_WorkingMode") = CInt(value)
        End Set
    End Property

    Public Property AvailableScheduleRulesShifts(Optional ByVal bolReload As Boolean = False) As DataTable
        Get
            Dim oLst = ViewState("EditScheduleRules_AvailableShifts")

            If oLst Is Nothing OrElse bolReload Then
                oLst = API.ShiftServiceMethods.GetShifts(Me.Page)
                ViewState("EditScheduleRules_AvailableShifts") = oLst
            End If
            Return oLst
        End Get
        Set(value As DataTable)
            ViewState("EditScheduleRules_AvailableShifts") = value
        End Set
    End Property

    Public Property AvailableScheduleRules(Optional ByVal bolReload As Boolean = False) As Generic.List(Of Integer)
        Get
            Dim oLst = ViewState("EditScheduleRules_Available")

            If oLst Is Nothing OrElse bolReload Then
                oLst = ScheduleRulesServiceMethods.GetUserScheduleRulesTypes(Me.Page).ToList()
                ViewState("EditScheduleRules_Available") = oLst
            End If
            Return oLst
        End Get
        Set(value As Generic.List(Of Integer))
            ViewState("EditScheduleRules_WorkingMode") = value
        End Set
    End Property

    Private Property ScheduleRulesValuesData(Optional ByVal bolReload As Boolean = False) As Generic.List(Of roScheduleRule)
        Get

            Dim tbValues As Generic.List(Of roScheduleRule) = Nothing

            If Me.Mode = WorkingMode.LabAgree Then
                tbValues = Session("LabAgree_LabAgreedScheduleRules")
            ElseIf Me.Mode = WorkingMode.Contract Then
                tbValues = Session("ContractLabAgree_LabAgreedScheduleRules")
            ElseIf Me.Mode = WorkingMode.Query Then
                tbValues = Session("Query_EmployeeScheduleRules")
            End If

            If bolReload Or tbValues Is Nothing Then

                Dim oList As New Generic.List(Of roScheduleRule)

                If Me.Mode = WorkingMode.LabAgree Then
                    If roTypes.Any2Integer(Session("LabAgree_SelectedID")) > -1 Then
                        oList = ScheduleRulesServiceMethods.GetLabAgreeScheduleRules(Me.Page, roTypes.Any2Integer(Session("LabAgree_SelectedID"))).ToList
                    End If
                ElseIf Me.Mode = WorkingMode.Contract Then
                    If roTypes.Any2String(Session("Contract_SelectedID")) <> String.Empty Then
                        oList = ScheduleRulesServiceMethods.GetContractScheduleRules(Me.Page, roTypes.Any2String(Session("Contract_SelectedID"))).ToList
                    End If
                ElseIf Me.Mode = WorkingMode.Query Then
                    If roTypes.Any2String(Session("Contract_SelectedID")) <> String.Empty Then
                        oList = ScheduleRulesServiceMethods.GetEmployeeCurrentScheduleRules(Me.Page, roTypes.Any2String(Session("Contract_SelectedID"))).ToList
                    End If
                End If

                tbValues = oList

                If Me.Mode = WorkingMode.LabAgree Then
                    Session("LabAgree_LabAgreedScheduleRules") = tbValues
                ElseIf Me.Mode = WorkingMode.Contract Then
                    Session("ContractLabAgree_LabAgreedScheduleRules") = tbValues
                ElseIf Me.Mode = WorkingMode.Query Then
                    Session("Query_EmployeeScheduleRules") = tbValues
                End If

            End If
            Return tbValues

        End Get
        Set(ByVal value As Generic.List(Of roScheduleRule))
            If Me.Mode = WorkingMode.LabAgree Then
                If value IsNot Nothing Then
                    Session("LabAgree_LabAgreedScheduleRules") = value
                Else
                    Session("LabAgree_LabAgreedScheduleRules") = Nothing
                End If
            ElseIf Me.Mode = WorkingMode.Contract Then
                If value IsNot Nothing Then
                    Session("ContractLabAgree_LabAgreedScheduleRules") = value
                Else
                    Session("ContractLabAgree_LabAgreedScheduleRules") = Nothing
                End If
            ElseIf Me.Mode = WorkingMode.Query Then
                If value IsNot Nothing Then
                    Session("Query_EmployeeScheduleRules") = value
                Else
                    Session("Query_EmployeeScheduleRules") = Nothing
                End If
            End If
        End Set
    End Property

    Private Function getMinStartupID() As Integer
        Dim newId As Integer = 0

        Dim oList As Generic.List(Of roScheduleRule) = ScheduleRulesValuesData()

        If oList IsNot Nothing AndAlso oList.Count > 0 Then
            Dim index As Integer = 0
            For Each oElem As roScheduleRule In oList
                If index = 0 Then
                    newId = oElem.Id
                Else
                    If oElem.Id < newId Then
                        newId = oElem.Id
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

    Private Function getScheduleRule(ByVal oId As Integer) As roScheduleRule
        Dim oObject As roScheduleRule = Nothing

        Dim oList As Generic.List(Of roScheduleRule) = ScheduleRulesValuesData()

        If oList IsNot Nothing AndAlso oList.Count > 0 Then
            For Each oElem As roScheduleRule In oList
                If oElem.Id = oId Then
                    oObject = roSupport.DeepClone(Of roScheduleRule)(oElem)

                    Exit For
                End If
            Next
        End If

        Return oObject
    End Function

    Private Function setScheduleRule(ByVal oObject As roScheduleRule, ByVal bIsNew As Boolean) As Boolean
        Dim bResult As Boolean = False

        Dim oList As Generic.List(Of roScheduleRule) = ScheduleRulesValuesData()

        If Not bIsNew Then
            Dim index As Integer = 0
            If oList IsNot Nothing AndAlso oList.Count > 0 Then
                For Each oElem As roScheduleRule In oList
                    If oElem.Id = oObject.Id Then
                        oList(index) = oObject
                        bResult = True
                        ScheduleRulesValuesData = oList
                        Exit For
                    End If
                    index = index + 1
                Next
            End If
        Else
            If oList Is Nothing Then oList = New Generic.List(Of roScheduleRule)
            oList.Add(oObject)
            ScheduleRulesValuesData = oList
        End If

        Return bResult
    End Function

#End Region

    Public Sub New()

    End Sub

    Private Sub Page_Load1(sender As Object, e As EventArgs) Handles Me.Load

        If Not IsPostBack Then
            LoadDefaultCombos()
            Dim oLst = AvailableScheduleRules(True)
        End If
    End Sub

    Private Sub LoadDefaultCombos()
        Me.cmbScheduleRuleType.Items.Clear()
        Me.cmbScheduleRuleType.ValueType = GetType(Integer)

        For Each oVal As Integer In System.Enum.GetValues(GetType(ScheduleRuleType))
            If AvailableScheduleRules.Contains(oVal) Then
                If oVal <> 7 Then Me.cmbScheduleRuleType.Items.Add(Me.Language.Translate("SchedulerRule.Name." & System.Enum.GetName(GetType(ScheduleRuleType), oVal), Me.DefaultScope), oVal)
            End If
        Next

        Me.tbOneShiftOneDayValidateComparison.Items.Clear()
        Me.tbPostOneShiftOneDayValidateComparison.Items.Clear()
        Me.tbOneShiftOneDayAvailableShifts.Items.Clear()
        Me.tbMinMaxShiftsSequence.Items.Clear()

        If AvailableScheduleRulesShifts IsNot Nothing Then
            For Each oVal As DataRow In AvailableScheduleRulesShifts.Rows
                Me.tbOneShiftOneDayAvailableShifts.Items.Add(oVal("Name"), oVal("ID"))
                Me.tbOneShiftOneDayValidateComparison.Items.Add(oVal("Name"), oVal("ID"))
                Me.tbPostOneShiftOneDayValidateComparison.Items.Add(oVal("Name"), oVal("ID"))
                Me.tbMinMaxShiftsInPeriod.Items.Add(oVal("Name"), oVal("ID"))
                Me.tbTwoShiftSequenceBeforeDay.Items.Add(oVal("Name"), oVal("ID"))
                Me.tbTwoShiftSequenceCurrentDay.Items.Add(oVal("Name"), oVal("ID"))
                Me.cmbShiftSequence.Items.Add(oVal("Name"), oVal("ID"))
            Next
        End If
        Me.cmbOneShiftOneDayWhen.Items.Clear()
        Me.cmbOneShiftOneDayWhen.ValueType = GetType(Integer)
        Me.cmbOneShiftOneDayWhen.Items.Add(Me.Language.Translate("cmbOneShiftOneDayWhen.Always", Me.DefaultScope), 0)
        Me.cmbOneShiftOneDayWhen.Items.Add(Me.Language.Translate("cmbOneShiftOneDayWhen.DayOfWeek", Me.DefaultScope), 1)
        Me.cmbOneShiftOneDayWhen.Items.Add(Me.Language.Translate("cmbOneShiftOneDayWhen.DayOfMonth", Me.DefaultScope), 2)
        Me.cmbOneShiftOneDayWhen.Items.Add(Me.Language.Translate("cmbOneShiftOneDayWhen.DayOfYear", Me.DefaultScope), 3)

        Me.cmbOneShiftOneDayWhenDayOfWeek.Items.Clear()
        Me.cmbOneShiftOneDayWhenDayOfWeek.ValueType = GetType(Integer)
        For n As Integer = 1 To 7
            Me.cmbOneShiftOneDayWhenDayOfWeek.Items.Add(Me.Language.Keyword("weekday." & n.ToString), If(n = 7, 0, n))
        Next

        Me.cmbLogicOr.Items.Clear()
        Me.cmbLogicOr.ValueType = GetType(Integer)
        Me.cmbLogicOr.Items.Add(Me.Language.Translate("cmbLogicOr.All", Me.DefaultScope), 0)
        Me.cmbLogicOr.Items.Add(Me.Language.Translate("cmbLogicOr.NotAll", Me.DefaultScope), 1)

        Me.cmbOneShiftOneDayValidate.Items.Clear()
        Me.cmbOneShiftOneDayValidate.ValueType = GetType(Integer)
        Me.cmbOneShiftOneDayValidate.Items.Add(Me.Language.Translate("cmbOneShiftOneDayValidate.Equals", Me.DefaultScope), 0)
        Me.cmbOneShiftOneDayValidate.Items.Add(Me.Language.Translate("cmbOneShiftOneDayValidate.Distinct", Me.DefaultScope), 1)

        Me.cmbPostOneShiftOneDayValidate.Items.Clear()
        Me.cmbPostOneShiftOneDayValidate.ValueType = GetType(Integer)
        Me.cmbPostOneShiftOneDayValidate.Items.Add(Me.Language.Translate("cmbOneShiftOneDayValidate.Equals", Me.DefaultScope), 0)
        Me.cmbPostOneShiftOneDayValidate.Items.Add(Me.Language.Translate("cmbOneShiftOneDayValidate.Distinct", Me.DefaultScope), 1)

        Me.cmbMinMaxFreeLabourDaysInPeriodPeriod.Items.Clear()
        Me.cmbMinMaxFreeLabourDaysInPeriodPeriod.ValueType = GetType(Integer)
        Me.cmbMinMaxFreeLabourDaysInPeriodPeriod.Items.Add(Me.Language.Translate("cmbFreeLabourDaysInPeriod.Week", Me.DefaultScope), CInt(ScheduleRuleScope.Week))
        Me.cmbMinMaxFreeLabourDaysInPeriodPeriod.Items.Add(Me.Language.Translate("cmbFreeLabourDaysInPeriod.Month", Me.DefaultScope), CInt(ScheduleRuleScope.Month))
        Me.cmbMinMaxFreeLabourDaysInPeriodPeriod.Items.Add(Me.Language.Translate("cmbFreeLabourDaysInPeriod.Year", Me.DefaultScope), CInt(ScheduleRuleScope.Year))

        Me.cmbMinMaxFreeLabourDaysInPeriodType.Items.Clear()
        Me.cmbMinMaxFreeLabourDaysInPeriodType.ValueType = GetType(Integer)
        Me.cmbMinMaxFreeLabourDaysInPeriodType.Items.Add(Me.Language.Translate("cmbFreeLabourDaysInPeriod.Laborable", Me.DefaultScope), CInt(ScheduleRuleDayType.Laborable))
        Me.cmbMinMaxFreeLabourDaysInPeriodType.Items.Add(Me.Language.Translate("cmbFreeLabourDaysInPeriod.NotLaborable", Me.DefaultScope), CInt(ScheduleRuleDayType.NotLaborable))

        Me.cmbSequence.Items.Clear()
        Me.cmbSequence.ValueType = GetType(Integer)
        Me.cmbSequence.Items.Add(Me.Language.Translate("cmbFreeLabourDaysInPeriod.Laborable", Me.DefaultScope), CInt(ScheduleRuleDayType.Laborable))
        Me.cmbSequence.Items.Add(Me.Language.Translate("cmbFreeLabourDaysInPeriod.NotLaborable", Me.DefaultScope), CInt(ScheduleRuleDayType.NotLaborable))

        Me.cmbTypeSequence.Items.Clear()
        Me.cmbTypeSequence.ValueType = GetType(Integer)
        Me.cmbTypeSequence.Items.Add(Me.Language.Translate("cmbTypeSequence.Shift", Me.DefaultScope), 0)
        Me.cmbTypeSequence.Items.Add(Me.Language.Translate("cmbTypeSequence.Type", Me.DefaultScope), 1)

        Me.cmbMinMaxShiftsInPeriodPeriod.Items.Clear()
        Me.cmbMinMaxShiftsInPeriodPeriod.ValueType = GetType(Integer)
        Me.cmbMinMaxShiftsInPeriodPeriod.Items.Add(Me.Language.Translate("cmbMinMaxShiftsInPeriodPeriod.Week", Me.DefaultScope), CInt(ScheduleRuleScope.Week))
        Me.cmbMinMaxShiftsInPeriodPeriod.Items.Add(Me.Language.Translate("cmbMinMaxShiftsInPeriodPeriod.Month", Me.DefaultScope), CInt(ScheduleRuleScope.Month))
        Me.cmbMinMaxShiftsInPeriodPeriod.Items.Add(Me.Language.Translate("cmbMinMaxShiftsInPeriodPeriod.Year", Me.DefaultScope), CInt(ScheduleRuleScope.Year))
        Me.cmbMinMaxShiftsInPeriodPeriod.Items.Add(Me.Language.Translate("cmbMinMaxShiftsInPeriodPeriod.Selection", Me.DefaultScope), CInt(ScheduleRuleScope.Selection))

        Me.cmbMinMaxShiftsSequence.Items.Clear()
        Me.cmbMinMaxShiftsSequence.ValueType = GetType(Integer)
        Me.cmbMinMaxShiftsSequence.Items.Add(Me.Language.Translate("cmbMinMaxShiftsInPeriodPeriod.Week", Me.DefaultScope), CInt(ScheduleRuleScope.Week))
        Me.cmbMinMaxShiftsSequence.Items.Add(Me.Language.Translate("cmbMinMaxShiftsInPeriodPeriod.Month", Me.DefaultScope), CInt(ScheduleRuleScope.Month))
        Me.cmbMinMaxShiftsSequence.Items.Add(Me.Language.Translate("cmbMinMaxShiftsInPeriodPeriod.Year", Me.DefaultScope), CInt(ScheduleRuleScope.Year))
        Me.cmbMinMaxShiftsSequence.Items.Add(Me.Language.Translate("cmbMinMaxShiftsInPeriodPeriod.Selection", Me.DefaultScope), CInt(ScheduleRuleScope.Selection))

        Me.cmbMinMaxExpectedHoursInPeriodPeriod.Items.Clear()
        Me.cmbMinMaxExpectedHoursInPeriodPeriod.ValueType = GetType(Integer)
        Me.cmbMinMaxExpectedHoursInPeriodPeriod.Items.Add(Me.Language.Translate("cmbFreeDaysInPeriod.Day", Me.DefaultScope), CInt(ScheduleRuleScope.Day))
        Me.cmbMinMaxExpectedHoursInPeriodPeriod.Items.Add(Me.Language.Translate("cmbFreeDaysInPeriod.Week", Me.DefaultScope), CInt(ScheduleRuleScope.Week))
        Me.cmbMinMaxExpectedHoursInPeriodPeriod.Items.Add(Me.Language.Translate("cmbFreeDaysInPeriod.Month", Me.DefaultScope), CInt(ScheduleRuleScope.Month))

        Me.cmbMinWeekendsInPeriod.Items.Clear()
        Me.cmbMinWeekendsInPeriod.ValueType = GetType(Integer)
        Me.cmbMinWeekendsInPeriod.Items.Add(Me.Language.Translate("cmbFreeDaysInPeriod.Week", Me.DefaultScope), CInt(ScheduleRuleScope.Week))
        Me.cmbMinWeekendsInPeriod.Items.Add(Me.Language.Translate("cmbFreeDaysInPeriod.Month", Me.DefaultScope), CInt(ScheduleRuleScope.Month))
        Me.cmbMinWeekendsInPeriod.Items.Add(Me.Language.Translate("cmbFreeDaysInPeriod.Year", Me.DefaultScope), CInt(ScheduleRuleScope.Year))

        cmbUserField.Items.Clear()
        Dim tbUserFields As DataTable = API.UserFieldServiceMethods.GetUserFields(Me.Page, UserFieldsTypes.Types.EmployeeField, "FieldType IN(1) AND Used=1", False)
        For Each oRow As DataRow In tbUserFields.Select("", "FieldName")
            cmbUserField.Items.Add(New DevExpress.Web.ListEditItem(oRow("FieldName"), oRow("FieldName")))
        Next
    End Sub

    Protected Sub ASPxScheduleRulesCallbackPanelContenido_Callback(ByVal sender As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase) Handles ASPxScheduleRulesCallbackPanelContenido.Callback

        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Dim oParameters As New ObjectCallbackRequest()
        oParameters = roJSONHelper.Deserialize(strParameter, oParameters.GetType())

        Dim bRet As Boolean = False

        If oParameters.IDLabAgree IsNot Nothing Then
            If Me.Mode = WorkingMode.LabAgree Then
                Session("LabAgree_SelectedID") = roTypes.Any2Integer(oParameters.IDLabAgree)
            Else
                Session("Contract_SelectedID") = roTypes.Any2String(oParameters.IDLabAgree)
            End If
        End If

        Select Case oParameters.Action
            Case "GETLABAGREERESCHEDULERULE"
                LoadLabAgreeScheduleRule(oParameters)
            Case "SAVELABAGREESCHEDULERULE"
                SaveLabAgreeScheduleRule(oParameters)
            Case "LOADRULECONFIGURATION"
                LoadDefaultRuleConfiguration()
            Case Else
                ASPxScheduleRulesCallbackPanelContenido.JSProperties.Add("cpAction", "UNKNOW")
                ASPxScheduleRulesCallbackPanelContenido.JSProperties.Add("cpResult", "NOK")
                ASPxScheduleRulesCallbackPanelContenido.JSProperties.Add("cpMessage", "Unknow call")
        End Select
    End Sub

    Private Sub LoadDefaultRuleConfiguration(Optional ByVal bResetItem As Boolean = True)
        Dim result As String = "OK"
        Try
            If Me.cmbScheduleRuleType.SelectedItem.Value = ScheduleRuleType.RestBetweenShifts Then
                Me.lblRestBetweenShifsTime.DateTime = CDate("1970/1/1 00:00")

            ElseIf Me.cmbScheduleRuleType.SelectedItem.Value = ScheduleRuleType.MinMaxShiftsSequence Then
                Me.tbMinMaxShiftsSequence.Tokens.Clear()
                Me.cmbTypeSequence.SelectedItem = Me.cmbTypeSequence.Items.FindByValue(0)
                Me.txtTypeSequence.Value = ""
                Me.txtPeriodictySequence.Value = ""
                Me.cmbMinMaxShiftsSequence.SelectedItem = Me.cmbMinMaxShiftsSequence.Items.FindByValue(0)
                Me.ckAlwaysSequence.Checked = False
                Me.txtDateSequenceFrom.Date = Nothing
                Me.txtDateSequenceTo.Date = Nothing
                Me.ckMinShiftsSequenceRepeat.Checked = False
                Me.txtMinShiftsSequenceRepeat.Value = "0"
                Me.ckMaxShiftsSequenceRepeat.Checked = False
                Me.txtMaxShiftsSequenceRepeat.Value = "0"

            ElseIf Me.cmbScheduleRuleType.SelectedItem.Value = ScheduleRuleType.OneShiftOneDay Then
                Me.tbOneShiftOneDayValidateComparison.Tokens.Clear()
                Me.tbOneShiftOneDayAvailableShifts.Tokens.Clear()

                Me.tbPostOneShiftOneDayValidateComparison.Tokens.Clear()

                Me.cmbOneShiftOneDayWhen.SelectedItem = Me.cmbOneShiftOneDayWhen.Items.FindByValue(0)
                Me.cmbOneShiftOneDayWhenDayOfWeek.SelectedItem = Me.cmbOneShiftOneDayWhenDayOfWeek.Items.FindByValue(1)
                Me.cmbOneShiftOneDayValidate.SelectedItem = Me.cmbOneShiftOneDayValidate.Items.FindByValue(0)
                Me.cmbPostOneShiftOneDayValidate.SelectedItem = Me.cmbPostOneShiftOneDayValidate.Items.FindByValue(0)

                Me.tbOneShiftOneDayValidateHours.DateTime = CDate("1970/1/1 00:00")
                Me.rbOneShiftOneDayValidateShift.Checked = True
                Me.txtOneShiftOneDayWhenWeek.Date = DateTime.Now.Date
                Me.txtOneShiftOneDayWhenYear.Date = DateTime.Now.Date
            ElseIf Me.cmbScheduleRuleType.SelectedItem.Value = ScheduleRuleType.MinMaxFreeLabourDaysInPeriod Then
                Me.cmbMinMaxFreeLabourDaysInPeriodPeriod.SelectedItem = Me.cmbMinMaxFreeLabourDaysInPeriodPeriod.Items.FindByValue(CInt(ScheduleRuleScope.Week))
                Me.cmbMinMaxFreeLabourDaysInPeriodType.SelectedItem = Me.cmbMinMaxFreeLabourDaysInPeriodType.Items.FindByValue(CInt(ScheduleRuleDayType.Laborable))
                Me.tbMinMaxShiftsInPeriod.Tokens.Clear()
                Me.ckMinFreeLabourDaysInPeriodRepeat.Checked = False
                Me.ckMaxFreeLabourDaysInPeriodRepeat.Checked = False
                Me.txtMaxFreeLabourDaysInPeriodRepeat.Value = "0"
                Me.txtMinFreeLabourDaysInPeriodRepeat.Value = "0"
            ElseIf Me.cmbScheduleRuleType.SelectedItem.Value = ScheduleRuleType.MinMaxShiftsInPeriod Then
                Me.txtDateFrom.Value = Nothing
                Me.txtDateTo.Value = Nothing
                Me.ckAlways.Checked = False
                Me.cmbMinMaxShiftsInPeriodPeriod.SelectedItem = Me.cmbMinMaxShiftsInPeriodPeriod.Items.FindByValue(CInt(ScheduleRuleScope.Week))
                Me.cmbLogicOr.SelectedItem = Me.cmbLogicOr.Items.FindByValue(0)
                Me.tbMinMaxShiftsInPeriod.Tokens.Clear()
                Me.ckMinShiftsInPeriodRepeat.Checked = False
                Me.ckMaxShiftsInPeriodRepeat.Checked = False
                Me.txtMaxShiftsInPeriodRepeat.Value = "0"
                Me.txtMinShiftsInPeriodRepeat.Value = "0"
            ElseIf Me.cmbScheduleRuleType.SelectedItem.Value = ScheduleRuleType.MinWeekendsInPeriod Then
                Me.txtMinWeekendsInPeriod.Value = 0
                Me.cmbMinWeekendsInPeriod.SelectedItem = Me.cmbMinMaxFreeLabourDaysInPeriodPeriod.Items.FindByValue(CInt(ScheduleRuleScope.Week))
            ElseIf Me.cmbScheduleRuleType.SelectedItem.Value = ScheduleRuleType.TwoShiftSequence Then
            ElseIf Me.cmbScheduleRuleType.SelectedItem.Value = ScheduleRuleType.MaxNotScheduled Then
                Me.txtMaxNotScheduled.Value = 0

            ElseIf Me.cmbScheduleRuleType.SelectedItem.Value = ScheduleRuleType.MinMaxDaysSequence Then
                Me.txtSequence.Value = 0
                Me.cmbSequence.SelectedItem = Me.cmbSequence.Items.FindByValue(CInt(ScheduleRuleDayType.Laborable))

            ElseIf Me.cmbScheduleRuleType.SelectedItem.Value = ScheduleRuleType.MinMaxExpectedHoursInPeriod Then
                cmbUserField.Items.Clear()
                Dim tbUserFields As DataTable = API.UserFieldServiceMethods.GetUserFields(Me.Page, UserFieldsTypes.Types.EmployeeField, "FieldType IN(1) AND Used=1", False)
                For Each oRow As DataRow In tbUserFields.Select("", "FieldName")
                    cmbUserField.Items.Add(New DevExpress.Web.ListEditItem(oRow("FieldName"), oRow("FieldName")))
                Next
                Me.cmbMinMaxExpectedHoursInPeriodPeriod.SelectedItem = Me.cmbMinMaxExpectedHoursInPeriodPeriod.Items.FindByValue(CInt(ScheduleRuleScope.Week))
                Me.ckMinExpectedHoursInPeriodRepeat.Checked = False
                Me.ckMaxExpectedHoursInPeriodRepeat.Checked = False
                Me.txtMaxExpectedHoursInPeriodRepeat.Value = "0"
                Me.txtMinExpectedHoursInPeriodRepeat.Value = "0"
            End If
        Catch ex As Exception
            result = "KO"
        End Try

    End Sub

    Private Sub LoadLabAgreeScheduleRule(ByVal oParameters As ObjectCallbackRequest, Optional ByVal eStartupValue As roScheduleRule = Nothing)
        Dim oCurrentRule As roScheduleRule = Nothing
        Dim result As String = "OK"
        Try

            If eStartupValue IsNot Nothing Then
                oCurrentRule = eStartupValue
            Else
                If oParameters.ID = "-1" Then
                    oCurrentRule = New roScheduleRule
                    oCurrentRule.Id = getMinStartupID()
                    If Me.Mode = WorkingMode.LabAgree Then
                        oCurrentRule.IdLabAgree = oParameters.IDLabAgree
                        oCurrentRule.IdContract = String.Empty
                    ElseIf Me.Mode = WorkingMode.Contract Then
                        oCurrentRule.IdLabAgree = 0
                        oCurrentRule.IdContract = oParameters.IDLabAgree
                    End If

                    If Me.Mode <> WorkingMode.Query Then
                        oCurrentRule.RuleName = ""
                        oCurrentRule.RuleDescription = Me.txtDescription.Text
                        oCurrentRule.Enabled = True

                        Me.cmbScheduleRuleType.SelectedItem = Me.cmbScheduleRuleType.Items.FindByValue(CInt(oCurrentRule.IDRule))

                        LoadDefaultRuleConfiguration()
                    End If
                Else
                    oCurrentRule = getScheduleRule(oParameters.ID)
                    Me.cmbScheduleRuleType.SelectedItem = Me.cmbScheduleRuleType.Items.FindByValue(CInt(oCurrentRule.IDRule))
                    Me.cmbScheduleRuleType.ClientEnabled = False
                End If
            End If

            If oCurrentRule Is Nothing Then Exit Sub

            Me.txtName.Text = oCurrentRule.RuleName
            Me.txtDescription.Text = oCurrentRule.RuleDescription
            Me.ckRuleActive.Checked = oCurrentRule.Enabled

            Select Case oCurrentRule.IDRule
                Case ScheduleRuleType.RestBetweenShifts
                    LoadRestBetweenShiftsRule(oCurrentRule)
                Case ScheduleRuleType.OneShiftOneDay
                    LoadOneShiftOneDayRule(oCurrentRule)
                Case ScheduleRuleType.MinMaxFreeLabourDaysInPeriod
                    LoadFreeLabourDaysInPeriod(oCurrentRule)
                Case ScheduleRuleType.MinMaxDaysSequence
                    LoadMinMaxDaysSequence(oCurrentRule)
                Case ScheduleRuleType.MinMaxShiftsInPeriod
                    LoadMinMaxShiftsInPeriod(oCurrentRule)
                Case ScheduleRuleType.MinWeekendsInPeriod
                    LoadMinWeekendsInPeriod(oCurrentRule)
                Case ScheduleRuleType.TwoShiftSequence
                    LoadTwoShiftSequence(oCurrentRule)
                Case ScheduleRuleType.MaxNotScheduled
                    LoadMaxNotScheduled(oCurrentRule)
                Case ScheduleRuleType.MinMaxExpectedHoursInPeriod
                    LoadMinMaxExpectedHoursInPeriod(oCurrentRule)
                Case ScheduleRuleType.MinMaxShiftsSequence
                    LoadMinMaxShiftsSequence(oCurrentRule)
            End Select
        Catch ex As Exception
            result = "KO"
        Finally
            ASPxScheduleRulesCallbackPanelContenido.JSProperties.Add("cpActionRO", "GETLABAGREERESCHEDULERULE")
            ASPxScheduleRulesCallbackPanelContenido.JSProperties.Add("cpNameRO", oCurrentRule.RuleName)
            ASPxScheduleRulesCallbackPanelContenido.JSProperties.Add("cpResultRO", result)
            ASPxScheduleRulesCallbackPanelContenido.JSProperties.Add("cpIsNewRO", False)
            ASPxScheduleRulesCallbackPanelContenido.JSProperties.Add("cpScope", oCurrentRule.Scope)
        End Try

    End Sub

    Private Sub SaveLabAgreeScheduleRule(ByVal oParameters As ObjectCallbackRequest)
        Dim rError As New roJSON.JSONError(False, "")
        Dim oCurrentRule As roScheduleRule = Nothing
        Dim bInclompleteData As Boolean = False

        Try
            Dim bolIsNew As Boolean = False
            If oParameters.ID = "-1" Then bolIsNew = True

            If bolIsNew Then

                Select Case Me.cmbScheduleRuleType.SelectedItem.Value
                    Case ScheduleRuleType.RestBetweenShifts
                        oCurrentRule = New roScheduleRule_RestBetweenShifts
                    Case ScheduleRuleType.OneShiftOneDay
                        oCurrentRule = New roScheduleRule_OneShiftOneDay
                    Case ScheduleRuleType.MinMaxFreeLabourDaysInPeriod
                        oCurrentRule = New roScheduleRule_MinMaxFreeLabourDaysInPeriod
                    Case ScheduleRuleType.MinMaxShiftsInPeriod
                        oCurrentRule = New roScheduleRule_MinMaxShiftsInPeriod
                    Case ScheduleRuleType.MinWeekendsInPeriod
                        oCurrentRule = New roScheduleRule_MinWeekendsInPeriod
                    Case ScheduleRuleType.TwoShiftSequence
                        oCurrentRule = New roScheduleRule_2ShiftSequence
                    Case ScheduleRuleType.MaxNotScheduled
                        oCurrentRule = New roScheduleRule_MaxNotScheduled
                    Case ScheduleRuleType.MinMaxDaysSequence
                        oCurrentRule = New roScheduleRule_MinMaxDaysSequence
                    Case ScheduleRuleType.MinMaxExpectedHoursInPeriod
                        oCurrentRule = New roScheduleRule_MinMaxExpectedHoursInPeriod
                    Case ScheduleRuleType.MinMaxShiftsSequence
                        oCurrentRule = New roScheduleRule_MinMaxShiftsSequence
                    Case ScheduleRuleType.Custom
                        oCurrentRule = New roScheduleRule_Custom
                    Case Else
                        oCurrentRule = New roScheduleRule
                End Select

                If Me.Mode = WorkingMode.LabAgree Then
                    oCurrentRule.IdLabAgree = Session("LabAgree_SelectedID")
                    oCurrentRule.IdContract = String.Empty
                Else
                    oCurrentRule.IdLabAgree = 0
                    oCurrentRule.IdContract = Session("Contract_SelectedID")
                End If

                oCurrentRule.Id = getMinStartupID()
            Else
                oCurrentRule = getScheduleRule(oParameters.ID)
            End If

            If oCurrentRule Is Nothing Then Exit Sub

            oCurrentRule.RuleName = Me.txtName.Text
            oCurrentRule.RuleDescription = Me.txtDescription.Text
            oCurrentRule.Enabled = Me.ckRuleActive.Checked
            oCurrentRule.IDRule = Me.cmbScheduleRuleType.SelectedItem.Value
            oCurrentRule.RuleType = ScheduleRuleBaseType.User

            Select Case oCurrentRule.IDRule
                Case ScheduleRuleType.RestBetweenShifts
                    bInclompleteData = SaveRestBetweenShiftsRule(oCurrentRule)
                Case ScheduleRuleType.OneShiftOneDay
                    bInclompleteData = SaveOneShiftOneDayRule(oCurrentRule)
                Case ScheduleRuleType.MinMaxFreeLabourDaysInPeriod
                    bInclompleteData = SaveFreeLabourDaysInPeriod(oCurrentRule)
                Case ScheduleRuleType.MinMaxShiftsInPeriod
                    bInclompleteData = SaveMinMaxShiftsInPeriod(oCurrentRule)
                Case ScheduleRuleType.MinWeekendsInPeriod
                    bInclompleteData = SaveMinWeekendsInPeriod(oCurrentRule)
                Case ScheduleRuleType.TwoShiftSequence
                    bInclompleteData = SaveTwoShiftSequence(oCurrentRule)
                Case ScheduleRuleType.MaxNotScheduled
                    bInclompleteData = SaveMaxNotScheduled(oCurrentRule)
                Case ScheduleRuleType.MinMaxDaysSequence
                    bInclompleteData = SaveMinMaxDaysSequence(oCurrentRule)
                Case ScheduleRuleType.MinMaxShiftsSequence
                    bInclompleteData = SaveMinMaxShiftsSequence(oCurrentRule)
                Case ScheduleRuleType.MinMaxExpectedHoursInPeriod
                    bInclompleteData = SaveMinMaxExpectedHoursInPeriod(oCurrentRule)
            End Select

            'modificar el startup value de la variable de sesion
            If bInclompleteData Then
                rError = New roJSON.JSONError(True, Me.Language.Translate("Validation.FieldsEmpty", Me.DefaultScope))
            End If

            If rError.Error = False Then
                setScheduleRule(oCurrentRule, bolIsNew)
            End If
        Catch ex As Exception
            rError = New roJSON.JSONError(True, ex.Message.ToString & " " & ex.StackTrace)
        Finally

            If rError.Error = False Then
                ASPxScheduleRulesCallbackPanelContenido.JSProperties.Add("cpActionRO", "SAVELABAGREESCHEDULERULE")
                ASPxScheduleRulesCallbackPanelContenido.JSProperties.Add("cpNameRO", oCurrentRule.RuleName)

                ASPxScheduleRulesCallbackPanelContenido.JSProperties("cpIsNewRO") = True
                ASPxScheduleRulesCallbackPanelContenido.JSProperties.Add("cpResultRO", "OK")
            Else
                LoadLabAgreeScheduleRule(oParameters, oCurrentRule)

                ASPxScheduleRulesCallbackPanelContenido.JSProperties("cpActionRO") = "SAVELABAGREESCHEDULERULE"
                ASPxScheduleRulesCallbackPanelContenido.JSProperties("cpNameRO") = oCurrentRule.RuleName
                ASPxScheduleRulesCallbackPanelContenido.JSProperties("cpResultRO") = "KO"
                ASPxScheduleRulesCallbackPanelContenido.JSProperties.Add("cpErrorRO", rError)
            End If
        End Try
    End Sub

#Region "MaxNotScheduled"

    Private Sub LoadMaxNotScheduled(oCurrentRule As roScheduleRule)
        Dim oRule As roScheduleRule_MaxNotScheduled = CType(oCurrentRule, roScheduleRule_MaxNotScheduled)

        txtMaxNotScheduled.Value = String.Join(",", oRule.MaximumNotScheduledDays)

    End Sub

    Private Function SaveMaxNotScheduled(ByRef oCurrentRule As roScheduleRule) As Boolean
        Dim bIncomplete As Boolean = False

        Dim oRule As roScheduleRule_MaxNotScheduled = CType(oCurrentRule, roScheduleRule_MaxNotScheduled)
        oRule.MaximumNotScheduledDays = txtMaxNotScheduled.Value

        Return bIncomplete
    End Function

#End Region

#Region "TwoShiftSequence"

    Private Sub LoadTwoShiftSequence(oCurrentRule As roScheduleRule)
        Dim oRule As roScheduleRule_2ShiftSequence = CType(oCurrentRule, roScheduleRule_2ShiftSequence)

        tbTwoShiftSequenceCurrentDay.Value = String.Join(",", oRule.CurrentDayShifts)
        tbTwoShiftSequenceBeforeDay.Value = String.Join(",", oRule.PreviousDayShifts)

    End Sub

    Private Function SaveTwoShiftSequence(ByRef oCurrentRule As roScheduleRule) As Boolean
        Dim bIncomplete As Boolean = False

        Dim oRule As roScheduleRule_2ShiftSequence = CType(oCurrentRule, roScheduleRule_2ShiftSequence)

        If tbTwoShiftSequenceCurrentDay.Tokens.Count > 0 Then
            oRule.CurrentDayShifts = (From s As Integer In roTypes.Any2String(tbTwoShiftSequenceCurrentDay.Value).Split(","c)).ToArray
        Else
            bIncomplete = True
        End If

        If tbTwoShiftSequenceBeforeDay.Tokens.Count > 0 Then
            oRule.PreviousDayShifts = (From s As Integer In roTypes.Any2String(tbTwoShiftSequenceBeforeDay.Value).Split(","c)).ToArray
        Else
            bIncomplete = True
        End If

        oRule.Type = ShiftSequenceType.Unwanted
        oRule.Scope = ScheduleRuleScope.Always

        Return bIncomplete
    End Function

#End Region

#Region "MinWeekendsInPeriod"

    Private Sub LoadMinMaxDaysSequence(oCurrentRule As roScheduleRule)
        Dim oRule As roScheduleRule_MinMaxDaysSequence = CType(oCurrentRule, roScheduleRule_MinMaxDaysSequence)

        Me.txtSequence.Value = oRule.MaximumDays
        Me.cmbSequence.SelectedItem = Me.cmbSequence.Items.FindByValue(CInt(oRule.DaysType))

    End Sub

    Private Function SaveMinMaxDaysSequence(ByRef oCurrentRule As roScheduleRule) As Boolean
        Dim bIncomplete As Boolean = False

        Dim oRule As roScheduleRule_MinMaxDaysSequence = CType(oCurrentRule, roScheduleRule_MinMaxDaysSequence)

        oRule.MinimumDays = -1
        oRule.MaximumDays = Me.txtSequence.Value
        oRule.DaysType = Me.cmbSequence.SelectedItem.Value

        Return bIncomplete
    End Function

#End Region

#Region "MinWeekendsInPeriod"

    Private Sub LoadMinWeekendsInPeriod(oCurrentRule As roScheduleRule)
        Dim oRule As roScheduleRule_MinWeekendsInPeriod = CType(oCurrentRule, roScheduleRule_MinWeekendsInPeriod)

        Me.txtMinWeekendsInPeriod.Value = oRule.Minimum
        Me.cmbMinWeekendsInPeriod.SelectedItem = Me.cmbMinMaxFreeLabourDaysInPeriodPeriod.Items.FindByValue(CInt(oRule.Scope))

    End Sub

    Private Function SaveMinWeekendsInPeriod(ByRef oCurrentRule As roScheduleRule) As Boolean
        Dim bIncomplete As Boolean = False

        Dim oRule As roScheduleRule_MinWeekendsInPeriod = CType(oCurrentRule, roScheduleRule_MinWeekendsInPeriod)

        oRule.Minimum = Me.txtMinWeekendsInPeriod.Value
        oRule.Scope = Me.cmbMinWeekendsInPeriod.SelectedItem.Value

        Return bIncomplete
    End Function

#End Region

#Region "MinMaxShiftsInPeriod"

    Private Sub LoadMinMaxShiftsInPeriod(oCurrentRule As roScheduleRule)
        Dim oRule As roScheduleRule_MinMaxShiftsInPeriod = CType(oCurrentRule, roScheduleRule_MinMaxShiftsInPeriod)

        Me.cmbMinMaxShiftsInPeriodPeriod.SelectedItem = Me.cmbMinMaxShiftsInPeriodPeriod.Items.FindByValue(CInt(oRule.Scope))

        If oRule.LogicOr = False Then
            Me.cmbLogicOr.SelectedItem = Me.cmbLogicOr.Items.FindByValue(0)
        Else
            Me.cmbLogicOr.SelectedItem = Me.cmbLogicOr.Items.FindByValue(1)
        End If

        Me.txtPeriodicty.Value = oRule.ScopePeriods

        If oRule.BeginPeriod.Year > 1970 AndAlso oRule.EndPeriod.Year > 1970 Then
            Me.txtDateFrom.Value = oRule.BeginPeriod
            Me.txtDateTo.Value = oRule.EndPeriod
            Me.ckAlways.Checked = True
        End If

        Me.tbMinMaxShiftsInPeriod.Value = String.Join(",", oRule.CurrentDayShifts)
        If oRule.Maximum > -1 Then
            Me.ckMaxShiftsInPeriodRepeat.Checked = True
            Me.txtMaxShiftsInPeriodRepeat.Value = oRule.Maximum
        Else
            Me.ckMaxShiftsInPeriodRepeat.Checked = False
            Me.txtMaxShiftsInPeriodRepeat.Value = 0
        End If

        If oRule.Minimum > -1 Then
            Me.ckMinShiftsInPeriodRepeat.Checked = True
            Me.txtMinShiftsInPeriodRepeat.Value = oRule.Minimum
        Else
            Me.ckMinShiftsInPeriodRepeat.Checked = False
            Me.txtMinShiftsInPeriodRepeat.Value = 0
        End If

    End Sub

    Private Function SaveMinMaxShiftsInPeriod(ByRef oCurrentRule As roScheduleRule) As Boolean
        Dim bIncomplete As Boolean = False

        Dim oRule As roScheduleRule_MinMaxShiftsInPeriod = CType(oCurrentRule, roScheduleRule_MinMaxShiftsInPeriod)

        oRule.Scope = Me.cmbMinMaxShiftsInPeriodPeriod.SelectedItem.Value
        oRule.ScopePeriods = roTypes.Any2Integer(Me.txtPeriodicty.Value)
        oRule.LogicOr = roTypes.Any2Boolean(Me.cmbLogicOr.SelectedItem.Value)
        If oRule.Scope = 4 Then
            oRule.BeginPeriod = Me.txtDateFrom.Value
            oRule.EndPeriod = Me.txtDateTo.Value
            oRule.ScopePeriods = 1
        Else
            If Me.ckAlways.Checked = True Then
                oRule.BeginPeriod = Me.txtDateFrom.Value
                oRule.EndPeriod = Me.txtDateTo.Value
            End If
        End If

        If tbMinMaxShiftsInPeriod.Tokens.Count > 0 Then
            oRule.CurrentDayShifts = (From s As Integer In roTypes.Any2String(tbMinMaxShiftsInPeriod.Value).Split(","c)).ToArray
        Else
            bIncomplete = True
        End If

        If Me.ckMaxShiftsInPeriodRepeat.Checked Then
            oRule.Maximum = Me.txtMaxShiftsInPeriodRepeat.Value
        Else
            oRule.Maximum = -1
        End If

        If Me.ckMinShiftsInPeriodRepeat.Checked Then
            oRule.Minimum = Me.txtMinShiftsInPeriodRepeat.Value
        Else
            oRule.Minimum = -1
        End If

        Return bIncomplete
    End Function

#End Region

#Region "Sequence Shifts"

    Private Sub LoadMinMaxShiftsSequence(oCurrentRule As roScheduleRule)
        Dim oRule As roScheduleRule_MinMaxShiftsSequence = CType(oCurrentRule, roScheduleRule_MinMaxShiftsSequence)

        Me.tbMinMaxShiftsSequence.Tokens.Clear()

        Me.cmbMinMaxShiftsSequence.SelectedItem = Me.cmbMinMaxShiftsSequence.Items.FindByValue(CInt(oRule.Scope))

        Me.txtPeriodictySequence.Value = oRule.ScopePeriods

        If oRule.BeginPeriod.Year > 1970 AndAlso oRule.EndPeriod.Year > 1970 Then
            Me.txtDateSequenceFrom.Value = oRule.BeginPeriod
            Me.txtDateSequenceTo.Value = oRule.EndPeriod
            Me.ckAlwaysSequence.Checked = True
        End If

        Dim contador = 1

        For Each shift In oRule.Shifts

            Dim token As String

            If shift.StartsWith("SCHTYPE_") Then

                token = shift.Replace("SCHTYPE_", "")

                Me.tbMinMaxShiftsSequence.Tokens.Add(contador.ToString + "-" + "*" & token & "*")
            Else
                Me.tbMinMaxShiftsSequence.Tokens.Add(contador.ToString + "-" + API.ShiftServiceMethods.GetShift(Me.Page, Convert.ToInt32(shift), False).Name)
            End If
            contador = contador + 1
        Next

        ' Me.tbMinMaxShiftsSequence.Value = String.Join(",", oRule.CurrentDayShifts)

        If oRule.Maximum > -1 Then
            Me.ckMaxShiftsSequenceRepeat.Checked = True
            Me.txtMaxShiftsSequenceRepeat.Value = oRule.Maximum
        Else
            Me.ckMaxShiftsSequenceRepeat.Checked = False
            Me.txtMaxShiftsSequenceRepeat.Value = 0
        End If

        If oRule.Minimum > -1 Then
            Me.ckMinShiftsSequenceRepeat.Checked = True
            Me.txtMinShiftsSequenceRepeat.Value = oRule.Minimum
        Else
            Me.ckMinShiftsSequenceRepeat.Checked = False
            Me.txtMinShiftsSequenceRepeat.Value = 0
        End If
    End Sub

    Private Function SaveMinMaxShiftsSequence(ByRef oCurrentRule As roScheduleRule) As Boolean
        Dim bIncomplete As Boolean = False

        Dim oRule As roScheduleRule_MinMaxShiftsSequence = CType(oCurrentRule, roScheduleRule_MinMaxShiftsSequence)

        oRule.Scope = Me.cmbMinMaxShiftsSequence.SelectedItem.Value
        oRule.ScopePeriods = roTypes.Any2Integer(Me.txtPeriodictySequence.Value)
        If oRule.Scope = 4 Then
            oRule.BeginPeriod = Me.txtDateSequenceFrom.Value
            oRule.EndPeriod = Me.txtDateSequenceTo.Value
            oRule.ScopePeriods = 1
        Else
            If Me.ckAlwaysSequence.Checked = True Then
                oRule.BeginPeriod = Me.txtDateSequenceFrom.Value
                oRule.EndPeriod = Me.txtDateSequenceTo.Value
            Else
                oRule.BeginPeriod = DateSerial(1970, 1, 1)
                oRule.EndPeriod = DateSerial(1970, 1, 1)
            End If
        End If

        If tbMinMaxShiftsSequence.Tokens.Count > 0 Then

            Dim tokenBegin = 0
            Dim tokenEnd = 0

            Dim tokens As List(Of String) = New List(Of String)
            For Each token In tbMinMaxShiftsSequence.Tokens

                tokenBegin = token.IndexOf("-") + 1
                tokenEnd = token.Length - tokenBegin

                token = token.Substring(tokenBegin, tokenEnd)

                If token.StartsWith("*") AndAlso token.EndsWith("*") Then
                    tokens.Add("SCHTYPE_" & token.Substring(1, token.Length - 2))
                Else
                    tokens.Add(Me.cmbShiftSequence.Items.FindByText(token).Value)
                End If

            Next
            oRule.Shifts = tokens.ToArray
        Else
            bIncomplete = True
        End If

        If Me.ckMaxShiftsSequenceRepeat.Checked Then
            oRule.Maximum = Me.txtMaxShiftsSequenceRepeat.Value
        Else
            oRule.Maximum = -1
        End If

        If Me.ckMinShiftsSequenceRepeat.Checked Then
            oRule.Minimum = Me.txtMinShiftsSequenceRepeat.Value
        Else
            oRule.Minimum = -1
        End If

        Return bIncomplete
    End Function

#End Region

#Region "MinMaxExpectedHoursInPeriod"

    Private Sub LoadMinMaxExpectedHoursInPeriod(oCurrentRule As roScheduleRule)
        Dim oRule As roScheduleRule_MinMaxExpectedHoursInPeriod = CType(oCurrentRule, roScheduleRule_MinMaxExpectedHoursInPeriod)

        Me.cmbMinMaxExpectedHoursInPeriodPeriod.SelectedItem = Me.cmbMinMaxExpectedHoursInPeriodPeriod.Items.FindByValue(CInt(oRule.Scope))

        If oRule.MaximumWorkingHours > -1 Then
            Me.ckMaxExpectedHoursInPeriodRepeat.Checked = True
            Me.txtMaxExpectedHoursInPeriodRepeat.Value = oRule.MaximumWorkingHours
        Else
            If CType(oCurrentRule, roScheduleRule_MinMaxExpectedHoursInPeriod).MaximumEmployeeField IsNot String.Empty Then
                Dim item = Me.cmbUserField.Items.FindByText(CType(oCurrentRule, roScheduleRule_MinMaxExpectedHoursInPeriod).MaximumEmployeeField)
                Me.cmbUserField.SelectedItem = item
                Me.ckUserField.Checked = True
            Else
                Me.ckMaxExpectedHoursInPeriodRepeat.Checked = False
                Me.txtMaxExpectedHoursInPeriodRepeat.Value = 0
            End If

        End If

        If oRule.MinimumWorkingHours > -1 Then
            Me.ckMinExpectedHoursInPeriodRepeat.Checked = True
            Me.txtMinExpectedHoursInPeriodRepeat.Value = oRule.MinimumWorkingHours
        Else
            Me.ckMinExpectedHoursInPeriodRepeat.Checked = False
            Me.txtMinExpectedHoursInPeriodRepeat.Value = 0
        End If

    End Sub

    Private Function SaveMinMaxExpectedHoursInPeriod(ByRef oCurrentRule As roScheduleRule) As Boolean
        Dim bIncomplete As Boolean = False

        Dim oRule As roScheduleRule_MinMaxExpectedHoursInPeriod = CType(oCurrentRule, roScheduleRule_MinMaxExpectedHoursInPeriod)

        oRule.Scope = Me.cmbMinMaxExpectedHoursInPeriodPeriod.SelectedItem.Value

        If Me.ckMaxExpectedHoursInPeriodRepeat.Checked Then
            oRule.MaximumWorkingHours = Me.txtMaxExpectedHoursInPeriodRepeat.Value
            oRule.MaximumEmployeeField = ""
        Else
            oRule.MaximumWorkingHours = -1
            If Me.ckUserField.Checked Then
                oRule.MaximumEmployeeField = If(Me.cmbUserField.SelectedIndex = -1, "", roTypes.Any2String(Me.cmbUserField.SelectedItem.Value))
            End If

        End If

        If Me.ckMinExpectedHoursInPeriodRepeat.Checked Then
            oRule.MinimumWorkingHours = Me.txtMinExpectedHoursInPeriodRepeat.Value
        Else
            oRule.MinimumWorkingHours = -1
        End If

        Return bIncomplete
    End Function

#End Region

#Region "FreeLabourDaysInPeriod"

    Private Sub LoadFreeLabourDaysInPeriod(oCurrentRule As roScheduleRule)
        Dim oRule As roScheduleRule_MinMaxFreeLabourDaysInPeriod = CType(oCurrentRule, roScheduleRule_MinMaxFreeLabourDaysInPeriod)

        Me.cmbMinMaxFreeLabourDaysInPeriodPeriod.SelectedItem = Me.cmbMinMaxFreeLabourDaysInPeriodPeriod.Items.FindByValue(CInt(oRule.Scope))
        Me.cmbMinMaxFreeLabourDaysInPeriodType.SelectedItem = Me.cmbMinMaxFreeLabourDaysInPeriodType.Items.FindByValue(CInt(oRule.DaysType))

        If oRule.DaysType = DaysType.Free Then

            Me.cmbMinMaxFreeLabourDaysInPeriodType.SelectedItem = cmbMinMaxFreeLabourDaysInPeriodType.Items.FindByValue(CInt(oRule.DaysType))
            If oRule.MaximumRestDays > -1 Then
                Me.ckMaxFreeLabourDaysInPeriodRepeat.Checked = True
                Me.txtMaxFreeLabourDaysInPeriodRepeat.Value = oRule.MaximumRestDays
            Else
                Me.ckMaxFreeLabourDaysInPeriodRepeat.Checked = False
                Me.txtMaxFreeLabourDaysInPeriodRepeat.Value = 0
            End If

            If oRule.MinimumRestDays > -1 Then
                Me.ckMinFreeLabourDaysInPeriodRepeat.Checked = True
                Me.txtMinFreeLabourDaysInPeriodRepeat.Value = oRule.MinimumRestDays
            Else
                Me.ckMinFreeLabourDaysInPeriodRepeat.Checked = False
                Me.txtMinFreeLabourDaysInPeriodRepeat.Value = 0
            End If
        Else

            Me.cmbMinMaxFreeLabourDaysInPeriodType.SelectedItem = cmbMinMaxFreeLabourDaysInPeriodType.Items.FindByValue(CInt(oRule.DaysType))

            If oRule.MaximumLabourDays > -1 Then
                Me.ckMaxFreeLabourDaysInPeriodRepeat.Checked = True
                Me.txtMaxFreeLabourDaysInPeriodRepeat.Value = oRule.MaximumLabourDays
            Else
                Me.ckMaxFreeLabourDaysInPeriodRepeat.Checked = False
                Me.txtMaxFreeLabourDaysInPeriodRepeat.Value = 0
            End If

            If oRule.MinimumLabourDays > -1 Then
                Me.ckMinFreeLabourDaysInPeriodRepeat.Checked = True
                Me.txtMinFreeLabourDaysInPeriodRepeat.Value = oRule.MinimumLabourDays
            Else
                Me.ckMinFreeLabourDaysInPeriodRepeat.Checked = False
                Me.txtMinFreeLabourDaysInPeriodRepeat.Value = 0
            End If
        End If

    End Sub

    Private Function SaveFreeLabourDaysInPeriod(ByRef oCurrentRule As roScheduleRule) As Boolean
        Dim bIncomplete As Boolean = False

        Dim oRule As roScheduleRule_MinMaxFreeLabourDaysInPeriod = CType(oCurrentRule, roScheduleRule_MinMaxFreeLabourDaysInPeriod)

        oRule.Scope = Me.cmbMinMaxFreeLabourDaysInPeriodPeriod.SelectedItem.Value

        If Me.ckMaxFreeLabourDaysInPeriodRepeat.Checked = False AndAlso Me.ckMinFreeLabourDaysInPeriodRepeat.Checked = False Then
            bIncomplete = True
        End If

        If roTypes.Any2Integer(Me.txtMaxFreeLabourDaysInPeriodRepeat.Value) < roTypes.Any2Integer(Me.txtMinFreeLabourDaysInPeriodRepeat.Value) Then
            If ckMaxFreeLabourDaysInPeriodRepeat.Checked = True AndAlso ckMinFreeLabourDaysInPeriodRepeat.Checked = True Then
                bIncomplete = True
            End If
        End If

        If Me.cmbMinMaxFreeLabourDaysInPeriodType.SelectedItem.Value = 1 Then
            oRule.DaysType = DaysType.Labour

            If Me.ckMaxFreeLabourDaysInPeriodRepeat.Checked Then
                oRule.MaximumLabourDays = Me.txtMaxFreeLabourDaysInPeriodRepeat.Value
                oRule.MaximumRestDays = -1
            Else
                oRule.MaximumLabourDays = -1
            End If

            If Me.ckMinFreeLabourDaysInPeriodRepeat.Checked Then
                oRule.MinimumLabourDays = Me.txtMinFreeLabourDaysInPeriodRepeat.Value
                oRule.MinimumRestDays = -1
            Else
                oRule.MinimumLabourDays = -1
            End If
        Else

            oRule.DaysType = DaysType.Free

            If Me.ckMaxFreeLabourDaysInPeriodRepeat.Checked Then
                oRule.MaximumRestDays = Me.txtMaxFreeLabourDaysInPeriodRepeat.Value
                oRule.MaximumLabourDays = -1
            Else
                oRule.MaximumRestDays = -1
            End If

            If Me.ckMinFreeLabourDaysInPeriodRepeat.Checked Then
                oRule.MinimumRestDays = Me.txtMinFreeLabourDaysInPeriodRepeat.Value
                oRule.MinimumLabourDays = -1
            Else
                oRule.MinimumRestDays = -1
            End If

        End If

        Return bIncomplete
    End Function

#End Region

#Region "RestBetweenShifts"

    Private Sub LoadRestBetweenShiftsRule(oCurrentRule As roScheduleRule)
        Me.lblRestBetweenShifsTime.DateTime = CDate("1970/1/1 " & Format(roTypes.Any2Time(CType(oCurrentRule, roScheduleRule_RestBetweenShifts).RestHours).TimeOnly, "HH:mm"))
    End Sub

    Private Function SaveRestBetweenShiftsRule(ByRef oCurrentRule As roScheduleRule) As Boolean
        Dim bIncomplete As Boolean = False
        CType(oCurrentRule, roScheduleRule_RestBetweenShifts).RestHours = roTypes.Any2Time(Format(lblRestBetweenShifsTime.DateTime, HelperWeb.GetShortTimeFormat())).NumericValue()

        Return bIncomplete
    End Function

#End Region

#Region "OneShiftOneDay"

    Private Sub LoadOneShiftOneDayRule(oCurrentRule As roScheduleRule)
        Dim oRule As roScheduleRule_OneShiftOneDay = CType(oCurrentRule, roScheduleRule_OneShiftOneDay)

        tbOneShiftOneDayAvailableShifts.Value = String.Join(",", oRule.CurrentDayShifts)

        Select Case oRule.Scope
            Case ScheduleRuleScope.Always
                cmbOneShiftOneDayWhen.SelectedItem = cmbOneShiftOneDayWhen.Items.FindByValue(0)
            Case ScheduleRuleScope.Week
                cmbOneShiftOneDayWhen.SelectedItem = cmbOneShiftOneDayWhen.Items.FindByValue(1)
                cmbOneShiftOneDayWhenDayOfWeek.SelectedItem = cmbOneShiftOneDayWhenDayOfWeek.Items.FindByValue(CInt(oRule.ReferenceDate.DayOfWeek))
            Case ScheduleRuleScope.Month
                cmbOneShiftOneDayWhen.SelectedItem = cmbOneShiftOneDayWhen.Items.FindByValue(2)
                txtOneShiftOneDayWhenWeek.Date = oRule.ReferenceDate
            Case ScheduleRuleScope.Year
                cmbOneShiftOneDayWhen.SelectedItem = cmbOneShiftOneDayWhen.Items.FindByValue(3)
                txtOneShiftOneDayWhenYear.Date = oRule.ReferenceDate
        End Select

        Select Case oRule.Type
            Case ShiftSequenceType.Wanted
                cmbOneShiftOneDayValidate.SelectedItem = cmbOneShiftOneDayValidate.Items.FindByValue(0)
            Case ShiftSequenceType.Unwanted
                cmbOneShiftOneDayValidate.SelectedItem = cmbOneShiftOneDayValidate.Items.FindByValue(1)
        End Select

        Select Case oRule.TypeNextDays
            Case ShiftSequenceType.Wanted
                cmbPostOneShiftOneDayValidate.SelectedItem = cmbPostOneShiftOneDayValidate.Items.FindByValue(0)
            Case ShiftSequenceType.Unwanted
                cmbPostOneShiftOneDayValidate.SelectedItem = cmbPostOneShiftOneDayValidate.Items.FindByValue(1)
        End Select

        If oRule.RestHours = -1 Then
            rbOneShiftOneDayValidateHours.Checked = False
            Me.tbOneShiftOneDayValidateHours.DateTime = CDate("1970/1/1 00:00")
        Else
            rbOneShiftOneDayValidateHours.Checked = True
            Me.tbOneShiftOneDayValidateHours.DateTime = CDate("1970/1/1 " & Format(roTypes.Any2Time(oRule.RestHours).TimeOnly, "HH:mm"))
        End If

        If oRule.PreviousDayShifts.Length = 0 Then
            rbOneShiftOneDayValidateShift.Checked = False
            tbOneShiftOneDayValidateComparison.Tokens.Clear()
        Else
            rbOneShiftOneDayValidateShift.Checked = True
            tbOneShiftOneDayValidateComparison.Value = String.Join(",", oRule.PreviousDayShifts)
        End If

        If oRule.NextDayShifts.Length = 0 Then
            rbPostOneShiftOneDayValidateShift.Checked = False
            tbPostOneShiftOneDayValidateComparison.Tokens.Clear()
        Else
            rbPostOneShiftOneDayValidateShift.Checked = True
            tbPostOneShiftOneDayValidateComparison.Value = String.Join(",", oRule.NextDayShifts)
        End If

    End Sub

    Private Function SaveOneShiftOneDayRule(ByRef oCurrentRule As roScheduleRule) As Boolean
        Dim bIncomplete As Boolean = False

        Dim oRule As roScheduleRule_OneShiftOneDay = CType(oCurrentRule, roScheduleRule_OneShiftOneDay)
        If tbOneShiftOneDayAvailableShifts.Tokens.Count > 0 Then
            oRule.CurrentDayShifts = (From s As Integer In roTypes.Any2String(tbOneShiftOneDayAvailableShifts.Value).Split(","c)).ToArray
        Else
            bIncomplete = True
        End If

        Select Case cmbOneShiftOneDayWhen.SelectedItem.Value
            Case 0
                oRule.Scope = ScheduleRuleScope.Always
            Case 1
                oRule.Scope = ScheduleRuleScope.Week

                Dim tmpDate As DateTime = DateTime.Now.Date
                While tmpDate.DayOfWeek <> cmbOneShiftOneDayWhenDayOfWeek.SelectedItem.Value
                    tmpDate = tmpDate.AddDays(1)
                End While
                oRule.ReferenceDate = tmpDate

            Case 2
                oRule.ReferenceDate = txtOneShiftOneDayWhenWeek.Date
                oRule.Scope = ScheduleRuleScope.Month
            Case 3
                oRule.ReferenceDate = txtOneShiftOneDayWhenYear.Date
                oRule.Scope = ScheduleRuleScope.Year

        End Select

        Select Case cmbOneShiftOneDayValidate.SelectedItem.Value
            Case 0
                oRule.Type = ShiftSequenceType.Wanted
            Case 1
                oRule.Type = ShiftSequenceType.Unwanted
        End Select

        Select Case cmbPostOneShiftOneDayValidate.SelectedItem.Value
            Case 0
                oRule.TypeNextDays = ShiftSequenceType.Wanted
            Case 1
                oRule.TypeNextDays = ShiftSequenceType.Unwanted
        End Select

        If rbOneShiftOneDayValidateHours.Checked Then
            oRule.RestHours = roTypes.Any2Time(Format(tbOneShiftOneDayValidateHours.DateTime, HelperWeb.GetShortTimeFormat())).NumericValue()
        Else
            oRule.RestHours = -1
        End If

        If rbOneShiftOneDayValidateShift.Checked Then
            If tbOneShiftOneDayValidateComparison.Tokens.Count > 0 Then
                oRule.PreviousDayShifts = (From s As Integer In roTypes.Any2String(tbOneShiftOneDayValidateComparison.Value).Split(","c)).ToArray
            Else
                bIncomplete = True
            End If
        Else
            oRule.PreviousDayShifts = {}
        End If

        If Me.rbPostOneShiftOneDayValidateShift.Checked Then
            If tbPostOneShiftOneDayValidateComparison.Tokens.Count > 0 Then
                oRule.NextDayShifts = (From s As Integer In roTypes.Any2String(tbPostOneShiftOneDayValidateComparison.Value).Split(","c)).ToArray
            Else
                bIncomplete = True
            End If
        Else
            oRule.NextDayShifts = {}
        End If

        Return bIncomplete
    End Function

#End Region

End Class