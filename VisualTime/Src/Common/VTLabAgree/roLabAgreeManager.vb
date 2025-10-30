Imports System.Data.Common
Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.Security.Base
Imports Robotics.Base.VTCalendar

Namespace LabAgree

    Public Class roLabAgreeManager
        Private oState As roLabAgreeManagerState = Nothing

        Public ReadOnly Property State As roLabAgreeManagerState
            Get
                Return oState
            End Get
        End Property

        Public Sub New()
            Me.oState = New roLabAgreeManagerState()
        End Sub

        Public Sub New(ByVal _State As roLabAgreeManagerState)
            Me.oState = _State
        End Sub

#Region "Methods"

        Public Function Load(ByVal IDLabAgree As Integer, Optional ByVal bAudit As Boolean = False) As roLabAgreeEngine

            Dim oLabAgree As roLabAgreeEngine = Nothing
            Try

                Dim strQuery As String = "@SELECT# * FROM LabAgree WHERE ID = " & IDLabAgree.ToString

                Dim oDt As DataTable = CreateDataTable(strQuery)

                If oDt IsNot Nothing AndAlso oDt.Rows.Count = 1 Then
                    oLabAgree = New roLabAgreeEngine
                    Dim oRow As DataRow = oDt.Rows(0)

                    oLabAgree.ID = roTypes.Any2Integer(oRow("ID"))
                    oLabAgree.Name = roTypes.Any2String(oRow("Name"))
                    oLabAgree.Description = roTypes.Any2String(oRow("Description"))
                    oLabAgree.Telecommuting = roTypes.Any2Boolean(oRow("Telecommuting"))
                    oLabAgree.TelecommutingMandatoryDays = roTypes.Any2String(oRow("TelecommutingMandatoryDays"))
                    oLabAgree.PresenceMandatoryDays = roTypes.Any2String(oRow("PresenceMandatoryDays"))
                    oLabAgree.TelecommutingOptionalDays = roTypes.Any2String(oRow("TelecommutingOptionalDays"))
                    oLabAgree.TelecommutingMaxDays = roTypes.Any2Integer(oRow("TelecommutingMaxDays"))
                    oLabAgree.TelecommutingMaxPercentage = roTypes.Any2Integer(oRow("TelecommutingMaxPercentage"))
                    oLabAgree.TelecommutingPeriodType = roTypes.Any2Integer(oRow("PeriodType"))
                    If Not IsDBNull(oRow("TelecommutingAgreementStart")) Then
                        oLabAgree.TelecommutingAgreementStart = roTypes.Any2DateTime(oRow("TelecommutingAgreementStart"))
                    Else
                        oLabAgree.TelecommutingAgreementStart = Nothing
                    End If
                    If Not IsDBNull(oRow("TelecommutingAgreementEnd")) Then
                        oLabAgree.TelecommutingAgreementEnd = roTypes.Any2DateTime(oRow("TelecommutingAgreementEnd"))
                    Else
                        oLabAgree.TelecommutingAgreementEnd = Nothing
                    End If

                    oLabAgree.LabAgreeAccrualRules = Me.GetLabAgreeAccrualRules(IDLabAgree, bAudit)
                    oLabAgree.StartupValues = Me.GetLabAgreeStartupValues(IDLabAgree, bAudit)
                    oLabAgree.LabAgreeCauseLimitValues = Me.GetLabAgreeCauseLimitValues(IDLabAgree, bAudit)
                    'Cargamos reglas de planificación
                    Dim calendarScheduleRulesManager As New roCalendarScheduleRulesManager(Nothing)
                    oLabAgree.LabAgreeScheduleRules = calendarScheduleRulesManager.GetScheduleRules(IDLabAgree)

                    oLabAgree.ExtraHoursConfiguration = roTypes.Any2Integer(oRow("ExtraHoursConfiguration"))
                    oLabAgree.ExtraHoursIDCauseSimples = roTypes.Any2String(oRow("ExtraHoursIDCauseSimples"))
                    oLabAgree.ExtraHoursIDCauseDoubles = roTypes.Any2Integer(oRow("ExtraHoursIDCauseDoubles"))
                    oLabAgree.ExtraHoursIDCauseTriples = roTypes.Any2Integer(oRow("ExtraHoursIDCauseTriples"))

                End If

                ' Auditar lectura
                If bAudit AndAlso oLabAgree IsNot Nothing Then
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tLabAgree, "", tbParameters, -1)
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roLabAgreeManager::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roLabAgreeManager::Load")
            End Try

            Return oLabAgree
        End Function

        Private Function GetLabAgreeAccrualRules(ByVal _IDLabAgree As Integer, Optional ByVal bAudit As Boolean = False) As Generic.List(Of roLabAgreeEngineAccrualRule)

            Dim oRet As New Generic.List(Of roLabAgreeEngineAccrualRule)

            Try

                Dim strSQL As String = "@SELECT# * " &
                                       "FROM LabAgreeAccrualsRules " &
                                       "WHERE IDLabAgree = " & _IDLabAgree.ToString
                Dim tb As DataTable = CreateDataTable(strSQL)

                For Each oLabAgreeRuleRow As DataRow In tb.Rows
                    Dim oRule As New roLabAgreeEngineAccrualRule()

                    oRule.IDLabAgree = roTypes.Any2Integer(oLabAgreeRuleRow("IDLabAgree"))
                    oRule.IDAccrualRule = roTypes.Any2Integer(oLabAgreeRuleRow("IdAccrualsRules"))

                    strSQL = "@SELECT# * FROM LabAgreeAccrualsRules WHERE IDLabAgree = " & oRule.IDLabAgree & " And IdAccrualsRules = " & oRule.IDAccrualRule
                    Dim oRuleTb As DataTable = CreateDataTable(strSQL)

                    If oRuleTb IsNot Nothing AndAlso oRuleTb.Rows.Count > 0 Then
                        Dim oRow As DataRow = oRuleTb.Rows(0)

                        oRule.BeginDate = oRow("BeginDate")
                        oRule.EndDate = oRow("EndDate")

                        oRule.LabAgreeRule = Me.LoadLabAgreeRule(oRule.IDAccrualRule, bAudit)
                    End If

                    If oRule.LabAgreeRule IsNot Nothing Then oRet.Add(oRule)
                Next
            Catch ex As DbException
                Me.State.UpdateStateInfo(ex, "roLabAgreeManager::GetLabAgreeAccrualRules")
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roLabAgreeManager::GetLabAgreeAccrualRules")
            Finally

            End Try

            Return oRet

        End Function

        Private Function LoadLabAgreeRule(ByVal _IDAccrualRule As Integer, Optional ByVal bAudit As Boolean = False) As roLabAgreeEngineRule

            Dim oRet As roLabAgreeEngineRule = Nothing

            Try

                Dim strSQL As String = "@SELECT# * FROM AccrualsRules WHERE IdAccrualsRule = " & _IDAccrualRule.ToString
                Dim tb As DataTable = CreateDataTable(strSQL)

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    oRet = New roLabAgreeEngineRule
                    Dim oRow As DataRow = tb.Rows(0)

                    oRet.ID = _IDAccrualRule
                    oRet.Name = roTypes.Any2String(oRow("Name"))
                    oRet.Description = roTypes.Any2String(oRow("Description"))

                    Dim strSchedule As String = roTypes.Any2String(oRow("Schedule"))
                    If strSchedule <> "" Then
                        oRet.Schedule = Me.LoadLabAgreeSchedule(strSchedule, bAudit) 'New roLabAgreeSchedule(strSchedule, oState)
                    End If

                    Dim strDefinition As String = roTypes.Any2String(oRow("Definition"))
                    If strDefinition <> "" Then
                        oRet.Definition = Me.LoadLabAgreeRuleDefinition(strDefinition, bAudit) 'New roLabAgreeRuleDefinition(New roCollection(strDefinition), oState)
                    End If

                    ' Auditar lectura
                    If bAudit Then
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        oState.AddAuditParameter(tbParameters, "{Name}", oRet.Name, "", 1)
                        Me.oState.Audit(VTBase.Audit.Action.aSelect, VTBase.Audit.ObjectType.tLabAgreeRule, oRet.Name, tbParameters, -1)
                    End If

                End If
            Catch ex As DbException
                Me.State.UpdateStateInfo(ex, "roLabAgreeManager::LoadLabAgreeRule")
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roLabAgreeManager::LoadLabAgreeRule")
            Finally

            End Try

            Return oRet

        End Function

        Private Function LoadLabAgreeSchedule(ByVal strSchedule As String, Optional ByVal bAudit As Boolean = False) As roLabAgreeEngineSchedule

            Dim oRet As roLabAgreeEngineSchedule = Nothing

            Try

                If strSchedule <> String.Empty Then

                    oRet = New roLabAgreeEngineSchedule
                    Dim arrStr() As String
                    arrStr = strSchedule.Split("@")

                    oRet.Day = 0
                    oRet.Days = 0
                    oRet.Month = 1
                    oRet.Months = 0
                    oRet.WeekDay = 1
                    oRet.Start = 0

                    If arrStr.Length > 0 Then
                        Select Case arrStr(0)
                            Case "D" ' Diari (Dies)
                                oRet.ScheduleType = LabAgreeScheduleScheduleType.Daily
                                oRet.Days = CInt(arrStr(1))
                            Case "S" ' Semanal (Dia Semana)
                                oRet.ScheduleType = LabAgreeScheduleScheduleType.Weekly
                                oRet.WeekDay = CInt(arrStr(1))
                            Case "M" ' Mensual
                                oRet.ScheduleType = LabAgreeScheduleScheduleType.Monthly
                                Select Case arrStr(1)
                                    Case "DM" 'Dia / Mes
                                        oRet.MonthlyType = LabAgreeScheduleMonthlyType.DayAndMonth
                                        oRet.Day = CInt(arrStr(2))
                                        oRet.Months = CInt(arrStr(3))
                                    Case "DS" 'Inici / DiaSem / Meses
                                        oRet.MonthlyType = LabAgreeScheduleMonthlyType.DayAndStartup
                                        oRet.Start = CInt(arrStr(2))
                                        oRet.WeekDay = CInt(arrStr(3))
                                        oRet.Months = CInt(arrStr(4))
                                End Select
                            Case "A" ' Anual (dia / mes)
                                oRet.ScheduleType = LabAgreeScheduleScheduleType.Annual
                                oRet.Day = CInt(arrStr(1))
                                oRet.Month = CInt(arrStr(2))
                        End Select
                    End If

                End If
            Catch ex As DbException
                Me.State.UpdateStateInfo(ex, "roLabAgreeManager::LoadLabAgreeSchedule")
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roLabAgreeManager::LoadLabAgreeSchedule")
            Finally

            End Try

            Return oRet

        End Function

        Private Function LoadLabAgreeRuleDefinition(ByVal strDefinition As String, Optional ByVal bAudit As Boolean = False) As roLabAgreeEngineRuleDefinition

            Dim oRet As roLabAgreeEngineRuleDefinition = Nothing

            Try

                If strDefinition <> String.Empty Then
                    Dim xDefinition = New roCollection(strDefinition)
                    oRet = New roLabAgreeEngineRuleDefinition()

                    If xDefinition.Exists("Action") Then oRet.Action = roTypes.Any2Integer(xDefinition.Item("Action"))
                    If xDefinition.Exists("Comparation") Then oRet.Comparation = roTypes.Any2Integer(xDefinition.Item("Comparation"))
                    If xDefinition.Exists("DestiAccrual") Then oRet.DestiAccrual = roTypes.Any2Integer(ExecuteScalar("@SELECT# isnull(id,0) from causes where id=" & roTypes.Any2Integer(xDefinition.Item("DestiAccrual"))))
                    If xDefinition.Exists("MainAccrual") Then oRet.MainAccrual = roTypes.Any2Integer(xDefinition.Item("MainAccrual"))

                    If xDefinition.Exists("ValueType") Then
                        oRet.ValueType = roTypes.Any2Integer(xDefinition.Item("ValueType"))
                    Else
                        Dim intSubAccrual As Integer = 0
                        If xDefinition.Exists("SubAccrual") Then intSubAccrual = roTypes.Any2Integer(xDefinition.Item("SubAccrual"))
                        If intSubAccrual = 0 Then
                            oRet.ValueType = LabAgreeRuleDefinitionValueType.DirectValue
                        ElseIf intSubAccrual > 0 Then
                            oRet.ValueType = LabAgreeRuleDefinitionValueType.ConceptValue
                            oRet.ValueIDConcept = intSubAccrual
                        End If
                    End If
                    If xDefinition.Exists("Value") Then oRet.Value = roTypes.Any2Double(xDefinition.Item("Value"))
                    If xDefinition.Exists("ValueUserField") AndAlso xDefinition.Item("ValueUserField") <> String.Empty Then
                        Dim oUserFieldState As New roUserFieldState()
                        roBusinessState.CopyTo(oState, oUserFieldState)
                        Dim oUserField As New roUserField(oUserFieldState, roTypes.Any2String(xDefinition.Item("ValueUserField")), Types.EmployeeField, False)
                        If oUserFieldState.Result = UserFieldResultEnum.NoError Then
                            oRet.ValueUserField = oUserField
                        End If
                    End If
                    If xDefinition.Exists("ValueIDConcept") Then oRet.ValueIDConcept = roTypes.Any2Integer(xDefinition.Item("ValueIDConcept"))

                    If xDefinition.Exists("Dif") Then oRet.Dif = roTypes.Any2Integer(xDefinition.Item("Dif"))
                    If xDefinition.Exists("Until") Then oRet.Until = roTypes.Any2Double(xDefinition.Item("Until"))
                    If xDefinition.Exists("UntilUserField") AndAlso xDefinition.Item("UntilUserField") <> String.Empty Then
                        Dim oUserFieldState As New roUserFieldState()
                        roBusinessState.CopyTo(oState, oUserFieldState)
                        Dim oUserField As New roUserField(oUserFieldState, roTypes.Any2String(xDefinition.Item("UntilUserField")), Types.EmployeeField, False)
                        If oUserFieldState.Result = UserFieldResultEnum.NoError Then
                            oRet.UntilUserField = oUserField
                        End If
                    End If
                End If
            Catch ex As DbException
                Me.State.UpdateStateInfo(ex, "roLabAgreeManager::LoadLabAgreeRuleDefinition")
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roLabAgreeManager::LoadLabAgreeRuleDefinition")
            Finally

            End Try

            Return oRet

        End Function

        Private Function GetLabAgreeStartupValues(ByVal _IDLabAgree As Integer, Optional ByVal bAudit As Boolean = False) As Generic.List(Of roEngineStartupValue)

            Dim oRet As New Generic.List(Of roEngineStartupValue)

            Try

                Dim strSQL As String = "@SELECT# * " &
                                       "FROM LabAgreeStartupValues " &
                                       "WHERE IDLabAgree = " & _IDLabAgree.ToString
                Dim tb As DataTable = CreateDataTable(strSQL)

                For Each oLabAgreeStartupRow As DataRow In tb.Rows
                    strSQL = "@SELECT# * FROM StartupValues WHERE IDStartupValue = " & roTypes.Any2Integer(oLabAgreeStartupRow("IDStartupValue")).ToString

                    Dim tbStartup As DataTable = CreateDataTable(strSQL)

                    If tbStartup IsNot Nothing AndAlso tbStartup.Rows.Count > 0 Then
                        Dim oStartupValue As New roEngineStartupValue
                        Dim oRow As DataRow = tbStartup.Rows(0)

                        oStartupValue.ID = roTypes.Any2Integer(oRow("IDStartupValue"))
                        oStartupValue.OriginalIDConcept = roTypes.Any2Integer(oRow("IDConcept"))
                        oStartupValue.IDConcept = roTypes.Any2Integer(oRow("IDConcept"))
                        oStartupValue.Name = roTypes.Any2String(oRow("Name"))

                        oStartupValue.ScalingUserField = roTypes.Any2String(oRow("ScalingUserField"))
                        oStartupValue.ScalingCoefficientUserField = roTypes.Any2String(oRow("ScalingCoefficientUserField"))
                        oStartupValue.ScalingFieldValues = New Generic.List(Of roEngineScalingValues)
                        Dim strStartUpFieldValues = roTypes.Any2String(oRow("ScalingValues")).Split(New Char() {"@"c}, StringSplitOptions.RemoveEmptyEntries)

                        For Each value As String In strStartUpFieldValues
                            oStartupValue.ScalingFieldValues.Add(New roEngineScalingValues() With {.UserField = value.Split("#")(0), .AccumValue = value.Split("#")(1)})
                        Next

                        oStartupValue.CalculatedType = roTypes.Any2Integer(oRow("CalculatedType"))

                        If oRow("StartValueType") IsNot DBNull.Value Then
                            Select Case oRow("StartValueType")
                                Case 0 'None
                                    oStartupValue.StartValueType = LabAgreeValueType.None
                                Case 1 'DirectValue
                                    oStartupValue.StartValueType = LabAgreeValueType.DirectValue
                                    oStartupValue.StartValue = roTypes.Any2Double(roTypes.Any2String(oRow("StartValue")).Replace(".", roConversions.GetDecimalDigitFormat()))
                                Case 2 'UserField
                                    oStartupValue.StartValueType = LabAgreeValueType.UserField

                                    Dim oUFState As New roUserFieldState()
                                    roBusinessState.CopyTo(oState, oUFState)
                                    Dim oUFStartup As New roUserField(oUFState, oRow("StartValue"), Types.EmployeeField, False, False)
                                    If oUFState.Result = UserFieldResultEnum.NoError Then
                                        oStartupValue.StartUserField = oUFStartup
                                    End If
                                Case 3 'CalculatedValue
                                    oStartupValue.StartValueType = LabAgreeValueType.CalculatedValue

                                    ' Valor Base
                                    If roTypes.Any2Integer(oRow("StartValueBaseType")) = 0 Then
                                        ' Directo
                                        oStartupValue.StartValueBaseType = LabAgreeValueTypeBase.DirectValue
                                        oStartupValue.StartValueBase = roTypes.Any2Double(roTypes.Any2String(oRow("StartValueBase")).Replace(".", roConversions.GetDecimalDigitFormat()))
                                    Else
                                        ' Campo ficha
                                        oStartupValue.StartValueBaseType = LabAgreeValueTypeBase.UserField
                                        Dim oUFState As New roUserFieldState()
                                        roBusinessState.CopyTo(oState, oUFState)
                                        Dim oUFStartup As New roUserField(oUFState, oRow("StartValueBase"), Types.EmployeeField, False, False)
                                        If oUFState.Result = UserFieldResultEnum.NoError Then
                                            oStartupValue.StartUserFieldBase = oUFStartup
                                        End If
                                    End If

                                    ' Total Base
                                    If roTypes.Any2Integer(oRow("TotalPeriodBaseType")) = 0 Then
                                        ' Directo
                                        oStartupValue.TotalPeriodBaseType = LabAgreeValueTypeBase.DirectValue
                                        oStartupValue.TotalPeriodBase = roTypes.Any2Double(roTypes.Any2String(oRow("TotalPeriodBase")).Replace(".", roConversions.GetDecimalDigitFormat()))
                                    Else
                                        ' Campo ficha
                                        oStartupValue.TotalPeriodBaseType = LabAgreeValueTypeBase.UserField
                                        Dim oUFState As New roUserFieldState()
                                        roBusinessState.CopyTo(oState, oUFState)
                                        Dim oUFStartup As New roUserField(oUFState, oRow("TotalPeriodBase"), Types.EmployeeField, False, False)
                                        If oUFState.Result = UserFieldResultEnum.NoError Then
                                            oStartupValue.StartUserFieldTotalPeriodBase = oUFStartup
                                        End If
                                    End If

                                    ' Valor Devengado
                                    If roTypes.Any2Integer(oRow("AccruedValueType")) = 0 Then
                                        ' Directo
                                        oStartupValue.AccruedValueType = LabAgreeValueTypeBase.DirectValue
                                        oStartupValue.AccruedValue = roTypes.Any2Double(roTypes.Any2String(oRow("AccruedValue")).Replace(".", roConversions.GetDecimalDigitFormat()))
                                    Else
                                        ' Campo ficha
                                        oStartupValue.AccruedValueType = LabAgreeValueTypeBase.UserField
                                        Dim oUFState As New roUserFieldState()
                                        roBusinessState.CopyTo(oState, oUFState)
                                        Dim oUFStartup As New roUserField(oUFState, oRow("AccruedValue"), Types.EmployeeField, False, False)
                                        If oUFState.Result = UserFieldResultEnum.NoError Then
                                            oStartupValue.StartUserFieldAccruedValue = oUFStartup
                                        End If
                                    End If

                                    ' Redondeo
                                    oStartupValue.RoundingType = roTypes.Any2Integer(oRow("RoundingType"))

                                    oStartupValue.ApplyEndCustomPeriod = False

                                    If Not IsDBNull(oRow("ApplyEndCustomPeriod")) AndAlso roTypes.Any2Boolean(oRow("ApplyEndCustomPeriod")) Then
                                        oStartupValue.ApplyEndCustomPeriod = True
                                        Dim oUFState As New roUserFieldState()
                                        roBusinessState.CopyTo(oState, oUFState)
                                        Dim oEndCustomPeriodUserField As New roUserField(oUFState, oRow("EndCustomPeriodUserField"), Types.EmployeeField, False, False)
                                        oStartupValue.EndCustomPeriodUserField = oEndCustomPeriodUserField
                                    End If

                            End Select

                            If roTypes.Any2Integer(oRow("StartValueType")) = 1 OrElse roTypes.Any2Integer(oRow("StartValueType")) = 2 OrElse roTypes.Any2Integer(oRow("StartValueType")) = 3 Then
                                oStartupValue.NewContractException = roTypes.Any2Boolean(oRow("NewContractException"))

                                Dim oUserFieldState As New roUserFieldState
                                roBusinessState.CopyTo(Me.oState, oUserFieldState)

                                oStartupValue.NewContractExceptionCriteria = roUserFieldCondition.LoadFromXml(roTypes.Any2String(oRow("NewContractExceptionCondition")), oUserFieldState, False)
                            Else
                                oStartupValue.NewContractException = False
                                oStartupValue.NewContractExceptionCriteria = New Generic.List(Of roUserFieldCondition)
                            End If
                        Else
                            oStartupValue.StartValueType = LabAgreeValueType.None
                        End If

                        If oRow("MaximumValueType") IsNot DBNull.Value Then
                            Select Case oRow("MaximumValueType")
                                Case 0 'None
                                    oStartupValue.MaximumValueType = LabAgreeValueType.None
                                Case 1 'DirectValue
                                    oStartupValue.MaximumValueType = LabAgreeValueType.DirectValue
                                    oStartupValue.MaximumValue = roTypes.Any2Double(roTypes.Any2String(oRow("MaximumValue")).Replace(".", roConversions.GetDecimalDigitFormat()))
                                Case 2 'UserField
                                    oStartupValue.MaximumValueType = LabAgreeValueType.UserField

                                    Dim oUFState As New roUserFieldState()
                                    roBusinessState.CopyTo(oState, oUFState)
                                    Dim oUFMaximum As New roUserField(oUFState, oRow("MaximumValue"), Types.EmployeeField, False, False)
                                    If oUFState.Result = UserFieldResultEnum.NoError Then
                                        oStartupValue.MaximumUserField = oUFMaximum
                                    End If

                                    oStartupValue.MaximumUserField = oUFMaximum
                            End Select
                        Else
                            oStartupValue.MaximumValueType = LabAgreeValueType.None
                        End If

                        If oRow("MinimumValueType") IsNot DBNull.Value Then
                            Select Case oRow("MinimumValueType")
                                Case 0 'None
                                    oStartupValue.MinimumValueType = LabAgreeValueType.None
                                Case 1 'DirectValue
                                    oStartupValue.MinimumValueType = LabAgreeValueType.DirectValue
                                    oStartupValue.MinimumValue = roTypes.Any2Double(roTypes.Any2String(oRow("MinimumValue")).Replace(".", roConversions.GetDecimalDigitFormat()))
                                Case 2 'UserField
                                    oStartupValue.MinimumValueType = LabAgreeValueType.UserField

                                    Dim oUFState As New roUserFieldState()
                                    roBusinessState.CopyTo(oState, oUFState)
                                    Dim oUFMinimum As New roUserField(oUFState, oRow("MinimumValue"), Types.EmployeeField, False, False)
                                    If oUFState.Result = UserFieldResultEnum.NoError Then
                                        oStartupValue.MinimumUserField = oUFMinimum
                                    End If

                                    oStartupValue.MinimumUserField = oUFMinimum
                            End Select
                        Else
                            oStartupValue.MinimumValueType = LabAgreeValueType.None
                        End If

                        ' Caducidad
                        If oStartupValue.StartValueType <> LabAgreeValueType.None AndAlso oRow("ExpirationValue") IsNot DBNull.Value Then
                            oStartupValue.Expiration = New roEngineStartupValueExpirationRule
                            oStartupValue.Expiration.Unit = roTypes.Any2Integer(oRow("ExpirationUnit"))
                            oStartupValue.Expiration.ExpireAfter = roTypes.Any2Integer(oRow("ExpirationValue"))
                        End If

                        ' Disfrute
                        If oStartupValue.StartValueType <> LabAgreeValueType.None AndAlso oRow("StartEnjoymentValue") IsNot DBNull.Value Then
                            oStartupValue.Enjoyment = New roEngineStartupValueEnjoymentRule
                            oStartupValue.Enjoyment.Unit = roTypes.Any2Integer(oRow("EnjoymentUnit"))
                            oStartupValue.Enjoyment.StartAfter = roTypes.Any2Integer(oRow("StartEnjoymentValue"))
                        End If

                        oRet.Add(oStartupValue)
                    End If
                Next
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roLabAgreeManager::GetLabAgreeStartupValues")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roLabAgreeManager::GetLabAgreeStartupValues")
            Finally

            End Try

            Return oRet

        End Function

        Private Function GetLabAgreeCauseLimitValues(ByVal _IDLabAgree As Integer, Optional ByVal bAudit As Boolean = False) As Generic.List(Of roLabAgreeEngineCauseLimitValues)

            Dim oRet As New Generic.List(Of roLabAgreeEngineCauseLimitValues)

            Try

                Dim strSQL As String = "@SELECT# * " &
                                       "FROM LabAgreeCauseLimitValues " &
                                       "WHERE IDLabAgree = " & _IDLabAgree.ToString
                Dim tb As DataTable = CreateDataTable(strSQL)

                For Each oLabAgreeCauseLimitRow As DataRow In tb.Rows
                    Dim oCauseValue As New roLabAgreeEngineCauseLimitValues() '(oLabAgreeCauseLimitRow("IDLabAgree"), oLabAgreeCauseLimitRow("IDCauseLimitValue"), _State, bAudit)

                    oCauseValue.IDLabAgree = roTypes.Any2Integer(oLabAgreeCauseLimitRow("IDLabAgree"))
                    oCauseValue.IDCauseLimitValue = roTypes.Any2Integer(oLabAgreeCauseLimitRow("IDCauseLimitValue"))

                    strSQL = "@SELECT# * FROM LabAgreeCauseLimitValues " &
                                       "WHERE IDLabAgree = " & oCauseValue.IDLabAgree & " And IDCauseLimitValue = " & oCauseValue.IDCauseLimitValue
                    Dim tbLimits As DataTable = CreateDataTable(strSQL)

                    If tbLimits IsNot Nothing AndAlso tbLimits.Rows.Count > 0 Then
                        Dim oRow As DataRow = tbLimits.Rows(0)

                        oCauseValue.BeginDate = oRow("BeginDate")
                        oCauseValue.EndDate = oRow("EndDate")

                        oCauseValue.CauseLimitValue = Me.LoadCauseLimitValue(oCauseValue.IDCauseLimitValue, bAudit)

                        ' Auditar lectura
                        If bAudit Then
                            Dim tbParameters As DataTable = oState.CreateAuditParameters()
                            Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tLabAgreeCauseLimitValue, oCauseValue.CauseLimitValue.Name, tbParameters, -1)
                        End If

                        oRet.Add(oCauseValue)
                    End If
                Next
            Catch ex As DbException
                Me.State.UpdateStateInfo(ex, "roLabAgreeManager::GetLabAgreeCauseLimitValues")
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roLabAgreeManager::GetLabAgreeCauseLimitValues")
            Finally

            End Try

            Return oRet

        End Function

        Private Function LoadCauseLimitValue(ByVal _IDCauseLimitValue As Integer, Optional ByVal bAudit As Boolean = False) As roEngineCauseLimitValue

            Dim oRet As roEngineCauseLimitValue = Nothing

            Try

                Dim strSQL As String = "@SELECT# * FROM CauseLimitValues " &
                                       "WHERE IDCauseLimitValue = " & _IDCauseLimitValue.ToString
                Dim tb As DataTable = CreateDataTable(strSQL)

                If tb IsNot Nothing AndAlso tb.Rows.Count = 1 Then
                    Dim oRow As DataRow = tb.Rows(0)
                    oRet = New roEngineCauseLimitValue() '(oLabAgreeCauseLimitRow("IDLabAgree"), oLabAgreeCauseLimitRow("IDCauseLimitValue"), _State, bAudit)

                    oRet.IDCauseLimitValue = roTypes.Any2Integer(oRow("IDCauseLimitValue"))
                    oRet.OriginalIDCause = roTypes.Any2Integer(oRow("IDCause"))
                    oRet.IDCause = roTypes.Any2Integer(oRow("IDCause"))
                    oRet.Name = roTypes.Any2String(oRow("Name"))
                    oRet.IDExcessCause = roTypes.Any2Integer(oRow("IDExcessCause"))

                    If oRow("MaximumAnnualValueType") IsNot DBNull.Value Then
                        Select Case oRow("MaximumAnnualValueType")
                            Case 0 'None
                                oRet.MaximumAnnualValueType = LabAgreeValueType.None
                            Case 1 'DirectValue
                                oRet.MaximumAnnualValueType = LabAgreeValueType.DirectValue
                                oRet.MaximumAnnualValue = roTypes.Any2Double(roTypes.Any2String(oRow("MaximumAnnualValue")).Replace(".", roConversions.GetDecimalDigitFormat()))
                            Case 2 'UserField
                                oRet.MaximumAnnualValueType = LabAgreeValueType.UserField

                                Dim oUFState As New roUserFieldState()
                                roBusinessState.CopyTo(oState, oUFState)
                                Dim oUFStartup As New roUserField(oUFState, oRow("MaximumAnnualValue"), Types.EmployeeField, False, False)
                                If oUFState.Result = UserFieldResultEnum.NoError Then
                                    oRet.MaximumAnnualField = oUFStartup
                                End If
                        End Select
                    Else
                        oRet.MaximumAnnualValueType = LabAgreeValueType.None
                    End If

                    If oRow("MaximumMonthlyType") IsNot DBNull.Value Then
                        Select Case oRow("MaximumMonthlyType")
                            Case 0 'None
                                oRet.MaximumMonthlyType = LabAgreeValueType.None
                            Case 1 'DirectValue
                                oRet.MaximumMonthlyType = LabAgreeValueType.DirectValue
                                oRet.MaximumMonthlyValue = roTypes.Any2Double(roTypes.Any2String(oRow("MaximumMonthlyValue")).Replace(".", roConversions.GetDecimalDigitFormat()))
                            Case 2 'UserField
                                oRet.MaximumMonthlyType = LabAgreeValueType.UserField

                                Dim oUFState As New roUserFieldState()
                                roBusinessState.CopyTo(oState, oUFState)
                                Dim oUFMaximumMonth As New roUserField(oUFState, oRow("MaximumMonthlyValue"), Types.EmployeeField, False, False)
                                If oUFState.Result = UserFieldResultEnum.NoError Then
                                    oRet.MaximumMonthlyField = oUFMaximumMonth
                                End If
                        End Select
                    Else
                        oRet.MaximumMonthlyType = LabAgreeValueType.None
                    End If

                    ' Auditar lectura
                    If bAudit Then
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        oState.AddAuditParameter(tbParameters, "{Name}", oRet.Name, "", 1)
                        Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tCauseLimitValue, oRet.Name, tbParameters, -1)
                    End If

                End If
            Catch ex As DbException
                Me.State.UpdateStateInfo(ex, "roLabAgreeManager::GetLabAgreeCauseLimitValues")
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roLabAgreeManager::GetLabAgreeCauseLimitValues")
            Finally

            End Try

            Return oRet

        End Function

        Public Function CauseLimitExecuteToday(ByVal oLabAgreeEngineCauseLimitValues As roLabAgreeEngineCauseLimitValues, ByVal p_rtTaskDate As Date) As Boolean
            '
            ' Devuelve si ese dia se tenia que ejecutar el limite por justificacion
            '

            Dim bRet As Boolean = True

            Try
                ' Primero validamos el período de validez del límite de justificación
                bRet = (p_rtTaskDate <= oLabAgreeEngineCauseLimitValues.EndDate AndAlso p_rtTaskDate >= oLabAgreeEngineCauseLimitValues.BeginDate)
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roLabAgreeManager::ExecuteToday")
                bRet = False
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roLabAgreeManager::ExecuteToday")
                bRet = False
            End Try

            Return bRet

        End Function

        ''' <summary>
        ''' Función usada en motores de cálculo para saber si una determinada regla de arrastre aplica en una fecha
        ''' </summary>
        ''' <param name="dDate"></param>
        ''' <returns></returns>
        Public Function AccrualRuleApplyOnDate(ByVal IDEmployee As Integer, oAccrualRule As roLabAgreeEngineAccrualRule, dDate As Date, ByRef DBAnnualWorkPeriod As DataTable) As Boolean
            Dim bRet As Boolean = False
            Dim iDayOfMonth As Integer
            Dim iMonthDiff As Integer
            Dim dFirstExecDate As Date

            Try

                ' Primero validamos el período de validez de la regla de acumulado
                If Not (dDate <= oAccrualRule.EndDate AndAlso dDate >= oAccrualRule.BeginDate) Then
                    Return False
                End If

                If oAccrualRule.LabAgreeRule.Schedule.ToString = "" Then Return False

                Dim oSchedule As New roLabAgreeEngineSchedule
                oSchedule = oAccrualRule.LabAgreeRule.Schedule

                Select Case oSchedule.ScheduleType
                    Case LabAgreeScheduleScheduleType.Daily
                        If DateDiff("d", oAccrualRule.BeginDate, dDate) Mod roTypes.Any2Long(oSchedule.Days) = 0 Then
                            bRet = True
                        End If
                    Case LabAgreeScheduleScheduleType.Weekly
                        If IIf(dDate.DayOfWeek = DayOfWeek.Sunday, 7, dDate.DayOfWeek) = roTypes.Any2Long(oSchedule.WeekDay) Then
                            bRet = True
                        End If
                    Case LabAgreeScheduleScheduleType.Monthly
                        If oSchedule.MonthlyType = LabAgreeScheduleMonthlyType.DayAndMonth Then
                            ' Primero validamos si es el día en cuestión
                            If roTypes.Any2Long(oSchedule.Day) <> 32 Then
                                iDayOfMonth = roTypes.Any2Long(oSchedule.Day)
                            Else
                                iDayOfMonth = getLastDayOfMonth(dDate.Date).Day
                            End If
                            If dDate.Day = iDayOfMonth Then
                                ' Luego validamos que han pasado los mese indicados

                                ' Primero miramos cual fue el primer mes que se ejecutó,
                                ' en función de la fecha de inicio de validez de la regla
                                If oAccrualRule.BeginDate.Day >= dDate.Day Then
                                    ' 1. Si el día de aplicación de la regla es anterior al de inicio de vigencia,
                                    ' el primer mes ejecución será el siguiente al de inicio de validez de la regla
                                    dFirstExecDate = oAccrualRule.BeginDate.AddMonths(1)
                                Else
                                    ' 2. Si el día de aplicación de la regla es posterior al de inicio de vigencia,
                                    ' el primer mes de ejecución de la regla es el mismo de la fecha de inicio de vigencia
                                    dFirstExecDate = oAccrualRule.BeginDate
                                End If

                                'Calculo la diferencia en meses entre la primera ejecución y la fecha de la tarea
                                iMonthDiff = (dDate.Year - dFirstExecDate.Year) * 12 + dDate.Month - dFirstExecDate.Month

                                If iMonthDiff Mod roTypes.Any2Long(oSchedule.Months) = 0 Then
                                    ' Toca calcular la regla
                                    bRet = True
                                End If
                            End If
                        Else
                            ' Cada n día de la semana de cada m meses (tercer jueves de cada 3 meses)
                            If oSchedule.MonthlyType = LabAgreeScheduleMonthlyType.DayAndStartup Then

                                ' Calculamos el día del més al que corresponden los datos
                                iDayOfMonth = getNthWeekdayOfMonth(dDate.Date, roTypes.Any2Long(oSchedule.Start), roTypes.Any2Long(oSchedule.WeekDay)).Day

                                'Calculo la diferencia en meses entre la primera ejecución y la fecha de la tarea
                                iMonthDiff = (dDate.Year - dFirstExecDate.Year) * 12 + dDate.Month - dFirstExecDate.Month

                                'Finalmente comprobamos si es el día y el mes concreto para lanzar el cálculo de la regla
                                If (dDate.Day = iDayOfMonth) AndAlso iMonthDiff Mod roTypes.Any2Long(oSchedule.Months) = 0 Then
                                    bRet = True
                                End If
                            End If
                        End If
                    Case LabAgreeScheduleScheduleType.Annual
                        If oSchedule.Day = 0 AndAlso oSchedule.Month = 0 Then
                            ' En el caso que sea el ultimo dia del periodo
                            ' debemos revisar que la fecha a procesar sea la misma que el ultimo dia de un tramo del usuario
                            'If DBAnnualWorkPeriod Is Nothing Then
                            '    Dim strSQL As String = "@SELECT# * FROM sysrovwEmployeesAnnualWorkPeriods with (nolock) WHERE IDEmployee = " & IDEmployee.ToString & " Order by  BeginPeriod "
                            '    DBAnnualWorkPeriod = CreateDataTable(strSQL)
                            'End If
                            'If DBAnnualWorkPeriod IsNot Nothing AndAlso DBAnnualWorkPeriod.Rows.Count > 0 Then
                            '    Dim oRow As DataRow() = DBAnnualWorkPeriod.Select("BeginPeriod<='" & Format(dDate, "yyyy/MM/dd") & "' AND EndPeriod >= '" & Format(dDate, "yyyy/MM/dd") & "'")
                            '    If oRow.Length > 0 Then
                            '        If roTypes.Any2Time(oRow(0)("EndPeriod")).Value = roTypes.Any2Time(dDate).Value Then
                            '            bRet = True
                            '        End If
                            '    End If

                            'End If

                        Else
                            ' En el caso que sea un dia concreto del año
                            If dDate.Day = roTypes.Any2Long(oSchedule.Day) AndAlso dDate.Month = roTypes.Any2Long(oSchedule.Month) Then
                                bRet = True
                            End If
                        End If
                    Case Else
                        bRet = False
                End Select
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roLabAgreeManager::AccrualRuleApplyOnDate", ex)
            End Try

            Return bRet
        End Function

        Private Function getLastDayOfMonth(ByVal pdate As Date, Optional ByVal pDif As Integer = 0) As Date
            '
            '   Calcula el ultimo dia del mes.
            '
            getLastDayOfMonth = DateAdd("d", -1, getFirstDayOfMonth(DateAdd("m", 1, pdate), 0))

        End Function

        Private Function getFirstDayOfMonth(ByVal pdate As Date, Optional ByVal pDif As Integer = 0) As Date
            '
            '   Calcula el primer dia del mes
            '
            getFirstDayOfMonth = "1/" & DatePart("m", pdate) - pDif & "/" & DatePart("yyyy", pdate)

        End Function

        Private Function getFirstDayOfWeek(ByVal pdate As Date, Optional ByVal pDif As Integer = 0) As Date
            '
            '   Calcula el primer dia del mes
            '

            getFirstDayOfWeek = DateAdd("d", (-1 * Microsoft.VisualBasic.Weekday(pdate)) + 2 + (pDif * 7), pdate)

        End Function

        Private Function getLastDayOfWeek(ByVal pdate As Date, Optional ByVal pDif As Integer = 0) As Date
            '
            '   Calcula el primer dia del mes
            '
            getLastDayOfWeek = DateAdd("d", 6, getFirstDayOfWeek(pdate, pDif))

        End Function

        Private Function getNthWeekdayOfMonth(ByVal pdate As Date, ByVal iNthWeekDay As Integer, ByVal iWeekDay As Integer) As Date
            '
            '   Calcula el enésimo día de la semana de un mes dado (ejemplo: 3er martes del mes --> iNthWeekDay=3, iWeekDay)
            '

            Dim iWeekdayFirstDayOfMonth As Integer
            Dim tmpDate As Date
            Dim bolBeforeDay As Boolean

            If iNthWeekDay < 5 Then
                'El 1,2,3,4 dia de la semana

                ' ¿Qué día de la semana es el primer día del mes?
                iWeekdayFirstDayOfMonth = Microsoft.VisualBasic.Weekday(getFirstDayOfMonth(pdate), vbMonday)

                ' Calculamos qué día del mes que corresponde
                If iWeekdayFirstDayOfMonth > iWeekDay Then
                    getNthWeekdayOfMonth = DateAdd("d", iWeekDay - iWeekdayFirstDayOfMonth + 7 * iNthWeekDay, getFirstDayOfMonth(pdate))
                Else
                    getNthWeekdayOfMonth = DateAdd("d", iWeekDay - iWeekdayFirstDayOfMonth + 7 * (iNthWeekDay - 1), getFirstDayOfMonth(pdate))
                End If
            Else
                ' El ultimo dia de la semana de un mes

                ' Obtenemos el ultimo dia del mes de la fecha indicada
                tmpDate = getLastDayOfMonth(pdate)

                ' Vamos hacia atrás hasta encontrar el dia de la semana que corresponda al indicado
                bolBeforeDay = True
                While bolBeforeDay
                    If Microsoft.VisualBasic.Weekday(tmpDate, vbMonday) = iWeekDay Then
                        bolBeforeDay = False
                    Else
                        tmpDate = DateAdd("d", -1, tmpDate)
                    End If
                End While
                getNthWeekdayOfMonth = tmpDate
            End If
        End Function

        Public Shared Function CloneLabAgreeAccrualRule(ByVal oLabAgreeEngineAccrualRule As roLabAgreeEngineAccrualRule) As roLabAgreeEngineAccrualRule
            Dim oNewLabAgreeEngineAccrualRule As New roLabAgreeEngineAccrualRule
            oNewLabAgreeEngineAccrualRule.BeginDate = oLabAgreeEngineAccrualRule.BeginDate
            oNewLabAgreeEngineAccrualRule.EndDate = oLabAgreeEngineAccrualRule.EndDate
            oNewLabAgreeEngineAccrualRule.IDLabAgree = oLabAgreeEngineAccrualRule.IDLabAgree
            oNewLabAgreeEngineAccrualRule.LabAgreeRule = oLabAgreeEngineAccrualRule.LabAgreeRule
            oNewLabAgreeEngineAccrualRule.IDAccrualRule = oLabAgreeEngineAccrualRule.IDAccrualRule

            Return oNewLabAgreeEngineAccrualRule
        End Function

#End Region

    End Class

End Namespace