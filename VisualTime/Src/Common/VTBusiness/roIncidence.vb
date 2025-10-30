Imports System.Data.Common
Imports System.Runtime.Serialization
Imports System.Xml.Serialization
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.Security.Base

Namespace Incidence

    <DataContract()>
    Public Class roIncidence

#Region "Declarations - Constructor"

        Private oState As roIncidenceState

        Private intID As Integer
        Private strModule As String
        Private strNamedID As String
        Private strDescription As String
        Private bolStored As Boolean
        Private intTreatAs As Integer
        Private bolWorkingTime As Boolean

        Public Sub New()
            Me.oState = New roIncidenceState()
            Me.intID = -1
        End Sub

        Public Sub New(ByVal _ID As Integer, ByVal _State As roIncidenceState)
            Me.oState = _State
            Me.ID = _ID
        End Sub

#End Region

#Region "Properties"

        <DataMember()>
        Public Property ID() As Integer
            Get
                Return Me.intID
            End Get
            Set(ByVal value As Integer)
                Me.intID = value
                Me.Load()
            End Set
        End Property
        <DataMember()>
        Public Property _Module() As String
            Get
                Return Me.strModule
            End Get
            Set(ByVal value As String)
                Me.strModule = value
            End Set
        End Property
        <DataMember()>
        Public Property NamedID() As String
            Get
                Return Me.strNamedID
            End Get
            Set(ByVal value As String)
                Me.strNamedID = value
            End Set
        End Property
        <DataMember()>
        Public Property Description() As String
            Get
                Return Me.strDescription
            End Get
            Set(ByVal value As String)
                Me.strDescription = value
            End Set
        End Property
        <DataMember()>
        Public Property Stored() As Boolean
            Get
                Return Me.bolStored
            End Get
            Set(ByVal value As Boolean)
                Me.bolStored = value
            End Set
        End Property
        <DataMember()>
        Public Property TreatAs() As Integer
            Get
                Return Me.intTreatAs
            End Get
            Set(ByVal value As Integer)
                Me.intTreatAs = value
            End Set
        End Property
        <DataMember()>
        Public Property WorkingTime() As Boolean
            Get
                Return Me.bolWorkingTime
            End Get
            Set(ByVal value As Boolean)
                Me.bolWorkingTime = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Load() As Boolean

            Dim bolRet As Boolean = False

            Dim tb As DataTable = Nothing
            Try

                Dim strSQL As String = "@SELECT# * FROM sysroDailyIncidencesTypes " &
                                       "WHERE [ID] = " & Me.intID.ToString
                tb = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    Me.strModule = tb.Rows(0)("Module")
                    Me.strNamedID = tb.Rows(0)("NamedID")
                    If Not IsDBNull(tb.Rows(0)("Description")) Then Me.strDescription = tb.Rows(0)("Description")
                    Me.bolStored = tb.Rows(0)("Stored")
                    If Not IsDBNull(tb.Rows(0)("TreatAs")) Then Me.intTreatAs = tb.Rows(0)("TreatAs")
                    Me.bolWorkingTime = tb.Rows(0)("WorkingTime")
                End If

                bolRet = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roIncidence::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roIncidence::Load")
            Finally

            End Try

            Return bolRet

        End Function

#End Region

#Region "Incidences"

        Public Shared Function GetIncidences(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByRef oState As roBusinessState) As DataTable

            Dim tb As DataTable = Nothing

            Try

                Dim strSQL As String
                strSQL = "@SELECT# TimeZones.Name AS TimeZoneName, " &
                                "DailyIncidences.BeginTime, DailyIncidences.EndTime, " &
                                "DailyIncidences.Value AS IncidenceValue, " &
                                "DailyCauses.Value,DailyCauses.Manual, DailyCauses.AccrualsRules, " &
                                "DailyIncidences.IDType, DailyCauses.CauseUser, DailyCauses.CauseUserType, DailyCauses.IsNotReliable, " &
                                "DailyCauses.IDEmployee, DailyCauses.Date, DailyCauses.IDRelatedIncidence, DailyCauses.IDCause, DailyCauses.IDCenter, DailyCauses.DefaultCenter, DailyCauses.ManualCenter, " &
                                "ISNULL(DailyIncidences.BeginTime, CONVERT(smalldatetime, '2079/01/01 00:00', 120)) AS BeginTimeOrder, DailyCauses.DailyRule, DailyCauses.AccruedRule, Causes.DayType, Causes.CustomType  " &
                         "FROM DailyIncidences INNER JOIN TimeZones ON DailyIncidences.IDZone = TimeZones.ID " &
                                "RIGHT OUTER JOIN DailyCauses ON DailyIncidences.IDEmployee = DailyCauses.IDEmployee AND " &
                                                                "DailyIncidences.Date = DailyCauses.Date AND " &
                                                                "DailyIncidences.ID = DailyCauses.IDRelatedIncidence INNER JOIN Causes ON DailyCauses.IDCause = Causes.ID " &
                         "WHERE DailyCauses.IDEmployee = " & intIDEmployee & " AND " &
                               "DailyCauses.Date = " & roTypes.Any2Time(xDate).SQLSmallDateTime & " " &
                         "ORDER BY DailyIncidences.BeginTime"
                tb = CreateDataTable(strSQL)
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetIncidences")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetIncidences")
            End Try

            Return tb

        End Function

        Public Shared Function GetIncidencesByFillter(ByVal intIDEmployee As Integer, ByVal strWhere As String, ByVal xDate As Date, ByRef oState As roBusinessState) As DataTable
            Dim tb As DataTable = Nothing

            Try

                Dim strSQL As String
                strSQL = "@SELECT# * " &
                         "FROM DailyIncidences " &
                         "WHERE DailyIncidences.IDEmployee = " & intIDEmployee & " AND " &
                               "DailyIncidences.Date = " & roTypes.Any2Time(xDate).SQLSmallDateTime
                If Len(strWhere) > 0 Then strSQL += " AND " & strWhere

                strSQL += " ORDER BY DailyIncidences.BeginTime"

                tb = CreateDataTable(strSQL, "")
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetIncidencesByFillter")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetIncidencesByFillter")
            Finally

            End Try

            Return tb

        End Function

        Public Shared Function GetCausesByFillter(ByVal intIDEmployee As Integer, ByVal strWhere As String, ByVal xDate As Date, ByRef oState As roBusinessState) As DataTable
            Dim tb As DataTable = Nothing

            Try

                Dim strSQL As String
                strSQL = "@SELECT# * " &
                         "FROM DailyCauses " &
                         "WHERE DailyCauses.IDEmployee = " & intIDEmployee & " AND " &
                               "DailyCauses.Date = " & roTypes.Any2Time(xDate).SQLSmallDateTime
                If Len(strWhere) > 0 Then strSQL += " AND " & strWhere

                tb = CreateDataTable(strSQL, "")
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetCausesByFillter")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetCausesByFillter")
            Finally

            End Try

            Return tb

        End Function

        Public Shared Function GetMassIncidences(ByVal intIDEmployees As String, ByVal xBeginDate As Date, ByVal xEndDate As Date, ByVal strIncidencesType As String, ByVal OnlyNotJustified As Boolean, ByVal strCenters As String, ByVal strCausesFilter As String, ByVal strCausesValueFilter As String, ByRef oState As roBusinessState) As DataTable

            Dim tb As DataTable = Nothing

            Try

                Dim strSQL As String

                strSQL = "@SELECT# Employees.ID as EmployeesID, Employees.Name as EmployeesName, IDContract, '' as PunchesLst, " &
                 " DailyIncidences.Date, TimeZones.Name AS TimeZoneName, DailyIncidences.BeginTime, DailyIncidences.EndTime, " &
                 " DailyIncidences.Value AS IncidenceValue, DailyCauses.Value,DailyCauses.Manual, " &
                 " DailyCauses.AccrualsRules, DailyIncidences.IDType, DailyCauses.CauseUser, DailyCauses.CauseUserType, DailyCauses.IsNotReliable, " &
                 " DailyCauses.IDEmployee, DailyCauses.Date, DailyCauses.IDRelatedIncidence, DailyCauses.IDCause, ISNULL(DailyIncidences.BeginTime, CONVERT(smalldatetime, '2079/01/01 00:00', 120)) AS BeginTimeOrder , DailyCauses.IDCenter , DailyCauses.IDCenter, DailyCauses.ManualCenter, DailyCauses.DefaultCenter, isNull(BusinessCenters.name, '') AS CenterName, DailyCauses.DailyRule, DailyCauses.AccruedRule " &
                 " FROM DailyIncidences INNER JOIN TimeZones ON DailyIncidences.IDZone = TimeZones.ID  " &
                 " INNER JOIN DailyCauses ON DailyIncidences.IDEmployee = DailyCauses.IDEmployee AND DailyIncidences.Date = DailyCauses.Date AND DailyIncidences.ID = DailyCauses.IDRelatedIncidence " &
                 " INNER JOIN Employees ON DailyIncidences.IDEmployee = Employees.ID " &
                 " LEFT OUTER JOIN dbo.BusinessCenters ON dbo.DailyCauses.IDCenter = dbo.BusinessCenters.ID " &
                 " INNER JOIN EmployeeContracts  ON Employees.ID = EmployeeContracts.IDEmployee AND dailyincidences.date BETWEEN EmployeeContracts.BeginDate AND EmployeeContracts.EndDate " &
                 " WHERE ((" & roTypes.Any2Time(roTypes.Any2Time(xBeginDate).DateOnly).SQLSmallDateTime & " >= EmployeeContracts.BeginDate" &
                 " And " & roTypes.Any2Time(roTypes.Any2Time(xBeginDate).DateOnly).SQLSmallDateTime & " <= EmployeeContracts.EndDate)" &
                 " Or (" & roTypes.Any2Time(roTypes.Any2Time(xEndDate).DateOnly).SQLSmallDateTime & " >= EmployeeContracts.BeginDate" &
                 " And " & roTypes.Any2Time(roTypes.Any2Time(xEndDate).DateOnly).SQLSmallDateTime & " <= EmployeeContracts.EndDate)" &
                 " Or (" & roTypes.Any2Time(roTypes.Any2Time(xBeginDate).DateOnly).SQLSmallDateTime & " <= EmployeeContracts.BeginDate" &
                 " And " & roTypes.Any2Time(roTypes.Any2Time(xEndDate).DateOnly).SQLSmallDateTime & " >= EmployeeContracts.BeginDate))" &
                 " And DailyCauses.IDEmployee IN(" & intIDEmployees & ")" &
                 " AND DailyCauses.Date Between " & roTypes.Any2Time(roTypes.Any2Time(xBeginDate).DateOnly).SQLSmallDateTime & " AND " & roTypes.Any2Time(roTypes.Any2Time(xEndDate).DateOnly).SQLSmallDateTime

                strSQL += " AND DailyIncidences.IDType IN(" & strIncidencesType & ")"
                'If OnlyNotJustified Then
                '    strSQL += " AND DailyCauses.IDCause = 0 "
                'End If

                If strCausesFilter <> String.Empty Then
                    strSQL &= " AND DailyCauses.IDCause IN(" & strCausesFilter & ") "
                End If

                If strCausesValueFilter <> String.Empty Then
                    Dim tmpFilterValues As String() = strCausesValueFilter.Split("#")
                    strSQL &= " AND DailyIncidences.Value " & IIf(roTypes.Any2Integer(tmpFilterValues(0)) = 0, ">= ", "< ") & tmpFilterValues(1)
                End If

                If strCenters <> "-1" Then
                    strSQL += " AND DailyCauses.IDCenter  IN(" & strCenters & ")"
                End If

                strSQL += " ORDER BY  Employees.Name, DailyIncidences.Date, DailyIncidences.BeginTime "

                tb = CreateDataTableWithoutTimeouts(strSQL)

                'Features (security)
                For Each oDataRow As DataRow In tb.Rows
                    Dim tbDetail As DataTable
                    Dim TotalDetail As String = ""
                    tbDetail = Scheduler.roScheduler.PresenceDetailEx(oDataRow("EmployeesID"), CDate(oDataRow("Date")), CDate(oDataRow("Date")), oState, -1)
                    For Each oRowDetail As DataRow In tbDetail.Rows
                        TotalDetail &= oRowDetail("Moves")
                    Next
                    oDataRow("PunchesLst") = TotalDetail.Trim()
                Next

                tb.AcceptChanges()
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetMassIncidences")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetMassIncidences")
            End Try

            Return tb

        End Function

        Public Shared Function GetMassCenters(ByVal intIDEmployees As String, ByVal xBeginDate As Date, ByVal xEndDate As Date, ByVal strCausesIDs As String, ByRef oState As roBusinessState, Optional strBusinessCenters As String = "", Optional directEmployeeIds As String = "-1", Optional directGroupIds As String = "-1") As DataTable

            Dim tb As DataTable = Nothing

            Try
                Dim strSQL As String

                If roBusinessSupport.GetCustomizationCode(oState) = "taif" Then
                    strSQL = "@SELECT# Employees.ID as EmployeesID, Employees.Name as EmployeesName, IDContract, '' as PunchesLst, " &
                             " DailyIncidences_MIRROR.Date as DateIncidence,TimeZones.Name AS TimeZoneName, DailyIncidences_MIRROR.BeginTime, DailyIncidences_MIRROR.EndTime, " &
                                           "DailyIncidences_MIRROR.Value AS IncidenceValue, DailyCauses_MIRROR.Value,DailyCauses_MIRROR.Manual, DailyCauses_MIRROR.AccrualsRules,DailyIncidences_MIRROR.IDType, DailyCauses_MIRROR.CauseUser, DailyCauses_MIRROR.CauseUserType, DailyCauses_MIRROR.IsNotReliable, " &
                                           "DailyCauses_MIRROR.IDEmployee, DailyCauses_MIRROR.Date , DailyCauses_MIRROR.IDRelatedIncidence, DailyCauses_MIRROR.IDCause, DailyCauses_MIRROR.IDCenter, DailyCauses_MIRROR.DefaultCenter, DailyCauses_MIRROR.ManualCenter, " &
                                           "ISNULL(DailyIncidences_MIRROR.BeginTime, CONVERT(smalldatetime, '2079/01/01 00:00', 120)) AS BeginTimeOrder,  DailyCauses_MIRROR.DailyRule, DailyCauses_MIRROR.AccruedRule  " &
                                    "FROM DailyIncidences_MIRROR INNER JOIN TimeZones ON DailyIncidences_MIRROR.IDZone = TimeZones.ID " &
                                           "RIGHT OUTER JOIN DailyCauses_MIRROR ON DailyIncidences_MIRROR.IDEmployee = DailyCauses_MIRROR.IDEmployee AND " &
                                                                           "DailyIncidences_MIRROR.Date = DailyCauses_MIRROR.Date AND " &
                                                                           "DailyIncidences_MIRROR.ID = DailyCauses_MIRROR.IDRelatedIncidence " &
                            " INNER JOIN Employees ON DailyCauses_MIRROR.IDEmployee = Employees.ID " &
                             " INNER JOIN EmployeeContracts  ON Employees.ID = EmployeeContracts.IDEmployee " &
                             " WHERE ((" & roTypes.Any2Time(roTypes.Any2Time(xBeginDate).DateOnly).SQLSmallDateTime & " >= EmployeeContracts.BeginDate" &
                             " And " & roTypes.Any2Time(roTypes.Any2Time(xBeginDate).DateOnly).SQLSmallDateTime & " <= EmployeeContracts.EndDate)" &
                             " Or (" & roTypes.Any2Time(roTypes.Any2Time(xEndDate).DateOnly).SQLSmallDateTime & " >= EmployeeContracts.BeginDate" &
                             " And " & roTypes.Any2Time(roTypes.Any2Time(xEndDate).DateOnly).SQLSmallDateTime & " <= EmployeeContracts.EndDate)" &
                             " Or (" & roTypes.Any2Time(roTypes.Any2Time(xBeginDate).DateOnly).SQLSmallDateTime & " <= EmployeeContracts.BeginDate" &
                             " And " & roTypes.Any2Time(roTypes.Any2Time(xEndDate).DateOnly).SQLSmallDateTime & " >= EmployeeContracts.BeginDate))" &
                             " And DailyCauses_MIRROR.IDEmployee IN(" & intIDEmployees & ")" &
                             " AND DailyCauses_MIRROR.Date Between " & roTypes.Any2Time(roTypes.Any2Time(xBeginDate).DateOnly).SQLSmallDateTime & " AND " & roTypes.Any2Time(roTypes.Any2Time(xEndDate).DateOnly).SQLSmallDateTime

                    If strCausesIDs <> String.Empty Then
                        strSQL &= " AND DailyCauses_MIRROR.IDCause IN(" & strCausesIDs & ") "
                    End If
                    If Not (String.IsNullOrEmpty(strBusinessCenters)) Then
                        strSQL &= " AND DailyCauses_MIRROR.IDCenter IN(" & strBusinessCenters & ") "
                    End If

                    strSQL += " ORDER BY  Employees.Name, DailyIncidences_MIRROR.Date, DailyIncidences_MIRROR.BeginTime "
                Else
                    If directEmployeeIds = String.Empty Then directEmployeeIds = "-1"
                    If directGroupIds = String.Empty Then directGroupIds = "-1"
                    strSQL = $"@SELECT# DISTINCT Employees.ID as EmployeesID, Employees.Name as EmployeesName, IDContract, '' as PunchesLst, 
                                        DailyIncidences.Date as DateIncidence,TimeZones.Name AS TimeZoneName, DailyIncidences.BeginTime, DailyIncidences.EndTime, 
                                        DailyIncidences.Value AS IncidenceValue, DailyCauses.Value,DailyCauses.Manual, DailyCauses.AccrualsRules,DailyIncidences.IDType, DailyCauses.CauseUser, DailyCauses.CauseUserType, DailyCauses.IsNotReliable, 
                                        DailyCauses.IDEmployee, DailyCauses.Date , DailyCauses.IDRelatedIncidence, DailyCauses.IDCause, DailyCauses.IDCenter, DailyCauses.DefaultCenter, DailyCauses.ManualCenter, 
                                        ISNULL(DailyIncidences.BeginTime, CONVERT(smalldatetime, '2079/01/01 00:00', 120)) AS BeginTimeOrder, DailyCauses.DailyRule , DailyCauses.AccruedRule  
                                FROM DailyIncidences 
                                INNER JOIN TimeZones ON DailyIncidences.IDZone = TimeZones.ID 
                                RIGHT OUTER JOIN DailyCauses ON DailyIncidences.IDEmployee = DailyCauses.IDEmployee 
                                                                AND DailyIncidences.Date = DailyCauses.Date 
                                                                AND  DailyIncidences.ID = DailyCauses.IDRelatedIncidence 
                                INNER JOIN Employees ON DailyCauses.IDEmployee = Employees.ID 
                                INNER JOIN EmployeeContracts  ON Employees.ID = EmployeeContracts.IDEmployee
                                INNER JOIN (
                                    @SELECT# RootGroup.id as IdRootGroup, Groups.id as IdGroup, Groups.Path as realpath, RootGroup.Path, eg.IDEmployee, eg.BeginDate, eg.EndDate 
                                    FROM Groups RootGroup
                                    INNER JOIN Groups on RootGroup.Path = Groups.path OR Groups.Path LIKE RootGroup.Path + '\%'
                                    INNER JOIN EmployeeGroups eg on eg.IDGroup = Groups.Id 
                                ) AUX ON DailyCauses.Date BETWEEN AUX.BeginDate AND AUX.EndDate  AND AUX.IDEmployee = DailyCauses.IDEmployee
                                WHERE {roTypes.Any2Time(roTypes.Any2Time(xBeginDate).DateOnly).SQLSmallDateTime} <= EmployeeContracts.EndDate AND {roTypes.Any2Time(roTypes.Any2Time(xEndDate).DateOnly).SQLSmallDateTime} >= EmployeeContracts.BeginDate
                                    And (DailyCauses.IDEmployee IN({directEmployeeIds}) OR AUX.IdRootGroup IN ({directGroupIds}))
                                    AND DailyCauses.Date Between {roTypes.Any2Time(roTypes.Any2Time(xBeginDate).DateOnly).SQLSmallDateTime} AND  {roTypes.Any2Time(roTypes.Any2Time(xEndDate).DateOnly).SQLSmallDateTime}"

                    If strCausesIDs <> String.Empty Then
                        strSQL &= " AND DailyCauses.IDCause IN(" & strCausesIDs & ") "
                    End If

                    If Not (String.IsNullOrEmpty(strBusinessCenters)) Then
                        strSQL &= " AND DailyCauses.IDCenter IN(" & strBusinessCenters & ") "
                    End If

                    strSQL += " ORDER BY  Employees.Name, DailyIncidences.Date, DailyIncidences.BeginTime "
                End If

                tb = CreateDataTableWithoutTimeouts(strSQL)

                'Features (security)
                For Each oDataRow As DataRow In tb.Rows
                    Dim tbDetail As DataTable
                    Dim TotalDetail As String = ""
                    tbDetail = Scheduler.roScheduler.PresenceDetailEx(oDataRow("EmployeesID"), CDate(oDataRow("Date")), CDate(oDataRow("Date")), oState, -1)
                    For Each oRowDetail As DataRow In tbDetail.Rows
                        TotalDetail &= oRowDetail("Moves")
                    Next
                    oDataRow("PunchesLst") = TotalDetail.Trim()
                Next

                tb.AcceptChanges()
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetMassCenters")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetMassCenters")
            End Try

            Return tb

        End Function

        Public Shared Function GetIncompletIncidences(ByVal xBegin As Date, ByVal xEnd As Date, ByVal intIDGroup As Integer, ByVal strIDEmployees As String, ByRef oState As roBusinessState) As DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String
                strSQL = "@SELECT# Employees.Name, DailyCauses.IDEmployee, DailyCauses.Date, Groups.Name as GroupName " &
                         "FROM Employees INNER JOIN " &
                                "EmployeeGroups ON Employees.ID = EmployeeGroups.IDEmployee INNER JOIN " &
                                "Groups ON EmployeeGroups.IDGroup = Groups.ID INNER JOIN " &
                                "DailyCauses ON Employees.ID = DailyCauses.IDEmployee " &
                                " INNER Join sysrovwSecurity_PermissionOverEmployees poe with (NOLOCK) on poe.IDPassport = " & oState.IDPassport & "  And poe.IDEmployee = DailyCauses.IDEmployee And DateAdd(Day, DateDiff(Day, 0, DailyCauses.Date), 0) between poe.BeginDate And poe.EndDate " &
                                " INNER Join sysrovwSecurity_PermissionOverFeatures pof With (NOLOCK) On pof.IDPassport = " & oState.IDPassport & " And pof.FeatureType = 'U' AND pof.FeatureAlias = 'Calendar.JustifyIncidences' AND Permission >3 " &
                         "WHERE " &
                              "EmployeeGroups.BeginDate <= DailyCauses.Date AND " &
                              "EmployeeGroups.EndDate >= DailyCauses.Date AND " &
                              "DailyCauses.IDCause = 0 AND " &
                              "Date >=" & roTypes.Any2Time(xBegin.Date).SQLSmallDateTime & " AND " &
                              "Date <=" & roTypes.Any2Time(xEnd.Date).SQLSmallDateTime & " "
                If intIDGroup > 0 Then
                    strSQL &= " AND (PATINDEX('%\" & intIDGroup.ToString & "\%', Path) > 0 OR " &
                                " PATINDEX('" & intIDGroup.ToString & "\%', Path) > 0 OR " &
                                "IDGroup = " & intIDGroup.ToString & ") "
                End If
                If strIDEmployees <> "" Then
                    strSQL &= " AND Employees.ID IN (" & strIDEmployees & ") "
                End If

                strSQL &= "GROUP BY DailyCauses.Date, Employees.Name, DailyCauses.IDEmployee, Groups.Name"

                oRet = CreateDataTableWithoutTimeouts(strSQL, , "IncompleteIncidences")
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetIncompletIncidences")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetIncompletIncidences")
            End Try

            Return oRet

        End Function

        Public Shared Function GetBradfordFactorDX(ByVal intIDEmployee As Integer) As Double

            Dim BradfordFactor As Double = 0
            Dim strShiftInfo As String = String.Empty
            Dim Result As New roCollection
            Dim intS As Integer 'Total de ocurrencias
            Dim intD As Integer 'Total de días
            Dim Fecha As String
            Dim ssqlmoves As String
            Dim ssqlaus As String
            Dim bNextAbsenceIsNewBlock As Boolean
            Dim sSQL As String

            Try
                bNextAbsenceIsNewBlock = True
                sSQL = "@select# * from dailyschedule where idemployee = " & intIDEmployee & " and date between " & roTypes.Any2Time(Now).Substract(13, "ww").SQLSmallDateTime & " and getdate() order by date asc"

                Dim tbDailySchedule As DataTable = CreateDataTable(sSQL)
                If tbDailySchedule IsNot Nothing AndAlso tbDailySchedule.Rows.Count > 0 Then
                    For Each oRow As DataRow In tbDailySchedule.Rows
                        Fecha = roTypes.Any2Time(oRow("Date")).SQLSmallDateTime
                        ' Miro si hoy tengo ausencia
                        ssqlmoves = "@select# count(*) from Punches where idemployee = " & intIDEmployee & " and shiftdate = " & Fecha
                        If roTypes.Any2Integer(ExecuteScalar(ssqlmoves)) = 0 Then
                            ssqlaus = "@select# expectedworkinghours from shifts where id = " & oRow("idshiftused")
                            If roTypes.Any2Double(ExecuteScalar(ssqlaus)) > 0 Then
                                ' Tengo incidencia de ausencia, luego debo contabilizar este dia
                                intD = intD + 1
                                If bNextAbsenceIsNewBlock Then intS = intS + 1
                                bNextAbsenceIsNewBlock = False
                            End If
                        Else
                            ' Hoy han habido movimientos, luego la próxima ausencia inicia ciclo
                            bNextAbsenceIsNewBlock = True
                        End If
                        'Preparo para el día siguiente
                        ' PENDIENTE: Estoy dando por hecho que todos los días están planificados (días consecutivos). Hay que tratar el caso en el que no sea así.
                    Next
                    BradfordFactor = (intS * intS * intD)
                End If
            Catch ex As DbException
            Catch ex As Exception
            Finally

            End Try

            Return BradfordFactor
        End Function

#End Region

    End Class

    <DataContract()>
    Public Class roIncidenceList

#Region "Declarations - Constructors"

        Private oState As roIncidenceState

        Private Items As ArrayList

        Public Sub New()
            Me.oState = New roIncidenceState()
            Me.Items = New ArrayList
        End Sub

        Public Sub New(ByVal _State As roIncidenceState)
            Me.oState = _State
            Me.Items = New ArrayList
        End Sub

#End Region

#Region "Properties "

        <DataMember()>
        <XmlArray("Incidences"), XmlArrayItem("roIncidence", GetType(roIncidence))>
        Public Property Incidences() As ArrayList
            Get
                Return Me.Items
            End Get
            Set(ByVal value As ArrayList)
                Me.Items = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function LoadData() As Boolean

            Dim bolRet As Boolean = False

            Me.Items = New ArrayList

            Dim tb As DataTable = Nothing
            Try

                Dim strSQL As String = "@SELECT# * FROM sysroDailyIncidencesTypes "
                tb = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    For Each oRow As DataRow In tb.Rows
                        Me.Items.Add(New roIncidence(oRow("ID"), Me.oState))
                    Next
                End If

                bolRet = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roIncidenceList::LoadData")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roIncidenceList::LoadData")
            Finally

            End Try

            Return bolRet

        End Function

        Public Function GetIncidences(Optional ByVal strWhere As String = "") As DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# * FROM sysroDailyIncidencesTypes "
                If strWhere <> "" Then strSQL &= "WHERE " & strWhere
                oRet = CreateDataTable(strSQL)
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roIncidenceList::GetIncidences")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roIncidenceList::GetIncidences")
            End Try

            Return oRet

        End Function

        Public Function GetIncidencesDescription(Optional ByVal strWhere As String = "") As DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# * FROM sysroDailyIncidencesDescription "
                If strWhere <> "" Then strSQL &= "WHERE " & strWhere
                oRet = CreateDataTable(strSQL)
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roIncidenceList::GetIncidences")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roIncidenceList::GetIncidences")
            End Try

            Return oRet

        End Function

#End Region

    End Class

End Namespace