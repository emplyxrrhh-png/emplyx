Imports System.Data.Common
Imports Robotics.DataLayer

Namespace DataAccess

    Friend NotInheritable Class FeaturesAccess

        Public Shared Sub GetFeaturesByType(ByVal table As DataTable, ByVal featureType As String)

            Try

                Dim Command As DbCommand = AccessHelper.CreateCommand("WebLogin_Features_SelectByType")
                Command.CommandType = CommandType.StoredProcedure
                AccessHelper.AddParameter(Command, "@featureType", DbType.AnsiString, 1).Value = featureType
                Dim Adapter As DbDataAdapter = AccessHelper.CreateDataAdapter(Command)
                Adapter.Fill(table)
            Finally

            End Try
        End Sub

        Public Shared Sub GetFeaturesByAlias(ByVal table As DataTable, ByVal featureAlias As String, ByVal featureType As String)

            Try

                Dim Command As DbCommand = AccessHelper.CreateCommand("WebLogin_Features_SelectByAlias")
                Command.CommandType = CommandType.StoredProcedure
                AccessHelper.AddParameter(Command, "@featureAlias", DbType.AnsiString, 50).Value = featureAlias
                AccessHelper.AddParameter(Command, "@featureType", DbType.AnsiString, 1).Value = featureType
                Dim Adapter As DbDataAdapter = AccessHelper.CreateDataAdapter(Command)
                Adapter.Fill(table)
            Finally

            End Try
        End Sub

        Public Shared Function GetFeatureIdByAlias(ByVal featureAlias As String, ByVal featureType As String) As Nullable(Of Integer)

            Try

                Dim Command As DbCommand = AccessHelper.CreateCommand("WebLogin_Features_SelectByAlias")
                Command.CommandType = CommandType.StoredProcedure
                AccessHelper.AddParameter(Command, "@featureAlias", DbType.AnsiString, 50).Value = featureAlias
                AccessHelper.AddParameter(Command, "@featureType", DbType.AnsiString, 1).Value = featureType
                Dim Result As Nullable(Of Integer)
                ''Dim ResultCmd As Object = Command.ExecuteScalar()
                ''If ResultCmd IsNot DBNull.Value Then
                ''    Result = CInt(ResultCmd)
                ''End If
                Dim Reader As DbDataReader = Command.ExecuteReader()
                If (Reader.Read) Then
                    Result = Reader.GetInt32(0)
                End If
                Reader.Close()
                Return Result
            Finally

            End Try
        End Function

        Public Shared Sub GetFeaturesById(ByVal table As DataTable, ByVal idFeature As Integer)

            Try

                Dim Command As DbCommand = AccessHelper.CreateCommand("WebLogin_Features_SelectById")
                Command.CommandType = CommandType.StoredProcedure
                AccessHelper.AddParameter(Command, "@idFeature", DbType.Int32).Value = idFeature
                Dim Adapter As DbDataAdapter = AccessHelper.CreateDataAdapter(Command)
                Adapter.Fill(table)
            Finally

            End Try
        End Sub

        Public Shared Sub GetApplications(ByVal table As DataTable)

            Try

                Dim Command As DbCommand = AccessHelper.CreateCommand("WebLogin_Features_SelectApplications")
                Command.CommandType = CommandType.StoredProcedure
                Dim Adapter As DbDataAdapter = AccessHelper.CreateDataAdapter(Command)
                Adapter.Fill(table)
            Finally

            End Try
        End Sub

    End Class

End Namespace