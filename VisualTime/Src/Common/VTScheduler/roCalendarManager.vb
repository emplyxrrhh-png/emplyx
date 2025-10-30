Imports System.Data.Common
Imports System.Drawing
Imports DocumentFormat.OpenXml.Office.Word
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Group
Imports Robotics.Base.VTBusiness.Shift
Imports Robotics.Base.VTBusiness.Zone
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTEmployees.Employee
Imports Robotics.Base.VTNotifications.Notifications
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports SpreadsheetLight

Namespace VTCalendar

    Public Class roCalendarManager
        Private oState As roCalendarState = Nothing
        Private oShiftCache As New Hashtable
        Private _oCalConfig As roCalendarPassportConfig = Nothing

        Private bShiftColorOnExport As Boolean = False
        Private bIncludeGroup As Boolean = False
        Private customization As String = String.Empty

        Public ReadOnly Property State As roCalendarState
            Get
                Return oState
            End Get
        End Property

        Public ReadOnly Property CalendarConfig As roCalendarPassportConfig
            Get
                If _oCalConfig Is Nothing Then _oCalConfig = GetCalendarPassportConfig()
                Return _oCalConfig
            End Get
        End Property

        Public Sub New()
            Me.oState = New roCalendarState()
        End Sub

        Public Sub New(ByVal _State As roCalendarState)
            Me.oState = _State
        End Sub

#Region "Methods"

        Public Function Load(ByVal xFirstDay As DateTime, ByVal xLastDay As DateTime, ByVal strEmployeeFilter As String, ByVal typeView As CalendarView, ByVal detailLevel As CalendarDetailLevel, ByVal bLoadChilds As Boolean, Optional ByVal bAudit As Boolean = False, Optional ByVal lstAssignments As String = "", Optional ByVal bolShiftData As Boolean = False, Optional ByVal bolShowIndictments As Boolean = False, Optional ByVal bolShowPunches As Boolean = False, Optional ByVal bolShowSeatingCapacity As Boolean = False) As roCalendar

            Dim bolRet As roCalendar = Nothing

            Try

                ' Validamos si tiene licencia de HRScheduling
                Dim oLicense As New roServerLicense
                Dim bolLicenseHRScheduling As Boolean = oLicense.FeatureIsInstalled("Feature\HRScheduling")

                bolRet = New roCalendar
                bolRet.FirstDay = xFirstDay
                bolRet.LastDay = xLastDay

                bolRet.FreezingDate = roParameters.GetFirstDate()

                ' Cargamos las filas de la cabezera del Calendario
                bolRet.CalendarHeader = GenerateHeaderCell(detailLevel, typeView, xFirstDay, xLastDay)

                ' Cargamos las filas del Calendario
                Dim oCalendarRowState As New roCalendarRowState(oState.IDPassport)

                Me._oCalConfig = Me.GetCalendarPassportConfig()

                bolRet.CalendarData = roCalendarRowManager.LoadRowsByCalendar(xFirstDay, xLastDay, strEmployeeFilter, oCalendarRowState, typeView, detailLevel, bolLicenseHRScheduling, Me.CalendarConfig, bLoadChilds, lstAssignments, bolShowSeatingCapacity)

                ' Cargamos los datos de las coberturas de HRScheduling, en caso de tener licencia y vista de planificacion
                If typeView = CalendarView.Planification AndAlso bolLicenseHRScheduling AndAlso strEmployeeFilter.Length > 0 Then
                    ' Solo mostramos datos de HRScheduling si han seleccionado un único grupo
                    If strEmployeeFilter.Count(Function(c As Char) c = "A") = 1 AndAlso strEmployeeFilter.Count(Function(c As Char) c = "B") = 0 Then
                        Dim oCalendarPeriodCoverageState As New roCalendarPeriodCoverageState(oState.IDPassport)
                        bolRet.CalendarHeader.PeriodCoverageData = roCalendarPeriodCoverageManager.LoadCoverageByCalendar(xFirstDay, xLastDay, strEmployeeFilter, oCalendarPeriodCoverageState, lstAssignments)
                    End If
                End If

                ' Cargamos los indicadores de planificacion en caso necesario
                If bolShowIndictments Then AddIndictmentsToCalendar(bolRet)

                ' Cargamos los fichajes de presencia en caso necesario
                If bolShowPunches Then AddPunchesToCalendar(bolRet, xFirstDay, xLastDay)

                ' Cargamos datos de aforos en caso ncesario
                If bolShowSeatingCapacity Then
                    bolRet.CalendarHeader.PeriodSeatingCapacityData = LoadCapacitySeating(bolRet, Me.oState)
                Else
                    bolRet.CalendarHeader.PeriodSeatingCapacityData = {}
                End If

                ' Cargamos los datos de los horarios mostrados en el calendario que tengan complementarias o puestos
                If bolShiftData Then
                    Dim oCalendarPeriodCoverageState As New roCalendarPeriodCoverageState(oState.IDPassport)
                    bolRet.CalendarShift = roCalendarRowManager.LoadShiftDataByCalendar(bolRet.CalendarData, oCalendarRowState)
                End If

                ' Auditar lectura
                If bAudit AndAlso bolRet IsNot Nothing Then
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{Name}", bolRet.FirstDay, "", 1)
                    Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tCalendar, bolRet.FirstDay, tbParameters, -1)
                End If
            Catch ex As Data.Common.DbException

                oState.UpdateStateInfo(ex, "roCalendarManager::Load")
            Catch ex As Exception

                oState.UpdateStateInfo(ex, "roCalendarManager::Load")
            End Try

            Return bolRet

        End Function

        Public Function SaveFromImport(ByRef oCalendar As roCalendar, Optional ByVal bAudit As Boolean = False, Optional ByVal bolCheckPermission As Boolean = False, Optional bolReplaceActualBudget As Boolean = False) As roCalendarResult
            Dim bolRet As Boolean = False
            Dim oRet As New roCalendarResult
            Dim oCalendarResultDays As New Generic.List(Of roCalendarDataDayError)

            Try
                oRet.Status = CalendarStatusEnum.OK
                oRet.CalendarDataResult = Array.Empty(Of roCalendarDataDayError)()

                Me.oState.Result = CalendarV2ResultEnum.NoError
                oRet.Status = CalendarStatusEnum.KO

                ' Validamos si tiene licencia de HRScheduling
                Dim oLicense As New roServerLicense
                Dim bolLicenseHRScheduling As Boolean = oLicense.FeatureIsInstalled("Feature\HRScheduling")

                Me.Validate(oCalendar, oCalendarResultDays, bolLicenseHRScheduling, bolCheckPermission)
                Dim tmpLst = oRet.CalendarDataResult.ToList
                tmpLst.AddRange(oCalendarResultDays)
                oRet.CalendarDataResult = tmpLst.ToArray

                'If bolRet Then

                ' Verificamos si hay que revisar la notificacion de Asignacion de horario
                Dim oNotificationState As New roNotificationState(-1)
                Dim oAssignedShiftNotifications As Generic.List(Of roNotification) = roNotification.GetNotifications("IDType = 51 And Activated=1", oNotificationState, , True)

                bolRet = True
                oRet.Status = CalendarStatusEnum.OK

                Dim initScheduleTask As Boolean = True
                Dim oCalendarRowState As New roCalendarRowPeriodDataState(oState.IDPassport)
                Dim oCalendarRowPeriodData As New roCalendarRowPeriodDataManager(oCalendarRowState)

                ' Si los datos son correctos los guardamos
                For Each oCalendarRow As roCalendarRow In oCalendar.CalendarData
                    ' Para cada fila, guardamos las celdas que se hayan modificado
                    If oCalendarRow.EmployeeData IsNot Nothing AndAlso oCalendarRow.EmployeeData.IDEmployee > 0 Then
                        If oCalendarRow.PeriodData IsNot Nothing Then
                            bolRet = oCalendarRowPeriodData.Save(oCalendarRow.EmployeeData, oCalendarRow.PeriodData, oCalendarResultDays, bolLicenseHRScheduling, True, False, False,,, oAssignedShiftNotifications, bolReplaceActualBudget)

                            If initScheduleTask Then initScheduleTask = bolRet

                            If Not bolRet Then
                                If oRet.Status = CalendarStatusEnum.OK Then oRet.Status = CalendarStatusEnum.KO

                                tmpLst = oRet.CalendarDataResult.ToList
                                tmpLst.AddRange(oCalendarResultDays)
                                oRet.CalendarDataResult = tmpLst.ToArray

                            End If
                        End If
                    End If
                Next

                ' Lanzamos la tarea
                If bolRet Then
                    roConnector.InitTask(TasksType.DAILYSCHEDULE)
                End If

                If bolRet And bAudit Then
                    ' Auditamos
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Me.oState.Audit(Audit.Action.aUpdate, Audit.ObjectType.tCalendar, "", tbParameters, -1)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roCalendarManager::Save")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarManager::Save")
            Finally

            End Try

            Return oRet

        End Function

        Public Function Save(ByRef oCalendar As roCalendar, Optional ByVal bAudit As Boolean = False, Optional ByVal bolCheckPermission As Boolean = False, Optional bolReplaceActualBudget As Boolean = False) As roCalendarResult
            Dim bolRet As Boolean = False
            Dim oRet As New roCalendarResult
            Dim oCalendarResultDays As New Generic.List(Of roCalendarDataDayError)

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Me.oState.Result = CalendarV2ResultEnum.NoError
                oRet.Status = CalendarStatusEnum.KO

                ' Validamos si tiene licencia de HRScheduling
                Dim oLicense As New roServerLicense
                Dim bolLicenseHRScheduling As Boolean = oLicense.FeatureIsInstalled("Feature\HRScheduling")

                bolRet = Me.Validate(oCalendar, oCalendarResultDays, bolLicenseHRScheduling, bolCheckPermission)

                If bolRet Then

                    ' Verificamos si hay que revisar la notificacion de Asignacion de horario
                    Dim oNotificationState As New roNotificationState(-1)
                    Dim oAssignedShiftNotifications As Generic.List(Of roNotification) = roNotification.GetNotifications("IDType = 51 And Activated=1", oNotificationState,, True)

                    bolRet = True
                    oRet.Status = CalendarStatusEnum.OK

                    Dim oCalendarRowState As New roCalendarRowPeriodDataState(oState.IDPassport)
                    roBusinessState.CopyTo(oState, oCalendarRowState)
                    Dim oCalendarRowPeriodData As New roCalendarRowPeriodDataManager(oCalendarRowState)

                    ' Si los datos son correctos los guardamos
                    For Each oCalendarRow As roCalendarRow In oCalendar.CalendarData
                        ' Para cada fila, guardamos las celdas que se hayan modificado
                        If oCalendarRow.EmployeeData IsNot Nothing AndAlso oCalendarRow.EmployeeData.IDEmployee > 0 Then
                            If oCalendarRow.PeriodData IsNot Nothing Then
                                bolRet = oCalendarRowPeriodData.Save(oCalendarRow.EmployeeData, oCalendarRow.PeriodData, oCalendarResultDays, bolLicenseHRScheduling, True, False, False,,, oAssignedShiftNotifications, bolReplaceActualBudget)
                                If Not bolRet Then
                                    oRet.Status = CalendarStatusEnum.KO
                                    Exit For
                                End If
                            End If
                        End If
                    Next

                    ' Lanzamos la tarea
                    If bolRet Then
                        roConnector.InitTask(TasksType.DAILYSCHEDULE)
                    End If
                End If

                If bolRet And bAudit Then
                    ' Auditamos
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Me.oState.Audit(Audit.Action.aUpdate, Audit.ObjectType.tCalendar, "", tbParameters, -1)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roCalendarManager::Save")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarManager::Save")
            Finally

                If Not bolRet Then
                    oRet.Status = CalendarStatusEnum.KO
                    oRet.CalendarDataResult = oCalendarResultDays.ToArray
                End If
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return oRet

        End Function

        Public Function AddCalendarRowDayData(ByRef oCalendar As roCalendar,
                                              ByVal oNewCalendarRowDayData As roCalendarRowDayData,
                                              ByVal intIDEmployee As Integer,
                                              ByVal bolCopyMainShifts As Boolean,
                                              ByVal bolCopyHolidays As Boolean,
                                              ByVal bolCopyAlternatives As Boolean,
                                              ByVal bolKeepHolidays As Boolean,
                                              ByVal bolKeepLockedDay As Boolean,
                                              ByVal bolLockDestDays As Boolean,
                                              ByVal bolCopyAssignment As Boolean,
                                              ByVal bolCopyFeastDays As Boolean,
                                              ByVal bolCopyBudgetPosition As Boolean,
                                              Optional bolCopyTelecommute As Boolean = False) As Boolean
            Dim bolRet As Boolean = False
            Dim bContinueChecking As Boolean = True

            Try

                Me.oState.Result = CalendarV2ResultEnum.RowDayData_EmployeeNotExist

                If oCalendar Is Nothing Or oNewCalendarRowDayData Is Nothing Or intIDEmployee <= 0 Then
                    Return bolRet
                    Exit Function
                End If

                If oCalendar.CalendarData Is Nothing Then
                    Return bolRet
                    Exit Function
                End If

                If oNewCalendarRowDayData.MainShift IsNot Nothing Then oNewCalendarRowDayData.IsHoliday = (oNewCalendarRowDayData.MainShift.Type = ShiftTypeEnum.Holiday_NoWorking OrElse oNewCalendarRowDayData.MainShift.Type = ShiftTypeEnum.Holiday_Working)

                Dim oCalendarRowState As New roCalendarRowPeriodDataState(oState.IDPassport)
                Dim oCalendarRowPeriodData As New roCalendarRowPeriodDataManager(oCalendarRowState)

                ' Buscamos el dia a modificar del empleado indicado
                For Each oExistingCalendarRow As roCalendarRow In oCalendar.CalendarData
                    If oExistingCalendarRow.EmployeeData IsNot Nothing AndAlso oExistingCalendarRow.EmployeeData.IDEmployee = intIDEmployee Then

                        Me.oState.Result = CalendarV2ResultEnum.RowDayData_PlannedDayNotExist

                        If oExistingCalendarRow.PeriodData IsNot Nothing AndAlso oExistingCalendarRow.PeriodData.DayData IsNot Nothing Then
                            For Each oExistingCalendarRowDayData As roCalendarRowDayData In oExistingCalendarRow.PeriodData.DayData
                                If oExistingCalendarRowDayData.PlanDate = oNewCalendarRowDayData.PlanDate Then
                                    ' Hemos encontrado el dia a modificar
                                    Dim bolCanModifyDay As Boolean = True
                                    Dim bolModified As Boolean = False

                                    Dim oBeforeMainShift As New roCalendarRowShiftData
                                    oBeforeMainShift = oExistingCalendarRowDayData.MainShift

                                    Dim bolBeforeIsLocked As Boolean = oExistingCalendarRowDayData.Locked

                                    Dim oBeforeAssignmentData As New roCalendarAssignmentCellData
                                    oBeforeAssignmentData = oExistingCalendarRowDayData.AssigData

                                    Dim lngBeforeIDDailyBudgetPosition As Long = oExistingCalendarRowDayData.IDDailyBudgetPosition

                                    Select Case oExistingCalendarRowDayData.EmployeeStatusOnDay
                                        Case EmployeeStatusOnDayEnum.InOtherDepartment
                                            'Ignorar. Llegará el registro en el que si se pueda planificar
                                            Me.oState.Result = CalendarV2ResultEnum.NoError
                                            bolRet = False
                                            Exit For
                                        Case EmployeeStatusOnDayEnum.NoContract
                                            'Sin contrato
                                            Me.oState.Result = CalendarV2ResultEnum.RowDayData_EmployeeWithoutContractOnDate
                                            bolRet = False
                                            Exit For
                                        Case EmployeeStatusOnDayEnum.Ok
                                            'Proceder
                                    End Select

                                    ' Miro si por permisos ese día se puede modificar
                                    If oExistingCalendarRow.EmployeeData.Permission > 3 Then
                                        ' Verificamos si podemos modificar un dia bloqueado
                                        If bolKeepLockedDay And oExistingCalendarRowDayData.Locked Then bolCanModifyDay = False
                                        If bolCanModifyDay Then
                                            bContinueChecking = True
                                            ' Tengo Vacaciones, y vienen Vacaciones
                                            If oExistingCalendarRowDayData.IsHoliday AndAlso oNewCalendarRowDayData.IsHoliday Then
                                                If Not bolKeepHolidays AndAlso bolCopyHolidays Then oExistingCalendarRowDayData.MainShift = oNewCalendarRowDayData.MainShift : bolModified = True
                                                If bolCopyMainShifts AndAlso Not oNewCalendarRowDayData.ShiftBase Is Nothing Then oExistingCalendarRowDayData.ShiftBase = oNewCalendarRowDayData.ShiftBase : bolModified = True
                                                bContinueChecking = False
                                            End If

                                            ' Tengo Vacaciones, y viene uno Normal o flotante
                                            If oExistingCalendarRowDayData.IsHoliday AndAlso Not oNewCalendarRowDayData.IsHoliday AndAlso bContinueChecking Then
                                                If bolKeepHolidays Then
                                                    If bolCopyMainShifts AndAlso Not oNewCalendarRowDayData.MainShift Is Nothing Then oExistingCalendarRowDayData.ShiftBase = oNewCalendarRowDayData.MainShift : bolModified = True
                                                Else
                                                    If bolCopyMainShifts AndAlso Not oNewCalendarRowDayData.MainShift Is Nothing Then
                                                        oExistingCalendarRowDayData.MainShift = oNewCalendarRowDayData.MainShift
                                                        oExistingCalendarRowDayData.ShiftBase = Nothing
                                                        oExistingCalendarRowDayData.IsHoliday = False
                                                        bolModified = True
                                                    End If
                                                End If
                                                bContinueChecking = False
                                            End If

                                            ' Tengo  Normal o flotante, y viene Vacaciones
                                            If Not oExistingCalendarRowDayData.IsHoliday AndAlso oNewCalendarRowDayData.IsHoliday AndAlso bContinueChecking Then
                                                If bolCopyHolidays Then
                                                    If bolCopyMainShifts AndAlso Not oNewCalendarRowDayData.ShiftBase Is Nothing Then
                                                        oExistingCalendarRowDayData.ShiftBase = oNewCalendarRowDayData.ShiftBase
                                                    Else
                                                        If oExistingCalendarRowDayData.MainShift Is Nothing Then
                                                            ' No puedo poner vacaciones si no se define un base de alguna manera ...
                                                            Me.oState.Result = CalendarV2ResultEnum.RowDayData_ShiftBaseRequired
                                                            Exit For
                                                        Else
                                                            oExistingCalendarRowDayData.ShiftBase = oExistingCalendarRowDayData.MainShift
                                                        End If
                                                    End If
                                                    oExistingCalendarRowDayData.MainShift = oNewCalendarRowDayData.MainShift
                                                    oExistingCalendarRowDayData.IsHoliday = True
                                                    bolModified = True
                                                Else
                                                    If bolCopyMainShifts AndAlso Not oNewCalendarRowDayData.ShiftBase Is Nothing Then
                                                        oExistingCalendarRowDayData.MainShift = oNewCalendarRowDayData.ShiftBase
                                                        bolModified = True
                                                    End If
                                                End If
                                                bContinueChecking = False
                                            End If

                                            ' Tengo  Normal o flotante, y viene Normal o flotante
                                            If bolCopyMainShifts AndAlso Not oExistingCalendarRowDayData.IsHoliday AndAlso Not oNewCalendarRowDayData.IsHoliday AndAlso Not oNewCalendarRowDayData.MainShift Is Nothing AndAlso bContinueChecking Then
                                                oExistingCalendarRowDayData.MainShift = oNewCalendarRowDayData.MainShift
                                                bolModified = True
                                            End If

                                            ' Validaciones finales de casos especiales
                                            If bolModified Then
                                                ' Si quedan vacaciones de tipo laborable sobre horario no laborable, error ...
                                                If oExistingCalendarRowDayData.IsHoliday AndAlso oExistingCalendarRowDayData.MainShift.Type = ShiftTypeEnum.Holiday_Working AndAlso oExistingCalendarRowDayData.ShiftBase.PlannedHours = 0 Then
                                                    Me.oState.Result = CalendarV2ResultEnum.RowDayData_ShiftBaseShouldBeWorking
                                                    bolRet = False
                                                    Exit For
                                                End If
                                            End If

                                            ' Finalizo
                                            If bolModified Then
                                                ' En caso necesario bloqueo el dia
                                                If bolLockDestDays Then oExistingCalendarRowDayData.Locked = True

                                                If Not oExistingCalendarRowDayData.Alerts Is Nothing Then
                                                    oExistingCalendarRowDayData.Alerts.OnHolidays = oExistingCalendarRowDayData.IsHoliday
                                                Else
                                                    oExistingCalendarRowDayData.Alerts = New roCalendarRowDayAlerts
                                                End If
                                            End If

                                            If oExistingCalendarRowDayData.IsHoliday AndAlso bolModified Then oExistingCalendarRowDayData.MainShift.StartHour = oExistingCalendarRowDayData.ShiftBase.StartHour
                                            If oExistingCalendarRowDayData.PlanDate <= Now.Date.Date Then oExistingCalendarRowDayData.ShiftUsed = oExistingCalendarRowDayData.MainShift

                                            ' Copiamos los horarios alternativos, en caso necesario
                                            If bolCopyAlternatives Then
                                                oExistingCalendarRowDayData.AltShift1 = oNewCalendarRowDayData.AltShift1
                                                oExistingCalendarRowDayData.AltShift2 = oNewCalendarRowDayData.AltShift2
                                                oExistingCalendarRowDayData.AltShift3 = oNewCalendarRowDayData.AltShift3
                                                bolModified = True
                                            End If

                                            ' Copiamos puesto en caso necesario
                                            If bolCopyAssignment Then
                                                oExistingCalendarRowDayData.AssigData = oNewCalendarRowDayData.AssigData
                                                bolModified = True
                                            End If

                                            ' Marcamos el dia como festivo en caso necesario
                                            If bolCopyFeastDays Then
                                                oExistingCalendarRowDayData.Feast = oNewCalendarRowDayData.Feast
                                                bolModified = True
                                            End If

                                            ' Si se asigna un puesto concreto, validamos si se puede asignar
                                            If Not oExistingCalendarRowDayData.AssigData Is Nothing AndAlso oExistingCalendarRowDayData.AssigData.ID > 0 Then
                                                If Not ValidateDayAssignmentData(oExistingCalendarRow.EmployeeData, oExistingCalendarRowDayData) Then
                                                    bolModified = False
                                                    Me.oState.Result = CalendarV2ResultEnum.RowDayData_AssignmentDataInvalid
                                                    bolRet = False
                                                    Exit For
                                                End If
                                            End If

                                            If bolCopyBudgetPosition Then
                                                ' Si se ha modificado la asignacion a una posicion del presupuesto
                                                ' a traves del asistente, ya sea añadiendo o quitandolo de la posicion
                                                ' Si previamente no tenia presupuesto y esta la fecha congelada, no se puede modificar el horario
                                                If lngBeforeIDDailyBudgetPosition = 0 And oNewCalendarRowDayData IsNot Nothing AndAlso oNewCalendarRowDayData.IDDailyBudgetPosition > 0 Then
                                                    If bolBeforeIsLocked And oBeforeMainShift IsNot Nothing AndAlso oExistingCalendarRowDayData.MainShift IsNot Nothing AndAlso oBeforeMainShift.ID <> oExistingCalendarRowDayData.MainShift.ID Then
                                                        bolModified = False
                                                        Me.oState.Result = CalendarV2ResultEnum.RowDayData_AssignmentDataInvalid
                                                        bolRet = False
                                                        Exit For
                                                    End If
                                                End If

                                                If lngBeforeIDDailyBudgetPosition <> oNewCalendarRowDayData.IDDailyBudgetPosition Then
                                                    oExistingCalendarRowDayData.IDDailyBudgetPosition = oNewCalendarRowDayData.IDDailyBudgetPosition
                                                    bolModified = True
                                                End If
                                            Else
                                                If bolModified And lngBeforeIDDailyBudgetPosition > 0 Then
                                                    ' No dejamos modificar la planificacion si esta asignado a un presupuesto previamente
                                                    bolModified = False
                                                    Me.oState.Result = CalendarV2ResultEnum.RowDayData_AssignmentDataInvalid
                                                    bolRet = False
                                                    Exit For
                                                End If
                                            End If

                                            ' Si el día no está bloqueado reviso teletrabajo
                                            If bolCopyTelecommute Then
                                                oExistingCalendarRowDayData.TelecommuteForced = oNewCalendarRowDayData.TelecommuteForced
                                                oExistingCalendarRowDayData.TelecommutingExpected = oNewCalendarRowDayData.TelecommutingExpected
                                                oExistingCalendarRowDayData.TelecommutingOptional = oNewCalendarRowDayData.TelecommutingOptional
                                                bolModified = True
                                            End If
                                        End If
                                    Else
                                        bolModified = False
                                        Me.oState.Result = CalendarV2ResultEnum.RowDayData_PermissionDeniedOverEmployee
                                        bolRet = False
                                        Exit For
                                    End If
                                    If bolModified Then oExistingCalendarRowDayData.HasChanged = True
                                    Me.oState.Result = CalendarV2ResultEnum.NoError
                                    bolRet = True
                                    Exit For
                                End If
                            Next
                        End If
                    End If
                Next
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roCalendarManager::AddCalendarRowDayData")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarManager::AddCalendarRowDayData")
            Finally

            End Try

            Return bolRet

        End Function

        Public Function Delete(ByVal oCalendar As roCalendar, Optional ByVal bAudit As Boolean = False) As Boolean

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
                    bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tCalendar, oCalendar.FirstDay, Nothing, -1)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roCalendarManager::Delete")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarManager::Delete")
            End Try

            Return bolRet

        End Function

        Public Function Validate(ByVal oCalendar As roCalendar, ByRef oCalendarResultDays As Generic.List(Of roCalendarDataDayError), ByVal bolLicenseHRScheduling As Boolean, Optional ByVal bolCheckPermission As Boolean = False) As Boolean
            Dim bolRet As Boolean = True

            Try

                Me.oState.Result = CalendarV2ResultEnum.NoError

                If oCalendar.CalendarData IsNot Nothing Then
                    Dim oParameters As roParameters
                    oParameters = New roParameters("OPTIONS", True)

                    Dim oCalendarRowState As New roCalendarRowPeriodDataState(oState.IDPassport)
                    Dim oCalendarRowPeriodData As New roCalendarRowPeriodDataManager(oCalendarRowState)

                    Dim strMsg As String = ""
                    For Each oCalendarRow As roCalendarRow In oCalendar.CalendarData
                        ' Para cada fila, validamos si los datos son correctos
                        If oCalendarRow.EmployeeData IsNot Nothing AndAlso oCalendarRow.EmployeeData.IDEmployee > 0 Then
                            If oCalendarRow.PeriodData IsNot Nothing Then
                                bolRet = oCalendarRowPeriodData.Validate(oCalendarRow.EmployeeData, oCalendarRow.PeriodData, strMsg, oParameters, oCalendarResultDays, bolLicenseHRScheduling, bolCheckPermission)
                                If Not bolRet Then
                                    Me.oState.Result = CalendarV2ResultEnum.InValidData
                                    Me.oState.ErrorDetail &= strMsg
                                End If
                            End If
                        End If
                    Next

                    ' Si hay algun valor erroneo, no seguimos
                    If Me.oState.Result <> CalendarV2ResultEnum.NoError Then
                        bolRet = False
                    End If
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roCalendarManager::Validate")
                bolRet = False
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarManager::Validate")
                bolRet = False
            Finally

            End Try

            Return bolRet

        End Function

        Private Function GenerateHeaderCell(ByVal detailLevel As CalendarDetailLevel, ByVal typeView As CalendarView, ByVal xStartDate As DateTime, ByVal xFinishDate As DateTime) As roCalendarHeader
            Dim oCalendarHeader As New roCalendarHeader
            Dim tmpFirtsDay As DateTime = xStartDate
            Dim tmpEndDay As DateTime = xFinishDate

            oCalendarHeader.EmployeeHeaderData = New roCalendarHeaderCell
            oCalendarHeader.EmployeeHeaderData.Row1Text = Me.oState.Language.Translate("Calendar.Header.Employees", "") 'Empleados
            oCalendarHeader.EmployeeHeaderData.Row2Text = Me.oState.Language.Translate("Calendar.Header.Assignments", "") 'Puestos
            oCalendarHeader.EmployeeHeaderData.BackColor = ""

            oCalendarHeader.SummaryHeaderData = New roCalendarHeaderCell
            oCalendarHeader.SummaryHeaderData.Row1Text = Me.oState.Language.Translate("Calendar.Header.Concept", "") 'Saldo
            oCalendarHeader.SummaryHeaderData.Row2Text = Me.oState.Language.Translate("Calendar.Header.Status", "") 'Estado
            oCalendarHeader.SummaryHeaderData.BackColor = ""

            Dim tmplist As New Generic.List(Of roCalendarHeaderCell)

            'Vista multiples dias
            If detailLevel = CalendarDetailLevel.Daily OrElse typeView = CalendarView.Review Then
                While tmpFirtsDay <= tmpEndDay

                    Dim tmpCell As New roCalendarHeaderCell
                    tmpCell.Row1Text = tmpFirtsDay.ToString("dddd", oState.Language.GetLanguageCulture)
                    tmpCell.Row2Text = tmpFirtsDay.ToString(Me.State.Language.GetLanguageCulture.DateTimeFormat.ShortDatePattern, Me.State.Language.GetLanguageCulture)

                    oState.Language.GetMonthAndDayDateFormat()
                    Dim existsAlert As Boolean = False

                    If Not existsAlert Then
                        If tmpFirtsDay.Date = DateTime.Now.Date Then
                            tmpCell.BackColor = "#44C57E" ' Verd suau
                        Else
                            If tmpFirtsDay.DayOfWeek = DayOfWeek.Sunday Then
                                tmpCell.BackColor = "#C0DEFD" 'Blau suau
                            Else
                                tmpCell.BackColor = "#F2F4F2" 'Blanc
                            End If
                        End If
                    Else
                        tmpCell.BackColor = "#FF9595" 'Vermell suau
                    End If

                    tmplist.Add(tmpCell)
                    tmpFirtsDay = tmpFirtsDay.AddDays(1)
                End While
                oCalendarHeader.PeriodHeaderData = tmplist.ToArray
            Else
                'Vista diaria
                tmpFirtsDay = tmpFirtsDay.AddDays(-1)
                tmpEndDay = tmpEndDay.AddDays(2)

                Dim xMinutes As Integer = 30
                If detailLevel = CalendarDetailLevel.Detail_15 Then xMinutes = 15
                If detailLevel = CalendarDetailLevel.Detail_30 Then xMinutes = 30
                If detailLevel = CalendarDetailLevel.Detail_60 Then xMinutes = 60

                While tmpFirtsDay < tmpEndDay
                    Dim tmpCell As New roCalendarHeaderCell
                    tmpCell.Row1Text = tmpFirtsDay.ToString("dd/MM/yyyy")
                    tmpCell.Row2Text = tmpFirtsDay.ToString("HH:mm")

                    Dim existsAlert As Boolean = False

                    If Not existsAlert Then
                        If DateTime.Now >= tmpFirtsDay AndAlso DateTime.Now < tmpFirtsDay.AddMinutes(xMinutes) Then
                            tmpCell.BackColor = "#44C57E" ' Verd suau
                        Else
                            tmpCell.BackColor = "#F2F4F2" 'Blanc
                        End If
                    Else
                        tmpCell.BackColor = "#FF9595" 'Vermell suau
                    End If

                    tmplist.Add(tmpCell)
                    tmpFirtsDay = tmpFirtsDay.AddMinutes(xMinutes)
                End While
                oCalendarHeader.PeriodHeaderData = tmplist.ToArray
            End If

            Return oCalendarHeader
        End Function

        Public Function CopyPlanv2(ByVal oParameters As roCollection, ByRef oState As roCalendarState,
                                 Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False
            oState.UpdateStateInfo()

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                ' 0.Obtenemos los parametros necesarios
                ' Empleado origen
                Dim intOriginEmployeeID As Integer = roTypes.Any2Integer(oParameters("lstOriginEmployees"))

                ' Empleados destino
                Dim strDestinEmployeeID() As String = roTypes.Any2String(oParameters("lstDestEmployees")).Split(",")
                Dim iDestinEmployeeID As New Generic.List(Of Integer)
                For Each sID As String In strDestinEmployeeID
                    iDestinEmployeeID.Add(roTypes.Any2Integer(sID))
                Next

                Dim xBeginDateSource As Date = Date.Parse(oParameters("BeginDateSource"))
                Dim xEndDateSource As Date = Date.Parse(oParameters("EndDateSource"))
                Dim xFromDateDestination As Date = Date.Parse(oParameters("FromDateDestination"))

                ' Teletrabajo
                Dim bolCopySchedule As Boolean = True
                Dim bolCopyTelecommute As Boolean = False
                If oParameters.Exists("CopyTelecommute") Then
                    Select Case roTypes.Any2Integer(oParameters("CopyTelecommute"))
                        Case 0
                            ' Copio sólo planificación
                        Case 1
                            ' Copio planificación y teletrabajo (de momento no implementado)
                            bolCopyTelecommute = True
                        Case 2
                            ' Copio sólo teletrabajo
                            bolCopySchedule = False
                            bolCopyTelecommute = True
                    End Select
                End If

                ' 1.Obtenemos el calendario a copiar del empleado origen
                Dim oSourceCalendar As New roCalendar
                oSourceCalendar = Load(xBeginDateSource, xEndDateSource, "B" & intOriginEmployeeID.ToString, CalendarView.Planification, CalendarDetailLevel.Daily, False)

                ' Para cada empleado destino
                For Each intDestinEmployeeID As Integer In iDestinEmployeeID
                    ' 2.Calculamos el periodo  de fechas a tratar en el empleado destino y las fechas en las que se debe aplicar el pegado
                    Dim xBeginDateDestination As Date
                    Dim xEndDateDestination As Date
                    Dim xApplyDays As New roCollection
                    SetPeriodDestinationFromParameters(xBeginDateDestination, xEndDateDestination, oParameters, xApplyDays, intDestinEmployeeID, bolCopyTelecommute)

                    ' 3.Nos guadamos los parametros de dias bloqueados/vacaciones/datos a copiar
                    Dim intBlockedMode As Integer = roTypes.Any2Integer(oParameters("BlockedMode"))
                    Dim bolKeepLockedDay As Boolean = IIf(intBlockedMode = 0, True, False)
                    Dim intHolidaysMode As Integer = roTypes.Any2Integer(oParameters("HolidaysMode"))
                    Dim bolKeepHolidays As Boolean = IIf(intHolidaysMode = 0, True, False)
                    Dim bolCopyMainShifts As Boolean = IIf(roTypes.Any2Integer(oParameters("CopyMainShifts")) = 1, True, False)
                    Dim bolCopyHolidays As Boolean = IIf(roTypes.Any2Integer(oParameters("CopyHolidays")) = 1, True, False)
                    Dim bolLockDestDays As Boolean = IIf(roTypes.Any2Integer(oParameters("LockDestDays")) = 1, True, False)
                    Dim bolCopyAssignment As Boolean = IIf(roTypes.Any2Integer(oParameters("CopyAssignment")) = 1, True, False)

                    ' 4.Obtenemmos el calendario del empleado destino de las fechas que tenemos que tratar
                    Dim oDestinationCalendar As New roCalendar
                    oDestinationCalendar = Load(xBeginDateDestination, xEndDateDestination, "B" & intDestinEmployeeID.ToString, CalendarView.Planification, CalendarDetailLevel.Daily, False)

                    ' 5.Aplicamos la copia en los dias que sean necesarios
                    If oDestinationCalendar IsNot Nothing AndAlso oDestinationCalendar.CalendarData IsNot Nothing Then
                        For Each oCalendarRowEmployeeData As roCalendarRow In oDestinationCalendar.CalendarData
                            If oCalendarRowEmployeeData IsNot Nothing AndAlso oCalendarRowEmployeeData.PeriodData IsNot Nothing Then
                                For Each oCalendarRowDayData As roCalendarRowDayData In oCalendarRowEmployeeData.PeriodData.DayData
                                    ' Para cada fecha del calendario del empleado destino comprobamos si hay que aplicar el pegado
                                    If xApplyDays.Exists(oCalendarRowDayData.PlanDate) Then ' OrElse (bolCopyTelecommute OrElse bolCopyOnlyTelecommute) Then

                                        Debug.Print("Pegar en el dia -->" & oCalendarRowDayData.PlanDate.ToString & " el horario en posicion " & xApplyDays.Item(oCalendarRowDayData.PlanDate))

                                        ' Obtenemos la celda origen a partir de la posicion indicada
                                        If oSourceCalendar IsNot Nothing AndAlso oSourceCalendar.CalendarData IsNot Nothing Then
                                            For Each oSourceCalendarRowEmployeeData As roCalendarRow In oSourceCalendar.CalendarData
                                                If oSourceCalendarRowEmployeeData IsNot Nothing AndAlso oSourceCalendarRowEmployeeData.PeriodData IsNot Nothing Then
                                                    For Each oSourceCalendarRowDayData As roCalendarRowDayData In oSourceCalendarRowEmployeeData.PeriodData.DayData
                                                        If oSourceCalendarRowDayData.PlanDate = roTypes.Any2Time(xBeginDateSource).Add(xApplyDays.Item(oCalendarRowDayData.PlanDate) - 1, "d").Value Then
                                                            ' Pegamos los datos del dia en la celda destino
                                                            Debug.Print("....Copiar los datos del dia  -->" & oSourceCalendarRowDayData.PlanDate.ToString)
                                                            Dim oNewCalendarRowDayData As New roCalendarRowDayData
                                                            If Not bolCopyTelecommute Then
                                                                ' Sólo copiamos horarios
                                                                oNewCalendarRowDayData.MainShift = oSourceCalendarRowDayData.MainShift
                                                                oNewCalendarRowDayData.AltShift1 = oSourceCalendarRowDayData.AltShift1
                                                                oNewCalendarRowDayData.AltShift2 = oSourceCalendarRowDayData.AltShift2
                                                                oNewCalendarRowDayData.AltShift3 = oSourceCalendarRowDayData.AltShift3
                                                                oNewCalendarRowDayData.IsHoliday = oSourceCalendarRowDayData.IsHoliday
                                                                oNewCalendarRowDayData.ShiftBase = oSourceCalendarRowDayData.ShiftBase
                                                                oNewCalendarRowDayData.ShiftUsed = oSourceCalendarRowDayData.ShiftUsed
                                                                oNewCalendarRowDayData.PlanDate = oCalendarRowDayData.PlanDate
                                                                oNewCalendarRowDayData.AssigData = oSourceCalendarRowDayData.AssigData
                                                            Else
                                                                ' Sólo copiamos teletrabajo
                                                                oNewCalendarRowDayData = oCalendarRowDayData
                                                                If oCalendarRowDayData.CanTelecommute Then
                                                                    oNewCalendarRowDayData.TelecommuteForced = True
                                                                    oNewCalendarRowDayData.TelecommutingExpected = oSourceCalendarRowDayData.TelecommutingExpected
                                                                    oNewCalendarRowDayData.TelecommutingOptional = oSourceCalendarRowDayData.TelecommutingOptional
                                                                End If
                                                            End If

                                                            AddCalendarRowDayData(oDestinationCalendar,
                                                                                  oNewCalendarRowDayData,
                                                                                  intDestinEmployeeID,
                                                                                  bolCopyMainShifts,
                                                                                  bolCopyHolidays,
                                                                                  False,
                                                                                  bolKeepHolidays,
                                                                                  bolKeepLockedDay,
                                                                                  bolLockDestDays,
                                                                                  bolCopyAssignment,
                                                                                  False,
                                                                                  False,
                                                                                  bolCopyTelecommute)
                                                        End If
                                                    Next
                                                End If
                                            Next
                                        End If
                                    End If
                                Next
                            End If
                        Next
                    End If

                    ' 5.Guardamos los datos
                    Dim oCalendarResult As roCalendarResult = Save(oDestinationCalendar, , True, True)
                    If oCalendarResult.Status = CalendarStatusEnum.OK Then
                        bolRet = True
                    Else
                        oState.Result = CalendarV2ResultEnum.InValidData
                        If oCalendarResult.CalendarDataResult IsNot Nothing Then
                            'Mostramos el primer error que tenga el resultado de guardar el calendario
                            oState.ErrorText = oCalendarResult.CalendarDataResult(0).ErrorText
                        End If
                        Exit For
                    End If

                Next
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roCalendarManager::CopyPlanv2")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCalendarManager::CopyPlanv2")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Function LoadShiftDataById(iShiftID As Integer) As roCalendarRowShiftData
            Dim oRet As roCalendarRowShiftData = Nothing
            Dim oShift As Robotics.Base.VTBusiness.Shift.roShift
            Dim auxColor As System.Drawing.Color = Color.White
            Try
                If iShiftID > 0 Then
                    oShift = New Base.VTBusiness.Shift.roShift()
                    oRet = New roCalendarRowShiftData
                    If Not oShiftCache.Contains(iShiftID) Then
                        oShift = New Base.VTBusiness.Shift.roShift(iShiftID, New Base.VTBusiness.Shift.roShiftState(oState.IDPassport))
                        oShiftCache.Add(iShiftID, oShift)
                    End If
                    oShift = oShiftCache(iShiftID)
                    oRet.ID = iShiftID
                    auxColor = System.Drawing.ColorTranslator.FromWin32(roTypes.Any2Integer(oShift.Color))
                    oRet.Color = roCalendarRowPeriodDataManager.HexConverter(auxColor)
                    oRet.Name = oShift.Name
                    oRet.PlannedHours = roTypes.Any2Time(roTypes.Any2Double(oShift.ExpectedWorkingHours)).Minutes
                    oRet.ShortName = oShift.ShortName
                    Select Case oShift.ShiftType
                        Case ShiftType.Normal
                            oRet.Type = ShiftTypeEnum.Normal
                            oRet.StartHour = roTypes.Any2Time(oShift.StartLimit).Value
                        Case ShiftType.NormalFloating
                            oRet.Type = ShiftTypeEnum.NormalFloating
                            oRet.StartHour = roTypes.Any2Time(oShift.StartFloating).Value
                        Case ShiftType.Vacations
                            oRet.Type = IIf(oShift.AreWorkingDays, ShiftTypeEnum.Holiday_Working, ShiftTypeEnum.Holiday_NoWorking)
                            oRet.StartHour = roTypes.Any2Time(oShift.StartLimit).Value
                    End Select

                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarManager::LoadShiftDataById")
            End Try
            Return oRet
        End Function

        Public Function LoadShiftDataByIdEx(iShiftID As Integer, Optional ForceLoadRigidDataLayers As Boolean = False) As roCalendarShift
            Dim oRet As roCalendarShift = Nothing
            Dim oShift As roCalendarShift = Nothing

            Try

                If iShiftID > 0 Then
                    If Not oShiftCache.Contains(iShiftID) Then
                        Dim oCalendarShiftManager As roCalendarShiftManager = New roCalendarShiftManager
                        oShift = New roCalendarShift
                        oShift = oCalendarShiftManager.GetShiftDefinition(iShiftID, ForceLoadRigidDataLayers)
                        oShiftCache.Add(iShiftID, oShift)
                    End If
                    oRet = oShiftCache(iShiftID)
                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarManager::LoadShiftDataByIdEx")
            Finally

            End Try
            Return oRet
        End Function

        Public Function LoadShiftDayDataByIdShift(iShiftID As Integer) As roCalendarRowShiftData
            Dim oRet As roCalendarRowShiftData = Nothing

            Try

                Dim oShiftDataDefinition As New roCalendarShift
                oShiftDataDefinition = LoadShiftDataByIdEx(iShiftID)
                oRet = New roCalendarRowShiftData
                oRet = MergeShiftData(oShiftDataDefinition, oRet)
            Catch ex As Exception
                'Do something
            Finally

            End Try
            Return oRet
        End Function

        Public Function LoadAssignmentDataById(iAssignmentID As Integer, ByVal iIDEmployee As Integer, ByVal iIDShift As Integer) As roCalendarAssignmentCellData
            Dim oRet As roCalendarAssignmentCellData = Nothing
            Dim otbl As DataTable = Nothing

            Try

                If iAssignmentID > 0 And iIDEmployee > 0 And iIDShift > 0 Then
                    otbl = VTBusiness.Assignment.roAssignment.GetEmployeeAndShiftAssignment(iIDEmployee, iIDShift, iAssignmentID)
                    If Not otbl Is Nothing AndAlso otbl.Rows.Count > 0 Then
                        oRet = New roCalendarAssignmentCellData
                        oRet.ID = roTypes.Any2Double(otbl.Rows(0)("ID"))
                        oRet.Color = roCalendarRowPeriodDataManager.HexConverter(System.Drawing.ColorTranslator.FromWin32(roTypes.Any2Integer(otbl.Rows(0)("Color"))))
                        oRet.Name = roTypes.Any2String(otbl.Rows(0)("Name"))
                        oRet.ShortName = roTypes.Any2String(otbl.Rows(0)("ShortName"))
                        oRet.Cover = roTypes.Any2Double(otbl.Rows(0)("Coverage"))
                    End If
                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarManager::LoadAssignmentDataById")
            Finally

            End Try
            Return oRet
        End Function

        Private Function SetPeriodDestinationFromParameters(ByRef xBeginDateDestination As Date, ByRef xEndDateDestination As Date, ByVal oParameters As roCollection, ByRef xApplyDays As roCollection, ByVal IDEmployeeDestination As Integer, ByVal bolCopyTelecommute As Boolean) As Boolean
            '
            ' Calculamos el periodo de fechas que necesitaremos cargar del empleado destino
            '

            Dim bolRet As Boolean = True

            Try

                ' Fijamos el inicio del periodo a partir de la fecha desde cuando se quiere pegar en el destino
                xBeginDateDestination = Date.Parse(oParameters("FromDateDestination"))

                ' Obtenemos el periodo de fechas que se quieren copiar del origne
                Dim xBeginDateSource As Date = Date.Parse(oParameters("BeginDateSource"))
                Dim xEndDateSource As Date = Date.Parse(oParameters("EndDateSource"))

                ' Calculamos el final del periodo
                Dim intRepeatMode As Integer = roTypes.Any2Integer(oParameters("RepeatMode"))
                Dim strRepeatModeValue As String = roTypes.Any2String(oParameters("RepeatModeValue"))
                Dim intRepeatStartMode As Integer = roTypes.Any2Integer(oParameters("RepeatStartMode"))
                Dim strRepeatStartModeValue As String = roTypes.Any2String(oParameters("RepeatStartModeValue"))
                Dim intRepeatSkipMode As Integer = roTypes.Any2Integer(oParameters("RepeatSkipMode"))
                Dim intRepeatSkipTimes As Integer = roTypes.Any2Integer(oParameters("RepeatSkipTimes"))
                Dim strRepeatSkipModeValue As String = roTypes.Any2String(oParameters("RepeatSkipModeValue"))

                Dim xActualDate As Date
                Dim intPeriodCopyDays As Integer = 0
                Dim intDaytoCopy As Integer = 0
                Dim intTimes As Integer = 0

                ' Nos guardamos el numero total de dias a copiar
                intPeriodCopyDays = DateDiff(DateInterval.Day, xBeginDateSource, xEndDateSource) + 1

                Dim xToDestinationDate As Date = Date.MaxValue

                If intRepeatMode = 0 Then
                    ' Numero de repeticiones
                    intTimes = roTypes.Any2Integer(strRepeatModeValue)
                Else
                    ' hasta una fecha concreta inclusive
                    xToDestinationDate = Date.Parse(strRepeatModeValue)

                    ' Calculamos el numero de dias del periodo a pegar
                    Dim intPeriodPasteDays As Integer = DateDiff(DateInterval.Day, xBeginDateDestination, xToDestinationDate) + 1

                    'Calculamos el numero de repeticiones
                    intTimes = Fix(intPeriodPasteDays / intPeriodCopyDays)
                    If intPeriodPasteDays Mod intPeriodCopyDays > 0 Then intTimes += 1
                End If

                ' Fecha inicial de pegado en el empleado destino
                xActualDate = xBeginDateDestination
                If EvaluateActualDate(xActualDate, IDEmployeeDestination, oParameters, intDaytoCopy, intPeriodCopyDays, intRepeatMode, xToDestinationDate, bolCopyTelecommute) Then
                    ' Nos guardamos la fecha en la que tenemos que pegar y la posicion de la fecha origen a copiar
                    xApplyDays.Add(xActualDate, intDaytoCopy)
                Else
                    ' Si la fecha es erronea, no hacemos nada y indicamos una fecha final invalida
                    intTimes = 0
                    xActualDate = xBeginDateDestination.AddDays(-1)
                End If

                For intRepeat As Integer = 1 To intTimes
                    ' Para cada repeticion

                    ' Revisar Modo Skip (cada x repeticiones saltar...)
                    If intRepeatSkipMode <> 0 And intRepeat > 1 AndAlso intRepeatSkipTimes > 0 AndAlso ((intRepeat - 1) Mod intRepeatSkipTimes = 0) Then
                        If intRepeatSkipMode = 1 Then
                            ' Siguiente dia de la semana
                            xActualDate = GetNextDayOfWeek(xActualDate, roTypes.Any2Integer(strRepeatSkipModeValue))
                        ElseIf intRepeatSkipMode = 2 Then
                            ' Dia del mes
                            xActualDate = GetNextDayofMonth(xActualDate, roTypes.Any2Integer(strRepeatSkipModeValue))
                        ElseIf intRepeatSkipMode = 3 Then
                            ' X dias
                            xActualDate = xActualDate.AddDays(strRepeatSkipModeValue)
                        End If
                    End If

                    ' Revisar cuando empieza la fecha de inicio de la actual repeticion,
                    ' siempre que no sea la primera
                    If intRepeat <> 1 Then
                        If intRepeatStartMode = 0 Then
                            ' Justo al terminar, vamos al siguiente dia
                            xActualDate = xActualDate.AddDays(1)
                        ElseIf intRepeatStartMode = 1 Then
                            ' Buscamos el siguinente [L,M,X..] de la semana
                            xActualDate = GetNextDayOfWeek(xActualDate, roTypes.Any2Integer(strRepeatStartModeValue))

                        ElseIf intRepeatStartMode = 2 Then
                            ' Buscamos el siguiente dia del mes
                            xActualDate = GetNextDayofMonth(xActualDate, roTypes.Any2Integer(strRepeatStartModeValue))
                        End If

                        If EvaluateActualDate(xActualDate, IDEmployeeDestination, oParameters, intDaytoCopy, intPeriodCopyDays, intRepeatMode, xToDestinationDate, bolCopyTelecommute) Then
                            xApplyDays.Add(xActualDate, intDaytoCopy)
                        Else
                            xActualDate = xActualDate.AddDays(-1)
                            Exit For
                        End If
                    End If

                    ' Obtenemos todos los dias siguientes hasta el ultimo dia de la actual repeticion, teniendo en cuenta el total de dias a copiar
                    For i As Integer = 1 To intPeriodCopyDays - 1
                        xActualDate = xActualDate.AddDays(1)
                        If EvaluateActualDate(xActualDate, IDEmployeeDestination, oParameters, intDaytoCopy, intPeriodCopyDays, intRepeatMode, xToDestinationDate, bolCopyTelecommute) Then
                            xApplyDays.Add(xActualDate, intDaytoCopy)
                        Else
                            xActualDate = xActualDate.AddDays(-1)
                            Exit For
                        End If
                    Next
                Next

                ' Nos guardamos el final del periodo
                xEndDateDestination = xActualDate
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roCalendarManager::SetPeriodDestinationFromParameters")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCalendarManager::SetPeriodDestinationFromParameters")
            Finally
            End Try

            Return bolRet

        End Function

        Private Function EvaluateActualDate(ByRef xActualDate As Date, ByVal IDEmployee As Integer, ByVal oParameters As roCollection, ByRef intDaytoCopy As Integer, ByVal intPeriodCopyDays As Integer, ByVal intRepeatMode As Integer, ByVal xToDestinationDate As Date, ByVal bolCopyOnlyTelecommute As Boolean) As Boolean
            '
            ' Evaluamos si la fecha es correcta o tenemos que buscar otra fecha posterior valida
            '
            Dim bolRet As Boolean = False

            Try

                ' Asignamos la posicion correcta de la fecha que debemos guardar
                intDaytoCopy += 1
                If intDaytoCopy > intPeriodCopyDays Then intDaytoCopy = 1

                ' Validamos la fecha de asignacion
                ' solo en el caso que se haya parametrizado un modo saltar y copiar al siguiente dia de vacaciones o dias bloqueados
                Dim intBlockedMode As Integer = roTypes.Any2Integer(oParameters("BlockedMode"))
                Dim intHolidaysMode As Integer = roTypes.Any2Integer(oParameters("HolidaysMode"))
                If intBlockedMode = 1 OrElse (intHolidaysMode = 1 AndAlso Not bolCopyOnlyTelecommute) Then
                    Dim bolInvalidDate As Boolean = True
                    While bolInvalidDate
                        Dim strSQL As String = "@SELECT# isnull(LockedDay,0) as LockedDay , isnull(IsHolidays,0) as  IsHolidays FROM DailySchedule WHERE IDEmployee=" & IDEmployee.ToString & " AND Date=" & roTypes.Any2Time(xActualDate).SQLSmallDateTime
                        Dim tb As DataTable = CreateDataTable(strSQL)
                        bolInvalidDate = False
                        If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                            Dim bolIsLockedDay As Boolean = roTypes.Any2Boolean(tb.Rows(0).Item("LockedDay"))
                            Dim bolIsHolidaysDay As Boolean = roTypes.Any2Boolean(tb.Rows(0).Item("IsHolidays"))
                            If bolIsLockedDay And intBlockedMode = 1 Then
                                ' Si el dia esta bloqueado y hay que saltar de dia
                                bolInvalidDate = True
                            ElseIf bolIsHolidaysDay AndAlso (intHolidaysMode = 1 AndAlso Not bolCopyOnlyTelecommute) Then
                                ' Si el dia es de vacaciones y hay que saltar de dia (sólo si estoy copiando planificación, y no teletrabajo)
                                bolInvalidDate = True
                            End If
                        End If
                        If bolInvalidDate Then xActualDate = xActualDate.AddDays(1)
                    End While
                End If

                If intRepeatMode = 1 And xToDestinationDate < xActualDate Then
                    ' Si la fecha actual es superior a la final, y estamos en ese modo  la fecha es incorrecta
                    bolRet = False
                Else
                    bolRet = True
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roCalendarManager::EvaluateActualDate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCalendarManager::EvaluateActualDate")
            Finally

            End Try

            Return bolRet

        End Function

        Private Function GetNextDayOfWeek(ByVal xActualDate As Date, ByVal NextDayofWeek As Integer) As Date
            Dim xRet As Date
            Dim intActualWeekDay As Integer = 0
            Try
                'Buscamos el siguiente XXX dia de la semana

                xActualDate = xActualDate.AddDays(1)
                For z As Integer = 1 To 7
                    If Weekday(xActualDate, FirstDayOfWeek.Sunday) = (NextDayofWeek + 1) Then
                        xRet = xActualDate
                        Exit For
                    End If
                    xActualDate = xActualDate.AddDays(1)
                Next z
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roCalendarManager::GetNextDayOfWeek")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCalendarManager::GetNextDayOfWeek")
            End Try

            Return xRet

        End Function

        Private Function GetNextDayofMonth(ByVal xActualDate As Date, ByVal NextDayofMonth As Integer) As Date
            Dim xRet As Date
            Dim intActualWeekDay As Integer = 0
            Try
                'Buscamos el siguiente dia del mes

                xActualDate = xActualDate.AddDays(1)

                If NextDayofMonth <> 32 Then
                    For z As Integer = 1 To 31
                        If Day(xActualDate) = NextDayofMonth Then
                            xRet = xActualDate
                            Exit For
                        End If
                        xActualDate = xActualDate.AddDays(1)
                    Next z
                Else
                    ' El ultimo dia del mes
                    xActualDate = New Date(Year(xActualDate), Month(xActualDate) + 1, 1)
                    xActualDate = xActualDate.AddDays(-1)
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roCalendarManager::GetNextDayofMonth")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCalendarManager::GetNextDayofMonth")
            End Try

            Return xRet

        End Function

        Public Function GetCalendarPassportConfig() As roCalendarPassportConfig

            Dim bolRet As New roCalendarPassportConfig

            Try

                Dim oContext As CContext = Nothing
                Try
                    oContext = WLHelper.GetContext(Me.oState.IDPassport)
                Catch
                    oContext = Nothing
                End Try

                If oContext IsNot Nothing Then
                    Dim oCalendarRemark As New Generic.List(Of roCalendarRemark)

                    Dim oConf As New roCollection(oContext.ConfXml)
                    Dim oRemarks As roCollection = oConf.Node("SchedulerRemarks")
                    If oRemarks IsNot Nothing Then
                        Dim intTotalRemarks As Integer = oRemarks.Item("TotalRemarks")
                        Dim strRemark As String
                        Dim RemarkConfig() As String
                        For n As Integer = 0 To intTotalRemarks - 1
                            strRemark = roTypes.Any2String(oRemarks.Item("Remark" & n.ToString))
                            RemarkConfig = strRemark.Split(";")
                            If RemarkConfig.Count = 4 Then

                                Dim oRemark As New roCalendarRemark()
                                oRemark.IdCause = roTypes.Any2Integer(RemarkConfig(0))
                                oRemark.Comparison = roTypes.Any2Integer(RemarkConfig(1))
                                oRemark.Value = roTypes.Any2DateTime(RemarkConfig(2))
                                oRemark.Color = roTypes.Any2Integer(RemarkConfig(3))

                                oCalendarRemark.Add(oRemark)
                            End If
                        Next
                    End If

                    While oCalendarRemark.Count < 3
                        oCalendarRemark.Add(New roCalendarRemark)
                    End While

                    bolRet.CalendarRemarks = oCalendarRemark.ToArray

                    Dim oCalendarAccruals As roCollection = oConf.Node("CalendarAccruals")
                    If oCalendarAccruals IsNot Nothing Then
                        bolRet.CalendarAccrual = oCalendarAccruals.Item("CalendarAccrual")
                        bolRet.CalendarHolidays = oCalendarAccruals.Item("CalendarHolidays")
                        bolRet.CalendarWorking = oCalendarAccruals.Item("CalendarWorking")
                        bolRet.CalendarOvertime = oCalendarAccruals.Item("CalendarOvertime")
                        bolRet.CalendarNotJustified = oCalendarAccruals.Item("CalendarNotJustified")
                    Else
                        bolRet.CalendarAccrual = New AdvancedParameter.roAdvancedParameter("ConceptCalendar", New AdvancedParameter.roAdvancedParameterState(Me.oState.IDPassport), False).Value
                        bolRet.CalendarHolidays = New AdvancedParameter.roAdvancedParameter("ConceptHolidaysCalendar", New AdvancedParameter.roAdvancedParameterState(Me.oState.IDPassport), False).Value
                        bolRet.CalendarWorking = New AdvancedParameter.roAdvancedParameter("ConceptNormalWorkCalendar", New AdvancedParameter.roAdvancedParameterState(Me.oState.IDPassport), False).Value
                        bolRet.CalendarOvertime = New AdvancedParameter.roAdvancedParameter("ConceptOverWorkingCalendar", New AdvancedParameter.roAdvancedParameterState(Me.oState.IDPassport), False).Value
                        bolRet.CalendarNotJustified = New AdvancedParameter.roAdvancedParameter("ConceptAbsenceCalendar", New AdvancedParameter.roAdvancedParameterState(Me.oState.IDPassport), False).Value
                    End If

                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCalendarManager::GetCalendarPassportConfig")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCalendarManager::GetCalendarPassportConfig")
            Finally

            End Try

            Return bolRet

        End Function

        Public Function SaveCalendarPassportConfig(ByVal oConf As roCalendarPassportConfig) As Boolean

            Dim bolRet As Boolean = True
            Try
                Dim oContext As CContext = Nothing
                Try
                    oContext = WLHelper.GetContext(Me.oState.IDPassport)
                Catch
                    oContext = Nothing
                End Try

                Dim RemarksConfig As roCollection = Nothing
                If oContext IsNot Nothing Then
                    RemarksConfig = New roCollection
                    RemarksConfig.Add("TotalRemarks", oConf.CalendarRemarks.Length)
                    For n As Integer = 0 To oConf.CalendarRemarks.Length - 1

                        Dim sIdCause As String = oConf.CalendarRemarks(n).IdCause.ToString
                        Dim sComparison As String = CInt(oConf.CalendarRemarks(n).Comparison).ToString
                        Dim sDate As String = Format(oConf.CalendarRemarks(n).Value, "HH:mm")
                        Dim sColor As String = oConf.CalendarRemarks(n).Color.ToString

                        RemarksConfig.Add("Remark" & n.ToString, sIdCause & ";" & sComparison & ";" & sDate & ";" & sColor)
                    Next

                    bolRet = True
                End If

                Dim AccrualsConfig As roCollection = Nothing
                If oContext IsNot Nothing Then
                    AccrualsConfig = New roCollection
                    AccrualsConfig.Add("CalendarAccrual", oConf.CalendarAccrual)
                    AccrualsConfig.Add("CalendarHolidays", oConf.CalendarHolidays)
                    AccrualsConfig.Add("CalendarWorking", oConf.CalendarWorking)
                    AccrualsConfig.Add("CalendarOvertime", oConf.CalendarOvertime)
                    AccrualsConfig.Add("CalendarNotJustified", oConf.CalendarNotJustified)
                    bolRet = True
                End If

                If oContext IsNot Nothing Then
                    Dim oRemarksConf As New roCollection(oContext.ConfXml)

                    If RemarksConfig IsNot Nothing Then
                        oRemarksConf.Remove("SchedulerRemarks")
                        oRemarksConf.Add("SchedulerRemarks", RemarksConfig)
                    End If

                    If AccrualsConfig IsNot Nothing Then
                        oRemarksConf.Remove("CalendarAccruals")
                        oRemarksConf.Add("CalendarAccruals", AccrualsConfig)
                    End If

                    oContext.ConfXml = oRemarksConf.XML
                    WLHelper.SetContext(Me.oState.IDPassport, oContext)
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCalendarManager::SaveCalendarPassportConfig")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCalendarManager::SaveCalendarPassportConfig")
            Finally
                '
            End Try

            Return bolRet

        End Function

        Public Function AssignPlanv2(ByVal strEmployees As String, ByVal xPlanDate As Date, ByRef oState As roCalendarState, Optional oNewCalendarRowDayData As roCalendarRowDayData = Nothing,
                                 Optional oProductiveUnitModePosition As roProductiveUnitModePosition = Nothing, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False
            oState.UpdateStateInfo()

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                ' Obtenemos los parametros necesarios
                ' Empleados a planificar
                Dim strEmployeeID() As String = strEmployees.Split(",")
                Dim iEmployeeID As New Generic.List(Of Integer)
                For Each sID As String In strEmployeeID
                    iEmployeeID.Add(roTypes.Any2Integer(sID))
                Next

                ' Para cada empleado a planificar
                For Each intEmployeeID As Integer In iEmployeeID
                    ' Obtenemmos el calendario del empleado en la fecha a planificar
                    Dim oCalendar As New roCalendar
                    oCalendar = Load(xPlanDate, xPlanDate, "B" & intEmployeeID.ToString, CalendarView.Planification, CalendarDetailLevel.Daily, False)

                    ' 5.Aplicamos la planificacion en los dias que sean necesarios
                    If oCalendar IsNot Nothing AndAlso oCalendar.CalendarData IsNot Nothing Then
                        For Each oCalendarRowEmployeeData As roCalendarRow In oCalendar.CalendarData
                            If oCalendarRowEmployeeData IsNot Nothing AndAlso oCalendarRowEmployeeData.PeriodData IsNot Nothing Then
                                For Each oCalendarRowDayData As roCalendarRowDayData In oCalendarRowEmployeeData.PeriodData.DayData
                                    ' Asignamos la planificacion indicada por presupuesto o por planificacion normal
                                    If oNewCalendarRowDayData Is Nothing AndAlso oProductiveUnitModePosition IsNot Nothing Then
                                        oNewCalendarRowDayData = New roCalendarRowDayData
                                        oNewCalendarRowDayData.PlanDate = oCalendarRowDayData.PlanDate

                                        oNewCalendarRowDayData.MainShift = New roCalendarRowShiftData
                                        oNewCalendarRowDayData.MainShift = oProductiveUnitModePosition.ShiftData

                                        oNewCalendarRowDayData.AssigData = New roCalendarAssignmentCellData
                                        oNewCalendarRowDayData.AssigData = oProductiveUnitModePosition.AssignmentData
                                        oNewCalendarRowDayData.IsHoliday = False
                                        oNewCalendarRowDayData.ShiftBase = Nothing
                                        oNewCalendarRowDayData.ShiftUsed = Nothing
                                        oNewCalendarRowDayData.IDDailyBudgetPosition = oProductiveUnitModePosition.ID
                                    End If

                                    AddCalendarRowDayData(oCalendar, oNewCalendarRowDayData, intEmployeeID, True, False, False, False, True, False, True, False, False)

                                Next
                            End If
                        Next
                    End If

                    ' 5.Guardamos los datos
                    Dim oCalendarResult As roCalendarResult = Save(oCalendar, True, True)
                    If oCalendarResult.Status = CalendarStatusEnum.OK Then
                        bolRet = True
                    Else
                        oState.Result = CalendarV2ResultEnum.InValidData
                        If oCalendarResult.CalendarDataResult IsNot Nothing Then
                            'Mostramos el primer error que tenga el resultado de guardar el calendario
                            oState.ErrorText = oCalendarResult.CalendarDataResult(0).ErrorText
                        End If
                        Exit For
                    End If

                Next
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roCalendarManager::AssignPlanv2")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCalendarManager::AssignPlanv2")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Function AddIndictmentsToCalendar(ByRef oCalendar As roCalendar, Optional ByVal sRulesToCheck As String = "") As Boolean
            '
            ' Asignamos los indicadores de planificacion a los empleados del calendario
            '

            Dim bolRet As Boolean = False
            Dim bContinueChecking As Boolean = True

            Try

                Me.oState.Result = CalendarV2ResultEnum.InValidData

                If oCalendar Is Nothing Then
                    Return bolRet
                    Exit Function
                End If

                If oCalendar.CalendarData Is Nothing Then
                    Return bolRet
                    Exit Function
                End If

                ' Cargamos los indicadores
                Dim oIndictments As New List(Of roCalendarScheduleIndictment)
                Try
                    Dim oCalRuleState As New roCalendarScheduleRulesState(oState.IDPassport)
                    Dim oCalRulesManager As New roCalendarScheduleRulesManager(oCalRuleState)
                    oIndictments = oCalRulesManager.CheckScheduleRules(oCalendar,, sRulesToCheck)
                Catch ex As Exception
                End Try

                'Eliminamos los indictments actuales en el calendario ya que queremos solo los nuevos
                For Each oCalendarRow As roCalendarRow In oCalendar.CalendarData
                    If oCalendarRow.EmployeeData IsNot Nothing Then
                        If oCalendarRow.PeriodData IsNot Nothing AndAlso oCalendarRow.PeriodData.DayData IsNot Nothing Then
                            For Each oPeriodData As roCalendarRowDayData In oCalendarRow.PeriodData.DayData
                                If oPeriodData.Alerts IsNot Nothing Then oPeriodData.Alerts.Indictments = {}
                            Next
                        End If
                    End If
                Next

                '  Añadimos los indicadores a cada empleado/dia que corresponda del calendario
                If oIndictments IsNot Nothing AndAlso oIndictments.Count > 0 Then
                    For Each _Indictment As roCalendarScheduleIndictment In oIndictments
                        If _Indictment.Dates IsNot Nothing AndAlso _Indictment.Dates.Count > 0 Then
                            For Each oCalendarRow As roCalendarRow In oCalendar.CalendarData
                                If oCalendarRow.EmployeeData IsNot Nothing AndAlso oCalendarRow.EmployeeData.IDEmployee = _Indictment.IDEmployee Then
                                    If oCalendarRow.PeriodData IsNot Nothing AndAlso oCalendarRow.PeriodData.DayData IsNot Nothing Then
                                        For Each oDayData As roCalendarRowDayData In oCalendarRow.PeriodData.DayData
                                            If _Indictment.Dates.Contains(oDayData.PlanDate) Then
                                                ' Añadimos el indicador  al empleado/dia correspondiente
                                                Dim oEmployeeIndictments As New List(Of roCalendarScheduleIndictment)
                                                If oDayData.Alerts IsNot Nothing AndAlso oDayData.Alerts.Indictments IsNot Nothing Then
                                                    oEmployeeIndictments = oDayData.Alerts.Indictments.ToList
                                                    oEmployeeIndictments.Add(_Indictment)
                                                    oDayData.Alerts.Indictments = oEmployeeIndictments.ToArray
                                                End If
                                            End If
                                        Next
                                    End If
                                End If
                            Next
                        End If
                    Next
                End If

                bolRet = True
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roCalendarManager::AddIndictmentsToCalendar")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarManager::AddIndictmentsToCalendar")
            Finally

            End Try

            Return bolRet

        End Function

        Public Function AddPunchesToCalendar(ByRef oCalendar As roCalendar, ByVal _FirstDay As DateTime, ByVal _LastDay As DateTime) As Boolean
            '
            ' Añadimos los fichajes de presencia a los empleados del calendario
            '

            Dim bolRet As Boolean = False
            Dim bContinueChecking As Boolean = True

            Try

                Me.oState.Result = CalendarV2ResultEnum.InValidData

                If oCalendar Is Nothing Then
                    Return bolRet
                    Exit Function
                End If

                If oCalendar.CalendarData Is Nothing Then
                    Return bolRet
                    Exit Function
                End If

                Dim oEmplotyeesState As New Employee.roEmployeeState
                Dim tbPunches As DataTable = Nothing
                Dim oRows() As DataRow = Nothing

                '  Añadimos los fichajes a cada empleado/dia que corresponda del calendario
                For Each oCalendarRow As roCalendarRow In oCalendar.CalendarData
                    If oCalendarRow.EmployeeData IsNot Nothing AndAlso oCalendarRow.EmployeeData.IDEmployee > 0 Then
                        If oCalendarRow.PeriodData IsNot Nothing AndAlso oCalendarRow.PeriodData.DayData IsNot Nothing Then
                            ' Obtenemos los fichajes del empleado en el periodo indicado
                            tbPunches = VTBusiness.Punch.roPunch.GetPunchesPres(oCalendarRow.EmployeeData.IDEmployee, _FirstDay, _LastDay, False, oEmplotyeesState, "ActualType IN(1,2)")
                            If tbPunches IsNot Nothing AndAlso tbPunches.Rows.Count > 0 Then
                                For Each oPeriodData As roCalendarRowDayData In oCalendarRow.PeriodData.DayData
                                    oRows = tbPunches.Select("ShiftDate = '" & Format(oPeriodData.PlanDate, "yyyy/MM/dd") & "'")
                                    If oRows IsNot Nothing AndAlso oRows.Length > 0 Then
                                        ' Añadimos los fichajes al dia del empleado
                                        Dim oEmployeePunches As New List(Of roCalendarRowPunchData)
                                        Dim oPunch As DataRow
                                        For nPunch As Integer = 0 To oRows.Length - 1
                                            Dim oCalendarRowPunchData As New roCalendarRowPunchData
                                            oPunch = oRows(nPunch)
                                            oCalendarRowPunchData.ActualType = oPunch("ActualType")
                                            oCalendarRowPunchData.DateTimePunch = oPunch("DateTime")
                                            oCalendarRowPunchData.ID = oPunch("ID")
                                            oEmployeePunches.Add(oCalendarRowPunchData)
                                        Next
                                        oPeriodData.PunchData = oEmployeePunches.ToArray
                                    End If
                                Next
                            End If
                        End If
                    End If
                Next

                bolRet = True
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roCalendarManager::AddPunchesToCalendar")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarManager::AddPunchesToCalendar")
            Finally

            End Try

            Return bolRet
        End Function

        ''' <summary>
        ''' Devuelve lista de capacidades reales o estimadas de las distintas zonas previstas para los empleados en el scope del calendario
        ''' </summary>
        ''' <param name="oCalendar"></param>
        ''' <param name="oState"></param>
        ''' <param name="oConnection"></param>
        ''' <returns></returns>
        Public Shared Function LoadCapacitySeating(ByRef oCalendar As roCalendar, ByRef oState As roCalendarState) As roCalendarSeatingCapacityDay()
            ' Llenamos los datos de afoto para las distintas zonas
            Dim oRet As New List(Of roCalendarSeatingCapacityDay)
            Dim tbSeatingCapacityDetail As DataTable = Nothing

            Dim strSQL As String

            Try

                ' Si no tiene permisos de lectura como minimo no mostramos nada
                Dim oPermissionPassport As Permission = WLHelper.GetPermissionOverFeature(oState.IDPassport, "Calendar.Scheduler", "U")

                If Zone.roZone.CapacityControlEnabled(New roZoneState(-1)) AndAlso oPermissionPassport >= Permission.Read Then
                    Dim selectedEmployeeIDs As String = String.Empty
                    selectedEmployeeIDs = String.Join(",", oCalendar.CalendarData.ToList.Select(Function(x) x.EmployeeData.IDEmployee).ToArray)
                    If selectedEmployeeIDs.Length = 0 Then
                        Return oRet.ToArray
                        Exit Function
                    End If

                    ' 00. Obtenemos las zonas previstas por fecha de los distintos empleados seleccionados en el periodo
                    ' Datos de todos los empleados (no sólo los seleccionados en el Calendario, dado que quiero info de aforos disponibles)
                    strSQL = "@SELECT#  *, ISNULL(TelecommutePlanned, TelecommutingExpected) Telecommute, CASE WHEN ISNULL(ZoneOnDate,'') <> '' THEN ZoneOnDate ELSE ISNULL(ExpectedZone,'?') END Zone  " &
                             "FROM [dbo].[EmployeeZonesBetweenDates] (" & roTypes.Any2Time(oCalendar.FirstDay).SQLSmallDateTime & "," & roTypes.Any2Time(oCalendar.LastDay).SQLSmallDateTime & ",'')"

                    tbSeatingCapacityDetail = CreateDataTable(strSQL)

                    If tbSeatingCapacityDetail Is Nothing Then
                        Return oRet.ToArray
                        Exit Function
                    End If

                    Dim oRows As roCalendarRow() = oCalendar.CalendarData
                    Dim oCalendarRowPeriodData As New roCalendarRowPeriodData
                    Dim oSeatingCapacityData As roCalendarSeatingCapacityDay
                    Dim lCapacities As New List(Of roCalendarCapacities)
                    Dim oCalendarCapacity As roCalendarCapacities
                    Dim lZoneNames As List(Of String)
                    Dim dicZones As New Dictionary(Of String, Integer)
                    lZoneNames = New List(Of String)
                    If Not tbSeatingCapacityDetail Is Nothing Then
                        lZoneNames = (From row In tbSeatingCapacityDetail.Select("IdEmployee In (" & selectedEmployeeIDs & ")").AsEnumerable() Select row.Field(Of String)("Zone")).Distinct().ToList().FindAll(Function(x) x.ToString.Trim.Length > 0)
                    End If

                    ' 01. Obtenemos aforos máximos de los distintos centros, en función de lo definido en las zonas que los conforman
                    For Each sZoneCalculated In lZoneNames
                        strSQL = "@SELECT# SUM(Capacity) FROM Zones WHERE Name = '" & sZoneCalculated & "'"
                        dicZones.Add(sZoneCalculated, roTypes.Any2Integer(ExecuteScalar(strSQL)))
                    Next

                    ' Filtro previo
                    Dim dView As DataView = tbSeatingCapacityDetail.DefaultView
                    'dView.RowFilter = "Zone IN (" & "'" & String.Join("','", lZoneNames) & "'" & ") AND NoWork = 0 AND InAbsence = 0 AND ISNULL(TelecommutePlanned, TelecommutingExpected) = 0"
                    dView.RowFilter = "Zone IN (" & "'" & String.Join("','", lZoneNames) & "'" & ")"

                    If oRows.Count > 0 AndAlso lZoneNames.Count > 0 Then
                        oCalendarRowPeriodData = oRows(0).PeriodData
                        For Each oDayData As roCalendarRowDayData In oCalendarRowPeriodData.DayData
                            oSeatingCapacityData = New roCalendarSeatingCapacityDay
                            lCapacities = New List(Of roCalendarCapacities)
                            For Each sZoneName As String In lZoneNames
                                oCalendarCapacity = New roCalendarCapacities
                                oCalendarCapacity.ZoneName = sZoneName
                                Select Case oDayData.PlanDate.Subtract(Now.Date).Days
                                    Case < 0
                                        oCalendarCapacity.CurrentSeating = roTypes.Any2Integer(dView.ToTable.Select("RefDate = '" & Format(oDayData.PlanDate, "yyyy/MM/dd") & "' AND Zone = '" & sZoneName & "'").Count)
                                    Case > 0
                                        oCalendarCapacity.CurrentSeating = roTypes.Any2Integer(dView.ToTable.Select("RefDate = '" & Format(oDayData.PlanDate, "yyyy/MM/dd") & "' AND Zone = '" & sZoneName & "' AND NoWork = 0 AND InAbsence = 0 AND Telecommute = 0").Count)
                                    Case = 0
                                        Dim iInZone As Integer = 0
                                        Dim iExpected As Integer = 0
                                        iInZone = roTypes.Any2Integer(dView.ToTable.Select("RefDate = '" & Format(oDayData.PlanDate, "yyyy/MM/dd") & "' AND ISNULL(ZoneOnDate,'') = '" & sZoneName & "'").Count)
                                        iExpected = roTypes.Any2Integer(dView.ToTable.Select("RefDate = '" & Format(oDayData.PlanDate, "yyyy/MM/dd") & "' AND ISNULL(ZoneOnDate,'') = '' AND ISNULL(ExpectedZone,'') = '" & sZoneName & "' AND NoWork = 0 AND InAbsence = 0 AND Telecommute = 0").Count)
                                        oCalendarCapacity.CurrentSeating = iInZone + iExpected
                                End Select
                                oCalendarCapacity.MaxSeatingCapacity = dicZones(sZoneName)
                                oCalendarCapacity.ZoneCapacityVisible = roTypes.Any2Boolean(ExecuteScalar("@SELECT# CapacityVisible FROM Zones WHERE Name = '" & sZoneName & "'"))
                                lCapacities.Add(oCalendarCapacity)
                            Next
                            oSeatingCapacityData.Capacities = lCapacities.ToArray
                            oRet.Add(oSeatingCapacityData)
                        Next
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roCalendarManager::LoadCapacitySeating")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCalendarManager::LoadCapacitySeating")
            Finally

            End Try

            Return oRet.ToArray

        End Function

        ''' <summary>
        ''' Usuarios que acudirán a una zona (la zona esperada en función de la zona de su último fichaje), exceptuando a quien hace la consulta, y siempre que no tengan teletrabajo, ni horario no laboral, ni ausencia prevista
        ''' </summary>
        ''' <param name="idEmployee"></param>
        ''' <param name="refDate"></param>
        ''' <param name="oState"></param>
        ''' <param name="oConnection"></param>
        ''' <returns></returns>
        Public Shared Function LoadEmployeesOnWorkCenter(idEmployee As Integer, refDate As Date, ByRef oState As roCalendarState) As DataTable
            Dim oRet As DataTable = Nothing

            Dim strSQL As String

            Try

                strSQL = "@SELECT# OtherEmployee.IDEmployee, sysrovwCurrentEmployeeGroups.EmployeeName, sysrovwCurrentEmployeeGroups.GroupName, Employees.Image AS EmployeeImage " &
                         "FROM [dbo].[EmployeeZonesBetweenDates] (" & roTypes.Any2Time(refDate).SQLSmallDateTime & "," & roTypes.Any2Time(refDate).SQLSmallDateTime & ",'') OtherEmployee " &
                         " INNER JOIN [dbo].[EmployeeZonesBetweenDates] (" & roTypes.Any2Time(refDate).SQLSmallDateTime & "," & roTypes.Any2Time(refDate).SQLSmallDateTime & ",'" & idEmployee.ToString & "') MySelf ON MySelf.ExpectedZone = OtherEmployee.ExpectedZone AND ISNULL(OtherEmployee.ExpectedZone,'') <> '' " &
                         " INNER JOIN sysrovwCurrentEmployeeGroups ON sysrovwCurrentEmployeeGroups.IDEmployee = OtherEmployee.IDEmployee " &
                         " INNER JOIN Employees ON Employees.ID = OtherEmployee.IDEmployee " &
                         " WHERE (ISNULL(OtherEmployee.TelecommutePlanned, OtherEmployee.TelecommutingExpected) = 0 AND OtherEmployee.InAbsence = 0 AND OtherEmployee.NoWork = 0) OR OtherEmployee.ZoneOnDate IS NOT NULL"

                oRet = CreateDataTable(strSQL)
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roCalendarManager::LoadEmployeesOnWorkCenter")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCalendarManager::LoadEmployeesOnWorkCenter")
            Finally

            End Try

            Return oRet

        End Function

#End Region

#Region "Excel Link"

        Public Function ExportToExcel(ByVal oCalendar As roCalendar, Optional strProfilePath As String = "", Optional strExcelOutPath As String = "", Optional strExpShiftFilter As String = "", Optional oExcelProfileBytes As Byte() = Nothing) As Byte()
            Dim oRet As Byte() = Nothing
            Dim oExcel As New SLDocument()

            Try

                Dim iFirstRow As Integer = 2
                Dim iFirstColumn As Integer = 3
                Dim iCurrentRow As Integer = iFirstRow
                Dim iCurrentCol As Integer = iFirstColumn
                Dim sNIF As String = String.Empty
                Dim bEncryptNIFOnCalendarExport As Boolean = True

                ' Miramos si se debe pintar color de horarios en las exportaciones de Excel
                bShiftColorOnExport = (roTypes.Any2String(AdvancedParameter.roAdvancedParameter.GetAdvancedParameterValue("Calendar.ShiftColorOnExport", Nothing, False)) <> "0")
                ' Cargamos identificador de personalización
                customization = VTBusiness.Common.roBusinessSupport.GetCustomizationCode().ToUpper()
                ' Vemos si hay que encriptar NIF en exportación
                Dim sParamValue As String = String.Empty
                sParamValue = roTypes.Any2String(AdvancedParameter.roAdvancedParameter.GetAdvancedParameterValue("Calendar.EncryptNIFOnCalendarExport", Nothing, False))
                bEncryptNIFOnCalendarExport = (sParamValue.ToUpper <> "FALSE" AndAlso sParamValue <> "0")
                '0.- Cargo definición de plantilla
                Dim oCalendarCellLayout As roCalendarCellLayout

                If oExcelProfileBytes IsNot Nothing AndAlso oExcelProfileBytes.Length > 0 Then
                    oCalendarCellLayout = LoadProfile("", oExcelProfileBytes)
                Else
                    ' Busco la plantilla en la carpeta personalizada del cliente ...
                    oExcelProfileBytes = Azure.RoAzureSupport.DownloadFileFromCompanyContainer("CalendarLinkCellLayout.xlsx", roLiveDatalinkFolders.templates.ToString, roLiveQueueTypes.datalink)
                    If oExcelProfileBytes Is Nothing OrElse oExcelProfileBytes.Length = 0 Then
                        ' Y si no la encontré, busco entre las comunes
                        oExcelProfileBytes = Azure.RoAzureSupport.GetCommonTemplateBytesFromAzure("CalendarLinkCellLayout.xlsx", DTOs.roLiveQueueTypes.datalink)
                    End If
                    oCalendarCellLayout = LoadProfile("", oExcelProfileBytes)
                End If

                '1.-Cabecera con fechas
                Dim oCalendarRow As roCalendarRow
                oCalendarRow = oCalendar.CalendarData(0)
                iCurrentCol = 3

                If roTypes.Any2String(customization).ToUpper = "SEDIFOC" Then
                    'Para COFIDES incluimos una columna al lado del nombre con el nombre del grupo
                    iFirstColumn = iFirstColumn + 1
                    iCurrentCol = iFirstColumn
                End If

                ' Si la plantilla incluye alertas de planificación, las calculo ahora
                If oCalendarCellLayout.CellLayout.ContainsKey(roCalendarCellLayout.eCalendarCellElement.ScheduleRulesFaults) Then
                    Dim oParam As AdvancedParameter.roAdvancedParameter
                    Dim sRulesToNotify As String = String.Empty
                    oParam = New AdvancedParameter.roAdvancedParameter("VTLive.Notifications.RulesToNotify", New AdvancedParameter.roAdvancedParameterState(oState.IDPassport), False)
                    sRulesToNotify = roTypes.Any2String(oParam.Value)
                    Me.AddIndictmentsToCalendar(oCalendar, sRulesToNotify)
                End If

                Dim iPos As Integer = 0
                For Each oDayData As roCalendarRowDayData In oCalendarRow.PeriodData.DayData
                    ExcelWriteCellDate(oExcel, iCurrentRow, iCurrentCol, oDayData.PlanDate.Date, 90, "0000")
                    If bShiftColorOnExport Then
                        Dim oColor As System.Drawing.Color = ColorTranslator.FromHtml(oCalendar.CalendarHeader.PeriodHeaderData(iPos).BackColor)
                        Dim ostyle As SpreadsheetLight.SLStyle
                        ostyle = oExcel.CreateStyle
                        ostyle.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, oColor, System.Drawing.Color.Blue)
                        oExcel.SetCellStyle(iCurrentRow, iCurrentCol, ostyle)
                    End If
                    ' Recuadro
                    OutlineRange(iCurrentRow, iCurrentCol, iCurrentRow, iCurrentCol + oCalendarCellLayout.Width - 1, oExcel)
                    iCurrentCol += oCalendarCellLayout.Width
                    iPos += 1
                Next
                iCurrentRow += 1

                '2.-Bucle de empleados
                For Each oCalendarRowEx As roCalendarRow In oCalendar.CalendarData
                    'Nombre de empleado
                    ExcelWriteCellString(oExcel, iCurrentRow, 1, oCalendarRowEx.EmployeeData.EmployeeName, 0, "0000")
                    'NIF de empleado como clave primaria
                    sNIF = GetEmployeeKeyById(oCalendarRowEx.EmployeeData.IDEmployee)
                    If bEncryptNIFOnCalendarExport Then
                        sNIF = CryptographyHelper.Encrypt(sNIF)
                    End If
                    If sNIF Is Nothing OrElse sNIF.Trim = String.Empty Then
                        'Marco en rojo para que se note el error (no hay NIF informado)
                        ExcelWriteSetAlarm(oExcel, iCurrentRow, 2, 1, 1, roCalendarCellLayout.eCalendarCellPattern.InvalidData, "¿NIF?")
                    Else
                        ExcelWriteCellString(oExcel, iCurrentRow, 2, sNIF, 0, "0000")
                    End If
                    Dim sGroup As String = String.Empty
                    If roTypes.Any2String(customization).ToUpper = "SEDIFOC" Then
                        'Para COFIDES incluimos una columna al lado del nombre con el nombre del grupo
                        Dim sGroupName As String = oCalendarRowEx.EmployeeData.GroupName

                        ExcelWriteCellString(oExcel, iCurrentRow, 3, FilterGroupName(sGroupName), 0, "0000")
                        ' Recuadro
                        OutlineRange(iCurrentRow, 1, iCurrentRow + oCalendarCellLayout.Height - 1, 3, oExcel)
                    Else
                        ' Recuadro
                        OutlineRange(iCurrentRow, 1, iCurrentRow + oCalendarCellLayout.Height - 1, 2, oExcel)
                    End If

                    '2.1.- Para cada empleado, recorro todos los días
                    iCurrentCol = iFirstColumn
                    For Each oDayData As roCalendarRowDayData In oCalendarRowEx.PeriodData.DayData
                        'Recupero la clave de exportación y la guardo como shortname
                        GetDayShiftsExportCode(oDayData)

                        ' Recuperlo la clave de exportacion del puesto y lo guardo como ShortName
                        GetDayAssignmentsExportCode(oDayData)

                        If strExpShiftFilter.Length <> 0 Then
                            If Not oDayData.MainShift Is Nothing AndAlso Not strExpShiftFilter.Split(",").Contains(oDayData.MainShift.ShortName.ToString.ToUpper) Then
                                oDayData.MainShift = Nothing
                            End If

                            If Not oDayData.ShiftBase Is Nothing AndAlso Not strExpShiftFilter.Split(",").Contains(oDayData.ShiftBase.ShortName.ToString.ToUpper) Then
                                oDayData.ShiftBase = Nothing
                            End If
                        End If

                        ExcelWriteCellWithProfile(oExcel, oCalendarCellLayout, iCurrentRow, iCurrentCol, oDayData)
                        iCurrentCol += oCalendarCellLayout.Width
                    Next
                    iCurrentRow += oCalendarCellLayout.Height
                Next
                oExcel.AutoFitColumn(1, oCalendar.CalendarHeader.PeriodHeaderData.Count * oCalendarCellLayout.Width + 3)

                Dim oStream As New IO.MemoryStream
                oExcel.SaveAs(oStream)
                oRet = oStream.ToArray()
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarManager::ExportToExcel")
                Return Nothing
            End Try
            Return oRet
        End Function

        Public Function ExportCompanyToExcel(ByVal oCalendar As roCalendar, Optional strProfilePath As String = "", Optional strExcelOutPath As String = "", Optional strExpShiftFilter As String = "", Optional oExcelProfileBytes As Byte() = Nothing) As Byte()
            Dim oRet As Byte() = Nothing
            Dim oExcel As New SLDocument()
            Dim columnConfigs As New List(Of ColumnConfig)

            Try
                ' Primero obtenemos la empresa principal del primer usuario
                Dim companyId As Integer = 0
                Dim filteredEmployees As New List(Of roCalendarRow)
                Dim bGroupState As New roGroupState(oState.IDPassport)

                If oCalendar.CalendarData.Count > 0 Then
                    Dim firstEmployee As roCalendarRow = oCalendar.CalendarData(0)
                    companyId = roGroup.GetRootCompanyIdFromGroupId(firstEmployee.EmployeeData.IDGroup, bGroupState)

                    For Each employee As roCalendarRow In oCalendar.CalendarData
                        Dim empCompanyId As Integer = roGroup.GetRootCompanyIdFromGroupId(employee.EmployeeData.IDGroup, bGroupState)

                        If empCompanyId = companyId Then
                            filteredEmployees.Add(employee)
                        End If
                    Next
                End If

                ' Printamos la informacion de la empresa
                Dim companyInfo As Dictionary(Of String, String) = GetCompanyInfoById(companyId)

                Dim iFirstRow As Integer = 2
                Dim iFirstColumn As Integer = 3
                Dim iCurrentRow As Integer = iFirstRow
                Dim iCurrentCol As Integer = iFirstColumn

                Dim oEmployeeState = New roEmployeeState()
                roBusinessState.CopyTo(oState, oEmployeeState)
                oEmployeeState.Language.SetLanguageReference("LiveEmployees", oState.Language.LanguageKey)
                Dim lblSocialDesignation As String = oEmployeeState.Language.Translate("employees._translate_socialdesignation", "")
                Dim lblHeadquarters As String = oEmployeeState.Language.Translate("employees._translate_headquarters", "")
                Dim lblNIF As String = oState.Language.Translate("Calendar.Header.NIF", "")
                Dim lblEconomicActivity As String = oEmployeeState.Language.Translate("employees._translate_economicactivity", "")
                Dim lblCollectiveReg As String = oEmployeeState.Language.Translate("employees._translate_colectiveinstreg", "")
                Dim lblWorkPlace As String = oEmployeeState.Language.Translate("employees._translate_workplace", "")
                Dim lblWorkingHours As String = oEmployeeState.Language.Translate("employees._translate_workingshift", "")
                Dim lblPrintedDate As String = oState.Language.Translate("Calendar.Header.PrintedDate", "")

                oExcel.SetCellValue(iCurrentRow, 1, lblSocialDesignation)
                oExcel.SetCellValue(iCurrentRow, 2, If(companyInfo.ContainsKey("USR__translate_socialDesignation"), companyInfo("USR__translate_socialDesignation"), ""))
                oExcel.SetCellValue(iCurrentRow, 3, lblWorkPlace)
                oExcel.SetCellValue(iCurrentRow, 4, If(companyInfo.ContainsKey("USR__translate_workPlace"), companyInfo("USR__translate_workPlace"), ""))
                oExcel.SetCellValue(iCurrentRow, 5, lblPrintedDate)
                oExcel.SetCellValue(iCurrentRow, 6, DateTime.Now.ToString("dd/MM/yyyy"))

                iCurrentRow += 1
                oExcel.SetCellValue(iCurrentRow, 1, lblHeadquarters)
                oExcel.SetCellValue(iCurrentRow, 2, If(companyInfo.ContainsKey("USR__translate_headquarters"), companyInfo("USR__translate_headquarters"), ""))
                oExcel.SetCellValue(iCurrentRow, 3, lblWorkingHours)
                oExcel.SetCellValue(iCurrentRow, 4, If(companyInfo.ContainsKey("USR__translate_workingShift"), companyInfo("USR__translate_workingShift"), ""))

                iCurrentRow += 1
                oExcel.SetCellValue(iCurrentRow, 1, lblNIF)
                oExcel.SetCellValue(iCurrentRow, 2, If(companyInfo.ContainsKey("USR_CIF"), companyInfo("USR_CIF"), ""))

                iCurrentRow += 1
                oExcel.SetCellValue(iCurrentRow, 1, lblEconomicActivity)
                oExcel.SetCellValue(iCurrentRow, 2, If(companyInfo.ContainsKey("USR__translate_economicActivity"), companyInfo("USR__translate_economicActivity"), ""))

                iCurrentRow += 1
                oExcel.SetCellValue(iCurrentRow, 1, lblCollectiveReg)
                oExcel.SetCellValue(iCurrentRow, 2, If(companyInfo.ContainsKey("USR__translate_colectiveInstReg"), companyInfo("USR__translate_colectiveInstReg"), ""))

                ' Linea de separacion entre la informacion de la empresa y los usuarios/horarios
                iCurrentRow += 1

                iFirstRow = iCurrentRow + 1
                iCurrentRow = iFirstRow

                Dim labelStyle As SpreadsheetLight.SLStyle = oExcel.CreateStyle()
                labelStyle.Font.Bold = True
                labelStyle.Font.FontSize = 12
                labelStyle.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray)

                Dim valueStyle As SpreadsheetLight.SLStyle = oExcel.CreateStyle()
                valueStyle.Font.Bold = False
                valueStyle.Font.FontSize = 12
                valueStyle.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray)

                oExcel.SetCellStyle(2, 1, iCurrentRow - 2, 1, labelStyle)
                oExcel.SetCellStyle(2, 3, iCurrentRow - 2, 3, labelStyle)
                oExcel.SetCellStyle(2, 5, 2, 5, labelStyle)

                oExcel.SetCellStyle(2, 2, iCurrentRow - 2, 2, valueStyle)
                oExcel.SetCellStyle(2, 4, iCurrentRow - 2, 4, valueStyle)
                oExcel.SetCellStyle(2, 6, 2, 6, valueStyle)

                ' Miramos si se debe pintar color de horarios en las exportaciones de Excel
                bShiftColorOnExport = (roTypes.Any2String(AdvancedParameter.roAdvancedParameter.GetAdvancedParameterValue("Calendar.ShiftColorOnExport", Nothing, False)) <> "0")
                ' Cargamos identificador de personalización
                customization = VTBusiness.Common.roBusinessSupport.GetCustomizationCode().ToUpper()

                '0.- Cargo definición de plantilla
                Dim oCalendarCellLayout As roCalendarCellLayout

                If oExcelProfileBytes IsNot Nothing AndAlso oExcelProfileBytes.Length > 0 Then
                    oCalendarCellLayout = LoadProfile("", oExcelProfileBytes)
                    ' Cargar configuración de columnas desde la plantilla
                    columnConfigs = LoadColumnConfiguration(oExcelProfileBytes)
                Else
                    ' Busco la plantilla en la carpeta personalizada del cliente ...
                    oExcelProfileBytes = Azure.RoAzureSupport.DownloadFileFromCompanyContainer("CalendarLinkCellCompanyLayout.xlsx", roLiveDatalinkFolders.templates.ToString, roLiveQueueTypes.datalink)
                    If oExcelProfileBytes Is Nothing OrElse oExcelProfileBytes.Length = 0 Then
                        ' Y si no la encontré, busco entre las comunes
                        oExcelProfileBytes = Azure.RoAzureSupport.GetCommonTemplateBytesFromAzure("CalendarLinkCellCompanyLayout.xlsx", DTOs.roLiveQueueTypes.datalink)
                    End If
                    oCalendarCellLayout = LoadProfile("", oExcelProfileBytes)
                    ' Cargar configuración de columnas desde la plantilla
                    columnConfigs = LoadColumnConfiguration(oExcelProfileBytes)
                End If

                ' Si no hay configuración, usamos valores por defecto
                If columnConfigs.Count = 0 Then
                    columnConfigs.Add(New ColumnConfig With {.Position = 1, .FieldId = "EmployeeKey", .DisplayName = Me.oState.Language.Translate("Calendar.Header.EmployeeKey", "")})
                    columnConfigs.Add(New ColumnConfig With {.Position = 2, .FieldId = "EmployeeName", .DisplayName = Me.oState.Language.Translate("Calendar.Header.EmployeeName", "")})
                    columnConfigs.Add(New ColumnConfig With {.Position = 3, .FieldId = "EmployeeGroup", .DisplayName = Me.oState.Language.Translate("Calendar.Header.EmployeeGroup", "")})
                End If

                ' Ajustar iFirstColumn según la cantidad de columnas configuradas
                iFirstColumn = columnConfigs.Max(Function(c) c.Position) + 1

                '1.- Cabecera con fechas
                Dim oCalendarRow As roCalendarRow
                If filteredEmployees.Count > 0 Then
                    oCalendarRow = filteredEmployees(0)
                Else
                    oCalendarRow = oCalendar.CalendarData(0)
                End If

                iCurrentCol = iFirstColumn

                ' Si la plantilla incluye alertas de planificación, las calculo ahora
                If oCalendarCellLayout.CellLayout.ContainsKey(roCalendarCellLayout.eCalendarCellElement.ScheduleRulesFaults) Then
                    Dim oParam As AdvancedParameter.roAdvancedParameter
                    Dim sRulesToNotify As String = String.Empty
                    oParam = New AdvancedParameter.roAdvancedParameter("VTLive.Notifications.RulesToNotify", New AdvancedParameter.roAdvancedParameterState(oState.IDPassport), False)
                    sRulesToNotify = roTypes.Any2String(oParam.Value)
                    Me.AddIndictmentsToCalendar(oCalendar, sRulesToNotify)
                End If

                Dim iPos As Integer = 0
                For Each oDayData As roCalendarRowDayData In oCalendarRow.PeriodData.DayData
                    ExcelWriteCellDate(oExcel, iCurrentRow, iCurrentCol, oDayData.PlanDate.Date, 90, "0000")
                    If bShiftColorOnExport Then
                        Dim oColor As System.Drawing.Color = ColorTranslator.FromHtml(oCalendar.CalendarHeader.PeriodHeaderData(iPos).BackColor)
                        Dim ostyle As SpreadsheetLight.SLStyle
                        ostyle = oExcel.CreateStyle
                        ostyle.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, oColor, System.Drawing.Color.Blue)
                        oExcel.SetCellStyle(iCurrentRow, iCurrentCol, ostyle)
                    End If
                    ' Recuadro
                    OutlineRange(iCurrentRow, iCurrentCol, iCurrentRow, iCurrentCol + oCalendarCellLayout.Width - 1, oExcel)
                    iCurrentCol += oCalendarCellLayout.Width
                    iPos += 1
                Next
                iCurrentRow += 1

                ' Encabezados de columnas para empleados
                For Each colConfig In columnConfigs
                    Dim headerText As String

                    ' Determinar si el texto debe traducirse o mostrarse literalmente
                    If colConfig.DisplayName.EndsWith("*") Then
                        ' Si termina en *, se muestra literal sin el asterisco
                        headerText = colConfig.DisplayName.TrimEnd("*"c)
                    Else
                        ' Si no termina en *, se traduce usando el sistema de traducción
                        headerText = Me.oState.Language.Translate("Calendar.Header." & colConfig.DisplayName, "")
                    End If

                    ExcelWriteCellString(oExcel, iCurrentRow, colConfig.Position, headerText, 0, "0000")
                Next

                ' Crear estilo para los días de la semana
                Dim weekdayStyle As SpreadsheetLight.SLStyle = oExcel.CreateStyle()
                weekdayStyle.Font.Bold = True
                weekdayStyle.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, System.Drawing.Color.White, System.Drawing.Color.White)

                ' Crear estilo para fines de semana
                Dim weekendStyle As SpreadsheetLight.SLStyle = oExcel.CreateStyle()
                weekendStyle.Font.Bold = True
                weekendStyle.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray)

                ' Añadir los nombres de los días de la semana
                iCurrentCol = iFirstColumn
                iPos = 0
                For Each oDayData As roCalendarRowDayData In oCalendarRow.PeriodData.DayData
                    Dim dayName As String = ""
                    Dim isWeekend As Boolean = False

                    Select Case oDayData.PlanDate.DayOfWeek
                        Case DayOfWeek.Monday
                            dayName = Me.oState.Language.Translate("Calendar.Header.Monday", "")
                        Case DayOfWeek.Tuesday
                            dayName = Me.oState.Language.Translate("Calendar.Header.Tuesday", "")
                        Case DayOfWeek.Wednesday
                            dayName = Me.oState.Language.Translate("Calendar.Header.Wednesday", "")
                        Case DayOfWeek.Thursday
                            dayName = Me.oState.Language.Translate("Calendar.Header.Thursday", "")
                        Case DayOfWeek.Friday
                            dayName = Me.oState.Language.Translate("Calendar.Header.Friday", "")
                        Case DayOfWeek.Saturday
                            dayName = Me.oState.Language.Translate("Calendar.Header.Saturday", "")
                            isWeekend = True
                        Case DayOfWeek.Sunday
                            dayName = Me.oState.Language.Translate("Calendar.Header.Sunday", "")
                            isWeekend = True
                    End Select

                    'Insertamos el valor del dia de la semana
                    ExcelWriteCellString(oExcel, iCurrentRow, iCurrentCol, dayName, 0, "0000")
                    If isWeekend Then
                        oExcel.SetCellStyle(iCurrentRow, iCurrentCol, weekendStyle)
                    Else
                        oExcel.SetCellStyle(iCurrentRow, iCurrentCol, weekdayStyle)
                    End If

                    OutlineRange(iCurrentRow, iCurrentCol, iCurrentRow, iCurrentCol + oCalendarCellLayout.Width - 1, oExcel)

                    iCurrentCol += oCalendarCellLayout.Width
                    iPos += 1
                Next

                iCurrentRow += 1

                '2.-Bucle de empleados - ahora usando los empleados filtrados
                For Each oCalendarRowEx As roCalendarRow In filteredEmployees
                    For Each colConfig In columnConfigs
                        Dim fieldValue As String = ""
                        'Insertamos los valores del campo de la ficha
                        If colConfig.FieldId.ToUpper().StartsWith("USR_") Then
                            Try
                                Dim fieldName As String = colConfig.FieldId
                                Dim oPassport As roPassport = roPassportManager.GetPassport(oCalendarRowEx.EmployeeData.IDEmployee, LoadType.Employee)
                                Dim field As roEmployeeUserField = roEmployeeUserField.GetEmployeeUserFieldValueAtDate(oCalendarRowEx.EmployeeData.IDEmployee, fieldName.Replace("USR_", "").Trim, DateAndTime.Now, New VTUserFields.UserFields.roUserFieldState(oPassport.ID), False)

                                If field IsNot Nothing Then
                                    fieldValue = field.FieldValue
                                Else
                                    fieldValue = ""
                                End If
                            Catch ex As Exception
                                fieldValue = ""
                            End Try
                        Else
                            'Insertamos los valores que soportamos por defecto
                            Select Case colConfig.FieldId.ToUpper()
                                Case "EMPLOYEENAME"
                                    fieldValue = oCalendarRowEx.EmployeeData.EmployeeName
                                Case "EMPLOYEEGROUP"
                                    fieldValue = roTypes.Any2String(GetLastGroup(oCalendarRowEx.EmployeeData.GroupName))
                                Case "EMPLOYEEKEY"
                                    Dim idEmployee As Integer = oCalendarRowEx.EmployeeData.IDEmployee
                                    Dim xParam As New AdvancedParameter.roAdvancedParameter("ImportPrimaryKeyUserField", New AdvancedParameter.roAdvancedParameterState)
                                    Dim strImportPrimaryKeyUserField As String = ""
                                    If roTypes.Any2String(xParam.Value).Length > 0 Then
                                        strImportPrimaryKeyUserField = roTypes.Any2String(xParam.Value)
                                    End If

                                    Dim oPassport As roPassport = roPassportManager.GetPassport(oCalendarRowEx.EmployeeData.IDEmployee, LoadType.Employee)
                                    If strImportPrimaryKeyUserField.Length = 0 Then strImportPrimaryKeyUserField = "NIF"
                                    Dim field As roEmployeeUserField = roEmployeeUserField.GetEmployeeUserFieldValueAtDate(oCalendarRowEx.EmployeeData.IDEmployee, strImportPrimaryKeyUserField.Trim, DateAndTime.Now, New VTUserFields.UserFields.roUserFieldState(oPassport.ID), False)

                                    If field IsNot Nothing Then
                                        fieldValue = field.FieldValue
                                    Else
                                        fieldValue = ""
                                    End If
                                Case Else
                                    fieldValue = "-"
                            End Select
                        End If

                        ' Escribir el valor en la celda
                        ExcelWriteCellString(oExcel, iCurrentRow, colConfig.Position, fieldValue, 0, "0000")
                    Next

                    ' Recuadro para las columnas de identificación
                    OutlineRange(iCurrentRow, 1, iCurrentRow + oCalendarCellLayout.Height - 1, iFirstColumn - 1, oExcel)

                    '2.1.- Para cada empleado, recorro todos los días
                    iCurrentCol = iFirstColumn
                    For Each oDayData As roCalendarRowDayData In oCalendarRowEx.PeriodData.DayData
                        'Recupero la clave de exportación y la guardo como shortname
                        GetDayShiftsExportCode(oDayData)

                        ' Recuperlo la clave de exportacion del puesto y lo guardo como ShortName
                        GetDayAssignmentsExportCode(oDayData)

                        If strExpShiftFilter.Length <> 0 Then
                            If Not oDayData.MainShift Is Nothing AndAlso Not strExpShiftFilter.Split(",").Contains(oDayData.MainShift.ShortName.ToString.ToUpper) Then
                                oDayData.MainShift = Nothing
                            End If

                            If Not oDayData.ShiftBase Is Nothing AndAlso Not strExpShiftFilter.Split(",").Contains(oDayData.ShiftBase.ShortName.ToString.ToUpper) Then
                                oDayData.ShiftBase = Nothing
                            End If
                        End If

                        ExcelWriteCellWithProfile(oExcel, oCalendarCellLayout, iCurrentRow, iCurrentCol, oDayData)
                        iCurrentCol += oCalendarCellLayout.Width
                    Next
                    iCurrentRow += oCalendarCellLayout.Height
                Next
                oExcel.AutoFitColumn(1, oCalendar.CalendarHeader.PeriodHeaderData.Count * oCalendarCellLayout.Width + 3)

                Dim oStream As New IO.MemoryStream
                oExcel.SaveAs(oStream)
                oRet = oStream.ToArray()
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarManager::ExportToExcel")
                Return Nothing
            End Try
            Return oRet
        End Function

        Private Function GetLastGroup(pathString As String) As String
            Dim lastBackslashPos As Integer
            lastBackslashPos = InStrRev(pathString, "\")

            If lastBackslashPos > 0 Then
                GetLastGroup = Trim(Right(pathString, Len(pathString) - lastBackslashPos))
            Else
                GetLastGroup = Trim(pathString)
            End If
        End Function

        Public Class ColumnConfig
            Public Property Position As Integer
            Public Property FieldId As String
            Public Property DisplayName As String
        End Class

        Private Function LoadColumnConfiguration(ByVal excelBytes As Byte()) As List(Of ColumnConfig)
            Dim result As New List(Of ColumnConfig)

            Try
                Using stream As New IO.MemoryStream(excelBytes)
                    Dim excel As New SLDocument(stream)

                    Dim sheetNames = excel.GetSheetNames()
                    If sheetNames.Count > 1 Then
                        excel.SelectWorksheet(sheetNames(1))

                        Dim row As Integer = 2
                        While Not String.IsNullOrEmpty(excel.GetCellValueAsString(row, 1))
                            Try
                                Dim config As New ColumnConfig()
                                config.Position = Integer.Parse(excel.GetCellValueAsString(row, 1))
                                config.FieldId = excel.GetCellValueAsString(row, 2)
                                config.DisplayName = excel.GetCellValueAsString(row, 3)

                                result.Add(config)
                            Catch

                            End Try

                            row += 1
                        End While
                    End If
                End Using
            Catch ex As Exception
                result = New List(Of ColumnConfig)()
            End Try

            Return result
        End Function

        Private Function GetCompanyInfoById(ByVal companyId As Integer) As Dictionary(Of String, String)
            Dim result As New Dictionary(Of String, String)

            Try
                If companyId > 0 Then
                    Dim sql As String = "@SELECT# USR__translate_socialDesignation, USR__translate_workPlace, " &
                               "USR__translate_headquarters, USR__translate_workingShift, " &
                               "USR__translate_colectiveInstReg, USR__translate_economicActivity, USR_CIF " &
                               $"FROM Groups WHERE ID = {companyId}"

                    Dim tbl As New DataTable
                    tbl = CreateDataTable(sql)

                    If tbl IsNot Nothing AndAlso tbl.Rows.Count > 0 Then
                        For Each col As System.Data.DataColumn In tbl.Columns
                            result(col.ColumnName) = Convert.ToString(tbl.Rows(0)(col.ColumnName))
                        Next
                    End If
                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarManager::GetCompanyInfoById")
            End Try

            Return result
        End Function

        Private Sub GetDayAssignmentsExportCode(ByRef oDayData As roCalendarRowDayData)
            Try
                If oDayData.AssigData IsNot Nothing Then oDayData.AssigData.ShortName = VTBusiness.Assignment.roAssignment.GetAssignmentExportKeyById(oDayData.AssigData.ID)
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarManager::GetDayAssignmentsExportCode")
            End Try
        End Sub

        Private Sub GetDayShiftsExportCode(ByRef oDayData As roCalendarRowDayData)
            Try
                If oDayData.MainShift IsNot Nothing Then oDayData.MainShift.ShortName = VTBusiness.Shift.roShift.GetShiftExportKeyById(oDayData.MainShift.ID)
                If oDayData.AltShift1 IsNot Nothing Then oDayData.AltShift1.ShortName = VTBusiness.Shift.roShift.GetShiftExportKeyById(oDayData.AltShift1.ID)
                If oDayData.AltShift2 IsNot Nothing Then oDayData.AltShift2.ShortName = VTBusiness.Shift.roShift.GetShiftExportKeyById(oDayData.AltShift2.ID)
                If oDayData.AltShift3 IsNot Nothing Then oDayData.AltShift3.ShortName = VTBusiness.Shift.roShift.GetShiftExportKeyById(oDayData.AltShift3.ID)
                If oDayData.ShiftBase IsNot Nothing Then oDayData.ShiftBase.ShortName = VTBusiness.Shift.roShift.GetShiftExportKeyById(oDayData.ShiftBase.ID)
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarManager::GetDayShiftsExportCode")
            End Try
        End Sub

        Public Function ImportFromExcelToDDBB(arrFile As Byte(), strEmployees As String, dStartDate As Date, dEndDate As Date, ByRef oResult As roCalendarResult, Optional bolCopyMainShifts As Boolean = True,
                                              Optional bolCopyHolidays As Boolean = False, Optional bolCopyAlternatives As Boolean = False, Optional bolKeepHolidays As Boolean = True,
                                              Optional bolKeepLockedDay As Boolean = True, Optional strProfilePath As String = "", Optional ByVal lstAssignments As String = "",
                                              Optional ByVal bLoadChilds As Boolean = False, Optional ByVal bExcelIsATemplate As Boolean = False, Optional oExcelProfileBytes As Byte() = Nothing) As CalendarStatusEnum
            Dim bReturn As CalendarStatusEnum = CalendarStatusEnum.OK
            Dim oCalendar As New roCalendar
            Try
                oCalendar = ImportFromExcelToScreen(arrFile, strEmployees, dStartDate, dEndDate, bolCopyMainShifts, bolCopyHolidays, bolCopyAlternatives, bolKeepHolidays, bolKeepLockedDay, "",
                                                    oResult, lstAssignments, bLoadChilds, strProfilePath, bExcelIsATemplate, oExcelProfileBytes)
                bReturn = Me.SaveFromImport(oCalendar).Status
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarManager::ImportFromExcelToDDBB")
                bReturn = CalendarStatusEnum.KO
            End Try
            Return bReturn
        End Function

        Public Function ImportFromASCIIToDDBB(arrFile As Byte(), ByRef oResult As roCalendarResult, Optional bolCopyMainShifts As Boolean = True,
                                              Optional bolCopyHolidays As Boolean = False, Optional bolCopyAlternatives As Boolean = False, Optional bolKeepHolidays As Boolean = True,
                                              Optional bolKeepLockedDay As Boolean = True, Optional strProfilePath As String = "", Optional ByVal lstAssignments As String = "",
                                              Optional ByVal bLoadChilds As Boolean = False, Optional oASCIIProfileBytes As Byte() = Nothing) As CalendarStatusEnum
            Dim bReturn As CalendarStatusEnum = CalendarStatusEnum.OK
            Dim oCalendar As New roCalendar
            Try
                oCalendar = ImportFromASCIIToScreen(arrFile, bolCopyMainShifts, bolCopyHolidays, bolCopyAlternatives, bolKeepHolidays, bolKeepLockedDay, "",
                                                    oResult, lstAssignments, bLoadChilds, strProfilePath, oASCIIProfileBytes)
                bReturn = Me.SaveFromImport(oCalendar).Status
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarManager::ImportFromASCIIToDDBB")
                bReturn = CalendarStatusEnum.KO
            End Try
            Return bReturn
        End Function

        Public Function ImportFromExcelToScreen(ByVal arrFile As Byte(), ByVal strEmployees As String, ByVal dStartDate As Date, ByVal dEndDate As Date, ByVal bolCopyMainShifts As Boolean, ByVal bolCopyHolidays As Boolean,
                                                ByVal bolCopyAlternatives As Boolean, ByVal bolKeepHolidays As Boolean, ByVal bolKeepLockedDay As Boolean,
                                                ByRef strFileNameError As String, ByRef oCalResult As roCalendarResult, ByVal lstAssignments As String, ByVal bLoadChilds As Boolean,
                                                Optional strProfilePath As String = "", Optional bExcelIsATemplate As Boolean = False, Optional oExcelProfileBytes As Byte() = Nothing) As roCalendar
            ' Tratamiento extendido de errores
            Dim oErrorList As New Generic.List(Of roCalendarDataDayError)
            Dim oError As roCalendarDataDayError = Nothing

            Dim oWSStats As SLWorksheetStatistics
            Dim iFirstRow As Integer = 3
            Dim iFirstColumn As Integer = 3
            Const iDateRow As Integer = 2
            Const iEmployeeKeyColumn As Integer = 2
            Const iEmployeeNameColumn As Integer = 1

            Dim sValue As String = String.Empty
            Dim fValue As Double = 0
            Dim sAux As String
            Dim dAux As DateTime
            Dim dValue As DateTime
            Dim oDayData As New roCalendarRowDayData
            Dim oMainShiftData As New roCalendarRowShiftData
            Dim oBaseShiftData As New roCalendarRowShiftData
            Dim oAltShift1Data As New roCalendarRowShiftData
            Dim oAltShift2Data As New roCalendarRowShiftData
            Dim oAltShift3Data As New roCalendarRowShiftData
            Dim oAssignmentData As New roCalendarAssignmentCellData

            Dim oState As roCalendarState

            Dim oCalendarManager As roCalendarManager = Nothing
            Dim oCalendar As roCalendar = Nothing
            Dim dPlannedDate As Date
            Dim iIdEmployee As Integer
            Dim iIdShift As Integer
            Dim iIdAssignment As Integer
            Dim sEmployeeKey As String
            Dim sEmployeeName As String
            Dim bErrorOnDayData As Boolean = False
            Dim bEmptyDayData As Boolean = False
            Dim bNoShiftSpecified As Boolean = True
            Dim dEmployeeIDs As New Dictionary(Of String, Integer)
            Dim dShiftIDs As New Dictionary(Of String, Integer)
            Dim dAssignmentIDs As New Dictionary(Of String, Integer)
            Dim bWarning As Boolean = False
            Dim bolCopyFeastDay As Boolean = False
            Dim bEncryptNIFOnCalendarExport As Boolean = True

            Try
                ' Vemos si hay que encriptar NIF en exportación
                Dim sParamValue As String = String.Empty
                sParamValue = roTypes.Any2String(AdvancedParameter.roAdvancedParameter.GetAdvancedParameterValue("Calendar.EncryptNIFOnCalendarExport", Nothing, False))
                bEncryptNIFOnCalendarExport = (sParamValue.ToUpper <> "FALSE" AndAlso sParamValue <> "0")

                ' Creo el fichero EXCEL de trabajo
                Dim oImportFile As New SLDocument(New System.IO.MemoryStream(arrFile))

                oState = New roCalendarState(Me.oState.IDPassport)
                oWSStats = oImportFile.GetWorksheetStatistics
                oCalendarManager = New roCalendarManager(oState)

                '0.- Cargo definición de plantilla
                Dim oCalendarCellLayout As roCalendarCellLayout = Nothing

                If oExcelProfileBytes Is Nothing Then
                    ' Busco la plantilla en la carpeta personalizada del cliente ...
                    oExcelProfileBytes = Azure.RoAzureSupport.DownloadFileFromCompanyContainer("CalendarLinkCellLayout.xlsx", roLiveDatalinkFolders.templates.ToString, roLiveQueueTypes.datalink)
                    If oExcelProfileBytes Is Nothing OrElse oExcelProfileBytes.Length = 0 Then
                        ' Y si no la encontré, busco entre las comunes
                        oExcelProfileBytes = Azure.RoAzureSupport.GetCommonTemplateBytesFromAzure("CalendarLinkCellLayout.xlsx", DTOs.roLiveQueueTypes.datalink)
                    End If
                    oCalendarCellLayout = LoadProfile("", oExcelProfileBytes)
                Else
                    oCalendarCellLayout = LoadProfile("", oExcelProfileBytes)
                End If

                Dim iEndColumnIndex As Integer = oWSStats.EndColumnIndex
                Dim iEndRowIndex As Integer = oWSStats.EndRowIndex

                If ValidateImportFile(oImportFile, oCalendarCellLayout, iEndRowIndex, iEndColumnIndex, bExcelIsATemplate) Then
                    If oCalendarCellLayout IsNot Nothing Then
                        '1.- Cargo datos de base de datos, paraq luego añadir el contenido del Excel
                        oCalendar = oCalendarManager.Load(dStartDate, dEndDate, strEmployees, CalendarView.Planification, CalendarDetailLevel.Daily, bLoadChilds, , lstAssignments)

                        '2.- Proceso el contenido del fichero excel
                        customization = VTBusiness.Common.roBusinessSupport.GetCustomizationCode().ToUpper()
                        If roTypes.Any2String(customization).ToUpper = "SEDIFOC" Then
                            'Para COFIDES incluimos una columna al lado del nombre con el nombre del grupo
                            iFirstColumn = 4
                        End If

                        ' Para cada día incluido en el Excel ...
                        For iColumn As Integer = iFirstColumn To iEndColumnIndex Step oCalendarCellLayout.Width
                            dPlannedDate = Robotics.VTBase.roTypes.Any2Time(oImportFile.GetCellValueAsDateTime(iDateRow, iColumn)).Value
                            If dPlannedDate = roTypes.CreateDateTime(1900, 1, 1) Then
                                LogError(oErrorList, CalendarErrorResultDayEnum.GenericError, oCalResult.Status,,,,,, Me.oState.Language.Translate("CalendarRowPeriodDataManager.Validate.InvalidDate", ""))
                                'ExcelWriteSetAlarm(oImportFile, iFirstRow, iColumn, Color.Red, Color.White, "")
                                'Marco fecha como incorrecta
                                ExcelWriteSetAlarm(oImportFile, iFirstRow, iColumn, oCalendarCellLayout.Width, 1, roCalendarCellLayout.eCalendarCellPattern.InvalidData, "")
                                'Marco todas las celdas de esa fecha como ignorada
                                ExcelWriteSetAlarm(oImportFile, iFirstRow + 1, iColumn, oCalendarCellLayout.Width, iEndRowIndex - iFirstRow + 1, roCalendarCellLayout.eCalendarCellPattern.Ignored, "")
                                Exit For
                            End If

                            Dim iXFactor As Integer = 1
                            Dim dEmployeeToSchedule As New Dictionary(Of Integer, String)

                            If bExcelIsATemplate Then
                                For Each oRow As roCalendarRow In oCalendar.CalendarData
                                    If Not dEmployeeToSchedule.ContainsKey(oRow.EmployeeData.IDEmployee) Then dEmployeeToSchedule.Add(oRow.EmployeeData.IDEmployee, oRow.EmployeeData.EmployeeName)
                                Next
                                iXFactor = dEmployeeToSchedule.Count
                            End If

                            For iX As Integer = 0 To iXFactor - 1
                                ' Para cada empleado incluido en el Excel
                                For iRow As Integer = iFirstRow To iEndRowIndex Step oCalendarCellLayout.Height
                                    bErrorOnDayData = False

                                    If Not bExcelIsATemplate Then
                                        ' Recupero id empleado si no lo tenía
                                        sEmployeeKey = oImportFile.GetCellValueAsString(iRow, iEmployeeKeyColumn)
                                        sEmployeeName = oImportFile.GetCellValueAsString(iRow, iEmployeeNameColumn)
                                        If bEncryptNIFOnCalendarExport Then
                                            sEmployeeKey = CryptographyHelper.Decrypt(sEmployeeKey)
                                        End If
                                        If Not dEmployeeIDs.ContainsKey(sEmployeeKey) Then
                                            iIdEmployee = GetEmployeeIdByKey(sEmployeeKey)
                                            dEmployeeIDs.Add(sEmployeeKey, iIdEmployee)
                                        End If
                                        If Not dEmployeeIDs.TryGetValue(sEmployeeKey, iIdEmployee) Then
                                            bErrorOnDayData = True
                                        End If
                                        If iIdEmployee = 0 Then
                                            bErrorOnDayData = True
                                        End If
                                    Else
                                        ' Recupero idemployee
                                        iIdEmployee = dEmployeeToSchedule.ToList(iX).Key
                                        sEmployeeKey = iIdEmployee.ToString
                                        sEmployeeName = dEmployeeToSchedule.ToList(iX).Value
                                    End If

                                    If Not bErrorOnDayData Then
                                        oDayData.MainShift = New roCalendarRowShiftData
                                        oDayData.AltShift1 = New roCalendarRowShiftData
                                        oDayData.AltShift2 = New roCalendarRowShiftData
                                        oDayData.AltShift3 = New roCalendarRowShiftData
                                        oDayData.ShiftBase = New roCalendarRowShiftData
                                        oMainShiftData = New roCalendarRowShiftData
                                        oBaseShiftData = New roCalendarRowShiftData
                                        oAltShift1Data = New roCalendarRowShiftData
                                        oAltShift2Data = New roCalendarRowShiftData
                                        oAltShift3Data = New roCalendarRowShiftData
                                        oAssignmentData = New roCalendarAssignmentCellData

                                        ' Recojo todas las propiedades del día/empleado
                                        bEmptyDayData = True
                                        Dim bolCopyTelecommute As Boolean = False
                                        For iCellCol As Integer = 1 To oCalendarCellLayout.Width
                                            For iCellRow As Integer = 1 To oCalendarCellLayout.Height
                                                Dim oElement As roCellElement
                                                oElement = New roCellElement(iCellRow, iCellCol, roCalendarCellLayout.eCalendarCellElementTypeData.Text)
                                                If oCalendarCellLayout.CellLayout.ContainsValue(oElement) Then
                                                    Dim eElement As roCalendarCellLayout.eCalendarCellElement = oCalendarCellLayout.CellLayout.First(Function(x) x.Value.Equals(oElement)).Key
                                                    'Recupero tipo
                                                    If oImportFile.HasCellValue(iRow + iCellRow - 1, iColumn + iCellCol - 1) Then
                                                        bEmptyDayData = False
                                                    End If
                                                    Select Case oCalendarCellLayout.CellLayout.Item(eElement).TypeData
                                                        Case roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                                            sValue = oImportFile.GetCellValueAsString(iRow + iCellRow - 1, iColumn + iCellCol - 1)
                                                        Case roCalendarCellLayout.eCalendarCellElementTypeData.Number
                                                            fValue = oImportFile.GetCellValueAsDouble(iRow + iCellRow - 1, iColumn + iCellCol - 1)
                                                        Case roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                                            'Miro si hay identificador de día anterior o día posterior
                                                            sAux = oImportFile.GetCellValueAsString(iRow + iCellRow - 1, iColumn + iCellCol - 1)
                                                            If sAux.Split("+").Length > 1 Then
                                                                dAux = roTypes.Any2DateTime(oImportFile.GetCellValueAsString(iRow + iCellRow - 1, iColumn + iCellCol - 1).Split("+")(0))
                                                                dValue = New Date(1899, 12, 31, dAux.Hour, dAux.Minute, dAux.Second)
                                                            ElseIf sAux.Split("-").Length > 1 Then
                                                                dAux = roTypes.Any2DateTime(oImportFile.GetCellValueAsString(iRow + iCellRow - 1, iColumn + iCellCol - 1).Split("-")(0))
                                                                dValue = New Date(1899, 12, 29, dAux.Hour, dAux.Minute, dAux.Second)
                                                            Else
                                                                If oImportFile.HasCellValue(iRow + iCellRow - 1, iColumn + iCellCol - 1) Then
                                                                    dAux = oImportFile.GetCellValueAsDateTime(iRow + iCellRow - 1, iColumn + iCellCol - 1)
                                                                    dValue = New Date(1899, 12, 30, dAux.Hour, dAux.Minute, dAux.Second)
                                                                Else
                                                                    dValue = Date.MinValue
                                                                End If
                                                            End If
                                                    End Select

                                                    If eElement = roCalendarCellLayout.eCalendarCellElement.MainShiftID OrElse
                                                            eElement = roCalendarCellLayout.eCalendarCellElement.AltShift1ID OrElse
                                                            eElement = roCalendarCellLayout.eCalendarCellElement.AltShift2ID OrElse
                                                            eElement = roCalendarCellLayout.eCalendarCellElement.AltShift3ID OrElse
                                                            eElement = roCalendarCellLayout.eCalendarCellElement.BaseShiftID Then
                                                        ' Recupero id de horario si no lo tenía
                                                        iIdShift = -1
                                                        bErrorOnDayData = False
                                                        bNoShiftSpecified = False
                                                        If Not dShiftIDs.ContainsKey(sValue) Then
                                                            iIdShift = VTBusiness.Shift.roShift.GetShiftIdByExportKey(sValue)
                                                            dShiftIDs.Add(sValue, iIdShift)
                                                        End If
                                                        If Not dShiftIDs.TryGetValue(sValue, iIdShift) Then
                                                            bErrorOnDayData = True
                                                        End If
                                                        If iIdShift = -1 Then
                                                            If Not sValue.Trim = String.Empty Then
                                                                'Marco día si se indicó un nombre de horario que no localicé
                                                                ExcelWriteSetAlarm(oImportFile, iRow, iColumn, oCalendarCellLayout.Width, oCalendarCellLayout.Height, roCalendarCellLayout.eCalendarCellPattern.InvalidData, "")
                                                                ExcelWriteSetAlarm(oImportFile, iRow + iCellRow - 1, iColumn + iCellCol - 1, Color.Red, Color.White, sValue)
                                                                LogError(oErrorList, CalendarErrorResultDayEnum.ShiftNotExist, oCalResult.Status,, iIdEmployee, dPlannedDate, sEmployeeName, sValue)
                                                                bErrorOnDayData = True
                                                            Else
                                                                bNoShiftSpecified = True
                                                            End If
                                                        End If
                                                    End If

                                                    If eElement = roCalendarCellLayout.eCalendarCellElement.Assignment Then
                                                        ' Recupero la info del puesto
                                                        iIdAssignment = -1
                                                        bErrorOnDayData = False
                                                        bNoShiftSpecified = False
                                                        If Not dAssignmentIDs.ContainsKey(sValue) Then
                                                            iIdAssignment = VTBusiness.Assignment.roAssignment.GetAssignmentIdByExportKey(sValue)
                                                            dAssignmentIDs.Add(sValue, iIdAssignment)
                                                        End If
                                                        If Not dAssignmentIDs.TryGetValue(sValue, iIdAssignment) Then
                                                            bErrorOnDayData = True
                                                        End If
                                                        If iIdAssignment = -1 And sValue.Length > 0 Then
                                                            If Not sValue.Trim = String.Empty Then
                                                                'Marco día si se indicó un nombre de puesto que no localicé
                                                                ExcelWriteSetAlarm(oImportFile, iRow, iColumn, oCalendarCellLayout.Width, oCalendarCellLayout.Height, roCalendarCellLayout.eCalendarCellPattern.InvalidData, "")
                                                                ExcelWriteSetAlarm(oImportFile, iRow + iCellRow - 1, iColumn + iCellCol - 1, Color.Red, Color.White, sValue)
                                                                LogError(oErrorList, CalendarErrorResultDayEnum.InvalidAssignmentData, oCalResult.Status,, iIdEmployee, dPlannedDate, sEmployeeName, sValue)
                                                                bErrorOnDayData = True
                                                            Else
                                                                bNoShiftSpecified = True
                                                            End If
                                                        End If

                                                    End If

                                                    If Not bErrorOnDayData OrElse bNoShiftSpecified Then
                                                        Select Case eElement
                                                            Case roCalendarCellLayout.eCalendarCellElement.MainShiftID
                                                                oMainShiftData.ShortName = sValue
                                                                oMainShiftData.ID = iIdShift
                                                            Case roCalendarCellLayout.eCalendarCellElement.MainShiftStartTime
                                                                oMainShiftData.StartHour = dValue
                                                            Case roCalendarCellLayout.eCalendarCellElement.MainShiftStartTime1
                                                                InitShiftLayersDefinitionDataIfNeeded(oMainShiftData)
                                                                oMainShiftData.ShiftLayersDefinition(0).LayerStartTime = dValue
                                                            Case roCalendarCellLayout.eCalendarCellElement.MainShiftOrdinaryHours1
                                                                InitShiftLayersDefinitionDataIfNeeded(oMainShiftData)
                                                                If dValue <> Date.MinValue Then oMainShiftData.ShiftLayersDefinition(0).LayerOrdinaryHours = dValue.Hour * 60 + dValue.Minute
                                                            Case roCalendarCellLayout.eCalendarCellElement.MainShiftComplementaryHours1
                                                                InitShiftLayersDefinitionDataIfNeeded(oMainShiftData)
                                                                If dValue <> Date.MinValue Then oMainShiftData.ShiftLayersDefinition(0).LayerComplementaryHours = dValue.Hour * 60 + dValue.Minute
                                                            Case roCalendarCellLayout.eCalendarCellElement.MainShiftStartTime2
                                                                InitShiftLayersDefinitionDataIfNeeded(oMainShiftData)
                                                                oMainShiftData.ShiftLayersDefinition(1).LayerStartTime = dValue
                                                            Case roCalendarCellLayout.eCalendarCellElement.MainShiftOrdinaryHours2
                                                                InitShiftLayersDefinitionDataIfNeeded(oMainShiftData)
                                                                If dValue <> Date.MinValue Then oMainShiftData.ShiftLayersDefinition(1).LayerOrdinaryHours = dValue.Hour * 60 + dValue.Minute
                                                            Case roCalendarCellLayout.eCalendarCellElement.MainShiftComplementaryHours2
                                                                InitShiftLayersDefinitionDataIfNeeded(oMainShiftData)
                                                                If dValue <> Date.MinValue Then oMainShiftData.ShiftLayersDefinition(1).LayerComplementaryHours = dValue.Hour * 60 + dValue.Minute
                                                    '    '----------------------BASE-------------------------------------
                                                            Case roCalendarCellLayout.eCalendarCellElement.BaseShiftID
                                                                oBaseShiftData.ShortName = sValue
                                                                oBaseShiftData.ID = iIdShift
                                                            Case roCalendarCellLayout.eCalendarCellElement.BaseShiftStartTime
                                                                oBaseShiftData.StartHour = dValue
                                                            Case roCalendarCellLayout.eCalendarCellElement.BaseShiftStartTime1
                                                                InitShiftLayersDefinitionDataIfNeeded(oBaseShiftData)
                                                                oBaseShiftData.ShiftLayersDefinition(0).LayerStartTime = dValue
                                                            Case roCalendarCellLayout.eCalendarCellElement.BaseShiftOrdinaryHours1
                                                                InitShiftLayersDefinitionDataIfNeeded(oBaseShiftData)
                                                                If dValue <> Date.MinValue Then oBaseShiftData.ShiftLayersDefinition(0).LayerOrdinaryHours = dValue.Hour * 60 + dValue.Minute
                                                            Case roCalendarCellLayout.eCalendarCellElement.BaseShiftComplementaryHours1
                                                                InitShiftLayersDefinitionDataIfNeeded(oBaseShiftData)
                                                                If dValue <> Date.MinValue Then oBaseShiftData.ShiftLayersDefinition(0).LayerComplementaryHours = dValue.Hour * 60 + dValue.Minute
                                                            Case roCalendarCellLayout.eCalendarCellElement.BaseShiftStartTime2
                                                                InitShiftLayersDefinitionDataIfNeeded(oBaseShiftData)
                                                                oBaseShiftData.ShiftLayersDefinition(1).LayerStartTime = dValue
                                                            Case roCalendarCellLayout.eCalendarCellElement.BaseShiftOrdinaryHours2
                                                                InitShiftLayersDefinitionDataIfNeeded(oBaseShiftData)
                                                                If dValue <> Date.MinValue Then oBaseShiftData.ShiftLayersDefinition(1).LayerOrdinaryHours = dValue.Hour * 60 + dValue.Minute
                                                            Case roCalendarCellLayout.eCalendarCellElement.BaseShiftComplementaryHours2
                                                                InitShiftLayersDefinitionDataIfNeeded(oBaseShiftData)
                                                                If dValue <> Date.MinValue Then oBaseShiftData.ShiftLayersDefinition(1).LayerComplementaryHours = dValue.Hour * 60 + dValue.Minute
                                                            Case roCalendarCellLayout.eCalendarCellElement.Assignment
                                                                oAssignmentData.ShortName = sValue
                                                                oAssignmentData.ID = iIdAssignment

                                                            Case roCalendarCellLayout.eCalendarCellElement.FeastDay
                                                                bolCopyFeastDay = True
                                                                oDayData.Feast = False
                                                                If sValue = "1" Then
                                                                    oDayData.Feast = True
                                                                End If
                                                            Case roCalendarCellLayout.eCalendarCellElement.Telecommuting
                                                                oDayData.TelecommuteForced = False
                                                                Dim bolCanTelecommute As Boolean = False
                                                                sValue = sValue.Trim.ToUpper

                                                                ' Si viene informado algun valor
                                                                If sValue.Trim <> "" Then

                                                                    ' Verificamos si puede teletrabajar ese dia por convenio/contrato
                                                                    For Each oExistingCalendarRow As roCalendarRow In oCalendar.CalendarData
                                                                        If oExistingCalendarRow.EmployeeData IsNot Nothing AndAlso oExistingCalendarRow.EmployeeData.IDEmployee = iIdEmployee Then
                                                                            For Each oExistingCalendarRowDayData As roCalendarRowDayData In oExistingCalendarRow.PeriodData.DayData
                                                                                If oExistingCalendarRowDayData.PlanDate = dPlannedDate Then
                                                                                    bolCanTelecommute = oExistingCalendarRowDayData.CanTelecommute
                                                                                    oDayData.TelecommutingMandatoryDays = oExistingCalendarRowDayData.TelecommutingMandatoryDays
                                                                                    oDayData.TelecommutingOptionalDays = oExistingCalendarRowDayData.TelecommutingOptionalDays
                                                                                    oDayData.PresenceMandatoryDays = oExistingCalendarRowDayData.PresenceMandatoryDays
                                                                                    Exit For
                                                                                End If
                                                                            Next
                                                                        End If
                                                                    Next

                                                                    ' Si el empleado puede teletrabajar por su acuerdo y el dia corresponde a un dia que es de trabajo

                                                                    If bolCanTelecommute Then

                                                                        Dim bolApplyDay As Boolean = False
                                                                        bolApplyDay = oDayData.TelecommutingMandatoryDays.Contains(dPlannedDate.DayOfWeek)
                                                                        If Not bolApplyDay Then bolApplyDay = oDayData.TelecommutingOptionalDays.Contains(dPlannedDate.DayOfWeek)
                                                                        If Not bolApplyDay Then bolApplyDay = oDayData.PresenceMandatoryDays.Contains(dPlannedDate.DayOfWeek)
                                                                        If bolApplyDay Then
                                                                            ' Si tiene valor debemos forzar un valor
                                                                            oDayData.TelecommuteForced = True
                                                                            oDayData.TelecommutingExpected = False
                                                                            oDayData.TelecommutingOptional = False

                                                                            Select Case sValue.Trim
                                                                                Case "T" ' Teletrabajo
                                                                                    oDayData.TelecommutingExpected = True
                                                                                    bolCopyTelecommute = True

                                                                                Case "P" ' Presencial
                                                                                    oDayData.TelecommutingExpected = False
                                                                                    bolCopyTelecommute = True

                                                                                Case "O" ' Opcional
                                                                                    oDayData.TelecommutingOptional = True
                                                                                    bolCopyTelecommute = True

                                                                                Case Else
                                                                                    If sValue.Trim.Contains("A") AndAlso sValue.Trim.Length <= 2 Then ' Acuerdo (A, AT, AP, AO)
                                                                                        oDayData.TelecommutingExpected = oDayData.TelecommutingMandatoryDays.Contains(dPlannedDate.DayOfWeek)
                                                                                        oDayData.TelecommutingOptional = oDayData.TelecommutingOptionalDays.Contains(dPlannedDate.DayOfWeek)
                                                                                        oDayData.TelecommuteForced = False
                                                                                        bolCopyTelecommute = True
                                                                                    End If
                                                                            End Select
                                                                        End If
                                                                    End If
                                                                End If

                                                                '--------------------------------------------------------------NO BORRAR-------------------------------------------------------------------------------------------------
                                                                '    '----------------------ALT1-------------------------------------
                                                                'Case roCalendarCellLayout.eCalendarCellElement.AltShift1ID
                                                                '    oAltShift1Data.ShortName = sValue
                                                                '    oAltShift1Data.ID = iIdShift
                                                                'Case roCalendarCellLayout.eCalendarCellElement.AltShift1StartTime
                                                                '    oAltShift1Data.ShiftLayersDefinition(0).LayerStartTime = dValue
                                                                'Case roCalendarCellLayout.eCalendarCellElement.AltShift1StartTime1
                                                                '    oAltShift1Data.ShiftLayersDefinition(0).LayerStartTime = dValue
                                                                'Case roCalendarCellLayout.eCalendarCellElement.AltShift1OrdinaryHours1
                                                                '    oAltShift1Data.ShiftLayersDefinition(0).LayerOrdinaryHours = fValue
                                                                'Case roCalendarCellLayout.eCalendarCellElement.AltShift1ComplementaryHours1
                                                                '    oAltShift1Data.ShiftLayersDefinition(0).LayerComplementaryHours = fValue
                                                                'Case roCalendarCellLayout.eCalendarCellElement.AltShift1StartTime2
                                                                '    oAltShift1Data.ShiftLayersDefinition(1).LayerStartTime = dValue
                                                                'Case roCalendarCellLayout.eCalendarCellElement.AltShift1OrdinaryHours1
                                                                '    oAltShift1Data.ShiftLayersDefinition(1).LayerOrdinaryHours = fValue
                                                                'Case roCalendarCellLayout.eCalendarCellElement.AltShift1ComplementaryHours2
                                                                '    oAltShift1Data.ShiftLayersDefinition(1).LayerComplementaryHours = fValue
                                                                '    '----------------------ALT2-------------------------------------
                                                                'Case roCalendarCellLayout.eCalendarCellElement.AltShift2ID
                                                                '    oAltShift2Data.ShortName = sValue
                                                                '    oAltShift2Data.ID = iIdShift
                                                                'Case roCalendarCellLayout.eCalendarCellElement.AltShift2StartTime
                                                                '    oAltShift2Data.StartHour = dValue
                                                                'Case roCalendarCellLayout.eCalendarCellElement.AltShift2StartTime1
                                                                '    oAltShift2Data.ShiftLayersDefinition(0).LayerStartTime = dValue
                                                                'Case roCalendarCellLayout.eCalendarCellElement.AltShift2OrdinaryHours1
                                                                '    oAltShift2Data.ShiftLayersDefinition(0).LayerOrdinaryHours = fValue
                                                                'Case roCalendarCellLayout.eCalendarCellElement.AltShift2ComplementaryHours1
                                                                '    oAltShift2Data.ShiftLayersDefinition(0).LayerComplementaryHours = fValue
                                                                'Case roCalendarCellLayout.eCalendarCellElement.AltShift2StartTime2
                                                                '    oAltShift2Data.ShiftLayersDefinition(1).LayerStartTime = dValue
                                                                'Case roCalendarCellLayout.eCalendarCellElement.AltShift2OrdinaryHours1
                                                                '    oAltShift2Data.ShiftLayersDefinition(1).LayerOrdinaryHours = fValue
                                                                'Case roCalendarCellLayout.eCalendarCellElement.AltShift2ComplementaryHours2
                                                                '    oAltShift2Data.ShiftLayersDefinition(1).LayerComplementaryHours = fValue
                                                                '    '----------------------ALT3-------------------------------------
                                                                'Case roCalendarCellLayout.eCalendarCellElement.AltShift3ID
                                                                '    oAltShift3Data.ShortName = sValue
                                                                '    oAltShift3Data.ID = iIdShift
                                                                'Case roCalendarCellLayout.eCalendarCellElement.AltShift3StartTime
                                                                '    oAltShift3Data.StartHour = dValue
                                                                'Case roCalendarCellLayout.eCalendarCellElement.AltShift3StartTime1
                                                                '    oAltShift3Data.ShiftLayersDefinition(0).LayerStartTime = dValue
                                                                'Case roCalendarCellLayout.eCalendarCellElement.AltShift3OrdinaryHours1
                                                                '    oAltShift3Data.ShiftLayersDefinition(0).LayerOrdinaryHours = fValue
                                                                'Case roCalendarCellLayout.eCalendarCellElement.AltShift3ComplementaryHours1
                                                                '    oAltShift3Data.ShiftLayersDefinition(0).LayerComplementaryHours = fValue
                                                                'Case roCalendarCellLayout.eCalendarCellElement.AltShift3StartTime2
                                                                '    oAltShift3Data.ShiftLayersDefinition(1).LayerStartTime = dValue
                                                                'Case roCalendarCellLayout.eCalendarCellElement.AltShift3OrdinaryHours1
                                                                '    oAltShift3Data.ShiftLayersDefinition(1).LayerOrdinaryHours = fValue
                                                                'Case roCalendarCellLayout.eCalendarCellElement.AltShift3ComplementaryHours2
                                                                '    oAltShift3Data.ShiftLayersDefinition(1).LayerComplementaryHours = fValue
                                                        End Select
                                                    Else
                                                        ' No encontré horario
                                                        Exit For
                                                    End If
                                                End If
                                            Next
                                            If bErrorOnDayData Then Exit For
                                        Next

                                        ' Ya he recogido toda la info del empleado/día. La proceso si no hay algún error y no está vacía ...
                                        If Not bErrorOnDayData AndAlso Not bEmptyDayData Then

                                            If Not oMainShiftData Is Nothing AndAlso oMainShiftData.ID = -1 Then oMainShiftData = Nothing
                                            If Not oAltShift1Data Is Nothing AndAlso oAltShift1Data.ID = -1 Then oAltShift1Data = Nothing
                                            If Not oAltShift2Data Is Nothing AndAlso oAltShift2Data.ID = -1 Then oAltShift2Data = Nothing
                                            If Not oAltShift3Data Is Nothing AndAlso oAltShift3Data.ID = -1 Then oAltShift3Data = Nothing
                                            If Not oBaseShiftData Is Nothing AndAlso oBaseShiftData.ID = -1 Then oBaseShiftData = Nothing

                                            If Not oAssignmentData Is Nothing AndAlso oAssignmentData.ID = -1 Then oAssignmentData = Nothing

                                            oDayData.PlanDate = dPlannedDate
                                            oDayData.HourData = roCalendarRowHourDataManager.LoadEmtyData()

                                            iIdShift = -1

                                            ' Horario Principal
                                            If Not oMainShiftData Is Nothing Then
                                                Dim oShiftDataDefinition As New roCalendarShift
                                                oShiftDataDefinition = oCalendarManager.LoadShiftDataByIdEx(oMainShiftData.ID)
                                                oMainShiftData = MergeShiftData(oShiftDataDefinition, oMainShiftData)
                                                If Not ValidateDayShiftData(oMainShiftData, oShiftDataDefinition) Then
                                                    'ExcelWriteSetAlarm(oImportFile, iRow, iColumn, Color.Red, Color.White, "")
                                                    ExcelWriteSetAlarm(oImportFile, iRow, iColumn, oCalendarCellLayout.Width, oCalendarCellLayout.Height, roCalendarCellLayout.eCalendarCellPattern.InvalidData, "")
                                                    LogError(oErrorList, CalendarErrorResultDayEnum.InvalidShiftDefinition, oCalResult.Status,, iIdEmployee, oDayData.PlanDate.ToShortDateString, sEmployeeName, oMainShiftData.ShortName)
                                                    bErrorOnDayData = True
                                                Else
                                                    iIdShift = oMainShiftData.ID
                                                End If
                                                oShiftDataDefinition = Nothing
                                            End If
                                            oDayData.MainShift = oMainShiftData

                                            ' Horario Base
                                            If Not oBaseShiftData Is Nothing Then
                                                Dim oShiftDataDefinition As New roCalendarShift
                                                oShiftDataDefinition = oCalendarManager.LoadShiftDataByIdEx(oBaseShiftData.ID)
                                                oBaseShiftData = MergeShiftData(oShiftDataDefinition, oBaseShiftData)
                                                If Not ValidateDayShiftData(oBaseShiftData, oShiftDataDefinition) Then
                                                    ExcelWriteSetAlarm(oImportFile, iRow, iColumn, oCalendarCellLayout.Width, oCalendarCellLayout.Height, roCalendarCellLayout.eCalendarCellPattern.InvalidData, "")
                                                    LogError(oErrorList, CalendarErrorResultDayEnum.InvalidShiftDefinition, oCalResult.Status,, iIdEmployee, oDayData.PlanDate.ToShortDateString, sEmployeeName, oBaseShiftData.ShortName)
                                                    bErrorOnDayData = True
                                                Else
                                                    iIdShift = oBaseShiftData.ID
                                                End If
                                                oShiftDataDefinition = Nothing
                                            End If
                                            oDayData.ShiftBase = oBaseShiftData

                                            'Puesto
                                            If Not oAssignmentData Is Nothing And iIdShift > 0 Then
                                                oAssignmentData = oCalendarManager.LoadAssignmentDataById(oAssignmentData.ID, iIdEmployee, iIdShift)

                                                If oAssignmentData Is Nothing Then
                                                    ExcelWriteSetAlarm(oImportFile, iRow, iColumn, oCalendarCellLayout.Width, oCalendarCellLayout.Height, roCalendarCellLayout.eCalendarCellPattern.InvalidData, "")
                                                    LogError(oErrorList, CalendarErrorResultDayEnum.InvalidAssignmentData, oCalResult.Status,, iIdEmployee, oDayData.PlanDate.ToShortDateString, sEmployeeName)
                                                    bErrorOnDayData = True
                                                End If
                                            End If
                                            oDayData.AssigData = oAssignmentData

                                            '--------------------------------------------------------------NO BORRAR-------------------------------------------------------------------------------------------------
                                            ' Primer alternativo
                                            'If Not oAltShift1Data Is Nothing Then
                                            '    Dim oShiftDataDefinition As New roCalendarShift
                                            '    oShiftDataDefinition = oCalendarManager.LoadShiftDataByIdEx(oAltShift1Data.ID)
                                            '    If ValidateDayShiftData(oAltShift1Data, oShiftDataDefinition) Then
                                            '        'Si se informó horario y todo es correcto, cargo definición de BBDD para informar el resto
                                            '        oAltShift1Data = MergeShiftData(oShiftDataDefinition, oAltShift1Data)
                                            '        oShiftDataDefinition = Nothing
                                            '    Else
                                            '        bErrorOnDayData = True
                                            '    End If
                                            'End If
                                            'oDayData.AltShift1 = oAltShift1Data

                                            ' Segundo alternativo
                                            'If Not oAltShift2Data Is Nothing Then
                                            '    Dim oShiftDataDefinition As New roCalendarShift
                                            '    oShiftDataDefinition = oCalendarManager.LoadShiftDataByIdEx(oAltShift2Data.ID)
                                            '    If ValidateDayShiftData(oAltShift2Data, oShiftDataDefinition) Then
                                            '        'Si se informó horario y todo es correcto, cargo definición de BBDD para informar el resto
                                            '        oAltShift2Data = MergeShiftData(oShiftDataDefinition, oAltShift2Data)
                                            '        oShiftDataDefinition = Nothing
                                            '    Else
                                            '        bErrorOnDayData = True
                                            '    End If
                                            'End If
                                            'oDayData.AltShift2 = oAltShift2Data

                                            ' Tercer alternativo
                                            'If Not oAltShift3Data Is Nothing Then
                                            '    Dim oShiftDataDefinition As New roCalendarShift
                                            '    oShiftDataDefinition = oCalendarManager.LoadShiftDataByIdEx(oAltShift3Data.ID)
                                            '    If ValidateDayShiftData(oAltShift3Data, oShiftDataDefinition) Then
                                            '        'Si se informó horario y todo es correcto, cargo definición de BBDD para informar el resto
                                            '        oAltShift3Data = MergeShiftData(oShiftDataDefinition, oAltShift3Data)
                                            '        oShiftDataDefinition = Nothing
                                            '    Else
                                            '        bErrorOnDayData = True
                                            '    End If
                                            'End If
                                            'oDayData.AltShift3 = oAltShift3Data

                                            If Not bErrorOnDayData AndAlso Not oCalendarManager.AddCalendarRowDayData(oCalendar, oDayData, iIdEmployee, bolCopyMainShifts, bolCopyHolidays, bolCopyAlternatives, bolKeepHolidays, bolKeepLockedDay, False, bolCopyMainShifts, bolCopyFeastDay, False, bolCopyTelecommute) Then
                                                ' Marco gráficamente el día/empleado como erróneo.
                                                Select Case oState.Result
                                                    Case CalendarV2ResultEnum.RowDayData_ShiftBaseShouldBeWorking
                                                        ExcelWriteSetAlarm(oImportFile, iRow, iColumn, oCalendarCellLayout.Width, oCalendarCellLayout.Height, roCalendarCellLayout.eCalendarCellPattern.InvalidData, "")
                                                        LogError(oErrorList, CalendarErrorResultDayEnum.InvalidAreWorkingDay, oCalResult.Status,, iIdEmployee, oDayData.PlanDate, sEmployeeName)
                                                    Case CalendarV2ResultEnum.RowDayData_EmployeeNotExist
                                                        'Marco empleado como no encontrado
                                                        'ExcelWriteSetAlarm(oImportFile, iRow, 1, Color.DarkOrange, Color.White, "")
                                                        ExcelWriteSetAlarm(oImportFile, iRow, 1, 2, oCalendarCellLayout.Height, roCalendarCellLayout.eCalendarCellPattern.InvalidData, "")
                                                        'Marco celda como ignorada
                                                        ExcelWriteSetAlarm(oImportFile, iRow, iColumn, oCalendarCellLayout.Width, oCalendarCellLayout.Height, roCalendarCellLayout.eCalendarCellPattern.Ignored, "")
                                                        LogError(oErrorList, CalendarErrorResultDayEnum.EmployeeDoesNotExist, oCalResult.Status, True, iIdEmployee, oDayData.PlanDate, sEmployeeName)
                                                    Case CalendarV2ResultEnum.RowDayData_PlannedDayNotExist
                                                        'ExcelWriteSetAlarm(oImportFile, 2, iColumn, Color.Orange, Color.White, "")
                                                        'Marco el día como no encontrado
                                                        ExcelWriteSetAlarm(oImportFile, 2, iColumn, oCalendarCellLayout.Width, 1, roCalendarCellLayout.eCalendarCellPattern.Ignored, "")
                                                        'Marco celda como ignorada
                                                        ExcelWriteSetAlarm(oImportFile, iRow, iColumn, oCalendarCellLayout.Width, oCalendarCellLayout.Height, roCalendarCellLayout.eCalendarCellPattern.Ignored, "")
                                                        LogError(oErrorList, CalendarErrorResultDayEnum.DateOutOfScope, oCalResult.Status, True, iIdEmployee, oDayData.PlanDate, sEmployeeName)
                                                    Case CalendarV2ResultEnum.RowDayData_ShiftBaseRequired
                                                        'ExcelWriteSetAlarm(oImportFile, 2, iColumn, Color.Red, Color.White, "")
                                                        ExcelWriteSetAlarm(oImportFile, iRow, iColumn, oCalendarCellLayout.Width, oCalendarCellLayout.Height, roCalendarCellLayout.eCalendarCellPattern.InvalidData, "")
                                                        LogError(oErrorList, CalendarErrorResultDayEnum.InvalidShiftBase, oCalResult.Status,, iIdEmployee, oDayData.PlanDate, sEmployeeName)
                                                    Case CalendarV2ResultEnum.RowDayData_ShiftNotExist
                                                        'ExcelWriteSetAlarm(oImportFile, iRow, iColumn, Color.Red, Color.White, sValue)
                                                        ExcelWriteSetAlarm(oImportFile, iRow, iColumn, oCalendarCellLayout.Width, oCalendarCellLayout.Height, roCalendarCellLayout.eCalendarCellPattern.InvalidData, "")
                                                        LogError(oErrorList, CalendarErrorResultDayEnum.ShiftNotExist, oCalResult.Status,, iIdEmployee, oDayData.PlanDate, sEmployeeName)
                                                    Case CalendarV2ResultEnum.InValidData
                                                        'ExcelWriteSetAlarm(oImportFile, iRow, iColumn, Color.Red, Color.White, sValue)
                                                        ExcelWriteSetAlarm(oImportFile, iRow, iColumn, oCalendarCellLayout.Width, oCalendarCellLayout.Height, roCalendarCellLayout.eCalendarCellPattern.InvalidData, "")
                                                        LogError(oErrorList, CalendarErrorResultDayEnum.GenericInvalidData, oCalResult.Status,, iIdEmployee, oDayData.PlanDate, sEmployeeName)
                                                    Case CalendarV2ResultEnum.ConnectionError, CalendarV2ResultEnum.Exception
                                                        'ExcelWriteSetAlarm(oImportFile, iRow, iColumn, Color.Red, Color.White, sValue)
                                                        ExcelWriteSetAlarm(oImportFile, iRow, iColumn, oCalendarCellLayout.Width, oCalendarCellLayout.Height, roCalendarCellLayout.eCalendarCellPattern.InvalidData, "")
                                                        LogError(oErrorList, CalendarErrorResultDayEnum.UnknownError, oCalResult.Status,, iIdEmployee, oDayData.PlanDate, sEmployeeName)
                                                    Case CalendarV2ResultEnum.RowDayData_PermissionDeniedOverEmployee
                                                        'ExcelWriteSetAlarm(oImportFile, iRow, iColumn, Color.Yellow, Color.White, sValue)
                                                        ExcelWriteSetAlarm(oImportFile, iRow, iColumn, oCalendarCellLayout.Width, oCalendarCellLayout.Height, roCalendarCellLayout.eCalendarCellPattern.Ignored, "")
                                                        LogError(oErrorList, CalendarErrorResultDayEnum.PermissionDenied, oCalResult.Status, True, iIdEmployee, oDayData.PlanDate, sEmployeeName)
                                                        bWarning = True
                                                    Case CalendarV2ResultEnum.RowDayData_EmployeeWithoutContractOnDate
                                                        'ExcelWriteSetAlarm(oImportFile, iRow, iColumn, Color.Yellow, Color.White, sValue)
                                                        ExcelWriteSetAlarm(oImportFile, iRow, iColumn, oCalendarCellLayout.Width, oCalendarCellLayout.Height, roCalendarCellLayout.eCalendarCellPattern.Ignored, "")
                                                        LogError(oErrorList, CalendarErrorResultDayEnum.NoContract, oCalResult.Status, True, iIdEmployee, oDayData.PlanDate, sEmployeeName)
                                                        bWarning = True
                                                    Case CalendarV2ResultEnum.RowDayData_AssignmentDataInvalid
                                                        'ExcelWriteSetAlarm(oImportFile, iRow, iColumn, Color.Yellow, Color.White, sValue)
                                                        ExcelWriteSetAlarm(oImportFile, iRow, iColumn, oCalendarCellLayout.Width, oCalendarCellLayout.Height, roCalendarCellLayout.eCalendarCellPattern.Ignored, "")
                                                        LogError(oErrorList, CalendarErrorResultDayEnum.InvalidAssignmentData, oCalResult.Status, True, iIdEmployee, oDayData.PlanDate, sEmployeeName)
                                                        bWarning = True

                                                End Select
                                            End If
                                        End If
                                    Else
                                        ' Error. No se pudo recoger el empleado. Marco ...
                                        'ExcelWriteSetAlarm(oImportFile, iRow, 1, Color.Red, Color.White, "")
                                        ExcelWriteSetAlarm(oImportFile, iRow, 1, 2, oCalendarCellLayout.Height, roCalendarCellLayout.eCalendarCellPattern.InvalidData, "")
                                        'Marco toda la fila como ignorada
                                        ExcelWriteSetAlarm(oImportFile, iRow, iColumn, oCalendarCellLayout.Width, oCalendarCellLayout.Height, roCalendarCellLayout.eCalendarCellPattern.Ignored, "")
                                        'LogError(oErrorList, CalendarErrorResultDayEnum.UnknownEmployee, oCalResult.Status, ,, oDayData.PlanDate, sEmployeeName & "-" & sEmployeeKey)
                                        LogError(oErrorList, CalendarErrorResultDayEnum.UnknownEmployee, oCalResult.Status, ,, oDayData.PlanDate, sEmployeeName & " ¿?")
                                    End If
                                    ' Si es una plantilla, no leo más
                                    If bExcelIsATemplate Then Exit For
                                Next
                            Next
                        Next

                        Try
                            ' Guardamos en Cloud
                            strFileNameError = "ImportCalendarErrors" & "_" & Now.Ticks.ToString & ".xlsx"
                            Dim ostream As New System.IO.MemoryStream
                            oImportFile.SaveAs(ostream)
                            Azure.RoAzureSupport.SaveFileOnAzure(ostream.ToArray(), strFileNameError, DTOs.roLiveQueueTypes.datalink)
                            strFileNameError = Azure.RoAzureSupport.GetCompanyName & "/" & strFileNameError
                        Catch ex As Exception
                            Me.oState.UpdateStateInfo(ex, "roCalendarManager::Error saving import log")
                        End Try
                    Else
                        ' No pude cargar plantilla
                        LogError(oErrorList, CalendarErrorResultDayEnum.GenericError, oCalResult.Status, ,,,,, Me.oState.Language.Translate("CalendarRowPeriodDataManager.Validate.ExcelCellProfileNotFound", ""))
                    End If
                Else
                    ' El fichero no parece válido
                    If Not bExcelIsATemplate Then
                        LogError(oErrorList, CalendarErrorResultDayEnum.GenericError, oCalResult.Status, ,,,,, Me.oState.Language.Translate("CalendarRowPeriodDataManager.Validate.InvalidImportFile", ""))
                    Else
                        LogError(oErrorList, CalendarErrorResultDayEnum.GenericError, oCalResult.Status, ,,,,, Me.oState.Language.Translate("CalendarRowPeriodDataManager.Validate.InvalidImportTemplateFile", ""))
                    End If
                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarManager::ImportFromExcelToScreen")
                LogError(oErrorList, CalendarErrorResultDayEnum.GenericError, oCalResult.Status, ,,,,, Me.oState.Language.Translate("CalendarRowPeriodDataManager.Validate.UnknownError", "") & " " & ex.Message)
            Finally
                If oErrorList.Count > 0 Then
                    oCalResult.CalendarDataResult = oErrorList.ToArray
                Else
                    oCalResult.Status = CalendarStatusEnum.OK
                End If

            End Try

            Return oCalendar
        End Function

        Public Function ImportFromASCIIToScreen(ByVal arrFile As Byte(), ByVal bolCopyMainShifts As Boolean, ByVal bolCopyHolidays As Boolean,
                                                ByVal bolCopyAlternatives As Boolean, ByVal bolKeepHolidays As Boolean, ByVal bolKeepLockedDay As Boolean,
                                                ByRef strFileNameError As String, ByRef oCalResult As roCalendarResult, ByVal lstAssignments As String, ByVal bLoadChilds As Boolean,
                                                Optional strProfilePath As String = "", Optional oASCIIProfileBytes As Byte() = Nothing) As roCalendar
            ' Tratamiento extendido de errores
            Dim oErrorList As New Generic.List(Of roCalendarDataDayError)

            Dim sValue As String = String.Empty
            Dim fValue As Double = 0
            Dim dValue As DateTime
            Dim sAux As String

            Dim oDayData As New roCalendarRowDayData
            Dim oMainShiftData As New roCalendarRowShiftData
            Dim oBaseShiftData As New roCalendarRowShiftData
            Dim oAltShift1Data As New roCalendarRowShiftData
            Dim oAltShift2Data As New roCalendarRowShiftData
            Dim oAltShift3Data As New roCalendarRowShiftData
            Dim oAssignmentData As New roCalendarAssignmentCellData
            Dim dStartDate As Date = Date.MinValue
            Dim dEndDate As Date = Date.MinValue

            Dim oState As roCalendarState

            Dim oCalendarManager As roCalendarManager = Nothing
            Dim oCalendar As roCalendar = Nothing
            Dim iIdEmployee As Integer
            Dim iIdShift As Integer
            Dim sEmployeeKey As String
            Dim sEmployeeName As String
            Dim bErrorOnDayData As Boolean = False
            Dim bErrorDataFormat As Boolean = False
            Dim bNoShiftSpecified As Boolean = True
            Dim dEmployeeIDs As New Dictionary(Of String, Integer)
            Dim dShiftIDs As New Dictionary(Of String, Integer)
            Dim bWarning As Boolean = False
            Dim bolCopyFeastDay As Boolean = False
            Dim strShiftKey As String
            Dim dDate As Date

            Dim oImportFile As New System.IO.MemoryStream()
            Dim oSReader As IO.StreamReader = Nothing

            Try

                ' Creo el fichero ASCII de trabajo a partir del array
                oImportFile.Write(arrFile, 0, arrFile.Length)
                oImportFile.Position = 0
                oSReader = New IO.StreamReader(oImportFile, True)

                Dim tbEmployee As DataTable = Nothing
                Dim strLine As String = ""
                Dim strIDSAP As String = ""
                Dim strDate As String = ""

                ' Obtenemos los diferentes empleados del fichero y la fecha de inicio(MIN) y final(MAX)
                Do While Not oSReader.EndOfStream
                    strLine = oSReader.ReadLine()

                    ' Empleados
                    strIDSAP = roTypes.String2Item(strLine, 0, ";").Trim
                    If Not dEmployeeIDs.ContainsKey(strIDSAP) Then
                        iIdEmployee = -1
                        tbEmployee = VTUserFields.UserFields.roEmployeeUserField.GetIDEmployeesFromUserFieldValue("SAP HR", strIDSAP, Now, New VTUserFields.UserFields.roUserFieldState())
                        If tbEmployee.Rows.Count > 0 Then
                            iIdEmployee = tbEmployee.Rows(0).Item("IDEmployee")
                        End If
                        dEmployeeIDs.Add(strIDSAP, iIdEmployee)
                    End If

                    ' Fecha MIN y MAX
                    strDate = roTypes.String2Item(strLine, 1, ";").Trim 'yyyymmdd
                    If strDate.Length = 8 Then
                        Try
                            dDate = New Date(roTypes.Any2Integer(Left(strDate, 4)), roTypes.Any2Integer(strDate.Substring(4, 2)), roTypes.Any2Integer(Right(strDate, 2)))
                            If dStartDate = Date.MinValue Then
                                dStartDate = dDate
                            Else
                                If dDate < dStartDate Then
                                    dStartDate = dDate
                                End If
                            End If
                            If dEndDate = Date.MinValue Then
                                dEndDate = dDate
                            Else
                                If dDate > dEndDate Then
                                    dEndDate = dDate
                                End If
                            End If
                        Catch ex As Exception
                        End Try
                    End If
                Loop

                ' Si la fecha MIN o MAX del periodo es incorrecta, no importamos el fichero
                If dStartDate = Date.MinValue OrElse dEndDate = Date.MinValue Then
                    bErrorDataFormat = True
                End If

                If Not bErrorDataFormat Then
                    'Cargo datos de base de datos, con los empleados y fechas del fichero a importar
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roCalendarManager::ImportFromASCIIToScreen: LoadCalendar: " & dStartDate.ToShortDateString & "," & dEndDate.ToShortDateString & ", Employees: " & String.Join(",", dEmployeeIDs.Values))

                    oState = New roCalendarState(Me.oState.IDPassport)
                    oCalendarManager = New roCalendarManager(oState)
                    oCalendar = oCalendarManager.Load(dStartDate, dEndDate, "B" & String.Join(",", dEmployeeIDs.Values).Replace(",", ",B"), CalendarView.Planification, CalendarDetailLevel.Daily, False)

                    ' Volvemos al inicio del fichero
                    oSReader.DiscardBufferedData()
                    oSReader.BaseStream.Seek(0, IO.SeekOrigin.Begin)
                    oSReader.BaseStream.Position = 0

                    Do While Not oSReader.EndOfStream
                        ' Para cada registro
                        bErrorOnDayData = False
                        bErrorDataFormat = False

                        iIdEmployee = -1
                        strDate = ""
                        strShiftKey = ""
                        sEmployeeName = ""
                        sEmployeeKey = ""

                        strLine = oSReader.ReadLine()

                        ' Validamos que contenga mínimo los 3 campos obligatorios
                        If roTypes.StringItemsCount(strLine, ";") < 3 Then
                            bErrorDataFormat = True
                        Else
                            ' Validamos y obtenemos la fecha de planificación
                            strDate = roTypes.String2Item(strLine, 1, ";").Trim 'yyyymmdd
                            Try
                                dDate = New Date(roTypes.Any2Integer(Left(strDate, 4)), roTypes.Any2Integer(strDate.Substring(4, 2)), roTypes.Any2Integer(Right(strDate, 2)))
                            Catch ex As Exception
                                dDate = Date.MinValue
                                bErrorDataFormat = True
                            End Try
                        End If

                        If Not bErrorDataFormat Then
                            ' Si no hay error en el formato de la linea,

                            ' Empleado
                            strIDSAP = roTypes.String2Item(strLine, 0, ";").Trim
                            iIdEmployee = dEmployeeIDs(strIDSAP)
                            If iIdEmployee = -1 Then
                                bErrorOnDayData = True
                            End If
                            sEmployeeName = strIDSAP
                            sEmployeeKey = strIDSAP

                            ' Horario
                            strShiftKey = roTypes.String2Item(strLine, 2, ";").Trim

                            ' Inicializamos los valores del dia
                            oDayData.MainShift = New roCalendarRowShiftData
                            oDayData.AltShift1 = New roCalendarRowShiftData
                            oDayData.AltShift2 = New roCalendarRowShiftData
                            oDayData.AltShift3 = New roCalendarRowShiftData
                            oDayData.ShiftBase = New roCalendarRowShiftData
                            oMainShiftData = New roCalendarRowShiftData
                            oBaseShiftData = New roCalendarRowShiftData
                            oAltShift1Data = New roCalendarRowShiftData
                            oAltShift2Data = New roCalendarRowShiftData
                            oAltShift3Data = New roCalendarRowShiftData
                            oAssignmentData = New roCalendarAssignmentCellData

                            ' Recupero id de horario si no lo tenía
                            iIdShift = -1
                            bNoShiftSpecified = False
                            If Not dShiftIDs.ContainsKey(strShiftKey) Then
                                iIdShift = VTBusiness.Shift.roShift.GetShiftIdByExportKey(strShiftKey)
                                dShiftIDs.Add(strShiftKey, iIdShift)
                            End If
                            If Not dShiftIDs.TryGetValue(strShiftKey, iIdShift) Then
                                bErrorOnDayData = True
                            End If
                            If iIdShift = -1 Then
                                If Not strShiftKey.Trim = String.Empty Then
                                    LogError(oErrorList, CalendarErrorResultDayEnum.ShiftNotExist, oCalResult.Status,, iIdEmployee, dDate, sEmployeeName, strShiftKey)
                                    bErrorOnDayData = True
                                Else
                                    bNoShiftSpecified = True
                                End If
                            End If
                        End If

                        If bErrorDataFormat Then
                            ' Si hay error en el formato de la linea a importar
                            LogError(oErrorList, CalendarErrorResultDayEnum.GenericError, oCalResult.Status,, iIdEmployee, Date.MinValue, sEmployeeName, strShiftKey, strLine & " - Incorrect format line")

                        ElseIf Not bErrorOnDayData OrElse bNoShiftSpecified Then
                            ' Horario
                            oMainShiftData.ShortName = strShiftKey
                            oMainShiftData.ID = iIdShift

                            ' Hora de inicio del horario
                            sAux = roTypes.String2Item(strLine, 3, ";").Trim
                            If sAux.Length > 0 Then
                                dValue = GetParseAsciiDateTimeValue(sAux)
                                oMainShiftData.StartHour = dValue

                                ' Hora de inicio primera franja
                                InitShiftLayersDefinitionDataIfNeeded(oMainShiftData)
                                oMainShiftData.ShiftLayersDefinition(0).LayerStartTime = dValue
                            End If

                            'Horas ordinarias primera franja
                            sAux = roTypes.String2Item(strLine, 4, ";").Trim
                            dValue = Date.MinValue
                            If sAux.Length > 0 Then
                                ' Horas ordinarias primera franja
                                InitShiftLayersDefinitionDataIfNeeded(oMainShiftData)
                                dValue = GetParseAsciiDateTimeValue(sAux)
                            End If
                            If dValue <> Date.MinValue Then oMainShiftData.ShiftLayersDefinition(0).LayerOrdinaryHours = dValue.Hour * 60 + dValue.Minute

                            ' Hora de inicio segunda franja
                            sAux = roTypes.String2Item(strLine, 5, ";").Trim
                            dValue = Date.MinValue
                            If sAux.Length > 0 Then
                                InitShiftLayersDefinitionDataIfNeeded(oMainShiftData)
                                dValue = GetParseAsciiDateTimeValue(sAux)
                                oMainShiftData.ShiftLayersDefinition(1).LayerStartTime = dValue
                            End If

                            ' Horas ordinarias segunda franja
                            sAux = roTypes.String2Item(strLine, 6, ";").Trim
                            dValue = Date.MinValue
                            If sAux.Length > 0 Then
                                ' Horas ordinarias primera franja
                                InitShiftLayersDefinitionDataIfNeeded(oMainShiftData)
                                dValue = GetParseAsciiDateTimeValue(sAux)
                            End If
                            If dValue <> Date.MinValue Then oMainShiftData.ShiftLayersDefinition(1).LayerOrdinaryHours = dValue.Hour * 60 + dValue.Minute

                            ' Hora de inicio horas complementarias (No se tratan)
                            'sAux = roTypes.String2Item(strLine, 7, ";").Trim
                            'dValue = Date.MinValue
                            'If sAux.Length > 0 Then
                            '    ' Horas ordinarias primera franja
                            '    InitShiftLayersDefinitionDataIfNeeded(oMainShiftData)
                            '    dValue = GetParseAsciiDateTimeValue(sAux)
                            'End If
                            'If dValue <> Date.MinValue Then oMainShiftData.ShiftLayersDefinition(0).LayerComplementaryHours = dValue.Hour * 60 + dValue.Minute

                            ' Horas complementarias primera franja
                            sAux = roTypes.String2Item(strLine, 8, ";").Trim
                            dValue = Date.MinValue
                            If sAux.Length > 0 Then
                                ' Horas ordinarias primera franja
                                InitShiftLayersDefinitionDataIfNeeded(oMainShiftData)
                                dValue = GetParseAsciiDateTimeValue(sAux)
                            End If
                            If dValue <> Date.MinValue Then oMainShiftData.ShiftLayersDefinition(0).LayerComplementaryHours = dValue.Hour * 60 + dValue.Minute

                            If Not oMainShiftData Is Nothing AndAlso oMainShiftData.ID = -1 Then oMainShiftData = Nothing

                            oAltShift1Data = Nothing
                            oAltShift2Data = Nothing
                            oAltShift3Data = Nothing
                            oBaseShiftData = Nothing
                            oAssignmentData = Nothing
                            oDayData.PlanDate = dDate
                            oDayData.HourData = roCalendarRowHourDataManager.LoadEmtyData()

                            iIdShift = -1
                            ' Horario Principal
                            If Not oMainShiftData Is Nothing Then
                                Dim oShiftDataDefinition As New roCalendarShift
                                oShiftDataDefinition = oCalendarManager.LoadShiftDataByIdEx(oMainShiftData.ID)
                                oMainShiftData = MergeShiftData(oShiftDataDefinition, oMainShiftData)
                                If Not ValidateDayShiftData(oMainShiftData, oShiftDataDefinition) Then
                                    LogError(oErrorList, CalendarErrorResultDayEnum.InvalidShiftDefinition, oCalResult.Status,, iIdEmployee, oDayData.PlanDate.ToShortDateString, sEmployeeName, oMainShiftData.ShortName)
                                    bErrorOnDayData = True
                                Else
                                    iIdShift = oMainShiftData.ID
                                End If
                                oShiftDataDefinition = Nothing
                            End If
                            oDayData.MainShift = oMainShiftData
                            oDayData.ShiftBase = oBaseShiftData
                            oDayData.AssigData = oAssignmentData

                            If Not bErrorOnDayData AndAlso Not oCalendarManager.AddCalendarRowDayData(oCalendar, oDayData, iIdEmployee, bolCopyMainShifts, bolCopyHolidays, bolCopyAlternatives, bolKeepHolidays, bolKeepLockedDay, False, bolCopyMainShifts, bolCopyFeastDay, False) Then
                                ' Marco gráficamente el día/empleado como erróneo.
                                Select Case oState.Result
                                    Case CalendarV2ResultEnum.RowDayData_ShiftBaseShouldBeWorking
                                        LogError(oErrorList, CalendarErrorResultDayEnum.InvalidAreWorkingDay, oCalResult.Status,, iIdEmployee, oDayData.PlanDate, sEmployeeName)
                                    Case CalendarV2ResultEnum.RowDayData_EmployeeNotExist
                                        'Marco empleado como no encontrado
                                        LogError(oErrorList, CalendarErrorResultDayEnum.EmployeeDoesNotExist, oCalResult.Status, True, iIdEmployee, oDayData.PlanDate, sEmployeeName)
                                    Case CalendarV2ResultEnum.RowDayData_PlannedDayNotExist
                                        'Marco el día como no encontrado
                                        LogError(oErrorList, CalendarErrorResultDayEnum.DateOutOfScope, oCalResult.Status, True, iIdEmployee, oDayData.PlanDate, sEmployeeName)
                                    Case CalendarV2ResultEnum.RowDayData_ShiftBaseRequired
                                        LogError(oErrorList, CalendarErrorResultDayEnum.InvalidShiftBase, oCalResult.Status,, iIdEmployee, oDayData.PlanDate, sEmployeeName)
                                    Case CalendarV2ResultEnum.RowDayData_ShiftNotExist
                                        LogError(oErrorList, CalendarErrorResultDayEnum.ShiftNotExist, oCalResult.Status,, iIdEmployee, oDayData.PlanDate, sEmployeeName)
                                    Case CalendarV2ResultEnum.InValidData
                                        LogError(oErrorList, CalendarErrorResultDayEnum.GenericInvalidData, oCalResult.Status,, iIdEmployee, oDayData.PlanDate, sEmployeeName)
                                    Case CalendarV2ResultEnum.ConnectionError, CalendarV2ResultEnum.Exception
                                        LogError(oErrorList, CalendarErrorResultDayEnum.UnknownError, oCalResult.Status,, iIdEmployee, oDayData.PlanDate, sEmployeeName)
                                    Case CalendarV2ResultEnum.RowDayData_PermissionDeniedOverEmployee
                                        LogError(oErrorList, CalendarErrorResultDayEnum.PermissionDenied, oCalResult.Status, True, iIdEmployee, oDayData.PlanDate, sEmployeeName)
                                        bWarning = True
                                    Case CalendarV2ResultEnum.RowDayData_EmployeeWithoutContractOnDate
                                        LogError(oErrorList, CalendarErrorResultDayEnum.NoContract, oCalResult.Status, True, iIdEmployee, oDayData.PlanDate, sEmployeeName)
                                        bWarning = True
                                    Case CalendarV2ResultEnum.RowDayData_AssignmentDataInvalid
                                        LogError(oErrorList, CalendarErrorResultDayEnum.InvalidAssignmentData, oCalResult.Status, True, iIdEmployee, oDayData.PlanDate, sEmployeeName)
                                        bWarning = True
                                End Select
                            End If
                        ElseIf iIdEmployee = -1 Then
                            ' Error. No se pudo recoger el empleado.
                            LogError(oErrorList, CalendarErrorResultDayEnum.UnknownEmployee, oCalResult.Status, ,, dDate, sEmployeeKey)
                        End If
                    Loop
                Else
                    LogError(oErrorList, CalendarErrorResultDayEnum.GenericError, oCalResult.Status, ,,,,, "Incorrect date period:" & dStartDate.ToShortDateString & " " & dEndDate.ToShortDateString)
                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarManager::ImportFromASCIIToScreen")
                LogError(oErrorList, CalendarErrorResultDayEnum.GenericError, oCalResult.Status, ,,,,, Me.oState.Language.Translate("CalendarRowPeriodDataManager.Validate.UnknownError", "") & " " & ex.Message)
            Finally
                If oErrorList.Count > 0 Then
                    oCalResult.CalendarDataResult = oErrorList.ToArray
                Else
                    oCalResult.Status = CalendarStatusEnum.OK
                End If

                If oImportFile IsNot Nothing Then oImportFile.Close()
                If oSReader IsNot Nothing Then oSReader.Close()
            End Try

            Return oCalendar
        End Function

        Public Sub LogError(ByRef oErrorList As Generic.List(Of roCalendarDataDayError), eErrorCode As CalendarErrorResultDayEnum, ByRef oCalResultStatus As CalendarStatusEnum, Optional bOnlyWarning As Boolean = False, Optional iIdEmployee As Integer = 0, Optional dDate As Date = Nothing, Optional sEmployeeName As String = "", Optional sShiftExportName As String = "", Optional sErrorText As String = "")
            Dim oerror As New roCalendarDataDayError
            Try
                oerror.ErrorCode = eErrorCode
                If bOnlyWarning Then
                    If oCalResultStatus = CalendarStatusEnum.OK Then oCalResultStatus = CalendarStatusEnum.WARNING
                Else
                    oCalResultStatus = CalendarStatusEnum.KO
                End If

                Select Case eErrorCode
                    Case CalendarErrorResultDayEnum.DateOutOfScope
                        oerror.ErrorText = sEmployeeName & " " & Me.oState.Language.Translate("CalendarRowPeriodDataManager.Validate.DateOutOfScope", "") & " " & dDate.ToShortDateString
                    Case CalendarErrorResultDayEnum.EmployeeDoesNotExist
                        oerror.ErrorText = sEmployeeName & " " & Me.oState.Language.Translate("CalendarRowPeriodDataManager.Validate.EmployeeDoesNotExist", "") & " " & dDate.ToShortDateString
                    Case CalendarErrorResultDayEnum.UnknownEmployee
                        oerror.ErrorText = sEmployeeName & " " & Me.oState.Language.Translate("CalendarRowPeriodDataManager.Validate.UnknownEmployee", "")
                    Case CalendarErrorResultDayEnum.FreezingDate
                        oerror.ErrorText = sEmployeeName & " " & Me.oState.Language.Translate("CalendarRowPeriodDataManager.Validate.FreezingDate", "") & " " & dDate.ToShortDateString
                    Case CalendarErrorResultDayEnum.GenericInvalidData
                        oerror.ErrorText = sEmployeeName & " " & Me.oState.Language.Translate("CalendarRowPeriodDataManager.Validate.GenericInvalidData", "") & " " & dDate.ToShortDateString
                    Case CalendarErrorResultDayEnum.InvalidAreWorkingDay
                        oerror.ErrorText = sEmployeeName & " " & Me.oState.Language.Translate("CalendarRowPeriodDataManager.Validate.InvalidAreWokringDay", "") & " " & dDate.ToShortDateString
                    Case CalendarErrorResultDayEnum.InvalidComplementaryData
                        oerror.ErrorText = sEmployeeName & " " & Me.oState.Language.Translate("CalendarRowPeriodDataManager.Validate.InvalidComplementaryData", "") & " " & dDate.ToShortDateString & "-" & sShiftExportName
                    Case CalendarErrorResultDayEnum.InvalidFloatingData
                        oerror.ErrorText = sEmployeeName & " " & Me.oState.Language.Translate("CalendarRowPeriodDataManager.Validate.InvalidFloatingData", "") & " " & dDate.ToShortDateString & "-" & sShiftExportName
                    Case CalendarErrorResultDayEnum.InvalidShiftBase
                        oerror.ErrorText = sEmployeeName & " " & Me.oState.Language.Translate("CalendarRowPeriodDataManager.Validate.InvalidShiftBase", "") & " " & dDate.ToShortDateString
                    Case CalendarErrorResultDayEnum.InvalidStartFloating
                        oerror.ErrorText = sEmployeeName & " " & Me.oState.Language.Translate("CalendarRowPeriodDataManager.Validate.InvalidStartFloating", "") & " " & dDate.ToShortDateString & "-" & sShiftExportName
                    Case CalendarErrorResultDayEnum.NoContract
                        oerror.ErrorText = sEmployeeName & " " & Me.oState.Language.Translate("CalendarRowPeriodDataManager.Validate.NoContract", "") & " " & dDate.ToShortDateString
                    Case CalendarErrorResultDayEnum.PermissionDenied
                        oerror.ErrorText = sEmployeeName & " " & Me.oState.Language.Translate("CalendarRowPeriodDataManager.Validate.PermissionDenied", "") & " " & dDate.ToShortDateString
                    Case CalendarErrorResultDayEnum.ShiftNotExist
                        oerror.ErrorText = sEmployeeName & " " & Me.oState.Language.Translate("CalendarRowPeriodDataManager.Validate.ShiftNotExist", "") & " " & dDate.ToShortDateString & "-" & sShiftExportName
                    Case CalendarErrorResultDayEnum.ShiftWithoutPermission
                        oerror.ErrorText = sEmployeeName & " " & Me.oState.Language.Translate("CalendarRowPeriodDataManager.Validate.ShiftWithoutPermission", "") & " " & dDate.ToShortDateString
                    Case CalendarErrorResultDayEnum.UnknownError
                        oerror.ErrorText = sEmployeeName & " " & Me.oState.Language.Translate("CalendarRowPeriodDataManager.Validate.UnknownError", "") & " " & dDate.ToShortDateString
                    Case CalendarErrorResultDayEnum.InvalidShiftDefinition
                        oerror.ErrorText = sEmployeeName & " " & Me.oState.Language.Translate("CalendarRowPeriodDataManager.Validate.InvalidShiftDefinition", "") & " " & dDate.ToShortDateString & "-" & sShiftExportName
                    Case CalendarErrorResultDayEnum.InvalidAssignmentData
                        oerror.ErrorText = sEmployeeName & " " & Me.oState.Language.Translate("CalendarRowPeriodDataManager.Validate.InvalidAssignmentData", "") & " " & dDate.ToShortDateString
                    Case CalendarErrorResultDayEnum.GenericError
                        oerror.ErrorText = sErrorText
                End Select
                If oerror.ErrorText.Trim.Length > 0 Then
                    oerror.ErrorText = oerror.ErrorText & vbNewLine
                End If
                oerror.IDEmployee = iIdEmployee
                oerror.ErrorDate = dDate
                oErrorList.Add(oerror)
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarManager::LogError")
            End Try
        End Sub

        Private Function ValidateDayAssignmentData(ByVal oCalendarRowEmployeeData As roCalendarRowEmployeeData, ByVal oCalendarRowDayData As roCalendarRowDayData) As Boolean

            Dim bolRet As Boolean = True
            Dim bContinueChecking As Boolean = True

            Try

                ' Si no hay puesto asignado no hacemos nada
                If oCalendarRowDayData Is Nothing OrElse oCalendarRowDayData.AssigData Is Nothing OrElse oCalendarRowDayData.AssigData.ID = 0 Then
                    Return True
                    Exit Function
                End If

                ' Obtenemos el horario al que esta asignado el puesto, puede ser el principal o el base
                ' Si tiene vacaciones el puesto lo tiene asignado el base
                Dim _IDShift As Integer = 0
                If Not oCalendarRowDayData.ShiftBase Is Nothing Then
                    If oCalendarRowDayData.ShiftBase.ID > 0 Then _IDShift = oCalendarRowDayData.ShiftBase.ID
                End If
                If _IDShift = 0 Then
                    If Not oCalendarRowDayData.MainShift Is Nothing Then
                        If oCalendarRowDayData.MainShift.ID > 0 Then _IDShift = oCalendarRowDayData.MainShift.ID
                    End If
                End If

                ' Validamos si el empleado tiene asignado el Puesto
                Dim bolExistShiftAssgnment As Boolean = False
                If Not oCalendarRowEmployeeData.Assignments Is Nothing Then
                    For Each oAssignments As roCalendarAssignmentData In oCalendarRowEmployeeData.Assignments
                        If oAssignments.ID = oCalendarRowDayData.AssigData.ID Then
                            bolExistShiftAssgnment = True
                            Exit For
                        End If

                    Next
                End If

                If bolExistShiftAssgnment Then
                    ' Validamos si el horario tiene asignado el puesto
                    bolExistShiftAssgnment = False
                    If _IDShift > 0 Then
                        Dim oShiftState As New roShiftState(oState.IDPassport)
                        bolExistShiftAssgnment = roShiftAssignment.ExistShiftAssignment(_IDShift, oCalendarRowDayData.AssigData.ID, oShiftState)
                    End If
                End If

                bolRet = bolExistShiftAssgnment
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roCalendarManager::ValidateDayAssignmentData")
                bolRet = False
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarManager::ValidateDayAssignmentData")
                bolRet = False
            Finally

            End Try

            Return bolRet
        End Function

        ''' <summary>
        ''' Validación de la integridad de los datos introducidos en el fichero Excel. No incluye validaciones funcionales
        ''' </summary>
        ''' <param name="oDayShiftData"></param>
        ''' <returns></returns>
        Private Function ValidateDayShiftData(ByRef oDayShiftData As roCalendarRowShiftData, oShiftDataDefinition As roCalendarShift) As Boolean
            Dim oRet As Boolean = False
            Try
                ' Validaciones a realizar
                ' 1.- Si tiene dos franjas, están ordenadas
                If oDayShiftData.ShiftLayers > 1 Then
                    If Not oDayShiftData.ShiftLayersDefinition(0).LayerStartTime < oDayShiftData.ShiftLayersDefinition(1).LayerStartTime Then
                        Return False
                    End If
                End If

                ' 2.0.- Hay parámetros de horarios por horas (inicio de franja, ordinarias o complementarias) en un horario que no lo permite
                ' TODO: Ahora no se puede hacer porque la función Merge elimina las layers si el teórico no lo permite. Se debería hacer allí. No es problema porque los posibles datos se ignoran

                ' 2.- Hay complementarias en un horario que no las permite (pero tiene alguna característica dinámica de franjas
                If Not oShiftDataDefinition.AllowComplementary Then
                    If Not oDayShiftData.ShiftLayersDefinition Is Nothing Then
                        Try
                            If oDayShiftData.ShiftLayers > 0 Then
                                If oDayShiftData.ShiftLayersDefinition(0).LayerComplementaryHours <> -1 AndAlso oDayShiftData.ShiftLayersDefinition(0).LayerComplementaryHours <> 0 Then
                                    Return False
                                End If
                            End If
                            If oDayShiftData.ShiftLayers > 1 Then
                                If oDayShiftData.ShiftLayersDefinition(1).LayerComplementaryHours <> -1 AndAlso oDayShiftData.ShiftLayersDefinition(1).LayerComplementaryHours <> 0 Then
                                    Return False
                                End If
                            End If
                        Catch ex As Exception
                            'Asumo que no hay franjas
                        End Try
                    End If
                End If

                ' 2.1.- Hay Ordinarias, y no se permite ni complementarias ni modificación de duración por ningún medio
                If Not (oShiftDataDefinition.AllowComplementary OrElse (oShiftDataDefinition.AllowModifyDuration1 OrElse (oShiftDataDefinition.AllowModifyIniHour1 AndAlso oShiftDataDefinition.HasLayer1FixedEnd))) Then
                    ' Sólo es necesario comprobar la primera franja de la definición porque la segunda tienen las mismas características
                    If Not oDayShiftData.ShiftLayersDefinition Is Nothing Then
                        Try
                            If oDayShiftData.ShiftLayers > 0 Then
                                If oDayShiftData.ShiftLayersDefinition(0).LayerOrdinaryHours <> -1 AndAlso oDayShiftData.ShiftLayersDefinition(0).LayerOrdinaryHours <> 0 AndAlso oDayShiftData.ShiftLayersDefinition(0).LayerOrdinaryHours <> oDayShiftData.ShiftLayersDefinition(0).LayerDuration Then
                                    Return False
                                End If
                            End If
                            If oDayShiftData.ShiftLayers > 1 Then
                                If oDayShiftData.ShiftLayersDefinition(1).LayerOrdinaryHours <> -1 AndAlso oDayShiftData.ShiftLayersDefinition(1).LayerOrdinaryHours <> 0 AndAlso oDayShiftData.ShiftLayersDefinition(1).LayerOrdinaryHours <> oDayShiftData.ShiftLayersDefinition(1).LayerDuration Then
                                    Return False
                                End If
                            End If
                        Catch ex As Exception
                            'Asumo que no hay franjas
                        End Try
                    End If
                End If

                ' 3.- La suma de ordinarias y complementarias, de existir, no difieren de la duración de su franja rígida, si es rígida
                ' 3.1.- Franja 1
                ' Primero veo si se puede modificar la duración de la franja, y en caso que no se pueda, su duración teórica.
                Dim bLayer1DurationCanBeModified As Boolean = False
                Dim bLayer2DurationCanBeModified As Boolean = False
                If oDayShiftData.ShiftLayers > 0 Then
                    bLayer1DurationCanBeModified = (oShiftDataDefinition.AllowModifyDuration1 OrElse (oShiftDataDefinition.AllowModifyIniHour1 AndAlso oShiftDataDefinition.HasLayer1FixedEnd))
                    If Not bLayer1DurationCanBeModified Then
                        If oShiftDataDefinition.AllowComplementary1 Then
                            ' C-M-FFL o C-F-F
                            ' Ordinarias + Complementarias no difieren de la duración de la franja fija (la duración se calcula en base a la teoría)
                            If oDayShiftData.ShiftLayersDefinition(0).LayerComplementaryHours + oDayShiftData.ShiftLayersDefinition(0).LayerOrdinaryHours <> DateDiff(DateInterval.Minute, oShiftDataDefinition.StartLayer1, oShiftDataDefinition.EndLayer1) Then
                                Return False
                            End If
                        Else
                            ' O-M-FFL
                            ' Ordinarias no difieren de la duración de la franja fija (la duración se calcula en base a la teoría)
                            If oDayShiftData.ShiftLayersDefinition(0).LayerOrdinaryHours <> DateDiff(DateInterval.Minute, oShiftDataDefinition.StartLayer1, oShiftDataDefinition.EndLayer1) Then
                                Return False
                            End If
                        End If
                    Else
                        '  PDTE PRUEBAS Se puede modificar la duración. Miro si se puede hacer moviendo el inicio, al tener un final fijo. Si es así, la duración se calcula diferente, pero no puede excederse
                        If Not oShiftDataDefinition.AllowModifyDuration1 Then
                            If oShiftDataDefinition.AllowComplementary1 Then
                                ' C-M-FFI
                                ' Ordinarias + Complementarias no difieren de la duración de la franja fija (la duración se calcula en base a la posible hora de inicio que puede haber cambiado)
                                If oDayShiftData.ShiftLayersDefinition(0).LayerComplementaryHours + oDayShiftData.ShiftLayersDefinition(0).LayerOrdinaryHours <> DateDiff(DateInterval.Minute, oDayShiftData.ShiftLayersDefinition(0).LayerStartTime, oShiftDataDefinition.EndLayer1) Then
                                    Return False
                                End If
                            Else
                                ' O-M-FFI
                                ' Ordinarias no difiere de la duración de la franja fija (la duración se calcula en base a la posible hora de inicio que puede haber cambiado)
                                If oDayShiftData.ShiftLayersDefinition(0).LayerOrdinaryHours <> DateDiff(DateInterval.Minute, oDayShiftData.ShiftLayersDefinition(0).LayerStartTime, oShiftDataDefinition.EndLayer1) Then
                                    Return False
                                End If
                            End If
                        End If
                    End If
                End If

                '3.2.- Franja 2
                If oDayShiftData.ShiftLayers > 1 Then
                    bLayer2DurationCanBeModified = (oShiftDataDefinition.AllowModifyDuration2 OrElse (oShiftDataDefinition.AllowModifyIniHour2 AndAlso oShiftDataDefinition.HasLayer2FixedEnd))
                    If Not bLayer2DurationCanBeModified Then
                        If oShiftDataDefinition.AllowComplementary2 Then
                            ' C-M-FFL o C-F-F
                            ' Ordinarias + Complementarias no difieren de la duración de la franja fija (la duración se calcula en base a la teoría)
                            If oDayShiftData.ShiftLayersDefinition(1).LayerComplementaryHours + oDayShiftData.ShiftLayersDefinition(1).LayerOrdinaryHours <> DateDiff(DateInterval.Minute, oShiftDataDefinition.StartLayer2, oShiftDataDefinition.EndLayer2) Then
                                Return False
                            End If
                        Else
                            ' O-M-FFL
                            ' Ordinarias no difieren de la duración de la franja fija (la duración se calcula en base a la teoría)
                            If oDayShiftData.ShiftLayersDefinition(1).LayerOrdinaryHours <> DateDiff(DateInterval.Minute, oShiftDataDefinition.StartLayer2, oShiftDataDefinition.EndLayer2) Then
                                Return False
                            End If
                        End If
                    Else
                        '  PDTE PRUEBAS Se puede modificar la duración. Miro si se puede hacer moviendo el inicio, al tener un final fijo. Si es así, la duración se calcula diferente, pero no puede excederse
                        If Not oShiftDataDefinition.AllowModifyDuration2 Then
                            If oShiftDataDefinition.AllowComplementary2 Then
                                ' C-M-FFI
                                '  Ordinarias + Complementarias no difieren de la duración de la franja fija (la duración se calcula en base a la posible hora de inicio que puede haber cambiado)
                                If oDayShiftData.ShiftLayersDefinition(1).LayerComplementaryHours + oDayShiftData.ShiftLayersDefinition(1).LayerOrdinaryHours <> DateDiff(DateInterval.Minute, oDayShiftData.ShiftLayersDefinition(1).LayerStartTime, oShiftDataDefinition.EndLayer2) Then
                                    Return False
                                End If
                            Else
                                ' O-M-FFI
                                ' Ordinarias no difiere de la duración de la franja fija (la duración se calcula en base a la posible hora de inicio que puede haber cambiado)
                                If oDayShiftData.ShiftLayersDefinition(1).LayerOrdinaryHours <> DateDiff(DateInterval.Minute, oDayShiftData.ShiftLayersDefinition(1).LayerStartTime, oShiftDataDefinition.EndLayer2) Then
                                    Return False
                                End If
                            End If
                        End If
                    End If
                End If

                ' 4.- Las franjas, de haber más de 1, no se solapan
                If oDayShiftData.ShiftLayers > 1 Then
                    If DateAdd(DateInterval.Minute, oDayShiftData.ShiftLayersDefinition(0).LayerDuration, oDayShiftData.ShiftLayersDefinition(0).LayerStartTime) > oDayShiftData.ShiftLayersDefinition(1).LayerStartTime Then
                        Return False
                    End If
                End If

                ' 5.- Se ha modificado la hora de inicio y el horario no es flotante (no confundir con horario por horas e inicio variable)
                If Not oShiftDataDefinition.IsFloating AndAlso Not (oShiftDataDefinition.AllowFloating OrElse oShiftDataDefinition.AllowComplementary) Then
                    If Not oDayShiftData.StartHour = oShiftDataDefinition.StartFloating Then
                        Return False
                    End If
                End If

                ' 6.- Se ha modificado el inicio de alguna franja, y el horario no lo permite
                If oDayShiftData.ShiftLayers > 0 AndAlso Not oShiftDataDefinition.AllowModifyIniHour1 Then
                    If Not oDayShiftData.ShiftLayersDefinition Is Nothing Then
                        ' PDTE PRUEBAS
                        If oDayShiftData.ShiftLayersDefinition(0).LayerStartTime <> New Date(1900, 1, 1) AndAlso oDayShiftData.ShiftLayersDefinition(0).LayerStartTime <> oShiftDataDefinition.StartLayer1 Then
                            'Definieron hora de inicio y no coinciden con el inicio teórico
                            Return False
                        End If
                    End If
                End If

                If oDayShiftData.ShiftLayers > 1 AndAlso Not oShiftDataDefinition.AllowModifyIniHour2 Then
                    If Not oDayShiftData.ShiftLayersDefinition Is Nothing Then
                        '  PDTE PRUEBAS
                        If oDayShiftData.ShiftLayersDefinition(1).LayerStartTime <> New Date(1900, 1, 1) AndAlso oDayShiftData.ShiftLayersDefinition(1).LayerStartTime <> oShiftDataDefinition.StartLayer2 Then
                            'Definieron hora de inicio y no coinciden con el inicio teórico
                            Return False
                        End If
                    End If
                End If

                ' Para horarios por horas, pongo valores "normalizados" para minimizar los datos que se guardarán en BBDD (xml de definición de franjas en dailyschedule)
                ' TODO: Si la función MergeShiftData fuera 100% correcta este bloque talvez no sería necesario
                If oShiftDataDefinition.AllowFloating OrElse oShiftDataDefinition.AllowComplementary Then
                    If Not oShiftDataDefinition.AllowComplementary Then
                        If oDayShiftData.ShiftLayers > 0 Then
                            oDayShiftData.ShiftLayersDefinition(0).LayerComplementaryHours = -1
                            oDayShiftData.ShiftLayersDefinition(0).LayerOrdinaryHours = -1
                        End If
                        If oDayShiftData.ShiftLayers > 1 Then
                            oDayShiftData.ShiftLayersDefinition(1).LayerComplementaryHours = -1
                            oDayShiftData.ShiftLayersDefinition(1).LayerOrdinaryHours = -1
                        End If
                    End If
                    If Not oShiftDataDefinition.AllowModifyDuration1 Then
                        If oDayShiftData.ShiftLayers > 0 Then
                            oDayShiftData.ShiftLayersDefinition(0).LayerDuration = -1
                        End If
                        If oDayShiftData.ShiftLayers > 1 Then
                            oDayShiftData.ShiftLayersDefinition(1).LayerDuration = -1
                        End If
                    End If
                    If Not oShiftDataDefinition.AllowModifyIniHour1 Then
                        If oDayShiftData.ShiftLayers > 0 Then
                            oDayShiftData.ShiftLayersDefinition(0).LayerStartTime = New Date(1900, 1, 1)
                        End If
                        If oDayShiftData.ShiftLayers > 1 Then
                            oDayShiftData.ShiftLayersDefinition(1).LayerStartTime = New Date(1900, 1, 1)
                        End If
                    End If
                End If

                oRet = True
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarManager::ValidateDayShiftData")
            End Try
            Return oRet
        End Function

        Private Sub InitShiftLayersDefinitionDataIfNeeded(ByRef oCalRowShiftData As roCalendarRowShiftData)
            Try
                ' Creo la estructura de parámetros de layers, vacía por defecto
                If oCalRowShiftData.ShiftLayersDefinition Is Nothing Then
                    Dim oCalendarShiftLayersDefinitionList As New List(Of roCalendarShiftLayersDefinition)
                    Dim oShiftLayerDefinition1 As New roCalendarShiftLayersDefinition
                    Dim oShiftLayerDefinition2 As New roCalendarShiftLayersDefinition
                    oCalendarShiftLayersDefinitionList.Add(oShiftLayerDefinition1)
                    oCalendarShiftLayersDefinitionList.Add(oShiftLayerDefinition2)
                    oCalRowShiftData.ShiftLayersDefinition = oCalendarShiftLayersDefinitionList.ToArray
                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarManager::InitShiftLayersDefinitionDataIfNeeded")
            End Try
        End Sub

        ''' <summary>
        ''' Pasa valores recogidos del Excel a una estructura de horario con las características teóricas. No se valida coherencia de datos de horarios complementarios.
        ''' </summary>
        ''' <param name="oDefinitionShiftData"></param>
        ''' <param name="oExcelShiftData"></param>
        ''' <returns></returns>
        Private Function MergeShiftDataEx(oDefinitionShiftData As roCalendarShift, oExcelShiftData As roCalendarRowShiftData) As roCalendarRowShiftData
            Dim oRet As New roCalendarRowShiftData
            Dim oCalendarShiftLayersDefinitionList As List(Of roCalendarShiftLayersDefinition) = Nothing
            Dim oShiftLayerDefinition As roCalendarShiftLayersDefinition = Nothing
            Try
                ' Informo propiedades básicas
                oRet.ID = oDefinitionShiftData.IDShift
                oRet.ShortName = oDefinitionShiftData.ShortName
                If oExcelShiftData.ShortName <> String.Empty Then
                    oRet.ShortName = oExcelShiftData.ShortName
                Else
                    oRet.ShortName = oDefinitionShiftData.ShortName
                End If
                oRet.Name = oDefinitionShiftData.Name
                oRet.Color = oDefinitionShiftData.Color
                oRet.ShiftLayers = oDefinitionShiftData.CountLayers
                oRet.Type = oDefinitionShiftData.Type
                oRet.ExistComplementaryData = oDefinitionShiftData.AllowComplementary
                oRet.ExistFloatingData = oDefinitionShiftData.AllowFloating
                oRet.ShiftLayersDefinition = Nothing
                oRet.BreakHours = oDefinitionShiftData.BreakHours
                oRet.PlannedHours = oDefinitionShiftData.WorkingHours
                oRet.StartHour = oDefinitionShiftData.StartFloating

                ' Hora de inicio del horario
                If oRet.Type = ShiftTypeEnum.NormalFloating Then
                    ' Si se definió en Excel, la que se definió, sino, por defecto ...
                    If oExcelShiftData.StartHour <> Date.MinValue Then oRet.StartHour = oExcelShiftData.StartHour
                ElseIf oRet.ShiftLayers > 0 Then
                    ' La hora de inicio es la de inicio de la primera franja, si se definió. Sino, la por defecto del horario
                    If Not oExcelShiftData.ShiftLayersDefinition Is Nothing AndAlso Not oExcelShiftData.ShiftLayersDefinition(0) Is Nothing AndAlso oExcelShiftData.ShiftLayersDefinition(0).LayerStartTime <> Date.MinValue Then oRet.StartHour = oExcelShiftData.ShiftLayersDefinition(0).LayerStartTime
                End If

                If oRet.ShiftLayers = 0 Then
                    oRet.PlannedHours = oDefinitionShiftData.WorkingHours
                ElseIf oRet.ShiftLayers > 0 Then
                    oCalendarShiftLayersDefinitionList = New List(Of roCalendarShiftLayersDefinition)
                    ' Trato la primera franja si se definió
                    oShiftLayerDefinition = New roCalendarShiftLayersDefinition
                    oShiftLayerDefinition.LayerID = oDefinitionShiftData.IDLayer1
                    ' 1.- Cargo valores por defecto, por si falta algún dato
                    ' 1.1- Inicio de franja
                    If oDefinitionShiftData.AllowModifyIniHour1 Then
                        oShiftLayerDefinition.LayerStartTime = oDefinitionShiftData.StartLayer1
                    Else
                        oShiftLayerDefinition.LayerStartTime = New Date(1900, 1, 1)
                    End If
                    ' 1.2.- Duración de franja
                    If oDefinitionShiftData.AllowModifyDuration1 Then
                        oShiftLayerDefinition.LayerDuration = oShiftLayerDefinition.LayerDuration
                    Else
                        oShiftLayerDefinition.LayerDuration = -1
                    End If
                    ' 1.3.- Ordinarias
                    oShiftLayerDefinition.LayerOrdinaryHours = -1
                    ' 1.4.- Complementarias
                    oShiftLayerDefinition.LayerComplementaryHours = -1

                    ' 2.- Cargo valores recogidos en Excel
                    If Not oExcelShiftData.ShiftLayersDefinition Is Nothing AndAlso Not oExcelShiftData.ShiftLayersDefinition(0) Is Nothing Then
                        If oExcelShiftData.ShiftLayersDefinition(0).LayerComplementaryHours <> -1 Then oShiftLayerDefinition.LayerComplementaryHours = oExcelShiftData.ShiftLayersDefinition(0).LayerComplementaryHours
                        If oExcelShiftData.ShiftLayersDefinition(0).LayerStartTime <> Date.MinValue Then oShiftLayerDefinition.LayerStartTime = oExcelShiftData.ShiftLayersDefinition(0).LayerStartTime
                        If oExcelShiftData.ShiftLayersDefinition(0).LayerOrdinaryHours <> -1 Then
                            ' En las ordinarias pueden venir, ordinarias realmente, o bien la duración del horario en aquellos casos en que no se permiten complementarias, y la definición del horario permite que la duración de la franja varie
                            If oDefinitionShiftData.AllowComplementary Then
                                oShiftLayerDefinition.LayerOrdinaryHours = oExcelShiftData.ShiftLayersDefinition(0).LayerOrdinaryHours
                                'TO BE CONTINUED
                            End If
                        End If
                        'TO BE CONTINUED
                        '...
                        oCalendarShiftLayersDefinitionList.Add(oShiftLayerDefinition)
                    Else
                        ' Valores por defecto
                        'TO BE CONTINUED
                        '...
                        oCalendarShiftLayersDefinitionList.Add(oShiftLayerDefinition)
                    End If

                End If

                If oRet.ShiftLayers > 1 Then
                    'Lo mismo para la franja 1
                    'TO BE CONTINUED
                    '...
                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarManager::CloneShiftData")
            End Try
            If Not oCalendarShiftLayersDefinitionList Is Nothing Then oRet.ShiftLayersDefinition = oCalendarShiftLayersDefinitionList.ToArray
            Return oRet
        End Function

        ''' <summary>
        ''' Pasa valores recogidos del Excel a una estructura de horario con las características teóricas. No se valida coherencia de datos de horarios complementarios.
        ''' </summary>
        ''' <param name="oDefinitionShiftData"></param>
        ''' <param name="oExcelShiftData"></param>
        ''' <returns></returns>
        Public Function MergeShiftData(oDefinitionShiftData As roCalendarShift, oExcelShiftData As roCalendarRowShiftData) As roCalendarRowShiftData
            Dim oRet As New roCalendarRowShiftData
            Dim oCalendarShiftLayersDefinitionList As List(Of roCalendarShiftLayersDefinition) = Nothing
            Dim oShiftLayerDefinition As roCalendarShiftLayersDefinition = Nothing
            Try
                ' Informo propiedades básicas
                oRet.ID = oDefinitionShiftData.IDShift
                oRet.ShortName = oDefinitionShiftData.ShortName
                If oExcelShiftData.ShortName <> String.Empty Then
                    oRet.ShortName = oExcelShiftData.ShortName
                Else
                    oRet.ShortName = oDefinitionShiftData.ShortName
                End If
                oRet.Name = oDefinitionShiftData.Name
                oRet.Color = oDefinitionShiftData.Color
                oRet.ShiftLayers = oDefinitionShiftData.CountLayers
                oRet.Type = oDefinitionShiftData.Type
                oRet.ExistComplementaryData = oDefinitionShiftData.AllowComplementary
                oRet.ExistFloatingData = oDefinitionShiftData.AllowFloating
                oRet.ShiftLayersDefinition = Nothing
                oRet.BreakHours = oDefinitionShiftData.BreakHours
                oRet.PlannedHours = oDefinitionShiftData.WorkingHours
                oRet.StartHour = oDefinitionShiftData.StartFloating
                oRet.AdvancedParameters = oDefinitionShiftData.AdvancedParameters
                ' TODO: Aquí deberíamos cargar el objeto oExcelShiftData cuando por ejemplo estamos llamando a esta función sin datos provenientes de Excel, como cuando se le llama desde roRequest
                ' De momento, cargo únicamente la hora de fin del horario, suponiendo que se horario rígido
                Try
                    Dim sqlaux As String = "@SELECT# EndLimit from shifts where id = " & oRet.ID
                    Dim tbl As New DataTable
                    tbl = CreateDataTable(sqlaux)
                    If tbl.Rows.Count() = 1 Then
                        oRet.EndHour = roTypes.Any2Time(tbl.Rows(0)("EndLimit")).Value
                    End If
                Catch ex As Exception
                End Try

                ' Hora de inicio del horario
                If oRet.Type = ShiftTypeEnum.NormalFloating Then
                    ' Si se definió en Excel, la que se definió, sino, por defecto ...
                    If oExcelShiftData.StartHour <> Date.MinValue Then oRet.StartHour = oExcelShiftData.StartHour
                ElseIf oRet.ShiftLayers > 0 Then
                    ' La hora de inicio es la de inicio de la primera franja, si se definió. Sino, la por defecto del horario
                    If Not oExcelShiftData.ShiftLayersDefinition Is Nothing AndAlso Not oExcelShiftData.ShiftLayersDefinition(0) Is Nothing AndAlso oExcelShiftData.ShiftLayersDefinition(0).LayerStartTime <> Date.MinValue Then oRet.StartHour = oExcelShiftData.ShiftLayersDefinition(0).LayerStartTime
                End If

                ' Horas planificadas y parámetros de horarios por horas, si los hay (a considerar: Inicio de Franja - Complementarias - Ordinarias - Duración de Franja)
                '     Si permite complementarias, Complementarias y Ordinarias son >= 0? SI
                '     En caso contratio, ¿Complementarias y Ordinarias son = -1? SI
                '     Si no hay complementarias, pero se puede modificar la duración de la franja, se informa LayerDuration (y Ordiarias vale -1 o no se tiene en cuenta? SI
                If oRet.ShiftLayers = 0 Then
                    oRet.PlannedHours = oDefinitionShiftData.WorkingHours
                ElseIf oRet.ShiftLayers > 0 Then
                    oCalendarShiftLayersDefinitionList = New List(Of roCalendarShiftLayersDefinition)
                    ' Trato la primera franja si se definió
                    oShiftLayerDefinition = New roCalendarShiftLayersDefinition
                    oShiftLayerDefinition.LayerID = oDefinitionShiftData.IDLayer1
                    ' Cargo valores por defecto, por si falta algún dato
                    oShiftLayerDefinition.LayerComplementaryHours = 0
                    oShiftLayerDefinition.LayerOrdinaryHours = DateDiff(DateInterval.Minute, oDefinitionShiftData.StartLayer1, oDefinitionShiftData.EndLayer1)
                    If oDefinitionShiftData.AllowModifyIniHour1 Then
                        oShiftLayerDefinition.LayerStartTime = oDefinitionShiftData.StartLayer1
                        oShiftLayerDefinition.LayerDuration = oShiftLayerDefinition.LayerOrdinaryHours
                    Else
                        oShiftLayerDefinition.LayerStartTime = New Date(1900, 1, 1)
                        oShiftLayerDefinition.LayerDuration = -1
                    End If

                    If Not oExcelShiftData.ShiftLayersDefinition Is Nothing AndAlso Not oExcelShiftData.ShiftLayersDefinition(0) Is Nothing Then
                        If oExcelShiftData.ShiftLayersDefinition(0).LayerComplementaryHours <> -1 Then oShiftLayerDefinition.LayerComplementaryHours = oExcelShiftData.ShiftLayersDefinition(0).LayerComplementaryHours
                        If oExcelShiftData.ShiftLayersDefinition(0).LayerOrdinaryHours <> -1 Then oShiftLayerDefinition.LayerOrdinaryHours = oExcelShiftData.ShiftLayersDefinition(0).LayerOrdinaryHours
                        If oExcelShiftData.ShiftLayersDefinition(0).LayerStartTime <> Date.MinValue Then oShiftLayerDefinition.LayerStartTime = oExcelShiftData.ShiftLayersDefinition(0).LayerStartTime

                        If oDefinitionShiftData.AllowModifyDuration1 Then
                            oShiftLayerDefinition.LayerDuration = oShiftLayerDefinition.LayerComplementaryHours + oShiftLayerDefinition.LayerOrdinaryHours
                            oRet.PlannedHours = oShiftLayerDefinition.LayerDuration
                        Else
                            ' Si me definieron complementarias
                            If oExcelShiftData.ShiftLayersDefinition(0).LayerComplementaryHours <> -1 Then
                                ' ...  y no se indicaron ordinarias, las calculo para completar. Después vendrá la validación
                                If oExcelShiftData.ShiftLayersDefinition(0).LayerOrdinaryHours = -1 AndAlso (oShiftLayerDefinition.LayerDuration - oExcelShiftData.ShiftLayersDefinition(0).LayerComplementaryHours) >= 0 Then oShiftLayerDefinition.LayerOrdinaryHours = oShiftLayerDefinition.LayerDuration - oExcelShiftData.ShiftLayersDefinition(0).LayerComplementaryHours
                            End If
                        End If

                        If oDefinitionShiftData.AllowModifyIniHour1 Then
                            If oExcelShiftData.ShiftLayersDefinition(0).LayerStartTime <> Date.MinValue Then oShiftLayerDefinition.LayerStartTime = oExcelShiftData.ShiftLayersDefinition(0).LayerStartTime
                            If Not oDefinitionShiftData.AllowModifyDuration1 Then
                                oRet.PlannedHours = DateDiff(DateInterval.Minute, oShiftLayerDefinition.LayerStartTime, oDefinitionShiftData.EndLayer1)
                            End If
                        End If
                        oCalendarShiftLayersDefinitionList.Add(oShiftLayerDefinition)
                    Else
                        ' Valores por defecto
                        If oDefinitionShiftData.AllowModifyDuration1 Then
                            oRet.PlannedHours = oShiftLayerDefinition.LayerDuration
                        End If

                        If oDefinitionShiftData.AllowModifyIniHour1 Then
                            If Not oDefinitionShiftData.AllowModifyDuration1 Then
                                oRet.PlannedHours = DateDiff(DateInterval.Minute, oShiftLayerDefinition.LayerStartTime, oDefinitionShiftData.EndLayer1)
                            End If
                        End If
                        oCalendarShiftLayersDefinitionList.Add(oShiftLayerDefinition)
                    End If

                End If

                If oRet.ShiftLayers > 1 Then
                    ' Trato la primera franja si se definió
                    oShiftLayerDefinition = New roCalendarShiftLayersDefinition
                    oShiftLayerDefinition.LayerID = oDefinitionShiftData.IDLayer2
                    ' Cargo valores por defecto, por si falta algún dato
                    oShiftLayerDefinition.LayerComplementaryHours = 0
                    oShiftLayerDefinition.LayerOrdinaryHours = DateDiff(DateInterval.Minute, oDefinitionShiftData.StartLayer2, oDefinitionShiftData.EndLayer2)
                    If oDefinitionShiftData.AllowModifyIniHour2 Then
                        oShiftLayerDefinition.LayerStartTime = oDefinitionShiftData.StartLayer2
                        oShiftLayerDefinition.LayerDuration = oShiftLayerDefinition.LayerOrdinaryHours
                    Else
                        oShiftLayerDefinition.LayerStartTime = New Date(1900, 1, 1)
                        oShiftLayerDefinition.LayerDuration = -1
                    End If

                    If Not oExcelShiftData.ShiftLayersDefinition Is Nothing AndAlso Not oExcelShiftData.ShiftLayersDefinition(1) Is Nothing Then

                        If oExcelShiftData.ShiftLayersDefinition(1).LayerComplementaryHours <> -1 Then oShiftLayerDefinition.LayerComplementaryHours = oExcelShiftData.ShiftLayersDefinition(1).LayerComplementaryHours
                        If oExcelShiftData.ShiftLayersDefinition(1).LayerOrdinaryHours <> -1 Then oShiftLayerDefinition.LayerOrdinaryHours = oExcelShiftData.ShiftLayersDefinition(1).LayerOrdinaryHours
                        If oExcelShiftData.ShiftLayersDefinition(1).LayerStartTime <> Date.MinValue Then oShiftLayerDefinition.LayerStartTime = oExcelShiftData.ShiftLayersDefinition(1).LayerStartTime

                        If oDefinitionShiftData.AllowModifyDuration2 Then
                            oShiftLayerDefinition.LayerDuration = oShiftLayerDefinition.LayerComplementaryHours + oShiftLayerDefinition.LayerOrdinaryHours
                            oRet.PlannedHours = oRet.PlannedHours + oShiftLayerDefinition.LayerDuration
                        Else
                            ' Si me definieron complementarias
                            If oExcelShiftData.ShiftLayersDefinition(1).LayerComplementaryHours <> -1 Then
                                ' ...  y no se indicaron ordinarias, las calculo para completar. Después vendrá la validación
                                If oExcelShiftData.ShiftLayersDefinition(1).LayerOrdinaryHours = -1 AndAlso (oShiftLayerDefinition.LayerDuration - oExcelShiftData.ShiftLayersDefinition(1).LayerComplementaryHours) >= 0 Then oShiftLayerDefinition.LayerOrdinaryHours = oShiftLayerDefinition.LayerDuration - oExcelShiftData.ShiftLayersDefinition(1).LayerComplementaryHours
                            End If
                        End If

                        If oDefinitionShiftData.AllowModifyIniHour2 Then
                            If oExcelShiftData.ShiftLayersDefinition(1).LayerStartTime <> Date.MinValue Then oShiftLayerDefinition.LayerStartTime = oExcelShiftData.ShiftLayersDefinition(1).LayerStartTime
                            If Not oDefinitionShiftData.AllowModifyDuration2 Then
                                oRet.PlannedHours = oRet.PlannedHours + DateDiff(DateInterval.Minute, oShiftLayerDefinition.LayerStartTime, oDefinitionShiftData.EndLayer2)
                            End If
                        End If
                        oCalendarShiftLayersDefinitionList.Add(oShiftLayerDefinition)
                    Else
                        ' Valores por defecto
                        If oDefinitionShiftData.AllowModifyDuration2 Then
                            oRet.PlannedHours = oRet.PlannedHours + oShiftLayerDefinition.LayerDuration
                        End If

                        If oDefinitionShiftData.AllowModifyIniHour2 Then
                            If Not oDefinitionShiftData.AllowModifyDuration2 Then
                                oRet.PlannedHours = oRet.PlannedHours + DateDiff(DateInterval.Minute, oShiftLayerDefinition.LayerStartTime, oDefinitionShiftData.EndLayer2)
                            End If
                        End If
                        oCalendarShiftLayersDefinitionList.Add(oShiftLayerDefinition)
                    End If
                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarManager::CloneShiftData")
            End Try
            If Not oCalendarShiftLayersDefinitionList Is Nothing Then oRet.ShiftLayersDefinition = oCalendarShiftLayersDefinitionList.ToArray
            Return oRet
        End Function

        ''' <summary>
        ''' Pasa valores recogidos del Excel a una estructura de horario con las características teóricas. No se valida coherencia de datos de horarios complementarios.
        ''' </summary>
        ''' <param name="oDefinitionShiftData"></param>
        ''' <param name="oReceivedShiftData"></param>
        Private Function GetLayersDurationForExport(oDefinitionShiftData As roCalendarShift, ByRef oReceivedShiftData As roCalendarRowShiftData, iLayer As Integer) As Double
            Dim oRet As Double = -1
            Dim oCalendarShiftLayersDefinitionList As List(Of roCalendarShiftLayersDefinition) = Nothing
            Dim oShiftLayerDefinition As roCalendarShiftLayersDefinition = Nothing
            Try
                Select Case iLayer
                    Case 1
                        If oDefinitionShiftData.CountLayers > 0 Then
                            If oDefinitionShiftData.AllowModifyIniHour1 AndAlso Not oDefinitionShiftData.AllowModifyDuration1 Then
                                'O-M-FFI
                                oRet = DateDiff(DateInterval.Minute, oReceivedShiftData.ShiftLayersDefinition(0).LayerStartTime, oDefinitionShiftData.EndLayer1)
                            ElseIf Not oDefinitionShiftData.AllowModifyIniHour1 AndAlso oDefinitionShiftData.AllowModifyDuration1 Then
                                'O-F-V
                                oRet = oReceivedShiftData.ShiftLayersDefinition(0).LayerDuration
                            ElseIf oDefinitionShiftData.AllowModifyIniHour1 AndAlso oDefinitionShiftData.AllowModifyDuration1 Then
                                'O-M-V
                                oRet = oReceivedShiftData.ShiftLayersDefinition(0).LayerDuration
                            End If
                        End If
                    Case 2
                        If oDefinitionShiftData.CountLayers > 1 Then
                            If oDefinitionShiftData.AllowModifyIniHour2 AndAlso Not oDefinitionShiftData.AllowModifyDuration2 Then
                                'O-M-FFI
                                oRet = DateDiff(DateInterval.Minute, oReceivedShiftData.ShiftLayersDefinition(1).LayerStartTime, oDefinitionShiftData.EndLayer2)
                            ElseIf Not oDefinitionShiftData.AllowModifyIniHour2 AndAlso oDefinitionShiftData.AllowModifyDuration2 Then
                                'O-F-V
                                oRet = oReceivedShiftData.ShiftLayersDefinition(1).LayerDuration
                            ElseIf oDefinitionShiftData.AllowModifyIniHour2 AndAlso oDefinitionShiftData.AllowModifyDuration2 Then
                                'O-M-V
                                oRet = oReceivedShiftData.ShiftLayersDefinition(1).LayerDuration
                            End If
                        End If
                End Select
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarManager::GetLayersDurationForExport")
            End Try
            Return oRet
        End Function

        Private Function GetEmployeeIdByKey(sKey As String) As Integer
            Dim oRet As Integer = 0
            If sKey.Trim = String.Empty Then Return oRet
            Try
                Dim tbEmployee As DataTable = VTUserFields.UserFields.roEmployeeUserField.GetIDEmployeesFromUserFieldValue("NIF", sKey, Now, New VTUserFields.UserFields.roUserFieldState())
                If tbEmployee.Rows.Count > 0 Then
                    oRet = tbEmployee.Rows(0).Item("IDEmployee")
                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarManager::GetEmployeeIdByKey")
            End Try
            Return oRet
        End Function

        Private Function GetEmployeeKeyById(iID As Integer) As String
            Dim oRet As String = String.Empty
            Try
                Dim oEmpUserField As VTUserFields.UserFields.roEmployeeUserField
                oEmpUserField = VTUserFields.UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(iID, "NIF", Now, New VTUserFields.UserFields.roUserFieldState)
                If oEmpUserField IsNot Nothing Then
                    Return oEmpUserField.FieldValue
                Else
                    Return String.Empty
                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarManager::GetEmployeeKeyById")
            End Try
            Return oRet
        End Function

        Protected Sub ExcelWriteSetAlarm(oExcel As SLDocument, iRow As Integer, iCol As Integer, cBackGroundColor As System.Drawing.Color, cTextColor As System.Drawing.Color, sTextValue As String)
            Try
                Dim ostyle As SpreadsheetLight.SLStyle
                ostyle = oExcel.CreateStyle
                ostyle.Font.FontColor = cTextColor
                ostyle.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, cBackGroundColor, System.Drawing.Color.Blue)
                If sTextValue.Trim <> String.Empty Then oExcel.SetCellValue(iRow, iCol, sTextValue)
                oExcel.SetCellStyle(iRow, iCol, ostyle)
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarManager::ExcelWriteSetAlarm")
            End Try
        End Sub

        Protected Sub ExcelWriteSetAlarm(oExcel As SLDocument, iRow As Integer, iCol As Integer, iwidth As Integer, iheight As Integer, oPattern As roCalendarCellLayout.eCalendarCellPattern, sTextValue As String)
            Try
                Dim cBackColor As System.Drawing.Color
                Dim cTextColor As System.Drawing.Color
                Select Case oPattern
                    Case roCalendarCellLayout.eCalendarCellPattern.Ignored
                        cBackColor = System.Drawing.Color.FromArgb(255, 255, 235, 156)
                        cTextColor = System.Drawing.Color.FromArgb(255, 156, 101, 0)
                    Case roCalendarCellLayout.eCalendarCellPattern.InvalidData
                        cBackColor = System.Drawing.Color.FromArgb(255, 255, 199, 206)
                        cTextColor = System.Drawing.Color.FromArgb(255, 156, 0, 6)
                End Select

                Dim ostyle As SpreadsheetLight.SLStyle
                ostyle = oExcel.CreateStyle
                ostyle.Font.FontColor = cTextColor
                ostyle.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, cBackColor, System.Drawing.Color.Blue)
                If sTextValue.Trim <> String.Empty Then oExcel.SetCellValue(iRow, iCol, sTextValue)
                oExcel.SetCellStyle(iRow, iCol, iRow + iheight - 1, iCol + iwidth - 1, ostyle)
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarManager::ExcelWriteSetAlarm")
            End Try
        End Sub

        Protected Sub ExcelWriteCellString(oExcel As SLDocument, iRow As Integer, iCol As Integer, sTextValue As String, iRotation As Integer, borders As String)
            Try
                Dim ostyle As SpreadsheetLight.SLStyle
                ostyle = oExcel.CreateStyle
                ostyle.Alignment.TextRotation = iRotation
                oExcel.SetCellValue(iRow, iCol, sTextValue)
                oExcel.SetCellStyle(iRow, iCol, ostyle)
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarManager::ExcelWriteCellString")
            End Try
        End Sub

        Protected Sub ExcelWriteCellInteger(oExcel As SLDocument, iRow As Integer, iCol As Integer, iValue As Integer, iRotation As Integer, borders As String)
            Try
                Dim ostyle As SpreadsheetLight.SLStyle
                ostyle = oExcel.CreateStyle
                ostyle.Alignment.TextRotation = iRotation
                oExcel.SetCellValue(iRow, iCol, iValue)
                oExcel.SetCellStyle(iRow, iCol, ostyle)
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarManager::ExcelWriteCellInteger")
            End Try
        End Sub

        Protected Sub ExcelWriteCellDate(oExcel As SLDocument, iRow As Integer, iCol As Integer, dValue As Date, iRotation As Integer, borders As String)
            Try
                Dim ostyle As SpreadsheetLight.SLStyle
                ostyle = oExcel.CreateStyle
                ostyle.Alignment.TextRotation = iRotation
                ostyle.FormatCode = "dd/MM/yyyy"
                oExcel.SetCellValue(iRow, iCol, dValue)
                oExcel.SetCellStyle(iRow, iCol, ostyle)
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarManager::ExcelWriteCellDate")
            End Try
        End Sub

        Protected Sub ExcelWriteCellTime(oExcel As SLDocument, iRow As Integer, iCol As Integer, tValue As DateTime, iRotation As Integer, borders As String)
            Try
                Dim ostyle As SpreadsheetLight.SLStyle
                ostyle = oExcel.CreateStyle
                ostyle.Alignment.TextRotation = iRotation
                ostyle.FormatCode = "h:mm"
                oExcel.SetCellValue(iRow, iCol, tValue)
                oExcel.SetCellStyle(iRow, iCol, ostyle)
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarManager::ExcelWriteCellTime")
            End Try
        End Sub

        Protected Sub ExcelWriteCellWithProfile(oExcel As SLDocument, oCalendarCellLayout As roCalendarCellLayout, iRow As Integer, iCol As Integer, oDayData As roCalendarRowDayData)
            Dim oElement As roCellElement
            Dim eElement As roCalendarCellLayout.eCalendarCellElement
            Dim oElementTypeData As roCalendarCellLayout.eCalendarCellElementTypeData
            Dim sCellValueText As String
            Dim dCellValueTime As DateTime
            Dim fCellValueNumber As Double
            Dim bError As Boolean = False
            Dim bEmptyCell As Boolean = False
            Dim oShiftDataDefinition As New roCalendarShift
            Dim oCalendarManager As New roCalendarManager
            Dim oCellColor As New System.Drawing.Color

            Try
                For i = 1 To oCalendarCellLayout.Height
                    For j = 1 To oCalendarCellLayout.Width
                        'Miro qué dato hay, si lo hay, en la posición i,j
                        bEmptyCell = False
                        oElement = New roCellElement(i, j, roCalendarCellLayout.eCalendarCellElementTypeData.Text)
                        Try
                            eElement = oCalendarCellLayout.CellLayout.First(Function(x) x.Value.Equals(oElement)).Key
                        Catch ex As Exception
                            bEmptyCell = True
                            eElement = Nothing
                        End Try
                        sCellValueText = ""
                        dCellValueTime = Nothing
                        oCellColor = Nothing
                        Select Case eElement
                            Case roCalendarCellLayout.eCalendarCellElement.MainShiftID
                                oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                If Not oDayData.MainShift Is Nothing Then
                                    sCellValueText = oDayData.MainShift.ShortName.ToString
                                    If bShiftColorOnExport Then oCellColor = ColorTranslator.FromHtml(oDayData.MainShift.Color)
                                End If
                            Case roCalendarCellLayout.eCalendarCellElement.MainShiftName
                                oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                If oDayData.MainShift IsNot Nothing Then
                                    sCellValueText = oDayData.MainShift.Name.ToString
                                    If bShiftColorOnExport Then oCellColor = ColorTranslator.FromHtml(oDayData.MainShift.Color)
                                End If
                            Case roCalendarCellLayout.eCalendarCellElement.MainShiftDescription
                                oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                If oDayData.MainShift IsNot Nothing Then
                                    sCellValueText = oDayData.MainShift.Description.ToString
                                End If
                            Case roCalendarCellLayout.eCalendarCellElement.AltShift1ID
                                oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                If Not oDayData.AltShift1 Is Nothing Then sCellValueText = oDayData.AltShift1.ShortName.ToString
                            Case roCalendarCellLayout.eCalendarCellElement.AltShift2ID
                                oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                If Not oDayData.AltShift2 Is Nothing Then sCellValueText = oDayData.AltShift2.ShortName.ToString
                            Case roCalendarCellLayout.eCalendarCellElement.AltShift3ID
                                oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                If Not oDayData.AltShift3 Is Nothing Then sCellValueText = oDayData.AltShift3.ShortName.ToString
                            Case roCalendarCellLayout.eCalendarCellElement.BaseShiftID
                                oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                If Not oDayData.ShiftBase Is Nothing Then
                                    sCellValueText = oDayData.ShiftBase.ShortName.ToString
                                    If bShiftColorOnExport Then oCellColor = ColorTranslator.FromHtml(oDayData.ShiftBase.Color)
                                End If
                            '----------------------MAIN-------------------------------------
                            Case roCalendarCellLayout.eCalendarCellElement.MainShiftStartTime
                                oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                If Not oDayData.MainShift Is Nothing Then dCellValueTime = oDayData.MainShift.StartHour

                            Case roCalendarCellLayout.eCalendarCellElement.MainShiftStartTime1
                                If Not oDayData.MainShift Is Nothing AndAlso Not oDayData.MainShift.ShiftLayersDefinition Is Nothing AndAlso oDayData.MainShift.ShiftLayersDefinition.Count > 0 Then
                                    ' Es un horario por horas
                                    oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                    'Si no venía la hora de inicio, y es un horario que no permite float, cojo la hora de inicio de la definición
                                    oShiftDataDefinition = oCalendarManager.LoadShiftDataByIdEx(oDayData.MainShift.ID)
                                    If Not oShiftDataDefinition.AllowModifyIniHour1 Then
                                        dCellValueTime = oShiftDataDefinition.StartLayer1
                                    Else
                                        dCellValueTime = oDayData.MainShift.ShiftLayersDefinition(0).LayerStartTime
                                    End If
                                ElseIf Not oDayData.MainShift Is Nothing Then
                                    ' Si es un horario normal indicamos el inicio del horario
                                    oShiftDataDefinition = oCalendarManager.LoadShiftDataByIdEx(oDayData.MainShift.ID, True)

                                    If oShiftDataDefinition.CountLayers > 0 Then
                                        oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                        dCellValueTime = oShiftDataDefinition.StartLayer1
                                    Else
                                        oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                        sCellValueText = ""
                                    End If
                                Else
                                    oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                    sCellValueText = ""
                                End If

                            Case roCalendarCellLayout.eCalendarCellElement.MainShiftOrdinaryHours1
                                If Not oDayData.MainShift Is Nothing AndAlso Not oDayData.MainShift.ShiftLayersDefinition Is Nothing AndAlso oDayData.MainShift.ShiftLayersDefinition.Count > 0 Then
                                    ' Es un horario por horas
                                    oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                    'Sólo informo si el horario permite complementarias o se puede modificar la duración de la franja de alguna manera
                                    oShiftDataDefinition = oCalendarManager.LoadShiftDataByIdEx(oDayData.MainShift.ID)
                                    'Si permite complementarias, las ordinarias vienen informadas
                                    If oShiftDataDefinition.AllowComplementary Then
                                        dCellValueTime = New DateTime(Now.Year, Now.Month, Now.Day, oDayData.MainShift.ShiftLayersDefinition(0).LayerOrdinaryHours \ 60, oDayData.MainShift.ShiftLayersDefinition(0).LayerOrdinaryHours Mod 60, 0)
                                    ElseIf (oShiftDataDefinition.AllowModifyDuration1 OrElse (oShiftDataDefinition.AllowModifyIniHour1 AndAlso oShiftDataDefinition.HasLayer1FixedEnd)) Then
                                        ' Si permite modificar la duración, en Excel mostraremos la duración en la misma casilla en que se muestran las ordinarias.
                                        Dim dLayerDuration As Double = -1
                                        dLayerDuration = GetLayersDurationForExport(oShiftDataDefinition, oDayData.MainShift, 1)
                                        If dLayerDuration <> -1 Then
                                            dCellValueTime = New DateTime(Now.Year, Now.Month, Now.Day, dLayerDuration \ 60, dLayerDuration Mod 60, 0)
                                        Else
                                            ' Error!
                                            oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                            sCellValueText = "#ERR"
                                        End If
                                    Else
                                        oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                        sCellValueText = ""
                                    End If
                                ElseIf Not oDayData.MainShift Is Nothing Then
                                    ' Si es un horario normal , obtenemos la druacion de la primera franja
                                    oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                    oShiftDataDefinition = oCalendarManager.LoadShiftDataByIdEx(oDayData.MainShift.ID, True)

                                    Dim dLayerDuration As Double = 0
                                    dLayerDuration = DateDiff(DateInterval.Minute, oShiftDataDefinition.StartLayer1, oShiftDataDefinition.EndLayer1)
                                    dCellValueTime = New DateTime(Now.Year, Now.Month, Now.Day, dLayerDuration \ 60, dLayerDuration Mod 60, 0)
                                Else

                                    oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                    sCellValueText = ""
                                End If

                            Case roCalendarCellLayout.eCalendarCellElement.MainShiftComplementaryHours1
                                If Not oDayData.MainShift Is Nothing AndAlso Not oDayData.MainShift.ShiftLayersDefinition Is Nothing AndAlso oDayData.MainShift.ShiftLayersDefinition.Count > 0 Then
                                    oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                    'Sólo informo si el horario permite complementarias
                                    oShiftDataDefinition = oCalendarManager.LoadShiftDataByIdEx(oDayData.MainShift.ID)
                                    If oShiftDataDefinition.AllowComplementary AndAlso oDayData.MainShift.ShiftLayersDefinition(0).LayerComplementaryHours <> -1 Then
                                        dCellValueTime = New DateTime(Now.Year, Now.Month, Now.Day, oDayData.MainShift.ShiftLayersDefinition(0).LayerComplementaryHours \ 60, oDayData.MainShift.ShiftLayersDefinition(0).LayerComplementaryHours Mod 60, 0)
                                    Else
                                        oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                        sCellValueText = ""
                                    End If
                                Else
                                    oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                    sCellValueText = ""
                                End If

                            Case roCalendarCellLayout.eCalendarCellElement.MainShiftStartTime2
                                If Not oDayData.MainShift Is Nothing AndAlso Not oDayData.MainShift.ShiftLayersDefinition Is Nothing AndAlso oDayData.MainShift.ShiftLayersDefinition.Count > 1 Then
                                    oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                    'Si no venía la hora de inicio, y es un horario que no permite float, cojo la hora de inicio de la definición
                                    oShiftDataDefinition = oCalendarManager.LoadShiftDataByIdEx(oDayData.MainShift.ID)
                                    If Not oShiftDataDefinition.AllowModifyIniHour2 Then
                                        dCellValueTime = oShiftDataDefinition.StartLayer2
                                    Else
                                        dCellValueTime = oDayData.MainShift.ShiftLayersDefinition(1).LayerStartTime
                                    End If
                                ElseIf Not oDayData.MainShift Is Nothing Then
                                    ' Si es un horario normal indicamos el inicio de la segunda franja
                                    oShiftDataDefinition = oCalendarManager.LoadShiftDataByIdEx(oDayData.MainShift.ID, True)

                                    If oShiftDataDefinition.CountLayers > 1 Then
                                        oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                        dCellValueTime = oShiftDataDefinition.StartLayer2
                                    Else
                                        oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                        sCellValueText = ""
                                    End If
                                Else
                                    oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                    sCellValueText = ""
                                End If

                            Case roCalendarCellLayout.eCalendarCellElement.MainShiftOrdinaryHours2
                                If Not oDayData.MainShift Is Nothing AndAlso Not oDayData.MainShift.ShiftLayersDefinition Is Nothing AndAlso oDayData.MainShift.ShiftLayersDefinition.Count > 1 Then
                                    ' Es un horario por horas
                                    oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                    'Sólo informo si el horario permite complementarias o se puede modificar la duración de la franja de alguna manera
                                    oShiftDataDefinition = oCalendarManager.LoadShiftDataByIdEx(oDayData.MainShift.ID)
                                    'Si permite complementarias, las ordinarias vienen informadas
                                    If oShiftDataDefinition.AllowComplementary Then
                                        dCellValueTime = New DateTime(Now.Year, Now.Month, Now.Day, oDayData.MainShift.ShiftLayersDefinition(1).LayerOrdinaryHours \ 60, oDayData.MainShift.ShiftLayersDefinition(1).LayerOrdinaryHours Mod 60, 0)
                                    ElseIf (oShiftDataDefinition.AllowModifyDuration2 OrElse (oShiftDataDefinition.AllowModifyIniHour2 AndAlso oShiftDataDefinition.HasLayer2FixedEnd)) Then
                                        ' Si permite modificar la duración, en Excel mostraremos la duración en la misma casilla en que se muestran las ordinarias.
                                        Dim dLayerDuration As Double = -1
                                        dLayerDuration = GetLayersDurationForExport(oShiftDataDefinition, oDayData.MainShift, 2)
                                        If dLayerDuration <> -1 Then
                                            dCellValueTime = New DateTime(Now.Year, Now.Month, Now.Day, dLayerDuration \ 60, dLayerDuration Mod 60, 0)
                                        Else
                                            ' Error!
                                            oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                            sCellValueText = "#ERR"
                                        End If
                                    Else
                                        oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                        sCellValueText = ""
                                    End If
                                ElseIf Not oDayData.MainShift Is Nothing Then
                                    ' Si es un horario normal , obtenemos la duracion de la segunda franja
                                    oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                    oShiftDataDefinition = oCalendarManager.LoadShiftDataByIdEx(oDayData.MainShift.ID, True)

                                    Dim dLayerDuration As Double = 0
                                    dLayerDuration = DateDiff(DateInterval.Minute, oShiftDataDefinition.StartLayer2, oShiftDataDefinition.EndLayer2)
                                    dCellValueTime = New DateTime(Now.Year, Now.Month, Now.Day, dLayerDuration \ 60, dLayerDuration Mod 60, 0)
                                Else
                                    oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                    sCellValueText = ""
                                End If

                            Case roCalendarCellLayout.eCalendarCellElement.MainShiftComplementaryHours2
                                If Not oDayData.MainShift Is Nothing AndAlso Not oDayData.MainShift.ShiftLayersDefinition Is Nothing AndAlso oDayData.MainShift.ShiftLayersDefinition.Count > 1 Then
                                    oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                    'Sólo informo si el horario permite complementarias
                                    oShiftDataDefinition = oCalendarManager.LoadShiftDataByIdEx(oDayData.MainShift.ID)
                                    If oShiftDataDefinition.AllowComplementary AndAlso oDayData.MainShift.ShiftLayersDefinition(1).LayerComplementaryHours <> -1 Then
                                        dCellValueTime = New DateTime(Now.Year, Now.Month, Now.Day, oDayData.MainShift.ShiftLayersDefinition(1).LayerComplementaryHours \ 60, oDayData.MainShift.ShiftLayersDefinition(1).LayerComplementaryHours Mod 60, 0)
                                    Else
                                        oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                        sCellValueText = ""
                                    End If
                                Else
                                    oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                    sCellValueText = ""
                                End If
                            Case roCalendarCellLayout.eCalendarCellElement.MainShiftPlannedHours
                                If Not oDayData.MainShift Is Nothing Then
                                    oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                    dCellValueTime = New DateTime(Now.Year, Now.Month, Now.Day, oDayData.MainShift.PlannedHours \ 60, oDayData.MainShift.PlannedHours Mod 60, 0)
                                Else
                                    oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                    sCellValueText = ""
                                End If
                            '----------------------BASE------------------------------------
                            Case roCalendarCellLayout.eCalendarCellElement.BaseShiftStartTime
                                oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                If Not oDayData.ShiftBase Is Nothing Then dCellValueTime = oDayData.ShiftBase.StartHour

                            Case roCalendarCellLayout.eCalendarCellElement.BaseShiftStartTime1
                                If Not oDayData.ShiftBase Is Nothing AndAlso Not oDayData.ShiftBase.ShiftLayersDefinition Is Nothing AndAlso oDayData.ShiftBase.ShiftLayersDefinition.Count > 0 Then
                                    oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                    'Si no venía la hora de inicio, y es un horario que no permite float, cojo la hora de inicio de la definición
                                    oShiftDataDefinition = oCalendarManager.LoadShiftDataByIdEx(oDayData.ShiftBase.ID)
                                    If Not oShiftDataDefinition.AllowModifyIniHour1 Then
                                        dCellValueTime = oShiftDataDefinition.StartLayer1
                                    Else
                                        dCellValueTime = oDayData.ShiftBase.ShiftLayersDefinition(0).LayerStartTime
                                    End If
                                ElseIf Not oDayData.ShiftBase Is Nothing Then
                                    ' Si es un horario normal indicamos el inicio del horario
                                    oShiftDataDefinition = oCalendarManager.LoadShiftDataByIdEx(oDayData.ShiftBase.ID, True)

                                    If oShiftDataDefinition.CountLayers > 0 Then
                                        oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                        dCellValueTime = oShiftDataDefinition.StartLayer1
                                    Else
                                        oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                        sCellValueText = ""
                                    End If
                                Else
                                    oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                    sCellValueText = ""
                                End If

                            Case roCalendarCellLayout.eCalendarCellElement.BaseShiftOrdinaryHours1
                                If Not oDayData.ShiftBase Is Nothing AndAlso Not oDayData.ShiftBase.ShiftLayersDefinition Is Nothing AndAlso oDayData.ShiftBase.ShiftLayersDefinition.Count > 0 Then
                                    ' Es un horario por horas
                                    oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                    'Sólo informo si el horario permite complementarias o se puede modificar la duración de la franja de alguna manera
                                    oShiftDataDefinition = oCalendarManager.LoadShiftDataByIdEx(oDayData.ShiftBase.ID)
                                    'Si permite complementarias, las ordinarias vienen informadas
                                    If oShiftDataDefinition.AllowComplementary Then
                                        dCellValueTime = New DateTime(Now.Year, Now.Month, Now.Day, oDayData.ShiftBase.ShiftLayersDefinition(0).LayerOrdinaryHours \ 60, oDayData.ShiftBase.ShiftLayersDefinition(0).LayerOrdinaryHours Mod 60, 0)
                                    ElseIf (oShiftDataDefinition.AllowModifyDuration1 OrElse (oShiftDataDefinition.AllowModifyIniHour1 AndAlso oShiftDataDefinition.HasLayer1FixedEnd)) Then
                                        ' Si permite modificar la duración, en Excel mostraremos la duración en la misma casilla en que se muestran las ordinarias.
                                        Dim dLayerDuration As Double = -1
                                        dLayerDuration = GetLayersDurationForExport(oShiftDataDefinition, oDayData.ShiftBase, 1)
                                        If dLayerDuration <> -1 Then
                                            dCellValueTime = New DateTime(Now.Year, Now.Month, Now.Day, dLayerDuration \ 60, dLayerDuration Mod 60, 0)
                                        Else
                                            ' Error!
                                            oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                            sCellValueText = "#ERR"
                                        End If
                                    Else
                                        oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                        sCellValueText = ""
                                    End If
                                ElseIf Not oDayData.ShiftBase Is Nothing Then
                                    ' Si es un horario normal , obtenemos la druacion de la primera franja
                                    oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                    oShiftDataDefinition = oCalendarManager.LoadShiftDataByIdEx(oDayData.ShiftBase.ID, True)

                                    Dim dLayerDuration As Double = 0
                                    dLayerDuration = DateDiff(DateInterval.Minute, oShiftDataDefinition.StartLayer1, oShiftDataDefinition.EndLayer1)
                                    dCellValueTime = New DateTime(Now.Year, Now.Month, Now.Day, dLayerDuration \ 60, dLayerDuration Mod 60, 0)
                                Else
                                    oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                    sCellValueText = ""
                                End If

                            Case roCalendarCellLayout.eCalendarCellElement.BaseShiftComplementaryHours1
                                If Not oDayData.ShiftBase Is Nothing AndAlso Not oDayData.ShiftBase.ShiftLayersDefinition Is Nothing AndAlso oDayData.ShiftBase.ShiftLayersDefinition.Count > 0 Then
                                    oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                    'Sólo informo si el horario permite complementarias
                                    oShiftDataDefinition = oCalendarManager.LoadShiftDataByIdEx(oDayData.ShiftBase.ID)
                                    If oShiftDataDefinition.AllowComplementary AndAlso oDayData.ShiftBase.ShiftLayersDefinition(0).LayerComplementaryHours <> -1 Then
                                        dCellValueTime = New DateTime(Now.Year, Now.Month, Now.Day, oDayData.ShiftBase.ShiftLayersDefinition(0).LayerComplementaryHours \ 60, oDayData.ShiftBase.ShiftLayersDefinition(0).LayerComplementaryHours Mod 60, 0)
                                    Else
                                        oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                        sCellValueText = ""
                                    End If
                                Else
                                    oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                    sCellValueText = ""
                                End If

                            Case roCalendarCellLayout.eCalendarCellElement.BaseShiftStartTime2
                                If Not oDayData.ShiftBase Is Nothing AndAlso Not oDayData.ShiftBase.ShiftLayersDefinition Is Nothing AndAlso oDayData.ShiftBase.ShiftLayersDefinition.Count > 1 Then
                                    oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                    'Si no venía la hora de inicio, y es un horario que no permite float, cojo la hora de inicio de la definición
                                    oShiftDataDefinition = oCalendarManager.LoadShiftDataByIdEx(oDayData.ShiftBase.ID)
                                    If Not oShiftDataDefinition.AllowModifyIniHour2 Then
                                        dCellValueTime = oShiftDataDefinition.StartLayer2
                                    Else
                                        dCellValueTime = oDayData.ShiftBase.ShiftLayersDefinition(1).LayerStartTime
                                    End If
                                ElseIf Not oDayData.ShiftBase Is Nothing Then
                                    ' Si es un horario normal indicamos el inicio de la segunda franja
                                    oShiftDataDefinition = oCalendarManager.LoadShiftDataByIdEx(oDayData.ShiftBase.ID, True)

                                    If oShiftDataDefinition.CountLayers > 1 Then
                                        oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                        dCellValueTime = oShiftDataDefinition.StartLayer2
                                    Else
                                        oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                        sCellValueText = ""
                                    End If
                                Else
                                    oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                    sCellValueText = ""
                                End If
                            Case roCalendarCellLayout.eCalendarCellElement.BaseShiftOrdinaryHours2
                                If Not oDayData.ShiftBase Is Nothing AndAlso Not oDayData.ShiftBase.ShiftLayersDefinition Is Nothing AndAlso oDayData.ShiftBase.ShiftLayersDefinition.Count > 1 Then
                                    ' Es un horario por horas
                                    oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                    'Sólo informo si el horario permite complementarias o se puede modificar la duración de la franja de alguna manera
                                    oShiftDataDefinition = oCalendarManager.LoadShiftDataByIdEx(oDayData.ShiftBase.ID)
                                    'Si permite complementarias, las ordinarias vienen informadas
                                    If oShiftDataDefinition.AllowComplementary Then
                                        dCellValueTime = New DateTime(Now.Year, Now.Month, Now.Day, oDayData.ShiftBase.ShiftLayersDefinition(1).LayerOrdinaryHours \ 60, oDayData.ShiftBase.ShiftLayersDefinition(1).LayerOrdinaryHours Mod 60, 0)
                                    ElseIf (oShiftDataDefinition.AllowModifyDuration2 OrElse (oShiftDataDefinition.AllowModifyIniHour2 AndAlso oShiftDataDefinition.HasLayer2FixedEnd)) Then
                                        ' Si permite modificar la duración, en Excel mostraremos la duración en la misma casilla en que se muestran las ordinarias.
                                        Dim dLayerDuration As Double = -1
                                        dLayerDuration = GetLayersDurationForExport(oShiftDataDefinition, oDayData.ShiftBase, 2)
                                        If dLayerDuration <> -1 Then
                                            dCellValueTime = New DateTime(Now.Year, Now.Month, Now.Day, dLayerDuration \ 60, dLayerDuration Mod 60, 0)
                                        Else
                                            ' Error!
                                            oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                            sCellValueText = "#ERR"
                                        End If
                                    Else
                                        oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                        sCellValueText = ""
                                    End If
                                ElseIf Not oDayData.ShiftBase Is Nothing Then
                                    ' Si es un horario normal , obtenemos la duracion de la segunda franja
                                    oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                    oShiftDataDefinition = oCalendarManager.LoadShiftDataByIdEx(oDayData.ShiftBase.ID, True)

                                    Dim dLayerDuration As Double = 0
                                    dLayerDuration = DateDiff(DateInterval.Minute, oShiftDataDefinition.StartLayer2, oShiftDataDefinition.EndLayer2)
                                    dCellValueTime = New DateTime(Now.Year, Now.Month, Now.Day, dLayerDuration \ 60, dLayerDuration Mod 60, 0)
                                Else
                                    oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                    sCellValueText = ""
                                End If
                            Case roCalendarCellLayout.eCalendarCellElement.BaseShiftComplementaryHours2
                                If Not oDayData.ShiftBase Is Nothing AndAlso Not oDayData.ShiftBase.ShiftLayersDefinition Is Nothing AndAlso oDayData.ShiftBase.ShiftLayersDefinition.Count > 1 Then
                                    oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                    'Sólo informo si el horario permite complementarias
                                    oShiftDataDefinition = oCalendarManager.LoadShiftDataByIdEx(oDayData.ShiftBase.ID)
                                    If oShiftDataDefinition.AllowComplementary AndAlso oDayData.ShiftBase.ShiftLayersDefinition(1).LayerComplementaryHours <> -1 Then
                                        dCellValueTime = New DateTime(Now.Year, Now.Month, Now.Day, oDayData.ShiftBase.ShiftLayersDefinition(1).LayerComplementaryHours \ 60, oDayData.ShiftBase.ShiftLayersDefinition(1).LayerComplementaryHours Mod 60, 0)
                                    Else
                                        oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                        sCellValueText = ""
                                    End If
                                Else
                                    oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                    sCellValueText = ""
                                End If
                            Case roCalendarCellLayout.eCalendarCellElement.FeastDay
                                If oDayData Is Nothing Then
                                    oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                    sCellValueText = "0"

                                    'Sólo indico si esta marcado ocomo festivo
                                    If oDayData.Feast Then
                                        sCellValueText = "1"
                                    End If
                                Else
                                    oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                    sCellValueText = "0"
                                End If

                            Case roCalendarCellLayout.eCalendarCellElement.Assignment
                                oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                If Not oDayData.AssigData Is Nothing Then sCellValueText = oDayData.AssigData.ShortName.ToString

                            Case roCalendarCellLayout.eCalendarCellElement.ScheduleRulesFaults
                                If oDayData.Alerts.Indictments.Count > 0 Then
                                    oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Number
                                    fCellValueNumber = oDayData.Alerts.Indictments.Count
                                Else
                                    oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                    sCellValueText = ""
                                End If
                            Case roCalendarCellLayout.eCalendarCellElement.ProgrammedAbsences
                                If oDayData.Alerts.OnAbsenceDaysInfo <> String.Empty Then
                                    oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                    sCellValueText = oDayData.Alerts.OnAbsenceDaysInfo
                                Else
                                    oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                    sCellValueText = ""
                                End If
                            Case roCalendarCellLayout.eCalendarCellElement.ProgrammedCauses
                                If oDayData.Alerts.OnAbsenceHoursInfo <> String.Empty Then
                                    oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                    sCellValueText = oDayData.Alerts.OnAbsenceHoursInfo
                                Else
                                    oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                    sCellValueText = ""
                                End If
                            Case roCalendarCellLayout.eCalendarCellElement.ProgrammedHolidays
                                If oDayData.Alerts.OnHolidaysHoursInfo <> String.Empty Then
                                    oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                    sCellValueText = oDayData.Alerts.OnHolidaysHoursInfo
                                Else
                                    oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                    sCellValueText = ""
                                End If
                            Case roCalendarCellLayout.eCalendarCellElement.ProgrammedOvertimes
                                If oDayData.Alerts.OnOvertimesHoursInfo <> String.Empty Then
                                    oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                    sCellValueText = oDayData.Alerts.OnOvertimesHoursInfo
                                Else
                                    oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                    sCellValueText = ""
                                End If
                            Case roCalendarCellLayout.eCalendarCellElement.Telecommuting
                                If oDayData IsNot Nothing Then
                                    oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                    sCellValueText = ""
                                    If oDayData.CanTelecommute Then
                                        Dim bolApplyDay As Boolean = False
                                        bolApplyDay = oDayData.TelecommutingMandatoryDays.Contains(oDayData.PlanDate.DayOfWeek)
                                        If Not bolApplyDay Then bolApplyDay = oDayData.TelecommutingOptionalDays.Contains(oDayData.PlanDate.DayOfWeek)
                                        If Not bolApplyDay Then bolApplyDay = oDayData.PresenceMandatoryDays.Contains(oDayData.PlanDate.DayOfWeek)

                                        ' Si tiene acuerdo y el dia es de trabajo
                                        If bolApplyDay Then
                                            ' Por defecto es presencial
                                            sCellValueText = "P"
                                            If oDayData.TelecommutingExpected Then sCellValueText = "T" ' En teletrabajo
                                            If oDayData.TelecommutingOptional Then sCellValueText = "O" ' Opcional

                                            If Not oDayData.TelecommuteForced Then
                                                ' Acuerdo
                                                sCellValueText = "A" & sCellValueText
                                            End If
                                        End If
                                    End If
                                Else
                                    oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                    sCellValueText = ""
                                End If
                                '--------------------------------------------------------------NO BORRAR-------------------------------------------------------------------------------------------------
                                '----------------------ALT1-------------------------------------
                                'Case roCalendarCellLayout.eCalendarCellElement.AltShift1StartTime
                                '    oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                '    If Not oDayData.AltShift1 Is Nothing Then dCellValueTime = oDayData.AltShift1.StartHour.ToShortTimeString
                                'Case roCalendarCellLayout.eCalendarCellElement.AltShift1StartTime1
                                '    If Not oDayData.AltShift1 Is Nothing AndAlso Not oDayData.AltShift1.ShiftLayersDefinition Is Nothing AndAlso oDayData.AltShift1.ShiftLayersDefinition.Count > 0 Then
                                '        oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                '        dCellValueTime = oDayData.AltShift1.ShiftLayersDefinition(0).LayerStartTime
                                '    Else
                                '        oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                '        sCellValueText = ""
                                '    End If
                                'Case roCalendarCellLayout.eCalendarCellElement.AltShift1OrdinaryHours1
                                '    If Not oDayData.AltShift1 Is Nothing AndAlso Not oDayData.AltShift1.ShiftLayersDefinition Is Nothing AndAlso oDayData.AltShift1.ShiftLayersDefinition.Count > 0 Then
                                '        oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                '        dCellValueTime = New DateTime(Now.Year, Now.Month, Now.Day, oDayData.AltShift1.ShiftLayersDefinition(0).LayerOrdinaryHours \ 60, oDayData.AltShift1.ShiftLayersDefinition(0).LayerOrdinaryHours Mod 60, 0)
                                '    Else
                                '        oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                '        sCellValueText = ""
                                '    End If
                                'Case roCalendarCellLayout.eCalendarCellElement.AltShift1ComplementaryHours1
                                '    If Not oDayData.AltShift1 Is Nothing AndAlso Not oDayData.AltShift1.ShiftLayersDefinition Is Nothing AndAlso oDayData.AltShift1.ShiftLayersDefinition.Count > 0 Then
                                '        oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                '        dCellValueTime = New DateTime(Now.Year, Now.Month, Now.Day, oDayData.AltShift1.ShiftLayersDefinition(0).LayerComplementaryHours \ 60, oDayData.AltShift1.ShiftLayersDefinition(0).LayerComplementaryHours Mod 60, 0)
                                '    Else
                                '        oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                '        sCellValueText = ""
                                '    End If
                                'Case roCalendarCellLayout.eCalendarCellElement.AltShift1StartTime2
                                '    If Not oDayData.AltShift1 Is Nothing AndAlso Not oDayData.AltShift1.ShiftLayersDefinition Is Nothing AndAlso oDayData.AltShift1.ShiftLayersDefinition.Count > 1 Then
                                '        oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                '        dCellValueTime = oDayData.AltShift1.ShiftLayersDefinition(1).LayerStartTime
                                '    Else
                                '        oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                '        sCellValueText = ""
                                '    End If
                                'Case roCalendarCellLayout.eCalendarCellElement.AltShift1OrdinaryHours2
                                '    If Not oDayData.AltShift1 Is Nothing AndAlso Not oDayData.AltShift1.ShiftLayersDefinition Is Nothing AndAlso oDayData.AltShift1.ShiftLayersDefinition.Count > 1 Then
                                '        oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                '        dCellValueTime = New DateTime(Now.Year, Now.Month, Now.Day, oDayData.AltShift1.ShiftLayersDefinition(1).LayerOrdinaryHours \ 60, oDayData.AltShift1.ShiftLayersDefinition(1).LayerOrdinaryHours Mod 60, 0)
                                '    Else
                                '        oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                '        sCellValueText = ""
                                '    End If
                                'Case roCalendarCellLayout.eCalendarCellElement.AltShift1ComplementaryHours2
                                '    If Not oDayData.AltShift1 Is Nothing AndAlso Not oDayData.AltShift1.ShiftLayersDefinition Is Nothing AndAlso oDayData.MainShift.ShiftLayersDefinition.Count > 1 Then
                                '        oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                '        dCellValueTime = New DateTime(Now.Year, Now.Month, Now.Day, oDayData.AltShift1.ShiftLayersDefinition(1).LayerComplementaryHours \ 60, oDayData.AltShift1.ShiftLayersDefinition(1).LayerComplementaryHours Mod 60, 0)
                                '    Else
                                '        oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                '        sCellValueText = ""
                                '    End If
                                '----------------------ALT2-------------------------------------
                                'Case roCalendarCellLayout.eCalendarCellElement.AltShift2StartTime
                                '    oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                '    If Not oDayData.AltShift2 Is Nothing Then dCellValueTime = oDayData.AltShift2.StartHour.ToShortTimeString
                                'Case roCalendarCellLayout.eCalendarCellElement.AltShift2StartTime1
                                '    If Not oDayData.AltShift2 Is Nothing AndAlso Not oDayData.AltShift2.ShiftLayersDefinition Is Nothing AndAlso oDayData.AltShift2.ShiftLayersDefinition.Count > 0 Then
                                '        oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                '        dCellValueTime = oDayData.AltShift2.ShiftLayersDefinition(0).LayerStartTime
                                '    Else
                                '        oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                '        sCellValueText = ""
                                '    End If
                                'Case roCalendarCellLayout.eCalendarCellElement.AltShift2OrdinaryHours1
                                '    If Not oDayData.AltShift2 Is Nothing AndAlso Not oDayData.AltShift2.ShiftLayersDefinition Is Nothing AndAlso oDayData.AltShift2.ShiftLayersDefinition.Count > 0 Then
                                '        oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                '        dCellValueTime = New DateTime(Now.Year, Now.Month, Now.Day, oDayData.AltShift2.ShiftLayersDefinition(0).LayerOrdinaryHours \ 60, oDayData.AltShift2.ShiftLayersDefinition(0).LayerOrdinaryHours Mod 60, 0)
                                '    Else
                                '        oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                '        sCellValueText = ""
                                '    End If
                                'Case roCalendarCellLayout.eCalendarCellElement.AltShift2ComplementaryHours1
                                '    If Not oDayData.AltShift2 Is Nothing AndAlso Not oDayData.AltShift2.ShiftLayersDefinition Is Nothing AndAlso oDayData.AltShift2.ShiftLayersDefinition.Count > 0 Then
                                '        oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                '        dCellValueTime = New DateTime(Now.Year, Now.Month, Now.Day, oDayData.AltShift2.ShiftLayersDefinition(0).LayerComplementaryHours \ 60, oDayData.AltShift2.ShiftLayersDefinition(0).LayerComplementaryHours Mod 60, 0)
                                '    Else
                                '        oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                '        sCellValueText = ""
                                '    End If
                                'Case roCalendarCellLayout.eCalendarCellElement.AltShift2StartTime2
                                '    If Not oDayData.AltShift2 Is Nothing AndAlso Not oDayData.AltShift2.ShiftLayersDefinition Is Nothing AndAlso oDayData.AltShift2.ShiftLayersDefinition.Count > 1 Then
                                '        oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                '        dCellValueTime = oDayData.AltShift2.ShiftLayersDefinition(1).LayerStartTime
                                '    Else
                                '        oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                '        sCellValueText = ""
                                '    End If
                                'Case roCalendarCellLayout.eCalendarCellElement.AltShift2OrdinaryHours2
                                '    If Not oDayData.AltShift2 Is Nothing AndAlso Not oDayData.AltShift2.ShiftLayersDefinition Is Nothing AndAlso oDayData.AltShift2.ShiftLayersDefinition.Count > 1 Then
                                '        oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                '        dCellValueTime = New DateTime(Now.Year, Now.Month, Now.Day, oDayData.AltShift2.ShiftLayersDefinition(1).LayerOrdinaryHours \ 60, oDayData.AltShift2.ShiftLayersDefinition(1).LayerOrdinaryHours Mod 60, 0)
                                '    Else
                                '        oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                '        sCellValueText = ""
                                '    End If
                                'Case roCalendarCellLayout.eCalendarCellElement.AltShift2ComplementaryHours2
                                '    If Not oDayData.AltShift2 Is Nothing AndAlso Not oDayData.AltShift2.ShiftLayersDefinition Is Nothing AndAlso oDayData.MainShift.ShiftLayersDefinition.Count > 1 Then
                                '        oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                '        dCellValueTime = New DateTime(Now.Year, Now.Month, Now.Day, oDayData.AltShift2.ShiftLayersDefinition(1).LayerComplementaryHours \ 60, oDayData.AltShift2.ShiftLayersDefinition(1).LayerComplementaryHours Mod 60, 0)
                                '    Else
                                '        oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                '        sCellValueText = ""
                                '    End If
                                '----------------------ALT3-------------------------------------
                                'Case roCalendarCellLayout.eCalendarCellElement.AltShift3StartTime
                                '    oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                '    If Not oDayData.AltShift3 Is Nothing Then dCellValueTime = oDayData.AltShift3.StartHour
                                'Case roCalendarCellLayout.eCalendarCellElement.AltShift3StartTime1
                                '    If Not oDayData.AltShift3 Is Nothing AndAlso Not oDayData.AltShift3.ShiftLayersDefinition Is Nothing AndAlso oDayData.AltShift3.ShiftLayersDefinition.Count > 0 Then
                                '        oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                '        dCellValueTime = oDayData.AltShift3.ShiftLayersDefinition(0).LayerStartTime
                                '    Else
                                '        oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                '        sCellValueText = ""
                                '    End If
                                'Case roCalendarCellLayout.eCalendarCellElement.AltShift3OrdinaryHours1
                                '    If Not oDayData.AltShift3 Is Nothing AndAlso Not oDayData.AltShift3.ShiftLayersDefinition Is Nothing AndAlso oDayData.AltShift3.ShiftLayersDefinition.Count > 0 Then
                                '        oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                '        dCellValueTime = New DateTime(Now.Year, Now.Month, Now.Day, oDayData.AltShift3.ShiftLayersDefinition(0).LayerOrdinaryHours \ 60, oDayData.AltShift3.ShiftLayersDefinition(0).LayerOrdinaryHours Mod 60, 0)
                                '    Else
                                '        oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                '        sCellValueText = ""
                                '    End If
                                'Case roCalendarCellLayout.eCalendarCellElement.AltShift3ComplementaryHours1
                                '    If Not oDayData.AltShift3 Is Nothing AndAlso Not oDayData.AltShift3 Is Nothing AndAlso Not oDayData.AltShift3.ShiftLayersDefinition Is Nothing AndAlso oDayData.AltShift3.ShiftLayersDefinition.Count > 0 Then
                                '        oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                '        dCellValueTime = New DateTime(Now.Year, Now.Month, Now.Day, oDayData.AltShift3.ShiftLayersDefinition(0).LayerComplementaryHours \ 60, oDayData.AltShift3.ShiftLayersDefinition(0).LayerComplementaryHours Mod 60, 0)
                                '    Else
                                '        oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                '        sCellValueText = ""
                                '    End If
                                'Case roCalendarCellLayout.eCalendarCellElement.AltShift3StartTime2
                                '    If Not oDayData.AltShift3 Is Nothing AndAlso Not oDayData.AltShift3.ShiftLayersDefinition Is Nothing AndAlso oDayData.AltShift3.ShiftLayersDefinition.Count > 1 Then
                                '        oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                '        dCellValueTime = oDayData.AltShift3.ShiftLayersDefinition(1).LayerStartTime
                                '    Else
                                '        oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                '        sCellValueText = ""
                                '    End If
                                'Case roCalendarCellLayout.eCalendarCellElement.AltShift3OrdinaryHours2
                                '    If Not oDayData.AltShift3 Is Nothing AndAlso Not oDayData.AltShift3.ShiftLayersDefinition Is Nothing AndAlso oDayData.AltShift3.ShiftLayersDefinition.Count > 1 Then
                                '        oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                '        dCellValueTime = New DateTime(Now.Year, Now.Month, Now.Day, oDayData.AltShift3.ShiftLayersDefinition(1).LayerOrdinaryHours \ 60, oDayData.AltShift3.ShiftLayersDefinition(1).LayerOrdinaryHours Mod 60, 0)
                                '    Else
                                '        oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                '        sCellValueText = ""
                                '    End If
                                'Case roCalendarCellLayout.eCalendarCellElement.AltShift3ComplementaryHours2
                                '    If Not oDayData.AltShift3 Is Nothing AndAlso Not oDayData.AltShift3.ShiftLayersDefinition Is Nothing AndAlso oDayData.AltShift3.ShiftLayersDefinition.Count > 1 Then
                                '        oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                '        dCellValueTime = New DateTime(Now.Year, Now.Month, Now.Day, oDayData.AltShift3.ShiftLayersDefinition(1).LayerComplementaryHours \ 60, oDayData.AltShift3.ShiftLayersDefinition(1).LayerComplementaryHours Mod 60, 0)
                                '    Else
                                '        oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                '        sCellValueText = ""
                                '    End If
                                'Case roCalendarCellLayout.eCalendarCellElement.Unknown
                                '    ' No corresonde a ningún dato conocido
                                '    ' ExcelWriteSetAlarm(oExcel, iRow + i - 1, iCol + j - 1, System.Drawing.Color.Yellow, Color.Black, "¿?")
                                '    ExcelWriteSetAlarm(oExcel, iRow + i - 1, iCol + j - 1, 1, 1, roCalendarCellLayout.eCalendarCellPattern.InvalidData, "¿?")
                                '    bError = True
                        End Select
                        If Not bError AndAlso Not bEmptyCell Then
                            If oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Text Then
                                oExcel.SetCellValue(iRow + i - 1, iCol + j - 1, sCellValueText)
                                If bShiftColorOnExport AndAlso oCellColor <> Nothing Then
                                    Dim ostyle As SpreadsheetLight.SLStyle
                                    ostyle = oExcel.CreateStyle
                                    ostyle.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, oCellColor, System.Drawing.Color.Blue)
                                    oExcel.SetCellStyle(iRow, iCol, ostyle)
                                End If
                            ElseIf oElementTypeData = roCalendarCellLayout.eCalendarCellElementTypeData.Number Then
                                If eElement = roCalendarCellLayout.eCalendarCellElement.ScheduleRulesFaults AndAlso fCellValueNumber > 0 Then
                                    Dim ostyle As SpreadsheetLight.SLStyle
                                    ostyle = oExcel.CreateStyle
                                    ostyle.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, System.Drawing.Color.Yellow, System.Drawing.Color.White)
                                    oExcel.SetCellStyle(iRow + i - 1, iCol + j - 1, ostyle)
                                End If
                                oExcel.SetCellValue(iRow + i - 1, iCol + j - 1, fCellValueNumber)
                            Else
                                ' Era una hora. Distingo el caso de hora no informada, y horas flotantes (que incluyen el calificador de día del horario, día anterior o día posterior)
                                Select Case dCellValueTime.Date
                                    Case DateTime.MinValue
                                        ' No estaba informada
                                        oExcel.SetCellValue(iRow + i - 1, iCol + j - 1, "")
                                    Case New Date(1899, 12, 30, 0, 0, 0, 0)
                                        ' Día del horario
                                        oExcel.SetCellValue(iRow + i - 1, iCol + j - 1, dCellValueTime.ToShortTimeString)
                                    Case New Date(1899, 12, 31, 0, 0, 0, 0)
                                        ' Día poserior al horario
                                        oExcel.SetCellValue(iRow + i - 1, iCol + j - 1, dCellValueTime.ToShortTimeString & "+")
                                    Case New Date(1899, 12, 29, 0, 0, 0, 0)
                                        ' Día anterior al horario
                                        oExcel.SetCellValue(iRow + i - 1, iCol + j - 1, dCellValueTime.ToShortTimeString + "-")
                                    Case Else
                                        ' No debería ocurrir
                                        oExcel.SetCellValue(iRow + i - 1, iCol + j - 1, dCellValueTime.ToShortTimeString)
                                End Select
                            End If
                        End If
                    Next
                Next
            Catch ex As Exception
                ExcelWriteSetAlarm(oExcel, iRow, iCol, oCalendarCellLayout.Width, oCalendarCellLayout.Height, roCalendarCellLayout.eCalendarCellPattern.InvalidData, "")
                Me.oState.UpdateStateInfo(ex, "roCalendarManager::ExcelWriteCellWithProfile")
            End Try
            'Finalmente Recuadro
            OutlineRange(iRow, iCol, iRow + oCalendarCellLayout.Height - 1, iCol + oCalendarCellLayout.Width - 1, oExcel)
        End Sub

        Public Function GetDayDataSimplified(oDayData As roCalendarRowDayData) As roDayShiftSimplified
            Dim ret As roDayShiftSimplified = New roDayShiftSimplified(oDayData.PlanDate)
            Dim oShiftDataDefinition As roCalendarShift

            Try

                If oDayData Is Nothing OrElse oDayData.MainShift Is Nothing Then
                    Return ret
                End If

                If Not oDayData.MainShift Is Nothing AndAlso Not oDayData.MainShift.ShiftLayersDefinition Is Nothing Then
                    ' HORARIO POR HORAS
                    ret.XShiftType = "HORAS"
                    oShiftDataDefinition = LoadShiftDataByIdEx(oDayData.MainShift.ID)
                    If oDayData.MainShift.ShiftLayersDefinition.Count > 0 Then
                        ' UNA FRANJA
                        If Not oShiftDataDefinition.AllowModifyIniHour1 Then
                            ret.Layer1StartTime = oShiftDataDefinition.StartLayer1
                        Else
                            ret.Layer1StartTime = oDayData.MainShift.ShiftLayersDefinition(0).LayerStartTime
                        End If
                        If oShiftDataDefinition.AllowComplementary Then
                            ret.OrdinaryMinutesLayer1 = oDayData.MainShift.ShiftLayersDefinition(0).LayerOrdinaryHours
                        ElseIf (oShiftDataDefinition.AllowModifyDuration1 OrElse (oShiftDataDefinition.AllowModifyIniHour1 AndAlso oShiftDataDefinition.HasLayer1FixedEnd)) Then
                            ret.OrdinaryMinutesLayer1 = GetLayersDurationForExport(oShiftDataDefinition, oDayData.MainShift, 1)
                        End If

                        If oShiftDataDefinition.AllowComplementary AndAlso oDayData.MainShift.ShiftLayersDefinition(0).LayerComplementaryHours <> -1 Then
                            ret.ComplementaryMinutesLayer1 = oDayData.MainShift.ShiftLayersDefinition(0).LayerComplementaryHours
                        End If

                        ret.Layer1EndTime = ret.Layer1StartTime.AddMinutes(ret.OrdinaryMinutesLayer1 + ret.ComplementaryMinutesLayer1)

                    End If
                    If oDayData.MainShift.ShiftLayersDefinition.Count > 1 Then
                        ' DOS FRANJA
                        If Not oShiftDataDefinition.AllowModifyIniHour2 Then
                            ret.Layer2StartTime = oShiftDataDefinition.StartLayer2
                        Else
                            ret.Layer2StartTime = oDayData.MainShift.ShiftLayersDefinition(1).LayerStartTime
                        End If
                        If oShiftDataDefinition.AllowComplementary Then
                            ret.OrdinaryMinutesLayer2 = oDayData.MainShift.ShiftLayersDefinition(1).LayerOrdinaryHours
                        ElseIf (oShiftDataDefinition.AllowModifyDuration2 OrElse (oShiftDataDefinition.AllowModifyIniHour2 AndAlso oShiftDataDefinition.HasLayer2FixedEnd)) Then
                            ret.OrdinaryMinutesLayer2 = GetLayersDurationForExport(oShiftDataDefinition, oDayData.MainShift, 2)
                        End If
                        If oShiftDataDefinition.AllowComplementary AndAlso oDayData.MainShift.ShiftLayersDefinition(1).LayerComplementaryHours <> -1 Then
                            ret.ComplementaryMinutesLayer2 = oDayData.MainShift.ShiftLayersDefinition(1).LayerComplementaryHours
                        End If
                        ret.Layer2EndTime = ret.Layer2StartTime.AddMinutes(ret.OrdinaryMinutesLayer2 + ret.ComplementaryMinutesLayer2)
                    End If
                ElseIf Not oDayData.MainShift Is Nothing Then
                    ' HORARIO NORMAL o FLOTANTE
                    ret.XShiftType = "NORMAL"
                    ret.ExpectedTime = Date.MinValue.AddMinutes(oDayData.MainShift.PlannedHours)
                    oShiftDataDefinition = LoadShiftDataByIdEx(oDayData.MainShift.ID, True)

                    Dim iOffsetHours As Double = 0
                    ' Flipadas con flotantes
                    If oShiftDataDefinition.CountLayers > 0 AndAlso oDayData.MainShift.Type = ShiftTypeEnum.NormalFloating Then
                        iOffsetHours = oDayData.MainShift.StartHour.Subtract(oShiftDataDefinition.StartLayer1).TotalHours
                    End If
                    If oShiftDataDefinition.CountLayers > 0 Then
                        ret.Layer1StartTime = oShiftDataDefinition.StartLayer1.AddHours(iOffsetHours)
                        ret.Layer1EndTime = oShiftDataDefinition.EndLayer1.AddHours(iOffsetHours)
                        ret.OrdinaryMinutesLayer1 = DateDiff(DateInterval.Minute, oShiftDataDefinition.StartLayer1, oShiftDataDefinition.EndLayer1)
                    End If
                    If oShiftDataDefinition.CountLayers > 1 Then
                        ret.Layer2StartTime = oShiftDataDefinition.StartLayer2.AddHours(iOffsetHours)
                        ret.Layer2EndTime = oShiftDataDefinition.EndLayer2.AddHours(iOffsetHours)
                        ret.OrdinaryMinutesLayer2 = DateDiff(DateInterval.Minute, oShiftDataDefinition.StartLayer2, oShiftDataDefinition.EndLayer2)
                    End If
                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarManager::GetDayDataProperty")
            End Try

            Return ret
        End Function

        Public Function GetParseAsciiDateTimeValue(ByVal _Date As String) As Date
            Dim ret As Date = Date.MinValue
            Dim dAux As Date = Date.MinValue
            Try

                If _Date.Length > 0 Then
                    If _Date.Split("+").Length > 1 Then
                        dAux = roTypes.Any2DateTime(_Date.Split("+")(0))
                        ret = New Date(1899, 12, 31, dAux.Hour, dAux.Minute, dAux.Second)
                    ElseIf _Date.Split("-").Length > 1 Then
                        dAux = roTypes.Any2DateTime(_Date.Split("-")(0))
                        ret = New Date(1899, 12, 29, dAux.Hour, dAux.Minute, dAux.Second)
                    Else
                        dAux = _Date
                        ret = New Date(1899, 12, 30, dAux.Hour, dAux.Minute, dAux.Second)
                    End If
                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarManager::GetParseAsciiDateTimeValue")
            End Try

            Return ret
        End Function

        Protected Function LoadProfile(sProfilePath As String, Optional oExcelProfileBytes As Byte() = Nothing) As roCalendarCellLayout
            Dim oRes As roCalendarCellLayout = Nothing
            Try
                Dim oExcelFile As SLDocument
                If oExcelProfileBytes Is Nothing Then
                    oExcelFile = New SLDocument(sProfilePath)
                Else
                    oExcelFile = New SLDocument(New System.IO.MemoryStream(oExcelProfileBytes))
                End If

                Dim oTopLeft As New SLCellPoint
                Dim oBottomRight As New SLCellPoint
                Dim sElementName As String = ""
                oRes = New roCalendarCellLayout
                ' Detecto límites de la definición de celdas (debe estar recuadrada)
                oExcelFile.SelectWorksheet(oExcelFile.GetSheetNames()(0))
                GetCellLayoutLimits(oTopLeft, oBottomRight, oExcelFile)
                oRes.Height = oBottomRight.RowIndex - oTopLeft.RowIndex + 1
                oRes.Width = oBottomRight.ColumnIndex - oTopLeft.ColumnIndex + 1
                Dim oElementName As roCalendarCellLayout.eCalendarCellElement = Nothing
                Dim oElementType As roCalendarCellLayout.eCalendarCellElementTypeData = Nothing

                For iColumn As Integer = oTopLeft.ColumnIndex To oBottomRight.ColumnIndex
                    For iRow As Integer = oTopLeft.RowIndex To oBottomRight.RowIndex
                        sElementName = oExcelFile.GetCellValueAsString(iRow, iColumn)
                        If Not sElementName.Trim = "" Then
                            Select Case sElementName.Trim.ToUpper
                                Case "HPRI"
                                    oElementName = roCalendarCellLayout.eCalendarCellElement.MainShiftID
                                    oElementType = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                Case "NPRI"
                                    oElementName = roCalendarCellLayout.eCalendarCellElement.MainShiftName
                                    oElementType = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                Case "HAL1"
                                    oElementName = roCalendarCellLayout.eCalendarCellElement.AltShift1ID
                                    oElementType = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                Case "HAL2"
                                    oElementName = roCalendarCellLayout.eCalendarCellElement.AltShift2ID
                                    oElementType = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                Case "HAL3"
                                    oElementName = roCalendarCellLayout.eCalendarCellElement.AltShift3ID
                                    oElementType = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                Case "HBAS"
                                    oElementName = roCalendarCellLayout.eCalendarCellElement.BaseShiftID
                                    oElementType = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                Case "PST"
                                    oElementName = roCalendarCellLayout.eCalendarCellElement.MainShiftStartTime
                                    oElementType = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                Case "PST1"
                                    oElementName = roCalendarCellLayout.eCalendarCellElement.MainShiftStartTime1
                                    oElementType = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                Case "POH1"
                                    oElementName = roCalendarCellLayout.eCalendarCellElement.MainShiftOrdinaryHours1
                                    oElementType = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                Case "PCH1"
                                    oElementName = roCalendarCellLayout.eCalendarCellElement.MainShiftComplementaryHours1
                                    oElementType = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                Case "PST2"
                                    oElementName = roCalendarCellLayout.eCalendarCellElement.MainShiftStartTime2
                                    oElementType = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                Case "POH2"
                                    oElementName = roCalendarCellLayout.eCalendarCellElement.MainShiftOrdinaryHours2
                                    oElementType = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                Case "PCH2"
                                    oElementName = roCalendarCellLayout.eCalendarCellElement.MainShiftComplementaryHours2
                                    oElementType = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                Case "PTEO"
                                    oElementName = roCalendarCellLayout.eCalendarCellElement.MainShiftPlannedHours
                                    oElementType = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                Case "BST"
                                    oElementName = roCalendarCellLayout.eCalendarCellElement.BaseShiftStartTime
                                    oElementType = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                Case "BST1"
                                    oElementName = roCalendarCellLayout.eCalendarCellElement.BaseShiftStartTime1
                                    oElementType = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                Case "BOH1"
                                    oElementName = roCalendarCellLayout.eCalendarCellElement.BaseShiftOrdinaryHours1
                                    oElementType = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                Case "BCH1"
                                    oElementName = roCalendarCellLayout.eCalendarCellElement.BaseShiftComplementaryHours1
                                    oElementType = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                Case "BST2"
                                    oElementName = roCalendarCellLayout.eCalendarCellElement.BaseShiftStartTime2
                                    oElementType = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                Case "BOH2"
                                    oElementName = roCalendarCellLayout.eCalendarCellElement.BaseShiftOrdinaryHours2
                                    oElementType = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                Case "BCH2"
                                    oElementName = roCalendarCellLayout.eCalendarCellElement.BaseShiftComplementaryHours2
                                    oElementType = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                Case "A1S"
                                    oElementName = roCalendarCellLayout.eCalendarCellElement.AltShift1StartTime
                                    oElementType = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                Case "A1S1"
                                    oElementName = roCalendarCellLayout.eCalendarCellElement.AltShift1StartTime1
                                    oElementType = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                Case "A1O1"
                                    oElementName = roCalendarCellLayout.eCalendarCellElement.AltShift1OrdinaryHours1
                                    oElementType = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                Case "A1C1"
                                    oElementName = roCalendarCellLayout.eCalendarCellElement.AltShift1ComplementaryHours1
                                    oElementType = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                Case "A1S2"
                                    oElementName = roCalendarCellLayout.eCalendarCellElement.AltShift1StartTime2
                                    oElementType = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                Case "A1O2"
                                    oElementName = roCalendarCellLayout.eCalendarCellElement.AltShift1OrdinaryHours2
                                    oElementType = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                Case "A1C2"
                                    oElementName = roCalendarCellLayout.eCalendarCellElement.AltShift1ComplementaryHours2
                                    oElementType = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                Case "A2S"
                                    oElementName = roCalendarCellLayout.eCalendarCellElement.AltShift2StartTime
                                    oElementType = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                Case "A2S1"
                                    oElementName = roCalendarCellLayout.eCalendarCellElement.AltShift2StartTime1
                                    oElementType = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                Case "A2O1"
                                    oElementName = roCalendarCellLayout.eCalendarCellElement.AltShift2OrdinaryHours1
                                    oElementType = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                Case "A2C1"
                                    oElementName = roCalendarCellLayout.eCalendarCellElement.AltShift2ComplementaryHours1
                                    oElementType = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                Case "A2S2"
                                    oElementName = roCalendarCellLayout.eCalendarCellElement.AltShift2StartTime2
                                    oElementType = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                Case "A2O2"
                                    oElementName = roCalendarCellLayout.eCalendarCellElement.AltShift2OrdinaryHours2
                                    oElementType = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                Case "A2C2"
                                    oElementName = roCalendarCellLayout.eCalendarCellElement.AltShift2ComplementaryHours2
                                    oElementType = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                Case "A3S"
                                    oElementName = roCalendarCellLayout.eCalendarCellElement.AltShift3StartTime
                                    oElementType = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                Case "A3S1"
                                    oElementName = roCalendarCellLayout.eCalendarCellElement.AltShift3StartTime1
                                    oElementType = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                Case "A3O1"
                                    oElementName = roCalendarCellLayout.eCalendarCellElement.AltShift3OrdinaryHours1
                                    oElementType = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                Case "A3C1"
                                    oElementName = roCalendarCellLayout.eCalendarCellElement.AltShift3ComplementaryHours1
                                    oElementType = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                Case "A3S2"
                                    oElementName = roCalendarCellLayout.eCalendarCellElement.AltShift3StartTime2
                                    oElementType = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                Case "A3O2"
                                    oElementName = roCalendarCellLayout.eCalendarCellElement.AltShift3OrdinaryHours2
                                    oElementType = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                Case "A3C2"
                                    oElementName = roCalendarCellLayout.eCalendarCellElement.AltShift3ComplementaryHours2
                                    oElementType = roCalendarCellLayout.eCalendarCellElementTypeData.Time
                                Case "AGM"
                                    oElementName = roCalendarCellLayout.eCalendarCellElement.Assignment
                                    oElementType = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                Case "FES"
                                    oElementName = roCalendarCellLayout.eCalendarCellElement.FeastDay
                                    oElementType = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                Case "SRF"
                                    oElementName = roCalendarCellLayout.eCalendarCellElement.ScheduleRulesFaults
                                    oElementType = roCalendarCellLayout.eCalendarCellElementTypeData.Number
                                Case "AUPD"
                                    oElementName = roCalendarCellLayout.eCalendarCellElement.ProgrammedAbsences
                                    oElementType = roCalendarCellLayout.eCalendarCellElementTypeData.Number
                                Case "AUPH"
                                    oElementName = roCalendarCellLayout.eCalendarCellElement.ProgrammedCauses
                                    oElementType = roCalendarCellLayout.eCalendarCellElementTypeData.Number
                                Case "AUPV"
                                    oElementName = roCalendarCellLayout.eCalendarCellElement.ProgrammedHolidays
                                    oElementType = roCalendarCellLayout.eCalendarCellElementTypeData.Number
                                Case "AUPO"
                                    oElementName = roCalendarCellLayout.eCalendarCellElement.ProgrammedOvertimes
                                    oElementType = roCalendarCellLayout.eCalendarCellElementTypeData.Number
                                Case "TEL"
                                    oElementName = roCalendarCellLayout.eCalendarCellElement.Telecommuting
                                    oElementType = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                Case "DPRI"
                                    oElementName = roCalendarCellLayout.eCalendarCellElement.MainShiftDescription
                                    oElementType = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                                Case Else
                                    oElementName = roCalendarCellLayout.eCalendarCellElement.Unknown
                                    oElementType = roCalendarCellLayout.eCalendarCellElementTypeData.Text
                            End Select
                            If oElementName.ToString.Length > 0 Then
                                oRes.AddElement(iRow - oTopLeft.RowIndex + 1, iColumn - oTopLeft.ColumnIndex + 1, oElementName, oElementType)
                            End If
                        End If
                    Next
                Next
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarManager::LoadProfile")
                oRes = Nothing
            End Try
            Return oRes
        End Function

        Private Sub GetCellLayoutLimits(ByRef oTopLeft As SLCellPoint, ByRef oBottomRight As SLCellPoint, oEx As SLDocument)
            Dim sStyle As SLStyle = oEx.CreateStyle
            Dim oWS As SLWorksheetStatistics
            Dim bTopLeftFound As Boolean = False
            Dim bBottomRightFound As Boolean = False

            Try
                oWS = oEx.GetWorksheetStatistics
                For i = 1 To oWS.EndRowIndex
                    For j = 1 To oWS.EndColumnIndex
                        sStyle = oEx.GetCellStyle(i, j)
                        'Arriba izquierda
                        If sStyle.Border.LeftBorder.BorderStyle <> DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.None AndAlso sStyle.Border.TopBorder.BorderStyle <> DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.None Then
                            bTopLeftFound = True
                            oTopLeft = New SLCellPoint(i, j)
                        End If
                        If sStyle.Border.RightBorder.BorderStyle <> DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.None AndAlso sStyle.Border.BottomBorder.BorderStyle <> DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.None Then
                            bBottomRightFound = True
                            oBottomRight = New SLCellPoint(i, j)
                        End If
                    Next
                Next
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarManager::GetCellLayoutLimits")
            End Try
        End Sub

        Private Function ValidateImportFile(oExcel As SLDocument, oCellLayout As roCalendarCellLayout, ByRef iLastRowIndex As Integer, ByRef iLastColumnIndex As Integer, Optional bExcelIsATemplate As Boolean = False) As Boolean
            Dim oWS As SLWorksheetStatistics
            Dim oRet As Boolean = True
            Const iDateRow As Integer = 2
            Const iEmployeeKeyColumn As Integer = 2
            Const iEmployeeNameColumn As Integer = 1
            Dim iTotalRows As Integer = 0
            iLastRowIndex = 0
            iLastColumnIndex = 0

            Try
                oWS = oExcel.GetWorksheetStatistics

                If oWS.StartRowIndex > 2 OrElse oWS.StartColumnIndex > 2 Then Return False

                ' Recupero índices de la última celda con información (abajo-derecha)
                ' Columna
                If Not oExcel.HasCellValue(iDateRow, oWS.EndColumnIndex - oCellLayout.Width + 1) Then
                    ' No hay fecha informada donde debería. Voy de derecha a izquierda hasta que encuentre una
                    For i As Integer = (oWS.EndColumnIndex - oCellLayout.Width) To 1 Step -1
                        If oExcel.HasCellValue(iDateRow, i) Then
                            iLastColumnIndex = i + oCellLayout.Width - 1
                            Exit For
                        End If
                    Next
                Else
                    iLastColumnIndex = oWS.EndColumnIndex
                End If

                ' Fila

                If Not oExcel.HasCellValue(oWS.EndRowIndex - oCellLayout.Height + 1, iEmployeeKeyColumn) AndAlso Not oExcel.HasCellValue(oWS.EndRowIndex - oCellLayout.Height + 1, iEmployeeNameColumn) Then
                    ' No hay datos de empleado donde debería. Voy de abajo arriba hasta que encuentre un valor
                    For i As Integer = (oWS.EndRowIndex - oCellLayout.Height) To 1 Step -1
                        If oExcel.HasCellValue(i, iEmployeeKeyColumn) OrElse oExcel.HasCellValue(i, iEmployeeNameColumn) Then
                            iLastRowIndex = i + oCellLayout.Height - 1
                            Exit For
                        End If
                    Next
                Else
                    iLastRowIndex = oWS.EndRowIndex
                End If
                If Not bExcelIsATemplate Then
                Else
                    ' Plantilla

                End If

                If bExcelIsATemplate Then
                    ' Es una plantilla.
                    ' No deben haber nombres ni NIFs informados
                    If iLastRowIndex <> 0 Then oRet = False
                    iLastRowIndex = oWS.EndRowIndex
                    ' Sólo debe haber una fila de horarios.
                    If (oWS.NumberOfRows - 1) / oCellLayout.Height > 1 Then oRet = False
                Else
                    If iLastColumnIndex = 0 OrElse iLastRowIndex = 0 Then oRet = False
                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarManager::ValidateImportFileLimits")
                Return False
            End Try
            Return oRet
        End Function

        Protected Sub OutlineRange(iFirstRow As Integer, iFirstColumn As Integer, iLastRow As Integer, ilAstColumn As Integer, oExcel As SLDocument)
            Dim oStyle As New SLStyle
            Try
                oStyle.Border.SetTopBorder(DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Thin, Color.Black)
                oExcel.SetCellStyle(iFirstRow, iFirstColumn, iFirstRow, ilAstColumn, oStyle)
                oStyle.RemoveBorder()
                oStyle.Border.SetBottomBorder(DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Thin, Color.Black)
                oExcel.SetCellStyle(iLastRow, iFirstColumn, iLastRow, ilAstColumn, oStyle)
                oStyle.RemoveBorder()
                oStyle.Border.SetLeftBorder(DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Thin, Color.Black)
                oExcel.SetCellStyle(iFirstRow, iFirstColumn, iLastRow, iFirstColumn, oStyle)
                oStyle.RemoveBorder()
                oStyle.Border.SetRightBorder(DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Thin, Color.Black)
                oExcel.SetCellStyle(iFirstRow, ilAstColumn, iLastRow, ilAstColumn, oStyle)
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarManager::GetCellLayoutLimits")
            End Try
        End Sub

        Private Function GetTempFilePath(ByRef ProcessName As String, ByVal strFileExtension As String) As String
            Try
                ' Obtener nombre del fichero temporal con los datos a importar
                Dim oSettings As New roSettings
                Dim strPath As String = oSettings.GetVTSetting(eKeys.Reports)
                Dim strPrefix As String = ProcessName
                If Me.oState.IDPassport >= 0 Then strPrefix &= "#" & Me.oState.IDPassport.ToString
                Dim tmpFileName As String = TemporaryFileName(strPath, strPrefix, strFileExtension)

                Return tmpFileName
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarManager::CreateTempFile")
                Return String.Empty
            End Try
        End Function

        Private Function TemporaryFileName(ByVal strPath As String, ByVal strPrefix As String, ByVal strExtension As String) As String
            Dim strRet As String = ""
            Try
                Dim Files() As String = System.IO.Directory.GetFiles(strPath, strPrefix & "_*." & strExtension)
                Dim intIndex As Integer = -1
                Dim i As Integer
                For Each strFile As String In Files
                    i = CInt(strFile.Split("_")(1).Split(".")(0))
                    If i > intIndex Then
                        intIndex = i
                    End If
                Next
                intIndex += 1

                ' Obtener nombre del fichero temporal
                strRet = System.IO.Path.Combine(strPath, strPrefix & "_" & intIndex.ToString & "." & strExtension)
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarManager::CreateTempFile")
            End Try
            Return strRet
        End Function

        Private Function FilterGroupName(sFullGroupName As String) As String
            Dim oRes As String = sFullGroupName
            Dim sTemp As String = sFullGroupName
            Try
                ' Me quedo con el nombre del grupo
                If sFullGroupName.Split("\").Length > 1 Then
                    sTemp = sFullGroupName.Split("\")(sFullGroupName.Split("\").Length - 1).Trim
                    oRes = sTemp
                End If

                ' Si el nombre del grupo contiene un código entre paréntesis, devuelvo ese código
                If sTemp.IndexOf("(") > -1 AndAlso sTemp.IndexOf(")") > -1 AndAlso sTemp.IndexOf(")") > sTemp.IndexOf("(") Then
                    oRes = sTemp.Substring(sTemp.IndexOf("(") + 1, sTemp.IndexOf(")") - sTemp.IndexOf("(") - 1)
                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarManager::CreateTempFile")
            End Try
            Return oRes
        End Function

#End Region

        Public Shared Function HasAlternativeShifts() As Boolean
            Dim strSQL As String = "@SELECT# COUNT(*) FROM DailySchedule WHERE ISNULL(IDShift2, 0) > 0 OR ISNULL(IDShift3, 0) > 0 OR ISNULL(IDShift4, 0) > 0"
            Return roTypes.Any2Integer(ExecuteScalar(strSQL)) > 0
        End Function

        Public Shared Function DeleteAlternativeShifts() As Boolean
            Dim strSQL As String = "@UPDATE# DailySchedule WITH (ROWLOCK) SET IDShift2 = NULL, IDShift3 = NULL, IDShift4 = NULL, IDShift1 = ISNULL(IDShiftUsed, IDShift1)"
            Return ExecuteSqlWithoutTimeOut(strSQL)
        End Function

        Public Shared Function ChangeTelecommuting(idEmployee As Integer, dDate As Date, type As DTOs.TelecommutingTypeEnum, bImpersonating As Boolean) As Boolean
            Try
                Dim bRet As Boolean = False
                Dim strSQL As String = String.Empty
                Dim bAlert As Boolean = False

                ' Si llego aquí, tengo acuerdo de teletrabajo.
                If bImpersonating Then
                    ' El cambio lo ha hecho un supervisor. Verificar si hay que notificar
                    Dim currentlyTelecommunicating As Boolean = False
                    strSQL = "@SELECT# ISNULL(Telecommuting,1) FROM DailySchedule WHERE Idemployee = " & idEmployee.ToString & " AND Date = " & roTypes.Any2Time(dDate).SQLSmallDateTime
                    currentlyTelecommunicating = roTypes.Any2Boolean(ExecuteScalar(strSQL))
                    bAlert = (roTypes.Any2Boolean(type) <> currentlyTelecommunicating)
                End If
                strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) SET Telecommuting = " & type & ", TelecommutingOptional = 0, Status = 0, Timestamp = GETDATE() WHERE Idemployee = " & idEmployee.ToString & " AND Date = " & roTypes.Any2Time(dDate).SQLSmallDateTime
                bRet = ExecuteSql(strSQL)

                If bAlert AndAlso bRet Then
                    strSQL = " IF NOT EXISTS (@SELECT# 1 FROM sysroNotificationTasks WHERE IDNotification= 1300 AND Executed = 0 AND Key1Numeric = " & idEmployee.ToString & " AND  Key3DateTime = " & roTypes.Any2Time(dDate).SQLSmallDateTime & " AND CONVERT(VARCHAR,Parameters) = '" & type & "') "
                    strSQL += "@INSERT# INTO sysroNotificationTasks (IDNotification, Key1Numeric, Key3DateTime, Parameters, FiredDate) VALUES (1300, " & idEmployee.ToString & "," & roTypes.Any2Time(dDate).SQLSmallDateTime & ",'" & type.ToString & "', " & roTypes.Any2Time(Now).SQLSmallDateTime & ")"
                    ExecuteSql(strSQL)
                End If

                Return bRet
            Catch ex As Exception
                Return False
            End Try
        End Function

        Public Shared Function GetEmployeePlanningBetweenDates(idEmployee As Integer, ByVal dBeginDate As Date, ByVal dEndDate As Date) As DataTable
            Dim lrret As New DataTable
            Dim strSQL As String
            Try
                Dim queryDateStart As String = roTypes.Any2Time(dBeginDate).SQLSmallDateTime
                Dim queryDateEnd As String = roTypes.Any2Time(dEndDate).SQLSmallDateTime

                ' Obtenemos la planificacion del empleado en el periodo seleccionado
                strSQL = "@SELECT# d.IDEmployee, d.Date as DatePlanned, d.IDShift1 as IdShift, s.Name as ShiftName, s.Color as ShiftColor    "
                strSQL &= " FROM DailySchedule d   "
                strSQL &= " inner join Shifts s on s.Id = d.IdShift1  "
                strSQL &= " WHERE d.Date between " & queryDateStart & " And " & queryDateEnd
                strSQL &= " and d.IdEmployee = " & idEmployee
                strSQL &= " order by d.date asc"
                Dim tb As DataTable = CreateDataTable(strSQL)
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    lrret = tb
                End If
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "roCalendarManager::GetEmployeePlanningBetweenDates")
            Finally
            End Try

            Return lrret
        End Function

        Public Shared Function GetMyTeamPlanEmployees(idEmployee As Integer, ByVal dBeginDate As Date, ByVal dEndDate As Date, ByVal oldShift As Integer) As DataTable
            Dim lrret As New DataTable
            Dim strSQL As String
            Try

                Dim queryDateStart As String = roTypes.Any2Time(dBeginDate).SQLSmallDateTime
                Dim queryDateEnd As String = roTypes.Any2Time(dEndDate).SQLSmallDateTime

                ' Filtro de empleados del mismo departamento que el solicitante a fecha inicial de la solicitud
                Dim strEmployees As String = " ( @SELECT# DISTINCT IDEmployee FROM sysroEmployeeGroups with (nolock) WHERE IDGroup = (@SELECT# idgroup from sysroEmployeeGroups with (nolock) where idemployee= " & idEmployee.ToString &
                                           " AND  " & roTypes.Any2Time(dBeginDate).SQLSmallDateTime & " between sysroEmployeeGroups.BeginDate And sysroEmployeeGroups.EndDate) " &
                                           " AND " & roTypes.Any2Time(dBeginDate).SQLSmallDateTime & "  between sysroEmployeeGroups.BeginDate And sysroEmployeeGroups.EndDate" &
                                                            " AND sysroEmployeeGroups.IDEmployee <>  " & idEmployee.ToString & ") z"

                ' Obtenemos la planificacion de todos los empleados en el periodo seleccionado
                If oldShift > 0 Then
                    strSQL = "@SELECT# d.IDEmployee, d.Date, d.IDShift1, e.Name as EmployeeName, e.Image as EmployeeImage, s.Name as ShiftName, -1 as EmployeeShift    "
                Else
                    ' Añadimos el horario del empleado origen para cada dia
                    strSQL = "@SELECT# d.IDEmployee, d.Date, d.IDShift1, e.Name as EmployeeName, e.Image as EmployeeImage, s.Name as ShiftName, ( @SELECT# isnull(IDShift1,0)  from DailySchedule with (nolock) where idemployee= " & idEmployee.ToString & "  and date = d.Date) as EmployeeShift    "
                End If

                strSQL &= " FROM DailySchedule d with (nolock)  "
                strSQL &= " INNER JOIN " & strEmployees & " ON d.idemployee = Z.idemployee  "
                strSQL &= " INNER JOIN Employees E with (nolock)  ON d.idemployee = e.id  "
                strSQL &= " INNER JOIN Shifts S with (nolock)  ON d.idShift1 = s.id  "
                strSQL &= " WHERE d.Date between " & queryDateStart & " And " & queryDateEnd
                strSQL &= " order by  EmployeeName "
                Dim tb As DataTable = CreateDataTable(strSQL)
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    lrret = tb
                End If
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "roCalendarManager::GetMyTeamPlanEmployees")
            Finally
            End Try

            Return lrret
        End Function

        ''' <summary>
        ''' Recupero el número de días que representa el % máximo de días de teletrabajo del acuerdo
        ''' </summary>
        ''' <param name="idEmployee"></param>
        ''' <param name="dDate"></param>
        ''' <param name="oCn"></param>
        ''' <returns></returns>
        Public Function CheckTelecommutingChange(idEmployee As Integer, dDate As Date, ByVal type As TelecommutingTypeEnum) As TelecommutingCheckChangeResult
            Dim eRet As TelecommutingCheckChangeResult
            Dim bPercentage As Boolean = False
            Dim strSQL As String

            Try

                ' 0.- Recupero información de teletrabajo del empleado ese día. Puede estar forzada por Supervisor (en calendario o por solicitud), o venir del acuerdo.
                strSQL = "@SELECT# * FROM sysrovwEmployeeTelecommuting " &
                         " WHERE IdEmployee = " & idEmployee.ToString & " AND Date = " & roTypes.Any2Time(dDate).SQLSmallDateTime

                Dim tbAux As DataTable = CreateDataTable(strSQL)

                Dim bTelecommutedForced As Boolean = False
                Dim bTelecommutedPlannedForced As Boolean = False
                Dim bTelecommutedOptionalForced As Boolean = False
                Dim bTelecommutedPlannedExpected As Boolean = False
                Dim bTelecommutedOptionalExpected As Boolean = False

                If Not tbAux Is Nothing AndAlso tbAux.Rows.Count > 0 Then
                    bTelecommutedForced = (tbAux.Rows(0)("TelecommutePlanned") IsNot DBNull.Value OrElse tbAux.Rows(0)("TelecommuteOptionalPlanned") IsNot DBNull.Value)

                    If bTelecommutedForced Then
                        ' 1.- Si hay planificación de teletrabajo que llegó por supervisor
                        bTelecommutedPlannedForced = roTypes.Any2Boolean(tbAux.Rows(0)("TelecommutePlanned"))
                        bTelecommutedOptionalForced = roTypes.Any2Boolean(tbAux.Rows(0)("TelecommuteOptionalPlanned"))

                        If Not bTelecommutedOptionalForced Then
                            ' Tengo algo forzado, y no es opcional. Siempre debe haber solicitud
                            Return TelecommutingCheckChangeResult._Request
                        Else
                            ' Es opcional. Luego sólo habrá solicitud si pido teletrabajo y me paso del máximo
                            If type = TelecommutingTypeEnum._AtOffice Then
                                Return TelecommutingCheckChangeResult._Direct
                            End If
                        End If
                    Else
                        ' 2.- No hay nada forzado. Viene del acuerdo
                        bTelecommutedPlannedExpected = roTypes.Any2Boolean(tbAux.Rows(0)("TelecommuteExpected"))
                        bTelecommutedOptionalExpected = roTypes.Any2Boolean(tbAux.Rows(0)("TelecommuteOptionalExpected"))

                        If Not bTelecommutedOptionalExpected Then
                            Return TelecommutingCheckChangeResult._Request
                        Else
                            If type = TelecommutingTypeEnum._AtOffice Then
                                Return TelecommutingCheckChangeResult._Direct
                            End If
                        End If
                    End If
                Else
                    Return TelecommutingCheckChangeResult._NoAgreement
                End If

                ' 3.- Se está solicitando Teletrabajo para un día opcional. Miro si se incumplirían los límites de marcados en el acuerdo
                Dim oETC As Employee.roEmployeeTelecommuteAgreement = Nothing
                Dim oEmployeeTelecommuteAgreement As Employee.roEmployeeTelecommuteAgreement = New Employee.roEmployeeTelecommuteAgreement
                Dim oEmployeeState As VTEmployees.Employee.roEmployeeState = New Employee.roEmployeeState(Me.oState.IDPassport)
                oETC = oEmployeeTelecommuteAgreement.GetTelecommuteAgreementOnDate(idEmployee, dDate, oEmployeeState)

                If oETC Is Nothing Then
                    Return TelecommutingCheckChangeResult._NoAgreement
                End If

                Dim oEmployeeTelecommuteStats As Employee.roEmployeeTelecommuteAgreementStats
                oEmployeeTelecommuteStats = VTEmployees.Employee.roEmployeeTelecommuteAgreement.GetTelecommuteStatsAtDate(idEmployee, dDate, oETC, oEmployeeState)

                ' Cálculo del límte
                If oETC.Agreement.MaxType = TelecommutingMaxType._Percentage Then
                    ' Máximo fijado por porcentaje. Trabajo con horas teóricas.

                    ' Total teóricas del día para el que se pide TC (para sumarlas al total que ya llevo en caso de que se planifique teletrabajo)
                    Dim dTotalWorkingHoursAtDate As Double
                    strSQL = "@SELECT# ISNULL(DailySchedule.ExpectedWorkingHours, Shifts.ExpectedWorkingHours) " &
                         "FROM DailySchedule " &
                         "INNER JOIN Shifts ON Shifts.ID = DailySchedule.IDShift1 " &
                         "WHERE DailySchedule.Date = " & roTypes.Any2Time(dDate.Date).SQLDateTime & " AND DailySchedule.IDEmployee = " & idEmployee.ToString
                    dTotalWorkingHoursAtDate = roTypes.Any2Double(ExecuteScalar(strSQL))

                    Dim dPercentageIfAtHome As Double = 0
                    dPercentageIfAtHome = (oEmployeeTelecommuteStats.TelecommutePlannedHours - oEmployeeTelecommuteStats.TelecommuteHolidaysHours + dTotalWorkingHoursAtDate) / (oEmployeeTelecommuteStats.TotalWorkingPlannedHours - oEmployeeTelecommuteStats.HolidaysHours) * 100
                    eRet = TelecommutingCheckChangeResult._Request
                    If dPercentageIfAtHome <= oETC.Agreement.MaxPercentage Then
                        eRet = TelecommutingCheckChangeResult._Direct
                    End If
                Else
                    ' Máximo fijado por días
                    eRet = TelecommutingCheckChangeResult._Request
                    If (oEmployeeTelecommuteStats.TelecommutePlannedDays + 1) <= oETC.Agreement.MaxDays Then
                        eRet = TelecommutingCheckChangeResult._Direct
                    End If
                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarManager::CheckTelecommutingChange")
                eRet = TelecommutingCheckChangeResult._ErrorChecking
            Finally

            End Try

            Return eRet
        End Function

    End Class

    Public Class roCalendarCellLayout

        Public Enum eCalendarCellElement
            MainShiftID 'HPRI
            AltShift1ID 'HAL1
            AltShift2ID 'HAL2
            AltShift3ID 'HAL3
            BaseShiftID 'HBAS
            MainShiftStartTime 'PST
            MainShiftStartTime1 'PST1
            MainShiftOrdinaryHours1 'POH1
            MainShiftComplementaryHours1 'PCH1
            MainShiftStartTime2 'PST2
            MainShiftOrdinaryHours2 'POH2
            MainShiftComplementaryHours2 'PCH2
            MainShiftPlannedHours 'PTEO
            BaseShiftStartTime 'BST
            BaseShiftStartTime1 'BST1
            BaseShiftOrdinaryHours1 'BOH1
            BaseShiftComplementaryHours1 'BCH1
            BaseShiftStartTime2 'BST2
            BaseShiftOrdinaryHours2 'BOH2
            BaseShiftComplementaryHours2 'BCH2
            AltShift1StartTime 'A1S
            AltShift1StartTime1 'A1S1
            AltShift1OrdinaryHours1 'A1O1
            AltShift1ComplementaryHours1 'A1C1
            AltShift1StartTime2 'A1S2
            AltShift1OrdinaryHours2 'A1O2
            AltShift1ComplementaryHours2 'A1C2
            AltShift2StartTime 'A2S
            AltShift2StartTime1 'A2S1
            AltShift2OrdinaryHours1 'A2O1
            AltShift2ComplementaryHours1 'A2C1
            AltShift2StartTime2 'A2S2
            AltShift2OrdinaryHours2 'A2O2
            AltShift2ComplementaryHours2 'A2C2
            AltShift3StartTime 'A3S
            AltShift3StartTime1 'A3S1
            AltShift3OrdinaryHours1  'A3O1
            AltShift3ComplementaryHours1  'A3C1
            AltShift3StartTime2  'A3S2
            AltShift3OrdinaryHours2   'A3O2
            AltShift3ComplementaryHours2 'A3C2
            Remarks 'REMA
            Unknown
            Assignment 'PUES
            FeastDay ' FES
            ScheduleRulesFaults 'SRF
            ProgrammedAbsences 'AUPD
            ProgrammedCauses 'AUPH
            ProgrammedHolidays 'AUPV
            ProgrammedOvertimes 'AUPO
            Telecommuting 'TEL
            MainShiftName 'NPRI
            MainShiftDescription 'DPRI
        End Enum

        Public Enum eCalendarCellElementTypeData
            Text
            Time
            Number
            DateOnly
        End Enum

        Public Enum eCalendarCellPattern
            InvalidData
            Ignored
        End Enum

        Private _dCellLayout As New Dictionary(Of eCalendarCellElement, roCellElement)
        Private oCellElement As roCellElement
        Private _iWidth As Integer
        Private _iHeight As Integer

        Public Property Width As Integer
            Get
                Return _iWidth
            End Get
            Set(value As Integer)
                _iWidth = value
            End Set
        End Property

        Public Property Height As Integer
            Get
                Return _iHeight
            End Get
            Set(value As Integer)
                _iHeight = value
            End Set
        End Property

        Public Sub AddElement(iRow As Integer, iColumn As Integer, eCellElementName As eCalendarCellElement, type As roCalendarCellLayout.eCalendarCellElementTypeData)
            If Not _dCellLayout.ContainsKey(eCellElementName) Then
                oCellElement = New roCellElement(iRow, iColumn, type)
                _dCellLayout.Add(eCellElementName, oCellElement)
            End If
        End Sub

        Public Property CellLayout As Dictionary(Of eCalendarCellElement, roCellElement)
            Get
                Return _dCellLayout
            End Get
            Set(value As Dictionary(Of eCalendarCellElement, roCellElement))
                _dCellLayout = value
            End Set
        End Property

    End Class

    Public Class roCellElement
        Protected _iColumn As Integer
        Protected _iRow As Integer
        Protected _type As roCalendarCellLayout.eCalendarCellElementTypeData
        Public Property ColumnIndex As Integer
            Get
                Return _iColumn
            End Get
            Set(value As Integer)
                _iColumn = value
            End Set
        End Property
        Public Property RowIndex As Integer
            Get
                Return _iRow
            End Get
            Set(value As Integer)
                _iRow = value
            End Set
        End Property
        Public Property TypeData As roCalendarCellLayout.eCalendarCellElementTypeData
            Get
                Return _type
            End Get
            Set(value As roCalendarCellLayout.eCalendarCellElementTypeData)
                _type = value
            End Set
        End Property

        Public Sub New(iRow As Integer, iColumn As Integer, type As roCalendarCellLayout.eCalendarCellElementTypeData)
            Me.RowIndex = iRow
            Me.ColumnIndex = iColumn
            Me.TypeData = type
        End Sub

        Public Overrides Function Equals(obj As Object) As Boolean
            Dim oElement As roCellElement
            Try
                oElement = CType(obj, roCellElement)
                Return (oElement.ColumnIndex = Me.ColumnIndex AndAlso oElement.RowIndex = Me.RowIndex)
            Catch ex As Exception
                Return False
            End Try
        End Function

    End Class

    Public Class roDayShiftSimplified
        Public Layer1StartTime As DateTime
        Public Layer2StartTime As DateTime
        Public Layer1EndTime As DateTime
        Public Layer2EndTime As DateTime
        Public OrdinaryMinutesLayer1 As Integer
        Public OrdinaryMinutesLayer2 As Integer
        Public ComplementaryMinutesLayer1 As Integer
        Public ComplementaryMinutesLayer2 As Integer
        Public OrdinaryTime As DateTime
        Public ComplementaryTime As DateTime
        Public ExpectedTime As DateTime
        Public XShiftType As String

        Public Sub New(dDate As Date)
            Layer1StartTime = Nothing
            Layer2StartTime = Nothing
            Layer1EndTime = Nothing
            Layer2EndTime = Nothing
            OrdinaryTime = Nothing
            ComplementaryTime = Nothing
            OrdinaryMinutesLayer1 = 0
            OrdinaryMinutesLayer2 = 0
            ComplementaryMinutesLayer1 = 0
            ComplementaryMinutesLayer2 = 0
            ExpectedTime = Nothing
            XShiftType = "NORMAL"
        End Sub

        Public ReadOnly Property Layer1StartText As String
            Get
                Return If(Layer1StartTime = Nothing, "", Layer1StartTime.ToShortTimeString)
            End Get
        End Property
        Public ReadOnly Property Layer1EndText As String
            Get
                Return If(Layer1EndTime = Nothing, "", Layer1EndTime.ToShortTimeString)
            End Get
        End Property
        Public ReadOnly Property Layer2StartText As String
            Get
                Return If(Layer2StartTime = Nothing, "", Layer2StartTime.ToShortTimeString)
            End Get
        End Property
        Public ReadOnly Property Layer2EndText As String
            Get
                Return If(Layer2EndTime = Nothing, "", Layer2EndTime.ToShortTimeString)
            End Get
        End Property
        Public ReadOnly Property OrdinaryTimeText As String
            Get
                If XShiftType = "HORAS" Then
                    Return If((OrdinaryMinutesLayer1 + OrdinaryMinutesLayer2) = 0, "", OrdinaryTime.AddMinutes(OrdinaryMinutesLayer1 + OrdinaryMinutesLayer2).ToShortTimeString)
                Else
                    Return ExpectedTime.ToShortTimeString
                End If

            End Get
        End Property
        Public ReadOnly Property ComplementaryTimeText As String
            Get
                Return If((ComplementaryMinutesLayer1 + ComplementaryMinutesLayer2) = 0, "", ComplementaryTime.AddMinutes(ComplementaryMinutesLayer1 + ComplementaryMinutesLayer2).ToShortTimeString)
            End Get
        End Property
    End Class

End Namespace