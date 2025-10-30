Imports System.Data.Common
Imports System.Data.SqlClient
Imports System.Text.RegularExpressions
Imports Moq
Imports Robotics.DataLayer

Public Class DatalayerHelper

    Public Enum SqlExecuteString
        None
        DeletePunchesCaptures
        AddManyMore
        DeleteTmpHolidaysControlBycontract
        InsertNotification
        GeneralExecute
        UpdateNotification
        ChannelCreated
        ChannelEmployeesInserted
        ChannelGroupsInserted
        ChannelSupervisorsInserted
        ChannelUpdated
        ChannelDeleted
        ChannelConversationsDeleted
        ChannelConversationMessagesDeleted
        BroadcasterAddTask
        BroadcasterDeleteTask
        QueryUsersWithPermissions
    End Enum

    Public Enum CreateDataTableString
        None
        SelectUnreliablePunchNotifications
        GeneralCreateDatatable
        SelectDeletedHolidays
        LoadPassport
        LoadPassportByEmployee
        LoadPassportByUser
    End Enum

    Public Property ExecuteSqlWasCalled As SqlExecuteString = SqlExecuteString.None
    Public Property ExcutedCreateDataTable As CreateDataTableString = CreateDataTableString.None

    Private Shared ExecuteSqlCallCount As New Dictionary(Of String, Integer)

    Public Property ExecuteSqlHistory As New List(Of ExecuteSqlItem)

    Public Sub New()
    End Sub

    Public Sub ExecuteScalarStub(ByVal sqlCommand As String, ByVal returnValue As Object)
        Robotics.DataLayer.Fakes.ShimAccessHelper.ExecuteScalarStringroBaseConnection =
           Function(ByVal strQuery As String, ByVal _Connection As Robotics.DataLayer.roBaseConnection)
               If strQuery.ToLower().Trim().Contains(sqlCommand.ToLower().Trim()) Then
                   Return returnValue
               End If
               Return Nothing
           End Function
    End Sub

    Public Sub ExecuteScalarStub(ByVal sqlCommand As Dictionary(Of String, Object))
        Robotics.DataLayer.Fakes.ShimAccessHelper.ExecuteScalarStringroBaseConnection =
           Function(ByVal strQuery As String, ByVal _Connection As Robotics.DataLayer.roBaseConnection)
               Dim valuePair As KeyValuePair(Of String, Object) = sqlCommand.ToList().FirstOrDefault(Function(x) strQuery.ToLower().Trim().Contains(x.Key.ToLower().Trim()))
               If valuePair.Key IsNot Nothing Then
                   Return valuePair.Value
               End If
               Return Nothing
           End Function
    End Sub

    Public Sub ExecuteScalarWithParametersStub(ByVal sqlCommand As Dictionary(Of String, Object))
        Robotics.DataLayer.Fakes.ShimAccessHelper.ExecuteScalarStringListOfCommandParameterroBaseConnection =
            Function(ByVal strQuery As String, ByVal parameters As List(Of CommandParameter), ByVal _Connection As Robotics.DataLayer.roBaseConnection)
                Dim valuePair As KeyValuePair(Of String, Object) = sqlCommand.ToList().FirstOrDefault(Function(x) strQuery.ToLower().Trim().Contains(x.Key.ToLower().Trim()))
                If valuePair.Key IsNot Nothing Then
                    Return valuePair.Value
                End If
                Return Nothing
            End Function
    End Sub

    Public Sub CreateCommandStub()
        Robotics.DataLayer.Fakes.ShimAccessHelper.CreateCommandStringroBaseConnection =
           Function(ByVal commandText As String, ByVal connection As roBaseConnection)


               Dim mockDbCommand As DbCommand = New SqlCommand("SELECT * FROM DORAEMON")

               Return mockDbCommand
           End Function
    End Sub

    Public Sub DbCommandMock()
        Robotics.DataLayer.Fakes.ShimAccessHelper.CreateCommandStringroBaseConnection =
                    Function()
                        Dim mockDbCommand As DbCommand = New SqlCommand("SELECT Something FROM SomeWhere")
                        Return mockDbCommand
                    End Function
    End Sub

    Public Sub DbCommandMock4Save()
        Robotics.DataLayer.Fakes.ShimAccessHelper.CreateCommandStringroBaseConnection =
                    Function(command, connection)
                        Dim mockDbCommand As DbCommand = New SqlCommand(command)
                        Return mockDbCommand
                    End Function
    End Sub

    Public Sub ExecuteReaderStub()
        System.Data.Common.Fakes.ShimDbCommand.AllInstances.ExecuteReader =
                    Function()
                        ' Crear un mock de DbDataReader
                        Dim mockDataReader As New Mock(Of DbDataReader)()

                        ' Configurar el mock para que devuelva datos específicos
                        mockDataReader.SetupSequence(Function(m) m.Read()).Returns(True).Returns(False)
                        mockDataReader.Setup(Function(m) m("BeginDate")).Returns(Now.Date.AddYears(-1))
                        mockDataReader.Setup(Function(m) m("EndDate")).Returns(Now.Date.AddYears(1))

                        ' Retornar el mock de DbDataReader
                        Return mockDataReader.Object
                    End Function
    End Sub

    Public Sub ExecuteSqlSpy()
        Robotics.DataLayer.Fakes.ShimAccessHelper.ExecuteSqlStringroBaseConnection =
                    Function(strSQL As String, oConn As roBaseConnection)
                        ExecuteSqlToCount(strSQL)
                        ExecuteSqlWasCalled = SqlExecuteString.GeneralExecute
                        If Regex.IsMatch(strSQL.ToLower, "\bdelete\b.*\bpunchescaptures\b") Then
                            ExecuteSqlWasCalled = SqlExecuteString.DeletePunchesCaptures
                            Return True
                        End If
                        If Regex.IsMatch(strSQL.ToLower, "\bdelete\b.*\btmpholidayscontrolbycontract\b") Then
                            ExecuteSqlWasCalled = SqlExecuteString.DeleteTmpHolidaysControlBycontract
                            Return True
                        End If
                        If Regex.IsMatch(strSQL, "\bINSERT\b.*\bsysroNotificationtasks\b") OrElse Regex.IsMatch(strSQL, "\bINSERT\b.*\bsysroNotificationTasks\b") Then
                            ExecuteSqlWasCalled = SqlExecuteString.InsertNotification
                            Return True
                        End If
                        If Regex.IsMatch(strSQL, "\bUPDATE\b.*\bsysroNotificationTasks\b.*") Then
                            ExecuteSqlWasCalled = SqlExecuteString.UpdateNotification
                            Return True
                        End If
                        If Regex.IsMatch(strSQL, "\bINSERT\b.*\bINTO Channels\b.*") Then
                            ExecuteSqlWasCalled = SqlExecuteString.ChannelCreated
                            Return True
                        End If
                        If Regex.IsMatch(strSQL, "\bINSERT\b.*\bINTO ChannelEmployees\b.*") Then
                            ExecuteSqlWasCalled = SqlExecuteString.ChannelEmployeesInserted
                            Return True
                        End If
                        If Regex.IsMatch(strSQL, "\bINSERT\b.*\bINTO ChannelGroups\b.*") Then
                            ExecuteSqlWasCalled = SqlExecuteString.ChannelGroupsInserted
                            Return True
                        End If
                        If Regex.IsMatch(strSQL, "\bINSERT\b.*\bINTO ChannelSupervisors\b.*") Then
                            ExecuteSqlWasCalled = SqlExecuteString.ChannelSupervisorsInserted
                            Return True
                        End If
                        If Regex.IsMatch(strSQL, "\bUPDATE\b.*\bChannels\b.*") Then
                            ExecuteSqlWasCalled = SqlExecuteString.ChannelUpdated
                            Return True
                        End If
                        If Regex.IsMatch(strSQL, "\bDELETE\b.*\bChannels\b.*") Then
                            ExecuteSqlWasCalled = SqlExecuteString.ChannelDeleted
                            Return True
                        End If
                        If Regex.IsMatch(strSQL, "\bDELETE\b.*\bChannelConversationMessages\b.*") Then
                            ExecuteSqlWasCalled = SqlExecuteString.ChannelConversationMessagesDeleted
                            Return True
                        End If
                        If Regex.IsMatch(strSQL, "\bDELETE\b.*\bChannelConversations\b.*") Then
                            ExecuteSqlWasCalled = SqlExecuteString.ChannelConversationsDeleted
                            Return True
                        End If
                        If Regex.IsMatch(strSQL, "\bINSERT\b.*\bTerminalsSyncTasks\b.*") Then
                            ExecuteSqlWasCalled = SqlExecuteString.BroadcasterAddTask
                            ExecuteSqlHistory.Add(New ExecuteSqlItem With {.SqlExecuteString = SqlExecuteString.BroadcasterAddTask, .SqlString = strSQL})
                            Return True
                        End If
                        If Regex.IsMatch(strSQL, "\bDELETE\b.*\bTerminalsSyncTasks\b.*") Then
                            ExecuteSqlWasCalled = SqlExecuteString.BroadcasterDeleteTask
                            ExecuteSqlHistory.Add(New ExecuteSqlItem With {.SqlExecuteString = SqlExecuteString.BroadcasterDeleteTask, .SqlString = strSQL})
                            Return True
                        End If
                    End Function
    End Sub

    Public Shared Function ExecuteSqlCallCountTotal(sqlString As String) As Integer
        Return If(ExecuteSqlCallCount.ContainsKey(sqlString), ExecuteSqlCallCount(sqlString), 0)
    End Function

    Public Shared Function ExecuteSqlToCount(sqlString As String) As Boolean
        Dim lowerSqlString = sqlString.ToLower()
        If Regex.IsMatch(lowerSqlString, "\bdelete\b.*\bchannelconversationmessages\b.*") Then
            Dim auxKey As String = "DeleteChannelConversationMessages"
            If Not ExecuteSqlCallCount.ContainsKey(auxKey) Then
                ExecuteSqlCallCount(auxKey) = 0
            End If
            ExecuteSqlCallCount(auxKey) += 1
            Return True
        End If
        If Regex.IsMatch(lowerSqlString, "\bdelete\b.*\bchannelconversations\b.*") Then
            Dim auxKey As String = "DeleteChannelConversations"
            If Not ExecuteSqlCallCount.ContainsKey(auxKey) Then
                ExecuteSqlCallCount(auxKey) = 0
            End If
            ExecuteSqlCallCount(auxKey) += 1
            Return True
        End If
        Return True
    End Function

    Public Sub CreateDataTableSpy()
        Robotics.DataLayer.Fakes.ShimAccessHelper.CreateDataTableStringString =
                    Function(strSQL As String, tablename As String) As DataTable
                        EvaluateDatatableSpy(strSQL)
                    End Function

        Robotics.DataLayer.Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString =
                    Function(strSQL As String, oCn As roBaseConnection, tablename As String) As DataTable
                        EvaluateDatatableSpy(strSQL)
                    End Function
    End Sub

    Private Sub EvaluateDatatableSpy(ByVal strSQL As String)
        If Regex.IsMatch(strSQL.ToLower, "\bFrom sysroNotificationTasks\b.*\bIDNotification=20\b") Then
            ExecuteSqlWasCalled = CreateDataTableString.SelectUnreliablePunchNotifications
        ElseIf Regex.IsMatch(strSQL, "\bSELECT\b.*\bsysroPassports WHERE IDUser=\b.*") Then
            ExecuteSqlWasCalled = CreateDataTableString.LoadPassportByUser
        ElseIf Regex.IsMatch(strSQL, "\bSELECT\b.*\bsysroPassports WHERE IDEmployee=\b.*") Then
            ExecuteSqlWasCalled = CreateDataTableString.LoadPassportByEmployee
        ElseIf Regex.IsMatch(strSQL, "\bSELECT\b.*\bsysroPassports WHERE ID=\b.*") Then
            ExecuteSqlWasCalled = CreateDataTableString.LoadPassport
        Else
            ExecuteSqlWasCalled = CreateDataTableString.GeneralCreateDatatable
        End If
    End Sub

    Public Sub CreateDataTableStub(defaultValues As Dictionary(Of String, Dictionary(Of String, Object)()))
        Robotics.DataLayer.Fakes.ShimAccessHelper.CreateDataTableStringString =
                    Function(strSQL As String, tablename As String) As DataTable
                        Return CreateDataTableFromSql(strSQL, defaultValues)
                    End Function

        Robotics.DataLayer.Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString =
                    Function(strSQL As String, oCn As roBaseConnection, tablename As String) As DataTable
                        Return CreateDataTableFromSql(strSQL, defaultValues)
                    End Function
    End Sub

    Private Function CreateDataTableFromSql(strSQL As String, defaultValues As Dictionary(Of String, Dictionary(Of String, Object)())) As DataTable
        Dim dt As New DataTable()

        ' Extraer los campos de la consulta SQL
        Dim fields As String = strSQL.ToLower().Substring(strSQL.ToLower().IndexOf("@select#") + 8, strSQL.ToLower().IndexOf("from") - strSQL.ToLower().IndexOf("@select#") - 8)
        Dim fieldList As String() = fields.Split(","c)
        Dim defaultValueKeysAsFields As Boolean = False

        Dim tableName As String = strSQL.ToLower().Substring(strSQL.ToLower().IndexOf("from") + 4)
        tableName = tableName.Trim()
        tableName = tableName.Substring(0, tableName.IndexOf(" ")).Trim()

        ' Agregar columnas al DataTable para cada campo
        For Each field As String In fieldList
            ' Extraer el nombre del campo
            Dim fieldName As String = field.Trim()

            If fieldName.Contains("*") Then
                defaultValueKeysAsFields = True
                Continue For
            End If

            If fieldName.Contains(".") Then fieldName = fieldName.Substring(fieldName.IndexOf(".") + 1).Trim()
            If fieldName.Contains(" as ") Then fieldName = field.Substring(field.IndexOf("as") + 2).Trim()

            Dim defaultType As Type = GetType(String)

            If defaultValues.ContainsKey(tableName) Then
                For Each defaultrow As Dictionary(Of String, Object) In defaultValues(tableName)
                    Dim values As Dictionary(Of String, Object) = defaultrow

                    For Each defaultField As String In values.Keys
                        If defaultField.ToLower = fieldName Then
                            defaultType = values(defaultField).GetType()
                            If defaultType Is GetType(DBNull) Then
                                defaultType = GetType(String)
                            End If
                        End If
                    Next
                Next
            End If


            dt.Columns.Add(New DataColumn(fieldName, defaultType))
        Next

        If defaultValues.ContainsKey(tableName) Then

            For Each defaultrow As Dictionary(Of String, Object) In defaultValues(tableName)
                Dim values As Dictionary(Of String, Object) = defaultrow

                If defaultValueKeysAsFields Then
                    For Each kvp As KeyValuePair(Of String, Object) In values
                        If Not dt.Columns.Contains(kvp.Key) Then
                            Dim defaultType As Type = kvp.Value.GetType()
                            dt.Columns.Add(New DataColumn(kvp.Key, defaultType))
                        End If
                    Next
                End If


                Dim row As DataRow = dt.NewRow()

                For Each kvp As KeyValuePair(Of String, Object) In values
                    row(kvp.Key) = kvp.Value
                Next
                dt.Rows.Add(row)
            Next
        End If

        Return dt
    End Function

    Public Sub CreateDataTableSpy(regexSQL As String)
        Robotics.DataLayer.Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString =
                    Function(strSQL As String, oCOn As roBaseConnection, tablename As String) As DataTable
                        If Regex.IsMatch(strSQL.ToLower, regexSQL.ToLower) Then
                            ExecuteSqlWasCalled = CreateDataTableString.GeneralCreateDatatable
                        End If
                    End Function
    End Sub


    Public Sub CreateExecuteSQLSpy(regexSQL As String)
        Robotics.DataLayer.Fakes.ShimAccessHelper.ExecuteSqlStringListOfCommandParameterroBaseConnectionBooleanNullableOfInt32 =
                    Function(strSQL As String, parameters As List(Of CommandParameter), oCOn As roBaseConnection, returnIdentity As Boolean, timeout As Nullable(Of Integer)) As Boolean
                        If Regex.IsMatch(strSQL.ToLower, regexSQL.ToLower) Then
                            ExecuteSqlWasCalled = SqlExecuteString.QueryUsersWithPermissions
                        End If
                        Return True
                    End Function
    End Sub

    Public Sub CreateDataTableSpyStSt(regexSQL As String)
        Robotics.DataLayer.Fakes.ShimAccessHelper.CreateDataTableStringString =
                    Function(strSQL As String, tablename As String) As DataTable
                        If strSQL.ToLower.Contains(regexSQL.ToLower) Then
                            ExecuteSqlWasCalled = CreateDataTableString.GeneralCreateDatatable
                        End If
                    End Function
    End Sub

    Function CreateDataTableMock(columns As String(), values As Object()())
        Dim stubDataTable As New System.Data.DataTable
        For Each columnName As String In columns
            stubDataTable.Columns.Add(columnName, GetType(Object))
        Next

        For Each rowValues As Object() In values
            stubDataTable.Rows.Add(rowValues)
        Next
        Return stubDataTable
    End Function

    Function CreateDataTableStub(dTableMocks As Dictionary(Of String, DataTableMock)) As DataTable
        Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString =
                    Function(strSQL As String, oCOn As roBaseConnection, tablename As String) As DataTable
                        For Each kvp As KeyValuePair(Of String, DataTableMock) In dTableMocks
                            'If strSQL.ToLower.Contains(kvp.Key.ToLower) Then
                            If Regex.IsMatch(strSQL.Replace("[", "").Replace("]", "").ToLower, kvp.Key.Replace("[", "").Replace("]", "").ToLower) Then
                                Return CreateDataTableMock(kvp.Value.columns, kvp.Value.values)
                            End If
                        Next
                    End Function
    End Function

    Function CreateDataTableStubExt(dTableMocks As Dictionary(Of String, DataTableMock)) As DataTable
        Fakes.ShimAccessHelper.CreateDataTableStringString =
                    Function(strSQL As String, tablename As String) As DataTable
                        For Each kvp As KeyValuePair(Of String, DataTableMock) In dTableMocks
                            'If strSQL.ToLower.Contains(kvp.Key.ToLower) Then
                            If Regex.IsMatch(strSQL.Replace("[", "").Replace("]", "").ToLower, kvp.Key.Replace("[", "").Replace("]", "").ToLower) Then
                                Return CreateDataTableMock(kvp.Value.columns, kvp.Value.values)
                            End If
                        Next
                    End Function
    End Function

    Function CreateDataTableWithoutTimeoutsStub(dTableMocks As Dictionary(Of String, DataTableMock)) As DataTable
        Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString =
                    Function(strSQL As String, oCOn As roBaseConnection, tablename As String) As DataTable
                        For Each kvp As KeyValuePair(Of String, DataTableMock) In dTableMocks
                            If strSQL.ToLower.Contains(kvp.Key.ToLower) Then
                                Return CreateDataTableMock(kvp.Value.columns, kvp.Value.values)
                            End If
                        Next
                    End Function
    End Function

    Public Sub CreateDataTableWithoutTimeoutsSpy()
        Robotics.DataLayer.Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString =
                    Function(strSQL As String, oCOn As roBaseConnection, tablename As String) As DataTable
                        If Regex.IsMatch(strSQL.ToLower, "from deletedprogrammedholidays") Then
                            ExcutedCreateDataTable = CreateDataTableString.SelectDeletedHolidays
                        End If
                    End Function
    End Sub

    Public Sub CreateDataTableWithoutTimeoutsSpy(regexSQL As String)
        Robotics.DataLayer.Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString =
                    Function(strSQL As String, oCOn As roBaseConnection, tablename As String) As DataTable
                        If Regex.IsMatch(strSQL.ToLower, regexSQL) Then
                            ExecuteSqlWasCalled = CreateDataTableString.GeneralCreateDatatable
                        End If
                    End Function
    End Sub

    Function GetTableNameFromQuery(strQuery As String) As String
        Return Regex.Match(strQuery, "FROM\s+((\w+\.)?\w+)", RegexOptions.IgnoreCase).Groups(1).Value
    End Function

    Function QueryContains(strQuery As String, strToFind As String) As Boolean
        Return Regex.IsMatch(strQuery, strToFind, RegexOptions.IgnoreCase)
    End Function

    Function StartTransaction()
        Fakes.ShimAccessHelper.StartTransactionIsolationLevel =
                    Function(isolationLevel As IsolationLevel)
                        Return False
                    End Function
    End Function

    Function EndTransaction()
        Fakes.ShimAccessHelper.EndCurrentTransactionBooleanBoolean =
                    Function(commit As Boolean, closeConnection As Boolean)
                        Return False
                    End Function
    End Function


    Function CreateDataAdapterMock()
        Fakes.ShimAccessHelper.CreateDataAdapterDbCommandBoolean =
            Function(dbCommand As DbCommand, returnIdentity As Boolean) As DbDataAdapter
                Dim mockDataAdapter As New Mock(Of DbDataAdapter)()
                
                mockDataAdapter.Setup(Function(m) m.Fill(It.IsAny(Of DataTable))).Returns(1)

                ' Retornar el mock de DbDataAdapter
                Return mockDataAdapter.Object
            End Function
    End Function




End Class

Public Class DataTableMock
    Property columns As String()
    Property values As Object()()
End Class

Public Class ExecuteSqlItem
    Property SqlExecuteString As DatalayerHelper.SqlExecuteString
    Property SqlString As String
End Class