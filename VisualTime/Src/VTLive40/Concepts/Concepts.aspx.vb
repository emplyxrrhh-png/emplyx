Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBusiness.Cause
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Concept
Imports Robotics.Base.VTBusiness.Shift
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API
Imports eRoundingType = Robotics.Base.DTOs.eRoundingType

Partial Class Concepts
    Inherits PageBase

    <Runtime.Serialization.DataContract()>
    Private Class ConceptsCallbackRequest

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

        <Runtime.Serialization.DataMember(Name:="resultClientAction")>
        Public resultClientAction As String

        <Runtime.Serialization.DataMember(Name:="AutomaticAccrualConf")>
        Public AutomaticAccrualConf As String

    End Class

    <Runtime.Serialization.DataContract()>
    Private Class AutomaticAccrualsClientControl

        <Runtime.Serialization.DataMember(Name:="causes")>
        Public causes As AutomaticAccrualItem()

        <Runtime.Serialization.DataMember(Name:="shifts")>
        Public shifts As AutomaticAccrualItem()

        <Runtime.Serialization.DataMember(Name:="clientEnabled")>
        Public clientEnabled As Boolean

        <Runtime.Serialization.DataMember(Name:="selectedHourCauses")>
        Public selectedHourCauses As Integer()

        <Runtime.Serialization.DataMember(Name:="selectedDayCauses")>
        Public selectedDayCauses As Integer()

        <Runtime.Serialization.DataMember(Name:="selectedDayShifts")>
        Public selectedDayShifts As Integer()

        <Runtime.Serialization.DataMember(Name:="accrualautomaticType")>
        Public accrualautomaticType As Integer

    End Class

    <Runtime.Serialization.DataContract()>
    Private Class AutomaticAccrualItem

        <Runtime.Serialization.DataMember(Name:="ID")>
        Public ID As Integer

        <Runtime.Serialization.DataMember(Name:="Name")>
        Public Name As String

    End Class

    Private Const FeatureAlias As String = "Concepts.Definition"
    Private oPermission As Permission

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("Ajax", "~/Base/Scripts/Ajax.js")
        Me.InsertExtraJavascript("jsDate", "~/Base/Scripts/jsDate.js")

        Me.InsertExtraJavascript("GenericData", "~/Base/Scripts/GenericData.js")

        Me.InsertExtraJavascript("CGroups", "~/Concepts/Scripts/CGroups.js")
        Me.InsertExtraJavascript("Concepts", "~/Concepts/Scripts/ConceptsV2.js")
        Me.InsertExtraJavascript("ConceptGroups", "~/Concepts/Scripts/ConceptsGroupsV2.js")

        Me.InsertExtraJavascript("roOptionPanelClient", "~/Base/Scripts/roOptionPanelClient.js")
        Me.InsertExtraJavascript("roOptPanelClientGroup", "~/Base/Scripts/roOptPanelClientGroup.js")

    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Comprueba la licencia
        If HelperSession.GetFeatureIsInstalledFromApplication("Forms\Concepts") = False Then
            WLHelperWeb.RedirectAccessDenied(False)
            Exit Sub
        End If

        roTreesConceptGroups.TreeCaption = Me.Language.Translate("TreeCaptionConceptGroups", Me.DefaultScope)
        roTreesConcepts.TreeCaption = Me.Language.Translate("TreeCaptionConcepts", Me.DefaultScope)

        roTreesConceptGroups.TreeDoubleCaption = Me.Language.Translate("TreeDoubleCaptionConceptGroups", Me.DefaultScope)
        roTreesConcepts.TreeDoubleCaption = Me.Language.Translate("TreeDoubleCaptionConcepts", Me.DefaultScope)

        Me.lblSaveChanges.InnerText = Me.Language.Translate("msgSaveChangesbar", DefaultScope)
        Me.lblSaveChangesBottom.InnerText = Me.lblSaveChanges.InnerText
        Me.lblUndoChanges.InnerText = Me.Language.Translate("msgUndoChangesbar", DefaultScope)
        Me.lblUndoChangesBottom.InnerText = Me.lblUndoChanges.InnerText

        Me.msgHasChanges.Value = Me.Language.Translate("msgHasChanges", DefaultScope)
        Me.msgHasErrors.Value = Me.Language.Translate("msgHasErrors", DefaultScope)
        Me.header1.Value = Me.Language.Translate("gridHeaderConcept", DefaultScope)
        Me.msgErrorNoConditions.Value = Me.Language.Translate("msgErrorNoConcepts", DefaultScope)

        Server.ScriptTimeout = 1000

        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)
        If Me.oPermission = Permission.None Then
            WLHelperWeb.RedirectAccessDenied(False)
            Exit Sub
        End If

        If Me.oPermission >= Permission.Write Then
            Me.hdnModeEdit.Value = "true"
        Else
            Me.DisableControls(Me.Controls)
            hdnModeEdit.Value = "false"
            Me.addCompositionTable.Style("display") = "none"
            Me.addConceptTable.Style("display") = "none"
        End If

        dateFormatValue.Value = HelperWeb.GetShortDateFormat

        Dim dTbl As DataTable = API.ConceptsServiceMethods.GetConceptsDataTable(Me.Page)
        If dTbl.Rows.Count = 0 Then
            Me.noRegs.Value = "1"
        Else
            Me.noRegs.Value = ""
        End If

    End Sub

    Protected Sub ASPxCallbackPanelContenido_Callback(ByVal sender As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase) Handles ASPxCallbackPanelContenido.Callback

        Try
            Dim strParameter As String = roTypes.Any2String(e.Parameter)
            strParameter = Server.UrlDecode(strParameter)

            Dim oParameters As New ConceptsCallbackRequest()
            oParameters = roJSONHelper.Deserialize(strParameter, oParameters.GetType())

            Select Case oParameters.Type
                Case "C"
                    ProcessConceptsRequest(oParameters)
                    ConceptsContent.Style("Display") = ""
                    ConceptGroupContent.Style("Display") = "none"
                Case "G"
                    ProcessConceptGroupRequest(oParameters)
                    ConceptsContent.Style("Display") = "none"
                    ConceptGroupContent.Style("Display") = ""
            End Select
        Catch ex As Exception
            ConceptsContent.Style("Display") = ""
            ConceptGroupContent.Style("Display") = "none"
        End Try

    End Sub

#Region "Concepts"

    Private Sub ProcessConceptsRequest(ByVal oParameters As ConceptsCallbackRequest)

        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)

        If Me.oPermission > Permission.None Then

            Select Case oParameters.Action
                Case "GETCONCEPT"
                    LoadConcept(oParameters)
                Case "SAVECONCEPT"
                    SaveConcept(oParameters)
            End Select

            ProcessGroupSelectedTabVisible(oParameters)
        End If

    End Sub

    Private Sub LoadConcept(ByVal oParameters As ConceptsCallbackRequest, Optional ByVal eConcept As roConcept = Nothing)
        Dim oCurrentConcept As roConcept

        If eConcept Is Nothing Then
            If oParameters.ID = -1 Then
                oCurrentConcept = New roConcept
            Else
                oCurrentConcept = API.ConceptsServiceMethods.GetConceptByID(Me, oParameters.ID, True)
            End If
        Else
            oCurrentConcept = eConcept
        End If

        Me.gridCompositions.Controls.Add(creaGridComposition())

        Me.dtRecDate.Value = Nothing
        Me.hdnCompositionChanges.Value = "0"

        LoadCombos(oParameters.ID, oCurrentConcept.DefaultQuery)

        If Me.oPermission < Permission.Write Then
            addCompositionTable.Visible = False
        End If

        If oCurrentConcept Is Nothing Then Exit Sub

        If oCurrentConcept.DefaultQuery Is Nothing Then oCurrentConcept.DefaultQuery = "Y"

        'ConnectorService.GetFirstDate
        Dim firstDate As Date = API.ConnectorServiceMethods.GetFirstDate(Me)

        'Recuperem data INICI del recalcul
        Dim RecalcDate As Date
        If oCurrentConcept.BeginDate > firstDate Then
            RecalcDate = oCurrentConcept.BeginDate.Date
        Else
            RecalcDate = firstDate
        End If

        'Recuperem data mes ANTIGA calculada
        Dim oldDate As Date
        Dim OldestDate As Nullable(Of Date)
        Dim RecalcDifferenceDays As Integer
        OldestDate = API.ConceptsServiceMethods.GetConceptOldestDate(Me, oCurrentConcept.ID)
        If OldestDate.HasValue Then
            oldDate = OldestDate
            If oldDate < RecalcDate Then
                OldestDate = RecalcDate
            Else
                OldestDate = oldDate
            End If

            'Recuperem DIFERENCIA en dies
            RecalcDifferenceDays = DateDiff(DateInterval.Day, API.ConceptsServiceMethods.GetConceptOldestDate(Me, oCurrentConcept.ID).Value, Date.Now)
        Else

        End If

        If oCurrentConcept.ApplyExpiredHours.HasValue AndAlso oCurrentConcept.ApplyExpiredHours.Value Then
            Me.optAccrualExpiration.Checked = True
            Me.cmbExpirationPeriodCause.SelectedItem = Me.cmbExpirationPeriodCause.Items.FindByValue(oCurrentConcept.ExpiredIDCause)

            Me.txtExpirationPeriodValue.Value = oCurrentConcept.ExpiredHoursCriteria.oValue
            If oCurrentConcept.ExpiredHoursCriteria.ExpiredHoursType = eExpiredHoursType.DaysType Then
                Me.cmbExpirationPeriodType.SelectedItem = Me.cmbExpirationPeriodType.Items.FindByValue("0")
            Else
                Me.cmbExpirationPeriodType.SelectedItem = Me.cmbExpirationPeriodType.Items.FindByValue("1")
            End If
        Else
            Me.optAccrualExpiration.Checked = False
            Me.cmbExpirationPeriodType.SelectedItem = Me.cmbExpirationPeriodType.Items(0)
            Me.cmbExpirationPeriodCause.SelectedItem = Me.cmbExpirationPeriodCause.Items(0)
            Me.txtExpirationPeriodValue.Value = "0"
        End If

        Dim firstDayOfMonth As Date = New Date(System.DateTime.Now.Year, DateTime.Now.Month, 1)

        Dim oDisable As Boolean = True
        If Me.oPermission > Permission.Read Then oDisable = False

        If recalcConfig.Contains("RecalcDate") Then
            recalcConfig("RecalcDate") = RecalcDate
        Else
            recalcConfig.Add("RecalcDate", RecalcDate)
        End If

        If recalcConfig.Contains("RecalcOldestDate") Then
            If OldestDate.HasValue Then
                recalcConfig("RecalcOldestDate") = OldestDate.Value
            Else
                recalcConfig("RecalcOldestDate") = ""
            End If
        Else
            If OldestDate.HasValue Then
                recalcConfig.Add("RecalcOldestDate", OldestDate.Value)
            Else
                recalcConfig.Add("RecalcOldestDate", "")
            End If
        End If

        If recalcConfig.Contains("RecalcDifference") Then
            recalcConfig("RecalcDifference") = RecalcDifferenceDays
        Else
            recalcConfig.Add("RecalcDifference", RecalcDifferenceDays)
        End If

        If recalcConfig.Contains("firstDayOfMonth") Then
            recalcConfig("firstDayOfMonth") = firstDayOfMonth
        Else
            recalcConfig.Add("firstDayOfMonth", firstDayOfMonth)
        End If

        Me.txtDescription.Text = oCurrentConcept.Description
        Me.txtShortName.Text = oCurrentConcept.ShortName
        Me.ColorConcept.Color = System.Drawing.ColorTranslator.FromWin32(oCurrentConcept.Color)
        Me.txtConceptName.Text = oCurrentConcept.Name

        If oCurrentConcept.Export <> "" Then
            Me.txtFactor.Text = oCurrentConcept.Export '.ToString.Replace(HelperWeb.GetDecimalDigitFormat, ".")
        Else
            Me.txtFactor.Text = "0"
        End If

        Dim strIDType As String = "0"
        If oParameters.ID <> -1 Then
            strIDType = IIf(oCurrentConcept.IDType = "H", "0", "1")
        End If

        If oCurrentConcept.CustomType.HasValue AndAlso oCurrentConcept.CustomType.Value Then
            strIDType = "2"
        End If

        Me.opConceptCustom.Checked = False

        If strIDType = "0" Then
            Me.oldConceptType.Value = "0"
            Me.divHoursAutomaticAccrual_lblHours.Style("display") = ""
            Me.divHoursAutomaticAccrual_lblDays.Style("display") = "none"
            opConceptTime.Checked = True
            opConceptTimes.Checked = False
            Me.opConceptCustom.Checked = False
        ElseIf strIDType = "1" Then
            Me.oldConceptType.Value = "1"
            Me.divHoursAutomaticAccrual_lblHours.Style("display") = "none"
            Me.divHoursAutomaticAccrual_lblDays.Style("display") = ""
            opConceptTime.Checked = False
            opConceptTimes.Checked = True
            Me.opConceptCustom.Checked = False
        Else
            Me.oldConceptType.Value = "2"
            Me.divHoursAutomaticAccrual_lblHours.Style("display") = "none"
            Me.divHoursAutomaticAccrual_lblDays.Style("display") = ""
            opConceptTime.Checked = False
            opConceptTimes.Checked = False
            Me.opConceptCustom.Checked = True
        End If

        If oCurrentConcept.RoundConceptBy.HasValue Then
            Me.txtRoundVal.Text = oCurrentConcept.RoundConceptBy.Value '.ToString.Replace(HelperWeb.GetDecimalDigitFormat, ".")
        Else
            Me.txtRoundVal.Text = "1"
        End If

        If oCurrentConcept.DefaultQuery = "L" Then
            opConceptTime.Enabled = False
            opConceptTime.Checked = False
            opConceptTimes.Enabled = False
            opConceptTimes.Checked = True
            opConceptCustom.Enabled = False
            opConceptCustom.Checked = False
        End If

        If oParameters.ID < 0 Then
            optRoundUP.Checked = False
            optRoundDown.Checked = False
            optRoundAprox.Checked = True
        Else
            optRoundUP.Checked = False
            optRoundDown.Checked = False
            optRoundAprox.Checked = False
            Select Case oCurrentConcept.RoundConveptType
                Case eRoundingType.Round_UP
                    optRoundUP.Checked = True
                Case eRoundingType.Round_Down
                    optRoundDown.Checked = True
                Case eRoundingType.Round_Near
                    optRoundAprox.Checked = True
            End Select
        End If

        If oCurrentConcept.RoundConceptBy.HasValue Then
            optChkRound.Checked = IIf(oCurrentConcept.RoundConceptBy > 1, True, False)
        Else
            optChkRound.Checked = False
        End If

        If oCurrentConcept.ViewInTerminals.HasValue Then
            chkShowInTerminals.Checked = IIf(oCurrentConcept.ViewInTerminals.Value.ToString.ToUpper = "TRUE", True, False)
        Else
            chkShowInTerminals.Checked = False
        End If

        If oCurrentConcept.FixedPay.HasValue Then
            If oCurrentConcept.FixedPay = True Then
                optFixedByAll.Checked = True
                optFixedByEmp.Checked = False
            Else
                optFixedByAll.Checked = False
                optFixedByEmp.Checked = True
            End If
        Else
            optFixedByAll.Checked = True
            optFixedByEmp.Checked = False
        End If

        If oCurrentConcept.PayValue.HasValue Then
            txtFixedPrice.Text = oCurrentConcept.PayValue.Value '.ToString.Replace(HelperWeb.GetDecimalDigitFormat, ".")
        Else
            txtFixedPrice.Text = "0"
        End If

        cmbFixedField.SelectedItem = cmbFixedField.Items.FindByValue(oCurrentConcept.UsedField)
        cmbShowValue.SelectedItem = cmbShowValue.Items.FindByValue(oCurrentConcept.DefaultQuery)

        If oCurrentConcept.RoundingBy.HasValue AndAlso oCurrentConcept.RoundingBy > 0 Then
            chkPayRound.Checked = True
            txtPayRound.Text = oCurrentConcept.RoundingBy.Value '.ToString.Replace(HelperWeb.GetDecimalDigitFormat, ".")
        Else
            chkPayRound.Checked = False
            txtPayRound.Text = "0"
        End If

        If oCurrentConcept.ViewInPays.HasValue Then
            If oCurrentConcept.ViewInPays.Value = True Then
                optPayPerHours.Checked = True
            Else
                optPayPerHours.Checked = False
            End If
        Else
            optPayPerHours.Checked = False
        End If

        If oCurrentConcept.IsAbsentiism.HasValue Then
            If oCurrentConcept.IsAbsentiism.Value = True Then
                optCheckAbsReport.Checked = True
            Else
                optCheckAbsReport.Checked = False
            End If
        Else
            optCheckAbsReport.Checked = False
        End If

        If oCurrentConcept.AbsentiismRewarded.HasValue Then
            If oCurrentConcept.AbsentiismRewarded.Value = True Then
                chkAbsRet.Checked = True
            Else
                chkAbsRet.Checked = False
            End If
        Else
            chkAbsRet.Checked = False
        End If
        If oCurrentConcept.IsAccrualWork.HasValue Then
            If oCurrentConcept.IsAccrualWork.Value = True Then
                chkAbsWorkHours.Checked = True
            Else
                chkAbsWorkHours.Checked = False
            End If
        Else
            chkAbsWorkHours.Checked = False
        End If

        If (oCurrentConcept.FinishDate.Day = 1 And oCurrentConcept.FinishDate.Month = 1 And oCurrentConcept.FinishDate.Year = 2079) Or
            (oCurrentConcept.FinishDate.Day = 1 And oCurrentConcept.FinishDate.Month = 1 And oCurrentConcept.FinishDate.Year = 1) Then
            dpPeriodEnd.Value = Nothing
        Else
            dpPeriodEnd.Date = oCurrentConcept.FinishDate.Date
        End If

        If oParameters.ID = -1 Then
            dpPeriodStart.Date = DateTime.Now.Date
        Else
            If oCurrentConcept.BeginDate.Day = 1 And oCurrentConcept.BeginDate.Month = 1 And oCurrentConcept.BeginDate.Year = 2079 Or
                oCurrentConcept.BeginDate.Day = 1 And oCurrentConcept.BeginDate.Month = 1 And oCurrentConcept.BeginDate.Year = 1 Then
                dpPeriodStart.Value = Nothing
            Else
                dpPeriodStart.Date = oCurrentConcept.BeginDate.Date
            End If
        End If

        optEmployeesPermissionAll.Checked = False
        optEmployeesPermissionNobody.Checked = False
        optEmployeesPermissionCriteria.Checked = False
        Select Case oCurrentConcept.EmployeesPremission
            Case 0
                optEmployeesPermissionAll.Checked = True
            Case 1
                optEmployeesPermissionNobody.Checked = True
            Case 2
                optEmployeesPermissionCriteria.Checked = True
        End Select

        'ppr
        Dim strcombosIndex As String = ""
        Dim bolCreateEmployeesPermissionEmpty As Boolean = True
        If oCurrentConcept.EmployeesPremission = 2 Then
            If Not oCurrentConcept.EmployeesConditions Is Nothing AndAlso oCurrentConcept.EmployeesConditions.Count > 0 Then
                bolCreateEmployeesPermissionEmpty = False
                strcombosIndex = Me.employeeCriteria.SetFilterValue(oCurrentConcept.EmployeesConditions(0))
            End If
        End If

        If bolCreateEmployeesPermissionEmpty = True Then
            Me.employeeCriteria.ClearFilterValue()
        End If

        Me.optNoAutomaticAccruals.Checked = False
        Me.optHoursAutomaticAccruals.Checked = False
        Me.optDaysAutomaticAccruals.Checked = False

        Dim selectedDayCauses As New Generic.List(Of Integer)
        Dim selectedDayShifts As New Generic.List(Of Integer)
        Dim selectedHourCauses As New Generic.List(Of Integer)
        Dim accrualautomaticType As Integer = 0

        If cmbDaysAutomaticAccrual_Userfield.Items.Count > 0 Then
            cmbHoursAutomaticAccrual_Userfield.SelectedItem = cmbHoursAutomaticAccrual_Userfield.Items(0)
            cmbDaysAutomaticAccrual_Userfield.SelectedItem = cmbDaysAutomaticAccrual_Userfield.Items(0)
            cmbDaysAutomaticAccrual.SelectedItem = cmbDaysAutomaticAccrual.Items(0)
            cmbHoursAutomaticAccrual.SelectedItem = cmbHoursAutomaticAccrual.Items(0)
        End If
        ckDaysAutomaticAccrualAllDays.Checked = True
        ckDaysAutomaticAccrualOnlyDays.Checked = False
        Me.txtHoursAutomaticAccrual_Fixed.Value = 0
        Me.txtDaysAutomaticAccrual_Fixed.Value = 0

        If Me.opConceptCustom.Checked = True Then
            oCurrentConcept.AutomaticAccrualType = eAutomaticAccrualType.DeactivatedType
        End If

        Select Case oCurrentConcept.AutomaticAccrualType
            Case eAutomaticAccrualType.DeactivatedType
                accrualautomaticType = CInt(eAutomaticAccrualType.DeactivatedType)
                Me.optNoAutomaticAccruals.Checked = True

            Case eAutomaticAccrualType.HoursType
                accrualautomaticType = CInt(eAutomaticAccrualType.HoursType)
                Me.optHoursAutomaticAccruals.Checked = True
                If oCurrentConcept.AutomaticAccrualCriteria.TotalCauses > 0 Then selectedHourCauses.AddRange(oCurrentConcept.AutomaticAccrualCriteria.Causes)

                If oCurrentConcept.AutomaticAccrualCriteria.FactorType = eFactorType.DirectValue Then
                    Me.cmbHoursAutomaticAccrual.SelectedItem = Me.cmbHoursAutomaticAccrual.Items.FindByValue(0)
                    Me.txtHoursAutomaticAccrual_Fixed.Value = oCurrentConcept.AutomaticAccrualCriteria.FactorValue
                Else
                    Me.cmbHoursAutomaticAccrual.SelectedItem = Me.cmbHoursAutomaticAccrual.Items.FindByValue(1)
                    Me.cmbHoursAutomaticAccrual_Userfield.SelectedItem = Me.cmbHoursAutomaticAccrual_Userfield.Items.FindByValue(oCurrentConcept.AutomaticAccrualCriteria.UserField.FieldName)
                End If

            Case eAutomaticAccrualType.DaysType
                accrualautomaticType = CInt(eAutomaticAccrualType.DaysType)
                Me.optDaysAutomaticAccruals.Checked = True
                If oCurrentConcept.AutomaticAccrualCriteria.TotalCauses > 0 Then selectedDayCauses.AddRange(oCurrentConcept.AutomaticAccrualCriteria.Causes)
                If oCurrentConcept.AutomaticAccrualCriteria.TotalShifts > 0 Then selectedDayShifts.AddRange(oCurrentConcept.AutomaticAccrualCriteria.Shifts)

                Me.ckDaysAutomaticAccrualOnlyDays.Checked = False
                Me.ckDaysAutomaticAccrualAllDays.Checked = False
                If oCurrentConcept.AutomaticAccrualCriteria.TypeAccrualDay = eAccrualDayType.AllDays Then
                    Me.ckDaysAutomaticAccrualAllDays.Checked = True
                Else
                    Me.ckDaysAutomaticAccrualOnlyDays.Checked = True
                End If

                If oCurrentConcept.AutomaticAccrualCriteria.FactorType = eFactorType.DirectValue Then
                    Me.cmbDaysAutomaticAccrual.SelectedItem = Me.cmbDaysAutomaticAccrual.Items.FindByValue(0)
                    Me.txtDaysAutomaticAccrual_Fixed.Value = oCurrentConcept.AutomaticAccrualCriteria.FactorValue
                Else
                    Me.cmbDaysAutomaticAccrual.SelectedItem = Me.cmbDaysAutomaticAccrual.Items.FindByValue(1)
                    Me.cmbDaysAutomaticAccrual_Userfield.SelectedItem = Me.cmbDaysAutomaticAccrual_Userfield.Items.FindByValue(oCurrentConcept.AutomaticAccrualCriteria.UserField.FieldName)
                End If
        End Select

        If oCurrentConcept.DefaultQuery = "L" Then
            Me.ckDaysAutomaticAccrualAllDays.Checked = False
            Me.ckDaysAutomaticAccrualOnlyDays.Checked = False
        End If

        If oCurrentConcept.AutomaticAccrualIDCause > 0 Then
            Me.cmbAutomaticAccrualCause.SelectedItem = Me.cmbAutomaticAccrualCause.Items.FindByValue(oCurrentConcept.AutomaticAccrualIDCause)
        Else
            Me.cmbAutomaticAccrualCause.SelectedItem = Nothing
        End If

        Me.optViewDays.Checked = False
        Me.optViewHours.Checked = False

        If oCurrentConcept.IDType Is Nothing Then
            Me.optViewHours.Checked = True
        ElseIf oCurrentConcept.IDType = "H" Then
            Me.optViewHours.Checked = True
        Else
            Me.optViewDays.Checked = True
        End If
        'Me.optViewDays.Enabled = False
        'Me.optViewHours.Enabled = False

        If Me.opConceptCustom.Checked = True Then
            Me.optViewDays.Enabled = True
            Me.optViewHours.Enabled = True
            Me.optViewDays.Checked = False
            Me.optViewHours.Checked = False

            If oCurrentConcept.IDType = "H" Then
                Me.optViewHours.Checked = True
            Else
                Me.optViewDays.Checked = True
            End If
        End If

        If oCurrentConcept.ApplyOnHolidaysRequest.HasValue Then
            Me.chkRequestHolidays.Checked = oCurrentConcept.ApplyOnHolidaysRequest
        Else
            Me.chkRequestHolidays.Checked = False
        End If

        If oCurrentConcept.DailyRecordMargin.HasValue Then
            Me.chkDailyRecord.Checked = True
            Me.txtDailyRecordMargin.Enabled = True
            Me.txtDailyRecordMargin.Text = oCurrentConcept.DailyRecordMargin

            If oCurrentConcept.AutoApproveRequestsDR Then
                Me.chkAutoApproveDR.Checked = True
            End If
        Else
            Me.chkDailyRecord.Checked = False
            Me.chkAutoApproveDR.Checked = False
            Me.txtDailyRecordMargin.Text = 0
            txtDailyRecordMargin.Enabled = False
        End If
        Me.chkDailyRecord.Enabled = True

        hdnConceptPeriodType.Value = oCurrentConcept.DefaultQuery

        Dim oClientControls As New AutomaticAccrualsClientControl

        oClientControls.causes = CreateAccrualsTagBoxJSON()
        oClientControls.shifts = CreateShiftsTagBoxJSON()
        oClientControls.clientEnabled = Me.oPermission > Permission.Read
        oClientControls.selectedDayCauses = selectedDayCauses.ToArray
        oClientControls.selectedDayShifts = selectedDayShifts.ToArray
        oClientControls.selectedHourCauses = selectedHourCauses.ToArray
        oClientControls.accrualautomaticType = accrualautomaticType

        ASPxCallbackPanelContenido.JSProperties.Add("cpActionRO", "GETCONCEPT")
        ASPxCallbackPanelContenido.JSProperties.Add("cpNameRO", oCurrentConcept.Name)
        ASPxCallbackPanelContenido.JSProperties.Add("cpCompositionsGridRO", CreateConceptCompositionJSON(oCurrentConcept))
        ASPxCallbackPanelContenido.JSProperties.Add("cpCombosIndexRO", strcombosIndex)
        ASPxCallbackPanelContenido.JSProperties.Add("cpClientControlsRO", roJSONHelper.Serialize(oClientControls))
        ASPxCallbackPanelContenido.JSProperties.Add("cpResultRO", "OK")
        ASPxCallbackPanelContenido.JSProperties.Add("cpIsNewRO", False)

    End Sub

    Private Function CreateAccrualsTagBoxJSON() As AutomaticAccrualItem()

        Dim oLst As AutomaticAccrualItem() = {}

        Try
            Dim lstItems As New Generic.List(Of AutomaticAccrualItem)

            Dim tbCauses As DataTable = CausesServiceMethods.GetCauses(Me.Page)
            For Each oRow As DataRow In tbCauses.Select("", "Name")
                Dim oTemp As New AutomaticAccrualItem
                oTemp.ID = oRow("Id")
                oTemp.Name = oRow("Name")
                lstItems.Add(oTemp)
            Next

            oLst = lstItems.ToArray()
        Catch ex As Exception
            Dim rError As New roJSON.JSONError(True, ex.Message.ToString & ex.StackTrace.ToString)
        End Try

        Return oLst
    End Function

    Private Function CreateShiftsTagBoxJSON() As AutomaticAccrualItem()
        Dim oLst As AutomaticAccrualItem() = {}
        Try
            Dim lstItems As New Generic.List(Of AutomaticAccrualItem)

            Dim tbCauses As DataTable = API.ShiftServiceMethods.GetShifts(Me.Page)
            For Each oRow As DataRow In tbCauses.Select("", "Name")
                Dim oTemp As New AutomaticAccrualItem
                oTemp.ID = oRow("Id")
                oTemp.Name = oRow("Name")
                lstItems.Add(oTemp)
            Next

            oLst = lstItems.ToArray()
        Catch ex As Exception
            Dim rError As New roJSON.JSONError(True, ex.Message.ToString & ex.StackTrace.ToString)
        End Try
        Return oLst
    End Function

    Private Sub SaveConcept(ByVal oParameters As ConceptsCallbackRequest)
        Dim strError As String = ""
        Dim strMessage As String = ""

        Dim oArrComp As New List(Of roConceptComposition)
        Dim oCurrentConcept As roConcept = Nothing

        LoadCombos(oParameters.ID, Nothing)
        Dim oConf As AutomaticAccrualsClientControl = roJSONHelper.Deserialize(oParameters.AutomaticAccrualConf, GetType(AutomaticAccrualsClientControl))

        Try
            'Check Permissions
            If Me.oPermission < Permission.Write Then
                strError = "KO"
                strMessage = Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope)
            End If

            Dim bolIsNew As Boolean = False
            If Request("ID") = "0" Or Request("ID") = "-1" Then bolIsNew = True
            If Me.oPermission = Permission.Write And bolIsNew Then
                strError = "KO"
                strMessage = Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope)
            End If

            If strError <> "KO" Then
                Dim oCompChanges As Boolean = False
                Dim oCompDate As Boolean = False
                Dim oDateRecalc As Date

                oCurrentConcept = API.ConceptsServiceMethods.GetConceptByID(Me, oParameters.ID, False)

                If oCurrentConcept Is Nothing Then Exit Sub

                LoadCombos(oParameters.ID, oCurrentConcept.DefaultQuery)
                oCurrentConcept.Name = txtConceptName.Text
                oCurrentConcept.Description = Me.txtDescription.Text
                oCurrentConcept.ShortName = Me.txtShortName.Text
                oCurrentConcept.Export = Me.txtFactor.Text
                oCurrentConcept.Color = Drawing.ColorTranslator.ToWin32(ColorConcept.Color)

                If Me.opConceptTime.Checked = True Then
                    oCurrentConcept.IDType = "H"
                Else
                    oCurrentConcept.IDType = "O"
                End If

                If Me.opConceptCustom.Checked Then
                    oCurrentConcept.CustomType = True
                    If Me.optViewDays.Checked Then
                        oCurrentConcept.IDType = "O"
                    Else
                        oCurrentConcept.IDType = "H"
                    End If
                Else
                    oCurrentConcept.CustomType = False
                End If

                Dim oLatamMex As New AdvancedParameter.roAdvancedParameter("Latam.Mex", New AdvancedParameter.roAdvancedParameterState())
                If oCurrentConcept.DefaultQuery = "L" AndAlso Not roTypes.Any2Boolean(oLatamMex.Value) Then
                    strError = "KO"
                    strMessage = Me.Language.Translate("ErrorConceptPeriodicityNotActive", DefaultScope)
                Else
                    oCurrentConcept.DefaultQuery = roTypes.Any2String(cmbShowValue.SelectedItem.Value)
                End If
                If Me.optAccrualExpiration.Checked AndAlso oCurrentConcept.DefaultQuery = "C" Then
                        oCurrentConcept.ApplyExpiredHours = True
                        oCurrentConcept.ExpiredIDCause = Me.cmbExpirationPeriodCause.SelectedItem.Value
                        oCurrentConcept.ExpiredHoursCriteria = New roExpiredHoursCriteria
                        oCurrentConcept.ExpiredHoursCriteria.ExpiredHoursType = If(Me.cmbExpirationPeriodType.SelectedItem.Value = "0", eExpiredHoursType.DaysType, eExpiredHoursType.MonthType)
                        oCurrentConcept.ExpiredHoursCriteria.oValue = roTypes.Any2Integer(Me.txtExpirationPeriodValue.Value)
                    Else
                        oCurrentConcept.ApplyExpiredHours = False
                        oCurrentConcept.ExpiredIDCause = 0
                        oCurrentConcept.ExpiredHoursCriteria = New roExpiredHoursCriteria
                        oCurrentConcept.ExpiredHoursCriteria.ExpiredHoursType = eExpiredHoursType.NotExpired
                        oCurrentConcept.ExpiredHoursCriteria.oValue = 0
                    End If

                    oCurrentConcept.RoundConceptBy = CDbl(txtRoundVal.Text.Replace(".", HelperWeb.GetDecimalDigitFormat))

                    If optRoundUP.Checked Then
                        oCurrentConcept.RoundConveptType = eRoundingType.Round_UP
                    End If

                    If optRoundDown.Checked Then
                        oCurrentConcept.RoundConveptType = eRoundingType.Round_Down
                    End If

                    If optRoundAprox.Checked Then
                        oCurrentConcept.RoundConveptType = eRoundingType.Round_Near
                    End If

                    oCurrentConcept.ViewInTerminals = chkShowInTerminals.Checked()

                    oCurrentConcept.UsedField = ""
                    oCurrentConcept.PayValue = Nothing
                    If optFixedByAll.Checked Then
                        oCurrentConcept.FixedPay = True
                        oCurrentConcept.PayValue = CDbl(txtFixedPrice.Text.Replace(".", HelperWeb.GetDecimalDigitFormat))
                    Else
                        oCurrentConcept.FixedPay = False
                        If Me.cmbFixedField.SelectedItem IsNot Nothing AndAlso optFixedByEmp.Checked = True Then
                            oCurrentConcept.UsedField = roTypes.Any2String(Me.cmbFixedField.SelectedItem.Value)
                        End If
                    End If

                    If chkPayRound.Checked Then
                        oCurrentConcept.RoundingBy = CDbl(txtPayRound.Text.Replace(".", HelperWeb.GetDecimalDigitFormat))
                    Else
                        oCurrentConcept.RoundingBy = Nothing
                    End If

                    oCurrentConcept.IsAbsentiism = optCheckAbsReport.Checked
                    oCurrentConcept.AbsentiismRewarded = chkAbsRet.Checked
                    oCurrentConcept.ViewInPays = optPayPerHours.Checked
                    oCurrentConcept.IsAccrualWork = chkAbsWorkHours.Checked

                    If dpPeriodStart.Value IsNot Nothing Then
                        oCurrentConcept.BeginDate = dpPeriodStart.Date.Date
                    Else
                        strError = "KO"
                        strMessage = Me.Language.Translate("ErrorNoBeginDate", DefaultScope)
                    End If

                    If dpPeriodEnd.Value IsNot Nothing Then
                        oCurrentConcept.FinishDate = dpPeriodEnd.Date.Date
                    Else
                        oCurrentConcept.FinishDate = New Date(2079, 1, 1)
                    End If

                    If optEmployeesPermissionAll.Checked Then
                        oCurrentConcept.EmployeesPremission = 0
                    End If

                    If optEmployeesPermissionNobody.Checked Then
                        oCurrentConcept.EmployeesPremission = 1
                    End If

                    If optEmployeesPermissionCriteria.Checked Then
                        oCurrentConcept.EmployeesPremission = 2
                    End If

                    If oCurrentConcept.EmployeesPremission = 2 Then
                        If employeeCriteria.IsValidFilter Then
                            Dim xList As New Generic.List(Of roUserFieldCondition)
                            xList.Add(employeeCriteria.OConditionValue)
                            oCurrentConcept.EmployeesConditions = xList
                        Else
                            strError = "KO"
                            strMessage = Me.Language.Translate("Error.NoCorrectVisibilityFilter", DefaultScope)
                        End If
                    End If

                    If Me.hdnCompositionChanges.Value = "1" Then
                        oCompChanges = True
                    End If

                    If Me.dtRecDate.Value IsNot Nothing AndAlso Me.dtRecDate.Date.Date > oCurrentConcept.BeginDate Then
                        oDateRecalc = Me.dtRecDate.Date
                        oCompDate = True
                    End If

                    Dim splitSep() As String = {"*&*"}
                    Dim compositions As String() = oParameters.resultClientAction.Split(splitSep, System.StringSplitOptions.None)

                    Dim splitComp() As String = {"*|*"}
                    For Each oComp As String In compositions
                        Dim oCompParameter As String() = oComp.Split(splitComp, System.StringSplitOptions.None)

                        If oCompParameter.Length = 18 Then
                            Dim oCComp As New roConceptComposition

                            oCComp.IDConcept = roTypes.Any2Integer(oCompParameter(0)) 'IDconcept
                            oCComp.IDCause = roTypes.Any2Integer(oCompParameter(1)) 'IDCause
                            oCComp.ID = roTypes.Any2Integer(oCompParameter(2)) 'IDConceptComposition
                            oCComp.IDShift = roTypes.Any2Integer(oCompParameter(3)) 'IDShift
                            oCComp.IDType = roTypes.Any2Integer(oCompParameter(4)) 'IDType
                            oCComp.TypeDayPlanned = roTypes.Any2Integer(oCompParameter(5)) 'TypeDayPlanned
                            oCComp.CompositionUserField = roTypes.Any2String(oCompParameter(6)) 'FactorUField
                            ' oCompParameter(7) 'Name
                            ' oCompParameter(8) 'Description
                            If oCompParameter(9).ToUpper = "FALSE" Then
                                oCComp.Conditions = Nothing
                            Else
                                Dim oCond As New roConceptCondition
                                If (oCComp.Conditions Is Nothing) Then oCComp.Conditions = New List(Of roConceptCondition)
                                oCComp.Conditions.Add(oCond)

                                If oCComp.Conditions(0) IsNot Nothing Then oCComp.Conditions(0).Compare = roTypes.Any2Integer(oCompParameter(11)) 'Compare
                                If oCComp.Conditions(0) IsNot Nothing Then oCComp.Conditions(0).Value_Type = roTypes.Any2Integer(oCompParameter(12)) 'Value_Type
                                If oCComp.Conditions(0) IsNot Nothing Then oCComp.Conditions(0).Value_IDCause = roTypes.Any2Integer(oCompParameter(13)) 'Value_IDCause
                                If oCComp.Conditions(0) IsNot Nothing Then oCComp.Conditions(0).Value_UserField = oCompParameter(14) 'Value_UserField
                                If oCComp.Conditions(0) IsNot Nothing Then oCComp.Conditions(0).Value_Direct = roTypes.Any2DateTime(oCompParameter(15)) 'Value_Direct

                                Dim newConditionCauses() As roConceptCondition.roConceptConditionCause = Nothing

                                Dim n As Integer = 0
                                Dim arrFields() As String = oCompParameter(16).Split("|")

                                For Each arrS As String In arrFields
                                    Dim arrParm() As String = arrS.Split(",")
                                    If arrParm(0).ToString.Trim <> "" Then
                                        Dim oCCause As New roConceptCondition.roConceptConditionCause
                                        oCCause.IDCause = arrParm(0)
                                        oCCause.Operation = arrParm(2)

                                        ReDim Preserve newConditionCauses(n)
                                        newConditionCauses(newConditionCauses.Length - 1) = oCCause
                                        n += 1
                                    End If
                                Next

                                If newConditionCauses IsNot Nothing Then
                                    If newConditionCauses.Length = 0 Then
                                        oCComp.Conditions(0).IDCauses = Nothing
                                    Else
                                        oCComp.Conditions(0).IDCauses = newConditionCauses.ToList
                                    End If
                                Else
                                    oCComp.Conditions(0).IDCauses = Nothing
                                End If

                            End If

                            If oCComp.CompositionUserField = String.Empty Then
                                oCComp.FactorValue = CDbl(oCompParameter(10).Replace(".", HelperWeb.GetDecimalDigitFormat)) 'FactorValue
                            Else
                                oCComp.FactorValue = 0
                            End If

                            oArrComp.Add(oCComp)
                        End If
                    Next

                    If oArrComp.Count > 0 Then
                        Dim oAComp(-1) As roConceptComposition
                        For Each oCm As roConceptComposition In oArrComp
                            ReDim Preserve oAComp(oAComp.Length)
                            oAComp(oAComp.Length - 1) = oCm
                        Next
                        oCurrentConcept.Composition = oAComp.ToList
                    Else
                        oCurrentConcept.Composition = New List(Of roConceptComposition)
                    End If

                    If oCurrentConcept.CustomType Then
                        Me.optNoAutomaticAccruals.Checked = False
                    End If

                    If Me.optNoAutomaticAccruals.Checked Then
                        oCurrentConcept.AutomaticAccrualType = eAutomaticAccrualType.DeactivatedType
                        oCurrentConcept.AutomaticAccrualCriteria = New roAutomaticAccrualCriteria
                        oCurrentConcept.AutomaticAccrualCriteria.AutomaticAccrualType = eAutomaticAccrualType.DeactivatedType

                    ElseIf Me.optHoursAutomaticAccruals.Checked Then
                        oCurrentConcept.AutomaticAccrualType = eAutomaticAccrualType.HoursType

                        oCurrentConcept.AutomaticAccrualCriteria = New roAutomaticAccrualCriteria
                        oCurrentConcept.AutomaticAccrualCriteria.AutomaticAccrualType = eAutomaticAccrualType.HoursType

                        If Me.cmbHoursAutomaticAccrual.SelectedItem Is Nothing OrElse (Me.cmbHoursAutomaticAccrual.SelectedItem IsNot Nothing AndAlso Me.cmbHoursAutomaticAccrual.SelectedItem.Value = 0) Then
                            oCurrentConcept.AutomaticAccrualCriteria.FactorType = eFactorType.DirectValue
                            oCurrentConcept.AutomaticAccrualCriteria.FactorValue = roTypes.Any2Double(Me.txtHoursAutomaticAccrual_Fixed.Value)
                        Else
                            oCurrentConcept.AutomaticAccrualCriteria.FactorType = eFactorType.UserField
                            oCurrentConcept.AutomaticAccrualCriteria.UserField = New roUserField()
                            oCurrentConcept.AutomaticAccrualCriteria.UserField.FieldName = Me.cmbHoursAutomaticAccrual_Userfield.SelectedItem.Value
                        End If

                        If oConf.selectedHourCauses.Length > 0 Then
                            oCurrentConcept.AutomaticAccrualCriteria.TotalCauses = oConf.selectedHourCauses.Length
                            oCurrentConcept.AutomaticAccrualCriteria.Causes = oConf.selectedHourCauses.ToList
                        End If

                    ElseIf Me.optDaysAutomaticAccruals.Checked Then
                        oCurrentConcept.AutomaticAccrualType = eAutomaticAccrualType.DaysType

                        oCurrentConcept.AutomaticAccrualCriteria = New roAutomaticAccrualCriteria
                        oCurrentConcept.AutomaticAccrualCriteria.AutomaticAccrualType = eAutomaticAccrualType.DaysType

                        If Me.cmbDaysAutomaticAccrual.SelectedItem Is Nothing OrElse (Me.cmbDaysAutomaticAccrual.SelectedItem IsNot Nothing AndAlso Me.cmbDaysAutomaticAccrual.SelectedItem.Value = 0) Then
                            oCurrentConcept.AutomaticAccrualCriteria.FactorType = eFactorType.DirectValue
                            oCurrentConcept.AutomaticAccrualCriteria.FactorValue = roTypes.Any2Double(Me.txtDaysAutomaticAccrual_Fixed.Value)
                        Else
                            oCurrentConcept.AutomaticAccrualCriteria.FactorType = eFactorType.UserField
                            oCurrentConcept.AutomaticAccrualCriteria.UserField = New roUserField()
                            oCurrentConcept.AutomaticAccrualCriteria.UserField.FieldName = Me.cmbDaysAutomaticAccrual_Userfield.SelectedItem.Value
                        End If

                        If oConf.selectedDayCauses.Length > 0 Then
                            oCurrentConcept.AutomaticAccrualCriteria.TotalCauses = oConf.selectedDayCauses.Length
                            oCurrentConcept.AutomaticAccrualCriteria.Causes = oConf.selectedDayCauses.ToList
                        End If

                        If oConf.selectedDayShifts.Length > 0 Then
                            oCurrentConcept.AutomaticAccrualCriteria.TotalShifts = oConf.selectedDayShifts.Length
                            oCurrentConcept.AutomaticAccrualCriteria.Shifts = oConf.selectedDayShifts.ToList
                        End If

                        oCurrentConcept.AutomaticAccrualCriteria.TypeAccrualDay = If(Me.ckDaysAutomaticAccrualAllDays.Checked, eAccrualDayType.AllDays, eAccrualDayType.SomeDays)

                    End If

                    If Me.cmbAutomaticAccrualCause.SelectedItem IsNot Nothing Then oCurrentConcept.AutomaticAccrualIDCause = Me.cmbAutomaticAccrualCause.SelectedItem.Value

                    oCurrentConcept.ApplyOnHolidaysRequest = Me.chkRequestHolidays.Checked
                    If Me.chkDailyRecord.Checked Then
                        oCurrentConcept.DailyRecordMargin = roTypes.Any2Integer(Me.txtDailyRecordMargin.Text)

                        If Me.chkAutoApproveDR.Checked Then
                            oCurrentConcept.AutoApproveRequestsDR = True
                        Else
                            oCurrentConcept.AutoApproveRequestsDR = False
                        End If
                    Else
                        oCurrentConcept.DailyRecordMargin = Nothing
                        oCurrentConcept.AutoApproveRequestsDR = False
                    End If
                    If strError = "" Then
                        If oCompChanges Then
                            If oCompDate = False Then
                                If API.ConceptsServiceMethods.SaveConcept(Me, oCurrentConcept, Nothing, True, True) = True Then
                                    oParameters.ID = oCurrentConcept.ID
                                Else
                                    strError = "KO"
                                    strMessage = API.ConceptsServiceMethods.LastErrorText
                                End If
                            Else
                                If API.ConceptsServiceMethods.SaveConcept(Me, oCurrentConcept, oDateRecalc, True, True) = True Then
                                    oParameters.ID = oCurrentConcept.ID
                                Else
                                    strError = "KO"
                                    strMessage = API.ConceptsServiceMethods.LastErrorText
                                End If
                            End If
                        Else
                            If API.ConceptsServiceMethods.SaveConcept(Me, oCurrentConcept, Nothing, False, True) = True Then
                                oParameters.ID = oCurrentConcept.ID
                            Else
                                strError = "KO"
                                strMessage = API.ConceptsServiceMethods.LastErrorText
                            End If
                        End If
                    End If
                End If
        Catch ex As Exception
            strError = "KO"
            strMessage = ex.Message
        End Try

        If strError = "KO" Then

            Dim hasDateValue As Boolean = False
            Dim tmpRecValue As Date
            If Me.dtRecDate.Value IsNot Nothing Then
                hasDateValue = True
                tmpRecValue = Me.dtRecDate.Date
            End If

            Dim tmpCompChanges As String = Me.hdnCompositionChanges.Value

            LoadConcept(oParameters, oCurrentConcept)

            If hasDateValue Then
                Me.dtRecDate.Value = tmpRecValue
            Else
                Me.dtRecDate.Value = Nothing
            End If

            Me.hdnCompositionChanges.Value = tmpCompChanges

            ASPxCallbackPanelContenido.JSProperties("cpActionRO") = "SAVECONCEPT"
            ASPxCallbackPanelContenido.JSProperties("cpResultRO") = "KO"
            ASPxCallbackPanelContenido.JSProperties("cpMessageRO") = strMessage
            ASPxCallbackPanelContenido.JSProperties("cpClientControlsRO") = roJSONHelper.Serialize(oConf)
            ASPxCallbackPanelContenido.JSProperties("cpCompositionsGridRO") = CreateConceptCompositionJSON(oCurrentConcept, oArrComp)
        Else
            HelperWeb.roSelector_SetSelection(oCurrentConcept.ID, "/source/" & oCurrentConcept.ID, "ctl00_contentMainBody_roTreesConcepts")
            LoadConcept(oParameters)
            ASPxCallbackPanelContenido.JSProperties("cpActionRO") = "SAVECONCEPT"
            ASPxCallbackPanelContenido.JSProperties("cpNewIdRO") = oParameters.ID
            ASPxCallbackPanelContenido.JSProperties("cpIsNewRO") = True
        End If
    End Sub

    Private Function CreateConceptCompositionJSON(ByVal oConcept As roConcept, Optional ByVal oArrComp As Generic.List(Of roConceptComposition) = Nothing) As String
        Try

            Dim oCurrentConcept As roConcept

            Dim n As Integer = 0

            Dim oCompositions As New Generic.List(Of Object)

            Dim oJSONField As Generic.List(Of JSONFieldItem)

            Dim oCondition As roConceptCondition
            Dim oCause As roCause

            Dim strJSONGroups As String = ""

            If oArrComp IsNot Nothing Then

                For Each oComp As roConceptComposition In oArrComp
                    oJSONField = New Generic.List(Of JSONFieldItem)

                    oJSONField.Add(New JSONFieldItem("IDConcept", oConcept.ID, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    oJSONField.Add(New JSONFieldItem("IDCause", oComp.IDCause, Nothing, roJSON.JSONType.None_JSON, Nothing, False))

                    oJSONField.Add(New JSONFieldItem("IDConceptComposition", oComp.ID, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    oJSONField.Add(New JSONFieldItem("IDShift", oComp.IDShift, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    oJSONField.Add(New JSONFieldItem("IDType", roTypes.Any2Integer(oComp.IDType), Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    oJSONField.Add(New JSONFieldItem("TypeDayPlanned", roTypes.Any2Integer(oComp.TypeDayPlanned), Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    oJSONField.Add(New JSONFieldItem("FactorUField", oComp.CompositionUserField, Nothing, roJSON.JSONType.None_JSON, Nothing, False))

                    If oComp.IDCause >= 0 AndAlso oComp.IDShift = 0 Then
                        oCause = CausesServiceMethods.GetCauseByID(Me.Page, oComp.IDCause, False)
                        If oCause IsNot Nothing Then
                            oJSONField.Add(New JSONFieldItem("Name", oCause.Name, Nothing, roJSON.JSONType.None_JSON, Nothing, False))

                            Dim isDayType As Integer = -1

                            If oConcept.IDType = "O" OrElse oConcept.CustomType = True Then isDayType = If((oCause.CustomType OrElse oCause.DayType) = True, 1, 0)

                            oJSONField.Add(New JSONFieldItem("IDCauseType", isDayType, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        Else 'Si no s'ha trobat la causa?
                            oJSONField.Add(New JSONFieldItem("Name", "<not found>", Nothing, roJSON.JSONType.None_JSON, Nothing, False))

                            oJSONField.Add(New JSONFieldItem("IDCauseType", "-1", Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        End If
                    ElseIf oComp.IDShift > 0 Then
                        Dim oShift As roShift = API.ShiftServiceMethods.GetShift(Me, oComp.IDShift, False)
                        If oShift IsNot Nothing Then
                            oJSONField.Add(New JSONFieldItem("Name", oShift.Name, Nothing, roJSON.JSONType.None_JSON, Nothing, False))

                            oJSONField.Add(New JSONFieldItem("IDCauseType", "-1", Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        Else 'Si no s'ha trobat la causa?
                            oJSONField.Add(New JSONFieldItem("Name", "<Not found>", Nothing, roJSON.JSONType.None_JSON, Nothing, False))

                            oJSONField.Add(New JSONFieldItem("IDCauseType", "-1", Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        End If
                    End If

                    If oComp.IDType = CompositionType.Absence Or oComp.IDType = CompositionType.Shift Then
                        oJSONField.Add(New JSONFieldItem("Description", Me.Language.Translate(System.Enum.GetName(GetType(TypeDayPlanned), oComp.IDType).ToString, DefaultScope), Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    Else
                        oJSONField.Add(New JSONFieldItem("Description", "", Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    End If

                    Dim checkCond As Boolean = False

                    If oComp.Conditions IsNot Nothing Then
                        If oComp.Conditions.Count > 0 Then
                            oCondition = oComp.Conditions(0)
                            checkCond = True
                        Else
                            oCondition = New roConceptCondition
                            checkCond = False
                        End If
                    Else
                        oCondition = New roConceptCondition
                        checkCond = False
                    End If

                    'camp optioncheck
                    oJSONField.Add(New JSONFieldItem("ConditionCheck", IIf(checkCond = True, "True", "False"), New String() {frmCompositions1.FindControl("optChkCondition").ClientID}, roJSON.JSONType.OptionCheck_JSON, Nothing, False))

                    'Camps de pantalla (Composition)
                    oJSONField.Add(New JSONFieldItem("FactorValue", oComp.FactorValue.ToString.Replace(".", HelperWeb.GetDecimalDigitFormat), New String() {frmCompositions1.FindControl("txtFactorComposition").ClientID}, roJSON.JSONType.Text_JSON, Nothing, False))

                    'Camps de pantalla (Conditions[0])
                    'Dim optChkCondition As ASP.base_webusercontrols_rooptionpanelclient_ascx = frmCompositions1.FindControl("optChkCondition")
                    Dim optChkCondition As WebUserControls_roOptionPanelClient = frmCompositions1.FindControl("optChkCondition")
                    oJSONField.Add(New JSONFieldItem("Compare", oCondition.Compare, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    oJSONField.Add(New JSONFieldItem("Value_Type", oCondition.Value_Type, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    oJSONField.Add(New JSONFieldItem("Value_IDCause", oCondition.Value_IDCause, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    oJSONField.Add(New JSONFieldItem("Value_UField", oCondition.Value_UserField, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    oJSONField.Add(New JSONFieldItem("Value_Direct", Format(oCondition.Value_Direct, HelperWeb.GetShortTimeFormat), New String() {optChkCondition.Content.FindControl("txtValueType").ClientID}, roJSON.JSONType.Text_JSON, Nothing, False))

                    'ConditionsCauses
                    If oCondition.IDCauses IsNot Nothing Then
                        If oCondition.IDCauses.Count > 0 Then
                            Dim strCondCauses As String = ""
                            For Each oCCause As roConceptCondition.roConceptConditionCause In oCondition.IDCauses
                                strCondCauses &= oCCause.IDCause & ","
                                oCause = CausesServiceMethods.GetCauseByID(Me, oCCause.IDCause, False)
                                If oCause Is Nothing Then
                                    strCondCauses &= "< notfound >,"
                                Else
                                    strCondCauses &= oCause.Name.Replace(", ", "") & ","
                                End If
                                strCondCauses &= oCCause.Operation & "|"
                            Next
                            If strCondCauses.Length > 0 Then strCondCauses = strCondCauses.Substring(0, Len(strCondCauses) - 1)
                            oJSONField.Add(New JSONFieldItem("CausesConditions", strCondCauses, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        Else
                            oJSONField.Add(New JSONFieldItem("CausesConditions", "", Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        End If
                    Else
                        oJSONField.Add(New JSONFieldItem("CausesConditions", "", Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    End If

                    oCompositions.Add(oJSONField)
                Next
            Else
                If oConcept.ID = -1 Then Return ""

                oCurrentConcept = API.ConceptsServiceMethods.GetConceptByID(Me, oConcept.ID, False)

                If oCurrentConcept Is Nothing Then Return ""

                'Si el concepte no te composicions, passem un objecte Json per omisio
                If oCurrentConcept.Composition Is Nothing Then

                    oJSONField = New Generic.List(Of JSONFieldItem)

                    oJSONField.Add(New JSONFieldItem("IDConcept", "", Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    oJSONField.Add(New JSONFieldItem("IDCause", "", Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    oJSONField.Add(New JSONFieldItem("IDCauseType", "-1", Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    oJSONField.Add(New JSONFieldItem("IDConceptComposition", "-1", Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    oJSONField.Add(New JSONFieldItem("IDShift", "", Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    oJSONField.Add(New JSONFieldItem("IDType", "0", Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    oJSONField.Add(New JSONFieldItem("TypeDayPlanned", "", Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    oJSONField.Add(New JSONFieldItem("FactorUField", "", Nothing, roJSON.JSONType.None_JSON, Nothing, False))

                    oJSONField.Add(New JSONFieldItem("Name", "", Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    oJSONField.Add(New JSONFieldItem("ConditionCheck", "False", Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    oJSONField.Add(New JSONFieldItem("FactorValue", "", Nothing, roJSON.JSONType.None_JSON, Nothing, False))

                    'Camps de pantalla (Conditions)
                    oJSONField.Add(New JSONFieldItem("Compare", "", Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    oJSONField.Add(New JSONFieldItem("Value_Type", "", Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    oJSONField.Add(New JSONFieldItem("Value_IDCause", "", Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    oJSONField.Add(New JSONFieldItem("Value_UField", "", Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    oJSONField.Add(New JSONFieldItem("Value_Direct", "", Nothing, roJSON.JSONType.None_JSON, Nothing, False))

                    ' Conditions buit
                    oJSONField.Add(New JSONFieldItem("CausesConditions", "", Nothing, roJSON.JSONType.None_JSON, Nothing, False))

                    oCompositions.Add(oJSONField)
                Else
                    'Bucle al compositions
                    For Each oComp As roConceptComposition In oCurrentConcept.Composition

                        oJSONField = New Generic.List(Of JSONFieldItem)

                        oJSONField.Add(New JSONFieldItem("IDConcept", oComp.IDConcept, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        oJSONField.Add(New JSONFieldItem("IDCause", oComp.IDCause, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        oJSONField.Add(New JSONFieldItem("IDConceptComposition", oComp.ID, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        oJSONField.Add(New JSONFieldItem("IDShift", oComp.IDShift, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        oJSONField.Add(New JSONFieldItem("IDType", roTypes.Any2Integer(oComp.IDType), Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        oJSONField.Add(New JSONFieldItem("TypeDayPlanned", roTypes.Any2Integer(oComp.TypeDayPlanned), Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        oJSONField.Add(New JSONFieldItem("FactorUField", oComp.CompositionUserField, Nothing, roJSON.JSONType.None_JSON, Nothing, False))

                        If oComp.IDCause >= 0 AndAlso oComp.IDShift = 0 Then
                            oCause = CausesServiceMethods.GetCauseByID(Me.Page, oComp.IDCause, False)
                            If oCause IsNot Nothing Then
                                oJSONField.Add(New JSONFieldItem("Name", oCause.Name, Nothing, roJSON.JSONType.None_JSON, Nothing, False))

                                Dim isDayType As Integer = -1
                                If oConcept.IDType = "O" OrElse oConcept.CustomType = True Then isDayType = If((oCause.CustomType OrElse oCause.DayType) = True, 0, 1)

                                oJSONField.Add(New JSONFieldItem("IDCauseType", isDayType, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                            Else 'Si no s'ha trobat la causa?
                                oJSONField.Add(New JSONFieldItem("Name", "<Not found>", Nothing, roJSON.JSONType.None_JSON, Nothing, False))

                                oJSONField.Add(New JSONFieldItem("IDCauseType", "-1", Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                            End If
                        ElseIf oComp.IDShift > 0 Then
                            Dim oShift As roShift = API.ShiftServiceMethods.GetShift(Me, oComp.IDShift, False)
                            If oShift IsNot Nothing Then
                                oJSONField.Add(New JSONFieldItem("Name", oShift.Name, Nothing, roJSON.JSONType.None_JSON, Nothing, False))

                                oJSONField.Add(New JSONFieldItem("IDCauseType", "-1", Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                            Else 'Si no s'ha trobat la causa?
                                oJSONField.Add(New JSONFieldItem("Name", "<Not found>", Nothing, roJSON.JSONType.None_JSON, Nothing, False))

                                oJSONField.Add(New JSONFieldItem("IDCauseType", "-1", Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                            End If
                        End If

                        If oComp.IDType = CompositionType.Absence Or oComp.IDType = CompositionType.Shift Then
                            oJSONField.Add(New JSONFieldItem("Description", Me.Language.Translate(System.Enum.GetName(GetType(TypeDayPlanned), oComp.TypeDayPlanned).ToString, DefaultScope), Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        Else
                            oJSONField.Add(New JSONFieldItem("Description", "", Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        End If

                        Dim checkCond As Boolean = False

                        If oComp.Conditions IsNot Nothing Then
                            If oComp.Conditions.Count > 0 Then
                                oCondition = oComp.Conditions(0)
                                checkCond = True
                            Else
                                oCondition = New roConceptCondition
                                checkCond = False
                            End If
                        Else
                            oCondition = New roConceptCondition
                            checkCond = False
                        End If

                        'camp optioncheck
                        oJSONField.Add(New JSONFieldItem("ConditionCheck", IIf(checkCond = True, "True", "False"), New String() {frmCompositions1.FindControl("optChkCondition").ClientID}, roJSON.JSONType.OptionCheck_JSON, Nothing, False))

                        'Camps de pantalla (Composition)
                        oJSONField.Add(New JSONFieldItem("FactorValue", oComp.FactorValue.ToString.Replace(".", HelperWeb.GetDecimalDigitFormat), New String() {frmCompositions1.FindControl("txtFactorComposition").ClientID}, roJSON.JSONType.Text_JSON, Nothing, False))

                        'Camps de pantalla (Conditions[0])
                        'Dim optChkCondition As ASP.base_webusercontrols_rooptionpanelclient_ascx = frmCompositions1.FindControl("optChkCondition")
                        Dim optChkCondition As WebUserControls_roOptionPanelClient = frmCompositions1.FindControl("optChkCondition")
                        oJSONField.Add(New JSONFieldItem("Compare", oCondition.Compare, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        oJSONField.Add(New JSONFieldItem("Value_Type", oCondition.Value_Type, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        oJSONField.Add(New JSONFieldItem("Value_IDCause", oCondition.Value_IDCause, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        oJSONField.Add(New JSONFieldItem("Value_UField", oCondition.Value_UserField, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        oJSONField.Add(New JSONFieldItem("Value_Direct", Format(oCondition.Value_Direct, HelperWeb.GetShortTimeFormat), New String() {optChkCondition.Content.FindControl("txtValueType").ClientID}, roJSON.JSONType.Text_JSON, Nothing, False))

                        'ConditionsCauses
                        If oCondition.IDCauses IsNot Nothing Then
                            If oCondition.IDCauses.Count > 0 Then
                                Dim strCondCauses As String = ""
                                For Each oCCause As roConceptCondition.roConceptConditionCause In oCondition.IDCauses
                                    strCondCauses &= oCCause.IDCause & ","
                                    oCause = CausesServiceMethods.GetCauseByID(Me, oCCause.IDCause, False)
                                    If oCause Is Nothing Then
                                        strCondCauses &= "< notfound >,"
                                    Else
                                        strCondCauses &= oCause.Name.Replace(", ", "") & ","
                                    End If
                                    strCondCauses &= oCCause.Operation & "|"
                                Next
                                If strCondCauses.Length > 0 Then strCondCauses = strCondCauses.Substring(0, Len(strCondCauses) - 1)
                                oJSONField.Add(New JSONFieldItem("CausesConditions", strCondCauses, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                            Else
                                oJSONField.Add(New JSONFieldItem("CausesConditions", "", Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                            End If
                        Else
                            oJSONField.Add(New JSONFieldItem("CausesConditions", "", Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        End If

                        oCompositions.Add(oJSONField)

                    Next
                End If
            End If

            strJSONGroups = ""
            For Each oObj As Object In oCompositions
                Dim strJSONText As String = ""
                strJSONText &= "{ ""composition"":  "
                strJSONText &= roJSONHelper.Serialize(oObj)
                strJSONText &= " } ,"
                strJSONGroups &= strJSONText
            Next

            If strJSONGroups.EndsWith(",") Then strJSONGroups = strJSONGroups.Substring(0, Len(strJSONGroups) - 1)

            Return strJSONGroups
        Catch ex As Exception
            Dim rError As New roJSON.JSONError(True, ex.Message.ToString & ex.StackTrace.ToString)
            Return rError.toJSON
        End Try
    End Function

    Private Function creaGridComposition() As HtmlTable
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

            hTable.ID = "tblGridCompositions"
            hTable.Attributes("class") = "GridStyle GridEmpleados"

            'TODO: Creo la capcelera (de moment manual... aixo es pot fer GENERIC)
            hTRow = New HtmlTableRow

            hTCell = New HtmlTableCell
            hTCell.Attributes("class") = "GridStyle-cellheader"
            hTCell.Attributes("style") = "border-right: 0;"
            hTCell.Width = "200px"
            hTCell.InnerHtml = Me.Language.Translate("gridHeaderCompName", Me.DefaultScope)
            'hTCell.ColSpan = 2
            hTRow.Cells.Add(hTCell)

            hTCell = New HtmlTableCell
            hTCell.Attributes("class") = "GridStyle-cellheader"
            hTCell.InnerHtml = Me.Language.Translate("gridHeaderCompDesc", Me.DefaultScope)
            'hTCell.ColSpan = 2
            hTRow.Cells.Add(hTCell)

            hTCell = New HtmlTableCell
            hTCell.Attributes("class") = "GridStyle-cellheader"
            hTCell.InnerHtml = Me.Language.Translate("gridHeaderCompFactor", Me.DefaultScope)
            hTCell.ColSpan = 2
            hTRow.Cells.Add(hTCell)

            hTable.Rows.Add(hTRow)

            Return hTable
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Private Sub LoadCombos(ByVal idConcept As Integer, ByVal conceptPeriodType As String)

        Me.frmCompositions1.loadCombosComposition(idConcept)
        Dim tbCauses As DataTable = CausesServiceMethods.GetCauses(Me.Page)

        cmbFixedField.Items.Clear()
        Dim dTblUF As DataTable = API.UserFieldServiceMethods.GetUserFields(Me.Page, Types.EmployeeField, "", False)
        For Each dRow As DataRow In dTblUF.Rows
            If Not dRow("FieldType") Is DBNull.Value Then
                'Si es tipo numerico o decimal, mostrar en pagos
                If dRow("FieldType") = 1 Or dRow("FieldType") = 3 Then
                    cmbFixedField.Items.Add(New DevExpress.Web.ListEditItem(dRow("FieldName"), "USR_" & dRow("FieldName")))
                End If
            End If
        Next

        cmbExpirationPeriodType.Items.Clear()
        cmbExpirationPeriodType.Items.Add(New DevExpress.Web.ListEditItem(Language.Translate("Day", DefaultScope), "0"))
        cmbExpirationPeriodType.Items.Add(New DevExpress.Web.ListEditItem(Language.Translate("Week", DefaultScope), "1"))

        cmbShowValue.Items.Clear()
        cmbShowValue.Items.Add(New DevExpress.Web.ListEditItem(Language.Translate("YearAccrual", DefaultScope), "Y"))
        cmbShowValue.Items.Add(New DevExpress.Web.ListEditItem(Language.Translate("MonthAccrual", DefaultScope), "M"))
        cmbShowValue.Items.Add(New DevExpress.Web.ListEditItem(Language.Translate("WeekAccrual", DefaultScope), "W"))
        cmbShowValue.Items.Add(New DevExpress.Web.ListEditItem(Language.Translate("ContractAccrual", DefaultScope), "C"))
        Dim oLatamMex As New AdvancedParameter.roAdvancedParameter("Latam.Mex", New AdvancedParameter.roAdvancedParameterState())
        If (conceptPeriodType Is Nothing OrElse conceptPeriodType = String.Empty OrElse conceptPeriodType = "L") AndAlso roTypes.Any2Boolean(oLatamMex.Value) Then
            cmbShowValue.Items.Add(New DevExpress.Web.ListEditItem(Language.Translate("LaboralYearAccrual", DefaultScope), "L"))
        End If

        cmbDaysAutomaticAccrual.Items.Clear()
        cmbDaysAutomaticAccrual.ValueType = GetType(Integer)
        cmbDaysAutomaticAccrual.Items.Add(Me.Language.Translate("AutomaticAccrual.DirectValue", DefaultScope), 0)
        cmbDaysAutomaticAccrual.Items.Add(Me.Language.Translate("AutomaticAccrual.UserFieldValue", DefaultScope), 1)

        cmbHoursAutomaticAccrual.Items.Clear()
        cmbHoursAutomaticAccrual.ValueType = GetType(Integer)
        cmbHoursAutomaticAccrual.Items.Add(Me.Language.Translate("AutomaticAccrual.DirectValue", DefaultScope), 0)
        cmbHoursAutomaticAccrual.Items.Add(Me.Language.Translate("AutomaticAccrual.UserFieldValue", DefaultScope), 1)

        cmbHoursAutomaticAccrual_Userfield.Items.Clear()
        cmbDaysAutomaticAccrual_Userfield.Items.Clear()
        Dim tbFactorUserFields As DataTable = API.UserFieldServiceMethods.GetUserFields(Me.Page, Types.EmployeeField, "FieldType IN(1,3) AND Used=1", False)
        For Each oRow As DataRow In tbFactorUserFields.Select("", "FieldName")
            cmbHoursAutomaticAccrual_Userfield.Items.Add(New DevExpress.Web.ListEditItem(oRow("FieldName"), oRow("FieldName")))
            cmbDaysAutomaticAccrual_Userfield.Items.Add(New DevExpress.Web.ListEditItem(oRow("FieldName"), oRow("FieldName")))
        Next

        cmbAutomaticAccrualCause.Items.Clear()
        cmbAutomaticAccrualCause.ValueType = GetType(Integer)

        cmbExpirationPeriodCause.Items.Clear()
        cmbExpirationPeriodCause.ValueType = GetType(Integer)

        For Each oRow As DataRow In tbCauses.Select("", "Name")
            cmbAutomaticAccrualCause.Items.Add(New DevExpress.Web.ListEditItem(oRow("Name"), oRow("Id")))
            cmbExpirationPeriodCause.Items.Add(New DevExpress.Web.ListEditItem(oRow("Name"), oRow("Id")))
        Next
    End Sub

    Private Sub ProcessGroupSelectedTabVisible(ByVal oParameters As ConceptsCallbackRequest)
        'Mostra el TAB seleccionat

        Me.div00.Style("display") = "none"
        Me.div1.Style("display") = "none"
        Me.div2.Style("display") = "none"
        Me.div3.Style("display") = "none"
        Me.div4.Style("display") = "none"
        Me.div5.Style("display") = "none"
        Me.div6.Style("display") = "none"
        Me.div7.Style("display") = "none"
        Me.div8.Style("display") = "none"
        Me.divDailyRecord.Style("display") = "none"

        Select Case oParameters.aTab
            Case 0
                Me.div00.Style("display") = ""
            Case 1
                Me.div2.Style("display") = ""
            Case 2
                Me.div3.Style("display") = ""
            Case 3
                Me.div4.Style("display") = ""
            Case 4
                Me.div5.Style("display") = ""
            Case 5
                Me.div6.Style("display") = ""
            Case 6
                Me.div7.Style("display") = ""
            Case 7
                Me.div8.Style("display") = ""
            Case 8
                Me.div9.Style("display") = ""
        End Select

        Dim oLicSupport As New roServerLicense()
        Dim dailyRecordInstalled As Boolean = oLicSupport.FeatureIsInstalled("Feature\DailyRecord")

        If dailyRecordInstalled Then
            Me.divDailyRecord.Style("display") = ""
        Else
            Me.divDailyRecord.Style("display") = "none"
        End If

    End Sub

#End Region

#Region "Concept Group"

    Private Sub ProcessConceptGroupRequest(ByVal oParameters As ConceptsCallbackRequest)
        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)

        If Me.oPermission > Permission.None Then
            'Mostra el TAB seleccionat

            Select Case oParameters.Action
                Case "GETCONCEPTGROUP"
                    LoadConceptGroup(oParameters)
                Case "SAVECONCEPTGROUP"
                    SaveConceptGroup(oParameters)
            End Select

            Select Case oParameters.aTab
                Case 0
                    Me.div20.Style("display") = ""
            End Select
        End If
    End Sub

    Private Sub LoadConceptGroup(ByVal oParameters As ConceptsCallbackRequest)
        Dim oCurrentConceptGroup As roConceptGroup = API.ConceptsServiceMethods.GetConceptGroupByID(Me, oParameters.ID, True)

        Me.gridConceptGroups.Controls.Add(creaGridConceptGroups(New DataTable, , False))

        Me.loadCombosConcepts()

        Me.txtConceptGroupName.Text = oCurrentConceptGroup.Name
        Me.txtBusinessGroup.Text = oCurrentConceptGroup.BusinessCenter

        ASPxCallbackPanelContenido.JSProperties.Add("cpActionRO", "GETCONCEPTGROUP")
        ASPxCallbackPanelContenido.JSProperties.Add("cpNameRO", oCurrentConceptGroup.Name)
        ASPxCallbackPanelContenido.JSProperties.Add("cpCompositionsGridRO", CreateConceptGroupsItemJSON(oCurrentConceptGroup.ID))
        ASPxCallbackPanelContenido.JSProperties.Add("cpResultRO", "OK")
        ASPxCallbackPanelContenido.JSProperties.Add("cpIsNewRO", False)

    End Sub

    Private Sub SaveConceptGroup(ByVal oParameters As ConceptsCallbackRequest)
        Dim strError As String = ""
        Dim strMessage As String = ""

        Dim oConceptGroupsItems As New Generic.List(Of Object)
        Dim oJSONField As Generic.List(Of JSONFieldItem)

        Try
            Dim oArrComp As New Generic.List(Of roConcept)

            'Check Permissions
            If Me.oPermission < Permission.Write Then
                strError = "KO"
                strMessage = Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope)
            End If

            If strError = String.Empty Then
                Dim bolIsNew As Boolean = False
                If oParameters.ID = 0 Or oParameters.ID = -1 Then bolIsNew = True

                If Me.oPermission = Permission.Write And bolIsNew Then
                    strError = "KO"
                    strMessage = Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope)
                End If

                If strError = String.Empty Then
                    Dim oCurrentConceptGroup As roConceptGroup = API.ConceptsServiceMethods.GetConceptGroupByID(Me, oParameters.ID, False)
                    If oCurrentConceptGroup Is Nothing Then Exit Sub

                    oCurrentConceptGroup.Name = txtConceptGroupName.Text
                    oCurrentConceptGroup.BusinessCenter = txtBusinessGroup.Text

                    Dim parameters As String() = oParameters.resultClientAction.Split("&")

                    Dim idConcept As String = ""
                    Dim nameConcept As String = ""
                    For Each oParam As String In parameters
                        If oParam.ToUpper.StartsWith("CONCEPT_") Then
                            Dim strIndex As String = oParam.Replace("CONCEPT_", "")
                            Dim oIndex As Integer = CInt(strIndex.Substring(0, strIndex.IndexOf("_")))
                            Dim oName As String() = strIndex.Substring(strIndex.IndexOf("_") + 1).Split("=")

                            Select Case oName(0).ToUpper
                                Case "IDCONCEPT"
                                    idConcept = oName(1)
                                    Dim oConcept As New roConcept()
                                    oConcept.ID = roTypes.Any2Integer(oName(1))
                                    oArrComp.Add(oConcept)
                                Case Else
                                    nameConcept = oName(1)
                                    oJSONField = New Generic.List(Of JSONFieldItem)
                                    oJSONField.Add(New JSONFieldItem("IDConcept", idConcept, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                                    oJSONField.Add(New JSONFieldItem("NameConcept", HttpUtility.UrlDecode(nameConcept), Nothing, roJSON.JSONType.None_JSON, Nothing, False))

                                    oConceptGroupsItems.Add(oJSONField)
                            End Select
                        End If
                    Next

                    If oArrComp.Count > 0 Then
                        Dim oAComp(-1) As roConcept
                        For Each oCm As roConcept In oArrComp
                            ReDim Preserve oAComp(oAComp.Length)
                            oAComp(oAComp.Length - 1) = oCm
                        Next
                        oCurrentConceptGroup.Concepts = oAComp.ToList
                    Else
                        oCurrentConceptGroup.Concepts = Nothing
                    End If

                    If Not API.ConceptsServiceMethods.SaveConceptGroup(Me, oCurrentConceptGroup, True) = True Then
                        strError = "KO"
                        strMessage = API.ConceptsServiceMethods.LastErrorText
                    Else
                        oParameters.ID = oCurrentConceptGroup.ID
                    End If
                End If
            End If
        Catch ex As Exception
            strError = "KO"
            strMessage = ex.Message
        End Try

        If strError = "KO" Then

            Dim strJSONGroups As String = ""
            For Each oObj As Object In oConceptGroupsItems
                Dim strJSONText As String = ""
                strJSONText &= "{ ""concepts"": "
                strJSONText &= roJSONHelper.Serialize(oObj)
                strJSONText &= " } ,"
                strJSONGroups &= strJSONText
            Next

            If strJSONGroups.EndsWith(",") Then strJSONGroups = strJSONGroups.Substring(0, Len(strJSONGroups) - 1)
            Me.gridConceptGroups.Controls.Add(creaGridConceptGroups(New DataTable, , False))

            Me.loadCombosConcepts()

            ASPxCallbackPanelContenido.JSProperties.Add("cpActionRO", "SAVECONCEPTGROUP")
            ASPxCallbackPanelContenido.JSProperties.Add("cpResultRO", "KO")
            ASPxCallbackPanelContenido.JSProperties.Add("cpMessageRO", strMessage)
            ASPxCallbackPanelContenido.JSProperties.Add("cpCompositionsGridRO", strJSONGroups)
        Else
            HelperWeb.roSelector_SetSelection(oParameters.ID, "/source/" & oParameters.ID, "ctl00_contentMainBody_roTreesConceptGroups")
            LoadConceptGroup(oParameters)
            ASPxCallbackPanelContenido.JSProperties("cpIsNewRO") = True
            ASPxCallbackPanelContenido.JSProperties("cpActionRO") = "SAVECONCEPTGROUP"
            ASPxCallbackPanelContenido.JSProperties("cpNewIdRO") = oParameters.ID
        End If

    End Sub

    Private Sub loadCombosConcepts()
        Try
            Dim dTbl As DataTable = API.ConceptsServiceMethods.GetConcepts(Me)

            cmbConcepts.Items.Clear()
            For Each dRow As DataRow In dTbl.Rows
                cmbConcepts.Items.Add(New DevExpress.Web.ListEditItem(dRow("Name"), dRow("ID")))
            Next
        Catch ex As Exception
        End Try
    End Sub

    Private Function creaGridConceptGroups(ByVal dTable As DataTable, Optional ByVal dTblControls As DataTable = Nothing, Optional ByVal editIcons As Boolean = False) As HtmlTable
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

            hTable.ID = "tblGridConceptGroups"
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

            'Bucle als registres
            For n As Integer = 0 To dTable.Rows.Count - 1
                hTRow = New HtmlTableRow
                altRow = IIf(altRow = "1", "2", "1")

                For y As Integer = 0 To dTable.Columns.Count - 1
                    'Comproba si es una columna que no es te de visualitzar
                    If dTblControls IsNot Nothing Then
                        Dim dRowSel() As DataRow = dTblControls.Select("NomCamp = '" & dTable.Columns(y).ColumnName & "'")
                        If dRowSel.Length > 0 Then
                            If dRowSel(0).Item("Visible") = False Then Continue For
                        End If
                    End If

                    hTCell = New HtmlTableCell

                    'Cambia el alternateRow
                    hTCell.Attributes("class") = "GridStyle-cell" & altRow

                    'hTCell.InnerText = dTable.Columns(y).ColumnName & ":" & dTable.Rows(n)(y).ToString
                    If editIcons = True Then
                        hTCell.Attributes("style") = "padding: 0px;"
                        strClickEdit = "editGridConceptGroup("
                        If dTblControls IsNot Nothing Then
                            For Each dRow As DataRow In dTblControls.Rows
                                strClickEdit &= "'" & dTable.Rows(n)(dRow(0)).ToString & "',"
                            Next
                        End If
                        strClickEdit = strClickEdit.Substring(0, strClickEdit.Length - 1) & ");"
                        hTCell.InnerHtml = "<a href=""javascript: void(0);"" class=""rowSelectable"" onclick=""" & strClickEdit & """>" & dTable.Rows(n)(y).ToString & "</a>"
                    Else
                        hTCell.InnerText = dTable.Rows(n)(y).ToString
                    End If

                    If hTCell.InnerText = "" Then hTCell.InnerText = " "
                    'Si es la ultima columna (per tancar el row)
                    If y = dTable.Columns.Count - 1 Then hTCell.Attributes("class") = hTCell.Attributes("class") & "  GridStyle-endcell" & altRow

                    'Carrega la celda al row
                    hTRow.Cells.Add(hTCell)

                    'Dibuixem les columnes de les icones d'edicio
                    If oPermission = Permission.Admin Then
                        If editIcons = True Then
                            If y = dTable.Columns.Count - 1 Then
                                hTCell = New HtmlTableCell
                                hTCell.Width = "40px"
                                hTCell.Attributes("class") = "GridStyle-cellheader"
                                hTCell.Attributes("style") = "border: 0; background-color: #E8EEF7;border-right: solid 1px #D7D7D7;"
                                Dim hAnchorEdit As New HtmlAnchor
                                Dim hAnchorRemove As New HtmlAnchor

                                strClickEdit = "editGridConceptGroup("
                                strClickRemove = "deleteGridConceptGroup("
                                If dTblControls IsNot Nothing Then
                                    For Each dRow As DataRow In dTblControls.Rows
                                        strClickEdit &= "'" & dTable.Rows(n)(dRow(0)).ToString & "',"
                                        strClickRemove &= "'" & dTable.Rows(n)(dRow(0)).ToString & "',"
                                    Next
                                End If
                                strClickEdit = strClickEdit.Substring(0, strClickEdit.Length - 1) & ");"
                                strClickRemove = strClickRemove.Substring(0, strClickRemove.Length - 1) & ");"

                                hAnchorEdit.Attributes("onclick") = strClickEdit
                                hAnchorEdit.Title = Me.Language.Keyword("Button.Edit")
                                hAnchorEdit.HRef = "javascript: void(0);"
                                hAnchorEdit.InnerHtml = "<img src=""" & Me.Page.ResolveUrl("~/Base/Images/Grid/edit.png") & """>"

                                hAnchorRemove.Attributes("onclick") = strClickRemove
                                hAnchorRemove.Title = Me.Language.Keyword("Button.Delete")
                                hAnchorRemove.HRef = "javascript: void(0);"
                                hAnchorRemove.InnerHtml = "<img src=""" & Me.Page.ResolveUrl("~/Base/Images/Grid/remove.png") & """>"
                                hTCell.Controls.Add(hAnchorEdit)
                                hTCell.Controls.Add(hAnchorRemove)

                                hTRow.Cells.Add(hTCell)
                            End If
                        End If
                    End If
                Next
                hTable.Rows.Add(hTRow)
            Next
            Return hTable
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Private Function CreateConceptGroupsItemJSON(ByVal IDConceptGroup As Integer) As String
        Try
            Dim n As Integer = 0

            Dim oConceptGroupsItems As New Generic.List(Of Object)

            Dim oJSONField As Generic.List(Of JSONFieldItem)
            Dim strJSONGroups As String = ""

            Dim oCurrentConceptGroup As roConceptGroup = API.ConceptsServiceMethods.GetConceptGroupByID(Me, IDConceptGroup, False)
            If oCurrentConceptGroup Is Nothing Then Return ""

            'Si el concepte no te composicions, passem un objecte Json per omisio
            If oCurrentConceptGroup.Concepts Is Nothing Then
                'strJSONGroups &= "{ 'concepts': ["
                oJSONField = New Generic.List(Of JSONFieldItem)
                oJSONField.Add(New JSONFieldItem("IDConcept", "", Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                oJSONField.Add(New JSONFieldItem("NameConcept", "", Nothing, roJSON.JSONType.None_JSON, Nothing, False))

                'Dim strJSON As String = oJSONField.CreateJSON
                'strJSONGroups &= strJSON & " ]} ,"
                oConceptGroupsItems.Add(oJSONField)
            Else
                'Bucle al compositions
                For Each oConcept As roConcept In oCurrentConceptGroup.Concepts
                    oJSONField = New Generic.List(Of JSONFieldItem)

                    'strJSONGroups &= "{ 'concepts': ["
                    'oJSONField = New roJSON
                    oJSONField.Add(New JSONFieldItem("IDConcept", oConcept.ID, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    oJSONField.Add(New JSONFieldItem("NameConcept", oConcept.Name, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    'Dim strJSON As String = oJSONField.CreateJSON
                    'strJSONGroups &= strJSON & " ]} ,"

                    oConceptGroupsItems.Add(oJSONField)
                Next
            End If

            strJSONGroups = ""
            For Each oObj As Object In oConceptGroupsItems
                Dim strJSONText As String = ""
                strJSONText &= "{ ""concepts"": "
                strJSONText &= roJSONHelper.Serialize(oObj)
                strJSONText &= " } ,"
                strJSONGroups &= strJSONText
            Next

            If strJSONGroups.EndsWith(",") Then strJSONGroups = strJSONGroups.Substring(0, Len(strJSONGroups) - 1)

            Return strJSONGroups
        Catch ex As Exception
            Dim rError As New roJSON.JSONError(True, ex.Message.ToString & ex.StackTrace.ToString)
            Return ""
        End Try
    End Function

#End Region

End Class