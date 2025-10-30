Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBusiness.Cause
Imports Robotics.Base.VTBusiness.Concept
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class Causes
    Inherits PageBase

    <Runtime.Serialization.DataContract()>
    Private Class CausesCallbackRequest

        <Runtime.Serialization.DataMember(Name:="aTab")>
        Public aTab As Integer

        <Runtime.Serialization.DataMember(Name:="ID")>
        Public ID As Integer

        <Runtime.Serialization.DataMember(Name:="Name")>
        Public Name As String

        <Runtime.Serialization.DataMember(Name:="action")>
        Public Action As String

        <Runtime.Serialization.DataMember(Name:="StampParam")>
        Public StampParam As Double

        <Runtime.Serialization.DataMember(Name:="gridDocuments")>
        Public gridDocuments As GridDocument()

    End Class

    <Runtime.Serialization.DataContract()>
    Private Class GridDocument

        <Runtime.Serialization.DataMember(Name:="iddocumenttrace")>
        Public IdDocumentTrace As Integer

        <Runtime.Serialization.DataMember(Name:="idcause")>
        Public IdCause As Integer

        <Runtime.Serialization.DataMember(Name:="idlabagree")>
        Public IdLabAgree As Integer

        <Runtime.Serialization.DataMember(Name:="iddocument")>
        Public IdDocument As Integer

        <Runtime.Serialization.DataMember(Name:="typerequest")>
        Public TypeRequest As Integer

        <Runtime.Serialization.DataMember(Name:="numitems")>
        Public NumItems As Integer

        <Runtime.Serialization.DataMember(Name:="numitems2")>
        Public NumItems2 As Integer

        <Runtime.Serialization.DataMember(Name:="flexiblewhen")>
        Public FlexibleWhen As Integer

        <Runtime.Serialization.DataMember(Name:="flexiblewhen2")>
        Public FlexibleWhen2 As Integer

        <Runtime.Serialization.DataMember(Name:="namelabagree")>
        Public NameLabAgree As String

        <Runtime.Serialization.DataMember(Name:="namedocument")>
        Public NameDocument As String

        <Runtime.Serialization.DataMember(Name:="nameinterval")>
        Public NameInterval As String

    End Class

    Private Const FeatureAlias As String = "Causes.Definition"
    Private oPermission As Permission

    Private Const FeatureCostCenterAlias As String = "BusinessCenters"
    Private oPermissionCostCenter As Permission

    Private bolCostCentersLicense As Boolean = False

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("Ajax", "~/Base/Scripts/Ajax.js")
        Me.InsertExtraJavascript("jsDate", "~/Base/Scripts/jsDate.js")
        Me.InsertExtraJavascript("GenericData", "~/Base/Scripts/GenericData.js")
        Me.InsertExtraJavascript("roOptionPanelClient", "~/Base/Scripts/roOptionPanelClient.js")
        Me.InsertExtraJavascript("roOptPanelClientGroup", "~/Base/Scripts/roOptPanelClientGroup.js")
        Me.InsertExtraJavascript("jsGrid", "~/Base/Scripts/jsGrid.js")
        Me.InsertExtraJavascript("CausesV2", "~/Causes/Scripts/CausesV2.js")
    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Comprueba la licencia
        If HelperSession.GetFeatureIsInstalledFromApplication("Forms\Causes") = False Then
            WLHelperWeb.RedirectAccessDenied(False)
            Exit Sub
        End If

        Me.oPermission = Me.GetFeaturePermission("Causes.Definition")
        If oPermission = Permission.None Then
            WLHelperWeb.RedirectAccessDenied(False)
        End If

        If oPermission < Permission.Write Then
            hdnModeEdit.Value = "true"
            Me.DisableControls(Me.Controls)
        Else
            hdnModeEdit.Value = "false"

        End If

        Me.causesV3Config.Style("display") = ""

        If Not IsPostBack Then
            Me.roTreesCauses.TreeCaption = Me.Language.Translate("TreeCaptionCauses", Me.DefaultScope)

            Me.lblSaveChanges.InnerText = Me.Language.Translate("msgSaveChangesbar", DefaultScope)
            Me.lblSaveChangesBottom.InnerText = Me.lblSaveChanges.InnerText
            Me.lblUndoChanges.InnerText = Me.Language.Translate("msgUndoChangesbar", DefaultScope)
            Me.lblUndoChangesBottom.InnerText = Me.lblUndoChanges.InnerText

            Me.msgHasChanges.Value = Me.Language.Translate("msgHasChanges", DefaultScope)
            Me.msgHasErrors.Value = Me.Language.Translate("msgHasErrors", DefaultScope)

            Me.hdnLabAgreeTitle.Value = Me.Language.Translate("gridHeaderLabAgree", DefaultScope)
            Me.hdnDocumentTitle.Value = Me.Language.Translate("gridHeaderDocument", DefaultScope)
            Me.hdnIntervalTitle.Value = Me.Language.Translate("gridHeaderInterval", DefaultScope)
            Me.hdnAbsencesAtBegin.Value = Me.Language.Translate("DocumentAbsences.AtBegin", DefaultScope)
            Me.hdnAbsencesAtEnd.Value = Me.Language.Translate("DocumentAbsences.AtEnd", DefaultScope)
            Me.hdnAbsencesEveryDays.Value = Me.Language.Translate("DocumentAbsences.EveryDaysJS", DefaultScope)
            Me.hdnAbsencesEveryWeeks.Value = Me.Language.Translate("DocumentAbsences.EveryWeeksJS", DefaultScope)
            Me.hdnAbsencesEveryMonths.Value = Me.Language.Translate("DocumentAbsences.EveryMonthsJS", DefaultScope)
            Me.hdnAbsencesEveryFlexible1.Value = Me.Language.Translate("DocumentAbsences.EveryFlexible1JS", DefaultScope)
            Me.hdnAbsencesEveryFlexible2.Value = Me.Language.Translate("DocumentAbsences.EveryFlexible2JS", DefaultScope)

            LoadCombos()
        End If

        Me.oPermissionCostCenter = Me.GetFeaturePermission(FeatureCostCenterAlias)

        Me.bolCostCentersLicense = HelperSession.GetFeatureIsInstalledFromApplication("Feature\CostControl")

    End Sub

    Protected Sub ASPxCallbackPanelContenido_Callback(ByVal sender As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase) Handles ASPxCallbackPanelContenido.Callback

        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Dim oParameters As New CausesCallbackRequest()
        oParameters = roJSONHelper.Deserialize(strParameter, oParameters.GetType())

        ProcessCausesRequest(oParameters)

    End Sub

    Private Sub ProcessCausesRequest(ByVal oParameters As CausesCallbackRequest)

        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)

        If roTypes.Any2Integer(HelperSession.AdvancedParametersCache("Causes.AllowDefaultValues")) = 1 Then
            Me.causeDefaultParameters.Style("display") = ""
        End If

        If Me.oPermission > Permission.None Then
            Select Case oParameters.Action
                Case "GETCAUSE"
                    LoadCause(oParameters)
                Case "SAVECAUSE"
                    SaveCause(oParameters)
            End Select

            ProcessCauseSelectedTabVisible(oParameters)
        End If

    End Sub

    Private Sub LoadCombos()

        Me.cmbConceptBalance.Items.Clear()
        Me.cmbConceptBalance.ValueType = GetType(Integer)
        Me.cmbConceptBalance.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("Concepts.None", DefaultScope), 0))

        Me.cmbConceptBalanceProductive.Items.Clear()
        Me.cmbConceptBalanceProductive.ValueType = GetType(Integer)
        Me.cmbConceptBalanceProductive.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("Concepts.None", DefaultScope), 0))

        Me.cmbAbsenceConceptBalance.Items.Clear()
        Me.cmbAbsenceConceptBalance.ValueType = GetType(Integer)
        Me.cmbAbsenceConceptBalance.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("Concepts.None", DefaultScope), 0))

        Me.cmbAbsenceConceptBalanceDays.Items.Clear()
        Me.cmbAbsenceConceptBalanceDays.ValueType = GetType(Integer)
        Me.cmbAbsenceConceptBalanceDays.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("RequestDays.WorkingDay", DefaultScope), 1))
        Me.cmbAbsenceConceptBalanceDays.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("RequestDays.NotWorkingDay", DefaultScope), 0))

        Me.cmbConceptBalanceProductiveDays.Items.Clear()
        Me.cmbConceptBalanceProductiveDays.ValueType = GetType(Integer)
        Me.cmbConceptBalanceProductiveDays.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("RequestDays.WorkingDay", DefaultScope), 1))
        Me.cmbConceptBalanceProductiveDays.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("RequestDays.NotWorkingDay", DefaultScope), 0))

        Dim dTblConcepts As DataTable = API.ConceptsServiceMethods.GetConcepts(Me)
        For Each dRC As DataRow In dTblConcepts.Rows
            If dRC("ID") <> 0 AndAlso (roTypes.Any2String(dRC("DefaultQuery")) = "Y" OrElse roTypes.Any2String(dRC("DefaultQuery")) = "C" OrElse roTypes.Any2String(dRC("DefaultQuery")) = "M") AndAlso roTypes.Any2String(dRC("IDType")) = "H" AndAlso roTypes.Any2Boolean(dRC("ApplyOnHolidaysRequest")) Then
                Me.cmbConceptBalanceProductive.Items.Add(New DevExpress.Web.ListEditItem(dRC("Name"), dRC("ID")))
                Me.cmbConceptBalance.Items.Add(New DevExpress.Web.ListEditItem(dRC("Name"), dRC("ID")))
            End If

            If dRC("ID") <> 0 AndAlso (roTypes.Any2String(dRC("DefaultQuery")) = "Y" OrElse roTypes.Any2String(dRC("DefaultQuery")) = "C" OrElse roTypes.Any2String(dRC("DefaultQuery")) = "M") AndAlso (roTypes.Any2String(dRC("IDType")) = "H" OrElse roTypes.Any2String(dRC("IDType")) = "O") AndAlso roTypes.Any2Boolean(dRC("ApplyOnHolidaysRequest")) Then
                Me.cmbAbsenceConceptBalance.Items.Add(New DevExpress.Web.ListEditItem(dRC("Name"), dRC("ID")))
            End If
        Next

        Me.cmbCauseType.Items.Clear()
        Me.cmbCauseType.ValueType = GetType(Integer)
        Me.cmbCauseType.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("Causes.Hours", DefaultScope), 0))
        Me.cmbCauseType.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("Causes.Days", DefaultScope), 1))
        Me.cmbCauseType.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("Causes.Custom", DefaultScope), 2))

        Me.cmbDayCauses.Items.Clear()
        Me.cmbDayCauses.ValueType = GetType(Integer)

        Dim dTblCauses As DataTable = CausesServiceMethods.GetCausesShortList(Me.Page)
        Dim oRows = dTblCauses.Select("DayType = 0 AND CustomType = 0")

        For Each dRC As DataRow In oRows
            Me.cmbDayCauses.Items.Add(dRC("Name"), dRC("ID"))
        Next

        cmbCauseFromUserField.Items.Clear()
        Dim tbFactorUserFields As DataTable = API.UserFieldServiceMethods.GetUserFields(Me.Page, Types.EmployeeField, "FieldType IN(1,3) AND Used=1", False)
        For Each oRow As DataRow In tbFactorUserFields.Select("", "FieldName")
            cmbCauseFromUserField.Items.Add(New DevExpress.Web.ListEditItem(oRow("FieldName"), oRow("FieldName")))
        Next

        Me.tbAvailableAbsenceRequests.Items.Clear()
        Me.tbAvailableAbsenceRequests.Items.Add(Me.Language.Translate("CausesRequestScope.ProgrammedCause", DefaultScope), 9)
        Me.tbAvailableAbsenceRequests.Items.Add(Me.Language.Translate("CausesRequestScope.ProgrammedAbsence", DefaultScope), 7)

        Me.tbAvailableOverWorkRequests.Items.Clear()
        Me.tbAvailableOverWorkRequests.Items.Add(Me.Language.Translate("CausesRequestScope.ProgrammedOvertime", DefaultScope), 14)
        Me.tbAvailableOverWorkRequests.Items.Add(Me.Language.Translate("CausesRequestScope.ExternalWork", DefaultScope), 4)

        Me.tbAvailableHolidayRequests.Items.Clear()
        Me.tbAvailableHolidayRequests.Items.Add(Me.Language.Translate("CausesRequestScope.ProgrammedHolidays", DefaultScope), 13)

        Me.cmbRequestCategory.Items.Clear()
        Me.cmbAtLevel.Items.Clear()
        Me.cmbMinLevel.Items.Clear()
        Me.cmbRequestCategory.ValueType = GetType(Integer)

        Dim dTbl As List(Of roSecurityCategory) = SecurityV3ServiceMethods.GetRequestCategories(Me.Page)

        If dTbl IsNot Nothing Then
            For Each dRow As roSecurityCategory In dTbl
                Me.cmbRequestCategory.Items.Add(New DevExpress.Web.ListEditItem(dRow.Description, dRow.ID))
            Next
        End If

        Me.cmbRequestCategory.SelectedItem = cmbRequestCategory.Items.FindByValue(6)

        For index As Integer = 1 To 10
            Me.cmbMinLevel.Items.Add(index.ToString, index)
        Next

        Me.cmbMinLevel.SelectedItem = cmbMinLevel.Items.FindByValue("10")

        For index2 As Integer = 2 To 11
            Me.cmbAtLevel.Items.Add(index2.ToString, index2)
        Next

        Me.cmbAtLevel.SelectedItem = cmbMinLevel.Items.FindByValue("2")

    End Sub

    Private Sub LoadCause(ByVal oParameters As CausesCallbackRequest, Optional ByVal eCause As roCause = Nothing)
        Dim oCurrentCause As roCause = Nothing
        Dim visibilityCombosIndex As String = ""
        Dim punchesCombosIndex As String = ""
        Dim strJSONgrids As String = ""
        Try
            Dim bDisable As Boolean = True
            If oPermission > Permission.Read Then
                bDisable = False
                Me.panTbDocumentTrace.Style("display") = ""
                Me.hdnReadOnlyMode.Value = "0"
            Else
                Me.panTbDocumentTrace.Style("display") = "none"
                Me.hdnReadOnlyMode.Value = "1"
            End If

            If eCause Is Nothing Then
                If oParameters.ID = -1 Then
                    oCurrentCause = New roCause
                    oCurrentCause.RequestAvailability = "-1"
                Else
                    oCurrentCause = CausesServiceMethods.GetCauseByID(Me, oParameters.ID, True)
                End If
            Else
                oCurrentCause = eCause
            End If

            If oCurrentCause Is Nothing Then Exit Sub

            Dim strAuxTime As String = String.Empty
            Dim bDisableTemp As Boolean = True

            'Pestaña "General"
            '=================
            Me.txtName.Text = oCurrentCause.Name
            Me.txtShortName.Text = oCurrentCause.ShortName
            Me.txtDescription.Text = oCurrentCause.Description
            Me.txtFactor.Text = oCurrentCause.Export
            Me.txtBusinessGroup.Text = oCurrentCause.BusinessCenter

            Me.txtCostFactor.Text = oCurrentCause.CostFactor
            Me.colorCause.Color = System.Drawing.ColorTranslator.FromWin32(oCurrentCause.Color)

            If oPermissionCostCenter > Permission.None Then
                txtCostFactor.Visible = True
            Else
                txtCostFactor.Visible = False
            End If

            If oPermissionCostCenter = Permission.Admin And oPermission = Permission.Admin Then
                txtCostFactor.Enabled = True
            Else
                txtCostFactor.Enabled = False
            End If

            If bolCostCentersLicense Then
                divCostFactor.Style("display") = ""
                divCostDetailNoLicense.Style("display") = "none"
            Else
                divCostFactor.Style("display") = "none"
                divCostDetailNoLicense.Style("display") = ""
            End If

            'Pestaña "Tipo"
            '==============

            cmbCauseType.SelectedItem = cmbCauseType.Items.FindByValue(0)
            If oCurrentCause.DayType Then
                cmbCauseType.SelectedItem = cmbCauseType.Items.FindByValue(1)
            ElseIf oCurrentCause.CustomType Then
                cmbCauseType.SelectedItem = cmbCauseType.Items.FindByValue(2)
            End If

            If cmbCauseFromUserField.Items.Count > 0 Then cmbCauseFromUserField.SelectedItem = cmbCauseFromUserField.Items(0)
            If cmbDayCauses.Items.Count > 0 Then cmbDayCauses.SelectedItem = cmbDayCauses.Items(0)

            If oCurrentCause.AutomaticEquivalenceCriteria IsNot Nothing AndAlso oCurrentCause.AutomaticEquivalenceType <> eAutomaticEquivalenceType.DeactivatedType Then
                cmbDayCauses.SelectedItem = cmbDayCauses.Items.FindByValue(oCurrentCause.AutomaticEquivalenceIDCause)

                Select Case oCurrentCause.AutomaticEquivalenceType
                    Case eAutomaticEquivalenceType.DeactivatedType
                        Me.optDaysFromCause.Checked = False
                        Me.ckCauseFromWorkingHours.Checked = True
                        Me.ckCauseFromUserField.Checked = False
                        Me.ckCauseFromFactor.Checked = False
                    Case eAutomaticEquivalenceType.DirectValueType
                        Me.optDaysFromCause.Checked = True
                        Me.ckCauseFromWorkingHours.Checked = False
                        Me.ckCauseFromUserField.Checked = False
                        Me.ckCauseFromFactor.Checked = True

                        Me.txtCauseFromFactor.Value = oCurrentCause.AutomaticEquivalenceCriteria.FactorValue

                    Case eAutomaticEquivalenceType.ExpectedWorkingHoursType
                        Me.optDaysFromCause.Checked = True
                        Me.ckCauseFromWorkingHours.Checked = True
                        Me.ckCauseFromUserField.Checked = False
                        Me.ckCauseFromFactor.Checked = False
                    Case eAutomaticEquivalenceType.FieldType
                        Me.optDaysFromCause.Checked = True
                        Me.ckCauseFromWorkingHours.Checked = False
                        Me.ckCauseFromUserField.Checked = True
                        Me.ckCauseFromFactor.Checked = False

                        Me.cmbCauseFromUserField.SelectedItem = Me.cmbCauseFromUserField.Items.FindByValue(oCurrentCause.AutomaticEquivalenceCriteria.UserField.FieldName)
                End Select
            Else
                Me.optDaysFromCause.Checked = False
                Me.ckCauseFromWorkingHours.Checked = True
                Me.ckCauseFromUserField.Checked = False
                Me.ckCauseFromFactor.Checked = False
            End If

            cmbAbsenceConceptBalance.SelectedItem = cmbAbsenceConceptBalance.Items.FindByValue(0)
            cmbAbsenceConceptBalanceDays.SelectedItem = cmbAbsenceConceptBalanceDays.Items.FindByValue(0)
            cmbConceptBalanceProductiveDays.SelectedItem = cmbConceptBalanceProductiveDays.Items.FindByValue(0)
            cmbConceptBalance.SelectedItem = cmbConceptBalance.Items.FindByValue(0)
            cmbConceptBalanceProductive.SelectedItem = cmbConceptBalanceProductive.Items.FindByValue(0)

            If oCurrentCause.WorkingType OrElse (Not oCurrentCause.WorkingType AndAlso oCurrentCause.ExternalWork) Then
                Me.optHoursProductive.Checked = True
                Me.optHoursNonProductive.Checked = False
            Else
                Me.optHoursProductive.Checked = False
                Me.optHoursNonProductive.Checked = True
            End If

            If Me.optHoursProductive.Checked Then
                If oCurrentCause.WorkingType Then
                    Me.optProductiveHoursOnCC.Checked = True
                    Me.optExternalProductiveHours.Checked = False

                    cmbConceptBalanceProductive.SelectedItem = cmbConceptBalanceProductive.Items.FindByValue(oCurrentCause.IDConceptBalance)

                    If Me.cmbConceptBalanceProductive.SelectedItem Is Nothing Then
                        Dim oList As New DevExpress.Web.ListEditItem
                        Dim oConcept As roConcept = API.ConceptsServiceMethods.GetConceptByID(Me.Page, oCurrentCause.IDConceptBalance, False)
                        If oConcept IsNot Nothing Then
                            oList.Text = oConcept.Name & ("*")
                            oList.Value = oConcept.ID
                            Me.cmbConceptBalanceProductive.Items.Add(oList)
                            cmbConceptBalanceProductive.SelectedItem = cmbConceptBalanceProductive.Items.FindByValue(oConcept.ID)
                        End If
                    End If
                    Me.cmbConceptBalanceProductiveDays.SelectedItem = cmbConceptBalanceProductiveDays.Items.FindByValue(If(oCurrentCause.ApplyWorkDaysOnConcept, 1, 0))

                ElseIf oCurrentCause.ExternalWork Then
                    Me.optProductiveHoursOnCC.Checked = False
                    Me.optExternalProductiveHours.Checked = True
                End If
            Else
                Me.optProductiveHoursOnCC.Checked = False
                Me.optExternalProductiveHours.Checked = False
            End If

            If Me.optHoursNonProductive.Checked Then
                If oCurrentCause.IsHoliday Then
                    Me.optHoursNonProductiveCause.Checked = False
                    Me.optHoursNonProductiveHoliday.Checked = True
                    cmbConceptBalance.SelectedItem = cmbConceptBalance.Items.FindByValue(oCurrentCause.IDConceptBalance)

                    If Me.cmbConceptBalance.SelectedItem Is Nothing Then
                        Dim oList As New DevExpress.Web.ListEditItem
                        Dim oConcept As roConcept = API.ConceptsServiceMethods.GetConceptByID(Me.Page, oCurrentCause.IDConceptBalance, False)
                        If oConcept IsNot Nothing Then
                            oList.Text = oConcept.Name & ("*")
                            oList.Value = oConcept.ID
                            Me.cmbConceptBalance.Items.Add(oList)
                            cmbConceptBalance.SelectedItem = cmbConceptBalance.Items.FindByValue(oConcept.ID)
                        End If
                    End If
                Else
                    Me.optHoursNonProductiveCause.Checked = True
                    Me.optHoursNonProductiveHoliday.Checked = False

                    cmbAbsenceConceptBalance.SelectedItem = cmbAbsenceConceptBalance.Items.FindByValue(oCurrentCause.IDConceptBalance)
                    If Me.cmbAbsenceConceptBalance.SelectedItem Is Nothing Then
                        Dim oList As New DevExpress.Web.ListEditItem
                        Dim oConcept As roConcept = API.ConceptsServiceMethods.GetConceptByID(Me.Page, oCurrentCause.IDConceptBalance, False)
                        If oConcept IsNot Nothing Then
                            oList.Text = oConcept.Name & ("*")
                            oList.Value = oConcept.ID
                            Me.cmbConceptBalance.Items.Add(oList)
                            cmbAbsenceConceptBalance.SelectedItem = cmbAbsenceConceptBalance.Items.FindByValue(oConcept.ID)
                        End If
                    End If

                    Me.cmbAbsenceConceptBalanceDays.SelectedItem = cmbAbsenceConceptBalanceDays.Items.FindByValue(If(oCurrentCause.ApplyWorkDaysOnConcept, 1, 0))

                End If
            Else
                Me.optHoursNonProductiveHoliday.Checked = False
                Me.optHoursNonProductiveCause.Checked = False
            End If

            If oParameters.ID = -1 Then
                cmbRequestCategory.SelectedItem = cmbRequestCategory.Items.FindByValue(6)
                Me.chkMinLevel.Checked = False
                Me.cmbMinLevel.SelectedItem = cmbMinLevel.Items.FindByValue("10")
                Me.chkAtLevel.Checked = False
                Me.cmbAtLevel.SelectedItem = cmbAtLevel.Items.FindByValue("2")
            Else
                cmbRequestCategory.SelectedItem = cmbRequestCategory.Items.FindByValue(CInt(oCurrentCause.IDCategory))
                If cmbRequestCategory.SelectedItem Is Nothing Then
                    cmbRequestCategory.SelectedItem = cmbRequestCategory.Items.FindByValue(6)
                End If

                If oCurrentCause.MinLevelOfAuthority = 11 Then
                    Me.chkMinLevel.Checked = False
                    Me.cmbMinLevel.SelectedItem = cmbMinLevel.Items.FindByValue("10")
                Else
                    Me.chkMinLevel.Checked = True
                    Me.cmbMinLevel.SelectedItem = cmbMinLevel.Items.FindByValue(oCurrentCause.MinLevelOfAuthority.ToString)
                End If

                If oCurrentCause.ApprovedAtLevel = 1 Then
                    Me.chkAtLevel.Checked = False
                    Me.cmbAtLevel.SelectedItem = cmbAtLevel.Items.FindByValue("2")
                Else
                    Me.chkAtLevel.Checked = True
                    Me.cmbAtLevel.SelectedItem = cmbAtLevel.Items.FindByValue(oCurrentCause.ApprovedAtLevel.ToString)
                End If
            End If

            Me.optCauseAbsences.Checked = oCurrentCause.StartsProgrammedAbsence
            Me.txtMaxsDaysAbsence.Text = roTypes.Any2String(oCurrentCause.MaxProgrammedAbsence)
            Me.optCauseCloseAbsences.Checked = oCurrentCause.PunchCloseProgrammedAbsence

            Me.optAbsenceMandatoryDays.Checked = IIf(oCurrentCause.AbsenceMandatoryDays = -1, False, True)

            If (Me.optAbsenceMandatoryDays.Checked) Then
                Me.txtMandatoryDaysOnPortalRequest.Text = oCurrentCause.AbsenceMandatoryDays
            Else
                Me.txtMandatoryDaysOnPortalRequest.Text = "0"
            End If

            bDisableTemp = (bDisable Or oCurrentCause.WorkingType)
            If roTypes.Any2Integer(oCurrentCause.Absence_MaxDays) <= 0 Then
                bDisableTemp = True
            End If

            Me.chkMaxDays.Checked = IIf(roTypes.Any2Integer(oCurrentCause.Absence_MaxDays) > 0, True, False)
            If oCurrentCause.Absence_MaxDays.HasValue = True Then
                Me.txtDaysDuration.Text = oCurrentCause.Absence_MaxDays
            Else
                Me.txtDaysDuration.Text = ""
            End If

            If bDisableTemp Then
                Me.chkMaxDays.Disabled = True
            End If

            bDisableTemp = (bDisable Or oCurrentCause.WorkingType)
            If Not oCurrentCause.Absence_BetweenMax.HasValue And Not oCurrentCause.Absence_BetweenMin.HasValue Then
                bDisableTemp = True
            End If

            If oCurrentCause.Absence_BetweenMax.HasValue Or oCurrentCause.Absence_BetweenMin.HasValue Then Me.chkTimePeriodAllowed.Checked = True
            If oCurrentCause.Absence_BetweenMax.HasValue Then Me.txtToPeriod.DateTime = oCurrentCause.Absence_BetweenMax.Value Else Me.txtToPeriod.DateTime = New Date(1899, 12, 30, 0, 0, 0)
            If oCurrentCause.Absence_BetweenMin.HasValue Then Me.txtFromPeriod.DateTime = oCurrentCause.Absence_BetweenMin.Value Else Me.txtFromPeriod.DateTime = New Date(1899, 12, 30, 0, 0, 0)

            If bDisableTemp Then
                Me.chkTimePeriodAllowed.Disabled = True
            End If

            bDisableTemp = (bDisable Or oCurrentCause.WorkingType)
            If Not oCurrentCause.Absence_DurationMax.HasValue And Not oCurrentCause.Absence_DurationMin.HasValue Then
                bDisableTemp = True
            End If

            If oCurrentCause.Absence_DurationMax.HasValue Or oCurrentCause.Absence_DurationMin.HasValue Then Me.chkTimeBetween.Checked = True
            If oCurrentCause.Absence_DurationMax.HasValue Then Me.txtMaxDuration.DateTime = oCurrentCause.Absence_DurationMax.Value Else Me.txtMaxDuration.DateTime = New Date(1899, 12, 30, 0, 0, 0)
            If oCurrentCause.Absence_DurationMin.HasValue Then Me.txtMinDuration.DateTime = oCurrentCause.Absence_DurationMin.Value Else Me.txtMinDuration.DateTime = New Date(1899, 12, 30, 0, 0, 0)

            'Pestaña "Redondeo"
            '==================
            Me.txtRoundBy.Text = oCurrentCause.RoundingBy.ToString("N0")

            Me.roOptValueUp.Checked = False
            Me.roOptValueDown.Checked = False
            Me.roOptValueAprox.Checked = False
            Select Case oCurrentCause.RoundingType
                Case eRoundingType.Round_UP
                    Me.roOptValueUp.Checked = True
                Case eRoundingType.Round_Down
                    Me.roOptValueDown.Checked = True
                Case eRoundingType.Round_Near
                    Me.roOptValueAprox.Checked = True
            End Select

            If oCurrentCause.RoundingByDailyScope = False Then ', "0", "1")
                Me.roOptIndivRound.Checked = True
                Me.roOptOneInDay.Checked = False
            Else
                Me.roOptIndivRound.Checked = False
                Me.roOptOneInDay.Checked = True
            End If

            'Pestaña "Visibilidad"
            '=====================
            Me.opVisibilityAll.Checked = False
            Me.opVisibilityNobody.Checked = False
            Me.opVisibilityCriteria.Checked = False
            If oCurrentCause.VisibilityPermissions = 0 Then
                Me.opVisibilityAll.Checked = True
            ElseIf oCurrentCause.VisibilityPermissions = 1 Then
                Me.opVisibilityNobody.Checked = True
            ElseIf oCurrentCause.VisibilityPermissions = 2 Then
                Me.opVisibilityCriteria.Checked = True
            End If

            If oCurrentCause.VisibilityPermissions = 2 AndAlso oCurrentCause.VisibilityCriteria IsNot Nothing AndAlso oCurrentCause.VisibilityCriteria.Count > 0 Then
                visibilityCombosIndex = Me.visibilityCriteria.SetFilterValue(oCurrentCause.VisibilityCriteria(0))
            End If

            'Pestaña "Fichajes"
            '=====================
            Me.txtInputCode.Text = oCurrentCause.ReaderInputcode

            Me.opPunchesAll.Checked = False
            Me.opPunchesNobody.Checked = False
            Me.opPunchesCriteria.Checked = False
            If oCurrentCause.InputPermissions = 0 Then
                Me.opPunchesAll.Checked = True
            ElseIf oCurrentCause.InputPermissions = 1 Then
                Me.opPunchesNobody.Checked = True
            ElseIf oCurrentCause.InputPermissions = 2 Then
                Me.opPunchesCriteria.Checked = True
            End If

            If oCurrentCause.InputPermissions = 2 AndAlso oCurrentCause.InputCriteria IsNot Nothing AndAlso oCurrentCause.InputCriteria.Count > 0 Then
                punchesCombosIndex = Me.punchesCriteria.SetFilterValue(oCurrentCause.InputCriteria(0))
            End If

            If oCurrentCause.MaxTimeToForecast > 0 Then
                Me.optCauseAutomaticValidation.Checked = True
                Me.txtCauseAutomaticValidation.DateTime = roTypes.Any2DateTime(roConversions.ConvertHoursToTime(oCurrentCause.MaxTimeToForecast))
            Else
                Me.optCauseAutomaticValidation.Checked = False
                Me.txtCauseAutomaticValidation.DateTime = New DateTime(1900, 1, 1, 0, 0, 0)
            End If

            'Pestaña "Incidencias"
            '=====================
            Me.opCheckIncidence.Checked = oCurrentCause.ApplyJustifyPeriod

            Dim disableSubItems As Boolean = IIf(oCurrentCause.ApplyJustifyPeriod = False, True, False)
            disableSubItems = disableSubItems Or bDisable

            If oCurrentCause.JustifyPeriodStart.HasValue Then
                Me.mskJustifyPeriodsStart.Text = roConversions.ConvertMinutesToTime(oCurrentCause.JustifyPeriodStart.Value)
            Else
                Me.mskJustifyPeriodsStart.Text = "00:00"
            End If

            If oCurrentCause.JustifyPeriodEnd.HasValue Then
                Me.mskJustifyPeriodsEnd.Text = roConversions.ConvertMinutesToTime(oCurrentCause.JustifyPeriodEnd.Value)
            Else
                Me.mskJustifyPeriodsEnd.Text = "00:00"
            End If

            If oCurrentCause.JustifyPeriodType.HasValue AndAlso oCurrentCause.JustifyPeriodType.Value = eJustifyPeriodType.JustifyPeriod Then
                Me.opJustifyPeriod.Checked = True
                Me.opJustifyNothing.Checked = False
            Else
                Me.opJustifyPeriod.Checked = False
                Me.opJustifyNothing.Checked = True
            End If

            If disableSubItems Then
                Me.opJustifyNothing.Enabled = False
                Me.opJustifyPeriod.Enabled = False
            End If

            If roTypes.Any2Boolean(HelperSession.AdvancedParametersCache("CustomCauseColorEnabled")) Then
                divColorCause.Style("display") = ""
            End If

            strJSONgrids = CreateDocumentsGridJson(oCurrentCause)

            Me.tbAvailableOverWorkRequests.Value = String.Empty
            Me.tbAvailableHolidayRequests.Value = String.Empty
            Me.tbAvailableAbsenceRequests.Value = String.Empty

            If Me.hdHasLeaveDocuments.Value = "1" Then
                Me.roOptAllRequestTypes.Checked = False
                Me.roOptLeaveRequest.Checked = True
                Me.roOptCustomRequestList.Checked = False
                Me.roOptNoneRequestList.Checked = False
            Else
                If oCurrentCause.RequestAvailability.Trim = "-1" Then
                    Me.roOptAllRequestTypes.Checked = True
                    Me.roOptLeaveRequest.Checked = False
                    Me.roOptCustomRequestList.Checked = False
                    Me.roOptNoneRequestList.Checked = False
                ElseIf oCurrentCause.RequestAvailability.Trim = "-2" Then
                    Me.roOptAllRequestTypes.Checked = False
                    Me.roOptLeaveRequest.Checked = False
                    Me.roOptCustomRequestList.Checked = False
                    Me.roOptNoneRequestList.Checked = True
                Else
                    Me.roOptAllRequestTypes.Checked = False
                    Me.roOptLeaveRequest.Checked = False
                    Me.roOptCustomRequestList.Checked = True
                    Me.roOptNoneRequestList.Checked = False

                    If Me.optHoursProductive.Checked Then
                        Me.tbAvailableOverWorkRequests.Value = oCurrentCause.RequestAvailability
                    Else
                        If Me.optHoursNonProductiveHoliday.Checked Then
                            Me.tbAvailableHolidayRequests.Value = oCurrentCause.RequestAvailability
                        Else
                            Me.tbAvailableAbsenceRequests.Value = oCurrentCause.RequestAvailability
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            Dim rError As New roJSON.JSONError(True, ex.Message.ToString & " " & ex.StackTrace)
            Response.Write(rError.toJSON)
        Finally
            ASPxCallbackPanelContenido.JSProperties.Add("cpActionRO", "GETCAUSE")
            ASPxCallbackPanelContenido.JSProperties.Add("cpNameRO", oCurrentCause.Name)
            ASPxCallbackPanelContenido.JSProperties.Add("cpDocumentsGridRO", strJSONgrids)
            ASPxCallbackPanelContenido.JSProperties.Add("cpVisibilityIndexRO", visibilityCombosIndex)
            ASPxCallbackPanelContenido.JSProperties.Add("cpPunchesIndexRO", punchesCombosIndex)
            ASPxCallbackPanelContenido.JSProperties.Add("cpResultRO", "OK")
            ASPxCallbackPanelContenido.JSProperties.Add("cpIsNewRO", False)
        End Try
    End Sub

    Private Function CreateDocumentsGridJson(ByVal oCause As roCause) As String
        Dim strFieldsGrid As String = ""

        Me.hdHasLeaveDocuments.Value = "0"

        If Not oCause.Documents Is Nothing AndAlso oCause.Documents.Count > 0 Then
            Dim strTexto As String
            For Each oCauseDocument As roCauseDocument In oCause.Documents

                If DocumentsServiceMethods.GetDocumentTemplateById(Me.Page, oCauseDocument.IDdocument, False).Scope = Robotics.Base.DTOs.DocumentScope.LeaveOrPermission Then
                    Me.hdHasLeaveDocuments.Value = "1"
                End If

                Select Case oCauseDocument.TypeRequest
                    Case TypeRequestEnum.AtBegin
                        strTexto = Me.Language.Translate("DocumentAbsences.AtBegin", Me.DefaultScope)

                    Case TypeRequestEnum.AtEnd
                        strTexto = Me.Language.Translate("DocumentAbsences.AtEnd", Me.DefaultScope)

                    Case TypeRequestEnum.EveryDays
                        Dim oParams As New Generic.List(Of String)(1)
                        oParams.Add(oCauseDocument.NumberItems)
                        strTexto = Me.Language.Translate("DocumentAbsences.EveryDays", Me.DefaultScope, oParams)

                    Case TypeRequestEnum.EveryWeeks
                        Dim oParams As New Generic.List(Of String)(1)
                        oParams.Add(oCauseDocument.NumberItems)
                        strTexto = Me.Language.Translate("DocumentAbsences.EveryWeeks", Me.DefaultScope, oParams)

                    Case TypeRequestEnum.EveryMonths
                        Dim oParams As New Generic.List(Of String)(1)
                        oParams.Add(oCauseDocument.NumberItems)
                        strTexto = Me.Language.Translate("DocumentAbsences.EveryMonths", Me.DefaultScope, oParams)

                    Case TypeRequestEnum.EveryFlexible1
                        Dim oParams As New Generic.List(Of String)(2)
                        oParams.Add(oCauseDocument.NumberItems)
                        oParams.Add(IIf(oCauseDocument.FlexibleWhen = 0, Me.Language.Translate("Flexible1.Ini", "DocumentAbsences"), Me.Language.Translate("Flexible1.Fin", "DocumentAbsences")))
                        strTexto = Me.Language.Translate("DocumentAbsences.EveryFlexible1", Me.DefaultScope, oParams)

                    Case TypeRequestEnum.EveryFlexible2
                        Dim oParams As New Generic.List(Of String)(4)
                        oParams.Add(oCauseDocument.NumberItems)
                        Select Case oCauseDocument.FlexibleWhen2
                            Case 0
                                oParams.Add(Me.Language.Translate("Flexible2.Days", "DocumentAbsences"))
                            Case 1
                                oParams.Add(Me.Language.Translate("Flexible2.Weeks", "DocumentAbsences"))
                            Case 2
                                oParams.Add(Me.Language.Translate("Flexible2.Months", "DocumentAbsences"))
                        End Select
                        oParams.Add(oCauseDocument.NumberItems2)
                        oParams.Add(IIf(oCauseDocument.FlexibleWhen = 0, Me.Language.Translate("Flexible1.Ini", "DocumentAbsences"), Me.Language.Translate("Flexible1.Fin", "DocumentAbsences")))
                        strTexto = Me.Language.Translate("DocumentAbsences.EveryFlexible2", Me.DefaultScope, oParams)

                    Case Else
                        strTexto = String.Empty
                End Select

                If oCauseDocument.IDLabAgree = 0 Then
                    oCauseDocument.LabAgreeName = Me.Language.Translate("AnyLabAgree", DefaultScope)
                End If

                strFieldsGrid &= "{fields:[{ field: 'idDocumentTrace', value: '" & oCauseDocument.ID & "' }, " &
                                 "{ field: 'idCause', value: '" & oCauseDocument.IDCause & "' }, " &
                                 "{ field: 'idLabAgree', value: '" & oCauseDocument.IDLabAgree & "' }, " &
                                 "{ field: 'idDocument', value: '" & oCauseDocument.IDdocument & "' }, " &
                                 "{ field: 'typeRequest', value: '" & oCauseDocument.TypeRequest & "' }, " &
                                 "{ field: 'numItems', value: '" & oCauseDocument.NumberItems & "' }, " &
                                 "{ field: 'numItems2', value: '" & oCauseDocument.NumberItems2 & "' }, " &
                                 "{ field: 'flexibleWhen', value: '" & oCauseDocument.FlexibleWhen & "' }, " &
                                 "{ field: 'flexibleWhen2', value: '" & oCauseDocument.FlexibleWhen2 & "' }, " &
                                 "{ field: 'nameDocument', value: '" & oCauseDocument.DocumentName.Replace("'", "\'") & "' }, " &
                                 "{ field: 'nameInterval', value: '" & strTexto & "' }, " &
                                  "{ field: 'nameLabAgree', value: '" & oCauseDocument.LabAgreeName.Replace("'", "\'") & " '}]},"
            Next
            If strFieldsGrid <> String.Empty Then
                strFieldsGrid = "[" & strFieldsGrid.Substring(0, strFieldsGrid.Length - 1) & "]"
            End If

        End If

        Return strFieldsGrid
    End Function

    Private Sub SaveCause(ByVal oParameters As CausesCallbackRequest)
        Dim strError As String = ""
        Dim rError As roJSON.JSONError = Nothing
        Dim strMessage As JSONMsgBox = Nothing

        Dim oCurrentCause As roCause = Nothing
        Try
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

            If bolIsNew Then
                oCurrentCause = New roCause()
                oCurrentCause.ID = -1
            Else
                oCurrentCause = CausesServiceMethods.GetCauseByID(Me, oParameters.ID, False)
            End If

            If oCurrentCause Is Nothing Then Exit Sub

            oCurrentCause.Name = txtName.Text
            oCurrentCause.Description = txtDescription.Text
            oCurrentCause.Export = txtFactor.Text
            oCurrentCause.ShortName = txtShortName.Text
            oCurrentCause.ReaderInputcode = roTypes.Any2Integer(Me.txtInputCode.Text)
            oCurrentCause.BusinessCenter = txtBusinessGroup.Text
            oCurrentCause.Color = Drawing.ColorTranslator.ToWin32(colorCause.Color)

            If oPermissionCostCenter > Permission.Read Then
                oCurrentCause.CostFactor = CDbl(txtCostFactor.Text.Replace(".", HelperWeb.GetDecimalDigitFormat))
            End If

            oCurrentCause.WorkingType = optProductiveHoursOnCC.Checked

            Select Case cmbCauseType.SelectedItem.Value
                Case 0 'horas
                    oCurrentCause.DayType = False
                    oCurrentCause.CustomType = False
                Case 1 'dias
                    oCurrentCause.DayType = True
                    oCurrentCause.CustomType = False
                Case 2 'custom
                    oCurrentCause.DayType = False
                    oCurrentCause.CustomType = True
            End Select

            oCurrentCause.IsHoliday = False
            oCurrentCause.ExternalWork = False
            oCurrentCause.IDConceptBalance = 0

            If oCurrentCause.WorkingType = False AndAlso oCurrentCause.DayType = False AndAlso oCurrentCause.CustomType = False Then
                If optHoursProductive.Checked AndAlso optExternalProductiveHours.Checked Then
                    oCurrentCause.ExternalWork = True
                ElseIf optHoursNonProductive.Checked AndAlso optHoursNonProductiveHoliday.Checked Then
                    oCurrentCause.IsHoliday = True
                    If cmbConceptBalance.SelectedItem IsNot Nothing Then
                        oCurrentCause.IDConceptBalance = roTypes.Any2Integer(cmbConceptBalance.SelectedItem.Value)
                    End If
                ElseIf optHoursNonProductive.Checked AndAlso optHoursNonProductiveCause.Checked Then
                    If cmbAbsenceConceptBalance.SelectedItem IsNot Nothing Then
                        oCurrentCause.IDConceptBalance = roTypes.Any2Integer(cmbAbsenceConceptBalance.SelectedItem.Value)
                    End If
                    If cmbAbsenceConceptBalanceDays.SelectedItem IsNot Nothing Then
                        oCurrentCause.ApplyWorkDaysOnConcept = roTypes.Any2Boolean(Me.cmbAbsenceConceptBalanceDays.SelectedItem.Value)
                    Else
                        oCurrentCause.ApplyWorkDaysOnConcept = False
                    End If
                End If

            ElseIf oCurrentCause.WorkingType = True Then
                If optHoursProductive.Checked AndAlso optExternalProductiveHours.Checked Then
                    oCurrentCause.ExternalWork = True
                Else
                    oCurrentCause.ExternalWork = False
                    If cmbConceptBalanceProductive.SelectedItem IsNot Nothing Then
                        oCurrentCause.IDConceptBalance = roTypes.Any2Integer(cmbConceptBalanceProductive.SelectedItem.Value)
                    End If
                    If cmbConceptBalanceProductiveDays.SelectedItem IsNot Nothing Then
                        oCurrentCause.ApplyWorkDaysOnConcept = roTypes.Any2Boolean(Me.cmbConceptBalanceProductiveDays.SelectedItem.Value)
                    Else
                        oCurrentCause.ApplyWorkDaysOnConcept = False
                    End If
                End If
            Else

            End If

            oCurrentCause.StartsProgrammedAbsence = Me.optCauseAbsences.Checked
            oCurrentCause.MaxProgrammedAbsence = roTypes.Any2Integer(Me.txtMaxsDaysAbsence.Text)
            oCurrentCause.PunchCloseProgrammedAbsence = Me.optCauseCloseAbsences.Checked

            If (Me.optAbsenceMandatoryDays.Checked) Then
                oCurrentCause.AbsenceMandatoryDays = Me.txtMandatoryDaysOnPortalRequest.Text
            Else
                oCurrentCause.AbsenceMandatoryDays = -1
            End If

            If oCurrentCause.DayType AndAlso Me.optDaysFromCause.Checked AndAlso cmbDayCauses.SelectedItem IsNot Nothing Then
                oCurrentCause.AutomaticEquivalenceIDCause = cmbDayCauses.SelectedItem.Value

                oCurrentCause.AutomaticEquivalenceCriteria = New roAutomaticEquivalenceCriteria
                If Me.ckCauseFromWorkingHours.Checked Then
                    oCurrentCause.AutomaticEquivalenceType = eAutomaticEquivalenceType.ExpectedWorkingHoursType
                    oCurrentCause.AutomaticEquivalenceCriteria.AutomaticEquivalenceType = eAutomaticEquivalenceType.ExpectedWorkingHoursType
                    oCurrentCause.AutomaticEquivalenceCriteria.FactorValue = 0
                    oCurrentCause.AutomaticEquivalenceCriteria.UserField = Nothing
                ElseIf Me.ckCauseFromFactor.Checked Then
                    oCurrentCause.AutomaticEquivalenceType = eAutomaticEquivalenceType.DirectValueType
                    oCurrentCause.AutomaticEquivalenceCriteria.AutomaticEquivalenceType = eAutomaticEquivalenceType.DirectValueType
                    oCurrentCause.AutomaticEquivalenceCriteria.FactorValue = Me.txtCauseFromFactor.Value
                    oCurrentCause.AutomaticEquivalenceCriteria.UserField = Nothing

                ElseIf Me.ckCauseFromUserField.Checked Then
                    oCurrentCause.AutomaticEquivalenceType = eAutomaticEquivalenceType.FieldType
                    oCurrentCause.AutomaticEquivalenceCriteria.AutomaticEquivalenceType = eAutomaticEquivalenceType.FieldType
                    oCurrentCause.AutomaticEquivalenceCriteria.FactorValue = 0
                    oCurrentCause.AutomaticEquivalenceCriteria.UserField = New roUserField()
                    oCurrentCause.AutomaticEquivalenceCriteria.UserField.FieldName = Me.cmbCauseFromUserField.Value
                End If
            Else
                oCurrentCause.AutomaticEquivalenceType = eAutomaticEquivalenceType.DeactivatedType
                oCurrentCause.AutomaticEquivalenceIDCause = 0
                oCurrentCause.AutomaticEquivalenceCriteria = New roAutomaticEquivalenceCriteria
                oCurrentCause.AutomaticEquivalenceCriteria.AutomaticEquivalenceType = eAutomaticEquivalenceType.DeactivatedType
                oCurrentCause.AutomaticEquivalenceCriteria.FactorValue = 0
                oCurrentCause.AutomaticEquivalenceCriteria.UserField = Nothing
            End If

            oCurrentCause.RoundingBy = roTypes.Any2Double(txtRoundBy.Text)
            If oCurrentCause.RoundingBy <= 0 Then oCurrentCause.RoundingBy = 1

            If roOptValueUp.Checked Then
                oCurrentCause.RoundingType = eRoundingType.Round_UP
            ElseIf roOptValueDown.Checked Then
                oCurrentCause.RoundingType = eRoundingType.Round_Down
            ElseIf roOptValueAprox.Checked Then
                oCurrentCause.RoundingType = eRoundingType.Round_Near
            End If

            oCurrentCause.RoundingByDailyScope = roOptOneInDay.Checked

            If opVisibilityAll.Checked Then
                oCurrentCause.VisibilityPermissions = 0
            ElseIf opVisibilityNobody.Checked Then
                oCurrentCause.VisibilityPermissions = 1
            ElseIf opVisibilityCriteria.Checked Then
                oCurrentCause.VisibilityPermissions = 2
            End If

            oCurrentCause.VisibilityCriteria = Nothing
            If oCurrentCause.VisibilityPermissions = 2 Then
                If visibilityCriteria.IsValidFilter Then
                    Dim xList As New Generic.List(Of roUserFieldCondition)
                    xList.Add(visibilityCriteria.OConditionValue)
                    oCurrentCause.VisibilityCriteria = xList
                Else
                    strError = "KO"
                    rError = New roJSON.JSONError(True, Me.Language.Translate("Error.NoCorrectVisibilityFilter", DefaultScope))
                End If
            End If

            If opPunchesAll.Checked Then
                oCurrentCause.InputPermissions = 0
            ElseIf opPunchesNobody.Checked Then
                oCurrentCause.InputPermissions = 1
            ElseIf opPunchesCriteria.Checked Then
                oCurrentCause.InputPermissions = 2
            End If

            oCurrentCause.InputCriteria = Nothing
            If oCurrentCause.InputPermissions = 2 Then
                Dim xList As New Generic.List(Of roUserFieldCondition)
                xList.Add(punchesCriteria.OConditionValue)
                oCurrentCause.InputCriteria = xList
            End If

            oCurrentCause.ApplyJustifyPeriod = Me.opCheckIncidence.Checked

            oCurrentCause.JustifyPeriodStart = roConversions.ConvertTimeToMinutes(Me.mskJustifyPeriodsStart.Text)
            oCurrentCause.JustifyPeriodEnd = roConversions.ConvertTimeToMinutes(Me.mskJustifyPeriodsEnd.Text)
            If Me.opJustifyPeriod.Checked Then
                oCurrentCause.JustifyPeriodType = eJustifyPeriodType.JustifyPeriod
            Else
                oCurrentCause.JustifyPeriodType = eJustifyPeriodType.DontJustify
            End If

            If Me.chkMaxDays.Checked Then
                oCurrentCause.Absence_MaxDays = roTypes.Any2Integer(Me.txtDaysDuration.Text)
            Else
                oCurrentCause.Absence_MaxDays = 0
            End If

            If chkTimePeriodAllowed.Checked = True Then
                oCurrentCause.Absence_BetweenMax = txtToPeriod.DateTime
                oCurrentCause.Absence_BetweenMin = txtFromPeriod.DateTime
            Else
                oCurrentCause.Absence_BetweenMax = Nothing
                oCurrentCause.Absence_BetweenMin = Nothing
            End If

            If chkTimeBetween.Checked = True Then
                oCurrentCause.Absence_DurationMax = txtMaxDuration.DateTime
                oCurrentCause.Absence_DurationMin = txtMinDuration.DateTime
            Else
                oCurrentCause.Absence_DurationMax = Nothing
                oCurrentCause.Absence_DurationMin = Nothing
            End If

            If oParameters.gridDocuments IsNot Nothing AndAlso oParameters.gridDocuments.Length > 0 Then
                Dim lstCauseDocuments As New Generic.List(Of roCauseDocument)(oParameters.gridDocuments.Length)
                Dim oCauseDocument As roCauseDocument

                For Each docGrid As GridDocument In oParameters.gridDocuments
                    oCauseDocument = New roCauseDocument() With {.ID = docGrid.IdDocumentTrace, .IDCause = docGrid.IdCause, .IDLabAgree = docGrid.IdLabAgree,
                                                                 .IDdocument = docGrid.IdDocument, .TypeRequest = docGrid.TypeRequest, .NumberItems = docGrid.NumItems,
                                                                 .NumberItems2 = docGrid.NumItems2, .FlexibleWhen = docGrid.FlexibleWhen, .FlexibleWhen2 = docGrid.FlexibleWhen2}
                    lstCauseDocuments.Add(oCauseDocument)
                Next
                oCurrentCause.TraceDocumentsAbsences = True
                oCurrentCause.Documents = lstCauseDocuments
            Else
                oCurrentCause.TraceDocumentsAbsences = False
                oCurrentCause.Documents = Nothing
            End If

            If oCurrentCause.InputPermissions <> 1 Then
                oCurrentCause.AllowInputFromReader = True
            Else
                oCurrentCause.AllowInputFromReader = False
                oCurrentCause.ReaderInputcode = 0
            End If

            If oCurrentCause.RoundingType = eRoundingType.Round_UP Then
                oCurrentCause.StringRoundingType = "+"
            ElseIf oCurrentCause.RoundingType = eRoundingType.Round_Down Then
                oCurrentCause.StringRoundingType = "-"
            Else
                oCurrentCause.StringRoundingType = "~"
            End If

            If Me.optCauseAutomaticValidation.Checked Then
                oCurrentCause.MaxTimeToForecast = roConversions.ConvertTimeToHours(Me.txtCauseAutomaticValidation.DateTime.ToShortTimeString)
            Else
                oCurrentCause.MaxTimeToForecast = 0
            End If

            If Me.roOptAllRequestTypes.Checked OrElse Me.roOptLeaveRequest.Checked Then
                oCurrentCause.RequestAvailability = "-1"
            ElseIf Me.roOptNoneRequestList.Checked Then
                oCurrentCause.RequestAvailability = "-2"
            ElseIf Me.roOptCustomRequestList.Checked Then
                If Me.optHoursProductive.Checked Then
                    oCurrentCause.RequestAvailability = Me.tbAvailableOverWorkRequests.Value
                Else
                    If Me.optHoursNonProductiveHoliday.Checked Then
                        oCurrentCause.RequestAvailability = Me.tbAvailableHolidayRequests.Value
                    Else
                        oCurrentCause.RequestAvailability = Me.tbAvailableAbsenceRequests.Value
                    End If
                End If
            End If

            If cmbRequestCategory.SelectedItem Is Nothing Then
                oCurrentCause.IDCategory = 6
            Else
                oCurrentCause.IDCategory = roTypes.Any2Integer(cmbRequestCategory.SelectedItem.Value)
            End If

            If Me.chkMinLevel.Checked Then
                oCurrentCause.MinLevelOfAuthority = roTypes.Any2Integer(cmbMinLevel.SelectedItem.Value)
            Else
                oCurrentCause.MinLevelOfAuthority = 11
            End If
            If Me.chkAtLevel.Checked Then
                oCurrentCause.ApprovedAtLevel = roTypes.Any2Integer(cmbAtLevel.SelectedItem.Value)
            Else
                oCurrentCause.ApprovedAtLevel = 1
            End If

            If strError = String.Empty Then
                If CausesServiceMethods.SaveCause(Me, oCurrentCause, True) = True Then
                    rError = New roJSON.JSONError(False, "OK:" & oCurrentCause.ID)
                    strError = "OK"
                Else
                    rError = New roJSON.JSONError(True, CausesServiceMethods.LastErrorText)
                    strError = "KO"
                End If
            End If
        Catch ex As Exception
            rError = New roJSON.JSONError(True, ex.Message.ToString & " " & ex.StackTrace)
            strError = "KO"
        Finally
            If strError = "KO" Then
                LoadCause(oParameters, oCurrentCause)
                ASPxCallbackPanelContenido.JSProperties("cpActionRO") = "SAVECAUSE"
                If strError = "KO" Then
                    ASPxCallbackPanelContenido.JSProperties("cpResultRO") = "KO"
                    ASPxCallbackPanelContenido.JSProperties("cpMessageRO") = rError
                Else
                    ASPxCallbackPanelContenido.JSProperties("cpActionRO") = "SAVECAUSE"
                    ASPxCallbackPanelContenido.JSProperties("cpMessageRO") = strMessage
                End If
            Else
                Dim treePath As String = "/source/" & oCurrentCause.ID
                HelperWeb.roSelector_SetSelection(oCurrentCause.ID.ToString, treePath, "ctl00_contentMainBody_roTreesCauses")
                LoadCause(oParameters)
                ASPxCallbackPanelContenido.JSProperties("cpIsNewRO") = True
                ASPxCallbackPanelContenido.JSProperties("cpActionRO") = "SAVECAUSE"
                ASPxCallbackPanelContenido.JSProperties.Add("cpNewIdRO", oParameters.ID)
            End If
        End Try

    End Sub

    Private Sub ProcessCauseSelectedTabVisible(ByVal oParameters As CausesCallbackRequest)
        'Mostra el TAB seleccionat

        Me.div00.Style("display") = "none"
        Me.div01.Style("display") = "none"
        Me.div02.Style("display") = "none"
        Me.div03.Style("display") = "none"
        Me.div04.Style("display") = "none"
        Me.div05.Style("display") = "none"

        Select Case oParameters.aTab
            Case 0
                Me.div00.Style("display") = ""
            Case 1
                Me.div01.Style("display") = ""
            Case 2
                Me.div02.Style("display") = ""
            Case 3
                Me.div03.Style("display") = ""
            Case 4
                Me.div04.Style("display") = ""
            Case 5
                Me.div05.Style("display") = ""
        End Select

        If roTypes.Any2Integer(HelperSession.AdvancedParametersCache("Causes.AllowIncidenceConfiguration")) = 1 Then
            Me.incidenceConfigurationZone.Style("display") = ""
        End If
    End Sub

End Class