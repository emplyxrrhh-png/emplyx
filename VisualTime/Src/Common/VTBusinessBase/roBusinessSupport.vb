Imports System.Data.Common
Imports System.Data.SqlClient
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.roTypes

Public Class roBusinessSupport

    Public Shared Function GetAllEmployees(ByVal strWhere As String, ByVal Feature As String, ByVal Type As String, ByVal _State As roBusinessState, Optional ByVal bAddImage As Boolean = False) As DataTable

        Dim oRet As DataTable = Nothing

        Try

            Dim strSQL As String
            strSQL = "@SELECT# * from (@SELECT# distinct sysrovwAllEmployeeGroups.IDEmployee, sysrovwAllEmployeeGroups.IDEmployee AS ID, sysrovwAllEmployeeGroups.EmployeeName,sysrovwAllEmployeeGroups.BeginDate, sysrovwAllEmployeeGroups.Path, sysrovwAllEmployeeGroups.GroupName, sysrovwAllEmployeeGroups.FullGroupName FROM sysrovwAllEmployeeGroups) ct WHERE 1=1 "
            If strWhere <> "" Then strSQL &= "AND " & strWhere & " "

            If Feature <> String.Empty Then
                strSQL = " @SELECT# tmp.* FROM (" & strSQL & ") tmp " & roSelector.GetEmployeePermissonInnerJoin(_State.IDPassport, Permission.Read, Feature, Type) & " ORDER BY EmployeeName"
            Else
                strSQL = strSQL & " ORDER BY EmployeeName "
            End If

            strSQL += " " & SQLServerHint.GetSQLHint(SQLServerHint.SelectHinted.GetAllEmployees)

            oRet = CreateDataTable(strSQL, )
        Catch ex As DbException
            _State.UpdateStateInfo(ex, "roBusinessSupport::GetAllEmployees")
        Catch ex As Exception
            _State.UpdateStateInfo(ex, "roBusinessSupport::GetAllEmployees")
        Finally

        End Try

        Return oRet

    End Function
    Public Shared Function GetEmployees(ByVal strWhere As String, ByVal Feature As String, ByVal Type As String, ByVal _State As roBusinessState, Optional ByVal bAddImage As Boolean = False) As DataTable

        Dim oRet As DataTable = Nothing

        Try

            Dim strSQL As String
            strSQL = "@SELECT# " &
                     "sysrovwAllEmployeeGroups.GroupName, sysrovwAllEmployeeGroups.[Path], sysrovwAllEmployeeGroups.SecurityFlags, " &
                     "sysrovwAllEmployeeGroups.IDEmployee, sysrovwAllEmployeeGroups.IDGroup, sysrovwAllEmployeeGroups.EmployeeName, " &
                     "sysrovwAllEmployeeGroups.BeginDate, sysrovwAllEmployeeGroups.EndDate, sysrovwAllEmployeeGroups.CurrentEmployee,  " &
                     "sysrovwAllEmployeeGroups.AttControlled, sysrovwAllEmployeeGroups.AccControlled, sysrovwAllEmployeeGroups.JobControlled,  " &
                     "sysrovwAllEmployeeGroups.ExtControlled, sysrovwAllEmployeeGroups.RiskControlled, sysrovwAllEmployeeGroups.FullGroupName,  " &
                     "sysrovwAllEmployeeGroups.IDCompany, sysrovwAllEmployeeGroups.isTransfer, sysrovwAllEmployeeGroups.CostCenter, " &
                     "EmployeeContracts.IDContract, EmployeeContracts.IDCard "

            If bAddImage Then
                strSQL &= ", Employees.Image "
            End If

            strSQL &= " FROM sysrovwAllEmployeeGroups WITH (NOLOCK) INNER JOIN Employees WITH (NOLOCK) " &
                                "ON sysrovwAllEmployeeGroups.IDEmployee = Employees.[ID] " &
                                "INNER JOIN EmployeeContracts WITH (NOLOCK) " &
                                "ON sysrovwAllEmployeeGroups.IDEmployee = EmployeeContracts.IDEmployee " &
                         "WHERE GETDATE() BETWEEN EmployeeContracts.BeginDate AND EmployeeContracts.EndDate "
            If strWhere <> "" Then strSQL &= "AND " & strWhere & " "

            If Feature <> String.Empty Then
                strSQL = " @SELECT# tmp.* FROM (" & strSQL & ") tmp " & roSelector.GetEmployeePermissonInnerJoin(_State.IDPassport, Permission.Read, Feature, Type) & " ORDER BY EmployeeName"

                'strSQL &= " AND " & WLHelper.GetEmployeePermissonWhereClause(_State.IDPassport, Permission.Read, Feature, Type, " ORDER BY Employees.Name", "sysrovwAllEmployeeGroups.IDEmployee")
            End If

            oRet = CreateDataTable(strSQL, )

            ' Configuro la tabla de resultado
            Dim oDataColumn As New DataColumn
            oDataColumn.ColumnName = "Type"
            oRet.Columns.Add(oDataColumn)

            For Each oDataRow As DataRow In oRet.Rows
                If oDataRow("CurrentEmployee") = 0 And oDataRow("Begindate") >= Now Then
                    ' El empleado es una futura incorporación
                    oDataRow("Type") = 4
                Else
                    If oDataRow("CurrentEmployee") = 0 And oDataRow("Begindate") < Now Then
                        ' El empleado es una baja
                        oDataRow("Type") = 3
                    Else
                        If oDataRow("CurrentEmployee") = 1 And oDataRow("Enddate") <> CDate("01/01/2079") Then
                            ' Empleado con movilidad
                            oDataRow("Type") = 2
                        Else
                            ' Empleado normal
                            oDataRow("Type") = 1
                        End If
                    End If
                End If
            Next

            oRet.AcceptChanges()
        Catch ex As DbException
            _State.UpdateStateInfo(ex, "roBusinessSupport::GetEmployees")
        Catch ex As Exception
            _State.UpdateStateInfo(ex, "roBusinessSupport::GetEmployees")
        Finally

        End Try

        Return oRet

    End Function

    Public Shared Function GetEmployeesStatus(ByVal idPassport As Integer, oLogState As roBusinessState) As DataTable

        Dim oRet As DataTable = Nothing

        Try
            Dim strSQL As String
            strSQL = "@SELECT# es.* FROM sysrovwEmployeeStatus es With (NOLOCK)" &
                    " INNER JOIN sysrovwSecurity_PermissionOverEmployees poe on poe.IDPassport = " & idPassport.ToString & " And poe.IDEmployee = es.IDEmployee AND CONVERT(DATE,GETDATE()) between poe.BeginDate and poe.EndDate" &
                    " INNER JOIN sysrovwSecurity_PermissionOverFeatures pof on pof.IDPassport = " & idPassport.ToString & " AND pof.FeatureType = 'U' AND pof.FeatureAlias = 'Employees' AND Permission >=3" &
                    " ORDER BY EmployeeName ASC"

            strSQL = strSQL & SQLServerHint.GetSQLHint(SQLServerHint.SelectHinted.GetEmployeesStatus)

            oRet = CreateDataTable(strSQL, )

        Catch ex As DbException
            oLogState.UpdateStateInfo(ex, "roBusinessSupport::GetEmployeesStatus")
        Catch ex As Exception
            oLogState.UpdateStateInfo(ex, "roBusinessSupport::GetEmployeesStatus")
        End Try

        Return oRet

    End Function

    ''' <summary>
    ''' Retorna información completa del estado actual del empleado, para Dashboard VTLive
    ''' </summary>
    ''' <param name="employees"></param>
    ''' <param name="oActiveConnection"></param>
    ''' <param name="oActiveTransaction"></param>
    ''' <returns></returns>
    Public Shared Function GetEmployeesDashboardStatus(ByVal idPassport As Integer, ByVal groups As List(Of Integer), Optional ByVal isConsultor As Boolean = False) As DataTable

        Dim oRet As DataTable = Nothing
        Dim oLogState As New roBusinessState("Common.BaseState", "")

        Try

            Dim groupsFiltered = String.Join(",", groups.ToArray)
            Dim employeesFilter = "-1"

            If groups.Contains(-1) Then

                Dim sExclusions As String = $"@SELECT# pe.IDEmployee FROM sysropassports_Employees pe  
                                    INNER JOIN sysrovwSecurity_PermissionOverEmployeeAndFeature poe on poe.IDPassport = {idPassport.ToString} And poe.IDEmployee = pe.IDEmployee
                                        AND CONVERT(DATE,GETDATE()) between poe.BeginDate and poe.EndDate AND poe.IdFeature = 1 AND poe.FeaturePermission >=1
                                    INNER JOIN employeegroups eg on eg.IDEmployee = pe.IDEmployee and convert(date,getdate()) between eg.BeginDate and eg.EndDate
                                        AND eg.IDGroup not in(@SELECT# idgroup from sysrovwSecurity_PermissionOverGroups where IdPassport = {idPassport.ToString})
                                    WHERE pe.IdPassport = {idPassport.ToString} and pe.Permission = 1"
                Dim dtExclusions As DataTable = CreateDataTable(sExclusions, )

                If dtExclusions IsNot Nothing AndAlso dtExclusions.Rows.Count > 0 Then
                    employeesFilter = String.Join(",", dtExclusions.AsEnumerable().Select(Function(x) x.Field(Of Integer)("IDEmployee")).ToArray())
                End If
            End If

            Dim strSQL As String = $"@SELECT# st.* FROM sysrovwDashboardEmployeeStatus st
                            INNER JOIN sysrovwSecurity_PermissionOverEmployeeAndFeature poe on poe.IDPassport = {idPassport} And poe.IDEmployee = st.IDEmployee AND CONVERT(DATE,GETDATE()) between poe.BeginDate and poe.EndDate and poe.IdFeature = 1 and poe.FeaturePermission > 0
                            where (st.IDGroup in ({groupsFiltered}) OR st.IDEmployee in ({employeesFilter})) ORDER BY st.EmployeeName ASC"

            strSQL = strSQL & SQLServerHint.GetSQLHint(SQLServerHint.SelectHinted.GetEmployeesDashboardStatus)

            oRet = CreateDataTable(strSQL, )

        Catch ex As DbException
            oLogState.UpdateStateInfo(ex, "roBusinessSupport::GetEmployeesDashboardStatus")
        Catch ex As Exception
            oLogState.UpdateStateInfo(ex, "roBusinessSupport::GetEmployeesDashboardStatus")
        End Try

        Return oRet

    End Function

    ''' <summary>
    ''' Devuelve la tabla con los empleados que están ahora en la zona, y toda su información relativa
    ''' </summary>
    ''' <param name="idPassport"></param>
    ''' <param name="zone"></param>
    ''' <param name="isConsultor"></param>
    ''' <param name="oActiveConnection"></param>
    ''' <param name="oActiveTransaction"></param>
    ''' <returns></returns>
    Public Shared Function GetEmployeesZoneStatus(ByVal idPassport As Integer, ByVal zone As Integer, Optional ByVal isConsultor As Boolean = False) As DataTable

        Dim oRet As DataTable = Nothing
        Dim oLogState As New roBusinessState("Common.BaseState", "")

        Try

            Dim strSQL As String = "@SELECT# sysrovwZoneEmployeeStatus.*, isnull(Terminals.Type, '') as LastPunchTerminalType " &
                     " FROM sysrovwZoneEmployeeStatus WITH (NOLOCK) " &
                     " INNER JOIN sysrovwSecurity_PermissionOverEmployees poe on poe.IDPassport = " & idPassport.ToString & " And poe.IDEmployee = sysrovwZoneEmployeeStatus.IDEmployee AND CONVERT(DATE,GETDATE()) between poe.BeginDate and poe.EndDate" &
                     " INNER JOIN sysrovwSecurity_PermissionOverFeatures pof on pof.IDPassport = " & idPassport.ToString & " AND pof.FeatureType = 'U' AND pof.FeatureAlias = 'Employees' AND Permission >=3" &
                     " LEFT JOIN Terminals WITH (NOLOCK) ON Terminals.ID = sysrovwZoneEmployeeStatus.LastPunchIDTerminal " &
                     " WHERE IdZone = " & zone & " AND (isnull(Terminals.Type, '') <> 'LivePortal' OR (Terminals.Type = 'LivePortal' AND LastPunchActualType <>2)) " &
                     " ORDER BY EmployeeName ASC"

            oRet = CreateDataTable(strSQL, )

        Catch ex As DbException
            oLogState.UpdateStateInfo(ex, "roBusinessSupport::GetEmployeesZoneStatus")
        Catch ex As Exception
            oLogState.UpdateStateInfo(ex, "roBusinessSupport::GetEmployeesZoneStatus")
        End Try

        Return oRet

    End Function

    ''' <summary>
    ''' Devuelve los empleados que estuvieron en la zona durante la última hora, y que no siguen en ella.
    ''' </summary>
    ''' <param name="idPassport"></param>
    ''' <param name="zone"></param>
    ''' <param name="isConsultor"></param>
    ''' <param name="oActiveConnection"></param>
    ''' <param name="oActiveTransaction"></param>
    ''' <returns></returns>
    Public Shared Function GetEmployeesInZoneDuringLastHour(ByVal idPassport As Integer, ByVal idZone As Integer) As DataTable

        Dim oRet As DataTable = Nothing
        Dim oLogState As New roBusinessState("Common.BaseState", "")

        Try

            Dim strSQL As String = "@SELECT# sysrovwLastMonthZoneMoves.*, isnull(Terminals.Type, '') AS TerminalOutType, Employees.Name, Employees.Image as EmployeeImage " &
                    " FROM sysrovwLastMonthZoneMoves WITH (NOLOCK) " &
                    " INNER JOIN Employees WITH (NOLOCK) ON Employees.ID = sysrovwLastMonthZoneMoves.IDEmployee " &
                    " INNER JOIN sysrovwSecurity_PermissionOverEmployees poe on poe.IDPassport = " & idPassport.ToString & " And poe.IDEmployee = sysrovwLastMonthZoneMoves.IDEmployee AND CONVERT(DATE,GETDATE()) between poe.BeginDate and poe.EndDate" &
                    " INNER JOIN sysrovwSecurity_PermissionOverFeatures pof on pof.IDPassport = " & idPassport.ToString & " AND pof.FeatureType = 'U' AND pof.FeatureAlias = 'Employees' AND Permission >=3" &
                    " LEFT JOIN Terminals WITH (NOLOCK) ON Terminals.ID = sysrovwLastMonthZoneMoves.IDterminalOut " &
                    " WHERE IDZoneIn = " & idZone.ToString & " AND DateTimeOut >= " & roTypes.Any2Time(Now.AddHours(-1)).SQLSmallDateTime & " AND (ISNULL(IDZoneOut,0) <> " & idZone.ToString & " OR (isnull(Terminals.Type, '') = 'LivePortal' AND ActualTypeOut = 2))"

            oRet = CreateDataTable(strSQL, )

        Catch ex As DbException
            oLogState.UpdateStateInfo(ex, "roBusinessSupport::GetEmployeesInZoneDuringLastHour")
        Catch ex As Exception
            oLogState.UpdateStateInfo(ex, "roBusinessSupport::GetEmployeesInZoneDuringLastHour")
        End Try

        Return oRet

    End Function

    Public Shared Function GetCurrentEmployeesZoneStatus(ByVal strWhere As String, ByVal _State As roBusinessState, Optional ByVal bAddImage As Boolean = False) As DataTable

        Dim oRet As DataTable = Nothing

        Try

            Dim strSQL As String
            strSQL = "@SELECT# sysrovwCurrentEmployeesZoneStatus.*, Employees.Name "

            If bAddImage Then
                strSQL &= ", Employees.Image "
            End If

            strSQL &= " FROM sysrovwCurrentEmployeesZoneStatus INNER JOIN Employees " &
                                "ON sysrovwCurrentEmployeesZoneStatus.IDEmployee = Employees.[ID] "

            If strWhere <> "" Then strSQL &= " where " & strWhere & " "

            oRet = CreateDataTable(strSQL, )

            oRet.AcceptChanges()
        Catch ex As DbException
            _State.UpdateStateInfo(ex, "roBusinessSupport::GetEmployees")
        Catch ex As Exception
            _State.UpdateStateInfo(ex, "roBusinessSupport::GetEmployees")
        Finally

        End Try

        Return oRet

    End Function

    Public Shared Function LaboralDaysInPeriod(ByVal IDEmployee As Integer, ByVal xBeginPeriod As DateTime, ByVal xEndPeriod As DateTime,
                                            ByVal _State As roBusinessState) As Integer
        Dim intRet As Integer = 0

        Dim tb As DataTable = Nothing
        Try

            Dim strSQL As String = "@SELECT# COUNT(*) " &
                                       "FROM DailySchedule LEFT JOIN Shifts Shifts1 ON DailySchedule.IDShift1 = Shifts1.[ID] " &
                                                          "LEFT JOIN Shifts Shifts2 ON DailySchedule.IDShift2 = Shifts2.[ID] " &
                                                          "LEFT JOIN Shifts Shifts3 ON DailySchedule.IDShift3 = Shifts3.[ID] " &
                                                          "LEFT JOIN Shifts Shifts4 ON DailySchedule.IDShift4 = Shifts4.[ID] " &
                                                          "LEFT JOIN Shifts ShiftsBase ON DailySchedule.IDShiftBase = ShiftsBase.[ID] " &
                                       "WHERE DailySchedule.IDEmployee = " & IDEmployee.ToString & " AND " &
                                             "DailySchedule.[Date] BETWEEN CONVERT(smalldatetime, '" & Format(xBeginPeriod.Date, "dd/MM/yyyy") & "', 103) AND " &
                                                                          "CONVERT(smalldatetime, '" & Format(xEndPeriod.Date, "dd/MM/yyyy") & "', 103) AND " &
                                             "(((Shifts1.ExpectedWorkingHours <> 0 OR Shifts2.ExpectedWorkingHours <> 0 OR Shifts3.ExpectedWorkingHours <> 0 OR Shifts4.ExpectedWorkingHours <> 0) AND (IsHolidays IS NULL OR IsHolidays=0)) OR ( (ISNULL(IsHolidays,0) = 1) AND ShiftsBase.ExpectedWorkingHours <> 0 ))"
            tb = CreateDataTable(strSQL, )
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                intRet = Any2Integer(tb.Rows(0)(0))
            End If
        Catch ex As DbException
            _State.UpdateStateInfo(ex, "roBusinessSupport::LaboralDaysInPeriod")
        Catch ex As Exception
            _State.UpdateStateInfo(ex, "roBusinessSupport::LaboralDaysInPeriod")
        Finally

        End Try

        Return intRet

    End Function

    Public Shared Function GetEmployeeForecastsInPeriod(ByVal IDEmployee As Integer, ByVal _BeginDate As Date, ByRef _State As roBusinessState) As DataTable

        Dim tb As DataTable = Nothing
        Try

            Dim strSQL As String = "@SELECT# * FROM (@SELECT# AbsenceID,IDEmployee,(@SELECT# Name from Causes where id = IDcause) as IDCause, CONVERT(NVARCHAR(4000),Description) as Description,BeginDate AS BeginDate, ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) as ENDDATE, null as BeginTime, null as EndTime, 0 as Duration, 'ProgrammedAbsence' as AbsenceType  from ProgrammedAbsences " &
                                    " UNION " &
                                    " @SELECT# ID,IDEmployee,(@SELECT# Name from Causes where id = IDcause) as IDCause, CONVERT(NVARCHAR(4000),Description) as Description,Date AS BeginDate,FinishDate as ENDDATE,BeginTime,EndTime, Duration, 'ProgrammedCause' as AbsenceType  from ProgrammedCauses " &
                                    " UNION  " &
                                    " @SELECT# ID,IDEmployee,(@SELECT# Name from Causes where id = IDcause) as IDCause, CONVERT(NVARCHAR(4000),Description) as Description,BeginDate AS BeginDate,EndDate as ENDDATE,BeginTime,EndTime, Duration, 'ProgrammedOverTime' as AbsenceType  from ProgrammedOvertimes " &
                                    " UNION  " &
                                    " @SELECT# ID,IDEmployee,(@SELECT# Name from Causes where id = IDcause) as IDCause, CONVERT(NVARCHAR(4000),Description) as Description,Date AS BeginDate,Date as ENDDATE,BeginTime,EndTime, Duration, 'ProgrammedHoliday' as AbsenceType  from ProgrammedHolidays) tmp " &
                                    " WHERE IDEmployee = " & IDEmployee & " and " & roTypes.Any2Time(_BeginDate).SQLDateTime & " between BeginDate and EndDate"

            tb = CreateDataTable(strSQL, )
        Catch ex As DbException
            _State.UpdateStateInfo(ex, "roBusinessSupport::GetEmployeeForecastsInPeriod")
        Catch ex As Exception
            _State.UpdateStateInfo(ex, "roBusinessSupport::LaboralDaysInPeriod")
        Finally

        End Try

        Return tb

    End Function

    ''' <summary>
    ''' Devuelve una cadena con los id's de los empleados obtenenidos del filter recibido
    ''' el parametro where deberá contener toda la expresio select completa
    ''' La cadena devuelta tendrá los codigos separados por comas: 1,4,5,14,23
    ''' Si el filter no retorna empleados, se retornará una cadena vacía
    ''' también se tiene en cuenta si se tienen permisos para consultar campos de la ficha del empleado
    ''' </summary>
    ''' <param name="sWhere"></param>
    ''' <returns>string</returns>
    ''' <remarks></remarks>
    Public Shared Function GetEmployeeListFromFilter(ByVal sWhere As String, ByRef _State As roBusinessState) As String

        Dim tb As DataTable = Nothing

        Dim sList As String = ""

        Dim strFeature = "Employees.UserFields.Information"
        Try

            tb = CreateDataTable(sWhere, )

            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                For Each oRow As DataRow In tb.Rows
                    If WLHelper.HasFeaturePermissionByEmployee(_State.IDPassport, strFeature, Permission.Read, oRow("ID")) Then
                        sList &= oRow("ID") & ","
                    End If
                Next
            End If

            If sList.EndsWith(",") Then sList = sList.Substring(0, sList.Length() - 1)
        Catch ex As DbException
            _State.UpdateStateInfo(ex, "roBusinessCommon::GetEmployeeListFromFilter")
        Catch ex As Exception
            _State.UpdateStateInfo(ex, "roBusinessCommon::GetEmployeeListFromFilter")
        Finally

        End Try

        Return sList

    End Function

#Region "Querys"

    ''' <summary>
    ''' Resumen de previsiones de ausencia por dia
    ''' </summary>
    ''' <param name="IDEmployee">ID de empleado a consultar</param>
    ''' <param name="xCurrentDate">Fecha actual a consultar</param>
    ''' <param name="xBeginPeriod">Devuelve fecha inicial del periodo</param>
    ''' <param name="xEndPeriod">Devuelve fecha final del periodo</param>
    ''' <param name="intDone">Devuelve núm. de días/horas ya disfrutados</param>
    ''' <param name="intPending">Devuelve número de días/horas solicitados pendientes de procesa</param>
    ''' <param name="intLasting">Devuelve el número de días/horas aprobados pendientes de disfrutar</param>
    ''' <param name="intDisponible">Devuelve los días/horas disponibles</param>
    ''' <param name="_State"></param>
    ''' <param name="oActiveConnection">Conexion activa (opcional)</param>
    ''' <returns>Devuelve TRUE si se realiza la función correctamente</returns>
    ''' <remarks></remarks>
    Public Shared Function PlannedAbsencesResumeQuery(ByVal IDEmployee As Integer, ByVal IDCause As Integer, ByVal xCurrentDate As DateTime, ByRef xBeginPeriod As DateTime, ByRef xEndPeriod As DateTime,
                                             ByVal xCauseDate As DateTime, ByRef intPending As Double, ByRef intLasting As Double, ByRef intDisponible As Double,
                                             ByVal _State As roBusinessState, ByVal bolApplyWorkDaysOnConcept As Boolean, ByVal strConceptType As String) As Boolean
        Dim bolRet As Boolean = False

        Try

            ' Obtenemos el periodo de calculo
            ' Actualmente a nivel de reglas de solicitudes solo puede ser Anual / Por contrato
            Dim strDefaultQuery As String = roTypes.Any2String(ExecuteScalar("@SELECT# ISNULL(DefaultQuery , 'Y') FROM Concepts WHERE ID in(@SELECT# isnull(IDConceptBalance,0) from causes where id =" & IDCause.ToString & ")"))
            If strDefaultQuery.Length = 0 Then strDefaultQuery = "Y"

            ' Determinar inicio i final del periodo anual
            xBeginPeriod = roParameters.BeginYearPeriod(xCurrentDate)
            ' A la fecha final le añadimos un año por si permiten solicitar previsiones de un año para otro
            xEndPeriod = roParameters.EndYearPeriod(xCurrentDate).AddYears(1)

            Dim lstDates As New Generic.List(Of DateTime)
            Dim strSQL As String = "@SELECT# BeginDate, EndDate From EmployeeContracts WHERE IDEmployee = " & IDEmployee & " AND " &
                                       "BeginDate <= " & Any2Time(xCauseDate).SQLSmallDateTime & " AND EndDate >= " & Any2Time(xCauseDate).SQLSmallDateTime
            Dim oSqlCommand As DbCommand = CreateCommand(strSQL)
            Dim rd As DbDataReader = oSqlCommand.ExecuteReader()
            If rd.Read() Then
                If strDefaultQuery = "C" Then
                    ' En el caso de saldos por contrato
                    ' la fecha de inicio del periodo es el inicio de contrato
                    ' y la fecha de final del periodo es la misma del final del contrato
                    xBeginPeriod = rd("BeginDate")
                    xEndPeriod = rd("EndDate")
                End If
                lstDates.Add(rd("BeginDate"))
                lstDates.Add(rd("EndDate"))
            End If
            rd.Close()

            If lstDates.Count > 0 And strDefaultQuery = "Y" Then
                If xBeginPeriod < lstDates(0) Then xBeginPeriod = lstDates(0)
            End If

            ' Obtener número de días/horas solicitados pendientes de procesar
            intPending = roBusinessSupport.GetPendingApprovalPlannedAbsences(IDEmployee, IDCause, xBeginPeriod, xEndPeriod, _State, bolApplyWorkDaysOnConcept, strConceptType)

            ' Obtener el número de días/horas aprobados pendientes de disfrutar
            intLasting = roBusinessSupport.GetApprovedButNotTakenPlannedAbsences(IDEmployee, IDCause, xCurrentDate, xEndPeriod, _State, bolApplyWorkDaysOnConcept, strConceptType)

            ' Obtener los días/horas disponibles de previsiones de ausencia por dias
            intDisponible = roBusinessSupport.GetDisponiblePlannedAbsences(IDEmployee, IDCause, xBeginPeriod, xEndPeriod, _State)

            bolRet = True
        Catch ex As DbException
            _State.UpdateStateInfo(ex, "roBusinessSupport::PlannedAbsencesResumeQuery")
        Catch ex As Exception
            _State.UpdateStateInfo(ex, "roBusinessSupport::PlannedAbsencesResumeQuery")
        End Try

        Return bolRet

    End Function

    ''' <summary>
    ''' Resumen de previsiones de ausencia por horas
    ''' </summary>
    ''' <param name="IDEmployee">ID de empleado a consultar</param>
    ''' <param name="xCurrentDate">Fecha actual a consultar</param>
    ''' <param name="xBeginPeriod">Devuelve fecha inicial del periodo</param>
    ''' <param name="xEndPeriod">Devuelve fecha final del periodo</param>
    ''' <param name="intDone">Devuelve núm. de días/horas ya disfrutados</param>
    ''' <param name="intPending">Devuelve número de días/horas solicitados pendientes de procesa</param>
    ''' <param name="intLasting">Devuelve el número de días/horas aprobados pendientes de disfrutar</param>
    ''' <param name="intDisponible">Devuelve los días/horas disponibles</param>
    ''' <param name="_State"></param>
    ''' <param name="oActiveConnection">Conexion activa (opcional)</param>
    ''' <returns>Devuelve TRUE si se realiza la función correctamente</returns>
    ''' <remarks></remarks>
    Public Shared Function PlannedCausesResumeQuery(ByVal IDEmployee As Integer, ByVal IDCause As Integer, ByVal xCurrentDate As DateTime, ByRef xBeginPeriod As DateTime, ByRef xEndPeriod As DateTime,
                                             ByVal xCauseDate As DateTime, ByRef intPending As Double, ByRef intLasting As Double, ByRef intDisponible As Double,
                                             ByVal _State As roBusinessState, ByVal bolApplyWorkDaysOnConcept As Boolean, ByVal strConceptType As String) As Boolean
        Dim bolRet As Boolean = False

        Try

            ' Obtenemos el periodo de calculo
            ' Actualmente a nivel de reglas de solicitudes solo puede ser Anual / Por contrato
            Dim strDefaultQuery As String = roTypes.Any2String(ExecuteScalar("@SELECT# ISNULL(DefaultQuery , 'Y') FROM Concepts WHERE ID in(@SELECT# isnull(IDConceptBalance,0) from causes where id =" & IDCause.ToString & ")"))
            If strDefaultQuery.Length = 0 Then strDefaultQuery = "Y"

            ' Determinar inicio i final del periodo anual
            xBeginPeriod = roParameters.BeginYearPeriod(xCurrentDate)
            ' A la fecha final le añadimos un año por si permiten solicitar previsiones de un año para otro
            xEndPeriod = roParameters.EndYearPeriod(xCurrentDate).AddYears(1)

            Dim lstDates As New Generic.List(Of DateTime)
            Dim strSQL As String = "@SELECT# BeginDate, EndDate From EmployeeContracts WHERE IDEmployee = " & IDEmployee & " AND " &
                                       "BeginDate <= " & Any2Time(xCauseDate).SQLSmallDateTime & " AND EndDate >= " & Any2Time(xCauseDate).SQLSmallDateTime
            Dim oSqlCommand As DbCommand = CreateCommand(strSQL)
            Dim rd As DbDataReader = oSqlCommand.ExecuteReader()
            If rd.Read() Then
                If strDefaultQuery = "C" Then
                    ' En el caso de saldos por contrato
                    ' la fecha de inicio del periodo es el inicio de contrato
                    ' y la fecha de final del periodo es la misma del final del contrato
                    xBeginPeriod = rd("BeginDate")
                    xEndPeriod = rd("EndDate")
                End If
                lstDates.Add(rd("BeginDate"))
                lstDates.Add(rd("EndDate"))
            End If
            rd.Close()

            If lstDates.Count > 0 And strDefaultQuery = "Y" Then
                If xBeginPeriod < lstDates(0) Then xBeginPeriod = lstDates(0)
            End If

            ' Obtener número de días/horas solicitados pendientes de procesar
            intPending = roBusinessSupport.GetPendingApprovalPlannedCauses(IDEmployee, IDCause, xBeginPeriod, xEndPeriod, _State, bolApplyWorkDaysOnConcept, strConceptType)

            ' Obtener el número de días/horas aprobados pendientes de disfrutar
            intLasting = roBusinessSupport.GetApprovedButNotTakenPlannedCauses(IDEmployee, IDCause, xCurrentDate, xEndPeriod, _State, bolApplyWorkDaysOnConcept, strConceptType)

            ' Obtener los días/horas disponibles de previsiones de ausencia por horas
            intDisponible = roBusinessSupport.GetDisponiblePlannedAbsences(IDEmployee, IDCause, xBeginPeriod, xEndPeriod, _State)

            bolRet = True
        Catch ex As DbException
            _State.UpdateStateInfo(ex, "roBusinessSupport::PlannedCausesResumeQuery")
        Catch ex As Exception
            _State.UpdateStateInfo(ex, "roBusinessSupport::PlannedCausesResumeQuery")
        End Try

        Return bolRet

    End Function

    Public Shared Function PlannedOvertimesResumeQuery(ByVal IDEmployee As Integer, ByVal IDCause As Integer, ByVal xCurrentDate As DateTime, ByRef xBeginPeriod As DateTime, ByRef xEndPeriod As DateTime,
                                             ByVal xOvertimeDate As DateTime, ByRef intPending As Double, ByRef intLasting As Double, ByRef intDisponible As Double,
                                             ByVal _State As roBusinessState, ByVal bolApplyWorkDaysOnConcept As Boolean, ByVal strConceptType As String) As Boolean
        Dim bolRet As Boolean = False

        Try

            ' Obtenemos el periodo de calculo
            ' Actualmente a nivel de reglas de solicitudes solo puede ser Anual / Por contrato
            Dim strDefaultQuery As String = roTypes.Any2String(ExecuteScalar("@SELECT# ISNULL(DefaultQuery , 'Y') FROM Concepts WHERE ID in(@SELECT# isnull(IDConceptBalance,0) from causes where id =" & IDCause.ToString & ")"))
            If strDefaultQuery.Length = 0 Then strDefaultQuery = "Y"

            ' Determinar inicio i final del periodo anual
            xBeginPeriod = roParameters.BeginYearPeriod(xCurrentDate)
            ' A la fecha final le añadimos un año por si permiten solicitar previsiones de un año para otro
            xEndPeriod = roParameters.EndYearPeriod(xCurrentDate).AddYears(1)

            Dim lstDates As New Generic.List(Of DateTime)
            Dim strSQL As String = "@SELECT# BeginDate, EndDate From EmployeeContracts WHERE IDEmployee = " & IDEmployee & " AND " &
                                       "BeginDate <= " & Any2Time(xOvertimeDate).SQLSmallDateTime & " AND EndDate >= " & Any2Time(xOvertimeDate).SQLSmallDateTime
            Dim oSqlCommand As DbCommand = CreateCommand(strSQL)
            Dim rd As DbDataReader = oSqlCommand.ExecuteReader()
            If rd.Read() Then
                If strDefaultQuery = "C" Then
                    ' En el caso de saldos por contrato
                    ' la fecha de inicio del periodo es el inicio de contrato
                    ' y la fecha de final del periodo es la misma del final del contrato
                    xBeginPeriod = rd("BeginDate")
                    xEndPeriod = rd("EndDate")
                End If
                lstDates.Add(rd("BeginDate"))
                lstDates.Add(rd("EndDate"))
            End If
            rd.Close()

            If lstDates.Count > 0 And strDefaultQuery = "Y" Then
                If xBeginPeriod < lstDates(0) Then xBeginPeriod = lstDates(0)
            End If

            ' Obtener número de días/horas solicitados pendientes de procesar
            intPending = roBusinessSupport.GetPendingApprovalPlannedOvertimes(IDEmployee, IDCause, xBeginPeriod, xEndPeriod, _State, bolApplyWorkDaysOnConcept, strConceptType)

            ' Obtener el número de días/horas aprobados pendientes de disfrutar
            intLasting = roBusinessSupport.GetApprovedButNotTakenPlannedOvertimes(IDEmployee, IDCause, xCurrentDate, xEndPeriod, _State, bolApplyWorkDaysOnConcept, strConceptType)

            ' Obtener los días/horas disponibles de previsiones de ausencia por horas
            intDisponible = roBusinessSupport.GetDisponiblePlannedAbsences(IDEmployee, IDCause, xBeginPeriod, xEndPeriod, _State)

            bolRet = True
        Catch ex As DbException
            _State.UpdateStateInfo(ex, "roBusinessSupport::PlannedOvertimesResumeQuery")
        Catch ex As Exception
            _State.UpdateStateInfo(ex, "roBusinessSupport::PlannedOvertimesResumeQuery")
        End Try

        Return bolRet

    End Function

    ''' <summary>
    ''' Cálculo de los días de vacaciones disponibles para un empleado. Sólo control sobre saldos de vacaciones anuales
    ''' </summary>
    ''' <param name="IDEmployee">ID de empleado a consultar</param>
    ''' <param name="IDShift">Horario de vacaciones</param>
    ''' <param name="xReferenceDate">Fecha actual a consultar</param>
    ''' <param name="xBeginPeriod">Devuelve fecha inicial del periodo usada para los cálculos</param>
    ''' <param name="xEndPeriod">Devuelve fecha final del periodo usada para los cálculos</param>
    ''' <param name="xVacationsDate">Día solicitado. Si se solicitó un periodo, día inicial del periodo solicitado</param>
    ''' <param name="intDone"></param>
    ''' <param name="intPending"></param>
    ''' <param name="intLasting"></param>
    ''' <param name="intDisponible"></param>
    ''' <param name="_State"></param>
    ''' <param name="IDContract">Id de contrato del empleado (ses usa en informes ...)</param>
    ''' <returns></returns>
    Public Shared Function VacationsResumeQuery(ByVal IDEmployee As Integer, ByVal IDShift As Integer, ByVal xReferenceDate As DateTime, ByRef xBeginPeriod As DateTime, ByRef xEndPeriod As DateTime,
                                             ByVal xVacationsDate As DateTime, ByRef intDone As Double, ByRef intPending As Double, ByRef intLasting As Double, ByRef intDisponible As Double,
                                             ByVal _State As roBusinessState, ByRef intExpiredDays As Double, ByRef intDaysWithoutEnjoyment As Double, Optional ByVal IDContract As String = "", Optional ByVal lstRequestDates As List(Of Date) = Nothing) As Boolean
        Dim bolRet As Boolean = False

        Dim IDConceptRequestNextYear As Integer = 0
        Dim intPendingFuture As Double = 0
        Dim intLastingFuture As Double = 0

        Try
            ' Obtenemos el periodo de calculo
            ' Actualmente a nivel de reglas de solicitudes solo puede ser Anual / Por contrato / Mensual
            ' TODO: Cierto, pero para el saldo del año en curso. Para el de control del año siguiente, sólo muestra el anual !!!. Y se pueden mezclar los dos tipos.
            '       Y en el caso de vacaciones a futuro, los periodos que se calculan son anuales (BeginYearPeriod, EndYearPeriod ...
            Dim strDefaultQuery As String = roTypes.Any2String(ExecuteScalar("@SELECT# ISNULL(DefaultQuery , 'Y') FROM Concepts WHERE ID in(@SELECT# isnull(IDConceptBalance,0) from Shifts where id =" & IDShift.ToString & ")"))
            If strDefaultQuery.Length = 0 Then strDefaultQuery = "Y"

            If strDefaultQuery = "L" Then
                'En este caso la fecha de referencia siempre tiene que ser el dia de hoy en todos los contextos
                ' ya que se debe tener en cuenta todo lo ya planificado a futuro y el saldo actual a fecha de hoy
                xReferenceDate = Now.Date
            End If

            ' Determinar inicio del periodo anual
            xBeginPeriod = roParameters.BeginYearPeriod(xReferenceDate)

            ' Determinar final del periodo anual
            ' A la fecha final le añadimos un año por si permiten solicitar previsiones de un año para otro
            If xEndPeriod.Year = Now().Year OrElse roTypes.Any2Time(xEndPeriod).DateOnly = roTypes.Any2Time("0001/01/01").DateOnly Then
                xEndPeriod = roParameters.EndYearPeriod(xReferenceDate).AddYears(1)
            End If

            If strDefaultQuery = "M" Then
                ' En el caso de saldos mensuales
                Dim oParams As New roParameters("OPTIONS", True)
                Dim intMonthIniDay As Integer = oParams.Parameter(Parameters.MonthPeriod)
                If intMonthIniDay = 0 Then intMonthIniDay = 1

                If xReferenceDate.Day > intMonthIniDay Then
                    'Si el dia es posterior al inicio del periodo (mismo mes)
                    xBeginPeriod = New Date(xReferenceDate.Year, xReferenceDate.Month, intMonthIniDay)
                ElseIf xReferenceDate.Day < intMonthIniDay Then
                    'Si el dia es anterior al inicio del periodo (mes anterior)
                    xBeginPeriod = New Date(xReferenceDate.AddMonths(-1).Year, xReferenceDate.AddMonths(-1).Month, intMonthIniDay)
                Else
                    'Si es el mismo dia
                    xBeginPeriod = xReferenceDate
                End If
            End If

            Dim lstDates As New Generic.List(Of DateTime)
            Dim strSQL As String = "@SELECT# BeginDate, EndDate From EmployeeContracts WHERE IDEmployee = " & IDEmployee & " AND " &
                                           "BeginDate <= " & Any2Time(xVacationsDate).SQLSmallDateTime & " AND EndDate >= " & Any2Time(xVacationsDate).SQLSmallDateTime
            Dim oSqlCommand As DbCommand = CreateCommand(strSQL)
            Dim rd As DbDataReader = oSqlCommand.ExecuteReader()
            If rd.Read() Then
                If strDefaultQuery = "C" OrElse strDefaultQuery = "L" Then
                    ' En el caso de saldos por contrato
                    ' la fecha de inicio del periodo es el inicio de contrato
                    ' y la fecha de final del periodo es la misma del final del contrato
                    xBeginPeriod = rd("BeginDate")
                    xEndPeriod = rd("EndDate")
                End If
                lstDates.Add(rd("BeginDate"))
                lstDates.Add(rd("EndDate"))
            End If
            rd.Close()

            ' Obtenemos si el horario trabaja con dias naturales o laborables
            Dim bolAreWorkingDays As Boolean = Any2Boolean(ExecuteScalar("@SELECT# ISNULL(AreWorkingDays , 0) FROM Shifts WHERE ID=" & IDShift))

            ' *** FUNCIONALIDAD SALDOS A FUTURO
            ' Obtenemos si el horario utiliza otro saldo para fechas futuras
            Dim bolApplyActualEndPeriod As Boolean = False
            IDConceptRequestNextYear = roTypes.Any2Integer(ExecuteScalar("@SELECT# ISNULL(IDConceptRequestNextYear , 0) FROM Shifts where id =" & IDShift.ToString))
            If IDConceptRequestNextYear > 0 Then
                ' Debemos revisar si la solicitud es del año siguiente o no
                Dim xBeginPeriodActualDay As DateTime = roParameters.BeginYearPeriod(Now.Date)
                Dim xBeginVacationPeriod As DateTime = roParameters.BeginYearPeriod(xVacationsDate)
                If xBeginVacationPeriod > xBeginPeriodActualDay Then
                    ' la solicitud es del año siguiente

                    ' hay que obtener el número de días solicitados pendientes de procesar del horario solicitado del periodo del año siguiente
                    ' y el número de días aprobados pendientes de disfrutar del horario solicitado del periodo del año siguiente
                    Dim xEndVacationPeriod As DateTime = roParameters.EndYearPeriod(xVacationsDate)
                    SetPeriodWithContracts(lstDates, xBeginVacationPeriod, xEndVacationPeriod, strDefaultQuery)
                    intPendingFuture = roBusinessSupport.GetPendingApprovalHollidays(IDEmployee, IDShift, xBeginVacationPeriod, Nothing, bolAreWorkingDays, _State)
                    intLastingFuture = roBusinessSupport.GetApprovedButNotTakenHolidays(IDEmployee, IDShift, xBeginVacationPeriod.AddDays(-1), xEndVacationPeriod, _State)

                    ' y debemos utilizar como saldo actual el IDConceptRequestNextYear en todos los calculos posteriores
                    IDShift = roTypes.Any2Integer(ExecuteScalar("@SELECT# ID FROM Shifts WHERE IDConceptBalance =" & IDConceptRequestNextYear.ToString & " AND ShiftType = 2 "))
                Else
                    ' la solicitud NO es del año siguiente
                    ' utilizamos el sado definido de forma habitual en todos los calculos posteriores
                End If

                ' se debe modificar la fecha de fin del periodo hasta finales del año actual
                bolApplyActualEndPeriod = True
            Else
                ' Verificamos si el saldo actual del horario se utiliza en algun otro horario como saldo actual en caso de solicitudes futuras
                Dim IDShift_IDConceptRequestNextYear As Integer = roTypes.Any2Integer(ExecuteScalar("@SELECT# TOP 1 isnull(ID, 0) FROM SHIFTS WHERE ISNULL(IDConceptRequestNextYear,0) > 0 and ISNULL(IDConceptRequestNextYear,0) in ( @SELECT# isnull(IDConceptBalance,0) FROM SHIFTS where ID=" & IDShift.ToString & " AND isnull(IDConceptBalance,0) > 0 )"))
                If IDShift_IDConceptRequestNextYear > 0 Then

                    ' En caso afirmativo,
                    ' se deben tener en cuenta las solicitudes pendientes y aprobadas del año siguiente del horario que lo utiliza para las solicitudes futuras
                    Dim xBeginPeriodNextYear As DateTime = roParameters.BeginYearPeriod(Now.Date)
                    xBeginPeriodNextYear = xBeginPeriodNextYear.AddYears(1)
                    Dim xEndPeriodNextYear As DateTime = roParameters.EndYearPeriod(xBeginPeriodNextYear)
                    SetPeriodWithContracts(lstDates, xBeginPeriodNextYear, xEndPeriodNextYear, strDefaultQuery)
                    ' TODO: En caso de tener finalización de contrato en el año actual, este cálculo para año siguiente da un periodo incorrecto tras cortar con contrato (EndPeriodNextYear pasa a valer el fin de contrato, pero begin year es inicio de siguiente periodo, luego Begin > End !!
                    intPendingFuture = roBusinessSupport.GetPendingApprovalHollidays(IDEmployee, IDShift_IDConceptRequestNextYear, xBeginPeriodNextYear, Nothing, bolAreWorkingDays, _State)
                    intLastingFuture = roBusinessSupport.GetApprovedButNotTakenHolidays(IDEmployee, IDShift_IDConceptRequestNextYear, xBeginPeriodNextYear.AddDays(-1), xEndPeriodNextYear, _State)

                    ' se utiliza el saldo definido de forma habitual
                    ' y se debe modifica la fecha de fin del periodo hasta finales del año actual
                    bolApplyActualEndPeriod = True
                End If
            End If

            If bolApplyActualEndPeriod Then
                ' en caso de ser un horario que se utiliza en las vacaciones a futuro
                ' se modifica la fecha de fin del periodo hasta finales de año actual en todos los calculos posteriores
                xEndPeriod = roParameters.EndYearPeriod(xReferenceDate)
            End If
            ' ******

            ' Ajustamos el periodo de calculo al contrato del empleado
            SetPeriodWithContracts(lstDates, xBeginPeriod, xEndPeriod, strDefaultQuery)

            ' Obtener número días ya disfrutados
            intDone = roBusinessSupport.GetAlreadyTakenHollidays(IDEmployee, IDShift, xReferenceDate, xBeginPeriod, _State)

            Dim lstHolidaysPendingDays As List(Of roHolidaysDay) = Nothing
            If strDefaultQuery = "L" Then
                ' Si el saldo es por contrato con valores iniciales, debemos guardarnos una lista de todas las fechas pendientes de consolidar
                ' para luego aplicarlas al tramo que le corresponda y las fechas de caducidad de cada tramo
                lstHolidaysPendingDays = New List(Of roHolidaysDay)
            End If

            ' Obtener número de días solicitados pendientes de procesar
            intPending = roBusinessSupport.GetPendingApprovalHollidays(IDEmployee, IDShift, Nothing, IIf(bolApplyActualEndPeriod, xEndPeriod, Nothing), bolAreWorkingDays, _State, lstHolidaysPendingDays)
            intPending += intPendingFuture

            ' Obtener el número de días aprobados pendientes de disfrutar
            intLasting = roBusinessSupport.GetApprovedButNotTakenHolidays(IDEmployee, IDShift, xReferenceDate, xEndPeriod, _State, lstHolidaysPendingDays)
            intLasting += intLastingFuture

            ' Obtener los días disponibles de vacaciones
            Dim lstHolidaysSummaryByPeriod As New Generic.List(Of roHolidaysSummaryByPeriod)
            Dim intIDConcept As Integer = 0
            intDisponible = roBusinessSupport.GetAvailableHolidays(IDEmployee, IDShift, xBeginPeriod, xEndPeriod, _State, strDefaultQuery, IDContract, lstHolidaysSummaryByPeriod, intIDConcept)

            If strDefaultQuery = "L" Then
                ' En este caso debemos aplicar en caso necesario las fechas de caducidad y las fechas de disfrute sobre 
                ' el saldo disponible de cada tramo y asignando las vacaciones pendientes a cada uno de los tramos
                If lstHolidaysPendingDays Is Nothing Then lstHolidaysPendingDays = New List(Of roHolidaysDay)

                ' Añadimos las fechas solicitadas de la solicitud que aun no está guardada, en caso necesario
                If lstRequestDates IsNot Nothing Then
                    For Each dtRequestDate As Date In lstRequestDates
                        Dim oHolidaysDay As New roHolidaysDay
                        oHolidaysDay.HolidayDate = dtRequestDate
                        oHolidaysDay.IsExpiredDate = 0
                        lstHolidaysPendingDays.Add(oHolidaysDay)
                        intPending += 1
                    Next
                End If

                lstHolidaysPendingDays = lstHolidaysPendingDays.OrderBy(Function(h) h.HolidayDate).ToList()

                ' Tratamiento del inicio de disfrute de cada tramo
                ' Eliminamos los valores disponibles de los tramos que aun no se pueden disfrutar, en caso necesario
                intDaysWithoutEnjoyment = 0
                If lstHolidaysSummaryByPeriod.Count > 0 AndAlso lstHolidaysSummaryByPeriod.Any(Function(x) x.StartEnjoymentDate IsNot Nothing) Then

                    ' Inicialmente tenemos en cuenta como fecha de referencia la fecha mayor de todas las pendientes incluyendo tambien el dia de hoy
                    Dim xStartEnjoynmentDateRef As Date = Now.Date
                    If lstHolidaysPendingDays IsNot Nothing AndAlso lstHolidaysPendingDays.Count > 0 Then
                        xStartEnjoynmentDateRef = New DateTime(Math.Max(Now.Date.Ticks, lstHolidaysPendingDays.Last.HolidayDate.Ticks))
                    End If

                    ' Eliminamos los valores disponibles de los tramos que aun no se pueden disfrutar
                    For Each holidaySummary As roHolidaysSummaryByPeriod In lstHolidaysSummaryByPeriod
                        If holidaySummary.StartEnjoymentDate.HasValue AndAlso holidaySummary.StartEnjoymentDate > xStartEnjoynmentDateRef Then
                            ' Si la fecha de inicio de disfrute es mayor que la fecha de referencia, eliminamos el valor disponible
                            holidaySummary.DaysWithoutEnjoyment = holidaySummary.ExpectedValue
                            holidaySummary.ExpectedValue = 0
                        End If
                    Next

                    ' Trabajamos con una copia de los valores disponibles del saldo
                    Dim lstHolidaysSummaryByPeriodCopy As New Generic.List(Of roHolidaysSummaryByPeriod)
                    For Each oHolidaysSummaryByPeriod As roHolidaysSummaryByPeriod In lstHolidaysSummaryByPeriod
                        Dim oHolidaysSummaryByPeriodCopy As New roHolidaysSummaryByPeriod
                        oHolidaysSummaryByPeriodCopy.BeginPeriod = oHolidaysSummaryByPeriod.BeginPeriod
                        oHolidaysSummaryByPeriodCopy.EndPeriod = oHolidaysSummaryByPeriod.EndPeriod
                        oHolidaysSummaryByPeriodCopy.ExpectedValue = oHolidaysSummaryByPeriod.ExpectedValue
                        oHolidaysSummaryByPeriodCopy.DaysWithoutEnjoyment = oHolidaysSummaryByPeriod.DaysWithoutEnjoyment
                        oHolidaysSummaryByPeriodCopy.StartEnjoymentDate = oHolidaysSummaryByPeriod.StartEnjoymentDate
                        lstHolidaysSummaryByPeriodCopy.Add(oHolidaysSummaryByPeriodCopy)
                    Next

                    ' Verificamos que cada uno de los dias pendientes de solicitar o planificados se puedan asignar a un tramo
                    ' en funcion a la fecha de inicio de disfrute
                    ' si no es así, se marcan todos los dias disponibles como dias que no se pueden disfrutar del tramo correspondiente
                    For Each oHolidaysDay As roHolidaysDay In lstHolidaysPendingDays
                        For Each oHolidaysSummaryByPeriod As roHolidaysSummaryByPeriod In lstHolidaysSummaryByPeriodCopy
                            If oHolidaysSummaryByPeriod.ExpectedValue > 0 Then
                                If oHolidaysSummaryByPeriod.StartEnjoymentDate.HasValue AndAlso oHolidaysSummaryByPeriod.StartEnjoymentDate > oHolidaysDay.HolidayDate Then
                                    ' Si el tramo tiene una fecha de disfrute superior a la fecha de solicitud, los dias de ese tramo no se pueden disfrutar
                                    oHolidaysSummaryByPeriod.DaysWithoutEnjoyment = oHolidaysSummaryByPeriod.ExpectedValue
                                    oHolidaysSummaryByPeriod.ExpectedValue = 0
                                Else
                                    ' Restamos el valor del tramo actual y seguimos con el siguiente
                                    ' esto es necesario para saber si un tramo ya se queda a 0 y no se tiene que que tener en cuenta para la siguiente fecha de vacaciones
                                    ' si no restasemos solo se revisaria siempre contra el primer tramo con valor positivo y que tuviera fecha de inicio de disfrute
                                    oHolidaysSummaryByPeriod.ExpectedValue -= 1
                                    Exit For
                                End If
                            End If

                            ' Si el dia de vacaciones corresponde al tramo actual , ya no tenemos que revisar mas
                            If oHolidaysDay.HolidayDate >= oHolidaysSummaryByPeriod.BeginPeriod AndAlso oHolidaysDay.HolidayDate <= oHolidaysSummaryByPeriod.EndPeriod Then
                                Exit For
                            End If
                        Next
                    Next

                    ' Asignamos el nuevo valor de disfrute a los tramos originales del saldo disponible
                    For Each oHolidaysSummaryByPeriod As roHolidaysSummaryByPeriod In lstHolidaysSummaryByPeriod
                        Dim oHolidaysSummaryByPeriodCopy As roHolidaysSummaryByPeriod = lstHolidaysSummaryByPeriodCopy.FirstOrDefault(Function(x) x.BeginPeriod = oHolidaysSummaryByPeriod.BeginPeriod AndAlso x.EndPeriod = oHolidaysSummaryByPeriod.EndPeriod)
                        If oHolidaysSummaryByPeriodCopy IsNot Nothing Then
                            oHolidaysSummaryByPeriod.DaysWithoutEnjoyment = oHolidaysSummaryByPeriodCopy.DaysWithoutEnjoyment
                        End If
                    Next

                    intDaysWithoutEnjoyment = lstHolidaysSummaryByPeriod.Sum(Function(p) p.DaysWithoutEnjoyment)
                End If

                ' Tratamiento de caducidades
                ' Añadimos a la lista de fechas pendientes de vacaciones, las fechas de caducidad que haya desde mañana hasta la ultima fecha de vacaciones pendiente (solicitudes pendientes de aprobar y vacaciones planificadas)
                Dim tbExpiredDates As DataTable = Nothing
                If lstHolidaysPendingDays IsNot Nothing AndAlso lstHolidaysPendingDays.Count > 0 Then
                    strSQL = "@SELECT# ExpiredDate, Value, BeginPeriod, EndPeriod FROM DailyAccruals WITH (NOLOCK) WHERE IDEmployee= " & IDEmployee.ToString & " AND IDConcept=" & intIDConcept.ToString &
                            " AND ExpiredDate >=" & Any2Time(Now.Date.AddDays(1)).SQLSmallDateTime & " AND ExpiredDate <= " & Any2Time(lstHolidaysPendingDays.Last.HolidayDate).SQLSmallDateTime

                    tbExpiredDates = CreateDataTable(strSQL)
                    If tbExpiredDates IsNot Nothing AndAlso tbExpiredDates.Rows.Count > 0 Then
                        For Each oRow As DataRow In tbExpiredDates.Rows
                            Dim oHolidaysDay As New roHolidaysDay
                            oHolidaysDay.HolidayDate = oRow("ExpiredDate")
                            oHolidaysDay.IsExpiredDate = 1
                            oHolidaysDay.BeginPeriod = oRow("BeginPeriod")
                            oHolidaysDay.EndPeriod = oRow("EndPeriod")
                            lstHolidaysPendingDays.Add(oHolidaysDay)
                        Next
                    End If
                End If

                ' Ordenamos las fechas (primero se aplican los dias de vacaciones , luego las caducidades (si el mismo dia existen ambos casos)
                lstHolidaysPendingDays = lstHolidaysPendingDays.OrderBy(Function(h) h.HolidayDate).ThenBy(Function(h) h.IsExpiredDate).ToList()

                intExpiredDays = 0
                ' Solo se realiza el tratamiento de las CADUCIDADES en el caso que existan caducidades a futuro
                ' sino no es necesario
                If tbExpiredDates IsNot Nothing AndAlso tbExpiredDates.Rows.Count > 0 Then
                    ' Revisamos cada una de las fechas de vacaciones pendientes y las caducidades 
                    ' y sobre los tramos de valores disponibles, aplicamos las vacaciones pendientes y las caducidades
                    ' si es un dia de vacaciones lo restamos del tramo correspondiente
                    ' si es una fecha de caducidad , dejamos el tramo correspondiente a 0 e indicamos cuantas van a caducar en ese momento
                    If lstHolidaysSummaryByPeriod IsNot Nothing AndAlso lstHolidaysSummaryByPeriod.Count > 0 Then
                        For Each oHolidaysDay As roHolidaysDay In lstHolidaysPendingDays
                            If oHolidaysDay.IsExpiredDate = 0 Then
                                ' Si el dia es de vacaciones
                                Dim bolApplyonPeriod As Boolean = False
                                For Each oHolidaysSummaryByPeriod As roHolidaysSummaryByPeriod In lstHolidaysSummaryByPeriod
                                    ' Si el dia de vacaciones corresponde al tramo actual , ya no tenemos que revisar mas
                                    If oHolidaysDay.HolidayDate >= oHolidaysSummaryByPeriod.BeginPeriod AndAlso oHolidaysDay.HolidayDate <= oHolidaysSummaryByPeriod.EndPeriod Then
                                        bolApplyonPeriod = True
                                    Else
                                        ' Obtenemos el valor del saldo en el tramo correspondiente hasta taskday, si el valor es positivo lo asignamos a dicho tramo
                                        ' se tiene el cuenta hasta la fecha de calculo porque el mismo dia puede haberse generado un valor inicial
                                        If oHolidaysSummaryByPeriod.ExpectedValue > 0 Then bolApplyonPeriod = True
                                    End If
                                    If bolApplyonPeriod Then
                                        ' En el caso que se ha asignado al tramo actual, 
                                        ' debemos restar un dia al saldo actual del tramo
                                        oHolidaysSummaryByPeriod.ExpectedValue -= 1
                                        Exit For
                                    End If
                                Next

                            Else
                                ' si el dia es de caducidad, debemos dejar el saldo actual a 0 del tramo correspondiente
                                ' y pasar los dias que quedaban a caducados

                                ' solo lo caducamos si la fecha no corresponde con el ultimo dia solicitado
                                ' ya que en ese caso prevalece en ese caso sola la fecha de vacaciones
                                If lstHolidaysPendingDays.Last.HolidayDate <> oHolidaysDay.HolidayDate Then

                                    For Each oHolidaysSummaryByPeriod As roHolidaysSummaryByPeriod In lstHolidaysSummaryByPeriod
                                        ' Si la fecha de caducidad corresponde al tramo actual , caducamos el valor actual del saldo

                                        If oHolidaysDay.BeginPeriod = oHolidaysSummaryByPeriod.BeginPeriod AndAlso oHolidaysDay.EndPeriod = oHolidaysSummaryByPeriod.EndPeriod AndAlso oHolidaysSummaryByPeriod.ExpectedValue > 0 Then
                                            oHolidaysSummaryByPeriod.ExpiredDays = oHolidaysSummaryByPeriod.ExpectedValue
                                            oHolidaysSummaryByPeriod.ExpectedValue = 0
                                            Exit For
                                        End If
                                    Next
                                End If
                            End If
                        Next

                        intExpiredDays = lstHolidaysSummaryByPeriod.Sum(Function(p) p.ExpiredDays)
                    End If
                End If
            End If

            bolRet = True

        Catch ex As DbException
            _State.UpdateStateInfo(ex, "roBusinessSupport::VacationsResumeQuery")
        Catch ex As Exception
            _State.UpdateStateInfo(ex, "roBusinessSupport::VacationsResumeQuery")
        End Try

        Return bolRet

    End Function

    Public Shared Sub SetPeriodWithContracts(ByVal lstDates As Generic.List(Of DateTime), ByRef xBeginPeriod As DateTime, ByRef xEndPeriod As DateTime, ByVal strDefaultQuery As String)
        Try
            If lstDates.Count > 0 AndAlso (strDefaultQuery = "Y" OrElse strDefaultQuery = "M") Then
                If xBeginPeriod < lstDates(0) Then xBeginPeriod = lstDates(0)
            End If

            If lstDates.Count > 1 Then
                If xEndPeriod > lstDates(1) AndAlso lstDates(1) < Date.Now() Then
                    xEndPeriod = lstDates(1)
                End If
            End If
        Catch ex As Exception

        End Try
    End Sub

    Public Shared Function GetPeriodDatesByContractAtDate(type As SummaryType, ByVal IDEmployee As Integer, ByVal InDate As DateTime, ByVal oState As roBusinessState, Optional bRaw As Boolean = False) As Generic.List(Of DateTime)

        Dim retDates As New Generic.List(Of DateTime)

        Try
            Dim oParams As New roParameters("OPTIONS", True)

            Dim intMonthIniDay As Integer = oParams.Parameter(Parameters.MonthPeriod)
            Dim intYearIniMonth As Integer = oParams.Parameter(Parameters.YearPeriod)
            Dim intWeekIniDay As Integer = oParams.Parameter(Parameters.WeekPeriod)
            If intMonthIniDay = 0 Then intMonthIniDay = 1
            If intYearIniMonth = 0 Then intYearIniMonth = 1
            Dim iDayOfWeek As Integer = InDate.DayOfWeek
            If iDayOfWeek = 0 Then iDayOfWeek = 7
            If intWeekIniDay > iDayOfWeek Then intWeekIniDay = intWeekIniDay - 7

            Dim theoricDates As List(Of DateTime) = New List(Of DateTime)

            ' Los contratos se deben calcular en base a una fecha del periodo resultante.
            ' Si solo miramos a día de hoy, no se caluclará ningún periodo pasado a día de hoy el empleado ya no tiene contrato
            Dim contractDates As List(Of Date) = New List(Of Date)

            ' Calculamos periodo
            Select Case type
                Case SummaryType.Daily, SummaryType.ChoosePeriod
                    theoricDates.Add(InDate)
                    theoricDates.Add(InDate)
                Case SummaryType.Semanal
                    Dim xbeginperiod As DateTime = InDate.AddDays(intWeekIniDay - iDayOfWeek)
                    Dim xendperiod As DateTime = xbeginperiod.AddDays(6)
                    theoricDates.Add(xbeginperiod)
                    theoricDates.Add(xendperiod)
                Case SummaryType.Mensual
                    Dim xBeginPeriod As DateTime
                    If InDate.Day > intMonthIniDay Then
                        'Si el dia es posterior al inicio del periodo (mismo mes)
                        xBeginPeriod = New Date(InDate.Year, InDate.Month, intMonthIniDay)
                    ElseIf InDate.Day < intMonthIniDay Then
                        'Si el dia es anterior al inicio del periodo (mes anterior)
                        xBeginPeriod = New Date(InDate.AddMonths(-1).Year, InDate.AddMonths(-1).Month, intMonthIniDay)
                    Else
                        'Si es el mismo dia
                        xBeginPeriod = InDate
                    End If
                    Dim xEndPeriod As DateTime = xBeginPeriod.AddMonths(1).AddDays(-1)
                    theoricDates.Add(xBeginPeriod)
                    theoricDates.Add(xEndPeriod)
                Case SummaryType.LastMonth
                    Dim xBeginPeriod As DateTime
                    If InDate.Day > intMonthIniDay Then
                        'Si el dia es posterior al inicio del periodo (mismo mes)
                        xBeginPeriod = New Date(InDate.Year, InDate.Month, intMonthIniDay)
                    ElseIf InDate.Day < intMonthIniDay Then
                        'Si el dia es anterior al inicio del periodo (mes anterior)
                        xBeginPeriod = New Date(InDate.AddMonths(-1).Year, InDate.AddMonths(-1).Month, intMonthIniDay)
                    Else
                        'Si es el mismo dia
                        xBeginPeriod = InDate
                    End If
                    xBeginPeriod = xBeginPeriod.AddMonths(-1)
                    Dim xEndPeriod As DateTime = xBeginPeriod.AddMonths(1).AddDays(-1)
                    theoricDates.Add(xBeginPeriod)
                    theoricDates.Add(xEndPeriod)
                Case SummaryType.Anual
                    Dim xBeginPeriod As DateTime
                    If InDate.Month > intYearIniMonth Then
                        xBeginPeriod = New DateTime(InDate.Year, intYearIniMonth, intMonthIniDay)
                    ElseIf InDate.Month = intYearIniMonth And InDate.Day >= intMonthIniDay Then
                        xBeginPeriod = New DateTime(InDate.Year, intYearIniMonth, intMonthIniDay)
                    Else
                        xBeginPeriod = New DateTime(InDate.Year - 1, intYearIniMonth, intMonthIniDay)
                    End If
                    Dim xEndPeriod As DateTime = xBeginPeriod.AddYears(1).AddDays(-1)
                    theoricDates.Add(xBeginPeriod)
                    theoricDates.Add(xEndPeriod)
                Case SummaryType.LastYear
                    Dim xBeginPeriod As DateTime
                    If InDate.Month > intYearIniMonth Then
                        xBeginPeriod = New DateTime(InDate.Year, intYearIniMonth, intMonthIniDay)
                    ElseIf InDate.Month = intYearIniMonth And InDate.Day >= intMonthIniDay Then
                        xBeginPeriod = New DateTime(InDate.Year, intYearIniMonth, intMonthIniDay)
                    Else
                        xBeginPeriod = New DateTime(InDate.Year - 1, intYearIniMonth, intMonthIniDay)
                    End If
                    xBeginPeriod = xBeginPeriod.AddYears(-1)
                    Dim xEndPeriod As DateTime = xBeginPeriod.AddYears(1).AddDays(-1)
                    theoricDates.Add(xBeginPeriod)
                    theoricDates.Add(xEndPeriod)
                Case SummaryType.NextContractAnnualizedPeriod
                    theoricDates = GetDatesOfAnnualWorkPeriodsInDate(IDEmployee, InDate, oState)
                    If theoricDates IsNot Nothing AndAlso theoricDates.Count > 1 Then
                        theoricDates = GetDatesOfAnnualWorkPeriodsInDate(IDEmployee, theoricDates(1).Date.AddDays(1), oState)
                        If theoricDates IsNot Nothing AndAlso theoricDates.Count > 1 Then
                            ' El tramo siguiente, para ser el siguiente, debe ser del mismo contrato que el que tengo hoy. Let's see ...
                            Dim currentContractDates As List(Of Date) = GetDatesOfContractInDate(IDEmployee, InDate, oState)
                            If Not (currentContractDates.Count > 0 AndAlso currentContractDates(1) >= theoricDates(0)) Then
                                theoricDates(0) = New DateTime(1900, 1, 1, 0, 0, 0)
                                theoricDates(1) = New DateTime(1900, 1, 1, 0, 0, 0)
                            End If
                        End If
                    End If
                Case SummaryType.Contrato, SummaryType.ContractAnnualized
                    theoricDates = GetDatesOfContractInDate(IDEmployee, InDate, oState)
            End Select

            ' Si encontré intervalo, lo limito al periodo de contrato que tuviese el empleado en ese intervalo.
            ' Si hay varios, nos qeudamos con el último que tenga días en el intevalo
            If theoricDates IsNot Nothing AndAlso theoricDates.Count > 1 Then
                contractDates = GetLastContractDatesInPeriod(IDEmployee, theoricDates(0), theoricDates(1), oState)
                theoricDates = GetPeriodsIntersection(contractDates(0), contractDates(1), theoricDates(0), theoricDates(1))
            End If

            If theoricDates IsNot Nothing AndAlso theoricDates.Count > 1 Then
                retDates.Add(theoricDates(0))

                If bRaw OrElse InDate < theoricDates(0) OrElse InDate > theoricDates(1) Then
                    ' Si InDate no pertenece al intervalo, o si quiero el intervalo sin ajustar, devuelvo el intervalo teórico
                    retDates.Add(theoricDates(1))
                Else
                    ' Si InDate pertenece al intervalo y no lo he pedido sin ajustar, devuelvo el intervalo ajustado
                    retDates.Add(InDate.Date)
                End If
            Else
                retDates.Add(New DateTime(1900, 1, 1, 0, 0, 0))
                retDates.Add(New DateTime(1900, 1, 1, 0, 0, 0))
            End If
        Catch ex As DbException
            oState.UpdateStateInfo(ex, "roBusinessSupport::GetDatesOfEmployeePeriodByContractInDate")
        Catch ex As Exception
            oState.UpdateStateInfo(ex, "roBusinessSupport::GetDatesOfEmployeePeriodByContractInDate")
        Finally

        End Try

        Return retDates

    End Function

    Public Shared Function GetPeriodsIntersection(xBeginPeriodA As Date, xEndPeriodA As Date, xBeginPeriodB As Date, xEndPeriodB As Date) As List(Of Date)
        Dim periodDates As List(Of DateTime) = New List(Of DateTime)

        ' Sólo si hay intersección ...
        If xBeginPeriodA <= xEndPeriodB AndAlso xEndPeriodA >= xBeginPeriodB Then
            ' ... calculo la intersección
            If xBeginPeriodA > xBeginPeriodB Then
                periodDates.Add(xBeginPeriodA)
            Else
                periodDates.Add(xBeginPeriodB)
            End If
            If xEndPeriodA > xEndPeriodB Then
                periodDates.Add(xEndPeriodB)
            Else
                periodDates.Add(xEndPeriodA)
            End If
        End If

        Return periodDates
    End Function

    ''' <summary>
    ''' Returns periods of A that are outside B
    ''' </summary>
    ''' <param name="xBeginPeriodA"></param>
    ''' <param name="xEndPeriodA"></param>
    ''' <param name="xBeginPeriodB"></param>
    ''' <param name="xEndPeriodB"></param>
    ''' <returns></returns>
    Public Shared Function GetPeriodsOutsideModifiedPeriod(xBeginPeriodA As Date, xEndPeriodA As Date, xBeginPeriodB As Date, xEndPeriodB As Date) As List(Of roDateTimePeriod)
        Dim periodDates As List(Of roDateTimePeriod) = New List(Of roDateTimePeriod)

        ' Sólo si hay intersección ...
        'dBeginNewContractDate > dBeginOldContractDate OrElse dEndNewContractDate < dEndOldContractDate
        If xBeginPeriodA <= xEndPeriodB AndAlso xEndPeriodA >= xBeginPeriodB Then
            ' ... calculo la intersección
            If xBeginPeriodA < xBeginPeriodB Then
                periodDates.Add(New roDateTimePeriod With {.TypePeriod = TypePeriodEnum.PeriodOther, .BeginDateTimePeriod = xBeginPeriodA, .EndDateTimePeriod = xBeginPeriodB.AddDays(-1)})
            End If
            If xEndPeriodA > xEndPeriodB Then
                periodDates.Add(New roDateTimePeriod With {.TypePeriod = TypePeriodEnum.PeriodOther, .BeginDateTimePeriod = xEndPeriodB.AddDays(1), .EndDateTimePeriod = xEndPeriodA})
            End If
        Else
            ' ... si no hay intersección, devuelvo el periodo A completo
            periodDates.Add(New roDateTimePeriod With {.TypePeriod = TypePeriodEnum.PeriodOther, .BeginDateTimePeriod = xBeginPeriodA, .EndDateTimePeriod = xEndPeriodA})
        End If

        Return periodDates
    End Function

    Public Shared Function GetDatesOfAnnualWorkPeriodsInDate(ByVal IDEmployee As Integer, ByVal InDate As DateTime, ByVal oState As roBusinessState) As Generic.List(Of DateTime)

        Dim lstDates As New Generic.List(Of DateTime)
        Dim rd As DbDataReader = Nothing
        Try

            Dim strSQL As String = "@SELECT# BeginPeriod as BeginDate, EndPeriod as EndDate From dbo.sysfnEmployeesAnnualWorkPeriods(" & IDEmployee.ToString & ") WHERE BeginPeriod <= " & Any2Time(InDate).SQLSmallDateTime & " AND EndPeriod >= " & Any2Time(InDate).SQLSmallDateTime

            Dim oSqlCommand As DbCommand = CreateCommand(strSQL)
            rd = oSqlCommand.ExecuteReader()
            If rd.Read() Then
                lstDates.Add(rd("BeginDate"))
                lstDates.Add(rd("EndDate"))
            End If

            rd.Close()
        Catch ex As DbException
            oState.UpdateStateInfo(ex, "roBusinessSupport::GetDatesOfAnnualWorkPeriodsInDate")
            If rd IsNot Nothing Then rd.Close()
        Catch ex As Exception
            oState.UpdateStateInfo(ex, "roBusinessSupport::GetDatesOfAnnualWorkPeriodsInDate")
        Finally

        End Try

        Return lstDates

    End Function

    Public Shared Function GetDatesOfContractInDate(ByVal IDEmployee As Integer, ByVal InDate As DateTime, ByVal oState As roBusinessState) As Generic.List(Of DateTime)

        Dim lstDates As New Generic.List(Of DateTime)

        Try

            Dim strSQL As String = "@SELECT# BeginDate, EndDate From EmployeeContracts WHERE IDEmployee = " & IDEmployee & " AND " &
                                       "BeginDate <= " & Any2Time(InDate.Date).SQLSmallDateTime & " AND EndDate >= " & Any2Time(InDate.Date).SQLSmallDateTime
            Dim oSqlCommand As DbCommand = CreateCommand(strSQL)
            Dim rd As DbDataReader = oSqlCommand.ExecuteReader()
            If rd.Read() Then
                lstDates.Add(rd("BeginDate"))
                lstDates.Add(rd("EndDate"))
            End If

            rd.Close()
        Catch ex As DbException
            oState.UpdateStateInfo(ex, "roBusinessSupport::GetDatesOfContractInDate")
        Catch ex As Exception
            oState.UpdateStateInfo(ex, "roBusinessSupport::GetDatesOfContractInDate")
        Finally

        End Try

        Return lstDates

    End Function

    Public Shared Function GetLastContractDatesInPeriod(ByVal IDEmployee As Integer, ByVal BeginPeriod As Date, ByVal EndPeriod As Date, ByVal oState As roBusinessState) As List(Of DateTime)

        Dim lstDates As New Generic.List(Of DateTime)

        Try

            Dim strSQL As String = "@SELECT# TOP 1 BeginDate, EndDate From EmployeeContracts WHERE IDEmployee = " & IDEmployee & " AND " &
                                   " BeginDate <= " & Any2Time(EndPeriod).SQLSmallDateTime & " And EndDate >= " & Any2Time(BeginPeriod).SQLSmallDateTime &
                                   " ORDER BY BeginDate DESC"

            Dim tb As DataTable = CreateDataTable(strSQL)

            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                lstDates.Add(tb.Rows(0)("BeginDate"))
                lstDates.Add(tb.Rows(0)("EndDate"))
            Else
                lstDates.Add(New DateTime(1900, 1, 1, 0, 0, 0))
                lstDates.Add(New DateTime(1900, 1, 1, 0, 0, 0))
            End If
        Catch ex As DbException
            oState.UpdateStateInfo(ex, "roBusinessSupport::GetLastContractDatesInPeriod")
        Catch ex As Exception
            oState.UpdateStateInfo(ex, "roBusinessSupport::GetLastContractDatesInPeriod")
        Finally

        End Try

        Return lstDates

    End Function

    ''' <summary>
    ''' Resumen de vacaciones/permisos por horas
    ''' </summary>
    ''' <param name="IDEmployee">ID de empleado a consultar</param>
    ''' <param name="xCurrentDate">Fecha actual a consultar</param>
    ''' <param name="xBeginPeriod">Devuelve fecha inicial del periodo</param>
    ''' <param name="xEndPeriod">Devuelve fecha final del periodo</param>
    ''' <param name="intPending">Devuelve número de días solicitados pendientes de procesa</param>
    ''' <param name="intLasting">Devuelve el número de días aprobados pendientes de disfrutar</param>
    ''' <param name="intDisponible">Devuelve los días disponibles de vacaciones</param>
    ''' <param name="_State"></param>
    ''' <param name="oActiveConnection">Conexion activa (opcional)</param>
    ''' <returns>Devuelve TRUE si se realiza la función correctamente</returns>
    ''' <remarks></remarks>
    Public Shared Function ProgrammedHolidaysResumeQuery(ByVal IDEmployee As Integer, ByVal IDCause As Integer, ByVal xCurrentDate As DateTime, ByRef xBeginPeriod As DateTime, ByRef xEndPeriod As DateTime,
                                         ByVal xVacationsDate As DateTime, ByRef intPending As Double, ByRef intLasting As Double, ByRef intDisponible As Double,
                                         ByVal _State As roBusinessState, Optional ByRef intMaxValue As Double = 0) As Boolean
        Dim bolRet As Boolean = False

        Try

            ' Obtenemos el periodo de calculo
            ' Actualmente a nivel de reglas de solicitudes solo puede ser Anual / Por contrato / Mensual
            Dim strDefaultQuery As String = roTypes.Any2String(ExecuteScalar("@SELECT# ISNULL(DefaultQuery , 'Y') FROM Concepts WHERE ID in(@SELECT# isnull(IDConceptBalance,0) from causes where id =" & IDCause.ToString & ")"))
            If strDefaultQuery.Length = 0 Then strDefaultQuery = "Y"

            ' Determinar inicio i final del periodo anual
            xBeginPeriod = roParameters.BeginYearPeriod(xCurrentDate)

            ' A la fecha final le añadimos un año por si permiten solicitar previsiones de un año para otro
            If xEndPeriod.Year = Now().Year OrElse roTypes.Any2Time(xEndPeriod).DateOnly = roTypes.Any2Time("0001/01/01").DateOnly Then
                xEndPeriod = roParameters.EndYearPeriod(xCurrentDate).AddYears(1)
            End If


            If strDefaultQuery = "M" Then
                ' En el caso de saldos mensuales
                Dim oParams As New roParameters("OPTIONS", True)
                Dim intMonthIniDay As Integer = oParams.Parameter(Parameters.MonthPeriod)
                If intMonthIniDay = 0 Then intMonthIniDay = 1

                If xCurrentDate.Day > intMonthIniDay Then
                    'Si el dia es posterior al inicio del periodo (mismo mes)
                    xBeginPeriod = New Date(xCurrentDate.Year, xCurrentDate.Month, intMonthIniDay)
                ElseIf xCurrentDate.Day < intMonthIniDay Then
                    'Si el dia es anterior al inicio del periodo (mes anterior)
                    xBeginPeriod = New Date(xCurrentDate.AddMonths(-1).Year, xCurrentDate.AddMonths(-1).Month, intMonthIniDay)
                Else
                    'Si es el mismo dia
                    xBeginPeriod = xCurrentDate
                End If
            End If

            Dim lstDates As New Generic.List(Of DateTime)
            Dim strSQL As String = "@SELECT# BeginDate, EndDate From EmployeeContracts WHERE IDEmployee = " & IDEmployee & " AND " &
                                       "BeginDate <= " & Any2Time(xVacationsDate).SQLSmallDateTime & " AND EndDate >= " & Any2Time(xVacationsDate).SQLSmallDateTime
            Dim oSqlCommand As DbCommand = CreateCommand(strSQL)
            Dim rd As DbDataReader = oSqlCommand.ExecuteReader()
            If rd.Read() Then
                If strDefaultQuery = "C" Then
                    ' En el caso de saldos por contrato
                    ' la fecha de inicio del periodo es el inicio de contrato
                    ' y la fecha de final del periodo es la misma del final del contrato
                    xBeginPeriod = rd("BeginDate")
                    xEndPeriod = rd("EndDate")
                End If
                lstDates.Add(rd("BeginDate"))
                lstDates.Add(rd("EndDate"))
            End If
            rd.Close()

            If lstDates.Count > 0 And (strDefaultQuery = "Y" Or strDefaultQuery = "M") Then
                If xBeginPeriod < lstDates(0) Then xBeginPeriod = lstDates(0)
            End If

            ' Obtener número de horas solicitados pendientes de procesar
            intPending = roBusinessSupport.GetPendingApprovalProgrammedHolidays(IDEmployee, IDCause, xBeginPeriod, xEndPeriod, _State)

            ' Obtener el número de horas aprobadas pendientes de disfrutar
            intLasting = roBusinessSupport.GetApprovedButNotTakenProgrammedHolidays(IDEmployee, IDCause, xCurrentDate, xEndPeriod, _State)

            ' Obtener las horas disponibles de vacaciones
            intDisponible = roBusinessSupport.GetDisponibleProgrammedHolidays(IDEmployee, IDCause, xBeginPeriod, xEndPeriod, _State)

            ' Añadimos la suma de los valores positivos del saldo
            strSQL = "@SELECT# isnull(SUM(VALUE),0) FROM  DailyAccruals   INNER JOIN Causes ON DailyAccruals.IDConcept = Causes.[IDConceptBalance] WHERE Causes.ID = " & IDCause & " AND  IDEmployee=" & IDEmployee
            strSQL &= " AND DailyAccruals.Date >= " & Any2Time(xBeginPeriod).SQLSmallDateTime & " AND DailyAccruals.Date <= " & Any2Time(xEndPeriod).SQLSmallDateTime
            strSQL &= " AND DailyAccruals.Value > 0.0"

            intMaxValue = Any2Double(ExecuteScalar(strSQL))

            bolRet = True
        Catch ex As DbException
            _State.UpdateStateInfo(ex, "roBusinessSupport::ProgrammedHolidaysResumeQuery")
        Catch ex As Exception
            _State.UpdateStateInfo(ex, "roBusinessSupport::ProgrammedHolidaysResumeQuery")
        End Try

        Return bolRet

    End Function

    Public Shared Function GetExpiredConceptValues(ByVal IDEmployee As Integer, ByVal IDConcept As Integer, ByVal xBeginDate As DateTime, ByVal xEndDate As DateTime,
                                             ByVal _State As roBusinessState) As Double
        Dim intRet As Double = 0

        Try

            ' Obtenemos el periodo de calculo
            Dim dblTotalPeriodValue As Double = 0, dblNegativeValue As Double = 0
            Dim dblPositiveValue As Double = 0, dblRuleValue As Double = 0, dblTotalValue As Double = 0
            Dim strDefaultQuery As String = "C"
            Dim lstDates As New Generic.List(Of DateTime)
            Dim strSQL As String = "@SELECT# BeginDate, EndDate From EmployeeContracts WHERE IDEmployee = " & IDEmployee & " AND " &
                                       "BeginDate <= " & Any2Time(xBeginDate).SQLSmallDateTime & " AND EndDate >= " & Any2Time(xBeginDate).SQLSmallDateTime
            Dim oSqlCommand As DbCommand = CreateCommand(strSQL)
            Dim rd As DbDataReader = oSqlCommand.ExecuteReader()
            If rd.Read() Then
                lstDates.Add(rd("BeginDate"))
                lstDates.Add(rd("EndDate"))
            End If
            rd.Close()

            If lstDates.Count = 0 Then
                Return 0
            End If

            ' Obtenemos los valores del saldo que la fecha de caducidad este entre el periodo indicado
            strSQL = "@SELECT# Date, PositiveValue, ExpiredDate FROM DailyAccruals WHERE IDEmployee=" & IDEmployee.ToString & " AND ExpiredDate BETWEEN  " & Any2Time(xBeginDate.Date).SQLSmallDateTime & " AND " & Any2Time(xEndDate.Date).SQLSmallDateTime &
                        " AND IDConcept = " & IDConcept.ToString &
                        " AND Date <= " & Any2Time(lstDates(1).Date).SQLSmallDateTime &
                        " AND Date >= " & Any2Time(lstDates(0).Date).SQLSmallDateTime &
                        " ORDER BY Date"
            Dim tb As DataTable = CreateDataTable(strSQL, )
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                For Each orow As DataRow In tb.Rows
                    ' Para cada valor con fecha de caducidad dentro del periodo solicitado
                    ' debemos revisar si dicho valor ya se ha gastado o no y en caso necesario cuanto queda por gastar

                    ' Obtenemos el valor total del saldo el dia antes a la fecha del saldo que caduca (Date - 1)
                    dblTotalPeriodValue = Any2Double(ExecuteScalar("@SELECT# SUM(isnull(Value,0)) AS total FROM DailyAccruals WHERE IDEmployee=" & IDEmployee.ToString & " AND IDConcept = " & IDConcept.ToString & " AND Date >= " & Any2Time(lstDates(0).Date).SQLSmallDateTime & " AND Date < " & Any2Time(orow("Date")).SQLSmallDateTime))

                    ' Valor que caduca
                    dblPositiveValue = Any2Double(orow("PositiveValue"))

                    ' Obtenemos el total de valores negativos entre la fecha del valor hasta el dia que caduca (las dos incluidas)
                    dblNegativeValue = Any2Double(ExecuteScalar("@SELECT# SUM(isnull(NegativeValue,0)) AS NegativeValue FROM DailyAccruals WHERE IDEmployee=" & IDEmployee.ToString & " AND IDConcept = " & IDConcept.ToString & " AND Date >= " & Any2Time(orow("Date")).SQLSmallDateTime & " AND Date <= " & Any2Time(orow("ExpiredDate")).SQLSmallDateTime))

                    ' Obtenemos el total de valores de reglas de caducidad entre la fecha del valor hasta el dia anterior al que caduca (date)
                    dblRuleValue = Any2Double(ExecuteScalar("@SELECT# SUM(isnull(Value,0)) AS total FROM DailyAccruals WHERE IDEmployee=" & IDEmployee.ToString & " AND IDConcept = " & IDConcept.ToString & " AND Date >= " & Any2Time(orow("Date")).SQLSmallDateTime & " AND Date < " & Any2Time(orow("ExpiredDate")).SQLSmallDateTime & " AND CarryOver=1 AND StartupValue=0"))

                    ' Sumamos todos los valores
                    dblTotalValue = dblTotalPeriodValue + dblPositiveValue - dblNegativeValue + dblRuleValue - intRet

                    ' Si el total es positivo
                    ' si es 0 o negativo, el valor ya se ha gastado previamente
                    If dblTotalValue > 0 Then
                        ' dicho valor resultante es el que va a caducar,
                        ' tenemos que sumarlo al total de valores a caducar que se deben tener en cuenta
                        intRet += dblTotalValue
                    End If
                Next
            End If
        Catch ex As DbException
            _State.UpdateStateInfo(ex, "roBusinessSupport::GetExpiredConceptValues")
        Catch ex As Exception
            _State.UpdateStateInfo(ex, "roBusinessSupport::GetExpiredConceptValues")
        End Try

        Return intRet

    End Function

    ''' <summary>
    ''' Obtener número días ya disfrutados
    ''' </summary>
    ''' <param name="IDEmployee">ID de empleado a solicitar</param>
    ''' <param name="xCurrentDate">Fecha actual del periodo</param>
    ''' <param name="xBeginPeriod">Fecha inicio del periodo</param>
    ''' <param name="_State">Estado</param>
    ''' <param name="oActiveConnection">Conexión activa (opcional)</param>
    ''' <returns>Devuelve el núm. de días ya disfrutados</returns>
    ''' <remarks></remarks>
    Public Shared Function GetAlreadyTakenHollidays(ByVal IDEmployee As Integer, ByVal IDShift As Integer, ByVal xCurrentDate As DateTime, ByVal xBeginPeriod As DateTime,
                        ByVal _State As roBusinessState) As Double

        Dim intRet As Double = 0

        Try
            ' Obtenemos todos los horarios que sean de vacaciones
            ' y tengan el mismo saldo configurado que el indicado
            Dim strSQLShifts As String = "@SELECT# DISTINCT ID FROM Shifts WHERE IDConceptBalance = (@SELECT# IDConceptBalance FROM Shifts WHERE ID =" & IDShift.ToString & ")   AND isnull(IDConceptBalance,0) > 0 AND ShiftType = 2 "
            Dim tbShifts As DataTable = CreateDataTable(strSQLShifts, )
            If tbShifts IsNot Nothing AndAlso tbShifts.Rows.Count > 0 Then
                For Each cRow As DataRow In tbShifts.Rows
                    Dim strSQL As String
                    strSQL = "@SELECT# COUNT(*),  (@SELECT# isnull(dailyfactor, 1) from shifts where id=" & cRow("ID") & ") as DailyFactor FROM DailySchedule WITH (NOLOCK) INNER JOIN Employees ON DailySchedule.IDEmployee = Employees.[ID] INNER JOIN Shifts ON DailySchedule.IDShiftUsed = Shifts.[ID] " &
                         "WHERE DailySchedule.IDEmployee = " & IDEmployee & " AND Shifts.ID = " & cRow("ID") & "  AND DailySchedule.[Date] BETWEEN " &
                         Any2Time(xBeginPeriod).SQLSmallDateTime & " AND " & Any2Time(xCurrentDate).SQLSmallDateTime

                    Dim tb As DataTable = CreateDataTable(strSQL, )
                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                        intRet += (Any2Double(tb.Rows(0)(0)) * Any2Double(tb.Rows(0)(1)))
                    End If
                Next
            End If
        Catch ex As DbException
            _State.UpdateStateInfo(ex, "roBusinessSupport::GetAlreadyTakenHollidays")
        Catch ex As Exception
            _State.UpdateStateInfo(ex, "roBusinessSupport::GetAlreadyTakenHollidays")
        End Try

        Return intRet

    End Function

    ''' <summary>
    ''' Obtener número de días/horas solicitados de previsiones de ausencia por dia pendientes de procesar
    ''' </summary>
    ''' <param name="IDEmployee">ID de empleado a solicitar</param>
    ''' <param name="xBeginPeriod">Fecha inicio de periodo</param>
    ''' <param name="xEndPeriod">Fecha fin de periodo</param>
    ''' <param name="_State">Estado</param>
    ''' <param name="oActiveConnection">Conexión activa (opcional)</param>
    ''' <returns>Devuelve el núm. de dias pendientes de procesar </returns>
    ''' <remarks></remarks>
    Public Shared Function GetPendingApprovalPlannedAbsences(ByVal IDEmployee As Integer, ByVal IDCause As Integer, ByVal xBeginPeriod As DateTime, ByVal xEndPeriod As DateTime,
                ByVal _State As roBusinessState, ByVal bolApplyWorkDaysOnConcept As Boolean, ByVal strConceptType As String) As Double

        Dim intRet As Double = 0

        Try
            ' Obtenemos todas las solicitudes pendientes de previsiones de ausencia por dia del empleado
            ' con el motivo indicado
            Dim queryDateStart As String = roTypes.Any2Time(xBeginPeriod).SQLSmallDateTime()
            Dim queryDateEnd As String = roTypes.Any2Time(xEndPeriod).SQLSmallDateTime()

            Dim strsql As String = "@SELECT# ID, Date1, isnull(Date2,Date1) as Date2 FROM Requests WHERE "

            Dim strWhere As String = "(RequestType IN(" & eRequestType.PlannedAbsences & ")) "
            strWhere &= " And Requests.Status In (" & eRequestStatus.Pending & "," & eRequestStatus.OnGoing & ")"
            strWhere &= " And Requests.IDEmployee = " & IDEmployee
            strWhere &= " And Requests.IDCause = " & IDCause

            strsql &= strWhere

            Dim tb As DataTable = CreateDataTable(strsql, )
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                For Each oRow As DataRow In tb.Rows
                    ' Para cada solicitud
                    Dim xBeginDate As Date = oRow("Date1")
                    Dim xEndDate As Date = oRow("Date2")

                    ' Obtenemos las horas/dias a tener en cuenta
                    intRet += GetEffectiveDaysHoursAbsence(IDEmployee, xBeginDate, xEndDate, strConceptType, bolApplyWorkDaysOnConcept, _State)
                Next
            End If
        Catch ex As DbException
            _State.UpdateStateInfo(ex, "roBusinessSupport::GetPendingApprovalPlannedAbsences")
        Catch ex As Exception
            _State.UpdateStateInfo(ex, "roBusinessSupport::GetPendingApprovalPlannedAbsences")
        End Try

        Return intRet

    End Function

    ''' <summary>
    ''' Obtener número de días/horas solicitados de previsiones de ausencia por horas pendientes de procesar
    ''' </summary>
    ''' <param name="IDEmployee">ID de empleado a solicitar</param>
    ''' <param name="xBeginPeriod">Fecha inicio de periodo</param>
    ''' <param name="xEndPeriod">Fecha fin de periodo</param>
    ''' <param name="_State">Estado</param>
    ''' <param name="oActiveConnection">Conexión activa (opcional)</param>
    ''' <returns>Devuelve el núm. de dias pendientes de procesar </returns>
    ''' <remarks></remarks>
    Public Shared Function GetPendingApprovalPlannedCauses(ByVal IDEmployee As Integer, ByVal IDCause As Integer, ByVal xBeginPeriod As DateTime, ByVal xEndPeriod As DateTime,
                        ByVal _State As roBusinessState, ByVal bolApplyWorkDaysOnConcept As Boolean, ByVal strConceptType As String) As Double

        Dim intRet As Double = 0

        Try
            ' Obtenemos todas las solicitudes pendientes de previsiones de ausencia por horas del empleado
            ' con el motivo indicado
            Dim queryDateStart As String = roTypes.Any2Time(xBeginPeriod).SQLSmallDateTime()
            Dim queryDateEnd As String = roTypes.Any2Time(xEndPeriod).SQLSmallDateTime()

            Dim strsql As String = "@SELECT# ID, Date1, isnull(Date2,Date1) as Date2,  convert(numeric(8,6), isnull(Hours,0)) as Duration  FROM Requests WHERE "

            Dim strWhere As String = "(RequestType IN(" & eRequestType.PlannedCauses & ")) "
            strWhere &= " AND Requests.Status In (" & eRequestStatus.Pending & "," & eRequestStatus.OnGoing & ")"
            strWhere &= " AND Requests.IDEmployee = " & IDEmployee
            strWhere &= " AND Requests.IDCause = " & IDCause

            strsql &= strWhere

            Dim tb As DataTable = CreateDataTable(strsql, )
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                For Each oRow As DataRow In tb.Rows
                    ' Para cada solicitud
                    Dim xBeginDate As Date = oRow("Date1")
                    Dim xEndDate As Date = oRow("Date2")
                    Dim dblDuration As Double = oRow("Duration")

                    ' Obtenemos las horas/dias a tener en cuenta
                    intRet += GetEffectiveDaysHoursCauses(IDEmployee, xBeginDate, xEndDate, dblDuration, strConceptType, bolApplyWorkDaysOnConcept, _State)
                Next
            End If
        Catch ex As DbException
            _State.UpdateStateInfo(ex, "roBusinessSupport::GetPendingApprovalPlannedCauses")
        Catch ex As Exception
            _State.UpdateStateInfo(ex, "roBusinessSupport::GetPendingApprovalPlannedCauses")
        End Try

        Return intRet

    End Function

    Public Shared Function GetPendingApprovalPlannedOvertimes(ByVal IDEmployee As Integer, ByVal IDCause As Integer, ByVal xBeginPeriod As DateTime, ByVal xEndPeriod As DateTime,
                ByVal _State As roBusinessState, ByVal bolApplyWorkDaysOnConcept As Boolean, ByVal strConceptType As String) As Double

        Dim intRet As Double = 0
        Try

            ' Obtenemos todas las solicitudes pendientes de previsiones de ausencia por horas del empleado
            ' con el motivo indicado
            Dim queryDateStart As String = roTypes.Any2Time(xBeginPeriod).SQLSmallDateTime()
            Dim queryDateEnd As String = roTypes.Any2Time(xEndPeriod).SQLSmallDateTime()

            Dim strsql As String = "@SELECT# ID, Date1, isnull(Date2,Date1) as Date2,  convert(numeric(8,6), isnull(Hours,0)) as Duration  FROM Requests WHERE "

            Dim strWhere As String = "(RequestType IN(" & eRequestType.PlannedOvertimes & ")) "
            strWhere &= " AND Requests.Status In (" & eRequestStatus.Pending & "," & eRequestStatus.OnGoing & ")"
            strWhere &= " AND Requests.IDEmployee = " & IDEmployee
            strWhere &= " AND Requests.IDCause = " & IDCause

            strsql &= strWhere

            Dim tb As DataTable = CreateDataTable(strsql, )
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                For Each oRow As DataRow In tb.Rows
                    ' Para cada solicitud
                    Dim xBeginDate As Date = oRow("Date1")
                    Dim xEndDate As Date = oRow("Date2")
                    Dim dblDuration As Double = oRow("Duration")

                    ' Obtenemos las horas/dias a tener en cuenta
                    intRet += GetEffectiveDaysHoursCauses(IDEmployee, xBeginDate, xEndDate, dblDuration, strConceptType, bolApplyWorkDaysOnConcept, _State)
                Next
            End If
        Catch ex As DbException
            _State.UpdateStateInfo(ex, "roBusinessSupport::GetPendingApprovalPlannedOvertimes")
        Catch ex As Exception
            _State.UpdateStateInfo(ex, "roBusinessSupport::GetPendingApprovalPlannedOvertimes")
        End Try

        Return intRet

    End Function

    Public Shared Function GetEffectiveDaysHoursCauses(ByVal IDEmployee As Integer, ByVal xBeginDate As Date, ByVal xEndDate As Date, ByVal dblDuration As Double, ByVal strConceptType As String, ByVal bolApplyWorkDaysOnConcept As Boolean,
                                                 ByVal _State As roBusinessState) As Double
        Dim intRet As Double = 0
        Try

            ' En funcion del tipo de saldo
            Select Case strConceptType
                Case "H"
                    ' Tipo Horas
                    Dim strsql As String = ""
                    If Not bolApplyWorkDaysOnConcept Then
                        ' Si aplica en dias naturales
                        ' sumamos los dias del periodo indicado x la duracion de la ausencia por horas
                        intRet = Math.Abs(DateDiff(DateInterval.Day, xBeginDate, xEndDate)) + 1
                        intRet = intRet * dblDuration
                    Else
                        ' Si aplica en dias laborables
                        ' sumamos la duracion de la ausencia por horas, de los dias de trabajo del periodo indicado
                        strsql = "@SELECT# sum((case when isnull(D.IsHolidays,0) = 1 then 0 else case when isnull(D.ExpectedWorkingHours, S.ExpectedWorkingHours) = 0 then 0 else " & dblDuration.ToString.Replace(",", ".") & " end end)) ExpectedWorkingHours FROM DailySchedule D, Shifts S WHERE D.IDEmployee=" & IDEmployee.ToString
                        strsql += " AND D.Date >= " & Any2Time(xBeginDate).SQLSmallDateTime
                        strsql += " AND D.Date <= " & Any2Time(xEndDate).SQLSmallDateTime
                        strsql += " AND S.ID = D.IDShift1 "

                        intRet = Any2Double(ExecuteScalar(strsql))
                    End If

                Case Else
                    ' Tipo Dias

                    If Not bolApplyWorkDaysOnConcept Then
                        ' Si aplica en dias naturales
                        ' sumamos los dias del periodo inidcado
                        intRet = Math.Abs(DateDiff(DateInterval.Day, xBeginDate, xEndDate)) + 1
                    Else
                        ' Si aplica en dias laborables
                        ' sumamos los dias del periodo inidcado, que sean laborables
                        intRet = roBusinessSupport.LaboralDaysInPeriod(IDEmployee, xBeginDate, xEndDate, _State)
                    End If
            End Select
        Catch ex As DbException
            _State.UpdateStateInfo(ex, "roBusinessSupport::GetEffectiveDaysHoursAbsence")
        Catch ex As Exception
            _State.UpdateStateInfo(ex, "roBusinessSupport::GetEffectiveDaysHoursAbsence")
        End Try

        Return intRet

    End Function

    Public Shared Function GetEffectiveDaysHoursAbsence(ByVal IDEmployee As Integer, ByVal xBeginDate As Date, xEndDate As Date, ByVal strConceptType As String, ByVal bolApplyWorkDaysOnConcept As Boolean,
                                                 ByVal _State As roBusinessState) As Double
        Dim intRet As Double = 0
        Try

            ' En funcion del tipo de saldo
            Select Case strConceptType
                Case "H"
                    ' Tipo Horas
                    Dim strsql As String = ""
                    ' sumamos las horas teoricas de los dias del periodo indicado
                    strsql = "@SELECT# sum((case when isnull(D.IsHolidays,0) = 1 then 0 else isnull(D.ExpectedWorkingHours, S.ExpectedWorkingHours)  end)) ExpectedWorkingHours FROM DailySchedule D, Shifts S WHERE D.IDEmployee=" & IDEmployee.ToString
                    strsql += " AND D.Date >= " & Any2Time(xBeginDate).SQLSmallDateTime
                    strsql += " AND D.Date <= " & Any2Time(xEndDate).SQLSmallDateTime
                    strsql += " AND S.ID = D.IDShift1 "
                    intRet = Any2Double(ExecuteScalar(strsql))
                Case Else
                    ' Tipo Dias

                    If Not bolApplyWorkDaysOnConcept Then
                        ' Si aplica en dias naturales
                        ' sumamos los dias del periodo inidcado
                        intRet = Math.Abs(DateDiff(DateInterval.Day, xBeginDate, xEndDate)) + 1
                    Else
                        ' Si aplica en dias laborables
                        ' sumamos los dias del periodo inidcado, que sean laborables
                        intRet = roBusinessSupport.LaboralDaysInPeriod(IDEmployee, xBeginDate, xEndDate, _State)
                    End If
            End Select
        Catch ex As DbException
            _State.UpdateStateInfo(ex, "roBusinessSupport::GetEffectiveDaysHoursAbsence")
        Catch ex As Exception
            _State.UpdateStateInfo(ex, "roBusinessSupport::GetEffectiveDaysHoursAbsence")
        End Try

        Return intRet

    End Function

    ''' <summary>
    ''' Obtener número de días solicitados pendientes de procesar
    ''' </summary>
    ''' <param name="IDEmployee">ID de empleado a solicitar</param>
    ''' <param name="xBeginPeriod">Fecha inicio de periodo</param>
    ''' <param name="xEndPeriod">Fecha fin de periodo</param>
    ''' <param name="_State">Estado</param>
    ''' <param name="oActiveConnection">Conexión activa (opcional)</param>
    ''' <returns>Devuelve el núm. de dias pendientes de procesar </returns>
    ''' <remarks></remarks>
    Public Shared Function GetPendingApprovalHollidays(ByVal IDEmployee As Integer, ByVal IDShift As Integer, ByVal xBeginPeriod As DateTime, ByVal xEndPeriod As DateTime, ByVal bolAreWorkingDays As Boolean,
                ByVal _State As roBusinessState, Optional lstHolidaysDays As List(Of roHolidaysDay) = Nothing) As Double

        Dim intRet As Double = 0
        Try

            ' Obtenemos todos los horarios que sean de vacaciones
            ' y tengan el mismo saldo configurado que el indicado
            Dim strSQLShifts As String = "@SELECT# DISTINCT ID FROM Shifts WHERE IDConceptBalance = (@SELECT# IDConceptBalance FROM Shifts WHERE ID =" & IDShift.ToString & ")   AND isnull(IDConceptBalance,0) > 0 AND ShiftType = 2 "
            Dim tbShifts As DataTable = CreateDataTable(strSQLShifts, )
            If tbShifts IsNot Nothing AndAlso tbShifts.Rows.Count > 0 Then
                For Each cRow As DataRow In tbShifts.Rows
                    Dim strSQL As String
                    strSQL = "@SELECT# sysroRequestDays.* " &
                         "FROM Requests WITH (NOLOCK) INNER JOIN Shifts " &
                                    "ON Requests.IDShift = Shifts.[ID] " &
                                    " INNER JOIN sysroRequestDays WITH (NOLOCK) " &
                                    "On Requests.ID = sysroRequestDays.[IDRequest] " &
                         "WHERE Requests.IDEmployee = " & IDEmployee.ToString & " And " &
                               "Shifts.ShiftType = 2 And " &
                               "Requests.IDShift = " & cRow("ID") & " And " &
                               "Requests.RequestType = " & eRequestType.VacationsOrPermissions & " And " &
                               "Requests.Status In (" & eRequestStatus.Pending & "," & eRequestStatus.OnGoing & ")"
                    If xBeginPeriod <> Nothing Then
                        strSQL = strSQL + " AND sysroRequestDays.Date >= " & Any2Time(xBeginPeriod).SQLSmallDateTime
                    End If

                    If xEndPeriod <> Nothing Then
                        strSQL = strSQL + " AND sysroRequestDays.Date <= " & Any2Time(xEndPeriod).SQLDateTime
                    End If

                    Dim tb As DataTable = CreateDataTable(strSQL, )
                    If tb IsNot Nothing Then
                        Dim dblFactor As Double = 1
                        dblFactor = Any2Double(ExecuteScalar("@SELECT# isnull(dailyfactor, 1) from shifts where id=" & cRow("ID")))
                        intRet += (Any2Double(tb.Rows.Count) * dblFactor)
                        If lstHolidaysDays IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                            For Each orow As DataRow In tb.Rows
                                Dim oHolidaysDay As New roHolidaysDay
                                oHolidaysDay.HolidayDate = orow("Date")
                                oHolidaysDay.IsExpiredDate = 0
                                oHolidaysDay.Factor = dblFactor
                                lstHolidaysDays.Add(oHolidaysDay)
                            Next
                        End If

                    End If
                Next
            End If
        Catch ex As DbException
            _State.UpdateStateInfo(ex, "roBusinessSupport::GetPendingApprovalHollidays")
        Catch ex As Exception
            _State.UpdateStateInfo(ex, "roBusinessSupport::GetPendingApprovalHollidays")
        End Try

        Return intRet

    End Function

    ''' <summary>
    ''' Obtener número de horas de vacaciones solicitados pendientes de procesar
    ''' </summary>
    ''' <param name="IDEmployee">ID de empleado a solicitar</param>
    ''' <param name="xBeginPeriod">Fecha inicio de periodo</param>
    ''' <param name="xEndPeriod">Fecha fin de periodo</param>
    ''' <param name="_State">Estado</param>
    ''' <param name="oActiveConnection">Conexión activa (opcional)</param>
    ''' <returns>Devuelve el núm. de dias pendientes de procesar </returns>
    ''' <remarks></remarks>
    Public Shared Function GetPendingApprovalProgrammedHolidays(ByVal IDEmployee As Integer, ByVal IDCause As Integer, ByVal xBeginPeriod As DateTime, ByVal xEndPeriod As DateTime,
                ByVal _State As roBusinessState) As Double

        Dim intRet As Double = 0
        Try

            Dim strSQL As String
            strSQL = "@SELECT# sysroRequestDays.* " &
                         "FROM Requests INNER JOIN sysroRequestDays " &
                                    "ON Requests.ID = sysroRequestDays.[IDRequest] " &
                         "WHERE Requests.IDEmployee = " & IDEmployee.ToString & " AND " &
                               "Requests.IDCause = " & IDCause & " AND " &
                               "Requests.RequestType = " & eRequestType.PlannedHolidays & " AND " &
                               "Requests.Status IN (" & eRequestStatus.Pending & "," & eRequestStatus.OnGoing & ") "
            Dim tb As DataTable = CreateDataTable(strSQL, )
            If tb IsNot Nothing Then
                For Each oRow As DataRow In tb.Rows

                    If Any2Boolean(oRow("AllDay")) Then
                        ' Si es todo el dia, obtenemos las horas teoricas del horario
                        strSQL = "@SELECT# (case when isnull(D.IsHolidays,0) = 1 then 0 else isnull(D.ExpectedWorkingHours, S.ExpectedWorkingHours)  end) ExpectedWorkingHours FROM DailySchedule D, Shifts S WHERE D.IDEmployee=" & IDEmployee.ToString
                        strSQL += " AND S.ID = D.IDShift1 "
                        strSQL += " AND D.Date = " & Any2Time(oRow("Date")).SQLSmallDateTime
                        intRet += Any2Double(ExecuteScalar(strSQL))
                    Else
                        ' Obtenemos la duracion
                        intRet += Any2Double(oRow("Duration"))
                    End If
                Next
            End If
        Catch ex As DbException
            _State.UpdateStateInfo(ex, "roBusinessSupport::GetPendingApprovalProgrammedHolidays")
        Catch ex As Exception
            _State.UpdateStateInfo(ex, "roBusinessSupport::GetPendingApprovalProgrammedHolidays")
        End Try

        Return intRet

    End Function

    ''' <summary>
    ''' Obtener el número de días aprobados pendientes de disfrutar
    ''' </summary>
    ''' <param name="IDEmployee">ID de empleado a solicitar</param>
    ''' <param name="xCurrentDate">Fecha actual</param>
    ''' <param name="xEndPeriod">Fecha fin de periodo</param>
    ''' <param name="_State">Estado</param>
    ''' <param name="oActiveConnection">Conexión activa (opcional)</param>
    ''' <returns>Devuelve el núm. de días</returns>
    ''' <remarks></remarks>
    Public Shared Function GetApprovedButNotTakenHolidays(ByVal IDEmployee As Integer, ByVal IDShift As Integer, ByVal xCurrentDate As DateTime, ByVal xEndPeriod As DateTime,
                ByVal _State As roBusinessState, Optional lstHolidaysDays As List(Of roHolidaysDay) = Nothing) As Double

        Dim intRet As Double = 0

        Try
            Dim strSQLShifts As String = "@SELECT# DISTINCT ID FROM Shifts WHERE IDConceptBalance = (@SELECT# IDConceptBalance FROM Shifts WHERE ID =" & IDShift.ToString & ")   AND isnull(IDConceptBalance,0) > 0 AND ShiftType = 2 "
            Dim tbShifts As DataTable = CreateDataTable(strSQLShifts, )
            If tbShifts IsNot Nothing AndAlso tbShifts.Rows.Count > 0 Then
                For Each cRow As DataRow In tbShifts.Rows
                    Dim strSQL As String

                    strSQL = "@SELECT# COUNT(*) ,  (@SELECT# isnull(dailyfactor, 1) from shifts where id=" & cRow("ID") & ") as DailyFactor " &
                         "FROM DailySchedule WITH(NOLOCK) INNER JOIN Employees " &
                                "ON DailySchedule.IDEmployee = Employees.[ID] " &
                         "WHERE DailySchedule.IDEmployee = " & IDEmployee.ToString & " AND " &
                               "DailySchedule.IDShift1 = " & cRow("ID") & " AND " &
                               "DailySchedule.[Date] BETWEEN " &
                                    "CONVERT(smalldatetime, '" & Format(xCurrentDate.AddDays(1).Date, "dd/MM/yyyy") & "', 103) AND " &
                                    "CONVERT(smalldatetime, '" & Format(xEndPeriod.Date, "dd/MM/yyyy") & "', 103)"

                    Dim tb As DataTable = CreateDataTable(strSQL, )
                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                        intRet += (Any2Double(tb.Rows(0)(0)) * Any2Double(tb.Rows(0)(1)))
                        If lstHolidaysDays IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                            Dim tbaux As DataTable = CreateDataTable(strSQL.Replace("COUNT(*)", "Date"))
                            If tbaux IsNot Nothing AndAlso tbaux.Rows.Count > 0 Then
                                For Each orow As DataRow In tbaux.Rows
                                    Dim oHolidaysDay As New roHolidaysDay
                                    oHolidaysDay.HolidayDate = orow("Date")
                                    oHolidaysDay.IsExpiredDate = 0
                                    oHolidaysDay.Factor = orow("DailyFactor")
                                    lstHolidaysDays.Add(oHolidaysDay)
                                Next
                            End If
                        End If
                    End If
                Next
            End If
        Catch ex As DbException
            _State.UpdateStateInfo(ex, "roBusinessSupport::GetApprovedButNotTakenHolidays")
        Catch ex As Exception
            _State.UpdateStateInfo(ex, "roBusinessSupport::GetApprovedButNotTakenHolidays")
        End Try

        Return intRet

    End Function

    ''' <summary>
    ''' Obtener el número de horas/dias aprobados pendientes de disfrutar
    ''' </summary>
    ''' <param name="IDEmployee">ID de empleado a solicitar</param>
    ''' <param name="xCurrentDate">Fecha actual</param>
    ''' <param name="xEndPeriod">Fecha fin de periodo</param>
    ''' <param name="_State">Estado</param>
    ''' <param name="oActiveConnection">Conexión activa (opcional)</param>
    ''' <returns>Devuelve el núm. de días</returns>
    ''' <remarks></remarks>
    Public Shared Function GetApprovedButNotTakenPlannedAbsences(ByVal IDEmployee As Integer, ByVal IDCause As Integer, ByVal xCurrentDate As DateTime, ByVal xEndPeriod As DateTime,
                ByVal _State As roBusinessState, ByVal bolApplyWorkDaysOnConcept As Boolean, ByVal strConceptType As String) As Double

        Dim intRet As Double = 0

        Try

            Dim queryDateStart As String = roTypes.Any2Time(xCurrentDate.AddDays(1).Date).SQLSmallDateTime()
            Dim queryDateEnd As String = roTypes.Any2Time(xEndPeriod).SQLSmallDateTime()

            Dim strWhere As String = ""

            strWhere &= " ( (BeginDate >= " & queryDateStart & " AND BeginDate <= " & queryDateEnd & ")"
            strWhere &= " OR "
            strWhere &= " (ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) >= " & queryDateStart & " AND ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) <= " & queryDateEnd & ")"
            strWhere &= " OR "
            strWhere &= " (BeginDate <= " & queryDateStart & " AND ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) >= " & queryDateEnd & ") ) "
            strWhere &= " AND Causes.ID=" & IDCause

            Dim strSQL As String
            strSQL = "@SELECT# BeginDate, CASE WHEN FinishDate IS NULL THEN DATEADD(day, MaxLastingDays-1, BeginDate) ELSE FinishDate END AS RealFinishDate " &
                         "FROM ProgrammedAbsences " &
                                    "INNER JOIN Causes On Causes.ID = ProgrammedAbsences.IDCause " &
                         "WHERE idEmployee = " & IDEmployee
            strSQL = strSQL & " AND " & strWhere
            strSQL = strSQL & " ORDER BY BeginDate ASC"
            Dim tb As DataTable = CreateDataTable(strSQL, )
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                For Each oRow As DataRow In tb.Rows
                    ' Para cada previson de ausencia por dias

                    ' Obtenemos las fechas a tener en cuenta en funcion del periodo a consultar
                    Dim xBeginDate As Date = xCurrentDate.AddDays(1).Date
                    If oRow("BeginDate") > xBeginDate Then xBeginDate = oRow("BeginDate")

                    Dim xEndDate As Date = xEndPeriod
                    If oRow("RealFinishDate") < xEndDate Then xEndDate = oRow("RealFinishDate")

                    ' Obtenemos las horas/dias a tener en cuenta
                    intRet += GetEffectiveDaysHoursAbsence(IDEmployee, xBeginDate, xEndDate, strConceptType, bolApplyWorkDaysOnConcept, _State)
                Next

            End If
        Catch ex As DbException
            _State.UpdateStateInfo(ex, "roBusinessSupport::GetApprovedButNotTakenPlannedAbsences")
        Catch ex As Exception
            _State.UpdateStateInfo(ex, "roBusinessSupport::GetApprovedButNotTakenPlannedAbsences")
        End Try

        Return intRet

    End Function

    ''' <summary>
    ''' Obtener el número de horas aprobados pendientes de disfrutar
    ''' </summary>
    ''' <param name="IDEmployee">ID de empleado a solicitar</param>
    ''' <param name="xCurrentDate">Fecha actual</param>
    ''' <param name="xEndPeriod">Fecha fin de periodo</param>
    ''' <param name="_State">Estado</param>
    ''' <param name="oActiveConnection">Conexión activa (opcional)</param>
    ''' <returns>Devuelve el núm. de días</returns>
    ''' <remarks></remarks>
    Public Shared Function GetApprovedButNotTakenProgrammedHolidays(ByVal IDEmployee As Integer, ByVal IDCause As Integer, ByVal xCurrentDate As DateTime, ByVal xEndPeriod As DateTime,
                ByVal _State As roBusinessState) As Double

        Dim intRet As Double = 0

        Try

            Dim strSQL As String
            strSQL = "@SELECT# * " &
                         "FROM ProgrammedHolidays INNER JOIN Employees " &
                                "ON ProgrammedHolidays.IDEmployee = Employees.[ID] " &
                         "WHERE ProgrammedHolidays.IDEmployee = " & IDEmployee.ToString & " AND " &
                               "ProgrammedHolidays.[Date] BETWEEN " &
                                    Any2Time(xCurrentDate.AddDays(1).Date).SQLSmallDateTime & " AND " &
                                     Any2Time(xEndPeriod.Date).SQLSmallDateTime &
                                     " AND IDCause=" & IDCause

            Dim tb As DataTable = CreateDataTable(strSQL, )
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                For Each oRow As DataRow In tb.Rows

                    If Any2Boolean(oRow("AllDay")) Then
                        ' Si es todo el dia, obtenemos las horas teoricas del horario
                        strSQL = "@SELECT# (case when isnull(D.IsHolidays,0) = 1 then 0 else isnull(D.ExpectedWorkingHours, S.ExpectedWorkingHours)  end) ExpectedWorkingHours FROM DailySchedule D, Shifts S WHERE D.IDEmployee=" & IDEmployee.ToString
                        strSQL += " AND S.ID = D.IDShift1 "
                        strSQL += " AND D.Date = " & Any2Time(oRow("Date")).SQLSmallDateTime
                        intRet += Any2Double(ExecuteScalar(strSQL))
                    Else
                        ' Obtenemos la duracion
                        intRet += Any2Double(oRow("Duration"))
                    End If
                Next

            End If
        Catch ex As DbException
            _State.UpdateStateInfo(ex, "roBusinessSupport::GetApprovedButNotTakenProgrammedHolidays")
        Catch ex As Exception
            _State.UpdateStateInfo(ex, "roBusinessSupport::GetApprovedButNotTakenProgrammedHolidays")
        End Try

        Return intRet

    End Function

    ''' <summary>
    ''' Obtener el número de horas aprobados pendientes de disfrutar
    ''' </summary>
    ''' <param name="IDEmployee">ID de empleado a solicitar</param>
    ''' <param name="xCurrentDate">Fecha actual</param>
    ''' <param name="xEndPeriod">Fecha fin de periodo</param>
    ''' <param name="_State">Estado</param>
    ''' <param name="oActiveConnection">Conexión activa (opcional)</param>
    ''' <returns>Devuelve el núm. de días</returns>
    ''' <remarks></remarks>
    Public Shared Function GetApprovedButNotTakenPlannedCauses(ByVal IDEmployee As Integer, ByVal IDCause As Integer, ByVal xCurrentDate As DateTime, ByVal xEndPeriod As DateTime,
                ByVal _State As roBusinessState, ByVal bolApplyWorkDaysOnConcept As Boolean, ByVal strConceptType As String) As Double

        Dim intRet As Double = 0

        Try

            Dim queryDateStart As String = roTypes.Any2Time(xCurrentDate.AddDays(1).Date).SQLSmallDateTime()
            Dim queryDateEnd As String = roTypes.Any2Time(xEndPeriod).SQLSmallDateTime()

            Dim strWhere As String = ""

            strWhere &= " ( (Date >= " & queryDateStart & " AND Date <= " & queryDateEnd & ")"
            strWhere &= " OR "
            strWhere &= " (IsNULL(FinishDate,Date) >= " & queryDateStart & " AND IsNULL(FinishDate,Date) <= " & queryDateEnd & ")"
            strWhere &= " OR "
            strWhere &= " (Date <= " & queryDateStart & " AND IsNULL(FinishDate,Date) >= " & queryDateEnd & ") ) "
            strWhere &= " AND Causes.ID=" & IDCause

            Dim strSQL As String
            strSQL = "@SELECT# Date as BeginDate, ISNULL(FinishDate, Date) AS RealFinishDate, convert(numeric(8,6), Duration) as duration " &
                         "FROM ProgrammedCauses " &
                                    "INNER JOIN Causes On Causes.ID = ProgrammedCauses.IDCause " &
                         "WHERE idEmployee = " & IDEmployee
            strSQL = strSQL & " AND " & strWhere
            strSQL = strSQL & " ORDER BY BeginDate ASC"
            Dim tb As DataTable = CreateDataTable(strSQL, )
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                For Each oRow As DataRow In tb.Rows
                    ' Para cada previson de ausencia por horas

                    ' Obtenemos las fechas a tener en cuenta en funcion del periodo a consultar
                    Dim xBeginDate As Date = xCurrentDate.AddDays(1).Date
                    If oRow("BeginDate") > xBeginDate Then xBeginDate = oRow("BeginDate")

                    Dim xEndDate As Date = xEndPeriod
                    If oRow("RealFinishDate") < xEndDate Then xEndDate = oRow("RealFinishDate")

                    Dim dblDuration As Double = oRow("Duration")

                    ' Obtenemos las horas/dias a tener en cuenta
                    intRet += GetEffectiveDaysHoursCauses(IDEmployee, xBeginDate, xEndDate, dblDuration, strConceptType, bolApplyWorkDaysOnConcept, _State)
                Next

            End If
        Catch ex As DbException
            _State.UpdateStateInfo(ex, "roBusinessSupport::GetApprovedButNotTakenPlannedCauses")
        Catch ex As Exception
            _State.UpdateStateInfo(ex, "roBusinessSupport::GetApprovedButNotTakenPlannedCauses")
        End Try

        Return intRet

    End Function

    Public Shared Function GetApprovedButNotTakenPlannedOvertimes(ByVal IDEmployee As Integer, ByVal IDCause As Integer, ByVal xCurrentDate As DateTime, ByVal xEndPeriod As DateTime,
                ByVal _State As roBusinessState, ByVal bolApplyWorkDaysOnConcept As Boolean, ByVal strConceptType As String) As Double

        Dim intRet As Double = 0

        Try

            Dim queryDateStart As String = roTypes.Any2Time(xCurrentDate.AddDays(1).Date).SQLSmallDateTime()
            Dim queryDateEnd As String = roTypes.Any2Time(xEndPeriod).SQLSmallDateTime()

            Dim strWhere As String = ""

            strWhere &= " ( (BeginDate >= " & queryDateStart & " AND BeginDate <= " & queryDateEnd & ")"
            strWhere &= " OR "
            strWhere &= " (IsNULL(EndDate,BeginDate) >= " & queryDateStart & " AND IsNULL(EndDate,BeginDate) <= " & queryDateEnd & ")"
            strWhere &= " OR "
            strWhere &= " (BeginDate <= " & queryDateStart & " AND IsNULL(EndDate,BeginDate) >= " & queryDateEnd & ") ) "
            strWhere &= " AND Causes.ID=" & IDCause

            Dim strSQL As String
            strSQL = "@SELECT# BeginDate as BeginDate, ISNULL(EndDate, BeginDate) AS RealFinishDate, convert(numeric(8,6), Duration) as duration " &
                         "FROM ProgrammedOvertimes " &
                                    "INNER JOIN Causes On Causes.ID = ProgrammedOvertimes.IDCause " &
                         "WHERE idEmployee = " & IDEmployee
            strSQL = strSQL & " AND " & strWhere
            strSQL = strSQL & " ORDER BY BeginDate ASC"
            Dim tb As DataTable = CreateDataTable(strSQL, )
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                For Each oRow As DataRow In tb.Rows
                    ' Para cada previson de ausencia por horas

                    ' Obtenemos las fechas a tener en cuenta en funcion del periodo a consultar
                    Dim xBeginDate As Date = xCurrentDate.AddDays(1).Date
                    If oRow("BeginDate") > xBeginDate Then xBeginDate = oRow("BeginDate")

                    Dim xEndDate As Date = xEndPeriod
                    If oRow("RealFinishDate") < xEndDate Then xEndDate = oRow("RealFinishDate")

                    Dim dblDuration As Double = oRow("Duration")

                    ' Obtenemos las horas/dias a tener en cuenta
                    intRet += GetEffectiveDaysHoursCauses(IDEmployee, xBeginDate, xEndDate, dblDuration, strConceptType, bolApplyWorkDaysOnConcept, _State)
                Next

            End If
        Catch ex As DbException
            _State.UpdateStateInfo(ex, "roBusinessSupport::GetApprovedButNotTakenPlannedOvertimes")
        Catch ex As Exception
            _State.UpdateStateInfo(ex, "roBusinessSupport::GetApprovedButNotTakenPlannedOvertimes")
        End Try

        Return intRet

    End Function

    Public Shared Function GetDisponibleProgrammedHolidays(ByVal IDEmployee As Integer, ByVal IDCause As Integer, ByVal xBeginPeriod As DateTime, ByVal xEndPeriod As DateTime,
                                              ByVal _State As roBusinessState) As Double
        Dim intRet As Double = 0

        Dim rd As DbDataReader = Nothing

        Try
            Dim strSQL As String
            strSQL = "@SELECT# isnull(SUM(DailyAccruals.Value), 0) as Total " &
                         "FROM DailyAccruals INNER JOIN Causes " &
                                "ON DailyAccruals.IDConcept = Causes.[IDConceptBalance] " &
                         "WHERE Causes.ID = " & IDCause & " AND " &
                               "DailyAccruals.IDEmployee = " & IDEmployee.ToString & " AND " &
                               "DailyAccruals.[Date] BETWEEN " &
                                    Any2Time(xBeginPeriod.Date).SQLSmallDateTime & " AND " &
                                    Any2Time(xEndPeriod.Date).SQLSmallDateTime & " AND " &
                               "DailyAccruals.[Date] <= " & Any2Time(Now.Date).SQLSmallDateTime

            Dim tb As DataTable = CreateDataTable(strSQL, )
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                intRet = Any2Double(tb.Rows(0).Item(0))
            End If
        Catch ex As DbException
            _State.UpdateStateInfo(ex, "roBusinessSupport::GetDisponibleProgrammedHolidays")
        Catch ex As Exception
            _State.UpdateStateInfo(ex, "roBusinessSupport::GetDisponibleProgrammedHolidays")
        End Try

        Return intRet

    End Function

    Public Shared Function GetDisponiblePlannedAbsences(ByVal IDEmployee As Integer, ByVal IDCause As Integer, ByVal xBeginPeriod As DateTime, ByVal xEndPeriod As DateTime,
                                              ByVal _State As roBusinessState) As Double

        Dim intRet As Double = 0

        Dim rd As DbDataReader = Nothing

        Try

            Dim strSQL As String
            strSQL = "@SELECT# isnull(SUM(DailyAccruals.Value), 0) as Total " &
                         "FROM DailyAccruals INNER JOIN Concepts " &
                                "ON DailyAccruals.IDConcept = Concepts.[ID] " &
                                "INNER JOIN Causes ON Concepts.[ID] = Causes.IDConceptBalance " &
                         "WHERE Causes.ID = " & IDCause & " AND " &
                               "DailyAccruals.IDEmployee = " & IDEmployee.ToString & " AND " &
                               "DailyAccruals.[Date] BETWEEN " &
                                    Any2Time(xBeginPeriod.Date).SQLSmallDateTime & " AND " &
                                    Any2Time(xEndPeriod.Date).SQLSmallDateTime & " AND " &
                               "DailyAccruals.[Date] < " & Any2Time(Now.Date).SQLSmallDateTime

            Dim tb As DataTable = CreateDataTable(strSQL, )
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                intRet = Any2Double(tb.Rows(0).Item(0))
            End If
        Catch ex As DbException
            _State.UpdateStateInfo(ex, "roBusinessSupport::GetDisponiblePlannedAbsences")
        Catch ex As Exception
            _State.UpdateStateInfo(ex, "roBusinessSupport::GetDisponiblePlannedAbsences")
        End Try

        Return intRet

    End Function

    Public Shared Function GetAvailableHolidays(ByVal IDEmployee As Integer, ByVal IDShift As Integer, ByVal xBeginPeriod As DateTime, ByVal xEndPeriod As DateTime,
                                              ByVal _State As roBusinessState, ByVal strDefaultQuery As String, Optional ByVal IDContract As String = "", Optional ByRef lstHolidaysSummaryByPeriod As Generic.List(Of roHolidaysSummaryByPeriod) = Nothing, Optional ByRef intIDConcept As Integer = 0) As Double

        Dim intRet As Double = 0

        Dim rd As DbDataReader = Nothing

        Try

            Dim strSQL As String

            lstHolidaysSummaryByPeriod = New Generic.List(Of roHolidaysSummaryByPeriod)


            strSQL = "@SELECT# isnull(SUM(DailyAccruals.Value), 0) as Total "
            If strDefaultQuery = "L" Then
                strSQL = strSQL & " , DailyAccruals.BeginPeriod, DailyAccruals.EndPeriod "
                intIDConcept = roTypes.Any2Integer(ExecuteScalar("@SELECT# ISNULL(ID , 0) FROM Concepts WHERE ID in(@SELECT# isnull(IDConceptBalance,0) from Shifts where id =" & IDShift.ToString & ")"))
            End If

            strSQL = strSQL & " FROM DailyAccruals WITH(NOLOCK) INNER JOIN Concepts " &
                                " ON DailyAccruals.IDConcept = Concepts.[ID] " &
                                " INNER JOIN Shifts ON Concepts.[ID] = Shifts.IDConceptBalance "
            If IDContract.Length > 0 Then
                strSQL = strSQL & " LEFT JOIN EmployeeContracts on DailyAccruals.[IDEmployee] = EmployeeContracts.[IDEmployee] AND EmployeeContracts.[IDContract] LIKE '" & IDContract.Replace("'", "''") & "'"
            End If
            strSQL = strSQL & " WHERE Shifts.ShiftType = 2 AND Shifts.ID = " & IDShift.ToString & " AND " &
                               " DailyAccruals.IDEmployee = " & IDEmployee.ToString & " AND " &
                               " DailyAccruals.[Date] BETWEEN " &
                                    Any2Time(xBeginPeriod.Date).SQLSmallDateTime & " AND " &
                                    Any2Time(xEndPeriod.Date).SQLSmallDateTime & " AND " &
                               " DailyAccruals.[Date] <= " & Any2Time(Now.Date).SQLSmallDateTime
            If IDContract.Length > 0 Then
                strSQL = strSQL & " AND DailyAccruals.[Date] BETWEEN EmployeeContracts.[BeginDate] AND EmployeeContracts.[EndDate]"
            End If

            If strDefaultQuery = "L" Then
                strSQL = strSQL & " Group by DailyAccruals.BeginPeriod, DailyAccruals.EndPeriod Order by DailyAccruals.BeginPeriod"
            End If

            Dim tb As DataTable = CreateDataTable(strSQL, )
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                intRet = Any2Double(tb.Rows(0).Item(0))
                If strDefaultQuery = "L" Then
                    ' En el caso, debemos agrupar los totales por tramos anuales, indicando en cada tramo la info necesaria
                    intRet = 0
                    For Each orow As DataRow In tb.Rows
                        Dim oHolidaysSummaryByPeriod As New roHolidaysSummaryByPeriod
                        oHolidaysSummaryByPeriod.IDConcept = intIDConcept
                        oHolidaysSummaryByPeriod.BeginPeriod = orow("BeginPeriod")
                        oHolidaysSummaryByPeriod.EndPeriod = orow("EndPeriod")
                        oHolidaysSummaryByPeriod.ActualValue = Any2Double(orow("Total"))
                        oHolidaysSummaryByPeriod.ExpectedValue = oHolidaysSummaryByPeriod.ActualValue
                        intRet += Any2Double(orow("Total"))
                        strSQL = "@SELECT# isnull(Value, 0) as StartupValueTotal, ExpiredDate, StartEnjoymentDate FROM DailyAccruals WITH(NOLOCK) WHERE IDEmployee = " & IDEmployee.ToString & " AND IDConcept= " & intIDConcept.ToString & " AND CarryOver = 1 AND StartupValue= 1 AND BeginPeriod=" & Any2Time(orow("BeginPeriod")).SQLSmallDateTime & " AND Date=" & Any2Time(orow("BeginPeriod")).SQLSmallDateTime
                        Dim StartupDatatb As DataTable = CreateDataTable(strSQL, )
                        If StartupDatatb IsNot Nothing AndAlso StartupDatatb.Rows.Count = 1 Then
                            oHolidaysSummaryByPeriod.StartupValue = Any2Double(StartupDatatb.Rows(0)("StartupValueTotal"))
                            oHolidaysSummaryByPeriod.ExpiredDate = IIf(IsDBNull(StartupDatatb.Rows(0)("ExpiredDate")), Nothing, StartupDatatb.Rows(0)("ExpiredDate"))
                            oHolidaysSummaryByPeriod.StartEnjoymentDate = IIf(IsDBNull(StartupDatatb.Rows(0)("StartEnjoymentDate")), Nothing, StartupDatatb.Rows(0)("StartEnjoymentDate"))
                        End If
                        lstHolidaysSummaryByPeriod.Add(oHolidaysSummaryByPeriod)
                    Next
                End If
            End If
        Catch ex As DbException
            _State.UpdateStateInfo(ex, "roBusinessSupport::GetDisponibleHolidays")
        Catch ex As Exception
            _State.UpdateStateInfo(ex, "roBusinessSupport::GetDisponibleHolidays")
        End Try
        Return intRet
    End Function

    ''' <summary>
    ''' Devuelve el número de días laborales que hay en el periodo indicado (se incluye fecha de inicio y fecha de fin)
    ''' </summary>
    ''' <param name="IDEmployee">ID de empleado a solicitar</param>
    ''' <param name="xBeginPeriod">Fecha inicio de periodo</param>
    ''' <param name="xEndPeriod">Fecha fin de periodo</param>
    ''' <param name="_State">Estado</param>
    ''' <param name="oActiveConnection">Conexión activa (opcional)</param>
    ''' <returns>Núm. de días laborales que hay en el periodo indicado</returns>
    ''' <remarks></remarks>
    Public Shared Function HolidayDaysInPeriod(ByVal IDEmployee As Integer, ByVal xBeginPeriod As DateTime, ByVal xEndPeriod As DateTime,
                                            ByVal _State As roBusinessState) As Integer
        Dim intRet As Integer = 0

        Dim tb As DataTable = Nothing
        Try

            Dim strSQL As String = "@SELECT# COUNT(*) " &
                                       "FROM DailySchedule " &
                                       "WHERE DailySchedule.IDEmployee = " & IDEmployee.ToString & " AND " &
                                             "DailySchedule.[Date] BETWEEN CONVERT(smalldatetime, '" & Format(xBeginPeriod.Date, "dd/MM/yyyy") & "', 103) AND " &
                                                                          "CONVERT(smalldatetime, '" & Format(xEndPeriod.Date, "dd/MM/yyyy") & "', 103) AND " &
                                             "(DailySchedule.IsHolidays = 1)"
            tb = CreateDataTable(strSQL)
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                intRet = Any2Integer(tb.Rows(0)(0))
            End If
        Catch ex As DbException
            _State.UpdateStateInfo(ex, "roBusinessSupport::LaboralDaysInPeriod")
        Catch ex As Exception
            _State.UpdateStateInfo(ex, "roBusinessSupport::LaboralDaysInPeriod")
        End Try

        Return intRet

    End Function

#End Region

#Region "Employee list querys"

    Public Shared Function GetEmployeesByAdvancedFilter(ByVal strAdvancedFilter As String, ByVal strWhere As String, ByVal Feature As String, ByVal Type As String, ByVal _State As roBusinessState) As DataTable
        Dim strDECLARE As String = String.Empty
        Dim strSQL As String = "@SELECT# DISTINCT sysrovwAllEmployeeGroups.* " &
                                    " FROM sysrovwAllEmployeeGroups INNER JOIN Employees ON sysrovwAllEmployeeGroups.IDEmployee = Employees.[ID] " &
                                    " INNER JOIN EmployeeContracts ON Employees.[ID] = EmployeeContracts.IDEmployee " &
                                    " INNER JOIN EmployeeUserFieldValues ON EmployeeUserFieldValues.[IDEmployee] = Employees.[ID] "

        'IMPORTANTE:El parametro strWhere se debe procesar adecuadamente. Según la forma Expresion1 + "#@#" +  Expresion2
        If strWhere <> String.Empty Then
            If strWhere.IndexOf("#@#") > 0 Then
                Dim strSplit As String() = {"#@#"}
                Dim strFilter = strWhere.Split(strSplit, StringSplitOptions.None)

                strDECLARE = strFilter(0)
                strSQL = strFilter(0) & " " & strSQL 'Contiene la expresion del declare para el parametro de funciones de la BBDD
                strWhere = strFilter(1) 'Contiene el filtro real
            End If
        End If

        Dim keywords As Generic.List(Of String) = roBusinessSupport.getKeywords()

        Dim strSQLWhere As String = ""

        Dim orFilters() As String = strAdvancedFilter.Split("}")

        For Each strFilter As String In orFilters
            If strFilter <> String.Empty Then
                strSQLWhere &= IIf(strSQLWhere <> String.Empty, " OR ", "") & roBusinessSupport.BuildFilter(strFilter.Substring(1), keywords)
            End If
        Next

        If strSQLWhere = String.Empty Then
            strSQLWhere = "1=2"
        End If

        If strWhere <> "" Then
            If strSQLWhere.Contains("OR") Then strSQLWhere = "(" & strSQLWhere & ")"

            strSQLWhere &= IIf(strSQLWhere <> "", " AND (" & strWhere & ") ", strWhere)
        End If

        If strSQLWhere <> "" Then strSQL &= "WHERE " & strSQLWhere

        Return roBusinessSupport.GetEmployeesSQL(strSQL, Feature, Type, " ORDER BY sysrovwAllEmployeeGroups.EmployeeName", "sysrovwAllEmployeeGroups.IDEmployee", _State, strDECLARE)

    End Function

    Public Shared Function BuildFilter(ByVal strAdvFilter As String, ByVal keywords As Generic.List(Of String)) As String
        Dim strFilter As String = ""

        Dim tempPatron As String = ""
        tempPatron = strAdvFilter

        For Each strKeyword As String In keywords
            Dim index As Integer = strAdvFilter.ToUpper.IndexOf(strKeyword & ":")
            If index > -1 Then
                tempPatron = Text.RegularExpressions.Regex.Replace(tempPatron, strKeyword & ":", "~" & strKeyword.ToUpper & "~", Text.RegularExpressions.RegexOptions.IgnoreCase)
            End If
        Next

        Dim paraValue As String = ""
        Dim asuntoValue As String = ""
        Dim jsonString As String = ""

        Dim ipos As Integer
        Dim keyValuesPairs() As String = tempPatron.Split("~")
        For Each sPas As String In keyValuesPairs

            If keywords.Contains(sPas.ToUpper) Then
                paraValue = keyValuesPairs(ipos + 1)
                If jsonString <> String.Empty Then jsonString &= ","
                jsonString &= "'" & sPas.ToUpper & "':'" + paraValue.Trim + "' "
            End If
            ipos = ipos + 1
        Next

        jsonString = "{" & jsonString & "}"

        Dim oDic As New Dictionary(Of String, String)
        oDic = VTBase.roJSONHelper.Deserialize(Of Dictionary(Of String, String))(jsonString)

        For Each strKey As String In oDic.Keys
            If oDic.Item(strKey).Trim <> String.Empty Then

                If strKey.ToUpper = "NAME" Or strKey.ToUpper = "NOMBRE" Or strKey.ToUpper = "NOM" Then
                    If strFilter <> String.Empty Then strFilter &= " AND "
                    strFilter &= "Employees.Name like '%" & oDic.Item(strKey) & "%'"
                ElseIf strKey.ToUpper = "CONTRACT" Or strKey.ToUpper = "CONTRATO" Or strKey.ToUpper = "CONTRACTE" Then
                    If strFilter <> String.Empty Then strFilter &= " AND "
                    strFilter &= "EmployeeContracts.IDContract like '%" & oDic.Item(strKey) & "%'"
                ElseIf strKey.ToUpper = "IDCARD" Or strKey.ToUpper = "DNI" Or strKey.ToUpper = "NIF" Then
                    If strFilter <> String.Empty Then strFilter &= " AND "
                    strFilter &= "EmployeeUserFieldValues.FieldName = 'NIF' AND EmployeeUserFieldValues.Value like '%" & oDic.Item(strKey) & "%'"
                ElseIf strKey.ToUpper.StartsWith("USR_") Then
                    If strFilter <> String.Empty Then strFilter &= " AND "
                    strFilter &= "EmployeeUserFieldValues.FieldName = '" & strKey.ToUpper.Replace("USR_", "") & "' AND EmployeeUserFieldValues.Value like '%" & oDic.Item(strKey) & "%'"
                ElseIf strKey.ToUpper = "COSTCENTER" Or strKey.ToUpper = "CENTROCOSTE" Or strKey.ToUpper = "CENTRECOST" Then
                    If strFilter <> String.Empty Then strFilter &= " AND "
                    strFilter &= "CostCenter like '%" & oDic.Item(strKey) & "%'"
                End If
            End If
        Next

        If strFilter <> String.Empty Then
            strFilter = "(" & strFilter & ")"
        End If

        Return strFilter
    End Function

    Public Shared Function getKeywords() As Generic.List(Of String)
        Dim keywords As New Generic.List(Of String)

        keywords.Add("NAME")
        keywords.Add("NOMBRE")
        keywords.Add("NOM")

        keywords.Add("CONTRACT")
        keywords.Add("CONTRATO")
        keywords.Add("CONTRACTE")

        keywords.Add("IDCARD")
        keywords.Add("DNI")
        keywords.Add("NIF")

        keywords.Add("COSTCENTER")
        keywords.Add("CENTROCOSTE")
        keywords.Add("CENTRECOST")

        Dim strSQL As String = "@SELECT# *, FieldName AS OriginalFieldName, 0 AS UsedInProcess FROM sysroUserFields " &
                                       "WHERE ISNULL(Type, 0) = 0 ORDER BY FieldName ASC"

        Dim dtUserFields As DataTable = CreateDataTable(strSQL)

        If dtUserFields IsNot Nothing AndAlso dtUserFields.Rows.Count > 0 Then
            For Each oRow As DataRow In dtUserFields.Rows
                keywords.Add("USR_" & roTypes.Any2String(oRow("OriginalFieldName")).ToUpper)
            Next
        End If

        Return keywords
    End Function

    Public Shared Function isKeyWord(ByVal strKeyword As String) As Boolean
        If strKeyword.ToUpper = "NAME" Or strKeyword.ToUpper = "NOMBRE" Or strKeyword.ToUpper = "NOM" Or
                   strKeyword.ToUpper = "CONTRACT" Or strKeyword.ToUpper = "CONTRATO" Or strKeyword.ToUpper = "CONTRACTE" Or
                   strKeyword.ToUpper = "IDCARD" Or strKeyword.ToUpper = "DNI" Or strKeyword.ToUpper = "NIF" Or strKeyword.ToUpper.StartsWith("USR_") Then
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' Devuelve un dataset con los datos de los empleados de la lista pasada por parámetro
    ''' La lista tiene que ser los codigos separados por comas: 1,4,5,14,23
    ''' Si la lista pasada no contiene ninguna coma se considera que se quiere hacer un where por un valor
    ''' </summary>
    ''' <param name="List"></param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetEmployeeFromList(ByVal List As String, ByRef oState As roBusinessState, ByVal Feature As String, ByVal Type As String, ByVal strWhere As String) As System.Data.DataSet
        Dim strQuery As String
        Dim oDataSet As System.Data.DataSet = Nothing

        Try

            strQuery = "@SELECT#  Employees.ID, Employees.Name, sysrovwAllEmployeeGroups.* From Employees "
            strQuery = strQuery & "Inner Join sysrovwAllEmployeeGroups "
            strQuery = strQuery & " ON Employees.ID = sysrovwAllEmployeeGroups.IDEmployee "
            If List <> "" Then
                strQuery = strQuery & " Where "
                If InStr(List, ",") > 0 Then
                    strQuery = strQuery & " ID IN (" & List & ")"
                Else
                    strQuery = strQuery & " ID  = " & List
                End If

                If strWhere <> String.Empty Then strQuery &= " AND " & strWhere

                If Feature <> String.Empty Then
                    strQuery = " @SELECT# tmp.* FROM (" & strQuery & ") tmp " & roSelector.GetEmployeePermissonInnerJoin(oState.IDPassport, Permission.Read, Feature, Type)
                    'strQuery &= " AND " & WLHelper.GetEmployeePermissonWhereClause(oState.IDPassport, Permission.Read, Feature, Type, "", "Employees.ID")
                    strQuery &= " ORDER BY tmp.Name"
                End If
            Else
                If Feature <> String.Empty Then
                    If (strWhere <> String.Empty) Then
                        strQuery &= " WHERE " & strWhere & " "
                    End If

                    strQuery = " @SELECT# tmp.* FROM (" & strQuery & ") tmp " & roSelector.GetEmployeePermissonInnerJoin(oState.IDPassport, Permission.Read, Feature, Type)
                    'strQuery &= WLHelper.GetEmployeePermissonWhereClause(oState.IDPassport, Permission.Read, Feature, Type, "", "Employees.ID")

                    strQuery &= " ORDER BY tmp.Name"
                Else
                    If strWhere <> String.Empty Then strQuery &= " WHERE " & strWhere
                End If

            End If

            oDataSet = CreateDataSet(strQuery)
        Catch ex As DbException
            oState.UpdateStateInfo(ex, "roBusinessSupport::GetEmployeeFromList")
        Catch ex As Exception
            oState.UpdateStateInfo(ex, "roBusinessSupport::GetEmployeeFromList")
        End Try

        Return oDataSet

    End Function

    Public Shared Function GetEmployeesFromListWithType(ByVal strListIDs As String, ByVal strFeature As String, ByVal strType As String, ByVal oState As roBusinessState) As DataTable

        Dim tbRet As DataTable = Nothing

        Try

            Dim ds As DataSet = roBusinessSupport.GetEmployeeFromList(strListIDs, oState, strFeature, strType, "")
            If ds IsNot Nothing AndAlso ds.Tables.Count > 0 Then
                tbRet = ds.Tables(0)
            End If
        Catch ex As DbException
            oState.UpdateStateInfo(ex, "roBusinessSupport::GetEmployeeFromListWithType")
        Catch ex As Exception
            oState.UpdateStateInfo(ex, "roBusinessSupport::GetEmployeeFromListWithType")
        End Try

        Return tbRet

    End Function

    Public Shared Function CheckIfEmployeeHasData(ByVal IDEmployee As Integer, ByRef oState As roBusinessState) As Boolean
        ' Borra los datos de un empleado
        Dim bolRet As Boolean = False
        oState.UpdateStateInfo()

        Try

            Dim strSQL As String = "@SELECT# count(Id) FROM Punches WHERE IDEmployee =" & IDEmployee

            bolRet = If(roTypes.Any2Integer(ExecuteScalar(strSQL)) > 0, True, False)
        Catch ex As Exception
            oState.UpdateStateInfo(ex, "roBusinessSupport::CheckIfEmployeeHasData")
            bolRet = False
        End Try

        Return bolRet

    End Function

    Public Shared Function GetActiveEmployeesCount(ByVal xDate As Date, ByVal oState As roBusinessState) As Integer

        Dim intRet As Integer = 0

        Try

            Dim strSQL As String
            strSQL = "@SELECT# COUNT(IDEmployee) AS Total From EmployeeContracts " &
                         "WHERE EndDate > " & Any2Time(DateTime2Date(xDate.Date)).SQLSmallDateTime & " AND " &
                               "BeginDate <= " & Any2Time(DateTime2Date(xDate.Date)).SQLSmallDateTime

            Dim tb As DataTable = CreateDataTable(strSQL, )
            If tb.Rows.Count = 1 Then
                If Not IsDBNull(tb.Rows(0).Item(0)) Then intRet = tb.Rows(0).Item(0)
            End If
        Catch ex As DbException
            oState.UpdateStateInfo(ex, "roBusinessSupport::GetActiveEmployeesCount")
        Catch ex As Exception
            oState.UpdateStateInfo(ex, "roBusinessSupport::GetActiveEmployeesCount")
        Finally

        End Try

        Return intRet

    End Function

    Public Shared Function GetActiveEmployeesTaskCount(ByVal xDate As Date, ByVal oState As roBusinessState) As Integer

        Dim intRet As Integer = 0

        Try

            Dim strSQL As String
            strSQL = "@SELECT# COUNT(EmployeeContracts.IDEmployee) AS Total FROM EmployeeContracts INNER JOIN Employees ON EmployeeContracts.IDEmployee = Employees.ID " &
                         "WHERE (EmployeeContracts.EndDate > " & Any2Time(DateTime2Date(xDate.Date)).SQLSmallDateTime & ") AND " &
                               "(EmployeeContracts.BeginDate <= " & Any2Time(DateTime2Date(xDate.Date)).SQLSmallDateTime & ") AND (Employees.Type = 'J')"

            Dim tb As DataTable = CreateDataTable(strSQL, )
            If tb.Rows.Count = 1 Then
                If Not IsDBNull(tb.Rows(0).Item(0)) Then intRet = tb.Rows(0).Item(0)
            End If
        Catch ex As DbException
            oState.UpdateStateInfo(ex, "roBusinessSupport::GetActiveEmployeesCount")
        Catch ex As Exception
            oState.UpdateStateInfo(ex, "roBusinessSupport::GetActiveEmployeesCount")
        Finally

        End Try

        Return intRet

    End Function

    Public Shared Function GetEmployeesByName(ByVal strLikeName As String, ByVal strWhere As String, ByVal Feature As String, ByVal Type As String, ByVal _State As roBusinessState) As DataTable

        Dim strSQL As String = "@SELECT# sysrovwAllEmployeeGroups.* FROM sysrovwAllEmployeeGroups INNER JOIN Employees ON sysrovwAllEmployeeGroups.IDEmployee = Employees.[ID] "
        Dim strDECLARE As String = String.Empty
        'IMPORTANTE:El parametro strWhere se debe procesar adecuadamente. Según la forma Expresion1 + "#@#" +  Expresion2
        If strWhere <> String.Empty Then
            If strWhere.IndexOf("#@#") > 0 Then
                Dim strSplit As String() = {"#@#"}
                Dim strFilter = strWhere.Split(strSplit, StringSplitOptions.None)

                strDECLARE = strFilter(0)
                strSQL = strFilter(0) & " " & strSQL 'Contiene la expresion del declare para el parametro de funciones de la BBDD
                strWhere = strFilter(1) 'Contiene el filtro real
            End If
        End If

        strLikeName = strLikeName.Replace("?", "%").Replace("'", "").Replace("*", "%").Trim()
        If Not strLikeName.StartsWith("%") Then strLikeName = "%" & strLikeName
        If Not strLikeName.EndsWith("%") Then strLikeName = strLikeName & "%"

        Dim strSQLWhere As String = ""
        If strLikeName <> "" Then strSQLWhere &= "Employees.Name LIKE '" & strLikeName & "' "
        If strWhere <> "" Then strSQLWhere &= " AND (" & strWhere & ") "
        If strSQLWhere <> "" Then strSQL &= "WHERE " & strSQLWhere

        Return roBusinessSupport.GetEmployeesSQL(strSQL, Feature, Type, "ORDER BY EmployeeName", "Employees.ID", _State, strDECLARE)

    End Function

    Public Shared Function GetEmployeesByIDContract(ByVal strLikeIDContract As String, ByVal strWhere As String, ByVal Feature As String, ByVal Type As String, ByVal _State As roBusinessState) As DataTable

        Dim strSQL As String = "@SELECT# sysrovwAllEmployeeGroups.*, EmployeeContracts.IDContract FROM sysrovwAllEmployeeGroups INNER JOIN Employees " &
                                   "ON sysrovwAllEmployeeGroups.IDEmployee = Employees.[ID] INNER JOIN EmployeeContracts " &
                                   "ON sysrovwAllEmployeeGroups.IDEmployee = EmployeeContracts.IDEmployee "
        Dim strDECLARE As String = String.Empty
        'IMPORTANTE:El parametro strWhere se debe procesar adecuadamente. Según la forma Expresion1 + "#@#" +  Expresion2
        If strWhere <> String.Empty Then
            If strWhere.IndexOf("#@#") > 0 Then
                Dim strSplit As String() = {"#@#"}
                Dim strFilter = strWhere.Split(strSplit, StringSplitOptions.None)

                strDECLARE = strFilter(0)
                strSQL = strFilter(0) & " " & strSQL 'Contiene la expresion del declare para el parametro de funciones de la BBDD
                strWhere = strFilter(1) 'Contiene el filtro real
            End If
        End If

        strLikeIDContract = strLikeIDContract.Replace("?", "%").Replace("'", "").Replace("*", "%").Trim()
        If Not strLikeIDContract.StartsWith("%") Then strLikeIDContract = "%" & strLikeIDContract
        If Not strLikeIDContract.EndsWith("%") Then strLikeIDContract = strLikeIDContract & "%"

        Dim strSQLWhere As String = ""
        If strLikeIDContract <> "" Then strSQLWhere &= "EmployeeContracts.IDContract LIKE '" & strLikeIDContract & "' "
        If strWhere <> "" Then strSQLWhere &= " AND (" & strWhere & ") "
        If strSQLWhere <> "" Then strSQL &= "WHERE " & strSQLWhere

        Return roBusinessSupport.GetEmployeesSQL(strSQL, Feature, Type, "ORDER BY EmployeeContracts.IDContract", "sysrovwAllEmployeeGroups.IDEmployee", _State, strDECLARE)

    End Function

    Public Shared Function GetEmployeesOnAccessGroup(ByVal _IDAccessGroup As Nullable(Of Integer), ByVal strWhere As String, ByVal Feature As String, ByVal Type As String, ByVal _State As roBusinessState) As DataTable

        Dim strSQL1 As String
        strSQL1 = "@SELECT# sysrovwAllEmployeeGroups.* " &
                     "FROM sysrovwAllEmployeeGroups INNER JOIN Employees " &
                            "ON sysrovwAllEmployeeGroups.IDEmployee = Employees.[ID] "
        Dim strSQLWhere As String = ""
        If _IDAccessGroup.HasValue Then strSQLWhere &= "Employees.IDAccessGroup = " & _IDAccessGroup.Value & " "
        If strWhere <> "" Then strSQLWhere &= " AND (" & strWhere & ") "
        If strSQLWhere <> "" Then strSQL1 &= "WHERE " & strSQLWhere
        'strSQL1 &= "ORDER BY Employees.Name"
        Dim strSQL2 As String
        strSQL2 = " UNION @SELECT# sysrovwAllEmployeeGroups.* FROM sysrovwAllEmployeeGroups " &
                      "INNER JOIN EmployeeAccessAuthorization ON sysrovwAllEmployeeGroups.IDEmployee = EmployeeAccessAuthorization.IDEmployee " &
                      "WHERE EmployeeAccessAuthorization.IDAuthorization = " & _IDAccessGroup.Value
        Return roBusinessSupport.GetEmployeesSQL(strSQL1 & strSQL2, Feature, Type, " ORDER BY sysrovwAllEmployeeGroups.EmployeeName", "sysrovwAllEmployeeGroups.IDEmployee", _State)
    End Function

    Public Shared Function GetGroupsOnAccessGroup(ByVal _IDAccessGroup As Nullable(Of Integer), ByVal strWhere As String, ByVal Feature As String, ByVal Type As String, ByVal _State As roBusinessState) As DataTable

        Dim oRet As DataTable = Nothing
        Try
            Dim strSQL As String = " @SELECT# sysrovwAllEmployeeGroups.* FROM sysrovwAllEmployeeGroups " &
                      " INNER JOIN EmployeeAccessAuthorization ON sysrovwAllEmployeeGroups.IDEmployee = EmployeeAccessAuthorization.IDEmployee " &
                      " INNER JOIN sysrovwSecurity_PermissionOverGroups pog ON pog.IdPassport = " & _State.IDPassport & " AND pog.IdGroup = sysrovwAllEmployeeGroups.IDGroup " &
                      "            AND DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0) between pog.BeginDate and pog.EndDate" &
                      " INNER JOIN sysrovwSecurity_PermissionOverFeatures pof on pof.IDPassport = " & _State.IDPassport & " AND pof.FeatureAlias = '" & Feature & "' AND pof.FeatureType = '" & Type & "' AND pof.Permission > 0 " &
                      " WHERE EmployeeAccessAuthorization.IDAuthorization = " & _IDAccessGroup.Value


            oRet = CreateDataTable($"{strSQL} ORDER BY sysrovwAllEmployeeGroups.EmployeeName", )

            ' Configuro la tabla de resultado
            Dim oDataColumn As New DataColumn
            oDataColumn.ColumnName = "Type"
            oRet.Columns.Add(oDataColumn)

            oRet.AcceptChanges()
        Catch ex As DbException
            _State.UpdateStateInfo(ex, "roBusinessSupport::GetGroupsSQL")
        Catch ex As Exception
            _State.UpdateStateInfo(ex, "roBusinessSupport::GetGroupsSQL")
        End Try

        Return oRet
    End Function

    Public Shared Function GetEmployeesByIDCard(ByVal strLikeIDCard As String, ByVal strWhere As String, ByVal Feature As String, ByVal Type As String, ByVal _State As roBusinessState) As DataTable

        'Dim Value As Object
        'Dim CardString As String

        strLikeIDCard = strLikeIDCard.Replace("'", "%").Replace("%", "")

        If Not IsNumeric(strLikeIDCard) Then
            strLikeIDCard = "-1"
        End If

        ' Si la tarjeta tiene un alias, obtiene ahora
        'Dim strCard As String = strLikeIDCard
        'Value = ExecuteScalar("@SELECT# IDCard FROM CardAliases WHERE RealValue LIKE '%" & CLng(strCard) & "'")
        'If Value IsNot Nothing Then
        '    CardString = CStr(Value)
        'Else
        '    CardString = strLikeIDCard
        'End If

        Dim strSQL As String
        strSQL = "@SELECT# sysrovwAllEmployeeGroups.*, sysroPassports_AuthenticationMethods.Credential AS IDCard " &
                     "FROM sysrovwAllEmployeeGroups INNER JOIN Employees " &
                            "ON sysrovwAllEmployeeGroups.IDEmployee = Employees.[ID] " &
                            "INNER JOIN sysroPassports " &
                            "ON sysrovwAllEmployeeGroups.IDEmployee = sysroPassports.IDEmployee " &
                            "INNER JOIN sysroPassports_AuthenticationMethods " &
                            "ON sysroPassports.ID = sysroPassports_AuthenticationMethods.IDPassport " &
                     "WHERE sysroPassports_AuthenticationMethods.Method = " & AuthenticationMethod.Card & " AND " &
                           "sysroPassports_AuthenticationMethods.Version = '' AND " &
                           "sysroPassports_AuthenticationMethods.BiometricID = 0 "
        Dim strDECLARE As String = String.Empty
        'IMPORTANTE:El parametro strWhere se debe procesar adecuadamente. Según la forma Expresion1 + "#@#" +  Expresion2
        If strWhere <> String.Empty Then
            If strWhere.IndexOf("#@#") > 0 Then
                Dim strSplit As String() = {"#@#"}
                Dim strFilter = strWhere.Split(strSplit, StringSplitOptions.None)

                strDECLARE = strFilter(0)
                strSQL = strFilter(0) & " " & strSQL 'Contiene la expresion del declare para el parametro de funciones de la BBDD
                strWhere = strFilter(1) 'Contiene el filtro real
            End If
        End If

        Dim strSQLWhere As String = ""
        If strLikeIDCard <> "" Then strSQL &= " AND sysroPassports_AuthenticationMethods.Credential LIKE '%" & strLikeIDCard & "'"
        If strWhere <> "" Then strSQL &= " AND (" & strWhere & ") "

        Return roBusinessSupport.GetEmployeesSQL(strSQL, Feature, Type, "ORDER BY IDCard", "sysrovwAllEmployeeGroups.IDEmployee", _State, strDECLARE)

    End Function

    ''' <summary>
    ''' Recupera el ID del empleado correspondiente a la tarjeta informada, cuando se guarda en bbdd sin codiicación.
    ''' Tiene en cuenta que la tarjeta venga de un terminal que entrega un código con menos bytes de los que entregaba el terminal en el que se registró la tarjeta
    ''' </summary>
    ''' <param name="CardID"></param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    Public Shared Function GetEmployeeIDFromCardIDHex(ByVal CardID As String, ByRef oState As roBusinessState) As Long
        Dim sSQL As String
        Dim Value As Object
        Dim aDate As New roTime
        Dim CardString As String
        Dim lngRet As Long = -1

        Try
            If CardID <> "0" Then
                ' Si la tarjeta tiene un alias, obtiene ahora
                aDate.Value = Now.Date
                Dim strCard As String = CardID
                Dim bolCardAliases As Boolean
                If strCard.Length.ToString Mod 2 > 0 Then strCard = strCard.PadLeft(strCard.Length.ToString + (2 - (strCard.Length.ToString Mod 2)), "0")
                Value = ExecuteScalar("@SELECT# IDCard FROM CardAliases WHERE dbo.fn_HexToIntnt(right(dbo.fn_decToBase(RealValue,16)," + strCard.Length.ToString + ")) = dbo.fn_HexToIntnt('" & strCard & "')")
                If Value IsNot Nothing Then
                    CardString = CStr(Value)
                    bolCardAliases = True
                Else
                    CardString = CardID
                    bolCardAliases = False
                End If

                sSQL = "@SELECT# TOP 1 Employees.ID " &
                             "FROM Employees " &
                                    "INNER JOIN sysroPassports " &
                                    "ON Employees.ID = sysroPassports.IDEmployee " &
                                    "INNER JOIN sysroPassports_AuthenticationMethods " &
                                    "ON sysroPassports.ID = sysroPassports_AuthenticationMethods.IDPassport " &
                                    "INNER JOIN EmployeeContracts ON Employees.ID = EmployeeContracts.IDEmployee " &
                             "WHERE sysroPassports_AuthenticationMethods.Method = " & AuthenticationMethod.Card & " AND " &
                                   "sysroPassports_AuthenticationMethods.Version = '' AND " &
                                   "sysroPassports_AuthenticationMethods.Enabled = 1 AND " &
                                   aDate.SQLDateTime & " >= BeginDate And EndDate >= " & aDate.SQLDateTime & " AND " &
                                   "sysroPassports_AuthenticationMethods.BiometricID = 0 AND "
                If bolCardAliases Then
                    sSQL &= "sysroPassports_AuthenticationMethods.Credential = '" & CardString & "'"
                Else
                    sSQL &= "dbo.fn_HexToIntnt(right(dbo.fn_decToBase(sysroPassports_AuthenticationMethods.Credential,16)," + strCard.Length.ToString + "))= dbo.fn_HexToIntnt('" & strCard & "')"
                End If
                sSQL &= " ORDER BY LEN(sysroPassports_AuthenticationMethods.Credential) ASC"
                lngRet = Any2Double(ExecuteScalar(sSQL))

                If lngRet = 0 Then
                    sSQL = "@SELECT# TOP 1 Employees.ID " &
                                 "FROM Employees " &
                                        "INNER JOIN sysroPassports " &
                                        "ON Employees.ID = sysroPassports.IDEmployee " &
                                        "INNER JOIN sysroPassports_AuthenticationMethods " &
                                        "ON sysroPassports.ID = sysroPassports_AuthenticationMethods.IDPassport " &
                                        "INNER JOIN EmployeeContracts ON Employees.ID = EmployeeContracts.IDEmployee " &
                                 "WHERE sysroPassports_AuthenticationMethods.Method = " & AuthenticationMethod.Card & " AND " &
                                       "sysroPassports_AuthenticationMethods.Version = '' AND " &
                                       "sysroPassports_AuthenticationMethods.Enabled = 1  AND " &
                                       "sysroPassports_AuthenticationMethods.BiometricID = 0 AND " &
                                       aDate.SQLDateTime & " >= BeginDate And EndDate >= " & aDate.SQLDateTime & " AND " &
                                       "dbo.fn_HexToIntnt(right(dbo.fn_decToBase(sysroPassports_AuthenticationMethods.Credential,16)," + strCard.Length.ToString + ")) = dbo.fn_HexToIntnt('" & strCard & "')" &
                                       " ORDER BY LEN(sysroPassports_AuthenticationMethods.Credential) ASC"
                    lngRet = Any2Double(ExecuteScalar(sSQL))
                End If
            End If
        Catch ex As DbException
            oState.UpdateStateInfo(ex, "roBusinessSupport::GetEmployeeIDFromCardIDHex")
        Catch ex As Exception
            oState.UpdateStateInfo(ex, "roBusinessSupport::GetEmployeeIDFromCardIDHex")
        End Try

        Return lngRet
    End Function

    ''' <summary>
    ''' Recupera el ID del empleado correspondiente a la tarjeta informada. No hace ninguna conversión del código de tarjeta.
    ''' Tiene en cuenta la longitud del código para determinar si tiene que comparar el valor con la base de datos utilizando un LIKE o no.
    ''' </summary>
    ''' <param name="CardID"></param>
    ''' <returns>El código del empleado. Si no existe, devuelve -1.</returns>
    ''' <remarks></remarks>
    Public Shared Function GetEmployeeIDFromCardID(ByVal CardID As String, oState As roBusinessState) As Long

        Dim lngRet As Long = -1
        Dim sSQL As String
        Dim aDate As New roTime
        Dim Value As Object
        Dim CardString As String

        Try
            aDate.Value = Now.Date

            If CardID <> "0" And CardID.Length > 0 Then

                If Not IsNumeric(CardID) Then
                    CardID = Convert.ToUInt64(CardID, 16)
                End If

                ' Si la tarjeta tiene un alias, obtiene ahora
                Dim strCard As String = CardID
                Dim bolCardAliases As Boolean
                Value = ExecuteScalar("@SELECT# IDCard FROM CardAliases WHERE RealValue LIKE '%" & strCard & "'")
                If Value IsNot Nothing Then
                    CardString = CStr(Value)
                    bolCardAliases = True
                Else
                    CardString = CardID
                    bolCardAliases = False
                End If

                sSQL = "@SELECT# TOP 1 Employees.ID " &
                         "FROM Employees " &
                                "INNER JOIN sysroPassports " &
                                "ON Employees.ID = sysroPassports.IDEmployee " &
                                "INNER JOIN sysroPassports_AuthenticationMethods " &
                                "ON sysroPassports.ID = sysroPassports_AuthenticationMethods.IDPassport " &
                                "INNER JOIN EmployeeContracts ON Employees.ID = EmployeeContracts.IDEmployee " &
                         "WHERE sysroPassports_AuthenticationMethods.Method = " & AuthenticationMethod.Card & " AND " &
                               "sysroPassports_AuthenticationMethods.Version = '' AND " &
                               "sysroPassports_AuthenticationMethods.Enabled = 1 AND " &
                               aDate.SQLDateTime & " >= BeginDate And EndDate >= " & aDate.SQLDateTime & " AND " &
                               "sysroPassports_AuthenticationMethods.BiometricID = 0 AND "
                If bolCardAliases Then
                    sSQL &= "sysroPassports_AuthenticationMethods.Credential = '" & CardString & "'"
                Else
                    sSQL &= "sysroPassports_AuthenticationMethods.Credential LIKE '%" & CLng(strCard) & "'"
                End If
                sSQL &= " ORDER BY LEN(sysroPassports_AuthenticationMethods.Credential) ASC"
                lngRet = Any2Double(ExecuteScalar(sSQL))

                If lngRet = 0 Then
                    sSQL = "@SELECT# TOP 1 Employees.ID " &
                                 "FROM Employees " &
                                        "INNER JOIN sysroPassports " &
                                        "ON Employees.ID = sysroPassports.IDEmployee " &
                                        "INNER JOIN sysroPassports_AuthenticationMethods " &
                                        "ON sysroPassports.ID = sysroPassports_AuthenticationMethods.IDPassport " &
                                        "INNER JOIN EmployeeContracts ON Employees.ID = EmployeeContracts.IDEmployee " &
                                 "WHERE sysroPassports_AuthenticationMethods.Method = " & AuthenticationMethod.Card & " AND " &
                                       "sysroPassports_AuthenticationMethods.Version = '' AND " &
                                       "sysroPassports_AuthenticationMethods.Enabled = 1 AND " &
                                       "sysroPassports_AuthenticationMethods.BiometricID = 0 AND " &
                                       aDate.SQLDateTime & " >= BeginDate And EndDate >= " & aDate.SQLDateTime & " AND " &
                                       "sysroPassports_AuthenticationMethods.Credential LIKE '%" & CLng(strCard) & "'" &
                                       " ORDER BY LEN(sysroPassports_AuthenticationMethods.Credential) ASC"
                    lngRet = Any2Double(ExecuteScalar(sSQL))
                End If
            End If
        Catch ex As DbException
            oState.UpdateStateInfo(ex, "roBusinessSupport::GetEmployeeIDFromCardIDV1")
        Catch ex As Exception
            oState.UpdateStateInfo(ex, "roBusinessSupport::GetEmployeeIDFromCardIDV1")
        End Try

        Return lngRet

    End Function

    Public Shared Function GetEmployeesByIDForPIN(ByVal iIDEmployee As Integer, ByVal Feature As String, ByVal Type As String, ByVal _State As roBusinessState) As DataTable

        Dim strSQL As String
        strSQL = "@SELECT# sysrovwAllEmployeeGroups.IDEmployee, sysrovwAllEmployeeGroups.CurrentEmployee, sysrovwAllEmployeeGroups.BeginDate, sysrovwAllEmployeeGroups.EndDate, sysroPassports_AuthenticationMethods.Credential AS IDCard, " &
                            " sysroPassports_AuthenticationMethods.Password AS PinNumber, sysroPassports_AuthenticationMethods.Enabled " &
                     "FROM sysrovwAllEmployeeGroups INNER JOIN Employees " &
                            "ON sysrovwAllEmployeeGroups.IDEmployee = Employees.[ID] " &
                            "INNER JOIN sysroPassports " &
                            "ON sysrovwAllEmployeeGroups.IDEmployee = sysroPassports.IDEmployee " &
                            "INNER JOIN sysroPassports_AuthenticationMethods " &
                            "ON sysroPassports.ID = sysroPassports_AuthenticationMethods.IDPassport " &
                     "WHERE sysroPassports_AuthenticationMethods.Method = " & AuthenticationMethod.Pin & " AND " &
                           "sysroPassports_AuthenticationMethods.Version = '' AND " &
                           "sysroPassports_AuthenticationMethods.BiometricID = 0 AND " &
                           "sysrovwAllEmployeeGroups.IDEmployee = " & iIDEmployee.ToString

        Return roBusinessSupport.GetEmployeesSQL(strSQL, Feature, Type, "ORDER BY IDCard", "sysrovwAllEmployeeGroups.IDEmployee", _State)

    End Function

    Public Shared Function GetIDEmployeeByNFC(ByVal NFCCredential As String, ByVal Feature As String, ByVal Type As String, ByVal _State As roBusinessState) As DataTable

        Dim strSQL As String
        strSQL = "@SELECT# sysrovwAllEmployeeGroups.*, sysroPassports_AuthenticationMethods.Credential AS NFCcode " &
                     "FROM sysrovwAllEmployeeGroups INNER JOIN Employees " &
                            "ON sysrovwAllEmployeeGroups.IDEmployee = Employees.[ID] " &
                            "INNER JOIN sysroPassports " &
                            "ON sysrovwAllEmployeeGroups.IDEmployee = sysroPassports.IDEmployee " &
                            "INNER JOIN sysroPassports_AuthenticationMethods " &
                            "ON sysroPassports.ID = sysroPassports_AuthenticationMethods.IDPassport " &
                     "WHERE sysroPassports_AuthenticationMethods.Method = " & AuthenticationMethod.NFC & " AND " &
                           " Credential = '" & NFCCredential & "' AND sysroPassports_AuthenticationMethods.Version = '' "

        Return roBusinessSupport.GetEmployeesSQL(strSQL, Feature, Type, "ORDER BY NFCcode", "sysrovwAllEmployeeGroups.IDEmployee", _State)

    End Function

    Public Shared Function GetEmployeesByPlate(ByVal strLikePlate As String, ByVal strWhere As String, ByVal Feature As String, ByVal Type As String,
                                            ByVal _State As roBusinessState) As DataTable

        Dim strSQL As String
        strSQL = "@SELECT# sysrovwAllEmployeeGroups.*, sysroPassports_AuthenticationMethods.Credential AS Plate " &
                     "FROM sysrovwAllEmployeeGroups INNER JOIN Employees " &
                            "ON sysrovwAllEmployeeGroups.IDEmployee = Employees.[ID] " &
                            "INNER JOIN sysroPassports " &
                            "ON sysrovwAllEmployeeGroups.IDEmployee = sysroPassports.IDEmployee " &
                            "INNER JOIN sysroPassports_AuthenticationMethods " &
                            "ON sysroPassports.ID = sysroPassports_AuthenticationMethods.IDPassport " &
                     "WHERE sysroPassports_AuthenticationMethods.Method = " & AuthenticationMethod.Plate
        Dim strDECLARE As String = String.Empty
        'IMPORTANTE:El parametro strWhere se debe procesar adecuadamente. Según la forma Expresion1 + "#@#" +  Expresion2
        If strWhere <> String.Empty Then
            If strWhere.IndexOf("#@#") > 0 Then
                Dim strSplit As String() = {"#@#"}
                Dim strFilter = strWhere.Split(strSplit, StringSplitOptions.None)

                strDECLARE = strFilter(0)
                strSQL = strFilter(0) & " " & strSQL 'Contiene la expresion del declare para el parametro de funciones de la BBDD
                strWhere = strFilter(1) 'Contiene el filtro real
            End If
        End If

        strLikePlate = strLikePlate.Replace("?", "%").Replace("'", "").Replace("*", "%").Trim()
        If Not strLikePlate.StartsWith("%") Then strLikePlate = "%" & strLikePlate
        If Not strLikePlate.EndsWith("%") Then strLikePlate = strLikePlate & "%"

        Dim strSQLWhere As String = ""
        If strLikePlate <> "" Then strSQL &= " AND sysroPassports_AuthenticationMethods.Credential LIKE '" & strLikePlate & "' "
        If strWhere <> "" Then strSQL &= " AND (" & strWhere & ") "

        Return roBusinessSupport.GetEmployeesSQL(strSQL, Feature, Type, "ORDER BY Plate", "sysrovwAllEmployeeGroups.IDEmployee", _State, strDECLARE)

    End Function

    Public Shared Function GetEmployeesByID(ByVal IdEmployee As Integer, ByVal Feature As String, ByVal Type As String, ByVal _State As roBusinessState) As DataTable

        Dim strSQL As String = "@SELECT# sysrovwAllEmployeeGroups.* FROM sysrovwAllEmployeeGroups WHERE sysrovwAllEmployeeGroups.IdEmployee = " & IdEmployee

        Return roBusinessSupport.GetEmployeesSQL(strSQL, Feature, Type, "", "sysrovwAllEmployeeGroups.IDEmployee", _State)

    End Function

    Public Shared Function GetEmployeesSQL(ByVal strSQL As String, ByVal Feature As String, ByVal Type As String, ByVal strOrderBy As String, ByVal strEmployeeField As String, ByVal _State As roBusinessState,
                                          Optional strDECLARE As String = "") As DataTable

        Dim oRet As DataTable = Nothing

        Try

            Dim strPermissionFilter As String = String.Empty

            'NECESITA COMPROBAR PERMISOS DE FEATURE
            If Feature <> String.Empty Then

                If strDECLARE.Length > 0 AndAlso strSQL.Contains(strDECLARE) Then
                    strSQL = strSQL.Replace(strDECLARE, "")
                End If

                Dim strSQLHeader = strDECLARE & vbCrLf & "@SELECT# * from ("

                strSQL = strSQLHeader & strSQL & ") tmp "

                strPermissionFilter = roSelector.GetEmployeePermissonInnerJoin(_State.IDPassport, Permission.Read, Feature, Type)

                strSQL &= " " & strPermissionFilter

                Dim sAuxOrderBy As String
                If strOrderBy.Length > 0 Then
                    sAuxOrderBy = strOrderBy
                    If strOrderBy.Split(".").Length > 1 Then
                        sAuxOrderBy = " ORDER BY tmp." & strOrderBy.Split(".")(1)
                    End If
                    strSQL &= " " & sAuxOrderBy
                End If
            Else
                strSQL &= " " & strOrderBy
            End If

            'Se elimina de la tabla sysrovwAllEmployeeGroups el campo de CostCenter, por lo que si es necesario se usa otra vista que lo incluye.
            If strSQL.ToUpper.IndexOf("COSTCENTER LIKE") > 0 Then
                strSQL = strSQL.Replace("sysrovwAllEmployeeGroups.", "sysrovwAllEmployeeGroupsWithCostCenter.")
                strSQL = strSQL.Replace(" sysrovwAllEmployeeGroups ", " sysrovwAllEmployeeGroupsWithCostCenter ")
            End If

            If strPermissionFilter.Length > 0 Then
                strSQL += " " & SQLServerHint.GetSQLHint(SQLServerHint.SelectHinted.GetEmployeesSQLWithPermissions)
            Else
                strSQL += " " & SQLServerHint.GetSQLHint(SQLServerHint.SelectHinted.GetEmployeesSQLWithoutPermissions)
            End If


            oRet = CreateDataTable(strSQL, )

            ' Configuro la tabla de resultado
            Dim oDataColumn As New DataColumn
            oDataColumn.ColumnName = "Type"
            oRet.Columns.Add(oDataColumn)

            For Each oDataRow As DataRow In oRet.Rows
                If oDataRow("CurrentEmployee") = 0 And oDataRow("Begindate") >= Now Then
                    ' El empleado es una futura incorporación
                    oDataRow("Type") = 4
                Else
                    If oDataRow("CurrentEmployee") = 0 And oDataRow("Begindate") < Now Then
                        ' El empleado es una baja
                        oDataRow("Type") = 3
                    Else
                        If oDataRow("CurrentEmployee") = 1 And oDataRow("Enddate") <> CDate("01/01/2079") Then
                            ' Empleado con movilidad
                            oDataRow("Type") = 2
                        Else
                            ' Empleado normal
                            oDataRow("Type") = 1
                        End If
                    End If
                End If
            Next

            oRet.AcceptChanges()
        Catch ex As DbException
            _State.UpdateStateInfo(ex, "roBusinessSupport::GetEmployeesSQL")
        Catch ex As Exception
            _State.UpdateStateInfo(ex, "roBusinessSupport::GetEmployeesSQL")
        End Try

        Return oRet

    End Function


    Public Shared Function GetEmployeesException(ByVal IDApplication As Integer, ByVal _State As roBusinessState) As System.Data.DataTable
        Dim oRet As DataTable = Nothing

        Try
            Dim strQuery As String = "@SELECT# IDEmployee, Permission FROM sysroPassports_Employees WHERE IDPassport = " & _State.IDPassport.ToString

            oRet = CreateDataTable(strQuery, )

        Catch ex As DbException
            _State.UpdateStateInfo(ex, "roBusinessSupport::GetEmployeesException")
        Catch ex As Exception
            _State.UpdateStateInfo(ex, "roBusinessSupport::GetEmployeesException")
        End Try

        Return oRet

    End Function

#End Region

#Region "Common Info"

    Public Shared Function GetEmployeeLockDatetoApply(ByVal IDEmployee As Integer, ByRef EmployeeLockDateType As Boolean, ByRef oState As roBusinessState, Optional ByVal bolAudit As Boolean = False) As Date
        '
        ' Obtenemos la fecha que actualmente esta utilizando el empleado como fecha de bloqueo e indicamos si la fecha esta asignada al empleado o es global
        '
        Dim oRet As Date = New Date(1900, 1, 1)
        Dim tb As DataTable = Nothing
        Try
            EmployeeLockDateType = False ' Por defecto fecha de bloqueo global
            Dim strSQL As String = "@SELECT# * from sysrovwEmployeeLockDate WITH(NOLOCK) where IDEmployee=" & IDEmployee.ToString
            tb = CreateDataTable(strSQL, )
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                If IsDate(Any2String(tb.Rows(0)("LockDate"))) Then oRet = Any2Time(tb.Rows(0)("LockDate")).Value
                If Any2Integer(tb.Rows(0)("EmployeeLockDateType")) = 1 Then EmployeeLockDateType = True
            End If

            If bolAudit Then
                Dim EmployeeName As String = GetEmployeeName(IDEmployee, oState)
                Dim tbParameters As DataTable = oState.CreateAuditParameters()
                Dim strObjectName As String = EmployeeName & " --> "
                strObjectName += oRet.ToShortDateString & " EmployeeType:" & EmployeeLockDateType.ToString

                oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tLockDate, strObjectName, tbParameters, -1)
            End If
        Catch ex As DbException
            If oState IsNot Nothing Then oState.UpdateStateInfo(ex, "roBusinessSupport::GetEmployeeLockDatetoApply")
        Catch ex As Exception
            If Not oState Is Nothing Then oState.UpdateStateInfo(ex, "roBusinessSupport::GetEmployeeLockDatetoApply")
        Finally

        End Try

        Return oRet

    End Function

    Public Shared Function SaveEmployeeLockDate(ByVal IDEmployee As Integer, ByVal LockDate As Date, ByVal EmployeeLockDateType As Boolean, ByRef oState As roBusinessState, Optional ByVal bolAudit As Boolean = False, Optional ByVal bolInitTask As Boolean = False) As Boolean
        '
        ' Guardamos la fecha de bloqueo del empleado en caso necesario, o dejamos la fecha global por defecto
        '
        Dim bolRet As Boolean = False
        Dim strLockDate As String

        Try
            Dim strSQL As String = "@UPDATE# Employees SET LockDate=@DateParam WHERE  ID=" & IDEmployee.ToString

            If EmployeeLockDateType Then
                strLockDate = Any2Time(LockDate).SQLSmallDateTime
            Else
                ' Quitamos la fecha de bloqueo asignada, para que pase a utilizar la fecha de bloqueo global
                strLockDate = "Null"
            End If
            strSQL = strSQL.Replace("@DateParam", strLockDate)

            ' Asignamos al empleado la fecha de bloqueo indicada
            bolRet = ExecuteSqlWithoutTimeOut(strSQL)

            If bolAudit AndAlso bolRet Then
                Dim EmployeeName As String = GetEmployeeName(IDEmployee, oState)
                Dim tbParameters As DataTable = oState.CreateAuditParameters()
                Dim strObjectName As String = EmployeeName & " --> "
                If strLockDate.ToUpper = "NULL" Then
                    strObjectName += strLockDate
                Else
                    strObjectName += LockDate.ToShortDateString
                End If

                oState.Audit(Audit.Action.aUpdate, Audit.ObjectType.tLockDate, strObjectName, tbParameters, -1)
            End If

            If bolRet Then
                ' En caso necesario, eliminamos la alerta de fichajes en periodo de cierre posterior a la fecha asignada
                ' y recalculamos todos los días a partir de la fecha de la alerta
                Dim xApplyLockDate As Date = GetEmployeeLockDatetoApply(IDEmployee, False, oState)
                Dim xAlert As DataTable = CreateDataTable("@SELECT# Key3DateTime FROM sysroNotificationTasks WHERE IDNotification=904 AND Key1Numeric=" & IDEmployee.ToString & " AND Key3DateTime > " & Any2Time(xApplyLockDate).SQLDateTime, )
                If xAlert IsNot Nothing AndAlso xAlert.Rows.Count > 0 Then
                    Dim xKey3DateTime As DateTime = Any2Time(xAlert.Rows(0)(0)).Value
                    strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) SET Status=0, [GUID] = '' WHERE IDEmployee=" & IDEmployee.ToString & " AND Date>=" & Any2Time(xKey3DateTime).SQLSmallDateTime & " AND Date <= getdate()"
                    bolRet = ExecuteSqlWithoutTimeOut(strSQL)

                    If bolRet Then
                        ' Eliminamos la alerta de fichajes en periodo de cierre
                        strSQL = "@DELETE# FROM sysroNotificationTasks WHERE IDNotification=904 AND Key1Numeric=" & IDEmployee.ToString & " AND Key3DateTime = " & Any2Time(xKey3DateTime).SQLDateTime
                        bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                    End If

                    If bolRet And bolInitTask Then
                        roConnector.InitTask(TasksType.MOVES)
                    End If

                End If
            End If
        Catch ex As DbException
            If Not oState Is Nothing Then oState.UpdateStateInfo(ex, "roBusinessSupport::SaveEmployeeLockDate")
        Catch ex As Exception
            If Not oState Is Nothing Then oState.UpdateStateInfo(ex, "roBusinessSupport::SaveEmployeeLockDate")
        Finally

        End Try

        Return bolRet

    End Function

    Public Shared Function SaveLockDate(ByVal LockDate As Date, ByRef oState As roBusinessState, Optional ByVal bolAudit As Boolean = False) As Boolean
        '
        ' Guardamos la fecha de bloqueo global que se utiliza por defecto
        '
        Dim bolRet As Boolean = False

        Dim strLockDate As String

        Try

            Dim strSQL As String = "@UPDATE# sysroDateParameters SET Value=@DateParam WHERE  ParameterName ='FirstDate'"

            If Not LockDate = CDate("1900/01/01") Then
                strLockDate = Any2Time(LockDate).SQLSmallDateTime
            Else
                ' Dejamos sin fecha de bloqueo global
                strLockDate = "Null"
            End If
            strSQL = strSQL.Replace("@DateParam", strLockDate)

            ' Asignamos fecha global de bloqueo
            bolRet = ExecuteSqlWithoutTimeOut(strSQL)

            If bolAudit And bolRet Then
                Dim tbParameters As DataTable = oState.CreateAuditParameters()
                Dim strObjectName As String = "GLOBAL --> "
                If strLockDate.ToUpper = "NULL" Then
                    strObjectName += strLockDate
                Else
                    strObjectName += LockDate.ToShortDateString
                End If

                oState.Audit(Audit.Action.aUpdate, Audit.ObjectType.tLockDate, strObjectName, tbParameters, -1)
            End If

            If bolRet Then
                ' En caso necesario, eliminamos la alerta de fichajes en periodo de cierre posterior a la fecha asignada
                ' de los empleados que no tengan fecha de cierre personalizada y tengan alguna alerta creada
                ' y recalculamos todos los días a partir de la fecha de la alerta

                If strLockDate.ToUpper = "NULL" Then strLockDate = Any2Time(CDate("1900/01/01")).SQLSmallDateTime

                Dim xAlert As DataTable = CreateDataTable("@SELECT# Key1Numeric, Key3DateTime FROM sysroNotificationTasks WHERE IDNotification=904  AND Key3DateTime > " & strLockDate & " AND Key1Numeric IN(@SELECT# ID FROM Employees WHERE LockDate IS NULL )", )
                If xAlert IsNot Nothing AndAlso xAlert.Rows.Count > 0 Then
                    For Each oRow As DataRow In xAlert.Rows
                        Dim xKey3DateTime As DateTime = Any2Time(oRow("Key3DateTime")).Value
                        strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) SET Status=0, [GUID] = '' WHERE IDEmployee=" & Any2Integer(oRow("Key1Numeric")) & " AND Date>=" & Any2Time(xKey3DateTime).SQLSmallDateTime & " AND Date <= getdate()"
                        bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                        If bolRet Then
                            ' Eliminamos la alerta de fichajes en periodo de cierre de esos empleados
                            strSQL = "@DELETE# FROM sysroNotificationTasks WHERE IDNotification=904 AND Key1Numeric=" & Any2Integer(oRow("Key1Numeric")) & " AND Key3DateTime = " & Any2Time(xKey3DateTime).SQLDateTime
                            bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                        End If
                    Next

                    roConnector.InitTask(TasksType.MOVES)
                End If
            End If
        Catch ex As DbException
            If Not oState Is Nothing Then oState.UpdateStateInfo(ex, "roBusinessSupport::SaveLockDate")
        Catch ex As Exception
            If Not oState Is Nothing Then oState.UpdateStateInfo(ex, "roBusinessSupport::SaveLockDate")
        Finally

        End Try

        Return bolRet

    End Function

    Public Shared Function GetEmployeesOnLockDate(ByVal strEmployeeFilter As String, ByVal xDate As Date, ByRef oState As roBusinessState) As DataTable
        '
        ' Obtenemos empleados que estan en periodo de cierre a una fecha concreta
        '
        Dim oRet As DataTable = Nothing

        Try

            Dim strSQL As String = "@SELECT# * FROM sysrovwEmployeeLockDate WHERE Lockdate >= " & Any2Time(xDate).SQLSmallDateTime
            If strEmployeeFilter.Length > 0 Then
                strSQL += " AND IDEmployee IN(" & strEmployeeFilter & ")"
            End If

            oRet = CreateDataTable(strSQL, )
        Catch ex As DbException
            If Not oState Is Nothing Then oState.UpdateStateInfo(ex, "roBusinessSupport::GetEmployeesOnLockDate")
        Catch ex As Exception
            If Not oState Is Nothing Then oState.UpdateStateInfo(ex, "roBusinessSupport::GetEmployeesOnLockDate")
        Finally

        End Try

        Return oRet

    End Function

    Public Shared Function GetEmployeeName(ByVal IdEmployee As Integer, ByRef oState As roBusinessState) As String

        Dim strName As String = String.Empty

        Try

            Dim strQuery As String = "@SELECT# Name FROM Employees WHERE ID = " & IdEmployee
            Dim tb As DataTable = CreateDataTable(strQuery, )
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                strName = roTypes.Any2String(tb.Rows(0).Item(0))
            End If
        Catch ex As DbException
            If oState IsNot Nothing Then oState.UpdateStateInfo(ex, "roBusinessSupport::GetEmployeeName")
        Catch ex As Exception
            If oState IsNot Nothing Then oState.UpdateStateInfo(ex, "roBusinessSupport::GetEmployeeName")
        End Try

        Return strName

    End Function

    Public Shared Function GetAuditEmployeeNames(ByVal lstEmployees As List(Of Integer), ByRef oState As roBusinessState) As String

        Dim sEmployeeNamesForAudit As String = String.Empty

        Try

            For Each idEmployee As Integer In lstEmployees.Take(20)
                sEmployeeNamesForAudit &= roBusinessSupport.GetEmployeeName(idEmployee, oState) & ","
            Next

            If sEmployeeNamesForAudit.Length > 0 Then
                sEmployeeNamesForAudit = sEmployeeNamesForAudit.Substring(0, sEmployeeNamesForAudit.Length - 1)
            End If

            If lstEmployees.Count > 20 Then sEmployeeNamesForAudit &= $" y {lstEmployees.Count - 20} {oState.Language.Keyword("employees")} +"

        Catch ex As DbException
            If oState IsNot Nothing Then oState.UpdateStateInfo(ex, "roBusinessSupport::GetAuditEmployeeNames")
        Catch ex As Exception
            If oState IsNot Nothing Then oState.UpdateStateInfo(ex, "roBusinessSupport::GetAuditEmployeeNames")
        End Try

        Return sEmployeeNamesForAudit

    End Function

    Public Shared Function GetGroupName(ByVal IdGroup As Integer, ByRef oState As roBusinessState) As String

        Dim strName As String = String.Empty

        Try

            Dim strQuery As String = "@SELECT# Name FROM Groups WHERE ID = " & IdGroup
            Dim tb As DataTable = CreateDataTable(strQuery, )
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                strName = roTypes.Any2String(tb.Rows(0).Item(0))
            End If
        Catch ex As DbException
            If Not oState Is Nothing Then oState.UpdateStateInfo(ex, "roBusinessSupport::GetGroupName")
        Catch ex As Exception
            If Not oState Is Nothing Then oState.UpdateStateInfo(ex, "roBusinessSupport::GetGroupName")
        Finally

        End Try

        Return strName

    End Function

    Public Shared Function GetGroupPath(ByVal IdGroup As Integer, ByRef oState As roBusinessState) As String

        Dim strName As String = String.Empty

        Try

            Dim strQuery As String = "@SELECT# Path FROM Groups WHERE ID = " & IdGroup
            Dim tb As DataTable = CreateDataTable(strQuery, )
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                strName = roTypes.Any2String(tb.Rows(0).Item(0))
            End If
        Catch ex As DbException
            If Not oState Is Nothing Then oState.UpdateStateInfo(ex, "roBusinessSupport::GetGroupName")
        Catch ex As Exception
            If Not oState Is Nothing Then oState.UpdateStateInfo(ex, "roBusinessSupport::GetGroupName")
        Finally

        End Try

        Return strName

    End Function

    Public Shared Function GetAccessGroupName(ByVal IDAccessGroup As Integer, ByVal _State As roBusinessState) As String
        Dim tbRet As String = ""

        Try

            Dim strSQL As String = "@SELECT# Name FROM AccessGroups WHERE ID = " & IDAccessGroup

            tbRet = roTypes.Any2String(ExecuteScalar(strSQL))
        Catch ex As Data.Common.DbException
            _State.UpdateStateInfo(ex, "roBusinessSupport::GetAccessGroupName")
        Catch ex As Exception
            _State.UpdateStateInfo(ex, "roBusinessSupport::GetAccessGroupName")
        Finally

        End Try

        Return tbRet
    End Function

    Public Shared Function GetCurrentGroupName(ByVal IDEmployee As Integer, ByRef oState As roBusinessState) As String
        Dim strGroupName As String = String.Empty

        oState.UpdateStateInfo()

        Try

            Dim strSQL As String
            strSQL = "@SELECT# g.Name from employees e inner join employeegroups eg  on e.id = eg.IDEmployee inner join groups g on eg.IDGroup = g.ID where " &
                         "e.ID = " & IDEmployee
            Dim tb As DataTable = CreateDataTable(strSQL, )
            If tb IsNot Nothing Then
                If tb.Rows.Count > 0 Then
                    strGroupName = roTypes.Any2String(tb.Rows(0)("Name"))
                End If
            End If
        Catch ex As DbException
            oState.UpdateStateInfo(ex, "roBusinessSupport::GetCurrentGroupName")
        Catch ex As Exception
            oState.UpdateStateInfo(ex, "roBusinessSupport::GetCurrentGroupName")
        Finally

        End Try

        Return strGroupName

    End Function

    Public Shared Function GetCurrentFullGroupName(ByVal IDEmployee As Integer, ByRef oState As roBusinessState) As String
        Dim strFullGroupName As String = String.Empty

        oState.UpdateStateInfo()

        Try

            Dim strSQL As String
            strSQL = "@SELECT# FullGroupName FROM sysrovwCurrentEmployeeGroups WHERE " &
                         "sysrovwCurrentEmployeeGroups.Begindate <= " & Any2Time(Now.Date).SQLSmallDateTime & " AND sysrovwCurrentEmployeeGroups.Enddate >= " & Any2Time(Now.Date).SQLSmallDateTime & " AND " &
                         "sysrovwCurrentEmployeeGroups.IDEmployee = " & IDEmployee
            Dim tb As DataTable = CreateDataTable(strSQL, )
            If tb IsNot Nothing Then
                If tb.Rows.Count > 0 Then
                    strFullGroupName = roTypes.Any2String(tb.Rows(0)("FullGroupName"))
                End If
            End If
        Catch ex As DbException
            oState.UpdateStateInfo(ex, "roBusinessSupport::GetCurrentFullGroupName")
        Catch ex As Exception
            oState.UpdateStateInfo(ex, "roBusinessSupport::GetCurrentFullGroupName")
        Finally

        End Try

        Return strFullGroupName

    End Function

    Public Shared Function HaveTeamJobMoves(ByVal IDEmployee As Integer) As Boolean
        Dim strSql As String
        Dim oDataset As DataSet

        strSql = "@SELECT# top 1 TeamJobMoves.IDTeam From TeamJobMoves "
        strSql &= "Inner Join EmployeeTeams On EmployeeTeams.IDTeam = TeamJobMoves.IDTeam "
        strSql &= "And EmployeeTeams.IDEmployee = " & IDEmployee

        oDataset = CreateDataSet(strSql)

        If oDataset IsNot Nothing Then
            If oDataset.Tables(0).Rows.Count > 0 Then
                HaveTeamJobMoves = True
            Else
                HaveTeamJobMoves = False
            End If
        Else
            HaveTeamJobMoves = False
        End If
    End Function

    Public Shared Function HaveDailyJobAccruals(ByVal IDEmployee As Integer) As Boolean
        Dim strSql As String
        Dim oDataset As DataSet

        strSql = "@SELECT# top 1 IDEmployee From DailyJobAccruals Where IDEmployee = " & IDEmployee

        oDataset = CreateDataSet(strSql)

        If oDataset IsNot Nothing Then
            If oDataset.Tables(0).Rows.Count > 0 Then
                HaveDailyJobAccruals = True
            Else
                HaveDailyJobAccruals = False
            End If
        Else
            HaveDailyJobAccruals = False
        End If
    End Function

    Public Shared Function HaveDailyTaskAccruals(ByVal IDEmployee As Integer) As Boolean
        Dim bRet As Boolean = False
        Dim strSql As String
        Dim oDataset As DataSet

        strSql = "@SELECT# top 1 IDEmployee From DailyTaskAccruals Where IDEmployee = " & IDEmployee

        oDataset = CreateDataSet(strSql)

        If oDataset IsNot Nothing Then
            If oDataset.Tables(0).Rows.Count > 0 Then
                bRet = True
            Else
                bRet = False
            End If
        Else
            bRet = False
        End If

        Return bRet
    End Function

    Public Shared Function HaveJobMoves(ByVal IDEmployee As Integer) As Boolean
        Dim strSql As String
        Dim oDataset As DataSet

        strSql = "@SELECT# top 1 IDEmployee From EmployeeJobMoves Where IDEmployee = " & IDEmployee

        oDataset = CreateDataSet(strSql)

        If oDataset IsNot Nothing Then
            If oDataset.Tables(0).Rows.Count > 0 Then
                HaveJobMoves = True
            Else
                HaveJobMoves = False
            End If
        Else
            HaveJobMoves = False
        End If
    End Function

    Public Shared Function EmployeeWithContract(ByVal intIDEmployee As Integer, ByRef oState As roBusinessState, Optional ByVal oDate As Object = Nothing) As Boolean

        Dim bolRet As Boolean = False
        Dim strSQL As String
        Dim xTime As New roTime

        oState.UpdateStateInfo()

        If oDate IsNot Nothing Then
            xTime.Value = CDate(oDate).Date
        Else
            xTime.Value = Now.Date
        End If

        Try

            strSQL = "@SELECT# IDEmployee FROM EmployeeContracts WHERE IDEmployee = " & intIDEmployee
            strSQL &= " AND " & xTime.SQLSmallDateTime & " >= BeginDate And EndDate >= " & xTime.SQLSmallDateTime

            Dim tb As DataTable = CreateDataTable(strSQL, )
            If tb IsNot Nothing Then
                bolRet = (tb.Rows.Count > 0)
            End If
        Catch ex As DbException
            oState.UpdateStateInfo(ex, "roBusinessSupport::EmployeeWithContract")
        Catch ex As Exception
            oState.UpdateStateInfo(ex, "roBusinessSupport::EmployeeWithContract")
        Finally

        End Try

        Return bolRet

    End Function

    Public Shared Function FirstContractDate(ByVal intIDEmployee As Integer, ByRef oState As roBusinessState) As DateTime

        Dim xRet As DateTime

        oState.UpdateStateInfo()

        Dim tb As DataTable = Nothing
        Try

            Dim strSQL As String
            strSQL = "@SELECT# Min(BeginDate) FROM EmployeeContracts " &
                         "WHERE IDEmployee=" & intIDEmployee.ToString
            tb = CreateDataTable(strSQL, )
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                If Not IsDBNull(tb.Rows(0)(0)) Then
                    xRet = CDate(tb.Rows(0)(0))
                End If
            End If
        Catch ex As DbException
            oState.UpdateStateInfo(ex, "roBusinessSupport::FirstContractDate")
        Catch ex As Exception
            oState.UpdateStateInfo(ex, "roBusinessSupport::FirstContractDate")
        Finally

        End Try

        Return xRet

    End Function

    Public Shared Function ExistEmployeeAssignment(ByVal _IDEmployee As Integer, ByVal _IDAssignment As Integer, ByVal _State As roBusinessState) As Boolean

        Dim bolRet As Boolean = False

        Try

            Dim strSQL As String = "@SELECT# * " &
                                       "FROM EmployeeAssignments " &
                                       "WHERE EmployeeAssignments.IDEmployee = " & _IDEmployee.ToString & " AND " &
                                             "EmployeeAssignments.IDAssignment = " & _IDAssignment.ToString
            Dim tb As DataTable = CreateDataTable(strSQL, )
            bolRet = (tb IsNot Nothing AndAlso tb.Rows.Count > 0)
        Catch ex As DbException
            _State.UpdateStateInfo(ex, "roBusinessSupport::ExistEmployeeAssignment")
        Catch ex As Exception
            _State.UpdateStateInfo(ex, "roBusinessSupport::ExistEmployeeAssignment")
        Finally

        End Try

        Return bolRet

    End Function

#End Region

#Region "Company Info"

    Public Shared Function GetCurrentCompanyId(ByVal IDEmployee As Integer, ByRef oState As roBusinessState, Optional ByRef CompanyName As String = "", Optional dDate As Date = Nothing) As Integer
        Dim strFullGroupName As String = String.Empty
        Dim strFullGroupPath As String = String.Empty

        oState.UpdateStateInfo()

        If dDate = Nothing Then dDate = Now.Date

        Try
            Dim strSQL As String
            strSQL = "@SELECT# FullGroupName, Path FROM sysrovwCurrentEmployeeGroups WHERE " &
                         "sysrovwCurrentEmployeeGroups.Begindate <= " & Any2Time(dDate).SQLSmallDateTime & " AND sysrovwCurrentEmployeeGroups.Enddate >= " & Any2Time(dDate).SQLSmallDateTime & " AND " &
                         "sysrovwCurrentEmployeeGroups.IDEmployee = " & IDEmployee
            Dim tb As DataTable = CreateDataTable(strSQL, )
            If tb IsNot Nothing Then
                If tb.Rows.Count > 0 Then
                    strFullGroupPath = roTypes.Any2String(tb.Rows(0)("Path"))
                    strFullGroupName = roTypes.Any2String(tb.Rows(0)("FullGroupName"))
                End If
            End If

            CompanyName = strFullGroupName.Split(" \ ")(0)
        Catch ex As DbException
            oState.UpdateStateInfo(ex, "roBusinessSupport::GetCurrentCompanyId")
        Catch ex As Exception
            oState.UpdateStateInfo(ex, "roBusinessSupport::GetCurrentCompanyId")
        End Try

        Return strFullGroupPath.Split("\")(0)
    End Function

#End Region

#Region "System"

    Public Shared Function InPeriodOfFreezing(ByVal xDate As Date, ByVal IDEmployee As Integer, ByRef oState As roBusinessState) As Boolean

        Dim bolRet As Boolean = False

        Try

            Dim xFreezingDateEmployee As Date = roBusinessSupport.GetEmployeeLockDatetoApply(IDEmployee, False, oState)
            bolRet = (xDate <= CDate(xFreezingDateEmployee))
        Catch ex As DbException
            If Not oState Is Nothing Then oState.UpdateStateInfo(ex, "roBusinessSupport::InPeriodOfFreezing")
        Catch ex As Exception
            If Not oState Is Nothing Then oState.UpdateStateInfo(ex, "roBusinessSupport::InPeriodOfFreezing")
        Finally

        End Try

        Return bolRet

    End Function

    Public Shared Function IsValidFileNameOrPath(ByVal strPath As String) As Boolean

        Dim bRet As Boolean = True

        Try

            If strPath IsNot Nothing Then

                For Each badChar As Char In System.IO.Path.GetInvalidPathChars
                    If InStr(strPath, badChar) > 0 Then
                        bRet = False
                    End If
                Next

                'obtener nombre
                Dim strName As String = System.IO.Path.GetFileName(strPath)
                For Each badChar As Char In System.IO.Path.GetInvalidFileNameChars
                    If InStr(strName, badChar) > 0 Then
                        bRet = False
                    End If
                Next

            End If
        Catch ex As Exception

        End Try

        Return bRet

    End Function

    Public Shared Function IsProcessRunning(ByVal Name As String) As Boolean
        Try
            Dim proc As System.Diagnostics.Process() = System.Diagnostics.Process.GetProcessesByName(Name)
            Return (proc.Length > 0)
        Catch ex As Exception
            Return True
        End Try

    End Function

#End Region

#Region "Cards conversion methods"

    ''' <summary>
    ''' Convierte un código de tarjeta (en decimal) a su equivalente en VT (compatible terminales mx)
    ''' </summary>
    ''' <param name="card">Código tarjeta</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function DataReader2VTValue(ByVal card As Long) As String

        Dim sCard As String
        Dim IDCard As String = ""
        Dim i As Integer

        sCard = Hex(card)

        For i = 1 To Len(sCard)
            IDCard = IDCard & Format(CLng("&H" & Mid(sCard, i, 1)), "00")
        Next i

        Return IDCard

    End Function

    ''' <summary>
    ''' Convierte un código de tarjeta de VT (compatible terminales mx) a su código real en decimal
    ''' </summary>
    ''' <param name="IDCard">Código tarjeta de VT (código compatible terminales mx)</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function VTValue2DataReader(ByVal IDCard As String) As Long
        Dim card As String = String.Empty
        Dim i As Integer

        If IDCard.Length Mod 2 <> 0 Then IDCard = "0" & IDCard

        i = 0
        Do While Len(IDCard) > 0
            'card = Hex(Right$(IDCard, 2)) & card
            card = Hex(IDCard.Substring(IDCard.Length - 2)) & card
            If Len(IDCard) > 1 Then
                'IDCard = Left$(IDCard, Len(IDCard) - 2)
                IDCard = IDCard.Substring(0, Len(IDCard) - 2)
            Else
                Exit Do
            End If
        Loop

        Return CLng("&H" & card)

    End Function

#End Region

#Region "Client Check Helper Methods"

    Public Shared Function CheckLivePortal() As Integer

        Dim oRet As Integer = -1

        Try

            'oCn = CreateConnection()
            'bolCloseCn = True

            oRet = 0

            'Dim strSQL As String = "@SELECT# Data FROM sysroParameters WHERE [ID] = 'DBVERSION'"

            'Dim tb As DataTable = CreateDataTable(strSQL, )
            'If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
            '    oRet = roTypes.Any2String(tb.Rows(0).Item("Data"))
            'End If
        Catch ex As DbException
            oRet = -1
        Catch ex As Exception
            oRet = -1
        Finally
            '
        End Try

        Return oRet

    End Function

    Public Shared Function SetUserPhoto(ByVal IdEmployee As String, ByVal oImage As Byte()) As Boolean

        Dim oRet As Boolean = False
        Dim oSqlCommand As DbCommand = Nothing
        Dim sSQL As String = ""

        Try

            sSQL = "@UPDATE# Employees "
            sSQL = sSQL & " Set Image = @Image "
            sSQL = sSQL & " WHERE ID=" & IdEmployee

            oSqlCommand = CreateCommand(sSQL)

            AddParameter(oSqlCommand, "@Image", System.Data.SqlDbType.Binary)

            'oSqlCommand.CommandText = sSQL

            If oImage Is Nothing Then
                oSqlCommand.Parameters("@Image").Value = DBNull.Value
            Else
                oSqlCommand.Parameters("@Image").Value = oImage
            End If

            oSqlCommand.ExecuteNonQuery()

            oRet = True
        Catch ex As DbException
            oRet = False
        Catch ex As Exception
            oRet = False
        Finally

        End Try
        Return oRet

    End Function

    Public Shared Function GetEmployeePhoto(ByVal IDEmployee As Integer) As Byte()

        Dim oRet As Byte()

        Try

            oRet = Nothing

            Dim strSQL As String = "@SELECT# Image FROM Employees WHERE [ID] = " & IDEmployee

            Dim tb As DataTable = CreateDataTable(strSQL, )
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                Dim oRow As DataRow = tb.Rows(0)

                If Not IsDBNull(oRow("image")) Then
                    Dim bits As Byte() = CType(oRow("Image"), Byte())

                    oRet = bits
                Else
                    oRet = Nothing
                End If
            End If
        Catch ex As DbException
            oRet = Nothing
        Catch ex As Exception
            oRet = Nothing
        Finally

        End Try
        Return oRet

    End Function

    Public Shared Function GetUserPhoto(ByVal IDPassport As Integer) As Byte()

        Dim oRet As Byte()

        Try

            oRet = Nothing

            Dim strSQL As String = "@SELECT# Image FROM sysroPassports WHERE [ID] = " & IDPassport

            Dim tb As DataTable = CreateDataTable(strSQL, )
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                Dim oRow As DataRow = tb.Rows(0)

                If Not IsDBNull(oRow("image")) Then
                    Dim bits As Byte() = CType(oRow("Image"), Byte())

                    oRet = bits
                Else
                    oRet = Nothing
                End If
            End If
        Catch ex As DbException
            oRet = Nothing
        Catch ex As Exception
            oRet = Nothing
        Finally

        End Try
        Return oRet

    End Function

    Public Shared Function GetCustomizationCode(Optional ByVal oState As roBaseState = Nothing) As String
        Dim oAdvState As New AdvancedParameter.roAdvancedParameterState(-1)

        If oState IsNot Nothing Then roBusinessState.CopyTo(oState, oAdvState)

        Dim oParam As New AdvancedParameter.roAdvancedParameter(AdvancedParameterType.Customization.ToString(), oAdvState)

        If oParam.Value IsNot Nothing Then
            Return oParam.Value.Trim
        Else
            Return ""
        End If
    End Function

#End Region

End Class