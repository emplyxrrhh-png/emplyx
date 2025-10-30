Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase.Extensions.roServerLicense
Imports Robotics.VTBase.roTypes

Public Class roLicenseSupport

    Sub New()
    End Sub

    Public Function GetVTLicenseInfo() As roVTLicense

        Dim oLicInfo As New roVTLicense

        Try
            Dim LicenseService As New roServerLicense

            oLicInfo.MaxExmployees = Any2Long(LicenseService.FeatureData("VisualTime Server", "MaxEmployees"))

            If roTypes.Any2String(LicenseService.FeatureData("VisualTime Server", "MaxJobEmployees")) <> String.Empty Then
                oLicInfo.MaxJobExmployees = Any2Long(LicenseService.FeatureData("VisualTime Server", "MaxJobEmployees"))
            Else
                oLicInfo.MaxJobExmployees = 0
            End If

            oLicInfo.IsProductiv = Any2Boolean(LicenseService.FeatureIsInstalled("Feature\Productiv"))

            Try
                oLicInfo.ServerLicensceStatus = LicenseService.Status(roLog.GetInstance())
            Catch ex As Exception
                oLicInfo.ServerLicensceStatus = roLicenseStatus.roLicense_Active
            End Try

            Try
                If String.IsNullOrEmpty(LicenseService.FeatureData("Version", "Edition")) Then
                    oLicInfo.Edition = roServerLicense.roVisualTimeEdition.NotSet
                Else
                    oLicInfo.Edition = System.Enum.Parse(GetType(roServerLicense.roVisualTimeEdition), Any2String(LicenseService.FeatureData("Version", "Edition")), True)
                End If
            Catch ex As Exception
                oLicInfo.Edition = roServerLicense.roVisualTimeEdition.NotSet
            End Try

            Dim tmpExpirationDate As String = Any2String(LicenseService.FeatureData("License", "EvalExpires"))
            Try
                If tmpExpirationDate <> String.Empty Then
                    oLicInfo.ExpirationDate = New DateTime(String2Item(tmpExpirationDate, 2, "/"), String2Item(tmpExpirationDate, 1, "/"), String2Item(tmpExpirationDate, 0, "/"), 0, 0, 0)
                Else
                    oLicInfo.ExpirationDate = New Date(2079, 1, 1)
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "GetVTLicenseInfo::error", ex)
            End Try
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "GetVTLicenseInfo::error", ex)
        End Try

        Return oLicInfo
    End Function

    Public Function CheckLicenseLimits(ByVal xDateTime As DateTime, ByVal oLicInfo As roVTLicense) As Boolean

        Dim bolRet As Boolean = False
        Try
            bolRet = False
            ' Empleados de presencia
            Dim intActiveEmployees As Long = Any2Long(GetActiveEmployeesCount(xDateTime))

            If oLicInfo.MaxExmployees < 1 OrElse intActiveEmployees > oLicInfo.MaxExmployees Then
                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "CheckLicenseLimits::Presencia::LicenseExceed::ActiveEmployees::" & intActiveEmployees & "::MaxEmployees::" & oLicInfo.MaxExmployees)
                Return bolRet
            End If

            If oLicInfo.MaxJobExmployees > 0 AndAlso oLicInfo.IsProductiv Then
                Dim Sql As String = "@SELECT# COUNT(Employees.ID) AS TotalProd From Employees,EmployeeContracts WHERE Employees.ID = EmployeeContracts.IDEmployee AND EmployeeContracts.EndDate > " & Any2Time(xDateTime).SQLSmallDateTime & " AND EmployeeContracts.BeginDate<=" & Any2Time(xDateTime).SQLSmallDateTime & " AND Employees.Type ='J'"
                intActiveEmployees = Any2Long(ExecuteScalar(Sql))
                If intActiveEmployees > oLicInfo.MaxJobExmployees Then
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "CheckLicenseLimits::ProductiV::LicenseExceed::ActiveEmployees::" & intActiveEmployees & "::MaxEmployees::" & oLicInfo.MaxExmployees)
                    Return bolRet
                End If
            End If

            bolRet = True
        Catch ex As Exception
            bolRet = True
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roLicenseSupport::CheckLicenseLimits:", ex)
        End Try

        Return bolRet

    End Function

    Public Function GetActiveEmployeesCount(ByVal xDateTime As DateTime) As Integer
        '
        ' Devuelve el numero de empleados en contrato para la fecha indicada
        '
        Dim intRet As Integer = 0
        Try
            Dim strSQL As String
            strSQL = "@SELECT# COUNT(IDEmployee) AS Total From EmployeeContracts"
            strSQL &= " WHERE EmployeeContracts.EndDate > " & Any2Time(xDateTime).SQLSmallDateTime
            strSQL &= " AND EmployeeContracts.BeginDate <= " & Any2Time(xDateTime).SQLSmallDateTime

            intRet = Any2Integer(ExecuteScalar(strSQL))
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roLicenseSupport::GetActiveEmployeesCount:", ex)
        End Try

        Return intRet

    End Function

    Public Function GetActiveJobEmployeesCount(ByVal xDateTime As DateTime) As Integer
        '
        ' Devuelve el numero de empleados en contrato para la fecha indicada
        '
        Dim intRet As Integer = 0
        Try
            Dim strSQL As String
            strSQL = "@SELECT# COUNT(EmployeeTeams.IDEmployee) AS Total From EmployeeTeams,EmployeeContracts"
            strSQL &= " WHERE EmployeeTeams.IDEmployee = EmployeeContracts.IDEmployee"

            strSQL &= " AND EmployeeContracts.EndDate > " & Any2Time(xDateTime).SQLSmallDateTime
            strSQL &= " AND EmployeeContracts.BeginDate <= " & Any2Time(xDateTime).SQLSmallDateTime
            strSQL &= " AND EmployeeTeams.FinishDate > " & Any2Time(xDateTime).SQLSmallDateTime
            strSQL &= " AND EmployeeTeams.BeginDate <= " & Any2Time(xDateTime.Date.AddDays(1).AddSeconds(-1)).SQLSmallDateTime

            intRet = Any2Integer(ExecuteScalar(strSQL))
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roLicenseSupport::GetActiveJobEmployeesCount:", ex)
        End Try

        Return intRet

    End Function

End Class