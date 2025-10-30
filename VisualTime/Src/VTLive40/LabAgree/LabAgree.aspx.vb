Imports System.Linq.Expressions
Imports DevExpress.Web
Imports Microsoft.Ajax.Utilities
Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTEmployees.LabAgree
Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class LabAgree
    Inherits PageBase

    <Runtime.Serialization.DataContract()>
    Private Class CallbackRequest

        <Runtime.Serialization.DataMember(Name:="aTab")>
        Public aTab As Integer

        <Runtime.Serialization.DataMember(Name:="ID")>
        Public ID As Integer

        <Runtime.Serialization.DataMember(Name:="oType")>
        Public Type As String

        <Runtime.Serialization.DataMember(Name:="action")>
        Public Action As String

        <Runtime.Serialization.DataMember(Name:="Name")>
        Public Name As String

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

        <Runtime.Serialization.DataMember(Name:="TelecommutingMaxOptionalDays")>
        Public TelecommutingMaxOptionalDays As Integer

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

#Region "Properties"

    Private ReadOnly Property GetAvailableRequestTypes(Optional ByVal bolReload As Boolean = False) As DataTable
        Get

            Dim tbDocuments As DataTable = Session("LabAgree_AvailableRequestTypes")

            If bolReload Or tbDocuments Is Nothing Then
                tbDocuments = API.LabAgreeServiceMethods.GetAvailableRequestType(Me.Page)
                Session("LabAgree_AvailableRequestTypes") = tbDocuments

            End If
            Return tbDocuments

        End Get
    End Property

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
            Dim dic = ViewState("LabAgree_WeekDaysDictionary")

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

                ViewState("LabAgree_WeekDaysDictionary") = dic
            End If

            Return dic
        End Get
        Set(value As Generic.List(Of WeekDayInfo))
            ViewState("LabAgree_WeekDaysDictionary") = value
        End Set
    End Property

    Private Property LabAgreedCausesLimitData(Optional ByVal bolReload As Boolean = False) As Generic.List(Of roLabAgreeCauseLimitValues)
        Get

            Dim tbCauses As Generic.List(Of roLabAgreeCauseLimitValues) = Session("LabAgree_LabAgreedCasuesLimitData")

            If bolReload Or tbCauses Is Nothing Then
                Dim oList As New Generic.List(Of roLabAgreeCauseLimitValues)
                If roTypes.Any2Integer(Session("LabAgree_SelectedID")) > -1 Then
                    Dim oCurrentLabAgree As roLabAgree = API.LabAgreeServiceMethods.GetLabAgreeByID(Me, Session("LabAgree_SelectedID"), True)
                    For Each oLabObject As roLabAgreeCauseLimitValues In oCurrentLabAgree.LabAgreeCauseLimitValues

                        If oLabObject.EndDate = New Date(2079, 1, 1) Then
                            oLabObject.EndDate = Nothing
                        End If

                        oList.Add(oLabObject)
                    Next

                End If
                tbCauses = oList
                Session("LabAgree_LabAgreedCasuesLimitData") = oList

            End If
            Return tbCauses

        End Get
        Set(ByVal value As Generic.List(Of roLabAgreeCauseLimitValues))
            If value IsNot Nothing Then
                Session("LabAgree_LabAgreedCasuesLimitData") = value
            Else
                Session("LabAgree_LabAgreedCasuesLimitData") = Nothing
            End If
        End Set
    End Property

    Private Property LabAgreedRulesData(Optional ByVal bolReload As Boolean = False) As Generic.List(Of roLabAgreeAccrualRule)
        Get

            Dim tbCauses As Generic.List(Of roLabAgreeAccrualRule) = Session("LabAgree_LabAgreedRulesData")

            If bolReload Or tbCauses Is Nothing Then
                Dim oList As New Generic.List(Of roLabAgreeAccrualRule)
                If roTypes.Any2Integer(Session("LabAgree_SelectedID")) > -1 Then
                    Dim oCurrentLabAgree As roLabAgree = API.LabAgreeServiceMethods.GetLabAgreeByID(Me, Session("LabAgree_SelectedID"), True)
                    For Each oLabObject As roLabAgreeAccrualRule In oCurrentLabAgree.LabAgreeAccrualRules

                        If oLabObject.EndDate = New Date(2079, 1, 1) Then
                            oLabObject.EndDate = Nothing
                        End If

                        oList.Add(oLabObject)
                    Next

                End If
                tbCauses = oList
                Session("LabAgree_LabAgreedRulesData") = oList

            End If
            Return tbCauses

        End Get
        Set(ByVal value As Generic.List(Of roLabAgreeAccrualRule))
            If value IsNot Nothing Then
                Session("LabAgree_LabAgreedRulesData") = value
            Else
                Session("LabAgree_LabAgreedRulesData") = Nothing
            End If
        End Set
    End Property

    Private Property LabAgreeStartUpValuesData(Optional ByVal bolReload As Boolean = False) As Generic.List(Of roStartupValue)
        Get

            Dim tbValues As Generic.List(Of roStartupValue) = Session("LabAgree_LabAgreedStartupValues")

            If bolReload Or tbValues Is Nothing Then

                Dim oList As New Generic.List(Of roStartupValue)

                If roTypes.Any2Integer(Session("LabAgree_SelectedID")) > -1 Then
                    Dim oCurrentLabAgree As roLabAgree = API.LabAgreeServiceMethods.GetLabAgreeByID(Me, Session("LabAgree_SelectedID"), True)
                    oList.AddRange(oCurrentLabAgree.StartupValues())
                End If

                tbValues = oList
                Session("LabAgree_LabAgreedStartupValues") = tbValues

            End If
            Return tbValues

        End Get
        Set(ByVal value As Generic.List(Of roStartupValue))
            If value IsNot Nothing Then
                Session("LabAgree_LabAgreedStartupValues") = value
            Else
                Session("LabAgree_LabAgreedStartupValues") = Nothing
            End If
        End Set
    End Property

    Private ReadOnly Property LabAgreeTelecommutingPattern(Optional ByVal bolReload As Boolean = False) As Generic.List(Of roWeekDays)
        Get

            Dim tbValues As Generic.List(Of roWeekDays) = New List(Of roWeekDays)()

            If bolReload Or tbValues Is Nothing Or tbValues.Count = 0 Then

                Dim oList As New Generic.List(Of roWeekDays)

                Dim row As roWeekDays = New roWeekDays()
                Dim oCurrentLabAgree As roLabAgree = API.LabAgreeServiceMethods.GetLabAgreeByID(Me, Session("LabAgree_SelectedID"), True)
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

    Private Property LabAgreeRequestValidationValuesData(Optional ByVal bolReload As Boolean = False) As Generic.List(Of roRequestRule)
        Get

            Dim tbValues As Generic.List(Of roRequestRule) = Session("LabAgree_LabAgreedRequestValidations")

            If bolReload Or tbValues Is Nothing Then

                Dim oList As New Generic.List(Of roRequestRule)

                If roTypes.Any2Integer(Session("LabAgree_SelectedID")) > -1 Then
                    Dim oCurrentLabAgree As roLabAgree = API.LabAgreeServiceMethods.GetLabAgreeByID(Me, Session("LabAgree_SelectedID"), True)
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

    Private Property LabAgreeScheduleRulesValuesData(Optional ByVal bolReload As Boolean = False) As Generic.List(Of roScheduleRule)
        Get

            Dim tbValues As Generic.List(Of roScheduleRule) = Session("LabAgree_LabAgreedScheduleRules")

            If bolReload Or tbValues Is Nothing Then

                Dim oList As New Generic.List(Of roScheduleRule)

                If roTypes.Any2Integer(Session("LabAgree_SelectedID")) > -1 Then
                    oList = API.ScheduleRulesServiceMethods.GetLabAgreeScheduleRules(Me.Page, roTypes.Any2Integer(Session("LabAgree_SelectedID"))).ToList
                End If

                tbValues = oList
                Session("LabAgree_LabAgreedScheduleRules") = tbValues
            End If

            tbValues = tbValues.FindAll(Function(x) x.RuleType = ScheduleRuleBaseType.User)

            Return tbValues

        End Get
        Set(ByVal value As Generic.List(Of roScheduleRule))
            If value IsNot Nothing Then
                ' Preservo las reglas generales del convenio, porque aquí me están llegando sólo las individuales
                Dim currentRulesInSession As List(Of roScheduleRule)
                currentRulesInSession = TryCast(Session("LabAgree_LabAgreedScheduleRules"), List(Of roScheduleRule))

                If currentRulesInSession Is Nothing Then
                    currentRulesInSession = New List(Of roScheduleRule)()
                End If

                ' Elimino de currentRulesInSession los elemetos con ID que vengan en value
                currentRulesInSession.RemoveAll(Function(rule) rule.RuleType <> ScheduleRuleBaseType.System)

                ' Agrego los nuevos elementos de value, asegurándome de que no haya duplicados si ya existían con RuleType diferente
                ' (aunque el RemoveAll anterior debería haberlos quitado si tenían el mismo ID)
                ' No obstante, para ser más robusto, se podría verificar antes de añadir.
                ' Por ahora, simplemente añadimos todos los elementos de 'value'.
                currentRulesInSession.AddRange(value)

                Session("LabAgree_LabAgreedScheduleRules") = currentRulesInSession
            Else
                Session("LabAgree_LabAgreedScheduleRules") = Nothing
            End If
        End Set
    End Property

    Private Property LabAgreeSystemScheduleRulesValuesData(Optional ByVal bolReload As Boolean = False) As Generic.List(Of roScheduleRule)
        Get

            Dim tbValues As Generic.List(Of roScheduleRule) = Session("LabAgree_LabAgreedScheduleRules")

            If bolReload OrElse tbValues Is Nothing Then

                Dim oList As New Generic.List(Of roScheduleRule)

                If roTypes.Any2Integer(Session("LabAgree_SelectedID")) > -1 Then
                    oList = API.ScheduleRulesServiceMethods.GetLabAgreeScheduleRules(Me.Page, roTypes.Any2Integer(Session("LabAgree_SelectedID"))).ToList
                End If

                tbValues = oList
                Session("LabAgree_LabAgreedScheduleRules") = tbValues

            End If

            tbValues = tbValues.FindAll(Function(x) x.RuleType = ScheduleRuleBaseType.System)

            Return tbValues

        End Get
        Set(ByVal value As Generic.List(Of roScheduleRule))
            If value IsNot Nothing Then
                Session("LabAgree_LabAgreedScheduleRules") = value
            Else
                Session("LabAgree_LabAgreedScheduleRules") = Nothing
            End If
        End Set
    End Property

#End Region

    Private Const FeatureAlias As String = "LabAgree.Definition"
    Private Const PRESENCE_ID As String = "0"
    Private Const TELECOMMUTING_ID As String = "1"
    Private Const OPTIONAL_ID As String = "2"
    Private oPermission As Permission

#Region "Events"

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("Ajax", "~/Base/Scripts/Ajax.js")
        Me.InsertExtraJavascript("jsDate", "~/Base/Scripts/jsDate.js")
        Me.InsertExtraJavascript("LabAgreeV2", "~/LabAgree/Scripts/LabAgreeV2.js")
        Me.InsertExtraJavascript("LabAgreeGrids", "~/LabAgree/Scripts/LabAgreeGrids.js")

        Me.InsertExtraJavascript("webUserForms", "~/Base/Scripts/webUserForms.js")
        Me.InsertExtraJavascript("frmEditStartupValue", "~/LabAgree/Scripts/frmEditStartupValue.js")
        Me.InsertExtraJavascript("frmEditLabAgreeRule", "~/LabAgree/Scripts/frmEditLabAgreeRule.js")
        Me.InsertExtraJavascript("frmEditCauseLimit", "~/LabAgree/Scripts/frmEditCauseLimit.js")
        Me.InsertExtraJavascript("frmEditRequestValidation", "~/LabAgree/Scripts/frmEditRequestValidation.js")
        Me.InsertExtraJavascript("frmEditScheduleRules", "~/LabAgree/Scripts/frmEditScheduleRules.js")

        Me.InsertExtraJavascript("GenericData", "~/Base/Scripts/GenericData.js")
        Me.InsertExtraJavascript("roOptionPanelClient", "~/Base/Scripts/roOptionPanelClient.js")
        Me.InsertExtraJavascript("roOptPanelClientGroup", "~/Base/Scripts/roOptPanelClientGroup.js")
    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Comprueba la licencia
        If Not HasFeaturePermission("LabAgree.Definition", Permission.Read) Then
            WLHelperWeb.RedirectAccessDenied(False)
            Return
        End If

        roTreesLabAgrees.TreeCaption = Me.Language.Translate("TreeCaptionLabAgree", Me.DefaultScope)

        Me.lblSaveChanges.InnerText = Me.Language.Translate("msgSaveChangesbar", DefaultScope)
        Me.lblSaveChangesBottom.InnerText = Me.lblSaveChanges.InnerText
        Me.lblUndoChanges.InnerText = Me.Language.Translate("msgUndoChangesbar", DefaultScope)
        Me.lblUndoChangesBottom.InnerText = Me.lblUndoChanges.InnerText

        Me.msgHasChanges.Value = Me.Language.Translate("msgHasChanges", DefaultScope)
        Me.msgHasErrors.Value = Me.Language.Translate("msgHasErrors", DefaultScope)
        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)

        If Me.oPermission < Permission.Write Then
            hdnModeEdit.Value = "true"
        Else
            hdnModeEdit.Value = "false"
        End If

        If Me.oPermission = Permission.None Then
            WLHelperWeb.RedirectAccessDenied(False)
            Exit Sub
        End If

        LoadStaticInfo()

        If Not HelperSession.GetFeatureIsInstalledFromApplication("Feature\Telecommuting") Then
            Me.divTelecommutingGeneral.Style("display") = "none"
        End If
        Me.ckTelecommuteNo.Value = True
        Me.ckTelecommuteYes.Value = False

        'Añadimos valores por defecto de configuración de horas extra
        If Not roTypes.Any2Boolean(HelperSession.AdvancedParametersCache("Latam.OvertimeManagement")) Then
            Me.divHorasExtraGeneral.Style("display") = "none"
        End If


        If Not Me.IsPostBack Then
            CreateLabAgreeRulesColumns()
            CreateStartupValuesColumns()
            CreateLabAgreeCauseLimitsColumns()
            CreateRequestValidationColumns()
            CreateScheduleRulesColumns()

            'Dim oRules = ScheduleRulesServiceMethods.GetScheduleRule(Me.Page)

            'If oRules.Length > 0 Then

            'End If
        End If

        Dim dTbl As DataTable = API.LabAgreeServiceMethods.GetLabAgrees(Me.Page)
        If dTbl.Rows.Count = 0 Then
            Me.noRegs.Value = "1"
        Else
            Me.noRegs.Value = ""
        End If

    End Sub

    Private Sub LoadStaticInfo()

        tbWorkingDays.Items.Clear()
        For Each oDay In LaboralDays
            tbWorkingDays.Items.Add(oDay.Name, oDay.ID)
        Next
        cmbWeekOrMonth.Items.Clear()

        cmbWeekOrMonth.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("LabAgree.Week", DefaultScope), 0))
        cmbWeekOrMonth.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("LabAgree.Month", DefaultScope), 1))
        cmbWeekOrMonth.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("LabAgree.Quarter", DefaultScope), 2))

        cmbDaysOrPercent.Items.Clear()

        cmbDaysOrPercent.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("LabAgree.Days", DefaultScope), 0))
        cmbDaysOrPercent.Items.Add(New DevExpress.Web.ListEditItem("%", 1))

        cmbUserField.Items.Clear()
        Dim tbUserFields As DataTable = API.UserFieldServiceMethods.GetUserFields(Me.Page, Types.EmployeeField, "FieldType IN(1) AND Used=1", False)
        For Each oRow As DataRow In tbUserFields.Select("", "FieldName")
            cmbUserField.Items.Add(New DevExpress.Web.ListEditItem(oRow("FieldName"), oRow("FieldName")))
        Next

        If roTypes.Any2Boolean(HelperSession.AdvancedParametersCache("Latam.OvertimeManagement")) Then
            tbCauses.Items.Clear()
            tbExtras.Items.Clear()
            cmbCausesDbl.Items.Clear()
            cmbCausesTpl.Items.Clear()

            Dim tbCausesAll As DataTable = API.CausesServiceMethods.GetCauses(Me.Page, "", False)
            For Each oRow As DataRow In tbCausesAll.Select("", "Name")
                tbCauses.Items.Add(New DevExpress.Web.ListEditItem(oRow("Name"), oRow("ID")))
                tbExtras.Items.Add(New DevExpress.Web.ListEditItem(oRow("Name"), oRow("ID")))
                cmbCausesDbl.Items.Add(New DevExpress.Web.ListEditItem(oRow("Name"), oRow("ID")))
                cmbCausesTpl.Items.Add(New DevExpress.Web.ListEditItem(oRow("Name"), oRow("ID")))
            Next

        End If



    End Sub

    Protected Sub ASPxCallbackPanelContenido_Callback(ByVal sender As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase) Handles ASPxCallbackPanelContenido.Callback

        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Dim oParameters As New CallbackRequest()
        oParameters = roJSONHelper.Deserialize(strParameter, oParameters.GetType())

        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)

        If Me.oPermission > Permission.None Then
            If oParameters.Action = "GETLABAGREE" Then
                Session("LabAgree_SelectedID") = oParameters.ID
                LoadLabAgree(oParameters)
            ElseIf oParameters.Action = "SAVELABAGREE" Then
                Session("LabAgree_SelectedID") = oParameters.ID
                saveLabAgree(oParameters)
            End If
        End If

        Select Case oParameters.aTab
            Case 0
                Me.div00.Style("display") = ""
        End Select

    End Sub

#End Region

#Region "Methods"

    Private Sub LoadLabAgree(ByVal oParameters As CallbackRequest, Optional ByVal eLabAgree As roLabAgree = Nothing)
        If Me.oPermission < Permission.Write Then
            Me.DisableControls()
        End If

        Dim result As String = "OK"
        Dim oCurrentLabAgree As roLabAgree = Nothing
        Try
            Dim reloadGrids As Boolean = False

            If eLabAgree IsNot Nothing Then
                oCurrentLabAgree = eLabAgree
            Else
                If oParameters.ID = -1 Then
                    oCurrentLabAgree = New roLabAgree
                Else
                    oCurrentLabAgree = API.LabAgreeServiceMethods.GetLabAgreeByID(Me, oParameters.ID, True)
                End If
                reloadGrids = True
            End If

            If oCurrentLabAgree Is Nothing Then Exit Sub

            Me.txtName.Text = oCurrentLabAgree.Name
            Me.txtDescription.Value = oCurrentLabAgree.Description

            If HelperSession.GetFeatureIsInstalledFromApplication("Feature\Telecommuting") Then
                Me.ckTelecommuteYes.Value = oCurrentLabAgree.Telecommuting
                Me.cmbWeekOrMonth.SelectedIndex = oCurrentLabAgree.TelecommutingPeriodType
                If oCurrentLabAgree.TelecommutingMaxPercentage > 0 Then

                    Me.cmbDaysOrPercent.SelectedIndex = 1
                    Me.txtTelecommutingMaxOptional.Value = oCurrentLabAgree.TelecommutingMaxPercentage
                Else
                    Me.txtTelecommutingMaxOptional.Value = oCurrentLabAgree.TelecommutingMaxDays
                    Me.cmbDaysOrPercent.SelectedIndex = 0

                End If

                Me.txtCanTelecommuteFrom.Value = oCurrentLabAgree.TelecommutingAgreementStart
                Me.txtCanTelecommuteTo.Value = oCurrentLabAgree.TelecommutingAgreementEnd
                Dim dsLabAgreePattern = Me.LabAgreeTelecommutingPattern
                ASPxCallbackPanelContenido.JSProperties.Add("cpTelecommutingPatternResult", roJSONHelper.SerializeNewtonSoft(dsLabAgreePattern))
                Dim dsTelecommutingOptions = Me.ListTelecommutinOptionsData
                ASPxCallbackPanelContenido.JSProperties.Add("cpTelecommutingOptions", roJSONHelper.SerializeNewtonSoft(dsTelecommutingOptions))
                ASPxCallbackPanelContenido.JSProperties.Add("cpTelecommutingMandatoryDays", oCurrentLabAgree.TelecommutingMandatoryDays)
                ASPxCallbackPanelContenido.JSProperties.Add("cpPresenceMandatoryDays", oCurrentLabAgree.PresenceMandatoryDays)
                ASPxCallbackPanelContenido.JSProperties.Add("cpTelecommutingOptionalDays", oCurrentLabAgree.TelecommutingOptionalDays)
            End If

            If roTypes.Any2Boolean(HelperSession.AdvancedParametersCache("Latam.OvertimeManagement")) Then
                If oCurrentLabAgree IsNot Nothing Then
                    Select Case (oCurrentLabAgree.ExtraHoursConfiguration)
                        Case LabAgreeExtraHoursConfiguration.Disabled
                            Me.ckExtraDisabled.Value = True
                            Me.ckExtra3x3.Value = False
                            Me.ckExtra9acc.Value = False
                        Case LabAgreeExtraHoursConfiguration.ThreeByThree
                            Me.ckExtraDisabled.Value = False
                            Me.ckExtra3x3.Value = True
                            Me.ckExtra9acc.Value = False
                        Case LabAgreeExtraHoursConfiguration.NineAcc
                            Me.ckExtraDisabled.Value = False
                            Me.ckExtra3x3.Value = False
                            Me.ckExtra9acc.Value = True
                    End Select
                End If
                Me.tbExtras.Value = oCurrentLabAgree.ExtraHoursIDCauseSimples
                Me.cmbCausesDbl.SelectedItem = Me.cmbCausesDbl.Items.FindByValue(oCurrentLabAgree.ExtraHoursIDCauseDoubles.ToString)
                Me.cmbCausesTpl.SelectedItem = Me.cmbCausesTpl.Items.FindByValue(oCurrentLabAgree.ExtraHoursIDCauseTriples.ToString)
            End If

            Me.BindGridLabAgreeRules(reloadGrids)
            Me.BindGridStartupValuesRules(reloadGrids)
            Me.BindGridLabAgreeCauseLimits(reloadGrids)
            Me.BindGridLabAgreeRequestValidationRules(reloadGrids)
            Me.BindGridLabAgreeScheduleRules(reloadGrids)

            Me.SetLabAgreeRulesData()
        Catch ex As Exception
            result = "KO"
        Finally
            ASPxCallbackPanelContenido.JSProperties.Add("cpActionRO", "GETLABAGREE")
            ASPxCallbackPanelContenido.JSProperties.Add("cpNameRO", oCurrentLabAgree.Name)
            ASPxCallbackPanelContenido.JSProperties.Add("cpResultRO", result)
            ASPxCallbackPanelContenido.JSProperties.Add("cpIsNew", False)

            GridLabAgreeRules.JSProperties.Add("cpAction", "LOAD")
            GridStartupValues.JSProperties.Add("cpAction", "LOAD")

            If GridLabAgreeRules.IsEditing Then
                GridLabAgreeRules.CancelEdit()
            End If

            If GridStartupValues.IsEditing Then
                GridStartupValues.CancelEdit()
            End If
        End Try

    End Sub

    Private Sub saveLabAgree(ByVal oParameters As CallbackRequest)
        Dim strError As String = ""
        Dim oCurrentLabAgree As roLabAgree = Nothing

        Try

            'Check Permissions
            If Me.oPermission < Permission.Write Then
                strError = Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope)
            End If

            Dim bolIsNew As Boolean = False
            If oParameters.ID < 1 Then bolIsNew = True

            If Me.oPermission = Permission.Write And bolIsNew Then
                strError = Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope)
            End If

            If strError = String.Empty Then
                If bolIsNew Then
                    oCurrentLabAgree = New roLabAgree
                    oCurrentLabAgree.ID = -1
                Else
                    oCurrentLabAgree = API.LabAgreeServiceMethods.GetLabAgreeByID(Me, oParameters.ID, False)
                End If

                If oCurrentLabAgree Is Nothing Then
                    Exit Sub
                End If

                oCurrentLabAgree.Name = txtName.Text
                oCurrentLabAgree.Description = txtDescription.Text

                Dim list As Generic.List(Of roLabAgreeAccrualRule) = LabAgreedRulesData

                For Each oObject As roLabAgreeAccrualRule In list
                    If oObject.EndDate = Nothing Then
                        oObject.EndDate = New Date(2079, 1, 1)
                    End If
                Next

                Dim listCauses As Generic.List(Of roLabAgreeCauseLimitValues) = LabAgreedCausesLimitData

                For Each oObject As roLabAgreeCauseLimitValues In listCauses
                    If oObject.EndDate = Nothing Then
                        oObject.EndDate = New Date(2079, 1, 1)
                    End If
                Next

                oCurrentLabAgree.StartupValues = LabAgreeStartUpValuesData
                oCurrentLabAgree.LabAgreeAccrualRules = list
                oCurrentLabAgree.LabAgreeCauseLimitValues = listCauses
                oCurrentLabAgree.LabAgreeRequestRules = LabAgreeRequestValidationValuesData

                If HelperSession.GetFeatureIsInstalledFromApplication("Feature\Telecommuting") Then
                    oCurrentLabAgree.Telecommuting = oParameters.Telecommuting
                    oCurrentLabAgree.TelecommutingMandatoryDays = oParameters.TelecommutingMandatoryDays
                    oCurrentLabAgree.PresenceMandatoryDays = oParameters.PresenceMandatoryDays
                    oCurrentLabAgree.TelecommutingPeriodType = oParameters.TelecommutingPeriodType
                    oCurrentLabAgree.TelecommutingOptionalDays = oParameters.TelecommutingOptionalDays
                    oCurrentLabAgree.TelecommutingAgreementStart = roTypes.Any2DateTime(oParameters.TelecommutingAgreementStart)
                    oCurrentLabAgree.TelecommutingAgreementEnd = roTypes.Any2DateTime(oParameters.TelecommutingAgreementEnd)
                    If Me.cmbDaysOrPercent.SelectedIndex = 1 Then
                        oCurrentLabAgree.TelecommutingMaxPercentage = oParameters.TelecommutingPercentage
                        oCurrentLabAgree.TelecommutingMaxDays = 0
                    Else
                        oCurrentLabAgree.TelecommutingMaxDays = oParameters.TelecommutingMaxOptionalDays
                        oCurrentLabAgree.TelecommutingMaxPercentage = 0
                    End If
                Else
                    oCurrentLabAgree.Telecommuting = False
                End If

                If roTypes.Any2Boolean(HelperSession.AdvancedParametersCache("Latam.OvertimeManagement")) Then
                    Select Case True
                        Case Me.ckExtraDisabled.Value
                            oCurrentLabAgree.ExtraHoursConfiguration = LabAgreeExtraHoursConfiguration.Disabled
                        Case Me.ckExtra3x3.Value
                            oCurrentLabAgree.ExtraHoursConfiguration = LabAgreeExtraHoursConfiguration.ThreeByThree
                        Case Me.ckExtra9acc.Value
                            oCurrentLabAgree.ExtraHoursConfiguration = LabAgreeExtraHoursConfiguration.NineAcc
                    End Select
                    oCurrentLabAgree.ExtraHoursIDCauseSimples = tbExtras.Value
                    oCurrentLabAgree.ExtraHoursIDCauseDoubles = cmbCausesDbl.Value
                    oCurrentLabAgree.ExtraHoursIDCauseTriples = cmbCausesTpl.Value
                End If

                If API.LabAgreeServiceMethods.SaveLabAgree(Me, oCurrentLabAgree, True) Then

                    Dim oNewSchedulingRules As roScheduleRule() = ParseClientSchedulerRules(oCurrentLabAgree)

                    API.ScheduleRulesServiceMethods.SaveLabAgreeScheduleRules(Me.Page, oNewSchedulingRules, oCurrentLabAgree.ID)

                    strError = API.ScheduleRulesServiceMethods.LastErrorText

                    Dim treePath As String = "/source/" & oCurrentLabAgree.ID
                    HelperWeb.roSelector_SetSelection(oCurrentLabAgree.ID.ToString, treePath, "ctl00_contentMainBody_roTreesLabAgrees")
                Else
                    strError = API.LabAgreeServiceMethods.LastErrorText
                    End If
                End If
        Catch ex As Exception
            strError = ex.Message
        Finally
            If strError <> String.Empty Then
                LoadLabAgree(oParameters, oCurrentLabAgree)
                strError = "MESSAGE" &
                            "TitleKey=Error.SaveLabAgree.Title&" +
                            "DescriptionText=" + strError + "&" +
                            "Option1TextKey=Error.SaveLabAgree.Option1Text&" +
                            "Option1DescriptionKey=Error.SaveLabAgree.Option1Description&" +
                            "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                            "IconUrl=" & HelperWeb.MsgBoxIconsUrl(HelperWeb.MsgBoxIcons.ErrorIcon)

                ASPxCallbackPanelContenido.JSProperties("cpActionRO") = "SAVELABAGREE"
                ASPxCallbackPanelContenido.JSProperties("cpResultRO") = strError
            Else
                LoadLabAgree(oParameters, oCurrentLabAgree)
                ASPxCallbackPanelContenido.JSProperties("cpIsNew") = True
            End If
        End Try

    End Sub

    Protected Sub SetLabAgreeRulesData()
        Dim oMaxAnualHours As roScheduleRule = Me.LabAgreeSystemScheduleRulesValuesData.Find(Function(x) x.IDRule = ScheduleRuleType.MinMaxExpectedHours)
        If oMaxAnualHours IsNot Nothing Then

            If (CType(oMaxAnualHours, roScheduleRule_MinMaxExpectedHours).MaximumEmployeeField <> "") Then
                Me.ckUserField.Checked = True
                Me.ckFixedValue.Checked = False
                'Me.divcmbUserField.Visible = True
                'Me.divtxtYearHours.Visible = False
            Else
                Me.ckFixedValue.Checked = True
                Me.ckUserField.Checked = False
                'Me.divcmbUserField.Visible = False
                'Me.divtxtYearHours.Visible = True
            End If

            Me.txtYearHours.Value = CType(oMaxAnualHours, roScheduleRule_MinMaxExpectedHours).MaximumWorkingHours

            Me.txtFork.Value = CType(oMaxAnualHours, roScheduleRule_MinMaxExpectedHours).MaximumWorkingHoursFork
            Dim item = Me.cmbUserField.Items.FindByText(CType(oMaxAnualHours, roScheduleRule_MinMaxExpectedHours).MaximumEmployeeField)
            Me.cmbUserField.SelectedItem = item
        Else
            Me.txtYearHours.Value = ""
            Me.txtFork.Value = 0
        End If

        Dim oMaxYearHolidays As roScheduleRule = Me.LabAgreeSystemScheduleRulesValuesData.Find(Function(x) x.IDRule = ScheduleRuleType.MaxHolidays)
        If oMaxYearHolidays IsNot Nothing Then
            Me.txtYearHolidays.Value = CType(oMaxYearHolidays, roScheduleRule_MaxHolidays).MaximumHoliDays
        Else
            Me.txtYearHolidays.Value = 0
        End If

        Dim oWorkOnHolidays As roScheduleRule = Me.LabAgreeSystemScheduleRulesValuesData.Find(Function(x) x.IDRule = ScheduleRuleType.WorkOnFestive)
        If oWorkOnHolidays IsNot Nothing Then
            Me.chkCanWorkOnFeastDays.Checked = Not CType(oWorkOnHolidays, roScheduleRule_WorkOnFestive).Enabled
        Else
            Me.chkCanWorkOnFeastDays.Checked = 0
        End If

        Dim oWorkOnWeekend As roScheduleRule = Me.LabAgreeSystemScheduleRulesValuesData.Find(Function(x) x.IDRule = ScheduleRuleType.WorkOnWeekend)
        If oWorkOnWeekend IsNot Nothing Then
            Me.chkCanWorkOnNonWorkingDays.Checked = Not CType(oWorkOnWeekend, roScheduleRule_WorkOnWeekend).Enabled
            Me.tbWorkingDays.Value = CType(oWorkOnWeekend, roScheduleRule_WorkOnWeekend).LabourDaysIndex
        Else
            Me.chkCanWorkOnNonWorkingDays.Checked = 0
            Me.tbWorkingDays.Value = "1,2,3,4,5"
        End If
    End Sub

    Protected Function ParseClientSchedulerRules(ByVal oCurrentLabAgree As roLabAgree) As roScheduleRule()
        Dim oNewSchedulingRules As New Generic.List(Of roScheduleRule)
        oNewSchedulingRules.AddRange(Me.LabAgreeScheduleRulesValuesData().ToArray)

        Dim oMaxAnualHours As roScheduleRule = Me.LabAgreeSystemScheduleRulesValuesData.Find(Function(x) x.IDRule = ScheduleRuleType.MinMaxExpectedHours)

        If Me.ckUserField.Checked Then
            Me.txtYearHours.Value = 0
        Else
            Me.cmbUserField.SelectedIndex = -1
        End If

        If oMaxAnualHours Is Nothing Then
            oMaxAnualHours = New roScheduleRule_MinMaxExpectedHours With {
                .Id = -1,
                .IDRule = ScheduleRuleType.MinMaxExpectedHours,
                .Enabled = True,
                .IdLabAgree = oCurrentLabAgree.ID,
                .MaximumWorkingHours = roTypes.Any2Integer(Me.txtYearHours.Value),
                .MinimumWorkingHours = 0,
                .MaximumWorkingHoursFork = roTypes.Any2Integer(Me.txtFork.Value),
                .MaximumEmployeeField = If(Me.cmbUserField.SelectedIndex = -1, "", roTypes.Any2String(Me.cmbUserField.SelectedItem.Value)),
                .MinimumEmployeeField = String.Empty
            }
        Else
            CType(oMaxAnualHours, roScheduleRule_MinMaxExpectedHours).MaximumWorkingHours = roTypes.Any2Integer(Me.txtYearHours.Value)
            CType(oMaxAnualHours, roScheduleRule_MinMaxExpectedHours).MaximumWorkingHoursFork = roTypes.Any2Integer(Me.txtFork.Value)
            If Me.cmbUserField.SelectedIndex = -1 Then
                CType(oMaxAnualHours, roScheduleRule_MinMaxExpectedHours).MaximumEmployeeField = ""
            Else
                CType(oMaxAnualHours, roScheduleRule_MinMaxExpectedHours).MaximumEmployeeField = roTypes.Any2String(Me.cmbUserField.SelectedItem.Value)
            End If
        End If

        oNewSchedulingRules.Add(oMaxAnualHours)

        Dim oMaxYearHolidays As roScheduleRule = Me.LabAgreeSystemScheduleRulesValuesData.Find(Function(x) x.IDRule = ScheduleRuleType.MaxHolidays)
        If oMaxYearHolidays Is Nothing Then
            oMaxYearHolidays = New roScheduleRule_MaxHolidays With {
                .Id = -1,
                .IDRule = ScheduleRuleType.MaxHolidays,
                .Enabled = True,
                .IdLabAgree = oCurrentLabAgree.ID,
                .MaximumHoliDays = roTypes.Any2Integer(txtYearHolidays.Value)
            }
        Else
            CType(oMaxYearHolidays, roScheduleRule_MaxHolidays).MaximumHoliDays = roTypes.Any2Integer(Me.txtYearHolidays.Value)
        End If
        oNewSchedulingRules.Add(oMaxYearHolidays)

        Dim oWorkOnHolidays As roScheduleRule = Me.LabAgreeSystemScheduleRulesValuesData.Find(Function(x) x.IDRule = ScheduleRuleType.WorkOnFestive)
        If oWorkOnHolidays Is Nothing Then
            oWorkOnHolidays = New roScheduleRule_WorkOnFestive With {
                .Id = -1,
                .IDRule = ScheduleRuleType.WorkOnFestive,
                .Enabled = Not chkCanWorkOnFeastDays.Checked,
                .IdLabAgree = oCurrentLabAgree.ID
            }
        Else
            CType(oWorkOnHolidays, roScheduleRule_WorkOnFestive).Enabled = Not chkCanWorkOnFeastDays.Checked
        End If
        oNewSchedulingRules.Add(oWorkOnHolidays)

        Dim oWorkOnWeekend As roScheduleRule = Me.LabAgreeSystemScheduleRulesValuesData.Find(Function(x) x.IDRule = ScheduleRuleType.WorkOnWeekend)
        If oWorkOnWeekend Is Nothing Then
            oWorkOnWeekend = New roScheduleRule_WorkOnWeekend With {
                .Id = -1,
                .IDRule = ScheduleRuleType.WorkOnWeekend,
                .Enabled = Not chkCanWorkOnNonWorkingDays.Checked,
                .IdLabAgree = oCurrentLabAgree.ID,
                .LabourDaysIndex = tbWorkingDays.Value
            }
        Else
            CType(oWorkOnWeekend, roScheduleRule_WorkOnWeekend).Enabled = Not chkCanWorkOnNonWorkingDays.Checked
            CType(oWorkOnWeekend, roScheduleRule_WorkOnWeekend).LabourDaysIndex = tbWorkingDays.Value
        End If
        oNewSchedulingRules.Add(oWorkOnWeekend)

        Return oNewSchedulingRules.ToArray
    End Function

#End Region

    Protected Sub btnAddNeLabAgreeRule_Init(ByVal sender As Object, ByVal e As EventArgs)
        Dim txtLabel As ASPxButton = CType(sender, ASPxButton)
        txtLabel.Text = Me.Language.Translate("Button.Title.NewRule", DefaultScope)
        txtLabel.ToolTip = ""
    End Sub

    Protected Sub btnAddNewStartupValue_Init(ByVal sender As Object, ByVal e As EventArgs)
        Dim txtLabel As ASPxButton = CType(sender, ASPxButton)
        txtLabel.Text = Me.Language.Translate("Button.Title.NewStartupValue", DefaultScope)
        txtLabel.ToolTip = ""
    End Sub

    Protected Sub lblStartupValueCaption_Init(ByVal sender As Object, ByVal e As EventArgs)
        Dim txtLabel As ASPxLabel = CType(sender, ASPxLabel)
        txtLabel.Text = Me.Language.Translate("grdStartup.Title", DefaultScope)
        txtLabel.ToolTip = ""
    End Sub

    Protected Sub lblLabAgreeRuleCaption_Init(ByVal sender As Object, ByVal e As EventArgs)
        Dim txtLabel As ASPxLabel = CType(sender, ASPxLabel)
        txtLabel.Text = Me.Language.Translate("grdRule.Title", DefaultScope)
        txtLabel.ToolTip = ""
    End Sub

    Protected Sub lblLabAgreeScheduleRule_Init(ByVal sender As Object, ByVal e As EventArgs)
        Dim txtLabel As ASPxLabel = CType(sender, ASPxLabel)
        txtLabel.Text = Me.Language.Translate("grdScheduleRule.Title", DefaultScope)
        txtLabel.ToolTip = ""
    End Sub

#Region "Grid LagAgreeRules"

    Private Sub CreateLabAgreeRulesColumns()

        Dim GridColumn As GridViewDataTextColumn
        Dim GridColumnCommand As GridViewCommandColumn
        Dim GridColumnDate As GridViewDataDateColumn

        Dim VisibleIndex As Integer = 0

        Me.GridLabAgreeRules.Columns.Clear()
        Me.GridLabAgreeRules.KeyFieldName = "IDAccrualRule"
        Me.GridLabAgreeRules.SettingsText.EmptyDataRow = " "
        Me.GridLabAgreeRules.Settings.VerticalScrollBarMode = ScrollBarMode.Auto

        If Me.oPermission > Permission.Read Then
            Me.GridLabAgreeRules.SettingsEditing.Mode = GridViewEditingMode.Inline
        Else
            Dim uiControl As Control = Me.GridLabAgreeRules.FindTitleTemplateControl("btnAddNeLabAgreeRule")
            If uiControl IsNot Nothing Then
                uiControl.Visible = False
            End If
        End If

        'Clave
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "IDAccrualRule"
        GridColumn.FieldName = "IDAccrualRule"
        GridColumn.ReadOnly = False
        GridColumn.Visible = False
        Me.GridLabAgreeRules.Columns.Add(GridColumn)

        If Me.oPermission >= Permission.Write Then

            GridColumnCommand = New GridViewCommandColumn()
            GridColumnCommand.ButtonRenderMode = GridCommandButtonRenderMode.Image

            Dim editButton As GridViewCommandColumnCustomButton = New GridViewCommandColumnCustomButton()
            editButton.ID = "ShowDetailButton"
            editButton.Image.Url = "~/Base/Images/Grid/edit.png"
            editButton.Text = Me.Language.Translate("GridLabAgreeRules.Column.Edit", DefaultScope) 'Mostrar detalles"

            GridColumnCommand.CustomButtons.Add(editButton)
            GridColumnCommand.ShowEditButton = False
            GridColumnCommand.ShowDeleteButton = False
            GridColumnCommand.Caption = " "
            GridColumnCommand.VisibleIndex = VisibleIndex
            GridColumnCommand.CellStyle.HorizontalAlign = HorizontalAlign.Center
            GridColumnCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
            GridColumnCommand.Width = 24
            VisibleIndex = VisibleIndex + 1

            Me.GridLabAgreeRules.Columns.Add(GridColumnCommand)

        End If

        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridLabAgreeRules.Column.LabAgree", DefaultScope) '"Fecha"
        GridColumn.FieldName = "IDAccrualRule"
        GridColumn.Visible = False
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        Me.GridLabAgreeRules.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridLabAgreeRules.Column.LabAgree", DefaultScope) '"Fecha"
        GridColumn.FieldName = "AccrualRule"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.UnboundType = DevExpress.Data.UnboundColumnType.String
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        Me.GridLabAgreeRules.Columns.Add(GridColumn)

        'Date
        GridColumnDate = New GridViewDataDateColumn()
        GridColumnDate.Caption = Me.Language.Translate("GridLabAgreeRules.Column.BeginDate", DefaultScope) '"Fecha"
        GridColumnDate.FieldName = "BeginDate"
        GridColumnDate.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnDate.ReadOnly = False
        GridColumnDate.Width = 100
        GridColumnDate.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnDate.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        Me.GridLabAgreeRules.Columns.Add(GridColumnDate)

        'Date
        GridColumnDate = New GridViewDataDateColumn()
        GridColumnDate.Caption = Me.Language.Translate("GridLabAgreeRules.Column.EndDate", DefaultScope) '"Fecha"
        GridColumnDate.FieldName = "EndDate"
        GridColumnDate.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnDate.ReadOnly = False
        GridColumnDate.Width = 100
        GridColumnDate.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnDate.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        Me.GridLabAgreeRules.Columns.Add(GridColumnDate)

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
            GridColumnCommand.Width = 24
            VisibleIndex = VisibleIndex + 1

            Me.GridLabAgreeRules.Columns.Add(GridColumnCommand)
        End If

    End Sub

    Private Sub BindGridLabAgreeRules(ByVal bolReload As Boolean)
        Me.GridLabAgreeRules.DataSource = Me.LabAgreedRulesData(bolReload)
        Me.GridLabAgreeRules.DataBind()
    End Sub

    Protected Sub GridLabAgreeRules_CustomUnboundColumnData(sender As Object, e As ASPxGridViewColumnDataEventArgs) Handles GridLabAgreeRules.CustomUnboundColumnData
        Select Case e.Column.FieldName
            Case "AccrualRule"
                If e.IsGetData Then
                    If e.GetListSourceFieldValue("IDAccrualRule") IsNot System.DBNull.Value Then
                        Dim oItem As roLabAgreeAccrualRule = getAccrualRule(roTypes.Any2Integer(e.GetListSourceFieldValue("IDAccrualRule")))
                        If oItem IsNot Nothing Then e.Value = oItem.LabAgreeRule.Name
                    End If
                End If
        End Select
    End Sub

    Protected Sub GridLabAgreeRules_CustomCallback(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxGridViewCustomCallbackEventArgs) Handles GridLabAgreeRules.CustomCallback
        If e.Parameters = "REFRESH" Then
            BindGridLabAgreeRules(False)
            GridLabAgreeRules.JSProperties("cpAction") = "REFRESH"
        ElseIf e.Parameters = "RELOAD" Then
            BindGridLabAgreeRules(True)
            GridLabAgreeRules.JSProperties("cpAction") = "RELOAD"
        End If
    End Sub

    Protected Sub GridLabAgreeRules_RowDeleting(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataDeletingEventArgs) Handles GridLabAgreeRules.RowDeleting
        Dim grid As ASPxGridView = CType(sender, ASPxGridView)
        Dim tb As Generic.List(Of roLabAgreeAccrualRule) = Me.LabAgreedRulesData()

        If e.Values("IDAccrualRule") > 0 OrElse e.Values("IDAccrualRule") < -1 Then
            Dim selObject As roLabAgreeAccrualRule = Nothing
            For Each oObject As roLabAgreeAccrualRule In tb
                If oObject.IDAccrualRule = e.Values("IDAccrualRule") Then
                    selObject = oObject
                    Exit For
                End If
            Next

            If selObject IsNot Nothing Then
                tb.Remove(selObject)
            End If

            Me.LabAgreedRulesData = tb
            e.Cancel = True

        End If

        BindGridLabAgreeRules(False)
        GridLabAgreeRules.JSProperties("cpAction") = "ROWDELETE"

    End Sub

#End Region

#Region "Grid LabAgreeCauseLimit"

    Private Sub CreateLabAgreeCauseLimitsColumns()

        Dim GridColumn As GridViewDataTextColumn
        Dim GridColumnCommand As GridViewCommandColumn
        Dim GridColumnDate As GridViewDataDateColumn

        Dim VisibleIndex As Integer = 0

        Me.GridLabAgreeCauseLimit.Columns.Clear()
        Me.GridLabAgreeCauseLimit.KeyFieldName = "IDCauseLimitValue"
        Me.GridLabAgreeCauseLimit.SettingsText.EmptyDataRow = " "
        Me.GridLabAgreeCauseLimit.Settings.VerticalScrollBarMode = ScrollBarMode.Auto

        If Me.oPermission > Permission.Read Then
            Me.GridLabAgreeCauseLimit.SettingsEditing.Mode = GridViewEditingMode.Inline
        Else
            Dim uiControl As Control = Me.GridLabAgreeCauseLimit.FindTitleTemplateControl("btnAddNeLabAgreeRule")
            If uiControl IsNot Nothing Then
                uiControl.Visible = False
            End If
        End If

        'Clave
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "IDCauseLimitValue"
        GridColumn.FieldName = "IDCauseLimitValue"
        GridColumn.ReadOnly = False
        GridColumn.Visible = False
        Me.GridLabAgreeCauseLimit.Columns.Add(GridColumn)

        If Me.oPermission >= Permission.Write Then

            GridColumnCommand = New GridViewCommandColumn()
            GridColumnCommand.ButtonRenderMode = GridCommandButtonRenderMode.Image

            Dim editButton As GridViewCommandColumnCustomButton = New GridViewCommandColumnCustomButton()
            editButton.ID = "ShowDetailButton"
            editButton.Image.Url = "~/Base/Images/Grid/edit.png"
            editButton.Text = Me.Language.Translate("GridLabAgreeCauseLimit.Column.Edit", DefaultScope) 'Mostrar detalles"

            GridColumnCommand.CustomButtons.Add(editButton)
            GridColumnCommand.ShowEditButton = False
            GridColumnCommand.ShowDeleteButton = False
            GridColumnCommand.Caption = " "
            GridColumnCommand.VisibleIndex = VisibleIndex
            GridColumnCommand.CellStyle.HorizontalAlign = HorizontalAlign.Center
            GridColumnCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
            GridColumnCommand.Width = 24
            VisibleIndex = VisibleIndex + 1

            Me.GridLabAgreeCauseLimit.Columns.Add(GridColumnCommand)

        End If

        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridLabAgreeCauseLimit.Column.CauseLimit", DefaultScope) '"Fecha"
        GridColumn.FieldName = "IDCauseLimitValue"
        GridColumn.Visible = False
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        Me.GridLabAgreeCauseLimit.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridLabAgreeCauseLimit.Column.CauseLimit", DefaultScope) '"Fecha"
        GridColumn.FieldName = "CauseLimit"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.UnboundType = DevExpress.Data.UnboundColumnType.String
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        Me.GridLabAgreeCauseLimit.Columns.Add(GridColumn)

        'Date
        GridColumnDate = New GridViewDataDateColumn()
        GridColumnDate.Caption = Me.Language.Translate("GridLabAgreeCauseLimit.Column.BeginDate", DefaultScope) '"Fecha"
        GridColumnDate.FieldName = "BeginDate"
        GridColumnDate.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnDate.ReadOnly = False
        GridColumnDate.Width = 100
        GridColumnDate.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnDate.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        Me.GridLabAgreeCauseLimit.Columns.Add(GridColumnDate)

        'Date
        GridColumnDate = New GridViewDataDateColumn()
        GridColumnDate.Caption = Me.Language.Translate("GridLabAgreeCauseLimit.Column.EndDate", DefaultScope) '"Fecha"
        GridColumnDate.FieldName = "EndDate"
        GridColumnDate.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnDate.ReadOnly = False
        GridColumnDate.Width = 100
        GridColumnDate.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnDate.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        Me.GridLabAgreeCauseLimit.Columns.Add(GridColumnDate)

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
            GridColumnCommand.Width = 24
            VisibleIndex = VisibleIndex + 1

            Me.GridLabAgreeCauseLimit.Columns.Add(GridColumnCommand)
        End If

    End Sub

    Private Sub BindGridLabAgreeCauseLimits(ByVal bolReload As Boolean)
        Me.GridLabAgreeCauseLimit.DataSource = Me.LabAgreedCausesLimitData(bolReload)
        Me.GridLabAgreeCauseLimit.DataBind()
    End Sub

    Protected Sub GridLabAgreeCauseLimit_CustomUnboundColumnData(sender As Object, e As ASPxGridViewColumnDataEventArgs) Handles GridLabAgreeCauseLimit.CustomUnboundColumnData
        Select Case e.Column.FieldName
            Case "CauseLimit"
                If e.IsGetData Then
                    If e.GetListSourceFieldValue("IDCauseLimitValue") IsNot System.DBNull.Value Then
                        Dim oItem As roLabAgreeCauseLimitValues = getCauseLimitValue(roTypes.Any2Integer(e.GetListSourceFieldValue("IDCauseLimitValue")))
                        If oItem IsNot Nothing Then e.Value = oItem.CauseLimitValue.Name
                    End If
                End If
        End Select
    End Sub

    Protected Sub GridLabAgreeCauseLimit_CustomCallback(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxGridViewCustomCallbackEventArgs) Handles GridLabAgreeCauseLimit.CustomCallback
        If e.Parameters = "REFRESH" Then
            BindGridLabAgreeCauseLimits(False)
            GridLabAgreeCauseLimit.JSProperties("cpAction") = "REFRESH"
        ElseIf e.Parameters = "RELOAD" Then
            BindGridLabAgreeCauseLimits(True)
            GridLabAgreeCauseLimit.JSProperties("cpAction") = "RELOAD"
        End If
    End Sub

    Protected Sub GridLabAgreeCauseLimit_RowDeleting(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataDeletingEventArgs) Handles GridLabAgreeCauseLimit.RowDeleting
        Dim grid As ASPxGridView = CType(sender, ASPxGridView)
        Dim tb As Generic.List(Of roLabAgreeCauseLimitValues) = Me.LabAgreedCausesLimitData()

        If e.Values("IDCauseLimitValue") > 0 OrElse e.Values("IDCauseLimitValue") < -1 Then
            Dim selObject As roLabAgreeCauseLimitValues = Nothing
            For Each oObject As roLabAgreeCauseLimitValues In tb
                If oObject.IDCauseLimitValue = e.Values("IDCauseLimitValue") Then
                    selObject = oObject
                    Exit For
                End If
            Next

            If selObject IsNot Nothing Then
                tb.Remove(selObject)
            End If

            Me.LabAgreedCausesLimitData = tb
            e.Cancel = True

        End If

        BindGridLabAgreeCauseLimits(False)
        GridLabAgreeCauseLimit.JSProperties("cpAction") = "ROWDELETE"
    End Sub

#End Region

#Region "Grid StartupValues"

    Private Sub CreateStartupValuesColumns()

        Dim GridColumn As GridViewDataTextColumn
        Dim GridColumnCommand As GridViewCommandColumn

        Dim VisibleIndex As Integer = 0

        Me.GridStartupValues.Columns.Clear()
        Me.GridStartupValues.KeyFieldName = "ID"
        Me.GridStartupValues.SettingsText.EmptyDataRow = " "
        Me.GridStartupValues.Settings.VerticalScrollBarMode = ScrollBarMode.Auto

        If Me.oPermission = Permission.Admin Or Me.oPermission >= Permission.Write Then
            Me.GridStartupValues.SettingsEditing.Mode = GridViewEditingMode.Inline
        Else
            Dim uiControl As Control = Me.GridStartupValues.FindTitleTemplateControl("btnAddNewStartupValue")
            If uiControl IsNot Nothing Then
                uiControl.Visible = False
            End If
        End If

        'Clave
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "Name"
        GridColumn.FieldName = "ID"
        GridColumn.ReadOnly = False
        GridColumn.Visible = False
        Me.GridStartupValues.Columns.Add(GridColumn)

        Dim obj As New roStartupValue

        If Me.oPermission >= Permission.Write Then

            GridColumnCommand = New GridViewCommandColumn()
            GridColumnCommand.ButtonRenderMode = GridCommandButtonRenderMode.Image

            Dim editButton As GridViewCommandColumnCustomButton = New GridViewCommandColumnCustomButton()
            editButton.ID = "ShowDetailButton"
            editButton.Image.Url = "~/Base/Images/Grid/edit.png"
            editButton.Text = Me.Language.Translate("GridLabAgreeRules.Column.Edit", DefaultScope) 'Mostrar detalles"

            GridColumnCommand.CustomButtons.Add(editButton)
            GridColumnCommand.ShowEditButton = False
            GridColumnCommand.ShowDeleteButton = False
            GridColumnCommand.Caption = " "
            GridColumnCommand.VisibleIndex = VisibleIndex
            GridColumnCommand.CellStyle.HorizontalAlign = HorizontalAlign.Center
            GridColumnCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
            GridColumnCommand.Width = 32
            VisibleIndex = VisibleIndex + 1

            Me.GridStartupValues.Columns.Add(GridColumnCommand)

        End If

        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridLabAgreeRules.Column.LabAgree", DefaultScope) '"Fecha"
        GridColumn.FieldName = "ID"
        GridColumn.Visible = False
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        Me.GridStartupValues.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridLabAgreeRules.Column.LabAgree", DefaultScope) '"Fecha"
        GridColumn.FieldName = "IDConcept"
        GridColumn.Visible = False
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        Me.GridStartupValues.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridLabAgreeStartupValues.Column.Name", DefaultScope) '"Fecha"
        GridColumn.FieldName = "StartupValue"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.UnboundType = DevExpress.Data.UnboundColumnType.String
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        Me.GridStartupValues.Columns.Add(GridColumn)

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

            Me.GridStartupValues.Columns.Add(GridColumnCommand)
        End If

    End Sub

    Protected Sub GridStartupValues_CustomUnboundColumnData(sender As Object, e As ASPxGridViewColumnDataEventArgs) Handles GridStartupValues.CustomUnboundColumnData
        Select Case e.Column.FieldName
            Case "StartupValue"
                If e.IsGetData Then
                    If e.GetListSourceFieldValue("ID") IsNot System.DBNull.Value Then
                        Dim oItem As roStartupValue = getStartupValue(roTypes.Any2Integer(e.GetListSourceFieldValue("ID")))
                        If oItem IsNot Nothing Then e.Value = oItem.Name
                    End If
                End If

        End Select
    End Sub

    Private Sub BindGridStartupValuesRules(ByVal bolReload As Boolean)
        Me.GridStartupValues.DataSource = Me.LabAgreeStartUpValuesData(bolReload)
        Me.GridStartupValues.DataBind()
    End Sub

    Protected Sub GridStartupValues_CustomCallback(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxGridViewCustomCallbackEventArgs) Handles GridStartupValues.CustomCallback
        If e.Parameters = "REFRESH" Then
            BindGridStartupValuesRules(False)
            GridStartupValues.JSProperties("cpAction") = "REFRESH"
        ElseIf e.Parameters = "RELOAD" Then
            BindGridStartupValuesRules(True)
            GridStartupValues.JSProperties("cpAction") = "RELOAD"
        End If
    End Sub

    Protected Sub GridStartupValues_RowDeleting(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataDeletingEventArgs) Handles GridStartupValues.RowDeleting
        Dim grid As ASPxGridView = CType(sender, ASPxGridView)
        Dim tb As Generic.List(Of roStartupValue) = Me.LabAgreeStartUpValuesData

        If e.Values("IDConcept") > 0 Then
            Dim selObject As roStartupValue = Nothing
            For Each oObject As roStartupValue In tb
                If oObject.IDConcept = e.Values("IDConcept") Then
                    selObject = oObject
                    Exit For
                End If
            Next

            If selObject IsNot Nothing Then
                tb.Remove(selObject)
            End If

            Me.LabAgreeStartUpValuesData = tb
            e.Cancel = True

        End If

        BindGridStartupValuesRules(False)
        GridStartupValues.JSProperties("cpAction") = "ROWDELETE"

    End Sub

#End Region

#Region "Grid RequestValidation"

    Private Sub CreateRequestValidationColumns()

        Dim GridColumn As GridViewDataTextColumn
        Dim GridColumnCommand As GridViewCommandColumn
        Dim GridCheckCommand As GridViewDataCheckColumn

        Dim VisibleIndex As Integer = 0

        Me.GridLabAgreeRequestValidation.Columns.Clear()
        Me.GridLabAgreeRequestValidation.KeyFieldName = "IDRule"
        Me.GridLabAgreeRequestValidation.SettingsText.EmptyDataRow = " "
        Me.GridLabAgreeRequestValidation.Settings.VerticalScrollBarMode = ScrollBarMode.Auto
        Me.GridLabAgreeRequestValidation.SettingsBehavior.AllowSort = True

        If Me.oPermission = Permission.Admin Or Me.oPermission >= Permission.Write Then
            Me.GridLabAgreeRequestValidation.SettingsEditing.Mode = GridViewEditingMode.Inline
        Else
            Dim uiControl As Control = Me.GridLabAgreeRequestValidation.FindTitleTemplateControl("btnAddNewLabAgreeRequestValidation")
            If uiControl IsNot Nothing Then
                uiControl.Visible = False
            End If
        End If

        'Clave
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "Name"
        GridColumn.FieldName = "IDRule"
        GridColumn.ReadOnly = False
        GridColumn.Visible = False
        Me.GridLabAgreeRequestValidation.Columns.Add(GridColumn)

        Dim obj As New roStartupValue

        If Me.oPermission >= Permission.Write Then

            GridColumnCommand = New GridViewCommandColumn()
            GridColumnCommand.ButtonRenderMode = GridCommandButtonRenderMode.Image

            Dim editButton As GridViewCommandColumnCustomButton = New GridViewCommandColumnCustomButton()
            editButton.ID = "ShowDetailButton"
            editButton.Image.Url = "~/Base/Images/Grid/edit.png"
            editButton.Text = Me.Language.Translate("GridLabAgreeRequestValidation.Column.Edit", DefaultScope) 'Mostrar detalles"

            GridColumnCommand.CustomButtons.Add(editButton)
            GridColumnCommand.ShowEditButton = False
            GridColumnCommand.ShowDeleteButton = False
            GridColumnCommand.Caption = " "
            GridColumnCommand.VisibleIndex = VisibleIndex
            GridColumnCommand.CellStyle.HorizontalAlign = HorizontalAlign.Center
            GridColumnCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
            GridColumnCommand.Width = 32
            VisibleIndex = VisibleIndex + 1

            Me.GridLabAgreeRequestValidation.Columns.Add(GridColumnCommand)

        End If

        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridLabAgreeRequestValidation.Column.LabAgree", DefaultScope) '"Fecha"
        GridColumn.FieldName = "ID"
        GridColumn.Visible = False
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        Me.GridLabAgreeRequestValidation.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridLabAgreeRequestValidation.Column.LabAgree", DefaultScope) '"Fecha"
        GridColumn.FieldName = "IDRule"
        GridColumn.Visible = False
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        Me.GridLabAgreeRequestValidation.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridLabAgreeRequestValidation.Column.Name", DefaultScope) '"Fecha"
        GridColumn.FieldName = "RuleValidation"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.UnboundType = DevExpress.Data.UnboundColumnType.String
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        Me.GridLabAgreeRequestValidation.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridLabAgreeRequestValidation.Column.RequestType", DefaultScope) '"Fecha"
        GridColumn.FieldName = "RequestTypeDesc"
        GridColumn.VisibleIndex = VisibleIndex
        GridColumn.SortIndex = 0
        GridColumn.SortOrder = DevExpress.Data.ColumnSortOrder.Ascending
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.UnboundType = DevExpress.Data.UnboundColumnType.String
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        Me.GridLabAgreeRequestValidation.Columns.Add(GridColumn)

        GridCheckCommand = New GridViewDataCheckColumn()
        GridCheckCommand.Caption = Me.Language.Translate("GridLabAgreeRequestValidation.Column.AutomaticRule", DefaultScope) '"Fecha"
        GridCheckCommand.FieldName = "IsAutomatic"
        GridCheckCommand.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridCheckCommand.ReadOnly = True
        GridCheckCommand.UnboundType = DevExpress.Data.UnboundColumnType.Boolean
        GridCheckCommand.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridCheckCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridCheckCommand.Width = 80
        Me.GridLabAgreeRequestValidation.Columns.Add(GridCheckCommand)

        GridCheckCommand = New GridViewDataCheckColumn()
        GridCheckCommand.Caption = Me.Language.Translate("GridLabAgreeRequestValidation.Column.Active", DefaultScope) '"Fecha"
        GridCheckCommand.FieldName = "Activated"
        GridCheckCommand.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridCheckCommand.ReadOnly = True
        GridCheckCommand.UnboundType = DevExpress.Data.UnboundColumnType.String
        GridCheckCommand.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridCheckCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridCheckCommand.Width = 80
        Me.GridLabAgreeRequestValidation.Columns.Add(GridCheckCommand)

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

            Me.GridLabAgreeRequestValidation.Columns.Add(GridColumnCommand)
        End If

    End Sub

    Protected Sub GridLabAgreeRequestValidation_CustomUnboundColumnData(sender As Object, e As ASPxGridViewColumnDataEventArgs) Handles GridLabAgreeRequestValidation.CustomUnboundColumnData
        Select Case e.Column.FieldName
            Case "RuleValidation"
                If e.IsGetData Then
                    If e.GetListSourceFieldValue("IDRule") IsNot System.DBNull.Value Then
                        Dim oItem As roRequestRule = getRequestValidation(roTypes.Any2Integer(e.GetListSourceFieldValue("IDRule")))
                        If oItem IsNot Nothing Then e.Value = oItem.Name
                    End If
                End If
            Case "RequestTypeDesc"
                If e.IsGetData Then
                    If e.GetListSourceFieldValue("IDRule") IsNot System.DBNull.Value Then
                        Dim oItem As roRequestRule = getRequestValidation(roTypes.Any2Integer(e.GetListSourceFieldValue("IDRule")))
                        If oItem IsNot Nothing Then
                            For Each oRow As DataRow In GetAvailableRequestTypes().Rows
                                If oItem.IDRequestType = roTypes.Any2Integer(oRow("IdType")) Then
                                    e.Value = oRow("reqType")
                                End If
                            Next

                        End If
                    End If
                End If
            Case "IsAutomatic"
                If e.IsGetData Then
                    If e.GetListSourceFieldValue("IDRule") IsNot System.DBNull.Value Then
                        Dim oItem As roRequestRule = getRequestValidation(roTypes.Any2Integer(e.GetListSourceFieldValue("IDRule")))
                        e.Value = False
                        If oItem IsNot Nothing Then e.Value = (oItem.IDRuleType = 4)
                    End If
                End If
        End Select
    End Sub

    Private Sub BindGridLabAgreeRequestValidationRules(ByVal bolReload As Boolean)
        Me.GridLabAgreeRequestValidation.DataSource = Me.LabAgreeRequestValidationValuesData(bolReload)
        Me.GridLabAgreeRequestValidation.DataBind()
    End Sub

    Protected Sub GridLabAgreeRequestValidation_CustomCallback(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxGridViewCustomCallbackEventArgs) Handles GridLabAgreeRequestValidation.CustomCallback
        If e.Parameters = "REFRESH" Then
            BindGridLabAgreeRequestValidationRules(False)
            GridLabAgreeRequestValidation.JSProperties("cpAction") = "REFRESH"
        ElseIf e.Parameters = "RELOAD" Then
            BindGridLabAgreeRequestValidationRules(True)
            GridLabAgreeRequestValidation.JSProperties("cpAction") = "RELOAD"
        End If
    End Sub

    Protected Sub GridLabAgreeRequestValidation_RowDeleting(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataDeletingEventArgs) Handles GridLabAgreeRequestValidation.RowDeleting
        Dim grid As ASPxGridView = CType(sender, ASPxGridView)
        Dim tb As Generic.List(Of roRequestRule) = Me.LabAgreeRequestValidationValuesData

        If e.Values("IDRule") > 0 OrElse e.Values("IDRule") < -1 Then
            Dim selObject As roRequestRule = Nothing
            For Each oObject As roRequestRule In tb
                If oObject.IDRule = e.Values("IDRule") Then
                    selObject = oObject
                    Exit For
                End If
            Next

            If selObject IsNot Nothing Then
                tb.Remove(selObject)
            End If

            Me.LabAgreeRequestValidationValuesData = tb
            e.Cancel = True

        End If

        BindGridLabAgreeRequestValidationRules(False)
        GridLabAgreeRequestValidation.JSProperties("cpAction") = "ROWDELETE"

    End Sub

#End Region

#Region "Grid ScheduleRules"

    Private Sub CreateScheduleRulesColumns()

        Dim GridColumn As GridViewDataTextColumn
        Dim GridColumnCommand As GridViewCommandColumn
        Dim GridCheckCommand As GridViewDataCheckColumn

        Dim VisibleIndex As Integer = 0

        Me.GridLabAgreeScheduleRules.Columns.Clear()
        Me.GridLabAgreeScheduleRules.KeyFieldName = "Id"
        Me.GridLabAgreeScheduleRules.SettingsText.EmptyDataRow = " "
        Me.GridLabAgreeScheduleRules.Settings.VerticalScrollBarMode = ScrollBarMode.Auto

        If Me.oPermission = Permission.Admin Or Me.oPermission >= Permission.Write Then
            Me.GridLabAgreeScheduleRules.SettingsEditing.Mode = GridViewEditingMode.Inline
        Else
            Dim uiControl As Control = Me.GridLabAgreeScheduleRules.FindTitleTemplateControl("btnAddNewLabAgreeScheduleRules")
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
        Me.GridLabAgreeScheduleRules.Columns.Add(GridColumn)

        Dim obj As New roStartupValue

        If Me.oPermission >= Permission.Write Then

            GridColumnCommand = New GridViewCommandColumn()
            GridColumnCommand.ButtonRenderMode = GridCommandButtonRenderMode.Image

            Dim editButton As GridViewCommandColumnCustomButton = New GridViewCommandColumnCustomButton()
            editButton.ID = "ShowDetailButton"
            editButton.Image.Url = "~/Base/Images/Grid/edit.png"
            editButton.Text = Me.Language.Translate("GridLabAgreeScheduleRules.Column.Edit", DefaultScope) 'Mostrar detalles"

            GridColumnCommand.CustomButtons.Add(editButton)
            GridColumnCommand.ShowEditButton = False
            GridColumnCommand.ShowDeleteButton = False
            GridColumnCommand.Caption = " "
            GridColumnCommand.VisibleIndex = VisibleIndex
            GridColumnCommand.CellStyle.HorizontalAlign = HorizontalAlign.Center
            GridColumnCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
            GridColumnCommand.Width = 32
            VisibleIndex = VisibleIndex + 1

            Me.GridLabAgreeScheduleRules.Columns.Add(GridColumnCommand)

        End If

        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridLabAgreeScheduleRules.Column.LabAgree", DefaultScope) '"Fecha"
        GridColumn.FieldName = "Id"
        GridColumn.Visible = False
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        Me.GridLabAgreeScheduleRules.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridLabAgreeScheduleRules.Column.LabAgree", DefaultScope) '"Fecha"
        GridColumn.FieldName = "IdLabAgree"
        GridColumn.Visible = False
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        Me.GridLabAgreeScheduleRules.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridLabAgreeScheduleRules.Column.Name", DefaultScope) '"Fecha"
        GridColumn.FieldName = "RuleName"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.UnboundType = DevExpress.Data.UnboundColumnType.String
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        Me.GridLabAgreeScheduleRules.Columns.Add(GridColumn)

        GridCheckCommand = New GridViewDataCheckColumn()
        GridCheckCommand.Caption = Me.Language.Translate("GridLabAgreeScheduleRules.Column.Active", DefaultScope) '"Fecha"
        GridCheckCommand.FieldName = "Enabled"
        GridCheckCommand.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridCheckCommand.ReadOnly = True
        GridCheckCommand.UnboundType = DevExpress.Data.UnboundColumnType.String
        GridCheckCommand.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridCheckCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridCheckCommand.Width = 80
        Me.GridLabAgreeScheduleRules.Columns.Add(GridCheckCommand)

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

            Me.GridLabAgreeScheduleRules.Columns.Add(GridColumnCommand)
        End If

    End Sub

    Protected Sub GridLabAgreeScheduleRules_CustomUnboundColumnData(sender As Object, e As ASPxGridViewColumnDataEventArgs) Handles GridLabAgreeScheduleRules.CustomUnboundColumnData
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

    Private Sub BindGridLabAgreeScheduleRules(ByVal bolReload As Boolean)
        Me.GridLabAgreeScheduleRules.DataSource = Me.LabAgreeScheduleRulesValuesData(bolReload)
        Me.GridLabAgreeScheduleRules.DataBind()
    End Sub

    Protected Sub GridLabAgreeScheduleRules_CustomCallback(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxGridViewCustomCallbackEventArgs) Handles GridLabAgreeScheduleRules.CustomCallback
        If e.Parameters = "REFRESH" Then
            BindGridLabAgreeScheduleRules(False)
            GridLabAgreeScheduleRules.JSProperties("cpAction") = "REFRESH"
        ElseIf e.Parameters = "RELOAD" Then
            BindGridLabAgreeScheduleRules(True)
            GridLabAgreeScheduleRules.JSProperties("cpAction") = "RELOAD"
        End If
    End Sub

    Protected Sub GridLabAgreeScheduleRules_RowDeleting(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataDeletingEventArgs) Handles GridLabAgreeScheduleRules.RowDeleting
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

        BindGridLabAgreeScheduleRules(False)
        GridLabAgreeScheduleRules.JSProperties("cpAction") = "ROWDELETE"

    End Sub

#End Region

#Region "Helper private methods"

    Private Function getAccrualRule(ByVal oId As Integer) As roLabAgreeAccrualRule
        Dim oObject As roLabAgreeAccrualRule = Nothing

        Dim oList As Generic.List(Of roLabAgreeAccrualRule) = LabAgreedRulesData()

        If oList IsNot Nothing AndAlso oList.Count > 0 Then
            For Each oElem As roLabAgreeAccrualRule In oList
                If oElem.IDAccrualRule = oId Then
                    oObject = oElem
                    Exit For
                End If
            Next
        End If

        Return oObject
    End Function

    Private Function getStartupValue(ByVal oId As Integer) As roStartupValue
        Dim oObject As roStartupValue = Nothing

        Dim oList As Generic.List(Of roStartupValue) = LabAgreeStartUpValuesData()

        If oList IsNot Nothing AndAlso oList.Count > 0 Then
            For Each oElem As roStartupValue In oList
                If oElem.ID = oId Then
                    oObject = oElem
                    Exit For
                End If
            Next
        End If

        Return oObject
    End Function

    Private Function getRequestValidation(ByVal oId As Integer) As roRequestRule
        Dim oObject As roRequestRule = Nothing

        Dim oList As Generic.List(Of roRequestRule) = LabAgreeRequestValidationValuesData

        If oList IsNot Nothing AndAlso oList.Count > 0 Then
            For Each oElem As roRequestRule In oList
                If oElem.IDRule = oId Then
                    oObject = oElem
                    Exit For
                End If
            Next
        End If

        Return oObject
    End Function

    Private Function getCauseLimitValue(ByVal oId As Integer) As roLabAgreeCauseLimitValues
        Dim oObject As roLabAgreeCauseLimitValues = Nothing

        Dim oList As Generic.List(Of roLabAgreeCauseLimitValues) = LabAgreedCausesLimitData()

        If oList IsNot Nothing AndAlso oList.Count > 0 Then
            For Each oElem As roLabAgreeCauseLimitValues In oList
                If oElem.IDCauseLimitValue = oId Then
                    oObject = oElem
                    Exit For
                End If
            Next
        End If

        Return oObject
    End Function

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