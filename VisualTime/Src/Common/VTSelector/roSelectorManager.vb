Imports System.Data.Common
Imports System.Data.SqlClient
Imports System.Runtime.Serialization
Imports System.Text
Imports System.Web
Imports System.Web.UI.WebControls
Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.DataLayer
Imports Robotics.VTBase

Namespace VTSelectorManager

    <DataContract>
    Public Class roSelectorManager
        Public Function GetEmployeesFromSelectorFilter(ByVal selectorFilter As roSelectorFilter, ByVal loadEmployeesWhenAllSelected As Boolean, ByVal loadEmployeesList As Boolean, ByVal createTemporalTable As Boolean) As roUniversalSelector
            Dim result As New roUniversalSelector
            Try
                Dim resultTable As New DataTable()
                resultTable.Columns.Add("IDEmployee", GetType(Integer))
                Dim employeesAndGroupsTable As New DataTable()
                Dim collectivesTable As New DataTable()
                Dim labagreesTable As New DataTable()
                result.Employees = New List(Of roSelectedEmployee)

                If selectorFilter.ComposeFilter.Trim = String.Empty Then
                    Select Case selectorFilter.ComposeMode.ToUpper
                        Case "ALL"
                            result.AllEmployeeSelected = True
                        Case "NONE"
                            result.NoEmployeeSelected = True
                        Case "CUSTOM"
                            ' Do nothing
                    End Select
                End If

                result.TemporalTableName = "#TempEmployees_" & Guid.NewGuid().ToString("N")
                result.Filter = selectorFilter
                Dim isOR As Boolean = result.Filter.Operation.ToUpper <> "AND"

                If Not result.AllEmployeeSelected AndAlso Not result.NoEmployeeSelected Then
                    ' Empleados y Grupos 
                    employeesAndGroupsTable = GetGroupsAndPositiveExceptionsEmployees(selectorFilter, Now.Date)
                    resultTable.Merge(employeesAndGroupsTable)
                    result.Employees = ConvertEmployeeTableToSelectedEmployeesList(employeesAndGroupsTable, SelectionSource.EmployeeOrGroup)

                    If resultTable.Rows.Count > 0 OrElse isOR Then
                        ' Colectivos
                        Dim selectedCollectivesIds As New List(Of Integer)
                        ExtractCollectivesIdsFromSelectionString(selectorFilter.ComposeFilter, selectedCollectivesIds)
                        If selectedCollectivesIds.Count > 0 Then
                            collectivesTable = GetCollectiveEmployees(selectedCollectivesIds, 0, Now.Date, Now.Date)
                            Dim collectiveEmployeesList = ConvertEmployeeTableToSelectedEmployeesList(collectivesTable.DefaultView.ToTable(True, "IDEmployee"), SelectionSource.Collective)
                            If isOR Then
                                resultTable.Merge(collectivesTable.DefaultView.ToTable(True, "IDEmployee"))
                                result.Employees.AddRange(collectiveEmployeesList)
                            Else
                                resultTable = TableIntersect(resultTable, collectivesTable.DefaultView.ToTable(True, "IDEmployee"))
                                result.Employees = EmployeeListIntersect(result.Employees, collectiveEmployeesList)
                            End If
                        End If

                        If resultTable.Rows.Count > 0 OrElse isOR Then
                            ' Acuerdos Laborales
                            Dim selectedLabAgreementsIds As New List(Of Integer)
                            ExtractLabAgreementIdsFromSelectionString(selectorFilter.ComposeFilter, selectedLabAgreementsIds)
                            If selectedLabAgreementsIds.Count > 0 Then
                                labagreesTable = GetLabAgreeEmployees(selectedLabAgreementsIds, Now.Date)
                                Dim agreementEmployeesList = ConvertEmployeeTableToSelectedEmployeesList(labagreesTable.DefaultView.ToTable(True, "IDEmployee"), SelectionSource.Agreement)
                                If isOR Then
                                    resultTable.Merge(labagreesTable.DefaultView.ToTable(True, "IDEmployee"))
                                    result.Employees.AddRange(agreementEmployeesList)
                                Else
                                    resultTable = TableIntersect(resultTable, labagreesTable.DefaultView.ToTable(True, "IDEmployee"))
                                    result.Employees = EmployeeListIntersect(result.Employees, agreementEmployeesList)
                                End If
                            End If
                        End If
                    End If
                End If

                If createTemporalTable Then AccessHelper.EmployeeSelectorBulkCopy(result.TemporalTableName, resultTable)

            Catch ex As Exception
                'Do Something
            End Try

            Return result
        End Function


#Region "Helper Methods"

        Private Function EmployeeListIntersect(list1 As List(Of roSelectedEmployee), list2 As List(Of roSelectedEmployee)) As List(Of roSelectedEmployee)
            ' Usar LINQ para encontrar la intersección basada en IdEmployee
            Dim intersection = list1.Join(
                       list2,
                       Function(e1) e1.IdEmployee,
                       Function(e2) e2.IdEmployee,
                       Function(e1, e2) e1).ToList()

            Return intersection
        End Function

        ''' <summary>
        ''' Convierte una tabla con IDs de empleados en una lista de objetos roSelectedEmployee
        ''' </summary>
        ''' <param name="employeeTable">DataTable con una columna IDEmployee</param>
        ''' <param name="sourceType">Origen del empleado: A=Grupo, B=Empleado, C=Colectivo, L=Acuerdo Laboral</param>
        ''' <returns>Lista de objetos roSelectedEmployee</returns>
        Public Function ConvertEmployeeTableToSelectedEmployeesList(employeeTable As DataTable, sourceType As SelectionSource) As List(Of roSelectedEmployee)
            Dim result As New List(Of roSelectedEmployee)()

            Try
                If employeeTable Is Nothing OrElse employeeTable.Rows.Count = 0 Then
                    Return result
                End If

                ' TODO: Informar EmployeeName y GroupName si va a ser útil
                For Each row As DataRow In employeeTable.Rows
                    Dim employee As New roSelectedEmployee() With {
                                .IdEmployee = Convert.ToInt32(row("IDEmployee")),
                                .SelectionSource = sourceType
                            }
                    result.Add(employee)
                Next

            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roSelectorManager:ConvertEmployeeTableToSelectedEmployeesList::Unkown exeception", ex)
            End Try

            Return result
        End Function
#End Region

#Region "Groups and Employees"
        Public Function GetGroupsAndPositiveExceptionsEmployees(selectorFilter As roSelectorFilter, referenceDate As Date) As DataTable
            Dim returnEmployees As DataTable = Nothing
            Try
                'TODO: Aclarar el parámetro AddOnlyDirectEmployees
                Dim addOnlyDirectEmployees As Boolean = True 'En llamada orginal se sustituye por empFilter[2]

                Dim sJoinFilter As String = roSelectorManager.GetEmpoyeesJoinTableWithoutPermissions(selectorFilter.ComposeFilter, selectorFilter.Filters, selectorFilter.UserFields, addOnlyDirectEmployees)

                Dim sqlCommand As String = $"@SELECT# sysrovwAllEmployeeGroups.IDEmployee
                                          FROM Employees 
                                          INNER JOIN sysrovwAllEmployeeGroups ON sysrovwAllEmployeeGroups.IDEmployee = Employees.ID
                                          INNER JOIN #{sJoinFilter} AS sem ON sysrovwAllEmployeeGroups.idemployee = sem.id and {roTypes.SQLDateTime(referenceDate.Date)} BETWEEN sem.BeginDate AND sem.EndDate"

                returnEmployees = AccessHelper.CreateDataTable(sqlCommand)

            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roSelectorManager:GetGroupsAndPositiveExceptionsEmployees::Unkown exeception", ex)
            End Try

            Return returnEmployees
        End Function

        Public Shared Function GetEmpoyeesJoinTableWithoutPermissions(ByVal strEmployeeFilter As String, ByVal strEmployeeStatus As String, ByVal strEmployeeUserFieldsFilter As String, Optional ByVal bAddOnlyDirectEmployees As Boolean = True) As String

            'Nombre de la tabla temporal donde estan los id's de empleados
            Dim sTableName As String = String.Empty

            Try
                Dim strEmployees As String = ""
                Dim strGroups As String = ""

                BuildSelectionFilterFromSelector(strEmployeeFilter, strEmployees, strGroups)

                Dim groupsJoin As String = String.Empty
                Dim employeeJoin As String = String.Empty
                Dim tmpGroupsTableName As String = String.Empty
                Dim tmpEmployeesTableName As String = String.Empty

                If strGroups <> "-2" Then
                    tmpGroupsTableName = $"wwg_{Guid.NewGuid.ToString().Replace("-", "")}"
                    AccessHelper.BulkCopyIdListFromString(tmpGroupsTableName, strGroups)

                    If bAddOnlyDirectEmployees Then
                        groupsJoin = $"LEFT JOIN (@SELECT# DISTINCT sg.id
                                    FROM groups AS sg
                                    INNER JOIN #{tmpGroupsTableName} AS sgr ON sg.id = sgr.Id 
                                ) AS GroupsSelected ON eg.IDGroup = GroupsSelected.ID"
                    Else
                        groupsJoin = $"LEFT JOIN (@SELECT# DISTINCT dg.id
                                    FROM groups AS sg
                                    INNER JOIN #{tmpGroupsTableName} AS sgr ON sg.id = sgr.Id
                                    INNER JOIN groups AS dg ON dg.Path = sg.Path OR dg.Path LIKE sg.Path + '\%' 
                                ) AS GroupsSelected ON eg.IDGroup = GroupsSelected.ID"
                    End If

                Else
                    groupsJoin = $"LEFT JOIN (SELECT DISTINCT sg.id
                                    FROM groups AS sg
                                ) AS GroupsSelected ON eg.IDGroup = GroupsSelected.ID"
                End If

                If strEmployees <> "-1" Then
                    tmpEmployeesTableName = $"wwe_{Guid.NewGuid.ToString().Replace("-", "")}"
                    AccessHelper.BulkCopyIdListFromString(tmpEmployeesTableName, strEmployees)

                    employeeJoin = $"LEFT JOIN (@SELECT# DISTINCT emp.id, CONVERT(date,'19700101') as EmpBeginDate, CONVERT(date,'20790101') as EmpEndDate
                                        FROM employees AS emp
                                        INNER JOIN #{tmpEmployeesTableName} AS sem ON emp.id = sem.Id
                                   ) AS EmployeesSelected ON eg.IDEmployee = EmployeesSelected.ID"
                Else
                    employeeJoin = $"LEFT JOIN (@SELECT# -1 AS ID, CONVERT(date,'19700101') as EmpBeginDate, CONVERT(date,'20790101') as EmpEndDate
                                   ) AS EmployeesSelected ON eg.IDEmployee = EmployeesSelected.ID"
                End If

                Dim sql As String

                sql = $"@SELECT# DISTINCT eg.IDEmployee as ID, ISNULL(EmployeesSelected.EmpBeginDate, eg.BeginDate) as BeginDate,
                                            ISNULL(EmployeesSelected.EmpEndDate, eg.EndDate) as EndDate
                            FROM sysroEmployeeGroups eg
                            {groupsJoin} 
                            {employeeJoin}
                        WHERE 
                            (GroupsSelected.ID IS NOT NULL {If(strEmployees = "-1", "", "OR EmployeesSelected.ID IS NOT NULL")} ) "

                Dim sFiltersSQL As String = ""
                If strEmployeeStatus <> "" Then
                    If Mid(strEmployeeStatus, 1, 1) = 0 Then  'Empleado actual
                        sFiltersSQL = sFiltersSQL & IIf(Len(sFiltersSQL) > 0, " and ", " ") & "(not (eg.CurrentEmployee=1) or eg.EndDate < " & roTypes.Any2Time(VTBase.roConstants.roNullDate).SQLSmallDateTime & ")"
                    End If
                    If Mid(strEmployeeStatus, 2, 1) = 0 Then  'Empleado en movimiento
                        sFiltersSQL = sFiltersSQL & IIf(Len(sFiltersSQL) > 0, " and ", " ") & "(not (eg.EndDate < " & roTypes.Any2Time(VTBase.roConstants.roNullDate).SQLSmallDateTime & "))"
                    End If
                    If Mid(strEmployeeStatus, 3, 1) = 0 Then  'Empleado de baja
                        sFiltersSQL = sFiltersSQL & IIf(Len(sFiltersSQL) > 0, " and ", " ") & "(not (eg.CurrentEmployee=0) or eg.BeginDate > " & roTypes.Any2Time(Now).SQLSmallDateTime & ")"
                    End If
                    If Mid(strEmployeeStatus, 4, 1) = 0 Then  'Empleado futur
                        sFiltersSQL = sFiltersSQL & IIf(Len(sFiltersSQL) > 0, " and ", " ") & "(not (eg.BeginDate > " & roTypes.Any2Time(Now).SQLSmallDateTime & "))"
                    End If

                    If Mid(strEmployeeStatus, 5, 1) = 1 Then
                        Dim strRet As String = BuildUserFieldsWhere(strEmployeeUserFieldsFilter)
                        If strRet <> String.Empty Then
                            sFiltersSQL = sFiltersSQL & IIf(Len(sFiltersSQL) > 0, " and ", " ") & strRet.Replace("Employees.ID", "eg.IDEmployee")
                        End If
                    End If

                    If sFiltersSQL <> String.Empty AndAlso sql <> String.Empty Then sql = $"{sql} AND ({sFiltersSQL})"
                End If


                sTableName = $"#empselector_{Guid.NewGuid.ToString().Replace("-", "")}"
                Dim dt As DataTable = AccessHelper.CreateDataTable(sql)
                AccessHelper.BulkCopy(sTableName, dt)

                If Not String.IsNullOrEmpty(tmpGroupsTableName) Then AccessHelper.ExecuteSql($"IF OBJECT_ID('tempdb..#{tmpGroupsTableName}') IS NOT NULL @DROP# TABLE #{tmpGroupsTableName};")
                If Not String.IsNullOrEmpty(tmpEmployeesTableName) Then AccessHelper.ExecuteSql($"IF OBJECT_ID('tempdb..#{tmpEmployeesTableName}') IS NOT NULL @DROP# TABLE #{tmpEmployeesTableName};")

            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roSelectorManager:GetWhereWithoutPermissions::Unkown exeception", ex)
            End Try



            Return sTableName
        End Function


        Public Shared Function GetWhereWithoutPermissions(ByVal strEmployeeFilter As String, ByVal strEmployeeStatus As String, ByVal strEmployeeUserFieldsFilter As String, Optional ByVal bAddOnlyDirectEmployees As Boolean = True) As String

            Dim oRet As String = String.Empty

            Try
                Dim strEmployees As String = ""
                Dim strGroups As String = ""

                BuildSelectionFilterFromSelector(strEmployeeFilter, strEmployees, strGroups)

                Dim tmpTableName As String = $"wwp_{Guid.NewGuid.ToString().Replace("-", "")}"
                AccessHelper.BulkCopyIdListFromString(tmpTableName, strGroups)

                If strGroups.Length > 0 Then
                    If Not bAddOnlyDirectEmployees Then
                        oRet = $"sysroEmployeeGroups.IDGroup IN(@select# pg.ID from groups 
                                     inner JOIN Groups pg ON pg.path = groups.path OR pg.path LIKE groups.path + '\%'
                                     inner JOIN #{tmpTableName} on #{tmpTableName}.Id = groups.id {If(strGroups = "-2", " OR 1=1 ", "")})"
                    Else
                        oRet = $"sysroEmployeeGroups.IDGroup IN({strGroups})"
                    End If

                Else
                    oRet = $"sysroEmployeeGroups.IDGroup IN(-1)"
                End If

                If strEmployees <> String.Empty Then
                    If oRet <> String.Empty Then
                        oRet = $"({oRet} OR sysroEmployeeGroups.IDEmployee IN ({strEmployees}))"
                    Else
                        oRet = $"sysroEmployeeGroups.IDEmployee IN ({strEmployees})"
                    End If
                End If


                Dim sFiltersSQL As String = ""
                If strEmployeeStatus <> "" Then
                    If Mid(strEmployeeStatus, 1, 1) = 0 Then  'Empleado actual
                        sFiltersSQL = sFiltersSQL & IIf(Len(sFiltersSQL) > 0, " and ", " ") & "(not (sysroEmployeeGroups.CurrentEmployee=1) or sysroEmployeeGroups.EndDate < " & roTypes.Any2Time(VTBase.roConstants.roNullDate).SQLSmallDateTime & ")"
                    End If
                    If Mid(strEmployeeStatus, 2, 1) = 0 Then  'Empleado en movimiento
                        sFiltersSQL = sFiltersSQL & IIf(Len(sFiltersSQL) > 0, " and ", " ") & "(not (sysroEmployeeGroups.EndDate < " & roTypes.Any2Time(VTBase.roConstants.roNullDate).SQLSmallDateTime & "))"
                    End If
                    If Mid(strEmployeeStatus, 3, 1) = 0 Then  'Empleado de baja
                        sFiltersSQL = sFiltersSQL & IIf(Len(sFiltersSQL) > 0, " and ", " ") & "(not (sysroEmployeeGroups.CurrentEmployee=0) or sysroEmployeeGroups.BeginDate > " & roTypes.Any2Time(Now).SQLSmallDateTime & ")"
                    End If
                    If Mid(strEmployeeStatus, 4, 1) = 0 Then  'Empleado futur
                        sFiltersSQL = sFiltersSQL & IIf(Len(sFiltersSQL) > 0, " and ", " ") & "(not (sysroEmployeeGroups.BeginDate > " & roTypes.Any2Time(Now).SQLSmallDateTime & "))"
                    End If

                    If Mid(strEmployeeStatus, 5, 1) = 1 Then
                        Dim strRet As String = BuildUserFieldsWhere(strEmployeeUserFieldsFilter)
                        If strRet <> String.Empty Then
                            sFiltersSQL = sFiltersSQL & IIf(Len(sFiltersSQL) > 0, " and ", " ") & strRet.Replace("Employees.ID", "sysroEmployeeGroups.IDEmployee")
                        End If
                    End If

                    If sFiltersSQL <> String.Empty AndAlso oRet <> String.Empty Then oRet = $"({oRet}) AND ({sFiltersSQL})"
                End If

            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roSelectorManager:GetWhereWithoutPermissions::Unkown exeception", ex)
            End Try

            Return oRet

        End Function

        Public Shared Sub BuildSelectionFilterFromSelector(ByVal strEmployeeSelectorValue As String, ByRef strEmployeesList As String, ByRef strGroupsList As String)

            Dim lstEmployees As Generic.List(Of Integer) = Nothing
            Dim lstGroups As Generic.List(Of Integer) = Nothing

            strEmployeesList = ""
            strGroupsList = ""

            ExtractIdsFromSelectionString(strEmployeeSelectorValue, lstEmployees, lstGroups)

            If lstEmployees IsNot Nothing Then
                strEmployeesList = String.Join(",", lstEmployees.Distinct())
            End If

            If lstGroups IsNot Nothing Then
                strGroupsList = String.Join(",", lstGroups.Distinct())
            End If

            If strEmployeesList = "" Then strEmployeesList = "-1"
            If strGroupsList = "" Then strGroupsList = "-1"

        End Sub

        Public Shared Sub ExtractIdsFromSelectionString(ByVal strSelection As String, ByRef lstEmployeeSelection As Generic.List(Of Integer), ByRef lstGroupSelection As Generic.List(Of Integer))

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


            If strSelection <> "" Then

                If strSelection = "ALL" Then
                    lstGroupSelection.Add(-2)
                Else
                    Dim Selection As String() = strSelection.Trim.Split(",")
                    If Selection.Length > 0 Then
                        For Each Sel As String In Selection
                            Sel = Sel.Trim
                            If Sel <> "" Then
                                Select Case Sel.Substring(0, 1)
                                    Case "A" ' Grupo
                                        lstGroupSelection.Add(Sel.Substring(1))
                                    Case "B" ' Empleado
                                        lstEmployeeSelection.Add(Sel.Substring(1))
                                End Select
                            End If
                        Next
                    End If
                End If
            End If

        End Sub

        Public Shared Sub ExtractCollectivesIdsFromSelectionString(ByVal strSelection As String, ByRef listCollectivesIds As Generic.List(Of Integer))

            If listCollectivesIds IsNot Nothing Then
                listCollectivesIds.Clear()
            Else
                listCollectivesIds = New Generic.List(Of Integer)
            End If

            If strSelection <> "" Then
                Dim Selection As String() = strSelection.Trim.Split(",")
                For Each Sel As String In Selection
                    Sel = Sel.Trim
                    If Sel <> "" AndAlso Sel.StartsWith("C") Then
                        listCollectivesIds.Add(Integer.Parse(Sel.Substring(1)))
                    End If
                Next
            End If
        End Sub

        Public Shared Sub ExtractLabAgreementIdsFromSelectionString(ByVal strSelection As String, ByRef listLabAgreenentsIds As Generic.List(Of Integer))

            If listLabAgreenentsIds IsNot Nothing Then
                listLabAgreenentsIds.Clear()
            Else
                listLabAgreenentsIds = New Generic.List(Of Integer)
            End If

            If strSelection <> "" Then
                Dim Selection As String() = strSelection.Trim.Split(",")
                For Each Sel As String In Selection
                    Sel = Sel.Trim
                    If Sel <> "" AndAlso Sel.StartsWith("L") Then
                        listLabAgreenentsIds.Add(Integer.Parse(Sel.Substring(1)))
                    End If
                Next
            End If
        End Sub

        Public Shared Function BuildUserFieldsWhere(ByVal _Filter As String, Optional dDate As Date = Nothing) As String

            Dim result As String = String.Empty
            Dim strRet As New StringBuilder

            If _Filter <> "" Then

                _Filter = HttpUtility.UrlDecode(_Filter)
                _Filter = StringDecodeControlChars(_Filter)

                Dim strFieldName As String
                Dim arrFilters As String() = _Filter.Split(Chr(127))
                Dim arrParams As String()


                For Each str As String In arrFilters
                    If str.Trim <> "" Then
                        arrParams = str.Split("~")
                        ' Obtenemos el nombre del campo
                        strFieldName = arrParams(0).Split("|")(0)
                        If strFieldName.ToUpper.StartsWith("USR_") Then strFieldName = strFieldName.Substring(4)

                        Dim oUserFieldName As String = strFieldName
                        Dim eCompareType As CompareType
                        Select Case arrParams(1)
                            Case "=" : eCompareType = CompareType.Equal
                            Case "<>" : eCompareType = CompareType.Distinct
                            Case ">" : eCompareType = CompareType.Major
                            Case ">=" : eCompareType = CompareType.MajorEqual
                            Case "<" : eCompareType = CompareType.Minor
                            Case "<=" : eCompareType = CompareType.MinorEqual
                            Case "*" : eCompareType = CompareType.StartWith
                            Case "**" : eCompareType = CompareType.Contains
                        End Select


                        Dim oUserFieldValue As String
                        'Desproteger valor (problemas al codificar/decodificar) le quitamos parentesis
                        If arrParams(2).StartsWith("(") AndAlso arrParams(2).EndsWith(")") Then
                            oUserFieldValue = arrParams(2).Substring(1, arrParams(2).Length - 2)
                        Else
                            oUserFieldValue = arrParams(2)
                        End If


                        Dim eFieldType As FieldTypes = roTypes.Any2Integer(AccessHelper.ExecuteScalar($"@SELECT# FieldType FROM sysroUserFields WHERE FieldName = '{oUserFieldName}'"))

                        strRet.Append($"{BuildFilterFromCondition(-1, oUserFieldName, oUserFieldValue, eFieldType, eCompareType, dDate)} {arrParams(3)} ")

                    End If
                Next

                result = strRet.ToString
                If result.EndsWith("AND ") Then result = result.Substring(0, result.Length - 4)
                If result.EndsWith("OR ") Then result = result.Substring(0, result.Length - 3)

                If result <> "" Then result = "(" & result & ")"

            End If

            Return result

        End Function

        Public Shared Function StringDecodeControlChars(ByVal sInput As String) As String
            '
            ' Descodifica un string codificado previamente con StringEncodeControlChars.
            '
            Dim sOutput As String = String.Empty
            Dim I As Integer

            sOutput = sInput
            For I = 1 To 31
                sOutput = sOutput.Replace("%" & I & "%", Chr(I))
            Next
            For I = 60 To 62
                sOutput = sOutput.Replace("%" & I & "%", Chr(I))
            Next
            For I = 123 To 255
                sOutput = sOutput.Replace("%" & I & "%", Chr(I))
            Next

            Return sOutput

        End Function

        Public Shared Function BuildFilterFromCondition(ByVal IDEmployee As Integer, ByVal strFieldName As String,
                                                                ByVal strUserFieldValue As String, ByVal eFieldType As FieldTypes, ByVal eCompare As CompareType,
                                                                Optional dDate As Date = Nothing) As String

            Dim strRet As String = ""
            Dim _FieldName As String = strFieldName 'Me.oUserField.FieldName.Replace("'", "''") '  "[USR_" & Me.oUserField.FieldName & "]"
            Dim _Value As String = strUserFieldValue 'Me.strValue
            Dim _defaultFieldValue As String = ""
            If dDate = Nothing Then dDate = DateTime.Today

            Select Case eFieldType
                Case FieldTypes.tText, FieldTypes.tList
                    _defaultFieldValue = ""
                    _Value = _Value.Replace("'", "''")
                    Select Case eCompare
                        Case CompareType.Equal
                            strRet = "CONVERT(NVARCHAR(4000), ISNULL([Value],'')) = '" & _Value & "' "
                        Case CompareType.Minor
                            strRet = "CONVERT(NVARCHAR(4000), ISNULL([Value],'')) < '" & _Value & "' "
                        Case CompareType.MinorEqual
                            strRet = " CONVERT(NVARCHAR(4000), ISNULL([Value],'')) <='" & _Value & "' "
                        Case CompareType.Major
                            strRet = " CONVERT(NVARCHAR(4000), ISNULL([Value],'')) > '" & _Value & "' "
                        Case CompareType.MajorEqual
                            strRet = "CONVERT(NVARCHAR(4000), ISNULL([Value],'')) >= '" & _Value & "' "
                        Case CompareType.Distinct
                            strRet = "CONVERT(NVARCHAR(4000), ISNULL([Value],'')) <> '" & _Value & "' "
                        Case CompareType.Contains
                            _Value = _Value.Replace("_", "ð_").Replace("%", "ð%").Replace("[", "ð[")
                            strRet = "CONVERT(NVARCHAR(4000), ISNULL([Value],'')) LIKE '%" & _Value & "%' ESCAPE 'ð'"
                        Case CompareType.NotContains
                            _Value = _Value.Replace("_", "ð_").Replace("%", "ð%").Replace("[", "ð[")
                            strRet = "CONVERT(NVARCHAR(4000), ISNULL([Value],'')) NOT LIKE '%" & _Value & "%' ESCAPE 'ð'"
                        Case CompareType.StartWith
                            _Value = _Value.Replace("_", "ð_").Replace("%", "ð%").Replace("[", "ð[")
                            strRet = "CONVERT(NVARCHAR(4000), ISNULL([Value],'')) LIKE '" & _Value & "%' ESCAPE 'ð'"
                        Case CompareType.EndWidth
                            _Value = _Value.Replace("_", "ð_").Replace("%", "ð%").Replace("[", "ð[")
                            strRet = "CONVERT(NVARCHAR(4000), ISNULL([Value],'')) LIKE '%" & _Value & "' ESCAPE 'ð'"
                    End Select

                Case FieldTypes.tNumeric
                    _defaultFieldValue = "0"
                    If IsNumeric(_Value) Then
                        _Value = CLng(_Value)
                        Select Case eCompare
                            Case CompareType.Equal
                                strRet = "CONVERT(int, CONVERT(NVARCHAR(4000), ISNULL([Value],0))) = " & _Value
                            Case CompareType.Minor
                                strRet = "CONVERT(int, CONVERT(NVARCHAR(4000), ISNULL([Value],0))) < " & _Value
                            Case CompareType.MinorEqual
                                strRet = " CONVERT(int, CONVERT(NVARCHAR(4000), ISNULL([Value],0))) <=" & _Value
                            Case CompareType.Major
                                strRet = " CONVERT(int, CONVERT(NVARCHAR(4000), ISNULL([Value],0))) > " & _Value
                            Case CompareType.MajorEqual
                                strRet = "CONVERT(int, CONVERT(NVARCHAR(4000), ISNULL([Value],0))) >= " & _Value
                            Case CompareType.Distinct
                                strRet = "CONVERT(int, CONVERT(NVARCHAR(4000), ISNULL([Value],0))) <> " & _Value
                        End Select
                    End If

                Case FieldTypes.tDecimal
                    _defaultFieldValue = "0.0"
                    If IsNumeric(_Value) Then
                        _Value = CStr(CDbl(_Value)).Replace(roConversions.GetDecimalDigitFormat(), ".")
                        Select Case eCompare
                            Case CompareType.Equal
                                strRet = "CONVERT(Numeric(16,6), CONVERT(NVARCHAR(4000), ISNULL([Value],0))) = " & _Value
                            Case CompareType.Minor
                                strRet = "CONVERT(Numeric(16,6), CONVERT(NVARCHAR(4000), ISNULL([Value],0))) < " & _Value
                            Case CompareType.MinorEqual
                                strRet = " CONVERT(Numeric(16,6), CONVERT(NVARCHAR(4000), ISNULL([Value],0))) <=" & _Value
                            Case CompareType.Major
                                strRet = " CONVERT(Numeric(16,6), CONVERT(NVARCHAR(4000), ISNULL([Value],0))) > " & _Value
                            Case CompareType.MajorEqual
                                strRet = "CONVERT(Numeric(16,6), CONVERT(NVARCHAR(4000), ISNULL([Value],0))) >= " & _Value
                            Case CompareType.Distinct
                                strRet = "CONVERT(Numeric(16,6), CONVERT(NVARCHAR(4000), ISNULL([Value],0))) <> " & _Value
                        End Select
                    End If

                Case FieldTypes.tDate
                    _defaultFieldValue = "19000101"
                    _Value = "CONVERT(smalldatetime, '" & roTypes.Any2Time(_Value).Value & "', 120)"
                    Select Case eCompare
                        Case CompareType.Equal
                            strRet = "CONVERT(smalldatetime, CONVERT(NVARCHAR(4000), [Value]), 120) = " & _Value
                        Case CompareType.Minor
                            strRet = "CONVERT(smalldatetime, CONVERT(NVARCHAR(4000), [Value]), 120) < " & _Value
                        Case CompareType.MinorEqual
                            strRet = " CONVERT(smalldatetime, CONVERT(NVARCHAR(4000), [Value]), 120) <=" & _Value
                        Case CompareType.Major
                            strRet = " CONVERT(smalldatetime, CONVERT(NVARCHAR(4000), [Value]), 120) > " & _Value
                        Case CompareType.MajorEqual
                            strRet = "CONVERT(smalldatetime, CONVERT(NVARCHAR(4000), [Value]), 120) >= " & _Value
                        Case CompareType.Distinct
                            strRet = "CONVERT(smalldatetime, CONVERT(NVARCHAR(4000), [Value]), 120) <> " & _Value
                    End Select

                Case FieldTypes.tTime
                    _defaultFieldValue = "0"
                    _Value = "CONVERT(float, '" & CStr(roConversions.ConvertTimeToHours(_Value)).Replace(roConversions.GetDecimalDigitFormat(), ".") & "')"
                    Select Case eCompare
                        Case CompareType.Equal
                            strRet = "CONVERT(float, ISNULL([Value],0)) = " & _Value
                        Case CompareType.Minor
                            strRet = "CONVERT(float, ISNULL([Value],0)) < " & _Value
                        Case CompareType.MinorEqual
                            strRet = " CONVERT(float, ISNULL([Value],0)) <=" & _Value
                        Case CompareType.Major
                            strRet = " CONVERT(float, ISNULL([Value],0)) > " & _Value
                        Case CompareType.MajorEqual
                            strRet = "CONVERT(float, ISNULL([Value],0)) >= " & _Value
                        Case CompareType.Distinct
                            strRet = "CONVERT(float, ISNULL([Value],0)) <> " & _Value
                    End Select

                Case FieldTypes.tDatePeriod
                    _defaultFieldValue = "1900/01/01*2079/01/01"
                    'TODO: Sólo se deberían aceptar cadenas de texto con formato fecha1*fecha2
                    Dim _ValueBegin As String = _Value.Split("*")(0)
                    If _ValueBegin = "" Then _ValueBegin = "1900/01/01"
                    Dim _ValueEnd As String = ""
                    If _Value.Split("*").Length > 1 Then _ValueEnd = _Value.Split("*")(1)
                    If _ValueEnd = "" Then _ValueEnd = "2079/01/01"
                    If _Value = "" Then _Value = "1900/01/01"

                    Select Case eCompare
                        Case CompareType.Equal
                            strRet = "CONVERT(NVARCHAR(4000), [Value]) = '" & _Value & "'"

                        Case CompareType.Distinct
                            strRet = "CONVERT(NVARCHAR(4000), [Value]) <> '" & _Value & "'"
                        Case CompareType.Contains
                            If _Value.Contains("*") Then
                                strRet = "CONVERT(smalldatetime, CASE WHEN SUBSTRING(ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'), 1, CHARINDEX('*',ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'))-1) = '' THEN '1900/01/01' ELSE SUBSTRING(ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'), 1, CHARINDEX('*',ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'))-1) END, 120) <= CONVERT(smalldatetime, '" & _ValueBegin & "', 120) AND " &
                                         "CONVERT(smalldatetime, CASE WHEN SUBSTRING(ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'), CHARINDEX('*',ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'))+1, LEN(ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'))) = '' THEN '2079/01/01' ELSE SUBSTRING(ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'), CHARINDEX('*',ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'))+1, LEN(ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'))) END, 120) >= CONVERT(smalldatetime, '" & _ValueEnd & "', 120)"
                            Else
                                strRet = "CONVERT(smalldatetime, CASE WHEN SUBSTRING(ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'), 1, CHARINDEX('*',ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'))-1) = '' THEN '1900/01/01' ELSE SUBSTRING(ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'), 1, CHARINDEX('*',ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'))-1) END, 120) <= CONVERT(smalldatetime, '" & _Value & "', 120) AND " &
                                         "CONVERT(smalldatetime, CASE WHEN SUBSTRING(ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'), CHARINDEX('*',ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'))+1, LEN(ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'))) = '' THEN '2079/01/01' ELSE SUBSTRING(ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'), CHARINDEX('*',ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'))+1, LEN(ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'))) END, 120) >= CONVERT(smalldatetime, '" & _Value & "', 120)"
                            End If
                        Case CompareType.NotContains
                            If _Value.Contains("*") Then
                                strRet = "CONVERT(smalldatetime, CASE WHEN SUBSTRING(ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'), 1, CHARINDEX('*',ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'))-1) = '' THEN '1900/01/01' ELSE SUBSTRING(ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'), 1, CHARINDEX('*',ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'))-1) END, 120) > CONVERT(smalldatetime, '" & _ValueBegin & "', 120) OR " &
                                         "CONVERT(smalldatetime, CASE WHEN SUBSTRING(ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'), CHARINDEX('*',ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'))+1, LEN(ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'))) = '' THEN '2079/01/01' ELSE SUBSTRING(ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'), CHARINDEX('*',ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'))+1, LEN(ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'))) END, 120) <CONVERT(smalldatetime, '" & _ValueEnd & "', 120)"
                            Else
                                strRet = "CONVERT(smalldatetime, CASE WHEN SUBSTRING(ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'), 1, CHARINDEX('*',ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'))-1) = '' THEN '1900/01/01' ELSE SUBSTRING(ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'), 1, CHARINDEX('*',ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'))-1) END, 120) > CONVERT(smalldatetime, '" & _Value & "', 120) OR " &
                                         "CONVERT(smalldatetime, CASE WHEN SUBSTRING(ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'), CHARINDEX('*',ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'))+1, LEN(ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'))) = '' THEN '2079/01/01' ELSE SUBSTRING(ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'), CHARINDEX('*',ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'))+1, LEN(ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'))) END, 120) <CONVERT(smalldatetime, '" & _Value & "', 120)"
                            End If
                    End Select

                Case FieldTypes.tTimePeriod
                    _defaultFieldValue = "1900/01/01 00:00:00*2079/01/01 00:00:00"
                    Dim _ValueBegin As String = _Value.Split("*")(0)
                    If _ValueBegin = "" Then _ValueBegin = "1900/01/01 00:00:00"
                    Dim _ValueEnd As String = ""
                    If _Value.Split("*").Length > 1 Then _ValueEnd = _Value.Split("*")(1)
                    If _ValueEnd = "" Then _ValueEnd = "2079/01/01 00:00:00"
                    If _Value = "" Then _Value = "1900/01/01 00:00:00"

                    Select Case eCompare
                        Case CompareType.Equal
                            strRet = _FieldName & " = '" & _Value & "'"
                        Case CompareType.Distinct
                            strRet = _FieldName & " <> '" & _Value & "'"
                        Case CompareType.Contains
                            If _Value.Contains("*") Then
                                strRet = "CONVERT(datetime, CASE WHEN SUBSTRING(ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'), 1, CHARINDEX('*',ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'))-1) = '' THEN '1900/01/01 00:00:00' ELSE SUBSTRING(ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'), 1, CHARINDEX('*',ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'))-1) END, 120) <= CONVERT(datetime, '" & _ValueBegin & "', 120) AND " &
                                         "CONVERT(datetime, CASE WHEN SUBSTRING(ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'), CHARINDEX('*',ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'))+1, LEN(ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'))) = '' THEN '2079/01/01 00:00:00' ELSE SUBSTRING(ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'), CHARINDEX('*',ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'))+1, LEN(ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'))) END, 120) >= CONVERT(datetime, '" & _ValueEnd & "', 120)"
                            Else
                                strRet = "CONVERT(datetime, CASE WHEN SUBSTRING(ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'), 1, CHARINDEX('*',ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'))-1) = '' THEN '1900/01/01' ELSE SUBSTRING(ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'), 1, CHARINDEX('*',ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'))-1) END, 120) <= CONVERT(smalldatetime, '" & _Value & "', 120) AND " &
                                         "CONVERT(datetime, CASE WHEN SUBSTRING(ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'), CHARINDEX('*',ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'))+1, LEN(ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'))) = '' THEN '2079/01/01' ELSE SUBSTRING(ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'), CHARINDEX('*',ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'))+1, LEN(ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'))) END, 120) >= CONVERT(datetime, '" & _Value & "', 120)"
                            End If
                        Case CompareType.NotContains
                            If _Value.Contains("*") Then
                                strRet = "CONVERT(datetime, CASE WHEN SUBSTRING(ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'), 1, CHARINDEX('*',ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'))-1) = '' THEN '1900/01/01 00:00:00' ELSE SUBSTRING(ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'), 1, CHARINDEX('*',ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'))-1) END, 120) > CONVERT(smalldatetime, '" & _ValueBegin & "', 120) OR " &
                                         "CONVERT(datetime, CASE WHEN SUBSTRING(ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'), CHARINDEX('*',ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'))+1, LEN(ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'))) = '' THEN '2079/01/01' ELSE SUBSTRING(ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'), CHARINDEX('*',ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'))+1, LEN(ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'))) END, 120) < CONVERT(datetime, '" & _ValueEnd & "', 120)"
                            Else
                                strRet = "CONVERT(datetime, CASE WHEN SUBSTRING(ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'), 1, CHARINDEX('*',ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'))-1) = '' THEN '1900/01/01 00:00:00' ELSE SUBSTRING(ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'), 1, CHARINDEX('*',ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'))-1) END, 120) > CONVERT(smalldatetime, '" & _Value & "', 120) OR " &
                                         "CONVERT(datetime, CASE WHEN SUBSTRING(ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'), CHARINDEX('*',ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'))+1, LEN(ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'))) = '' THEN '2079/01/01' ELSE SUBSTRING(ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'), CHARINDEX('*',ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'))+1, LEN(ISNULL(CONVERT(NVARCHAR(4000), [Value]),'*'))) END, 120) < CONVERT(datetime, '" & _Value & "', 120)"
                            End If
                    End Select

                Case FieldTypes.tLink
                    _defaultFieldValue = ""
                    _Value = _Value.Replace("'", "''")
                    Select Case eCompare
                        Case CompareType.Equal
                            strRet = "CONVERT(NVARCHAR(4000), ISNULL([Value],'')) = '" & _Value & "' "
                        Case CompareType.Minor
                            strRet = "CONVERT(NVARCHAR(4000), ISNULL([Value],'')) < '" & _Value & "' "
                        Case CompareType.MinorEqual
                            strRet = "CONVERT(NVARCHAR(4000), ISNULL([Value],'')) <= '" & _Value & "' "
                        Case CompareType.Major
                            strRet = "CONVERT(NVARCHAR(4000), ISNULL([Value],'')) > '" & _Value & "' "
                        Case CompareType.MajorEqual
                            strRet = "CONVERT(NVARCHAR(4000), ISNULL([Value],'')) >= '" & _Value & "' "
                        Case CompareType.Distinct
                            strRet = "CONVERT(NVARCHAR(4000), ISNULL([Value],'')) <> '" & _Value & "' "
                        Case CompareType.Contains
                            _Value = _Value.Replace("_", "ð_").Replace("%", "ð%").Replace("[", "ð[")
                            strRet = "CONVERT(NVARCHAR(4000), ISNULL([Value],'')) LIKE '%" & _Value & "%' ESCAPE 'ð'"
                        Case CompareType.NotContains
                            _Value = _Value.Replace("_", "ð_").Replace("%", "ð%").Replace("[", "ð[")
                            strRet = "CONVERT(NVARCHAR(4000), ISNULL([Value],'')) NOT LIKE '%" & _Value & "%' ESCAPE 'ð'"
                        Case CompareType.StartWith
                            _Value = _Value.Replace("_", "ð_").Replace("%", "ð%").Replace("[", "ð[")
                            strRet = "CONVERT(NVARCHAR(4000), ISNULL([Value],'')) LIKE '" & _Value & "%' ESCAPE 'ð'"
                        Case CompareType.EndWidth
                            _Value = _Value.Replace("_", "ð_").Replace("%", "ð%").Replace("[", "ð[")
                            strRet = "CONVERT(NVARCHAR(4000), ISNULL([Value],'')) LIKE '%" & _Value & "' ESCAPE 'ð'"
                    End Select

                Case FieldTypes.tDocument
                    _defaultFieldValue = ""
                    _Value = _Value.Replace("'", "''")
                    Select Case eCompare
                        Case CompareType.Distinct
                            strRet = "'0' <> '" & _Value & "'"
                    End Select

            End Select

            If strRet <> "" Then

                If IDEmployee = -1 Then
                    strRet = "Employees.ID IN (@SELECT# IDEmployee " &
                         "FROM [dbo].[GetAllEmployeeUserFieldValue]('" & _FieldName & "', " & roTypes.Any2Time(dDate).SQLSmallDateTime & ") " &
                         "WHERE " & strRet & ")"
                Else
                    strRet = "Employees.ID IN (@SELECT# tmpField.IDEmployee from (@SELECT# " & IDEmployee & " AS IDEmployee, ISNULL(dbo.GetValueFromEmployeeUserFieldValuesWithDefault(" & IDEmployee & ",'" & _FieldName & "', " & roTypes.Any2Time(dDate).SQLSmallDateTime & ",'" & _defaultFieldValue & "'),'') as Value) as tmpField WHERE " & strRet & ")"
                End If

            End If

            Return strRet

        End Function


#End Region

#Region "Collective Employees"

        Public Function GetCollectiveEmployees(collectiveId As Integer, referenceDate As Date) As DataTable
            Dim returnCollectiveEmployees As DataTable = Nothing

            Try
                returnCollectiveEmployees = GetCollectiveEmployees({collectiveId}.ToList, 0, referenceDate, referenceDate)
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roSelectorManager:GetCollectiveEmployees::Unkown exeception", ex)
            End Try
            Return returnCollectiveEmployees
        End Function

        Public Function GetCollectiveEmployees(collectiveIds As List(Of Integer), idEmployee As Integer, beginDate As Date, endDate As Date) As DataTable
            Dim returnCollectiveEmployees As New DataTable

            Try
                roTrace.GetInstance().InitTraceEvent()
                Using sqlcommand As DbCommand = AccessHelper.CreateCommand("CheckCollectivesEmployees")
                    sqlcommand.CommandType = CommandType.StoredProcedure

                    ' Crear y agregar parámetros
                    Dim parameter As DbParameter = AccessHelper.AddParameter(sqlcommand, "@CollectiveIDs", DbType.AnsiString, 4000)
                    parameter.Direction = ParameterDirection.Input
                    parameter.Value = String.Join(",", collectiveIds)

                    parameter = AccessHelper.AddParameter(sqlcommand, "@EmployeeID", DbType.Int32, 4)
                    parameter.Direction = ParameterDirection.Input
                    parameter.Value = idEmployee

                    parameter = AccessHelper.AddParameter(sqlcommand, "@StartDate", DbType.DateTime, 8)
                    parameter.Direction = ParameterDirection.Input
                    parameter.Value = beginDate

                    parameter = AccessHelper.AddParameter(sqlcommand, "@EndDate", DbType.DateTime, 8)
                    parameter.Direction = ParameterDirection.Input
                    parameter.Value = endDate

                    Dim errorMessageParam = AccessHelper.AddParameter(sqlcommand, "@ErrorMessage", DbType.AnsiString, 4000)
                    errorMessageParam.Direction = ParameterDirection.Output

                    Dim hasErrorsParam = AccessHelper.AddParameter(sqlcommand, "@HasErrors", DbType.Boolean, 1)
                    hasErrorsParam.Direction = ParameterDirection.Output

                    Using adapter As DbDataAdapter = AccessHelper.CreateDataAdapter(sqlcommand)
                        Dim watch As Stopwatch = Stopwatch.StartNew()
                        adapter.Fill(returnCollectiveEmployees)
                        watch.Stop()
                        roLog.GetInstance().AddSqlProcessTime(watch.Elapsed.TotalSeconds, roLog.ProcessTimeUnit.seconds)
                        returnCollectiveEmployees.DefaultView.ToTable(True, "IDEmployee")
                        ' Recuperar los valores de los parámetros de salida
                        Dim errorMessage As String = Convert.ToString(errorMessageParam.Value)
                        Dim hasErrors As Boolean = Convert.ToBoolean(hasErrorsParam.Value)

                        ' Ahora puedes usar estos valores según sea necesario
                        If hasErrors Then
                            ' Manejar el error
                            roLog.GetInstance().logMessage(roLog.EventType.roError, $"Error en CheckCollectivesEmployees: {errorMessage}")
                        End If
                    End Using
                End Using

                roTrace.GetInstance().AddTraceEvent($"Collectives employees recovered: collectives -> {String.Join(",", collectiveIds)} employee -> {idEmployee} begindate -> {beginDate} enddate -> {endDate}")

                Return returnCollectiveEmployees

            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roSelectorManager:GetCollectiveEmployees::Unkown exeception", ex)
            End Try
            Return returnCollectiveEmployees
        End Function

#End Region

#Region "LabAgree Employees"
        Public Function GetLabAgreeEmployees(labagreeIds As List(Of Integer), referenceDate As Date) As DataTable
            Dim returnLabAgreeEmployees As DataTable = Nothing
            Try
                Dim sqlCommand As String = $"@SELECT# EmployeeContracts.IDEmployee  
                                          FROM EmployeeContracts 
                                          INNER JOIN sysrovwCurrentEmployeeGroups ON sysrovwCurrentEmployeeGroups.IDEmployee = EmployeeContracts.IDEmployee
                                          INNER JOIN Labagree ON labagree.id = EmployeeContracts.IDLabAgree AND LabAgree.Id IN ({String.Join(",", labagreeIds)})
                                          WHERE CAST({roTypes.Any2Time(referenceDate).SQLSmallDateTime} as Date) BETWEEN EmployeeContracts.BeginDate AND EmployeeContracts.EndDate"

                returnLabAgreeEmployees = AccessHelper.CreateDataTable(sqlCommand)

            Catch ex As DbException
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roSelectorManager:GetLabAgreeEmployees::Unkown exeception", ex)
            End Try

            Return returnLabAgreeEmployees
        End Function
#End Region

#Region "Helper Methods"
        Public Function GetAllEmployees(labagreeId As Integer, referenceDate As Date) As DataTable
            Dim returnLabAgreeEmployees As DataTable = Nothing
            Try
                Dim sqlCommand As String = $"@SELECT# EmployeeContracts.IDEmployee  
                                          FROM EmployeeContracts 
                                          INNER JOIN sysrovwCurrentEmployeeGroups ON sysrovwCurrentEmployeeGroups.IDEmployee = EmployeeContracts.IDEmployee
                                          INNER JOIN Labagree ON labagree.id = EmployeeContracts.IDLabAgree AND LabAgree.Id = {labagreeId}
                                          WHERE CAST({roTypes.Any2Time(referenceDate).SQLSmallDateTime} as Date) BETWEEN EmployeeContracts.BeginDate AND EmployeeContracts.EndDate"

                returnLabAgreeEmployees = AccessHelper.CreateDataTable(sqlCommand)

            Catch ex As DbException
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roSelectorManager:GetAllEmployees::Unkown exeception", ex)
            End Try

            Return returnLabAgreeEmployees
        End Function

        Public Function TableIntersect(table1 As DataTable, table2 As DataTable) As DataTable
            Dim resultTable As DataTable = table1.Clone()
            Try
                Dim intersection = table1.AsEnumerable().Join(
                                        table2.AsEnumerable(),
                                        Function(row1) row1.Field(Of Integer)("IDEmployee"),
                                        Function(row2) row2.Field(Of Integer)("IDEmployee"),
                                        Function(row1, row2) row1)

                If intersection.Any() Then
                    resultTable = intersection.CopyToDataTable()
                End If

            Catch ex As Exception
                'Do nothing
            End Try
            Return resultTable
        End Function
#End Region

    End Class
End Namespace
