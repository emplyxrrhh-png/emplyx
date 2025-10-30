Imports System.Data.Common
Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading
Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports System.Diagnostics
Imports System.Data.SqlClient

Public NotInheritable Class AccessHelper

    Private Sub New()
    End Sub

#Region "Connection String"

    Public Shared Function GetConectionString() As String
        Return AccessHelperSql.GetConnectionString()
    End Function

    Public Shared Function GetReadConectionString() As String
        Return AccessHelperSql.GetConnectionString(True)
    End Function

#End Region

#Region "Create Base Connection / Transaction"
    Friend Shared Function CreateBaseConnection(Optional ByVal bFromReadSource As Boolean = False) As DbConnection
        Select Case Configuration.DataProvider
            Case ProviderType.Sql
                If bFromReadSource Then
                    Return AccessHelperSql.CreateReadConnection()
                Else
                    Return AccessHelperSql.CreateConnection()
                End If
            Case Else
                Return Nothing
        End Select
    End Function

    Friend Shared Function CreateBaseTransaction(Optional ByVal bFromReadSource As Boolean = False, Optional ByVal isolationLevel As System.Data.IsolationLevel = System.Data.IsolationLevel.ReadCommitted) As DbTransaction
        Dim Conn As DbConnection = CreateBaseConnection(bFromReadSource)
        Return Conn.BeginTransaction(isolationLevel)
    End Function

    Public Shared Function CreateConnectionForCompany(ByVal companyName As String) As roBaseConnection
        Select Case Configuration.DataProvider
            Case ProviderType.Sql
                Dim oCn As DbConnection = AccessHelperSql.CreateCompanyConnection(companyName)
                Return roBaseConnection.InitConnectionForCompany(oCn)
            Case Else
                Return Nothing
        End Select
    End Function

    Public Shared Function StartTransaction(Optional ByVal isolationLevel As IsolationLevel = IsolationLevel.ReadCommitted) As Boolean
        Dim oCn As roBaseConnection = roCacheManager.GetInstance.GetConnection(False)
        Dim bClose As Boolean = False

        If oCn IsNot Nothing Then

            If oCn.GetType = GetType(roTransaction) Then
                bClose = False
            ElseIf oCn.GetType = GetType(roConnection) Then
                bClose = True
                roCacheManager.GetInstance.RemoveCurrentConnection()
                oCn = roBaseConnection.Init(Nothing, True, False, isolationLevel, False)
                roCacheManager.GetInstance.UpdateConnection(oCn)
            Else
                bClose = False
            End If
        Else
            bClose = True
            oCn = roBaseConnection.Init(Nothing, True, False, isolationLevel, False)
            roCacheManager.GetInstance.UpdateConnection(oCn)
        End If

        Return bClose
    End Function

    Public Shared Function EndCurrentTransaction(ByVal bEnd As Boolean, ByVal bCommit As Boolean) As Boolean
        Dim oCn As roBaseConnection = roCacheManager.GetInstance.GetConnection(False)
        Dim bClosed As Boolean = False

        If bEnd Then
            If oCn IsNot Nothing Then

                If oCn.GetType = GetType(roTransaction) Then
                    oCn.CloseIfNeeded(bCommit)
                    roCacheManager.GetInstance.RemoveCurrentConnection()
                    bClosed = True

                ElseIf oCn.GetType = GetType(roConnection) Then
                    bClosed = True
                Else
                    bClosed = False
                End If
            Else
                bClosed = True
            End If
        End If

        Return bClosed
    End Function

    Public Shared Sub CleanUpCurrentConnection()
        roCacheManager.GetInstance().RemoveCurrentConnection()
    End Sub

#End Region

#Region "Create Command"

    ''' <summary>
    ''' Returns a new command object.
    ''' </summary>
    ''' <param name="commandText">The text of the query.</param>
    ''' <param name="connection">The connection to the database.</param>
    Public Shared Function CreateCommand(ByVal commandText As String, Optional ByVal connection As roBaseConnection = Nothing) As DbCommand
        Select Case Configuration.DataProvider
            Case ProviderType.Sql
                commandText = ConvertToStandardTSQL(commandText)

                Dim oCn As roBaseConnection = Nothing
                If connection IsNot Nothing Then
                    oCn = connection
                Else
                    oCn = roCacheManager.GetInstance().GetConnection()
                End If
                If oCn IsNot Nothing Then
                    Try
                        If oCn.GetType = GetType(roTransaction) Then
                            Return AccessHelperSql.CreateCommand(commandText, oCn.Connection, oCn.Transaction)
                        ElseIf oCn.GetType = GetType(roConnection) Then
                            Return AccessHelperSql.CreateCommand(commandText, oCn.Connection, Nothing)
                        Else
                            Return Nothing
                        End If
                    Catch ex As ConnectionStringException
                        Robotics.VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "AccessHelper::CreateCommand::" & ex.Message, ex)
                        Return Nothing
                    End Try
                Else
                    Return Nothing
                End If
            Case Else
                Return Nothing
        End Select

    End Function

#End Region

#Region "CreateDataAdapter"

    ''' <summary>
    ''' Returns a new data adapter object.
    ''' </summary>
    Public Shared Function CreateDataAdapter() As DbDataAdapter
        Select Case Configuration.DataProvider
            Case ProviderType.Sql
                Return AccessHelperSql.CreateDataAdapter(Nothing)
            Case Else
                Return Nothing
        End Select
    End Function

    ''' <summary>
    ''' Returns a new data adapter object.
    ''' </summary>
    ''' <param name="selectCommand">The command to use for loading data.</param>
    Public Shared Function CreateDataAdapter(ByVal selectCommand As DbCommand, Optional ByVal lBuilder As Boolean = False) As DbDataAdapter
        Select Case Configuration.DataProvider
            Case ProviderType.Sql
                Return AccessHelperSql.CreateDataAdapter(selectCommand, lBuilder)
            Case Else
                Return Nothing
        End Select
    End Function

    ''' <summary>
    ''' Adds a new parameter to specified command.
    ''' </summary>
    ''' <param name="command">The command in which to add the parameter.</param>
    Public Shared Function AddParameter(ByVal command As DbCommand) As DbParameter
        Select Case Configuration.DataProvider
            Case ProviderType.Sql
                Return AccessHelperSql.AddParameter(command, "", DbType.String, 0, ParameterDirection.Input, Nothing)
            Case Else
                Return Nothing
        End Select
    End Function

    ''' <summary>
    ''' Adds a new parameter to specified command.
    ''' </summary>
    ''' <param name="command">The command in which to add the parameter.</param>
    ''' <param name="parameterName">The name of the parameter.</param>
    Public Shared Function AddParameter(ByVal command As DbCommand, ByVal parameterName As String) As DbParameter
        Select Case Configuration.DataProvider
            Case ProviderType.Sql
                Return AccessHelperSql.AddParameter(command, parameterName, DbType.String, 0, ParameterDirection.Input, Nothing)
            Case Else
                Return Nothing
        End Select
    End Function

    ''' <summary>
    ''' Adds a new parameter to specified command.
    ''' </summary>
    ''' <param name="command">The command in which to add the parameter.</param>
    ''' <param name="parameterName">The name of the parameter.</param>
    ''' <param name="dataType">One of the DbType values.</param>
    Public Shared Function AddParameter(ByVal command As DbCommand, ByVal parameterName As String, ByVal dataType As DbType) As DbParameter
        Select Case Configuration.DataProvider
            Case ProviderType.Sql
                Return AccessHelperSql.AddParameter(command, parameterName, dataType, 0, ParameterDirection.Input, Nothing)
            Case Else
                Return Nothing
        End Select
    End Function

    ''' <summary>
    ''' Adds a new parameter to specified command.
    ''' </summary>
    ''' <param name="command">The command in which to add the parameter.</param>
    ''' <param name="parameterName">The name of the parameter.</param>
    ''' <param name="dataType">One of the DbType values.</param>
    ''' <param name="size">The length of the parameter.</param>
    Public Shared Function AddParameter(ByVal command As DbCommand, ByVal parameterName As String, ByVal dataType As DbType, ByVal size As Integer) As DbParameter
        Select Case Configuration.DataProvider
            Case ProviderType.Sql
                Return AccessHelperSql.AddParameter(command, parameterName, dataType, size, ParameterDirection.Input, Nothing)
            Case Else
                Return Nothing
        End Select
    End Function

    ''' <summary>
    ''' Adds a new parameter to specified command.
    ''' </summary>
    ''' <param name="command">The command in which to add the parameter.</param>
    ''' <param name="parameterName">The name of the parameter.</param>
    ''' <param name="dataType">One of the DbType values.</param>
    ''' <param name="size">The length of the parameter.</param>
    ''' <param name="sourceColumn">The name of the source column.</param>
    Public Shared Function AddParameter(ByVal command As DbCommand, ByVal parameterName As String, ByVal dataType As DbType, ByVal size As Integer, ByVal sourceColumn As String) As DbParameter
        Select Case Configuration.DataProvider
            Case ProviderType.Sql
                Return AccessHelperSql.AddParameter(command, parameterName, dataType, size, ParameterDirection.Input, sourceColumn)
            Case Else
                Return Nothing
        End Select
    End Function

    ''' <summary>
    ''' Adds a new parameter to specified command.
    ''' </summary>
    ''' <param name="command">The command in which to add the parameter.</param>
    ''' <param name="parameterName">The name of the parameter.</param>
    ''' <param name="dataType">One of the DbType values.</param>
    ''' <param name="size">The length of the parameter.</param>
    ''' <param name="direction">One of the ParameterDirection values.</param>
    Public Shared Function AddParameter(ByVal command As DbCommand, ByVal parameterName As String, ByVal dataType As DbType, ByVal size As Integer, ByVal direction As ParameterDirection) As DbParameter
        Select Case Configuration.DataProvider
            Case ProviderType.Sql
                Return AccessHelperSql.AddParameter(command, parameterName, dataType, size, direction, Nothing)
            Case Else
                Return Nothing
        End Select
    End Function

    ''' <summary>
    ''' Adds a new parameter to specified command.
    ''' </summary>
    ''' <param name="command">The command in which to add the parameter.</param>
    ''' <param name="parameterName">The name of the parameter.</param>
    ''' <param name="dataType">One of the DbType values.</param>
    ''' <param name="size">The length of the parameter.</param>
    ''' <param name="direction">One of the ParameterDirection values.</param>
    ''' <param name="sourceColumn">The name of the source column.</param>
    Public Shared Function AddParameter(ByVal command As DbCommand, ByVal parameterName As String, ByVal dataType As DbType, ByVal size As Integer, ByVal direction As ParameterDirection, ByVal sourceColumn As String) As DbParameter
        Select Case Configuration.DataProvider
            Case ProviderType.Sql
                Return AccessHelperSql.AddParameter(command, parameterName, dataType, size, direction, sourceColumn)
            Case Else
                Return Nothing
        End Select
    End Function

    ''' <summary>
    ''' Set Parameters on Commnad
    ''' </summary>
    ''' <param name="parameters">parameters to set</param>
    ''' <param name="cmd">command to set</param>
    Private Shared Sub SetParameters(parameters As List(Of CommandParameter), cmd As DbCommand)

        If Not parameters Is Nothing AndAlso parameters.Count > 0 Then
            For Each parameter As CommandParameter In parameters
                Select Case parameter.Type
                    Case CommandParameter.ParameterType.tInt
                        AddParameter(cmd, parameter.Name, DbType.Int32)
                        cmd.Parameters(cmd.Parameters.Count - 1).Value = parameter.Value
                    Case CommandParameter.ParameterType.tBoolean
                        AddParameter(cmd, parameter.Name, DbType.Boolean)
                        cmd.Parameters(cmd.Parameters.Count - 1).Value = parameter.Value
                    Case CommandParameter.ParameterType.tDouble
                        AddParameter(cmd, parameter.Name, DbType.Double)
                        cmd.Parameters(cmd.Parameters.Count - 1).Value = parameter.Value
                    Case CommandParameter.ParameterType.tDateTime
                        AddParameter(cmd, parameter.Name, DbType.DateTime)
                        cmd.Parameters(cmd.Parameters.Count - 1).Value = parameter.Value
                    Case CommandParameter.ParameterType.tVarBinary
                        AddParameter(cmd, parameter.Name, DbType.Binary)
                        cmd.Parameters(cmd.Parameters.Count - 1).Value = parameter.Value
                    Case Else
                        AddParameter(cmd, parameter.Name)
                        cmd.Parameters(cmd.Parameters.Count - 1).Value = parameter.Value
                End Select
            Next
        End If
    End Sub

#End Region

#Region "ExecuteSql"

    Public Shared Function ExecuteSql(ByVal strQuery As String, Optional ByVal oConnection As roBaseConnection = Nothing) As Boolean
        Return ExecuteSql(strQuery, Nothing, oConnection, True)
    End Function

    Public Shared Function ExecuteSql(ByVal strQuery As String, ByVal parameters As List(Of CommandParameter), Optional ByVal oConnection As roBaseConnection = Nothing, Optional ByVal bTimeout As Boolean = True, Optional ByVal iTimeoutSeconds As Nullable(Of Integer) = Nothing) As Boolean
        Dim oCn As roBaseConnection = Nothing
        Dim bRes As Boolean = False
        Dim watch As Stopwatch = Stopwatch.StartNew()

        If oConnection IsNot Nothing Then
            oCn = oConnection
        Else
            oCn = roCacheManager.GetInstance().GetConnection()
        End If

        If oCn IsNot Nothing Then
            Dim cmd As DbCommand = CreateCommand(strQuery, oCn)

            If parameters IsNot Nothing Then SetParameters(parameters, cmd)
            If Not bTimeout AndAlso cmd IsNot Nothing Then
                cmd.CommandTimeout = 0
            ElseIf iTimeoutSeconds.HasValue AndAlso cmd IsNot Nothing Then
                cmd.CommandTimeout = iTimeoutSeconds.Value
            End If

            cmd.ExecuteNonQuery()

            bRes = True
        Else
            bRes = False
        End If

        watch.Stop()
        roLog.GetInstance().AddSqlProcessTime(watch.Elapsed.TotalSeconds, roLog.ProcessTimeUnit.seconds)

        Return bRes
    End Function

    Public Shared Function ExecuteSqlWithoutTimeOut(ByVal strQuery As String) As Boolean
        Dim oRet As Boolean = ExecuteSql(strQuery, Nothing, Nothing, False)

        Return oRet
    End Function

    Public Shared Function ExecuteSqlWithoutTimeOut(ByVal strQuery As String, Optional ByVal oConnection As roBaseConnection = Nothing) As Boolean
        Dim oRet As Boolean = ExecuteSql(strQuery, Nothing, oConnection, False)

        Return oRet
    End Function

    Public Shared Function ExecuteSqlWithTimeOut(ByVal strQuery As String, ByVal iTimeout As Integer) As Boolean
        Dim oRet As Boolean = ExecuteSql(strQuery, Nothing, Nothing, False, iTimeout)

        Return oRet
    End Function

    Public Shared Function ExecuteSqlWithTimeOut(ByVal strQuery As String, ByVal iTimeout As Integer, Optional ByVal oConnection As roBaseConnection = Nothing) As Boolean
        Dim oRet As Boolean = ExecuteSql(strQuery, Nothing, oConnection, False, iTimeout)

        Return oRet
    End Function

    Public Shared BulkCopyIdFromQuery As Action(Of String, String, String) = Sub(destinationTableName, sqlSelect, idFieldName)
                                                                                 Dim oCn As roBaseConnection = Nothing
                                                                                 Dim watch As Stopwatch = Stopwatch.StartNew()
                                                                                 oCn = roCacheManager.GetInstance().GetConnection()
                                                                                 destinationTableName = destinationTableName.Replace("#", "")

                                                                                 Dim tempTableName As String = $"#{destinationTableName}"
                                                                                 AccessHelper.ExecuteSql($"IF OBJECT_ID('tempdb..{tempTableName}') IS NULL @CREATE# TABLE {tempTableName} (Id INT NOT NULL); @DELETE# FROM {tempTableName};")
                                                                                 AccessHelper.ExecuteSql($"@INSERT# INTO {tempTableName} @SELECT# bulktmp.{idFieldName} FROM ({sqlSelect}) bulktmp ")

                                                                                 watch.Stop()
                                                                                 roLog.GetInstance().AddSqlProcessTime(watch.Elapsed.TotalSeconds, roLog.ProcessTimeUnit.seconds)
                                                                             End Sub

    Public Shared BulkCopyIdListFromString As Action(Of String, String) = Sub(destinationTableName, employeeFilterCommaSeparated)
                                                                              Dim ids As String() = employeeFilterCommaSeparated.Split(",")
                                                                              BulkCopyIdList(destinationTableName, ids)
                                                                          End Sub

    Public Shared BulkCopyIdList As Action(Of String, String()) = Sub(destinationTableName, numbers)
                                                                      Dim oCn As roBaseConnection = Nothing
                                                                      Dim watch As Stopwatch = Stopwatch.StartNew()
                                                                      oCn = roCacheManager.GetInstance().GetConnection()
                                                                      destinationTableName = destinationTableName.Replace("#", "")

                                                                      Dim tempTableName As String = $"#{destinationTableName}"
                                                                      AccessHelper.ExecuteSql($"IF OBJECT_ID('tempdb..{tempTableName}') IS NULL @CREATE# TABLE {tempTableName} (Id INT NOT NULL); @DELETE# FROM {tempTableName};")

                                                                      Dim table As DataTable = New DataTable()
                                                                      If oCn IsNot Nothing Then
                                                                          Dim bulkCopy As New SqlBulkCopy(oCn.Connection, SqlBulkCopyOptions.Default, oCn.Transaction)
                                                                          bulkCopy.DestinationTableName = tempTableName
                                                                          table.Columns.Add("Id", GetType(Integer))
                                                                          For Each num As Integer In numbers
                                                                              table.Rows.Add(num)
                                                                          Next
                                                                          bulkCopy.WriteToServer(table)
                                                                      End If
                                                                      watch.Stop()
                                                                      roLog.GetInstance().AddSqlProcessTime(watch.Elapsed.TotalSeconds, roLog.ProcessTimeUnit.seconds)
                                                                  End Sub


    Public Shared BulkCopyIdDatatable As Action(Of String, DataTable) = Sub(destinationTableName, dt)
                                                                            Dim oCn As roBaseConnection = Nothing
                                                                            Dim watch As Stopwatch = Stopwatch.StartNew()
                                                                            oCn = roCacheManager.GetInstance().GetConnection()
                                                                            destinationTableName = destinationTableName.Replace("#", "")

                                                                            Dim tempTableName As String = $"#{destinationTableName}"
                                                                            AccessHelper.ExecuteSql($"IF OBJECT_ID('tempdb..{tempTableName}') IS NULL @CREATE# TABLE {tempTableName} (Id INT NOT NULL); @DELETE# FROM {tempTableName};")

                                                                            Dim table As DataTable = New DataTable()
                                                                            If oCn IsNot Nothing Then
                                                                                Dim bulkCopy As New SqlBulkCopy(oCn.Connection, SqlBulkCopyOptions.Default, oCn.Transaction)
                                                                                bulkCopy.DestinationTableName = tempTableName
                                                                                table.Columns.Add("Id", GetType(Integer))
                                                                                For Each oRow As DataRow In dt.Rows
                                                                                    table.Rows.Add(roTypes.Any2Integer(oRow("Id")))
                                                                                Next
                                                                                bulkCopy.WriteToServer(table)
                                                                            End If
                                                                            watch.Stop()
                                                                            roLog.GetInstance().AddSqlProcessTime(watch.Elapsed.TotalSeconds, roLog.ProcessTimeUnit.seconds)
                                                                        End Sub

    Public Shared BulkCopy As Action(Of String, DataTable) = Sub(destinationTableName, table)
                                                                 Dim oCn As roBaseConnection = Nothing
                                                                 Dim watch As Stopwatch = Stopwatch.StartNew()
                                                                 oCn = roCacheManager.GetInstance().GetConnection()

                                                                 If oCn IsNot Nothing Then
                                                                     If destinationTableName.StartsWith("#") Then CreateTempTableFromDataTable(destinationTableName, table)

                                                                     Dim bulkCopy As New SqlBulkCopy(oCn.Connection, SqlBulkCopyOptions.Default, oCn.Transaction)
                                                                     bulkCopy.DestinationTableName = destinationTableName
                                                                     bulkCopy.WriteToServer(table)
                                                                 End If
                                                                 watch.Stop()
                                                                 roLog.GetInstance().AddSqlProcessTime(watch.Elapsed.TotalSeconds, roLog.ProcessTimeUnit.seconds)
                                                             End Sub

    Public Shared EmployeeSelectorBulkCopy As Action(Of String, DataTable) = Sub(destinationTableName, table)
                                                                                 Dim oCn As roBaseConnection = Nothing
                                                                                 Dim watch As Stopwatch = Stopwatch.StartNew()
                                                                                 oCn = roCacheManager.GetInstance().GetConnection()

                                                                                 AccessHelper.ExecuteSql($"IF OBJECT_ID('tempdb..{destinationTableName}') IS NULL 
                                                                                                           @CREATE# TABLE {destinationTableName} (IDEmployee INT NOT NULL); 
                                                                                                           @DELETE# FROM {destinationTableName};")

                                                                                 If oCn IsNot Nothing Then
                                                                                     Dim bulkCopy As New SqlBulkCopy(oCn.Connection, SqlBulkCopyOptions.Default, oCn.Transaction)
                                                                                     bulkCopy.DestinationTableName = destinationTableName
                                                                                     bulkCopy.WriteToServer(table)
                                                                                 End If
                                                                                 watch.Stop()
                                                                                 roLog.GetInstance().AddSqlProcessTime(watch.Elapsed.TotalSeconds, roLog.ProcessTimeUnit.seconds)
                                                                             End Sub



    Public Shared Function CreateTempTableFromDataTable(tempTableName As String, dataTable As DataTable) As Boolean
        If Not tempTableName.StartsWith("#") Then
            tempTableName = "#" & tempTableName
        End If

        Dim createTableSql As New StringBuilder()
        createTableSql.AppendLine($"IF OBJECT_ID('tempdb..{tempTableName}') IS NOT NULL")
        createTableSql.AppendLine($"    @DROP# TABLE {tempTableName};")
        createTableSql.AppendLine($"@CREATE# TABLE {tempTableName} (")

        Dim columnDefinitions As New List(Of String)()

        For Each column As DataColumn In dataTable.Columns
            Dim sqlType As String = GetSqlDataType(column.DataType)
            Dim nullable As String = If(column.AllowDBNull, "NULL", "NOT NULL")
            columnDefinitions.Add($"    [{column.ColumnName}] {sqlType} {nullable}")
        Next

        createTableSql.AppendLine(String.Join("," & Environment.NewLine, columnDefinitions))
        createTableSql.AppendLine(");")

        Return ExecuteSql(createTableSql.ToString())
    End Function

    Private Shared Function GetSqlDataType(type As Type) As String
        Select Case type.Name
            Case "String"
                Return "NVARCHAR(MAX)"
            Case "Int32"
                Return "INT"
            Case "Int64"
                Return "BIGINT"
            Case "Boolean"
                Return "BIT"
            Case "DateTime"
                Return "DATETIME"
            Case "Double"
                Return "FLOAT"
            Case "Decimal"
                Return "DECIMAL(18,6)"
            Case "Byte[]"
                Return "VARBINARY(MAX)"
            Case "Guid"
                Return "UNIQUEIDENTIFIER"
            Case Else
                Return "NVARCHAR(MAX)"
        End Select
    End Function
#End Region

#Region "ExecuteScalar"

    Public Shared Function ExecuteScalar(ByVal strQuery As String, Optional ByVal oConnection As roBaseConnection = Nothing) As Object
        Return AccessHelper.ExecuteScalar(strQuery, Nothing, oConnection)
    End Function

    Public Shared Function ExecuteScalar(ByVal strQuery As String, ByVal parameters As List(Of CommandParameter), Optional ByVal oConnection As roBaseConnection = Nothing) As Object
        Dim oRet As Object = Nothing
        Dim oCn As roBaseConnection = Nothing
        Dim watch As Stopwatch = Stopwatch.StartNew()

        If oConnection IsNot Nothing Then
            oCn = oConnection
        Else
            oCn = roCacheManager.GetInstance().GetConnection()
        End If

        If oCn IsNot Nothing Then
            Dim cmd As DbCommand = CreateCommand(strQuery, oCn)

            If parameters IsNot Nothing Then SetParameters(parameters, cmd)
            oRet = cmd.ExecuteScalar()
        End If

        watch.Stop()
        roLog.GetInstance().AddSqlProcessTime(watch.Elapsed.TotalSeconds, roLog.ProcessTimeUnit.seconds)

        Return oRet
    End Function

#End Region

#Region "CreateDataTable"


    Public Shared Function CreateDataTable(ByVal strQuery As String, Optional ByVal oConnection As roBaseConnection = Nothing, Optional ByVal strTableName As String = "") As DataTable
        Return DataLayer.AccessHelper.CreateDataTable(strQuery, Nothing, oConnection, strTableName)
    End Function

    ''' <summary>
    ''' Return DataTable without parameters
    ''' </summary>
    ''' <param name="strQuery">query to execute</param>
    ''' <param name="strTableName">table name</param>
    ''' <param name="cn">database connection</param>
    ''' <param name="trans">transaction</param>
    ''' <returns>return datable result of sql query</returns>
    Public Shared Function CreateDataTable(ByVal strQuery As String, ByVal strTableName As String) As DataTable
        Dim tbResult As DataTable = CreateDataTable(strQuery, Nothing, Nothing, strTableName)

        Return tbResult
    End Function

    Public Shared Function CreateDataTable(ByVal strQuery As String, parameters As List(Of CommandParameter), ByVal strTableName As String) As DataTable
        Dim tbResult As DataTable = CreateDataTable(strQuery, parameters, Nothing, strTableName)

        Return tbResult
    End Function

    Public Shared Function CreateDataTable(ByVal strQuery As String, parameters As List(Of CommandParameter), Optional ByVal oConnection As roBaseConnection = Nothing, Optional ByVal strTableName As String = "", Optional ByVal bTimeout As Boolean = True, Optional ByVal iTimeoutSeconds As Nullable(Of Integer) = Nothing) As DataTable

        Dim oCn As roBaseConnection
        Dim tb As DataTable = Nothing
        Dim watch As Stopwatch = Stopwatch.StartNew()

        If oConnection IsNot Nothing Then
            oCn = oConnection
        Else
            oCn = roCacheManager.GetInstance().GetConnection()
        End If

        If oCn IsNot Nothing Then
            If strTableName <> "" Then
                tb = New DataTable(strTableName)
            Else
                tb = New DataTable
            End If

            Dim cmd As DbCommand = CreateCommand(strQuery, oCn)
            If parameters IsNot Nothing Then SetParameters(parameters, cmd)
            Dim ad As DbDataAdapter = CreateDataAdapter(cmd)

            If Not bTimeout AndAlso ad.SelectCommand IsNot Nothing Then
                ad.SelectCommand.CommandTimeout = 0
            ElseIf iTimeoutSeconds.HasValue AndAlso ad.SelectCommand IsNot Nothing Then
                ad.SelectCommand.CommandTimeout = iTimeoutSeconds.Value
            End If

#If DEBUG Then
            Dim stopwatch As New Stopwatch()
            stopwatch.Start()
#End If

            ad.Fill(tb)

#If DEBUG Then
            stopwatch.Stop()
            Dim elapsedSeconds As Double = stopwatch.Elapsed.TotalSeconds

            If roTypes.Any2Boolean(Robotics.VTBase.roConstants.GetConfigurationParameter("DebugMode")) AndAlso elapsedSeconds > 5 Then
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "AccessHelper::CreateDataTable::Query took more than 10 seconds::" & elapsedSeconds & "s::" & strQuery)
            End If
#End If
        End If

        watch.Stop()
        roLog.GetInstance().AddSqlProcessTime(watch.Elapsed.TotalSeconds, roLog.ProcessTimeUnit.seconds)

        Return tb
    End Function

    Public Shared Function CreateDataTableWithoutTimeouts(ByVal strQuery As String, Optional ByVal cn As roBaseConnection = Nothing, Optional ByVal strTableName As String = "") As DataTable
        Dim tbResult As DataTable = CreateDataTable(strQuery, Nothing, Nothing, strTableName, False)

        Return tbResult
    End Function

#End Region

#Region "CreateDataSet"

    Public Shared Function CreateDataSet(ByVal strQuery As String, Optional ByVal oConnection As roBaseConnection = Nothing, Optional ByVal strDataSetName As String = "") As DataSet
        Dim oCn As roBaseConnection
        Dim ds As DataSet = Nothing

        Dim watch As Stopwatch = Stopwatch.StartNew()

        If oConnection IsNot Nothing Then
            oCn = oConnection
        Else
            oCn = roCacheManager.GetInstance().GetConnection()
        End If

        If oCn IsNot Nothing Then
            If strDataSetName <> "" Then
                ds = New DataSet(strDataSetName)
            Else
                ds = New DataSet()
            End If

            Dim cmd As DbCommand = CreateCommand(strQuery, oConnection)
            Dim ad As DbDataAdapter = CreateDataAdapter(cmd)

            ad.Fill(ds)
        End If

        watch.Stop()
        roLog.GetInstance().AddSqlProcessTime(watch.Elapsed.TotalSeconds, roLog.ProcessTimeUnit.seconds)

        Return ds
    End Function

    Public Shared Function CreateDataSet(ByVal strQuery As String, ByVal strDataSetName As String) As DataSet
        Return CreateDataSet(strQuery, Nothing, strDataSetName)
    End Function

#End Region

#Region "DataReader"

    Public Shared Function CreateDataReader(ByVal strQuery As String) As DbDataReader
        Return CreateDataReader(strQuery, Nothing, "")
    End Function

    Public Shared Function CreateDataReader(ByVal strQuery As String, Optional ByVal oConnection As roBaseConnection = Nothing, Optional ByVal strDataSetName As String = "") As DbDataReader
        Dim oRet As Object = Nothing
        Dim oCn As roBaseConnection
        Dim watch As Stopwatch = Stopwatch.StartNew()

        If oConnection IsNot Nothing Then
            oCn = oConnection
        Else
            oCn = roCacheManager.GetInstance().GetConnection()
        End If

        If oCn IsNot Nothing Then
            Dim cmd As DbCommand = CreateCommand(strQuery, oCn)
            oRet = cmd.ExecuteReader
        End If

        watch.Stop()
        roLog.GetInstance().AddSqlProcessTime(watch.Elapsed.TotalSeconds, roLog.ProcessTimeUnit.seconds)

        Return oRet
    End Function

#End Region

#Region "SQL injection"
    Private Shared Function ConvertToStandardTSQL(strSQL As String) As String

        Dim oRet As String = strSQL

        Dim bIsSafe As Boolean = False

        Dim aVerbsToCheck As String() = {"DELETE ", "INSERT ", "SELECT ", "UPDATE ", "EXEC ", "DECLARE ", "DROP ", "CREATE ", "ALTER ", "WAITFOR DELAY "}
        Dim aDelimiters As Char() = {"@", "#"}
        Dim aForbbidenPreceedingChars As ArrayList = New ArrayList({" ", ";", "("})

        ' Casos en los que hacer la vista gorda, porque son seguros ...
        If oRet.Contains("sysroPunchesTransactions") Then
            bIsSafe = True
        End If

        If Not bIsSafe Then
            ' Detección de comentarios. Es peligroso, porque hay inserts y updates que introducen cadenas de texto como -->, o xml como la auditoría que incluyen cadenas como --&
            If oRet.Contains("--") AndAlso Not oRet.Contains("-->") AndAlso Not oRet.Contains("--&") AndAlso Not oRet.Contains("'--'") Then
                Throw New DataEngineException("DataEngineError!::CommentDetected::" & strSQL)
            End If
        End If

        For Each sVerb As String In aVerbsToCheck

            If oRet.ToUpper().Contains(sVerb.Trim) Then
                Dim origin As String = oRet.ToUpper().Replace(aDelimiters(0) & sVerb.Trim & aDelimiters(1), "")
                Dim counterOrigin As Integer = origin.Length

                Dim treat As String = oRet.ToUpper().Replace(aDelimiters(0) & sVerb.Trim & aDelimiters(1), sVerb.Trim)
                Dim treated As String = treat.Replace(sVerb, " ")
                Dim counterTreat As Integer = treated.Length
                Dim falseAlarm As Boolean = True
                Dim sPart As String = String.Empty

                If Not bIsSafe AndAlso counterOrigin <> counterTreat Then
                    ' Antes debería mirar si la letra anterior al verbo es otra letra, lo que descarta la inyección
                    If Not oRet.ToUpper.StartsWith(sVerb.ToUpper) Then
                        Try
                            For i As Integer = 1 To oRet.ToUpper.Split(New String() {sVerb.ToUpper}, StringSplitOptions.None).Length - 1
                                sPart = oRet.ToUpper.Split(New String() {sVerb.ToUpper}, StringSplitOptions.None)(i - 1)
                                If sPart <> "" Then
                                    falseAlarm = Not aForbbidenPreceedingChars.Contains(sPart.Substring(sPart.Length - 1))
                                End If
                                If Not falseAlarm Then Exit For
                            Next
                        Catch ex As Exception
                        End Try
                    Else
                        falseAlarm = False
                    End If

                    If Not falseAlarm Then
                        Throw New DataEngineException("DataEngineError!::CheckVerb::" & sVerb & "::" & strSQL)
                    Else
                        oRet = System.Text.RegularExpressions.Regex.Replace(oRet, aDelimiters(0) & sVerb.Trim & aDelimiters(1), sVerb.Trim, RegexOptions.IgnoreCase)
                    End If
                Else
                    oRet = Regex.Replace(oRet, aDelimiters(0) & sVerb.Trim & aDelimiters(1), sVerb.Trim, RegexOptions.IgnoreCase)
                End If
            End If
        Next

        Return oRet
    End Function

    Public Shared Function GetSQLCommandText(dacmd As System.Data.SqlClient.SqlCommand) As String
        Dim sqlText As New StringBuilder

        If dacmd IsNot Nothing Then
            sqlText = New StringBuilder(dacmd.CommandText.ToString())
            ' Replace the parameters with values.
            For i As Integer = dacmd.Parameters.Count - 1 To 0 Step -1
                Dim parm As System.Data.SqlClient.SqlParameter = dacmd.Parameters(i)
                If IsDBNull(parm.Value) Then
                    sqlText.Replace(parm.ParameterName, "NULL")
                Else
                    If parm.SqlDbType = SqlDbType.NVarChar OrElse parm.SqlDbType = SqlDbType.NText Then
                        ' Pongo los quotes
                        sqlText.Replace(parm.ParameterName, "'" + parm.Value.ToString() + "'")
                    ElseIf parm.SqlDbType = SqlDbType.SmallDateTime Then
                        'sqlText.Replace(parm.ParameterName, "convert(smalldatetime,'" + parm.Value.ToString() + "',120)")
                        sqlText.Replace(parm.ParameterName, Robotics.VTBase.roTypes.Any2Time(parm.Value).SQLSmallDateTime)
                    ElseIf parm.SqlDbType = SqlDbType.DateTime OrElse parm.SqlDbType = SqlDbType.DateTime2 Then
                        'sqlText.Replace(parm.ParameterName, "convert(datetime,'" + parm.Value.ToString() + "',120)")
                        sqlText.Replace(parm.ParameterName, Robotics.VTBase.roTypes.Any2Time(parm.Value).SQLDateTime)
                    ElseIf parm.SqlDbType = SqlDbType.Bit Then
                        If parm.Value.ToString() = "True" Then
                            sqlText.Replace(parm.ParameterName, "1")
                        Else
                            sqlText.Replace(parm.ParameterName, "0")
                        End If
                    Else
                        sqlText.Replace(parm.ParameterName, parm.Value.ToString())
                    End If
                End If
            Next
        End If

        Return sqlText.ToString()
    End Function

    Public Shared Function GetDatabaseCompatibilityLevel() As String
        Dim sqlText As String = "@SELECT# compatibility_level FROM sys.databases WHERE name = DB_NAME()"
        Dim iCL As Integer = 120
        Try
            iCL = roTypes.Any2Integer(ExecuteScalar(sqlText))
        Catch ex As Exception
            iCL = 120
        End Try
        Return iCL
    End Function

#End Region

#Region "Diagnostics queries"
    Public Shared Function CreateDiagnosticsCommand(ByVal commandText As String) As DbCommand
        Dim connection As roBaseConnection = roCacheManager.GetInstance().GetConnection()

        Select Case Configuration.DataProvider
            Case ProviderType.Sql
                Try
                    If connection.GetType = GetType(roTransaction) Then
                        Return AccessHelperSql.CreateCommand(commandText, connection.Connection, connection.Transaction)
                    ElseIf connection.GetType = GetType(roConnection) Then
                        Return AccessHelperSql.CreateCommand(commandText, connection.Connection, Nothing)
                    Else
                        Return Nothing
                    End If
                Catch ex As ConnectionStringException
                    Robotics.VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "AccessHelper::CreateDiagnosticsCommand::" & ex.Message, ex)
                    Return Nothing
                End Try

            Case ProviderType.Oracle
                Return Nothing
            Case ProviderType.VistaDB
                Return Nothing
            Case Else
                Return Nothing
        End Select
    End Function

#End Region

#Region "Upgrade MT DB"

    Public Shared Function CreateROCommand(ByVal commandText As String) As DbCommand
        Dim connection As roBaseConnection = roCacheManager.GetInstance().GetConnection()

        Select Case Configuration.DataProvider
            Case ProviderType.Sql
                Try
                    If connection.GetType = GetType(roTransaction) Then
                        Return AccessHelperSql.CreateCommand(commandText, connection.Connection, connection.Transaction)
                    ElseIf connection.GetType = GetType(roConnection) Then
                        Return AccessHelperSql.CreateCommand(commandText, connection.Connection, Nothing)
                    Else
                        Return Nothing
                    End If
                Catch ex As ConnectionStringException
                    Robotics.VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "AccessHelper::CreateROCommand::" & ex.Message, ex)
                    Return Nothing
                End Try
            Case Else
                Return Nothing
        End Select
    End Function

    Public Shared Function UpgradeClientDB() As Boolean
        Try

            Dim iCurrentDBVersion As Integer = VTBase.roTypes.Any2Integer(ExecuteScalar("@SELECT# Data FROM sysroParameters WHERE ID='DBVERSION'"))
            Dim bUpdating As Boolean = VTBase.roTypes.Any2Boolean(ExecuteScalar("@SELECT# Value FROM sysroLiveAdvancedParameters WHERE ParameterName='VTLive.DB.Updating'"))
            Dim xStarted As DateTime = DateTime.MinValue

            Dim sDate As String = VTBase.roTypes.Any2String(ExecuteScalar("@SELECT# Value FROM sysroLiveAdvancedParameters WHERE ParameterName='VTLive.DB.UpdateStart'"))
            If sDate <> String.Empty Then
                xStarted = DateTime.ParseExact(sDate, "yyyy-MM-dd HH:mm:ss", Nothing)
            End If

            If Not bUpdating OrElse (bUpdating AndAlso DateTime.Now.Subtract(xStarted).TotalMinutes > 45) Then
                Dim updatingSQL As String = "IF NOT EXISTS (@SELECT# 1 FROM sysroLiveAdvancedParameters WHERE ParameterName='VTLive.DB.Updating') " &
                                                "@INSERT# INTO sysroLiveAdvancedParameters (ParameterName, Value) VALUES('VTLive.DB.Updating','true') "
                ExecuteSql(updatingSQL)

                updatingSQL = "@UPDATE# sysroLiveAdvancedParameters Set Value = 'true' where ParameterName = 'VTLive.DB.Updating'"
                ExecuteSql(updatingSQL)

                updatingSQL = "IF NOT EXISTS (@SELECT# 1 FROM sysroLiveAdvancedParameters WHERE ParameterName='VTLive.DB.UpdateStart') " &
                                                "@INSERT# INTO sysroLiveAdvancedParameters (ParameterName, Value) VALUES('VTLive.DB.UpdateStart','" & DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") & "') "
                ExecuteSql(updatingSQL)

                updatingSQL = "@UPDATE# sysroLiveAdvancedParameters Set Value = '" & DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") & "' where ParameterName = 'VTLive.DB.UpdateStart'"
                ExecuteSql(updatingSQL)

                Dim bContinue As Boolean = True
                Dim bUpdated As Boolean = False
                Try
                    If iCurrentDBVersion > 0 Then
                        Dim oAvailableKeys As Dictionary(Of String, String) = Azure.RoAzureSupport.GetUpgradeDBMasterDic()
                        While (oAvailableKeys.ContainsKey("FromVersion" & iCurrentDBVersion) AndAlso bContinue)
                            VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roInfo, "RunDatabaseUpdaterTask::Applying database update for version " & iCurrentDBVersion)

                            bUpdated = True
                            bContinue = True

                            Dim bCurrentUpdate As String() = Azure.RoAzureSupport.GetUpgradeFile(oAvailableKeys("FromVersion" & iCurrentDBVersion))
                            Dim bUpdateWithErrors As Boolean = False
                            Dim sqlQuery As New StringBuilder
                            Dim iCurrentSentenceLine = 1
                            For Each oSQLline As String In bCurrentUpdate
                                If oSQLline.Trim.ToUpper = "GO" Then
                                    Try
                                        Dim cmd As DbCommand = CreateROCommand(sqlQuery.ToString())
                                        cmd.CommandTimeout = 0

                                        cmd.ExecuteNonQuery()
                                    Catch ex As System.Data.Common.DbException
                                        bUpdateWithErrors = True
                                        VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "RunDatabaseUpdaterTask::Error applying sql sentence line:: " & iCurrentSentenceLine, ex)
                                    End Try
                                    sqlQuery.Clear()
                                    iCurrentSentenceLine = iCurrentSentenceLine + 1
                                Else
                                    sqlQuery.Append((oSQLline & vbNewLine))
                                End If
                            Next

                            If Not bUpdateWithErrors Then
                                VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "RunDatabaseUpdaterTask::Patch" & iCurrentDBVersion & " completed with no errors")
                            End If
                            Dim iNextVersion As Integer = VTBase.roTypes.Any2Integer(ExecuteScalar("@SELECT# Data FROM sysroParameters WHERE ID='DBVERSION'"))

                            If iCurrentDBVersion <> iNextVersion Then
                                iCurrentDBVersion = iNextVersion
                            Else
                                bContinue = False
                            End If

                        End While

                        If bUpdated Then
                            VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roInfo, "RunDatabaseUpdaterTask::DB update finished at patch::" & iCurrentDBVersion)

                            Try
                                Dim specialPatch As String() = Azure.RoAzureSupport.GetUpgradeFile("RunOnceByVersion.sql")
                                Dim sqlQuery As New StringBuilder
                                Dim iCurrentSentenceLine = 1
                                For Each oSQLline As String In specialPatch
                                    If oSQLline.Trim.ToUpper = "GO" Then
                                        Try
                                            Dim cmd As DbCommand = CreateROCommand(sqlQuery.ToString())
                                            cmd.CommandTimeout = 0
                                            cmd.ExecuteNonQuery()
                                        Catch ex As System.Data.Common.DbException
                                            VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "RunDatabaseUpdaterTask::Error applying RunOnceByVersion patch on line:: " & iCurrentSentenceLine, ex)
                                        End Try
                                        sqlQuery.Clear()
                                        iCurrentSentenceLine += 1
                                    Else
                                        sqlQuery.Append((oSQLline & vbNewLine))
                                    End If
                                Next

                                VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roInfo, "RunDatabaseUpdaterTask:: 'RunOnceByVersion.sql' completed")
                            Catch ex As Exception
                                VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "RunDatabaseUpdaterTask::Failed to execute RunOnceByVersion patch 'RunOnceByVersion.sql'", ex)
                            End Try
                        End If

                    End If

                    iCurrentDBVersion = VTBase.roTypes.Any2Integer(ExecuteScalar("@SELECT# Data FROM sysroParameters WHERE ID='DXVERSION'"))
                    bContinue = True
                    bUpdated = False
                    If iCurrentDBVersion >= 0 Then
                        Dim oAvailableKeys As Dictionary(Of String, String) = Azure.RoAzureSupport.GetUpgradeDBReportsDic()

                        While (oAvailableKeys.ContainsKey("FromVersion" & iCurrentDBVersion) AndAlso bContinue)
                            VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "RunDatabaseUpdaterTask::Applying report for version " & iCurrentDBVersion)
                            bContinue = True
                            bUpdated = True

                            Dim bCurrentUpdate As String() = Azure.RoAzureSupport.GetReportFile(oAvailableKeys("FromVersion" & iCurrentDBVersion))
                            Dim bUpdateWithErrors As Boolean = False
                            Dim sqlQuery As New StringBuilder
                            Dim iCurrentSentenceLine = 1
                            For Each oSQLline As String In bCurrentUpdate
                                If oSQLline.Trim.ToUpper = "GO" Then
                                    Try
                                        Dim cmd As DbCommand = CreateROCommand(sqlQuery.ToString())
                                        cmd.CommandTimeout = 0

                                        cmd.ExecuteNonQuery()
                                    Catch ex As System.Data.Common.DbException
                                        bUpdateWithErrors = True
                                        VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "RunDatabaseUpdaterTask::Error applying report sql sentence line:: " & iCurrentSentenceLine, ex)
                                    End Try
                                    sqlQuery.Clear()
                                    iCurrentSentenceLine = iCurrentSentenceLine + 1
                                Else
                                    sqlQuery.Append((oSQLline & vbNewLine))
                                End If
                            Next

                            If Not bUpdateWithErrors Then
                                VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "RunDatabaseUpdaterTask::Report" & iCurrentDBVersion & " completed with no errors")
                            End If
                            Dim iNextVersion As Integer = VTBase.roTypes.Any2Integer(ExecuteScalar("@SELECT# Data FROM sysroParameters WHERE ID='DXVERSION'"))

                            If iCurrentDBVersion <> iNextVersion Then
                                iCurrentDBVersion = iNextVersion
                            Else
                                bContinue = False
                            End If

                        End While

                        If bUpdated Then
                            VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "RunDatabaseUpdaterTask::Report update finished at patch::" & iCurrentDBVersion)
                        End If

                    End If

                    'Si el cliente es un Upgrade desde NewONE, actualizo BBD
                    Try
                        Dim mLicenseContent As String = roTypes.Any2String(Threading.Thread.GetDomain.GetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString & "_license"))
                        If Not (mLicenseContent.Contains("Edition") AndAlso mLicenseContent.Contains("Starter")) AndAlso (roTypes.Any2Integer(ExecuteScalar("@SELECT# COUNT(*) FROM sysroGUI WHERE Edition='Starter'")) > 0) Then
                            VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roInfo, "RunDatabaseUpdaterTask::Upgrading NewONE database to Standard")

                            bUpdated = True
                            bContinue = True

                            Dim bCurrentUpdate As String() = Azure.RoAzureSupport.GetUpgradeFile("UpgradeFromNewONE.sql")
                            Dim bUpdateWithErrors As Boolean = False
                            Dim sqlQuery As New StringBuilder
                            Dim iCurrentSentenceLine = 1
                            For Each oSQLline As String In bCurrentUpdate
                                If oSQLline.Trim.ToUpper = "GO" Then
                                    Try
                                        Dim cmd As DbCommand = CreateROCommand(sqlQuery.ToString())
                                        cmd.CommandTimeout = 0

                                        cmd.ExecuteNonQuery()
                                    Catch ex As System.Data.Common.DbException
                                        bUpdateWithErrors = True
                                        VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "RunDatabaseUpdaterTask::Error applying sql sentence line:: " & iCurrentSentenceLine, ex)
                                    End Try
                                    sqlQuery.Clear()
                                    iCurrentSentenceLine = iCurrentSentenceLine + 1
                                Else
                                    sqlQuery.Append((oSQLline & vbNewLine))
                                End If
                            Next

                            If Not bUpdateWithErrors Then
                                VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "RunDatabaseUpdaterTask::Upgrade patch from NewONE customer completed with no errors")
                            End If
                        End If
                    Catch ex As Exception
                        VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "RunDatabaseUpdaterTask::Error upgrading NewONE database to Standard", ex)
                    End Try

                Catch ex As Exception
                    VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "RunDatabaseUpdaterTask::Error runing container scripts", ex)
                End Try

                updatingSQL = "@UPDATE# sysroLiveAdvancedParameters Set Value = 'false' where ParameterName = 'VTLive.DB.Updating'"
                ExecuteSql(updatingSQL)
            End If
        Catch ex As Exception
            'do nothing
        End Try

    End Function

#End Region

#Region "Shared connection Information initialize"
    Public Shared Sub InitializeSharedInstanceData(ByVal appType As roAppType, ByVal eLogType As roLiveQueueTypes)
        Dim watch As Stopwatch = Stopwatch.StartNew()
        Dim applicationName As String = appType.ToString

        If applicationName <> String.Empty Then
            If VTBase.roConstants.IsMultitenantServiceEnabled Then
                Threading.Thread.GetDomain().SetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_AppName", applicationName)
            Else
                roConstants.SetGlobalEnvironmentParameter(GlobalAsaxParameter.AppName, applicationName)
            End If
        End If

        While Not Robotics.DataLayer.AccessHelper.InitPrivateData(applicationName, eLogType)
            System.Threading.Thread.Sleep(5000)
        End While

        watch.Stop()
        roLog.GetInstance().logSystemMessage(roLog.EventType.roInfo, $"{applicationName}::Instance started",, True, roLog.AddProcessTime(watch.Elapsed.TotalSeconds, roLog.ProcessTimeUnit.seconds))
    End Sub

    Private Shared Function InitPrivateData(ByVal applicationName As String, ByVal eLogType As roLiveQueueTypes) As Boolean

        If VTBase.roConstants.IsMultitenantServiceEnabled Then
            Threading.Thread.GetDomain().SetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_DefaultLogLevel", 0)
        Else
            roConstants.SetGlobalEnvironmentParameter(GlobalAsaxParameter.LogLevel, 0)
        End If

        Dim serviceConfiguration As roServiceConfiguration = New roServiceConfigurationRepository(eLogType).GetServiceConfiguration()
        If serviceConfiguration Is Nothing Then
            roLog.GetInstance().logSystemMessage(roLog.EventType.roError, $"InitSharedInformation::{applicationName}::Could not get cosmos default log level")
            Return False
        End If

        Robotics.Azure.RoAzureSupport.SetDefaultLogLevel(serviceConfiguration.loglevel, serviceConfiguration.tracelevel, applicationName)

        Dim oConfigValue As roAzureConfig = New roConfigRepository().GetConfigParameter(roConfigParameter.keyvalut)
        If oConfigValue Is Nothing OrElse String.IsNullOrEmpty(oConfigValue.value) Then
            roLog.GetInstance().logSystemMessage(roLog.EventType.roError, $"InitSharedInformation::{applicationName}::Could not get cosmos keyvault configuration")
            Return False
        End If

        Dim tmpUsername As String = Azure.RoAzureSupport.GetVisualtimeDBUsername()
        Dim tmpPassword As String = Azure.RoAzureSupport.GetVisualtimeDBPassword()

        'Cargamos el listado de idiomas disponibles en el sistem
        roCacheManager.GetInstance().GetLocales()

        If String.IsNullOrEmpty(tmpUsername) OrElse String.IsNullOrEmpty(tmpPassword) Then
            roLog.GetInstance().logSystemMessage(roLog.EventType.roError, $"InitSharedInformation::{applicationName}::Could not get keyvault db initialization info")
            Return False
        Else
            Return True
        End If


    End Function

    Public Shared Function SetThreadCompanyInformation(ByVal sourceThreadData As roThreadData, ByVal o11yDic As Dictionary(Of String, String), ByVal traceDic As Dictionary(Of String, String), ByVal oConf As roCompanyConfiguration) As Boolean
        Dim bInitialized As Boolean = True


        Threading.Thread.CurrentThread.CurrentUICulture = New System.Globalization.CultureInfo("es-ES")
        Threading.Thread.CurrentThread.CurrentCulture = New System.Globalization.CultureInfo("es-ES")

        roTelemetryInfo.GetInstance().UpdateO11yInfo(o11yDic)
        roTelemetryInfo.GetInstance().UpdateTraceInfo(traceDic)
        roConstants.RestoreThreadData(sourceThreadData)

        Threading.Thread.GetDomain().SetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_ExcludeFromTrace", True)

        Threading.Thread.GetDomain().SetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_DBConnectionString", oConf.dbconnectionstring)

        If oConf.readdbconnectionstring IsNot Nothing AndAlso oConf.readdbconnectionstring <> String.Empty Then
            Threading.Thread.GetDomain().SetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_ReadDBConnectionString", oConf.readdbconnectionstring)
        Else
            Threading.Thread.GetDomain().SetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_ReadDBConnectionString", oConf.dbconnectionstring)
        End If

        Threading.Thread.GetDomain().SetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_company", oConf.Id)
        Threading.Thread.GetDomain().SetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_license", oConf.license)


        Try
            Dim strSQL As String = "@SELECT# ID FROM sysroPassports WHERE Description = '@@ROBOTICS@@System'"
            Dim idPassportObject As Object = AccessHelper.ExecuteScalar(strSQL)
            If idPassportObject IsNot Nothing Then
                Dim idPassport = roTypes.Any2Integer(idPassportObject)
                Thread.GetDomain().SetData(Thread.CurrentThread.ManagedThreadId.ToString() & "_" & GlobalAsaxParameter.SystemPassportID.ToString(), idPassport)
            Else
                bInitialized = False
                roLog.GetInstance().logMessage(roLog.EventType.roError, $"SetThreadInformation::Could not initialize database connection")
            End If

        Catch ex As Exception
            bInitialized = False
            roLog.GetInstance().logMessage(roLog.EventType.roError, "SetThreadInformation::Error::", ex)
        End Try

        If bInitialized AndAlso oConf.Id <> String.Empty Then
            Dim companyLogLevel As String = DataLayer.roCacheManager.GetInstance().GetAdvParametersCache(RoAzureSupport.GetCompanyName(), $"Application.LogLevel").Trim
            Dim companyTraceLevel As String = DataLayer.roCacheManager.GetInstance().GetAdvParametersCache(RoAzureSupport.GetCompanyName(), $"Application.TraceLevel").Trim

            roCacheManager.GetInstance().NeedToRefreshCompanyCache(RoAzureSupport.GetCompanyName())
            roConstants.SetDefaultCompanyTraceAndLogLevel(companyLogLevel, companyTraceLevel)
        End If

        Return bInitialized
    End Function


    Public Shared Function SetThreadInformation(ByVal strAppName As String, ByVal oConf As roCompanyConfiguration, ByVal iLogLevel As Integer, ByVal iTraceLevel As Integer, ByVal idTask As Integer, Optional strPoolname As String = "") As Boolean
        Dim bInitialized As Boolean = True
        Threading.Thread.GetDomain().SetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_RequestGUID", Guid.NewGuid.ToString())

        roTrace.GetInstance().AddTraceInfo(idTask.ToString(), strPoolname.ToUpper, oConf.Id)

        Threading.Thread.CurrentThread.CurrentUICulture = New System.Globalization.CultureInfo("es-ES")
        Threading.Thread.CurrentThread.CurrentCulture = New System.Globalization.CultureInfo("es-ES")

        Threading.Thread.GetDomain().SetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_AppName", strAppName)
        Threading.Thread.GetDomain().SetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_DefaultLogLevel", iLogLevel)
        Threading.Thread.GetDomain().SetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_DefaultTraceLevel", iTraceLevel)

        If strPoolname.Length = 0 Then
            strPoolname = strAppName
        Else
            strPoolname = strAppName & "_" & strPoolname
        End If
        Threading.Thread.GetDomain().SetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_PoolName", strPoolname)
        Threading.Thread.GetDomain().SetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_DBConnectionString", oConf.dbconnectionstring)

        If oConf.readdbconnectionstring IsNot Nothing AndAlso oConf.readdbconnectionstring <> String.Empty Then
            Threading.Thread.GetDomain().SetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_ReadDBConnectionString", oConf.readdbconnectionstring)
        Else
            Threading.Thread.GetDomain().SetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_ReadDBConnectionString", oConf.dbconnectionstring)
        End If

        Threading.Thread.GetDomain().SetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_company", oConf.Id)
        Threading.Thread.GetDomain().SetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_license", oConf.license)

        Try
            Dim strSQL As String = "@SELECT# ID FROM sysroPassports WHERE Description = '@@ROBOTICS@@System'"
            Dim idPassportObject As Object = AccessHelper.ExecuteScalar(strSQL)
            If idPassportObject IsNot Nothing Then
                Dim idPassport = roTypes.Any2Integer(idPassportObject)
                Thread.GetDomain().SetData(Thread.CurrentThread.ManagedThreadId.ToString() & "_" & GlobalAsaxParameter.SystemPassportID.ToString(), idPassport)
            Else
                bInitialized = False
                roLog.GetInstance().logMessage(roLog.EventType.roError, $"SetThreadInformation::Could not initialize database connection")
            End If

        Catch ex As Exception
            bInitialized = False
            roLog.GetInstance().logMessage(roLog.EventType.roError, "SetThreadInformation::Error::", ex)
        End Try

        If bInitialized AndAlso oConf.Id <> String.Empty Then
            Dim companyLogLevel As String = DataLayer.roCacheManager.GetInstance().GetAdvParametersCache(RoAzureSupport.GetCompanyName(), $"Application.LogLevel").Trim
            Dim companyTraceLevel As String = DataLayer.roCacheManager.GetInstance().GetAdvParametersCache(RoAzureSupport.GetCompanyName(), $"Application.TraceLevel").Trim

            roCacheManager.GetInstance().NeedToRefreshCompanyCache(RoAzureSupport.GetCompanyName())
            roConstants.SetDefaultCompanyTraceAndLogLevel(companyLogLevel, companyTraceLevel)
        End If

        Return bInitialized
    End Function

    Public Shared Function ClearThreadInformation() As Boolean
        Threading.Thread.GetDomain().SetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_PoolName", String.Empty)
        Threading.Thread.GetDomain().SetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_DBConnectionString", String.Empty)
        Threading.Thread.GetDomain().SetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_ReadDBConnectionString", String.Empty)
        Threading.Thread.GetDomain().SetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_company", String.Empty)
        Threading.Thread.GetDomain().SetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_license", String.Empty)

        roCacheManager.GetInstance().RemoveCurrentConnection()
        Threading.Thread.GetDomain().SetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_RequestGUID", String.Empty)

        Threading.Thread.GetDomain().SetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_" & GlobalAsaxParameter.SystemPassportID.ToString(), Nothing)
        Return True
    End Function

    Public Shared Function ClearThreadCompanyInformation() As Boolean
        Threading.Thread.GetDomain().SetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_PoolName", String.Empty)
        Threading.Thread.GetDomain().SetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_DBConnectionString", String.Empty)
        Threading.Thread.GetDomain().SetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_ReadDBConnectionString", String.Empty)
        Threading.Thread.GetDomain().SetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_company", String.Empty)
        Threading.Thread.GetDomain().SetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_license", String.Empty)

        roCacheManager.GetInstance().RemoveCurrentConnection()
        Return True
    End Function

#End Region

End Class