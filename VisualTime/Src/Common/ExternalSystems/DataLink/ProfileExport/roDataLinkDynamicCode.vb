Imports System.Data.Common
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase

Namespace DataLink



    Public Class DataLinkDynamicCode
        Inherits roDataLinkExport

        Private Shared oDynamicClasses As New Hashtable
        Private Shared oDynamicClassesTimeStamps As New Hashtable

#Region "DataAdapters globales para códgio dinámico"

        Public Shared Function CreateDataTable_Employees(ByVal EmployeeID As Integer, ByRef oState As roDataLinkState) As DataRow
            Dim dt As New DataTable

            Try
                Dim strSQL As String = "@SELECT# * FROM Employees WHERE ID=@idEmployee"
                Dim cmd As DbCommand = CreateCommand(strSQL)

                AddParameter(cmd, "@idEmployee", DbType.Int32)
                Dim da As DbDataAdapter = CreateDataAdapter(cmd, False)

                da.SelectCommand.Parameters("@idEmployee").Value = EmployeeID
                dt.Rows.Clear()
                da.Fill(dt)
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportEmployees:CreateDataTable_Employees")
                Return dt.NewRow
            End Try

            If dt.Rows.Count = 0 Then
                Return dt.NewRow
            Else
                Return dt.Rows(0)
            End If

        End Function

        Public Shared Function CreateDataTable_EmployeeMobilities(ByVal EmployeeID As Integer, ByRef oState As roDataLinkState) As DataTable
            Dim dt As New DataTable

            Try
                Dim strSQL As String = "@SELECT# Groups.*, BeginDate, EndDate from EmployeeGroups inner join Groups on Groups.ID = EmployeeGroups.IDGroup where IDEmployee = @idEmployee ORDER BY BeginDate asc"
                Dim cmd As DbCommand = CreateCommand(strSQL)

                AddParameter(cmd, "@idEmployee", DbType.Int32)
                Dim da As DbDataAdapter = CreateDataAdapter(cmd, False)

                da.SelectCommand.Parameters("@idEmployee").Value = EmployeeID
                dt.Rows.Clear()
                da.Fill(dt)
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportEmployees:CreateDataTable_EmployeeMobilities")
            End Try

            Return dt
        End Function

        Public Shared Function GetEmployeeUserFieldValue(ByVal Value As String, FieldType As FieldTypes) As String
            If IsNothing(Value) Then Value = ""

            Dim strValue As String = Value

            Try
                Select Case FieldType
                    Case FieldTypes.tDecimal, FieldTypes.tTime
                        Dim oInfo As System.Globalization.NumberFormatInfo = System.Globalization.CultureInfo.CurrentCulture.NumberFormat
                        strValue = Value.Replace(".", oInfo.CurrencyDecimalSeparator)

                End Select
            Catch ex As Exception

            End Try

            Return strValue
        End Function

        Public Shared Function CreateDataTable_EmployeeUserFields(ByVal EmployeeID As Integer, ByVal xDate As Date, ByRef oState As roDataLinkState) As DataTable
            Dim dt As New DataTable

            Try
                Dim strDateNow As String = "'" & xDate.ToString("yyyyMMdd") & "'"
                Dim strSQL As String = "@SELECT# GEAUFV.FieldName, value,Date, sysroUserFields.FieldType " &
                    "from GetEmployeeAllUserFieldValue(@idEmployee," & strDateNow & ") as GEAUFV INNER JOIN sysroUserFields ON GEAUFV.FieldName=sysroUserFields.FIELDNAME COLLATE Modern_Spanish_CI_AS"

                Dim cmd As DbCommand = CreateCommand(strSQL)

                AddParameter(cmd, "@idEmployee", DbType.Int32)
                Dim da As DbDataAdapter = CreateDataAdapter(cmd, False)

                da.SelectCommand.Parameters("@idEmployee").Value = EmployeeID
                dt.Rows.Clear()
                da.Fill(dt)
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportEmployees:CreateDataTable_EmployeeUserFields")
            End Try

            Return dt
        End Function

        Public Shared Function CreateDataTable_EmployeeUserFieldValuesHistory(ByVal EmployeeID As Integer, ByVal sFilter As String, ByRef oState As roDataLinkState) As DataTable
            Dim dt As New DataTable

            Try
                If sFilter.Length > 0 Then
                    Dim strSQL As String
                    strSQL = "@SELECT# Date, Date As BeginDate, getdate() As EndDate, employeeuserfieldvalues.FieldName, Value, sysroUserFields.FieldType from employeeuserfieldvalues, sysroUserFields  where idemployee = " & EmployeeID.ToString & " AND sysroUserFields.FieldName = employeeuserfieldvalues.FieldName And employeeuserfieldvalues.FieldName in  (" & sFilter & ") AND sysroUserFields.Type = 0  ORDER by employeeuserfieldvalues.FieldName asc, Date desc"

                    Dim cmd As DbCommand = CreateCommand(strSQL)

                    Dim da As DbDataAdapter = CreateDataAdapter(cmd, False)

                    dt.Rows.Clear()
                    da.Fill(dt)

                    ' Ahora recorro e informo la fecha fin del cada valor
                    Dim sFieldName As String = String.Empty
                    Dim sLastFieldName As String = String.Empty
                    Dim dDate, dLastDate As DateTime
                    sLastFieldName = String.Empty
                    If dt.Rows.Count > 0 Then
                        For Each oEmpUserFieldValue In dt.Rows
                            If sFieldName.Trim <> String.Empty Then
                                sFieldName = roTypes.Any2String(oEmpUserFieldValue("FieldName"))
                                dDate = oEmpUserFieldValue("Date")
                                If sFieldName.Trim <> String.Empty AndAlso sFieldName <> sLastFieldName Then
                                    oEmpUserFieldValue("EndDate") = DateSerial(2079, 1, 1)
                                Else
                                    oEmpUserFieldValue("EndDate") = dLastDate.AddDays(-1)
                                End If
                                sLastFieldName = sFieldName
                                dLastDate = dDate
                            End If
                        Next
                    End If
                End If
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportEmployees:CreateDataTable_EmployeeUserFieldValuesHistory")
            End Try

            Return dt
        End Function

        Public Shared Function CreateDataTable_Login(ByVal EmployeeID As Integer, ByRef oState As roDataLinkState) As DataTable
            Dim dt As New DataTable

            Try
                Dim strSQL As String = "@SELECT# srpp.Name ,srCr.Credential as Login,srCa.Credential as Card from sysroPassports srp " &
                                        "left join sysroPassports srpp on srp.IDParentPassport = srpp.ID " &
                                        "left join sysroPassports_AuthenticationMethods srCr on srp.ID = srCr.IDPassport and srCr.Method = 1 " &
                                        "left join sysroPassports_AuthenticationMethods srCa on srp.ID = srCa.IDPassport and srCa.Method = 3 " &
                                        "where srp.IDEmployee =@idEmployee"
                Dim cmd As DbCommand = CreateCommand(strSQL)

                AddParameter(cmd, "@idEmployee", DbType.Int32)

                Dim da As DbDataAdapter = CreateDataAdapter(cmd, False)
                da.SelectCommand.Parameters("@idEmployee").Value = EmployeeID
                dt.Rows.Clear()
                da.Fill(dt)
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportTasks:CreateDataTable_Login")
            End Try

            Return dt
        End Function

        Public Shared Function CreateDataTable_Language(ByVal EmployeeID As Integer, ByRef oState As roDataLinkState) As DataTable
            Dim dt As New DataTable

            Try
                Dim strSQL As String = "@SELECT# LanguageKey from sysroPassports as sp " &
                                        "inner join sysroLanguages as sl on sp.IDLanguage = sl.ID " &
                                        "where IDEmployee =@idEmployee"
                Dim cmd As DbCommand = CreateCommand(strSQL)

                AddParameter(cmd, "@idEmployee", DbType.Int32)

                Dim da As DbDataAdapter = CreateDataAdapter(cmd, False)
                da.SelectCommand.Parameters("@idEmployee").Value = EmployeeID
                dt.Rows.Clear()
                da.Fill(dt)
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportTasks:CreateDataTable_Language")
            End Try

            Return dt
        End Function

        Public Shared Function CreateDataTable_Authorizations(ByVal EmployeeID As Integer, ByRef oState As roDataLinkState) As DataTable
            Dim dt As New DataTable

            Try
                Dim strSQL As String = "@SELECT# AccessGroups.ShortName As ShortName FROM sysrovwAccessAuthorizations inner join AccessGroups on AccessGroups.ID = sysrovwAccessAuthorizations.IDAuthorization WHERE IDEmployee = " & EmployeeID
                strSQL = strSQL & " UNION @SELECT# AccessGroups.ShortName As ShortName  FROM Employees inner join AccessGroups on AccessGroups.ID = Employees.IDAccessGroup  WHERE Employees.ID = " & EmployeeID & " AND IDAccessGroup IS NOT NULL"

                Dim cmd As DbCommand = CreateCommand(strSQL)

                AddParameter(cmd, "@idEmployee", DbType.Int32)

                Dim da As DbDataAdapter = CreateDataAdapter(cmd, False)
                da.SelectCommand.Parameters("@idEmployee").Value = EmployeeID
                dt.Rows.Clear()
                da.Fill(dt)
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportTasks:CreateDataTable_Login")
            End Try

            Return dt
        End Function

        Public Shared Function CreateDataTable_Contracts(ByVal EmployeeID As Integer, ByRef oState As roDataLinkState) As DataTable
            Dim dt As New DataTable

            Try
                Dim strSQL As String =
                    "@SELECT# EmployeeContracts.*, ISNULL(LabAgree.Name,'') AS LabAgreeName from EmployeeContracts LEFT JOIN LabAgree ON EmployeeContracts.IDLabAgree = LabAgree.ID " &
                    "WHERE EmployeeContracts.IDEmployee=@idEmployee ORDER BY EmployeeContracts.BeginDate DESC"
                Dim cmd As DbCommand = CreateCommand(strSQL)

                AddParameter(cmd, "@idEmployee", DbType.Int32)

                Dim da As DbDataAdapter = CreateDataAdapter(cmd, False)
                da.SelectCommand.Parameters("@idEmployee").Value = EmployeeID
                dt.Rows.Clear()
                da.Fill(dt)
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportTasks:CreateDataTable_Contracts")
            End Try

            Return dt
        End Function

        Public Shared Function CreateDataTable_Groups(ByVal EmployeeID As Integer, ByRef oState As roDataLinkState) As DataTable
            Dim dt As New DataTable

            Try
                Dim strDateNow As String = "'" & Date.Now.ToString("yyyyMMdd") & "'"
                Dim strSQL As String = "@SELECT# Groups.* from Groups inner join dbo.GetEmployeeGroupTree(@idEmployee,null," & strDateNow & ") eg on Groups.ID = eg.ID" &
                                        " order by Path ASC"

                Dim cmd As DbCommand = CreateCommand(strSQL)

                AddParameter(cmd, "@idEmployee", DbType.Int32)

                Dim da As DbDataAdapter = CreateDataAdapter(cmd, False)
                da.SelectCommand.Parameters("@idEmployee").Value = EmployeeID
                dt.Rows.Clear()
                da.Fill(dt)
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportEmployees:CreateDataTable_Groups")
            End Try

            Return dt
        End Function

        Public Shared Function CreateDataTable_GroupsOnDate(ByVal EmployeeID As Integer, ByVal dDate As DateTime, ByRef oState As roDataLinkState) As DataTable
            Dim dt As New DataTable

            Try
                Dim strDateNow As String = "'" & dDate.ToString("yyyyMMdd") & "'"
                Dim strSQL As String = "@SELECT# Groups.*, BusinessCenters.Name BusinessCenterName from Groups " &
                                       "inner join dbo.GetEmployeeGroupTree(@idEmployee,null," & strDateNow & ") eg on Groups.ID = eg.ID " &
                                       "left join dbo.BusinessCenters on groups.idcenter = BusinessCenters.id " &
                                       "order by Path ASC"

                Dim cmd As DbCommand = CreateCommand(strSQL)

                AddParameter(cmd, "@idEmployee", DbType.Int32)

                Dim da As DbDataAdapter = CreateDataAdapter(cmd, False)
                da.SelectCommand.Parameters("@idEmployee").Value = EmployeeID
                dt.Rows.Clear()
                da.Fill(dt)
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportEmployees:CreateDataTable_GroupsOnDate")
            End Try

            Return dt
        End Function

#End Region





















    End Class

End Namespace