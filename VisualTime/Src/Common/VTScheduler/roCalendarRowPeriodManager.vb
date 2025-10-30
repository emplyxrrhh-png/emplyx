Imports System.Data.Common
Imports System.Drawing
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Zone
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTHolidays
Imports Robotics.Base.VTNotifications
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Namespace VTCalendar

    Public Class roCalendarRowPeriodDataManager
        Private oState As roCalendarRowPeriodDataState = Nothing

        Public Sub New()
            Me.oState = New roCalendarRowPeriodDataState()
        End Sub

        Public Sub New(ByVal _State As roCalendarRowPeriodDataState)
            Me.oState = _State
        End Sub

#Region "Methods"

        Public Function Load(ByVal xDate As DateTime, ByVal intIDEmployee As Integer, Optional ByVal bAudit As Boolean = False) As roCalendarRow

            Dim bolRet As roCalendarRow = Nothing

            Try

                ' Auditar lectura
                If bAudit AndAlso bolRet IsNot Nothing Then
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{Name}", "", "", 1)
                    Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tCalendarRow, "", tbParameters, -1)
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCalendarRowPeriodDataManager::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCalendarRowPeriodDataManager::Load")
            Finally

            End Try

            Return bolRet

        End Function

        Public Function Save(ByVal oCalendarRowEmployeeData As roCalendarRowEmployeeData, ByVal oCalendarRowPeriodData As roCalendarRowPeriodData, ByRef oCalendarResultDays As Generic.List(Of roCalendarDataDayError), ByVal bolLicenseHRScheduling As Boolean, Optional ByVal bAudit As Boolean = False, Optional bolInitTask As Boolean = True, Optional ByVal bolValidateData As Boolean = True, Optional ByVal oParameters As roParameters = Nothing, ByVal Optional bolCheckPermission As Boolean = False, Optional ByVal oAssignedShiftNotifications As Generic.List(Of Notifications.roNotification) = Nothing, Optional bolReplaceActualBudget As Boolean = False) As Boolean

            Dim bolRet As Boolean = False
            Dim xEmptyDate As New Date(1899, 12, 30, 0, 0, 0, 0)

            Dim strMsg As String = ""
            Dim oCalendarDataDayError As New roCalendarDataDayError

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()
                If bolValidateData Then
                    If Not Me.Validate(oCalendarRowEmployeeData, oCalendarRowPeriodData, strMsg, oParameters, oCalendarResultDays, bolLicenseHRScheduling, bolCheckPermission) Then
                        oState.Result = roCalendarRowPeriodDataState.ResultEnum.InvalidData
                        oState.ErrorText = strMsg
                        Return bolRet
                        Exit Function
                    End If
                End If

                For Each oCalendarRowDayData As roCalendarRowDayData In oCalendarRowPeriodData.DayData
                    ' Solo guardamos las celdas que hayan cambiado
                    If oCalendarRowDayData.HasChanged Then

                        Dim strShiftName As String = ""
                        Dim intIDSHift As Double = 0
                        Dim ShiftComplementaryFloatingData As roCalendarRowShiftData = Nothing

                        ' Obtenemos la fecha/empleado a actualizar
                        Dim tb As New DataTable("DailySchedule")
                        Dim strSQL As String = "@SELECT# * FROM DailySchedule with (nolock) WHERE IDEmployee= " & oCalendarRowEmployeeData.IDEmployee.ToString & " AND Date=" & roTypes.Any2Time(oCalendarRowDayData.PlanDate).SQLSmallDateTime
                        Dim cmd As DbCommand = CreateCommand(strSQL)
                        Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                        Dim bolModifiedShift As Boolean = False
                        Dim bolNewRow As Boolean = False

                        da.Fill(tb)

                        Dim oRow As DataRow
                        If tb.Rows.Count = 0 Then
                            oRow = tb.NewRow
                            bolNewRow = True
                        Else
                            oRow = tb.Rows(0)
                        End If

                        Dim iOldShift = roTypes.Any2Integer(oRow("IDShift1"))
                        Dim bHasHolidaysBeforeUpdate As Boolean = roTypes.Any2Boolean(oRow("IsHolidays"))
                        Dim bIsLockedBeforeUpdate As Boolean = roTypes.Any2Boolean(oRow("LockedDay"))

                        Dim bIsFeastBeforeUpdate As Boolean = roTypes.Any2Boolean(oRow("FeastDay"))

                        Dim lngIDDailyBudgetPositionBeforeUpdate As Long = roTypes.Any2Long(oRow("IDDailyBudgetPosition"))
                        Dim intIDAssignmentBeforeUpdate As Integer = roTypes.Any2Integer(oRow("IDAssignment"))
                        Dim intIDShiftBeforeUpdate As Integer = roTypes.Any2Integer(oRow("IDShift1"))
                        Dim oRowBeforeUpdate As DataRow = tb.NewRow
                        oRowBeforeUpdate.ItemArray = oRow.ItemArray.Clone()

                        oRow("IDEmployee") = oCalendarRowEmployeeData.IDEmployee
                        oRow("Date") = oCalendarRowDayData.PlanDate

                        If IsDBNull(oRow("IDShift1")) OrElse roTypes.Any2Double(oRow("IDShift1")) = 0 Then
                            bolModifiedShift = False
                        Else
                            ' Si previamente tiene asignado un horario,
                            ' en caso necesario hay que revisar el nuevo horario asignado
                            ' para la notificacion de horario asignado
                            bolModifiedShift = True
                        End If

                        oRow("IDShift1") = DBNull.Value
                        oRow("ExpectedWorkingHours") = DBNull.Value

                        Dim ExpectedWorkingHours As Double = -1

                        If oCalendarRowDayData.MainShift IsNot Nothing AndAlso oCalendarRowDayData.MainShift.ID > 0 Then
                            oRow("IDShift1") = oCalendarRowDayData.MainShift.ID

                            ' En caso de tener activa la notificacion de horario asignado
                            ' revisamos si coincide con el que se asigna en este momento, en el caso que sea una modificacion
                            If oAssignedShiftNotifications IsNot Nothing AndAlso oAssignedShiftNotifications.Count > 0 AndAlso bolModifiedShift AndAlso oCalendarRowDayData.PlanDate > DateTime.Now.Date Then
                                Dim oNotificationState As New Notifications.roNotificationState
                                Notifications.roNotification.ExecuteAssignedShiftNotification(oNotificationState, oAssignedShiftNotifications, oCalendarRowEmployeeData.IDEmployee, oCalendarRowDayData.PlanDate, oCalendarRowDayData.MainShift.ID, iOldShift, True)
                            End If

                            strShiftName = oCalendarRowDayData.MainShift.Name
                            intIDSHift = oCalendarRowDayData.MainShift.ID

                            ' Verificamos si tenemos que guardar datos complementarios o flotantes
                            If Not oCalendarRowDayData.IsHoliday Then
                                If oCalendarRowDayData.MainShift.ExistComplementaryData Or oCalendarRowDayData.MainShift.ExistFloatingData Then
                                    If oCalendarRowDayData.MainShift.ShiftLayers > 0 Then ShiftComplementaryFloatingData = oCalendarRowDayData.MainShift

                                    ' En el caso de franjas flotantes nos debemos guardar las horas teoricas del dia
                                    If oCalendarRowDayData.MainShift.ExistFloatingData Then ExpectedWorkingHours = roTypes.Any2Time(0).Add(oCalendarRowDayData.MainShift.PlannedHours, "n").NumericValue
                                End If
                            End If
                        End If

                        oRow("IDShift2") = DBNull.Value
                        If oCalendarRowDayData.AltShift1 IsNot Nothing AndAlso oCalendarRowDayData.AltShift1.ID > 0 Then oRow("IDShift2") = oCalendarRowDayData.AltShift1.ID

                        oRow("IDShift3") = DBNull.Value
                        If oCalendarRowDayData.AltShift2 IsNot Nothing AndAlso oCalendarRowDayData.AltShift2.ID > 0 Then oRow("IDShift3") = oCalendarRowDayData.AltShift2.ID

                        oRow("IDShift4") = DBNull.Value
                        If oCalendarRowDayData.AltShift3 IsNot Nothing AndAlso oCalendarRowDayData.AltShift3.ID > 0 Then oRow("IDShift4") = oCalendarRowDayData.AltShift3.ID

                        If oCalendarRowDayData.ShiftUsed IsNot Nothing AndAlso oCalendarRowDayData.ShiftUsed.ID > 0 Then
                            oRow("IDShiftUsed") = oCalendarRowDayData.ShiftUsed.ID
                            strShiftName = oCalendarRowDayData.ShiftUsed.Name
                            intIDSHift = oCalendarRowDayData.ShiftUsed.ID

                            ' Verificamos si tenemos que guardar datos complementarios o flotantes
                            If Not oCalendarRowDayData.IsHoliday Then
                                If oCalendarRowDayData.ShiftUsed.ExistComplementaryData Or oCalendarRowDayData.ShiftUsed.ExistFloatingData Then
                                    If oCalendarRowDayData.ShiftUsed.ShiftLayers > 0 Then ShiftComplementaryFloatingData = oCalendarRowDayData.ShiftUsed
                                    ' En el caso de franjas flotantes nos debemos guardar las horas teoricas del dia
                                    If oCalendarRowDayData.ShiftUsed.ExistFloatingData Then ExpectedWorkingHours = roTypes.Any2Time(0).Add(oCalendarRowDayData.ShiftUsed.PlannedHours, "n").NumericValue
                                End If
                            End If
                        End If

                        oRow("StartShift1") = DBNull.Value
                        If oCalendarRowDayData.MainShift IsNot Nothing AndAlso oCalendarRowDayData.MainShift.Type = ShiftTypeEnum.NormalFloating Then oRow("StartShift1") = oCalendarRowDayData.MainShift.StartHour

                        oRow("StartShift2") = DBNull.Value
                        If oCalendarRowDayData.AltShift1 IsNot Nothing AndAlso oCalendarRowDayData.AltShift1.Type = ShiftTypeEnum.NormalFloating Then oRow("StartShift2") = oCalendarRowDayData.AltShift1.StartHour

                        oRow("StartShift3") = DBNull.Value
                        If oCalendarRowDayData.AltShift2 IsNot Nothing AndAlso oCalendarRowDayData.AltShift2.Type = ShiftTypeEnum.NormalFloating Then oRow("StartShift3") = oCalendarRowDayData.AltShift2.StartHour

                        oRow("StartShift4") = DBNull.Value
                        If oCalendarRowDayData.AltShift3 IsNot Nothing AndAlso oCalendarRowDayData.AltShift3.Type = ShiftTypeEnum.NormalFloating Then oRow("StartShift4") = oCalendarRowDayData.AltShift3.StartHour

                        If oCalendarRowDayData.ShiftUsed IsNot Nothing AndAlso oCalendarRowDayData.ShiftUsed.Type = ShiftTypeEnum.NormalFloating Then oRow("StartShiftUsed") = oCalendarRowDayData.ShiftUsed.StartHour

                        ' Remarks no

                        oRow("IDShiftBase") = DBNull.Value
                        If oCalendarRowDayData.ShiftBase IsNot Nothing AndAlso oCalendarRowDayData.ShiftBase.ID > 0 Then
                            oRow("IDShiftBase") = oCalendarRowDayData.ShiftBase.ID
                            ' Verificamos si tenemos que guardar datos complementarios o flotantes
                            If oCalendarRowDayData.ShiftBase.ExistComplementaryData Or oCalendarRowDayData.ShiftBase.ExistFloatingData Then
                                If oCalendarRowDayData.ShiftBase.ShiftLayers > 0 Then ShiftComplementaryFloatingData = oCalendarRowDayData.ShiftBase
                                If oCalendarRowDayData.ShiftBase.ExistFloatingData Then ExpectedWorkingHours = roTypes.Any2Time(0).Add(oCalendarRowDayData.ShiftBase.PlannedHours, "n").NumericValue
                            End If

                        End If

                        oRow("StartShiftBase") = DBNull.Value
                        If oCalendarRowDayData.ShiftBase IsNot Nothing AndAlso oCalendarRowDayData.ShiftBase.Type = ShiftTypeEnum.NormalFloating Then oRow("StartShiftBase") = oCalendarRowDayData.ShiftBase.StartHour

                        ' Si starter y horario flexible ...
                        oRow("StartFlexible1") = DBNull.Value
                        oRow("EndFlexible1") = DBNull.Value
                        oRow("ShiftName1") = DBNull.Value
                        oRow("ShiftColor1") = DBNull.Value
                        oRow("StartFlexibleBase") = DBNull.Value
                        oRow("EndFlexibleBase") = DBNull.Value
                        oRow("ShiftNameBase") = DBNull.Value
                        oRow("ShiftColorBase") = DBNull.Value
                        oRow("StartFlexibleUsed") = DBNull.Value
                        oRow("EndFlexibleUsed") = DBNull.Value
                        oRow("ShiftNameUsed") = DBNull.Value
                        oRow("ShiftColorUsed") = DBNull.Value
                        If oCalendarRowDayData.MainShift IsNot Nothing AndAlso oCalendarRowDayData.MainShift.AdvancedParameters.ToList.FindAll(Function(x) x.Name = "Starter" AndAlso x.Value = "1").Count > 0 Then
                            If oCalendarRowDayData.MainShift.StartHour <> xEmptyDate OrElse oCalendarRowDayData.MainShift.EndHour <> xEmptyDate Then
                                oRow("StartFlexible1") = oCalendarRowDayData.MainShift.StartHour
                                oRow("EndFlexible1") = oCalendarRowDayData.MainShift.EndHour
                                ExpectedWorkingHours = roTypes.Any2Time(0).Add(oCalendarRowDayData.MainShift.PlannedHours, "n").NumericValue
                                Dim sName As String = oCalendarRowDayData.MainShift.Name
                                Dim iColor As Integer = System.Drawing.ColorTranslator.ToWin32(Drawing.ColorTranslator.FromHtml(oCalendarRowDayData.MainShift.Color.Replace("-", "")))
                                GetStarterShistNameAndColor(oCalendarRowDayData.MainShift.StartHour, oCalendarRowDayData.MainShift.EndHour, oCalendarRowDayData.MainShift.PlannedHours, sName, iColor, Me.oState)
                                oRow("ShiftName1") = sName
                                oRow("ShiftColor1") = iColor
                            End If
                        End If

                        If oCalendarRowDayData.ShiftBase IsNot Nothing AndAlso oCalendarRowDayData.ShiftBase.AdvancedParameters.ToList.FindAll(Function(x) x.Name = "Starter" AndAlso x.Value = "1").Count > 0 Then
                            oRow("StartFlexibleBase") = DBNull.Value
                            oRow("EndFlexibleBase") = DBNull.Value
                            If oCalendarRowDayData.ShiftBase.StartHour <> xEmptyDate OrElse oCalendarRowDayData.ShiftBase.EndHour <> xEmptyDate Then
                                oRow("StartFlexibleBase") = oCalendarRowDayData.ShiftBase.StartHour
                                oRow("EndFlexibleBase") = oCalendarRowDayData.ShiftBase.EndHour
                                ExpectedWorkingHours = roTypes.Any2Time(0).Add(oCalendarRowDayData.ShiftBase.PlannedHours, "n").NumericValue
                                Dim sName As String = oCalendarRowDayData.MainShift.Name
                                Dim iColor As Integer = System.Drawing.ColorTranslator.ToWin32(Drawing.ColorTranslator.FromHtml(oCalendarRowDayData.ShiftBase.Color.Replace("-", "")))
                                GetStarterShistNameAndColor(oCalendarRowDayData.ShiftBase.StartHour, oCalendarRowDayData.ShiftBase.EndHour, oCalendarRowDayData.ShiftBase.PlannedHours, sName, iColor, Me.oState)
                                oRow("ShiftNameBase") = sName
                                oRow("ShiftColorBase") = iColor
                            End If
                        End If

                        oRow("IsHolidays") = oCalendarRowDayData.IsHoliday

                        Dim dNow As DateTime = Date.Now
                        If bHasHolidaysBeforeUpdate AndAlso Not oCalendarRowDayData.IsHoliday Then
                            Dim oProgrammedManager As New roProgrammedHolidayManager
                            oProgrammedManager.RegisterDeleteProgrammedHoliday(oCalendarRowEmployeeData.IDEmployee, , oCalendarRowDayData.PlanDate, dNow)
                            oRow("TimestampHolidays") = dNow
                        ElseIf oCalendarRowDayData.IsHoliday AndAlso Not bHasHolidaysBeforeUpdate Then
                            oRow("TimestampHolidays") = dNow
                        End If

                        oRow("LockedDay") = oCalendarRowDayData.Locked

                        oRow("FeastDay") = oCalendarRowDayData.Feast

                        If oCalendarRowDayData.Remarks <> "" AndAlso oCalendarRowDayData.Remarks <> "0" Then
                            oRow("Remarks") = oCalendarRowDayData.Remarks
                        End If

                        oRow("IDDailyBudgetPosition") = DBNull.Value
                        If oCalendarRowDayData.IDDailyBudgetPosition > 0 Then oRow("IDDailyBudgetPosition") = oCalendarRowDayData.IDDailyBudgetPosition

                        If ExpectedWorkingHours > 0 Then oRow("ExpectedWorkingHours") = ExpectedWorkingHours

                        ' Incluimos los datos de complementarias y/o flotantes en caso necesario
                        oRow("LayersDefinition") = DBNull.Value
                        If ShiftComplementaryFloatingData IsNot Nothing Then
                            Dim oXml As New roCollection()
                            oXml = GetShiftLayerData(ShiftComplementaryFloatingData)
                            oRow("LayersDefinition") = oXml.XML
                        End If

                        ' Datos de HRScheduling
                        oRow("IDAssignment") = DBNull.Value
                        oRow("IsCovered") = DBNull.Value
                        oRow("OldIDAssignment") = DBNull.Value
                        oRow("IDEmployeeCovered") = DBNull.Value
                        oRow("IDAssignmentBase") = DBNull.Value
                        If bolLicenseHRScheduling AndAlso oCalendarRowDayData.AssigData IsNot Nothing Then
                            If roTypes.Any2Double(oRow("IDShiftBase")) > 0 Then
                                ' Si hay horario base, asignamos el puesto al horari base
                                oRow("IDAssignmentBase") = oCalendarRowDayData.AssigData.ID
                            Else
                                ' Si no hay horario base, asignamos el puesto al horario principal
                                oRow("IDAssignment") = oCalendarRowDayData.AssigData.ID
                            End If
                        End If

                        If bolLicenseHRScheduling Then
                            Dim strBudgetMsg As String = String.Empty
                            ' Validamos que no hayan modificado un dia que este asignado a un presupuesto
                            ' Lo unico que se puede modificar son las horas complementarias en horarios de tipos por horas
                            If Not bolNewRow AndAlso lngIDDailyBudgetPositionBeforeUpdate > 0 AndAlso oCalendarRowDayData.IDDailyBudgetPosition > 0 Then
                                If (lngIDDailyBudgetPositionBeforeUpdate = oCalendarRowDayData.IDDailyBudgetPosition) OrElse bolReplaceActualBudget Then
                                    If Not bolReplaceActualBudget Then
                                        If oCalendarRowDayData.MainShift Is Nothing OrElse intIDShiftBeforeUpdate <> oCalendarRowDayData.MainShift.ID Then
                                            strBudgetMsg = "Error"
                                        End If
                                        If oCalendarRowDayData.AssigData Is Nothing OrElse intIDAssignmentBeforeUpdate <> oCalendarRowDayData.AssigData.ID Then
                                            strBudgetMsg = "Error"
                                        End If
                                    End If

                                    If strBudgetMsg.Length = 0 Then
                                        ' En el caso de horarios por horas debemos revisar si se ha modificado la informacion extendida
                                        ' y si el total de horas ordinarias + complementarias es igual
                                        If oCalendarRowDayData.MainShift IsNot Nothing AndAlso oCalendarRowDayData.MainShift.ExistComplementaryData Then
                                            Dim oCalendarRowShiftData As New roCalendarRowShiftData

                                            oCalendarRowShiftData.ExistFloatingData = oCalendarRowDayData.MainShift.ExistFloatingData
                                            oCalendarRowShiftData.ExistComplementaryData = oCalendarRowDayData.MainShift.ExistComplementaryData
                                            oCalendarRowShiftData.ShiftLayers = oCalendarRowDayData.MainShift.ShiftLayers
                                            oCalendarRowShiftData.Type = oCalendarRowDayData.MainShift.Type
                                            oCalendarRowShiftData.StartHour = oCalendarRowDayData.MainShift.StartHour

                                            Dim oRowsBefore(0 To 0) As DataRow
                                            oRowsBefore(0) = oRowBeforeUpdate
                                            AssignFloatingComplementaryData(oCalendarRowShiftData, oRowsBefore, New roCalendarRowPeriodDataState)
                                            Dim TotalHours As Double = 0
                                            For _w As Integer = 0 To oCalendarRowShiftData.ShiftLayers - 1
                                                TotalHours += oCalendarRowShiftData.ShiftLayersDefinition(_w).LayerOrdinaryHours + oCalendarRowShiftData.ShiftLayersDefinition(_w).LayerComplementaryHours
                                            Next

                                            For _w As Integer = 0 To oCalendarRowDayData.MainShift.ShiftLayers - 1
                                                TotalHours -= oCalendarRowDayData.MainShift.ShiftLayersDefinition(_w).LayerOrdinaryHours + oCalendarRowDayData.MainShift.ShiftLayersDefinition(_w).LayerComplementaryHours
                                            Next

                                            If TotalHours <> 0 Then
                                                strBudgetMsg = "Error"
                                            End If
                                        End If
                                    End If
                                Else
                                    strBudgetMsg = "Error"
                                End If
                            End If

                            If strBudgetMsg.Length > 0 Then
                                oCalendarDataDayError = New roCalendarDataDayError
                                oCalendarDataDayError.IDEmployee = oCalendarRowEmployeeData.IDEmployee
                                oCalendarDataDayError.ErrorDate = oCalendarRowDayData.PlanDate
                                oCalendarDataDayError.ErrorCode = CalendarErrorResultDayEnum.InvalidAssignmentData
                                oCalendarDataDayError.ErrorText = Me.oState.Language.Translate("CalendarRowPeriodDataManager.Validate.invalidBudgetData", "")
                                oCalendarResultDays.Add(oCalendarDataDayError)
                                oState.Result = roCalendarRowPeriodDataState.ResultEnum.InvalidData
                                oState.ErrorText = Me.oState.Language.Translate("CalendarRowPeriodDataManager.Validate.invalidBudgetData", "")
                                Return False
                                Exit Function
                            End If
                        End If

                        ' Teletrabajo
                        ' Sólo cambio el estado de teletrabajo del día si me viene forzado desde el cliente
                        Try
                            Dim bSendTelecommuteNotificacion = False
                            If oCalendarRowDayData.TelecommuteForced Then
                                Dim bEmployeeHasTelecommuteAgreementOnDate As Boolean = False
                                Dim sEmployeeTelecommuteMandatoryDays As String = String.Empty
                                Dim sEmployeeTelecommuteOptionalDays As String = String.Empty
                                Dim iEmployeeTelecommuteMaxDays As Integer = 0
                                Dim iEmployeeTelecommuteMaxPercentage As Integer = 0
                                Dim iEmployeeTelecommutePeriodType As Integer = 0
                                Employee.roEmployee.GetEmployeeTelecommutingDataOnDate(oCalendarRowDayData.PlanDate, oCalendarRowEmployeeData.IDEmployee, New Employee.roEmployeeState(-1), bEmployeeHasTelecommuteAgreementOnDate, sEmployeeTelecommuteMandatoryDays, sEmployeeTelecommuteOptionalDays, iEmployeeTelecommuteMaxDays, iEmployeeTelecommuteMaxPercentage, iEmployeeTelecommutePeriodType)

                                If oCalendarRowDayData.MainShift IsNot Nothing Then
                                    ' En función del día de la semana
                                    ' Puede teletrabajar cualquier dia, y el valor puede cambiar
                                    If oCalendarRowDayData.TelecommutingExpected.HasValue Then
                                        ' Miro si hay que lanzar notificación
                                        Dim bTelecommuteExpected As Boolean = False
                                        If oRow("Telecommuting") Is DBNull.Value Then
                                            bTelecommuteExpected = sEmployeeTelecommuteMandatoryDays.Contains(oCalendarRowDayData.PlanDate.DayOfWeek)
                                        Else
                                            bTelecommuteExpected = roTypes.Any2Boolean(oRow("Telecommuting"))
                                        End If
                                        If bTelecommuteExpected <> oCalendarRowDayData.TelecommutingExpected.Value Then
                                            ' Tenía teletrabajo (fijado o por definición), y me lo han quitado
                                            bSendTelecommuteNotificacion = True
                                        End If
                                        oRow("Telecommuting") = oCalendarRowDayData.TelecommutingExpected.Value
                                        oRow("TelecommutingOptional") = If(oCalendarRowDayData.TelecommutingOptional.HasValue, oCalendarRowDayData.TelecommutingOptional.Value, 0)
                                    End If
                                End If
                            Else
                                If Not IsDBNull(oRow("Telecommuting")) AndAlso roTypes.Any2Boolean(oRow("Telecommuting")) <> oCalendarRowDayData.TelecommutingExpected.Value Then
                                    bSendTelecommuteNotificacion = True
                                End If

                                oRow("Telecommuting") = DBNull.Value
                                oRow("TelecommutingOptional") = DBNull.Value
                            End If

                            If bSendTelecommuteNotificacion Then
                                Dim strSQLAlert As String = String.Empty
                                strSQLAlert = " IF NOT EXISTS (@SELECT# 1 FROM sysroNotificationTasks WHERE IDNotification= 1300 AND Executed = 0 AND Key1Numeric = " & oCalendarRowEmployeeData.IDEmployee.ToString & " AND  Key3DateTime = " & roTypes.Any2Time(oCalendarRowDayData.PlanDate).SQLDateTime & " AND CONVERT(VARCHAR,Parameters) = '" & If(Not roTypes.Any2Boolean(oRow("Telecommuting")), TelecommutingTypeEnum._AtOffice.ToString, TelecommutingTypeEnum._AtHome.ToString) & "') "
                                strSQLAlert += "@INSERT# INTO sysroNotificationTasks (IDNotification, Key1Numeric, Key3DateTime, Parameters, FiredDate) VALUES (1300, " & oCalendarRowEmployeeData.IDEmployee.ToString & "," & roTypes.Any2Time(oCalendarRowDayData.PlanDate).SQLDateTime & ",'" & If(Not roTypes.Any2Boolean(oRow("Telecommuting")), TelecommutingTypeEnum._AtOffice.ToString, TelecommutingTypeEnum._AtHome.ToString) & "', " & roTypes.Any2Time(Now).SQLSmallDateTime & ")"
                                ExecuteSql(strSQLAlert)
                            End If
                        Catch ex As Exception
                            roLog.GetInstance().logMessage(roLog.EventType.roError, "roCalendarRowPeriodDataManager::Save:Error saving telecommute data: Detail: ", ex)
                        End Try

                        ' Marcamos para recalcular
                        oRow("Status") = 0
                        oRow("JobStatus") = 0
                        oRow("TaskStatus") = 0
                        oRow("GUID") = ""
                        If oCalendarRowDayData.MainShift IsNot Nothing AndAlso oCalendarRowDayData.MainShift.ID > 0 AndAlso iOldShift > 0 AndAlso iOldShift <> oCalendarRowDayData.MainShift.ID Then
                            oRow("IDPreviousShift") = iOldShift
                        ElseIf oCalendarRowDayData.MainShift Is Nothing OrElse oCalendarRowDayData.MainShift.ID = 0 Then
                            oRow("IDPreviousShift") = DBNull.Value
                        End If

                        If bolNewRow OrElse roTimeStamps.CheckIfScheduleHasChanged(oRow) Then
                            oRow("Timestamp") = Now
                        End If

                        If tb.Rows.Count = 0 Then
                            tb.Rows.Add(oRow)
                        End If
                        da.Update(tb)

                        If bAudit Then
                            ' Auditamos
                            Dim tbParameters As DataTable = oState.CreateAuditParameters()
                            oState.AddAuditParameter(tbParameters, "{IDShift}", intIDSHift, "", 1)
                            oState.AddAuditParameter(tbParameters, "{ShiftName}", strShiftName, "", 1)
                            oState.AddAuditParameter(tbParameters, "{EmployeeID}", oCalendarRowEmployeeData.IDEmployee, "", 1)
                            oState.AddAuditParameter(tbParameters, "{EmployeeName}", oCalendarRowEmployeeData.EmployeeName, "", 1)
                            oState.AddAuditParameter(tbParameters, "{ShiftDate}", oCalendarRowDayData.PlanDate, "", 1)
                            oState.AddAuditParameter(tbParameters, "{IsLockedBeforeUpdate}", bIsLockedBeforeUpdate, "", 1)
                            oState.AddAuditParameter(tbParameters, "{HasHolidaysBeforeUpdate}", bHasHolidaysBeforeUpdate, "", 1)
                            oState.AddAuditParameter(tbParameters, "{IsFeastBeforeUpdate}", bIsFeastBeforeUpdate, "", 1)
                            oState.AddAuditParameter(tbParameters, "{NowLocked}", oCalendarRowDayData.Locked, "", 1)
                            oState.AddAuditParameter(tbParameters, "{NowFeast}", oCalendarRowDayData.Feast, "", 1)
                            oState.AddAuditParameter(tbParameters, "{NowHolidays}", oCalendarRowDayData.IsHoliday, "", 1)
                            oState.Audit(Audit.Action.aUpdate, Audit.ObjectType.tCalendarRow, oCalendarRowEmployeeData.EmployeeName, tbParameters, -1)
                        End If

                        If bolLicenseHRScheduling Then
                            ' Marcamos para recálculo las dotaciones planificadas para el empleado y fecha, y notificamos al servidor en función del parámetro 'bolNotify'
                            Dim oSchedulerState As New Scheduler.roSchedulerState
                            bolRet = Scheduler.roDailyCoverage.Recalculate(Scheduler.roDailyCoverage.RecalculateTaskType.Update_Planned, oSchedulerState, oCalendarRowEmployeeData.IDEmployee, , oCalendarRowDayData.PlanDate, False)

                            If bolRet Then
                                ' Marcamos para recálculo las reales planificadas para el empleado y fecha, y no notificamos al servidor.
                                bolRet = Scheduler.roDailyCoverage.Recalculate(Scheduler.roDailyCoverage.RecalculateTaskType.Update_Actual, oSchedulerState, oCalendarRowEmployeeData.IDEmployee, , oCalendarRowDayData.PlanDate, False)
                            End If

                        End If
                    End If
                Next
                bolRet = True

                ' Notificamos el cambio en caso necesario
                If bolInitTask Then roConnector.InitTask(TasksType.DAILYSCHEDULE)
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roCalendarRowPeriodDataManager::Save")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarRowPeriodDataManager::Save")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Function Delete(ByVal oCalendarRow As roCalendarRow, Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = True

            Try

                If bolRet Then
                    Dim DeleteQuerys() As String = {""}

                    For Each strSQL As String In DeleteQuerys
                        If strSQL <> String.Empty Then bolRet = ExecuteSql(strSQL)

                        If Not bolRet Then Exit For
                    Next
                End If

                If bolRet And bAudit Then
                    ' Auditamos
                    bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tCalendarRow, "", Nothing, -1)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roCalendarRowPeriodDataManager::Delete")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarRowPeriodDataManager::Delete")
            End Try

            Return bolRet

        End Function

        Public Function Validate(ByVal oCalendarRowEmployeeData As roCalendarRowEmployeeData, ByVal oCalendarRowPeriodData As roCalendarRowPeriodData, ByRef strMsg As String, ByVal oParameters As roParameters, ByRef oCalendarResultDays As Generic.List(Of roCalendarDataDayError), ByVal bolLicenseHRScheduling As Boolean, ByVal Optional bolCheckPermission As Boolean = True) As Boolean
            Dim bolRet As Boolean = True

            Dim bolHasPermission As Boolean = False

            Dim oCalendarDataDayError As New roCalendarDataDayError

            Try

                strMsg = ""

                If oCalendarRowEmployeeData Is Nothing Then
                    oState.Result = roCalendarRowPeriodDataState.ResultEnum.InvalidData
                    strMsg &= Me.oState.Language.Translate("CalendarRowPeriodDataManager.Validate.invalidEmployeeData", "") & vbNewLine
                End If

                If oCalendarRowEmployeeData IsNot Nothing AndAlso oCalendarRowEmployeeData.IDEmployee <= 0 Then
                    oState.Result = roCalendarRowPeriodDataState.ResultEnum.InvalidData
                    strMsg &= Me.oState.Language.Translate("CalendarRowPeriodDataManager.Validate.invalidEmployeeData", "") & vbNewLine
                End If

                If strMsg.Length > 0 Then
                    bolRet = False
                    Return bolRet
                    Exit Function
                End If

                ' Obtenemos la fecha de congelacion
                'Dim oDate As Date = roBusinessSupport.GetEmployeeLockDatetoApply(oCalendarRowEmployeeData.IDEmployee, False, oState, oTrans.Connection)

                Dim oShiftState As New Shift.roShiftState(oState.IDPassport)

                ' Obtenemos los contratos del empleado
                Dim oContractState As New Contract.roContractState(oState.IDPassport, oState.Context, oState.ClientAddress)
                Dim tbContracts As DataTable = Contract.roContract.GetContractsByIDEmployee(oCalendarRowEmployeeData.IDEmployee, oContractState)

                ' Obtenemos las previsiones de vacaciones por horas
                Dim oProgrammedHolidaysState As New roProgrammedHolidayState(oState.IDPassport, oState.Context, oState.ClientAddress)
                Dim oProgrammedHolidayManager As New roProgrammedHolidayManager()
                Dim lstProgrammedHolidays As New Generic.List(Of roProgrammedHoliday)
                If Not oCalendarRowPeriodData.DayData Is Nothing AndAlso oCalendarRowPeriodData.DayData.Count > 0 Then
                    lstProgrammedHolidays = oProgrammedHolidayManager.GetProgrammedHolidays(oCalendarRowEmployeeData.IDEmployee, oProgrammedHolidaysState, "Date >=" & roTypes.Any2Time(oCalendarRowPeriodData.DayData(0).PlanDate).SQLSmallDateTime & " AND Date <=" & roTypes.Any2Time(oCalendarRowPeriodData.DayData(oCalendarRowPeriodData.DayData.Count - 1).PlanDate).SQLSmallDateTime)
                End If

                Dim oShift As Shift.roShift = Nothing

                ' Validamos los datos del periodo del empleado
                For Each oCalendarRowDayData As roCalendarRowDayData In oCalendarRowPeriodData.DayData
                    ' Verificamos los datos del día planificado
                    Dim intIDShift1 As Integer = 0
                    Dim intIDShift2 As Integer = 0
                    Dim intIDShift3 As Integer = 0
                    Dim intIDShift4 As Integer = 0
                    Dim intIDShiftBase As Integer = 0

                    ' Solo validamos las celdas que hayan cambiado
                    If oCalendarRowDayData.HasChanged Then
                        ' Validamos si el dia esta dentro de un contrato del empleado
                        Dim oRows() As DataRow = Nothing
                        Dim strError As String = ""
                        oRows = tbContracts.Select("BeginDate <= '" & Format(oCalendarRowDayData.PlanDate, "yyyy/MM/dd") & "' AND " &
                                                                           "EndDate >= '" & Format(oCalendarRowDayData.PlanDate, "yyyy/MM/dd") & "'")
                        If oRows.Length = 0 Then
                            oState.Result = roCalendarRowPeriodDataState.ResultEnum.InvalidData
                            strError = oCalendarRowEmployeeData.EmployeeName & " " & Me.oState.Language.Translate("CalendarRowPeriodDataManager.Validate.NoContract", "") & " " & oCalendarRowDayData.PlanDate
                            strMsg &= strError & vbNewLine
                            oCalendarDataDayError = New roCalendarDataDayError
                            oCalendarDataDayError.IDEmployee = oCalendarRowEmployeeData.IDEmployee
                            oCalendarDataDayError.ErrorDate = oCalendarRowDayData.PlanDate
                            oCalendarDataDayError.ErrorCode = CalendarErrorResultDayEnum.NoContract
                            oCalendarDataDayError.ErrorText = strError
                            oCalendarResultDays.Add(oCalendarDataDayError)
                        End If

                        ' Verificamos si esta dentro del periodo de congelacion
                        If oCalendarRowDayData.PlanDate <= oCalendarRowEmployeeData.FreezingDate Then
                            oState.Result = roCalendarRowPeriodDataState.ResultEnum.InvalidData
                            strError = oCalendarRowEmployeeData.EmployeeName & " " & Me.oState.Language.Translate("CalendarRowPeriodDataManager.Validate.FreezingDate", "") & " " & oCalendarRowDayData.PlanDate
                            strMsg &= strError & vbNewLine
                            oCalendarDataDayError = New roCalendarDataDayError
                            oCalendarDataDayError.IDEmployee = oCalendarRowEmployeeData.IDEmployee
                            oCalendarDataDayError.ErrorDate = oCalendarRowDayData.PlanDate
                            oCalendarDataDayError.ErrorCode = CalendarErrorResultDayEnum.FreezingDate
                            oCalendarDataDayError.ErrorText = strError
                            oCalendarResultDays.Add(oCalendarDataDayError)
                        End If

                        If bolCheckPermission Then
                            ' Verificamos los permisos del pasaporte actual sobre la planificación
                            If Not WLHelper.HasFeaturePermissionByEmployeeOnDate(oState.IDPassport, "Calendar.Scheduler", Permission.Write, oCalendarRowEmployeeData.IDEmployee, oCalendarRowDayData.PlanDate) Then
                                oState.Result = roCalendarRowPeriodDataState.ResultEnum.InvalidData
                                strError = oCalendarRowEmployeeData.EmployeeName & " " & Me.oState.Language.Translate("CalendarRowPeriodDataManager.Validate.PermissionDenied", "") & " " & oCalendarRowDayData.PlanDate
                                strMsg &= strError & vbNewLine
                                oCalendarDataDayError = New roCalendarDataDayError
                                oCalendarDataDayError.IDEmployee = oCalendarRowEmployeeData.IDEmployee
                                oCalendarDataDayError.ErrorDate = oCalendarRowDayData.PlanDate
                                oCalendarDataDayError.ErrorCode = CalendarErrorResultDayEnum.PermissionDenied
                                oCalendarDataDayError.ErrorText = strError

                                oCalendarResultDays.Add(oCalendarDataDayError)
                            End If
                        End If

                        ' Verificamos los permisos sobre los horarios
                        If oCalendarRowDayData.MainShift IsNot Nothing Then intIDShift1 = oCalendarRowDayData.MainShift.ID
                        If oCalendarRowDayData.AltShift1 IsNot Nothing Then intIDShift2 = oCalendarRowDayData.AltShift1.ID
                        If oCalendarRowDayData.AltShift2 IsNot Nothing Then intIDShift3 = oCalendarRowDayData.AltShift2.ID
                        If oCalendarRowDayData.AltShift3 IsNot Nothing Then intIDShift4 = oCalendarRowDayData.AltShift3.ID
                        If oCalendarRowDayData.ShiftBase IsNot Nothing Then intIDShiftBase = oCalendarRowDayData.ShiftBase.ID
                        If Not Shift.roShift.ShiftIsAllowed(oShiftState, intIDShift1, intIDShift2, intIDShift3, intIDShift4) Then
                            oState.Result = roCalendarRowPeriodDataState.ResultEnum.InvalidData
                            strError = oCalendarRowEmployeeData.EmployeeName & " " & Me.oState.Language.Translate("CalendarRowPeriodDataManager.Validate.ShiftWithoutPermission", "") & " " & oCalendarRowDayData.PlanDate
                            strMsg &= strError & vbNewLine
                            oCalendarDataDayError = New roCalendarDataDayError
                            oCalendarDataDayError.IDEmployee = oCalendarRowEmployeeData.IDEmployee
                            oCalendarDataDayError.ErrorDate = oCalendarRowDayData.PlanDate
                            oCalendarDataDayError.ErrorCode = CalendarErrorResultDayEnum.ShiftWithoutPermission
                            oCalendarDataDayError.ErrorText = strError

                            oCalendarResultDays.Add(oCalendarDataDayError)
                        End If

                        For i As Integer = 1 To 5
                            ' Verificamos que los horarios sean correctos
                            Dim bolCheckShift As Boolean = False
                            Dim bolShiftBase As Boolean = False
                            Dim xStartHour As Date
                            Dim oCalendarRowShiftData As roCalendarRowShiftData = Nothing

                            oShift = New Shift.roShift()

                            Select Case i
                                Case 1
                                    bolCheckShift = True
                                    If oCalendarRowDayData.MainShift IsNot Nothing Then
                                        oCalendarRowShiftData = oCalendarRowDayData.MainShift
                                        oShift.LoadGeneral(oCalendarRowDayData.MainShift.ID)
                                        xStartHour = oCalendarRowDayData.MainShift.StartHour
                                    Else
                                        oShift = Nothing
                                    End If
                                Case 2
                                    If oCalendarRowDayData.AltShift1 IsNot Nothing Then
                                        oCalendarRowShiftData = oCalendarRowDayData.AltShift1
                                        oShift.LoadGeneral(oCalendarRowDayData.AltShift1.ID)
                                        xStartHour = oCalendarRowDayData.AltShift1.StartHour
                                        bolCheckShift = True
                                    End If
                                Case 3
                                    If oCalendarRowDayData.AltShift2 IsNot Nothing Then
                                        oCalendarRowShiftData = oCalendarRowDayData.AltShift2
                                        oShift.LoadGeneral(oCalendarRowDayData.AltShift2.ID)
                                        xStartHour = oCalendarRowDayData.AltShift2.StartHour
                                        bolCheckShift = True
                                    End If
                                Case 4
                                    If oCalendarRowDayData.AltShift3 IsNot Nothing Then
                                        oCalendarRowShiftData = oCalendarRowDayData.AltShift3
                                        oShift.LoadGeneral(oCalendarRowDayData.AltShift3.ID)
                                        xStartHour = oCalendarRowDayData.AltShift3.StartHour
                                        bolCheckShift = True
                                    End If
                                Case 5
                                    If oCalendarRowDayData.ShiftBase IsNot Nothing Then
                                        oCalendarRowShiftData = oCalendarRowDayData.ShiftBase
                                        oShift.LoadGeneral(oCalendarRowDayData.ShiftBase.ID)
                                        xStartHour = oCalendarRowDayData.ShiftBase.StartHour
                                        bolCheckShift = True
                                        bolShiftBase = True
                                    End If
                            End Select

                            If bolCheckShift Then
                                If oShift Is Nothing OrElse oShift.ID <= 0 Then
                                    'oState.Result = roCalendarRowPeriodDataState.ResultEnum.InvalidData
                                    'strError = oCalendarRowEmployeeData.EmployeeName & " " & Me.oState.Language.Translate("CalendarRowPeriodDataManager.Validate.ShiftNotExist", "") & " " & oCalendarRowDayData.PlanDate
                                    'strMsg &= strError & vbNewLine
                                    'oCalendarDataDayError = New roCalendarDataDayError
                                    'oCalendarDataDayError.IDEmployee = oCalendarRowEmployeeData.IDEmployee
                                    'oCalendarDataDayError.ErrorDate = oCalendarRowDayData.PlanDate
                                    'oCalendarDataDayError.ErrorCode = CalendarErrorResultDayEnum.ShiftNotExist
                                    'oCalendarDataDayError.ErrorText = strError

                                    'oCalendarResultDays.Add(oCalendarDataDayError)
                                Else
                                    ValidateDayShift(oShift, xStartHour, oShiftState, oCalendarRowEmployeeData, oCalendarRowDayData, strMsg, bolShiftBase, oCalendarRowShiftData, oCalendarResultDays, lstProgrammedHolidays)
                                End If
                            End If
                        Next

                        ' Validamos HRScheduling
                        If Not oCalendarRowDayData.AssigData Is Nothing AndAlso oCalendarRowDayData.AssigData.ID > 0 Then
                            Dim bolExistShiftAssgnment As Boolean = False

                            ' Si no tiene licencia , error
                            If bolLicenseHRScheduling Then
                                ' Revisamos si el empleado tiene asignado el Puesto, y si ese puesto se puede asignar al horario
                                If Not oCalendarRowEmployeeData.Assignments Is Nothing Then
                                    For Each oAssignments As roCalendarAssignmentData In oCalendarRowEmployeeData.Assignments
                                        If oAssignments.ID = oCalendarRowDayData.AssigData.ID Then
                                            bolExistShiftAssgnment = True
                                            Exit For
                                        End If

                                    Next
                                End If

                                If bolExistShiftAssgnment Then
                                    ' Revisamos si el horario tiene asignado el puesto
                                    bolExistShiftAssgnment = False
                                    Dim _IDShiftAssignment As Integer = 0
                                    _IDShiftAssignment = intIDShift1
                                    If intIDShiftBase > 0 Then _IDShiftAssignment = intIDShiftBase
                                    If _IDShiftAssignment > 0 Then
                                        bolExistShiftAssgnment = Shift.roShiftAssignment.ExistShiftAssignment(_IDShiftAssignment, oCalendarRowDayData.AssigData.ID, oShiftState)
                                    End If
                                End If
                            End If

                            If Not bolExistShiftAssgnment Then
                                oState.Result = roCalendarRowPeriodDataState.ResultEnum.InvalidData
                                strError = oCalendarRowEmployeeData.EmployeeName & " " & Me.oState.Language.Translate("CalendarRowPeriodDataManager.Validate.InvalidAssignmentData", "") & " " & oCalendarRowDayData.PlanDate & "-" & oShift.Name
                                strMsg &= strError & vbNewLine
                                oCalendarDataDayError = New roCalendarDataDayError
                                oCalendarDataDayError.IDEmployee = oCalendarRowEmployeeData.IDEmployee
                                oCalendarDataDayError.ErrorDate = oCalendarRowDayData.PlanDate
                                oCalendarDataDayError.ErrorCode = CalendarErrorResultDayEnum.InvalidAssignmentData
                                oCalendarDataDayError.ErrorText = strError
                                oCalendarResultDays.Add(oCalendarDataDayError)
                            End If
                        End If
                    End If
                Next

                If strMsg.Length > 0 Then bolRet = False
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCalendarRowPeriodDataManager::Validate")
                bolRet = False
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCalendarRowPeriodDataManager::Validate")
                bolRet = False
            Finally

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
                oState.UpdateStateInfo(ex, "roCalendarRowPeriodDataManager::GetShiftLayerData")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCalendarRowPeriodDataManager::GetShiftLayerData")
            Finally

            End Try

            Return oXml
        End Function

        Public Function ValidateComplementaryData(ByVal oCalendarRowShiftData As roCalendarRowShiftData, ByVal oShift As Shift.roShift)
            Dim bolRet As Boolean = False
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
                        If roTypes.Any2Time(dblTotalTimeLayer).Value = roTypes.Any2Time(dblTotalComplementary).Value Then bolRet = True

                        If Not bolRet Then Exit For
                    End If
                Next
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCalendarRowPeriodDataManager::ValidateComplementaryData")
                bolRet = False
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCalendarRowPeriodDataManager::ValidateComplementaryData")
                bolRet = False
            Finally

            End Try

            Return bolRet
        End Function

        Public Function ValidateFloatingData(ByVal oCalendarRowShiftData As roCalendarRowShiftData, ByVal oShift As Shift.roShift)
            Dim bolRet As Boolean = False
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
                oState.UpdateStateInfo(ex, "roCalendarRowPeriodDataManager::ValidateFloatingData")
                bolRet = False
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCalendarRowPeriodDataManager::ValidateFloatingData")
                bolRet = False
            Finally

            End Try

            Return bolRet

        End Function

        Public Function ValidateDayShift(ByVal oShift As Shift.roShift, ByVal xStartHour As Date, ByVal oShiftState As Shift.roShiftState, ByVal oCalendarRowEmployeeData As roCalendarRowEmployeeData, ByVal oCalendarRowDayData As roCalendarRowDayData, ByRef strMsg As String, ByVal bolShiftBase As Boolean, ByVal oCalendarRowShiftData As roCalendarRowShiftData, ByRef oCalendarResultDays As Generic.List(Of roCalendarDataDayError), ByRef lstProgrammedHolidays As Generic.List(Of roProgrammedHoliday)) As Boolean
            Dim bolRet As Boolean = True
            Dim oCalendarDataDayError As New roCalendarDataDayError
            Dim strError As String = ""

            Try

                If oShift Is Nothing Then
                    bolRet = False
                    Return bolRet
                    Exit Function
                End If

                ' Si el horario es flotante, comprobamos que nos hayan indicado la hora de inicio
                If oShift.ShiftType = ShiftType.NormalFloating Then
                    ' la fecha solo puede ser una de estas tres
                    If Not (roTypes.Any2Time(xStartHour).DateOnly = New Date(1899, 12, 30, 0, 0, 0) Or roTypes.Any2Time(xStartHour).DateOnly = New Date(1899, 12, 29, 0, 0, 0) Or roTypes.Any2Time(xStartHour).DateOnly = New Date(1899, 12, 31, 0, 0, 0)) Then
                        oState.Result = roCalendarRowPeriodDataState.ResultEnum.InvalidData
                        strError = oCalendarRowEmployeeData.EmployeeName & " " & Me.oState.Language.Translate("CalendarRowPeriodDataManager.Validate.InvalidStartFloating", "") & " " & oCalendarRowDayData.PlanDate & "-" & oShift.Name
                        strMsg &= strError & vbNewLine
                        oCalendarDataDayError = New roCalendarDataDayError
                        oCalendarDataDayError.IDEmployee = oCalendarRowEmployeeData.IDEmployee
                        oCalendarDataDayError.ErrorDate = oCalendarRowDayData.PlanDate
                        oCalendarDataDayError.ErrorCode = CalendarErrorResultDayEnum.InvalidStartFloating
                        oCalendarDataDayError.ErrorText = strError
                        oCalendarResultDays.Add(oCalendarDataDayError)

                        bolRet = False
                    End If
                End If

                ' Comprobamos si en los datos del dia hay complementrarias y/o ordinarias
                Dim bolExistComplementaryData As Boolean = False
                Dim bolExistFloatingData As Boolean = False
                If oCalendarRowShiftData IsNot Nothing AndAlso oCalendarRowShiftData.ShiftLayersDefinition IsNot Nothing Then
                    For Each ShiftLayerDefinition As roCalendarShiftLayersDefinition In oCalendarRowShiftData.ShiftLayersDefinition
                        If ShiftLayerDefinition.LayerComplementaryHours <> -1 Or ShiftLayerDefinition.LayerOrdinaryHours <> -1 Then
                            bolExistComplementaryData = True
                        End If
                        If ShiftLayerDefinition.LayerStartTime <> New Date(1900, 1, 1) Or ShiftLayerDefinition.LayerDuration <> -1 Then
                            bolExistFloatingData = True
                        End If
                    Next
                End If

                ' Datos de franjas flotantes
                If oShift.AllowFloatingData Then
                    ' Si el horario tiene franjas flotantes, comprobamos que hayan datos validos
                    bolRet = False
                    If bolExistFloatingData Then
                        ' Comprobamos que los datos indicados en el dia tengan coherencia con los de la definicion del horario
                        If ValidateFloatingData(oCalendarRowShiftData, oShift) Then bolRet = True
                    End If
                Else
                    ' Si el horario no tiene datos flotantes, no deben existir datos de ese tipo
                    bolRet = True
                    If bolExistFloatingData Then
                        bolRet = False
                    End If
                End If
                If Not bolRet Then
                    oState.Result = roCalendarRowPeriodDataState.ResultEnum.InvalidData
                    strError = oCalendarRowEmployeeData.EmployeeName & " " & Me.oState.Language.Translate("CalendarRowPeriodDataManager.Validate.InvalidFloatingData", "") & " " & oCalendarRowDayData.PlanDate & "-" & oShift.Name
                    strMsg &= strError & vbNewLine
                    oCalendarDataDayError = New roCalendarDataDayError
                    oCalendarDataDayError.IDEmployee = oCalendarRowEmployeeData.IDEmployee
                    oCalendarDataDayError.ErrorDate = oCalendarRowDayData.PlanDate
                    oCalendarDataDayError.ErrorCode = CalendarErrorResultDayEnum.InvalidFloatingData
                    oCalendarDataDayError.ErrorText = strError
                    oCalendarResultDays.Add(oCalendarDataDayError)
                End If

                ' Datos de complementarias
                If oShift.AllowComplementary Then
                    ' Si el horario tiene complementarias, comprobamos que hayan datos validos
                    bolRet = False
                    If bolExistComplementaryData Then
                        ' Comprobamos que los datos indicados en el dia tengan coherencia con los de la definicion del horario
                        If ValidateComplementaryData(oCalendarRowShiftData, oShift) Then bolRet = True
                    End If
                Else
                    ' Si el horario no tiene complementarias, no deben existir datos de ese tipo
                    bolRet = True
                    If bolExistComplementaryData Then
                        bolRet = False
                    End If
                End If
                If Not bolRet Then
                    oState.Result = roCalendarRowPeriodDataState.ResultEnum.InvalidData
                    strError = oCalendarRowEmployeeData.EmployeeName & " " & Me.oState.Language.Translate("CalendarRowPeriodDataManager.Validate.InvalidComplementaryData", "") & " " & oCalendarRowDayData.PlanDate & "-" & oShift.Name
                    strMsg &= strError & vbNewLine
                    oCalendarDataDayError = New roCalendarDataDayError
                    oCalendarDataDayError.IDEmployee = oCalendarRowEmployeeData.IDEmployee
                    oCalendarDataDayError.ErrorDate = oCalendarRowDayData.PlanDate
                    oCalendarDataDayError.ErrorCode = CalendarErrorResultDayEnum.InvalidComplementaryData
                    oCalendarDataDayError.ErrorText = strError
                    oCalendarResultDays.Add(oCalendarDataDayError)
                End If

                Dim oShiftBase As Shift.roShift = Nothing
                ' Si el horario es de Vacaciones
                If oShift.ShiftType = ShiftType.Vacations And Not bolShiftBase Then
                    ' Comprobar que exista un horario base  y que esta marcada la propiedad IsHolidays
                    If oCalendarRowDayData.ShiftBase IsNot Nothing AndAlso oCalendarRowDayData.ShiftBase.ID <> 0 Then
                        oShiftBase = New Shift.roShift()
                        oShiftBase.LoadGeneral(oCalendarRowDayData.ShiftBase.ID)
                    End If

                    If oShiftBase Is Nothing Or Not oCalendarRowDayData.IsHoliday Then
                        oState.Result = roCalendarRowPeriodDataState.ResultEnum.InvalidData
                        strError = oCalendarRowEmployeeData.EmployeeName & " " & Me.oState.Language.Translate("CalendarRowPeriodDataManager.Validate.InvalidShiftBase", "") & " " & oCalendarRowDayData.PlanDate
                        strMsg &= strError & vbNewLine
                        oCalendarDataDayError = New roCalendarDataDayError
                        oCalendarDataDayError.IDEmployee = oCalendarRowEmployeeData.IDEmployee
                        oCalendarDataDayError.ErrorDate = oCalendarRowDayData.PlanDate
                        oCalendarDataDayError.ErrorCode = CalendarErrorResultDayEnum.InvalidShiftBase
                        oCalendarDataDayError.ErrorText = strError

                        oCalendarResultDays.Add(oCalendarDataDayError)

                        bolRet = False
                    Else
                        ' Verificamos si el horario de vacaciones se aplica a dia laborable o natural
                        If oShift.AreWorkingDays And oShiftBase.ExpectedWorkingHours = 0 Then
                            oState.Result = roCalendarRowPeriodDataState.ResultEnum.InvalidData
                            strError = oCalendarRowEmployeeData.EmployeeName & " " & Me.oState.Language.Translate("CalendarRowPeriodDataManager.Validate.InvalidAreWokringDay", "") & " " & oCalendarRowDayData.PlanDate
                            strMsg &= strError & vbNewLine
                            oCalendarDataDayError = New roCalendarDataDayError
                            oCalendarDataDayError.IDEmployee = oCalendarRowEmployeeData.IDEmployee
                            oCalendarDataDayError.ErrorDate = oCalendarRowDayData.PlanDate
                            oCalendarDataDayError.ErrorCode = CalendarErrorResultDayEnum.InvalidAreWorkingDay
                            oCalendarDataDayError.ErrorText = strError
                            oCalendarResultDays.Add(oCalendarDataDayError)

                            bolRet = False
                        End If

                        ' Verificamos si el dia tiene planificado vacaciones por horas,
                        ' en ese caso no debe permitir asignar el horario
                        For Each oProgrammedHoliday As roProgrammedHoliday In lstProgrammedHolidays
                            If oProgrammedHoliday.ProgrammedDate = oCalendarRowDayData.PlanDate Then
                                oState.Result = roCalendarRowPeriodDataState.ResultEnum.InvalidData
                                strError = oCalendarRowEmployeeData.EmployeeName & " " & Me.oState.Language.Translate("CalendarRowPeriodDataManager.Validate.ExistProgrammedHoliday", "") & " " & oCalendarRowDayData.PlanDate
                                strMsg &= strError & vbNewLine
                                oCalendarDataDayError = New roCalendarDataDayError
                                oCalendarDataDayError.IDEmployee = oCalendarRowEmployeeData.IDEmployee
                                oCalendarDataDayError.ErrorDate = oCalendarRowDayData.PlanDate
                                oCalendarDataDayError.ErrorCode = CalendarErrorResultDayEnum.ExistProgrammedHoliday
                                oCalendarDataDayError.ErrorText = strError
                                oCalendarResultDays.Add(oCalendarDataDayError)

                                bolRet = False
                            End If
                        Next

                    End If
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCalendarRowPeriodDataManager::ValidateDayShift")
                bolRet = False
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCalendarRowPeriodDataManager::ValidateDayShift")
                bolRet = False
            Finally

            End Try

            Return bolRet
        End Function

#End Region

#Region "Helpers"

        Public Shared Function HexConverter(c As System.Drawing.Color) As String
            Return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2")
        End Function

        Public Shared Function LoadCellsByCalendar(ByVal _FirstDay As DateTime, ByVal _LastDay As DateTime, ByVal _IDEmployee As Integer, ByVal _IDGroup As Integer, ByVal oPermission As Integer,
                                                   ByVal oParameters As roParameters, ByVal _typeView As CalendarView, ByVal _detailLevel As CalendarDetailLevel, ByVal oConceptNormalWork As Concept.roConcept, ByVal oConceptAbsence As Concept.roConcept,
                                                   ByVal oConceptOverWorking As Concept.roConcept, ByRef _State As roCalendarRowPeriodDataState, ByVal bolLicenseHRScheduling As Boolean,
                                                   Optional ByRef oShiftCache As Hashtable = Nothing, Optional ByVal _BeginGroupDate As DateTime = Nothing, Optional ByVal _EndGroupDate As DateTime = Nothing, Optional ByVal _IsEmployeePortal As Boolean = False,
                                                   Optional ByVal tbCauses As DataTable = Nothing, Optional ByRef tbProgrammedAbsences As DataTable = Nothing, Optional ByVal oEmployeeState As Employee.roEmployeeState = Nothing, Optional ByVal oContractState As Contract.roContractState = Nothing, Optional ByVal oProgrammedAbsState As Absence.roProgrammedAbsenceState = Nothing, Optional ByVal oProgrammedOvertimesState As roProgrammedOvertimeState = Nothing,
                                                   Optional ByVal oProgrammedHolidaysState As roProgrammedHolidayState = Nothing, Optional ByVal oProgrammedOvertimeyManager As roProgrammedOvertimeManager = Nothing, Optional ByVal oProgrammedHolidayManager As roProgrammedHolidayManager = Nothing,
                                                   Optional oSchedulerRemarks As Scheduler.roSchedulerRemarks = Nothing, Optional bLoadSeatingCapacity As Boolean = False, Optional bLoadAlerts As Boolean = True, Optional ByVal sVTEdition As String = "") As roCalendarRowPeriodData

            ' Llenamos las celdas de del empleado/grupo del periodo indicado
            Dim oRet As New roCalendarRowPeriodData
            Dim bolRet As Boolean = False
            Dim oCalendarCell As roCalendarRowDayData
            Dim oCalendarPeriodCells As New Generic.List(Of roCalendarRowDayData)
            Dim oRows As DataRow() = Nothing
            Dim oRowsInfo As DataRow() = Nothing
            Dim strSQL As String = String.Empty

            Try
                If oEmployeeState Is Nothing Then oEmployeeState = New Employee.roEmployeeState(_State.IDPassport, _State.Context, _State.ClientAddress)

                ' 01. Obtenemos la movilidades
                Dim tbEmployeeMobilities As DataTable = Employee.roMobility.GetMobilities(_IDEmployee, oEmployeeState)

                ' 02. Obtenemos los contratos
                If oContractState Is Nothing Then oContractState = New Contract.roContractState(_State.IDPassport, _State.Context, _State.ClientAddress)
                Dim tbContracts As DataTable = Contract.roContract.GetContractsByIDEmployee(_IDEmployee, oContractState)

                Dim tbDetail As DataTable = Nothing
                Dim dtEmpRequests As DataTable = Nothing
                Dim tbProgrammedCauses As DataTable = Nothing
                Dim lstProgrammedHolidays As New Generic.List(Of roProgrammedHoliday)
                Dim lstProgrammedOvertimes As New Generic.List(Of roProgrammedOvertime)

                If bLoadAlerts Then
                    ' 03. Obtenemos las previsiones
                    If oProgrammedAbsState Is Nothing Then oProgrammedAbsState = New Absence.roProgrammedAbsenceState(_State.IDPassport, _State.Context, _State.ClientAddress)

                    tbProgrammedAbsences = Absence.roProgrammedAbsence.GetProgrammedAbsences(_IDEmployee, "", oProgrammedAbsState)
                    tbProgrammedCauses = Absence.roProgrammedAbsence.GetProgrammedCauses(_IDEmployee, _FirstDay, _LastDay, oProgrammedAbsState)
                    If oProgrammedHolidaysState Is Nothing Then oProgrammedHolidaysState = New roProgrammedHolidayState(_State.IDPassport, _State.Context, _State.ClientAddress)
                    If oProgrammedHolidayManager Is Nothing Then oProgrammedHolidayManager = New roProgrammedHolidayManager()
                    lstProgrammedHolidays = oProgrammedHolidayManager.GetProgrammedHolidays(_IDEmployee, oProgrammedHolidaysState, "Date >=" & roTypes.Any2Time(_FirstDay).SQLSmallDateTime & " AND Date <=" & roTypes.Any2Time(_LastDay).SQLSmallDateTime)

                    If oProgrammedOvertimeyManager Is Nothing Then oProgrammedOvertimeyManager = New roProgrammedOvertimeManager()
                    If oProgrammedOvertimesState Is Nothing Then oProgrammedOvertimesState = New roProgrammedOvertimeState(_State.IDPassport, _State.Context, _State.ClientAddress)
                    lstProgrammedOvertimes = oProgrammedOvertimeyManager.GetProgrammedOvertimes(_IDEmployee, oProgrammedOvertimesState)

                    If tbCauses Is Nothing Then tbCauses = CreateDataTable("@SELECT# ID, Name, ShortName FROM CAUSES")
                End If

                If _IsEmployeePortal Then dtEmpRequests = roCalendarRowPeriodDataManager.GetCalendarRequestsByEmployee(_IDEmployee, _FirstDay, _LastDay, _State)

                ' 04. Obtenemos la planificacion de todo el periodo
                tbDetail = VTBusiness.Scheduler.roScheduler.GetPlan(_IDEmployee, _FirstDay, _LastDay, oEmployeeState, _IDGroup, True)

                ' 05. Información para cálculos de aforo
                ' TODO: (la cargo globalmente en el cliente de esta función
                Dim tbSeatingCapacityDetail As DataTable = Nothing
                If bLoadSeatingCapacity AndAlso Zone.roZone.CapacityControlEnabled(New roZoneState(-1)) Then
                    strSQL = "@SELECT# *, CASE WHEN ISNULL(ZoneOnDate,'') <> '' THEN ZoneOnDate ELSE ISNULL(ExpectedZone,'?') END Zone FROM [dbo].[EmployeeZonesBetweenDates] (" & roTypes.Any2Time(_FirstDay).SQLSmallDateTime & "," & roTypes.Any2Time(_LastDay).SQLSmallDateTime & ",'" & _IDEmployee.ToString & "')"
                    tbSeatingCapacityDetail = CreateDataTable(strSQL)
                End If

                ' 06. Cargamos los valores por defecto del teletrabajo en caso que no tenga planificación para ese dia
                Dim dtTelecommuteAgreements As DataTable
                strSQL = "@SELECT# * from sysrovwTelecommutingAgreement where IDEmployee = " & _IDEmployee
                dtTelecommuteAgreements = CreateDataTable(strSQL)

                ' 07. .Para cada día del periodo , obtenemos los datos necesarios
                Dim dAct As Date = _FirstDay
                Dim dEnd As Date = _LastDay
                Dim auxColor As System.Drawing.Color = Color.White

                Dim oSchduleState As Scheduler.roSchedulerState = Nothing
                Dim tbDailyCauses1 As DataTable = Nothing
                Dim tbDailyCauses2 As DataTable = Nothing
                Dim tbDailyCauses3 As DataTable = Nothing

                If _typeView = CalendarView.Review AndAlso Not _IsEmployeePortal Then

                    oSchduleState = New VTBusiness.Scheduler.roSchedulerState()
                    roBusinessState.CopyTo(_State, oSchduleState)

                    If oSchedulerRemarks Is Nothing Then
                        oSchedulerRemarks = New Scheduler.roSchedulerRemarks(oSchduleState.IDPassport, oSchduleState)
                    End If

                    Dim curIndex As Integer = 0
                    Dim strCompare As String = ""
                    For Each oRemark As Scheduler.roCalendarRemark In oSchedulerRemarks.Remarks
                        If curIndex < 3 And oRemark.IDCause >= 0 Then
                            Select Case oRemark.Compare
                                Case Scheduler.RemarkCompare.Equal
                                    strCompare = "="
                                Case Scheduler.RemarkCompare.Minor
                                    strCompare = "<"
                                Case Scheduler.RemarkCompare.MinorEqual
                                    strCompare = "<="
                                Case Scheduler.RemarkCompare.Major
                                    strCompare = ">"
                                Case Scheduler.RemarkCompare.MajorEqual
                                    strCompare = ">="
                                Case Scheduler.RemarkCompare.Distinct
                                    strCompare = "<>"
                                Case Else
                                    strCompare = "="
                            End Select

                            Dim oTotalResult As Decimal = Math.Round(CDate(oRemark.Value).TimeOfDay.TotalHours, 2)
                            strSQL = "@SELECT# Date " &
                                         "FROM DailyCauses WITH (NOLOCK)" &
                                         "WHERE IDEmployee = " & _IDEmployee.ToString & " AND IDCause = " & oRemark.IDCause & " AND Date between '" & _FirstDay.ToString("yyyyMMdd") & "' AND " & "'" & _LastDay.ToString("yyyyMMdd") & "' " &
                                         "GROUP BY Date " &
                                         "HAVING CONVERT(numeric(9,2), SUM(Value)) " & strCompare & " CONVERT(numeric(9,2)," & CStr(oTotalResult).Replace(",", ".") & ")"

                            Select Case curIndex
                                Case 0 : tbDailyCauses1 = CreateDataTable(strSQL)
                                Case 1 : tbDailyCauses2 = CreateDataTable(strSQL)
                                Case 2 : tbDailyCauses3 = CreateDataTable(strSQL)
                            End Select
                        End If
                        curIndex = curIndex + 1
                    Next

                End If

                While (dAct <= dEnd)
                    oCalendarCell = New roCalendarRowDayData
                    oCalendarCell.PlanDate = dAct.Date

                    ' En el caso que tengamos que cachear la definicion de los horarios (VISTA DE UN DÍA)
                    If _detailLevel <> CalendarDetailLevel.Daily Then
                        ' Inicializamos los tramos del dia
                        oCalendarCell.HourData = roCalendarRowHourDataManager.LoadEmtyData(_detailLevel)
                    End If

                    ' Comprobamos si tiene contrato para la fecha
                    oRows = tbContracts.Select("BeginDate <= '" & Format(dAct, "yyyy/MM/dd") & "' AND " &
                                                                           "EndDate >= '" & Format(dAct, "yyyy/MM/dd") & "'")
                    Dim lstDates As New Generic.List(Of DateTime)
                    If oRows.Length = 0 Then
                        oCalendarCell.EmployeeStatusOnDay = EmployeeStatusOnDayEnum.NoContract
                    Else
                        If oRows.Length = 1 Then
                            lstDates.Add(oRows(0)("BeginDate"))
                            lstDates.Add(oRows(0)("EndDate"))
                        Else
                            lstDates = Nothing
                        End If
                    End If

                    ' Comprobamos si tiene movilidad para la fecha
                    Dim strWhere As String = ""

                    If Not _BeginGroupDate = Nothing Then
                        strWhere = " AND BeginDate >= '" & Format(_BeginGroupDate, "yyyy/MM/dd") & "' "
                    End If
                    If Not _EndGroupDate = Nothing Then
                        strWhere += " AND EndDate <= '" & Format(_EndGroupDate, "yyyy/MM/dd") & "' "
                    End If

                    oRows = tbEmployeeMobilities.Select("IDGroup = " & _IDGroup & " AND " &
                                                                         "(BeginDate <= '" & Format(dAct, "yyyy/MM/dd") & "' AND " &
                                                                           "EndDate >= '" & Format(dAct, "yyyy/MM/dd") & "' " & strWhere & ")")

                    If oCalendarCell.EmployeeStatusOnDay = EmployeeStatusOnDayEnum.Ok Then
                        If oRows.Length = 0 Then oCalendarCell.EmployeeStatusOnDay = EmployeeStatusOnDayEnum.InOtherDepartment
                    End If

                    ' Si el día tiene estado OK, obtenemos el detalle
                    If oCalendarCell.EmployeeStatusOnDay = EmployeeStatusOnDayEnum.Ok OrElse _IDGroup = -1 Then
                        oCalendarCell.IncidenceData = Nothing

                        'If _TypeView = 1 Then
                        oRows = tbDetail.Select("Date = '" & Format(dAct, "yyyy/MM/dd") & "'")
                        If oRows.Length > 0 Then
                            ' Teletrabajo
                            Dim bEmployeeHasTelecommuteAgreementOnDate As Boolean = roTypes.Any2Boolean(oRows(0)("TelecommutingFromView"))
                            Dim sEmployeeTelecommuteMandatoryDays As String = roTypes.Any2String(oRows(0)("TelecommutingMandatoryDays"))
                            Dim sEmployeePresenceMandatoryDays As String = roTypes.Any2String(oRows(0)("PresenceMandatoryDays"))
                            Dim sEmployeeTelecommuteOptionalDays As String = roTypes.Any2String(oRows(0)("TelecommutingOptionalDays"))
                            Dim iEmployeeTelecommuteMaxDays As Integer = roTypes.Any2Integer(oRows(0)("TelecommutingMaxDays"))
                            Dim iEmployeeTelecommuteMaxPercentage As Integer = roTypes.Any2Integer(oRows(0)("TelecommutingMaxPercentage"))
                            Dim iEmployeeTelecommutePeriodType As Integer = roTypes.Any2Integer(oRows(0)("TelecommutingPeriodType"))

                            If Not bEmployeeHasTelecommuteAgreementOnDate Then
                                ' Sin acuerdo ese día.
                                ' No puede teletrabajar (tampoco puede indicarlo un supervisor) ...
                                oCalendarCell.CanTelecommute = False
                            Else
                                ' Tiene acuerdo de teletrabajo ese día.
                                ' Por tanto, sea com sea, podría acabar teletrabajando, aunque fuese vía solicitud, o directamente porque el supervisor así lo decide y configura en Calendario del empleado
                                oCalendarCell.CanTelecommute = True
                                oCalendarCell.TelecommutingMandatoryDays = sEmployeeTelecommuteMandatoryDays
                                oCalendarCell.PresenceMandatoryDays = sEmployeePresenceMandatoryDays
                                oCalendarCell.TelecommutingOptionalDays = sEmployeeTelecommuteOptionalDays
                                oCalendarCell.TelecommutingMaxDays = iEmployeeTelecommuteMaxDays
                                oCalendarCell.TelecommutingMaxPercentage = iEmployeeTelecommuteMaxPercentage
                                oCalendarCell.TelecommutingPeriodType = iEmployeeTelecommutePeriodType
                            End If

                            If roTypes.Any2Integer(oRows(0)("IDShiftUsed")) > 0 Then
                                ' Horario utilizado
                                oCalendarCell.ShiftUsed = New roCalendarRowShiftData
                                oCalendarCell.ShiftUsed.ID = roTypes.Any2Integer(oRows(0)("IDShiftUsed"))
                                oCalendarCell.ShiftUsed.Name = roTypes.Any2String(oRows(0)("NameUsedShift"))
                                auxColor = System.Drawing.ColorTranslator.FromWin32(roTypes.Any2Integer(oRows(0)("UsedColor")))
                                oCalendarCell.ShiftUsed.Color = roCalendarRowPeriodDataManager.HexConverter(auxColor)
                                oCalendarCell.ShiftUsed.PlannedHours = roTypes.Any2Time(roTypes.Any2Double(oRows(0)("ExpectedWorkingHoursUsedShift"))).Minutes
                                oCalendarCell.ShiftUsed.ShortName = roTypes.Any2String(oRows(0)("UsedShortName"))
                                oCalendarCell.ShiftUsed.AdvancedParameters = LoadShiftAdvancedParatemers(oRows(0)("AdvancedParametersUsed"), _State)
                                oCalendarCell.ShiftUsed.WhoToNotifyBefore = roTypes.Any2Integer(oRows(0)("WhoToNotifyBefore"))
                                oCalendarCell.ShiftUsed.WhoToNotifyAfter = roTypes.Any2Integer(oRows(0)("WhoToNotifyAfter"))
                                oCalendarCell.ShiftUsed.NotifyEmployeeBeforeAt = roTypes.Any2Integer(oRows(0)("NotifyEmployeeBeforeAt"))
                                oCalendarCell.ShiftUsed.NotifyEmployeeAfterAt = roTypes.Any2Integer(oRows(0)("NotifyEmployeeAfterAt"))
                                oCalendarCell.ShiftUsed.EnableNotifyBefore = roTypes.Any2Boolean(oRows(0)("EnableNotifyBefore"))
                                oCalendarCell.ShiftUsed.EnableNotifyAfter = roTypes.Any2Boolean(oRows(0)("EnableNotifyAfter"))
                                If Not roTypes.Any2Boolean(oRows(0)("IsFloatingUsedShift")) Then
                                    Select Case roTypes.Any2Integer(oRows(0)("ShiftTypeUsedShift"))
                                        Case 0, 1  'Normal
                                            If oCalendarCell.ShiftUsed.AdvancedParameters.ToList.FindAll(Function(x) x.Name = "Starter" AndAlso x.Value = "1").Count > 0 Then
                                                ' Starter
                                                oCalendarCell.ShiftUsed.Type = ShiftTypeEnum.Normal
                                                oCalendarCell.ShiftUsed.StartHour = roTypes.Any2Time(oRows(0)("StartFlexibleUsed")).Value
                                                oCalendarCell.ShiftUsed.EndHour = roTypes.Any2Time(oRows(0)("EndFlexibleUsed")).Value
                                                oCalendarCell.ShiftUsed.ShortName = oCalendarCell.ShiftUsed.Name
                                            Else
                                                oCalendarCell.ShiftUsed.Type = ShiftTypeEnum.Normal
                                                oCalendarCell.ShiftUsed.StartHour = roTypes.Any2Time(oRows(0)("StartLimitUsedShift")).Value
                                                oCalendarCell.ShiftUsed.EndHour = roTypes.Any2Time(oRows(0)("EndLimitUsedShift")).Value
                                            End If

                                        Case 2  'Vacaciones
                                            oCalendarCell.ShiftUsed.Type = IIf(roTypes.Any2Boolean(oRows(0)("AreWorkingDaysUsedShift")), ShiftTypeEnum.Holiday_Working, ShiftTypeEnum.Holiday_NoWorking)
                                            oCalendarCell.ShiftUsed.StartHour = roTypes.Any2Time(oRows(0)("StartLimitBase")).Value
                                            oCalendarCell.ShiftUsed.EndHour = roTypes.Any2Time(oRows(0)("EndLimitBase")).Value
                                    End Select
                                Else
                                    ' Flotante
                                    oCalendarCell.ShiftUsed.Type = ShiftTypeEnum.NormalFloating
                                    oCalendarCell.ShiftUsed.StartHour = roTypes.Any2Time(oRows(0)("StartShiftUsed")).Value
                                    oCalendarCell.ShiftUsed.EndHour = CType(roTypes.Any2Time(oRows(0)("EndLimitUsedShift")).Value, Date).AddMinutes(DateDiff(DateInterval.Minute, CDate(roTypes.Any2Time(oRows(0)("StartShiftUsed")).Value), CDate(roTypes.Any2Time(oRows(0)("StartFloatingUsedShift")).Value)))
                                End If

                                If Not (oCalendarCell.ShiftUsed.Type = ShiftTypeEnum.Holiday_NoWorking Or oCalendarCell.ShiftUsed.Type = ShiftTypeEnum.Holiday_NoWorking) Then
                                    ' Si no es de vacaciones
                                    ' Asignamos los datos de complementarias o de flotantes, en caso necesario
                                    oCalendarCell.ShiftUsed.ExistComplementaryData = roTypes.Any2Boolean(oRows(0)("ExistComplementaryDataUsedShift"))
                                    oCalendarCell.ShiftUsed.ExistFloatingData = roTypes.Any2Boolean(oRows(0)("ExistFloatingDataUsedShift"))
                                    AssignFloatingComplementaryData(oCalendarCell.ShiftUsed, oRows, _State)

                                    ' Asignamos el descanso definido
                                    oCalendarCell.ShiftUsed.BreakHours = roTypes.Any2Time(roTypes.Any2Double(oRows(0)("BreakHoursUsedShift"))).Minutes
                                End If

                            End If

                            If roTypes.Any2Integer(oRows(0)("IDShift1")) > 0 Then
                                ' Horario principal
                                oCalendarCell.MainShift = New roCalendarRowShiftData
                                oCalendarCell.MainShift.ID = roTypes.Any2Integer(oRows(0)("IDShift1"))
                                oCalendarCell.MainShift.Name = roTypes.Any2String(oRows(0)("NameShift1"))
                                oCalendarCell.MainShift.Description = roTypes.Any2String(oRows(0)("Description1"))
                                auxColor = System.Drawing.ColorTranslator.FromWin32(roTypes.Any2Integer(oRows(0)("ShiftColor1")))
                                oCalendarCell.MainShift.Color = roCalendarRowPeriodDataManager.HexConverter(auxColor)
                                oCalendarCell.MainShift.PlannedHours = roTypes.Any2Time(roTypes.Any2Double(oRows(0)("ExpectedWorkingHours1"))).Minutes
                                oCalendarCell.MainShift.ShortName = roTypes.Any2String(oRows(0)("ShortName1"))
                                oCalendarCell.MainShift.Export = roTypes.Any2String(oRows(0)("Export1"))
                                oCalendarCell.MainShift.AdvancedParameters = LoadShiftAdvancedParatemers(oRows(0)("AdvancedParameters1"), _State)
                                If Not roTypes.Any2Boolean(oRows(0)("IsFloating1")) Then
                                    Select Case roTypes.Any2Integer(oRows(0)("ShiftType1"))
                                        Case 0, 1  'Normal
                                            If oCalendarCell.MainShift.AdvancedParameters.ToList.FindAll(Function(x) x.Name = "Starter" AndAlso x.Value = "1").Count > 0 Then
                                                ' Starter
                                                oCalendarCell.MainShift.Type = ShiftTypeEnum.Normal
                                                oCalendarCell.MainShift.StartHour = roTypes.Any2Time(oRows(0)("StartFlexible1")).Value
                                                oCalendarCell.MainShift.EndHour = roTypes.Any2Time(oRows(0)("EndFlexible1")).Value
                                                oCalendarCell.MainShift.ShortName = oCalendarCell.MainShift.Name
                                            Else
                                                oCalendarCell.MainShift.Type = ShiftTypeEnum.Normal
                                                oCalendarCell.MainShift.StartHour = roTypes.Any2Time(oRows(0)("StartLimit1")).Value
                                                oCalendarCell.MainShift.EndHour = roTypes.Any2Time(oRows(0)("EndLimit1")).Value
                                            End If

                                        Case 2  'Vacaciones
                                            oCalendarCell.MainShift.Type = IIf(roTypes.Any2Boolean(oRows(0)("AreWorkingDays1")), ShiftTypeEnum.Holiday_Working, ShiftTypeEnum.Holiday_NoWorking)
                                            oCalendarCell.MainShift.StartHour = roTypes.Any2Time(oRows(0)("StartLimitBase")).Value
                                            oCalendarCell.MainShift.EndHour = roTypes.Any2Time(oRows(0)("EndLimitBase")).Value
                                    End Select
                                Else
                                    ' Flotante
                                    oCalendarCell.MainShift.Type = ShiftTypeEnum.NormalFloating
                                    oCalendarCell.MainShift.StartHour = roTypes.Any2Time(oRows(0)("StartShift1")).Value
                                    If oCalendarCell.MainShift.StartHour = "0001/01/01 00:00:00" Then oCalendarCell.MainShift.StartHour = "1899/12/30 00:00:00"
                                    oCalendarCell.MainShift.EndHour = CType(roTypes.Any2Time(oRows(0)("EndLimit1")).Value, Date).AddMinutes(DateDiff(DateInterval.Minute, CDate(roTypes.Any2Time(oRows(0)("StartFloating1")).Value), CDate(oCalendarCell.MainShift.StartHour)))
                                End If

                                If Not (oCalendarCell.MainShift.Type = ShiftTypeEnum.Holiday_NoWorking Or oCalendarCell.MainShift.Type = ShiftTypeEnum.Holiday_NoWorking) Then
                                    ' Asignamos los datos de complementarias o de flotantes, en caso necesario
                                    oCalendarCell.MainShift.ExistComplementaryData = roTypes.Any2Boolean(oRows(0)("ExistComplementaryDataShift1"))
                                    oCalendarCell.MainShift.ExistFloatingData = roTypes.Any2Boolean(oRows(0)("ExistFloatingData"))
                                    AssignFloatingComplementaryData(oCalendarCell.MainShift, oRows, _State)

                                    ' Asignamos el descanso definido
                                    oCalendarCell.MainShift.BreakHours = roTypes.Any2Time(roTypes.Any2Double(oRows(0)("BreakHours1"))).Minutes
                                End If

                            End If

                            If roTypes.Any2Integer(oRows(0)("IDShiftBase")) > 0 Then
                                ' Horario base
                                oCalendarCell.ShiftBase = New roCalendarRowShiftData
                                oCalendarCell.ShiftBase.ID = roTypes.Any2Integer(oRows(0)("IDShiftBase"))
                                oCalendarCell.ShiftBase.Name = roTypes.Any2String(oRows(0)("NameShiftBase"))
                                auxColor = System.Drawing.ColorTranslator.FromWin32(roTypes.Any2Integer(oRows(0)("ShiftColorBase")))
                                oCalendarCell.ShiftBase.Color = roCalendarRowPeriodDataManager.HexConverter(auxColor)
                                oCalendarCell.ShiftBase.PlannedHours = roTypes.Any2Time(roTypes.Any2Double(oRows(0)("ExpectedWorkingHoursBase"))).Minutes
                                oCalendarCell.ShiftBase.ShortName = roTypes.Any2String(oRows(0)("ShortNameBase"))
                                oCalendarCell.ShiftBase.AdvancedParameters = LoadShiftAdvancedParatemers(oRows(0)("AdvancedParametersBase"), _State)
                                If Not roTypes.Any2Boolean(oRows(0)("IsFloatingBase")) Then
                                    Select Case roTypes.Any2Integer(oRows(0)("ShiftTypeBase"))
                                        Case 0, 1  'Normal
                                            If oCalendarCell.ShiftBase.AdvancedParameters.ToList.FindAll(Function(x) x.Name = "Starter" AndAlso x.Value = "1").Count > 0 Then
                                                ' Starter
                                                oCalendarCell.ShiftBase.Type = ShiftTypeEnum.Normal
                                                oCalendarCell.ShiftBase.StartHour = roTypes.Any2Time(oRows(0)("StartFlexibleBase")).Value
                                                oCalendarCell.ShiftBase.EndHour = roTypes.Any2Time(oRows(0)("EndFlexibleBase")).Value
                                                oCalendarCell.ShiftBase.ShortName = oCalendarCell.ShiftBase.Name
                                            Else
                                                oCalendarCell.ShiftBase.Type = ShiftTypeEnum.Normal
                                                oCalendarCell.ShiftBase.StartHour = roTypes.Any2Time(oRows(0)("StartLimitBase")).Value
                                                Try
                                                    oCalendarCell.ShiftBase.EndHour = roTypes.Any2Time(oRows(0)("EndLimitBase")).Value
                                                Catch ex As Exception
                                                End Try
                                            End If
                                        Case 2  'Vacaciones
                                            oCalendarCell.ShiftBase.Type = IIf(roTypes.Any2Boolean(oRows(0)("AreWorkingDaysBase")), ShiftTypeEnum.Holiday_Working, ShiftTypeEnum.Holiday_NoWorking)
                                    End Select
                                Else
                                    ' Flotante
                                    oCalendarCell.ShiftBase.Type = ShiftTypeEnum.NormalFloating
                                    oCalendarCell.ShiftBase.StartHour = roTypes.Any2Time(oRows(0)("StartShiftBase")).Value
                                    If oCalendarCell.ShiftBase.StartHour = "0001/01/01 00:00:00" Then oCalendarCell.ShiftBase.StartHour = "1899/12/30 00:00:00"
                                    Try
                                        oCalendarCell.ShiftBase.EndHour = CType(roTypes.Any2Time(oRows(0)("EndLimitBase")).Value, Date).AddMinutes(DateDiff(DateInterval.Minute, CDate(roTypes.Any2Time(oRows(0)("StartFloatingBase")).Value), CDate(oCalendarCell.ShiftBase.StartHour)))
                                    Catch ex As Exception
                                    End Try

                                End If

                                ' Asignamos los datos de complementarias o de flotantes, en caso necesario
                                oCalendarCell.ShiftBase.ExistComplementaryData = roTypes.Any2Boolean(oRows(0)("ExistComplementaryDataBase"))
                                oCalendarCell.ShiftBase.ExistFloatingData = roTypes.Any2Boolean(oRows(0)("ExistFloatingDataBase"))
                                AssignFloatingComplementaryData(oCalendarCell.ShiftBase, oRows, _State)

                                ' Asignamos el descanso definido
                                oCalendarCell.ShiftBase.BreakHours = roTypes.Any2Time(roTypes.Any2Double(oRows(0)("BreakHoursBase"))).Minutes

                            End If

                            If roTypes.Any2Integer(oRows(0)("IDPreviousShift")) > 0 Then
                                ' Horario principal
                                oCalendarCell.PreviousShift = New roCalendarRowShiftData
                                oCalendarCell.PreviousShift.ID = roTypes.Any2Integer(oRows(0)("IDPreviousShift"))
                            End If

                            ' En el caso que tengamos que cachear la definicion de los horarios (VISTA DE UN DÍA)
                            If _detailLevel <> CalendarDetailLevel.Daily Then
                                ' Obtenemos el horario utilizado, si no está informado obtenemos el principal
                                Dim intShiftHourDefinition As Integer = 0
                                Dim StartFloating As Date = Nothing
                                Dim oCalendarRowShiftData As New roCalendarRowShiftData

                                If roTypes.Any2Integer(oRows(0)("IDShiftUsed")) > 0 Then
                                    intShiftHourDefinition = roTypes.Any2Integer(oRows(0)("IDShiftUsed"))
                                    StartFloating = oCalendarCell.ShiftUsed.StartHour
                                    If oCalendarCell.ShiftUsed IsNot Nothing Then oCalendarRowShiftData = oCalendarCell.ShiftUsed

                                ElseIf roTypes.Any2Integer(oRows(0)("IDShift1")) > 0 Then
                                    intShiftHourDefinition = roTypes.Any2Integer(oRows(0)("IDShift1"))
                                    StartFloating = oCalendarCell.MainShift.StartHour
                                    If oCalendarCell.MainShift IsNot Nothing Then oCalendarRowShiftData = oCalendarCell.MainShift
                                End If

                                If intShiftHourDefinition > 0 Then
                                    Dim oShift As Shift.roShift = Nothing
                                    ' En caso necesario lo añadimos al cache de horarios
                                    If Not oShiftCache.Contains(intShiftHourDefinition) Then
                                        oShift = New Shift.roShift(intShiftHourDefinition, New Shift.roShiftState(_State.IDPassport))
                                        oShiftCache.Add(oShift.ID, oShift)
                                    End If

                                    oShift = oShiftCache(intShiftHourDefinition)

                                    If oShift IsNot Nothing Then
                                        ' Si el horario asignado es de vacaciones, hay que guardarse tambien el horario base
                                        If oCalendarCell.ShiftBase IsNot Nothing Then
                                            If Not oShiftCache.Contains(oCalendarCell.ShiftBase.ID) Then
                                                oShift = New Shift.roShift(oCalendarCell.ShiftBase.ID, New Shift.roShiftState(_State.IDPassport))
                                                oShiftCache.Add(oShift.ID, oShift)
                                            End If
                                            oShift = oShiftCache(oCalendarCell.ShiftBase.ID)
                                            StartFloating = oCalendarCell.ShiftBase.StartHour
                                            If oCalendarCell.ShiftBase IsNot Nothing Then oCalendarRowShiftData = oCalendarCell.ShiftBase
                                        End If

                                        ' Obtenemos los tramos para ese dia y empleado en funcion del horario asignado
                                        Dim oCalendarRowHourDataState As New roCalendarRowHourDataState(_State.IDPassport)
                                        Dim oCalendarRowHourData As New roCalendarRowHourDataManager(oCalendarRowHourDataState)
                                        oCalendarCell.HourData = oCalendarRowHourData.Load(dAct, _IDEmployee, oShift, StartFloating, tbProgrammedCauses, oCalendarRowShiftData, tbProgrammedAbsences, lstProgrammedHolidays, lstProgrammedOvertimes, _detailLevel)
                                    End If
                                End If

                            End If

                            oCalendarCell.IsHoliday = roTypes.Any2Boolean(oRows(0)("IsHolidays"))

                            oCalendarCell.Locked = roTypes.Any2Boolean(oRows(0)("LockedDay"))

                            If roTypes.Any2DateTime(oRows(0)("Timestamp")) <> DateTime.MinValue Then
                                oCalendarCell.Timestamp = roTypes.Any2DateTime(oRows(0)("Timestamp"))
                            End If

                            oCalendarCell.Feast = roTypes.Any2Boolean(oRows(0)("FeastDay"))
                            If oCalendarCell.Feast Then
                                oCalendarCell.FeastDescription = roTypes.Any2String(oRows(0)("FeastDayDescription"))
                                If oCalendarCell.FeastDescription.Length = 0 Then oCalendarCell.FeastDescription = _State.Language.Translate("CalendarRowPeriodDataManager.FeastDay.FeastDayDescription", "")
                            End If

                            oCalendarCell.IDDailyBudgetPosition = roTypes.Any2Long(oRows(0)("IDDailyBudgetPosition"))
                            If oCalendarCell.IDDailyBudgetPosition > 0 Then
                                oCalendarCell.ProductiveUnit = roTypes.Any2String(oRows(0)("ProductiveUnitName"))
                            End If

                            ' Obtenemos Puesto asignado en caso de tener licencia
                            If bolLicenseHRScheduling Then
                                oCalendarCell.AllowAssignment = IIf(roTypes.Any2Double(oRows(0)("AvailableAssignments")) > 0, True, False)
                                If roTypes.Any2Double(oRows(0)("IDAssignmentFix")) > 0 Then
                                    oCalendarCell.AssigData = New roCalendarAssignmentCellData
                                    oCalendarCell.AssigData.ID = roTypes.Any2Double(oRows(0)("IDAssignmentFix"))
                                    oCalendarCell.AssigData.Name = roTypes.Any2String(oRows(0)("AssignmentFixName"))
                                    oCalendarCell.AssigData.ShortName = roTypes.Any2String(oRows(0)("AssignmentFixShortName"))
                                    oCalendarCell.AssigData.Cover = roTypes.Any2Double(oRows(0)("CoverageAssignment"))
                                    oCalendarCell.AssigData.Color = roCalendarRowPeriodDataManager.HexConverter(System.Drawing.ColorTranslator.FromWin32(roTypes.Any2Integer(oRows(0)("AssignmentFixColor"))))
                                End If
                            End If

                            ' Indicamos si tene observaciones ese día
                            oCalendarCell.Remarks = IIf(roTypes.Any2String(oRows(0)("Remarks")) = "", "0", roTypes.Any2String(oRows(0)("Remarks")))

                            ' Centro de trabajo y zona esperada del empleado ese día
                            If bLoadSeatingCapacity AndAlso tbSeatingCapacityDetail IsNot Nothing AndAlso tbSeatingCapacityDetail.Rows.Count > 0 Then
                                Dim oRowsSeating() As DataRow = Nothing
                                oRowsSeating = tbSeatingCapacityDetail.Select("RefDate = '" & Format(dAct, "yyyy/MM/dd") & "'")
                                If oRowsSeating.Count > 0 Then
                                    oCalendarCell.Workcenter = roTypes.Any2String(oRowsSeating(0)("CalculatedWorkcenter"))
                                    oCalendarCell.ZoneName = roTypes.Any2String(oRowsSeating(0)("Zone"))
                                End If
                            End If

                            ' Teletrabajo
                            ' Vemos si se espera que teletrabaje.
                            If oCalendarCell.MainShift IsNot Nothing Then
                                If Not oRows(0)("Telecommuting") Is DBNull.Value Then
                                    oCalendarCell.TelecommutingExpected = roTypes.Any2Boolean(oRows(0)("Telecommuting"))
                                    oCalendarCell.TelecommuteForced = True

                                    If roTypes.Any2Boolean(oRows(0)("TelecommutingOptional")) Then
                                        oCalendarCell.TelecommutingOptional = True
                                    Else
                                        oCalendarCell.TelecommutingOptional = False
                                    End If
                                Else
                                    ' Miro en función del día de la semana
                                    oCalendarCell.TelecommutingExpected = sEmployeeTelecommuteMandatoryDays.Contains(oCalendarCell.PlanDate.DayOfWeek)
                                    oCalendarCell.TelecommutingOptional = sEmployeeTelecommuteOptionalDays.Contains(oCalendarCell.PlanDate.DayOfWeek)
                                    oCalendarCell.TelecommuteForced = False
                                End If
                            Else
                                ' Si no hay teóricas, o estoy en ausencia, no puedo teletrabajar ni ir a la oficina
                                oCalendarCell.TelecommutingExpected = False
                                oCalendarCell.TelecommutingOptional = False
                                oCalendarCell.TelecommuteForced = False
                            End If
                        Else

                            Dim oTCRows As DataRow() = dtTelecommuteAgreements.Select("(ContractStart <= '" & Format(dAct, "yyyy/MM/dd") & "' AND ContractEnd >= '" & Format(dAct, "yyyy/MM/dd") & "')" &
                                                                           " AND (TelecommutingAgreementStart <= '" & Format(dAct, "yyyy/MM/dd") & "' AND TelecommutingAgreementEnd >= '" & Format(dAct, "yyyy/MM/dd") & "')")

                            If oTCRows.Length = 0 Then
                                oCalendarCell.CanTelecommute = False
                            Else
                                oCalendarCell.CanTelecommute = roTypes.Any2Boolean(oTCRows(0)("Telecommuting"))
                                oCalendarCell.TelecommutingMandatoryDays = roTypes.Any2String(oTCRows(0)("TelecommutingMandatoryDays"))
                                oCalendarCell.PresenceMandatoryDays = roTypes.Any2String(oTCRows(0)("PresenceMandatoryDays"))
                                oCalendarCell.TelecommutingOptionalDays = roTypes.Any2String(oTCRows(0)("TelecommutingOptionalDays"))
                                oCalendarCell.TelecommutingMaxDays = roTypes.Any2Integer(oTCRows(0)("TelecommutingMaxDays"))
                                oCalendarCell.TelecommutingMaxPercentage = roTypes.Any2Integer(oTCRows(0)("TelecommutingMaxPercentage"))
                                oCalendarCell.TelecommutingPeriodType = roTypes.Any2Integer(oTCRows(0)("PeriodType"))
                            End If

                        End If

                        If bLoadAlerts Then

                            ' Alertas del dia
                            oCalendarCell.Alerts = New roCalendarRowDayAlerts
                            oCalendarCell.Alerts.OnAbsenceDays = False
                            oRows = tbProgrammedAbsences.Select("(BeginDate <= '" & Format(dAct, "yyyy/MM/dd") & "' AND " &
                                                                "RealFinishDate >= '" & Format(dAct, "yyyy/MM/dd") & "')")
                            If oRows.Length > 0 Then
                                oCalendarCell.Alerts.OnAbsenceDays = True
                                oRowsInfo = tbCauses.Select("ID=" & oRows(0)("IDCause").ToString)
                                If oRowsInfo.Length > 0 Then
                                    oCalendarCell.Alerts.OnAbsenceDaysInfo = oRowsInfo(0)("ShortName").ToString
                                End If
                            End If

                            oCalendarCell.Alerts.OnAbsenceHours = False
                            oRows = tbProgrammedCauses.Select("Date <= '" & Format(dAct, "yyyy/MM/dd") & "' and isnull(FinishDate, date) >= '" & Format(dAct, "yyyy/MM/dd") & "'")
                            If oRows.Length > 0 Then
                                oCalendarCell.Alerts.OnAbsenceHours = True
                                For Each xRow As DataRow In oRows
                                    oRowsInfo = tbCauses.Select("ID=" & xRow("IDCause").ToString)
                                    If oRowsInfo.Length > 0 Then
                                        If oCalendarCell.Alerts.OnAbsenceHoursInfo.Length > 0 Then
                                            oCalendarCell.Alerts.OnAbsenceHoursInfo += "," & oRowsInfo(0)("ShortName").ToString
                                        Else
                                            oCalendarCell.Alerts.OnAbsenceHoursInfo = oRowsInfo(0)("ShortName").ToString
                                        End If
                                    End If
                                Next
                            End If

                            oCalendarCell.Alerts.OnHolidaysHours = False
                            If lstProgrammedHolidays.Count > 0 Then
                                For Each oProgrammedHoliday As roProgrammedHoliday In lstProgrammedHolidays
                                    If oProgrammedHoliday.ProgrammedDate = dAct Then
                                        oCalendarCell.Alerts.OnHolidaysHours = True
                                        oRowsInfo = tbCauses.Select("ID=" & oProgrammedHoliday.IDCause.ToString)
                                        If oRowsInfo.Length > 0 Then
                                            If oCalendarCell.Alerts.OnHolidaysHoursInfo.Length > 0 Then
                                                oCalendarCell.Alerts.OnHolidaysHoursInfo += "," & oRowsInfo(0)("ShortName").ToString
                                            Else
                                                oCalendarCell.Alerts.OnHolidaysHoursInfo = oRowsInfo(0)("ShortName").ToString
                                            End If
                                        End If
                                    End If
                                Next
                            End If

                            oCalendarCell.Alerts.OnOvertimesHours = False
                            If lstProgrammedOvertimes.Count > 0 Then
                                For Each oProgrammedOvertime As roProgrammedOvertime In lstProgrammedOvertimes
                                    If oProgrammedOvertime.ProgrammedBeginDate <= dAct And oProgrammedOvertime.ProgrammedEndDate >= dAct Then
                                        oCalendarCell.Alerts.OnOvertimesHours = True
                                        oRowsInfo = tbCauses.Select("ID=" & oProgrammedOvertime.IDCause.ToString)
                                        If oRowsInfo.Length > 0 Then
                                            If oCalendarCell.Alerts.OnOvertimesHoursInfo.Length > 0 Then
                                                oCalendarCell.Alerts.OnOvertimesHoursInfo += "," & oRowsInfo(0)("ShortName").ToString
                                            Else
                                                oCalendarCell.Alerts.OnOvertimesHoursInfo = oRowsInfo(0)("ShortName").ToString
                                            End If
                                        End If
                                    End If
                                Next
                            End If

                            oCalendarCell.Alerts.OnHolidays = oCalendarCell.IsHoliday
                            oCalendarCell.Alerts.UnexpectedlyAbsent = False
                            If dAct = Now.Date Then
                                ' En el caso que cargamos los datos del dia de hoy, revisamos si el empleado esta ausente
                                Dim oEmployeeStatus As Employee.roEmployeeStatus
                                oEmployeeStatus = New Employee.roEmployeeStatus(_IDEmployee, oEmployeeState)
                                If oCalendarCell.ShiftUsed IsNot Nothing AndAlso oCalendarCell.ShiftUsed.ID IsNot Nothing Then
                                    If Not oEmployeeStatus.IsPresent AndAlso (Not oEmployeeStatus.BeginMandatory.HasValue OrElse oEmployeeStatus.BeginMandatory.Value < Now) Then
                                        oCalendarCell.Alerts.UnexpectedlyAbsent = True
                                    End If
                                End If
                            End If
                        End If

                        oCalendarCell.FlagColor = "" 'FALTA

                        If _typeView = CalendarView.Review OrElse _IsEmployeePortal Then
                            oCalendarCell.IncidenceData = New roCalendarRowIncidenceData

                            If _IsEmployeePortal AndAlso dtEmpRequests IsNot Nothing Then
                                oRows = dtEmpRequests.Select("RequestedDate = '" & Format(dAct, "yyyy/MM/dd") & "'")
                                If oCalendarCell.MainShift IsNot Nothing AndAlso oRows IsNot Nothing AndAlso oRows.Length > 0 AndAlso Not IsDBNull(oRows(0)("ID")) Then
                                    If roTypes.Any2Integer(oRows(0)("RequestType")) = eRequestType.ChangeShift Then
                                        Dim iSource = roTypes.Any2Integer(oRows(0)("IDSourceShift"))
                                        If iSource = 0 OrElse iSource = oCalendarCell.MainShift.ID Then
                                            auxColor = System.Drawing.ColorTranslator.FromWin32(roTypes.Any2Integer(oRows(0)("IDSourceShiftColor")))
                                            oCalendarCell.RequestedShift = roCalendarRowPeriodDataManager.HexConverter(auxColor)
                                        Else
                                            oCalendarCell.RequestedShift = oCalendarCell.MainShift.Color
                                        End If
                                    Else
                                        If roTypes.Any2Integer(oRows(0)("IDEmployee")) = _IDEmployee Then
                                            auxColor = System.Drawing.ColorTranslator.FromWin32(roTypes.Any2Integer(oRows(0)("IDRequestShiftColor")))
                                            oCalendarCell.RequestedShift = roCalendarRowPeriodDataManager.HexConverter(auxColor)
                                        Else
                                            auxColor = System.Drawing.ColorTranslator.FromWin32(roTypes.Any2Integer(oRows(0)("IDSourceShiftColor")))
                                            oCalendarCell.RequestedShift = roCalendarRowPeriodDataManager.HexConverter(auxColor)
                                        End If
                                    End If
                                Else
                                    oCalendarCell.RequestedShift = String.Empty
                                End If
                            End If

                            If Not _IsEmployeePortal Then
                                ' Obtenemos los datos de la vista de revision
                                If oConceptAbsence IsNot Nothing AndAlso oConceptAbsence.ID > 0 Then oCalendarCell.IncidenceData.Absence = Math.Abs(Concept.roConcept.GetAccrualValueOnDate(_IDEmployee, oParameters, dAct, True, oConceptAbsence, oEmployeeState, lstDates))
                                If oConceptNormalWork IsNot Nothing AndAlso oConceptNormalWork.ID > 0 Then oCalendarCell.IncidenceData.NormalWork = Math.Abs(Concept.roConcept.GetAccrualValueOnDate(_IDEmployee, oParameters, dAct, True, oConceptNormalWork, oEmployeeState, lstDates))
                                If oConceptOverWorking IsNot Nothing AndAlso oConceptOverWorking.ID > 0 Then oCalendarCell.IncidenceData.OverWorking = Math.Abs(Concept.roConcept.GetAccrualValueOnDate(_IDEmployee, oParameters, dAct, True, oConceptOverWorking, oEmployeeState, lstDates))

                                ' Calculamos los %
                                Dim dblTotalTime As Double = oCalendarCell.IncidenceData.Absence + oCalendarCell.IncidenceData.NormalWork + oCalendarCell.IncidenceData.OverWorking
                                If dblTotalTime = 0 Then
                                    oCalendarCell.IncidenceData.NormalWork = 100
                                Else
                                    oCalendarCell.IncidenceData.Absence = Math.Round((oCalendarCell.IncidenceData.Absence * 100) / dblTotalTime, 2)
                                    oCalendarCell.IncidenceData.NormalWork = Math.Round((oCalendarCell.IncidenceData.NormalWork * 100) / dblTotalTime, 2)
                                    oCalendarCell.IncidenceData.OverWorking = Math.Round((oCalendarCell.IncidenceData.OverWorking * 100) / dblTotalTime, 2)

                                    Dim dblDif As Double = 100 - (oCalendarCell.IncidenceData.Absence + oCalendarCell.IncidenceData.NormalWork + oCalendarCell.IncidenceData.OverWorking)
                                    If dblDif < 0 Then
                                        dblDif = Math.Abs(dblDif)
                                        If oCalendarCell.IncidenceData.Absence >= dblDif Then oCalendarCell.IncidenceData.Absence = oCalendarCell.IncidenceData.Absence - dblDif
                                        If oCalendarCell.IncidenceData.OverWorking >= dblDif Then oCalendarCell.IncidenceData.OverWorking = oCalendarCell.IncidenceData.OverWorking - dblDif
                                        If oCalendarCell.IncidenceData.NormalWork >= dblDif Then oCalendarCell.IncidenceData.NormalWork = oCalendarCell.IncidenceData.NormalWork - dblDif
                                    End If
                                End If

                                'Mostramos las alertas
                                Dim bolOverWorking As Boolean = False
                                Dim bolAbsences As Boolean = False
                                Dim bolNotjustified As Boolean = False

                                Dim bRemarkActive As Boolean() = Scheduler.roSchedulerRemarks.GetCalendarSchedulerRemarks(_IDEmployee, dAct, oSchedulerRemarks, oSchduleState, tbDailyCauses1, tbDailyCauses2, tbDailyCauses3)

                                ' No justificados > 0
                                'Dim oDataTable As DataTable = oEmployees.GetCausesByFillter(_IDEmployee, "IDCause IN(0)", dAct, oEmployeeState)
                                'If oDataTable IsNot Nothing AndAlso oDataTable.Rows.Count > 0 Then bolNotjustified = True
                                oCalendarCell.IncidenceData.Remark1 = bRemarkActive(0)

                                ' Si hay alguna incidencia de ausencia
                                'oDataTable = oEmployees.GetIncidencesByFillter(_IDEmployee, "IDType IN(@SELECT# ID FROM sysroDailyIncidencesTypes WHERE WorkingTime = 0)", dAct, oEmployeeState)
                                'If oDataTable IsNot Nothing AndAlso oDataTable.Rows.Count > 0 Then bolAbsences = True
                                oCalendarCell.IncidenceData.Remark2 = bRemarkActive(1)

                                ' Si hay alguna incidencia de extras (EXTRAS EN NORMALES, EXTRAS EN FLEXIBLE)
                                'oDataTable = oEmployees.GetIncidencesByFillter(_IDEmployee, "IDType IN(1010,1030)", dAct, oEmployeeState)
                                'If oDataTable IsNot Nothing AndAlso oDataTable.Rows.Count > 0 Then bolOverWorking = True
                                oCalendarCell.IncidenceData.Remark3 = bRemarkActive(2)
                            Else
                                oCalendarCell.IncidenceData.NormalWork = Concept.roConcept.GetAccrualValueOnDate(_IDEmployee, oParameters, dAct, True, oConceptNormalWork, oEmployeeState)
                            End If

                        End If
                    End If

                    ' Si el supervisor tiene permisos de escritura y es un día normal
                    ' lo marcamos para que se pueda modificar
                    If oPermission > 3 And oCalendarCell.EmployeeStatusOnDay = EmployeeStatusOnDayEnum.Ok Then oCalendarCell.CanBeModified = True

                    oCalendarPeriodCells.Add(oCalendarCell)

                    ' vamos al siguiente día
                    dAct = dAct.AddDays(1)
                End While

                oRet.DayData = oCalendarPeriodCells.ToArray

                bolRet = True
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roCalendarRowPeriodDataManager::LoadCellsByCalendar")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roCalendarRowPeriodDataManager::LoadCellsByCalendar")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function LoadSchedulerPlanByCalendar(ByVal _FirstDay As DateTime, ByVal _LastDay As DateTime, ByVal _IDEmployee As Integer, ByRef _State As roCalendarRowPeriodDataState, Optional ByVal ProgrammedHolidays_IDCause As Integer = -1) As roCalendarRowPeriodData

            ' Llenamos las celdas de del empleado/grupo del periodo indicado
            Dim oRet As New roCalendarRowPeriodData

            Dim bolRet As Boolean = False
            Dim oCalendarCell As roCalendarRowDayData
            Dim oCalendarPeriodCells As New Generic.List(Of roCalendarRowDayData)

            Dim oRows() As DataRow = Nothing

            Try

                Dim oEmployeeState As New Employee.roEmployeeState(_State.IDPassport, _State.Context, _State.ClientAddress)

                ' 01. Obtenemos la movilidades
                Dim tbEmployeeMobilities As DataTable = Employee.roMobility.GetMobilities(_IDEmployee, oEmployeeState)

                ' 02. Obtenemos los contratos
                Dim oContractState As New Contract.roContractState(_State.IDPassport, _State.Context, _State.ClientAddress)
                Dim tbContracts As DataTable = Contract.roContract.GetContractsByIDEmployee(_IDEmployee, oContractState)

                ' Obtenemos el ultimo fin de contrato
                Dim xLastEndDateContract = New DateTime(1990, 1, 1)
                Try
                    If tbContracts IsNot Nothing AndAlso tbContracts.Rows.Count > 0 AndAlso Not tbContracts.Rows(tbContracts.Rows.Count - 1).Item("EndDate") Is Nothing Then
                        xLastEndDateContract = tbContracts.Rows(tbContracts.Rows.Count - 1).Item("EndDate")
                    End If
                Catch ex As Exception
                End Try

                ' 03. Obtenemos las previsiones de vacaciones por horas
                Dim oProgrammedHolidaysState As New roProgrammedHolidayState(_State.IDPassport, _State.Context, _State.ClientAddress)
                Dim oProgrammedHolidayManager As New roProgrammedHolidayManager()
                Dim lstProgrammedHolidays As New Generic.List(Of roProgrammedHoliday)
                Dim strFillterProgrammedHolidays As String = "Date >=" & roTypes.Any2Time(_FirstDay).SQLSmallDateTime & " AND Date <=" & roTypes.Any2Time(_LastDay).SQLSmallDateTime

                If ProgrammedHolidays_IDCause <> -1 Then
                    strFillterProgrammedHolidays = strFillterProgrammedHolidays & " AND IDCause=" & ProgrammedHolidays_IDCause
                End If

                lstProgrammedHolidays = oProgrammedHolidayManager.GetProgrammedHolidays(_IDEmployee, oProgrammedHolidaysState, strFillterProgrammedHolidays)

                Dim tbDetail As DataTable = Nothing

                ' 04. Obtenemos la planificacion de todo el periodo
                tbDetail = VTBusiness.Scheduler.roScheduler.GetPlan(_IDEmployee, _FirstDay, _LastDay, oEmployeeState, -1, True)

                ' 05 .Para cada día del periodo , obtenemos los datos necesarios
                Dim dAct As Date = _FirstDay
                Dim dEnd As Date = _LastDay
                Dim auxColor As System.Drawing.Color = Color.White
                Dim oSchduleState As Scheduler.roSchedulerState = Nothing

                While (dAct <= dEnd)
                    oCalendarCell = New roCalendarRowDayData
                    oCalendarCell.PlanDate = dAct.Date

                    ' Comprobamos si tiene contrato para la fecha
                    oRows = tbContracts.Select("BeginDate <= '" & Format(dAct, "yyyy/MM/dd") & "' AND " &
                                                                           "EndDate >= '" & Format(dAct, "yyyy/MM/dd") & "'")
                    If oRows.Length = 0 Then
                        oCalendarCell.EmployeeStatusOnDay = EmployeeStatusOnDayEnum.NoContract

                        If dAct > xLastEndDateContract AndAlso xLastEndDateContract > Now.Date Then
                            ' Si la fecha es posterior a la fecha del ultimo contrato y la fecha del ultimo contrato es posterior a hoy
                            ' habilitamos el dia
                            oCalendarCell.EmployeeStatusOnDay = EmployeeStatusOnDayEnum.Ok
                        End If
                    End If

                    ' Si el día tiene estado OK, obtenemos el detalle
                    If oCalendarCell.EmployeeStatusOnDay = EmployeeStatusOnDayEnum.Ok Then
                        oCalendarCell.IncidenceData = Nothing

                        oRows = tbDetail.Select("Date = '" & Format(dAct, "yyyy/MM/dd") & "'")
                        If oRows.Length > 0 Then
                            If roTypes.Any2Integer(oRows(0)("IDShift1")) > 0 Then
                                ' Horario principal
                                oCalendarCell.MainShift = New roCalendarRowShiftData
                                oCalendarCell.MainShift.ID = roTypes.Any2Integer(oRows(0)("IDShift1"))
                                oCalendarCell.MainShift.Name = roTypes.Any2String(oRows(0)("NameShift1"))
                                auxColor = System.Drawing.ColorTranslator.FromWin32(roTypes.Any2Integer(oRows(0)("ShiftColor1")))
                                oCalendarCell.MainShift.Color = roCalendarRowPeriodDataManager.HexConverter(auxColor)
                                oCalendarCell.MainShift.PlannedHours = roTypes.Any2Time(roTypes.Any2Double(oRows(0)("ExpectedWorkingHours1"))).Minutes
                                oCalendarCell.MainShift.ShortName = roTypes.Any2String(oRows(0)("ShortName1"))
                                oCalendarCell.MainShift.StartHour = New DateTime(1970, 1, 1, 0, 0, 0, 0)
                            End If

                            oCalendarCell.IsHoliday = roTypes.Any2Boolean(oRows(0)("IsHolidays"))
                        End If

                        ' Indicamos si el dia tiene vacaciones por horas asignadas en el dia
                        oCalendarCell.Alerts = New roCalendarRowDayAlerts
                        oCalendarCell.Alerts.OnHolidaysHours = False
                        If lstProgrammedHolidays.Count > 0 Then
                            For Each oProgrammedHoliday As roProgrammedHoliday In lstProgrammedHolidays
                                If oProgrammedHoliday.ProgrammedDate = dAct Then
                                    oCalendarCell.Alerts.OnHolidaysHours = True
                                    Exit For
                                End If
                            Next
                        End If
                    End If

                    oCalendarPeriodCells.Add(oCalendarCell)

                    ' vamos al siguiente día
                    dAct = dAct.AddDays(1)
                End While

                oRet.DayData = oCalendarPeriodCells.ToArray

                bolRet = True
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roCalendarRowPeriodDataManager::LoadSchedulerPlanByCalendar")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roCalendarRowPeriodDataManager::LoadSchedulerPlanByCalendar")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Sub AssignFloatingComplementaryData(ByRef oCalendarRowShiftData As roCalendarRowShiftData, ByVal oRows() As DataRow, ByRef _State As roCalendarRowPeriodDataState)
            Try

                If oCalendarRowShiftData.ExistFloatingData Or oCalendarRowShiftData.ExistComplementaryData Then
                    Dim totallayersduration As Double = 0
                    Dim oCalendarShiftLayersDefinitionList As New List(Of roCalendarShiftLayersDefinition)

                    Dim oLayersDefinition As New roCollection(roTypes.Any2String(oRows(0)("LayersDefinition")))
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
                                If oLayersDefinition.Exists("LayerComplementaryHours_" & iLayer.ToString) Then
                                    oCalendarShiftLayersDefinition.LayerComplementaryHours = roTypes.Any2Time(roTypes.Any2Double(oLayersDefinition("LayerComplementaryHours_" & iLayer.ToString))).Minutes
                                End If
                                If oLayersDefinition.Exists("LayerOrdinaryHours_" & iLayer.ToString) Then oCalendarShiftLayersDefinition.LayerOrdinaryHours = roTypes.Any2Time(roTypes.Any2Double(oLayersDefinition("LayerOrdinaryHours_" & iLayer.ToString))).Minutes

                                If iLayer = intTotalLayers Then
                                    ' Si es la última franja, asigno la hora de finalización del horario
                                    oCalendarRowShiftData.EndHour = oCalendarShiftLayersDefinition.LayerStartTime.AddMinutes(roTypes.Any2Double(oCalendarShiftLayersDefinition.LayerComplementaryHours) + roTypes.Any2Double(oCalendarShiftLayersDefinition.LayerOrdinaryHours))
                                End If
                            End If

                            oCalendarShiftLayersDefinitionList.Add(oCalendarShiftLayersDefinition)
                        Next

                    End If

                    oCalendarRowShiftData.ShiftLayersDefinition = oCalendarShiftLayersDefinitionList.ToArray
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roCalendarRowPeriodDataManager::AssignFloatingComplementaryData")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roCalendarRowPeriodDataManager::AssignFloatingComplementaryData")

            End Try

        End Sub

        Public Shared Function GetUniqueKey(ByRef oCalendarRowShiftData As roCalendarRowShiftData, ByRef _State As roCalendarRowPeriodDataState) As String
            Dim strRet As String = ""

            Try

                If oCalendarRowShiftData IsNot Nothing Then
                    If oCalendarRowShiftData.ExistComplementaryData Or oCalendarRowShiftData.ExistFloatingData Then
                        ' Si el horario es por horas
                        ' la clave es el ID del horario + total de horas planificadas + nº de franjas + [hora inicio horario] + [hora fin horario]
                        strRet = oCalendarRowShiftData.ID.ToString + "_" + oCalendarRowShiftData.ShortName + "_" + oCalendarRowShiftData.PlannedHours.ToString + "_" + oCalendarRowShiftData.ShiftLayers.ToString + "_" + oCalendarRowShiftData.StartHour.ToString("ddHHmm") + "_" + oCalendarRowShiftData.EndHour.ToString("ddHHmm")
                        For iLayer As Integer = 0 To oCalendarRowShiftData.ShiftLayers - 1
                            ' Cada franaja ID Franja, Hora de inicio , Duracion
                            strRet += "_" + oCalendarRowShiftData.ShiftLayersDefinition(iLayer).LayerID.ToString
                            strRet += "_" + oCalendarRowShiftData.ShiftLayersDefinition(iLayer).LayerStartTime.ToString("ddHHmm")
                            strRet += "_" + oCalendarRowShiftData.ShiftLayersDefinition(iLayer).LayerDuration.ToString
                        Next
                    Else
                        ' En cualquier otro caso,
                        ' la clave es el ID del horario + starthour + endhour
                        strRet = oCalendarRowShiftData.ID.ToString + "_" + oCalendarRowShiftData.ShortName + "_" + oCalendarRowShiftData.StartHour.ToString("ddHHmm") + "_" + oCalendarRowShiftData.EndHour.ToString("ddHHmm")
                    End If
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roCalendarRowPeriodDataManager::GetUniqueKey")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roCalendarRowPeriodDataManager::GetUniqueKey")
            End Try

            Return strRet
        End Function

        Public Shared Function AddShiftRequetsToCalendar(ByRef oCalendar As roCalendar, ByVal _FirstDay As DateTime, ByVal _LastDay As DateTime, ByRef _State As roCalendarRowPeriodDataState) As Boolean
            '
            ' Añadimos los colores de las horarios solicitados pendientes de aprobar en el calendario
            '

            Dim bolRet As Boolean = False
            Dim bContinueChecking As Boolean = True

            Try

                Dim oRows() As DataRow = Nothing

                ' Añadimos los fichajes a cada empleado/dia que corresponda del calendario
                For Each oCalendarRow As roCalendarRow In oCalendar.CalendarData
                    If oCalendarRow.EmployeeData IsNot Nothing AndAlso oCalendarRow.EmployeeData.IDEmployee > 0 Then

                        Dim dtEmpRequests As DataTable = roCalendarRowPeriodDataManager.GetCalendarRequestsByEmployee(oCalendarRow.EmployeeData.IDEmployee, _FirstDay, _LastDay, _State)

                        If dtEmpRequests IsNot Nothing AndAlso oCalendarRow.PeriodData IsNot Nothing AndAlso oCalendarRow.PeriodData.DayData IsNot Nothing Then
                            For Each oPeriodDayData As roCalendarRowDayData In oCalendarRow.PeriodData.DayData

                            Next
                        End If
                    End If
                Next

                bolRet = True
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roCalendarManager::AddShiftRequetsToCalendar")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roCalendarManager::AddShiftRequetsToCalendar")
            Finally

            End Try

            Return bolRet
        End Function

        ''' <summary>
        ''' Devuelve un Datatable con todos las solicitudes de un solo empleado que se pueden mostrar en el calendario
        ''' </summary>
        ''' <param name="_IDEmployee">ID de empleado a recuperar las solicitudes</param>
        ''' <param name="_SQLFilter">Filtro SQL para el Where (ejemplo: 'RequestType = 1 And Reque...')</param>
        ''' <param name="_SQLOrder">Ordenación SQL (ejemplo: 'RequestType ASC' o 'RequestDate ASC')</param>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function GetCalendarRequestsByEmployee(ByVal _IDEmployee As Integer, ByVal _FirstDay As DateTime, ByVal _LastDay As DateTime, ByVal _State As roCalendarRowPeriodDataState) As System.Data.DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# dt as RequestedDate, Requests.ID, Requests.IDEmployee, Requests.IDEmployeeExchange, Requests.RequestType, " &
                                        " case when dt = Requests.Date1 then Requests.IDShift " &
                                        " Else (@SELECT# IDShift1 from DailySchedule where DailySchedule.IDEmployee = Requests.IDEmployeeExchange And DailySchedule.Date = dt) End AS IDRequestShift, " &
                                        " Case when dt = Requests.Date1 then (@SELECT# Color from Shifts where ID = Requests.IDShift) when RequestType = 6 then (@SELECT# Color FROM Shifts WHERE ID = Requests.IDShift)" &
                                        " Else (@SELECT# Color from Shifts where ID = (@SELECT# IDShift1 from DailySchedule where DailySchedule.IDEmployee = Requests.IDEmployeeExchange And DailySchedule.Date = dt)) End AS IDRequestShiftColor, " &
                                        " Case when dt = Requests.Date1 then Requests.Field4 " &
                                        " Else (@SELECT# IDShift1 from DailySchedule where DailySchedule.IDEmployee = Requests.IDEmployee And DailySchedule.Date = dt) End AS IDSourceShift, " &
                                        " Case when dt = Requests.Date1 then (@SELECT# Color from Shifts where ID = Requests.Field4) " &
                                        " Else (@SELECT# Color from Shifts where ID = (@SELECT# IDShift1 from DailySchedule where DailySchedule.IDEmployee = Requests.IDEmployee And DailySchedule.Date = dt)) End AS IDSourceShiftColor " &
                                        " from dbo.alldays(" & roTypes.Any2Time(_FirstDay).SQLDateTime & "," & roTypes.Any2Time(_LastDay).SQLDateTime & ") left join Requests " &
                                        " ON ((dt between Requests.Date1 And isnull(Requests.Date2, Requests.Date1) And ((RequestType =5 Or RequestType = 7 or RequestType = 9 or RequestType = 14) Or ((RequestType = 6 Or RequestType = 13) And exists( @SELECT# * from sysroRequestDays where sysroRequestDays.IDRequest = Requests.ID And sysroRequestDays.Date = dt) ) )) " &
                                        " Or (dt = Requests.Date1 Or dt = Requests.Date2 And RequestType = 8)) " &
                                        " And Status in(0,1) And (Requests.IDEmployee = " & _IDEmployee & " Or Requests.IDEmployeeExchange = " & _IDEmployee & " )"

                oRet = CreateDataTable(strSQL)
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roCalendarRowPeriodDataManager::GetCalendarRequestsByEmployee")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roCalendarRowPeriodDataManager::GetCalendarRequestsByEmployee")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function LoadShiftAdvancedParatemers(sParameters As String, ByVal _State As roCalendarRowPeriodDataState) As roShiftAdvParameters()
            Dim oRet As New List(Of roShiftAdvParameters)
            Dim aTemp As String() = {}
            Dim aTemp2 As String() = {}
            Try
                If sParameters.Contains("[") AndAlso sParameters.Contains("]") Then
                    aTemp = sParameters.Replace(vbLf, "").Split("]")

                    For Each sParameter As String In aTemp
                        If sParameter.Trim.Contains("[") Then
                            aTemp2 = sParameter.Split("=")
                            If aTemp2.Count = 2 Then
                                oRet.Add(New roShiftAdvParameters(aTemp2(0).Trim, aTemp2(1).Replace("[", "").Trim))
                            End If
                        End If
                    Next
                End If
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roCalendarRowPeriodDataManager::LoadShiftAdvancedParatemers")
            End Try
            Return oRet.ToArray
        End Function

        Public Sub GetStarterShistNameAndColor(dStartTime As DateTime, dEndTime As DateTime, iDuration As Integer, ByRef sShiftName As String, ByRef iRGBColor As Integer, ByVal _State As roCalendarRowPeriodDataState)
            Dim sHex As String
            Try
                sShiftName = "U" & dStartTime.ToString("HHmm") & dEndTime.ToString("HHmm") & iDuration.ToString
                sHex = Conversion.Hex(dStartTime.ToString("mmHH") & dEndTime.ToString("mmHH") & iDuration.ToString).Substring(0, 6)
                iRGBColor = ColorTranslator.ToWin32(Drawing.ColorTranslator.FromHtml("#" & sHex))
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roCalendarRowPeriodDataManager::GetStarterShistNameAndColor")
            End Try
        End Sub

#End Region

    End Class

End Namespace