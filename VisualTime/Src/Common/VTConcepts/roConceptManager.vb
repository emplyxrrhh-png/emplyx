Imports System.Data.Common
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase

Namespace Concepts

    Public Class roConceptManager
        Private oState As roConceptManagerState = Nothing

        Public ReadOnly Property State As roConceptManagerState
            Get
                Return oState
            End Get
        End Property

        Public Sub New()
            Me.oState = New roConceptManagerState()
        End Sub

        Public Sub New(ByVal _State As roConceptManagerState)
            Me.oState = _State
        End Sub

#Region "Methods"

        Public Function Load(ByVal IDConcept As Integer, Optional ByVal bAudit As Boolean = False) As roConceptEngine

            Dim oConcept As roConceptEngine = Nothing
            Try

                Dim strQuery As String = "@SELECT# * FROM Concepts WHERE ID = " & IDConcept.ToString

                Dim oDt As DataTable = CreateDataTable(strQuery)

                If oDt IsNot Nothing AndAlso oDt.Rows.Count = 1 Then
                    oConcept = New roConceptEngine
                    Dim oRow As DataRow = oDt.Rows(0)

                    oConcept.ID = IDConcept
                    oConcept.Name = roTypes.Any2String(oRow("Name"))
                    oConcept.Description = roTypes.Any2String(oRow("Description"))
                    oConcept.Color = IIf(Not IsDBNull(oRow("Color")), oRow("Color"), 0)
                    oConcept.IDType = roTypes.Any2String(oRow("IDType"))
                    oConcept.ShortName = roTypes.Any2String(oRow("ShortName"))
                    oConcept.BeginDate = oRow("BeginDate")
                    oConcept.FinishDate = oRow("FinishDate")
                    oConcept.ViewInEmployees = oRow("ViewInEmployees")
                    If Not IsDBNull(oRow("ViewInTerminals")) Then
                        oConcept.ViewInTerminals = oRow("ViewInTerminals")
                    End If
                    If Not IsDBNull(oRow("ViewInPays")) Then
                        oConcept.ViewInPays = oRow("ViewInPays")
                    End If
                    If Not IsDBNull(oRow("FixedPay")) Then
                        oConcept.FixedPay = oRow("FixedPay")
                    End If
                    If Not IsDBNull(oRow("PayValue")) Then
                        oConcept.PayValue = CDbl(oRow("PayValue"))
                    End If
                    oConcept.UsedField = roTypes.Any2String(oRow("UsedField"))
                    If Not IsDBNull(oRow("RoundingBy")) Then
                        oConcept.RoundingBy = CDbl(oRow("RoundingBy"))
                    End If
                    If Not IsDBNull(oRow("Export")) Then
                        oConcept.Export = oRow("Export")
                    End If
                    oConcept.DefaultQuery = roTypes.Any2String(oRow("DefaultQuery"))
                    If Not IsDBNull(oRow("IsExported")) Then
                        oConcept.IsExported = oRow("IsExported")
                    End If
                    If Not IsDBNull(oRow("IsIntervaled")) Then
                        oConcept.IsIntervaled = oRow("IsIntervaled")
                    End If
                    If Not IsDBNull(oRow("RoundConceptBy")) Then
                        oConcept.RoundConceptBy = CDbl(oRow("RoundConceptBy"))
                    End If
                    Select Case roTypes.Any2String(oRow("RoundConceptType"))
                        Case "+"
                            oConcept.RoundConveptType = eRoundingType.Round_UP
                        Case "~"
                            oConcept.RoundConveptType = eRoundingType.Round_Near
                        Case "-"
                            oConcept.RoundConveptType = eRoundingType.Round_Down
                    End Select
                    If Not IsDBNull(oRow("IsAbsentiism")) Then
                        oConcept.IsAbsentiism = oRow("IsAbsentiism")
                    End If
                    If Not IsDBNull(oRow("AbsentiismRewarded")) Then
                        oConcept.AbsentiismRewarded = oRow("AbsentiismRewarded")
                    End If
                    If Not IsDBNull(oRow("IsAccrualWork")) Then
                        oConcept.IsAccrualWork = oRow("IsAccrualWork")
                    End If
                    oConcept.EmployeesPermission = roTypes.Any2Integer(oRow("EmployeesPermission"))

                    'Dim oUserFieldState As New VTUserFields.UserFields.roUserFieldState
                    'roBusinessState.CopyTo(Me.oState, oUserFieldState)
                    'Me.lstEmployeesConditions = VTUserFields.UserFields.roUserFieldCondition.LoadFromXml(roTypes.Any2String(oRow("EmployeesCriteria")), oUserFieldState, False)
                    'roBusinessState.CopyTo(oUserFieldState, Me.oState)
                    oConcept.EmployeesConditions = VTUserFields.UserFields.roUserFieldCondition.LoadFromXml(roTypes.Any2String(oRow("EmployeesCriteria")), New VTUserFields.UserFields.roUserFieldState(), False)

                    oConcept.Composition = Me.GetConceptCompositions(oConcept.ID, bAudit)

                    ' Parámetros avanzados en descripcion
                    Try
                        If InStr(roTypes.Any2String(oRow("Description")), "#") > 0 Then
                            oConcept.AdvParameter = Mid(roTypes.Any2String(oRow("Description")), InStr(roTypes.Any2String(oRow("Description")), "#"), 4)
                        End If
                    Catch ex As Exception
                        oConcept.AdvParameter = ""
                    End Try


                    oConcept.ApplyExpiredHours = False
                    If Not IsDBNull(oRow("ApplyExpiredHours")) Then
                        oConcept.ApplyExpiredHours = oRow("ApplyExpiredHours")
                    End If

                    oConcept.ExpiredIDCause = roTypes.Any2Integer(oRow("ExpiredIDCause"))
                    If oConcept.ApplyExpiredHours Then
                        oConcept.ExpiredHoursCriteria = New roEngineExpiredHoursCriteria

                        Dim criteriaXml = roTypes.Any2String(oRow("ExpiredHoursCriteria"))
                        If criteriaXml <> "" Then
                            ' Añadimos la composición a la colección
                            Dim oDefinition As New roCollection(criteriaXml)

                            If oDefinition.Exists("Value") Then
                                oConcept.ExpiredHoursCriteria.Value = oDefinition("Value")
                            End If

                            If oDefinition.Exists("ExpiredHoursType") Then
                                oConcept.ExpiredHoursCriteria.ExpiredHoursType = roTypes.Any2Integer(oDefinition("ExpiredHoursType"))
                            End If

                            oConcept.ExpiredHoursCriteria.LabAgreementsAffected = New List(Of Integer)
                            If oDefinition.Exists("LabAgreementsAffected") Then
                                Dim strLabAgreementsAffected As String = roTypes.Any2String(oDefinition("LabAgreementsAffected"))
                                If strLabAgreementsAffected.Trim.Length > 0 Then
                                    oConcept.ExpiredHoursCriteria.LabAgreementsAffected = roTypes.Any2String(oDefinition("LabAgreementsAffected")).Split(",").Select(Function(s) Convert.ToInt32(s)).ToList()
                                End If
                            End If
                        End If
                    End If

                    oConcept.AutomaticAccrualType = roTypes.Any2Integer(oRow("AutomaticAccrualType"))
                    oConcept.AutomaticAccrualIDCause = roTypes.Any2Integer(oRow("AutomaticAccrualIDCause"))
                    If oConcept.AutomaticAccrualType <> eAutomaticAccrualType.DeactivatedType Then
                        oConcept.AutomaticAccrualCriteria = New roEngineAutomaticAccrualCriteria
                        oConcept.AutomaticAccrualCriteria.AutomaticAccrualType = oConcept.AutomaticAccrualType
                        Dim strXmlCriteria As String = roTypes.Any2String(oRow("AutomaticAccrualCriteria"))
                        If strXmlCriteria <> "" Then

                            ' Añadimos la composición a la colección
                            Dim oDefinition As New roCollection(strXmlCriteria)

                            If oDefinition.Exists("FactorType") Then
                                oConcept.AutomaticAccrualCriteria.FactorType = oDefinition("FactorType")
                            End If

                            If oDefinition.Exists("FactorValue") And oConcept.AutomaticAccrualCriteria.FactorType = eFactorType.DirectValue Then
                                oConcept.AutomaticAccrualCriteria.FactorValue = oDefinition("FactorValue")
                            End If

                            If oDefinition.Exists("FactorField") And oConcept.AutomaticAccrualCriteria.FactorType = eFactorType.UserField Then
                                oConcept.AutomaticAccrualCriteria.UserFieldName = roTypes.Any2String(oDefinition("FactorField"))
                                oConcept.AutomaticAccrualCriteria.UserField = New VTUserFields.UserFields.roUserField(New VTUserFields.UserFields.roUserFieldState(), oDefinition("FactorField"), UserFieldsTypes.Types.EmployeeField, False)
                            End If

                            If oDefinition.Exists("TypeAccrualDay") Then
                                oConcept.AutomaticAccrualCriteria.TypeAccrualDay = oDefinition("TypeAccrualDay")
                            End If

                            If oDefinition.Exists("TotalCauses") Then
                                oConcept.AutomaticAccrualCriteria.TotalCauses = oDefinition("TotalCauses")
                            End If

                            If oDefinition.Exists("TotalShifts") Then
                                oConcept.AutomaticAccrualCriteria.TotalShifts = oDefinition("TotalShifts")
                            End If

                            If oConcept.AutomaticAccrualCriteria.TotalCauses > 0 Then
                                For i As Integer = 1 To oConcept.AutomaticAccrualCriteria.TotalCauses
                                    If oDefinition.Exists("CauseCriteria_" & i.ToString) Then
                                        oConcept.AutomaticAccrualCriteria.Causes.Add(roTypes.Any2Integer(oDefinition("CauseCriteria_" & i.ToString)))
                                    End If
                                Next
                            End If

                            If oConcept.AutomaticAccrualCriteria.TotalShifts > 0 Then
                                For i As Integer = 1 To oConcept.AutomaticAccrualCriteria.TotalShifts
                                    If oDefinition.Exists("ShiftCriteria_" & i.ToString) Then
                                        oConcept.AutomaticAccrualCriteria.Shifts.Add(roTypes.Any2Integer(oDefinition("ShiftCriteria_" & i.ToString)))
                                    End If
                                Next
                            End If
                        End If

                    End If

                    ' Si el saldo tiene devengo automatico, añadimos a la composicion la justificación de devengo automatico
                    If oConcept.AutomaticAccrualType > eAutomaticAccrualType.DeactivatedType Then
                        Dim newConceptCause = New roEngineConceptComposition
                        newConceptCause.FactorValue = 1
                        newConceptCause.IDCause = oConcept.AutomaticAccrualIDCause
                        newConceptCause.IDType = CompositionType.Cause
                        newConceptCause.IDConcept = oConcept.ID
                        oConcept.Composition.Add(newConceptCause)
                    End If

                    If Not IsDBNull(oRow("CustomType")) Then
                        oConcept.CustomType = oRow("CustomType")
                    End If

                    If Not IsDBNull(oRow("ApplyOnHolidaysRequest")) Then
                        oConcept.ApplyOnHolidaysRequest = oRow("ApplyOnHolidaysRequest")
                    End If

                    oConcept.AutoApproveRequestsDR = False
                    If Not IsDBNull(oRow("AutoApproveRequestsDR")) Then
                        oConcept.AutoApproveRequestsDR = oRow("AutoApproveRequestsDR")
                    End If

                End If

                ' Auditar lectura
                If bAudit AndAlso oConcept IsNot Nothing Then
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tShift, "", tbParameters, -1)
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roConceptManager::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roConceptManager::Load")
            Finally

            End Try

            Return oConcept
        End Function

        Public Function GetConceptCompositions(ByVal _IDConcept As Integer, Optional ByVal bAudit As Boolean = False) As Generic.List(Of roEngineConceptComposition)
            Dim oRet As New Generic.List(Of roEngineConceptComposition)

            Try

                Dim strSQL As String = "@SELECT# ConceptCauses.*, Concepts.IDType AS ConceptType, isnull(Concepts.CustomType,0) as CustomType  " &
                                       "FROM ConceptCauses INNER JOIN Concepts " &
                                                "ON ConceptCauses.IDConcept = Concepts.ID " &
                                       "WHERE ConceptCauses.IDConcept = " & _IDConcept.ToString
                Dim tb As DataTable = CreateDataTable(strSQL)

                For Each oCompositionRow As DataRow In tb.Rows
                    oRet.Add(LoadEngineConceptComposition(oCompositionRow("ID"), bAudit))
                Next
            Catch ex As DbException
                Me.State.UpdateStateInfo(ex, "roConceptManager::GetConceptCompositions")
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roConceptManager::GetConceptCompositions")
            Finally

            End Try

            Return oRet

        End Function

        Public Function LoadEngineConceptComposition(ByVal _IDConceptComposition As Integer, Optional ByVal bAudit As Boolean = False) As roEngineConceptComposition
            Dim oRet As New roEngineConceptComposition

            Try

                Dim strSQL As String = "@SELECT# ConceptCauses.*, Concepts.IDType AS ConceptType, isnull(Concepts.CustomType,0) as CustomType, Causes.Description  " &
                                       " FROM ConceptCauses INNER JOIN Concepts " &
                                                " ON ConceptCauses.IDConcept = Concepts.ID " &
                                                " LEFT OUTER JOIN Causes on ConceptCauses.IDCause = Causes.ID " &
                                       " WHERE ConceptCauses.ID = " & _IDConceptComposition.ToString
                Dim tb As DataTable = CreateDataTable(strSQL)

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                    Dim oRow As DataRow = tb.Rows(0)

                    Dim strXml As String = roTypes.Any2String(oRow("Composition"))

                    oRet.IDConcept = roTypes.Any2Integer(oRow("IDConcept"))
                    oRet.IDCause = roTypes.Any2Integer(oRow("IDCause"))

                    If oRow("IDShift") IsNot DBNull.Value Then
                        oRet.IDShift = roTypes.Any2Integer(oRow("IDShift"))
                    Else
                        oRet.IDShift = 0
                    End If

                    oRet.IDType = roTypes.Any2Integer(oRow("IDType"))
                    oRet.TypeDayPlanned = roTypes.Any2Integer(oRow("TypeDayPlanned"))

                    Try
                        ' Parámetro avanzado de la justificación para nocturnidad (FIAT)
                        If InStr(roTypes.Any2String(oRow("Description")), "#") > 0 Then
                            oRet.AdvParameter = Mid(roTypes.Any2String(oRow("Description")), InStr(roTypes.Any2String(oRow("Description")), "#"), 4)
                        End If
                    Catch ex As Exception
                        oRet.AdvParameter = ""
                    End Try


                    If oRow("FieldFactor") IsNot DBNull.Value Then
                        oRet.CompositionUserField = roTypes.Any2String(oRow("FieldFactor"))
                    Else
                        oRet.CompositionUserField = ""
                    End If

                    ' En función del tipo de acumulado, obtenemos el nombre del campo donde hay el valor del factor directo
                    Dim strFactorField As String = "HoursFactor"
                    ' En el caso de que el acumulado sea de dias o personalziado miramos el campo relacionado
                    If roTypes.Any2String(oRow("ConceptType")) = "O" Or roTypes.Any2Boolean(oRow("CustomType")) Then
                        strFactorField = "OccurrencesFactor"
                    End If

                    If strXml <> "" Then

                        ' Añadimos la composición a la colección
                        Dim oComposition As New roCollection(strXml)

                        Dim n As Integer = 1
                        Dim oConditionNode As roCollection = oComposition.Node("Condition" & n.ToString)
                        While oConditionNode IsNot Nothing

                            Dim oCondition As New roEngineConceptCondition()

                            If oConditionNode IsNot Nothing Then
                                Dim strNode As String = roTypes.Any2String(oConditionNode.Item("IDCauses"))
                                For Each strCause As String In strNode.Split("~")
                                    oCondition.IDCauses.Add(New roEngineConceptConditionCause() With {.IDCause = strCause.Substring(0, strCause.Length - 1), .Operation = strCause.Substring(strCause.Length - 1)})
                                Next
                                oCondition.Compare = roTypes.Any2Integer(oConditionNode.Item("Compare"))
                                oCondition.Value_Type = roTypes.Any2Integer(oConditionNode.Item("ValueType"))
                                Select Case oCondition.Value_Type
                                    Case ValueType.DirectValue
                                        If oConditionNode.Item("Value_Direct") IsNot Nothing Then
                                            oCondition.Value_Direct = roTypes.Any2Time(oConditionNode.Item("Value_Direct")).Value
                                        End If
                                    Case ValueType.IDCause
                                        oCondition.Value_IDCause = roTypes.Any2Integer(oConditionNode.Item("Value_IDCause"))
                                    Case ValueType.UserField
                                        oCondition.Value_UserField = roTypes.Any2String(oConditionNode.Item("Value_UserField"))
                                End Select
                            End If

                            oRet.Conditions.Add(oCondition)
                            n += 1
                            oConditionNode = oComposition.Node("Condition" & n.ToString)
                        End While

                        oRet.FactorType = oComposition.Item("FactorType")
                        Select Case oRet.FactorType
                            Case ValueType.DirectValue
                                oRet.FactorValue = roTypes.Any2Double(oRow(strFactorField)) ' Any2Double(oComposition.Item("FactorValue"))
                            Case ValueType.UserField
                                oRet.FactorUserField = roTypes.Any2String(oComposition.Item("FactorUserField"))
                        End Select
                    Else
                        ' Añadimos la composición básica a la colección (para compatibilidad con veriones anteriores)
                        oRet.FactorType = ValueType.DirectValue
                        oRet.FactorValue = roTypes.Any2Double(oRow(strFactorField))
                    End If

                    ' Auditar lectura
                    If bAudit Then
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tConceptComposition, "", tbParameters, -1)
                    End If

                End If
            Catch ex As DbException
                Me.State.UpdateStateInfo(ex, "roConceptManager::LoadEngineConceptComposition")
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roConceptManager::LoadEngineConceptComposition")
            Finally

            End Try

            Return oRet

        End Function

#End Region

    End Class

End Namespace