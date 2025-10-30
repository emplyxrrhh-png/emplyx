Imports DevExpress.Web
Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBusiness.BusinessCenter
Imports Robotics.Base.VTBusiness.Concept
Imports Robotics.Base.VTBusiness.Shift
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class Shiftsv2
    Inherits PageBase

    <Runtime.Serialization.DataContract()>
    Private Class ShiftsCallbackRequest

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

        <Runtime.Serialization.DataMember(Name:="resultRules")>
        Public resultRules As String

        <Runtime.Serialization.DataMember(Name:="resultLayers")>
        Public resultLayers As String

        <Runtime.Serialization.DataMember(Name:="resultTimeZones")>
        Public resultTimeZones As String

        <Runtime.Serialization.DataMember(Name:="resultAssignments")>
        Public resultAssignments As String

        <Runtime.Serialization.DataMember(Name:="checkVacationsEmpty")>
        Public checkVacationsEmpty As Boolean

        <Runtime.Serialization.DataMember(Name:="RecalcDate")>
        Public RecalcDate As String

        <Runtime.Serialization.DataMember(Name:="AllowComplementary")>
        Public AllowComplementaryHours As Boolean

        <Runtime.Serialization.DataMember(Name:="AllowFloatingData")>
        Public AllowFloatingData As Boolean

        <Runtime.Serialization.DataMember(Name:="BreakHours")>
        Public BreakHours As String

        <Runtime.Serialization.DataMember(Name:="DailyRules")>
        Public DailyRules As roShiftDailyRule()

        <Runtime.Serialization.DataMember(Name:="DRPatternPunches")>
        Public DRPatternPunches As String

    End Class

    Private Const FeatureAlias As String = "Shifts.Definition"
    Private oPermission As Permission

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.OverrrideDefaultScope = "Shifts"
        Me.InsertExtraJavascript("Ajax", "~/Base/Scripts/Ajax.js")
        Me.InsertExtraJavascript("jsDate", "~/Base/Scripts/jsDate.js")

        Me.InsertExtraJavascript("SGroups", "~/Shifts/Scripts/SGroupsV2.js")
        Me.InsertExtraJavascript("ShiftsGroupsV2", "~/Shifts/Scripts/ShiftsGroupsV2.js")
        Me.InsertExtraJavascript("Shifts", "~/Shifts/Scripts/ShiftsCompact.js")

        Me.InsertExtraJavascript("GenericData", "~/Base/Scripts/GenericData.js")

        Me.InsertExtraJavascript("webUserForms", "~/Base/Scripts/webUserForms.js")
        Me.InsertExtraJavascript("jsGrid", "~/Base/Scripts/jsGrid.js")
        Me.InsertExtraJavascript("roOptionPanelClient", "~/Base/Scripts/roOptionPanelClient.js")
        Me.InsertExtraJavascript("roOptPanelClientGroup", "~/Base/Scripts/roOptPanelClientGroup.js")
        Me.InsertExtraJavascript("roTimeLine", "~/Base/Scripts/roTimeLine.js")
        Me.InsertExtraJavascript("roTabContainerClient", "~/Base/Scripts/roTabContainerClient.js")
        Me.InsertExtraJavascript("frmAddZone", "~/Shifts/Scripts/frmAddZone.js")
        Me.InsertExtraJavascript("frmEditShiftFlexible", "~/Shifts/Scripts/frmEditShiftFlexible.js")
        Me.InsertExtraJavascript("frmEditShiftMandatory", "~/Shifts/Scripts/frmEditShiftMandatory.js")
        Me.InsertExtraJavascript("frmEditShiftBreak", "~/Shifts/Scripts/frmEditShiftBreak.js")
        Me.InsertExtraJavascript("frmNewTypeZone", "~/Shifts/Scripts/frmNewTypeZoneV2.js")
        Me.InsertExtraJavascript("frmDailyRule", "~/Shifts/Scripts/frmDailyRule.js")
        Me.InsertExtraJavascript("utils", "~/Base/Scripts/Live/utils.js")

    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.OverrrideDefaultScope = "Shifts"
        Me.InsertExtraCssIncludes("~/Shifts/Styles/roDailyRule.css")
        Me.InsertExtraCssIncludes("~/Shifts/Styles/roDailyRecord.css")

        ' Comprueba la licencia
        If HelperSession.GetFeatureIsInstalledFromApplication("Forms\Shifts") = False Then
            WLHelperWeb.RedirectAccessDenied(False)
            Exit Sub
        End If

        roTreesShifts.TreeCaption = Me.Language.Translate("TreeCaptionShifts", Me.DefaultScope)

        Me.lblAddPair.Text = Me.Language.Translate("btnAddPair.Text", DefaultScope)
        Me.lblDefinitionAdd.Text = Me.Language.Translate("lblDefinitionAdd.Text", DefaultScope)
        Me.LabelDailyRecordEntrada.Text = Me.Language.Translate("LabelDailyRecordEntrada", DefaultScope)
        Me.LabelDailyRecordSalida.Text = Me.Language.Translate("LabelDailyRecordSalida", DefaultScope)

        Me.msgHasChanges.Value = Me.Language.Translate("msgHasChanges", DefaultScope)
        Me.msgHasErrors.Value = Me.Language.Translate("msgHasErrors", DefaultScope)
        Me.msgErrorNoCorrectRule.Value = Me.Language.Translate("msgErrorNoCorrectRule", DefaultScope)

        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)
        If Me.oPermission = Permission.None Then
            WLHelperWeb.RedirectAccessDenied(False)
            Exit Sub
        End If

        If Me.oPermission < Permission.Write Then
            hdnModeEdit.Value = "true"
        Else
            hdnModeEdit.Value = "false"
        End If

        dateFormatValue.Value = HelperWeb.GetShortDateFormat

        'Comprobem si no hi han registres, en aquest cas, carreguem un registre nou
        Dim dTbl As DataTable = API.ShiftServiceMethods.GetShifts(Me.Page, , False)
        If dTbl.Rows.Count = 0 Then
            Me.noRegs.Value = "1"
        Else
            Me.noRegs.Value = ""
        End If


        If Not IsPostBack Then
            LoadShiftCombos()
        End If

    End Sub

    Protected Sub ASPxCallbackPanelContenido_Callback(ByVal sender As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase) Handles ASPxCallbackPanelContenido.Callback

        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Dim oParameters As New ShiftsCallbackRequest()
        oParameters = roJSONHelper.DeserializeNewtonSoft(strParameter, oParameters.GetType())

        Select Case oParameters.Type
            Case "S"
                ProcessShiftsRequest(oParameters)
                ShiftsContent.Style("Display") = ""
                ShiftGroupContent.Style("Display") = "none"
            Case "G"
                ProcessShiftGroupRequest(oParameters)
                ShiftsContent.Style("Display") = "none"
                ShiftGroupContent.Style("Display") = ""
        End Select
    End Sub

#Region "Shifts"

    Private Sub ProcessShiftsRequest(ByVal oParameters As ShiftsCallbackRequest)

        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)

        If Me.oPermission > Permission.None Then

            Select Case oParameters.Action
                Case "GETSHIFT"
                    LoadShiftData(oParameters,, True)
                Case "SAVESHIFT"
                    SaveShiftData(oParameters)
            End Select

            ProcessShiftSelectedTabVisible(oParameters)
        End If
    End Sub

    Private Sub LoadShiftCombos()
        Dim oRow As DataRow

        cmbStartES.Items.Clear()
        cmbStartES.ValueType = GetType(Integer)
        cmbStartES.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ManualDetect.ShiftDay", DefaultScope), 1))
        cmbStartES.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ManualDetect.ShiftAfter", DefaultScope), 2))
        cmbStartES.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ManualDetect.ShiftBefore", DefaultScope), 0))

        cmbEndES.Items.Clear()
        cmbEndES.ValueType = GetType(Integer)
        cmbEndES.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ManualDetect.ShiftDay", DefaultScope), 1))
        cmbEndES.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ManualDetect.ShiftAfter", DefaultScope), 2))
        cmbEndES.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ManualDetect.ShiftBefore", DefaultScope), 0))

        '
        cmbHolidayShiftTypeHours.Items.Clear()
        cmbHolidayShiftTypeHours.ValueType = GetType(Integer)
        cmbHolidayShiftTypeHours.Items.Add(Me.Language.Translate("HolidayShiftTypeHours.Theoric", DefaultScope), 0)
        cmbHolidayShiftTypeHours.Items.Add(Me.Language.Translate("HolidayShiftTypeHours.Manual", DefaultScope), 1)

        'Causes
        cmbRuleCauses2.Items.Clear()
        cmbCauseHoliday.Items.Clear()
        cmbRuleCauses2.ValueType = GetType(Integer)
        cmbCauseHoliday.ValueType = GetType(String)
        Dim dTblCauses As DataTable = CausesServiceMethods.GetCauses(Me)
        For Each dRC As DataRow In dTblCauses.Rows
            If dRC("ID") <> 0 Then
                If Not roTypes.Any2Boolean(dRC("DayType")) And Not roTypes.Any2Boolean(dRC("CustomType")) Then
                    cmbRuleCauses2.Items.Add(New DevExpress.Web.ListEditItem(dRC("Name"), dRC("ID")))
                End If
            End If

            Dim dType As Integer = 0
            If roTypes.Any2Boolean(dRC("CustomType")) OrElse roTypes.Any2Boolean(dRC("DayType")) Then dType = 1

            cmbCauseHoliday.Items.Add(New DevExpress.Web.ListEditItem(dRC("Name"), dRC("ID") & "_" & dType))
        Next

        cmbRequestDays.Items.Clear()
        cmbRequestDays.ValueType = GetType(Integer)
        cmbRequestDays.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("RequestDays.WorkingDay", DefaultScope), 1))
        cmbRequestDays.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("RequestDays.NotWorkingDay", DefaultScope), 0))

        cmbConceptBalance.Items.Clear()
        cmbConceptBalance.ValueType = GetType(Integer)
        cmbConceptBalance.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("Concepts.None", DefaultScope), 0))

        cmbFutureBalance.Items.Clear()
        cmbFutureBalance.ValueType = GetType(Integer)
        cmbFutureBalance.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("Concepts.None", DefaultScope), 0))

        Dim dTblConcepts As DataTable = API.ConceptsServiceMethods.GetHolidaysConcepts(Me)
        For Each dRC As DataRow In dTblConcepts.Rows
            If dRC("ID") <> 0 Then
                cmbConceptBalance.Items.Add(New DevExpress.Web.ListEditItem(dRC("Name"), dRC("ID")))
                If roTypes.Any2String(dRC("DefaultQuery")) = "Y" Then
                    cmbFutureBalance.Items.Add(New DevExpress.Web.ListEditItem(dRC("Name"), dRC("ID")))
                End If

            End If
        Next

        cmbRuleConcept1.Items.Clear()
        Dim dTblInc As DataTable = API.IncidenceServiceMethods.GetIncidences(Me, "ID <> 0")
        If dTblInc IsNot Nothing Then
            Dim tbIncSorted As New DataTable
            tbIncSorted.Columns.Add(New DataColumn("ID", GetType(Integer)))
            tbIncSorted.Columns.Add(New DataColumn("Name", GetType(String)))
            For Each dRI As DataRow In dTblInc.Rows
                oRow = tbIncSorted.NewRow
                oRow("ID") = dRI("ID")
                oRow("Name") = Me.Language.Keyword("Incidence." & dRI("ID"))
                tbIncSorted.Rows.Add(oRow)
            Next
            For Each dRI As DataRow In tbIncSorted.Select("", "Name ASC")
                If (dRI("ID").Equals(2000)) Then Continue For
                cmbRuleConcept1.Items.Add(New ListEditItem(dRI("Name"), dRI("ID")))
            Next
        End If

        cmbRuleConcept2.Items.Clear()
        Dim dTblInc2 As DataTable = API.IncidenceServiceMethods.GetIncidences(Me, "ID <> 0")
        If dTblInc2 IsNot Nothing Then
            Dim tbIncSorted As New DataTable
            tbIncSorted.Columns.Add(New DataColumn("ID", GetType(Integer)))
            tbIncSorted.Columns.Add(New DataColumn("Name", GetType(String)))
            For Each dRI As DataRow In dTblInc.Rows
                oRow = tbIncSorted.NewRow
                oRow("ID") = dRI("ID")
                oRow("Name") = Me.Language.Keyword("Incidence." & dRI("ID"))
                tbIncSorted.Rows.Add(oRow)
            Next
            For Each dRI As DataRow In tbIncSorted.Select("", "Name ASC")
                cmbRuleConcept2.Items.Add(New DevExpress.Web.ListEditItem(dRI("Name"), dRI("ID")))
            Next
        End If

        cmbRuleZone1.Items.Clear()
        Dim dTblZones As DataTable = API.ShiftServiceMethods.GetTimeZones(Me.Page)
        cmbRuleZone1.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("Rules.Combo.AnyZone", DefaultScope), -1))
        For Each dRowZone As DataRow In dTblZones.Rows
            cmbRuleZone1.Items.Add(New DevExpress.Web.ListEditItem(dRowZone("Name"), dRowZone("ID")))
        Next

        cmbRuleCriteria1.Items.Clear()
        cmbRuleCriteria1.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("Rules.Criteria1.AnyTime", DefaultScope), 0))
        cmbRuleCriteria1.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("Rules.Criteria1.MajorOrEqual", DefaultScope), 1))
        cmbRuleCriteria1.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("Rules.Criteria1.MinorOrEqual", DefaultScope), 2))
        cmbRuleCriteria1.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("Rules.Criteria1.Between", DefaultScope), 3))

        cmbRuleConditionValueType.Items.Clear()
        cmbRuleConditionValueType.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("Rules.ConditionValueType.DirectValue", DefaultScope), 0))
        cmbRuleConditionValueType.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("Rules.ConditionValueType.UserFieldValue", DefaultScope), 1))

        cmbRuleCriteria2.Items.Clear()
        cmbRuleCriteria2.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("Rules.Criteria2.AllTime", DefaultScope), 0))
        cmbRuleCriteria2.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("Rules.Criteria2.TimeTo", DefaultScope), 1))

        cmbRuleActionValueType.Items.Clear()
        cmbRuleActionValueType.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("Rules.ActionValueType.DirectValue", DefaultScope), 0))
        cmbRuleActionValueType.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("Rules.ActionValueType.UserFieldValue", DefaultScope), 1))

        Dim tbUserFields As DataTable = API.UserFieldServiceMethods.GetUserFields(Me, Types.EmployeeField, "Used = 1", False)

        Dim dvUserFields As New DataView(tbUserFields)
        dvUserFields.RowFilter = "FieldType = 4"
        dvUserFields.Sort = "FieldName ASC"
        Me.cmbRuleFromValueUserFields.Items.Clear()
        Me.cmbRuleToValueUserFields.Items.Clear()
        Me.cmbRuleMaxValueUserFields.Items.Clear()
        For Each oUserFieldRow As DataRow In dvUserFields.ToTable.Rows
            Me.cmbRuleFromValueUserFields.Items.Add(New DevExpress.Web.ListEditItem(oUserFieldRow("FieldName"), oUserFieldRow("FieldName")))
            Me.cmbRuleToValueUserFields.Items.Add(New DevExpress.Web.ListEditItem(oUserFieldRow("FieldName"), oUserFieldRow("FieldName")))
            Me.cmbRuleMaxValueUserFields.Items.Add(New DevExpress.Web.ListEditItem(oUserFieldRow("FieldName"), oUserFieldRow("FieldName")))
        Next

        dvUserFields = New DataView(tbUserFields)
        dvUserFields.RowFilter = "FieldType = 7"
        dvUserFields.Sort = "FieldName ASC"
        Me.cmbRuleBetweenValueUserFields.Items.Clear()
        For Each oUserFieldRow As DataRow In dvUserFields.ToTable.Rows
            Me.cmbRuleBetweenValueUserFields.Items.Add(New DevExpress.Web.ListEditItem(oUserFieldRow("FieldName"), oUserFieldRow("FieldName")))
        Next

        cmbTotTimeNoSup.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("Flexible.GenerateExtraHours", DefaultScope), 0))
        cmbTotTimeNoSup.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("Flexible.GenerateIncidence", DefaultScope), 1))

        cmbGSumRetInf.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("Mandatory.TimeWorked", DefaultScope), 0))
        cmbGSumRetInf.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("Mandatory.IgnoreIncidence", DefaultScope), 1))

        cmbGSumRetInt.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("Mandatory.TimeWorked", DefaultScope), 0))
        cmbGSumRetInt.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("Mandatory.IgnoreIncidence", DefaultScope), 1))

        cmbGSumAntInf.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("Mandatory.TimeWorked", DefaultScope), 0))
        cmbGSumAntInf.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("Mandatory.IgnoreIncidence", DefaultScope), 1))

        Me.cmbStartFloating.Items.Clear()
        Me.cmbStartFloating.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("StartFloating.ShiftDay", DefaultScope), 1))
        Me.cmbStartFloating.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("StartFloating.ShiftAfter", DefaultScope), 2))
        Me.cmbStartFloating.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("StartFloating.ShiftBefore", DefaultScope), 0))

        Me.tbCollectives.Items.Clear()
        Dim collectives As roCollective() = API.CollectiveServiceMethods.GetAllCollectives(Me)
        If collectives IsNot Nothing Then
            Dim collectivesTable As New DataTable
            collectivesTable.Columns.Add(New DataColumn("ID", GetType(Integer)))
            collectivesTable.Columns.Add(New DataColumn("Name", GetType(String)))
            For Each collective As roCollective In collectives
                oRow = collectivesTable.NewRow
                oRow("ID") = collective.Id
                oRow("Name") = collective.Name
                collectivesTable.Rows.Add(oRow)
            Next
            For Each dRI As DataRow In collectivesTable.Select("", "Name ASC")
                Me.tbCollectives.Items.Add(New DevExpress.Web.ListEditItem(dRI("Name"), dRI("ID")))
            Next
        End If

        'Carrega de combos, etc en sub-formularis        
        Me.frmAddZone1.loadFormAddZone()
        Me.frmEditShiftBreak.loadFormEditShiftBreak()
        Me.frmEditShiftFlexible.loadFormEditShiftFlexible()
        Me.frmEditShiftMandatory.loadFormEditShiftMandatory()

    End Sub

    Private Function GetParameterFromCollection(ByVal oRequest As String, ByVal oParameters As String()) As String
        Dim result As String = ""

        Dim oCurLayer = New roShiftLayer
        Dim oCurCollection As New roCollection

        For Each oParameter As String In oParameters
            If oParameter <> String.Empty Then
                Dim oField As String = oParameter.Split("=")(0).ToUpper()
                Dim oValue As String = oParameter.Split("=")(1)

                If oField = oRequest Then
                    result = oValue
                    Exit For
                End If
            End If
        Next

        Return result
    End Function

    Private Sub SaveShiftData(ByVal oParameters As ShiftsCallbackRequest)
        Dim strError As String = ""
        Dim rError As roJSON.JSONError = Nothing
        Dim strMessage As JSONMsgBox = Nothing

        Dim oCurrentShift As roShift = Nothing
        Try
            cmbGroup.Items.Clear()
            cmbGroup.ValueType = GetType(Integer)

            cmbGroup.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ShiftGroup.General", DefaultScope), 0))
            Dim dTblGroups As DataTable = API.ShiftServiceMethods.GetShiftGroups(Me)
            For Each dRow As DataRow In dTblGroups.Rows
                If dRow("ID").ToString <> "0" Then
                    cmbGroup.Items.Add(New DevExpress.Web.ListEditItem(dRow("Name"), dRow("ID")))
                End If
            Next

            'Check Permissions
            If Me.oPermission < Permission.Write Then
                rError = New roJSON.JSONError(True, Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope))
                strError = "KO"
            End If

            Dim bolIsNew As Boolean = False
            If Request("ID") = "0" Or Request("ID") = "-1" Then bolIsNew = True
            If Me.oPermission = Permission.Write And bolIsNew Then
                rError = New roJSON.JSONError(True, Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope))
                strError = "KO"
            End If

            oCurrentShift = API.ShiftServiceMethods.GetShift(Me, oParameters.ID, True)

            If oCurrentShift Is Nothing Then Return

            Dim oGenericData As roCollection = Nothing
            Dim oGroupFilter1 As roCollection = Nothing
            Dim oGroupFilter2 As roCollection = Nothing
            Dim oGroupFilter3 As roCollection = Nothing

            Dim olayerTypes As roShiftLayer = API.ShiftServiceMethods.CreateEmptyLayer(Me)

            Dim bolCheckVacationsEmpty As Boolean = False

            bolCheckVacationsEmpty = oParameters.checkVacationsEmpty

            oCurrentShift.Color = Drawing.ColorTranslator.ToWin32(colorShift.Color)
            oCurrentShift.Description = txtDescription.Text

            Select Case cmbEndES.SelectedItem.Value
                Case 0
                    oCurrentShift.EndLimit = roTypes.CreateDateTime(1899, 12, 29, txtEndES.DateTime.Hour, txtEndES.DateTime.Minute, 0)
                Case 1
                    oCurrentShift.EndLimit = roTypes.CreateDateTime(1899, 12, 30, txtEndES.DateTime.Hour, txtEndES.DateTime.Minute, 0)
                Case 2
                    oCurrentShift.EndLimit = roTypes.CreateDateTime(1899, 12, 31, txtEndES.DateTime.Hour, txtEndES.DateTime.Minute, 0)
            End Select

            oCurrentShift.ExpectedWorkingHours = roTypes.Any2Time(Format(txtTotHorPrev.DateTime, HelperWeb.GetShortTimeFormat())).NumericValue()
            Dim breakTimeValue As Single = 0
            For Each value As String In oParameters.BreakHours.Split("||")
                breakTimeValue = breakTimeValue + roTypes.Any2Time(value).NumericValue()
            Next

            oCurrentShift.BreakHours = breakTimeValue
            oCurrentShift.IDGroup = cmbGroup.SelectedItem.Value
            oCurrentShift.AllowFloatingData = oParameters.AllowFloatingData

            oCurrentShift.StartFloating = Nothing
            oCurrentShift.AreWorkingDays = False
            oCurrentShift.IDCauseHolidays = 0
            oCurrentShift.IDConceptBalance = 0

            If optNormalFloating.Checked Then
                oCurrentShift.ShiftType = Robotics.Base.DTOs.ShiftType.NormalFloating
                If cmbStartFloating.SelectedItem IsNot Nothing Then
                    Select Case cmbStartFloating.SelectedItem.Value
                        Case 0
                            oCurrentShift.StartFloating = roTypes.CreateDateTime(1899, 12, 29, txtStartFloating.DateTime.Hour, txtStartFloating.DateTime.Minute, 0)
                        Case 1
                            oCurrentShift.StartFloating = roTypes.CreateDateTime(1899, 12, 30, txtStartFloating.DateTime.Hour, txtStartFloating.DateTime.Minute, 0)
                        Case 2
                            oCurrentShift.StartFloating = roTypes.CreateDateTime(1899, 12, 31, txtStartFloating.DateTime.Hour, txtStartFloating.DateTime.Minute, 0)
                    End Select
                End If

                oCurrentShift.HolidayValue = 0
                oCurrentShift.TypeHolidayValue = HolidayValueType.ExpectedWorkingHours_Value

            ElseIf optVacations.Checked Then
                oCurrentShift.ShiftType = Robotics.Base.DTOs.ShiftType.Vacations
                oCurrentShift.DailyFactor = txtDailyFactor.Value

                If cmbRequestDays.SelectedItem.Value = 1 Then
                    oCurrentShift.AreWorkingDays = True
                Else
                    oCurrentShift.AreWorkingDays = False
                End If

                If cmbCauseHoliday.SelectedItem IsNot Nothing Then
                    oCurrentShift.IDCauseHolidays = roTypes.Any2Integer(roTypes.Any2String(cmbCauseHoliday.SelectedItem.Value).Split("_")(0))
                Else
                    oCurrentShift.IDCauseHolidays = 0
                End If

                If cmbFutureBalance.SelectedItem IsNot Nothing Then
                    oCurrentShift.IDConceptRequestNextYear = roTypes.Any2Integer(roTypes.Any2String(cmbFutureBalance.SelectedItem.Value).Split("_")(0))
                Else
                    oCurrentShift.IDConceptRequestNextYear = 0
                End If

                If cmbConceptBalance.SelectedItem IsNot Nothing Then
                    oCurrentShift.IDConceptBalance = cmbConceptBalance.SelectedItem.Value
                Else
                    oCurrentShift.IDConceptBalance = 0
                End If

                'Comprobamos que no exista otro horario con el mismo saldo seleccionado
                If oCurrentShift.IDConceptBalance <> 0 Then
                    'Comprobamos si el saldo seleccionado es de tipo año laboral, en caso de serlo, no pueden haber 2 horarios de tipo vacaciones con el mismo saldo seleccionado                    
                    Dim oConcept As Robotics.Base.VTBusiness.Concept.roConcept = API.ConceptsServiceMethods.GetConceptByID(Nothing, CInt(oCurrentShift.IDConceptBalance), False)
                    If oConcept.DefaultQuery = "L" Then
                        Dim dTbl As DataTable = API.ShiftServiceMethods.GetShifts(Me.Page)
                        If dTbl.Select("IDConceptBalance = " & oCurrentShift.IDConceptBalance & " AND ID <> " & oCurrentShift.ID & " AND ShiftType = 2").Length > 0 Then
                            rError = New roJSON.JSONError(True, Me.Language.Translate("ShiftError.DuplicatedBalance", DefaultScope))
                            strError = "KO"
                            Return
                        End If
                        If Me.txtDailyFactor.Value <> 1 Then
                            rError = New roJSON.JSONError(True, Me.Language.Translate("ShiftError.InvalidDailyFactor", DefaultScope))
                            strError = "KO"
                            Return
                        End If
                    End If
                End If

                oCurrentShift.TypeHolidayValue = cmbHolidayShiftTypeHours.SelectedItem().Value
                If (oCurrentShift.TypeHolidayValue = HolidayValueType.ExpectedWorkingHours_Value) Then
                    oCurrentShift.HolidayValue = 0
                Else

                    If roTypes.Any2Integer(roTypes.Any2String(cmbCauseHoliday.SelectedItem.Value).Split("_")(1)) = 0 Then
                        oCurrentShift.HolidayValue = roConversions.ConvertTimeToHours(dtHolidaySelectHours.DateTime.ToShortTimeString)
                    Else
                        oCurrentShift.HolidayValue = roTypes.Any2Double(txtHolidaySelectHours.Value)
                    End If
                End If

            ElseIf optUnique.Checked OrElse optNormal.Checked Then
                oCurrentShift.ShiftType = Robotics.Base.DTOs.ShiftType.Normal
                oCurrentShift.AllowComplementary = False ' If((optPerHours.Checked = True), oParameters.AllowComplementaryHours, False)

                oCurrentShift.HolidayValue = 0
                oCurrentShift.TypeHolidayValue = HolidayValueType.ExpectedWorkingHours_Value


                If optUnique.Checked Then
                    oCurrentShift.AdvancedParameters = "Starter=[1]"
                End If
            ElseIf optPerHours.Checked Then
                oCurrentShift.ShiftType = Robotics.Base.DTOs.ShiftType.Normal
                oCurrentShift.AllowComplementary = True

                oCurrentShift.HolidayValue = 0
                oCurrentShift.TypeHolidayValue = HolidayValueType.ExpectedWorkingHours_Value
            End If

            oCurrentShift.IsObsolete = chkObsolete.Checked

            oCurrentShift.ManualLimit = optManualDetect.Checked
            oCurrentShift.Name = txtShiftName.Text
            oCurrentShift.ShortName = txtShortName.Text
            oCurrentShift.AdvancedParameters = txtParams.Text
            oCurrentShift.ExportName = txtExport.Text

            Select Case cmbStartES.SelectedItem.Value
                Case 0
                    oCurrentShift.StartLimit = roTypes.CreateDateTime(1899, 12, 29, txtStartES.DateTime.Hour, txtStartES.DateTime.Minute, 0)
                Case 1
                    oCurrentShift.StartLimit = roTypes.CreateDateTime(1899, 12, 30, txtStartES.DateTime.Hour, txtStartES.DateTime.Minute, 0)
                Case 2
                    oCurrentShift.StartLimit = roTypes.CreateDateTime(1899, 12, 31, txtStartES.DateTime.Hour, txtStartES.DateTime.Minute, 0)
            End Select

            If chkGSumRetInf.Checked Then
                oGroupFilter1 = New roCollection
                Dim oAction As String = "TreatAsWork"
                If cmbGSumRetInf.SelectedItem.Value = 1 Then
                    oAction = "Ignore"
                End If
                oGroupFilter1.Add(olayerTypes.XmlKey(roXmlLayerKeys.Action), oAction)
                oGroupFilter1.Add(olayerTypes.XmlKey(roXmlLayerKeys.Value), Format(txtGSumRetHour.DateTime, HelperWeb.GetShortTimeFormat()))
            Else
                oGroupFilter1 = Nothing
            End If

            If chkGSumRetInt.Checked Then
                oGroupFilter2 = New roCollection
                Dim oAction As String = "TreatAsWork"
                If cmbGSumRetInt.SelectedItem.Value = 1 Then
                    oAction = "Ignore"
                End If
                oGroupFilter2.Add(olayerTypes.XmlKey(roXmlLayerKeys.Action), oAction)
                oGroupFilter2.Add(olayerTypes.XmlKey(roXmlLayerKeys.Value), Format(txtGSumretInt.DateTime, HelperWeb.GetShortTimeFormat()))
            Else
                oGroupFilter2 = Nothing
            End If

            If chkGSumAntInf.Checked Then
                oGroupFilter3 = New roCollection
                Dim oAction As String = "TreatAsWork"
                If cmbGSumAntInf.SelectedItem.Value = 1 Then
                    oAction = "Ignore"
                End If
                oGroupFilter3.Add(olayerTypes.XmlKey(roXmlLayerKeys.Action), oAction)
                oGroupFilter3.Add(olayerTypes.XmlKey(roXmlLayerKeys.Value), Format(txtGSumAntInf.DateTime, HelperWeb.GetShortTimeFormat()))
            Else
                oGroupFilter3 = Nothing
            End If

            If chkTotTimeNoSup.Checked Then ' Or chkTotTimeNoLleg.Checked Then
                If oGenericData Is Nothing Then oGenericData = New roCollection
                If cmbTotTimeNoSup.SelectedItem.Value = 0 Then
                    oGenericData.Add(olayerTypes.XmlKey(roXmlLayerKeys.MaxTimeAction), "Overtime")
                Else
                    oGenericData.Add(olayerTypes.XmlKey(roXmlLayerKeys.MaxTimeAction), "Incidence")
                End If
                oGenericData.Add(olayerTypes.XmlKey(roXmlLayerKeys.MaxTime), Format(txtTotTimeNoSup.DateTime, HelperWeb.GetShortTimeFormat()))
                '    oGenericData.Add(olayerTypes.XmlKey(roXmlLayerKeys.MinTime), Format(txtTotTimeNoLleg1.DateTime, HelperWeb.GetShortTimeFormat()))
            End If

            If chkTotTimeNoLleg.Checked Then
                If oGenericData Is Nothing Then oGenericData = New roCollection
                oGenericData.Add(olayerTypes.XmlKey(roXmlLayerKeys.MinTime), Format(txtTotTimeNoLleg1.DateTime, HelperWeb.GetShortTimeFormat()))
            End If
            If opVisibilityAll.Checked = True Then
                oCurrentShift.VisibilityPermissions = 0
            ElseIf opVisibilityNobody.Checked = True Then
                oCurrentShift.VisibilityPermissions = 1
            ElseIf opVisibilityCriteria.Checked = True Then
                oCurrentShift.VisibilityPermissions = 2
            ElseIf optVisibilityCollectives.Checked = True Then
                oCurrentShift.VisibilityPermissions = 3
            End If

            oCurrentShift.VisibilityCriteria = Nothing
            If oCurrentShift.VisibilityPermissions = 2 Then
                If visibilityCriteria.IsValidFilter Then
                    Dim xList As New Generic.List(Of roUserFieldCondition)
                    xList.Add(visibilityCriteria.OConditionValue)
                    oCurrentShift.VisibilityCriteria = xList
                Else
                    strError = "KO"
                    rError = New roJSON.JSONError(True, Me.Language.Translate("Error.NoCorrectVisibilityFilter", DefaultScope))
                End If
            End If

            oCurrentShift.VisibilityCollectives = Nothing
            If oCurrentShift.VisibilityPermissions = 3 Then
                oCurrentShift.VisibilityCollectives = tbCollectives.Value
            End If


            Try
                ''''''PARSE LAYERS
                oCurrentShift.Layers = New ArrayList(ParseResultLayers(oParameters.resultLayers, olayerTypes, oCurrentShift, oGenericData, oGroupFilter1, oGroupFilter2, oGroupFilter3))
            Catch ex As Exception
                oCurrentShift.Layers = New ArrayList
                rError = New roJSON.JSONError(True, ex.Message.ToString & " " & ex.StackTrace)
                strError = "KO"
            End Try

            Try
                ''''''PARSE RULES
                oCurrentShift.SimpleRules = ParseResultRules(oParameters.resultRules, oCurrentShift).ToList
            Catch ex As Exception
                oCurrentShift.SimpleRules = New Generic.List(Of roShiftRule)
                rError = New roJSON.JSONError(True, ex.Message.ToString & " " & ex.StackTrace)
                strError = "KO"
            End Try

            Try
                ''''''PARSE ZONES
                oCurrentShift.TimeZones = ParseResultZones(oParameters.resultTimeZones, oCurrentShift).ToList
            Catch ex As Exception
                oCurrentShift.TimeZones = New Generic.List(Of roShiftTimeZone)
                rError = New roJSON.JSONError(True, ex.Message.ToString & " " & ex.StackTrace)
                strError = "KO"
            End Try

            Try
                ''''''PARSE ASSIGNMENTS
                oCurrentShift.Assignments = ParseResultAssignments(oParameters.resultAssignments, oCurrentShift).ToList
            Catch ex As Exception
                oCurrentShift.Assignments = New Generic.List(Of roShiftAssignment)
                rError = New roJSON.JSONError(True, ex.Message.ToString & " " & ex.StackTrace)
                strError = "KO"
            End Try

            If oParameters.DailyRules IsNot Nothing Then
                For Each oRule In oParameters.DailyRules
                    If oRule.Actions IsNot Nothing Then
                        For Each oAction In oRule.Actions
                            oAction.PlusDirectValue = oAction.PlusDirectValue.Replace(System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator, ".")
                            oAction.PlusDirectValueResult = oAction.PlusDirectValueResult.Replace(System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator, ".")
                        Next
                    End If
                Next
            End If

            oCurrentShift.DailyRules = oParameters.DailyRules.ToList

            Dim tmpRecalcDate As Date = roTypes.Any2DateTime(oParameters.RecalcDate)

            Dim bNeedRecalc As Boolean = Me.recalcConfig("NeedRecalc")

            If bNeedRecalc Then
                If tmpRecalcDate.Ticks <> roTypes.CreateDateTime(2079, 1, 1).Ticks Then
                    oCurrentShift.StartDate = oParameters.RecalcDate
                Else
                    oCurrentShift.StartDate = Date.MinValue
                End If
            Else
                oCurrentShift.StartDate = Nothing
            End If


            If oCurrentShift.ShiftType <> Robotics.Base.DTOs.ShiftType.Vacations Then
                'DailyRecordPattern
                Dim isDailyRecordLicense = HelperSession.GetFeatureIsInstalledFromApplication("Feature\DailyRecord")
                Dim punches(9) As roShiftPunchesPatternItem
                Dim dLastPunch As DateTime = DateTime.MinValue
                Dim drPatternPunches As String = oParameters.DRPatternPunches
                Dim iAddDays As Integer = 0
                If isDailyRecordLicense AndAlso drPatternPunches.Length > 0 Then
                    Dim i = 0
                    Dim timeValues = oParameters.DRPatternPunches.Split(",")
                    For Each timeEdit In timeValues
                        If indxFranja(i) < roTypes.Any2Integer(hdnCurrentPair.Value) AndAlso timeEdit.Length > 0 Then
                            Dim punch As roShiftPunchesPatternItem = New roShiftPunchesPatternItem
                            punch.DateTime = roTypes.Any2DateTime("1899-12-30 " + timeEdit)
                            If iAddDays > 0 Then
                                punch.DateTime = punch.DateTime.AddDays(iAddDays)
                            End If

                            If ((i Mod 2) = 0) Then
                                punch.Type = 1 'Entrada
                            Else
                                punch.Type = 2 'Salida
                            End If
                            If punch.DateTime < dLastPunch Then
                                iAddDays += 1
                                punch.DateTime = punch.DateTime.AddDays(1)
                            End If
                            dLastPunch = punch.DateTime
                            punches(i) = punch
                            i += 1
                        End If
                    Next
                    ReDim Preserve punches(i - 1)
                    If (oCurrentShift.PunchesPattern Is Nothing) Then
                        oCurrentShift.PunchesPattern = New roShiftPunchesPattern
                    End If
                    oCurrentShift.PunchesPattern.Punches = punches
                End If
            End If



            If strError = String.Empty AndAlso oCurrentShift.ShortName = String.Empty Then
                rError = New roJSON.JSONError(True, Me.Language.Translate("Validation.Error.ShortNameRequiered", Me.DefaultScope))
                strError = "KO"
            End If

            Dim perHoursValidate = (oCurrentShift.AllowComplementary OrElse oCurrentShift.AllowFloatingData)

            If strError = String.Empty AndAlso (optPerHours.Checked AndAlso Not perHoursValidate) Then
                rError = New roJSON.JSONError(True, Me.Language.Translate("Validation.Error.PerHoursValidate", Me.DefaultScope))
                strError = "KO"
            End If

            If strError = String.Empty Then
                If API.ShiftServiceMethods.SaveShift(Me, oCurrentShift, bolCheckVacationsEmpty, True) Then
                    rError = New roJSON.JSONError(False, "OK:" & oCurrentShift.ID)
                    strError = "OK"
                ElseIf roWsUserManagement.SessionObject.States.ShiftState.Result = ShiftResultEnum.VacationsShiftNotEmpty Then
                    Dim oMsgParams As New Generic.List(Of String)
                    oMsgParams.Add(roWsUserManagement.SessionObject.States.ShiftState.ErrorText)
                    strMessage = New JSONMsgBox("QUESTION", Me.Language.Translate("VacationsShiftNotEmpty.Warnning.Title", Me.DefaultScope), Me.Language.Translate("VacationsShiftNotEmpty.Warnning.Description", Me.DefaultScope, oMsgParams), Me.Language.Keyword("Button.Accept"), Me.Language.Keyword("Button.Cancel"), "window.frames['ifPrincipal'].saveDefChanges(false); HideMsgBoxForm(); return false;", "HideMsgBoxForm(); return false;", True, True, Me.Language.Translate("VacationsShiftNotEmpty.Warnning.Accept.Description", Me.DefaultScope), Me.Language.Translate("VacationsShiftNotEmpty.Warnning.Cancel.Description", Me.DefaultScope))
                    strError = "QUESTION"
                Else
                    rError = New roJSON.JSONError(True, API.ShiftServiceMethods.LastErrorText)
                    strError = "KO"
                End If
            End If
        Catch ex As Exception
            rError = New roJSON.JSONError(True, ex.Message.ToString & " " & ex.StackTrace)
            strError = "KO"
        Finally

            If strError = "KO" OrElse strError = "QUESTION" Then

                Dim hasDateValue As Boolean = False
                Dim tmpRecValue As Date
                If Me.dtRecDate.Value IsNot Nothing Then
                    hasDateValue = True
                    tmpRecValue = Me.dtRecDate.Date
                End If

                LoadShiftData(oParameters, oCurrentShift)

                If hasDateValue Then
                    Me.dtRecDate.Value = tmpRecValue
                Else
                    Me.dtRecDate.Value = Nothing
                End If

                If strError = "KO" Then
                    ASPxCallbackPanelContenido.JSProperties("cpActionRO") = "SAVESHIFT"
                    ASPxCallbackPanelContenido.JSProperties("cpResultRO") = "KO"
                    ASPxCallbackPanelContenido.JSProperties("cpMessageRO") = rError
                Else
                    ASPxCallbackPanelContenido.JSProperties("cpActionRO") = "SAVESHIFT"
                    ASPxCallbackPanelContenido.JSProperties("cpResultRO") = "QUESTION"
                    ASPxCallbackPanelContenido.JSProperties("cpMessageRO") = strMessage
                End If
            Else
                Dim treePath As String = "/source/"
                If oCurrentShift.IDGroup > 0 Then treePath &= "A" & oCurrentShift.IDGroup & "/"
                treePath &= "B" & oCurrentShift.ID
                HelperWeb.roSelector_SetSelection("B" & oCurrentShift.ID.ToString, treePath, "ctl00_contentMainBody_roTreesShifts")
                LoadShiftData(oParameters)
                ASPxCallbackPanelContenido.JSProperties("cpActionRO") = "SAVESHIFT"
                ASPxCallbackPanelContenido.JSProperties("cpNewIdRO") = oParameters.ID
                ASPxCallbackPanelContenido.JSProperties("cpIsNewRO") = True
            End If
        End Try
    End Sub

    Private Function indxFranja(i As Integer) As Integer
        Return Math.Truncate(i / 2)
    End Function

    Private Sub LoadShiftData(ByVal oParameters As ShiftsCallbackRequest, Optional ByVal sShift As roShift = Nothing, Optional ByVal bAudit As Boolean = False)
        Dim strError As String = ""
        Dim strMessage As String = ""
        Dim oCurrentShift As roShift = Nothing
        Dim strcombosIndex As String = ""

        Try
            chkAllowComplementary.Checked = False
            cmbGroup.Items.Clear()
            cmbGroup.ValueType = GetType(Integer)
            cmbGroup.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ShiftGroup.General", DefaultScope), 0))
            Dim dTblGroups As DataTable = API.ShiftServiceMethods.GetShiftGroups(Me)
            For Each dRow As DataRow In dTblGroups.Rows
                If dRow("ID").ToString <> "0" Then
                    cmbGroup.Items.Add(New DevExpress.Web.ListEditItem(dRow("Name"), dRow("ID")))
                End If
            Next

            Me.dtRecDate.Value = Nothing
            'If Me.recalcConfig("FirstDat
            If recalcConfig.Contains("FirstDate") Then
                Me.recalcConfig("FirstDate") = API.ConnectorServiceMethods.GetFirstDate(Nothing).AddDays(1)
            Else
                Me.recalcConfig.Add("FirstDate", API.ConnectorServiceMethods.GetFirstDate(Nothing).AddDays(1))
            End If

            If recalcConfig.Contains("NeedRecalc") Then
                Me.recalcConfig("NeedRecalc") = False
            Else
                Me.recalcConfig.Add("NeedRecalc", False)
            End If

            Me.hdnRuleChanges.Value = "0"
            Me.hdnDailyRuleChanges.Value = "0"

            If sShift Is Nothing Then
                If oParameters.ID < 1 Then
                    oCurrentShift = New roShift
                    oCurrentShift.ID = -1
                    hdnNewShift("NewShift") = -1
                    hdnNewShift("Layers") = 0
                Else
                    oCurrentShift = API.ShiftServiceMethods.GetShift(Me, oParameters.ID, bAudit)
                    hdnNewShift("NewShift") = oParameters.ID
                    hdnNewShift("Layers") = oCurrentShift.Layers.Count
                    hdnSelectedGroup.Value = oCurrentShift.IDGroup
                End If
            Else
                oCurrentShift = sShift
            End If

            Me.gridRules.Controls.Add(creaGridRules())
            Me.gridDailyRules.Controls.Add(creaGridDailyRules())

            txtShiftName.Text = oCurrentShift.Name
            txtDescription.Text = oCurrentShift.Description
            colorShift.Color = System.Drawing.ColorTranslator.FromWin32(oCurrentShift.Color)
            txtShortName.Text = oCurrentShift.ShortName
            txtExport.Text = oCurrentShift.ExportName
            If oParameters.ID <> -1 Then
                cmbGroup.SelectedItem = cmbGroup.Items.FindByValue(oCurrentShift.IDGroup)
            Else
                cmbGroup.SelectedItem = cmbGroup.Items.FindByValue(roTypes.Any2Integer(hdnSelectedGroup.Value))
            End If

            optNormalFloating.Checked = False
            optVacations.Checked = False
            optNormal.Checked = False
            optPerHours.Checked = False
            optUnique.Checked = False

            If oCurrentShift.ID = -1 Then
                Me.txtDailyFactor.Value = 1.0
            Else
                Me.txtDailyFactor.Value = oCurrentShift.DailyFactor
            End If

            Dim advParams As roShiftAdvParameters() = LoadShiftAdvancedParatemers(oCurrentShift.AdvancedParameters)
            If advParams.ToList().FindAll(Function(x) x.Name = "Starter" AndAlso x.Value = "1").Count > 0 Then
                optUnique.Checked = True
                If oCurrentShift.IsObsolete Then chkObsolete.Disabled = True
            Else
                Select Case oCurrentShift.ShiftType
                    Case Robotics.Base.DTOs.ShiftType.Normal
                        If (oCurrentShift.AllowComplementary = True OrElse oCurrentShift.AllowFloatingData) Then
                            optPerHours.Checked = True
                            chkAllowComplementary.Checked = oCurrentShift.AllowComplementary
                        Else
                            optNormal.Checked = True
                        End If
                    Case Robotics.Base.DTOs.ShiftType.Vacations : optVacations.Checked = True
                    Case Robotics.Base.DTOs.ShiftType.NormalFloating
                        optNormalFloating.Checked = True
                End Select
            End If

            If oCurrentShift.StartFloating.HasValue Then
                If oCurrentShift.StartFloating.Value.Day = 29 Then cmbStartFloating.SelectedItem = cmbStartFloating.Items.FindByValue("0")
                If oCurrentShift.StartFloating.Value.Day = 30 Then cmbStartFloating.SelectedItem = cmbStartFloating.Items.FindByValue("1")
                If oCurrentShift.StartFloating.Value.Day = 31 Then cmbStartFloating.SelectedItem = cmbStartFloating.Items.FindByValue("2")

                txtStartFloating.DateTime = oCurrentShift.StartFloating.Value
            Else
                cmbStartFloating.SelectedItem = cmbStartFloating.Items.FindByValue("1")
            End If

            If oCurrentShift.TypeHolidayValue = HolidayValueType.ExpectedWorkingHours_Value Then
                cmbHolidayShiftTypeHours.SelectedItem = cmbHolidayShiftTypeHours.Items.FindByValue(0)
            ElseIf oCurrentShift.TypeHolidayValue = HolidayValueType.Direct_Value Then
                cmbHolidayShiftTypeHours.SelectedItem = cmbHolidayShiftTypeHours.Items.FindByValue(1)
            End If

            txtStartES.DateTime = oCurrentShift.StartLimit
            txtEndES.DateTime = oCurrentShift.EndLimit
            chkObsolete.Checked = oCurrentShift.IsObsolete
            chkObsoleteLastValue.Checked = chkObsolete.Checked

            If oCurrentShift.ManualLimit Then
                optAutoDetect.Checked = False
                optManualDetect.Checked = True
            Else
                optAutoDetect.Checked = True
                optManualDetect.Checked = False
            End If

            Select Case oCurrentShift.StartLimit.Day
                Case 29
                    cmbStartES.SelectedItem = cmbStartES.Items.FindByValue(0)
                Case 30
                    cmbStartES.SelectedItem = cmbStartES.Items.FindByValue(1)
                Case 31
                    cmbStartES.SelectedItem = cmbStartES.Items.FindByValue(2)
                Case Else
                    cmbStartES.SelectedItem = cmbStartES.Items.FindByValue(1)
            End Select

            Select Case oCurrentShift.EndLimit.Day
                Case 29
                    cmbEndES.SelectedItem = cmbEndES.Items.FindByValue(0)
                Case 30
                    cmbEndES.SelectedItem = cmbEndES.Items.FindByValue(1)
                Case 31
                    cmbEndES.SelectedItem = cmbEndES.Items.FindByValue(2)
                Case Else
                    cmbEndES.SelectedItem = cmbEndES.Items.FindByValue(1)
            End Select

            cmbConceptBalance.SelectedItem = cmbConceptBalance.Items.FindByValue(oCurrentShift.IDConceptBalance)
            cmbFutureBalance.SelectedItem = cmbFutureBalance.Items.FindByValue(oCurrentShift.IDConceptRequestNextYear)

            If Me.cmbConceptBalance.SelectedItem Is Nothing Then
                Dim oList As New DevExpress.Web.ListEditItem
                Dim oConcept As roConcept = API.ConceptsServiceMethods.GetConceptByID(Me.Page, oCurrentShift.IDConceptBalance, False)
                If oConcept IsNot Nothing Then
                    oList.Text = oConcept.Name & ("*")
                    oList.Value = oConcept.ID
                    Me.cmbConceptBalance.Items.Add(oList)
                    cmbConceptBalance.SelectedItem = cmbConceptBalance.Items.FindByValue(oConcept.ID)
                End If
            Else
                Dim selectedConceptBalance = oCurrentShift.IDConceptBalance
                If sShift Is Nothing Then
                    selectedConceptBalance = Me.cmbConceptBalance.SelectedItem.Value
                End If
                Dim oConcept As roConcept = API.ConceptsServiceMethods.GetConceptByID(Me.Page, selectedConceptBalance, False)
                If oConcept IsNot Nothing AndAlso oConcept.DefaultQuery = "L" Then
                    Me.cmbFutureBalance.SelectedItem = Me.cmbFutureBalance.Items(0)
                    Me.cmbFutureBalance.ClientEnabled = False
                End If
            End If

            If Me.cmbFutureBalance.SelectedItem Is Nothing Then
                Dim oList As New DevExpress.Web.ListEditItem
                Dim oConceptFuture As roConcept = API.ConceptsServiceMethods.GetConceptByID(Me.Page, oCurrentShift.IDConceptRequestNextYear, False)
                If oConceptFuture IsNot Nothing Then
                    oList.Text = oConceptFuture.Name & ("*")
                    oList.Value = oConceptFuture.ID
                    Me.cmbFutureBalance.Items.Add(oList)
                    cmbFutureBalance.SelectedItem = cmbFutureBalance.Items.FindByValue(oConceptFuture.ID)
                End If
            End If

            If oCurrentShift.AreWorkingDays Then
                cmbRequestDays.SelectedItem = cmbRequestDays.Items.FindByValue(1)
            Else
                cmbRequestDays.SelectedItem = cmbRequestDays.Items.FindByValue(0)
            End If

            If oCurrentShift.IDCauseHolidays > 0 Then
                cmbCauseHoliday.SelectedItem = Nothing

                cmbCauseHoliday.SelectedItem = cmbCauseHoliday.Items.FindByValue((oCurrentShift.IDCauseHolidays & "_" & 0))
                dtHolidaySelectHours.Value = roTypes.Any2Time("1970-01-01 " & roConversions.ConvertHoursToTime(oCurrentShift.HolidayValue)).ValueDateTime()
                txtHolidaySelectHours.Value = 0

                If cmbCauseHoliday.SelectedItem Is Nothing Then
                    cmbCauseHoliday.SelectedItem = cmbCauseHoliday.Items.FindByValue((oCurrentShift.IDCauseHolidays & "_" & 1))

                    txtHolidaySelectHours.Value = oCurrentShift.HolidayValue
                    dtHolidaySelectHours.Value = roTypes.Any2Time("1970-01-01 00:00").ValueDateTime()
                End If
            Else
                cmbCauseHoliday.SelectedItem = Nothing
                txtHolidaySelectHours.Value = 0
                dtHolidaySelectHours.Value = roTypes.Any2Time("1970-01-01 00:00").ValueDateTime()
            End If

            txtTotHorPrev.DateTime = CDate("1970/1/1 " & Format(roTypes.Any2Time(oCurrentShift.ExpectedWorkingHours).TimeOnly, "HH:mm"))

            txtParams.Text = oCurrentShift.AdvancedParameters
            If oCurrentShift IsNot Nothing AndAlso oCurrentShift.AdvancedParameters IsNot Nothing Then
                paramsObsolete.Visible = oCurrentShift.AdvancedParameters.Length > 0
            End If

            Dim oChkMaxTime As Boolean = False
            Dim oChkMinTime As Boolean = False

            Dim oChkFilt1 As Boolean = False
            Dim oChkFilt2 As Boolean = False
            Dim oChkFilt3 As Boolean = False

            Dim oChkAllowModIni As Boolean = False
            Dim oChkAllowModDuration As Boolean = False

            'Camps layer General
            Dim LG_CHKMAXTIME As Boolean = False
            Dim LG_MAXTIME As String = "00:00"
            Dim LG_CHKMINTIME As Boolean = False
            Dim LG_MINTIME As Date
            Dim LG_MAXACTION As String = "0"
            Dim LG_CHKFILTER1 As Boolean = False
            Dim LG_TXTFILTER1 As Date
            Dim LG_CMBFILTER1 As String = "0"
            Dim LG_CHKFILTER2 As Boolean = False
            Dim LG_TXTFILTER2 As Date
            Dim LG_CMBFILTER2 As String = "0"
            Dim LG_CHKFILTER3 As Boolean = False
            Dim LG_TXTFILTER3 As Date
            Dim LG_CMBFILTER3 As String = "0"

            Dim oData As roCollection
            If oCurrentShift.Layers IsNot Nothing Then
                'Recuperació de les layers per la franja horaria GENERAL
                For Each oLayer As roShiftLayer In oCurrentShift.Layers
                    oData = New roCollection(oLayer.DataStoredXML)

                    If oLayer.LayerType = roLayerTypes.roLTDailyTotalsFilter Then
                        If oData.Exists(oLayer.XmlKey(roXmlLayerKeys.MaxTime)) Then
                            Dim oMaxTime As Date = oData.Item(oLayer.XmlKey(roXmlLayerKeys.MaxTime))
                            oChkMaxTime = True
                            LG_CHKMAXTIME = oChkMaxTime
                            LG_MAXTIME = Format(oMaxTime, "HH:mm").ToString
                        End If

                        If oData.Exists(oLayer.XmlKey(roXmlLayerKeys.MinTime)) Then
                            Dim oMinTime As Date = oData.Item(oLayer.XmlKey(roXmlLayerKeys.MinTime))
                            oChkMinTime = True
                            LG_CHKMINTIME = oChkMinTime
                            LG_MINTIME = oMinTime
                        End If

                        If oData.Exists(oLayer.XmlKey(roXmlLayerKeys.MaxTimeAction)) Then
                            Dim oMaxTimeAction As String = oData.Item(oLayer.XmlKey(roXmlLayerKeys.MaxTimeAction))
                            Dim oVal = 0
                            If oMaxTimeAction.ToUpper <> "OVERTIME" Then oVal = 1
                            LG_MAXACTION = oVal.ToString
                        End If

                    ElseIf oLayer.LayerType = roLayerTypes.roLTGroupFilter Then

                        If oData.Exists(oLayer.XmlKey(roXmlLayerKeys.Target)) Then
                            Dim oTarget As String = oData.Item(oLayer.XmlKey(roXmlLayerKeys.Target))
                            Dim oValue As Date = Nothing
                            Dim oAction As String = ""
                            Dim oCmbVal As String = "0"

                            If oData.Exists(oLayer.XmlKey(roXmlLayerKeys.Value)) Then
                                oValue = oData.Item(oLayer.XmlKey(roXmlLayerKeys.Value))
                            End If

                            If oData.Exists(oLayer.XmlKey(roXmlLayerKeys.Action)) Then
                                oAction = oData.Item(oLayer.XmlKey(roXmlLayerKeys.Action))
                                If oAction.ToUpper <> "TREATASWORK" Then
                                    oCmbVal = "1"
                                End If
                            End If

                            If oTarget = "1020" Then
                                oChkFilt1 = True
                                LG_CHKFILTER1 = oChkFilt1
                                LG_TXTFILTER1 = oValue
                                LG_CMBFILTER1 = oCmbVal.ToString
                            ElseIf oTarget = "1021" Then
                                oChkFilt2 = True
                                LG_CHKFILTER2 = oChkFilt2
                                LG_TXTFILTER2 = oValue
                                LG_CMBFILTER2 = oCmbVal.ToString
                            ElseIf oTarget = "1022" Then
                                oChkFilt3 = True
                                LG_CHKFILTER3 = oChkFilt3
                                LG_TXTFILTER3 = oValue
                                LG_CMBFILTER3 = oCmbVal.ToString
                            End If
                        End If

                    End If

                Next
            End If

            chkTotTimeNoSup.Checked = LG_CHKMAXTIME
            txtTotTimeNoSup.DateTime = LG_MAXTIME
            cmbTotTimeNoSup.SelectedItem = cmbTotTimeNoSup.Items.FindByValue(LG_MAXACTION)

            chkTotTimeNoLleg.Checked = LG_CHKMINTIME
            txtTotTimeNoLleg1.DateTime = LG_MINTIME

            chkGSumRetInf.Checked = LG_CHKFILTER1
            txtGSumRetHour.DateTime = LG_TXTFILTER1
            cmbGSumRetInf.SelectedItem = cmbGSumRetInf.Items.FindByValue(LG_CMBFILTER1)

            chkGSumRetInt.Checked = LG_CHKFILTER2
            txtGSumretInt.DateTime = LG_TXTFILTER2
            cmbGSumRetInt.SelectedItem = cmbGSumRetInt.Items.FindByValue(LG_CMBFILTER2)

            chkGSumAntInf.Checked = LG_CHKFILTER3
            txtGSumAntInf.DateTime = LG_TXTFILTER3
            cmbGSumAntInf.SelectedItem = cmbGSumAntInf.Items.FindByValue(LG_CMBFILTER3)


            Dim bolCreateEmployeesPermissionEmpty As Boolean = True
            opVisibilityAll.Checked = False
            opVisibilityNobody.Checked = False
            opVisibilityCriteria.Checked = False
            optVisibilityCollectives.Checked = False
            tbCollectives.Value = String.Empty
            If HelperSession.GetFeatureIsInstalledFromApplication("Feature\Collectives") = True Then
                optVisibilityCollectives.Enabled = True
            Else
                optVisibilityCollectives.Enabled = False
                lblCollectivesCriteriaDesc.Text = Me.Language.Translate("NotAllowedInLicense", DefaultScope)
            End If
            If oCurrentShift.ID = -1 Then
                oCurrentShift.VisibilityPermissions = 1 ' Por defecto los horarios no son visibles desde portal
            End If
            Select Case oCurrentShift.VisibilityPermissions
                Case 0
                    opVisibilityAll.Checked = True
                Case 1
                    opVisibilityNobody.Checked = True
                Case 2
                    opVisibilityCriteria.Checked = True
                    If oCurrentShift.VisibilityCriteria IsNot Nothing AndAlso oCurrentShift.VisibilityCriteria.Count > 0 Then
                        bolCreateEmployeesPermissionEmpty = False
                        strcombosIndex = Me.visibilityCriteria.SetFilterValue(oCurrentShift.VisibilityCriteria(0))
                    End If
                Case 3
                    If HelperSession.GetFeatureIsInstalledFromApplication("Feature\Collectives") = True Then
                        optVisibilityCollectives.Checked = True
                        If oCurrentShift.VisibilityCollectives IsNot Nothing AndAlso oCurrentShift.VisibilityCollectives <> String.Empty Then
                            tbCollectives.Value = oCurrentShift.VisibilityCollectives
                        End If
                    End If

            End Select

            'Me.GetFeaturePermission(FeatureCostCenterAlias)

            If bolCreateEmployeesPermissionEmpty = True Then
                Me.visibilityCriteria.ClearFilterValue()
            End If

            If Me.oPermission < Permission.Write Then
                DisableControls(Me.Controls)
                panTbRules.Controls.Clear()
                panTbDailyRules.Controls.Clear()
                toolbarDefinition.Controls.Clear()
                toolbarZones.Controls.Clear()

            End If

            If oCurrentShift.DailyRules IsNot Nothing Then
                For Each oRule In oCurrentShift.DailyRules
                    If oRule.Actions IsNot Nothing Then
                        For Each oAction In oRule.Actions
                            oAction.PlusDirectValue = oAction.PlusDirectValue.Replace(".", System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator)
                            oAction.PlusDirectValueResult = oAction.PlusDirectValueResult.Replace(".", System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator)
                        Next
                    End If
                Next
            End If

            Me.divNormalFloating.Style("display") = IIf(HelperSession.GetFeatureIsInstalledFromApplication("Feature\MultipleShifts"), "", "none")

            'Declaración de la jornada
            Me.LoadDailyRecord(oCurrentShift)

            Dim existingShortNamesTable As DataTable = API.ShiftServiceMethods.GetExistingShortNamesAndExport(Me)
            Dim existingShortNames As String = String.Join(",", existingShortNamesTable.AsEnumerable().[Select](Function(row) row.Field(Of String)("ShortName")))
            Dim existingExports As String = String.Join(",", existingShortNamesTable.AsEnumerable().[Select](Function(row) row.Field(Of String)("Export")))
            Me.hdnExistingShortNames.Value = existingShortNames & ", " & existingExports

        Finally
            ASPxCallbackPanelContenido.JSProperties.Add("cpActionRO", "GETSHIFT")
            If strError = String.Empty Then
                ASPxCallbackPanelContenido.JSProperties.Add("cpResultRO", "OK")
                ASPxCallbackPanelContenido.JSProperties.Add("cpNameRO", oCurrentShift?.Name)
                ASPxCallbackPanelContenido.JSProperties.Add("cpCombosIndexRO", strcombosIndex)
                ASPxCallbackPanelContenido.JSProperties.Add("cpShiftRulesRO", CreateShiftRulesJSON(oCurrentShift))
                ASPxCallbackPanelContenido.JSProperties.Add("cpShiftAssignmentsRO", CreateShiftAssignmentsJSON(oCurrentShift))
                ASPxCallbackPanelContenido.JSProperties.Add("cpShiftTLZonesRO", retTLZonesJSON(oCurrentShift))
                ASPxCallbackPanelContenido.JSProperties.Add("cpShiftHourZonesRO", retHourZonesJSON(oCurrentShift))

                ASPxCallbackPanelContenido.JSProperties.Add("cpShiftDailyRules", roJSONHelper.SerializeNewtonSoft(oCurrentShift?.DailyRules))

                ASPxCallbackPanelContenido.JSProperties.Add("cpIsNewRO", False)
            Else
                ASPxCallbackPanelContenido.JSProperties.Add("cpResultRO", "KO")
                ASPxCallbackPanelContenido.JSProperties.Add("cpMessageRO", strMessage)
            End If

        End Try
    End Sub

    Private Sub LoadDailyRecord(oCurrentShift As roShift)
        Dim isDailyRecordLicense = HelperSession.GetFeatureIsInstalledFromApplication("Feature\DailyRecord")
        Dim timeEdits = {txtDREntrada1, txtDRSalida1, txtDREntrada2, txtDRSalida2, txtDREntrada3, txtDRSalida3, txtDREntrada4, txtDRSalida4, txtDREntrada5, txtDRSalida5}
        Dim divsFranjas = {drPair1, drPair2, drPair3, drPair4, drPair5}
        Dim btnDeleteFranjas = {btnRemovePair1, btnRemovePair2, btnRemovePair3, btnRemovePair4, btnRemovePair5}
        Dim pattern As roShiftPunchesPattern = oCurrentShift.PunchesPattern

        If isDailyRecordLicense Then
            'Reseteamos los valores
            hdnCurrentPair.Value = 1
            For Each timeEdit In timeEdits
                timeEdit.Text = ""
            Next
            If (oCurrentShift.PunchesPattern IsNot Nothing) Then
                Dim punchesLength As Integer = pattern.Punches.Length
                If punchesLength > 0 Then
                    Dim lastPunchValue As DateTime = roTypes.CreateDateTime(1899, 1, 1)
                    For i As Integer = 0 To (punchesLength - 1)
                        If pattern.Punches(i) IsNot Nothing Then
                            Dim punch = pattern.Punches(i).DateTime.ToString("HH:mm").ToString
                            timeEdits(i).Text = punch
                            If (i Mod 2) = 0 Then
                                Dim newIndx As Integer = i / 2
                                divsFranjas(newIndx).Style("display") = "" 'Mostramos la fila si existen fichajes
                                hdnCurrentPair.Value = newIndx + 1
                            End If
                            If i = (timeEdits.Length - 1) Then Me.btnAddPair.Style("display") = "none !important" 'Ocultamos botón añadir si existen todas las filas

                            Dim currentPunchValue = roTypes.Any2DateTime("1899-12-30 " & punch)
                            If (currentPunchValue.CompareTo(lastPunchValue) = -1) Then Me.drDailyChanged.Style("display") = "block"
                            lastPunchValue = currentPunchValue
                        End If
                    Next
                    If (punchesLength >= 2 AndAlso (punchesLength Mod 2) = 0) Then
                        'Mostramos el botón de eliminar en la última franja
                        Dim indxDeleteBtn = (punchesLength / 2) - 1
                        btnDeleteFranjas(indxDeleteBtn).Style("opacity") = "1"
                        btnDeleteFranjas(indxDeleteBtn).Style("pointer-events") = "inherit"
                    End If
                End If
            End If
        Else
            Me.divDR.Style("display") = "none"
        End If
    End Sub

    Private Function CreateShiftAssignmentsJSON(ByVal oCurrentShift As roShift) As String
        Try
            Dim oJGAssigments As New Generic.List(Of Object)

            Dim oJFAssignments As Generic.List(Of JSONFieldItem)

            Dim strJSONGroups As String = ""

            If oCurrentShift.Assignments IsNot Nothing AndAlso oCurrentShift.Assignments.Count > 0 Then
                Dim tblAssignment As DataTable = API.AssignmentServiceMethods.GetAssignmentsDataTable(Me, "", False)
                For Each oShiftAssignment As roShiftAssignment In oCurrentShift.Assignments
                    oJFAssignments = New Generic.List(Of JSONFieldItem)
                    oJFAssignments.Add(New JSONFieldItem("IDAssignment", oShiftAssignment.IDAssignment, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    Dim dRowSel() As DataRow = tblAssignment.Select("ID = " & oShiftAssignment.IDAssignment.ToString)
                    oJFAssignments.Add(New JSONFieldItem("AssignmentName", dRowSel(0).Item("Name").ToString, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    oJFAssignments.Add(New JSONFieldItem("Coverage", oShiftAssignment.Coverage * 100, Nothing, roJSON.JSONType.None_JSON, Nothing, False))

                    oJGAssigments.Add(oJFAssignments)
                Next
            End If

            strJSONGroups = "{ ""assignments"": ["

            For Each oObj As Object In oJGAssigments
                Dim strJSONText As String = ""
                strJSONText &= "{ ""fields"": "
                strJSONText &= roJSONHelper.Serialize(oObj)
                strJSONText &= " } ,"
                strJSONGroups &= strJSONText
            Next
            If strJSONGroups.EndsWith(",") Then strJSONGroups = strJSONGroups.Substring(0, Len(strJSONGroups) - 1)
            strJSONGroups &= "]}"

            Return strJSONGroups.Replace("<br>", "").Replace("<br/>", "")
        Catch ex As Exception
            Return ""
        End Try
    End Function

    Private Function ParseResultAssignments(ByVal resultAssignments As String, ByVal oCurrentShift As roShift) As roShiftAssignment()
        Dim oArrAssignments As New Generic.SortedList(Of Integer, roShiftAssignment)

        Dim splitSep() As String = {"*&*"}
        Dim splitObjSep() As String = {"*|*"}

        Dim layers As String() = resultAssignments.Split(splitSep, System.StringSplitOptions.None)
        Dim cIndex As Integer = 0
        For Each oComp As String In layers
            If oComp <> String.Empty Then
                Dim oAssignmentParameters As String() = oComp.Split(splitObjSep, System.StringSplitOptions.None)

                Dim cAssignment As New roShiftAssignment
                For Each oParameter As String In oAssignmentParameters
                    If oParameter <> String.Empty Then
                        Dim oField As String = oParameter.Split("=")(0).ToUpper()
                        Dim oValue As String = oParameter.Split("=")(1)

                        Select Case oField
                            Case "IDASSIGNMENT"  '1
                                cAssignment.IDAssignment = roTypes.Any2Integer(oValue)
                            Case "COVERAGE" '(90)"
                                Dim dblCovergae = roTypes.Any2Double(oValue)
                                If dblCovergae > 0 Then
                                    dblCovergae = dblCovergae / 100.0
                                End If
                                cAssignment.Coverage = dblCovergae
                        End Select
                    End If
                Next

                oArrAssignments.Add(cIndex, cAssignment)
                cIndex = cIndex + 1
            End If
        Next

        Dim rAssignments As New Generic.List(Of roShiftAssignment)
        Dim oCountAssignment As Integer = 1
        For Each kvp As KeyValuePair(Of Integer, roShiftAssignment) In oArrAssignments
            Dim oAssignment As roShiftAssignment = kvp.Value
            oAssignment.IDShift = oCurrentShift.ID
            rAssignments.Add(oAssignment)
            oCountAssignment += 1
        Next kvp

        Return rAssignments.ToArray()
    End Function

    Private Function CreateShiftRulesJSON(ByVal oCurrentShift As roShift) As String
        Try
            Dim n As Integer = 0
            Dim oJSONField As roJSON

            Dim strJSONGroups As String = ""
            Dim strAux As String

            'Si el Shifte no te Layers, passem un objecte Json per omisio
            If oCurrentShift.SimpleRules IsNot Nothing AndAlso oCurrentShift.SimpleRules.Any() Then
                'Bucle al compositions
                For Each oRules As roShiftRule In oCurrentShift.SimpleRules
                    strJSONGroups &= "{ 'simplerules': ["
                    oJSONField = New roJSON
                    oJSONField.addField("IDShift", oRules.IDShift, Nothing, roJSON.JSONType.None_JSON)
                    oJSONField.addField("IDRule", oRules.ID, Nothing, roJSON.JSONType.None_JSON)
                    strAux = API.ShiftServiceMethods.GetShiftRuleDescription(Me.Page, oRules).Replace("'", "´")
                    oJSONField.addField("RuleDescription", strAux, Nothing, roJSON.JSONType.None_JSON)
                    oJSONField.addField("Incidence", oRules.IDIncidence, Nothing, roJSON.JSONType.None_JSON)
                    oJSONField.addField("Zone", oRules.IDZone, Nothing, roJSON.JSONType.None_JSON)
                    oJSONField.addField("ConditionValueType", oRules.ConditionValueType, Nothing, roJSON.JSONType.None_JSON)
                    oJSONField.addField("ToTime", Format(oRules.ToTime, HelperWeb.GetShortTimeFormat), Nothing, roJSON.JSONType.None_JSON)
                    oJSONField.addField("FromTime", Format(oRules.FromTime, HelperWeb.GetShortTimeFormat), Nothing, roJSON.JSONType.None_JSON)
                    oJSONField.addField("FromValueUserField", oRules.FromValueUserFieldName, Nothing, roJSON.JSONType.None_JSON)
                    oJSONField.addField("ToValueUserField", oRules.ToValueUserFieldName, Nothing, roJSON.JSONType.None_JSON)
                    oJSONField.addField("BetweenValueUserField", oRules.BetweenValueUserFieldName, Nothing, roJSON.JSONType.None_JSON)
                    oJSONField.addField("Cause", oRules.IDCause, Nothing, roJSON.JSONType.None_JSON)
                    oJSONField.addField("ActionValueType", oRules.ActionValueType, Nothing, roJSON.JSONType.None_JSON)
                    oJSONField.addField("MaxTime", Format(oRules.MaxTime, HelperWeb.GetShortTimeFormat), Nothing, roJSON.JSONType.None_JSON)
                    oJSONField.addField("MaxValueUserField", oRules.MaxValueUserFieldName, Nothing, roJSON.JSONType.None_JSON)

                    Dim strJSON As String = oJSONField.CreateJSON
                    strJSONGroups &= strJSON & " ]} ,"
                Next
            End If

            If strJSONGroups.EndsWith(",") Then strJSONGroups = strJSONGroups.Substring(0, Len(strJSONGroups) - 1)
            Return strJSONGroups.Replace("<br>", "").Replace("<br/>", "")
        Catch ex As Exception
            Return ""
        End Try
    End Function

    Private Function ParseResultRules(ByVal resultRules As String, ByVal oCurrentShift As roShift) As roShiftRule()
        Dim oArrRules As New Generic.SortedList(Of Integer, roShiftRule)

        Dim splitSep() As String = {"*&*"}
        Dim splitObjSep() As String = {"*|*"}

        Dim layers As String() = resultRules.Split(splitSep, System.StringSplitOptions.None)
        Dim cIndex As Integer = 0
        For Each oComp As String In layers
            If oComp <> String.Empty Then
                Dim orulesParameter As String() = oComp.Split(splitObjSep, System.StringSplitOptions.None)

                Dim cRule As New roShiftRule
                For Each oParameter As String In orulesParameter
                    If oParameter <> String.Empty Then
                        Dim oField As String = oParameter.Split("=")(0).ToUpper()
                        Dim oValue As String = oParameter.Split("=")(1)

                        Select Case oField
                            Case "CAUSE" '(28)
                                cRule.IDCause = roTypes.Any2Integer(oValue)
                            Case "FROMTIME" '00:00"
                                cRule.FromTime = roTypes.Any2DateTime(oValue) ' Format(oValue, HelperWeb.GetShortTimeFormat())
                            Case "IDRULE"
                                'nothing to do
                            Case "IDSHIFT" '(128)"
                                'nothin to do
                            Case "INCIDENCE" '(1031)"
                                cRule.IDIncidence = roTypes.Any2Integer(oValue)
                            Case "MAXTIME" '12:00"
                                cRule.MaxTime = roTypes.Any2DateTime(oValue) 'Format(oValue, HelperWeb.GetShortTimeFormat())
                            Case "TOTIME" '23:59"
                                cRule.ToTime = roTypes.Any2DateTime(oValue) 'Format(oValue, HelperWeb.GetShortTimeFormat())
                            Case "ZONE" '(0)"
                                cRule.IDZone = roTypes.Any2Integer(oValue)
                            Case "CONDITIONVALUETYPE"
                                cRule.ConditionValueType = roTypes.Any2Integer(oValue)
                            Case "FROMVALUEUSERFIELD"
                                cRule.FromValueUserFieldName = oValue
                            Case "TOVALUEUSERFIELD"
                                cRule.ToValueUserFieldName = oValue
                            Case "BETWEENVALUEUSERFIELD"
                                cRule.BetweenValueUserFieldName = oValue
                            Case "ACTIONVALUETYPE"
                                cRule.ActionValueType = roTypes.Any2Integer(oValue)
                            Case "MAXVALUEUSERFIELD"
                                cRule.MaxValueUserFieldName = oValue
                        End Select
                    End If
                Next

                oArrRules.Add(cIndex, cRule)
                cIndex = cIndex + 1
            End If
        Next

        'Carrega de les regles simples en el currentshift -------------------------------------------
        Dim resultSimpleRules As New Generic.List(Of roShiftRule)
        Dim oCountRule As Integer = 1
        For Each kvp As KeyValuePair(Of Integer, roShiftRule) In oArrRules
            Dim oRule As roShiftRule = kvp.Value
            oRule.IDShift = oCurrentShift.ID
            oRule.ID = oCountRule
            oRule.Type = ShiftRuleType.Simple
            resultSimpleRules.Add(oRule)
            oCountRule += 1
        Next kvp

        Return resultSimpleRules.ToArray()
    End Function

    Private Function retTLZonesJSON(ByVal oCurrentShift As roShift) As String
        Try
            Dim strJSONGroups As String = ""
            Dim oJSONField As roJSON

            Dim oData As roCollection
            Dim oDataChild As roCollection

            Dim oLayer As roShiftLayer
            Dim intLayerCounter As Integer

            intLayerCounter = 0

            If oCurrentShift.Layers IsNot Nothing Then
                For Each oLayer In oCurrentShift.Layers
                    Select Case oLayer.LayerType
                        Case roLayerTypes.roLTDailyTotalsFilter, roLayerTypes.roLTGroupFilter
                            Continue For
                    End Select

                    oData = New roCollection(oLayer.DataStoredXML)

                    oJSONField = Me.ParseLayersToJSON(oLayer, oData)

                    strJSONGroups &= "{ 'layers': ["
                    Dim strJSON As String = oJSONField.CreateJSON
                    strJSONGroups &= strJSON & " ]} ,"

                    If oLayer.ChildLayers IsNot Nothing Then
                        For Each oLayerChild As roShiftLayer In oLayer.ChildLayers
                            oDataChild = New roCollection(oLayerChild.DataStoredXML)

                            oJSONField = Me.ParseLayersToJSON(oLayerChild, oDataChild)

                            strJSONGroups &= "{ 'layers': ["
                            strJSON = oJSONField.CreateJSON
                            strJSONGroups &= strJSON & " ]} ,"
                        Next
                    End If
                Next

                If strJSONGroups.EndsWith(",") Then strJSONGroups = strJSONGroups.Substring(0, Len(strJSONGroups) - 1)
                Return strJSONGroups.Replace("<br>", "").Replace("<br/>", "")
            Else
                Return ""
            End If
        Catch ex As Exception
            Return ""
        End Try
    End Function

    Private Function retHourZonesJSON(ByVal oCurrentShift As roShift) As String
        Try
            Dim strJSONGroups As String = ""
            Dim oJSONField As roJSON

            Dim dTblTimeZones As DataTable = API.ShiftServiceMethods.GetTimeZones(Me)

            Dim oCount As Integer = 0
            Dim oldID As String = ""

            If oCurrentShift.TimeZones IsNot Nothing Then
                For Each oTimeZone In oCurrentShift.TimeZones

                    If oldID = oTimeZone.IDZone.ToString Then
                        oCount += 1
                    Else
                        oCount = 0
                    End If

                    oldID = oTimeZone.IDZone.ToString

                    oJSONField = Me.ParseTimeZonesToJSON(oTimeZone, dTblTimeZones, oCount)

                    strJSONGroups &= "{ 'layers': ["
                    Dim strJSON As String = oJSONField.CreateJSON
                    strJSONGroups &= strJSON & " ]} ,"
                Next

                If strJSONGroups.EndsWith(",") Then strJSONGroups = strJSONGroups.Substring(0, Len(strJSONGroups) - 1)
                Return strJSONGroups.Replace("<br>", "").Replace("<br/>", "")
            Else
                Return ""
            End If
        Catch ex As Exception
            Return ""
        End Try
    End Function

    Private Function ParseTimeZonesToJSON(ByVal oTimeZone As roShiftTimeZone, ByVal dtblTimeZones As DataTable, ByVal oCount As String) As roJSON
        Try
            Dim oJSONField As New roJSON

            Dim oDateBegin As Date = Nothing
            Dim oDateFinish As Date = Nothing
            Dim oMaxTime As Date = Nothing

            Dim oBeginDay As Integer = 1
            Dim oFinishDay As Integer = 1
            Dim oMaxTimeDay As Integer = 1
            Dim oFloatingBDay As Integer = 1

            Dim oValueDay As Integer = 1

            Dim oTitle As String = ""
            Dim dTblResult As DataRow() = dtblTimeZones.Select("ID = " & oTimeZone.IDZone.ToString)
            If dTblResult.Length > 0 Then
                oTitle = dTblResult(0)("Name")
            End If

            oJSONField.addField("TYPE", "HourZone" & oTimeZone.IDZone.ToString, Nothing, roJSON.JSONType.None_JSON)
            oJSONField.addField("LAYERID", oTimeZone.IDZone.ToString & "_" & oCount, Nothing, roJSON.JSONType.None_JSON)
            oJSONField.addField("TITLE", oTitle, Nothing, roJSON.JSONType.None_JSON)

            oDateBegin = oTimeZone.BeginTime
            If oDateBegin.Day = 29 Then oBeginDay = 0
            If oDateBegin.Day = 31 Then oBeginDay = 2
            oJSONField.addField("Begin", Format(oDateBegin, "HH:mm"), Nothing, roJSON.JSONType.None_JSON)
            oJSONField.addField("BeginDay", oBeginDay, Nothing, roJSON.JSONType.None_JSON)

            oDateFinish = oTimeZone.EndTime
            If oDateFinish.Day = 29 Then oFinishDay = 0
            If oDateFinish.Day = 31 Then oFinishDay = 2
            oJSONField.addField("Finish", Format(oDateFinish, "HH:mm"), Nothing, roJSON.JSONType.None_JSON)
            oJSONField.addField("FinishDay", oFinishDay, Nothing, roJSON.JSONType.None_JSON)

            'Diferencia entre dates
            If oDateBegin <> Nothing AndAlso oDateFinish <> Nothing Then
                If oBeginDay = 0 Then oDateBegin = roTypes.CreateDateTime(1989, 12, 29, oDateBegin.Hour, oDateBegin.Minute, 0)
                If oBeginDay = 1 Then oDateBegin = roTypes.CreateDateTime(1989, 12, 30, oDateBegin.Hour, oDateBegin.Minute, 0)
                If oBeginDay = 2 Then oDateBegin = roTypes.CreateDateTime(1989, 12, 31, oDateBegin.Hour, oDateBegin.Minute, 0)

                If oFinishDay = 0 Then oDateFinish = roTypes.CreateDateTime(1989, 12, 29, oDateFinish.Hour, oDateFinish.Minute, 0)
                If oFinishDay = 1 Then oDateFinish = roTypes.CreateDateTime(1989, 12, 30, oDateFinish.Hour, oDateFinish.Minute, 0)
                If oFinishDay = 2 Then oDateFinish = roTypes.CreateDateTime(1989, 12, 31, oDateFinish.Hour, oDateFinish.Minute, 0)

                Dim oMinDiff As Integer = DateDiff(DateInterval.Minute, oDateBegin, oDateFinish)
                oJSONField.addField("DateDiffMin", oMinDiff, Nothing, roJSON.JSONType.None_JSON)
            End If

            ' Zona bloqueada
            oJSONField.addField("IsLocked", oTimeZone.IsBlocked.ToString.ToLower, Nothing, roJSON.JSONType.None_JSON)

            Return oJSONField
        Catch ex As Exception
            Dim rError As New roJSON.JSONError(True, ex.Message.ToString & ex.StackTrace.ToString)
            Response.Write(rError.toJSON)
            Return Nothing
        End Try
    End Function

    Private Function ParseResultZones(ByVal resultTimeZones As String, ByVal oCurrentShift As roShift) As roShiftTimeZone()
        Dim oArrTimeZones As New Generic.SortedList(Of Integer, roShiftTimeZone)

        Dim splitSep() As String = {"*&*"}
        Dim splitObjSep() As String = {"*|*"}

        Dim layers As String() = resultTimeZones.Split(splitSep, System.StringSplitOptions.None)
        Dim cIndex As Integer = 0

        For Each oComp As String In layers
            If oComp <> String.Empty Then
                Dim oTimeZoneParameter As String() = oComp.Split(splitObjSep, System.StringSplitOptions.None)

                Dim cTimeZone As New roShiftTimeZone
                For Each oParameter As String In oTimeZoneParameter
                    If oParameter <> String.Empty Then
                        Dim oField As String = oParameter.Split("=")(0).ToUpper()
                        Dim oValue As String = oParameter.Split("=")(1)

                        Select Case oField
                            Case "BEGIN"  '08:00
                                Dim dBegin As DateTime
                                Dim dHour As String = oValue.Split(":")(0)
                                Dim dMinute As String = oValue.Split(":")(1)
                                Dim dDay As Integer = 30
                                Dim oBeginDay As String = GetParameterFromCollection("BEGINDAY", oTimeZoneParameter)
                                If oBeginDay = "0" Then dDay = 29
                                If oBeginDay = "2" Then dDay = 31
                                dBegin = roTypes.CreateDateTime(1899, 12, dDay, dHour, dMinute, 0)
                                cTimeZone.BeginTime = dBegin
                            Case "FINISH" '10:00"
                                Dim dFinish As DateTime
                                Dim dHour As String = oValue.Split(":")(0)
                                Dim dMinute As String = oValue.Split(":")(1)
                                Dim dDay As Integer = 30
                                Dim oBeginDay As String = GetParameterFromCollection("FINISHDAY", oTimeZoneParameter)
                                If oBeginDay = "0" Then dDay = 29
                                If oBeginDay = "2" Then dDay = 31
                                dFinish = roTypes.CreateDateTime(1899, 12, dDay, dHour, dMinute, 0)
                                cTimeZone.EndTime = dFinish
                            Case "FINISHDAY" '(1)"
                            Case "TLID" '1_0"
                                cTimeZone.IDZone = oValue.Split("_")(0)
                            Case "TLTYPE" '(HourZone1)"
                            Case "ISLOCKED"
                                cTimeZone.IsBlocked = IIf(oValue.ToString.ToUpper = "TRUE", True, False)
                        End Select
                    End If
                Next

                oArrTimeZones.Add(cIndex, cTimeZone)
                cIndex = cIndex + 1
            End If
        Next

        'Carrega de les zones en el currentshift ------------------------------------------
        Dim resultZones As New Generic.List(Of roShiftTimeZone)
        Dim oCountZone As Integer = 1
        For Each kvp As KeyValuePair(Of Integer, roShiftTimeZone) In oArrTimeZones
            Dim oZone As roShiftTimeZone = kvp.Value
            oZone.IDShift = oCurrentShift.ID
            resultZones.Add(oZone)
            oCountZone += 1
        Next kvp

        Return resultZones.ToArray()
    End Function

    Private Function ParseLayersToJSON(ByRef oLayer As roShiftLayer, ByVal oData As roCollection) As roJSON
        Try
            Dim oJSONField As New roJSON

            oJSONField.addField("TYPE", System.Enum.GetName(GetType(roLayerTypes), oLayer.LayerType).ToString, Nothing, roJSON.JSONType.None_JSON)

            oJSONField.addField("LAYERID", oLayer.ID, Nothing, roJSON.JSONType.None_JSON)

            If oLayer.ParentID > 0 Then
                oJSONField.addField("PARENTID", oLayer.ParentID, Nothing, roJSON.JSONType.None_JSON)
            End If

            Dim oDateBegin As Date = Nothing
            Dim oDateFinish As Date = Nothing
            Dim oMaxTime As Date = Nothing

            Dim oBeginDay As Integer = 1
            Dim oFinishDay As Integer = 1
            Dim oMaxTimeDay As Integer = 1
            Dim oFloatingBDay As Integer = 1

            Dim oValueDay As Integer = 1

            If oData.Exists(oLayer.XmlKey(roXmlLayerKeys.AllowModifyIniHour)) Then
                Dim oString As String = oData.Item(oLayer.XmlKey(roXmlLayerKeys.AllowModifyIniHour))
                oJSONField.addField("AllowModIni", oString, Nothing, roJSON.JSONType.None_JSON)
                'txtRestHourEnd.Text = Format(xDate, "HH:mm")
            End If

            If oData.Exists(oLayer.XmlKey(roXmlLayerKeys.AllowModifyDuration)) Then
                Dim oString As String = oData.Item(oLayer.XmlKey(roXmlLayerKeys.AllowModifyDuration))
                oJSONField.addField("AllowModDuration", oString, Nothing, roJSON.JSONType.None_JSON)
                'txtRestHourEnd.Text = Format(xDate, "HH:mm")
            End If

            If oData.Exists(oLayer.XmlKey(roXmlLayerKeys.Action)) Then
                Dim oString As String = oData.Item(oLayer.XmlKey(roXmlLayerKeys.Action))
                oJSONField.addField("Action", oString, Nothing, roJSON.JSONType.None_JSON)
                'txtRestHourEnd.Text = Format(xDate, "HH:mm")
            End If

            If oData.Exists(oLayer.XmlKey(roXmlLayerKeys.Begin)) Then
                oDateBegin = oData.Item(oLayer.XmlKey(roXmlLayerKeys.Begin))
                If oDateBegin.Day = 29 Then oBeginDay = 0
                If oDateBegin.Day = 31 Then oBeginDay = 2
                If oDateBegin.Day = 1 Then oBeginDay = 3
                oJSONField.addField("Begin", Format(oDateBegin, "HH:mm"), Nothing, roJSON.JSONType.None_JSON)
                oJSONField.addField("BeginDay", oBeginDay, Nothing, roJSON.JSONType.None_JSON)
                'txtRestHourBegin.Text = Format(xDate, "HH:mm")
            End If

            If oData.Exists(oLayer.XmlKey(roXmlLayerKeys.Finish)) Then
                oDateFinish = oData.Item(oLayer.XmlKey(roXmlLayerKeys.Finish))
                If oDateFinish.Day = 29 Then oFinishDay = 0
                If oDateFinish.Day = 31 Then oFinishDay = 2
                If oDateFinish.Day = 1 Then oFinishDay = 3
                oJSONField.addField("Finish", Format(oDateFinish, "HH:mm"), Nothing, roJSON.JSONType.None_JSON)
                oJSONField.addField("FinishDay", oFinishDay, Nothing, roJSON.JSONType.None_JSON)
                'txtRestHourEnd.Text = Format(xDate, "HH:mm")
            End If

            'Diferencia entre dates
            If oDateBegin <> Nothing AndAlso oDateFinish <> Nothing Then
                If oBeginDay = 0 Then oDateBegin = roTypes.CreateDateTime(1989, 12, 29, oDateBegin.Hour, oDateBegin.Minute, 0)
                If oBeginDay = 1 Then oDateBegin = roTypes.CreateDateTime(1989, 12, 30, oDateBegin.Hour, oDateBegin.Minute, 0)
                If oBeginDay = 2 Then oDateBegin = roTypes.CreateDateTime(1989, 12, 31, oDateBegin.Hour, oDateBegin.Minute, 0)

                If oFinishDay = 0 Then oDateFinish = roTypes.CreateDateTime(1989, 12, 29, oDateFinish.Hour, oDateFinish.Minute, 0)
                If oFinishDay = 1 Then oDateFinish = roTypes.CreateDateTime(1989, 12, 30, oDateFinish.Hour, oDateFinish.Minute, 0)
                If oFinishDay = 2 Then oDateFinish = roTypes.CreateDateTime(1989, 12, 31, oDateFinish.Hour, oDateFinish.Minute, 0)

                Dim oMinDiff As Integer = DateDiff(DateInterval.Minute, oDateBegin, oDateFinish)
                oJSONField.addField("DateDiffMin", oMinDiff, Nothing, roJSON.JSONType.None_JSON)
            End If

            If oData.Exists(oLayer.XmlKey(roXmlLayerKeys.FloatingBeginUpTo)) Then
                Dim oFloatingB As Date = oData.Item(oLayer.XmlKey(roXmlLayerKeys.FloatingBeginUpTo))
                If oFloatingB.Day = 29 Then oFloatingBDay = 0
                If oFloatingB.Day = 31 Then oFloatingBDay = 2
                oJSONField.addField("FloatingBeginUpTo", Format(oFloatingB, "HH:mm"), Nothing, roJSON.JSONType.None_JSON)
                oJSONField.addField("FloatingBeginUpToDay", oFloatingBDay, Nothing, roJSON.JSONType.None_JSON)
            End If

            If oData.Exists(oLayer.XmlKey(roXmlLayerKeys.FloatingFinishMinutes)) Then
                Dim oString As String = oData.Item(oLayer.XmlKey(roXmlLayerKeys.FloatingFinishMinutes))
                oJSONField.addField("FloatingFinishMinutes", oString, Nothing, roJSON.JSONType.None_JSON)
            End If

            If oData.Exists(oLayer.XmlKey(roXmlLayerKeys.MaxBreakTime)) Then
                Dim oDateMax As Date = oData.Item(oLayer.XmlKey(roXmlLayerKeys.MaxBreakTime))
                oJSONField.addField("MaxBreakTime", Format(oDateMax, "HH:mm"), Nothing, roJSON.JSONType.None_JSON)
                'txtRestHourEnd.Text = Format(xDate, "HH:mm")
            End If

            If oData.Exists(oLayer.XmlKey(roXmlLayerKeys.NotificationForUser)) Then
                Dim oString As String = oData.Item(oLayer.XmlKey(roXmlLayerKeys.NotificationForUser))
                oJSONField.addField("NotificationForUser", oString, Nothing, roJSON.JSONType.None_JSON)
            End If
            If oData.Exists(oLayer.XmlKey(roXmlLayerKeys.NotificationForSupervisor)) Then
                Dim oString As String = oData.Item(oLayer.XmlKey(roXmlLayerKeys.NotificationForSupervisor))
                oJSONField.addField("NotificationForSupervisor", oString, Nothing, roJSON.JSONType.None_JSON)
            End If

            If oData.Exists(oLayer.XmlKey(roXmlLayerKeys.NotificationForUserBeforeTime)) Then
                Dim oDateMax As Date = oData.Item(oLayer.XmlKey(roXmlLayerKeys.NotificationForUserBeforeTime))
                oJSONField.addField("NotificationForUserBeforeTime", Format(oDateMax, "HH:mm"), Nothing, roJSON.JSONType.None_JSON)
            End If

            If oData.Exists(oLayer.XmlKey(roXmlLayerKeys.NotificationForUserAfterTime)) Then
                Dim oDateMax As Date = oData.Item(oLayer.XmlKey(roXmlLayerKeys.NotificationForUserAfterTime))
                oJSONField.addField("NotificationForUserAfterTime", Format(oDateMax, "HH:mm"), Nothing, roJSON.JSONType.None_JSON)
            End If

            If oData.Exists(oLayer.XmlKey(roXmlLayerKeys.RealBegin)) Then
                Dim oDateMax As Date = oData.Item(oLayer.XmlKey(roXmlLayerKeys.RealBegin))
                oJSONField.addField("RealBegin", Format(oDateMax, "HH:mm"), Nothing, roJSON.JSONType.None_JSON)
            End If

            If oData.Exists(oLayer.XmlKey(roXmlLayerKeys.RealFinish)) Then
                Dim oDateMax As Date = oData.Item(oLayer.XmlKey(roXmlLayerKeys.RealFinish))
                oJSONField.addField("RealFinish", Format(oDateMax, "HH:mm"), Nothing, roJSON.JSONType.None_JSON)
            End If

            If oData.Exists(oLayer.XmlKey(roXmlLayerKeys.MaxTime)) Then
                oMaxTime = oData.Item(oLayer.XmlKey(roXmlLayerKeys.MaxTime))
                oJSONField.addField("MaxTime", Format(oMaxTime, "HH:mm"), Nothing, roJSON.JSONType.None_JSON)
            End If

            If oData.Exists(oLayer.XmlKey(roXmlLayerKeys.MaxTimeAction)) Then
                Dim oString As String = oData.Item(oLayer.XmlKey(roXmlLayerKeys.MaxTimeAction))
                oJSONField.addField("MaxTimeAction", oString, Nothing, roJSON.JSONType.None_JSON)
                'txtRestHourEnd.Text = Format(xDate, "HH:mm")
            End If

            If oData.Exists(oLayer.XmlKey(roXmlLayerKeys.MinTime)) Then
                Dim oMinTime As Date = oData.Item(oLayer.XmlKey(roXmlLayerKeys.MinTime))
                oJSONField.addField("MinTime", Format(oMinTime, "HH:mm"), Nothing, roJSON.JSONType.None_JSON)
                'txtRestHourEnd.Text = Format(xDate, "HH:mm")
            End If

            If oData.Exists(oLayer.XmlKey(roXmlLayerKeys.MinBreakTime)) Then
                Dim MinBreakTime As Date = oData.Item(oLayer.XmlKey(roXmlLayerKeys.MinBreakTime))
                oJSONField.addField("MinBreakTime", Format(MinBreakTime, "HH:mm"), Nothing, roJSON.JSONType.None_JSON)
            End If

            If oData.Exists(oLayer.XmlKey(roXmlLayerKeys.MinBreakAction)) Then
                Dim MinBreakAction As String = oData.Item(oLayer.XmlKey(roXmlLayerKeys.MinBreakAction))
                oJSONField.addField("MinBreakAction", MinBreakAction, Nothing, roJSON.JSONType.None_JSON)
            End If

            If oData.Exists(oLayer.XmlKey(roXmlLayerKeys.NoPunchBreakTime)) Then
                Dim NoPunchBreakTime As Date = oData.Item(oLayer.XmlKey(roXmlLayerKeys.NoPunchBreakTime))
                oJSONField.addField("NoPunchBreakTime", Format(NoPunchBreakTime, "HH:mm"), Nothing, roJSON.JSONType.None_JSON)
            End If

            If oData.Exists(oLayer.XmlKey(roXmlLayerKeys.Target)) Then
                Dim oString As String = oData.Item(oLayer.XmlKey(roXmlLayerKeys.Target))
                oJSONField.addField("Target", oString, Nothing, roJSON.JSONType.None_JSON)
                'txtRestHourEnd.Text = Format(xDate, "HH:mm")
            End If

            If oData.Exists(oLayer.XmlKey(roXmlLayerKeys.Value)) Then
                Dim oValue As Date = oData.Item(oLayer.XmlKey(roXmlLayerKeys.Value))
                If oValue.Day = 29 Then oValueDay = 0
                If oValue.Day = 31 Then oValueDay = 2
                oJSONField.addField("Value", Format(oValue, "HH:mm"), Nothing, roJSON.JSONType.None_JSON)
                oJSONField.addField("ValueDay", oValueDay, Nothing, roJSON.JSONType.None_JSON)
                'txtRestHourEnd.Text = Format(xDate, "HH:mm")
            End If

            Return oJSONField
        Catch ex As Exception
            Dim rError As New roJSON.JSONError(True, ex.Message.ToString & ex.StackTrace.ToString)
            Response.Write(rError.toJSON)
            Return Nothing
        End Try
    End Function

    Private Function ParseResultLayers(ByVal resultLayers As String, ByVal olayerTypes As roShiftLayer, ByVal oCurrentShift As roShift,
                                       ByVal oGenericData As roCollection, ByVal oGroupFilter1 As roCollection, ByVal oGroupFilter2 As roCollection, ByVal oGroupFilter3 As roCollection) As roShiftLayer()
        'Layers per guardar les capes tipades
        Dim oArrLayers As New Generic.SortedList(Of String, roShiftLayer)
        Dim oArrLayers_Data As New Generic.SortedList(Of String, roCollection)

        Dim oArrChildLayers_WorkingData As New Generic.SortedList(Of String, roCollection)

        Dim oArrChildLayers_Unit1020Data As New Generic.SortedList(Of String, roCollection)
        Dim oArrChildLayers_Unit1021Data As New Generic.SortedList(Of String, roCollection)
        Dim oArrChildLayers_Unit1022Data As New Generic.SortedList(Of String, roCollection)

        Dim oArrChildLayers_Paid1040Data As New Generic.SortedList(Of String, roCollection)

        Dim bolMinBreakAction = False
        Dim bolMinBreakTime = False

        Dim bolCheckIni = False
        Dim bolCheckDuration = False

        Dim splitSep() As String = {"*&*"}
        Dim splitObjSep() As String = {"*|*"}

        Dim layers As String() = resultLayers.Split(splitSep, System.StringSplitOptions.None)
        Dim intlayer As Integer = 1
        For Each oComp As String In layers
            If oComp <> String.Empty Then
                Dim olayersParameter As String() = oComp.Split(splitObjSep, System.StringSplitOptions.None)

                Dim oCurLayer = New roShiftLayer
                Dim oCurCollection As New roCollection
                oCurLayer.ID = intlayer ' roTypes.Any2Integer(GetParameterFromCollection("TLID", olayersParameter))

                For Each oParameter As String In olayersParameter
                    If oParameter <> String.Empty Then
                        Dim oField As String = oParameter.Split("=")(0).ToUpper()
                        Dim oValue As String = oParameter.Split("=")(1)

                        Select Case oField
                            Case "BEGIN"    '08:00
                                Dim dHour As String = oValue.Split(":")(0)
                                Dim dMinute As String = oValue.Split(":")(1)
                                Dim day As Integer = 30
                                Dim oBeginDay As String = GetParameterFromCollection("BEGINDAY", olayersParameter)
                                If oBeginDay = "0" Then day = 29
                                If oBeginDay = "2" Then day = 31
                                If oBeginDay = "3" Then day = 1
                                oCurCollection.Add(olayerTypes.XmlKey(roXmlLayerKeys.Begin), roTypes.CreateDateTime(1899, 12, day, dHour, dMinute, 0))
                            Case "REALBEGIN"    '08:00
                                Dim dHour As String = oValue.Split(":")(0)
                                Dim dMinute As String = oValue.Split(":")(1)
                                Dim day As Integer = 30
                                Dim oBeginDay As String = GetParameterFromCollection("BEGINDAY", olayersParameter)
                                If oBeginDay = "0" Then day = 29
                                If oBeginDay = "2" Then day = 31
                                If oBeginDay = "3" Then day = 1
                                oCurCollection.Add(olayerTypes.XmlKey(roXmlLayerKeys.RealBegin), roTypes.CreateDateTime(1899, 12, day, dHour, dMinute, 0))
                            Case "DATEDIFFMIN"
                            Case "FINISH"
                                Dim dHour As String = oValue.Split(":")(0)
                                Dim dMinute As String = oValue.Split(":")(1)
                                Dim day As Integer = 30
                                Dim oFinishDay As String = GetParameterFromCollection("FINISHDAY", olayersParameter)
                                If oFinishDay = "0" Then day = 29
                                If oFinishDay = "2" Then day = 31
                                If oFinishDay = "3" Then day = 1
                                oCurCollection.Add(olayerTypes.XmlKey(roXmlLayerKeys.Finish), roTypes.CreateDateTime(1899, 12, day, dHour, dMinute, 0))
                            Case "REALFINISH"
                                Dim dHour As String = oValue.Split(":")(0)
                                Dim dMinute As String = oValue.Split(":")(1)
                                Dim day As Integer = 30
                                Dim oFinishDay As String = GetParameterFromCollection("FINISHDAY", olayersParameter)
                                If oFinishDay = "0" Then day = 29
                                If oFinishDay = "2" Then day = 31
                                If oFinishDay = "3" Then day = 1
                                oCurCollection.Add(olayerTypes.XmlKey(roXmlLayerKeys.RealFinish), roTypes.CreateDateTime(1899, 12, day, dHour, dMinute, 0))
                            Case "TLTYPE"
                                Select Case oValue.ToUpper
                                    Case "ROLTMANDATORY"
                                        oCurLayer.LayerType = roLayerTypes.roLTMandatory
                                    Case "ROLTWORKING"
                                        oCurLayer.LayerType = roLayerTypes.roLTWorking
                                    Case "ROLTBREAK"
                                        oCurLayer.LayerType = roLayerTypes.roLTBreak
                                End Select
                            Case "MAXBREAKACTION" '(CreateIncidence)"
                                oCurCollection.Add(olayerTypes.XmlKey(roXmlLayerKeys.MaxBreakAction), oValue)
                            Case "MINBREAKACTION" '(CreateIncidence)"
                                oCurCollection.Add(olayerTypes.XmlKey(roXmlLayerKeys.MinBreakAction), oValue)
                            Case "ALLOWMODINI" '(Allow modify Initial Hour)"
                                If (oValue OrElse bolCheckIni) Then
                                    bolCheckIni = True
                                    oCurCollection.Add(olayerTypes.XmlKey(roXmlLayerKeys.AllowModifyIniHour), bolCheckIni)
                                End If
                            Case "ALLOWMODDURATION" '(Allow modify duration shift)"
                                If (oValue OrElse bolCheckDuration) Then
                                    bolCheckDuration = True
                                    oCurCollection.Add(olayerTypes.XmlKey(roXmlLayerKeys.AllowModifyDuration), bolCheckDuration)
                                End If

                            Case "WORKING_BEGIN" '08:00

                                Dim dWorking_Begin As DateTime
                                Dim dHour As String = oValue.Split(":")(0)
                                Dim dMinute As String = oValue.Split(":")(1)
                                Dim dDay As Integer = 30

                                Dim oBeginDayValue As String = GetParameterFromCollection("WORKING_BEGINDAY", olayersParameter)
                                If oBeginDayValue = String.Empty Then
                                    If oCurCollection(olayerTypes.XmlKey(roXmlLayerKeys.Begin)) IsNot Nothing Then
                                        dDay = oCurCollection(olayerTypes.XmlKey(roXmlLayerKeys.Begin)).day
                                    End If
                                Else
                                    If oBeginDayValue = "0" Then dDay = 29
                                    If oBeginDayValue = "2" Then dDay = 31
                                End If
                                dWorking_Begin = roTypes.CreateDateTime(1899, 12, dDay, dHour, dMinute, 0)

                                If Not oArrChildLayers_WorkingData.ContainsKey(oCurLayer.ID) Then
                                    oArrChildLayers_WorkingData.Add(oCurLayer.ID, New roCollection)
                                End If

                                If dDay = 30 Then
                                    oArrChildLayers_WorkingData(oCurLayer.ID).Add(olayerTypes.XmlKey(roXmlLayerKeys.Begin), Format(dWorking_Begin, "HH:mm"))
                                Else
                                    oArrChildLayers_WorkingData(oCurLayer.ID).Add(olayerTypes.XmlKey(roXmlLayerKeys.Begin), dWorking_Begin)
                                End If
                            Case "WORKING_FINISH"
                                Dim dWorking_Finish As DateTime
                                Dim dHour As String = oValue.Split(":")(0)
                                Dim dMinute As String = oValue.Split(":")(1)
                                Dim dDay As Integer = 30

                                Dim oFinishDayValue As String = GetParameterFromCollection("WORKING_FINISHDAY", olayersParameter)
                                If oFinishDayValue = String.Empty Then
                                    If oCurCollection(olayerTypes.XmlKey(roXmlLayerKeys.Finish)) IsNot Nothing Then
                                        dDay = oCurCollection(olayerTypes.XmlKey(roXmlLayerKeys.Finish)).day
                                    End If
                                Else
                                    If oFinishDayValue = "0" Then dDay = 29
                                    If oFinishDayValue = "2" Then dDay = 31
                                End If
                                dWorking_Finish = roTypes.CreateDateTime(1899, 12, dDay, dHour, dMinute, 0)

                                If Not oArrChildLayers_WorkingData.ContainsKey(oCurLayer.ID) Then
                                    oArrChildLayers_WorkingData.Add(oCurLayer.ID, New roCollection)
                                End If

                                If dDay = 30 Then
                                    oArrChildLayers_WorkingData(oCurLayer.ID).Add(olayerTypes.XmlKey(roXmlLayerKeys.Finish), Format(dWorking_Finish, "HH:mm"))
                                Else
                                    oArrChildLayers_WorkingData(oCurLayer.ID).Add(olayerTypes.XmlKey(roXmlLayerKeys.Finish), dWorking_Finish)
                                End If
                            Case "WORKING_MAXTIME"
                                If Not oArrChildLayers_WorkingData.ContainsKey(oCurLayer.ID) Then
                                    oArrChildLayers_WorkingData.Add(oCurLayer.ID, New roCollection)
                                End If
                                oArrChildLayers_WorkingData(oCurLayer.ID).Add(olayerTypes.XmlKey(roXmlLayerKeys.MaxTime), oValue)
                            Case "WORKING_MAXTIMEACTION" '(Overtime)
                                If Not oArrChildLayers_WorkingData.ContainsKey(oCurLayer.ID) Then
                                    oArrChildLayers_WorkingData.Add(oCurLayer.ID, New roCollection)
                                End If
                                oArrChildLayers_WorkingData(oCurLayer.ID).Add(olayerTypes.XmlKey(roXmlLayerKeys.MaxTimeAction), oValue)
                            Case "WORKING_MINTIME" ' 00:15
                                If Not oArrChildLayers_WorkingData.ContainsKey(oCurLayer.ID) Then
                                    oArrChildLayers_WorkingData.Add(oCurLayer.ID, New roCollection)
                                End If
                                oArrChildLayers_WorkingData(oCurLayer.ID).Add(olayerTypes.XmlKey(roXmlLayerKeys.MinTime), oValue)
                            Case "FLOATINGBEGINUPTO" ' 10:30"
                                Dim dFloating As DateTime
                                Dim dHour As String = oValue.Split(":")(0)
                                Dim dMinute As String = oValue.Split(":")(1)
                                Dim dDay As Integer = 30
                                Dim oFloatingDayValue As String = GetParameterFromCollection("FLOATINGBEGINUPTODAY", olayersParameter)

                                If oFloatingDayValue = "0" Then dDay = 29
                                If oFloatingDayValue = "2" Then dDay = 31
                                dFloating = roTypes.CreateDateTime(1899, 12, dDay, dHour, dMinute, 0)

                                oCurCollection.Add(olayerTypes.XmlKey(roXmlLayerKeys.FloatingBeginUpTo), dFloating)
                            Case "FLOATINGBEGINUPTODAY" ' (1)"
                            Case "FloatingFinishMinutes".ToUpper
                                Dim intMinutes As Integer = roTypes.Any2Integer(oValue)
                                oCurCollection.Add(olayerTypes.XmlKey(roXmlLayerKeys.FloatingFinishMinutes), intMinutes)
                            Case "UNIT_1020_ACTION" '(TreatAsWork)"
                                If Not oArrChildLayers_Unit1020Data.ContainsKey(oCurLayer.ID) Then
                                    oArrChildLayers_Unit1020Data.Add(oCurLayer.ID, New roCollection)
                                End If
                                oArrChildLayers_Unit1020Data(oCurLayer.ID).Add(olayerTypes.XmlKey(roXmlLayerKeys.Action), oValue)
                            Case "UNIT_1020_BEGIN" '10:00"

                                Dim d1020_Begin As DateTime
                                Dim dHour As String = oValue.Split(":")(0)
                                Dim dMinute As String = oValue.Split(":")(1)
                                Dim dDay As Integer = 30
                                Dim oBeginDayValue As String = GetParameterFromCollection("UNIT_1020_BEGINDAY", olayersParameter)
                                If oBeginDayValue = "0" Then dDay = 29
                                If oBeginDayValue = "2" Then dDay = 31
                                d1020_Begin = roTypes.CreateDateTime(1899, 12, dDay, dHour, dMinute, 0)

                                If Not oArrChildLayers_Unit1020Data.ContainsKey(oCurLayer.ID) Then
                                    oArrChildLayers_Unit1020Data.Add(oCurLayer.ID, New roCollection)
                                End If
                                oArrChildLayers_Unit1020Data(oCurLayer.ID).Add(olayerTypes.XmlKey(roXmlLayerKeys.Begin), d1020_Begin)
                            Case "UNIT_1020_FINISH" '19:00"
                                Dim d1020_Finish As DateTime
                                Dim dHour As String = oValue.Split(":")(0)
                                Dim dMinute As String = oValue.Split(":")(1)
                                Dim dDay As Integer = 30
                                Dim oFinishDayValue As String = GetParameterFromCollection("UNIT_1020_FINISHDAY", olayersParameter)
                                If oFinishDayValue = "0" Then dDay = 29
                                If oFinishDayValue = "2" Then dDay = 31
                                d1020_Finish = roTypes.CreateDateTime(1899, 12, dDay, dHour, dMinute, 0)

                                If Not oArrChildLayers_Unit1020Data.ContainsKey(oCurLayer.ID) Then
                                    oArrChildLayers_Unit1020Data.Add(oCurLayer.ID, New roCollection)
                                End If
                                oArrChildLayers_Unit1020Data(oCurLayer.ID).Add(olayerTypes.XmlKey(roXmlLayerKeys.Finish), d1020_Finish)
                            Case "UNIT_1020_VALUE" '00:00"

                                Dim d1020_Value As DateTime
                                Dim dHour As String = oValue.Split(":")(0)
                                Dim dMinute As String = oValue.Split(":")(1)
                                Dim dDay As Integer = 30
                                Dim oDayValue As String = GetParameterFromCollection("UNIT_1020_VALUEDAY", olayersParameter)
                                If oDayValue = "0" Then dDay = 29
                                If oDayValue = "2" Then dDay = 31
                                d1020_Value = roTypes.CreateDateTime(1899, 12, dDay, dHour, dMinute, 0)

                                If Not oArrChildLayers_Unit1020Data.ContainsKey(oCurLayer.ID) Then
                                    oArrChildLayers_Unit1020Data.Add(oCurLayer.ID, New roCollection)
                                End If
                                oArrChildLayers_Unit1020Data(oCurLayer.ID).Add(olayerTypes.XmlKey(roXmlLayerKeys.Value), Format(d1020_Value, "H:mm:ss"))
                            Case "UNIT_1021_ACTION" '(Ignore)"
                                If Not oArrChildLayers_Unit1021Data.ContainsKey(oCurLayer.ID) Then
                                    oArrChildLayers_Unit1021Data.Add(oCurLayer.ID, New roCollection)
                                End If
                                oArrChildLayers_Unit1021Data(oCurLayer.ID).Add(olayerTypes.XmlKey(roXmlLayerKeys.Action), oValue)
                            Case "UNIT_1021_BEGIN" '10:00"
                                Dim d1021_Begin As DateTime
                                Dim dHour As String = oValue.Split(":")(0)
                                Dim dMinute As String = oValue.Split(":")(1)
                                Dim dDay As Integer = 30
                                Dim oDayValue As String = GetParameterFromCollection("UNIT_1021_BEGINDAY", olayersParameter)
                                If oDayValue = "0" Then dDay = 29
                                If oDayValue = "2" Then dDay = 31
                                d1021_Begin = roTypes.CreateDateTime(1899, 12, dDay, dHour, dMinute, 0)

                                If Not oArrChildLayers_Unit1021Data.ContainsKey(oCurLayer.ID) Then
                                    oArrChildLayers_Unit1021Data.Add(oCurLayer.ID, New roCollection)
                                End If
                                oArrChildLayers_Unit1021Data(oCurLayer.ID).Add(olayerTypes.XmlKey(roXmlLayerKeys.Begin), d1021_Begin)
                            Case "UNIT_1021_FINISH" '19:00"
                                Dim d1021_Finish As DateTime
                                Dim dHour As String = oValue.Split(":")(0)
                                Dim dMinute As String = oValue.Split(":")(1)
                                Dim dDay As Integer = 30
                                Dim oDayValue As String = GetParameterFromCollection("UNIT_1021_FINISHDAY", olayersParameter)
                                If oDayValue = "0" Then dDay = 29
                                If oDayValue = "2" Then dDay = 31
                                d1021_Finish = roTypes.CreateDateTime(1899, 12, dDay, dHour, dMinute, 0)

                                If Not oArrChildLayers_Unit1021Data.ContainsKey(oCurLayer.ID) Then
                                    oArrChildLayers_Unit1021Data.Add(oCurLayer.ID, New roCollection)
                                End If
                                oArrChildLayers_Unit1021Data(oCurLayer.ID).Add(olayerTypes.XmlKey(roXmlLayerKeys.Finish), d1021_Finish)
                            Case "UNIT_1021_VALUE" '02:22"
                                Dim d1021_Value As DateTime
                                Dim dHour As String = oValue.Split(":")(0)
                                Dim dMinute As String = oValue.Split(":")(1)
                                Dim dDay As Integer = 30
                                Dim oDayValue As String = GetParameterFromCollection("UNIT_1021_VALUEDAY", olayersParameter)
                                If oDayValue = "0" Then dDay = 29
                                If oDayValue = "2" Then dDay = 31
                                d1021_Value = roTypes.CreateDateTime(1899, 12, dDay, dHour, dMinute, 0)

                                If Not oArrChildLayers_Unit1021Data.ContainsKey(oCurLayer.ID) Then
                                    oArrChildLayers_Unit1021Data.Add(oCurLayer.ID, New roCollection)
                                End If
                                oArrChildLayers_Unit1021Data(oCurLayer.ID).Add(olayerTypes.XmlKey(roXmlLayerKeys.Value), roTypes.Any2Time(d1021_Value).TimeOnly)

                            Case "UNIT_1021_VALUEDAY" '(1)"
                            Case "UNIT_1022_ACTION" '(Ignore)"

                                If Not oArrChildLayers_Unit1022Data.ContainsKey(oCurLayer.ID) Then
                                    oArrChildLayers_Unit1022Data.Add(oCurLayer.ID, New roCollection)
                                End If
                                oArrChildLayers_Unit1022Data(oCurLayer.ID).Add(olayerTypes.XmlKey(roXmlLayerKeys.Action), oValue)
                            Case "UNIT_1022_BEGIN" '10:00"

                                Dim d1022_Begin As DateTime
                                Dim dHour As String = oValue.Split(":")(0)
                                Dim dMinute As String = oValue.Split(":")(1)
                                Dim dDay As Integer = 30
                                Dim oDayValue As String = GetParameterFromCollection("UNIT_1022_BEGINDAY", olayersParameter)
                                If oDayValue = "0" Then dDay = 29
                                If oDayValue = "2" Then dDay = 31
                                d1022_Begin = roTypes.CreateDateTime(1899, 12, dDay, dHour, dMinute, 0)

                                If Not oArrChildLayers_Unit1022Data.ContainsKey(oCurLayer.ID) Then
                                    oArrChildLayers_Unit1022Data.Add(oCurLayer.ID, New roCollection)
                                End If
                                oArrChildLayers_Unit1022Data(oCurLayer.ID).Add(olayerTypes.XmlKey(roXmlLayerKeys.Begin), d1022_Begin)
                            Case "UNIT_1022_FINISH" '19:00"
                                Dim d1022_Finish As DateTime
                                Dim dHour As String = oValue.Split(":")(0)
                                Dim dMinute As String = oValue.Split(":")(1)
                                Dim dDay As Integer = 30
                                Dim oDayValue As String = GetParameterFromCollection("UNIT_1022_FINISHDAY", olayersParameter)
                                If oDayValue = "0" Then dDay = 29
                                If oDayValue = "2" Then dDay = 31
                                d1022_Finish = roTypes.CreateDateTime(1899, 12, dDay, dHour, dMinute, 0)

                                If Not oArrChildLayers_Unit1022Data.ContainsKey(oCurLayer.ID) Then
                                    oArrChildLayers_Unit1022Data.Add(oCurLayer.ID, New roCollection)
                                End If
                                oArrChildLayers_Unit1022Data(oCurLayer.ID).Add(olayerTypes.XmlKey(roXmlLayerKeys.Finish), d1022_Finish)
                            Case "UNIT_1022_VALUE" '03:33"

                                Dim d1022_Value As DateTime
                                Dim dHour As String = oValue.Split(":")(0)
                                Dim dMinute As String = oValue.Split(":")(1)
                                Dim dDay As Integer = 30
                                Dim oDayValue As String = GetParameterFromCollection("UNIT_1022_VALUEDAY", olayersParameter)
                                If oDayValue = "0" Then dDay = 29
                                If oDayValue = "2" Then dDay = 31
                                d1022_Value = roTypes.CreateDateTime(1899, 12, dDay, dHour, dMinute, 0)

                                If Not oArrChildLayers_Unit1022Data.ContainsKey(oCurLayer.ID) Then
                                    oArrChildLayers_Unit1022Data.Add(oCurLayer.ID, New roCollection)
                                End If
                                oArrChildLayers_Unit1022Data(oCurLayer.ID).Add(olayerTypes.XmlKey(roXmlLayerKeys.Value), roTypes.Any2Time(d1022_Value).TimeOnly)

                            Case "UNIT_1022_VALUEDAY" '(1)"

                            Case "MAXBREAKTIME" '07:19
                                oCurCollection.Add(olayerTypes.XmlKey(roXmlLayerKeys.MaxBreakTime), oValue)
                            Case "NOTIFICATIONFORUSER"
                                oCurCollection.Add(olayerTypes.XmlKey(roXmlLayerKeys.NotificationForUser), oValue)
                            Case "NOTIFICATIONFORUSERBEFORETIME"
                                oCurCollection.Add(olayerTypes.XmlKey(roXmlLayerKeys.NotificationForUserBeforeTime), oValue)
                            Case "NOTIFICATIONFORUSERAFTERTIME"
                                oCurCollection.Add(olayerTypes.XmlKey(roXmlLayerKeys.NotificationForUserAfterTime), oValue)
                            Case "NOTIFICATIONFORSUPERVISOR"
                                oCurCollection.Add(olayerTypes.XmlKey(roXmlLayerKeys.NotificationForSupervisor), oValue)
                            Case "REALBEGIN"
                                oCurCollection.Add(olayerTypes.XmlKey(roXmlLayerKeys.RealBegin), oValue)
                            Case "REALFINISH"
                                oCurCollection.Add(olayerTypes.XmlKey(roXmlLayerKeys.RealFinish), oValue)
                            Case "MINBREAKTIME" '00:33
                                oCurCollection.Add(olayerTypes.XmlKey(roXmlLayerKeys.MinBreakTime), oValue)
                            Case "NOPUNCHBREAKTIME" ' 00:55
                                oCurCollection.Add(olayerTypes.XmlKey(roXmlLayerKeys.NoPunchBreakTime), oValue)
                            Case "PAID_1040_BEGIN" '11:11"
                                Dim d1040_Begin As DateTime
                                Dim dHour As String = oValue.Split(":")(0)
                                Dim dMinute As String = oValue.Split(":")(1)
                                Dim dDay As Integer = 30
                                Dim oDayValue As String = GetParameterFromCollection("PAID_1040_BEGINDAY", olayersParameter)
                                If oDayValue = "0" Then dDay = 29
                                If oDayValue = "2" Then dDay = 31
                                If oDayValue = "3" Then dDay = 1
                                d1040_Begin = roTypes.CreateDateTime(1899, 12, dDay, dHour, dMinute, 0)

                                If Not oArrChildLayers_Paid1040Data.ContainsKey(oCurLayer.ID) Then
                                    oArrChildLayers_Paid1040Data.Add(oCurLayer.ID, New roCollection)
                                End If
                                oArrChildLayers_Paid1040Data(oCurLayer.ID).Add(olayerTypes.XmlKey(roXmlLayerKeys.Begin), d1040_Begin)

                            Case "PAID_1040_FINISH" ' 18:30"
                                Dim d1040_Finish As DateTime
                                Dim dHour As String = oValue.Split(":")(0)
                                Dim dMinute As String = oValue.Split(":")(1)
                                Dim dDay As Integer = 30
                                Dim oDayValue As String = GetParameterFromCollection("PAID_1040_FINISHDAY", olayersParameter)
                                If oDayValue = "0" Then dDay = 29
                                If oDayValue = "2" Then dDay = 31
                                If oDayValue = "3" Then dDay = 1
                                d1040_Finish = roTypes.CreateDateTime(1899, 12, dDay, dHour, dMinute, 0)

                                If Not oArrChildLayers_Paid1040Data.ContainsKey(oCurLayer.ID) Then
                                    oArrChildLayers_Paid1040Data.Add(oCurLayer.ID, New roCollection)
                                End If
                                oArrChildLayers_Paid1040Data(oCurLayer.ID).Add(olayerTypes.XmlKey(roXmlLayerKeys.Finish), d1040_Finish)
                            Case "PAID_1040_VALUE" '00:44"
                                Dim d1040_Value As DateTime
                                Dim dHour As String = oValue.Split(":")(0)
                                Dim dMinute As String = oValue.Split(":")(1)
                                Dim dDay As Integer = 30
                                d1040_Value = roTypes.CreateDateTime(1899, 12, dDay, dHour, dMinute, 0)

                                If Not oArrChildLayers_Paid1040Data.ContainsKey(oCurLayer.ID) Then
                                    oArrChildLayers_Paid1040Data.Add(oCurLayer.ID, New roCollection)
                                End If
                                oArrChildLayers_Paid1040Data(oCurLayer.ID).Add(olayerTypes.XmlKey(roXmlLayerKeys.Value), d1040_Value)
                        End Select
                    End If
                Next
                intlayer = intlayer + 1
                oArrLayers.Add(oCurLayer.ID, oCurLayer)
                oArrLayers_Data.Add(oCurLayer.ID, oCurCollection)
            End If
        Next

        'Carrega de les layers al currentshift --------------------------------------------
        Dim rLayers As New Generic.List(Of roShiftLayer)
        Dim oCountLayer As Integer = 0
        For Each kvp As KeyValuePair(Of String, roShiftLayer) In oArrLayers
            Dim oLayer As roShiftLayer = kvp.Value
            oLayer.IDShift = oCurrentShift.ID
            oCountLayer += 1
            oLayer.ID = oCountLayer
            oLayer.ParentID = 0
            oLayer.DataStoredXML = oArrLayers_Data(kvp.Key).XML
            If oLayer.ChildLayers Is Nothing Then oLayer.ChildLayers = New List(Of roShiftLayer)

            Select Case oLayer.LayerType
                Case roLayerTypes.roLTWorking ' ChildLayers Flexible --------------------------------------
                    If oArrChildLayers_WorkingData.ContainsKey(kvp.Key) Then

                        Dim tmpLayer As New roShiftLayer
                        tmpLayer.IDShift = oCurrentShift.ID
                        tmpLayer.ParentID = oLayer.ID
                        oCountLayer += 1
                        tmpLayer.ID = oCountLayer
                        tmpLayer.LayerType = roLayerTypes.roLTWorkingMaxMinFilter
                        tmpLayer.DataStoredXML = oArrChildLayers_WorkingData(kvp.Key).XML
                        oLayer.ChildLayers.Add(tmpLayer)
                    End If
                Case roLayerTypes.roLTMandatory ' ChildLayers Rigid -----------------------------------------

                    'Unit 1020 --------------------------------------------------------------------------------
                    If oArrChildLayers_Unit1020Data.ContainsKey(kvp.Key) Then
                        Dim tmpLayer As New roShiftLayer
                        tmpLayer.IDShift = oCurrentShift.ID
                        tmpLayer.ParentID = oLayer.ID
                        oCountLayer += 1
                        tmpLayer.ID = oCountLayer
                        tmpLayer.LayerType = roLayerTypes.roLTUnitFilter
                        oArrChildLayers_Unit1020Data(kvp.Key).Add(olayerTypes.XmlKey(roXmlLayerKeys.Target), 1020)
                        tmpLayer.DataStoredXML = oArrChildLayers_Unit1020Data(kvp.Key).XML
                        'tmpLayer.XmlKey = {"Begin", "Finish",
                        oLayer.ChildLayers.Add(tmpLayer)
                    End If

                    'Unit 1021 --------------------------------------------------------------------------------
                    If oArrChildLayers_Unit1021Data.ContainsKey(kvp.Key) Then
                        Dim tmpLayer As New roShiftLayer
                        tmpLayer.IDShift = oCurrentShift.ID
                        tmpLayer.ParentID = oLayer.ID
                        oCountLayer += 1
                        tmpLayer.ID = oCountLayer
                        tmpLayer.LayerType = roLayerTypes.roLTUnitFilter
                        oArrChildLayers_Unit1021Data(kvp.Key).Add(olayerTypes.XmlKey(roXmlLayerKeys.Target), 1021)
                        tmpLayer.DataStoredXML = oArrChildLayers_Unit1021Data(kvp.Key).XML
                        oLayer.ChildLayers.Add(tmpLayer)
                    End If

                    'Unit 1022 --------------------------------------------------------------------------------
                    If oArrChildLayers_Unit1022Data.ContainsKey(kvp.Key) Then
                        Dim tmpLayer As New roShiftLayer
                        tmpLayer = New roShiftLayer
                        tmpLayer.IDShift = oCurrentShift.ID
                        tmpLayer.ParentID = oLayer.ID
                        oCountLayer += 1
                        tmpLayer.ID = oCountLayer
                        tmpLayer.LayerType = roLayerTypes.roLTUnitFilter
                        oArrChildLayers_Unit1022Data(kvp.Key).Add(olayerTypes.XmlKey(roXmlLayerKeys.Target), 1022)
                        tmpLayer.DataStoredXML = oArrChildLayers_Unit1022Data(kvp.Key).XML
                        oLayer.ChildLayers.Add(tmpLayer)
                    End If
                Case roLayerTypes.roLTBreak ' Descans  -----------------------------------------------------------
                    'PaidTime
                    'Unit 1022
                    If oArrChildLayers_Paid1040Data.ContainsKey(kvp.Key) Then
                        Dim tmpLayer As New roShiftLayer
                        tmpLayer = New roShiftLayer
                        tmpLayer.IDShift = oCurrentShift.ID
                        tmpLayer.ParentID = oLayer.ID
                        oCountLayer += 1
                        tmpLayer.ID = oCountLayer
                        tmpLayer.LayerType = roLayerTypes.roLTPaidTime
                        oArrChildLayers_Paid1040Data(kvp.Key).Add(olayerTypes.XmlKey(roXmlLayerKeys.Target), "1040")
                        tmpLayer.DataStoredXML = oArrChildLayers_Paid1040Data(kvp.Key).XML
                        oLayer.ChildLayers.Add(tmpLayer)
                    End If
            End Select

            rLayers.Add(oLayer)
        Next kvp

        'Layer Definicio horari Generals
        If oGenericData IsNot Nothing Then
            Dim oGeneralLayer As New roShiftLayer
            oGeneralLayer.IDShift = oCurrentShift.ID
            oCountLayer += 1
            oGeneralLayer.ID = oCountLayer
            oGeneralLayer.LayerType = roLayerTypes.roLTDailyTotalsFilter
            oGeneralLayer.DataStoredXML = oGenericData.XML
            rLayers.Add(oGeneralLayer)
        End If

        'Filtre 1 General
        If oGroupFilter1 IsNot Nothing Then
            Dim oG1Layer As New roShiftLayer
            oG1Layer.IDShift = oCurrentShift.ID
            oCountLayer += 1
            oG1Layer.ID = oCountLayer
            oG1Layer.LayerType = roLayerTypes.roLTGroupFilter
            oGroupFilter1.Add(olayerTypes.XmlKey(roXmlLayerKeys.Target), "1020")
            oG1Layer.DataStoredXML = oGroupFilter1.XML
            rLayers.Add(oG1Layer)
        End If
        'Filtre 2 General
        If oGroupFilter2 IsNot Nothing Then
            Dim oG2Layer As New roShiftLayer
            oG2Layer.IDShift = oCurrentShift.ID
            oCountLayer += 1
            oG2Layer.ID = oCountLayer
            oG2Layer.LayerType = roLayerTypes.roLTGroupFilter
            oGroupFilter2.Add(olayerTypes.XmlKey(roXmlLayerKeys.Target), "1021")
            oG2Layer.DataStoredXML = oGroupFilter2.XML
            rLayers.Add(oG2Layer)
        End If
        'Filtre 3 General
        If oGroupFilter3 IsNot Nothing Then
            Dim oG3Layer As New roShiftLayer
            oG3Layer.IDShift = oCurrentShift.ID
            oCountLayer += 1
            oG3Layer.ID = oCountLayer
            oG3Layer.LayerType = roLayerTypes.roLTGroupFilter
            oGroupFilter3.Add(olayerTypes.XmlKey(roXmlLayerKeys.Target), "1022")
            oG3Layer.DataStoredXML = oGroupFilter3.XML
            rLayers.Add(oG3Layer)
        End If

        Return rLayers.ToArray()
    End Function

    Private Function creaGridRules() As HtmlTable
        Try
            Dim hTable As New HtmlTable
            Dim hTRow As New HtmlTableRow
            Dim hTCell As HtmlTableCell
            Dim altRow As String = "2"
            Dim noRegs As Boolean = False

            Dim strClickEdit As String = ""         'Href onclick Mode edicio
            Dim strClickRemove As String = ""       'Href onclick Mode eliminacio

            hTable.Border = 0
            hTable.CellPadding = 0
            hTable.CellSpacing = 0

            hTable.ID = "tblGridRules"
            hTable.Attributes("class") = "GridStyle GridEmpleados"

            'TODO: Creo la capcelera (de moment manual... aixo es pot fer GENERIC)
            hTRow = New HtmlTableRow

            hTCell = New HtmlTableCell
            hTCell.Attributes("class") = "GridStyle-cellheader"
            hTCell.Attributes("style") = "border-right: 0;"
            hTCell.Width = "200px"
            hTCell.InnerHtml = Me.Language.Translate("gridHeaderName", Me.DefaultScope)
            'hTCell.ColSpan = 2
            hTRow.Cells.Add(hTCell)

            hTCell = New HtmlTableCell
            hTCell.Attributes("class") = "GridStyle-cellheader"
            hTCell.InnerHtml = Me.Language.Translate("gridHeaderDescription", Me.DefaultScope)
            'hTCell.ColSpan = 2
            hTRow.Cells.Add(hTCell)

            hTable.Rows.Add(hTRow)

            Return hTable
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Private Function creaGridDailyRules() As HtmlTable
        Try
            Dim hTable As New HtmlTable
            Dim hTRow As New HtmlTableRow
            Dim hTCell As HtmlTableCell
            Dim altRow As String = "2"
            Dim noRegs As Boolean = False

            Dim strClickEdit As String = ""         'Href onclick Mode edicio
            Dim strClickRemove As String = ""       'Href onclick Mode eliminacio

            hTable.Border = 0
            hTable.CellPadding = 0
            hTable.CellSpacing = 0

            hTable.ID = "tblGridDailyRules"
            hTable.Attributes("class") = "GridStyle GridEmpleados"

            'TODO: Creo la capcelera (de moment manual... aixo es pot fer GENERIC)
            hTRow = New HtmlTableRow

            hTCell = New HtmlTableCell
            hTCell.Attributes("class") = "GridStyle-cellheader"
            hTCell.Attributes("style") = "border-right: 0;"
            hTCell.Width = "200px"
            hTCell.InnerHtml = Me.Language.Translate("gridDailyHeaderName", Me.DefaultScope)
            'hTCell.ColSpan = 2
            hTRow.Cells.Add(hTCell)

            hTCell = New HtmlTableCell
            hTCell.Attributes("class") = "GridStyle-cellheader"
            hTCell.InnerHtml = Me.Language.Translate("gridHeaderDescription", Me.DefaultScope)
            'hTCell.ColSpan = 2
            hTRow.Cells.Add(hTCell)

            hTable.Rows.Add(hTRow)

            Return hTable
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Private Function creaGridAssignments() As HtmlTable
        Try
            Dim hTable As New HtmlTable
            Dim hTRow As New HtmlTableRow
            Dim hTCell As HtmlTableCell
            Dim altRow As String = "2"
            Dim noRegs As Boolean = False

            Dim strClickEdit As String = ""         'Href onclick Mode edicio
            Dim strClickRemove As String = ""       'Href onclick Mode eliminacio

            hTable.Border = 0
            hTable.CellPadding = 0
            hTable.CellSpacing = 0

            hTable.ID = "tblGridAssignments"
            hTable.Attributes("class") = "GridStyle GridEmpleados"

            'TODO: Creo la capcelera (de moment manual... aixo es pot fer GENERIC)
            hTRow = New HtmlTableRow

            hTCell = New HtmlTableCell
            hTCell.Attributes("class") = "GridStyle-cellheader"
            hTCell.Attributes("style") = "border-right: 0;"
            hTCell.Width = "80px"
            hTCell.InnerHtml = Me.Language.Translate("gridHeaderAssignment", Me.DefaultScope)
            'hTCell.ColSpan = 2
            hTRow.Cells.Add(hTCell)

            hTCell = New HtmlTableCell
            hTCell.Attributes("class") = "GridStyle-cellheader"
            hTCell.InnerHtml = Me.Language.Translate("gridHeaderCoverage", Me.DefaultScope)
            hTCell.Width = "50px"
            'hTCell.ColSpan = 2
            hTRow.Cells.Add(hTCell)

            hTable.Rows.Add(hTRow)

            Return hTable
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Private Sub ProcessShiftSelectedTabVisible(ByVal oParameters As ShiftsCallbackRequest)
        'Mostra el TAB seleccionat
        Me.isHRScheduling.Value = IIf(HelperSession.GetFeatureIsInstalledFromApplication("Feature\HRScheduling") = True, "1", "0")
        Me.div00.Style("display") = "none"
        Me.div02.Style("display") = "none"

        Select Case oParameters.aTab
            Case 0
                Me.div00.Style("display") = ""
            Case 1
                Me.div01.Style("display") = ""
            Case 2
                Me.div02.Style("display") = ""

        End Select
    End Sub

#End Region

#Region "ShiftGroups"

    Private Sub ProcessShiftGroupRequest(ByVal oParameters As ShiftsCallbackRequest)
        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)

        If Me.oPermission > Permission.None Then
            'Mostra el TAB seleccionat

            Select Case oParameters.Action
                Case "GETSHIFTGROUP"
                    LoadShiftGroupData(oParameters, True)
                Case "SAVESHIFTGROUP"
                    SaveShiftGroupData(oParameters)
            End Select

            Select Case oParameters.aTab
                Case 0
                    Me.div20.Style("display") = ""
            End Select
        End If
    End Sub

    Private Sub SaveShiftGroupData(ByVal oParameters As ShiftsCallbackRequest)
        Dim strError As String = ""
        Dim strMessage As String = ""

        Try
            'Check Permissions
            If Me.oPermission < Permission.Write Then
                strMessage = Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope)
                strError = "KO"
            End If

            Dim oShiftGroup As roShiftGroup = API.ShiftServiceMethods.GetShiftGroup(Me, oParameters.ID, True)
            If oShiftGroup Is Nothing Then Return

            Dim strTempBusinessGroup As String = Me.txtBusinessGroup.Text

            Dim bContinuar As Boolean = True

            'revisar si hay cambio en el BusinessGroup si hay diferencias, comprobar si hay algun grupo de usuarios que tenga ese businessgroup 'si lo hay se cancela
            If oShiftGroup.BusinessGroup <> strTempBusinessGroup AndAlso Not API.ShiftServiceMethods.BusinessGroupListInUse(Me, oShiftGroup.BusinessGroup, oShiftGroup.ID) AndAlso API.UserAdminServiceMethods.BusinessGroupListInUse(Me, oShiftGroup.BusinessGroup) Then
                bContinuar = False
            End If

            If bContinuar AndAlso strError = String.Empty Then
                oShiftGroup.BusinessGroup = strTempBusinessGroup
                oShiftGroup.Name = Me.txtShiftGroupName.Text
                If Not API.ShiftServiceMethods.SaveShiftGroup(Me, oShiftGroup, True) Then
                    strMessage = API.ShiftServiceMethods.LastErrorText
                    strError = "KO"
                End If
            Else
                strMessage = Me.Language.Translate("BusinessGroupInUse.Description", DefaultScope)
                strError = "KO"
            End If
        Catch ex As Exception
            strMessage = ex.Message
            strError = "KO"
        Finally

            If strError = String.Empty Then
                LoadShiftGroupData(oParameters)
                ASPxCallbackPanelContenido.JSProperties("cpIsNewRO") = True
            Else
                ASPxCallbackPanelContenido.JSProperties.Add("cpActionRO", "SAVESHIFTGROUP")
                ASPxCallbackPanelContenido.JSProperties.Add("cpResultRO", "KO")
                ASPxCallbackPanelContenido.JSProperties.Add("cpMessageRO", strMessage)
            End If

        End Try

    End Sub

    Private Sub LoadShiftGroupData(ByVal oParameters As ShiftsCallbackRequest, Optional ByVal bAudit As Boolean = False)
        Try

            Dim oShiftGroup As roShiftGroup = API.ShiftServiceMethods.GetShiftGroup(Me, oParameters.ID, bAudit)
            If oShiftGroup Is Nothing Then Return

            'Check Permissions
            Dim disControls As Boolean = False
            If Me.oPermission < Permission.Write Then
                disControls = True
            End If

            Me.txtBusinessGroup.Text = oShiftGroup.BusinessGroup
            Me.txtShiftGroupName.Text = oShiftGroup.Name
            Me.hdnSelectedGroup.Value = oShiftGroup.ID

            Dim bolMoveIcon As Boolean = False

            Dim nRowControl As DataRow
            Dim strCols() As String = {Me.Language.Translate("ColumnShiftName", Me.DefaultScope)}
            Dim sizeCols() As String = {"600px"}
            Dim cssCols() As String = {"GridStyle-cellheader"}

            'Carrega Horaris del grup
            Dim dtblCurrent As DataTable = API.ShiftServiceMethods.GetShiftsFromGroup(Me, oShiftGroup.ID)

            For y As Integer = dtblCurrent.Columns.Count - 1 To 2 Step -1
                dtblCurrent.Columns.Remove(dtblCurrent.Columns(y).ColumnName)
            Next

            Dim tblControlCurrent As DataTable

            If dtblCurrent.Rows.Count > 0 Then
                tblControlCurrent = New DataTable
                tblControlCurrent.Columns.Add("NomCamp") 'Nom del Camp
                tblControlCurrent.Columns.Add("NomParam") 'Parametre que es pasara
                tblControlCurrent.Columns.Add("Visible") 'Si es te que carregar en les columnes o no...

                nRowControl = tblControlCurrent.NewRow
                nRowControl("NomCamp") = "ID"
                nRowControl("NomParam") = ""
                nRowControl("Visible") = False
                tblControlCurrent.Rows.Add(nRowControl)

                nRowControl = tblControlCurrent.NewRow
                nRowControl("NomCamp") = "Name"
                nRowControl("NomParam") = ""
                nRowControl("Visible") = True
                tblControlCurrent.Rows.Add(nRowControl)

                Dim htmlHGridCurrent As HtmlTable = creaHeaderLists(strCols, sizeCols, cssCols)
                Me.divHeaderShift.Controls.Add(htmlHGridCurrent)

                Dim htmlTGridCurrent As HtmlTable = creaGridLists(dtblCurrent, strCols, sizeCols, tblControlCurrent, True, 1, True)
                Me.divGridShift.Controls.Add(htmlTGridCurrent)
            Else
                Me.divHeaderShift.InnerHtml = Me.Language.Translate("NoShifts", Me.DefaultScope) '"No hay empleados actualmente"
            End If

            ASPxCallbackPanelContenido.JSProperties.Add("cpActionRO", "GETSHIFTGROUP")
            ASPxCallbackPanelContenido.JSProperties.Add("cpResultRO", "OK")
            ASPxCallbackPanelContenido.JSProperties.Add("cpIsNewRO", False)
        Catch ex As Exception
            Dim rError As New roJSON.JSONError(True, ex.Message.ToString & " " & ex.StackTrace)
            Response.Write(rError.toJSON)
        End Try
    End Sub

    Private Function creaHeaderLists(ByVal nomCols() As String, ByVal sizeCols() As String, ByVal cssCols() As String) As HtmlTable
        Try
            Dim hTable As New HtmlTable
            Dim hTRow As New HtmlTableRow
            Dim hTCell As HtmlTableCell

            hTable.Border = 0
            hTable.CellPadding = 0
            hTable.CellSpacing = 0

            hTable.Attributes("class") = "GridStyle GridGrupos"
            hTable.Style("border-bottom") = "0"
            hTable.Style("margin-bottom") = "0"

            'TODO: Creo la capcelera (de moment manual... aixo es pot fer GENERIC)
            hTRow = New HtmlTableRow

            For n As Integer = 0 To nomCols.Length - 1
                hTCell = New HtmlTableCell
                hTCell.Attributes("class") = cssCols(n) '"GridStyle-cellheader"
                hTCell.InnerHtml = nomCols(n)
                If nomCols(n) = "" Then hTCell.InnerText = " "
                hTCell.Width = sizeCols(n)
                hTRow.Cells.Add(hTCell)
            Next

            'Celda d'espai per edicio
            hTCell = New HtmlTableCell
            hTCell.InnerText = " "
            hTCell.Width = "40px"
            hTRow.Cells.Add(hTCell)

            hTable.Rows.Add(hTRow)

            Return hTable
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Crea un Grid d'estructura de Ausencias
    ''' </summary>
    ''' <param name="dTable"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function creaGridLists(ByVal dTable As DataTable, Optional ByVal nomCols() As String = Nothing, Optional ByVal sizeCols() As String = Nothing, Optional ByVal dTblControls As DataTable = Nothing, Optional ByVal editIcons As Boolean = False, Optional ByVal colSpanCol As Integer = 1, Optional ByVal moveIcon As Boolean = False) As HtmlTable
        Try
            Dim hTable As New HtmlTable
            Dim hTRow As New HtmlTableRow
            Dim hTCell As HtmlTableCell
            Dim altRow As String = "2"
            Dim noRegs As Boolean = False

            Dim strClickShow As String = ""         'Href onclick Mode edicio
            Dim strClickMove As String = ""       'Href onclick Mode eliminacio

            hTable.Border = 0
            hTable.CellPadding = 0
            hTable.CellSpacing = 0

            hTable.Attributes("class") = "GridStyle GridGrupos"
            'hTable.Style("border-bottom") = "0"
            hTable.Style("margin-bottom") = "0"

            'Bucle als registres

            For n As Integer = 0 To dTable.Rows.Count - 1
                hTRow = New HtmlTableRow
                altRow = IIf(altRow = "1", "2", "1")

                Dim colsizeint As Integer = -1
                For y As Integer = 0 To dTable.Columns.Count - 1
                    'Comproba si es una columna que no es te de visualitzar
                    If dTblControls IsNot Nothing Then
                        Dim dRowSel() As DataRow = dTblControls.Select("NomCamp = '" & dTable.Columns(y).ColumnName & "'")
                        If dRowSel.Length > 0 Then
                            If dRowSel(0).Item("Visible") = False Then Continue For
                        End If
                    End If

                    colsizeint += 1
                    hTCell = New HtmlTableCell
                    hTCell.Width = sizeCols(colsizeint)

                    'Cambia el alternateRow
                    hTCell.Attributes("class") = "GridStyle-cell" & altRow
                    hTCell.Style("display") = "table-cell"

                    'hTCell.InnerText = dTable.Columns(y).ColumnName & ":" & dTable.Rows(n)(y).ToString
                    hTCell.InnerText = dTable.Rows(n)(y).ToString
                    If IsDate(dTable.Rows(n)(y)) Then
                        hTCell.InnerText = roTypes.Any2String(dTable.Rows(n)(y))
                        If hTCell.InnerText = "01/01/2079" Then hTCell.InnerText = ""
                        'hTCell.Style("text-align") = "center"
                    End If
                    If hTCell.InnerText = "" Then hTCell.InnerText = " "
                    'Si es la ultima columna (per tancar el row)
                    If y = dTable.Columns.Count - 1 Then hTCell.Attributes("class") = hTCell.Attributes("class") & "  GridStyle-endcell" & altRow

                    'Carrega la celda al row
                    hTRow.Cells.Add(hTCell)

                    'Dibuixem les columnes de les icones d'edicio
                    If editIcons AndAlso y = dTable.Columns.Count - 1 Then
                        hTCell = New HtmlTableCell
                        hTCell.Width = "40px"
                        hTCell.Attributes("class") = "GridStyle-cellheader"
                        hTCell.Attributes("style") = "border: 0; background-color: #E8EEF7;border-right: solid 1px #D7D7D7;"
                        Dim hAnchorEdit As New HtmlAnchor
                        Dim hAnchorRemove As New HtmlAnchor

                        strClickShow = "cargaHorario2("
                        strClickMove = ""
                        If dTblControls IsNot Nothing Then
                            For Each dRow As DataRow In dTblControls.Rows
                                strClickShow &= "'" & dTable.Rows(n)(dRow(0)).ToString & "',"
                            Next
                        End If
                        strClickShow = strClickShow.Substring(0, strClickShow.Length - 1) & ");"

                        hAnchorEdit.Attributes("onclick") = strClickShow
                        hAnchorEdit.Title = Me.Language.Translate("ViewShift", Me.DefaultScope) '"Ver empleado"
                        hAnchorEdit.HRef = "javascript: void(0);"
                        hAnchorEdit.InnerHtml = "<img src=""" & Me.Page.ResolveUrl("~/Base/Images/Grid/viewshift.png") & """>"

                        hTCell.Controls.Add(hAnchorEdit)
                        hTRow.Cells.Add(hTCell)
                    End If
                Next
                hTable.Rows.Add(hTRow)
            Next
            Return hTable
        Catch ex As Exception
            Dim htmlTableErr As New HtmlTable
            Dim htmlTableErrCell As New HtmlTableCell
            Dim htmlTableErrRow As New HtmlTableRow
            htmlTableErrCell.InnerHtml = ex.Message.ToString & " " & ex.StackTrace.ToString
            htmlTableErrRow.Cells.Add(htmlTableErrCell)
            htmlTableErr.Rows.Add(htmlTableErrRow)
            Return htmlTableErr
        End Try
    End Function

    Private Function LoadShiftAdvancedParatemers(sParameters As String) As roShiftAdvParameters()
        Dim oRet As New List(Of roShiftAdvParameters)
        Dim aTemp As String() = {}
        Dim aTemp2 As String() = {}
        Try
            If sParameters IsNot Nothing AndAlso sParameters.Contains("[") AndAlso sParameters.Contains("]") Then
                aTemp = sParameters.Replace(vbLf, "").Split("]")

                For Each sParameter As String In aTemp
                    If sParameter.Trim.Contains("[") Then
                        aTemp2 = sParameter.Split("=")
                        If aTemp2.Count = 2 Then
                            oRet.Add(New roShiftAdvParameters(aTemp2(0).Trim, aTemp2(1).Replace("[", "").Trim))
                        End If
                    End If
                Next
            End If
        Catch ex As Exception
            oRet = New List(Of roShiftAdvParameters)
        End Try
        Return oRet.ToArray
    End Function

    Private Sub Shiftsv2_PreInit(sender As Object, e As EventArgs) Handles Me.PreInit
        Me.OverrrideDefaultScope = "Shifts"
    End Sub

    Private Sub Shiftsv2_PreLoad(sender As Object, e As EventArgs) Handles Me.PreLoad
        Me.OverrrideDefaultScope = "Shifts"
    End Sub

    Private Sub Shiftsv2_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        Me.OverrrideDefaultScope = "Shifts"
    End Sub

#End Region

End Class