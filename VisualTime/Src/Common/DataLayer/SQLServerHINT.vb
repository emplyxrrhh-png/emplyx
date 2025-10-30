Public NotInheritable Class SQLServerHint
    Public Enum SelectHinted
        GetRequestPassportLevelOfAuthority
        GetEmployeesStatus
        GetEmployeesDashboardStatus
        GetForecastDocumentationFaultAlerts
        GetDesktopAlerts_EmployeesWithNonJustifiedPunches
        GetDesktopAlerts_EmployeesWithNonReliablePunches
        GetDesktopAlerts_EmployeesThatShouldBePresent
        GetDesktopAlerts_PendingRequests
        GetRequestNextSupervisorsNames
        GetRequestsSupervisor
        GetRequestsDashboardResume
        LoadRowsByCalendar
        GetDirectSupervisorNotification
        DataLink_ExportRequests
        ApproveRefuse
        HasPermissionOverRequest
        GetUserFieldsUsedInShifts
        GetEmployeeInfo
        GetEmployeeListByContract
        GetEmployeeList
        GetAllEmployees
        GetEmployeeAccessLevelMxaAdvancedMode_Data
        GetEmployeeTimeZonesZKPush2_Data
        GetEmployeesLive
        GetEmployeesSQLWithPermissions
        GetEmployeesSQLWithoutPermissions
        GetEmployeesFromGroupWithPermissions
        GetEmployeesFromGroupWithTypeWithPermissions
        GetShiftOldestDate
    End Enum

    Public Enum SQLHint
        None
        ASSUME_JOIN_PREDICATE_DEPENDS_ON_FILTERS
        FORCE_LEGACY_CARDINALITY_ESTIMATION
        QUERY_OPTIMIZER_COMPATIBILITY_LEVEL_150
    End Enum

    Public Shared Function GetSQLHint(sqlHinted As SelectHinted) As String
        Dim resHint As String = String.Empty
        Try
            ' Default values
            Select Case sqlHinted
                Case SelectHinted.GetDesktopAlerts_EmployeesWithNonJustifiedPunches, SelectHinted.GetDesktopAlerts_EmployeesWithNonReliablePunches, SelectHinted.GetDesktopAlerts_EmployeesThatShouldBePresent, SelectHinted.LoadRowsByCalendar, SelectHinted.GetEmployeesSQLWithPermissions, SelectHinted.GetEmployeesFromGroupWithPermissions, SelectHinted.GetEmployeesFromGroupWithTypeWithPermissions
                    resHint = SQLHint.ASSUME_JOIN_PREDICATE_DEPENDS_ON_FILTERS.ToString
                Case SelectHinted.GetEmployeesStatus, SelectHinted.GetRequestNextSupervisorsNames, SelectHinted.GetEmployeesDashboardStatus, SelectHinted.GetUserFieldsUsedInShifts, SelectHinted.GetAllEmployees, SelectHinted.GetEmployeeAccessLevelMxaAdvancedMode_Data, SelectHinted.GetEmployeeTimeZonesZKPush2_Data, SelectHinted.GetEmployeesLive, SelectHinted.GetEmployeesSQLWithoutPermissions, SelectHinted.GetShiftOldestDate
                    resHint = SQLHint.FORCE_LEGACY_CARDINALITY_ESTIMATION.ToString
                Case SelectHinted.GetDesktopAlerts_PendingRequests, SelectHinted.GetDirectSupervisorNotification, SelectHinted.GetRequestsSupervisor, SelectHinted.DataLink_ExportRequests, SelectHinted.ApproveRefuse, SelectHinted.GetRequestPassportLevelOfAuthority, SelectHinted.HasPermissionOverRequest, SelectHinted.GetForecastDocumentationFaultAlerts, SelectHinted.GetRequestsDashboardResume, SelectHinted.GetEmployeeInfo
                    resHint = SQLHint.QUERY_OPTIMIZER_COMPATIBILITY_LEVEL_150.ToString
                Case SelectHinted.GetEmployeeListByContract, SelectHinted.GetEmployeeList
                    resHint = String.Empty
            End Select

            ' Custom values if configured
            Dim resHintCustom As String = roCacheManager.GetInstance().GetAdvParametersCache(Azure.RoAzureSupport.GetCompanyName, $"SQLHint.{sqlHinted}")
            If resHintCustom <> String.Empty Then
                resHint = resHintCustom
                If resHintCustom.ToLower = SQLHint.None.ToString.ToLower Then
                    resHint = String.Empty
                End If
            End If

        Catch ex As Exception
            resHint = String.Empty
        End Try

        If resHint <> String.Empty Then
            resHint = $" OPTION (USE HINT('{resHint}'));"
        End If

        Return resHint
    End Function

End Class
