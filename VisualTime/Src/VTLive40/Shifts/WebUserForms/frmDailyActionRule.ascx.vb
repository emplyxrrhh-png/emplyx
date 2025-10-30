Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class WebUserForms_frmDailyActionRule
    Inherits UserControlBase

    Public Property Instance() As String
        Get
            If ViewState("roDailyRuleActionInstance") Is Nothing Then
                Return ""
            Else
                Return ViewState("roDailyRuleActionInstance")
            End If
        End Get
        Set(ByVal value As String)
            ViewState("roDailyRuleActionInstance") = value
        End Set
    End Property

    Private Sub WebUserForms_frmDailyActionRule_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then

            cmbActions.ClientInstanceName = "cmbActionsClient_" & Me.Instance
            cmbActions.ClientSideEvents.SelectedIndexChanged = "function(s,e){ showActionConfiguration(" & Me.Instance & ",s);}"

            cmbPlusCause.ClientInstanceName = "cmbPlusCauseClient_" & Me.Instance
            cmbPlusCause.ClientSideEvents.SelectedIndexChanged = "function(s,e){ showCauseValueByType(" & Me.Instance & ",s);}"

            cmbPlusActions.ClientInstanceName = "cmbPlusActionsClient_" & Me.Instance
            cmbPlusActions.ClientSideEvents.SelectedIndexChanged = "function(s,e){ showPlusConfiguration(" & Me.Instance & ",s);}"

            cmbPlusResultActions.ClientInstanceName = "cmbPlusResultActionsClient_" & Me.Instance
            cmbPlusResultActions.ClientSideEvents.SelectedIndexChanged = "function(s,e){ showPlusResultConfiguration(" & Me.Instance & ",s);}"

            cmbCarryOverAction.ClientInstanceName = "cmbCarryOverActionClient_" & Me.Instance
            cmbCarryOverAction.ClientSideEvents.SelectedIndexChanged = "function(s,e){ showCarryOverConfiguration(" & Me.Instance & ",s);}"

            cmbCarryOverActionResult.ClientInstanceName = "cmbCarryOverActionResultClient_" & Me.Instance
            cmbCarryOverActionResult.ClientSideEvents.SelectedIndexChanged = "function(s,e){ showCarryOverResultConfiguration(" & Me.Instance & ",s);}"

            cmbCarryOverSingleAction.ClientInstanceName = "cmbCarryOverSingleActionClient_" & Me.Instance
            cmbCarryOverSingleAction.ClientSideEvents.SelectedIndexChanged = "function(s,e){ showCarryOverSingleConfiguration(" & Me.Instance & ",s);}"

            cmbCarryOverSingleActionCauses.ClientInstanceName = "cmbCarryOverSingleActionCausesClient_" & Me.Instance
            cmbCarryOverSingleActionCauses.ClientSideEvents.SelectedIndexChanged = "function(s,e){ showCarryOverSingleCausesConfiguration(" & Me.Instance & ",s);}"

            cmbCarryOverCauseFrom.ClientInstanceName = "cmbCarryOverCauseFromClient_" & Me.Instance
            cmbCarryOverCauseTo.ClientInstanceName = "cmbCarryOverCauseToClient_" & Me.Instance

            txtCarryOverValue.ClientInstanceName = "txtCarryOverValueClient_" & Me.Instance
            cmbCarryOverConditionPart.ClientInstanceName = "cmbCarryOverConditionPartClient_" & Me.Instance
            cmbCarryOverConditionNumber.ClientInstanceName = "cmbCarryOverConditionNumberClient_" & Me.Instance
            cmbCauseUFieldCarryOver.ClientInstanceName = "cmbCauseUFieldCarryOverClient_" & Me.Instance

            txtCarryOverResultValue.ClientInstanceName = "txtCarryOverResultValueClient_" & Me.Instance
            cmbCarryOverConditionPartResult.ClientInstanceName = "cmbCarryOverConditionPartResultClient_" & Me.Instance
            cmbCarryOverConditionNumberResult.ClientInstanceName = "cmbCarryOverConditionNumberResultClient_" & Me.Instance
            cmbCauseUFieldCarryOverResult.ClientInstanceName = "cmbCauseUFieldCarryOverResultClient_" & Me.Instance

            cmbPlusSign.ClientInstanceName = "cmbPlusSignClient_" & Me.Instance

            txtPlusValueTime.ClientInstanceName = "txtPlusValueTimeClient_" & Me.Instance
            txtPlusValueNumber.ClientInstanceName = "txtPlusValueNumberClient_" & Me.Instance
            cmbPlusConditionPart.ClientInstanceName = "cmbPlusConditionPartClient_" & Me.Instance
            cmbPlusConditionNumber.ClientInstanceName = "cmbPlusConditionNumberClient_" & Me.Instance
            cmbCauseUFieldPlus.ClientInstanceName = "cmbCauseUFieldPlusClient_" & Me.Instance

            txtPlusValueResultTime.ClientInstanceName = "txtPlusValueResultTimeClient_" & Me.Instance
            txtPlusValueResultNumber.ClientInstanceName = "txtPlusValueResultNumberClient_" & Me.Instance
            cmbPlusConditionPartResult.ClientInstanceName = "cmbPlusConditionPartResultClient_" & Me.Instance
            cmbPlusConditionNumberResult.ClientInstanceName = "cmbPlusConditionNumberResultClient_" & Me.Instance
            cmbCauseUFieldPlusResult.ClientInstanceName = "cmbCauseUFieldPlusResultClient_" & Me.Instance

            cmbCauseActionAdd.ClientInstanceName = "cmbCauseActionAddClient_" & Me.Instance
            cmbCauseActionAdd2.ClientInstanceName = "cmbCauseActionAdd2Client_" & Me.Instance

            ibtNewCauseActionOK.Src = Me.ResolveUrl("~/Base/Images/Grid/save.png")
            ibtNewCauseActionOK.Attributes("onclick") = "AddCauseActionOK(" & Me.Instance & ",0,'divNewCauseActions_" & Me.Instance & "'); return false;"
            ibtNewCauseActionCancel.Src = Me.ResolveUrl("~/Base/Images/Grid/cancel.png")
            ibtNewCauseActionCancel.Attributes("onclick") = "ShowWindow('divNewCauseActions_" & Me.Instance & "',false);"

            imgAddListValueAction.Src = Me.ResolveUrl("~/Base/Images/Grid/add.png")
            imgAddListValueAction.Attributes("onclick") = "ShowWindow('divNewCauseActions_" & Me.Instance & "',true);"
            imgRemoveListValueAction.Src = Me.ResolveUrl("~/Base/Images/Grid/remove.png")
            imgRemoveListValueAction.Attributes("onclick") = "RemoveListValueAction(" & Me.Instance & ", 0, 'divNewCauseActions_" & Me.Instance & "',true);"

            Me.loadCombosAction()
        End If
    End Sub

    Public Sub loadCombosAction()
        Try

            Dim dTbl As DataTable = CausesServiceMethods.GetCausesShortList(Me.Page)
            Dim tbUserFields As DataTable = API.UserFieldServiceMethods.GetUserFields(Me.Page, Types.EmployeeField, "FieldType IN(1,3,4) AND Used=1", False)

            cmbActions.Items.Clear()
            cmbActions.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ActionTitle.CarryOver", Me.DefaultScope), CInt(RuleAction.CarryOver)))
            cmbActions.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ActionTitle.Plus", Me.DefaultScope), CInt(RuleAction.Plus)))
            cmbActions.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ActionTitle.CarryOverSingle", Me.DefaultScope), CInt(RuleAction.CarryOverSingle)))
            cmbActions.SelectedIndex = 0

            cmbCauseActionAdd.Items.Clear()
            cmbCauseActionAdd2.Items.Clear()

            For Each dRow As DataRow In dTbl.Rows
                cmbCauseActionAdd.Items.Add(New DevExpress.Web.ListEditItem(dRow("Name"), dRow("ID")))
                cmbCauseActionAdd2.Items.Add(New DevExpress.Web.ListEditItem(dRow("Name"), dRow("ID")))
                cmbCarryOverSingleActionCauses.Items.Add(New DevExpress.Web.ListEditItem(dRow("Name"), dRow("ID")))
            Next
            cmbCauseActionAdd.SelectedIndex = 0
            cmbCauseActionAdd2.SelectedIndex = 1
            cmbCarryOverSingleActionCauses.SelectedIndex = 0

            cmbCarryOverSingleAction.Items.Clear()
            cmbCarryOverSingleAction.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ActionTitle.SingleAction", Me.DefaultScope), 0))
            cmbCarryOverSingleAction.SelectedIndex = 0

            Dim fromStr As String = Me.Language.Translate("ActionTitle.FromCause", Me.DefaultScope)
            Dim toStr As String = Me.Language.Translate("ActionTitle.ToCause", Me.DefaultScope)

            cmbPlusCause.Items.Clear()
            cmbCarryOverCauseFrom.Items.Clear()
            cmbCarryOverCauseTo.Items.Clear()
            For Each dRow As DataRow In dTbl.Rows

                Dim dType As Integer = 0

                If Robotics.VTBase.roTypes.Any2Boolean(dRow("CustomType")) OrElse Robotics.VTBase.roTypes.Any2Boolean(dRow("DayType")) Then dType = 1

                'No permitimos elegir la justificación del tipo Horas Teóricas
                If dRow("ID") <> 4 Then
                    cmbPlusCause.Items.Add(New DevExpress.Web.ListEditItem(dRow("Name"), dRow("ID") & "_" & dType))
                End If

                cmbCarryOverCauseFrom.Items.Add(New DevExpress.Web.ListEditItem(fromStr & "  " & dRow("Name"), dRow("ID")))
                cmbCarryOverCauseTo.Items.Add(New DevExpress.Web.ListEditItem(toStr & "  " & dRow("Name"), dRow("ID")))
            Next
            cmbPlusCause.SelectedIndex = 0
            cmbCarryOverCauseFrom.SelectedIndex = 0
            cmbCarryOverCauseTo.SelectedIndex = 0

            cmbCarryOverAction.Items.Clear()
            cmbCarryOverAction.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate(System.Enum.GetName(GetType(ValueType), ValueType.DirectValue).ToString & "_CarryOver", DefaultScope), CInt(ValueType.DirectValue)))
            cmbCarryOverAction.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate(System.Enum.GetName(GetType(ValueType), ValueType.UserField).ToString & "_CarryOver", DefaultScope), CInt(ValueType.UserField)))
            cmbCarryOverAction.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate(System.Enum.GetName(GetType(ValueType), ValueType.IDCause).ToString & "_CarryOver", DefaultScope), CInt(ValueType.IDCause)))
            cmbCarryOverAction.SelectedIndex = 0

            cmbCarryOverActionResult.Items.Clear()
            cmbCarryOverActionResult.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate(System.Enum.GetName(GetType(ValueType), ValueType.DirectValue).ToString & "_CarryOverResult", DefaultScope), CInt(ValueType.DirectValue)))
            cmbCarryOverActionResult.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate(System.Enum.GetName(GetType(ValueType), ValueType.UserField).ToString & "_CarryOverResult", DefaultScope), CInt(ValueType.UserField)))
            cmbCarryOverActionResult.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate(System.Enum.GetName(GetType(ValueType), ValueType.IDCause).ToString & "_CarryOverResult", DefaultScope), CInt(ValueType.IDCause)))
            cmbCarryOverActionResult.SelectedIndex = 0

            cmbCarryOverConditionPart.Items.Clear()
            cmbCarryOverConditionPartResult.Items.Clear()
            cmbCarryOverConditionPart.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ActionTitle.ConditionPart1", Me.DefaultScope), CInt(RulePart.Part1)))
            cmbCarryOverConditionPart.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ActionTitle.ConditionPart2", Me.DefaultScope), CInt(RulePart.Part2)))
            cmbCarryOverConditionPartResult.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ActionTitle.ConditionPart1Result", Me.DefaultScope), CInt(RulePart.Part1)))
            cmbCarryOverConditionPartResult.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ActionTitle.ConditionPart2Result", Me.DefaultScope), CInt(RulePart.Part2)))
            cmbCarryOverConditionPart.SelectedIndex = 0
            cmbCarryOverConditionPartResult.SelectedIndex = 0

            cmbCarryOverConditionNumber.Items.Clear()
            cmbCarryOverConditionNumberResult.Items.Clear()
            cmbCarryOverConditionNumber.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ActionTitle.ConditionNumber1", Me.DefaultScope), CInt(RuleCondition.Condition1)))
            cmbCarryOverConditionNumber.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ActionTitle.ConditionNumber2", Me.DefaultScope), CInt(RuleCondition.Condition2)))
            cmbCarryOverConditionNumber.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ActionTitle.ConditionNumber3", Me.DefaultScope), CInt(RuleCondition.Condition3)))
            cmbCarryOverConditionNumberResult.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ActionTitle.ConditionNumber1", Me.DefaultScope), CInt(RuleCondition.Condition1)))
            cmbCarryOverConditionNumberResult.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ActionTitle.ConditionNumber2", Me.DefaultScope), CInt(RuleCondition.Condition2)))
            cmbCarryOverConditionNumberResult.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ActionTitle.ConditionNumber3", Me.DefaultScope), CInt(RuleCondition.Condition3)))
            cmbCarryOverConditionNumber.SelectedIndex = 0
            cmbCarryOverConditionNumberResult.SelectedIndex = 0

            cmbPlusActions.Items.Clear()
            cmbPlusActions.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate(System.Enum.GetName(GetType(ValueType), ValueType.DirectValue).ToString & "_Plus", DefaultScope), CInt(ValueType.DirectValue)))
            cmbPlusActions.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate(System.Enum.GetName(GetType(ValueType), ValueType.UserField).ToString & "_Plus", DefaultScope), CInt(ValueType.UserField)))
            cmbPlusActions.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate(System.Enum.GetName(GetType(ValueType), ValueType.IDCause).ToString & "_Plus", DefaultScope), CInt(ValueType.IDCause)))
            cmbPlusActions.SelectedIndex = 0

            cmbPlusResultActions.Items.Clear()
            cmbPlusResultActions.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate(System.Enum.GetName(GetType(ValueType), ValueType.DirectValue).ToString & "_PlusResult", DefaultScope), CInt(ValueType.DirectValue)))
            cmbPlusResultActions.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate(System.Enum.GetName(GetType(ValueType), ValueType.UserField).ToString & "_PlusResult", DefaultScope), CInt(ValueType.UserField)))
            cmbPlusResultActions.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate(System.Enum.GetName(GetType(ValueType), ValueType.IDCause).ToString & "_PlusResult", DefaultScope), CInt(ValueType.IDCause)))
            cmbPlusResultActions.SelectedIndex = 0

            cmbPlusConditionPart.Items.Clear()
            cmbPlusConditionPartResult.Items.Clear()
            cmbPlusConditionPart.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ActionTitle.ConditionPart1", Me.DefaultScope), CInt(RulePart.Part1)))
            cmbPlusConditionPart.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ActionTitle.ConditionPart2", Me.DefaultScope), CInt(RulePart.Part2)))
            cmbPlusConditionPartResult.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ActionTitle.ConditionPart1Result", Me.DefaultScope), CInt(RulePart.Part1)))
            cmbPlusConditionPartResult.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ActionTitle.ConditionPart2Result", Me.DefaultScope), CInt(RulePart.Part2)))
            cmbPlusConditionPart.SelectedIndex = 0
            cmbPlusConditionPartResult.SelectedIndex = 0

            cmbPlusConditionNumber.Items.Clear()
            cmbPlusConditionNumberResult.Items.Clear()
            cmbPlusConditionNumber.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ActionTitle.ConditionNumber1", Me.DefaultScope), CInt(RuleCondition.Condition1)))
            cmbPlusConditionNumber.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ActionTitle.ConditionNumber2", Me.DefaultScope), CInt(RuleCondition.Condition2)))
            cmbPlusConditionNumber.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ActionTitle.ConditionNumber3", Me.DefaultScope), CInt(RuleCondition.Condition3)))
            cmbPlusConditionNumberResult.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ActionTitle.ConditionNumber1", Me.DefaultScope), CInt(RuleCondition.Condition1)))
            cmbPlusConditionNumberResult.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ActionTitle.ConditionNumber2", Me.DefaultScope), CInt(RuleCondition.Condition2)))
            cmbPlusConditionNumberResult.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ActionTitle.ConditionNumber3", Me.DefaultScope), CInt(RuleCondition.Condition3)))
            cmbPlusConditionNumber.SelectedIndex = 0
            cmbPlusConditionNumberResult.SelectedIndex = 0

            cmbCauseUFieldPlus.Items.Clear()
            cmbCauseUFieldPlusResult.Items.Clear()
            cmbCauseUFieldCarryOver.Items.Clear()
            cmbCauseUFieldCarryOverResult.Items.Clear()
            For Each oRow As DataRow In tbUserFields.Select("", "FieldName")
                cmbCauseUFieldPlus.Items.Add(New DevExpress.Web.ListEditItem(oRow("FieldName"), oRow("FieldName")))
                cmbCauseUFieldPlusResult.Items.Add(New DevExpress.Web.ListEditItem(oRow("FieldName"), oRow("FieldName")))
                cmbCauseUFieldCarryOver.Items.Add(New DevExpress.Web.ListEditItem(oRow("FieldName"), oRow("FieldName")))
                cmbCauseUFieldCarryOverResult.Items.Add(New DevExpress.Web.ListEditItem(oRow("FieldName"), oRow("FieldName")))
            Next
            cmbCauseUFieldPlus.SelectedIndex = 0
            cmbCauseUFieldPlusResult.SelectedIndex = 0
            cmbCauseUFieldCarryOver.SelectedIndex = 0
            cmbCauseUFieldCarryOverResult.SelectedIndex = 0

            cmbPlusSign.Items.Clear()
            cmbPlusSign.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ActionTitle.SignPlus", Me.DefaultScope), CInt(OperatorCondition.Positive)))
            cmbPlusSign.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ActionTitle.SignMinus", Me.DefaultScope), CInt(OperatorCondition.Negative)))
            cmbPlusSign.SelectedIndex = 0
        Catch ex As Exception
            Response.Write(ex.StackTrace)
        End Try
    End Sub

End Class