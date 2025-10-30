Imports System.Data.Common
Imports System.Text
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Support
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTHolidays
Imports Robotics.DataLayer
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Namespace VTCalendar

    Public Class roCalendarRowManager
        Private oState As roCalendarRowState = Nothing

        Public Sub New()
            Me.oState = New roCalendarRowState()
        End Sub

        Public Sub New(ByVal _State As roCalendarRowState)
            Me.oState = _State
        End Sub

#Region "Helpers"

        Public Shared Function LoadRowsByCalendar(ByVal _FirstDay As DateTime, ByVal _LastDay As DateTime, ByVal _strEmployeeFilter As String, ByRef oState As roCalendarRowState,
                                                  ByVal _typeView As CalendarView, ByVal _detailLevel As CalendarDetailLevel, ByVal bolLicenseHRScheduling As Boolean, ByVal oPassportconfig As roCalendarPassportConfig,
                                                   Optional ByVal _ChildGroups As Boolean = False, Optional ByVal _lstAssignment As String = "", Optional bLoadSeatingCapacity As Boolean = False) As roCalendarRow()
            ' Llenamos las filas del calendario
            Dim oRet As New Generic.List(Of roCalendarRow)
            Dim bolRet As Boolean = False
            Dim EmployeeIDs As New Generic.List(Of Integer)

            Dim strEmployees As String = "-1"

            Try

                ' 00. Obtenemos los grupos sobre los que tiene permisos el Supervisor y las excepciones sobre empleados
                'Dim oStateGroup As New Group.roGroupState(oState.IDPassport)
                'Dim tbGroups As DataTable = Group.roGroup.GetGroupsWithPermissions(2, oStateGroup)
                'Dim oGroups() As DataRow = Nothing
                'Dim oEmployeesRow() As DataRow = Nothing

                Dim oShiftCache As New Hashtable

                'Dim oEmployeeExceptions As DataTable = VTBusiness.Common.roBusinessSupport.GetEmployeesException(2, New Employee.roEmployeeState(oState.IDPassport))

                Dim oCalRuleState As New VTCalendar.roCalendarScheduleRulesState(oState.IDPassport)
                Dim oCalRulesManager As New VTCalendar.roCalendarScheduleRulesManager(oCalRuleState)
                Dim oParameters As New roParameters("OPTIONS", True)

                ' Obtenemos el saldo de horas de la pantalla de calendario
                Dim strShortNameConcept As String = oPassportconfig.CalendarAccrual
                Dim oConcept As New Concept.roConcept
                If strShortNameConcept.Length > 0 Then
                    oConcept.ID = roTypes.Any2Integer(ExecuteScalar("@SELECT# ISNULL(ID,0) FROM CONCEPTS WHERE SHORTNAME LIKE '" & strShortNameConcept & "'"))
                    oConcept.Load()
                End If

                ' Obtenemos el saldo de Vacaciones de la pantalla de calendario
                strShortNameConcept = oPassportconfig.CalendarHolidays
                Dim oConceptHolidays As New Concept.roConcept
                Dim idShiftHolidays As Integer = 0
                If strShortNameConcept.Length > 0 Then
                    oConceptHolidays.ID = roTypes.Any2Integer(ExecuteScalar("@SELECT# ISNULL(ID,0) FROM CONCEPTS WHERE SHORTNAME LIKE '" & strShortNameConcept & "'"))
                    oConceptHolidays.Load()
                    If oConceptHolidays IsNot Nothing Then
                        Dim strHolidayShiftSQL As String = "@SELECT# id from Shifts where IDConceptBalance = " & oConceptHolidays.ID & " and ShiftType = 2"
                        idShiftHolidays = roTypes.Any2Integer(DataLayer.AccessHelper.ExecuteScalar(strHolidayShiftSQL))
                    End If

                End If

                ' Obtenemos el saldo de Horas Normales de la vista revision
                strShortNameConcept = oPassportconfig.CalendarWorking
                Dim oConceptNormalWork As New Concept.roConcept
                If strShortNameConcept.Length > 0 Then
                    oConceptNormalWork.ID = roTypes.Any2Integer(ExecuteScalar("@SELECT# ISNULL(ID,0) FROM CONCEPTS WHERE SHORTNAME LIKE '" & strShortNameConcept & "'"))
                    oConceptNormalWork.Load()
                End If

                ' Obtenemos el saldo de Horas Ausencia de la vista revision
                strShortNameConcept = oPassportconfig.CalendarNotJustified
                Dim oConceptAbsence As New Concept.roConcept
                If strShortNameConcept.Length > 0 Then
                    oConceptAbsence.ID = roTypes.Any2Integer(ExecuteScalar("@SELECT# ISNULL(ID,0) FROM CONCEPTS WHERE SHORTNAME LIKE '" & strShortNameConcept & "'"))
                    oConceptAbsence.Load()
                End If

                ' Obtenemos el saldo de Horas Extras de la vista revision
                strShortNameConcept = oPassportconfig.CalendarOvertime
                Dim oConceptOverWorking As New Concept.roConcept
                If strShortNameConcept.Length > 0 Then
                    oConceptOverWorking.ID = roTypes.Any2Integer(ExecuteScalar("@SELECT# ISNULL(ID,0) FROM CONCEPTS WHERE SHORTNAME LIKE '" & strShortNameConcept & "'"))
                    oConceptOverWorking.Load()
                End If

                ' 01. Obtenemos los criterios de seleccion
                Dim conf As String() = _strEmployeeFilter.Split("¬")

                Dim sEmpFilter As String = conf(0)
                Dim sFilterStatus As String = If(conf.Length > 1 AndAlso conf(1).Length > 0, conf(1), "11110")
                Dim sFilterUserFields As String = If(conf.Length > 2 AndAlso conf(2).Length > 0, conf(2), "")

                ' Empleados y grupos
                EmployeeIDs = roSelector.GetEmployeeList(oState.IDPassport, "", "", Nothing, sEmpFilter, sFilterStatus, sFilterUserFields, Not _ChildGroups, _FirstDay, _LastDay)
                If EmployeeIDs.Any() Then strEmployees = String.Join(",", EmployeeIDs)

                Dim strQuery As String = String.Empty

                strQuery &= " @SELECT# EmployeeGroups.IDEmployee, EmployeeGroups.IDGroup, EmployeeGroups.BeginDate, EmployeeGroups.EndDate, "
                strQuery &= " Groups.FullGroupName, Employees.Name AS EmployeeName , Groups.Name as GroupName, "
                strQuery &= " (@SELECT# isnull(count(*),0) from sysrosubvwCurrentEmployeePeriod where sysrosubvwCurrentEmployeePeriod.IDEmployee = Employees.ID ) as CurrentEmployee, (@SELECT# LockDate from sysrovwEmployeeLockDate where sysrovwEmployeeLockDate.IDEmployee = Employees.ID)  as LockDate "
                strQuery &= " FROM EmployeeGroups INNER JOIN Employees ON EmployeeGroups.IDEmployee = Employees.ID INNER JOIN Groups ON EmployeeGroups.IDGroup = Groups.ID "
                strQuery &= $" INNER JOIN sysrovwSecurity_PermissionOverEmployeeAndFeature poe on poe.IDPassport={oState.IDPassport} And poe.IDEmployee = EmployeeGroups.IDEmployee 
                                            And poe.FeatureAlias='Calendar' AND poe.FeatureType='U' AND poe.FeaturePermission >=3
                                            And EmployeeGroups.BeginDate between poe.BeginDate And poe.EndDate "
                strQuery &= " WHERE EmployeeGroups.IDEmployee IN (" & strEmployees & ") "


                Dim sDateInf As String = roTypes.Any2Time(_FirstDay).SQLSmallDateTime
                Dim sDateSup As String = roTypes.Any2Time(_LastDay).SQLSmallDateTime

                strQuery &= $" AND {sDateInf} <= EmployeeGroups.EndDate AND {sDateSup} >= EmployeeGroups.BeginDate AND 
                               (@SELECT# COUNT(*) FROM EmployeeContracts ec WHERE ec.IDEmployee = EmployeeGroups.IDEmployee AND {sDateInf} <= ec.EndDate AND {sDateSup} >= ec.BeginDate) > 0"

                If _lstAssignment IsNot Nothing AndAlso _lstAssignment.Length > 0 Then
                    strQuery &= " AND ((@SELECT# COUNT(*) FROM EmployeeAssignments WHERE EmployeeAssignments.IDEmployee = EmployeeGroups.IDEmployee AND EmployeeAssignments.IDAssignment IN(" & _lstAssignment & "))  > 0 "

                    ' Si han seleccionado 'Sin puesto'
                    If _lstAssignment = "0" OrElse _lstAssignment.StartsWith("0,") Then
                        strQuery &= " OR (@SELECT# COUNT(*) FROM EmployeeAssignments WHERE EmployeeAssignments.IDEmployee = EmployeeGroups.IDEmployee )  = 0 "
                    End If

                    strQuery &= " ) "
                End If
                strQuery &= " ORDER BY FullGroupName , EmployeeName"

                strQuery = strQuery & SQLServerHint.GetSQLHint(SQLServerHint.SelectHinted.LoadRowsByCalendar)

                Dim dTbl As System.Data.DataTable = CreateDataTable(strQuery, Nothing, , False)

                Dim oCalendarRow As roCalendarRow = Nothing

                Dim oPermissionPassport As Permission = Permission.None
                If _typeView = CalendarView.Planification Then
                    oPermissionPassport = WLHelper.GetPermissionOverFeature(oState.IDPassport, "Calendar.Scheduler", "U")
                Else
                    oPermissionPassport = WLHelper.GetPermissionOverFeature(oState.IDPassport, "Calendar.Highlight", "U")
                End If

                Dim oSchedulerRemarks As Scheduler.roSchedulerRemarks = Nothing
                If _typeView = CalendarView.Review Then

                    Dim oSchduleState = New VTBusiness.Scheduler.roSchedulerState()
                    roBusinessState.CopyTo(oState, oSchduleState)

                    oSchedulerRemarks = New Scheduler.roSchedulerRemarks(oSchduleState.IDPassport, oSchduleState)
                End If

                'Cargar los datos de los empleados seleccionados
                If dTbl IsNot Nothing Then
                    Dim intPos As Integer = 0

                    Dim tbCauses As DataTable = CreateDataTable("@SELECT# ID, Name, ShortName FROM CAUSES")
                    Dim intAssignments As Integer = roTypes.Any2Integer(ExecuteScalar("@SELECT# isnull(count(*),0) as total  from Assignments"))
                    Dim oCalendarRowPeriodDataState As New roCalendarRowPeriodDataState(oState.IDPassport)
                    Dim oCalendarRowSummaryState As New roCalendarRowSummaryDataState(oState.IDPassport)
                    Dim oCalendarRowSummary As New roCalendarRowSummaryDataManager(oCalendarRowSummaryState)
                    Dim oStateEmployee As New Employee.roEmployeeState(oState.IDPassport, oState.Context, oState.ClientAddress)

                    Dim oProgrammedAbsState As New Absence.roProgrammedAbsenceState(oState.IDPassport, oState.Context, oState.ClientAddress)
                    Dim oProgrammedHolidaysState As New roProgrammedHolidayState(oState.IDPassport, oState.Context, oState.ClientAddress)
                    Dim oProgrammedHolidayManager As New roProgrammedHolidayManager()
                    Dim oProgrammedOvertimeyManager As New roProgrammedOvertimeManager()
                    Dim oProgrammedOvertimesState As New roProgrammedOvertimeState(oState.IDPassport, oState.Context, oState.ClientAddress)
                    Dim oContractState As New Contract.roContractState(oState.IDPassport, oState.Context, oState.ClientAddress)

                    For Each oRowEmp As DataRow In dTbl.Rows
                        Dim bolFilter As Boolean = True
                        ' En caso necesario filtramos los empleados
                        If sFilterStatus <> "" Then
                            Dim bolCurrentEmployee As Boolean = (roTypes.Any2Double(oRowEmp("CurrentEmployee")) > 0)
                            Dim intStatus As Integer = 0

                            If Not bolCurrentEmployee And oRowEmp("Begindate") >= Now Then
                                ' El empleado es una futura incorporación
                                intStatus = 4
                            Else
                                If Not bolCurrentEmployee And oRowEmp("Begindate") < Now Then
                                    ' El empleado es una baja
                                    intStatus = 3
                                Else
                                    If bolCurrentEmployee And oRowEmp("Enddate") <> CDate("01/01/2079") Then
                                        ' Empleado con movilidad
                                        intStatus = 2
                                    Else
                                        ' Empleado normal
                                        intStatus = 1
                                    End If
                                End If
                            End If

                            bolFilter = ((sFilterStatus.Substring(0, 1) = "1" AndAlso intStatus = 1) OrElse
                                         (sFilterStatus.Substring(1, 1) = "1" AndAlso intStatus = 2) OrElse
                                         (sFilterStatus.Substring(2, 1) = "1" AndAlso intStatus = 3) OrElse
                                         (sFilterStatus.Substring(3, 1) = "1" AndAlso intStatus = 4))
                        End If

                        ' Si el grupo obtenido está dentro de la lista que puede gestionar el Supervisor
                        If bolFilter Then
                            Dim bolAddEmployee As Boolean = True

                            If oPermissionPassport < Permission.Read Then bolAddEmployee = False

                            If bolAddEmployee Then
                                ' Añadimos los datos del empleado
                                oCalendarRow = New roCalendarRow
                                intPos += 1
                                oCalendarRow.Pos = intPos

                                ' Datos generales del empleado
                                oCalendarRow.EmployeeData = New roCalendarRowEmployeeData
                                oCalendarRow.EmployeeData.IDEmployee = oRowEmp("IDEmployee")
                                oCalendarRow.EmployeeData.IDGroup = oRowEmp("IDGroup")
                                oCalendarRow.EmployeeData.GroupName = oRowEmp("FullGroupName")
                                oCalendarRow.EmployeeData.EmployeeName = oRowEmp("EmployeeName")
                                oCalendarRow.EmployeeData.FreezingDate = oRowEmp("LockDate")

                                ' el permiso sobre el empleado siempre es el de la funcionalidad
                                oCalendarRow.EmployeeData.Permission = CInt(oPermissionPassport)

                                ' En el caso que tenga licencia de Scheduling,  cargamos los puestos que puede cubrir
                                If bolLicenseHRScheduling AndAlso intAssignments > 0 Then
                                    Dim oCalendarRowAssignmentsDataState As New roCalendarRowAssignmentsDataState(oState.IDPassport)
                                    oCalendarRow.EmployeeData.Assignments = roCalendarRowAssignmentsDataManager.LoadAssignments(oRowEmp("IDEmployee"), oCalendarRowAssignmentsDataState)
                                End If

                                oCalendarRow.EmployeeData.BackgroundColor = "#FFFFFF"

                                Dim tbProgrammedAbsences As DataTable = Nothing

                                ' Datos del periodo seleccionado
                                oCalendarRow.PeriodData = roCalendarRowPeriodDataManager.LoadCellsByCalendar(_FirstDay, _LastDay, oRowEmp("IDEmployee"), oRowEmp("IDGroup"), oCalendarRow.EmployeeData.Permission, oParameters, _typeView, _detailLevel, oConceptNormalWork, oConceptAbsence, oConceptOverWorking, oCalendarRowPeriodDataState, bolLicenseHRScheduling, oShiftCache, oRowEmp("Begindate"), oRowEmp("Enddate"),, tbCauses, tbProgrammedAbsences, oStateEmployee, oContractState, oProgrammedAbsState, oProgrammedOvertimesState, oProgrammedHolidaysState, oProgrammedOvertimeyManager, oProgrammedHolidayManager, oSchedulerRemarks, bLoadSeatingCapacity, True)

                                ' Datos resumen del empleado
                                oCalendarRow.SummaryData = oCalendarRowSummary.Load(_FirstDay, _LastDay, oRowEmp("IDEmployee"), oParameters, oConcept, idShiftHolidays, oConceptHolidays, oStateEmployee, oCalRulesManager, oProgrammedAbsState, oProgrammedHolidaysState, oProgrammedHolidayManager, oProgrammedOvertimeyManager, oProgrammedOvertimesState, tbProgrammedAbsences)

                                oRet.Add(oCalendarRow)
                            End If
                        End If
                    Next
                End If

                bolRet = True
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roCalendarRowManager::LoadRowsByCalendar")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCalendarRowManager::LoadRowsByCalendar")
            End Try

            Return oRet.ToArray

        End Function

        Public Shared Function LoadShiftDataByCalendar(ByVal oCalendarData As roCalendarRow(), ByRef oState As roCalendarRowState) As Generic.List(Of roCalendarShift)
            ' Obtenemos los horarios utilizados en el calendario que sea de tipo por horas o con Puestos
            Dim oRet As New Generic.List(Of roCalendarShift)
            Dim lstShifts As New Generic.List(Of Integer)
            Dim bolRet As Boolean = False
            Dim oCalendarShift As roCalendarShift = Nothing

            Try

                If oCalendarData Is Nothing Then
                    Return oRet
                End If

                Dim oCalendarShiftManager As New roCalendarShiftManager

                For Each oCalendar As roCalendarRow In oCalendarData
                    For Each oDayData As roCalendarRowDayData In oCalendar.PeriodData.DayData
                        ' Si el horario es tiene complementarias o tiene puestos
                        If oDayData.MainShift IsNot Nothing AndAlso oDayData.MainShift.ID > 0 AndAlso
                            (oDayData.MainShift.ExistComplementaryData OrElse oDayData.AllowAssignment) AndAlso Not lstShifts.Contains(oDayData.MainShift.ID) Then
                            oCalendarShift = New roCalendarShift
                            oCalendarShift = oCalendarShiftManager.GetShiftDefinition(oDayData.MainShift.ID)
                            oRet.Add(oCalendarShift)
                            lstShifts.Add(oDayData.MainShift.ID)
                        End If
                    Next
                Next

                bolRet = True
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roCalendarRowManager::LoadShiftDataByCalendar")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCalendarRowManager::LoadShiftDataByCalendar")
            End Try

            Return oRet

        End Function

        Public Shared Function GetGroupsFromNode(ByVal _IDNode As Integer, ByRef oState As roCalendarRowState) As String
            Dim strEmployeefilter As String = String.Empty
            Dim bolret As Boolean = False

            Try

                ' Obtenemos todos los grupos del nodo seleccionado
                'Dim oNodeManager As New VTSecurity.roSecurityNodeManager()
                'Dim oNode As roSecurityNode = oNodeManager.Load(_IDNode, True, True)
                'If oNode IsNot Nothing Then
                '    If oNode.Groups IsNot Nothing Then
                '        For Each oGroup As roSecurityNodeGroup In oNode.Groups
                '            strEmployeefilter += "A" & oGroup.IDGroup & ","
                '        Next
                '    End If

                '    If oNode.Children IsNot Nothing Then
                '        For Each oNodeChildren As roSecurityNode In oNode.Children
                '            strEmployeefilter += GetGroupsFromNode(oNodeChildren.ID, oState)
                '        Next
                '    End If
                'End If

                bolret = True
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roCalendarRowManager::GetGroupsFromNode")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCalendarRowManager::GetGroupsFromNode")
            End Try

            Return strEmployeefilter

        End Function

        Public Shared Function GetEmployeeListFromNode(ByVal _IDNode As Integer, ByRef oState As roCalendarRowState, Optional ByVal _DayStart As DateTime = Nothing, Optional ByVal _DayEnd As DateTime = Nothing) As String
            Dim strRet As String = String.Empty
            Dim bolRet As Boolean

            Try

                Dim strEmployeefilter As String = String.Empty
                Dim strEmployees As String = String.Empty

                ' Obtenemos todos los grupos del nodo seleccionado

                strEmployeefilter = GetGroupsFromNode(_IDNode, oState)

                ' Obtenemos la lista de empleados
                If strEmployeefilter.Length > 0 Then strEmployeefilter = strEmployeefilter.Substring(0, strEmployeefilter.Length - 1)
                If strEmployeefilter <> String.Empty Then


                    Dim lstEmployees As Generic.List(Of Integer) = roSelector.GetEmployeeList(oState.IDPassport, "Calendar.Scheduler", "U", Permission.Read,
                                                                                                        strEmployeefilter, "11110", "", False, _DayStart, _DayEnd)

                    Dim empBuilder As New StringBuilder
                    For Each iID As Integer In lstEmployees
                        empBuilder.Append("B" & iID.ToString & ",")
                    Next
                    If empBuilder.Length > 0 Then strEmployees = empBuilder.ToString(0, empBuilder.Length - 1)
                End If

                strRet = strEmployees
                bolRet = True
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roCalendarRowManager::GetEmployeeListFromNode")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCalendarRowManager::GetEmployeeListFromNode")
            End Try

            Return strRet
        End Function

        Public Shared Function GetAssignmentsFilter(ByVal _IDNode As Integer, ByRef oState As roCalendarRowState, ByVal _DayStart As DateTime, ByVal _DayEnd As DateTime) As String
            Dim strRet As String = String.Empty
            Dim bolRet As Boolean = False
            Dim strSQL As String = ""

            Try

                strSQL = "@SELECT# distinct IDAssignment from ProductiveUnit_Mode_Positions where IDMode in( " &
                    "@SELECT# distinct IDMode from DailyBudgets where IDNode = " & _IDNode &
                    " and Date between " & roTypes.Any2Time(_DayStart).SQLSmallDateTime & " and " & roTypes.Any2Time(_DayEnd).SQLSmallDateTime & ")"

                Dim dtAssignments As DataTable = DataLayer.AccessHelper.CreateDataTable(strSQL)

                If dtAssignments IsNot Nothing AndAlso dtAssignments.Rows.Count > 0 Then
                    For Each oRow As DataRow In dtAssignments.Rows
                        If strRet <> String.Empty Then strRet &= ","
                        strRet &= oRow("IDAssignment")
                    Next
                End If

                bolRet = True
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roCalendarRowManager::GetAssignmentsFilter")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCalendarRowManager::GetAssignmentsFilter")
            End Try

            Return strRet

        End Function

#End Region

    End Class

End Namespace