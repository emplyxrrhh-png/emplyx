Imports DevExpress.Web
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTEmployees.Contract
Imports Robotics.Base.VTEmployees.LabAgree
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class WebUserControls_ContractScheduleRules
    Inherits UserControlBase

    <Runtime.Serialization.DataContract()>
    Private Class CallbackRequest

        <Runtime.Serialization.DataMember(Name:="aTab")>
        Public aTab As Integer

        <Runtime.Serialization.DataMember(Name:="ID")>
        Public ID As String

        <Runtime.Serialization.DataMember(Name:="IDLabAgree")>
        Public IDLabAgree As String

        <Runtime.Serialization.DataMember(Name:="action")>
        Public Action As String

        <Runtime.Serialization.DataMember(Name:="StampParam")>
        Public StampParam As Double

        <Runtime.Serialization.DataMember(Name:="resultClientAction")>
        Public resultClientAction As String

        <Runtime.Serialization.DataMember(Name:="Telecommuting")>
        Public Telecommuting As Boolean

        <Runtime.Serialization.DataMember(Name:="TelecommutingMandatoryDays")>
        Public TelecommutingMandatoryDays As String

        <Runtime.Serialization.DataMember(Name:="PresenceMandatoryDays")>
        Public PresenceMandatoryDays As String

        <Runtime.Serialization.DataMember(Name:="TelecommutingMaxDays")>
        Public TelecommutingMaxDays As Integer

        <Runtime.Serialization.DataMember(Name:="TelecommutingMaxPercentage")>
        Public TelecommutingMaxPercentage As Integer

        <Runtime.Serialization.DataMember(Name:="TelecommutingPeriodType")>
        Public TelecommutingPeriodType As Integer

        <Runtime.Serialization.DataMember(Name:="TelecommutingOptionalDays")>
        Public TelecommutingOptionalDays As String

        <Runtime.Serialization.DataMember(Name:="TelecommutingAgreementStart")>
        Public TelecommutingAgreementStart As String

        <Runtime.Serialization.DataMember(Name:="TelecommutingAgreementEnd")>
        Public TelecommutingAgreementEnd As String

        <Runtime.Serialization.DataMember(Name:="TelecommutingPercentage")>
        Public TelecommutingPercentage As Integer

    End Class

    <Serializable()>
    <Runtime.Serialization.DataContract()>
    Private Class WeekDayInfo

        <Runtime.Serialization.DataMember(Name:="ID")>
        Public ID As Integer

        <Runtime.Serialization.DataMember(Name:="Name")>
        Public Name As String

        <Runtime.Serialization.DataMember(Name:="IdWeek")>
        Public IdWeek As Integer

        Public Sub New()

        End Sub

    End Class

    Private Const PRESENCE_ID As String = "0"
    Private Const TELECOMMUTING_ID As String = "1"
    Private Const OPTIONAL_ID As String = "2"

    Private Property ListTelecommutinOptionsData(Optional ByVal bolReload As Boolean = False, Optional ByVal bolCheckStatus As Boolean = False) As DataTable
        Get
            Dim tb As DataTable = Session("LabAgrre_TelecommutingOptions")

            If bolReload OrElse tb Is Nothing Then

                tb = New DataTable()
                tb.Columns.Add("ID")
                tb.Columns.Add("Name")
                tb.Columns.Add("ImageSrc")
                Dim dr As DataRow = tb.NewRow()

                dr("ID") = PRESENCE_ID
                dr("Name") = Me.Language.Translate("Presence", Me.DefaultScope)
                dr("ImageSrc") = "Images/Presence48.png"
                tb.Rows.Add(dr)

                dr = tb.NewRow()
                dr("ID") = TELECOMMUTING_ID
                dr("Name") = Me.Language.Translate("Telecommuting", Me.DefaultScope)
                dr("ImageSrc") = "Images/Telecommuting48.png"
                tb.Rows.Add(dr)

                dr = tb.NewRow()
                dr("ID") = OPTIONAL_ID
                dr("Name") = Me.Language.Translate("Optional", Me.DefaultScope)
                dr("ImageSrc") = "Images/OptionalTelecommuting48.png"
                tb.Rows.Add(dr)

                If tb IsNot Nothing Then
                    tb.PrimaryKey = New DataColumn() {tb.Columns("ID")}
                    tb.AcceptChanges()
                End If

                Session("LabAgrre_TelecommutingOptions") = tb
            End If
            Return tb
        End Get
        Set(value As DataTable)
            Session("LabAgrre_TelecommutingOptions") = value
        End Set
    End Property

    Private Property LaboralDays(Optional ByVal bolReload As Boolean = False) As Generic.List(Of WeekDayInfo)
        Get
            Dim dic = ViewState("ContractLabAgree_WeekDaysDictionary")

            If dic Is Nothing Then
                dic = New Generic.List(Of WeekDayInfo)

                For n As Integer = 1 To 7
                    Dim item As New WeekDayInfo With {
                        .ID = If(n = 7, 0, n),
                        .Name = Me.Language.Keyword("weekday." & n.ToString),
                        .IdWeek = 1
                    }
                    dic.Add(item)
                Next

                ViewState("ContractLabAgree_WeekDaysDictionary") = dic
            End If

            Return dic
        End Get
        Set(value As Generic.List(Of WeekDayInfo))
            ViewState("ContractLabAgree_WeekDaysDictionary") = value
        End Set
    End Property

    Private ReadOnly Property LabAgreeTelecommutingPattern(ByVal idLabAgree As Integer, Optional ByVal bolReload As Boolean = False) As Generic.List(Of roWeekDays)
        Get

            Dim tbValues As Generic.List(Of roWeekDays) = New List(Of roWeekDays)()

            If bolReload Or tbValues Is Nothing Or tbValues.Count = 0 Then

                Dim oList As New Generic.List(Of roWeekDays)

                Dim row As roWeekDays = New roWeekDays()
                Dim oCurrentLabAgree As roLabAgree = API.LabAgreeServiceMethods.GetLabAgreeByID(Nothing, idLabAgree, True)
                If oCurrentLabAgree IsNot Nothing AndAlso oCurrentLabAgree.Telecommuting Then
                    Dim mandatoryDays As String() = oCurrentLabAgree.TelecommutingMandatoryDays.Split(",")
                    Dim presenceMandatoryDays As String() = oCurrentLabAgree.PresenceMandatoryDays.Split(",")
                    Dim optionalDays As String() = oCurrentLabAgree.TelecommutingOptionalDays.Split(",")

                    row.Day1 = GetTelecommutingTypeDay("1", oCurrentLabAgree)
                    row.Day2 = GetTelecommutingTypeDay("2", oCurrentLabAgree)
                    row.Day3 = GetTelecommutingTypeDay("3", oCurrentLabAgree)
                    row.Day4 = GetTelecommutingTypeDay("4", oCurrentLabAgree)
                    row.Day5 = GetTelecommutingTypeDay("5", oCurrentLabAgree)
                    row.Day6 = GetTelecommutingTypeDay("6", oCurrentLabAgree)
                    row.Day0 = GetTelecommutingTypeDay("0", oCurrentLabAgree)
                    row.Week = "1"
                    oList.Add(row)
                    tbValues = oList
                End If
            End If
            Return tbValues

        End Get
    End Property

    Private ReadOnly Property ContractTelecommutingPattern(ByVal idContract As String, Optional ByVal bolReload As Boolean = False) As Generic.List(Of roWeekDays)
        Get

            Dim tbValues As Generic.List(Of roWeekDays) = New List(Of roWeekDays)()

            If bolReload Or tbValues Is Nothing Or tbValues.Count = 0 Then

                Dim oList As New Generic.List(Of roWeekDays)

                Dim row As roWeekDays = New roWeekDays()
                Dim contract = API.ContractsServiceMethods.GetContract(Nothing, idContract, True)
                If contract IsNot Nothing AndAlso contract.Telecommuting Then
                    Dim mandatoryDays As String() = contract.TelecommutingMandatoryDays.Split(",")
                    Dim presenceMandatoryDays As String() = contract.PresenceMandatoryDays.Split(",")
                    Dim optionalDays As String() = contract.TelecommutingOptionalDays.Split(",")

                    row.Day1 = GetTelecommutingTypeDay("1", contract)
                    row.Day2 = GetTelecommutingTypeDay("2", contract)
                    row.Day3 = GetTelecommutingTypeDay("3", contract)
                    row.Day4 = GetTelecommutingTypeDay("4", contract)
                    row.Day5 = GetTelecommutingTypeDay("5", contract)
                    row.Day6 = GetTelecommutingTypeDay("6", contract)
                    row.Day0 = GetTelecommutingTypeDay("0", contract)
                    row.Week = "1"
                    oList.Add(row)
                    tbValues = oList
                End If
            End If
            Return tbValues

        End Get
    End Property

    Private Property LabAgreeScheduleRulesValuesData(Optional ByVal bolReload As Boolean = False) As Generic.List(Of roScheduleRule)
        Get

            Dim tbValues As Generic.List(Of roScheduleRule) = Session("ContractLabAgree_LabAgreedScheduleRules")

            If bolReload Or tbValues Is Nothing Then

                Dim oList As New Generic.List(Of roScheduleRule)

                If roTypes.Any2String(Session("Contract_SelectedID")) <> String.Empty Then
                    oList = ScheduleRulesServiceMethods.GetContractScheduleRules(Me.Page, roTypes.Any2String(Session("Contract_SelectedID"))).ToList
                End If

                tbValues = oList
                Session("ContractLabAgree_LabAgreedScheduleRules") = tbValues
            End If

            tbValues = tbValues.FindAll(Function(x) x.RuleType = ScheduleRuleBaseType.User)

            Return tbValues

        End Get
        Set(ByVal value As Generic.List(Of roScheduleRule))
            If value IsNot Nothing Then
                Session("ContractLabAgree_LabAgreedScheduleRules") = value
            Else
                Session("ContractLabAgree_LabAgreedScheduleRules") = Nothing
            End If
        End Set
    End Property

    Private Property LabAgreeSystemScheduleRulesValuesData(Optional ByVal bolReload As Boolean = False) As Generic.List(Of roScheduleRule)
        Get

            Dim tbValues As Generic.List(Of roScheduleRule) = Session("ContractLabAgree_LabAgreedScheduleRules")

            If bolReload Or tbValues Is Nothing Then

                Dim oList As New Generic.List(Of roScheduleRule)

                If roTypes.Any2String(Session("Contract_SelectedID")) <> String.Empty Then
                    oList = ScheduleRulesServiceMethods.GetContractScheduleRules(Me.Page, roTypes.Any2String(Session("Contract_SelectedID"))).ToList
                End If

                tbValues = oList
                Session("ContractLabAgree_LabAgreedScheduleRules") = tbValues

            End If

            tbValues = tbValues.FindAll(Function(x) x.RuleType = ScheduleRuleBaseType.System)

            Return tbValues

        End Get
        Set(ByVal value As Generic.List(Of roScheduleRule))
            If value IsNot Nothing Then
                Session("ContractLabAgree_LabAgreedScheduleRules") = value
            Else
                Session("ContractLabAgree_LabAgreedScheduleRules") = Nothing
            End If
        End Set
    End Property

    Private Property oPermission As Permission
        Get
            If Session("ContractLabAgree_EmployeePermission") IsNot Nothing Then
                Return CType(Session("ContractLabAgree_EmployeePermission"), Permission)
            Else
                Return Permission.None
            End If
        End Get
        Set(value As Permission)
            Session("ContractLabAgree_EmployeePermission") = value
        End Set
    End Property

    Private Property intEmployeeID As Integer
        Get
            Return roTypes.Any2Integer(Session("ContractLabAgree_EmployeeEdittingId"))
        End Get
        Set(value As Integer)
            Session("ContractLabAgree_EmployeeEdittingId") = value
        End Set
    End Property

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Dim cacheManager As New Robotics.Web.Base.NoCachePageBase

        cacheManager.InsertJavascriptIncludes(Me.Parent.Page)

        cacheManager.InsertExtraJavascript("Ajax", "~/Base/Scripts/Ajax.js", Me.Parent.Page)
        cacheManager.InsertExtraJavascript("jsDate", "~/Base/Scripts/jsDate.js", Me.Parent.Page)

        cacheManager.InsertExtraJavascript("webUserForms", "~/Base/Scripts/webUserForms.js", Me.Parent.Page)
        cacheManager.InsertExtraJavascript("frmEditScheduleRules", "~/LabAgree/Scripts/frmEditScheduleRules.js", Me.Parent.Page)

        cacheManager.InsertExtraJavascript("GenericData", "~/Base/Scripts/GenericData.js", Me.Parent.Page)
        cacheManager.InsertExtraJavascript("roOptionPanelClient", "~/Base/Scripts/roOptionPanelClient.js", Me.Parent.Page)
        cacheManager.InsertExtraJavascript("roOptPanelClientGroup", "~/Base/Scripts/roOptPanelClientGroup.js", Me.Parent.Page)

    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        LoadStaticInfo()
        If Not Me.IsPostBack Then
            CreateScheduleRulesColumns()
        End If
    End Sub

    Public Sub LoadEmployeeData(ByVal idEmploye As Integer)
        Dim EmployeeID As String = roTypes.Any2String(idEmploye)
        If EmployeeID IsNot Nothing AndAlso EmployeeID.Length > 0 Then
            Me.intEmployeeID = CInt(EmployeeID)
        End If

        Me.oPermission = Me.GetFeaturePermissionByEmployee("LabAgree.EmployeeAssign", Me.intEmployeeID)

        CreateScheduleRulesColumns()
    End Sub

    Private Sub LoadStaticInfo()

        If Not HelperSession.GetFeatureIsInstalledFromApplication("Feature\Telecommuting") Then
            Me.divTelecommutingGeneral.Style("display") = "none"
            Me.divSchedulingRules.Style("width") = "100%"
        End If
        Me.ckTelecommuteNo.Value = True
        Me.ckTelecommuteYes.Value = False

        cmbWeekOrMonth.Items.Clear()

        cmbWeekOrMonth.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ContractScheduleRules.Week", DefaultScope), 0))
        cmbWeekOrMonth.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ContractScheduleRules.Month", DefaultScope), 1))
        cmbWeekOrMonth.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ContractScheduleRules.Quarter", DefaultScope), 2))

        cmbDaysOrPercent.Items.Clear()

        cmbDaysOrPercent.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ContractScheduleRules.Days", DefaultScope), 0))
        cmbDaysOrPercent.Items.Add(New DevExpress.Web.ListEditItem("%", 1))

        tbWorkingDays.Items.Clear()
        For Each oDay In LaboralDays
            tbWorkingDays.Items.Add(oDay.Name, oDay.ID)
        Next

    End Sub

    Protected Sub ContractScheduleRulesCallback_Callback(ByVal sender As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase) Handles ContractScheduleRulesCallback.Callback

        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Dim oParameters As New CallbackRequest()
        oParameters = roJSONHelper.Deserialize(strParameter, oParameters.GetType())

        If Me.oPermission > Permission.None Then
            If oParameters.Action = "GETCONTRACTSCHEDULERULES" Then
                Session("Contract_SelectedID") = oParameters.ID
                LoadContractScheduleRules(oParameters)
            ElseIf oParameters.Action = "SAVECONTRACTSCHEDULERULES" Then
                Session("Contract_SelectedID") = oParameters.ID
                SaveContractScheduleRules(oParameters)
            End If
        End If

        Select Case oParameters.aTab
            Case 0
                Me.div00.Style("display") = ""
        End Select

    End Sub

#Region "Methods"

    Private Sub LoadContractScheduleRules(ByVal oParameters As CallbackRequest)
        If Me.oPermission < Permission.Write Then
            Me.DisableControls()
        End If

        Dim result As String = "OK"
        Try
            BindGridContractScheduleRules(True)
            Me.SetLabAgreeRulesData(oParameters.IDLabAgree)
            If HelperSession.GetFeatureIsInstalledFromApplication("Feature\Telecommuting") Then
                Me.GetTelecommutingData(oParameters.ID)
            End If

            If Me.LabAgreeScheduleRulesValuesData.Count > 0 Then
                Me.optOverwriteScheduleRules.Checked = True
            Else
                Me.optOverwriteScheduleRules.Checked = False
            End If
        Catch ex As Exception
            result = "KO"
        Finally
            ContractScheduleRulesCallback.JSProperties.Add("cpActionRO", "GETCONTRACTSCHEDULERULES")
            ContractScheduleRulesCallback.JSProperties.Add("cpResultRO", result)
            ContractScheduleRulesCallback.JSProperties.Add("cpIsNew", False)
        End Try

    End Sub

    Private Sub SaveContractScheduleRules(ByVal oParameters As CallbackRequest)
        Dim strError As String = ""

        Try

            'Check Permissions
            If Me.oPermission < Permission.Write Then
                strError = Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope)
            End If

            If HelperSession.GetFeatureIsInstalledFromApplication("Feature\Telecommuting") Then
                Dim oOldContract As roContract = Nothing
                oOldContract = API.ContractsServiceMethods.GetContract(Nothing, oParameters.ID, False)
                Dim oContract As roContract = Nothing
                oContract = oOldContract

                If Me.optOverwriteTelecommuting.Checked Then
                    oContract.Telecommuting = oParameters.Telecommuting

                    oContract.TelecommutingPeriodType = oParameters.TelecommutingPeriodType
                    oContract.TelecommutingOptionalDays = oParameters.TelecommutingOptionalDays
                    oContract.TelecommutingMandatoryDays = oParameters.TelecommutingMandatoryDays
                    oContract.PresenceMandatoryDays = oParameters.PresenceMandatoryDays
                    oContract.TelecommutingAgreementStart = roTypes.Any2DateTime(oParameters.TelecommutingAgreementStart)
                    oContract.TelecommutingAgreementEnd = roTypes.Any2DateTime(oParameters.TelecommutingAgreementEnd)
                    If Me.cmbDaysOrPercent.SelectedIndex = 1 Then
                        oContract.TelecommutingMaxPercentage = oParameters.TelecommutingPercentage
                        oContract.TelecommutingMaxDays = 0
                    Else
                        oContract.TelecommutingMaxDays = oParameters.TelecommutingMaxDays
                        oContract.TelecommutingMaxPercentage = 0
                    End If
                Else
                    'oContract.Telecommuting = Nothing
                    oContract.TelecommutingAgreementStart = Nothing
                    oContract.TelecommutingAgreementEnd = Nothing
                End If

                Dim TCSaved = API.ContractsServiceMethods.SaveContract(Nothing, oContract, True)

            End If

            If strError = String.Empty Then
                Dim oNewSchedulingRules As roScheduleRule() = ParseClientSchedulerRules(oParameters.resultClientAction)
                Dim bSaved As Boolean = ScheduleRulesServiceMethods.SaveContractScheduleRules(Me.Page, oNewSchedulingRules, oParameters.ID)
            End If
        Catch ex As Exception
            strError = ex.Message
        Finally
            If strError <> String.Empty Then
                LoadContractScheduleRules(oParameters)
                strError = "MESSAGE" &
                            "TitleKey=Error.SaveContractScheduleRules.Title&" +
                            "DescriptionText=" + strError + "&" +
                            "Option1TextKey=Error.SaveContractScheduleRules.Option1Text&" +
                            "Option1DescriptionKey=Error.SaveContractScheduleRules.Option1Description&" +
                            "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                            "IconUrl=" & HelperWeb.MsgBoxIconsUrl(HelperWeb.MsgBoxIcons.ErrorIcon)

                ContractScheduleRulesCallback.JSProperties("cpActionRO") = "SAVECONTRACTSCHEDULERULES"
                ContractScheduleRulesCallback.JSProperties("cpResultRO") = strError
            Else
                ContractScheduleRulesCallback.JSProperties("cpActionRO") = "SAVECONTRACTSCHEDULERULES"
            End If
        End Try

    End Sub

    Protected Sub GetTelecommutingData(ByVal idContract As String)
        Dim contract = API.ContractsServiceMethods.GetContract(Nothing, idContract, True)
        If contract IsNot Nothing Then
            Dim dsTelecommutingOptions = Me.ListTelecommutinOptionsData
            ContractScheduleRulesCallback.JSProperties.Add("cpTelecommutingOptions", roJSONHelper.SerializeNewtonSoft(dsTelecommutingOptions))
            If contract.TelecommutingAgreementStart IsNot Nothing AndAlso contract.TelecommutingAgreementEnd IsNot Nothing Then
                Me.ckTelecommuteYes.Value = contract.Telecommuting
                Me.cmbWeekOrMonth.SelectedIndex = contract.TelecommutingPeriodType
                If contract.TelecommutingMaxPercentage > 0 Then

                    Me.cmbDaysOrPercent.SelectedIndex = 1
                    Me.txtTelecommutingMaxOptional.Value = contract.TelecommutingMaxPercentage
                Else
                    Me.txtTelecommutingMaxOptional.Value = contract.TelecommutingMaxDays
                    Me.cmbDaysOrPercent.SelectedIndex = 0

                End If

                Me.txtCanTelecommuteFrom.Value = contract.TelecommutingAgreementStart
                Me.txtCanTelecommuteTo.Value = contract.TelecommutingAgreementEnd
                Me.optOverwriteTelecommuting.Checked = True
                Dim dsContractTCPattern = Me.ContractTelecommutingPattern(contract.IDContract)
                ContractScheduleRulesCallback.JSProperties.Add("cpTelecommutingPatternResult", roJSONHelper.SerializeNewtonSoft(dsContractTCPattern))
                ContractScheduleRulesCallback.JSProperties.Add("cpTelecommutingMandatoryDays", contract.TelecommutingMandatoryDays)
                ContractScheduleRulesCallback.JSProperties.Add("cpPresenceMandatoryDays", contract.PresenceMandatoryDays)
                ContractScheduleRulesCallback.JSProperties.Add("cpTelecommutingOptionalDays", contract.TelecommutingOptionalDays)
            Else
                If contract.LabAgree IsNot Nothing Then
                    If contract.LabAgree.TelecommutingAgreementStart IsNot Nothing AndAlso contract.LabAgree.TelecommutingAgreementEnd IsNot Nothing Then
                        Me.ckTelecommuteYes.Value = contract.LabAgree.Telecommuting
                        Me.cmbWeekOrMonth.SelectedIndex = contract.LabAgree.TelecommutingPeriodType
                        If contract.LabAgree.TelecommutingMaxPercentage > 0 Then

                            Me.cmbDaysOrPercent.SelectedIndex = 1
                            Me.txtTelecommutingMaxOptional.Value = contract.LabAgree.TelecommutingMaxPercentage
                        Else
                            Me.txtTelecommutingMaxOptional.Value = contract.LabAgree.TelecommutingMaxDays
                            Me.cmbDaysOrPercent.SelectedIndex = 0

                        End If
                        Me.txtCanTelecommuteFrom.Value = contract.LabAgree.TelecommutingAgreementStart
                        Me.txtCanTelecommuteTo.Value = contract.LabAgree.TelecommutingAgreementEnd
                        Dim dsLabAgreePattern = Me.LabAgreeTelecommutingPattern(contract.LabAgree.ID)
                        ContractScheduleRulesCallback.JSProperties.Add("cpTelecommutingPatternResult", roJSONHelper.SerializeNewtonSoft(dsLabAgreePattern))
                        ContractScheduleRulesCallback.JSProperties.Add("cpTelecommutingMandatoryDays", contract.LabAgree.TelecommutingMandatoryDays)
                        ContractScheduleRulesCallback.JSProperties.Add("cpPresenceMandatoryDays", contract.LabAgree.PresenceMandatoryDays)
                        ContractScheduleRulesCallback.JSProperties.Add("cpTelecommutingOptionalDays", contract.LabAgree.TelecommutingOptionalDays)
                        Me.optOverwriteTelecommuting.Checked = False
                    Else
                        Me.optOverwriteTelecommuting.Checked = False
                        Me.ckTelecommuteYes.Value = False
                        Me.ckTelecommuteNo.Value = True
                    End If
                Else
                    Me.optOverwriteTelecommuting.Checked = False
                    Me.ckTelecommuteYes.Value = False
                    Me.ckTelecommuteNo.Value = True
                End If

            End If

        End If

    End Sub

    Protected Sub SetLabAgreeRulesData(ByVal idLabAgree As Integer)

        Dim labAgreeRules As Generic.List(Of roScheduleRule) = ScheduleRulesServiceMethods.GetLabAgreeScheduleRules(Me.Page, idLabAgree).ToList
        Dim tmpRule As roScheduleRule = Nothing

        Dim oMaxAnualHours As roScheduleRule = Me.LabAgreeSystemScheduleRulesValuesData.Find(Function(x) x.IDRule = ScheduleRuleType.MinMaxExpectedHours)
        If oMaxAnualHours IsNot Nothing Then
            Me.txtYearHours.Value = CType(oMaxAnualHours, roScheduleRule_MinMaxExpectedHours).MaximumWorkingHours
            Me.txtFork.Value = CType(oMaxAnualHours, roScheduleRule_MinMaxExpectedHours).MaximumWorkingHoursFork
        Else
            tmpRule = labAgreeRules.Find(Function(x) x.IDRule = ScheduleRuleType.MinMaxExpectedHours)
            If tmpRule IsNot Nothing Then
                Me.txtYearHours.Value = CType(tmpRule, roScheduleRule_MinMaxExpectedHours).MaximumWorkingHours
                Me.txtFork.Value = 0
            End If
        End If

        Dim oMaxYearHolidays As roScheduleRule = Me.LabAgreeSystemScheduleRulesValuesData.Find(Function(x) x.IDRule = ScheduleRuleType.MaxHolidays)
        If oMaxYearHolidays IsNot Nothing Then
            Me.txtYearHolidays.Value = CType(oMaxYearHolidays, roScheduleRule_MaxHolidays).MaximumHoliDays
        Else
            tmpRule = labAgreeRules.Find(Function(x) x.IDRule = ScheduleRuleType.MaxHolidays)
            If tmpRule IsNot Nothing Then
                Me.txtYearHolidays.Value = CType(tmpRule, roScheduleRule_MaxHolidays).MaximumHoliDays
            End If
        End If

        Dim oWorkOnHolidays As roScheduleRule = Me.LabAgreeSystemScheduleRulesValuesData.Find(Function(x) x.IDRule = ScheduleRuleType.WorkOnFestive)
        If oWorkOnHolidays IsNot Nothing Then
            Me.chkCanWorkOnFeastDays.Checked = Not CType(oWorkOnHolidays, roScheduleRule_WorkOnFestive).Enabled
        Else
            tmpRule = labAgreeRules.Find(Function(x) x.IDRule = ScheduleRuleType.WorkOnFestive)
            If tmpRule IsNot Nothing Then
                Me.chkCanWorkOnFeastDays.Checked = Not CType(tmpRule, roScheduleRule_WorkOnFestive).Enabled
            End If
        End If

        Dim oWorkOnWeekend As roScheduleRule = Me.LabAgreeSystemScheduleRulesValuesData.Find(Function(x) x.IDRule = ScheduleRuleType.WorkOnWeekend)
        If oWorkOnWeekend IsNot Nothing Then
            Me.chkCanWorkOnNonWorkingDays.Checked = Not CType(oWorkOnWeekend, roScheduleRule_WorkOnWeekend).Enabled
            Me.tbWorkingDays.Value = CType(oWorkOnWeekend, roScheduleRule_WorkOnWeekend).LabourDaysIndex
        Else
            tmpRule = labAgreeRules.Find(Function(x) x.IDRule = ScheduleRuleType.WorkOnWeekend)
            If tmpRule IsNot Nothing Then
                Me.chkCanWorkOnNonWorkingDays.Checked = Not CType(tmpRule, roScheduleRule_WorkOnWeekend).Enabled
                Me.tbWorkingDays.Value = CType(tmpRule, roScheduleRule_WorkOnWeekend).LabourDaysIndex
            End If
        End If

        Me.txtYearHours.JSProperties.Add("cpCustomized", If(oMaxAnualHours IsNot Nothing, True, False))
        Me.txtYearHours.JSProperties.Add("cpSwitch", "switchYearHours")

        Me.txtYearHolidays.JSProperties.Add("cpCustomized", If(oMaxYearHolidays IsNot Nothing, True, False))
        Me.txtYearHolidays.JSProperties.Add("cpSwitch", "switchYearHolidays")

        Me.tbWorkingDays.JSProperties.Add("cpCustomized", If(oWorkOnWeekend IsNot Nothing, True, False))
        Me.tbWorkingDays.JSProperties.Add("cpSwitch", "switchWorkingDays")

        Me.chkCanWorkOnFeastDays.JSProperties.Add("cpCustomized", If(oWorkOnHolidays IsNot Nothing, True, False))
        Me.chkCanWorkOnFeastDays.JSProperties.Add("cpSwitch", "switchCanWorkOnFeastDays")

        Me.chkCanWorkOnNonWorkingDays.JSProperties.Add("cpCustomized", If(oWorkOnWeekend IsNot Nothing, True, False))
        Me.chkCanWorkOnNonWorkingDays.JSProperties.Add("cpSwitch", "switchCanWorkOnNonWorkingDays")

    End Sub

    Protected Function ParseClientSchedulerRules(ByVal clientCustomizedRules As String) As roScheduleRule()
        Dim oNewSchedulingRules As New Generic.List(Of roScheduleRule)

        Dim labAgreeRules As New Generic.List(Of roScheduleRule)

        If Me.optOverwriteScheduleRules.Checked = True Then
            oNewSchedulingRules.AddRange(Me.LabAgreeScheduleRulesValuesData().ToArray)
        End If

        Dim oMaxAnualHours As roScheduleRule = Me.LabAgreeSystemScheduleRulesValuesData.Find(Function(x) x.IDRule = ScheduleRuleType.MinMaxExpectedHours)

        If roTypes.Any2Boolean(clientCustomizedRules(0).ToString) Then
            If oMaxAnualHours IsNot Nothing Then
                CType(oMaxAnualHours, roScheduleRule_MinMaxExpectedHours).MaximumWorkingHours = roTypes.Any2Integer(Me.txtYearHours.Value)
                CType(oMaxAnualHours, roScheduleRule_MinMaxExpectedHours).MaximumWorkingHoursFork = roTypes.Any2Integer(Me.txtFork.Value)
            Else
                oMaxAnualHours = New roScheduleRule_MinMaxExpectedHours With {
                                    .Id = -1,
                                    .IDRule = ScheduleRuleType.MinMaxExpectedHours,
                                    .Enabled = True,
                                    .IdLabAgree = 0,
                                    .IdContract = roTypes.Any2String(Session("Contract_SelectedID")),
                                    .MaximumWorkingHours = roTypes.Any2Integer(Me.txtYearHours.Value),
                                    .MinimumWorkingHours = 0,
                                    .MaximumEmployeeField = String.Empty,
                                    .MinimumEmployeeField = String.Empty,
                                    .MaximumWorkingHoursFork = roTypes.Any2Integer(Me.txtFork.Value)
                                }
            End If

            oNewSchedulingRules.Add(oMaxAnualHours)
        End If

        Dim oMaxYearHolidays As roScheduleRule = Me.LabAgreeSystemScheduleRulesValuesData.Find(Function(x) x.IDRule = ScheduleRuleType.MaxHolidays)
        If roTypes.Any2Boolean(clientCustomizedRules(1).ToString) Then
            If oMaxYearHolidays IsNot Nothing Then
                CType(oMaxYearHolidays, roScheduleRule_MaxHolidays).MaximumHoliDays = roTypes.Any2Integer(Me.txtYearHolidays.Value)
            Else
                oMaxYearHolidays = New roScheduleRule_MaxHolidays With {
                                       .Id = -1,
                                       .IDRule = ScheduleRuleType.MaxHolidays,
                                       .Enabled = True,
                                       .IdLabAgree = 0,
                                       .IdContract = roTypes.Any2String(Session("Contract_SelectedID")),
                                       .MaximumHoliDays = roTypes.Any2Integer(txtYearHolidays.Value)
                                   }
            End If
            oNewSchedulingRules.Add(oMaxYearHolidays)
        End If

        Dim oWorkOnHolidays As roScheduleRule = Me.LabAgreeSystemScheduleRulesValuesData.Find(Function(x) x.IDRule = ScheduleRuleType.WorkOnFestive)
        If roTypes.Any2Boolean(clientCustomizedRules(3).ToString) Then
            If oWorkOnHolidays IsNot Nothing Then
                CType(oWorkOnHolidays, roScheduleRule_WorkOnFestive).Enabled = Not chkCanWorkOnFeastDays.Checked
            Else
                oWorkOnHolidays = New roScheduleRule_WorkOnFestive With {
                                        .Id = -1,
                                        .IDRule = ScheduleRuleType.WorkOnFestive,
                                        .Enabled = Not chkCanWorkOnFeastDays.Checked,
                                        .IdLabAgree = 0,
                                        .IdContract = roTypes.Any2String(Session("Contract_SelectedID"))
                                    }
            End If
            oNewSchedulingRules.Add(oWorkOnHolidays)
        End If

        Dim oWorkOnWeekend As roScheduleRule = Me.LabAgreeSystemScheduleRulesValuesData.Find(Function(x) x.IDRule = ScheduleRuleType.WorkOnWeekend)
        If roTypes.Any2Boolean(clientCustomizedRules(4).ToString) OrElse roTypes.Any2Boolean(clientCustomizedRules(2).ToString) Then
            If oWorkOnWeekend IsNot Nothing Then
                CType(oWorkOnWeekend, roScheduleRule_WorkOnWeekend).Enabled = Not chkCanWorkOnNonWorkingDays.Checked
                CType(oWorkOnWeekend, roScheduleRule_WorkOnWeekend).LabourDaysIndex = tbWorkingDays.Value
            Else
                oWorkOnWeekend = New roScheduleRule_WorkOnWeekend With {
                                    .Id = -1,
                                    .IDRule = ScheduleRuleType.WorkOnWeekend,
                                    .Enabled = Not chkCanWorkOnNonWorkingDays.Checked,
                                    .IdLabAgree = 0,
                                    .IdContract = roTypes.Any2String(Session("Contract_SelectedID")),
                                    .LabourDaysIndex = tbWorkingDays.Value
                                }
            End If
            oNewSchedulingRules.Add(oWorkOnWeekend)
        End If

        Return oNewSchedulingRules.ToArray
    End Function

#End Region

#Region "Grid ScheduleRules"

    Private Sub CreateScheduleRulesColumns()

        Dim GridColumn As GridViewDataTextColumn
        Dim GridColumnCommand As GridViewCommandColumn
        Dim GridCheckCommand As GridViewDataCheckColumn

        Dim VisibleIndex As Integer = 0

        Me.GridContractScheduleRules.Columns.Clear()
        Me.GridContractScheduleRules.KeyFieldName = "Id"
        Me.GridContractScheduleRules.SettingsText.EmptyDataRow = " "
        Me.GridContractScheduleRules.Settings.VerticalScrollBarMode = ScrollBarMode.Auto

        If Me.oPermission = Permission.Admin Or Me.oPermission >= Permission.Write Then
            Me.GridContractScheduleRules.SettingsEditing.Mode = GridViewEditingMode.Inline
        Else
            Dim uiControl As Control = Me.GridContractScheduleRules.FindTitleTemplateControl("btnAddNewLabAgreeScheduleRules")
            If uiControl IsNot Nothing Then
                uiControl.Visible = False
            End If
        End If

        'Clave
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "Name"
        GridColumn.FieldName = "Id"
        GridColumn.ReadOnly = False
        GridColumn.Visible = False
        Me.GridContractScheduleRules.Columns.Add(GridColumn)

        Dim obj As New roStartupValue

        If Me.oPermission >= Permission.Write Then

            GridColumnCommand = New GridViewCommandColumn()
            GridColumnCommand.ButtonRenderMode = GridCommandButtonRenderMode.Image

            Dim editButton As GridViewCommandColumnCustomButton = New GridViewCommandColumnCustomButton()
            editButton.ID = "ShowDetailButton"
            editButton.Image.Url = "~/Base/Images/Grid/edit.png"
            editButton.Text = Me.Language.Translate("GridContractScheduleRules.Column.Edit", DefaultScope) 'Mostrar detalles"

            GridColumnCommand.CustomButtons.Add(editButton)
            GridColumnCommand.ShowEditButton = False
            GridColumnCommand.ShowDeleteButton = False
            GridColumnCommand.Caption = " "
            GridColumnCommand.VisibleIndex = VisibleIndex
            GridColumnCommand.CellStyle.HorizontalAlign = HorizontalAlign.Center
            GridColumnCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
            GridColumnCommand.Width = 32
            VisibleIndex = VisibleIndex + 1

            Me.GridContractScheduleRules.Columns.Add(GridColumnCommand)

        End If

        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridContractScheduleRules.Column.LabAgree", DefaultScope) '"Fecha"
        GridColumn.FieldName = "Id"
        GridColumn.Visible = False
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        Me.GridContractScheduleRules.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridContractScheduleRules.Column.LabAgree", DefaultScope) '"Fecha"
        GridColumn.FieldName = "IdLabAgree"
        GridColumn.Visible = False
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        Me.GridContractScheduleRules.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridContractScheduleRules.Column.Name", DefaultScope) '"Fecha"
        GridColumn.FieldName = "RuleName"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.UnboundType = DevExpress.Data.UnboundColumnType.String
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        Me.GridContractScheduleRules.Columns.Add(GridColumn)

        GridCheckCommand = New GridViewDataCheckColumn()
        GridCheckCommand.Caption = Me.Language.Translate("GridContractScheduleRules.Column.Active", DefaultScope) '"Fecha"
        GridCheckCommand.FieldName = "Enabled"
        GridCheckCommand.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridCheckCommand.ReadOnly = True
        GridCheckCommand.UnboundType = DevExpress.Data.UnboundColumnType.String
        GridCheckCommand.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridCheckCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridCheckCommand.Width = 80
        Me.GridContractScheduleRules.Columns.Add(GridCheckCommand)

        If Me.oPermission >= Permission.Write Then
            'Command buttons
            GridColumnCommand = New GridViewCommandColumn()
            GridColumnCommand.ButtonRenderMode = GridCommandButtonRenderMode.Image
            GridColumnCommand.ShowDeleteButton = True
            GridColumnCommand.ShowEditButton = False
            GridColumnCommand.ShowCancelButton = False
            GridColumnCommand.ShowUpdateButton = True
            GridColumnCommand.Caption = " "
            GridColumnCommand.VisibleIndex = VisibleIndex
            GridColumnCommand.CellStyle.HorizontalAlign = HorizontalAlign.Center
            GridColumnCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
            GridColumnCommand.Width = 32
            VisibleIndex = VisibleIndex + 1

            Me.GridContractScheduleRules.Columns.Add(GridColumnCommand)
        End If

    End Sub

    Protected Sub GridContractScheduleRules_CustomUnboundColumnData(sender As Object, e As ASPxGridViewColumnDataEventArgs) Handles GridContractScheduleRules.CustomUnboundColumnData
        Select Case e.Column.FieldName
            Case "RuleValidation"
                If e.IsGetData Then
                    If e.GetListSourceFieldValue("Id") IsNot System.DBNull.Value Then
                        Dim oItem As roScheduleRule = getSchedulerRule(roTypes.Any2Integer(e.GetListSourceFieldValue("IDRule")))
                        If oItem IsNot Nothing Then e.Value = oItem.RuleName
                    End If
                End If

        End Select
    End Sub

    Private Sub BindGridContractScheduleRules(ByVal bolReload As Boolean)
        Me.GridContractScheduleRules.DataSource = Me.LabAgreeScheduleRulesValuesData(bolReload)
        Me.GridContractScheduleRules.DataBind()
    End Sub

    Protected Sub GridContractScheduleRules_CustomCallback(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxGridViewCustomCallbackEventArgs) Handles GridContractScheduleRules.CustomCallback
        If e.Parameters = "REFRESH" Then
            BindGridContractScheduleRules(False)
            GridContractScheduleRules.JSProperties("cpAction") = "REFRESH"
        ElseIf e.Parameters = "RELOAD" Then
            BindGridContractScheduleRules(True)
            GridContractScheduleRules.JSProperties("cpAction") = "RELOAD"
        End If
    End Sub

    Protected Sub GridContractScheduleRules_RowDeleting(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataDeletingEventArgs) Handles GridContractScheduleRules.RowDeleting
        Dim grid As ASPxGridView = CType(sender, ASPxGridView)
        Dim tb As Generic.List(Of roScheduleRule) = Me.LabAgreeScheduleRulesValuesData

        If e.Values("Id") > 0 OrElse e.Values("Id") < -1 Then
            Dim selObject As roScheduleRule = Nothing
            For Each oObject As roScheduleRule In tb
                If oObject.Id = e.Values("Id") Then
                    selObject = oObject
                    Exit For
                End If
            Next

            If selObject IsNot Nothing Then
                tb.Remove(selObject)
            End If

            Me.LabAgreeScheduleRulesValuesData = tb
            e.Cancel = True

        End If
        BindGridContractScheduleRules(False)
        GridContractScheduleRules.JSProperties("cpAction") = "ROWDELETE"

    End Sub

    Private Function getSchedulerRule(ByVal oId As Integer) As roScheduleRule
        Dim oObject As roScheduleRule = Nothing

        Dim oList As Generic.List(Of roScheduleRule) = LabAgreeScheduleRulesValuesData

        If oList IsNot Nothing AndAlso oList.Count > 0 Then
            For Each oElem As roScheduleRule In oList
                If oElem.Id = oId Then
                    oObject = oElem
                    Exit For
                End If
            Next
        End If

        Return oObject
    End Function

    Private Function GetTelecommutingTypeDay(ByVal strDay As String, ByVal oCurrentContract As roContract) As String
        If oCurrentContract.TelecommutingMandatoryDays.Split(",").Contains(strDay) Then
            Return TELECOMMUTING_ID
        Else
            If oCurrentContract.TelecommutingOptionalDays.Split(",").Contains(strDay) Then
                Return OPTIONAL_ID
            Else

                Return PRESENCE_ID
            End If
        End If
    End Function

    Private Function GetTelecommutingTypeDay(ByVal strDay As String, ByVal oCurrentLabAgree As roLabAgree) As String
        If oCurrentLabAgree.TelecommutingMandatoryDays.Split(",").Contains(strDay) Then
            Return TELECOMMUTING_ID
        Else
            If oCurrentLabAgree.TelecommutingOptionalDays.Split(",").Contains(strDay) Then
                Return OPTIONAL_ID
            Else
                Return PRESENCE_ID
            End If
        End If
    End Function

#End Region

End Class