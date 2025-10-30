Imports System.Data.Common
Imports System.Drawing
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Namespace VTBudgets

    Public Class roBudgetRowPeriodDataManager
        Private oState As roBudgetRowPeriodDataState = Nothing

        Public Sub New()
            Me.oState = New roBudgetRowPeriodDataState()
        End Sub

        Public Sub New(ByVal _State As roBudgetRowPeriodDataState)
            Me.oState = _State
        End Sub

#Region "Methods"

        Public Function Save(ByVal oBudgetRowProductiveUnitData As roBudgetRowProductiveUnitData, ByVal oBudgetRowPeriodData As roBudgetRowPeriodData, ByRef oBudgetResultDays As Generic.List(Of roBudgetDataDayError), ByVal bolLicenseHRScheduling As Boolean, Optional ByVal bAudit As Boolean = False, Optional bolInitTask As Boolean = True, Optional ByVal bolValidateData As Boolean = True, Optional ByVal oParameters As roParameters = Nothing, ByVal Optional bolCheckPermission As Boolean = False, Optional ByVal bolAssignEmployees As Boolean = False) As Boolean
            Dim bolRet As Boolean = False
            Dim xEmptyDate As New Date(1899, 12, 30, 0, 0, 0, 0)
            Dim strMsg As String = ""
            Dim bHaveToClose As Boolean = False

            Try

                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()
                If bolValidateData Then
                    If Not Me.Validate(oBudgetRowProductiveUnitData, oBudgetRowPeriodData, strMsg, oParameters, oBudgetResultDays, bolLicenseHRScheduling, bolCheckPermission) Then
                        oState.Result = BudgetRowPeriodDataResultEnum.InvalidData
                        oState.ErrorText = strMsg
                        Return bolRet
                        Exit Function
                    End If
                End If

                For Each oBudgetRowDayData As roBudgetRowDayData In oBudgetRowPeriodData.DayData
                    ' Solo guardamos las celdas que hayan cambiado
                    If oBudgetRowDayData.HasChanged Then

                        Dim strShiftName As String = ""
                        Dim intIDSHift As Double = 0
                        Dim ShiftComplementaryFloatingData As roCalendarRowShiftData = Nothing

                        ' Obtenemos la Fecha y UP a actualizar del nodo
                        Dim tb As New DataTable("DailyBudgets")
                        Dim strSQL As String = "@SELECT# * FROM DailyBudgets WHERE IDNode= " & oBudgetRowProductiveUnitData.IDNode.ToString & " AND IDProductiveUnit= " & oBudgetRowProductiveUnitData.ProductiveUnit.ID.ToString & " AND Date=" & roTypes.Any2Time(oBudgetRowDayData.PlanDate).SQLSmallDateTime
                        Dim cmd As DbCommand = CreateCommand(strSQL)
                        Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)

                        da.Fill(tb)

                        Dim oRow As DataRow
                        If tb.Rows.Count = 0 Then
                            oRow = tb.NewRow
                            oRow("ID") = GetNextIDDailyBudget()
                        Else
                            oRow = tb.Rows(0)
                        End If

                        ' Guardamos los datos generales
                        oRow("IDNode") = oBudgetRowProductiveUnitData.IDNode
                        oRow("IDProductiveUnit") = oBudgetRowProductiveUnitData.ProductiveUnit.ID
                        oRow("Date") = oBudgetRowDayData.PlanDate
                        oRow("IDMode") = oBudgetRowDayData.ProductiveUnitMode.ID
                        oRow("CostValue") = oBudgetRowDayData.ProductiveUnitMode.CostValue

                        If tb.Rows.Count = 0 Then
                            tb.Rows.Add(oRow)
                        End If
                        da.Update(tb)

                        bolRet = True

                        ' Guardamos las posiciones
                        Dim strActualPositions As String = "-1"

                        For Each oUnitModePosition As roProductiveUnitModePosition In oBudgetRowDayData.ProductiveUnitMode.UnitModePositions
                            bolRet = SaveDailyBudgetPosition(oRow("ID"), oUnitModePosition, oBudgetRowDayData.PlanDate, True, bolAssignEmployees)
                            If Not bolRet Then
                                oState.Result = BudgetRowPeriodDataResultEnum.InvalidData
                                Return bolRet
                                Exit Function
                            End If
                            strActualPositions += "," & oUnitModePosition.ID
                        Next

                        ' Borramos las posiciones que ya no exsiten
                        strSQL = "@SELECT# * FROM DailyBudget_Positions WHERE IDDailyBudget=" & oRow("ID").ToString & " AND ID NOT IN(" & strActualPositions & ")"
                        Dim tbPos As DataTable = CreateDataTable(strSQL)
                        If tbPos IsNot Nothing AndAlso tbPos.Rows.Count > 0 Then
                            For Each oRowPos As DataRow In tbPos.Rows
                                Dim oUnitModePosition As New roProductiveUnitModePosition
                                oUnitModePosition.ID = oRowPos("ID")
                                bolRet = DeleteDailyBudgetPosition(oUnitModePosition, True)
                                If Not bolRet Then
                                    oState.Result = BudgetRowPeriodDataResultEnum.InvalidData
                                    Return bolRet
                                    Exit Function
                                End If
                            Next
                        End If

                        If bAudit Then
                            ' Auditamos
                            Dim tbParameters As DataTable = oState.CreateAuditParameters()
                            oState.AddAuditParameter(tbParameters, "{PlanDate}", oBudgetRowDayData.PlanDate.ToString, "", 1)
                            oState.Audit(Audit.Action.aUpdate, Audit.ObjectType.tBudgetRow, oBudgetRowProductiveUnitData.ProductiveUnit.Name, tbParameters, -1)
                        End If

                        ' Eliminamos todas las notificaciones de cobertura insuficiente del presupuesto relacionado
                        ' posteriormente el notificador volvera a revisar si sigue habiendo cobertura insuficiente o no
                        strSQL = "@DELETE# FROM sysroNotificationTasks WHERE IDNotification IN (@SELECT# ID FROM Notifications WHERE idtype IN (54)) AND Key1Numeric=" & oRow("ID").ToString
                        ExecuteSql(strSQL)
                    End If
                Next

                bolRet = True
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roBudgetRowPeriodDataManager::Save")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roBudgetRowPeriodDataManager::Save")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Function SaveDailyBudgetPosition(ByVal IDDailyBudget As Long, ByRef oProductiveUnitModePosition As roProductiveUnitModePosition, ByVal xPlanDate As Date, Optional ByVal bAudit As Boolean = False, Optional ByVal bolAssignEmployees As Boolean = False) As Boolean
            Dim bolRet As Boolean = False
            Dim bolIsNew As Boolean = False
            Dim bHaveToClose As Boolean = False

            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim tb As New DataTable("DailyBudget_Positions")
                Dim strSQL As String = "@SELECT# * FROM DailyBudget_Positions WHERE ID=" & oProductiveUnitModePosition.ID.ToString
                Dim cmd As DbCommand = CreateCommand(strSQL)
                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(tb)

                Dim oAuditDataOld As DataRow = Nothing
                Dim oAuditDataNew As DataRow = Nothing

                Dim oRow As DataRow

                Dim bolActionInsert As Boolean = False

                If tb.Rows.Count = 0 Then
                    oRow = tb.NewRow
                    bolIsNew = True
                    bolActionInsert = True
                Else
                    oRow = tb.Rows(0)
                    oAuditDataOld = Extensions.roAudit.CloneRow(oRow)

                End If

                If oProductiveUnitModePosition.ID > 0 Then
                    oRow("ID") = oProductiveUnitModePosition.ID
                Else
                    oRow("ID") = GetNextIdDailyBudgetPosition()
                    oProductiveUnitModePosition.ID = oRow("ID")
                End If
                oRow("IDDailyBudget") = IDDailyBudget
                oRow("Quantity") = oProductiveUnitModePosition.Quantity
                oRow("IsExpandable") = oProductiveUnitModePosition.IsExpandable
                oRow("IDShift") = oProductiveUnitModePosition.ShiftData.ID

                Dim ShiftComplementaryFloatingData As roCalendarRowShiftData = Nothing
                Dim ExpectedWorkingHours As Double = -1
                Dim xEmptyDate As New Date(1899, 12, 30, 0, 0, 0, 0)

                oRow("ExpectedWorkingHours") = DBNull.Value

                ' Verificamos si tenemos que guardar datos complementarios o flotantes
                If oProductiveUnitModePosition.ShiftData.ExistComplementaryData Or oProductiveUnitModePosition.ShiftData.ExistFloatingData Then
                    If oProductiveUnitModePosition.ShiftData.ShiftLayers > 0 Then ShiftComplementaryFloatingData = oProductiveUnitModePosition.ShiftData

                    ' En el caso de franjas flotantes nos debemos guardar las horas teoricas del dia
                    If oProductiveUnitModePosition.ShiftData.ExistFloatingData Then ExpectedWorkingHours = roTypes.Any2Time(0).Add(oProductiveUnitModePosition.ShiftData.PlannedHours, "n").NumericValue
                End If

                oRow("StartShift") = DBNull.Value
                If oProductiveUnitModePosition.ShiftData IsNot Nothing AndAlso oProductiveUnitModePosition.ShiftData.StartHour <> xEmptyDate AndAlso oProductiveUnitModePosition.ShiftData.Type = ShiftTypeEnum.NormalFloating Then oRow("StartShift") = oProductiveUnitModePosition.ShiftData.StartHour

                If ExpectedWorkingHours > 0 Then oRow("ExpectedWorkingHours") = ExpectedWorkingHours

                oRow("LayersDefinition") = DBNull.Value

                If ShiftComplementaryFloatingData IsNot Nothing Then
                    Dim oXml As New roCollection()
                    oXml = GetShiftLayerData(ShiftComplementaryFloatingData)
                    oRow("LayersDefinition") = oXml.XML
                End If

                ' Datos del puesto
                oRow("IDAssignment") = DBNull.Value

                If Not oProductiveUnitModePosition.AssignmentData Is Nothing Then
                    oRow("IDAssignment") = oProductiveUnitModePosition.AssignmentData.ID
                End If

                If tb.Rows.Count = 0 Then
                    tb.Rows.Add(oRow)
                End If
                da.Update(tb)

                ' Asignamos los empleados
                If bolAssignEmployees Then
                    If Not bolIsNew Then
                        ' Si es una posicion ya existente,
                        ' primero eliminamos las asignaciones actuales
                        strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) Set IDDailyBudgetPosition = 0 WHERE IDDailyBudgetPosition=" & oProductiveUnitModePosition.ID.ToString
                        bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                        If Not bolRet Then
                            oState.Result = BudgetRowPeriodDataResultEnum.InvalidData
                            Return bolRet
                            Exit Function
                        End If
                    End If

                    ' Asignamos los empleados actuales
                    If oProductiveUnitModePosition.EmployeesData.Count > 0 Then

                        Dim oProductiveUnitPositionManager As New roProductiveUnitModePositionManager(New roProductiveUnitState(oState.IDPassport))
                        For Each oProductiveUnitModePositionEmployeeData As roProductiveUnitModePositionEmployeeData In oProductiveUnitModePosition.EmployeesData
                            bolRet = oProductiveUnitPositionManager.AddEmployeePlanOnPosition(oProductiveUnitModePosition, xPlanDate, oProductiveUnitModePositionEmployeeData, True)
                            If Not bolRet Then
                                Return bolRet
                                Exit Function
                            End If
                        Next
                    End If
                End If

                bolRet = True

                ' Auditamos
                If bolRet And bAudit Then
                    oAuditDataNew = oRow
                    Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                    Extensions.roAudit.AddFieldsValues(tbAuditParameters, oAuditDataNew, oAuditDataOld)
                    Dim oAuditAction As Audit.Action = IIf(oAuditDataOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)

                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Me.oState.Audit(If(bolActionInsert, Audit.Action.aInsert, Audit.Action.aUpdate), Audit.ObjectType.tDailyBudgetPosition, oProductiveUnitModePosition.ID, tbParameters, -1)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roBudgetRowPeriodDataManager::SaveDailyBudgetPosition")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roBudgetRowPeriodDataManager::SaveDailyBudgetPosition")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Private Function GetNextIdDailyBudgetPosition() As Long
            Dim intRet As Long = 0

            Try

                Dim strQry As String = "@SELECT# (ISNULL(MAX(ID), 0) + 1) FROM DailyBudget_Positions "

                intRet = roTypes.Any2Long(ExecuteScalar(strQry))
            Catch ex As Data.Common.DbException
                Me.oState.UpdateStateInfo(ex, "roBudgetRowPeriodDataManager::GetNextIdDailyBudgetPosition")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roBudgetRowPeriodDataManager::GetNextIdDailyBudgetPosition")
            End Try

            Return intRet
        End Function

        Private Function GetNextIDDailyBudget() As Long

            Dim intRet As Long = 0

            Try

                Dim strSQL As String = "@SELECT# Max(ID) AS Contador FROM DailyBudgets "
                Dim tb As DataTable = CreateDataTable(strSQL)
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    If Not IsDBNull(tb.Rows(0).Item(0)) Then
                        intRet = tb.Rows(0).Item(0)
                    End If
                End If

                intRet += 1
            Catch ex As Data.Common.DbException
                Me.oState.UpdateStateInfo(ex, "roBudgetRowPeriodDataManager::GetNextIDDailyBudget")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roBudgetRowPeriodDataManager::GetNextIDDailyBudget")
            End Try

            Return intRet

        End Function

        Public Function Validate(ByVal oBudgetRowProductiveUnitData As roBudgetRowProductiveUnitData, ByVal oBudgetRowPeriodData As roBudgetRowPeriodData, ByRef strMsg As String, ByVal oParameters As roParameters, ByRef oBudgetResultDays As Generic.List(Of roBudgetDataDayError), ByVal bolLicenseHRScheduling As Boolean, ByVal Optional bolCheckPermission As Boolean = True) As Boolean
            Dim bolRet As Boolean = True

            Dim bolHasPermission As Boolean = False

            Dim oBudgetDataDayError As New roBudgetDataDayError

            Try
                strMsg = ""

                If oBudgetRowProductiveUnitData Is Nothing Then
                    oState.Result = BudgetRowPeriodDataResultEnum.InvalidData
                    strMsg &= Me.oState.Language.Translate("BudgetRowPeriodDataManager.Validate.invalidProductiveUnitData", "") & vbNewLine
                End If

                If oBudgetRowProductiveUnitData IsNot Nothing AndAlso oBudgetRowProductiveUnitData.IDNode <= 0 Then
                    oState.Result = BudgetRowPeriodDataResultEnum.InvalidData
                    strMsg &= Me.oState.Language.Translate("BudgetRowPeriodDataManager.Validate.invalidNodeData", "") & vbNewLine
                End If

                If oBudgetRowProductiveUnitData.ProductiveUnit Is Nothing OrElse oBudgetRowProductiveUnitData.ProductiveUnit.ID <= 0 Then
                    oState.Result = BudgetRowPeriodDataResultEnum.InvalidData
                    strMsg &= Me.oState.Language.Translate("BudgetRowPeriodDataManager.Validate.invalidProductiveUnitData", "") & vbNewLine
                End If

                If strMsg.Length > 0 Then
                    bolRet = False
                    Return bolRet
                    Exit Function
                End If

                ' Obtenemos la fecha de congelacion
                Dim oFirstDate As Object = oParameters.Parameter(Parameters.FirstDate)

                For Each oBudgetRowDayData As roBudgetRowDayData In oBudgetRowPeriodData.DayData
                    ' Solo validamos las celdas que hayan cambiado
                    If oBudgetRowDayData.HasChanged Then
                        Dim strError As String = ""
                        'Verificamos si esta dentro del periodo de congelacion
                        If oFirstDate IsNot Nothing AndAlso IsDate(oFirstDate) Then
                            If oBudgetRowDayData.PlanDate <= CDate(oFirstDate) Then
                                oState.Result = BudgetRowPeriodDataResultEnum.InvalidData
                                strError = oBudgetRowProductiveUnitData.ProductiveUnit.Name & " " & Me.oState.Language.Translate("CalendarRowPeriodDataManager.Validate.FreezingDate", "") & " " & oBudgetRowDayData.PlanDate
                                strMsg &= strError & vbNewLine
                                oBudgetDataDayError = New roBudgetDataDayError
                                oBudgetDataDayError.IDProductiveUnit = oBudgetRowDayData.ProductiveUnitMode.IDProductiveUnit
                                oBudgetDataDayError.ErrorDate = oBudgetRowDayData.PlanDate
                                oBudgetDataDayError.ErrorCode = BudgetErrorResultDayEnum.FreezingDate
                                oBudgetDataDayError.ErrorText = strError
                                oBudgetResultDays.Add(oBudgetDataDayError)
                            End If
                        End If
                    End If
                Next

                If strMsg.Length > 0 Then bolRet = False
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roBudgetRowPeriodDataManager::Validate")
                bolRet = False
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBudgetRowPeriodDataManager::Validate")
                bolRet = False
            End Try

            Return bolRet

        End Function

        Public Function GetShiftLayerData(ByVal oCalendarRowShiftData As roCalendarRowShiftData) As roCollection
            Dim oXml As New roCollection
            Try

                oXml.Add("TotalLayers", oCalendarRowShiftData.ShiftLayers)
                Dim i As Integer = 1
                For Each oShiftLayerDefinition As roCalendarShiftLayersDefinition In oCalendarRowShiftData.ShiftLayersDefinition
                    oXml.Add("LayerID_" & i.ToString, oShiftLayerDefinition.LayerID)

                    If oCalendarRowShiftData.ExistComplementaryData Then
                        If oShiftLayerDefinition.LayerComplementaryHours >= 0 Then oXml.Add("LayerComplementaryHours_" & i.ToString, roTypes.Any2Time(0).Add(oShiftLayerDefinition.LayerComplementaryHours, "n").NumericValue)
                        If oShiftLayerDefinition.LayerOrdinaryHours >= 0 Then oXml.Add("LayerOrdinaryHours_" & i.ToString, roTypes.Any2Time(0).Add(oShiftLayerDefinition.LayerOrdinaryHours, "n").NumericValue)
                    End If
                    If oCalendarRowShiftData.ExistFloatingData Then
                        If oShiftLayerDefinition.ExistLayerDuration AndAlso oShiftLayerDefinition.LayerDuration > 0 Then oXml.Add("LayerFloatingDuration_" & i.ToString, oShiftLayerDefinition.LayerDuration)
                        If oShiftLayerDefinition.ExistLayerStartTime AndAlso oShiftLayerDefinition.LayerStartTime <> New Date(1900, 1, 1, 0, 0, 0) Then oXml.Add("LayerFloatingBeginTime_" & i.ToString, oShiftLayerDefinition.LayerStartTime)
                    End If
                    i += 1
                Next
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roBudgetRowPeriodDataManager::GetShiftLayerData")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBudgetRowPeriodDataManager::GetShiftLayerData")
            Finally

            End Try

            Return oXml
        End Function

        Public Function ValidateComplementaryData(ByVal oCalendarRowShiftData As roCalendarRowShiftData, ByVal oShift As Shift.roShift)
            Dim bolRet As Boolean = False
            Dim bolCloseTrans As Boolean = False
            Dim oCalendarDataDayError As New roCalendarDataDayError
            Dim strError As String = ""

            Try

                If oShift Is Nothing Then
                    bolRet = False
                    Return bolRet
                    Exit Function
                End If

                ' Para cada franja rigida validamos que el tiempo total de la franja y las horas ordinarias y complementarias coincida
                For Each oLayer As Shift.roShiftLayer In oShift.Layers
                    If oLayer.LayerType = roLayerTypes.roLTMandatory Then
                        Dim dblTotalComplementary As Double = 0
                        Dim dblTotalTimeLayer As Double = 0

                        Dim oBeginLayer As DateTime = oLayer.Data("Begin")
                        Dim oFinishLayer As DateTime = oLayer.Data("Finish")

                        If oLayer.Data.Exists("AllowModifyIniHour") Then
                            ' Si la franja tiene hora de inicio flotante, obtenemos el dato en el dia
                            If oCalendarRowShiftData IsNot Nothing AndAlso oCalendarRowShiftData.ShiftLayersDefinition IsNot Nothing Then
                                For Each ShiftLayerDefinition As roCalendarShiftLayersDefinition In oCalendarRowShiftData.ShiftLayersDefinition
                                    If ShiftLayerDefinition.LayerID = oLayer.ID Then
                                        If ShiftLayerDefinition.LayerStartTime <> New Date(1900, 1, 1) Then oBeginLayer = ShiftLayerDefinition.LayerStartTime
                                        Exit For
                                    End If
                                Next
                            End If
                        End If

                        If oLayer.Data.Exists("FloatingFinishMinutes") Then
                            ' En caso de final flotante en funcion de la entrada, modificamos el final de la franja
                            oFinishLayer = roTypes.Any2Time(oBeginLayer).Add(oLayer.Data.Item("FloatingFinishMinutes"), "n").Value
                        End If

                        If oLayer.Data.Exists("AllowModifyDuration") Then
                            ' Si la franja tiene duracion flotante, debe existir en los datos del dia
                            If oCalendarRowShiftData IsNot Nothing AndAlso oCalendarRowShiftData.ShiftLayersDefinition IsNot Nothing Then
                                For Each ShiftLayerDefinition As roCalendarShiftLayersDefinition In oCalendarRowShiftData.ShiftLayersDefinition
                                    If ShiftLayerDefinition.LayerID = oLayer.ID Then
                                        If ShiftLayerDefinition.LayerDuration <> -1 Then oFinishLayer = roTypes.Any2Time(oBeginLayer).Add(ShiftLayerDefinition.LayerDuration, "n").Value
                                        Exit For
                                    End If
                                Next
                            End If

                        End If
                        'Tiempo de la franja
                        dblTotalTimeLayer = roTypes.Any2Time(oFinishLayer).NumericValue - roTypes.Any2Time(oBeginLayer).NumericValue

                        'Tiempo de la ordinarias + complememtarias
                        If oCalendarRowShiftData IsNot Nothing AndAlso oCalendarRowShiftData.ShiftLayersDefinition IsNot Nothing Then
                            For Each ShiftLayerDefinition As roCalendarShiftLayersDefinition In oCalendarRowShiftData.ShiftLayersDefinition
                                If ShiftLayerDefinition.LayerID = oLayer.ID Then
                                    dblTotalComplementary = roTypes.Any2Time(0).Add(ShiftLayerDefinition.LayerComplementaryHours, "n").NumericValue + roTypes.Any2Time(0).Add(ShiftLayerDefinition.LayerOrdinaryHours, "n").NumericValue
                                    Exit For
                                End If
                            Next
                        End If
                        ' Si no coinciden es incorrecto
                        If dblTotalTimeLayer = dblTotalComplementary Then bolRet = True

                        If Not bolRet Then Exit For
                    End If
                Next
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roBudgetRowPeriodDataManager::ValidateComplementaryData")
                bolRet = False
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBudgetRowPeriodDataManager::ValidateComplementaryData")
                bolRet = False
            End Try

            Return bolRet
        End Function

        Public Function ValidateFloatingData(ByVal oCalendarRowShiftData As roCalendarRowShiftData, ByVal oShift As Shift.roShift)
            Dim bolRet As Boolean = False
            Dim bolCloseTrans As Boolean = False
            Dim oCalendarDataDayError As New roCalendarDataDayError
            Dim strError As String = ""

            Try

                If oShift Is Nothing Then
                    bolRet = False
                    Return bolRet
                    Exit Function
                End If

                For Each oLayer As Shift.roShiftLayer In oShift.Layers
                    If oLayer.LayerType = roLayerTypes.roLTMandatory Then
                        bolRet = True
                        If oLayer.Data.Exists("AllowModifyIniHour") Then
                            ' Si la franja tiene hora de inicio flotante, debe existir en los datos del dia
                            bolRet = False
                            If oCalendarRowShiftData IsNot Nothing AndAlso oCalendarRowShiftData.ShiftLayersDefinition IsNot Nothing Then
                                For Each ShiftLayerDefinition As roCalendarShiftLayersDefinition In oCalendarRowShiftData.ShiftLayersDefinition
                                    If ShiftLayerDefinition.LayerID = oLayer.ID Then
                                        If ShiftLayerDefinition.LayerStartTime <> New Date(1900, 1, 1) Then bolRet = True
                                        Exit For
                                    End If
                                Next
                            End If
                            If Not bolRet Then
                                Return bolRet
                                Exit Function
                            End If
                        End If

                        If oLayer.Data.Exists("AllowModifyDuration") Then
                            ' Si la franja tiene duracion flotante, debe existir en los datos del dia
                            bolRet = False
                            If oCalendarRowShiftData IsNot Nothing AndAlso oCalendarRowShiftData.ShiftLayersDefinition IsNot Nothing Then
                                For Each ShiftLayerDefinition As roCalendarShiftLayersDefinition In oCalendarRowShiftData.ShiftLayersDefinition
                                    If ShiftLayerDefinition.LayerID = oLayer.ID Then
                                        If ShiftLayerDefinition.LayerDuration <> -1 Then bolRet = True
                                        Exit For
                                    End If
                                Next
                            End If
                            If Not bolRet Then
                                Return bolRet
                                Exit Function
                            End If
                        End If

                    End If
                Next

                ' En el caso que el horario tenga mas de una franja, debemos validar que no se solapen,
                ' Ademas obtenemos los periodos y los validamos
                Dim oBeginLayer2 As DateTime = Now.Date
                Dim oBeginLayer1 As DateTime = Now.Date
                Dim oFinishLayer1 As DateTime = Now.Date
                Dim oFinishLayer2 As DateTime = Now.Date
                Dim i As Integer = 0

                For Each oLayer As Shift.roShiftLayer In oShift.Layers
                    If oLayer.LayerType = roLayerTypes.roLTMandatory Then
                        i += 1
                        Dim oBeginLayer As DateTime = oLayer.Data("Begin")
                        Dim oFinishLayer As DateTime = oLayer.Data("Finish")

                        If oLayer.Data.Exists("AllowModifyIniHour") Then
                            ' Si la franja tiene hora de inicio flotante, obtenemos el dato en el dia
                            If oCalendarRowShiftData IsNot Nothing AndAlso oCalendarRowShiftData.ShiftLayersDefinition IsNot Nothing Then
                                For Each ShiftLayerDefinition As roCalendarShiftLayersDefinition In oCalendarRowShiftData.ShiftLayersDefinition
                                    If ShiftLayerDefinition.LayerID = oLayer.ID Then
                                        If ShiftLayerDefinition.LayerStartTime <> New Date(1900, 1, 1) Then oBeginLayer = ShiftLayerDefinition.LayerStartTime
                                        Exit For
                                    End If
                                Next
                            End If
                        End If

                        If oLayer.Data.Exists("FloatingFinishMinutes") Then
                            ' En caso de final flotante en funcion de la entrada, modificamos el final de la franja
                            oFinishLayer = roTypes.Any2Time(oBeginLayer).Add(oLayer.Data.Item("FloatingFinishMinutes"), "n").Value
                        End If

                        If oLayer.Data.Exists("AllowModifyDuration") Then
                            ' Si la franja tiene duracion flotante, debe existir en los datos del dia
                            If oCalendarRowShiftData IsNot Nothing AndAlso oCalendarRowShiftData.ShiftLayersDefinition IsNot Nothing Then
                                For Each ShiftLayerDefinition As roCalendarShiftLayersDefinition In oCalendarRowShiftData.ShiftLayersDefinition
                                    If ShiftLayerDefinition.LayerID = oLayer.ID Then
                                        If ShiftLayerDefinition.LayerDuration <> -1 Then oFinishLayer = roTypes.Any2Time(oBeginLayer).Add(ShiftLayerDefinition.LayerDuration, "n").Value
                                        Exit For
                                    End If
                                Next
                            End If

                        End If

                        If i = 1 Then
                            oFinishLayer1 = oFinishLayer
                            oBeginLayer1 = oBeginLayer
                        Else
                            oBeginLayer2 = oBeginLayer
                            oFinishLayer2 = oFinishLayer
                        End If
                        If Not bolRet Then Exit For
                    End If
                Next

                ' Si hay 2 franjas miramos que no se solape la hora final de la primera y la hora de inicio de la segunda
                If i = 2 Then
                    If oFinishLayer1 >= oBeginLayer2 Then bolRet = False
                End If

                ' Validamos que los periodos de cada franja sean correctos
                If i > 0 Then
                    ' Validamos la franja 1
                    If oBeginLayer1 >= oFinishLayer1 Then bolRet = False
                End If

                If i > 1 Then
                    ' Validamos la franja 2
                    If oBeginLayer2 >= oFinishLayer2 Then bolRet = False
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roBudgetRowPeriodDataManager::ValidateFloatingData")
                bolRet = False
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBudgetRowPeriodDataManager::ValidateFloatingData")
                bolRet = False
            End Try

            Return bolRet

        End Function

        Public Function GetProductiveUnitPlan(ByVal IdNode As Integer, ByVal intIDProductiveUnit As Integer, ByVal xBegin As DateTime, ByVal xEnd As DateTime, ByRef oState As roBudgetRowPeriodDataState) As DataTable
            '
            ' Obtiene la planificación de la unidad productiva en el periodo indicado
            '
            Dim tbPlan As DataTable = Nothing

            oState.UpdateStateInfo()

            Try

                Dim strSQL As String
                strSQL = " @SELECT# DailyBudgets.ID as IDBudget, DailyBudgets.Date, DailyBudgets.IDMode, ProductiveUnit_Modes.Name as ModeName, ProductiveUnit_Modes.ShortName as ModeShortName, ProductiveUnit_Modes.Color as ModeColor, DailyBudgets.CostValue as ModeCostValue " &
                        "FROM DailyBudgets " &
                        "INNER JOIN ProductiveUnits On dbo.DailyBudgets.IDProductiveUnit = dbo.ProductiveUnits.ID  " &
                        "INNER JOIN ProductiveUnit_Modes  On dbo.DailyBudgets.IDMode = ProductiveUnit_Modes.ID AND dbo.DailyBudgets.IDProductiveUnit = dbo.ProductiveUnit_Modes.IDProductiveUnit"

                strSQL &= " WHERE DailyBudgets.IDProductiveUnit = " & intIDProductiveUnit & " And " &
                              "DailyBudgets.Date >= " & roTypes.Any2Time(xBegin.Date).SQLSmallDateTime & " And DailyBudgets.Date <= " & roTypes.Any2Time(xEnd.Date).SQLSmallDateTime

                strSQL &= " And DailyBudgets.IDNode = " & IdNode.ToString

                tbPlan = CreateDataTableWithoutTimeouts(strSQL, , "ProductiveUnitPlan")
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roBudgetRowPeriodDataManager::GetProductiveUnitPlan")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBudgetRowPeriodDataManager::GetProductiveUnitPlan")
            Finally

            End Try

            Return tbPlan

        End Function

        Public Function GetProductiveUnitPlanPositionEmployees(ByVal oDailyPositions As DataTable, ByRef oState As roBudgetRowPeriodDataState) As DataTable

            Dim tbPlan As DataTable = Nothing

            oState.UpdateStateInfo()

            ' Obtiene la planificacion de los empleados asignados a las posiciones
            Dim strDailyPositions As String = "-1"
            If oDailyPositions IsNot Nothing AndAlso oDailyPositions.Rows IsNot Nothing Then
                For Each oRow In oDailyPositions.Rows
                    strDailyPositions += "," & oRow("IDPosition")
                Next
            End If

            Try

                Dim strSQL As String
                strSQL = " @SELECT# Employees.Name as EmployeeName, " &
                        "DailySchedule.Date, " &
                        "Shifts.ShortName AS ShortName1, " &
                        "Shifts.Color AS ShiftColor1, DailySchedule.Status, " &
                        "DailySchedule.IDShift1, " &
                        "DailySchedule.IDEmployee, DailySchedule.LockedDay, DailySchedule.FeastDay, " &
                        "DailySchedule.StartShift1, " &
                        "DailySchedule.IDAssignment, " &
                        "DailySchedule.IDDailyBudgetPosition, " &
                        "Assignments.Color AS AssignmentColor, Assignments.Name AS AssignmentName, Assignments.ShortName AS AssignmentShortName, " &
                        "isnull(DailySchedule.ExpectedWorkingHours,Shifts.ExpectedWorkingHours) As ExpectedWorkingHours1," &
                        " isnull((@SELECT# count(*) FROM ProgrammedAbsences WHERE ProgrammedAbsences.idEmployee = DailySchedule.IDEmployee AND BeginDate <= DailySchedule.Date and  CASE WHEN FinishDate IS NULL THEN DATEADD(day, MaxLastingDays-1, BeginDate) ELSE FinishDate END >= DailySchedule.Date),0) as TotalAbsences," &
                        "Shifts.BreakHours As BreakHours1," &
                        "Shifts.Name As NameShift1," &
                        "Shifts.ShiftType  As ShiftType1," &
                        "Shifts.StartLimit as StartLimit1," &
                        "Shifts.IsFloating  As IsFloating1," &
                        "ISNULL(Shifts.AreWorkingDays,0) As AreWorkingDays1," &
                        "DailySchedule.Remarks, " &
                        "Shifts.AllowFloatingData as ExistFloatingData, " &
                        "Shifts.AllowComplementary as ExistComplementaryDataShift1, " &
                        "isnull(LayersDefinition,'') as LayersDefinition " &
                        "FROM DailySchedule " &
                        "INNER JOIN Employees On dbo.DailySchedule.IDEmployee = dbo.Employees.ID " &
                        "LEFT OUTER JOIN Shifts On dbo.DailySchedule.IDShift1 = dbo.Shifts.ID " &
                        "LEFT OUTER JOIN Assignments On dbo.DailySchedule.IDAssignment = Assignments.ID "

                strSQL &= "WHERE DailySchedule.IDDailyBudgetPosition in( " & strDailyPositions & ")"
                strSQL &= "ORDER BY  Employees.Name, DailySchedule.Date"

                tbPlan = CreateDataTableWithoutTimeouts(strSQL, , "PlanEmployeePositions")
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roBudgetRowPeriodDataManager::GetProductiveUnitPlanPositionEmployees")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBudgetRowPeriodDataManager::GetProductiveUnitPlanPositionEmployees")
            Finally

            End Try

            Return tbPlan

        End Function

        Public Function GetProductiveUnitPlanPositions(ByVal IdNode As Integer, ByVal intIDProductiveUnit As Integer, ByVal xBegin As DateTime, ByVal xEnd As DateTime, ByRef oState As roBudgetRowPeriodDataState) As DataTable
            '
            ' Obtiene las posiciones planificadas de la unidad productiva en el periodo indicado
            '
            Dim tbPlan As DataTable = Nothing

            oState.UpdateStateInfo()

            Try

                Dim strSQL As String
                strSQL = " @SELECT# DailyBudgets.ID as IDBudget, DailyBudgets.Date, DailyBudget_Positions.ID as IDPosition, DailyBudget_Positions.Quantity, DailyBudget_Positions.IsExpandable, DailyBudget_Positions.IDShift, DailyBudget_Positions.StartShift, isnull(DailyBudget_Positions.LayersDefinition, '') as LayersDefinition , isnull(DailyBudget_Positions.ExpectedWorkingHours, Shifts.ExpectedWorkingHours) as ExpectedWorkingHours , DailyBudget_Positions.IDAssignment, " &
                        " Shifts.Color as ShiftColor, Shifts.Name as ShiftName, Shifts.ShortName as ShiftShortName, Shifts.IsFloating  As IsFloatingShift, Shifts.StartFloating  As StartFloating, Shifts.ShiftType  As ShiftType, " &
                        " Shifts.StartLimit as ShiftStartLimit, Shifts.EndLimit as ShiftEndLimit, ISNULL(Shifts.AreWorkingDays,0) As AreWorkingDaysShift, Shifts.AllowComplementary as ExistComplementaryDataShift, " &
                        " Shifts.BreakHours As BreakHoursShift, Shifts.AllowFloatingData as ExistFloatingDataShift, Assignments.Name as AssignmentName, Assignments.ShortName as AssignmentShortName, Assignments.Color as  AssignmentColor " &
                        " FROM DailyBudgets " &
                        " INNER JOIN DailyBudget_Positions On dbo.DailyBudgets.ID = dbo.DailyBudget_Positions.IDDailyBudget  " &
                        " INNER JOIN Shifts  On dbo.DailyBudget_Positions.IDShift = Shifts.ID  " &
                        " INNER JOIN Assignments  On dbo.DailyBudget_Positions.IDAssignment = Assignments.ID  "

                strSQL &= " WHERE DailyBudgets.IDProductiveUnit = " & intIDProductiveUnit & " And " &
                              "DailyBudgets.Date >= " & roTypes.Any2Time(xBegin.Date).SQLSmallDateTime & " And DailyBudgets.Date <= " & roTypes.Any2Time(xEnd.Date).SQLSmallDateTime

                strSQL &= " And DailyBudgets.IDNode = " & IdNode.ToString

                tbPlan = CreateDataTableWithoutTimeouts(strSQL, , "PlanPosition")
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roBudgetRowPeriodDataManager::GetProductiveUnitPlanPositions")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBudgetRowPeriodDataManager::GetProductiveUnitPlanPositions")
            Finally

            End Try

            Return tbPlan

        End Function

        Public Function Delete(ByVal oBudgetRowProductiveUnitData As roBudgetRowProductiveUnitData, ByVal oBudgetRowPeriodData As roBudgetRowPeriodData, ByRef oBudgetResultDays As Generic.List(Of roBudgetDataDayError), ByVal bolLicenseHRScheduling As Boolean, Optional ByVal bAudit As Boolean = False, Optional bolInitTask As Boolean = True, Optional ByVal bolValidateData As Boolean = True, Optional ByVal oParameters As roParameters = Nothing, ByVal Optional bolCheckPermission As Boolean = False) As Boolean
            Dim bolRet As Boolean = False
            Dim xEmptyDate As New Date(1899, 12, 30, 0, 0, 0, 0)
            Dim strMsg As String = ""
            Dim bHaveToClose As Boolean = False

            Try

                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                For Each oBudgetRowDayData As roBudgetRowDayData In oBudgetRowPeriodData.DayData
                    ' Borramos las posiciones del dia planificado
                    For Each oUnitModePosition As roProductiveUnitModePosition In oBudgetRowDayData.ProductiveUnitMode.UnitModePositions
                        bolRet = DeleteDailyBudgetPosition(oUnitModePosition, True)
                        If Not bolRet Then
                            oState.Result = BudgetRowPeriodDataResultEnum.InvalidData
                            Return bolRet
                            Exit Function
                        End If
                    Next

                    ' Borramos el dia planificado
                    Dim strSQLDEL As String = "@DELETE# FROM DailyBudgets WHERE IDNode= " & oBudgetRowProductiveUnitData.IDNode.ToString & " AND IDProductiveUnit= " & oBudgetRowProductiveUnitData.ProductiveUnit.ID.ToString & " AND Date=" & roTypes.Any2Time(oBudgetRowDayData.PlanDate).SQLSmallDateTime
                    bolRet = ExecuteSqlWithoutTimeOut(strSQLDEL)
                    If Not bolRet Then
                        oState.Result = BudgetRowPeriodDataResultEnum.InvalidData
                        Return bolRet
                        Exit Function
                    End If

                    If bAudit Then
                        ' Auditamos
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        oState.AddAuditParameter(tbParameters, "{PlanDate}", oBudgetRowDayData.PlanDate.ToString, "", 1)
                        oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tBudgetRow, oBudgetRowProductiveUnitData.ProductiveUnit.Name, tbParameters, -1)
                    End If

                Next

                bolRet = True
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roBudgetRowPeriodDataManager::Delete")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roBudgetRowPeriodDataManager::Delete")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Function DeleteDailyBudgetPosition(ByRef oProductiveUnitModePosition As roProductiveUnitModePosition, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False
            Dim bolIsNew As Boolean = False
            Dim bHaveToClose As Boolean = False

            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                ' Borramos la posicion indicada
                Dim strSQL As String = "@DELETE# FROM DailyBudget_Positions WHERE ID=" & oProductiveUnitModePosition.ID.ToString
                bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                If Not bolRet Then
                    oState.Result = BudgetRowPeriodDataResultEnum.InvalidData
                    Return bolRet
                    Exit Function
                End If

                ' Quitamos las asignaciones a los empleados
                strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) Set IDDailyBudgetPosition = 0 WHERE IDDailyBudgetPosition=" & oProductiveUnitModePosition.ID.ToString
                bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                If Not bolRet Then
                    oState.Result = BudgetRowPeriodDataResultEnum.InvalidData
                    Return bolRet
                    Exit Function
                End If

                ' Auditamos
                If bolRet And bAudit Then
                    Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tDailyBudgetPosition, oProductiveUnitModePosition.ID, tbParameters, -1)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roProductiveUnitModePositionManager::DeleteDailyBudgetPosition")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roProductiveUnitModePositionManager::DeleteDailyBudgetPosition")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

#End Region

#Region "Helpers"

        Public Shared Function HexConverter(c As System.Drawing.Color) As String
            Return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2")
        End Function

        Public Shared Function LoadCellsByBudget(ByVal _FirstDay As DateTime, ByVal _LastDay As DateTime, ByVal _IDProductiveUnit As Integer, ByVal _IDNode As Integer, ByVal oPermission As Integer,
                                                   ByVal oParameters As roParameters, ByVal _typeView As BudgetView, ByVal _detailLevel As BudgetDetailLevel,
                                                   ByRef _State As roBudgetRowPeriodDataState, ByVal bolLicenseHRScheduling As Boolean, Optional ByVal bolLoadShiftHourData As Boolean = False, Optional ByRef oShiftCache As Hashtable = Nothing) As roBudgetRowPeriodData

            ' Llenamos las celdas de de la UP/Nodo del periodo indicado
            Dim oRet As New roBudgetRowPeriodData

            Dim bolRet As Boolean = False
            Dim oBudgetCell As roBudgetRowDayData
            Dim oBudgetPeriodCells As New Generic.List(Of roBudgetRowDayData)

            Dim oRows() As DataRow = Nothing

            Dim oRowDetails() As DataRow = Nothing

            Dim oRowEmployeeDetails() As DataRow = Nothing

            Try
                If oShiftCache Is Nothing Then
                    oShiftCache = New Hashtable
                End If

                Dim oBudgetRowPeriodDataManager As New roBudgetRowPeriodDataManager(_State)

                ' Cargamos los modos planificados
                Dim tbDetail As DataTable = Nothing
                tbDetail = oBudgetRowPeriodDataManager.GetProductiveUnitPlan(_IDNode, _IDProductiveUnit, _FirstDay, _LastDay, _State)

                ' Cargamos las posiciones planificadas
                Dim tbPositions As DataTable = Nothing
                tbPositions = oBudgetRowPeriodDataManager.GetProductiveUnitPlanPositions(_IDNode, _IDProductiveUnit, _FirstDay, _LastDay, _State)

                ' Cargamos la planificacion de los empleados asignados a las posiciones del modo de cada dia
                Dim tbEmployeePositions As DataTable = Nothing
                tbEmployeePositions = oBudgetRowPeriodDataManager.GetProductiveUnitPlanPositionEmployees(tbPositions, _State)

                ' Para cada día del periodo , obtenemos los datos necesarios
                Dim dAct As Date = _FirstDay
                Dim dEnd As Date = _LastDay
                Dim auxColor As System.Drawing.Color = Color.White

                While (dAct <= dEnd)
                    Dim bolAssignedEmployees As Boolean = False
                    oBudgetCell = New roBudgetRowDayData
                    oBudgetCell.PlanDate = dAct.Date

                    oRows = tbDetail.Select("Date = '" & Format(dAct, "yyyy/MM/dd") & "'")
                    If oRows.Length = 0 Then
                        ' Dia sin modo asignado
                        oBudgetCell.ProductiveUnitStatus = ProductiveUnitStatusOnDayEnum.NoPlanned
                    End If

                    If oBudgetCell.ProductiveUnitStatus = ProductiveUnitStatusOnDayEnum.Ok Then
                        ' Cargamos los datos del modo planificado
                        oBudgetCell.ProductiveUnitMode = New roProductiveUnitMode
                        oBudgetCell.ProductiveUnitMode.ID = oRows(0)("IDMode")
                        oBudgetCell.ProductiveUnitMode.IDProductiveUnit = _IDProductiveUnit
                        oBudgetCell.ProductiveUnitMode.HtmlColor = ColorTranslator.ToHtml(System.Drawing.ColorTranslator.FromWin32(roTypes.Any2Integer(oRows(0)("ModeColor"))))
                        oBudgetCell.ProductiveUnitMode.Name = oRows(0)("ModeName")
                        oBudgetCell.ProductiveUnitMode.ShortName = oRows(0)("ModeShortName")
                        oBudgetCell.ProductiveUnitMode.CostValue = oRows(0)("ModeCostValue")

                        ' Cargamos los datos de las posiciones
                        Dim oProductiveUnitModePositions As New Generic.List(Of roProductiveUnitModePosition)
                        oRowDetails = tbPositions.Select("IDBudget=" & roTypes.Any2String(oRows(0)("IDBudget")))
                        If oRowDetails.Length > 0 Then
                            For Each oRowDetail As DataRow In oRowDetails

                                Dim oShift As Shift.roShift = Nothing
                                ' Para cada posicion
                                Dim oProductiveUnitModePosition As New roProductiveUnitModePosition
                                oProductiveUnitModePosition.ID = oRowDetail("IDPosition")
                                oProductiveUnitModePosition.IDProductiveUnitMode = _IDProductiveUnit
                                oProductiveUnitModePosition.IsExpandable = oRowDetail("IsExpandable")
                                oProductiveUnitModePosition.Quantity = oRowDetail("Quantity")

                                ' Datos del horario asignado
                                oProductiveUnitModePosition.ShiftData = New roCalendarRowShiftData
                                oProductiveUnitModePosition.ShiftData.ID = roTypes.Any2Integer(oRowDetail("IDShift"))

                                If _detailLevel = BudgetDetailLevel.Hour Then
                                    ' En caso necesario lo añadimos al cache de horarios
                                    If Not oShiftCache.Contains(oProductiveUnitModePosition.ShiftData.ID) Then
                                        oShift = New Shift.roShift(oProductiveUnitModePosition.ShiftData.ID, New Shift.roShiftState(_State.IDPassport))
                                        oShiftCache.Add(oShift.ID, oShift)
                                    End If
                                End If

                                oProductiveUnitModePosition.ShiftData.Name = roTypes.Any2String(oRowDetail("ShiftName"))
                                auxColor = System.Drawing.ColorTranslator.FromWin32(roTypes.Any2Integer(oRowDetail("ShiftColor")))
                                oProductiveUnitModePosition.ShiftData.Color = HexConverter(auxColor)
                                oProductiveUnitModePosition.ShiftData.PlannedHours = roTypes.Any2Time(roTypes.Any2Double(oRowDetail("ExpectedWorkingHours"))).Minutes
                                oProductiveUnitModePosition.ShiftData.ShortName = roTypes.Any2String(oRowDetail("ShiftShortName"))
                                If Not roTypes.Any2Boolean(oRowDetail("IsFloatingShift")) Then
                                    Select Case roTypes.Any2Integer(oRowDetail("ShiftType"))
                                        Case 0, 1  'Normal
                                            oProductiveUnitModePosition.ShiftData.Type = ShiftTypeEnum.Normal
                                            oProductiveUnitModePosition.ShiftData.StartHour = roTypes.Any2Time(oRowDetail("ShiftStartLimit")).Value
                                            oProductiveUnitModePosition.ShiftData.EndHour = roTypes.Any2Time(oRowDetail("ShiftEndLimit")).Value
                                        Case 2  'Vacaciones
                                            oProductiveUnitModePosition.ShiftData.Type = IIf(roTypes.Any2Boolean(oRowDetail("AreWorkingDaysShift")), ShiftTypeEnum.Holiday_Working, ShiftTypeEnum.Holiday_NoWorking)
                                            oProductiveUnitModePosition.ShiftData.StartHour = roTypes.Any2Time(oRowDetail("ShiftStartLimit")).Value
                                            oProductiveUnitModePosition.ShiftData.EndHour = roTypes.Any2Time(oRowDetail("ShiftEndLimit")).Value
                                    End Select
                                Else
                                    ' Flotante
                                    oProductiveUnitModePosition.ShiftData.Type = ShiftTypeEnum.NormalFloating
                                    oProductiveUnitModePosition.ShiftData.StartHour = roTypes.Any2Time(oRowDetail("StartShift")).Value
                                    oProductiveUnitModePosition.ShiftData.EndHour = CType(roTypes.Any2Time(oRowDetail("ShiftEndLimit")).Value, Date).AddMinutes(DateDiff(DateInterval.Minute, CDate(roTypes.Any2Time(oRowDetail("StartShift")).Value), CDate(roTypes.Any2Time(oRowDetail("StartFloating")).Value)))
                                End If

                                ' Asignamos los datos de complementarias o de flotantes, en caso necesario
                                oProductiveUnitModePosition.ShiftData.ExistComplementaryData = roTypes.Any2Boolean(oRowDetail("ExistComplementaryDataShift"))
                                oProductiveUnitModePosition.ShiftData.ExistFloatingData = roTypes.Any2Boolean(oRowDetail("ExistFloatingDataShift"))

                                roProductiveUnitModePositionManager.AssignFloatingComplementaryData(oProductiveUnitModePosition.ShiftData, oRowDetail, New roProductiveUnitState)

                                ' Asignamos el descanso definido
                                oProductiveUnitModePosition.ShiftData.BreakHours = roTypes.Any2Time(roTypes.Any2Double(oRowDetail("BreakHoursShift"))).Minutes

                                If bolLoadShiftHourData Or _detailLevel = BudgetDetailLevel.Hour Then
                                    ' Cargamos el detalle de las tramos de la posicion del modo del presupuesto
                                    oProductiveUnitModePosition.ShiftHourData = roProductiveUnitModePositionManager.LoadTheoricLayers(oProductiveUnitModePosition, New roProductiveUnitState)
                                End If

                                ' Datos del puesto asignado
                                oProductiveUnitModePosition.AssignmentData = New roCalendarAssignmentCellData
                                oProductiveUnitModePosition.AssignmentData.ID = roTypes.Any2Double(oRowDetail("IDAssignment"))
                                oProductiveUnitModePosition.AssignmentData.Name = roTypes.Any2String(oRowDetail("AssignmentName"))
                                oProductiveUnitModePosition.AssignmentData.ShortName = roTypes.Any2String(oRowDetail("AssignmentShortName"))
                                oProductiveUnitModePosition.AssignmentData.Color = HexConverter(System.Drawing.ColorTranslator.FromWin32(roTypes.Any2Integer(oRowDetail("AssignmentColor"))))

                                ' Cargar los datos de los empleados asignados a la posicion
                                oRowEmployeeDetails = tbEmployeePositions.Select("IDDailyBudgetPosition=" & roTypes.Any2String(oRowDetail("IDPosition")))

                                oProductiveUnitModePosition.EmployeesData = roProductiveUnitModePositionManager.LoadEmployeesData(oRowEmployeeDetails, New roProductiveUnitState, _detailLevel, oShiftCache)
                                If oProductiveUnitModePosition.EmployeesData IsNot Nothing AndAlso oProductiveUnitModePosition.EmployeesData.Length > 0 Then bolAssignedEmployees = True

                                ' Calculamos el % de cobertura de la posicion
                                oProductiveUnitModePosition.Coverage = 100
                                If oProductiveUnitModePosition.Quantity > 0 Then
                                    oProductiveUnitModePosition.Coverage = Math.Round((oProductiveUnitModePosition.EmployeesData.ToList.FindAll(Function(x) x.Alerts.OnAbsenceDays = False).Count * 100) / oProductiveUnitModePosition.Quantity, 2)
                                End If

                                oProductiveUnitModePosition.Alerts = New roCalendarRowDayAlerts
                                oProductiveUnitModePosition.Alerts.OnAbsenceDays = oProductiveUnitModePosition.EmployeesData.ToList.FindAll(Function(x) x.Alerts.OnAbsenceDays = True).Count
                                'oProductiveUnitModePosition.Alerts.UnexpectedlyAbsent = oProductiveUnitModePosition.EmployeesData.ToList.FindAll(Function(x) x.Alerts.UnexpectedlyAbsent = True).Count

                                oProductiveUnitModePositions.Add(oProductiveUnitModePosition)
                            Next
                        End If

                        ' Asignamos las posiciones
                        oBudgetCell.ProductiveUnitMode.UnitModePositions = oProductiveUnitModePositions.ToArray()

                        ' Si estamos en la vista planificacion
                        Dim intCoveragePositions As Integer = 0
                        If _typeView = BudgetView.Planification Then
                            ' Calculamos el % de cobertura de las posiciones asignadas
                            Dim intOverCoveragePosition As Integer = 0
                            oBudgetCell.ProductiveUnitMode.Coverage = 100
                            If oBudgetCell.ProductiveUnitMode.UnitModePositions.Count > 0 Then
                                For Each oPosition As roProductiveUnitModePosition In oBudgetCell.ProductiveUnitMode.UnitModePositions
                                    If oPosition.Coverage >= 100 Then intCoveragePositions += 1
                                    If oPosition.Coverage > 100 Then intOverCoveragePosition += 1
                                Next
                                oBudgetCell.ProductiveUnitMode.Coverage = Math.Round((intCoveragePositions * 100) / oBudgetCell.ProductiveUnitMode.UnitModePositions.Count, 2)

                                ' En el caso que todas las posiciones esten cubiertas y haya alguna que ademas este sobrecubierta
                                ' el % lo incrementamos
                                If intCoveragePositions = oBudgetCell.ProductiveUnitMode.UnitModePositions.Count And intOverCoveragePosition > 0 Then
                                    oBudgetCell.ProductiveUnitMode.Coverage = 100 + Math.Round((intOverCoveragePosition * 100) / intCoveragePositions, 2)
                                End If
                            End If
                        End If

                    End If

                    ' Si el supervisor tiene permisos de escritura
                    ' lo marcamos para que se pueda modificar
                    If oPermission > 3 Then oBudgetCell.CanBeModified = True

                    'Si estamos en la vista definicion y ya tiene empleados asignados no se puede modificar el modo asignado
                    If _typeView = BudgetView.Definition And bolAssignedEmployees Then
                        oBudgetCell.CanBeModified = False
                    End If

                    ' Añadimos los datos de la celda al calendario de la UP
                    oBudgetPeriodCells.Add(oBudgetCell)

                    ' vamos al siguiente día
                    dAct = dAct.AddDays(1)
                End While

                oRet.DayData = oBudgetPeriodCells.ToArray

                bolRet = True
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roBudgetRowPeriodDataManager::LoadCellsByBudget")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roBudgetRowPeriodDataManager::LoadCellsByBudget")
            End Try

            Return oRet

        End Function

#End Region

    End Class

End Namespace