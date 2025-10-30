Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class WebUserForms_frmDailyRule
    Inherits UserControlBase

    Private Sub WebUserForms_frmDailyRule_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Me.cmbApplyWhen.ClientInstanceName = "cmbApplyWhenClient"

            Me.loadCombos()
        End If
    End Sub

    Private Sub loadCombos()
        Me.cmbApplyWhen.Items.Clear()
        Me.cmbApplyWhen.ValueType = GetType(Integer)
        For Each oValue As DayOfWeek In System.Enum.GetValues(GetType(DayValidationRule))
            cmbApplyWhen.Items.Add(Me.Language.Translate("DayValidationRule." & CInt(oValue), Me.DefaultScope), CInt(oValue))
        Next


        Me.tbOldShiftInList.Items.Clear()

        Dim dtShifts As DataTable = API.ShiftServiceMethods.GetShifts(Me.Page,, True)
        Dim index As Integer = 0
        If dtShifts IsNot Nothing AndAlso dtShifts.Rows.Count > 0 Then
            Me.hdnSourceShiftList.Add("hdnShiftNumber", dtShifts.Rows.Count)
            For Each oRow As DataRow In dtShifts.Rows

                Me.hdnSourceShiftList.Add("sInfo_" & index, $"{oRow("Id")}@@{If(roTypes.Any2Boolean(oRow("IsObsolete")), 1, 0)}@@{oRow("Name")}")

                'Me.tbOldShiftInList.Items.Add(oRow("Name"), oRow("Id"))

                index += 1
            Next
        End If

        Me.cmbApplyScheduleValidationRule.Items.Clear()
        Me.cmbApplyScheduleValidationRule.ValueType = GetType(Integer)
        For Each oValue In System.Enum.GetValues(GetType(ApplyScheduleValidationRule))
            cmbApplyScheduleValidationRule.Items.Add(Me.Language.Translate("ScheduleValidationRule." & CInt(oValue), Me.DefaultScope), CInt(oValue))
        Next

        Me.tbScheduleValidationRule.Items.Clear()

        Dim dtLabAgrees As DataTable = API.LabAgreeServiceMethods.GetLabAgrees(Me.Page)
        Dim labAgreeRules As roScheduleRule()
        If dtLabAgrees IsNot Nothing AndAlso dtLabAgrees.Rows.Count > 0 Then
            For Each oRowLabAgree As DataRow In dtLabAgrees.Rows
                labAgreeRules = API.ScheduleRulesServiceMethods.GetLabAgreeScheduleRules(Me.Page, oRowLabAgree("Id"))
                For Each oRowRule As roScheduleRule In labAgreeRules
                    'Solo mostramos las de descanso (IDRule = 1)
                    If oRowRule.IDRule = ScheduleRuleType.RestBetweenShifts AndAlso oRowRule.Enabled Then
                        Dim ruleName = oRowRule.RuleName
                        If oRowRule.RuleType = ScheduleRuleBaseType.System Then
                            ruleName = Me.Language.Translate("ScheduleRuleType." & System.Enum.GetName(GetType(ScheduleRuleType), oRowRule.IDRule), Me.DefaultScope)
                        End If
                        Me.tbScheduleValidationRule.Items.Add(oRowLabAgree("Name") & " / " & ruleName, oRowRule.Id)
                    End If
                Next
            Next
        End If

        Me.hdnScheduleValidationRuleCount.Add("count", Me.tbScheduleValidationRule.Items.Count)

    End Sub

End Class