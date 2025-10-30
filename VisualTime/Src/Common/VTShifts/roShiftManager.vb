Imports System.Data.Common
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Namespace Shifts

    Public Class roShiftManager
        Private oState As roShiftManagerState = Nothing

        Public ReadOnly Property State As roShiftManagerState
            Get
                Return oState
            End Get
        End Property

        Public Sub New()
            Me.oState = New roShiftManagerState()
        End Sub

        Public Sub New(ByVal _State As roShiftManagerState)
            Me.oState = _State
        End Sub

#Region "Methods"

        Public Function Load(ByVal IDShift As Integer, Optional ByVal bAudit As Boolean = False) As roShiftEngine

            Dim oShift As roShiftEngine = Nothing
            Try

                Dim strQuery As String = " @SELECT# * from Shifts Where ID = " & IDShift.ToString

                Dim oDt As DataTable = CreateDataTable(strQuery)

                If oDt IsNot Nothing AndAlso oDt.Rows.Count = 1 Then
                    oShift = New roShiftEngine
                    Dim oRow As DataRow = oDt.Rows(0)

                    oShift.ID = oRow("ID")
                    oShift.Name = oRow("Name")
                    oShift.Description = roTypes.Any2String(oRow("Description"))
                    oShift.Color = IIf(Not IsDBNull(oRow("Color")), oRow("Color"), 0)
                    oShift.ExpectedWorkingHours = oRow("ExpectedWorkingHours")
                    oShift.IsObsolete = oRow("IsObsolete")
                    oShift.IsTemplate = oRow("IsTemplate")
                    oShift.StartLimit = oRow("StartLimit")
                    oShift.EndLimit = oRow("EndLimit")
                    oShift.ManualLimit = oRow("ManualLimit")
                    oShift.ShortName = IIf(Not IsDBNull(oRow("ShortName")), oRow("ShortName"), "")
                    oShift.TypeShift = IIf(Not IsDBNull(oRow("TypeShift")), oRow("TypeShift"), "")
                    oShift.IDGroup = oRow("IDGroup")
                    oShift.WebVisible = oRow("WebVisible")
                    oShift.WebLaboral = IIf(Not IsDBNull(oRow("WebLaboral")), oRow("WebLaboral"), False)
                    oShift.IDConceptBalance = IIf(Not IsDBNull(oRow("IDConceptBalance")), oRow("IDConceptBalance"), 0)
                    oShift.IDConceptRequestNextYear = IIf(Not IsDBNull(oRow("IDConceptRequestNextYear")), oRow("IDConceptRequestNextYear"), 0)
                    oShift.IDCauseHolidays = IIf(Not IsDBNull(oRow("IDCauseHolidays")), oRow("IDCauseHolidays"), 0)
                    oShift.AreWorkingDays = IIf(Not IsDBNull(oRow("AreWorkingDays")), oRow("AreWorkingDays"), True)
                    oShift.AdvancedParameters = IIf(Not IsDBNull(oRow("AdvancedParameters")), oRow("AdvancedParameters"), "")
                    oShift.EnableNotifyExit = roTypes.Any2Boolean(oRow("EnableNotifyExit"))
                    oShift.CompleteExitAt = roTypes.Any2Integer(oRow("CompleteExitAt"))

                    oShift.EnableCompleteExit = roTypes.Any2Boolean(oRow("EnableCompleteExit"))
                    oShift.NotifyEmployeeExitAt = roTypes.Any2Integer(oRow("NotifyEmployeeExitAt"))

                    oShift.IDCenter = IIf(IsDBNull(oRow("IDCenter")), 0, oRow("IDCenter"))
                    oShift.ApplyCenterOnAbsence = IIf(IsDBNull(oRow("ApplyCenterOnAbsence")), False, oRow("ApplyCenterOnAbsence"))
                    oShift.AllowComplementary = IIf(IsDBNull(oRow("AllowComplementary")), False, oRow("AllowComplementary"))
                    oShift.BreakHours = IIf(IsDBNull(oRow("BreakHours")), 0, oRow("BreakHours"))
                    oShift.AllowFloatingData = IIf(IsDBNull(oRow("AllowFloatingData")), False, oRow("AllowFloatingData"))
                    oShift.ExportName = IIf(Not IsDBNull(oRow("Export")), oRow("Export"), "")
                    oShift.TypeHolidayValue = IIf(Not IsDBNull(oRow("TypeHolidayValue")), oRow("TypeHolidayValue"), HolidayValueType.ExpectedWorkingHours_Value)

                    oShift.HolidayValue = IIf(Not IsDBNull(oRow("HolidayValue")), oRow("HolidayValue"), 0)

                    oShift.DailyFactor = IIf(Not IsDBNull(oRow("DailyFactor")), oRow("DailyFactor"), 1)

                    Dim oLicense As New roServerLicense
                    Dim bMultipleShifts As Boolean = oLicense.FeatureIsInstalled("Feature\MultipleShifts")

                    '->Dim bolIsFloating As Boolean = (Me.bolMultipleShifts And Any2Boolean(oRow("IsFloating")))
                    Dim bolIsFloating As Boolean = (bMultipleShifts And roTypes.Any2Boolean(oRow("IsFloating")))
                    Select Case roTypes.Any2Integer(oRow("ShiftType"))
                        Case 0, 1
                            If bolIsFloating Then
                                oShift.ShiftType = ShiftType.NormalFloating
                            Else
                                oShift.ShiftType = ShiftType.Normal
                            End If

                        Case 2
                            oShift.ShiftType = ShiftType.Vacations
                    End Select

                    If Not IsDBNull(oRow("StartFloating")) Then
                        oShift.StartFloating = oRow("StartFloating")
                    Else
                        oShift.StartFloating = Nothing
                    End If

                    oShift.Layers = Me.LoadLayers(oShift, bAudit)

                    oShift.SimpleRules = Me.GetShiftRultes(oShift, ShiftRuleType.Simple, bAudit)

                    oShift.DailyRules = Me.GetDailyShiftRules(oShift, bAudit)

                    oShift.TimeZones = Me.GetShiftTimeZones(oShift, bAudit)

                    oShift.Assignments = Me.GetShiftAssignments(oShift, bAudit)

                    oShift.VisibilityPermissions = roTypes.Any2Integer(oRow("VisibilityPermissions"))

                End If

                ' Auditar lectura
                If bAudit AndAlso oShift IsNot Nothing Then
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tShift, "", tbParameters, -1)
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roShiftManager::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShiftManager::Load")
            Finally

            End Try

            Return oShift
        End Function

        Private Function LoadLayers(ByVal oShift As roShiftEngine, Optional ByVal bAudit As Boolean = False) As Generic.List(Of roShiftEngineLayer)
            Dim oLayers As New Generic.List(Of roShiftEngineLayer)

            Try

                Dim strShiftLayerTypes As String = ""
                Dim strSQL As String = " @SELECT# * from sysroShiftsLayers Where (IDShift = " & oShift.ID & " And ParentLayerID IS NULL) OR (IDShift = " & oShift.ID & " And ParentLayerID = 0) Order by IDType "
                Dim tb As DataTable = CreateDataTable(strSQL, "sysroShiftsLayers")

                For Each dRow As DataRow In tb.Rows
                    Dim oLayer As New roShiftEngineLayer()

                    oLayer.ID = dRow("ID")
                    oLayer.IDShift = dRow("IDShift")
                    oLayer.LayerType = dRow("IDType")
                    oLayer.ParentID = IIf(IsDBNull(dRow("ParentLayerID")), -1, dRow("ParentLayerID"))

                    oLayer.Data = New roCollection
                    oLayer.Data.Clear()
                    oLayer.Data.LoadXMLString(dRow("Definition"))

                    Dim tbChilds As DataTable = CreateDataTable(" @SELECT# * from sysroShiftsLayers Where IDShift = " & oShift.ID & " And ParentLayerID = " & dRow("ID") & " Order by IDType ", "sysroShiftLayersChilds")
                    If tbChilds.Rows.Count > 0 Then
                        oLayer.ChildLayers = New Generic.List(Of roShiftEngineLayer)
                        'Fem consulta per carregar els layers fills
                        For Each dRowChild As DataRow In tbChilds.Rows
                            Dim oLayerChild As New roShiftEngineLayer()
                            oLayerChild.ID = dRowChild("ID")
                            oLayerChild.IDShift = dRowChild("IDShift")
                            oLayerChild.LayerType = dRowChild("IDType")
                            oLayerChild.ParentID = IIf(IsDBNull(dRowChild("ParentLayerID")), -1, dRowChild("ParentLayerID"))

                            oLayerChild.Data = New roCollection
                            oLayerChild.Data.Clear()
                            oLayerChild.Data.LoadXMLString(dRowChild("Definition"))

                            oLayer.ChildLayers.Add(oLayerChild)

                            strShiftLayerTypes &= IIf(strShiftLayerTypes <> "", ",", "") & System.Enum.GetName(GetType(roLayerTypes), oLayerChild.LayerType)
                        Next
                    Else
                        oLayer.ChildLayers = Nothing
                    End If

                    oLayers.Add(oLayer)

                    strShiftLayerTypes &= IIf(strShiftLayerTypes <> "", ",", "") & System.Enum.GetName(GetType(roLayerTypes), oLayer.LayerType)
                Next

                If tb.Rows.Count > 0 And bAudit Then
                    ' Auditamos consulta múltiple capas horario
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{ShiftLayerTypes}", strShiftLayerTypes, "", 1)
                    oState.AddAuditParameter(tbParameters, "{ShiftName}", oShift.Name, "", 1)
                    oState.Audit(Audit.Action.aMultiSelect, Audit.ObjectType.tShiftLayer, oShift.Name, tbParameters, -1)
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roShiftManager::LoadLayers")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShiftManager::LoadLayers")
            Finally

            End Try

            Return oLayers
        End Function

        Public Function GetShiftRultes(ByVal oShift As roShiftEngine, ByVal _RuleType As ShiftRuleType, Optional ByVal bAudit As Boolean = False) As Generic.List(Of roShiftEngineRule)

            Dim oRet As New Generic.List(Of roShiftEngineRule)

            Try

                Dim strSQL As String = "@SELECT# * " &
                                       "FROM sysroShiftsCausesRules " &
                                       "WHERE IDShift = " & oShift.ID.ToString & " AND " &
                                             "RuleType = " & CStr(_RuleType) & " " &
                                       "ORDER BY ID"
                Dim tb As DataTable = CreateDataTable(strSQL)

                For Each oRow As DataRow In tb.Rows

                    Dim oRule As New roShiftEngineRule

                    Dim strXml As String = roTypes.Any2String(oRow("Definition"))
                    oRule.ID = roTypes.Any2Integer(oRow("ID"))
                    oRule.IDShift = oShift.ID

                    If strXml <> "" Then
                        ' Añadimos la composición a la colección
                        Dim oDefinition As New roCollection(strXml)

                        If oDefinition.Exists("Incidence") Then
                            oRule.IDIncidence = oDefinition("Incidence")
                        End If

                        If oDefinition.Exists("Zone") Then
                            oRule.IDZone = oDefinition("Zone")
                        End If

                        oRule.ConditionValueType = eShiftRuleValueType.DirectValue
                        If oDefinition.Exists("ConditionValueType") Then
                            oRule.ConditionValueType = oDefinition("ConditionValueType")
                        End If

                        If oDefinition.Exists("FromTime") Then
                            oRule.FromTime = CDate(roTypes.Any2Time(CDate(oDefinition("FromTime"))).Value)
                        End If
                        If oDefinition.Exists("ToTime") Then
                            oRule.ToTime = CDate(roTypes.Any2Time(CDate(oDefinition("ToTime"))).Value)
                        End If

                        oRule.FromValueUserFieldName = String.Empty
                        If oDefinition.Exists("FromValueUserField") AndAlso oDefinition("FromValueUserField") <> "" Then
                            oRule.FromValueUserFieldName = oDefinition("FromValueUserField")
                        End If

                        oRule.ToValueUserFieldName = String.Empty
                        If oDefinition.Exists("ToValueUserField") AndAlso oDefinition("ToValueUserField") <> "" Then
                            oRule.ToValueUserFieldName = oDefinition("ToValueUserField")
                        End If

                        oRule.BetweenValueUserFieldName = String.Empty
                        If oDefinition.Exists("BetweenValueUserField") AndAlso oDefinition("BetweenValueUserField") <> "" Then
                            oRule.BetweenValueUserFieldName = oDefinition("BetweenValueUserField")
                        End If

                        If oDefinition.Exists("Cause") Then
                            oRule.IDCause = oDefinition("Cause")
                        End If

                        oRule.ActionValueType = eShiftRuleValueType.DirectValue
                        If oDefinition.Exists("ActionValueType") Then
                            oRule.ActionValueType = oDefinition("ActionValueType")
                        End If

                        If oDefinition.Exists("MaxTime") Then
                            oRule.MaxTime = CDate(roTypes.Any2Time(CDate(oDefinition("MaxTime"))).Value)
                        End If

                        oRule.MaxValueUserFieldName = String.Empty
                        If oDefinition.Exists("MaxValueUserField") AndAlso oDefinition("MaxValueUserField") <> "" Then
                            oRule.MaxValueUserFieldName = oDefinition("MaxValueUserField")
                        End If

                        oRule.Type = _RuleType
                    End If

                    oRet.Add(oRule)
                Next
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roShiftManager::GetShiftRultes")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShiftManager::GetShiftRultes")
            Finally

            End Try

            Return oRet

        End Function

        Public Function GetDailyShiftRules(ByVal oShift As roShiftEngine, Optional ByVal bAudit As Boolean = False) As Generic.List(Of roShiftDailyRule)

            Dim oRet As New Generic.List(Of roShiftDailyRule)

            Try

                Dim strSQL As String = "@SELECT# * " &
                                       "FROM sysroShiftsCausesRules " &
                                       "WHERE IDShift = " & oShift.ID.ToString & " AND " &
                                             "RuleType = 3 " &
                                       "ORDER BY ID"
                Dim tb As DataTable = CreateDataTable(strSQL)

                For Each oRow As DataRow In tb.Rows
                    Dim oShiftDailyRule As New roShiftDailyRule

                    Dim strXml As String = roTypes.Any2String(oRow("Definition"))
                    oShiftDailyRule.ID = roTypes.Any2Integer(oRow("ID"))
                    oShiftDailyRule.IDShift = oShift.ID

                    If strXml <> "" Then
                        ' Añadimos la composición a la colección
                        Dim oDefinition As New roCollection(strXml)

                        If oDefinition.Exists("Name") Then
                            oShiftDailyRule.Name = oDefinition("Name")
                        End If

                        If oDefinition.Exists("Description") Then
                            oShiftDailyRule.Description = oDefinition("Description")
                        End If

                        If oDefinition.Exists("DayValidationRule") Then
                            oShiftDailyRule.DayValidationRule = roTypes.Any2Integer(oDefinition("DayValidationRule"))
                        End If

                        If oDefinition.Exists("PreviousShiftValidationRule") Then
                            oShiftDailyRule.PreviousShiftValidationRule = roTypes.Any2String(oDefinition("PreviousShiftValidationRule")).Split(",").ToList().ConvertAll(Function(str) roTypes.Any2Integer(str))
                        End If

                        If oDefinition.Exists("ApplyScheduleValidationRule") Then
                            oShiftDailyRule.ApplyScheduleValidationRule = roTypes.Any2Integer(oDefinition("ApplyScheduleValidationRule"))
                        End If

                        If oDefinition.Exists("ScheduleRulesValidationRule") Then
                            oShiftDailyRule.ScheduleRulesValidationRule = roTypes.Any2String(oDefinition("ScheduleRulesValidationRule")).Split(",").ToList().ConvertAll(Function(str) roTypes.Any2Integer(str))
                        End If


                        ' Agregamos todas las condiciones
                        If oDefinition.Exists("TotalConditions") Then
                            Dim dblTotalConditions As Double = oDefinition("TotalConditions")
                            For i As Integer = 1 To dblTotalConditions
                                Dim oShiftDailyRuleCondition As New roShiftDailyRuleCondition

                                If oDefinition.Exists("ConditionCauses_" & i.ToString) Then
                                    Dim strConditionCause As String = oDefinition("ConditionCauses_" & i.ToString)
                                    If strConditionCause.Length > 0 Then
                                        For Each strCause As String In strConditionCause.Split(",")
                                            Dim oShiftDailyRuleConditionCause As New roShiftDailyRuleConditionCause
                                            oShiftDailyRuleConditionCause.IDCause = roTypes.Any2Double(strCause.Split("_")(0))
                                            oShiftDailyRuleConditionCause.Operation = roTypes.Any2Integer(strCause.Split("_")(1))
                                            oShiftDailyRuleConditionCause.Name = roTypes.Any2String(ExecuteScalar("@SELECT# Name from Causes where id=" & oShiftDailyRuleConditionCause.IDCause))
                                            oShiftDailyRuleCondition.ConditionCauses.Add(oShiftDailyRuleConditionCause)
                                        Next
                                    End If
                                End If

                                If oDefinition.Exists("ConditionTimeZones_" & i.ToString) Then
                                    Dim strConditionZone As String = oDefinition("ConditionTimeZones_" & i.ToString)
                                    If strConditionZone.Length > 0 Then
                                        For Each iZones As Integer In strConditionZone.Split(",")
                                            Dim oShiftDailyRuleConditionTimeZone As New roShiftDailyRuleConditionTimeZone
                                            oShiftDailyRuleConditionTimeZone.IDTimeZone = iZones
                                            If oShiftDailyRuleConditionTimeZone.IDTimeZone <> -1 Then
                                                oShiftDailyRuleConditionTimeZone.Name = roTypes.Any2String(ExecuteScalar("@SELECT# Name from TimeZones where id=" & oShiftDailyRuleConditionTimeZone.IDTimeZone))
                                            Else
                                                oShiftDailyRuleConditionTimeZone.Name = Me.oState.Language.Translate("CRUFLCOM.Shifts.All", "")
                                            End If

                                            oShiftDailyRuleCondition.ConditionTimeZones.Add(oShiftDailyRuleConditionTimeZone)
                                        Next
                                    End If
                                End If

                                If oDefinition.Exists("Compare_" & i.ToString) Then
                                    oShiftDailyRuleCondition.Compare = roTypes.Any2Integer(oDefinition("Compare_" & i.ToString))
                                End If

                                If oDefinition.Exists("Type_" & i.ToString) Then
                                    oShiftDailyRuleCondition.Type = roTypes.Any2Integer(oDefinition("Type_" & i.ToString))
                                End If

                                If oDefinition.Exists("FromValue_" & i.ToString) Then
                                    oShiftDailyRuleCondition.FromValue = roTypes.Any2String(oDefinition("FromValue_" & i.ToString))
                                End If

                                If oDefinition.Exists("ToValue_" & i.ToString) Then
                                    oShiftDailyRuleCondition.ToValue = roTypes.Any2String(oDefinition("ToValue_" & i.ToString))
                                End If

                                If oDefinition.Exists("UserField_" & i.ToString) Then
                                    oShiftDailyRuleCondition.UserField = roTypes.Any2String(oDefinition("UserField_" & i.ToString))
                                End If

                                If oDefinition.Exists("CompareCauses_" & i.ToString) Then
                                    Dim strConditionCause As String = oDefinition("CompareCauses_" & i.ToString)
                                    If strConditionCause.Length > 0 Then
                                        For Each strCause As String In strConditionCause.Split(",")
                                            Dim oShiftDailyRuleConditionCause As New roShiftDailyRuleConditionCause
                                            oShiftDailyRuleConditionCause.IDCause = roTypes.Any2Double(strCause.Split("_")(0))
                                            oShiftDailyRuleConditionCause.Operation = roTypes.Any2Integer(strCause.Split("_")(1))
                                            oShiftDailyRuleConditionCause.Name = roTypes.Any2String(ExecuteScalar("@SELECT# Name from Causes where id=" & oShiftDailyRuleConditionCause.IDCause))
                                            oShiftDailyRuleCondition.CompareCauses.Add(oShiftDailyRuleConditionCause)
                                        Next
                                    End If
                                End If

                                If oDefinition.Exists("CompareTimeZones_" & i.ToString) Then
                                    Dim strConditionZone As String = oDefinition("CompareTimeZones_" & i.ToString)
                                    If strConditionZone.Length > 0 Then
                                        For Each iZones As Integer In strConditionZone.Split(",")
                                            Dim oShiftDailyRuleConditionTimeZone As New roShiftDailyRuleConditionTimeZone
                                            oShiftDailyRuleConditionTimeZone.IDTimeZone = iZones
                                            If oShiftDailyRuleConditionTimeZone.IDTimeZone <> -1 Then
                                                oShiftDailyRuleConditionTimeZone.Name = roTypes.Any2String(ExecuteScalar("@SELECT# Name from TimeZones where id=" & oShiftDailyRuleConditionTimeZone.IDTimeZone))
                                            Else
                                                oShiftDailyRuleConditionTimeZone.Name = Me.oState.Language.Translate("CRUFLCOM.Shifts.All", "")
                                            End If

                                            oShiftDailyRuleCondition.CompareTimeZones.Add(oShiftDailyRuleConditionTimeZone)
                                        Next
                                    End If
                                End If

                                ' Añadimos la condicion
                                oShiftDailyRule.Conditions.Add(oShiftDailyRuleCondition)

                            Next
                        End If

                        ' Agregamos todas las acciones
                        If oDefinition.Exists("TotalActions") Then
                            Dim dblTotalActions As Double = oDefinition("TotalActions")
                            For i As Integer = 1 To dblTotalActions
                                Dim oShiftDailyRuleAction As New roShiftDailyRuleAction
                                If oDefinition.Exists("Action_" & i.ToString) Then
                                    oShiftDailyRuleAction.Action = roTypes.Any2Integer(oDefinition("Action_" & i.ToString))
                                End If

                                If oDefinition.Exists("CarryOverAction_" & i.ToString) Then
                                    oShiftDailyRuleAction.CarryOverAction = roTypes.Any2Integer(oDefinition("CarryOverAction_" & i.ToString))
                                End If

                                If oDefinition.Exists("CarryOverDirectValue_" & i.ToString) Then
                                    oShiftDailyRuleAction.CarryOverDirectValue = roTypes.Any2String(oDefinition("CarryOverDirectValue_" & i.ToString))
                                End If

                                If oDefinition.Exists("CarryOverUserFieldValue_" & i.ToString) Then
                                    oShiftDailyRuleAction.CarryOverUserFieldValue = roTypes.Any2String(oDefinition("CarryOverUserFieldValue_" & i.ToString))
                                End If

                                If oDefinition.Exists("CarryOverConditionPart_" & i.ToString) Then
                                    oShiftDailyRuleAction.CarryOverConditionPart = roTypes.Any2Integer(oDefinition("CarryOverConditionPart_" & i.ToString))
                                End If

                                If oDefinition.Exists("CarryOverConditionNumber_" & i.ToString) Then
                                    oShiftDailyRuleAction.CarryOverConditionNumber = roTypes.Any2Integer(oDefinition("CarryOverConditionNumber_" & i.ToString))
                                End If

                                If oDefinition.Exists("CarryOverActionResult_" & i.ToString) Then
                                    oShiftDailyRuleAction.CarryOverActionResult = roTypes.Any2Integer(oDefinition("CarryOverActionResult_" & i.ToString))
                                End If

                                If oDefinition.Exists("CarryOverDirectValueResult_" & i.ToString) Then
                                    oShiftDailyRuleAction.CarryOverDirectValueResult = roTypes.Any2String(oDefinition("CarryOverDirectValueResult_" & i.ToString))
                                End If

                                If oDefinition.Exists("CarryOverUserFieldValueResult_" & i.ToString) Then
                                    oShiftDailyRuleAction.CarryOverUserFieldValueResult = roTypes.Any2String(oDefinition("CarryOverUserFieldValueResult_" & i.ToString))
                                End If

                                If oDefinition.Exists("CarryOverConditionPartResult_" & i.ToString) Then
                                    oShiftDailyRuleAction.CarryOverConditionPartResult = roTypes.Any2Integer(oDefinition("CarryOverConditionPartResult_" & i.ToString))
                                End If

                                If oDefinition.Exists("CarryOverConditionNumberResult_" & i.ToString) Then
                                    oShiftDailyRuleAction.CarryOverConditionNumberResult = roTypes.Any2Integer(oDefinition("CarryOverConditionNumberResult_" & i.ToString))
                                End If

                                If oDefinition.Exists("CarryOverIDCauseFrom_" & i.ToString) Then
                                    oShiftDailyRuleAction.CarryOverIDCauseFrom = roTypes.Any2Integer(oDefinition("CarryOverIDCauseFrom_" & i.ToString))
                                End If

                                If oDefinition.Exists("CarryOverIDCauseTo_" & i.ToString) Then
                                    oShiftDailyRuleAction.CarryOverIDCauseTo = roTypes.Any2Integer(oDefinition("CarryOverIDCauseTo_" & i.ToString))
                                End If

                                If oDefinition.Exists("PlusIDCause_" & i.ToString) Then
                                    oShiftDailyRuleAction.PlusIDCause = roTypes.Any2Integer(oDefinition("PlusIDCause_" & i.ToString))
                                End If

                                If oDefinition.Exists("PlusAction_" & i.ToString) Then
                                    oShiftDailyRuleAction.PlusAction = roTypes.Any2Integer(oDefinition("PlusAction_" & i.ToString))
                                End If

                                If oDefinition.Exists("PlusDirectValue_" & i.ToString) Then
                                    oShiftDailyRuleAction.PlusDirectValue = roTypes.Any2String(oDefinition("PlusDirectValue_" & i.ToString))
                                End If

                                If oDefinition.Exists("PlusUserFieldValue_" & i.ToString) Then
                                    oShiftDailyRuleAction.PlusUserFieldValue = roTypes.Any2String(oDefinition("PlusUserFieldValue_" & i.ToString))
                                End If

                                If oDefinition.Exists("PlusConditionPart_" & i.ToString) Then
                                    oShiftDailyRuleAction.PlusConditionPart = roTypes.Any2Integer(oDefinition("PlusConditionPart_" & i.ToString))
                                End If

                                If oDefinition.Exists("PlusConditionNumber_" & i.ToString) Then
                                    oShiftDailyRuleAction.PlusConditionNumber = roTypes.Any2Integer(oDefinition("PlusConditionNumber_" & i.ToString))
                                End If

                                If oDefinition.Exists("PlusActionResult_" & i.ToString) Then
                                    oShiftDailyRuleAction.PlusActionResult = roTypes.Any2Integer(oDefinition("PlusActionResult_" & i.ToString))
                                End If

                                If oDefinition.Exists("PlusDirectValueResult_" & i.ToString) Then
                                    oShiftDailyRuleAction.PlusDirectValueResult = roTypes.Any2String(oDefinition("PlusDirectValueResult_" & i.ToString))
                                End If

                                If oDefinition.Exists("PlusUserFieldValueResult_" & i.ToString) Then
                                    oShiftDailyRuleAction.PlusUserFieldValueResult = roTypes.Any2String(oDefinition("PlusUserFieldValueResult_" & i.ToString))
                                End If

                                If oDefinition.Exists("PlusConditionPartResult_" & i.ToString) Then
                                    oShiftDailyRuleAction.PlusConditionPartResult = roTypes.Any2Integer(oDefinition("PlusConditionPartResult_" & i.ToString))
                                End If

                                If oDefinition.Exists("PlusConditionNumberResult_" & i.ToString) Then
                                    oShiftDailyRuleAction.PlusConditionNumberResult = roTypes.Any2Integer(oDefinition("PlusConditionNumberResult_" & i.ToString))
                                End If

                                If oDefinition.Exists("PlusActionSign_" & i.ToString) Then
                                    oShiftDailyRuleAction.PlusActionSign = roTypes.Any2Integer(oDefinition("PlusActionSign_" & i.ToString))
                                End If

                                If oDefinition.Exists("CarryOverSingleCause_" & i.ToString) Then
                                    oShiftDailyRuleAction.CarryOverSingleCause = roTypes.Any2Integer(oDefinition("CarryOverSingleCause_" & i.ToString))
                                End If

                                If oDefinition.Exists("ActionCauses_" & i.ToString) Then
                                    Dim strActionCause As String = oDefinition("ActionCauses_" & i.ToString)
                                    If strActionCause.Length > 0 Then
                                        For Each strCause As String In strActionCause.Split(",")
                                            Dim oShiftDailyRuleActionCause As New roShiftDailyRuleActionCause
                                            oShiftDailyRuleActionCause.IDCause = roTypes.Any2Double(strCause.Split("_")(0))
                                            oShiftDailyRuleActionCause.IDCause2 = roTypes.Any2Integer(strCause.Split("_")(1))
                                            oShiftDailyRuleActionCause.Name = roTypes.Any2String(ExecuteScalar("@SELECT# Name from Causes where id=" & oShiftDailyRuleActionCause.IDCause))
                                            oShiftDailyRuleActionCause.Name2 = roTypes.Any2String(ExecuteScalar("@SELECT# Name from Causes where id=" & oShiftDailyRuleActionCause.IDCause2))
                                            oShiftDailyRuleAction.ActionCauses.Add(oShiftDailyRuleActionCause)
                                        Next
                                    End If
                                End If

                                ' Añadimos la accion
                                oShiftDailyRule.Actions.Add(oShiftDailyRuleAction)

                            Next
                        End If

                    End If

                    If Not oShiftDailyRule Is Nothing Then
                        oShiftDailyRule.Priority = oRet.Count + 1
                        oRet.Add(oShiftDailyRule)
                    End If
                Next
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roShiftManager::GetDailyShiftRules")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShiftManager::GetDailyShiftRules")
            Finally

            End Try

            Return oRet

        End Function

        Public Function GetShiftTimeZones(ByVal oShift As roShiftEngine, Optional ByVal bAudit As Boolean = False) As Generic.List(Of roShiftEngineTimeZone)

            Dim oRet As New Generic.List(Of roShiftEngineTimeZone)

            Try

                Dim strSQL As String = "@SELECT# * " &
                                       "FROM sysroShiftTimeZones " &
                                       "WHERE IDShift = " & oShift.ID.ToString
                Dim tb As DataTable = CreateDataTable(strSQL)

                If tb IsNot Nothing Then

                    For Each oRow As DataRow In tb.Rows
                        Dim oTimezone As New roShiftEngineTimeZone

                        oTimezone.IDShift = oRow("IDShift")
                        oTimezone.IDZone = oRow("IDZone")
                        oTimezone.BeginTime = CDate(oRow("BeginTime"))
                        oTimezone.EndTime = CDate(oRow("EndTime"))
                        oTimezone.IsBlocked = roTypes.Any2Boolean(oRow("IsBlocked"))

                        oRet.Add(oTimezone)
                    Next

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roShiftTimeZone::GetShiftTimeZones")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShiftTimeZone::GetShiftTimeZones")
            Finally

            End Try

            Return oRet

        End Function

        Public Function GetShiftAssignments(ByVal oShift As roShiftEngine, Optional ByVal bAudit As Boolean = False) As Generic.List(Of roShiftEngineAssignment)

            Dim oRet As New Generic.List(Of roShiftEngineAssignment)

            Try

                Dim strSQL As String = "@SELECT# ShiftAssignments.* " &
                                       "FROM ShiftAssignments INNER JOIN Assignments " &
                                                "ON ShiftAssignments.IDAssignment = Assignments.ID " &
                                       "WHERE ShiftAssignments.IDShift = " & oShift.ID.ToString & " " &
                                       "ORDER BY Assignments.Name"
                Dim tb As DataTable = CreateDataTable(strSQL)
                If tb IsNot Nothing Then

                    For Each oRow As DataRow In tb.Rows
                        Dim oAssig As New roShiftEngineAssignment

                        oAssig.IDShift = oRow("IDShift")
                        oAssig.IDAssignment = oRow("IDAssignment")
                        oAssig.Coverage = roTypes.Any2Double(oRow("Coverage"))

                        oRet.Add(oAssig)
                    Next
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roShiftAssignment::GetShiftAssignments")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roShiftAssignment::GetShiftAssignments")
            Finally

            End Try

            Return oRet

        End Function

#End Region

    End Class

End Namespace