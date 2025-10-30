Imports System.Linq
Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.VTBase
Imports System.Text
Imports Robotics.DataLayer
Imports System.Web
Imports Robotics.Base.VTSelectorManager.roSelectorManager

Public Class roSelector


    Public Shared Sub GetExpandedGroupsAndEmployees(ByVal strEmployeeFilter As String, ByRef lstEmployeeSelection As Generic.List(Of Integer), ByRef lstGroupSelection As Generic.List(Of Integer))


        Try
            Dim strEmployees As String = ""
            Dim strGroups As String = ""

            If lstEmployeeSelection IsNot Nothing Then
                lstEmployeeSelection.Clear()
            Else
                lstEmployeeSelection = New Generic.List(Of Integer)
            End If

            If lstGroupSelection IsNot Nothing Then
                lstGroupSelection.Clear()
            Else
                lstGroupSelection = New Generic.List(Of Integer)
            End If

            BuildSelectionFilterFromSelector(strEmployeeFilter, strEmployees, strGroups)

            If strGroups.Length > 0 AndAlso strGroups <> "-1" Then
                Dim sSQL As String = $"@select# pg.ID from groups 
		                    inner JOIN Groups pg ON pg.path = groups.path OR pg.path LIKE groups.path + '\%' where groups.id in({strGroups}) OR '{strGroups}' = '-2' "

                Dim dt As DataTable = AccessHelper.CreateDataTable(sSQL)
                If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                    lstGroupSelection = New List(Of Integer)
                    lstGroupSelection.AddRange(dt.AsEnumerable().Select(Function(x) Integer.Parse(x("ID").ToString())))
                End If
            End If

            If strEmployees <> String.Empty AndAlso strEmployees <> "-1" Then
                lstEmployeeSelection = New List(Of Integer)
                lstEmployeeSelection.AddRange(strEmployees.Split(",").Select(Function(x) Integer.Parse(x)))
            End If



        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roSelector_GetExpandedGroupsAndEmployees::Unkown exeception", ex)
        End Try

    End Sub



    Public Shared Function BuildSelectionStringFromIDs(ByVal lstEmployeeSelection As Integer(), ByVal lstGroupSelection As Integer()) As String

        Dim strSelection As New StringBuilder

        If lstEmployeeSelection IsNot Nothing Then
            For Each iEmp As Integer In lstEmployeeSelection
                strSelection.Append($"B{iEmp},")
            Next
        End If

        If lstGroupSelection IsNot Nothing Then
            For Each iGroup As Integer In lstGroupSelection
                strSelection.Append($"A{iGroup},")
            Next
        End If

        If (strSelection.Length > 0) Then
            Return strSelection.ToString(0, strSelection.Length - 1)
        Else
            Return String.Empty
        End If

    End Function






    Public Shared Function GetEmployeeCount(ByVal idPassport As Integer, ByVal sFeature As String, ByVal sFeatureType As String,
                                                          ByVal strEmployeeFilter As String, ByVal strEmployeeUserFieldsFilter As String,
                                                          ByVal bAddOnlyDirectEmployees As Boolean, ByVal xDate As DateTime) As Integer

        Dim iEmployeeCount As Integer = 0

        Try

            ' 01. Obtenemos los criterios de seleccion
            Dim conf As String() = strEmployeeFilter.Split("¬")

            Dim EmployeeIDs As New Generic.List(Of Integer)
            Dim GroupsIDs As New Generic.List(Of Integer)

            Dim intIDGroup As Integer
            Dim intIDEmployee As Integer

            ' Empleados y grupos
            strEmployeeFilter = conf(0)

            For Each strID As String In strEmployeeFilter.Split(",")
                If strID.StartsWith("B") Then
                    ' Empleado seleccionado de forma directa
                    intIDEmployee = strID.Substring(1)
                    If Not EmployeeIDs.Contains(intIDEmployee) Then
                        EmployeeIDs.Add(intIDEmployee)
                    End If

                ElseIf strID.StartsWith("A") Then
                    ' Obtenemos los grupos seleccionados de forma directa
                    intIDGroup = strID.Substring(1)
                    If Not GroupsIDs.Contains(intIDGroup) Then
                        GroupsIDs.Add(intIDGroup)
                    End If
                End If
            Next

            Dim strSelection As String = BuildSelectionStringFromIDs(EmployeeIDs.ToArray, GroupsIDs.ToArray)
            iEmployeeCount = GetEmployeeListByContract(idPassport, sFeature, sFeatureType, Nothing, strSelection, strEmployeeUserFieldsFilter, bAddOnlyDirectEmployees, xDate, xDate).Count()


        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roSelector_GetEmployeeCount:: Unkown exeception", ex)
        End Try

        Return iEmployeeCount

    End Function

    Public Shared Function GetEmployeeListByContract(ByVal idPassport As Integer, ByVal sFeature As String, ByVal sFeatureType As String,
                                                     ByVal eRequieredPermission As Nullable(Of Permission), ByVal strEmployeeFilter As String,
                                                     ByVal strEmployeeUserFieldsFilter As String, ByVal bAddOnlyDirectEmployees As Boolean,
                                                     ByVal dateInf As Nullable(Of Date), ByVal dateSup As Nullable(Of Date)) As Generic.List(Of Integer)

        Dim retEmployeListIDs As Generic.List(Of Integer) = Nothing
        Try

            Dim oFilter As New roSelectorFilter() With {
                .UserFields = strEmployeeUserFieldsFilter,
                .ComposeFilter = strEmployeeFilter,
                .Filters = "",
                .Operation = "OR",
                .ComposeMode = "custom"
            }

            Dim oBehaviour As New roSelectorFilterContext With {
                .IdPassport = idPassport,
                .Feature = sFeature,
                .FeatureType = sFeatureType,
                .RequieredPermission = eRequieredPermission,
                .AddOnlyDirectEmployees = bAddOnlyDirectEmployees,
                .DateInf = dateInf,
                .DateSup = dateSup
            }

            Dim tb As DataTable = DataLayer.AccessHelper.CreateDataTable($"@SELECT# * FROM {GetEmployeeListByContractOnTemporalTableName(oFilter, oBehaviour)}")
            If tb IsNot Nothing Then
                retEmployeListIDs = tb.AsEnumerable().Select(Function(row) roTypes.Any2Integer(row("id"))).ToList()
            Else
                retEmployeListIDs = New List(Of Integer)
            End If


        Catch ex As Exception
            retEmployeListIDs = New List(Of Integer)
            roLog.GetInstance().logMessage(roLog.EventType.roError, "GetEmployeesInPeriodList:: Unkown exeception", ex)
        End Try

        Return retEmployeListIDs

    End Function

    Public Shared Function GetEmployeeList(ByVal idPassport As Integer, ByVal sFeature As String, ByVal sFeatureType As String,
                                           ByVal eRequieredPermission As Nullable(Of Permission), ByVal strEmployeeFilter As String,
                                           ByVal strEmployeeStatus As String, ByVal strEmployeeUserFieldsFilter As String,
                                           ByVal bAddOnlyDirectEmployees As Boolean, ByVal dateInf As Nullable(Of Date), ByVal dateSup As Nullable(Of Date)) As Generic.List(Of Integer)
        Dim retEmployeListIDs As Generic.List(Of Integer) = Nothing

        Try
            Dim oFilter As New roSelectorFilter() With {
                .UserFields = strEmployeeUserFieldsFilter,
                .ComposeFilter = strEmployeeFilter,
                .Filters = strEmployeeStatus,
                .Operation = "OR",
                .ComposeMode = "custom"
            }

            Dim oBehaviour As New roSelectorFilterContext With {
                .IdPassport = idPassport,
                .Feature = sFeature,
                .FeatureType = sFeatureType,
                .RequieredPermission = eRequieredPermission,
                .AddOnlyDirectEmployees = bAddOnlyDirectEmployees,
                .DateInf = dateInf,
                .DateSup = dateSup
            }

            Dim tb As DataTable = DataLayer.AccessHelper.CreateDataTable($"@SELECT# * FROM {GetEmployeeListOnTemporalTableName(oFilter, oBehaviour)}")
            If tb IsNot Nothing Then
                retEmployeListIDs = tb.AsEnumerable().Select(Function(row) roTypes.Any2Integer(row("ID"))).ToList()
            Else
                retEmployeListIDs = New List(Of Integer)
            End If


        Catch ex As Exception
            retEmployeListIDs = New List(Of Integer)()
            roLog.GetInstance().logMessage(roLog.EventType.roError, "GetEmployeeList:: Unkown exeception", ex)
        End Try

        Return retEmployeListIDs

    End Function


    Public Shared Function GetEmployeeListByContractOnTemporalTableName(ByVal oFilter As roSelectorFilter, ByVal oBehaviour As roSelectorFilterContext) As String

        Dim tmpEmployeesTableName As String
        Dim strEmployeeStatus As String = ""

        If oFilter.Filters <> String.Empty Then
            strEmployeeStatus = oFilter.Filters
        Else
            strEmployeeStatus = $"1111{If(oFilter.UserFields.Length > 0, "1", "0")}"
        End If

        Try

            Dim sComputedFeature As String = ""
            Dim reqPermission As Permission = Nothing
            If oBehaviour.Feature.Contains("=") Then
                Dim oConf As String() = oBehaviour.Feature.Split("=")

                If oConf.Length > 0 Then
                    sComputedFeature = oConf(0)
                End If

                If oConf.Length > 1 Then
                    Select Case oConf(1).ToUpper
                        Case "WRITE"
                            reqPermission = Permission.Write
                        Case "READ"
                            reqPermission = Permission.Read
                        Case "NONE"
                            reqPermission = Permission.None
                        Case "ADMIN"
                            reqPermission = Permission.Admin
                        Case Else
                            reqPermission = Permission.Read
                    End Select
                End If

                sComputedFeature = oBehaviour.Feature.Split("=")(0)
            Else
                reqPermission = If(oBehaviour.RequieredPermission.HasValue, oBehaviour.RequieredPermission.Value, Permission.Read)
                sComputedFeature = oBehaviour.Feature
            End If


            If Not oBehaviour.DateInf.HasValue OrElse (oBehaviour.DateInf.HasValue And oBehaviour.DateInf = Date.MinValue) Then oBehaviour.DateInf = DateTime.Now.Date
            If Not oBehaviour.DateSup.HasValue OrElse (oBehaviour.DateSup.HasValue And oBehaviour.DateSup = Date.MinValue) Then oBehaviour.DateSup = DateTime.Now.Date


            Dim strSQL As String
            Dim filterTableName As String = GetEmpoyeesJoinTableWithoutPermissions(oFilter.ComposeFilter, strEmployeeStatus, oFilter.UserFields, oBehaviour.AddOnlyDirectEmployees)
            If sComputedFeature <> String.Empty Then
                strSQL = $"@SELECT# DISTINCT temp.idemployee from(
                               @SELECT# aeg.idemployee, aeg.idgroup, 
	                              case when ec.BeginDate >= aeg.begindate then ec.BeginDate else aeg.begindate end as mindate,
	                              case when ec.EndDate >= aeg.Enddate then aeg.Enddate else ec.enddate end as maxdate,  aeg.Path   
                               FROM dbo.EmployeeContracts ec
                               INNER Join sysrovwSecurity_PermissionOverEmployeeAndFeature poe on poe.IDPassport={oBehaviour.IdPassport} And poe.IDEmployee = ec.IDEmployee 
                                            And poe.FeatureAlias='{sComputedFeature}' AND poe.FeatureType='{oBehaviour.FeatureType}' AND poe.FeaturePermission >={CInt(reqPermission)}
                                            And convert(DATE,GETDATE()) between poe.BeginDate And poe.EndDate
                               INNER JOIN dbo.sysrovwEmployees_AllMobilities aeg on ec.IDEmployee = aeg.idemployee and ec.BeginDate <= aeg.enddate and ec.EndDate >= aeg.begindate   
                               INNER JOIN {filterTableName} AS sem ON aeg.idemployee = sem.id and {roTypes.SQLSmallDateTime(oBehaviour.DateInf)} <= sem.EndDate AND {roTypes.SQLSmallDateTime(oBehaviour.DateSup)} >= sem.BeginDate 
                               ) temp where {roTypes.SQLSmallDateTime(oBehaviour.DateInf)} <= temp.maxdate and {roTypes.SQLSmallDateTime(oBehaviour.DateSup)} >= temp.mindate "
            Else
                strSQL = $"@SELECT# DISTINCT temp.idemployee from(
                               @SELECT# aeg.idemployee, aeg.idgroup, 
	                              case when ec.BeginDate >= aeg.begindate then ec.BeginDate else aeg.begindate end as mindate,
	                              case when ec.EndDate >= aeg.Enddate then aeg.Enddate else ec.enddate end as maxdate,  aeg.Path   
                               FROM dbo.EmployeeContracts ec
                               INNER JOIN dbo.sysrovwEmployees_AllMobilities aeg on ec.IDEmployee = aeg.idemployee and ec.BeginDate <= aeg.enddate and ec.EndDate >= aeg.begindate   
                               INNER JOIN {filterTableName} AS sem ON aeg.idemployee = sem.id and {roTypes.SQLSmallDateTime(oBehaviour.DateInf)} <= sem.EndDate AND {roTypes.SQLSmallDateTime(oBehaviour.DateSup)} >= sem.BeginDate
                               ) temp where {roTypes.SQLSmallDateTime(oBehaviour.DateInf)} <= temp.maxdate and {roTypes.SQLSmallDateTime(oBehaviour.DateSup)} >= temp.mindate "
            End If

            tmpEmployeesTableName = $"#esf_{Guid.NewGuid.ToString().Replace("-", "")}"
            AccessHelper.BulkCopyIdFromQuery(tmpEmployeesTableName, strSQL, "IDEmployee")

            If Not String.IsNullOrEmpty(filterTableName) Then AccessHelper.ExecuteSql($"IF OBJECT_ID('tempdb..#{filterTableName}') IS NOT NULL @DROP# TABLE #{filterTableName};")
        Catch ex As Exception
            tmpEmployeesTableName = String.Empty
            roLog.GetInstance().logMessage(roLog.EventType.roError, "GetEmployeesInPeriodList:: Unkown exeception", ex)
        End Try

        Return tmpEmployeesTableName

    End Function

    Public Shared Function GetEmployeeListOnTemporalTableName(ByVal oFilter As roSelectorFilter, ByVal oBehaviour As roSelectorFilterContext) As String

        Dim tmpEmployeesTableName As String

        Try
            Dim sComputedFeature As String = ""
            Dim reqPermission As Permission = Nothing
            If oFilter.Filters = "" OrElse oFilter.Filters.Length < 5 Then oFilter.Filters = "10000"

            If oBehaviour.Feature.Contains("=") Then
                Dim oConf As String() = oBehaviour.Feature.Split("=")

                If oConf.Length > 0 Then
                    sComputedFeature = oConf(0)
                End If

                If oConf.Length > 1 Then
                    Select Case oConf(1).ToUpper
                        Case "WRITE"
                            reqPermission = Permission.Write
                        Case "READ"
                            reqPermission = Permission.Read
                        Case "NONE"
                            reqPermission = Permission.None
                        Case "ADMIN"
                            reqPermission = Permission.Admin
                        Case Else
                            reqPermission = Permission.Read
                    End Select
                End If

                sComputedFeature = oBehaviour.Feature.Split("=")(0)
            Else
                reqPermission = If(oBehaviour.RequieredPermission.HasValue, oBehaviour.RequieredPermission.Value, Permission.Read)
                sComputedFeature = oBehaviour.Feature
            End If

            Dim sDateSQLFilter As String = String.Empty
            Dim sDateJoinFilter As String = String.Empty
            If Not oBehaviour.DateInf.HasValue OrElse (oBehaviour.DateInf.HasValue And oBehaviour.DateInf = Date.MinValue) Then
                sDateSQLFilter = " AND eg.EndDate >= CONVERT(date, GETDATE())"
                sDateJoinFilter = $" AND {roTypes.SQLSmallDateTime(roTypes.UnspecifiedNow.Date)} between sem.BeginDate and sem.EndDate"
            Else
                Dim sDateInf As String = roTypes.SQLSmallDateTime(oBehaviour.DateInf)
                Dim sDateSup As String = roTypes.SQLSmallDateTime(oBehaviour.DateSup)

                sDateSQLFilter = $" AND {sDateInf} <= eg.EndDate AND {sDateSup} >= eg.BeginDate AND {sDateInf} <= ec.EndDate AND {sDateSup} >= ec.BeginDate"
                sDateJoinFilter = $" AND {roTypes.SQLSmallDateTime(oBehaviour.DateInf)} <= sem.EndDate AND {roTypes.SQLSmallDateTime(oBehaviour.DateSup)} >= sem.BeginDate"
            End If

            Dim strSQL As String
            Dim filterTableName As String = GetEmpoyeesJoinTableWithoutPermissions(oFilter.ComposeFilter, oFilter.Filters, oFilter.UserFields, oBehaviour.AddOnlyDirectEmployees)
            If sComputedFeature <> String.Empty Then
                strSQL = $"@SELECT# DISTINCT eg.IDEmployee FROM sysroEmployeeGroups eg
                                INNER Join sysrovwSecurity_PermissionOverEmployeeAndFeature poe on poe.IDPassport={oBehaviour.IdPassport} And poe.IDEmployee = eg.IDEmployee 
                                            And poe.FeatureAlias='{sComputedFeature}' AND poe.FeatureType='{oBehaviour.FeatureType}' AND poe.FeaturePermission >={CInt(reqPermission)}
                                            And convert(DATE,GETDATE()) between poe.BeginDate And poe.EndDate
                                {If(oBehaviour.DateInf.HasValue, " INNER JOIN EmployeeContracts ec on eg.IDEmployee = ec.IDEmployee ", "")}
                                INNER JOIN {filterTableName} AS sem ON eg.idemployee = sem.id {sDateJoinFilter}
                                WHERE 1=1 "
            Else
                strSQL = $"@SELECT# DISTINCT eg.IDEmployee FROM sysroEmployeeGroups eg
                            {If(oBehaviour.DateInf.HasValue, " INNER JOIN EmployeeContracts ec on eg.IDEmployee = ec.IDEmployee ", "")}
                            INNER JOIN {filterTableName} AS sem ON eg.idemployee = sem.id {sDateJoinFilter}
                            WHERE 1=1 "
            End If

            strSQL &= sDateSQLFilter

            ' Si queremos empleados por fechas, ataco a sysroEmployeeGroups
            If Not oBehaviour.DateInf.HasValue OrElse (oBehaviour.DateInf.HasValue And oBehaviour.DateInf = Date.MinValue) Then
                strSQL = strSQL.Replace("sysroEmployeeGroups", "sysrovwEmployees_CurrentAndFutureMobilities")
            Else
                strSQL = strSQL.Replace("sysroEmployeeGroups", "sysrovwEmployees_AllMobilities")
            End If


            tmpEmployeesTableName = $"#esf_{Guid.NewGuid.ToString().Replace("-", "")}"
            AccessHelper.BulkCopyIdFromQuery(tmpEmployeesTableName, strSQL, "IDEmployee")

            If Not String.IsNullOrEmpty(filterTableName) Then AccessHelper.ExecuteSql($"IF OBJECT_ID('tempdb..#{filterTableName}') IS NOT NULL @DROP# TABLE #{filterTableName};")
        Catch ex As Exception
            tmpEmployeesTableName = String.Empty
            roLog.GetInstance().logMessage(roLog.EventType.roError, "GetEmployeeList:: Unkown exeception", ex)
        End Try

        Return tmpEmployeesTableName

    End Function




#Region "Helper for employee permissions join clauses"
    Public Shared Function GetEmployeePermissonInnerJoin(ByVal IdPassport As Integer, ByVal requieredPermission As Permission, ByVal Feature As String, ByVal Type As String, Optional ByVal strDateField As String = "GETDATE()", Optional ByVal strIDEmployeeField As String = "IDEmployee") As String
        Dim strPermissionFilter As String = String.Empty

        Try
            Dim oSelectedFeature As String = ""

            Dim reqPermission As Permission = requieredPermission
            If Feature.Contains("=") Then
                Dim oConf As String() = Feature.Split("=")

                If oConf.Length > 0 Then
                    oSelectedFeature = oConf(0)
                End If

                If oConf.Length > 1 Then
                    Select Case oConf(1).ToUpper
                        Case "WRITE"
                            reqPermission = Permission.Write
                        Case "READ"
                            reqPermission = Permission.Read
                        Case "NONE"
                            reqPermission = Permission.None
                        Case "ADMIN"
                            reqPermission = Permission.Admin
                        Case Else
                            reqPermission = Permission.Read
                    End Select
                End If

                oSelectedFeature = Feature.Split("=")(0)
            Else
                reqPermission = Permission.Read
                oSelectedFeature = Feature
            End If

            Dim employeeFeatureId As Integer = 1
            If oSelectedFeature.ToUpperInvariant.StartsWith("EMPLOYEES") Then
                employeeFeatureId = 1
            ElseIf oSelectedFeature.ToUpperInvariant.StartsWith("CALENDAR") Then
                employeeFeatureId = 2
            ElseIf oSelectedFeature.ToUpperInvariant.StartsWith("ACCESS") Then
                employeeFeatureId = 9
            ElseIf oSelectedFeature.ToUpperInvariant.StartsWith("LABAGREE") Then
                employeeFeatureId = 11
            ElseIf oSelectedFeature.ToUpperInvariant.StartsWith("TASKS") Then
                employeeFeatureId = 25
            ElseIf oSelectedFeature.ToUpperInvariant.StartsWith("VISITS") Then
                employeeFeatureId = 31
            ElseIf oSelectedFeature.ToUpperInvariant.StartsWith("DOCUMENTS") Then
                employeeFeatureId = 32
            End If

            If Not strIDEmployeeField.Contains(".") Then strIDEmployeeField = "tmp." & strIDEmployeeField

            strPermissionFilter = $" INNER JOIN sysrovwSecurity_PermissionOverEmployeeAndFeature poe on poe.IDPassport = {IdPassport.ToString} And poe.IDEmployee = {strIDEmployeeField} 
                                        AND CONVERT(DATE,GETDATE()) between poe.BeginDate and poe.EndDate AND poe.IdFeature = {employeeFeatureId} AND poe.FeaturePermission >={CInt(requieredPermission)}"

            'strPermissionFilter = "inner join sysrovwGetPermissionOverEmployee gpove on gpove.PassportID = " & IdPassport & " and gpove.EmployeeID = tmp." & strIDEmployeeField & " and gpove.EmployeeFeatureID = " & employeeFeatureId & " and gpove.CalculatedPermission >= " & CInt(requieredPermission) & " and (convert(date," & strDateField & ",120)) between gpove.BeginDate and gpove.EndDate "
            'strPermissionFilter &= "inner join sysrovwGetPermissionOverFeature gpovf on gpovf.idPassport = gpove.PassportID and gpovf.FeatureID = gpove.EmployeeFeatureID and gpovf.Permission >= " & CInt(requieredPermission)
        Catch ex As Exception
            strPermissionFilter = String.Empty
        End Try

        Return strPermissionFilter
    End Function

    Public Shared Function GetEmployeeExceptionsInnerJoin(ByVal IdPassport As Integer, ByVal requieredPermission As Permission, ByVal Feature As String, ByVal Type As String, Optional ByVal strDateField As String = "GETDATE()", Optional ByVal strIDEmployeeField As String = "IDEmployee", Optional ByVal strIDGroupField As String = "IDGroup") As String
        Dim strPermissionFilter As String = String.Empty

        Try
            Dim oSelectedFeature As String = ""

            Dim reqPermission As Permission = requieredPermission
            If Feature.Contains("=") Then
                Dim oConf As String() = Feature.Split("=")

                If oConf.Length > 0 Then
                    oSelectedFeature = oConf(0)
                End If

                If oConf.Length > 1 Then
                    Select Case oConf(1).ToUpper
                        Case "WRITE"
                            reqPermission = Permission.Write
                        Case "READ"
                            reqPermission = Permission.Read
                        Case "NONE"
                            reqPermission = Permission.None
                        Case "ADMIN"
                            reqPermission = Permission.Admin
                        Case Else
                            reqPermission = Permission.Read
                    End Select
                End If

                oSelectedFeature = Feature.Split("=")(0)
            Else
                reqPermission = Permission.Read
                oSelectedFeature = Feature
            End If

            Dim employeeFeatureId As Integer = 1
            If oSelectedFeature.ToUpperInvariant.StartsWith("EMPLOYEES") Then
                employeeFeatureId = 1
            ElseIf oSelectedFeature.ToUpperInvariant.StartsWith("CALENDAR") Then
                employeeFeatureId = 2
            ElseIf oSelectedFeature.ToUpperInvariant.StartsWith("ACCESS") Then
                employeeFeatureId = 9
            ElseIf oSelectedFeature.ToUpperInvariant.StartsWith("LABAGREE") Then
                employeeFeatureId = 11
            ElseIf oSelectedFeature.ToUpperInvariant.StartsWith("TASKS") Then
                employeeFeatureId = 25
            ElseIf oSelectedFeature.ToUpperInvariant.StartsWith("VISITS") Then
                employeeFeatureId = 31
            ElseIf oSelectedFeature.ToUpperInvariant.StartsWith("DOCUMENTS") Then
                employeeFeatureId = 32
            End If


            strPermissionFilter = $"INNER JOIN sysropassports_Employees pe on pe.IdPassport = {IdPassport.ToString} AND pe.IDEmployee = tmp.{strIDEmployeeField} and pe.Permission = 1 
                                    INNER JOIN sysrovwSecurity_PermissionOverEmployeeAndFeature poe on poe.IDPassport = {IdPassport.ToString} And poe.IDEmployee = pe.IDEmployee
                                        AND CONVERT(DATE,GETDATE()) between poe.BeginDate and poe.EndDate AND poe.IdFeature = {employeeFeatureId} AND poe.FeaturePermission >={CInt(requieredPermission)}
                                    INNER JOIN employeegroups eg on eg.IDEmployee = pe.IDEmployee and convert(date,getdate()) between eg.BeginDate and eg.EndDate
                                        AND eg.IDGroup not in(@SELECT# idgroup from sysrovwSecurity_PermissionOverGroups where IdPassport = {IdPassport.ToString})"

        Catch ex As Exception
            strPermissionFilter = String.Empty
        End Try

        Return strPermissionFilter
    End Function

    Public Shared Function GetGroupPermissonInnerJoin(ByVal IdPassport As Integer, ByVal requieredPermission As Permission, ByVal Feature As String, ByVal Type As String, ByVal strOrderBy As String, ByVal strGroupField As String) As String
        Dim strPermissionFilter As String = String.Empty

        Try
            Dim oSelectedFeature As String = ""

            If Feature.Contains("=") Then
                Dim oConf As String() = Feature.Split("=")

                If oConf.Length > 0 Then
                    oSelectedFeature = oConf(0)
                End If

                If oConf.Length > 1 Then
                    Select Case oConf(1).ToUpper
                        Case "WRITE"
                            requieredPermission = Permission.Write
                        Case "READ"
                            requieredPermission = Permission.Read
                        Case "NONE"
                            requieredPermission = Permission.None
                        Case "ADMIN"
                            requieredPermission = Permission.Admin
                    End Select
                End If

                oSelectedFeature = Feature.Split("=")(0)
            Else
                requieredPermission = Permission.Read
                oSelectedFeature = Feature
            End If

            Dim employeeFeatureId As Integer = 1
            If oSelectedFeature.ToUpperInvariant.StartsWith("EMPLOYEES") Then
                employeeFeatureId = 1
            ElseIf oSelectedFeature.ToUpperInvariant.StartsWith("CALENDAR") Then
                employeeFeatureId = 2
            ElseIf oSelectedFeature.ToUpperInvariant.StartsWith("ACCESS") Then
                employeeFeatureId = 9
            ElseIf oSelectedFeature.ToUpperInvariant.StartsWith("LABAGREE") Then
                employeeFeatureId = 11
            ElseIf oSelectedFeature.ToUpperInvariant.StartsWith("TASKS") Then
                employeeFeatureId = 25
            ElseIf oSelectedFeature.ToUpperInvariant.StartsWith("VISITS") Then
                employeeFeatureId = 31
            ElseIf oSelectedFeature.ToUpperInvariant.StartsWith("DOCUMENTS") Then
                employeeFeatureId = 32
            End If

            strPermissionFilter = $" INNER JOIN sysrovwSecurity_PermissionOverGroups pog ON pog.IdPassport = {IdPassport} AND pog.IdGroup = {strGroupField}
                                                AND DATEADD(day, DATEDIFF(Day, 0, GETDATE()), 0) between pog.BeginDate and pog.EndDate
                                     INNER JOIN sysrovwSecurity_PermissionOverFeatures pof on pof.IDPassport = {IdPassport} AND pof.IdFeature = {employeeFeatureId} AND pof.Permission > 0 "

        Catch ex As Exception
            strPermissionFilter = String.Empty
        End Try

        Return strPermissionFilter
    End Function

#End Region


End Class
