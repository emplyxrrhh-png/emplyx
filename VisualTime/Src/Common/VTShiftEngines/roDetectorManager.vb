Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Move
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions.VTLiveTasks
Imports Robotics.VTBase.roTypes

Namespace VTShiftEngines

    Public Class roDetectorManager
        Private oState As roEngineState = Nothing
        Public Const mPriority = 40
        Public Const mPreviousProcessPriority = 0
        Private mTask As roLiveTask
        Private mIDEmployee As Integer = 0
        Private mExecuteSingleDayCallStack As New Hashtable
        Private ListIDs As New roCollection
        Private mGUIDChanged As Boolean = False
        Public mCurrentDate As Date = Date.MinValue

        Public ReadOnly Property State As roEngineState
            Get
                Return oState
            End Get
        End Property

#Region "Constructores"

        Public Sub New(IDTask As Integer)
            oState = New roEngineState()
            mTask = New roLiveTask(IDTask, New roLiveTaskState(oState.IDPassport))
        End Sub

        Public Sub New(IDTask As Integer, ByVal _State As roEngineState)
            oState = _State
            mTask = New roLiveTask(IDTask, New roLiveTaskState(oState.IDPassport))
        End Sub

#End Region

#Region "Métodos"

        Public Function ExecuteBatch(ByRef bolGUIDChanged As Boolean, Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False
            Dim LastDate As Date = Nothing
            Dim YesterdayIsFixed As Boolean
            Dim bCreateAdditionalTask As Boolean = False

            Try

                ' Recuperamos ID de empleado de la tarea
                mIDEmployee = If(mTask.Parameters IsNot Nothing AndAlso mTask.Parameters.Exists("IDEmployee"), roTypes.Any2Integer(mTask.Parameters("IDEmployee")), 0)
                If mIDEmployee = 0 Then
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roDetectorManager::ExecuteBatch: Unable to set IDEmployee from task.")
                    Me.oState.Result = EngineResultEnum.EmployeeRequired
                    Return False
                End If

                Me.oState.Result = EngineResultEnum.NoError

                ' Selecciona los dias sin procesar hasta la fecha actual, de 30 en 30.
                Dim strSql As String = "@SELECT#  IDEmployee, Date FROM DailySchedule with (nolock) " &
                            "WHERE Status<" & mPriority.ToString &
                                " AND Date<=" & Any2Time(Now.Date).SQLSmallDateTime & " AND IDEmployee=" & mIDEmployee.ToString &
                                " AND GUID = '" & VTBase.roConstants.GetManagedThreadGUID() & "'" &
                                " ORDER BY IDEmployee, Date"

                Dim dTbl As System.Data.DataTable = CreateDataTable(strSql)
                If dTbl IsNot Nothing AndAlso dTbl.Rows.Count > 0 Then
                    Dim totalActions As Integer = dTbl.Rows.Count
                    If totalActions = 0 Then totalActions = 1
                    Dim stepProgress As Double = 100 / totalActions
                    Dim rowIndex As Integer = 0

                    For Each oRowEmp As DataRow In dTbl.Rows
                        ' Obtiene empleado y fecha
                        mCurrentDate = oRowEmp("Date")

                        ' Mira si debemos dejar que detecte hacia atras (fechas anteriores) o ya lo hemos hecho en la anterior iteración
                        YesterdayIsFixed = (DateDiff("d", LastDate, mCurrentDate) = 1)

                        ' Realiza el proceso
                        bolRet = ExecuteSingleDay(mIDEmployee, Any2Time(mCurrentDate), YesterdayIsFixed, False, bCreateAdditionalTask, (rowIndex = dTbl.Rows.Count - 1))
                        If Not bolRet Then Return False

                        ' En el caso que se haya modificado el GUID de algun registro a procesar , salimos
                        If mGUIDChanged Then
                            bolGUIDChanged = True
                            Return bolRet
                        End If

                        ' Vamos al siguiente registro
                        LastDate = mCurrentDate

                        ' Actualizo progeso (con lo que se fuerza actualización de IsAliveAt
                        mTask.Progress = mTask.Progress + stepProgress
                        mTask.Save()

                        rowIndex += 1
                    Next



                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roDetectorManager::ExecuteBatch")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDetectorManager::ExecuteBatch")
            Finally
                If bCreateAdditionalTask Then
                    ' Generamos la tarea para que se proceso el dia marcado
                    Dim oParams As New roCollection
                    oParams.Add("TaskType", TasksType.DAILYSCHEDULE.ToString)
                    oParams.Add("IDEmployee", mIDEmployee)
                    roLiveTask.CreateLiveTask(roLiveTaskTypes.RunEngineEmployee, oParams, New roLiveTaskState())
                End If
            End Try

            Return bolRet

        End Function

        Private Function ExecuteSingleDay(ByVal zEmployee As Integer, ByVal zDate As roTime, ByVal YesterdayIsFixed As Boolean, ByVal TomorrowIsFixed As Boolean, ByRef bCreateAdditionalTask As Boolean, Optional ByVal bSetEmployeeStatus As Boolean = True) As Boolean
            '
            ' Procesa un dia concreto
            '
            Dim bolret As Boolean = False
            Dim z As New roEngineData

            Try
                ' 000. Si la fecha a procesar es futura, no hace nada
                If DateDiff("d", zDate.DateOnly, Now) < 0 Then Return True

                ' 00. Prepara datos necesarios
                z.Employee = zEmployee
                z.ProcDate = zDate
                Debug.Print(Now & "     ----> DETECTOR: Processing employee '" & zEmployee & "', date " & zDate.Value)

                ' 0. Guarda call stack, comprueba que no haya bucles recursivos
                If Not ExecuteSingleDay_CheckCallStack(zEmployee & "\" & CStr(zDate.Value)) Then Return True

                ' 1. Obtiene horarios
                '    Obtiene los horarios posibles de ayer, hoy y mañana.
                '    Calcula los limites horarios de cada fecha.
                If Not Execute_GetShiftData(z) Then Return False

                ' 2. Obtiene movimientos marcados como de ayer, hoy y mañana
                If Not roBaseEngineManager.Execute_GetMovesLive(z, ListIDs, oState) Then Return False

                ' Verificamos que no se ha marcado para procesar por otro proceso
                mGUIDChanged = roBaseEngineManager.DailyScheduleGUIDChangedOrStatusOverwritted(z.Employee, z.ProcDate.ValueDateTime, mPreviousProcessPriority, oState)
                If mGUIDChanged Then Return True

                ' 3. Asigna horario detectado hoy como correcto.
                If Not SetSelectedShift(z) Then Return False

                ' 4. Recalcula limites horarios en base a los horarios detectados
                If Not EvaluateDayLimits(z) Then Return False

                ' 5. Reevalua a que fecha pertenecen los movimientos según nuevos limites.
                If Not ReevaluateMovesDates(z) Then Return False

                ' 6. Procesa la fecha anterior y/o posterior si es necesario
                If z.bMustProcessYesterday AndAlso Not YesterdayIsFixed Then
                    ' Marcamos la fecha anterior como pendiente de detectar
                    If Not ExecuteSql("@UPDATE# DailySchedule WITH (ROWLOCK) SET TimestampEngine = GETDATE(), Status=0, TaskStatus=0, GUID='' WHERE IDEmployee=" & zEmployee & " AND Date=" & zDate.Add(-1, "d").SQLSmallDateTime) Then
                        Return False
                    End If

                    bCreateAdditionalTask = True
                End If
                If z.bMustProcessTomorrow AndAlso Not TomorrowIsFixed Then
                    ' Marcamos la fecha siguiente como pendiente de detectar
                    If Not ExecuteSql("@UPDATE# DailySchedule WITH (ROWLOCK) SET TimestampEngine = GETDATE(), Status=0, TaskStatus=0, GUID='' WHERE IDEmployee=" & zEmployee & " AND Date=" & zDate.Add(1, "d").SQLSmallDateTime) Then
                        Return False
                    End If

                    bCreateAdditionalTask = True
                End If

                ' 7. Si hay ausencia utiliza el horario principal por defecto
                Dim ExistAbsence As Boolean = True
                If Array.Exists(z.Moves.Moves.ToArray(), Function(x As roMoveItem) roTypes.Any2Time(x.ShiftDate).VBNumericValue = zDate.VBNumericValue) Then ExistAbsence = False
                If ExistAbsence Then
                    If z.ShiftID(roEngineData.Today) <> 0 Then
                        ' Verificamos que no se ha marcado para procesar por otro proceso
                        mGUIDChanged = roBaseEngineManager.DailyScheduleGUIDChangedOrStatusOverwritted(z.Employee, z.ProcDate.ValueDateTime, mPreviousProcessPriority, oState)
                        If mGUIDChanged Then
                            Return bolret
                        End If
                        If Not ExecuteSql("@UPDATE# DAILYSCHEDULE WITH (ROWLOCK) SET IDShiftUsed=" & z.ShiftID(roEngineData.Today) &
                            ", TimestampEngine = GETDATE(), Status=" & mPriority & ",JobStatus=0, TaskStatus=0 WHERE IDEmployee=" & z.Employee & " AND Date=" & z.ProcDate.SQLSmallDateTime) Then
                            Return False
                        End If
                    End If

                    Dim sSQL = "@SELECT# StartLimit, EndLimit, isnull(AllowComplementary,0) as IsComplementary, isnull(AdvancedParameters,'') as AdvancedParameters FROM Shifts with (nolock)  WHERE ID=" & z.ShiftID(roEngineData.Today).ToString
                    Dim dTbl As System.Data.DataTable = CreateDataTable(sSQL)
                    If dTbl IsNot Nothing AndAlso dTbl.Rows.Count > 0 Then
                        If Not Any2String(dTbl.Rows(0)("AdvancedParameters")).Contains("Starter=[1]") Then
                            If Not ExecuteSql("@UPDATE# DAILYSCHEDULE WITH (ROWLOCK) SET StartShiftUsed=" & Any2Time(z.FloatingTime(roEngineData.Today), True).SQLDateTime &
                                                " ,StartFlexibleUsed=null" &
                                                " ,EndFlexibleUsed=null" &
                                                " ,ShiftNameUsed=null" &
                                                " ,ShiftColorUsed=null" &
                                                " WHERE IDEmployee=" & z.Employee & " AND Date=" & z.ProcDate.SQLSmallDateTime) Then
                                Return bolret
                            End If
                        Else
                            ' En el caso del horario starter
                            If Not ExecuteSql("@UPDATE# DAILYSCHEDULE WITH (ROWLOCK) SET StartShiftUsed=" & Any2Time(z.FloatingTime(roEngineData.Today), True).SQLDateTime &
                                                " ,StartFlexibleUsed=" & Any2Time(z.FlexibleStartTime(roEngineData.Today), True).SQLDateTime &
                                                " ,EndFlexibleUsed=" & Any2Time(z.FlexibleEndTime(roEngineData.Today), True).SQLDateTime &
                                                " ,ShiftNameUsed='" & Any2String(z.FlexibleName(roEngineData.Today)).Replace("'", "''") & "'" &
                                                " ,ShiftColorUsed=" & z.FlexibleColor(roEngineData.Today) &
                                                " WHERE IDEmployee=" & z.Employee & " AND Date=" & z.ProcDate.SQLSmallDateTime) Then
                                Return bolret
                            End If
                        End If
                    End If

                End If

                ' 8. Elimina este proceso del call stack
                mExecuteSingleDayCallStack.Remove(zEmployee & "\" & CStr(zDate.Value))

                ' Cuando se calcula un día individual siempre setearemos el estado del empleado. 
                ' Si venimos de una iteración de varios días, solo lo hacemos en la últia para evitar problemas de rendimiento
                If bSetEmployeeStatus Then
                    ' 9. Actualizamos el estado actual del empleado en caso necesario
                    ' Obtenemos los datos del ultimo fichaje
                    roBaseEngineManager.SetEmployeeLastPunchData(zEmployee, zDate, oState)

                    ' Obtenemos la primera hora obligada que tiene que estar presente
                    'Dim ShiftToday As New Shift.roShift(z.SelectedShiftID(roEngineData.Today), New Shift.roShiftState(-1), False)
                    Dim ShiftToday As roShiftEngine
                    ShiftToday = roBaseEngineManager.GetShiftFromCache(z.SelectedShiftID(roEngineData.Today), oState)
                    'Dim ShiftTomorrow As New Shift.roShift(z.SelectedShiftID(roEngineData.Tomorrow), New Shift.roShiftState(-1), False)
                    Dim ShiftTomorrow As roShiftEngine
                    ShiftTomorrow = roBaseEngineManager.GetShiftFromCache(z.SelectedShiftID(roEngineData.Tomorrow), oState)

                    roBaseEngineManager.SetEmployeeBeginMandatory(z, ShiftToday, ShiftTomorrow, oState)

                    ' Eliminamos posibles alertas de fichajes impares y fichajes no fiables
                    'Call ManageAlerts(zEmployee, zDate, mConnection, mConn)
                End If


                bolret = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roDetectorManager::ExecuteSingleDay")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDetectorManager::ExecuteSingleDay")
            End Try

            Return bolret

        End Function

        Private Function ExecuteSingleDay_CheckCallStack(ByVal StackKey As String) As Boolean
            '
            ' Comprueba la pila de dias procesados por el detector para
            '  mirar que no entremos en bucles recursivos, etc.
            '

            If mExecuteSingleDayCallStack.Count > 10 Then Return False

            If mExecuteSingleDayCallStack.ContainsKey(StackKey) Then Return False

            mExecuteSingleDayCallStack.Add(StackKey, "")

            Return True

        End Function

        Private Function Execute_GetShiftData(ByRef z As roEngineData) As Boolean
            '
            ' Obtiene los horarios posibles de ayer, hoy y mañana.
            ' Calcula los limites horarios de cada fecha.
            '
            Dim Index As Integer
            Dim bolret As Boolean = False

            Try

                ' Obtiene horarios y limites de ayer, hoy y mañana
                For Index = roEngineData.Yesterday To roEngineData.Tomorrow
                    ' Obtiene horarios
                    bolret = Execute_GetShiftData_GetShiftIDs(z, Index)
                    If Not bolret Then
                        Return bolret
                    End If
                Next
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roDetectorManager::Execute_GetShiftData")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDetectorManager::Execute_GetShiftData")
            End Try

            Return bolret

        End Function

        Private Function Execute_GetShiftData_GetShiftIDs(ByRef z As roEngineData, ByVal RelativeDate As Integer) As Boolean
            '
            ' Obtiene los horarios posibles de un dia
            '

            Dim bolret As Boolean = False
            Dim SQL As String

            Try

                ' Prepara SQL
                SQL = "@SELECT# * FROM DailySchedule  with (nolock) WHERE IDEmployee=" &
                        z.Employee & " AND Date=" & z.ProcDate.Add(RelativeDate - 1, "d").SQLSmallDateTime

                ' Obtiene IDs de horarios asignados a esa fecha
                Dim dTbl As System.Data.DataTable = CreateDataTable(SQL)
                If dTbl IsNot Nothing AndAlso dTbl.Rows.Count > 0 Then
                    ' Obtiene horarios
                    z.ShiftID(RelativeDate) = Any2Long(dTbl.Rows(0)("IDShift1"))

                    ' Obtenemos horas de inicio
                    z.FloatingTime(RelativeDate) = Any2Time(dTbl.Rows(0)("StartShift1"), True).NumericValue(True)

                    z.FlexibleStartTime(RelativeDate) = Any2Time(dTbl.Rows(0)("StartFlexible1"), True).NumericValue(True)
                    z.FlexibleEndTime(RelativeDate) = Any2Time(dTbl.Rows(0)("EndFlexible1"), True).NumericValue(True)
                    z.FlexibleName(RelativeDate) = Any2String(dTbl.Rows(0)("ShiftName1"))
                    z.FlexibleColor(RelativeDate) = Any2Double(dTbl.Rows(0)("ShiftColor1"))

                    ' Obtiene horario utilizado
                    z.UsedShiftID(RelativeDate) = Any2Long(dTbl.Rows(0)("IDShiftUsed"))
                    z.UsedFloatingTime(RelativeDate) = Any2Time(dTbl.Rows(0)("StartShiftUsed"), True).NumericValue(True)

                    z.UsedFlexibleStartTime(RelativeDate) = Any2Time(dTbl.Rows(0)("StartFlexibleUsed"), True).NumericValue(True)
                    z.UsedFlexibleEndTime(RelativeDate) = Any2Time(dTbl.Rows(0)("EndFlexibleUsed"), True).NumericValue(True)

                    ' Asignamos horario como seleccionado
                    z.SelectedShiftID(RelativeDate) = z.ShiftID(RelativeDate)
                    z.SelectedFloatingTime(RelativeDate) = z.FloatingTime(RelativeDate)
                End If

                bolret = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roDetectorManager::Execute_GetShiftData_GetShiftIDs")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDetectorManager::Execute_GetShiftData_GetShiftIDs")
            End Try

            Return bolret

        End Function

        Private Function SetSelectedShift(ByRef z As roEngineData) As Boolean
            '
            ' Fija el horario detectado hoy como correcto.
            '
            Dim bolret As Boolean = False

            Try

                If z.SelectedShiftID(roEngineData.Today) <> 0 Then
                    If Not ExecuteSql("@UPDATE# DAILYSCHEDULE WITH (ROWLOCK) SET IDShiftUsed=" & z.SelectedShiftID(roEngineData.Today) &
                            ", TimestampEngine = GETDATE(), Status=" & mPriority & ",JobStatus=0, TaskStatus=0 WHERE IDEmployee=" & z.Employee & " AND Date=" & z.ProcDate.SQLSmallDateTime) Then
                        Return bolret
                    End If

                    ' Añadimos la hora base en caso de ser flotante y info de horario starter en caso necesario
                    Dim zShift As roShiftEngine
                    zShift = roBaseEngineManager.GetShiftFromCache(z.SelectedShiftID(roEngineData.Today), oState)

                    If zShift IsNot Nothing Then
                        If Not zShift.AdvancedParameters.Contains("Starter=[1]") Then
                            'If Not Any2String(dTbl.Rows(0)("AdvancedParameters")).Contains("Starter=[1]") Then
                            If Not ExecuteSql("@UPDATE# DAILYSCHEDULE WITH (ROWLOCK) SET StartShiftUsed=" & Any2Time(z.SelectedFloatingTime(roEngineData.Today), True).SQLDateTime &
                                                " ,StartFlexibleUsed=null" &
                                                " ,EndFlexibleUsed=null" &
                                                " ,ShiftNameUsed=null" &
                                                " ,ShiftColorUsed=null" &
                                                " WHERE IDEmployee=" & z.Employee & " AND Date=" & z.ProcDate.SQLSmallDateTime) Then
                                Return bolret
                            End If
                        Else
                            ' En el caso del horario starter
                            If Not ExecuteSql("@UPDATE# DAILYSCHEDULE WITH (ROWLOCK) SET StartShiftUsed=" & Any2Time(z.SelectedFloatingTime(roEngineData.Today), True).SQLDateTime &
                                                " ,StartFlexibleUsed=" & Any2Time(z.SelectedFlexibleStartTime(roEngineData.Today), True).SQLDateTime &
                                                " ,EndFlexibleUsed=" & Any2Time(z.SelectedFlexibleEndTime(roEngineData.Today), True).SQLDateTime &
                                                " ,ShiftNameUsed='" & Any2String(z.SelectedFlexibleName(roEngineData.Today)).Replace("'", "''") & "'" &
                                                " ,ShiftColorUsed=" & z.SelectedFlexibleColor(roEngineData.Today) &
                                                " WHERE IDEmployee=" & z.Employee & " AND Date=" & z.ProcDate.SQLSmallDateTime) Then
                                Return bolret
                            End If
                        End If
                    End If

                    ' Si el dia esta marcado como vacaciones el horario utilizado siempre sera el idshift1
                    If Any2Boolean(ExecuteScalar("@SELECT# isnull(IsHolidays, 0) FROM DailySchedule with (nolock) WHERE IDEmployee=" & z.Employee & " And Date =" & z.ProcDate.SQLSmallDateTime)) AndAlso
                         Not ExecuteSql("@UPDATE# DAILYSCHEDULE WITH (ROWLOCK) Set IDShiftUsed=" & z.ShiftID(roEngineData.Today) &
                                " ,StartFlexibleUsed=null" &
                                " ,EndFlexibleUsed=null" &
                                " ,ShiftNameUsed=null" &
                                " ,ShiftColorUsed=null " &
                                " ,StartShiftUsed=null, TimestampEngine = GETDATE(), Status=" & mPriority & ",JobStatus=0, TaskStatus=0 WHERE IDEmployee=" & z.Employee & " And Date=" & z.ProcDate.SQLSmallDateTime) Then
                        Return bolret
                    End If
                Else
                    If Not ExecuteSql("@UPDATE#  DAILYSCHEDULE WITH (ROWLOCK) Set IDShiftUsed=Null, StartShiftUsed=null" &
                        " ,StartFlexibleUsed=null" &
                        " ,EndFlexibleUsed=null" &
                        " ,ShiftNameUsed=null" &
                        " ,ShiftColorUsed=null " &
                        " ,TimestampEngine = GETDATE(), Status=" & mPriority & ",JobStatus=0, TaskStatus=0 WHERE IDEmployee=" & z.Employee & " And Date=" & z.ProcDate.SQLSmallDateTime) Then
                        Return bolret
                    End If
                End If

                bolret = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roDetectorManagerSetSelectedShift")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDetectorManager::SetSelectedShift")
            End Try

            Return bolret

        End Function

        Private Function EvaluateDayLimits(ByRef z As roEngineData) As Boolean
            '
            ' Calcula los limites horarios en base a los horarios
            '  detectados.
            '
            Dim RelativeDate As Integer
            Dim DateTime As New roTime
            Dim zFloatingTime As Double
            Dim bolret As Boolean = False
            Dim StartHour As roTime
            Dim EndHour As roTime
            Dim oCollection As New roCollection
            Dim LayerComplementaryHours As Double
            Dim LayerOrdinaryHours As Double
            Dim intTotalLayers As Double
            Dim zBaseFloatingTime As Double

            Try

                ' Calculamos limites diarios segun los horarios seleccionados
                For RelativeDate = roEngineData.Yesterday To roEngineData.Tomorrow
                    StartHour = Any2Time(0, True)
                    EndHour = Any2Time(0, True)

                    DateTime = z.ProcDate.Add(RelativeDate - 1, "d")
                    z.Limits(RelativeDate) = New roTimePeriod
                    If z.SelectedShiftID(RelativeDate) <> 0 Then
                        ' Hora de inicio planificada
                        zFloatingTime = z.SelectedFloatingTime(RelativeDate)

                        ' Hora inicio base
                        roBaseEngineManager.GetBaseFloatingTime(z.SelectedShiftID(RelativeDate), zBaseFloatingTime, oState)

                        ' Obtenemos tiempo a desplazar
                        zFloatingTime = zFloatingTime - zBaseFloatingTime

                        Dim zShift As roShiftEngine
                        zShift = roBaseEngineManager.GetShiftFromCache(z.SelectedShiftID(RelativeDate), oState)

                        If zShift IsNot Nothing Then
                            If zShift.AllowComplementary Then
                                'If Any2Boolean(dTbl.Rows(0)("IsComplementary")) Then
                                ' Si es un horario por horas, debemos obtener la hora de inicio y fin planificada
                                'Obtenemos el total de horas complementarias de la franja correspondiente
                                Dim oXML As String = Any2String(ExecuteScalar("@SELECT# ISNULL(LayersDefinition, '') FROM DailySchedule with (nolock)  WHERE IDEMPLOYEE=" & z.Employee.ToString & " AND DATE=" & DateTime.SQLSmallDateTime))
                                If Len(oXML) > 0 Then
                                    Try
                                        oCollection.LoadXMLString(oXML)

                                        If oCollection.Exists("TotalLayers") Then
                                            intTotalLayers = Any2Double(oCollection("TotalLayers"))
                                            For iLayer = 1 To intTotalLayers
                                                LayerComplementaryHours = 0
                                                LayerOrdinaryHours = 0
                                                If oCollection.Exists("LayerFloatingBeginTime_" & iLayer) Then
                                                    If iLayer = 1 Then
                                                        ' Si es la primera franja , obtenemos la hora de inicio del horario
                                                        StartHour = Any2Time(oCollection("LayerFloatingBeginTime_" & iLayer), True)
                                                    End If

                                                    EndHour = Any2Time(oCollection("LayerFloatingBeginTime_" & iLayer), True)
                                                End If

                                                If oCollection.Exists("LayerComplementaryHours_" & iLayer) Then LayerComplementaryHours = Any2Time(Any2Double(oCollection("LayerComplementaryHours_" & iLayer))).Minutes
                                                If oCollection.Exists("LayerOrdinaryHours_" & iLayer) Then LayerOrdinaryHours = Any2Time(Any2Double(oCollection("LayerOrdinaryHours_" & iLayer))).Minutes

                                                ' La hora final es la el inicio de la ultima franja + las horas ordinarias y complementarias
                                                EndHour = Any2Time(EndHour, True).Add(LayerComplementaryHours + LayerOrdinaryHours, "n")

                                            Next iLayer
                                        End If
                                    Catch ex As Exception
                                        'No registramos la excepción ya que no implica ningún error de cálculo
                                    End Try
                                End If
                            End If

                            ' Si es un horario Starter

                            If zShift.AdvancedParameters.Contains("Starter=[1]") Then
                                'If Any2String(dTbl.Rows(0)("AdvancedParameters")).Contains("Starter=[1]") Then
                                ' Debemos ajustar los limites a los valores del DailySchedule
                                Dim StarterDs As System.Data.DataTable = CreateDataTable("@SELECT# StartFlexible1,EndFlexible1 FROM DailySchedule with (nolock)  WHERE IDEMPLOYEE=" & z.Employee & " AND DATE=" & DateTime.SQLSmallDateTime)
                                If StarterDs IsNot Nothing AndAlso StarterDs.Rows.Count > 0 AndAlso Not IsDBNull(StarterDs.Rows(0)("StartFlexible1")) AndAlso Not IsDBNull(StarterDs.Rows(0)("EndFlexible1")) Then
                                    StartHour = Any2Time(StarterDs.Rows(0)("StartFlexible1"), True)
                                    EndHour = Any2Time(StarterDs.Rows(0)("EndFlexible1"), True)
                                End If
                            End If

                            Dim mShiftLimits As New roTimePeriod
                            ' TODOCACHE

                            'mShiftLimits.Begin = Any2Time(dTbl.Rows(0)("StartLimit"), True)
                            mShiftLimits.Begin = Any2Time(zShift.StartLimit, True)
                            'mShiftLimits.Finish = Any2Time(dTbl.Rows(0)("EndLimit"), True)
                            mShiftLimits.Finish = Any2Time(zShift.EndLimit, True)
                            z.Limits(RelativeDate).Begin = mShiftLimits.Begin.Add(DateTime).Add(Any2Time(zFloatingTime, True))
                            z.Limits(RelativeDate).Finish = mShiftLimits.Finish.Add(DateTime).Add(Any2Time(zFloatingTime, True))

                            If StartHour.NumericValue(True) <> 0 OrElse EndHour.NumericValue(True) <> 0 Then
                                Try
                                    z.Limits(RelativeDate).Begin = DateTime.Add(StartHour)
                                    z.Limits(RelativeDate).Finish = DateTime.Add(EndHour)
                                Catch ex As Exception
                                    'No registramos la excepción ya que no implica ningún error de cálculo
                                End Try
                            End If
                        End If
                    Else
                        ' Si no hay horario, los limites son fijados por los otros limites existentes
                        z.Limits(RelativeDate).Begin = Any2Time("00:00", True).Add(DateTime)
                        z.Limits(RelativeDate).Finish = Any2Time("23:59", True).Add(DateTime)
                    End If
                    'Debug.Print("ShiftDate:" & DateTime.Value.ToString & ":: Begin  --> " & z.Limits(RelativeDate).Begin.Value)
                    'Debug.Print("ShiftDate:" & DateTime.Value.ToString & ":: Finish --> " & z.Limits(RelativeDate).Finish.Value)
                Next

                bolret = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roDetectorManager::EvaluateDayLimits")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDetectorManager::EvaluateDayLimits")
            End Try

            Return bolret

        End Function

        Private Function ReevaluateMovesDates(ByRef z As roEngineData)
            '
            ' Calcula de nuevo a que fecha pertenecen los movimientos segun
            '  los nuevos limites horarios detectados.
            '
            Dim Index As Integer
            Dim RelativeDate As Integer

            Dim Divergence As Double
            Dim BestDivergenceRelativeDate As Integer
            Dim BestDivergence As Double

            Dim MoveDate As New roTime
            Dim bolret As Boolean = False

            Try

                For Index = 0 To z.Moves.Moves.Count - 1
                    ' Calcula que dia tiene menor divergencia
                    BestDivergence = 9999
                    For RelativeDate = roEngineData.Yesterday To roEngineData.Tomorrow
                        With z.Limits(RelativeDate)
                            Divergence = Math.Abs(z.Moves.Moves(Index).Period.Begin.VBNumericValue - .Begin.VBNumericValue) +
                        Math.Abs(z.Moves.Moves(Index).Period.Finish.VBNumericValue - .Finish.VBNumericValue)
                            If Divergence < BestDivergence Then
                                ' Guardamos mejor fecha hasta el momento
                                BestDivergence = Divergence
                                BestDivergenceRelativeDate = RelativeDate
                            End If
                        End With
                    Next

                    ' Obtiene fecha del movimiento
                    MoveDate = z.ProcDate.Add(BestDivergenceRelativeDate - 1, "d")

                    ' Mira si la fecha del movimiento ya era correcta o no
                    If MoveDate.VBNumericValue <> z.Moves.Moves(Index).ShiftDate.VBNumericValue Then
                        ' La fecha era distinta, actualiza movimiento

                        If Not ExecuteSql("@UPDATE#  Punches SET ShiftDate=" & MoveDate.SQLSmallDateTime &
                                    " WHERE ID=" & z.Moves.Moves(Index).ID) Then
                            Return False
                        End If

                        If ListIDs.Exists("ID" & Any2String(Index)) AndAlso Not ExecuteSql("@UPDATE# Punches SET ShiftDate=" & MoveDate.SQLSmallDateTime &
                            " WHERE ID=" & ListIDs("ID" & Any2String(Index), roCollection.roSearchMode.roByKey)) Then
                            Return False
                        End If

                        ' Indica que las fechas a las que va el movimiento deben actualizarse
                        If BestDivergenceRelativeDate = roEngineData.Yesterday Then
                            z.bMustProcessYesterday = True
                        End If
                        If BestDivergenceRelativeDate = roEngineData.Tomorrow Then
                            z.bMustProcessTomorrow = True
                        End If

                        ' Indica que las fechas de las que se quita el movimiento deben actualizarse
                        If z.Moves.Moves(Index).ShiftDate.VBNumericValue < z.ProcDate.VBNumericValue Then
                            ' El movimiento se quitó de ayer
                            z.bMustProcessYesterday = True
                        ElseIf z.Moves.Moves(Index).ShiftDate.VBNumericValue > z.ProcDate.VBNumericValue Then
                            ' El movimiento se quitó de hoy
                            z.bMustProcessTomorrow = True
                        End If

                        ' Asignamos la fecha correcta al fichaje en memoria
                        z.Moves.Moves(Index).ShiftDate.Value = MoveDate.Value
                    Else
                        ' En el caso de Live revisamos si se debe reasignar la fecha del fichaje de salida
                        If ListIDs.Exists("ID" & Any2String(Index)) Then
                            ' Obtenemos fecha asignada actual
                            Dim strShiftDate As String
                            strShiftDate = Any2String(ExecuteScalar("@SELECT#  ShiftDate From Punches WHERE ID =" & ListIDs("ID" & Any2String(Index), roCollection.roSearchMode.roByKey)))
                            If IsDate(strShiftDate) AndAlso MoveDate.VBNumericValue <> Any2Time(strShiftDate).VBNumericValue Then
                                ' Si la fecha asignada a cambiado actualizamos el fichaje
                                If Not ExecuteSql("@UPDATE#  Punches SET ShiftDate=" & MoveDate.SQLSmallDateTime &
                                    " WHERE ID=" & ListIDs("ID" & Any2String(Index), roCollection.roSearchMode.roByKey)) Then
                                    Return False
                                End If

                                ' Indica que las fechas a las que va el movimiento deben actualizarse
                                If BestDivergenceRelativeDate = roEngineData.Yesterday Then
                                    z.bMustProcessYesterday = True
                                End If
                                If BestDivergenceRelativeDate = roEngineData.Tomorrow Then
                                    z.bMustProcessTomorrow = True
                                End If

                                ' Indica que las fechas de las que se quita el movimiento deben actualizarse
                                If Any2Time(strShiftDate).VBNumericValue < z.ProcDate.VBNumericValue Then
                                    ' El movimiento se quitó de ayer
                                    z.bMustProcessYesterday = True
                                ElseIf Any2Time(strShiftDate).VBNumericValue > z.ProcDate.VBNumericValue Then
                                    ' El movimiento se quitó de hoy
                                    z.bMustProcessTomorrow = True
                                End If
                            End If
                        End If
                    End If
                Next

                bolret = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roDetectorManager::ReevaluateMovesDates")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDetectorManager::ReevaluateMovesDates")
            End Try

            Return bolret

        End Function

        Private Function ExecuteSingleDay_DayWillBeDetected(ByVal zEmployee As Long, ByRef zDate As roTime) As Boolean
            '
            ' Devuelve True si este dia va a procesarse en algun momento (el Status es inferior
            '   al de nuestro proceso)
            '
            Dim myStatus As String
            Dim bolret As Boolean = False

            Try

                myStatus = Any2String(ExecuteScalar("@SELECT# ISNULL(Status, -1) FROM DailySchedule with (nolock)  WHERE IDEmployee=" & zEmployee & " AND Date=" & zDate.SQLSmallDateTime))
                If myStatus = "" Then
                    bolret = False
                Else
                    bolret = (Any2Integer(myStatus) < mPriority)
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roDetectorManager::ExecuteSingleDay_DayWillBeDetected")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDetectorManager::ExecuteSingleDay_DayWillBeDetected")
            End Try
            Return bolret

        End Function

#End Region

    End Class

End Namespace