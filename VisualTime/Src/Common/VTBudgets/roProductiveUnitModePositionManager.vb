Imports System.Data.Common
Imports System.Drawing
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTCalendar
Imports Robotics.Base.VTEmployees
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.roTypes

Namespace VTBudgets

    Public Class roProductiveUnitModePositionManager
        Private oState As roProductiveUnitState = Nothing

        Public Sub New()
            Me.oState = New roProductiveUnitState()
        End Sub

        Public Sub New(ByVal _State As roProductiveUnitState)
            Me.oState = _State
        End Sub

#Region "Methods"

        ''' <summary>
        ''' Carga datos de la posicion del modo
        ''' </summary>
        ''' <param name="idProductiveUnitModePosition"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function LoadProductiveUnitModePosition(idProductiveUnitModePosition As Long, Optional ByVal bAudit As Boolean = False) As roProductiveUnitModePosition

            Dim bolRet As New roProductiveUnitModePosition

            oState.Result = ProductiveUnitResultEnum.NoError

            bolRet.ID = -1

            If idProductiveUnitModePosition > 0 Then

                Try

                    Dim strSQL As String = "@SELECT# UMP.ID, UMP.IDMode, UMP.Quantity, UMP.IsExpandable, UMP.IDShift, UMP.StartShift, UMP.LayersDefinition, isnull(UMP.ExpectedWorkingHours, Sh.ExpectedWorkingHours) as ExpectedWorkingHours , UMP.IDAssignment " &
                                            " , Sh.Name as ShiftName, Sh.Color as ShiftColor, Sh.ShortName as ShiftShortName, Sh.IsFloating  as ShiftIsFloating, Sh.ShiftType  As ShiftType, Sh.StartLimit As ShiftStartLimit, ISNULL(Sh.AreWorkingDays,0) As ShiftAreWorkingDays " &
                                        " , Sh.StartLimit As StartLimitBase, Sh.AllowComplementary as ShiftExistComplementaryData, Sh.AllowFloatingData as ShiftExistFloatingData, Sh.BreakHours As ShiftBreakHours " &
                                        " , AM.Name as AssignmentName , AM.ShortName as AssignmentShortName, AM.Color as AssignmentColor" &
                                        " FROM ProductiveUnit_Mode_Positions UMP, Shifts Sh, Assignments AM  WHERE UMP.IDShift = Sh.ID AND UMP.IDAssignment = AM.ID AND  UMP.ID=" & idProductiveUnitModePosition.ToString
                    Dim tb As DataTable = CreateDataTable(strSQL, )
                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                        bolRet.ID = Any2Long(tb.Rows(0)("ID"))
                        bolRet.IDProductiveUnitMode = Any2Long(tb.Rows(0)("IDMode"))
                        bolRet.Quantity = Any2Double(tb.Rows(0)("Quantity"))
                        bolRet.IsExpandable = Any2Boolean(tb.Rows(0)("IsExpandable"))

                        ' Datos del horario
                        bolRet.ShiftData = New roCalendarRowShiftData
                        bolRet.ShiftData.ID = roTypes.Any2Integer(tb.Rows(0)("IDShift"))
                        bolRet.ShiftData.Name = roTypes.Any2String(tb.Rows(0)("ShiftName"))
                        Dim auxColor As System.Drawing.Color
                        auxColor = System.Drawing.ColorTranslator.FromWin32(roTypes.Any2Integer(tb.Rows(0)("ShiftColor")))
                        bolRet.ShiftData.Color = HexConverter(auxColor)
                        bolRet.ShiftData.PlannedHours = roTypes.Any2Time(roTypes.Any2Double(tb.Rows(0)("ExpectedWorkingHours"))).Minutes
                        bolRet.ShiftData.ShortName = roTypes.Any2String(tb.Rows(0)("ShiftShortName"))
                        If Not roTypes.Any2Boolean(tb.Rows(0)("ShiftIsFloating")) Then
                            Select Case roTypes.Any2Integer(tb.Rows(0)("ShiftType"))
                                Case 0, 1  'Normal
                                    bolRet.ShiftData.Type = ShiftTypeEnum.Normal
                                    bolRet.ShiftData.StartHour = roTypes.Any2Time(tb.Rows(0)("ShiftStartLimit")).Value
                                Case 2  'Vacaciones
                                    bolRet.ShiftData.Type = IIf(roTypes.Any2Boolean(tb.Rows(0)("ShiftAreWorkingDays")), ShiftTypeEnum.Holiday_Working, ShiftTypeEnum.Holiday_NoWorking)
                            End Select
                        Else
                            ' Flotante
                            bolRet.ShiftData.Type = ShiftTypeEnum.NormalFloating
                            bolRet.ShiftData.StartHour = roTypes.Any2Time(tb.Rows(0)("StartShift")).Value
                        End If

                        ' Asignamos los datos de complementarias o de flotantes, en caso necesario
                        bolRet.ShiftData.ExistComplementaryData = roTypes.Any2Boolean(tb.Rows(0)("ShiftExistComplementaryData"))
                        bolRet.ShiftData.ExistFloatingData = roTypes.Any2Boolean(tb.Rows(0)("ShiftExistFloatingData"))
                        AssignFloatingComplementaryData(bolRet.ShiftData, tb.Rows(0), oState)

                        ' Asignamos el descanso definido
                        bolRet.ShiftData.BreakHours = roTypes.Any2Time(roTypes.Any2Double(tb.Rows(0)("ShiftBreakHours"))).Minutes

                        ' Datos del puesto asignado
                        bolRet.AssignmentData = New roCalendarAssignmentCellData
                        bolRet.AssignmentData.ID = roTypes.Any2Double(tb.Rows(0)("IDAssignment"))
                        bolRet.AssignmentData.Name = roTypes.Any2String(tb.Rows(0)("AssignmentName"))
                        bolRet.AssignmentData.ShortName = roTypes.Any2String(tb.Rows(0)("AssignmentShortName"))
                        bolRet.AssignmentData.Color = HexConverter(System.Drawing.ColorTranslator.FromWin32(roTypes.Any2Integer(tb.Rows(0)("AssignmentColor"))))
                    End If

                    If bAudit Then
                        ' Auditar lectura
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        oState.AddAuditParameter(tbParameters, "{ID}", bolRet.ID, "", 1)

                        Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tProductiveUnitModePosition, bolRet.ID, tbParameters, -1)
                    End If
                Catch ex As Data.Common.DbException
                    Me.oState.UpdateStateInfo(ex, "roProductiveUnitModePositionManager::LoadProductiveUnitModePosition")
                Catch ex As Exception
                    Me.oState.UpdateStateInfo(ex, "roProductiveUnitModePositionManager::LoadProductiveUnitModePosition")
                Finally

                End Try

            End If

            Return bolRet

        End Function

        ''' <summary>
        ''' Guarda una posicion de un modo de una unidad productiva
        ''' </summary>
        ''' <param name="oProductiveUnitModePosition"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function SaveProductiveUnitModePosition(oProductiveUnitModePosition As roProductiveUnitModePosition, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False
            Dim bolIsNew As Boolean = False
            Dim bHaveToClose As Boolean = False

            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                If ValidateProductiveUnitModePosition(oProductiveUnitModePosition, Me.oState) Then

                    Dim tb As New DataTable("ProductiveUnitModePosition")
                    Dim strSQL As String = "@SELECT# * FROM ProductiveUnit_Mode_Positions WHERE ID=" & oProductiveUnitModePosition.ID.ToString
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
                        oRow("ID") = GetNextId()
                        oProductiveUnitModePosition.ID = oRow("ID")
                    End If

                    oRow("IDMode") = oProductiveUnitModePosition.IDProductiveUnitMode
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

                    bolRet = True

                    ' Auditamos
                    If bolRet And bAudit Then
                        oAuditDataNew = oRow
                        Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                        Extensions.roAudit.AddFieldsValues(tbAuditParameters, oAuditDataNew, oAuditDataOld)
                        Dim oAuditAction As Audit.Action = IIf(oAuditDataOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)

                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        Me.oState.Audit(If(bolActionInsert, Audit.Action.aInsert, Audit.Action.aUpdate), Audit.ObjectType.tProductiveUnitModePosition, oProductiveUnitModePosition.ID, tbParameters, -1)
                    End If

                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roProductiveUnitModePositionManager::SaveProductiveUnitModePosition")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roProductiveUnitModePositionManager::SaveProductiveUnitModePosition")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Elimina  una posicion de un modo de una unidad productiva
        ''' </summary>
        ''' <param name="oProductiveUnitModePosition"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function DeleteProductiveUnitModePosition(oProductiveUnitModePosition As roProductiveUnitModePosition, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False

            Try
                bolRet = ExecuteSql("@DELETE# ProductiveUnit_Mode_Positions WHERE ID=" & oProductiveUnitModePosition.ID)

                If bolRet And bAudit Then
                    ' Auditamos
                    bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tProductiveUnitModePosition, oProductiveUnitModePosition.ID, Nothing, -1)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roProductiveUnitModePositionManager::DeleteProductiveUnitModePosition")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roProductiveUnitModePositionManager::DeleteProductiveUnitModePosition")
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
                oState.UpdateStateInfo(ex, "roProductiveUnitModePositionManager::GetShiftLayerData")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roProductiveUnitModePositionManager::GetShiftLayerData")
            Finally

            End Try

            Return oXml
        End Function

#End Region

#Region "Helpers"

        Public Shared Function LoadPositionsByMode(ByVal _IDMode As Double, ByRef oState As roProductiveUnitState) As roProductiveUnitModePosition()
            ' obtenemos todas las posiciones del modo de la unidad productiva
            Dim oRet As New Generic.List(Of roProductiveUnitModePosition)
            Dim bolRet As Boolean = False

            Try
                Dim strQuery As String = String.Empty

                strQuery &= " @SELECT# ID FROM ProductiveUnit_Mode_Positions WHERE IDMode = " & _IDMode.ToString
                strQuery &= " ORDER BY ID"

                Dim dTbl As System.Data.DataTable = CreateDataTable(strQuery)

                Dim oProductiveUnitModePosition As roProductiveUnitModePosition = Nothing

                'Cargar los datos de las posiciones
                If dTbl IsNot Nothing Then
                    Dim intPos As Integer = 0

                    For Each oPosition As DataRow In dTbl.Rows
                        ' Añadimos los datos de la posicion
                        oProductiveUnitModePosition = New roProductiveUnitModePosition

                        Dim oUnitModePositionManager As New roProductiveUnitModePositionManager
                        oProductiveUnitModePosition = oUnitModePositionManager.LoadProductiveUnitModePosition(Any2Long(oPosition("ID")))

                        oRet.Add(oProductiveUnitModePosition)
                    Next
                End If

                bolRet = True
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roProductiveUnitModePositionManager::LoadPositionsByMode")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roProductiveUnitModePositionManager::LoadPositionsByMode")
            End Try

            Return oRet.ToArray

        End Function

        Public Shared Function ValidateProductiveUnitModePosition(ByVal oProductiveUnitModePosition As roProductiveUnitModePosition, ByVal _State As roProductiveUnitState) As Boolean

            Dim bolRet As Boolean = True
            Dim strSQL As String = ""
            Try

                _State.Result = ProductiveUnitResultEnum.NoError

                If bolRet Then
                    'validar que la cantidad sea mayor a 0
                    If oProductiveUnitModePosition.Quantity <= 0 And Not oProductiveUnitModePosition.IsExpandable Then
                        _State.Result = ProductiveUnitResultEnum.UP_Mode_Position_InvalidQuantity
                        bolRet = False
                    End If
                End If

                If bolRet Then
                    'validar que el puesto existe un puesto asignado
                    If oProductiveUnitModePosition.AssignmentData IsNot Nothing AndAlso oProductiveUnitModePosition.AssignmentData.ID = 0 Then
                        _State.Result = ProductiveUnitResultEnum.UP_Mode_Position_InvalidAssignment
                        bolRet = False
                    End If
                End If

                If bolRet Then
                    'validar que exista un horario asignado
                    If oProductiveUnitModePosition.ShiftData IsNot Nothing AndAlso oProductiveUnitModePosition.ShiftData.ID = 0 Then
                        _State.Result = ProductiveUnitResultEnum.UP_Mode_Position_InvalidShift
                        bolRet = False
                    End If
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roProductiveUnitModePosition::ValidateProductiveUnitModePosition")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roProductiveUnitModePosition::ValidateProductiveUnitModePosition")
            End Try

            Return bolRet

        End Function

        Public Shared Function LoadTheoricLayers(ByVal oProductiveUnitModePosition As roProductiveUnitModePosition, ByRef _State As roProductiveUnitState) As roCalendarRowHourData()

            Dim oRet As roCalendarRowHourData() = Nothing

            _State.Result = ProductiveUnitResultEnum.NoError

            Try

                oRet = VTCalendar.roCalendarRowHourDataManager.LoadEmtyData()

                ' Obtenemos los tramos para ese dia y empleado en funcion del horario asignado
                Dim oCalendarRowHourDataState As New roCalendarRowHourDataState(_State.IDPassport)
                Dim oCalendarRowHourData As New roCalendarRowHourDataManager(oCalendarRowHourDataState)

                Dim oShiftState As New VTBusiness.Shift.roShiftState(-1)
                roBusinessState.CopyTo(_State, oShiftState)
                Dim oShift As New VTBusiness.Shift.roShift(oProductiveUnitModePosition.ShiftData.ID, oShiftState, False)

                oRet = oCalendarRowHourData.LoadTheoricLayers(DateTime.Now.Date, oShift, oProductiveUnitModePosition.ShiftData, oProductiveUnitModePosition.ShiftData.StartHour)
            Catch ex As Data.Common.DbException
                _State.UpdateStateInfo(ex, "roProductiveUnitModePositionManager::LoadTheoricLayers")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roProductiveUnitModePositionManager::LoadTheoricLayers")
            Finally

            End Try

            Return oRet

        End Function

        Private Function GetNextId() As Long
            Dim intRet As Long = 0

            Try
                Dim strQry As String = "@SELECT# (ISNULL(MAX(ID), 0) + 1) FROM ProductiveUnit_Mode_Positions "
                intRet = roTypes.Any2Long(ExecuteScalar(strQry))
            Catch ex As Data.Common.DbException
                Me.oState.UpdateStateInfo(ex, "roProductiveUnitModePositionManager::GetNextIDDailyBudget")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roProductiveUnitModePositionManager::GetNextIDDailyBudget")
            End Try

            Return intRet

        End Function

        Public Shared Function HexConverter(c As System.Drawing.Color) As String
            Return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2")
        End Function

        Public Shared Sub AssignFloatingComplementaryData(ByRef oCalendarRowShiftData As roCalendarRowShiftData, ByVal oRow As DataRow, ByRef _State As roProductiveUnitState)
            Try

                If oCalendarRowShiftData.ExistFloatingData Or oCalendarRowShiftData.ExistComplementaryData Then
                    Dim oCalendarShiftLayersDefinitionList As New List(Of roCalendarShiftLayersDefinition)

                    Dim oLayersDefinition As New roCollection(roTypes.Any2String(oRow("LayersDefinition")))
                    If oLayersDefinition.Exists("TotalLayers") Then
                        Dim intTotalLayers As Integer = roTypes.Any2Integer(oLayersDefinition("TotalLayers"))
                        oCalendarRowShiftData.ShiftLayers = intTotalLayers
                        For iLayer As Integer = 1 To intTotalLayers
                            Dim oCalendarShiftLayersDefinition As New roCalendarShiftLayersDefinition
                            If oLayersDefinition.Exists("LayerID_" & iLayer.ToString) Then oCalendarShiftLayersDefinition.LayerID = roTypes.Any2Integer(oLayersDefinition("LayerID_" & iLayer.ToString))

                            ' Datos flotantes
                            If oCalendarRowShiftData.ExistFloatingData Then
                                If oLayersDefinition.Exists("LayerFloatingDuration_" & iLayer.ToString) Then oCalendarShiftLayersDefinition.LayerDuration = roTypes.Any2Double(oLayersDefinition("LayerFloatingDuration_" & iLayer.ToString))
                                If oLayersDefinition.Exists("LayerFloatingBeginTime_" & iLayer.ToString) Then
                                    oCalendarShiftLayersDefinition.LayerStartTime = roTypes.Any2Time(oLayersDefinition("LayerFloatingBeginTime_" & iLayer.ToString)).Value
                                    If iLayer = 1 Then
                                        ' Si es la primera franja debemos modificar la hora de inicio del horario, siempre y cuando no sea flotante
                                        If oCalendarRowShiftData.Type <> ShiftTypeEnum.NormalFloating Then oCalendarRowShiftData.StartHour = oCalendarShiftLayersDefinition.LayerStartTime
                                    End If
                                End If
                            End If

                            ' Datos complementarios
                            If oCalendarRowShiftData.ExistComplementaryData Then
                                If oLayersDefinition.Exists("LayerComplementaryHours_" & iLayer.ToString) Then oCalendarShiftLayersDefinition.LayerComplementaryHours = roTypes.Any2Time(roTypes.Any2Double(oLayersDefinition("LayerComplementaryHours_" & iLayer.ToString))).Minutes
                                If oLayersDefinition.Exists("LayerOrdinaryHours_" & iLayer.ToString) Then oCalendarShiftLayersDefinition.LayerOrdinaryHours = roTypes.Any2Time(roTypes.Any2Double(oLayersDefinition("LayerOrdinaryHours_" & iLayer.ToString))).Minutes
                            End If

                            oCalendarShiftLayersDefinitionList.Add(oCalendarShiftLayersDefinition)
                        Next

                    End If

                    oCalendarRowShiftData.ShiftLayersDefinition = oCalendarShiftLayersDefinitionList.ToArray
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roProductiveUnitModePositionManager::AssignFloatingComplementaryData")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roProductiveUnitModePositionManager::AssignFloatingComplementaryData")

            End Try

        End Sub

        Public Shared Function LoadEmployeesData(ByVal _DataRowPosition As DataRow(), ByRef _State As roProductiveUnitState, ByVal _detailLevel As BudgetDetailLevel, Optional ByRef oShiftCache As Hashtable = Nothing) As roProductiveUnitModePositionEmployeeData()
            ' Cargamos los datos de los empleados asignados a la posición

            Dim oRet As New Generic.List(Of roProductiveUnitModePositionEmployeeData)

            Dim tbPlan As DataTable = Nothing
            Dim bolRet As Boolean = False

            Dim tbProgrammedAbsences As DataTable = Nothing
            Dim oRows() As DataRow = Nothing

            Try
                Dim oShift As VTBusiness.Shift.roShift = Nothing

                If _DataRowPosition IsNot Nothing Then
                    For Each oRowDetail As DataRow In _DataRowPosition
                        ' Para cada empleado asignado, obtenemos los datos de planificación definidos
                        Dim oProductiveUnitModePositionEmployeeData As New roProductiveUnitModePositionEmployeeData
                        Dim auxColor As System.Drawing.Color = Color.White
                        oProductiveUnitModePositionEmployeeData.EmployeeName = Any2String(oRowDetail("EmployeeName"))
                        oProductiveUnitModePositionEmployeeData.IDEmployee = Any2Integer(oRowDetail("IDEmployee"))
                        oProductiveUnitModePositionEmployeeData.ShiftData = New roCalendarRowShiftData

                        oProductiveUnitModePositionEmployeeData.ShiftData.ID = Any2Integer(oRowDetail("IDShift1"))
                        oProductiveUnitModePositionEmployeeData.ShiftData.Name = Any2String(oRowDetail("NameShift1"))
                        auxColor = System.Drawing.ColorTranslator.FromWin32(Any2Integer(oRowDetail("ShiftColor1")))
                        oProductiveUnitModePositionEmployeeData.ShiftData.Color = HexConverter(auxColor)
                        oProductiveUnitModePositionEmployeeData.ShiftData.PlannedHours = Any2Time(Any2Double(oRowDetail("ExpectedWorkingHours1"))).Minutes
                        oProductiveUnitModePositionEmployeeData.ShiftData.ShortName = Any2String(oRowDetail("ShortName1"))
                        If Not Any2Boolean(oRowDetail("IsFloating1")) Then
                            Select Case Any2Integer(oRowDetail("ShiftType1"))
                                Case 0, 1  'Normal
                                    oProductiveUnitModePositionEmployeeData.ShiftData.Type = ShiftTypeEnum.Normal
                                    oProductiveUnitModePositionEmployeeData.ShiftData.StartHour = Any2Time(oRowDetail("StartLimit1")).Value
                                Case 2  'Vacaciones
                                    oProductiveUnitModePositionEmployeeData.ShiftData.Type = IIf(Any2Boolean(oRowDetail("AreWorkingDays1")), ShiftTypeEnum.Holiday_Working, ShiftTypeEnum.Holiday_NoWorking)
                                    oProductiveUnitModePositionEmployeeData.ShiftData.StartHour = Any2Time(oRowDetail("StartLimit1")).Value
                            End Select
                        Else
                            ' Flotante
                            oProductiveUnitModePositionEmployeeData.ShiftData.Type = ShiftTypeEnum.NormalFloating
                            oProductiveUnitModePositionEmployeeData.ShiftData.StartHour = Any2Time(oRowDetail("StartShift1")).Value
                        End If

                        ' Asignamos los datos de complementarias o de flotantes, en caso necesario
                        oProductiveUnitModePositionEmployeeData.ShiftData.ExistComplementaryData = Any2Boolean(oRowDetail("ExistComplementaryDataShift1"))
                        oProductiveUnitModePositionEmployeeData.ShiftData.ExistFloatingData = Any2Boolean(oRowDetail("ExistFloatingData"))

                        roProductiveUnitModePositionManager.AssignFloatingComplementaryData(oProductiveUnitModePositionEmployeeData.ShiftData, oRowDetail, New roProductiveUnitState)

                        ' Asignamos el descanso definido
                        oProductiveUnitModePositionEmployeeData.ShiftData.BreakHours = Any2Time(Any2Double(oRowDetail("BreakHours1"))).Minutes

                        ' Añadimos las alertas del empleado
                        oProductiveUnitModePositionEmployeeData.Alerts = New roCalendarRowDayAlerts

                        ' Verificamos si ese dia esta de ausencia

                        'If oRowDetail("Date") = Now.Date Then
                        '    ' En el caso que cargamos los datos del dia de hoy, revisamos si el empleado esta ausente
                        '    Dim oEmployeeStatus As VTBusiness.Employee.roEmployeeStatus
                        '    oEmployeeStatus = New VTBusiness.Employee.roEmployeeStatus(Any2Integer(oRowDetail("IDEmployee")), New VTBusiness.Employee.roEmployeeState(-1))

                        '    If Not oEmployeeStatus.IsPresent AndAlso (Not oEmployeeStatus.BeginMandatory.HasValue OrElse oEmployeeStatus.BeginMandatory.Value < Now) Then
                        '        oProductiveUnitModePositionEmployeeData.Alerts.OnAbsenceDays = True
                        '    End If
                        'ElseIf oRowDetail("Date") > Now.Date Then
                        If Any2Double(oRowDetail("TotalAbsences")) > 0 Then oProductiveUnitModePositionEmployeeData.Alerts.OnAbsenceDays = True
                        'End If

                        If _detailLevel = BudgetDetailLevel.Hour Then
                            ' Obtenemos los tramos del horario asignado para ese dia
                            ' en caso de necesitar cargar los datos (vista diaria)
                            Dim oCalendarRowHourDataState As New roCalendarRowHourDataState(_State.IDPassport)
                            Dim oCalendarRowHourData As New roCalendarRowHourDataManager(oCalendarRowHourDataState)

                            ' En caso necesario lo añadimos al cache de horarios
                            If Not oShiftCache.Contains(Any2Integer(oRowDetail("IDShift1"))) Then
                                oShift = New VTBusiness.Shift.roShift(Any2Integer(oRowDetail("IDShift1")), New VTBusiness.Shift.roShiftState(_State.IDPassport))
                                oShiftCache.Add(oShift.ID, oShift)
                            End If

                            oShift = oShiftCache(Any2Integer(oRowDetail("IDShift1")))
                            oProductiveUnitModePositionEmployeeData.HourData = oCalendarRowHourData.Load(oRowDetail("Date"), oProductiveUnitModePositionEmployeeData.IDEmployee, oShift, oProductiveUnitModePositionEmployeeData.ShiftData.StartHour, Nothing, oProductiveUnitModePositionEmployeeData.ShiftData, Nothing, New Generic.List(Of roProgrammedHoliday), New Generic.List(Of roProgrammedOvertime))
                        End If

                        ' Añadimos los datos del empleado
                        oRet.Add(oProductiveUnitModePositionEmployeeData)
                    Next
                End If

                bolRet = True
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roProductiveUnitModePositionManager::LoadEmployeesData")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roProductiveUnitModePositionManager::LoadEmployeesData")
            End Try

            Return oRet.ToArray

        End Function

        Public Function GetEmployeesAvailableForPosition(ByVal _IDNode As Integer, ByVal xPlanDate As Date, ByVal oProductiveUnitModePosition As roProductiveUnitModePosition, Optional ByVal _strEmployeesNode As String = "") As roBudgetEmployeeAvailableForPosition()
            ' Obtenemos la lista de empleados que pueden cubrir una posicion
            Dim oRet As New Generic.List(Of roBudgetEmployeeAvailableForPosition)
            Dim bolRet As Boolean = False

            Try

                Dim oBudgetState As New roBudgetState(-1)
                roBusinessState.CopyTo(Me.oState, oBudgetState)

                ' Obtenemos los empleados del nodo que pueden realizar el puesto indicado
                'Dim strEmployees As String = roBudgetManager.GetEmployeeListFromNode(_IDNode, oBudgetState, xPlanDate)
                Dim strEmployees As String = _strEmployeesNode
                If strEmployees.Length = 0 Then strEmployees = roBudgetManager.GetEmployeeListFromNode(_IDNode, oBudgetState, xPlanDate, True)

                Dim strSQL As String

                If strEmployees.Length = 0 Then strEmployees = "-1"

                ' Filtro para empleados de ausencia prevista diaria
                Dim strSQLProgrammedAbsence As String
                strSQLProgrammedAbsence = " AND isnull((@SELECT# count(*) from ProgrammedAbsences WITH (NOLOCK) WHERE IDEmployee = sysrovwAllEmployeeGroups.IDEmployee AND "
                strSQLProgrammedAbsence &= " ( ( (BeginDate >= " & Any2Time(xPlanDate).SQLSmallDateTime & " AND BeginDate <= " & Any2Time(xPlanDate).SQLSmallDateTime & ")"
                strSQLProgrammedAbsence &= " OR "
                strSQLProgrammedAbsence &= " (ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) >= " & Any2Time(xPlanDate).SQLSmallDateTime & " AND ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) <= " & Any2Time(xPlanDate).SQLSmallDateTime & ")"
                strSQLProgrammedAbsence &= " OR "
                strSQLProgrammedAbsence &= " (BeginDate <= " & Any2Time(xPlanDate).SQLSmallDateTime & " AND ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) >= " & Any2Time(xPlanDate).SQLSmallDateTime & ")))),0) = 0"

                ' Obtenemos el campo a utilizar para el coste del empleado
                Dim oParameters As New roParameters("OPTIONS", True)

                Dim strCostField As String = roTypes.Any2String(oParameters.Parameter(Parameters.EmployeeFieldCost))
                strCostField = strCostField.Replace("USR_", "")

                Dim strEmployeeCost As String
                If strCostField.Length > 0 Then
                    strEmployeeCost = "(@SELECT# top(1) convert(numeric(18,6), convert(nvarchar(100),isnull(Value,''))) from EmployeeUserFieldValues WITH (NOLOCK) " &
                        " WHERE  FieldName='" & strCostField.Replace("'", "''") & "' AND EmployeeUserFieldValues.idEmployee=sysrovwAllEmployeeGroups.IDEmployee and Date<=" & roTypes.Any2Time(xPlanDate).SQLSmallDateTime & ") as 'Cost', "
                Else
                    strEmployeeCost = "'0.000' AS 'Cost', "
                End If

                ' Filtro para empleados ya asignados a una posicion
                Dim strDailyBudgetPosition As String
                strDailyBudgetPosition = " AND ISNULL((@SELECT# COUNT(*) FROM DailySchedule WITH (NOLOCK) WHERE DailySchedule.IDEmployee = sysrovwAllEmployeeGroups.IDEmployee AND DailySchedule.Date = " & Any2Time(xPlanDate).SQLSmallDateTime & " AND ISNULL(DailySchedule.IDDailyBudgetPosition, 0) > 0), 0) = 0  "

                strEmployees = "(" & strEmployees & ")"

                ' Empleados con el horario asignado y puesto asignado sin asignar a ningun presupuesto
                strSQL = "@SELECT# sysrovwAllEmployeeGroups.IDEmployee, sysrovwAllEmployeeGroups.EmployeeName, " &
                                "DailySchedule.IDAssignment AS 'IDAssignment', Assignments.Name AS 'AssignmentName', " &
                                "DailySchedule.IDShift1 AS 'IDShift1', Shifts.Name AS 'ShiftName', " &
                                "sysrovwAllEmployeeGroups.IDGroup, sysrovwAllEmployeeGroups.FullGroupName, " &
                                strEmployeeCost &
                                "100000  AS 'OrderField'  " &
                        "FROM sysrovwAllEmployeeGroups WITH (NOLOCK)  INNER JOIN DailySchedule WITH (NOLOCK) " &
                                "ON DailySchedule.IDEmployee = sysrovwAllEmployeeGroups.IDEmployee " &
                                "INNER JOIN ShiftAssignments WITH (NOLOCK)  ON DailySchedule.IDShift1 = ShiftAssignments.IDShift " &
                                "INNER JOIN Shifts WITH (NOLOCK)  ON Shifts.ID = DailySchedule.IDShift1 " &
                                "INNER JOIN EmployeeAssignments WITH (NOLOCK)  ON EmployeeAssignments.IDEmployee = sysrovwAllEmployeeGroups.IDEmployee " &
                                "INNER JOIN Assignments WITH (NOLOCK)  ON EmployeeAssignments.IDAssignment = Assignments.ID " &
                                "INNER JOIN EmployeeContracts WITH (NOLOCK)  ON sysrovwAllEmployeeGroups.IDEmployee = EmployeeContracts.IDEmployee " &
                        "WHERE sysrovwAllEmployeeGroups.IDEmployee IN " & strEmployees & " AND " &
                            "sysrovwAllEmployeeGroups.BeginDate <= " & Any2Time(xPlanDate).SQLSmallDateTime & " AND sysrovwAllEmployeeGroups.EndDate >= " & Any2Time(xPlanDate).SQLSmallDateTime & " AND " &
                            "DailySchedule.Date = " & Any2Time(xPlanDate).SQLSmallDateTime & " AND DailySchedule.IDSHift1 = " & oProductiveUnitModePosition.ShiftData.ID & " AND ShiftAssignments.IDAssignment = " & oProductiveUnitModePosition.AssignmentData.ID & " AND DailySchedule.IDAssignment = " & oProductiveUnitModePosition.AssignmentData.ID & " AND EmployeeAssignments.IDAssignment = " & oProductiveUnitModePosition.AssignmentData.ID & " AND " &
                            "(EmployeeContracts.EndDate >= " & Any2Time(xPlanDate).SQLSmallDateTime & " ) AND (EmployeeContracts.BeginDate <= " & Any2Time(xPlanDate).SQLSmallDateTime & ")  " &
                        strDailyBudgetPosition &
                        strSQLProgrammedAbsence

                strSQL &= " UNION "

                ' Empleados sin horario asignado y que puedan cubrir el puesto/horario
                strSQL &= "@SELECT# sysrovwAllEmployeeGroups.IDEmployee, sysrovwAllEmployeeGroups.EmployeeName, " &
                        "NULL AS 'IDAssignment', '' AS 'AssignmentName', " &
                        "NULL AS 'IDShift1', '' AS 'ShiftName', " &
                        "sysrovwAllEmployeeGroups.IDGroup, sysrovwAllEmployeeGroups.FullGroupName, " &
                        strEmployeeCost &
                        "80000 AS 'OrderField' " &
                        "FROM sysrovwAllEmployeeGroups WITH (NOLOCK)  INNER JOIN EmployeeContracts WITH (NOLOCK) ON sysrovwAllEmployeeGroups.IDEmployee = EmployeeContracts.IDEmployee " &
                        "WHERE sysrovwAllEmployeeGroups.IDEmployee IN " & strEmployees & " AND " &
                        "sysrovwAllEmployeeGroups.BeginDate <= " & Any2Time(xPlanDate).SQLSmallDateTime & "  AND sysrovwAllEmployeeGroups.EndDate >= " & Any2Time(xPlanDate).SQLSmallDateTime & "  AND " &
                        "ISNULL((@SELECT# COUNT(*) FROM DailySchedule WITH (NOLOCK) WHERE DailySchedule.IDEmployee = sysrovwAllEmployeeGroups.IDEmployee AND DailySchedule.Date = " & Any2Time(xPlanDate).SQLSmallDateTime & " AND ISNULL(DailySchedule.IDShift1, 0) > 0), 0) = 0 AND " &
                        "(@SELECT# COUNT(*) FROM EmployeeAssignments WITH (NOLOCK)  WHERE EmployeeAssignments.IDEmployee = sysrovwAllEmployeeGroups.IDEmployee AND EmployeeAssignments.IDAssignment = " & oProductiveUnitModePosition.AssignmentData.ID & " ) = 1 AND " &
                        "(EmployeeContracts.EndDate >= " & Any2Time(xPlanDate).SQLSmallDateTime & ") AND (EmployeeContracts.BeginDate <= " & Any2Time(xPlanDate).SQLSmallDateTime & ")  " &
                        strDailyBudgetPosition &
                        strSQLProgrammedAbsence

                strSQL &= " UNION "

                ' Empleados con el horario asignado y diferente puesto asignado sin asignar a ningun presupuesto
                strSQL &= "@SELECT# sysrovwAllEmployeeGroups.IDEmployee, sysrovwAllEmployeeGroups.EmployeeName, " &
                                "DailySchedule.IDAssignment AS 'IDAssignment', (@SELECT# Name from Assignments where id=DailySchedule.IDAssignment) AS 'AssignmentName', " &
                                "DailySchedule.IDShift1 AS 'IDShift1', Shifts.Name AS 'ShiftName', " &
                                "sysrovwAllEmployeeGroups.IDGroup, sysrovwAllEmployeeGroups.FullGroupName, " &
                                strEmployeeCost &
                                "60000 AS 'OrderField'  " &
                        "FROM sysrovwAllEmployeeGroups WITH (NOLOCK) INNER JOIN DailySchedule WITH (NOLOCK) " &
                                "ON DailySchedule.IDEmployee = sysrovwAllEmployeeGroups.IDEmployee " &
                                "INNER JOIN ShiftAssignments WITH (NOLOCK) ON DailySchedule.IDShift1 = ShiftAssignments.IDShift " &
                                "INNER JOIN Shifts WITH (NOLOCK) ON Shifts.ID = DailySchedule.IDShift1 " &
                                "INNER JOIN EmployeeAssignments WITH (NOLOCK) ON EmployeeAssignments.IDEmployee = sysrovwAllEmployeeGroups.IDEmployee " &
                                "INNER JOIN Assignments WITH (NOLOCK)  ON EmployeeAssignments.IDAssignment = Assignments.ID " &
                                "INNER JOIN EmployeeContracts WITH (NOLOCK) ON sysrovwAllEmployeeGroups.IDEmployee = EmployeeContracts.IDEmployee " &
                        "WHERE sysrovwAllEmployeeGroups.IDEmployee IN " & strEmployees & " AND " &
                            "sysrovwAllEmployeeGroups.BeginDate <= " & Any2Time(xPlanDate).SQLSmallDateTime & " AND sysrovwAllEmployeeGroups.EndDate >= " & Any2Time(xPlanDate).SQLSmallDateTime & " AND " &
                            "DailySchedule.Date = " & Any2Time(xPlanDate).SQLSmallDateTime & " AND isnull(DailySchedule.IDSHift1,0) = " & oProductiveUnitModePosition.ShiftData.ID & " AND isnull(DailySchedule.IDAssignment,0) <> " & oProductiveUnitModePosition.AssignmentData.ID & " AND EmployeeAssignments.IDAssignment = " & oProductiveUnitModePosition.AssignmentData.ID & " AND " &
                            "(EmployeeContracts.EndDate >= " & Any2Time(xPlanDate).SQLSmallDateTime & " ) AND (EmployeeContracts.BeginDate <= " & Any2Time(xPlanDate).SQLSmallDateTime & ") " &
                        strDailyBudgetPosition &
                        strSQLProgrammedAbsence

                strSQL &= " UNION "

                ' Empleados con el horario diferente al indicado pero que puedan hacer el puesto indicado sin asignar a ningun presupuesto
                strSQL &= "@SELECT# sysrovwAllEmployeeGroups.IDEmployee, sysrovwAllEmployeeGroups.EmployeeName, " &
                                "DailySchedule.IDAssignment AS 'IDAssignment', (@SELECT# Name from Assignments where id=DailySchedule.IDAssignment) AS 'AssignmentName', " &
                                "DailySchedule.IDShift1 AS 'IDShift1', Shifts.Name AS 'ShiftName', " &
                                "sysrovwAllEmployeeGroups.IDGroup, sysrovwAllEmployeeGroups.FullGroupName, " &
                                strEmployeeCost &
                                "40000  AS 'OrderField'  " &
                        "FROM sysrovwAllEmployeeGroups WITH (NOLOCK) INNER JOIN DailySchedule WITH (NOLOCK) " &
                                "ON DailySchedule.IDEmployee = sysrovwAllEmployeeGroups.IDEmployee " &
                                "INNER JOIN Shifts WITH (NOLOCK) ON Shifts.ID = DailySchedule.IDShift1 " &
                                "INNER JOIN EmployeeAssignments WITH (NOLOCK) ON EmployeeAssignments.IDEmployee = sysrovwAllEmployeeGroups.IDEmployee " &
                                "INNER JOIN Assignments WITH (NOLOCK)  ON EmployeeAssignments.IDAssignment = Assignments.ID " &
                                "INNER JOIN EmployeeContracts WITH (NOLOCK) ON sysrovwAllEmployeeGroups.IDEmployee = EmployeeContracts.IDEmployee " &
                        "WHERE sysrovwAllEmployeeGroups.IDEmployee IN " & strEmployees & " AND " &
                            "sysrovwAllEmployeeGroups.BeginDate <= " & Any2Time(xPlanDate).SQLSmallDateTime & " AND sysrovwAllEmployeeGroups.EndDate >= " & Any2Time(xPlanDate).SQLSmallDateTime & " AND " &
                            "DailySchedule.Date = " & Any2Time(xPlanDate).SQLSmallDateTime & " AND isnull(DailySchedule.LockedDay,0) = 0 AND isnull(DailySchedule.IDSHift1,0) <> " & oProductiveUnitModePosition.ShiftData.ID & " AND EmployeeAssignments.IDAssignment = " & oProductiveUnitModePosition.AssignmentData.ID & " AND " &
                            "(EmployeeContracts.EndDate >= " & Any2Time(xPlanDate).SQLSmallDateTime & " ) AND (EmployeeContracts.BeginDate <= " & Any2Time(xPlanDate).SQLSmallDateTime & ") " &
                        strDailyBudgetPosition &
                        strSQLProgrammedAbsence

                'strSQL &= "ORDER BY 'OrderField' DESC, EmployeeName"
                strSQL &= " ORDER BY EmployeeName"
                Dim dTbl As System.Data.DataTable = CreateDataTable(strSQL)

                Dim oEmployeeState As New Employee.roEmployeeState(-1)
                roBusinessState.CopyTo(Me.oState, oEmployeeState)

                'Cargar los datos de las posiciones
                If dTbl IsNot Nothing Then
                    For Each dRow As DataRow In dTbl.Rows
                        Dim oBudgetEmployeeAvailableForPosition As New roBudgetEmployeeAvailableForPosition
                        oBudgetEmployeeAvailableForPosition.IDEmployee = Any2Integer(dRow("IDEmployee"))
                        oBudgetEmployeeAvailableForPosition.EmployeeName = Any2String(dRow("EmployeeName"))
                        oBudgetEmployeeAvailableForPosition.IDAssignment = Any2Integer(dRow("IDAssignment"))
                        oBudgetEmployeeAvailableForPosition.AssignmentName = Any2String(dRow("AssignmentName"))
                        oBudgetEmployeeAvailableForPosition.IDShift = Any2Integer(dRow("IDShift1"))
                        oBudgetEmployeeAvailableForPosition.ShiftName = Any2String(dRow("ShiftName"))
                        oBudgetEmployeeAvailableForPosition.IDGroup = Any2Integer(dRow("IDGroup"))
                        oBudgetEmployeeAvailableForPosition.FullGroupName = Any2String(dRow("FullGroupName"))
                        oBudgetEmployeeAvailableForPosition.Cost = roTypes.Any2Double(Any2String(dRow("Cost")).Replace(".", Me.oState.Language.GetDecimalDigitFormat))

                        oRet.Add(oBudgetEmployeeAvailableForPosition)
                    Next
                End If

                ' Añadimos la informacion de reglas de planificacion a los empleados disponibles de la lista
                Dim strEmployeesIndicments As String = "B-1"
                For Each oBudgetEmployeeAvailableForPosition As roBudgetEmployeeAvailableForPosition In oRet
                    strEmployeesIndicments += ",B" & oBudgetEmployeeAvailableForPosition.IDEmployee
                Next

                ' Cargamos el calendario de planificacion de los empleados disponibles para dicha posicion
                Dim oCalendarState = New roCalendarState(Me.oState.IDPassport)
                Dim oCalendarManager As New roCalendarManager(oCalendarState)

                Dim oCalendar As New roCalendar
                oCalendar = oCalendarManager.Load(xPlanDate, xPlanDate, strEmployeesIndicments, CalendarView.Planification, CalendarDetailLevel.Daily, False)

                ' Asignamos el horario/puesto de la posicion en la fecha indicada para cada empleado del calendario
                If oCalendar IsNot Nothing AndAlso oCalendar.CalendarData IsNot Nothing Then
                    For Each oCalendarRowEmployeeData As roCalendarRow In oCalendar.CalendarData
                        If oCalendarRowEmployeeData IsNot Nothing AndAlso oCalendarRowEmployeeData.PeriodData IsNot Nothing Then
                            For Each oCalendarRowDayData As roCalendarRowDayData In oCalendarRowEmployeeData.PeriodData.DayData
                                If oCalendarRowDayData.PlanDate = xPlanDate Then
                                    ' Asignamos el horario/puesto de la posicion
                                    Dim oNewCalendarRowDayData As New roCalendarRowDayData
                                    oNewCalendarRowDayData.MainShift = oProductiveUnitModePosition.ShiftData
                                    oNewCalendarRowDayData.AltShift1 = Nothing
                                    oNewCalendarRowDayData.AltShift2 = Nothing
                                    oNewCalendarRowDayData.AltShift3 = Nothing
                                    oNewCalendarRowDayData.IsHoliday = False
                                    oNewCalendarRowDayData.ShiftBase = Nothing
                                    oNewCalendarRowDayData.ShiftUsed = oProductiveUnitModePosition.ShiftData
                                    oNewCalendarRowDayData.PlanDate = xPlanDate
                                    oNewCalendarRowDayData.AssigData = oProductiveUnitModePosition.AssignmentData
                                    oNewCalendarRowDayData.IDDailyBudgetPosition = 0
                                    oNewCalendarRowDayData.Locked = oCalendarRowDayData.Locked
                                    oNewCalendarRowDayData.Feast = oCalendarRowDayData.Feast

                                    oCalendarManager.AddCalendarRowDayData(oCalendar, oNewCalendarRowDayData, oCalendarRowEmployeeData.EmployeeData.IDEmployee, True, False, False, False, False, False, True, True, False)
                                End If
                            Next
                        End If
                    Next
                End If

                ' Cargamos los impactos de planificación
                Dim oIndictments As New List(Of roCalendarScheduleIndictment)
                Try
                    Dim oCalRuleState As New roCalendarScheduleRulesState(oState.IDPassport)
                    Dim oCalRulesManager As New roCalendarScheduleRulesManager(oCalRuleState)
                    oIndictments = oCalRulesManager.CheckScheduleRules(oCalendar)
                Catch ex As Exception
                End Try

                ' Añadimos los indicments a los empleados disponibles
                If oIndictments IsNot Nothing AndAlso oIndictments.Count > 0 Then
                    For Each _Indictment As roCalendarScheduleIndictment In oIndictments
                        If _Indictment.Dates.Contains(xPlanDate) Then
                            For Each oBudgetEmployeeAvailableForPosition As roBudgetEmployeeAvailableForPosition In oRet
                                If oBudgetEmployeeAvailableForPosition.IDEmployee = _Indictment.IDEmployee Then
                                    Dim oEmployeeIndictments As New List(Of roCalendarScheduleIndictment)
                                    If oBudgetEmployeeAvailableForPosition.Indictments IsNot Nothing Then
                                        oEmployeeIndictments = oBudgetEmployeeAvailableForPosition.Indictments.ToList
                                    End If
                                    oEmployeeIndictments.Add(_Indictment)
                                    oBudgetEmployeeAvailableForPosition.Indictments = oEmployeeIndictments.ToArray
                                    oBudgetEmployeeAvailableForPosition.TotalIndictments = oBudgetEmployeeAvailableForPosition.Indictments.Count
                                End If
                            Next
                        End If
                    Next
                End If

                ' Ordenamos la lista de empleados disponibles por idoneidad(salen antes los que tengan menos reglas de planificacion incumplidas)
                oRet = oRet.OrderBy(Function(ind) ind.TotalIndictments).ToList()

                bolRet = True
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roProductiveUnitModePositionManager::GetEmployeesAvailableForPosition")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roProductiveUnitModePositionManager::GetEmployeesAvailableForPosition")
            End Try

            Return oRet.ToArray
        End Function

        Public Function GetEmployeeAvailableForPositionAndCurrentStatusEmployeesSummary(ByVal _IDNode As Integer, ByVal xPlanDate As Date, ByVal oProductiveUnitModePosition As roProductiveUnitModePosition) As roEmployeeAvailableForPositionAndCurrentStatusEmployeesSummary
            Dim oRet As New roEmployeeAvailableForPositionAndCurrentStatusEmployeesSummary
            Dim bolRet As Boolean = False

            Try

                Dim oBudgetState As New roBudgetState(-1)
                roBusinessState.CopyTo(Me.oState, oBudgetState)

                ' Obtenemos los empleados del nodo que pueden realizar el puesto indicado
                Dim strEmployees As String = roBudgetManager.GetEmployeeListFromNode(_IDNode, oBudgetState, xPlanDate, True)

                Dim oManager As New roProductiveUnitModePositionManager(Me.oState)
                Dim oEmployees = oManager.GetEmployeesAvailableForPosition(_IDNode, xPlanDate, oProductiveUnitModePosition, strEmployees)

                oRet.BudgetEmployeeAvailableForPositions = oEmployees

                Dim oBudgetManager As New roBudgetManager(oBudgetState)
                Dim oStatus = roBudgetManager.CurrentStatusEmployeesSummaryOnNode(_IDNode, xPlanDate, oBudgetState, strEmployees)

                oRet.CurrentStatusEmployeesSummary = oStatus

                bolRet = True
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roProductiveUnitModePositionManager::GetEmployeeAvailableForPositionAndCurrentStatusEmployeesSummary")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roProductiveUnitModePositionManager::GetEmployeeAvailableForPositionAndCurrentStatusEmployeesSummary")
            End Try

            Return oRet

        End Function

        Public Function AddEmployeesPlanOnPosition(ByVal oProductiveUnitModePosition As roProductiveUnitModePosition, ByVal xPlanDate As Date, ByVal oProductiveUnitModePositionEmployeeData As roProductiveUnitModePositionEmployeeData(), Optional bolReplaceActualBudget As Boolean = False) As Boolean
            Dim bolRet As Boolean = False
            Dim bHaveToClose As Boolean = False

            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                For Each oEmpPlan In oProductiveUnitModePositionEmployeeData
                    bolRet = Me.AddEmployeePlanOnPosition(oProductiveUnitModePosition, xPlanDate, oEmpPlan, bolReplaceActualBudget)

                    If Not bolRet Then Exit For
                Next
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roProductiveUnitModePositionManager::AddEmployeesPlanOnPosition")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roProductiveUnitModePositionManager::AddEmployeesPlanOnPosition")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet
        End Function

        Public Function AddEmployeePlanOnPosition(ByVal oProductiveUnitModePosition As roProductiveUnitModePosition, ByVal xPlanDate As Date, ByVal oProductiveUnitModePositionEmployeeData As roProductiveUnitModePositionEmployeeData, Optional bolReplaceActualBudget As Boolean = False) As Boolean
            ' Añadimos el empleado a la posicion del presupuesto y lo planificamos
            Dim bolRet As Boolean = False
            Dim bHaveToClose As Boolean = False

            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                ' Obtenemos los datos del calendario del empleado al que tenemos que asignar los datos
                Dim oCalendarState = New roCalendarState(Me.oState.IDPassport)
                Dim oCalendarManager As New roCalendarManager(oCalendarState)

                Dim oEmployeeCalendar As New roCalendar
                oEmployeeCalendar = oCalendarManager.Load(xPlanDate, xPlanDate, "B" & oProductiveUnitModePositionEmployeeData.IDEmployee.ToString, CalendarView.Planification, CalendarDetailLevel.Daily, False)

                If oEmployeeCalendar IsNot Nothing AndAlso oEmployeeCalendar.CalendarData IsNot Nothing Then
                    For Each oEmployeeCalendarRowEmployeeData As roCalendarRow In oEmployeeCalendar.CalendarData
                        If oEmployeeCalendarRowEmployeeData IsNot Nothing AndAlso oEmployeeCalendarRowEmployeeData.PeriodData IsNot Nothing Then
                            For Each oEmployeeCalendarRowDayData As roCalendarRowDayData In oEmployeeCalendarRowEmployeeData.PeriodData.DayData
                                If oEmployeeCalendarRowDayData.PlanDate = xPlanDate Then
                                    ' Asignamnos el horario y puesto de la posicion
                                    Dim oNewCalendarRowDayData As New roCalendarRowDayData
                                    oNewCalendarRowDayData.MainShift = oProductiveUnitModePositionEmployeeData.ShiftData
                                    oNewCalendarRowDayData.AltShift1 = Nothing
                                    oNewCalendarRowDayData.AltShift2 = Nothing
                                    oNewCalendarRowDayData.AltShift3 = Nothing
                                    oNewCalendarRowDayData.IsHoliday = False
                                    oNewCalendarRowDayData.ShiftBase = Nothing
                                    oNewCalendarRowDayData.ShiftUsed = oProductiveUnitModePositionEmployeeData.ShiftData
                                    oNewCalendarRowDayData.PlanDate = xPlanDate
                                    oNewCalendarRowDayData.AssigData = oProductiveUnitModePosition.AssignmentData
                                    oNewCalendarRowDayData.IDDailyBudgetPosition = oProductiveUnitModePosition.ID
                                    oNewCalendarRowDayData.Locked = oEmployeeCalendarRowDayData.Locked
                                    bolRet = oCalendarManager.AddCalendarRowDayData(oEmployeeCalendar, oNewCalendarRowDayData, oProductiveUnitModePositionEmployeeData.IDEmployee, True, False, False, False, False, False, True, False, True)
                                    If Not bolRet Then
                                        roBusinessState.CopyTo(oCalendarState, Me.oState)
                                        'Me.oState.Result = ProductiveUnitResultEnum.InValidData
                                        Exit For
                                    End If
                                End If
                            Next
                        End If
                    Next

                    ' Guardamos los datos
                    If bolRet Then
                        Dim oCalendarResult As roCalendarResult = oCalendarManager.Save(oEmployeeCalendar, True, True, bolReplaceActualBudget)

                        If oCalendarResult.Status = CalendarStatusEnum.OK Then
                            bolRet = True
                        Else
                            Me.oState.Result = ProductiveUnitResultEnum.InValidData
                            If oCalendarResult.CalendarDataResult IsNot Nothing Then
                                'Mostramos el primer error que tenga el resultado de guardar el calendario
                                Me.oState.ErrorText = oCalendarResult.CalendarDataResult(0).ErrorText
                            End If
                        End If
                    End If

                    If bolRet Then
                        ' Eliminamos todas las notificaciones de cobertura insuficiente del presupuesto relacionado
                        ' posteriormente el notificador volvera a revisar si sigue habiendo cobertura insuficiente o no
                        Dim strSQL As String = "@DELETE# FROM sysroNotificationTasks WHERE IDNotification IN (@SELECT# ID FROM Notifications WHERE idtype IN (54)) AND Key1Numeric in(@SELECT# IDDailyBudget FROM DailyBudget_Positions WHERE ID=" & oProductiveUnitModePosition.ID & ")"
                        bolRet = ExecuteSql(strSQL)
                    End If

                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roProductiveUnitModePositionManager::AddEmployeePlanOnPosition")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roProductiveUnitModePositionManager::AddEmployeePlanOnPosition")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet
        End Function

        Public Function RemoveEmployeeFromPosition(ByVal oProductiveUnitModePosition As roProductiveUnitModePosition, ByVal xPlanDate As Date, ByVal oProductiveUnitModePositionEmployeeData As roProductiveUnitModePositionEmployeeData) As Boolean
            ' Eliminamos el empleado de la posicion del presupuesto
            Dim bolRet As Boolean = False
            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                ' Obtenemos los datos del calendario del empleado al que tenemos que asignar los datos
                Dim oCalendarState = New roCalendarState(Me.oState.IDPassport)
                Dim oCalendarManager As New roCalendarManager(oCalendarState)

                Dim oEmployeeCalendar As New roCalendar
                oEmployeeCalendar = oCalendarManager.Load(xPlanDate, xPlanDate, "B" & oProductiveUnitModePositionEmployeeData.IDEmployee.ToString, CalendarView.Planification, CalendarDetailLevel.Daily, False)

                If oEmployeeCalendar IsNot Nothing AndAlso oEmployeeCalendar.CalendarData IsNot Nothing Then
                    For Each oEmployeeCalendarRowEmployeeData As roCalendarRow In oEmployeeCalendar.CalendarData
                        If oEmployeeCalendarRowEmployeeData IsNot Nothing AndAlso oEmployeeCalendarRowEmployeeData.PeriodData IsNot Nothing Then
                            For Each oEmployeeCalendarRowDayData As roCalendarRowDayData In oEmployeeCalendarRowEmployeeData.PeriodData.DayData
                                If oEmployeeCalendarRowDayData.PlanDate = xPlanDate Then
                                    ' Asignamnos el horario y puesto de la posicion
                                    Dim oNewCalendarRowDayData As New roCalendarRowDayData
                                    oNewCalendarRowDayData.MainShift = oEmployeeCalendarRowDayData.MainShift
                                    oNewCalendarRowDayData.AltShift1 = oEmployeeCalendarRowDayData.AltShift1
                                    oNewCalendarRowDayData.AltShift2 = oEmployeeCalendarRowDayData.AltShift2
                                    oNewCalendarRowDayData.AltShift3 = oEmployeeCalendarRowDayData.AltShift3
                                    oNewCalendarRowDayData.IsHoliday = oEmployeeCalendarRowDayData.IsHoliday
                                    oNewCalendarRowDayData.ShiftBase = oEmployeeCalendarRowDayData.ShiftBase
                                    oNewCalendarRowDayData.ShiftUsed = oEmployeeCalendarRowDayData.ShiftUsed
                                    oNewCalendarRowDayData.PlanDate = oEmployeeCalendarRowDayData.PlanDate
                                    oNewCalendarRowDayData.IDDailyBudgetPosition = 0
                                    bolRet = oCalendarManager.AddCalendarRowDayData(oEmployeeCalendar, oNewCalendarRowDayData, oProductiveUnitModePositionEmployeeData.IDEmployee, False, False, False, False, False, False, False, False, True)
                                    If Not bolRet Then
                                        Exit For
                                    End If
                                End If
                            Next
                        End If
                    Next

                    If bolRet Then
                        ' Guardamos los datos
                        Dim oCalendarResult As roCalendarResult = oCalendarManager.Save(oEmployeeCalendar, True, True)

                        If oCalendarResult.Status = CalendarStatusEnum.OK Then
                            bolRet = True
                        Else
                            Me.oState.Result = ProductiveUnitResultEnum.InValidData
                            If oCalendarResult.CalendarDataResult IsNot Nothing Then
                                'Mostramos el primer error que tenga el resultado de guardar el calendario
                                Me.oState.ErrorText = oCalendarResult.CalendarDataResult(0).ErrorText
                            End If
                        End If
                    End If

                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roProductiveUnitModePositionManager::RemoveEmployeeFromPosition")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roProductiveUnitModePositionManager::RemoveEmployeeFromPosition")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet
        End Function

#End Region

    End Class

End Namespace