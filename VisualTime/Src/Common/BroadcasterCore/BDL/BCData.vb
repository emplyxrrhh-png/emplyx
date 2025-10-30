Imports System.Data
Imports System.Data.Common
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase

Namespace DataLogicLayer

    Public Class BCData

        Public Function GetRelays(ByVal TerminalID As Integer) As Data.DataTable
            Dim oDataTable As DataTable = Nothing

            Dim sSQL As String
            Try
                sSQL = "@SELECT# AccessGroupsPermissions.IDAccessGroup, TerminalReaders.Output,TerminalReaders.Mode"
                sSQL += " FROM TerminalReaders WITH (NOLOCK) INNER JOIN"
                sSQL += " AccessGroupsPermissions WITH (NOLOCK) ON TerminalReaders.IDZone = AccessGroupsPermissions.IDZone"
                sSQL += $" WHERE IDTerminal= {TerminalID} AND ISNULL(TerminalReaders.Output, 0) > 0"
                sSQL += " ORDER BY AccessGroupsPermissions.IDAccessGroup"
                oDataTable = CreateDataTable(sSQL)
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "BCData::GetRelays: Unexpected error: ", ex)
            End Try
            Return oDataTable
        End Function

        Public Function GetSirens(ByVal TerminalID As Integer) As Data.DataSet
            Dim oDataSet As DataSet = Nothing

            Dim sSQL As String
            Try
                sSQL = " @SELECT# Weekday as DayOf, [Hour] as StartDate, Duration"
                sSQL += " FROM TerminalSirens WITH (NOLOCK)"
                sSQL += $" WHERE IDTerminal = {TerminalID} "
                sSQL += " ORDER BY Weekday"
                sSQL += ";"
                sSQL += "@SELECT# ZonesInactivity.WeekDay as DayOf, ZonesInactivity.[Begin] as StartDate,"
                sSQL += " ZonesInactivity.[End] as FinishDate, ISNULL(TerminalReaders.[Output], 0) as OutP"
                sSQL += " FROM  ZonesInactivity WITH (NOLOCK), TerminalReaders WITH (NOLOCK) "
                sSQL += $" WHERE ZonesInactivity.IDZone= TerminalReaders.IDZone AND TerminalReaders.IDTerminal= {TerminalID} AND ISNULL(TerminalReaders.[Output],0) > 0 "
                sSQL += " ORDER BY ZonesInactivity.WeekDay"
                sSQL += ";"
                sSQL += "@SELECT# ZonesInactivity.WeekDay as DayOf, ZonesInactivity.[Begin] as StartDate,"
                sSQL += " ZonesInactivity.[End] as FinishDate, TerminalReadersMZA.[Output] as OutP"
                sSQL += " FROM  ZonesInactivity WITH (NOLOCK), TerminalReadersMZA WITH (NOLOCK) "
                sSQL += $" WHERE ZonesInactivity.IDZone= TerminalReadersMZA.IDZone AND TerminalReadersMZA.IDTerminal= {TerminalID} AND ISNULL(TerminalReadersMZA.[Output],0) > 0"
                sSQL += " ORDER BY ZonesInactivity.WeekDay"
                oDataSet = CreateDataSet(sSQL)
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "BCData::GetSirens: Unexpected error: ", ex)
            End Try
            Return oDataSet
        End Function

        Public Function GetTimeZones(ByVal TerminalID As Integer) As Data.DataSet
            Dim oDataSet As DataSet = Nothing
            Try
                Dim sSQL As String
                sSQL = " @SELECT# TR.ID as ReaderID, AG.ID as GroupID, AGP.ID as PeriodID, AGPD.DayOfWeek, AGPD.BeginTime, AGPD.EndTime "
                sSQL += " FROM dbo.TerminalReaders TR WITH (NOLOCK) INNER JOIN dbo.Zones Z WITH (NOLOCK) ON "
                sSQL += " TR.IDZone = Z.ID INNER JOIN dbo.AccessGroupsPermissions AGPZ WITH (NOLOCK) ON "
                sSQL += " Z.ID = AGPZ.IDZone INNER JOIN dbo.AccessGroups AG WITH (NOLOCK) ON"
                sSQL += " AGPZ.IDAccessGroup = AG.ID INNER JOIN dbo.AccessPeriodDaily AGPD WITH (NOLOCK) ON "
                sSQL += " AGPZ.IDAccessPeriod = AGPD.IDAccessPeriod INNER JOIN dbo.AccessPeriods AGP WITH (NOLOCK) ON "
                sSQL += " AGP.ID = AGPZ.IDAccessPeriod "
                sSQL += $" Where TR.IDTerminal = {TerminalID} "
                sSQL += " And AGPD.DayOfWeek<8"
                sSQL += " ORDER BY AG.ID, AGPD.DayOfWeek"
                sSQL += ";"
                sSQL += " @SELECT# TR.ID as ReaderID, AG.ID as GroupID, AGPH.Day, AGPH.Month, AGPH.BeginTime, AGPH.EndTime, AGPH.IDAccessPeriod "
                sSQL += " FROM dbo.TerminalReaders TR WITH (NOLOCK) INNER JOIN dbo.Zones Z WITH (NOLOCK) ON "
                sSQL += " TR.IDZone = Z.ID INNER JOIN dbo.AccessGroupsPermissions AGPZ WITH (NOLOCK) ON "
                sSQL += " Z.ID = AGPZ.IDZone INNER JOIN dbo.AccessGroups AG WITH (NOLOCK) ON"
                sSQL += " AGPZ.IDAccessGroup = AG.ID INNER JOIN dbo.AccessPeriodHolidays AGPH WITH (NOLOCK) ON "
                sSQL += " AGPZ.IDAccessPeriod = AGPH.IDAccessPeriod INNER JOIN dbo.AccessPeriods AGP WITH (NOLOCK) ON "
                sSQL += " AGP.ID = AGPZ.IDAccessPeriod "
                sSQL += $" Where TR.IDTerminal = {TerminalID} "
                sSQL += " And agph.Month <> 0"
                sSQL += " ORDER BY AG.ID, AGPH.Day, AGPH.Month"
                sSQL += ";"
                sSQL += " @SELECT# TR.ID as ReaderID, AG.ID as GroupID, EAA.Date, AGPH.BeginTime, AGPH.EndTime "
                sSQL += " FROM dbo.TerminalReaders TR WITH (NOLOCK) INNER JOIN dbo.Zones Z WITH (NOLOCK) ON "
                sSQL += " TR.IDZone = Z.ID INNER JOIN dbo.AccessGroupsPermissions AGPZ WITH (NOLOCK) ON "
                sSQL += " Z.ID = AGPZ.IDZone INNER JOIN dbo.AccessGroups AG WITH (NOLOCK) ON"
                sSQL += " AGPZ.IDAccessGroup = AG.ID INNER JOIN dbo.AccessPeriodHolidays AGPH WITH (NOLOCK) ON "
                sSQL += " AGPZ.IDAccessPeriod = AGPH.IDAccessPeriod INNER JOIN dbo.AccessPeriods AGP WITH (NOLOCK) ON "
                sSQL += " AGP.ID = AGPZ.IDAccessPeriod INNER JOIN dbo.EventAccessAuthorization EAA WITH (NOLOCK) ON"
                sSQL += " EAA.IDAuthorization = AGPZ.IDAccessGroup INNER JOIN EventsScheduler ES WITH (NOLOCK) ON"
                sSQL += " EAA.IDEvent = ES.ID"
                sSQL += $" Where TR.IDTerminal = {TerminalID} "
                sSQL += " And agph.Month = 0"
                sSQL += " ORDER BY AG.ID, AGPH.Day, AGPH.Month"

                oDataSet = CreateDataSet(sSQL)
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "BCData::GetTimeZones: Unexpected error: ", ex)
            End Try
            Return oDataSet
        End Function

        Public Function GetTimeZonesMxa(ByVal TerminalID As Integer, Optional strTimeZonesWhere As String = "") As Data.DataSet
            Dim oDataSet As DataSet = Nothing
            Try
                Dim sSQL As String
                sSQL = " @SELECT# TR.ID as ReaderID, AG.ID as GroupID, AGP.ID as PeriodID, AGPD.DayOfWeek, AGPD.BeginTime, AGPD.EndTime "
                sSQL += " FROM dbo.TerminalReaders TR WITH (NOLOCK) INNER JOIN dbo.Zones Z WITH (NOLOCK) ON "
                sSQL += " TR.IDZone = Z.ID INNER JOIN dbo.AccessGroupsPermissions AGPZ WITH (NOLOCK) ON "
                sSQL += " Z.ID = AGPZ.IDZone INNER JOIN dbo.AccessGroups AG WITH (NOLOCK) ON"
                sSQL += " AGPZ.IDAccessGroup = AG.ID INNER JOIN dbo.AccessPeriodDaily AGPD WITH (NOLOCK) ON "
                sSQL += " AGPZ.IDAccessPeriod = AGPD.IDAccessPeriod INNER JOIN dbo.AccessPeriods AGP WITH (NOLOCK) ON "
                sSQL += " AGP.ID = AGPZ.IDAccessPeriod "
                sSQL += $" Where TR.IDTerminal = {TerminalID} "
                sSQL += $" And AGPD.DayOfWeek<8 {IIf(strTimeZonesWhere <> "", $" AND AGP.ID IN ({strTimeZonesWhere})", "")} "
                sSQL += " ORDER BY AG.ID, AGPD.DayOfWeek"
                sSQL += ";"
                sSQL += " @SELECT# TR.ID as ReaderID, AG.ID as GroupID, AGPH.Day, AGPH.Month, AGPH.BeginTime, AGPH.EndTime, AGPH.IDAccessPeriod "
                sSQL += " FROM dbo.TerminalReaders TR WITH (NOLOCK) INNER JOIN dbo.Zones Z WITH (NOLOCK) ON "
                sSQL += " TR.IDZone = Z.ID INNER JOIN dbo.AccessGroupsPermissions AGPZ WITH (NOLOCK) ON "
                sSQL += " Z.ID = AGPZ.IDZone INNER JOIN dbo.AccessGroups AG WITH (NOLOCK) ON"
                sSQL += " AGPZ.IDAccessGroup = AG.ID INNER JOIN dbo.AccessPeriodHolidays AGPH WITH (NOLOCK) ON "
                sSQL += " AGPZ.IDAccessPeriod = AGPH.IDAccessPeriod INNER JOIN dbo.AccessPeriods AGP WITH (NOLOCK) ON "
                sSQL += " AGP.ID = AGPZ.IDAccessPeriod "
                sSQL += $" Where TR.IDTerminal = {TerminalID} "
                sSQL += " And agph.Month <> 0"
                sSQL += " ORDER BY AG.ID, AGPH.Day, AGPH.Month"
                sSQL += ";"
                sSQL += " @SELECT# TR.ID as ReaderID, AG.ID as GroupID, EAA.Date, AGPH.BeginTime, AGPH.EndTime "
                sSQL += " FROM dbo.TerminalReaders TR WITH (NOLOCK) INNER JOIN dbo.Zones Z WITH (NOLOCK) ON "
                sSQL += " TR.IDZone = Z.ID INNER JOIN dbo.AccessGroupsPermissions AGPZ WITH (NOLOCK) ON "
                sSQL += " Z.ID = AGPZ.IDZone INNER JOIN dbo.AccessGroups AG WITH (NOLOCK) ON"
                sSQL += " AGPZ.IDAccessGroup = AG.ID INNER JOIN dbo.AccessPeriodHolidays AGPH WITH (NOLOCK) ON "
                sSQL += " AGPZ.IDAccessPeriod = AGPH.IDAccessPeriod INNER JOIN dbo.AccessPeriods AGP WITH (NOLOCK) ON "
                sSQL += " AGP.ID = AGPZ.IDAccessPeriod INNER JOIN dbo.EventAccessAuthorization EAA WITH (NOLOCK) ON"
                sSQL += " EAA.IDAuthorization = AGPZ.IDAccessGroup INNER JOIN EventsScheduler ES WITH (NOLOCK) ON"
                sSQL += " EAA.IDEvent = ES.ID"
                sSQL += $" Where TR.IDTerminal = {TerminalID} "
                sSQL += " And agph.Month = 0"
                sSQL += " ORDER BY AG.ID, AGPH.Day, AGPH.Month"

                oDataSet = CreateDataSet(sSQL)
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "BCData::GetTimeZones: Unexpected error: ", ex)
            End Try
            Return oDataSet
        End Function

        Public Function GetAccess(ByVal TerminalID As Integer) As Data.DataSet
            Dim oDataSet As DataSet = Nothing
            Try
                Dim sSQL As String
                sSQL = "@SELECT# distinct AccessGroupsPermissions.IDAccessGroup, TerminalReaders.ID as ReaderID"
                sSQL += " FROM AccessGroupsPermissions INNER JOIN"
                sSQL += " TerminalReaders ON AccessGroupsPermissions.IDZone = TerminalReaders.IDZone"
                sSQL += $" WHERE IDTerminal= {TerminalID} "
                sSQL += " ORDER BY AccessGroupsPermissions.IDAccessGroup"
                sSQL += ";"
                sSQL += "@SELECT# distinct AccessGroupsPermissions.IDAccessGroup, TerminalReadersMZA.ID as ReaderID"
                sSQL += " FROM AccessGroupsPermissions INNER JOIN"
                sSQL += " TerminalReadersMZA ON AccessGroupsPermissions.IDZone = TerminalReadersMZA.IDZone"
                sSQL += $" WHERE IDTerminal= {TerminalID} "
                sSQL += " ORDER BY AccessGroupsPermissions.IDAccessGroup"
                oDataSet = CreateDataSet(sSQL)
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "BCData::GetAccess: Unexpected error: ", ex)
            End Try
            Return oDataSet
        End Function

        Public Function GetEmployeeImage(ByVal _IDEmployee As Integer) As Data.DataRow
            'TODO: OPTIMIZAR NO HACIENDOLA POR EMPLEADO
            Dim oDataTable As DataTable = Nothing
            Try
                Dim sSQL As String
                sSQL = $"@SELECT# image FROM Employees WHERE ID = {_IDEmployee} "

                oDataTable = CreateDataTable(sSQL, )
                If oDataTable.Rows.Count > 0 Then
                    Return oDataTable.Rows(0)
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "BCData::GetEmployeeImage: Unexpected error: ", ex)
            End Try
            Return Nothing
        End Function

        ''' <summary>
        ''' Recupera un string con los id's de justificaciones permitidos por terminal para un empleado dado
        ''' </summary>
        ''' <param name="_IDEmployee"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetEmployeeAllowedCausesForPunch(ByVal _IDEmployee As Integer) As String
            'TODO: OPTIMIZAR NO HACIENDOLA POR EMPLEADO
            Dim sResult As String = ""
            Dim oCauseState As New VTBusiness.Cause.roCauseState
            Dim oData As System.Data.DataTable = Nothing
            Try
                oData = VTBusiness.Cause.roCause.GetCausesByEmployeeInputPermissions(_IDEmployee, oCauseState, )
                For Each row As System.Data.DataRow In oData.Rows
                    If sResult.Length = 0 Then
                        sResult = sResult + row.Item("ID").ToString
                    Else
                        sResult = sResult + "," + row.Item("ID").ToString
                    End If
                Next
                Return sResult
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "BCData::GetEmployeeAllowedCauses: Unexpected error: ", ex)
            Finally
                If oData IsNot Nothing Then oData.Dispose()
            End Try
            Return Nothing
        End Function

        ''' <summary>
        ''' Recupera las autorizaciones de acceso de un empleado para un terminal y lector dados
        ''' </summary>
        ''' <param name="_IDEmployee"></param>
        ''' <param name="_IDTerminal"></param>
        ''' <param name="_IDReader"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetEmployeeAccessAuthorization(ByVal _IDEmployee As Integer, ByVal _IDTerminal As Integer, Optional _IDReader As Integer = 0) As ArrayList
            'TODO: OPTIMIZAR NO HACIENDOLA POR EMPLEADO
            Dim aResult As New ArrayList
            Dim sSQL As String

            Try

                sSQL = "@SELECT# sysrovwAccessAuthorizations.IDAuthorization from AccessGroupsPermissions "
                sSQL += "INNER JOIN sysrovwAccessAuthorizations ON sysrovwAccessAuthorizations.IDAuthorization = AccessGroupsPermissions.IDAccessGroup "
                sSQL += "INNER JOIN TerminalReaders on TerminalReaders.IDZone=AccessGroupsPermissions.IDZone "
                If _IDReader > 0 Then
                    sSQL += $"WHERE(IDEmployee = {_IDEmployee} And IDTerminal = {_IDTerminal} And terminalreaders.ID = {_IDReader})"
                Else
                    sSQL += $"WHERE(IDEmployee = {_IDEmployee} And IDTerminal = {_IDTerminal})"
                End If

                Dim dt As DataTable = CreateDataTable(sSQL, )

                'Si hay limite de empleados buscamos si tiene permiso
                For i As Integer = 0 To dt.Rows.Count - 1
                    aResult.Add(dt.Rows(i).Item(0))
                Next

                Return aResult
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "BCData::GetEmployeeAccessAuthorization: Unexpected error: ", ex)
                Throw ex
            End Try

            Return Nothing

        End Function

        ''' <summary>
        ''' Recupera los niveles de acceso para un empleado en terminales mxAplus
        ''' </summary>
        ''' <param name="_IDTerminal"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetEmployeeAccessLevelMxaAdvancedMode_Data(ByVal _IDTerminal As Integer, ByVal bAdvancedAccessMode As Boolean) As DataTable
            Dim tRet As DataTable
            Dim sSQL As String

            Try

                If bAdvancedAccessMode Then
                    ' En el dataset de empleados no recuperé los grupos de accesos con permiso. Los valido ahora para el empleado y terminal en cuestión
                    sSQL = " @SELECT# AGP.IDAccessPeriod as IDAccessPeriod, Terminalreaders.id as IDReader, Employees.ID as ID "
                    sSQL += " FROM EmployeeContracts WITH (NOLOCK) "
                    sSQL += " RIGHT OUTER JOIN Employees WITH (NOLOCK) ON EmployeeContracts.IDEmployee = Employees.ID "
                    sSQL += " LEFT OUTER JOIN sysroPassports WITH (NOLOCK) ON Employees.ID = sysroPassports.IDEmployee "
                    sSQL += " LEFT OUTER JOIN sysroPassports_AuthenticationMethods pau_card WITH (NOLOCK) ON sysroPassports.ID = pau_card.IDPassport and pau_card.Method=3 and pau_card.Enabled=1 "
                    sSQL += " LEFT OUTER JOIN CardAliases WITH (NOLOCK) ON pau_card.Credential = convert(nvarchar,CardAliases.IDCard)  "
                    sSQL += " LEFT OUTER JOIN sysroPassports_AuthenticationMethods pau_pin WITH (NOLOCK) ON sysroPassports.ID = pau_pin.IDPassport and pau_pin.Method=5 and pau_pin.Enabled=1 "
                    sSQL += " LEFT OUTER JOIN sysroLanguages WITH (NOLOCK) ON sysroPassports.IDLanguage = sysroLanguages.ID "
                    sSQL += " LEFT OUTER JOIN sysrovwAccessAuthorizations WITH (NOLOCK) ON sysrovwAccessAuthorizations.IDEmployee = Employees.ID"
                    sSQL += " INNER JOIN (@SELECT# DISTINCT IDAccessGroup, IDZone, IDAccessPeriod  from AccessGroupsPermissions WITH (NOLOCK), sysrovwAccessAuthorizations WITH (NOLOCK) where AccessGroupsPermissions.IDAccessGroup = sysrovwAccessAuthorizations.IDAuthorization) AGP ON sysrovwAccessAuthorizations.IDAuthorization=AGP.IDAccessGroup  "
                    sSQL += $" INNER JOIN TerminalReaders WITH (NOLOCK) on TerminalReaders.IDZone=AGP.IDZone and TerminalReaders.IDTerminal= {_IDTerminal} "
                    sSQL += " WHERE (dbo.EmployeeContracts.BeginDate < GETDATE()) AND (DATEADD(dd, 0, DATEDIFF(dd, 0, GETDATE())) <= dbo.EmployeeContracts.EndDate)  "
                    sSQL += " GROUP BY AGP.IDAccessPeriod, Terminalreaders.id, Employees.ID  Order by IDAccessPeriod, IDReader"
                Else
                    sSQL = "@SELECT# AGP.IDAccessPeriod , TR.ID as IDReader, agp.IDAccessGroup as ID FROM AccessGroupsPermissions AGP WITH (NOLOCK), Terminalreaders TR WITH (NOLOCK) "
                    sSQL += "WHERE AGP.IDZone = TR.IDZone "
                    sSQL += $"AND tr.idterminal = {_IDTerminal} "
                    sSQL += "ORDER BY IDAccessPeriod, IDReader"
                End If

                sSQL &= DataLayer.SQLServerHint.GetSQLHint(DataLayer.SQLServerHint.SelectHinted.GetEmployeeAccessLevelMxaAdvancedMode_Data)
                tRet = CreateDataTable(sSQL)

                Return tRet
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "BCData::GetEmployeeAccessLevelMxaAdvancedMode_Data: Unexpected error: ", ex)
                Throw ex
            End Try

            Return Nothing

        End Function

        ''' <summary>
        ''' Recupera los niveles de acceso para un empleado en terminales mxAplus
        ''' </summary>
        ''' <param name="_IDTerminal"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetEmployeeAccessLevelMxaAdvancedMode_Ex(ByVal _IDTerminal As Integer, ByRef dAuxTable As DataTable, ByVal ID As Integer, ByRef aVirtualTimeZones As ArrayList, ByVal bAdvancedAccessMode As Boolean) As ArrayList
            Dim aResult As New ArrayList
            Dim iAccessPeriod As Integer = 0
            Dim iReader As Integer = 0
            Dim iAccessLevel As Integer = 0

            Try

                Dim dv1 As DataView = dAuxTable.DefaultView
                dv1.RowFilter = "ID = " & ID.ToString
                dv1.Sort = "IDAccessPeriod ASC,IDReader ASC"

                iReader = 0
                iAccessPeriod = 0
                '1.- Ahora aseguro que un empleado sólo tiene un permiso para un horario dado, calculando por qué lectores puede acceder en ese horario
                For i As Integer = 0 To dv1.ToTable.Rows.Count - 1
                    iAccessPeriod = roTypes.Any2Integer(dv1.ToTable.Rows(i)("IDAccessPeriod"))
                    iReader = roTypes.Any2Integer(dv1.ToTable.Rows(i)("IDReader"))
                    iAccessLevel = iAccessLevel + 10 ^ (iReader - 1)

                    If i + 1 <= dv1.ToTable.Rows.Count - 1 Then
                        If iAccessPeriod <> roTypes.Any2Integer(dv1.ToTable.Rows(i + 1)("IDAccessPeriod")) Then
                            ' El siguiente registro es otro accessperiod. Lo añado
                            aResult.Add(iAccessPeriod.ToString + "@" + iAccessLevel.ToString)
                            iAccessLevel = 0
                        End If
                    Else
                        ' Es el último accessperiod. Lo añado
                        aResult.Add(iAccessPeriod.ToString + "@" + iAccessLevel.ToString)
                    End If
                Next

                Return aResult
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "BCData::GetEmployeeAccessLevelMxaAdvancedMode_Ex: Unexpected error: ", ex)
                Throw ex
            End Try

            Return Nothing

        End Function

        ''' <summary>
        ''' Recupera los niveles de acceso para un empleado en terminales ZK con protocolo PUSH v2 (rxC)
        ''' </summary>
        ''' <param name="_IDTerminal"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetEmployeeTimeZonesZKPush2_Ex(ByVal _IDTerminal As Integer, ByRef dAuxTable As DataTable, ByVal ID As Integer, ByRef aVirtualTimeZones As Dictionary(Of Integer, ArrayList), ByVal bAdvancedAccessMode As Boolean) As ArrayList
            Dim aResult As New ArrayList
            Dim iAccessPeriod As Integer = 0

            Try

                Dim dv1 As DataView = dAuxTable.DefaultView
                dv1.RowFilter = "ID = " & ID.ToString
                dv1.Sort = "IDAccessPeriod ASC,IDReader ASC"

                iAccessPeriod = 0
                '1.- Ahora aseguro que un empleado sólo tiene un permiso para un horario dado, calculando por qué lectores puede acceder en ese horario
                For i As Integer = 0 To dv1.ToTable.Rows.Count - 1
                    iAccessPeriod = roTypes.Any2Integer(dv1.ToTable.Rows(i)("IDAccessPeriod"))
                    If aVirtualTimeZones.ContainsKey(iAccessPeriod) Then
                        aResult.AddRange(aVirtualTimeZones.Item(iAccessPeriod))
                    Else
                        ' Esto no debería ocurrir
                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "BCData::GetEmployeeTimeZonesZKPush2: Error recovering id for virtual timezone. Real id is " + iAccessPeriod.ToString)
                    End If
                Next

                Return aResult
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "BCData::GetEmployeeTimeZonesZKPush2: Unexpected error: ", ex)
                Throw ex
            End Try

            Return Nothing

        End Function

        Public Function GetEmployeeTimeZonesZKPush2_Data(ByVal _IDTerminal As Integer, ByVal bAdvancedAccessMode As Boolean) As DataTable
            Dim sSQL As String
            Dim tRet As DataTable
            Try

                If bAdvancedAccessMode Then
                    ' En el dataset de empleados no recuperé los grupos de accesos con permiso. Los valido ahora para el empleado y terminal en cuestión
                    sSQL = " @SELECT# AGP.IDAccessPeriod as IDAccessPeriod, Terminalreaders.id as IDReader, Employees.ID   as ID  "
                    sSQL += " FROM EmployeeContracts "
                    sSQL += " RIGHT OUTER JOIN Employees WITH (NOLOCK) ON EmployeeContracts.IDEmployee = Employees.ID "
                    sSQL += " LEFT OUTER JOIN sysroPassports WITH (NOLOCK) ON Employees.ID = sysroPassports.IDEmployee "
                    sSQL += " LEFT OUTER JOIN sysroPassports_AuthenticationMethods pau_card WITH (NOLOCK) ON sysroPassports.ID = pau_card.IDPassport and pau_card.Method=3 and pau_card.Enabled=1 "
                    sSQL += " LEFT OUTER JOIN CardAliases WITH (NOLOCK) ON pau_card.Credential = convert(nvarchar,CardAliases.IDCard)  "
                    sSQL += " LEFT OUTER JOIN sysroPassports_AuthenticationMethods pau_pin WITH (NOLOCK) ON sysroPassports.ID = pau_pin.IDPassport and pau_pin.Method=5 and pau_pin.Enabled=1 "
                    sSQL += " LEFT OUTER JOIN sysroLanguages WITH (NOLOCK) ON sysroPassports.IDLanguage = sysroLanguages.ID "
                    sSQL += " LEFT OUTER JOIN sysrovwAccessAuthorizations WITH (NOLOCK) ON sysrovwAccessAuthorizations.IDEmployee = Employees.ID"
                    sSQL += " INNER JOIN (@SELECT# DISTINCT IDAccessGroup, IDZone, IDAccessPeriod  from AccessGroupsPermissions WITH (NOLOCK), sysrovwAccessAuthorizations WITH (NOLOCK) where AccessGroupsPermissions.IDAccessGroup = sysrovwAccessAuthorizations.IDAuthorization) AGP ON sysrovwAccessAuthorizations.IDAuthorization=AGP.IDAccessGroup  "
                    sSQL += $" INNER JOIN TerminalReaders WITH (NOLOCK) on TerminalReaders.IDZone=AGP.IDZone AND TerminalReaders.IDZone > -1 AND TerminalReaders.IDTerminal= {_IDTerminal} "
                    sSQL += " WHERE (dbo.EmployeeContracts.BeginDate < GETDATE()) AND (DATEADD(dd, 0, DATEDIFF(dd, 0, GETDATE())) <= dbo.EmployeeContracts.EndDate)  "
                    sSQL += " Order by IDAccessPeriod, IDReader"
                Else
                    sSQL = "@SELECT# AGP.IDAccessPeriod , TR.ID as IDReader, agp.IDAccessGroup as ID from AccessGroupsPermissions AGP WITH (NOLOCK), Terminalreaders TR WITH (NOLOCK) "
                    sSQL += "where AGP.IDZone = TR.IDZone "
                    sSQL += $"and tr.idterminal = {_IDTerminal} "
                    sSQL += "order by IDAccessPeriod, IDReader"
                End If

                sSQL &= DataLayer.SQLServerHint.GetSQLHint(DataLayer.SQLServerHint.SelectHinted.GetEmployeeTimeZonesZKPush2_Data)

                tRet = CreateDataTable(sSQL)
                Return tRet
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "BCData::GetEmployeeTimeZonesZKPush2_Data: Unexpected error: ", ex)
                Throw ex
            End Try

            Return Nothing

        End Function

        Public Function GetEmployeesLive(ByVal _IDTerminal As Integer, ByVal bAdvancedAccessMode As Boolean, Optional ByVal _CheckAcc As Boolean = False, Optional bPermissionsOverEmployeeWhenAdvanced As Boolean = False) As Data.DataTable
            Dim oDataTable As DataTable = Nothing

            Try

                Dim sSQL As String
                If Not bAdvancedAccessMode OrElse Not _CheckAcc Then
                    ' Modo de accesos compatibilidad (un empleado sólo puede pertenecer a un grupo de accesos
                    sSQL = " @SELECT# distinct dbo.Employees.ID as EmployeeID, dbo.Employees.IDAccessGroup as GroupID, dbo.Employees.Alias, dbo.Employees.Name, "
                    sSQL += " sysroLanguages.LanguageKey, dbo.Employees.AccControlled, pau_card.credential as IDCard, cardaliases.realvalue as RealValue,"
                    sSQL += " pau_pin.Password AS PINData, sysroPassports.AuthenticationMerge "
                    sSQL += " FROM EmployeeContracts WITH (NOLOCK)"
                    sSQL += " RIGHT OUTER JOIN Employees WITH (NOLOCK) ON EmployeeContracts.IDEmployee = Employees.ID "
                    sSQL += " LEFT OUTER JOIN sysroPassports WITH (NOLOCK) ON Employees.ID = sysroPassports.IDEmployee"
                    sSQL += " LEFT OUTER JOIN sysroPassports_AuthenticationMethods pau_card WITH (NOLOCK) "
                    sSQL += " ON sysroPassports.ID = pau_card.IDPassport and pau_card.Method=3 and pau_card.Enabled=1"
                    sSQL += " LEFT OUTER JOIN CardAliases WITH (NOLOCK) ON pau_card.Credential = convert(nvarchar,CardAliases.IDCard) "
                    sSQL += " LEFT OUTER JOIN sysroPassports_AuthenticationMethods pau_pin WITH (NOLOCK)"
                    sSQL += " ON sysroPassports.ID = pau_pin.IDPassport and pau_pin.Method=5 and pau_pin.Enabled=1"
                    sSQL += " LEFT OUTER JOIN sysroLanguages WITH (NOLOCK) ON sysroPassports.IDLanguage = sysroLanguages.ID"
                    If _CheckAcc Then
                        sSQL += " INNER JOIN (@SELECT# DISTINCT IDAccessGroup, IDZone FROM AccessGroupsPermissions WITH (NOLOCK)) AGP ON Employees.IDAccessGroup=AGP.IDAccessGroup "
                        sSQL += $" INNER JOIN TerminalReaders WITH (NOLOCK) on TerminalReaders.IDZone=AGP.IDZone AND TerminalReaders.IDZone > -1 AND TerminalReaders.IDTerminal= {_IDTerminal} "
                    End If
                    sSQL += " WHERE (dbo.EmployeeContracts.BeginDate < GETDATE()) AND (DATEADD(dd, 0, DATEDIFF(dd, 0, GETDATE())) <= dbo.EmployeeContracts.EndDate) "
                    sSQL += " Order by EmployeeID"
                Else
                    If Not bPermissionsOverEmployeeWhenAdvanced Then
                        ' Modo avanzado de accesos. Un empleado puede disponer de múltiples autorizaciones de acceso
                        sSQL = " @SELECT# distinct dbo.Employees.ID as EmployeeID, AGP.IDAccessGroup as GroupID, dbo.Employees.Alias, dbo.Employees.Name,  sysroLanguages.LanguageKey, "
                        sSQL += " dbo.Employees.AccControlled, pau_card.credential as IDCard, cardaliases.realvalue as RealValue, pau_pin.Password AS PINData, sysroPassports.AuthenticationMerge  "
                        sSQL += " FROM EmployeeContracts WITH (NOLOCK) "
                        sSQL += " RIGHT OUTER JOIN Employees WITH (NOLOCK) ON EmployeeContracts.IDEmployee = Employees.ID "
                        sSQL += " LEFT OUTER JOIN sysroPassports WITH (NOLOCK) ON Employees.ID = sysroPassports.IDEmployee "
                        sSQL += " LEFT OUTER JOIN sysroPassports_AuthenticationMethods pau_card WITH (NOLOCK) ON sysroPassports.ID = pau_card.IDPassport and pau_card.Method=3 and pau_card.Enabled=1 "
                        sSQL += " LEFT OUTER JOIN CardAliases WITH (NOLOCK) ON pau_card.Credential = convert(nvarchar,CardAliases.IDCard)  "
                        sSQL += " LEFT OUTER JOIN sysroPassports_AuthenticationMethods pau_pin WITH (NOLOCK) ON sysroPassports.ID = pau_pin.IDPassport and pau_pin.Method=5 and pau_pin.Enabled=1 "
                        sSQL += " LEFT OUTER JOIN sysroLanguages WITH (NOLOCK) ON sysroPassports.IDLanguage = sysroLanguages.ID "
                        sSQL += " LEFT OUTER JOIN sysrovwAccessAuthorizations WITH (NOLOCK) ON sysrovwAccessAuthorizations.IDEmployee = Employees.ID"
                        sSQL += " INNER JOIN (@SELECT# DISTINCT IDAccessGroup, IDZone  from AccessGroupsPermissions WITH (NOLOCK), sysrovwAccessAuthorizations WITH (NOLOCK) where AccessGroupsPermissions.IDAccessGroup = sysrovwAccessAuthorizations.IDAuthorization) AGP ON sysrovwAccessAuthorizations.IDAuthorization=AGP.IDAccessGroup  "
                        sSQL += $" INNER JOIN TerminalReaders WITH (NOLOCK) on TerminalReaders.IDZone=AGP.IDZone AND TerminalReaders.IDZone > -1 AND TerminalReaders.IDTerminal= {_IDTerminal} "
                        sSQL += " WHERE (dbo.EmployeeContracts.BeginDate < GETDATE()) AND (DATEADD(dd, 0, DATEDIFF(dd, 0, GETDATE())) <= dbo.EmployeeContracts.EndDate)  Order by EmployeeID"
                    Else
                        ' Modo avanzado de accesos. Un empleado puede disponer de múltiples autorizaciones de acceso
                        sSQL = " @SELECT# distinct dbo.Employees.ID as EmployeeID, dbo.Employees.Alias, dbo.Employees.Name,  sysroLanguages.LanguageKey, "
                        sSQL += " dbo.Employees.AccControlled, pau_card.credential as IDCard, cardaliases.realvalue as RealValue, pau_pin.Password AS PINData, sysroPassports.AuthenticationMerge  "
                        sSQL += " FROM EmployeeContracts WITH (NOLOCK) "
                        sSQL += " RIGHT OUTER JOIN Employees WITH (NOLOCK) ON EmployeeContracts.IDEmployee = Employees.ID "
                        sSQL += " LEFT OUTER JOIN sysroPassports WITH (NOLOCK) ON Employees.ID = sysroPassports.IDEmployee "
                        sSQL += " LEFT OUTER JOIN sysroPassports_AuthenticationMethods pau_card WITH (NOLOCK) ON sysroPassports.ID = pau_card.IDPassport and pau_card.Method=3 and pau_card.Enabled=1 "
                        sSQL += " LEFT OUTER JOIN CardAliases WITH (NOLOCK) ON pau_card.Credential = convert(nvarchar,CardAliases.IDCard)  "
                        sSQL += " LEFT OUTER JOIN sysroPassports_AuthenticationMethods pau_pin WITH (NOLOCK) ON sysroPassports.ID = pau_pin.IDPassport and pau_pin.Method=5 and pau_pin.Enabled=1 "
                        sSQL += " LEFT OUTER JOIN sysroLanguages WITH (NOLOCK) ON sysroPassports.IDLanguage = sysroLanguages.ID "
                        sSQL += " LEFT OUTER JOIN sysrovwAccessAuthorizations WITH (NOLOCK) ON sysrovwAccessAuthorizations.IDEmployee = Employees.ID"
                        sSQL += " INNER JOIN (@SELECT# DISTINCT IDAccessGroup, IDZone  from AccessGroupsPermissions WITH (NOLOCK), sysrovwAccessAuthorizations WITH (NOLOCK) where AccessGroupsPermissions.IDAccessGroup = sysrovwAccessAuthorizations.IDAuthorization) AGP ON sysrovwAccessAuthorizations.IDAuthorization=AGP.IDAccessGroup  "
                        sSQL += $" INNER JOIN TerminalReaders WITH (NOLOCK) on TerminalReaders.IDZone=AGP.IDZone AND TerminalReaders.IDZone > -1 AND TerminalReaders.IDTerminal= {_IDTerminal} "
                        sSQL += " WHERE (dbo.EmployeeContracts.BeginDate < GETDATE()) AND (DATEADD(dd, 0, DATEDIFF(dd, 0, GETDATE())) <= dbo.EmployeeContracts.EndDate)  Order by EmployeeID"
                    End If
                End If

                sSQL &= DataLayer.SQLServerHint.GetSQLHint(DataLayer.SQLServerHint.SelectHinted.GetEmployeesLive)

                oDataTable = CreateDataTable(sSQL, )
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "BCData::GetEmployeesLive: Unexpected error: ", ex)
                Throw ex
            End Try
            Return oDataTable
        End Function

        Public Function GetEmployeesAllBio_Live(ByVal _Version As String, ByVal bAdvancedAccessMode As Boolean, Optional ByVal IDEmployee As Integer = 0, Optional ByVal _IDTerminal As Integer = 0) As Data.DataTable
            'TODO: OPTIMIZAR NO HACIENDOLA POR EMPLEADO
            Dim oDataTable As DataTable = Nothing
            Dim sSQL As String

            Try

                If Not bAdvancedAccessMode OrElse _IDTerminal = 0 Then
                    sSQL = "@SELECT# p.idemployee as EmployeeID, pam.BiometricData as BioData, pam.BiometricID as BiometricID"
                    sSQL += " from sysroPassports p WITH (NOLOCK)"
                    sSQL += " inner join sysroPassports_AuthenticationMethods pam WITH (NOLOCK)"
                    sSQL += $" on p.ID=pam.IDPassport and pam.Method = 4 AND pam.Version = '{_Version}' AND pam.Enabled = 1"
                    sSQL += " LEFT JOIN EmployeeContracts WITH (NOLOCK)"
                    sSQL += " ON p.IDEmployee = EmployeeContracts.IDEmployee "
                    sSQL += " LEFT JOIN Employees e WITH (NOLOCK)"
                    sSQL += " ON p.IDEmployee = e.ID "
                    If _IDTerminal > 0 Then
                        sSQL += " INNER JOIN (@SELECT# DISTINCT IDAccessGroup, IDZone FROM AccessGroupsPermissions WITH (NOLOCK)) AGP ON e.IDAccessGroup=AGP.IDAccessGroup "
                        sSQL += $" INNER JOIN TerminalReaders WITH (NOLOCK) on TerminalReaders.IDZone=AGP.IDZone  AND TerminalReaders.IDZone > -1 and TerminalReaders.IDTerminal= {_IDTerminal} "
                    End If
                    sSQL += " WHERE EmployeeContracts.BeginDate < GETDATE() AND DATEADD(dd, 0, DATEDIFF(dd, 0, GETDATE())) <= dbo.EmployeeContracts.EndDate "
                    If IDEmployee > 0 Then sSQL += $" AND p.idemployee= {IDEmployee} "
                    sSQL += " ORDER BY p.IDEmployee"
                Else
                    sSQL = "@SELECT# p.idemployee as EmployeeID, pam.BiometricData as BioData, pam.BiometricID as BiometricID"
                    sSQL += " from sysroPassports p WITH (NOLOCK)"
                    sSQL += " inner join sysroPassports_AuthenticationMethods pam WITH (NOLOCK)"
                    sSQL += $" on p.ID=pam.IDPassport and pam.Method = 4 AND pam.Version = '{_Version}' AND pam.Enabled = 1"
                    sSQL += " LEFT JOIN EmployeeContracts WITH (NOLOCK) "
                    sSQL += " ON p.IDEmployee = EmployeeContracts.IDEmployee "
                    sSQL += " LEFT JOIN Employees e WITH (NOLOCK)"
                    sSQL += " ON p.IDEmployee = e.ID "
                    sSQL += " LEFT OUTER JOIN sysrovwAccessAuthorizations WITH (NOLOCK) ON sysrovwAccessAuthorizations.IDEmployee = p.IDEmployee "
                    sSQL += " INNER JOIN (@SELECT# DISTINCT IDAccessGroup, IDZone  from AccessGroupsPermissions WITH (NOLOCK), sysrovwAccessAuthorizations WITH (NOLOCK) where AccessGroupsPermissions.IDAccessGroup = sysrovwAccessAuthorizations.IDAuthorization) AGP ON sysrovwAccessAuthorizations.IDAuthorization=AGP.IDAccessGroup  "
                    sSQL += $" INNER JOIN TerminalReaders WITH (NOLOCK) on TerminalReaders.IDZone=AGP.IDZone  AND TerminalReaders.IDZone > -1 and TerminalReaders.IDTerminal= {_IDTerminal} "
                    sSQL += " WHERE EmployeeContracts.BeginDate < GETDATE() AND DATEADD(dd, 0, DATEDIFF(dd, 0, GETDATE())) <= dbo.EmployeeContracts.EndDate "
                    If IDEmployee > 0 Then sSQL += $" AND p.idemployee= {IDEmployee} "
                    sSQL += " ORDER BY p.IDEmployee"
                End If

                oDataTable = CreateDataTable(sSQL)
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "BCData::GetEmployeesBioRx: Unexpected error: ", ex)
                Throw ex
            End Try
            Return oDataTable
        End Function

        Public Function GetCauses(Optional avoidReaderInputCondeZero As Boolean = False) As Data.DataTable
            Dim oDataTable As DataTable = Nothing
            Try
                Dim sSQL As String
                sSQL = $"@SELECT# [ID], ReaderInputCode, Name, WorkingType FROM Causes 
                         WHERE [ID] > 0 AND AllowInputFromReader = 1 {If(avoidReaderInputCondeZero, " AND ReaderInputCode <> 0 ", "")}
                         ORDER BY Name"

                oDataTable = CreateDataTable(sSQL)
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "BCData::GetCauses: Unexpected error: ", ex)
            End Try
            Return oDataTable
        End Function

        Public Function SomeCausesDependsOnEmployee() As Boolean
            Dim iCount As Integer = 0
            Try
                Dim sSQL As String
                sSQL = "@SELECT# count(*)  FROM Causes " &
                       "WHERE [ID] > 0 AND AllowInputFromReader = 1 AND InputPermissions = 2"
                iCount = roTypes.Any2Integer(ExecuteScalar(sSQL))

                Return iCount > 0
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "BCData::SomeCausesDependsOnEmployee: Unexpected error: ", ex)
                Return False
            End Try
        End Function

        Public Function GetTerminalReaders(ByVal TerminalID As Integer) As Data.DataTable
            Dim oDataTable As DataTable = Nothing

            Try
                Dim sSql As String
                sSql = "@SELECT# * FROM TerminalReaders " &
                       " LEFT OUTER JOIN Terminals ON Terminals.ID = TerminalReaders.IdTerminal  " &
                       $"WHERE TerminalReaders.IDTerminal = {TerminalID} " &
                       "ORDER BY TerminalReaders.IdTerminal, TerminalReaders.ID"
                oDataTable = CreateDataTable(sSql)
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "BCData::GetTerminalReaders: Unexpected error: ", ex)
            End Try
            Return oDataTable
        End Function

    End Class

End Namespace