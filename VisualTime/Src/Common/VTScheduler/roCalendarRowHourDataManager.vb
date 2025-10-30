Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.VTBase

Namespace VTCalendar

    Public Class roCalendarRowHourDataManager
        Private oState As roCalendarRowHourDataState = Nothing

        Public Sub New()
            Me.oState = New roCalendarRowHourDataState()
        End Sub

        Public Sub New(ByVal _State As roCalendarRowHourDataState)
            Me.oState = _State
        End Sub

#Region "Methods"

        Public Function Load(ByVal xDate As DateTime, ByVal intIDEmployee As Integer, ByVal oShift As Shift.roShift, ByVal StartFloating As Date, ByVal tbProgrammedCauses As DataTable, ByVal oCalendarRowShiftData As roCalendarRowShiftData, ByVal tbProgrammedAbsences As DataTable, ByVal lstProgrammedHolidays As Generic.List(Of roProgrammedHoliday), ByVal lstProgrammedOvertimes As Generic.List(Of roProgrammedOvertime), Optional ByVal _detailLevel As CalendarDetailLevel = CalendarDetailLevel.Detail_30) As roCalendarRowHourData()

            Dim oRet As New Generic.List(Of roCalendarRowHourData)
            Dim oCalendarRowHourData As roCalendarRowHourData = Nothing
            Dim intBeginDay As Integer = 1
            Dim intEndDay As Integer = 48
            Dim bolExistProgrammedAbsence As Boolean = False

            Dim bolRet As Boolean = False

            Dim intBeginPeriod As Integer = 1
            Dim intEndPeriod As Integer = 29

            Try

                Select Case _detailLevel
                    Case CalendarDetailLevel.Detail_15
                        intBeginDay = 1
                        intEndDay = 96
                        intBeginPeriod = 1
                        intEndPeriod = 14
                    Case CalendarDetailLevel.Detail_30
                        intBeginDay = 1
                        intEndDay = 48
                        intBeginPeriod = 1
                        intEndPeriod = 29
                    Case CalendarDetailLevel.Detail_60
                        intBeginDay = 1
                        intEndDay = 24
                        intBeginPeriod = 1
                        intEndPeriod = 59
                End Select

                If oShift Is Nothing Then
                    Return Nothing
                    Exit Function
                End If

                If tbProgrammedCauses IsNot Nothing Then
                    Dim oRows() As DataRow = tbProgrammedCauses.Select("Date <= '" & Format(xDate, "yyyy/MM/dd") & "' and isnull(FinishDate, date) >= '" & Format(xDate, "yyyy/MM/dd") & "'")
                    If oRows.Length = 0 Then
                        tbProgrammedCauses = Nothing
                    End If
                End If

                If tbProgrammedAbsences IsNot Nothing Then
                    Dim oRows() As DataRow = tbProgrammedAbsences.Select("(BeginDate <= '" & Format(xDate, "yyyy/MM/dd") & "' AND " &
                                                            "RealFinishDate >= '" & Format(xDate, "yyyy/MM/dd") & "')")
                    If oRows.Length > 0 Then bolExistProgrammedAbsence = True
                End If

                ' Obtenemos los fichajes del dia
                Dim oPunchesState As New Punch.roPunchState(-1)
                Dim tbPunches As DataTable = Punch.roPunch.GetPunches("IDEmployee=" & intIDEmployee & " AND ShiftDate=" & roTypes.Any2Time(xDate).SQLSmallDateTime & " AND ActualType IN(1,2) Order by DateTime asc", oPunchesState)

                ' Tramos del dia Anterior
                Dim BeginDatePeriod As New DateTime(1899, 12, 28, 23, 59, 0)
                Dim EndDatePeriod As New DateTime(1899, 12, 28, 23, 59, 0)
                For i As Integer = intBeginDay To intEndDay
                    BeginDatePeriod = DateAdd(DateInterval.Minute, intBeginPeriod, EndDatePeriod)
                    EndDatePeriod = DateAdd(DateInterval.Minute, intEndPeriod, BeginDatePeriod)
                    oCalendarRowHourData = New roCalendarRowHourData
                    oCalendarRowHourData = AssignRowHourType(BeginDatePeriod, EndDatePeriod, xDate, intIDEmployee, oShift, StartFloating, tbPunches, tbProgrammedCauses, oCalendarRowShiftData, bolExistProgrammedAbsence, lstProgrammedHolidays, lstProgrammedOvertimes)
                    oRet.Add(oCalendarRowHourData)
                Next

                ' Tramos del dia Actual
                BeginDatePeriod = New DateTime(1899, 12, 29, 23, 59, 0)
                EndDatePeriod = New DateTime(1899, 12, 29, 23, 59, 0)
                For i As Integer = intBeginDay To intEndDay
                    BeginDatePeriod = DateAdd(DateInterval.Minute, intBeginPeriod, EndDatePeriod)
                    EndDatePeriod = DateAdd(DateInterval.Minute, intEndPeriod, BeginDatePeriod)
                    oCalendarRowHourData = New roCalendarRowHourData
                    oCalendarRowHourData = AssignRowHourType(BeginDatePeriod, EndDatePeriod, xDate, intIDEmployee, oShift, StartFloating, tbPunches, tbProgrammedCauses, oCalendarRowShiftData, bolExistProgrammedAbsence, lstProgrammedHolidays, lstProgrammedOvertimes)
                    oRet.Add(oCalendarRowHourData)
                Next

                ' Tramos del dia Posterior
                BeginDatePeriod = New DateTime(1899, 12, 30, 23, 59, 0)
                EndDatePeriod = New DateTime(1899, 12, 30, 23, 59, 0)
                For i As Integer = intBeginDay To intEndDay
                    BeginDatePeriod = DateAdd(DateInterval.Minute, intBeginPeriod, EndDatePeriod)
                    EndDatePeriod = DateAdd(DateInterval.Minute, intEndPeriod, BeginDatePeriod)
                    oCalendarRowHourData = New roCalendarRowHourData
                    oCalendarRowHourData = AssignRowHourType(BeginDatePeriod, EndDatePeriod, xDate, intIDEmployee, oShift, StartFloating, tbPunches, tbProgrammedCauses, oCalendarRowShiftData, bolExistProgrammedAbsence, lstProgrammedHolidays, lstProgrammedOvertimes)
                    oRet.Add(oCalendarRowHourData)
                Next

                bolRet = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCalendarRowHourDataManager::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCalendarRowHourDataManager::Load")
            Finally

            End Try

            Return oRet.ToArray

        End Function

        Public Function LoadTheoricLayers(ByVal xDate As DateTime, ByVal oShift As Shift.roShift, ByVal oCalendarRowShiftData As roCalendarRowShiftData, ByVal StartFloating As Date, Optional ByVal _detailLevel As CalendarDetailLevel = CalendarDetailLevel.Detail_30) As roCalendarRowHourData()

            Dim oRet As New Generic.List(Of roCalendarRowHourData)
            Dim oCalendarRowHourData As roCalendarRowHourData = Nothing
            Dim intBeginDay As Integer = 1
            Dim intEndDay As Integer = 48
            Dim bolExistProgrammedAbsence As Boolean = False

            Dim bolRet As Boolean = False

            Dim intBeginPeriod As Integer = 1
            Dim intEndPeriod As Integer = 29

            Try

                Select Case _detailLevel
                    Case CalendarDetailLevel.Detail_15
                        intBeginDay = 1
                        intEndDay = 96
                        intBeginPeriod = 1
                        intEndPeriod = 14
                    Case CalendarDetailLevel.Detail_30
                        intBeginDay = 1
                        intEndDay = 48
                        intBeginPeriod = 1
                        intEndPeriod = 29
                    Case CalendarDetailLevel.Detail_60
                        intBeginDay = 1
                        intEndDay = 24
                        intBeginPeriod = 1
                        intEndPeriod = 59
                End Select

                If oShift Is Nothing Then
                    Return Nothing
                    Exit Function
                End If

                ' Tramos del dia Anterior
                Dim BeginDatePeriod As New DateTime(1899, 12, 28, 23, 59, 0)
                Dim EndDatePeriod As New DateTime(1899, 12, 28, 23, 59, 0)
                For i As Integer = intBeginDay To intEndDay
                    BeginDatePeriod = DateAdd(DateInterval.Minute, intBeginPeriod, EndDatePeriod)
                    EndDatePeriod = DateAdd(DateInterval.Minute, intEndPeriod, BeginDatePeriod)
                    oCalendarRowHourData = New roCalendarRowHourData
                    oCalendarRowHourData = AssignTheoricRowHourType(BeginDatePeriod, EndDatePeriod, xDate, oShift, oCalendarRowShiftData, StartFloating)
                    oRet.Add(oCalendarRowHourData)
                Next

                ' Tramos del dia Actual
                BeginDatePeriod = New DateTime(1899, 12, 29, 23, 59, 0)
                EndDatePeriod = New DateTime(1899, 12, 29, 23, 59, 0)
                For i As Integer = intBeginDay To intEndDay
                    BeginDatePeriod = DateAdd(DateInterval.Minute, intBeginPeriod, EndDatePeriod)
                    EndDatePeriod = DateAdd(DateInterval.Minute, intEndPeriod, BeginDatePeriod)
                    oCalendarRowHourData = New roCalendarRowHourData
                    oCalendarRowHourData = AssignTheoricRowHourType(BeginDatePeriod, EndDatePeriod, xDate, oShift, oCalendarRowShiftData, StartFloating)
                    oRet.Add(oCalendarRowHourData)
                Next

                ' Tramos del dia Posterior
                BeginDatePeriod = New DateTime(1899, 12, 30, 23, 59, 0)
                EndDatePeriod = New DateTime(1899, 12, 30, 23, 59, 0)
                For i As Integer = intBeginDay To intEndDay
                    BeginDatePeriod = DateAdd(DateInterval.Minute, intBeginPeriod, EndDatePeriod)
                    EndDatePeriod = DateAdd(DateInterval.Minute, intEndPeriod, BeginDatePeriod)
                    oCalendarRowHourData = New roCalendarRowHourData
                    oCalendarRowHourData = AssignTheoricRowHourType(BeginDatePeriod, EndDatePeriod, xDate, oShift, oCalendarRowShiftData, StartFloating)
                    oRet.Add(oCalendarRowHourData)
                Next

                bolRet = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCalendarRowHourDataManager::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCalendarRowHourDataManager::Load")
            Finally

            End Try

            Return oRet.ToArray

        End Function

        Private Function AssignRowHourType(ByVal BeginDatePeriod As DateTime, EndDatePeriod As DateTime, ByVal xDate As DateTime, ByVal intIDEmployee As Integer, ByVal _Shift As Shift.roShift, ByVal StartFloating As Date, ByVal tbPunches As DataTable, ByVal tbProgrammedCauses As DataTable, ByVal oCalendarRowShiftData As roCalendarRowShiftData, ByVal bolExistProgrammedAbsence As Boolean, ByVal lstProgrammedHolidays As Generic.List(Of roProgrammedHoliday), ByVal lstProgrammedOvertimes As Generic.List(Of roProgrammedOvertime)) As roCalendarRowHourData
            Dim oRet As New roCalendarRowHourData
            Dim bolRet As Boolean = False
            Dim oShift As Shift.roShift = _Shift

            Try

                Dim FloatingTime As Double = 0
                If oShift.ShiftType = ShiftType.NormalFloating AndAlso oShift.StartFloating.HasValue Then
                    ' En el caso de horario flotante, obtenemos la hora de inicio flotante base y la asignada
                    ' para obtener la diferencia
                    Dim StartFloatingShift As Double = roTypes.Any2Time(oShift.StartFloating).NumericValue
                    FloatingTime = roTypes.Any2Time(StartFloating).NumericValue - StartFloatingShift
                End If

                ' Revisamos las franjas OBLIGADAS/FLEXIBLES
                For Each oLayer As Shift.roShiftLayer In oShift.Layers

                    If oLayer.Data.Exists("Begin") Then
                        If roTypes.Any2Time(oLayer.Data.Item("Begin")).DateOnly = roTypes.Any2Time("0001/01/01").DateOnly Then
                            oLayer.Data.Item("Begin") = roTypes.Any2Time(roTypes.Any2Time(oLayer.Data.Item("Begin")).NumericValue + roTypes.Any2Time("1899/12/30").NumericValue).Value
                        End If
                    End If

                    If oLayer.Data.Exists("Finish") Then
                        If roTypes.Any2Time(oLayer.Data.Item("Finish")).DateOnly = roTypes.Any2Time("0001/01/01").DateOnly Then
                            oLayer.Data.Item("Finish") = roTypes.Any2Time(roTypes.Any2Time(oLayer.Data.Item("Finish")).NumericValue + roTypes.Any2Time("1899/12/30").NumericValue).Value
                        End If
                    End If

                    If oLayer.LayerType = roLayerTypes.roLTMandatory Then
                        ' OBLIGADA

                        ' Inicio de la franja
                        Dim BeginLayer As Double = roTypes.Any2Time(roTypes.Any2Time(oLayer.Data.Item("Begin")).NumericValue + FloatingTime).NumericValue

                        If oLayer.Data.Exists("FloatingBeginUpTo") Then
                            ' En caso de entrada flotante, calculamos el inicio en base a los fichajes, si hay

                            ' Inicialmente la entrada es el final del inicio flotante
                            Dim BestIn As Double = roTypes.Any2Time(oLayer.Data.Item("FloatingBeginUpTo")).NumericValue

                            ' Si tiene fichajes, verificamos si alguno de ellos puede ser el inicio de franja
                            If tbPunches IsNot Nothing AndAlso tbPunches.Rows.Count > 0 Then

                                Dim tbClonePunches As DataTable = tbPunches

                                ' El inicio es el inicio de la franja
                                Dim FromIn As Double = BeginLayer
                                Dim FromInDay As Date = roTypes.Any2Time(FromIn).DateOnly

                                ' Obtiene mejor entrada (si existe)
                                For Each orow As DataRow In tbPunches.Rows
                                    If roTypes.Any2Double(orow("ActualType")) = 1 Then
                                        ' Si el fichaje es de entrada

                                        ' Obtenemos la diferencia diaria entre el fichaje y el dia asignado
                                        Dim PunchDay As Integer = DateDiff(DateInterval.Day, xDate, roTypes.Any2Time(orow("DateTime")).DateOnly)

                                        ' Convertimos el fichaje al dia de la franja
                                        Dim PunchIn As Double = roTypes.Any2Time(DateAdd(DateInterval.Day, PunchDay, FromInDay)).NumericValue
                                        PunchIn = PunchIn + roTypes.Any2Time(roTypes.Any2Time(orow("DateTime")).TimeOnly).NumericValue

                                        ' Obtenemos la fecha/hora mayor entre el inicio de la franja y el fichaje de entrada
                                        Dim ThisIn As Double = Math.Max(roTypes.Any2Time(PunchIn).NumericValue, FromIn)

                                        ' Obtenemos el siguiente fichaje de salida
                                        Dim oRows() As DataRow = tbClonePunches.Select("DateTime > '" & Format(orow("Datetime"), "yyyy/MM/dd HH:mm") & "' and ActualType = 2")
                                        If oRows.Length > 0 Then
                                            ' Convertimos el fichaje de salida al dia de la franja
                                            ' Obtenemos la diferencia diaria entre el fichaje y el dia asignado
                                            Dim PunchOutDay As Integer = DateDiff(DateInterval.Day, xDate, roTypes.Any2Time(oRows(0)("DateTime")).DateOnly)

                                            Dim PunchOut As Double = roTypes.Any2Time(DateAdd(DateInterval.Day, PunchOutDay, FromInDay)).NumericValue
                                            PunchOut = PunchOut + roTypes.Any2Time(roTypes.Any2Time(oRows(0)("DateTime")).TimeOnly).NumericValue

                                            ' Si la salida es posterior al inicio de la franja y el fichaje es anterior a la mejor entrada
                                            If PunchOut >= FromIn And ThisIn <= BestIn Then
                                                BestIn = ThisIn
                                            End If

                                        End If
                                    End If
                                Next
                            End If

                            BeginLayer = BestIn
                        ElseIf oLayer.Data.Exists("AllowModifyIniHour") Then
                            ' En caso de entrada a una hora variable, buscamos la hora de inicio indicada en el dia
                            For Each oShiftLayerDefinition As roCalendarShiftLayersDefinition In oCalendarRowShiftData.ShiftLayersDefinition
                                If oShiftLayerDefinition.ExistLayerStartTime And oShiftLayerDefinition.LayerID = oLayer.ID Then
                                    ' Si la hora indicada es de la franja que estamos validando la asignamos como inicio de la franja
                                    BeginLayer = roTypes.Any2Time(roTypes.Any2Time(oShiftLayerDefinition.LayerStartTime).NumericValue + FloatingTime).NumericValue
                                    Exit For
                                End If
                            Next
                        End If

                        Dim _Begin As Double = Math.Max(BeginLayer, roTypes.Any2Time(BeginDatePeriod).NumericValue)

                        Dim FinishLayer As Double = roTypes.Any2Time(oLayer.Data.Item("Finish")).NumericValue + FloatingTime
                        If oLayer.Data.Exists("FloatingFinishMinutes") Then
                            FinishLayer = roTypes.Any2Time(BeginLayer).Add(oLayer.Data.Item("FloatingFinishMinutes"), "n").NumericValue
                        End If

                        If oLayer.Data.Exists("AllowModifyDuration") Then
                            ' En caso de entrada a una hora variable, buscamos la hora de inicio indicada en el dia
                            For Each oShiftLayerDefinition As roCalendarShiftLayersDefinition In oCalendarRowShiftData.ShiftLayersDefinition
                                If oShiftLayerDefinition.ExistLayerDuration And oShiftLayerDefinition.LayerID = oLayer.ID Then
                                    ' Si la duracion indicada es de la franja que estamos validando la asignamos como final de la franja
                                    FinishLayer = roTypes.Any2Time(BeginLayer).Add(oShiftLayerDefinition.LayerDuration, "n").NumericValue
                                    Exit For
                                End If
                            Next
                        End If

                        Dim _Finish As Double = Math.Min(FinishLayer, roTypes.Any2Time(EndDatePeriod).NumericValue)

                        If _Finish - _Begin > 0 Or roTypes.Any2Time(EndDatePeriod).Value = roTypes.Any2Time(BeginLayer).Value Then
                            oRet.DailyHourType = DailyHourTypeEnum.Mandatory

                            ' En el caso que el tramo este en una franja obligada y el horario permita complementarias
                            ' , revisamos si el tramo es de ordinarias o complementarias
                            If _Shift.AllowComplementary Then
                                For Each oShiftLayerDefinition As roCalendarShiftLayersDefinition In oCalendarRowShiftData.ShiftLayersDefinition
                                    If oShiftLayerDefinition.LayerID = oLayer.ID Then
                                        ' Otenemos el periodo de horas complementarias de la franja que estamos revisando
                                        ' y determinamos si el tramo es de complementarias o no
                                        Dim oBeginComplementary As DateTime = roTypes.Any2Time(roTypes.Any2Time(BeginLayer).NumericValue + roTypes.Any2Time(0).Add(oShiftLayerDefinition.LayerOrdinaryHours, "n").NumericValue).Value
                                        Dim oFinishComplementary As DateTime = roTypes.Any2Time(FinishLayer).Value
                                        _Begin = Math.Max(roTypes.Any2Time(oBeginComplementary).NumericValue, roTypes.Any2Time(BeginDatePeriod).NumericValue)
                                        _Finish = Math.Min(roTypes.Any2Time(oFinishComplementary).NumericValue, roTypes.Any2Time(EndDatePeriod).NumericValue)
                                        If _Finish - _Begin > 0 Or roTypes.Any2Time(EndDatePeriod).Value = roTypes.Any2Time(oBeginComplementary).Value Then
                                            ' Asignamos ese tramo como tipo complementario
                                            oRet.DailyHourType = DailyHourTypeEnum.Complementary
                                        End If
                                        Exit For
                                    End If
                                Next
                            End If
                            Exit For
                        End If

                    ElseIf oLayer.LayerType = roLayerTypes.roLTWorking And oRet.DailyHourType <> DailyHourTypeEnum.Mandatory Then
                        ' FLEXIBLE
                        Dim _Begin As Double = Math.Max(roTypes.Any2Time(oLayer.Data.Item("Begin")).NumericValue + FloatingTime, roTypes.Any2Time(BeginDatePeriod).NumericValue)
                        Dim _Finish As Double = Math.Min(roTypes.Any2Time(oLayer.Data.Item("Finish")).NumericValue + FloatingTime, roTypes.Any2Time(EndDatePeriod).NumericValue)

                        If _Finish - _Begin > 0 Or roTypes.Any2Time(EndDatePeriod).Value = roTypes.Any2Time(roTypes.Any2Time(oLayer.Data.Item("Begin")).NumericValue + FloatingTime).Value Then
                            oRet.DailyHourType = DailyHourTypeEnum.Flexible
                        End If
                    End If
                Next

                ' Si ese dia haya ausencia diaria, lo marcamos
                If bolExistProgrammedAbsence Then oRet.IsHoursAbsence = True

                If Not oRet.IsHoursAbsence Then
                    ' Revisamos si ese tramo esta dentro de una prevision de ausencia por horas
                    If tbProgrammedCauses IsNot Nothing AndAlso tbProgrammedCauses.Rows.Count > 0 Then
                        Dim oRows() As DataRow = tbProgrammedCauses.Select("Date <= '" & Format(xDate, "yyyy/MM/dd") & "' and isnull(FinishDate, date) >= '" & Format(xDate, "yyyy/MM/dd") & "'")
                        If oRows.Length > 0 Then
                            Dim _Begin As Double = Math.Max(roTypes.Any2Time(oRows(0)("BeginTime")).NumericValue, roTypes.Any2Time(BeginDatePeriod).NumericValue)
                            Dim _Finish As Double = Math.Min(roTypes.Any2Time(oRows(0)("EndTime")).NumericValue, roTypes.Any2Time(EndDatePeriod).NumericValue)

                            If _Finish - _Begin > 0 Or roTypes.Any2Time(EndDatePeriod).Value = roTypes.Any2Time(_Begin).Value Then
                                oRet.IsHoursAbsence = True
                            End If
                        End If
                    End If
                End If

                ' Si ese dia tiene vacaciones por horas, lo marcamos
                If lstProgrammedHolidays.Count > 0 Then
                    ' Revisamos si la prevision es del dia completo
                    For Each oProgrammedHoliday As roProgrammedHoliday In lstProgrammedHolidays
                        If oProgrammedHoliday.ProgrammedDate = xDate And oProgrammedHoliday.AllDay Then
                            oRet.IsHoursHoliday = True
                            Exit For
                        End If
                    Next

                    If Not oRet.IsHoursHoliday Then
                        ' Revisamos si la prevision es de un periodo concreto
                        For Each oProgrammedHoliday As roProgrammedHoliday In lstProgrammedHolidays
                            If oProgrammedHoliday.ProgrammedDate = xDate And Not oProgrammedHoliday.AllDay Then
                                Dim _Begin As Double = Math.Max(roTypes.Any2Time(oProgrammedHoliday.BeginTime).NumericValue, roTypes.Any2Time(BeginDatePeriod).NumericValue)
                                Dim _Finish As Double = Math.Min(roTypes.Any2Time(oProgrammedHoliday.EndTime).NumericValue, roTypes.Any2Time(EndDatePeriod).NumericValue)

                                If _Finish - _Begin > 0 Or roTypes.Any2Time(EndDatePeriod).Value = roTypes.Any2Time(_Begin).Value Then
                                    oRet.IsHoursHoliday = True
                                    Exit For
                                End If
                            End If
                        Next
                    End If
                End If

                ' Si ese dia tiene horas de exceso, lo marcamos
                If lstProgrammedOvertimes.Count > 0 Then
                    For Each oProgrammedOvertime As roProgrammedOvertime In lstProgrammedOvertimes
                        If oProgrammedOvertime.ProgrammedBeginDate <= xDate And oProgrammedOvertime.ProgrammedEndDate >= xDate Then
                            Dim _Begin As Double = Math.Max(roTypes.Any2Time(oProgrammedOvertime.BeginTime).NumericValue, roTypes.Any2Time(BeginDatePeriod).NumericValue)
                            Dim _Finish As Double = Math.Min(roTypes.Any2Time(oProgrammedOvertime.EndTime).NumericValue, roTypes.Any2Time(EndDatePeriod).NumericValue)

                            If _Finish - _Begin > 0 Or roTypes.Any2Time(EndDatePeriod).Value = roTypes.Any2Time(_Begin).Value Then
                                oRet.IsHoursOvertime = True
                                Exit For
                            End If
                        End If
                    Next
                End If

                bolRet = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCalendarRowHourDataManager::AssignRowHourType")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCalendarRowHourDataManager::AssignRowHourType")
            Finally

            End Try

            Return oRet

        End Function

        Private Function AssignTheoricRowHourType(ByVal BeginDatePeriod As DateTime, EndDatePeriod As DateTime, ByVal xDate As DateTime, ByVal _Shift As Shift.roShift, ByVal oCalendarRowShiftData As roCalendarRowShiftData, ByVal StartFloating As Date) As roCalendarRowHourData
            Dim oRet As New roCalendarRowHourData
            Dim bolRet As Boolean = False
            Dim oShift As Shift.roShift = _Shift

            Try

                Dim FloatingTime As Double = 0
                If oShift.ShiftType = ShiftType.NormalFloating AndAlso oShift.StartFloating.HasValue Then
                    ' En el caso de horario flotante, obtenemos la hora de inicio flotante base y la asignada
                    ' para obtener la diferencia
                    Dim StartFloatingShift As Double = roTypes.Any2Time(oShift.StartFloating).NumericValue
                    FloatingTime = roTypes.Any2Time(StartFloating).NumericValue - StartFloatingShift
                End If

                ' Revisamos las franjas OBLIGADAS/FLEXIBLES
                For Each oLayer As Shift.roShiftLayer In oShift.Layers

                    If oLayer.Data.Exists("Begin") Then
                        If roTypes.Any2Time(oLayer.Data.Item("Begin")).DateOnly = roTypes.Any2Time("0001/01/01").DateOnly Then
                            oLayer.Data.Item("Begin") = roTypes.Any2Time(roTypes.Any2Time(oLayer.Data.Item("Begin")).NumericValue + roTypes.Any2Time("1899/12/30").NumericValue).Value
                        End If
                    End If

                    If oLayer.Data.Exists("Finish") Then
                        If roTypes.Any2Time(oLayer.Data.Item("Finish")).DateOnly = roTypes.Any2Time("0001/01/01").DateOnly Then
                            oLayer.Data.Item("Finish") = roTypes.Any2Time(roTypes.Any2Time(oLayer.Data.Item("Finish")).NumericValue + roTypes.Any2Time("1899/12/30").NumericValue).Value
                        End If
                    End If

                    If oLayer.LayerType = roLayerTypes.roLTMandatory Then
                        ' OBLIGADA

                        ' Inicio de la franja
                        Dim BeginLayer As Double = roTypes.Any2Time(roTypes.Any2Time(oLayer.Data.Item("Begin")).NumericValue + FloatingTime).NumericValue

                        If oLayer.Data.Exists("FloatingBeginUpTo") Then
                            ' En caso de entrada flotante, calculamos el inicio en base a los fichajes, si hay

                            ' Inicialmente la entrada es el final del inicio flotante
                            Dim BestIn As Double = roTypes.Any2Time(oLayer.Data.Item("FloatingBeginUpTo")).NumericValue

                            BeginLayer = BestIn
                        ElseIf oLayer.Data.Exists("AllowModifyIniHour") Then
                            ' En caso de entrada a una hora variable, buscamos la hora de inicio indicada en el dia
                            For Each oShiftLayerDefinition As roCalendarShiftLayersDefinition In oCalendarRowShiftData.ShiftLayersDefinition
                                If oShiftLayerDefinition.ExistLayerStartTime And oShiftLayerDefinition.LayerID = oLayer.ID Then
                                    ' Si la hora indicada es de la franja que estamos validando la asignamos como inicio de la franja
                                    BeginLayer = roTypes.Any2Time(roTypes.Any2Time(oShiftLayerDefinition.LayerStartTime).NumericValue + FloatingTime).NumericValue
                                    Exit For
                                End If
                            Next
                        End If

                        Dim _Begin As Double = Math.Max(BeginLayer, roTypes.Any2Time(BeginDatePeriod).NumericValue)

                        Dim FinishLayer As Double = roTypes.Any2Time(oLayer.Data.Item("Finish")).NumericValue + FloatingTime
                        If oLayer.Data.Exists("FloatingFinishMinutes") Then
                            FinishLayer = roTypes.Any2Time(BeginLayer).Add(oLayer.Data.Item("FloatingFinishMinutes"), "n").NumericValue
                        End If

                        If oLayer.Data.Exists("AllowModifyDuration") Then
                            ' En caso de entrada a una hora variable, buscamos la hora de inicio indicada en el dia
                            For Each oShiftLayerDefinition As roCalendarShiftLayersDefinition In oCalendarRowShiftData.ShiftLayersDefinition
                                If oShiftLayerDefinition.ExistLayerDuration And oShiftLayerDefinition.LayerID = oLayer.ID Then
                                    ' Si la duracion indicada es de la franja que estamos validando la asignamos como final de la franja
                                    FinishLayer = roTypes.Any2Time(BeginLayer).Add(oShiftLayerDefinition.LayerDuration, "n").NumericValue
                                    Exit For
                                End If
                            Next
                        End If

                        Dim _Finish As Double = Math.Min(FinishLayer, roTypes.Any2Time(EndDatePeriod).NumericValue)

                        If _Finish - _Begin > 0 Or roTypes.Any2Time(EndDatePeriod).Value = roTypes.Any2Time(BeginLayer).Value Then
                            oRet.DailyHourType = DailyHourTypeEnum.Mandatory

                            ' En el caso que el tramo este en una franja obligada y el horario permita complementarias
                            ' , revisamos si el tramo es de ordinarias o complementarias
                            If _Shift.AllowComplementary Then
                                For Each oShiftLayerDefinition As roCalendarShiftLayersDefinition In oCalendarRowShiftData.ShiftLayersDefinition
                                    If oShiftLayerDefinition.LayerID = oLayer.ID Then
                                        ' Otenemos el periodo de horas complementarias de la franja que estamos revisando
                                        ' y determinamos si el tramo es de complementarias o no
                                        Dim oBeginComplementary As DateTime = roTypes.Any2Time(roTypes.Any2Time(BeginLayer).NumericValue + roTypes.Any2Time(0).Add(oShiftLayerDefinition.LayerOrdinaryHours, "n").NumericValue).Value
                                        Dim oFinishComplementary As DateTime = roTypes.Any2Time(FinishLayer).Value
                                        _Begin = Math.Max(roTypes.Any2Time(oBeginComplementary).NumericValue, roTypes.Any2Time(BeginDatePeriod).NumericValue)
                                        _Finish = Math.Min(roTypes.Any2Time(oFinishComplementary).NumericValue, roTypes.Any2Time(EndDatePeriod).NumericValue)
                                        If _Finish - _Begin > 0 Or roTypes.Any2Time(EndDatePeriod).Value = roTypes.Any2Time(oBeginComplementary).Value Then
                                            ' Asignamos ese tramo como tipo complementario
                                            oRet.DailyHourType = DailyHourTypeEnum.Complementary
                                        End If
                                        Exit For
                                    End If
                                Next
                            End If
                            Exit For
                        End If

                    ElseIf oLayer.LayerType = roLayerTypes.roLTWorking And oRet.DailyHourType <> DailyHourTypeEnum.Mandatory Then
                        ' FLEXIBLE
                        Dim _Begin As Double = Math.Max(roTypes.Any2Time(oLayer.Data.Item("Begin")).NumericValue + FloatingTime, roTypes.Any2Time(BeginDatePeriod).NumericValue)
                        Dim _Finish As Double = Math.Min(roTypes.Any2Time(oLayer.Data.Item("Finish")).NumericValue + FloatingTime, roTypes.Any2Time(EndDatePeriod).NumericValue)

                        If _Finish - _Begin > 0 Or roTypes.Any2Time(EndDatePeriod).Value = roTypes.Any2Time(roTypes.Any2Time(oLayer.Data.Item("Begin")).NumericValue + FloatingTime).Value Then
                            oRet.DailyHourType = DailyHourTypeEnum.Flexible
                        End If
                    End If
                Next

                bolRet = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCalendarRowHourDataManager::AssignTheoricRowHourType")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCalendarRowHourDataManager::AssignTheoricRowHourType")
            Finally

            End Try

            Return oRet

        End Function

        Public Function GetCalendarDayHourData(IDEmployee As Integer, IDGroup As Integer, xDate As Date, IDShift As Integer, StartFloating As Date, ByVal detailLevel As CalendarDetailLevel, oCalendarRowShiftData As roCalendarRowShiftData) As roCalendarRowHourData()
            Dim dayHourData As roCalendarRowHourData() = {}

            Try

                Dim oShift As New Shift.roShift(IDShift, New Shift.roShiftState(oState.IDPassport))

                If IDEmployee <> -1 OrElse IDGroup <> -1 Then
                    Dim oProgrammedAbsState As New Absence.roProgrammedAbsenceState(oState.IDPassport, oState.Context, oState.ClientAddress)
                    Dim tbProgrammedCauses As DataTable = Absence.roProgrammedAbsence.GetProgrammedCauses(IDEmployee, xDate, xDate, oProgrammedAbsState)

                    Dim tbProgrammedAbsences As DataTable = Absence.roProgrammedAbsence.GetProgrammedAbsences(IDEmployee, oProgrammedAbsState)

                    Dim oProgrammedHolidaysState As New VTHolidays.roProgrammedHolidayState(oState.IDPassport, oState.Context, oState.ClientAddress)
                    Dim oProgrammedHolidayManager As New VTHolidays.roProgrammedHolidayManager()
                    Dim lstProgrammedHolidays As New Generic.List(Of roProgrammedHoliday)
                    lstProgrammedHolidays = oProgrammedHolidayManager.GetProgrammedHolidays(IDEmployee, oProgrammedHolidaysState, "Date =" & roTypes.Any2Time(xDate).SQLSmallDateTime)

                    Dim oProgrammedOvertimeyManager As New VTHolidays.roProgrammedOvertimeManager()
                    Dim oProgrammedOvertimesState As New VTHolidays.roProgrammedOvertimeState(oState.IDPassport, oState.Context, oState.ClientAddress)
                    Dim lstProgrammedOvertimes As New Generic.List(Of roProgrammedOvertime)
                    lstProgrammedOvertimes = oProgrammedOvertimeyManager.GetProgrammedOvertimes(IDEmployee, oProgrammedOvertimesState)

                    Dim oCalendarRowHourDataState As New roCalendarRowHourDataState(oState.IDPassport)
                    Dim oCalendarRowHourData As New roCalendarRowHourDataManager(oCalendarRowHourDataState)
                    dayHourData = oCalendarRowHourData.Load(xDate, IDEmployee, oShift, StartFloating, tbProgrammedCauses, oCalendarRowShiftData, tbProgrammedAbsences, lstProgrammedHolidays, lstProgrammedOvertimes, detailLevel)
                Else

                    Dim oCalendarRowHourDataState As New roCalendarRowHourDataState(oState.IDPassport)
                    Dim oCalendarRowHourData As New roCalendarRowHourDataManager(oCalendarRowHourDataState)
                    dayHourData = oCalendarRowHourData.LoadTheoricLayers(xDate, oShift, oCalendarRowShiftData, StartFloating)
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCalendarRowHourDataManager::GetCalendarDayHourData")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCalendarRowHourDataManager::GetCalendarDayHourData")
            Finally

            End Try

            Return dayHourData
        End Function

#End Region

#Region "Helpers"

        Public Shared Function LoadEmtyData(Optional ByVal _detailLevel As CalendarDetailLevel = CalendarDetailLevel.Detail_30) As roCalendarRowHourData()
            Dim oRet As New Generic.List(Of roCalendarRowHourData)
            Dim oCalendarRowHourData As roCalendarRowHourData = Nothing
            Dim intBeginDay As Integer = 1
            Dim intEndDay As Integer = 48

            Dim bolRet As Boolean = False

            Dim intBeginPeriod As Integer = 1
            Dim intEndPeriod As Integer = 29

            Try

                Select Case _detailLevel
                    Case CalendarDetailLevel.Detail_15
                        intBeginDay = 1
                        intEndDay = 96
                        intBeginPeriod = 1
                        intEndPeriod = 14
                    Case CalendarDetailLevel.Detail_30
                        intBeginDay = 1
                        intEndDay = 48
                        intBeginPeriod = 1
                        intEndPeriod = 29
                    Case CalendarDetailLevel.Detail_60
                        intBeginDay = 1
                        intEndDay = 24
                        intBeginPeriod = 1
                        intEndPeriod = 59
                End Select

                ' Tramos del dia Anterior
                Dim BeginDatePeriod As New DateTime(1899, 12, 28, 23, 59, 0)
                Dim EndDatePeriod As New DateTime(1899, 12, 28, 23, 59, 0)
                For i As Integer = intBeginDay To intEndDay
                    BeginDatePeriod = DateAdd(DateInterval.Minute, intBeginPeriod, EndDatePeriod)
                    EndDatePeriod = DateAdd(DateInterval.Minute, intEndPeriod, BeginDatePeriod)
                    oCalendarRowHourData = New roCalendarRowHourData
                    oCalendarRowHourData.DailyHourType = DailyHourTypeEnum.Untyped
                    oRet.Add(oCalendarRowHourData)
                Next

                ' Tramos del dia Actual
                BeginDatePeriod = New DateTime(1899, 12, 29, 23, 59, 0)
                EndDatePeriod = New DateTime(1899, 12, 29, 23, 59, 0)
                For i As Integer = intBeginDay To intEndDay
                    BeginDatePeriod = DateAdd(DateInterval.Minute, intBeginPeriod, EndDatePeriod)
                    EndDatePeriod = DateAdd(DateInterval.Minute, intEndPeriod, BeginDatePeriod)
                    oCalendarRowHourData = New roCalendarRowHourData
                    oCalendarRowHourData.DailyHourType = DailyHourTypeEnum.Untyped
                    oRet.Add(oCalendarRowHourData)
                Next

                ' Tramos del dia Posterior
                BeginDatePeriod = New DateTime(1899, 12, 30, 23, 59, 0)
                EndDatePeriod = New DateTime(1899, 12, 30, 23, 59, 0)
                For i As Integer = intBeginDay To intEndDay
                    BeginDatePeriod = DateAdd(DateInterval.Minute, intBeginPeriod, EndDatePeriod)
                    EndDatePeriod = DateAdd(DateInterval.Minute, intEndPeriod, BeginDatePeriod)
                    oCalendarRowHourData = New roCalendarRowHourData
                    oCalendarRowHourData.DailyHourType = DailyHourTypeEnum.Untyped
                    oRet.Add(oCalendarRowHourData)
                Next

                bolRet = True
            Catch ex As Exception
            Finally
            End Try

            Return oRet.ToArray

        End Function

#End Region

    End Class

End Namespace